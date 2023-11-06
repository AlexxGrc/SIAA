using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.Data.Entity;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Linq;
using SIAAPI.Models.Comercial;

namespace SIAAPI.Models.Administracion
{

    [Table("c_Banco")]
    public class c_Banco
    {
        [Key]
        public int IDBanco { get; set; }


        [Required(ErrorMessage = "La clave del SAT es obligatoria")]
        [DisplayName("Código de Sat")]
        [StringLength(3)]
        public string ClaveBanco { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [DisplayName("Nombre del banco")]
        [StringLength(30)]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "La Razón Social es obligatoria")]
        [DisplayName("Razón social")]
        [StringLength(200)]
        public string RazonSocial { get; set; }
        [Required(ErrorMessage = "El RFC es obligatorio")]
        [DisplayName("RFC del banco")]
        [StringLength(13)]
        public string RFC { get; set; }
        public virtual ICollection<BancoEmpresa> BancoEmpresa { get; set; }
    }
    public class c_BancoContext : DbContext
    {
        public c_BancoContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<c_Banco> c_Bancos { get; set; }
    }


    public class BancoRepository
    {
        public IEnumerable<SelectListItem> GetBanco()
        {
            using (var context = new c_BancoContext())
            {
                List<SelectListItem> lista = context.c_Bancos.AsNoTracking()
                    .OrderBy(n => n.Nombre)
                        .Select(n =>
                        new SelectListItem
                        {
                            Value = n.IDBanco.ToString(),
                            Text = n.ClaveBanco + " | " + n.Nombre
                        }).ToList();
                var countrytip = new SelectListItem()
                {
                    Value = null,
                    Text = "--- Selecciona un Banco---"
                };
                lista.Insert(0, countrytip);
                return new SelectList(lista, "Value", "Text");
            }

        }

        public IEnumerable<SelectListItem> GetBancoporcliente(string nombre)
        {
            using (var context = new c_BancoContext())
            {
                string cadenasql = "select B.IDBanco, (B.Nombre + ' ' + BC.CuentaBanco + ' '+M.Descripcion ) as Banco from (dbo.Clientes C inner join(dbo.BancoCliente as BC inner join c_Banco as B on BC.IDBanco = B.IDBanco) on BC.IDCliente = C.IDCliente) inner join c_Moneda as M on BC.IDMoneda = M.IDMoneda where C.Nombre = '" + nombre + "' order by B.Nombre";


                List<SelectListItem> lista = context.Database.SqlQuery<VBcoCliente>(cadenasql).ToList()
                    .OrderBy(n => n.Banco)
                        .Select(n =>
                        new SelectListItem
                        {
                            Value = n.Banco,
                            Text = n.Banco
                        }).ToList();
                var countrytip = new SelectListItem()
                {
                    Value = null,
                    Text = "--- Selecciona un Banco Registrado cliente---"
                };
                lista.Insert(0, countrytip);
                return new SelectList(lista, "Value", "Text");
            }
        }
      


        public string getNombreBanco(int _id)

        {
            c_Banco elemento = new c_BancoContext().c_Bancos.Single(m => m.IDBanco == _id);
            return elemento.ClaveBanco + " " + elemento.Nombre;

        }


    }


    public class VBcoCliente
    {
    public int IDBanco { get; set; }
    public string Banco { get; set; }

    }
 
}
