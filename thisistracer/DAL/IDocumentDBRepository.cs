using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace thisistracer.DAL {
    public interface IDocumentDBRepository<T> {
        Database ReadOrCreateDatabase();
        DocumentCollection ReadOrCreateCollection(string databaseLink);

        IEnumerable<T> GetItems(Expression<Func<T, bool>> predicate);
        IEnumerable<T> GetItems(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null);

        Task<Document> CreateItemAsync(T item);
        T GetItem(Expression<Func<T, bool>> predicate);
        Task<Document> UpdateItemAsync(string id, T item);
        Document GetDocument(string id);
    }
}
