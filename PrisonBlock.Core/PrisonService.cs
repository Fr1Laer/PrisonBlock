using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace PrisonBlock.Core;

public sealed class PrisonService
{
    public ObservableCollection<Prisoner> Prisoners { get; } = new();

    public void Add(Prisoner prisoner)
    {
        ArgumentNullException.ThrowIfNull(prisoner);
        Prisoners.Add(prisoner);
    }

    public bool Remove(Prisoner prisoner) => prisoner is not null && Prisoners.Remove(prisoner);
    public string PerformCrazyAction(Prisoner? p) => p is null ? "Valige esmalt vang" : p.CrazyAction();
    public string PerformWork(Prisoner? p) => p is IWork w ? w.Work() : $"{p?.Name ?? "Unknown"}-l puudub ametikoht";
    public string PerformStudy(Prisoner? p) => p is IStudy s ? s.Study() : $"{p?.Name ?? "Unknown"}-l puudub õppimisega seotud roll";
    public string PerformChess(Prisoner? p) => p is IPlayChess c ? c.PlayChess() : $"{p?.Name ?? "Unknown"}-l puudub maleroll";

}
