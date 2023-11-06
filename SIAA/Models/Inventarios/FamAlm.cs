using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.Mvc;


namespace SIAAPI.Models.Inventarios
{
   //Nombre de la tabla
    [Table("FamAlm")]
    public class FamAlm
    {
        //Primary Key ID
        [Key]
        public int ID { get; set; }
        public int IDFamilia{ get; set; }
        public int IDAlmacen { get; set; }
    }

    public class FamAlmContext : DbContext
    {
        public FamAlmContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<FamAlm> FamAlms { get; set; }

    }

    [Table("VFamAlm")]
    public class VFamAlm
    {
        [Key]
        public int ID { get; set; }
        public int IDFamilia { get; set; }
        [DisplayName("Código de Familia")]
        public string CCodFam { get; set; }
        [DisplayName("Familia")]
        public string Familia { get; set; }
        public int IDAlmacen { get; set; }
        [DisplayName("Código de Almacen")]
        public string CodAlm { get; set; }
        [DisplayName("Almacen")]
        public string Almacen { get; set; }
    }

    public class VFamAlmContext : DbContext
    {
        public VFamAlmContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<VFamAlm> VFamAlms { get; set; }

    }

}
