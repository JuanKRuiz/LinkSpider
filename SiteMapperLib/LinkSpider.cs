using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SiteMapperLib
{
    public class LinkSpider
    {
        object _lockbject = new object();
        HttpClient _httpClient = new HttpClient();
        readonly Regex _rx = new Regex(@"<a.*?href=(""|')(?<href>.*?)(""|').*?>(?<value>.*?)</a>");

        readonly Uri _originalUrl;
        public Uri OriginalUrl
        { get { return _originalUrl; } }

        #region Lists
        HashSet<LinkElement> _fullUrlList = new HashSet<LinkElement>(new LinkElementComparer());
        public HashSet<LinkElement> FullUrlList
        {
            get { return _fullUrlList; }
        }

        HashSet<LinkElement> _externalUrlList = new HashSet<LinkElement>(new LinkElementComparer());
        public HashSet<LinkElement> ExternalUrlList
        { get { return _externalUrlList; } }


        HashSet<LinkElement> _brokenUrlList = new HashSet<LinkElement>(new LinkElementComparer());
        public HashSet<LinkElement> BrokenUrlList
        { get { return _brokenUrlList; } }

        #endregion Lists

        public LinkSpider(string url)
        {
            _originalUrl = new Uri(url);
        }

        public async Task WeaveWebAsync()
        {
            await Task.Run(() => { WeaveWeb(); });
        }

        public void WeaveWeb()
        {
            Reset();
            var rootLink = new LinkElement() { url = _originalUrl.AbsoluteUri };
            _fullUrlList.Add(rootLink);

            List<LinkElement> nextLnks = (from lnk in _fullUrlList
                        where lnk.explored == false
                        select lnk).ToList();

            while (nextLnks.Count() > 0)
            {
                Parallel.ForEach<LinkElement>(nextLnks, (le) =>
                {
                    ExploreLink(le);
                });

                nextLnks = (from lnk in _fullUrlList
                            where lnk.explored == false
                            select lnk).ToList();
            }
        }

        HttpClient httpClient = new HttpClient();
        private void ExploreLink(LinkElement linkElement)
        {
            
            string htmlFragment = string.Empty;

            try
            {
                htmlFragment = _httpClient.GetStringAsync(linkElement.url).Result;
            }
            catch
            {
                _brokenUrlList.Add(linkElement);
                return;
            }

            GetCompleteLinksFromHtmlFragment(htmlFragment);
            MarkLinkAsExplored(linkElement);
        }
        
        private void MarkLinkAsExplored(LinkElement linkElement)
        {
            lock (_lockbject)
            {
                var linkElementAdded = _fullUrlList.Add(linkElement);

                if (!linkElementAdded)
                    _fullUrlList.Where(link => link.url == linkElement.url).First().explored = true;
                else
                    linkElement.explored = true;
            }
        }

        private void GetCompleteLinksFromHtmlFragment(string htmlFragment)
        {
            UriBuilder uribldr = new UriBuilder(_originalUrl);

            foreach (Match match in _rx.Matches(htmlFragment))
            {
                var link = match.Groups["href"].Value;

                link = ClearURL(link);

                if (!link.Contains("://"))
                {
                    uribldr.Host = _originalUrl.Host;
                    uribldr.Port = _originalUrl.Port;
                    uribldr.Path = link;
                    _fullUrlList.Add(new LinkElement() { url = uribldr.Uri.AbsoluteUri });
                }
                else if (link.StartsWith(_originalUrl.AbsoluteUri))
                {
                    var uri = new Uri(link);
                    uribldr.Path = uri.AbsolutePath;
                    _fullUrlList.Add(new LinkElement() { url = uribldr.Uri.AbsoluteUri });
                }
                else
                {
                    var uri = new Uri(link);
                    if (!uri.AbsoluteUri.StartsWith(_originalUrl.AbsoluteUri))
                        _externalUrlList.Add(new LinkElement() { url = uri.AbsoluteUri });
                }
            }
        }

        private static string ClearURL(string link)
        {
            if (link.Contains("#"))
            {
                link = link.Remove(link.IndexOf("#"));
            }
            return link;
        }

        private void Reset()
        {
            this._brokenUrlList.Clear();
            this._externalUrlList.Clear();
            this._fullUrlList.Clear();
        }
    }
}
