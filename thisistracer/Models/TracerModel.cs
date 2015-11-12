using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using thisistracer.Models;

namespace thisistracer.Models {
    public class TracerModel {
        public string userId { get; set; }
        public Uri uri { get; set; }
        public BlobMetadata metadata {get; set;}
    }
}
