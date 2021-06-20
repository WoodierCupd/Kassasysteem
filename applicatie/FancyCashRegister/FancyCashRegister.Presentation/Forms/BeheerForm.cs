using FancyCashRegister.Domain.Models;
using FancyCashRegister.Services.Helpers;
using FancyCashRegister.Services.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Serilog;

namespace FancyCashRegister.Forms
{
    public partial class BeheerForm : Form
    {
        private readonly GebruikersRepository _gebruikersRepo;
        private readonly Config _config;


        public BeheerForm()
        {
            InitializeComponent();
            _gebruikersRepo = new GebruikersRepository();
            _config = new ConfigRepository().GetAppConfig();
        }

        private void btnAfsluiten_Click(object sender, EventArgs e)
        {
            var doorgaanMetAflsuiten = true;
            if (_config.BevestigAfsluiten)
            {
                if (MessageBox.Show("De applicatie wordt afgesloten. Wilt u doorgaan?", "Afsluiten", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.No)
                {
                    doorgaanMetAflsuiten = false;
                    Log.Debug("programma asluiten werd gecanceld");
                }

            }

            if (doorgaanMetAflsuiten)
            {
                Application.Exit();
                Log.Debug("programma asluiten werd afgesloten via beheer scherm");
                Log.Debug("============= End run Logging =============");
            }
        }

        private void BeheerForm_Load(object sender, EventArgs e)
        {
            IEnumerable<Screen> screens = Screen.AllScreens;

            Rectangle bounds = screens
                .Where(s => s.Primary == true)
                .First().Bounds;

            SetBounds(bounds.X, bounds.Y, bounds.Width, bounds.Height);
            //TopMost = true;
            FormBorderStyle = FormBorderStyle.None;

            // TODO: rol heeft nog geen niveau, match op string is niet ideaal -->
            if (ConfigRepository.HuidigeGebruiker.Rol.Id == _gebruikersRepo.Rollen
                .Where(r => r.Naam == "beheerder")
                .FirstOrDefault()?.Id)
            {
                tcBeheerTabs.Enabled = true;
            }

            lbGebruikers.DataSource = _gebruikersRepo.Gebruikers.ToList();
            lbGebruikers.ValueMember = nameof(Gebruiker.Id);
            lbGebruikers.DisplayMember = nameof(Gebruiker.Gebruikersnaam);

            lbRollen.DataSource = _gebruikersRepo.Rollen.ToList();
            lbRollen.ValueMember = nameof(Rol.Id);
            lbRollen.DisplayMember = nameof(Rol.Naam);

            txtPincode.Mask = new string('0', _config.LengtePincode);
            lbGebruikers.ClearSelected();
            lbRollen.ClearSelected();

            txtGebruikerId.Text = string.Empty;
            txtGebruikersnaam.Text = string.Empty;
            txtVolledigeNaam.Text = string.Empty;
            txtPincode.Text = string.Empty;
            chkKanInloggen.Checked = false;
            Log.Debug("beheer scherm werd geopend");
        }

        private void BeheerForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Owner?.Show();
        }

        private void btnTerug_Click(object sender, EventArgs e)
        {
            Owner?.Show();
            Close();
            Log.Information("gebruiker ging terug naar hooftscherm");
        }

        private void lbGebruikers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbGebruikers.SelectedItem is Gebruiker geselecteerdeGebruiker)
            {
                txtGebruikerId.Text = geselecteerdeGebruiker.Id.ToString();
                txtGebruikersnaam.Text = geselecteerdeGebruiker.Gebruikersnaam; 
                txtVolledigeNaam.Text = geselecteerdeGebruiker.VolledigeNaam;
                chkKanInloggen.Checked = geselecteerdeGebruiker.IsActief;
                Log.Debug($"ander gebruiker werd geselecteerd ({geselecteerdeGebruiker.Gebruikersnaam })");
                foreach (var rolItem in lbRollen.Items)
                {
                    if (rolItem is Rol rol)
                    {
                        if (rol.Id == geselecteerdeGebruiker.Rol.Id)
                        {
                            lbRollen.SelectedItem = rol;
                            break;
                        }
                    }
                }
            }
        }

        private void btnOpslaan_Click(object sender, EventArgs e)
        {
            if (lbGebruikers.SelectedItem != null ||
                string.IsNullOrWhiteSpace(txtPincode.Text) == false
                && (txtPincode.Text.Length < _config.LengtePincode
                    || Regex.IsMatch(txtPincode.Text, "[^0-9]")))
            {
                MessageBox.Show("Pincode voldoet niet aan de eisen", "Pincode ongeldig", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Log.Error("Pincode voldoet niet aan de eisen");
            }
            else
            {
                var rolId = 0;
                if (lbRollen.SelectedItem is Rol rol)
                {
                    rolId = rol.Id;
                }

                var aangepasteGebruiker = new Gebruiker
                {
                    Id = int.Parse(txtGebruikerId.Text),
                    RolId = rolId,
                    Gebruikersnaam = txtGebruikersnaam.Text,
                    VolledigeNaam = txtVolledigeNaam.Text,
                    Pincode = string.IsNullOrWhiteSpace(txtPincode.Text)
                    ? null
                    : new DataHelper().GetSha256Hash(txtPincode.Text),
                    IsActief = chkKanInloggen.Checked,
                };

                if (_gebruikersRepo.UpdateGebruiker(aangepasteGebruiker))
                {
                    txtGebruikerId.Text = string.Empty;
                    txtGebruikersnaam.Text = string.Empty;
                    txtVolledigeNaam.Text = string.Empty;
                    txtPincode.Text = string.Empty;
                    chkKanInloggen.Checked = false;
                    lbGebruikers.ClearSelected();
                    lbRollen.ClearSelected();

                    MessageBox.Show("Aanpassingen opgeslagen", "Opgeslagen", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Log.Debug("Aanpassingen opgeslagen");
                }
                else
                {
                    MessageBox.Show("Fout opgetreden bij opslaan wijzigingen", "Fout opgetreden", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    
                    //ik probeerde hier de foutmelding weer te geven. dit lukt alleen nog niet
                    //Log.Error(Exception + "Fout opgetreden bij opslaan wijzigingen");
                    //Log.Error("This is an exception");
                }
            }
        }

        private void btnAfmelden_Click(object sender, EventArgs e)
        {
            ConfigRepository.HuidigeGebruiker = null;
            Owner.Owner.Show();
            Owner.Close();
            Close();
            Log.Debug("huidige gebruiker afgemeld");

        }

        private void chkKanInloggen_CheckedChanged(object sender, EventArgs e)
        {
            //bool checkBox = chkKanInloggen.Checked;
            //if (checkBox)
            //{
            //    Log.Information("staat van inloggen van de huidige gebruiker werd veranderd naar /true/");
            //}
            //else
            //{
            //    Log.Information("staat van inloggen van de huidige gebruiker werd veranderd naar /false/");
            //}
        }
    }
}
