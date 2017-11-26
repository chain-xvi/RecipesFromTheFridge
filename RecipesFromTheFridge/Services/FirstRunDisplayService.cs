using System;
using System.Threading.Tasks;

using RecipesFromTheFridge.Helpers;
using RecipesFromTheFridge.Views;

using Windows.ApplicationModel;

namespace RecipesFromTheFridge.Services
{
    public class FirstRunDisplayService
    {
        internal static async Task ShowIfAppropriateAsync()
        {
            bool hasShownFirstRun = false;
            hasShownFirstRun = await Windows.Storage.ApplicationData.Current.LocalSettings.ReadAsync<bool>(nameof(hasShownFirstRun));

            if (!hasShownFirstRun)
            {
                await Windows.Storage.ApplicationData.Current.LocalSettings.SaveAsync(nameof(hasShownFirstRun), true);
                var dialog = new FirstRunDialog();
                await dialog.ShowAsync();
            }
        }
    }
}
