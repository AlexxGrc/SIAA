using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SIAAPI.Models.Cfdi
{
    public class ProveedorSaldoFacturas
    {

        [Key]
        public int ID { get; set; }
        public string Proveedor { get; set; }
        public decimal TotalFacturado { get; set; }
        public decimal ImportePagado { get; set; }
        public decimal ImporteSaldoInsoluto { get; set; }
        public string Moneda { get; set; }
    }
    public class ProveedorSaldoFacturasContext : DbContext
    {
        public ProveedorSaldoFacturasContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<ProveedorSaldoFacturas> ProveedorSaldoFacturas { get; set; }
    }


}