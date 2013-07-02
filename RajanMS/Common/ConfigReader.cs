using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Common
{
    public sealed class ConfigReader
    {
        public string Path { get; private set; }

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        static extern uint GetPrivateProfileString(
           string lpAppName,
           string lpKeyName,
           string lpDefault,
           StringBuilder lpReturnedString,
           uint nSize,
           string lpFileName);

        public ConfigReader(string file)
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = string.Concat(basePath, file);

            if (!File.Exists(filePath))
                throw new Exception("Cannont find file");

            Path = filePath;
        }

        public string ReadString(string section, string key)
        {
            StringBuilder temp = new StringBuilder(255);
            
            GetPrivateProfileString(section, key, string.Empty, temp, 255, Path);
            
            return temp.ToString();
        }

        public int ReadInt(string section, string key)
        {
            return Int32.Parse(ReadString(section, key));
        }
    }
}
