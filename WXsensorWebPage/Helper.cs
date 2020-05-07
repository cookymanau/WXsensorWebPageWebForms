using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WXsensorWebPage
{
    public static class myHelper
    {

        public static string NullOrEmpty(string stdX)
        {

            if (stdX == null || stdX == string.Empty)
            {
                stdX = "1";
            }

            return stdX;
        }


        public static void SetCursorWait() => Cursor.Current = Cursors.WaitCursor;
        public static void SetCursorNormal() => Cursor.Current = Cursors.Default;
         
        public static decimal MinMax(decimal a, decimal b) => Math.Max(a, b);

    }
}
