using System.IO;
using System.Collections.Generic;

namespace WindowsFormsApplication1
{
    class Configuration
    {
        public static List<ConfigItem> cfgItems;
        public struct ConfigItem
        {
            public string Order;
            public string Name;
            public string IP;
        }
        public static bool ReadConfig(string FileName)
        {
            StreamReader sr = new StreamReader(FileName);
            string line = sr.ReadLine();//header
            line = sr.ReadLine();
            cfgItems = new List<ConfigItem>();
            while (line != null)
            {
                string[] strs = line.Split(new char[]{'\t'});
                ConfigItem item = new ConfigItem();
                item.Order = strs[0];
                item.Name = strs[1];
                cfgItems.Add(item);
                line = sr.ReadLine();
            }
            
            return true;
        }
    }
}