namespace SIAAPI.Models.Comercial
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity;
    using System.Data.Entity.Spatial;
    using System.Linq;
    using System.Web.Mvc;

    [Table("AtributodeFamilia")]
    public partial class AtributodeFamilia
    {
        [Key]
        [Column(Order = 0)]
        public int IDAtributo { get; set; }



        public int IDFamilia { get; set; }
        [DisplayName("Descripción")]
        [Required]
       
        public string Descripcion { get; set; }

        [Required]
        [StringLength(50)]
        public string Tipo { get; set; }

        [StringLength(200)]
        public string Valores { get; set; }
        [DisplayName("Título")]
        [Required]
        public string Titulo { get; set; }

        public bool Requerido { get; set; }

        [DisplayName("Longitud Mínima")]
        public int LongitudMin { get; set; }
        [DisplayName("Longitud Máxima")]
        public int LongitudMax { get; set; }

        public string Ayuda { get; set; }

    }

    public class AtributodeFamiliaContext : DbContext
    {
        public AtributodeFamiliaContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<AtributodeFamilia> AtributodeFamilias { get; set; }


    }


    public class AtributoFamiliaRepository
    {
        public IEnumerable<SelectListItem> GetTipo()
        {

            List<SelectListItem> lista = new List<SelectListItem>();

            var countrytip = new SelectListItem()
            {
                Value = "CADENA",
                Text = "CADENA"
            };
            var countrytip2 = new SelectListItem()
            {
                Value = "NUMERO",
                Text = "NUMERO"
            };
            var countrytip3 = new SelectListItem()
            {
                Value = "LISTA",
                Text = "LISTA"
            };

            var countrytip4 = new SelectListItem()
            {
                Value = "COLOR",
                Text = "COLOR"
            };

            var countrytip5 = new SelectListItem()
            {
                Value = "checkbox",
                Text = "checkbox"
            };
            //var countrytip4 = new SelectListItem()
            //{
            //    Value = "RADIO LISTA",
            //    Text = "RADIO LISTA"
            //};
            lista.Insert(0, countrytip5);
            lista.Insert(0, countrytip4);
            lista.Insert(0, countrytip3);
            lista.Insert(0, countrytip2);
            lista.Insert(0, countrytip);
            return new SelectList(lista, "Value", "Text");

        }
    }

   
}
