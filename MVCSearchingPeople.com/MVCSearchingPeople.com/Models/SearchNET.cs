using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Web.Hosting;
using AQueryAPI;
using DomainModel.Abstract;
using DomainModel.People;
using GQueryAPI;
using HtmlAgilityPack;
using LMFC;
using SocialAPI;
using VQueryAPI;

namespace MVCSearchingPeople.com.Models
{
    public class SearchNET : ISearchNET
    {
        internal FakeHuman fake = new FakeHuman();
        internal IHumanRepository rep;
        internal int opcode;
        public void Search(IHumanRepository rep, int code)
        {
            manlist.Clear();
            if (code == 1 || code == 2)
            {
                rep.Humans.Clear();
                counter = 0;
            }
            this.opcode = code;
            this.rep = rep;
            //создание нового потока для 
            //параллельных вычислений
            Thread threadG = new Thread(GetRawDataG);
            threadG.Name = "GoogleService";
            threadG.Start();
            GetRawDataSoc();
            //ожидание окончания выполнения второго потока
            threadG.Join();
            //подготовить информацию о человеке
            PrepareHuman();
        }
        private void PrepareHuman()
        {
            foreach (Man item in manlist)
            {
                if (!item.Private)
                {
                    Human h = new Human();
                    Human.Settings = "search" + opcode.ToString();
                    h.Additional = new List<KeyValuePair<string, string>>();
                    h.Tags = new Dictionary<string, double>();
                    h.Links = new Dictionary<string, string>();
                    h.HumanID = counter;
                    h.Name = item.name;
                    if (item.city[0] == null)
                    {
                        if (item.city[1] != null)
                        {
                            if (fake.Location != null)
                            {
                                if (item.city[1].ToLower() == fake.Location.ToLower())
                                    h.Location = item.city[1];
                                else continue;
                            }
                            else
                            {
                                h.Location = item.city[1];
                            }
                        }
                        else h.Location = "Неизвестный";
                    }
                    else if (item.city[0] != null)
                    {
                        if (fake.Location != null)
                        {
                            if (item.city[0].ToLower() == fake.Location.ToLower()) 
                                h.Location = item.city[0];
                            else continue;
                        }
                        else
                        {
                            h.Location = item.city[0];
                        }
                    }
                    if (h.Location.Length > 20)
                    {
                        h.Location.Substring(0, 15);
                        h.Location += "...";
                    }
                    if (item.age != null) h.Age = item.age;
                    else if (fake.Age != null && fake.Age != 0) h.Age = fake.Age.ToString();
                    if (item.photo != null) h.Image = item.photo;
                    //другая информация
                    List<string> list = new List<string>();
                    foreach (string link in item.links)
                    {
                        if (link != null && link.IndexOfAny("абвгдеёжзийклмнопрстуфхцчшщъыьэюя".ToCharArray()) == -1 && !h.Links.ContainsKey(link))
                            h.Links.Add(link, GetWebsiteName(link));
                    }
                    if (item.tags.Count > 0)
                    {
                        foreach (var word in item.tags)
                        {
                            h.Tags.Add(word.Key, word.Value);
                        }
                    }
                    if (item.birthday != null)
                    {
                        h.Additional.Add(new KeyValuePair<string, string>("День рождения: ", item.birthday));
                    }
                    if (item.nicks.Count > 0)
                    {
                        foreach (string nick in item.nicks)
                        {
                            if (nick != null)
                                h.Additional.Add(new KeyValuePair<string, string>("Используемый ник: ", nick));
                        }
                    }
                    if (item.family != null)
                    {
                        h.Additional.Add(new KeyValuePair<string, string>("Семейное положение: ", item.family));
                    }
                    if (item.phone.Length > 0)
                    {
                        foreach (string phone in item.phone)
                        {
                            if (phone != null)
                                h.Additional.Add(new KeyValuePair<string, string>("Способ связи: ", phone));
                        }
                    }
                    if (item.edc.Length > 0)
                    {
                        foreach (Education edu in item.edc)
                        {
                            if (edu != null)
                            {
                                h.Additional.Add(new KeyValuePair<string, string>("ВУЗ: ", edu.univer));
                                h.Additional.Add(new KeyValuePair<string, string>("Факультет: ", edu.faculty));
                                h.Additional.Add(new KeyValuePair<string, string>("Кафедра: ", edu.cafedra));
                                h.Additional.Add(new KeyValuePair<string, string>("Форма обучения: ", edu.form));
                                h.Additional.Add(new KeyValuePair<string, string>("Статус: ", edu.state));
                            }
                        }
                    }
                    if (item.jobs.Count > 0)
                    {
                        foreach (string job in item.jobs)
                        {
                            if (job != null)
                                h.Additional.Add(new KeyValuePair<string, string>("Место работы: ", job));
                        }
                    }
                    //сохранение в репозиторий и 
                    //удаленное заполнение уже с репозитория
                    rep.SaveProduct(h);
                    counter++;
                }
            }
        }
        public void GetFake(FakeHuman fake)
        {
            this.fake.Name = fake.Name;
            this.fake.Age = fake.Age;
            this.fake.Location = fake.Location;
        }
        //инициализаия полей
        private ManualResetEvent mre = new ManualResetEvent(false);
        Analyzer proc = new Analyzer();
        object _lock = new object();
        internal string htmlpagevk;
        internal string htmlpage;
        internal List<Man> manlist = new List<Man>();
        internal AlchemyAPI alchemy = new AlchemyAPI();
        internal short counter = 0;
        public SearchNET()
        {
            alchemy = new AlchemyAPI();
            alchemy.LoadAPIKey(HostingEnvironment.ApplicationPhysicalPath + "bin\\api_key.txt");
            manlist = new List<Man>();
        }
        public void GetRawDataSoc()
        {
            VQueryService vservice;
            if (fake.Age != null)
            {
                vservice = new VQueryService(fake.Name, (short)fake.Age, "belarus");
            }
            else
            {
                vservice = new VQueryService(fake.Name, 0, "belarus");
            }
            List<VQueryResult> r = null;
            if (opcode != 1 && opcode != 2) r = vservice.GetResult();
            if (r != null && r.Count > 0)
            {
                int i = 0;
                Thread.Sleep(2000);
                foreach (var item in r)
                {
                    long begin = 0;
                    Man man = new Man();
                    begin = DateTime.Now.Ticks;
                    //блокировка до поступления нового человека
                    mre.Reset();
                    htmlpagevk = man.vk_parsing(item.Url);
                    lock (_lock)
                    {
                        manlist.Add(man);
                    }
                    SmartAnalyse(true, i);
                    i++;
                    //разрешение на обработку данных другим потоком
                    mre.Set();
                    TimeSpan span = new TimeSpan(DateTime.Now.Ticks - begin);
                    if (span.TotalSeconds < 4)
                    {
                        int duration = 4 - (int)span.TotalSeconds; 
                        Thread.Sleep(duration * 1000);
                    }
                }
            }
            else
            {
                GQueryService gservice;
                List<GQueryResult> g = null;
                if (fake.Location != null)
                {
                    gservice = new GQueryService(fake.Name + " " + fake.Location, "my.mail.ru");
                }
                else if (fake.Age != null)
                {
                    gservice = new GQueryService(fake.Name + " " + fake.Age, "my.mail.ru");
                }
                else gservice = new GQueryService(fake.Name, "my.mail.ru");
                if (opcode != 2) g = gservice.GetResult();
                if (g != null && g.Count > 0)
                {
                    int i = 0;
                    foreach (var item in g)
                    {
                        if (item.Title.ToLower().Contains(fake.Name.ToLower()))
                        {
                            long begin = 0;
                            Man man = new Man();
                            begin = DateTime.Now.Ticks;
                            //блокировка до поступления нового человека
                            mre.Reset();
                            htmlpagevk = man.mm_parsing(item.Url);
                            lock (_lock)
                            {
                                manlist.Add(man);
                            }
                            SmartAnalyse(false, i);
                            i++;
                            //сигнал о разрешении обработки данных
                            //другим потоком
                            mre.Set();
                            TimeSpan span = new TimeSpan(DateTime.Now.Ticks - begin);
                            if (span.TotalSeconds < 4)
                            {
                                int duration = 4 - (int)span.TotalSeconds;
                                Thread.Sleep(duration * 1000);
                            }
                        }
                    }
                }
                else
                {
                    if (fake.Location != null)
                    {
                        g = new GQueryService(fake.Name + " " + fake.Location, "odnoklassniki.ru").GetResult();
                    }
                    else if (fake.Age != null)
                    {
                        g = new GQueryService(fake.Name + " " + fake.Age, "odnoklassniki.ru").GetResult();
                    }
                    else g = new GQueryService(fake.Name, "odnoklassniki.ru").GetResult();
                    if (g != null && g.Count > 0)
                    {
                        int i = 0;
                        foreach (var item in g)
                        {
                            if (item.Title.ToLower().Contains(fake.Name.ToLower()))
                            {
                                long begin = 0;
                                Man man = new Man();
                                begin = DateTime.Now.Ticks;
                                //блокировка до поступления нового человека
                                mre.Reset();
                                htmlpagevk = man.od_parsing(item.Url);
                                lock (_lock)
                                {
                                    manlist.Add(man);
                                }
                                SmartAnalyse(false, i);
                                i++;
                                //сигнал о разрешении обработки данных
                                //другим потоком
                                mre.Set();
                                TimeSpan span = new TimeSpan(DateTime.Now.Ticks - begin);
                                if (span.TotalSeconds < 4)
                                {
                                    int duration = 4 - (int)span.TotalSeconds;
                                    Thread.Sleep(duration * 1000);
                                }
                            }
                        }
                    }
                }
            }
            mre.Set();
            //проанализировать на ключевые элементы
            //SmartAnalyse(true, i);
        }
        public void GetRawDataG()
        {
            mre.WaitOne();
            for (int i = 0; i < manlist.Count; i++)
            {
                string query;
                if (manlist[i] != null)
                {
                    for (int j = 2; j >= 2; j--)
                    {
                        long begin = DateTime.Now.Ticks;
                        query = SmartQueryToGoogle(i, j);
                        if (query != null && query != "")
                        {
                            GQueryService service = new GQueryService(query);
                            List<GQueryResult> result = service.GetResult();
                            if (result != null && result.Count > 0)
                            {
                                foreach (var item in result)
                                {
                                    if (j == 0 || j == 1)
                                    {
                                        if (item.Title.Contains(manlist[i].nicks[j].ToString()) || ( item.Content != null && item.Content.Contains(manlist[i].nicks[j].ToString())))
                                        {
                                            if (item.Url.Contains("http"))
                                            manlist[i].find(item.Url);
                                        }
                                    }
                                    else if (j == 2)
                                    {
                                        if (item.Title.Contains(manlist[i].name) || item.Title.Contains(manlist[i].eman(manlist[i].name))
                                            || item.Content.Contains(manlist[i].name) || item.Content.Contains(manlist[i].eman(manlist[i].name)))
                                        {
                                            if (item.Url.Contains("http"))
                                            manlist[i].find(item.Url);
                                        }
                                    }
                                }
                            }
                        }
                        if (manlist[i].links.Count >= 5) break;
                        TimeSpan span = new TimeSpan(DateTime.Now.Ticks - begin);
                        if (span.TotalSeconds < 4)
                        {
                            int duration = 4 - (int)span.TotalSeconds;
                            Thread.Sleep(duration * 1000);
                        }
                    }
                }
            }
        }
        public string GetWebsiteName(string url)
        {
            char[] link = new char[50];
            int i = 7;
            int j = 0;
            while (i < url.Length)
            {
                if (url[i] == 'w' && url[i + 1] == 'w' && url[i + 2] == 'w')
                {
                    while (url[i++] != '.') ;
                }
                if (url[i] == '/') break;
                link[j++] = url[i++];
            }
            String s = new string(link);
            return s;
        }
        public void SmartAnalyse(bool vk, int id)
        {
            HtmlDocument doc = new HtmlDocument();  
            HtmlNode node = null;
            if (vk)
            {
                doc.LoadHtml(htmlpagevk);
                node = doc.DocumentNode.SelectSingleNode("//div[@id=\"profile_wide\"]");
            }
            else
            {
                doc.LoadHtml(htmlpagevk);
                node = doc.DocumentNode.SelectSingleNode("//body");   
            }
            if (node != null)
            {
                string s = node.InnerText;
                proc.Text = s;
                SortedList list = proc.GetKey(50, 20, 3);
                foreach (EL_Value item in list.Values)
                {
                    if (!manlist[id].tags.ContainsKey(item.EL.Name))
                    {
                        manlist[id].tags.Add(item.EL.Name, item.Value);
                    }
                    else
                    {
                        manlist[id].tags[item.EL.Name] = (manlist[id].tags[item.EL.Name] + item.Value) / 2;
                    }
                }
            }
        }
        public string SmartQueryToGoogle(int i, int opcode)
        {
            if (opcode == 0)
            {
                if (manlist[i].nicks.Count > 0 )
                {
                    if (manlist[i].nicks[0].ToString().IndexOf("id") != 0)
                        return manlist[i].nicks[0].ToString();
                    else return null;
                }
                else return null;
            }
            else if (opcode == 1)
            {
                if (manlist[i].nicks.Count > 1)
                {
                    if (manlist[i].nicks[1].ToString().IndexOf("id") != 0)
                        return manlist[i].nicks[1].ToString();
                    else return null;
                }
                else return null;
            }
            else if (opcode == 2)
            {
                if (manlist[i].city.Length == 1 && manlist[i].city[0] != null)
                    return manlist[i].name + " " + manlist[i].city[0];
                else if (manlist[i].city.Length == 2 && manlist[i].city[1] != null)
                    return manlist[i].name + " " + manlist[i].city[1];
                else if (manlist[i].age != null)
                    return manlist[i].name + " " + manlist[i].age;
                else return null;
            }
            else return null;
        }
        private bool GetResponse(string url, Encoding encoding)
        {
            //метод отправки запроса серверу
            //и получение от него ответа
            long begin = DateTime.Now.Ticks;
            if (url.IndexOf("http") > 0)
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.AllowAutoRedirect = true;
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
                    TimeSpan span = new TimeSpan(DateTime.Now.Ticks - begin);
                    return true;
                }

                catch
                {
                    return false;
                }
            }
            else return false;
        }

        public string[] geturls(string htmls)
        {
            string[] str = new string[100];
            int i = 0;
            HtmlDocument html = new HtmlDocument();
            html.LoadHtml(htmls);
            HtmlNodeCollection col = html.DocumentNode.SelectNodes("//a");
            foreach (HtmlNode node in col)
            {
                if (node.Attributes["href"] != null)
                {
                    str[i] = node.Attributes["href"].Value;
                    if (i != 0)
                    {
                        if (CompareTo(str[i], str[i - 1]))
                        {
                            str[i] = str[i].Substring(0, str[i].Length - 1);
                        }
                        else i--;
                    }
                    i++;
                }
                if (i == 100) break;
            }
            return str;
        }
        private bool CompareTo(string a, string b)
        {
            int i = 0;
            while (a.Length > i && b.Length > i)
            {
                if (a[i] == b[i]) i++;
                else break;
            }
            if (i <= a.Length / 2) return true;
            else return false;
        }
    }
}