using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using SIAAPI.Models.Administracion;
using SIAAPI.Models.Comercial;
using System.Web.Mvc;

namespace SIAAPI.Models.Produccion
{
    [Table("Trabajador")]
    public class Trabajador
    {

        [Key]
        public int IDTrabajador { get; set; }

        //[Required(ErrorMessage = "RFC del trabajador")]
        //[Index("IX_RFCTrabajador", 1, IsUnique = true)]
       

        [Required(ErrorMessage = "El nombre del trabajador es obligatorio")]
        [Index("IX_NomTrabajador", 2, IsUnique = true)]
        [DisplayName("Nombre")]
        [StringLength(150)]
        public string Nombre { get; set; }

        [DisplayName("E-mail")]
        [StringLength(100)]
        public string Mail { get; set; }

        [DisplayName("Telefono")]
        [StringLength(15)]
        public string Telefono { get; set; }

        [DisplayName("Foto")]
        public byte[] Photo { get; set; }

        [DisplayName("Oficina")]
        public int IDOficina { get; set; }
        public virtual Oficina Oficina { get; set; }

        [DisplayName("Activo")]
        public bool Activo { get; set; }

        [DisplayName("Notas")]
        [StringLength(250)]
        public string Notas { get; set; }
        public DateTime FechaIngreso { get; set; }

        public virtual ICollection<TrabajadorProceso> TrabajadorProcesos { get; set; }
    }

    [Table("TrabajadorProceso")]

    public class TrabajadorProceso
    {
        [Key]
        public int IDTrabajadorProceso { get; set; }

        public int IDTrabajador { get; set; }
        public virtual Trabajador Trabajador { get; set; }

        public int IDProceso { get; set; }
        public virtual Proceso Proceso { get; set; }



    }

   

    public class TrabajadorPr
    {
        [Key]
        public int IDTrabajadorProceso { get; set; }

        public int IDTrabajador { get; set; }
        public virtual Trabajador Trabajador { get; set; }

        public int IDProceso { get; set; }
        public virtual Proceso Proceso { get; set; }

        public string Nombre { get; set; }
        public string Nombreproceso { get; set; }

    }




    public class TrabajadorContext : DbContext
    {
        public TrabajadorContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<Trabajador> Trabajadores { get; set; }
        public DbSet<c_TipoContrato> c_TipoContratos { get; set; }
        public DbSet<c_TipoJornada> c_TipoJornadas { get; set; }
        public DbSet<c_PeriodicidadPago> c_PeriocidadPagos { get; set; }
        public DbSet<Oficina> Oficinas { get; set; }
        public DbSet<TrabajadorProceso> TrabajadorProcesoes { get; set; }
        public DbSet<TrabajadorPr> TrabajadorPr { get; set; }

        public DbSet<Proceso> Procesos { get; set; }
    }

    public class PRepository
    {
        public IEnumerable<SelectListItem> GetProcesoPorTrabajador(int IDTrabajador)
        {
            List<SelectListItem> lista;
            if (IDTrabajador == 0)
            {
                lista = new List<SelectListItem>();
                lista.Add(new SelectListItem() { Value = "0", Text = "Elige un trabajador" });
                return (lista);
            }
            using (var context = new TrabajadorContext())
            {
                string cadenasql = "select t.nombre, p.NombreProceso, tp.IDTrabajadorProceso, tp.idTrabajador, tp.idproceso from TrabajadorProceso as tp inner join Trabajador as t on t.idtrabajador=tp.IDTrabajador inner join Proceso as p on p.idproceso=tp.idproceso  where t.idtrabajador = " + IDTrabajador + " ";
                lista = context.Database.SqlQuery<TrabajadorPr>(cadenasql).ToList()
                    .OrderBy(n => n.IDProceso)
                        .Select(n =>
                        new SelectListItem
                         {
                             Value = n.IDProceso.ToString(),
                             Text = n.Nombreproceso
                         }).ToList();
                //lista.Add(new SelectListItem() { Value = "0", Text = "Elige un trabajador" });
                var countrytip = new SelectListItem()
                {
                    Value = null,
                    Text = "--- Selecciona un Proceso ---"
                };
                lista.Insert(0, countrytip);
                return new SelectList(lista, "Value", "Text");
            }

        }

    }


    
}

 