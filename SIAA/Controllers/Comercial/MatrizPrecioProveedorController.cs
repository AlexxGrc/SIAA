using PagedList;
using SIAAPI.Models.Administracion;
using SIAAPI.Models.Comercial;
using SIAAPI.Models.Login;
using SIAAPI.ViewModels.Comercial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SIAAPI.Controllers.Comercial
{
    [Authorize(Roles = "Administrador,Sistemas,Proveedor")]
    public class MatrizPrecioProveedorController : Controller
    {
        // GET: MatrizPrecioProveedor
        private MatrizPrecioProveedorContext db = new MatrizPrecioProveedorContext();
        // GET: Almacen
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.ASortParm = String.IsNullOrEmpty(sortOrder) ? "Proveedor" : "Proveedor";
            ViewBag.BSortParm = String.IsNullOrEmpty(sortOrder) ? "Articulo" : "Articulo";
            // Not sure here
            if (searchString == null)
            {
                searchString = currentFilter;
            }

            // Pass filtering string to view in order to maintain filtering when paging
            ViewBag.SearchString = searchString;
            //Paginación
            var elementos = from s in db.MPP
                            select s;
            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                elementos = elementos.Where(s => s.Proveedor.Empresa.Contains(searchString) || s.Articulo.Descripcion.Contains(searchString));
            }

            //Ordenacion

            switch (sortOrder)
            {
                case "Proveedor":
                    elementos = elementos.OrderBy(s => s.Proveedor.Empresa);
                    break;
                case "Articulo":
                    elementos = elementos.OrderBy(s => s.Articulo.Descripcion);
                    break;
                default:
                    elementos = elementos.OrderBy(s => s.IDMatrizPrecio);
                    break;
            }

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = db.MPP.OrderBy(e => e.IDMatrizPrecio).Count(); // Total number of elements

            // Populate DropDownList
            ViewBag.PageSize = new List<SelectListItem>()
            {
                new SelectListItem { Text = "10", Value = "10", Selected = true },
                new SelectListItem { Text = "25", Value = "25" },
                new SelectListItem { Text = "50", Value = "50" },
                new SelectListItem { Text = "100", Value = "100" },
                new SelectListItem { Text = "Todo", Value = count.ToString() }
             };

            int pageNumber = (page ?? 1);
            int pageSize = (PageSize ?? 10);
            ViewBag.psize = pageSize;

            return View(elementos.ToPagedList(pageNumber, pageSize));
            //Paginación
        }
 
        public ActionResult Details(int id)
        {
            
            MatrizPrecioProveedor matrizprecioproveedor = db.MPP.Find(id);
          
            return View(matrizprecioproveedor);
        }

       
        // GET: Almacen/Create
        public ActionResult Create()
        {
            ViewBag.IDProveedor = new SelectList(db.Proveedores.OrderBy(s => s.Empresa), "IDProveedor", "Empresa");
            ViewBag.IDArticulo = new SelectList(db.Articulo.OrderBy(s => s.Descripcion), "IDArticulo", "Descripcion");
            return View();
        }

        // POST: Almacen/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MatrizPrecioProveedor matrizprecioproveedor)
        {
            try
            {
                // TODO: Add insert logic here
                
                db.MPP.Add(matrizprecioproveedor);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.IDProveedor = new SelectList(db.Proveedores, "IDProveedor", "Empresa", matrizprecioproveedor.IDProveedor);
                ViewBag.IDArticulo = new SelectList(db.Articulo, "IDArticulo", "Descripcion", matrizprecioproveedor.IDArticulo);
                return View();
            }
        }

      
        public ActionResult Edit(int? id)
        {

            MatrizPrecioProveedor matrizprecioproveedor = db.MPP.Find(id);

            ViewBag.IDProveedor = new SelectList(db.Proveedores, "IDProveedor", "Empresa", matrizprecioproveedor.IDProveedor);
            ViewBag.IDArticulo = new SelectList(db.Articulo, "IDArticulo", "Descripcion", matrizprecioproveedor.IDArticulo);
            return View(matrizprecioproveedor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MatrizPrecioProveedor matrizprecioproveedor)
        {
            if (ModelState.IsValid)
            {
                db.Entry(matrizprecioproveedor).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IDProveedor = new SelectList(db.Proveedores, "IDProveedor", "Empresa", matrizprecioproveedor.IDProveedor);
            ViewBag.IDArticulo = new SelectList(db.Articulo, "IDArticulo", "Descripcion", matrizprecioproveedor.IDArticulo);
            return View(matrizprecioproveedor);
        }
        public ActionResult Delete(int? id)
        {

            MatrizPrecioProveedor matrizprecioproveedor = db.MPP.Find(id);

            return View(matrizprecioproveedor);
        }

        // POST: Proveedores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MatrizPrecioProveedor matrizprecioproveedor = db.MPP.Find(id);
            db.MPP.Remove(matrizprecioproveedor);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        public ActionResult Agrega(int? id)
        {
            ViewBag.id = id;
            List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            int usuario = userid.Select(s => s.UserID).FirstOrDefault();

            List<CarritoP> pedido = db.Database.SqlQuery<CarritoP>("select CarritoP.IDProveedor,Articulo.MinimoCompra,Articulo.MinimoVenta,c_ClaveUnidad.Nombre as Unidad,Carrito.IDCarrito,Carrito.usuario,Carrito.IDCaracteristica,Carrito.Precio,Carrito.Cantidad,Caracteristica.Presentacion as Presentacion,Articulo.Descripcion,Articulo.IDArticulo,c_Moneda.Descripcion as Moneda,c_Moneda.IDMoneda as IDMoneda,Carrito.Precio * Carrito.Cantidad as Importe, Carrito.Nota from  Carrito INNER JOIN Caracteristica ON Carrito.IDCaracteristica= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo INNER JOIN c_Moneda ON c_Moneda.IDMoneda = Articulo.IDMoneda INNER JOIN c_ClaveUnidad ON c_ClaveUnidad.IDClaveUnidad = Articulo.IDClaveUnidad where usuario='" + usuario + "' and IDCarrito=" + id + "").ToList();

            int idProveedor = pedido.Select(s => s.IDProveedor).FirstOrDefault();
            int idart = pedido.Select(s => s.IDArticulo).FirstOrDefault();
            decimal cantidad = pedido.Select(s => s.Cantidad).FirstOrDefault();
            int idcarac = pedido.Select(s => s.IDCaracteristica).FirstOrDefault();
            Articulo articulo = new ArticuloContext().Articulo.Find(idart);
            c_ClaveUnidad unidad = new c_ClaveUnidadContext().c_ClaveUnidades.Find(articulo.IDClaveUnidad);
            ViewBag.unidad = unidad.Nombre;
            //var elementos = db.Database.SqlQuery<MatrizPrecioProveedor>("select * from MatrizPrecioProveedor where IDArticulo =" + idart + " and IDProveedor =" + idproveedor + "").ToList();
            var elementos = from s in db.MPP
                            where s.IDArticulo.Equals(idart) && s.IDProveedor.Equals(idProveedor)
                            select s;
            try
            {


                Proveedor proveedor = new ProveedorContext().Proveedores.Find(idProveedor);
                c_Moneda moneda = new c_MonedaContext().c_Monedas.Find(proveedor.IDMoneda);
                ViewBag.moneda = moneda.Descripcion;
                ViewBag.IDProveedor = new SelectList(db.Proveedores.Where(s => s.IDProveedor.Equals(idProveedor)), "IDProveedor", "Nombre");
                ViewBag.IDArticulo = new SelectList(db.Articulo.Where(s => s.IDArticulo.Equals(idart)), "IDArticulo", "Descripcion");

                ViewBag.rangoinferior = .01;
                ViewBag.rangosuperior = 999.99;

                ClsDatoEntero countRango = db.Database.SqlQuery<ClsDatoEntero>("select count(IDMatrizPrecio) as Dato from MatrizPrecioProveedor where IDArticulo =" + idart + " and IDProveedor =" + idProveedor + "").ToList()[0];
                if (countRango.Dato != 0)
                {

                    List<MatrizPrecioProveedor> rango = db.Database.SqlQuery<MatrizPrecioProveedor>("select * from [dbo].[MatrizPrecioProveedor] where IDMatrizPrecio=(SELECT MAX(IDMatrizPrecio) from MatrizPrecioProveedor) and IDArticulo =" + idart + " and IDProveedor =" + idProveedor + "").ToList();
                    decimal num = rango.Select(s => s.RangSup).FirstOrDefault();

                    decimal rangocorrecto = num + .01M;
                    ViewBag.rangoinferior = rangocorrecto;

                }
                //List<VRangoPlaneacionNiveles> rangonivel = db.Database.SqlQuery<VRangoPlaneacionNiveles>("select HE.IDProveedor,RPN.IDMoneda,RPN.TC,RPN.IDRPN,RPN.RangoInf,RPN.RangoSup,RPN.Costo,RPN.Porcentaje,HE.Descripcion,HE.Presentacion,HE.IDArticulo,HE.IDCaracteristica,A.nameFoto from RangoPlaneacionNiveles as RPN inner join HEspecificacionE as HE on RPN.IDHE=HE.IDHE inner join Articulo as A on HE.IDArticulo=A.IDArticulo where HE.IDArticulo=" + idart + " and HE.IDCaracteristica=" + idcarac + " and HE.IDProvedor=" + idProveedor + " order by RPN.RangoInf,RPN.RangoSup").ToList();

                //ViewBag.rangoniveles = rangonivel;
                //int contador = rangonivel.Count;
                //if (contador == 0)
                //{
                //    ViewBag.rangoniveles = null;
                //}

                //ClsDatoString monedapsugerido = db.Database.SqlQuery<ClsDatoString>("select distinct(M.Descripcion) as Dato from RangoPlaneacionNiveles as RPN inner join HEspecificacionE as HE on RPN.IDHE=HE.IDHE inner join c_Moneda as M on M.IDMoneda=RPN.IDMoneda where HE.IDArticulo=" + idart + " and HE.IDCaracteristica=" + idcarac + " and HE.IDCliente=" + idcliente + "").ToList()[0];
                //ViewBag.monedapsugerido = monedapsugerido.Dato;
                return View(elementos);
            }
            catch (Exception err)
            {
                ViewBag.mensaje = err.Message;
            }
            return View(elementos);
        }

        [HttpPost]
        public ActionResult Insertar(int id, int IDProveedor, int IDArticulo, decimal rangi, decimal rangs, decimal precio)
        {
            try
            {
                db.Database.ExecuteSqlCommand("INSERT INTO MatrizPrecioProveedor([IDProveedor],[IDArticulo],[RangInf],[RangSup],[Precio]) values ('" + IDProveedor + "','" + IDArticulo + "','" + rangi + "', '" + rangs + "','" + precio + "')");

                List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
                int usuario = userid.Select(s => s.UserID).FirstOrDefault();
                List<CarritoP> pedido = db.Database.SqlQuery<CarritoP>("select CarritoP.IDProveedor,Articulo.MinimoCompra,Articulo.MinimoVenta,c_ClaveUnidad.Nombre as Unidad,CarritoP.IDCarrito,CarritoP.usuario,CarritoP.IDCaracteristica,CarritoP.Precio,CarritoP.Cantidad,Caracteristica.Presentacion as Presentacion,Articulo.Descripcion,Articulo.IDArticulo,c_Moneda.Descripcion as Moneda,c_Moneda.IDMoneda as IDMoneda,CarritoP.Precio * CarritoP.Cantidad as Importe, CarritoP.Nota from  CarritoP INNER JOIN Caracteristica ON CarritoP.IDCaracteristica= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo INNER JOIN c_Moneda ON c_Moneda.IDMoneda = Articulo.IDMoneda INNER JOIN c_ClaveUnidad ON c_ClaveUnidad.IDClaveUnidad = Articulo.IDClaveUnidad where usuario='" + usuario + "'").ToList();
                ViewBag.carritoP = pedido;
                for (int i = 0; i < pedido.Count(); i++)
                {
                    db.Database.ExecuteSqlCommand("update [dbo].[CarritoP] set [Precio]=(select dbo.GetPrecio(" + IDProveedor + "," + ViewBag.carritoP[i].IDArticulo + ",0," + ViewBag.carrito[i].Cantidad + ")) where IDCarrito=" + ViewBag.carritoP[i].IDCarrito + " and usuario=" + usuario + "");
                }

                return Json(true);
            }
            catch (Exception err)
            {
                return Json(500, err.Message);
            }
        }


        [HttpPost]
        public JsonResult Edititem(int id, decimal rangi, decimal rangs, decimal precio, int IDProveedor)
        {
            try
            {
                db.Database.ExecuteSqlCommand("update [dbo].[MatrizPrecioProveedor] set [RangInf]=" + rangi + ", [RangSup]='" + rangs + "' , Precio='" + precio + "' where IDMatrizPrecio=" + id);

                List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
                int usuario = userid.Select(s => s.UserID).FirstOrDefault();
                List<CarritoP> pedido = db.Database.SqlQuery<CarritoP>("select CarritoP.IDProveedor,Articulo.MinimoCompra,Articulo.MinimoVenta,c_ClaveUnidad.Nombre as Unidad,CarritoP.IDCarrito,CarritoP.usuario,CarritoP.IDCaracteristica,CarritoP.Precio,Carrito.Cantidad,Caracteristica.Presentacion as Presentacion,Articulo.Descripcion,Articulo.IDArticulo,c_Moneda.Descripcion as Moneda,c_Moneda.IDMoneda as IDMoneda,CarritoP.Precio * CarritoP.Cantidad as Importe, CarritoP.Nota from  CarritoP INNER JOIN Caracteristica ON CarritoP.IDCaracteristica= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo INNER JOIN c_Moneda ON c_Moneda.IDMoneda = Articulo.IDMoneda INNER JOIN c_ClaveUnidad ON c_ClaveUnidad.IDClaveUnidad = Articulo.IDClaveUnidad where usuario='" + usuario + "'").ToList();
                ViewBag.carritoP = pedido;
                for (int i = 0; i < pedido.Count(); i++)
                {
                    db.Database.ExecuteSqlCommand("update [dbo].[CarritoP] set [Precio]=(select dbo.GetPrecio(" + IDProveedor + "," + ViewBag.carritoP[i].IDArticulo + ",0," + ViewBag.carritoP[i].Cantidad + ")) where IDCarrito=" + ViewBag.carritoP[i].IDCarrito + " and usuario=" + usuario + "");
                }
                return Json(true);
            }
            catch (Exception err)
            {
                return Json(500, err.Message);
            }
        }

        [HttpPost]
        public JsonResult Deleteitem(int id, int IDProveedor)
        {
            try
            {
                db.Database.ExecuteSqlCommand("delete from [dbo].[MatrizPrecioProveedor] where [IDMatrizPrecio]='" + id + "'");

                List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
                int usuario = userid.Select(s => s.UserID).FirstOrDefault();
                List<CarritoP> pedido = db.Database.SqlQuery<CarritoP>("select CarritoP.IDProveedor,Articulo.MinimoCompra,Articulo.MinimoVenta,c_ClaveUnidad.Nombre as Unidad,CarritoP.IDCarrito,CarritoP.usuario,CarritoP.IDCaracteristica,CarritoP.Precio,CarritoP.Cantidad,Caracteristica.Presentacion as Presentacion,Articulo.Descripcion,Articulo.IDArticulo,c_Moneda.Descripcion as Moneda,c_Moneda.IDMoneda as IDMoneda,CarritoP.Precio * CarritoP.Cantidad as Importe, CarritoP.Nota from  CarritoP INNER JOIN Caracteristica ON Carrito.IDCaracteristica= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo INNER JOIN c_Moneda ON c_Moneda.IDMoneda = Articulo.IDMoneda INNER JOIN c_ClaveUnidad ON c_ClaveUnidad.IDClaveUnidad = Articulo.IDClaveUnidad where usuario='" + usuario + "'").ToList();
                ViewBag.carritoP = pedido;
                for (int i = 0; i < pedido.Count(); i++)
                {
                    db.Database.ExecuteSqlCommand("update [dbo].[CarritoP] set [Precio]=(select dbo.GetPrecio(" + IDProveedor + "," + ViewBag.carritoP[i].IDArticulo + ",0," + ViewBag.carritoP[i].Cantidad + ")) where IDCarrito=" + ViewBag.carritoP[i].IDCarrito + " and usuario=" + usuario + "");
                }
                return Json(true);
            }
            catch (Exception err)
            {
                return Json(500, err.Message);
            }
        }

        public ActionResult InsertarSugerido(int id, int? idcarrito)
        {
            try
            {
                List<VRangoPlaneacionNivelesProv> rangonivel = db.Database.SqlQuery<VRangoPlaneacionNivelesProv>("select HE.IDProveedor,RPN.IDMoneda,RPN.TC,RPN.IDRPN,RPN.RangoInf,RPN.RangoSup,RPN.Costo,RPN.Porcentaje,HE.Descripcion,HE.Presentacion,HE.IDArticulo,HE.IDCaracteristica,A.nameFoto from RangoPlaneacionNiveles as RPN inner join HEspecificacionE as HE on RPN.IDHE=HE.IDHE inner join Articulo as A on HE.IDArticulo=A.IDArticulo where RPN.IDRPN = " + id + "").ToList();
                int idmoneda = rangonivel.Select(s => s.IDMoneda).FirstOrDefault();
                int idProveedor = rangonivel.Select(s => s.IDProveedores).FirstOrDefault();
                int idart = rangonivel.Select(s => s.IDArticulo).FirstOrDefault();
                decimal rangi = rangonivel.Select(s => s.RangoInf).FirstOrDefault();
                decimal rangs = rangonivel.Select(s => s.RangoSup).FirstOrDefault();
                decimal precio = rangonivel.Select(s => s.Costo).FirstOrDefault();
                Proveedor proveedor = new ProveedorContext().Proveedores.Find(idProveedor);
                if (proveedor.IDMoneda != idmoneda)
                {
                    string fecha = DateTime.Now.ToString("yyyy/MM/dd");
                    VCambio cambio = db.Database.SqlQuery<VCambio>("select dbo.GetTipocambio('" + fecha + "'," + idmoneda + "," + proveedor.IDMoneda + ") as TC").ToList()[0];
                    precio = precio * cambio.TC;
                }
                db.Database.ExecuteSqlCommand("INSERT INTO MatrizPrecioProveedor([IDProveedor],[IDArticulo],[RangInf],[RangSup],[Precio]) values ('" + idProveedor + "','" + idart + "','" + rangi + "', '" + rangs + "','" + precio + "')");

                List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
                int usuario = userid.Select(s => s.UserID).FirstOrDefault();
                List<CarritoP> pedido = db.Database.SqlQuery<CarritoP>("select Carrito.IDProveedor,Articulo.MinimoCompra,Articulo.MinimoVenta,c_ClaveUnidad.Nombre as Unidad,Carrito.IDCarrito,Carrito.usuario,Carrito.IDCaracteristica,Carrito.Precio,Carrito.Cantidad,Caracteristica.Presentacion as Presentacion,Articulo.Descripcion,Articulo.IDArticulo,c_Moneda.Descripcion as Moneda,c_Moneda.IDMoneda as IDMoneda,Carrito.Precio * Carrito.Cantidad as Importe, Carrito.Nota from  Carrito INNER JOIN Caracteristica ON Carrito.IDCaracteristica= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo INNER JOIN c_Moneda ON c_Moneda.IDMoneda = Articulo.IDMoneda INNER JOIN c_ClaveUnidad ON c_ClaveUnidad.IDClaveUnidad = Articulo.IDClaveUnidad where usuario='" + usuario + "'").ToList();
                ViewBag.carritoP = pedido;
                for (int i = 0; i < pedido.Count(); i++)
                {
                    db.Database.ExecuteSqlCommand("update [dbo].[CarritoP] set [Precio]=(select dbo.GetPrecio(" + idProveedor + "," + ViewBag.carritoP[i].IDArticulo + ",0," + ViewBag.carritoP[i].Cantidad + ")) where IDCarrito=" + ViewBag.carritoP[i].IDCarrito + " and usuario=" + usuario + "");
                }
                return RedirectToAction("Agrega", new { id = idcarrito });

            }
            catch (Exception err)
            {
                return RedirectToAction("Agrega", new { id = idcarrito });
            }

        }
    }
}