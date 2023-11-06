using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using SIAAPI.Models.Comercial;
using SIAAPI.Models.Inventarios;
using SIAAPI.Models.PlaneacionProduccion;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using SIAAPI.Models.Login;

namespace SIAAPI.Models.Suajes
{
    

    //Modelo
    public class VBitacoraSuajes
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int IdOrden { get; set; }

        public DateTime FechaCreacion { get; set; }

        public int IDModeloProduccion { get; set; }

        public decimal Cantidad { get; set; }

        public int IDArticulo { get; set; }

        public string Nombre { get; set; }

        public string EstadoOrden { get; set; }

        public string cref { get; set; }

        public int IDCaracteristica { get; set; }

        //public decimal MetroslIneales { get; set; }

        //public string Material { get; set; }

        ////public int Ancho { get; set; }

        ////public bool Cyrel { get; set; }

        //public int TipoEtiqueta { get; set; }
    }

    //Instancia de la BD

    public enum TipoOperacionSuaje
    {
        Entrada,
        Salida,
        Reposición
    }
    public class TipoOpeSuaje
    {
        public TipoOperacionSuaje tipooperacion { get; set; }
    }
    public class VBitacoraSuajesContext : DbContext
    {
        public VBitacoraSuajesContext() : base("name=DefaultConnection")
        {
            Database.SetInitializer<VBitacoraSuajesContext>(null);
        }
        public DbSet<VBitacoraSuajes> VBitacoraSuaje { get; set; }
    }
    [Table("ValeSuaje")]
    public class ValeSuaje
    {
        [Key]

        public int IDValeSuaje { get; set; }

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
        public string TipoOperacion { get; set; }
        public TipoOperacionSuaje TipoOVale { get; set; }
    }

    [Table("DetValeSuaje")]
    public class DetValeSuaje
    {
        [Key]
        public int IDDetValeSuaje { get; set; }
        public int IDValeSuaje { get; set; }

        public int IDArticulo { get; set; }

        public int IDCaracteristica { get; set; }

        public decimal Cantidad { get; set; }
        public decimal Precio { get; set; }
        public decimal Importe { get; set; }
        public int IDAlmacen { get; set; }
        public string Moneda { get; set; }
        public string Motivo { get; set; }
    }
    public class ValeSuajeContext : DbContext
    {
        public ValeSuajeContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<ValeSuaje> valeSuajes { get; set; }
        public DbSet<DetValeSuaje> detVales { get; set; }

    }

    public class VValeSuaje
    {
        [Key]

        public int IDValeSuaje { get; set; }

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
        public string TipoOperacion { get; set; }
       
    }
    public class VDetValeSuaje
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