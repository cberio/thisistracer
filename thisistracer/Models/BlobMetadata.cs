using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thisistracer.Models {
    public class BlobMetadata {
        public float latitude { get; set; } = 37.5651f;
        public float longitude { get; set; } = 126.98955f;
        [DataType(DataType.Date)]
        public DateTime picDate { get; set; } = DateTime.Now;
    }
}
