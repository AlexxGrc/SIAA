using SIAAPI.Models.Comercial;
using SIAAPI.Models.Produccion;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SIAAPI.Models.PlaneacionProduccion
{
    [Table("Formulas")]
    public class Formula
    {
        [Key]
        public int IDFormula {get;set;}
        public int IDProceso { get; set; }
        public int IDTipoArticulo { get; set; }

        public string FormulaP { get; set; }


        public virtual TipoArticulo TipoArticulo { get; set; }
        public virtual Proceso Proceso { get; set; }

    }
    public class FormulaContext : DbContext
    {
        public FormulaContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<Formula> Formulas { get; set; }
    }
}