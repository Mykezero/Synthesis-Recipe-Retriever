using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SynthesisRecipeRetriever.Classes
{
    public class XPaths
    {
        /// <summary>
        /// Xpath used to get recipe ids.
        /// </summary>
        public static readonly string ID_PATH = "//table[@class='stdtbl']/tr/td/div[@class='recipe-id']";

        /// <summary>
        /// Xpath used to get recipe materials.
        /// </summary>
        public static readonly string MATERIALS_PATH = "//table[@class='stdtbl']/tr/td/table[tr/td=\"Cost\"]";

        /// <summary>
        /// Xpath used to get all recipe skill requirements.
        /// Returns all table definitions including the ones that include recipe ids. 
        /// </summary>
        public static readonly string SKILL_PATH = "//table[@class='stdtbl']/tr/td[div[@class='recipe-skill']]";

        /// <summary>
        /// Xpath used to get recipe name.
        /// </summary>
        public static readonly string NAME_PATH = "//table[@class='stdtbl']/tr/td/table[tr/td=\"Profit\"]/tr/td[a]";

        /// <summary>
        /// Xpath used to get skill requirements for a particular recipe.
        /// </summary>
        public static readonly string SKILL_SUBPATH = "div[@class='recipe-skill']";
    }
}
