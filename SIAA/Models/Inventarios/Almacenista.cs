using SIAAPI.Models.Administracion;
using SIAAPI.Models.Comercial;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SIAAPI.Models.Inventarios
{
     [Table("Almacenista")]
    public class Almacenista
    {
        [Key]
        public int IDA{ get; set; }

        [Required(ErrorMessage = "RFC del Almacenista")]
        [Index("IX_RFCAlmacenista", 1, IsUnique = true)]
        [DisplayName("RFC")]
        [StringLength(15)]
        public string RFC { get; set; }

        [Required(ErrorMessage = "El nombre del Almacenista es obligatorio")]
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

        [Display(Name = "Fecha de Ingreso")]
        [DataType(DataType.Date)]
      //  [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime FechaIngreso { get; set; }

        [DisplayName("Tipo de contrato")]
        public int IDTipoContrato { get; set; }
        public virtual c_TipoContrato c_TipoContrato { get; set; }

        [DisplayName("Tipo de Jornada")]
        public int IDTipoJornada { get; set; }
        public virtual c_TipoJornada c_Tipojornada { get; set; }

        [DisplayName("PeriocidadPago")]
        public int IDPeriocidadPago { get; set; }
        public virtual c_PeriodicidadPago c_PeriocidadPago { get; set; }

        [DisplayName("Oficina")]
        public int IDOficina { get; set; }
        public virtual Oficina Oficina { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string ClaveAcceso { get; set; }

        [DisplayName("Activo")]
        public bool Activo { get; set; }

        [DisplayName("Notas")]
        [StringLength(250)]
        public string Notas { get; set; }
    }
    public class AlmacenistaContext : DbContext
    {
        public AlmacenistaContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<Almacenista> Almacenistas { get; set; }
        public DbSet<c_TipoContrato> c_TipoContratos { get; set; }
        public DbSet<c_TipoJornada> c_TipoJornadas { get; set; }
        public DbSet<c_PeriodicidadPago> c_PeriocidadPagos { get; set; }
        public DbSet<Oficina> Oficinas { get; set; }
    }

}
    