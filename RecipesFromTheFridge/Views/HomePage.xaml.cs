using Newtonsoft.Json;
using RecipesFromTheFridge.Helpers;
using RecipesFromTheFridge.Models;
using RecipesFromTheFridge.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace RecipesFromTheFridge.Views
{
    /// <summary>
    /// 
    /// </summary>
    public sealed partial class HomePage : Page, INotifyPropertyChanged
    {
        private ObservableCollection<Ingredient> ingredientList;
        private ObservableCollection<Recipe> recipesList;


        /// <summary>
        /// The constructor of the <see cref="HomePage"/>
        /// </summary>
        public HomePage()
        {
            InitializeComponent();
        }


        /// <summary>
        /// Handles the Out Navigation event of the <c>MainPage</c>
        /// </summary>
        /// <remarks>
        /// the <c>ingredientList</c> is saved in the <c>App.IngredientList</c>
        /// </remarks>
        /// <param name="e">Contains the <c>NavigationEventArgs</c></param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (ingredientList.Count > 0)
                App.IngredientList = ingredientList;
        }


        /// <summary>
        /// Handles the Incoming user navigation event of the <c>MainPage</c>
        /// </summary>
        /// <remarks>
        /// <para>
        /// the <see cref="ingredientList"/> and the <see cref="App.IngredientList"/> are initialized.
        /// and the <see cref="RecipesNumberSlider"/> is being assigned the value of the <see cref="App.RecipeSliderValue"/>
        /// which is the previous (before navigation) <see cref="RecipesNumberSlider"/>
        /// </para>
        /// <para>
        /// If there are any ingredients, remake the call of the API and get the list of recipes
        /// <see cref="RecipesManipulator.GetRecipes(ObservableCollection{Ingredient}, ObservableCollection{Recipe}, GridView, int)"/>
        /// and then check if there is no internet connection reset the <see cref="App.IngredientList"/> and <see cref="ingredientList"/>
        /// </para>
        /// <para>
        /// Try and <see cref="Serializer.Serialize(object)"/> and then <see cref="Serializer.SaveFavoriteRecipesAsync(string)"/>,
        /// try because it depends on the existance of the file!
        /// </para>
        /// </remarks>
        /// <param name="e"></param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            ingredientList = App.IngredientList;
            recipesList = new ObservableCollection<Recipe>();

            RecipesNumberSlider.Value = App.RecipeSliderValue;

            if (ingredientList.Count > 0)
            {
                InternetWaitingProgressRing.IsActive = true;
                RecipesManipulator.GetRecipes(ingredientList, recipesList, RecipesGridView, Convert.ToInt16(RecipesNumberSlider.Value), ReasonsOfGettingRecipes.ThroughTextBoxText);
                if (!InternetConnectionAvailabilityCheckService.IsInternetAvailable())
                {
                    App.IngredientList = new ObservableCollection<Ingredient>();
                    ingredientList = App.IngredientList;
                }
            }

            try
            {
                string userAsJsonString = await Serializer.ReadFavoriteRecipesAsync();
                List<Recipe> favRecipes = Serializer.Deserialize<List<Recipe>>(userAsJsonString);
                App.FavoriteRecipes = favRecipes;
            }
            catch (Exception)
            {
                //File does not exist, still not saved!
            }

            InternetWaitingProgressRing.IsActive = false;

            ApplicationViewTitleBar titleBar = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar;
            titleBar.ForegroundColor = Colors.Black;
            titleBar.ButtonForegroundColor = Colors.Black;
        }


        /// <summary>
        /// The property changed event handler
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
        /// Handles the <see cref="AddIngredientButton"/> click event to add ingredients to the <see cref="ingredientList"/> and ultimately
        /// add recipes to the <see cref="recipesList"/>
        /// </summary>
        /// <para>
        /// Check if the given ingredient string is null or empty through the never old <see cref="String.IsNullOrEmpty(string)"/>
        /// </para>
        /// <para>
        /// check if that ingredient exists in the <see cref="IngredientManager.AllIngredients"/>, check for the internet connection
        /// availability through the <see cref="InternetConnectionAvailabilityCheckService.IsInternetAvailable()"/> this is a point so if everything is nominal moving on
        /// <strong>if it does</strong>, add it to the ingredientList through the <see cref="IngredientManipulator.AddIngredientToTheListView(ObservableCollection{Ingredient}, Ingredient)"/>
        /// and get the recipes through the <see cref="RecipesManipulator.GetRecipes(ObservableCollection{Ingredient}, ObservableCollection{Recipe}, GridView, int)"/>
        /// and then save the <see cref="RecipesNumberSlider"/> in the <see cref="App.RecipeSliderValue"/>
        /// <strong>if it doesn't</strong>, show a bunch of dialogs saying the message of the problem
        /// </para>
        /// <param name="sender">Holds data of the sender that fires the event</param>
        /// <param name="e">Holds event data of the event</param>
        private async void AddIngredientButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(NewIngredientAutoSuggestBox.Text))
            {
                //TODO >>> check if the recipe exists in the App.FavoriteRecipes and if so make it favored!
                Ingredient ingredient = IngredientManager.AllIngredients.FirstOrDefault(i => i.Name == NewIngredientAutoSuggestBox.Text.ToLower());
                if (ingredient != null)
                {
                    if (InternetConnectionAvailabilityCheckService.IsInternetAvailable())
                    {
                        InternetWaitingProgressRing.IsActive = true;
                        IngredientManipulator.AddIngredientToTheListView(ingredientList, ingredient);
                        RecipesManipulator.GetRecipes(ingredientList, recipesList, RecipesGridView, Convert.ToInt16(RecipesNumberSlider.Value), ReasonsOfGettingRecipes.ThroughTextBoxText);
                        App.RecipeSliderValue = Convert.ToInt16(RecipesNumberSlider.Value);
                    }
                    else
                    {
                        await NoInternetConnectionDialog.ShowNoInternetConnectionDialog();
                        ingredientList = new ObservableCollection<Ingredient>();
                        App.IngredientList = new ObservableCollection<Ingredient>();
                    }
                }
                else
                {
                    ShowKiddingMeFlyout("The ingredient you entered doesn't exist on this planet...");
                }
            }
            else
            {
                ShowKiddingMeFlyout("No ingredient entered");
            }
            NewIngredientAutoSuggestBox.Text = String.Empty;

            InternetWaitingProgressRing.IsActive = false;
        }


        /// <summary>
        /// Shows a Flyout with the sent string as Content
        /// </summary>
        /// <param name="kiddingMeFlyoutText">Holds the flyout string</param>
        private void ShowKiddingMeFlyout(string kiddingMeFlyoutText)
        {
            (KiddingMeFlyout.Content as TextBlock).Text = kiddingMeFlyoutText;
            KiddingMeFlyout.ShowAt(NewIngredientAutoSuggestBox);
        }


        /// <summary>
        /// Handles the <c>DeleteIngredientButton</c> click event
        /// </summary>
        /// <para>
        /// Deletes the ingredient through the <see cref="IngredientManipulator.DeleteIngredient(ObservableCollection{Ingredient}, int)"/></para>
        /// and re-gets the recipes based on the updated <see cref="ingredientList"/> through the <see cref="RecipesManipulator.GetRecipes(ObservableCollection{Ingredient}, ObservableCollection{Recipe}, GridView, int)"/>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteIngredientButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            IngredientManipulator.DeleteIngredient(ingredientList, Convert.ToInt16((sender as Button).Tag));
            RecipesManipulator.GetRecipes(ingredientList, recipesList, RecipesGridView, Convert.ToInt16(RecipesNumberSlider.Value), ReasonsOfGettingRecipes.ThroughTextBoxText);
        }


        /// <summary>
        /// Handles the <see cref="NewIngredientAutoSuggestBox"/>_TextChanged event
        /// </summary>
        /// <para>
        /// Checks if the user input is the reason of the text changing, if it is,
        /// get all the ingredients that start with the given text through a good old
        /// <see cref="System.Linq"/>.
        /// then the retreived ingredients are stored in a list of ingredients, and
        /// since we want just their names we are gonna retreive them and store them
        /// in a list then assign it to the autosuggestbox's ItemSource.
        /// </para>
        /// <param name="sender">Holds the <see cref="AutoSuggestBox"/> that fires the event </param>
        /// <param name="args">Holds the event data</param>
        private void NewIngredientAutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                var wantedIngredients = IngredientManager.AllIngredients.Where(i => i.Name.StartsWith(sender.Text.ToLower()));

                List<string> wantedNames = new List<string>();
                foreach (Ingredient ingredient in wantedIngredients)
                {
                    wantedNames.Add(ingredient.Name);
                }
                sender.ItemsSource = wantedNames;
            }
        }


        /// <summary>
        /// Handles the <see cref="RecipesGridView"/> SelectionChanged event
        /// </summary>
        /// <para>
        /// Makes the frame navigate to the <see cref="WebViewPage"/> and sends the selected recipe as a parameter
        /// </para>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GridView_SelectionChanged(object sender, SelectionChangedEventArgs e) =>
            Frame.Navigate(typeof(WebViewPage), recipesList[(sender as GridView).SelectedIndex]);


        /// <summary>
        /// Handles the <c>AddToFavoritesButton.Click</c> event to add the recipe to favorites
        /// </summary>
        /// <para>
        /// Gets the Recipe that's wanted to be favored, through (again) a good old <see cref="System.Linq"/>
        /// and then duplicate that recipe and see if it exists in the <see cref="App.FavoriteRecipes"/>
        /// and then, if it is, unfavorite it, if it is not, favorite it!
        /// </para>
        /// <para>
        /// Serialize.
        /// </para>
        /// <param name="sender">Holds the object that fires the event</param>
        /// <param name="e">Contains event data of the event</param>
        private async void AddToFavoritesButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Recipe wantedToBeFavoredOrUnfavoredRecipe = recipesList.FirstOrDefault(r => r.tag == Convert.ToInt16((sender as Button).Tag));

            Recipe rec = App.FavoriteRecipes.FirstOrDefault(recipe => recipe.image_url == wantedToBeFavoredOrUnfavoredRecipe.image_url);
            if (!wantedToBeFavoredOrUnfavoredRecipe.IsFavored && rec == null)
            {
                wantedToBeFavoredOrUnfavoredRecipe.IsFavored = true;
                App.FavoriteRecipes.Add(wantedToBeFavoredOrUnfavoredRecipe);
                (((sender as Button).Flyout as Flyout).Content as TextBlock).Text = String.Format("{0} {1}", wantedToBeFavoredOrUnfavoredRecipe.title, "was added to the favorites");
                (sender as Button).Flyout.ShowAt((sender as Button));
                (sender as Button).Style = Application.Current.Resources["CompactFavorite"] as Style;
            }
            else
            {
                wantedToBeFavoredOrUnfavoredRecipe.IsFavored = false;
                App.FavoriteRecipes.Remove(rec);
                (((sender as Button).Flyout as Flyout).Content as TextBlock).Text = String.Format("{0} {1}", wantedToBeFavoredOrUnfavoredRecipe.title, "was removed from the favorites");
                (sender as Button).Flyout.ShowAt((sender as Button));
                (sender as Button).Style = Application.Current.Resources["CompactNotFavorite"] as Style;
            }

            string FavoriteRecipesAsJson = Serializer.Serialize(App.FavoriteRecipes);
            await Serializer.SaveFavoriteRecipesAsync(FavoriteRecipesAsJson);

        }


        /// <summary>
        /// Handles the key up event as the <see cref="Page_KeyUp(object, Windows.UI.Xaml.Input.KeyRoutedEventArgs)"/>
        /// mainly to check for the <see cref="Windows.System.VirtualKey.Enter"/> click to call the
        /// <see cref="AddIngredientButton_Click(object, RoutedEventArgs)"/>
        /// </summary>
        /// <param name="sender">Holds the object that fires the event</param>
        /// <param name="e">Contains event data of the event</param>
        private void Page_KeyUp(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                if (!NewIngredientAutoSuggestBox.IsSuggestionListOpen)
                {
                    AddIngredientButton_Click(AddIngredientButton, new RoutedEventArgs());
                }
            }
        }


        /// <summary>
        /// Handles the <see cref="RecipesNumberSlider"/>.ValueChanged event 
        /// </summary>
        /// <para>
        /// first of all a delay is create by calling the <see cref="CreateADelay(int)"/> to give the user some time to change
        /// the value,
        /// </para>
        /// <para>
        /// Then check if there are any recipes in the <see cref="recipesList"/>, if true, call the <see cref="RecipesManipulator.GetRecipes(ObservableCollection{Ingredient}, ObservableCollection{Recipe}, GridView, int)"/>
        /// passing in the slider's value as an int value!
        /// </para>
        /// <param name="sender">Holds the object that fire the event</param>
        /// <param name="e">Holds event data of the event</param>
        private void RecipesNumberSlider_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            if (recipesList != null && recipesList.Any())
            {
                RecipesManipulator.GetRecipes(ingredientList, recipesList, RecipesGridView, Convert.ToInt32(RecipesNumberSlider.Value), ReasonsOfGettingRecipes.ThroughChangingSliderValue, 2000);
            }
        }




        //TODO. look for the added recipes in the add button click event handler, if there are any recipes
        //      that are previously been favored by their url, and make their foreground pink!
        //      that is all

    }
}
