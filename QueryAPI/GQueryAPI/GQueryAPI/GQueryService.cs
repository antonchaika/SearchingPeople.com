using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Data;
using System.Net;
using System.IO;
using System.ComponentModel;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
namespace GQueryAPI
{
    public class GQueryService
    {
        private const string Query = "http://www.google.ru/search?hl=ru&safe={0}&q={1}&start={2}";
        /// <summary>
        /// Safety mode. This property is for filtering search reasults. (i.e. "porn", "fuck" etc.)
        /// </summary>
        [DefaultValue(false)]
        public bool Filter { get; set; }

        private int linksCount;
        /// <summary>
        /// Number of ResultObjects (links) in Google result page.
        /// </summary>
        public int LinksCount
        {
            get
            {
                return linksCount;
            }
            set
            {
                linksCount = value > 100 ? 100 : value;
            }
        }
        private string site;
        /// <summary>
        /// Ability for searching on one special web site.
        /// </summary>
        public string Site
        {
            get
            {
                return site;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new Exception("Site cannot be empty or null");
                }
                site = value;
            }
        }
        /// <summary>
        /// Search request for Google Search.
        /// </summary>
        public string SearchQuery { get; private set; }
        private string searchKeyword;
        /// <summary>
        /// Get keyword of current searching.
        /// </summary>
        public string SearchKeyword
        {
            get
            {
                return searchKeyword;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new Exception("Keyword cannot be empty or null");
                }
                searchKeyword = value;
            }
        }
        private WebResponse response;
        private List<string> pageSources;
        //private WebClient webclient;
        public int StartFrom { get; set; }
        /// <summary>
        /// Constructor of GQueryService class.
        /// </summary>
        /// <param name="keyword">Keyword for search request</param>
        public GQueryService(string keyword) : this(keyword, 9, false, null) { }
        public GQueryService(string keyword, string site) : this(keyword, 9, false, site) { }
        /// <summary>
        /// Constructor of GQueryService class.
        /// </summary>
        /// <param name="keyword">Keyword for search request</param>
        /// <param name="filter">Safety mode (true or false)</param>
        /// <param name="linksnum">Number of links in result list</param>
        /// <param name="wc">WebClient object for sending/retrieving data</param>
        public GQueryService(string keyword, int linksnum, bool filter, string site)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                throw new Exception("Keyword cannot be null or empty");
            }
            Filter = filter;
            SearchKeyword = keyword;
            if (!string.IsNullOrEmpty(site))
            {
                SearchKeyword += " site:" + site.ToString();
            }
            LinksCount = linksnum;
            StartFrom = 0;
            pageSources = new List<string>();
            SearchQuery = string.Format(Query, Filter, SearchKeyword, StartFrom);
            //webclient = wc;
        }

        private WebResponse GetResponse(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            try
            {
                request.Method = "GET";
                request.Referer = "www.antonio.na.by"; //temporary referer
                request.AllowAutoRedirect = false;
                //request.Proxy = new WebProxy("79.175.158.156", 8080);
                request.UserAgent = "Opera/9.80 (Windows NT 6.1; U; ru) Presto/2.10.229 Version/11.61";
                return request.GetResponse();
            }
            catch
            {
                return null;
            }
        }
        private bool GetResponse()
        {
            try
            {
                response = GetResponse(SearchQuery);
                if (response == null)
                {
                    return false;
                }
                using (var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    pageSources.Add(reader.ReadToEnd());
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Retrieving pages from Google Search
        /// </summary>
        /// <returns>True if there were no errors and false if there were some errors</returns>
        private bool GetPages()
        {
            for (int i = 0; i < LinksCount / 10 + 1; i++)
            {
                SearchQuery = string.Format(Query, Filter, SearchKeyword, StartFrom);
                if (!GetResponse())
                {
                    return false;
                }
                StartFrom += 10;
            }
            return true;
        }

        /// <summary>
        /// Searching in Google current key phrase and writing results to list.
        /// </summary>
        /// <returns>List of GQueryResult objects</returns>
        public List<GQueryResult> GetResult()
        {
            if (GetPages())
            {
                var s = new List<GQueryResult>();
                foreach (string item in pageSources)
                {
                    /*StreamWriter w1 = new StreamWriter("C:\\Users\\Anton Chaika\\Desktop\\2.html");
                    w1.Write(item);
                    w1.Close();
                    html.Save(w2);
                    w2.Close();*/
                    //StreamWriter w2 = new StreamWriter("C:\\Users\\Anton Chaika\\Desktop\\3.html");
                    HtmlDocument html = new HtmlDocument();
                    html.LoadHtml(item);
                    HtmlNodeCollection htmlcol = html.DocumentNode.SelectNodes("//li[@class='g']");
                    if (htmlcol != null)
                    {
                        foreach (HtmlNode node in htmlcol)
                        {
                            GQueryResult obj = new GQueryResult();
                            HtmlNode a = node.Descendants("a").First();
                            if (a != null)
                            {
                                obj.Url = a.GetAttributeValue("href", null);
                                obj.Title = a.InnerText;
                            }
                            else throw new InvalidOperationException();
                            IEnumerable<HtmlNode> span = node.Descendants("span");
                            if (span.Count() != 0)
                            {
                                //bool error = true;
                                bool full_gl = false;
                                bool full_st = false;
                                for (int i = 0; i < span.Count(); i++)
                                {
                                    if (span.ElementAt(i).GetAttributeValue("class", null) == "gl" && full_gl == false)
                                    {
                                        IEnumerable<HtmlNode> en = span.ElementAt(i).Descendants("a");
                                        if (en.Count() > 0)
                                        {
                                            obj.CachedUrl = en.First().GetAttributeValue("href", null);
                                            full_gl = true;
                                        }
                                        //error = false;
                                    }
                                    //else error = true;
                                    if (span.ElementAt(i).GetAttributeValue("class", null) == "st" && full_st == false)
                                    {
                                        obj.Content = span.ElementAt(i).InnerText;
                                        full_st = true;
                                        //error = false;
                                    }
                                    //else error = true;
                                }
                            }
                            s.Add(obj);
                        }

                    }
                    else return null;
                }
                return s;
            }
            else return null;
        }
    }
    public struct GQueryResult
    {
        private string title;
        /// <summary>
        /// Get the title of the Google search result.
        /// </summary>
        public string Title
        {
            get { return title; }
            internal set { title = value; }
        }
        private string content;
        /// <summary>
        /// Get the brief content of the Google search result.
        /// </summary>
        public string Content
        {
            get { return content; }
            internal set { content = value; }
        }
        private string url;
        /// <summary>
        /// Get the URL of the Google search result.
        /// </summary>
        public string Url
        {
            get { return url; }
            internal set { url = value; }
        }
        private string cachedurl;
        /// <summary>
        /// Get the cached URL of the Google search result.
        /// </summary>
        public string CachedUrl
        {
            get { return cachedurl; }
            internal set { cachedurl = value; }
        }
    }
}
