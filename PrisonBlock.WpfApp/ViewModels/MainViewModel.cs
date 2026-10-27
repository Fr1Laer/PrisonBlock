using PrisonBlock.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Web;

namespace PrisonBlock.WpfApp.ViewModels;

public sealed class MainViewModel : INotifyPropertyChanged
{
    private readonly PrisonService _service = new();

    private int _selectedWorkload = 500000;
    private bool _isRunning;

    // Arvutuste valikuid saab muuta ainult töövälisel ajal
    public bool CanEditComputations => !IsRunning;
    private string _computationStatus = "Käivitamise ootamine";

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

    // Eeltöötluse objektide valik ja aruande kogumine
    public ObservableCollection<PrisonerChoice> ComputationPrisoners { get; } = new();
    public ObservableCollection<int> WorkloadOptions { get; } =
        new() { 100000, 500000, 1000000, 3000000, 1000000000};

    public ObservableCollection<string> ComputationReport { get; } = new();

    public RelayCommand AddCommand { get; }
    public RelayCommand RemoveCommand { get; }
    public RelayCommand CrazyActionCommand { get; }
    public RelayCommand WorkCommand { get; }
    public RelayCommand StudyCommand { get; }
    public RelayCommand ChessCommand { get; }

    public RelayCommand RunSequentialCommand { get; }
    public RelayCommand RunParallelCommand { get; }
    public RelayCommand CancelCommand { get; }

    // Salvestab iga objekti katkestamise tokeni
    private readonly Dictionary<string, CancellationTokenSource>
        _objectCancellationSources = new();

    // Salvestab objektid, mille arvutus on juba tühistatud
    private readonly HashSet<string> _cancelledObjects = new();
        
    // Tagastab arvutuste objektide nimed

    public IEnumerable<string> CancellationTargets => _objectCancellationSources.Keys.Where(name => !_cancelledObjects.Contains(name));

    private string? _selectedCancellationTarget;

    public string? SelectedCancellationTarget
    {
        get => _selectedCancellationTarget;
        set
        {
            if (_selectedCancellationTarget == value)
                return;

            _selectedCancellationTarget = value;
            OnPropertyChanged();
        }
    }


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

    public int SelectedWorkload
    {
        get => _selectedWorkload;
        set
        {
            if (_selectedWorkload == value)
                return;

            _selectedWorkload = value;
            OnPropertyChanged();
        }
    }

    public bool IsRunning
    {
        get => _isRunning;
        private set
        {
            if (_isRunning == value)
                return;

            _isRunning = value;

            // Uuendab arvutuste käivitamise olekut
            OnPropertyChanged(nameof(IsRunning));

            // Uuendab arvutuste valikute lubatud olekut
            OnPropertyChanged(nameof(CanEditComputations));

            RunSequentialCommand.RaiseCanExecuteChanged();
            RunParallelCommand.RaiseCanExecuteChanged();
            CancelCommand.RaiseCanExecuteChanged();
        }
    }

    public string ComputationStatus
    {
        get => _computationStatus;
        private set
        {
            if (_computationStatus == value)
                return;

            _computationStatus = value;
            OnPropertyChanged();
        }
    }
    public MainViewModel()
    {
        RepositoryTypes = new ObservableCollection<string>(_service.RepositoryTypes);

        AddCommand = new RelayCommand(Add, CanAdd);
        RemoveCommand = new RelayCommand(Remove, HasSelection);
        CrazyActionCommand = new RelayCommand(CrazyAction, HasSelection);
        WorkCommand = new RelayCommand(Work, () => SelectedPrisoner is IWork);
        StudyCommand = new RelayCommand(Study, () => SelectedPrisoner is IStudy);
        ChessCommand = new RelayCommand(Chess, () => SelectedPrisoner is IPlayChess);

        RunSequentialCommand = new RelayCommand(async () => await RunComputationsAsync(false), () => !IsRunning);
        RunParallelCommand = new RelayCommand(async () => await RunComputationsAsync(true), () => !IsRunning);
        CancelCommand = new RelayCommand(CancelComputations, () => IsRunning);

        _service.LogGenerated += AddLog;

        AddStarterPrisoners();
        RefreshPrisoners();

        RefreshComputationPrisoners();
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

        /// new prisoners
        _service.Add(new Gardener("Gardener"));
        _service.Add(new Musician("Musician"));
        _service.Add(new Gambler("Gambler"));
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

        RefreshComputationPrisoners();

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

        RefreshComputationPrisoners();

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

    // Valmistab arvutusteks valitavate vangide nimekirja

    private bool _computationSelectionInitialized;

    private void RefreshComputationPrisoners()
    {
        HashSet<string> previouslySelected = ComputationPrisoners
            .Where(p => p.IsSelected)
            .Select(p => p.Name)
            .ToHashSet(StringComparer.Ordinal);

        bool initializeSelection = !_computationSelectionInitialized;
        int index = 0;

        ComputationPrisoners.Clear();

        foreach (Prisoner prisoner in _service.GetAll())
        {
            bool isSelected;

            if (initializeSelection)
            {
                // Vaikimisi valib rakendus kolm esimest vangi
                isSelected = index < 3;
            }
            else
            {
                isSelected = previouslySelected.Contains(prisoner.Name);
            }

            ComputationPrisoners.Add(
                new PrisonerChoice(prisoner, isSelected));

            index++;
        }

        _computationSelectionInitialized = true;

        // Valib vaikimisi esimese valitud objekti
        if (SelectedCancellationTarget is null ||
            !CancellationTargets.Contains(SelectedCancellationTarget))
        {
            SelectedCancellationTarget = CancellationTargets.FirstOrDefault();
        }
    }


    // Käivitab mõlema režiimi arvutused samade andmesnapshot'idega

    private async Task RunComputationsAsync(bool parallel)
    {
        if (IsRunning)
            return;

        // Lähtestab eelmise arvutuse tühistatud objektid
        _cancelledObjects.Clear();

        List<PrisonerChoice> selected = ComputationPrisoners
            .Where(p => p.IsSelected)
            .ToList();

        if (selected.Count < 3)
        {
            ComputationStatus =
                "Valige arvutuseks vähemalt kolm vangi";
            return;
        }

        // Salvestab lähteandmed enne arvutuste käivitamist
        List<PrisonerSnapshot> snapshots = selected
            .Select(p => new PrisonerSnapshot(
                p.Name,
                p.Type,
                p.Energy))
            .ToList();

        int iterations = SelectedWorkload;

        // Loob igale objektile eraldi katkestamise allika
        foreach (CancellationTokenSource source
                 in _objectCancellationSources.Values)
        {
            source.Dispose();
        }

        _objectCancellationSources.Clear();

        foreach (PrisonerSnapshot snapshot in snapshots)
        {
            // Iga objekt saab oma katkestamise allika
            _objectCancellationSources[snapshot.Name] =
                new CancellationTokenSource();
        }

        // Uuendab tühistamise sihtobjekti valikut
        OnPropertyChanged(nameof(CancellationTargets));

        if (SelectedCancellationTarget is null ||
            !_objectCancellationSources.ContainsKey(SelectedCancellationTarget))
        {
            SelectedCancellationTarget = snapshots.First().Name;
        }

        IsRunning = true;

        ComputationReport.Clear();

        // Valib vaikimisi esimese tühistamise sihtobjekti
        SelectedCancellationTarget = CancellationTargets.FirstOrDefault();

        ComputationStatus = parallel
            ? "Paralleelarvutus käib..."
            : "Järjestikune arvutus käib...";

        AddLog(ComputationStatus);

        Stopwatch totalStopwatch = Stopwatch.StartNew();

        try
        {
            if (parallel)
            {
                // Käivitab iga objekti eraldi ülesandes
                Task<ComputationResult>[] tasks = snapshots
                   .Select(snapshot => RunOneAsync(
                       snapshot,
                       iterations,
                       _objectCancellationSources[snapshot.Name].Token))
                   .ToArray();

                ComputationResult[] results = await Task.WhenAll(tasks);

                foreach (ComputationResult result in results)
                    ComputationReport.Add(result.ToReportLine());
            }
            else
            {
                // Käivitab järgmise arvutuse pärast eelmise lõppu
                foreach (PrisonerSnapshot snapshot in snapshots)
                {
                    ComputationResult result = await RunOneAsync(
                        snapshot,
                        iterations,
                        _objectCancellationSources[snapshot.Name].Token);

                    ComputationReport.Add(result.ToReportLine());
                }
            }

            ComputationStatus = "Arvutus lõpetatud";
        }
        catch (OperationCanceledException)
        {
            // Käsitleb kasutaja algatatud arvutuse tühistamist
            ComputationStatus = "Arvutus tühistati";
            ComputationReport.Add(ComputationStatus);
        }
        catch (Exception ex)
        {
            // Salvestab ootamatu arvutusvea
            ComputationStatus = $"Arvutuse viga: {ex.Message}";
            ComputationReport.Add(ComputationStatus);
            AddLog(ComputationStatus);
        }
        finally
        {
            // Peatab koguaega mõõtva kella
            totalStopwatch.Stop();

            ComputationReport.Add(
                $"Koguaeg: {totalStopwatch.Elapsed.TotalMilliseconds:F2} ms");

            AddLog(ComputationStatus);

            // Vabastab iga objekti katkestamise allika
            foreach (CancellationTokenSource source
                     in _objectCancellationSources.Values)
            {
                source.Dispose();
            }

            _objectCancellationSources.Clear();

            // Uuendab tühistamise sihtobjektide loendit
            OnPropertyChanged(nameof(CancellationTargets));

            // Lähtestab valitud tühistamise sihtobjekti
            SelectedCancellationTarget = null;

            // Taastab kasutajaliidese tööoleku
            IsRunning = false;
        }
    }


    // Iga objekti vead ja tühistamine töödeldakse eraldi
    private static async Task<ComputationResult> RunOneAsync(
        PrisonerSnapshot snapshot,
        int iterations,
        CancellationToken token)

    {
        Stopwatch stopwatch = Stopwatch.StartNew();

        try
        {
            long value = await Task.Run(
                () => PerformCpuCalculation(snapshot, iterations, token));

            stopwatch.Stop();

            return new ComputationResult(
                snapshot.Name,
                "Õnnestus",
                $"Tulemus: {value}",
                stopwatch.Elapsed);
        }
        catch (OperationCanceledException)
        {
            stopwatch.Stop();

            return new ComputationResult(
                snapshot.Name,
                "Tühistatud",
                "Arvutus katkestati kasutaja soovil",
                stopwatch.Elapsed);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            return new ComputationResult(
                snapshot.Name,
                "Viga",
                ex.Message,
                stopwatch.Elapsed);
        }
    }

    // CPU-põhine korduv simulatsioon, mis kasutab ainult snapshot'i andmeid
    private static long PerformCpuCalculation(
        PrisonerSnapshot snapshot,
        int iterations,
        CancellationToken token)
    {
        uint state = 2166136261;

        unchecked
        {
            foreach (char character in snapshot.Name)
            {
                state ^= character;
                state *= 16777619;
            }

            foreach (char character in snapshot.Type)
            {
                state ^= character;
                state *= 16777619;
            }

            state ^= (uint)snapshot.Energy;
            state *= 16777619;

            if (state == 0)
                state = 1;

            ulong checksum = 0;

            for (int i = 0; i < iterations; i++)
            {
                // Kontrollib tühistamist regulaarselt CPU-tsüklis
                if ((i & 4095) == 0)
                    token.ThrowIfCancellationRequested();

                state ^= state << 13;
                state ^= state >> 17;
                state ^= state << 5;

                checksum += (ulong)(state ^ (uint)i);
                checksum = (checksum << 7) |
                           (checksum >> 57);
            }

            token.ThrowIfCancellationRequested();

            return (long)(checksum & long.MaxValue);
        }
    }

    // Katkestab valitud objekti arvutuse ainult ühe korra
    private void CancelComputations()
    {
        if (SelectedCancellationTarget is not string name)
            return;

        if (_cancelledObjects.Contains(name))
            return;

        if (_objectCancellationSources.TryGetValue(name, out var source))
        {
            source.Cancel();
            _cancelledObjects.Add(name);

            // Uuendab tühistatud objektide kuvamist
            OnPropertyChanged(nameof(CancellationTargets));

            ComputationStatus =
                $"Objekti {name} arvutuse tühistamine...";
        }
    }

    // Arvutuse jaoks kasutatav muutumatu andmesnapshot
    private sealed record PrisonerSnapshot(
        string Name,
        string Type,
        int Energy);

    private sealed record ComputationResult(
        string Name,
        string Status,
        string Message,
        TimeSpan Duration)
    {
        public string ToReportLine() =>
            $"{Name} | {Status} | {Message} | " +
            $"{Duration.TotalMilliseconds:F2} ms";
    }

    // Valitav objekt arvutuste loendis
    public sealed class PrisonerChoice : INotifyPropertyChanged
    {
        private bool _isSelected;

        public event PropertyChangedEventHandler? PropertyChanged;

        public string Name { get; }
        public string Type { get; }
        public int Energy { get; }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected == value)
                    return;

                _isSelected = value;
                PropertyChanged?.Invoke(
                    this,
                    new PropertyChangedEventArgs(nameof(IsSelected)));
            }
        }

        public PrisonerChoice(Prisoner prisoner, bool isSelected)
        {
            Name = prisoner.Name;
            Type = prisoner.GetType().Name;
            Energy = prisoner.Energy;
            _isSelected = isSelected;
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

