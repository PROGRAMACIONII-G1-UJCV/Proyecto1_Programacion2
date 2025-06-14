using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backend.Modelos;
using System.Windows.Forms;
using System.Data.Entity;

namespace Frontend
{
    public class DatabaseHelper
    {
        private ProyectoEntities CrearContexto()
        {
            return new ProyectoEntities();
        }

        #region Métodos para Productos
        public List<Productos> ObtenerProductos()
        {
            using (var context = CrearContexto())
            {
                return context.Productos
            .Include(p => p.DetalleFactura) 
            .AsNoTracking() 
            .ToList();
            }
        }

        public bool AgregarProducto(Productos producto)
        {
            try
            {
                using (var context = CrearContexto())
                {
                    context.Productos.Add(producto);
                    return context.SaveChanges() > 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al agregar producto: {ex.Message}");
                return false;
            }
        }

        public bool ActualizarProducto(Productos producto)
        {
            try
            {
                using (var context = CrearContexto())
                {
                    var productoExistente = context.Productos.Find(producto.IdProducto);
                    if (productoExistente != null)
                    {
                        context.Entry(productoExistente).CurrentValues.SetValues(producto);
                        return context.SaveChanges() > 0;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al actualizar producto: {ex.Message}");
                return false;
            }
        }

        public bool EliminarProducto(int id)
        {
            try
            {
                using (var context = CrearContexto())
                {
                    var producto = context.Productos.Find(id);
                    if (producto != null)
                    {
                        context.Productos.Remove(producto);
                        return context.SaveChanges() > 0;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar producto: {ex.Message}");
                return false;
            }
        }
        #endregion

        #region Métodos para Clientes
        public List<Clientes> ObtenerClientes()
        {
            using (var context = CrearContexto())
            {
                return context.Clientes.ToList();
            }
        }

        public bool AgregarCliente(Clientes cliente)
        {
            try
            {
                using (var context = CrearContexto())
                {
                    context.Clientes.Add(cliente);
                    return context.SaveChanges() > 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al agregar cliente: {ex.Message}");
                return false;
            }
        }

        public bool ActualizarCliente(Clientes cliente)
        {
            try
            {
                using (var context = CrearContexto())
                {
                    var clienteExistente = context.Clientes.Find(cliente.IdCliente);
                    if (clienteExistente != null)
                    {
                        context.Entry(clienteExistente).CurrentValues.SetValues(cliente);
                        return context.SaveChanges() > 0;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al actualizar cliente: {ex.Message}");
                return false;
            }
        }

        public bool EliminarCliente(int id)
        {
            try
            {
                using (var context = CrearContexto())
                {
                    var cliente = context.Clientes.Find(id);
                    if (cliente != null)
                    {
                        context.Clientes.Remove(cliente);
                        return context.SaveChanges() > 0;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar cliente: {ex.Message}");
                return false;
            }
        }
        #endregion

        #region Métodos para Empleados
        public List<Empleados> ObtenerEmpleados(bool includeFacturas = false)
        {
            using (var context = CrearContexto())
            {
                var query = context.Empleados.AsQueryable();

                if (includeFacturas)
                {
                    query = query.Include(e => e.Facturas);
                }

                return query.AsNoTracking().ToList();
            }
        }

        public bool EmpleadoTieneFacturas(int idEmpleado)
        {
            using (var context = CrearContexto())
            {
                return context.Facturas.Any(f => f.IdEmpleado == idEmpleado);
            }
        }

        public bool AgregarEmpleado(Empleados empleado)
        {
            try
            {
                using (var context = CrearContexto())
                {
                    context.Empleados.Add(empleado);
                    return context.SaveChanges() > 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al agregar empleado: {ex.Message}");
                return false;
            }
        }

        public bool ActualizarEmpleado(Empleados empleado)
        {
            try
            {
                using (var context = CrearContexto())
                {
                    var empleadoExistente = context.Empleados.Find(empleado.IdEmpleado);
                    if (empleadoExistente != null)
                    {
                        context.Entry(empleadoExistente).CurrentValues.SetValues(empleado);
                        return context.SaveChanges() > 0;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al actualizar empleado: {ex.Message}");
                return false;
            }
        }

        public bool EliminarEmpleado(int id)
        {
            try
            {
                using (var context = CrearContexto())
                {
                    var empleado = context.Empleados.Find(id);
                    if (empleado != null)
                    {
                        context.Empleados.Remove(empleado);
                        return context.SaveChanges() > 0;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar empleado: {ex.Message}");
                return false;
            }
        }
        #endregion

        #region Métodos para Facturas
        public List<Facturas> ObtenerFacturas()
        {
            using (var context = CrearContexto())
            {
                return context.Facturas
                    .Include(f => f.Clientes)
                    .Include(f => f.Empleados)
                    .Include(f => f.DetalleFactura)
                    .ToList();
            }
        }

        public List<DetalleFactura> ObtenerDetallesFactura(int idFactura)
        {
            using (var context = CrearContexto())
            {
                return context.DetalleFactura
                    .Where(d => d.IdFactura == idFactura)
                    .Include(d => d.Productos)
                    .ToList();
            }
        }

        public int AgregarFactura(Facturas factura)
        {
            try
            {
                using (var context = CrearContexto())
                {
                    
                    if (factura.SubTotal < 0 || factura.ISV < 0 || factura.Descuento < 0 || factura.TotalPagar < 0)
                    {
                        MessageBox.Show("Los valores numéricos no pueden ser negativos");
                        return -1;
                    }

                    context.Facturas.Add(factura);
                    context.SaveChanges();
                    return factura.IdFactura;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al agregar factura: {ex.Message}\n{ex.InnerException?.Message}");
                return -1;
            }
        }

        public Facturas BuscarFacturaPorId(int id)
        {
            try
            {
                using (var context = CrearContexto())
                {
                    return context.Facturas
                        .Include(f => f.Clientes)
                        .Include(f => f.Empleados)
                        .Include(f => f.DetalleFactura.Select(d => d.Productos))
                        .FirstOrDefault(f => f.IdFactura == id);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al buscar factura: {ex.Message}");
                return null;
            }
        }
        #endregion

        public bool ExisteCliente(string documento)
        {
            using (var context = CrearContexto())
            {
                return context.Clientes.Any(c => c.Documento == documento);
            }
        }

        public bool ClienteTieneFacturas(int idCliente)
        {
            using (var context = CrearContexto())
            {
                return context.Facturas.Any(f => f.IdCliente == idCliente);
            }
        }
        public bool ExisteCodigoEmpleado(string codigo)
        {
            using (var context = CrearContexto())
            {
                return context.Empleados.Any(e => e.Codigo == codigo);
            }
        }

        public bool ExisteDocumentoEmpleado(string documento)
        {
            using (var context = CrearContexto())
            {
                return context.Empleados.Any(e => e.Documento == documento);
            }
        }
        public bool ExisteProducto(string codigo)
        {
            using (var context = CrearContexto())
            {
                return context.Productos.Any(p => p.Codigo == codigo);
            }

        }


    }

}
