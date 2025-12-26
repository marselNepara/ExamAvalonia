using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ExamAvalonia.Data;

namespace ExamAvalonia.Windows;

public partial class AddEditSupplier : Window
{
    private Supplier _currentSupplier;
    private bool _isEdit;

    public AddEditSupplier()
    {
        InitializeComponent();
        _currentSupplier = new Supplier();
        _isEdit = false;
        TitleText.Text = "Add Supplier";
    }

    public AddEditSupplier(Supplier supplier)
    {
        InitializeComponent();
        _currentSupplier = supplier;
        _isEdit = true;
        TitleText.Text = "Edit Supplier";

        NameBox.Text = supplier.Name;
        PhoneBox.Text = supplier.Phone;
        ContactBox.Text = supplier.ContactPerson;
        EmailBox.Text = supplier.Email;
    }

    private void Save_Click(object? sender, RoutedEventArgs e)
    {
        _currentSupplier.Name = NameBox.Text;
        _currentSupplier.Phone = PhoneBox.Text;
        _currentSupplier.ContactPerson = ContactBox.Text;
        _currentSupplier.Email = EmailBox.Text;

        if (!_isEdit)
            App.DbContext.Suppliers.Add(_currentSupplier);
        else
            App.DbContext.Suppliers.Update(_currentSupplier);

        App.DbContext.SaveChanges();
        Close(true);
    }

    private void Cancel_Click(object? sender, RoutedEventArgs e)
    {
        Close(false);
    }
}
