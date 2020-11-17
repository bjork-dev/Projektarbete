using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Butik
{
    public class CodeDiscount
    {
        private string code;
        private int discount;

        public string Code
        {
            get => code;
            set
            {
                if (value.Length < 3 || value.Length > 20)
                    throw new Exception("Code must be between 3 and 20 characters long.");

                char[] letters = value.ToCharArray();
                var wrongSymbols = letters.Where(l => !char.IsLetterOrDigit(l));
                if (wrongSymbols.Any()) throw new Exception("Code must consist only of letters and numbers.");

                code = value;
            }
        }

        public int Discount
        {
            get => discount;
            set
            {
                if (value > 100 || value < 1)
                {
                    throw new Exception("Discount must be an integer from 1 to 100.");
                }

                discount = value;
            }
        }
    }
    public class Store
    {
        private string name;
        private string description;
        private string imageName;
        private decimal price;

        public string Name
        {
            get => name;
            set
            {
                if (value == string.Empty)
                {
                    throw new Exception("Name cannot be null");
                }

                name = value;
            }
        }

        public decimal Price
        {
            get => price;
            set
            {
                if (value <= 0)
                {
                    throw new Exception("Price cannot be 0 or less");
                }

                price = Math.Round(value, 2);
            }
        }

        public string Description
        {
            get => description;
            set
            {
                if (value == string.Empty)
                {
                    throw new Exception("Description cannot be null");

                }

                description = value;
            }
        }

        public string ImageName
        {
            get => imageName;
            set
            {
                if (imageName == string.Empty)
                {
                    throw new Exception("Image name cannot be null");
                }

                imageName = value;
            }
        }

    }
    public partial class MainWindow : Window
    {
        //Global variables
        private const string SavedCartPath = @"C:\Windows\Temp\cart.csv";
        private const string Path = @"C:\Windows\Temp\store.csv";
        public const string CouponLocalPath = "Coupon.csv";
        public const string CouponGlobalPath = @"C:\Windows\Temp\" + CouponLocalPath;
        private readonly ListBox CartBody = new ListBox { Margin = new Thickness(5), MaxHeight = 335, MinHeight = 100 };
        internal decimal sumTotal;
        internal decimal sumWithoutDiscount;
        private List<string> savedItemsList = new List<string>(); //Stores the cart items
        private TextBox couponBox;
        private List<CodeDiscount> discountsList = new List<CodeDiscount>(); // List with valid codes and discounts (string "code", int discount)
        private int discount;

        // Global textblock for the total price, text changed dynamically by event handler
        private TextBlock totalPrice = new TextBlock
        {
            Text = "Total price: 0 kr",
            FontFamily = new FontFamily("Constantia"),
            TextWrapping = TextWrapping.Wrap,
            Margin = new Thickness(5),
            FontSize = 15
        };

        public MainWindow()
        {
            InitializeComponent();
            Start();
        }

        private void Start()
        {

            // Window options
            Title = "Store";
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
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            //Title text
            TextBlock titleText = new TextBlock
            {
                Text = "Store",
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(5),
                FontFamily = new FontFamily("Constantia"),
                FontSize = 25,
                TextAlignment = TextAlignment.Center
            };
            grid.Children.Add(titleText);
            Grid.SetColumn(titleText, 0);
            Grid.SetRow(titleText, 0);

            //Cart heading
            TextBlock cartText = new TextBlock
            {
                Text = "Cart",
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(5),
                FontFamily = new FontFamily("Constantia"),
                FontSize = 25,
                TextAlignment = TextAlignment.Center
            };
            grid.Children.Add(cartText);
            Grid.SetColumn(cartText, 1);
            Grid.SetRow(cartText, 0);

            // Create the Cart panel
            Grid rightPanel = CreateCartPanel();
            grid.Children.Add(rightPanel);
            Grid.SetRow(rightPanel, 1);
            Grid.SetColumn(rightPanel, 1);

            //Create store panel
            WrapPanel leftPanel = StorePanel();
            grid.Children.Add(leftPanel);
            Grid.SetRow(leftPanel, 1);
            Grid.SetColumn(leftPanel, 0);

            if (!File.Exists(CouponGlobalPath)) //Copy coupon file from project if it does not exist in Temp
            {
                File.Copy("Coupon.csv", CouponGlobalPath);
            }

            LoadDiscounts(discountsList, CouponGlobalPath); // Load saved discounts   
        }

        private Grid CreateCartPanel()  //Right side of the application
        {
            // Nested grid for the part Cart
            Grid cartGrid = new Grid
            {
                Margin = new Thickness(5)
            };
            cartGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            cartGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            cartGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            cartGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            cartGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            cartGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            cartGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            cartGrid.ColumnDefinitions.Add(new ColumnDefinition());
            cartGrid.ColumnDefinitions.Add(new ColumnDefinition());
            Grid.SetColumn(cartGrid, 3);
            Grid.SetRow(cartGrid, 1);

            // Cart list
            cartGrid.Children.Add(CartBody);
            Grid.SetColumn(CartBody, 0);
            Grid.SetRow(CartBody, 0);
            Grid.SetColumnSpan(CartBody, 2);

            if (File.Exists(SavedCartPath))
            {
                foreach (var item in File.ReadAllLines(SavedCartPath).Select(a => a.Split(',')))
                {
                    string name = item[0];
                    string price = item[1].Trim(' ');
                    CartBody.Items.Add($"{name} ({price}kr)");
                    savedItemsList.Add(name + ", " + price);
                    sumWithoutDiscount += decimal.Parse(price);
                }
                totalPrice.Text = ShowTotalPrice();
            }

            // Remove button
            Button remove = new Button
            {
                Content = "remove item",
                Margin = new Thickness(5),
                Padding = new Thickness(5),
                FontFamily = new FontFamily("Constantia"),
                FontSize = 15,
                ToolTip = "Select a product in the cart and click remove"
            };
            cartGrid.Children.Add(remove);
            Grid.SetColumn(remove, 0);
            Grid.SetRow(remove, 1);
            remove.Click += RemoveFromCartOnClick;

            // Remove All button
            Button removeAll = new Button
            {
                Content = "remove all",
                Margin = new Thickness(5),
                Padding = new Thickness(5),
                FontFamily = new FontFamily("Constantia"),
                FontSize = 15
            };
            cartGrid.Children.Add(removeAll);
            Grid.SetColumn(removeAll, 1);
            Grid.SetRow(removeAll, 1);
            removeAll.Click += RemoveAllCartOnClick;

            // Enter coupon
            Label couponLabel = new Label
            {
                Content = "Enter coupon:",
                Margin = new Thickness(5),
                FontFamily = new FontFamily("Constantia"),
                HorizontalContentAlignment = HorizontalAlignment.Right,
                FontSize = 15
            };
            cartGrid.Children.Add(couponLabel);
            Grid.SetColumn(couponLabel, 0);
            Grid.SetRow(couponLabel, 2);

            couponBox = new TextBox
            {
                VerticalContentAlignment = VerticalAlignment.Center,
                Margin = new Thickness(5),
                MaxLength = 20
            };
            cartGrid.Children.Add(couponBox);
            Grid.SetColumn(couponBox, 1);
            Grid.SetRow(couponBox, 2);
            couponBox.KeyDown += CouponBox_KeyDown;

            cartGrid.Children.Add(totalPrice);
            Grid.SetColumn(totalPrice, 0);
            Grid.SetRow(totalPrice, 3);
            Grid.SetColumnSpan(totalPrice, 2);

            //Save cart button
            Button saveBtn = new Button
            {
                Content = "Save cart",
                Margin = new Thickness(5),
                Padding = new Thickness(5),
                FontFamily = new FontFamily("Constantia"),
                FontSize = 20
            };
            cartGrid.Children.Add(saveBtn);
            Grid.SetColumn(saveBtn, 0);
            Grid.SetRow(saveBtn, 4);
            saveBtn.Click += SaveCart;

            // Button checkout
            Button checkout = new Button
            {
                Content = "Checkout",
                Margin = new Thickness(5),
                Padding = new Thickness(5),
                FontFamily = new FontFamily("Constantia"),
                FontSize = 20
            };
            cartGrid.Children.Add(checkout);
            Grid.SetColumn(checkout, 1);
            Grid.SetRow(checkout, 4);
            checkout.Click += CheckoutOnClick;

            return cartGrid;
        }

        private WrapPanel StorePanel() //Left side of the application
        {
            var p = new Store();

            WrapPanel wrapPanel = new WrapPanel
            {
                Margin = new Thickness(5)
            };
            if (!File.Exists(Path)) //If no store.csv file exists in the temp folder, copy the file from project to temp
            {
                File.Copy("store.csv", Path);
            }

            foreach (var item in File.ReadAllLines(Path).Select(a => a.Split(','))) //Reads csv in order: name, price, description, image name
            {
                try
                {
                    // for each product creates nested grid with image, name, description and button "buy (price)"  
                    Grid itemGrid = new Grid
                    {
                        Margin = new Thickness(5, 25, 5, 25)
                    };
                    itemGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                    itemGrid.RowDefinitions.Add(new RowDefinition());
                    itemGrid.RowDefinitions.Add(new RowDefinition());
                    itemGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                    p.Name = item[0]; //Assign each delimted item to an attribute
                    p.Price = decimal.Parse(item[1]);
                    p.Description = item[2];
                    p.ImageName = item[3];

                    ImageSource source =
                        new BitmapImage(new Uri(@"Images\" + p.ImageName.Trim(' '),
                            UriKind.Relative)); //Creates an image from the image folder + image name
                    Image image = new Image
                    {
                        Source = source,
                        Width = 100,
                        Height = 100,
                        Stretch = Stretch.UniformToFill,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(5)
                    };

                    RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.HighQuality);
                    itemGrid.Children.Add(image);
                    Grid.SetRow(image, 0);

                    TextBlock titleTextBlock = new TextBlock  //Create a title textBox from the store item's name
                    {
                        Text = p.Name,
                        Margin = new Thickness(5),
                        FontSize = 18,
                        Width = 150,
                        TextWrapping = TextWrapping.Wrap,
                        TextAlignment = TextAlignment.Center,

                    };
                    itemGrid.Children.Add(titleTextBlock);
                    Grid.SetRow(titleTextBlock, 1);

                    TextBlock descriptionTextBlock = new TextBlock
                    {
                        Text = p.Description.Trim(' '),
                        Margin = new Thickness(5),
                        FontSize = 14,
                        Width = 150,
                        TextAlignment = TextAlignment.Center,
                        TextWrapping = TextWrapping.Wrap,
                    };

                    itemGrid.Children.Add(descriptionTextBlock);
                    Grid.SetRow(descriptionTextBlock, 2);

                    var button = new Button //The buy button, fetches price attribute from store item
                    {
                        Content = $"Buy ({p.Price}kr)",
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Width = 100,
                        Height = 40,
                        Tag = p.Name,
                        DataContext = p.Price,
                        VerticalAlignment = VerticalAlignment.Center,
                    };
                    itemGrid.Children.Add(button);
                    Grid.SetRow(button, 3);
                    button.Click += AddToCartOnClick;

                    wrapPanel.Children.Add(itemGrid);

                }
                catch (Exception)
                {
                    if (p.Name == null || p.Price == 0 || p.Description == null || p.ImageName == null)
                    {
                        continue; //Skips the iteration, if one of the attributes are missing, instead of crashing.
                    }
                    MessageBox.Show("Alert!\n\nOne or more products could not be added due to missing attributes in csv file");
                }
            }
            return wrapPanel;
        }

        // Load saved discounts from a file at CouponPath. If it doesn´t exist, then load discounts from the local file Coupons.scv   
        public static void LoadDiscounts(List<CodeDiscount> discountsList, string path,
            ObservableCollection<string> discountsShow = null)
        {
            var lines = File.ReadAllLines(path).Select(a => a.Split(','));
            foreach (var item in lines)
            {
                try // if code or discount is incorrect, skip it
                {
                    int discount = int.Parse(item[1]);
                    string code = item[0].ToLower();

                    if (IsDuplicate(code, discountsList)) continue;
                    try
                    {
                        discountsList.Add(new CodeDiscount { Code = code, Discount = discount });
                        discountsShow?.Add(code + "   " + discount + " %");
                    }
                    catch
                    {
                        // ignored
                    }
                }
                catch
                {
                    // ignored
                }
            }
        }

        // Check whether this code is already in the list
        public static bool IsDuplicate(string code, List<CodeDiscount> list)
        {
            var duplication = list.Where(l => l.Code == code);
            return duplication.Any();
        }

        //Adds the name of the product to the cart using button tag and updates the total price by assigning the price variable to datacontext
        private void AddToCartOnClick(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            CartBody.Items.Add($"{button.Tag} ({button.DataContext}kr)");
            string savedItem = $"{button.Tag}, {button.DataContext}";
            savedItemsList.Add(savedItem);
            sumWithoutDiscount += (decimal)button.DataContext;
            totalPrice.Text = ShowTotalPrice();
        }
        // Adds item to reciept and displays it to the user, then clears the lists that previously contained the cart items and resets the total sum and discount.
        private void CheckoutOnClick(object sender, RoutedEventArgs e)
        {
            var checkoutList = (from object item in CartBody.Items select item.ToString()).ToList();
            var items = savedItemsList.OrderByDescending(p => p);
            Button button = (Button)sender;
            if (sumWithoutDiscount == 0)
            {
                MessageBox.Show("You did not buy anything");
            }
            else
            {
                MessageBox.Show("Thank you for your purchase! \n\nReceipt: \n\n" + string.Join('\n', checkoutList) +
                                "\n\n" + ShowTotalPrice());

                CartBody.Items.Clear();
                savedItemsList.Clear();
                sumWithoutDiscount = 0;
                discount = 0;
                couponBox.IsEnabled = true;
                couponBox.Clear();
                totalPrice.Text = ShowTotalPrice();
            }
        }

        private void RemoveAllCartOnClick(object sender, RoutedEventArgs e) //Gets the selected index in the listbox and then removes the item connected.
        {
            const string message = "Would you like to remove all items from the cart?";
            var result = MessageBox.Show(message, "Remove all", MessageBoxButton.YesNo);
            if (result != MessageBoxResult.Yes) return;

            CartBody.Items.Clear();
            sumWithoutDiscount = 0;
            discount = 0;
            totalPrice.Text = ShowTotalPrice();
            couponBox.IsEnabled = true;
            couponBox.Clear();
            savedItemsList.Clear();
        }

        private void RemoveFromCartOnClick(object sender, RoutedEventArgs e)
        {
            int indexToRemove = CartBody.SelectedIndex;
            if (indexToRemove == -1)
            {
                MessageBox.Show("First select a product in the cart and then click the button \"remove\"");
            }
            else
            {
                string itemToRemove = (string)CartBody.SelectedItem;
                int indexStar = itemToRemove.LastIndexOf('(');
                int indexEnd = itemToRemove.LastIndexOf("kr)");
                string sumTrim = itemToRemove.Substring(indexStar + 1, indexEnd - indexStar - 1);
                decimal sumToRemove = decimal.Parse(sumTrim);
                UpdateSum(sumToRemove);
                totalPrice.Text = ShowTotalPrice();
                CartBody.Items.RemoveAt(indexToRemove);
                savedItemsList.RemoveAt(indexToRemove);
            }
        }

        private void SaveCart(object sender, RoutedEventArgs e) //Saves added cart items to cart.csv in temp folder, creates the file if it does not exist.
        {
            if (sumWithoutDiscount == 0)
            {
                MessageBox.Show("Cannot save an empty cart.");
            }
            else
            {
                File.WriteAllLines(SavedCartPath, savedItemsList);
                MessageBox.Show("Cart saved");
            }
        }

        // Check whether the coupon gives a discount
        private void CouponBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string code = couponBox.Text.ToLower();
                var validCodeDiscount = discountsList.FirstOrDefault(c => c.Code == code);

                if (validCodeDiscount != null)
                {
                    couponBox.IsEnabled = false;
                    discount = validCodeDiscount.Discount;
                    totalPrice.Text = ShowTotalPrice();
                }
                else
                {
                    MessageBox.Show("Coupon is not valid :(");
                    couponBox.Clear();
                }
            }
        }

        // Create a string with the total price
        private string ShowTotalPrice()
        {
            UpdateSum();
            return (discount == 0) ? "Total price: " + sumTotal + " kr" : "Total price with " + discount + "% discount: " + sumTotal + " kr";
        }

        // Calculate the price with discount. If a product was removed from the cart the method update the total price.
        private void UpdateSum(decimal subtrahend = 0)
        {
            double multiplier = (100.0 - discount) / 100.0;
            sumWithoutDiscount -= subtrahend;
            sumTotal = Math.Round(Convert.ToDecimal(multiplier) * sumWithoutDiscount, 2);
        }
    }
}