using System;
using System.Collections.Generic;
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

namespace Butik
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Start();
        }

        private void Start()
        {
            // Window options
            Title = "Store";
            Width = 1000;
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

            ////to be changed

            ////Item 1 label
            //Label itemLabel = new Label
            //{
            //    Content = "test1",
            //    Margin = new Thickness(5),
            //    FontFamily = new FontFamily("Constantia"),
            //    FontSize = 25,
            //    HorizontalContentAlignment = HorizontalAlignment.Center
            //};
            //grid.Children.Add(itemLabel);
            //Grid.SetColumn(itemLabel, 0);
            //Grid.SetRow(itemLabel, 1);

            ////Item 1 label
            //Label itemLabel2 = new Label
            //{
            //    Content = "test2",
            //    Margin = new Thickness(5),
            //    FontFamily = new FontFamily("Constantia"),
            //    FontSize = 25,
            //    HorizontalContentAlignment = HorizontalAlignment.Center
            //};
            //grid.Children.Add(itemLabel2);
            //Grid.SetColumn(itemLabel2, 0);
            //Grid.SetRow(itemLabel2, 2);
            //Label itemLabel3 = new Label
            //{
            //    Content = "test3",
            //    Margin = new Thickness(5),
            //    FontFamily = new FontFamily("Constantia"),
            //    FontSize = 25,
            //    HorizontalContentAlignment = HorizontalAlignment.Center

            //};
            //grid.Children.Add(itemLabel3);
            //Grid.SetColumn(itemLabel3, 1);
            //Grid.SetRow(itemLabel3, 1);

            ////Item 1 label
            //Label itemLabel4 = new Label
            //{
            //    Content = "test4",
            //    Margin = new Thickness(5),
            //    FontFamily = new FontFamily("Constantia"),
            //    FontSize = 25,
            //    HorizontalContentAlignment = HorizontalAlignment.Center
            //};
            //grid.Children.Add(itemLabel4);
            //Grid.SetColumn(itemLabel4, 1);
            //Grid.SetRow(itemLabel4, 2);

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
            ListBox cartBody = new ListBox { Margin = new Thickness(5) };
            cartBody.Items.Add("item 1, price");
            cartBody.Items.Add("item 2, price");
            cartBody.Items.Add("item 3, price");
            cartBody.SelectedIndex = 0;
            cartGrid.Children.Add(cartBody);
            Grid.SetColumn(cartBody, 0);
            Grid.SetRow(cartBody, 0);
            Grid.SetColumnSpan(cartBody, 2);

            // Remove button
            Button remove = new Button
            {
                Content = "remove item",
                Margin = new Thickness(5),
                Padding = new Thickness(5),
                FontFamily = new FontFamily("Constantia"),
                FontSize = 15
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

            // Total price
            TextBlock totalPrice = new TextBlock
            {
                Text = "Total price: 0 kr",
                FontFamily = new FontFamily("Constantia"),
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(5),
                FontSize = 15
            };
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

            return cartGrid;
        }
    }
}
