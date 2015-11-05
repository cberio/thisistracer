using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using thisistracer.Models;
using System.Drawing;

namespace thisistracer.DAL.Home {
    public class BlobStorage : IBlobStorage {

        private CloudStorageAccount storageAccount;
        private CloudBlobClient blobClient;
        private CloudBlobContainer container;
        private CloudBlobDirectory directory;
        private CloudBlockBlob blob;

        public BlobStorage() {
            
            try {
                storageAccount = CloudStorageAccount.Parse(Util.Utils.GetAppConfigure("AzureStroageConnection"));
                blobClient = storageAccount.CreateCloudBlobClient();
                container = blobClient.GetContainerReference("thisistracer");

                if (container.CreateIfNotExists()) {
                    container.SetPermissions(
                        new BlobContainerPermissions {
                            PublicAccess = BlobContainerPublicAccessType.Blob
                        }
                    );
                }
            } catch (Exception ex) {
                throw new Exception("Azure Connection Problem :" + ex.InnerException);
            }
        }

        public virtual void UploadBlob(MemoryStream ms, string blobName) {
            if (ms == null)
                throw new ArgumentNullException("MemoryStream is null");

            blob = container.GetBlockBlobReference(blobName);

            ms.Position = 0;
            ms.Seek(0, SeekOrigin.Begin);

            //blob.UploadFromStreamAsync(ms);
            blob.UploadFromStream(ms);

            if (ms != null) {
                ms.Close();
                ms.Dispose();
            }
        }

        public void DeleteBlob(string blobName) {
            if (string.IsNullOrEmpty(blobName))
                throw new ArgumentException("blobNmae is empty");

            blob = container.GetBlockBlobReference(blobName);
            blob.DeleteIfExistsAsync();
        }

        public IEnumerable<IListBlobItem> GetBlobs(string userId = "sample") {
            if (string.IsNullOrEmpty(userId))
                userId = "sample";

            directory = container.GetDirectoryReference(userId);

            return directory.ListBlobs(false, BlobListingDetails.Metadata);
        }

        public IEnumerable<PhotoMapModel> IBlobToModel(IEnumerable<IListBlobItem> IListBlob) {
            List<PhotoMapModel> photoMapList = new List<PhotoMapModel>();
            int idx = 0;
            DateTime dt = DateTime.Now;
            float lat = 37.5651f;
            float lng = 126.98955f;

            if (IListBlob != null) {                

                foreach (IListBlobItem item in IListBlob) {
                    if (item.GetType() == typeof(CloudBlockBlob)) {
                        blob = (CloudBlockBlob)item;
                        PhotoMapModel photoMap = new PhotoMapModel() {
                            idx = idx,
                            ContentType = blob.Properties.ContentType,
                            F_Size = blob.Properties.Length,
                            F_Name = blob.Name,
                            F_OrgName = GetBlobMetadata("orgName")?.ToString(),
                            F_Url = blob.Uri,
                            PicDate = DateTime.TryParse(GetBlobMetadata("Date")?.ToString(), out dt) ? DateTime.Parse(GetBlobMetadata("Date")?.ToString()) : dt,
                            F_Latitude = float.TryParse(GetBlobMetadata("lat")?.ToString(), out lat) ? float.Parse(GetBlobMetadata("lat").ToString()) : lat,
                            F_Longitude = float.TryParse(GetBlobMetadata("lng")?.ToString(), out lng) ? float.Parse(GetBlobMetadata("lng").ToString()) : lng
                        };

                        idx++;
                        photoMapList.Add(photoMap);
                    }
                }
            } else {
                PhotoMapModel photoMap = new PhotoMapModel() {
                    idx = idx,
                    ContentType = "image/jpg",
                    F_Size = 10,
                    F_Name = "test",
                    F_OrgName = "test",
                    F_Url = new Uri("https://cberio.blob.core.windows.net/thisistracer/sample/image_20151012061100110.jpg"),
                    PicDate = dt,
                    F_Latitude = lat,
                    F_Longitude = lng
                };

                photoMapList.Add(photoMap);
            }

            return photoMapList;
        }

        public void SetBlobMetadata(Dictionary<string, string> metadatas) {
            if (metadatas == null)
                return;

            foreach(var item in metadatas) {
                blob.Metadata[item.Key] = item.Value;
            }

            blob.SetMetadata();
            metadatas.Clear();
        }

        public void SetBlobProperty(string contentType) {
            blob.Properties.ContentType = contentType ?? "image/jpeg";
            blob.Properties.CacheControl= "max-age=3600, must-revalidate";
            blob.SetProperties();
        }

        public object GetBlobMetadata(string medatadaKey) {
            if (blob.Metadata.ContainsKey(medatadaKey)) {
                return blob.Metadata[medatadaKey];
            } else {
                return null;
            }
        }

        public void ProcessAfterUpload(MemoryStream ms) {
            
        }
    }
}
