using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Deepleo.Web.Services
{
    public static class StrHelper
    {
        public static string HandlePic(string s)
        {
            char[] ch = s.ToCharArray();
            int length = s.Length;
            int i;
            int picStart = s.IndexOf("![");
            if (picStart < 0) return s;
            //获取 图片名 图片url
            string picName = "";
            string url = "";
            for (i = picStart + 2; i < length; i++)
            {
                if (ch[i] == ']') break;
                picName += ch[i];
            }
            for (i = i + 2; i < length; i++)
            {
                if (ch[i] == ')') break;
                url += ch[i];
            }
            return s.Substring(0, picStart) + "!/" + picName + "/" + "<" + url + ">" + s.Substring(i + 1, length);
        }

        public static string HandleLink(string s)
        {
            char[] ch = s.ToCharArray();
            int length = s.Length;
            int i;
            //获取 链接名 链接
            int linkStart = s.IndexOf('[');
            if (linkStart < 0) return s;
            string linkName = "";
            string link = "";
            for (i = linkStart + 1; i < length; i++)
            {
                if (ch[i] == ']') break;
                linkName += ch[i];
            }
            for (i = i + 2; i < length; i++)
            {
                if (ch[i] == ')') break;
                link += ch[i];
            }
            return s.Substring(0, linkStart) + "/" + linkName + "/" + "<" + link + ">" + s.Substring(i + 1, length);
        }
    }
}