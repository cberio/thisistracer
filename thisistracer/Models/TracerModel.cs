using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using thisistracer.Models;

namespace thisistracer.Models {
    public class TracerModel : TableEntity {
        public TracerModel(string partitionKey, string rowKey) {
            this.PartitionKey = partitionKey;
            this.RowKey = rowKey;
        }
        public TracerModel() { }

        public string userId { get; set; }
        public string uri { get; set; }

        public double latitude { get; set; } = 33.3823521d;
        public double longitude { get; set; } = 126.54295d;
        [DataType(DataType.Date)]
        public DateTime picDate { get; set; } = DateTime.Now;
    }
}
