using SIAAPI.Models.Administracion;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SIAAPI.Models.CartaPorte
{
    [Table("perfilcartaporte")]
    public class PerfilCartaPorte
    {
        [Key]
        public int idperfil { get; set; }
        
        public int idorigen { get; set; }
        public int iddestino { get; set; }
        public decimal distanciarecorrida { get; set; }
        public int idtransporte { get; set; }
        public int idoperador { get; set; }
        public int idemisor { get; set; }
        public int idreceptor { get; set; }
    }
    public class PerfilCartaPorteContext : DbContext
    {
        public PerfilCartaPorteContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<PerfilCartaPorte> PerfilCartaPorte { get; set; }
    }

    [Table("FacturaComplementocartasP")]
    public class FacturaComplementoCartasP
    {
        [Key]
        public int ID { get; set; }

        public int IDFacturaViene { get; set; }
        public DateTime Fecha { get; set; }
       
        
        public string UUID { get; set; }
        public string XMLRuta { get; set; }

        public int Mes { get; set; }
        public int Anno { get; set; }
        public int IDOperador { get; set; }

    }

    public class FacturaCartaPorteContext : DbContext
    {
        public FacturaCartaPorteContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<FacturaComplementoCartasP> FacturaCartaPorte { get; set; }
    }

    [Table("FolioCartaP")]

    public class FolioCartaP
    {

        [Key]
        public int IDFolioCartaP { get; set; }

        [DisplayName("Folio")]
        [Required(ErrorMessage = "El folio es obligatorio")]

        public int Numero { get; set; }

        [DisplayName("Serie")]
        [Required(ErrorMessage = "La serie es obligatoria")]
        [StringLength(2)]
        public string Serie { get; set; }

        public int IDTipoComprobante { get; set; }
        public virtual c_TipoComprobante c_TipoComprobante { get; set; }



    }

    public class FolioCartaPContext : DbContext
    {
        public FolioCartaPContext() : base("name=DefaultConnection")
        {
            Database.SetInitializer<FolioCartaPContext>(null);
        }
        public DbSet<FolioCartaP> FoliosC { get; set; }
        public DbSet<c_TipoComprobante> c_TipoComprobantes { get; set; }



    }
}