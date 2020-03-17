
using InvoiceGenerator.Data;
using InvoiceGenerator.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InvoiceGenerator.Repository
{
    public interface IInvoice : IRepository<tblInvoice>
    {
        tblInvoice GetInvoicebyId(string Id);
        tblInvoice GetInvoicebyCode(string Code);
        List<tblInvoice> GetAllInvoices();
    }
    public class Invoice :IInvoice
    {
        public ApplicationDbContext _context { get; }
        public ApplicationUser _applicationUser { get; }
        public Invoice(ApplicationDbContext Context, ApplicationUser applicationUser)
        {
            _context = Context;
            _applicationUser = applicationUser;
        }
        public tblInvoice GetInvoicebyId(string Id)
        {
            // return Invoice if it belongs to the application user, default to null if not found.  
            var Invoice = _context.Invoices
                .Include( x =>x.Items)
                .Where(x => x.Id.ToString() == Id && x.ApplicationUserId == _applicationUser.Id)
                .FirstOrDefault();
            return Invoice;
        }
        public tblInvoice GetInvoicebyCode(string Code)
        {
            // return Invoice if it belongs to the application user, default to null if not found.  
            var Invoice = _context.Invoices
                .Where(x => x.Code == Code && x.ApplicationUserId == _applicationUser.Id)
                .FirstOrDefault(null);
            return Invoice;
        }
        public List<tblInvoice> GetAllInvoices()
        {
            var Invoice = _context.Invoices
               .Where(x => x.ApplicationUserId == _applicationUser.Id)
               .ToList();
            return Invoice;
        }
        public void AddRecord(tblInvoice entity)
        {
            _context.Invoices.Add(entity);
        }
        public void AddRecords(IEnumerable<tblInvoice> entities)
        {
            _context.Invoices.AddRange(entities);
        }
        public void RemoveRecord(tblInvoice entity)
        {
            _context.Invoices.Remove(entity);
        }
        public void RemoveRecords(IEnumerable<tblInvoice> entities)
        {
            _context.Invoices.RemoveRange(entities);
        }


    }
}
