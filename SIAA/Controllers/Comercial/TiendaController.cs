using PagedList;
using SIAAPI.Models.Comercial;
using SIAAPI.Models.Login;
using SIAAPI.ViewModels.Articulo;
using SIAAPI.ViewModels.Comercial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace SIAAPI.Controllers.Comercial
{
    [Authorize(Roles = "Administrador,Ventas,Sistemas,Almacenista,AdminProduccion, Produccion,Comercial, GerenteVentas, Gerencia,Compras")]
    public class TiendaController : Controller
    {
        // GET: Tienda
        public ViewResult TiendaGeneral(string sortOrder, string currentFilter, string searchString, int? page)
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

            var elementos = va.Database.SqlQuery<VPArticulo>("Select top 100 *  from VPArticulo  order by  len(descripcion),descripcion ");

            if (!string.IsNullOrEmpty(searchString))
            {
                 elementos = va.Database.SqlQuery<VPArticulo>("Select * from VPArticulo where (cref like '%" + searchString + "%' or Descripcion like '%" + searchString + "%') order by len(descripcion),descripcion");
               // elementos = elementos.Where(s => s.Descripcion.Contains(searchString));

            }
           
          
          

            //Ordenacion


            //Paginación
            int pageSize = 12;
            int pageNumber = (page ?? 1);
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = va.VPArticulos.Count(); // Total number of elements

            // return View(elementos.ToPagedList(pageNumber, pageSize));
            return View(elementos.OrderBy(i => i.Cref).ToPagedList(page ?? 1, 12));
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
            catch(Exception)
            {
                throw;
            }
        }


        public ActionResult GetPresentacionesTienda(int id, string searchString="")

        {
            ViewBag.searchString = searchString;
            try
            {
                VPArticuloContext db = new VPArticuloContext();
                var x = db.VPArticulos.Find(id);
                ViewBag.Descripcion = x.Descripcion;
                ViewBag.IDArticulo = x.IDArticulo;
                ViewBag.Cref = x.Cref;

                var lista = db.Database.SqlQuery<Caracteristica>("select * from Caracteristica where Articulo_IDArticulo=" + id + " and obsoleto='0'").ToList();
                return View(lista);
            }
            catch (Exception err)
            {
                throw;
            }
        }

        public ActionResult addCotizacion(int id)
        {
            Session["IDcaracteristica"] = id;
            Caracteristica cara = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from Caracteristica where id=" + id).FirstOrDefault();
            FormulaSiaapi.Formulas firmy = new FormulaSiaapi.Formulas();
            string ancho = firmy.getValorCadena("ANCHO",cara.Presentacion);
            string largo = firmy.getValorCadena("LARGO", cara.Presentacion);
            Articulo art = new ArticuloContext().Database.SqlQuery<Articulo>("select * from Articulo where IDArticulo=" + cara.Articulo_IDArticulo).FirstOrDefault();
            bool  descripcion = art.Descripcion.Contains("TERMOENCOGIBLE");

            if (descripcion)
            {
                return RedirectToAction("Termoencogible", "Cotizador", new {Id=0 });
            }
            else
            {
                return RedirectToAction("SuajeExistente", "Cotizador", new { eje = ancho, avance = largo });
            }
           
       
        }

        public ActionResult AddCarritoGeneral(FormCollection coleccion, string searchString="")
            
        {
            ViewBag.searchString = searchString;
            decimal Cantidad = decimal.Parse(coleccion.Get("Cantidad").ToString());
            int id = int.Parse(coleccion.Get("id").ToString());


            string fecha = DateTime.Now.ToString("yyyy/MM/dd");
            ArticuloContext db = new ArticuloContext();
            Caracteristica c = db.Database.SqlQuery<Caracteristica>("select * from Caracteristica where ID=" + id).ToList().FirstOrDefault();
            try
            {
                Articulo articulo = db.Articulo.Find(c.Articulo_IDArticulo);
                //string usuario = System.Web.HttpContext.Current.Session["SessionU"].ToString();
                List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
                int usuario = userid.Select(s => s.UserID).FirstOrDefault();

                // CarritoContext carrito = new CarritoContext();
                //    var consulta = carrito.Carritos.Where(a => a.IDCaracteristica.Equals(id)).FirstOrDefault();
               
               int  numero = db.Database.SqlQuery<Carrito>("SELECT * FROM [dbo].[Carrito] where IDCaracteristica='" + id + "' and usuario='" + usuario + "'").ToList().Count;
                int IDAlmacen = 2;
                try
                {
                    SIAAPI.Models.Comercial.ClsDatoEntero IDAlm = db.Database.SqlQuery<SIAAPI.Models.Comercial.ClsDatoEntero>("select min(idAlmacen) as Dato from Almacen where idalmacen=(select min(a.idalmacen) from Almacen as a inner join FamAlm as f on f.idalmacen=a.idalmacen where f.idfamilia=" + articulo.IDFamilia + ")").ToList().FirstOrDefault();

                    if (IDAlm != null)
                    {
                        IDAlmacen = IDAlm.Dato;
                    }
                }
                catch (Exception err)
                {

                }

                if (numero != 0)
                {
                    ViewBag.errorusuario = "INTENTASTE METER DOS VECES EL MISMO ARTICULO CON LA MISMA PRESENTACION";

                }
                else
                {
                    string cadena = "";
                    ClsDatoEntero contarcliente = db.Database.SqlQuery<ClsDatoEntero>("select count(IDCarrito) as Dato from Carrito where usuario='" + usuario + "'").ToList()[0];

                    if (contarcliente.Dato != 0)
                    {
                        ClsDatoEntero cliente = db.Database.SqlQuery<ClsDatoEntero>("select distinct IDCliente as Dato from Carrito where usuario='" + usuario + "'").ToList()[0];
                       if (cliente.Dato != 0)
                        {
                       
                            Clientes clientee = new ClientesContext().Clientes.Find(cliente.Dato);
                            VCambio cambio = db.Database.SqlQuery<VCambio>("select dbo.GetTipocambio('" + fecha + "'," + articulo.IDMoneda + "," + articulo.IDMoneda + ") as TC").ToList().FirstOrDefault();
                            cadena = "insert into carrito (usuario, IDCaracteristica, Cantidad, Precio,IDCliente,IDMoneda,IDAlmacen) values ('" + usuario + "'," + id + "," + Cantidad + ",dbo.GetPrecio(" + cliente.Dato + "," + c.Articulo_IDArticulo + ",0," + Cantidad + ")*" + cambio.TC + "," + cliente.Dato + "," + articulo.IDMoneda + "," + IDAlmacen + ")";

                        }
                        else
                        {
                            cadena = "insert into carrito (usuario, IDCaracteristica, Cantidad, Precio,IDCliente,IDMoneda,IDAlmacen) values ('" + usuario + "'," + id + "," + Cantidad + ",dbo.GetPrecio(0," + c.Articulo_IDArticulo + ",0," + Cantidad + "),0,'" + articulo.IDMoneda + "'," + IDAlmacen + ")";
                        }

                    }
                    else
                    {
                        cadena = "insert into carrito (usuario, IDCaracteristica, Cantidad, Precio,IDCliente,IDMoneda,IDAlmacen) values ('" + usuario + "'," + id + "," + Cantidad + ",dbo.GetPrecio(0," + c.Articulo_IDArticulo + ",0," + Cantidad + "),0,'" + articulo.IDMoneda + "'," + IDAlmacen + ")";

                    }

                    db.Database.ExecuteSqlCommand(cadena);
                }

                return RedirectToAction("TiendaGeneral", new { searchString = ViewBag.searchString });
            }
            catch (Exception err)
            {
                string mensaje = err.Message;
                return RedirectToAction("TiendaGeneral", new { searchString = ViewBag.searchString });
            }
        }



       
        
        public ActionResult AgregarPresentacion2(int id )
        {
            ArticuloContext dba = new ArticuloContext();
            Articulo arti = dba.Articulo.Find(id);
            Familia fami = new FamiliaContext().Familias.Find(arti.IDFamilia);
            var atributos = new AtributodeFamiliaContext().Database.SqlQuery<AtributodeFamilia>("Select * from AtributodeFamilia where idfamilia=" +arti.IDFamilia ).ToList();
            ViewBag.nombrearticulo = arti.Descripcion;
            ViewBag.Cref = arti.Cref;
            ViewBag.IDArticulo = arti.IDArticulo;
            return View(atributos);
        }

        [HttpPost]
      
        public ActionResult AgregarPresentacion2(int id, string Presentacion)
        {
            Console.Write("Pase aqui");
            VCaracteristicaContext dbar = new VCaracteristicaContext();

           
            if (Presentacion.Substring((Presentacion.Length-1),1)==",")
            {
                Presentacion = Presentacion.Substring(0, Presentacion.Length-1);
            }

            string jpresentacion = String.IsNullOrEmpty(Presentacion) == true ? "{}" : "{" + Presentacion +"}";

           
                int NewIDP = dbar.Database.SqlQuery<int>("SELECT ISNULL(MAX(IDPRESENTACION)+1,0) from Caracteristica where Articulo_IDArticulo = " + id + " ").FirstOrDefault();
                //int NewIDP = Convert.ToInt32(db.Database.SqlQuery("Select MAX(IDPresentacion)+1 from Caracteristica where Articulo_IDArticulo = ", pre.Articulo_IDArticulo));

                if (NewIDP==0)
                {
                    NewIDP = 1;
                }
           
                dbar.Database.ExecuteSqlCommand("insert into Caracteristica ( IDPresentacion, Cotizacion, version, Presentacion, jsonPresentacion, Articulo_IDArticulo )  values (" + NewIDP + ",0,0,'" + Presentacion + "','" + jpresentacion + "'," + id + ")");


            List<SIAAPI.Models.Administracion.Materiales> materiales = new List<SIAAPI.Models.Administracion.Materiales>();
            string cadenaM = "Select* from Materiales where Obsoleto <> '1'";
            materiales = new ArticuloContext().Database.SqlQuery<SIAAPI.Models.Administracion.Materiales>(cadenaM).ToList();
            List<SelectListItem> listaArticulo = new List<SelectListItem>();
            listaArticulo.Add(new SelectListItem { Text = "--Todos los Materiales--", Value = "0" });

            foreach (var m in materiales)
            {
                listaArticulo.Add(new SelectListItem { Text = m.Clave, Value = m.ID.ToString() });
            }

            ViewBag.Materiales = listaArticulo;

            return RedirectToAction("GetPresentacionesTienda", "Tienda", new { id = id });
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