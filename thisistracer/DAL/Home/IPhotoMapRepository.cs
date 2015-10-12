using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using thisistracer.Models;

namespace thisistracer.DAL.Home
{
    public interface IPhotoMapRepository : IDisposable
    {
        IEnumerable<PhotoMapModel> GetMapInfoList(System.Security.Principal.IPrincipal User);
        object GetBlobMetadata(CloudBlockBlob blob, string metadataKey);

        float? GetLatitude(Image targetImg);
        float? GetLongitude(Image targetImg);

        float ExifGpsToFloat(PropertyItem propItemRef, PropertyItem propItem);
        byte[] GetPropertyItemValue(Image img, int propId);

        Task<Image> LoadPicture(Uri url);
        
        CloudBlobContainer GetContainer(string containerName);
        CloudBlobContainer GetContainer();

        void UploadToBlobStorage(System.Web.HttpPostedFileBase fs, System.Security.Principal.IPrincipal User);

        Image RotateImage(Image img);
    }
}
