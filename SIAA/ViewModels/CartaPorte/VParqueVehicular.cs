using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SIAAPI.ViewModels.CartaPorte
{
    public class VParqueVehicular
    {
        [Key]
        public int IDParqueV { set; get; }

        [DisplayName("Tipo Permiso")]
        public int IDPermisoSCT { get; set; }
        [DisplayName("Clave Permiso SCT")]
        public string ClavePermisoSCT { get; set; }
        [DisplayName("Tipo Permiso")]
        public string TipoPermiso { get; set; }
        [DisplayName("No. Permiso SCT")]
        public string NoPermisoSCT { set; get; }
        [DisplayName("Aseguradora")]
        public string Aseguradora { set; get; }
        [DisplayName("Póliza de seguro")]
        public string PolizaSeguro { get; set; }
        [DisplayName("Tipo Vehículo")]
        public int IDVehiculo { get; set; }
        [DisplayName("Clave Vehículo")]
        public string ClaveVehiculo { get; set; }
        [DisplayName("Tipo Vehículo")]
        public string TipoVehiculo { get; set; }
        [DisplayName("Placa Vehículo")]
        public string PlacaVehiculo { get; set; }
        [DisplayName("Año Vehiculo")]
        public int AnnoVehiculo { get; set; }
        [DisplayName("Modelo")]
        public string Modelo { get; set; }
        [DisplayName("Color")]
        public string Color { get; set; }
        [DisplayName("Tipo Remolque")]
        public int IDRemolque { get; set; }
        [DisplayName("Clave Remolque")]
        public string ClaveRemolque { get; set; }
        [DisplayName("Tipo Remolque")]
        public string TipoRemolque { get; set; }
        [DisplayName("Placa")]
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
    public class VParqueVehicularDBContext : DbContext
    {
        public VParqueVehicularDBContext() : base("name = DefaultConnection")
        {

        }

        public DbSet<VParqueVehicular> VParqueVehicular { get; set; }
    }
}