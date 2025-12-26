using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ExamAvalonia.Data;

namespace ExamAvalonia.Windows;

public partial class AddEditWarehouse : Window
{
    private Warehouse _currentWarehouse;
    private bool _isEdit;
    public AddEditWarehouse()
    {
        InitializeComponent();
        _currentWarehouse = new Warehouse();
        _isEdit = false;
        TitleText.Text = "Add Warehouse";
    }

    public AddEditWarehouse(Warehouse warehouse)
    {
        InitializeComponent();
        _currentWarehouse = warehouse;
        _isEdit = true;
        TitleText.Text = "Edit Warehouse";
        NameBox.Text = warehouse.Name;
        AddressBox.Text = warehouse.Address;
    }

    private void Save_Click(object? sender, RoutedEventArgs e)
    {
        _currentWarehouse.Name = NameBox.Text;
        _currentWarehouse.Address = AddressBox.Text;

        if (!_isEdit)
            App.DbContext.Warehouses.Add(_currentWarehouse);
        else
            App.DbContext.Warehouses.Update(_currentWarehouse);

        App.DbContext.SaveChanges();
        Close(true);
    }

    private void Cancel_Click(object? sender, RoutedEventArgs e)
    {
        Close(false);
    }
}