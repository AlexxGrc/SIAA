using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SIAAPI.Models.Produccion
{
    public class MtsL
    {
        [Key]
        public int IDMaterialAsignado { get; set; }

        //public string Nombre { get; set; }
        //public int IDMaquina { get; set; }
        public int IDCliente { get; set; }
        //public  string Maquina { get; set; }
        //public int IDProceso { get; set; }
        public int Orden { get; set; }
        public string EstadoOrden { get; set; }
        public string Liberar { get; set; }
        public string Cliente { get; set; }
        public DateTime FechaCreacion { get; set; }
        public int IDCaracteristica  { get; set; }
        public  string Cref { get; set; }
        public string Descripcion { get; set; }
        //public decimal ancho { get; set; }
        //public decimal largo { get; set; }
        //public decimal M2Asignados { get; set; }
        //public decimal M2Entregados { get; set; }
        //public decimal MLAsignados { get; set; }
        //public decimal MLEntregado { get; set; }
        public int CaracteristicaArticulo { get; set; }
    }
    public class MtsLContext : DbContext
    {
        public MtsLContext() : base("name=DefaultConnection")
        {
            Database.SetInitializer<MtsLContext>(null);
        }
        public DbSet<MtsL> MtsLs { get; set; }
       
    }
}