using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace thisistracer.Models
{
    public class PhotoMapModel
    {
        [Key] [DisplayName("순서")]
        public int idx { get; set; }
        [DisplayName("파일명")]
        public string F_Name { get; set; }
        [Bindable(false)]
        public string F_OrgName { get; set; }
        [DisplayName("파일크기")]
        public long F_Size { get; set; }
        [DisplayName("날짜")]
        public DateTime PicDate { get; set; }
        public string ContentType { get; set; }
        [DisplayName("URL")]
        public Uri F_Url { get; set; }
        [DisplayName("Lat")]
        public float? F_Latitude { get; set; }
        [DisplayName("Lng")]
        public float? F_Longitude { get; set; }
    }
}
