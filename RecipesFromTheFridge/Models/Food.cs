using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipesFromTheFridge.Models
{
    public class Food
    {
        public string uri { get; set; }
        public string label { get; set; }
    }

    public class Parsed
    {
        public Food food { get; set; }
        public double quantity { get; set; }
        public Measure measure { get; set; }
    }

    public class Food2
    {
        public string uri { get; set; }
        public string label { get; set; }
    }

    public class Measure
    {
        public string uri { get; set; }
        public string label { get; set; }
    }

    public class Hint
    {
        public Food2 food { get; set; }
        public List<Measure> measures { get; set; }
    }

    public class FoodRootObject
    {
        public string text { get; set; }
        public List<Parsed> parsed { get; set; }
        public List<Hint> hints { get; set; }
        public int page { get; set; }
        public int numPages { get; set; }
    }
}
