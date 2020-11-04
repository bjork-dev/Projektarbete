using Butik;
using System;
using System.Collections.Generic;
using System.Data;
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

namespace Butik_Creator
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

            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

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
            Grid assortmentPanel = CreatAssortmentPanel();
            grid.Children.Add(assortmentPanel);
            Grid.SetRow(assortmentPanel, 1);
            Grid.SetColumn(assortmentPanel, 0);

            // Create Discount Panel
            Grid discountPanel = CreatDiscountPanel();
            grid.Children.Add(discountPanel);
            Grid.SetRow(discountPanel, 1);
            Grid.SetColumn(discountPanel, 1);
        }
        private Grid CreatAssortmentPanel()
        {
            // Nested grid for the part Assortment
            Grid assortmentGrid = new Grid
            {
                Margin = new Thickness(5)
            };
            assortmentGrid.RowDefinitions.Add(new RowDefinition());
            assortmentGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            assortmentGrid.ColumnDefinitions.Add(new ColumnDefinition());




            return assortmentGrid;
        }

        private Grid CreatDiscountPanel()
        {
            // Nested grid for the part Discount
            Grid discountGrid = new Grid
            {
                Margin = new Thickness(5)
            };
            discountGrid.RowDefinitions.Add(new RowDefinition());
            discountGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            discountGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            // Table
            DataGrid dataGrid = new DataGrid();
            discountGrid.Children.Add(dataGrid);
            Grid.SetRow(dataGrid, 0);

            DataTable table = new DataTable();
            table.Columns.Add(new DataColumn("Code", typeof(string)));
            table.Columns.Add(new DataColumn("Discount, %", typeof(int)));
            table.Columns.Add(new DataColumn("Remove", typeof(bool)));

            table.Rows.Add(new object[] { "code1", 1, false });

            dataGrid.ItemsSource = table.DefaultView;


            // StackPanel 
            StackPanel panelRemoveButton = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right };
            discountGrid.Children.Add(panelRemoveButton);
            Grid.SetRow(panelRemoveButton, 1);

            // CheckBox 
            CheckBox markAllCheckBox = new CheckBox
            {
                Content = "Select all",
                FontSize = 15,
                Margin = new Thickness(5),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center
            };
            panelRemoveButton.Children.Add(markAllCheckBox);

            // Button Remove
            Button removeButton = new Button
            {
                Content = "Remove selected",
                Margin = new Thickness(5),
                FontSize = 15,
                HorizontalAlignment = HorizontalAlignment.Right,
                Padding = new Thickness(5)
            };
            panelRemoveButton.Children.Add(removeButton);

            // Button Save
            Button saveButton = new Button
            {
                Content = "Save",
                Margin = new Thickness(5, 20, 5, 5),
                FontSize = 15,
                Padding = new Thickness(10),
            };
            Grid.SetRow(saveButton, 2);
            discountGrid.Children.Add(saveButton);

            return discountGrid;
        }
    }
}