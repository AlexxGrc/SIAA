using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SIAAPI.ViewModels.Articulo
{
    [Table("Caracteristica")]
    public class VCaracteristica
    {
        [Key]
        public int ID { get; set; }

        public int IDPresentacion { get; set; }

        public int Articulo_IDArticulo { get; set; }

        public int Cotizacion { get; set; }
        public int version { get; set; }

        public string Presentacion { get; set; }

        public bool obsoleto { get; set; }

        public string jsonPresentacion { get; set; }
    }
    public class VCaracteristicaContext : DbContext
    {
        public VCaracteristicaContext() : base("name=DefaultConnection")
        {
            Database.SetInitializer<VCaracteristicaContext>(null);
        }

        public DbSet<VCaracteristica> VCaracteristica { get; set; }
    }
}