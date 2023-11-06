
using SIAAPI.Models.Comercial;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace SIAAPI.Models.Inventarios
{

    public class Clslotetintacreate
    {
        [Key]
        public int id { get; set; }

        [Display(Name = "Numero de elementos")]
        public int NoEnvases { get; set; }


        public decimal cantidad { get; set; }


        public string ccodalm { get; set; }

        public string  unidad { get; set; }

        public int iddetrecepcion { get; set; }

        public int  IDRecepcion { get; set; }
    }

    [Table("Clslotetinta")]
    public class Clslotetinta
    {
        [Key]
        public int id { get; set; }

        public int idarticulo { get; set; }

        public int idcaracteristica { get; set; }

        [DisplayName("Fecha")]
        public DateTime fecha  { get; set; }
        [DisplayName("Factura")]
        public string factura { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria")]
        [DisplayName("Cantidad")]
        public decimal cantidad { get; set; }
        [DisplayName("Unidad de Medida")]
        public string unidad { get; set; }

        [DisplayName("Envase No")]
        public int consecutivo { get; set; }

        public int IDFamilia { get; set; }

        public int IDAlmacen { get; set; }

        [DisplayName("Clave")]
        public string Cref { get; set; }

        [DisplayName("Almacen")]
        public string ccodalm { get; set; }

        [DisplayName("Recepcion")]
        public int IDRecepcion { get; set; }

        [DisplayName("Orden de compra")]
        public int OrdenCompra { get; set; }

        public int iddetrecepcion { get; set; }

        [DisplayName("Lote")]
        public string lote { get; set; }
        [DisplayName("Estado")]
        public string Estado { get; set; }
    }

    public class ClslotetintaContext : DbContext
    {
        public ClslotetintaContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<Clslotetinta> Tintas { get; set; }
       
    }


}