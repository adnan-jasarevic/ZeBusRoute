using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using ZeBusRoute.Models;

namespace ZeBusRoute.ViewModels;

public class TicketsViewModel : INotifyPropertyChanged
{
    private int _selectedTabIndex;
    
    public event PropertyChangedEventHandler? PropertyChanged;

    public ObservableCollection<TicketType> DostupneKarte { get; set; }
    public ObservableCollection<UserTicket> MojeKarte { get; set; }

    public int SelectedTabIndex
    {
        get => _selectedTabIndex;
        set
        {
            if (_selectedTabIndex != value)
            {
                _selectedTabIndex = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(BuyTicketsTabColor));
                OnPropertyChanged(nameof(MyTicketsTabColor));
                OnPropertyChanged(nameof(IsBuyTicketsTabVisible));
                OnPropertyChanged(nameof(IsMyTicketsTabVisible));
                OnPropertyChanged(nameof(BuyTicketsUnderlineVisible));
                OnPropertyChanged(nameof(MyTicketsUnderlineVisible));
            }
        }
    }

    // Tab Colors
    public Color BuyTicketsTabColor => SelectedTabIndex == 0 ? Color.FromArgb("#8BC34A") : Color.FromArgb("#9E9E9E");
    public Color MyTicketsTabColor => SelectedTabIndex == 1 ? Color.FromArgb("#8BC34A") : Color.FromArgb("#9E9E9E");

    // Tab Visibility
    public bool IsBuyTicketsTabVisible => SelectedTabIndex == 0;
    public bool IsMyTicketsTabVisible => SelectedTabIndex == 1;
    
    // Underline Visibility
    public bool BuyTicketsUnderlineVisible => SelectedTabIndex == 0;
    public bool MyTicketsUnderlineVisible => SelectedTabIndex == 1;

    public ICommand PurchaseTicketCommand { get; }
    public ICommand ChangeTabCommand { get; }

    public TicketsViewModel()
    {
        DostupneKarte = new ObservableCollection<TicketType>
        {
            new TicketType 
            { 
                Ime = "Jednokratna karta", 
                Opis = "Vazi 30 minuta", 
                Cijena = 2.00m 
            },
            new TicketType 
            { 
                Ime = "Dnevna karta", 
                Opis = "Beskonacan broj voznji na 12 sati", 
                Cijena = 8.00m 
            },
            new TicketType 
            { 
                Ime = "Mjesecna karta", 
                Opis = "Beskonacan broj voznji na 30 dana", 
                Cijena = 50.00m 
            }
        };

        MojeKarte = new ObservableCollection<UserTicket>
        {
            new UserTicket 
            { 
                Id = "T001", 
                Type = "Monthly Pass", 
                ValidUntil = new DateTime(2026, 2, 5),
                QrCodeData = "TICKET_T001_MONTHLY"
            },
            new UserTicket 
            { 
                Id = "T002", 
                Type = "Day Pass", 
                ValidUntil = new DateTime(2026, 1, 6),
                QrCodeData = "TICKET_T002_DAY"
            }
        };

        PurchaseTicketCommand = new Command<TicketType>(OnPurchaseTicket);
        ChangeTabCommand = new Command<string>(OnChangeTab);
    }

    private void OnChangeTab(string tabIndex)
    {
        if (int.TryParse(tabIndex, out int index))
        {
            SelectedTabIndex = index;
        }
    }

    private async void OnPurchaseTicket(TicketType? ticket)
    {
        if (ticket == null) return;
        
        // treba implementirati logiku kupovine
        if (Application.Current?.Windows.Count > 0)
        {
            var mainPage = Application.Current.Windows[0].Page;
            await mainPage.DisplayAlertAsync(
                "Kupi", 
                $"Kupite kartu {ticket.Ime} za {ticket.CijenaDisplay}", 
                "OK");
        }
    }

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}