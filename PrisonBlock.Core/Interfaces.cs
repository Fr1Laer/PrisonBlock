using System;
using System.Collections.Generic;
using System.Text;

namespace PrisonBlock.Core;

public interface IWork
{
    string Work();
}

public interface IStudy
{
    string Study();
}

public interface IPlayChess
{
    string PlayChess();
}

public interface IPrisonerEventSource
{
    event Action<Prisoner>? Added;
    event Action<Prisoner>? Removing;
    event Action<Prisoner>? CrazyActionPerformed;
}
