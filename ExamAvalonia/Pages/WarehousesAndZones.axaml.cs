using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ExamAvalonia.Data;
using ExamAvalonia.Windows;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ExamAvalonia.Pages;

public partial class WarehousesAndZones : UserControl
{
    public WarehousesAndZones()
    {
        InitializeComponent();
        LoadWarehouses();
    }

    private void LoadWarehouses()
    {
        var search = WarehouseSearchBox.Text?.ToLower();

        var query = App.DbContext.Warehouses.Include(w => w.StorageZones).AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(w => w.Name.ToLower().Contains(search));

        WarehousesGrid.ItemsSource = query.ToList();
        ZonesGrid.ItemsSource = null;
    }

    private void WarehouseSearchBox_TextChanged(object? sender, TextChangedEventArgs e)
    {
        LoadWarehouses();
    }

    private void WarehousesGrid_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var warehouse = WarehousesGrid.SelectedItem as Warehouse;
        if (warehouse == null)
        {
            ZonesGrid.ItemsSource = null;
            return;
        }

        ZonesGrid.ItemsSource = App.DbContext.StorageZones
            .Where(z => z.WarehouseId == warehouse.WarehouseId)
            .ToList();
    }

    private async void AddWarehouse_Click(object? sender, RoutedEventArgs e)
    {
        var window = new AddEditWarehouse();
        var result = await window.ShowDialog<bool>(this.VisualRoot as Window);
        if (result) LoadWarehouses();
    }

    private async void EditWarehouse_Click(object? sender, RoutedEventArgs e)
    {
        var warehouse = (sender as Button)?.DataContext as Warehouse;
        if (warehouse == null) return;

        var window = new AddEditWarehouse(warehouse);
        var result = await window.ShowDialog<bool>(this.VisualRoot as Window);
        if (result) LoadWarehouses();
    }

    private void DeleteWarehouse_Click(object? sender, RoutedEventArgs e)
    {
        var warehouse = (sender as Button)?.DataContext as Warehouse;
        if (warehouse == null) return;

        if (App.DbContext.StorageZones.Any(z => z.WarehouseId == warehouse.WarehouseId))
            return; 

        App.DbContext.Warehouses.Remove(warehouse);
        App.DbContext.SaveChanges();
        LoadWarehouses();
    }


    private async void AddZone_Click(object? sender, RoutedEventArgs e)
    {
        var warehouse = WarehousesGrid.SelectedItem as Warehouse;
        if (warehouse == null) return;

        var window = new AddEditZone(warehouse.WarehouseId);
        var result = await window.ShowDialog<bool>(this.VisualRoot as Window);
        if (result) WarehousesGrid_SelectionChanged(null, null);
    }

    private async void EditZone_Click(object? sender, RoutedEventArgs e)
    {
        var zone = (sender as Button)?.DataContext as StorageZone;
        if (zone == null) return;

        var window = new AddEditZone(zone);
        var result = await window.ShowDialog<bool>(this.VisualRoot as Window);
        if (result) WarehousesGrid_SelectionChanged(null, null);
    }

    private void DeleteZone_Click(object? sender, RoutedEventArgs e)
    {
        var zone = (sender as Button)?.DataContext as StorageZone;
        if (zone == null) return;

        App.DbContext.StorageZones.Remove(zone);
        App.DbContext.SaveChanges();
        WarehousesGrid_SelectionChanged(null, null);
    }
}