using SIAAPI.Models.Administracion;
using SIAAPI.Models.Comercial;
using SIAAPI.Models.Login;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SIAAPI.Models.Produccion
{

    //[Table("LiberaOrden")]
    //public class LiberaOrden
    //{
    //    [Key]
    //    public int IDLiberaOrden { get; set; }
    //    [DisplayName("Orden de Producción")]
    //    [Required]
    //    public int IDOrden { get; set; }
    //    [DisplayName("Fecha de Liberación")]
    //    public Nullable<DateTime> FechaLiberacion { get; set; }
    //    [DisplayName("Artículo")]
    //    public int IDArticulo { get; set; }
    //    public virtual Articulo Articulo { get; set; }

    //    [DisplayName("Característica")]
    //    public int IDCaracteristica { get; set; }
    //    public virtual Caracteristica Caracteristica { get; set; }

    //    [DisplayName("Cantidad a Liberar")]
    //    public decimal Cantidad { get; set; }
    //    [DisplayName("Unidad")]
    //    public int IDClaveUnidad { get; set; }
    //    public virtual c_ClaveUnidad c_ClaveUnidad {get;set;}
    //}
    [Table("LiberaOrdenProduccion")]
    public class LiberaOrden
    {
        [Key]
        public int IDLibera { get; set; }
        [DisplayName("Orden de Producción")]
        [Required]
        public int IDOrden { get; set; }
        [DisplayName("Fecha de Liberación")]
        public DateTime FechaLiberacion { get; set; }

        [DisplayName("Almacen")]
        public int IDAlmacen { get; set; }
        
        [DisplayName("Cantidad a Liberar")]
        public decimal Cantidad { get; set; }

        [DisplayName("Usuario")]
        public string Usuario { get; set; }
        public string Lote { get; set; }

        public string TipoLiberacion { get; set; }
    }
    public class LiberaOrdenContext : DbContext
    {
        public LiberaOrdenContext() : base("name=DefaultConnection")
        {
            Database.SetInitializer<LiberaOrdenContext>(null);
        }
        public DbSet<LiberaOrden> LiberaOrdenes { get; set; }

    }

}

