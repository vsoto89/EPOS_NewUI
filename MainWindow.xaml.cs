using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace EPOS_NewUI;

public partial class MainWindow : Window
{
    private readonly ObservableCollection<TicketItem> _ticketItems = new();
    private readonly List<ProductItem> _allProducts = new();
    private readonly DispatcherTimer _clockTimer = new();
    private string? _selectedCategory;
    private string _searchText = string.Empty;

    public MainWindow()
    {
        InitializeComponent();
        InitializeProducts();
        BuildCategoryButtons();
        TicketListBox.ItemsSource = _ticketItems;
        UpdateSummary();
        RenderProducts();

        // Inicializar vendedores (ejemplo) y bloquear venta hasta seleccionar uno
        SellerComboBox.ItemsSource = new List<string> { "Juan Pérez", "María Gómez", "Administrador" };
        SellerComboBox.SelectedIndex = -1;

        SearchBox.IsEnabled = false;
        CategoryPanel.IsEnabled = false;
        ProductsPanel.IsEnabled = false;
        ClearButton.IsEnabled = false;
        CheckoutButton.IsEnabled = false;
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        _clockTimer.Interval = TimeSpan.FromSeconds(1);
        _clockTimer.Tick += (_, _) => CurrentTimeText.Text = DateTime.Now.ToString("HH:mm:ss");
        _clockTimer.Start();
        CurrentTimeText.Text = DateTime.Now.ToString("HH:mm:ss");
    }

    private void SellerComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var hasSeller = SellerComboBox.SelectedItem != null;
        SearchBox.IsEnabled = hasSeller;
        CategoryPanel.IsEnabled = hasSeller;
        ProductsPanel.IsEnabled = hasSeller;
        ClearButton.IsEnabled = hasSeller;
        CheckoutButton.IsEnabled = hasSeller;
    }

    private void InitializeProducts()
    {
        _allProducts.AddRange(new[]
        {
            // Precios expresados en CLP (enteros) para evitar inconsistencias entre visualización y cálculo
            new ProductItem("Café Americano", "Bebidas", 3m),
            new ProductItem("Capuccino", "Bebidas", 4m),
            new ProductItem("Jugo Natural", "Bebidas", 3m),
            new ProductItem("Sandwich Club", "Comidas", 4m),
            new ProductItem("Bagel de Jamón", "Comidas", 6m),
            new ProductItem("Ensalada César", "Comidas", 6m),
            new ProductItem("Brownie", "Postres", 4m),
            new ProductItem("Cheesecake", "Postres", 5m),
            new ProductItem("Agua Mineral", "Bebidas", 2m)
        });
    }

    private void BuildCategoryButtons()
    {
        CategoryPanel.Children.Clear();

        var allButton = CreateCategoryButton("Todas");
        CategoryPanel.Children.Add(allButton);

        foreach (var category in _allProducts.Select(item => item.Category).Distinct().OrderBy(item => item))
        {
            CategoryPanel.Children.Add(CreateCategoryButton(category));
        }
    }

    private Button CreateCategoryButton(string category)
    {
        var button = new Button
        {
            Content = category,
            Tag = category,
            Style = (Style)FindResource("CategoryButtonStyle")
        };

        button.Click += CategoryButton_Click;
        return button;
    }

    private void CategoryButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button)
        {
            return;
        }

        _selectedCategory = button.Tag?.ToString();
        RenderProducts();
    }

    private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        _searchText = SearchBox.Text?.Trim() ?? string.Empty;
        RenderProducts();
    }

    private void RenderProducts()
    {
        ProductsPanel.Children.Clear();

        var filteredProducts = _allProducts
            .Where(product => MatchesFilter(product))
            .ToList();

        foreach (var product in filteredProducts)
        {
            var button = new Button
            {
                Tag = product,
                Style = (Style)FindResource("ProductButtonStyle"),
                Content = new StackPanel
                {
                    Children =
                    {
                        new TextBlock
                        {
                            Text = product.Name,
                            FontWeight = FontWeights.SemiBold,
                            TextWrapping = TextWrapping.Wrap,
                            TextAlignment = TextAlignment.Center
                        },
                        new TextBlock
                        {
                            Text = $"CLP {product.Price:N0}",
                            Margin = new Thickness(0, 6, 0, 0),
                            FontSize = 13,
                            Foreground = new SolidColorBrush(Color.FromRgb(209, 250, 229))
                        }
                    }
                }
            };

            button.Click += ProductButton_Click;
            ProductsPanel.Children.Add(button);
        }
    }

    private bool MatchesFilter(ProductItem product)
    {
        var categoryMatches = _selectedCategory is null || _selectedCategory == "Todas" || string.Equals(product.Category, _selectedCategory, StringComparison.OrdinalIgnoreCase);
        var searchMatches = string.IsNullOrWhiteSpace(_searchText) || product.Name.Contains(_searchText, StringComparison.OrdinalIgnoreCase);
        return categoryMatches && searchMatches;
    }

    private void ProductButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button || button.Tag is not ProductItem product)
        {
            return;
        }

        var existing = _ticketItems.FirstOrDefault(item => item.ProductName == product.Name);
        if (existing is null)
        {
            _ticketItems.Add(new TicketItem(product.Name, product.Price, 1));
        }
        else
        {
            existing.Quantity++;
        }

        UpdateSummary();
    }

    private void ClearButton_Click(object sender, RoutedEventArgs e)
    {
        _ticketItems.Clear();
        UpdateSummary();
    }

    private void CheckoutButton_Click(object sender, RoutedEventArgs e)
    {
        if (_ticketItems.Count == 0)
        {
            MessageBox.Show("Agrega productos al ticket antes de cobrar.", "Ticket vacío", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var subtotal = _ticketItems.Sum(item => item.Subtotal);
        var tax = subtotal * 0.15m;
        var total = subtotal + tax;
        MessageBox.Show($"Ticket listo para cobrar. Total: {total:C2}", "Cobro", MessageBoxButton.OK, MessageBoxImage.Information);
        _ticketItems.Clear();
        UpdateSummary();
    }

    private void UpdateSummary()
    {
        var subtotal = _ticketItems.Sum(item => item.Subtotal);
        var tax = subtotal * 0.15m;
        var total = subtotal + tax;
        var quantity = _ticketItems.Sum(item => item.Quantity);

        var clpCulture = CultureInfo.GetCultureInfo("es-CL");

        SubtotalText.Text = $"Subtotal: {subtotal.ToString("C0", clpCulture)}";
        TaxText.Text = $"IVA: {tax.ToString("C0", clpCulture)}";
        TotalText.Text = $"Total: {total.ToString("C0", clpCulture)}";
        ItemCountText.Text = $"{quantity} artículos";
    }

    private sealed class TicketItem : INotifyPropertyChanged
    {
        private int _quantity;
        private static readonly CultureInfo ClpCulture = CultureInfo.GetCultureInfo("es-CL");

        public TicketItem(string productName, decimal unitPrice, int quantity)
        {
            ProductName = productName;
            UnitPrice = unitPrice;
            Quantity = quantity;
        }

        public string ProductName { get; }
        public decimal UnitPrice { get; }
        public int Quantity
        {
            get => _quantity;
            set
            {
                if (_quantity == value)
                {
                    return;
                }

                _quantity = value;
                OnPropertyChanged(nameof(Quantity));
                OnPropertyChanged(nameof(Summary));
                OnPropertyChanged(nameof(Subtotal));
                OnPropertyChanged(nameof(SubtotalDisplay));
            }
        }

        public decimal Subtotal => UnitPrice * Quantity;
        public string Summary => Quantity == 1 ? "1 unidad" : $"{Quantity} unidades";
        public string SubtotalDisplay => Subtotal.ToString("C0", ClpCulture);
        public string UnitPriceDisplay => UnitPrice.ToString("C0", ClpCulture);

                public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    private void IncreaseQty_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement fe || fe.DataContext is not TicketItem item)
            return;

        item.Quantity++;
        UpdateSummary();
    }

    private void DecreaseQty_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement fe || fe.DataContext is not TicketItem item)
            return;

        if (item.Quantity > 1)
        {
            item.Quantity--;
        }
        else
        {
            _ticketItems.Remove(item);
        }

        UpdateSummary();
    }

    private sealed class ProductItem
    {
        public ProductItem(string name, string category, decimal price)
        {
            Name = name;
            Category = category;
            Price = price;
        }

        public string Name { get; }
        public string Category { get; }
        public decimal Price { get; }
    }
}