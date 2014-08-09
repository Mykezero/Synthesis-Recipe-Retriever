using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SynthesisRecipeRetriever.Classes
{
    public class Link
    {
        public string Url;
        public string Filename;
        public HtmlDocument Document;
        private Retriever Retriever;

        public Link(string url)
        {
            this.Url = url;
            this.Filename = Clean(url);
            this.Document = new HtmlDocument();
            this.Retriever = Retriever.GetInstance();
            this.Retriever.CreateFile(Filename, url);
            Document.Load(Filename);
        }

        public string Clean(string url)
        {
            return url.Replace("http://", "").Replace("/", "-");
        }

        public List<Link> GetLinks(string xpath)
        {
            var nodes = Document.DocumentNode.SelectNodes(xpath);

            if (nodes == null) return null;

            var links = nodes.Descendants()
            .Where(x => x.Name == "a")
            .Select(x => x.Attributes["href"])
            .Select(x => Url.Replace("/recipes", x.Value))
            .Select(x => new Link(x))
            .ToList();

            return links;
        }
    }
}
