using Avalonia.Controls;
using Avalonia.Interactivity;
using ExamAvalonia.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using ExamAvalonia.Windows;
using System.Threading.Tasks;

namespace ExamAvalonia.Pages;

public partial class Products : UserControl
{
    public Products()
    {
        InitializeComponent();
        LoadCategories();
        LoadData();
    }

    private void LoadData()
    {
        string search = SearchBox.Text?.ToLower();
        var selectedCategory = CategoryComboBox.SelectedItem as ProductCategory;

        var query = App.DbContext.Products
            .Include(p => p.Category)
            .Include(p => p.StockBalances)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(p =>
                p.Name.ToLower().Contains(search));
        }

        if (selectedCategory != null)
        {
            query = query.Where(p =>
                p.CategoryId == selectedCategory.CategoryId);
        }

        ProductsGrid.ItemsSource = query.ToList();
    }

    private void LoadCategories()
    {
        var categories = App.DbContext.ProductCategories.ToList();

        CategoryComboBox.ItemsSource = categories;
    }

    private void SearchBox_TextChanged(object? sender, TextChangedEventArgs e)
    {
        LoadData();
    }

    private void CategoryComboBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        LoadData();
    }

    private void ResetFilters_Click(object? sender, RoutedEventArgs e)
    {
        SearchBox.Text = string.Empty;
        CategoryComboBox.SelectedItem = null;
        LoadData();
    }

    private async void Add_Click(object? sender, RoutedEventArgs e)
    {
        var window = new AddAndEditProducts();
        var result = await window.ShowDialog<bool>(this.VisualRoot as Window);

        if (result == true)
            LoadData();
    }
    private async void Edit_Click(object? sender, RoutedEventArgs e)
    {
        var button = sender as Button;
        var product = button?.DataContext as Product;

        if (product == null)
            return;

        var window = new AddAndEditProducts(product);
        var result = await window.ShowDialog<bool>(this.VisualRoot as Window);

        if (result == true)
            LoadData();
    }

    private async void Delete_Click(object? sender, RoutedEventArgs e)
    {
        var button = sender as Button;
        var product = button?.DataContext as Product;

        if (product == null)
            return;

        App.DbContext.Products.Remove(product);
        App.DbContext.SaveChanges();

        LoadData();
    }

}
