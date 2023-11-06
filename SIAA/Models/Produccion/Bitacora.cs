using SIAAPI.Models.Administracion;
using SIAAPI.Models.Comercial;
using SIAAPI.Models.Login;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SIAAPI.Models.Produccion
{

    [Table("Bitacora")]
    public class Bitacora
    {
        [Key]
        public int IDBitacora { get; set; }
        [DisplayName("Orden de Producción")]
        [Required]
        public int IDOrden { get; set; }
        public virtual OrdenProduccion OrdenProduccion { get; set; }
        [DisplayName("Estatus")]
        public string EstadoBitacora { get; set; }

        //[DisplayName("Estatus")]
        //public int IDEstadoOrden{ get; set; }
        //public virtual EstadoOrden EstadoOrden { get; set; }
        [DisplayName("Responsable")]
        [Required]
        public int IDTrabajador { get; set; }
        public virtual Trabajador Trabajador { get; set; }
        [DisplayName("Fecha Inicio")]
        [Required]
        public DateTime FechaInicio { get; set; }
        [DisplayName("Fecha Fin")]
        public Nullable<DateTime> FechaFin { get; set; }
        [DisplayName("Diferencia de Horas")]
        public string DiferenciaHoras { get; set; }
        [DisplayName("Proceso")]
        [Required]
        public int IDProceso { get; set; }
        public virtual Proceso Proceso { get; set; }
        //[DisplayName("Máquina")]
        //public int IDArticulo { get; set; }
        //public virtual Articulo Articulo { get; set; }
        [DisplayName("Observación")]
        public string Observacion { get; set; }
        [DisplayName("Imagen")]
        public string Imagen { get; set; }
        [DisplayName("Cantidad")]
        public decimal Cantidad { get; set; }
        [DisplayName("Unidad")]
        public int IDClaveUnidad { get; set; }
        public virtual c_ClaveUnidad c_ClaveUnidad { get; set; }


        [DisplayName("Máquina")]
        [Required]
        public int IDMaquina { get; set; }

    }
    public class BitacoraContext : DbContext
    {
        public BitacoraContext() : base("name=DefaultConnection")
        {
            Database.SetInitializer<BitacoraContext>(null);
        }
        public DbSet<Bitacora> Bitacoras { get; set; }
        public DbSet<Trabajador> Trabajador { get; set; }
        public DbSet<OrdenProduccion> OrdendeProduccion { get; set; }
        public DbSet<EstadoOrden> EstadoOrden { get; set; }
        public DbSet<Proceso> Proceso { get; set; }
        public DbSet<Articulo> Articulo { get; set; }

    }
    public class ConBitacora
    {
        public int IDOrden { get; set; }
        public int IDTrabajador { get; set; }
        
        public int IDProceso { get; set; }
  
        public int IDMaquina { get; set; }

    }




    [Table("Bitacora")]
    public class VBitacora
    {
        [Key]
        public int IDBitacora { get; set; }

        [DisplayName("Orden de Producción")]
        public int IDOrden { get; set; }
        public virtual OrdenProduccion OrdenProduccion { get; set; }
        [DisplayName("Estatus")]
        public string EstadoBitacora { get; set; }


        [DisplayName("Responsable")]
        public int IDTrabajador { get; set; }
        public virtual Trabajador Trabajador { get; set; }

        [DisplayName("Fecha Inicio")]
        public DateTime FechaInicio { get; set; }

        [DisplayName("Fecha Fin")]
        public Nullable<DateTime> FechaFin { get; set; }
        [DisplayName("Diferencia de Horas")]
        public string DiferenciaHoras { get; set; }
        [DisplayName("Proceso")]
        public int IDProceso { get; set; }
        public virtual Proceso Proceso { get; set; }

        [DisplayName("Observación")]
        public string Observacion { get; set; }
        [DisplayName("Imagen")]
        public string Imagen { get; set; }
        [DisplayName("Cantidad")]
        public Nullable<decimal> Cantidad { get; set; }
        [DisplayName("Unidad")]
        public Nullable<int> IDClaveUnidad { get; set; }
        public virtual c_ClaveUnidad c_ClaveUnidad { get; set; }

    }
    public class VBitacoraContext : DbContext
    {

        public VBitacoraContext() : base("name=DefaultConnection")
        {
            Database.SetInitializer<VBitacoraContext>(null);
        }
        public DbSet<VBitacora> VBitacoras { get; set; }
        public DbSet<Trabajador> Trabajadores { get; set; }
        // public DbSet<OrdenP> OrdenP { get; set; }

    }

    [Table("ReporteDesperfecto")]
    public class ReporteDesperfecto
    {
        [Key]
        public int IDReporte { get; set; }
        [DisplayName("No. Bitácora")]
        public int IDBitacora { get; set; }
        [DisplayName("Máquina")]
        public int IDArticulo { get; set; }
        public virtual Articulo Articulo { get; set; }
        [DisplayName("Fecha Inicio Paro")]
        public DateTime FechaInicioParo { get; set; }
        [DisplayName("Fecha Terminación Paro")]
        public Nullable<DateTime> FechaTerminacionParo { get; set; }
        [DisplayName("Duración de Falla")]
        public string TiempoFalla { get; set; }
        [DisplayName("Falla")]
        public string Falla { get; set; }
    }
    public class ReporteDesperfectoContext : DbContext
    {
        public ReporteDesperfectoContext() : base("name=DefaultConnection")
        {
            Database.SetInitializer<ReporteDesperfectoContext>(null);
        }
        public DbSet<ReporteDesperfecto> ReporteDesperfectos { get; set; }

    }

    [Table("VOrdenProduccionPedido")]
    public class VOrdenProduccionPedido
    {
        [Key]
        [DisplayName("IDPedido")]
        public int IDPedido { get; set; }
        [DisplayName("Fecha Pedido")]
        public DateTime FechaPedido { get; set; }

        [DisplayName("Cliente")]
        public string Cliente { get; set; }
        [DisplayName("Clave")]
        public string Clave { get; set; }
        [DisplayName("Descripción")]
        public string Descripcion { get; set; }
        [DisplayName("Presentación")]
        public string Presentacion { get; set; }

        [DisplayName("IDOrden")]
        public int IDOrden { get; set; }

        [DisplayName("Fecha Compromiso")]
        public Nullable<DateTime> FechaCompromiso { get; set; }
        [DisplayName("Fecha Inicio")]
        public Nullable<DateTime> FechaInicio { get; set; }
        [DisplayName("Fecha Programada")]
        public Nullable<DateTime> FechaProgramada { get; set; }
        [DisplayName("Fecha de Creación")]
        public Nullable<DateTime> FechaCreacion { get; set; }

        [DisplayName("Estado de la orden")]
        public string EstadoOrden { get; set; }

        [DisplayName("Cotizacion")]
        public int Cotizacion { get; set; }

        [DisplayName("Cantidad")]
        public decimal Cantidad { get; set; }
    }
    public class VOrdenProduccionPedidoContext : DbContext
    {

        public VOrdenProduccionPedidoContext() : base("name=DefaultConnection")
        {
        }
        public DbSet<VOrdenProduccionPedido> VOrdenProduccionPedido { get; set; }


    }

    [Table("VBitacoraReporte")]
    public class VBitacoraReporte
    {
        [Key]
        public int IDBitacora { get; set; }
        [DisplayName("Estado Bitacora")]
        public string EstadoBitacora { get; set; }
        [DisplayName("Pedido")]
        public int IDPedido { get; set; }
        [DisplayName("Fecha Pedido")]
        public DateTime FechaPedido { get; set; }
        [DisplayName("Fecha Compromiso")]
        public DateTime FechaRequiere { get; set; }
        [DisplayName("Orden de Producción")]
        public int IDOrden { get; set; }
        [DisplayName("Estado Orden")]
        public string EstadoOrden { get; set; }
        [DisplayName("IDResponsable")]
        public int IDTrabajador { get; set; }
        [DisplayName("Responsable")]
        public string Trabajador { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true,
        DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}")]
        [DisplayName("Fecha Inicio")]
        public DateTime FechaInicio { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true,
         DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}")]
        [DisplayName("Fecha Fin")]
        public Nullable<DateTime> FechaFin { get; set; }
        [DisplayName("Diferencia de Horas")]
        public string DiferenciaHorasInicioFin { get; set; }
        [DisplayName("IDProceso")]
        public int IDProceso { get; set; }
        [DisplayName("Proceso")]
        public string NombreProceso { get; set; }

        [DisplayName("Observación")]
        public string Observacion { get; set; }

        [DisplayName("Cantidad")]
        public decimal Cantidad { get; set; }
        [DisplayName("Unidad")]
        public Nullable<int> IDClaveUnidad { get; set; }
        [DisplayName("Cantidad")]
        public string Unidad { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true,
        DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}")]
        [DisplayName("Fecha Incicio Paro")]
        public Nullable<DateTime> FechaInicioParo { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true,
        DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}")]
        [DisplayName("Fecha Terminación Paro")]
        public Nullable<DateTime> FechaTerminacionParo { get; set; }
        [DisplayName("Tiempo Falla")]
        public string TiempoFalla { get; set; }
        [DisplayName("Falla")]
        public string Falla { get; set; }
    }

    public class VBitacoraReporteContext : DbContext
    {

        public VBitacoraReporteContext() : base("name=DefaultConnection")
        {
        }
        public DbSet<VBitacoraReporte> VBitacoraReporte { get; set; }
    }

    [Table("VMatAsignado")]
    public class VMatAsignado
    {
        [Key]
        public int IDMaterialAsignado { get; set; }
        [DisplayName("No. Pedido")]
        public int NoPedido { get; set; }
        [DisplayName("Fecha Pedido")]
        public DateTime Fecha { get; set; }
        [DisplayName("No. Orden")]
        public int Orden { get; set; }
        [DisplayName("Clave")]
        public string Cref { get; set; }
        [DisplayName("Material")]
        public string Descripcion { get; set; }
        [DisplayName("Ancho")]
        public decimal ancho { get; set; }
        [DisplayName("Largo")]
        public decimal largo { get; set; }
        [DisplayName("M2 Asignados")]
        public decimal M2Asignados { get; set; }
        [DisplayName("M2 Entregados")]
        public decimal M2Entregados { get; set; }
        [DisplayName("MLAsignados")]
        public decimal MLAsignados { get; set; }
        [DisplayName("MLEntregados")]
        public decimal MLEntregado { get; set; }
        [DisplayName("Almacen")]
        public string Almacen { get; set; }

    }

    public class VMatAsignadoContext : DbContext
    {

        public VMatAsignadoContext() : base("name=DefaultConnection")
        {
        }
        public DbSet<VMatAsignado> VMatAsignado { get; set; }
    }



    public class BitacoraRepository
    {

        public IEnumerable<SelectListItem> GetTrabajadoresProcesoxMaquina(int IDMaquina)
            {
                List<SelectListItem> lista;
                if (IDMaquina == 0)
                {
                    lista = new List<SelectListItem>();
                    lista.Add(new SelectListItem() { Value = "0", Text = "Elige una Maquina Primero" });
                    return (lista);
                }

                int IDProceso = new ArticuloContext().Database.SqlQuery<ClsDatoEntero>("select IDProceso as Dato from Articulo inner join MaquinaProceso Articulo.IDArticulo=MaquinaProceso.IDArticulo where IDArticulo=" + IDMaquina).FirstOrDefault().Dato;

                using (var context = new TrabajadorContext())
                {
                    string cadenasql = "Select trabajador.*  from trabajador inner join trabajadorproceso on trabajador.idTrabajador=TrabajadorProceso.IDTrabajador where IDproceso="+IDProceso+" order by trabajador.IDTrabajador ";
                lista = context.Database.SqlQuery<Trabajador>(cadenasql).ToList()

                .Select(n =>
                 new SelectListItem
                 {
                     Value = n.IDTrabajador.ToString(),
                     Text = n.IDTrabajador.ToString() + " | " + n.Nombre
                     }).ToList();
                    var countrytip = new SelectListItem()
                    {
                        Value = null,
                        Text = "--- Selecciona un Trabajador ---"
                    };
                lista.Insert(0, countrytip);
                return new SelectList(lista, "Value", "Text");
                }

            } // fin del metodo


        public IEnumerable<SelectListItem> GetMaquinas()
        {
            List<SelectListItem> lista;
            lista = new List<SelectListItem>();
            using (var context = new ArticuloContext())
            {
                string cadenasql = "Select Articulo.*  from Articulo inner join MAquinaproceso on Articulo.IDArticulo=Maquinaproceso.IDArticulo where idtipoArticulo=3 order by case idproceso when 5 then 1 else idproceso end asc ";
                lista = context.Database.SqlQuery<Articulo>(cadenasql).ToList()

                .Select(n =>
                 new SelectListItem
                 {
                     Value = n.IDArticulo.ToString(),
                     Text = n.Descripcion.ToString() 
                 }).ToList();
                var countrytip = new SelectListItem()
                {
                    Value = null,
                    Text = "--- Selecciona una Maquina ---"
                };
                lista.Insert(0, countrytip);
                return new SelectList(lista, "Value", "Text");
            }

        } // fin del metodo

        [Table("VOrdenProduccionPedido")]
    public class VOrdenProduccionPedido
    {
        [Key]
        [DisplayName("IDPedido")]
        public int IDPedido { get; set; }
        [DisplayName("Fecha Pedido")]
        public DateTime FechaPedido { get; set; }

        [DisplayName("Cliente")]
        public string Cliente { get; set; }
        [DisplayName("Clave")]
        public string Clave { get; set; }
        [DisplayName("Descripción")]
        public string Descripcion { get; set; }
        [DisplayName("Presentación")]
        public string Presentacion { get; set; }

        [DisplayName("IDOrden")]
        public int IDOrden { get; set; }

        [DisplayName("Fecha Compromiso")]
        public Nullable<DateTime> FechaCompromiso { get; set; }
        [DisplayName("Fecha Inicio")]
        public Nullable<DateTime> FechaInicio { get; set; }
        [DisplayName("Fecha Programada")]
        public Nullable<DateTime> FechaProgramada { get; set; }
        [DisplayName("Fecha de Creación")]
        public Nullable<DateTime> FechaCreacion { get; set; }
        
        [DisplayName("Estado de la orden")]
        public string EstadoOrden { get; set; }


    }
    public class VOrdenProduccionPedidoContext : DbContext
    {

        public VOrdenProduccionPedidoContext() : base("name=DefaultConnection")
        {
        }
        public DbSet<VOrdenProduccionPedido> VOrdenProduccionPedido { get; set; }


    }

    [Table("VBitacoraReporte")]
    public class VBitacoraReporte
    {
        [Key]
        public int IDBitacora { get; set; }
        [DisplayName("Estado Bitacora")]
        public string EstadoBitacora { get; set; }
        [DisplayName("Pedido")]
        public int IDPedido{ get; set; }
        [DisplayName("Fecha Pedido")]
        public DateTime FechaPedido { get; set; }
        [DisplayName("Fecha Compromiso")]
        public DateTime FechaRequiere { get; set; }
        [DisplayName("Orden de Producción")]
        public int IDOrden { get; set; }
        [DisplayName("Estado Orden")]
        public string EstadoOrden { get; set; }
        [DisplayName("IDResponsable")]
        public int IDTrabajador { get; set; }
        [DisplayName("Responsable")]
        public string Trabajador { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true,
        DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}")]
        [DisplayName("Fecha Inicio")]
        public DateTime FechaInicio { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true,
         DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}")]
        [DisplayName("Fecha Fin")]
        public Nullable<DateTime> FechaFin { get; set; }
        [DisplayName("Diferencia de Horas")]
        public string DiferenciaHorasInicioFin { get; set; }
        [DisplayName("IDProceso")]
        public int IDProceso { get; set; }
        [DisplayName("Proceso")]
        public string NombreProceso { get; set; }

        [DisplayName("Observación")]
        public string Observacion { get; set; }
       
        [DisplayName("Cantidad")]
        public decimal Cantidad { get; set; }
        [DisplayName("Unidad")]
        public Nullable<int> IDClaveUnidad { get; set; }
        [DisplayName("Cantidad")]
        public string Unidad { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true,
        DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}")]
        [DisplayName("Fecha Incicio Paro")]
        public Nullable<DateTime> FechaInicioParo { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true,
        DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}")]
        [DisplayName("Fecha Terminación Paro")]
        public Nullable<DateTime> FechaTerminacionParo { get; set; }
        [DisplayName("Tiempo Falla")]
        public string TiempoFalla { get; set; }
        [DisplayName("Falla")]
        public string Falla { get; set; }
    }

    public class VBitacoraReporteContext : DbContext
    {

        public VBitacoraReporteContext() : base("name=DefaultConnection")
        {  
        }
        public DbSet<VBitacoraReporte> VBitacoraReporte { get; set; } 
    }

    [Table("VMatAsignado")]
    public class VMatAsignado
    {
        [Key]
        public int IDMaterialAsignado { get; set; }
        [DisplayName("No. Pedido")]
        public int NoPedido { get; set; }
        [DisplayName("Fecha Pedido")]
        public DateTime Fecha { get; set; }
        [DisplayName("No. Orden")]
        public int Orden { get; set; }
        [DisplayName("Clave")]
        public string Cref{ get; set; }
        [DisplayName("Material")]
        public string Descripcion { get; set; }
        [DisplayName("Ancho")]
        public decimal ancho { get; set; }
        [DisplayName("Largo")]
        public decimal largo { get; set; }
        [DisplayName("M2 Asignados")]
        public decimal M2Asignados { get; set; }
        [DisplayName("M2 Entregados")]
        public decimal M2Entregados { get; set; }
        [DisplayName("MLAsignados")]
        public decimal MLAsignados { get; set; }
        [DisplayName("MLEntregados")]
        public decimal MLEntregado { get; set; }
        [DisplayName("Almacen")]
        public string Almacen { get; set; }

    }

    public class VMatAsignadoContext : DbContext
    {

        public VMatAsignadoContext() : base("name=DefaultConnection")
        {
        }
        public DbSet<VMatAsignado> VMatAsignado { get; set; }
    }



    }  // fin del Repository


  }// fin del nameespace