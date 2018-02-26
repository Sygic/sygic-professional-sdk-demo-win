using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace WindowsDemo
{
    public class MyListener : ApplicationAPI.OnSearchListener
    {
        private ListBox resultList = null;
        public MyListener(ListBox results)
        {
            resultList = results;
        }
        public override void OnResult(string input, List<ApplicationAPI.SWayPoint> results, int resultCode)
        {
            if (resultList != null)
            {
                resultList.Invoke(new Action(() =>
                {
                    resultList.Items.Clear();
                }));

                lock (resultList)
                {
                    foreach (var pt in results)
                    {
                        string res = pt.Location.lX + ", " + pt.Location.lY + ", " + pt.GetAddress();

                        resultList.Invoke(new Action(() =>
                        {
                            resultList.Items.Add(res);
                        }));

                    }
                }
            }

        }
    }
}
