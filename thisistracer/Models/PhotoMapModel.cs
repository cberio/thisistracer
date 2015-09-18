using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace thisistracer.Models
{
    public class PhotoMapModel
    {
        [Key]
        public int idx { get; set; }
        public string F_Name { get; set; }
        public string F_OrgName { get; set; }
        public long F_Size { get; set; }
        public DateTime PicDate { get; set; }
        public string ContentType { get; set; }
        public Uri F_Url { get; set; }
        public float? F_Latitude { get; set; }
        public float? F_Longitude { get; set; }
    }
}
