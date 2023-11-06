using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SIAAPI.ViewModels.produccion
{
    [Table("VLiberaciones")]
    public class VLiberaciones
    {
        [Key]
        public int IDLibera { get; set; }
        [DisplayName("No. Pedido")]
        public int IDPedido { get; set; }
        [DisplayName("No. de Orden de Producción")]
        public int IDOrden { get; set; }
        [DisplayName("Fecha de Liberación")]
        public Nullable<DateTime> FechaLiberacion { get; set; }
        [DisplayName("Cliente")]
        public string Nombre { get; set; }
        [DisplayName("Clave")]
        public string Cref { get; set; }
        [DisplayName("Artículo")]
        public string Descripcion { get; set; }
        [DisplayName("Presentación")]
        public string Presentacion { get; set; }

        [DisplayName("Lote")]
        public string Lote { get; set; }
        [DisplayName("Cantidad liberada")]
        public decimal Cantidad { get; set; }
        [DisplayName("Tipo Liberación")]
        public string TipoLiberacion { get; set; }
        [DisplayName("Almacén")]
        public string Almacen { get; set; }

        [DisplayName("Usuario")]
        public string Usuario { get; set; }
    }
    public class VLiberacionesContext : DbContext
    {
        public VLiberacionesContext() : base("name=DefaultConnection")
        {
            Database.SetInitializer<VLiberacionesContext>(null);
        }
        public DbSet<VLiberaciones> VLiberaciones { get; set; }

    }
}