using InvoiceGenerator;
using InvoiceGenerator.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Repository
{
    // Generic repository interface
    public interface IRepository<T> where T : class
    {
        // T GetRecord(Guid Id);
        // IEnumerable<T> GetAllRecords();       
        // IEnumerable<T> FindRecords(Expression<Func<T, bool>> predicate);
        void AddRecord(T entity);
        void AddRecords(IEnumerable<T> entities);
        void RemoveRecord(T entity);
        void RemoveRecords(IEnumerable<T> entities);
    }

    // generic Repository
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly DbContext _Dbcontext;
        protected readonly ApplicationUser _applicationUser;
        public Repository(DbContext Dbcontext, ApplicationUser applicationUser)
        {
            _Dbcontext = Dbcontext;
            _applicationUser = applicationUser;
        }
        public void AddRecord(T entity)
        {
            _Dbcontext.Set<T>().Add(entity);
        }
        public void AddRecords(IEnumerable<T> entities)
        {
            _Dbcontext.Set<T>().AddRange(entities);
        }
        public IEnumerable<T> FindRecords(Expression<Func<T, bool>> predicate)
        {
            return _Dbcontext.Set<T>().Where(predicate);
        }
        public T FindRecord(Expression<Func<T, bool>> predicate)
        {
            T result;
            try
            {
                result = _Dbcontext.Set<T>().Where(predicate).Single();
            }
            catch
            {
                return null;
            }

            return result;
        }
        private IEnumerable<T> GetAllRecords()
        {
            IEnumerable<T> result;
            try
            {
                result = _Dbcontext.Set<T>().ToList();
            }
            catch
            {
                return null;
            }
            return result;
        }
        public T GetRecord(Guid Id)
        {
            T result;
            try
            {
                result = _Dbcontext.Set<T>().Find(Id);
            }
            catch
            {
                return null;
            }
            return result;
        }
        public void RemoveRecord(T entity)
        {
            _Dbcontext.Set<T>().Remove(entity);
        }
        public void RemoveRecords(IEnumerable<T> entities)
        {
            _Dbcontext.Set<T>().RemoveRange(entities);
        }
    }
}
