using System.Windows;
using WarsztatV2.Menu;

namespace WarsztatV2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // domyslnie podmenu zlecienie - ukryte
            HideSubmenu();

            // wypisanie danych aktualnie zalogowanego uzytkownika
            IdUser.Text = "Witaj ! \nJan Kowalski";
        }

        // metoda rozwijajaca podmenu zlecenia
        private void ShowSubmenu()
        {       
          if(NoweZlecenie.IsVisible==true)
            HideSubmenu();

          else
            {
                NoweZlecenie.Visibility = Visibility.Visible;
                DoNaprawy.Visibility = Visibility.Visible;
                DoOdbioru.Visibility = Visibility.Visible;
                HistoriaZlecen.Visibility = Visibility.Visible;
            }
        }

        // metoda ukrywajaca podmenu zlecenia
        private void HideSubmenu()
        {
            NoweZlecenie.Visibility = Visibility.Collapsed;
            DoNaprawy.Visibility = Visibility.Collapsed;
            DoOdbioru.Visibility = Visibility.Collapsed;
            HistoriaZlecen.Visibility = Visibility.Collapsed;

        }

        //metoda rozwijajaca podmenu zlecenia
        private void ZleceniaClick(object sender, RoutedEventArgs e)
        {
           ShowSubmenu();
        }

        //------------ metody przelaczajace zawartosc prawej czesci aplikacji ------------//
        private void OFirmieClick(object sender, RoutedEventArgs e)
        {

            RightContent.Content = new OFirmie();
        }

        private void KlienciClick(object sender, RoutedEventArgs e)
        {

            RightContent.Content = new Klienci();
        }

        private void PracownicyClick(object sender, RoutedEventArgs e)
        {

            RightContent.Content = new Pracownicy();
        }


        private void AktualnosciClick(object sender, RoutedEventArgs e)
        {

            RightContent.Content = new Aktualnosci();
        }

        private void SamochodyClick(object sender, RoutedEventArgs e)
        {

            RightContent.Content = new Samochody();
        }

    }
}
