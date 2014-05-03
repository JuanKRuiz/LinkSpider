Link Spider
===========

This High performance Portable Library and Console Client search for links in a website and can creates standard sitemap.xml file.

##LinkSpider Console Features
Full support to all LinkSpider Portable Class Library features

###Samples
####Fast create
This creates
*sitemap.xml : standard sitemap
*plain.txt   : plain text file listing all links in website

```
LinkSpiderConsole.exe --u http://yoursite.com
```
####Customizing output files --s --p
```
LinkSpiderConsole.exe --u http://yoursite.com --s YOURsitemap.xml --p YOURplain.txt
```

####Navigation Filtering --n
This avoid to explore links containing some url patterns.
This sample shows how to avoid url  exploration when it contains this fragments
*/tag/
*/pages/

```
LinkSpiderConsole.exe --u http://yoursite.com --n /tag/,/pages/
```

This is very useful to improve performance not waisting time in unuseful urls.

####Sitemap Filtering --m
This avoid to include links containing some url patterns in sitemap file.
This sample shows how to avoid some links in sitemap when it contains this fragments
*/tag/
*/pages/


```
LinkSpiderConsole.exe --u http://yoursite.com --m /tag/,/pages/
```

###Additional files
This tool creates optionally 2 differente files :
* brokenLinks.txt: including broken links targeting current site
* externalLinks.txt: including all links targeting to other domains

##LinkSpider Portable Class Library Features

###LinkSpider Class
* High performance
* Explore websites using parallel features
* Ready for async / await  operations
* Broken links list in the website 
* List with all website links
* List with all external links
* Support exploration filters to avoid browse for links in pages including some url patterns

###SitemapTarantula Class
* Builds standard sitemap.xml file
* Support output filtering to exclude links with some url patterns
* Support data generation in Unicode and UTF8 Encodings
