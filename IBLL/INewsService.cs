using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBLL
{
    public interface INewsService : IBaseService<News>
    {
        News GetLatestNews(int CID);
        News GetNewsById(int nid);
        List<News> GetRecentNews(int CID);
    }
}
