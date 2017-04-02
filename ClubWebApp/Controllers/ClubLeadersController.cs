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
    public class ClubLeadersController : ApiController
    {
        private CBEntities db = new CBEntities();
       
        // GET: api/ClubLeaders/5
        [ResponseType(typeof(ClubLeader))]
        public IHttpActionResult GetClubLeader(int id)
        {
            try
            {
                var leader = db.ClubLeader.Where(e => e.Delflag == 0 && e.CID == id).First();
                BotLeader jLeader = new BotLeader()
                {
                    Lid = leader.ID,
                    Lname = leader.Lname,
                    Lmajor = leader.Lmajor,
                    Lcontact = leader.Lcontact,
                    Lregiste = (DateTime)leader.Lregiste,
                    Lsex = (short)leader.Lsex,
                    Limage = leader.Limage
                };
                return Json(jLeader, new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });
            } catch(Exception ex) { }
            return Ok("");
        }

    }

    public class BotLeader
    {
        public int Lid { get; set; }
        public string Lname { get; set; }
        public string Lmajor { get; set; }
        public string Lcontact { get; set; }
        public DateTime Lregiste { get; set; }
        public short Lsex { get; set; }
        public string Limage { get; set; }
    }
}