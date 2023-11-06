using SIAAPI.Models.Login;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SIAAPI.Models.Comercial
{
    [Table("MovComercial")]
    public class MovComercial
    {
        [Key]
        public int IDMovimiento { get; set; }
        [DisplayName("Documento Destino")]
        public string DocumentoDestino { get; set; }
        [DisplayName("No. Destino")]
        public int IDDestino { get; set; }
        [DisplayName("Cantidad")]
        public decimal Cantidad { get; set; }
        [DisplayName("Fecha")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Fecha { get; set; }
        [DisplayName("No. Origen")]
        public int IDOrigen { get; set; }
        [DisplayName("Documento Origen")]
        public string DocumentoOrigen { get; set; }
        [DisplayName("Usuario")]
        public int UserID { get; set; }
        public virtual User User { get; set; }
        [DisplayName("No. Partida Destino")]
        public int IDDetDestino { get; set; }
        [DisplayName("No. Partida Origen")]
        public int IDDetOrigen { get; set; }


    }
    [Table("MovAutorizacion")]
    public class MovAutorizacion {
        [Key]
        public int IDMovimientoA { get; set; }
        [DisplayName("Documento")]
        public string Documento { get; set; }
        [DisplayName("No. Documento")]
        public int IDDocumento { get; set; }
        [DisplayName("Fecha")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Fecha { get; set; }
        [DisplayName("Usuario")]
        public int UserID { get; set; }
        public virtual User User { get; set; }
    }

    public class MovimientosContext : DbContext
    {

        public MovimientosContext() : base("name=DefaultConnection")
        {
            Database.SetInitializer<MovimientosContext>(null);
        }
        public DbSet<MovComercial> MovComerciales { get; set; }
        public DbSet<MovAutorizacion> MovAutorizaciones { get; set; }
        public DbSet<User> Users { get; set; }
    }
}