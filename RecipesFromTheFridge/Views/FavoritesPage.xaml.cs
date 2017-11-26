using RecipesFromTheFridge.Models;
using RecipesFromTheFridge.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System.Runtime.CompilerServices;
using RecipesFromTheFridge.Helpers;

namespace RecipesFromTheFridge.Views
{
    public sealed partial class FavoritesPage : Page, INotifyPropertyChanged
    {

        /// <summary>
        /// The favorites <c>ObservableCollection{Recipe}</c> that it bound into the favorites GridView.
        /// </summary>
        ObservableCollection<Recipe> favoriteRecipes;


        /// <summary>
        /// The constructor of the <see cref="FavoritesPage"/>
        /// </summary>
        /// <para>
        /// After initializing components, we add the recipes in the <see cref="App.FavoriteRecipes"/>
        /// to the <see cref="favoriteRecipes"/>
        /// </para>
        public FavoritesPage()
        {
            InitializeComponent();
            favoriteRecipes = new ObservableCollection<Recipe>();
            foreach (Recipe recipe in App.FavoriteRecipes)
            {
                favoriteRecipes.Add(recipe);
            }
        }


        /// <summary>
        /// Handles the Incoming navigation to the FavoritesPage
        /// </summary>
        /// <para>
        /// It check if there is no internet connection by calling the <see cref="InternetConnectionAvailabilityCheckService.IsInternetAvailable()"/>
        /// and displays a dialog saying so by calling <see cref="NoInternetConnectionDialog.ShowNoInternetConnectionDialog()"/>
        /// </para>
        /// <param name="e">Holds the event data of the event</param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!InternetConnectionAvailabilityCheckService.IsInternetAvailable())
                await NoInternetConnectionDialog.ShowNoInternetConnectionDialog();
        }


        /// <summary>
        /// Handles the property changed event
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        
        
        private void Set<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            OnPropertyChanged(propertyName);
        }

        
        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));


        /// <summary>
        /// Handles the DeleteFromFavoriteButton Click to delete the recipe from the <see cref="favoriteRecipes"/>
        /// and the <see cref="App.FavoriteRecipes"/>
        /// </summary>
        /// <para>
        /// Makes a call to the <see cref="RecipesManipulator.RemoveRecipeFromFavorites(ObservableCollection{Recipe}, System.Collections.Generic.List{Recipe}, int)"/>
        /// and sends the two lists and the recipe's tag to delete it
        /// </para>
        /// <param name="sender">Holds the object that fires the event</param>
        /// <param name="e">Holds the event data of the event</param>
        private async void DeleteFromFavoriteButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            RecipesManipulator.RemoveRecipeFromFavorites(favoriteRecipes, App.FavoriteRecipes, Convert.ToInt16((sender as Button).Tag));

            // Serialize
            string FavoriteRecipesAsJson = Serializer.Serialize(App.FavoriteRecipes);
            await Serializer.SaveFavoriteRecipesAsync(FavoriteRecipesAsJson);
        }


        /// <summary>
        /// Handles the 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Page_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            // Serialize
            string FavoriteRecipesAsJson = Serializer.Serialize(App.FavoriteRecipes);
            await Serializer.SaveFavoriteRecipesAsync(FavoriteRecipesAsJson);
        }


        /// <summary>
        /// Handles the favorites GridView selection changed event.
        /// </summary>
        /// <para>
        /// Makes a navigation to the <see cref="WebViewPage"/> sending the selected recipe.
        /// </para>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GridView_SelectionChanged(object sender, SelectionChangedEventArgs e) =>
            Frame.Navigate(typeof(WebViewPage), favoriteRecipes[(sender as GridView).SelectedIndex]);
    }
}
