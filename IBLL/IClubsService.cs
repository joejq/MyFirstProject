using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBLL
{
    public interface IClubsService : IBaseService<Clubs>
    {
        //添加特有方法
        Clubs GetClubById(int id);
        int GetIdByClubnameIgnoreDelflag(string clubname);
    }
}
