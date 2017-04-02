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
    public class NoticeController : Controller
    {
        static int Cid;
        static int pageSize = 10;

        IBLL.INoticeService noticeService = new BLL.NoticeService();
        // GET: Admin/Notice
        public ActionResult Index(int id)
        {
            Cid = id;
            int pageIndex;
            if (!int.TryParse(Request["pageIndex"], out pageIndex))
            {
                pageIndex = 1;
            }
            IBLL.INoticeService noticeService = new BLL.NoticeService();
            int total;
            var noticeList = noticeService.LoadPageEntities(pageIndex, pageSize, out total, e => e.CID == Cid && e.Delflag == 0, e => e.Subtime, false).ToList();
            int pageCount = (int)Math.Ceiling((double)total / 10);
            pageIndex = pageIndex < 1 ? 1 : pageIndex;
            pageIndex = pageIndex > pageCount ? pageCount : pageIndex;
            string pageBarString = PageBar.GetPageBarByType(pageIndex, pageCount, "loadNotice");
            var jNotice = JsonConvert.SerializeObject(new { notice = noticeList, pageBar = pageBarString }, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            return Json(jNotice, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddNotice()
        {
            Model.Notice notice = new Model.Notice()
            {
                Ncontent = Request["content"],
                CID = Cid
            };
            #region update club subtime
            IBLL.IClubsService cs = new BLL.ClubsService();
            Model.Clubs c = cs.LoadEntities(e => e.ID == Cid).First();
            c.Subtime = DateTime.Now;
            cs.EditEntity(c);
            #endregion
            if (noticeService.AddEntity(notice))
            {
                return Content("Ok");
            }
            else
            {
                return Content("Fail");
            }
        }
        public ActionResult EditNotice()
        {
            int nid;
            if (int.TryParse(Request["id"], out nid))
            {
                if (nid <= 0) return Content("Error");
                Model.Notice notice = noticeService.LoadEntities(e => e.ID == nid).First();
                notice.Ncontent = Request["content"];
                if (noticeService.EditEntity(notice))
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
                return Content("Error");
            }
        }

        public ActionResult DeleteNotice()
        {
            int nid;
            if (int.TryParse(Request["id"], out nid))
            {
                if (nid <= 0) return Content("Error");
                if (noticeService.LogicDeleteEntity(nid)) return Content("Ok");
                else return Content("Fail");
            }
            else return Content("Error");
        }
    }
}