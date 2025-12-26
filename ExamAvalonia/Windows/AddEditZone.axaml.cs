using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ExamAvalonia.Data;

namespace ExamAvalonia.Windows;

public partial class AddEditZone : Window
{
    private StorageZone _currentZone;
    private bool _isEdit;

    public int WarehouseId { get; set; }

    public AddEditZone(int warehouseId)
    {
        InitializeComponent();
        _currentZone = new StorageZone();
        _currentZone.WarehouseId = warehouseId;
        _isEdit = false;
        TitleText.Text = "Add Zone";
        WarehouseId = warehouseId;
    }
    public AddEditZone(StorageZone zone)
    {
        InitializeComponent();
        _currentZone = zone;
        _isEdit = true;
        TitleText.Text = "Edit Zone";
        CodeBox.Text = zone.ZoneCode;
        DescriptionBox.Text = zone.Description;
        WarehouseId = zone.WarehouseId;
    }

    private void Save_Click(object? sender, RoutedEventArgs e)
    {
        _currentZone.ZoneCode = CodeBox.Text;
        _currentZone.Description = DescriptionBox.Text;
        _currentZone.WarehouseId = WarehouseId;

        if (!_isEdit)
            App.DbContext.StorageZones.Add(_currentZone);
        else
            App.DbContext.StorageZones.Update(_currentZone);

        App.DbContext.SaveChanges();
        Close(true);
    }

    private void Cancel_Click(object? sender, RoutedEventArgs e)
    {
        Close(false);
    }
}