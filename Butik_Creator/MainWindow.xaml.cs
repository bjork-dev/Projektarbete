using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Butik_Creator
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

        public decimal Price { get; set; }

        public string Description
        {
            get => description;
            set
            {
                if (description == string.Empty)
                {
                    throw new Exception("description cannot be null");

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
        private const string Path = @"C:\Windows\Temp\store.csv";
        public const string CouponLocalPath = "Coupon.csv";
        public const string CouponGlobalPath = @"C:\Windows\Temp\" + CouponLocalPath;
        private List<string> assortItemsSave = new List<string>();
        private List<string> comparer = new List<string>();

        private List<Store> storeList = new List<Store>();

        private List<CodeDiscount>
            discountsList =
                new List<CodeDiscount>(); // List with valid codes and discounts (string "code", int discount)

        private ObservableCollection<string>
            discountsShow =
                new ObservableCollection<string>();// List for displaying valid codes and discounts ("code   discount %")
        private ObservableCollection<string>
            assortShow =
                new ObservableCollection<string>();

        TextBox codeTextBox;
        TextBox discountTextBox;
        ListBox discountListBox;
        Button addButton, discardButton, saveChangesButton;

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

        ComboBox imageBox = new ComboBox()
        {
            Text = "Select image",
            IsReadOnly = true,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(5),
            Width = 100,
            Height = 25,
            // Stretch = Stretch.Uniform
        };

        Button assortSaveChangesButton = new Button
        {
            Content = "Save changes",
            Margin = new Thickness(5),
            FontSize = 12,
            Padding = new Thickness(5),
            IsEnabled = false
        };

        Button assortRemoveButton = new Button
        {
            Content = "Remove",
            Margin = new Thickness(5),
            FontSize = 12,
            Padding = new Thickness(5),
            IsEnabled = false
        };

        Button assortAddButton = new Button
        {
            Content = "Add new",
            Margin = new Thickness(5),
            FontSize = 12,
            Padding = new Thickness(5),
            IsEnabled = true
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

            if (!File.Exists(Path)) //Copy store file from project if it does not exist in Temp
            {
                File.Copy("store.csv", Path);
            }

            if (!File.Exists(CouponGlobalPath)) //Copy coupon file from project if it does not exist in Temp
            {
                File.Copy("Coupon.csv", CouponGlobalPath);
            }

            LoadDiscounts(discountsList, discountsShow); // Load saved discounts from a file (if it exists)   
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
            assortmentListBox.ItemsSource = assortShow;

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


            RenderOptions.SetBitmapScalingMode(imageBox, BitmapScalingMode.HighQuality);
            nestedGrid.Children.Add(imageBox);
            Grid.SetColumn(imageBox, 3);
            Grid.SetRow(imageBox, 0);
            Grid.SetRowSpan(imageBox, 3);

            var dir = Directory.GetCurrentDirectory();
            foreach (var item in Directory.GetFiles($@"{dir}\Images")
            ) //Gets all files in the image folder and then extracts the image filename
            {
                string[] arr = item.Split('\\');
                var lastItem = arr.Last();
                imageBox.Items.Add(lastItem);

                Image testImage = new Image //Adds an image per file in the image folder
                {
                    Source = new BitmapImage(new Uri(@$"Images\{lastItem}", UriKind.Relative)),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(5),
                    Width = 80,
                    Height = 80,
                    Stretch = Stretch.Uniform
                };
                RenderOptions.SetBitmapScalingMode(testImage, BitmapScalingMode.HighQuality);
                bildGallery.Children.Add(testImage);

                Label imageLabel = new Label //To describe the filenames of the images
                {
                    Content = lastItem,
                    Margin = new Thickness(5),
                    HorizontalContentAlignment = HorizontalAlignment.Center

                };
                bildGallery.Children.Add(imageLabel);
            }

            Grid.SetRow(assortSaveChangesButton, 3);
            Grid.SetColumn(assortSaveChangesButton, 1);
            nestedGrid.Children.Add(assortSaveChangesButton);
            assortSaveChangesButton.Click += AssortSaveChanges;


            Grid.SetRow(assortRemoveButton, 3);
            Grid.SetColumn(assortRemoveButton, 2);
            nestedGrid.Children.Add(assortRemoveButton);
            assortRemoveButton.Click += AssortRemove;

            Grid.SetRow(assortAddButton, 4);
            Grid.SetColumn(assortAddButton, 1);
            nestedGrid.Children.Add(assortAddButton);
            Grid.SetColumnSpan(assortAddButton, 2);
            assortAddButton.Click += AddAssortment;

            Button assortSaveButton = new Button
            {
                Content = "Save to file",
                Margin = new Thickness(90, 5, 150, 5),
                FontSize = 12,
                Padding = new Thickness(5),
            };
            Grid.SetRow(assortSaveButton, 1);
            Grid.SetColumn(assortSaveButton, 2);
            assortmentGrid.Children.Add(assortSaveButton);
            assortSaveButton.Click += SaveAssortment;

            assortmentListBox.SelectionChanged += AssortmentListBox_SelectionChanged;
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

            try
            {
                var discount = int.Parse(discountTextBox.Text);
                string code = codeTextBox.Text.ToLower();

                List<CodeDiscount> copyDiscountsList = discountsList.Select(l => l).ToList();
                copyDiscountsList.RemoveAt(indexSelected);

                if (!IsDuplicate(code, copyDiscountsList)) // check whether this new code is already in the list
                {
                    try
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
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                else MessageBox.Show("This code already exists");
            }
            catch
            {
                MessageBox.Show("Discount must be an integer.");
            }
        }

        // Load saved discounts from a file at CouponPath. If it doesn´t exist, then load discounts from the local file Coupons.scv   
        public static void LoadDiscounts(List<CodeDiscount> discountsList,
            ObservableCollection<string> discountsShow = null)
        {
            string path;
            if (File.Exists(CouponGlobalPath))
            {
                path = CouponGlobalPath;
            }
            else if (File.Exists(CouponLocalPath))
            {
                path = CouponLocalPath;
            }
            else return;

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

                storeList.Add(new Store
                    {Name = p.Name, Price = p.Price, Description = p.Description, ImageName = p.ImageName});
                assortShow?.Add($"{p.Name} {p.Price}kr");
                comparer.Add(p.Name.ToLower());
                assortItemsSave.Add($"{p.Name},{p.Price},{p.Description},{p.ImageName}");
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
            try
            {
                var discount = int.Parse(discountTextBox.Text);
                string code = codeTextBox.Text.ToLower();

                if (!IsDuplicate(code, discountsList)) // check whether this code is already in the list
                {
                    try
                    {
                        discountsList.Add(new CodeDiscount { Code = code, Discount = discount });
                        discountsShow.Add(code + "   " + discount + " %");
                        codeTextBox.Clear();
                        discountTextBox.Clear();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                else MessageBox.Show("This code already exists");
            }
            catch
            {
                MessageBox.Show("Discount must be an integer.");
            }
        }

        private void SaveCoupon(object sender, RoutedEventArgs e)
        {
            //string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Coupon.csv"); //temporary path for testing
            List<string> temp = discountsList.Select(code => code.Code + "," + code.Discount).ToList();
            File.WriteAllLines(CouponGlobalPath, temp);
            MessageBox.Show("File saved");
        }

        private void RemoveAll(object sender, RoutedEventArgs e)
        {
            if (assortmentListBox.Items.Count == 0) return;
            const string message = "Would you like to remove all items from the store file?";
            var result = MessageBox.Show(message, "Remove all", MessageBoxButton.YesNo);
            if (result != MessageBoxResult.Yes) return;
            assortShow.Clear();
            File.Create(Path);
        }

        private void AddAssortment(object sender, RoutedEventArgs e)
        {
            try
            {
                var s = new Store
                {
                    Name = nameTextBox.Text,
                    Price = decimal.Parse(priceTextBox.Text),
                    Description = descriptionTextBox.Text,
                    ImageName = imageBox.SelectionBoxItem.ToString()
                };

                for (int i = 0; i < assortmentListBox.Items.Count; i++)
                {
                    if (!comparer.Contains(s.Name)) continue;
                    MessageBox.Show("Product already exists.");
                    return;
                }

                if (s.ImageName == string.Empty)
                {
                    MessageBox.Show("Please select an image");
                }
                else
                {
                    assortShow.Add($"{s.Name} {s.Price}kr");
                    assortItemsSave.Add($"{s.Name},{s.Price},{s.Description},{s.ImageName}");
                    comparer.Add(s.Name);
                    storeList.Add(new Store()
                    { Name = s.Name, Price = s.Price, Description = s.Description, ImageName = s.ImageName });
                }

            }
            catch (Exception a)
            {
                MessageBox.Show(a.Message);
            }
        }

        private void SaveAssortment(object sender, RoutedEventArgs e)
        {
            if (assortItemsSave.Count == 0)
            {
                MessageBox.Show("Add items first");
            }
            else
            {
                File.WriteAllLines(Path, assortItemsSave);
            }

        }

        private void AssortmentListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (assortmentListBox.SelectedIndex != -1)
            {
                nameTextBox.Text = storeList[assortmentListBox.SelectedIndex].Name;
                priceTextBox.Text = storeList[assortmentListBox.SelectedIndex].Price
                    .ToString(CultureInfo.InvariantCulture);
                descriptionTextBox.Text = storeList[assortmentListBox.SelectedIndex].Description;
                imageBox.Text = storeList[assortmentListBox.SelectedIndex].ImageName;
                assortSaveChangesButton.IsEnabled = true;
                saveChangesButton.IsEnabled = true;
                assortRemoveButton.IsEnabled = true;
            }
        }
        private void AssortSaveChanges(object sender, RoutedEventArgs e)
        {
            int indexSelected = assortmentListBox.SelectedIndex;

            try
            {
                var name = nameTextBox.Text;
                var price = priceTextBox.Text;
                var description = descriptionTextBox.Text;
                var imageName = imageBox.Text;

                List<Store> copyStoreList = storeList.Select(l => l).ToList();
                copyStoreList.RemoveAt(indexSelected);

                try //Send help
                {
                    storeList[indexSelected].Name = name;
                    storeList[indexSelected].Price = decimal.Parse(price);
                    storeList[indexSelected].Description = description;
                    storeList[indexSelected].ImageName = imageName;
                    assortShow[indexSelected] = $"{name} {price}kr";
                    codeTextBox.Clear();
                    discountTextBox.Clear();
                    discountListBox.SelectedIndex = -1;
                    addButton.IsEnabled = true;
                    assortSaveChangesButton.IsEnabled = false;
                    assortRemoveButton.IsEnabled = false;
                    comparer.RemoveAt(indexSelected);
                    comparer.Insert(indexSelected, name);
                    assortItemsSave.Insert(indexSelected,$"{name},{price},{description},{imageName}" );
                    assortItemsSave.RemoveAt(indexSelected + 1);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }


            }
            catch (Exception)
            {
                // ignored
            }
        }
        private void AssortRemove(object sender, RoutedEventArgs e)
        {
            int indexSelected = assortmentListBox.SelectedIndex;
            assortRemoveButton.IsEnabled = false;
            assortSaveChangesButton.IsEnabled = false;
            assortAddButton.IsEnabled = true;
            nameTextBox.Clear();
            priceTextBox.Clear();
            descriptionTextBox.Clear();
            discountListBox.SelectedIndex = -1;
            assortShow.RemoveAt(indexSelected);
            comparer.RemoveAt(indexSelected);
            assortItemsSave.RemoveAt(indexSelected);
        }
    }
}