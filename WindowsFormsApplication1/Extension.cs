using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    static class Extension
    {
        public static bool IsCB(this string str)
        {
            try
            {
                return str.Substring(0, 2) == "CB";
            }
            catch (ArgumentOutOfRangeException e)
            {
                return false;
            }
        }

        public static bool IsCL(this string str)
        {
            try
            {
                return str.Substring(0, 2) == "CL";
            }
            catch (ArgumentOutOfRangeException e)
            {
                return false;
            }
        }

        public static string GetCommandLine(this string str)
        {
            if (!str.IsCL())
            {
                return null;
            }
            try
            {
                return str.Substring(2, str.Length - 2);
            }
            catch (ArgumentOutOfRangeException e)
            {
                return null;
            }
        }

        public static string GetClipboard(this string str)
        {
            if (!str.IsCB())
            {
                return null;
            }
            try
            {
                return str.Substring(2, str.Length - 2);
            }
            catch (ArgumentOutOfRangeException e)
            {
                return null;
            }
        }
        public static string ExecuteCmdLine(this string str)
        {
            try
            {
                //Conctruct Process 
                Process process = new Process();
                //
                process.StartInfo.FileName = "cmd.exe";
                //Close Shell
                process.StartInfo.UseShellExecute = false;
                //Redirect STDIN/STDOUT/STDERROR
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                //No Window
                process.StartInfo.CreateNoWindow = true;
                //
                process.Start();
                //Execute
                process.StandardInput.WriteLine(str.GetCommandLine());
                //process.StandardInput.WriteLine("netstat");
                //process.StandardInput.WriteLine("exit");
                //Get result!!!!不返回
                string strRst = process.StandardOutput.ReadToEnd();
                MessageBox.Show(strRst);
                return strRst;
            }
            catch
            {
                return null;
            }
        }
    }
}
