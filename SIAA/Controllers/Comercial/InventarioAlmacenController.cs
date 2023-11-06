using SIAAPI.Models.Comercial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.Entity;
using PagedList;

using System.IO;
using static SIAAPI.Reportes.InventarioPorAlmacen;
using SIAAPI.Reportes;
using OfficeOpenXml;
using System.Drawing;
using OfficeOpenXml.Style;
using System.Globalization;
using SIAAPI.Models.Administracion;
using SIAAPI.ViewModels.Comercial;
using SIAAPI.Models.Inventarios;
using System.Text;
using SIAAPI.Models.Produccion;
using System.Data.SqlClient;
using System.Runtime.InteropServices.WindowsRuntime;
using SIAAPI.Models.Login;
using SIAAPI.ViewModels.Articulo;
using SIAAPI.Models.PlaneacionProduccion;
using Automatadll;
using System.Xml.Serialization;
using SIAAPI.ClasesProduccion;
using System.Xml;

namespace SIAAPI.Controllers.Comercial
{
    public class InventarioAlmacenController : Controller
    {

        private InventarioAlmacenContext db = new InventarioAlmacenContext();
        // GET: InventarioAlmacen
        public ActionResult Index(string sortOrder, string Almacen, string Enlistar, string currentFilter, string searchString, int? page, int? PageSize)
        {

            //Ejecutar();
            //EjecutarAjusteTintas();


            db.Database.ExecuteSqlCommand("update inventarioalmacen set porllegar=0 where porllegar<0");
            db.Database.ExecuteSqlCommand("update inventarioalmacen set porllegar=0 where porllegar<0");
            db.Database.ExecuteSqlCommand("update inventarioalmacen set apartado=0 where apartado<0");
            db.Database.ExecuteSqlCommand("update inventarioalmacen set existencia=0 where existencia<0.09");
            db.Database.ExecuteSqlCommand("update inventarioalmacen set disponibilidad=(Existencia-Apartado)");
            ViewBag.CurrentSort = sortOrder;

            ViewBag.ASortParm = String.IsNullOrEmpty(sortOrder) ? "Almacen" : "Almacen";
            ViewBag.BSortParm = String.IsNullOrEmpty(sortOrder) ? "Articulo" : "Articulo";
            ViewBag.CSortParm = String.IsNullOrEmpty(sortOrder) ? "Clave" : "Clave";
            // Not sure here
            if (searchString == null)
            {
                searchString = currentFilter;
            }
            if (Almacen==null)
            {
               
               
                    Almacen = "0";
                
            }
            if (Almacen == "")
            {
                Almacen = "0";
            }
            int IDAlmacen = 0;
            try
            {
               IDAlmacen= int.Parse(Almacen);
            }
            catch (Exception err)
            {

            }
            // Pass filtering string to view in order to maintain filtering when paging
            ViewBag.SearchString = searchString;
            //Paginación
            //var elementos = db.Database.SqlQuery<VInventarioAlmacen>("select Carrito.IDCarrito,Carrito.usuario,Carrito.IDCaracteristica,dbo.GetPrecio(0,Caracteristica.Articulo_IDArticulo,0,Carrito.Cantidad) as Precio,Carrito.Cantidad,Caracteristica.Presentacion as Presentacion,Articulo.Descripcion,Articulo.IDArticulo,c_Moneda.Descripcion as Moneda,c_Moneda.IDMoneda as IDMoneda,dbo.GetPrecio(0,Caracteristica.Articulo_IDArticulo,0,Carrito.Cantidad) * Carrito.Cantidad as Importe, Carrito.Nota from  Carrito INNER JOIN Caracteristica ON Carrito.IDCaracteristica= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo INNER JOIN c_Moneda ON c_Moneda.IDMoneda = Articulo.IDMoneda where usuario='" + usuario + "'").ToList();
            ViewBag.Almacen = new AlmacenRepository().GetAlmacenes();

            ViewBag.Almacenseleccionado = Almacen;/// mandar el viewbag el parametro que viene de la pagina anterior

            var Lista = new List<SelectListItem>();

            //Lista.Add(new SelectListItem { Text = "CAST&CURE", Value = "CAST&CURE" });
            Lista.Add(new SelectListItem { Text = "Existencias", Value = "Existencias" });

            ViewBag.Enlistar = new SelectList(Lista, "Value", "Text");
            //var elementos = from s in db.VInventarioAlmacenes
            //                select s;
            string ConsultaSql = "select top 500 * from [dbo].[VInventarioAlmacen]";

            string FiltroSql = string.Empty;
            if (IDAlmacen!=0)
            {
                FiltroSql = "where idalmacen=" + IDAlmacen;
            }
            if (Enlistar == "Existencias")
            {
                if (Almacen!="0")
                {
                    if (Almacen!=null)
                    {
                        FiltroSql = "where existencia>0 and idalmacen="+ IDAlmacen;
                    }
                }
                else
                {
                    FiltroSql = "where existencia>0 ";
                }
                

            }


            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                if (Almacen != "0")
                {
                    if (Almacen != null)
                    {
                        FiltroSql = "where idalmacen="+ IDAlmacen +" and (Cref like '%"+searchString+ "%' or Articulo like '%" + searchString + "%' or Presentacion like '%" + searchString + "%')";
                        //elementos = elementos.Where(s => s.IDAlmacen== IDAlmacen && ( s.Cref.Contains(searchString) || s.Articulo.Contains(searchString) || s.Presentacion.Contains(searchString)));

                    }
                }
                else
                {
                    FiltroSql = "where  (Almacen like '%" + searchString + "%' or Cref like '%" + searchString + "%' or Articulo like '%" + searchString + "%' or Presentacion like '%" + searchString + "%')";

                    //elementos = elementos.Where(s => s.Almacen.Contains(searchString) || s.Cref.Contains(searchString) || s.Articulo.Contains(searchString) || s.Presentacion.Contains(searchString));

                }
            }

            if (!string.IsNullOrEmpty(searchString) && Enlistar == "Existencias")
            {
                if (Almacen != "0")
                {
                    if (Almacen != null)
                    {
                        FiltroSql = "where idalmacen=" + IDAlmacen + " and  existencia>0 and (Cref like '%" + searchString + "%' or Articulo like '%" + searchString + "%' or Presentacion like '%" + searchString + "%')";

                        //elementos = elementos.Where(s => s.Existencia > 0 && s.IDAlmacen == IDAlmacen && ( s.Cref.Contains(searchString) || s.Articulo.Contains(searchString) || s.Presentacion.Contains(searchString)));

                    }
                }
                else
                {
                    FiltroSql = "where  existencia>0 and (Almacen like'%" + searchString + "%' or Cref like '%" + searchString + "%' or Articulo like '%" + searchString + "%' or Presentacion like '%" + searchString + "%')";

                    //elementos = elementos.Where(s => s.Existencia > 0 && (s.Almacen.Contains(searchString) || s.Cref.Contains(searchString) || s.Articulo.Contains(searchString) || s.Presentacion.Contains(searchString)));

                }
            }
          


            //Ordenacion

            switch (sortOrder)
            {
                case "Almacen":
                    FiltroSql = FiltroSql + " order by IDAlmacen";
                    //elementos = elementos.OrderBy(s => s.Almacen);
                    break;
                case "Clave":
                    FiltroSql = FiltroSql + " order by cref";
                    //elementos = elementos.OrderBy(s => s.Cref);
                    break;
                case "Articulo":
                    FiltroSql = FiltroSql + " order by articulo";
                    //elementos = elementos.OrderBy(s => s.Articulo);
                    break;

                default:
                    FiltroSql = FiltroSql + " order by IDInventarioAlmacen desc";
                    //elementos = elementos.OrderByDescending(s => s.IDInventarioAlmacen);
                    break;
            }
           string  CadenaSql = ConsultaSql + " " + FiltroSql ;
            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            var elementos = db.Database.SqlQuery<VInventarioAlmacen>(CadenaSql).ToList();
            int count = elementos.Count();
            //int count = db.InventarioAlmacenes.OrderByDescending(e => e.IDInventarioAlmacen).Count(); // Total number of elements

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
            ViewBag.Lista = Enlistar;
            ViewBag.IDAlmacen = Almacen;


            return View(elementos.ToPagedList(pageNumber, pageSize));
            //Paginación
        }

     
        public ActionResult InventarioStock(int IDAlmacen, string MensajeRespuesta="", int? IDFamilia=0)
        {

            ViewBag.Message = MensajeRespuesta;
            ViewBag.IDAlmacen = new RepositiryAlma().GetAlmacenes();

            ViewBag.Almacenseleccionado = IDAlmacen;/// mandar el viewbag el parametro que viene de la pagina anterior

            //ViewBag.IDAlmacen = IDAlmacen;
            List<SelectListItem> clientes;
            string cadena1c = "select*from Clientes where Nombre like '%Stock %'";
            
            clientes = new ClientesContext().Database.SqlQuery<Clientes>(cadena1c).ToList().OrderBy(s => s.Nombre).Select(c => new SelectListItem
            {
                Value = c.IDCliente.ToString(),
                Text = c.Nombre
            }).ToList();

            SelectListItem sinclien = new SelectListItem();
            sinclien.Text = "---- Selecciona el Cliente ----";
            sinclien.Value = "0";
            sinclien.Selected = true;
            clientes.Add(sinclien);

            ViewBag.clientes = clientes;
            ViewBag.IDFamilia = new FamAlmRepository().GetAlmacenesF(IDAlmacen);
            List<VStockvsAlmacen> almacen = new List<VStockvsAlmacen>();
            string cadena1 = "";
            if (IDFamilia !=0) 
            {
                cadena1 = "select * from VStockvsAlmacen where IDAlmacen=" + IDAlmacen +" and IDFamilia="+ IDFamilia;

            }
            else
            {
                cadena1 = "select * from VStockvsAlmacen where IDAlmacen=" + IDAlmacen;

            }
            almacen = db.Database.SqlQuery<VStockvsAlmacen>(cadena1).ToList();
            
           

            return View(almacen);
        }
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
        public void EjecutarAjusteTintas()
        {
            List<Almacen> almacens = new AlmacenContext().Database.SqlQuery<Almacen>("select*from almacen where descripcion like '%TINTAS%'").ToList();
            foreach (Almacen alm in almacens)
            {
                List<InventarioAlmacen> inventarios = new InventarioAlmacenContext().Database.SqlQuery<InventarioAlmacen>("select*from inventarioalmacen where idalmacen=" + alm.IDAlmacen).ToList();
                foreach (InventarioAlmacen almacen in inventarios)
                {
                    decimal existencia = 0M;

                    try
                    {
                        string consulta = "select sum(cantidad) as dato  from clslotetinta where  estado='Existe' and idcaracteristica=" + almacen.IDCaracteristica + " and idalmacen=" + almacen.IDAlmacen;
                        ClsDatoDecimal datoDecimal = new ArticuloContext().Database.SqlQuery<ClsDatoDecimal>(consulta).FirstOrDefault();
                        existencia = datoDecimal.Dato;
                    }
                    catch (Exception err)
                    {

                    }
                        db.Database.ExecuteSqlCommand("update inventarioalmacen set existencia=" + existencia + " where idalmacen=" + almacen.IDAlmacen + " and idcaracteristica=" + almacen.IDCaracteristica);
                }
            }


            db.Database.ExecuteSqlCommand("update inventarioalmacen set porllegar=0 where porllegar<0");
            db.Database.ExecuteSqlCommand("update inventarioalmacen set apartado=0 where apartado<0");
            db.Database.ExecuteSqlCommand("update inventarioalmacen set existencia=0 where existencia<0.09");
            db.Database.ExecuteSqlCommand("update inventarioalmacen set disponibilidad=(Existencia-Apartado)");

        }

        public ActionResult GeneraOrden(FormCollection collection)
        {

            int articulo = int.Parse(collection.Get("IDArticulo"));
            int carac = int.Parse(collection.Get("IDCaracteristica"));
            int IDAlmacen = int.Parse(collection.Get("IDAlmacen"));
            int cotizacion = int.Parse(collection.Get("idcotizacionarticulo"));
            int IDCliente = int.Parse(collection.Get("idcliente"));
            decimal cantidad = decimal.Parse(collection.Get("Cantidad"));
            SIAAPI.Models.Comercial.Caracteristica caracteristica = new SIAAPI.Models.Comercial.ArticuloContext().Database.SqlQuery<SIAAPI.Models.Comercial.Caracteristica>("select * from Caracteristica where id=" + carac).FirstOrDefault();


            var datosOrden = new List<string>();
            datosOrden.Add(collection.Get("Cantidad"));
            datosOrden.Add(collection.Get("idcotizacionarticulo"));
            datosOrden.Add(collection.Get("IDArticulo"));
            datosOrden.Add(collection.Get("IDCaracteristica"));


            string mmensaje = "";
           
           int idhe = 0;
           List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
           int usuario = userid.Select(s => s.UserID).FirstOrDefault();
           VCaracteristicaContext presentacionBase = new VCaracteristicaContext();
           Articulo articulos = new ArticuloContext().Articulo.Find(articulo);

               
           VCaracteristica presentacion = presentacionBase.VCaracteristica.Find(carac);

           ClsDatoEntero dato2 = new HEspecificacionEContext().Database.SqlQuery<ClsDatoEntero>("select IDCotizacion as Dato from Caracteristica where id=" + presentacion.ID).FirstOrDefault();

           idhe = dato2.Dato;
           if (idhe == 0)
           {
               return Json(new { errorMessage = "El producto no  ha sido cotizado " + articulos.Cref + " No de presentacion " + presentacion.IDPresentacion });
           }
            ////crear pedido
           decimal subtotal = 0, iva = 0, total = 0, subtotalr = 0, ivar = 0, totalr = 0, importetotal = 0, importe = 0, importeiva = 0, Precio = 0;
            int num = 0;
            int iddetpedido = 0;
            try
            {
                
                Clientes cliente = new ClientesContext().Clientes.Find(IDCliente);
               
                DateTime fecha = DateTime.Now;
                string fecha1 = fecha.ToString("yyyy/MM/dd");

                DateTime fechareq = DateTime.Now;
                string fecha2 = fechareq.ToString("yyyy/MM/dd");

                List<c_Moneda> monedaorigen;
                monedaorigen = db.Database.SqlQuery<c_Moneda>("select * from c_Moneda WHERE ClaveMoneda='MXN'").ToList();
                int origen = monedaorigen.Select(s => s.IDMoneda).FirstOrDefault();


                VCambio tcambio = db.Database.SqlQuery<VCambio>("select dbo.GetTipocambio('" + fecha1 + "',180," + origen + ") as TC").ToList()[0];
                decimal tCambio = tcambio.TC;

                db.Database.ExecuteSqlCommand("INSERT INTO [dbo].[EncPedido]([Fecha],[FechaRequiere],[IDCliente],[IDFormapago],[IDMoneda],[Observacion],[Subtotal],[IVA],[Total],[IDMetodoPago],[IDCondicionesPago],[Status],[TipoCambio],[UserID],[IDUsoCFDI],[IDVendedor],[Entrega], [OCompra]) values" +
                    " ('" + fecha1 + "','" + fecha2 + "','" + cliente.IDCliente + "','" + cliente.IDFormapago + "','" + cliente.IDMoneda + "','','" + subtotal + "','" + iva + "','" + total + "','" + cliente.IDMetodoPago + "','" + cliente.IDCondicionesPago + "','Activo','" + tCambio + "','" + usuario + "','" + cliente.IDUsoCFDI + "','" + cliente.IDVendedor + "','" + cliente.Entrega + "','')");
                //db.Database.ExecuteSqlCommand("INSERT INTO [dbo].[EncPedido]([Fecha],[FechaRequiere],[IDCliente],[IDFormapago],[IDMoneda],[Observacion],[Subtotal],[IVA],[Total],[IDMetodoPago],[IDCondicionesPago],[IDAlmacen],[Status],[TipoCambio],[UserID],[IDUsoCFDI],[IDVendedor],[Entrega]) values ('" + fecha1 + "','" + fecha2 + "','" + encPedido.IDCliente + "','" + encPedido.IDFormapago + "','" + encPedido.IDMoneda + "','" + encPedido.Observacion + "','" + subtotal + "','" + iva + "','" + total + "','" + encPedido.IDMetodoPago + "','" + encPedido.IDCondicionesPago + "','" + encPedido.IDAlmacen + "','Inactivo','" + tCambio + "','" + usuario + "','" + encPedido.IDUsoCFDI + "','" + encPedido.IDVendedor + "','" + encPedido.Entrega + "')");
                db.SaveChanges();

                List<EncPedido> numero;
                numero = db.Database.SqlQuery<EncPedido>("SELECT * FROM [dbo].[EncPedido] WHERE IDPedido = (SELECT MAX(IDPedido) from EncPedido)").ToList();
                num = numero.Select(s => s.IDPedido).FirstOrDefault();

                //////////////precio
                ///
                string cadenam = " select * from MatrizPrecioCliente where IDArticulo = " + articulo + " and IDCliente = " + IDCliente;
                MatrizPrecioCliente Moneaveri = new CarritoContext().Database.SqlQuery<MatrizPrecioCliente>(cadenam).ToList().FirstOrDefault();
                //Monedadestino = Moneaveri.IDMoneda;

                Articulo articuloPedido = new ArticuloContext().Articulo.Find(articulo);
                try
                {
                    Precio = Moneaveri.Precio;
                }
                catch (Exception err)
                {
                    mmensaje = err.Message;
                }
              
                importe = cantidad * Precio;
                importeiva = importe * decimal.Parse(SIAAPI.Properties.Settings.Default.ValorIVA);
                importetotal = importeiva + importe;
                db.Database.ExecuteSqlCommand("INSERT INTO DetPedido([IDPedido],[IDArticulo],[Cantidad],[Costo],[CantidadPedida],[Descuento],[Importe],[IVA],[ImporteIva],[ImporteTotal],[Nota],[Ordenado],[Caracteristica_ID],[IDAlmacen],[Suministro],[Status],[Presentacion],[jsonPresentacion],[IDDetExterna],[GeneraOrdenP],[IDRemision],[IDPrefactura]) values" +
                    " ('" + num + "','" + articulo + "','" + cantidad + "','" + Precio + "' * dbo.GetTipocambio('" + fecha1 + "'," + articuloPedido.IDMoneda + "," + articuloPedido.IDMoneda + "),'" + cantidad + "','0','" + importe + "' * dbo.GetTipocambio('" + fecha1 + "'," + articuloPedido.IDMoneda + "," + articuloPedido.IDMoneda + "),'true','" + importeiva + "' *  dbo.GetTipocambio('" + fecha1 + "'," + articuloPedido.IDMoneda + "," + articuloPedido.IDMoneda + "),'" + importetotal + "' * dbo.GetTipocambio('" + fecha1 + "'," + articuloPedido.IDMoneda + "," + articuloPedido.IDMoneda + "),'Pedido Stock','0','" + carac + "','" + IDAlmacen + "','0','Activo','" + presentacion.Presentacion + "','" + presentacion.jsonPresentacion + "','0','" + articuloPedido.GeneraOrden + "','0','0')");

                DetPedido det = new PedidoContext().Database.SqlQuery<DetPedido>("select*from DetPedido where idpedido="+ num).ToList().FirstOrDefault();
                iddetpedido = det.IDDetPedido;
            }
            catch (Exception err)
            {
                mmensaje = err.Message;
            }
                try
                {
                        automata Automata;

                        XmlSerializer serializer = new XmlSerializer(typeof(automata));

                        string path = Path.Combine(Server.MapPath("~/Automatas/Ordenes.Xml"));


                        using (TextReader reader = new StreamReader(path))
                        {
                            serializer = new XmlSerializer(typeof(automata));
                            Automata = (automata)serializer.Deserialize(reader);
                        }

                        Automata.establecerestadoinicial();

                        string estadoinicial = Automata.Estadoactual.Nombre;


                         bool modeloTermoEncogible = false;
                         bool modeloMangaTranparente = false;
                         bool modeloAdherible = false;
                         modeloTermoEncogible = siTermoencongible(caracteristica.IDCotizacion);
                         modeloMangaTranparente = siTermoencongibleTrans(caracteristica.IDCotizacion);
                         modeloAdherible = siAdherible(caracteristica.IDCotizacion);
                         int ModelosDeProduccion_IDModeloProduccion = 0;
                         if (modeloTermoEncogible)
                         {
                             ModelosDeProduccion_IDModeloProduccion = 8;
                         }
                         if (modeloMangaTranparente)
                         {
                             ModelosDeProduccion_IDModeloProduccion = 7;
                         }
                         if (modeloAdherible)
                         {
                             ModelosDeProduccion_IDModeloProduccion = 4;
                         }
                         if (ModelosDeProduccion_IDModeloProduccion == 0)
                         {
                             return Json(new { errorMessage = "Sin modelo de producción" });
                         }

                         int idprocesoactual = db.Database.SqlQuery<int>("select Proceso_IDProceso from ModeloProceso where ModelosDeProduccion_IDModeloProduccion='" + ModelosDeProduccion_IDModeloProduccion + "' and orden=1").FirstOrDefault();
                         string fecharequiere = DateTime.Now.ToString("yyyy/MM/dd");
                         string fecharegistro = DateTime.Now.ToString("yyyy/MM/dd");

                         ArticulosProduccionContext dba = new ArticulosProduccionContext();
                         Articulo articuloaproducion = new ArticuloContext().Articulo.Find(articulo);

                         int idordenproduccion = 0;
                         string fecha = "";
                                      
                       
                          string cadenaorden = "INSERT INTO [dbo].[OrdenProduccion] ([IDModeloProduccion],[IDCliente],[IDArticulo],[IDCaracteristica],[Descripcion],[Presentacion],[Indicaciones],[FechaCompromiso],[FechaInicio],[FechaProgramada],[FechaRealdeInicio],[FechaRealdeTerminacion],[Cantidad],[IDPedido],[IDDetPedido],[Prioridad],[EstadoOrden],[UserID],[Liberar],idhe) VALUES(" +
                          ModelosDeProduccion_IDModeloProduccion + ",'" + IDCliente + "','" + articulo + "','" + carac + "','" + articuloaproducion.Descripcion + "','" + caracteristica.Presentacion + "','','" + fecharequiere + "',SYSDATETIME(),null,null,null,'" + cantidad + "','" + num + "','" + iddetpedido + "',0,'" + estadoinicial + "'," + usuario + ",'Activo'," + idhe + ")";
                          db.Database.ExecuteSqlCommand(cadenaorden);
                          idordenproduccion = db.Database.SqlQuery<int>("select max(IDOrden) from OrdenProduccion").FirstOrDefault();
                          fecha = DateTime.Now.ToString("yyyy-MM-dd");
                 
                          int AlmacenViene = 0;
                          

                          AlmacenViene = IDAlmacen;
                    ///insertar seguimiento
                           try
                           {
                               string cadena = "insert into seguimientostock(IDOrden,Fecha,FechaCompromiso,IDAlmacen,IDArticulo,Presentacion,IDCaracteristica,Cantidad,Status) values" +
                                   " (" + idordenproduccion + ",SYSDATETIME(),SYSDATETIME()," + AlmacenViene + "," + articulo + ",'" + caracteristica.Presentacion + "'," + carac + "," + cantidad + ",'" + estadoinicial + "')";

                               db.Database.ExecuteSqlCommand(cadena);
                           }
                           catch (Exception err)
                           {
                               mmensaje = err.Message;
                           }
                           ///insertar pedidoStock
                           try
                           {
                               string cadena = "insert into PedidosStock (IDOrden,IDPedido,Status) values (" + idordenproduccion + "," + num + ",'" + estadoinicial + "')";
                               db.Database.ExecuteSqlCommand(cadena);
                           }
                           catch (Exception err)
                           {
                               mmensaje = err.Message;
                           }

                           decimal existencia = 0;
                           decimal llegar = 0;
                           InventarioAlmacen ia = new InventarioAlmacenContext().InventarioAlmacenes.ToList().Where(s => s.IDAlmacen == AlmacenViene && s.IDCaracteristica == carac).FirstOrDefault();

                           if (ia != null)
                           {

                               db.Database.ExecuteSqlCommand("update InventarioAlmacen set PorLlegar=(PorLlegar+" + cantidad + ") where IDArticulo= " + articulo + " and IDCaracteristica=" + carac + " and IDAlmacen=" + AlmacenViene + "");
                               existencia = ia.Existencia;
                           }
                           else
                           {
                                   string cadena = "INSERT INTO inventarioAlmacen (IDAlmacen,IDArticulo,IDCaracteristica,Existencia, PorLlegar,Apartado,Disponibilidad) values ("
                                   + AlmacenViene + "," + articulo + "," + carac + ",0," + cantidad + ",0,0)";
                               db.Database.ExecuteSqlCommand(cadena);
                               existencia = 0;
                           }


                            try
                            {
                                 int   ORDEN = db.Database.SqlQuery<int>("select max(IDOrden) from OrdenProduccion").FirstOrDefault();
                                 Caracteristica carateristica = new WorkinProcessContext().Database.SqlQuery<Caracteristica>("select * from caracteristica where id=" + carac).ToList().FirstOrDefault();
                                 InventarioAlmacen iai = new InventarioAlmacenContext().InventarioAlmacenes.ToList().Where(s => s.IDAlmacen == AlmacenViene && s.IDCaracteristica == carac).FirstOrDefault();

                                 string cadenam = "INSERT INTO [dbo].[MovimientoArticulo] ([Fecha],[Id],[IDPresentacion],[Articulo_IDArticulo],[Accion],[cantidad],[Documento],[NoDocumento],[Lote],[IDAlmacen],[TipoOperacion],[acumulado],[observacion],[Hora], usuario) VALUES ";
                                 cadenam += " (getdate(), " + carateristica.ID + "," + carateristica.IDPresentacion + "," + carateristica.Articulo_IDArticulo + ",'Orden Producción'," + iai.Existencia + ",'Orden Producción Stock'," + ORDEN + ",''," + AlmacenViene + ",'NA'," + iai.Existencia + ",'',CONVERT (time, SYSDATETIMEOFFSET()),"+usuario+")";
                                 db.Database.ExecuteSqlCommand(cadenam);
                            }
                            catch (Exception err)
                            {
                                mmensaje = err.Message;
                            }

                            var lista = new VistaModeloProcesoContext().Database.SqlQuery<VistaModeloProceso>("Select * from [VModeloProceso] where idModeloProduccion=" + ModelosDeProduccion_IDModeloProduccion + " order by orden").ToList();
                            ViewBag.listaprocesos = lista;



                            for (int i = 0; i < lista.Count(); i++)
                            {

                                db.Database.ExecuteSqlCommand("INSERT INTO [dbo].[OrdenProduccionDetalle] ([IDOrden],[IDProceso],[EstadoProceso]) VALUES ('" + idordenproduccion + "','" + ViewBag.listaprocesos[i].IDProceso + "','Conflicto')");

                            }

                            decimal tc = db.Database.SqlQuery<ClsDatoDecimal>("select dbo.GetTipocambio(GETDATE(),(select idMoneda from C_MONEDA WHERE clavemoneda='USD'),(select idMoneda from C_MONEDA WHERE clavemoneda='MXN')  )  as Dato").ToList()[0].Dato;

                            string queonda = CrearOrden(idhe, cantidad, idordenproduccion, tc, articuloaproducion.IDMoneda, ModelosDeProduccion_IDModeloProduccion);

                }
                catch (Exception err)
                {
                            mmensaje = err.Message;
                           return Json(new { errorMessage = "El producto tuvo un error " + articulos.Cref + " ->" + err.Message });
                }
            if (mmensaje !="")
            {
                ViewBag.MensajeF = mmensaje;
            }
            else
            {
                ViewBag.MensajeF = "Orden de producción creada";
            }
           


            return RedirectToAction("InventarioStock", new { IDAlmacen = IDAlmacen, MensajeRespuesta= ViewBag.MensajeF });

        }
        public bool siTermoencongibleTrans(int cotizacion)
        {
            ClsCotizador elemento = new ClsCotizador();
            elemento = null;

            Cotizaciones archivocotizacion = new ArchivoCotizadorContext().cotizaciones.Find(cotizacion);
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

            bool tran = false;
            if (elemento.Tintas.Count() == 0 && elemento.mangatermo)
            {

                tran = true;
            }
            return tran;

        }
        public bool siAdherible(int cotizacion)
        {
            ClsCotizador elemento = new ClsCotizador();
            elemento = null;

            Cotizaciones archivocotizacion = new ArchivoCotizadorContext().cotizaciones.Find(cotizacion);
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


            bool tipoetiquetaAdhe = false;
            if (!elemento.mangatermo)
            {
                if (elemento.IDSuaje == 0)
                {
                    if (elemento.SuajeNuevo)
                    {
                        tipoetiquetaAdhe = true;
                    }
                    else
                    {

                    }
                }
                else
                {
                    tipoetiquetaAdhe = true;
                }


            }
            return tipoetiquetaAdhe;



        }

        public ActionResult Solicitar(int IDAlmacen, int IDArticulo, int IDCaracteristica, decimal Cantidad)
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

                //DetSolicitud solicitud = db.Database.SqlQuery<DetSolicitud>("Select * from Detsolicitud where IDDetSolicitud=" + id).ToList().FirstOrDefault();

                Proveedor proveedordesconocido = new ProveedorContext().Proveedores.Find(SIAAPI.Properties.Settings.Default.IDProveedorDesconocido);
                Articulo articuloaproducion = new ArticuloContext().Articulo.Find(IDArticulo);
                Caracteristica caracteristica = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from Caracteristica where id=" + IDCaracteristica).FirstOrDefault();

                //      cadmen =  cadmen +" provee desc " + proveedordesconocido +;
                EncRequisiciones requisicion = new EncRequisiciones();
                requisicion.IDRequisicion = num;
                requisicion.Fecha = DateTime.Now;
                requisicion.FechaRequiere = DateTime.Now;
                requisicion.IDProveedor = proveedordesconocido.IDProveedor;
                requisicion.IDFormapago = proveedordesconocido.IDFormaPago;
                //proveedordesconocido.IDMoneda;
                requisicion.Observacion = "Stock";
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
                    decimal costo = db.Database.SqlQuery<ClsDatoDecimal>("select dbo.[GetCosto](0," + IDArticulo + "," + Cantidad + ") as Dato").ToList().FirstOrDefault().Dato;

                    decimal subtotal = Math.Round(costo * Cantidad);
                    decimal iva = Math.Round(subtotal * Decimal.Parse(SIAAPI.Properties.Settings.Default.ValorIVA), 2);
                    decimal total = subtotal + iva;
                    Articulo articulo = new ArticuloContext().Articulo.Find(IDArticulo);
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

                    StringBuilder cadena = new StringBuilder();

                    cadena.Append("INSERT INTO DetRequisiciones ");
                    cadena.Append("(IDRequisicion,IDArticulo,Cantidad,Descuento,costo,Importe,CantidadPedida,ImporteIva,IVA,ImporteTotal,Nota,Ordenado,Caracteristica_ID,IDAlmacen,Suministro,status,Presentacion,jsonPresentacion) values (");
                    cadena.Append(num + ",");
                    cadena.Append(IDArticulo + ",");
                    cadena.Append(Cantidad + ",");
                    cadena.Append("0,");
                    cadena.Append(costo + ",");
                    cadena.Append(subtotal + ","); //importe
                    cadena.Append(Cantidad + ","); //cantidadpedida
                    cadena.Append(iva + ","); //iva
                    cadena.Append("'1',");
                    cadena.Append(total + ",");
                    cadena.Append("'',");
                    cadena.Append("'0',"); //ordenado                  
                    cadena.Append(IDCaracteristica + ",");
                    cadena.Append(IDAlmacen + ",");
                    cadena.Append("0,"); //suministro
                    cadena.Append("'Activo',"); //status
                    cadena.Append("'" + caracteristica.Presentacion + "',");
                    cadena.Append("'{" + caracteristica.Presentacion + "}')");

                    try
                    {
                        db.Database.ExecuteSqlCommand(cadena.ToString());

                        decimal existencia = 0;
                        InventarioAlmacen inv = new InventarioAlmacenContext().InventarioAlmacenes.ToList().Where(s => s.IDAlmacen == IDAlmacen && s.IDCaracteristica == IDCaracteristica).FirstOrDefault();
                        Caracteristica cara = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from Caracteristica where id=" + IDCaracteristica).FirstOrDefault();
                        if (inv != null)
                        {
                            //- No hace nada por que las requisicion no afectan en nada

                            existencia = inv.Existencia;
                        }
                        else
                        {
                            db.Database.ExecuteSqlCommand("INSERT INTO inventarioAlmacen (IDAlmacen,IDArticulo,IDCaracteristica,Existencia, PorLlegar,Apartado,Disponibilidad) values ("
                                + IDAlmacen + "," + cara.Articulo_IDArticulo + "," + IDCaracteristica + ",0,0,0,0))");
                            existencia = 0;
                        }

                        string moviartcadena = "Insert into MovimientoArticulo (fecha,id, idPresentacion, Articulo_IDArticulo, Accion, Cantidad, Documento, Nodocumento, Lote, IDAlmacen,TipoOperacion, Acumulado, Observacion, Hora) ";
                        moviartcadena += "values (getdate()," + IDCaracteristica + "," + cara.IDPresentacion + "," + cara.Articulo_IDArticulo + ",'Requisicion para pedido', " + Cantidad + ",'Requisción'," + num + ",''," + IDAlmacen + ",'N/A'," + existencia + ",'Falta de stock',CONVERT (time, SYSDATETIMEOFFSET())) ";

                        db.Database.ExecuteSqlCommand(moviartcadena);
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

                //db.Database.ExecuteSqlCommand("update detsolicitud set status='Requerido', CantidadPedida=" + Cantidad + ", DocumentoR='Requisicion', NumeroDR=" + num + " where IDDetsolicitud =" + id);
                return RedirectToAction("Index", "EncRequisiciones");
                //return RedirectToAction("InventarioStock", "InventarioAlmacen", new { IDAlmacen = IDAlmacen });
            }
            catch (Exception err)
            {
                string mensaje = err.Message;
                ViewBag.Mensaje = mensaje;
                Session["Mensaje"] = ViewBag.Mensaje;
                return RedirectToAction("InventarioStock", "InventarioAlmacen", new { IDAlmacen = IDAlmacen });

            }
        }

        public bool siTermoencongible(int cotizacion)
        {
            ClsCotizador elemento = new ClsCotizador();
            elemento = null;

            Cotizaciones archivocotizacion = new ArchivoCotizadorContext().cotizaciones.Find(cotizacion);
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

            return elemento.mangatermo;
        }
        public string CrearOrden(int cotizacion, decimal cantidad, int ordenproduccion, decimal TC, int IDMoneda, int modelo) // cfreamos mensajes que devuelve la rutina
        {


            string mensajeerror = string.Empty;

            ClsCotizador elemento = new ClsCotizador();
            Cotizaciones archivocotizacion = new ArchivoCotizadorContext().cotizaciones.Find(cotizacion);
            XmlDocument documento = new XmlDocument();
            string nombredearchivo = archivocotizacion.Ruta.TrimEnd() + "\\" + archivocotizacion.NombreArchivo + ".xml";
            documento.Load(nombredearchivo);
            elemento = null;
            XmlSerializer serializerX = new XmlSerializer(typeof(ClsCotizador));
            using (Stream reader = new FileStream(nombredearchivo, FileMode.Open))
            {
                // Call the Deserialize method to restore the object's state.
                elemento = (ClsCotizador)serializerX.Deserialize(reader);
            }
            elemento.Cantidad = cantidad;

            FormulaEspecializada.Formulaespecializada formula = new FormulaEspecializada.Formulaespecializada();
            formula = igualar(elemento, formula);

            formula.Calcular();

            elemento.CantidadMPMts2 = formula.CantidadMPMts2;
            elemento.anchomaterialenmm = formula.anchomaterialenmm;
            elemento.largomaterialenMts = formula.largomaterialenMts;
            elemento.CintasMaster = formula.CintasMaster;
            elemento.Numerodecintas = formula.Numerodecintas;
            elemento.MtsdeMerma = formula.MtsdeMerma;
            elemento.CostototalMP = formula.CostototalMP;

            elemento.HrPrensa = formula.getHoraPrensa();

            if (elemento.mangatermo)
            {
                elemento.HrSellado = formula.HrSellado;
                elemento.HrInspeccion = formula.HrInspeccion;
                elemento.HrCorte = formula.HrCorte;
            }
            else
            {
                elemento.HrEmbobinado = formula.getHoraEmbobinado();
            }


            Plantilla articulosdeplantilla = null;



            if (modelo == 4)
            {
                try
                {
                    articulosdeplantilla = Modelo4(elemento);
                }
                catch (Exception err)
                {
                    string mensajeerror1 = err.Message;
                }




            }

            if (modelo == 8)
            {
                articulosdeplantilla = Modelo8(elemento);

            }

            foreach (ArticuloXML artipro in articulosdeplantilla.Articulos)
            {


                FormulaSiaapi.Formulas formulaparasacarinfo = new FormulaSiaapi.Formulas();

                string forMU = artipro.Formula;


                ArticuloContext basa = new ArticuloContext();
                int idar = int.Parse(artipro.IDArticulo);
                Articulo articulo = basa.Articulo.Find(idar);

                Caracteristica caraarticulo = basa.Database.SqlQuery<Caracteristica>("select * from caracteristica where id=" + artipro.IDCaracteristica).FirstOrDefault();

                string formulanueva = forMU;
                try
                {
                    formulanueva = formulaparasacarinfo.sustituircontenidocadena(forMU, double.Parse(cantidad.ToString()));
                }
                catch (Exception err)
                {

                }
                if (artipro.FactorCierre == null)
                { artipro.FactorCierre = "0"; }
                double valorfin = formulaparasacarinfo.Calcular(formulanueva, double.Parse(artipro.FactorCierre.ToString()));

                //////////////////////checa el costo ///////////////////////

                double costo = 0;
                double costounitario = 0;
                try
                {
                    ClsDatoDecimal cuanto = new CobroContext().Database.SqlQuery<ClsDatoDecimal>("SELECT [dbo].[GetCosto] (0," + articulo.IDArticulo + "," + valorfin + ") as Dato ").ToList()[0];
                    costo = double.Parse(cuanto.Dato.ToString());
                }
                catch (Exception err)
                {
                    mensajeerror += err.Message;
                }

                string clavemonedacotizar = new c_MonedaContext().c_Monedas.Find(articulo.IDMoneda).ClaveMoneda;


                decimal tcc = TC;
                if (articulo.IDMoneda == IDMoneda)
                {
                    tcc = 1;
                }
                costounitario = costo;
                if (valorfin >= 0)
                {
                    costo = costo * valorfin;
                }


                decimal costofinal = 0;



                if (articulo.c_Moneda.ClaveMoneda == "MXN" && clavemonedacotizar == "USD")
                {
                    costofinal = (decimal)costo / tcc;
                }
                if (articulo.c_Moneda.ClaveMoneda == "USD" && clavemonedacotizar == "MXN")
                {
                    costofinal = (decimal)costo * tcc;
                }
                if (articulo.c_Moneda.ClaveMoneda == clavemonedacotizar)
                {
                    costofinal = (decimal)costo;
                }

                //  new CobroContext().Database.ExecuteSqlCommand("INSERT INTO [dbo].[RangoPlaneacionArticulo] ([IDHE],[RangoInf],[RangoSup],[Costo],[Version],[IDArticulo],[IDTipoArticulo],[IDMoneda],[IDProceso],[TC],cantidad) VALUES (" + hoja.IDHE + "," + rango.RangoInf + "," + rango.RangoSup + "," + costofinal + "," + hoja.Version + "," + articulo.IDArticulo + "," + articulo.IDTipoArticulo + "," + IDMoneda + "," + articulop.IDProceso + "," + tcc + "," + valorfin + ")");

                //Solicitando LDM
                costofinal = Math.Round(costofinal, 2);

                if ((articulo.IDTipoArticulo == 1) || (articulo.IDTipoArticulo == 4) || (articulo.IDTipoArticulo == 6) || (articulo.IDTipoArticulo == 7))
                {
                    StringBuilder cadenasolcitud = new StringBuilder();

                    cadenasolcitud.Append("insert into [DetSolicitud]([IDArticulo],[Caracteristica_ID],[IDAlmacen],[Documento],[Numero],[Cantidad],[Costo],[CantidadPedida],");
                    cadenasolcitud.Append("[Descuento],[Importe],[IVA],[ImporteIva],[ImporteTotal],[Nota],[Requerido],[Suministro],[Status],[DocumentoR],[NumeroDR],[Presentacion],[jsonPresentacion]) values (");


                    cadenasolcitud.Append(artipro.IDArticulo + ",");
                    cadenasolcitud.Append(artipro.IDCaracteristica + ",");
                    Caracteristica caracterisp = null;
                    try
                    {
                        caracterisp = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from Caracteristica where ID=" + artipro.IDCaracteristica).ToList().FirstOrDefault();
                    }
                    catch (Exception err)
                    {
                        string mensajeerro = err.Message;
                    }



                    switch (articulo.IDTipoArticulo)   /// de acurdo altipo de articulo es al almacen 
                    {
                        case 6:
                            cadenasolcitud.Append("6,"); // 
                            break;
                        case 7:
                            cadenasolcitud.Append("1,");
                            break;
                        default:
                            cadenasolcitud.Append("2,");
                            break;

                    }

                    cadenasolcitud.Append("'Orden de  Produccion',");
                    cadenasolcitud.Append(ordenproduccion + ",");
                    cadenasolcitud.Append(valorfin + ","); //PEDIDO
                    cadenasolcitud.Append(Math.Round((costofinal / decimal.Parse(valorfin.ToString())), 2) + ",");
                    cadenasolcitud.Append("0,");
                    cadenasolcitud.Append("0,"); //descuento
                    cadenasolcitud.Append(costofinal + ",");

                    cadenasolcitud.Append("'1',");
                    decimal ivaimp = Math.Round(costofinal * decimal.Parse(SIAAPI.Properties.Settings.Default.ValorIVA), 2);
                    cadenasolcitud.Append(ivaimp + ",");
                    cadenasolcitud.Append(costofinal + ivaimp + ",");
                    cadenasolcitud.Append("'',");
                    cadenasolcitud.Append("'0',");
                    cadenasolcitud.Append("0,"); //suministro
                    cadenasolcitud.Append("'Solicitado',"); //status
                    cadenasolcitud.Append("'',"); //documento
                    cadenasolcitud.Append("0,");
                    cadenasolcitud.Append("'" + caracterisp.Presentacion + "',");
                    cadenasolcitud.Append("'')");




                    try
                    {
                        db.Database.ExecuteSqlCommand(cadenasolcitud.ToString());
                    }
                    catch (Exception err)
                    {
                        string mensaje = err.Message;
                    }

                }

                try
                {
                    string existe = "'0'";
                    if ((articulo.IDTipoArticulo == 3 || articulo.IDTipoArticulo == 5) || (artipro.IDProceso == "3" || artipro.IDProceso == "7" || artipro.IDProceso == "10" || artipro.IDProceso == "11" || artipro.IDProceso == "12" || artipro.IDProceso == "16" || artipro.IDProceso == "4"))
                    {
                        existe = "'1'";
                    }
                    db.Database.ExecuteSqlCommand("INSERT INTO [dbo].[ArticuloProduccion]([IDHE],[IDArticulo],[IDTipoArticulo],[IDCaracteristica],[IDProceso],[IDOrden],[Cantidad],[IDClaveUnidad],[Indicaciones],[CostoPlaneado],[CostoReal],TC,TCR,[Existe]) VALUES('" + caraarticulo.IDCotizacion + "','" + articulo.IDArticulo + "','" + articulo.IDTipoArticulo + "','" + artipro.IDCaracteristica + "','" + artipro.IDProceso + "','" + ordenproduccion + "'," + valorfin + ",'" + articulo.IDClaveUnidad + "','" + artipro.Indicaciones + "'," + costofinal + ",0," + tcc + ",0," + existe + ")");
                    /////   aqui vamos a traer el costo del articulo /////

                }
                catch (Exception err)
                {
                    string mensaje = err.Message;
                }
            }
            // fin del for


            return mensajeerror;


        }


        public ActionResult InventarioArtA()
        {


            List<ReporteInventarioAlm> almacen = new List<ReporteInventarioAlm>();
            string cadena1 = "select IDAlmacen, CodAlm, Descripcion from dbo.Almacen order by Descripcion";
            almacen = db.Database.SqlQuery<ReporteInventarioAlm>(cadena1).ToList();
            List<SelectListItem> listaalmacen = new List<SelectListItem>();
            listaalmacen.Add(new SelectListItem { Text = "--Todos los almacenes --", Value = "0" });
            foreach (var m in almacen)
            {
                listaalmacen.Add(new SelectListItem { Text = m.Descripcion, Value = m.IDAlmacen.ToString() });
            }
            ViewBag.idalmacen = listaalmacen;

            List<ReporteInventarioFam> familia = new List<ReporteInventarioFam>();
            string cadena2 = "select IDFamilia, CCodFam, Descripcion from [dbo].[Familia]";
            familia = db.Database.SqlQuery<ReporteInventarioFam>(cadena2).ToList();
            List<SelectListItem> listafamilia = new List<SelectListItem>();
            listafamilia.Add(new SelectListItem { Text = "--Todas las familias--", Value = "0" });
            foreach (var m in familia)
            {
                listafamilia.Add(new SelectListItem { Text = m.Descripcion, Value = m.IDFamilia.ToString() });
            }
            ViewBag.idfamilia = listafamilia;

            return View();
        }

        public Plantilla Modelo4(ClsCotizador elemento)
        {

            Plantilla plantilla = new Plantilla();


            ArticuloXML nuevomanodeobra = new ArticuloXML();

            nuevomanodeobra.IDArticulo = "2916";
            nuevomanodeobra.IDCaracteristica = "3972";
            nuevomanodeobra.Formula = elemento.HrPrensa.ToString();


            nuevomanodeobra.IDTipoArticulo = "5";
            nuevomanodeobra.IDProceso = "5";
            nuevomanodeobra.FactorCierre = "0.166667";



            plantilla.Articulos.Add(nuevomanodeobra);


            /********************************* maquina de obra *********************************/

            if (elemento.IDMaquinaPrensa == 0)
            {

                ArticuloXML nuevomaquina = new ArticuloXML();

                nuevomaquina.IDArticulo = "87";
                nuevomaquina.IDCaracteristica = "32";


                nuevomaquina.Formula = elemento.HrPrensa.ToString();


                nuevomaquina.IDTipoArticulo = "3";
                nuevomaquina.IDProceso = "5";
                nuevomaquina.FactorCierre = "0.16667";

                plantilla.Articulos.Add(nuevomaquina);
            }

            else

            {
                Caracteristica cara = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from Caracteristica where Articulo_IDArticulo=" + elemento.IDMaquinaPrensa).FirstOrDefault();
                ArticuloXML nuevomaquina = new ArticuloXML();

                nuevomaquina.IDArticulo = elemento.IDMaquinaPrensa.ToString();
                nuevomaquina.IDCaracteristica = cara.ID.ToString();


                nuevomaquina.Formula = elemento.HrPrensa.ToString();

                nuevomaquina.IDTipoArticulo = "3";
                nuevomaquina.IDProceso = "5";
                nuevomaquina.FactorCierre = "0.25";

                plantilla.Articulos.Add(nuevomaquina);


            }

            /***************************************  suaje ****************************/

            if (elemento.IDSuaje == 0)
            {
                ArticuloXML nuevoplaneacion = new ArticuloXML();

                nuevoplaneacion.IDArticulo = "8674";
                nuevoplaneacion.IDCaracteristica = "9763";
                nuevoplaneacion.IDTipoArticulo = "2";
                nuevoplaneacion.IDProceso = "5";
                nuevoplaneacion.Formula = "1/50";
                nuevoplaneacion.Indicaciones = "Esperando Suaje";

                plantilla.Articulos.Add(nuevoplaneacion);
            }


            if (elemento.IDSuaje > 0)
            {
                ArticuloXML nuevoplaneacion = new ArticuloXML();

                Caracteristica CIDSuaje = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from Caracteristica where ID=" + elemento.IDSuaje).ToList().FirstOrDefault();

                int IDSuaje = CIDSuaje.Articulo_IDArticulo;

                nuevoplaneacion.IDArticulo = IDSuaje.ToString();
                nuevoplaneacion.IDCaracteristica = elemento.IDSuaje.ToString();
                nuevoplaneacion.IDTipoArticulo = "2";
                nuevoplaneacion.IDProceso = "5";
                nuevoplaneacion.Formula = "1/50";
                nuevoplaneacion.Indicaciones = "";

                plantilla.Articulos.Add(nuevoplaneacion);
            }
            if (elemento.IDSuaje2 > 0)
            {
                ArticuloXML nuevoplaneacion3 = new ArticuloXML();

                Caracteristica CIDSuaje = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from Caracteristica where ID=" + elemento.IDSuaje2).ToList().FirstOrDefault();

                int IDSuaje = CIDSuaje.Articulo_IDArticulo;

                nuevoplaneacion3.IDArticulo = IDSuaje.ToString();
                nuevoplaneacion3.IDCaracteristica = elemento.IDSuaje.ToString();
                nuevoplaneacion3.IDTipoArticulo = "2";
                nuevoplaneacion3.IDProceso = "5";
                nuevoplaneacion3.Formula = "1/50";
                nuevoplaneacion3.Indicaciones = "";

                plantilla.Articulos.Add(nuevoplaneacion3);
            }


            Materiales material = new MaterialesContext().Materiales.Find(elemento.IDMaterial);


            int IDMaterial = verificamaterial(material.Clave, elemento.anchomaterialenmm, material.Largo, material.Fam, material.Descripcion, material.Precio);

            ArticuloXML nuevoplaneacion2 = new ArticuloXML();

            int IDMaterialArticulo = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from Caracteristica where ID=" + IDMaterial).ToList().FirstOrDefault().Articulo_IDArticulo;

            nuevoplaneacion2.IDArticulo = IDMaterialArticulo.ToString();
            nuevoplaneacion2.IDCaracteristica = IDMaterial.ToString();
            nuevoplaneacion2.IDTipoArticulo = "6";
            nuevoplaneacion2.IDProceso = "5";
            nuevoplaneacion2.Formula = elemento.CantidadMPMts2.ToString();
            nuevoplaneacion2.FactorCierre = "0";
            nuevoplaneacion2.Indicaciones = "";


            plantilla.Articulos.Add(nuevoplaneacion2);


            // *************************

            if (elemento.IDMaterial2 > 0)
            {

                ArticuloXML nuevoplaneacion3 = new ArticuloXML();


                Materiales material2 = new MaterialesContext().Materiales.Find(elemento.IDMaterial2);
                int IDMaterial2 = verificamaterial(material2.Clave, elemento.anchomaterialenmm, material2.Largo, material2.Fam, material2.Descripcion, material2.Precio);

                int IDMaterialArticulo2 = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from Caracteristica where ID=" + IDMaterial2).ToList().FirstOrDefault().Articulo_IDArticulo;

                nuevoplaneacion3.IDArticulo = IDMaterialArticulo2.ToString();
                nuevoplaneacion3.IDCaracteristica = IDMaterial2.ToString();
                nuevoplaneacion3.IDTipoArticulo = "6";
                nuevoplaneacion3.IDProceso = "5";

                nuevoplaneacion3.Formula = elemento.CantidadMPMts2.ToString();


                nuevoplaneacion3.Indicaciones = "";


                plantilla.Articulos.Add(nuevoplaneacion3);
            }



            //////////////////////////////////////////////////////////tintas //////////////////////
            foreach (Tinta tinta in elemento.Tintas)
            {
                ArticuloXML nuevoplaneaciontinta = new ArticuloXML();

                int IDTinta = tinta.IDTinta;



                nuevoplaneaciontinta.IDArticulo = IDTinta.ToString();

                try
                {
                    int IDMpresentaciontin = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from Caracteristica where Articulo_IDArticulo=" + IDTinta).ToList().FirstOrDefault().ID;

                    nuevoplaneaciontinta.IDCaracteristica = IDMpresentaciontin.ToString();
                    nuevoplaneaciontinta.IDTipoArticulo = "6";
                    nuevoplaneaciontinta.IDProceso = "5";

                    decimal cantidadmillartinta = (elemento.Cantidad / elemento.CantidadMPMts2);



                    nuevoplaneaciontinta.Formula = "" + elemento.CantidadMPMts2 + " / 300";

                    nuevoplaneaciontinta.FactorCierre = "0.125";

                    nuevoplaneaciontinta.Indicaciones = "";


                    plantilla.Articulos.Add(nuevoplaneaciontinta);
                }

                catch (Exception err)
                {
                    string mensajedeerror = err.Message;
                }


            }



            /**** proceso embobinado *******************/
            ArticuloXML nuevotiempoembobinado = new ArticuloXML();



            nuevotiempoembobinado.IDArticulo = 226.ToString();
            nuevotiempoembobinado.IDCaracteristica = 215.ToString();
            nuevotiempoembobinado.IDTipoArticulo = "5";
            nuevotiempoembobinado.IDProceso = "4";
            nuevotiempoembobinado.Formula = elemento.HrEmbobinado.ToString();
            nuevotiempoembobinado.FactorCierre = "0.1666";
            nuevotiempoembobinado.Indicaciones = "";


            plantilla.Articulos.Add(nuevotiempoembobinado);


            if (elemento.IDMaquinaEmbobinado == 0)
            {

                ArticuloXML nuevomaquinaenbobinado = new ArticuloXML();

                nuevomaquinaenbobinado.IDArticulo = "91";
                nuevomaquinaenbobinado.IDCaracteristica = "3970";


                nuevomaquinaenbobinado.Formula = elemento.HrEmbobinado.ToString();


                nuevomaquinaenbobinado.IDTipoArticulo = "3";
                nuevomaquinaenbobinado.IDProceso = "4";
                nuevomaquinaenbobinado.FactorCierre = "0.25";

                plantilla.Articulos.Add(nuevomaquinaenbobinado);
            }
            else
            {
                Caracteristica cara = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from Caracteristica where Articulo_IDArticulo=" + elemento.IDMaquinaEmbobinado).FirstOrDefault();
                ArticuloXML nuevomaquina = new ArticuloXML();

                nuevomaquina.IDArticulo = elemento.IDMaquinaEmbobinado.ToString();
                nuevomaquina.IDCaracteristica = cara.ID.ToString();


                nuevomaquina.Formula = elemento.HrEmbobinado.ToString();

                nuevomaquina.IDTipoArticulo = "3";
                nuevomaquina.IDProceso = "4";
                nuevomaquina.FactorCierre = "0.25";

                plantilla.Articulos.Add(nuevomaquina);
            }


            if (elemento.IDCentro > 0)
            {
                ArticuloXML centros = new ArticuloXML();
                Caracteristica carac = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from Caracteristica where Articulo_IDArticulo=" + elemento.IDCentro).FirstOrDefault();
                centros.IDArticulo = elemento.IDCentro.ToString();

                centros.IDCaracteristica = carac.ID.ToString();

                centros.IDTipoArticulo = "4";
                centros.IDProceso = "4";
                decimal numeroderollos = (elemento.Cantidad * 1000) / elemento.Cantidadxrollo;
                decimal cuantoscaven = 1000M / (elemento.anchoproductomm * elemento.productosalpaso);
                decimal numerodecentros = (numeroderollos / Math.Truncate(cuantoscaven));
                centros.Formula = numerodecentros.ToString(); /// la cantidad de centros entre la cantidad de centros que caben en una pieza de rollo completa)
                centros.FactorCierre = "1";
                centros.Indicaciones = "";

                plantilla.Articulos.Add(centros);

            }

            if (elemento.IDCaja > 0)
            {
                ArticuloXML cajas = new ArticuloXML();



                cajas.IDArticulo = elemento.IDCaja.ToString();

                Caracteristica caracaja = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from Caracteristica where Articulo_IDArticulo=" + elemento.IDCaja).FirstOrDefault();
                cajas.IDCaracteristica = caracaja.ID.ToString();
                cajas.IDTipoArticulo = "4";
                cajas.IDProceso = "4";
                int numcajas = int.Parse(Math.Truncate(((elemento.Cantidad / elemento.Minimoproducir) * 0.75M) + 0.5M).ToString());
                cajas.Formula = numcajas.ToString();
                cajas.FactorCierre = "1";
                cajas.Indicaciones = "";

                plantilla.Articulos.Add(cajas);
            }



            return plantilla;
        }
        public int verificamaterial(string clave, int ancho, int largo, string fam, string Material, decimal Costo)
        {
            ArticuloContext db = new ArticuloContext();
            string clavemat = clave + "-" + ancho;

            int IDArticulo = 0;

            List<Articulo> arti = db.Database.SqlQuery<Articulo>("select * from Articulo where cref='" + clavemat + "'").ToList();

            if (arti.Count > 0)
            {
                IDArticulo = arti.FirstOrDefault().IDArticulo;

            }
            else /// no existe el articulo
            {
                string cadenax = "select * from Familia where ccodfam='" + fam + "'";
                int IDFam = db.Database.SqlQuery<Familia>(cadenax).ToList().FirstOrDefault().IDFamilia;
                if (IDFam == 0)
                {
                    IDFam = 56;
                }


                Articulo NuevoArticulo = new Articulo();

                NuevoArticulo.Cref = clavemat;
                NuevoArticulo.IDAQL = 0;
                NuevoArticulo.IDClaveUnidad = 57;
                NuevoArticulo.IDInspeccion = 0;
                NuevoArticulo.IDFamilia = IDFam;
                NuevoArticulo.IDInspeccion = 0;
                NuevoArticulo.IDMoneda = 181;
                NuevoArticulo.IDMuestreo = 0;
                NuevoArticulo.IDTipoArticulo = 6;
                NuevoArticulo.ManejoCar = false;
                NuevoArticulo.nameFoto = "";
                NuevoArticulo.CtrlStock = true;
                NuevoArticulo.GeneraOrden = false;
                NuevoArticulo.esKit = false;
                NuevoArticulo.Descripcion = Material + " " + ancho + " MM X " + largo + " MTS";
                NuevoArticulo.ExistenDev = false;
                NuevoArticulo.obsoleto = false;
                NuevoArticulo.bCodigodebarra = true;


                db.Articulo.Add(NuevoArticulo);
                db.SaveChanges();




                IDArticulo = db.Database.SqlQuery<Articulo>("select * from Articulo where cref='" + clavemat + "'").ToList().FirstOrDefault().IDArticulo;
                try
                {
                    db.Database.ExecuteSqlCommand("delete from MatrizCosto  where IDArticulo=" + IDArticulo);
                    db.Database.ExecuteSqlCommand("insert into MatrizCosto ( IDArticulo, Ranginf, RangSup, Precio ) values (" + IDArticulo + ",0,999999," + Costo + ")");

                    //db.Database.ExecuteSqlCommand("delete from MatrizPrecio  where IDArticulo=" + IDArticulo);
                    //db.Database.ExecuteSqlCommand("insert into MatrizPrecio ( IDArticulo, Ranginf, RangSup, Precio ) values (" + IDArticulo + ",0,999999," + Costo * 2 + ")");
                }
                catch (Exception err)
                {
                    string Mensaje = err.Message;
                }



            }

            List<Caracteristica> Carac = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from Caracteristica where Articulo_IDArticulo=" + IDArticulo + " and Presentacion like '%ANCHO:%" + ancho + ",LARGO:%" + largo + "%'").ToList();

            if (Carac.Count > 0) // existe la presntacion
            {
                return Carac.FirstOrDefault().ID; // retorna el id de la presentacion existente
            }
            else
            {
                string Presentacion = "ANCHO:" + ancho + ",LARGO:" + largo;
                string jsonPresentacion = "{" + Presentacion + "}";

                int NewIDP = db.Database.SqlQuery<int>("SELECT ISNULL(MAX(IDPRESENTACION)+1,0) from Caracteristica where Articulo_IDArticulo = " + IDArticulo + " ").FirstOrDefault();
                //int NewIDP = Convert.ToInt32(db.Database.SqlQuery("Select MAX(IDPresentacion)+1 from Caracteristica where Articulo_IDArticulo = ", pre.Articulo_IDArticulo));
                NewIDP = NewIDP > 0 ? NewIDP : 1;
                db.Database.ExecuteSqlCommand("insert into Caracteristica ( IDPresentacion, Cotizacion, version, Presentacion, jsonPresentacion, Articulo_IDArticulo )  values (" + NewIDP + ",0,0,'" + Presentacion + "','" + jsonPresentacion + "'," + IDArticulo + ")");

                int retornar = 0;
                retornar = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from Caracteristica where Articulo_IDArticulo=" + IDArticulo + " and IDPresentacion=" + NewIDP).ToList().FirstOrDefault().ID;
                try
                {
                    db.Database.ExecuteSqlCommand("insert into inventarioAlmacen ( IDAlmacen, IDArticulo, IDCaracteristica, Existencia, PorLlegar,Apartado, Disponibilidad )  values (6," + IDArticulo + "," + retornar + ",0,0,0,0)");
                }
                catch (Exception err)
                {
                    string mensaje = err.Message;
                }
                return retornar;

            }




        }

        public Plantilla Modelo8(ClsCotizador elemento)
        {

            Plantilla plantilla = new Plantilla();

            ArticuloXML nuevomanodeobra = new ArticuloXML();

            nuevomanodeobra.IDArticulo = "2916";
            nuevomanodeobra.IDCaracteristica = "3972";
            nuevomanodeobra.Formula = elemento.HrPrensa.ToString();
            nuevomanodeobra.IDTipoArticulo = "5";
            nuevomanodeobra.IDProceso = "5";
            nuevomanodeobra.FactorCierre = "0.16667";



            plantilla.Articulos.Add(nuevomanodeobra);


            /*********************************   maquina *********************************/

            if (elemento.IDMaquinaPrensa == 0)
            {


                ArticuloXML nuevomaquina = new ArticuloXML();

                nuevomaquina.IDArticulo = "3558";  // nilpeter uv
                nuevomaquina.IDCaracteristica = "4504";


                nuevomaquina.Formula = elemento.HrPrensa.ToString();


                nuevomaquina.IDTipoArticulo = "3";
                nuevomaquina.IDProceso = "5";
                nuevomaquina.FactorCierre = "0.166667";

                plantilla.Articulos.Add(nuevomaquina);

            }
            else
            {

                Caracteristica cara = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from Caracteristica where Articulo_IDArticulo=" + elemento.IDMaquinaPrensa).FirstOrDefault();
                ArticuloXML nuevomaquina = new ArticuloXML();

                nuevomaquina.IDArticulo = elemento.IDMaquinaPrensa.ToString();
                nuevomaquina.IDCaracteristica = cara.ID.ToString();


                nuevomaquina.Formula = elemento.HrPrensa.ToString();

                nuevomaquina.IDTipoArticulo = "3";
                nuevomaquina.IDProceso = "5";
                nuevomaquina.FactorCierre = "0.25";

                plantilla.Articulos.Add(nuevomaquina);

            }

            /***************************************  suaje ****************************/
            try
            {
                ArticuloXML nuevosuaje = new ArticuloXML();



                nuevosuaje.IDArticulo = "8674";
                nuevosuaje.IDCaracteristica = "9763";
                nuevosuaje.IDTipoArticulo = "2";
                nuevosuaje.IDProceso = "5";
                nuevosuaje.Formula = "1/" + elemento.DiluirSuajeEnPedidos;
                nuevosuaje.Indicaciones = "";


                plantilla.Articulos.Add(nuevosuaje);
            }
            catch (Exception err)
            {
                string mensajedeerro = err.Message;

            }

            Materiales material = new MaterialesContext().Materiales.Find(elemento.IDMaterial);


            int IDMaterial = verificamaterial(material.Clave, elemento.anchomaterialenmm, material.Largo, material.Fam, material.Descripcion, material.Precio);

            ArticuloXML nuevoplaneacion2 = new ArticuloXML();

            int IDMaterialArticulo = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from Caracteristica where id=" + IDMaterial).FirstOrDefault().Articulo_IDArticulo;

            nuevoplaneacion2.IDArticulo = IDMaterialArticulo.ToString();
            nuevoplaneacion2.IDCaracteristica = IDMaterial.ToString();
            nuevoplaneacion2.IDTipoArticulo = "6";
            nuevoplaneacion2.IDProceso = "5";
            nuevoplaneacion2.Formula = elemento.CantidadMPMts2.ToString();

            nuevoplaneacion2.Indicaciones = "";


            plantilla.Articulos.Add(nuevoplaneacion2);

            
            foreach (Tinta tinta in elemento.Tintas)
            {
                ArticuloXML nuevoplaneaciontinta = new ArticuloXML();

                int IDTinta = tinta.IDTinta;

                int IDMpresentaciontin = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from Caracteristica where Articulo_IDArticulo=" + IDTinta).ToList().FirstOrDefault().ID;

                nuevoplaneaciontinta.IDArticulo = IDTinta.ToString();
                nuevoplaneaciontinta.IDCaracteristica = IDMpresentaciontin.ToString();
                nuevoplaneaciontinta.IDTipoArticulo = "6";
                nuevoplaneaciontinta.IDProceso = "5";




                nuevoplaneaciontinta.Formula = "(" + elemento.CantidadMPMts2 + "*" + (tinta.Area / 100M) + ")/300";

                nuevoplaneaciontinta.FactorCierre = "0.25";

                nuevoplaneaciontinta.Indicaciones = "";


                plantilla.Articulos.Add(nuevoplaneaciontinta);


            }


            /**** proceso sellado *******************/

            ArticuloXML nuevotiemposellado = new ArticuloXML();



            nuevotiemposellado.IDArticulo = 231.ToString();
            nuevotiemposellado.IDCaracteristica = 220.ToString();
            nuevotiemposellado.IDTipoArticulo = "5";
            nuevotiemposellado.IDProceso = "11";
            nuevotiemposellado.Formula = "C*" + Math.Round((elemento.MaterialNecesitado / 7200) / elemento.Cantidad, 6); // 7200 MTS X HRS
            nuevotiemposellado.FactorCierre = "0.15";
            nuevotiemposellado.Indicaciones = "";


            plantilla.Articulos.Add(nuevotiemposellado);


            /**** proceso inspeccion *******************/

            ArticuloXML nuevotiempoinspeccion = new ArticuloXML();



            nuevotiempoinspeccion.IDArticulo = 232.ToString();
            nuevotiempoinspeccion.IDCaracteristica = 219.ToString();
            nuevotiempoinspeccion.IDTipoArticulo = "5";
            nuevotiempoinspeccion.IDProceso = "12";
            nuevotiempoinspeccion.Formula = "(((" + elemento.Cantidad + "* " + elemento.largoproductomm + ") / 50) / 60 )+0.33";
            nuevotiempoinspeccion.FactorCierre = "0.25";
            nuevotiempoinspeccion.Indicaciones = "";


            plantilla.Articulos.Add(nuevotiempoinspeccion);

            /**** proceso inspeccion *******************/

            ArticuloXML tiempocorte = new ArticuloXML();



            tiempocorte.IDArticulo = 232.ToString();
            tiempocorte.IDCaracteristica = 219.ToString();
            tiempocorte.IDTipoArticulo = "5";
            tiempocorte.IDProceso = "12";
            tiempocorte.Formula = "  ((((" + elemento.Cantidad + " * 1000*" + elemento.largoproductomm + ")) / 13000) / 60) + (((" + elemento.Cantidad * 1000 + ") /" + elemento.Cantidadxrollo + " * 3) / 60"; // 13 MTS * MIN
            tiempocorte.FactorCierre = "0.25";
            tiempocorte.Indicaciones = "";


            plantilla.Articulos.Add(tiempocorte);


            /******* proceso de empaque ******************/

            if (elemento.IDCentro > 0)
            {
                ArticuloXML centros = new ArticuloXML();
                Caracteristica carac = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from Caracteristica where Articulo_IDArticulo=" + elemento.IDMaquinaEmbobinado).FirstOrDefault();
                centros.IDArticulo = elemento.IDCentro.ToString();

                centros.IDCaracteristica = carac.ID.ToString();

                centros.IDTipoArticulo = "4";
                centros.IDProceso = "4";
                decimal numeroderollos = (elemento.Cantidad * 1000) / elemento.Cantidadxrollo;
                decimal cuantoscaven = 1000M / (elemento.anchoproductomm * elemento.productosalpaso);
                decimal numerodecentros = (numeroderollos / Math.Truncate(cuantoscaven));
                centros.Formula = numerodecentros.ToString(); /// la cantidad de centros entre la cantidad de centros que caben en una pieza de rollo completa)
                centros.FactorCierre = "1";
                centros.Indicaciones = "";

                plantilla.Articulos.Add(centros);

            }

            if (elemento.IDCaja > 0)
            {
                ArticuloXML cajas = new ArticuloXML();



                cajas.IDArticulo = elemento.IDCaja.ToString();

                Caracteristica caracaja = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from Caracteristica where Articulo_IDArticulo=" + elemento.IDCaja).FirstOrDefault();
                cajas.IDCaracteristica = caracaja.ID.ToString();
                cajas.IDTipoArticulo = "4";
                cajas.IDProceso = "16";
                int numcajas = int.Parse(Math.Truncate(((elemento.Cantidad / elemento.Minimoproducir) * 0.75M) + 0.5M).ToString());
                cajas.Formula = numcajas.ToString();
                cajas.FactorCierre = "1";
                cajas.Indicaciones = "";

                plantilla.Articulos.Add(cajas);
            }

            return plantilla;


        }
        public FormulaEspecializada.Formulaespecializada igualar(ClsCotizador elemento, FormulaEspecializada.Formulaespecializada formula)
        {
            formula.anchoproductomm = elemento.anchoproductomm;
            formula.largoproductomm = elemento.largoproductomm;
            formula.Cantidad = elemento.Cantidad;
            formula.Cantidadxrollo = elemento.Cantidadxrollo;
            formula.gapeje = elemento.gapeje;
            formula.gapavance = elemento.gapavance;
            formula.Numerodetintas = elemento.Numerodetintas;
            formula.cavidadesdesuaje = elemento.cavidadesdesuajeEje;



            formula.conadhesivo = elemento.conadhesivo;
            formula.hotstamping = elemento.hotstamping;
            formula.CostoM2Cinta = elemento.CostoM2Cinta;
            formula.LargoCinta = elemento.LargoCinta;

            return formula;
        }

        public ActionResult InventarioArtLev()
        {


            List<ReporteInventarioAlm> almacen = new List<ReporteInventarioAlm>();
            string cadena1 = "select IDAlmacen, CodAlm, Descripcion from dbo.Almacen order by Descripcion";
            almacen = db.Database.SqlQuery<ReporteInventarioAlm>(cadena1).ToList();
            List<SelectListItem> listaalmacen = new List<SelectListItem>();
            listaalmacen.Add(new SelectListItem { Text = "--Todos los almacenes --", Value = "0" });
            foreach (var m in almacen)
            {
                listaalmacen.Add(new SelectListItem { Text = m.Descripcion, Value = m.IDAlmacen.ToString() });
            }
            ViewBag.idalmacen = listaalmacen;

            List<ReporteInventarioFam> familia = new List<ReporteInventarioFam>();
            string cadena2 = "select IDFamilia, CCodFam, Descripcion from [dbo].[Familia]";
            familia = db.Database.SqlQuery<ReporteInventarioFam>(cadena2).ToList();
            List<SelectListItem> listafamilia = new List<SelectListItem>();
            listafamilia.Add(new SelectListItem { Text = "--Todas las familias--", Value = "0" });
            foreach (var m in familia)
            {
                listafamilia.Add(new SelectListItem { Text = m.Descripcion, Value = m.IDFamilia.ToString() });
            }
            ViewBag.idfamilia = listafamilia;

            return View();
        }

        [HttpPost]
        public ActionResult InventarioArtLev(InventarioPorAlmacenLev modelo)
        {

            InventarioPorAlmacenLev report = new InventarioPorAlmacenLev();
            byte[] abytes = report.PrepareReport(modelo.idalmacen, modelo.idfamilia);
            return File(abytes, "application/pdf");
            ////return Redirect("index");
        }

        public ActionResult InventarioArt()
        {


            List<ReporteInventarioAlm> almacen = new List<ReporteInventarioAlm>();
            string cadena1 = "select IDAlmacen, CodAlm, Descripcion from dbo.Almacen order by Descripcion";
            almacen = db.Database.SqlQuery<ReporteInventarioAlm>(cadena1).ToList();
            List<SelectListItem> listaalmacen = new List<SelectListItem>();
            listaalmacen.Add(new SelectListItem { Text = "--Todos los almacenes --", Value = "0" });
            foreach (var m in almacen)
            {
                listaalmacen.Add(new SelectListItem { Text = m.Descripcion, Value = m.IDAlmacen.ToString() });
            }
            ViewBag.idalmacen = listaalmacen;

            List<ReporteInventarioFam> familia = new List<ReporteInventarioFam>();
            string cadena2 = "select IDFamilia, CCodFam, Descripcion from [dbo].[Familia]";
            familia = db.Database.SqlQuery<ReporteInventarioFam>(cadena2).ToList();
            List<SelectListItem> listafamilia = new List<SelectListItem>();
            listafamilia.Add(new SelectListItem { Text = "--Todas las familias--", Value = "0" });
            foreach (var m in familia)
            {
                listafamilia.Add(new SelectListItem { Text = m.Descripcion, Value = m.IDFamilia.ToString() });
            }
            ViewBag.idfamilia = listafamilia;

            return View();
        }

        public ActionResult ReporteCostoProm()
        {
            AlmacenContext dbc = new AlmacenContext();
            var alm = dbc.Almacenes.OrderBy(m => m.Descripcion).ToList();
            List<SelectListItem> listaAlm = new List<SelectListItem>();
            listaAlm.Add(new SelectListItem { Text = "--Todos los Almacenes--", Value = "0" });

            foreach (var m in alm)
            {
                listaAlm.Add(new SelectListItem { Text = m.Descripcion, Value = m.IDAlmacen.ToString() });
            }
            ViewBag.idalmacen = listaAlm;

            FamiliaContext dba = new FamiliaContext();
            var fam = dba.Familias.OrderBy(m => m.Descripcion).ToList();
            List<SelectListItem> listaFam = new List<SelectListItem>();
            listaFam.Add(new SelectListItem { Text = "--Todas las familias--", Value = "0" });

            foreach (var m in fam)
            {
                listaFam.Add(new SelectListItem { Text = m.Descripcion, Value = m.IDFamilia.ToString() });
            }
            ViewBag.idfamilia = listaFam;
            EncReporteAVG elemento = new EncReporteAVG();
            return View(elemento);
        }

        [HttpPost]
        public ActionResult ReporteCostoProm(EncReporteAVG elemento, FormCollection coleccion)
        {
            EncReporteAVGContext dba = new EncReporteAVGContext();
            int idalm = elemento.IDAlmacen;
            int idfam = elemento.IDFamilia;

            Deletelotealltempinv(idalm, idfam);


            string cual = coleccion.Get("Enviar");
            if (cual == "Generar reporte")
            {

                var datosrep = dba.EncReporteAVG.ToList();
                ReporteCostoPromedio report = new ReporteCostoPromedio();
                byte[] abytes = report.PrepareReport(idalm, idfam);
                return File(abytes, "applicacion/pdf", "ReporteArtCostoProm.pdf");
                //return View(elemento);
            }
            else
            {



                //Listado de datos

                string cadenaSql = "";
                EncReporteCP Encabezado = new EncReporteCP();
                decimal Cambio = 0;
                TipoCambioContext db = new TipoCambioContext();
                string fecha = DateTime.Now.ToString("yyyy/MM/dd");
                List<c_Moneda> monedaorigen = db.Database.SqlQuery<c_Moneda>("select * from c_Moneda WHERE ClaveMoneda='MXN'").ToList();
                int origen = monedaorigen.Select(s => s.IDMoneda).FirstOrDefault();
                List<c_Moneda> monedadestino = db.Database.SqlQuery<c_Moneda>("select * from c_Moneda WHERE ClaveMoneda='USD'").ToList();
                int destino = monedadestino.Select(s => s.IDMoneda).FirstOrDefault();
                DateTime fecAct = DateTime.Today;
                string FA = fecAct.ToString("dd/MM/yyyy");
                VCambio cambio = db.Database.SqlQuery<VCambio>("select dbo.GetTipocambio(convert(Datetime,'" + FA + "',103)," + destino + "," + origen + ") as TC").ToList()[0];
                Cambio = cambio.TC;



                //var costo = new VPrecioAVGContext().VPrecioAVGs.ToList();

                if (idalm == 0 && idfam == 0)
                {
                    cadenaSql = "select * from TempInvProm order by idFamilia, Cref";
                }
                if (idalm == 0 && idfam != 0)
                {
                    cadenaSql = "select * from TempInvProm where IDFamilia=" + idfam + " order by idFamilia, Cref";
                }
                if (idalm != 0 && idfam == 0)
                {
                    cadenaSql = "select * from TempInvProm where IDalmacen=" + idalm + " order by idFamilia, Cref";
                }
                if (idalm != 0 && idfam != 0)
                {
                    cadenaSql = "select * from TempInvProm where IDFamilia=" + idfam + " and IDalmacen=" + idalm + " order by idFamilia, Cref";
                }

                var costo = dba.Database.SqlQuery<TempInvProm>(cadenaSql).ToList();

                List<EncReporteCP> Listencabezado = new List<EncReporteCP>
            {
                new EncReporteCP
                {
                    Rep = Encabezado.Rep,
                    QueFamilia = Encabezado.QueFamilia,
                    QueAlmacen = Encabezado.QueAlmacen
                }

                  };


                //var costo = dba.RArtCostoProm.ToList();
                //ViewBag.sumatoria = costo;

                ExcelPackage Ep = new ExcelPackage();
                var Sheet = Ep.Workbook.Worksheets.Add("Artículos Costo Promedio");

                int row = 1;
                //Fijar la fuente para A1:Q1
                Sheet.Cells["A1:T1"].Style.Font.Size = 20;
                Sheet.Cells["A1:T1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:T1"].Style.Font.Bold = true;
                Sheet.Cells["A1:T1"].Style.Font.Color.SetColor(Color.DarkBlue);
                Sheet.Cells["A1:T1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                Sheet.Cells["A1"].RichText.Add("Artículos Costo Promedio");
                Sheet.Cells["A1:T1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;

                if (idalm == 0 && idfam == 0)
                {
                    row = 2;
                    Sheet.Cells["C2"].Value = "Todos los almacenes";
                    row = 3;
                    Sheet.Cells["C3"].Value = "Todas las familias";
                }
                if (idalm == 0 && idfam != 0)
                {
                    row = 2;
                    Sheet.Cells["C2"].Value = "Todos los almacenes";
                    row = 3;
                    string fam = new FamiliaContext().Familias.Find(idfam).Descripcion;
                    Sheet.Cells["C2"].Value = "Familia: " + fam;

                }
                if (idalm != 0 && idfam == 0)
                {
                    row = 2;
                    Sheet.Cells["C3"].Value = "Todas las familias";
                    row = 3;
                    string alm = new AlmacenContext().Almacenes.Find(idalm).Descripcion;
                    Sheet.Cells["C3"].Value = "Almacen: " + alm; ;

                }
                if (idalm != 0 && idfam != 0)
                {
                    row = 2;
                    string fam = new FamiliaContext().Familias.Find(idfam).Descripcion;
                    Sheet.Cells["C2"].Value = "Familia: " + fam;
                    row = 3;
                    string alm = new AlmacenContext().Almacenes.Find(idalm).Descripcion;
                    Sheet.Cells["C3"].Value = "Almacen: " + alm; ;
                }
                row = 4;

                Sheet.Cells["B4"].Value = "Tipo de cambio";
                Sheet.Cells[string.Format("B4", row)].Style.Numberformat.Format = "$#,##0.00";
                Sheet.Cells["C4"].Value = cambio.TC;
                row = 5;
                Sheet.Cells["A5:T5"].Style.Font.Name = "Calibri";
                Sheet.Cells["A5:T5"].Style.Font.Size = 12;
                Sheet.Cells["A5:T5"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;
                Sheet.Cells["A5:T5"].Style.Font.Bold = true;

                Sheet.Cells["A5"].Value = "ID";
                Sheet.Cells["B5"].Value = "Clave";
                Sheet.Cells["C5"].Value = "Artículo";
                Sheet.Cells["D5"].Value = "NP";
                Sheet.Cells["E5"].Value = "Presentación";
                Sheet.Cells["F5"].Value = "Fecha Ultima Compra";
                Sheet.Cells["G5"].Value = "Ultimo Costo";
                Sheet.Cells["H5"].Value = "Ultima Moneda";
                Sheet.Cells["I5"].Value = "Costo Resgistrado";
                Sheet.Cells["J5"].Value = "Moneda Resgistrada";
                Sheet.Cells["K5"].Value = "Costo Promedio Pesos";
                Sheet.Cells["L5"].Value = "Costo Promedio Dls";
                Sheet.Cells["M5"].Value = "Existencia";
                Sheet.Cells["N5"].Value = "Unidad";
                Sheet.Cells["O5"].Value = "Costo Inventario MXN";
                Sheet.Cells["P5"].Value = "Costo Inventario DLS";
                Sheet.Cells["Q5"].Value = "Almacen";
                Sheet.Cells["R5"].Value = "Familia";
                Sheet.Cells["S5"].Value = "Stock Mínimo";
                Sheet.Cells["T5"].Value = "Stock Máximo";
                Sheet.Cells["U5"].Value = "Mínimo de Venta";
                Sheet.Cells["V5"].Value = "Mínimo de Compra";

                row = 6;
                foreach (TempInvProm item in costo)
                {
                    Sheet.Cells[string.Format("A{0}", row)].Value = item.ID;
                    Sheet.Cells[string.Format("B{0}", row)].Value = item.cref;
                    Sheet.Cells[string.Format("C{0}", row)].Value = item.Descripcion;
                    Sheet.Cells[string.Format("D{0}", row)].Value = item.np;
                    Sheet.Cells[string.Format("E{0}", row)].Value = item.Presentacion;
                    Sheet.Cells[string.Format("F{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    Sheet.Cells[string.Format("F{0}", row)].Value = item.FechaUltimaCompra;
                    Sheet.Cells[string.Format("G{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("G{0}", row)].Value = item.UltimoCosto;
                    Sheet.Cells[string.Format("H{0}", row)].Value = item.UltimaMoneda;
                    Caracteristica cara = new ArticuloContext().Database.SqlQuery<Caracteristica>("select  * from Caracteristica where ID=" + item.IDCaracteristica).FirstOrDefault();
                    Articulo arti = new ArticuloContext().Articulo.Find(cara.Articulo_IDArticulo);
                    ClsDatoDecimal dato = new ArticuloContext().Database.SqlQuery<ClsDatoDecimal>("select dbo.getcosto(0," + cara.Articulo_IDArticulo + ",1) as Dato").FirstOrDefault();

                    Sheet.Cells[string.Format("I{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("I{0}", row)].Value = dato.Dato;
                    Sheet.Cells[string.Format("J{0}", row)].Value = arti.c_Moneda.ClaveMoneda;
                    Sheet.Cells[string.Format("K{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("K{0}", row)].Value = item.PromedioenPesos;
                    Sheet.Cells[string.Format("L{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("L{0}", row)].Value = item.Promedioendls;
                    Sheet.Cells[string.Format("M{0}", row)].Style.Numberformat.Format = "#,##0.00";
                    Sheet.Cells[string.Format("M{0}", row)].Value = item.existencia;
                    Sheet.Cells[string.Format("N{0}", row)].Value = item.Unidad;
                    Sheet.Cells[string.Format("O{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("O{0}", row)].Value = item.CostoInvPesos;
                    Sheet.Cells[string.Format("P{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("P{0}", row)].Value = item.CostoInvDls;
                    Sheet.Cells[string.Format("Q{0}", row)].Value = item.Almacen;
                    Sheet.Cells[string.Format("R{0}", row)].Value = item.Familia;
                    Sheet.Cells[string.Format("S{0}", row)].Style.Numberformat.Format = "#,##0.00";
                    Sheet.Cells[string.Format("S{0}", row)].Value = item.StockMin;
                    Sheet.Cells[string.Format("T{0}", row)].Style.Numberformat.Format = "#,##0.00";
                    Sheet.Cells[string.Format("T{0}", row)].Value = item.StockMax;
                    Sheet.Cells[string.Format("U{0}", row)].Style.Numberformat.Format = "#,##0.00";
                    Sheet.Cells[string.Format("U{0}", row)].Value = item.MinimoVenta;
                    Sheet.Cells[string.Format("V{0}", row)].Style.Numberformat.Format = "#,##0.00";
                    Sheet.Cells[string.Format("V{0}", row)].Value = item.MinimoCompra;
                    row++;
                }
                Sheet.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment: filename=" + "ReporteExcelArtCostoProm.xlsx");
                Response.BinaryWrite(Ep.GetAsByteArray());
                Response.End();
                return View();
            }
        }
        public ActionResult IIporlotemp(string Almacen)
        {
            if (Almacen==null)
            {
                Almacen = "6";
            }
            if (Almacen==string.Empty)
            {
                Almacen = "6";
            }

            ViewBag.Almacen = Almacen;
            int IDAlmacen = int.Parse(Almacen);
            var elementos = new InventarioAlmacenContext().inventariompxcbs.OrderBy(s => s.codigo).ToList().Where(s=> s.IDAlmacen==IDAlmacen);
            ViewBag.Conteo = elementos.Count();
            return View(elementos);
        }

        [HttpPost]
        public ActionResult IIporlotemp(FormCollection coleccion)

        {
            string Almacen = coleccion.Get("Almacen").ToString();

            if (Almacen == null)
            {
                Almacen = "6";
            }
            if (Almacen == string.Empty)
            {
                Almacen = "6";
            }

            Session["Almacen"] = Almacen;

            string codigo = coleccion.Get("codigo").ToString();
            string[] separacion = codigo.Split('/');

            bool existe = false;


            try
            {
                Clslotemp lotemp = new Clslotemp();
                lotemp = db.Database.SqlQuery<Clslotemp>("Select * from clslotemp where MetrosDisponibles>0 and loteinterno='" + codigo + "'").ToList().FirstOrDefault();

                if (lotemp != null)
                {
                    existe = true;
                }
            }
            catch (Exception err)
            {

            }


            try
            {
                int ancho = int.Parse(separacion[1].ToString());
                int largo = int.Parse(separacion[2].ToString());
                decimal m2 = Math.Round(decimal.Parse(largo.ToString()) * (decimal.Parse(ancho.ToString()) / 1000M), 2);
                inventariompxcb inventariompxcb = new inventariompxcb();
                inventariompxcb = db.Database.SqlQuery<inventariompxcb>("Select * from inventariompxcb where codigo='" + codigo + "' ").ToList().FirstOrDefault();


                if (inventariompxcb == null)
                {
                    //class 
                    string clave = separacion[0]; //+ "-" + separacion[1];
                    new InventarioAlmacenContext().Database.ExecuteSqlCommand("insert into inventariompxcb (codigo,clave,ancho, largo,m2,cinta, Existencia,IDAlmacen ) values ('" + codigo + "','" + clave + "'," + ancho + "," + largo + "," + m2 + "," + separacion[5] + ",'" + existe + "',"+Almacen+")");

                }
            }
            catch (Exception err)
            {

            }
            return RedirectToAction("IIporlotemp",new { Almacen =Almacen  });
        }

        public ActionResult IIporloteTintas(string Almacen)
        {
            if (Almacen == null)
            {
                Almacen = "1";
            }
            if (Almacen == string.Empty)
            {
                Almacen = "1";
            }

            ViewBag.Almacen = Almacen;
            int IDAlmacen = int.Parse(Almacen);
            var elementos = new InventarioAlmacenContext().inventariotintaxcbs.OrderBy(s => s.id).ToList().Where(s => s.IDAlmacen == IDAlmacen);
            ViewBag.Conteo = elementos.Count();
            return View(elementos);
        }

        [HttpPost]
        public ActionResult IIporloteTintas(FormCollection coleccion)

        {
            string Almacen = coleccion.Get("Almacen").ToString();

            if (Almacen == null)
            {
                Almacen = "1";
            }
            if (Almacen == string.Empty)
            {
                Almacen = "1";
            }

            Session["Almacen"] = Almacen;

            string codigo = coleccion.Get("Lote").ToString();
            string[] separacion = codigo.Split('&');

            bool existe = false;

            Clslotetinta lotemp = new Clslotetinta();
            try
            {
                
                lotemp = db.Database.SqlQuery<Clslotetinta>("Select * from [clslotetinta] where  lote='" + codigo + "'").ToList().FirstOrDefault();

                if (lotemp != null)
                {
                    existe = true;
                }
            }
            catch (Exception err)
            {

            }


            try
            {
                ////////POSICIONES///////7777
                //Codigo almacen-clave - cantidad - unidad - -factura - consecutivo
                //[O] -- [1] --[2]--[3]-- [4]--[5]

                string codigoAlmacen = separacion[0];
                string claveArt = separacion[1];
                decimal cantidad = decimal.Parse(separacion[2].ToString());
                string Unidad = separacion[3];
                string factura = separacion[4];
                int Consecutivo = int.Parse(separacion[5].ToString());
                int iddetrecepcion = 0;
                int idalmacen = int.Parse(Almacen);
                string estado = "";
                if (existe)
                {
                    iddetrecepcion = lotemp.iddetrecepcion;
                    idalmacen = lotemp.IDAlmacen;

                    estado = lotemp.Estado;
                }

               

                inventariotintaxcb inventariompxcb = new inventariotintaxcb();
                inventariompxcb = db.Database.SqlQuery<inventariotintaxcb>("Select * from inventariotintaxcb where Lote='" + codigo + "' ").ToList().FirstOrDefault();


                if (inventariompxcb == null)
                {
                    //class 
                    
                    new InventarioAlmacenContext().Database.ExecuteSqlCommand("insert into inventariotintaxcb (Lote,clave,consecutivo, cantidad,iddetrecepcion, Existencia,IDAlmacen,Estado )" +
                                                                    " values ('" + codigo + "','" + claveArt + "'," + Consecutivo + "," + cantidad + "," + iddetrecepcion + ",'" + existe + "'," + idalmacen + ",'"+estado+"')");

                }
            }
            catch (Exception err)
            {

            }
            return RedirectToAction("IIporloteTintas", new { Almacen = Almacen });
        }
        [HttpPost]
        public JsonResult ActualizarLoteTinta(int id)
        {

            try
            {
                inventariotintaxcb inventariompxcb = new inventariotintaxcb();
                inventariompxcb = db.Database.SqlQuery<inventariotintaxcb>("Select * from inventariotintaxcb where id='" + id + "' ").ToList().FirstOrDefault();

                Clslotetinta lotemp = new Clslotetinta();
                try
                {

                    lotemp = db.Database.SqlQuery<Clslotetinta>("Select * from [clslotetinta] where  lote='" + inventariompxcb.Lote + "'").ToList().FirstOrDefault();

                    string cadena = "update clslotetinta set estado='Existe' where id=" + lotemp.id;
                    db.Database.ExecuteSqlCommand(cadena);
                    string cadena1 = "update inventariotintaxcb set estado='Existe' where id=" +id;
                    db.Database.ExecuteSqlCommand(cadena1);

                    try
                    {
                        new InventarioAlmacenContext().Database.ExecuteSqlCommand("update InventarioAlmacen set Existencia=(Existencia+" + lotemp.cantidad + ") where idalmacen=" + lotemp.IDAlmacen + " and idcaracteristica=" + lotemp.idcaracteristica);
                        new InventarioAlmacenContext().Database.ExecuteSqlCommand("update InventarioAlmacen set Disponibilidad=(Existencia-Apartado) where idalmacen=" + lotemp.IDAlmacen + " and idcaracteristica=" + lotemp.idcaracteristica);

                    
                        List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
                    int usuario = userid.Select(s => s.UserID).FirstOrDefault();

                    try
                    {
                        InventarioAlmacen arrr = new InventarioAlmacenContext().InventarioAlmacenes.Where(s => s.IDAlmacen == lotemp.IDAlmacen && s.IDCaracteristica == lotemp.idcaracteristica).FirstOrDefault();
                            Caracteristica cara = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from Caracteristica where id=" + lotemp.idcaracteristica).FirstOrDefault();

                            string cadenam = "INSERT INTO [dbo].[MovimientoArticulo] ([Fecha],[Id],[IDPresentacion],[Articulo_IDArticulo],[Accion],[cantidad],[Documento],[NoDocumento],[Lote],[IDAlmacen],[TipoOperacion],[acumulado],[observacion],[Hora],Usuario) VALUES ";
                        cadenam += " (getdate(), " + cara.ID + "," + cara.IDPresentacion + "," + cara.Articulo_IDArticulo + ",'Devolución de Lote Tinta Interno'," + inventariompxcb.cantidad + ",'Devolución de Lote Tinta Interno '," + 0 + ",'" + lotemp.lote + "',"+lotemp.IDAlmacen+",'E'," + arrr.Existencia + ",'',CONVERT (time, SYSDATETIMEOFFSET())," + usuario + ")";
                        db.Database.ExecuteSqlCommand(cadenam);
                    }
                    catch (Exception err)
                    {
                        string mensajee = err.Message;
                    }
                }
                    catch (Exception  err)
                    {

                    }
                }
                catch (Exception err)
                {

                }

                return Json(true);
            }


            catch (Exception err)
            {
                return Json(500, err.Message);
            }




        }

        public ActionResult AgregarTinta(int id, string Mensaje="")
        {
            ViewBag.Mensaje = Mensaje;
            inventariotintaxcb inventariompxcb = new inventariotintaxcb();
            inventariompxcb = db.Database.SqlQuery<inventariotintaxcb>("Select * from inventariotintaxcb where id=" + id + "").ToList().FirstOrDefault();
            Session["Almacen"] = inventariompxcb.IDAlmacen;
            Articulo articulo = new Articulo();
            articulo = db.Database.SqlQuery<Articulo>("Select * from Articulo where cref='" + inventariompxcb.clave + "'").ToList().FirstOrDefault();

            if (articulo == null)
            {
                throw new Exception("Artículo " + inventariompxcb.clave + " no registrado en Artículos");
            }

            Caracteristica CARACTERISTICA = new Caracteristica();
            CARACTERISTICA = db.Database.SqlQuery<Caracteristica>("Select * from Caracteristica where Articulo_IDArticulo=" + articulo.IDArticulo).ToList().FirstOrDefault();

            ViewBag.IDArticulo = articulo.Descripcion;
            ViewBag.idInven = id;
            ViewBag.Caracteristica = CARACTERISTICA.Presentacion;
            List<Caracteristica> cara = new List<Caracteristica>();
            string cadena2 = "select * from [dbo].[Caracteristica] where Articulo_IDArticulo=" + articulo.IDArticulo;
            cara = db.Database.SqlQuery<Caracteristica>(cadena2).ToList();


            List<SelectListItem> listafamilia = new List<SelectListItem>();
            listafamilia.Add(new SelectListItem { Text = "--Selecciona una presentacion--", Value = "0" });
            foreach (var n in cara)
            {
                listafamilia.Add(new SelectListItem { Text = n.IDPresentacion + "|" + n.Presentacion, Value = n.ID.ToString(), });
            }
            ViewBag.IDCaracteristica = listafamilia;

            return View();
        }
        [HttpPost]
        public ActionResult AgregarLoteTinta(int id, int IDCaracteristica)
        {
            string Almacen = Session["Almacen"].ToString();
            int IDAlmacen = int.Parse(Almacen);
            try
            {

                inventariotintaxcb inventariompxcb = new inventariotintaxcb();
                inventariompxcb = db.Database.SqlQuery<inventariotintaxcb>("Select * from inventariotintaxcb where id=" + id + "").ToList().FirstOrDefault();
                Clslotetinta lotemp = new Clslotetinta();
                bool existe = false;
                string estado = "";
                try
                {

                    lotemp = db.Database.SqlQuery<Clslotetinta>("Select * from clslotetinta where  lote='" + inventariompxcb.clave + "'").ToList().FirstOrDefault();
                    if (lotemp != null)
                    {
                        existe = true;
                        estado= lotemp.Estado;

                    }

                }
                catch (Exception err)
                {

                }




                Caracteristica cara = new Caracteristica();
                cara = db.Database.SqlQuery<Caracteristica>("Select * from Caracteristica where ID=" + IDCaracteristica).ToList().FirstOrDefault();

                Articulo articulo = new Articulo();
                articulo = db.Database.SqlQuery<Articulo>("Select * from Articulo where IDArticulo='" + cara.Articulo_IDArticulo + "'").ToList().FirstOrDefault();
               
                if (existe)
                {
                 
                    return RedirectToAction("AgregarTinta", new {id=id, Mensaje="Lote existente con estado de "+ estado});
                }
                else
                {
                    try
                    {
                        string[] separacion = inventariompxcb.Lote.Split('&');
                        ////////POSICIONES///////7777
                        //Codigo almacen-clave - cantidad - unidad - -factura - consecutivo
                        //[O] -- [1] --[2]--[3]-- [4]--[5]

                        string codigoAlmacen = separacion[0];
                        string claveArt = separacion[1];
                        decimal cantidad = decimal.Parse(separacion[2].ToString());
                        string Unidad = separacion[3];
                        string factura = separacion[4];
                        int Consecutivo = int.Parse(separacion[5].ToString());
                        int iddetrecepcion = 0;
                        int idalmacen = int.Parse(Almacen);
                        int IDRecepcion = 0;
                        int IDOrdenCompra = 0;
                        if (existe)
                        {
                            iddetrecepcion = lotemp.iddetrecepcion;
                            idalmacen = IDAlmacen;

                            DetRecepcion detRecepcion = new RecepcionContext().DetRecepciones.Find(iddetrecepcion);
                            IDRecepcion = detRecepcion.IDRecepcion;
                            IDOrdenCompra = detRecepcion.IDExterna;
                        }
                        string NoFactura = factura.Remove(0, 1);
                        int IDFactura = int.Parse(NoFactura);
                        //BUSCAR FACTURA RELACIONADA A LA RECEPCION

                        string busqueda = "select*from encrecepcion where documentofactura='" + factura + "'";
                        EncRecepcion encRecepcion = new RecepcionContext().Database.SqlQuery<EncRecepcion>(busqueda).ToList().FirstOrDefault();
                        if (encRecepcion!=null)
                        {
                            DetRecepcion det = new RecepcionContext().Database.SqlQuery<DetRecepcion>("select*from detRecepcion where idrecepcion=" + encRecepcion.IDRecepcion+ " and caracteristica_id="+ cara.ID).ToList().FirstOrDefault();
                            iddetrecepcion = det.IDDetRecepcion;
                            idalmacen = IDAlmacen;
                            IDRecepcion = det.IDRecepcion;
                            IDOrdenCompra = det.IDExterna;
                        }


                        string cadenaMP = "INSERT INTO [dbo].[clslotetinta]([idarticulo],[idcaracteristica],[fecha],[factura],[cantidad],[unidad],[consecutivo],[IDFamilia],[IDAlmacen],[cref],[IDRecepcion],[OrdenCompra],[iddetrecepcion],[lote],[ccodalm],[Estado])" +
                            "VALUES(" + articulo.IDArticulo + "," + cara.ID + ",'" + DateTime.Now.ToString() + "','" + factura + "'," + cantidad + ",'" + Unidad + "','" + Consecutivo + "'," + articulo.IDFamilia + "," + IDAlmacen + ",'" + claveArt + "'," + IDRecepcion + "," + IDOrdenCompra + "," + iddetrecepcion + ",'" + inventariompxcb.Lote + "','" + codigoAlmacen + "','Existe')";
                        db.Database.ExecuteSqlCommand(cadenaMP);

                        InventarioAlmacen arr = new InventarioAlmacenContext().InventarioAlmacenes.Where(s => s.IDAlmacen == IDAlmacen && s.IDCaracteristica == cara.ID).FirstOrDefault();
                        if (arr == null)
                        {
                            new InventarioAlmacenContext().Database.ExecuteSqlCommand("insert into InventarioAlmacen ( IDAlmacen,IDArticulo,IDCaracteristica,Existencia,Porllegar,Apartado,Disponibilidad) values (" + IDAlmacen + "," + cara.Articulo_IDArticulo + "," + cara.ID + "," + cantidad + ",0,0," + cantidad + ")");
                        }
                        else
                        {

                            new InventarioAlmacenContext().Database.ExecuteSqlCommand("update InventarioAlmacen set Existencia=(Existencia+" + cantidad + ") where idalmacen=" + IDAlmacen + " and idcaracteristica=" + cara.ID);
                            new InventarioAlmacenContext().Database.ExecuteSqlCommand("update InventarioAlmacen set Disponibilidad=(Existencia-Apartado) where idalmacen=" + IDAlmacen + " and idcaracteristica=" + cara.ID);

                        }
                        List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
                        int usuario = userid.Select(s => s.UserID).FirstOrDefault();

                        try
                        {
                            InventarioAlmacen arrr = new InventarioAlmacenContext().InventarioAlmacenes.Where(s => s.IDAlmacen == IDAlmacen && s.IDCaracteristica == cara.ID).FirstOrDefault();
                            
                            string cadenam = "INSERT INTO [dbo].[MovimientoArticulo] ([Fecha],[Id],[IDPresentacion],[Articulo_IDArticulo],[Accion],[cantidad],[Documento],[NoDocumento],[Lote],[IDAlmacen],[TipoOperacion],[acumulado],[observacion],[Hora],Usuario) VALUES ";
                            cadenam += " (getdate(), " + cara.ID + "," + cara.IDPresentacion + "," + cara.Articulo_IDArticulo + ",'Carga de Lote Tinta Interno'," + inventariompxcb.cantidad + ",'Carga de Lote Tinta Interno '," + 0 + ",'" + inventariompxcb.Lote + "',"+ IDAlmacen + ",'E'," + arrr.Existencia + ",'Lote ingresado desde Inventario por lotes',CONVERT (time, SYSDATETIMEOFFSET()),'" + usuario + "')";
                            db.Database.ExecuteSqlCommand(cadenam);
                        }
                        catch (Exception err)
                        {
                            string mensajee = err.Message;
                        }

                        db.Database.ExecuteSqlCommand("Update inventariotintaxcb set existencia='true', estado='Existe' where id=" + id);

                    }
                    catch (Exception err)
                    {
                        return RedirectToAction("AgregarTinta", new { id = id, Mensaje = "No coincide con ninguna recepción" });
                    }

                }
                  
                //}



            }
            catch (Exception err)
            {
                return Json(500, err.Message);
            }
            return RedirectToAction("IIporloteTintas");
        }


        public ActionResult AgregarPre(int id)
        {
           
            inventariompxcb inventariompxcb = new inventariompxcb();
            inventariompxcb = db.Database.SqlQuery<inventariompxcb>("Select * from inventariompxcb where id=" + id + "").ToList().FirstOrDefault();
            Session["Almacen"] = inventariompxcb.IDAlmacen;
            Articulo articulo = new Articulo();
            articulo = db.Database.SqlQuery<Articulo>("Select * from Articulo where cref='" + inventariompxcb.clave + "'").ToList().FirstOrDefault();

            if (articulo== null)
            {
                throw new Exception("Artículo " + inventariompxcb.clave + " no registrado en Artículos");
            }

            Caracteristica CARACTERISTICA = new Caracteristica();
            CARACTERISTICA = db.Database.SqlQuery<Caracteristica>("Select * from Caracteristica where Articulo_IDArticulo=" + articulo.IDArticulo).ToList().FirstOrDefault();

            ViewBag.IDArticulo = articulo.Descripcion;
            ViewBag.idInven = id;
            ViewBag.Caracteristica = CARACTERISTICA.Presentacion;
            List<Caracteristica> cara = new List<Caracteristica>();
            string cadena2 = "select * from [dbo].[Caracteristica] where Articulo_IDArticulo=" + articulo.IDArticulo;
            cara = db.Database.SqlQuery<Caracteristica>(cadena2).ToList();


            List<SelectListItem> listafamilia = new List<SelectListItem>();
            listafamilia.Add(new SelectListItem { Text = "--Selecciona una presentacion--", Value = "0" });
            foreach (var n in cara)
            {
                listafamilia.Add(new SelectListItem { Text = n.IDPresentacion + "|" + n.Presentacion, Value = n.ID.ToString(), });
            }
            ViewBag.IDCaracteristica = listafamilia;

            return View();
        }
        [HttpPost]
        public ActionResult AgregarLote(int id, int IDCaracteristica)
        {
            string Almacen = Session["Almacen"].ToString();
            int IDAlmacen = int.Parse(Almacen); 
            try
            {

                inventariompxcb inventariompxcb = new inventariompxcb();
                inventariompxcb = db.Database.SqlQuery<inventariompxcb>("Select * from inventariompxcb where id=" + id + "").ToList().FirstOrDefault();
                Clslotemp lotemp = new Clslotemp();
                bool existe = false;
                try
                {

                    lotemp = db.Database.SqlQuery<Clslotemp>("Select * from clslotemp where MetrosDisponibles=0 and loteinterno='" + inventariompxcb.codigo + "'").ToList().FirstOrDefault();
                    if (lotemp != null)
                    {
                        existe = true;
                    }

                }
                catch (Exception err)
                {

                }




                Caracteristica cara = new Caracteristica();
                cara = db.Database.SqlQuery<Caracteristica>("Select * from Caracteristica where ID=" + IDCaracteristica).ToList().FirstOrDefault();

                Articulo articulo = new Articulo();
                articulo = db.Database.SqlQuery<Articulo>("Select * from Articulo where IDArticulo='" + cara.Articulo_IDArticulo + "'").ToList().FirstOrDefault();
                if (existe == true)
                {
                    try
                    {

                        db.Database.ExecuteSqlCommand("Update Clslotemp set metrosCuadrados=" + inventariompxcb.m2 + ", metrosdisponibles=" + inventariompxcb.m2 + "where ID=" + lotemp.ID);

                        decimal existencias = 0;
                        try
                        {
                            existencias = db.Database.SqlQuery<ClsDatoDecimal>("select sum(metrosdisponibles) as dato from Clslotemp  where idarticulo=" + articulo.IDArticulo + " and idcaracteristica=" + cara.ID +" and IDAlmacen="+Almacen).ToList().FirstOrDefault().Dato;

                        }
                        catch (Exception err)
                        {

                        }

                        InventarioAlmacen arr = new InventarioAlmacenContext().InventarioAlmacenes.Where(s => s.IDAlmacen == IDAlmacen && s.IDCaracteristica == cara.ID).FirstOrDefault();
                        if (arr == null)
                        {
                            new InventarioAlmacenContext().Database.ExecuteSqlCommand("insert into InventarioAlmacen ( IDAlmacen,IDArticulo,IDCaracteristica,Existencia,Porllegar,Apartado,Disponibilidad) values ("+IDAlmacen+","+cara.Articulo_IDArticulo+","+cara.ID+","+existencias+",0,0,0)");
                        }
                        else
                        {

                            new InventarioAlmacenContext().Database.ExecuteSqlCommand("update InventarioAlmacen set Existencia=" + existencias + ", Disponibilidad=(" + existencias + "- apartado) where idalmacen=" + Almacen + " and idcaracteristica=" + cara.ID);
                        }
                        try
                        {
                            string cadenam = "INSERT INTO [dbo].[MovimientoArticulo] ([Fecha],[Id],[IDPresentacion],[Articulo_IDArticulo],[Accion],[cantidad],[Documento],[NoDocumento],[Lote],[IDAlmacen],[TipoOperacion],[acumulado],[observacion],[Hora]) VALUES ";
                            cadenam += " (getdate(), " + cara.ID + "," + cara.IDPresentacion + "," + cara.Articulo_IDArticulo + ",'Carga de Lote Interno'," + inventariompxcb.m2 + ",'Carga de Lote Interno '," + 0 + ",'" + inventariompxcb.codigo + "',"+Almacen+",'E'," + existencias + ",'',CONVERT (time, SYSDATETIMEOFFSET()))";
                            db.Database.ExecuteSqlCommand(cadenam);
                        }
                        catch (Exception err)
                        {
                            string mensajee = err.Message;
                        }

                        db.Database.ExecuteSqlCommand("Update inventariompxcb set existencia='true' where id=" + id);

                    }
                    catch (Exception err)
                    {

                    }
                }
                else
                {
                    try
                    {
                        string[] separacion = inventariompxcb.codigo.Split('/');



                        string cadenaMP = "INSERT INTO [dbo].[Clslotemp]([IDArticulo],[IDCaracteristica],[NoCinta],[IDDetOrdenCompra],[Ancho],[Largo],[Lote],[LoteInterno],[Fecha],[OrdenCompra],[MetrosCuadrados],[Metrosutilizados],[MetrosDisponibles],[IDProveedor],[IDAlmacen],[FacturaProv])VALUES(" +
                                                                            articulo.IDArticulo + "," + cara.ID + "," + separacion[5] + ",0," + inventariompxcb.ancho + "," + inventariompxcb.largo + ",'','" + inventariompxcb.codigo + "'," + "SYSDATETIME(),6," + inventariompxcb.m2 + ",0," + inventariompxcb.m2 + ",6,"+Almacen+",0)";
                        db.Database.ExecuteSqlCommand(cadenaMP);

                        decimal existencias = 0;
                        try
                        {
                            existencias = db.Database.SqlQuery<ClsDatoDecimal>("select sum(metrosdisponibles) as dato from Clslotemp  where idarticulo=" + articulo.IDArticulo + " and idcaracteristica=" + cara.ID).ToList().FirstOrDefault().Dato;

                        }
                        catch (Exception err)
                        {

                        }


                        InventarioAlmacen arr = new InventarioAlmacenContext().InventarioAlmacenes.Where(s => s.IDAlmacen == IDAlmacen && s.IDCaracteristica == cara.ID).FirstOrDefault();
                        if (arr == null)
                        {
                            new InventarioAlmacenContext().Database.ExecuteSqlCommand("insert into InventarioAlmacen ( IDAlmacen,IDArticulo,IDCaracteristica,Existencia,Porllegar,Apartado,Disponibilidad) values (" + IDAlmacen + "," + cara.Articulo_IDArticulo + "," + cara.ID + "," + existencias + ",0,0,"+existencias+")");
                        }
                        else
                        {

                            new InventarioAlmacenContext().Database.ExecuteSqlCommand("update InventarioAlmacen set Existencia=" + existencias + ", Disponibilidad=(" + existencias + "- apartado) where idalmacen=" + Almacen + " and idcaracteristica=" + cara.ID);
                        }

                        try
                        {
                            string cadenam = "INSERT INTO [dbo].[MovimientoArticulo] ([Fecha],[Id],[IDPresentacion],[Articulo_IDArticulo],[Accion],[cantidad],[Documento],[NoDocumento],[Lote],[IDAlmacen],[TipoOperacion],[acumulado],[observacion],[Hora]) VALUES ";
                            cadenam += " (getdate(), " + cara.ID + "," + cara.IDPresentacion + "," + cara.Articulo_IDArticulo + ",'Carga de Lote Interno'," + inventariompxcb.m2 + ",'Carga de Lote Interno '," + 0 + ",'" + inventariompxcb.codigo + "',"+Almacen+",'E'," + existencias + ",'',CONVERT (time, SYSDATETIMEOFFSET()))";
                            db.Database.ExecuteSqlCommand(cadenam);
                        }
                        catch (Exception err)
                        {
                            string mensajee = err.Message;
                        }

                        db.Database.ExecuteSqlCommand("Update inventariompxcb set existencia='true' where id=" + id);

                    }
                    catch (Exception err)
                    {

                    }
                }



            }
            catch (Exception err)
            {
                return Json(500, err.Message);
            }
            return RedirectToAction("IIporlotemp");
        }
        [HttpPost]
        public JsonResult VerificarExistencias()
        {
            try
            {
                var elementos = new InventarioAlmacenContext().inventariompxcbs.Where(s => s.Existencia == true).ToList();

                foreach (var item in elementos)
                {
                    decimal existencias = 0;
                    try
                    {
                        Articulo articulo = new Articulo();
                        articulo = db.Database.SqlQuery<Articulo>("Select * from Articulo where cref='" + item.clave + "'").ToList().FirstOrDefault();

                        Caracteristica cara = new Caracteristica();
                        cara = db.Database.SqlQuery<Caracteristica>("Select * from Caracteristica where Articulo_IDArticulo=" + articulo.IDArticulo + " and Presentacion='ANCHO:" + item.ancho + ",LARGO:" + item.largo + "'").ToList().FirstOrDefault();

                        existencias = db.Database.SqlQuery<ClsDatoDecimal>("select sum(metrosdisponibles) as dato from Clslotemp  where idarticulo=" + articulo.IDArticulo + " and idcaracteristica=" + cara.ID).ToList().FirstOrDefault().Dato;


                        try
                        {
                            new InventarioAlmacenContext().Database.ExecuteSqlCommand("update InventarioAlmacen set Existencia=" + existencias + ", Disponibilidad=(" + existencias + "- apartado) where idalmacen=1 and idcaracteristica=" + cara.ID);

                        }
                        catch (Exception err)
                        {

                        }

                    }
                    catch (Exception err)
                    {

                    }
                }

                return Json(true);
            }
             


            catch (Exception err)
            {
                return Json(500, err.Message);
            } 
        }
        [HttpPost]
        public JsonResult Deletelote()
        {
            try
            {
                new InventarioAlmacenContext().Database.ExecuteSqlCommand("delete from inventariompxcb");
                return Json(true);
            }
            catch (Exception err)
            {
                return Json(500, err.Message);
            }
        }
        [HttpPost]
        public JsonResult DeleteitemLote(int id)
        {
            try
            {
                CarritoContext car = new CarritoContext();
                car.Database.ExecuteSqlCommand("delete from inventariompxcb where id=" + id);

                return Json(true);
            }
            catch (Exception err)
            {
                return Json(500, err.Message);
            }
        }
        [HttpPost]
        public JsonResult DeleteLoteIntemTinta(int id)
        {
            try
            {
                CarritoContext car = new CarritoContext();
                car.Database.ExecuteSqlCommand("delete from inventariotintaxcb where id=" + id);

                return Json(true);
            }
            catch (Exception err)
            {
                return Json(500, err.Message);
            }
        }
        public ActionResult Deleteloteall()
        {
            try
            {
                new InventarioAlmacenContext().Database.ExecuteSqlCommand("delete from inventariompxcb ");
            }
            catch (Exception err)
            {

            }
            return RedirectToAction("InventarioInicialporlotemp");
        }

        public void Deletelotealltempinv(int IDAlmacen, int IDFamilia)
        {
            try
            {
                new VPedidoAvgContext().Database.ExecuteSqlCommand("delete from TempInvProm");

                string filtro = "select * from inventarioalmacen inner join articulo as a on inventarioalmacen.Idarticulo=a.Idarticulo  Where Existencia>0 ";
                if (IDAlmacen > 0)
                {
                    filtro = filtro + " and inventarioalmacen.idalmacen=" + IDAlmacen + " ";
                }
                if (IDFamilia > 0)
                {
                    filtro = filtro + " and a.idFamilia=" + IDFamilia;
                }

                List<InventarioAlmacen> elementos = new InventarioAlmacenContext().Database.SqlQuery<InventarioAlmacen>(filtro).ToList();

                foreach (InventarioAlmacen elemento in elementos)
                {
                    /// vamos a buscar si el elemento tiene una recepcion
                    /// 
                    /// vamos a buscar si el elemento tiene una recepcion
                    /// 
                    Articulo arti = new ArticuloContext().Articulo.Find(elemento.IDArticulo);
                    VPedidoAvg recepcionado = new VPedidoAvgContext().VPedidoAvgs.Where(s => s.IDArticulo == elemento.IDArticulo).ToList().FirstOrDefault();

                    decimal ultimocosto = 0;
                    int idultimamoneda = 0;
                    string ultimamoneda;
                    decimal costopromediopesos = 0;
                    decimal costopromediodolares = 0;
                    DateTime Ultimafecha = DateTime.Now;
                    // si viene nulo es por que no tenemosreceocion
                    if (recepcionado != null)
                    {
                        ultimocosto = recepcionado.UltimoCosto;
                        idultimamoneda = int.Parse(recepcionado.IDUltimaMoneda.ToString());
                        costopromediopesos = recepcionado.PromedioenPesos;
                        costopromediodolares = recepcionado.Promedioendls;
                        Ultimafecha = recepcionado.FechaUltimaCompra;
                        c_Moneda moneda = new c_MonedaContext().c_Monedas.Find(recepcionado.IDUltimaMoneda);
                        ultimamoneda = moneda.ClaveMoneda;
                    }
                    else
                    {

                        // hay que ir por su costo a articulo

                        ClsDatoDecimal costo = new CostoArticuloContext().Database.SqlQuery<ClsDatoDecimal>("select dbo.getcosto(0," + elemento.IDArticulo + ",1) as Dato ").FirstOrDefault();


                        try
                        {
                            ultimocosto = costo.Dato;
                            ultimamoneda = arti.c_Moneda.ClaveMoneda;
                        }
                        catch (Exception nulo)
                        {
                            ultimocosto = 0;
                           // Articulo articulo = new ArticuloContext().Articulo.Find(elemento.IDArticulo);
                            ultimamoneda = arti.c_Moneda.ClaveMoneda;
                        }

                        if (ultimamoneda == "MXN")
                        {
                            costopromediopesos = ultimocosto;
                        }
                        else
                        {
                            ClsDatoDecimal tc = new InventarioContext().Database.SqlQuery<ClsDatoDecimal>("select dbo.GetTipocambioCadena(getdate(),'" + ultimamoneda + "','MXN') as Dato").FirstOrDefault();

                            costopromediopesos = ultimocosto * tc.Dato;
                        }

                        if (ultimamoneda == "USD")
                        {
                            costopromediodolares = ultimocosto;
                        }
                        else
                        {
                            ClsDatoDecimal tc = new InventarioContext().Database.SqlQuery<ClsDatoDecimal>("select dbo.GetTipocambioCadena(getdate(),'" + ultimamoneda + "','USD') as Dato").FirstOrDefault();

                            costopromediodolares = ultimocosto * tc.Dato;
                        }




                    }

                   
                    Almacen almacen;
                    Familia familia;

                    if (IDAlmacen > 0)
                    {
                        almacen = new AlmacenContext().Almacenes.Find(IDAlmacen);
                    }
                    else
                    {
                        almacen = new AlmacenContext().Almacenes.Find(elemento.IDAlmacen);
                    }
                    if (IDFamilia > 0)
                    {
                        familia = new FamiliaContext().Familias.Find(IDFamilia);
                    }
                    else
                    {
                        familia = new FamiliaContext().Familias.Find(arti.IDFamilia);
                    }


                    string UF = Ultimafecha.Year.ToString() + "-" + Ultimafecha.Month.ToString() + "-" + Ultimafecha.Day.ToString();

                    //   VArtCaracteristica cara = new VArtCaracteristicaContext().VArtC.ToList().Where(s => s.IDArticulo == elemento.IDArticulo && s.IDCaracteristica == elemento.IDCaracteristica).FirstOrDefault();


                    Caracteristica cara = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from Caracteristica where id=" + elemento.IDCaracteristica).FirstOrDefault();


                    try
                    {
                        string sqlCadena = "Insert into TempInvProm (IDArticulo, FechaUltimaCompra,UltimoCosto, UltimaMoneda,PromedioenPesos,Promedioendls,IDCaracteristica,cref, Descripcion,  Presentacion, np, existencia, Unidad, CostoInvPesos, CostoInvDls,  IDFamilia, Familia, IDAlmacen, Almacen,MinimoVenta, MinimoCompra, Stockmin, StockMax )";
                        sqlCadena += "values( " + arti.IDArticulo + ", '" + UF + "'," + ultimocosto + ", '" + ultimamoneda + "', " + costopromediopesos + ", " + costopromediodolares + ", " + cara.ID + ", '" + arti.Cref + "', '" + arti.Descripcion + "', '" + cara.Presentacion + "', " + cara.IDPresentacion + ", " + elemento.Existencia + ", '" + arti.c_ClaveUnidad.Nombre + "', " + costopromediopesos * elemento.Existencia + ", " + costopromediodolares * elemento.Existencia + ", " + familia.IDFamilia + ",'" + familia.Descripcion + "', " + almacen.IDAlmacen + ", '" + almacen.Descripcion + "', " + arti.MinimoVenta + ", " + arti.MinimoCompra + ", " + arti.StockMin + ", " + arti.StockMax + " )";
                        db.Database.ExecuteSqlCommand(sqlCadena);
                    }
                    catch (Exception err)
                    {
                        string mensaje = err.Message;
                    }
                }

            }
            catch (Exception err)
            {
                string mensaje = err.Message;
            }
        }

        public class FamAlmRepository
        {
            public IEnumerable<SelectListItem> GetAlmacenesF(int IDAlmacen)
            {

                List<SelectListItem> lista;
                using (var context = new FamiliaContext())
                {
                    string cadenasql = "select*from Familia as f inner join FamAlm as fa on f.idfamilia=fa.idfamilia where fa.idalmacen=" + IDAlmacen;
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

        [HttpPost]
        public ActionResult InventarioArtA(InventarioPorAlmacenA modelo, FormCollection coleccion)
        {
            int idA = modelo.idalmacen;
            int idF = modelo.idfamilia;
            string cual = coleccion.Get("Enviar");
            string cadena = "";
            string cadenaE = string.Empty;
            string cadenaE1 = string.Empty;


            string cadenacinta = "";

            if (cual == "Generar reporte")
            {
                InventarioPorAlmacenA report = new InventarioPorAlmacenA();
                byte[] abytes = report.PrepareReport(modelo.idalmacen, modelo.idfamilia);
                return File(abytes, "application/pdf");
                ////return Redirect("index");
            }
            if (cual == "Generar excel")
            {

                ReporteInventarioAlmacen encabezado = new ReporteInventarioAlmacen();

                List<ReporteInventarioArt> inventario = new List<ReporteInventarioArt>();
                List<VClslotemp> cintas = new List<VClslotemp>();
                VCambio cambio = db.Database.SqlQuery<VCambio>("select dbo.GetTipocambioCadena(GETDATE(),'USD','MXN') as TC").ToList()[0];
                encabezado.TC = cambio.TC;

                try
                {
                    if (idA == 0 && idF == 0)
                    {
                        // obtengo lista de datos encabezado Reporte
                        encabezado.Rep = 100;
                        encabezado.Titulo = "REPORTE GENERAL DE ARTÍCULOS";

                        //cadena cuerpo del reporte 
                        cadena = "select * from ReporteInventarioArt where existencia>0 order by IDAlmacen, IDFamilia, Cref";

                        //cadena Hoja 2
                        cadenacinta = "select * from VClslotemp";

                    }

                    if (idA == 0 && idF != 0)
                    {
                        // obtengo lista de datos encabezado Reporte
                        encabezado.Rep = 101;
                        cadenaE = "select IDFamilia, CCodFam, Descripcion from familia  where IDfamilia = " + idF + "";
                        ReporteInventarioFam fam1 = db.Database.SqlQuery<ReporteInventarioFam>(cadenaE).ToList()[0];
                        encabezado.Familia = fam1.Descripcion;
                        encabezado.Titulo = "REPORTE GENERAL DE ARTÍCULOS, DE LA FAMILIA " + encabezado.Familia + "";

                        //cadena cuerpo del reporte
                        cadena = "select * from ReporteInventarioArt where IDFamilia = " + idF + " and existencia>0 order by IDAlmacen, IDFamilia, Cref";

                        //cadena Hoja 2
                        cadenacinta = "select * from VClslotemp where IDFamilia = " + idF + "";


                    }
                    if (idA != 0 && idF == 0)
                    {
                        // obtengo lista de datos encabezado Reporte
                        encabezado.Rep = 110;
                        cadenaE = "select IDAlmacen, CodAlm, Descripcion  from almacen  where IDAlmacen = " + idA + "";
                        ReporteInventarioAlm enc1 = db.Database.SqlQuery<ReporteInventarioAlm>(cadenaE).ToList()[0];
                        encabezado.Almacen = enc1.Descripcion;
                        encabezado.Titulo = "REPORTE GENERAL DE ARTÍCULOS, DEL ALMACEN " + encabezado.Almacen + "";

                        //cadena cuerpo del reporte
                        cadena = "select * from ReporteInventarioArt where IDAlmacen = " + idA + "  and existencia>0  order by IDAlmacen, IDFamilia, Cref";

                        //cadena Hoja 2
                        cadenacinta = "select * from VClslotemp where IDAlmacen = " + idA + "";

                    }
                    if (idA != 0 && idF != 0)
                    {
                        // obtengo lista de datos encabezado Reporte
                        encabezado.Rep = 111;
                        cadenaE = "select IDFamilia, CCodFam, Descripcion from familia  where IDfamilia = " + idF + "";
                        ReporteInventarioFam fam2 = db.Database.SqlQuery<ReporteInventarioFam>(cadenaE).ToList()[0];
                        encabezado.Familia = fam2.Descripcion;
                        cadenaE1 = "select IDAlmacen, CodAlm, Descripcion  from almacen  where IDAlmacen = " + idA + "";
                        ReporteInventarioAlm enc2 = db.Database.SqlQuery<ReporteInventarioAlm>(cadenaE1).ToList()[0];
                        encabezado.Almacen = enc2.Descripcion;
                        encabezado.Titulo = "REPORTE GENERAL DE ARTÍCULOS, DEL ALMACEN " + encabezado.Almacen + " DE LA FAMILIA " + encabezado.Familia + "";

                        //cadena cuerpo del reporte
                        cadena = " select * from ReporteInventarioArt  where IDAlmacen = " + idA + " and IDFamilia= " + idF + "  and existencia>0  order by IDAlmacen, IDFamilia, Cref";

                        //cadena Hoja 2
                        cadenacinta = "select * from VClslotemp where IDAlmacen = " + idA + " and IDFamilia= " + idF + "";
                    }
                }
                catch (SqlException err)
                {
                    string mensajedeerror = err.Message;
                }

                // obtengo lista de datos Hoja 1
                inventario = db.Database.SqlQuery<ReporteInventarioArt>(cadena).ToList();
                ViewBag.datos = inventario;

                // obtengo lista de datos Hoja  2
                cintas = db.Database.SqlQuery<VClslotemp>(cadenacinta).ToList();
                ViewBag.datosDet = cintas;



                encabezado.TC = cambio.TC;
                if (idA == 0 && idF == 0)
                {
                    encabezado.Rep = 100;
                    encabezado.Titulo = "REPORTE GENERAL DE ARTÍCULOS";

                }
                if (idA == 0 && idF != 0)
                {
                    encabezado.Rep = 101;
                    string cadenaA = "select IDFamilia, CCodFam, Descripcion from familia  where IDfamilia = " + idF + "";
                    ReporteInventarioFam fam = db.Database.SqlQuery<ReporteInventarioFam>(cadenaA).ToList()[0];
                    encabezado.Familia = fam.Descripcion;
                    encabezado.Titulo = "REPORTE GENERAL DE ARTÍCULOS, DE LA FAMILIA " + encabezado.Familia + "";
                }
                if (idA != 0 && idF == 0)
                {
                    encabezado.Rep = 110;
                    string cadenaA = "select IDAlmacen, CodAlm, Descripcion  from almacen  where IDAlmacen = " + idA + "";
                    ReporteInventarioAlm enc = db.Database.SqlQuery<ReporteInventarioAlm>(cadenaA).ToList()[0];
                    encabezado.Almacen = enc.Descripcion;
                    encabezado.Titulo = "REPORTE GENERAL DE ARTÍCULOS, DEL ALMACEN " + encabezado.Almacen + "";
                }
                if (idA != 0 && idF != 0)
                {
                    encabezado.Rep = 111;
                    string cadenaA = "select IDFamilia, CCodFam, Descripcion from familia  where IDfamilia = " + idF + "";
                    ReporteInventarioFam fam = db.Database.SqlQuery<ReporteInventarioFam>(cadenaE).ToList()[0];
                    encabezado.Familia = fam.Descripcion;
                    string cadenaA1 = "select IDAlmacen, CodAlm, Descripcion  from almacen  where IDAlmacen = " + idA + "";
                    ReporteInventarioAlm enc = db.Database.SqlQuery<ReporteInventarioAlm>(cadenaA1).ToList()[0];
                    encabezado.Almacen = enc.Descripcion;
                    encabezado.Titulo = "REPORTE GENERAL DE ARTÍCULOS, DEL ALMACEN " + encabezado.Almacen + " DE LA FAMILIA " + encabezado.Familia + "";
                }

                ExcelPackage Ep = new ExcelPackage();
                //Crear la hoja y poner el nombre de la pestaña del libro
                var Sheet = Ep.Workbook.Worksheets.Add("Reporte Artículos");

                // en la fila1 formateo las celdas y coloco el título de la hoja
                // RichText permite agregar las propiedades de tipo de letra: color, negrita, etc.
                int row = 1;
                Empresa empresa = new EmpresaContext().empresas.Find(2);
                // System.Drawing.Image logo = byteArrayToImage(empresa.Logo);
                //Image jpg = Image.GetInstance(logo, BaseColor.WHITE);
                int alto = 18;
                int ancho = 75;


                //Fijar la fuente para A1:Q1
                Sheet.Cells["A1:J1"].Style.Font.Size = 20;
                Sheet.Cells["A1:J1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:J3"].Style.Font.Bold = true;
                Sheet.Cells["A1:J1"].Style.Font.Color.SetColor(Color.DarkBlue);
                Sheet.Cells["A1:J1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                Sheet.Cells["A1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                Sheet.Cells["A1:J1"].Style.WrapText = true;

                using (System.Drawing.Image logo = byteArrayToImage(empresa.Logo))
                {
                    var excelImage = Sheet.Drawings.AddPicture("My Logo", logo);

                    //add the image to row 20, column E
                    excelImage.SetPosition(0, 0, 0, 0);
                    excelImage.SetSize(ancho, alto);
                }
                Sheet.Cells["A1:J1"].RichText.Add("           " + encabezado.Titulo);

                row = 2;
                Sheet.Cells["A1:J1"].Style.Font.Size = 12;
                Sheet.Cells["A1:J1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:J1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
                Sheet.Cells["A2:J2"].Style.Font.Bold = true;
                //Subtitulo según el filtrado del periodo de datos
                row = 2;
                Sheet.Cells[string.Format("A2", row)].Value = encabezado.Titulo;
                row = 3;
                Sheet.Cells["A2:B2"].Style.WrapText = true;
                Sheet.Cells[string.Format("A2", row)].Value = "Precio Dolar";
                Sheet.Cells[string.Format("B2", row)].Style.Numberformat.Format = "0.0000";
                Sheet.Cells[string.Format("B2", row)].Value = encabezado.TC;

                string fecha = DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day;
                Sheet.Cells["I2:J2"].Style.WrapText = true;
                Sheet.Cells[string.Format("I2", row)].Value = "Fecha de emisión";
                Sheet.Cells[string.Format("J2", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("J2", row)].Value = fecha;

                //En la fila3 se da el formato a el encabezado
                row = 4;
                Sheet.Cells.Style.Font.Name = "Calibri";
                Sheet.Cells.Style.Font.Size = 10;
                Sheet.Cells["A3:J3"].Style.Font.Bold = true;
                Sheet.Cells["A3:J3"].Style.WrapText = true;
                Color colorfromHex = System.Drawing.ColorTranslator.FromHtml("#424242");
                Sheet.Cells["A3:J3"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                Sheet.Cells["A3:J3"].Style.Fill.BackgroundColor.SetColor(colorfromHex);

                // Se establece el nombre que identifica a cada una de las columnas de datos.
                Sheet.Cells["A3"].RichText.Add("Clave");
                Sheet.Cells["B3"].RichText.Add("Articulo");
                Sheet.Cells["C3"].RichText.Add("NP");
                Sheet.Cells["D3"].RichText.Add("Presentación");
                Sheet.Cells["E3"].RichText.Add("Existencia");
                Sheet.Cells["F3"].RichText.Add("Por Llegar");
                Sheet.Cells["G3"].RichText.Add("Apartado");
                Sheet.Cells["H3"].RichText.Add("Disponibilidad");
                Sheet.Cells["I3"].RichText.Add("Almacen");
                Sheet.Cells["J3"].RichText.Add("Familia");


                //Aplicar borde doble al rango de celdas A3:Q3
                //Sheet.Cells["A3:J3"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;

                // en la fila4, el foreach llenara cada fila de la hoja de excel, con cada fila que trae de la bd.
                // Se establecen los formatos para las celdas: Fecha, Moneda

                //En la fila3 se da el formato a el encabezado
                row = 4;
                Sheet.Cells.Style.Font.Name = "Calibri";
                Sheet.Cells.Style.Font.Size = 10;
                Sheet.Cells["A4:J4"].Style.Font.Bold = true;
                //Sheet.Cells["A3:J3"].Style.Font.Color.SetColor(Color.White);
                Sheet.Cells["A4:J4"].Style.WrapText = true;
                Color colorfromHexs = System.Drawing.ColorTranslator.FromHtml("#616161");
                Sheet.Cells["A4:J4"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                Sheet.Cells["A4:J4"].Style.Fill.BackgroundColor.SetColor(colorfromHexs);






                row = 5;
                Sheet.Cells.Style.Font.Bold = false;
                foreach (ReporteInventarioArt item in ViewBag.datos)
                {

                    Caracteristica caracteristica = new ArticuloContext().Database.SqlQuery<Caracteristica>("select*from caracteristica where id="+ item.IDCaracteristica).ToList().FirstOrDefault();
                    Sheet.Cells.Style.Font.Name = "Calibri";
                    Sheet.Cells.Style.Font.Size = 10;
                    Sheet.Cells["A" + row + ":J" + row + ""].Style.Font.Bold = true;
                    Sheet.Cells["A" + row + ":J" + row + ""].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    Sheet.Cells["A" + row + ":J" + row + ""].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);


                    Sheet.Cells[string.Format("A{0}", row)].Value = item.Cref;
                    Sheet.Cells[string.Format("B{0}", row)].Value = item.Articulo;
                    Sheet.Cells[string.Format("C{0}", row)].Value = caracteristica.IDPresentacion;
                    Sheet.Cells[string.Format("D{0}", row)].Value = item.Presentacion;
                    Sheet.Cells[string.Format("E{0}", row)].Style.Numberformat.Format = "0.0000";
                    Sheet.Cells[string.Format("E{0}", row)].Value = item.Existencia;
                    Sheet.Cells[string.Format("F{0}", row)].Style.Numberformat.Format = "0.0000";
                    Sheet.Cells[string.Format("F{0}", row)].Value = item.Porllegar;
                    Sheet.Cells[string.Format("G{0}", row)].Style.Numberformat.Format = "0.0000";
                    Sheet.Cells[string.Format("G{0}", row)].Value = item.Apartado;
                    Sheet.Cells[string.Format("H{0}", row)].Style.Numberformat.Format = "0.0000";
                    Sheet.Cells[string.Format("H{0}", row)].Value = item.Disponibilidad;
                    var alm = db.Database.SqlQuery<Almacen>("select * from Almacen where idAlmacen = " + item.IDAlmacen).ToList().FirstOrDefault();
                    Sheet.Cells[string.Format("I{0}", row)].Value = alm.Descripcion;
                    var fam = db.Database.SqlQuery<Familia>("select * from Familia where IDFamilia = " + item.IDFamilia).ToList().FirstOrDefault();
                    Sheet.Cells[string.Format("J{0}", row)].Value = fam.Descripcion;
                    row++;


                    ArticuloContext dbe = new ArticuloContext();
                    Articulo articulo = dbe.Articulo.Where(s => s.Cref == item.Cref).ToList().FirstOrDefault();
                    if (articulo.IDTipoArticulo == 6)
                    {
                        SqlConnection conexion = (SqlConnection)new ArticuloFEContext().Database.Connection;

                        try
                        {
                            conexion.Open();
                        }
                        catch (Exception erro)
                        {

                        }
                        IDataReader datos = new SqlCommand("select IDARTICulo,idcaracteristica, ancho, largo, count(id) as numerocintas,MetrosDisponibles from clslotemp where idalmacen=" + item.IDAlmacen + " and idArticulo=" + articulo.IDArticulo + " and IDCaracteristica=" + item.IDCaracteristica + " and metrosdisponibles>0  group by IDARTICulo,idcaracteristica, ancho, metrosdisponibles,largo", conexion).ExecuteReader();
                        try
                        {
                            while (datos.Read())
                            {
                                int Cintas = int.Parse(datos["numerocintas"].ToString());
                                int anchoe = int.Parse(datos["ancho"].ToString());

                                decimal metrosdisponibleslineales = decimal.Parse(datos["MetrosDisponibles"].ToString()) / decimal.Parse((anchoe * 0.001M).ToString());
                                int largo = int.Parse(Math.Round(metrosdisponibleslineales, 0).ToString());


                                //Sheet.Cells["B{0}"].Style.Font.Bold = true;
                                Sheet.Cells[string.Format("B{0}", row)].Value = Cintas + " Cintas  de " + anchoe + " X " + largo;
                                row++;

                            }
                        }
                        catch (Exception errcintas)
                        {

                        }
                        SqlConnection.ClearAllPools();
                    }

                }

                //Hoja No. 2
                Sheet = Ep.Workbook.Worksheets.Add("Cintas");
                row = 1;
                //Fijar la fuente para A1:Q1
                Sheet.Cells["A1:L1"].Style.Font.Size = 20;
                Sheet.Cells["A1:L1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:L3"].Style.Font.Bold = true;
                Sheet.Cells["A1:L1"].Style.Font.Color.SetColor(Color.DarkBlue);
                Sheet.Cells["A1:L1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                Sheet.Cells["A1"].RichText.Add("Detalle de recepciones");

                row = 2;
                Sheet.Cells["A1:L1"].Style.Font.Size = 12;
                Sheet.Cells["A1:L1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:L1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
                Sheet.Cells["A2:L2"].Style.Font.Bold = true;
                //Subtitulo según el filtrado del periodo de datos
                row = 2;
                Sheet.Cells[string.Format("A2", row)].Value = encabezado.Titulo;
                row = 3;
                Sheet.Cells[string.Format("A2", row)].Value = "Precio Dolar";
                Sheet.Cells[string.Format("B2", row)].Style.Numberformat.Format = "0.0000";
                Sheet.Cells[string.Format("B2", row)].Value = encabezado.TC;

                Sheet.Cells[string.Format("D2", row)].Value = "Fecha de emisión";
                Sheet.Cells[string.Format("E2", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("E2", row)].Value = fecha;

                //En la fila3 se da el formato a el encabezado
                row = 4;
                Sheet.Cells.Style.Font.Name = "Calibri";
                Sheet.Cells.Style.Font.Size = 10;
                Sheet.Cells["A3:L3"].Style.Font.Bold = true;
                Sheet.Cells["A3:L3"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                Sheet.Cells["A3:L3"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                // Se establece el nombre que identifica a cada una de las columnas de datos.
                Sheet.Cells["A3"].RichText.Add("Clave");
                Sheet.Cells["B3"].RichText.Add("Artículo");
                Sheet.Cells["C3"].RichText.Add("Presentación");
                Sheet.Cells["D3"].RichText.Add("Tipo de Artículo"); ;
                Sheet.Cells["E3"].RichText.Add("Lote Interno");
                Sheet.Cells["F3"].RichText.Add("Ancho");
                Sheet.Cells["G3"].RichText.Add("Largo");
                Sheet.Cells["H3"].RichText.Add("Lote");
                Sheet.Cells["I3"].RichText.Add("M2");
                Sheet.Cells["J3"].RichText.Add("M Usados");
                Sheet.Cells["K3"].RichText.Add("M disponibles");
                Sheet.Cells["L3"].RichText.Add("Almacen");

                //Aplicar borde doble al rango de celdas A3:Q3
                Sheet.Cells["A3:L3"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;

                // en la fila4, el foreach llenara cada fila de la hoja de excel, con cada fila que trae de la bd.
                // Se establecen los formatos para las celdas: Fecha, Moneda
                row = 4;
                Sheet.Cells.Style.Font.Bold = false;
                foreach (VClslotemp itemD in ViewBag.datosDet)
                {

                    Sheet.Cells[string.Format("A{0}", row)].Value = itemD.Cref;
                    Sheet.Cells[string.Format("B{0}", row)].Value = itemD.Articulo;
                    Sheet.Cells[string.Format("C{0}", row)].Value = itemD.Presentacion;
                    Sheet.Cells[string.Format("D{0}", row)].Value = itemD.Descripcion;
                    Sheet.Cells[string.Format("E{0}", row)].Value = itemD.LoteInterno;
                    Sheet.Cells[string.Format("F{0}", row)].Value = itemD.Ancho;
                    Sheet.Cells[string.Format("G{0}", row)].Value = itemD.Largo;
                    Sheet.Cells[string.Format("H{0}", row)].Value = itemD.Lote;
                    Sheet.Cells[string.Format("I{0}", row)].Style.Numberformat.Format = "0.0000";
                    Sheet.Cells[string.Format("I{0}", row)].Value = itemD.MetrosCuadrados;
                    Sheet.Cells[string.Format("J{0}", row)].Style.Numberformat.Format = "0.0000";
                    Sheet.Cells[string.Format("J{0}", row)].Value = itemD.Metrosutilizados;
                    Sheet.Cells[string.Format("K{0}", row)].Style.Numberformat.Format = "0.0000";
                    Sheet.Cells[string.Format("K{0}", row)].Value = itemD.MetrosDisponibles;
                    Sheet.Cells[string.Format("L{0}", row)].Style.Numberformat.Format = "0.00";
                    Sheet.Cells[string.Format("L{0}", row)].Value = itemD.Almacen;

                    row++;
                }
                //Se genera el archivo y se descarga

                Sheet.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment: filename=" + "Articulo.xlsx");
                Response.BinaryWrite(Ep.GetAsByteArray());
                Response.End();
                return Redirect("/blah");
            }
            return Redirect("index");
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



        /////////////////////////////////////////////77
        ///
        //[HttpPost]
        //public JsonResult VerificarExistenciasT()
        //{
        //    try
        //    {
        //        var elementos = new InventarioAlmacenContext().inventariotintaxcbs.Where(s => s.Existencia == true).ToList();

        //        foreach (var item in elementos)
        //        {
        //            decimal existencias = 0;
        //            try
        //            {
        //                Articulo articulo = new Articulo();
        //                articulo = db.Database.SqlQuery<Articulo>("Select * from Articulo where cref='" + item.clave + "'").ToList().FirstOrDefault();

        //                Caracteristica cara = new Caracteristica();
        //                cara = db.Database.SqlQuery<Caracteristica>("Select * from Caracteristica where Articulo_IDArticulo=" + articulo.IDArticulo + " and Presentacion='ANCHO:" + item.ancho + ",LARGO:" + item.largo + "'").ToList().FirstOrDefault();

        //                existencias = db.Database.SqlQuery<ClsDatoDecimal>("select sum(metrosdisponibles) as dato from Clslotemp  where idarticulo=" + articulo.IDArticulo + " and idcaracteristica=" + cara.ID).ToList().FirstOrDefault().Dato;


        //                try
        //                {
        //                    new InventarioAlmacenContext().Database.ExecuteSqlCommand("update InventarioAlmacen set Existencia=" + existencias + ", Disponibilidad=(" + existencias + "- apartado) where idalmacen=1 and idcaracteristica=" + cara.ID);

        //                }
        //                catch (Exception err)
        //                {

        //                }

        //            }
        //            catch (Exception err)
        //            {

        //            }
        //        }

        //        return Json(true);
        //    }



        //    catch (Exception err)
        //    {
        //        return Json(500, err.Message);
        //    }
        //}
        [HttpPost]
        public JsonResult DeleteloteT()
        {
            try
            {
                new InventarioAlmacenContext().Database.ExecuteSqlCommand("delete from inventariotintaxcbs");
                return Json(true);
            }
            catch (Exception err)
            {
                return Json(500, err.Message);
            }
        }



        ///////SEGUIMIENTO STOCK
        ///
        public ActionResult SeguimientoStock(string sortOrder, string currentFilter, int? page, int? PageSize, string searchString = "")
        {
           
            // Not sure here
            if (searchString == null)
            {
                searchString = currentFilter;
            }

            // Pass filtering string to view in order to maintain filtering when paging
            ViewBag.SearchString = searchString;
            //Paginación
            List<VSeguimientoStock> almacen = new List<VSeguimientoStock>();
            string cadena1 = "";
            if (searchString != "")
            {
                cadena1 = "select * from VSeguimientoStock where Almacen like '%" + searchString + "%' or IDOrden like '%" + searchString + "%' or Clave like '%"+searchString+"%'";

            }
            else
            {
                cadena1 = "select * from VSeguimientoStock";

            }
            almacen = db.Database.SqlQuery<VSeguimientoStock>(cadena1).ToList();



           
            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = almacen.OrderBy(e => e.IDSeguimiento).Count(); // Total number of elements

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

            return View(almacen.ToPagedList(pageNumber, pageSize));
            //Paginación
        }

        public ActionResult CancelarOP(int IDOrden)
        {
            OrdenProduccion orden = new OrdenProduccionContext().OrdenesProduccion.Find(IDOrden);
           
            ViewBag.OrdenP = orden;
            ViewBag.IDOrden = IDOrden;

            ViewBag.idestadoanterior = orden.EstadoOrden;
            ViewBag.Descripcion = orden.Descripcion;



            ViewBag.FecCambio = DateTime.Today.ToShortDateString();
            ViewBag.HorCambio = DateTime.Now.ToString("HH:mm:ss");

           
            
            return View();
        }
            [HttpPost]
        public ActionResult CancelarOP(int IDOrden, CambioEstado estado)
        {
           
            List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            int usuario = userid.Select(s => s.UserID).FirstOrDefault();
            string usuarioNombre = userid.Select(s => s.Username).FirstOrDefault();
            OrdenProduccion orden = new OrdenProduccionContext().OrdenesProduccion.Find(IDOrden);



            estado.EstadoActual = "Cancelada";
            estado.Fecha = DateTime.Now;
            estado.EstadoAnterior = orden.EstadoOrden;
            estado.IDOrden = orden.IDOrden;



            DetPedido detallepedido = new PedidoContext().DetPedido.Where(s => s.IDPedido == orden.IDPedido && s.IDArticulo == orden.IDArticulo && s.Caracteristica_ID == orden.IDCaracteristica).FirstOrDefault();
            int AlmacenViene = detallepedido.IDAlmacen;

            Caracteristica carateristica = new WorkinProcessContext().Database.SqlQuery<Caracteristica>("select * from caracteristica where id=" + orden.IDCaracteristica).ToList().FirstOrDefault();

            InventarioAlmacen inv = new InventarioAlmacenContext().InventarioAlmacenes.ToList().Where(s => s.IDAlmacen == AlmacenViene && s.IDCaracteristica == orden.IDCaracteristica).FirstOrDefault();



            db.Database.ExecuteSqlCommand("update InventarioAlmacen set Porllegar=(Porllegar-" + orden.Cantidad + ") where IDArticulo= " + orden.IDArticulo + " and IDCaracteristica=" + orden.IDCaracteristica + " and IDAlmacen=" + AlmacenViene + "");
                db.Database.ExecuteSqlCommand("update InventarioAlmacen set Porllegar=0 where IDArticulo= " + orden.IDArticulo + " and IDCaracteristica=" + orden.IDCaracteristica + " and IDAlmacen=" + AlmacenViene + " and porllegar<0");
                try
                {
                    string cadenam = "INSERT INTO [dbo].[MovimientoArticulo] ([Fecha],[Id],[IDPresentacion],[Articulo_IDArticulo],[Accion],[cantidad],[Documento],[NoDocumento],[Lote],[IDAlmacen],[TipoOperacion],[acumulado],[observacion],[Hora], Usuario) VALUES ";
                    cadenam += "                           (getdate(), " + carateristica.ID + "," + carateristica.IDPresentacion + "," + carateristica.Articulo_IDArticulo + ",'Cancelación Orden Stock'," + orden.Cantidad + ",'Orden Produccion '," + orden.IDOrden + ",''," + AlmacenViene + ",'N/A'," + inv.Existencia + ",'',CONVERT (time, SYSDATETIMEOFFSET()),"+usuario+")";
                    db.Database.ExecuteSqlCommand(cadenam);
                }
                catch (Exception err)
                {

                }
                List<MaterialAsignado> solicitud = db.Database.SqlQuery<MaterialAsignado>("Select * from MaterialAsignado where [orden]=" + IDOrden).ToList();

                foreach (MaterialAsignado item in solicitud)
                {

                    decimal m2 = decimal.Parse(item.cantidad.ToString()) * (decimal.Parse(item.ancho.ToString()) / 1000);

                    db.Database.ExecuteSqlCommand("update [dbo].[InventarioAlmacen] set disponibilidad= (disponibilidad +" + m2 + "), [apartado]= (apartado-" + m2 + ") where idalmacen=" + item.idalmacen + " and idarticulo=" + item.idmapri + "and idcaracteristica=" + item.idcaracteristica);

                    Caracteristica caracteristica = new Caracteristica();
                    caracteristica = db.Database.SqlQuery<Caracteristica>("Select * from Caracteristica where ID=" + item.idcaracteristica).ToList().FirstOrDefault();
                    InventarioAlmacen inventario = new InventarioAlmacenContext().InventarioAlmacenes.ToList().Where(a => a.IDAlmacen == item.idalmacen && a.IDCaracteristica == item.idcaracteristica).FirstOrDefault();


                    try
                    {
                        string cadenam = "INSERT INTO [dbo].[MovimientoArticulo] ([Fecha],[Id],[IDPresentacion],[Articulo_IDArticulo],[Accion],[cantidad],[Documento],[NoDocumento],[Lote],[IDAlmacen],[TipoOperacion],[acumulado],[observacion],[Hora],Usuario) VALUES ";
                        cadenam += " (getdate(), " + caracteristica.ID + "," + caracteristica.IDPresentacion + "," + caracteristica.Articulo_IDArticulo + ",'Eliminación de Asignación de Material en Producción OP Stock'," + m2 + ",'Orden Producción '," + item.orden + ",''," + item.idalmacen + ",'N/A'," + inventario.Existencia + ",'',CONVERT (time, SYSDATETIMEOFFSET()),"+usuario+")";
                        db.Database.ExecuteSqlCommand(cadenam);
                    }
                    catch (Exception err)
                    {
                        string mensajee = err.Message;
                    }

                    db.Database.ExecuteSqlCommand("delete from  MaterialAsignado where idmaterialasignado=" + item.IDMaterialAsignado);


                }

                List<WorkinProcess> work = db.Database.SqlQuery<WorkinProcess>("Select * from WorkinProcess where [idorden]=" + IDOrden).ToList();

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

            try
            {
                string cadena3 = " insert into dbo.CambioEstado(IDOrden, fecha, hora, EstadoAnterior, EstadoActual, motivo, usuario) values " +
                    "(" + estado.IDOrden + ",'" + DateTime.Now.ToString("yyyy-MM-dd") + "','" + DateTime.Now.ToString("HH:mm:ss") + "','" + estado.EstadoAnterior + "', '" + estado.EstadoActual + "', '" + estado.motivo + "','" + usuarioNombre + "')";
                db.Database.ExecuteSqlCommand(cadena3);

            }
            catch (Exception err)
            {

            }
            try
            {
                string cadena3 = "update Encpedido set status='Cancelado' where idpedido= " + orden.IDPedido; 
                       db.Database.ExecuteSqlCommand(cadena3);
                string cadena31 = "update detpedido set status='Cancelado' where idpedido= " + orden.IDPedido;
                db.Database.ExecuteSqlCommand(cadena31);

            }
            catch (Exception err)
            {

            }
            return Redirect("SeguimientoStock");
        }




    }

    public class RepositiryAlma
    {
        public IEnumerable<SelectListItem> GetAlmacenes()
        {
            using (var context = new AlmacenContext())
            {
                List<SelectListItem> lista = context.Almacenes.AsNoTracking()
                    .Where(n=> n.IDAlmacen!=19 &&  n.IDAlmacen!=3 && n.IDAlmacen!=12 && n.IDAlmacen!=15 && n.IDAlmacen!=16 && n.IDAlmacen!=21)
                    .OrderBy(n => n.Descripcion)
                        .Select(n =>
                        new SelectListItem
                        {
                            Value = n.IDAlmacen.ToString(),
                            Text = n.Descripcion
                        }).ToList();
                var descripciontip = new SelectListItem()
                {
                    Value = "0",
                    Text = "--- Selecciona un almacen ---"
                };
                lista.Insert(0, descripciontip);
                return new SelectList(lista, "Value", "Text");
            }
        }

    }
}
