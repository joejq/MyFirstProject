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
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace ClubWebApp.Controllers
{
    public class ClubsController : ApiController
    {
        private CBEntities db = new CBEntities();

        // GET: api/Clubs
        public IHttpActionResult GetClubs()
        {
            var allClubs = db.Clubs.Where(e => e.Delflag == 0).ToList();
            if (allClubs.Count <= 0) return Ok("");
            List<BotClub> jClubs = new List<BotClub>();
            foreach (var i in allClubs)
            {
                jClubs.Add(new BotClub()
                {
                    Cid = i.ID,
                    Cname = i.Cname
                });
            }
            return Json(jClubs, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
        }

        // GET: api/Clubs/5
        [ResponseType(typeof(Clubs))]
        public IHttpActionResult GetClubs(int id)
        {
            Clubs clubs = null;
            try
            {
                clubs = db.Clubs.Where(e => e.Delflag == 0 && e.ID == id).First();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            
            if (clubs == null)
            {
                return Json("{}");
            }
            return Json(clubs, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
        }

        public IHttpActionResult GetClub(string clubname)
        {
            int id = -1;
            try
            {
                var club = db.Clubs.Where(e => e.Delflag == 0 && e.Cname == clubname).First();
                id = club.ID;
            }
            catch (Exception ex){ }
            //if (clubs == null)
            //{
            //    return Json("{}");
            //}
            return Ok(id);
        }       
    }

    public class BotClub
    {
        public int Cid { get; set; }
        public string Cname { get; set; }
    }
}