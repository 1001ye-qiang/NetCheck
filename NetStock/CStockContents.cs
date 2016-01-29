using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetStock
{
    class CStockContents
    {
        public List<CLineContents> lstLc = new List<CLineContents>();
        public void Push(CLineContents lc)
        {
            lstLc.Add(lc);

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

        public string GetResult()
        {
            return "";
        }

        public long MaxNum = long.MinValue;
        public long MinNum = long.MaxValue;
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
