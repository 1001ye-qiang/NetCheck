using LitJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetStock
{
    class CAllStockCode
    {
        public List<string> lstCode = new List<string>();
        public List<string> sh_code = new List<string>();
        public List<string> sz_code_a = new List<string>();
        public List<string> sz_code_s = new List<string>();
        public List<string> sz_code_c = new List<string>();
        public CAllStockCode()
        {
            // LST CODE

            string sdata = GetHttpData.GetUrltoHtml(GetHttpData.libCode);

            JsonData jdata = JsonMapper.ToObject(sdata);

            JsonData jdata2 = jdata["list"];

            for (int i = 0; i < jdata2.Count; ++i)
            {
                string code = (string)jdata2[i]["CODE"];
                lstCode.Add(code);
                if (code.StartsWith("06"))
                    sh_code.Add(code);
                else if(code.StartsWith("1002"))
                    sz_code_s.Add(code);
                else if(code.StartsWith("130"))
                    sz_code_c.Add(code);
                else
                    sz_code_a.Add(code);
            }
        }
        public CAllStockCode(string code)
        {
            lstCode.Add(code);
        }
    }
}
