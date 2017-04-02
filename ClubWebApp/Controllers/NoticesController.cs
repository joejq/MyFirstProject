using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Model;
using Newtonsoft.Json;

namespace ClubWebApp.Controllers
{
    public class NoticesController : ApiController
    {
        private CBEntities db = new CBEntities();
      
        // GET: api/Notices/5
        [ResponseType(typeof(Notice))]
        public IHttpActionResult GetNotice(int id)
        {
            var temp1 = db.Notice.Where(e => e.Delflag == 0 && e.CID == id);
            int count = temp1.Count() > 3 ? 3 : temp1.Count();
            if (count <= 0) return Ok("");
            var temp2 = temp1.OrderByDescending(e => e.Subtime).Skip(0).Take(count);
            var notice = temp2.ToList();
            
            List<BotNotice> jNotice = new List<BotNotice>();
            foreach(var i in notice)
            {
                jNotice.Add(new BotNotice()
                {
                    Nid = i.ID,
                    Ncontent = i.Ncontent,
                    Subtime = (DateTime)i.Subtime
                });
            }
            return Json(jNotice, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
        }      
    }

    public class BotNotice
    {
        public int Nid { get; set; }
        public string Ncontent { get; set; }
        public DateTime Subtime { get; set; }
    }
}