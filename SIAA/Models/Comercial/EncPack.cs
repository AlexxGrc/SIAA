using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.ComponentModel;

namespace SIAAPI.Models.Comercial
{
    [Table("Encpack")]

    public class EncPack
    {
        [Key]
        public   int idencpack { get; set; }

        public int idPedido { get; set; }

        public int Version { get; set; }

        public DateTime Fecha { get; set; }

        public string Cliente { get; set; }

        public string status { get; set; }
        public string FechaEmpacada { get; set; }

        [DisplayName("Observación")]
        public string observa { get; set; }

        public string chofer { get; set; }

        public int cajas { get; set; }

        public string Repartidor { get; set; }

        public int Idusuario { get; set; }

        DateTime? FechaP { get; set; }

        DateTime? Fechaemp { get; set; }

        public int Paquetes { get; set; }


    }

    [Table("detpack")]
    public class detpack
    {
        [Key]
        public int iddetpack { get; set; }

        public int iddetpedido { get; set; }

        public int Version { get; set; }

        public string Cref { get; set; }
        public int NP { get; set; }
        public string Descripcion { get; set; }

        public decimal Cantidad { get; set; }

        public string  Lote { get; set; }

        public string LoteMp { get; set; }

        public String Serie { get; set; }

        public string Pedimento { get; set; }

        public decimal CantEmp { get; set; }

        public  int IDOrden { get; set; }

        public string Observacion { get; set; }

        public string Estado { get; set; }

        public int Cajas { get; set; }

        public int Paquetes { get; set; }

        public int idEncPack { get; set; }
        public decimal Kilos { get; set; }

    }

    public class EncPackContext : DbContext
    {
        public EncPackContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<EncPack> EncPackaging { get; set; }

        public DbSet<detpack> detalles { get; set; }
    }
}