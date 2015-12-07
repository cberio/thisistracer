using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using System.Linq.Expressions;

namespace thisistracer.DAL.Home {
    public interface ITableStorage<T> where T: ITableEntity {
        void Insert(T item);
        void Insert(IEnumerable<T> items);
        IEnumerable<TElement> GetItems<TElement>(Expression<Func<TElement, bool>> predicate) where TElement : ITableEntity, new();
        T GetItem(Expression<Func<T, bool>> predicate);
        T GetItem(string partitionKey, string rowKey);
    }
}
