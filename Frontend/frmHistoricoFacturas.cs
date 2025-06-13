using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Frontend
{
    public partial class frmHistoricoFacturas : Form
    {
        private DatabaseHelper dbHelper = new DatabaseHelper();

        public frmHistoricoFacturas()
        {
            InitializeComponent();
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            try
            {
                int idFactura = int.Parse(txtIdFactura.Text);
                var factura = dbHelper.BuscarFacturaPorId(idFactura);

                if (factura != null)
                {
                    dgvDetalles.DataSource = factura.DetalleFactura.ToList();
                    lblInfoFactura.Text = $"Factura N° {factura.IdFactura} - Fecha: {factura.Fecha.ToShortDateString()} - Total: {factura.TotalPagar:C2}";
                }
                else
                {
                    MessageBox.Show("No se encontró la factura");
                    dgvDetalles.DataSource = null;
                    lblInfoFactura.Text = "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }
    }
}
