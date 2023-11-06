using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SIAAPI.Models.Comercial;
using SIAAPI.Models.Login;
using SIAAPI.ViewModels.Cfdi;
using SIAAPI.ViewModels.Comercial;

namespace SIAAPI.Controllers.Comercial
{
    [Authorize(Roles = "Almacenista,Administrador,Compras,Sistemas,Facturacion")]

    public class CarritoCController : Controller
    {
        private CarritoContext db = new CarritoContext();


        public ActionResult Index(int idProveedor = 0)
        {


            List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            int usuario = userid.Select(s => s.UserID).FirstOrDefault();
            // string usuario = System.Web.HttpContext.Current.Session["SessionU"].ToString();

            try
            {


                //  IDCliente = int.Parse( coleccion.Get("IDCliente"));
                if (idProveedor > 0)
                {
                    //ClsDatoDecimal precioCarrito = db.Database.SqlQuery<ClsDatoDecimal>("select precio as Dato from CarritoC where usuario='" + usuario + "'").ToList().FirstOrDefault();


                    actualizaprecio(idProveedor);
                }
                if (idProveedor == 0)
                {
                    ClsDatoEntero contarproveedor = db.Database.SqlQuery<ClsDatoEntero>("select distinct idProveedor as Dato from CarritoC where usuario='" + usuario + "'").ToList().FirstOrDefault();

                    if (contarproveedor == null)
                    {
                        ViewBag.nota = "";
                        idProveedor = 0;
                        ViewBag.Proveedorseleccionado = 0;
                    }
                    else
                    {

                        ClsDatoString cref = db.Database.SqlQuery<ClsDatoString>("select Articulo.Cref as Dato from  CarritoC INNER JOIN Caracteristica ON CarritoC.IDCaracteristica= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo INNER JOIN c_Moneda ON c_Moneda.IDMoneda = CarritoC.IDMoneda INNER JOIN c_ClaveUnidad ON c_ClaveUnidad.IDClaveUnidad = Articulo.IDClaveUnidad where usuario='" + usuario + "'").ToList()[0];
                        ViewBag.cref = cref.Dato;
                        idProveedor = contarproveedor.Dato;
                        actualizaprecio(idProveedor);
                        string cadena = "select idarticulo as Dato from Articulo where cref='" + ViewBag.cref + "'";
                        ClsDatoEntero idarticulo = db.Database.SqlQuery<ClsDatoEntero>(cadena).ToList().FirstOrDefault();
                        ViewBag.idart = idarticulo.Dato;

                        //string ob = "select Observacion as Dato from MatrizPrecioProv where idProveedor=" + idProveedor + " and idarticulo=" + ViewBag.idart;
                        //ClsDatoString Observacion = db.Database.SqlQuery<ClsDatoString>(ob).ToList().FirstOrDefault();

                        //ViewBag.nota = Observacion.Dato;


                        //string ejecutar = "update [dbo].[CarritoC] set [Nota]='" + Observacion.Dato + "' where IDCarrito=" + ViewBag.idCarrito + " and usuario=" + usuario;

                        //db.Database.ExecuteSqlCommand(ejecutar);

                        ViewBag.Proveedorseleccionado = contarproveedor.Dato;
                    }
                }
            }
            catch (Exception err)
            {



            }




            var elementos = db.Database.SqlQuery<VCarrito>("select (CarritoC.Precio * (select dbo.GetTipocambio(GETDATE(),CarritoC.IDMoneda,(select IDMoneda from c_Moneda WHERE ClaveMoneda='MXN')))) * CarritoC.Cantidad as preciomex,Articulo.MinimoCompra,Articulo.MinimoVenta,Articulo.Cref as Cref, Caracteristica.IDPresentacion as IDPresentacion,c_ClaveUnidad.Nombre as Unidad,CarritoC.IDCarrito,CarritoC.usuario,CarritoC.IDCaracteristica,CarritoC.Precio,CarritoC.Cantidad,Caracteristica.Presentacion as Presentacion,Articulo.Descripcion,Articulo.IDArticulo,c_Moneda.Descripcion as Moneda,c_Moneda.IDMoneda as IDMoneda,CarritoC.Precio * CarritoC.Cantidad as Importe, CarritoC.Nota from  CarritoC INNER JOIN Caracteristica ON CarritoC.IDCaracteristica= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo INNER JOIN c_Moneda ON c_Moneda.IDMoneda = CarritoC.IDMoneda INNER JOIN c_ClaveUnidad ON c_ClaveUnidad.IDClaveUnidad = Articulo.IDClaveUnidad where usuario='" + usuario + "'").ToList();

            var divisa = db.Database.SqlQuery<ResumenFac>("select (select ClaveMoneda FROM C_MONEDA WHERE IDMoneda=CarritoC.IDMoneda) as MonedaOrigen, (select SUM(CarritoC.Precio * CarritoC.Cantidad)) as Subtotal, SUM(CarritoC.Precio * CarritoC.Cantidad)*0.16 as IVA, (SUM(CarritoC.Precio * CarritoC.Cantidad)) + (SUM(CarritoC.Precio * CarritoC.Cantidad)*0.16) as Total ,0 as TotalPesos from CarritoC INNER JOIN Caracteristica ON CarritoC.IDCaracteristica= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo INNER JOIN c_Moneda ON c_Moneda.IDMoneda = CarritoC.IDMoneda INNER JOIN c_ClaveUnidad ON c_ClaveUnidad.IDClaveUnidad = Articulo.IDClaveUnidad where usuario='" + usuario + "' group by CarritoC.IDMoneda").ToList();
            ViewBag.sumatoria = divisa;

            ProveedorContext prov = new ProveedorContext();
            var proveedor = prov.Proveedores.OrderBy(s => s.Empresa).ToList();
            List<SelectListItem> li = new List<SelectListItem>();
            li.Add(new SelectListItem { Text = "--Selecciona Proveedor--", Value = "0" });

            foreach (var m in proveedor)
            {
                li.Add(new SelectListItem { Text = m.Empresa, Value = m.IDProveedor.ToString() });


            }
            ViewBag.proveedor = li;

            return View(elementos);
        }

        public ActionResult grabar(FormCollection coleccion)
        {
            int idproveedor = int.Parse(coleccion.Get("idproveedor").ToString());
            int elementos = ((coleccion.Count -1)/ 4);
            for (int i=0; i<elementos;i++)
            {
                decimal precio = decimal.Parse(coleccion.Get("precio" + i));
                string nota = coleccion.Get("Nota" + i).ToString();
                string id= coleccion.Get("ID" + i).ToString();
                string cantidad = coleccion.Get("Cantidad" + i).ToString();
                new CarritoContext().Database.ExecuteSqlCommand("update carritoc set nota= '" + nota + "', precio=" + precio + ", Cantidad="+cantidad+" WHERE IDCarrito=" + id);
            }
            return RedirectToAction("Index", new { idProveedor = idproveedor });
          }

        public void actualizaprecio(int id)
        {
            Proveedor proveedor = new ProveedorContext().Proveedores.Find(id);

            List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            int usuario = userid.Select(s => s.UserID).FirstOrDefault();

            db.Database.ExecuteSqlCommand("update [dbo].[CarritoC] set [IDProveedor]=" + id + " where usuario=" + usuario + "");

            //     db.Database.ExecuteSqlCommand("update [dbo].[Carrito] set [IDCliente]=" + id + " where usuario=" + usuario + "");

            List<VCarritoV> pedido = db.Database.SqlQuery<VCarritoV>("select CarritoC.IDProveedor,Articulo.MinimoCompra,Articulo.MinimoVenta,c_ClaveUnidad.Nombre as Unidad,CarritoC.IDCarrito,CarritoC.usuario,CarritoC.IDCaracteristica,CarritoC.Precio,CarritoC.Cantidad,Caracteristica.Presentacion as Presentacion,Articulo.Descripcion,Articulo.IDArticulo,c_Moneda.Descripcion as Moneda,c_Moneda.IDMoneda as IDMoneda,CarritoC.Precio * CarritoC.Cantidad as Importe, CarritoC.Nota from  CarritoC INNER JOIN Caracteristica ON CarritoC.IDCaracteristica= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo INNER JOIN c_Moneda ON c_Moneda.IDMoneda = Articulo.IDMoneda INNER JOIN c_ClaveUnidad ON c_ClaveUnidad.IDClaveUnidad = Articulo.IDClaveUnidad where usuario='" + usuario + "'").ToList();


            ViewBag.carrito = pedido;


            string fecha = DateTime.Now.ToString("yyyy/MM/dd");
            for (int i = 0; i < pedido.Count(); i++)
            {


                int monedaorigen = ViewBag.carrito[i].IDMoneda;
                VCambio cambio = db.Database.SqlQuery<VCambio>("select dbo.GetTipocambio('" + fecha + "'," + monedaorigen + "," + proveedor.IDMoneda + ") as TC").ToList().FirstOrDefault();
                int Monedadestino = monedaorigen;

                try
                {
                    string cadenam = " select * from MatrizPrecioProv where IDArticulo = " + ViewBag.carrito[i].IDArticulo + " and IDProveedor = " + id;
                    MatrizPrecioProv Moneaveri = new CarritoPContext().Database.SqlQuery<MatrizPrecioProv>(cadenam).ToList().FirstOrDefault();
                    Monedadestino = Moneaveri.IDMoneda;



                }
                catch (Exception err)
                {

                }

                //string ob = "select Observacion as Dato from MatrizPrecioProv where idProveedor=" + id + " and idarticulo=" + ViewBag.carrito[i].IDArticulo;
                //ClsDatoString Observacion = db.Database.SqlQuery<ClsDatoString>(ob).ToList().FirstOrDefault();

                //string nota = string.Empty;
                //try
                //{
                //    nota = Observacion.Dato;
                //}
                //catch (Exception ERR)
                //{

                //}
                string cadenapreciocarrito = "select precio as dato from CarritoC where idcarrito=" + ViewBag.carrito[i].IDCarrito;
                ClsDatoDecimal preciocarrito = new CarritoContext().Database.SqlQuery<ClsDatoDecimal>(cadenapreciocarrito).ToList().FirstOrDefault();

                /// 
                string cadenaprecio = "select dbo.GetCosto(" + id + "," + ViewBag.carrito[i].IDArticulo + "," + ViewBag.carrito[i].Cantidad + ") as Dato";
                ClsDatoDecimal precio = new CarritoContext().Database.SqlQuery<ClsDatoDecimal>(cadenaprecio).ToList().FirstOrDefault();



                string cadenaprecioprov = "select precio as dato from CarritoC where idcarrito=" + ViewBag.carrito[i].IDCarrito;
                ClsDatoDecimal preciopro = new CarritoContext().Database.SqlQuery<ClsDatoDecimal>(cadenaprecioprov).ToList().FirstOrDefault();


                decimal Precioarticulo = 0;
                if (preciocarrito.Dato > 0)
                {
                    Precioarticulo = preciocarrito.Dato;
                }
                else
                {
                    if (precio.Dato > 0)
                    {
                        Precioarticulo = precio.Dato;
                    }



                }


                string ejecutar = "update [dbo].[CarritoC] set [Precio]=" + Precioarticulo + ",IDMoneda=" + Monedadestino + ", Nota='" + ViewBag.carrito[i].Nota + "'  where IDCarrito=" + ViewBag.carrito[i].IDCarrito + " and usuario=" + usuario;

                db.Database.ExecuteSqlCommand(ejecutar);
            }/// 
        }

         /// <summary>
        /// Post del index
        /// </summary>
        /// <returns></returns>
        /// 
        //[HttpPost]
        //public ActionResult Index(IEnumerable<SIAAPI.Models.Comercial.VCarrito> vcarrito)
        //{

        //    return View(vcarrito);
        //}

        public ActionResult Create()
        {
            return View();
        }

       

        [HttpPost]

        public ActionResult Edit(Carrito carrito)
        {
            if (ModelState.IsValid)
            {

                db.Database.ExecuteSqlCommand("update [dbo].[CarritoC] set [Cantidad]='" + carrito.Cantidad + "' where IDCarrito=" + carrito.IDCarrito);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(carrito);
        }
        [HttpPost]
        public JsonResult Editgeneral(int id, decimal cantidad, string nota, decimal Precio)
        {

            List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            int usuario = userid.Select(s => s.UserID).FirstOrDefault();
            var elementos = db.Database.SqlQuery<VCarrito>("select * from CarritoC where usuario='" + usuario + "'").ToList();
            try
            {
                foreach ( var p in elementos)     
            {
               
                    System.Diagnostics.Debug.WriteLine("Cantidad a cambiar " + cantidad);
                    CarritoContext car = new CarritoContext();
                    db.Database.ExecuteSqlCommand("update [dbo].[CarritoC] set [Cantidad]=" + cantidad + ", [Nota]='" + nota + "', Precio=" + Precio + "  where IDCarrito=" + id);
                    //return Json(true);
                }
                
            }
            catch (Exception err)
            {
                return Json(500, err.Message);
            }

            return Json(true);
        }
        [HttpPost]
        public JsonResult Edititem(int id, decimal cantidad, string nota, decimal Precio)
        {

             try
            {
                

                    System.Diagnostics.Debug.WriteLine("Cantidad a cambiar " + cantidad);
                    CarritoContext car = new CarritoContext();
                    db.Database.ExecuteSqlCommand("update [dbo].[CarritoC] set [Cantidad]=" + cantidad + ", [Nota]='" + nota + "', Precio=" + Precio + "  where IDCarrito=" + id);
                return Json(true);
            }

            
            catch (Exception err)
            {
                return Json(500, err.Message);
            }

                   


        }

        [HttpPost]
        public JsonResult Deleteitem(int id)
        {
            try
            {
                CarritoContext car = new CarritoContext();
                car.Database.ExecuteSqlCommand("delete from CarritoC where IDCarrito=" + id);

                return Json(true);
            }
            catch (Exception err)
            {
                return Json(500, err.Message);
            }
        }
        public ActionResult getPrecio(int id)
        {
            Proveedor proveedor = new ProveedorContext().Proveedores.Find(id);

            List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            int usuario = userid.Select(s => s.UserID).FirstOrDefault();

            db.Database.ExecuteSqlCommand("update [dbo].[CarritoC] set [IDProveedor]=" + id + " where usuario=" + usuario + "");

            //List<VCarritoV> pedido = db.Database.SqlQuery<VCarritoV>("select Carrito.IDCliente,Articulo.MinimoCompra,Articulo.MinimoVenta,c_ClaveUnidad.Nombre as Unidad,Carrito.IDCarrito,Carrito.usuario,Carrito.IDCaracteristica,Carrito.Precio,Carrito.Cantidad,Caracteristica.Presentacion as Presentacion,Articulo.Descripcion,Articulo.IDArticulo,c_Moneda.Descripcion as Moneda,c_Moneda.IDMoneda as IDMoneda,Carrito.Precio * Carrito.Cantidad as Importe, Carrito.Nota from  Carrito INNER JOIN Caracteristica ON Carrito.IDCaracteristica= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo INNER JOIN c_Moneda ON c_Moneda.IDMoneda = Articulo.IDMoneda INNER JOIN c_ClaveUnidad ON c_ClaveUnidad.IDClaveUnidad = Articulo.IDClaveUnidad where usuario='" + usuario + "'").ToList();
            List<VCarrito> pedido = db.Database.SqlQuery<VCarrito>("select (CarritoC.Precio * (select dbo.GetTipocambio(GETDATE(),CarritoC.IDMoneda,(select IDMoneda from c_Moneda WHERE ClaveMoneda='MXN')))) * CarritoC.Cantidad as preciomex,Articulo.MinimoCompra,Articulo.MinimoVenta,c_ClaveUnidad.Nombre as Unidad,CarritoC.IDCarrito,CarritoC.usuario,CarritoC.IDCaracteristica,CarritoC.Precio,CarritoC.Cantidad,Caracteristica.Presentacion as Presentacion,Articulo.Descripcion,Articulo.IDArticulo,c_Moneda.Descripcion as Moneda,c_Moneda.IDMoneda as IDMoneda,CarritoC.Precio * CarritoC.Cantidad as Importe, CarritoC.Nota from  CarritoC INNER JOIN Caracteristica ON CarritoC.IDCaracteristica= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo INNER JOIN c_Moneda ON c_Moneda.IDMoneda = Articulo.IDMoneda INNER JOIN c_ClaveUnidad ON c_ClaveUnidad.IDClaveUnidad = Articulo.IDClaveUnidad where usuario='" + usuario + "'").ToList();


            ViewBag.carrito = pedido;
            string fecha = DateTime.Now.ToString("yyyy/MM/dd");
            for (int i = 0; i < pedido.Count(); i++)
            {
                VCambio cambio;
                int monedaorigen = ViewBag.carrito[i].IDMoneda;
                try
                {
                    cambio = db.Database.SqlQuery<VCambio>("select dbo.GetTipocambio('" + fecha + "'," + monedaorigen + "," + proveedor.IDMoneda + ") as TC").ToList()[0];
                    db.Database.ExecuteSqlCommand("update [dbo].[CarritoC] set [IDMoneda]=" + proveedor.IDMoneda + ",[Precio]=(select dbo.GetCosto(" + ViewBag.carrito[i].IDArticulo + ",1))*" + cambio.TC + " where IDCarrito=" + ViewBag.carrito[i].IDCarrito + " and usuario=" + usuario + "");
                }
                catch (Exception err)
                {
                    cambio = db.Database.SqlQuery<VCambio>("select dbo.GetTipocambio('" + fecha + "'," + monedaorigen + "," + monedaorigen + ") as TC").ToList()[0];
                    string mensajeerror = err.Message;
                }




            }
            return Json(true);
        }


    }
}
