using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using SIAAPI.Models.Login;
using SIAAPI.Models.Suajes;
using SIAAPI.Models.Comercial;
using SIAAPI.Models.Inventarios;
using SIAAPI.Models.PlaneacionProduccion;
using SIAAPI.ViewModels.Comercial;

namespace SIAAPI.Controllers.Suajes
{
    public class SuajeController : Controller
    {
        private AjustesAlmacenContext db = new AjustesAlmacenContext();

        private VBitacoraSuajesContext dbSuaje = new VBitacoraSuajesContext();
        // GET: Suaje
        public ActionResult Index()
        {
            return View();
        }

      

        public ActionResult BitacoraSuajes(int? page, int? PageSize, string Fechainicio="", string Fechafinal="", string clave = "")
        {
            string CadSql = " ";

            //if (clave =="")
            //{
            //    CadSql = "SELECT AProduccion.IDOrden, OProduccion.FechaCreacion, OProduccion.IDModeloProduccion, LOProduccion.Cantidad, Art.IDArticulo, Cli.Nombre, OProduccion.EstadoOrden, Art.cref, OProduccion.IDCaracteristica FROM ArticuloProduccion as AProduccion INNER JOIN OrdenProduccion as OProduccion on AProduccion.IDOrden = OProduccion.IDOrden INNER JOIN LiberaOrdenProduccion as LOProduccion on LOProduccion.IDOrden = OProduccion.IDOrden INNER JOIN Articulo as Art on Art.IDArticulo = AProduccion.IDArticulo INNER JOIN Clientes as Cli on Cli.IDCliente = OProduccion.IDCliente where Art.cref =  '" + clave + "'and  (FechaCreacion BETWEEN  '" + Fechainicio + "' and '" + Fechafinal + "')";

            //}
            //else
            //{
                //Las Fechas de que manera se acomodaran para ahcer las consultas segun la BD
                CadSql = "SELECT AProduccion.IDOrden, OProduccion.FechaCreacion, OProduccion.IDModeloProduccion, LOProduccion.Cantidad, Art.IDArticulo, Cli.Nombre, OProduccion.EstadoOrden, Art.cref, OProduccion.IDCaracteristica FROM ArticuloProduccion as AProduccion INNER JOIN OrdenProduccion as OProduccion on AProduccion.IDOrden = OProduccion.IDOrden INNER JOIN LiberaOrdenProduccion as LOProduccion on LOProduccion.IDOrden = OProduccion.IDOrden INNER JOIN Articulo as Art on Art.IDArticulo = AProduccion.IDArticulo INNER JOIN Clientes as Cli on Cli.IDCliente = OProduccion.IDCliente where (Art.IDFamilia=80 or Art.IDFamilia=81 or  Art.IDFamilia=91 or Art.IDFamilia=75 or Art.IDFamilia=71 or Art.IDFamilia=93 or Art.IDFamilia=72 or Art.IDFamilia=11 or Art.IDFamilia=69  or Art.IDFamilia=70  or Art.IDFamilia=95) and   Art.cref =  '" + clave + "'and  (OProduccion.FechaCreacion BETWEEN  '" + Fechainicio + "' and '" + Fechafinal + "') and (OProduccion.EstadoOrden<>'Cancelada') order by OProduccion.FechaCreacion desc";

            //}
            if (Fechainicio =="" && Fechafinal =="")
            {
                CadSql = "SELECT AProduccion.IDOrden, OProduccion.FechaCreacion, OProduccion.IDModeloProduccion, LOProduccion.Cantidad, Art.IDArticulo, Cli.Nombre, OProduccion.EstadoOrden, Art.cref, OProduccion.IDCaracteristica FROM ArticuloProduccion as AProduccion INNER JOIN OrdenProduccion as OProduccion on AProduccion.IDOrden = OProduccion.IDOrden INNER JOIN LiberaOrdenProduccion as LOProduccion on LOProduccion.IDOrden = OProduccion.IDOrden INNER JOIN Articulo as Art on Art.IDArticulo = AProduccion.IDArticulo INNER JOIN Clientes as Cli on Cli.IDCliente = OProduccion.IDCliente where (Art.IDFamilia=80 or Art.IDFamilia=81 or  Art.IDFamilia=91 or Art.IDFamilia=75 or Art.IDFamilia=71 or Art.IDFamilia=93 or Art.IDFamilia=72 or Art.IDFamilia=11 or Art.IDFamilia=69  or Art.IDFamilia=70  or Art.IDFamilia=95) and   Art.cref =  '" + clave + "' and (OProduccion.EstadoOrden<>'Cancelada') order by OProduccion.FechaCreacion desc";

            }

            List<VBitacoraSuajes> elementos = dbSuaje.Database.SqlQuery<VBitacoraSuajes>(CadSql).ToList();

            int count = elementos.Count;

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
            ViewBag.Clave = clave;
            ViewBag.FechaInicio = Fechainicio;
            ViewBag.FechaFin = Fechafinal;


            return View(elementos.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult ValeSuaje(string currentFilter, string searchString, int Almacen = 0, int? IDFamilia = 0)
        {
            if (searchString == null)
            {
                searchString = currentFilter;
            }

            // Pass filtering string to view in order to maintain filtering when paging
            ViewBag.SearchString = searchString;
            ViewBag.IDFamilia = new FamAlmRepository().GetAlmacenesF(Almacen);
            ViewBag.Almacen = new AlmacenRepository().GetAlmacenes();
            ViewBag.IDAlmacen = Almacen;
            ViewBag.Familia = IDFamilia;
            ViewBag.Almacenseleccionado = Almacen;/// mandar el viewbag el parametro que viene de la pagina anterior
            List<VInventarioAlmacen> almacen = new List<VInventarioAlmacen>();
            string cadena1 = "";
            if (Almacen != 0)
            {
                cadena1 = "select Top 50 * from VInventarioAlmacen as i inner join Articulo as a on i.idarticulo=a.idarticulo where  (a.idfamilia=80 or a.idfamilia=91 or a.idfamilia=81 or a.idfamilia=75 or a.idfamilia=71 or a.idfamilia=76 or a.idfamilia=72 or a.idfamilia=93 or a.idfamilia=11 or a.idfamilia=69) and i.IDAlmacen=" + Almacen;

                if (IDFamilia != 0)
                {
                    cadena1 = "select Top 50 * from VInventarioAlmacen as i inner join Articulo as a on i.idarticulo=a.idarticulo where  i.IDAlmacen=" + Almacen + " and a.IDFamilia=" + IDFamilia;

                }
                if (!string.IsNullOrEmpty(searchString) && IDFamilia != 0)
                {
                    cadena1 = "select Top 50 * from VInventarioAlmacen as i inner join Articulo as a on i.idarticulo=a.idarticulo where  i.IDAlmacen=" + Almacen + " and a.IDFamilia=" + IDFamilia + " and a.Cref like '%" + searchString + " %'";

                }
                if (!string.IsNullOrEmpty(searchString) && IDFamilia == 0)
                {
                    cadena1 = "select Top 50 * from VInventarioAlmacen as i inner join Articulo as a on i.idarticulo=a.idarticulo where  i.IDAlmacen=" + Almacen + " and a.Cref like '%" + searchString + "%'";

                }
                almacen = db.Database.SqlQuery<VInventarioAlmacen>(cadena1).ToList();
                ViewBag.Registros = almacen.Count();

                return View(almacen);
            }

            return View();

        }
        public ActionResult AddCarritoGeneral(FormCollection coleccion, string searchString = "")

        {
            ViewBag.searchString = searchString;
            decimal Cantidad = decimal.Parse(coleccion.Get("Cantidad").ToString());
            int id = int.Parse(coleccion.Get("id").ToString());
            int IDAlmacen = int.Parse(coleccion.Get("Almacen").ToString());

            string fecha = DateTime.Now.ToString("yyyy/MM/dd");
            ArticuloContext db = new ArticuloContext();
            Caracteristica c = db.Database.SqlQuery<Caracteristica>("select * from Caracteristica where ID=" + id).ToList().FirstOrDefault();
            try
            {
                Articulo articulo = db.Articulo.Find(c.Articulo_IDArticulo);
                //string usuario = System.Web.HttpContext.Current.Session["SessionU"].ToString();
                List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
                int usuario = userid.Select(s => s.UserID).FirstOrDefault();

                int numero = db.Database.SqlQuery<CarritoVale>("SELECT * FROM [dbo].[CarritoValeSuaje] where IDCaracteristica='" + id + "' and usuario='" + usuario + "'").ToList().Count;



                if (numero != 0)
                {
                    ViewBag.errorusuario = "INTENTASTE METER DOS VECES EL MISMO ARTICULO CON LA MISMA PRESENTACION";

                }
                else
                {
                    string cadena = "";
                 

                    cadena = "insert into CarritoValeSuaje (usuario, IDCaracteristica, Cantidad, Precio, IDAlmacen) values ('" + usuario + "'," + id + "," + Cantidad + ",dbo.GetPrecio(0," + c.Articulo_IDArticulo + ",0," + Cantidad + "), " + IDAlmacen + ")";



                    db.Database.ExecuteSqlCommand(cadena);
                }

                return RedirectToAction("ValeSuaje", new { searchString = ViewBag.searchString, Almacen = IDAlmacen });
            }
            catch (Exception err)
            {
                string mensaje = err.Message;
                return RedirectToAction("ValeSuaje", new { searchString = ViewBag.searchString, Almacen = IDAlmacen });
            }
        }

        public ActionResult CrearValeSuaje(int IDAlmacen)
        {


            List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            int usuario = userid.Select(s => s.UserID).FirstOrDefault();


            CarritoContext car = new CarritoContext();

            ViewBag.IDAlmacen = IDAlmacen;



            List<VCarritoValeSuaje> pedido = db.Database.SqlQuery<VCarritoValeSuaje>("select c.Precio,c.IDCarritoValeSuaje,c.usuario,c.IDAlmacen, c.IDCaracteristica,c.Cantidad,c_ClaveUnidad.Nombre as Unidad, c_Moneda.ClaveMoneda as Moneda, Articulo.Cref as Cref, Caracteristica.Presentacion as Presentacion,Articulo.Descripcion,Articulo.IDArticulo,(c.Precio * c.Cantidad) as Importe from  CarritoValeSuaje as c INNER JOIN Caracteristica ON c.IDCaracteristica= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo INNER JOIN c_Moneda ON c_Moneda.IDMoneda =Articulo.IDMoneda INNER JOIN  c_ClaveUnidad ON c_ClaveUnidad.IDClaveUnidad = Articulo.IDClaveUnidad where usuario='" + usuario + "'").ToList();

        
            ViewBag.carrito = pedido;


            ClsDatoEntero c = db.Database.SqlQuery<ClsDatoEntero>("select count(IDCarritoValeSuaje) as Dato from CarritoValeSuaje" +
                " where  usuario='" + usuario + "'").ToList().FirstOrDefault();
            int x = c.Dato;
            ViewBag.dato = x;

            ClsDatoEntero cantidad = db.Database.SqlQuery<ClsDatoEntero>("select count(Cantidad) as Dato from CarritoValeSuaje where Cantidad=0 and usuario='" + usuario + "'").ToList().FirstOrDefault();
            ViewBag.datocantidad = cantidad.Dato;

            ClsDatoEntero preciocontar = db.Database.SqlQuery<ClsDatoEntero>("select count(Precio) as Dato from CarritoValeSuaje where Precio=0 and usuario='" + usuario + "'").ToList().FirstOrDefault();
            ViewBag.datoprecio = preciocontar.Dato;


            List<ValidarCarrito> validaprecio = db.Database.SqlQuery<ValidarCarrito>("select c.Precio, dbo.GetValidaCosto(Articulo.IDArticulo, c.Cantidad, c.Precio) as Dato from CarritoValeSuaje as c INNER JOIN Caracteristica ON c.IDCaracteristica= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo  where c.usuario='" + usuario + "'").ToList();
            int countDato = validaprecio.Count(s => s.Dato.Equals(true));

            int countCarrito = pedido.Count();

            if (countDato == countCarrito)
            {
                ViewBag.validacarrito = 1;
            }
            else
            {
                ViewBag.validacarrito = 0;
            }
            //Termina la consulta del carrito
            return View();
        }

        [HttpPost]
        public ActionResult CrearValeSuaje(ValeSuaje vale, FormCollection coleccion)
        {

            string seleccion = coleccion.Get("select");
            string precioP = coleccion.Get("Precio");
            decimal subtotal = 0, iva = 0, total = 0, subtotalr = 0, ivar = 0, totalr = 0, importetotal = 0, importe = 0, importeiva = 0;
            List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            int usuario = userid.Select(s => s.UserID).FirstOrDefault();
            List<VCarritoValeSuaje> pedido = db.Database.SqlQuery<VCarritoValeSuaje>("select c.Precio,c.IDCarritoValeSuaje,c.usuario,c.IDAlmacen, c.IDCaracteristica,c.Cantidad,c_ClaveUnidad.Nombre as Unidad, c_Moneda.ClaveMoneda as Moneda, Articulo.Cref as Cref, Caracteristica.Presentacion as Presentacion,Articulo.Descripcion,Articulo.IDArticulo,(c.Precio * c.Cantidad) as Importe from  CarritoValeSuaje as c INNER JOIN Caracteristica ON c.IDCaracteristica= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo INNER JOIN c_Moneda ON c_Moneda.IDMoneda =Articulo.IDMoneda INNER JOIN  c_ClaveUnidad ON c_ClaveUnidad.IDClaveUnidad = Articulo.IDClaveUnidad where usuario='" + usuario + "'").ToList();

            try
            {
                foreach (var det in pedido)
                {
                    Articulo articulokit = new ArticuloContext().Articulo.Find(det.IDArticulo);

                    InventarioAlmacen ia = new InventarioAlmacenContext().InventarioAlmacenes.ToList().Where(s => s.IDAlmacen == det.IDAlmacen && s.IDCaracteristica == det.IDCaracteristica).FirstOrDefault();

                    if (ia == null)
                    {

                        ClsDatoString almacen = db.Database.SqlQuery<ClsDatoString>("select Descripcion as Dato from Almacen where IDAlmacen ='" + det.IDAlmacen + "'").ToList()[0];



                        throw new Exception("No hay Existencia suficiente almacen para " + articulokit.Cref + " en la cantidad de " + det.Cantidad);
                    }
                    if (ia.Existencia < det.Cantidad)
                    {
                        throw new Exception("No hay Existencia suficiente almacen para " + articulokit.Cref + " en la cantidad de " + det.Cantidad);
                    }

                }
            }
            catch (Exception err)
            {

            }


            try
            {
                subtotal = pedido.Sum(s => s.Importe);
                iva = subtotal * (decimal)0.16;
                total = subtotal + iva;
            }
            catch (Exception e)
            {

            }
            ViewBag.Subtotal = subtotal.ToString("C");
            ViewBag.IVA = iva.ToString("C");
            ViewBag.Total = total.ToString("C");
            ViewBag.carrito = pedido;
            //Termina 

            if (ModelState.IsValid)
            {
                int num = 0;
                DateTime fecha = DateTime.Now;
                string fecha1 = fecha.ToString("yyyy/MM/dd");



                db.Database.ExecuteSqlCommand("INSERT INTO [dbo].[ValeSuaje]([Fecha],[IDAlmacen],[UserID],[Observacion],[Entregar],[Concepto],[Solicito], [Estado], TipoOperacion) values ('" + fecha1 + "','" + vale.IDAlmacen + "','" + usuario + "','" + vale.Observacion + "','" + vale.Entregar + "','" + vale.Concepto + "','" + vale.Solicito + "','Inactivo', '"+vale.TipoOVale+"')");
                
                db.SaveChanges();


                List<ValeSuaje> numero;
                numero = db.Database.SqlQuery<ValeSuaje>("SELECT * FROM [dbo].[ValeSuaje] WHERE IDValeSuaje = (SELECT MAX(IDValeSuaje) from ValeSuaje)").ToList();
                num = numero.Select(s => s.IDValeSuaje).FirstOrDefault();

                //   string cadenafinal = "";
                int contador = 0;
                string[] arraydatos1 = precioP.Split(',');
                string[] arraydatosMotivo = seleccion.Split(',');

                foreach (var det in pedido)
                {
                    decimal Precio = decimal.Parse(arraydatos1[contador]);
                    string Motivo = arraydatosMotivo[contador];
                    Articulo articulo = new ArticuloContext().Articulo.Find(det.IDArticulo);

                    importe = det.Cantidad * Precio;
                    importeiva = importe * decimal.Parse(SIAAPI.Properties.Settings.Default.ValorIVA);
                    importetotal = importeiva + importe;
                    db.Database.ExecuteSqlCommand("INSERT INTO DetValeSuaje([IDValeSuaje],[IDArticulo],[Moneda],[IDCaracteristica],[Cantidad],[IDAlmacen],[Precio],[Importe], Motivo) values ('" + num + "','" + det.IDArticulo + "','" + det.Moneda + "','" + det.IDCaracteristica + "','" + det.Cantidad + "','" + det.IDAlmacen + "','" + Precio + "','" + importe + "' ,'"+ Motivo+"')");

                    db.Database.ExecuteSqlCommand("delete from CarritoValeSuaje where IDCarritoValeSuaje='" + det.IDCarritoValeSuaje + "'");
                    db.SaveChanges();

                    contador++;
                }

                return RedirectToAction("ValesSuaje");
               


            }



            return View(vale);

        }

        public ActionResult ValesSuajes(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
        {
            ViewBag.CurrentSort = sortOrder;
            // Not sure here
            if (searchString == null)
            {
                searchString = currentFilter;
            }

            // Pass filtering string to view in order to maintain filtering when paging
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
            ValeSuajeContext dl = new ValeSuajeContext();
            ViewBag.CurrentFilter = searchString;
            DateTime fechaini = DateTime.Now.AddDays(-60);
            //Paginación
            string cadena;
            cadena = "select * from ValeSuaje order by IDValeSuaje desc";

            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                cadena = "select * from ValeSuaje where  IDValeSuaje='" + searchString + "'";



            }
            List<VValeSuaje> elementos = dl.Database.SqlQuery<VValeSuaje>(cadena).ToList<VValeSuaje>();

            //Ordenacion


            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = elementos.Count(); // Total number of elements

            // Populate DropDownList
            ViewBag.PageSize = new List<SelectListItem>()
            {
                new SelectListItem { Text = "10", Value = "10", Selected = true },
                new SelectListItem { Text = "25", Value = "25" },
                new SelectListItem { Text = "50", Value = "50" },
                new SelectListItem { Text = "100", Value = "100" },
                new SelectListItem { Text = "Todos", Value = count.ToString() }
             };

            int pageNumber = (page ?? 1);
            int pageSize = (PageSize ?? 10);
            ViewBag.psize = pageSize;

            return View(elementos.ToPagedList(pageNumber, pageSize));
            //Paginación
            //return View(db.AjustesAlmacenes.ToList());
        }
        public ActionResult AutorizarVale(int? id)
        {
            try
            {
                db.Database.ExecuteSqlCommand("update ValeSuaje set estado='Activo' where IDValeSuaje=" + id);
            }
            catch (Exception err)
            {

            }
            List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            int usuario = userid.Select(s => s.UserID).FirstOrDefault();

            List<DetValeSuaje> vale = db.Database.SqlQuery<DetValeSuaje>("select * from DetValeSuaje where IDValeSuaje= " + id).ToList();

            foreach (var det in vale)
            {
                Articulo articulo = new ArticuloContext().Articulo.Find(det.IDArticulo);
                ValeSuaje suaje = new ValeSuajeContext().valeSuajes.Find(id);
                if (suaje.TipoOperacion =="Entrada")
                {
                    try
                    {
                        if (articulo.CtrlStock)
                        {
                            Caracteristica carateristica = new ValeSalidaContext().Database.SqlQuery<Caracteristica>("select * from caracteristica where id=" + det.IDCaracteristica).ToList().FirstOrDefault();
                            try
                            {
                                string cadenapro = "UPDATE dbo.InventarioAlmacen SET Existencia = (Existencia+" + det.Cantidad + ") WHERE IDAlmacen = " + det.IDAlmacen + " and IDCaracteristica =" + det.IDCaracteristica + "";
                                db.Database.ExecuteSqlCommand(cadenapro);
                                db.Database.ExecuteSqlCommand("UPDATE dbo.InventarioAlmacen SET Disponibilidad=(existencia-apartado) where IDAlmacen=" + det.IDAlmacen + " and idcaracteristica=" + det.IDCaracteristica);

                            }
                            catch (Exception err)
                            {

                            }
                            try
                            {
                                InventarioAlmacen invO = new InventarioAlmacenContext().InventarioAlmacenes.ToList().Where(s => s.IDAlmacen == det.IDAlmacen && s.IDCaracteristica == det.IDCaracteristica).FirstOrDefault();

                                string cadenam = "INSERT INTO [dbo].[MovimientoArticulo] ([Fecha],[Id],[IDPresentacion],[Articulo_IDArticulo],[Accion],[cantidad],[Documento],[NoDocumento],[Lote],[IDAlmacen],[TipoOperacion],[acumulado],[observacion],[Hora], Usuario) VALUES ";
                                cadenam += " (getdate(), " + carateristica.ID + "," + carateristica.IDPresentacion + "," + det.IDArticulo + ",'Vale de salida',";
                                cadenam += det.Cantidad + ",'Vale de Entrada Suaje '," + id + ",'',";
                                cadenam += det.IDAlmacen + ",'E'," + invO.Existencia + ",'Autorización vale de Entrada', CONVERT (time, SYSDATETIMEOFFSET())," + usuario + ")";

                                db.Database.ExecuteSqlCommand(cadenam);
                            }
                            catch (Exception err)
                            {

                            }
                        }

                    }
                    catch (Exception err)
                    {

                    }
                }
                if (suaje.TipoOperacion == "Salida")
                {
                    try
                    {
                        if (articulo.CtrlStock)
                        {
                            Caracteristica carateristica = new ValeSalidaContext().Database.SqlQuery<Caracteristica>("select * from caracteristica where id=" + det.IDCaracteristica).ToList().FirstOrDefault();
                            try
                            {
                                string cadenapro = "UPDATE dbo.InventarioAlmacen SET Existencia = (Existencia- " + det.Cantidad + ") WHERE IDAlmacen = " + det.IDAlmacen + " and IDCaracteristica =" + det.IDCaracteristica + "";
                                db.Database.ExecuteSqlCommand(cadenapro);
                                db.Database.ExecuteSqlCommand("UPDATE dbo.InventarioAlmacen SET Disponibilidad=(existencia-apartado) where IDAlmacen=" + det.IDAlmacen + " and idcaracteristica=" + det.IDCaracteristica);

                            }
                            catch (Exception err)
                            {

                            }
                            try
                            {
                                InventarioAlmacen invO = new InventarioAlmacenContext().InventarioAlmacenes.ToList().Where(s => s.IDAlmacen == det.IDAlmacen && s.IDCaracteristica == det.IDCaracteristica).FirstOrDefault();

                                string cadenam = "INSERT INTO [dbo].[MovimientoArticulo] ([Fecha],[Id],[IDPresentacion],[Articulo_IDArticulo],[Accion],[cantidad],[Documento],[NoDocumento],[Lote],[IDAlmacen],[TipoOperacion],[acumulado],[observacion],[Hora], Usuario) VALUES ";
                                cadenam += " (getdate(), " + carateristica.ID + "," + carateristica.IDPresentacion + "," + det.IDArticulo + ",'Vale de salida',";
                                cadenam += det.Cantidad + ",'Vale de salida Suaje '," + id + ",'',";
                                cadenam += det.IDAlmacen + ",'S'," + invO.Existencia + ",'Autorización vale de salida Suaje', CONVERT (time, SYSDATETIMEOFFSET())," + usuario + ")";

                                db.Database.ExecuteSqlCommand(cadenam);
                            }
                            catch (Exception err)
                            {

                            }
                        }

                    }
                    catch (Exception err)
                    {

                    }
                }
                if (suaje.TipoOperacion == "Reposición")
                {
                    try
                    {
                        if (articulo.CtrlStock)
                        {
                            Caracteristica carateristica = new ValeSalidaContext().Database.SqlQuery<Caracteristica>("select * from caracteristica where id=" + det.IDCaracteristica).ToList().FirstOrDefault();
                            try
                            {
                                string cadenapro = "UPDATE dbo.InventarioAlmacen SET Existencia = (Existencia- " + det.Cantidad + ") WHERE IDAlmacen = " + det.IDAlmacen + " and IDCaracteristica =" + det.IDCaracteristica + "";
                                db.Database.ExecuteSqlCommand(cadenapro);
                                db.Database.ExecuteSqlCommand("UPDATE dbo.InventarioAlmacen SET Disponibilidad=(existencia-apartado) where IDAlmacen=" + det.IDAlmacen + " and idcaracteristica=" + det.IDCaracteristica);

                            }
                            catch (Exception err)
                            {

                            }
                            try
                            {
                                InventarioAlmacen invO = new InventarioAlmacenContext().InventarioAlmacenes.ToList().Where(s => s.IDAlmacen == det.IDAlmacen && s.IDCaracteristica == det.IDCaracteristica).FirstOrDefault();

                                string cadenam = "INSERT INTO [dbo].[MovimientoArticulo] ([Fecha],[Id],[IDPresentacion],[Articulo_IDArticulo],[Accion],[cantidad],[Documento],[NoDocumento],[Lote],[IDAlmacen],[TipoOperacion],[acumulado],[observacion],[Hora], Usuario) VALUES ";
                                cadenam += " (getdate(), " + carateristica.ID + "," + carateristica.IDPresentacion + "," + det.IDArticulo + ",'Vale de salida',";
                                cadenam += det.Cantidad + ",'Vale de salida '," + id + ",'',";
                                cadenam += det.IDAlmacen + ",'S'," + invO.Existencia + ",'Autorización vale de salida', CONVERT (time, SYSDATETIMEOFFSET())," + usuario + ")";

                                db.Database.ExecuteSqlCommand(cadenam);
                            }
                            catch (Exception err)
                            {

                            }
                        }

                    }
                    catch (Exception err)
                    {

                    }
                }
                
            }

            return RedirectToAction("ValesDeSalida");
        }

        public ActionResult ValesDeSuajes(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.DateSortParm = sortOrder == "FechaAjuste" ? "date_desc" : "Date";
            ViewBag.AlmacenSortParm = String.IsNullOrEmpty(sortOrder) ? "IDAlmacen" : "IDAlmacen";
            ViewBag.ArticuloSortParm = String.IsNullOrEmpty(sortOrder) ? "IDArticulo" : "IDArticulo";
            ViewBag.CaracteristicaSortParm = String.IsNullOrEmpty(sortOrder) ? "IDCaracteristica" : "IDCaracteristica";
            // Not sure here
            if (searchString == null)
            {
                searchString = currentFilter;
            }

            // Pass filtering string to view in order to maintain filtering when paging
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
            ValeSalidaContext dl = new ValeSalidaContext();
            ViewBag.CurrentFilter = searchString;
            DateTime fechaini = DateTime.Now.AddDays(-60);
            //Paginación
            string cadena;
            cadena = "select * from ValeSuaje order by IDValeSuaje desc";

            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                cadena = "select * from ValeSuaje where  IDValeSuaje='" + searchString + "'";



            }
            List<VValeSuaje> elementos = dl.Database.SqlQuery<VValeSuaje>(cadena).ToList<VValeSuaje>();

            //Ordenacion


            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = elementos.Count(); // Total number of elements

            // Populate DropDownList
            ViewBag.PageSize = new List<SelectListItem>()
            {
                new SelectListItem { Text = "10", Value = "10", Selected = true },
                new SelectListItem { Text = "25", Value = "25" },
                new SelectListItem { Text = "50", Value = "50" },
                new SelectListItem { Text = "100", Value = "100" },
                new SelectListItem { Text = "Todos", Value = count.ToString() }
             };

            int pageNumber = (page ?? 1);
            int pageSize = (PageSize ?? 10);
            ViewBag.psize = pageSize;

            return View(elementos.ToPagedList(pageNumber, pageSize));
            //Paginación
            //return View(db.AjustesAlmacenes.ToList());
        }

        public ActionResult DetailsVales(int? id)
        {

            List<VDetValeSalida> pedido = db.Database.SqlQuery<VDetValeSalida>("select Det.Precio,Det.IDDetValeSuaje, Det.IDAlmacen, Det.IDCaracteristica,Det.Cantidad,c_ClaveUnidad.Nombre as Unidad, c_Moneda.ClaveMoneda as Moneda, Articulo.Cref as Cref, Caracteristica.Presentacion as Presentacion,Articulo.Descripcion,Articulo.IDArticulo,(Det.Precio * Det.Cantidad )as Importe from  DetValeSalida INNER JOIN Caracteristica ON DetValeSalida.IDCaracteristica= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo INNER JOIN c_Moneda ON c_Moneda.IDMoneda =Articulo.IDMoneda INNER JOIN  c_ClaveUnidad ON c_ClaveUnidad.IDClaveUnidad = Articulo.IDClaveUnidad where DetValeSalida.IDValeSalida=" + id).ToList();

            ViewBag.req = pedido;


          
            ValeSalida valeSalida = new ValeSalidaContext().ValeSalida.Find(id);
            if (valeSalida == null)
            {
                return HttpNotFound();
            }
            return View(valeSalida);
        }



    }
}
public class FamAlmRepository
{
    public IEnumerable<SelectListItem> GetAlmacenesF(int IDAlmacen)
    {

        List<SelectListItem> lista;
        using (var context = new FamiliaContext())
        {
            string cadenasql = "select*from Familia as f inner join FamAlm as fa on f.idfamilia=fa.idfamilia where (f.idfamilia=80 or f.idfamilia=91 or f.idfamilia=81 or f.idfamilia=75 or f.idfamilia=71 or f.idfamilia=76 or f.idfamilia=72 or f.idfamilia=93 or f.idfamilia=11 or f.idfamilia=69) and fa.idalmacen=" + IDAlmacen;
            lista = context.Database.SqlQuery<Familia>(cadenasql).ToList().OrderBy(n => n.Descripcion)
                    .Select(n =>
                    new SelectListItem
                    {
                        Value = n.IDFamilia.ToString(),
                        Text = n.Descripcion
                    }).ToList();
            var descripciontip = new SelectListItem()
            {
                Value = "0",
                Text = "--- Selecciona una familia ---"
            };
            lista.Insert(0, descripciontip);
            return new SelectList(lista, "Value", "Text");
        }
    }

}
