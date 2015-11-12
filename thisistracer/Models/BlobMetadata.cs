using Microsoft.Azure.Documents.Spatial;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thisistracer.Models {
    public class BlobMetadata {
        public double latitude { get; set; } = 33.3823521d;
        public double longitude { get; set; } = 126.54295d;

        public Point location { get; set; } 
        [DataType(DataType.Date)]
        public DateTime picDate { get; set; } = DateTime.Now;
    }
}
