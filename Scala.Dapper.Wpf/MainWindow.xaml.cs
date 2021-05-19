using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Scala.Dapper.Core.Entities;
using Scala.Dapper.Core.Services;

namespace Scala.Dapper.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        DapperSyncService boekenService;
        bool isNew;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            boekenService = new DapperSyncService();

            PopulateBoeken();
            PopulateAuteurs();
            PopulateUitgevers();
            ActivateLeft();
        }
        private void ClearControls()
        {
            txtTitel.Text = "";
            txtJaar.Text = "";
            cmbAuteur.SelectedIndex = -1;
            cmbUitgever.SelectedIndex = -1;
        }
        private void ActivateLeft()
        {
            grpLeft.IsEnabled = true;
            grpRight.IsEnabled = false;
            btnBewaren.Visibility = Visibility.Hidden;
            btnAnnuleren.Visibility = Visibility.Hidden;

            grpAuteurs.IsEnabled = true;
            grpUitgevers.IsEnabled = true;
        }
        private void ActivateRight()
        {
            grpLeft.IsEnabled = false;
            grpRight.IsEnabled = true;
            btnBewaren.Visibility = Visibility.Visible;
            btnAnnuleren.Visibility = Visibility.Visible;

            grpAuteurs.IsEnabled = false;
            grpUitgevers.IsEnabled = false;
        }

        private void PopulateBoeken()
        {
            ClearControls();
            lstBoeken.ItemsSource = null;
            Auteur auteur = null;
            Uitgever uitgever = null;
            if (cmbFilterAuteur.SelectedItem != null)
                auteur = (Auteur)cmbFilterAuteur.SelectedItem;
            if (cmbFilterUitgever.SelectedItem != null)
                uitgever = (Uitgever)cmbFilterUitgever.SelectedItem;
            lstBoeken.ItemsSource = boekenService.GetBoeken(auteur, uitgever);
        }
        private void PopulateAuteurs()
        {
            cmbFilterAuteur.ItemsSource = null;
            cmbAuteur.ItemsSource = null;
            lstAuteurs.ItemsSource = null;

            List<Auteur> auteurs = boekenService.GetAuteurs();

            cmbAuteur.SelectedValuePath = "Id";
            cmbAuteur.DisplayMemberPath = "Naam";

            cmbFilterAuteur.ItemsSource = auteurs;
            cmbAuteur.ItemsSource = auteurs;
            lstAuteurs.ItemsSource = auteurs;
        }
        private void PopulateUitgevers()
        {
            cmbFilterUitgever.ItemsSource = null;
            cmbUitgever.ItemsSource = null;
            lstUitgevers.ItemsSource = null;

            List<Uitgever> uitgevers = boekenService.GetUitgevers();

            cmbUitgever.SelectedValuePath = "Id";
            cmbUitgever.DisplayMemberPath = "Naam";

            cmbFilterUitgever.ItemsSource = uitgevers;
            cmbUitgever.ItemsSource = uitgevers;
            lstUitgevers.ItemsSource = uitgevers;

        }


        private void CmbFilterAuteur_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PopulateBoeken();
        }
        private void CmbFilterUitgever_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PopulateBoeken();
        }
        private void BtnClearAuteurFilter_Click(object sender, RoutedEventArgs e)
        {
            cmbFilterAuteur.SelectedIndex = -1;
            PopulateBoeken();
        }
        private void BtnClearUitgeverFilter_Click(object sender, RoutedEventArgs e)
        {
            cmbFilterUitgever.SelectedIndex = -1;
            PopulateBoeken();
        }

        private void LstBoeken_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstBoeken.SelectedItem != null)
            {
                Boek boek = (Boek)lstBoeken.SelectedItem;
                txtTitel.Text = boek.Titel;
                txtJaar.Text = boek.Jaar.ToString();
                cmbAuteur.SelectedValue = boek.AuteurId;
                cmbUitgever.SelectedValue = boek.UitgeverId;
            }
        }

        private void BtnNieuw_Click(object sender, RoutedEventArgs e)
        {
            isNew = true;
            ActivateRight();
            ClearControls();
            txtTitel.Focus();
        }
        private void BtnWijzig_Click(object sender, RoutedEventArgs e)
        {
            if (lstBoeken.SelectedItem != null)
            {
                isNew = false;
                ActivateRight();
                txtTitel.Focus();
            }
        }
        private void BtnAnnuleren_Click(object sender, RoutedEventArgs e)
        {
            ActivateLeft();
            LstBoeken_SelectionChanged(null, null);
        }
        private void BtnVerwijder_Click(object sender, RoutedEventArgs e)
        {
            if (lstBoeken.SelectedItem != null)
            {
                if (MessageBox.Show("Ben je zeker?", "Boek wissen", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    Boek boek = (Boek)lstBoeken.SelectedItem;
                    if (!boekenService.BoekVerwijderen(boek))
                    {
                        MessageBox.Show("We konden het boek niet verwijderen !", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    ClearControls();
                    PopulateBoeken();
                }

            }
        }
        private void BtnBewaren_Click(object sender, RoutedEventArgs e)
        {
            string titel = txtTitel.Text.Trim();
            if (titel.Length == 0)
            {
                MessageBox.Show("Je dient een titel op te geven !", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                txtTitel.Focus();
                return;
            }
            if (cmbAuteur.SelectedItem == null)
            {
                MessageBox.Show("Je dient een auteur te selecteren !", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                cmbAuteur.Focus();
                return;
            }
            Auteur auteur = (Auteur)cmbAuteur.SelectedItem;
            if (cmbUitgever.SelectedItem == null)
            {
                MessageBox.Show("Je dient een uitgever te selecteren !", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                cmbUitgever.Focus();
                return;
            }
            Uitgever uitgever = (Uitgever)cmbUitgever.SelectedItem;
            int.TryParse(txtJaar.Text, out int jaar);
            Boek boek;
            if (isNew)
            {
                boek = new Boek(titel, auteur.Id, uitgever.Id, jaar);
                if (!boekenService.BoekToevoegen(boek))
                {
                    MessageBox.Show("We konden het nieuwe boek niet bewaren.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            else
            {
                boek = (Boek)lstBoeken.SelectedItem;
                boek.Titel = titel;
                boek.AuteurId = auteur.Id;
                boek.UitgeverId = uitgever.Id;
                boek.Jaar = jaar;
                if (!boekenService.BoekWijzigen(boek))
                {
                    MessageBox.Show("We konden het boek niet wijzigen.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            PopulateBoeken();
            lstBoeken.SelectedItem = boek;
            LstBoeken_SelectionChanged(null, null);
            ActivateLeft();
        }

        private void LstAuteurs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            txtAuteur.Text = "";
            if (lstAuteurs.SelectedItem != null)
            {
                txtAuteur.Text = ((Auteur)lstAuteurs.SelectedItem).Naam;
            }
        }
        private void BtnClearAuteur_Click(object sender, RoutedEventArgs e)
        {
            lstAuteurs.SelectedIndex = -1;
            txtAuteur.Text = "";
            txtAuteur.Focus();
        }
        private void BtnAuteurToevoegen_Click(object sender, RoutedEventArgs e)
        {
            string naam = txtAuteur.Text.Trim();
            if (naam.Length == 0)
            {
                MessageBox.Show("Je dient een naam op te geven !", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                txtAuteur.Focus();
                return;
            }
            Auteur auteur = new Auteur(naam);
            if (!boekenService.AuteurToevoegen(auteur))
            {
                MessageBox.Show("We konden de nieuwe auteur niet bewaren !", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                txtAuteur.Focus();
                return;
            }
            PopulateAuteurs();
            lstAuteurs.SelectedItem = auteur;
            LstAuteurs_SelectionChanged(null, null);
        }
        private void BtnAuteurWijzigen_Click(object sender, RoutedEventArgs e)
        {
            if (lstAuteurs.SelectedItem == null)
            {
                MessageBox.Show("Je dient eerst een auteur te selecteren !", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            string naam = txtAuteur.Text.Trim();
            if (naam.Length == 0)
            {
                MessageBox.Show("Je dient een naam op te geven !", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                txtAuteur.Focus();
                return;
            }
            Auteur auteur = (Auteur)lstAuteurs.SelectedItem;
            auteur.Naam = naam;
            if (!boekenService.AuteurWijzigen(auteur))
            {
                MessageBox.Show("We konden de auteur niet wijzigen !", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                txtAuteur.Focus();
                return;
            }
            PopulateAuteurs();
            lstAuteurs.SelectedItem = auteur;
            LstAuteurs_SelectionChanged(null, null);
        }
        private void BtnAuteurVerwijderen_Click(object sender, RoutedEventArgs e)
        {
            if (lstAuteurs.SelectedItem == null)
            {
                MessageBox.Show("Je dient eerst een auteur te selecteren !", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            Auteur auteur = (Auteur)lstAuteurs.SelectedItem;
            if (boekenService.IsAuteurInGebruik(auteur))
            {
                MessageBox.Show("Deze auteur is nog in gebruik en kan momenteel niet verwijderd worden !", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (!boekenService.AuteurVerwijderen(auteur))
            {
                MessageBox.Show("We konden de auteur niet verwijderen !", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            PopulateAuteurs();
            txtAuteur.Text = "";
            lstAuteurs.SelectedIndex = -1;
        }

        private void LstUitgevers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            txtUitgever.Text = "";
            if (lstUitgevers.SelectedItem != null)
            {
                txtUitgever.Text = ((Uitgever)lstUitgevers.SelectedItem).Naam;
            }
        }
        private void BtnClearUitgever_Click(object sender, RoutedEventArgs e)
        {
            lstUitgevers.SelectedIndex = -1;
            txtUitgever.Text = "";
            txtUitgever.Focus();
        }
        private void BtnUitgeverToevoegen_Click(object sender, RoutedEventArgs e)
        {
            string naam = txtUitgever.Text.Trim();
            if (naam.Length == 0)
            {
                MessageBox.Show("Je dient een naam op te geven !", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                txtUitgever.Focus();
                return;
            }
            Uitgever uitgever = new Uitgever(naam);
            if (!boekenService.UitgeverToevoegen(uitgever))
            {
                MessageBox.Show("We konden de nieuwe uitgever niet toevoegen !", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                txtUitgever.Focus();
                return;
            }
            PopulateUitgevers();
            lstUitgevers.SelectedItem = uitgever;
            LstUitgevers_SelectionChanged(null, null);
        }
        private void BtnUitgeverWijzigen_Click(object sender, RoutedEventArgs e)
        {
            if (lstUitgevers.SelectedItem == null)
            {
                MessageBox.Show("Je dient eerst een uitgever te selecteren !", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            string naam = txtUitgever.Text.Trim();
            if (naam.Length == 0)
            {
                MessageBox.Show("Je dient een naam op te geven !", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                txtUitgever.Focus();
                return;
            }
            Uitgever uitgever = (Uitgever)lstUitgevers.SelectedItem;
            uitgever.Naam = naam;
            if (!boekenService.UitgeverWijzigen(uitgever))
            {
                MessageBox.Show("We konden de uitgever niet wijzigen !", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                txtUitgever.Focus();
                return;
            }
            PopulateUitgevers();
            lstUitgevers.SelectedItem = uitgever;
            LstUitgevers_SelectionChanged(null, null);
        }
        private void BtnUitgeverVerwijderen_Click(object sender, RoutedEventArgs e)
        {
            if (lstUitgevers.SelectedItem == null)
            {
                MessageBox.Show("Je dient eerst een uitgever te selecteren !", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            Uitgever uitgever = (Uitgever)lstUitgevers.SelectedItem;
            if (boekenService.IsUitgeverInGebruik(uitgever))
            {
                MessageBox.Show("Deze auteur is nog in gebruik en kan momenteel niet verwijderd worden !", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (!boekenService.UitgeverVerwijderen(uitgever))
            {
                MessageBox.Show("We konden de uitgever niet verwijderen !", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            PopulateUitgevers();
            txtUitgever.Text = "";
            lstUitgevers.SelectedIndex = -1;
        }

    }
}
