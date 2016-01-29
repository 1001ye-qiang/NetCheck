using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetStock
{
    class CTime
    {
        public string begin;
        public string end;
        public CTime()
        {
            // TIME
            DateTime dt = new DateTime(DateTime.Now.Ticks);
            end = dt.ToString("yyyyMMdd");

            DateTime dt2 = dt.AddMonths(-12);
            //dt2 = dt2.AddDays(-20);
            begin = dt2.ToLocalTime().ToString("yyyyMMdd");
        }
        public CTime(string begin, string end)
        {
            this.begin = begin;
            this.end = end;
        }
    }
}
