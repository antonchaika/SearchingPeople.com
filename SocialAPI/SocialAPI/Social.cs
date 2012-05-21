using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using hp = HtmlAgilityPack;

namespace Parsing1
{
    public class education
    {
        public string[] univer = new string [3];
        public string[] faculty = new string[3];
        public string[] cafedra = new string[3];
        public string[] form = new string[3];
        public string[] state = new string[3];
        public string[] school = new string[3];
    }

    class Man
    {
        public string name;
        public string photo;
        public string nick;
        public string birthday;
        public string age;
        public string city1;
        public string city2;
        public string family;
        public string phone1;
        public string phone2;
        public string skype;
        public string twitter;
        public string livejournal;
        public string site;
        public education edc = new education();
        public string[] jobs = new string[10];
        public string getnick(string buf1,string buf2)
        {
            if (buf1 == buf2) return "";
            int i=0,j=buf1.Length-1,k=buf2.Length-1;
            while (buf1[i] == buf2[i]) i++;
            while (buf1[j] == buf2[k]) { j--; k--; }
            string str = buf2.Substring(i, k - i+1);
            return str;
        }
        public string[] geturls(string m)
        {
            string[] str = new string[5];
            int i = 0;
            string pattern = ("http:.*?<");
            MatchCollection m1 = Regex.Matches(m, pattern);
            foreach (Match mat in m1)
            {
                str[i] = mat.Groups[0].ToString();
                str[i] = str[i].Substring(0, str[i].Length - 1);
                i++;
            }
            return str;
        }
    }
    class Program
    {
        static void vk_parsing(Man man,string url)
        {
            string buf1, buf2, buf3;
            int u_count = 0, s_count = 0, j_count = 0; ;
            StreamWriter sw = new StreamWriter("data.txt");
            hp.HtmlDocument doc = new hp.HtmlDocument();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            doc.Load(response.GetResponseStream(), Encoding.GetEncoding(1251));

            //name 
            hp.HtmlNode node = doc.DocumentNode.SelectSingleNode("//title");
            man.name = node.InnerText;
            buf1 = man.name;
            sw.WriteLine(man.name);
            //nick
            node = doc.DocumentNode.SelectSingleNode("//h4[@class=\"simple page_top\"]/div[@class=\"page_name\"]");
            buf2 = node.InnerText;
            man.nick = man.getnick(buf1, buf2);
            sw.WriteLine(man.nick);

            //photo
            node = doc.DocumentNode.SelectSingleNode("//div[@id=\"profile_avatar\"]/a/img");
            man.photo = node.Attributes["src"].Value;
            sw.WriteLine(man.photo);

            hp.HtmlNodeCollection nnodes = doc.DocumentNode.SelectNodes("//div[@class=\"label fl_l\"]");
            hp.HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//div[@class=\"labeled fl_l\"]");
            int i = 0;
            while (i < nnodes.Count)
            {
                switch (nnodes[i].InnerText)
                {
                    case "День рождения:":
                        man.birthday = nodes[i].InnerText;
                        sw.WriteLine(man.birthday);
                        i++;
                        break;

                    case "Родной город:":
                        man.city1 = nodes[i].InnerText;
                        sw.WriteLine(man.city1);
                        i++;
                        break;

                    case "Семейное положение:":
                        man.family = nodes[i].InnerText;
                        sw.WriteLine(man.family);
                        i++;
                        break;

                    case "Город:":
                        man.city2 = nodes[i].InnerText;
                        sw.WriteLine(man.city2);
                        i++;
                        break;

                    case "Моб. телефон:":
                        man.phone1 = nodes[i].InnerText;
                        sw.WriteLine(man.phone1);
                        i++;
                        break;

                    case "Дом. телефон:":
                        man.phone2 = nodes[i].InnerText;
                        sw.WriteLine(man.phone2);
                        i++;
                        break;

                    case "Skype:":
                        man.skype = nodes[i].InnerText;
                        sw.WriteLine(man.skype);
                        i++;
                        break;

                    case "Twitter:":
                        man.twitter = nodes[i].InnerText;
                        sw.WriteLine(man.twitter);
                        i++;
                        break;

                    case "LiveJournal:":
                        man.livejournal = nodes[i].InnerText;
                        sw.WriteLine(man.livejournal);
                        i++;
                        break;

                    case "Веб-сайт:":
                        man.site = nodes[i].InnerText;
                        sw.WriteLine(man.site);
                        i++;
                        break;

                    case "ВУЗ:":
                        man.edc.univer[u_count] = nodes[i].InnerText;
                        sw.WriteLine(man.edc.univer[u_count]);
                        u_count++;
                        i++;
                        break;

                    case "Факультет:":
                        man.edc.faculty[u_count] = nodes[i].InnerText;
                        sw.WriteLine(man.edc.faculty[u_count]);
                        i++;
                        break;

                    case "Кафедра:":
                        man.edc.cafedra[u_count] = nodes[i].InnerText;
                        sw.WriteLine(man.edc.cafedra[u_count]);
                        i++;
                        break;

                    case "Форма обучения:":
                        man.edc.form[u_count] = nodes[i].InnerText;
                        sw.WriteLine(man.edc.form[u_count]);
                        i++;
                        break;

                    case "Статус:":
                        man.edc.state[u_count] = nodes[i].InnerText;
                        sw.WriteLine(man.edc.state[u_count]);
                        i++;
                        break;

                    case "Гимназия:":
                        man.edc.school[s_count] = nodes[i].InnerText;
                        man.edc.school[s_count] = "Гимназия " + man.edc.school[s_count];
                        sw.WriteLine(man.edc.school[s_count]);
                        s_count++;
                        i++;
                        break;

                    case "Школа:":
                        man.edc.school[s_count] = nodes[i].InnerText;
                        man.edc.school[s_count] = "Школа " + man.edc.school[s_count];
                        sw.WriteLine(man.edc.school[s_count]);
                        s_count++;
                        i++;
                        break;

                    case "Лицей:":
                        man.edc.school[s_count] = nodes[i].InnerText;
                        man.edc.school[s_count] = "Лицей " + man.edc.school[s_count];
                        sw.WriteLine(man.edc.school[s_count]);
                        s_count++;
                        i++;
                        break;

                    case "Место работы:":
                        man.jobs[j_count] = nodes[i].InnerText;
                        sw.WriteLine(man.jobs[j_count]);
                        j_count++;
                        i++;
                        break;


                    default:
                        i++;
                        break;
                }
            }
            sw.Close();
        }
        static void od_parsing(Man man,string url)
        {
            StreamWriter sw = new StreamWriter("data2.txt");
            hp.HtmlDocument doc = new hp.HtmlDocument();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            doc.Load(response.GetResponseStream(), Encoding.UTF8);

            //name 
            hp.HtmlNode node = doc.DocumentNode.SelectSingleNode("//a[@class=\"url fn\"]");
            man.name = node.InnerText;
            sw.WriteLine(man.name);
            //photo
            node = doc.DocumentNode.SelectSingleNode("//link[@rel=\"image_src\"]");
            man.photo = node.Attributes["href"].Value;
            sw.WriteLine(man.photo);
            //age
            node = doc.DocumentNode.SelectSingleNode("//span[@itemprop=\"description\"]");
            man.age = node.InnerText;
            sw.WriteLine(man.age);
           
            //city2
            node = doc.DocumentNode.SelectSingleNode("//span[@class=\"locality\"]");
            man.city2 = node.InnerText;
            sw.WriteLine(man.city2);
            sw.Close();

            //
        }

        static void mm_parsing(Man man, string url)
        {
            StreamWriter sw = new StreamWriter("data3.txt");
            hp.HtmlDocument doc = new hp.HtmlDocument();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            doc.Load(response.GetResponseStream(), Encoding.GetEncoding(1251));

            //nick
            string pattern = "mail/.*";
            Match m = Regex.Match(url, pattern);
            man.nick = m.Groups[0].ToString().Substring(5);
            sw.WriteLine(man.nick);

            //name 
            hp.HtmlNode node = doc.DocumentNode.SelectSingleNode("//h1[@class=\"reset-style\"]");
            man.name = node.InnerText;
            sw.WriteLine(man.name);

            //city 
            hp.HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//div[@class=\"mb3\"]/span[@class=\"mf_grey\"]");
            hp.HtmlNodeCollection nodes2 = doc.DocumentNode.SelectNodes("//div[@class=\"mb3\"]/span[@class=\"mf_nobr\"]");
            int j=0;
            while (j<nodes.Count)
            {
                switch (nodes[j].InnerText)
                {
                    case "Откуда:":
                        node = doc.DocumentNode.SelectSingleNode("//div[@class=\"mb3\"]");
                        man.city1 = node.InnerText;
                        int i=0;
                        while (man.city1[i] != ',') i++;
                        i++;
                        while (man.city1[i] != ',') i++;
                        i += 2;
                        man.city1 = man.city1.Substring(i);
                        sw.WriteLine(man.city1);
                        j++;
                        break;
                    case "Родилась: ":
                    case "Родился: ":
                        man.birthday = nodes2[j].InnerText;
                        sw.WriteLine(man.birthday);
                        j++;
                        break;
                    default:
                        j++;
                        break;
                }
            }
            //school
            node = doc.DocumentNode.SelectSingleNode("//div[@class=\"clBorder clBlue mf_ohd mb10\"]/div/a/b");
            man.edc.school[0] = node.InnerText;
            node = doc.DocumentNode.SelectSingleNode("//div[@class=\"clBorder clBlue mf_ohd mb10\"]/div/span");
            man.edc.school[0] = man.edc.school[0] + " " + node.InnerText;
            sw.WriteLine(man.edc.school[0]);

            sw.Close();
        }

        static void fb_parsing(Man man, string url)
        {
            string buf1, buf2, buf3;
            int u_count = 0, s_count = 0, j_count = 0;
            //url = url + "/info";
            StreamWriter sw = new StreamWriter("data3.txt");
            hp.HtmlDocument doc = new hp.HtmlDocument();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            doc.Load(response.GetResponseStream(), Encoding.UTF8);

            //name 
            hp.HtmlNode node = doc.DocumentNode.SelectSingleNode("//title");
            man.name = node.InnerText;
            sw.WriteLine(man.name);
           
            sw.Close();
        }

        static void tw_parsing(Man man, string url)
        {
            StreamWriter sw = new StreamWriter("data3.txt");
            hp.HtmlDocument doc = new hp.HtmlDocument();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.AllowAutoRedirect = false;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            doc.Load(response.GetResponseStream(), Encoding.UTF8);

            //name 
            hp.HtmlNode node = doc.DocumentNode.SelectSingleNode("//h1[@class=\"fullname\"]");
            man.name = node.InnerText;
            sw.WriteLine(man.name);
            sw.Close();
        }
        static void Main(string[] args)
        {
            Man man = new Man();
            vk_parsing(man,"http://vk.com/castiar");
            //od_parsing(man,"http://www.odnoklassniki.ru/profile/148512003810");
            //tw_parsing(man, "http://twitter.com/#!/castiar");
           // fb_parsing(man, "https://www.facebook.com");
          //  mm_parsing(man, "http://my.mail.ru/mail/syromolotik");
           
        }
    }
}
