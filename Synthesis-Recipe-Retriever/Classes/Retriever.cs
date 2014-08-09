using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace SynthesisRecipeRetriever.Classes
{
    public class Retriever
    {
        // Our singleton instance.
        private static Retriever _dataRetriever;

        /// <summary>
        /// Goes out to the web if the file doesn't exist and grabs it's html code.
        /// Will return true of success and will throw an error on failure.
        /// Stores filename, xpath variables, Urls for access outside of class.
        /// </summary>
        /// <param name="playerType"></param>
        /// <returns></returns>
        public bool CreateFile(String fileName, String url)
        {
            try
            {
                // Create the html locally if the file does not exist.
                // It will go to the web to get the data and will write it to file.
                if (!File.Exists(fileName))
                {
                    using (StreamWriter sw = new StreamWriter(fileName))
                    {
                        sw.WriteLine(GetHtmlSourceFast(url));
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return true;
        }

        /// <summary>
        /// Returns a singleton instance of data retriever
        /// </summary>
        /// <returns></returns>
        public static Retriever GetInstance()
        {
            return _dataRetriever ?? (_dataRetriever = new Retriever());
        }

        private string GetHtmlSourceFast(string url)
        {
            string source = String.Empty;
            
            using (WebClient client = new WebClient())
            {
                source = client.DownloadString(url);
            }

            return source;
        }

        /// <summary>
        /// Goes out to the webserver and fetches the html file after running
        /// the necessary javascript commands.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private String GetHtmlFromBrowser(String url)
        {
            var driver = new FirefoxDriver();
            driver.Navigate().GoToUrl(url);

            // The javascript code that allows us to get the right statistics.
            List<Script> scripts = new List<Script>();
            // scripts.Add(new Script("MainContent_ddlSeason", "0"));

            // Run all the scripts.
            foreach (var script in scripts)
            {
                script.RunScript(driver);
                Thread.Sleep(2000);
            }

            String source = driver.PageSource;

            // Close firefox and return the html string
            driver.Quit();
            driver.Dispose();
            return source;
        }
    }
}
