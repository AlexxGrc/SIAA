using SIAAPI.Models.Administracion;
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

namespace SIAAPI.Models.Comercial
{
    [Table("EncRequisiciones")]
    public class EncRequisiciones
    {
       
        [Key]
        public int IDRequisicion { get; set; }
        [DisplayName("Fecha")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required]
        public DateTime Fecha { get; set; }
        [DisplayName("Fecha Requerida")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required]
        public DateTime FechaRequiere { get; set; }
        [DisplayName("Proveedor")]
        [Required]
        public int IDProveedor { get; set; }
        public virtual Proveedor Proveedor { get; set; }
        [DisplayName("Método de Pago")]
        [Required]
        public int IDMetodoPago { get; set; }
        public virtual c_MetodoPago c_MetodoPago { get; set; }
        [DisplayName("Forma de Pago")]
        [Required]
        public int IDFormapago { get; set; }
        public virtual c_FormaPago c_FormaPago { get; set; }
        [DisplayName("Divisa")]
        [Required]
        public int IDMoneda { get; set; }
        public virtual c_Moneda c_Moneda { get; set; }
        [DisplayName("Condiciones de Pago")]
        [Required]
        public int IDCondicionesPago { get; set; }
        public virtual CondicionesPago CondicionesPago { get; set; }
        [DisplayName("Almacen")]
        [Required]
        public int IDAlmacen{ get; set; }
        public virtual Almacen Almacen { get; set; }
        [DisplayName("Uso CFDI")]
        public int IDUsoCFDI { get; set; }
        public virtual c_UsoCFDI c_UsoCFDI { get; set; }
        [DisplayName("Observación")]
        [StringLength(250)]
        public string Observacion { get; set; }
        [DisplayName("Subtotal")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        [Required]
        public decimal Subtotal { get; set; }
        [DisplayName("IVA")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        [Required]
        public decimal IVA { get; set; }
        [DisplayName("Total")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        [Required]
        public decimal Total { get; set; }

        [DisplayName("Status")]
        [StringLength(20)]
        public string Status{ get; set; }

        [DisplayName("Tipo de Cambio")]
        public decimal TipoCambio { get; set; }


        [DisplayName("Usuario")]
        public int UserID { get; set; }
        public virtual User User { get; set; }

        public virtual ICollection<DetRequisiciones> DetRequisiones { get; set; }
        public virtual ICollection<DocuRelaRequisiciones> DocuRelaRequisiones { get; set; }

        public virtual ICollection<DocumentoRequisiciones> DocumentoRequisiones { get; set; }
    


    }



    [Table("DetRequisiciones")]
    public class DetRequisiciones
    {
        [Key]
        public int IDDetRequisiciones { get; set; }
        [DisplayName("Requisición")]
        [Required]
        public int IDRequisicion { get; set; }
        public virtual EncRequisiciones Requisicion { get; set; }
        [DisplayName("Artículo")]
        [Required]
        public int IDArticulo { get; set; }
        public virtual Articulo Articulo { get; set; }
        [DisplayName("Característica")]
        [Required]
        public int Caracteristica_ID { get; set; }
        public virtual Caracteristica Caracteristica { get; set; }
        [DisplayName("Presentación")]
        [Required]
        public string Presentacion { get; set; }
        [DisplayName("jsonPresentación")]
  
        public string jsonPresentacion { get; set; }
        [DisplayName("Cantidad")]
        [Required]
        public decimal Cantidad { get; set; }
        [DisplayName("Costo")]
        [Required]
        public decimal Costo { get; set; }
        [DisplayName("Cantidad Pedida")]
        [Required]
        public decimal CantidadPedida { get; set; }
        [DisplayName("Descuento")]
        public decimal Descuento { get; set; }
        [DisplayName("Importe")]
        [Required]
        public decimal Importe { get; set; }
        [DisplayName("IVA")]
        [Required]
        public bool IVA { get; set; }
        [DisplayName("Importe IVA")]
        [Required]
        public decimal ImporteIva { get; set; }
        [DisplayName("Importe Total")]
        [Required]
        public decimal ImporteTotal { get; set; }
        [DisplayName("Nota")]
    
        public string Nota { get; set; }
        [DisplayName("Orden de Compra")]
        [Required]
        public bool Ordenado { get; set; }
        public decimal Suministro { get; set; }
        public string Status { get; set; }
        [DisplayName("Almacen")]
        [Required]
        public int IDAlmacen { get; set; }
        public virtual Almacen Almacen { get; set; }

    }

    public class DocuRelaRequisiciones
    {
        [Key]
        public int IDDocuRelaRequisiciones { get; set; }
        [DisplayName("Requisición")]
        [Required]
        public int IDRequisicion { get; set; }
        public virtual EncRequisiciones Requisicion { get; set; }
        [DisplayName("Documento")]
        public string Documento { get; set; }
        [DisplayName("Número")]
        public int Numero { get; set; }
    }

    public class DocumentoRequisiciones
    {
        [Key]
        public int IDDocumentoRequisiciones { get; set; }
        [DisplayName("Requisición")]
        [Required]
        public int IDRequisicion { get; set; }
        public virtual EncRequisiciones Requisicion { get; set; }
        [DisplayName("Descripción")]
        [Required]
        [StringLength(250)]
        public string Descripcion { get; set; }
        [DisplayName("Documento")]
        public int Documento { get; set; }
    }

    public class RequisicionesContext : DbContext
    {

        public RequisicionesContext() : base("name=DefaultConnection")
        {
            Database.SetInitializer<RequisicionesContext>(null);
        }
        //Clases internas
        public DbSet<EncRequisiciones> EncRequisicioness { get; set; }
        public DbSet<DetRequisiciones> DetRequisicioness { get; set; }
        public DbSet<DocuRelaRequisiciones> DocuRelaRequisicioness { get; set; }
        public DbSet<DocumentoRequisiciones> DocumentoRequisicioness { get; set; }

        //Clases externas EncRequisiciones
        public DbSet<Proveedor> Proveedor { get; set; }
        public DbSet<c_MetodoPago> c_MetodoPagos { get; set; }
        public DbSet<c_FormaPago> c_FormaPagos { get; set; }
        public DbSet<c_Moneda> c_Monedas { get; set; }
        public DbSet<CondicionesPago> CondicionesPagos { get; set; }
        public DbSet<Almacen> Almacenes { get; set; }
        public DbSet<c_UsoCFDI> c_UsoCFDIS { get; set; }
        public DbSet<Articulo> Articulo { get; set; }
        public DbSet<Caracteristica> Caracteristica { get; set; }
    }


    [Table("VRequisiciones")]
    public class VRequisiciones
    {

        [Key]
        public int IDRequisicion { get; set; }
        [DisplayName("Fecha")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Fecha { get; set; }
        [DisplayName("Fecha Requerida")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime FechaRequiere { get; set; }
        [DisplayName("IDProveedor")]
        public int IDProveedor { get; set; }
        [DisplayName("RFC")]
        public string RFC { get; set; }
        [DisplayName("Proveedor")]
        public string Empresa { get; set; }
        [DisplayName("Subtotal")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal Subtotal { get; set; }
        [DisplayName("IVA")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal IVA { get; set; }
        [DisplayName("Total")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal Total { get; set; }

        [DisplayName("Divisa")]
        public string ClaveMoneda { get; set; }
        [DisplayName("Tipo de Cambio")]
        public decimal TipoCambio { get; set; }
        [DisplayName("Total")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal TotalPesos { get; set; }
        [DisplayName("Status")]
        public string Status { get; set; }
        [DisplayName("Almacen")]
        public string Almacen { get; set; }
        [DisplayName("Observación")]
        public string Observacion { get; set; }
        [DisplayName("Generado por")]
        public string Username { get; set; }
    }
    public class VRequisicionesContext : DbContext
    {
        public VRequisicionesContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<VRequisiciones> VRequisiciones { get; set; }
    }
}