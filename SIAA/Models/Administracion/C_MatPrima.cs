using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.Data.Entity;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIAAPI.Models.Administracion
{
    [Table("Peso")]
    public class Peso
    {
        [Key]
        public int idpeso { get; set; }
        [DisplayName("Clave MP")]
        public string IDmatpri { get; set; }
        [DisplayName("Peso * Mt2")]
        public decimal PesoxMt { get; set; }
    }
    public class MateriaPrimaContext : DbContext
    {
        public MateriaPrimaContext() : base("name=DefaultConnection")
        {
            Database.SetInitializer<MateriaPrimaContext>(null);
        }
        public System.Data.Entity.DbSet<SIAAPI.Models.Administracion.Peso> Pesos { get; set; }
        //public DbSet<Bitacora> Bitacoras { get; set; }      
    }
}