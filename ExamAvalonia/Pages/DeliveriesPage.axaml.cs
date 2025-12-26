using Avalonia.Controls;
using Avalonia.Interactivity;
using ExamAvalonia.Data;
using ExamAvalonia.Windows;
using Microsoft.EntityFrameworkCore;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using System;
using System.Linq;

namespace ExamAvalonia.Pages;

public partial class DeliveriesPage : UserControl
{
    public DeliveriesPage()
    {
        InitializeComponent();
        LoadDeliveries();
    }

    private void LoadDeliveries()
    {
        var query = App.DbContext.Deliveries
            .Include(d => d.Supplier)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(SearchBox.Text))
        {
            string search = SearchBox.Text.ToLower();
            query = query.Where(d => d.Supplier.Name.ToLower().Contains(search));
        }

        if (DateFilter.SelectedDate.HasValue)
        {
            var date = DateFilter.SelectedDate.Value.Date;
            query = query.Where(d => d.DeliveryDate.ToDateTime(TimeOnly.MinValue) == date);
        }

        var deliveries = query
            .OrderBy(d => d.DeliveryDate)
            .ToList()
            .Select((d, index) => new
            {
                RowNumber = index + 1,
                d.DeliveryId,
                DeliveryDate = d.DeliveryDate.ToString("yyyy-MM-dd"),
                SupplierName = d.Supplier.Name,
                d.Status
            })
            .ToList();

        DeliveriesGrid.ItemsSource = deliveries;
    }

    private void SearchBox_TextChanged(object? sender, Avalonia.Controls.TextChangedEventArgs e)
    {
        LoadDeliveries();
    }

    private void DateFilter_SelectedDateChanged(object? sender, DatePickerSelectedValueChangedEventArgs e)
    {
        LoadDeliveries();
    }

    private void ResetFilters_Click(object? sender, RoutedEventArgs e)
    {
        SearchBox.Text = string.Empty;
        DateFilter.SelectedDate = null;
        LoadDeliveries();
    }

    private async void AddDelivery_Click(object? sender, RoutedEventArgs e)
    {
        var window = new Windows.AddEditDeliveryWindow();
        var parent = this.VisualRoot as Window;
        await window.ShowDialog(parent);
        LoadDeliveries();
    }

    private async void EditDelivery_Click(object? sender, RoutedEventArgs e)
    {
        var deliveryObj = (sender as Button)?.Tag;
        if (deliveryObj == null) return;

        int deliveryId = (int)deliveryObj.GetType().GetProperty("DeliveryId")!.GetValue(deliveryObj)!;
        var delivery = App.DbContext.Deliveries.Find(deliveryId);
        if (delivery == null) return;

        var window = new Windows.AddEditDeliveryWindow(delivery);
        var parent = this.VisualRoot as Window;
        await window.ShowDialog(parent);
        LoadDeliveries();
    }

    private async void DeleteDelivery_Click(object? sender, RoutedEventArgs e)
    {
        var deliveryObj = (sender as Button)?.Tag;
        if (deliveryObj == null) return;

        int deliveryId = (int)deliveryObj.GetType().GetProperty("DeliveryId")!.GetValue(deliveryObj)!;
        string supplierName = (string)deliveryObj.GetType().GetProperty("SupplierName")!.GetValue(deliveryObj)!;

        var confirm = MessageBoxManager.GetMessageBoxStandard(
            "Confirm delete",
            $"Delete delivery from '{supplierName}'?",
            ButtonEnum.YesNo,
            Icon.Question);

        var parent = this.VisualRoot as Window;
        var result = await confirm.ShowWindowDialogAsync(parent);
        if (result != ButtonResult.Yes) return;

        var delivery = App.DbContext.Deliveries
            .Include(d => d.DeliveryItems)
            .FirstOrDefault(d => d.DeliveryId == deliveryId);

        if (delivery != null)
        {
            App.DbContext.DeliveryItems.RemoveRange(delivery.DeliveryItems);
            App.DbContext.Deliveries.Remove(delivery);
            App.DbContext.SaveChanges();
        }

        LoadDeliveries();
    }

    private async void ViewItems_Click(object? sender, RoutedEventArgs e)
    {
        var deliveryObj = (sender as Button)?.Tag;
        if (deliveryObj == null) return;

        int deliveryId = (int)deliveryObj.GetType().GetProperty("DeliveryId")!.GetValue(deliveryObj)!;
        var delivery = App.DbContext.Deliveries
            .Include(d => d.DeliveryItems)
            .ThenInclude(di => di.Product)
            .FirstOrDefault(d => d.DeliveryId == deliveryId);

        if (delivery != null)
        {
            var window = new AddEditDeliveryWindow(delivery);
            var parent = this.VisualRoot as Window;
            await window.ShowDialog(parent);
        }

        LoadDeliveries();
    }

}
