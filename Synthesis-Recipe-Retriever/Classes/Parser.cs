using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using SynthesisRecipeRetriever.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Controls;

namespace SynthesisRecipeRetriever.Classes
{
    public class Parser
    {
        /// <summary>
        /// Singleton instance for this class.
        /// </summary>
        private static Parser _data;

        /// <summary>
        /// The html source for statistics
        /// </summary>
        private HtmlDocument _document = new HtmlDocument();

        /// <summary>
        /// Provides singleton access to this class. 
        /// </summary>
        private Retriever _retriever = Retriever.GetInstance();

        /// <summary>
        /// The name our current document is bound to. 
        /// </summary>
        private static string _filename = String.Empty;

        private Parser(String fileName)
        {
            _filename = fileName;

            // Read in the html file.
            _document.Load(fileName);
        }

        private HtmlNodeCollection GetHtmlNodes(String XPath)
        {
            return _document.DocumentNode.SelectNodes(XPath);
        }

        /// <summary>
        /// Provides access to our playerdata instance
        /// </summary>
        /// <param name="url"></param>
        /// <param name="xpath"></param>
        /// <returns></returns>
        public static Parser GetInstance(String fileName)
        {
            if (_filename != fileName) return _data = new Parser(fileName);

            return _data ?? (_data = new Parser(fileName));
        }

        public List<Recipe> GetRecipes()
        {
            var table = this.GetHtmlNodes("//table[@class='stdtbl']/tr");

            if (table == null) return new List<Recipe>();

            var idNodes = this.GetHtmlNodes(XPaths.ID_PATH).Descendants().Where(x => x.Name == "a").ToArray();

            var materialNodes = this.GetHtmlNodes(XPaths.MATERIALS_PATH);

            var recipeSkillNodes = this.GetHtmlNodes(XPaths.SKILL_PATH);

            var resultNodes = this.GetHtmlNodes(XPaths.NAME_PATH);

            var skillNodes = recipeSkillNodes.Select(x => x.SelectNodes(XPaths.SKILL_SUBPATH)).ToArray();

            List<Recipe> RecipeList = new List<Recipe>();

            for (int i = 0; i < idNodes.Length; i++)
            {
                var id = GetID(idNodes[i]);
                var name = resultNodes[i].Descendants().First().InnerText;
                var quantity = GetQuantity(resultNodes[i].InnerText);
                var materials = GetCraftMaterials(materialNodes[i]);
                var skills = GetCraftSkill(skillNodes[i]);
                var recipe = new Recipe(id, name, quantity, materials, skills);
                RecipeList.Add(recipe);
            }

            return RecipeList;
        }

        private int GetID(HtmlNode htmlNode)
        {
            return int.Parse(htmlNode.InnerText.Replace("id:", ""));
        }

        private Dictionary<string, int> GetCraftSkill(HtmlNodeCollection htmlNode)
        {
            var recipeSkill = new Dictionary<string, int>();

            // Go into each div and store its requirements
            // in our skills list. 
            foreach (var node in htmlNode)
            {
                // Remove the braces and split the text by a space
                var parts = node.InnerText.Replace("(", "").Replace(")", "").Split(' ');

                // Craft requirement will always be in the first part. 
                string craft = parts[0];

                // Level reuquirement will always be in the second part. 
                // This should not fail since the string is in the form of: 
                // "Alchemy (4)"
                int level = int.Parse(parts[1]);

                // Add this requirement to the list of requirements. 
                recipeSkill.Add(craft, level);
            }

            return recipeSkill;
        }

        private int GetQuantity(string recipeName)
        {
            int quantity = 1;

            var xs = recipeName.Split(' ').Where(x => x.Contains('x')).Select(x => x.Replace("x", ""));

            foreach (var x in xs)
            {
                if (int.TryParse(x, out quantity)) break;
            }

            return quantity;
        }

        private Dictionary<string, int> GetCraftMaterials(HtmlNode materials)
        {
            Dictionary<string, int> values = new Dictionary<string, int>();

            // Contains information about the ingredients used in a synth
            // Name : name of the ingredient
            // Quantity : How much is needed in a single synth
            foreach (var material in materials.Descendants().Where(x => x.Name == "a"))
            {
                var quantity = GetQuantity(material.ParentNode.InnerText);

                string name = material.ParentNode.InnerText.Replace("x" + quantity, "").Trim();

                // Key already exists so we can't add it. 
                if (values.ContainsKey(name)) continue;

                values.Add(name, quantity);
            }

            return values;
        }
    }
}