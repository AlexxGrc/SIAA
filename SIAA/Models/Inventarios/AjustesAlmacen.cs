using SIAAPI.Models.Comercial;
using SIAAPI.Models.Login;
using SIAAPI.Models.Produccion;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SIAAPI.Models.Inventarios
{
    [Table("AjustesAlmacen")]
    public class AjustesAlmacen
    {
        [Key]
        [DisplayName("Código del movimiento")]
        public int IDAjuste { get; set; }

        [Required(ErrorMessage = "Fecha del Ajuste")]
        [DisplayFormat(ApplyFormatInEditMode = true,
        DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime FechaAjuste { get; set; }

        [Required(ErrorMessage = "Es necesario especificar el Almacen")]
        [DisplayName("Almacen")]
        public int IDAlmacen { get; set; }
        public virtual Almacen Almacen { get; set; }
        [Required(ErrorMessage = "Es necesario indicar el artículo")]
        [DisplayName("ID Artículo")]
        public virtual Articulo Articulo { get; set; }
        public int IDArticulo { get; set; }
        [Required(ErrorMessage = "Es necesario indicar la caracteristica")]
        [DisplayName("Articulo Presentacion")]
        public int ID { get; set; }
        public virtual Caracteristica Caracteristica { get; set; }

        [DisplayName("Lote")]


        public string Lote { get; set; }
        [DisplayName("Cantidad")]
        [Required(ErrorMessage = "Es necesario indicar la cantidad")]
        public decimal Cantidad { get; set; }
        [DisplayName("Tipo de operación")]
        public Tipo TipoOperacion { get; set; }
        [DisplayName("Ajusto")]
        public int UserID { get; set; }
        public virtual User User { get; set; }
        [DisplayName("Observación")]
        public string Observacion { get; set; }
    }

    public enum Tipo
    {
        Entrada,
        Salida
    }
    public class Tipoajuste
    {
        public Tipo tipooperacion { get; set; }
    }

    public class AjustesAlmacenContext : DbContext
    {
        public AjustesAlmacenContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<AjustesAlmacen> AjustesAlmacenes { get; set; }
        public DbSet<InventarioAlmacen> invAlmacenes { get; set; }
        public DbSet<Articulo> Articulos { get; set; }
        public DbSet<Caracteristica> Caracteristicas { get; set; }
        public DbSet<Almacen> Almacenes { get; set; }
        public DbSet<User> Usuarios { get; set; }

    }


    [Table("VAjustesAlmacen")]
    public class VAjustesAlmacen
    {
        [Key]
        [DisplayName("Código del movimiento")]
        public int IDAjuste { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true,
        DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime FechaAjuste { get; set; }

        public int IDAlmacen { get; set; }
        [DisplayName("Almacen")]
        public string Almacen { get; set; }
        [DisplayName("ID Artículo")]
        public int IDArticulo { get; set; }
        public string Cref { get; set; }
        [DisplayName("Articulo Presentacion")]
        public string Articulo { get; set; }

        public int ID { get; set; }
        public string Presentacion { get; set; }
        public int Version { get; set; }

        [DisplayName("Lote")]
        public string Lote { get; set; }
        [DisplayName("Cantidad")]
        public decimal Cantidad { get; set; }
        [DisplayName("Tipo de operación")]
        public string TipoOperacion { get; set; }
        [DisplayName("Ajusto")]
        public int UserID { get; set; }
        public string UserName { get; set; }
        [DisplayName("Observación")]
        public string Observacion { get; set; }
    }
    public class VAjustesAlmacenContext : DbContext
    {
        public VAjustesAlmacenContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<VAjustesAlmacen> VAjustesAlmacenes { get; set; }

    }
    [Table("ValeSalida")]
    public class ValeSalida
    {
        [Key]

        public int IDValeSalida { get; set; }

        //[Required(ErrorMessage = "Fecha del Ajuste")]
        [DisplayFormat(ApplyFormatInEditMode = true,
        DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime Fecha { get; set; }

        [Required(ErrorMessage = "Es necesario especificar el Almacen")]
        [DisplayName("Almacen")]
        public int IDAlmacen { get; set; }
        public virtual Almacen Almacen { get; set; }

        [DisplayName("Ajusto")]
        public int UserID { get; set; }
        public virtual User User { get; set; }
        [DisplayName("Observación")]
        public string Observacion { get; set; }
        public string Entregar { get; set; }
        public string Concepto { get; set; }
        public string Solicito { get; set; }
        public string Estado { get; set; }

    }
    [Table("DetValeSalida")]
    public class DetValeSalida
    {
        [Key]
        public int IDDetValeSalida { get; set; }
        public int IDValeSalida { get; set; }

        public int IDArticulo { get; set; }

        public int IDCaracteristica { get; set; }

        public decimal Cantidad { get; set; }
        public decimal Precio { get; set; }
        public decimal Importe { get; set; }
        public int IDAlmacen { get; set; }
        public string Moneda { get; set; }
    }
    public class ValeSalidaContext : DbContext
    {
        public ValeSalidaContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<ValeSalida> ValeSalida { get; set; }
        public DbSet<DetValeSalida> DetValeSalida { get; set; }
    }
    public class VDetValeSalida
    {
        public int IDValeSalida { get; set; }
        public int IDDetValeSalida { get; set; }
        public int usuario { get; set; }
        public int IDCaracteristica { get; set; }
        public decimal Precio { get; set; }
        public decimal Cantidad { get; set; }

        public string Presentacion { get; set; }
        public int IDArticulo { get; set; }
        public string Moneda { get; set; }

        public string Cref { get; set; }
        //public string Descripcion { get; set; }

        public decimal Importe { get; set; }

        //public decimal ImporteIva { get; internal set; }
        //public string Moneda { get; set; }

        public string Unidad { get; set; }

        public int IDAlmacen { get; set; }
    }


  
}