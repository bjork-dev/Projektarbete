using Butik;
using System;
using System.Collections.Generic;
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

namespace Butik_Creator
{
    public partial class MainWindow : Window
    {
        string discountPath = @"C:\Windows\Temp\Coupon.csv";
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

            ListBox assortmentListBox = new ListBox
            {
                Margin = new Thickness(5),
                //   MaxHeight = 300
            };
            assortmentGrid.Children.Add(assortmentListBox);
            Grid.SetColumn(assortmentListBox, 0);
            Grid.SetRow(assortmentListBox, 0);

            assortmentListBox.Items.Add("item 1  100 kr");
            assortmentListBox.Items.Add("item 2  200 kr");
            assortmentListBox.Items.Add("item 3  300 kr");

            Button removeAllButton = new Button
            {
                Content = "Remove all",
                Margin = new Thickness(5),
                FontSize = 12,
                Padding = new Thickness(5)
            };
            assortmentGrid.Children.Add(removeAllButton);
            Grid.SetColumn(removeAllButton, 0);
            Grid.SetRow(removeAllButton, 1);

            Label galleryLabel = new Label
            {
                Content = "Bild gallery",
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

            TextBox nameTextBox = new TextBox
            {
                Margin = new Thickness(5)
            };
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

            TextBox descriptionTextBox = new TextBox
            {
                Margin = new Thickness(5)
            };
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

            TextBox priceTextBox = new TextBox
            {
                Margin = new Thickness(5)
            };
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

            ListBox discountListBox = new ListBox
            {
                Margin = new Thickness(5),
                MaxHeight = 500
            };
            discountGrid.Children.Add(discountListBox);
            Grid.SetColumn(discountListBox, 0);
            Grid.SetRow(discountListBox, 0);

            discountListBox.Items.Add("code1   1%");
            discountListBox.Items.Add("code5   5%");
            discountListBox.Items.Add("12345678912345678912   100%");

            // Nested grid for buttons Remove and RemoveAll
            Grid removeButtonGrid = new Grid { Margin = new Thickness(5) };
            removeButtonGrid.ColumnDefinitions.Add(new ColumnDefinition());
            removeButtonGrid.ColumnDefinitions.Add(new ColumnDefinition());
            discountGrid.Children.Add(removeButtonGrid);
            Grid.SetRow(removeButtonGrid, 1);
            Grid.SetColumn(removeButtonGrid, 0);

            Button removeButton = new Button
            {
                Content = "Remove",
                Margin = new Thickness(5),
                FontSize = 12,
                Padding = new Thickness(5)
            };
            removeButtonGrid.Children.Add(removeButton);
            Grid.SetColumn(removeButton, 0);

            Button removeAllButton = new Button
            {
                Content = "Remove all",
                Margin = new Thickness(5),
                FontSize = 12,
                Padding = new Thickness(5)
            };
            removeButtonGrid.Children.Add(removeAllButton);
            Grid.SetColumn(removeAllButton, 1);

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

            Label codeLabel = new Label
            {
                Content = "Code",
                Margin = new Thickness(5),
                HorizontalContentAlignment = HorizontalAlignment.Right
            };
            addDiscountGrid.Children.Add(codeLabel);
            Grid.SetColumn(codeLabel, 0);
            Grid.SetRow(codeLabel, 0);

            TextBox codeTextBox = new TextBox
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
                Content = "Discount",
                Margin = new Thickness(5),
                HorizontalContentAlignment = HorizontalAlignment.Right
            };
            addDiscountGrid.Children.Add(discountLabel);
            Grid.SetColumn(discountLabel, 0);
            Grid.SetRow(discountLabel, 1);

            TextBox discountTextBox = new TextBox
            {
                Margin = new Thickness(5),
                VerticalAlignment = VerticalAlignment.Center
            };
            addDiscountGrid.Children.Add(discountTextBox);
            Grid.SetColumn(discountTextBox, 1);
            Grid.SetRow(discountTextBox, 1);

            Button addButton = new Button
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

            Button saveChangesButton = new Button
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

            Button discartButton = new Button
            {
                Content = "Discart",
                Margin = new Thickness(5),
                FontSize = 12,
                Padding = new Thickness(5),
                IsEnabled = false
            };
            Grid.SetRow(discartButton, 3);
            Grid.SetColumn(discartButton, 1);
            addDiscountGrid.Children.Add(discartButton);

            TextBlock discountInfo = new TextBlock
            {
                Text = "Select a discount code to change",
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(5),
                FontSize = 12,
            };
            Grid.SetRow(discountInfo, 4);
            Grid.SetColumn(discountInfo, 0);
            addDiscountGrid.Children.Add(discountInfo);
            Grid.SetColumnSpan(discountInfo, 2);

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
            saveButton.Click += SaveButton_Click;

            return discountGrid;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}