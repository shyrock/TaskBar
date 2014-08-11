using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using Microsoft.Win32;

namespace WindowsFormsApplication1
{
    class RunAtStartup
    {
        public static bool Enable(string PathName)
        {
            RegistryKey reg = null;
            try
            {  
                if (!System.IO.File.Exists(PathName))  
                    throw new Exception("File not exist!");  
                String name = PathName.Substring(PathName.LastIndexOf(@"\") + 1);  
                reg = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                if (reg == null)  
                {
                    reg = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");  
                }
                reg.SetValue(name, PathName);
            }
            catch
            {
            }
            finally
            {
                if (reg != null)
                    reg.Close();  
            }
            return true;
        }
        public static bool Disable(string PathName)
        {
            RegistryKey reg = null;
            try
            {
                if (!System.IO.File.Exists(PathName))
                    throw new Exception("File not exist!");
                String name = PathName.Substring(PathName.LastIndexOf(@"\") + 1);
                reg = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                if (reg == null)
                {
                    return false;
                }
                reg.SetValue(name, false);
            }
            catch
            {
            }
            finally
            {
                if (reg != null)
                    reg.Close();
            }
            
            return true;
        }
    }
}
