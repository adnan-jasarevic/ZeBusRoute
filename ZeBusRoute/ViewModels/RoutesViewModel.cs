using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using ZeBusRoute.Models;
using ZeBusRoute.Services;

namespace ZeBusRoute.ViewModels;

public class RoutesViewModel : INotifyPropertyChanged
{
    private string _odabraniTipDana = "RadniDan";
    private string _odabranoVrijemeUDanu = "Jutro";
    private Linija? _odabranaLinija;
    private bool _jeRasporedVidljiv;

    public event PropertyChangedEventHandler? PropertyChanged;

    public ObservableCollection<Linija> SveLinije { get; set; } = new();
    public ObservableCollection<Linija> FiltriraneLinije { get; set; } = new();
    public ObservableCollection<PolasakSaStanicom> Polasci { get; set; } = new();

    public string OdabraniTipDana
    {
        get => _odabraniTipDana;
        set
        {
            if (_odabraniTipDana != value)
            {
                _odabraniTipDana = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(RadniDanBojaPositine));
                OnPropertyChanged(nameof(RadniDanBojaTeksta));
                OnPropertyChanged(nameof(VikendBojaPositine));
                OnPropertyChanged(nameof(VikendBojaTeksta));
                OnPropertyChanged(nameof(PraznikBojaPositine));
                OnPropertyChanged(nameof(PraznikBojaTeksta));
                FilterLinije();
            }
        }
    }

    public string OdabranoVrijemeUDanu
    {
        get => _odabranoVrijemeUDanu;
        set
        {
            if (_odabranoVrijemeUDanu != value)
            {
                _odabranoVrijemeUDanu = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(JutroBojaPositine));
                OnPropertyChanged(nameof(JutroBojaTeksta));
                OnPropertyChanged(nameof(PodneBojaPositine));
                OnPropertyChanged(nameof(PodneBojaTeksta));
                OnPropertyChanged(nameof(VecerBojaPositine));
                OnPropertyChanged(nameof(VecerBojaTeksta));
                FilterLinije();
            }
        }
    }

    // Boje za Tip Dana
    public Color RadniDanBojaPositine => OdabraniTipDana == "RadniDan" ? Color.FromArgb("#8BC34A") : Color.FromArgb("#E0E0E0");
    public Color RadniDanBojaTeksta => OdabraniTipDana == "RadniDan" ? Colors.White : Color.FromArgb("#757575");
    
    public Color VikendBojaPositine => OdabraniTipDana == "Vikend" ? Color.FromArgb("#8BC34A") : Color.FromArgb("#E0E0E0");
    public Color VikendBojaTeksta => OdabraniTipDana == "Vikend" ? Colors.White : Color.FromArgb("#757575");
    
    public Color PraznikBojaPositine => OdabraniTipDana == "Praznik" ? Color.FromArgb("#8BC34A") : Color.FromArgb("#E0E0E0");
    public Color PraznikBojaTeksta => OdabraniTipDana == "Praznik" ? Colors.White : Color.FromArgb("#757575");

    // Boje za Vrijeme Dana
    public Color JutroBojaPositine => OdabranoVrijemeUDanu == "Jutro" ? Color.FromArgb("#8BC34A") : Color.FromArgb("#E0E0E0");
    public Color JutroBojaTeksta => OdabranoVrijemeUDanu == "Jutro" ? Colors.White : Color.FromArgb("#757575");
    
    public Color PodneBojaPositine => OdabranoVrijemeUDanu == "Podne" ? Color.FromArgb("#8BC34A") : Color.FromArgb("#E0E0E0");
    public Color PodneBojaTeksta => OdabranoVrijemeUDanu == "Podne" ? Colors.White : Color.FromArgb("#757575");
    
    public Color VecerBojaPositine => OdabranoVrijemeUDanu == "Vecer" ? Color.FromArgb("#8BC34A") : Color.FromArgb("#E0E0E0");
    public Color VecerBojaTeksta => OdabranoVrijemeUDanu == "Vecer" ? Colors.White : Color.FromArgb("#757575");

    public Linija? OdabranaLinija
    {
        get => _odabranaLinija;
        set
        {
            if (_odabranaLinija != value)
            {
                _odabranaLinija = value;
                OnPropertyChanged();
                if (value != null)
                {
                    UcitajRaspored(value);
                }
            }
        }
    }

    public bool JeRasporedVidljiv
    {
        get => _jeRasporedVidljiv;
        set
        {
            if (_jeRasporedVidljiv != value)
            {
                _jeRasporedVidljiv = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(JeListaLinijaVidljiva));
            }
        }
    }

    public bool JeListaLinijaVidljiva => !JeRasporedVidljiv;

    public ICommand OdaberiTipDanaCommand { get; }
    public ICommand OdaberiVrijemeUDanu { get; }
    public ICommand OdaberiLinijuCommand { get; }
    public ICommand VratiSeNaLinijeCommand { get; }

    public RoutesViewModel()
    {
        OdaberiTipDanaCommand = new Command<string>(PriOdabiruTipaDana);
        OdaberiVrijemeUDanu = new Command<string>(PriOdabiruVremenaUDanu);
        OdaberiLinijuCommand = new Command<Linija>(PriOdabiruLinije);
        VratiSeNaLinijeCommand = new Command(PriPovratkuNaLinije);

        UcitajLinije();
    }

    private void UcitajLinije()
    {
        try
        {
            DataService.InitDb();
            var linije = DataService.GetLinije();

            SveLinije = new ObservableCollection<Linija>(linije);
            FilterLinije();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Greška pri učitavanju linija: {ex.Message}");
        }
    }

    private void FilterLinije()
    {
        FiltriraneLinije.Clear();
        
        // Ovdje možete dodati logiku filtriranja po danu i vremenu
        // Za sada prikazujemo sve linije
        foreach (var linija in SveLinije)
        {
            FiltriraneLinije.Add(linija);
        }
    }

    private void UcitajRaspored(Linija linija)
    {
        try
        {
            Polasci.Clear();
            var polasci = DataService.GetPolasci(linija.Id);
            var stanice = DataService.GetStanice(linija.Id);

            // Filter polazaka po vremenu dana
            var filtriraniPolasci = polasci.Where(p => FiltrirajPoVremenuDana(p.Vrijeme)).ToList();

            foreach (var polazak in filtriraniPolasci)
            {
                var stanica = stanice.FirstOrDefault();
                Polasci.Add(new PolasakSaStanicom
                {
                    Vrijeme = polazak.Vrijeme,
                    Stanica = stanica?.Naziv ?? "Centar",
                    Status = "Na vrijeme"
                });
            }

            JeRasporedVidljiv = true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Greška pri učitavanju rasporeda: {ex.Message}");
        }
    }

    private bool FiltrirajPoVremenuDana(TimeOnly vrijeme)
    {
        int sat = vrijeme.Hour;
        return OdabranoVrijemeUDanu switch
        {
            "Jutro" => sat >= 5 && sat < 12,
            "Podne" => sat >= 12 && sat < 17,
            "Vecer" => sat >= 17 || sat < 5,
            _ => true
        };
    }

    private void PriOdabiruTipaDana(string tipDana)
    {
        OdabraniTipDana = tipDana;
    }

    private void PriOdabiruVremenaUDanu(string vrijemeUDanu)
    {
        OdabranoVrijemeUDanu = vrijemeUDanu;
    }

    private void PriOdabiruLinije(Linija linija)
    {
        OdabranaLinija = linija;
    }

    private void PriPovratkuNaLinije()
    {
        JeRasporedVidljiv = false;
        OdabranaLinija = null;
    }

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}