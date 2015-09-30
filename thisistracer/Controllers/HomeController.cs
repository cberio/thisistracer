using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using thisistracer.DAL.Home;
using thisistracer.Models;


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
            return View(iPhotoMap.GetMapInfoList());
        }

        [HttpPost]
        public string Upload(List<HttpPostedFileBase> fileUpload)
        {
            //foreach (var item in fileUpload)
            //{
            //    if (item != null)
            //        iPhotoMap.UploadToBlobStorage(item);
            //}
            
            return "success";
        }

        [HttpGet, Authorize] 
        public ActionResult Upload()
        {
            return View();
        }
    }
}