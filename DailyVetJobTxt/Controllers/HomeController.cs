using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DailyVetJobTxt.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Welcome to Veteran Job Texting Service!";

            Models.UserPrefsModel model = new Models.UserPrefsModel();

            if (Request.IsAuthenticated)
            {
                string keywords = "";
                int zipCode = 0;

                LData.BizPhoneUser.GetUserPrefs(User.Identity.Name, out zipCode, out keywords);
                model.KeyWords = keywords;
                model.ZipCode = zipCode;
            }

            return View(model);
        }

        public ActionResult About()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SaveUserPrefs(Models.UserPrefsModel model)
        {
            LData.BizPhoneUser.SaveUserPrefs(User.Identity.Name, model.ZipCode, model.KeyWords);

            return PartialView("_PrefSaveSuccess");
        }
    }
}
