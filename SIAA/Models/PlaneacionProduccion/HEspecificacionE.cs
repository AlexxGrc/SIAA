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
    [Table("HEspecificacionE")]
    public class HEspecificacionE
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

        public string ESPECIFICACION { get; set; }

    }
    public class HEspecificacionEContext : DbContext
    {

        public HEspecificacionEContext() : base("name=DefaultConnection")
        {

        }
        //Clases internas
        public DbSet<HEspecificacionE> HEspecificacionesE { get; set; }
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

    [Table("VHEspCteE")]
    public class VHEspCteE
    {
        [Key]
        public int IDHE { get; set; }
        public int Planeacion { get; set; }

        [DisplayName("Versión")]
        public int Version { get; set; }

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
    


    }
    public class VHEspCteEContext : DbContext
    {

        public VHEspCteEContext() : base("name=DefaultConnection")
        {

        }
        //Clases internas
        public DbSet<VHEspCteE> VHEspCtesE { get; set; }
    }

    [Table("DocumentoE")]
    public class VDocumentoE
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

    [Table("DocumentoE")]
    public class DocumentoE
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
    public class DocumentoEContext : DbContext
    {
        public DocumentoEContext() : base("name=DefaultConnection")
        {

        }
        //Clases internas
        public DbSet<DocumentoE> DocumentosE { get; set; }
    }


    [Table("ArticulosPlaneacionE")]
    public class ArticulosPlaneacionE
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

    public class ArticulosPlaneacionEContext : DbContext
    {
        public ArticulosPlaneacionEContext() : base("name=DefaultConnection")
        {

        }
        //Clases internas
        public DbSet<ArticulosPlaneacionE> ArticulosPlaneacionesE { get; set; }
    }


    [Table("ArticulosPlaneacionE")]
    public class VArticulosPlaneacionE
    {

        [Key]
        public int IDArtPlan { get; set; }
        [Required]
        public int IDHE { get; set; }
        [Required]
        //public int Version { get; set; }
        //[Required]
        //public int Planeacion { get; set; }
        //[Required]
        public string cref { get; set; }
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


    [Table("DocumentoPlaneacionE")]
    public class DocumentoPlaneacionE
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
    public class DocumentoPlaneacionEContext : DbContext
    {
        public DocumentoPlaneacionEContext() : base("name=DefaultConnection")
        {

        }
        //Clases internas
        public DbSet<DocumentoPlaneacionE> DocumentoPlaneacionesE { get; set; }
    }

}



