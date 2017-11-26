using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;

namespace RecipesFromTheFridge.Models
{
    public class Recipe
    {
        public string publisher { get; set; }
        public string f2f_url { get; set; }
        public string title { get; set; }
        public string source_url { get; set; }
        public string recipe_id { get; set; }
        public string image_url { get; set; }
        public double social_rank { get; set; }
        public string publisher_url { get; set; }
        public bool IsFavored { get; set; }
        public int tag { get; set; }
    }

    public class RootObject
    {
        public ObservableCollection<Recipe> recipes { get; set; }
    }
}
