using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBLL
{
    public interface IActivitiesSerivice : IBaseService<Activities>
    {
        Activities GetLatestActivity(int CID);
        Activities GetActiById(int aid);
        List<Activities> GetRecentActivities(int CID);
    }
}
