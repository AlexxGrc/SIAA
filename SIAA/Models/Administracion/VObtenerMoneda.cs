using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SIAAPI.Models.Administracion
{ 
    [Table("VObtenerMoneda")]
        public class VObtenerMoneda
    {
            [Key]
            public int IDTipoCambio { get; set; }

        [Display(Name = "Fecha")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public Nullable<System.DateTime> FechaTC { get; set; }
        
        public int IDMonedaOrigen { get; set; }

        [Display(Name = "Moneda Origen")]
        [StringLength(100)]
            public string MonedaOrigen { get; set; }
    
        public int IDMonedaDestino { get; set; }

        [Display(Name = "Moneda Destino")]
        [StringLength(100)]
       
        public string MonedaDestino { get; set; }

            [Display(Name = "Tipo de cambio")]
        //[DisplayFormat(DataFormatString = "{0:n4}")]
           // [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:n4}")]
        //[DisplayFormat(DataFormatString = "{0:C}")]
        public double TC { get; set; }

        }
        public class VObtenerMonedaContext : DbContext
        {
            public VObtenerMonedaContext() : base("name=DefaultConnection")
            {

            }
            public DbSet<VObtenerMoneda> VObtenerMonedas { get; set; }
        }

    }

