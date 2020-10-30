﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Butik
{
    internal class Item
    {
        public string Name;
        public decimal Price;
        public string Description;
        public string ImageName;
    }

    public partial class MainWindow : Window
    {
        private readonly List<Item> itemList = new List<Item>();
        private const string Path = @"C:\Windows\Temp\store.csv";
        public readonly ListBox CartBody = new ListBox { Margin = new Thickness(5) };
        internal decimal Sum;

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

            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });
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
        }

        private Grid CreateCartPanel()
        {
            // Nested grid for the part Cart
            Grid cartGrid = new Grid
            {
                Margin = new Thickness(5)
            };

            cartGrid.RowDefinitions.Add(new RowDefinition());
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

            TextBox couponBox = new TextBox
            {
                VerticalContentAlignment = VerticalAlignment.Center,
                Margin = new Thickness(5)
            };
            cartGrid.Children.Add(couponBox);
            Grid.SetColumn(couponBox, 1);
            Grid.SetRow(couponBox, 2);

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

        private WrapPanel StorePanel()
        {
            var p = new Item();

            WrapPanel wrapPanel = new WrapPanel
            {
                Margin = new Thickness(5)
            };

            try
            {
                var test = File.ReadAllLines(Path).Select(a => a.Split(','));
            }
            catch (Exception e)
            {
                MessageBox.Show("Cannot locate store file\nDetails: " + e.Message);
                Environment.Exit(0);
            }
            var lines = File.ReadAllLines(Path).Select(a => a.Split(','));

            foreach (var item in lines) //Reads csv in order: name, price, description, image name
            {
                try
                {
                    p.Name = item[0];
                    p.Price = decimal.Parse(item[1]);
                    p.Description = item[2];
                    p.ImageName = item[3];
                    itemList.Add(p);

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
                    wrapPanel.Children.Add(image);
                    //Product title
                    Label titleLabel = new Label
                    {
                        Content = p.Name + "\n\n" + p.Description.Trim(' '),
                        Margin = new Thickness(5),
                        FontFamily = new FontFamily("Constantia"),
                        FontSize = 18,
                        HorizontalContentAlignment = HorizontalAlignment.Left
                    };
                    wrapPanel.Children.Add(titleLabel);

                    var button = new Button
                    {
                        Content = $"Buy ({p.Price}kr)",
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        Tag = p.Name,
                        DataContext = p.Price
                    };
                    button.Click += AddToCartOnClick;
                    wrapPanel.Children.Add(button);
                }
                catch (Exception)
                {
                    if (p.Name == null || p.Price == 0 || p.Description == null || p.ImageName == null)
                    {
                        continue; //Skips the iteration, if one of the attributes are missing.
                    }
                    MessageBox.Show("Alert!\n\nOne or more products could not be added due to missing attributes in csv file");
                }
            }
            return wrapPanel;
        }

        //Adds the name of the product to the cart using button tag and updates the total price by assigning the price variable to datacontext
        private void AddToCartOnClick(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            CartBody.Items.Add($"{button.Tag} ({button.DataContext} kr)");
            Sum += (decimal)button.DataContext;
            totalPrice.Text = "Total price: " + Sum + " kr";
        }

        private void CheckoutOnClick(object sender, RoutedEventArgs e)
        {
            List<string> itemsList = new List<string>();
            foreach (var item in CartBody.Items)
            {
                itemsList.Add(item.ToString());
            }

            var items = itemsList.OrderByDescending(p => p);
            Button button = (Button)sender;
            if (Sum == 0)
            {
                MessageBox.Show("You did not buy anything");
            }
            else
            {
                MessageBox.Show("Thank you for your purchase! \n\nReceipt: \n\n" + string.Join('\n', items) +
                                "\n\nTotal price: " + Sum + "kr");
                CartBody.Items.Clear();
                Sum = 0;
                totalPrice.Text = "Total price: " + Sum + "kr";
            }
        }
        private void RemoveAllCartOnClick(object sender, RoutedEventArgs e)
        {
            string message = "Would you like to remove all items from the cart?";
            string caption = "Remove all";
            var result = MessageBox.Show(message, caption, MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                CartBody.Items.Clear();
                Sum = 0;
                totalPrice.Text = "Total price: " + Sum + " kr";
            }
        }
    }
}