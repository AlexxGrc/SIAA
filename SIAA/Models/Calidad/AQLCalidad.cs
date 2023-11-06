using SIAAPI.Models.Comercial;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Security.Permissions;
using System.Web;

namespace SIAAPI.Models.Calidad
{

    //Cambie del Model de Articulo AQLCalidad, Inspeccion y Muestreo aquí, para tener todo lo de calidad junto}
    //en Models/Comercial/ Articulo.cs agregar:  using SIAAPI.Models.Calidad;
    //en Models/Comercial/ Kit.cs agregar:  using SIAAPI.Models.Calidad;
    //en Controllers/Comercial/ArticuloController agregar:  using SIAAPI.Models.Calidad;

    [Table("AQLCalidad")]
    public class AQLCalidad
    {
        [Key]
        public int IDAQL { get; set; }
        [DisplayName("Descripción")]
        public string Descripcion { get; set; }

        public virtual ICollection<Articulo> Articulo { get; set; }
    }

    [Table("Inspeccion")]
    public class Inspeccion
    {
        [Key]
        public int IDInspeccion { get; set; }
        [DisplayName("Descripción")]
        public string Descripcion { get; set; }
        public virtual ICollection<Articulo> Articulo { get; set; }
    }

    [Table("Muestreo")]
    public class Muestreo
    {
        [Key]
        public int IDMuestreo { get; set; }
        [DisplayName("Descripción")]
        public string Descripcion { get; set; }
        public virtual ICollection<Articulo> Articulo { get; set; }
    }

    public class CalidadContext : DbContext
    {
        public CalidadContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<Muestreo> Muestreos { get; set; }
        public DbSet<Inspeccion> Inspecciones { get; set; }
        public DbSet<AQLCalidad> AQLCalidads { get; set; }
    }

    [Table("NormaCalidad")]
    public class NormaCalidad
    {
        [Key]
        public int IDNorma { get; set; }
        [DisplayName("Norma")]
        public string Norma { get; set; }
        [DisplayName("Folio")]
        public string Folio { get; set; }
        [DisplayName("Código")]
        public string Codigo { get; set; }
        [DisplayName("No. Revision")]
        public string NoRevision { get; set; }
        [DisplayName("Aceptación")]
        public string ColumnaAceptacion { get; set; }
        [DisplayName("Fecha")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required]
        public DateTime Fecha { get; set; }
    }
    public class NormaCalidadContext : DbContext
    {
        public NormaCalidadContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<NormaCalidad> NormasCalidad { get; set; }
    }


    [Table("CalidadLetra")]
    public class CalidadLetra
    {
        [Key]
        public int IDLetra { get; set; }
        [DisplayName("Limite Inferior")]
        public decimal LI_Lote_mill { get; set; }
        [DisplayName("Limite Superior")]
        public decimal LS_Lote_mill { get; set; }
        [DisplayName("Nivel General de Inspección I")]
        public string NGI1 { get; set; }
        [DisplayName("Nivel General de Inspección II")]
        public string NGI2 { get; set; }
        [DisplayName("Nivel General de Inspección III")]
        public string NGI3 { get; set; }
    }
    public class CalidadLetraContext : DbContext
    {
        public CalidadLetraContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<CalidadLetra> CalidadLetras { get; set; }
    }

    [Table("CalidadLimiteAceptacion")]
    public class CalidadLimiteAceptacion
    {
        [Key]
        public int IDAceptacion { get; set; }
        [DisplayName("Codigo de letra")]
        public string CodigoLetra { get; set; }
        [DisplayName("Tamaño de la muestra")]
        public decimal TamanoMuestra { get; set; }
        [DisplayName("Aceptación 1")]
        public int AC1 { get; set; }
        [DisplayName("Rechazo 1")]
        public int RE1 { get; set; }
        [DisplayName("Aceptación 1.5")]
        public int AC15 { get; set; }
        [DisplayName("Rechazo 1.5")]
        public int RE15 { get; set; }
        [DisplayName("Aceptación 2.5")]
        public int AC25 { get; set; }
        [DisplayName("Rechazo 2.5")]
        public int RE25 { get; set; }
        [DisplayName("Aceptación 4")]
        public int AC4 { get; set; }
        [DisplayName("Rechazo 4")]
        public int RE4 { get; set; }
        [DisplayName("Aceptación 6.5")]
        public int AC65 { get; set; }
        [DisplayName("Rechazo 6.5")]
        public int RE65 { get; set; }
        [DisplayName("Aceptación 10")]
        public int AC10 { get; set; }
        [DisplayName("Rechazo 10")]
        public int RE10 { get; set; }
    }
    public class CalidadLimiteAceptacionContext : DbContext
    {
        public CalidadLimiteAceptacionContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<CalidadLimiteAceptacion> CalidadLimitesAceptacion { get; set; }
    }



    [Table("CertificadoCalidad")]
    public class CertificadoCalidad
    {
        [Key]
        public int IDCertificado { get; set; }
        public DateTime FechaCertificado { get; set; }
        public int IDOrden { get; set; }
        public int IDPedido { get; set; }
        public int IDLibera { get; set; }
        public int IDCliente { get; set; }
        public int IDArticulo { get; set; }
        public int IDCaracteristica { get; set; }
        public string Descripcion { get; set; }
        public string Presentacion { get; set; }

        public string Lote { get; set; }
        public decimal Cantidad { get; set; }
        public int IDMuestreo { get; set; }
        public int IDInspeccion { get; set; }
        public string CodigoLetra { get; set; }
        public string MetodoResultadoPresentacion { get; set; }
        public string PresentacionResultado { get; set; }
        public string PresentacionFinalUnidad { get; set; }
        public string PresentacionTintas { get; set; }
        public string Clave { get; set; }
        public string Responsable { get; set; }

    }
    public class CertificadoCalidadContext : DbContext
    {
        public CertificadoCalidadContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<CertificadoCalidad> CertificadosCalidad { get; set; }
    }


    [Table("Metodo")]
    public class Metodo
    {
        [Key]
        public int IDMetodo { get; set; }
        public string Descripcion { get; set; }
    }

    public class MetodoContext : DbContext
    {
        public MetodoContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<Metodo> Metodos { get; set; }
    }

}
