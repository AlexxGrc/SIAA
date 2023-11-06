using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SIAAPI.Models.Cfdi
{
    public class ClienteSaldoFacturas
    {

        [Key]
        public int ID { get; set; }
        //public System.DateTime Fecha { get; set; }
        //public string Serie { get; set; }
        //public int Numero { get; set; }
        public string NombreCliente { get; set; }
        public decimal TotalFacturado { get; set; }
        public decimal ImportePagado { get; set; }
        public decimal ImporteSaldoInsoluto { get; set; }
        public string Moneda { get; set; }
    }
    public class ClienteSaldoFacturasContext : DbContext
    {
        public ClienteSaldoFacturasContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<ClienteSaldoFacturas> ClienteSaldoFacturas { get; set; }
    }


}