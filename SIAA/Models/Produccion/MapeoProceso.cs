using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SIAAPI.Models.Produccion
{
    public class MapeoProceso
    {
 
   }
    [Table("VMaquinaProceso")]
    public class VMaquinaProceso
    {
        [Key]
        public int IDMaquinaProceso { get; set; }
        public int IDProceso { get; set; }
        [DisplayName("Proceso")]
        public string NombreProceso { get; set; }
        [DisplayName("Usa Máquina")]
        public bool UsaMaquina { get; set; }
        [DisplayName("IDMáquina")]
        public int IDArticulo { get; set; }
        [DisplayName("Máquina")]
        public string Descripcion { get; set; }

        public int IDTipoArticulo { get; set; }


    }
    public class VMaquinaProcesoContext : DbContext
    {
        public VMaquinaProcesoContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<VMaquinaProceso> VMaquinaProceso { get; set; }

    }

    //public class VOrdenProcesoEstado
    //{
    //    [Key]
    //    public int IDOrdenDetalle { get; set; }
    //    [DisplayName("IDOrden")]
    //    public int IDOrden { get; set; }
    //    [DisplayName("IDProceso")]
    //    public int IDProceso { get; set; }
    //    [DisplayName("IDEstadoOrden")]
    //    public int IDEstadoOrden { get; set; }
    //    [DisplayName("Estado")]
    //    public string Estado { get; set; }

    //}
    //public class VOrdenProcesoEstadoContext : DbContext
    //{
    //    public VOrdenProcesoEstadoContext() : base("name=DefaultConnection")
    //    {

    //    }
    //    public DbSet<VOrdenProcesoEstado> VOrdenProcesoEstado { get; set; }

    //}

    //public class VOrdenProduccion
    //{
    //    [Key]
    //    public int IDOrden { get; set; }
    //    public int IDModeloProduccion { get; set; }
    //    [DisplayName("Modelo")]
    //    public string ModeloProduccion { get; set; }
    //    public int IDCliente { get; set; }
    //    [DisplayName("Cliente")]
    //    public string Nombre { get; set; }
    //    public int IDArticulo { get; set; }
    //    [DisplayName("IDCaracteristica")]
    //    public int IDCaracteristica { get; set; }
    //    [DisplayName("Articulo")]
    //    public string Descripcion { get; set; }
    //    [DisplayName("Presentacion")]
    //    public string Presentacion { get; set; }
    //    [DisplayName("Indicaciones")]
    //    public string Indicaciones { get; set; }
    //    [DisplayName("Fecha compromiso")]
    //    public DateTime FechaCompromiso { get; set; }
    //    [DisplayName("Fecha inicio")]
    //    public DateTime FechaInicio { get; set; }
    //    [DisplayName("Fecha programada")]
    //    public DateTime FechaProgramada{ get; set; }
    //    [DisplayName("Fecha real de inicio")]
    //    public DateTime FechaRealdeInicio { get; set; }
    //    [DisplayName("Fecha terminación")]
    //    public DateTime FechaRealdeTerminacion { get; set; }
    //    [DisplayName("Cantidad")]
    //    public decimal Cantidad { get; set; }
    //    [DisplayName("IDPedido")]
    //    public int IDPedido { get; set; }

    //    public int IDDetPedido { get; set; }
    //    [DisplayName("Prioridad")]
    //    public int Prioridad { get; set; }
    //    public int IDEstadoOrden { get; set; }
    //    [DisplayName("Estado")]
    //    public string Estado { get; set; }
    //    public int IDProceso { get; set; }
    //    [DisplayName("Proceso")]
    //    public string NombreProceso { get; set; }
    //}
    //public class VOrdenProduccionContext : DbContext
    //{
    //    public VOrdenProduccionContext() : base("name=DefaultConnection")
    //    {

    //    }
    //    public DbSet<VOrdenProduccion> VOrdenProduccion { get; set; }

    //}

}