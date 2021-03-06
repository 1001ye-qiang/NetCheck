﻿using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetStock
{
    class Program
    {
        public string Path = "./Stock/";
        public string TableName = "list";


        static string curTime;
        static string beginTime;
        static List<string> lstCode;


        static void Init()
        {
            // TIME
            CTime time = new CTime();
            curTime = time.end;
            beginTime = time.begin;

            // LST CODE
            CAllStockCode asc = new CAllStockCode();
            lstCode = asc.lstCode;
        }

        static CLineContents GetRealtimeData(string code)
        {
            DateTime dt = new DateTime(DateTime.Now.Ticks);
            // dapan is out data after 17,maybe, but gegu delay a day;
            if (dt.DayOfWeek != DayOfWeek.Sunday && dt.DayOfWeek != DayOfWeek.Saturday && dt.Hour > 9 && dt.Hour < 15)
            {
                string nowData = GetHttpData.GetUrltoHtml(string.Format(GetHttpData.nowFormat, code));

                int start = nowData.LastIndexOf('{');
                nowData = nowData.Substring(start, nowData.IndexOf('}') - start + 1);
                JsonData tmpData = JsonMapper.ToObject(nowData);

                CLineContents lc2 = new CLineContents();
                try
                {
                    lc2.time = (string)tmpData["time"];
                    lc2.code = (string)tmpData["code"];
                    lc2.name = (string)tmpData["name"];
                    lc2.last = (float)((double)tmpData["price"]);
                    lc2.high = (float)(double)tmpData["high"];
                    lc2.low = (float)(double)tmpData["low"];
                    lc2.start = (float)(double)tmpData["open"];
                    lc2.yes_last = (float)((double)tmpData["yestclose"]);
                    lc2.price = (float)(double)tmpData["updown"];
                    lc2.rate = (float)((double)tmpData["percent"] * 100);
                    lc2.chg_rate = 0;
                    lc2.chg_num = (int)tmpData["volume"];
                    //lc2.chg_value = (float)(double)tmpData["turnover"];
                    lc2.total = 0;
                    lc2.total_on = 0;
                }
                catch (Exception e) { Console.WriteLine(e.ToString() + "\n" + nowData); }

                if (lc2.start >= float.Epsilon)
                {
                    return lc2;
                }
            }
            return null;
        }

        static string GetCodeData(string code, string beginTime, string curTime)
        {
            try
            {
                string url = string.Format(GetHttpData.urlFormat, "", code, beginTime, curTime);
                string contents = GetHttpData.GetUrltoHtml(url, "GB18030");
                string[] lines = contents.Split('\n');

                // 记录数过滤。。。
                if (lines.Length <= 3) return null;

                CStockContents sc = new CStockContents();

                for (int j = lines.Length - 1; j > 0; --j)
                {
                    // 过滤停。。。
                    if (lines[j].Contains("None,") || lines[j] == "")
                        continue;

                    CLineContents lc = new CLineContents(lines[j]);
                    sc.Push(lc);

                    if (j == 1)
                    {
                        CLineContents lcTmp = GetRealtimeData(code);
                        if(lcTmp != null)
                            sc.Push(lcTmp);
                    }
                }
                string res = sc.GetResult();
                if (res != null || res != "")
                    return (res);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString() + "\n" + code);
            }
            return null;
        }





        static void Main(string[] args)
        {
            Init();

            // LOG FILE 
            string logName = "Log_" + curTime + ".txt";
            int index = 0;
            while (File.Exists(logName))
            {
                logName = "Log_" + curTime + "_" + ++index + ".txt";
            }
            FileStream aFile = new FileStream(logName, FileMode.OpenOrCreate);
            StreamWriter sw = new StreamWriter(aFile);

            // RESULT
            List<string> result = new List<string>();

            for (int i = 0; i < lstCode.Count; ++i)
            {
                string code = lstCode[i];
                string res = GetCodeData(code, beginTime, curTime);

                if (res == null) continue;
                result.Add(res);
            }

            for (int i = 0; i < result.Count; ++i)
            {
                sw.WriteLine(result);
            }
            sw.Close();
        }

        static float GetNumAverage(CLineContents lc)
        {
            return (float)lc.chg_value / lc.chg_num;
        }

        static float GetInflow(CLineContents lc)
        {
            return (float)lc.chg_value - lc.yes_last * lc.chg_num;
        }
    }
}
