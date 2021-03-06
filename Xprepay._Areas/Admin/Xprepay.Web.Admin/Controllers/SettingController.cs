﻿using System.Web.Mvc;
using Xprepay.Services;
using Xprepay.Web.Admin.ViewModels;

namespace Xprepay.Web.Admin.Controllers
{
    public class SettingController : AdminControllerBase
    {
        public ActionResult Index()
        {
            var model = new SettingIndexViewModel().BuildSettings();
            return AreaView("setting/index.cshtml", model);
        }

        [HttpPost]
        public StandardJsonResult Update(string key, string value)
        {
            var result = new StandardJsonResult();
            result.Try(() =>
            {
                var service = Ioc.Get<ISettingService>();
                service.Update(key, value);
                SettingContext.Instance.Refresh();
            });
            return result;
        }

        public ActionResult Refresh()
        {
            SettingContext.Instance.Refresh();
            return RedirectToAction("index");
        }
    }
}
