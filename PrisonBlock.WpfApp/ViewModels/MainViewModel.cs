using PrisonBlock.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Web;
using System.Linq;

namespace PrisonBlock.WpfApp.ViewModels;

public sealed class MainViewModel : INotifyPropertyChanged
{
    private readonly PrisonService _service = new();

    private string _selectedRepositoryType = nameof(Rookie);
    private string _newName = "";
    private string _searchText = "";

    private Prisoner? _selectedPrisoner;

    public event PropertyChangedEventHandler? PropertyChanged;

    public ObservableCollection<string> RepositoryTypes { get; }
    public ObservableCollection<Prisoner> Prisoners { get; } = new();
    public ObservableCollection<Prisoner> FilteredPrisoners { get; } = new();
    public ObservableCollection<Prisoner> SortedPrisoners { get; } = new();
    public ObservableCollection<string> Summary { get; } = new();
    public ObservableCollection<string> Log { get; } = new();

    public RelayCommand AddCommand { get; }
    public RelayCommand RemoveCommand { get; }
    public RelayCommand CrazyActionCommand { get; }
    public RelayCommand WorkCommand { get; }
    public RelayCommand StudyCommand { get; }
    public RelayCommand ChessCommand { get; }

    public string SelectedRepositoryType
    {
        get => _selectedRepositoryType;
        set
        {
            if (_selectedRepositoryType == value)
                return;

            _selectedRepositoryType = value;
            OnPropertyChanged();
            RefreshPrisoners();
        }
    }

    public string NewName
    {
        get => _newName;
        set
        {
            if (_newName == value)
                return;

            _newName = value;
            OnPropertyChanged();
            AddCommand.RaiseCanExecuteChanged();
        }
    }

    public string SearchText
    {
        get => _searchText;
        set
        {
            if (_searchText == value)
                return;

            _searchText = value;
            OnPropertyChanged();
            RefreshQueries();
        }
    }

    public Prisoner? SelectedPrisoner
    {
        get => _selectedPrisoner;
        set
        {
            if (ReferenceEquals(_selectedPrisoner, value))
                return;

            _selectedPrisoner = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(SelectedDetails));
            OnPropertyChanged(nameof(SelectedEnergy));

            RemoveCommand.RaiseCanExecuteChanged();
            CrazyActionCommand.RaiseCanExecuteChanged();
            WorkCommand.RaiseCanExecuteChanged();
            StudyCommand.RaiseCanExecuteChanged();
            ChessCommand.RaiseCanExecuteChanged();
        }
    }

    public string SelectedDetails => SelectedPrisoner?.GetDetails() ?? "Vangi ei valitud";

    public int SelectedEnergy => SelectedPrisoner?.Energy ?? 0;

    public MainViewModel()
    {
        RepositoryTypes = new ObservableCollection<string>(_service.RepositoryTypes);

        AddCommand = new RelayCommand(Add, CanAdd);
        RemoveCommand = new RelayCommand(Remove, HasSelection);
        CrazyActionCommand = new RelayCommand(CrazyAction, HasSelection);
        WorkCommand = new RelayCommand(Work, () => SelectedPrisoner is IWork);
        StudyCommand = new RelayCommand(Study, () => SelectedPrisoner is IStudy);
        ChessCommand = new RelayCommand(Chess, () => SelectedPrisoner is IPlayChess);

        _service.LogGenerated += AddLog;

        AddStarterPrisoners();
        RefreshPrisoners();
    }

    private bool HasSelection() => SelectedPrisoner is not null;

    private bool CanAdd() => !string.IsNullOrWhiteSpace(NewName) && NewName.Trim().Length <= 20;

    private void AddStarterPrisoners()
    {
        _service.Add(new Rookie("Rookie"));
        _service.Add(new Philosopher("Philosopher"));
        _service.Add(new Artist("Artist"));
        _service.Add(new ChessMaster("ChessMaster"));
        _service.Add(new Chef("Chef"));
    }

    private void Add()
    {
        Prisoner? prisoner = PrisonFactory.Create(SelectedRepositoryType, NewName, out string error);

        if (prisoner is null)
        {
            AddLog(error);
            return;
        }

        _service.Add(prisoner);
        NewName = "";
        RefreshPrisoners();
        SelectedPrisoner = prisoner;
    }

    private void Remove()
    {
        if (SelectedPrisoner is null)
            return;

        Prisoner removed = SelectedPrisoner;
        _service.Remove(removed);

        SelectedPrisoner = null;
        RefreshPrisoners();

        AddLog($"{removed.Name} eemaldati plokist 13");
    }

    private void CrazyAction()
    {
        if (SelectedPrisoner is null)
            return;

        AddLog(_service.PerformCrazyAction(SelectedPrisoner));
        RefreshAfterAction();
    }

    private void Work()
    {
        AddLog(_service.PerformWork(SelectedPrisoner));
        RefreshAfterAction();
    }

    private void Study()
    {
        AddLog(_service.PerformStudy(SelectedPrisoner));
        RefreshAfterAction();
    }

    private void Chess()
    {
        AddLog(_service.PerformChess(SelectedPrisoner));
        RefreshAfterAction();
    }
    private void RefreshAfterAction()
    {
        Prisoner? selected = SelectedPrisoner;

        RefreshQueries();

        SelectedPrisoner = selected;

        OnPropertyChanged(nameof(SelectedDetails));
        OnPropertyChanged(nameof(SelectedEnergy));
    }

    private void RefreshPrisoners()
    {
        Prisoners.Clear();

        foreach (Prisoner prisoner in _service.GetByType(SelectedRepositoryType))
        {
            Prisoners.Add(prisoner);
        }

        SelectedPrisoner = null;
        RefreshQueries();
    }

    private void RefreshQueries()
    {
        FilteredPrisoners.Clear();

        IEnumerable<Prisoner> filtered = Prisoners;

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            filtered = filtered.Where(p => p.Name.Contains(SearchText.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        foreach (Prisoner prisoner in filtered)
        {
            FilteredPrisoners.Add(prisoner);
        }

        SortedPrisoners.Clear();

        foreach (Prisoner prisoner in Prisoners.OrderByDescending(p => p.Energy))
        {
            SortedPrisoners.Add(prisoner);
        }

        Summary.Clear();

        foreach (var group in _service.GetAll().GroupBy(p => p.GetType().Name).OrderBy(g => g.Key))
        {
            Summary.Add($"{group.Key}: {group.Count()}");
        }
    }

    private void AddLog(string message)
    {
        Log.Add($"[{DateTime.Now:HH:mm:ss}] {message}");
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

