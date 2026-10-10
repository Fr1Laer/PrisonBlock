using PrisonBlock.Core;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PrisonBlock.WpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly PrisonService _service = new();

        private Prisoner _selectedPrisoner;

        public event PropertyChangedEventHandler? PropertyChanged;

        public System.Collections.ObjectModel.ObservableCollection<Prisoner> Prisoners => _service.Prisoners;

        public Prisoner? SelectedPrisoner 
        { 
            get => _selectedPrisoner; 
            set 
            { 
                _selectedPrisoner = value; 
                PropertyChanged?.Invoke(this, new(nameof(SelectedPrisoner))); 
                RefreshSelected(); 
            } 
        }
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            AddStarterPrisoners();
        }

        private void AddStarterPrisoners()
        {
            _service.Add(new Rookie("Rookie"));
            _service.Add(new Philoshopher("Philoshopher"));
            _service.Add(new Artist("Artist"));
            _service.Add(new ChessMaster("ChessMaster"));

            SelectedPrisoner = Prisoners[0];
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            string type = (TypeBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";
            Prisoner? p = PrisonFactory.Create(type, NameTextBox.Text, out string error);
            if (p is null)
            {
                MessageBox.Show(error, "Kehtetu sisestus",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            _service.Add(p);
            SelectedPrisoner = p;
            NameTextBox.Clear();
            Log($"Lisati {p.Name} ({p.GetType().Name})");
        }

        private void Crazy_Click(object sender, RoutedEventArgs e)
        {
            Run(_service.PerformCrazyAction);
        }

        private void Work_Click(object sender, RoutedEventArgs e)
        {
            Run(_service.PerformWork);
        }

        private void Study_Click(object sender, RoutedEventArgs e)
        {
            Run(_service.PerformStudy);
        }

        private void Chess_Click(object sender, RoutedEventArgs e)
        {
            Run(_service.PerformChess);
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedPrisoner is null)
            {
                Log("Enne vangide eemaldamist vali üks neist välja");
                return;
            }

            var name = SelectedPrisoner.Name;
            _service.Remove(SelectedPrisoner);
            SelectedPrisoner = Prisoners.Count > 0 ? Prisoners[0] : null;
            Log($"{name} eemaldati plokist 13");
        }

        private void Run(Func<Prisoner?, string> action)
        {
            Log(action(SelectedPrisoner));
            RefreshSelected();
            PrisonerList.Items.Refresh();
        }
        private void RefreshSelected()
        {
            DetailsText.Text = SelectedPrisoner?.GetDetails() ?? "Vangit ei valitud";
            EnergyBar.Value = SelectedPrisoner?.Energy ?? 0;
        }

        private void Log(string message)
        {
            LogBox.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}{Environment.NewLine}");
            LogBox.ScrollToEnd();
        }
    }
}