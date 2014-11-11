using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using SynthesisRecipeRetriever.Models;
using SynthesisRecipeRetriever.Classes;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.ComponentModel;

namespace SynthesisRecipeRetriever
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private BackgroundWorker _bgworker = new BackgroundWorker();

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            this.Loaded += new RoutedEventHandler(MainWindow_Loaded);
            this._bgworker.DoWork += new DoWorkEventHandler(_bgworker_DoWork);
        }

        private void _bgworker_DoWork(object sender, DoWorkEventArgs e)
        {
            GetRecipes();
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e) 
        {
            _bgworker.RunWorkerAsync();
        }

        public void GetRecipes()
        {
            // Get all links for all recipes

            // Go to each link and get the html for all subpages

            // For all html pages...
            // Parse their strings and return their recipes. 
            // Save all recipes to one file. 

            Task.Factory.StartNew(() =>
            {
                // Get all links from the main recipes page.
                var main = new Link("http://www.ffxiah.com/recipes");
                Text += "Gathering links from " + main.Url + "...\n";
                var categories = main.GetLinks("//table[@class = 'craft-tbl']");

                // Get the links from the craft subpages(Alchemy, Goldsmithing, ...)
                var subcategories = new List<Link>();
                foreach (var item in categories)
                {
                    Text += "Gathering links from " + item.Url + "...\n";
                    subcategories.AddRange(item.GetLinks("//ul[@id = 'page-menu']"));
                }

                // Get the links from each crafts 1st page of recipes and
                // save the recipes.
                var allLinks = new List<Link>();
                var recipes = new List<Recipe>();
                foreach (var item in subcategories)
                {
                    Text += "Parsing recipes from " + item.FileName + "...\n";
                    List<Recipe> rpl = Parser.GetInstance(item.FileName).GetRecipes();
                    recipes.AddRange(rpl);

                    var links = item.GetLinks("//ul[@class = 'pager']");
                    Text += "Gathering links from " + item.FileName + "...\n";
                    
                    if (links == null) continue;
                    allLinks.AddRange(links);
                }

                // If there are additional pages, also retrieve their recipes. 
                foreach (var item in allLinks)
                {
                    Text += "Parsing recipes from " + item.FileName + "...\n";
                    List<Recipe> rpl = Parser.GetInstance(item.FileName).GetRecipes();
                    recipes.AddRange(rpl);
                }

                // Serialize the recipes list to file. 
                Text += "Serializing recipes to recipes.xml...\n";
                using (Stream fStream = new FileStream("recipes.xml",
                    FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(recipes.GetType());
                    xmlSerializer.Serialize(fStream, recipes);
                }


                // Print out the number of recipes found. 
                Text += "Job Done!\n";
                Text += recipes.Count + "recipes retrieved!\n";
            }).Wait();
        }


        public String _text;
        public String Text
        {
            get 
            {
                return _text;
            }
            set
            {
                _text = value;
                RaisePropertyChanged("Text");
            }
        }

        private void RaisePropertyChanged(String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this,new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
