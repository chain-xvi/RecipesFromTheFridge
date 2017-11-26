using System;
using RecipesFromTheFridge.Services;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using RecipesFromTheFridge.Models;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Windows.ApplicationModel.Core;
using Windows.UI.ViewManagement;
using Windows.UI;

namespace RecipesFromTheFridge
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : Application
    {
        private Lazy<ActivationService> _activationService;

        private ActivationService ActivationService
        {
            get { return _activationService.Value; }
        }

        private static ObservableCollection<Ingredient> ingredientList;
        internal static ObservableCollection<Ingredient> IngredientList { get => ingredientList; set => ingredientList = value; }


        private static List<Recipe> favoriteRecipes;
        public static List<Recipe> FavoriteRecipes { get => favoriteRecipes; set => favoriteRecipes = value; }

        private static int recipeSliderValue;
        public static int RecipeSliderValue { get => recipeSliderValue; set => recipeSliderValue = value; }

        /// <summary>
        /// Initializes a new instance of the <see cref="App"/> class.
        /// This is the first line of authored code executed, and as such
        /// is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();

            // Deferred execution until used. Check https://msdn.microsoft.com/library/dd642331(v=vs.110).aspx for further info on Lazy<T> class.
            _activationService = new Lazy<ActivationService>(CreateActivationService);

            IngredientList = new ObservableCollection<Ingredient>();
            FavoriteRecipes = new List<Recipe>();
            RecipeSliderValue = 30;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            if (!e.PrelaunchActivated)
            {
                await ActivationService.ActivateAsync(e);
            }

            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
            ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

        }

        /// <summary>
        /// Invoked when the application is activated by some means other than normal launching.
        /// </summary>
        /// <param name="args">Event data for the event.</param>
        protected override async void OnActivated(IActivatedEventArgs args)
        {
            await ActivationService.ActivateAsync(args);
        }

        private ActivationService CreateActivationService()
        {
            return new ActivationService(this, typeof(Views.HomePage), new Views.ShellPage());
        }
    }
}
