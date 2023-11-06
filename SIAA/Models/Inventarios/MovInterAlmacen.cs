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
    [Table("MovInterAlmacen")]
    public class MovInterAlmacen
    {
        [Key]
        [DisplayName("Código del movimiento")]
        public int IDMovimiento { get; set; }

        [Required(ErrorMessage = "Fecha del Movimiento")]
        [DisplayFormat(ApplyFormatInEditMode = true,
        DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime FechaMovimiento{ get; set; }

        [Required(ErrorMessage = "Es necesario especificar el Almacen de Salida")]
        [DisplayName("Almacen de Salida")]
        public int IDAlmacenS { get; set; }
        [Required(ErrorMessage = "Es necesario indicar el artículo")]
        [DisplayName("ID Artículo")]
        public int IDArticulo { get; set; }
        [Required(ErrorMessage = "Es necesario indicar la caracteristica")]
        [DisplayName("Articulo Presentacion")]
        public int IDCaracteristica{ get; set; }
      
        [DisplayName("Lote")]
        public string Lote { get; set; }

        [Required(ErrorMessage = "Es necesario indicar la cantidad")]
        [DisplayName("Cantidad")]
        public decimal Cantidad { get; set; }
            [Required(ErrorMessage = "Es necesario indicar el trabajador que entrega el material")]
        [DisplayName("ID del Trabajador que entrega el material")]
        public int IDTrabajadorS { get; set; }
        [Required(ErrorMessage = "Es necesario indicar el trabajador que recibe el material")]
        [DisplayName("ID del Trabajador que recibe el material")]
        public int IDTrabajadorE{ get; set; }
        [Required(ErrorMessage = "Es necesario especificar el Almacen de Entrada")]
        [DisplayName("Almacen de Entrada")]
        public int IDAlmacenE { get; set; }
        [DisplayName("Observación")]
        public string Observacion { get; set; }

    }
    [Table("MovInterPresentacion")]
    public class MovInterPresentacion
    {
        [Key]
        [DisplayName("Código del movimiento")]
        public int IDMovimiento { get; set; }

        [Required(ErrorMessage = "Fecha del Movimiento")]
        [DisplayFormat(ApplyFormatInEditMode = true,
        DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime FechaMovimiento { get; set; }

        [Required(ErrorMessage = "Es necesario especificar el Almacen de Salida")]
        [DisplayName("Almacen de Salida")]
        public int IDAlmacenS { get; set; }
        [Required(ErrorMessage = "Es necesario indicar el artículo")]
        [DisplayName("ID Artículo")]
        public int IDArticulo { get; set; }
        [Required(ErrorMessage = "Es necesario indicar la caracteristica")]
        [DisplayName("Articulo Presentacion 1")]
        public int IDCaracteristica { get; set; }

        [DisplayName("Lote")]
        public string Lote { get; set; }

        [Required(ErrorMessage = "Es necesario indicar la cantidad")]
        [DisplayName("Cantidad")]
        public decimal Cantidad { get; set; }
       
        [Required(ErrorMessage = "Es necesario especificar el Almacen de Entrada")]
        [DisplayName("Almacen de Entrada")]
        public int IDAlmacenE { get; set; }
        [Required(ErrorMessage = "Es necesario indicar la caracteristica")]
        [DisplayName("Articulo Presentacion 2")]
        public int IDCaracteristica2 { get; set; }
        [DisplayName("Observación")]
        public string Observacion { get; set; }

    }
    public class MovInterAlmacenContext : DbContext
    {
        public MovInterAlmacenContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<MovInterAlmacen> MovInterAlmacenes { get; set; }
    }

    [Table("VMovInterAlmacen")]
    public class VMovInterAlmacen
    {
        [Key]
        [DisplayName("Código del movimiento")]
        public int IDMovimiento { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true,
        DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}")]
        public Nullable<DateTime> FechaMovimiento { get; set; }
        [DisplayName("ID Almacen de Salida")]
        public int IDAlmacenS { get; set; }
        [DisplayName("Almacen de Salida")]
        public string AlmacenSalida { get; set; }
        [DisplayName("ID Artículo")]
        public int IDArticulo { get; set; }
        [DisplayName("ID Característica")]
        public int IDCaracteristica { get; set; }
        [DisplayName("Código Articulo")]
        public string Cref { get; set; }
        [DisplayName("Descripción Articulo")]
        public string Descripcion { get; set; }
        [DisplayName("Presentación")]
        public string Presentacion { get; set; }
        [DisplayName("Lote")]
        public string Lote { get; set; }
        [DisplayName("Cantidad")]
        public decimal Cantidad { get; set; }
        [DisplayName("ID del Trabajador Entrega")]
        public int IDTrabajadorS { get; set; }
        [DisplayName("Entrego")]
        public string Entrego { get; set; }
        [DisplayName("Código Almacen Entrada")]
        public int IDAlmacenE { get; set; }
        [DisplayName("Almacen de Entrada")]
        public string AlmacenEntrada { get; set; }
        [DisplayName("ID del Trabajador Recibe")]
        public int IDTrabajadorE { get; set; }
        [DisplayName("Recibio")]
        public string Recibio { get; set; }
        [DisplayName("Observación")]
        public string Observacion { get; set; }

    }
    [Table("VMovInterPresentaciones")]
    public class VMovInterPresentaciones
    {
        [Key]
        [DisplayName("Código del movimiento")]
        public int IDMovimiento { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true,
        DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}")]
        public Nullable<DateTime> FechaMovimiento { get; set; }
        [DisplayName("ID Almacen de Salida")]
        public int IDAlmacenS { get; set; }
        [DisplayName("Almacen de Salida")]
        public string AlmacenSalida { get; set; }
        [DisplayName("ID Artículo")]
        public int IDArticulo { get; set; }
       
        [DisplayName("Código Articulo")]
        public string Cref { get; set; }
        [DisplayName("Descripción Articulo")]
        public string Descripcion { get; set; }
        [DisplayName("Presentación")]
        public string Presentacion { get; set; }
        [DisplayName("Lote")]
        public string Lote { get; set; }
        [DisplayName("Cantidad")]
        public decimal Cantidad { get; set; }
       
       
        [DisplayName("Código Almacen Entrada")]
        public int IDAlmacenE { get; set; }
        [DisplayName("Almacen de Entrada")]
        public string AlmacenEntrada { get; set; }
        [DisplayName("Prsentacion 2")]
        public string Presentacion2 { get; set; }
        
        [DisplayName("Usuario")]
        public string Usuario { get; set; }
        [DisplayName("Observación")]
        public string Observacion { get; set; }

    }
    public class VMovInterAlmacenContext : DbContext
    {
        public VMovInterAlmacenContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<VMovInterAlmacen> VMovInterAlmacenes { get; set; }
        public DbSet<VMovInterPresentaciones> vMovInterPresentaciones { get; set; }
    }



    [Table("VProductoAlmacen")]
    public class VProductoAlmacen
    {
        [Key]
        [DisplayName("ID")]
        public int ID { get; set; }
        [DisplayName("ID Almacen")]
        public int IDAlmacen { get; set; }
        [DisplayName("Almacen")]
        public string Almacen { get; set; }
        [DisplayName("ID Artículo")]
        public int IDArticulo { get; set; }
        [DisplayName("ID Característica")]
        public int IDCaracteristica { get; set; }
        [DisplayName("Código")]
        public string Cref { get; set; }
        [DisplayName("Descripción")]
        public string Descripcion { get; set; }
        [DisplayName("Presentación")]
        public string Presentacion { get; set; }
        [DisplayName("Existencia")]
        public decimal Existencia { get; set; }
    }

    public class VProductoAlmacenContext : DbContext
    {
        public VProductoAlmacenContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<VProductoAlmacen> VProductoAlmacenes { get; set; }
    }
}