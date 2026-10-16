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

## Kaasüliõpilase `issue` #2

- Tuleb lisada kolm uut tegelasetüüpi/klassi, millest igaüks käitub omal viisil, ning vähemalt üks eksemplar igast uuest tüübist. Kolm eksemplari olemasolevatest tüüpidest ei ole piisav

- Uued tüübid peavad kasutama sama salvestussüsteemi, sündmuste süsteemi ja MVVM-i ning olema kasutajaliideses kasutatavad

### Kaasüliõpilane lisas kolm uut tüüpi: `Gardener`, `Musician`, `Gambler`

## Näide programmi tööst (Vol. 2), millele on lisatud uued tüübid

<img width="1031" height="624" alt="Знімок екрана 2026-10-16 123433" src="https://github.com/user-attachments/assets/25d6d1bf-c1c1-49b6-aaef-f75133534af6" />

<img width="1036" height="625" alt="Знімок екрана 2026-10-16 123445" src="https://github.com/user-attachments/assets/7964a0af-eb3f-4f16-b231-69d94dc979e7" />

<img width="1037" height="624" alt="Знімок екрана 2026-10-16 123529" src="https://github.com/user-attachments/assets/1df2cbfb-704b-411f-beb0-c1efebfcabb8" />

Need näited näitavad, et uued tüübid töötavad korralikult. Samuti toimib sorteerimine, vangidevaheline suhtlus uue tegelase lisamisel ning vangide tüüpide kokkuvõtmine

## Teiste vangide reaktsioon, kui sama rühma kuuluv vang eemaldati\

<img width="981" height="534" alt="Знімок екрана 2026-10-16 123918" src="https://github.com/user-attachments/assets/93623288-f844-4ae7-a54f-0c7afc7f72a2" />

Sellest näitest on näha, kuidas teised vangid hüvasti jätavad eemaldatud vangiga 
