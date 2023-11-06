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
    [Table("TipoCambio")]
    public class TipoCambio
    {
        [Key]
        public int IDTipoCambio { get; set; }

        [Display(Name = "Fecha")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public Nullable<System.DateTime> FechaTC { get; set; }

        [Display(Name = "Moneda origen")]
        [Required(ErrorMessage = "Por favor ingrese la moneda origen")]
        
        public int IDMonedaOrigen { get; set; }
       public virtual c_Moneda c_Monedas { get; set; }

        [Display(Name = "Moneda destino")]
        [Required(ErrorMessage = "Por favor ingrese la moneda destino")]
        public int IDMonedaDestino { get; set; }
       public virtual c_Moneda c_Monedas1 { get; set; }

        [Range(0.01, 1000.00,
                    ErrorMessage = "El tipo de cambio debe estar entre 0.01 y 1000.00")]
        [Display(Name = "Tipo de cambio")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:n4}")]
        [Required(ErrorMessage = "Por favor ingrese el tipo de cambio")]
        public float TC { get; set; }

        public virtual ICollection<c_TipoCuota> c_TipoCuota { get; set; }

    }

    public class TipoCambioContext : DbContext
    {
        public TipoCambioContext() : base("name=DefaultConnection")
        {

        }
    public DbSet<TipoCambio> TipoCambios { get; set; }
}

}

