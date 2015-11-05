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
        [Key]
        public int idx { get; set; }
        public string UserId { get; set; }
        public string AlbumName { get; set; }
        public DateTime AlbumCreateDate { get; set; }

        [NotMapped]
        public virtual ICollection<PhotoMapModel> Photos { get; set; }
    }
}
