using Butik;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace Butik_Creator
{
    public class CodeDiscount
    {
        public string Code;
        public int Discount;
    }

    public class Store
    {
        public string Name;
        public decimal Price;
        public string Description;
        public string ImageName;
    }

    public partial class MainWindow : Window
    {
        private const string Path = @"C:\Windows\Temp\store.csv";
        public const string CouponPath = @"C:\Windows\Temp\Coupon.csv";

        private List<CodeDiscount>
            discountsList =
                new List<CodeDiscount>(); // List with valid codes and discounts (string "code", int discount)

        private ObservableCollection<string>
            discountsShow =
                new ObservableCollection<string>(); // List for displaying valid codes and discounts ("code   discount %")

        TextBox codeTextBox;
        TextBox discountTextBox;
        ListBox discountListBox;
        Button addButton, discardButton, saveChangesButton;
        private Dictionary<string, int> coupons;

        ListBox assortmentListBox = new ListBox
        {
            Margin = new Thickness(5),
            //   MaxHeight = 300
        };

        TextBox nameTextBox = new TextBox
        {
            Margin = new Thickness(5)
        };

        TextBox priceTextBox = new TextBox
        {
            Margin = new Thickness(5)
        };

        TextBox descriptionTextBox = new TextBox
        {
            Margin = new Thickness(5)
        };

        public MainWindow()
        {
            InitializeComponent();
            Start();
        }

        private void Start()
        {
            // Window options
            Title = "Store management";
            Width = 1200;
            Height = 600;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            // Scrolling
            ScrollViewer root = new ScrollViewer();
            root.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            Content = root;

            // Main grid
            Grid grid = new Grid();
            root.Content = grid;
            grid.Margin = new Thickness(5);
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition());

            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });

            //Title Assortment
            TextBlock assortmentHeading = new TextBlock
            {
                Text = "Assortment",
                Margin = new Thickness(5),
                FontSize = 25,
                TextAlignment = TextAlignment.Center
            };
            grid.Children.Add(assortmentHeading);
            Grid.SetColumn(assortmentHeading, 0);
            Grid.SetRow(assortmentHeading, 0);

            //Title Discounts
            TextBlock discountHeading = new TextBlock
            {
                Text = "Discounts",
                Margin = new Thickness(5),
                FontSize = 25,
                TextAlignment = TextAlignment.Center
            };
            grid.Children.Add(discountHeading);
            Grid.SetColumn(discountHeading, 1);
            Grid.SetRow(discountHeading, 0);

            // Create Assortment Panel
            Grid assortmentPanel = CreateAssortmentPanel();
            grid.Children.Add(assortmentPanel);
            Grid.SetRow(assortmentPanel, 1);
            Grid.SetColumn(assortmentPanel, 0);

            // Create Discount Panel
            Grid discountPanel = CreateDiscountPanel();
            grid.Children.Add(discountPanel);
            Grid.SetRow(discountPanel, 1);
            Grid.SetColumn(discountPanel, 1);

            LoadDiscounts(); // Load saved discounts from a file CouponPath (if it exists)
            LoadStore(); // Load saved store from a file store (if it exists)
        }

        private Grid CreateAssortmentPanel()
        {
            // Nested grid for the part Assortment
            Grid assortmentGrid = new Grid
            {
                Margin = new Thickness(5)
            };
            assortmentGrid.RowDefinitions.Add(new RowDefinition());
            assortmentGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            assortmentGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            assortmentGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            assortmentGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            assortmentGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) });


            assortmentGrid.Children.Add(assortmentListBox);
            Grid.SetColumn(assortmentListBox, 0);
            Grid.SetRow(assortmentListBox, 0);

            Button assortRemoveAll = new Button
            {
                Content = "Remove all",
                Margin = new Thickness(5),
                FontSize = 12,
                Padding = new Thickness(5)
            };
            assortmentGrid.Children.Add(assortRemoveAll);
            Grid.SetColumn(assortRemoveAll, 0);
            Grid.SetRow(assortRemoveAll, 1);
            assortRemoveAll.Click += RemoveAll;

            Label galleryLabel = new Label
            {
                Content = "Image Gallery",
                FontSize = 15,
                Margin = new Thickness(5),
                HorizontalContentAlignment = HorizontalAlignment.Center
            };
            assortmentGrid.Children.Add(galleryLabel);
            Grid.SetColumn(galleryLabel, 0);
            Grid.SetRow(galleryLabel, 2);
            Grid.SetColumnSpan(galleryLabel, 2);

            WrapPanel bildGallery = new WrapPanel { Orientation = Orientation.Horizontal };
            assortmentGrid.Children.Add(bildGallery);
            Grid.SetColumn(bildGallery, 0);
            Grid.SetRow(bildGallery, 3);
            Grid.SetColumnSpan(bildGallery, 2);

            for (int i = 0; i < 5; i++)
            {
                Image testImage = new Image
                {
                    Source = new BitmapImage(new Uri(@"Images\lundgrens.jpg", UriKind.Relative)),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(5),
                    Width = 80,
                    Height = 80,
                    Stretch = Stretch.Uniform
                };
                RenderOptions.SetBitmapScalingMode(testImage, BitmapScalingMode.HighQuality);
                bildGallery.Children.Add(testImage);

                Image testImage3 = new Image
                {
                    Source = new BitmapImage(new Uri(@"Images\siberia.png", UriKind.Relative)),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(5),
                    Width = 80,
                    Height = 80,
                    Stretch = Stretch.Uniform
                };
                RenderOptions.SetBitmapScalingMode(testImage3, BitmapScalingMode.HighQuality);
                bildGallery.Children.Add(testImage3);
            }

            Grid nestedGrid = new Grid
            {
                Margin = new Thickness(5)
            };

            nestedGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            nestedGrid.ColumnDefinitions.Add(new ColumnDefinition());
            nestedGrid.ColumnDefinitions.Add(new ColumnDefinition());
            nestedGrid.ColumnDefinitions.Add(new ColumnDefinition());
            nestedGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            nestedGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            nestedGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            nestedGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            nestedGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            assortmentGrid.Children.Add(nestedGrid);
            Grid.SetColumn(nestedGrid, 1);
            Grid.SetRow(nestedGrid, 0);

            Label nameLabel = new Label
            {
                Content = "Name",
                Margin = new Thickness(5),
                HorizontalContentAlignment = HorizontalAlignment.Right
            };
            nestedGrid.Children.Add(nameLabel);
            Grid.SetColumn(nameLabel, 0);
            Grid.SetRow(nameLabel, 0);


            nestedGrid.Children.Add(nameTextBox);
            Grid.SetColumn(nameTextBox, 1);
            Grid.SetRow(nameTextBox, 0);
            Grid.SetColumnSpan(nameTextBox, 2);

            Label descriptionLabel = new Label
            {
                Content = "Description",
                Margin = new Thickness(5),
                HorizontalContentAlignment = HorizontalAlignment.Right
            };
            nestedGrid.Children.Add(descriptionLabel);
            Grid.SetColumn(descriptionLabel, 0);
            Grid.SetRow(descriptionLabel, 1);

            nestedGrid.Children.Add(descriptionTextBox);
            Grid.SetColumn(descriptionTextBox, 1);
            Grid.SetRow(descriptionTextBox, 1);
            Grid.SetColumnSpan(descriptionTextBox, 2);

            Label priceLabel = new Label
            {
                Content = "Price",
                Margin = new Thickness(5),
                HorizontalContentAlignment = HorizontalAlignment.Right
            };
            nestedGrid.Children.Add(priceLabel);
            Grid.SetColumn(priceLabel, 0);
            Grid.SetRow(priceLabel, 2);


            nestedGrid.Children.Add(priceTextBox);
            Grid.SetColumn(priceTextBox, 1);
            Grid.SetRow(priceTextBox, 2);
            Grid.SetColumnSpan(priceTextBox, 2);

            Image testImage2 = new Image
            {
                Source = new BitmapImage(new Uri(@"Images\lundgrens.jpg", UriKind.Relative)),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(5),
                Width = 80,
                Height = 80,
                Stretch = Stretch.Uniform
            };
            RenderOptions.SetBitmapScalingMode(testImage2, BitmapScalingMode.HighQuality);
            nestedGrid.Children.Add(testImage2);
            Grid.SetColumn(testImage2, 3);
            Grid.SetRow(testImage2, 0);
            Grid.SetRowSpan(testImage2, 3);

            Button saveChangesButton = new Button
            {
                Content = "Save changes",
                Margin = new Thickness(5),
                FontSize = 12,
                Padding = new Thickness(5),
                IsEnabled = false
            };
            Grid.SetRow(saveChangesButton, 3);
            Grid.SetColumn(saveChangesButton, 1);
            nestedGrid.Children.Add(saveChangesButton);

            Button removeButton = new Button
            {
                Content = "Remove",
                Margin = new Thickness(5),
                FontSize = 12,
                Padding = new Thickness(5),
                IsEnabled = false
            };
            Grid.SetRow(removeButton, 3);
            Grid.SetColumn(removeButton, 2);
            nestedGrid.Children.Add(removeButton);

            Button addButton = new Button
            {
                Content = "Add new",
                Margin = new Thickness(5),
                FontSize = 12,
                Padding = new Thickness(5),
                IsEnabled = true
            };
            Grid.SetRow(addButton, 4);
            Grid.SetColumn(addButton, 1);
            nestedGrid.Children.Add(addButton);
            Grid.SetColumnSpan(addButton, 2);
            addButton.Click += AddAssortment;

            Button saveButton = new Button
            {
                Content = "Save to file",
                Margin = new Thickness(90, 5, 150, 5),
                FontSize = 12,
                Padding = new Thickness(5),
            };
            Grid.SetRow(saveButton, 1);
            Grid.SetColumn(saveButton, 2);
            assortmentGrid.Children.Add(saveButton);


            return assortmentGrid;
        }

        private Grid CreateDiscountPanel()
        {
            // Nested grid for the part Discount
            Grid discountGrid = new Grid
            {
                Margin = new Thickness(5)
            };
            discountGrid.ColumnDefinitions.Add(new ColumnDefinition());
            discountGrid.ColumnDefinitions.Add(new ColumnDefinition());
            discountGrid.RowDefinitions.Add(new RowDefinition());
            discountGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            discountListBox = new ListBox
            {
                Margin = new Thickness(5),
                MaxHeight = 500,
            };
            discountGrid.Children.Add(discountListBox);
            Grid.SetColumn(discountListBox, 0);
            Grid.SetRow(discountListBox, 0);
            discountListBox.ItemsSource = discountsShow;
            discountListBox.SelectionChanged += DiscountListBox_SelectionChanged;


            // Nested grid for buttons Remove and RemoveAll
            Grid removeButtonGrid = new Grid { Margin = new Thickness(5) };
            removeButtonGrid.ColumnDefinitions.Add(new ColumnDefinition());
            removeButtonGrid.ColumnDefinitions.Add(new ColumnDefinition());
            discountGrid.Children.Add(removeButtonGrid);
            Grid.SetRow(removeButtonGrid, 1);
            Grid.SetColumn(removeButtonGrid, 0);

            // Nested grid for textBoxes Code, Discount, buttons SaveChanges, Discart, AddNew
            Grid addDiscountGrid = new Grid { Margin = new Thickness(5) };
            addDiscountGrid.ColumnDefinitions.Add(new ColumnDefinition());
            addDiscountGrid.ColumnDefinitions.Add(new ColumnDefinition());
            addDiscountGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            addDiscountGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            addDiscountGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            addDiscountGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            addDiscountGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            discountGrid.Children.Add(addDiscountGrid);
            Grid.SetRow(addDiscountGrid, 0);
            Grid.SetColumn(addDiscountGrid, 1);

            Button removeButton = new Button
            {
                Content = "Remove",
                Margin = new Thickness(5),
                FontSize = 12,
                Padding = new Thickness(5)
            };
            removeButtonGrid.Children.Add(removeButton);
            Grid.SetColumn(removeButton, 0);
            removeButton.Click += RemoveButton_Click;

            Button removeAllButton = new Button
            {
                Content = "Remove all",
                Margin = new Thickness(5),
                FontSize = 12,
                Padding = new Thickness(5)
            };
            removeButtonGrid.Children.Add(removeAllButton);
            Grid.SetColumn(removeAllButton, 1);
            removeAllButton.Click += RemoveAllButton_Click;

            Label codeLabel = new Label
            {
                Content = "Code",
                Margin = new Thickness(5),
                HorizontalContentAlignment = HorizontalAlignment.Right
            };
            addDiscountGrid.Children.Add(codeLabel);
            Grid.SetColumn(codeLabel, 0);
            Grid.SetRow(codeLabel, 0);

            codeTextBox = new TextBox
            {
                Margin = new Thickness(5),
                VerticalAlignment = VerticalAlignment.Center,
                MaxLength = 20
            };
            addDiscountGrid.Children.Add(codeTextBox);
            Grid.SetColumn(codeTextBox, 1);
            Grid.SetRow(codeTextBox, 0);

            Label discountLabel = new Label
            {
                Content = "Discount (%)",
                Margin = new Thickness(5),
                HorizontalContentAlignment = HorizontalAlignment.Right
            };
            addDiscountGrid.Children.Add(discountLabel);
            Grid.SetColumn(discountLabel, 0);
            Grid.SetRow(discountLabel, 1);

            discountTextBox = new TextBox
            {
                Margin = new Thickness(5),
                VerticalAlignment = VerticalAlignment.Center
            };
            addDiscountGrid.Children.Add(discountTextBox);
            Grid.SetColumn(discountTextBox, 1);
            Grid.SetRow(discountTextBox, 1);

            addButton = new Button
            {
                Content = "Add new",
                Margin = new Thickness(5),
                FontSize = 12,
                Padding = new Thickness(5),
            };
            Grid.SetRow(addButton, 2);
            Grid.SetColumn(addButton, 0);
            addDiscountGrid.Children.Add(addButton);
            Grid.SetColumnSpan(addButton, 2);
            addButton.Click += AddCoupon;

            saveChangesButton = new Button
            {
                Content = "Save changes",
                Margin = new Thickness(5),
                FontSize = 12,
                Padding = new Thickness(5),
                IsEnabled = false
            };
            Grid.SetRow(saveChangesButton, 3);
            Grid.SetColumn(saveChangesButton, 0);
            addDiscountGrid.Children.Add(saveChangesButton);
            saveChangesButton.Click += SaveChangesButton_Click;

            discardButton = new Button
            {
                Content = "Discard",
                Margin = new Thickness(5),
                FontSize = 12,
                Padding = new Thickness(5),
                IsEnabled = false
            };
            Grid.SetRow(discardButton, 3);
            Grid.SetColumn(discardButton, 1);
            addDiscountGrid.Children.Add(discardButton);
            discardButton.Click += DiscardButtonClick;

            Button saveButton = new Button
            {
                Content = "Save to file",
                Margin = new Thickness(10),
                FontSize = 12,
                Padding = new Thickness(5),
            };
            Grid.SetRow(saveButton, 1);
            Grid.SetColumn(saveButton, 1);
            discountGrid.Children.Add(saveButton);
            saveButton.Click += SaveCoupon;

            return discountGrid;
        }

        // Remove selected code and discount
        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            int indexToRemove = discountListBox.SelectedIndex;
            if (indexToRemove == -1)
            {
                MessageBox.Show("First select a code and then click the button \"remove\"");
            }
            else
            {
                discountListBox.SelectedIndex = -1;
                discountsShow.RemoveAt(indexToRemove);
                discountsList.RemoveAt(indexToRemove);
                discountTextBox.Clear();
                codeTextBox.Clear();
                addButton.IsEnabled = true;
                saveChangesButton.IsEnabled = false;
                discardButton.IsEnabled = false;
            }
        }

        private void RemoveAllButton_Click(object sender, RoutedEventArgs e)
        {
            const string message = "Would you like to remove all discounts?";
            var result = MessageBox.Show(message, "Remove all", MessageBoxButton.YesNo);
            if (result != MessageBoxResult.Yes) return;
            discountsShow.Clear();
            discountsList.Clear();
        }

        //Save changes to the selected code
        private void SaveChangesButton_Click(object sender, RoutedEventArgs e)
        {
            int indexSelected = discountListBox.SelectedIndex;
            // Check that correct values are entered
            int discount = IsDiscountCorrect(discountTextBox.Text);
            int codeCheck = IsCodeCorrect(codeTextBox.Text);

            switch (codeCheck)
            {
                // 
                case 1 when discount > 0:
                    {
                        string code = codeTextBox.Text.ToLower();
                        List<CodeDiscount> copyDiscountsList = discountsList.Select(l => l).ToList();
                        copyDiscountsList.RemoveAt(indexSelected);
                        var duplication = copyDiscountsList.Where(l => l.Code == code);

                        if (!duplication.Any()) // check that there are no two identical codes
                        {
                            discountsList[indexSelected].Code = code;
                            discountsList[indexSelected].Discount = discount;
                            discountsShow[indexSelected] = code + "   " + discount + " %";
                            codeTextBox.Clear();
                            discountTextBox.Clear();
                            discountListBox.SelectedIndex = -1;
                            addButton.IsEnabled = true;
                            saveChangesButton.IsEnabled = false;
                            discardButton.IsEnabled = false;
                            discountListBox.SelectedIndex = -1;

                        }
                        else MessageBox.Show("This code already exists");

                        break;
                    }
                case -1:
                    MessageBox.Show("Code must be at least 3 characters long.");
                    break;
                case -2:
                    MessageBox.Show("Code must consist only of letters and numbers.");
                    break;
                default:
                    {
                        switch (discount)
                        {
                            case -1:
                                MessageBox.Show("Discount must be an integer.");
                                break;
                            case -2:
                                MessageBox.Show("Discount must be an integer from 1 to 100.");
                                break;
                        }

                        break;
                    }
            }
        }

        // Load saved discounts from a file CouponPath (if it exists)
        private void LoadDiscounts()
        {
            if (!File.Exists(CouponPath)) return;
            var lines = File.ReadAllLines(CouponPath).Select(a => a.Split(','));
            foreach (var item in lines)
            {
                if (IsCodeCorrect(item[0]) == 1 && IsDiscountCorrect(item[1]) > 0
                ) // check that values are correct otherwise skip them
                {
                    string code = item[0];
                    int discount = int.Parse(item[1]);
                    discountsList.Add(new CodeDiscount { Code = code, Discount = discount });
                    discountsShow.Add(code + "   " + discount + " %");
                }
            }
        }

        private void LoadStore()
        {
            var p = new Store();
            if (!File.Exists(Path)) return;
            foreach (var item in File.ReadAllLines(Path).Select(a => a.Split(','))
            ) //Reads csv in order: name, price, description, image name
            {
                p.Name = item[0];
                p.Price = decimal.Parse(item[1]);
                p.Description = item[2];
                p.ImageName = item[3];
                assortmentListBox.Items.Add($"{p.Name} {p.Price}kr");
            }
        }

        // Remove selection from ListBox
        private void DiscardButtonClick(object sender, RoutedEventArgs e)
        {
            discardButton.IsEnabled = false;
            saveChangesButton.IsEnabled = false;
            addButton.IsEnabled = true;
            discountTextBox.Clear();
            codeTextBox.Clear();
            discountListBox.SelectedIndex = -1;
        }

        // Show selected code and discount in the appropried TextBox
        private void DiscountListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (discountListBox.SelectedIndex != -1)
            {
                codeTextBox.Text = discountsList[discountListBox.SelectedIndex].Code;
                discountTextBox.Text = discountsList[discountListBox.SelectedIndex].Discount.ToString();
                addButton.IsEnabled = false;
                saveChangesButton.IsEnabled = true;
                discardButton.IsEnabled = true;
            }
        }

        // Add new code
        private void AddCoupon(object sender, RoutedEventArgs e)
        {
            // Check that correct values are entered
            int discount = IsDiscountCorrect(discountTextBox.Text);
            int codeCheck = IsCodeCorrect(codeTextBox.Text);

            switch (codeCheck)
            {
                // code and discount are correct
                case 1 when discount > 0:
                    {
                        string code = codeTextBox.Text.ToLower();
                        var duplication = discountsList.Where(l => l.Code == code);

                        if (!duplication.Any()) // check whether this code is already in the list
                        {
                            discountsList.Add(new CodeDiscount { Code = code, Discount = discount });
                            discountsShow.Add(code + "   " + discount + " %");
                            codeTextBox.Clear();
                            discountTextBox.Clear();
                        }
                        else MessageBox.Show("This code already exists");

                        break;
                    }
                case -1:
                    MessageBox.Show("Code must be at least 3 characters long.");
                    break;
                case -2:
                    MessageBox.Show("Code must consist only of letters and numbers.");
                    break;
                default:
                    switch (discount)
                    {
                        case -1:
                            MessageBox.Show("Discount must be an integer.");
                            break;
                        case -2:
                            MessageBox.Show("Discount must be an integer from 1 to 100.");
                            break;
                    }

                    break;
            }
        }

        // Check that entered core is correct. Return 1 if code is correct, otherwise return an error code. 
        private int IsCodeCorrect(string code)
        {
            if (code.Length < 3) return -1;

            char[] letters = code.ToCharArray();
            var wrongSymbols = letters.Where(l => !char.IsLetterOrDigit(l));

            if (wrongSymbols.Any()) return -2;

            return 1;
        }

        // Check that entered discount is correct. Return discount(int) if entered value is correct, otherwise return an error code. 
        private int IsDiscountCorrect(string discountToCheck)
        {
            int discount;
            try
            {
                discount = int.Parse(discountToCheck);
            }
            catch
            {
                return -1;
            }

            if (discount > 100 || discount < 1)
            {
                return -2;
            }

            return discount;
        }

        private void SaveCoupon(object sender, RoutedEventArgs e)
        {
            //string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Coupon.csv"); //temporary path for testing
            List<string> temp = discountsList.Select(code => code.Code + "," + code.Discount).ToList();
            File.WriteAllLines(CouponPath, temp);
            MessageBox.Show("File saved");
        }

        private void RemoveAll(object sender, RoutedEventArgs e)
        {
            if (assortmentListBox.Items.Count == 0) return;
            const string message = "Would you like to remove all items from the cart?";
            var result = MessageBox.Show(message, "Remove all", MessageBoxButton.YesNo);
            if (result != MessageBoxResult.Yes) return;
            assortmentListBox.Items.Clear();
            File.WriteAllText(Path, null);
        }

        private void AddAssortment(object sender, RoutedEventArgs e)
        {
            var s = new Store
            {
                Name = nameTextBox.Text,
                Price = decimal.Parse(priceTextBox.Text),
                Description = descriptionTextBox.Text
            };


            assortmentListBox.Items.Add($"{s.Name} {s.Price}kr");


        }


    }
}
