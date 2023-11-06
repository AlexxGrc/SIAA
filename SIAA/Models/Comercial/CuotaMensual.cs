using SIAAPI.Models.Administracion;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SIAAPI.Models.Comercial
{
    [Table("CuotaMensual")]
    public partial class CuotaMensual
    {
        [Key]
        public int IDCuotaMensual { get; set; }

        [Required(ErrorMessage = "Mes obligatorio")]
        [DisplayName("Mes")]
        public int IDMes { get; set; }
        public virtual ICollection<c_Meses> c_Meses { get; set; }

        [Required(ErrorMessage = "Año obligatorio")]
        [DisplayName("Año")]
        public int Ano { get; set; }

        [Required(ErrorMessage = "Cuota obligatorio")]
        [DisplayName("Cuota")]
        public decimal Cuota { get; set; }

        [Required(ErrorMessage = "Porcentaje de comisión obligatorio")]
        [DisplayName("Porcentaje de comisión")]
        public decimal PorcComision { get; set; }

        [Required(ErrorMessage = "Moneda obligatorio")]
        [DisplayName("Moneda")]
        public int IDMoneda { get; set; }

        public virtual ICollection<c_Moneda> Moneda { get; set; }
        [DisplayName("Vendedor")]

        public int IDVendedor { get; set; }   
        public virtual ICollection<Vendedor> Vendedor { get; set; }
    }

    public class CuotaMensualContext : DbContext
    {
        public CuotaMensualContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<CuotaMensual> CuotaMensual { get; set; }
        public DbSet<c_Moneda> c_Monedas { get; set; }
        public DbSet<Vendedor> Vendedores { get; set; }
        public DbSet<c_Meses> c_Meses { get; set; }
    }

    //public partial class Meses
    //{
    //    [Key]
    //    public int IDMes { get; set; }

    //    [Required(ErrorMessage = "Mes")]
    //    [DisplayName("Mes")]
    //    public string Mes { get; set; }
    //}

    public class MesesContext : DbContext
    {
        public MesesContext() : base("name=DefaultConnection")
        {

        }
        //public DbSet<Meses> Meses { get; set; }
    }

    }
    


