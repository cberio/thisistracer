using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using thisistracer.DAL.Home;
using thisistracer.DAL;
using thisistracer.Models;
using Microsoft.AspNet.Identity;
using Microsoft.WindowsAzure.Storage.Blob;
using thisistracer.Util;

namespace thisistracer.Controllers {
    public class HomeController : Controller {
        
        IImageProcessRepository _iImageMap;
        IBlobStorage _iBlobMap;
        IDocumentDBRepository<TracerModel> _doc;
        ITableStorage<TracerModel> _tab;

        public HomeController(IImageProcessRepository imgRepo, IBlobStorage iBlobStorage, IDocumentDBRepository<TracerModel> docDB, ITableStorage<TracerModel> tab) {
            _iImageMap = imgRepo;
            _iBlobMap = iBlobStorage;
            _doc = docDB;
            _tab = tab;
        }

        // GET: Home
        public ActionResult Index() {
            //IEnumerable<IListBlobItem> blobItem = _iBlobMap.GetBlobs(User.Identity.GetUserId());
            //IEnumerable<TracerModel> photoMapList = _iBlobMap.IBlobToModel(blobItem);
            var userId = User.Identity.GetUserId() ?? "sample";
            IEnumerable<TracerModel> photoMapList = _tab.GetItems<TracerModel>(d => d.PartitionKey == userId);

            return View(photoMapList);

            /* documentDB */
            //var userId = User.Identity.GetUserId() ?? "sample";
            //var map = _doc.GetItems(d => d.userId == userId);

            //return View(map);
        }

        public ActionResult Test() {
            var map = _doc.GetItems(d => d.userId == User.Identity.GetUserId());

            return View(map);
        }

        [HttpPost, Authorize]
        public ActionResult Upload(List<HttpPostedFileBase> fileUpload) {
            System.IO.MemoryStream ms;
            //Dictionary<string, string> metadata;
            BlobMetadata metadata;
            Uri blobUri;

            foreach (var item in fileUpload) {
                //if (item != null)
                //iPhotoMap.UploadToBlobStorage(item, User);

                if (item != null && item?.ContentLength > 0 && item?.ContentType == "image/jpeg") {
                    ms = new System.IO.MemoryStream();

                    string uniqueName = Utils.GetUniqueFileName(User, item.FileName);

                    // RotateImage
                    ms = _iImageMap.ProcessB4Upload(item);

                    // Get Metadata
                    metadata = _iImageMap.GenerateMetadataFromImg(ms);

                    // Upload to blobStorage
                    blobUri = _iBlobMap.UploadBlob(ms, uniqueName);
                    //metadata

                    // Set Metadata And Property
                    //_iBlobMap.SetBlobMetadata(metadata);
                    var tracer = new TracerModel(User.Identity.GetUserId(), Guid.NewGuid().ToString()) {
                        userId = User.Identity.GetUserId(),
                        uri = blobUri.ToString(),
                        latitude = metadata.latitude,
                        longitude = metadata.longitude,
                        picDate = metadata.picDate
                    };

                    //_doc.CreateItemAsync(tracer);
                    _tab.Insert(tracer);
                    _iBlobMap.SetBlobProperty(item.ContentType);
                }
            }

            return View("Upload");
        }

        [HttpGet, Authorize]
        public ActionResult Upload() {
            return View();
        }

        [HttpGet]
        public ActionResult List() {
            //var userId = User.Identity.GetUserId() ?? "sample";
            //var map = _doc.GetItems(d => d.userId == userId);

            //return View(map);
            var userId = User.Identity.GetUserId() ?? "sample";
            IEnumerable<TracerModel> photoMapList = _tab.GetItems<TracerModel>(d => d.PartitionKey == userId);

            return View(photoMapList);
        }
    }
}