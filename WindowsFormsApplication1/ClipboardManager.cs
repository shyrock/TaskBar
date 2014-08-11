using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace WindowsFormsApplication1
{ 

    class ClipboardManager
    {
        private static string clipboardBackup = null;


        public static bool Set(string data)
        {
            try
            {
                clipboardBackup = Clipboard.GetText();
                Clipboard.SetText(data);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static bool Recover()
        {
            //try
            //{
                if (clipboardBackup != null)
                {
                    Clipboard.SetText(clipboardBackup);
                    clipboardBackup = null;
                    return true;
                }
            //}
            /*catch (Threading.ThreadStateException e)
            {
                Console.WriteLine("ThreadStateException");

            catch
            {
                return false;
            }*/
            return false;
        }

    }
}
