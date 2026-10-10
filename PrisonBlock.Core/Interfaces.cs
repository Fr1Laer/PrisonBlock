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
