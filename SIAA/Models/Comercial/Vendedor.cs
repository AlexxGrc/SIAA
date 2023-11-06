using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Web.Mvc;
using SIAAPI.Models.Administracion;

namespace SIAAPI.Models.Comercial
{
    [Table("Vendedor")]
    public class Vendedor
    {
        [Key]
        public int IDVendedor { get; set; }


        [Required(ErrorMessage = "Campo obligatorio")]
        [DisplayName("RFC")]
        public string RFC { get; set; }

        [Required(ErrorMessage = "Campo obligatorio")]
        [DisplayName("Nombre")]
        //[StringLength(150)]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Campo obligatorio")]
        [DisplayName("Cuota")]
        //[DisplayName("Cuota mensual")]
        public decimal CuotaVendedor { get; set; }

        [Required(ErrorMessage = "Campo obligatorio")]
        [DisplayName("Tipo de cuota")]
        public int IDTipoCuota { get; set; }
        public virtual ICollection<c_TipoCuota> c_TipoCuota { get; set; }

        [DisplayName("Tipo de contrato")]
        public int IDTipoContrato { get; set; }
        public virtual c_TipoContrato c_TipoContrato { get; set; }

        [DisplayName("Autorizado a cotizar")]
        public bool AutorizadoACotizar { get; set; }

        [DisplayName("Activo")]
        public bool Activo { get; set; }

        [Required(ErrorMessage = "Campo obligatorio")]
        [DisplayName("Porcentaje comisión")]
        public bool Porcentajecomision { get; set; }

        [DisplayName("Tipo de vendedor")]
        public int IDTipoVendedor { get; set; }
        public virtual TipoVendedor TipoVendedor { get; set; }

        [DisplayName("Periodicidad de pago")]
        public int IDPeriocidadPago { get; set; }
        public virtual c_PeriodicidadPago c_PeriodicidadPago { get; set; }
        [DisplayName("Oficina")]
        public int IDOficina { get; set; }
        public virtual Oficina Oficina { get; set; }
        [DisplayName("Correo")]
        public string Mail { get; set; }
        [DisplayName("Teléfono")]
        //[StringLength(15)]
        public string Telefono { get; set; }
        [DisplayName("Contraseña")]
        //[Required]
        [DataType(DataType.Password)]
        public string Perfil { get; set; }
        [DisplayName("Foto")]
        public byte[] Photo { get; set; }
        [DisplayName("Notas")]
        //[StringLength(250)]
        public string Notas { get; set; }
        [DisplayName("Divisa de comisión")]
        public int IDMoneda { get; set; }
        public virtual c_Moneda c_Moneda { get; set; }

        [DisplayName("Porcentaje de comisión")]
        public decimal Comision { get; set; }

        [DisplayName("Esquema de comisión")]
        //[StringLength(20)]
        public string EsquemaComision{ get; set; }
        
        public virtual ICollection<Clientes> Clientes { get; set; }
        public virtual ICollection<ClientesP> ClientesP { get; set; }

    }

    public class VendedorContext : DbContext
   {
       public VendedorContext() : base("name=DefaultConnection")
       {
            Database.SetInitializer<VendedorContext>(null);
        }
        public DbSet<Vendedor> Vendedores { get; set; }
        public DbSet<c_TipoCuota> c_TipoCuota { get; set; }
        public DbSet<c_Moneda> c_Monedas { get; set; }
        public DbSet<Oficina> Oficinas { get; set; }
        public DbSet<c_PeriodicidadPago> c_PeriocidadPagos { get; set; }
        public DbSet<TipoVendedor> TipoVendedores { get; set; }
        public DbSet<c_TipoContrato> c_TipoContratos { get; set; }
    }

    public class VendedorRepository
    {

        public IEnumerable<SelectListItem> GetEsquema()
        {

            var lista = new List<SelectListItem>();

            var activo = new SelectListItem()
            {
                Value = "Fijo",
                Text = "Fijo"
            }; lista.Insert(0, activo);
            var inactivo = new SelectListItem()
            {
                Value = "Rentabilidad",
                Text = "Rentabilidad"
            }; lista.Insert(0, inactivo);
            var obsoleto = new SelectListItem()
            {
                Value = "Fijo/Rentabilidad",
                Text = "Fijo/Rentabilidad"
            }; lista.Insert(0, obsoleto);
            
            return new SelectList(lista, "Value", "Text");
        }
            public IEnumerable<SelectListItem> GetVendedor()

        {
            using (var context = new VendedorContext())
            {

            List<SelectListItem> lista = context.Vendedores.AsNoTracking().Where(n => n.Activo.Equals(true))
                                   .OrderBy(n => n.Nombre)

                                       .Select(n =>  new SelectListItem
                                           {

                                               Value = n.IDVendedor.ToString(),
                                               Text = n.Nombre
                                           }).ToList();
            


                    var countrytip = new SelectListItem()
                    {

                        Value = null,
                        Text = "--- Selecciona un Vendedor---"
                    };

                    lista.Insert(0, countrytip);
                    return new SelectList(lista, "Value", "Text");
               
               

                }
                
               
            }
        }
    

    }
