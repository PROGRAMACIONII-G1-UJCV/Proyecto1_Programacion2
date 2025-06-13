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
    public partial class frmPrincipal : Form
    {
        public frmPrincipal()
        {
            InitializeComponent();
        }
        private void btnProductos_Click(object sender, EventArgs e)
        {
            var frm = new frmProductos();
            frm.ShowDialog();
        }

        private void btnClientes_Click(object sender, EventArgs e)
        {
            var frm = new frmClientes();
            frm.ShowDialog();
        }

        private void btnEmpleados_Click(object sender, EventArgs e)
        {
            var frm = new frmEmpleados();
            frm.ShowDialog();
        }

        private void btnFacturas_Click(object sender, EventArgs e)
        {
            var frm = new frmFacturas();
            frm.ShowDialog();
        }

        private void btnHistoricoFacturas_Click(object sender, EventArgs e)
        {
            var frm = new frmHistoricoFacturas();
            frm.ShowDialog();
        }
    }
}
