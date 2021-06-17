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
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Topshelf.Logging;
using Serilog;

namespace FancyCashRegister.Forms
{
    public partial class LoginForm : Form
    {
        private readonly GebruikersRepository _gebruikersRepo;
        private readonly Config _config;
        

        public LoginForm()
        {
            InitializeComponent();
            _gebruikersRepo = new GebruikersRepository();
            _config = new ConfigRepository().GetAppConfig();
            
        }

        

        private void LoginForm_Load(object sender, EventArgs e)
        {
            IEnumerable<Screen> screens = Screen.AllScreens;

            Rectangle bounds = screens
                .Where(s => s.Primary == true)
                .First().Bounds;
            
            SetBounds(bounds.X, bounds.Y, bounds.Width, bounds.Height);
            //TopMost = true;
            FormBorderStyle = FormBorderStyle.None;
            //WindowState = FormWindowState.Maximized;

            lbGebruikers.DataSource = _gebruikersRepo.Gebruikers.ToList();
            lbGebruikers.ValueMember = nameof(Gebruiker.Id);
            lbGebruikers.DisplayMember = nameof(Gebruiker.Gebruikersnaam);
            lbGebruikers.ClearSelected();

            txtPincode.Mask = new string('0', _config.LengtePincode);
            lbGebruikers.Select();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            LogStart();
            if (lbGebruikers.SelectedItem is Gebruiker geselecteerdeGebruiker)
            {
                if (!geselecteerdeGebruiker.IsActief)
                {
                    MessageBox.Show("Gebruiker is niet geactiveerd", "Niet actief", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    Log.Error("Gebruiker is niet geactiveerd", "Niet actief", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
                else
                {
                    var ingevoerdePinCorrect = new DataHelper().CheckHash(txtPincode.Text, geselecteerdeGebruiker.Pincode);
                    //string ingevoerdePinCorrectMsg = "Login geslaagd"; weet niet waar dit nodig voor maar maybe komen we daar later nog achter.
                    if (true)
                    {

                    }
                    if (ingevoerdePinCorrect)
                    {
                        ConfigRepository.HuidigeGebruiker = geselecteerdeGebruiker;
                        Log.Information($"gebruiker ingelogt");
                        new MainForm().Show(this);
                        txtPincode.Text = string.Empty;
                        Hide();
                        
                    }
                }
            }
        }

        private void lbGebruikers_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtPincode.Text = string.Empty;
            txtPincode.Select();

        }

        private void btnAfsluiten_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        public void LogStart()
        {
            Log.Logger = new LoggerConfiguration()
                      .MinimumLevel.Debug()
                      .WriteTo.File(@"C:\Users\stefa\OneDrive\Desktop\amo-1e 2020-2021\blok-b jaar 1\pra\b5- KassaSysteem -\project\Kassasysteem\data\logs\logs.txt", outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}", rollingInterval: RollingInterval.Day)
                      .MinimumLevel.Debug()
                      .CreateLogger();
            Log.Information("============= Started Logging =============");

        }

    }
}
