using MyTest1.Worker;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using MyTest1.Models;

namespace MyTest1.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View(); //
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        /*
         * Get list of urls from sitemap
         */
        [HttpPost]
        public ActionResult JsonPrs(string id)
        {
            SiteMapParser siteMapParser = new SiteMapParser();
            return Json(siteMapParser.Test(id));
        }

        /*
         * Get timeots for each url
         */
        [HttpPost]
        public ActionResult JsonPrsTimeOuts(string url)
        {
            SiteMapParser siteMapParser = new SiteMapParser();
            return Json(siteMapParser.steedTest(url));
        }
        /*
        * Sort data and add to DB
        */
        [HttpPost]
        public JsonResult FinalSort(List<CheckUrl> SortList)
        {

            SiteMapParser siteMapParser = new SiteMapParser();
            return Json(siteMapParser.SortList(SortList));
        }
        /*
        * Gett hosts from DB
        */
        public ActionResult History()
        {
            List<string> AllDomens = new List<string>();
            AllDomens.Add("All");
            TestUrlModel Db = new TestUrlModel();
            var Base = Db.CheckUrls;
            foreach (var item in Base)
            {
                if (!AllDomens.Contains(item.Host))
                {
                    AllDomens.Add(item.Host);
                }
            }
            ViewBag.AllDomens = AllDomens;
            return View();
        }
        /*
         * History responce from DB
         */
        [HttpPost]
        public ActionResult JsonGetSortedHistory(string id)
        {
           
            TestUrlModel Db = new TestUrlModel();
            var Base = Db.CheckUrls;
            if (!string.IsNullOrEmpty(id) && id != "All")
            {
                var SortedBase = Base.Where(e => e.Host == id);
                return Json(SortedBase);
            }
            return Json(Base);
        }


    }
}