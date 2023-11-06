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
        [Table("VMovArticulo")]
        public class VMovArticulo
        {
            [Key]
            [DisplayName("Código del movimiento")]
            public int IDMovimiento { get; set; }
            [DisplayName("Fecha")]
            [DataType(DataType.Date)]
            [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
            public DateTime Fecha { get; set; }
            public TimeSpan Hora { get; set; }

            [DisplayName("Acción")]
            public string Accion { get; set; }
            [DisplayName("Documento")]
            public string Documento { get; set; }
            public int NoDocumento { get; set; }
            [DisplayName("Operación")]
            public string TipoOperacion { get; set; }
            [DisplayName("IDAlmacen")]
            public int IDAlmacen { get; set; }
            [DisplayName("Almacen")]
            public string Almacen { get; set; }
            [DisplayName("ID Artículo")]
            public int IDArticulo { get; set; }
            [DisplayName("ID Característica")]
            public int IDCaracteristica { get; set; }
            [DisplayName("Código")]
            public string Cref { get; set; }

            [DisplayName("Descripción")]
            public string Descripcion { get; set; }
            [DisplayName("Presentación")]
            public string Presentacion { get; set; }
            [DisplayName("Lote")]
            public string Lote { get; set; }
            [DisplayName("Cantidad")]
            public decimal Cantidad { get; set; }
            [DisplayName("Acumulado")]
            public decimal Acumulado { get; set; }
            [DisplayName("Observación")]
            public string observacion { get; set; }

        }
        public class VMovArticuloContext : DbContext
        {
            public VMovArticuloContext() : base("name=DefaultConnection")
            {

            }
            public DbSet<VMovArticulo> VMovArticulos { get; set; }
        }
    }