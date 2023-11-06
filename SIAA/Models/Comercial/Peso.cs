using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SIAAPI.Models.Comercial
{
    [Table("Peso")]
    public class Peso
    {
        [Key]
        public int idpeso { get; set; }

        public string IDmatpri { get; set; }

        public decimal PesoxMt { get; set; }

    }

    public class PesoContext : DbContext
    {
        public PesoContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<Peso> Pesos { get; set; }
    }
}