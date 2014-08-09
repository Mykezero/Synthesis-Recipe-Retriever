using SynthesisRecipeRetriever.Classes;
using HtmlAgilityPack;
using System;
using System.Linq;
using System.Windows;
using System.Runtime.InteropServices;
using SynthesisRecipeRetriever.Models;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace SynthesisRecipeRetriever
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static String Text = String.Empty;

        public App()
        {
            // Get all links for all recipes

            // Go to each link and get the html for all subpages

            // For all html pages...
            // Parse their strings and return their recipes. 
            // Save all recipes to one file. 

            var main = new Link("http://www.ffxiah.com/recipes");
            var categories = main.GetLinks("//table[@class = 'craft-tbl']");
            var subcategories = new List<Link>();
            var allLinks = new List<Link>();
            var recipes = new List<Recipe>();

            foreach (var item in categories)
            {
                subcategories.AddRange(item.GetLinks("//ul[@id = 'page-menu']"));
            }

            foreach (var item in subcategories)
            {
                List<Recipe> rpl = Parser.GetInstance(item.Filename).GetRecipes();
                recipes.AddRange(rpl);
            }

            using (Stream fStream = new FileStream("Recipes.xml",
                FileMode.Create, FileAccess.Write, FileShare.None))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(recipes.GetType());
                xmlSerializer.Serialize(fStream, recipes);
            }
        }
    }
}
