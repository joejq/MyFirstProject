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
    public class ActivitiesController : ApiController
    {
        private CBEntities db = new CBEntities();

        // GET: api/Activities/5
        [ResponseType(typeof(Activities))]
        public IHttpActionResult GetActivities(int id)
        {
            var temp1 = db.Activities.Where(e => e.Delflag == 0 && e.CID == id);
            int count = temp1.Count() > 3 ? 3 : temp1.Count();
            if (count <= 0) return Ok("");
            var temp2 = temp1.OrderByDescending(e => e.Atime).Skip(0).Take(count);
            var activities = temp2.ToList();
            //var activities = db.Activities.Where(e => e.Delflag == 0 && e.CID == id).ToList();
            //if (activities.Count <= 0) return Ok("");
            List<BotActivities> jActi = new List<BotActivities>();
            foreach(var i in activities)
            {
                jActi.Add(new BotActivities()
                {
                    Aid = i.ID,
                    Atitle = i.Atitle,
                    Aimage = i.Aimage,
                    Atime = (DateTime)i.Atime
                });
            }
            return Json(jActi, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
        }
        
    }
    public class BotActivities
    {
        public int Aid { get; set; }
        public string Atitle { get; set; }
        public string Aimage { get; set; }
        public DateTime Atime { get; set; }
    }
}