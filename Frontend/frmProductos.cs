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
    public partial class frmProductos : Form
    {
        private DatabaseHelper dbHelper = new DatabaseHelper();
        private List<Productos> productos = new List<Productos>();

        public frmProductos()
        {
            InitializeComponent();
            CargarProductos();
        }

        private void CargarProductos()
        {
            productos = dbHelper.ObtenerProductos();
            dgvProductos.DataSource = productos;
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
