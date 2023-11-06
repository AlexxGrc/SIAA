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

namespace SIAAPI.Models.Produccion
{
    [Table("OrdenProduccion")]
    public class OrdenProduccion
    {
        [Key]
        [DisplayName("Código de la Orden")]
        public int IDOrden { get; set; }
        [DisplayName("Fecha Creacion")]

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime FechaCreacion { get; set; }

        [DisplayName("Modelo Producción")]
        public int IDModeloProduccion { get; set; }
        public virtual ModelosDeProduccion ModeloProduccion { get; set; }
        [DisplayName("Cliente")]
        public int IDCliente { get; set; }
        public virtual Clientes Cliente { get; set; }
        [DisplayName("Artículo")]
        public int IDArticulo { get; set; }

        public virtual Articulo Articulo { get; set; }


        [DisplayName("Caractersitica")]
        public int IDCaracteristica { get; set; }
        [DisplayName("Descripción")]
        public string Descripcion { get; set; }
        [DisplayName("Presentación")]
        public string Presentacion { get; set; }
        [DisplayName("Indicaciones")]
        public string Indicaciones { get; set; }
        [DisplayName("Fecha Compromiso")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime FechaCompromiso { get; set; }
        [DisplayName("Fecha Inicio")]
        [DataType(DataType.Date)]
        public DateTime FechaInicio { get; set; }
        [DisplayName("Fecha Programada")]
        [DataType(DataType.Date)]
        public Nullable<DateTime> FechaProgramada { get; set; }
        [DisplayName("Fecha Real de Inicio")]
        public Nullable<DateTime> FechaRealdeInicio { get; set; }
        [DisplayName("Fecha Real de Terminacion")]
        public Nullable<DateTime> FechaRealdeTerminacion { get; set; }
        [DisplayName("Cantidad")]
        public decimal Cantidad { get; set; }
        [DisplayName("Pedido")]
        public int IDPedido { get; set; }
        public virtual EncPedido Pedido { get; set; }
        [DisplayName("Detalle Pedido")]
        public int IDDetPedido { get; set; }
        public virtual DetPedido DetPedido { get; set; }
        [DisplayName("Prioridad")]
        public int Prioridad { get; set; }
        [DisplayName("Estado de la Orden")]
        public string EstadoOrden { get; set; }

        //[DisplayName("Proceso Actual")]
        //public int IDProceso { get; set; }
        //public virtual Proceso Proceso { get; set; }
        public int UserID { get; set; }
        public virtual User User { get; set; }
        public string Liberar { get; set; }

        public int IDHE { get; set; }
        public bool Arrastre { get; set; }
    }
    public class OrdenProduccionContext : DbContext
    {
        public OrdenProduccionContext() : base("name=DefaultConnection")
        {
            Database.SetInitializer<OrdenProduccionContext>(null);
        }
        public DbSet<OrdenProduccion> OrdenesProduccion { get; set; }

        public System.Data.Entity.DbSet<SIAAPI.Models.Comercial.Articulo> Articuloes { get; set; }

        public System.Data.Entity.DbSet<SIAAPI.Models.Comercial.Clientes> Clientes { get; set; }

        public System.Data.Entity.DbSet<SIAAPI.Models.Comercial.DetPedido> DetPedidoes { get; set; }

        public System.Data.Entity.DbSet<SIAAPI.Models.Produccion.ModelosDeProduccion> ModelosDeProduccions { get; set; }

        public System.Data.Entity.DbSet<SIAAPI.Models.Comercial.EncPedido> EncPedidoes { get; set; }

        public System.Data.Entity.DbSet<SIAAPI.Models.Login.User> Users { get; set; }
    }
    [Table("OrdenProduccion")]
    public class VOrdenProduccion
    {
        [Key]
        [DisplayName("Código de la Orden")]
        public int IDOrden { get; set; }
        [DisplayName("Modelo Producción")]
        public string ModeloProduccion { get; set; }
        public int IDModeloProduccion { get; set; }
        [DisplayName("Cliente")]
        public string Cliente { get; set; }
        [DisplayName("Artículo")]
        public int IDArticulo { get; set; }
        [DisplayName("Caractersitica")]
        public int IDCaracteristica { get; set; }
        [DisplayName("Descripción")]
        public string Descripcion { get; set; }
        [DisplayName("Presentación")]
        public string Presentacion { get; set; }
        [DisplayName("Indicaciones")]
        public string Indicaciones { get; set; }
        [DisplayName("Fecha Compromiso")]
        [DataType(DataType.Date)]
        public DateTime FechaCompromiso { get; set; }
        [DisplayName("Fecha Compromiso")]
        [DataType(DataType.Date)]
        public DateTime FechaInicio { get; set; }

       
        [DisplayName("Fecha Creacion")]
        [DataType(DataType.Date)]
        public DateTime FechaCreacion { get; set; }

        [DisplayName("Fecha Programada")]
        [DataType(DataType.Date)]
        public Nullable<DateTime> FechaProgramada { get; set; }
        [DisplayName("Fecha Real de Inicio")]
        public Nullable<DateTime> FechaRealdeInicio { get; set; }
        [DisplayName("Fecha Real de Terminacion")]
        public Nullable<DateTime> FechaRealdeTerminacion { get; set; }
        [DisplayName("Cantidad")]
        public decimal Cantidad { get; set; }
        [DisplayName("Pedido")]
        public int IDPedido { get; set; }
        [DisplayName("Detalle Pedido")]
        public int IDDetPedido { get; set; }
        [DisplayName("Prioridad")]
        public int Prioridad { get; set; }
        [DisplayName("Estado de la Orden")]
        public string EstadoOrden { get; set; }

    }
    public class VOrdenProduccionContext : DbContext
    {
        public VOrdenProduccionContext() : base("name=DefaultConnection")
        {
            Database.SetInitializer<VOrdenProduccionContext>(null);
        }
        public DbSet<VOrdenProduccion> VOrdenesProduccion { get; set; }
    }
    [Table("OrdenProduccionDetalle")]
    public class OrdenProduccionDetalle
    {
        [Key]
        public int IDOrdenDetalle { get; set; }
        [DisplayName("Orden de Producción")]
        public int IDOrden { get; set; }
        public virtual OrdenProduccion OrdenProduccion { get; set; }
        [DisplayName("Proceso")]
        public int IDProceso { get; set; }
        public virtual Proceso Proceso { get; set; }
        [DisplayName("Estado de la Orden")]
        public string EstadoProceso { get; set; }

        //[DisplayName("Usuario")]
        //public int UserID { get; set; }
        //public virtual User User { get; set;}

    }
	public class OrdenProduccionPrioridades
    {
        
        public int IDOrden { get; set; }
      
        public int IDProceso { get; set; }
    
        public string EstadoProceso { get; set; }
        public string Cliente { get; set; }
        public string Articulo { get; set; }
        public string Caracteristica { get; set; }

        public int Prioridad { get; set; }
      

    }
    [Table("Prioridades")]
    public class Prioridades
    {
        [Key]
        public int IDPrioridades { get; set; }

        public int IDOrden { get; set; }

        public int IDProceso { get; set; }
        public int IDMaquina { get; set; }
        public int Prioridad { get; set; }
        public string Estado { get; set; }

    


    }
    public class PrioridadesContext : DbContext
    {
        public PrioridadesContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<Prioridades> Prioridades { get; set; }
    }
    public class OrdenProduccionDetalleContext : DbContext
    {
        public OrdenProduccionDetalleContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<OrdenProduccionDetalle> OrdenProduccionDetalles { get; set; }
    }
    [Table("ArticuloProduccion")]
    public class ArticuloProduccion
    {

        [Key]
        public int IDArtProd { get; set; }


        [DisplayName("Artículo")]
        [Required]
        public int IDArticulo { get; set; }


        [DisplayName("Tipo de Artículo")]
        [Required]
        public int IDTipoArticulo { get; set; }

        [DisplayName("Carasterística")]
        [Required]
        public int IDCaracteristica { get; set; }
        [DisplayName("Proceso")]
        [Required]
        public int IDProceso { get; set; }
        public virtual Proceso Proceso { get; set; }
        [DisplayName("Orden de Producción")]
        public int IDOrden { get; set; }


        //[DisplayName("Versión")]
        //[Required]
        //public int Version { get; set; }



        [DisplayName("Cantidad")]
        public decimal Cantidad { get; set; }
        [DisplayName("Unidad")]
        public int IDClaveUnidad { get; set; }
        [DisplayName("Indicaciones ")]
        public string Indicaciones { get; set; }

        [DisplayName("Costo Planeado")]
        public decimal CostoPlaneado { get; set; }
        [DisplayName("Costo Real")]
        public decimal CostoReal { get; set; }
        public bool Existe { get; set; }

        [DisplayName("Hoja de Especificación")]
        [Required]
        public int IDHE { get; set; }

        public decimal TC { get; set; }

        public decimal TCR { get; set; }
    }
    public class VArticulosProduccion
    {

        [Key]
        public int IDArtProd { get; set; }
        public int IDArticulo { get; set; }
        public int IDCaracteristica { get; set; }
        public int IDHE { get; set; }

        //public int Version { get; set; }

        public string Articulo { get; set; }

        public string TipoArticulo { get; set; }

        public string Caracteristica { get; set; }

        public string Proceso { get; set; }

        public int IDOrden { get; set; }
        public decimal Cantidad { get; set; }

        public int IDClaveUnidad { get; set; }
        public string Unidad { get; set; }
        public string Indicaciones { get; set; }

        //public int Planeacion { get; set; }

        public decimal CostoPlaneado { get; set; }

        public decimal CostoReal { get; set; }
        public bool Existe { get; set; }
        public string Cref { get; set; }


    }
    public class ArticulosProduccionContext : DbContext
    {
        public ArticulosProduccionContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<ArticuloProduccion> ArticulosProducciones { get; set; }
    }
    [Table("ArticuloProduccionReal")]
    public class ArticuloProduccionReal
    {
        [Key]
        public int IDArtProdR { get; set; }

        [DisplayName("ID de orden")]
        public int IDOrden { get; set; }
        [DisplayName("ID de Artículo")]
        public int IDArtProd { get; set; }
        [DisplayName("ID de Característica")]
        public int IDCaracteristica { get; set; }
        [DisplayName("ID del Trabajador")]
        public int IDTrabajador { get; set; }
        [DisplayName("ID del Proceso")]
        public int IDProceso { get; set; }
        [DisplayName("Cantidad")]
        public decimal Cantidad { get; set; }
        [DisplayName("Unidad")]
        public int IDClaveUnidad { get; set; }

    }

    public class ArticuloProduccionRealContext : DbContext
    {
        public ArticuloProduccionRealContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<ArticuloProduccionReal> ArticuloProduccionReals { get; set; }
    }
    [Table("EstadoOrden")]
    public class EstadoOrden
    {
        [Key]
        [DisplayName("Código dela Orden")]
        public int IDEstadoOrden { get; set; }
        [DisplayName("Descripción")]
        public string Descripcion { get; set; }
        [DisplayName("Tipo")]
        public string Tipo { get; set; }
    }
    public class EstadoOrdenContext : DbContext
    {
        public EstadoOrdenContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<EstadoOrden> EstadoOrdenes { get; set; }
    }


    [Table("DocumentoProduccion")]
    public class VDocumentoProduccion
    {

        [Key]
        public int IDDocumento { get; set; }
        public int IDOrden { get; set; }
        public int IDProceso { get; set; }
        public string Proceso { get; set; }
        public string Descripcion { get; set; }
        public int Planeacion { get; set; }
        public int Version { get; set; }
        public string Nombre { get; set; }
    }

    [Table("DocumentoProduccion")]
    public class DocumentoProduccion
    {

        [Key]
        public int IDDocumento { get; set; }
        public int IDOrden { get; set; }
        public int IDProceso { get; set; }
        public string Descripcion { get; set; }
        public int Planeacion { get; set; }
        public int Version { get; set; }
        public string Nombre { get; set; }
    }
    public class DocumentoProduccionContext : DbContext
    {
        public DocumentoProduccionContext() : base("name=DefaultConnection")
        {

        }
        //Clases internas
        public DbSet<DocumentoProduccion> DocumentosProduccion { get; set; }
    }



    [Table("CambioEstado")]
    public class CambioEstado
    {
        [Key]
        public int IDCambioEdo { get; set; }
        [DisplayName("ID de Orden")]
        public int IDOrden { get; set; }
        //public virtual OrdenProduccion OrdenProducciones { get; set; }
        [DisplayName("Fecha del cambio ")]
        public DateTime Fecha { get; set; }
        [DisplayName("Hora ")]
        public TimeSpan Hora { get; set; }
        [DisplayName("Estado Anterior ")]
        public string EstadoAnterior { get; set; }
        [DisplayName("Estado Actual ")]
        public string EstadoActual { get; set; }
        //public virtual EstadoOrden EstadoOrdenes { get; set; }

        [DisplayName("Motivo")]
        public string motivo { get; set; }
        [DisplayName("Usuario que hizo el cambio")]
        public string Usuario { get; set; }
        //  public virtual Trabajador Trabajadores { get; set; }
    }

    public class CambioEstadoContext : DbContext
    {
        public CambioEstadoContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<CambioEstado> CambioEstados { get; set; }
        public DbSet<OrdenProduccion> OrdenProducciones { get; set; }
        public DbSet<EstadoOrden> EstadoOrdenes { get; set; }
        public DbSet<Trabajador> Trabajadores { get; set; }

    }

    public class VOP
    {
        [Key]
        [DisplayName("Código de la Orden")]
        public int IDOrden { get; set; }
        [DisplayName("Código del Pedido")]
        public int IDPedido { get; set; }
        public int IDCliente { get; set; }
        public string noExpediente { get; set; }
        [DisplayName("Cliente")]
        public string Cliente { get; set; }
        public int IDModeloProduccion { get; set; }
        [DisplayName("Modelo Producción")]
        public string Modelo { get; set; }
        [DisplayName("Artículo")]
        public int IDArticulo { get; set; }
        [DisplayName("Caractersitica")]
        public int IDCaracteristica { get; set; }
        [DisplayName("Descripción")]
        public string Descripcion { get; set; }
        [DisplayName("Presentación")]
        public string Presentacion { get; set; }
        [DisplayName("Cantidad")]
        public decimal Cantidad { get; set; }
        [DisplayName("Indicaciones")]
        public string Indicaciones { get; set; }
        [DisplayName("Fecha Compromiso")]
        [DataType(DataType.Date)]
        public DateTime FechaCompromiso { get; set; }
        [DisplayName("Fecha Inicio")]
        [DataType(DataType.Date)]
        public DateTime FechaInicio { get; set; }
        [DisplayName("Fecha Programada")]
        [DataType(DataType.Date)]
        public Nullable<DateTime> FechaProgramada { get; set; }
        [DisplayName("Prioridad")]
        public int Prioridad { get; set; }
        [DisplayName("Liberar")]
        public string Liberar { get; set; }
        [DisplayName("Estado de la Orden")]
        public string EstadoOrden { get; set; }
        [DisplayName("Fecha Creacio")]
        public DateTime FechaCreacion { get; set; }
        public int UserID { get; set; }
        [DisplayName("Usuario")]
        public string UserName { get; set; }
        public string Oficina { get; set; }

    }
    public class VOPContext : DbContext
    {
        public VOPContext() : base("name=DefaultConnection")
        {
            Database.SetInitializer<VOPContext>(null);
        }
        public DbSet<VOP> VOPs { get; set; }
    }


    /////////////////////////////
    ///

    public class VOrdenesALiberar
    {

        [Key]
        public int IDOrden { get; set; }
        public int IDArticulo { get; set; }
        public int IDCaracteristica { get; set; }
       

        public int IDPedido { get; set; }

        public int IDCliente { get; set; }

        public string Presentacion { get; set; }
        public int IDOrdenesALiberar { get; set; }
        public decimal Cantidad { get; set; }

        


    }
    public class VOrdenesALiberarContext : DbContext
    {
        public VOrdenesALiberarContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<VOrdenesALiberar> VOrdenesALiberar { get; set; }
    }














}