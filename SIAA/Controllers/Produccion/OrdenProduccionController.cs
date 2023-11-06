using Automatadll;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using PagedList;
using SIAAPI.ClasesProduccion;
using SIAAPI.Models.Administracion;
using SIAAPI.Models.Calidad;
using SIAAPI.Models.Comercial;
using SIAAPI.Models.Login;
using SIAAPI.Models.PlaneacionProduccion;
using SIAAPI.Models.Produccion;
using SIAAPI.Reportes;
using SIAAPI.ViewModels.Articulo;
using SIAAPI.ViewModels.Comercial;
using SIAAPI.ViewModels.produccion;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Serialization;

namespace SIAAPI.Controllers.Produccion
{
    [Authorize(Roles = "Administrador,Compras,AdminProduccion, Produccion,GsetionCalidad, Sistemas, Almacenista, Ventas, BitacoraProduccion,Bitacora,Calidad,Comercial,GerenteVentas, Gerencia, GestionCalidad,MesaControl")]
    public class OrdenProduccionController : Controller
    {
        private OrdenProduccionContext db = new OrdenProduccionContext();
        public ActionResult Index(string sortOrder, string currentFilter, string Fechainicio, string Fechafinal, string searchString, int? page, int? PageSize, int? idproceso)
        {
            
            bool MostrarOrdenerSuma = false;
            ViewBag.Vista = MostrarOrdenerSuma;
            ViewBag.Ingresadas = "";
            ViewBag.Producidas = "";
            ViewBag.CurrentSort = sortOrder;
            ViewBag.ASortParm = String.IsNullOrEmpty(sortOrder) ? "IDOrden" : "IDOrden";
            // Not sure here
            if (searchString == null)
            {
                searchString = currentFilter;
            }

            // Pass filtering string to view in order to maintain filtering when paging
            ViewBag.SearchString = searchString;
            //Paginación
            var elementos = from s in db.OrdenesProduccion.OrderByDescending(s => s.IDOrden)
                            select s;


            //Ordenacion

            switch (sortOrder)
            {
                case "IDOrden":
                    elementos = elementos.OrderByDescending(s => s.IDOrden);
                    break;

                default:
                    elementos = elementos.OrderByDescending(s => s.IDOrden);
                    break;
            }
            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                elementos = elementos.Where(s => s.IDOrden.ToString().Contains(searchString) || s.Descripcion.Contains(searchString) || s.Presentacion.Contains(searchString));
            }

          

            if (!String.IsNullOrEmpty(Fechainicio) && !String.IsNullOrEmpty(Fechafinal)) //pusieron una fecha
            {
                DateTime fechai = DateTime.Parse(Fechainicio);
                DateTime fechaf = DateTime.Parse(Fechafinal);
                MostrarOrdenerSuma = true;
                int OrdenesIngresadas = db.OrdenesProduccion.Where(e => (e.EstadoOrden!="Cancelada" && e.EstadoOrden!="Conflicto") && (e.FechaCreacion >= fechai && e.FechaCreacion <= fechaf)).Count(); // Total number of elements
                int OrdenesProducidas = db.OrdenesProduccion.Where(e => e.EstadoOrden=="Finalizada" && (e.FechaCreacion >= fechai && e.FechaCreacion <= fechaf)).Count(); // Total number of elements
                ViewBag.Ingresadas = OrdenesIngresadas;
                ViewBag.Producidas = OrdenesProducidas;
                ViewBag.Vista = MostrarOrdenerSuma;

            }

            int count = db.OrdenesProduccion.OrderByDescending(e => e.IDOrden).Count(); // Total number of elements

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
        public ActionResult Mapeo(string currentFilter, string searchString, int? page, int? PageSize)
        {

            if (searchString == null)
            {
                searchString = currentFilter;
            }

            // Pass filtering string to view in order to maintain filtering when paging
            ViewBag.SearchString = searchString;
            //Paginación
            var elementos = from s in db.OrdenesProduccion
                            select s;

            elementos = elementos.Where(s => s.EstadoOrden == "Iniciada" || s.EstadoOrden == "Programada").OrderBy(s => s.IDOrden);
            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                elementos = elementos.Where(s => (s.IDOrden.ToString().Contains(searchString) || s.Descripcion.Contains(searchString) || s.Presentacion.Contains(searchString) || s.Cliente.Nombre.Contains(searchString)) && ((s.Liberar != "Finalizado") || (s.EstadoOrden != "Cancelada") || (s.EstadoOrden != "Finalizada"))).OrderBy(s => s.Prioridad);
            }



            //Paginación
            int count = db.OrdenesProduccion.OrderBy(e => e.IDOrden).Count(); // Total number of elements

            // Populate DropDownList
            ViewBag.PageSize = new List<SelectListItem>()
            {
                new SelectListItem { Text = "10", Value = "10" },
                new SelectListItem { Text = "25", Value = "25" },
                new SelectListItem { Text = "50", Value = "50", Selected = true },
                new SelectListItem { Text = "100", Value = "100" },
                new SelectListItem { Text = "Todo", Value = count.ToString() }
             };

            int pageNumber = (page ?? 1);
            int pageSize = (PageSize ?? 50);
            ViewBag.psize = pageSize;

            return View(elementos.ToPagedList(pageNumber, pageSize));
            //Paginación
        }


        /// //////ordenes Liberadas
        public ActionResult OrdenesLiberadas(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize, int? idorden, int? idproceso, int? orden, int? Maquina = 0)
        {

            int pageNumber = 0;
            int pageSize = 0;
            int count = 0;
            string cadena = "";

            LiberaOrdenContext dl = new LiberaOrdenContext();

            if (searchString == null)
            {
                searchString = currentFilter;
            }

            ViewBag.SearchString = searchString;

            cadena = "select LO.IDLibera, LO.TipoLiberacion, LO.IDALMACEN, LO.Lote,LO.IDOrden,LO.FechaLiberacion,LO.Cantidad,(select Username from [dbo].[User] where UserID=LO.UserID) as Usuario,(select Nombre from c_ClaveUnidad where IDClaveUnidad=A.IDClaveUnidad) as Unidad from LiberaOrdenProduccion as LO inner join OrdenProduccion as OP on OP.IDOrden=LO.IDOrden inner join Articulo as A on A.IDArticulo=OP.IDArticulo  order by LO.FechaLiberacion desc";

            List<LiberaOrden> elementos = dl.Database.SqlQuery<LiberaOrden>(cadena).ToList<LiberaOrden>();

            if (searchString != null)
            {
                cadena = "select LO.Idlibera, LO.TipoLiberacion, LO.IDALMACEN,LO.Lote, LO.IDOrden,LO.FechaLiberacion,LO.Cantidad,(select Username from [dbo].[User] where UserID=LO.UserID) as Usuario,(select Nombre from c_ClaveUnidad where IDClaveUnidad=A.IDClaveUnidad) as Unidad from LiberaOrdenProduccion as LO inner join OrdenProduccion as OP on OP.IDOrden=LO.IDOrden inner join Articulo as A on A.IDArticulo=OP.IDArticulo  where  OP.IDorden='" + searchString + "'";

                elementos = dl.Database.SqlQuery<LiberaOrden>(cadena).ToList<LiberaOrden>();

            }
            if (searchString == "")
            {
                cadena = "select l.*, (select Nombre from c_ClaveUnidad where IDClaveUnidad=A.IDClaveUnidad) as Unidad from LiberaOrdenProduccion as l inner join OrdenProduccion as o on l.idorden=o.idorden) inner join Articulo as A on  o.IDArticulo = A.IDArticulo order by l.FechaLiberacion desc";

                elementos = dl.Database.SqlQuery<LiberaOrden>(cadena).ToList<LiberaOrden>();
            }


            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            count = elementos.Count(); // Total number of elements

            // Populate DropDownList
            ViewBag.PageSize = new List<SelectListItem>()
            {
                new SelectListItem { Text = "10", Value = "10" },
                new SelectListItem { Text = "25", Value = "25", Selected = true },
                new SelectListItem { Text = "50", Value = "50" },
                new SelectListItem { Text = "100", Value = "100" },
                new SelectListItem { Text = "Todo", Value = count.ToString() }
             };

            pageNumber = (page ?? 1);
            pageSize = (PageSize ?? 25);
            ViewBag.psize = pageSize;

            return View(elementos.ToPagedList(pageNumber, pageSize));

        }

        public ActionResult AjusteLiberacion(int id, int? page)
        {
            string cadena = "select LO.IDLibera, LO.TipoLiberacion, LO.IDALMACEN, LO.Lote,LO.IDOrden,LO.FechaLiberacion,LO.Cantidad,(select Username from [dbo].[User] where UserID=LO.UserID) as Usuario,(select Nombre from c_ClaveUnidad where IDClaveUnidad=A.IDClaveUnidad) as Unidad from LiberaOrdenProduccion as LO inner join OrdenProduccion as OP on OP.IDOrden=LO.IDOrden inner join Articulo as A on A.IDArticulo=OP.IDArticulo where IDLibera=" + id;

            List<LiberaOrden> elementos = new LiberaOrdenContext().Database.SqlQuery<LiberaOrden>(cadena).ToList<LiberaOrden>();

            var TipoOperacion = new List<SelectListItem>();
            TipoOperacion.Add(new SelectListItem { Text = "---Seleccionar---", Value = "S/N" });

            TipoOperacion.Add(new SelectListItem { Text = "Entrada", Value = "E" });
            TipoOperacion.Add(new SelectListItem { Text = "Salida", Value = "S" });

            ViewBag.TipoO = new SelectList(TipoOperacion, "Value", "Text");

            ViewBag.page = page;

            ViewData["TipoO"] = TipoOperacion;
            return View(elementos);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AjusteLiberacion(LiberaOrden ajustesAlmacen, FormCollection coleccion, int? page)
        {
            string operacion = coleccion.Get("TipoO");
            string Libera = coleccion.Get("Libera");
            string Cantidad = coleccion.Get("CantidadLiberada");
            string Orden = coleccion.Get("Orden");
            string Almacen = coleccion.Get("Almacen");
            string Observacion = coleccion.Get("Observacion");
            List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            int usuario = userid.Select(s => s.UserID).FirstOrDefault();
            try
            {

                if (operacion == "E")
                {
                    decimal CantidadTotal = ajustesAlmacen.Cantidad + decimal.Parse(Cantidad);
                    string comandos3 = "update dbo.LiberaOrdenProduccion set Cantidad= " + CantidadTotal + " where [IDLibera]=" + int.Parse(Libera);
                    db.Database.ExecuteSqlCommand(comandos3);

                    OrdenProduccion o = new OrdenProduccionContext().OrdenesProduccion.Find(int.Parse(Orden));
                    Caracteristica carateristica = new WorkinProcessContext().Database.SqlQuery<Caracteristica>("select * from caracteristica where id=" + o.IDCaracteristica).ToList().FirstOrDefault();
                    try
                    {
                        string cadenapro = "UPDATE dbo.InventarioAlmacen SET Existencia = (Existencia+ " + decimal.Parse(Cantidad) + ") WHERE IDAlmacen = " + int.Parse(Almacen) + " and IDCaracteristica =" + o.IDCaracteristica + "";
                        db.Database.ExecuteSqlCommand(cadenapro);
                        db.Database.ExecuteSqlCommand("UPDATE dbo.InventarioAlmacen SET apartado=0 where  apartado<0 and  IDAlmacen=" + int.Parse(Almacen) + " and idcaracteristica=" + o.IDCaracteristica);

                        db.Database.ExecuteSqlCommand("UPDATE dbo.InventarioAlmacen SET Disponibilidad=(existencia-apartado) where IDAlmacen=" + int.Parse(Almacen) + " and idcaracteristica=" + o.IDCaracteristica);

                    }
                    catch (Exception err)
                    {

                    }
                    try
                    {
                        InventarioAlmacen invO = new InventarioAlmacenContext().InventarioAlmacenes.ToList().Where(s => s.IDAlmacen == int.Parse(Almacen) && s.IDCaracteristica == o.IDCaracteristica).FirstOrDefault();

                        string cadenam = "INSERT INTO [dbo].[MovimientoArticulo] ([Fecha],[Id],[IDPresentacion],[Articulo_IDArticulo],[Accion],[cantidad],[Documento],[NoDocumento],[Lote],[IDAlmacen],[TipoOperacion],[acumulado],[observacion],[Hora], Usuario) VALUES ";
                        cadenam += " (getdate(), " + carateristica.ID + "," + carateristica.IDPresentacion + "," + o.IDArticulo + ",'Ajuste Liberación Producción',";
                        cadenam += decimal.Parse(Cantidad) + ",'Orden Producción '," + o.IDOrden + ",'',";
                        cadenam += int.Parse(Almacen) + ",'E'," + invO.Existencia + ",'" + Observacion + "',CONVERT (time, SYSDATETIMEOFFSET())," + usuario + ")";

                        db.Database.ExecuteSqlCommand(cadenam);
                    }
                    catch (Exception err)
                    {

                    }
                }
                if (operacion == "S")
                {
                    decimal CantidadTotal = ajustesAlmacen.Cantidad - decimal.Parse(Cantidad);
                    string comandos3 = "update dbo.LiberaOrdenProduccion set Cantidad= " + CantidadTotal + " where [IDLibera]=" + int.Parse(Libera);
                    db.Database.ExecuteSqlCommand(comandos3);

                    OrdenProduccion o = new OrdenProduccionContext().OrdenesProduccion.Find(int.Parse(Orden));
                    Caracteristica carateristica = new WorkinProcessContext().Database.SqlQuery<Caracteristica>("select * from caracteristica where id=" + o.IDCaracteristica).ToList().FirstOrDefault();
                    try
                    {
                        string cadenapro = "UPDATE dbo.InventarioAlmacen SET Existencia = (Existencia- " + decimal.Parse(Cantidad) + ") WHERE IDAlmacen = " + int.Parse(Almacen) + " and IDCaracteristica =" + o.IDCaracteristica + "";
                        db.Database.ExecuteSqlCommand(cadenapro);
                        db.Database.ExecuteSqlCommand("UPDATE dbo.InventarioAlmacen SET apartado=0 where  apartado<0 and IDAlmacen=" + int.Parse(Almacen) + " and idcaracteristica=" + o.IDCaracteristica);

                        db.Database.ExecuteSqlCommand("UPDATE dbo.InventarioAlmacen SET Disponibilidad=(existencia-apartado) where IDAlmacen=" + int.Parse(Almacen) + " and idcaracteristica=" + o.IDCaracteristica);

                    }
                    catch (Exception err)
                    {

                    }
                    try
                    {
                        InventarioAlmacen invO = new InventarioAlmacenContext().InventarioAlmacenes.ToList().Where(s => s.IDAlmacen == int.Parse(Almacen) && s.IDCaracteristica == o.IDCaracteristica).FirstOrDefault();

                        string cadenam = "INSERT INTO [dbo].[MovimientoArticulo] ([Fecha],[Id],[IDPresentacion],[Articulo_IDArticulo],[Accion],[cantidad],[Documento],[NoDocumento],[Lote],[IDAlmacen],[TipoOperacion],[acumulado],[observacion],[Hora], Usuario) VALUES ";
                        cadenam += " (getdate(), " + carateristica.ID + "," + carateristica.IDPresentacion + "," + o.IDArticulo + ",'Liberación Producción',";
                        cadenam += decimal.Parse(Cantidad) + ",'Orden Producción '," + o.IDOrden + ",'',";
                        cadenam += int.Parse(Almacen) + ",'S'," + invO.Existencia + ",'" + Observacion + "',CONVERT (time, SYSDATETIMEOFFSET())," + usuario + ")";

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
            return RedirectToAction("OrdenesLiberadas", new { page = page });
        }
        public ActionResult Prioridad(string currentFilter, string searchString, int? page, int? PageSize, FormCollection coleccion, int? id = 0, int? Maquina = 0, int? Estado = 0)
        {
            int pageNumber = 1;
            int pageSize = 10;
            int count = 0;

            VMaquinaProcesoContext dba = new VMaquinaProcesoContext();
            List<SelectListItem> nueva = new SelectList(dba.VMaquinaProceso, "IDArticulo", "Descripcion", Maquina).ToList();
            nueva.Insert(0, (new SelectListItem { Text = "Programar Prioridad", Value = "0" }));
            ViewBag.Maquinas = nueva;
            ViewBag.Maquina = Maquina;

            EstadoOrdenContext dbes = new EstadoOrdenContext();

            List<SelectListItem> estad = new SelectList(dbes.EstadoOrdenes.Where(s => s.Tipo.Equals("G")), "IDEstadoOrden", "Descripcion", Estado).ToList();
            estad.Insert(0, (new SelectListItem { Text = "Ver estados", Value = "0" }));
            ViewBag.Estados = estad;
            ViewBag.Estado = Estado;
            string cadena = "";


            cadena = "select * from [OrdenProduccion] as OP inner join encpedido as ep on op.idpedido=ep.idpedido where  ( op.estadoOrden='Conflicto' or op.estadoOrden='Lista'  or op.estadoOrden='En Revision') and (ep.status<>'Inactivo' ) order by op.idorden desc";
            db.Database.CommandTimeout = 300;
            List<OrdenProduccion> elementos = db.Database.SqlQuery<OrdenProduccion>(cadena).ToList<OrdenProduccion>();

            ViewBag.Conteo = elementos.Count();


            string cadenaStock = "SELECT o.* FROM ORDENPRODUCCION as o  inner join pedidosstock as p on o.idorden=p.idorden where o.EstadoOrden<>'Cancelada' order by o.IDOrden desc";
            db.Database.CommandTimeout = 300;
            List<OrdenProduccion> elementosStock = db.Database.SqlQuery<OrdenProduccion>(cadenaStock).ToList<OrdenProduccion>();
            ViewBag.OrdenesStock = elementosStock;

            if (Estado != 0 && Maquina == 0)
            {
                cadena = "select * from [OrdenProduccion] as OP inner join EncPedido as e on e.idpedido=op.idpedido  inner join [EstadoOrden] as Es on OP.EstadoOrden=Es.Descripcion  where (e.status<>'Inactivo') and Es.IDEstadoOrden=" + Estado;
                db.Database.CommandTimeout = 300;
                List<OrdenProduccion> ele = db.Database.SqlQuery<OrdenProduccion>(cadena).ToList<OrdenProduccion>();
                //Paginación
                // DROPDOWNLIST FOR UPDATING PAGE SIZE
                count = ele.Count;
                ViewBag.Conteo = count;

                /////////// STOCK
                ///
                cadenaStock = "SELECT o.* FROM ORDENPRODUCCION as o  inner join pedidosstock as p on o.idorden=p.idorden where o.EstadoOrden='" + Estado + "' order by o.IDOrden desc";

                db.Database.CommandTimeout = 300;
                elementosStock = db.Database.SqlQuery<OrdenProduccion>(cadenaStock).ToList<OrdenProduccion>();
                ViewBag.OrdenesStock = elementosStock;
                // Populate DropDownList
                ViewBag.PageSize = new List<SelectListItem>()
                 {
                new SelectListItem { Text = "10", Value = "10"},
                new SelectListItem { Text = "25", Value = "25" , Selected = true},
                new SelectListItem { Text = "50", Value = "50"  },
                new SelectListItem { Text = "100", Value = "100" },
                new SelectListItem { Text = "Todo", Value = count.ToString() }
                  };

                pageNumber = (page ?? 1);
                pageSize = (PageSize ?? 25);
                ViewBag.psize = pageSize;

                return View(ele.ToPagedList(pageNumber, pageSize));

            }
            else if (Maquina != 0 && Estado != 0)
            {
                cadena = "select * from [OrdenProduccion] as OP inner join EncPedido as ep  on op.idoedido=ep.idpedido inner join EstadoOrden as E  on  OP.EstadoOrden=E.Descripcion inner join [ArticuloProduccion] as AP on OP.IDOrden=AP.IDOrden inner join Articulo as A on OP.IDArticulo=A.IDArticulo where  (ep.status<>'Inactivo' ) and AP.IDArticulo=" + Maquina + " and E.IDEstadoOrden=" + Estado;
                db.Database.CommandTimeout = 300;
                List<OrdenProduccion> ele = db.Database.SqlQuery<OrdenProduccion>(cadena).ToList<OrdenProduccion>();
                //Paginación
                // DROPDOWNLIST FOR UPDATING PAGE SIZE
                count = ele.Count;
                ViewBag.Conteo = count;

                // Populate DropDownList
                ViewBag.PageSize = new List<SelectListItem>()
                 {
                new SelectListItem { Text = "10", Value = "10" },
                new SelectListItem { Text = "25", Value = "25" },
                new SelectListItem { Text = "50", Value = "50" , Selected = true},
                new SelectListItem { Text = "100", Value = "100" },
                new SelectListItem { Text = "Todo", Value = count.ToString() }
                  };

                pageNumber = (page ?? 1);
                pageSize = (PageSize ?? 10);
                ViewBag.psize = pageSize;

                return View(ele.ToPagedList(pageNumber, pageSize));

            }
            else if (Maquina != 0 && Estado == 0)
            {
                return RedirectToAction("PrioridadMaquina", "OrdenProduccion", new { idmaquina = Maquina });

            }
            else if (id != 0)
            {
                try
                {
                    ClsDatoEntero cuenta = db.Database.SqlQuery<ClsDatoEntero>("select count([IDOrden]) as Dato from [OrdenProduccion] where IdOrden=" + id + "").ToList()[0];
                    int valor = cuenta.Dato;
                    if (valor != 0)
                    {
                        ViewBag.idOrden = id;
                        if (searchString == null)
                        {
                            searchString = currentFilter;
                        }

                        // Pass filtering string to view in order to maintain filtering when paging
                        ViewBag.SearchString = searchString;
                        cadena = "select * from [OrdenProduccion] as OP inner join encpedido as e on op.idpedido=e.idpedido where (e.status<>'Inactivo') and  op.idorden=" + id + "  order by op.idorden desc";
                        db.Database.CommandTimeout = 300;
                        elementos = db.Database.SqlQuery<OrdenProduccion>(cadena).ToList<OrdenProduccion>();
                        /////////// STOCK
                        cadenaStock = "SELECT o.* FROM ORDENPRODUCCION as o  inner join pedidosstock as p on o.idorden=p.idorden where o.idorden='" + id + "' order by o.IDOrden desc";

                        db.Database.CommandTimeout = 300;
                        elementosStock = db.Database.SqlQuery<OrdenProduccion>(cadenaStock).ToList<OrdenProduccion>();
                        ViewBag.OrdenesStock = elementosStock;
                        //Busqueda
                        if (!string.IsNullOrEmpty(searchString))
                        {

                            cadena = "select * from [OrdenProduccion] as OP inner join encpedido as e on e.idpedido=op.idpedido inner join Aticulo as a on op.IDArticulo= a.idarticulo inner join Clientes as c on op.idcliente=c.idcliente where  (e.status<>'Inactivo') and (op.idorden like '%" + searchString + "%' or a.cref like '%" + searchString + "%' or a.Descripcion like '%" + searchString + "%' or OP.Presentacion like '%" + searchString + " or c.Nombre like '%" + searchString + "%')";

                            /////////// STOCK
                            cadenaStock = "SELECT o.* FROM ORDENPRODUCCION as o  inner join pedidosstock as p on o.idorden=p.idorden where o.idorden='" + searchString + "' order by o.IDOrden desc";

                            db.Database.CommandTimeout = 300;
                            elementosStock = db.Database.SqlQuery<OrdenProduccion>(cadenaStock).ToList<OrdenProduccion>();
                            ViewBag.OrdenesStock = elementosStock;

                        }
                        if (elementos.Count() == 0)
                        {
                            ViewBag.mensaje = "Pedido Inactivo";
                        }
                        db.Database.CommandTimeout = 300;
                        List<OrdenProduccion> ele = db.Database.SqlQuery<OrdenProduccion>(cadena).ToList<OrdenProduccion>();

                        //Paginación
                        // Total number of elements
                        ViewBag.Conteo = ele.Count();
                        // Populate DropDownList
                        ViewBag.PageSize = new List<SelectListItem>()
                            {
                                new SelectListItem { Text = "10", Value = "10" },
                                new SelectListItem { Text = "25", Value = "25" },
                                new SelectListItem { Text = "50", Value = "50", Selected = true },
                                new SelectListItem { Text = "100", Value = "100" },
                                new SelectListItem { Text = "Todo", Value = ele.Count().ToString() }
                             };

                        pageNumber = (page ?? 1);
                        pageSize = (PageSize ?? 10);
                        ViewBag.psize = pageSize;

                        return View(ele.ToPagedList(pageNumber, pageSize));

                    }
                    else
                    {
                        ViewBag.mensaje = "Orden Inexistente";
                        cadena = "select * from [OrdenProduccion] as OP inner join encpedido as ep on op.idpedido=ep.idpedido where  ( op.estadoOrden='Conflicto' or op.estadoOrden='Lista'  or op.estadoOrden='En Revision') and (ep.status<>'Inactivo') order by op.idorden desc";
                        db.Database.CommandTimeout = 300;
                        elementos = db.Database.SqlQuery<OrdenProduccion>(cadena).ToList<OrdenProduccion>();
                        /////////// STOCK
                        cadenaStock = "SELECT o.* FROM ORDENPRODUCCION as o  inner join pedidosstock as p on o.idorden=p.idorden where o.EstadoOrden<>'Cancelada' order by o.IDOrden desc";

                        db.Database.CommandTimeout = 300;
                        elementosStock = db.Database.SqlQuery<OrdenProduccion>(cadenaStock).ToList<OrdenProduccion>();
                        ViewBag.OrdenesStock = elementosStock;
                        count = db.OrdenesProduccion.OrderBy(e => e.IDOrden).Count(); // Total number of elements
                        ViewBag.Conteo = count;
                        // Populate DropDownList
                        ViewBag.PageSize = new List<SelectListItem>()
                            {
                                new SelectListItem { Text = "10", Value = "10" },
                                new SelectListItem { Text = "25", Value = "25" },
                                new SelectListItem { Text = "50", Value = "50", Selected = true },
                                new SelectListItem { Text = "100", Value = "100" },
                                new SelectListItem { Text = "Todo", Value = count.ToString() }
                             };

                        pageNumber = (page ?? 1);
                        pageSize = (PageSize ?? 10);
                        ViewBag.psize = pageSize;


                    }
                }
                catch (Exception err)
                {

                }
            }
            else
            {



                if (searchString == null)
                {
                    searchString = currentFilter;
                }

                // Pass filtering string to view in order to maintain filtering when paging
                ViewBag.SearchString = searchString;
                cadena = "select * from [OrdenProduccion] as OP inner join encpedido as ep on op.idpedido=ep.idpedido where ( op.estadoOrden='Conflicto' or op.estadoOrden='Lista'  or op.estadoOrden='En Revision') and (ep.status<>'Inactivo' ) order by op.idorden desc";
                db.Database.CommandTimeout = 300;
                elementos = db.Database.SqlQuery<OrdenProduccion>(cadena).ToList<OrdenProduccion>();
                /////////// STOCK
                cadenaStock = "SELECT o.* FROM ORDENPRODUCCION as o  inner join pedidosstock as p on o.idorden=p.idorden where o.EstadoOrden<>'Cancelada' order by o.IDOrden desc";

                db.Database.CommandTimeout = 300;
                elementosStock = db.Database.SqlQuery<OrdenProduccion>(cadenaStock).ToList<OrdenProduccion>();
                ViewBag.OrdenesStock = elementosStock;
                //Busqueda
                if (!string.IsNullOrEmpty(searchString))
                {

                    cadena = "select * from [OrdenProduccion] as OP inner join Articulo as a on op.IDArticulo= a.idarticulo inner join encpedido as ep on ep.idpedido=op.idpedido  inner join Clientes as c on op.idcliente=c.idcliente where (ep.status<>'Inactivo') and (op.idorden like '%" + searchString + "%' or a.cref like '%" + searchString + "%' or a.Descripcion like '%" + searchString + "%' or OP.Presentacion like '%" + searchString + "%' or c.Nombre like '%" + searchString + "%')";
                    db.Database.CommandTimeout = 300;
                    List<OrdenProduccion> ele = db.Database.SqlQuery<OrdenProduccion>(cadena).ToList<OrdenProduccion>();
                    if (ele.Count() == 0)
                    {
                        ViewBag.mensaje = "Revisar estado del pedido";
                    }


                    //Paginación
                    int countt = ele.Count(); // Total number of elements
                    ViewBag.Conteo = countt;
                    // Populate DropDownList
                    ViewBag.PageSize = new List<SelectListItem>()
                        {
                            new SelectListItem { Text = "10", Value = "10" },
                            new SelectListItem { Text = "25", Value = "25" },
                            new SelectListItem { Text = "50", Value = "50", Selected = true },
                            new SelectListItem { Text = "100", Value = "100" },
                            new SelectListItem { Text = "Todo", Value = countt.ToString() }
                         };

                    pageNumber = (page ?? 1);
                    pageSize = (PageSize ?? 10);
                    ViewBag.psize = pageSize;

                    return View(ele.ToPagedList(pageNumber, pageSize));
                }



            }

            count = elementos.Count(); // Total number of elements
            ViewBag.Conteo = count;
            // Populate DropDownList
            ViewBag.PageSize = new List<SelectListItem>()
            {
                new SelectListItem { Text = "10", Value = "10" },
                new SelectListItem { Text = "25", Value = "25" },
                new SelectListItem { Text = "50", Value = "50", Selected = true },
                new SelectListItem { Text = "100", Value = "100" },
                new SelectListItem { Text = "Todo", Value = count.ToString() }
             };

            pageNumber = (page ?? 1);
            pageSize = (PageSize ?? 10);
            ViewBag.page = pageNumber;
            ViewBag.psize = pageSize;
            return View(elementos.ToPagedList(pageNumber, pageSize));

        }


        public ActionResult Details(int? id)
        {

            OrdenProduccion ordendeProduccion = db.OrdenesProduccion.Find(id);
            VCaracteristica caracteristica = new VCaracteristicaContext().VCaracteristica.Find(ordendeProduccion.IDCaracteristica);

            ClsDatoEntero countlibera = db.Database.SqlQuery<ClsDatoEntero>("select count(IDOrden) as Dato from LiberaOrdenProduccion where IDOrden=" + id + "").ToList()[0];
            if (countlibera.Dato != 0)
            {
                List<LiberaOrden> libera = db.Database.SqlQuery<LiberaOrden>("select LO.IDOrden,LO.FechaLiberacion,LO.Cantidad,(select Username from [dbo].[User] where UserID=LO.UserID) as Usuario,(select Nombre from c_ClaveUnidad where IDClaveUnidad=A.IDClaveUnidad) as Unidad from LiberaOrdenProduccion as LO inner join OrdenProduccion as OP on OP.IDOrden=LO.IDOrden inner join Articulo as A on A.IDArticulo=OP.IDArticulo where OP.IDOrden=" + id + "").ToList();
                ViewBag.Libera = libera;
            }
            else
            {
                ViewBag.Libera = null;
            }

            List<VDocumentoE> documentos = db.Database.SqlQuery<VDocumentoE>("select DE.IDDocumento,DE.IDHE,DE.Version,DE.IDProceso,DE.Descripcion,DE.Planeacion,DE.Nombre,P.NombreProceso AS Proceso from DocumentoE as DE inner join Proceso as P on P.IDProceso=DE.IDProceso where DE.Version=" + caracteristica.version + " and DE.Planeacion=" + caracteristica.Cotizacion + "").ToList();
            ViewBag.Documentos = documentos;

            List<VArticulosPlaneacionE> datos = db.Database.SqlQuery<VArticulosPlaneacionE>("select  AP.IDArtPlan,AP.IDHE,A.Descripcion as Articulo,TP.Descripcion as TipoArticulo,C.Presentacion,P.NombreProceso as Proceso,AP.Formuladerelacion,AP.factorcierre,AP.Indicaciones from ArticulosPlaneacionE as AP inner join Articulo as A on A.IDArticulo=AP.IDArticulo inner join Caracteristica as C on C.ID=AP.IDCaracteristica inner join Proceso as P on P.IDProceso=AP.IDProceso inner join TipoArticulo as TP on TP.IDTipoArticulo=A.IDTipoArticulo where AP.Version=" + caracteristica.version + " and AP.Planeacion=" + caracteristica.Cotizacion + "").ToList();

            ViewBag.Datos = datos;

            List<VDocumentoProduccion> documentosp = db.Database.SqlQuery<VDocumentoProduccion>("select D.IDDocumento,D.IDOrden,D.Version,D.IDProceso,D.Descripcion,D.Planeacion,D.Nombre,P.NombreProceso AS Proceso from DocumentoProduccion as D inner join Proceso as P on P.IDProceso=D.IDProceso where D.IDOrden=" + id + "").ToList();
            ViewBag.DocumentosP = documentosp;
            string cadena = "select AP.Existe,A.IDArticulo as IDArtProd,A.Descripcion as Articulo,TA.Descripcion as TipoArticulo,C.Presentacion as Caracteristica, C.ID as IDCaracteristica, P.NombreProceso as Proceso,AP.IDOrden,AP.Cantidad,CU.Nombre as Unidad,AP.Indicaciones,AP.CostoPlaneado,AP.CostoReal from ArticuloProduccion as AP inner join Articulo as A on A.IDArticulo=AP.IDArticulo inner join TipoArticulo as TA on A.IDTipoArticulo=TA.IDTipoArticulo inner join Caracteristica as C on C.ID=AP.IDCaracteristica inner join Proceso as P on P.IDProceso=AP.IDProceso inner join c_ClaveUnidad as CU on CU.IDClaveUnidad=AP.IDClaveUnidad  where AP.IDOrden=" + id + "";
            List<VArticulosProduccion> articulosproduccion = db.Database.SqlQuery<VArticulosProduccion>(cadena).ToList();

            ViewBag.DatosP = articulosproduccion;
            return View(ordendeProduccion);
        }
        public ActionResult Checkin(int? id)
        {
            List<VArticulosProduccion> articulosproduccion = db.Database.SqlQuery<VArticulosProduccion>("select AP.Existe,A.IDArticulo as IDArtProd,A.Descripcion as Articulo,TA.Descripcion as TipoArticulo,C.Presentacion as Caracteristica, C.ID AS IDCaracteristica, P.NombreProceso as Proceso,AP.IDOrden,AP.Cantidad,CU.Nombre as Unidad,AP.Indicaciones,AP.CostoPlaneado,AP.CostoReal from ArticuloProduccion as AP inner join Articulo as A on A.IDArticulo=AP.IDArticulo inner join TipoArticulo as TA on A.IDTipoArticulo=TA.IDTipoArticulo inner join Caracteristica as C on C.ID=AP.IDCaracteristica inner join Proceso as P on P.IDProceso=AP.IDProceso inner join c_ClaveUnidad as CU on CU.IDClaveUnidad=AP.IDClaveUnidad where AP.IDOrden='" + id + "' and (TA.Descripcion='Herramienta' or TA.Descripcion='Cintas' or TA.Descripcion='Tintas' or TA.Descripcion='Insumo'  )").ToList();

            ViewBag.ArticulosP = articulosproduccion;
            ViewBag.id = id;
           

            return View(articulosproduccion);
        }

        public ActionResult EditCheckin(List<VArticulosProduccion> cr, int? idorden, FormCollection collection)
        {
            ArticulosProduccionContext db1 = new ArticulosProduccionContext();


            string llevaarrastre = collection.Get("Arrastre");
            string llevaarrastref = collection.Get("orden.Arrastre");

            if (llevaarrastre=="on")
            {
                db1.Database.ExecuteSqlCommand("update ordenproduccion set arrastre='1' where idorden="+ idorden);
            }
            if (llevaarrastref == "false")
            {
                db1.Database.ExecuteSqlCommand("update ordenproduccion set arrastre='0' where idorden=" + idorden);
            }

            foreach (var i in cr)
            {
                if (i.Existe.Equals(false))
                {

                    string cadena = "update ArticuloProduccion set Existe='0' where IDArticulo = " + i.IDArtProd + " and IdOrden = " + idorden;
                    db1.Database.ExecuteSqlCommand(cadena);
                    
                }
                else if (i.Existe.Equals(true))
                {
                    string cadena = "update ArticuloProduccion set Existe='1' where IDArticulo = " + i.IDArtProd + " and IdOrden = " + idorden;
                    db1.Database.ExecuteSqlCommand(cadena);

                }

            }

            ClsDatoEntero c = db.Database.SqlQuery<ClsDatoEntero>("select count(IDArtProd) as Dato from ArticuloProduccion where IDOrden =" + idorden + "").ToList()[0];
            int x = c.Dato;

            ClsDatoEntero b = db.Database.SqlQuery<ClsDatoEntero>("select count(IDArtProd) as Dato from ArticuloProduccion where IDOrden =" + idorden + " and Existe='1'").ToList()[0];
            int y = b.Dato;

            if (x == y)
            {
                 automata AutomataOrden;

                XmlSerializer serializer = new XmlSerializer(typeof(automata));

               
                string path = Path.Combine(Server.MapPath("~/Automatas/Ordenes.Xml"));


                using (TextReader reader = new StreamReader(path))
                {
                    serializer = new XmlSerializer(typeof(automata));
                    AutomataOrden = (automata)serializer.Deserialize(reader);
                }


                AutomataOrden.establecerestadoinicial();

                OrdenProduccion op = new OrdenProduccionContext().OrdenesProduccion.Find(idorden);

                AutomataOrden.Estadoactual.Nombre = op.EstadoOrden;

                 AutomataOrden.ejecutar(op.EstadoOrden, "L");

                string estadoOrden = AutomataOrden.Estadoactual.Nombre;


                db.Database.ExecuteSqlCommand("update [dbo].[OrdenProduccion] set [EstadoOrden]='" + estadoOrden + "' where [IDOrden]='" + idorden + "'");
            }
            else
            {
                db.Database.ExecuteSqlCommand("update [dbo].[OrdenProduccion] set [FechaProgramada]=null ,[EstadoOrden]='Conflicto' where [IDOrden]='" + idorden + "'");
            }
            return RedirectToAction("Prioridad", new { searchString = idorden });
        }

        public ActionResult Seleccionar(int? id)
        {
            OrdenProduccion ordendeProduccion = db.OrdenesProduccion.Find(id);

            System.Web.HttpContext.Current.Session["IDOrden"] = id;
            System.Web.HttpContext.Current.Session["IDModeloProduccion"] = ordendeProduccion.IDModeloProduccion;
            return RedirectToAction("Prioridad");

        }

        public ActionResult ArticuloProduccion(string sortOrder, string currentFilter, string searchString, int? page, int? id, string TipoArticulo, int? idorden)
        {
            ViewBag.VerPag = null;
            int idordenn = 0;
            ArticuloContext db = new ArticuloContext();
            if (TipoArticulo != null && TipoArticulo.Equals("Documento"))
            {
                try
                {
                    idordenn = int.Parse(System.Web.HttpContext.Current.Session["IDOrden"].ToString());

                    ViewBag.idorden = idordenn;
                    idorden = idordenn;

                }
                catch
                {
                    ViewBag.idorden = idorden;
                }
                ViewBag.idproceso = id;
                return RedirectToAction("Documento", new { idorden = idorden, idproceso = id });
            }
            try
            {
                idordenn = int.Parse(System.Web.HttpContext.Current.Session["IDOrden"].ToString());

                ViewBag.idorden = idordenn;
                idorden = idordenn;
            }
            catch
            {
                ViewBag.idorden = idorden;
            }

            ViewBag.idproceso = id;

            OrdenProduccionContext dbhe = new OrdenProduccionContext();
            OrdenProduccion ordenp = dbhe.OrdenesProduccion.Find(idorden);
            ViewBag.idcorden = ordenp.IDCaracteristica;
            ViewBag.preso = ordenp.Presentacion;
            ViewBag.artio = ordenp.Descripcion;



            ProcesoContext procesos = new ProcesoContext();
            Proceso proceso = procesos.Procesos.Find(id);
            ViewBag.nombreproceso = proceso.NombreProceso;


            var procesoLst = new List<string>();
            if (proceso.UsaMaquina.Equals(true))
            {
                procesoLst.Add("Maquina");
            }
            if (proceso.UsaManoDeObra.Equals(true))
            {
                procesoLst.Add("Mano de obra");
            }
            if (proceso.UsaHerramientas.Equals(true))
            {
                procesoLst.Add("Herramienta");
            }
            if (proceso.UsaInsumos.Equals(true))
            {
                procesoLst.Add("Insumo");
            }
            if (proceso.UsaDocumentos.Equals(true))
            {
                procesoLst.Add("Documento");
            }

            ViewBag.TipoArticulo = new SelectList(procesoLst);

            ViewBag.TArticulo = TipoArticulo;

            ViewBag.SearchString = searchString;



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
            ArticuloContext va = new ArticuloContext();

            var elementos = from s in va.Articulo
                            select s;
            //Filtro Divisa
            if (!String.IsNullOrEmpty(TipoArticulo))
            {

                ClsDatoEntero idtipoarticulo = db.Database.SqlQuery<ClsDatoEntero>("select IDTipoArticulo as Dato from TipoArticulo where Descripcion='" + TipoArticulo + "'").ToList()[0];
                elementos = (from s in db.Articulo
                             select s).OrderByDescending(s => s.IDArticulo);
                elementos = elementos.Where(s => s.IDTipoArticulo == idtipoarticulo.Dato).OrderByDescending(s => s.IDArticulo);
                ViewBag.VerPag = 1;

            }
            if (!string.IsNullOrEmpty(searchString) && !String.IsNullOrEmpty(TipoArticulo))
            {
                ClsDatoEntero idtipoa = db.Database.SqlQuery<ClsDatoEntero>("select IDTipoArticulo as Dato from TipoArticulo where Descripcion='" + TipoArticulo + "'").ToList()[0];
                elementos = elementos.Where(s => s.Descripcion.Contains(searchString) && s.IDTipoArticulo.Equals(idtipoa.Dato));
                ViewBag.VerPag = 1;


            }


            int pageSize = 12;
            int pageNumber = (page ?? 1);
            int count = va.Articulo.OrderBy(e => e.IDArticulo).Count();
            List<VDocumentoProduccion> documentos = db.Database.SqlQuery<VDocumentoProduccion>("select D.IDDocumento,D.IDOrden,D.Version,D.IDProceso,D.Descripcion,D.Planeacion,D.Nombre,P.NombreProceso AS Proceso from DocumentoProduccion as D inner join Proceso as P on P.IDProceso=D.IDProceso where D.IDOrden='" + idorden + "' and D.IDProceso='" + id + "'").ToList();
            ViewBag.Documentos = documentos;
            List<VArticulosProduccion> articulosproduccion = new List<VArticulosProduccion>();
            try
            {
                string cadena = "select AP.Existe,A.IDArticulo as IDArtProd,A.cref as Articulo,TA.Descripcion as TipoArticulo,C.Presentacion as Caracteristica, C.ID as IDCaracteristica, P.NombreProceso as Proceso,AP.IDOrden,AP.Cantidad,CU.Nombre as Unidad,AP.Indicaciones,AP.CostoPlaneado,AP.CostoReal from ArticuloProduccion as AP inner join Articulo as A on A.IDArticulo=AP.IDArticulo inner join TipoArticulo as TA on A.IDTipoArticulo=TA.IDTipoArticulo inner join Caracteristica as C on C.ID=AP.IDCaracteristica inner join Proceso as P on P.IDProceso=AP.IDProceso inner join c_ClaveUnidad as CU on CU.IDClaveUnidad=AP.IDClaveUnidad where AP.IDOrden='" + idorden + "' and AP.IDProceso='" + id + "'";
                articulosproduccion = db.Database.SqlQuery<VArticulosProduccion>(cadena).ToList();
            }
            catch (Exception err)
            {
                string mensajeerror = err.Message;
            }


            ViewBag.Datos = articulosproduccion;

            return View(elementos.OrderBy(i => i.Descripcion).ToPagedList(page ?? 1, pageSize));

        }

        [HttpPost]
        public JsonResult Deleteitem(int id)
        {
            try
            {
                ArticulosProduccionContext car = new ArticulosProduccionContext();
                car.Database.ExecuteSqlCommand("delete from ArticuloProduccion where IDArtProd=" + id);

                return Json(true);
            }
            catch (Exception err) { return Json(500, err.Message); }
        }
        public ActionResult GetPresentaciones(int? id)
        {

            VPArticuloContext db = new VPArticuloContext();
            var x = db.VPArticulos.Find(id);
            ViewBag.Descripcion = x.Descripcion;
            var lista = db.Database.SqlQuery<Caracteristica>("select * from Caracteristica where Articulo_IDArticulo=" + id + "").ToList();
            return PartialView(lista);

        }


        [HttpPost]
        public JsonResult addarticuloproduccion(int? idorden, int? idarticulo, int? idcaracteristica, int? idproceso, decimal? cantidad, int? idunidad, string indicaciones, int? idcaorden)
        {
            try
            {

                VCaracteristica vcaracteristica = new VCaracteristicaContext().VCaracteristica.Find(idcaorden);
                Articulo articulo = new ArticuloContext().Articulo.Find(idarticulo);
                db.Database.ExecuteSqlCommand("INSERT INTO [dbo].[ArticuloProduccion]([Version],[IDArticulo],[IDTipoArticulo],[IDCaracteristica],[IDProceso],[IDOrden],[Cantidad],[IDClaveUnidad],[Indicaciones],[Planeacion],[CostoPlaneado],[CostoReal],[Existe]) VALUES('" + vcaracteristica.version + "','" + idarticulo + "','" + articulo.IDTipoArticulo + "','" + idcaracteristica + "','" + idproceso + "','" + idorden + "','" + cantidad + "','" + idunidad + "','" + indicaciones + "','" + vcaracteristica.Cotizacion + "',0,0,0)");

                return Json(new HttpStatusCodeResult(200));
            }
            catch (Exception err)
            {
                return Json(new { errorMessage = err.Message });
            }

        }


        [HttpPost]
        public JsonResult addfechapro(int id, DateTime fecha)
        {
            try
            {
                string fechaformato = fecha.ToString("yyyy-MM-dd");

               automata AutomataOrden;

                XmlSerializer serializer = new XmlSerializer(typeof(automata));

                // Declare an object variable of the type to be deserialized.


                string path = Path.Combine(Server.MapPath("~/Automatas/Ordenes.Xml"));


                using (TextReader reader = new StreamReader(path))
                {
                    serializer = new XmlSerializer(typeof(automata));
                    AutomataOrden = (automata)serializer.Deserialize(reader);
                }


                AutomataOrden.establecerestadoinicial();

                OrdenProduccion op = new OrdenProduccionContext().OrdenesProduccion.Find(id);



                AutomataOrden.ejecutar(op.EstadoOrden, "P");

                string estadoOrden = AutomataOrden.Estadoactual.Nombre;


                db.Database.ExecuteSqlCommand("update OrdenProduccion set EstadoOrden='" + estadoOrden + "', FechaProgramada='" + fechaformato + "' where IDOrden=" + id);

                 automata Automataproceso;


                path = Path.Combine(Server.MapPath("~/Automatas/Proceso.Xml"));


                using (TextReader reader = new StreamReader(path))
                {
                    serializer = new XmlSerializer(typeof(automata));
                    Automataproceso = (automata)serializer.Deserialize(reader);
                }


                Automataproceso.establecerestadoinicial();

                ModeloProcesoContext mpc = new ModeloProcesoContext();

                OrdenProduccion orden = new OrdenProduccionContext().OrdenesProduccion.Find(id);
                int primerproceso = mpc.ModeloProceso.Where(s => s.ModelosDeProduccion_IDModeloProduccion == orden.IDModeloProduccion).OrderBy(s => s.orden).FirstOrDefault().Proceso_IDProceso;

                OrdenProduccionDetalle procesodetalle = new OrdenProduccionDetalleContext().Database.SqlQuery<OrdenProduccionDetalle>("select * from OrdenProducciondetalle where idorden=" + orden.IDOrden + " and idproceso=" + primerproceso).ToList().FirstOrDefault();

                Automataproceso.ejecutar(procesodetalle.EstadoProceso, "P");

                string estadoproceso = Automataproceso.Estadoactual.Nombre;

                db.Database.ExecuteSqlCommand("update OrdenProducciondetalle set EstadoProceso='" + estadoproceso + "' where IDOrdenDetalle=" + procesodetalle.IDOrdenDetalle);



                return Json(true);
            }
            catch (Exception err)
            {
                return Json(500, err.Message);
            }
        }

        public ActionResult LiberarOrden(int id, int IDOrdenL, string Mensaje = "")
        {
            ViewBag.OrdenLibera = IDOrdenL;
            ViewBag.idorden = id;
            OrdenProduccion ordenp = new OrdenProduccionContext().OrdenesProduccion.Find(id);
            Articulo articulo = new ArticuloContext().Articulo.Find(ordenp.IDArticulo);
            c_ClaveUnidad unidad = new c_ClaveUnidadContext().c_ClaveUnidades.Find(articulo.IDClaveUnidad);
            ViewBag.IDArticulo = new SelectList(new ArticuloContext().Articulo.Where(s => s.IDArticulo == ordenp.IDArticulo), "IDArticulo", "Descripcion");
            VCaracteristicaContext caracteristica = new VCaracteristicaContext();
            var carac = caracteristica.VCaracteristica.Where(x => x.ID == ordenp.IDCaracteristica).ToList();




            DetPedido detpedido = new PedidoContext().DetPedido.Find(ordenp.IDDetPedido);

            AlmacenContext dblib = new AlmacenContext();
            ViewBag.IDAlmacen = new SelectList(dblib.Almacenes, "IDAlmacen", "Descripcion", detpedido.IDAlmacen);

            ClsDatoEntero trabajador = db.Database.SqlQuery<ClsDatoEntero>(" select IDTrabajador as Dato from bitacora where  IDOrden=" + id + " order by IDBitacora desc").ToList().FirstOrDefault();

            DateTime fecha = DateTime.Now;
            string lote = Completa3(trabajador.Dato) + Completa5(id);
            ViewBag.Lote = lote;
            List<SelectListItem> listcaracteristica = new List<SelectListItem>();
            ViewBag.MensajeError = Mensaje;
            foreach (var x in carac)
            {

                listcaracteristica.Add(new SelectListItem { Text = x.Presentacion, Value = x.ID.ToString() });

            }
            ViewBag.IDCaracteristica = listcaracteristica;

            string horafecha = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            ViewBag.horafecha = horafecha;
            ViewBag.Unidad = unidad.Nombre;
            ViewBag.CantidadOriginal = ordenp.Cantidad;
            try
            {
                ClsDatoDecimal cantidadliberada = db.Database.SqlQuery<ClsDatoDecimal>("select sum(Cantidad) as Dato from LiberaOrdenProduccion where IDOrden=" + id + "").ToList()[0];
                ViewBag.CantidadLiberada = cantidadliberada.Dato;
            }
            catch (Exception err)
            {
                string mensaje = err.Message;
                ViewBag.CantidadLiberada = 0;
            }

            return View();
        }

        [HttpPost]
        public ActionResult LiberarOrden(int? IDOrden, DateTime FechaLiberacion, decimal Cantidad, string Observacion, int? idAlmacen, string Lote, FormCollection coleccion)
        {
            OrdenProduccion orden = new OrdenProduccionContext().OrdenesProduccion.Find(IDOrden);
            EncPedido pedido = new PedidoContext().EncPedidos.Find(orden.IDPedido);
            var detallepedido = new PedidoContext().DetPedido.Where(s => s.IDDetPedido == orden.IDDetPedido);
            List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            int usuario = userid.Select(s => s.UserID).FirstOrDefault();
            string FechaLiberacionf = DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year;
            DateTime fechaLiberacionFinal = DateTime.Now;
            string FechaLiberacionfe = DateTime.Now.ToString("yyyy-MM-dd");
            int AlmacenViene = 0;
            string OrdenLiberaDelete = coleccion.Get("OrdenLibera");

            try
            {
                string consulta= "select * from LiberaOrdenProduccion where IDOrden ="+IDOrden+" and TipoLiberacion = 'Final'";
                LiberaOrden libera = new LiberaOrdenContext().Database.SqlQuery<LiberaOrden>(consulta).ToList().FirstOrDefault();

                if (libera!=null)
                {
                    return RedirectToAction("LiberarOrden", new { id =IDOrden, IDOrdenL= OrdenLiberaDelete,Mensaje="La orden ya tiene una liberación final" });
                }

            }
            catch (Exception err)
            {

            }
            try
            {
                if (coleccion.Get("Enviar").Equals("Liberacion Final"))
                {

                    db.Database.ExecuteSqlCommand("delete from OrdenesALiberar where IDOrden=" + IDOrden);

                    try
                    {
                        string cadenas = "select e.* from encpedido as e inner join PedidosStock as p on p.idpedido=e.idpedido where p.idorden=" + IDOrden;

                        EncPedido enc = new PedidoContext().Database.SqlQuery<EncPedido>(cadenas).ToList().FirstOrDefault();
                        if (enc!= null)
                        {
                            string ejecutar = "update encpedido set status='Finalizado' where idpedido=" + enc.IDPedido;
                            string ejecuatr2 = "update detpedido set status='Finalizado' where idpedido="+ enc.IDPedido;

                            db.Database.ExecuteSqlCommand(ejecuatr2);
                            db.Database.ExecuteSqlCommand(ejecutar);
                        }

                    }
                    catch (Exception err)
                    {

                    }
                   

                }
                else
                {

                    db.Database.ExecuteSqlCommand("delete from OrdenesALiberar where IDOrdenesALiberar=" + int.Parse(OrdenLiberaDelete));


                }
            }
            catch (Exception err)
            {

            }
            foreach (var item in detallepedido)
            {
                AlmacenViene = item.IDAlmacen;
            }

            string cadena = "";
            string cadenapro = "";


            cadena = "INSERT INTO [dbo].[LiberaOrdenProduccion]([IDOrden],[FechaLiberacion],[Cantidad],[UserID], idAlmacen, lote)VALUES(" + IDOrden + ",SYSDATETIME()," + Cantidad + "," + usuario + "," + AlmacenViene + ",'" + Lote + "')";
            db.Database.ExecuteSqlCommand(cadena);
            InventarioAlmacen inv = new InventarioAlmacenContext().InventarioAlmacenes.ToList().Where(s => s.IDAlmacen == AlmacenViene && s.IDCaracteristica == orden.IDCaracteristica).FirstOrDefault();
            idAlmacen = AlmacenViene;
            //if (AlmacenViene == idAlmacen)
            //{
            decimal ex = 0;
            if (inv != null)
            {
                cadenapro = "UPDATE dbo.InventarioAlmacen SET Existencia = (Existencia+ " + Cantidad + "), Disponibilidad = (Disponibilidad+" + Cantidad + "), PorLlegar =(Porllegar-" + Cantidad + ") WHERE IDAlmacen = " + idAlmacen + " and IDCaracteristica =" + orden.IDCaracteristica + "";
                db.Database.ExecuteSqlCommand(cadenapro);
                db.Database.ExecuteSqlCommand("UPDATE dbo.InventarioAlmacen SET porllegar=0 where porllegar<0 and IDAlmacen=" + idAlmacen + " and idcaracteristica=" + orden.IDCaracteristica);
                ex = inv.Existencia;
            }
            else
            {
                cadenapro = "INSERT INTO dbo.InventarioAlmacen (IDAlmacen,IDArticulo,IDCaracteristica,Existencia, PorLlegar,Apartado,Disponibilidad)VALUES (" + idAlmacen + "," + orden.IDArticulo + "," + orden.IDCaracteristica + "," + Cantidad + ",0,0," + Cantidad + ")";
                ex = 0;
                db.Database.ExecuteSqlCommand(cadenapro);
            }

            Caracteristica carateristica = new WorkinProcessContext().Database.SqlQuery<Caracteristica>("select * from caracteristica where id=" + orden.IDCaracteristica).ToList().FirstOrDefault();
            InventarioAlmacen invO = new InventarioAlmacenContext().InventarioAlmacenes.ToList().Where(s => s.IDAlmacen == AlmacenViene && s.IDCaracteristica == orden.IDCaracteristica).FirstOrDefault();

            try
            {
                string cadenam = "INSERT INTO [dbo].[MovimientoArticulo] ([Fecha],[Id],[IDPresentacion],[Articulo_IDArticulo],[Accion],[cantidad],[Documento],[NoDocumento],[Lote],[IDAlmacen],[TipoOperacion],[acumulado],[observacion],[Hora]) VALUES ";
                cadenam += " (getdate(), " + carateristica.ID + "," + carateristica.IDPresentacion + "," + orden.IDArticulo + ",'Liberación Producción',";
                cadenam += Cantidad + ",'Orden Producción '," + IDOrden + ",'',";
                cadenam += idAlmacen + ",'E'," + invO.Existencia + ",'',CONVERT (time, SYSDATETIMEOFFSET()))";

                db.Database.ExecuteSqlCommand(cadenam);
            }
            catch (Exception err)
            {

            }

           
            decimal CantidadLiberada = 0;
            try
            {
                ClsDatoDecimal cantidadliberada = db.Database.SqlQuery<ClsDatoDecimal>("select sum(Cantidad) as Dato from LiberaOrdenProduccion where IDOrden=" + IDOrden + "").ToList()[0];
                CantidadLiberada = cantidadliberada.Dato;
            }
            catch (Exception err)
            {
                string mensaje = err.Message;
                CantidadLiberada = 0;
            }
            decimal total = CantidadLiberada + Cantidad;



            int idLiberacion = 0;
            try
            {
                ClsDatoEntero liberacion = db.Database.SqlQuery<ClsDatoEntero>("select max(IDLibera) as Dato from liberaordenproduccion where idorden = " + IDOrden).ToList().FirstOrDefault();


                idLiberacion = liberacion.Dato;
            }
            catch (Exception err)
            {

            }
            db.Database.ExecuteSqlCommand("update LiberaOrdenProduccion set  TipoLiberacion='Parcial' where IDLibera=" + idLiberacion + " and IDOrden=" + IDOrden + "");
            if (coleccion.Get("Enviar").Equals("Liberacion Final"))
            {
                FinalizarOrden(IDOrden, idAlmacen, idLiberacion);
            }

 			try
            {
                //db.Database.ExecuteSqlCommand("Update prioridades set estado='Iniciada', prioridad=0 where idorden=" + IDOrden);
                //db.Database.ExecuteSqlCommand("Update ordenproduccion set Liberar='Finalizado' where idorden=" + IDOrden);
            }
            catch (Exception err)
            {

            }


            return RedirectToAction("Calidad", new { IDLibera = idLiberacion });
        }



        public void FinalizarOrden(int? id, int? IDAlmacen, int IDLiberacion)
        {

            OrdenProduccion orden = new OrdenProduccionContext().OrdenesProduccion.Find(id);
            DetPedido detpedido = new PedidoContext().DetPedido.Find(orden.IDDetPedido);
            decimal CantidadLiberada = 0;
            try
            {
                ClsDatoDecimal cantidadliberada = db.Database.SqlQuery<ClsDatoDecimal>("select sum(Cantidad) as Dato from LiberaOrdenProduccion where IDOrden=" + id + "").ToList().FirstOrDefault();
                CantidadLiberada = cantidadliberada.Dato;
            }
            catch (Exception err)
            {
                string mensaje = err.Message;
                CantidadLiberada = 0;
            }
            decimal total = orden.Cantidad - CantidadLiberada;




            XmlSerializer serializer = new XmlSerializer(typeof(automata));

            // Declare an object variable of the type to be deserialized.
            automata Automata;


            string cadena1 = "select * from OrdenProduccionDetalle where IDOrden=" + id;
            OrdenProduccionDetalleContext dbart = new OrdenProduccionDetalleContext();
            List<OrdenProduccionDetalle> t = dbart.Database.SqlQuery<OrdenProduccionDetalle>(cadena1).ToList();
            ViewBag.Orden = t;



            foreach (OrdenProduccionDetalle ORD in t)
            {
                db.Database.ExecuteSqlCommand("update OrdenProducciondetalle set EstadoProceso='Terminado' where IDOrden=" + ORD.IDOrden + "");

            }

            string path = Path.Combine(Server.MapPath("~/Automatas/Ordenes.Xml"));


            using (TextReader reader = new StreamReader(path))
            {
                serializer = new XmlSerializer(typeof(automata));
                Automata = (automata)serializer.Deserialize(reader);
            }



            OrdenProduccion o = new OrdenProduccionContext().OrdenesProduccion.Find(id);



            Automata.ejecutar(o.EstadoOrden, "F");
            string estadoOrden = "";
            if (o.EstadoOrden == "Parcial")
            {
                estadoOrden = "Finalizada";
            }
            else
            {
                estadoOrden = Automata.Estadoactual.Nombre;
            }

            try
            {
                db.Database.ExecuteSqlCommand("Update prioridades set estado='Finalizada', prioridad=0 where idorden=" + id);
                //db.Database.ExecuteSqlCommand("Update ordenproduccion set Liberar='Finalizado' where idorden=" + IDOrden);
            }
            catch (Exception err)
            {

            }
            db.Database.ExecuteSqlCommand("update OrdenProduccion set Liberar='Finalizado', EstadoOrden='" + estadoOrden + "' where IDOrden=" + id + "");

            db.Database.ExecuteSqlCommand("update LiberaOrdenProduccion set  TipoLiberacion='Final' where IDLibera=" + IDLiberacion + " and IDOrden=" + id + "");

            Calidad(IDLiberacion);

        }

       

        public ActionResult Documento(int? idorden, int? idproceso)
        {
            ViewBag.idorden = idorden;
            ViewBag.idproceso = idproceso;
            return View();
        }
        [HttpPost]
        public ActionResult Documento(HttpPostedFileBase file, int? idorden, int? idproceso, string Descripcion)
        {
            OrdenProduccion orden = new OrdenProduccionContext().OrdenesProduccion.Find(idorden);
            VCaracteristica caracteristica = new VCaracteristicaContext().VCaracteristica.Find(orden.IDCaracteristica);
            string extension = Path.GetExtension(Request.Files[0].FileName).ToLower();

            HttpPostedFileBase archivo = Request.Files["file"];
            try
            {


                if (file != null && file.ContentLength > 0)
                {
                    
                    var nombreArchivo = Path.GetFileName(file.FileName);
                    var path = Path.Combine(Server.MapPath("~/Documentos"), nombreArchivo);


                    string nameArchivo = file.FileName;

                    db.Database.ExecuteSqlCommand("INSERT INTO [dbo].[DocumentoProduccion] ([IDOrden],[Version],[IDProceso],[Descripcion],[Planeacion],[Nombre]) VALUES('" + idorden + "','" + caracteristica.version + "','" + idproceso + "','" + Descripcion + "','" + caracteristica.Cotizacion + "','" + nameArchivo + "')");

                    file.SaveAs(path);
                    
                }
            }
            catch (Exception err)
            {
                string mensaje = err.Message;
                ViewBag.Mensajeerror = "No se pudo subir el archivo";
                return View();
            }

            return RedirectToAction("ArticuloProduccion", new { id = idproceso });


        }

        public FileResult DescargarDoc(int id)
        {
            DocumentoProduccionContext pf = new DocumentoProduccionContext();
            var elemento = pf.DocumentosProduccion.Single(m => m.IDDocumento == id);

            string path = Server.MapPath("~/Documentos/" + elemento.Nombre);
            byte[] fileBytes = System.IO.File.ReadAllBytes(path);
            MemoryStream ms = new MemoryStream(fileBytes, 0, 0, true, true);
            Response.AddHeader("content-disposition", "attachment;filename=" + elemento.Nombre + "");
            Response.Buffer = true;
            Response.Clear();
            Response.OutputStream.Write(ms.GetBuffer(), 0, ms.GetBuffer().Length);
            Response.OutputStream.Flush();
            Response.End();
            return new FileStreamResult(Response.OutputStream, "application/pdf");
        }
        [HttpPost]
        public JsonResult Deleteitemdoc(int id)
        {
            try
            {

                DocumentoProduccionContext pf = new DocumentoProduccionContext();

                var elemento = pf.DocumentosProduccion.Single(m => m.IDDocumento == id);
                var file = Path.Combine(Server.MapPath("~/Documentos/" + elemento.Nombre));
                System.IO.File.Delete(file);

                pf.Database.ExecuteSqlCommand("delete from DocumentoProduccion where IDDocumento=" + id);
                return Json(true);
            }
            catch (Exception err)
            {
                return Json(500, err.Message);
            }
        }
        public ActionResult PDF(int id)
        {
            EntregaRemisionesContext db = new EntregaRemisionesContext();
            EmpresaContext dbe = new EmpresaContext();
            var empresa = dbe.empresas.Single(m => m.IDEmpresa == 2);

            OrdenProduccion oprod = new OrdenProduccionContext().OrdenesProduccion.Find(id);

            System.Drawing.Image logoempresa = byteArrayToImage(empresa.Logo);
            string contentType = System.Net.Mime.MediaTypeNames.Application.Pdf;
            try
            {

                if (oprod.IDModeloProduccion == 4)
                {
                    CreaOrdenProduccionPDF documento = new CreaOrdenProduccionPDF(logoempresa, id);
                    return new FilePathResult(documento.nombreDocumento, contentType);
                }
                if (oprod.IDModeloProduccion == 8 || oprod.IDModeloProduccion== 7)
                {
                    CreaOrdenTermoPDF documento = new CreaOrdenTermoPDF(logoempresa, id);
                    return new FilePathResult(documento.nombreDocumento, contentType);
                }

            }
            catch (Exception err)
            {
                string mensaje = err.Message;
            }
           return  RedirectToAction("Index");

        }


        public ActionResult CambioPlaneacion(int id)
        {
            Caracteristica cara = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from Caracteristica where id=" + id).FirstOrDefault();

            Articulo arti = new ArticuloContext().Articulo.Find(cara.Articulo_IDArticulo);

            ViewBag.Articulo = arti;
            ViewBag.Presentacion = cara;

            ClsCotizador elemento = new ClsCotizador();
            Cotizaciones archivocotizacion = new ArchivoCotizadorContext().cotizaciones.Find(cara.IDCotizacion);
            XmlDocument documento = new XmlDocument();
            XmlSerializer serializerX = new XmlSerializer(typeof(ClsCotizador));
            try
            {
             
                string nombredearchivo = archivocotizacion.Ruta.TrimEnd() + "\\" + archivocotizacion.NombreArchivo + ".xml";
                documento.Load(nombredearchivo);

          
                using (Stream reader = new FileStream(nombredearchivo, FileMode.Open))
                {
                    // Call the Deserialize method to restore the object's state.
                    elemento = (ClsCotizador)serializerX.Deserialize(reader);
                }
            }
            catch (Exception er)
            {
                string mensajedeerror = er.Message;
                elemento = (ClsCotizador)serializerX.Deserialize(StringExtensiones.ToXmlReader(archivocotizacion.contenido));
            }

            ClsCambioPlaneacion cambio = new ClsCambioPlaneacion();

            cambio.IDCotizacion = cara.IDCotizacion;
            cambio.IDSuaje1 = elemento.IDSuaje;
            cambio.IDSuaje2 = elemento.IDSuaje2;
           
            cambio.IDMaquinaPrensa = elemento.IDMaquinaPrensa;
            cambio.IDMaquinaEmbobinado = elemento.IDMaquinaEmbobinado;
            cambio.cyrel = elemento.cyrel;

            ViewBag.IDMaquinaPrensa = new Repository().GetMaquinas(elemento.IDMaquinaPrensa);


           
            
            ViewBag.IDMaquinaEmbobinado = new Repository().GetMaquinasEmbo(elemento.IDMaquinaEmbobinado);

            ViewBag.IDSuaje1 = new Repository().GetSuajes(elemento.IDSuaje);


            ViewBag.IDSuaje2 = new Repository().GetPlecas(elemento.IDSuaje2);


            return View(cambio);


        }

        [HttpPost]
        
        public ActionResult CambioPlaneacion(ClsCambioPlaneacion cambio)
        {

            ClsCotizador elemento = new ClsCotizador();
            Cotizaciones archivocotizacion = new ArchivoCotizadorContext().cotizaciones.Find(cambio.IDCotizacion);
            XmlSerializer serializerX = new XmlSerializer(typeof(ClsCotizador));
            string nombredearchivo = "";
            try
            {
                XmlDocument documento = new XmlDocument();
                 nombredearchivo = archivocotizacion.Ruta.TrimEnd() + "\\" + archivocotizacion.NombreArchivo + ".xml";
                documento.Load(nombredearchivo);


                using (Stream reader = new FileStream(nombredearchivo, FileMode.Open))
                {
                    // Call the Deserialize method to restore the object's state.
                    elemento = (ClsCotizador)serializerX.Deserialize(reader);
                }
            }
            catch (Exception er)
            {
                string mensajedeerror = er.Message;
                elemento = (ClsCotizador)serializerX.Deserialize(StringExtensiones.ToXmlReader(archivocotizacion.contenido));
            }
            elemento.IDMaquinaPrensa = cambio.IDMaquinaPrensa;
            elemento.IDMaquinaEmbobinado = cambio.IDMaquinaEmbobinado;
            elemento.IDSuaje = cambio.IDSuaje1;
            elemento.IDSuaje2 = cambio.IDSuaje2;
            elemento.cyrel = cambio.cyrel;

            Caracteristica cara = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from Caracteristica where id=" + cambio.IDSuaje1).FirstOrDefault();
            FormulaSiaapi.Formulas formula = new FormulaSiaapi.Formulas();


            decimal temancho = elemento.anchoproductomm;

            try { elemento.anchoproductomm= decimal.Parse(formula.getvalor("EJE", cara.Presentacion).ToString());
                if (elemento.anchoproductomm==0M)
                {
                    elemento.anchoproductomm = temancho;
                }
            }
            catch(Exception ERR)
            {

            }

            decimal temlargo = elemento.largoproductomm;
            try
            {
                elemento.largoproductomm = decimal.Parse(formula.getvalor("AVANCE", cara.Presentacion).ToString());
                if (elemento.largoproductomm == 0M)
                {
                    elemento.largoproductomm = decimal.Parse(formula.getvalor("DESARROLLO", cara.Presentacion).ToString());
                    if (elemento.largoproductomm == 0M)
                    {
                        elemento.largoproductomm = temlargo;
                    }
                }

            }
            catch (Exception ERR)
            {

            }

            int Cavidades = 0;

            try
            {
                Cavidades = int.Parse(formula.getvalor("CAV EJE", cara.Presentacion).ToString());
                if (Cavidades == 0)
                {
                    Cavidades = int.Parse(formula.getvalor("CAV EJE ", cara.Presentacion).ToString());
                    if (Cavidades == 0)
                    {
                        Cavidades = int.Parse(formula.getvalor("CAVIDADES EJE", cara.Presentacion).ToString());
                        if (Cavidades == 0)
                        {
                            Cavidades = int.Parse(formula.getvalor("CAVIDADES AL EJE", cara.Presentacion).ToString());
                            if (Cavidades == 0)
                            {
                                Cavidades = int.Parse(formula.getvalor("CAV AL EJE", cara.Presentacion).ToString());
                                if (Cavidades == 0)
                                {
                                    Cavidades = int.Parse(formula.getvalor("CAV AL EJE ", cara.Presentacion).ToString());
                                    if (Cavidades == 0)
                                    {
                                        Cavidades = int.Parse(formula.getvalor("CAVIDADES AL EJE ", cara.Presentacion).ToString());
                                        if (Cavidades == 0)
                                        {
                                            Cavidades = int.Parse(formula.getvalor("REP EJE", cara.Presentacion).ToString());
                                            if (Cavidades == 0)
                                            {
                                                Cavidades = int.Parse(formula.getvalor("REPETICIONES EJE", cara.Presentacion).ToString());
                                                if (Cavidades == 0)
                                                {
                                                    Cavidades = int.Parse(formula.getvalor("REPETICIONES AL EJE", cara.Presentacion).ToString());
                                                    if (Cavidades == 0)
                                                    {
                                                        Cavidades = int.Parse(formula.getvalor("Repeticiones de etiqueta al eje del suaje ", cara.Presentacion).ToString());
                                                        if (Cavidades == 0)
                                                        {
                                                            Cavidades = int.Parse(formula.getvalor("Repeticiones de etiqueta al eje del suaje", cara.Presentacion).ToString());

                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }

                                }
                            }

                        }
                    }
                }
            }
            catch (Exception err)
            {
                string mensajederror = err.Message;
                Cavidades = 1;
            }

            decimal gapeje;
            try
            {
                gapeje = decimal.Parse(formula.getvalor("GAP EJE", cara.Presentacion).ToString());
                if (gapeje == 0M)
                {
                    gapeje = decimal.Parse(formula.getvalor("GAP AL EJE ", cara.Presentacion).ToString());
                    if (gapeje == 0M)
                    {
                        gapeje = decimal.Parse(formula.getvalor("Separación entre etiquetas", cara.Presentacion).ToString());
                        if (gapeje == 0M)
                        {
                            gapeje = int.Parse(formula.getvalor("Separacion entre etiquetas", cara.Presentacion).ToString());
                            if (gapeje == 0M)
                            {
                                gapeje = int.Parse(formula.getvalor("Repeticiones de etiqueta al eje del suaje ", cara.Presentacion).ToString());
                                if (gapeje == 0M)
                                {
                                    gapeje = int.Parse(formula.getvalor("Repeticiones de etiqueta al eje del suaje", cara.Presentacion).ToString());
                                    if (gapeje == 0M)
                                    {
                                        gapeje = int.Parse(formula.getvalor("Separación entre etiquetas ", cara.Presentacion).ToString());

                                    }
                                }
                            }
                               
                        }

                    }
                }
            }
            catch (Exception err)
            {
                string mensajederror = err.Message;
                gapeje = 0;
            }
            elemento.gapeje = gapeje;
            elemento.cavidades = Cavidades;
            elemento.cavidadesdesuajeEje = Cavidades;

            if (elemento.Tintas.Count == 0)
            {
                try
                {
                    elemento.anchomaterialenmm = int.Parse(formula.getValorCadena("ANCHO BCA", cara.Presentacion));
                    if (elemento.anchomaterialenmm == 0)
                    {
                        throw new Exception("No tiene registro de la ancho bco");
                    }
                }
                catch (Exception err)
                {
                    elemento.anchomaterialenmm = int.Parse(((elemento.anchoproductomm * decimal.Parse(elemento.cavidadesdesuajeEje.ToString())) * decimal.Parse(((elemento.cavidadesdesuajeEje - 1) * elemento.gapeje).ToString())).ToString()) + 8;
                }
            }

            if (elemento.Tintas.Count == 1)
            {

                bool fondeada = false;

                foreach ( Tinta tin in elemento.Tintas)
                {
                    if (tin.Area == 100)
                    {
                        fondeada = true;
                    }
                }
                if (fondeada)
                {
                    try
                    {
                        elemento.anchomaterialenmm = int.Parse(formula.getValorCadena("ANCHO BCA", cara.Presentacion));
                        if (elemento.anchomaterialenmm == 0)
                        {
                            throw new Exception("No tiene registro de la ancho bco");
                        }
                    }

                    catch (Exception err)
                    {
                        elemento.anchomaterialenmm = int.Parse(((elemento.anchoproductomm * decimal.Parse(elemento.cavidadesdesuajeEje.ToString())) * decimal.Parse(((elemento.cavidadesdesuajeEje - 1) * elemento.gapeje).ToString())).ToString()) + 8;
                    }
                }
                else
                {
                    try
                    {
                        elemento.anchomaterialenmm = int.Parse(formula.getValorCadena("ANCHO IMP", cara.Presentacion));
                        if (elemento.anchomaterialenmm == 0)
                        {
                            throw new Exception("No tiene registro de la ancho impreso");
                        }
                    }
                    catch (Exception err)
                    {
                        decimal operacion = (decimal.Parse(elemento.anchoproductomm.ToString()) * decimal.Parse(elemento.cavidadesdesuajeEje.ToString())) * decimal.Parse(((elemento.cavidadesdesuajeEje - 1) * elemento.gapeje).ToString()) + 16;
                        elemento.anchomaterialenmm = Convert.ToInt32(operacion);
                    }
                }
            }


            if (elemento.Tintas.Count >= 2)
            {
                try
                {
                    elemento.anchomaterialenmm = int.Parse(formula.getValorCadena("ANCHO IMP", cara.Presentacion));
                    if (elemento.anchomaterialenmm==0)
                    {
                        throw new Exception("No tiene registro de la ancho impreso");
                    }
                }
                catch(Exception err)
                {
                     
                }
            }




            StringWriter stringwriter = new StringWriter();
            XmlSerializer x = new XmlSerializer(elemento.GetType());
            x.Serialize(stringwriter, elemento);

            string xmlstring = stringwriter.ToString().Replace("encoding=\"utf-16\"", "");


            EscribeArchivoXML(xmlstring, nombredearchivo, true);
            return RedirectToAction("Prioridad");

        }

        public static void EscribeArchivoXML(string contenido, string rutaArchivo, bool sobrescribir = true)
        {

            XmlDocument cotizacion = new XmlDocument();
            cotizacion.LoadXml(contenido);

            XmlTextWriter escribirXML;
            escribirXML = new XmlTextWriter(rutaArchivo, Encoding.UTF8);
            escribirXML.Formatting = Formatting.Indented;
            cotizacion.WriteTo(escribirXML);
            escribirXML.Flush();
            escribirXML.Close();


        }

        public System.Drawing.Image byteArrayToImage(byte[] byteArrayIn)
        {
            System.Drawing.Image returnImage = null;
            try
            {
                MemoryStream ms = new MemoryStream(byteArrayIn, 0, byteArrayIn.Length);
                ms.Write(byteArrayIn, 0, byteArrayIn.Length);
                returnImage = System.Drawing.Image.FromStream(ms, true);//Exception occurs here
            }
            catch { }
            return returnImage;

        }


        
        public ActionResult SubirPrioridad(int? id, int maquina, int? page)
        {
            string cadena = "select distinct(OP.IDORDEN),OP.* from ordenproduccion as op inner join articuloproduccion as ap on op.idorden=ap.idorden inner join EstadoOrden as e on op.estadoorden=e.descripcion where (op.estadoorden='Iniciada' or op.estadoorden='Programada') and ap.idarticulo=" + maquina + " and ap.Idtipoarticulo=3 ";
            List<OrdenProduccion> ele = db.Database.SqlQuery<OrdenProduccion>(cadena).ToList<OrdenProduccion>();

            OrdenProduccion orden = new OrdenProduccionContext().OrdenesProduccion.Find(id);
           ClsDatoEntero prioridad = db.Database.SqlQuery<ClsDatoEntero>("select prioridad as Dato from prioridades where idmaquina='"+maquina+"' and idorden="+ id +"and estado='Programado'").ToList().FirstOrDefault();

            int prioridadarriba = prioridad.Dato - 1;
            int prioridadabajo = prioridadarriba + 1;
            ClsDatoEntero prioridadOrdenInter = db.Database.SqlQuery<ClsDatoEntero>("select IDOrden as Dato from prioridades where prioridad="+ prioridadabajo + "and  idmaquina='" + maquina + "' and estado='Programado'").ToList().FirstOrDefault();

            VMaquinaProceso maquinaproceso = new VMaquinaProcesoContext().VMaquinaProceso.Where(s => s.IDArticulo == maquina).FirstOrDefault();
            int idprocesodelamaquina = maquinaproceso.IDProceso;
            ClsDatoEntero prioridadIn = db.Database.SqlQuery<ClsDatoEntero>("select idorden as Dato from prioridades where idmaquina='" + maquina + "' and prioridad=" + prioridadarriba + "and idproceso="+idprocesodelamaquina+" and estado='Programado'").ToList().FirstOrDefault();
            int idordencambio = prioridadIn.Dato;

            db.Database.ExecuteSqlCommand("update [dbo].[Prioridades] set [prioridad]=" + prioridadarriba + " where estado='Programado' and IDProceso=" + idprocesodelamaquina + " and IDOrden=" + id);
            db.Database.ExecuteSqlCommand("update [dbo].[Prioridades] set [prioridad]=" + prioridadabajo + " where estado='Programado' and IDProceso=" + idprocesodelamaquina + " and IDOrden=" + idordencambio);


            if (page == null)
            {
                page = 1;
            }
            return RedirectToAction("PrioridadMaquina", new { idmaquina = maquina, page = page });
        }

        public ActionResult BajarPrioridad(int? id, int maquina, int? page)
        {
            string cadena = "select distinct(OP.IDORDEN),OP.* from ordenproduccion as op inner join articuloproduccion as ap on op.idorden=ap.idorden inner join EstadoOrden as e on op.estadoorden=e.descripcion where (op.estadoorden='Iniciada' or op.estadoorden='Programada') and ap.idarticulo=" + maquina + " and ap.Idtipoarticulo=3 ";
            List<OrdenProduccion> ele = db.Database.SqlQuery<OrdenProduccion>(cadena).ToList<OrdenProduccion>();

            OrdenProduccion orden = new OrdenProduccionContext().OrdenesProduccion.Find(id);
            ClsDatoEntero prioridad = db.Database.SqlQuery<ClsDatoEntero>("select prioridad as Dato from prioridades where idmaquina='" + maquina + "' and idorden=" + id + "and estado='Programado'").ToList().FirstOrDefault();

            int prioridadabajo = prioridad.Dato + 1;
            int prioridadarriba = prioridadabajo - 1;

            VMaquinaProceso maquinaproceso = new VMaquinaProcesoContext().VMaquinaProceso.Where(s => s.IDArticulo == maquina).FirstOrDefault();
            int idprocesodelamaquina = maquinaproceso.IDProceso;
            ClsDatoEntero prioridadIn = db.Database.SqlQuery<ClsDatoEntero>("select idorden as Dato from prioridades where idmaquina='" + maquina + "' and prioridad=" + prioridadabajo + "and idproceso="+idprocesodelamaquina+" and estado='Programado'").ToList().FirstOrDefault();
            int idordencambio = prioridadIn.Dato;
            db.Database.ExecuteSqlCommand("update [dbo].[Prioridades] set [prioridad]=" + prioridadabajo + " where estado='Programado' and IDProceso=" + idprocesodelamaquina + " and IDOrden=" + id);
            db.Database.ExecuteSqlCommand("update [dbo].[Prioridades] set [prioridad]=" + prioridadarriba + " where estado='Programado' and IDProceso=" + idprocesodelamaquina + " and IDOrden=" + idordencambio);
            if (page == null)
            {
                page = 1;
            }
            return RedirectToAction("PrioridadMaquina", new { idmaquina = maquina, page = page });
        }

        


        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        [HttpPost]
        public JsonResult getcaracteristica(int id)
        {
            VCaracteristicaContext caracteristica = new VCaracteristicaContext();
            var carac = caracteristica.VCaracteristica.Where(x => x.Articulo_IDArticulo == id).ToList();
            List<SelectListItem> listmetodo = new List<SelectListItem>();


            foreach (var x in carac)
            {

                listmetodo.Add(new SelectListItem { Text = x.Presentacion, Value = x.ID.ToString() });

            }



            return Json(new SelectList(listmetodo, "Value", "Text", JsonRequestBehavior.AllowGet));
        }
        [HttpPost]
        public JsonResult getunidad(int id)
        {
            Articulo articulo = new ArticuloContext().Articulo.Find(id);
            c_ClaveUnidad unidad = new c_ClaveUnidadContext().c_ClaveUnidades.Find(articulo.IDClaveUnidad);


            return Json(unidad.Nombre, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult getidunidad(int id)
        {
            Articulo articulo = new ArticuloContext().Articulo.Find(id);
            c_ClaveUnidad unidad = new c_ClaveUnidadContext().c_ClaveUnidades.Find(articulo.IDClaveUnidad);


            return Json(unidad.IDClaveUnidad, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ValeProducto(int? id)
        {
            ViewBag.idorden = id;
            OrdenProduccion ordenp = new OrdenProduccionContext().OrdenesProduccion.Find(id);

            ViewBag.IDArticulo = new SelectList(new ArticuloContext().Articulo.Where(s => s.obsoleto != true), "IDArticulo", "Descripcion");
            ViewBag.IDTrabajador = new SelectList(new TrabajadorContext().Trabajadores, "IDTrabajador", "Nombre");
            ViewBag.IDProceso = new SelectList(new ProcesoContext().Procesos, "IDProceso", "NombreProceso");
            ViewBag.IDAlmacen = new SelectList(new AlmacenContext().Almacenes, "IDAlmacen", "Descripcion");
            string horafecha = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            ViewBag.horafecha = horafecha;

            return View();
        }

        [HttpPost]
        public ActionResult ValeProducto(ValeProducto valeproducto)
        {
            string fecha = valeproducto.Fecha.ToString("yyyy-MM-dd HH:mm:ss");
            List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            int usuario = userid.Select(s => s.UserID).FirstOrDefault();
            db.Database.ExecuteSqlCommand("INSERT INTO [dbo].[ValeProducto]([Fecha],[FechaDevolucion],[IDArticulo],[IDCaracteristica],[IDProceso],[IDOrden],[Cantidad],[IDClaveUnidad],[UserID],[IDTrabajador],[IDAlmacen],[Devuelto]) VALUES('" + fecha + "',null," + valeproducto.IDArticulo + "," + valeproducto.IDCaracteristica + "," + valeproducto.IDProceso + "," + valeproducto.IDOrden + "," + valeproducto.Cantidad + "," + valeproducto.IDClaveUnidad + "," + usuario + "," + valeproducto.IDTrabajador + "," + valeproducto.IDAlmacen + ",0)");

            db.Database.ExecuteSqlCommand("exec dbo.RestarAlmacen " + valeproducto.IDArticulo + "," + valeproducto.IDCaracteristica + "," + valeproducto.Cantidad + "," + valeproducto.IDAlmacen + "");
            return RedirectToAction("AlmacenProduccion", new { id = valeproducto.IDOrden });
        }

        public ActionResult DevolverProducto(int? id)
        {

            ValeProducto ordenp = new ValeProductoContext().ValeProductos.Find(id);
            ViewBag.idorden = ordenp.IDOrden;
            string horafecha = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            ViewBag.horafecha = horafecha;

            ViewBag.id = id;
            return View();
        }

        [HttpPost]
        public ActionResult DevolverProducto(ValeProducto valeproducto, int? id)
        {
            ValeProducto ordenp = new ValeProductoContext().ValeProductos.Find(id);
            string fecha = valeproducto.Fecha.ToString("yyyy-MM-dd HH:mm:ss");
            if (valeproducto.Devuelto <= ordenp.Cantidad)
            {
                db.Database.ExecuteSqlCommand("update [dbo].[ValeProducto] set [FechaDevolucion]='" + fecha + "',[Devuelto]=" + valeproducto.Devuelto + " where [IDValeProducto]=" + id + "");
                db.Database.ExecuteSqlCommand("exec dbo.AgregarAlmacen " + ordenp.IDArticulo + "," + ordenp.IDCaracteristica + "," + valeproducto.Devuelto + "," + ordenp.IDAlmacen + "");
            }
            else
            {
                db.Database.ExecuteSqlCommand("update [dbo].[ValeProducto] set [FechaDevolucion]='" + fecha + "',[Devuelto]=" + ordenp.Cantidad + " where [IDValeProducto]=" + id + "");
                db.Database.ExecuteSqlCommand("exec dbo.AgregarAlmacen " + ordenp.IDArticulo + "," + ordenp.IDCaracteristica + "," + ordenp.Cantidad + "," + ordenp.IDAlmacen + "");
            }
            return RedirectToAction("AlmacenProduccion", new { id = valeproducto.IDOrden });
        }
        public ActionResult AlmacenProduccion(string currentFilter, string searchString, int? page, int? PageSize, int? idorden)
        {
            ValeProductoContext db = new ValeProductoContext();
            if (searchString == null)
            {
                searchString = currentFilter;
            }

            // Pass filtering string to view in order to maintain filtering when paging
            ViewBag.SearchString = searchString;
            //Paginación
            var elementos = from s in db.ValeProductos
                            select s;

            ViewBag.idorden = idorden;
            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                elementos = elementos.Where(s => s.IDOrden.ToString().Contains(searchString));
            }
            if (idorden != null)
            {
                elementos = elementos.Where(s => s.IDOrden == idorden).OrderBy(s => s.IDOrden);
            }
            elementos = elementos.OrderBy(s => s.IDOrden);
            //Paginación
            int count = db.ValeProductos.OrderBy(e => e.IDOrden).Count(); // Total number of elements

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


        public CambioEstadoContext dbce = new CambioEstadoContext();
        public ArticuloContext dbart = new ArticuloContext();
        public OrdenProduccionContext dbpro = new OrdenProduccionContext();
        public OrdenProduccionDetalleContext dbprod = new OrdenProduccionDetalleContext();
        public EstadoOrdenContext dbedoo = new EstadoOrdenContext();
        // GET: CambioEstado

        public ActionResult CambioEstado(int idorden)
        {
            string cadena1 = " select * from dbo.OrdenProduccion where [IDOrden]= " + idorden + "";
            OrdenProduccion ordenp = dbpro.Database.SqlQuery<OrdenProduccion>(cadena1).ToList().FirstOrDefault();
            ViewBag.OrdenP = ordenp;
            ViewBag.IDOrden = idorden;

            ViewBag.idestadoanterior = ordenp.EstadoOrden;



            SelectList lista = new SelectList(new List<SelectListItem>
                {
                     new SelectListItem { Selected=true, Text = ordenp.EstadoOrden, Value = ordenp.EstadoOrden},
                    new SelectListItem {  Text = "Lista", Value = "Lista"},
                    new SelectListItem {  Text = "Cancelada", Value = "Cancelada" },
                     new SelectListItem {  Text = "Conflicto", Value = "Conflicto" },
                        new SelectListItem {  Text = "En Revision", Value = "En Revision" },
                      new SelectListItem {  Text = "Finalizada", Value = "Finalizada" },
                }, "Value", "Text");


            if (ordenp.EstadoOrden == "Conflicto")
            {
                lista = new SelectList(new List<SelectListItem>
                {
                     new SelectListItem { Selected=true, Text = ordenp.EstadoOrden, Value = ordenp.EstadoOrden},

                    new SelectListItem {  Text = "Cancelada", Value = "Cancelada" },

                        new SelectListItem {  Text = "En Revision", Value = "En Revision" },

                }, "Value", "Text");

            }

            if (ordenp.EstadoOrden == "En Revison")
            {
                lista = new SelectList(new List<SelectListItem>
                {
                     new SelectListItem { Selected=true, Text = ordenp.EstadoOrden, Value = ordenp.EstadoOrden},
                       new SelectListItem {  Text = "Lista", Value = "Lista"},
                    new SelectListItem {  Text = "Cancelada", Value = "Cancelada" },
                     new SelectListItem {  Text = "Conflicto", Value = "Conflicto" },


                }, "Value", "Text");

            }
            if (ordenp.EstadoOrden == "Iniciada")
            {
                lista = new SelectList(new List<SelectListItem>
                {
                     new SelectListItem { Selected=true, Text = ordenp.EstadoOrden, Value = ordenp.EstadoOrden},

                    new SelectListItem {  Text = "Cancelada", Value = "Cancelada" },
                     new SelectListItem {  Text = "Conflicto", Value = "Conflicto" },

                      new SelectListItem {  Text = "Finalizada", Value = "Finalizada" },
                }, "Value", "Text");

            }





            ViewBag.EstadoActual = lista;

            ViewBag.FecCambio = DateTime.Today.ToShortDateString();
            ViewBag.HorCambio = DateTime.Now.ToString("HH:mm:ss");
            ViewBag.descripcion = ordenp.EstadoOrden;

            return View();
        }
        // POST: CambioEstado
        [HttpPost]
        public ActionResult CambioEstado(CambioEstado elemento, FormCollection form)
        {
            OrdenProduccion orden = new OrdenProduccionContext().OrdenesProduccion.Find(elemento.IDOrden);
            EncPedido pedido = new PedidoContext().EncPedidos.Find(orden.IDPedido);
            var detallepedido = new PedidoContext().DetPedido.Where(s => s.IDPedido == orden.IDPedido && s.IDArticulo == orden.IDArticulo && s.Caracteristica_ID == orden.IDCaracteristica);

            string estadoActual = form.Get("EstadoA");
            string cadena1 = " select * from dbo.OrdenProduccion where [IDOrden]= " + elemento.IDOrden + "";
            OrdenProduccion ordenp = dbpro.Database.SqlQuery<OrdenProduccion>(cadena1).ToList().FirstOrDefault();
            ViewBag.OrdenP = ordenp;
            ViewBag.IDOrden = elemento.IDOrden;

            try
            {

                User userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList().FirstOrDefault();

                int AlmacenViene = 0;

                foreach (var item in detallepedido)
                {
                    AlmacenViene = item.IDAlmacen;
                }
                string estadoOrden = elemento.EstadoActual;

                Caracteristica carateristica = new WorkinProcessContext().Database.SqlQuery<Caracteristica>("select * from caracteristica where id=" + ordenp.IDCaracteristica).ToList().FirstOrDefault();

                InventarioAlmacen inv = new InventarioAlmacenContext().InventarioAlmacenes.ToList().Where(s => s.IDAlmacen == AlmacenViene && s.IDCaracteristica == ordenp.IDCaracteristica).FirstOrDefault();


                string estadoProceso = "";
                if (estadoOrden =="Cancelada")
                {
                    estadoProceso = "Cancelado";
                }

                if (estadoOrden == "Iniciada")
                {
                    estadoProceso = "Iniciado";
                }
                if (estadoOrden == "Conclicto" || estadoOrden == "En Revision")
                {
                    estadoProceso = "Conflicto";
                }
                if (estadoOrden == "Lista")
                {
                    estadoProceso = "Conflicto";
                }
                if (estadoOrden == "Finalizada")
                {
                    estadoProceso = "Finalizado";
                }

                if (elemento.EstadoActual == "Cancelada")
                {
                    db.Database.ExecuteSqlCommand("update InventarioAlmacen set Porllegar=(Porllegar-" + orden.Cantidad + ") where IDArticulo= " + orden.IDArticulo + " and IDCaracteristica=" + orden.IDCaracteristica + " and IDAlmacen=" + AlmacenViene + "");
                    db.Database.ExecuteSqlCommand("update InventarioAlmacen set Porllegar=0 where IDArticulo= " + orden.IDArticulo + " and IDCaracteristica=" + orden.IDCaracteristica + " and IDAlmacen=" + AlmacenViene + " and porllegar<0");
                    try
                    {
                        string cadenam = "INSERT INTO [dbo].[MovimientoArticulo] ([Fecha],[Id],[IDPresentacion],[Articulo_IDArticulo],[Accion],[cantidad],[Documento],[NoDocumento],[Lote],[IDAlmacen],[TipoOperacion],[acumulado],[observacion],[Hora]) VALUES ";
                        cadenam += "                           (getdate(), " + carateristica.ID + "," + carateristica.IDPresentacion + "," + carateristica.Articulo_IDArticulo + ",'Cancelación Orden'," + orden.Cantidad + ",'Orden Produccion '," + orden.IDOrden + ",''," + AlmacenViene + ",'N/A'," + inv.Existencia + ",'',CONVERT (time, SYSDATETIMEOFFSET()))";
                        db.Database.ExecuteSqlCommand(cadenam);
                    }
                    catch (Exception err)
                    {

                    }
                    try
                    {
                        List<MaterialAsignado> solicitud = db.Database.SqlQuery<MaterialAsignado>("Select * from MaterialAsignado where [orden]=" + elemento.IDOrden).ToList();

                        foreach (MaterialAsignado item in solicitud)
                        {

                            decimal m2 = decimal.Parse(item.cantidad.ToString()) * (decimal.Parse(item.ancho.ToString()) / 1000);

                            db.Database.ExecuteSqlCommand("update [dbo].[InventarioAlmacen] set disponibilidad= (disponibilidad +" + m2 + "), [apartado]= (apartado-" + m2 + ") where idalmacen=" + item.idalmacen + " and idarticulo=" + item.idmapri + "and idcaracteristica=" + item.idcaracteristica);

                            Caracteristica caracteristica = new Caracteristica();
                            caracteristica = db.Database.SqlQuery<Caracteristica>("Select * from Caracteristica where ID=" + item.idcaracteristica).ToList().FirstOrDefault();
                            InventarioAlmacen inventario = new InventarioAlmacenContext().InventarioAlmacenes.ToList().Where(a => a.IDAlmacen == item.idalmacen && a.IDCaracteristica == item.idcaracteristica).FirstOrDefault();


                            try
                            {
                                string cadenam = "INSERT INTO [dbo].[MovimientoArticulo] ([Fecha],[Id],[IDPresentacion],[Articulo_IDArticulo],[Accion],[cantidad],[Documento],[NoDocumento],[Lote],[IDAlmacen],[TipoOperacion],[acumulado],[observacion],[Hora]) VALUES ";
                                cadenam += " (getdate(), " + caracteristica.ID + "," + caracteristica.IDPresentacion + "," + caracteristica.Articulo_IDArticulo + ",'Eliminación de Asignación de Material en Producción'," + m2 + ",'Orden Producción '," + item.orden + ",''," + item.idalmacen + ",'N/A'," + inventario.Existencia + ",'',CONVERT (time, SYSDATETIMEOFFSET()))";
                                db.Database.ExecuteSqlCommand(cadenam);
                            }
                            catch (Exception err)
                            {
                                string mensajee = err.Message;
                            }

                            db.Database.ExecuteSqlCommand("delete from  MaterialAsignado where idmaterialasignado=" + item.IDMaterialAsignado);


                        }

                    }
                    catch (Exception err)
                    {

                    }
                    try
                    {
                        List<WorkinProcess> work = db.Database.SqlQuery<WorkinProcess>("Select * from WorkinProcess where [idorden]=" + elemento.IDOrden).ToList();

                        foreach (WorkinProcess registro in work)
                        {
                            decimal largo = registro.ocupadolineal;
                            string[] cadenas = registro.loteinterno.Split('/');
                            string ancho = cadenas[1];

                            decimal m2 = largo * (decimal.Parse(ancho) / 1000);

                            db.Database.ExecuteSqlCommand("update clslotemp set metrosdisponibles=(metrosdisponibles+" + m2 + "), metrosutilizados=(metrosutilizados-" + m2 + ")  where ID =" + registro.IDlotemp);
                            db.Database.ExecuteSqlCommand("update MaterialAsignado set entregado=0 where orden=" + registro.Orden + " and entregado<0");


                            db.Database.ExecuteSqlCommand("delete from [dbo].[WorkinProcess] where idworkingprocess=" + registro.IDWorkingProcess);
                            db.Database.ExecuteSqlCommand("update inventarioalmacen set Existencia=(Existencia+" + m2 + "), apartado=(apartado+" + m2 + ") where idarticulo= " + registro.IDArticulo + " and idalmacen=" + registro.IDAlmacen + " and idcaracteristica=" + registro.IDCaracteristica);
                            db.Database.ExecuteSqlCommand("update inventarioalmacen set Disponibilidad=(Existencia-apartado) where idarticulo= " + registro.IDArticulo + " and idalmacen=" + registro.IDAlmacen + " and idcaracteristica=" + registro.IDCaracteristica);

                            Caracteristica carateristicaw = new WorkinProcessContext().Database.SqlQuery<Caracteristica>("select * from caracteristica where id=" + registro.IDCaracteristica).ToList().FirstOrDefault();
                            InventarioAlmacen invw = new InventarioAlmacenContext().InventarioAlmacenes.ToList().Where(s => s.IDAlmacen == registro.IDAlmacen && s.IDCaracteristica == registro.IDCaracteristica).FirstOrDefault();

                            int usuario = userid.UserID;


                            try
                            {
                                string cadena = "INSERT INTO [dbo].[MovimientoArticulo] ([Fecha],[Id],[IDPresentacion],[Articulo_IDArticulo],[Accion],[cantidad],[Documento],[NoDocumento],[Lote],[IDAlmacen],[TipoOperacion],[acumulado],[observacion],[Hora], usuario) VALUES ";
                                cadena += " (getdate(), " + carateristica.ID + "," + carateristica.IDPresentacion + "," + carateristica.Articulo_IDArticulo + ",'Regreso de Produccion por cancelación de OP'," + m2 + ",'Orden Produccion '," + registro.Orden + ",'" + registro.loteinterno + "'," + registro.IDAlmacen + ",'E'," + inv.Existencia + ",'Cinta devuelta',CONVERT (time, SYSDATETIMEOFFSET()), " + usuario + ")";
                                db.Database.ExecuteSqlCommand(cadena);
                            }
                            catch (Exception err)
                            {
                                string mensajee = err.Message;
                            }


                        }
                    }
                    catch (Exception err)
                    {

                    }
                   

                }

                string motivo = string.Empty;



                if (estadoOrden == "Cancelada")
                {
                    string cadenaP = "update prioridades set estado='Cancelado', prioridad=0 where idorden= "+ elemento.IDOrden;
                    dbce.Database.ExecuteSqlCommand(cadenaP);

                }
                if (estadoOrden == "Lista")
                {
                    string cadenaP = "Delete from prioridades where idorden=" + elemento.IDOrden;
                    dbce.Database.ExecuteSqlCommand(cadenaP);

                }
                if (estadoOrden == "Conflicto" || estadoOrden == "En Revision")
                {
                    string cadenaP = "Delete from prioridades where idorden=" + elemento.IDOrden;
                    dbce.Database.ExecuteSqlCommand(cadenaP);

                }
                if (estadoOrden == "Finalizada")
                {
                    string cadenaP = "Delete from prioridades where idorden=" + elemento.IDOrden;
                    dbce.Database.ExecuteSqlCommand(cadenaP);

                }



                try
                {
                    motivo = elemento.motivo;
                }
                catch (Exception err)
                {

                }
                /// verificar automata proceso
                string cadena3 = " insert into dbo.CambioEstado(IDOrden, fecha, hora, EstadoAnterior, EstadoActual, motivo, usuario) values (" + ViewBag.IDOrden + ",'" + DateTime.Now.ToString("yyyy-MM-dd") + "','" + DateTime.Now.ToString("HH:mm:ss") + "','" + estadoActual + "', '" + estadoOrden + "', '" + motivo + "','" + userid.Username + "')";
                dbce.Database.ExecuteSqlCommand(cadena3);

                string cadena4 = " Update dbo.OrdenProduccion set EstadoOrden='" + estadoOrden + "', prioridad=0 where IDOrden= " + ViewBag.IDOrden + "";
                dbpro.Database.ExecuteSqlCommand(cadena4);

                db.Database.ExecuteSqlCommand("update OrdenProduccionDetalle set EstadoProceso='"+ estadoProceso + "' where IDOrden= " + elemento.IDOrden);

                

                return RedirectToAction("Prioridad");
            }
            catch (Exception ERR)
            {
                ViewBag.idestadoanterior = ordenp.EstadoOrden;


                SelectList lista = new SelectList(new List<SelectListItem>
                {
                     new SelectListItem { Selected=true, Text = ordenp.EstadoOrden, Value = ordenp.EstadoOrden},
                    new SelectListItem {  Text = "Lista", Value = "Lista"},
                    new SelectListItem {  Text = "Cancelada", Value = "Cancelada" },
                });

                ViewBag.IDEstadoActual = lista;

                ViewBag.edoorden = ordenp.EstadoOrden; ;
                return View();
            }
        }
 public ActionResult EnRevision(int IDOrden, int? page, int? PageSize)


        {

            int pageNumber = (page ?? 1);
            int pageSize = (PageSize ?? 10);

            OrdenProduccion orden = new OrdenProduccionContext().OrdenesProduccion.Find(IDOrden);
            EncPedido pedido = new PedidoContext().EncPedidos.Find(orden.IDPedido);
            var detallepedido = new PedidoContext().DetPedido.Where(s => s.IDPedido == orden.IDPedido && s.IDArticulo == orden.IDArticulo && s.Caracteristica_ID == orden.IDCaracteristica).FirstOrDefault();

            string estadoActual =orden.EstadoOrden;
            string cadena1 = " select * from dbo.OrdenProduccion where [IDOrden]= " + IDOrden + "";
            OrdenProduccion ordenp = dbpro.Database.SqlQuery<OrdenProduccion>(cadena1).ToList().FirstOrDefault();
            ViewBag.OrdenP = ordenp;
            ViewBag.IDOrden = IDOrden;

            try
            {

                User userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList().FirstOrDefault();

                int AlmacenViene = 0;

                
                string estadoOrden = "En Revision";

                Caracteristica carateristica = new WorkinProcessContext().Database.SqlQuery<Caracteristica>("select * from caracteristica where id=" + ordenp.IDCaracteristica).ToList().FirstOrDefault();

                InventarioAlmacen inv = new InventarioAlmacenContext().InventarioAlmacenes.ToList().Where(s => s.IDAlmacen == AlmacenViene && s.IDCaracteristica == ordenp.IDCaracteristica).FirstOrDefault();


                string estadoProceso = "";
               
                    estadoProceso = "Conflicto";
                

               
                string motivo = string.Empty;



                if (estadoOrden == "Conflicto" || estadoOrden == "En Revision")
                {
                    string cadenaP = "Delete from prioridades where idorden=" + IDOrden;
                    dbce.Database.ExecuteSqlCommand(cadenaP);

                }




                motivo = "Se reviso la orden";
                /// verificar automata proceso
                string cadena3 = " insert into dbo.CambioEstado(IDOrden, fecha, hora, EstadoAnterior, EstadoActual, motivo, usuario) values (" + ViewBag.IDOrden + ",'" + DateTime.Now.ToString("yyyy-MM-dd") + "','" + DateTime.Now.ToString("HH:mm:ss") + "','" + estadoActual + "', '" + estadoOrden + "', '" + motivo + "','" + userid.Username + "')";
                dbce.Database.ExecuteSqlCommand(cadena3);

                string cadena4 = " Update dbo.OrdenProduccion set EstadoOrden='" + estadoOrden + "', prioridad=0 where IDOrden= " + IDOrden + "";
                dbpro.Database.ExecuteSqlCommand(cadena4);

                db.Database.ExecuteSqlCommand("update OrdenProduccionDetalle set EstadoProceso='" + estadoProceso + "' where IDOrden= " + IDOrden);


             
                return RedirectToAction("Prioridad",new { page = page, PageSize = PageSize } );
            }

            catch (Exception ERR)
            {
                return RedirectToAction("Prioridad", new { page = page, PageSize = PageSize });
            }
        }
        public ActionResult DetailsLOP(int? id)
        {

            OrdenProduccion ordendeProduccion = db.OrdenesProduccion.Find(id);
            VCaracteristica caracteristica = new VCaracteristicaContext().VCaracteristica.Find(ordendeProduccion.IDCaracteristica);

            ClsDatoEntero countlibera = db.Database.SqlQuery<ClsDatoEntero>("select count(IDOrden) as Dato from LiberaOrdenProduccion where IDOrden=" + id + "").ToList()[0];
            if (countlibera.Dato != 0)
            {
                List<LiberaOrden> libera = db.Database.SqlQuery<LiberaOrden>("select LO.IDOrden,LO.FechaLiberacion,LO.Cantidad,(select Username from [dbo].[User] where UserID=LO.UserID) as Usuario,(select Nombre from c_ClaveUnidad where IDClaveUnidad=A.IDClaveUnidad) as Unidad from LiberaOrdenProduccion as LO inner join OrdenProduccion as OP on OP.IDOrden=LO.IDOrden inner join Articulo as A on A.IDArticulo=OP.IDArticulo where OP.IDOrden=" + id + "").ToList();
                ViewBag.Libera = libera;
            }
            else
            {
                ViewBag.Libera = null;
            }

            return View(ordendeProduccion);
        }


        public ActionResult Incidencias(int id)
        {
            OrdenProduccionContext db1 = new OrdenProduccionContext();
            string cadena1 = " select * from dbo.V_Incidencias where [IDOrden]= " + id + "";
            List<V_Incidencias> elemento = db1.Database.SqlQuery<V_Incidencias>(cadena1).ToList();
            ViewBag.incidencia = elemento;

            return View(elemento);
        }
        
        public ActionResult CambiarMaquina(int id, int idmaquinanueva, int orden, int proceso)
        {

            try
            {
                Caracteristica caracte = db.Database.SqlQuery<Caracteristica>("select * from Caracteristica where Articulo_IDArticulo=" + idmaquinanueva).ToList().FirstOrDefault();

 				db.Database.ExecuteSqlCommand("update articuloproduccion set IdArticulo=" + idmaquinanueva + ", IDCaracteristica=" + caracte.ID + " where idorden=" + orden + " and IDProceso=" + proceso + " and idarticulo=" + id);
                db.Database.ExecuteSqlCommand("update prioridades set idmaquina=" + idmaquinanueva + " where estado='Programado' and idorden=" + orden + " and IDProceso=" + proceso);

			return Json(new HttpStatusCodeResult(200, "La maquina fue cambiada exitosamente!"));
            }
            catch (SqlException err)
            {
                string mensajederror = err.Message;
                return Json(new HttpStatusCodeResult(500, "No Pude"));
            }



        }

        /// <summary>
        /// //
        /// </summary>
        /// <param name="id"></param>
        /// <param name="idmaquinanueva"></param>
        /// <param name="orden"></param>
        /// <param name="proceso"></param>
        /// <returns></returns>

        public ActionResult CambiarCaracteristica(int id, int idcaracteristicanueva, int orden, int caracteristica)
        {

            try
            {
                Caracteristica caracte = db.Database.SqlQuery<Caracteristica>("select * from Caracteristica where ID=" + idcaracteristicanueva).ToList().FirstOrDefault();


                db.Database.ExecuteSqlCommand("update articuloproduccion set IdArticulo=" + caracte.Articulo_IDArticulo + ", IDCaracteristica=" + idcaracteristicanueva + " where idArtProd=" + id);
                return Json(new HttpStatusCodeResult(200, "Fue cambiada exitosamente!"));
            }
            catch (SqlException err)
            {
                string mensajederror = err.Message;
                return Json(new HttpStatusCodeResult(500, "No Pude"));
            }

        }


        public ActionResult Asignaciondemateriales(int? page, int? PageSize, string searchString = "")
        {

            var elementos = from s in db.OrdenesProduccion.Where(s => (s.EstadoOrden != "Cancelada" && (s.EstadoOrden == "Conflicto" || s.EstadoOrden == "Programada"))).OrderBy(s => s.IDOrden)
                            select s;

            ViewBag.searchString = searchString;

            if (searchString != null && searchString != string.Empty)
            {
                elementos.Where(s => s.IDOrden.ToString() == searchString || s.Cliente.Nombre.Contains(searchString) || s.Articulo.Cref.Contains(searchString));
            }
            int pageNumber = (page ?? 1);
            int pageSize = (PageSize ?? 10);
            ViewBag.psize = pageSize;
            return View(elementos.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult AsignaciondemateOrden(int IDOrden, string searchString = "")
        {
            OrdenProduccion ordenproduccion = new OrdenProduccionContext().OrdenesProduccion.Find(IDOrden);

            ViewBag.orden = ordenproduccion;

            var ordendeprocesos = new OrdenProduccionDetalleContext().OrdenProduccionDetalles.ToList().Where(s => s.IDOrden == IDOrden).OrderBy(s => s.IDOrdenDetalle);

            ViewBag.orden = ordenproduccion;
            ViewBag.ordendeprocesos = ordendeprocesos;

            var articulosproduccion = new ArticulosProduccionContext().ArticulosProducciones.ToList().Where(s => s.IDOrden == IDOrden);

            return View(articulosproduccion);
        }

        public ActionResult AddWip(FormCollection forma, string searchString = "")
        {
            int IDOrden = int.Parse(forma.Get("IDOrden"));
            int IDAlmacen = int.Parse(forma.Get("IDAlmacen"));
            int IDCaracteristica = int.Parse(forma.Get("IDCaracteristica"));
            int IDProceso = int.Parse(forma.Get("IDProceso"));
            decimal Cantidad = decimal.Parse(forma.Get("Cantidad"));

            Caracteristica cara = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from Caracteristica where ID=" + IDCaracteristica).ToList()[0];

            WorkinProcess WIP = new WorkinProcess();



            WIP.IDAlmacen = IDAlmacen;
            WIP.IDproceso = IDProceso;

            WIP.IDArticulo = cara.Articulo_IDArticulo;

            WIP.IDCaracteristica = IDCaracteristica;
            WIP.Cantidad = Cantidad;
            WIP.Devuelto = 0;
            WIP.loteinterno = "";

            WIP.Merma = 0;
            WIP.Ocupado = 0;
            WIP.Observacion = String.Empty;

            WorkinProcessContext db = new WorkinProcessContext();

            db.WIP.Add(WIP);
            db.SaveChanges();


            return RedirectToAction("AsignaciondemateOrden", new { IDOrden = IDOrden, searchString = searchString });
        }




        public ActionResult BuscarOrden(int? id)
        {
            ViewBag.idOrden = "";
            ViewBag.mensaje = "";
            try
            {
                ClsDatoEntero cuenta = db.Database.SqlQuery<ClsDatoEntero>("select count([IDOrden]) as Dato from [OrdenProduccion] where IdOrden=" + id + "").ToList()[0];
                int valor = cuenta.Dato;
                if (valor != 0)
                {
                    ViewBag.idOrden = id;
                    List<OrdenProduccion> ordenes = new OrdenProduccionContext().OrdenesProduccion.Where(s => s.IDOrden == id).ToList();

                    return View(ordenes);

                }

                else
                {
                    ViewBag.mensaje = "Orden Inexistente";
                }
            }
            catch (Exception err)
            {

            }

            return View();

        }
        public ActionResult getJsonCaracteristicaArticuloMaterial(int id)
        {
            var presentacion = new ArticuloRepository().GetCaracteristicaPorArticulosindefecto(id);
            return Json(presentacion, JsonRequestBehavior.AllowGet);

        }

        public IEnumerable<SelectListItem> getPresentacionPorArticuloMaterial(int ida)
        {
            var presentacion = new ArticuloRepository().GetCaracteristicaPorArticulo(ida);
            return presentacion;

        }
        [HttpPost]
        public ActionResult CambiarMaterial(int? id, int IDArtProduc, decimal cantidad, int? almacen, int? articulo, int? presentacion, int idorden, string cara, decimal ex)
        {

            FormulaSiaapi.Formulas formu = new FormulaSiaapi.Formulas();
            string ancho1 = formu.getValorCadena("ANCHO", cara);

            string largo1 = formu.getValorCadena("LARGO", cara);
            decimal metroscuadrados = cantidad * (Convert.ToDecimal(ancho1) / 1000);

            decimal m2 = Math.Round(metroscuadrados, 3);
            decimal anReal = 0;
            decimal larReal = 0;
            try
            {

                List<MaterialAsignado> ele = new InventarioAlmacenContext().Database.SqlQuery<MaterialAsignado>("select*from MaterialAsignado where orden=" + idorden).ToList();
                ViewBag.mensaje = "";


                bool insertar = true;
                foreach (MaterialAsignado valores in ele)
                {
                    anReal = valores.ancho;
                    larReal = valores.largo;
                    if (IDArtProduc == valores.idmapri)
                    {
                        insertar = false;
                    }

                }
                if (insertar)
                {


                    string cadena3 = " insert into dbo.MaterialAsignado(Orden, idmapri, ancho, largo, cantidad, idcaracteristica, idalmacen) values (" + idorden + "," + Convert.ToInt32(articulo) + "," + Convert.ToInt32(ancho1) + "," + Convert.ToInt32(largo1) + ", " + cantidad + "," + presentacion + ", " + almacen + ")";
                    db.Database.ExecuteSqlCommand(cadena3);
                    db.Database.ExecuteSqlCommand("update [dbo].[InventarioAlmacen] set  [apartado]=( apartado+" + m2 + ") where idalmacen=" + almacen + " and idarticulo=" + Convert.ToInt32(articulo) + " and idcaracteristica=" + presentacion);

                    db.Database.ExecuteSqlCommand("update [dbo].[InventarioAlmacen] set apartado=0 where apartado<0 and idalmacen=" + almacen + " and idarticulo=" + Convert.ToInt32(articulo) + " and idcaracteristica=" + presentacion);

                    db.Database.ExecuteSqlCommand("update [dbo].[InventarioAlmacen] set disponibilidad=(existencia-apartado) where  idalmacen=" + almacen + " and idarticulo=" + Convert.ToInt32(articulo) + " and idcaracteristica=" + presentacion);


                    Caracteristica caracteristica = new Caracteristica();
                    caracteristica = db.Database.SqlQuery<Caracteristica>("Select * from Caracteristica where ID=" + presentacion).ToList().FirstOrDefault();
                    InventarioAlmacen inventario = new InventarioAlmacenContext().InventarioAlmacenes.ToList().Where(a => a.IDAlmacen == almacen && a.IDCaracteristica == presentacion).FirstOrDefault();


                    try
                    {
                        string cadenam = "INSERT INTO [dbo].[MovimientoArticulo] ([Fecha],[Id],[IDPresentacion],[Articulo_IDArticulo],[Accion],[cantidad],[Documento],[NoDocumento],[Lote],[IDAlmacen],[TipoOperacion],[acumulado],[observacion],[Hora]) VALUES ";
                        cadenam += " (getdate(), " + caracteristica.ID + "," + caracteristica.IDPresentacion + "," + caracteristica.Articulo_IDArticulo + ",'Asignación Material en Producción'," + m2 + ",'Orden Producción '," + idorden + ",''," + almacen + ",'N/A'," + inventario.Existencia + ",'',CONVERT (time, SYSDATETIMEOFFSET()))";
                        db.Database.ExecuteSqlCommand(cadenam);
                    }
                    catch (Exception err)
                    {
                        string mensajee = err.Message;
                    }

                }

                try
                {

                }
                catch (Exception)
                {

                }
             return RedirectToAction("AsignarMaterial", new { IDArtPro = IDArtProduc, idorden = idorden });



            }
            catch
            {
                return RedirectToAction("AsignarMaterial", new { IDArtPro = IDArtProduc, idorden = idorden });
            }
        }

        
        public ActionResult CambiarSuaje(int IDArtPro)
        {
            ArticuloProduccion articulopro = new ArticulosProduccionContext().ArticulosProducciones.Find(IDArtPro);
            Articulo artdes = new ArticuloContext().Articulo.Find(articulopro.IDArticulo);



            Caracteristica cara = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from Caracteristica where id=" + articulopro.IDCaracteristica).ToList().FirstOrDefault();
            FormulaSiaapi.Formulas formux = new FormulaSiaapi.Formulas();

            string Ceje = "0";
            Ceje = formux.getValorCadena("CAV EJE", cara.Presentacion);
            if (Ceje == "")
            {
                Ceje = formux.getValorCadena("CAVIDADES EJE", cara.Presentacion);
                if (Ceje == "")
                {
                    Ceje = formux.getValorCadena("CAV AL EJE", cara.Presentacion);
                    if (Ceje == "")
                    {
                        Ceje = formux.getValorCadena("CAVIDADES AL EJE", cara.Presentacion);
                        if (Ceje == "")
                        {
                            Ceje = formux.getValorCadena("CAV EJE ", cara.Presentacion);
                            if (Ceje == "")
                            {
                                Ceje = formux.getValorCadena("CAV AL EJE ", cara.Presentacion);
                                if (Ceje == "")
                                {
                                    Ceje = formux.getValorCadena("REP EJE ", cara.Presentacion);
                                    if (Ceje == "")
                                    {
                                        Ceje = formux.getValorCadena("REPETICIONES AL EJE", cara.Presentacion);
                                        if (Ceje == "")
                                        {
                                            Ceje = formux.getValorCadena("REPETICIONES EJE", cara.Presentacion);

                                        }
                                    }
                                }
                            }
                        }
                    }

                }
            }


            ViewBag.Ceje = Ceje;

            ViewBag.DescripcionArt = artdes.Descripcion;


            ArticuloContext dbar = new ArticuloContext();
            var articulos = dbar.Database.SqlQuery<Articulo>("select *  from Articulo where idtipoarticulo=2 and  obsoleto='0' and descripcion Like 'SUAJE%' order by cref").ToList().OrderBy(i => i.Descripcion).ToList();


            ViewBag.Articulop = new SelectList(articulos, "IDARTICULO", "DESCRIPCION", articulopro.IDArticulo);


            var datosArticuloF = dbar.Database.SqlQuery<Articulo>("select *  from Articulo where idtipoarticulo=2 and  obsoleto='0' and (idfamilia=77 or idfamilia=76 or idfamilia=80 or idfamilia=91 or idfamilia=81 or idfamilia=75 or idfamilia=71 or idfamilia=93 or idfamilia=72 or idfamilia=11 or idfamilia=69) order by cref").ToList();
            List<SelectListItem> liAC = new List<SelectListItem>();
            liAC.Add(new SelectListItem { Text = "--Selecciona un Suaje--", Value = "0" });
            foreach (var a in datosArticuloF)
            {
                liAC.Add(new SelectListItem { Text = a.Cref + " | " + a.Descripcion, Value = a.IDArticulo.ToString() });

            }
            ViewBag.idarticulo = liAC;
            ViewBag.IDCaracteristica = getPresentacionPorArticuloMaterial(0);
            return View(articulopro);
        }

        [HttpPost]
        public ActionResult CambiarSuaje(ArticuloProduccion ArtPro, FormCollection coleccion)
        {

            try
            {
                Caracteristica cara = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from Caracteristica where id=" + ArtPro.IDCaracteristica).ToList().FirstOrDefault();
                FormulaSiaapi.Formulas formux = new FormulaSiaapi.Formulas();

                string Ceje = "0";
                Ceje = formux.getValorCadena("CAV EJE", cara.Presentacion);

             
                if (Ceje == "")
                {
                    Ceje = formux.getValorCadena("CAVIDADES EJE", cara.Presentacion);
                    if (Ceje == "")
                    {
                        Ceje = formux.getValorCadena("CAV AL EJE", cara.Presentacion);
                        if (Ceje == "")
                        {
                            Ceje = formux.getValorCadena("CAVIDADES AL EJE", cara.Presentacion);
                            if (Ceje == "")
                            {
                                Ceje = formux.getValorCadena("CAV EJE ", cara.Presentacion);
                                if (Ceje == "")
                                {
                                    Ceje = formux.getValorCadena("CAV AL EJE ", cara.Presentacion);
                                    if (Ceje == "")
                                    {
                                        Ceje = formux.getValorCadena("REP EJE", cara.Presentacion);
                                        if (Ceje == "")
                                        {
                                            Ceje = formux.getValorCadena("REPETICIONES EJE", cara.Presentacion);
                                            if (Ceje == "")
                                            {
                                                Ceje = formux.getValorCadena("REPETICIONES AL EJE", cara.Presentacion);

                                            }

                                        }
                                    }
                                }
                            }
                        }

                    }
                }
                
              int cavidades = 1;

                if (Ceje != "0")
                {
                    cavidades = int.Parse(Ceje);
                }
                else
                {
                    string cavitem = coleccion.Get("Cavidades").ToString();
                    cavidades = int.Parse(cavitem);
                }

                OrdenProduccion orden = new OrdenProduccionContext().OrdenesProduccion.Find(ArtPro.IDOrden);

                decimal largo = 0;

                
                List<ArticuloProduccion> arti = new ArticulosProduccionContext().ArticulosProducciones.Where(s => s.IDOrden == ArtPro.IDOrden && s.IDTipoArticulo == 6).ToList();

                foreach (ArticuloProduccion ap in arti)
                {
                   
                    Caracteristica carabobina = new ArticuloContext().Database.SqlQuery<Caracteristica>("Select * from caracteristica where id=" + ap.IDCaracteristica).FirstOrDefault();
                    string ancho = formux.getValorCadena("ANCHO", carabobina.Presentacion);
                    decimal anchod = decimal.Parse(ancho);
                    largo = ap.Cantidad * (anchod * 1000M);
                    decimal m2 = largo * (anchod / 1000M);
                    db.Database.ExecuteSqlCommand("update ArticuloProduccion set Cantidad=" + m2 + " where IDArtProd= " + ap.IDArtProd);

                }

                db.Database.ExecuteSqlCommand("update [dbo].[ArticuloProduccion] set [IDArticulo]=" + ArtPro.IDArticulo + ",[IDCaracteristica]=" + ArtPro.IDCaracteristica + " where IDArtProd=" + ArtPro.IDArtProd + "");
                db.SaveChanges();
                return RedirectToAction("Prioridad");



            }
            catch(Exception err)
            {
                return RedirectToAction("CambiarSuaje", new { IDArtPro = ArtPro.IDArtProd });
            }
        }

        
        
        
        public ActionResult CambiarPrioridad(FormCollection colleccion)
        {
            int orden = int.Parse(colleccion.Get("op").ToString());
            int prioridad = int.Parse(colleccion.Get("cambioPriori").ToString());
            int maquina = int.Parse(colleccion.Get("maquina").ToString());

            string cadena = "select distinct(OP.IDORDEN),OP.* from ordenproduccion as op inner join articuloproduccion as ap on op.idorden=ap.idorden inner join EstadoOrden as e on op.estadoorden=e.descripcion where (op.estadoorden='Iniciada' or op.estadoorden='Programada') and ap.idarticulo=" + maquina + " and ap.Idtipoarticulo=3 order by prioridad";
            List<OrdenProduccion> ele = db.Database.SqlQuery<OrdenProduccion>(cadena).ToList<OrdenProduccion>();

  			VMaquinaProceso maquinaproceso = new VMaquinaProcesoContext().VMaquinaProceso.Where(s => s.IDArticulo == maquina).FirstOrDefault();
            int idprocesodelamaquina = maquinaproceso.IDProceso;

            foreach (OrdenProduccion ordenp in ele)
            {
                if (ordenp.Prioridad >= prioridad)
                {
                    new OrdenProduccionContext().Database.ExecuteSqlCommand("update prioridades set prioridad = prioridad + 1 where IDOrden=" + ordenp.IDOrden);
                }
            }
            db.Database.ExecuteSqlCommand("update [dbo].[Prioridades] set [prioridad]=" + prioridad + " where estado='Programado' and IDProceso=" + idprocesodelamaquina + " and idmaquina=" + maquina + "and IDOrden=" + orden);

            return RedirectToAction("PrioridadMaquina", new { idmaquina = maquina });
        }



        public ActionResult CambiarPleca(int IDArtPro)
        {
            ArticuloProduccion articulopro = new ArticulosProduccionContext().ArticulosProducciones.Find(IDArtPro);
            Articulo artdes = new ArticuloContext().Articulo.Find(articulopro.IDArticulo);

            ViewBag.DescripcionArt = artdes.Descripcion;


            var articulos = new ArticuloContext().Articulo.Where(s => s.IDFamilia == artdes.IDFamilia);

           

            ArticuloContext dbar = new ArticuloContext();
            var datosArticuloF = dbar.Database.SqlQuery<Articulo>("select distinct Articulo.*  from Articulo inner join Caracteristica on Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo Where IDFamilia=" + artdes.IDFamilia).ToList().OrderBy(i => i.Cref).ToList();
            List<SelectListItem> liAC = new List<SelectListItem>();
            liAC.Add(new SelectListItem { Text = "Sin Pleca", Value = "0" });
            foreach (var a in datosArticuloF)
            {
                if (a.IDArticulo== artdes.IDArticulo)
                {
                    liAC.Add(new SelectListItem { Text = a.Cref + " | " + a.Descripcion, Value = a.IDArticulo.ToString(), Selected=true });
                }
                else
                {
                    liAC.Add(new SelectListItem { Text = a.Cref + " | " + a.Descripcion, Value = a.IDArticulo.ToString() });
                }

            }
            ViewBag.idarticulo = liAC;



            ViewBag.IDCaracteristica = getPresentacionPorArticuloMaterial(0);
            return View(articulopro);
        }

        [HttpPost]
        public ActionResult CambiarPleca(ArticuloProduccion ArtPro)
        {

            try
            {

                db.Database.ExecuteSqlCommand("update [dbo].[ArticuloProduccion] set [IDArticulo]=" + ArtPro.IDArticulo + ",[IDCaracteristica]=" + ArtPro.IDCaracteristica + " where IDArtProd=" + ArtPro.IDArtProd + "");
                db.SaveChanges();
                return RedirectToAction("Prioridad");

            }
            catch
            {
                return View();
            }
        }

        public ActionResult CambiarCentro(int IDArtPro)
        {
            ArticuloProduccion articulopro = new ArticulosProduccionContext().ArticulosProducciones.Find(IDArtPro);
            Articulo artdes = new ArticuloContext().Articulo.Find(articulopro.IDArticulo);

            ViewBag.DescripcionArt = artdes.Descripcion;


            var articulos = new ArticuloContext().Articulo.Where(s => s.IDFamilia == artdes.IDFamilia);

           
            ArticuloContext dbar = new ArticuloContext();
            var datosArticuloF = dbar.Database.SqlQuery<Articulo>("select distinct Articulo.*  from Articulo inner join Caracteristica on Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo Where IDFamilia=" + artdes.IDFamilia).ToList().OrderBy(i => i.Cref).ToList();
            List<SelectListItem> liAC = new List<SelectListItem>();
            liAC.Add(new SelectListItem { Text = "  N/A", Value = "0" });
            foreach (var a in datosArticuloF)
            {
                if (a.IDArticulo == artdes.IDArticulo)
                {
                    liAC.Add(new SelectListItem { Text = a.Cref + " | " + a.Descripcion, Value = a.IDArticulo.ToString(), Selected = true });
                }
                else
                {
                    liAC.Add(new SelectListItem { Text = a.Cref + " | " + a.Descripcion, Value = a.IDArticulo.ToString() });
                }

            }
            ViewBag.idarticulo = liAC;



            ViewBag.IDCaracteristica = getPresentacionPorArticuloMaterial(0);
            return View(articulopro);
        }

        [HttpPost]
        public ActionResult CambiarCentro(ArticuloProduccion ArtPro)
        {

            try
            {

                db.Database.ExecuteSqlCommand("update [dbo].[ArticuloProduccion] set [IDArticulo]=" + ArtPro.IDArticulo + ",[IDCaracteristica]=" + ArtPro.IDCaracteristica + " where IDArtProd=" + ArtPro.IDArtProd + "");
                db.SaveChanges();
                return RedirectToAction("Prioridad");

            }
            catch
            {
                return View();
            }
        }


        [HttpPost]
        public JsonResult EliminarPleca(int id)
        {
            CarritoContext car = new CarritoContext();
            try
            {
              
                List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
                int usuario = userid.Select(s => s.UserID).FirstOrDefault();
                try
                {
                    ArticuloProduccion produccion = new ArticulosProduccionContext().ArticulosProducciones.Find(id);
                    string cadena = "insert into [RegistroEliminacionArtOPSuajePleca] (IDArticulo,IDCaracteristica,IDOrden,[Fecha],Usuario) values" +
                        "(" + produccion.IDArticulo + "," + produccion.IDCaracteristica + "," + produccion.IDOrden + ",SYSDATETIME()," + usuario + ")";
                    car.Database.ExecuteSqlCommand(cadena);

                }
                catch (Exception err)
                {

                }


                car.Database.ExecuteSqlCommand("delete from ArticuloProduccion where IDArtProd=" + id);


                return Json(true);
            }
            catch (Exception err)
            {
                return Json(500, err.Message);
            }
        }

        public ActionResult PrioridadMaquina(int? page, int? PageSize, int? idmaquina = 0, int IDOrden=0)
        {
            if (idmaquina == 0)
            {
                return RedirectToAction("Prioridad");
            }
            int pageNumber = 0;
            int pageSize = 0;
            Reenumerar(idmaquina);

            VMaquinaProcesoContext dba = new VMaquinaProcesoContext();
            List<SelectListItem> nueva = new SelectList(dba.VMaquinaProceso, "IDArticulo", "Descripcion", idmaquina).ToList();
            nueva.Insert(0, (new SelectListItem { Text = "Programar Prioridad", Value = "0" }));
            ViewBag.Maquinas = nueva;
            ViewBag.Maquina = idmaquina;

            VMaquinaProceso maquinaproceso = new VMaquinaProcesoContext().VMaquinaProceso.Where(s => s.IDArticulo == idmaquina).FirstOrDefault();
            int idprocesodelamaquina = maquinaproceso.IDProceso;

            string cadena = "select top 100 o.*from ordenproduccion as o inner join encpedido as ep on ep.idpedido=o.idpedido inner join prioridades as p on o.idorden = p.idorden where (ep.status<>'Inactivo' or ep.status<>'Cancelado') and  p.idmaquina = "+idmaquina+ " and p.idproceso = "+ idprocesodelamaquina + " and (p.estado = 'Iniciada' or p.estado='Programado') ORDER BY CASE WHEN p.estado = 'Iniciada' then 1 when p.estado = 'Programado' then 2  end, p.prioridad";


            if (IDOrden == 0)
            {
                
            }
            else 
            {
                cadena = "select top 100  o.*from ordenproduccion as o  inner join encpedido as ep on ep.idpedido=o.idpedido inner join prioridades as p on o.idorden=p.idorden where (ep.status<>'Inactivo' or ep.status<>'Cancelado') and  p.idmaquina=" + idmaquina + " and p.idproceso=" + idprocesodelamaquina + " and o.idorden="+IDOrden+ " and (p.estado = 'Iniciada' or p.estado='Programado') ORDER BY CASE WHEN p.estado = 'Iniciada' then 1 when p.estado = 'Programado' then 2  end, p.prioridad";
                
            }

            ViewBag.IDorden = IDOrden;


            List<OrdenProduccion> ele = db.Database.SqlQuery<OrdenProduccion>(cadena).ToList<OrdenProduccion>();
            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = ele.Count;
            ViewBag.Conteo = count;
            // Populate DropDownList
            ViewBag.PageSize = new List<SelectListItem>()
                 {
                new SelectListItem { Text = "10", Value = "10" },
                new SelectListItem { Text = "25", Value = "25" },
                new SelectListItem { Text = "50", Value = "50" , Selected = true},
                new SelectListItem { Text = "100", Value = "100" },
                new SelectListItem { Text = "Todo", Value = count.ToString() }
                  };

            pageNumber = (page ?? 1);
            pageSize = (PageSize ?? 25);
            ViewBag.psize = pageSize;

            return View(ele.ToPagedList(pageNumber, pageSize));


        }

        public ActionResult Programar(int orden)
        {
            OrdenProduccion ordenpro = new OrdenProduccionContext().OrdenesProduccion.Find(orden);
            ModeloProcesoContext mpc = new ModeloProcesoContext();

            int primerproceso = mpc.ModeloProceso.Where(s => s.ModelosDeProduccion_IDModeloProduccion == ordenpro.IDModeloProduccion).OrderBy(s => s.orden).FirstOrDefault().Proceso_IDProceso;

            ClsDatoEntero idmaquina = db.Database.SqlQuery<ClsDatoEntero>("select idarticulo as Dato from [articuloproduccion] where  idtipoarticulo=3 and idproceso=" + primerproceso + " and IdOrden=" + orden + "").ToList().FirstOrDefault();
            mpc.Database.ExecuteSqlCommand("update OrdenProduccionDetalle set EstadoProceso='Programado' where IDorden=" + orden + " and IDProceso=" + primerproceso);

            VMaquinaProcesoContext dba = new VMaquinaProcesoContext();
            List<SelectListItem> nueva = new SelectList(dba.VMaquinaProceso, "IDArticulo", "Descripcion", idmaquina.Dato).ToList();
            nueva.Insert(0, (new SelectListItem { Text = "Selecciona la Maquina", Value = "0" }));
            ViewBag.Maquinas = nueva;
            ViewBag.Maquina = idmaquina.Dato;
            ViewBag.IDOrden = orden;
            return View();
        }

        // POST: Programar
        [HttpPost]
        public ActionResult Programar(FormCollection coleccion)
        {
            string orden = coleccion.Get("orden");
            string fecha = coleccion.Get("fecha");
            string maquina = coleccion.Get("Maquina");
            int idorden = Convert.ToInt32(orden);
            DateTime fechaReal = Convert.ToDateTime(fecha);
            int cambioMaquina = Convert.ToInt32(maquina);
            VMaquinaProceso maquinaproceso = new VMaquinaProcesoContext().VMaquinaProceso.Where(s => s.IDArticulo == cambioMaquina).FirstOrDefault();
            int idprocesodelamaquina = maquinaproceso.IDProceso;

            try
            {

                db.Database.ExecuteSqlCommand("update [dbo].[ArticuloProduccion] set [IDArticulo]=" + cambioMaquina + " where idorden=" + idorden + " and idtipoarticulo=3 and idproceso=5");
                db.Database.ExecuteSqlCommand("update ordenproduccion set prioridad =9999 where idorden=" + idorden);
                string cadena = "insert into prioridades (IDOrden,IDProceso,IDMaquina,Prioridad,Estado) values(" + idorden + ","+ idprocesodelamaquina + "," + cambioMaquina + ",9999,'Programado')";
                db.Database.ExecuteSqlCommand(cadena);

                addfechapro(idorden, fechaReal);
                return RedirectToAction("PrioridadMaquina", new { idmaquina= cambioMaquina });

            }
            catch
            {
                return View();
            }
        }




        public void Reenumerar(int? maquina)
        {
            try
            {
                VMaquinaProceso maquinaproceso = new VMaquinaProcesoContext().VMaquinaProceso.Where(s => s.IDArticulo == maquina).FirstOrDefault();
                int idprocesodelamaquina = maquinaproceso.IDProceso;
                
                string cadena = "select o.*from ordenproduccion as o inner join prioridades as p on o.idorden=p.idorden where p.idproceso="+idprocesodelamaquina+" and  p.idmaquina=" + maquina + " and p.estado='Programado' ORDER BY CASE WHEN p.estado = 'Iniciada' then 1 when p.estado = 'Programado' then 2 end , p.prioridad";

                List<OrdenProduccion> ele = db.Database.SqlQuery<OrdenProduccion>(cadena).ToList<OrdenProduccion>();

                int priori = 1;
               foreach (var ordenpro in ele)
                {
                    db.Database.ExecuteSqlCommand("update [dbo].[Prioridades] set [prioridad]=" + priori + " where estado='Programado' and  idorden=" + ordenpro.IDOrden);
                    priori++;
                }
                db.Database.ExecuteSqlCommand("update [dbo].[Prioridades] set [prioridad]=0 where estado='Terminado'");
                db.Database.ExecuteSqlCommand("update [dbo].[Prioridades] set [prioridad]=0 where estado='Iniciada'");
                db.Database.ExecuteSqlCommand("update [dbo].[Prioridades] set [prioridad]=0 where estado='Cancelado'");
                db.Database.ExecuteSqlCommand("update [dbo].[Prioridades] set [prioridad]=0 where estado='Finalizada'");
                db.Database.ExecuteSqlCommand("update [dbo].[Prioridades] set [prioridad]=0 where estado='Finalizado'");

            }
            catch (Exception err)
            {

            }

        }

        public string Completa(int valor) // acompleta a dos digitos el valor
        {
            string cc = valor.ToString();
            // separo los decimales si los hay
            if (cc.Length == 1)
            { return ("0" + cc); }
            else if (cc.Length == 2)
            {
                return cc;
            }
            return cc;
        }

        public string Completa3(int valor) // acompleta a dos digitos el valor
        {
            string cc = valor.ToString();
            // separo los decimales si los hay
            if (cc.Length == 1)
            { return ("00" + cc); }
            else if (cc.Length == 2)
            {
                return "0" + cc;
            }
            return cc;
        }



        public string Completa5(int valor) // acompleta a dos digitos el valor
        {
            string cc = valor.ToString();

            for (int i = 6; i >= cc.Length; i--)
            {
                cc = "0" + cc;
            }
            return cc;
        }

        public ActionResult EntreFechasProd()
        {
            EFecha elemento = new EFecha();

            return View(elemento);
        }

    
        [HttpPost]
        public ActionResult EntreFechasProd(EFecha modelo, FormCollection coleccion)
        {
            VOPContext dbr = new VOPContext();
            string FI = modelo.fechaini.Year.ToString() + "-" + modelo.fechaini.Month.ToString() + "-" + modelo.fechaini.Day.ToString();
            string FF = modelo.fechafin.Year.ToString() + "-" + modelo.fechafin.Month.ToString() + "-" + modelo.fechafin.Day.ToString();

            string cual = coleccion.Get("Enviar");

            string cadena = "";
            if (cual == "Generar reporte")
            {
                return View();
            }
            if (cual == "Generar excel")
            {

                cadena = "select * from VOP where FechaCreacion >= '" + FI + "' and FechaCreacion <='" + FF + "' ";
                var datos = dbr.Database.SqlQuery<VOP>(cadena).ToList();
                ViewBag.datos = datos;
                ExcelPackage Ep = new ExcelPackage();
                //Crear la hoja y poner el nombre de la pestaña del libro
                var Sheet = Ep.Workbook.Worksheets.Add("Orden de Producción");

                // en la fila1 formateo las celdas y coloco el título de la hoja
                // RichText permite agregar las propiedades de tipo de letra: color, negrita, etc.
                int row = 1;
                //Fijar la fuente para A1:Q1
                Sheet.Cells["A1:P1"].Style.Font.Size = 20;
                Sheet.Cells["A1:P1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:P3"].Style.Font.Bold = true;
                Sheet.Cells["A1:P1"].Style.Font.Color.SetColor(Color.DarkBlue);
                Sheet.Cells["A1:P1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                Sheet.Cells["A1"].RichText.Add("Listado de Ordenes de Producción");

                row = 2;
                Sheet.Cells["A1:P1"].Style.Font.Size = 12;
                Sheet.Cells["A1:P1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:P1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
                Sheet.Cells["A2:P2"].Style.Font.Bold = true;
                //Subtitulo según el filtrado del periodo de datos
                row = 2;
                Sheet.Cells[string.Format("A2", row)].Value = "Fecha inicial";
                Sheet.Cells[string.Format("B2", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("B2", row)].Value = FI;
                Sheet.Cells[string.Format("D2", row)].Value = "Fecha Final";
                Sheet.Cells[string.Format("E2", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("E2", row)].Value = FF;
                //En la fila3 se da el formato a el encabezado
                row = 3;
                Sheet.Cells.Style.Font.Name = "Calibri";
                Sheet.Cells.Style.Font.Size = 10;
                Sheet.Cells["A3:P3"].Style.Font.Bold = true;
                Sheet.Cells["A3:P3"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                Sheet.Cells["A3:P3"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                // Se establece el nombre que identifica a cada una de las columnas de datos.
                Sheet.Cells["A3"].RichText.Add("IDOrden");
                Sheet.Cells["B3"].RichText.Add("IDPedido");
                //Sheet.Cells["C3"].RichText.Add("IDCliente");
                Sheet.Cells["C3"].RichText.Add("No. Expediente");
                Sheet.Cells["D3"].RichText.Add("Cliente");
                Sheet.Cells["E3"].RichText.Add("Modelo");
                Sheet.Cells["F3"].RichText.Add("Articulo");
                Sheet.Cells["G3"].RichText.Add("Presentación");
                Sheet.Cells["H3"].RichText.Add("Cantidad");
                Sheet.Cells["I3"].RichText.Add("FechaCompromiso");
                Sheet.Cells["J3"].RichText.Add("FechaInicio");
                Sheet.Cells["K3"].RichText.Add("FechaProgramada");
                Sheet.Cells["L3"].RichText.Add("Liberar");
                Sheet.Cells["M3"].RichText.Add("EstadoOrden");
                Sheet.Cells["N3"].RichText.Add("FechaCreación");
                Sheet.Cells["O3"].RichText.Add("Usuario");
                Sheet.Cells["P3"].RichText.Add("Oficina");
                Sheet.Cells["Q3"].RichText.Add("No Tintas");
                Sheet.Cells["R3"].RichText.Add("Tipo");



                //Aplicar borde doble al rango de celdas A3:Q3
                Sheet.Cells["A3:S3"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;

                // en la fila4, el foreach llenara cada fila de la hoja de excel, con cada fila que trae de la bd.
                // Se establecen los formatos para las celdas: Fecha, Moneda
                row = 4;
                Sheet.Cells.Style.Font.Bold = false;
                foreach (VOP item in ViewBag.datos)
                {
                    Sheet.Cells[string.Format("A{0}", row)].Value = item.IDOrden;
                    Sheet.Cells[string.Format("B{0}", row)].Value = item.IDPedido;
                    //Sheet.Cells[string.Format("C{0}", row)].Value = item.IDCliente;
                    Sheet.Cells[string.Format("C{0}", row)].Value = item.noExpediente;
                    Sheet.Cells[string.Format("D{0}", row)].Value = item.Cliente;
                    Sheet.Cells[string.Format("E{0}", row)].Value = item.Modelo;
                    Sheet.Cells[string.Format("F{0}", row)].Value = item.Descripcion;
                    Sheet.Cells[string.Format("G{0}", row)].Value = item.Presentacion;
                    Sheet.Cells[string.Format("H{0}", row)].Value = item.Cantidad;
                    Sheet.Cells[string.Format("I{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    Sheet.Cells[string.Format("I{0}", row)].Value = item.FechaCompromiso;
                    Sheet.Cells[string.Format("J{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    Sheet.Cells[string.Format("J{0}", row)].Value = item.FechaInicio;
                    Sheet.Cells[string.Format("K{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    Sheet.Cells[string.Format("K{0}", row)].Value = item.FechaProgramada;
                    Sheet.Cells[string.Format("L{0}", row)].Value = item.Liberar;
                    Sheet.Cells[string.Format("M{0}", row)].Value = item.EstadoOrden;
                    Sheet.Cells[string.Format("N{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    Sheet.Cells[string.Format("N{0}", row)].Value = item.FechaCreacion;
                    Sheet.Cells[string.Format("O{0}", row)].Value = item.UserName;
                    Sheet.Cells[string.Format("P{0}", row)].Value = item.Oficina;
                    string cadT = "SELECT COUNT(*) as Dato  from dbo.ArticuloProduccion WHERE IDTipoArticulo = 7 and idOrden = " + item.IDOrden + "";
                    ClsDatoEntero cuentaT = db.Database.SqlQuery<ClsDatoEntero>(cadT).ToList()[0];
                    Sheet.Cells[string.Format("Q{0}", row)].Value = cuentaT.Dato;
                    if (item.IDModeloProduccion == 4)
                    {
                        Sheet.Cells[string.Format("R{0}", row)].Value = "Flexografía";
                    }
                    if (item.IDModeloProduccion == 8)
                    {
                        Sheet.Cells[string.Format("R{0}", row)].Value = "Termoencogible";
                    }



                    row++;
                }
                //Se genera el archivo y se descarga

                Sheet.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment: filename=" + "ReporteExcelOP.xlsx");
                Response.BinaryWrite(Ep.GetAsByteArray());
                Response.End();
                return Redirect("/blah");
            }
            return Redirect("Prioridad");
        }
        public ActionResult EditarCalidad(int IDCertificado, int page, string MensajeError = "")
        {
            ViewBag.Mensaje = "";
            if (MensajeError != "")
            {
                ViewBag.Mensaje = MensajeError;
            }
            else
            {
                ViewBag.Mensaje = "";
            }

            ViewBag.page = page;

            string cadena1 = " select * from dbo.CertificadoCalidad where [IDCertificado]= " + IDCertificado + "";
            CertificadoCalidad liberacion = new WorkinProcessContext().Database.SqlQuery<CertificadoCalidad>(cadena1).ToList().FirstOrDefault();
            OrdenProduccion orden = new WorkinProcessContext().Database.SqlQuery<OrdenProduccion>("Select*from OrdenProduccion where idorden=" + liberacion.IDOrden).ToList().FirstOrDefault();
            Articulo articulo = new WorkinProcessContext().Database.SqlQuery<Articulo>("Select*from Articulo where IDArticulo=" + orden.IDArticulo).ToList().FirstOrDefault();

            ViewBag.liberacion = liberacion;
            ViewBag.IDLibera = liberacion.IDLibera;
            ViewBag.IDOrden = liberacion.IDOrden;
            ViewBag.IDCliente = new SelectList(new ClientesContext().Clientes, "IDCliente", "Nombre", liberacion.IDCliente);

            ViewBag.Cliente = liberacion.IDCliente;

            ViewBag.CantidadLiberada = liberacion.Cantidad;
            ViewBag.PresentacionResultado = liberacion.PresentacionResultado;
            ViewBag.PresentacionMetodo = liberacion.MetodoResultadoPresentacion;
            ViewBag.PresentacionTintas = liberacion.PresentacionTintas;
            ViewBag.IDArticulo = new SelectList(new ArticuloContext().Articulo.Where(s => s.IDArticulo == orden.IDArticulo), "IDArticulo", "Descripcion");

            CalidadContext db = new CalidadContext();
            ViewBag.IDMuestreo = new SelectList(db.Muestreos, "IDMuestreo", "Descripcion", articulo.IDMuestreo);
            ViewBag.IDInspeccion = new SelectList(db.Inspecciones, "IDInspeccion", "Descripcion", articulo.IDInspeccion);
            string Letra = "";
            var CalidadLetras = new CalidadLetraContext().CalidadLetras;
            if (articulo.IDInspeccion == 1)
            {
                foreach (CalidadLetra CalidadL in CalidadLetras)
                {
                    if (liberacion.Cantidad >= CalidadL.LI_Lote_mill && liberacion.Cantidad <= CalidadL.LS_Lote_mill)
                    {
                        Letra = CalidadL.NGI1;
                    }

                }


            }
            else if (articulo.IDInspeccion == 2)
            {
                foreach (CalidadLetra CalidadL in CalidadLetras)
                {
                    if (liberacion.Cantidad >= CalidadL.LI_Lote_mill && liberacion.Cantidad <= CalidadL.LS_Lote_mill)
                    {
                        Letra = CalidadL.NGI2;
                    }

                }
            }
            else if (articulo.IDInspeccion == 3)
            {
                foreach (CalidadLetra CalidadL in CalidadLetras)
                {
                    if (liberacion.Cantidad >= CalidadL.LI_Lote_mill && liberacion.Cantidad <= CalidadL.LS_Lote_mill)
                    {
                        Letra = CalidadL.NGI3;
                    }

                }
            }
            else
            {
                Letra = "N/A";
            }
            if (Letra == "N/A")
            {
                throw new Exception("Artículo Inspección N/A");
            }
            try
            {
                CalidadLimiteAceptacion CodigoLetra = new WorkinProcessContext().Database.SqlQuery<CalidadLimiteAceptacion>("Select*from CalidadLimiteAceptacion where CodigoLetra='" + Letra + "'").ToList().FirstOrDefault();

                ViewBag.Revisar = CodigoLetra.TamanoMuestra;
                ViewBag.Letra = Letra;
            }
            catch (Exception err)
            {
                ViewBag.Revisar = 0;
                ViewBag.Letra = "";
            }


            return View();
        }

        [HttpPost]
        public ActionResult EditarCalidad(CertificadoCalidad certificado, FormCollection coleccion, int? page)
        {
            string cadena1 = " select * from dbo.CertificadoCalidad where [IDLibera]= " + certificado.IDLibera + "";
            CertificadoCalidad liberacion = new WorkinProcessContext().Database.SqlQuery<CertificadoCalidad>(cadena1).ToList().FirstOrDefault();
           
            
            Articulo articulo = new WorkinProcessContext().Database.SqlQuery<Articulo>("Select*from Articulo where IDArticulo=" + liberacion.IDArticulo).ToList().FirstOrDefault();
            Clientes clientes = new ClientesContext().Clientes.Find(certificado.IDCliente);
            clientes.IDCliente = clientes.IDCliente;
            int contador = 0;
            string[] arraydatos;
            arraydatos = liberacion.Presentacion.Split(',');
            int cuantos = arraydatos.Length;
            string metodo = "";
            string resultado = "";
            string parametro = "";
            string valor = string.Empty;
            string unidad = string.Empty;
            string PresentacionResultado = "";
            string PresentacionMetodoResultado = "";
            string PresentacionFinalResultado = "";
            string PresentacionFinalMetodoResultado = "";
            string PresentacionOriginal = "";
            string PresentacionUnidad = "";


            for (int i = 0; i < arraydatos.Length; i++)
            {

                try
                {
                    metodo = coleccion.Get("IDMetodo" + i);
                    resultado = coleccion.Get("Resultado" + i);
                    parametro = coleccion.Get("Parametro" + i);
                    valor = coleccion.Get("Valor" + i);
                    
                    if (parametro == string.Empty)
                    {
                        throw new Exception("No hay parametros");
                    }
                    string cuenta = arraydatos[i];
                    string concatenacion = "";
                    string concatenacionMetodo = "";
                    string concatenacionValor = "";
                    string concatenacionUnidad = "";
                    string[] arraydatoscortados;
                    string acc = "";
                    string valor1 = "";

                    try
                    {
                        arraydatoscortados = cuenta.Split(':');
                        acc = arraydatoscortados[0] /*+ ": " + arraydatoscortados[1] + "   "*/;
                        valor1 = arraydatoscortados[1];

                    }
                    catch (Exception err)
                    {

                    }

                    if (acc.ToUpper() != "OBSERVACION" && acc.ToUpper() != "PLECA")
                    {
                        if (valor1 != "0")
                        {
                            concatenacionValor = parametro + ":" + valor + ",";
                            concatenacionMetodo = parametro + ":" + metodo + ",";
                            concatenacion = parametro + ":" + resultado + ",";



                            PresentacionMetodoResultado += concatenacionMetodo;
                            PresentacionResultado += concatenacion;

                        }
                    }

                }
                catch (Exception err)
                {
                    string mensaje = "ya no hay mas parametros";
                }

            }

            PresentacionFinalResultado = PresentacionResultado.TrimEnd(',');
            PresentacionFinalMetodoResultado = PresentacionMetodoResultado.TrimEnd(',');
        
            
            int CantidadEtiquetasMal = int.Parse(coleccion.Get("CantidadMal"));
            try
            {
               
               

                if ((CantidadEtiquetasMal >= CantidadR(articulo.IDArticulo, CantidadEtiquetasMal)))
                {

                    ViewBag.Mensaje = "No se genera certificado de calidad por rechazo";


                }
                else
                {
                   
                     try
                    {
                     
                        string ingresar = "update [dbo].[CertificadoCalidad]  set [IDCliente]="+certificado.IDCliente+",MetodoResultadoPresentacion='"+ PresentacionFinalMetodoResultado + "',PresentacionResultado='"+ PresentacionFinalResultado + "' where IDCertificado="+ certificado.IDCertificado;

                        db.Database.ExecuteSqlCommand(ingresar);

                    }
                    catch (Exception err)
                    {

                    }
                }
            }
            catch (Exception err)
            {

            }
            try
            {

                string actualizar = "update certificadoCalidad set  idcliente='" + certificado.IDCliente + "',Cantidad=" + certificado.Cantidad + " Where IDCertificado=" + liberacion.IDCertificado;
                db.Database.ExecuteSqlCommand(actualizar);

            }
            catch (Exception err)
            {

            }
            if (ViewBag.Mensaje != "" && ViewBag.Mensaje != null)
            {
                return RedirectToAction("Calidad", new { IDLibera = certificado.IDLibera, MensajeError = ViewBag.Mensaje });
            }

         

            return RedirectToAction("OrdenesLiberadas", new { page = page });


        }
        
        public ActionResult AsignarMaterial(int IDArtPro, int idorden, string currentFilter, string searchString)
        {
            ArticulosProduccionContext db = new ArticulosProduccionContext();
            ViewBag.idorden = idorden;
            ViewBag.IDArtProduc = IDArtPro;
            ArticuloProduccion articulopro = db.ArticulosProducciones.Find(IDArtPro);
            Articulo artdes = new ArticuloContext().Articulo.Find(articulopro.IDArticulo);

            ViewBag.DescripcionArt = artdes.Descripcion;
            ViewBag.can = articulopro.Cantidad;

            if (searchString == "")
            {
                searchString = currentFilter;
            }
            // Not sure here
            if (searchString == null)
            {
                searchString = currentFilter;
            }

            // Pass filtering string to view in order to maintain filtering when paging
            ViewBag.SearchString = searchString;

            string cadena = "select vi.*from VInventarioAlmacen as vi inner join Articulo as a on a.IDArticulo=vi.IDArticulo inner join Caracteristica as c on c.id=vi.IDCAracteristica where c.obsoleto='0' and a.obsoleto='0' and vi.idarticulo="+ articulopro.IDArticulo + " and (vi.IDAlmacen=6 OR vi.IDAlmacen=11) ";

            List<SIAAPI.Models.Comercial.VInventarioAlmacen> elementos = new SIAAPI.Models.Comercial.InventarioAlmacenContext().Database.SqlQuery<SIAAPI.Models.Comercial.VInventarioAlmacen>(cadena).OrderBy(s => s.Cref).ToList();

            if (elementos.Count == 0)
            {
                try
                {
                    db.Database.ExecuteSqlCommand("insert into inventarioalmacen (idalmacen, idarticulo, idcaracteristica, existencia, porllegar, apartado, disponibilidad) values (6," + articulopro.IDArticulo + "," + articulopro.IDCaracteristica + ",0,0,0,0)");
                    db.Database.ExecuteSqlCommand("insert into inventarioalmacen (idalmacen, idarticulo, idcaracteristica, existencia, porllegar, apartado, disponibilidad) values (11," + articulopro.IDArticulo + "," + articulopro.IDCaracteristica + ",0,0,0,0)");
                }
                catch (Exception err2)
                {

                }
            }

            ViewBag.Elementos = elementos;
            VInventarioAlmacenContext dd = new VInventarioAlmacenContext();
            SIAAPI.Models.Comercial.ClsDatoEntero familia = dd.Database.SqlQuery<SIAAPI.Models.Comercial.ClsDatoEntero>("select idfamilia as Dato from  [Articulo] where [idarticulo]=" + artdes.IDArticulo).ToList().FirstOrDefault();

            string cadena1 = "select vi.*from VInventarioAlmacen as vi inner join Articulo as a on a.IDArticulo=vi.IDArticulo inner join Caracteristica as c on c.id=vi.IDCAracteristica where c.obsoleto='0' and a.obsoleto='0' and a.idfamilia=" + familia.Dato + " and vi.idarticulo <>" + artdes.IDArticulo;

            List<SIAAPI.Models.Comercial.VInventarioAlmacen> ele1 = new SIAAPI.Models.Comercial.InventarioAlmacenContext().Database.SqlQuery<SIAAPI.Models.Comercial.VInventarioAlmacen>(cadena1).OrderBy(s => s.Cref).ToList();
            ViewBag.Elementos2 = ele1;
            if (!string.IsNullOrEmpty(searchString) )
            {
                string cadenas = "select vi.*from VInventarioAlmacen as vi inner join Articulo as a on a.IDArticulo=vi.IDArticulo inner join Caracteristica as c on c.id=vi.IDCAracteristica where c.obsoleto='0' and a.obsoleto='0' and a.cref like '%" + searchString + "%' and (vi.IDAlmacen=6 or vi.IDalmacen=11) ";

                List<SIAAPI.Models.Comercial.VInventarioAlmacen> elementosI = new SIAAPI.Models.Comercial.InventarioAlmacenContext().Database.SqlQuery<SIAAPI.Models.Comercial.VInventarioAlmacen>(cadenas).OrderBy(s => s.Cref).ToList();

                ViewBag.Elementos = elementosI;
                ViewBag.Elementos2 = 0;

            }


            return View();
        }
        

        public ActionResult OrdenesALiberar(string sortOrder, string currentFilter, int? page, int? PageSize, string searchString = "")
        {
            //EjecutarOrdenesaLiberar();
            int pageNumber = 0;
            int pageSize = 0;
            int count = 0;


            string cadena = "";


            ViewBag.idorden = null;
            ViewBag.idproceso = null;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.vercreate = null;

            if (searchString != "")
            {
                
                cadena = "select op.idOrden, op.IDArticulo, op.IDCaracteristica, op.Presentacion, op.IDPedido, op.IDCliente, OL.IDOrdenesALiberar, OL.Cantidad from Ordenproduccion as op  inner join OrdenesALiberar as OL on op.idorden=OL.IDOrden where  op.idorden='" + searchString + "' and op.EstadoOrden<>'Cancelada'";

            }
            else
            {

                cadena = "select op.idOrden, op.IDArticulo, op.IDCaracteristica, op.IDPedido, op.Presentacion, op.IDCliente, OL.IDOrdenesALiberar, OL.Cantidad from Ordenproduccion as op  inner join OrdenesALiberar as OL on op.idorden=OL.IDOrden where  op.EstadoOrden<>'Cancelada' ";

            }

            List<VOrdenesALiberar> ele = db.Database.SqlQuery<VOrdenesALiberar>(cadena).ToList<VOrdenesALiberar>();
            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            count = ele.Count;

            // Populate DropDownList
            ViewBag.PageSize = new List<SelectListItem>()
            {
                new SelectListItem { Text = "10", Value = "10", Selected = true },
                new SelectListItem { Text = "25", Value = "25" },
                new SelectListItem { Text = "50", Value = "50" },
                new SelectListItem { Text = "100", Value = "100" },
                new SelectListItem { Text = "Todo", Value = count.ToString() }
             };

            pageNumber = (page ?? 1);
            pageSize = (PageSize ?? 10);
            ViewBag.psize = pageSize;

            return View(ele.ToPagedList(pageNumber, pageSize));


        }
        public decimal CantidadR(int id, decimal cantidad)
        {   decimal Rev = 0;
            Articulo articulo = new ArticuloContext().Articulo.Find(id);
            try
            {
             
                string Letra = "";
                var CalidadLetras = new CalidadLetraContext().Database.SqlQuery<CalidadLetra>("select*from CalidadLetra ").ToList(); 
                if (articulo.IDInspeccion == 1)
                {
                    foreach (CalidadLetra CalidadL in CalidadLetras)
                    {
                        if (cantidad >= CalidadL.LI_Lote_mill && cantidad <= CalidadL.LS_Lote_mill)
                        {
                            Letra = CalidadL.NGI1;
                        }

                    }


                }
                else if (articulo.IDInspeccion == 2)
                {
                    foreach (CalidadLetra CalidadL in CalidadLetras)
                    {
                        if (cantidad >= CalidadL.LI_Lote_mill && cantidad <= CalidadL.LS_Lote_mill)
                        {
                            Letra = CalidadL.NGI2;
                        }

                    }
                }
                else if (articulo.IDInspeccion == 3)
                {
                    foreach (CalidadLetra CalidadL in CalidadLetras)
                    {
                        if (cantidad >= CalidadL.LI_Lote_mill && cantidad <= CalidadL.LS_Lote_mill)
                        {
                            Letra = CalidadL.NGI3;
                        }

                    }
                }
                else
                {
                    Letra = "N/A";
                }
                if (Letra == "N/A")
                {
                    throw new Exception("Artículo Inspección N/A");
                }
                try
                {
                    CalidadLimiteAceptacion CodigoLetra = new WorkinProcessContext().Database.SqlQuery<CalidadLimiteAceptacion>("Select*from CalidadLimiteAceptacion where CodigoLetra='" + Letra + "'").ToList().FirstOrDefault();

                    Rev = CodigoLetra.TamanoMuestra;
                    ViewBag.Letra = Letra;
                }
                catch (Exception err)
                {
                    ViewBag.Revisar = 0;
                    ViewBag.Letra = "";
                }

             


            

            }
            catch (Exception err)
            {
                
            }
    return Rev;
       

        }

        public bool seterminaelultimoproceo(int idorden, int idprocesoqueseestafinalizado)
        {
            OrdenProduccion op = new OrdenProduccionContext().OrdenesProduccion.Find(idorden);
            ModelosDeProduccion modpro = new ModelosDeProduccionContext().ModelosDeProducciones.Find(op.IDModeloProduccion);

            ModeloProceso mpfinal = new ModeloProcesoContext().ModeloProceso.Where(s => s.ModelosDeProduccion_IDModeloProduccion == modpro.IDModeloProduccion).OrderByDescending(s => s.orden).FirstOrDefault();
            if (mpfinal.Proceso_IDProceso == idprocesoqueseestafinalizado)
            {
                return true;
            }
            else
            {
                return false;
            }



        }

        public void EjecutarOrdenesaLiberar()
        {

            //List<OrdenProduccion> orden = new OrdenProduccionContext().Database.SqlQuery<OrdenProduccion>("select o.* from OrdenProduccion as o inner join OrdenesALiberar as ol on o.IDOrden =ol.IDOrden where o.EstadoOrden='Cancelada' or o.EstadoOrden='Finalizada'").ToList();
            List<Prioridades> orden = new OrdenProduccionContext().Database.SqlQuery<Prioridades>("select p.* from OrdenProduccion as o inner join OrdenesALiberar as ol on o.IDOrden =ol.IDOrden inner join prioridades as p on p.idorden=o.idorden where  p.Estado='Terminado' or p.Estado='Finalizada'").ToList();

            
            foreach (Prioridades produccion in orden)
            {
                bool eselutimo = seterminaelultimoproceo(produccion.IDOrden, produccion.IDProceso);
                if (eselutimo)
                {
                    LiberaOrden libera = new LiberaOrdenContext().Database.SqlQuery<LiberaOrden>("select*from LiberaOrden where idorden="+ produccion.IDOrden+" and tipoLiberacion='Final'").ToList().FirstOrDefault();

                    if (libera!=null)
                    {
                        db.Database.ExecuteSqlCommand("delete from OrdenesALiberar where idorden=" + produccion.IDOrden);
                    }
                    
                }
               
            }
        }
        
        public ActionResult Solicitar(int id, decimal Cantidad, int IDAlmacen, int idarticulopro)
        {
            string cadmen = "";
            try
            {
                int num = 1;
                try
                {
                    string cadenanum = "select max(IDRequisicion)+1 as Dato from EncRequisiciones";
                    num = db.Database.SqlQuery<ClsDatoEntero>(cadenanum).ToList().FirstOrDefault().Dato;

                }
                catch (Exception err)
                {
                    string mensaje = err.Message;
                    num = 1;
                }

                cadmen = "numero de Requisicion " + num;

                MaterialAsignado solicitud = db.Database.SqlQuery<MaterialAsignado>("Select * from MaterialAsignado where [IDMaterialAsignado]=" + id).ToList().FirstOrDefault();
                ViewBag.orden = solicitud.orden;
                Proveedor proveedordesconocido = new ProveedorContext().Proveedores.Find(SIAAPI.Properties.Settings.Default.IDProveedorDesconocido);

                EncRequisiciones requisicion = new EncRequisiciones();
                requisicion.IDRequisicion = num;
                requisicion.Fecha = DateTime.Now;
                requisicion.FechaRequiere = DateTime.Now;
                requisicion.IDProveedor = proveedordesconocido.IDProveedor;
                requisicion.IDFormapago = proveedordesconocido.IDFormaPago;
                requisicion.Observacion = "Para el Documento Orden" + solicitud.orden;
                requisicion.Subtotal = 0;
                requisicion.IVA = 0;
                requisicion.Total = 0;
                requisicion.IDMetodoPago = proveedordesconocido.IDMetodoPago;
                requisicion.IDCondicionesPago = proveedordesconocido.IDCondicionesPago;
                requisicion.IDAlmacen = IDAlmacen;
                requisicion.Status = "Activo";
                List<User> userid;
                userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
                requisicion.UserID = userid.Select(s => s.UserID).FirstOrDefault();
                requisicion.TipoCambio = 1;
                requisicion.IDUsoCFDI = 1;

                try
                {
                    decimal costo = db.Database.SqlQuery<ClsDatoDecimal>("select dbo.[GetCosto](0," + solicitud.idmapri + "," + Cantidad + ") as Dato").ToList().FirstOrDefault().Dato;

                    decimal subtotal = Math.Round(costo * Cantidad);
                    decimal iva = Math.Round(subtotal * Decimal.Parse(SIAAPI.Properties.Settings.Default.ValorIVA), 2);
                    decimal total = subtotal + iva;
                    Articulo articulo = new ArticuloContext().Articulo.Find(solicitud.idmapri);
                    requisicion.IDMoneda = articulo.IDMoneda;
                    requisicion.Subtotal = subtotal;
                    requisicion.IVA = iva;
                    requisicion.Total = total;
                    RequisicionesContext bdr = new RequisicionesContext();
                    bdr.EncRequisicioness.Add(requisicion);
                    try
                    {
                        bdr.SaveChanges();
                    }
                    catch (Exception eroor)
                    {
                        string mensajedeerror = eroor.Message;
                    }
                    DetRequisiciones detalle = new DetRequisiciones();
                    ClsDatoString presentacionCara = db.Database.SqlQuery<ClsDatoString>("select presentacion as Dato from [Caracteristica] where id=" + solicitud.idcaracteristica + "").ToList()[0];
                    decimal m2 = Cantidad * (solicitud.ancho / 1000);

                    StringBuilder cadena = new StringBuilder();

                    cadena.Append("INSERT INTO DetRequisiciones ");
                    cadena.Append("(IDRequisicion,IDArticulo,Cantidad,Descuento,costo,Importe,CantidadPedida,ImporteIva,IVA,ImporteTotal,Nota,Ordenado,Caracteristica_ID,IDAlmacen,Suministro,status,Presentacion,jsonPresentacion) values (");
                    cadena.Append(num + ",");
                    cadena.Append(solicitud.idmapri + ",");
                    cadena.Append(m2 + ",");
                    cadena.Append("0,");
                    cadena.Append(costo + ",");
                    cadena.Append(subtotal + ","); //importe
                    cadena.Append("0,"); //cantidadpedida
                    cadena.Append(iva + ","); //iva
                    cadena.Append("'1',");
                    cadena.Append(total + ",");

                    cadena.Append("'',");
                    cadena.Append("'0',"); //ordenado                  
                    cadena.Append(solicitud.idcaracteristica + ",");
                    cadena.Append(IDAlmacen + ",");
                    cadena.Append("0,"); //suministro
                    cadena.Append("'Activo',"); //status
                    cadena.Append("'" + presentacionCara.Dato + "',");
                    cadena.Append("'{" + presentacionCara.Dato + "}')");

                    try
                    {
                        db.Database.ExecuteSqlCommand(cadena.ToString());
                    }
                    catch (Exception err)
                    {
                        string mensaje = err.Message;
                    }


                    ViewBag.Mensaje = "ok";

                }
                catch (Exception err)
                {
                    string mensaje = err.Message;
                    ViewBag.Mensaje = "mensaje";
                }





                Session["Mensaje"] = ViewBag.Mensaje;
                return RedirectToAction("AsignarMaterial", new { IDArtPro = idarticulopro, idorden = ViewBag.orden });
            }
            catch (Exception err)
            {
                string mensaje = err.Message;
                ViewBag.Mensaje = mensaje;
                Session["Mensaje"] = ViewBag.Mensaje;
                return RedirectToAction("AsignarMaterial", new { IDArtPro = idarticulopro, idorden = ViewBag.orden });


            }
        }

        [HttpPost]
        public JsonResult DeleteMaterialAsignado(int id)
        {
            WorkinProcessContext db = new WorkinProcessContext();
            try
            {
                MaterialAsignado solicitud = db.Database.SqlQuery<MaterialAsignado>("Select * from MaterialAsignado where [IDMaterialAsignado]=" + id).ToList().FirstOrDefault();
                decimal m2 = decimal.Parse(solicitud.cantidad.ToString()) * (decimal.Parse(solicitud.ancho.ToString()) / 1000);

                db.Database.ExecuteSqlCommand("update [dbo].[InventarioAlmacen] set disponibilidad= disponibilidad +" + m2 + ", [apartado]=( apartado-" + m2 + ") where idalmacen=" + solicitud.idalmacen + " and idarticulo=" + solicitud.idmapri + "and idcaracteristica=" + solicitud.idcaracteristica);
                db.Database.ExecuteSqlCommand("update [dbo].[InventarioAlmacen] set [apartado]=0 where apartado<0 and idalmacen=" + solicitud.idalmacen + " and idarticulo=" + solicitud.idmapri + "and idcaracteristica=" + solicitud.idcaracteristica);
                db.Database.ExecuteSqlCommand("update [dbo].[InventarioAlmacen] set disponibilidad=(existencia-apartado) where  idalmacen=" + solicitud.idalmacen + " and idarticulo=" + solicitud.idmapri + "and idcaracteristica=" + solicitud.idcaracteristica);

                Caracteristica caracteristica = new Caracteristica();
                caracteristica = db.Database.SqlQuery<Caracteristica>("Select * from Caracteristica where ID=" + solicitud.idcaracteristica).ToList().FirstOrDefault();
                InventarioAlmacen inventario = new InventarioAlmacenContext().InventarioAlmacenes.ToList().Where(a => a.IDAlmacen == solicitud.idalmacen && a.IDCaracteristica == solicitud.idcaracteristica).FirstOrDefault();


                try
                {
                    string cadenam = "INSERT INTO [dbo].[MovimientoArticulo] ([Fecha],[Id],[IDPresentacion],[Articulo_IDArticulo],[Accion],[cantidad],[Documento],[NoDocumento],[Lote],[IDAlmacen],[TipoOperacion],[acumulado],[observacion],[Hora]) VALUES ";
                    cadenam += " (getdate(), " + caracteristica.ID + "," + caracteristica.IDPresentacion + "," + caracteristica.Articulo_IDArticulo + ",'Eliminación de Asignación de Material en Producción'," + m2 + ",'Orden Producción '," + solicitud.orden + ",''," + solicitud.idalmacen + ",'N/A'," + inventario.Existencia + ",'',CONVERT (time, SYSDATETIMEOFFSET()))";
                    db.Database.ExecuteSqlCommand(cadenam);
                }
                catch (Exception err)
                {
                    string mensajee = err.Message;
                }

                db.Database.ExecuteSqlCommand("delete from  MaterialAsignado where idmaterialasignado=" + id);

                return Json(true);
            }
            catch (Exception err)
            {
                return Json(500, err.Message);
            }
        }



        public JsonResult getSuaje(int IDSuaje)
        {
            Caracteristica cara = new ArticuloContext().Database.SqlQuery<Caracteristica>("Select * from Caracteristica where ID=" + IDSuaje).ToList().FirstOrDefault();
            SuajeCaracteristicas suajec = new SuajeCaracteristicas();
            FormulaSiaapi.Formulas formula = new FormulaSiaapi.Formulas();
            suajec.Eje = 0;

            try
            {
                suajec.Eje = decimal.Parse(formula.getvalor("Medida al Eje", cara.Presentacion).ToString());
            }
            catch (Exception err)
            {
                string mensajederror = err.Message;
                suajec.Eje = 0;
            }
            suajec.Avance = 0;
            try
            {
                suajec.Avance = decimal.Parse(formula.getvalor("Medida al Avance", cara.Presentacion).ToString());
            }
            catch (Exception err)
            {
                string mensajederror = err.Message;
                suajec.Avance = 0;
            }
            suajec.Cavidades = 2;

            try
            {
                suajec.Cavidades = int.Parse(formula.getvalor("CAV EJE", cara.Presentacion).ToString());
                if (suajec.Cavidades == 0)
                {
                    suajec.Cavidades = int.Parse(formula.getvalor("CAV EJE ", cara.Presentacion).ToString());
                    if (suajec.Cavidades == 0)
                    {
                        suajec.Cavidades = int.Parse(formula.getvalor("CAVIDADES EJE", cara.Presentacion).ToString());
                        if (suajec.Cavidades == 0)
                        {
                            suajec.Cavidades = int.Parse(formula.getvalor("CAVIDADES AL EJE", cara.Presentacion).ToString());
                            if (suajec.Cavidades == 0)
                            {
                                suajec.Cavidades = int.Parse(formula.getvalor("CAV AL EJE", cara.Presentacion).ToString());
                                if (suajec.Cavidades == 0)
                                {
                                    suajec.Cavidades = int.Parse(formula.getvalor("CAV AL EJE ", cara.Presentacion).ToString());
                                    if (suajec.Cavidades == 0)
                                    {
                                        suajec.Cavidades = int.Parse(formula.getvalor("CAVIDADES AL EJE ", cara.Presentacion).ToString());
                                        if (suajec.Cavidades == 0)
                                        {
                                            suajec.Cavidades = int.Parse(formula.getvalor("REP EJE", cara.Presentacion).ToString());
                                            if (suajec.Cavidades == 0)
                                            {
                                                suajec.Cavidades = int.Parse(formula.getvalor("REPETICIONES EJE", cara.Presentacion).ToString());
                                                if (suajec.Cavidades == 0)
                                                {
                                                    suajec.Cavidades = int.Parse(formula.getvalor("REPETICIONES AL EJE", cara.Presentacion).ToString());

                                                }
                                            }
                                        }
                                    }
                                }

                            }
                        }

                    }
                }
            }
            catch (Exception err)
            {
                string mensajederror = err.Message;
                suajec.Cavidades = 2;
            }


            suajec.Gapeje = 3;
            try
            {
                suajec.Gapeje = decimal.Parse(formula.getvalor("Separación entre etiquetas al eje", cara.Presentacion).ToString());
                if (suajec.Gapeje == 0)
                {
                    suajec.Gapeje = decimal.Parse(formula.getvalor("Gap Eje", cara.Presentacion).ToString());
                }

                }
            catch (Exception err)
            {
                string mensajederror = err.Message;
                suajec.Gapeje = 2;
            }



            suajec.Gapavance = 3;
            try
            {
                suajec.Gapavance = decimal.Parse(formula.getvalor("Separación entre etiquetas al avance", cara.Presentacion).ToString());
            }
            catch (Exception err)
            {
                string mensajederror = err.Message;
                suajec.Gapavance = 2;
            }

            try
            {
                suajec.AnchoCinta = int.Parse(formula.getvalor("Ancho de material para etiqueta blanca", cara.Presentacion).ToString());
            }
            catch (Exception err)
            {
                string mensajederror = err.Message;
                suajec.AnchoCinta = 0;
            }

            try
            {
                suajec.AnchoCintaimpresa = int.Parse(formula.getvalor("Ancho de material para etiqueta blanca", cara.Presentacion).ToString());
            }
            catch (Exception err)
            {
                string mensajederror = err.Message;
                suajec.AnchoCintaimpresa = 0;
            }

            suajec.TH = 0;
            try
            {
                suajec.TH = int.Parse(formula.getvalor("DIENTES_TH", cara.Presentacion).ToString());
                if (suajec.TH == 0)
                {
                    suajec.TH = int.Parse(formula.getvalor("TH", cara.Presentacion).ToString());
                    if (suajec.TH == 0)
                    {
                        suajec.TH = int.Parse(formula.getvalor("DIENTES", cara.Presentacion).ToString());
                    }
                }

            }
            catch (Exception err)
            {
                string mensajederror = err.Message;
                suajec.TH = 0;
            }


            return Json(suajec, JsonRequestBehavior.AllowGet);
        }


        public class Repository
        {          

            public IEnumerable<SelectListItem> GetSuajes()
            {
                using (var context = new ArticuloContext())
                {
                    string cadena = "select C.ID as IDCaracteristica, A.Cref,A.Descripcion from Articulo as A inner join Caracteristica as C on A.IDArticulo=C.Articulo_IDArticulo where A.IDTipoArticulo=2 and A.obsoleto='0' and C.obsoleto='0' and (A.IDFamilia=76 or A.IDFamilia=75 or A.IDFamilia=77 or A.IDFamilia=71 or A.IDFamilia =72 or A.IDFamilia=11 or A.IDFamilia=69 or A.IDFamilia=80) order by A.descripcion";
                    List<suajeC> listadesuaje = context.Database.SqlQuery<suajeC>(cadena).ToList();

                    List<SelectListItem> listadesuajes = new List<SelectListItem>();
                    foreach (suajeC item in listadesuaje)
                    {
                        SelectListItem art = new SelectListItem { Text = item.Cref + "  |   " + item.Descripcion, Value = item.IDCaracteristica.ToString() };

                        listadesuajes.Add(art);
                    }

                    var descripciontip = new SelectListItem()
                    {
                        Value = "0",
                        Text = "--- Selecciona una Suaje ---"
                    };
                    listadesuajes.Insert(0, descripciontip);
                    return new SelectList(listadesuajes, "Value", "Text");
                }
            }


            public IEnumerable<SelectListItem> GetSuajes(int seleccionado)
            {
                using (var context = new ArticuloContext())
                {
                    string cadena = "select C.ID as IDCaracteristica, A.Cref,A.Descripcion from Articulo as A inner join Caracteristica as C on A.IDArticulo=C.Articulo_IDArticulo where A.IDTipoArticulo=2 and A.obsoleto='0' and C.obsoleto='0' and ((a.idfamilia= 11 or a.idfamilia= 69 or a.idfamilia= 71 or a.idfamilia= 72 or a.idfamilia= 75 or a.IDFamilia=76 or a.IDFamilia=77 or a.IDFamilia=79 or a.IDFamilia=80 or a.IDFamilia=81)) order by A.descripcion";
                    List<suajeC> listadesuaje = context.Database.SqlQuery<suajeC>(cadena).ToList();

                    List<SelectListItem> listadesuajes = new List<SelectListItem>();
                    foreach (suajeC item in listadesuaje)
                    {
                        SelectListItem art = new SelectListItem { Text = item.Cref + "  |   " + item.Descripcion, Value = item.IDCaracteristica.ToString() };
                        if (item.IDCaracteristica==seleccionado)
                        {
                            art.Selected = true;
                        }
                        listadesuajes.Add(art);
                    }

                    var descripciontip = new SelectListItem()
                    {
                        Value = "0",
                        Text = "--- Selecciona una Suaje ---"
                    };
                    listadesuajes.Insert(0, descripciontip);
                    return new SelectList(listadesuajes, "Value", "Text");
                }
            }

            public IEnumerable<SelectListItem> GetMaquinas(int seleccionado)
            {
                using (var context = new ArticuloContext())
                {
                    string cadena = "select A.* from (Articulo as A inner join Caracteristica as C on A.IDArticulo=C.Articulo_IDArticulo) inner join Maquinaproceso as M on A.IDArticulo=M.IDArticulo where A.IDTipoArticulo=3 and A.obsoleto='0' and C.obsoleto='0'  and M.IDProceso=5 order by A.descripcion";
                    List<Articulo> listadesuaje = context.Database.SqlQuery<Articulo>(cadena).ToList();

                    List<SelectListItem> listadesuajes = new List<SelectListItem>();
                    foreach (Articulo item in listadesuaje)
                    {
                        SelectListItem art = new SelectListItem { Text =  item.Descripcion, Value = item.IDArticulo.ToString() };
                        if (item.IDArticulo == seleccionado)
                        {
                            art.Selected = true;
                        }
                        listadesuajes.Add(art);
                    }
                    if (seleccionado == 0)
                    {
                        var descripciontip = new SelectListItem()
                        {
                            Value = "0",
                            Text = "--- Selecciona una Maquina ---"
                        };
                        listadesuajes.Insert(0, descripciontip);
                    }
                    return new SelectList(listadesuajes, "Value", "Text");
                }
            }

            public IEnumerable<SelectListItem> GetMaquinasEmbo(int seleccionado)
            {
                using (var context = new ArticuloContext())
                {
                    string cadena = "select A.* from (Articulo as A inner join Caracteristica as C on A.IDArticulo=C.Articulo_IDArticulo) inner join Maquinaproceso as M on A.IDArticulo=M.IDArticulo where A.IDTipoArticulo=3 and A.obsoleto='0' and C.obsoleto='0'  and (M.IDProceso=4 or M.IDProceso=16) order by M.IDproceso , A.descripcion";
                    List<Articulo> listadesuaje = context.Database.SqlQuery<Articulo>(cadena).ToList();

                    List<SelectListItem> listadesuajes = new List<SelectListItem>();
                    foreach (Articulo item in listadesuaje)
                    {
                        SelectListItem art = new SelectListItem { Text = item.Descripcion, Value = item.IDArticulo.ToString() };
                        if (item.IDArticulo == seleccionado)
                        {
                            art.Selected = true;
                        }
                        listadesuajes.Add(art);
                    }
                    if (seleccionado == 0)
                    {
                        var descripciontip = new SelectListItem()
                        {
                            Value = "0",
                            Text = "--- Selecciona una Maquina ---"
                        };
                        listadesuajes.Insert(0, descripciontip);
                    }
                    return new SelectList(listadesuajes, "Value", "Text");
                }
            }

            public IEnumerable<SelectListItem> GetPlecas()
            {
                using (var context = new ArticuloContext())
                {
                    string cadena = "select C.ID as IDCaracteristica, A.Cref,A.Descripcion from Articulo as A inner join Caracteristica as C on A.IDArticulo=C.Articulo_IDArticulo where A.IDTipoArticulo=2 and A.obsoleto='0' and C.obsoleto='0' and (A.IDFamilia=70) order by A.cref";
                    List<suajeC> listadesuaje = context.Database.SqlQuery<suajeC>(cadena).ToList();

                    List<SelectListItem> listadesuajes = new List<SelectListItem>();
                    foreach (suajeC item in listadesuaje)
                    {
                        SelectListItem art = new SelectListItem { Text = item.Cref + "  |   " + item.Descripcion, Value = item.IDCaracteristica.ToString() };

                        listadesuajes.Add(art);
                    }

                    var descripciontip = new SelectListItem()
                    {
                        Value = "0",
                        Text = "--- Selecciona una Suaje ---"
                    };
                    listadesuajes.Insert(0, descripciontip);
                    return new SelectList(listadesuajes, "Value", "Text");
                }
            }
              

            public IEnumerable<SelectListItem> GetSuajesProduccion(int Seleccionado)
            {
                using (var context = new ArticuloContext())
                {
                    string cadena = "select C.ID as IDCaracteristica, A.Cref,A.Descripcion from Articulo as A inner join Caracteristica as C on A.IDArticulo=C.Articulo_IDArticulo where A.IDTipoArticulo=2 and A.obsoleto='0' and C.obsoleto='0' and (A.IDFamilia=76 or A.IDFamilia=75 or A.IDFamilia=77 or A.IDFamilia=71 or A.IDFamilia =72 or A.IDFamilia=11 or A.IDFamilia=69) order by A.descripcion";
                    List<suajeC> listadesuaje = context.Database.SqlQuery<suajeC>(cadena).ToList();

                    List<SelectListItem> listadesuajes = new List<SelectListItem>();
                    foreach (suajeC item in listadesuaje)
                    {
                        SelectListItem art = new SelectListItem { Text = item.Descripcion, Value = item.IDCaracteristica.ToString() };
                        if (item.IDCaracteristica == Seleccionado)
                        {
                            art.Selected = true;
                        }
                        listadesuajes.Add(art);
                    }

                    var descripciontip = new SelectListItem()
                    {
                        Value = "0",
                        Text = "--- Selecciona una Suaje ---"
                    };
                    listadesuajes.Insert(0, descripciontip);
                    return new SelectList(listadesuajes, "Value", "Text");
                }
            }

            public IEnumerable<SelectListItem> GetPlecas(int Seleccionado)
            {
                using (var context = new ArticuloContext())
                {
                    string cadena = "select C.ID as IDCaracteristica, A.Cref,A.Descripcion from Articulo as A inner join Caracteristica as  C on A.IDArticulo=C.Articulo_IDArticulo inner join Familia as f on a.idfamilia=f.idfamilia where A.IDTipoArticulo=2 and A.obsoleto='0' and C.obsoleto='0' and  (f.descripcion like'%suaje%' or f.descripcion like '%pleca%') order by A.cref";
                    List<suajeC> listadesuaje = context.Database.SqlQuery<suajeC>(cadena).ToList();

                    List<SelectListItem> listadesuajes = new List<SelectListItem>();
                    foreach (suajeC item in listadesuaje)
                    {
                        SelectListItem art = new SelectListItem { Text = item.Cref + " | " + item.Descripcion, Value = item.IDCaracteristica.ToString() };
                        if (item.IDCaracteristica == Seleccionado)
                        {
                            art.Selected = true;
                        }
                        listadesuajes.Add(art);
                    }

                    var descripciontip = new SelectListItem()
                    {
                        Value = "0",
                        Text = "--- Selecciona una Suaje ---"
                    };
                    listadesuajes.Insert(0, descripciontip);
                    return new SelectList(listadesuajes, "Value", "Text");
                }
            }

        }

        public JsonResult getsuajesblando(string buscar)
        {


            using (var context = new ArticuloContext())
            {
                string cadena = "select C.ID as IDCaracteristica, A.Cref,A.Descripcion from Articulo as A inner join Caracteristica as C on A.IDArticulo=C.Articulo_IDArticulo INNER JOIN Familia as f on a.IDFamilia=f.IDFamilia where A.IDTipoArticulo=2 and A.obsoleto='0' and C.obsoleto='0' and (f.descripcion Like '%SUAJE%' or f.descripcion Like '%PLECA%') and A.cref like '%" + buscar + "%'   order by A.descripcion";
                List<suajeC> listadesuaje = context.Database.SqlQuery<suajeC>(cadena).ToList();

                List<SelectListItem> listadesuajes = new List<SelectListItem>();
                foreach (suajeC item in listadesuaje)
                {
                    SelectListItem art = new SelectListItem { Text = item.Cref + "  |   " + item.Descripcion, Value = item.IDCaracteristica.ToString() };

                    listadesuajes.Add(art);
                }

                return Json(listadesuajes, JsonRequestBehavior.AllowGet);
            }


        }


        public JsonResult catidadrechazo(decimal cantidadLi, int IDOrden)
        {


            string mens = "Select*from OrdenProduccion where idorden=" + IDOrden;
            SIAAPI.Models.Produccion.OrdenProduccion orden = new SIAAPI.Models.Produccion.WorkinProcessContext().Database.SqlQuery<SIAAPI.Models.Produccion.OrdenProduccion>(mens).ToList().FirstOrDefault();
            SIAAPI.Models.Comercial.Articulo articulo = new SIAAPI.Models.Produccion.WorkinProcessContext().Database.SqlQuery<SIAAPI.Models.Comercial.Articulo>("Select*from Articulo where IDArticulo=" + orden.IDArticulo).ToList().FirstOrDefault();

            string Descripcion = "";
            if (articulo.IDMuestreo == 1)
            {
                Descripcion = "RE1";
            }
            if (articulo.IDMuestreo == 2)
            {
                Descripcion = "RE15";
            }
            if (articulo.IDMuestreo == 3)
            {
                Descripcion = "RE25";
            }
            if (articulo.IDMuestreo == 4)
            {
                Descripcion = "RE4";
            }
            if (articulo.IDMuestreo == 5)
            {
                Descripcion = "RE65";
            }
            if (articulo.IDMuestreo == 6)
            {
                Descripcion = "RE10";
            }
            if (articulo.IDMuestreo == 0)
            {
                Descripcion = "N/A";
            }
            string Letra = "";
            var CalidadLetras = new SIAAPI.Models.Calidad.CalidadLetraContext().CalidadLetras;
            if (articulo.IDInspeccion == 1)
            {
                foreach (SIAAPI.Models.Calidad.CalidadLetra CalidadL in CalidadLetras)
                {
                    if (cantidadLi >= CalidadL.LI_Lote_mill && cantidadLi <= CalidadL.LS_Lote_mill)
                    {
                        Letra = CalidadL.NGI1;
                    }

                }


            }
            else if (articulo.IDInspeccion == 2)
            {
                foreach (SIAAPI.Models.Calidad.CalidadLetra CalidadL in CalidadLetras)
                {
                    if (cantidadLi >= CalidadL.LI_Lote_mill && cantidadLi <= CalidadL.LS_Lote_mill)
                    {
                        Letra = CalidadL.NGI2;
                    }

                }
            }
            else if (articulo.IDInspeccion == 3)
            {
                foreach (SIAAPI.Models.Calidad.CalidadLetra CalidadL in CalidadLetras)
                {
                    if (cantidadLi >= CalidadL.LI_Lote_mill && cantidadLi <= CalidadL.LS_Lote_mill)
                    {
                        Letra = CalidadL.NGI3;
                    }

                }
            }
            else
            {
                Letra = "N/A";
            }
            if (Letra == "N/A")
            {
                throw new Exception("Artículo Inspección N/A");
            }
            decimal muestraRevi = 0;
            try
            {
                SIAAPI.Models.Calidad.CalidadLimiteAceptacion CodigoLetra = new SIAAPI.Models.Calidad.CalidadContext().Database.SqlQuery<SIAAPI.Models.Calidad.CalidadLimiteAceptacion>("Select*from CalidadLimiteAceptacion where CodigoLetra='" + Letra + "'").ToList().FirstOrDefault();

                muestraRevi = CodigoLetra.TamanoMuestra;

                ViewBag.Letra = Letra;
            }
            catch (Exception err)
            {
                ViewBag.Revisar = 0;
                ViewBag.Letra = "";
            }
            int LimiteRechazo = 0;
            try
            {
                SIAAPI.Models.Comercial.AlmacenContext db = new SIAAPI.Models.Comercial.AlmacenContext();
                if (Descripcion == "N/A")
                {
                    throw new Exception("Artículo Muestreo N/A");
                }
                string cadenaA = " select " + Descripcion + " as Dato from dbo.CalidadLimiteAceptacion where [CodigoLetra]='" + Letra + "'";
                SIAAPI.Models.Comercial.ClsDatoEntero rechazo = db.Database.SqlQuery<SIAAPI.Models.Comercial.ClsDatoEntero>(cadenaA).ToList().FirstOrDefault();
                LimiteRechazo = rechazo.Dato;
            }
            catch (Exception err)
            {

            }
            var result = new { Result = "Successed", rechazo = LimiteRechazo, Revision = muestraRevi };
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Calidad(int IDLibera, string MensajeError = "")
        {
            ViewBag.Mensaje = "";
            if (MensajeError != "")
            {
                ViewBag.Mensaje = MensajeError;
            }
            else
            {
                ViewBag.Mensaje = "";
            }

            char letra = (char)916;
            ViewBag.Delta = letra;

            string cadena1 = " select * from dbo.LiberaOrdenProduccion where [IDLibera]= " + IDLibera + "";
            LiberaOrden liberacion = new WorkinProcessContext().Database.SqlQuery<LiberaOrden>(cadena1).ToList().FirstOrDefault();
            OrdenProduccion orden = new WorkinProcessContext().Database.SqlQuery<OrdenProduccion>("Select*from OrdenProduccion where idorden=" + liberacion.IDOrden).ToList().FirstOrDefault();
            Articulo articulo = new WorkinProcessContext().Database.SqlQuery<Articulo>("Select*from Articulo where IDArticulo=" + orden.IDArticulo).ToList().FirstOrDefault();
            
            ViewBag.liberacion = liberacion;
            ViewBag.IDLibera = IDLibera;
            ViewBag.IDOrden = liberacion.IDOrden;
            ViewBag.IDCliente = new SelectList(new ClientesContext().Clientes, "IDCliente", "Nombre", orden.IDCliente);

            ViewBag.Cliente = orden.IDCliente;
            ViewBag.CantidadLiberada = liberacion.Cantidad;
            ViewBag.Presentacion = orden.Presentacion;
            ViewBag.Articulo = articulo;
            ViewBag.orden = orden;
            CalidadContext db = new CalidadContext();
            ViewBag.IDMuestreo = new SelectList(db.Muestreos, "IDMuestreo", "Descripcion", articulo.IDMuestreo);
            ViewBag.IDInspeccion = new SelectList(db.Inspecciones, "IDInspeccion", "Descripcion", articulo.IDInspeccion);
            string Letra = "";
            var CalidadLetras = new CalidadLetraContext().CalidadLetras;
            if (articulo.IDInspeccion == 1)
            {
                foreach (CalidadLetra CalidadL in CalidadLetras)
                {
                    if (liberacion.Cantidad >= CalidadL.LI_Lote_mill && liberacion.Cantidad <= CalidadL.LS_Lote_mill)
                    {
                        Letra = CalidadL.NGI1;
                    }

                }


            }
            else if (articulo.IDInspeccion == 2)
            {
                foreach (CalidadLetra CalidadL in CalidadLetras)
                {
                    if (liberacion.Cantidad >= CalidadL.LI_Lote_mill && liberacion.Cantidad <= CalidadL.LS_Lote_mill)
                    {
                        Letra = CalidadL.NGI2;
                    }

                }
            }
            else if (articulo.IDInspeccion == 3)
            {
                foreach (CalidadLetra CalidadL in CalidadLetras)
                {
                    if (liberacion.Cantidad >= CalidadL.LI_Lote_mill && liberacion.Cantidad <= CalidadL.LS_Lote_mill)
                    {
                        Letra = CalidadL.NGI3;
                    }

                }
            }
            else
            {
                Letra = "N/A";
            }
            if (Letra == "N/A")
            {
                throw new Exception("Artículo Inspección N/A");
            }
            try
            {
                CalidadLimiteAceptacion CodigoLetra = new WorkinProcessContext().Database.SqlQuery<CalidadLimiteAceptacion>("Select*from CalidadLimiteAceptacion where CodigoLetra='" + Letra + "'").ToList().FirstOrDefault();

                ViewBag.Revisar = CodigoLetra.TamanoMuestra;
                ViewBag.Letra = Letra;
            }
            catch (Exception err)
            {
                ViewBag.Revisar = 0;
                ViewBag.Letra = "";
            }

            ClsCotizador elemento;
            Cotizaciones archivocotizacion = new ArchivoCotizadorContext().cotizaciones.Find(orden.IDHE);
            XmlSerializer serializerX = new XmlSerializer(typeof(ClsCotizador));
            try
            {
                XmlDocument documento = new XmlDocument();
                string nombredearchivo = archivocotizacion.Ruta.TrimEnd() + "\\" + archivocotizacion.NombreArchivo + ".xml";
                documento.Load(nombredearchivo);


                using (Stream reader = new FileStream(nombredearchivo, FileMode.Open))
                {
                    // Call the Deserialize method to restore the object's state.
                    elemento = (ClsCotizador)serializerX.Deserialize(reader);
                }
            }
            catch (Exception er)
            {
                string mensajedeerror = er.Message;
                elemento = (ClsCotizador)serializerX.Deserialize(StringExtensiones.ToXmlReader(archivocotizacion.contenido));
            }


            return View(elemento);
        }

        // POST: Calidad
        [HttpPost]
        public ActionResult Calidad( FormCollection coleccion)
        {
            string catidadMal = coleccion.Get("CantidadMal");
            string CodigoLetra = coleccion.Get("CodigoLetra");
            int ordenpagina = Int16.Parse(coleccion.Get("Orden"));
            int articulopagina = Int16.Parse(coleccion.Get("IDArticulo"));
            string idliberacion = coleccion.Get("IDLiberacion");
            int IDCliente = int.Parse(coleccion.Get("IDCliente").ToString());
           
            OrdenProduccion orden = new WorkinProcessContext().Database.SqlQuery<OrdenProduccion>("Select*from OrdenProduccion where idorden=" + ordenpagina).ToList().FirstOrDefault();
            Articulo articulo = new WorkinProcessContext().Database.SqlQuery<Articulo>("Select*from Articulo where IDArticulo=" + articulopagina).ToList().FirstOrDefault();
            Clientes clientes = new ClientesContext().Clientes.Find(IDCliente);
            orden.IDCliente = clientes.IDCliente;
            int contador = 0;
            string[] arraydatos;
            arraydatos = orden.Presentacion.Split(',');
            int cuantos = arraydatos.Length;
            string metodo = "";
            string resultado = "";
            string parametro = "";
            string valor = string.Empty;
            string unidad = string.Empty;
            string PresentacionResultado = "";
            string PresentacionMetodoResultado = "";
            string PresentacionFinalResultado = "";
            string PresentacionFinalMetodoResultado = "";
            string PresentacionOriginal = "";
            string PresentacionUnidad = "";


            for (int i = 0; i < arraydatos.Length; i++)
            {

                try
                {
                    metodo = coleccion.Get("IDMetodo" + i);
                    resultado = coleccion.Get("Resultado" + i);
                    parametro = coleccion.Get("Parametro" + i);
                    valor = coleccion.Get("Valor" + i);
                    unidad = coleccion.Get("Unidad" + i);

                    if (parametro==string.Empty)
                    {
                        throw new Exception("No hay parametros");
                    }
                    string cuenta = arraydatos[i];
                    string concatenacion = "";
                    string concatenacionMetodo = "";
                    string concatenacionValor = "";
                    string concatenacionUnidad= "";
                    string[] arraydatoscortados;
                    string acc = "";
                    string valor1 = "";
                   
                    try
                    {
                        arraydatoscortados = cuenta.Split(':');
                        acc = arraydatoscortados[0] /*+ ": " + arraydatoscortados[1] + "   "*/;
                        valor1 = arraydatoscortados[1];

                    }
                    catch (Exception err)
                    {

                    }

                    if (acc.ToUpper() != "OBSERVACION" && acc.ToUpper() != "PLECA")
                    {
                        if (valor1 != "0")
                        {
                            concatenacionValor = parametro + ":" + valor + ",";
                            concatenacionUnidad = parametro + ":" + unidad + ",";
                            concatenacionMetodo = parametro + ":" + metodo + ",";
                            concatenacion = parametro + ":" + resultado + ",";
                           
                           

                            PresentacionOriginal += concatenacionValor;
                            PresentacionUnidad += concatenacionUnidad;
                            PresentacionMetodoResultado += concatenacionMetodo;
                            PresentacionResultado += concatenacion;
                          
                        }
                    }
                            
                }
                catch (Exception err)
                {
                    string mensaje = "ya no hay mas parametros";
                }
                          
            }

            PresentacionFinalResultado = PresentacionResultado.TrimEnd(','); 
            PresentacionFinalMetodoResultado = PresentacionMetodoResultado.TrimEnd(','); 
            string PresentacionFinalOriginal = PresentacionOriginal.TrimEnd(',');
            string PresentacionFinalUnidad = PresentacionUnidad.TrimEnd(',');

            string Descripcion = "";
            if (articulo.IDMuestreo == 1)
            {
                Descripcion = "RE1";
            }
            if (articulo.IDMuestreo == 2)
            {
                Descripcion = "RE15";
            }
            if (articulo.IDMuestreo == 3)
            {
                Descripcion = "RE25";
            }
            if (articulo.IDMuestreo == 4)
            {
                Descripcion = "RE4";
            }
            if (articulo.IDMuestreo == 5)
            {
                Descripcion = "RE65";
            }
            if (articulo.IDMuestreo == 6)
            {
                Descripcion = "RE10";
            }
            if (articulo.IDMuestreo == 0)
            {
                Descripcion = "N/A";
            }
            int CantidadEtiquetasMal = int.Parse(catidadMal);
            try
            {
                if (Descripcion == "N/A")
                {
                    throw new Exception("Artículo Muestreo N/A");
                }
                string cadenaA = " select " + Descripcion + " as Dato from dbo.CalidadLimiteAceptacion where [CodigoLetra]='" + CodigoLetra + "'";
                ClsDatoEntero rechazo = db.Database.SqlQuery<ClsDatoEntero>(cadenaA).ToList().FirstOrDefault();


                if ((CantidadEtiquetasMal >= rechazo.Dato))
                {

                    ViewBag.Mensaje = "No se genera certificado de calidad por rechazo";


                }
                else
                {
                    string cadena1 = " select * from dbo.LiberaOrdenProduccion where [IDLibera]= " + idliberacion + "";
                    LiberaOrden liberacion = new WorkinProcessContext().Database.SqlQuery<LiberaOrden>(cadena1).ToList().FirstOrDefault();

                    SIAAPI.Models.Comercial.Clientes cliente = new SIAAPI.Models.Comercial.ClientesContext().Clientes.Find(orden.IDCliente);
                    string fecha = DateTime.Now.Day + "/" + DateTime.Now.Month + "/" + DateTime.Now.Year;
                    try
                    {
                        List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
                        string usuario = userid.Select(s => s.Username).FirstOrDefault();

                        string ingresar = "INSERT INTO [dbo].[CertificadoCalidad] ([FechaCertificado],[IDOrden],[IDPedido],[IDLibera],[IDCliente],[IDArticulo], IDCaracteristica,Descripcion,Presentacion,IDMuestreo,IDInspeccion,Lote,Cantidad,CodigoLetra,MetodoResultadoPresentacion,PresentacionResultado,PresentacionTintas,Clave,Responsable, PresentacionFinalUnidad) VALUES (sysdatetimeoffset(),'" + orden.IDOrden + "','" + orden.IDPedido + "','" + liberacion.IDLibera + "','" + orden.IDCliente + "','" + orden.IDArticulo + "'," + orden.IDCaracteristica + ",'" + orden.Descripcion.Replace("'","''") + "','" + PresentacionFinalOriginal + "'," + articulo.IDMuestreo + "," + articulo.IDInspeccion + ",'" + liberacion.Lote + "'," + liberacion.Cantidad + ",'" + CodigoLetra + "','" + PresentacionFinalMetodoResultado + "','" + PresentacionFinalResultado + "','" + PresentacionFinalOriginal + "','" + articulo.Cref + "','" + usuario + "','"+PresentacionFinalUnidad+"')";

                        db.Database.ExecuteSqlCommand(ingresar);

                    }
                    catch (Exception err)
                    {

                    }
                }
            }
            catch (Exception err)
            {

            }

            if (ViewBag.Mensaje != "" &&  ViewBag.Mensaje != null)
            {
                return RedirectToAction("Calidad", new { IDLibera = idliberacion, MensajeError = ViewBag.Mensaje });
            }

            return RedirectToAction("OrdenesLiberadas");


        }
        public ActionResult PdfCertificado(int id)
        {

            CertificadoCalidad certificado = new CertificadoCalidadContext().CertificadosCalidad.Where(s => s.IDCertificado == id).FirstOrDefault();
            Clientes clientes = new ClientesContext().Clientes.Find(certificado.IDCliente);
            Muestreo muestreo = new CalidadContext().Muestreos.Find(certificado.IDMuestreo);
            DetPedido pedido = new PedidoContext().DetPedido.Where(s => s.IDPedido== certificado.IDPedido && s.Caracteristica_ID== certificado.IDCaracteristica).FirstOrDefault();
            DocumentoCe x = new DocumentoCe();

            x.lote = certificado.Lote;
            //x.fecha = certificado.FechaCertificado.Day+ "-" + certificado.FechaCertificado.Month + "-" + certificado.FechaCertificado.Year;
            String fnew = String.Format(certificado.FechaCertificado.ToString(), "dd/mm/aaaa");
            x.fecha = fnew + " " + certificado.FechaCertificado.Hour;
            x.Cliente = clientes.Nombre;
            x.Etiqueta = certificado.Descripcion;
            x.Presentacion = certificado.Presentacion;
            x.CantidadLiberada = certificado.Cantidad;
                       
            x.Muestreo = muestreo.Descripcion;
            x.Responsable = certificado.Responsable;
            x.Orden = certificado.IDOrden;
            x.Pedido = certificado.IDPedido;
            x.Liberacion = certificado.IDLibera;
            x.IDArticulo = certificado.IDArticulo;
            x.Letra = certificado.CodigoLetra;
            x.PresentacionResultado = certificado.PresentacionResultado;
            x.PresentacionFinalUnidad = certificado.PresentacionFinalUnidad;
            x.MetodoResultado = certificado.MetodoResultadoPresentacion;
            x.Inspeccion = certificado.IDInspeccion;
            x.PresentacionTintas = certificado.PresentacionTintas;
            x.ClaveArticulo = certificado.Clave;

            EmpresaContext dbe = new EmpresaContext();

            var empresa = dbe.empresas.Single(m => m.IDEmpresa == 2);
            x.Empresa = empresa.RazonSocial;
            x.Telefono = empresa.Telefono;
            x.RFC = empresa.RFC;
            x.Direccion = empresa.Calle + " " + empresa.NoExt + " " + empresa.NoInt + "\n" + empresa.Colonia + " \n" + empresa.Municipio + "," + empresa.Estados.Estado;
                       
            string contentType = System.Net.Mime.MediaTypeNames.Application.Pdf;
            System.Drawing.Image logoempresa = byteArrayToImage(empresa.Logo);

            try
            {

                CreaCertificadoPDF documento = new CreaCertificadoPDF(logoempresa, x);
               
                return new FilePathResult(documento.nombreDocumento, contentType);

            }
            catch (Exception err)
            {
                String mensaje = err.Message;
            }
            return RedirectToAction("Index");

        }

        public ActionResult EditarObservcion(int idorden)
        {
            string cadena1 = " select * from dbo.OrdenProduccion where [IDOrden]= " + idorden + "";
            OrdenProduccion ordenp = dbpro.Database.SqlQuery<OrdenProduccion>(cadena1).ToList().FirstOrDefault();
            
            ViewBag.OrdenP = ordenp;
            ViewBag.IDOrden = idorden;
           
            ViewBag.idestadoanterior = ordenp.EstadoOrden;
            
            ViewBag.FecCambio = DateTime.Today.ToShortDateString();
            ViewBag.HorCambio = DateTime.Now.ToString("HH:mm:ss");

            string cadena2 = "  select *  from dbo.EstadoOrden where [Descripcion]= '" + ViewBag.idestadoanterior + "'";
            List<EstadoOrden> edoorden = dbedoo.Database.SqlQuery<EstadoOrden>(cadena2).ToList();
            ViewBag.edoorden = edoorden;
            foreach (var edo in ViewBag.edoorden)
            {
                ViewBag.descripcion = edo.Descripcion;
                ViewBag.tipo = edo.Tipo;
            }


            ViewBag.IDTrabajador = 1;
            return View(ordenp);
        }

        // POST: CambioEstado
        [HttpPost]
        public ActionResult EditarObservcion(OrdenProduccion elemento)
        {
            OrdenProduccion orden = new OrdenProduccionContext().OrdenesProduccion.Find(elemento.IDOrden);

            try
            {
                db.Database.ExecuteSqlCommand("update OrdenProduccion set Indicaciones='" + elemento.Indicaciones + "' where IDOrden= " + orden.IDOrden + "");

                return RedirectToAction("Prioridad");
            }


            catch (Exception ERR)
            {
                return PartialView();
            }
        }


        public ActionResult AgregaPleca(int id, string searchString, int? page)
        {
            OrdenProduccion orden = new OrdenProduccionContext().OrdenesProduccion.Find(id);
            ViewBag.Orden = orden;

            IEnumerable<Articulo> elementos = null;
            if (!string.IsNullOrEmpty(searchString))
            {
                elementos = new ArticuloContext().Database.SqlQuery<Articulo>("select a.* from Articulo as a inner join familia as f on a.idfamilia=f.idfamilia where (f.descripcion like'%suaje%' or f.descripcion like '%pleca%')  and (a.descripcion like'%"+searchString+"%' or a.cref like'%"+searchString+"%')  order by a.descripcion").ToList();
               
                ViewBag.VerPag = 1;


            }
            if (string.IsNullOrEmpty(searchString))
            {
                elementos = new ArticuloContext().Database.SqlQuery<Articulo>("select a.* from Articulo as a inner join familia as f on a.idfamilia=f.idfamilia where (f.descripcion like'%suaje%' or f.descripcion like '%pleca%')   order by a.descripcion").ToList();

                ViewBag.VerPag = 1;


            }
            int pageSize = 12;
            int pageNumber = (page ?? 1);

            return View(elementos.ToPagedList(pageNumber, pageSize));
       
           
        }

        public ActionResult ActualizaPretin( int IDOrden)
        {
            OrdenProduccion OP = new OrdenProduccionContext().OrdenesProduccion.Find(IDOrden);
            new OrdenProduccionContext().Database.ExecuteSqlCommand("update ordenproduccion set ordenproduccion.presentacion =C.presentacion from ordenproduccion inner join caracteristica as C on ordenproduccion.Idcaracteristica=C.ID where IDorden="+ IDOrden);
            new OrdenProduccionContext().Database.ExecuteSqlCommand("update ordenproduccion set ordenproduccion.descripcion =A.Descripcion from ordenproduccion inner join Articulo as A on ordenproduccion.IDArticulo=A.IDArticulo where IDorden=" + IDOrden);
            return RedirectToAction("Prioridad", new { searchString=IDOrden });
        }

        public ActionResult AgregaPlecaSql( int IDArticulo, int IDOrden)
        {
            Caracteristica cara = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from Caracteristica where Articulo_IDArticulo=" + IDArticulo).FirstOrDefault();
            try
            {
                string cadensasql = "Insert into ArticuloProduccion (IDArticulo,IDTipoArticulo,IDCaracteristica,IDProceso,IDOrden, Cantidad, IDClaveUnidad,Indicaciones, CostoPlaneado,CostoReal,Existe, IDHE, TC,TCR) values ";
                cadensasql += "(" + IDArticulo + ",2," + cara.ID + ",5," + IDOrden + ",0.02,53,'',1,1,'1',0,1,1);";
                new ArticuloContext().Database.ExecuteSqlCommand(cadensasql);
                    }
            catch(Exception err)
            {
            }
            return RedirectToAction("Prioridad");

        }

        public ActionResult VerApartados(int idarticulo,int idcaracteristica,int idalmacen)
        {
            List<Vapartado> elementos = new OrdenCompraContext().Database.SqlQuery<Vapartado>("select * from Vapartado where idarticulo=" + idarticulo + " and idcaracteristica=" + idcaracteristica + " and idalmacen=" + idalmacen).ToList();

            return PartialView(elementos);
        }


        public ActionResult VLiberaciones()
        {
            EFecha elemento = new EFecha();
            return View(elemento);
        }

        [HttpPost]
        public ActionResult VLiberaciones(EFecha modelo, FormCollection coleccion)
        {
            VLiberacionesContext dbe = new VLiberacionesContext();
            string FI = modelo.fechaini.Year.ToString() + "-" + modelo.fechaini.Month.ToString() + "-" + modelo.fechaini.Day.ToString();
            string FF = modelo.fechafin.Year.ToString() + "-" + modelo.fechafin.Month.ToString() + "-" + modelo.fechafin.Day.ToString();

            string cual = coleccion.Get("Enviar");
            string cadena = "";

            if (cual == "Generar reporte")
            {
                return View();
            }
            if (cual == "Generar excel")
            {

                cadena = "select * from VLiberaciones where FechaLiberacion >= '" + FI + "' and FechaLiberacion <= '" + FF + "'  order by IDOrden desc";
                var datos = dbe.Database.SqlQuery<VLiberaciones>(cadena).ToList();
                ViewBag.datos = datos;

                ExcelPackage Ep = new ExcelPackage();
                //Crear la hoja y poner el nombre de la pestaña del libro
                var Sheet = Ep.Workbook.Worksheets.Add("Ordenes Liberadas");

                // en la fila1 formateo las celdas y coloco el título de la hoja
                // RichText permite agregar las propiedades de tipo de letra: color, negrita, etc.
                int row = 1;
                //Fijar la fuente para A1:Q1
                Sheet.Cells["A1:M1"].Style.Font.Size = 20;
                Sheet.Cells["A1:M1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:M3"].Style.Font.Bold = true;
                Sheet.Cells["A1:M1"].Style.Font.Color.SetColor(Color.DarkBlue);
                Sheet.Cells["A1:M1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                Sheet.Cells["A1"].RichText.Add("Ordenes Liberadas");

                row = 2;
                Sheet.Cells["A1:M1"].Style.Font.Size = 12;
                Sheet.Cells["A1:M1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:M1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
                Sheet.Cells["A2:M2"].Style.Font.Bold = true;
                //Subtitulo según el filtrado del periodo de datos
                row = 2;
                Sheet.Cells[string.Format("A2", row)].Value = "Fecha inicial";
                Sheet.Cells[string.Format("B2", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("B2", row)].Value = FI;
                Sheet.Cells[string.Format("D2", row)].Value = "Fecha Final";
                Sheet.Cells[string.Format("E2", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("E2", row)].Value = FF;
                //En la fila3 se da el formato a el encabezado
                row = 3;
                Sheet.Cells.Style.Font.Name = "Calibri";
                Sheet.Cells.Style.Font.Size = 10;
                Sheet.Cells["A3:M3"].Style.Font.Bold = true;
                Sheet.Cells["A3:M3"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                Sheet.Cells["A3:M3"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                // Se establece el nombre que identifica a cada una de las columnas de datos.
                Sheet.Cells["A3"].RichText.Add("No. Liberación");
                Sheet.Cells["B3"].RichText.Add("No. Pedido");
                Sheet.Cells["C3"].RichText.Add("No. Orden");
                Sheet.Cells["D3"].RichText.Add("Fecha Liberación");
                Sheet.Cells["E3"].RichText.Add("Cliente");
                Sheet.Cells["F3"].RichText.Add("Clave");
                Sheet.Cells["G3"].RichText.Add("Artículo");
                Sheet.Cells["H3"].RichText.Add("Presentación");
                Sheet.Cells["I3"].RichText.Add("Lote");
                Sheet.Cells["J3"].RichText.Add("Cantidad");
                Sheet.Cells["K3"].RichText.Add("Tipo de Liberación");
                Sheet.Cells["L3"].RichText.Add("Almacén");
                Sheet.Cells["M3"].RichText.Add("Libero");

                //Aplicar borde doble al rango de celdas A3:Q3
                Sheet.Cells["A3:M3"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;

                // en la fila4, el foreach llenara cada fila de la hoja de excel, con cada fila que trae de la bd.
                // Se establecen los formatos para las celdas: Fecha, Moneda
                row = 4;
                Sheet.Cells.Style.Font.Bold = false;
                foreach (VLiberaciones item in ViewBag.datos)
                {
                    Sheet.Cells[string.Format("A{0}", row)].Value = item.IDLibera;
                    Sheet.Cells[string.Format("B{0}", row)].Value = item.IDPedido;
                    Sheet.Cells[string.Format("C{0}", row)].Value = item.IDOrden;
                    Sheet.Cells[string.Format("D{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    Sheet.Cells[string.Format("D{0}", row)].Value = item.FechaLiberacion;
                    Sheet.Cells[string.Format("E{0}", row)].Value = item.Nombre;
                    Sheet.Cells[string.Format("F{0}", row)].Value = item.Cref;
                    Sheet.Cells[string.Format("G{0}", row)].Value = item.Descripcion;
                    Sheet.Cells[string.Format("H{0}", row)].Value = item.Presentacion;
                    Sheet.Cells[string.Format("I{0}", row)].Value = item.Lote;
                    Sheet.Cells[string.Format("J{0}", row)].Value = item.Cantidad;
                    Sheet.Cells[string.Format("K{0}", row)].Value = item.TipoLiberacion;
                    Sheet.Cells[string.Format("L{0}", row)].Value = item.Almacen;
                    Sheet.Cells[string.Format("M{0}", row)].Value = item.Usuario;

                    row++;
                }


                //Se genera el archivo y se descarga

                Sheet.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment: filename=" + "VLiberaciones.xlsx");
                Response.BinaryWrite(Ep.GetAsByteArray());
                Response.End();
                return Redirect("/blah");
            }
            return Redirect("index");
        }

        public ActionResult EntreFechasMtsL()
        {

            EFecha elemento = new EFecha();


            return View(elemento);
        }


        [HttpPost]
        public ActionResult EntreFechasMtsL(EFecha modelo, FormCollection coleccion)
        {
            MtsLContext dbr = new MtsLContext();
            string FI = modelo.fechaini.Year.ToString() + "-" + modelo.fechaini.Month.ToString() + "-" + modelo.fechaini.Day.ToString();
            string FF = modelo.fechafin.Year.ToString() + "-" + modelo.fechafin.Month.ToString() + "-" + modelo.fechafin.Day.ToString();
            decimal mtsE = 0;
            decimal mtsP = 0;
            decimal porcenTBCA = 0;
            decimal porcenT1 = 0;
            decimal porcenT14 = 0;
            decimal porcenT57 = 0;
            decimal porcenT8 = 0;

            int cuentab = 0;
            int cuenta1 = 0;
            int cuenta24 = 0;
            int cuenta57 = 0;
            int cuenta8 = 0;

            decimal PB = 0;
            decimal P1 = 0;
            decimal P24 = 0;
            decimal P57 = 0;
            decimal P8 = 0;
            string cual = coleccion.Get("Enviar");

            string cadena = "";
            if (cual == "Generar reporte pdf")
            {
                return View();
            }
            if (cual == "Generar excel")
            {

                cadena = "select distinct * from MtsL where   FechaCreacion >= '" + FI + " 00:00:00.1' and FechaCreacion <='" + FF + " 23:59:59.999' and EstadoOrden ='Finalizada' and Liberar='Finalizado' order by Orden";
                var datos = dbr.Database.SqlQuery<MtsL>(cadena).ToList();
                ViewBag.datos = datos;
                ExcelPackage Ep = new ExcelPackage();
                //Crear la hoja y poner el nombre de la pestaña del libro
                var Sheet = Ep.Workbook.Worksheets.Add("Mts Lineales");

                // en la fila1 formateo las celdas y coloco el título de la hoja
                // RichText permite agregar las propiedades de tipo de letra: color, negrita, etc.
                int row = 1;
                //Fijar la fuente para A1:Q1
                Sheet.Cells["A1:T1"].Style.Font.Size = 20;
                Sheet.Cells["A1:T1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:T3"].Style.Font.Bold = true;
                Sheet.Cells["A1:T1"].Style.Font.Color.SetColor(Color.DarkBlue);
                Sheet.Cells["A1:T1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                Sheet.Cells["A1"].RichText.Add("Metros Lineales");

                row = 2;
                Sheet.Cells["A1:T1"].Style.Font.Size = 12;
                Sheet.Cells["A1:T1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:T1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
                Sheet.Cells["A2:T2"].Style.Font.Bold = true;
                //Subtitulo según el filtrado del periodo de datos
                row = 2;
                Sheet.Cells[string.Format("A2", row)].Value = "Fecha inicial";
                Sheet.Cells[string.Format("B2", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("B2", row)].Value = FI;
                Sheet.Cells[string.Format("D2", row)].Value = "Fecha Final";
                Sheet.Cells[string.Format("E2", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("E2", row)].Value = FF;
                //En la fila3 se da el formato a el encabezado

                row = 3;
                Sheet.Cells.Style.Font.Name = "Calibri";
                Sheet.Cells.Style.Font.Size = 10;
                Sheet.Cells["I3:U3"].Style.Font.Bold = true;
                Sheet.Cells["I3:U3"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                Sheet.Cells["N3:P3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                Sheet.Cells["Q3:S3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                Sheet.Cells["I3:U3"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                // Se establece el nombre que identifica a cada una de las columnas de datos.
                Sheet.Cells["I3"].RichText.Add("Mts Entregados Totales");
                Sheet.Cells["J3"].RichText.Add("Mts Embobinados Totales");
                Sheet.Cells["L3"].RichText.Add("% Desperdicio Bca");
                Sheet.Cells["M3"].RichText.Add("% Desperdicio 1 Tinta");
                Sheet.Cells["N3"].RichText.Add("% Desperdicio 2 a 4 Tintas");
                Sheet.Cells["Q3"].RichText.Add("% Desperdicio 5 a 7 Tintas");
                Sheet.Cells["T3"].RichText.Add("% Desperdicio 8 Tintas");
                Sheet.Cells["U3"].RichText.Add("% Desperdicio Manga Termoencogible");

                row = 5;
                Sheet.Cells.Style.Font.Name = "Calibri";
                Sheet.Cells.Style.Font.Size = 10;
                Sheet.Cells["A5:T5"].Style.Font.Bold = true;
                Sheet.Cells["A5:T5"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                Sheet.Cells["A5:T5"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                // Se establece el nombre que identifica a cada una de las columnas de datos.
                Sheet.Cells["A5"].RichText.Add("Nombre");
                Sheet.Cells["B5"].RichText.Add("Máquina");
                Sheet.Cells["C5"].RichText.Add("Orden");
                Sheet.Cells["D5"].RichText.Add("Cliente");
                Sheet.Cells["E5"].RichText.Add("Clave");
                Sheet.Cells["F5"].RichText.Add("Materia Prima");
                Sheet.Cells["G5"].RichText.Add("Ancho");
                Sheet.Cells["H5"].RichText.Add("Largo");
                Sheet.Cells["I5"].RichText.Add("Mts Entregados");
                Sheet.Cells["J5"].RichText.Add("Mts Embobinados");
                Sheet.Cells["K5"].RichText.Add("% Diferencia");
                Sheet.Cells["L5"].RichText.Add("No Tintas");
                //Sheet.Cells["M5"].RichText.Add("1");
                //Sheet.Cells["N5"].RichText.Add("2");
                //Sheet.Cells["O5"].RichText.Add("3");
                //Sheet.Cells["P5"].RichText.Add("4");
                //Sheet.Cells["Q5"].RichText.Add("5");
                //Sheet.Cells["R5"].RichText.Add("6");
                //Sheet.Cells["S5"].RichText.Add("7");
                //Sheet.Cells["T5"].RichText.Add("8");
                //Aplicar borde doble al rango de celdas A3:Q3
                Sheet.Cells["A5:L5"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;

                // en la fila4, el foreach llenara cada fila de la hoja de excel, con cada fila que trae de la bd.
                // Se establecen los formatos para las celdas: Fecha, Moneda
                row = 6;
                Sheet.Cells.Style.Font.Bold = false;


                Sheet.Cells.Style.Font.Name = "Calibri";
                Sheet.Cells.Style.Font.Size = 10;
                Sheet.Cells["A"+row+":T"+row+""].Style.Font.Bold = true;
                Sheet.Cells["A"+row+":T"+row+""].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                Sheet.Cells["A"+row+ ":T" + row + ""].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                Sheet.Cells[string.Format("A{0}", row)].Value = "EMBOBINADO";
                Sheet.Cells[string.Format("B{0}", row)].Value = "";
                Sheet.Cells[string.Format("C{0}", row)].Value = "";
                Sheet.Cells[string.Format("D{0}", row)].Value = "";
                Sheet.Cells[string.Format("E{0}", row)].Value = "";
                Sheet.Cells[string.Format("F{0}", row)].Value = "";
                Sheet.Cells[string.Format("G{0}", row)].Value = "";
                Sheet.Cells[string.Format("H{0}", row)].Value = "";
                //Sheet.Cells[string.Format("I{0}", row)].Style.Numberformat.Format = "#,##0.00";
                Sheet.Cells[string.Format("I{0}", row)].Value ="";
                Sheet.Cells["A"+row+":L"+row+""].Style.Border.Bottom.Style = ExcelBorderStyle.Double;

                row = 7;
                foreach (MtsL item in ViewBag.datos)
                {
                  
                    var datosBitacora = dbr.Database.SqlQuery<ConBitacora>("select distinct idorden, idtrabajador, idmaquina, idproceso from bitacora where idorden=" + item.Orden).ToList();
                    foreach (ConBitacora bitacora in datosBitacora)
                    {
                        Trabajador trabajador = new TrabajadorContext().Trabajadores.Find(bitacora.IDTrabajador);
                        Articulo maquina = new ArticuloContext().Articulo.Find(bitacora.IDMaquina);
                        Proceso proceso = new ProcesoContext().Procesos.Find(bitacora.IDProceso);
                        MaterialAsignado material = new InventarioAlmacenContext().MaterialAsignado.Find(item.IDMaterialAsignado);
                        if (bitacora.IDProceso == 4)
                        {
                            decimal mestrosentregadoslineales = 0;
                            try
                            {
                                ClsDatoDecimal suma = db.Database.SqlQuery<ClsDatoDecimal>("select sum (ocupadolineal) as Dato from workinprocess where orden=" + item.Orden + "and idcaracteristica=" + item.IDCaracteristica).ToList().FirstOrDefault();
                                mestrosentregadoslineales = Math.Round(suma.Dato, 3);

                            }
                            catch (Exception err)
                            {

                            }
                           

                            if (item.Descripcion.Contains("NYLON") || item.Descripcion.Contains("ADHESIVO") || item.Descripcion.Contains("LAMINADO"))
                            {

                            }
                            else
                            {
                                OrdenProduccion orden = new OrdenProduccionContext().OrdenesProduccion.Find(item.Orden);

                                Sheet.Cells[string.Format("A{0}", row)].Value = trabajador.Nombre;
                                Sheet.Cells[string.Format("B{0}", row)].Value = maquina.Cref;
                                Sheet.Cells[string.Format("C{0}", row)].Value = item.Orden;
                                Sheet.Cells[string.Format("D{0}", row)].Value = item.Cliente;
                                Sheet.Cells[string.Format("E{0}", row)].Value = item.Cref;
                                Sheet.Cells[string.Format("F{0}", row)].Value = item.Descripcion;
                                Sheet.Cells[string.Format("G{0}", row)].Value = material.ancho;
                                Sheet.Cells[string.Format("H{0}", row)].Value = material.largo;
                                //Sheet.Cells[string.Format("I{0}", row)].Style.Numberformat.Format = "#,##0.00";
                                Sheet.Cells[string.Format("I{0}", row)].Value = Math.Round(mestrosentregadoslineales,3);
                                mtsE += mestrosentregadoslineales;


                                decimal mtsEmbo = 0;
                                decimal largo = 0;
                                decimal gapavance = 0;
                                decimal cantidadB = 0;
                                decimal alPaso = 0;
                                Caracteristica ARTP = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from Caracteristica where Id=" + orden.IDCaracteristica).FirstOrDefault();
                                int idhe = ARTP.IDCotizacion;
                                Cotizaciones archivocotizacion = new ArchivoCotizadorContext().cotizaciones.Find(idhe);
                                ClsCotizador especificacion = new ClsCotizador();
                                especificacion = null;
                                try
                                {
                                    XmlSerializer serializerX = new XmlSerializer(typeof(ClsCotizador));
                                    XmlDocument documento = new XmlDocument();

                                    ///Deserealización del XML COIZACION
                                    try
                                    {

                                        string nombredearchivo = archivocotizacion.Ruta.TrimEnd() + "\\" + archivocotizacion.NombreArchivo + ".xml";
                                        documento.Load(nombredearchivo);


                                        using (Stream reader = new FileStream(nombredearchivo, FileMode.Open))
                                        {
                                            // Call the Deserialize method to restore the object's state.
                                            especificacion = (ClsCotizador)serializerX.Deserialize(reader);
                                        }
                                    }
                                    catch (Exception er)
                                    {
                                        string mensajedeerror = er.Message;
                                        especificacion = (ClsCotizador)serializerX.Deserialize(StringExtensiones.ToXmlReader(archivocotizacion.contenido));
                                    }
                                }
                                catch (Exception err)
                                {

                                }
                                try
                                {
                                    try
                                    {
                                        ClsDatoDecimal cantidadL = db.Database.SqlQuery<ClsDatoDecimal>(" select sum(cantidad) as dato from LiberaOrdenProduccion where idorden=" + item.Orden).ToList().FirstOrDefault();
                                        cantidadB = cantidadL.Dato;
                                    }
                                    catch (Exception err)
                                    {

                                    }
                                    FormulaSiaapi.Formulas FORMULA = new FormulaSiaapi.Formulas();
                                    FORMULA.cadenadepresentacion = orden.Presentacion;
                                    try
                                    {
                                        //largo = FORMULA.getvalor("LARGO", orden.Presentacion);
                                        largo = especificacion.largoproductomm;

                                    }
                                    catch
                                    {

                                    }
                                    try
                                    {
                                        //gapavance = FORMULA.getvalor("GAP AVANCE", orden.Presentacion);

                                        gapavance = especificacion.gapavance;
                                    }
                                    catch
                                    {

                                    }
                                    //try
                                    //{
                                    //    //alPaso = FORMULA.getvalor("AL PASO", orden.Presentacion);
                                    //    alPaso = especificacion.productosalpaso;


                                    //}
                                    //catch
                                    //{

                                    //}


                                    ///repiticones del suaje
                                    ///
                                    Caracteristica cara = new ArticuloContext().Database.SqlQuery<Caracteristica>("select c.*from  ArticuloProduccion as ap inner join Caracteristica as c on c.ID=ap.IDCaracteristica where ap.IDTipoArticulo=2 and ap.IDProceso=5 and idorden="+ item.Orden).FirstOrDefault();


                                    try
                                    {
                                        try
                                        {
                                           alPaso = int.Parse(FORMULA.getvalor("REP EJE", cara.Presentacion).ToString());
                                            if (alPaso == 0)
                                            {
                                                alPaso = int.Parse(FORMULA.getvalor("REPETICIONES EJE", cara.Presentacion).ToString());
                                                if (alPaso == 0)
                                                {
                                                    alPaso = int.Parse(FORMULA.getvalor("REPETICIONES AL EJE", cara.Presentacion).ToString());
                                                    if (alPaso == 0)
                                                    {
                                                        alPaso = int.Parse(FORMULA.getvalor("CAV EJE", cara.Presentacion).ToString());
                                                        if (alPaso == 0)
                                                        {
                                                            alPaso = int.Parse(FORMULA.getvalor("CAVIDADES EJE", cara.Presentacion).ToString());
                                                            if (alPaso == 0)
                                                            {
                                                                alPaso = int.Parse(FORMULA.getvalor("CAVIDADES AL EJE", cara.Presentacion).ToString());

                                                            }
                                                        }
                                                    }
                                                }
                                            }

                                        }
                                        catch (Exception err)
                                        {
                                            string mensajederror = err.Message;
                                            alPaso = 2;
                                        }


                                    }
                                    catch
                                    {

                                    }
                                    mtsEmbo = ((cantidadB / alPaso) * (largo + gapavance));
                                    //mtsEmbo = ((cantidadB * (Convert.ToDecimal(largo) + Convert.ToDecimal(gapavance)))/ Convert.ToDecimal(alPaso)) /1000;  //// dividirlo enytre las etiquetas al paso (repeticiones al eje)

                                }
                                catch (Exception err)
                                {

                                }

                                Sheet.Cells[string.Format("J{0}", row)].Value = Math.Round(mtsEmbo,3);
                                mtsP += mtsEmbo;

                                string cadT = "SELECT COUNT(*) as Dato  from dbo.ArticuloProduccion WHERE IDTipoArticulo = 7 and idOrden = " + item.Orden + "";
                                ClsDatoEntero cuentaT = db.Database.SqlQuery<ClsDatoEntero>(cadT).ToList().FirstOrDefault();

                                decimal porcentaje = 0;
                                try
                                {
                                    porcentaje = 1 - (mtsEmbo / mestrosentregadoslineales);
                                }
                                catch (Exception err)
                                {

                                }
                                if (cuentaT.Dato == 0)
                                {
                                    porcenTBCA += porcentaje;
                                    cuentab++;
                                }
                                if (cuentaT.Dato == 1)
                                {
                                    porcenT1 += porcentaje;
                                    cuenta1++;
                                }
                                if (cuentaT.Dato == 2 || cuentaT.Dato == 3 || cuentaT.Dato == 4)
                                {
                                    porcenT14 += porcentaje;
                                    cuenta24++;
                                }
                                if (cuentaT.Dato == 5 || cuentaT.Dato == 6 || cuentaT.Dato == 7)
                                {
                                    porcenT57 += porcentaje;
                                    cuenta57++;
                                }
                                if (cuentaT.Dato == 8)
                                {
                                    porcenT8 += porcentaje;
                                    cuenta8++;
                                }
                                Sheet.Cells[string.Format("K{0}", row)].Style.Numberformat.Format = "#0.00%";
                                Sheet.Cells[string.Format("K{0}", row)].Value = porcentaje;
                                Sheet.Cells[string.Format("L{0}", row)].Value = cuentaT.Dato;
                                ////////para las tintas deserializacion de la cotizacion
                                ///----------------------------------------------------------///






                                row++;
                            }
                        }

                      


                    }
                }
                ////////////////////para prensa
                Sheet.Cells.Style.Font.Name = "Calibri";
                Sheet.Cells.Style.Font.Size = 10;
                Sheet.Cells["A" + row + ":T" + row + ""].Style.Font.Bold = true;
                Sheet.Cells["A" + row + ":T" + row + ""].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                Sheet.Cells["A" + row + ":T" + row + ""].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                Sheet.Cells[string.Format("A{0}", row)].Value = "PRENSA";
                Sheet.Cells[string.Format("B{0}", row)].Value = "";
                Sheet.Cells[string.Format("C{0}", row)].Value = "";
                Sheet.Cells[string.Format("D{0}", row)].Value = "";
                Sheet.Cells[string.Format("E{0}", row)].Value = "";
                Sheet.Cells[string.Format("F{0}", row)].Value = "";
                Sheet.Cells[string.Format("G{0}", row)].Value = "";
                Sheet.Cells[string.Format("H{0}", row)].Value = "";
                //Sheet.Cells[string.Format("I{0}", row)].Style.Numberformat.Format = "#,##0.00";
                Sheet.Cells[string.Format("I{0}", row)].Value = "";
                Sheet.Cells["A" + row + ":L" + row + ""].Style.Border.Bottom.Style = ExcelBorderStyle.Double;
                row++;
                foreach (MtsL item in ViewBag.datos)
                {

                    var datosBitacora = dbr.Database.SqlQuery<ConBitacora>("select distinct idorden, idtrabajador, idmaquina, idproceso from bitacora where idorden=" + item.Orden).ToList();
                    foreach (ConBitacora bitacora in datosBitacora)
                    {
                        Trabajador trabajador = new TrabajadorContext().Trabajadores.Find(bitacora.IDTrabajador);
                        Articulo maquina = new ArticuloContext().Articulo.Find(bitacora.IDMaquina);
                        Proceso proceso = new ProcesoContext().Procesos.Find(bitacora.IDProceso);
                        MaterialAsignado material = new InventarioAlmacenContext().MaterialAsignado.Find(item.IDMaterialAsignado);
                       
                        ////pROCESO DE PRENSA
                        if (bitacora.IDProceso == 5)
                        {
                            decimal mestrosentregadoslineales = 0;
                            try
                            {
                                ClsDatoDecimal suma = db.Database.SqlQuery<ClsDatoDecimal>("select sum (cantidad) as Dato from materialasignado where orden=" + item.Orden + "and idcaracteristica=" + item.IDCaracteristica).ToList().FirstOrDefault();
                                mestrosentregadoslineales = Math.Round(suma.Dato, 3);

                            }
                            catch (Exception err)
                            {

                            }


                            if (item.Descripcion.Contains("NYLON") || item.Descripcion.Contains("ADHESIVO") || item.Descripcion.Contains("LAMINADO"))
                            {

                            }
                            else
                            {
                                OrdenProduccion orden = new OrdenProduccionContext().OrdenesProduccion.Find(item.Orden);

                                Sheet.Cells[string.Format("A{0}", row)].Value = trabajador.Nombre;
                                Sheet.Cells[string.Format("B{0}", row)].Value = maquina.Cref;
                                Sheet.Cells[string.Format("C{0}", row)].Value = item.Orden;
                                Sheet.Cells[string.Format("D{0}", row)].Value = item.Cliente;
                                Sheet.Cells[string.Format("E{0}", row)].Value = item.Cref;
                                Sheet.Cells[string.Format("F{0}", row)].Value = item.Descripcion;
                                Sheet.Cells[string.Format("G{0}", row)].Value = material.ancho;
                                Sheet.Cells[string.Format("H{0}", row)].Value = material.largo;
                                //Sheet.Cells[string.Format("I{0}", row)].Style.Numberformat.Format = "#,##0.00";
                                Sheet.Cells[string.Format("I{0}", row)].Value = Math.Round(mestrosentregadoslineales, 3);
                                mtsE += mestrosentregadoslineales;


                                decimal mtsEmbo = 0;
                                decimal largo = 0;
                                decimal gapavance = 0;
                                decimal cantidadB = 0;
                                decimal alPaso = 0;
                                Caracteristica ARTP = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from Caracteristica where Id=" + orden.IDCaracteristica).FirstOrDefault();
                                int idhe = ARTP.IDCotizacion;
                                Cotizaciones archivocotizacion = new ArchivoCotizadorContext().cotizaciones.Find(idhe);
                                ClsCotizador especificacion = new ClsCotizador();
                                especificacion = null;
                                try
                                {
                                    XmlSerializer serializerX = new XmlSerializer(typeof(ClsCotizador));
                                    XmlDocument documento = new XmlDocument();

                                    ///Deserealización del XML COIZACION
                                    try
                                    {

                                        string nombredearchivo = archivocotizacion.Ruta.TrimEnd() + "\\" + archivocotizacion.NombreArchivo + ".xml";
                                        documento.Load(nombredearchivo);


                                        using (Stream reader = new FileStream(nombredearchivo, FileMode.Open))
                                        {
                                            // Call the Deserialize method to restore the object's state.
                                            especificacion = (ClsCotizador)serializerX.Deserialize(reader);
                                        }
                                    }
                                    catch (Exception er)
                                    {
                                        string mensajedeerror = er.Message;
                                        especificacion = (ClsCotizador)serializerX.Deserialize(StringExtensiones.ToXmlReader(archivocotizacion.contenido));
                                    }
                                }
                                catch (Exception err)
                                {

                                }
                                try
                                {
                                    try
                                    {
                                        ClsDatoDecimal cantidadL = db.Database.SqlQuery<ClsDatoDecimal>(" select sum(cantidad) as dato from LiberaOrdenProduccion where idorden=" + item.Orden).ToList().FirstOrDefault();
                                        cantidadB = cantidadL.Dato;
                                    }
                                    catch (Exception err)
                                    {

                                    }
                                    FormulaSiaapi.Formulas FORMULA = new FormulaSiaapi.Formulas();
                                    FORMULA.cadenadepresentacion = orden.Presentacion;
                                    try
                                    {
                                        //largo = FORMULA.getvalor("LARGO", orden.Presentacion);
                                        largo = especificacion.largoproductomm;

                                    }
                                    catch
                                    {

                                    }
                                    try
                                    {
                                        //gapavance = FORMULA.getvalor("GAP AVANCE", orden.Presentacion);

                                        gapavance = especificacion.gapavance;
                                    }
                                    catch
                                    {

                                    }
                                  
                                    Caracteristica cara = new ArticuloContext().Database.SqlQuery<Caracteristica>("select c.*from  ArticuloProduccion as ap inner join Caracteristica as c on c.ID=ap.IDCaracteristica where ap.IDTipoArticulo=2 and ap.IDProceso=5 and idorden=" + item.Orden).FirstOrDefault();


                                    try
                                    {
                                        try
                                        {
                                            alPaso = int.Parse(FORMULA.getvalor("REP EJE", cara.Presentacion).ToString());
                                            if (alPaso == 0)
                                            {
                                                alPaso = int.Parse(FORMULA.getvalor("REPETICIONES EJE", cara.Presentacion).ToString());
                                                if (alPaso == 0)
                                                {
                                                    alPaso = int.Parse(FORMULA.getvalor("REPETICIONES AL EJE", cara.Presentacion).ToString());
                                                    if (alPaso == 0)
                                                    {
                                                        alPaso = int.Parse(FORMULA.getvalor("CAV EJE", cara.Presentacion).ToString());
                                                        if (alPaso == 0)
                                                        {
                                                            alPaso = int.Parse(FORMULA.getvalor("CAVIDADES EJE", cara.Presentacion).ToString());
                                                            if (alPaso == 0)
                                                            {
                                                                alPaso = int.Parse(FORMULA.getvalor("CAVIDADES AL EJE", cara.Presentacion).ToString());

                                                            }
                                                        }
                                                    }
                                                }
                                            }

                                        }
                                        catch (Exception err)
                                        {
                                            string mensajederror = err.Message;
                                            alPaso = 2;
                                        }


                                    }
                                    catch
                                    {

                                    }
                                    mtsEmbo = ((cantidadB / alPaso) * (largo + gapavance));
                                    //mtsEmbo = ((cantidadB * (Convert.ToDecimal(largo) + Convert.ToDecimal(gapavance)))/ Convert.ToDecimal(alPaso)) /1000;  //// dividirlo enytre las etiquetas al paso (repeticiones al eje)

                                }
                                catch (Exception err)
                                {

                                }
                                ///metros utilizados
                                ///

                                WorkinProcess woek = new WorkinProcessContext().Database.SqlQuery<WorkinProcess>("select*from WorkinProcess where orden="+ item.Orden +" and idcaracteristica="+item.IDCaracteristica ).ToList().FirstOrDefault();
                                decimal ocupadoLineal = 0M;
                                decimal MetrosUtilizados = 0M;
                                try
                                {
                                    ClsDatoDecimal cantidadL = db.Database.SqlQuery<ClsDatoDecimal>(" select sum(ocupadolineal) as dato from WorkinProcess where orden=" + item.Orden +" and idcaracteristica="+ item.IDCaracteristica).ToList().FirstOrDefault();
                                    ocupadoLineal = cantidadL.Dato;
                                }
                                catch (Exception err)
                                {

                                }
                                if (ocupadoLineal != 0)
                                {
                                    MetrosUtilizados = ocupadoLineal;
                                }
                                Sheet.Cells[string.Format("J{0}", row)].Value = Math.Round(MetrosUtilizados, 3);
                                //mtsP += MetrosUtilizados;

                                string cadT = "SELECT COUNT(*) as Dato  from dbo.ArticuloProduccion WHERE IDTipoArticulo = 7 and idOrden = " + item.Orden + "";
                                ClsDatoEntero cuentaT = db.Database.SqlQuery<ClsDatoEntero>(cadT).ToList().FirstOrDefault();

                                decimal porcentaje = 0;
                                try
                                {
                                    //porcentaje = 1 - (MetrosUtilizados / mestrosentregadoslineales);
                                    porcentaje = (MetrosUtilizados - mestrosentregadoslineales) * (1) / MetrosUtilizados;
                                }
                                catch (Exception err)
                                {

                                }
                                if (cuentaT.Dato == 0)
                                {
                                    porcenTBCA += porcentaje;
                                    cuentab++;
                                }
                                if (cuentaT.Dato == 1)
                                {
                                    porcenT1 += porcentaje;
                                    cuenta1++;
                                }
                                if (cuentaT.Dato == 2 || cuentaT.Dato == 3 || cuentaT.Dato == 4)
                                {
                                    porcenT14 += porcentaje;
                                    cuenta24++;
                                }
                                if (cuentaT.Dato == 5 || cuentaT.Dato == 6 || cuentaT.Dato == 7)
                                {
                                    porcenT57 += porcentaje;
                                    cuenta57++;
                                }
                                if (cuentaT.Dato == 8)
                                {
                                    porcenT8 += porcentaje;
                                    cuenta8++;
                                }
                                Sheet.Cells[string.Format("K{0}", row)].Style.Numberformat.Format = "#0.00%";
                                Sheet.Cells[string.Format("K{0}", row)].Value = porcentaje;
                                Sheet.Cells[string.Format("L{0}", row)].Value = cuentaT.Dato;
                                ////////para las tintas deserializacion de la cotizacion
                                ///----------------------------------------------------------///






                                row++;
                            }
                        }




                    }
                }
                //////aun no hay condición con este cliente
                cadena = "select distinct * from MtsL where   IDCliente = 283 and  FechaCreacion >= '" + FI + " 00:00:00.1' and FechaCreacion <='" + FF + " 23:59:59.999' and EstadoOrden !='Cancelada' order by Orden";
                 datos = dbr.Database.SqlQuery<MtsL>(cadena).ToList();
                ViewBag.datos = datos;


                try
                {
                    PB = (porcenTBCA / Convert.ToDecimal(cuentab));
                }
                catch (Exception err)
                {

                }
                try
                {
                    P1 = (porcenT1 / Convert.ToDecimal(cuenta1));
                }
                catch (Exception err)
                {

                }
                try
                {
                    P24 = (porcenT14 / Convert.ToDecimal(cuenta24));
                }
                catch (Exception err)
                {

                }
                try
                {
                    P57 = (porcenT57 / Convert.ToDecimal(cuenta57));
                }
                catch (Exception err)
                {

                }
                try
                {
                    P8 = (porcenT8 / Convert.ToDecimal(cuenta8));
                }
                catch (Exception err)
                {

                }
            
               
            
               
               
                row = 4;
                Sheet.Cells.Style.Font.Bold = false;
                Sheet.Cells[string.Format("I4", row)].Style.Numberformat.Format = "#,##0.00";
                Sheet.Cells[string.Format("I4", row)].Value = mtsE;
                Sheet.Cells[string.Format("J4", row)].Style.Numberformat.Format = "#,##0.00";
                Sheet.Cells[string.Format("J4", row)].Value = mtsP;
                Sheet.Cells[string.Format("L4", row)].Style.Numberformat.Format = "#0.00%";
                Sheet.Cells[string.Format("L4", row)].Value = PB;
                Sheet.Cells[string.Format("M4", row)].Style.Numberformat.Format = "#0.00%";
                Sheet.Cells[string.Format("M4", row)].Value = P1;
                Sheet.Cells[string.Format("N4", row)].Style.Numberformat.Format = "#0.00%";
                Sheet.Cells[string.Format("N4", row)].Value = P24;
                Sheet.Cells[string.Format("Q4", row)].Style.Numberformat.Format = "#0.00%";
                Sheet.Cells[string.Format("Q4", row)].Value = P57;
                Sheet.Cells[string.Format("T4", row)].Style.Numberformat.Format = "#0.00%";
                Sheet.Cells[string.Format("T4", row)].Value = P8;
                //Se genera el archivo y se descarga

                Sheet.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment: filename=" + "ReporteExcelMtsL.xlsx");
                Response.BinaryWrite(Ep.GetAsByteArray());
                Response.End();
                return Redirect("/blah");
            }
            return Redirect("Index");
        }

    }


    public static class StringExtensiones
    {
        internal static XmlReader ToXmlReader(this string value)
        {
            var settings = new XmlReaderSettings { ConformanceLevel = ConformanceLevel.Fragment, IgnoreWhitespace = true, IgnoreComments = true };

            var xmlReader = XmlReader.Create(new StringReader(value), settings);
            xmlReader.Read();
            return xmlReader;
        }
    }
}

