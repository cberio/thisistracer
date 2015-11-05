using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using thisistracer.DAL.Home;
using thisistracer.Models;
using Microsoft.AspNet.Identity;
using Microsoft.WindowsAzure.Storage.Blob;
using thisistracer.Util;
using System.Drawing;

namespace thisistracer.Controllers {
    public class HomeController : Controller {
        //IBlobStorageRepository iPhotoMap;

        IImageProcessRepository _iImageMap;
        IBlobStorage _iBlobMap;

        //public HomeController(IBlobStorageRepository repo) {
        //    iPhotoMap = repo;
        //}

        public HomeController(IImageProcessRepository imgRepo, IBlobStorage iBlobStorage) {
            _iImageMap = imgRepo;
            _iBlobMap = iBlobStorage;
        }

        // GET: Home
        public ActionResult Index() {
            //return View(iPhotoMap.GetMapInfoList(User));
            IEnumerable<IListBlobItem> blobItem = _iBlobMap.GetBlobs(User.Identity.GetUserId());
            IEnumerable<PhotoMapModel> photoMapList = _iBlobMap.IBlobToModel(blobItem);

            return View(photoMapList);
        }

        [HttpPost, Authorize]
        public ActionResult Upload(List<HttpPostedFileBase> fileUpload) {
            System.IO.MemoryStream ms;
            Dictionary<string, string> metadata;

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
                    _iBlobMap.UploadBlob(ms, uniqueName);

                    // Set Metadata And Property
                    _iBlobMap.SetBlobMetadata(metadata);
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
            IEnumerable<IListBlobItem> blobItem = _iBlobMap.GetBlobs(User.Identity.GetUserId());
            IEnumerable<PhotoMapModel> photoMapList = _iBlobMap.IBlobToModel(blobItem);

            return View(photoMapList);
        }

        [HttpGet, Authorize]
        public JsonResult GetJsonList() {
            IEnumerable<IListBlobItem> blobItem = _iBlobMap.GetBlobs(User.Identity.GetUserId());
            IEnumerable<PhotoMapModel> photoMapList = _iBlobMap.IBlobToModel(blobItem);

            return Json(photoMapList, JsonRequestBehavior.AllowGet);
        }
    }
}