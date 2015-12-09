using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using thisistracer.Models;

namespace thisistracer.Models {
    public class Album {
        public Guid AlbumId { get; set; } = Guid.NewGuid();
        public string UserId { get; set; }
        public string AlbumName { get; set; }
        public DateTime AlbumCreateDate { get; set; }

        [NotMapped]
        public virtual ICollection<TracerModel> Photos { get; set; }
    }
}
