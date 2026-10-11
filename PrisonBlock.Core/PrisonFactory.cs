using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using static PrisonBlock.Core.Artist;
namespace PrisonBlock.Core;

public static class PrisonFactory
{
    public static Prisoner? Create(string type, string name, out string error)
    {
        error = string.Empty;

        if (string.IsNullOrWhiteSpace(name))
        {
            error = "Nimi ei tohi olla tühi";

            return null;
        }

        var cleanName = name.Trim();

        return type.Trim().ToLowerInvariant() switch
        {
            "rookie" => new Rookie(cleanName),
            "philosopher" => new Philoshopher(cleanName),
            "artist" => new Artist(cleanName),
            "chessmaster" => new ChessMaster(cleanName),

            // chef added 
            "chef" => new Chef(cleanName),

            _ => Invalid(out error)
        };
    }
    private static Prisoner? Invalid(out string error)
    {
        error = "Vangitüüp teadmata";
        return null;
    }
}
