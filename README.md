# PrisonBlock

## Valitud maailma ja rakenduse eesmärki

Humoorikas WPF-rakendus, kus neli vangi (Rookie, Philosopher, Artist, ChessMaster, Chef, Gardener, Musician, Gambler) saavad teha erinevaid tegevusi

## Kuidas programmiga töötada

- Sisestage vangide nimi

- Valige vangide tüüp

- Valige ta allolevast nimekirjast

- Valige üks tegevustest, mida vang peab tegema: `CrazyAction`, `Work`, `Study`, `Play Chess`

- Vangitüübist sõltuvalt viiakse läbi vastav tegevus. Tulemus kuvatakse logides

- Vangi eemaldamiseks tuleb ta nimekirjast valida ja seejärel klõpsata nupule **Remove**

## Klassihierarhia ning liideste lühikirjeldust

- Abstraktne `Prisoner` on baasklass neljale alamklassile
- Liidesed `IWork`, `IStudy` ja `IPlayChess` määravad vangide võimekused
- `Prisoners` vastutab iga vangide tegude eest
- `PrisonFactory` kontrollib nime õigsust, eemaldab üleliigsed tühikud ja väljastab veateate, kui nimi on tühi või kui lukustatud objekti tüüp on teadmata
- `PrisonService` kontrollib valitud vangide olemasolu ja toetab liideseid IWork, IStudy ja IPlayChess, et kindlaks teha, millised tegevused on konkreetsele vangile kättesaadavad

## CrazyAction-tegevuste ja olekureeglite selgitust

Iga vang täidab funktsiooni CrazyAction() erinevalt. Tegevused võivad muuta energiat, meistriteoste arvu või malevõitude arvu. Energia jääb vahemikku 0–100

## Valitud iseseisvalt õpitud WPF UI elemendi põhjendust

ProgressBar kuvab valitud vangi energiat visuaalselt, muutes oleku jälgimise lihtsaks

## Vähemalt viit kontrollitud kasutusjuhtu

### 1. Vangi lisamine kehtiva nimega

<img width="980" height="536" alt="image" src="https://github.com/user-attachments/assets/a6503fe4-1eeb-4016-bd04-f33639eec847" />

### 2. Tühja nime tagasilükkamine

<img width="976" height="535" alt="image" src="https://github.com/user-attachments/assets/4eab6c9c-961c-4c25-82d7-2230277fb454" />

### 3. Erinevate vangide CrazyAction() käivitamine

<img width="985" height="537" alt="image" src="https://github.com/user-attachments/assets/eaad9fa0-9f75-4fa5-8720-89b0fd773dfc" />

### 4. Artist-i Work ja Study tegevuste kasutamine

<img width="971" height="515" alt="image" src="https://github.com/user-attachments/assets/2dfec6a3-4fea-4e0f-a19f-439d557a28fd" />

### 5. Sobimatu tegevuse proovimine annab veateate

<img width="979" height="535" alt="image" src="https://github.com/user-attachments/assets/7099eb2f-3153-4950-b198-a7271d297b45" />

## Kaasüliõpilase `issue`

Ülesandeks oli luua uus vangitüüp `Chef`. See vang pärineb abstraktsest klassist `Prisoner`. Lisaks kasutab ta liidest `IWork`. Lisaks sellele realiseerib see vang oma meetodit `CrazyAction`. See vang lisatakse ka järgmistesse failidesse: `PrisonerFactory`, `MainWindow.xaml`, `MainWindow.xaml.cs` meetodis `AddStarterPrisoners()`

## Näide uue programmiversiooni tööst, mis rakendab uut vangitüüpi `Chef`

<img width="983" height="538" alt="image" src="https://github.com/user-attachments/assets/1ef0d091-703b-4575-90d7-98173b5e1f3d" />

Selles näites näidati tegevusi `CrazyAction`, `Work`, `Study`, `Play Chess`