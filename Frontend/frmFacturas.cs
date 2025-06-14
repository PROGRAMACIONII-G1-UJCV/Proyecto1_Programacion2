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
    public partial class frmFacturas : Form
    {
        private DatabaseHelper dbHelper = new DatabaseHelper();
        private List<Clientes> clientes = new List<Clientes>();
        private List<Empleados> empleados = new List<Empleados>();
        private List<Productos> productos = new List<Productos>();
        private List<DetalleFactura> detalles = new List<DetalleFactura>();

        public frmFacturas()
        {
            InitializeComponent();
            CargarDatosIniciales();
            ConfigurarControles();
        }
        private void ConfigurarControles()
        {
            txtSubTotal.Validating += new CancelEventHandler(ValidarCantidad);

        }
        private void CargarDatosIniciales()
        {
            clientes = dbHelper.ObtenerClientes();
            empleados = dbHelper.ObtenerEmpleados();
            productos = dbHelper.ObtenerProductos();

            cmbCliente.DataSource = clientes;
            cmbCliente.DisplayMember = "Nombre";
            cmbCliente.ValueMember = "IdCliente";

            cmbEmpleado.DataSource = empleados;
            cmbEmpleado.DisplayMember = "Nombre";
            cmbEmpleado.ValueMember = "IdEmpleado";

            cmbProducto.DataSource = productos;
            cmbProducto.DisplayMember = "Nombre";
            cmbProducto.ValueMember = "IdProducto";

            dtpFecha.Value = DateTime.Now;
        }
        private void ValidarCantidad(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCantidad.Text) || !int.TryParse(txtCantidad.Text, out int cantidad) || cantidad <= 0)
            {
                MessageBox.Show("La cantidad debe ser un número mayor a cero", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                e.Cancel = true;
            }
        }

        private void btnAgregarProducto_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbProducto.SelectedItem == null)
                {
                    MessageBox.Show("Seleccione un producto");
                    return;
                }

                int cantidad = int.Parse(txtCantidad.Text);
                if (cantidad <= 0)
                {
                    MessageBox.Show("La cantidad debe ser mayor a cero");
                    return;
                }

                var productoSeleccionado = (Productos)cmbProducto.SelectedItem;
                if (productoSeleccionado.Stock < cantidad)
                {
                    MessageBox.Show("No hay suficiente stock");
                    return;
                }

                var detalle = new DetalleFactura
                {
                    IdProducto = productoSeleccionado.IdProducto,
                    NombreProducto = productoSeleccionado.Nombre,
                    Precio = productoSeleccionado.Precio,
                    CantidadLlevada = cantidad,
                    SubTotal = productoSeleccionado.Precio * cantidad
                };

                detalles.Add(detalle);
                ActualizarDetalles();
                CalcularTotales();
                txtCantidad.Text = "1";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void btnQuitarProducto_Click(object sender, EventArgs e)
        {
            if (dgvDetalles.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione un producto para quitar");
                return;
            }

            var detalleSeleccionado = (DetalleFactura)dgvDetalles.SelectedRows[0].DataBoundItem;
            detalles.Remove(detalleSeleccionado);
            ActualizarDetalles();
            CalcularTotales();
        }

        private void ActualizarDetalles()
        {
            dgvDetalles.DataSource = null;
            dgvDetalles.DataSource = detalles;
        }

        private void CalcularTotales()
        {
            decimal subtotal = detalles.Sum(d => d.SubTotal);
            decimal isv = subtotal * 0.15m; // 15% de ISV
            decimal descuento = 0; // Podría implementar lógica de descuento según categoría del cliente

            if (cmbCliente.SelectedItem != null)
            {
                var cliente = (Clientes)cmbCliente.SelectedItem;
                if (cliente.Categoria == "VIP")
                    descuento = subtotal * 0.05m; // 5% de descuento para VIP
                else if (cliente.Categoria == "Premium")
                    descuento = subtotal * 0.10m; // 10% de descuento para Premium
            }

            decimal total = subtotal + isv - descuento;

            txtSubTotal.Text = subtotal.ToString("N2");
            txtISV.Text = isv.ToString("N2");
            txtDescuento.Text = descuento.ToString("N2");
            txtTotal.Text = total.ToString("N2");
        }

        private void btnGuardarFactura_Click(object sender, EventArgs e)
        {
            try
            {
                if (detalles.Count == 0)
                {
                    MessageBox.Show("Agregue al menos un producto a la factura");
                    return;
                }

                
                decimal subtotal, isv, descuento, total;

                if (!decimal.TryParse(txtSubTotal.Text.Replace("$", "").Replace(",", ""), out subtotal))
                {
                    MessageBox.Show("El subtotal no tiene un formato válido");
                    return;
                }

                if (!decimal.TryParse(txtISV.Text.Replace("$", "").Replace(",", ""), out isv))
                {
                    MessageBox.Show("El ISV no tiene un formato válido");
                    return;
                }

                if (!decimal.TryParse(txtDescuento.Text.Replace("$", "").Replace(",", ""), out descuento))
                {
                    MessageBox.Show("El descuento no tiene un formato válido");
                    return;
                }

                if (!decimal.TryParse(txtTotal.Text.Replace("$", "").Replace(",", ""), out total))
                {
                    MessageBox.Show("El total no tiene un formato válido");
                    return;
                }

                var factura = new Facturas
                {
                    Fecha = dtpFecha.Value,
                    IdCliente = ((Clientes)cmbCliente.SelectedItem).IdCliente,
                    IdEmpleado = ((Empleados)cmbEmpleado.SelectedItem).IdEmpleado,
                    SubTotal = subtotal,
                    ISV = isv,
                    Descuento = descuento,
                    TotalPagar = total,
                    DetalleFactura = detalles
                };

                int idFactura = dbHelper.AgregarFactura(factura);
                if (idFactura > 0)
                {
                    MessageBox.Show($"Factura guardada correctamente. N° {idFactura}");
                    LimpiarFormulario();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar factura: {ex.Message}");
            }
        }

        private void LimpiarFormulario()
        {
            detalles.Clear();
            ActualizarDetalles();
            CalcularTotales();
            dtpFecha.Value = DateTime.Now;
        }

        private void cmbProducto_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbProducto.SelectedItem != null)
            {
                var producto = (Productos)cmbProducto.SelectedItem;
                txtPrecio.Text = producto.Precio.ToString("C2");
            }
        }

        private void cmbCliente_SelectedIndexChanged(object sender, EventArgs e)
        {
            CalcularTotales();
        }
    }
}
