# QuickLook-Spider

The search engine I implemented for my License project. It's composed of a web crawler that indexes the data retrieved into a Elasticsearch database. The Crawler is parallel and can gather data from multiple links at once, storing all the further references to yet uncrawled sites onto the database to be crawled later
The stored data about the websites is then queries and showed onto a ASP.NET MVC interface website
