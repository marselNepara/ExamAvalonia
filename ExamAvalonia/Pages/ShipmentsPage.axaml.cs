using Avalonia.Controls;
using Avalonia.Interactivity;
using ExamAvalonia.Data;
using ExamAvalonia.Windows;
using Microsoft.EntityFrameworkCore;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using System;
using System.Linq;

namespace ExamAvalonia.Pages
{
    public partial class ShipmentsPage : UserControl
    {
        private int? selectedWarehouseId = null;

        public ShipmentsPage()
        {
            InitializeComponent();
            LoadWarehouses();
            LoadShipments();
        }

        private void LoadWarehouses()
        {
            var warehouses = App.DbContext.Warehouses.OrderBy(w => w.Name).ToList();
            WarehouseFilter.ItemsSource = warehouses;
        }

        private void LoadShipments()
        {
            var query = App.DbContext.Shipments.Include(s => s.Warehouse).AsQueryable();

            if (DateFilter.SelectedDate.HasValue)
            {
                var date = DateFilter.SelectedDate.Value.Date;
                query = query.Where(s => s.ShipmentDate.ToDateTime(TimeOnly.MinValue) == date);
            }

            if (selectedWarehouseId.HasValue)
            {
                query = query.Where(s => s.WarehouseId == selectedWarehouseId.Value);
            }

            var shipments = query
                .OrderBy(s => s.ShipmentDate)
                .ToList()
                .Select((s, index) => new
                {
                    RowNumber = index + 1,
                    s.ShipmentId,
                    WarehouseName = s.Warehouse.Name,
                    ShipmentDate = s.ShipmentDate.ToString("yyyy-MM-dd"),
                    s.Recipient,
                    s.Status
                })
                .ToList();

            ShipmentsGrid.ItemsSource = shipments;
        }

        private void DateFilter_SelectedDateChanged(object? sender, DatePickerSelectedValueChangedEventArgs e)
        {
            LoadShipments();
        }

        private void WarehouseFilter_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (WarehouseFilter.SelectedItem is Warehouse warehouse)
                selectedWarehouseId = warehouse.WarehouseId;
            else
                selectedWarehouseId = null;

            LoadShipments();
        }

        private void ResetFilters_Click(object? sender, RoutedEventArgs e)
        {
            DateFilter.SelectedDate = null;
            WarehouseFilter.SelectedIndex = -1;
            selectedWarehouseId = null;
            LoadShipments();
        }

        private async void AddShipment_Click(object? sender, RoutedEventArgs e)
        {
            var window = new AddEditShipmentWindow();
            var parent = this.VisualRoot as Window;
            await window.ShowDialog(parent);
            LoadShipments(); 
        }

        private async void EditShipment_Click(object? sender, RoutedEventArgs e)
        {
            var shipmentObj = (sender as Button)?.Tag;
            if (shipmentObj == null) return;

            int shipmentId = (int)shipmentObj.GetType().GetProperty("ShipmentId")!.GetValue(shipmentObj)!;
            var shipment = App.DbContext.Shipments.Find(shipmentId);
            if (shipment == null) return;

            var window = new AddEditShipmentWindow(shipment);
            var parent = this.VisualRoot as Window;
            await window.ShowDialog(parent);
            LoadShipments();
        }

        private async void DeleteShipment_Click(object? sender, RoutedEventArgs e)
        {
            var shipmentObj = (sender as Button)?.Tag;
            if (shipmentObj == null) return;

            int shipmentId = (int)shipmentObj.GetType().GetProperty("ShipmentId")!.GetValue(shipmentObj)!;

            var confirm = MessageBoxManager.GetMessageBoxStandard(
                "Confirm delete",
                $"Delete this shipment?",
                ButtonEnum.YesNo,
                Icon.Question);

            var parent = this.VisualRoot as Window;
            var result = await confirm.ShowWindowDialogAsync(parent);
            if (result != ButtonResult.Yes) return;

            var shipment = App.DbContext.Shipments.Find(shipmentId);
            if (shipment != null)
            {
                App.DbContext.Shipments.Remove(shipment);
                App.DbContext.SaveChanges();
            }

            LoadShipments();
        }
    }
}
