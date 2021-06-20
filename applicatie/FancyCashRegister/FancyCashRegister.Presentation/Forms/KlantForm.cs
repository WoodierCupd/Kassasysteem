using FancyCashRegister.Domain.Models;
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
using Serilog;

namespace FancyCashRegister.Forms
{
    public partial class KlantForm : Form
    {
        private Timer _advertentieTimer;
        private AdvertentieRepository _advertentieRepo;


        public KlantForm(BindingSource bsOrderProducten)
        {
            InitializeComponent();
            _advertentieRepo = new AdvertentieRepository();

            this.bsProductenInOrder = bsOrderProducten;
            bsProductenInOrder.ListChanged += bsProductenInOrder_ListChanged;
            
            _advertentieTimer = new Timer()
            {
                Interval = 5000, // ms
            };

        }

        private void KlantForm_Load(object sender, EventArgs e)
        {
            
            IEnumerable<Screen> screens = Screen.AllScreens;
            
            // bij meerdere schermen op secundair scherm plaatsen
            if (screens.Count() > 1)
            {
                Rectangle bounds = screens
                    .Where(s => s.Primary == false)
                    .First().Bounds;

                SetBounds(bounds.X, bounds.Y, bounds.Width, bounds.Height);
                FormBorderStyle = FormBorderStyle.None;
            }

            //hiermee worden de producten ingeladen op het klanten form.
            dgProductenInOrder.DataSource = bsProductenInOrder;
            //hier starten de advertenties en hun timer
            _advertentieTimer.Tick += _advertentieTimer_Tick;
            _advertentieTimer.Start();
            Log.Information("message: klanten scherm geopend");
        }

        // deze events gaan over het verandere van de bedragen die te betalen/terug te geven/ gekregen zijn.
        internal void TeBetalenChanged(object sender, EventArgs e)
        {
            txtTeBetalen.Text = (sender as TextBox)?.Text;
        }

        public void TeruggaveChanged(object sender, EventArgs e)
        {
            txtTeruggave.Text = (sender as TextBox)?.Text;
        }

        public void OntvangenChanged(object sender, EventArgs e)
        {
            txtOntvangen.Text = (sender as TextBox)?.Text;
        }

        // van ad verandere gebeurt hier
        private void _advertentieTimer_Tick(object sender, EventArgs e)
        {
            pbAdvertentie01.Load(_advertentieRepo.GetNextAdUri());
        }

        //deselecteerd alle producten zodra...?
        private void dgProductenInOrder_DataSourceChanged(object sender, EventArgs e)
        {
            dgProductenInOrder.ClearSelection();
        }

        private void dgProductenInOrder_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            dgProductenInOrder.ClearSelection();
        }

        internal void OrderProductenChanged(BindingSource bindingSource)
        {
                //set product op null zodat er niet in de order staat
            bsProductenInOrder = null;
                //set het gewilde product
            bsProductenInOrder = bindingSource;
                // set de data source (ofwel waar de data vandaan komt) op null
            dgProductenInOrder.DataSource = null;
                // set de data source (of wel de data waar het vandaan komt) in op de selectie
            dgProductenInOrder.DataSource = bsProductenInOrder;
                // roept onderstaand methode aan in het bruin met 2 params
            bsProductenInOrder_ListChanged(this, null);
                // onderstaande regel voegt daad werkelijk het product toe aan de lijst van producten op het main form zodat dit door word geschakkeld naar het klantenForm
            bsProductenInOrder.ListChanged += bsProductenInOrder_ListChanged;
                // logged dat het product word toegevoegt
            
        }

        private void dgProductenInOrder_ColumnAdded(object sender, DataGridViewColumnEventArgs e)
        {
            var ignoreColumns = new[] { 
                nameof(OrderProduct.Id), 
                nameof(OrderProduct.Beschrijving), 
                nameof(OrderProduct.IsActief), 
                nameof(OrderProduct.Plus), 
                nameof(OrderProduct.Min), 
                nameof(OrderProduct.Categorie) 
            };
            var alignRightColumns = new[] { 
                nameof(OrderProduct.Stuksprijs), 
                nameof(OrderProduct.TotaalPrijs) 
            };
            var moneyColumns = new[] { 
                nameof(OrderProduct.Stuksprijs), 
                nameof(OrderProduct.TotaalPrijs)
            };

            if (ignoreColumns.Any(x => x == e.Column.Name))
            {
                e.Column.Visible = false;
            }

            if (alignRightColumns.Any(x => x == e.Column.Name))
            {
                e.Column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }

            if (moneyColumns.Any(x => x == e.Column.Name))
            {
                e.Column.DefaultCellStyle.Format = "c2";
            }
        }

        private void bsProductenInOrder_ListChanged(object sender, ListChangedEventArgs e)
        {
            var productenInOrder = bsProductenInOrder.DataSource as IEnumerable<OrderProduct>;
            var totaalPrijs = productenInOrder.Sum(p => p.TotaalPrijs);
            txtTeBetalen.Text = $"{totaalPrijs:c2}";
        }

        public void Reset()
        {
            txtTeBetalen.Text = $"{0:c2}";
            txtOntvangen.Text = $"{0:c2}";
            txtTeruggave.Text = $"{0:c2}";
        }
    }
}
