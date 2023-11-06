using SIAAPI.Models.Cfdi;
using SIAAPI.Models.Comercial;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace SIAAPI.Models.Administracion
{
    /* Tabla BancoEmpresa */
    [Table("BancoEmpresa")]
    public class BancoEmpresa
    {
        [Key]
        public int IDBancoEmpresa { get; set; }

        public int IDEmpresa { get; set; }

        [Required(ErrorMessage = "El banco es obligatorio")]
        [DisplayName("Banco")]
        public int IDBanco { get; set; }
        public virtual c_Banco c_Banco { get; set; }

        [Required(ErrorMessage = "La cuenta bancaria es obligatoria")]
        [DisplayName("Cuenta Bancaria")]
        public string CuentaBanco { get; set; }
        [Required(ErrorMessage = "La Moneda es obligatoria")]
       

        [DisplayName("Cuenta Contable")]
        public string cuentaContable { get; set; }
        [DisplayName("Moneda")]
        public int IDMoneda { get; set; }

        public virtual c_Moneda c_Moneda { get; set; }
        public virtual ICollection<PagoFactura> PagoFactura { get; set; }
    }

    public class BancoEmpresaContext : DbContext
    {
        public BancoEmpresaContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<BancoEmpresa> BancoEmpresa { get; set; }
        public DbSet<c_Banco> c_Banco { get; set; }
        public DbSet<c_Moneda> c_Moneda { get; set; }
        public DbSet<VBcoEmpresa> VBcoEmpresa { get; set; }
        public DbSet<Empresa> Empresa { get; set; }
    }

    public class BancoEmpresaRepository
    {
       public BancoEmpresaContext db = new BancoEmpresaContext();
        public IEnumerable<SelectListItem> GetBancoEmpresa()
        {
                
                string cadenasql = "select * from [dbo].[VBcoEmpresa] order by Nombre;";
                List<SelectListItem> lista = db.Database.SqlQuery<VBcoEmpresa>(cadenasql).ToList()
                     .OrderBy(n => n.Nombre)
                        .Select(n =>
                        new SelectListItem
                        {
                            Value = n.IDBancoEmpresa.ToString(),
                            Text =  n.Nombre + " | " + n.RFC + " | " + n.CuentaBanco + " | " + n.ClaveMoneda + " | " + n.Descripcion
                        }).ToList();
                var countrytip = new SelectListItem()
                {
                    Value = null,
                    Text = "--- Selecciona un Banco de la empresa---"
                };
                lista.Insert(0, countrytip);
                return new SelectList(lista, "Value", "Text");
            }
        


    }

    [Table("VBcoEmpresa")]
    public class VBcoEmpresa
    {
        [Key]
        public int ID { get; set; }
        public int IDBancoEmpresa { get; set; }
        [DisplayName("Banco")]
        public string Nombre { get; set; }
        [DisplayName("Razón Social")]
        public string RazonSocial { get; set; }
        [DisplayName("RFC")]
        public string RFC { get; set; }
        [DisplayName("Cuenta Bancaria")]
        public string CuentaBanco { get; set; }
        public int IDMoneda { get; set; }
        [DisplayName("Clave Moneda")]
        public string ClaveMoneda { get; set; }
        [DisplayName("Moneda")]
        public string Descripcion { get; set; }

        [DisplayName("Cuenta Contable")]
        public string cuentaContable { get; set; }
    }
    public class VBcoEmpresaContext : DbContext
    {
        public VBcoEmpresaContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<VBcoEmpresa> VBcoEmpresas { get; set; }

    }

}