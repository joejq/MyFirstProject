using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBLL
{
    public interface INoticeService : IBaseService<Notice>
    {
        Notice GetLatestNotice(int CID);
        List<Notice> GetRecentNotice(int CID);
    }
}
