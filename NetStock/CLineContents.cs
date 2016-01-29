using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetStock
{
    class CLineContents
    {
        public CLineContents(string line)
        {
            try
            {
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
                chg_num = Convert.ToInt64(rows[11]);
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
        public long chg_num;
        public float chg_value;
        public float total;
        public float total_on;
    }
}
