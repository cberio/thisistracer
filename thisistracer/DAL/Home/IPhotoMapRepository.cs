﻿using Microsoft.WindowsAzure.Storage.Blob;
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
    interface IPhotoMapRepository : IDisposable
    {
        IEnumerable<PhotoMapModel> GetMapInfoList();

        float? GetLatitude(Image targetImg);
        float? GetLongitude(Image targetImg);
        float? GetLatitude(Uri uri);
        float? GetLongitude(Uri uri);

        float ExifGpsToFloat(PropertyItem propItemRef, PropertyItem propItem);
        byte[] GetPropertyItemValue(Image img, int propId);

        Task<Image> LoadPicture(Uri url);

        CloudBlobContainer GetContainer(string containerName);
        CloudBlobContainer GetContainer();

        void UploadToBlobStorage(System.Web.HttpPostedFileBase fs);

        Image RotateImage(Image img);
    }
}