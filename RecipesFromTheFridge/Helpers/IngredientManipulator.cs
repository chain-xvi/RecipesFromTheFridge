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
        public static void AddIngredientToTheListView(ObservableCollection<Ingredient> allingredients, Ingredient ingredient)
        {   
            if (allingredients.FirstOrDefault(i => i.Name == ingredient.Name) == null)
            {
                allingredients.Add(new Ingredient { Name = ingredient.Name });
                TagIngredients(allingredients);
            }
            else
            {
                return;
            }
        }

        private static void TagIngredients(ObservableCollection<Ingredient> allingredients)
        {
            int i = 1;
            foreach (Ingredient ingredient in allingredients)
            {
                ingredient.Tag = i;
                i++;
            }
        }

        public static void DeleteIngredient(ObservableCollection<Ingredient> allIngredients, int tag)
        {
            allIngredients.Remove(allIngredients.FirstOrDefault(i => i.Tag == tag));
        }
    }
}
