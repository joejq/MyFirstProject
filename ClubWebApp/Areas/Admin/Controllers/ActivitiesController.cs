using Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClubWebApp.Areas.Admin.Controllers
{
    [LoginFilterAttribute]
    public class ActivitiesController : Controller
    {
        static int Cid;
        static int pageSize = 15;
        const string actiDefaultUrl = "/Content/images/default/150_150.png";

        IBLL.IActivitiesSerivice actiService = new BLL.ActivitiesService();
        // GET: Admin/Activities
        public ActionResult Index(int id)
        {
            Cid = id;
            int pageIndex;
            if (!int.TryParse(Request["pageIndex"], out pageIndex))
            {
                pageIndex = 1;
            }
            IBLL.IActivitiesSerivice actiService = new BLL.ActivitiesService();
            int total;
            var actiList = actiService.LoadPageEntities(pageIndex, pageSize, out total, e => e.CID == Cid && e.Delflag == 0, e => e.Atime, false).ToList();
            int pageCount = (int)Math.Ceiling((double)total / pageSize);
            pageIndex = pageIndex < 1 ? 1 : pageIndex;
            pageIndex = pageIndex > pageCount ? pageCount : pageIndex;
            string pageBarString = PageBar.GetPageBarByType(pageIndex, pageCount, "loadActivities");
            var jActi = JsonConvert.SerializeObject(new { activities = actiList, pageBar = pageBarString }, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            return Json(jActi, JsonRequestBehavior.AllowGet);
            //return View();
        }
        public ActionResult AddActivities()
        {
            return View();
        }
        [ValidateInput(false)]
        public ActionResult AddActivitiesSubmit()
        {
            DateTime date;
            try
            {
                date = Convert.ToDateTime(Request["date"]);
            }
            catch (Exception ex)
            {
                date = DateTime.Now;
            }
            Model.Activities acti = new Model.Activities()
            {
                Atitle = Request["title"],
                Acontent = Request["content"],
                Aimage = string.IsNullOrEmpty(Request["url"]) ? actiDefaultUrl : Request["url"],
                Atime = date,
                CID = Cid
            };
            #region update club subtime
            IBLL.IClubsService cs = new BLL.ClubsService();
            Model.Clubs c = cs.LoadEntities(e => e.ID == Cid).First();
            c.Subtime = DateTime.Now;
            cs.EditEntity(c);
            #endregion
            if (actiService.AddEntity(acti))
            {
                return Content("Ok");
            }
            else
            {
                return Content("Fail");
            }
        }
        
        public ActionResult EditActivities()
        {
            int aid;
            if (int.TryParse(Request["id"], out aid))
            {
                if (aid <= 0) return Content("Error");
                Model.Activities acti = actiService.GetActiById(aid);
                ViewBag.Aid = acti.ID;
                ViewData["Atitle"] = acti.Atitle;
                ViewData["Aimg"] = acti.Aimage;
                ViewData["Adate"] = acti.Atime;
                ViewData["Acontent"] = acti.Acontent;
                return View();
            }
            else
            {
                return Content("Error");
            }
        }
        [ValidateInput(false)]
        public ActionResult EditActivitiesSubmit()
        {
            int aid;
            DateTime date;
            try
            {
                date = Convert.ToDateTime(Request["date"]);
            }
            catch (Exception ex)
            {
                date = DateTime.Now;
            }
            if (int.TryParse(Request["id"], out aid))
            {
                if (aid <= 0) return Content("Error");
                Model.Activities acti = new Model.Activities()
                {
                    ID = aid,
                    Atitle = Request["title"],
                    Acontent = Request["content"],
                    Aimage = string.IsNullOrEmpty(Request["url"]) ? actiDefaultUrl : Request["url"],
                    Atime = date,
                    CID = Cid
                };
                if (actiService.EditEntity(acti))
                {
                    return Content("Ok");
                }
                else
                {
                    return Content("Fail");
                }
            }
            else
            {
                return Content("Error!");
            }
        }
        public ActionResult DeleteActivities()
        {
            int aid;
            if (int.TryParse(Request["id"], out aid))
            {
                if (aid <= 0) return Content("Error");
                if (actiService.LogicDeleteEntity(aid)) return Content("Ok");
                else return Content("Fail");
            }
            else return Content("Error");
        }
    }
}