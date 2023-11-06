using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SIAAPI.Models.Inventarios
{
    [Table("VMovimientoAlmacen")]
    public class VMovimientoAlmacen
    {

        [Key]
        public string IDV { get; set; }
        public int IDMovimiento { get; set; }

        [Display(Name = "Fecha")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime FechaMovimiento { get; set; }

        //[Display(Name = "Hora")]
        //[DataType(DataType.Time)]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:HH:mm}")]
        //public DateTime Hora { get; set; }

        [DisplayFormat(DataFormatString = "{0:hh\\:mm}", ApplyFormatInEditMode = true)]
        public TimeSpan? Hora { get; set; }
        public string Referencia { get; set; }

        public string Producto { get; set; }

        public string Presentacion { get; set; }
        public string Lote { get; set; }
        public string Accion { get; set; }
        public string Documento { get; set; }

        [Display(Name = "No de Docto")]
        public Int32 NoDocumento { get; set; }

        [Display(Name = "Operacion")]
        public string TipoOperacion { get; set; }
        public string Almacen { get; set; }
        public decimal Cantidad { get; set; }
        public decimal Acumulado { get; set; }
        public string Observacion { get; set; }

        public int IDArticulo { get; set; }
        public int IDCaracteristica { get; set; }

        public int IDAlmacen { get; set; }

    }
    public class VMovimientoAlmacenContext : DbContext
    {
        public VMovimientoAlmacenContext() : base("name=DefaultConnection")
        {
        }
        public DbSet<VMovimientoAlmacen> VMovimientoAlmacenes { get; set; }
    }
}