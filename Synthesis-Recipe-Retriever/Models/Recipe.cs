using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SynthesisRecipeRetriever.Models
{
    /// <summary>
    /// A class that represents an crafting recipe.
    /// Comments use Antidote x12 crafting recipe as an example. 
    /// </summary>
    public class Recipe
    {
        /// <summary>
        /// Id of the recipe
        /// </summary>
        [XmlAttribute("ID")]
        public int _id = 0;

        /// <summary>
        /// What craft does the recipe belong to: 
        /// Alchemy (5)
        /// </summary>
        [XmlIgnore]
        public Dictionary<string, int> _skills = new Dictionary<string, int>();

        public List<CraftSkill> Skills
        {
            get
            {
                return ConvertToCraftSkillList(this._skills);
            }
        }

        /// <summary>
        /// What ingredients and how many of each:
        /// Wijnruit, x3
        /// </summary>
        [XmlIgnore]
        public Dictionary<String, int> _materials = new Dictionary<string, int>();

        public List<Material> Materials
        {
            get
            {
                return ConvertToMaterialList(this._materials);
            }
        }

        /// <summary>
        /// The result for the crafting recipe: 
        /// Antidote
        /// </summary>
        [XmlAttribute("Name")]
        public string _name = String.Empty;

        /// <summary>
        /// How much of a product is produced. 
        /// </summary>
        [XmlAttribute("Quantity")]
        public int _quantity = 1;

        public Recipe() { }

        public Recipe(int id, String name, int quantity, Dictionary<string, int> materials, Dictionary<string, int> skill)
        {
            // Set the recipe's id. 
            this._id = id;

            // Set the recipe's name
            this._name = name;

            // Set the recipes quantity
            this._quantity = quantity;

            // Set the recipe's materials needed. 
            this._materials = materials;

            // Set the recipe required craft and craft levels. 
            this._skills = skill;
        }

        public class CraftSkill
        {
            public CraftSkill() { }
            
            [XmlAttribute("Craft")]
            public string Craft;
            
            [XmlAttribute("Level")]
            public int Level;
        }

        public class Material
        {
            public Material() { }

            [XmlAttribute("Ingredient")]
            public string Ingredient;

            [XmlAttribute("Quantity")]
            public int Quantity;
        }

        public List<CraftSkill> ConvertToCraftSkillList(Dictionary<string, int> dictionary)
        {
            List<CraftSkill> skills = new List<CraftSkill>();

            foreach (var item in dictionary)
            {
                skills.Add(new CraftSkill() 
                { 
                    Craft = item.Key, 
                    Level = item.Value 
                });
            }

            return skills;
        }

        public List<Material> ConvertToMaterialList(Dictionary<string, int> dictionary)
        {
            List<Material> materials = new List<Material>();

            foreach (var item in dictionary)
            {
                materials.Add(new Material()
                {
                    Ingredient = item.Key,
                    Quantity = item.Value
                });
            }

            return materials;
        }
    }
}
