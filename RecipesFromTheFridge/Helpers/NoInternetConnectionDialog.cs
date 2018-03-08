using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace RecipesFromTheFridge.Helpers
{
    class NoInternetConnectionDialog
    {

        /// <summary>
        /// Shows a dialog saying there is no internet connection...
        /// </summary>
        /// <returns>returns an asynchronous Task</returns>
        internal static async Task ShowNoInternetConnectionDialog()
        {
            ContentDialog dialog = new ContentDialog
            {
                Title="Whoops... No Internet Connection Was Found!",
                Content="There Is No Internet Connection. Please Check Your Network And Try Again Later!",
                CloseButtonText = "OK"
            };
            await dialog.ShowAsync();
        }
    }
}
