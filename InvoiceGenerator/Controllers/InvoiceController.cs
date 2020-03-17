using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using InvoiceGenerator.Data;
using InvoiceGenerator.Data.Entities;
using InvoiceGenerator.Models;
using InvoiceGenerator.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Repository;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace InvoiceGenerator.Controllers
{
    [Authorize]
    public class InvoiceController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ApplicationDbContext _applicationDBcontext;
        private readonly IMapper _mapper;
        private readonly ILoggerFactory loggerfactory;
        private readonly ILogger logger;
        private readonly AppSettings _appSettings;
        public InvoiceController(UserManager<ApplicationUser> userManager,
                                        SignInManager<ApplicationUser> signInManager,
                                        RoleManager<ApplicationRole> roleManager,
                                        ApplicationDbContext applicationDBcontext,
                                      IMapper mapper,
                                      ILoggerFactory loggerfactory,
                                      AppSettings Settings
        )

        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _applicationDBcontext = applicationDBcontext;
            _mapper = mapper;
            _appSettings = Settings;
            this.loggerfactory = loggerfactory;

            logger = loggerfactory.CreateLogger<Program>();
        }
        public InvoiceVewModel TallyInvoice(InvoiceVewModel model)
        {
            decimal Price = 0.0m;
            decimal Tax = 0.0m;
            for (var ct = 0; ct < model.Items.Count; ct++)
            {
                Price += decimal.Parse(model.Items[ct].Price);
                Tax += decimal.Parse(model.Items[ct].Tax);
                 model.Items[ct].Total = (decimal.Parse(model.Items[ct].Price) + decimal.Parse(model.Items[ct].Tax)).ToString();
            }
            model.Price = Price.ToString();
            model.Tax = Tax.ToString();
            model.Total = (Price + Tax).ToString();
            return model;
        }
        [HttpPost] 
        public IActionResult AddItem(InvoiceVewModel model)
        {  
            if (ModelState.IsValid)
            {
                model.Items.Add(new InvoiceItemViewModel() { InvoiceId = model.Id, Id = Guid.NewGuid().ToString(), RowVersion="" , Item = "", Price = "0.0", Tax = "0.0", Total = "0.0" });
                model = TallyInvoice(model);
            }
            ViewData["Action"] = "Edit";
            return View("Invoice", model);
        }
        [HttpGet]
        public IActionResult Index()
        {
            
            ApplicationUser Usr = _userManager.GetUserAsync(User).Result;
            var _unitOfWork = new UnitofWork(_applicationDBcontext, Usr, loggerfactory);
            InvoiceIndexVewModel model = new InvoiceIndexVewModel();
            var Invoices = _unitOfWork.Invoices.GetAllInvoices();
            model.Items = _mapper.Map<List<tblInvoice>, List<InvoiceIndexItemVewModel>>(Invoices);
            return View("Index", model);
        }
        [HttpGet]
        public IActionResult IndexMsg(string Message)
        {

            ApplicationUser Usr = _userManager.GetUserAsync(User).Result;
            var _unitOfWork = new UnitofWork(_applicationDBcontext, Usr, loggerfactory);
            InvoiceIndexVewModel model = new InvoiceIndexVewModel();
            var Invoices = _unitOfWork.Invoices.GetAllInvoices();
            model.Items = _mapper.Map<List<tblInvoice>, List<InvoiceIndexItemVewModel>>(Invoices);
            model.Message = Message;
            return View("Index", model);
        }
        [HttpGet]
        public IActionResult RaiseInvoice()
        {
            InvoiceVewModel model = new InvoiceVewModel();
            model.ApplicationUserId = _userManager.GetUserAsync(User).Result.Id.ToString();
            model.Date = DateTime.Now.ToString("dd/mmm/yyyy HH:mm");
            model.From = "";
            model.Id = Guid.NewGuid().ToString();
            model.Status = status.Open;
            ViewData["Action"] = "Edit";
            return View("Invoice", model);
        }
        [HttpPost]
        public async Task<IActionResult> SaveInvoiceAsync(InvoiceVewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser Usr = await _userManager.GetUserAsync(User);
                var _unitOfWork = new UnitofWork(_applicationDBcontext, Usr, loggerfactory);
                var Exists = _unitOfWork.Invoices.GetInvoicebyId(model.Id);

                if (Exists != null)
                {
                    // need to dig in to automapper on this lot more complex than it looks.
                    Exists.Code = model.Code;
                    Exists.Date = model.Date;
                    Exists.From = model.From;
                    Exists.Note = model.Note;
                    Exists.Price = model.Price;
                    Exists.Tax = model.Tax;
                    Exists.To = model.To;
                    Exists.AttentionOf = model.AttentionOf;
                    Exists.ToEmail = model.ToEmail;
                    Exists.Total = model.Total;                   
                    // as the logic to update and add Items to the Invoice is complex decided to clear the list and replaces it with the 
                    // one in the  model.  EF seems happy with this approach which it was not when modifying/adding Items to existing list.
                    Exists.Items.Clear();
                    for (int ct = 0; ct < model.Items.Count; ct++)
                    {
                        Exists.Items.Add(new tblInvoiceItems()
                        {
                            Item = model.Items[ct].Item,
                            Price = model.Items[ct].Price,
                            Tax = model.Items[ct].Tax,
                            Total = model.Items[ct].Total,
                        });
                    }   
                }
                else 
                { 
                 var ToSave = _mapper.Map<InvoiceVewModel, tblInvoice>(model);
                 _unitOfWork.Invoices.AddRecord(ToSave);
                }
                var result = _unitOfWork.Complete();
                if (result.StartsWith("Failure "))
                {
                    ModelState.AddModelError("", result);
                    //ModelState.AddModelError("", "Invoice not saved, Error recieved on posting Invoice to Database. ");
                }
                else { ModelState.AddModelError("", "Invoice saved"); }
            }
            ViewData["Action"] = "Edit";
            return View("Invoice", model);
        }
        [HttpPost]
        public IActionResult RemoveItem(InvoiceVewModel model, string ItemId)
        {
            if (ModelState.IsValid)
            {
                model.Items = model.Items.Where(x => x.Id != ItemId).ToList();
                ModelState.AddModelError("", "Removed Item, Save to update database else changes will be lost. ");
                model = TallyInvoice(model);
            }
            ViewData["Action"] = "Edit";
            return View("Invoice", model);
        }
        [HttpPost]
        public IActionResult ProcessInvoice(InvoiceVewModel model)
        {
            if (ModelState.IsValid)
            {
                model = TallyInvoice(model);
            }
            ViewData["Action"] = "Edit";
            return View("Invoice", model);
        }
        [HttpGet]
        public IActionResult EditInvoice(string Id)
        {
            ApplicationUser Usr = _userManager.GetUserAsync(User).Result;
            var unitOfWork = new UnitofWork(_applicationDBcontext, Usr, loggerfactory);
            var result = unitOfWork.Invoices.GetInvoicebyId(Id);
            var model = _mapper.Map<tblInvoice, InvoiceVewModel>(result);
            ViewData["Action"] = "View";
            if (model.Status == status.Open)
            {
                ViewData["Action"] = "Edit";
            }
            return View("Invoice", model);
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ViewInvoice(string Id)
        {
            var result = _applicationDBcontext.Invoices
                .Include(x => x.Items)
                .Where(x => x.Id.ToString() == Id)
                .FirstOrDefault();
            var model = _mapper.Map<tblInvoice, InvoiceVewModel>(result);
            ViewData["Action"] = "View";
            return View("Invoice", model);
        }
        public IActionResult DeleteInvoice(string Id)
        {
            ApplicationUser Usr = _userManager.GetUserAsync(User).Result;
            var unitOfWork = new UnitofWork(_applicationDBcontext, Usr, loggerfactory);
            var result = unitOfWork.Invoices.GetInvoicebyId(Id);
            if (result.Status == status.Open)
            {
                unitOfWork.Invoices.RemoveRecord(result);
                var x = unitOfWork.Complete();
                return RedirectToAction("IndexMsg", new { Message = "Invoice "+ result.Code +  " removed" });
            }
            return  RedirectToAction("IndexMsg",new {Message="Unable to remove Invoice " + result.Code + " Status is not Open. "  }); 
        }
        [HttpGet]
        public async Task<IActionResult> EmailInvoiceAsync(string Id)
        {
            ApplicationUser Usr = _userManager.GetUserAsync(User).Result;
            var unitOfWork = new UnitofWork(_applicationDBcontext, Usr, loggerfactory);
            var result = unitOfWork.Invoices.GetInvoicebyId(Id);
            var model = _mapper.Map<tblInvoice, InvoiceVewModel>(result);

            model.OrganisationEmail = Usr.OrganisationEmail;
            model.OrganisationName = Usr.OrganisationName;
            model.Person = Usr.Person;
            model.AddressCode = Usr.AddressCode;
            model.AddressL1 = Usr.AddressL1;
            model.AddressL2 = Usr.AddressL2;
            model.AddressL3 = Usr.AddressL3;
            model.AddressL4 = Usr.AddressL4;
            model.AddressL5 = Usr.AddressL5;
            model.InvoiceUrl = Url.Action("ViewInvoice", "Invoice",new {Id=model.Id },Request.Scheme);
            var HTMLstring = await this.RenderViewAsync<InvoiceVewModel>("InvoiceEmail", model, true);
            //StreamWriter sw = new StreamWriter("C:\\Users\\Carl\\Emailtest.html");
            //sw.Write(HTMLstring);
            //sw.Close();
            var PostEmail = new SendGridClient(_appSettings.SendGridKey);
            var From = new EmailAddress(Usr.Email, Usr.UserName);
            var Replyto = new EmailAddress(Usr.OrganisationEmail, Usr.OrganisationName);
            var To = new EmailAddress(result.ToEmail, result.To);
            var Subject = "Invoice :" + model.Code + " from :" + model.OrganisationName + " Date :" + model.Date; 
            var Mail = MailHelper.CreateSingleEmail(From, To, Subject, "", HTMLstring);
            var response = await PostEmail.SendEmailAsync(Mail);
            result.Status = status.Sent;
            unitOfWork.Complete();
            return RedirectToAction("Index");
        }
    }
}