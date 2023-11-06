using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SIAAPI.Models.Comercial
{
    [Table("CarritoP")]
    public class CarritoP
    {
        [Key]
        public int IDCarrito { get; set; }
        public int usuario { get; set; }
        public int IDCaracteristica { get; set; }
        public decimal Precio { get; set; }
        public decimal Cantidad { get; set; }
        public string jsonPresentacion { get; set; }
        public string Presentacion { get; set; }
        public int IDArticulo { get; set; }
        public int IDMoneda { get; set; }
        public string Descripcion { get; set; }

        public decimal Importe { get; set; }
        public string Nota { get; set; }
        public decimal ImporteIva { get; internal set; }
        public string Moneda { get; set; }
        public decimal MinimoCompra { get; set; }
        public decimal MinimoVenta { get; set; }
        public string Unidad { get; set; }
        public int IDProveedor { get; set; }
        [DisplayName("Precio MXN")]
        public decimal preciomex { get; set; }
    }
      
    public class CarritoPContext : DbContext
    {
        public CarritoPContext() : base("name=DefaultConnection")
        {
            Database.SetInitializer<CarritoContext>(null);
        }
        public DbSet<CarritoP> Carritos { get; set; }
        public DbSet<Caracteristica> Caracteristica { get; set; }
        public DbSet<Articulo> Articulo { get; set; }
       
    }
}