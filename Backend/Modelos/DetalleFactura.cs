//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Backend.Modelos
{
    using System;
    using System.Collections.Generic;
    
    public partial class DetalleFactura
    {
        public int IdDetalle { get; set; }
        public int IdFactura { get; set; }
        public int IdProducto { get; set; }
        public string NombreProducto { get; set; }
        public decimal Precio { get; set; }
        public int CantidadLlevada { get; set; }
        public decimal SubTotal { get; set; }
    
        public virtual Facturas Facturas { get; set; }
        public virtual Productos Productos { get; set; }
    }
}
