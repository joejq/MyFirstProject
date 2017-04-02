using IBLL;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class ClubsService : BaseService<Clubs>, IClubsService
    {
        public override void SetCurrentDal()
        {
            this.CurrentDal = new DAL.ClubsDal();
        }

        public bool DeleteEntity(int ID)
        {
            var temp = CurrentDal.LoadEntities(e => e.ID == ID).ToList();
            var entity = temp[0];
            return CurrentDal.DeleteEntity(entity);
        }

        public bool LogicDeleteEntity(int ID)
        {
            var temp = CurrentDal.LoadEntities(e => e.ID == ID).ToList();
            var entity = temp[0];
            entity.Delflag = 1;
            return CurrentDal.EditEntity(entity);
        }

        public bool LogicDeleteEntity(Clubs entity)
        {
            entity.Delflag = 1;
            return CurrentDal.EditEntity(entity);
        }

        public Clubs GetClubById(int id)
        {
            var temp = CurrentDal.LoadEntities(e => e.Delflag == 0 && e.ID == id).ToList();
            return temp[0];
        }

        public int GetIdByClubnameIgnoreDelflag(string clubname)
        {
            return CurrentDal.LoadEntities(e => e.Cname == clubname).First().ID;
        }
    }
}
