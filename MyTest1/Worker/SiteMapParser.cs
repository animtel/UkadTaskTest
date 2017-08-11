using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using MyTest1.Models;
using HtmlAgilityPack;

namespace MyTest1.Worker
{
    public class SiteMapParser
    {
        private Request _request;
        private List<CheckUrl> _requestTimeots;
        private string _host;
        private string _result;
        private int _plus1;

        public SiteMapParser()
        {
            _request = new Request();
            _requestTimeots = new List<CheckUrl>();
            _host = string.Empty;
            _result = "";
            _plus1 = 0;
        }

        public List<CheckUrl> Test(string Url)
        {
            /*
             * Cheking url
             */
            if (!Url.StartsWith("http"))
            {
                Url = "http://" + Url;
            }

            if (Url.EndsWith(".xml"))
            {
                _host = Url;
            }
            else
            {
                Uri uriUrl = new Uri(Url);
                _host = new Uri(uriUrl, "/sitemap.xml").ToString();
            }
            GetUrlList();
            return _requestTimeots;
        }

        private void GetUrlList()
        {
            string ContentXml = _request.GetUrl(_host);
            /*
             * I can use XmlDocument but some sitemaps is different.
             * I will use Regex. It is solid.
             */
            MatchCollection UrlCollection = new Regex("<loc>(?<url>(.*?))</loc>").Matches(ContentXml);
            if (UrlCollection.Count != 0) //if sitemap.xml if not empty
            {
                foreach (Match item in UrlCollection)
                {
                    string itemStr = item.Groups["url"].Value;
                    if (itemStr != "")
                    {
                        CheckUrl AddToList = new CheckUrl();

                        AddToList.Url = itemStr;
                        AddToList.MinTime = 0;
                        AddToList.MaxTime = 0;
                        _requestTimeots.Add(AddToList); // Add urls for test
                    }
                }
            }
            else // if sitemap is empty we need build our list
            {
                Uri uriUrl = new Uri(_host);
                _host = new Uri(uriUrl, "/").ToString();
                string[] urls = BuildSitemap();
                foreach (string url in urls)
                {
                    CheckUrl AddToList = new CheckUrl();

                    AddToList.Url = url;
                    AddToList.MinTime = 0;
                    AddToList.MaxTime = 0;
                    _requestTimeots.Add(AddToList); // Add urls for test
                }
            }
        }
        /*
         * Sorting List on a server.
         */
        public List<CheckUrl> SortList(List<CheckUrl> toSort)
        {
            List<CheckUrl> result = toSort.OrderBy(x => x.MinTime).ThenBy(x => x.MaxTime).ToList();
          
            TestUrlModel Db = new TestUrlModel();
            Database.SetInitializer(
                new DropCreateDatabaseIfModelChanges<TestUrlModel>());
            for (int i = 0; i< result.Count; i++)
            {
                try
                {
                    string Host = new Regex(@"[^\/]+").Match(result[i].Url.Replace("http://", "").Replace("https://", "")).Value;
                    result[i].Host = Host;
                   
                    Db.CheckUrls.Add(result[i]);
                    Db.SaveChanges();
                }
                catch { }
            }
            
            return result;
        }
        /*
         * Testing speed.
         */
        public CheckUrl steedTest(string url) //We will make 3 test to get Min and Max value
        {
            // RequestTimeots.Sort((a, b) => a.MinTime.CompareTo(b.MinTime));


            int MinVal = 0;
            int MaxVal = 0;
            for (int i = 0; i < 3; i++)
            {
                int TestValue = _request.GetUrlTime(url);
                if (TestValue > 0)
                {
                    if (MinVal == 0 || MinVal > TestValue)
                    {
                        MinVal = TestValue;
                    }
                    if (TestValue > MaxVal)
                    {
                        MaxVal = TestValue;
                    }
                }
            }

            CheckUrl AddToList = new CheckUrl();

            AddToList.Host = _host;
            AddToList.Url = url;
            AddToList.MinTime = MinVal;
            AddToList.MaxTime = MaxVal;
          
            
            
            return AddToList;
        }
        /*
         * Finding all url that belong to site
         */
        private string[] BuildSitemap()
        {
            List<string> allurls = new List<string>();
            allurls.Add(_host);
            for (int i = 0; i < allurls.Count; i++)
            {
                string urlToCheck = allurls[i];
                Finder(urlToCheck, allurls);
            }
            
            return allurls.ToArray();
        }

        void Finder(string urlToCheck, List<string> allurls)
        {
            string responce = _request.GetUrl(urlToCheck);
            responce = HttpUtility.HtmlDecode(responce);
            HtmlDocument document = new HtmlAgilityPack.HtmlDocument();
            document.LoadHtml(responce);
            var baseUrl = new Uri(_host);
            var noder = document.DocumentNode.SelectNodes("//a[@href]");
            if (noder != null)
            {
                foreach (HtmlNode link in noder)
                {
                    try
                    {

                        HtmlAttribute att = link.Attributes["href"];
                        Uri url = new Uri(baseUrl, att.Value.TrimEnd('/'));
                        string stringUrl = url.ToString();
                        if (url.Host == baseUrl.Host && !stringUrl.Contains("#"))
                        {
                            if (!allurls.Contains(stringUrl))
                            {
                                allurls.Add(stringUrl);
                            }
                        }

                    }
                    catch
                    {

                    }

                }
            }

        }



    }
}