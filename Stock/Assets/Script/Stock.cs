using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System;


public class MainProcess
{
    public void Excute()
    {
        AllExistStockID aId = new AllExistStockID();
        aId.LoadByLocal();
    }
}

public class ConstData
{
    public static string IDLstPath = "./stock/ExitStockID_sh.txt";
    public static string IDLstPath2 = "./stock/ExitStockID_sz.txt";
    public static string IDLstPath3 = "./stock/ExitStockID_zxb.txt";
    public static string IDLstPath4 = "./stock/ExitStockID_cyb.txt";
    public static string UrlBaseSina = "http://hq.sinajs.cn/list=";
    public static string sh = "sh";
    public static string sz = "sz";
}

public class FileOpt
{
    public static void WriteFile(string path, byte[] bytes)
    {
        path.Replace("\\", "/");
        string dir = path.Substring(0, path.LastIndexOf("/"));
        CheckPath(dir);

        FileStream fs = new FileStream(path, FileMode.Create);
        fs.Write(bytes, 0, bytes.Length);
        fs.Flush();
        fs.Close();
        fs.Dispose();
    }

    public static byte[] ReadFile(string path)
    {
        FileStream fs = new FileStream(path, FileMode.OpenOrCreate);

        if (File.Exists(path))
        {
            byte[] data = new byte[fs.Length];
            fs.Read(data, 0, (int)fs.Length);
            return data;
        }
        return null;
    }

    public static string ReadFileTxt(string path)
    {
        FileStream fs = new FileStream(path, FileMode.OpenOrCreate);

        if (File.Exists(path))
        {
            string data = File.ReadAllText(path);
            return data;
        }
        return null;
    }

    static void CheckPath(string path)
    {
        if (!Directory.Exists(path))
        {
            string parent = path.Substring(0, path.LastIndexOf("/"));
            CheckPath(parent);
            Directory.CreateDirectory(path);
        }
    }
}

public class GetHttpData
{
    public static string GetUrltoHtml(string Url, string type = "UTF-8")
    {
        try
        {
            System.Net.WebRequest wReq = System.Net.WebRequest.Create(Url);
            // Get the response instance.
            System.Net.WebResponse wResp = wReq.GetResponse();
            System.IO.Stream respStream = wResp.GetResponseStream();
            // Dim reader As StreamReader = New StreamReader(respStream)
            using (System.IO.StreamReader reader = new System.IO.StreamReader(respStream, Encoding.GetEncoding(type)))
            {
                return reader.ReadToEnd();
            }
        }
        catch (System.Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        return "";
    }
}

public class AllExistStockID
{
    public List<string> lstCode = new List<string>();
    public List<string> lstCode2 = new List<string>();
    public List<string> lstCode3 = new List<string>();
    public List<string> lstCode4 = new List<string>();

    public void LoadByNet()
    {
        lstCode.Clear();
        lstCode2.Clear();
        lstCode3.Clear();
        lstCode4.Clear();

        // 1
        string content = "";
        for(int i = 600000; i < 606999; ++i)
        {
            string code = string.Format("{0:000000}", i);
            string result = GetHttpData.GetUrltoHtml(ConstData.UrlBaseSina + ConstData.sh + code);
            if (result != null && result.Length > 100)
            {
                lstCode.Add(code);
            }
        }

        for (int i = 0; i < lstCode.Count; ++i)
        {
            content += lstCode[i] + "\t";
        }
        FileOpt.WriteFile(ConstData.IDLstPath, UTF8Encoding.UTF8.GetBytes(content));

        // 2
        content = "";
        for (int i = 000001; i < 001999; ++i)
        {
            string code = string.Format("{0:000000}", i);
            string result = GetHttpData.GetUrltoHtml(ConstData.UrlBaseSina + ConstData.sz + code);
            if (result != null && result.Length > 100)
            {
                lstCode2.Add(code);
            }
        }

        for (int i = 0; i < lstCode2.Count; ++i)
        {
            content += lstCode2[i] + "\t";
        }
        FileOpt.WriteFile(ConstData.IDLstPath2, UTF8Encoding.UTF8.GetBytes(content));

        // 3
        content = "";
        for (int i = 002000; i < 002999; ++i)
        {
            string code = string.Format("{0:000000}", i);
            string result = GetHttpData.GetUrltoHtml(ConstData.UrlBaseSina + ConstData.sz + code);
            if (result != null && result.Length > 100)
            {
                lstCode3.Add(code);
            }
        }
        for (int i = 0; i < lstCode3.Count; ++i)
        {
            content += lstCode3[i] + "\t";
        }
        FileOpt.WriteFile(ConstData.IDLstPath3, UTF8Encoding.UTF8.GetBytes(content));

        // 4
        content = "";
        for (int i = 300001; i < 301999; ++i)
        {
            string code = string.Format("{0:000000}", i);
            string result = GetHttpData.GetUrltoHtml(ConstData.UrlBaseSina + ConstData.sz + code);
            if (result != null && result.Length > 100)
            {
                lstCode4.Add(code);
            }
        }
        for(int i = 0; i < lstCode4.Count; ++i)
        {
            content += lstCode4[i] + "\t";
        }
        FileOpt.WriteFile(ConstData.IDLstPath4, UTF8Encoding.UTF8.GetBytes(content));
    }

    public void LoadByLocal()
    {
        if (!File.Exists(ConstData.IDLstPath)
            || !File.Exists(ConstData.IDLstPath2)
            || !File.Exists(ConstData.IDLstPath3)
            || !File.Exists(ConstData.IDLstPath4))
        {
            LoadByNet();
        }
        else
        {
            lstCode.Clear();
            lstCode2.Clear();
            lstCode3.Clear();
            lstCode4.Clear();

            string contents = FileOpt.ReadFileTxt(ConstData.IDLstPath);
            string[] array = contents.Split('\t');
            for (int i = 0; i < array.Length; ++i)
            {
                lstCode.Add(array[i]);
            }


            contents = FileOpt.ReadFileTxt(ConstData.IDLstPath2);
            array = contents.Split('\t');
            for (int i = 0; i < array.Length; ++i)
            {
                lstCode2.Add(array[i]);
            }

            contents = FileOpt.ReadFileTxt(ConstData.IDLstPath3);
            array = contents.Split('\t');
            for (int i = 0; i < array.Length; ++i)
            {
                lstCode3.Add(array[i]);
            }

            contents = FileOpt.ReadFileTxt(ConstData.IDLstPath4);
            array = contents.Split('\t');
            for (int i = 0; i < array.Length; ++i)
            {
                lstCode4.Add(array[i]);
            }
        }
    }
}

public class StockItem
{
    List<DataItem> lstDi = new List<DataItem>();
}

public class DataItem
{
    public string date;
    public string code;

    public float price;
    public float yestclose;

    public float percent;
    public float updown;

    public float high;
    public float low;

    public float volume;
    public float turnover;

    public float chgrate; // 换手率
}
public class StoreItemStock
{
    public const string model = "http://quotes.money.163.com/service/chddata.html?code={0}{1}&start={2}&end={3}&fields=TCLOSE;HIGH;LOW;TOPEN;LCLOSE;CHG;PCHG;VOTURNOVER;VATURNOVER";
    //"http://quotes.money.163.com/service/chddata.html?code=1000001&start=19910403&end=20151211";
    // http://api.money.126.net/data/feed/1000001
    public const string urlBase = "http://hq.sinajs.cn/list=";
    public const string pathBase = "./stockItem/";

    public static void Excute(string id, string type)
    {
        if (!File.Exists(pathBase + id))
        {
        }
        long time = GetTime();

        string now = "";
        string url = string.Format(model, type, id, time, now);

        string data = GetHttpData.GetUrltoHtml(url);

        CheckData();

        StoreFile();
    }

    static long GetTime()
    {
        return 0;
    }

    static void CheckData()
    {
    }

    static void StoreFile()
    { }
}

public class AlanesizeItemStock
{
}

public class HistoryResult
{
}
