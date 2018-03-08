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
            TextBlocks = new List<TextBlock>() { CaloriesValue, CaloriesUnit, FatValue, FatUnit, SatFatValue, SatFatUnit, TansFatValue, TansFatUnit, CarbsValue, CarbsUnit, SugarsValue, SugarsUnit, ProteinValue, ProteinUnit, CholeValue, CholeUnit, SodiumValue, SodiumUnit, CalciumValue, CalciumUnit, MagnesiumValue, MagnesiumUnit, PotassiumValue, PotassiumUnit, IronValue, IronUnit, PhosphorusValue, PhosphorusUnit, VitAValue, VitAUnit, VitBValue, VitBFatUnit, VitDValue, VitDUnit, VitEValue, VitEUnit, VitKValue, VitKUnit, NiacinValue, NiacinUnit, VitB12Value, VitB12Unit, ThiaminValue, ThiaminUnit, RiboflavinValue, RiboflavinUnit, FolateValue, FolateUnit, CaloriesDailyValue, CaloriesDailyUnit, FatDailyValue, FatDailyUnit, SatFatDailyValue, SatFatDailyUnit, CarbsDailyValue, CarbsDailyUnit, ProteinDailyValue, ProteinDailyUnit, CholeDailyValue, CholeDailyUnit, SodiumDailyValue, SodiumDailyUnit, CalciumDailyValue, CalciumDailyUnit, MagnesiumDailyValue, MagnesiumDailyUnit, PotassiumDailyValue, PotassiumDailyUnit, IronDailyValue, IronDailyUnit, PhosphorusDailyValue, PhosphorusDailyUnit };
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


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void GetFoodButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
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
                    TotalDaily totalDaily = nutritionRootObject.totalDaily;
                    

                    ValuesAndUnitsWriter(TextBlocks, await CheckNullsAndReturnValidValues(totalNutrients, totalDaily));
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
        

        /// <summary>
        /// Check the nutrition values if null, else, then add them to a list.
        /// <para>
        /// Some food types don't have certain values, so they get null, and generate exceptions,
        /// </para>
        /// </summary>
        /// <param name="totalNutrients">the TotalNutrients object that holds all the incoming
        /// values of the nutrition informations
        /// </param>
        /// <returns>A list full of valid values.</returns>
        private async Task<List<string>> CheckNullsAndReturnValidValues(TotalNutrients totalNutrients, TotalDaily totalDaily)
        {
            return await Task.Run(() =>
            {
                List<string> listOfValues = new List<string>();
                if (totalNutrients.ENERC_KCAL != null)
                {
                    listOfValues.AddRange(new List<string> { Math.Round(totalNutrients.ENERC_KCAL.Quantity, 2).ToString(), totalNutrients.ENERC_KCAL.unit });
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

                if (totalNutrients.VITA_RAE != null)
                {
                    listOfValues.AddRange(new List<string> { Math.Round(totalNutrients.VITA_RAE.quantity, 2).ToString(), totalNutrients.VITA_RAE.unit });
                }
                else
                {
                    listOfValues.AddRange(new List<string> { String.Empty, String.Empty });
                }

                if (totalNutrients.VITB6A != null)
                {
                    listOfValues.AddRange(new List<string> { Math.Round(totalNutrients.VITB6A.quantity, 2).ToString(), totalNutrients.VITB6A.unit });
                }
                else
                {
                    listOfValues.AddRange(new List<string> { String.Empty, String.Empty });
                }

                if (totalNutrients.VITD != null)
                {
                    listOfValues.AddRange(new List<string> { Math.Round(totalNutrients.VITD.quantity, 2).ToString(), totalNutrients.VITD.unit });
                }
                else
                {
                    listOfValues.AddRange(new List<string> { String.Empty, String.Empty });
                }

                if (totalNutrients.TOCPHA != null)
                {
                    listOfValues.AddRange(new List<string> { Math.Round(totalNutrients.TOCPHA.quantity, 2).ToString(), totalNutrients.TOCPHA.unit });
                }
                else
                {
                    listOfValues.AddRange(new List<string> { String.Empty, String.Empty });
                }

                if (totalNutrients.VITK1 != null)
                {
                    listOfValues.AddRange(new List<string> { Math.Round(totalNutrients.VITK1.quantity, 2).ToString(), totalNutrients.VITK1.unit });
                }
                else
                {
                    listOfValues.AddRange(new List<string> { String.Empty, String.Empty });
                }

                if (totalNutrients.NIA != null)
                {
                    listOfValues.AddRange(new List<string> { Math.Round(totalNutrients.NIA.quantity, 2).ToString(), totalNutrients.NIA.unit });
                }
                else
                {
                    listOfValues.AddRange(new List<string> { String.Empty, String.Empty });
                }

                if (totalNutrients.VITB12 != null)
                {
                    listOfValues.AddRange(new List<string> { Math.Round(totalNutrients.VITB12.quantity, 2).ToString(), totalNutrients.VITB12.unit });
                }
                else
                {
                    listOfValues.AddRange(new List<string> { String.Empty, String.Empty });
                }

                if (totalNutrients.THIA != null)
                {
                    listOfValues.AddRange(new List<string> { Math.Round(totalNutrients.THIA.quantity, 2).ToString(), totalNutrients.THIA.unit });
                }
                else
                {
                    listOfValues.AddRange(new List<string> { String.Empty, String.Empty });
                }

                if (totalNutrients.RIBF != null)
                {
                    listOfValues.AddRange(new List<string> { Math.Round(totalNutrients.RIBF.quantity, 2).ToString(), totalNutrients.RIBF.unit });
                }
                else
                {
                    listOfValues.AddRange(new List<string> { String.Empty, String.Empty });
                }

                if (totalNutrients.FOLDFE != null)
                {
                    listOfValues.AddRange(new List<string> { Math.Round(totalNutrients.FOLDFE.quantity, 2).ToString(), totalNutrients.FOLDFE.unit });
                }
                else
                {
                    listOfValues.AddRange(new List<string> { String.Empty, String.Empty });
                }



                if (totalDaily.ENERC_KCAL != null)
                {
                    listOfValues.AddRange(new List<string> { Math.Round(totalDaily.ENERC_KCAL.quantity, 2).ToString(), totalDaily.ENERC_KCAL.unit });
                }
                else
                {
                    listOfValues.AddRange(new List<string> { String.Empty, String.Empty });
                }

                if (totalDaily.FAT != null)
                {
                    listOfValues.AddRange(new List<string> { Math.Round(totalDaily.FAT.quantity, 2).ToString(), totalDaily.FAT.unit });
                }
                else
                {
                    listOfValues.AddRange(new List<string> { String.Empty, String.Empty });
                }

                if (totalDaily.FASAT != null)
                {
                    listOfValues.AddRange(new List<string> { Math.Round(totalDaily.FASAT.quantity, 2).ToString(), totalDaily.FASAT.unit });
                }
                else
                {
                    listOfValues.AddRange(new List<string> { String.Empty, String.Empty });
                }

                if (totalDaily.CHOCDF != null)
                {
                    listOfValues.AddRange(new List<string> { Math.Round(totalDaily.CHOCDF.quantity, 2).ToString(), totalDaily.CHOCDF.unit });
                }
                else
                {
                    listOfValues.AddRange(new List<string> { String.Empty, String.Empty });
                }

                if (totalDaily.PROCNT != null)
                {
                    listOfValues.AddRange(new List<string> { Math.Round(totalDaily.PROCNT.quantity, 2).ToString(), totalDaily.PROCNT.unit });
                }
                else
                {
                    listOfValues.AddRange(new List<string> { String.Empty, String.Empty });
                }

                if (totalDaily.CHOLE != null)
                {
                    listOfValues.AddRange(new List<string> { Math.Round(totalDaily.CHOLE.quantity, 2).ToString(), totalDaily.CHOLE.unit });
                }
                else
                {
                    listOfValues.AddRange(new List<string> { String.Empty, String.Empty });
                }

                if (totalDaily.NA != null)
                {
                    listOfValues.AddRange(new List<string> { Math.Round(totalDaily.NA.quantity, 2).ToString(), totalDaily.NA.unit });
                }
                else
                {
                    listOfValues.AddRange(new List<string> { String.Empty, String.Empty });
                }

                if (totalDaily.CA != null)
                {
                    listOfValues.AddRange(new List<string> { Math.Round(totalDaily.CA.quantity, 2).ToString(), totalDaily.CA.unit });
                }
                else
                {
                    listOfValues.AddRange(new List<string> { String.Empty, String.Empty });
                }

                if (totalDaily.MG != null)
                {
                    listOfValues.AddRange(new List<string> { Math.Round(totalDaily.MG.quantity, 2).ToString(), totalDaily.MG.unit });
                }
                else
                {
                    listOfValues.AddRange(new List<string> { String.Empty, String.Empty });
                }

                if (totalDaily.K != null)
                {
                    listOfValues.AddRange(new List<string> { Math.Round(totalDaily.K.quantity, 2).ToString(), totalDaily.K.unit });
                }
                else
                {
                    listOfValues.AddRange(new List<string> { String.Empty, String.Empty });
                }

                if (totalDaily.FE != null)
                {
                    listOfValues.AddRange(new List<string> { Math.Round(totalDaily.FE.quantity, 2).ToString(), totalDaily.FE.unit });
                }
                else
                {
                    listOfValues.AddRange(new List<string> { String.Empty, String.Empty });
                }

                if (totalDaily.P != null)
                {
                    listOfValues.AddRange(new List<string> { Math.Round(totalDaily.P.quantity, 2).ToString(), totalDaily.P.unit });
                }
                else
                {
                    listOfValues.AddRange(new List<string> { String.Empty, String.Empty });
                }

                if (totalDaily.VITA_RAE != null)
                {
                    listOfValues.AddRange(new List<string> { Math.Round(totalDaily.VITA_RAE.quantity, 2).ToString(), totalDaily.VITA_RAE.unit });
                }
                else
                {
                    listOfValues.AddRange(new List<string> { String.Empty, String.Empty });
                }

                if (totalDaily.VITB6A != null)
                {
                    listOfValues.AddRange(new List<string> { Math.Round(totalDaily.VITB6A.quantity, 2).ToString(), totalDaily.VITB6A.unit });
                }
                else
                {
                    listOfValues.AddRange(new List<string> { String.Empty, String.Empty });
                }

                if (totalDaily.VITD != null)
                {
                    listOfValues.AddRange(new List<string> { Math.Round(totalDaily.VITD.quantity, 2).ToString(), totalDaily.VITD.unit });
                }
                else
                {
                    listOfValues.AddRange(new List<string> { String.Empty, String.Empty });
                }

                if (totalDaily.TOCPHA != null)
                {
                    listOfValues.AddRange(new List<string> { Math.Round(totalDaily.TOCPHA.quantity, 2).ToString(), totalDaily.TOCPHA.unit });
                }
                else
                {
                    listOfValues.AddRange(new List<string> { String.Empty, String.Empty });
                }

                if (totalDaily.VITK1 != null)
                {
                    listOfValues.AddRange(new List<string> { Math.Round(totalDaily.VITK1.quantity, 2).ToString(), totalDaily.VITK1.unit });
                }
                else
                {
                    listOfValues.AddRange(new List<string> { String.Empty, String.Empty });
                }

                if (totalDaily.NIA != null)
                {
                    listOfValues.AddRange(new List<string> { Math.Round(totalDaily.NIA.quantity, 2).ToString(), totalDaily.NIA.unit });
                }
                else
                {
                    listOfValues.AddRange(new List<string> { String.Empty, String.Empty });
                }

                if (totalDaily.VITB12 != null)
                {
                    listOfValues.AddRange(new List<string> { Math.Round(totalDaily.VITB12.quantity, 2).ToString(), totalDaily.VITB12.unit });
                }
                else
                {
                    listOfValues.AddRange(new List<string> { String.Empty, String.Empty });
                }

                if (totalDaily.THIA != null)
                {
                    listOfValues.AddRange(new List<string> { Math.Round(totalDaily.THIA.quantity, 2).ToString(), totalDaily.THIA.unit });
                }
                else
                {
                    listOfValues.AddRange(new List<string> { String.Empty, String.Empty });
                }

                if (totalDaily.RIBF != null)
                {
                    listOfValues.AddRange(new List<string> { Math.Round(totalDaily.RIBF.quantity, 2).ToString(), totalDaily.RIBF.unit });
                }
                else
                {
                    listOfValues.AddRange(new List<string> { String.Empty, String.Empty });
                }

                if (totalDaily.FOLDFE != null)
                {
                    listOfValues.AddRange(new List<string> { Math.Round(totalDaily.FOLDFE.quantity, 2).ToString(), totalDaily.FOLDFE.unit });
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
