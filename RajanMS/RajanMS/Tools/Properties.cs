using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace RajanMS.Tools
{
    public sealed class Config
    {
        private Dictionary<string, string> m_collection;

        public Config(string file)
        {
            m_collection = new Dictionary<string, string>();

            var lines = File.ReadAllLines(file);

            foreach (string line in lines)
            {
                int index = line.IndexOf('=');

                string header = line.Substring(0, index);
                string body = line.Remove(0, index + 1);

                m_collection.Add(header, body);
            }
        }

        public string this[string header]
        {
            get
            {
                return m_collection[header];
            }
        }

        public string GetString(string header)
        {
            return m_collection[header];
        }
        public int GetInt(string header)
        {
            return Convert.ToInt32(m_collection[header]);
        }
    }
}
