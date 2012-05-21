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

namespace VQueryAPI
{
    /// <summary>
    /// Global public class for using VK offline search
    /// </summary>
    public class VQueryService
    {
        private string Query = "http://vk.com/search?c[name]=1&c[q]={0}&c[section]=people";
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
        private HtmlDocument html;
        /// <summary>
        /// Constructor of GQueryService class.
        /// </summary>
        /// <param name="name">Name for search request</param>
        public VQueryService(string name) : this(name, 0, null) { }
        /// <summary>
        /// Constructor of GQueryService class.
        /// </summary>
        /// <param name="name">Name for search request</param>
        /// <param name="age">Age of the human</param>
        public VQueryService(string name, short age) : this(name, age, null) { }
        /// <summary>
        /// Constructor of GQueryService class.
        /// </summary>
        /// <param name="name">Name for search request</param>
        /// <param name="age">Age of the human</param>
        /// <param name="loc">Location of the human</param>
        public VQueryService(string name, short age, string loc)
        {
            html = new HtmlDocument();
            if (string.IsNullOrEmpty(name))
            {
                throw new Exception("Keyword cannot be null or empty");
            }
            SearchKeyword = name;
            if (age != 0 && age > 12)
            {
                Query += "&c[age_from]=" + (age - 3).ToString() + "&c[age_to]=" + (age + 3).ToString();
            }
            if (!string.IsNullOrEmpty(loc) && loc == "belarus")
            {
                Query += "&c[country]=3";
            }
            SearchQuery = string.Format(Query, SearchKeyword);
        }

        private WebResponse GetResponse(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            try
            {
                request.Method = "GET";
                request.Referer = "www.google.com"; //temporary referer
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
                html.Load(response.GetResponseStream(), Encoding.GetEncoding(1251));
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Searching in Google current key phrase and writing results to list.
        /// </summary>
        /// <returns>List of GQueryResult objects</returns>
        public List<VQueryResult> GetResult()
        {
            if (GetResponse())
            {
                var s = new List<VQueryResult>();
                HtmlNodeCollection htmlcol = html.DocumentNode.SelectNodes("//td[@id='results']/div");
                if (htmlcol != null)
                {
                    foreach (HtmlNode node in htmlcol)
                    {
                        VQueryResult obj = new VQueryResult();
                        HtmlNode a = node.Descendants("a").ElementAt(1);
                        if (a != null)
                        {
                            obj.Url = "http://vk.com" + a.GetAttributeValue("href", null);
                            obj.Title = a.InnerText;
                        }
                        s.Add(obj);
                    }
                }
                else return null;
                return s;
            }
            else return null;
        }
    }

    /// <summary>
    /// Result class for VK
    /// </summary>
    public struct VQueryResult
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
        private string url;
        /// <summary>
        /// Get the URL of the Google search result.
        /// </summary>
        public string Url
        {
            get { return url; }
            internal set { url = value; }
        }
    }
}
