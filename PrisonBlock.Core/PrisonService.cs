using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace PrisonBlock.Core;

public sealed class PrisonService
{
    public Repository<Rookie> Rookies { get; } = new();
    public Repository<Philosopher> Philosophers { get; } = new();
    public Repository<Artist> Artists { get; } = new();
    public Repository<ChessMaster> ChessMasters { get; } = new();
    public Repository<Chef> Chefs { get; } = new();

    public event Action<string>? LogGenerated;

    public PrisonService()
    {
        Rookies.LogGenerated += ForwardLog;
        Philosophers.LogGenerated += ForwardLog;
        Artists.LogGenerated += ForwardLog;
        ChessMasters.LogGenerated += ForwardLog;
        Chefs.LogGenerated += ForwardLog;
    }

    public string[] RepositoryTypes =>
    [
        nameof(Rookie),
        nameof(Philosopher),
        nameof(Artist),
        nameof(ChessMaster),
        nameof(Chef)
     ];

    public IEnumerable<Prisoner> GetAll() =>
        Rookies.GetAll().Cast<Prisoner>()
            .Concat(Philosophers.GetAll())
            .Concat(Artists.GetAll())
            .Concat(ChessMasters.GetAll())
            .Concat(Chefs.GetAll());

    public IEnumerable<Prisoner> GetByType(string type) => type switch
    {
        nameof(Rookie) => Rookies.GetAll(),
        nameof(Philosopher) => Philosophers.GetAll(),
        nameof(Artist) => Artists.GetAll(),
        nameof(ChessMaster) => ChessMasters.GetAll(),
        nameof(Chef) => Chefs.GetAll(),
        _ => Enumerable.Empty<Prisoner>()
    };

    private void ForwardLog(string message) => LogGenerated?.Invoke(message);

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
            default:
                throw new ArgumentException("Tundmatu tegelastüüp", nameof(prisoner));
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
            _ => false
        };

    public string PerformCrazyAction(Prisoner? p)
    {
        if (p is null)
            return "Valige esmalt vang";

        string result = p.CrazyAction();

        switch (p)
        {
            case Rookie r: 
                Rookies.PublishCrazyAction(r); 
                break;
            case Philosopher r:
                Philosophers.PublishCrazyAction(r);
                break;
            case Artist r: 
                Artists.PublishCrazyAction(r); 
                break;
            case ChessMaster r:
                ChessMasters.PublishCrazyAction(r);
                break;
            case Chef r: 
                Chefs.PublishCrazyAction(r); 
                break;
        }

        return result;
    }

    public string PerformWork(Prisoner? p) => p is IWork w ? w.Work() : $"{p?.Name ?? "Unknown"}-l puudub ametikoht";
    public string PerformStudy(Prisoner? p) => p is IStudy s ? s.Study() : $"{p?.Name ?? "Unknown"}-l puudub õppimisega seotud roll";
    public string PerformChess(Prisoner? p) => p is IPlayChess c ? c.PlayChess() : $"{p?.Name ?? "Unknown"}-l puudub maleroll";
}
