using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace LinkSpiderLib
{
    public class LinkSpider : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly Regex _rx = new Regex(@"<a\s+(?:[^>]*?\s+)?href=([""'])(?<href>.*?)\1", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private readonly Uri _originalUrl;
        private bool _disposed;

        public Uri OriginalUrl => _originalUrl;
        public List<string> URLExplorationFilter { get; set; }

        private readonly ConcurrentDictionary<string, LinkElement> _fullUrlList = new(StringComparer.OrdinalIgnoreCase);
        private readonly ConcurrentDictionary<string, byte> _externalUrlList = new(StringComparer.OrdinalIgnoreCase);
        private readonly ConcurrentDictionary<string, byte> _brokenUrlList = new(StringComparer.OrdinalIgnoreCase);

        public IEnumerable<string> FullUrlList => _fullUrlList.Keys.OrderBy(url => url);
        public IEnumerable<string> ExternalUrlList => _externalUrlList.Keys.OrderBy(url => url);
        public IEnumerable<string> BrokenUrlList => _brokenUrlList.Keys.OrderBy(url => url);

        public LinkSpider(string url)
        {
            _originalUrl = new Uri(url);
            URLExplorationFilter = new List<string>();
            _httpClient = new HttpClient();
            // Standard User-Agent to prevent bots blockage by hostings
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (compatible; LinkSpider/2.0; +http://juank-io)");
        }

        public async Task WeaveWebAsync()
        {
            await WeaveWebAsync(8); // Default to 8 concurrent requests
        }

        public async Task WeaveWebAsync(int maxDegreeOfParallelism)
        {
            Reset();
            var rootLink = new LinkElement { url = _originalUrl.AbsoluteUri, explored = false };
            _fullUrlList[rootLink.url] = rootLink;

            using var semaphore = new SemaphoreSlim(maxDegreeOfParallelism);

            while (true)
            {
                var nextLnks = _fullUrlList.Values.Where(lnk => !lnk.explored).ToList();
                if (nextLnks.Count == 0)
                {
                    break;
                }

                var batchTasks = new List<Task>();
                foreach (var le in nextLnks)
                {
                    // Mark as explored before entering task to avoid duplicate pick-up in the same loop
                    le.explored = true;

                    await semaphore.WaitAsync();
                    var task = Task.Run(async () =>
                    {
                        try
                        {
                            await ExploreLinkAsync(le);
                        }
                        finally
                        {
                            semaphore.Release();
                        }
                    });
                    batchTasks.Add(task);
                }

                await Task.WhenAll(batchTasks);
            }
        }

        public void WeaveWeb()
        {
            WeaveWebAsync().GetAwaiter().GetResult();
        }

        public async Task WeaveSinglePageAsync()
        {
            Reset();
            var rootLink = new LinkElement { url = _originalUrl.AbsoluteUri };
            _fullUrlList[rootLink.url] = rootLink;

            try
            {
                var htmlFragment = await _httpClient.GetStringAsync(_originalUrl.AbsoluteUri);
                GetCompleteLinksFromHtmlFragment(htmlFragment, _originalUrl.AbsoluteUri);
                rootLink.explored = true;
            }
            catch
            {
                _brokenUrlList[_originalUrl.AbsoluteUri] = 0;
            }
        }

        public void WeaveSinglePage()
        {
            WeaveSinglePageAsync().GetAwaiter().GetResult();
        }

        private async Task ExploreLinkAsync(LinkElement linkElement)
        {
            if (MatchExplorationFilters(linkElement.url))
            {
                return;
            }

            try
            {
                var htmlFragment = await _httpClient.GetStringAsync(linkElement.url);
                GetCompleteLinksFromHtmlFragment(htmlFragment, linkElement.url);
            }
            catch
            {
                _brokenUrlList[linkElement.url] = 0;
            }
        }

        private void GetCompleteLinksFromHtmlFragment(string htmlFragment, string currentUrl)
        {
            var baseUri = new Uri(currentUrl);

            foreach (Match match in _rx.Matches(htmlFragment))
            {
                var link = match.Groups["href"].Value;
                link = ClearURL(link);

                if (string.IsNullOrWhiteSpace(link))
                {
                    continue;
                }

                try
                {
                    // Resolve relative or absolute URL using .NET's native Uri resolver
                    var resolvedUri = new Uri(baseUri, link);

                    // Only explore pages belonging to the same host/domain
                    if (resolvedUri.Host.Equals(_originalUrl.Host, StringComparison.OrdinalIgnoreCase))
                    {
                        // Clean hash fragments and build the clean absolute URL
                        var cleanUrl = resolvedUri.GetLeftPart(UriPartial.Path);
                        
                        var newElement = new LinkElement { url = cleanUrl, explored = false };
                        _fullUrlList.TryAdd(cleanUrl, newElement);
                    }
                    else
                    {
                        _externalUrlList.TryAdd(resolvedUri.AbsoluteUri, 0);
                    }
                }
                catch
                {
                    // If Uri resolution fails, we can log it or ignore it (not a valid link)
                }
            }
        }

        private bool MatchExplorationFilters(string link)
        {
            foreach (var filter in URLExplorationFilter)
            {
                if (link.Contains(filter, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        private static string ClearURL(string link)
        {
            if (link.Contains('#'))
            {
                link = link.Remove(link.IndexOf('#'));
            }
            return link.Trim();
        }

        private void Reset()
        {
            _brokenUrlList.Clear();
            _externalUrlList.Clear();
            _fullUrlList.Clear();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _httpClient.Dispose();
                }
                _disposed = true;
            }
        }
    }
}
