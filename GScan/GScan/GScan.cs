using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using System.Net;
using System.IO;
using System.Threading;
using System.Xml;
using GQueryAPI;
using VQueryAPI;
using AQueryAPI;
using SocialAPI;
using HtmlAgilityPack;

namespace GScan
{
    public class ScanSevice
    {
        public static void Main(string[] args)
        {
            ScanSevice service = new ScanSevice();
            service.GetRawDataVK();
            service.GetRawDataG();
            //Thread threadVK = new Thread(service.GetRawDataVK);
            //Thread threadG = new Thread(service.GetRawDataG);
            //threadVK.Start();
            //Thread.Sleep(1000);
            //service.GetRawDataG();
            //AutoResetEvent auto = new AutoResetEvent(false);
            //Timer t = new Timer(service.GetRawDataG, auto, 1500, Timeout.Infinite);
            //threadG.Start();
        }
        object _lock = new object();
        internal string htmlpage;
        internal Dictionary<SocialAPI.Human, string[]> humans;
        internal List<SocialAPI.Human> humanlist;
        internal AlchemyAPI alchemy;
        public ScanSevice()
        {
            alchemy = new AlchemyAPI();
            alchemy.LoadAPIKey("api_key.txt");
            humanlist = new List<SocialAPI.Human>();
            humans = new Dictionary<Human, string[]>();
        }
        public void GetRawDataVK()
        {
            VQueryService vservice = new VQueryService("вашкевич илья", 0, "belarus");
            List<VQueryResult> r = vservice.GetResult();
            //
            if (r.Count != 0) Console.WriteLine("Status OK in VK results");
            //
            int i = 0;
            SocialAPI.Human man = new SocialAPI.Human();
            foreach (var item in r)
            {
                long begin = DateTime.Now.Ticks;
                if (i == 1) break;
                i++;
                Thread.Sleep(2000);
                htmlpage = Social.vk_parsing(man, item.Url);
                SmartAnalyse(true);
                if (string.IsNullOrEmpty(man.site) || man.site.IndexOfAny("абвгдеёжзиклмнопрстуфхцчшщъыьэюя".ToCharArray()) != -1)
                {
                    man.site = item.Url;
                }
                lock (_lock)
                {
                    humanlist.Add(man);
                }
                TimeSpan span = new TimeSpan(DateTime.Now.Ticks - begin);
                if (span.TotalSeconds < 5)
                {
                    int duration = 5 - (int)span.TotalSeconds;
                    Console.WriteLine("Result duration" + duration.ToString());
                    Thread.Sleep(duration*1000);
                }
            }
        }
        public void GetRawDataG()
        {
            for (int i = 0; i < humanlist.Count; i++)
            {
                double rate = 0;
                long begin = DateTime.Now.Ticks;
                GQueryService service = new GQueryService(humanlist[i].name + " " + humanlist[i].city1);
                List<GQueryResult> result = service.GetResult();
                //
                if (result.Count != 0) Console.WriteLine("Status OK in Google results");
                //
                foreach (var item in result)
                {
                    if (GetResponse(item.Url, Encoding.UTF8))
                    {
                        humans.Add(humanlist[i], geturls(htmlpage));
                        SmartAnalyse(false);
                        rate = SmartGoogleAnalyse();
                    }
                }
                TimeSpan span = new TimeSpan(DateTime.Now.Ticks - begin);
                if (span.TotalSeconds < 5) Thread.Sleep((5 - (int)span.TotalSeconds)*1000);
            }
        }
        public double SmartGoogleAnalyse()
        {
            return 0;
        }
        public void SmartAnalyse(bool vk)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(htmlpage);
            HtmlNode node = null;
            if (vk)
            {
                node = doc.DocumentNode.SelectSingleNode("//div[@id=\"profile_wide\"]");
            }
            else
            {
                node = doc.DocumentNode.SelectSingleNode("//body");
            }
            string s = node.InnerText;
            StreamWriter sw = new StreamWriter("data2.txt");
            sw.Write(s);
            sw.Close();
            string[] source = s.Split(new char[] { '.', '?', '!', ' ', ';', ':', ',' }, StringSplitOptions.RemoveEmptyEntries);
            var dist = from word in source
                       where word.IndexOfAny("1234567890!@#$%^&*()\\|+~`-/-+_ ".ToCharArray()) == -1 && word.Length >= 4
                       group word by word into wordsgroup
                       orderby wordsgroup.Count() descending
                       select wordsgroup;
            Dictionary<string, int> words = new Dictionary<string, int>();
            int i = 0;
            foreach(var item in dist)
            {
                if ((i = item.Count()) >= 3)
                {
                    words.Add(item.Key, i);
                }
            }
            //string category = alchemy.TextGetCategory(s);
            //XmlDocument xml = new XmlDocument();
            //xml.LoadXml(category);
            //Console.WriteLine(xml.SelectSingleNode("//category").InnerText);
        }
        public string[] SmartQueryToGoogle(int i)
        {
            string[] nik = null;
            if (humanlist[i].site.IndexOf("vk") > -1)
            {
                string urlnik = humanlist[i].site.Substring(14);
            }
            return nik;
        }
        private bool GetResponse(string url, Encoding encoding)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            //request.Referer = "tut.by"; //temporary referer
            request.AllowAutoRedirect = true;
            //request.Proxy = new WebProxy("79.175.158.156", 8080);
            request.UserAgent = "Opera/9.80 (Windows NT 6.1; U; ru)";
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response == null)
                {
                    return false;
                }
                using (var reader = new StreamReader(response.GetResponseStream(), encoding))
                {
                    string enc = response.ContentEncoding;
                    htmlpage = reader.ReadToEnd();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public string[] geturls(string html)
        {
            string[] str = new string[5];
            int i = 0;
            string pattern = ("http:.*?<");
            MatchCollection m1 = Regex.Matches(html, pattern);
            foreach (Match mat in m1)
            {
                str[i] = mat.Groups[0].ToString();
                str[i] = str[i].Substring(0, str[i].Length - 1);
                i++;
            }
            return str;
        }
    }
}