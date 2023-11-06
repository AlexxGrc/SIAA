using SIAAPI.Models.Comercial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SIAAPI.ViewModels.produccion
{
   public class ListaMaquina
    {
        public int IDArticulo { get; set; }

        public string Descripcion { get; set; }
    }

    public class MaquinaRepository
    {
        public IEnumerable<SelectListItem> GetMaquinaByProceso(int  Proceso)
        {
            using (var context = new ArticuloContext())
            {
                string cadenasql = " select Articulo.IDArticulo,  Articulo.Descripcion from Articulo inner join MaquinaProceso on Articulo.IDArticulo=MAquinaProceso.IDArticulo where MaquinaProceso.IDProceso=" + Proceso + " and Articulo.IDTipoArticulo=3  order by Articulo.Descripcion";


                List<SelectListItem> lista = context.Database.SqlQuery<ListaMaquina>(cadenasql).ToList()
                    .OrderBy(n => n.Descripcion)
                        .Select(n =>
                        new SelectListItem
                        {
                            Value = n.IDArticulo.ToString(),
                            Text = n.Descripcion
                        }).ToList();
                //var countrytip = new SelectListItem()
                //{
                //    Value = null,
                //    Text = "--- Selecciona ---"
                //};
                //lista.Insert(0, countrytip);
                return new SelectList(lista, "Value", "Text");
            }
        }


    }

}