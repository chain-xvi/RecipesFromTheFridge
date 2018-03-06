using GoalList.Helpers;
using RecipesFromTheFridge.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RecipesFromTheFridge.Services
{
    class WebFoodApiCallService
    {

        internal async static Task<NutritionRootObject> GetFoodApiCallAsync(string food, string measure, double quantity) =>
 // TODO: Now we will get food URIs from the Db, (just one tbh 🤗😊),
 //      then we need to call the other API that will hold the info and data to display...
 //      https://developer.edamam.com/edamam-docs-nutrition-api
 await Task.Run(async () =>
 {
     HttpClient client = new HttpClient();
     if (!String.IsNullOrEmpty(food))
     {
         HttpResponseMessage responseMessage = await client.GetAsync("https://api.edamam.com/api/food-database/parser?ingr=" + food + "&app_id=e1a8d40f&app_key=70c5acdfd4f0daf79e727fdf220e17ef");
         string jsonResponse = await responseMessage.Content.ReadAsStringAsync();
         string s = jsonResponse;
         try
         {
             // Monitor the json response
             FoodRootObject foodRootObject = await Json.ToObjectAsync<FoodRootObject>(jsonResponse);

             if (foodRootObject.parsed.Count == 0)
             {
                 return null;
             }

             if (foodRootObject.parsed[0].quantity == 0)
             {
                 quantity = 1;
             }
             else
             {
                 quantity = foodRootObject.parsed[0].quantity;
             }
             string measureURI = String.Empty;

             if (foodRootObject.parsed[0].measure == null)
             {
                 if (measure == null)
                 {
                     measureURI = foodRootObject.hints[0].measures.FirstOrDefault(m => m.label == "Pound").uri;
                 }
                 else
                 {
                     measureURI = foodRootObject.hints[0].measures.FirstOrDefault(m => m.label == measure).uri;
                 }
             }
             else
             {
                 measureURI = foodRootObject.parsed[0].measure.uri;
             }

             NutritionRootObject nutritionRootObject = await GetNutritionInfoAsync(foodRootObject, measureURI, quantity);
             return nutritionRootObject;
         }
         catch (Exception)
         {
             return null;
         }
     }
     else
     {
         return null;
     }
 });


        private static async Task<NutritionRootObject> GetNutritionInfoAsync(FoodRootObject food, string measure, double quantity = 1)
        {
            return await Task.Run(async () =>
            {
                string result;
                var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api.edamam.com/api/food-database/nutrients?app_id=e1a8d40f&app_key=70c5acdfd4f0daf79e727fdf220e17ef");
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                string jsonToSend = "{\"yield\": 1," +
                    "\"ingredients\": [{\"quantity\":" + quantity + "," +
                    "\"measureURI\": \"" + measure + "\"," +
                    "\"foodURI\": \"" + food.parsed[0].food.uri + "\"}]}";

                using (var streamWriter = new StreamWriter(await httpWebRequest.GetRequestStreamAsync()))
                {
                    streamWriter.Write(jsonToSend);
                    streamWriter.Flush();
                }

                var httpResponse = await httpWebRequest.GetResponseAsync();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    result = streamReader.ReadToEnd();
                }

                try
                {
                    NutritionRootObject nutritionRootObject = await Json.ToObjectAsync<NutritionRootObject>(result);
                    return nutritionRootObject;
                }
                catch (Exception)
                {
                    return null;
                }
            });

        }
    }
}
