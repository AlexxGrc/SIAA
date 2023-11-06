using SIAAPI.Models.Administracion;
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
    //Nombre de la tabla
    [Table("Proveedores")]
    public class Proveedor
    {
        //Primary Key IDProveedor
        [Key]
        public int IDProveedor { get; set; }

        //RazonSocial: Razón social del proveedor
        [DisplayName("Razón Social")]
        [Required]
        public int IDRegimenFiscal { get; set; }
        public virtual c_RegimenFiscal RegimenFiscal { get; set; }

        //Empresa: Nombre de la empresa del proveedor
        [DisplayName("Empresa")]
        [StringLength(100)]
        [Required]
        public string Empresa { get; set; }

        //Calle: Calle de la empresa del proveedor
        [DisplayName("Calle")]
        [StringLength(100)]
        [Required]
        public string Calle { get; set; }

        //NoExt: Número exterior de la empresa del proveedor
        [DisplayName("No. Exterior")]
        [StringLength(10)]
        public string NoExt { get; set; }

        //NoInt: Número interior de la empresa del proveedor
        [DisplayName("No. Interior")]
        [StringLength(10)]
        public string NoInt { get; set; }

        //Colonia: Colonia de la empresa del proveedor
        [DisplayName("Colonia")]
        [StringLength(100)]
        [Required]
        public string Colonia { get; set; }

        //Municipio: Municipio de la empresa del proveedor
        [DisplayName("Municipio")]
        [StringLength(100)]
        [Required]
        public string Municipio { get; set; }

        //CP: Código postal de la empresa del proveedor
        [DisplayName("C.P.")]
        [StringLength(5, MinimumLength = 5, ErrorMessage = "El campo C.P. debe contener 5 dígitos")]
        [Required]
        public string CP { get; set; }

        public int IDEstado { get; set; }
        public virtual Estados Estados { get; set; }
        [DisplayName("País")]
        [Required]
        public int IDPais { get; set; }
        public virtual Paises paises { get; set; }

        //IDMoneda: Tipo de moneda con la que se paga al proveedor
        [DisplayName("Divisa")]
        [Required]
        public int IDMoneda { get; set; }
        public virtual c_Moneda Moneda { get; set; }

        //IDFormadePago: Forma en la que se paga al proveedor
        [DisplayName("Forma de Pago")]
        [Required]
        public int IDFormaPago { get; set; }
        public virtual c_FormaPago FormaPago { get; set; }

        //IDMetododePago: Método de pago con el que se paga al proveedor
        [DisplayName("Método de Pago")]
        [Required]
        public int IDMetodoPago { get; set; }
        public virtual c_MetodoPago MetodoPago { get; set; }



        //IDConfianza: Calificación de confianza del proveedor
        [DisplayName("Confianza")]
        [StringLength(30)]
        [Required]
        public string Confianza { get; set; }

        //IDServicio: Calificación de servicio del proveedor
        [DisplayName("Servicio")]

        public decimal Servicio { get; set; }

        //IDTentrego: Calificación de puntualidad de entregra del proveedor
        [DisplayName("Tiempo de Entrega")]

        public decimal Tentrego { get; set; }

        //IDCalidad: Calificación de calidad del proveedor

        [DisplayName("Calidad")]

        public decimal Calidad { get; set; }



        //Estado
        [DisplayName("Estatus")]
        [StringLength(30)]
        [Required]
        public string Status { get; set; }

        [DisplayName("Producto")]
        [StringLength(100)]
        [Required]
        public string Producto { get; set; }

        [DisplayName("Condiciones de pago")]
        [Required]
        public int IDCondicionesPago { get; set; }

        [DisplayName("Teléfono 1")]
        [StringLength(15, MinimumLength = 10, ErrorMessage = "El campo Telefóno debe contener un mínimo de 10 dígitos")]

        public string Telefonouno { get; set; }

        [DisplayName("Teléfono 2")]
        [StringLength(15, MinimumLength = 10, ErrorMessage = "El campo Telefóno debe contener un mínimo de 10 dígitos")]

        public string Telefonodos { get; set; }

        public string RFC { get; set; }

        public int IDTipoIVA { get; set; }
        public virtual c_TipoIVA c_TiposIVA { get; set; }
        public virtual CondicionesPago CondicionesPago { get; set; }
        public virtual ICollection<c_TipoCuota> c_TipoCuota { get; set; }
        public virtual ICollection<ContactosProv> ContactosProv { get; set; }
        public virtual ICollection<BancosProv> BancosProv { get; set; }

   
    }

    public class ProveedorContext : DbContext
    {
        public ProveedorContext() : base("name=DefaultConnection")
        {
            Database.SetInitializer<ProveedorContext>(null);
        }
        public DbSet<Proveedor> Proveedores { get; set; }
        public DbSet<BancosProv> BancosProvs { get; set; }
        public DbSet<c_Moneda> c_Monedas { get; set; }
        public DbSet<c_FormaPago> c_FormaPagos { get; set; }
        public DbSet<Estados> Estados { get; set; }
        public DbSet<Paises> Paises { get; set; }
        public DbSet<c_MetodoPago> c_MetodoPagos { get; set; }
        public DbSet<c_RegimenFiscal> c_RegimenFiscal { get; set; }

        public DbSet<CondicionesPago> CondicionesPagos { get; set; }
        public DbSet<c_TipoIVA> c_TiposIVA { get; set; }
    }

    public class VProveedores
    {
        //Primary Key IDProveedor
        [Key]
        public int IDProveedor { get; set; }

        //Empresa: Nombre de la empresa del proveedor
        [DisplayName("Empresa")]
        public string Empresa { get; set; }
        public string RFC { get; set; }
        [DisplayName("Teléfono uno")]
        [StringLength(15, MinimumLength = 10, ErrorMessage = "El campo Telefóno debe contener un mínimo de 10 dígitos")]
        public string Telefonouno { get; set; }
        [DisplayName("Teléfono dos")]
        [StringLength(15, MinimumLength = 10, ErrorMessage = "El campo Telefóno debe contener un mínimo de 10 dígitos")]
        public string Telefonodos { get; set; }
        [DisplayName("Producto")]
       public string Producto { get; set; }
        //Calle: Calle de la empresa del proveedor
        [DisplayName("Calle")]
         public string Calle { get; set; }

        //NoExt: Número exterior de la empresa del proveedor
        [DisplayName("No. Exterior")]
        [StringLength(10)]
        public string NoExt { get; set; }

        //Colonia: Colonia de la empresa del proveedor
        [DisplayName("Colonia")]
        public string Colonia { get; set; }

        //Municipio: Municipio de la empresa del proveedor
        [DisplayName("Municipio")]
        public string Municipio { get; set; }

        //IDEstado: Estado donde se ubica la empresa del proveedor
        [DisplayName("Estado")]
        public string Estado { get; set; }
        [DisplayName("Pais")]
        public string Pais { get; set; }

        [DisplayName("IDFormaPago")]
        public int IDFormaPago { get; set; }
        [DisplayName("FormaPago")]
        public string FormaPago { get; set; }
        [DisplayName("IDMetodoPago")]
        public int IDMetodoPago { get; set; }
        [DisplayName("MetodoPago")]
        public string MetodoPago { get; set; }
        [DisplayName("IDMoneda")]
        public int IDMoneda { get; set; }
        [DisplayName("Moneda")]
        public string Moneda { get; set; }
        [DisplayName("IDRegimenFiscal")]
        public int IDRegimenFiscal { get; set; }
        [DisplayName("RegimenFiscal")]
        public string RegimenFiscal { get; set; }
        [DisplayName("Status")]
        public string Status { get; set; }
    }
    public class VProveedoresContext : DbContext
    {
        public VProveedoresContext() : base("name=DefaultConnection")
        {
            Database.SetInitializer<VProveedoresContext>(null);
        }
        public DbSet<VProveedores> VProveedores { get; set; }
    }
        public class ElementosRepository
    {
        public IEnumerable<SelectListItem> GetNumeros()
        {

            var lista = new List<SelectListItem>();
         
                var diez = new SelectListItem()
                { Value = "10",
                    Text = "10" }; lista.Insert(0, diez);
            var nueve = new SelectListItem()
            {Value = "9",
                Text = "9"};lista.Insert(0, nueve);
            var ocho = new SelectListItem()
            {Value = "8",
                Text = "8"}; lista.Insert(0, ocho);
            var siete = new SelectListItem()
            {Value = "7",
                Text = "7" }; lista.Insert(0, siete);
            var seis = new SelectListItem()
            {Value = "6",
                Text = "6"};
            lista.Insert(0, seis);
          
            var cero = new SelectListItem()
            {Value = null,
                Text = "--- Selecciona una Calificación---"
            }; lista.Insert(0, cero);
            return new SelectList(lista, "Value", "Text");
        }

        public IEnumerable<SelectListItem> GetStatus()
        {

            var lista = new List<SelectListItem>();

            var activo = new SelectListItem()
            {Value = "Activo",
                Text = "Activo"
            }; lista.Insert(0, activo);
            var inactivo = new SelectListItem()
            {Value = "Suspendido",
                Text = "Suspendido"
            }; lista.Insert(0, inactivo);
            var obsoleto = new SelectListItem()
            {Value = "Obsoleto",
                Text = "Obsoleto"
            }; lista.Insert(0, obsoleto);
            return new SelectList(lista, "Value", "Text");
        }

        public IEnumerable<SelectListItem> GetConfianza()
        {

            var lista = new List<SelectListItem>();
            var noconfiable = new SelectListItem()
            {
                Value = "No confiable",
                Text = "No confiable"
            }; lista.Insert(0, noconfiable);
            var confiable = new SelectListItem()
            {
                Value = "Confiable",
                Text = "Confiable"
            }; lista.Insert(0, confiable);

            var Condicionado = new SelectListItem()
            {
                Value = "Condicionado",
                Text = "Condicionado"
            }; lista.Insert(0, Condicionado);

            return new SelectList(lista, "Value", "Text");
        }
    }
}




