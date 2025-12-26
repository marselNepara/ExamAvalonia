using Avalonia.Controls;
using Avalonia.Interactivity;
using ExamAvalonia.Data;
using Microsoft.EntityFrameworkCore;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace ExamAvalonia.Windows;

public partial class AddEditDeliveryWindow : Window
{
    private Delivery? delivery;
    private ObservableCollection<DeliveryItemViewModel> items = new();
    public AddEditDeliveryWindow()
    {
        InitializeComponent();
        LoadSuppliers();
        LoadWarehouses();
        ItemsGrid.ItemsSource = items;
    }

    public AddEditDeliveryWindow(Delivery d) : this()
    {
        delivery = d;
        SupplierBox.SelectedItem = App.DbContext.Suppliers.Find(delivery.SupplierId);
        WarehouseBox.SelectedItem = App.DbContext.Warehouses.Find(delivery.WarehouseId);
        DeliveryDatePicker.SelectedDate = new DateTimeOffset(delivery.DeliveryDate.ToDateTime(TimeOnly.MinValue));
        StatusBox.Text = delivery.Status;

        var existingItems = App.DbContext.DeliveryItems
            .Include(di => di.Product)
            .Where(di => di.DeliveryId == delivery.DeliveryId)
            .ToList()
            .Select(di => new DeliveryItemViewModel
            {
                ProductId = di.ProductId,
                ProductName = di.Product.Name,
                Quantity = di.Quantity,
                PurchasePrice = di.PurchasePrice
            });

        foreach (var i in existingItems)
            items.Add(i);
    }

    private void LoadSuppliers()
    {
        var suppliers = App.DbContext.Suppliers.OrderBy(s => s.Name).ToList();
        SupplierBox.ItemsSource = suppliers;
    }

    private void LoadWarehouses()
    {
        var warehouses = App.DbContext.Warehouses.OrderBy(w => w.Name).ToList();
        WarehouseBox.ItemsSource = warehouses;
    }

    private async void AddItem_Click(object? sender, RoutedEventArgs e)
    {
        var window = new AddEditDeliveryItemWindow();
        var parent = this.VisualRoot as Window;
        await window.ShowDialog(parent);

        if (window.Item != null)
            items.Add(window.Item);
    }

    private void DeleteItem_Click(object? sender, RoutedEventArgs e)
    {
        var item = (sender as Button)?.Tag as DeliveryItemViewModel;
        if (item != null)
            items.Remove(item);
    }

    private async void Save_Click(object? sender, RoutedEventArgs e)
    {
        if (SupplierBox.SelectedItem == null ||
            WarehouseBox.SelectedItem == null ||
            DeliveryDatePicker.SelectedDate == null ||
            string.IsNullOrWhiteSpace(StatusBox.Text))
        {
            var msg = MessageBoxManager.GetMessageBoxStandard(
                "Error",
                "Please fill all required fields (*)",
                ButtonEnum.Ok,
                MsBox.Avalonia.Enums.Icon.Error);
            var parent = this.VisualRoot as Window;
            if (parent != null)
                await msg.ShowWindowDialogAsync(parent);
            return;
        }

        if (delivery == null)
        {
            delivery = new Delivery
            {
                SupplierId = ((Supplier)SupplierBox.SelectedItem).SupplierId,
                WarehouseId = ((Warehouse)WarehouseBox.SelectedItem).WarehouseId,
                DeliveryDate = DateOnly.FromDateTime(DeliveryDatePicker.SelectedDate.Value.DateTime),
                Status = StatusBox.Text
            };
            App.DbContext.Deliveries.Add(delivery);
            App.DbContext.SaveChanges(); 
        }
        else
        {
            delivery.SupplierId = ((Supplier)SupplierBox.SelectedItem).SupplierId;
            delivery.WarehouseId = ((Warehouse)WarehouseBox.SelectedItem).WarehouseId;
            delivery.DeliveryDate = DateOnly.FromDateTime(DeliveryDatePicker.SelectedDate.Value.DateTime);
            delivery.Status = StatusBox.Text;

            App.DbContext.Deliveries.Update(delivery);

            var oldItems = App.DbContext.DeliveryItems.Where(di => di.DeliveryId == delivery.DeliveryId);
            App.DbContext.DeliveryItems.RemoveRange(oldItems);
        }

        App.DbContext.SaveChanges();

        foreach (var item in items)
        {
            var di = new DeliveryItem
            {
                DeliveryId = delivery.DeliveryId,
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                PurchasePrice = item.PurchasePrice
            };
            App.DbContext.DeliveryItems.Add(di);
        }
        App.DbContext.SaveChanges();
        Close();
    }

    private void Cancel_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}

public class DeliveryItemViewModel
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = "";
    public int Quantity { get; set; }
    public decimal PurchasePrice { get; set; }
}
