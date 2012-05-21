using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;

namespace ThematicAnalyser
{
    class Program
    {
        static void Main(string[] args)
        {
            string s = new StreamReader("data2.txt").ReadToEnd();
            Analyzer an = new Analyzer(s);
            SortedList list = an.GetKey(20, 20, 4);
            Dictionary<string, double> dict = new Dictionary<string, double>();
            foreach (IE_Value item in list.Values)
            {
                dict.Add(item.IE.Name, item.Value);
            }
            var r = from item in dict
                    orderby item.Value descending
                    select item;
            foreach(KeyValuePair<string, double> item in r)
            {
                Console.WriteLine(item.Key + " " + item.Value);
            }
        }
    }
}