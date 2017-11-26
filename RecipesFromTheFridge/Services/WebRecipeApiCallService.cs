using RecipesFromTheFridge.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using RecipesFromTheFridge.Helpers;
using GoalList.Helpers;
using unirest_net.http;
using Windows.UI.Core;
using Newtonsoft.Json;

namespace RecipesFromTheFridge.Services
{
    public class WebRecipeApiCallService
    {
        internal async static Task<RootObject> GetRecipeTitleByCallingFoodAPI(ObservableCollection<Ingredient> allIngredients)
        {
            HttpClient client = new HttpClient();
            if (allIngredients.Count != 0)
            {
                HttpClient httpClient = new HttpClient();
                string listOfIngredients = String.Empty;
                if (allIngredients.Count == 1)
                {
                    foreach (Ingredient ingredient in allIngredients)
                    {
                        listOfIngredients += ingredient.Name;
                    }
                }
                else
                {
                    foreach (Ingredient ingredient in allIngredients)
                    {
                        listOfIngredients += ingredient.Name + ",";
                    }
                }

                HttpResponseMessage response = await client.GetAsync("http://food2fork.com/api/search?key=1361ee921fc663845ecb612e49d63797&q=" + listOfIngredients);
                string JsonAsString = await response.Content.ReadAsStringAsync();
                RootObject rootObject = JsonConvert.DeserializeObject<RootObject>(JsonAsString);
                //GetRecipeMainUrl(rootObject);
                return rootObject;
            }
            else
            {
                return null;
            }
        }
    }
}
