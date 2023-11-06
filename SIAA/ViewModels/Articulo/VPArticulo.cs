using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SIAAPI.ViewModels.Articulo
{
    [Table("VPArticulo")]
    public class VPArticulo
    {
       
            [Key]
            public int IDArticulo { get; set; }

          
            public string Cref { get; set; }

           
            public string Descripcion { get; set; }

            public decimal Precio { get; set; }

            public string Moneda { get; set;  }

            public int IDFamilia { get; set; }

            public string Familia { get; set; }

            public string Tipo { get; set; }

        public string Unidad { get; set; }

        public string nameFoto { get; set; }

        public Boolean ManejoCar { get; set; }
        public bool GeneraOrden { get; set; }

        public int IDCotizacion { get; set; }

    }
    public class VPArticuloContext : DbContext
    {
        public VPArticuloContext() : base("name=DefaultConnection")
        {
            Database.SetInitializer<VPArticuloContext>(null);
        }
        public DbSet<VPArticulo> VPArticulos { get; set; }
       

        
    }
}