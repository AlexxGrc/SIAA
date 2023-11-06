using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using SIAAPI.Models.Login;
using SIAAPI.Models.Soporte;

namespace SIAAPI.Models.Soporte
{
    [Table("MensajesSoporte")]

    public class MensajesSoporte
    {

        ////////////////////
        [Key]
        [DisplayName("No. Ticket")]
        public int Id_Ticket { get; set; }
        [DisplayName("Fecha")]
        public DateTime Fecha_Hora { get; set; }
        public string Mensaje { get; set; }
        public string Rol { get; set; }
        public int UserID { get; set; }
        public virtual User Users { get; set; }
        [DisplayName("Cerrado por Soporte")]
        public bool EstadoTicket { get; set; }
        [DisplayName("Cerrado por Cliente")]
        public bool CerradoPorCliente { get; set; }
        [DisplayName("Error generado por")]
        public string ErrorGeneradoPor { get; set; }
        public string TiempoEstimado { get; set; }
        
    }
    public class MSdbContext : DbContext
    {
        public MSdbContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<MensajesSoporte> MenSoportes { get; set; }
        public DbSet<User> Users  { get; set; }

    }


    [Table("RespuestaSoporte")]
    public class RespuestaSoporte
    {
        [Key]
        public int ID { get; set; }
        public int Id_Ticket { get; set; }
        public DateTime Fecha_Atencion { get; set; }
        public string Respuesta { get; set; }
        [DisplayName("Cerrado por Soporte")]
        public bool EstadoTicket { get; set; }
        public string Prioridad { get; set; }
        public string Rol { get; set; }
        public string UserN { get; set; }
        [DisplayName("Cerrado por Cliente")]
        public bool CerradoPorCliente { get; set; }
    }

    public class MRdbContext : DbContext
    {
        public MRdbContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<RespuestaSoporte> ResSoportes { get; set; }
        public object MenSoportes { get; internal set; }

        public System.Data.Entity.DbSet<SIAAPI.Models.Soporte.MensajesSoporte> MensajesSoportes { get; set; }

        public System.Data.Entity.DbSet<SIAAPI.Models.Login.User> Users { get; set; }
    }
}


