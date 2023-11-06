using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SIAAPI.Models.Cfdi
{
    [Table("EncFacturaProv")]
    public class TempFacturacion
    {

        [Key]
        public int Ticket { get; set; }
        public DateTime FechaFac { get; set; }
        public string Estado { get; set; }
        public string SerieDigital { get; set; }
        public string NumeroDigital { get; set; }
        public int IDPrefactura { get; set; }
        public DateTime FechaPrefactura { get; set; }
        public string SeriePre { get; set; }
        public int NumeroPre { get; set; }
        public string documento { get; set; }
        public int iddocumento { get; set; }
        public int iDRemision { get; set; }
        public DateTime FechaRem { get; set; }
        public int IDPedido { get; set; }
        public DateTime FechaPed { get; set; }
        public DateTime FechaRequiere { get; set; }
        public string Proviene { get; set; }
        
    }
    public class TempFacturacionContext : DbContext
    {
        public TempFacturacionContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<TempFacturacion> TempFacturacions { get; set; }
    }
}