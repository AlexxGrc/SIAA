using SIAAPI.Models.Administracion;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Collections;
using System.Web.Mvc;
using System.Linq;

namespace SIAAPI.Models.Comercial
{
    //Tabla Clientes
    [Table("Clientes")]
    public class Clientes
    {
        [Key]
        public int IDCliente { get; set; }
        [DisplayName("Nombre")]
        [Required]
        //[StringLength(200)]
        public string Nombre { get; set; }
        [DisplayName("Mayorista")]
        public bool Mayorista { get; set; }
        [DisplayName("Factura cantidad exacta")]
        public bool FacturacionExacta { get; set; }
        [DisplayName("Grupo")]
        [Required]
        public int IDGrupo { get; set; }
        public virtual c_Grupo c_Grupo { get; set; }
        [DisplayName("Regimen social")]
        [Required]
        public int IDRegimenFiscal { get; set; }
        public virtual c_RegimenFiscal c_RegimenFiscal { get; set; }
        [DisplayName("Estatus")]
        [Required]
        public string Status { get; set; }
        [DisplayName("Oficina")]
        [Required]
        public int IDOficina { get; set; }
        public virtual Oficina Oficina { get; set; }
        [DisplayName("Correo contacto")]
        [Required]
        //[StringLength(100)]
        public string Correo { get; set; }
        [DisplayName("Contraseña")]
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DisplayName("Teléfono")]
        [StringLength(200, MinimumLength = 10, ErrorMessage = "El campo Telefóno debe contener un mínimo de 10 dígitos")]
        [Required]
        public string Telefono { get; set; }
        [DisplayName("Calle")]
        [Required]
        //[StringLength(100)]
        public string Calle { get; set; }
        [DisplayName("No. Exterior")]
        [Required]
        //[StringLength(10)]
        public string NumExt { get; set; }
        [DisplayName("No. Interior")]

        //[StringLength(10)]
        public string NumInt { get; set; }
        [DisplayName("Colonia")]
        [Required]
        //[StringLength(100)]
        public string Colonia { get; set; }
        [DisplayName("Municipio")]
        [Required]
        //[StringLength(100)]
        public string Municipio { get; set; }
        [DisplayName("C.P.")]
        [Required]
        //[StringLength(5, MinimumLength = 5, ErrorMessage = "El campo C.P. debe contener 5 dígitos")]
        public string CP { get; set; }
        [DisplayName("Estado")]
        [Required]
        public int IDEstado { get; set; }
        public virtual Estados Estados { get; set; }
        [DisplayName("País")]
        [Required]
        public int IDPais { get; set; }
        public virtual Paises paises { get; set; }
        [DisplayName("Vendedor")]
        [Required]
        public int IDVendedor { get; set; }
        public virtual Vendedor Vendedor { get; set; }
        [DisplayName("Observación")]
        //[StringLength(250)]
        public string Observacion { get; set; }

        [DisplayName("CURP")]
     //   [StringLength(18, MinimumLength = 13, ErrorMessage = "El campo CURP debe contener 18 dígitos al menos 13")]
        public string Curp { get; set; }
        [DisplayName("Ventas Acumuladas")]
        [Required]
        public decimal VentasAcu { get; set; }
        [Display(Name = "Fecha de ultima venta")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Ultimaventa { get; set; }
        [DisplayName("RFC")]
        [Required]
        [StringLength(13, MinimumLength = 12, ErrorMessage = "El campo RFC debe contener 12 0 13  dígitos")]
        public string RFC { get; set; }
        [DisplayName("Correo CFDI")]
        [Required]
        //[StringLength(100)]
        public string CorreoCfdi { get; set; }

        [DisplayName("Forma de Pago")]
        [Required]
        public int IDFormapago { get; set; }
        public virtual c_FormaPago c_FormaPago { get; set; }
        [DisplayName("Método de Pago")]
        [Required]
        public int IDMetodoPago { get; set; }
        public virtual c_MetodoPago c_MetodoPago { get; set; }
        [DisplayName("Divisa")]
        [Required]
        public int IDMoneda { get; set; }
        public virtual c_Moneda c_Moneda { get; set; }

        [DisplayName("Condiciones de pago")]
        [Required]
        public int IDCondicionesPago { get; set; }

        [DisplayName("Uso CFDI")]
        [Required]
        public int IDUsoCFDI { get; set; }
        public virtual c_UsoCFDI c_UsoCFDI { get; set; }

        [DisplayName("Certificado de Calidad")]
        [Required]
        public bool CertificadoCalidad { get; set; }

        [DisplayName("Cuenta Contable")]
        [Required]
        public string cuentaContable { get; set; }
        public virtual CondicionesPago CondicionesPago { get; set; }

        [DisplayName("Correo de complemento de pago")]
        public string CorreoPagoC { get; set; }
        [DisplayName("No Expediente")]
        public string noExpediente { get; set; }
        public bool SinFactura { get; set; }

       
        [Required]
        public string Nombre40 { get; set; }

        [DisplayName("Regimen Societario")]
        [Required]
        public string RegimenSocietario { get; set; }
        public virtual ICollection<ReferenciaPagoCliente> ReferenciaPagoCliente { get; set; }
        public virtual ICollection<ContactosClie> ContactosClie { get; set; }
        public virtual ICollection<Cobro> Cobro { get; set; }
        public virtual ICollection<Entrega> Entrega { get; set; }
        public virtual ICollection<c_TipoCuota> c_TipoCuota { get; set; }

    }


    [Table("VCliente")]
    public class VCliente
    {
        [Key]
        public int IDCliente { get; set; }
        public string noExpediente { get; set; }
        [DisplayName("Nombre")]
        public string Nombre { get; set; }
        public string Grupo { get; set; }
        [DisplayName("Mayorista")]
        public bool Mayorista { get; set; }
        public string Correo { get; set; }
        public string Telefono { get; set; }
        public string Municipio { get; set; }
        public string Estado { get; set; }
        public string Status { get; set; }
        public string Vendedor { get; set; }
    }
    public class VClienteContext : DbContext
    {

        public VClienteContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<VCliente> VClientes { get; set; }
    }

    public class ClientesContext : DbContext
    {

        public ClientesContext() : base("name=DefaultConnection")
        {
            Database.SetInitializer<ClientesContext>(null);
        }
        public DbSet<Clientes> Clientes { get; set; }
        public DbSet<Cobro> Cobros { get; set; }
        public DbSet<Entrega> Entregas { get; set; }
        public DbSet<ContactosClie> ContactosClies { get; set; }
        public DbSet<c_Grupo> c_Grupos { get; set; }
        public DbSet<Oficina> Oficinas { get; set; }
        public DbSet<Vendedor> Vendedores { get; set; }
        public DbSet<c_Banco> c_Bancos { get; set; }
        public DbSet<c_FormaPago> c_FormaPagos { get; set; }
        public DbSet<c_MetodoPago> c_MetodoPagos { get; set; }
        public DbSet<c_Moneda> c_Monedas { get; set; }
        public DbSet<c_RegimenFiscal> c_RegimenFiscales { get; set; }
        public DbSet<c_UsoCFDI> c_UsoCFDIS { get; set; }
        public DbSet<Estados> estados{ get; set; }
        public DbSet<Paises> paises { get; set; }

        public System.Data.Entity.DbSet<SIAAPI.Models.Administracion.CondicionesPago> CondicionesPagos { get; set; }
    }
    public class FechaCreacionDomEC
    {
        [Key]
        public int IDCreacion { get; set; }
        public DateTime Fecha { get; set; }
        public int IDUsuario { get; set; }
        public int IDEntrega { get; set; }
    }

    public class FechaCreacionDomECContext : DbContext
    {
        public FechaCreacionDomECContext() : base("name=DefaultConnection")
        {

        }
        DbSet<FechaCreacionDomEC> fechacreacionDomicilio { get; set; }
    }

    public class VFechaCreacionDomEC
    {
        [Key]
        public int IDCreacion { get; set; }
        public DateTime Fecha { get; set; }
        public int IDUsuario { get; set; }
        public int IDEntrega { get; set; }
        public string Username { get; set; }
    }

    public class VFechaCreacionDomECContext : DbContext
    {
        public VFechaCreacionDomECContext() : base("name=DefaultConnection")
        {

        }
        DbSet<VFechaCreacionDomEC> vfechacreacionDomicilio { get; set; }
    }

    public class ClienteRepository
    {
        public IEnumerable<SelectListItem> GetClientes()
        {
            using (var context = new ClientesContext())
            {
                //List<SelectListItem> listaPeriocidadPago = context.Clientes.AsNoTracking().Include(p => p.c_Grupo)
                                 //   .OrderBy(n => n.IDGrupo).ThenBy(n => n.Nombre)
                List<SelectListItem> listaPeriocidadPago = context.Clientes.AsNoTracking()
                    .OrderBy(n => n.Nombre)
                        .Select(n =>
                        new SelectListItem
                        {
                            Value = n.IDCliente.ToString(),
                            Text =  n.Nombre
                        }).ToList();

                var descripciontip = new SelectListItem()
                {
                    Value = "0",
                    Text = "--- Selecciona un cliente ---"
                };
                listaPeriocidadPago.Insert(0, descripciontip);
                return new SelectList(listaPeriocidadPago, "Value", "Text");
            }
        }
        public IEnumerable<SelectListItem> GetClientesNombre()
        {
            using (var context = new ClientesContext())
            {
                List<SelectListItem> listaPeriocidadPago = context.Clientes.AsNoTracking().Include(p => p.c_Grupo)
                    .OrderBy(n => n.Nombre)
                        .Select(n =>
                        new SelectListItem
                        {
                            Value = n.IDCliente.ToString(),
                            Text = n.Nombre
                        }).ToList();
                var descripciontip = new SelectListItem()
                {
                    Value = "0",
                    Text = "--- Selecciona un cliente ---"
                };
                listaPeriocidadPago.Insert(0, descripciontip);
                return new SelectList(listaPeriocidadPago, "Value", "Text");
            }
        }

        public IEnumerable<SelectListItem> GetClientesFactura()
        {
            using (var context = new ClientesContext())
            {
                List<SelectListItem> listaPeriocidadPago = context.Clientes.AsNoTracking().Include(p => p.c_Grupo)
                    .OrderBy(n => n.Nombre)
                        .Select(n =>
                        new SelectListItem
                        {
                            Value = n.Nombre,
                            Text = n.Nombre
                        }).ToList();
                var descripciontip = new SelectListItem()
                {
                    Value = "Todas",
                    Text = "Todas"
                };
                //listaPeriocidadPago.Insert(0, descripciontip);
                return new SelectList(listaPeriocidadPago, "Value", "Text");
            }
        }

        public class ClienteAllRepository
        {
            public IEnumerable<SelectListItem> GetClientes()
            {
                using (var context = new ClientesContext())
                {
                    List<SelectListItem> listaCliente = context.Clientes.AsNoTracking()
                        .OrderBy(n => n.Nombre)
                            .Select(n =>
                            new SelectListItem
                            {
                                Value = n.IDCliente.ToString(),
                                Text = n.Nombre + "\t|" + n.RFC
                            }).ToList();
                    var descripciontip = new SelectListItem()
                    {
                        Value = "0",
                        Text = "--- Seleccione un cliente ---"
                    };
                    listaCliente.Insert(0, descripciontip);
                    return new SelectList(listaCliente, "Value", "Text");
                }
            }

            public List<Clientes> GetClientesList()
            {
                using (var context = new ClientesContext())
                {
                    List<Clientes> listaCliente = context.Clientes.ToList();

                    return listaCliente;
                }
            }
        }
    }
}

