using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class PageBar
    {
        /// <summary>
        /// 获取当页的开始编号和结束编号，从而从数据库中查询数据
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageCount"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public static void GetStartandEndIndex(int pageIndex, int pageSize, out int startIndex, out int endIndex)
        {
            startIndex = (pageIndex - 1) * pageSize + 1;
            endIndex = pageIndex * pageSize;
        }

        public static string GetPageBarByType(int pageIndex, int pageCount, string type)
        {
            #region 页码条算法
            if (pageCount == 1)
            {
                return string.Empty;//只有一页不出现页面条
            }
            int start = pageIndex - 5;  //起始 要求页码条长度为10
            if (start < 1) start = 1;
            int end = start + 9; //终止位置

            if (end > pageCount) //这种情况要重新计算起始，保证页码条的长度为10
            {
                end = pageCount;
                start = end - 9 > 0 ? end - 9 : 1;
            }
            #endregion

            #region 组合字符串
            StringBuilder sb = new StringBuilder();
            sb.Append("<a href=\"javascript:" + type + "(1);\" class='myPageBar'>首页</a> ");
            //根据当前页码判断是否加此标签
            if (pageIndex > 1) sb.AppendFormat("<a href=\"javascript:" + type + "({0});\" class='myPageBar'>上一页</a>", pageIndex - 1);
            for (int i = start; i <= end; i++)
            {
                //当前页的页码不设置链接跳转
                if (i == pageIndex)
                {
                    sb.Append(i);
                }
                else
                {
                    sb.AppendFormat("<a href=\"javascript:" + type + "({0});\" class='myPageBar'>{0}</a>", i);
                }
                sb.Append(" ");
            }
            //根据当前页码判断是否加此标签
            if (pageIndex < pageCount) sb.AppendFormat("<a href=\"javascript:" + type + "({0});\" class='myPageBar'>下一页</a>", pageIndex + 1);
            sb.AppendFormat("<a href=\"javascript:" + type + "({0});\" class='myPageBar'>尾页</a>", pageCount);
            #endregion
            return sb.ToString();
        }

        /// <summary>
        /// 获取数字页码条(最大长度为10)
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageCount"></param>
        /// <returns></returns>
        public static string GetPageBar(int pageIndex, int pageCount)
        {
            #region 页码条算法
            if (pageCount <= 1)
            {
                return string.Empty;//只有一页不出现页面条
            }
            int start = pageIndex - 5;  //起始 要求页码条长度为10
            if (start < 1) start = 1;
            int end = start + 9; //终止位置

            if (end > pageCount) //这种情况要重新计算起始，保证页码条的长度为10
            {
                end = pageCount;
                start = end - 9 > 0 ? end - 9 : 1;
            }
            #endregion

            #region 组合字符串
            StringBuilder sb = new StringBuilder();
            sb.Append("<a href=\"?pageIndex=1\" class='myPageBar'>首页</a> ");
            //根据当前页码判断是否加此标签
            if (pageIndex > 1) sb.AppendFormat("<a href=\"?pageIndex={0}\" class='myPageBar'>上一页</a>", pageIndex - 1);
            for (int i = start; i <= end; i++)
            {
                //当前页的页码不设置链接跳转
                if (i == pageIndex)
                {
                    sb.Append(i);
                }
                else
                {
                    sb.AppendFormat("<a href=\"?pageIndex={0}\" class='myPageBar'>{0}</a>", i);
                }
                sb.Append(" ");
            }
            //根据当前页码判断是否加此标签
            if (pageIndex < pageCount) sb.AppendFormat("<a href=\"?pageIndex={0}\" class='myPageBar'>下一页</a>", pageIndex + 1);
            sb.AppendFormat("<a href=\"?pageIndex={0}\" class='myPageBar'>尾页</a>", pageCount);
            #endregion
            return sb.ToString();
        }
    }
}
