using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SIAAPI.Models.Inventarios
{
    public class Clslotempcreate
    {
        [Key]
        public int ID { get; set; }

        public int IDArticulo { get; set; }

        public int IDCaracteristica { get; set; }

        public string Cref {get; set;}

        [Display(Name = "Numero de elementos")]
        public int NoCintas { get; set; }

        public int IDDetOrdenCompra { get; set; }

        public int IDRecepcion { get; set; }

        [Display(Name = "Ancho de material")]
        public int Ancho { get; set; }

        [Display(Name = "Largo Material")]
        public int Largo { get; set; }

        [Display(Name = "Lote")]
        public string Lote { get; set; }

        [Display(Name = "Lote interno")]
        public string LoteInterno { get; set; }

        [Display(Name = "M2")]
        public decimal MetrosCuadrados { get; set; }

        [Display(Name = "Orden de Compra")]
        public int OrdenCompra { get; set; }

        public int IDCarrito { get; set; }

        public int Facturaprov { get; set; }

       
    }

    [Table("Clslotemp")]
    public class Clslotemp
    {
        [Key]
        public int ID { get; set; }

        [Display(Name = "Numero de Cinta")]
        public int NoCinta { get; set; }

        public int IDDetOrdenCompra { get; set; }

        public int IDArticulo { get; set; }

        public int IDCaracteristica { get; set; }

        [Display(Name = "Ancho de material")]
        public int Ancho { get; set; }

        [Display(Name = "Largo Material")]
        public int Largo { get; set; }

        [Display(Name = "Lote")]
        public string Lote { get; set; }

        public string LoteInterno { get; set; }

        [Display(Name = "Fecha")]
        public DateTime Fecha { get; set; }

        [Display(Name = "Orden de Compra")]
        public int OrdenCompra { get; set; }

        [Display(Name = "M2")]
        public decimal MetrosCuadrados { get; set; }

        [Display(Name = "M2 utilizados")]
        public decimal Metrosutilizados { get; set; }

        [Display(Name = "Disponibles")]
        public decimal MetrosDisponibles { get; set; }

        public int IDProveedor { get; set; }

        public int IDAlmacen { get; set; }

        public int Facturaprov { get; set; }

        public int IDRecepcion { get; set; }

    }


    public class ClslotempContext : DbContext
    {
        public ClslotempContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<Clslotemp> materiaP { get; set; }

    }

    [Table("VClslotemp")]
    public class VClslotemp
    {
        [Key]
        public int ID { get; set; }
        public int IDArticulo { get; set; }
        public string Cref { get; set; }
        public string Articulo { get; set; }
        public int IDCaracteristica { get; set; }
        public string Presentacion { get; set; }
        public int IDTipoArticulo { get; set; }
        public string Descripcion { get; set; }
        public int IDFamilia { get; set; }

        [Display(Name = "Ancho de material")]
        public int Ancho { get; set; }

        [Display(Name = "Largo Material")]
        public int Largo { get; set; }

        [Display(Name = "Lote")]
        public string Lote { get; set; }

        public string LoteInterno { get; set; }

        [Display(Name = "M2")]
        public decimal MetrosCuadrados { get; set; }

        [Display(Name = "M2 utilizados")]
        public decimal Metrosutilizados { get; set; }

        [Display(Name = "Disponibles")]
        public decimal MetrosDisponibles { get; set; }

        public int IDAlmacen { get; set; }
        public string Almacen { get; set; }

    }


    public class VClslotempContext : DbContext
    {
        public VClslotempContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<VClslotemp> VmateriaP { get; set; }

    }
}