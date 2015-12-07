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
    public class TableStorage<T> : ITableStorage<T> where T: ITableEntity, new() {

        private CloudStorageAccount storageAccount;
        private CloudTableClient tableClient;
        private CloudTable table;
        private TableBatchOperation batchOperation;
        private TableOperation tableOperation;
        private TableResult tableResult;

        public TableStorage() {
            try {
                storageAccount = CloudStorageAccount.Parse(Util.Utils.GetAppConfigure("AzureStroageConnection"));
                tableClient = storageAccount.CreateCloudTableClient();
                table = tableClient.GetTableReference("thisistracer");

                table.CreateIfNotExists();

            } catch (Exception ex) {storageAccount = CloudStorageAccount.Parse(Util.Utils.GetAppConfigure("AzureStroageConnection"));
                throw new Exception("Azure Table Storage Connection Problem :" + ex.InnerException);
            }
        }

        public void Insert(T item) {
            tableOperation = TableOperation.Insert(item);
            table.Execute(tableOperation);
        }

        public void Insert(IEnumerable<T> items) {
            batchOperation = new TableBatchOperation();

            if(items.Count() > 100) {
                throw new Exception("Choose under 100 items");
            }

            foreach(var item in items)
                batchOperation.Insert(item);

            table.ExecuteBatch(batchOperation);
        }

        public IEnumerable<TElement> GetItems<TElement>(Expression<Func<TElement, bool>> predicate) where TElement : ITableEntity, new(){
            var query = table.CreateQuery<TElement>().Where(predicate).AsEnumerable();

            return query;
        }

        public T GetItem(Expression<Func<T, bool>> predicate) {

            throw new NotImplementedException("");
        }

        public T GetItem(string partitionKey, string rowKey) {
            tableOperation = TableOperation.Retrieve<T>(partitionKey, rowKey);
            tableResult = table.Execute(tableOperation);

            if (tableResult.Result != null)
                return (T)tableResult.Result;
            else
                throw new NullReferenceException("null");
        }        
    }
}
