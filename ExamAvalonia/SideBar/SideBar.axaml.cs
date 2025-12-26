using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ExamAvalonia.Pages;
namespace ExamAvalonia;

public partial class SideBar : UserControl
{
    public SideBar()
    {
        InitializeComponent();
    }

    private void Products_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        MainWindow.Instance.MainControl.Content = new Products();
    }

    private void Warehouses_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        MainWindow.Instance.MainControl.Content = new WarehousesAndZones();
    }

    private void Suppliers_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        MainWindow.Instance.MainControl.Content = new Suppliers();
    }

    private void Deliveries_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        MainWindow.Instance.MainControl.Content = new DeliveriesPage();
    }

    private void Shipments_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        MainWindow.Instance.MainControl.Content = new ShipmentsPage();
    }
}