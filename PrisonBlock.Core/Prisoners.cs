namespace PrisonBlock.Core;

public sealed class Rookie : Prisoner, IWork
{
    public Rookie(string name) : base(name, 85) { }

    internal Rookie(Guid id, string name, int energy)
        : base(id, name, energy) { }

    public string Work()
    {
        if (!TrySpendEnergy(20))
            return $"{Name} on liiga väsinud, et töötada";

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

public sealed class Philosopher : Prisoner, IStudy
{
    public Philosopher(string name) : base(name, 60) { }

    internal Philosopher(Guid id, string name, int energy)
        : base(id, name, energy) { }

    public string Study()
    {
        if (!TrySpendEnergy(10))
            return $"{Name} on liiga väsinud, et õppida";

        return $"{Name} uuris, kas vangla-võileiba võib pidada eksistentsiaalseks dilemmaks";
    }

    public override string CrazyAction() =>
        Energy >= 50
            ? $"{Name} annab lukustatud ukse ees seistes 20-minutilise loengu vabadusest"
            : $"{Name} mõtiskleb energia mõiste üle. Kahjuks ei suurenda see mõiste energiat";
}

public sealed class Artist : Prisoner, IWork, IStudy
{
    private int _masterpieces;

    public Artist(string name) : base(name, 75) { }

    internal Artist(Guid id, string name, int energy, int masterpieces)
        : base(id, name, energy)
    {
        _masterpieces = Math.Max(0, masterpieces);
    }

    public int Masterpieces => _masterpieces;

    public string Work()
    {
        if (!TrySpendEnergy(15))
            return $"{Name} on liiga väsinud, et maalida veel üht meistriteost";

        _masterpieces++;
        return $"{Name} maalib seinale meistriteose #{_masterpieces}";
    }

    public string Study()
    {
        if (!TrySpendEnergy(8))
            return $"{Name} ei saa õppidal, kunstiline inspiratsioon on võtnud pausi";

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

    public ChessMaster(string name) : base(name, 65) { }

    internal ChessMaster(Guid id, string name, int energy, int wins)
        : base(id, name, energy)
    {
        _wins = Math.Max(0, wins);
    }

    public int Wins => _wins;

    public string Study()
    {
        if (!TrySpendEnergy(7))
            return $"{Name} on praegu liiga väsinud, et malet avanguid õppida";

        return $"{Name} mõtleb kolm käiku ette";
    }

    public string PlayChess()
    {
        if (!TrySpendEnergy(12))
            return $"{Name} keeldub malemängust: energiast ei piisa";

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

public sealed class Chef : Prisoner, IWork
{
    public Chef(string name) : base(name, 70) { }

    internal Chef(Guid id, string name, int energy)
        : base(id, name, energy) { }

    public string Work()
    {
        if (!TrySpendEnergy(7))
            return $"{Name} on praegu liiga väsinud, et süüa teha";

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

public sealed class Gardener : Prisoner, IWork
{
    private int _plants;

    public Gardener(string name) : base(name, 80) { }

    internal Gardener(Guid id, string name, int energy, int plants)
        : base(id, name, energy)
    {
        _plants = Math.Max(0, plants);
    }

    public int Plants => _plants;

    public string Work()
    {
        if (!TrySpendEnergy(12))
            return $"{Name} on liiga väsinud, et vangla hoovis taimi kasta";

        _plants++;
        return $"{Name} istutab betoonipragu tomatitaime #{_plants}";
    }

    public override string GetDetails() => base.GetDetails() + $" | Taimed: {Plants}";

    public override string CrazyAction()
    {
        if (Energy < 20)
        {
            RestoreEnergy(15);
            return $"{Name} jääb porgandipeenra kõrvale magama. Energia on taastunud";
        }

        TrySpendEnergy(15);
        _plants++;
        return $"{Name} kuulutab hoovi iseseisvaks lillekuningriigiks ja istutab taime #{_plants}";
    }
}

public sealed class Musician : Prisoner, IWork, IStudy
{
    private int _songs;

    public Musician(string name) : base(name, 70) { }

    internal Musician(Guid id, string name, int energy, int songs)
        : base(id, name, energy)
    {
        _songs = Math.Max(0, songs);
    }

    public int Songs => _songs;

    public string Work()
    {
        if (!TrySpendEnergy(10))
            return $"{Name} on liiga väsinud, et plekktassil trummi lüüa";

        _songs++;
        return $"{Name} esitab koridoris plekktassiga laulu #{_songs}";
    }

    public string Study()
    {
        if (!TrySpendEnergy(6))
            return $"{Name} ei suuda noote õppida, sest tal on kõrvad kinni";

        return $"{Name} õpib noote, mis on kirjutatud vangla toidumenüü tagaküljele";
    }

    public override string GetDetails() => base.GetDetails() + $" | Laulud: {Songs}";

    public override string CrazyAction()
    {
        if (!TrySpendEnergy(14))
        {
            RestoreEnergy(12);
            return $"{Name} annab vaikuse-kontserdi ja puhkab. Energia on taastunud";
        }

        _songs++;
        return $"{Name} annab rakuseina taga kontserdi. Laul #{_songs} saab kõigi lemmikuks";
    }
}

public sealed class Gambler : Prisoner, IPlayChess
{
    private int _cheats;

    public Gambler(string name) : base(name, 55) { }

    internal Gambler(Guid id, string name, int energy, int cheats)
        : base(id, name, energy)
    {
        _cheats = Math.Max(0, cheats);
    }

    public int Cheats => _cheats;

    public string PlayChess()
    {
        if (!TrySpendEnergy(9))
            return $"{Name} on liiga väsinud, et malelauda isegi ära kaotada";

        _cheats++;
        return $"{Name} mängib malet ja vahetab salaja etturi ära. Petmine #{_cheats}";
    }

    public override string GetDetails() => base.GetDetails() + $" | Petmised: {Cheats}";

    public override string CrazyAction()
    {
        if (Energy < 15)
        {
            RestoreEnergy(10);
            return $"{Name} kaotab kõik sigaretid ja läheb nutma. Energia on taastunud";
        }

        TrySpendEnergy(13);
        _cheats++;
        return $"{Name} panustab oma sööklaportsjoni ja kaotab selle. Petmine #{_cheats}";
    }
}