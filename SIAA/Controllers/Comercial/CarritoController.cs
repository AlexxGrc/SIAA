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
    [Authorize(Roles = "Administrador,Ventas,Sistemas,Almacenista,AdminProduccion, Produccion,Comercial, GerenteVentas,Compras")]
    public class CarritoController : Controller
    {
        private CarritoContext db = new CarritoContext();


        public ActionResult Index(int IDCliente = 0, int IDOficina = 0, string Mensaje ="")
        {
            List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            int usuario = userid.Select(s => s.UserID).FirstOrDefault();

         //   int IDCliente = 0;
            try
            {
              //  IDCliente = int.Parse( coleccion.Get("IDCliente"));
                if (IDCliente>0)
                {
                    actualizaprecio(IDCliente);
                    
                }
                if (IDCliente==0)
                {
                    ClsDatoEntero contarcliente = db.Database.SqlQuery<ClsDatoEntero>("select distinct IDCliente as Dato from Carrito where usuario='" + usuario + "'").ToList().FirstOrDefault();

                    if (contarcliente == null)
                    {

                        IDCliente = 0;
                        ViewBag.ClienteSelecciado = 0;
                    }
                    else
                    {
                        IDCliente = contarcliente.Dato;
                        actualizaprecio(IDCliente);
                        ViewBag.ClienteSelecciado = contarcliente.Dato;
                    }
                }
            }
            catch (Exception err)
            {
                             
               
            }

            ViewBag.Mensaje = Mensaje;
                           
            var elementos = db.Database.SqlQuery<VCarritoV>("select (Carrito.Precio * (select dbo.GetTipocambio(GETDATE(),Carrito.IDMoneda,(select IDMoneda from c_Moneda WHERE ClaveMoneda='MXN')))) * Carrito.Cantidad as preciomex,c_Moneda.ClaveMoneda as Moneda,Carrito.IDMoneda,Carrito.IDCliente, Carrito.IDAlmacen,Articulo.MinimoCompra,Articulo.MinimoVenta,c_ClaveUnidad.Nombre as Unidad,Carrito.IDCarrito,Carrito.usuario,Carrito.IDCaracteristica,Carrito.Precio,Carrito.Cantidad,Caracteristica.Presentacion as Presentacion,Articulo.Descripcion,Articulo.IDArticulo,c_Moneda.Descripcion as Moneda,c_Moneda.IDMoneda as IDMoneda,Carrito.Precio * Carrito.Cantidad as Importe, Carrito.Nota,Carrito.IDAlmacen from  Carrito INNER JOIN Caracteristica ON Carrito.IDCaracteristica= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo INNER JOIN c_Moneda ON c_Moneda.IDMoneda = Carrito.IDMoneda INNER JOIN c_ClaveUnidad ON c_ClaveUnidad.IDClaveUnidad = Articulo.IDClaveUnidad where usuario='" + usuario + "'").ToList();
            ViewBag.VCarritoElementos = elementos;

            var divisa = db.Database.SqlQuery<ResumenFac>("select (select ClaveMoneda FROM C_MONEDA WHERE IDMoneda=Carrito.IDMoneda) as MonedaOrigen, (select SUM(Carrito.Precio * Carrito.Cantidad)) as Subtotal, SUM(Carrito.Precio * Carrito.Cantidad)*0.16 as IVA, (SUM(Carrito.Precio * Carrito.Cantidad)) + (SUM(Carrito.Precio * Carrito.Cantidad)*0.16) as Total ,0 as TotalPesos from Carrito INNER JOIN Caracteristica ON Carrito.IDCaracteristica= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo INNER JOIN c_Moneda ON c_Moneda.IDMoneda = Carrito.IDMoneda INNER JOIN c_ClaveUnidad ON c_ClaveUnidad.IDClaveUnidad = Articulo.IDClaveUnidad where usuario='" + usuario+ "' group by Carrito.IDMoneda").ToList();
            ViewBag.sumatoria = divisa;
                
            List<ArticulosComprados> articulosc = db.Database.SqlQuery<ArticulosComprados>("select  distinct(MovimientoArticulo.Articulo_IDArticulo) as IDArticulo,max(MovimientoArticulo.Fecha) as Fecha, max(MovimientoArticulo.cantidad) as cantidad ,Articulo.Cref as Cref, EncPedido.IDCliente as IDCliente,MovimientoArticulo.Id as IDCaracteristica,Clientes.Nombre as Cliente,Articulo.Descripcion,Caracteristica.Presentacion,Articulo.nameFoto from MovimientoArticulo inner join Articulo on Articulo.IDArticulo=MovimientoArticulo.Articulo_IDArticulo inner join Caracteristica on MovimientoArticulo.Id=Caracteristica.ID join EncPedido on EncPedido.IDPedido=MovimientoArticulo.NoDocumento inner join Clientes on Clientes.IDCliente=EncPedido.IDCliente where MovimientoArticulo.Accion='Pedido' and EncPedido.IDCliente=" + IDCliente + " group by MovimientoArticulo.Articulo_IDArticulo,MovimientoArticulo.Id,Articulo.Cref,EncPedido.IDCliente,Clientes.Nombre,MovimientoArticulo.cantidad,Articulo.Descripcion,Caracteristica.Presentacion,Articulo.nameFoto").ToList();

            ViewBag.articulosc = articulosc;

            ViewBag.IDDOficina = 0;
            if (IDCliente !=0)
            {
                Clientes cc = new ClientesContext().Clientes.Find(IDCliente);
                IDOficina = cc.IDOficina;
                ViewBag.IDDOficina = IDOficina;
            }

            //Almacen de de ajuste
            var almacenS = new OficinaContext().Oficinas.ToList();
            List<SelectListItem> liaS = new List<SelectListItem>();
            liaS.Add(new SelectListItem { Text = "--Selecciona una Oficina--", Value = "0" });
            foreach (var a in almacenS)
            {
                //liaS.Add(new SelectListItem { Text = a.NombreOficina, Value = a.IDOficina.ToString() });
                if (a.IDOficina == IDOficina)
                {
                    liaS.Add(new SelectListItem { Text = a.NombreOficina, Value = a.IDOficina.ToString(), Selected = true });
                }
                else
                {
                    liaS.Add(new SelectListItem { Text = a.NombreOficina, Value = a.IDOficina.ToString() });
                }
            }
            ViewBag.Oficina = liaS;

            ClientesContext prov = new ClientesContext();
            var cliente = prov.Database.SqlQuery<Clientes>("select*from Clientes where status='Activo' order by Nombre").ToList();
            //var cliente = prov.Clientes.OrderBy(s => s.Nombre).ToList();
            List<SelectListItem> li = new List<SelectListItem>();
            li.Add(new SelectListItem { Text = "--Selecciona Cliente--", Value = "0" });

            foreach (var m in cliente)
            {
                if (m.IDCliente == IDCliente)
                {
                    li.Add(new SelectListItem { Text = m.Nombre, Value = m.IDCliente.ToString(),Selected=true });
                }
                else
                {
                    li.Add(new SelectListItem { Text = m.Nombre, Value = m.IDCliente.ToString() });
                }

            }
            if (IDCliente==0)
            {
                ViewBag.InventarioList = getClienteOficina(0);
            }
            else
            {
                ViewBag.InventarioList = li;
            }
               
            return View();
        }
        public ActionResult getJsonClientesvsOficina(int id)
        {
            var inventario = new PRepositoryC().GetClientevsOficina(id);
            Session["Oficina"] = id;
            return Json(inventario, JsonRequestBehavior.AllowGet); ;

        }
        public IEnumerable<SelectListItem> getClienteOficina(int idoficina)
        {
            var inventario = new PRepositoryC().GetClientevsOficina(idoficina);
            Session["Oficina"] = idoficina;
            return inventario;

        } //
        public ActionResult Create()
        {
            return View();
        }

     
        [HttpPost]

        public ActionResult Edit(Carrito carrito)
        {
            if (ModelState.IsValid)
            {
                System.Diagnostics.Debug.WriteLine("Carrito.cantidad"+ carrito.Cantidad);
                System.Diagnostics.Debug.WriteLine("Carrito." + carrito.IDCarrito);
                db.Database.ExecuteSqlCommand("update [dbo].[Carrito] set [Cantidad]='" + carrito.Cantidad + "' where IDCarrito=" + carrito.IDCarrito);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(carrito);
        }
        [HttpPost]
        public JsonResult Edititem(int id, decimal cantidad, string nota, int idalmacen)
        {
            try
            {
                CarritoContext car = new CarritoContext();
                db.Database.ExecuteSqlCommand("update [dbo].[Carrito] set [Cantidad]=" + cantidad + ", [Nota]='" + nota + "', idalmacen=" + idalmacen + "  where IDCarrito=" + id);

         
                List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
                int usuario = userid.Select(s => s.UserID).FirstOrDefault();
                

                List<VCarritoV> pedido = db.Database.SqlQuery<VCarritoV>("select Carrito.IDCliente,Articulo.MinimoCompra,Articulo.MinimoVenta,c_ClaveUnidad.Nombre as Unidad,Carrito.IDCarrito,Carrito.usuario,Carrito.IDCaracteristica,Carrito.Precio,Carrito.Cantidad,Caracteristica.Presentacion as Presentacion,Articulo.Descripcion,Articulo.IDArticulo,c_Moneda.Descripcion as Moneda,c_Moneda.IDMoneda as IDMoneda,Carrito.Precio * Carrito.Cantidad as Importe, Carrito.Nota from  Carrito INNER JOIN Caracteristica ON Carrito.IDCaracteristica= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo INNER JOIN c_Moneda ON c_Moneda.IDMoneda = Articulo.IDMoneda INNER JOIN c_ClaveUnidad ON c_ClaveUnidad.IDClaveUnidad = Articulo.IDClaveUnidad where usuario='" + usuario + "'").ToList();


                ViewBag.carrito = pedido;

                for (int i = 0; i < pedido.Count(); i++)
                {

                    db.Database.ExecuteSqlCommand("update [dbo].[Carrito] set [Precio]=(select dbo.GetPrecio(" + ViewBag.carrito[i].IDCliente + "," + ViewBag.carrito[i].IDArticulo + ",0," + ViewBag.carrito[i].Cantidad + ")) where IDCarrito=" + ViewBag.carrito[i].IDCarrito + " and usuario=" + usuario + "");

                }
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
                car.Database.ExecuteSqlCommand("delete from Carrito where IDCarrito=" + id);

                return Json(true);
            }
            catch(Exception err)
            {
                return Json(500, err.Message);
            }
        }

        public ActionResult getPrecio(int id)
        {
            try
            {
                actualizaprecio(id);
            }
            catch(Exception err)
            {
                string mensajedeerror = err.Message;
            }



            return Json(true);
        }

        public ActionResult AddCarritoGeneral(FormCollection coleccion, string searchString = "")

        {
            ViewBag.searchString = searchString;
            decimal Cantidad = decimal.Parse(coleccion.Get("Cantidad").ToString());
            int id = int.Parse(coleccion.Get("id").ToString());
            int IDCliente = int.Parse(coleccion.Get("IDCliente").ToString());

            string fecha = DateTime.Now.ToString("yyyy/MM/dd");
            ArticuloContext db = new ArticuloContext();
            Caracteristica c = db.Database.SqlQuery<Caracteristica>("select * from Caracteristica where ID=" + id).ToList().FirstOrDefault();
            try
            {
                Articulo articulo = db.Articulo.Find(c.Articulo_IDArticulo);
                //string usuario = System.Web.HttpContext.Current.Session["SessionU"].ToString();
                List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
                int usuario = userid.Select(s => s.UserID).FirstOrDefault();

                
                List<Carrito> numero;
                numero = db.Database.SqlQuery<Carrito>("SELECT * FROM [dbo].[Carrito] where IDCaracteristica=" + id + " and usuario='" + usuario + "'").ToList();
                int consulta = numero.Select(s => s.IDCaracteristica).FirstOrDefault();

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

                if (consulta != 0)
                {


                }
                else
                {
                   
                   
                        

                        VCambio cambio = db.Database.SqlQuery<VCambio>("select dbo.GetTipocambio('" + fecha + "'," + articulo.IDMoneda + "," + articulo.IDMoneda + ") as TC").ToList()[0];
                       string cadena = "insert into carrito (usuario, IDCaracteristica, Cantidad, Precio,IDCliente,IDMoneda,IDAlmacen) values" +
                        " ('" + usuario + "'," + id + ","+Cantidad+",dbo.GetPrecio(" +IDCliente+ "," + c.Articulo_IDArticulo + ",0,"+Cantidad+")*" + cambio.TC + "," + IDCliente + ",'" + articulo.IDMoneda + "',"+ IDAlmacen + ")";





                    db.Database.ExecuteSqlCommand(cadena);
                }

                return RedirectToAction("Index", new { IDCliente = IDCliente, searchString = searchString } );
            }
            catch (Exception err)
            {
                string mensaje = err.Message;
                return RedirectToAction("Index", new { IDCliente = IDCliente, searchString = searchString });
            }
        }

       

        public void actualizaprecio(int id)
        {
            Clientes cliente = new ClientesContext().Clientes.Find(id);

            List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            int usuario = userid.Select(s => s.UserID).FirstOrDefault();

            db.Database.ExecuteSqlCommand("update [dbo].[Carrito] set [IDCliente]=" + id + " where usuario=" + usuario + "");

            //     db.Database.ExecuteSqlCommand("update [dbo].[Carrito] set [IDCliente]=" + id + " where usuario=" + usuario + "");

            List<VCarritoV> pedido = db.Database.SqlQuery<VCarritoV>("select Carrito.IDAlmacen,Carrito.IDCliente,Articulo.MinimoCompra,Articulo.MinimoVenta,c_ClaveUnidad.Nombre as Unidad,Carrito.IDCarrito,Carrito.usuario,Carrito.IDCaracteristica,Carrito.Precio,Carrito.Cantidad,Caracteristica.Presentacion as Presentacion,Articulo.Descripcion,Articulo.IDArticulo,c_Moneda.Descripcion as Moneda,c_Moneda.IDMoneda as IDMoneda,Carrito.Precio * Carrito.Cantidad as Importe, Carrito.Nota from  Carrito INNER JOIN Caracteristica ON Carrito.IDCaracteristica= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo INNER JOIN c_Moneda ON c_Moneda.IDMoneda = Articulo.IDMoneda INNER JOIN c_ClaveUnidad ON c_ClaveUnidad.IDClaveUnidad = Articulo.IDClaveUnidad where usuario='" + usuario + "'").ToList();


            ViewBag.carrito = pedido;


            string fecha = DateTime.Now.ToString("yyyy/MM/dd");
            for (int i = 0; i < pedido.Count(); i++)
            {

                Almacen alma = new AlmacenContext().Almacenes.Find(pedido[i].IDAlmacen);

                Oficina ofi = new OficinaContext().Oficinas.Find(cliente.IDOficina);

                string nombre  = ofi.NombreOficina;
                if (alma.Descripcion.Contains(nombre))
                {
                    //db.Database.ExecuteSqlCommand(" update carrito set idalmacen=0 where IDCarrito=" + pedido[i].IDCarrito);

                }
                else
                {
                    ///pertenece a una oficina distinta
                    ///

                    Almacen almacensi = new AlmacenContext().Database.SqlQuery<Almacen>("select a.*from almacen as a inner join famalm as f on a.idalmacen=f.idalmacen inner join Articulo as ar on ar.idfamilia=f.idfamilia where ar.idarticulo="+ pedido[i].IDArticulo + " and a.descripcion like '%"+ofi.NombreOficina+"%'").ToList().FirstOrDefault();
                    if (almacensi == null)
                    {
                        db.Database.ExecuteSqlCommand(" update carrito set idalmacen=0 where IDCarrito=" + pedido[i].IDCarrito);

                    }
                    else
                    {
                        db.Database.ExecuteSqlCommand(" update carrito set idalmacen=" + almacensi.IDAlmacen + " where IDCarrito=" + pedido[i].IDCarrito);

                    }
                }
                int monedaorigen = ViewBag.carrito[i].IDMoneda;
                VCambio cambio = db.Database.SqlQuery<VCambio>("select dbo.GetTipocambio('" + fecha + "'," + monedaorigen + "," + cliente.IDMoneda + ") as TC").ToList().FirstOrDefault();
                int Monedadestino = monedaorigen;

                try
                {
                    string cadenam = " select * from MatrizPrecioCliente where IDArticulo = " + ViewBag.carrito[i].IDArticulo + " and IDCliente = " + id;
                    MatrizPrecioCliente Moneaveri = new CarritoContext().Database.SqlQuery<MatrizPrecioCliente>(cadenam).ToList().FirstOrDefault();
                    Monedadestino = Moneaveri.IDMoneda;
                }
                catch (Exception err)
                {

                }

                string cadenaprecio = "select dbo.GetPrecio(" + id + "," + ViewBag.carrito[i].IDArticulo + ",0," + ViewBag.carrito[i].Cantidad + ") as Dato";
                ClsDatoDecimal precio = new CarritoContext().Database.SqlQuery<ClsDatoDecimal>(cadenaprecio).ToList().FirstOrDefault();
                //if (monedaorigen == Monedadestino)
                //{
                //    db.Database.ExecuteSqlCommand("update [dbo].[Carrito] set [Precio]=" + precio.Dato + "  where IDCarrito=" + ViewBag.carrito[i].IDCarrito + " and usuario=" + usuario + "");
                //}
                //else
                //{
                //db.Database.ExecuteSqlCommand("update [dbo].[Carrito] set [Precio]=" + precio.Dato * cambio.TC + ",IDMoneda=" +Monedadestino+"  where IDCarrito=" + ViewBag.carrito[i].IDCarrito + " and usuario=" + usuario + "");
                //}
                db.Database.ExecuteSqlCommand("update [dbo].[Carrito] set [Precio]=" + precio.Dato + ",IDMoneda=" + Monedadestino + "  where IDCarrito=" + ViewBag.carrito[i].IDCarrito + " and usuario=" + usuario + "");

            }/// 
        }


    }
}
public class PRepositoryC
{

    public IEnumerable<SelectListItem> GetClientevsOficina(int? IDOficina)
    {
        List<SelectListItem> lista;
        if (IDOficina == 0)
        {
            lista = new List<SelectListItem>();
            lista.Add(new SelectListItem() { Value = "0", Text = "Elige una Oficina primero" });
            return (lista);
        }
        using (var context = new VInventarioAlmacenContext())
        {
            string cadenasql = "Select c.* from Clientes as c Inner join Oficina as o on c.idoficina=o.idoficina where c.status='Activo' and  o.idoficina="+ IDOficina;
            lista = context.Database.SqlQuery<Clientes>(cadenasql).ToList()

                .OrderBy(n => n.Nombre)
                    .Select(n =>
                    new SelectListItem
                    {
                        Value = n.IDCliente.ToString(),
                        Text = n.Nombre
                    }).ToList();
            var countrytip = new SelectListItem()
            {
                Value = "0",
                Text = "--- Selecciona un Cliente---"
            };
            lista.Insert(0, countrytip);
            return new SelectList(lista, "Value", "Text");
        }

    }

}