using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Backend.Modelos;

namespace Frontend
{
    public partial class frmClientes : Form
    {
        private DatabaseHelper dbHelper = new DatabaseHelper();
        private List<Clientes> clientes = new List<Clientes>();

        public frmClientes()
        {
            InitializeComponent();
            CargarClientes();
        }

        private void CargarClientes()
        {
            clientes = dbHelper.ObtenerClientes();
            dgvClientes.DataSource = clientes;
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            try
            {
                var cliente = new Clientes
                {
                    TipoDocumento = cmbTipoDoc.Text,
                    Documento = txtDocumento.Text,
                    Nombre = txtNombre.Text,
                    FechaIngreso = dtpFechaIngreso.Value,
                    Categoria = cmbCategoria.Text
                };

                if (dbHelper.AgregarCliente(cliente))
                {
                    MessageBox.Show("Cliente agregado correctamente");
                    CargarClientes();
                    LimpiarCampos();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            if (dgvClientes.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione un cliente");
                return;
            }

            try
            {
                var cliente = (Clientes)dgvClientes.SelectedRows[0].DataBoundItem;
                cliente.TipoDocumento = cmbTipoDoc.Text;
                cliente.Documento = txtDocumento.Text;
                cliente.Nombre = txtNombre.Text;
                cliente.FechaIngreso = dtpFechaIngreso.Value;
                cliente.Categoria = cmbCategoria.Text;

                if (dbHelper.ActualizarCliente(cliente))
                {
                    MessageBox.Show("Cliente actualizado correctamente");
                    CargarClientes();
                    LimpiarCampos();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvClientes.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione un cliente");
                return;
            }

            try
            {
                var cliente = (Clientes)dgvClientes.SelectedRows[0].DataBoundItem;
                if (MessageBox.Show("¿Está seguro de eliminar este cliente?", "Confirmar", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    if (dbHelper.EliminarCliente(cliente.IdCliente))
                    {
                        MessageBox.Show("Cliente eliminado correctamente");
                        CargarClientes();
                        LimpiarCampos();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void dgvClientes_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvClientes.SelectedRows.Count > 0)
            {
                var cliente = (Clientes)dgvClientes.SelectedRows[0].DataBoundItem;
                cmbTipoDoc.Text = cliente.TipoDocumento;
                txtDocumento.Text = cliente.Documento;
                txtNombre.Text = cliente.Nombre;
                dtpFechaIngreso.Value = cliente.FechaIngreso;
                cmbCategoria.Text = cliente.Categoria;
            }
        }

        private void LimpiarCampos()
        {
            cmbTipoDoc.Text = "";
            txtDocumento.Text = "";
            txtNombre.Text = "";
            dtpFechaIngreso.Value = DateTime.Now;
            cmbCategoria.Text = "";
        }
    }
}
