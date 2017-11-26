using Newtonsoft.Json;
using RecipesFromTheFridge.Helpers;
using RecipesFromTheFridge.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace RecipesFromTheFridge.Services
{
    class Serializer
    {
        static StorageFolder localStorageFolder = ApplicationData.Current.LocalFolder;

        private static readonly JsonSerializerSettings settings = new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.All,
            Formatting = Formatting.Indented
        };

        public static string Serialize(object obj)
        {
            if (obj == null)
            {
                throw new NullReferenceException();
            }

            string jsonString = JsonConvert.SerializeObject(obj, settings);
            return jsonString;
        }

        public static T Deserialize<T>(string jsonString)
        {
            T obj = JsonConvert.DeserializeObject<T>(jsonString, settings);
            return obj;
        }

        internal async static Task SaveFavoriteRecipesAsync(string content)
        {
            await Task.Run(async () =>
            {
                StorageFile file = await localStorageFolder.CreateFileAsync("TheGreatFavoriteRecipes.json", CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(file, content);
            });
            await Task.CompletedTask;
        }

        internal async static Task<string> ReadFavoriteRecipesAsync()
        {
            StorageFile file = await localStorageFolder.GetFileAsync("TheGreatFavoriteRecipes.json");
            string userAsJson = await FileIO.ReadTextAsync(file);
            return userAsJson;
        }
    }
}
