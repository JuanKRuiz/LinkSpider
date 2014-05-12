Link Spider
===========
![LinkSpider logo](https://raw.githubusercontent.com/JuanKRuiz/LinkSpider/master/LingSpider-logo.png)

**Link Spider** is a High performance **Portable** Class Library searching for links in a website or webpage allowing you [optionally] creating standard **sitemap.xml** file.

This proyect also includes a **Console Client** as an utility to generate sitemap.xml of any site.

* Library works with parallel tasks to reach maximum perfomance
* async - await operations support

##LinkSpider Portable Class Library Features

###LinkSpider Class
* High performance
* Explore single webpages using parallel features
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


##LinkSpider Console Features
Full support to all LinkSpider Portable Class Library features

###Samples
####Fast create
This creates
* sitemap.xml : standard sitemap
* plain.txt   : plain text file listing all links in website

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
* /tag/
* /pages/

```
LinkSpiderConsole.exe --u http://yoursite.com --n /tag/,/pages/
```

This is very useful to improve performance not waisting time in unuseful urls.

####Sitemap Filtering --m
This avoid to include links containing some url patterns in sitemap file.
This sample shows how to avoid some links in sitemap when it contains this fragments
* /tag/
* /pages/


```
LinkSpiderConsole.exe --u http://yoursite.com --m /tag/,/pages/
```

####Single webpage mode
Every execution mode listed before also support `--o` special parameter to analyze just the page passed as parameter.

```
LinkSpiderConsole.exe --u http://yoursite.com/myWebPage --m /tag/,/pages/ --o
```

###Additional files
This tool creates optionally 2 differente files :
* brokenLinks.txt: including broken links targeting current site
* externalLinks.txt: including all links targeting to other domains

