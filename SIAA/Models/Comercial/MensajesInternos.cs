using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Data.Entity;
using Microsoft.ApplicationInsights.Web;

namespace SIAAPI.Models.Comercial
{
    [Table("MensajesInternos")]


    public class MensajesInternos
    {
        [Key]
        public int Id_Mensaje { get; set; }
        public DateTime Fecha { get; set; }
        public string Mensaje { get; set; }
        public string Documento { get; set; }
        public string No_Documento { get; set; }
        public string Rol { get; set; }
        public bool Estado { get; set; }

    }
    public class MIdbContext : DbContext
    {
        public MIdbContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<MensajesInternos> MenPros { get; set; }

    }

}
