using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using ExamAvalonia.Pages;
namespace ExamAvalonia;

public partial class SideBar : UserControl
{
    private Button _activeButton;

    public SideBar()
    {
        InitializeComponent();
    }

    private void SetActive(Button btn)
    {
        // —брос предыдущей кнопки
        if (_activeButton != null)
            _activeButton.Background = Brushes.Gray;

        _activeButton = btn;
        _activeButton.Background = Brushes.DodgerBlue;
        _activeButton.Foreground = Brushes.White;
    }

    private void Products_Click(object? sender, RoutedEventArgs e)
    {
        MainWindow.Instance.MainControl.Content = new Products();
        SetActive(ProductsBtn);
    }

    private void Warehouses_Click(object? sender, RoutedEventArgs e)
    {
        MainWindow.Instance.MainControl.Content = new WarehousesAndZones();
        SetActive(WarehousesBtn);
    }

    private void Suppliers_Click(object? sender, RoutedEventArgs e)
    {
        MainWindow.Instance.MainControl.Content = new Suppliers();
        SetActive(SuppliersBtn);
    }

    private void Deliveries_Click(object? sender, RoutedEventArgs e)
    {
        MainWindow.Instance.MainControl.Content = new DeliveriesPage();
        SetActive(DeliveriesBtn);
    }

    private void Shipments_Click(object? sender, RoutedEventArgs e)
    {
        MainWindow.Instance.MainControl.Content = new ShipmentsPage();
        SetActive(ShipmentsBtn);
    }
}