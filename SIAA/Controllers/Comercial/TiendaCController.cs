using PagedList;
using SIAAPI.Models.Comercial;
using SIAAPI.Models.Login;
using SIAAPI.ViewModels.Articulo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Data;
using System.Data.Entity;
using SIAAPI.ViewModels.Comercial;

namespace SIAAPI.Controllers.Comercial
{
    public class TiendaCController : Controller
    {
        // GET: Tienda
        public ViewResult TiendaGeneralC(string sortOrder, string currentFilter, string searchString, int? page)
        {

            ViewBag.SearchString = searchString;

            //Paginación
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;
            //Paginación

            VPArticuloContext va = new VPArticuloContext();

            var elementos = from s in va.VPArticulos
                            select s;

            if (!string.IsNullOrEmpty(searchString))
            {
                elementos = elementos.Where(s => (s.Descripcion.Contains(searchString) || s.Cref.Contains(searchString))).OrderBy(s => s.Cref);

            }

            //Ordenacion


            //Paginación
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = va.VPArticulos.OrderBy(e => e.IDArticulo).Count(); // Total number of elements

            // return View(elementos.ToPagedList(pageNumber, pageSize));
            return View(elementos.OrderBy(i => i.Descripcion).ToPagedList(page ?? 1, 12));
            //Paginación
            //return View(elementos.ToList());
        }


        public JsonResult GetJsonPresentaciones(int id)

        {
            try
            {
                ArticuloContext db = new ArticuloContext();
                var lista = db.Database.SqlQuery<Caracteristica>("select * from Caracteristica where id=" + id).ToList();
                return Json(new { Results = lista }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }
        }


        public ActionResult GetPresentacionesTiendaC(int id, string SearchString = "")

        {
            ViewBag.SearchString = SearchString;
            try
            {
                VPArticuloContext db = new VPArticuloContext();
                var x = db.VPArticulos.Find(id);
                ViewBag.Descripcion = x.Descripcion;
                ViewBag.Cref = x.Cref;
                ViewBag.IDArticulo = x.IDArticulo;
                var lista = db.Database.SqlQuery<Caracteristica>("select * from Caracteristica where Articulo_IDArticulo=" + id + " and obsoleto='0'").ToList();
                return View(lista);
            }
            catch (Exception err)
            {
                throw;
            }
        }

        public ActionResult AddCarritoGeneral(FormCollection coleccion, string searchString = "")

        {
            ViewBag.searchString = searchString;
            decimal cantidad = decimal.Parse(coleccion.Get("Cantidad").ToString());
            int id = int.Parse(coleccion.Get("id").ToString());

            ArticuloContext db = new ArticuloContext();

            Caracteristica c = db.Database.SqlQuery<Caracteristica>("select * from Caracteristica where ID=" + id).ToList()[0];
            VCambio cantidadm = db.Database.SqlQuery<VCambio>("select Articulo.MinimoCompra as TC from Caracteristica inner join Articulo on Articulo.IDArticulo=Caracteristica.Articulo_IDArticulo where Caracteristica.ID=" + id).ToList()[0];
            // decimal cantidad= cantidadm.TC;
            Articulo articulo = db.Articulo.Find(c.Articulo_IDArticulo);

            string fecha = DateTime.Now.ToString("yyyy/MM/dd");
            if (cantidad == 0)
            {
                cantidad = 1;
            }
            List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            int usuario = userid.Select(s => s.UserID).FirstOrDefault();
            // CarritoContext carrito = new CarritoContext();
            //    var consulta = carrito.Carritos.Where(a => a.IDCaracteristica.Equals(id)).FirstOrDefault();
            List<CarritoC> numero;
            numero = db.Database.SqlQuery<CarritoC>("SELECT * FROM [dbo].[CarritoC] where IDCaracteristica='" + id + "' and usuario='" + usuario + "'").ToList();
            int consulta = numero.Select(s => s.IDCaracteristica).FirstOrDefault();


            if (consulta != 0)
            {


            }
            else
            {


                string cadena = "";
                ClsDatoEntero contarprov = db.Database.SqlQuery<ClsDatoEntero>("select count(IDCarrito) as Dato from CarritoC where usuario='" + usuario + "'").ToList()[0];

                if (contarprov.Dato != 0)
                {
                    ClsDatoEntero proveedorc = new CobroContext().Database.SqlQuery<ClsDatoEntero>("select distinct idproveedor as Dato from CarritoC where usuario= " + usuario + " group by [IDProveedor]").FirstOrDefault();
                    if (proveedorc.Dato != 0)
                    {

                        Proveedor provee = new ProveedorContext().Proveedores.Find(proveedorc.Dato);
                        VCambio cambio = db.Database.SqlQuery<VCambio>("select dbo.GetTipocambio('" + fecha + "'," + articulo.IDMoneda + "," + articulo.IDMoneda + ") as TC").ToList().FirstOrDefault();


                        string ob = "select Observacion as Dato from MatrizPrecioProv where idProveedor=" + provee.IDProveedor + " and idarticulo=" + c.Articulo_IDArticulo;
                        ClsDatoString Observacion = db.Database.SqlQuery<ClsDatoString>(ob).ToList().FirstOrDefault();


                        // cadena = "insert into carritoC (usuario, IDCaracteristica, Cantidad, Precio, Nota, IDProveedor,IDMoneda) values ('" + usuario + "'," + id + "," + cantidad + ",dbo.GetCosto(0," + c.Articulo_IDArticulo + "," + cantidad + "),'" + Observacion.Dato + "'0,'" + articulo.IDMoneda + "')";

                        string nota = string.Empty;
                        try
                        {
                            nota = Observacion.Dato;
                        }
                        catch (Exception ERR)
                        {

                        }


                        cadena = "insert into carritoC (usuario, IDCaracteristica, Cantidad, Precio, Nota, IDProveedor,IDMoneda) values ('" + usuario + "'," + id + "," + cantidad + ",dbo.GetCosto(0," + c.Articulo_IDArticulo + "," + cantidad + "),'" + nota + "',0,'" + articulo.IDMoneda + "')";

                    }
                    else
                    {
                        cadena = "insert into carritoC (usuario, IDCaracteristica, Cantidad, Precio,IDProveedor,IDMoneda) values ('" + usuario + "'," + id + "," + cantidad + ",dbo.GetCosto(0," + c.Articulo_IDArticulo + "," + cantidad + "),0,'" + articulo.IDMoneda + "')";
                    }

                }
                else
                {
                    //try{
                    //    ClsDatoEntero proveedorc = new CobroContext().Database.SqlQuery<ClsDatoEntero>("select distinct [IDProveedor] as Dato from CarritoC where usuario= " + usuario + " group by [IDProveedor]").FirstOrDefault();
                    //    Proveedor provee = new ProveedorContext().Proveedores.Find(proveedorc.Dato);
                    //    string ob = "select Observacion as Dato from MatrizPrecioProv where idProveedor=" + provee.IDProveedor + " and idarticulo=" + c.Articulo_IDArticulo;
                    //    ClsDatoString Observacion = db.Database.SqlQuery<ClsDatoString>(ob).ToList().FirstOrDefault();

                    //    ViewBag.nota = Observacion.Dato;
                    //    cadena = "insert into carritoC (usuario, IDCaracteristica, Cantidad, Precio, Nota, IDProveedor,IDMoneda) values ('" + usuario + "'," + id + "," + cantidad + ",dbo.GetCosto(0," + c.Articulo_IDArticulo + "," + cantidad + "),'"+ Observacion.Dato + "'0,'" + articulo.IDMoneda + "')";

                    //}
                    //catch (Exception err)
                    //{
                    cadena = "insert into carritoC (usuario, IDCaracteristica, Cantidad, Precio, IDProveedor,IDMoneda) values ('" + usuario + "'," + id + "," + cantidad + ",dbo.GetCosto(0," + c.Articulo_IDArticulo + "," + cantidad + "),0,'" + articulo.IDMoneda + "')";

                    //}

                }





                db.Database.ExecuteSqlCommand(cadena);
            }

            return RedirectToAction("TiendaGeneralC", new { searchString = ViewBag.searchString });


        }



        public ActionResult AgregarPresentacion3(int id)
        {
            ArticuloContext dba = new ArticuloContext();
            Articulo arti = dba.Articulo.Find(id);
            Familia fami = new FamiliaContext().Familias.Find(arti.IDFamilia);
            var atributos = new AtributodeFamiliaContext().Database.SqlQuery<AtributodeFamilia>("Select * from AtributodeFamilia where idfamilia=" + arti.IDFamilia).ToList();
            ViewBag.nombrearticulo = arti.Descripcion;
            ViewBag.Cref = arti.Cref;
            ViewBag.IDArticulo = arti.IDArticulo;
            return View(atributos);
        }

        [HttpPost]

        public ActionResult AgregarPresentacion4(int id, string Presentacion)
        {
            Console.Write("Pase aqui");
            VCaracteristicaContext dbar = new VCaracteristicaContext();


            if (Presentacion.Substring((Presentacion.Length - 1), 1) == ",")
            {
                Presentacion = Presentacion.Substring(0, Presentacion.Length - 1);
            }

            string jpresentacion = String.IsNullOrEmpty(Presentacion) == true ? "{}" : "{" + Presentacion + "}";


            int NewIDP = dbar.Database.SqlQuery<int>("SELECT ISNULL(MAX(IDPRESENTACION)+1,0) from Caracteristica where Articulo_IDArticulo = " + id + " ").FirstOrDefault();
            //int NewIDP = Convert.ToInt32(db.Database.SqlQuery("Select MAX(IDPresentacion)+1 from Caracteristica where Articulo_IDArticulo = ", pre.Articulo_IDArticulo));

            if (NewIDP == 0)
            {
                NewIDP = 1;
            }

            dbar.Database.ExecuteSqlCommand("insert into Caracteristica ( IDPresentacion, Cotizacion, version, Presentacion, jsonPresentacion, Articulo_IDArticulo )  values (" + NewIDP + ",0,0,'" + Presentacion + "','" + jpresentacion + "'," + id + ")");


            return Json(true);

            // return RedirectToAction("GetPresentacionesTiendaC", "TiendaC", new { id = id });
        }

        public ActionResult TiendaArticulos()
        {

            return View();
        }

        public ActionResult TiendaCliente()
        {

            return View();
        }

        public ActionResult TiendaVendedor()
        {

            return View();
        }



    }
}