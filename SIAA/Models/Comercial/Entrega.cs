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
    //Tabla Entrega
    [Table("Entrega")]
    public class Entrega
    {
        [Key]
        public int IDEntrega { get; set; }
        [DisplayName("Cliente")]
        public int IDCliente { get; set; }
        public virtual Clientes Clientes { get; set; }
        [DisplayName("Calle")]
        [Required]
        [StringLength(100)]
        public string CalleEntrega { get; set; }
        [DisplayName("No. Exterior")]
        [StringLength(40)]
        public string NumExtEntrega { get; set; }
        [DisplayName("No. Interior")]
        [StringLength(40)]
        public string NumIntentrega { get; set; }
        [DisplayName("Colonia")]
        [Required]
        [StringLength(100)]
        public string ColoniaEntrega { get; set; }
        [DisplayName("Municipio")]
        [Required]
        [StringLength(100)]
        public string MunicipioEntrega { get; set; }
        [DisplayName("Estado")]
        [Required]
        public int IDEstado { get; set; }
        public virtual Estados Estados { get; set; }
        [DisplayName("C.P.")]
        [Required]
        [StringLength(5, MinimumLength = 5, ErrorMessage = "El campo C.P. debe contener 5 dígitos")]
        public string CPEntrega { get; set; }
        [DisplayName("Observación")]
        [StringLength(250)]
        public string ObservacionEntrega { get; set; }
        [DisplayName("Lunes")]
        public bool DiaEntLu { get; set; }

        [DisplayName("Martes")]
        public bool DiaEntMa { get; set; }

        [DisplayName("Miércoles")]
        public bool DiaEntMi { get; set; }

        [DisplayName("Jueves")]
        public bool DiaEntJu { get; set; }

        [DisplayName("Viernes")]
        public bool DiaEntVi { get; set; }
        [DisplayName("Horario")]
        [StringLength(100)]
        public string HorarioEnt { get; set; }
        [DisplayName("Requisitos de Entrega")]
        [StringLength(250)]
        public string RequisitosEntrega { get; set; }
        [DisplayName("Requisitos de Empaque")]
        [StringLength(250)]
        public string RequisitosEmpaque { get; set; }

        /////AGREGADOS
        ///
        public int IDPais { get; set; }
        [DisplayName("Clave Pais")]
        public string c_Pais { get; set; }

        [DisplayName("Clave Estado")]
        public string c_Estado { get; set; }
        [DisplayName("Municipio")]
        public int IDMunicipio { get; set; }
        [DisplayName("Clave Estado")]
        public string c_Municipio { get; set; }
        [DisplayName("Localidad")]
        public int IDLocalidad { get; set; }
        [DisplayName("Clave Localidad")]
        public string c_Localidad { get; set; }
        [DisplayName("Colonia")]
        public int IDColonia { get; set; }
        [DisplayName("Clave Colonia")]
        public string c_Colonia { get; set; }
        [DisplayName("Referencia")]
        public string Referencia { get; set; }

    }

    [Table("VEntrega")]
    public class VEntregaC
    {
        [Key]
        public int IDDomicilioEntrega { get; set; }
        public int IDCliente { get; set; }
        public string NumExt { get; set; }
        public string Calle { get; set; }
        public string Estado { get; set; }
        public string Pais { get; set; }
        public string Localidad { get; set; }
        public string Municipio { get; set; }
    }

    public class EntregaContext : DbContext
    {
        public EntregaContext() : base("name=DefaultConnection")
        {
          
        }
        public DbSet<Entrega> Entregas { get; set; }
        public DbSet<Clientes> Clientess { get; set; }
        public DbSet<Estados> Estados { get; set; }
        //public System.Data.Entity.DbSet<SIAAPI.Models.Comercial.Clientes> Clientes { get; set; }

        //public System.Data.Entity.DbSet<SIAAPI.Models.Comercial.Estados> Estados { get; set; }
    }

    public class ReporitoryEntrega
    {
        public IEnumerable<SelectListItem> GetEntregaCliente(int IDCliente)
        {
            List<SelectListItem> lista;

            using (var context = new EntregaContext())
            {
                string cadenasql = "Select * from [dbo].[VEntrega]";

                lista = context.Database.SqlQuery<VEntregaC>(cadenasql).ToList().OrderBy(n => n.IDDomicilioEntrega).Select(n => new SelectListItem
                {
                    Value = n.IDDomicilioEntrega.ToString(),
                    Text = n.Estado + " " + n.Municipio + " " + n.NumExt
                }).ToList();
                var countrytip = new SelectListItem()
                {
                    Value = null,
                    Text = "Seleccione un domicilio de entrega"
                };
                lista.Insert(0, countrytip);
                return new SelectList(lista, "Value", "Text");
            }

        }

    }
}