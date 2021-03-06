﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;
using thisistracer.Models;
using Microsoft.WindowsAzure.Storage;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using thisistracer.Util;
using Microsoft.AspNet.Identity;

namespace thisistracer.DAL.Home
{
    public class BlobStorageRepository : IBlobStorageRepository, IDisposable
    {
        private CloudStorageAccount storageAccount;
        private CloudBlobClient blobClient;
        private CloudBlobContainer container;
        private CloudBlobDirectory directory;

        public BlobStorageRepository()
        {
            try
            {
                storageAccount = CloudStorageAccount.Parse(Util.Utils.GetAppConfigure("AzureStroageConnection"));
                blobClient = storageAccount.CreateCloudBlobClient();
                container = blobClient.GetContainerReference("thisistracer");

                if (container.CreateIfNotExists())
                {
                    container.SetPermissions(
                        new BlobContainerPermissions
                        {
                            PublicAccess = BlobContainerPublicAccessType.Blob
                        }
                    );
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Azure Connection Problem :" + ex.InnerException);
            }
        }
        
        public IEnumerable<PhotoMapModel> GetMapInfoList(System.Security.Principal.IPrincipal User) {
            List<PhotoMapModel> L_bStroage = new List<PhotoMapModel>();
            int idx = 0;
            DateTime dt = DateTime.Now;
            float lat = 37.5651f;
            float lng = 126.98955f;

            if(User.Identity.GetUserId() != null) {
                directory = container.GetDirectoryReference(User.Identity.GetUserId());
            } else {
                directory = container.GetDirectoryReference("sample");
            }

            foreach (IListBlobItem item in directory.ListBlobs(false, BlobListingDetails.Metadata)) {
                
                if (item.GetType() == typeof(CloudBlockBlob)) {
                    CloudBlockBlob blob = (CloudBlockBlob)item;
                    PhotoMapModel bStorage = new PhotoMapModel() {
                        idx = idx,
                        ContentType = blob.Properties.ContentType,
                        F_Size = blob.Properties.Length,
                        F_Name = blob.Name,
                        F_OrgName = GetBlobMetadata(blob, "orgName")?.ToString(), //blob.Metadata["OrgName"] ?? "";
                        F_Url = blob.Uri,
                        PicDate = DateTime.TryParse(GetBlobMetadata(blob, "Date")?.ToString(), out dt) ? DateTime.Parse(blob.Metadata["Date"]) : dt,
                        F_Latitude = float.TryParse(GetBlobMetadata(blob, "lat")?.ToString(), out lat) ? (float?)float.Parse(blob.Metadata["lat"]) : lat,
                        F_Longitude = float.TryParse(GetBlobMetadata(blob, "lng")?.ToString(), out lng) ? (float?)float.Parse(blob.Metadata["lng"]) : lng
                    };
                    idx++;                    
                    L_bStroage.Add(bStorage);
                }
            }

            return L_bStroage;
        }

        public object GetBlobMetadata(CloudBlockBlob blob, string metadataKey) {
            if(blob.Metadata.ContainsKey(metadataKey)) {
                return blob.Metadata[metadataKey];
            } else {
                return null;
            }
        }

        public float? GetLatitude(Image targetImg)
        {
            try
            {
                //Property Item 0x0001 - PropertyTagGpsLatitudeRef
                PropertyItem propItemRef = targetImg.GetPropertyItem(1);
                //Property Item 0x0002 - PropertyTagGpsLatitude
                PropertyItem propItemLat = targetImg.GetPropertyItem(2);

                //https://cberio.blob.core.windows.net/photo/collection01/20150723_101520.jpg
                //https://cberio.blob.core.windows.net/photo/collection01/20150722_234712.jpg

                //System.Diagnostics.Debug.WriteLine("==== Orient = " + targetImg.GetPropertyItem(274).Value[0]);
                return ExifGpsToFloat(propItemRef, propItemLat);
            }
            catch (ArgumentException)
            {
                return null;
            }
            finally
            {
                //if (targetImg != null)
                //    targetImg.Dispose();
            }
        }
        public float? GetLongitude(Image targetImg)
        {
            try
            {
                ///Property Item 0x0003 - PropertyTagGpsLongitudeRef
                PropertyItem propItemRef = targetImg.GetPropertyItem(0x0003);
                //Property Item 0x0004 - PropertyTagGpsLongitude
                PropertyItem propItemLong = targetImg.GetPropertyItem(0x0004);
                return ExifGpsToFloat(propItemRef, propItemLong);
            }
            catch (ArgumentException)
            {
                return null;
            }
            finally
            {
                //if (targetImg != null)
                //    targetImg.Dispose();
            }
        }

        public float ExifGpsToFloat(PropertyItem propItemRef, PropertyItem propItem)
        {
            uint degreesNumerator = BitConverter.ToUInt32(propItem.Value, 0);
            uint degreesDenominator = BitConverter.ToUInt32(propItem.Value, 4);
            float degrees = degreesNumerator / (float)degreesDenominator;

            uint minutesNumerator = BitConverter.ToUInt32(propItem.Value, 8);
            uint minutesDenominator = BitConverter.ToUInt32(propItem.Value, 12);
            float minutes = minutesNumerator / (float)minutesDenominator;

            uint secondsNumerator = BitConverter.ToUInt32(propItem.Value, 16);
            uint secondsDenominator = BitConverter.ToUInt32(propItem.Value, 20);
            float seconds = secondsNumerator / (float)secondsDenominator;

            float coorditate = degrees + (minutes / 60f) + (seconds / 3600f);
            string gpsRef = System.Text.Encoding.ASCII.GetString(new byte[1] { propItemRef.Value[0] }); //N, S, E, or W
            if (gpsRef == "S" || gpsRef == "W")
                coorditate = 0 - coorditate;
            return coorditate;
        }
        public byte[] GetPropertyItemValue(Image img, int propId)
        {
            byte[] propVal = null;

            try
            {
                PropertyItem propItem = img.GetPropertyItem(propId);
                propVal = propItem.Value;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException(ex.InnerException.ToString());
            }

            return propVal;
        }
        /// <summary>
        /// Loads an image from a URL into a Bitmap object.
        /// Currently as written if there is an error during downloading of the image, no exception is thrown.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<Image> LoadPicture(Uri url)
        {
            HttpWebRequest wreq;
            HttpWebResponse wresp;
            Stream mystream;
            Bitmap bmp;

            bmp = null;
            mystream = null;
            wresp = null;
            try
            {
                wreq = (HttpWebRequest)WebRequest.Create(url);
                wreq.AllowWriteStreamBuffering = true;
                wresp = (HttpWebResponse)await Task.Factory
                    .FromAsync<WebResponse>(wreq.BeginGetResponse,
                                            wreq.EndGetResponse,
                                            null);

                if ((mystream = wresp.GetResponseStream()) != null)
                    bmp = new Bitmap(mystream);

                //wreq = (HttpWebRequest)HttpWebRequest.Create(url);
                //wreq.AllowWriteStreamBuffering = true;

                //wresp = (HttpWebResponse)wreq.GetResponse();

                //if ((mystream = wresp.GetResponseStream()) != null)
                //    bmp = new Bitmap(mystream);
            }
            catch
            {
                // Do nothing... 
            }
            finally
            {
                if (mystream != null)
                    mystream.Close();

                if (wresp != null)
                    wresp.Close();
            }

            return (Image)bmp;
        }

        public CloudBlobContainer GetContainer(string containerName)
        {
            container = blobClient.GetContainerReference(containerName);

            if (container.CreateIfNotExists())
            {
                // configure container for public access
                var permissions = container.GetPermissions();
                permissions.PublicAccess = BlobContainerPublicAccessType.Container;
                container.SetPermissions(permissions);
            }

            return container;
        }
        public CloudBlobContainer GetContainer()
        {
            return container;
        }

        public void UploadToBlobStorage(System.Web.HttpPostedFileBase fs, System.Security.Principal.IPrincipal User)
        {
            if (fs == null || fs.ContentLength == 0)
                return;

            if (fs.ContentType != "image/jpeg")
                return;

            string uniqueName = string.Format(User.Identity.GetUserId()+ "/image_{0}{1}",
                DateTime.Now.ToString("yyyyMMddhhmmssms"), Path.GetExtension(fs.FileName));


            Image bmp = Image.FromStream(fs.InputStream);
            RotateImage(bmp);

            CloudBlockBlob blob = container.GetBlockBlobReference(uniqueName);
            blob.Properties.ContentType = fs.ContentType;            

            var lat = GetLatitude(bmp);
            var lng = GetLongitude(bmp);
            var date = GetPropertyItemValue(bmp, 36867) ?? null; // pic date

            blob.Metadata["OrgName"] = fs.FileName;
            if (lat != null && lng != null)
            {
                blob.Metadata["lat"] = lat.ToString();
                blob.Metadata["lng"] = lng.ToString();
            }
            if (date != null)
            {
                Regex r = new Regex(":");
                blob.Metadata["Date"] = r.Replace(Encoding.UTF8.GetString(date).Replace("\0", ""), "-", 2);
            }

            MemoryStream ms = new MemoryStream();
            ms.Position = 0;

            bmp.Save(ms, ImageFormat.Jpeg);

            ms.Seek(0, SeekOrigin.Begin);

            blob.UploadFromStream(ms);
            blob.Properties.CacheControl = "max-age=3600, must-revalidate";
            blob.SetProperties();
            blob.SetMetadata();

            if (ms != null)
            {
                ms.Close();
                ms.Dispose();
            }

            if (bmp != null)
                bmp.Dispose();

        }

        public Image RotateImage(Image bmp)
        {
            if (Array.IndexOf(bmp.PropertyIdList, 274) > -1)
            {
                var orientation = (int)bmp.GetPropertyItem(274).Value[0];
                switch (orientation)
                {
                    case 1:
                        // No rotation required.
                        break;
                    case 2:
                        bmp.RotateFlip(RotateFlipType.RotateNoneFlipX);
                        break;
                    case 3:
                        bmp.RotateFlip(RotateFlipType.Rotate180FlipNone);
                        break;
                    case 4:
                        bmp.RotateFlip(RotateFlipType.Rotate180FlipX);
                        break;
                    case 5:
                        bmp.RotateFlip(RotateFlipType.Rotate90FlipX);
                        break;
                    case 6:
                        bmp.RotateFlip(RotateFlipType.Rotate90FlipNone);
                        break;
                    case 7:
                        bmp.RotateFlip(RotateFlipType.Rotate270FlipX);
                        break;
                    case 8:
                        bmp.RotateFlip(RotateFlipType.Rotate270FlipNone);
                        break;
                }
                // This EXIF data is now invalid and should be removed.
                bmp.RemovePropertyItem(274);
            }

            return bmp;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~PhotoMapRepository() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
