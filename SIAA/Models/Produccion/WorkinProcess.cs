using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SIAAPI.Models.Produccion
{
    [Table("WorkinProcess")]
    public class WorkinProcess
    {
        [Key]
        public  int IDWorkingProcess { get; set; }
       public  int  IDAlmacen { get; set; }
        public int  IDArticulo { get; set; }

        public int IDCaracteristica { get; set; }

        public decimal Cantidad { get; set; }
        public int Orden { get; set; }
        public int IDproceso { get; set; }
        public decimal Ocupado { get; set; }
        public decimal Devuelto { get; set; }
        public decimal  Merma { get; set; }
        public string  Observacion { get; set; }
        public int IDlotemp { get; set; }
        public string loteinterno { get; set; }

        public decimal ocupadolineal { get; set; }

        public int IDClsMateriaPrima { get; set; }
    }


    public class WorkinProcessContext : DbContext
    {
        public WorkinProcessContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<WorkinProcess> WIP { get; set; }
       
    }
}