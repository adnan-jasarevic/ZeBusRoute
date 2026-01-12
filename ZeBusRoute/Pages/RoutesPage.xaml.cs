using System.Diagnostics;
using ZeBusRoute.Models;
using ZeBusRoute.Services;
using ZeBusRoute.ViewModels;

namespace ZeBusRoute.Pages;

public partial class RoutesPage : ContentPage
{
    public RoutesPage()
    {
        InitializeComponent();
        // BindingContext je postavljen u xamlu u RoutesViewModel
        // viewmodel automatski ucitava podatke
    }
}
