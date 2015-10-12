using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using thisistracer.DAL.Home;
using thisistracer.Models;
using Microsoft.AspNet.Identity;

namespace thisistracer.Controllers
{
    public class HomeController : Controller
    {
        IPhotoMapRepository iPhotoMap;

        public HomeController(IPhotoMapRepository repo)
        {
            iPhotoMap = repo;
        }

        // GET: Home
        public ActionResult Index()
        {
            //if(User.Identity.GetUserId() != null)
                return View(iPhotoMap.GetMapInfoList(User));
            //else 
            //    return View(iPhotoMap.GetMapInfoList());
        }

        [HttpPost]
        public ActionResult Upload(List<HttpPostedFileBase> fileUpload)
        {

            foreach (var item in fileUpload) {
                if (item != null)
                    iPhotoMap.UploadToBlobStorage(item, User);
            }

            return View("Upload");
        }

        [HttpGet, Authorize] 
        public ActionResult Upload()
        {
            return View();
        }
    }
}