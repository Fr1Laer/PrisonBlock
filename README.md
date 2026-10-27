# PrisonBlock

## Valitud maailma ja rakenduse eesmärki

Humoorikas WPF-rakendus, kus vangid (Rookie, Philosopher, Artist, ChessMaster, Chef, Gardener, Musician, Gambler) saavad teha erinevaid tegevusi. Igal vangitüübil on oma omadused ja võimekused

## Kuidas programmiga töötada

- Sisestage vangide nimi

- Valige vangide tüüp

- Lisage vang nupuga **Lisa vang**

- Valige üks tegevustest, mida vang peab tegema: `CrazyAction`, `Work`, `Study`, `Play Chess`

- Vangitüübist sõltuvalt viiakse läbi vastav tegevus. Tulemus kuvatakse logides

- Vangi eemaldamiseks tuleb ta nimekirjast valida ja seejärel klõpsata nupule **Remove**

- Kasutage otsingut vangide leidmiseks nime järgi

- Vaadake sorteeritud vangide nimekirja, tüüpide kokkuvõtet ja tegevuste logi

## Vol. 2 selgitus

## Klassihierarhia ning liideste lühikirjeldust

- `Prisoner` – abstraktne baasklass, mis sisaldab vangide ühiseid omadusi ja meetodeid

- Vangitüübid: `Rookie`, `Philosopher`, `Artist`, `ChessMaster`, `Chef`, `Musician`, `Athlete` ja `Comedian`

- `IWork`, `IStudy` ja `IPlayChess` - liidesed, mis määravad, milliseid tegevusi vang saab sooritada

- `PrisonFactory` - loob vange vastavalt valitud tüübile ja kontrollib sisendit

- `IRepository<T>` ja `Repository<T>` - võimaldavad vange lisada, eemaldada, otsida ja kuvada

- `PrisonService` – haldab vangidega seotud toiminguid ja suhtlust hoidlate vahel

- `MainViewModel` - haldab kasutajaliidese andmeid, käske, filtreerimist, sorteerimist ja statistikat

- `MainWindow.xaml` - rakenduse graafiline kasutajaliides

- `RelayCommand` - rakendab `ICommand`-liidest ning seob kasutajaliidese toimingud `ViewModeli` meetoditega

- `Events` - võimaldavad objektidel toimunud sündmustest teistele objektidele teada anda. Neid kasutatakse näiteks vangide lisamise, eemaldamise ja reaktsioonide logimiseks

- `Generics` - võimaldavad kirjutada taaskasutatavat koodi eri andmetüüpide jaoks. Näiteks `Repository<T>` saab hallata erinevat tüüpi vange ilma eraldi hoidlaklassi loomata

## Sündmused ja vangide reaktsioonid

- Olemasolevad vangid tervitavad uut vangi
- Vangi eemaldamisel jätavad teised vangid temaga hüvasti
- `CrazyAction` käivitamisel reageerivad sama hoidla teised vangid
- Eemaldatud vang ei reageeri enam hilisematele sündmustele
- Reaktsioonid salvestatakse tegevuste logisse ning võivad muuta vangide olekut

## CrazyAction-tegevuste ja olekureeglite selgitust

Igal vangitüübil on oma `CrazyAction()` meetodi teostus. Tegevused võivad muuta energiat, meistriteoste arvu või malevõitude arvu. Energia jääb vahemikku 0–100

## MVVM-arhitektuur

- `Model` - esindab vange ja nende omadusi ning tegevusi. `Model` hõlmab järgmisi komponente: `Prisoner`, `Prisoners`, `PrisonerFactory`

- `View` - `MainWindow.xaml`, mis määrab rakenduse graafilise kasutajaliidese

- `ViewModel` - `MainViewModel`, mis haldab kasutajaliidese andmeid, käske, filtreerimist, sorteerimist ja statistikat

- `DataBinding` - seob kasutajaliidese elemendid ViewModeli andmetega

- `RelayCommand` - võimaldab siduda kasutajaliidese nupud `ViewModeli` käskudega

## LINQ-päringud

Rakendus kasutab LINQ vangide andmete töötlemiseks:

- `Where` - vangide filtreerimine nime järgi

- `OrderByDescending` - vangide sorteerimine energia järgi kahanevas järjekorras

- `GroupBy` - vangide rühmitamine tüübi järgi

- `Count` - vangide arvu leidmine igas rühmas

- `Concat` - eri hoidlate vangide ühendamine ühtseks nimekirjaks

---

## Vol. 3 selgitus

---

## CPU-arvutused ja paralleeltöötlus

Rakendus võimaldab võrrelda vangide andmete põhjal tehtavaid CPU-põhiseid arvutusi järjestikuses ja paralleelses režiimis

### Arvutuse objektid

Arvutusteks saab valida vähemalt kolm vangi. Enne arvutuste käivitamist salvestatakse iga valitud vangi kohta muutumatu andmesnapshot, mis sisaldab:

- vangi nime
- vangi tüüpi
- energia väärtust

Arvutused kasutavad snapshot'i andmeid, mitte töö käigus muutuvaid vangide omadusi. Nii kasutatakse järjestikuses ja paralleelses režiimis samu sisendandmeid

### Arvutuste mahu valimine

Kasutaja saab valida ühe vangi kohta tehtavate iteratsioonide arvu:

- 100 000
- 500 000
- 1 000 000
- 3 000 000
- 1 000 000 000

Suurem iteratsioonide arv suurendab CPU-koormust ja võib pikendada arvutuste kestust märkimisväärselt

### Järjestikune režiim

Järjestikuses režiimis käivitatakse iga vangi arvutus pärast eelmise arvutuse lõppu. Iga objekti tulemus, lõppolek ja arvutuse kestus lisatakse aruandesse

### Paralleelne režiim

Paralleelses režiimis käivitatakse objektide arvutused eraldi `Task.Run` ülesannetes. Kõigi ülesannete lõpetamist oodatakse `Task.WhenAll` abil

Mõlemas režiimis kasutatakse sama arvutusalgoritmi ja samu sisendandmeid. Paralleelse režiimi tegelik kiirendus sõltub protsessori võimalustest ja süsteemi koormusest

### Tühistamine ja veakäsitlus

Kasutaja saab valida loendist konkreetse objekti ja tühistada ainult selle objekti arvutuse. Igal objektil on oma `CancellationTokenSource` ja `CancellationToken`

Arvutuse tsükkel kontrollib tühistamistaotlust regulaarselt ning katkestab töö, kui vastava objekti arvutus tühistatakse. Tühistatud objekt eemaldatakse käimasoleva arvutuse ajal tühistamiseks saadaolevate objektide loendist

Iga objekti arvutuse puhul käsitletakse eraldi järgmisi olekuid:

- **Õnnestus** - arvutus lõpetati edukalt ja tulemus saadi kätte
- **Tühistatud** - arvutus katkestati kasutaja tühistamistaotluse tõttu
- **Viga** - arvutuse käigus tekkis erand

Ühe objekti viga või tühistamine ei takista teiste objektide arvutuste tulemuste töötlemist. Uue arvutuse käivitamisel lähtestatakse eelmise käivituse tühistamise olekud

### Aruanne ja jõudluse võrdlus

Aruandes kuvatakse iga objekti nimi, lõppolek, tulemus või veateade ning arvutuse kestus millisekundites. Lisaks kuvatakse kõigi arvutuste koguaeg

Järjestikuse ja paralleelse režiimi võrdlemiseks tuleb valida samad objektid ja sama iteratsioonide arv. Tulemuste ja kestuste võrdlemine aitab hinnata paralleeltöötluse mõju rakenduse jõudlusele

### Kasutatud tehnoloogiad

- `Task.Run` - CPU-põhiste arvutuste käivitamine taustal
- `Task.WhenAll` - paralleelsete ülesannete lõpetamise ootamine;
- `CancellationTokenSource` ja `CancellationToken` - iga objekti arvutuse eraldi tühistamine
- `Stopwatch` – arvutuste kestuse mõõtmine
- muutumatud snapshot-andmed - samade sisendandmete kasutamine mõlemas režiimis
- WPF-i andmesidumine (`INotifyPropertyChanged`, `ObservableCollection`) - kasutajaliidese ja arvutustulemuste uuendamine

### Programmi töötamise näide 
