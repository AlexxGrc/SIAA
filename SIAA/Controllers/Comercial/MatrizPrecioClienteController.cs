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
    [Authorize(Roles = "Administrador,Sistemas,Ventas,Comercial, Gerencia,GerenteVentas")]
    public class MatrizPrecioClienteController : Controller
    {
        // GET: MatrizPrecioCliente
        private MatrizPrecioClienteContext db = new MatrizPrecioClienteContext();
        // GET: Almacen
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.ASortParm = String.IsNullOrEmpty(sortOrder) ? "Cliente" : "Cliente";
            ViewBag.BSortParm = String.IsNullOrEmpty(sortOrder) ? "Articulo" : "Articulo";
            // Not sure here
            if (searchString == null)
            {
                searchString = currentFilter;
            }

            // Pass filtering string to view in order to maintain filtering when paging
            ViewBag.SearchString = searchString;
            //Paginación
            var elementos = from s in db.MPC
                            select s;
            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                elementos = elementos.Where(s => s.Clientes.Nombre.Contains(searchString) || s.Articulo.Descripcion.Contains(searchString));
            }

            //Ordenacion

            switch (sortOrder)
            {
                case "Cliente":
                    elementos = elementos.OrderBy(s => s.Clientes.Nombre);
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
            int count = db.MPC.OrderBy(e => e.IDMatrizPrecio).Count(); // Total number of elements

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
            
            MatrizPrecioCliente matrizpreciocliente = db.MPC.Find(id);
          
            return View(matrizpreciocliente);
        }

       
        // GET: Almacen/Create
        public ActionResult Create()
        {
            ViewBag.IDCliente = new SelectList(db.Clientes.OrderBy(s => s.Nombre), "IDCliente", "Nombre");
            ViewBag.IDArticulo = new SelectList(db.Articulo.OrderBy(s => s.Descripcion), "IDArticulo", "Descripcion");
            return View();
        }

        // POST: Almacen/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MatrizPrecioCliente matrizpreciocliente)
        {
            try
            {
                // TODO: Add insert logic here
                
                db.MPC.Add(matrizpreciocliente);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.IDCliente = new SelectList(db.Clientes, "IDCliente", "Nombre", matrizpreciocliente.IDCliente);
                ViewBag.IDArticulo = new SelectList(db.Articulo, "IDArticulo", "Descripcion", matrizpreciocliente.IDArticulo);
                return View();
            }
        }

      
        public ActionResult Edit(int? id)
        {

            MatrizPrecioCliente matrizpreciocliente = db.MPC.Find(id);

            ViewBag.IDCliente = new SelectList(db.Clientes, "IDCliente", "Nombre", matrizpreciocliente.IDCliente);
            ViewBag.IDArticulo = new SelectList(db.Articulo, "IDArticulo", "Descripcion", matrizpreciocliente.IDArticulo);
            ViewBag.IDMoneda = new SelectList(db.c_Monedas, "IDMoneda", "ClaveMoneda", matrizpreciocliente.IDMoneda);
            return View(matrizpreciocliente);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MatrizPrecioCliente matrizpreciocliente)
        {
            if (ModelState.IsValid)
            {
                db.Entry(matrizpreciocliente).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IDCliente = new SelectList(db.Clientes, "IDCliente", "Nombre", matrizpreciocliente.IDCliente);
            ViewBag.IDArticulo = new SelectList(db.Articulo, "IDArticulo", "Descripcion", matrizpreciocliente.IDArticulo);
            ViewBag.IDMoneda = new SelectList(db.c_Monedas, "IDMoneda", "ClaveMoneda", matrizpreciocliente.IDMoneda);
            return View(matrizpreciocliente);
        }
        public ActionResult Delete(int? id)
        {

            MatrizPrecioCliente matrizpreciocliente = db.MPC.Find(id);

            return View(matrizpreciocliente);
        }

        // POST: Clientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MatrizPrecioCliente matrizpreciocliente = db.MPC.Find(id);
            db.MPC.Remove(matrizpreciocliente);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        
        public ActionResult Agrega(int? id)
        {
            ViewBag.id = id;
            List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            int usuario = userid.Select(s => s.UserID).FirstOrDefault();

            List<VCarritoV> pedido = db.Database.SqlQuery<VCarritoV>("select Carrito.IDCliente,Articulo.MinimoCompra,Articulo.MinimoVenta,c_ClaveUnidad.Nombre as Unidad,Carrito.IDCarrito,Carrito.usuario,Carrito.IDCaracteristica,Carrito.Precio,Carrito.Cantidad,Caracteristica.Presentacion as Presentacion,Articulo.Descripcion,Articulo.IDArticulo,c_Moneda.Descripcion as Moneda,c_Moneda.IDMoneda as IDMoneda,Carrito.Precio * Carrito.Cantidad as Importe, Carrito.Nota from  Carrito INNER JOIN Caracteristica ON Carrito.IDCaracteristica= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo INNER JOIN c_Moneda ON c_Moneda.IDMoneda = Articulo.IDMoneda INNER JOIN c_ClaveUnidad ON c_ClaveUnidad.IDClaveUnidad = Articulo.IDClaveUnidad where usuario='" + usuario + "' and IDCarrito=" + id + "").ToList();

            int idcliente = pedido.Select(s => s.IDCliente).FirstOrDefault();
            int idart = pedido.Select(s => s.IDArticulo).FirstOrDefault();
            decimal cantidad = pedido.Select(s => s.Cantidad).FirstOrDefault();
            int idcarac = pedido.Select(s => s.IDCaracteristica).FirstOrDefault();
            Articulo articulo = new ArticuloContext().Articulo.Find(idart);

            ViewBag.Cref = articulo.Cref;

            c_ClaveUnidad unidad = new c_ClaveUnidadContext().c_ClaveUnidades.Find(articulo.IDClaveUnidad);
            ViewBag.unidad = unidad.Nombre;
            //var elementos = db.Database.SqlQuery<MatrizPrecioCliente>("select * from MatrizPrecioCliente where IDArticulo =" + idart + " and IDCliente =" + idcliente + "").ToList();
            var elementos = from s in db.MPC
                            where s.IDArticulo.Equals(idart) && s.IDCliente.Equals(idcliente)
                            select s;
            try
            {
               

                Clientes cliente = new ClientesContext().Clientes.Find(idcliente);
                c_Moneda monedaarticulo = new c_MonedaContext().c_Monedas.Find(articulo.IDMoneda);
                c_Moneda monedacliente = new c_MonedaContext().c_Monedas.Find(cliente.IDMoneda);

                ViewBag.moneda = monedaarticulo.Descripcion;
                ViewBag.monedacliente = monedacliente.Descripcion;

                ViewBag.IDCliente = new SelectList(db.Clientes.Where(s => s.IDCliente.Equals(idcliente)), "IDCliente", "Nombre");
                ViewBag.IDArticulo = new SelectList(db.Articulo.Where(s => s.IDArticulo.Equals(idart)), "IDArticulo", "Descripcion");
                ViewBag.IDMoneda = new SelectList(db.c_Monedas, "IDMoneda", "ClaveMoneda", articulo.IDMoneda);

                ViewBag.rangoinferior = .01;
                ViewBag.rangosuperior = 999.99;

                ClsDatoEntero countRango = db.Database.SqlQuery<ClsDatoEntero>("select count(IDMatrizPrecio) as Dato from MatrizPrecioCliente where IDArticulo =" + idart + " and IDCliente =" + idcliente + "").ToList()[0];
                if (countRango.Dato != 0)
                {

                    List<MatrizPrecioCliente> rango = db.Database.SqlQuery<MatrizPrecioCliente>("select * from [dbo].[MatrizPrecioCliente] where IDMatrizPrecio=(SELECT MAX(IDMatrizPrecio) from MatrizPrecioCliente) and IDArticulo =" + idart + " and IDCliente =" + idcliente + "").ToList();
                    decimal num = rango.Select(s => s.RangSup).FirstOrDefault();

                    decimal rangocorrecto = num + .01M;
                    ViewBag.rangoinferior = rangocorrecto;

                }
                List<VRangoPlaneacionNiveles> rangonivel = db.Database.SqlQuery<VRangoPlaneacionNiveles>("select HE.IDCliente,RPN.IDMoneda,RPN.TC,RPN.IDRPN,RPN.RangoInf,RPN.RangoSup,RPN.Costo,RPN.Porcentaje,HE.Descripcion,HE.Presentacion,HE.IDArticulo,HE.IDCaracteristica,A.nameFoto from RangoPlaneacionNiveles as RPN inner join HEspecificacionE as HE on RPN.IDHE=HE.IDHE inner join Articulo as A on HE.IDArticulo=A.IDArticulo where HE.IDArticulo=" + idart + " and HE.IDCaracteristica=" + idcarac + " and HE.IDCliente=" + idcliente + " order by RPN.RangoInf,RPN.RangoSup").ToList();

                ViewBag.rangoniveles = rangonivel;
                int contador = rangonivel.Count;
                if(contador==0)
                {
                    ViewBag.rangoniveles = null;
                }

                ClsDatoString monedapsugerido = db.Database.SqlQuery<ClsDatoString>("select distinct(M.Descripcion) as Dato from RangoPlaneacionNiveles as RPN inner join HEspecificacionE as HE on RPN.IDHE=HE.IDHE inner join c_Moneda as M on M.IDMoneda=RPN.IDMoneda where HE.IDArticulo=" + idart + " and HE.IDCaracteristica=" + idcarac + " and HE.IDCliente=" + idcliente + "").ToList()[0];
                ViewBag.monedapsugerido = monedapsugerido.Dato;
                return View(elementos);
            }
            catch (Exception err)
            {
               ViewBag.mensaje=err.Message;
            }
            return View(elementos);
        }
        
        [HttpPost]
        public ActionResult Insertar(int id, int IDCliente, int IDArticulo, decimal rangi, decimal rangs, decimal precio, int IDMoneda)
        {

            
            try
            {
                db.Database.ExecuteSqlCommand("INSERT INTO MatrizPrecioCliente([IDCliente],[IDArticulo],[RangInf],[RangSup],[Precio],IDMoneda) values ('" + IDCliente + "','" + IDArticulo + "','" + rangi + "', '" + rangs + "','" + precio + "',"+IDMoneda+")");

                List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
                int usuario = userid.Select(s => s.UserID).FirstOrDefault();
                List<VCarritoV> pedido = db.Database.SqlQuery<VCarritoV>("select Carrito.IDCliente,Articulo.MinimoCompra,Articulo.MinimoVenta,c_ClaveUnidad.Nombre as Unidad,Carrito.IDCarrito,Carrito.usuario,Carrito.IDCaracteristica,Carrito.Precio,Carrito.Cantidad,Caracteristica.Presentacion as Presentacion,Articulo.Descripcion,Articulo.IDArticulo,c_Moneda.Descripcion as Moneda,c_Moneda.IDMoneda as IDMoneda,Carrito.Precio * Carrito.Cantidad as Importe, Carrito.Nota from  Carrito INNER JOIN Caracteristica ON Carrito.IDCaracteristica= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo INNER JOIN c_Moneda ON c_Moneda.IDMoneda = Articulo.IDMoneda INNER JOIN c_ClaveUnidad ON c_ClaveUnidad.IDClaveUnidad = Articulo.IDClaveUnidad where usuario='" + usuario + "'").ToList();
                 ViewBag.carrito = pedido;
                for (int i = 0; i < pedido.Count(); i++)
                {
                     db.Database.ExecuteSqlCommand("update [dbo].[Carrito] set [Precio]=(select dbo.GetPrecio(" + IDCliente + "," + ViewBag.carrito[i].IDArticulo + ",0," + ViewBag.carrito[i].Cantidad + ")), IDMoneda="+ IDMoneda+" where IDCarrito=" + ViewBag.carrito[i].IDCarrito + " and usuario=" + usuario + "");
                   }

                return Json(true);
            }
            catch (Exception err)
            {
                return Json(500, err.Message);
            }
        }


        [HttpPost]
        public JsonResult Edititem(int id, decimal rangi,decimal rangs,decimal precio,int IDCliente)
        {
            try
            {
                db.Database.ExecuteSqlCommand("update [dbo].[MatrizPrecioCliente] set [RangInf]=" + rangi + ", [RangSup]='" + rangs + "' , Precio='"+precio+"' where IDMatrizPrecio=" + id);

                List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
                int usuario = userid.Select(s => s.UserID).FirstOrDefault();
                List<VCarritoV> pedido = db.Database.SqlQuery<VCarritoV>("select Carrito.IDCliente,Articulo.MinimoCompra,Articulo.MinimoVenta,c_ClaveUnidad.Nombre as Unidad,Carrito.IDCarrito,Carrito.usuario,Carrito.IDCaracteristica,Carrito.Precio,Carrito.Cantidad,Caracteristica.Presentacion as Presentacion,Articulo.Descripcion,Articulo.IDArticulo,c_Moneda.Descripcion as Moneda,c_Moneda.IDMoneda as IDMoneda,Carrito.Precio * Carrito.Cantidad as Importe, Carrito.Nota from  Carrito INNER JOIN Caracteristica ON Carrito.IDCaracteristica= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo INNER JOIN c_Moneda ON c_Moneda.IDMoneda = Articulo.IDMoneda INNER JOIN c_ClaveUnidad ON c_ClaveUnidad.IDClaveUnidad = Articulo.IDClaveUnidad where usuario='" + usuario + "'").ToList();
                ViewBag.carrito = pedido;
                for (int i = 0; i < pedido.Count(); i++)
                {
                    db.Database.ExecuteSqlCommand("update [dbo].[Carrito] set [Precio]=(select dbo.GetPrecio(" + IDCliente + "," + ViewBag.carrito[i].IDArticulo + ",0," + ViewBag.carrito[i].Cantidad + ")) where IDCarrito=" + ViewBag.carrito[i].IDCarrito + " and usuario=" + usuario + "");
                }
                return Json(true);
            }
            catch (Exception err)
            {
                return Json(500, err.Message);
            }
        }

        [HttpPost]
        public JsonResult Deleteitem(int id, int IDCliente)
        {
            try
            {
                db.Database.ExecuteSqlCommand("delete from [dbo].[MatrizPrecioCliente] where [IDMatrizPrecio]='" + id + "'");

                List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
                int usuario = userid.Select(s => s.UserID).FirstOrDefault();
                List<VCarritoV> pedido = db.Database.SqlQuery<VCarritoV>("select Carrito.IDCliente,Articulo.MinimoCompra,Articulo.MinimoVenta,c_ClaveUnidad.Nombre as Unidad,Carrito.IDCarrito,Carrito.usuario,Carrito.IDCaracteristica,Carrito.Precio,Carrito.Cantidad,Caracteristica.Presentacion as Presentacion,Articulo.Descripcion,Articulo.IDArticulo,c_Moneda.Descripcion as Moneda,c_Moneda.IDMoneda as IDMoneda,Carrito.Precio * Carrito.Cantidad as Importe, Carrito.Nota from  Carrito INNER JOIN Caracteristica ON Carrito.IDCaracteristica= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo INNER JOIN c_Moneda ON c_Moneda.IDMoneda = Articulo.IDMoneda INNER JOIN c_ClaveUnidad ON c_ClaveUnidad.IDClaveUnidad = Articulo.IDClaveUnidad where usuario='" + usuario + "'").ToList();
                ViewBag.carrito = pedido;
                for (int i = 0; i < pedido.Count(); i++)
                {
                    db.Database.ExecuteSqlCommand("update [dbo].[Carrito] set [Precio]=(select dbo.GetPrecio(" + IDCliente + "," + ViewBag.carrito[i].IDArticulo + ",0," + ViewBag.carrito[i].Cantidad + ")) where IDCarrito=" + ViewBag.carrito[i].IDCarrito + " and usuario=" + usuario + "");
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
                List<VRangoPlaneacionNiveles> rangonivel = db.Database.SqlQuery<VRangoPlaneacionNiveles>("select HE.IDCliente,RPN.IDMoneda,RPN.TC,RPN.IDRPN,RPN.RangoInf,RPN.RangoSup,RPN.Costo,RPN.Porcentaje,HE.Descripcion,HE.Presentacion,HE.IDArticulo,HE.IDCaracteristica,A.nameFoto from RangoPlaneacionNiveles as RPN inner join HEspecificacionE as HE on RPN.IDHE=HE.IDHE inner join Articulo as A on HE.IDArticulo=A.IDArticulo where RPN.IDRPN = " + id + "").ToList();
                int idmoneda = rangonivel.Select(s => s.IDMoneda).FirstOrDefault();
                int idcliente = rangonivel.Select(s => s.IDCliente).FirstOrDefault();
                int idart = rangonivel.Select(s => s.IDArticulo).FirstOrDefault();
                decimal rangi = rangonivel.Select(s => s.RangoInf).FirstOrDefault();
                decimal rangs = rangonivel.Select(s => s.RangoSup).FirstOrDefault();
                decimal precio = rangonivel.Select(s => s.Costo).FirstOrDefault();
                Clientes cliente = new ClientesContext().Clientes.Find(idcliente);
                if (cliente.IDMoneda != idmoneda)
                {
                    string fecha = DateTime.Now.ToString("yyyy/MM/dd");
                    VCambio cambio = db.Database.SqlQuery<VCambio>("select dbo.GetTipocambio('" + fecha + "'," + idmoneda + "," + cliente.IDMoneda + ") as TC").ToList()[0];
                    precio = precio * cambio.TC;
                }
                db.Database.ExecuteSqlCommand("INSERT INTO MatrizPrecioCliente([IDCliente],[IDArticulo],[RangInf],[RangSup],[Precio]) values ('" + idcliente + "','" + idart + "','" + rangi + "', '" + rangs + "','" + precio + "')");

                List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
                int usuario = userid.Select(s => s.UserID).FirstOrDefault();
                List<VCarritoV> pedido = db.Database.SqlQuery<VCarritoV>("select Carrito.IDCliente,Articulo.MinimoCompra,Articulo.MinimoVenta,c_ClaveUnidad.Nombre as Unidad,Carrito.IDCarrito,Carrito.usuario,Carrito.IDCaracteristica,Carrito.Precio,Carrito.Cantidad,Caracteristica.Presentacion as Presentacion,Articulo.Descripcion,Articulo.IDArticulo,c_Moneda.Descripcion as Moneda,c_Moneda.IDMoneda as IDMoneda,Carrito.Precio * Carrito.Cantidad as Importe, Carrito.Nota from  Carrito INNER JOIN Caracteristica ON Carrito.IDCaracteristica= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo INNER JOIN c_Moneda ON c_Moneda.IDMoneda = Articulo.IDMoneda INNER JOIN c_ClaveUnidad ON c_ClaveUnidad.IDClaveUnidad = Articulo.IDClaveUnidad where usuario='" + usuario + "'").ToList();
                ViewBag.carrito = pedido;
                for (int i = 0; i < pedido.Count(); i++)
                {
                    db.Database.ExecuteSqlCommand("update [dbo].[Carrito] set [Precio]=(select dbo.GetPrecio(" + idcliente + "," + ViewBag.carrito[i].IDArticulo + ",0," + ViewBag.carrito[i].Cantidad + ")) where IDCarrito=" + ViewBag.carrito[i].IDCarrito + " and usuario=" + usuario + "");
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