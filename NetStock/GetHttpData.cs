using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetStock
{
    class GetHttpData
    {
        public static string sh_code = "0000001";
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
}
