using RecipesFromTheFridge.Models;
using RecipesFromTheFridge.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace RecipesFromTheFridge.Helpers
{
    class RecipesManipulator
    {

        /// <summary>
        /// Makes a call to <see cref="WebRecipeApiCallService.GetRecipeTitleByCallingFoodAPI(ObservableCollection{Ingredient})"/> to get recipes and adds them to the lists
        /// </summary>
        /// <para>
        /// Makes a call to <see cref="WebRecipeApiCallService.GetRecipeTitleByCallingFoodAPI(ObservableCollection{Ingredient})"/>
        /// to get a list of recipes
        /// </para>
        /// <para>
        /// Assigns all the recipes to the allRecipes list with the restriction of the count, after removing every recipe in it!
        /// </para>
        /// <para>
        /// Then tags the recipes in the allRecipes list, by calling the <see cref="TagRecipes(ObservableCollection{Recipe})"/>
        /// and favorites them by calling <see cref="ReFavoriteRecipes(ObservableCollection{Recipe}, List{Recipe})"/>
        /// </para>
        /// <param name="allIngredients">Holds the ingredient list</param>
        /// <param name="allRecipes">Holds the recipes list</param>
        /// <param name="recipesGridView">Holds the recipes GridView</param>
        /// <param name="count">Holds the count of the recipes</param>
        internal async static void GetRecipes(ObservableCollection<Ingredient> allIngredients,
            ObservableCollection<Recipe> allRecipes,
            GridView recipesGridView,
            int count,
            ReasonsOfGettingRecipes reasons,
            int ms = 0)
        {
            if (reasons == ReasonsOfGettingRecipes.ThroughChangingSliderValue)
            {
                await CreatingDelay.CreateDelay(ms);
            }

            try
            {
                RootObject rootObject = await WebRecipeApiCallService.GetRecipeTitleByCallingFoodAPI(allIngredients);

                foreach (Recipe recipe in allRecipes.ToList())
                {
                    allRecipes.Remove(recipe);
                }

                int i = 1;
                foreach (Recipe recipe in rootObject.recipes)
                {
                    if (i <= count)
                    {
                        allRecipes.Add(recipe);
                        i++;
                    }
                    else
                    {
                        break;
                    }
                }
                TagRecipes(allRecipes);
                ReFavoriteRecipes(allRecipes, App.FavoriteRecipes, recipesGridView);

            }
            catch (Exception)
            {
                // No internet connection.
            }
        }


        /// <summary>
        /// Tags each and every recipe in the list.
        /// </summary>
        /// <param name="recipesCollection">Holds the recipes list.</param>
        private static void TagRecipes(ObservableCollection<Recipe> recipesCollection)
        {
            int i = 1;
            foreach (Recipe recipe in recipesCollection)
            {
                recipe.tag = i;
                i++;
            }
        }


        /// <summary>
        /// Removes the recipe -that's been gotten by its tag- from the lists, both the regular list and the App level list.
        /// </summary>
        /// <param name="allFavoriteRecipes">Holds the recipes list</param>
        /// <param name="allFavoriteRecipesAppLevel">Holds the App level recipes list</param>
        /// <param name="tag">recipe's tag</param>
        internal static void RemoveRecipeFromFavorites(ObservableCollection<Recipe> allFavoriteRecipes, List<Recipe> allFavoriteRecipesAppLevel, int tag)
        {
            allFavoriteRecipes.Remove(allFavoriteRecipes.FirstOrDefault(r => r.tag == tag));
            allFavoriteRecipesAppLevel.Remove(allFavoriteRecipesAppLevel.FirstOrDefault(r => r.tag == tag));
        }


        /// <summary>
        /// Remakes the recipes as favorites
        /// </summary>
        /// <param name="allRecipes">Holds the allRecipes list</param>
        /// <param name="favoriteRecipes">Holds the favorite recipes</param>
        private static void ReFavoriteRecipes(ObservableCollection<Recipe> allRecipes, List<Recipe> favoriteRecipes, GridView gridView)
        {
            foreach (Recipe recipe in allRecipes)
            {
                foreach (Recipe reci in favoriteRecipes)
                {
                    if (recipe.source_url == reci.source_url)
                    {
                        recipe.IsFavored = true;
                    }
                }
            }
        }
    }
}
