using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;

namespace PrisonBlock.Core;

public sealed class Rookie : Prisoner, IWork
{
    public Rookie(string name) : base(name, 85) 
    { 
    }

    public string Work()
    {
        if (!TrySpendEnergy(20))
        {
            return $"{Name} on liiga väsinud, et töötada";
        }
            
        return $"{Name} poleeris koridori põrandat";
    }

    public override string CrazyAction()
    {
        if (Energy < 25)
        {
            RestoreEnergy(20);
            return $"{Name} peidab end viis minutit teki alla. Energia on taastunud";
        }

        TrySpendEnergy(20);

        return $"{Name} alustab lusikaga dramaatilist põgenemist";
    }
}

public sealed class Philoshopher : Prisoner, IStudy
{
    public Philoshopher(string name) : base(name, 60)
    {
    }

    public string Study()
    {
        if (!TrySpendEnergy(10))
        {
            return $"{Name} on liiga väsinud, et õppida";
        }

        return $"{Name} uuris, kas vangla-võileiba võib pidada eksistentsiaalseks dilemmaks";
    }

    public override string CrazyAction()
    {
        return Energy >= 50
            ? $"{Name} annab lukustatud ukse ees seistes 20-minutilise loengu vabadusest"
            : $"{Name} mõtiskleb energia mõiste üle. Kahjuks ei suurenda see mõiste energiat";
    }
}

public sealed class Artist : Prisoner, IWork, IStudy
{
    private int _masterpieces;
    public Artist(string name) : base(name, 75)
    {
    }

    public int Masterpieces => _masterpieces;

    public string Work()
    {
        if (!TrySpendEnergy(15))
        {
            return $"{Name} on liiga väsinud, et maalida veel üht meistriteost";
        }

        _masterpieces++;

        return $"{Name} maalib seinale meistriteose #{_masterpieces}";
    }

    public string Study()
    {
        if (!TrySpendEnergy(8))
        {
            return $"{Name} ei saa õppidal, kunstiline inspiratsioon on võtnud pausi";
        }

        return $"{Name} uurib värviteooriat ja hindab kohe raku beežikaid seinu";
    }

    public override string GetDetails() => base.GetDetails() + $" | Meistriteosed: {Masterpieces}";

    public override string CrazyAction()
    {
        if (!TrySpendEnergy(12))
        {
            RestoreEnergy(15);
            return $"{Name} kuulutab tühja lõuendi performance-kunstiteoseks ja säästab energiat";
        }

        _masterpieces++;

        return $"{Name} muudab vangla inspekteerimise kontrollnimekirja meistriteoseks #{_masterpieces}";
    }
}

public sealed class ChessMaster : Prisoner, IPlayChess, IStudy
{
    private int _wins;
    public ChessMaster(string name) : base(name, 65) 
    { 
    }

    public int Wins => _wins;

    public string Study()
    {
        if (!TrySpendEnergy(7))
        {
            return $"{Name} on praegu liiga väsinud, et malet avanguid õppida";
        }

        return $"{Name} mõtleb kolm käiku ette";
    }

    public string PlayChess()
    {
        if (!TrySpendEnergy(12))
        {
            return $"{Name} keeldub malemängust: energiast ei piisa";
        }

        _wins++;

        return $"{Name} võidab malemängu väljamõeldud vastase vastu. Võit #{Wins}";
    }

    public override string GetDetails() => base.GetDetails() + $" | Malivõidud: {Wins}";

    public override string CrazyAction()
    {

        if (Energy < 20)
        {
            RestoreEnergy(15);
            return $"{Name} teeb strateegilise uinaku, sest isegi suurmeistrid peavad end taaskäivitama";
        }

        TrySpendEnergy(18);

        return $"{Name} paigutab sööklataldrikud ümber hiiglaslikuks maleseisuks ja kuulutab matti";
    }
}

// new prisoner: Chef
public sealed class Chef : Prisoner, IWork
{
    public Chef(string name) : base(name, 70)
    {
    }

    public string Work()
    {
        if (!TrySpendEnergy(7))
        {
            return $"{Name} on praegu liiga väsinud, et süüa teha";
        }

        return $"{Name} avastab endale uusi meetodeid, valmistades vangidele järgmist sööki";
    }

    public override string CrazyAction()
    {
        if (Energy < 18)
        {
            RestoreEnergy(15);
            return $"{Name} teeb köögis lühikese uinaku, unistades Michelin-tärnist. Energia on taastunud";
        }

        TrySpendEnergy(16);

        return $"{Name} korraldab toiduvalmistamisvõistluse, kasutades vangla toidujääke";
    }
}

