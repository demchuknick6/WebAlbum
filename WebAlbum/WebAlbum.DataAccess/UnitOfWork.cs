using System;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Threading.Tasks;
using WebAlbum.DomainServices;

namespace WebAlbum.DataAccess
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DatabaseContext _db;

        public UnitOfWork(DatabaseContext db) => _db = db;

        public int Save()
        {
            try
            {
                return _db.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                        Trace.TraceInformation("Property: {0} Error: {1}", 
                            validationError.PropertyName, validationError.ErrorMessage);
                }
                throw;
            }
        }

        public async Task<int> SaveAsync()
        {
            try
            {
                return await _db.SaveChangesAsync();
            }
            catch (DbEntityValidationException dbEx) // verbose failure message
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
                throw;
            }
        }

        private bool _disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    _db.Dispose();
                }
            }
            this._disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
