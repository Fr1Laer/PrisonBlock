namespace PrisonBlock.Core;

public sealed class PrisonService
{
    public Repository<Rookie> Rookies { get; } = new();
    public Repository<Philosopher> Philosophers { get; } = new();
    public Repository<Artist> Artists { get; } = new();
    public Repository<ChessMaster> ChessMasters { get; } = new();
    public Repository<Chef> Chefs { get; } = new();
    public Repository<Gardener> Gardeners { get; } = new();
    public Repository<Musician> Musicians { get; } = new();
    public Repository<Gambler> Gamblers { get; } = new();

    public event Action<string>? LogGenerated;

    public PrisonService()
    {
        Rookies.LogGenerated += ForwardLog;
        Philosophers.LogGenerated += ForwardLog;
        Artists.LogGenerated += ForwardLog;
        ChessMasters.LogGenerated += ForwardLog;
        Chefs.LogGenerated += ForwardLog;
        Gardeners.LogGenerated += ForwardLog;
        Musicians.LogGenerated += ForwardLog;
        Gamblers.LogGenerated += ForwardLog;
    }

    public string[] RepositoryTypes =>
    [
        nameof(Rookie),
        nameof(Philosopher),
        nameof(Artist),
        nameof(ChessMaster),
        nameof(Chef),
        nameof(Gardener),
        nameof(Musician),
        nameof(Gambler)
    ];

    public IEnumerable<Prisoner> GetAll() =>
        Rookies.GetAll().Cast<Prisoner>()
            .Concat(Philosophers.GetAll())
            .Concat(Artists.GetAll())
            .Concat(ChessMasters.GetAll())
            .Concat(Chefs.GetAll())
            .Concat(Gardeners.GetAll())
            .Concat(Musicians.GetAll())
            .Concat(Gamblers.GetAll());

    public IEnumerable<Prisoner> GetByType(string type) => type switch
    {
        nameof(Rookie) => Rookies.GetAll(),
        nameof(Philosopher) => Philosophers.GetAll(),
        nameof(Artist) => Artists.GetAll(),
        nameof(ChessMaster) => ChessMasters.GetAll(),
        nameof(Chef) => Chefs.GetAll(),
        nameof(Gardener) => Gardeners.GetAll(),
        nameof(Musician) => Musicians.GetAll(),
        nameof(Gambler) => Gamblers.GetAll(),
        _ => Enumerable.Empty<Prisoner>()
    };

    private void ForwardLog(string message) =>
        LogGenerated?.Invoke(message);

    public void Add(Prisoner prisoner)
    {
        ArgumentNullException.ThrowIfNull(prisoner);

        switch (prisoner)
        {
            case Rookie p:
                Rookies.Add(p);
                break;
            case Philosopher p:
                Philosophers.Add(p);
                break;
            case Artist p:
                Artists.Add(p);
                break;
            case ChessMaster p:
                ChessMasters.Add(p);
                break;
            case Chef p:
                Chefs.Add(p);
                break;
            case Gardener p:
                Gardeners.Add(p);
                break;
            case Musician p:
                Musicians.Add(p);
                break;
            case Gambler p:
                Gamblers.Add(p);
                break;
            default:
                throw new ArgumentException(
                    "Tundmatu tegelastüüp",
                    nameof(prisoner));
        }
    }

    public bool Remove(Prisoner prisoner) =>
        prisoner switch
        {
            Rookie p => Rookies.Remove(p),
            Philosopher p => Philosophers.Remove(p),
            Artist p => Artists.Remove(p),
            ChessMaster p => ChessMasters.Remove(p),
            Chef p => Chefs.Remove(p),
            Gardener p => Gardeners.Remove(p),
            Musician p => Musicians.Remove(p),
            Gambler p => Gamblers.Remove(p),
            _ => false
        };

    public string PerformCrazyAction(Prisoner? prisoner)
    {
        if (prisoner is null)
            return "Valige esmalt vang";

        string result = prisoner.CrazyAction();

        switch (prisoner)
        {
            case Rookie p:
                Rookies.PublishCrazyAction(p);
                break;
            case Philosopher p:
                Philosophers.PublishCrazyAction(p);
                break;
            case Artist p:
                Artists.PublishCrazyAction(p);
                break;
            case ChessMaster p:
                ChessMasters.PublishCrazyAction(p);
                break;
            case Chef p:
                Chefs.PublishCrazyAction(p);
                break;
            case Gardener p:
                Gardeners.PublishCrazyAction(p);
                break;
            case Musician p:
                Musicians.PublishCrazyAction(p);
                break;
            case Gambler p:
                Gamblers.PublishCrazyAction(p);
                break;
        }

        return result;
    }

    public string PerformWork(Prisoner? prisoner) =>
        prisoner is IWork work
            ? work.Work()
            : $"{prisoner?.Name ?? "Unknown"}-l puudub ametikoht";

    public string PerformStudy(Prisoner? prisoner) =>
        prisoner is IStudy study
            ? study.Study()
            : $"{prisoner?.Name ?? "Unknown"}-l puudub õppimisega seotud roll";

    public string PerformChess(Prisoner? prisoner) =>
        prisoner is IPlayChess chess
            ? chess.PlayChess()
            : $"{prisoner?.Name ?? "Unknown"}-l puudub maleroll";

    // Asendab kogu maailma õigetes tüübipõhistes hoidlates
    public void ReplaceAll(IEnumerable<Prisoner> prisoners)
    {
        ArgumentNullException.ThrowIfNull(prisoners);

        List<Prisoner> items = prisoners.ToList();

        if (items.Any(p => p is null))
            throw new ArgumentException(
                "Vangide loend sisaldab tühje elemente",
                nameof(prisoners));

        if (items.Select(p => p.Id).Distinct().Count() != items.Count)
            throw new InvalidOperationException(
                "Vangide ID-d peavad olema unikaalsed");

        List<Rookie> rookies = new();
        List<Philosopher> philosophers = new();
        List<Artist> artists = new();
        List<ChessMaster> chessMasters = new();
        List<Chef> chefs = new();
        List<Gardener> gardeners = new();
        List<Musician> musicians = new();
        List<Gambler> gamblers = new();

        foreach (Prisoner prisoner in items)
        {
            switch (prisoner)
            {
                case Rookie p:
                    rookies.Add(p);
                    break;
                case Philosopher p:
                    philosophers.Add(p);
                    break;
                case Artist p:
                    artists.Add(p);
                    break;
                case ChessMaster p:
                    chessMasters.Add(p);
                    break;
                case Chef p:
                    chefs.Add(p);
                    break;
                case Gardener p:
                    gardeners.Add(p);
                    break;
                case Musician p:
                    musicians.Add(p);
                    break;
                case Gambler p:
                    gamblers.Add(p);
                    break;
                default:
                    throw new ArgumentException(
                        $"Tundmatu vangitüüp: {prisoner.GetType().Name}",
                        nameof(prisoners));
            }
        }

        Rookies.ReplaceAll(rookies);
        Philosophers.ReplaceAll(philosophers);
        Artists.ReplaceAll(artists);
        ChessMasters.ReplaceAll(chessMasters);
        Chefs.ReplaceAll(chefs);
        Gardeners.ReplaceAll(gardeners);
        Musicians.ReplaceAll(musicians);
        Gamblers.ReplaceAll(gamblers);
    }
}