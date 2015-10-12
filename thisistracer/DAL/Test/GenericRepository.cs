using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace thisistracer.DAL.Test {
    public abstract class GenericRepository<C, T> : IGenericRepository<T> where T : class where C : DbContext, new() {
                
        private C _entities = new C();
        public C Context {
            get { return _entities; }
            set { _entities = value; }
        }

        public virtual IQueryable<T> GetAll() {
            throw new NotImplementedException();
        }

        public IQueryable<T> FindBy(Expression<Func<T, bool>> predicate) {
            throw new NotImplementedException();
        }

        public virtual void Add(T entity) {
            throw new NotImplementedException();
        }

        public virtual void Delete(T entity) {
            throw new NotImplementedException();
        }

        public virtual void Edit(T entity) {
            throw new NotImplementedException();
        }

        public virtual void Save() {
            throw new NotImplementedException();
        }

        #region Dispose
        private bool disposed = false;

        protected virtual void Dispose(bool disposing) {

            if (!this.disposed)
                if (disposing)
                    _entities.Dispose();

            this.disposed = true;
        }

        public void Dispose() {

            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
