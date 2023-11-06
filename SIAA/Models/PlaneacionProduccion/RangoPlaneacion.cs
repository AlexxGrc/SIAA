using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SIAAPI.Models.PlaneacionProduccion
{
    [Table("RangoPlaneacionCosto")]
    public class RangoPlaneacionCosto
    {
        [Key]
        public int IDRP { get; set; }

        public int IDHE { get; set; }

        [Required(ErrorMessage = "Rango Inferior es Obligatorio")]
        [DisplayName("Rango inferior")]
        public decimal RangoInf {get; set;}

        [Required(ErrorMessage = "Rango Superior es Obligatorio")]
        [DisplayName("Rango superior")]
        public decimal RangoSup { get; set; }

        //[Required(ErrorMessage = "Cantidad")]
        //[DisplayName("Cantidad")]
        //public decimal Precio { get; set; }

        [DisplayName("Costo")]
        public decimal Costo { get; set; }


        [DisplayName("Version")]
        public int Version { get; set; }
    }

    [Table("RangoPlaneacionCosto")]
    public class VRangoPlaneacionCosto
    {
        [Key]
        [DisplayName("Rango inferior")]
        public decimal RangoInf { get; set; }

        [DisplayName("Rango superior")]
        public decimal RangoSup { get; set; }
        
        [DisplayName("Costo")]
        public decimal Costo { get; set; }

        [DisplayName("Tipo de Artículo")]
        public string Descripcion { get; set; }
    }

    public class VRangosPlaneacion
    {
        [Key]
        [DisplayName("Rango inferior")]
        public decimal RangoInf { get; set; }

        [DisplayName("Rango superior")]
        public decimal RangoSup { get; set; }

       
    }

    public class RangoPlaneacionContext : DbContext
    {
        public RangoPlaneacionContext() : base("name=DefaultConnection")
        {
            Database.SetInitializer<RangoPlaneacionContext>(null);
        }
        public DbSet<RangoPlaneacionCosto> Rangos { get; set; }
    }

    //public class RangoPlaneacionRepository
    //{
    //    public IEnumerable<SelectListItem> GetComboRangos(int id)
    //    {
    //        using (var context = new RangoPlaneacionContext())
    //        {
    //            List<SelectListItem> lista = context.Rangos.AsNoTracking()
    //                .OrderBy(n => n.RangoInf).Where(n => n.IDHE ==id)
    //                    .Select(n =>
    //                    new SelectListItem
    //                    {
    //                        Value = n.IDRP.ToString(),
    //                        Text = n.RangoInf+ " | " + n.RangoSup + "|" + n.Cantidad 
    //                    }).ToList();
    //            var countrytip = new SelectListItem()
    //            {
    //                Value = null,
    //                Text = "--- Selecciona un Rango---"
    //            };
    //            lista.Insert(0, countrytip);
    //            return new SelectList(lista, "Value", "Text");
    //        }

    //    }

    //    public List<RangoPlaneacionCosto> GetListaPorSpec(int id)
    //    {
    //        using (var context = new RangoPlaneacionContext())
    //        {
    //            return context.Rangos.Where(m => m.IDHE==id).ToList();
               
    //        }

    //    }

    //    public Decimal getCostoRango(int _id)

    //    {
    //        RangoPlaneacionCosto elemento = new RangoPlaneacionContext().Rangos.Find(_id);
    //        return elemento.Costo;

    //    }

    //    public void SetCostoRango(int _id, decimal costo)

    //    {
    //        try
    //        {
    //            new RangoPlaneacionContext().Database.ExecuteSqlCommand("update RangoPlaneacionCosto set costo=" + costo + " where idhr=" + _id);

    //        }
    //        catch(Exception err)
    //        {
    //            String mensaje = err.Message;
    //        }

    //    }


    }
