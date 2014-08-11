using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.IO;

namespace WindowsFormsApplication1
{
    class CommandCfg
    {
        public static string GetClipboard()
        {
            try
            {
                StreamReader sr = new StreamReader(Config.CommandCfgFile);
                return sr.ReadLine();//header
            }
            catch
            {
                return null;
            }
            finally
            {

            }
        }

        public static string GetCommand()
        {
            try
            {
                StreamReader sr = new StreamReader(Config.CommandLineFile);
                return sr.ReadLine();
            }
            catch
            {
                return null;
            }
            finally
            {

            }
        }

    }
}
