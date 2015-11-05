using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;

namespace thisistracer.DAL.Home {
    public interface IImageProcessRepository {

        Dictionary<string, string> GenerateMetadataFromImg(MemoryStream img);

        float? GetLatitude(Image targetImg);
        float? GetLongitude(Image targetImg);

        float ExifGpsToFloat(PropertyItem propItemRef, PropertyItem propItem);
        byte[] GetPropertyItemValue(Image img, int propId);

        Task<Image> LoadPicture(Uri url);
        Image RotateImage(Image img);

        MemoryStream ProcessB4Upload(System.Web.HttpPostedFileBase fs);
        
    }
}
