using Avalonia.Controls;
using Avalonia.Interactivity;
using ExamAvalonia.Data;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using System.Linq;

namespace ExamAvalonia.Windows;

public partial class AddEditDeliveryItemWindow : Window
{
    public DeliveryItemViewModel? Item { get; private set; }

    public AddEditDeliveryItemWindow()
    {
        InitializeComponent();
        LoadProducts();
    }

    private void LoadProducts()
    {
        var products = App.DbContext.Products
            .OrderBy(p => p.Name)
            .ToList();

        ProductBox.ItemsSource = products;
        ProductBox.SelectedIndex = -1;
    }

    private async void Ok_Click(object? sender, RoutedEventArgs e)
    {
        if (ProductBox.SelectedItem == null || QuantityBox.Value <= 0 || PriceBox.Value < 0)
        {
            var msg = MessageBoxManager.GetMessageBoxStandard(
                "Error",
                "Please fill all required fields (*) correctly.",
                ButtonEnum.Ok,
                MsBox.Avalonia.Enums.Icon.Error);
            await msg.ShowWindowDialogAsync(this);
            return;
        }

        var product = ProductBox.SelectedItem as Product;

        Item = new DeliveryItemViewModel
        {
            ProductId = product.ProductId,
            ProductName = product.Name,
            Quantity = (int)QuantityBox.Value,
            PurchasePrice = (decimal)PriceBox.Value
        };

        Close();
    }

    private void Cancel_Click(object? sender, RoutedEventArgs e)
    {
        Item = null;
        Close();
    }
}
