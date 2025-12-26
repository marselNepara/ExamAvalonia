using Avalonia.Controls;
using Avalonia.Interactivity;
using ExamAvalonia.Data;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using System;
using System.Linq;

namespace ExamAvalonia.Windows
{
    public partial class AddEditShipmentWindow : Window
    {
        private Shipment? shipment;

        public AddEditShipmentWindow()
        {
            InitializeComponent();
            LoadWarehouses();
        }

        public AddEditShipmentWindow(Shipment s) : this()
        {
            shipment = s;

            WarehouseBox.SelectedItem = App.DbContext.Warehouses.Find(shipment.WarehouseId);
            ShipmentDatePicker.SelectedDate = new DateTimeOffset(shipment.ShipmentDate.ToDateTime(TimeOnly.MinValue));
            RecipientBox.Text = shipment.Recipient;
            StatusBox.Text = shipment.Status;
        }

        private void LoadWarehouses()
        {
            var warehouses = App.DbContext.Warehouses.OrderBy(w => w.Name).ToList();
            WarehouseBox.ItemsSource = warehouses;
        }

        private async void Save_Click(object? sender, RoutedEventArgs e)
        {
            if (WarehouseBox.SelectedItem == null ||
                ShipmentDatePicker.SelectedDate == null ||
                string.IsNullOrWhiteSpace(RecipientBox.Text) ||
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

            if (shipment == null)
            {
                shipment = new Shipment
                {
                    WarehouseId = ((Warehouse)WarehouseBox.SelectedItem).WarehouseId,
                    ShipmentDate = DateOnly.FromDateTime(ShipmentDatePicker.SelectedDate.Value.DateTime),
                    Recipient = RecipientBox.Text,
                    Status = StatusBox.Text
                };
                App.DbContext.Shipments.Add(shipment);
            }
            else
            {
                shipment.WarehouseId = ((Warehouse)WarehouseBox.SelectedItem).WarehouseId;
                shipment.ShipmentDate = DateOnly.FromDateTime(ShipmentDatePicker.SelectedDate.Value.DateTime);
                shipment.Recipient = RecipientBox.Text;
                shipment.Status = StatusBox.Text;

                App.DbContext.Shipments.Update(shipment);
            }

            App.DbContext.SaveChanges();
            Close();
        }

        private void Cancel_Click(object? sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
