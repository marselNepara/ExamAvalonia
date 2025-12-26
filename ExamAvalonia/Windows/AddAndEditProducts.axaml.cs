using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ExamAvalonia.Data;
using System.Linq;

namespace ExamAvalonia.Windows;

public partial class AddAndEditProducts : Window
{
    private Product _currentProduct;
    private bool _isEditMode;

    public AddAndEditProducts()
    {
        InitializeComponent();
        LoadCategories();

        _currentProduct = new Product();
        _isEditMode = false;

        TitleText.Text = "Add product";
    }

    public AddAndEditProducts(Product product)
    {
        InitializeComponent();
        LoadCategories();

        _currentProduct = product;
        _isEditMode = true;

        TitleText.Text = "Edit product";
        FillFields();
    }

    private void LoadCategories()
    {
        CategoryBox.ItemsSource = App.DbContext.ProductCategories.ToList();
    }

    private void FillFields()
    {
        NameBox.Text = _currentProduct.Name;
        ArticleBox.Text = _currentProduct.Article;
        UnitBox.Text = _currentProduct.Unit;
        MinStockBox.Value = _currentProduct.MinStock ?? 0;

        CategoryBox.SelectedItem = App.DbContext.ProductCategories
            .FirstOrDefault(c => c.CategoryId == _currentProduct.CategoryId);
    }

    private void Save_Click(object? sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NameBox.Text) ||
            CategoryBox.SelectedItem == null)
        {
            ShowError("Fill required fields");
            return;
        }

        _currentProduct.Name = NameBox.Text;
        _currentProduct.Article = ArticleBox.Text;
        _currentProduct.Unit = UnitBox.Text;
        _currentProduct.MinStock = (int?)MinStockBox.Value;

        var category = CategoryBox.SelectedItem as ProductCategory;
        _currentProduct.CategoryId = category.CategoryId;

        if (!_isEditMode)
            App.DbContext.Products.Add(_currentProduct);
        else
            App.DbContext.Products.Update(_currentProduct);

        App.DbContext.SaveChanges();
        Close(true);
    }

    private void Cancel_Click(object? sender, RoutedEventArgs e)
    {
        Close(false);
    }

    private async void ShowError(string text)
    {
        var dialog = new Window
        {
            Width = 300,
            Height = 150,
            Content = new TextBlock
            {
                Text = text,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
            }
        };

        await dialog.ShowDialog(this);
    }
}