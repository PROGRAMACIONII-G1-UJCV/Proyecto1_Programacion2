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
    public partial class frmEmpleados : Form
    {
        private DatabaseHelper dbHelper = new DatabaseHelper();
        private List<Empleados> empleados = new List<Empleados>();

        public frmEmpleados()
        {
            InitializeComponent();
            CargarEmpleados();
            ConfigurarControles();
        }
        private void ConfigurarControles()
        {

            dtpFechaIngreso.Format = DateTimePickerFormat.Short;
            dtpFechaIngreso.MaxDate = DateTime.Today;
            txtDocumento.Validating += new CancelEventHandler(ValidarDocumento);
            txtNombre.Validating += new CancelEventHandler(ValidarNombre);
            txtCodigo.Validating += new CancelEventHandler(ValidarCodigo);
        }

        private void CargarEmpleados()
        {
            empleados = dbHelper.ObtenerEmpleados();
            dgvEmpleados.DataSource = empleados;
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
        private void ValidarCodigo(object sender, EventArgs e)
        {
            string codigo = txtCodigo.Text.Trim();

            if (!string.IsNullOrEmpty(codigo) && dbHelper.ExisteCodigoEmpleado(codigo))
            {
                MessageBox.Show("Este código ya está en uso", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCodigo.Focus();
            }
            else if (!Regex.IsMatch(codigo, @"^[a-zA-Z0-9]+$"))
            {
                MessageBox.Show("El código solo puede contener letras y números", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCodigo.Focus();
            }
            else if (string.IsNullOrWhiteSpace(codigo))
            {
                MessageBox.Show("El código no puede estar vacío", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCodigo.Focus();
            }
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (!ValidarFormulario()) return;

            try
            {
                if (dbHelper.ExisteDocumentoEmpleado(txtDocumento.Text.Trim()))
                {
                    MessageBox.Show("Ya existe un empleado con este documento", "Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtDocumento.Focus();
                    return;
                }
                var empleado = new Empleados
                {
                    Codigo = txtCodigo.Text.Trim(),
                    Nombre = txtNombre.Text.Trim(),
                    FechaIngreso = dtpFechaIngreso.Value.Date,
                    Puesto = cmbPuesto.Text.Trim(),
                    TipoDocumento = cmbTipoDoc.Text.Trim(),
                    Documento = txtDocumento.Text.Trim()
                };

                if (dbHelper.AgregarEmpleado(empleado))
                {
                    MessageBox.Show("Empleado agregado correctamente", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CargarEmpleados();
                    LimpiarCampos();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al agregar empleado: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidarFormulario()
        {
            if (string.IsNullOrWhiteSpace(txtCodigo.Text))
            {
                MessageBox.Show("Ingrese el código del empleado", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCodigo.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("Ingrese el nombre del empleado", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNombre.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(cmbPuesto.Text))
            {
                MessageBox.Show("Seleccione el puesto del empleado", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbPuesto.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtDocumento.Text))
            {
                MessageBox.Show("Ingrese el documento del empleado", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDocumento.Focus();
                return false;
            }

            return true;
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            if (dgvEmpleados.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione un empleado", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!ValidarFormulario()) return;

            try
            {
                var empleado = (Empleados)dgvEmpleados.SelectedRows[0].DataBoundItem;
                empleado.Codigo = txtCodigo.Text.Trim();
                empleado.Nombre = txtNombre.Text.Trim();
                empleado.FechaIngreso = dtpFechaIngreso.Value.Date;
                empleado.Puesto = cmbPuesto.Text.Trim();
                empleado.TipoDocumento = cmbTipoDoc.Text.Trim();
                empleado.Documento = txtDocumento.Text.Trim();

                if (dbHelper.ActualizarEmpleado(empleado))
                {
                    MessageBox.Show("Empleado actualizado correctamente", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CargarEmpleados();
                    LimpiarCampos();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al actualizar empleado: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvEmpleados.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione un empleado", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var empleado = (Empleados)dgvEmpleados.SelectedRows[0].DataBoundItem;

                
                if (dbHelper.EmpleadoTieneFacturas(empleado.IdEmpleado))
                {
                    MessageBox.Show("No se puede eliminar el empleado porque tiene facturas asociadas",
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (MessageBox.Show($"¿Está seguro de eliminar al empleado {empleado.Nombre}?",
                    "Confirmar Eliminación",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (dbHelper.EliminarEmpleado(empleado.IdEmpleado))
                    {
                        MessageBox.Show("Empleado eliminado correctamente", "Éxito",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        CargarEmpleados();
                        LimpiarCampos();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar empleado: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvEmpleados_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvEmpleados.SelectedRows.Count > 0)
            {
                var empleado = (Empleados)dgvEmpleados.SelectedRows[0].DataBoundItem;
                txtCodigo.Text = empleado.Codigo;
                txtNombre.Text = empleado.Nombre;
                dtpFechaIngreso.Value = empleado.FechaIngreso;
                cmbPuesto.Text = empleado.Puesto;
                cmbTipoDoc.Text = empleado.TipoDocumento;
                txtDocumento.Text = empleado.Documento;
            }
        }

        private void LimpiarCampos()
        {
            txtCodigo.Clear();
            txtNombre.Clear();
            dtpFechaIngreso.Value = DateTime.Today;
            cmbPuesto.SelectedIndex = -1;
            cmbTipoDoc.SelectedIndex = -1;
            txtDocumento.Clear();
        }
    }
}
