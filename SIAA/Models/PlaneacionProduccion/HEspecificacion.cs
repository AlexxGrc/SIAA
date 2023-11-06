using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SIAAPI.Models.PlaneacionProduccion
{
    [Table("HEspecificacion")]
    public class HEspecificacion
    {
        [Key]
        public int IDHE { get; set; }
        public int Planeacion { get; set; }
        [DisplayName("Fecha")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required]
        public DateTime FechaEspecificacion { get; set; }
        [DisplayName("ID Cliente")]
        public int IDCliente { get; set; }
        [DisplayName("ID Prospecto Cliente")]
        public int IDClienteP { get; set; }
        [DisplayName("ID Familia")]
        [Required]
        public int IDFamilia { get; set; }

        [DisplayName("ID Articulo")]
        public int IDArticulo { get; set; }
        [DisplayName("ID Característica")]
        public int IDCaracteristica { get; set; }
        [DisplayName("Presentación")]
        public string Presentacion { get; set; }
        [DisplayName("Descripción")]
        [Required]
        public string Descripcion { get; set; }
        [DisplayName("Modelo de Producción")]
        public int IDModeloProduccion { get; set; }
        [DisplayName("Version")]
        public int Version { get; set; }

    }
    public class HEspecificacionContext : DbContext
    {

        public HEspecificacionContext() : base("name=DefaultConnection")
        {

        }
        //Clases internas
        public DbSet<HEspecificacion> HEspecificaciones { get; set; }
    }


    //[Table("HE_Modelo")]
    //public class HE_Modelo
    //{
    //    [DisplayName("ID Modelo Producción")]
    //    public int IDEHM { get; set; }
    //    public int IDHE { get; set; }
    //    public int IDModeloProduccion { get; set; }

    //}
    //public class HE_ModeloContext : DbContext
    //{

    //    public HE_ModeloContext() : base("name=DefaultConnection")
    //    {

    //    }
    //    //Clases internas
    //    public DbSet<HE_Modelo> HE_Modelos { get; set; }
    //}

    [Table("VHEspCte")]
    public class VHEspCte
    {
        [Key]
        public int IDHE { get; set; }
        public int Planeacion { get; set; }
        [DisplayName("Fecha")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required]
        public DateTime FechaEspecificacion { get; set; }
        [DisplayName("Cliente")]
        public string Nombre { get; set; }
        [DisplayName("Tipo de Cliente")]
        public string TipoCliente { get; set; }
        [DisplayName("Familia")]
        public string Familia { get; set; }
        [DisplayName("Descripción")]
        public string Descripcion { get; set; }
        [DisplayName("Presentación")]
        public string Presentacion { get; set; }
        [DisplayName("Artículo")]
        public string Articulo { get; set; }
        [DisplayName("Modelo de Producción")]
        public string ModeloProduccion { get; set; }
        [DisplayName("Versión")]
        public int Version { get; set; }

    }
    public class VHEspCteContext : DbContext
    {

        public VHEspCteContext() : base("name=DefaultConnection")
        {

        }
        //Clases internas
        public DbSet<VHEspCte> VHEspCtes { get; set; }
    }



    [Table("Documento")]
    public class VDocumento
    {

        [Key]
        public int IDDocumento { get; set; }
        public int IDHE { get; set; }
        public int IDProceso { get; set; }
        public string Proceso { get; set; }
        public string Descripcion { get; set; }
        public int Planeacion { get; set; }
        public int Version { get; set; }
        public string Nombre { get; set; }
    }

    [Table("Documento")]
    public class Documento
    {

        [Key]
        public int IDDocumento { get; set; }
        public int IDHE { get; set; }
        public int IDProceso { get; set; }
        public string Descripcion { get; set; }
        public int Planeacion { get; set; }
        public int Version { get; set; }
        public string Nombre { get; set; }
    }
    public class DocumentoContext : DbContext
    {
        public DocumentoContext() : base("name=DefaultConnection")
        {

        }
        //Clases internas
        public DbSet<Documento> Documentos { get; set; }
    }

    [Table("ArticulosPlaneacion")]
    public class ArticulosPlaneacion
    {

        [Key]
        public int IDArtPlan { get; set; }
        [Required]
        public int IDHE { get; set; }
        [Required]
        public int Version { get; set; }
        [Required]
        public int Planeacion { get; set; }
        [Required]
        public int IDArticulo { get; set; }
        [Required]
        public int IDTipoArticulo { get; set; }
       [Required]
        public int IDCaracteristica { get; set; }
        [Required]
        public int IDProceso { get; set; }
        public string Formuladerelacion { get; set; }
        public decimal factorcierre { get; set; }
        public string Indicaciones { get; set; }
    }

    public class ArticulosPlaneacionContext : DbContext
    {
        public ArticulosPlaneacionContext() : base("name=DefaultConnection")
        {

        }
        //Clases internas
        public DbSet<ArticulosPlaneacion> ArticulosPlaneaciones { get; set; }
    }


    [Table("ArticulosPlaneacion")]
    public class VArticulosPlaneacion
    {

        [Key]
        public int IDArtPlan { get; set; }
        [Required]
        public int IDHE { get; set; }
        [Required]
        public int Version { get; set; }
        [Required]
        public int Planeacion { get; set; }
        [Required]
        public string Articulo { get; set; }
        [Required]
        public string TipoArticulo { get; set; }
        [Required]
        public string Presentacion { get; set; }
        [Required]
        public string Proceso { get; set; }
        public string Formuladerelacion { get; set; }
        public decimal factorcierre { get; set; }
        public string Indicaciones { get; set; }
    }


    [Table("DocumentoPlaneacion")]
    public class DocumentoPlaneacion
    {

        [Key]
        public int IDDocumentoPlaneacion { get; set; }
        [Required]
        public int IDHE { get; set; }
        public int Version { get; set; }
        [Required]
        public int IDProceso { get; set; }
        public string Descripcion { get; set; }
        public string Ruta { get; set; }
      
    }
    public class DocumentoPlaneacionContext : DbContext
    {
        public DocumentoPlaneacionContext() : base("name=DefaultConnection")
        {

        }
        //Clases internas
        public DbSet<DocumentoPlaneacion> DocumentoPlaneaciones { get; set; }
    }

}



