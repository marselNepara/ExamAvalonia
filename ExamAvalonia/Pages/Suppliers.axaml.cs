using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ExamAvalonia.Data;
using ExamAvalonia.Windows;
using System.Linq;

namespace ExamAvalonia.Pages;

public partial class Suppliers : UserControl
{
    public Suppliers()
    {
        InitializeComponent();
        LoadData();
    }

    private void LoadData()
    {
        var search = SearchBox.Text?.ToLower();
        var query = App.DbContext.Suppliers.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(s => s.Name.ToLower().Contains(search));

        SuppliersGrid.ItemsSource = query.ToList();
    }

    private void SearchBox_TextChanged(object? sender, TextChangedEventArgs e)
    {
        LoadData();
    }

    private async void AddSupplier_Click(object? sender, RoutedEventArgs e)
    {
        var window = new AddEditSupplier();
        var result = await window.ShowDialog<bool>(this.VisualRoot as Window);
        if (result) LoadData();
    }

    private async void EditSupplier_Click(object? sender, RoutedEventArgs e)
    {
        var supplier = (sender as Button)?.DataContext as Supplier;
        if (supplier == null) return;

        var window = new AddEditSupplier(supplier);
        var result = await window.ShowDialog<bool>(this.VisualRoot as Window);
        if (result) LoadData();
    }

    private void DeleteSupplier_Click(object? sender, RoutedEventArgs e)
    {
        var supplier = (sender as Button)?.DataContext as Supplier;
        if (supplier == null) return;

        App.DbContext.Suppliers.Remove(supplier);
        App.DbContext.SaveChanges();
        LoadData();
    }
}