
using InvoiceGenerator;
using InvoiceGenerator.Data;
using InvoiceGenerator.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repository
{

    public interface IUnitofWork : IDisposable
    {
        IInvoice Invoices { get; }

        string Complete();       
    }

    public class UnitofWork : IUnitofWork
    {
        private readonly ApplicationDbContext _DbContext;
        public ILoggerFactory LoggerFactory { get; }
        public ILogger  Logger { get; }

        private ApplicationUser _applicationUser;
        public UnitofWork(ApplicationDbContext DbContext, ApplicationUser User,ILoggerFactory loggerFactory)
        {
            _DbContext = DbContext;
            _applicationUser = User;
            LoggerFactory = loggerFactory;
            Logger = LoggerFactory.CreateLogger<Program>();
            Invoices = new Invoice(_DbContext, _applicationUser);
        }

        public IInvoice Invoices { get; private set; }
       

        public string Complete()
        {

            Logger.LogInformation(" message goes here ");

            try {
            
                var changesMade = _DbContext.SaveChanges();
                
     
                if (changesMade > 0)
                {
                    return " Changes made : " + changesMade.ToString();
                }
                else 
                {
                    return "No changes made";                
                };

            }
            catch (DbUpdateException ex) 
            {
                return "Failure " +  ex.Message;
            }
        }
        public void Dispose()
        {
            _DbContext.Dispose();
        }
    }
}
