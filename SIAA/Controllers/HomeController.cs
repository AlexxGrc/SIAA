using SIAAPI.Models.Comercial;
using SIAAPI.Models.Produccion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SIAAPI.Controllers
{
    public class HomeController : Controller
    {
        [AllowAnonymous] //This is for Un-Authorize User
        public ActionResult Index()
        {





            return View();
        }

        private InventarioAlmacenContext db = new InventarioAlmacenContext();
        public void Ejecutar()
        {
            List<InventarioAlmacen> inventarios = new InventarioAlmacenContext().Database.SqlQuery<InventarioAlmacen>("select*from inventarioalmacen where idalmacen=6").ToList();
            foreach (InventarioAlmacen almacen in inventarios)
            {
                decimal existencia = 0M;

                try
                {
                    string consulta = "select sum(Metrosdisponibles) as dato  from Clslotemp where idcaracteristica=" + almacen.IDCaracteristica + " and idalmacen=" + almacen.IDAlmacen;
                    ClsDatoDecimal datoDecimal = new ArticuloContext().Database.SqlQuery<ClsDatoDecimal>(consulta).FirstOrDefault();
                    existencia = datoDecimal.Dato;
                }
                catch (Exception err)
                {

                }
                db.Database.ExecuteSqlCommand("update inventarioalmacen set existencia=" + existencia + " where idalmacen=" + almacen.IDAlmacen + " and idcaracteristica=" + almacen.IDCaracteristica);
            }
            List<InventarioAlmacen> inventarios1 = new InventarioAlmacenContext().Database.SqlQuery<InventarioAlmacen>("select*from inventarioalmacen where idalmacen=11").ToList();
            foreach (InventarioAlmacen almacen in inventarios1)
            {
                decimal existencia = 0M;

                try
                {
                    ClsDatoDecimal datoDecimal = new ArticuloContext().Database.SqlQuery<ClsDatoDecimal>("select sum(Metrosdisponibles) as dato  from Clslotemp where idcaracteristica=" + almacen.IDCaracteristica + " and idalmacen=" + almacen.IDAlmacen).FirstOrDefault();
                    existencia = datoDecimal.Dato;
                }
                catch (Exception err)
                {

                }
                db.Database.ExecuteSqlCommand("update inventarioalmacen set existencia=" + existencia + " where idalmacen=" + almacen.IDAlmacen + " and idcaracteristica=" + almacen.IDCaracteristica);
            }

            db.Database.ExecuteSqlCommand("update inventarioalmacen set porllegar=0 where porllegar<0");
            db.Database.ExecuteSqlCommand("update inventarioalmacen set porllegar=0 where porllegar<0");
            db.Database.ExecuteSqlCommand("update inventarioalmacen set apartado=0 where apartado<0");
            db.Database.ExecuteSqlCommand("update inventarioalmacen set existencia=0 where existencia<0.09");
            db.Database.ExecuteSqlCommand("update inventarioalmacen set disponibilidad=(Existencia-Apartado)");

        }
        


        public ActionResult MyProfile()
        {
            
            //  return View("Index", "_Layout");
            return View();
        }
        public ActionResult Contact()
        {
            return View("Index", "_Layout");
        }
        public ActionResult About()
        {
            return View("Index", "_Layout");
        }
        //[Authorize(Roles="Admin")]
        //public ActionResult AdminIndex()
        //{

        // return View("Index","_Layout");
        //}

        //[Authorize(Roles= "Admin,User")]
        //public ActionResult UserIndex()
        //{
        //    return View("Index", "_Layout");
        //}
        public ActionResult RenderImage(int id)
        {
            EmpresaContext db = new EmpresaContext();
            Empresa item = db.empresas.Find(id);

            byte[] photoBack = item.Logo;

            return File(photoBack, "image/png");
        }



    }

    public class graficaventas
    {
        public string Mes { get; set; }
        public decimal Monto { get; set; }
    }
}
