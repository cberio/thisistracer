using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using thisistracer.Models;

namespace thisistracer.DAL.Home {
    public interface IBlobStorage{
        IEnumerable<IListBlobItem> GetBlobs(string userId = "sample");
        Uri UploadBlob(System.IO.MemoryStream ms, string blobName);
        void DeleteBlob(string blobName);

        //IEnumerable<PhotoMapModel> IBlobToModel(IEnumerable<IListBlobItem> IListBlob);
        void SetBlobMetadata(Dictionary<string, string> metadatas);
        void SetBlobProperty(string contentType);
        object GetBlobMetadata(string medatadaKey);

        void ProcessAfterUpload(MemoryStream ms);
    }
}
