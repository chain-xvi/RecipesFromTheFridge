using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipesFromTheFridge.Models
{
    /// <summary>
    /// 
    /// </summary>
    class Ingredient
    {
        private string name;
        public string Name { get => name; set => name = value; }

        private int tag;
        public int Tag { get => tag; set => tag = value; }
    }

    static class IngredientManager
    {
        public static List<Ingredient> AllIngredients
        {
            get => new List<Ingredient>
            { 
                    new Ingredient{Name = "butter"},
                    new Ingredient{Name = "eggs"},
                    new Ingredient{Name = "milk"},
                    new Ingredient{Name = "parmesan"},
                    new Ingredient{Name = "cheddar"},
                    new Ingredient{Name = "cream"},
                    new Ingredient{Name = "sour cream"},
                    new Ingredient{Name = "cream cheese"},
                    new Ingredient{Name = "mozzarella"},
                    new Ingredient{Name = "american cheese"},
                    new Ingredient{Name = "yogurt"},
                    new Ingredient{Name = "evaporated milk"},
                    new Ingredient{Name = "condensed milk"},
                    new Ingredient{Name = "whipped cream"},
                    new Ingredient{Name = "half and half"},
                    new Ingredient{Name = "monterey jack cheese"},
                    new Ingredient{Name = "feta"},
                    new Ingredient{Name = "cottage cheese"},
                    new Ingredient{Name = "ice cream"},
                    new Ingredient{Name = "goat cheese"},
                    new Ingredient{Name = "frosting"},
                    new Ingredient{Name = "swiss cheese"},
                    new Ingredient{Name = "buttermilk"},
                    new Ingredient{Name = "velveeta"},
                    new Ingredient{Name = "ricotta"},
                    new Ingredient{Name = "powdered milk"},
                    new Ingredient{Name = "blue cheese"},
                    new Ingredient{Name = "provolone"},
                    new Ingredient{Name = "colby cheese"},
                    new Ingredient{Name = "gouda"},
                    new Ingredient{Name = "pepper jack"},
                    new Ingredient{Name = "italian cheese"},
                    new Ingredient{Name = "soft cheese"},
                    new Ingredient{Name = "romano"},
                    new Ingredient{Name = "brie"},
                    new Ingredient{Name = "pepperjack cheese"},
                    new Ingredient{Name = "italian cheese"},
                    new Ingredient{Name = "soft cheese"},
                    new Ingredient{Name = "romano"},
                    new Ingredient{Name = "brie"},
                    new Ingredient{Name = "pepperjack cheese"},
                    new Ingredient{Name = "cheese soup"},
                    new Ingredient{Name = "pizza cheese"},
                    new Ingredient{Name = "ghee"},
                    new Ingredient{Name = "pecorino cheese"},
                    new Ingredient{Name = "gruyere"},
                    new Ingredient{Name = "creme fraiche"},
                    new Ingredient{Name = "neufchatel"},
                    new Ingredient{Name = "muenster"},
                    new Ingredient{Name = "asiago"},
                    new Ingredient{Name = "queso fresco cheese"},
                    new Ingredient{Name = "hard cheese"},
                    new Ingredient{Name = "havarti cheese"},
                    new Ingredient{Name = "mascarpone"},
                    //new Ingredient{Name = "gruyere"},
                    //new Ingredient{Name = "gruyere"},
                    //new Ingredient{Name = "gruyere"},
                    //new Ingredient{Name = "gruyere"},
                    //new Ingredient{Name = "gruyere"},
                    //new Ingredient{Name = "gruyere"},
                    //new Ingredient{Name = "gruyere"},
                    //new Ingredient{Name = "gruyere"},
                    //new Ingredient{Name = "gruyere"},
                    //new Ingredient{Name = "gruyere"},
                    //new Ingredient{Name = "gruyere"},
                    //new Ingredient{Name = "gruyere"},
                    //new Ingredient{Name = "gruyere"},
                    //new Ingredient{Name = "gruyere"},
                    //new Ingredient{Name = "gruyere"},
                    //new Ingredient{Name = "gruyere"},
                    //new Ingredient{Name = "gruyere"},
                    //new Ingredient{Name = "gruyere"},
                    //new Ingredient{Name = "gruyere"},
                    //new Ingredient{Name = "gruyere"},
                    //new Ingredient{Name = "gruyere"},
                    //new Ingredient{Name = "gruyere"},
                };
        }
    }
}
