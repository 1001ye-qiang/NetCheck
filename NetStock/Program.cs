using LitJson;
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



        static void Main(string[] args)
        {
            //string content = GetUrltoHtml(StoreItemStock.model, "UTF-8");
            //string name = "./1000001.txt";


            // TODO 预处理
            // 存
            // 更新预处理
            // 存
            // 计算结果
            // 存
            // 更新计算结果
            // 存

            // TIME
            DateTime dt = new DateTime(DateTime.Now.Ticks);
            string curTime = dt.ToString("yyyyMMdd");

            DateTime dt2 = dt.AddMonths(-12);
            //dt2 = dt2.AddDays(-20);
            string beginTime = dt2.ToLocalTime().ToString("yyyyMMdd");

            // LST CODE
            List<string> lstCode = new List<string>();

            string sdata = GetHttpData.GetUrltoHtml(GetHttpData.libCode);

            JsonData jdata = JsonMapper.ToObject(sdata);

            JsonData jdata2 = jdata["list"];

            for (int i = 0; i < jdata2.Count; ++i)
            {
                lstCode.Add((string)jdata2[i]["CODE"]);
            }

            FileStream aFile = new FileStream("Log_" + curTime + ".txt", FileMode.OpenOrCreate);
            StreamWriter sw = new StreamWriter(aFile);
            SortedList<string, float> results = new SortedList<string, float>();
            for (int i = 0; i < lstCode.Count; ++i)
            {
                try
                {
                    //string url = "http://quotes.money.163.com/service/chddata.html?code=0000001&start=20100101&end=20160121";
                    string url = string.Format(GetHttpData.urlFormat, "", lstCode[i], beginTime, curTime);
                    string contents = GetHttpData.GetUrltoHtml(url, "GB18030");
                    string[] lines = contents.Split('\n');

                    // 记录数过滤。。。
                    if (lines.Length <= 3) continue;

                    // TEST+++++++++++++++++++
                    CStockContents sc = new CStockContents();
                    // TEST+++++++++++++++++++++++


                    for (int j = lines.Length - 1; j > 0; --j)
                    {
                        if (lines[j].Contains("None") || lines[j] == "") continue;

                        CLineContents lc = new CLineContents(lines[j]);
                        sc.Push(lc);

                        if (j == 1)
                        {
                            if (dt.Hour > 9 && dt.Hour < 24)
                            {
                                string nowData = GetHttpData.GetUrltoHtml(string.Format(GetHttpData.nowFormat, lstCode[i]));

                                int start = nowData.LastIndexOf('{');
                                nowData = nowData.Substring(start, nowData.IndexOf('}') - start + 1);
                                JsonData tmpData = JsonMapper.ToObject(nowData);

                                CLineContents lc2 = new CLineContents();
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
                                lc2.chg_value = (float)(double)tmpData["turnover"];
                                lc2.total = 0;
                                lc2.total_on = 0;
                                if (lc2.start >= float.Epsilon)
                                    sc.Push(lc2);
                            }
                        }
                    }

                    //sc.WriteFile();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString() + "\n" + lstCode[i]);
                }
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

        public class CLineContents
        {
            public CLineContents(string line)
            {
                try {
                string[] rows = line.Split(',');
                time = rows[0];
                code = rows[1];
                name = rows[2];
                last = Convert.ToSingle(rows[3]);
                high = Convert.ToSingle(rows[4]);
                low = Convert.ToSingle(rows[5]);
                start = Convert.ToSingle(rows[6]);
                yes_last = Convert.ToSingle(rows[7]);
                price = Convert.ToSingle(rows[8]);
                rate = Convert.ToSingle(rows[9]);

                chg_rate = Convert.ToSingle(rows[10]);
                chg_num = Convert.ToInt32(rows[11]);
                chg_value = Convert.ToSingle(rows[12]);
                total = Convert.ToSingle(rows[13]);
                total_on = Convert.ToSingle(rows[14]);
                }
                catch (Exception e) { Console.WriteLine(e.Message + "\n" + line); }
            }
            public CLineContents() { }
            public string time;
            public string code;
            public string name;
            public float last;
            public float high;
            public float low;
            public float start;
            public float yes_last;

            public float price;
            public float rate;
            public float chg_rate;
            public int chg_num;
            public float chg_value;
            public float total;
            public float total_on;
        }

        public class CStockContents
        {
            List<CLineContents> lstLc = new List<CLineContents>();
            public void Push(CLineContents lc)
            {
                // decrease
                nd.CheckDecrease(lc);
                if (nd.stop)
                {
                    lstTest.Add(nd);
                    nd = new NumDecreaseTest();
                    nd.CheckDecrease(lc);
                }

                for (int i = lstTest.Count - 1; i >= 0; --i)
                {
                    lstTest[i].CheckDecrease(lc);
                    if (lstTest[i].checkCount <= 0)
                    {
                        lstResult.Add(lstResult[i]);
                        lstTest.RemoveAt(i);
                    }
                }

                // extremum
                if (MaxNum < lc.chg_num)
                {
                    MaxNum = lc.chg_num;
                }
                if (MinNum > lc.chg_num)
                {
                    MinNum = lc.chg_num;
                }
                if (MaxPrice < lc.last)
                {
                    MaxPrice = lc.last;
                }
                if (MinPrice > lc.last)
                {
                    MinPrice = lc.last;
                }

                // average
                if (lstAverage.Count > 0)
                    lstAverage[lstAverage.Count - 1].tomorrowRate = lc.rate;

                lstAverage.Add(new CAverageValue(lc));
            }

            public int MaxNum = int.MinValue;
            public int MinNum = int.MaxValue;
            public float MaxPrice = float.MinValue;
            public float MinPrice = float.MaxValue;

            static float GetInflow(CLineContents lc)
            {
                return (float)lc.chg_value - lc.yes_last * lc.chg_num;
            }
            List<CAverageValue> lstAverage = new List<CAverageValue>();
            public class CAverageValue
            {
                public CAverageValue(CLineContents lc)
                {
                    amplitude = (lc.high - lc.low) / lc.yes_last;
                    priceAverage = (lc.last - lc.start) / 2;
                    numAverage = GetInflow(lc);
                    time = lc.time;
                    rate = lc.rate;
                }
                public float amplitude;
                public float numAverage;
                public float priceAverage;
                public string time;
                public float rate;
                public float tomorrowRate;
            }



            public List<NumDecreaseTest> lstResult = new List<NumDecreaseTest>();
            public List<NumDecreaseTest> lstTest = new List<NumDecreaseTest>();
            NumDecreaseTest nd = new NumDecreaseTest();
            public class NumDecreaseTest
            {
                public string name;
                public string time;
                public float decreaseAverage = float.MaxValue;
                public int count;

                public bool stop = false;
                public float stopValue;
                public int checkCount = 3;
                public List<float> afterRate = new List<float>();

                public void CheckDecrease(CLineContents lc)
                {
                    if (stop) // check result en queue
                    {
                        afterRate.Add((lc.price - stopValue) / stopValue);
                        --checkCount;
                    }
                    else // decrease
                    {
                        if (lc.chg_num < decreaseAverage && lc.rate > -9.8f && lc.rate < -1f
                            || count >= 3 && lc.chg_num < decreaseAverage * 1.05f && lc.rate > -9.8f && lc.rate < 1f
                            )
                        {
                            if (count == 0)
                            {
                                decreaseAverage = lc.chg_num;
                                ++count;
                                name = lc.name;
                            }
                            else
                            {
                                decreaseAverage = (float)(decreaseAverage + lc.chg_num) / 2;
                            }
                            stopValue = lc.price;
                            time = lc.time;
                        }
                        else if (lc.rate <= -9.8f)
                        {
                        }
                        else
                        {
                            if (count >= 3)
                            {
                                stop = true;
                            }
                            else
                            {
                                count = 0;
                                decreaseAverage = float.MaxValue;
                            }
                        }
                    }
                }
            }


        }
    }

    public class GetHttpData
    {
        public static string libCode = "http://quotes.money.163.com/hs/service/diyrank.php?page=0&count=3000&sort=PERCENT&order=desc&query=STYPE:EQA&fields=CODE";
        public static string urlFormat = "http://quotes.money.163.com/service/chddata.html?code={0}{1}&start={2}&end={3}";
        public static string nowFormat = "http://api.money.126.net/data/feed/{0}";
        public static string GetUrltoHtml(string Url, string type = "UTF-8")
        {
            try
            {
                System.Net.WebRequest wReq = System.Net.WebRequest.Create(Url);

                System.Net.WebResponse wResp = wReq.GetResponse();
                System.IO.Stream respStream = wResp.GetResponseStream();

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

    public class CheckAllExistStock
    {
        public int sh = 600000;

        public int sz = 000000;
        public int sz_small = 002000;
        public int cy = 300000;

        public string model = "http://hq.sinajs.cn/list=sh601006";
    }

    public class StoreItemStock
    {
        public static string model = "http://quotes.money.163.com/service/chddata.html?code=1000001&start=19910403&end=20150923";
    }

    public class AlanesizeItemStock
    {
    }

    public class HistoryResult
    {
    }
}
