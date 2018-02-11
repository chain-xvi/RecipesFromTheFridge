using RecipesFromTheFridge.Models;
using RecipesFromTheFridge.Services;
using System.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml.Controls;
using Windows.Foundation;
using System.Threading.Tasks;

namespace RecipesFromTheFridge.Views
{
    public sealed partial class NutritionInfosPage : Page, INotifyPropertyChanged
    {

        /// <summary>
        /// Holds all the possible Measurement units that will get back in the json call...
        /// </summary>
        public List<string> Measures = new List<string> { "Pound", "Kilogram", "Ounce", "Gram", "Cup", "Liter", "Jumbo", "Whole", "Salt spoon", "Quart", "Teaspoon", "Smidgen", "Drop", "Gallon", "Dash", "Handful", "Scoop", "Bowl", "Cubic inch", "Pinch", "Milliliter", "Tablespoon", "Fluid ounce", "Bottle", "Tad", "Pint", "Dessert spoon" };
        
        double quantity = 0;
        
        public string FoodLabel { get; set; }
        
        private List<TextBlock> TextBlocks;

        public NutritionInfosPage()
        {
            InitializeComponent();
            TextBlocks = new List<TextBlock>() { CaloriesValue, CaloriesUnit, FatValue, FatUnit, SatFatValue, SatFatUnit, TansFatValue, TansFatUnit, CarbsValue, CarbsUnit, SugarsValue, SugarsUnit, ProteinValue, ProteinUnit, CholeValue, CholeUnit, SodiumValue, SodiumUnit, CalciumValue, CalciumUnit, MagnesiumValue, MagnesiumUnit, PotassiumValue, PotassiumUnit, IronValue, IronUnit, PhosphorusValue, PhosphorusUnit };
            //measureUnitComboBox.SelectedIndex = 0;
        }

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

        private async void getFoodButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(foodTextBox.Text))
            {
                NutritionRootObject nutritionRootObject = await WebFoodApiCallService.GetFoodApiCallAsync(foodTextBox.Text,
                    Measures.FirstOrDefault(m => Measures.IndexOf(m) == measureUnitComboBox.SelectedIndex),
                    quantity);

                if (nutritionRootObject != null)
                {
                    FoodLabelTextBlock.Text = nutritionRootObject.ingredients[0].parsed[0].food;
                    QuantityTextBlock.Text = nutritionRootObject.ingredients[0].parsed[0].quantity.ToString();
                    UnitTextBlock.Text = nutritionRootObject.ingredients[0].parsed[0].measure;


                    TotalNutrients totalNutrients = nutritionRootObject.totalNutrients;


                    ValuesAndUnitsWriter(TextBlocks, await CheckNullsAndReturnValidValues(totalNutrients));
                    initialInformatoionStackPanel.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    AllInformationStackPanel.Visibility = Windows.UI.Xaml.Visibility.Visible;
                }
            }
        }

        private void quantityTextBox_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
        {
            if (sender.Text != ".")
            {
                if (sender.Text.ToList().Count > 0)
                {
                    if (sender.Text.ToList<char>().Last<char>() != '.')
                    {
                        if (!Double.TryParse(sender.Text, out quantity))
                        {
                            sender.Text = String.Empty;
                        }
                    }
                    else
                    {
                        if (sender.Text.ToList().Count(p => p == '.') > 1)
                        {
                            List<Char> myChars = sender.Text.ToList();
                            myChars.RemoveAt(sender.Text.ToList().Count - 1);

                            string s = String.Empty;

                            foreach (var item in myChars)
                            {
                                s += item;
                            }
                            sender.Text = s;
                            sender.Select(sender.Text.Length, 0);
                        }
                    }
                }
            }
            else
            {
                sender.Text = String.Empty;
            }
        }

        private void ValuesAndUnitsWriter(List<TextBlock> textBoxesList, List<string> nutritionValue)
        {
            for (int i = 0; i < textBoxesList.Count; i++)
            {
                textBoxesList[i].Text = nutritionValue[i];
            }
        }

        private async Task<List<string>> CheckNullsAndReturnValidValues(TotalNutrients totalNutrients)
        {
            return await Task.Run(() =>
            {
                List<string> listOfValues = new List<string>();
                if (totalNutrients.ENERC_KCAL != null)
                {
                    listOfValues.AddRange(new List<string> { Math.Round(totalNutrients.ENERC_KCAL.quantity, 2).ToString(), totalNutrients.ENERC_KCAL.unit });
                }
                else
                {
                    listOfValues.AddRange(new List<string> { String.Empty, String.Empty });
                }

                if (totalNutrients.FAT != null)
                {
                    listOfValues.AddRange(new List<string> { Math.Round(totalNutrients.FAT.quantity, 2).ToString(), totalNutrients.FAT.unit });
                }
                else
                {
                    listOfValues.AddRange(new List<string> { String.Empty, String.Empty });
                }

                if (totalNutrients.FASAT != null)
                {
                    listOfValues.AddRange(new List<string> { Math.Round(totalNutrients.FASAT.quantity, 2).ToString(), totalNutrients.FASAT.unit });
                }
                else
                {
                    listOfValues.AddRange(new List<string> { String.Empty, String.Empty });
                }

                if (totalNutrients.FATRN != null)
                {
                    listOfValues.AddRange(new List<string> { Math.Round(totalNutrients.FATRN.quantity, 2).ToString(), totalNutrients.FATRN.unit });
                }
                else
                {
                    listOfValues.AddRange(new List<string> { String.Empty, String.Empty });
                }

                if (totalNutrients.CHOCDF != null)
                {
                    listOfValues.AddRange(new List<string> { Math.Round(totalNutrients.CHOCDF.quantity, 2).ToString(), totalNutrients.CHOCDF.unit });
                }
                else
                {
                    listOfValues.AddRange(new List<string> { String.Empty, String.Empty });
                }

                if (totalNutrients.SUGAR != null)
                {
                    listOfValues.AddRange(new List<string> { Math.Round(totalNutrients.SUGAR.quantity, 2).ToString(), totalNutrients.SUGAR.unit });
                }
                else
                {
                    listOfValues.AddRange(new List<string> { String.Empty, String.Empty });
                }

                if (totalNutrients.PROCNT != null)
                {
                    listOfValues.AddRange(new List<string> { Math.Round(totalNutrients.PROCNT.quantity, 2).ToString(), totalNutrients.PROCNT.unit });
                }
                else
                {
                    listOfValues.AddRange(new List<string> { String.Empty, String.Empty });
                }

                if (totalNutrients.CHOLE != null)
                {
                    listOfValues.AddRange(new List<string> { Math.Round(totalNutrients.CHOLE.quantity, 2).ToString(), totalNutrients.CHOLE.unit });
                }
                else
                {
                    listOfValues.AddRange(new List<string> { String.Empty, String.Empty });
                }

                if (totalNutrients.NA != null)
                {
                    listOfValues.AddRange(new List<string> { Math.Round(totalNutrients.NA.quantity, 2).ToString(), totalNutrients.NA.unit });
                }
                else
                {
                    listOfValues.AddRange(new List<string> { String.Empty, String.Empty });
                }

                if (totalNutrients.CA != null)
                {
                    listOfValues.AddRange(new List<string> { Math.Round(totalNutrients.CA.quantity, 2).ToString(), totalNutrients.CA.unit });
                }
                else
                {
                    listOfValues.AddRange(new List<string> { String.Empty, String.Empty });
                }

                if (totalNutrients.MG != null)
                {
                    listOfValues.AddRange(new List<string> { Math.Round(totalNutrients.MG.quantity, 2).ToString(), totalNutrients.MG.unit });
                }
                else
                {
                    listOfValues.AddRange(new List<string> { String.Empty, String.Empty });
                }

                if (totalNutrients.K != null)
                {
                    listOfValues.AddRange(new List<string> { Math.Round(totalNutrients.K.quantity, 2).ToString(), totalNutrients.K.unit });
                }
                else
                {
                    listOfValues.AddRange(new List<string> { String.Empty, String.Empty });
                }

                if (totalNutrients.FE != null)
                {
                    listOfValues.AddRange(new List<string> { Math.Round(totalNutrients.FE.quantity, 2).ToString(), totalNutrients.FE.unit });
                }
                else
                {
                    listOfValues.AddRange(new List<string> { String.Empty, String.Empty });
                }

                if (totalNutrients.P != null)
                {
                    listOfValues.AddRange(new List<string> { Math.Round(totalNutrients.P.quantity, 2).ToString(), totalNutrients.P.unit });
                }
                else
                {
                    listOfValues.AddRange(new List<string> { String.Empty, String.Empty });
                }

                return listOfValues;
            });


        }
    }
}
