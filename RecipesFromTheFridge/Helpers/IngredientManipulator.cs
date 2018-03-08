using RecipesFromTheFridge.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace RecipesFromTheFridge.Helpers
{
    class IngredientManipulator
    {
        /// <summary>
        /// Adds ingredient to the allIngredient collection based on the sent indredient.
        /// <para>
        /// Check if the sent ingredient is already in the collection.
        /// If true, add it, otherwise return.
        /// </para>
        /// </summary>
        /// <param name="allIngredients">the ingredients collection.</param>
        /// <param name="ingredient">the ingredient to add.</param>
        public static void AddIngredientToTheListView(ObservableCollection<Ingredient> allIngredients, Ingredient ingredient)
        {   
            if (allIngredients.FirstOrDefault(i => i.Name == ingredient.Name) == null)
            {
                allIngredients.Add(new Ingredient { Name = ingredient.Name });
                TagIngredients(allIngredients);
            }
            else
            {
                return;
            }
        }


        /// <summary>
        /// Tags the items properly in the collection, mainly to get them when we want them out of the collection
        /// in this case to delete the ingredients
        /// </summary>
        /// <param name="allingredients">holds the ingredients collection</param>
        private static void TagIngredients(ObservableCollection<Ingredient> allingredients)
        {
            int i = 1;
            foreach (Ingredient ingredient in allingredients)
            {
                ingredient.Tag = i;
                i++;
            }
        }


        /// <summary>
        /// Deletes the ingredient based on its tag.
        /// </summary>
        /// <param name="allIngredients">Holds the ingredients collection</param>
        /// <param name="tag">Holds the tag</param>
        public static void DeleteIngredient(ObservableCollection<Ingredient> allIngredients, int tag)
        {
            allIngredients.Remove(allIngredients.FirstOrDefault(i => i.Tag == tag));
        }
    }
}
