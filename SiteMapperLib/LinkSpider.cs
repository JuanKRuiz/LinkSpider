using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace LinkSpiderLib;

public class LinkSpider : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly Regex _rx = new(@"<a\s+(?:[^>]*?\s+)?href=([""'])(?<href>.*?)\1", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private readonly Uri _originalUrl;
    private bool _disposed;

    public Uri OriginalUrl => _originalUrl;
    public List<string> URLExplorationFilter { get; set; } = [];

    private readonly ConcurrentDictionary<string, LinkElement> _fullUrlList = new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentDictionary<string, byte> _externalUrlList = new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentDictionary<string, byte> _brokenUrlList = new(StringComparer.OrdinalIgnoreCase);

    public IEnumerable<string> FullUrlList => _fullUrlList.Keys.OrderBy(url => url);
    public IEnumerable<string> ExternalUrlList => _externalUrlList.Keys.OrderBy(url => url);
    public IEnumerable<string> BrokenUrlList => _brokenUrlList.Keys.OrderBy(url => url);

    public LinkSpider(string url)
    {
        _originalUrl = new Uri(url);
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (compatible; LinkSpider/2.0; +http://juank-io)");
    }

    public async Task WeaveWebAsync() => await WeaveWebAsync(8);

    public async Task WeaveWebAsync(int maxDegreeOfParallelism)
    {
        Reset();
        var rootLink = new LinkElement { Url = _originalUrl.AbsoluteUri, Explored = false };
        _fullUrlList[rootLink.Url] = rootLink;

        using var semaphore = new SemaphoreSlim(maxDegreeOfParallelism);

        while (true)
        {
            var nextLnks = _fullUrlList.Values.Where(lnk => !lnk.Explored).ToList();
            if (nextLnks.Count == 0)
            {
                break;
            }

            var batchTasks = new List<Task>();
            foreach (var le in nextLnks)
            {
                le.Explored = true;

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

    public void WeaveWeb() => WeaveWebAsync().GetAwaiter().GetResult();

    public async Task WeaveSinglePageAsync()
    {
        Reset();
        var rootLink = new LinkElement { Url = _originalUrl.AbsoluteUri, Explored = false };
        _fullUrlList[rootLink.Url] = rootLink;

        try
        {
            var htmlFragment = await _httpClient.GetStringAsync(_originalUrl.AbsoluteUri);
            GetCompleteLinksFromHtmlFragment(htmlFragment, _originalUrl.AbsoluteUri);
            rootLink.Explored = true;
        }
        catch
        {
            _brokenUrlList[_originalUrl.AbsoluteUri] = 0;
        }
    }

    public void WeaveSinglePage() => WeaveSinglePageAsync().GetAwaiter().GetResult();

    private async Task ExploreLinkAsync(LinkElement linkElement)
    {
        if (MatchExplorationFilters(linkElement.Url))
        {
            return;
        }

        try
        {
            var htmlFragment = await _httpClient.GetStringAsync(linkElement.Url);
            GetCompleteLinksFromHtmlFragment(htmlFragment, linkElement.Url);
        }
        catch
        {
            _brokenUrlList[linkElement.Url] = 0;
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
                var resolvedUri = new Uri(baseUri, link);

                if (resolvedUri.Host.Equals(_originalUrl.Host, StringComparison.OrdinalIgnoreCase))
                {
                    var cleanUrl = resolvedUri.GetLeftPart(UriPartial.Path);
                    var newElement = new LinkElement { Url = cleanUrl, Explored = false };
                    _fullUrlList.TryAdd(cleanUrl, newElement);
                }
                else
                {
                    _externalUrlList.TryAdd(resolvedUri.AbsoluteUri, 0);
                }
            }
            catch
            {
                // Ignore invalid links
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
