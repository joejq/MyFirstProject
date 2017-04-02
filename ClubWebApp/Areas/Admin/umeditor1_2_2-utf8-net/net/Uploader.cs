using System;
using System.Collections.Generic;
using System.Web;
using System.IO;
using System.Collections;


/// <summary>
/// UEditor缂栬緫鍣ㄩ€氱敤涓婁紶绫?
/// </summary>
public class Uploader
{
    string state = "SUCCESS";

    string URL = null;
    string currentType = null;
    string uploadpath = null;
    string filename = null;
    string originalName = null;
    HttpPostedFile uploadFile = null;

    /**
  * 涓婁紶鏂囦欢鐨勪富澶勭悊鏂规硶
  * @param HttpContext
  * @param string
  * @param  string[]
  *@param int
  * @return Hashtable
  */
    public Hashtable upFile(HttpContext cxt, string pathbase, string[] filetype, int size)
    {
        pathbase = pathbase + DateTime.Now.ToString("yyyy-MM-dd") + "/";
        uploadpath = cxt.Server.MapPath(pathbase);//鑾峰彇鏂囦欢涓婁紶璺緞

        try
        {
            uploadFile = cxt.Request.Files[0];
            originalName = uploadFile.FileName;

            //鐩綍鍒涘缓
            createFolder();

            //鏍煎紡楠岃瘉
            if (checkType(filetype))
            {
                state = "涓嶅厑璁哥殑鏂囦欢绫诲瀷";
            }
            //澶у皬楠岃瘉
            if (checkSize(size))
            {
                state = "鏂囦欢澶у皬瓒呭嚭缃戠珯闄愬埗";
            }
            //淇濆瓨鍥剧墖
            if (state == "SUCCESS")
            {
                filename = reName();
                uploadFile.SaveAs(uploadpath + filename);
                URL = pathbase + filename;
            }
        }
        catch (Exception e)
        {
            state = "鏈煡閿欒";
            URL = "";
        }
        return getUploadInfo();
    }

    /**
 * 涓婁紶娑傞甫鐨勪富澶勭悊鏂规硶
  * @param HttpContext
  * @param string
  * @param  string[]
  *@param string
  * @return Hashtable
 */
    public Hashtable upScrawl(HttpContext cxt, string pathbase, string tmppath, string base64Data)
    {
        pathbase = pathbase + DateTime.Now.ToString("yyyy-MM-dd") + "/";
        uploadpath = cxt.Server.MapPath(pathbase);//鑾峰彇鏂囦欢涓婁紶璺緞
        FileStream fs = null;
        try
        {
            //鍒涘缓鐩綍
            createFolder();
            //鐢熸垚鍥剧墖
            filename = System.Guid.NewGuid() + ".png";
            fs = File.Create(uploadpath + filename);
            byte[] bytes = Convert.FromBase64String(base64Data);
            fs.Write(bytes, 0, bytes.Length);

            URL = pathbase + filename;
        }
        catch (Exception e)
        {
            state = "鏈煡閿欒";
            URL = "";
        }
        finally
        {
            fs.Close();
            deleteFolder(cxt.Server.MapPath(tmppath));
        }
        return getUploadInfo();
    }

    /**
* 鑾峰彇鏂囦欢淇℃伅
* @param context
* @param string
* @return string
*/
    public string getOtherInfo(HttpContext cxt, string field)
    {
        string info = null;
        if (cxt.Request.Form[field] != null && !String.IsNullOrEmpty(cxt.Request.Form[field]))
        {
            info = field == "fileName" ? cxt.Request.Form[field].Split(',')[1] : cxt.Request.Form[field];
        }
        return info;
    }

    /**
     * 鑾峰彇涓婁紶淇℃伅
     * @return Hashtable
     */
    private Hashtable getUploadInfo()
    {
        Hashtable infoList = new Hashtable();

        infoList.Add("state", state);
        infoList.Add("url", URL);
        infoList.Add("originalName", originalName);
        infoList.Add("name", Path.GetFileName(URL));
        infoList.Add("size", uploadFile.ContentLength);
        infoList.Add("type", Path.GetExtension(originalName));

        return infoList;
    }

    /**
     * 閲嶅懡鍚嶆枃浠?
     * @return string
     */
    private string reName()
    {
        return System.Guid.NewGuid() + getFileExt();
    }

    /**
     * 鏂囦欢绫诲瀷妫€娴?
     * @return bool
     */
    private bool checkType(string[] filetype)
    {
        currentType = getFileExt();
        return Array.IndexOf(filetype, currentType) == -1;
    }

    /**
     * 鏂囦欢澶у皬妫€娴?
     * @param int
     * @return bool
     */
    private bool checkSize(int size)
    {
        return uploadFile.ContentLength >= (size * 1024 * 1024);
    }

    /**
     * 鑾峰彇鏂囦欢鎵╁睍鍚?
     * @return string
     */
    private string getFileExt()
    {
        string[] temp = uploadFile.FileName.Split('.');
        return "." + temp[temp.Length - 1].ToLower();
    }

    /**
     * 鎸夌収鏃ユ湡鑷姩鍒涘缓瀛樺偍鏂囦欢澶?
     */
    private void createFolder()
    {
        if (!Directory.Exists(uploadpath))
        {
            Directory.CreateDirectory(uploadpath);
        }
    }

    /**
     * 鍒犻櫎瀛樺偍鏂囦欢澶?
     * @param string
     */
    public void deleteFolder(string path)
    {
        //if (Directory.Exists(path))
        //{
        //    Directory.Delete(path, true);
        //}
    }
}