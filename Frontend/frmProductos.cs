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
    public partial class frmProductos : Form
    {
        private DatabaseHelper dbHelper = new DatabaseHelper();
        private List<Productos> productos = new List<Productos>();

        public frmProductos()
        {
            InitializeComponent();
            CargarProductos();
            ConfigurarControles();
        }

        private void ConfigurarControles()
        {

            txtCodigo.Validating += new CancelEventHandler(ValidarCodigo);
            txtNombre.Validating += new CancelEventHandler(ValidarNombre);
        }
        private void CargarProductos()
        {
            productos = dbHelper.ObtenerProductos();
            dgvProductos.DataSource = productos;
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
       
        private void ValidarCodigo(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCodigo.Text))
            {
                MessageBox.Show("El código no puede estar vacío", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                e.Cancel = true;
            }
            else if (!Regex.IsMatch(txtCodigo.Text.Trim(), @"^[a-zA-Z0-9]+$"))
            {
                MessageBox.Show("El código solo puede contener letras y números", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                e.Cancel = true;
            }
            else if (dbHelper.ExisteProducto(txtCodigo.Text.Trim()))
            {
                MessageBox.Show("Este código ya está en uso", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                e.Cancel = true;
            }
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            try
            {
                var producto = new Productos
                {
                    Codigo = txtCodigo.Text,
                    Nombre = txtNombre.Text,
                    Precio = decimal.Parse(txtPrecio.Text),
                    Stock = int.Parse(txtStock.Text)
                };
                if (string.IsNullOrWhiteSpace(producto.Codigo) || string.IsNullOrWhiteSpace(producto.Nombre))
                {
                    MessageBox.Show("El código y el nombre son obligatorios");
                    return;
                }
                if (producto.Precio <= 0)
                {
                    MessageBox.Show("El precio debe ser mayor a cero");
                    return;
                }
                if (producto.Stock < 0)
                {
                    MessageBox.Show("El stock no puede ser negativo");
                    return;
                }

                if (dbHelper.AgregarProducto(producto))
                {
                    MessageBox.Show("Producto agregado correctamente");
                    CargarProductos();
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
            if (dgvProductos.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione un producto");
                return;
            }

            try
            {
                var producto = (Productos)dgvProductos.SelectedRows[0].DataBoundItem;
                producto.Codigo = txtCodigo.Text;
                producto.Nombre = txtNombre.Text;
                producto.Precio = decimal.Parse(txtPrecio.Text);
                producto.Stock = int.Parse(txtStock.Text);

                if (dbHelper.ActualizarProducto(producto))
                {
                    MessageBox.Show("Producto actualizado correctamente");
                    CargarProductos();
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
            if (dgvProductos.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione un producto");
                return;
            }

            try
            {
                var producto = (Productos)dgvProductos.SelectedRows[0].DataBoundItem;
                if (MessageBox.Show("¿Está seguro de eliminar este producto?", "Confirmar", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    if (dbHelper.EliminarProducto(producto.IdProducto))
                    {
                        MessageBox.Show("Producto eliminado correctamente");
                        CargarProductos();
                        LimpiarCampos();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void dgvProductos_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvProductos.SelectedRows.Count > 0)
            {
                var producto = (Productos)dgvProductos.SelectedRows[0].DataBoundItem;
                txtCodigo.Text = producto.Codigo;
                txtNombre.Text = producto.Nombre;
                txtPrecio.Text = producto.Precio.ToString();
                txtStock.Text = producto.Stock.ToString();
            }
        }

        private void LimpiarCampos()
        {
            txtCodigo.Text = "";
            txtNombre.Text = "";
            txtPrecio.Text = "";
            txtStock.Text = "";
        }
    }
}
