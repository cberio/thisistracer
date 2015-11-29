using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace thisistracer.DAL {
    public class DocumentDBRepository<T> : IDocumentDBRepository<T> {
        //Use the Database if it exists, if not create a new Database
        public Database ReadOrCreateDatabase() {
            var db = Client.CreateDatabaseQuery()
                            .Where(d => d.Id == DatabaseId)
                            .AsEnumerable()
                            .FirstOrDefault();

            if (db == null) {
                db = Client.CreateDatabaseAsync(new Database { Id = DatabaseId }).Result;
            }

            return db;
        }

        //Use the DocumentCollection if it exists, if not create a new Collection
        public DocumentCollection ReadOrCreateCollection(string databaseLink) {
            var col = Client.CreateDocumentCollectionQuery(databaseLink)
                              .Where(c => c.Id == CollectionId)
                              .AsEnumerable()
                              .FirstOrDefault();

            if (col == null) {
                var collectionSpec = new DocumentCollection { Id = CollectionId };
                var requestOptions = new RequestOptions { OfferType = "S1" };

                col = Client.CreateDocumentCollectionAsync(databaseLink, collectionSpec, requestOptions).Result;
            }

            return col;
        }

        //Expose the "database" value from configuration as a property for internal use
        private string databaseId;
        private String DatabaseId {
            get {
                if (string.IsNullOrEmpty(databaseId)) {
                    databaseId = ConfigurationManager.AppSettings["database"];
                }

                return databaseId;
            }
        }

        //Expose the "collection" value from configuration as a property for internal use
        private string collectionId;
        private String CollectionId {
            get {
                if (string.IsNullOrEmpty(collectionId)) {
                    collectionId = ConfigurationManager.AppSettings["collection"];
                }

                return collectionId;
            }
        }

        //Use the ReadOrCreateDatabase function to get a reference to the database.
        private Database database;
        private Database Database {
            get {
                if (database == null) {
                    database = ReadOrCreateDatabase();
                }

                return database;
            }
        }

        //Use the ReadOrCreateCollection function to get a reference to the collection.
        private DocumentCollection collection;
        private DocumentCollection Collection {
            get {
                if (collection == null) {
                    collection = ReadOrCreateCollection(Database.SelfLink);
                }

                return collection;
            }
        }

        //This property establishes a new connection to DocumentDB the first time it is used, 
        //and then reuses this instance for the duration of the application avoiding the
        //overhead of instantiating a new instance of DocumentClient with each request
        private DocumentClient client;
        private DocumentClient Client {
            get {
                if (client == null) {
                    string endpoint = ConfigurationManager.AppSettings["endpoint"];
                    string authKey = ConfigurationManager.AppSettings["authKey"];
                    Uri endpointUri = new Uri(endpoint);
                    client = new DocumentClient(endpointUri, authKey);
                }

                return client;
            }
        }

        public IEnumerable<T> GetItems(Expression<Func<T, bool>> predicate) {
            return Client.CreateDocumentQuery<T>(Collection.DocumentsLink)
                .Where(predicate)
                .AsEnumerable();
        }

        public IEnumerable<T> GetItems(Expression<Func<T, bool>> predicate, 
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null) {

            IQueryable<T> query = Client.CreateDocumentQuery<T>(Collection.DocumentsLink);

            if(predicate != null) {
                query = query.Where(predicate);
            }

            if(orderBy != null) {
                return orderBy(query).AsEnumerable();
            } else {
                return query.AsEnumerable();
            }
        }

        public async Task<Document> CreateItemAsync(T item) {
            return await Client.CreateDocumentAsync(Collection.SelfLink, item);
        }

        public T GetItem(Expression<Func<T, bool>> predicate) {
            return Client.CreateDocumentQuery<T>(Collection.DocumentsLink)
                        .Where(predicate)
                        .AsEnumerable()
                        .FirstOrDefault();
        }

        public async Task<Document> UpdateItemAsync(string id, T item) {
            Document doc = GetDocument(id);
            return await Client.ReplaceDocumentAsync(doc.SelfLink, item);
        }

        public Document GetDocument(string id) {
            return Client.CreateDocumentQuery(Collection.DocumentsLink)
                .Where(d => d.Id == id)
                .AsEnumerable()
                .FirstOrDefault();
        }
    }
}
