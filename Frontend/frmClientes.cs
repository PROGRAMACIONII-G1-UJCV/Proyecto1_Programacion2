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
            ConfigurarControles();
        }

        private void ConfigurarControles()
        {
            
            dtpFechaIngreso.Format = DateTimePickerFormat.Short;
            dtpFechaIngreso.MaxDate = DateTime.Today;
            txtDocumento.Validating += new CancelEventHandler(ValidarDocumento);
            txtNombre.Validating += new CancelEventHandler(ValidarNombre);
        }

        private void CargarClientes()
        {
            try
            {
                clientes = dbHelper.ObtenerClientes();
                dgvClientes.AutoGenerateColumns = false;
                dgvClientes.DataSource = new BindingList<Clientes>(clientes);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar clientes: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        
        private bool ValidarFormulario()
        {
            if (string.IsNullOrWhiteSpace(cmbTipoDoc.Text))
            {
                MessageBox.Show("Seleccione el tipo de documento", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbTipoDoc.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtDocumento.Text))
            {
                MessageBox.Show("Ingrese el número de documento", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDocumento.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("Ingrese el nombre del cliente", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNombre.Focus();
                return false;
            }

            if (dtpFechaIngreso.Value > DateTime.Today)
            {
                MessageBox.Show("La fecha de ingreso no puede ser futura", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dtpFechaIngreso.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(cmbCategoria.Text))
            {
                MessageBox.Show("Seleccione la categoría del cliente", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbCategoria.Focus();
                return false;
            }

            return true;
        }

        private void ValidarDocumento(object sender, CancelEventArgs e)
        {
            string tipoDoc = cmbTipoDoc.Text;
            string documento = txtDocumento.Text.Trim();

            if (string.IsNullOrEmpty(documento)) return;

            switch (tipoDoc)
            {
                case "DNI":
                    if (!Regex.IsMatch(documento, @"^\d{13}$"))
                    {
                        MessageBox.Show("El DNI debe tener 13 dígitos", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        e.Cancel = true;
                    }
                    break;
                case "RUC":
                    if (!Regex.IsMatch(documento, @"^\d{11}$"))
                    {
                        MessageBox.Show("El RUC debe tener 11 dígitos", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        e.Cancel = true;
                    }
                    break;
                case "Pasaporte":
                    if (!Regex.IsMatch(documento, @"^[a-zA-Z0-9]{6,12}$"))
                    {
                        MessageBox.Show("Formato de pasaporte inválido", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        e.Cancel = true;
                    }
                    break;
            }
        }

        private void ValidarNombre(object sender, CancelEventArgs e)
        {
            if (txtNombre.Text.Trim().Length < 3)
            {
                MessageBox.Show("El nombre debe tener al menos 3 caracteres", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                e.Cancel = true;
            }
            else if (!Regex.IsMatch(txtNombre.Text.Trim(), @"^[a-zA-Z\s]+$"))
            {
                MessageBox.Show("El nombre solo puede contener letras y espacios", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                e.Cancel = true;
            }
        }

        
        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (!ValidarFormulario()) return;

            try
            {
                if (dbHelper.ExisteCliente(txtDocumento.Text.Trim()))
                {
                    MessageBox.Show("Ya existe un cliente con este documento", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var cliente = new Clientes
                {
                    TipoDocumento = cmbTipoDoc.Text.Trim(),
                    Documento = txtDocumento.Text.Trim(),
                    Nombre = txtNombre.Text.Trim(),
                    FechaIngreso = dtpFechaIngreso.Value.Date,
                    Categoria = cmbCategoria.Text.Trim()
                };

                if (dbHelper.AgregarCliente(cliente))
                {
                    MessageBox.Show("Cliente agregado correctamente", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CargarClientes();
                    LimpiarCampos();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al agregar cliente: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            if (dgvClientes.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione un cliente", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!ValidarFormulario()) return;

            try
            {
                var clienteActual = (Clientes)dgvClientes.SelectedRows[0].DataBoundItem;

                
                if (clienteActual.Documento != txtDocumento.Text.Trim() &&
                    dbHelper.ExisteCliente(txtDocumento.Text.Trim()))
                {
                    MessageBox.Show("Ya existe un cliente con este documento", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                clienteActual.TipoDocumento = cmbTipoDoc.Text.Trim();
                clienteActual.Documento = txtDocumento.Text.Trim();
                clienteActual.Nombre = txtNombre.Text.Trim();
                clienteActual.FechaIngreso = dtpFechaIngreso.Value.Date;
                clienteActual.Categoria = cmbCategoria.Text.Trim();

                if (dbHelper.ActualizarCliente(clienteActual))
                {
                    MessageBox.Show("Cliente actualizado correctamente", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CargarClientes();
                    LimpiarCampos();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al actualizar cliente: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvClientes.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione un cliente", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var cliente = (Clientes)dgvClientes.SelectedRows[0].DataBoundItem;

                
                if (dbHelper.ClienteTieneFacturas(cliente.IdCliente))
                {
                    MessageBox.Show("No se puede eliminar el cliente porque tiene facturas asociadas", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (MessageBox.Show($"¿Está seguro de eliminar al cliente {cliente.Nombre}?",
                    "Confirmar Eliminación",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (dbHelper.EliminarCliente(cliente.IdCliente))
                    {
                        MessageBox.Show("Cliente eliminado correctamente", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        CargarClientes();
                        LimpiarCampos();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar cliente: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            cmbTipoDoc.SelectedIndex = -1;
            txtDocumento.Clear();
            txtNombre.Clear();
            dtpFechaIngreso.Value = DateTime.Today;
            cmbCategoria.SelectedIndex = -1;
        }

        private void cmbTipoDoc_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            if (!string.IsNullOrEmpty(txtDocumento.Text))
            {
                ValidarDocumento(txtDocumento, new CancelEventArgs());
            }
        }
    }
}
