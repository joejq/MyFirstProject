using IBLL;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class ClubLeaderService : BaseService<ClubLeader>, IClubLeaderService
    {
        public override void SetCurrentDal()
        {
            this.CurrentDal = new DAL.ClubLeaderDal();
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

        public bool LogicDeleteEntity(ClubLeader entity)
        {
            entity.Delflag = 1;
            return CurrentDal.EditEntity(entity);
        }

        public ClubLeader GetLeaderByCid(int cid)
        {
            var temp = CurrentDal.LoadEntities(e => e.CID == cid && e.Delflag == 0).ToList();
            if(temp.Count <= 0) return null;
            else return temp[0];
        }
    }
}
