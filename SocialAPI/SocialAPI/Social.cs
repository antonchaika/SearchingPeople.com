using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using hp = HtmlAgilityPack;

namespace SocialAPI
{
    public class Education
    {
        public string univer;
        public string faculty;
        public string cafedra;
        public string form;
        public string state;
    }

    public class Man
    {
        public string name;
        public string photo;
        public ArrayList nicks = new ArrayList();
        public string birthday;
        public string age;
        public string[] city = new string[2];
        public string family;
        public string[] phone = new string[2];
        public string skype;
        public int u_count;
        public Education[] edc = new Education[3];
        public ArrayList schools = new ArrayList();
        public ArrayList jobs = new ArrayList();
        public ArrayList links = new ArrayList();
        public bool Private { get; private set; }
        public Man()
        {
            int i;
            for (i = 0; i < 3; i++)
                edc[i] = new Education();
        }
        public void check()
        {
            Console.WriteLine(name);
            Console.WriteLine(photo);
            Console.WriteLine(birthday);
            Console.WriteLine(age);
            Console.WriteLine(city[0]);
            Console.WriteLine(city[1]);
            Console.WriteLine(family);
            Console.WriteLine(phone[0]);
            Console.WriteLine(phone[1]);
            Console.WriteLine(skype);
            foreach (Education item in edc)
            {
                Console.WriteLine(item.univer);
                Console.WriteLine(item.faculty);
                Console.WriteLine(item.cafedra);
                Console.WriteLine(item.form);
                Console.WriteLine(item.state);
            }
            foreach (string item in schools)
            {
                Console.WriteLine(item);
            }
            foreach (string item in jobs)
            {
                Console.WriteLine(item);
            }
            foreach (string item in links)
            {
                Console.WriteLine(item);
            }
            foreach (string item in nicks)
            {
                Console.WriteLine(item);
            }
            Console.ReadLine();
        }
        public void getnick(string buf1, string buf2)//получить ник ВКонтакте
        {
            if (buf1 == buf2) return;
            int i = 0, j = buf1.Length - 1, k = buf2.Length - 1;
            while (buf1[i] == buf2[i]) i++;
            while (buf1[j] == buf2[k]) { j--; k--; }
            buf2 = buf2.Substring(i, k - i + 1);
            nicks.Add(buf2.Replace("&quot;", ""));
        }
        public void get_links(hp.HtmlDocument doc)//получить все ссылки со страницу
        {
            int i = 0;
            string link;
            hp.HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//a[@href]");
            foreach (hp.HtmlNode node in nodes)
            {
                link = node.InnerText;
                if (link.Contains("http") == true)
                {
                    if (link[link.Length - 1] == '/') link = link.Substring(0, link.Length - 1);
                    links.Add(link);
                    i++;
                }
            }
        }
        public int compare_links(hp.HtmlDocument doc)//найти все ссылки на странице и сравнить их с Man.links
        {
            int i, j;
            i = 0;
            string link;
            hp.HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//a[@href]");
            foreach (hp.HtmlNode node in nodes)
            {
                link = node.InnerText;
                if (link.Contains("http") == true)
                {
                    if (link[link.Length - 1] == '/') link = link.Substring(0, link.Length - 1);
                    for (j = 0; j < links.Count; j++)
                        if (links[j] != null && link == (string)links[j]) i++;
                }
            }
            return i;
        }
        public void get_birthday(string buf, string age)//вычисление даты рождения (Одноклассники)
        {
            string pattern = "[0-9][0-9]?";
            string date, month, year;
            Match m = Regex.Match(buf, pattern);
            if (m != null)
            {
                date = m.Groups[0].ToString();
                m = Regex.Match(age, pattern);
                age = m.Groups[0].ToString();
                if (buf.Contains("января")) month = "01";
                else if (buf.Contains("февраля")) month = "02";
                else if (buf.Contains("марта")) month = "03";
                else if (buf.Contains("апреля")) month = "04";
                else if (buf.Contains("мая")) month = "05";
                else if (buf.Contains("июня")) month = "06";
                else if (buf.Contains("июля")) month = "07";
                else if (buf.Contains("августа")) month = "08";
                else if (buf.Contains("сентября")) month = "09";
                else if (buf.Contains("октября")) month = "10";
                else if (buf.Contains("ноября")) month = "11";
                else month = "12";
                DateTime dtm = DateTime.Now;
                int b = dtm.Year - Int32.Parse(age);
                if (dtm.Month < Int32.Parse(month)) b--;
                else if (dtm.Month == Int32.Parse(month) && dtm.Day < Int32.Parse(date)) b--;
                year = b.ToString();
                birthday = date + "." + month + "." + year;
            }
        }
        public string eman(string name)//меняем местами имя и фамилию 
        {
            int i;
            string buf;
            i = name.IndexOf(' ');
            buf = name.Substring(0, i);
            name = name.Substring(i + 1, name.Length - i - 1);
            return (name + " " + buf);
        }
        public string normal_date(string date)//переводим дату в формат "DD.MM.YYYY"
        {
            if (date == null) return null;
            int i, k = 0;
            char[] date_n = new char[10];

            for (i = 0; i < 2; i++) //day
                if (date[i] >= '0' && date[i] <= '9')
                {
                    date_n[k] = date[i];
                    k++;
                }
            date_n[k] = '.';
            k++;
            i++;
            //month
            if (date[i] >= '0' && date[i] <= '9')
            {
                date_n[k] = date[i];
                i++; k++;
                date_n[k] = date[i];
                i++; k++;
            }
            else
            {
                if (date.Contains("январ") || date.Contains("Jan")) { date_n[k] = '0'; k++; date_n[k] = '1'; k++; }
                if (date.Contains("феврал") || date.Contains("Feb")) { date_n[k] = '0'; k++; date_n[k] = '2'; k++; }
                if (date.Contains("март") || date.Contains("Mar")) { date_n[k] = '0'; k++; date_n[k] = '3'; k++; }
                if (date.Contains("апрел") || date.Contains("Apr")) { date_n[k] = '0'; k++; date_n[k] = '4'; k++; }
                if (date.Contains("мая") || date.Contains("май") || date.Contains("May")) { date_n[k] = '0'; k++; date_n[k] = '5'; k++; }
                if (date.Contains("июн") || date.Contains("Jun")) { date_n[k] = '0'; k++; date_n[k] = '6'; k++; }
                if (date.Contains("июл") || date.Contains("Jul")) { date_n[k] = '0'; k++; date_n[k] = '7'; k++; }
                if (date.Contains("август") || date.Contains("Aug")) { date_n[k] = '0'; k++; date_n[k] = '8'; k++; }
                if (date.Contains("сентябр") || date.Contains("Sep")) { date_n[k] = '0'; k++; date_n[k] = '9'; k++; }
                if (date.Contains("октябр") || date.Contains("Oct")) { date_n[k] = '1'; k++; date_n[k] = '0'; k++; }
                if (date.Contains("ноябр") || date.Contains("Nov")) { date_n[k] = '1'; k++; date_n[k] = '1'; k++; }
                if (date.Contains("декабр") || date.Contains("Dec")) { date_n[k] = '1'; k++; date_n[k] = '2'; k++; }
            }

            while (i < date.Length && (date[i] < '0' || date[i] > '9')) i++;
            //year
            if (i == date.Length) return new string(date_n);
            date_n[k] = '.';
            k++;
            if (date.Length - i >= 4)
                while (i < date.Length && date[i] >= '0' && date[i] <= '9')
                {
                    date_n[k] = date[i];
                    k++; i++;
                }
            else
            {
                if (date[i] <= '2') { date_n[k] = '2'; k++; date_n[k] = '0'; k++; }
                else { date_n[k] = '1'; k++; date_n[k] = '9'; k++; }
                date_n[k] = date[i];
                k++; i++;
                date_n[k] = date[i];
            }
            return new string(date_n);
        }
        public bool find_date(string date, string text)//найти дату в различных форматах
        {
            if (date == null) return false;
            if (text.Contains(date)) return true;
            string day = date.Substring(0, 2);
            string month = date.Substring(3, 2);
            switch (month)
            {
                case "01": month = "января"; break;
                case "02": month = "февраля"; break;
                case "03": month = "марта"; break;
                case "04": month = "апреля"; break;
                case "05": month = "мая"; break;
                case "06": month = "июня"; break;
                case "07": month = "июля"; break;
                case "08": month = "августа"; break;
                case "09": month = "сентября"; break;
                case "10": month = "октября"; break;
                case "11": month = "ноября"; break;
                case "12": month = "декабря"; break;
            }
            date = day + " " + month;
            if (text.Contains(date)) return true;
            return false;
        }
        public string vk_parsing(string url)
        {
            string buf1, buf2;
            string html;
            hp.HtmlDocument doc = new hp.HtmlDocument();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(1251)))
            {
                html = sr.ReadToEnd();
            }
            doc.LoadHtml(html);
            if (!html.Contains("Страница скрыта"))
            {
                links.Add(url);
                string pattern = "vk.com/.*";
                Match m = Regex.Match(url, pattern);
                if (m.Groups[0].ToString().IndexOf("id") != 0)
                {
                    nicks.Add(m.Groups[0].ToString().Substring(7));
                }
                //name 
                hp.HtmlNode node = doc.DocumentNode.SelectSingleNode("//title");
                if (node != null)
                name = node.InnerText;
                buf1 = name;
                //nick
                node = doc.DocumentNode.SelectSingleNode("//h4[@class=\"simple page_top\"]/div[@class=\"page_name\"]");
                if (node != null)
                {
                    buf2 = node.InnerText;
                    getnick(buf1, buf2);
                }
                //photo
                node = doc.DocumentNode.SelectSingleNode("//div[@id=\"profile_avatar\"]/a/img");
                if (node == null)
                {
                    node = doc.DocumentNode.SelectSingleNode("//div[@id=\"profile_avatar\"]/img");
                }
                if (node != null)
                {
                    photo = node.Attributes["src"].Value;
                }
                hp.HtmlNodeCollection nnodes = doc.DocumentNode.SelectNodes("//div[@class=\"label fl_l\"]");
                hp.HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//div[@class=\"labeled fl_l\"]");
                int i;
                u_count = -1;
                i = 0;
                if (nnodes != null && nodes != null)
                while (i < nnodes.Count)
                {
                    switch (nnodes[i].InnerText)
                    {
                        case "День рождения:":
                            birthday = nodes[i].InnerText;
                            birthday = normal_date(birthday);
                            i++;
                            break;

                        case "Родной город:":
                            city[0] = nodes[i].InnerText;
                            i++;
                            break;

                        case "Семейное положение:":
                            family = nodes[i].InnerText;
                            i++;
                            break;

                        case "Город:":
                            city[1] = nodes[i].InnerText;
                            if (city[1] == city[0]) city[1] = null;
                            i++;
                            break;

                        case "Моб. телефон:":
                            phone[0] = nodes[i].InnerText;
                            if (phone[0] == "Информация скрыта") phone[0] = null;
                            i++;
                            break;

                        case "Дом. телефон:":
                            phone[1] = nodes[i].InnerText;
                            if (phone[1] == "Информация скрыта") phone[1] = null;
                            i++;
                            break;

                        case "Skype:":
                            skype = nodes[i].InnerText;
                            if (nicks.Contains(skype) == false) nicks.Add(skype);
                            i++;
                            break;

                        case "Twitter:":
                            links.Add("http://twitter.com/" + nodes[i].InnerText);
                            if (nicks.Contains(nodes[i].InnerText) == false) nicks.Add(nodes[i].InnerText);
                            i++;
                            break;

                        case "LiveJournal:":
                            links.Add("http://livejournal.com/" + nodes[i].InnerText);
                            if (nicks.Contains(nodes[i].InnerText) == false) nicks.Add(nodes[i].InnerText);
                            i++;
                            break;

                        case "ВУЗ:":
                            u_count++;
                            edc[u_count].univer = nodes[i].InnerText;
                            i++;
                            break;

                        case "Факультет:":
                            edc[u_count].faculty = nodes[i].InnerText;
                            i++;
                            break;

                        case "Кафедра:":
                            edc[u_count].cafedra = nodes[i].InnerText;
                            i++;
                            break;

                        case "Форма обучения:":
                            edc[u_count].form = nodes[i].InnerText;
                            i++;
                            break;

                        case "Статус:":
                            edc[u_count].state = nodes[i].InnerText;
                            i++;
                            break;

                        case "Гимназия:":
                            buf1 = nodes[i].InnerHtml.Replace("<br>", "\n");
                            schools.Add("Гимназия " + buf1);
                            i++;
                            break;

                        case "Школа:":
                            buf1 = nodes[i].InnerHtml.Replace("<br>", "\n");
                            schools.Add("Школа " + buf1);
                            i++;
                            break;

                        case "Лицей:":
                            buf1 = nodes[i].InnerHtml.Replace("<br>", "\n");
                            schools.Add("Лицей " + buf1);
                            i++;
                            break;

                        case "Место работы:":
                            buf1 = nodes[i].InnerHtml.Replace("<br>", "\n");
                            buf1 = buf1.Replace("&quot;", "\"");
                            jobs.Add(buf1);
                            i++;
                            break;

                        default:
                            i++;
                            break;
                    }
                }
                get_links(doc);
            }
            else
            {
                Private = true;
            }
            return html;
        }
        public string od_parsing(string url)
        {
            string html;
            hp.HtmlDocument doc = new hp.HtmlDocument();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(1251)))
            {
                html = sr.ReadToEnd();
            }
            doc.LoadHtml(html);
            string buf;
            //name 
            hp.HtmlNode node = doc.DocumentNode.SelectSingleNode("//a[@class=\"url fn\"]");
            name = node.InnerText;
            //photo
            node = doc.DocumentNode.SelectSingleNode("//link[@rel=\"image_src\"]");
            photo = node.Attributes["href"].Value;
            //age
            node = doc.DocumentNode.SelectSingleNode("//span[@itemprop=\"description\"]");
            age = node.InnerText;
            node = doc.DocumentNode.SelectSingleNode("//div[@class=\"lh-150 panelRounded\"]");
            buf = node.InnerText;
            get_birthday(buf, age);
            //city
            node = doc.DocumentNode.SelectSingleNode("//span[@class=\"locality\"]");
            city[0] = node.InnerText;
            int i;
            hp.HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//span[@class=\"tico\"]/a/i");
            hp.HtmlNodeCollection nnodes = doc.DocumentNode.SelectNodes("//span[@class=\"tico\"]/a/span");
            for (i = 0; i < nodes.Count; i++)
            {
                string type = nodes[i].Attributes["title"].Value;
                switch (type)
                {
                    case "школа":
                        schools.Add(nnodes[i].InnerText);
                        break;
                    case "университет":
                        if (u_count == 0) u_count = -1;
                        u_count++;
                        edc[u_count].univer = nnodes[i].InnerText;
                        break;
                    case "организация":
                        jobs.Add(nnodes[i].InnerText);
                        break;
                }
            }
            //check();
            return html;
        }
        public string mm_parsing(string url)
        {
            string html;
            hp.HtmlDocument doc = new hp.HtmlDocument();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(1251)))
            {
                html = sr.ReadToEnd();
            }
            doc.LoadHtml(html);
            //nick
            string pattern = "mail/.*";
            Match m = Regex.Match(url, pattern);
            nicks.Add(m.Groups[0].ToString().Substring(5));

            //name 
            hp.HtmlNode node = doc.DocumentNode.SelectSingleNode("//h1[@class=\"reset-style\"]");
            name = node.InnerText;

            hp.HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//div[@class=\"mb3\"]/span[@class=\"mf_grey\"]");
            hp.HtmlNodeCollection nodes2 = doc.DocumentNode.SelectNodes("//div[@class=\"mb3\"]/span[@class=\"mf_nobr\"]");
            int j = 0;
            while (j < nodes.Count)
            {
                switch (nodes[j].InnerText)
                {
                    case "Откуда:":
                        node = doc.DocumentNode.SelectSingleNode("//div[@class=\"mb3\"]");
                        city[0] = node.InnerText;
                        int i = 0;
                        while (city[0][i] != ',') i++;
                        i++;
                        while (city[0][i] != ',') i++;
                        i += 2;
                        city[0] = city[0].Substring(i);
                        j++;
                        break;
                    case "Родилась: ":
                    case "Родился: ":
                        birthday = nodes2[j].InnerText;
                        birthday = normal_date(birthday);
                        j++;
                        break;
                    default:
                        j++;
                        break;
                }
            }
            //school
            node = doc.DocumentNode.SelectSingleNode("//div[@class=\"clBorder clBlue mf_ohd mb10\"]/div/a/b");
            string buf = node.InnerText;
            node = doc.DocumentNode.SelectSingleNode("//div[@class=\"clBorder clBlue mf_ohd mb10\"]/div/span");
            schools.Add(buf + " " + node.InnerText);
            //check();
            return html;
        }
        public void find(string url)//найти информацию из Man на произвольной странице
        {
            int p = 0;//счетчик совпадений
            if (links.Contains(url)) return;
            hp.HtmlDocument doc = new hp.HtmlDocument();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            doc.Load(response.GetResponseStream(), Encoding.GetEncoding(1251));
            hp.HtmlNode node = doc.DocumentNode.SelectSingleNode("//body");
            if (node != null)
            {
                string text = node.InnerText;
                //name
                if (text.Contains(eman(name)) == true || text.Contains(name) == true) p++;
                //date
                if (birthday != null)
                {
                    if (find_date(birthday, text) == true) p += 2;
                }
                //city
                if (city[0] != null && text.Contains(city[0]) == true) p++;
                if (city[1] != null && text.Contains(city[1]) == true) p++;
                //nick
                foreach (string nick in nicks)
                {
                    if (text.Contains(nick) == true) p += 2;
                }
                //links
                int i;
                i = compare_links(doc);
                p += i;
                if (p > 2) links.Add(url);
                //check();
            }
        }
    }
    /*class Program
    {
        static void Main(string[] args)
        {
            Man man = new Man();
            man.vk_parsing("http://vk.com/castiar");
            man.find("http://www.kinopoisk.ru/level/79/user/1309822/");
            man.od_parsing("http://www.odnoklassniki.ru/profile/533756403899");
            man.mm_parsing("http://my.mail.ru/mail/tony_teller");
        }
    }*/
}