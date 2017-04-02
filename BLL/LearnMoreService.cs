using IBLL;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class LearnMoreService : BaseService<LearnMore>, ILearnMoreService
    {
        public bool DeleteEntity(int ID)
        {
            var temp = CurrentDal.LoadEntities(e => e.ID == ID).ToList();
            var entity = temp[0];
            return CurrentDal.DeleteEntity(entity);
        }

        public bool LogicDeleteEntity(LearnMore entity)
        {
            return true;
        }

        public bool LogicDeleteEntity(int ID)
        {
            return true;
            //var temp = CurrentDal.LoadEntities(e => e.ID == ID).ToList();
            //var entity = temp[0];
            //entity.Delflag = 1;
            //return CurrentDal.EditEntity(entity);
        }

        public override void SetCurrentDal()
        {
            this.CurrentDal = new DAL.LearnMoreDal();
        }
    }
}
