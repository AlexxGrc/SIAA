using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.ComponentModel;
using SIAAPI.Models.CartaPorte;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SIAAPI.Models.CartaPorte
{

    [Table("ParqueVehicular")]
    public class ParqueVehicular
    {
        [Key]
        public int IDParqueV { set; get; }

        [DisplayName("Tipo Permiso")]
        public int IDPermisoSCT { get; set; }
        [DisplayName("Clave Permiso SCT")]
        [StringLength(6)]
        public string ClavePermisoSCT { get; set; }
        [DisplayName("No. Permiso SCT")]
        [StringLength(50)]
        public string NoPermisoSCT { set; get; }
        [DisplayName("Aseguradora")]
        [StringLength(100)]
        public string Aseguradora { set; get; }
        [DisplayName("Póliza de seguro")]
        [StringLength(50)]
        public string PolizaSeguro { get; set; }
        [DisplayName("Tipo Vehículo")]
        public int IDVehiculo { get; set; }
        [DisplayName("Clave Vehículo")]
        [StringLength(6)]
        public string ClaveVehiculo { get; set; }
        [DisplayName("Placa Vehículo")]
        [StringLength(50)]
        public string PlacaVehiculo { get; set; }
        [DisplayName("Año Vehiculo")]
        public int AnnoVehiculo { get; set; }
        [DisplayName("Modelo")]
        [StringLength(50)]
        public string Modelo { get; set; }
        [DisplayName("Color")]
        [StringLength(50)]
        public string Color { get; set; }
        [DisplayName("Tipo Remolque")]
        public int IDRemolque { get; set; }
        [DisplayName("Clave Remolque")]
        [StringLength(6)]
        public string ClaveRemolque { get; set; }
        [DisplayName("Placa")]
        [StringLength(50)]
        public string Placa { get; set; }
        [DisplayName("Es Arrendado")]
        public Boolean EsArrendado { get; set; }
        [DisplayName("Arrendatario")]
        public int IDArrendatario { get; set; }
        [DisplayName("Es de un Propietario")]
        public Boolean TienePropietario { get; set; }
        [DisplayName("Propietario")]
        public int IDPropietario { get; set; }

    }
    public class ParqueVehicularDBContext : DbContext
    {
        public ParqueVehicularDBContext() : base("name = DefaultConnection")
        {
            Database.SetInitializer<ParqueVehicularDBContext>(null);
        }

        public DbSet<ParqueVehicular> ParqueVe { get; set; }
        public DbSet<c_TipoPermiso> TPermiso { get; set; }
        public DbSet<c_ConfigAutotransporte> CTransporte { get; set; }
        public DbSet<c_SubTipoRem> STRemolque { get; set; }
    }

}
