using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeroLimits.Regainet.Retrieval;
using System.IO;
using System.Threading;

namespace SynthesisRecipeRetriever.Classes
{
    public class Link
    {
        private HtmlDocument _document = new HtmlDocument();
        
        private Retriever _retriever = new Retriever();
        
        private FileInfo _fileInfo;

        private const int CRAWL_WAIT_TIME = 2000;

        public Link(string url)
        {
            // Set the url
            this.Url = url;

            // Set the filename based on the url.
            this.FileName = Clean(Url);

            // Set the file info. 
            this._fileInfo = new FileInfo(FileName);
            
            // Save the html code to file and get the source
            // as a string. 
            this.Source = GetHtmlSource(Url, FileName);
            
            // Load the html for querying.
            this._document.LoadHtml(Source);
        }

        /// <summary>
        /// The webpage's url.
        /// </summary>
        public String Url { get; set; }


        /// <summary>
        /// The filename the html is saved under. 
        /// </summary>
        public String FileName { get; set; }

        /// <summary>
        /// The html source itself. 
        /// </summary>
        public String Source { get; set; }

        /// <summary>
        /// Gets the html source, saves it to file and then returns it.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public String GetHtmlSource(string url, string filename)
        {
            String source = String.Empty;

            // If the file doesn't exist try to create it. 
            if (!File.Exists(filename))
            {
                // Attempts to read the webpage's html and saves
                // it to disk. 
                _retriever.SaveHtmlToFile(url, filename);
                Thread.Sleep(CRAWL_WAIT_TIME);
            }

            // If the file exists now read it to get the source. 
            if (File.Exists(filename))
            {
                // Read the lines from source and join them with new lines. 
                source = String.Join(Environment.NewLine, File.ReadAllLines(filename));
            }

            // Return either "" or the html source code. 
            return source;
        }

        public string Clean(string url)
        {
            return url.Replace("http://", "").Replace("/", "-").Replace(".", "-");
        }

        public List<Link> GetLinks(string xpath)
        {
            var nodes = _document.DocumentNode.SelectNodes(xpath);

            if (nodes == null) return new List<Link>();

            var urls = nodes.Descendants()
            .Where(x => x.Name == "a")
            .Select(x => x.Attributes["href"])
            .Select(x => Url.Replace("/recipes", x.Value))
            .ToList();

            var links = new List<Link>();
            foreach (var url in urls)
            {
                links.Add(new Link(url));
            }

            return links;
        }
    }
}
