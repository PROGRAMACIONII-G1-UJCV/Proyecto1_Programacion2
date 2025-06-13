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
    public partial class frmEmpleados : Form
    {
        private DatabaseHelper dbHelper = new DatabaseHelper();
        private List<Empleados> empleados = new List<Empleados>();

        public frmEmpleados()
        {
            InitializeComponent();
            CargarEmpleados();
        }

        private void CargarEmpleados()
        {
            empleados = dbHelper.ObtenerEmpleados();
            dgvEmpleados.DataSource = empleados;
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            try
            {
                var empleado = new Empleados
                {
                    Codigo = txtCodigo.Text,
                    Nombre = txtNombre.Text,
                    FechaIngreso = dtpFechaIngreso.Value,
                    Puesto = cmbPuesto.Text,
                    TipoDocumento = cmbTipoDoc.Text,
                    Documento = txtDocumento.Text
                };

                if (dbHelper.AgregarEmpleado(empleado))
                {
                    MessageBox.Show("Empleado agregado correctamente");
                    CargarEmpleados();
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
            if (dgvEmpleados.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione un empleado");
                return;
            }

            try
            {
                var empleado = (Empleados)dgvEmpleados.SelectedRows[0].DataBoundItem;
                empleado.Codigo = txtCodigo.Text;
                empleado.Nombre = txtNombre.Text;
                empleado.FechaIngreso = dtpFechaIngreso.Value;
                empleado.Puesto = cmbPuesto.Text;
                empleado.TipoDocumento = cmbTipoDoc.Text;
                empleado.Documento = txtDocumento.Text;

                if (dbHelper.ActualizarEmpleado(empleado))
                {
                    MessageBox.Show("Empleado actualizado correctamente");
                    CargarEmpleados();
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
            if (dgvEmpleados.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione un empleado");
                return;
            }

            try
            {
                var empleado = (Empleados)dgvEmpleados.SelectedRows[0].DataBoundItem;
                if (MessageBox.Show("¿Está seguro de eliminar este empleado?", "Confirmar", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    if (dbHelper.EliminarEmpleado(empleado.IdEmpleado))
                    {
                        MessageBox.Show("Empleado eliminado correctamente");
                        CargarEmpleados();
                        LimpiarCampos();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
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
            txtCodigo.Text = "";
            txtNombre.Text = "";
            dtpFechaIngreso.Value = DateTime.Now;
            cmbPuesto.Text = "";
            cmbTipoDoc.Text = "";
            txtDocumento.Text = "";
        }
    }
}
