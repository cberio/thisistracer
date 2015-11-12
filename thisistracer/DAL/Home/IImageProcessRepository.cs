using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using thisistracer.Models;

namespace thisistracer.DAL.Home {
    public interface IImageProcessRepository {

        BlobMetadata GenerateMetadataFromImg(MemoryStream img);

        float? GetLatitude(Image targetImg);
        float? GetLongitude(Image targetImg);

        double GetLatitudeD(Image targetImg);
        double GetLongitudeD(Image targetImg);

        float ExifGpsToFloat(PropertyItem propItemRef, PropertyItem propItem);
        double ExifGpsToFloatD(PropertyItem propItemRef, PropertyItem propItem);

        byte[] GetPropertyItemValue(Image img, int propId);

        Task<Image> LoadPicture(Uri url);
        Image RotateImage(Image img);

        MemoryStream ProcessB4Upload(System.Web.HttpPostedFileBase fs);
        
    }
}
