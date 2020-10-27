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
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());

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
            Grid.SetColumnSpan(titleText, 2);

            //to be changed

            //Item 1 label
            Label itemLabel = new Label
            {
                Content = "test1",
                Margin = new Thickness(5),
                FontFamily = new FontFamily("Constantia"),
                FontSize = 25,
                HorizontalContentAlignment = HorizontalAlignment.Center
            };
            grid.Children.Add(itemLabel);
            Grid.SetColumn(itemLabel, 0);
            Grid.SetRow(itemLabel, 1);

            //Item 1 label
            Label itemLabel2 = new Label
            {
                Content = "test2",
                Margin = new Thickness(5),
                FontFamily = new FontFamily("Constantia"),
                FontSize = 25,
                HorizontalContentAlignment = HorizontalAlignment.Center
            };
            grid.Children.Add(itemLabel2);
            Grid.SetColumn(itemLabel2, 0);
            Grid.SetRow(itemLabel2, 2);
            Label itemLabel3 = new Label
            {
                Content = "test3",
                Margin = new Thickness(5),
                FontFamily = new FontFamily("Constantia"),
                FontSize = 25,
                HorizontalContentAlignment = HorizontalAlignment.Center

            };
            grid.Children.Add(itemLabel3);
            Grid.SetColumn(itemLabel3, 1);
            Grid.SetRow(itemLabel3, 1);

            //Item 1 label
            Label itemLabel4 = new Label
            {
                Content = "test4",
                Margin = new Thickness(5),
                FontFamily = new FontFamily("Constantia"),
                FontSize = 25,
                HorizontalContentAlignment = HorizontalAlignment.Center
            };
            grid.Children.Add(itemLabel4);
            Grid.SetColumn(itemLabel4, 1);
            Grid.SetRow(itemLabel4, 2);


            //Cart text
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
            Grid.SetColumn(cartText, 3);
            Grid.SetRow(cartText, 0);
            Grid.SetColumnSpan(cartText, 2);


            //Save cart button
            Button saveBtn = new Button
            {
                Content = "Save cart",
                Margin = new Thickness(5),
                Padding = new Thickness(5),
                FontFamily = new FontFamily("Constantia"),
                FontSize = 20
            };
            grid.Children.Add(saveBtn);
            Grid.SetColumn(saveBtn, 0);
            Grid.SetRow(saveBtn, 4);
        }
    }
}
