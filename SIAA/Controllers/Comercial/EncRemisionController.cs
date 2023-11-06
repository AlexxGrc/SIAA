using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using SIAAPI.Models.Comercial;
using PagedList;
using SIAAPI.Models.Administracion;
using SIAAPI.ViewModels.Comercial;
using SIAAPI.ViewModels.Articulo;
using System.Web;
using SIAAPI.Models.Login;
using SIAAPI.ViewModels.Cfdi;

using System.IO;
using SIAAPI.Reportes;
using SIAAPI.Models.Cfdi;
using System.Collections;
using OfficeOpenXml;
using System.Drawing;
using OfficeOpenXml.Style;
using System.Globalization;
using SIAAPI.Models.Produccion;
using SIAAPI.Models.Inventarios;
using System.Text;

namespace SIAAPI.Controllers.Comercial
{
    public class EncRemisionController : Controller
    {
        private PedidoContext db = new PedidoContext();
        RemisionContext BD = new RemisionContext();
        ClientesContext prov = new ClientesContext();
        public ActionResult Index(string Divisa, string Status, string sortOrder, string currentFilter, string IDCliente, string Numero, string Fechainicio, string Fechafinal, int? page, int? PageSize)
        {
            string filtro = string.Empty;

            if (Divisa != string.Empty && Divisa != null)
            {
                filtro = " C.ClaveMoneda='" + Divisa + "'";
            }

            if (Status != string.Empty && Status != null)
            {
                if (filtro == string.Empty)
                {
                    filtro = " R.Status='" + Status + "'";
                }
                else
                {
                    filtro += " and  R.Status='" + Status + "'";
                }
            }

            if (IDCliente == null || IDCliente == string.Empty)
            {
                IDCliente = 0.ToString();
            }

            if (int.Parse(IDCliente) != 0)
            {
                if (filtro == string.Empty)
                {
                    filtro = " R.IDCliente=" + IDCliente + "";
                }
                else
                {
                    filtro += " and R.IDCliente=" + IDCliente + "";
                }
            }

            if (!String.IsNullOrEmpty(Numero))
            {
                if (filtro == string.Empty)
                {
                    filtro = "  R.IDRemision=" + Numero + "";
                }
                else
                {
                    filtro += " and  R.IDRemision=" + Numero + "";
                }

            }


            if (!String.IsNullOrEmpty(Fechainicio) && String.IsNullOrEmpty(Fechafinal)) //pusieron una fecha
            {
                if (filtro == string.Empty)
                {
                    filtro = "  R.Fecha BETWEEN '" + Fechainicio + " 00:00:00.1' and '" + Fechainicio + " 23:59:59.999' ";
                }
                else
                {
                    filtro += " and R.Fecha BETWEEN '" + Fechainicio + " 00:00:00.1' and '" + Fechainicio + " 23:59:59.999'";
                }
            }


            if (!String.IsNullOrEmpty(Fechainicio) && !String.IsNullOrEmpty(Fechafinal)) //pusieron una fecha
            {
                if (filtro == string.Empty)
                {
                    filtro = "   R.Fecha BETWEEN '" + Fechainicio + " 00:00:00.1' and '" + Fechafinal + " 23:59:59.999' ";
                }
                else
                {
                    filtro += " and R.Fecha BETWEEN '" + Fechainicio + " 00:00:00.1' and '" + Fechafinal + " 23:59:59.999'";
                }
            }

            // carga status
            List<SelectListItem> StaLst = getEstados(Status);

            ViewBag.Status = StaLst;

            ViewBag.StatusSeleccionado = Status;

            // Carga monedas
            List<SelectListItem> divisasel = getMonedas(Divisa);

            ViewBag.Divisa = divisasel; // construye el combo con la divisa seleccionada anteriormente

            ViewBag.DivisaSeleccionada = Divisa;  // el que viene del pagina
            ViewBag.FechaIni = Fechainicio;
            ViewBag.FechaFin = Fechafinal;


            ViewBag.CurrentSort = sortOrder;

            ViewBag.NumeroSortParm = String.IsNullOrEmpty(sortOrder) ? "ID" : "";
            ViewBag.FechaSortParm = sortOrder == "Fecha" ? "Fecha" : "";
            /// carga Cliente
            /// 
            var Clientes = new ClienteRepository().GetClientesNombre();

            ViewBag.IDCliente = Clientes;

            ViewBag.IDClienteSeleccionado = IDCliente;

            // Pass filtering string to view in order to maintain filtering when paging
            ViewBag.Fechainicio = Fechainicio;
            ViewBag.Fechafinal = Fechafinal;


            string Orden = string.Empty;
            switch (sortOrder)
            {


                case "Numero":
                    Orden = " order by   R.IDRemision asc ";
                    break;
                case "Fecha":
                    Orden = " order by R.fecha ";
                    break;

                default:
                    Orden = " order by R.IDRemision desc ";
                    break;
            }


            if (filtro == string.Empty) // si viene vacio tomamos los del ultimo mes
            {
                filtro += " R.Fecha >='" + DateTime.Now.AddDays(-30).Year + "-" + DateTime.Now.AddDays(-30).Month + "-" + DateTime.Now.AddDays(-30).Day + "'  and  Fecha<'" + DateTime.Now.AddDays(1).Year + "-" + DateTime.Now.AddDays(1).Month + "-" + DateTime.Now.AddDays(1).Day + " 23:00:00' ";
                //ViewBag.Fechainicio = "" + DateTime.Now.AddDays(-30).Year + "-" + DateTime.Now.AddDays(-30).Month + "-" + DateTime.Now.AddDays(-30).Day + "";
                //ViewBag.Fechafinal = "" + DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + "";
            }







            //Filtro Status


            string ConsultaSql = "select R.* from EncRemision AS R inner join c_Moneda as C on R.IDMoneda= C.IDMoneda  ";

            string cadenaSQl = ConsultaSql + " where " + filtro + "  " + Orden;

            var elementos = db.Database.SqlQuery<EncRemision>(cadenaSQl).ToList();

            string ConsultaSqlResumen = "select M.ClaveMoneda as Moneda, (SUM(Subtotal)) as Subtotal, (SUM(IVA)) as IVA, (SUM(Total)) as Total, (SUM(Total * TC)) as TotalenPesos from encRemision inner join c_Moneda as M on EncRemision.IDMoneda=M.IDMoneda  ";
            string ConsultaAgrupado = " group by Moneda order by Moneda ";


            ViewBag.sumatoria = "";
            try
            {

                var SumaLst = new List<string>();
                var SumaQry = ConsultaSqlResumen + " " + filtro + "  and Encremision.Status!='Cancelado' " + ConsultaAgrupado;
                List<ResumenFac> data = db.Database.SqlQuery<ResumenFac>(SumaQry).ToList();
                ViewBag.sumatoria = data;

            }
            catch (Exception err)
            {
                string mensaje = err.Message;
            }



            int count = elementos.Count(); // Total number of elements

            // Populate DropDownList
            ViewBag.PageSize = new List<SelectListItem>()
            {
                new SelectListItem { Text = "10 ", Value = "10", Selected = true },
                new SelectListItem { Text = "25", Value = "25" },
                new SelectListItem { Text = "50", Value = "50" },
                new SelectListItem { Text = "100", Value = "100" },
                new SelectListItem { Text = "Todos", Value = count.ToString() }
             };

            int pageNumber = (page ?? 1);
            int pageSize = (PageSize ?? 10);
            ViewBag.psize = pageSize;

            return View(elementos.ToPagedList(pageNumber, pageSize));



        }

        public ActionResult Details(int? id)
        {
            List<VDetRemision> orden = BD.Database.SqlQuery<VDetRemision>("select DetRemision.IDExterna,DetRemision.Devolucion,DetRemision.Lote,Articulo.MinimoVenta,DetRemision.IDRemision,DetRemision.Suministro,Articulo.Descripcion as Articulo,DetRemision.Cantidad,DetRemision.Costo,DetRemision.CantidadPedida,DetRemision.Descuento,DetRemision.Importe,DetRemision.IVA,DetRemision.ImporteIva,DetRemision.ImporteTotal,DetRemision.Nota,DetRemision.Ordenado, Caracteristica.Presentacion as Presentacion,Caracteristica.jsonPresentacion as jsonPresentacion, DetRemision.Status, DetRemision.Caracteristica_ID,Detremision.IDAlmacen from  DetRemision INNER JOIN Caracteristica ON DetRemision.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo where IDRemision='" + id + "'").ToList();

            ViewBag.req = orden;

            var filtro = BD.Database.SqlQuery<ResumenFac>("select (select ClaveMoneda FROM C_MONEDA WHERE IDMoneda=EncRemision.IDMoneda) as MonedaOrigen, (SUM(Subtotal)) as Subtotal, (SUM(IVA)) as IVA, (SUM(Total)) as Total, (SUM(Total * TipoCambio)) as TotalenPesos from EncRemision inner join Clientes on Clientes.IDCliente=EncRemision.IDCliente  where EncRemision.IDRemision='" + id + "' group by EncRemision.IDMoneda ").ToList();
            ViewBag.sumatoria = filtro;


            EncRemision encRemision = BD.EncRemisiones.Find(id);

            return View(encRemision);
        }
        public ActionResult CancelarDevolucionRemi(int? id)
        {
            RemisionContext bd = new RemisionContext();
            string fecha = DateTime.Now.ToString("yyyyMMdd");

            EncRemision encRemision = bd.EncRemisiones.Find(id);
            List<DetDevolucionR> remision = BD.Database.SqlQuery<DetDevolucionR>("select distinct(dd.iddetdevolucionr), dd.* from detdevolucionr as dd inner join detremision as dr on dd.idremision=dr.idremision where dd.IDDevolucionR='" + id + "'  and dd.Status='Activo'").ToList();

            ViewBag.req = remision;



            foreach (var details in remision)
            {
                Caracteristica caracteristica = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from Caracteristica where id=" + details.Caracteristica_ID).ToList().FirstOrDefault();
                Articulo articulodetalle = new ArticuloContext().Articulo.Find(caracteristica.Articulo_IDArticulo);

                if (articulodetalle.esKit)
                {

                    InventarioAlmacen invKit = new InventarioAlmacenContext().InventarioAlmacenes.ToList().Where(a => a.IDAlmacen == details.IDAlmacen && a.IDCaracteristica == details.Caracteristica_ID).FirstOrDefault();
                    decimal existenciakit = 0;

                    if (invKit != null)
                    {
                        /// kit

                        db.Database.ExecuteSqlCommand("update InventarioAlmacen set Existencia=(Existencia+" + details.Cantidad + ") where IDArticulo= " + articulodetalle.IDArticulo + " and IDCaracteristica=" + details.Caracteristica_ID + " and IDAlmacen=" + details.IDAlmacen + "");
                        db.Database.ExecuteSqlCommand("update InventarioAlmacen set Disponibilidad=(Existencia-Apartado) where IDArticulo= " + articulodetalle.IDArticulo + " and IDCaracteristica=" + details.Caracteristica_ID + " and IDAlmacen=" + details.IDAlmacen + "");

                        // existenciakit = invKit.Existencia;
                    }

                    try
                    {
                        // movimiento kit
                        Caracteristica carateristicakit = new WorkinProcessContext().Database.SqlQuery<Caracteristica>("select * from caracteristica where id=" + details.Caracteristica_ID).ToList().FirstOrDefault();

                        string cadenam = "INSERT INTO [dbo].[MovimientoArticulo] ([Fecha],[Id],[IDPresentacion],[Articulo_IDArticulo],[Accion],[cantidad],[Documento],[NoDocumento],[Lote],[IDAlmacen],[TipoOperacion],[acumulado],[observacion],[Hora]) VALUES ";
                        cadenam += " (getdate(), " + carateristicakit.ID + "," + carateristicakit.IDPresentacion + "," + articulodetalle.IDArticulo + ",'Devolución Remisión',";
                        cadenam += details.Cantidad + ",' Cancelación Devolución Remisión Kit'," + details.IDRemision + ",'',";
                        cadenam += details.IDAlmacen + ",'N/A',0,'',CONVERT (time, SYSDATETIMEOFFSET()))";

                        db.Database.ExecuteSqlCommand(cadenam);
                    }
                    catch (Exception err)
                    {

                    }


                    if (articulodetalle.CtrlStock)
                    {
                        var kits = new KitContext().Database.SqlQuery<Kit>("select * from kit where IDARTICULO=" + articulodetalle.IDArticulo).ToList();
                        foreach (Kit kista in kits)
                        {
                            Articulo articulocomponente = new ArticuloContext().Articulo.Find(kista.IDArticuloComp);

                            //if (articulocomponente.CtrlStock)
                            //{

                            //    RemisionContext db = new RemisionContext();
                            //    try
                            //    {
                            //        ClsDatoDecimal cantidadAK = db.Database.SqlQuery<ClsDatoDecimal>("select cantidadnue as Dato from DetRemisionKit where IDRemision =" + id + " and idarticulo=" + kista.IDArticuloComp + "and caracteristica_ID=" + kista.IDCaracteristica).ToList()[0];
                            //        ClsDatoDecimal cantidadAKAN = db.Database.SqlQuery<ClsDatoDecimal>("select cantidadAnt as Dato from DetRemisionKit where IDRemision =" + id + " and idarticulo=" + kista.IDArticuloComp + "and caracteristica_ID=" + kista.IDCaracteristica).ToList()[0];


                            //        InventarioAlmacen inv = new InventarioAlmacenContext().InventarioAlmacenes.ToList().Where(s => s.IDAlmacen == details.IDAlmacen && s.IDCaracteristica == kista.IDCaracteristica).FirstOrDefault();
                            //        if (inv != null)
                            //        {
                            //            db.Database.ExecuteSqlCommand("UPDATE dbo.InventarioAlmacen SET Existencia =(Existencia+" + cantidadAK.Dato + ")  WHERE IDAlmacen = " + kista.IDAlmacen + " and IDCaracteristica =" + kista.IDCaracteristica);
                            //            db.Database.ExecuteSqlCommand("UPDATE dbo.InventarioAlmacen SET  Apartado =(Apartado+ " + cantidadAKAN.Dato + ") WHERE IDAlmacen = " + kista.IDAlmacen + " and IDCaracteristica =" + kista.IDCaracteristica);

                            //            db.Database.ExecuteSqlCommand("UPDATE dbo.InventarioAlmacen SET  Disponibilidad = (Existencia-Apartado) WHERE IDAlmacen = " + details.IDAlmacen + " and IDCaracteristica =" + kista.IDCaracteristica);

                            //            Caracteristica caracteristicak = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from Caracteristica where id=" + kista.IDCaracteristica).ToList().FirstOrDefault();
                            //            InventarioAlmacen inv1 = new InventarioAlmacenContext().InventarioAlmacenes.ToList().Where(s => s.IDAlmacen == kista.IDAlmacen && s.IDCaracteristica == kista.IDCaracteristica).FirstOrDefault();

                            //            string cadenam = "INSERT INTO [dbo].[MovimientoArticulo] ([Fecha],[Id],[IDPresentacion],[Articulo_IDArticulo],[Accion],[cantidad],[Documento],[NoDocumento],[Lote],[IDAlmacen],[TipoOperacion],[acumulado],[observacion],[Hora]) VALUES ";
                            //            cadenam += " (getdate(), " + caracteristicak.ID + "," + caracteristicak.IDPresentacion + "," + caracteristicak.Articulo_IDArticulo + ",'Cancelación Remisión'," + cantidadAK.Dato + ",'Remisión '," + details.IDRemision + ",''," + details.IDAlmacen + ",'E'," + inv1.Existencia + ",'',CONVERT (time, SYSDATETIMEOFFSET()))";
                            //            db.Database.ExecuteSqlCommand(cadenam);



                            //        }
                            //    }
                            //    catch (Exception err )
                            //    {

                            //    }

                            //}

                        }

                    }
                }
                else
                {
                    InventarioAlmacen inv = new InventarioAlmacenContext().InventarioAlmacenes.ToList().Where(s => s.IDAlmacen == details.IDAlmacen && s.IDCaracteristica == details.Caracteristica_ID).FirstOrDefault();
                    if (inv != null)
                    {



                        db.Database.ExecuteSqlCommand("UPDATE dbo.InventarioAlmacen SET Existencia =(Existencia-" + details.Cantidad + ") WHERE IDAlmacen = " + details.IDAlmacen + " and IDCaracteristica =" + details.Caracteristica_ID);
                        //db.Database.ExecuteSqlCommand("UPDATE dbo.InventarioAlmacen SET  Apartado =(Apartado+ " + details.Cantidad + ") WHERE IDAlmacen = " + details.IDAlmacen + " and IDCaracteristica =" + details.Caracteristica_ID);

                        db.Database.ExecuteSqlCommand("UPDATE dbo.InventarioAlmacen SET  Disponibilidad = (Existencia-Apartado) WHERE IDAlmacen = " + details.IDAlmacen + " and IDCaracteristica =" + details.Caracteristica_ID);

                        InventarioAlmacen inv1 = new InventarioAlmacenContext().InventarioAlmacenes.ToList().Where(s => s.IDAlmacen == details.IDAlmacen && s.IDCaracteristica == details.Caracteristica_ID).FirstOrDefault();

                        string cadenam = "INSERT INTO [dbo].[MovimientoArticulo] ([Fecha],[Id],[IDPresentacion],[Articulo_IDArticulo],[Accion],[cantidad],[Documento],[NoDocumento],[Lote],[IDAlmacen],[TipoOperacion],[acumulado],[observacion],[Hora]) VALUES ";
                        cadenam += " (getdate(), " + caracteristica.ID + "," + caracteristica.IDPresentacion + "," + caracteristica.Articulo_IDArticulo + ",'Cancelación Devolución Remisión'," + details.Cantidad + ",'Devolución Remisión '," + details.IDRemision + ",''," + details.IDAlmacen + ",'S'," + inv1.Existencia + ",'',CONVERT (time, SYSDATETIMEOFFSET()))";
                        db.Database.ExecuteSqlCommand(cadenam);


                    }


                    db.Database.ExecuteSqlCommand("UPDATE dbo.detremision SET devolucion =(devolucion-" + details.Cantidad + ") WHERE IDDetRemision = " + details.IDDetRemision);



                }



            }




            db.Database.ExecuteSqlCommand("update EncDevolucionR set [Status]='Cancelado' where IDDevolucionR='" + id + "'");
            db.Database.ExecuteSqlCommand("update DetDevolucionR set [Status]='Cancelado'  where IDDevolucionR='" + id + "'");





            return RedirectToAction("IndexDevolucionR", new { Numero = id });



        }

        public ActionResult Cancelar(int? id)
        {
            RemisionContext bd = new RemisionContext();
            string fecha = DateTime.Now.ToString("yyyyMMdd");

            EncRemision encRemision = bd.EncRemisiones.Find(id);
            List<VDetRemision> remision = BD.Database.SqlQuery<VDetRemision>("select DetRemision.Caracteristica_ID,DetRemision.Devolucion,DetRemision.Lote,Articulo.MinimoCompra,DetRemision.IDRemision,DetRemision.Suministro,Articulo.Descripcion as Articulo,DetRemision.Cantidad,DetRemision.Costo,DetRemision.CantidadPedida,DetRemision.Descuento,DetRemision.Importe,DetRemision.IVA,DetRemision.ImporteIva,DetRemision.ImporteTotal,DetRemision.Nota,DetRemision.Ordenado, Caracteristica.Presentacion as Presentacion,Caracteristica.jsonPresentacion as jsonPresentacion, DetRemision.Status,Detremision.IDAlmacen from  DetRemision INNER JOIN Caracteristica ON DetRemision.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo where IDRemision='" + id + "'  and Status='Activo'").ToList();

            ViewBag.req = remision;
            //for (int i = 0; i < remision.Count(); i++)
            //{
            //    db.Database.ExecuteSqlCommand("exec dbo.CancelaMovArt'" + fecha + "'," + ViewBag.req[i].Caracteristica_ID + ",'CanRemVta'," + ViewBag.req[i].Cantidad + ",'Remisión'," + id + ",'" + ViewBag.req[i].Cantidad + "','" + encRemision.IDAlmacen + "','" + ViewBag.req[i].Nota + "',0");
            //}


            foreach (var details in remision)
            {
                Caracteristica caracteristica = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from Caracteristica where id=" + details.Caracteristica_ID).ToList().FirstOrDefault();
                Articulo articulodetalle = new ArticuloContext().Articulo.Find(caracteristica.Articulo_IDArticulo);

                if (articulodetalle.esKit)
                {

                    InventarioAlmacen invKit = new InventarioAlmacenContext().InventarioAlmacenes.ToList().Where(a => a.IDAlmacen == details.IDAlmacen && a.IDCaracteristica == details.Caracteristica_ID).FirstOrDefault();
                    decimal existenciakit = 0;

                    if (invKit != null)
                    {
                        /// kit
                        db.Database.ExecuteSqlCommand("update InventarioAlmacen set Apartado=" + details.Cantidad + ", Disponibilidad=(0-" + details.Cantidad + ") where IDArticulo= " + articulodetalle.IDArticulo + " and IDCaracteristica=" + details.Caracteristica_ID + " and IDAlmacen=" + details.IDAlmacen + "");

                        // existenciakit = invKit.Existencia;
                    }

                    try
                    {
                        // movimiento kit
                        Caracteristica carateristicakit = new WorkinProcessContext().Database.SqlQuery<Caracteristica>("select * from caracteristica where id=" + details.Caracteristica_ID).ToList().FirstOrDefault();

                        string cadenam = "INSERT INTO [dbo].[MovimientoArticulo] ([Fecha],[Id],[IDPresentacion],[Articulo_IDArticulo],[Accion],[cantidad],[Documento],[NoDocumento],[Lote],[IDAlmacen],[TipoOperacion],[acumulado],[observacion],[Hora]) VALUES ";
                        cadenam += " (getdate(), " + carateristicakit.ID + "," + carateristicakit.IDPresentacion + "," + articulodetalle.IDArticulo + ",'Remisión',";
                        cadenam += details.Cantidad + ",' Cancelación Remisión Kit'," + details.IDRemision + ",'',";
                        cadenam += details.IDAlmacen + ",'N/A',0,'',CONVERT (time, SYSDATETIMEOFFSET()))";

                        db.Database.ExecuteSqlCommand(cadenam);
                    }
                    catch (Exception err)
                    {

                    }


                    if (articulodetalle.CtrlStock)
                    {
                        var kits = new KitContext().Database.SqlQuery<Kit>("select * from kit where IDARTICULO=" + articulodetalle.IDArticulo).ToList();
                        foreach (Kit kista in kits)
                        {

                            decimal cantidadreal = 0;
                            if (kista.porcantidad == true)
                            {
                                cantidadreal = details.Suministro * kista.Cantidad;
                            }
                            else if (kista.porporcentaje == true)
                            {
                                cantidadreal = details.Suministro * (kista.Cantidad / 100);
                            }

                            InventarioAlmacen inv = new InventarioAlmacenContext().InventarioAlmacenes.ToList().Where(s => s.IDAlmacen == details.IDAlmacen && s.IDCaracteristica == kista.IDCaracteristica).FirstOrDefault();
                            if (inv != null)
                            {
                                db.Database.ExecuteSqlCommand("UPDATE dbo.InventarioAlmacen SET Existencia =(Existencia+" + cantidadreal + ")  WHERE IDAlmacen = " + details.IDAlmacen + " and IDCaracteristica =" + kista.IDCaracteristica);
                                db.Database.ExecuteSqlCommand("UPDATE dbo.InventarioAlmacen SET  Apartado =(Apartado+ " + cantidadreal + ") WHERE IDAlmacen = " + details.IDAlmacen + " and IDCaracteristica =" + kista.IDCaracteristica);

                                db.Database.ExecuteSqlCommand("UPDATE dbo.InventarioAlmacen SET  Disponibilidad = (Existencia-Apartado) WHERE IDAlmacen = " + details.IDAlmacen + " and IDCaracteristica =" + kista.IDCaracteristica);

                                Caracteristica caracteristicak = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from Caracteristica where id=" + kista.IDCaracteristica).ToList().FirstOrDefault();

                                string cadenam = "INSERT INTO [dbo].[MovimientoArticulo] ([Fecha],[Id],[IDPresentacion],[Articulo_IDArticulo],[Accion],[cantidad],[Documento],[NoDocumento],[Lote],[IDAlmacen],[TipoOperacion],[acumulado],[observacion],[Hora]) VALUES ";
                                cadenam += " (getdate(), " + caracteristicak.ID + "," + caracteristicak.IDPresentacion + "," + caracteristicak.Articulo_IDArticulo + ",'Cancelación Remisión'," + cantidadreal + ",'Remisión '," + details.IDRemision + ",''," + details.IDAlmacen + ",'E'," + (inv.Existencia + cantidadreal) + ",'',CONVERT (time, SYSDATETIMEOFFSET()))";
                                db.Database.ExecuteSqlCommand(cadenam);



                            }
                        }

                    }
                }
                else
                {
                    InventarioAlmacen inv = new InventarioAlmacenContext().InventarioAlmacenes.ToList().Where(s => s.IDAlmacen == details.IDAlmacen && s.IDCaracteristica == details.Caracteristica_ID).FirstOrDefault();
                    if (inv != null)
                    {
                        string cadenam = "INSERT INTO [dbo].[MovimientoArticulo] ([Fecha],[Id],[IDPresentacion],[Articulo_IDArticulo],[Accion],[cantidad],[Documento],[NoDocumento],[Lote],[IDAlmacen],[TipoOperacion],[acumulado],[observacion],[Hora]) VALUES ";
                        cadenam += " (getdate(), " + caracteristica.ID + "," + caracteristica.IDPresentacion + "," + caracteristica.Articulo_IDArticulo + ",'Cancelación Remisión'," + details.Suministro + ",'Remisión '," + details.IDRemision + ",''," + details.IDAlmacen + ",'E'," + (inv.Existencia + details.Cantidad) + ",'',CONVERT (time, SYSDATETIMEOFFSET()))";
                        db.Database.ExecuteSqlCommand(cadenam);



                        db.Database.ExecuteSqlCommand("UPDATE dbo.InventarioAlmacen SET Existencia =(Existencia+" + details.Suministro + ") WHERE IDAlmacen = " + details.IDAlmacen + " and IDCaracteristica =" + details.Caracteristica_ID);
                        db.Database.ExecuteSqlCommand("UPDATE dbo.InventarioAlmacen SET  Apartado =(Apartado+ " + details.Cantidad + ") WHERE IDAlmacen = " + details.IDAlmacen + " and IDCaracteristica =" + details.Caracteristica_ID);

                        db.Database.ExecuteSqlCommand("UPDATE dbo.InventarioAlmacen SET  Disponibilidad = (Existencia-Apartado) WHERE IDAlmacen = " + details.IDAlmacen + " and IDCaracteristica =" + details.Caracteristica_ID);

                    }
                }



            }




            db.Database.ExecuteSqlCommand("update EncRemision set [Status]='Cancelado' where IDRemision='" + id + "' and Status='Activo'");
            db.Database.ExecuteSqlCommand("update DetRemision set [Status]='Cancelado'  where IDRemision='" + id + "' and Status='Activo'");




            db.Database.ExecuteSqlCommand("update EncPedido set [Status]='Activo' where [IDPedido] in (select IDExterna from DetRemision   where IDRemision=" + id + ")");

            this.Activardetallepedido(id);

            //  List<VDetRemision> remision1 = BD.Database.SqlQuery<VDetRemision>("select DetRemision.Caracteristica_ID,DetRemision.Devolucion,DetRemision.Lote,Articulo.MinimoCompra,DetRemision.IDRemision,DetRemision.Suministro,Articulo.Descripcion as Articulo,DetRemision.Cantidad,DetRemision.Costo,DetRemision.CantidadPedida,DetRemision.Descuento,DetRemision.Importe,DetRemision.IVA,DetRemision.ImporteIva,DetRemision.ImporteTotal,DetRemision.Nota,DetRemision.Ordenado, Caracteristica.Presentacion as Presentacion,Caracteristica.jsonPresentacion as jsonPresentacion, DetRemision.Status,DetResimion.IDAlmacen from  DetRemision INNER JOIN Caracteristica ON DetRemision.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo where IDRemision='" + id + "'  and idexterna=0").ToList();





            return RedirectToAction("Index", new { Numero = id });



        }
        //public ActionResult Cancelar(int? id)
        //{
        //    db.Database.ExecuteSqlCommand("update EncRemision set [Status]='Cancelado' where IDRemision='" + id + "'");
        //    db.Database.ExecuteSqlCommand("update DetRemision set [Status]='Cancelado'  where IDRemision='" + id + "'");
        //    return RedirectToAction("Index");
        //}
        public JsonResult getmetodo(int id)
        {
            var metodo = prov.Clientes.Where(x => x.IDCliente == id).ToList();
            int numero = 0;
            List<SelectListItem> listmetodo = new List<SelectListItem>();

            //listmetodo.Add(new SelectListItem { Text = "--Select metodo de pago--", Value = "0" });
            if (metodo != null)
            {
                foreach (var x in metodo)
                {
                    numero = x.IDMetodoPago;
                    listmetodo.Add(new SelectListItem { Text = x.c_MetodoPago.Descripcion, Value = x.IDMetodoPago.ToString() });

                }

            }
            listmetodo.Add(new SelectListItem { Text = "-------------------------", Value = "0" });
            var todosmetodo = prov.c_MetodoPagos.ToList();
            if (todosmetodo != null)
            {
                foreach (var x in todosmetodo)
                {
                    listmetodo.Add(new SelectListItem { Text = x.Descripcion, Value = x.IDMetodoPago.ToString() });

                }

            }
            return Json(new SelectList(listmetodo, "Value", "Text", JsonRequestBehavior.AllowGet));
        }


        public JsonResult getforma(int id)
        {
            var forma = prov.Clientes.Where(x => x.IDCliente == id).ToList();
            List<SelectListItem> listforma = new List<SelectListItem>();
            if (forma != null)
            {
                foreach (var x in forma)
                {
                    listforma.Add(new SelectListItem { Text = x.c_FormaPago.Descripcion, Value = x.IDFormapago.ToString() });
                }
            }
            listforma.Add(new SelectListItem { Text = "-------------------------", Value = "0" });
            var todosforma = prov.c_FormaPagos.ToList();
            if (todosforma != null)
            {
                foreach (var x in todosforma)
                {
                    listforma.Add(new SelectListItem { Text = x.Descripcion, Value = x.IDFormaPago.ToString() });
                }
            }
            return Json(new SelectList(listforma, "Value", "Text", JsonRequestBehavior.AllowGet));
        }
        public JsonResult getmoneda(int id)
        {
            var moneda = prov.Clientes.Where(x => x.IDCliente == id).ToList();
            List<SelectListItem> listmoneda = new List<SelectListItem>();
            if (moneda != null)
            {
                foreach (var x in moneda)
                {
                    listmoneda.Add(new SelectListItem { Text = x.c_Moneda.Descripcion, Value = x.IDMoneda.ToString() });
                }
            }
            listmoneda.Add(new SelectListItem { Text = "-------------------------", Value = "0" });
            var todosmoneda = prov.c_Monedas.ToList();
            if (todosmoneda != null)
            {
                foreach (var x in todosmoneda)
                {
                    listmoneda.Add(new SelectListItem { Text = x.Descripcion, Value = x.IDMoneda.ToString() });
                }
            }
            return Json(new SelectList(listmoneda, "Value", "Text", JsonRequestBehavior.AllowGet));
        }

        public JsonResult getcondiciones(int id)
        {
            var condiciones = prov.Clientes.Where(x => x.IDCliente == id).ToList();
            List<SelectListItem> listcondiciones = new List<SelectListItem>();
            if (condiciones != null)
            {
                foreach (var x in condiciones)
                {
                    listcondiciones.Add(new SelectListItem { Text = x.CondicionesPago.Descripcion, Value = x.IDCondicionesPago.ToString() });
                }
            }
            listcondiciones.Add(new SelectListItem { Text = "-------------------------", Value = "0" });
            var todoscondiciones = prov.CondicionesPagos.ToList();
            if (todoscondiciones != null)
            {
                foreach (var x in todoscondiciones)
                {
                    listcondiciones.Add(new SelectListItem { Text = x.Descripcion, Value = x.IDCondicionesPago.ToString() });
                }
            }
            return Json(new SelectList(listcondiciones, "Value", "Text", JsonRequestBehavior.AllowGet));
        }

        public ActionResult Pedido(int? id)
        {
            ViewBag.moneda = null;
            ViewBag.metodo = null;
            ViewBag.forma = null;
            ViewBag.condiciones = null;
            ViewBag.proveedor = null;
            ViewBag.entrega = null;
            ViewBag.tipoentrega = null;
            ViewBag.id = id;
            List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            int UserID = userid.Select(s => s.UserID).FirstOrDefault();

            List<VEncPedido> orden = db.Database.SqlQuery<VEncPedido>("select EncPedido.IDPedido, CONVERT(VARCHAR(10),EncPedido.Fecha,103) AS Fecha,CONVERT(VARCHAR(10),EncPedido.FechaRequiere,103) AS FechaRequiere,Clientes.Nombre as Cliente from EncPedido INNER JOIN Clientes ON EncPedido.IDCliente=Clientes.IDCliente where  EncPedido.IDPedido='" + id + "'").ToList();
            ViewBag.data = orden;

            ClsDatoEntero denc = db.Database.SqlQuery<ClsDatoEntero>("select count(IDPedido) as Dato from EncPedido where IDPedido='" + id + "' and (Status='Activo' or Status ='PreFacturado')").ToList()[0];
            ViewBag.otro = denc.Dato;

            List<VDetPedido> elementos = db.Database.SqlQuery<VDetPedido>("select DetPedido.GeneraOrdenP,DetPedido.IDRemision,DetPedido.IDPrefactura,DetPedido.IDDetPedido,DetPedido.IDPedido,DetPedido.Suministro,Articulo.Descripcion as Articulo,DetPedido.Cantidad,DetPedido.Costo,DetPedido.CantidadPedida,DetPedido.Descuento,DetPedido.Importe,DetPedido.IVA,DetPedido.ImporteIva,DetPedido.ImporteTotal, DetPedido.Nota,DetPedido.Ordenado, Caracteristica.Presentacion as Presentacion,Caracteristica.jsonPresentacion as jsonPresentacion  from  DetPedido INNER JOIN Caracteristica ON DetPedido.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo where IDPedido='" + id + "' and (Status='Activo' or Status='PreFacturado')").ToList();
            ViewBag.datos = elementos;

            ClsDatoEntero dcompra = db.Database.SqlQuery<ClsDatoEntero>("select count(IDPedido) as Dato from DetPedido where IDPedido='" + id + "' and (Status='Activo' or Status='PreFacturado')").ToList()[0];


            db.Database.ExecuteSqlCommand("delete from CarritoRemision where userID= " + UserID);

            if (id != null && denc.Dato > 0 && dcompra.Dato > 0 && UserID != 0)
            {
                for (int i = 0; i < elementos.Count(); i++)
                {
                    PedidoContext dboc = new PedidoContext();
                    DetPedido detPedido = dboc.DetPedido.Find(ViewBag.datos[i].IDDetPedido);

                    decimal importe = ViewBag.datos[i].CantidadPedida * ViewBag.datos[i].Costo;
                    decimal importeiva = importe * (decimal)0.16;
                    decimal importetotal = importeiva + importe;


                    Articulo ARTICULO = new ArticuloContext().Articulo.Find(detPedido.IDArticulo);

                    int IDC = detPedido.IDArticulo;
                    SIAAPI.Models.Comercial.Articulo articulo = new SIAAPI.Models.Comercial.ArticuloContext().Articulo.Find(IDC);
                    if (articulo.esKit)
                    {
                        decimal suministro = (detPedido.Cantidad - detPedido.Suministro);
                        if (suministro < 0)
                        {
                            suministro = 0;
                        }
                        db.Database.ExecuteSqlCommand("INSERT INTO CarritoRemision([IDDetExterna],[IDPedido],[IDArticulo],[Cantidad],[Costo],[CantidadPedida],[Descuento],[Importe],[IVA],[ImporteIva],[ImporteTotal],[Nota],[Ordenado],[Caracteristica_ID],[IDAlmacen],[Suministro],[Status],[Presentacion],[jsonPresentacion])   values ('" + ViewBag.datos[i].IDDetPedido + "','" + ViewBag.datos[i].IDPedido + "','" + detPedido.IDArticulo + "','" + detPedido.Cantidad + "','" + detPedido.Costo + "'," + suministro + ",'" + detPedido.Descuento + "','" + importe + "','" + detPedido.IVA + "','" + importeiva + "','" + importetotal + "','" + detPedido.Nota + "','" + detPedido.Ordenado + "','" + detPedido.Caracteristica_ID + "','" + detPedido.IDAlmacen + "','" + detPedido.Suministro + "','Activo','" + ViewBag.datos[i].Presentacion + "', '" + ViewBag.datos[i].jsonPresentacion + "')");

                    }
                    else
                    {
                        decimal suministro = (detPedido.Cantidad - detPedido.Suministro);
                        if (suministro < 0)
                        {
                            suministro = 0;
                        }
                        db.Database.ExecuteSqlCommand("INSERT INTO CarritoRemision([IDDetExterna],[IDPedido],[IDArticulo],[Cantidad],[Costo],[CantidadPedida],[Descuento],[Importe],[IVA],[ImporteIva],[ImporteTotal],[Nota],[Ordenado],[Caracteristica_ID],[IDAlmacen],[Suministro],[Status],[Presentacion],[jsonPresentacion])   values ('" + ViewBag.datos[i].IDDetPedido + "','" + ViewBag.datos[i].IDPedido + "','" + detPedido.IDArticulo + "','" + detPedido.Cantidad + "','" + detPedido.Costo + "'," + suministro + ",'" + detPedido.Descuento + "','" + importe + "','" + detPedido.IVA + "','" + importeiva + "','" + importetotal + "','" + detPedido.Nota + "','" + detPedido.Ordenado + "','" + detPedido.Caracteristica_ID + "','" + detPedido.IDAlmacen + "','" + detPedido.Suministro + "','Activo','" + ViewBag.datos[i].Presentacion + "', '" + ViewBag.datos[i].jsonPresentacion + "')");

                    }

                    //}


                }

                db.Database.ExecuteSqlCommand("update [dbo].[CarritoRemision] set [UserID]='" + UserID + "' where [IDPedido]='" + id + "'");
            }
          
            ClientesContext pr = new ClientesContext();
            EncPedido encPedido = db.EncPedidos.Find(id);

            List<SelectListItem> usocfdi = new List<SelectListItem>();
            c_UsoCFDI usocfdip = pr.c_UsoCFDIS.Find(encPedido.IDUsoCFDI);
            usocfdi.Add(new SelectListItem { Text = usocfdip.Descripcion, Value = usocfdip.IDUsoCFDI.ToString() });
            ViewBag.usocfdi = usocfdi;

            //ViewBag.IDUsoCFDI = new SelectList(db.c_UsoCFDIS.Where(s => s.IDUsoCFDI.Equals(encPedido.IDUsoCFDI)), "IDUsoCFDI", "Descripcion");
            //ViewBag.IDAlmacen = new SelectList(db.Almacenes, "IDAlmacen", "Descripcion",encPedido.IDAlmacen);
            ViewBag.IDAlmacenP = db.Almacenes.ToList();

            var cliente = prov.Clientes.ToList();

            List<SelectListItem> moneda = new List<SelectListItem>();
            c_Moneda monedap = pr.c_Monedas.Find(encPedido.IDMoneda);
            moneda.Add(new SelectListItem { Text = monedap.Descripcion, Value = monedap.IDMoneda.ToString() });
            moneda.Add(new SelectListItem { Text = "-------------------------", Value = "0" });
            var todosmoneda = prov.c_Monedas.ToList();
            if (todosmoneda != null)
            {
                foreach (var y in todosmoneda)
                {
                    moneda.Add(new SelectListItem { Text = y.Descripcion, Value = y.IDMoneda.ToString() });
                }
            }

            ViewBag.moneda = moneda;

            List<SelectListItem> metodo = new List<SelectListItem>();
            c_MetodoPago metodop = pr.c_MetodoPagos.Find(encPedido.IDMetodoPago);
            metodo.Add(new SelectListItem { Text = metodop.Descripcion, Value = metodop.IDMetodoPago.ToString() });
            ViewBag.metodo = metodo;

            List<SelectListItem> forma = new List<SelectListItem>();
            c_FormaPago formap = pr.c_FormaPagos.Find(encPedido.IDFormapago);
            forma.Add(new SelectListItem { Text = formap.Descripcion, Value = formap.IDFormaPago.ToString() });
            ViewBag.forma = forma;

            List<SelectListItem> condiciones = new List<SelectListItem>();
            CondicionesPago condicionesp = pr.CondicionesPagos.Find(encPedido.IDCondicionesPago);
            condiciones.Add(new SelectListItem { Text = condicionesp.Descripcion, Value = condicionesp.IDCondicionesPago.ToString() });
            ViewBag.condiciones = condiciones;

            List<SelectListItem> vendedor = new List<SelectListItem>();
            Vendedor vendedorp = pr.Vendedores.Find(encPedido.IDVendedor);
            vendedor.Add(new SelectListItem { Text = vendedorp.Nombre, Value = vendedorp.IDVendedor.ToString() });
            ViewBag.vendedor = vendedor;

            List<SelectListItem> li = new List<SelectListItem>();
            Clientes mm = pr.Clientes.Find(encPedido.IDCliente);
            li.Add(new SelectListItem { Text = mm.Nombre, Value = mm.IDCliente.ToString() });
            ViewBag.cliente = li;

            List<SelectListItem> entrega = new List<SelectListItem>();
            entrega.Add(new SelectListItem { Text = encPedido.Entrega, Value = encPedido.Entrega });
            ViewBag.entrega = entrega;



            List<VCarritoRemision> lista = db.Database.SqlQuery<VCarritoRemision>("select CarritoRemision.IDArticulo, CarritoRemision.Lote,CarritoRemision.IDDetExterna,CarritoRemision.IDCarritoRemision,CarritoRemision.IDPedido,CarritoRemision.Suministro,Articulo.Descripcion as Articulo,CarritoRemision.Cantidad,CarritoRemision.Costo,CarritoRemision.CantidadPedida,CarritoRemision.Descuento,CarritoRemision.Importe,CarritoRemision.IVA,CarritoRemision.ImporteIva,CarritoRemision.ImporteTotal,CarritoRemision.Nota,CarritoRemision.Ordenado, Caracteristica.Presentacion as Presentacion,Caracteristica.ID as IDCaracteristica,Caracteristica.jsonPresentacion as jsonPresentacion, CarritoRemision.IDAlmacen as IDAlmacen  from  CarritoRemision INNER JOIN Caracteristica ON CarritoRemision.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo where UserID='" + UserID + "' and IDPedido=" + id + "").ToList();
            ClsDatoEntero c = db.Database.SqlQuery<ClsDatoEntero>("select count(IDCarritoRemision) as Dato from CarritoRemision where  UserID='" + UserID + "' and IDPedido='" + id + "'").ToList()[0];
            ViewBag.dato = c.Dato;
            var resumen = db.Database.SqlQuery<ResumenFac>("select (select ClaveMoneda FROM C_MONEDA WHERE IDMoneda=EncPedido.IDMoneda) as MonedaOrigen, (SUM(Importe)) as Subtotal, (SUM(ImporteIva)) as IVA, (SUM(ImporteTotal)) as Total, (SUM(ImporteTotal * EncPedido.TipoCambio)) as TotalenPesos from CarritoRemision inner join EncPedido on EncPedido.IDPedido=CarritoRemision.IDPedido where CarritoRemision.UserID='" + UserID + "' and CarritoRemision.IDPedido='" + id + "' group by EncPedido.IDMoneda").ToList();
            ViewBag.sumatoria = resumen;

           

            return View(lista);




        }



        [HttpPost]
        public ActionResult update(int? id, string Fecha, string Proviene, int? IDMoneda, int? IDCliente, int? IDAlmacen, int? IDFormaPago, int? IDCondicionesPago, int? IDMetodoPago, int? IDUsoCFDI, int? IDVendedor, List<VCarritoRemision> cr, string Entrega = "", string Observacion = "")
        {
            EncPedido pedido = new PedidoContext().EncPedidos.Find(id);
            bool tienecintas = false;
            bool tieneKits = false;
            foreach (VCarritoRemision elev in cr)
            {
                Caracteristica cara = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from Caracteristica where id=" + elev.IDCaracteristica).ToList().FirstOrDefault();

                Articulo ar = new ArticuloContext().Articulo.Find(cara.Articulo_IDArticulo);

                if (ar.esKit)
                {
                    if (elev.Suministro > 0)
                    {
                        if (ar.CtrlStock)
                        {
                            var kits = new KitContext().Database.SqlQuery<Kit>("select * from kit where IDARTICULO=" + ar.IDArticulo).ToList();

                            foreach (Kit kitc in kits)
                            {
                                Articulo articulokit = new ArticuloContext().Articulo.Find(kitc.IDArticuloComp);
                                InventarioAlmacen ia = new InventarioAlmacenContext().InventarioAlmacenes.ToList().Where(s => s.IDAlmacen == elev.IDAlmacen && s.IDCaracteristica == kitc.IDCaracteristica).FirstOrDefault();



                                decimal cantidad = elev.Suministro * kitc.Cantidad;
                                if (ia == null)
                                {

                                    ClsDatoString almacen = db.Database.SqlQuery<ClsDatoString>("select Descripcion as Dato from Almacen where IDAlmacen ='" + elev.IDAlmacen + "'").ToList()[0];



                                    throw new Exception("No hay Existencia en almacen para sutir tu remision " + articulokit.Cref + " en la cantidad de " + cantidad + " Por favor ingresa un invetario inicial del este articulo en el almacen " + almacen.Dato);
                                }
                                try
                                {
                                    Almacen almacen = new AlmacenContext().Almacenes.Find(ia.IDAlmacen);
                                    if (ia.Existencia < cantidad)
                                    {
                                        throw new Exception("No hay Existencia en almacen para sutir tu remision " + articulokit.Cref + " en la cantidad de " + cantidad + " solo hay " + ia.Existencia);
                                    }
                                }
                                catch (Exception err)
                                {

                                    return Content(err.Message);
                                }
                            }




                            foreach (Kit kitc in kits)
                            {
                                Articulo articulokit = new ArticuloContext().Articulo.Find(kitc.IDArticuloComp);
                                InventarioAlmacen ia = new InventarioAlmacenContext().InventarioAlmacenes.ToList().Where(s => s.IDAlmacen == elev.IDAlmacen && s.IDCaracteristica == kitc.IDCaracteristica).FirstOrDefault();



                                decimal cantidad = elev.Suministro * kitc.Cantidad;
                                if (ia == null)
                                {

                                    ClsDatoString almacen = db.Database.SqlQuery<ClsDatoString>("select Descripcion as Dato from Almacen where IDAlmacen ='" + elev.IDAlmacen + "'").ToList()[0];



                                    throw new Exception("No hay Existencia en almacen para sutir tu remision " + articulokit.Cref + " en la cantidad de " + cantidad + " Por favor ingresa un invetario inicial del este articulo en el almacen " + almacen.Dato);
                                }
                                try
                                {
                                    Almacen almacen = new AlmacenContext().Almacenes.Find(ia.IDAlmacen);
                                    if (ia.Existencia < cantidad)
                                    {
                                        throw new Exception("No hay Existencia en almacen para sutir tu remision " + articulokit.Cref + " en la cantidad de " + cantidad + " solo hay " + ia.Existencia);
                                    }
                                }
                                catch (Exception err)
                                {

                                    return Content(err.Message);
                                }
                            }


                        }
                    }
                }
                else
                {
                    if (ar.CtrlStock)
                    {
                        if (elev.Suministro > 0)
                        {
                            InventarioAlmacen ia = new InventarioAlmacenContext().InventarioAlmacenes.ToList().Where(s => s.IDAlmacen == elev.IDAlmacen && s.IDCaracteristica == elev.IDCaracteristica).FirstOrDefault();
                            Almacen almacen = new AlmacenContext().Almacenes.Find(elev.IDAlmacen);
                            if (ia == null)
                            {
                                throw new Exception("No hay Existencia en almacen para sutir tu remision " + elev.Articulo + " en la cantidad de " + elev.Suministro + " Por favor ingresa un invetario inicial del este articulo en el almacen " + almacen.Descripcion);
                            }
                            try
                            {
                                if (ia.Existencia < elev.Suministro)
                                {
                                    throw new Exception("No hay Existencia en almacen para sutir tu remision " + elev.Articulo + " en la cantidad de " + elev.Suministro + " solo hay " + ia.Existencia);
                                }
                            }
                            catch (Exception err)
                            {

                                return Content(err.Message);
                            }
                        }
                    }
                }


            }


            decimal subtotal = 0, iva = 0, total = 0, subtotalr = 0, ivar = 0, totalr = 0, diferencia = 0, Cambio = 0, Precio = 0;
            string fecha = DateTime.Now.ToString("yyyyMMdd");
            List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            int UserID = userid.Select(s => s.UserID).FirstOrDefault();
            db.Database.ExecuteSqlCommand("insert into [dbo].[EncRemision] ([Fecha],[Observacion],[DocumentoFactura],[IDCliente],[IDFormaPago],[IDMoneda],[IDMetodoPago],[IDCondicionesPago],[IDAlmacen],[TipoCambio],[UserID],[IDUsoCFDI],[Subtotal],[IVA],[Total],[Status],[IDVendedor],[Entrega]) values('" + Fecha + "','" + Observacion + "','" + Proviene + "','" + IDCliente + "','" + IDFormaPago + "','" + IDMoneda + "','" + IDMetodoPago + "','" + IDCondicionesPago + "',3,'" + pedido.TipoCambio + "','" + UserID + "','" + IDUsoCFDI + "','0','0','0','Activo','" + IDVendedor + "','" + Entrega + "')");

            List<EncRemision> numero = db.Database.SqlQuery<EncRemision>("select * from [dbo].[EncRemision] WHERE IDRemision = (SELECT MAX(IDRemision) from EncRemision)").ToList();
            int num = numero.Select(s => s.IDRemision).FirstOrDefault();

            List<c_Moneda> clavemoneda = db.Database.SqlQuery<c_Moneda>("select * from c_Moneda WHERE IDMoneda='" + IDMoneda + "'").ToList();
            string clave = clavemoneda.Select(s => s.ClaveMoneda).FirstOrDefault();

            foreach (VCarritoRemision i in cr)
            {
                PedidoContext oc = new PedidoContext();
                DetPedido detPedido = oc.DetPedido.Find(i.IDDetExterna);
                EncPedido encPedido = oc.EncPedidos.Find(i.IDPedido);
                Caracteristica caracteristica = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from Caracteristica where id=" + i.IDCaracteristica).ToList().FirstOrDefault();
                Articulo articulodetalle = new ArticuloContext().Articulo.Find(caracteristica.Articulo_IDArticulo);

                if (articulodetalle.IDTipoArticulo == 6)
                {
                    tienecintas = true;
                }

                if (articulodetalle.esKit)
                {
                    tieneKits = true;
                }
                //Datos para tipo de cambio
                List<c_Moneda> claved = db.Database.SqlQuery<c_Moneda>("select * from c_Moneda WHERE IDMoneda='" + encPedido.IDMoneda + "'").ToList();
                //string clavedet = claved.Select(s => s.ClaveMoneda).FirstOrDefault();
                int dosc = claved.Select(s => s.IDMoneda).FirstOrDefault();

                List<c_Moneda> uno = db.Database.SqlQuery<c_Moneda>("select * from c_Moneda WHERE ClaveMoneda='" + clave + "'").ToList();
                int unoc = uno.Select(s => s.IDMoneda).FirstOrDefault();
                VCambio cambiodet = db.Database.SqlQuery<VCambio>("select dbo.GetTipocambio('" + fecha + "'," + dosc + "," + unoc + ") as TC").ToList()[0];
                Cambio = cambiodet.TC;
                Precio = i.Costo;
                //Precio = i.Costo * Cambio;
                decimal suministro = detPedido.Suministro + i.Suministro;
                if (i.Cantidad == i.Suministro)
                {
                    subtotal = i.Suministro * Precio;
                    iva = subtotal * decimal.Parse(SIAAPI.Properties.Settings.Default.ValorIVA);
                    total = subtotal + iva;


                    db.Database.ExecuteSqlCommand("update [dbo].[DetPedido] set [IDRemision]='" + num + "', [Status]='Finalizado', [Suministro]='" + suministro + "' where [IDDetPedido]='" + i.IDDetExterna + "'");
                    int num2 = 0;
                    int num3 = 0;
                    db.Database.ExecuteSqlCommand("insert into DetRemision([IDRemision],[IDExterna],[IDArticulo],[Cantidad],[Costo],[CantidadPedida],[Descuento],[Importe],[IVA],[ImporteIva],[ImporteTotal],[Nota],[Ordenado],[Caracteristica_ID],[IDAlmacen],[Suministro],[Status],[Presentacion],[jsonPresentacion],[IDDetExterna],[Lote],[Devolucion]) values ('"
                                                                           + num + "','" + detPedido.IDPedido + "','" + detPedido.IDArticulo + "','" + i.Suministro + "','" + Precio + "'," + i.Cantidad + ",'0','" + subtotal + "','true','" + iva + "','" + total + "','" + detPedido.Nota + "','0','" + detPedido.Caracteristica_ID + "','" + detPedido.IDAlmacen + "','" + i.Suministro + "','Activo','" + detPedido.Presentacion + "','" + detPedido.jsonPresentacion + "','" + i.IDDetExterna + "','" + i.Lote + "','0')");

                    List<DetRemision> numero2 = db.Database.SqlQuery<DetRemision>("select * from [dbo].[DetRemision] WHERE IDDetRemision = (SELECT MAX(IDDetRemision) from DetRemision)").ToList();
                    num2 = numero2.Select(s => s.IDRemision).FirstOrDefault();

                    num3 = numero2.Select(s => s.IDDetRemision).FirstOrDefault();

                    db.Database.ExecuteSqlCommand("insert into [dbo].[MovComercial] ([DocumentoDestino],[IDDestino],[Cantidad],[Fecha],[IDOrigen],[DocumentoOrigen],[UserID],[IDDetOrigen],[IDDetDestino]) values('Remisión','" + num + "','" + i.Suministro + "','" + fecha + "','" + i.IDPedido + "','Pedido','" + UserID + "','" + i.IDDetExterna + "','" + num3 + "')");

                    var detallepedido = new PedidoContext().DetPedido.Where(s => s.IDDetPedido == i.IDDetExterna);

                    int AlmacenViene = 0;

                    foreach (var item in detallepedido)
                    {
                        AlmacenViene = item.IDAlmacen;
                    }

                    if (!articulodetalle.esKit && articulodetalle.CtrlStock)
                    {
                        if (AlmacenViene == i.IDAlmacen)
                        {
                            db.Database.ExecuteSqlCommand("update InventarioAlmacen set  Existencia=(Existencia-" + i.Suministro + ") where IDArticulo= " + caracteristica.Articulo_IDArticulo + " and IDCaracteristica=" + i.IDCaracteristica + " and IDAlmacen=" + AlmacenViene + "");
                            db.Database.ExecuteSqlCommand("update InventarioAlmacen set Apartado=(Apartado-" + i.Suministro + ") where IDArticulo= " + caracteristica.Articulo_IDArticulo + " and IDCaracteristica=" + i.IDCaracteristica + " and IDAlmacen=" + AlmacenViene + "");
                            db.Database.ExecuteSqlCommand("update InventarioAlmacen set Apartado=0 where IDArticulo= " + caracteristica.Articulo_IDArticulo + " and IDCaracteristica=" + i.IDCaracteristica + " and IDAlmacen=" + AlmacenViene + " and Apartado<0");
                            db.Database.ExecuteSqlCommand("update InventarioAlmacen set  Disponibilidad=Existencia where apartado=0 and IDArticulo= " + caracteristica.Articulo_IDArticulo + " and IDCaracteristica=" + i.IDCaracteristica + " and IDAlmacen=" + AlmacenViene + "");
                            db.Database.ExecuteSqlCommand("update InventarioAlmacen set  Disponibilidad=(Disponibilidad-" + i.Suministro + ") where Apartado>0 and   IDArticulo= " + caracteristica.Articulo_IDArticulo + " and IDCaracteristica=" + i.IDCaracteristica + " and IDAlmacen=" + AlmacenViene + "");

                            try
                            {

                                 int usuario = userid.Select(s => s.UserID).FirstOrDefault();

                                Caracteristica carateristica = new WorkinProcessContext().Database.SqlQuery<Caracteristica>("select * from caracteristica where id=" + i.IDCaracteristica).ToList().FirstOrDefault();
                                InventarioAlmacen inv = new InventarioAlmacenContext().InventarioAlmacenes.ToList().Where(s => s.IDAlmacen == AlmacenViene && s.IDCaracteristica == i.IDCaracteristica).FirstOrDefault();

                                string cadenam = "INSERT INTO [dbo].[MovimientoArticulo] ([Fecha],[Id],[IDPresentacion],[Articulo_IDArticulo],[Accion],[cantidad],[Documento],[NoDocumento],[Lote],[IDAlmacen],[TipoOperacion],[acumulado],[observacion],[Hora], usuario) VALUES ";
                                cadenam += " (getdate(), " + carateristica.ID + "," + carateristica.IDPresentacion + "," + carateristica.Articulo_IDArticulo + ",'Remisión'," + i.Suministro + ",'Remisión '," + num + ",''," + AlmacenViene + ",'S'," + inv.Existencia + ",'',CONVERT (time, SYSDATETIMEOFFSET()), "+usuario+")";
                                db.Database.ExecuteSqlCommand(cadenam);
                            }
                            catch (Exception err)
                            {
                                string mensajee = err.Message;
                            }

                        }
                        else
                        {

                            db.Database.ExecuteSqlCommand("update InventarioAlmacen set  Existencia=(Existencia-" + i.Suministro + ") where IDArticulo= " + caracteristica.Articulo_IDArticulo + " and IDCaracteristica=" + i.IDCaracteristica + " and IDAlmacen=" + i.IDAlmacen + "");
                            db.Database.ExecuteSqlCommand("update InventarioAlmacen set Apartado=(Apartado-" + i.Suministro + ") where IDArticulo= " + caracteristica.Articulo_IDArticulo + " and IDCaracteristica=" + i.IDCaracteristica + " and IDAlmacen=" + AlmacenViene + "");
                            db.Database.ExecuteSqlCommand("update InventarioAlmacen set Apartado=0 where IDArticulo= " + caracteristica.Articulo_IDArticulo + " and IDCaracteristica=" + i.IDCaracteristica + " and IDAlmacen=" + AlmacenViene + " and Apartado<0");
                            db.Database.ExecuteSqlCommand("update InventarioAlmacen set  Disponibilidad=Existencia where apartado=0 and IDArticulo= " + caracteristica.Articulo_IDArticulo + " and IDCaracteristica=" + i.IDCaracteristica + " and IDAlmacen=" + AlmacenViene + "");
                            db.Database.ExecuteSqlCommand("update InventarioAlmacen set  Disponibilidad=(Disponibilidad-Apartado) where Apartado>0 and   IDArticulo= " + caracteristica.Articulo_IDArticulo + " and IDCaracteristica=" + i.IDCaracteristica + " and IDAlmacen=" + AlmacenViene + "");
                            db.Database.ExecuteSqlCommand("update InventarioAlmacen set  Disponibilidad=Existencia where  IDArticulo= " + caracteristica.Articulo_IDArticulo + " and IDCaracteristica=" + i.IDCaracteristica + " and IDAlmacen=" + i.IDAlmacen + "");



                            try
                            {
                                Caracteristica carateristica = new WorkinProcessContext().Database.SqlQuery<Caracteristica>("select * from caracteristica where id=" + i.IDCaracteristica).ToList().FirstOrDefault();
                                InventarioAlmacen inv = new InventarioAlmacenContext().InventarioAlmacenes.ToList().Where(s => s.IDAlmacen == i.IDAlmacen && s.IDCaracteristica == i.IDCaracteristica).FirstOrDefault();

                                string cadenam = "INSERT INTO [dbo].[MovimientoArticulo] ([Fecha],[Id],[IDPresentacion],[Articulo_IDArticulo],[Accion],[cantidad],[Documento],[NoDocumento],[Lote],[IDAlmacen],[TipoOperacion],[acumulado],[observacion],[Hora]) VALUES ";
                                cadenam += " (getdate(), " + carateristica.ID + "," + carateristica.IDPresentacion + "," + carateristica.Articulo_IDArticulo + ",'Remisión'," + i.Suministro + ",'Remisión '," + num + ",''," + i.IDAlmacen + ",'S'," + inv.Existencia + ",'',CONVERT (time, SYSDATETIMEOFFSET()))";
                                db.Database.ExecuteSqlCommand(cadenam);
                            }
                            catch (Exception err)
                            {
                                string mensajee = err.Message;
                            }





                        }
                    }
                    //if (articulodetalle.esKit)
                    //{
                    //    generasalidakit(articulodetalle.IDArticulo, i.IDCaracteristica, i.Suministro, fecha, i, num3, detPedido.IDAlmacen, num);
                    //}

                    if (detPedido.IDPrefactura != 0)
                    {
                        db.Database.ExecuteSqlCommand("update [dbo].[DetRemision] set  [Status]='PreFacturado' where [IDDetRemision]='" + num2 + "'");
                        //db.Database.ExecuteSqlCommand("update [dbo].[EncRemision] set  [Status]='Finalizado' where [IDRemision]='" + num + "'");
                    }

                    db.Database.ExecuteSqlCommand("delete from [dbo].[CarritoRemision] where [IDCarritoRemision]='" + i.IDCarritoRemision + "'");



                }
                else if (i.Cantidad > i.Suministro)
                {
                    if (i.Suministro > 0)
                    {
                        diferencia = i.Cantidad - i.Suministro;
                        subtotal = i.Suministro * Precio;
                        iva = subtotal * (decimal)0.16;
                        total = subtotal + iva;


                        if (tieneKits)
                        {
                            db.Database.ExecuteSqlCommand("update [dbo].[DetPedido] set [IDRemision]='" + num + "' where [IDDetPedido]='" + i.IDDetExterna + "'");
                        }
                        else
                        {
                            db.Database.ExecuteSqlCommand("update [dbo].[DetPedido] set [IDRemision]='" + num + "', [Status]='Activo', [Suministro]='" + suministro + "' where [IDDetPedido]='" + i.IDDetExterna + "'");

                        }
                        db.Database.ExecuteSqlCommand("insert into DetRemision([IDRemision],[IDExterna],[IDArticulo],[Cantidad],[Costo],[CantidadPedida],[Descuento],[Importe],[IVA],[ImporteIva],[ImporteTotal],[Nota],[Ordenado],[Caracteristica_ID],[IDAlmacen],[Suministro],[Status],[Presentacion],[jsonPresentacion],[IDDetExterna],[Lote],[Devolucion]) values ('"
                                                                              + num + "','" + detPedido.IDPedido + "','" + detPedido.IDArticulo + "','" + i.Suministro + "','" + Precio + "'," + i.Cantidad + ",'0','" + subtotal + "','true','" + iva + "','" + total + "','" + detPedido.Nota + "','0','" + detPedido.Caracteristica_ID + "','" + detPedido.IDAlmacen + "','" + i.Suministro + "','Activo','" + detPedido.Presentacion + "','" + detPedido.jsonPresentacion + "','" + i.IDDetExterna + "','" + i.Lote + "','0')");


                        List<DetRemision> numero2 = db.Database.SqlQuery<DetRemision>("select * from [dbo].[DetRemision] WHERE IDDetRemision = (SELECT MAX(IDDetRemision) from DetRemision)").ToList();
                        int num2 = numero2.Select(s => s.IDRemision).FirstOrDefault();
                        int num3 = numero2.Select(s => s.IDDetRemision).FirstOrDefault();

                        db.Database.ExecuteSqlCommand("insert into [dbo].[MovComercial] ([DocumentoDestino],[IDDestino],[Cantidad],[Fecha],[IDOrigen],[DocumentoOrigen],[UserID],[IDDetOrigen],[IDDetDestino]) values('Remisión','" + num + "','" + i.Suministro + "','" + fecha + "','" + i.IDPedido + "','Pedido','" + UserID + "','" + i.IDDetExterna + "','" + num3 + "')");

                        //db.Database.ExecuteSqlCommand("exec dbo.MovArt '" + fecha + "','" + i.IDCaracteristica + "','RemVta','" + i.Suministro + "','Remision','" + num2 + "','" + i.Lote + "','" + i.IDAlmacen + "','" + i.Nota + "',0");
                        var detallepedido = new PedidoContext().DetPedido.Where(s => s.IDDetPedido == i.IDDetExterna);

                        int AlmacenViene = 0;

                        foreach (var item in detallepedido)
                        {
                            AlmacenViene = item.IDAlmacen;
                        }

                        if (!articulodetalle.esKit && articulodetalle.CtrlStock)
                        {
                            if (AlmacenViene == i.IDAlmacen)
                            {
                                db.Database.ExecuteSqlCommand("update InventarioAlmacen set  Existencia=(Existencia-" + i.Suministro + ") where IDArticulo= " + caracteristica.Articulo_IDArticulo + " and IDCaracteristica=" + i.IDCaracteristica + " and IDAlmacen=" + AlmacenViene + "");
                                db.Database.ExecuteSqlCommand("update InventarioAlmacen set Apartado=(Apartado-" + i.Suministro + ") where IDArticulo= " + caracteristica.Articulo_IDArticulo + " and IDCaracteristica=" + i.IDCaracteristica + " and IDAlmacen=" + AlmacenViene + "");
                                db.Database.ExecuteSqlCommand("update InventarioAlmacen set Apartado=0 where IDArticulo= " + caracteristica.Articulo_IDArticulo + " and IDCaracteristica=" + i.IDCaracteristica + " and IDAlmacen=" + AlmacenViene + " and Apartado<0");
                                db.Database.ExecuteSqlCommand("update InventarioAlmacen set  Disponibilidad=Existencia where apartado=0 and IDArticulo= " + caracteristica.Articulo_IDArticulo + " and IDCaracteristica=" + i.IDCaracteristica + " and IDAlmacen=" + AlmacenViene + "");
                                db.Database.ExecuteSqlCommand("update InventarioAlmacen set  Disponibilidad=(Disponibilidad-" + i.Suministro + ") where Apartado>0 and   IDArticulo= " + caracteristica.Articulo_IDArticulo + " and IDCaracteristica=" + i.IDCaracteristica + " and IDAlmacen=" + AlmacenViene + "");

                                try
                                {
                                    Caracteristica carateristica = new WorkinProcessContext().Database.SqlQuery<Caracteristica>("select * from caracteristica where id=" + i.IDCaracteristica).ToList().FirstOrDefault();
                                    InventarioAlmacen inv = new InventarioAlmacenContext().InventarioAlmacenes.ToList().Where(s => s.IDAlmacen == AlmacenViene && s.IDCaracteristica == i.IDCaracteristica).FirstOrDefault();

                                    string cadenam = "INSERT INTO [dbo].[MovimientoArticulo] ([Fecha],[Id],[IDPresentacion],[Articulo_IDArticulo],[Accion],[cantidad],[Documento],[NoDocumento],[Lote],[IDAlmacen],[TipoOperacion],[acumulado],[observacion],[Hora]) VALUES ";
                                    cadenam += " (getdate(), " + carateristica.ID + "," + carateristica.IDPresentacion + "," + carateristica.Articulo_IDArticulo + ",'Remisión'," + i.Suministro + ",'Remisión '," + num + ",''," + AlmacenViene + ",'S'," + inv.Existencia + ",'',CONVERT (time, SYSDATETIMEOFFSET()))";
                                    db.Database.ExecuteSqlCommand(cadenam);
                                }
                                catch (Exception err)
                                {
                                    string mensajee = err.Message;
                                }
                            }
                            else
                            {
                                db.Database.ExecuteSqlCommand("update InventarioAlmacen set  Existencia=(Existencia-" + i.Suministro + ") where IDArticulo= " + caracteristica.Articulo_IDArticulo + " and IDCaracteristica=" + i.IDCaracteristica + " and IDAlmacen=" + i.IDAlmacen + "");
                                db.Database.ExecuteSqlCommand("update InventarioAlmacen set Apartado=(Apartado-" + i.Suministro + ") where IDArticulo= " + caracteristica.Articulo_IDArticulo + " and IDCaracteristica=" + i.IDCaracteristica + " and IDAlmacen=" + AlmacenViene + "");
                                db.Database.ExecuteSqlCommand("update InventarioAlmacen set Apartado=0 where IDArticulo= " + caracteristica.Articulo_IDArticulo + " and IDCaracteristica=" + i.IDCaracteristica + " and IDAlmacen=" + AlmacenViene + " and Apartado<0");
                                db.Database.ExecuteSqlCommand("update InventarioAlmacen set  Disponibilidad=Existencia where apartado=0 and IDArticulo= " + caracteristica.Articulo_IDArticulo + " and IDCaracteristica=" + i.IDCaracteristica + " and IDAlmacen=" + AlmacenViene + "");
                                db.Database.ExecuteSqlCommand("update InventarioAlmacen set  Disponibilidad=(Disponibilidad-Apartado) where Apartado>0 and   IDArticulo= " + caracteristica.Articulo_IDArticulo + " and IDCaracteristica=" + i.IDCaracteristica + " and IDAlmacen=" + AlmacenViene + "");
                                db.Database.ExecuteSqlCommand("update InventarioAlmacen set  Disponibilidad=Existencia where  IDArticulo= " + caracteristica.Articulo_IDArticulo + " and IDCaracteristica=" + i.IDCaracteristica + " and IDAlmacen=" + i.IDAlmacen + "");

                                try
                                {
                                    Caracteristica carateristica = new WorkinProcessContext().Database.SqlQuery<Caracteristica>("select * from caracteristica where id=" + i.IDCaracteristica).ToList().FirstOrDefault();
                                    InventarioAlmacen inv = new InventarioAlmacenContext().InventarioAlmacenes.ToList().Where(s => s.IDAlmacen == i.IDAlmacen && s.IDCaracteristica == i.IDCaracteristica).FirstOrDefault();

                                    string cadenam = "INSERT INTO [dbo].[MovimientoArticulo] ([Fecha],[Id],[IDPresentacion],[Articulo_IDArticulo],[Accion],[cantidad],[Documento],[NoDocumento],[Lote],[IDAlmacen],[TipoOperacion],[acumulado],[observacion],[Hora]) VALUES ";
                                    cadenam += " (getdate(), " + carateristica.ID + "," + carateristica.IDPresentacion + "," + carateristica.Articulo_IDArticulo + ",'Remisión'," + i.Suministro + ",'Remisión '," + num + ",''," + i.IDAlmacen + ",'S'," + inv.Existencia + ",'',CONVERT (time, SYSDATETIMEOFFSET()))";
                                    db.Database.ExecuteSqlCommand(cadenam);
                                }
                                catch (Exception err)
                                {
                                    string mensajee = err.Message;
                                }

                            }
                        }
                        //if (articulodetalle.esKit)
                        //{
                        //    generasalidakit(articulodetalle.IDArticulo, i.IDCaracteristica, i.Suministro, fecha, i, num3, detPedido.IDAlmacen, num);
                        //}


                        db.Database.ExecuteSqlCommand("delete from [dbo].[CarritoRemision] where [IDCarritoRemision]='" + i.IDCarritoRemision + "'");
                        if (detPedido.IDPrefactura != 0)
                        {
                            db.Database.ExecuteSqlCommand("update [dbo].[DetRemision] set  [Status]='PreFacturado' where [IDDetRemision]='" + num3 + "'");
                            //  db.Database.ExecuteSqlCommand("update [dbo].[EncRemision] set  [Status]='Finalizado' where [IDRemision]='" + num + "'");
                        }
                    }

                }
                else if (i.Cantidad < i.Suministro)
                {
                    diferencia = i.Suministro - i.Cantidad;
                    subtotal = i.Suministro * Precio;
                    iva = subtotal * (decimal)0.16;
                    total = subtotal + iva;

                    db.Database.ExecuteSqlCommand("update [dbo].[DetPedido] set [IDRemision]='" + num + "', [Status]='Finalizado', [Suministro]='" + i.Suministro + "' where [IDDetPedido]='" + i.IDDetExterna + "'");

                    db.Database.ExecuteSqlCommand("insert into DetRemision([IDRemision],[IDExterna],[IDArticulo],[Cantidad],[Costo],[CantidadPedida],[Descuento],[Importe],[IVA],[ImporteIva],[ImporteTotal],[Nota],[Ordenado],[Caracteristica_ID],[IDAlmacen],[Suministro],[Status],[Presentacion],[jsonPresentacion],[IDDetExterna],[Lote],[Devolucion]) values ('"
                                                                              + num + "','" + detPedido.IDPedido + "','" + detPedido.IDArticulo + "','" + i.Suministro + "','" + Precio + "'," + i.Cantidad + ",'0','" + subtotal + "','true','" + iva + "','" + total + "','" + detPedido.Nota + "','0','" + detPedido.Caracteristica_ID + "','" + detPedido.IDAlmacen + "','" + i.Suministro + "','Activo','" + detPedido.Presentacion + "','" + detPedido.jsonPresentacion + "','" + i.IDDetExterna + "','" + i.Lote + "','0')");


                    List<DetRemision> numero2 = db.Database.SqlQuery<DetRemision>("select * from [dbo].[DetRemision] WHERE IDDetRemision = (SELECT MAX(IDDetRemision) from DetRemision)").ToList();
                    int num2 = numero2.Select(s => s.IDRemision).FirstOrDefault();
                    int num3 = numero2.Select(s => s.IDDetRemision).FirstOrDefault();

                    db.Database.ExecuteSqlCommand("insert into [dbo].[MovComercial] ([DocumentoDestino],[IDDestino],[Cantidad],[Fecha],[IDOrigen],[DocumentoOrigen],[UserID],[IDDetOrigen],[IDDetDestino]) values('Remisión','" + num + "','" + i.Cantidad + "','" + fecha + "','" + i.IDPedido + "','Pedido','" + UserID + "','" + i.IDDetExterna + "','" + num3 + "')");



                    //db.Database.ExecuteSqlCommand("exec dbo.MovArt '" + fecha + "','" + i.IDCaracteristica + "','RemVta','" + i.Cantidad + "','Remision','" + num2 + "','" + i.Lote + "','" + i.IDAlmacen + "','" + i.Nota + "',0");

                    var detallepedido = new PedidoContext().DetPedido.Where(s => s.IDDetPedido == i.IDDetExterna);

                    int AlmacenViene = 0;

                    foreach (var item in detallepedido)
                    {
                        AlmacenViene = item.IDAlmacen;
                    }

                    if (!articulodetalle.esKit && articulodetalle.CtrlStock)
                    {
                        if (AlmacenViene == i.IDAlmacen)
                        {
                            db.Database.ExecuteSqlCommand("update InventarioAlmacen set  Existencia=(Existencia-" + i.Suministro + ") where IDArticulo= " + caracteristica.Articulo_IDArticulo + " and IDCaracteristica=" + i.IDCaracteristica + " and IDAlmacen=" + AlmacenViene + "");
                            db.Database.ExecuteSqlCommand("update InventarioAlmacen set Apartado=(Apartado-" + i.Suministro + ") where IDArticulo= " + caracteristica.Articulo_IDArticulo + " and IDCaracteristica=" + i.IDCaracteristica + " and IDAlmacen=" + AlmacenViene + "");
                            db.Database.ExecuteSqlCommand("update InventarioAlmacen set Apartado=0 where IDArticulo= " + caracteristica.Articulo_IDArticulo + " and IDCaracteristica=" + i.IDCaracteristica + " and IDAlmacen=" + AlmacenViene + " and Apartado<0");
                            db.Database.ExecuteSqlCommand("update InventarioAlmacen set  Disponibilidad=Existencia where apartado=0 and IDArticulo= " + caracteristica.Articulo_IDArticulo + " and IDCaracteristica=" + i.IDCaracteristica + " and IDAlmacen=" + AlmacenViene + "");
                            db.Database.ExecuteSqlCommand("update InventarioAlmacen set  Disponibilidad=(Disponibilidad-" + i.Suministro + ") where Apartado>0 and   IDArticulo= " + caracteristica.Articulo_IDArticulo + " and IDCaracteristica=" + i.IDCaracteristica + " and IDAlmacen=" + AlmacenViene + "");
                            try
                            {
                                Caracteristica carateristica = new WorkinProcessContext().Database.SqlQuery<Caracteristica>("select * from caracteristica where id=" + i.IDCaracteristica).ToList().FirstOrDefault();
                                InventarioAlmacen inv = new InventarioAlmacenContext().InventarioAlmacenes.ToList().Where(s => s.IDAlmacen == AlmacenViene && s.IDCaracteristica == i.IDCaracteristica).FirstOrDefault();

                                string cadenam = "INSERT INTO [dbo].[MovimientoArticulo] ([Fecha],[Id],[IDPresentacion],[Articulo_IDArticulo],[Accion],[cantidad],[Documento],[NoDocumento],[Lote],[IDAlmacen],[TipoOperacion],[acumulado],[observacion],[Hora]) VALUES ";
                                cadenam += " (getdate(), " + carateristica.ID + "," + carateristica.IDPresentacion + "," + carateristica.Articulo_IDArticulo + ",'Remisión'," + i.Suministro + ",'Remisión '," + num + ",''," + AlmacenViene + ",'S'," + inv.Existencia + ",'',CONVERT (time, SYSDATETIMEOFFSET()))";
                                db.Database.ExecuteSqlCommand(cadenam);
                            }
                            catch (Exception err)
                            {
                                string mensajee = err.Message;
                            }

                        }
                        else
                        {
                            db.Database.ExecuteSqlCommand("update InventarioAlmacen set  Existencia=(Existencia-" + i.Suministro + ") where IDArticulo= " + caracteristica.Articulo_IDArticulo + " and IDCaracteristica=" + i.IDCaracteristica + " and IDAlmacen=" + i.IDAlmacen + "");
                            db.Database.ExecuteSqlCommand("update InventarioAlmacen set Apartado=(Apartado-" + i.Suministro + ") where IDArticulo= " + caracteristica.Articulo_IDArticulo + " and IDCaracteristica=" + i.IDCaracteristica + " and IDAlmacen=" + AlmacenViene + "");
                            db.Database.ExecuteSqlCommand("update InventarioAlmacen set Apartado=0 where IDArticulo= " + caracteristica.Articulo_IDArticulo + " and IDCaracteristica=" + i.IDCaracteristica + " and IDAlmacen=" + AlmacenViene + " and Apartado<0");
                            db.Database.ExecuteSqlCommand("update InventarioAlmacen set  Disponibilidad=Existencia where apartado=0 and IDArticulo= " + caracteristica.Articulo_IDArticulo + " and IDCaracteristica=" + i.IDCaracteristica + " and IDAlmacen=" + AlmacenViene + "");
                            db.Database.ExecuteSqlCommand("update InventarioAlmacen set  Disponibilidad=(Disponibilidad-Apartado) where Apartado>0 and   IDArticulo= " + caracteristica.Articulo_IDArticulo + " and IDCaracteristica=" + i.IDCaracteristica + " and IDAlmacen=" + AlmacenViene + "");
                            db.Database.ExecuteSqlCommand("update InventarioAlmacen set  Disponibilidad=Existencia where  IDArticulo= " + caracteristica.Articulo_IDArticulo + " and IDCaracteristica=" + i.IDCaracteristica + " and IDAlmacen=" + i.IDAlmacen + "");
                            try
                            {
                                Caracteristica carateristica = new WorkinProcessContext().Database.SqlQuery<Caracteristica>("select * from caracteristica where id=" + i.IDCaracteristica).ToList().FirstOrDefault();
                                InventarioAlmacen inv = new InventarioAlmacenContext().InventarioAlmacenes.ToList().Where(s => s.IDAlmacen == i.IDAlmacen && s.IDCaracteristica == i.IDCaracteristica).FirstOrDefault();

                                string cadenam = "INSERT INTO [dbo].[MovimientoArticulo] ([Fecha],[Id],[IDPresentacion],[Articulo_IDArticulo],[Accion],[cantidad],[Documento],[NoDocumento],[Lote],[IDAlmacen],[TipoOperacion],[acumulado],[observacion],[Hora]) VALUES ";
                                cadenam += " (getdate(), " + carateristica.ID + "," + carateristica.IDPresentacion + "," + carateristica.Articulo_IDArticulo + ",'Remisión'," + i.Suministro + ",'Remisión '," + num + ",''," + i.IDAlmacen + ",'S'," + inv.Existencia + ",'',CONVERT (time, SYSDATETIMEOFFSET()))";
                                db.Database.ExecuteSqlCommand(cadenam);
                            }
                            catch (Exception err)
                            {
                                string mensajee = err.Message;
                            }
                        }
                    }
                    //if (articulodetalle.esKit)
                    //{
                    //    generasalidakit(articulodetalle.IDArticulo, i.IDCaracteristica, i.Suministro, fecha, i, num3, detPedido.IDAlmacen, num);
                    //}




                    db.Database.ExecuteSqlCommand("delete from [dbo].[CarritoRemision] where [IDCarritoRemision]='" + i.IDCarritoRemision + "'");


                    if (detPedido.IDPrefactura != 0)
                    {
                        db.Database.ExecuteSqlCommand("update [dbo].[DetRemision] set  [Status]='PreFacturado' where [IDDetRemision]='" + num3 + "'");
                    }

                }

                ClsDatoEntero c = db.Database.SqlQuery<ClsDatoEntero>("select count(IDPedido) as Dato from DetPedido where IDPedido ='" + i.IDPedido + "'").ToList()[0];
                int x = c.Dato;

                ClsDatoEntero b = db.Database.SqlQuery<ClsDatoEntero>("select count(IDPedido) as Dato from DetPedido where IDPedido='" + i.IDPedido + "' and Status = 'Finalizado'").ToList()[0];
                int y = b.Dato;

                if (x == y)
                {
                    db.Database.ExecuteSqlCommand("update [dbo].[EncPedido] set [Status]='Finalizado' where [IDPedido]='" + i.IDPedido + "'");
                }
                else
                {
                    db.Database.ExecuteSqlCommand("update [dbo].[EncPedido] set [Status]='Activo' where [IDPedido]='" + i.IDPedido + "'");
                }


                if (articulodetalle.esKit)
                {
                    if (articulodetalle.CtrlStock)
                    {
                        var kits = new KitContext().Database.SqlQuery<Kit>("select * from kit where IDARTICULO=" + articulodetalle.IDArticulo).ToList();
                        foreach (Kit kista in kits)
                        {

                            db.Database.ExecuteSqlCommand("update [dbo].[detSolicitud] set [Status]='No solicitado' where [numero]=" + i.IDPedido + " and IDArticulo=" + kista.IDArticuloComp + " and Caracteristica_ID=" + kista.IDCaracteristica + " and status='Solicitado'");
                        }
                    }


                }
                else
                {

                    if (articulodetalle.CtrlStock)
                    {
                        db.Database.ExecuteSqlCommand("update [dbo].[detSolicitud] set [Status]='No solicitado' where [numero]=" + i.IDPedido + " and IDArticulo=" + articulodetalle.IDArticulo + " and Caracteristica_ID=" + i.IDCaracteristica + " and status='Solicitado'");
                    }
                }


            }


            ClsDatoEntero c1 = db.Database.SqlQuery<ClsDatoEntero>("select count(IDRemision) as Dato from DetRemision where IDRemision ='" + num + "'").ToList()[0];
            int x1 = c1.Dato;

            ClsDatoEntero b1 = db.Database.SqlQuery<ClsDatoEntero>("select count(IDRemision) as Dato from DetRemision where IDRemision='" + num + "' and Status = 'PreFacturado'").ToList().FirstOrDefault();
            int y1 = b1.Dato;

            if (x1 == y1)
            {
                db.Database.ExecuteSqlCommand("update [dbo].[EncRemision] set  [Status]='PreFacturado' where [IDRemision]='" + num + "'");
            }
            else
            {
                db.Database.ExecuteSqlCommand("update [dbo].[EncRemision] set [Status]='Activo' where [IDRemision]='" + num + "'");
            }


            List<c_Moneda> monedaorigen = db.Database.SqlQuery<c_Moneda>("select * from c_Moneda WHERE ClaveMoneda='MXN'").ToList();
            int origen = monedaorigen.Select(s => s.IDMoneda).FirstOrDefault();
            VCambio cambioaux = db.Database.SqlQuery<VCambio>("select dbo.GetTipocambio('" + fecha + "'," + IDMoneda + "," + origen + ") as TC").ToList()[0];
            decimal TipoCambio = cambioaux.TC;
            List<DetRemision> datostotales = db.Database.SqlQuery<DetRemision>("select * from dbo.DetRemision where IDRemision='" + num + "'").ToList();
            subtotalr = datostotales.Sum(s => s.Importe);
            ivar = subtotalr * (decimal)0.16;
            totalr = subtotalr + ivar;
            db.Database.ExecuteSqlCommand("update [dbo].[EncRemision] set [Subtotal]=" + subtotalr + ",[IVA]=" + ivar + ",[Total]=" + totalr + " where [IDRemision]=" + num + "");
            //db.Database.ExecuteSqlCommand("update [dbo].[EncRemision] set [Subtotal]='" + subtotalr + "',[IVA]='" + ivar + "',[Total]='" + totalr + "',[TipoCambio]='" + TipoCambio + "' where [IDRemision]='" + num + "'");


            if (tieneKits == true)
            {
                return RedirectToAction("RemisionKit", new { id = num });
            }
            else if (tienecintas == true)
            {
                return RedirectToAction("CintasRemision", new { idremision = num });
            }
            else
            {
                return RedirectToAction("Index");
            }

        }
        public ActionResult RemisionKit(int id)
        {
            List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            int usuario = userid.Select(s => s.UserID).FirstOrDefault();

            db.Database.ExecuteSqlCommand("delete from CarritoRemisionKit where userid=" + usuario);


            ViewBag.remision = id;
            List<DetRemision> detremision = db.Database.SqlQuery<DetRemision>("select d.* from articulo as a inner join detremision as d on a.idarticulo=d.idarticulo where a.esKit='True' and idremision=" + id).ToList();

            ViewBag.Detremision = detremision;


            foreach (var c in detremision)
            {
                try
                {

                    List<DetPedidoKit> pedidoKit = new DetPedidoKitContext().Database.SqlQuery<DetPedidoKit>("select*from DetPedidoKit where iddetpedido=" + c.IDDetExterna).ToList();

                    if (pedidoKit.Count != 0)
                    {
                        foreach (DetPedidoKit det in pedidoKit)
                        {
                            SIAAPI.Models.Comercial.Caracteristica caracteristicas = new SIAAPI.Models.Comercial.ArticuloContext().Database.SqlQuery<SIAAPI.Models.Comercial.Caracteristica>("select * from Caracteristica where id=" + det.IDCaracteristica).ToList().FirstOrDefault();
                            decimal CantidadComponete = 0;
                            CantidadComponete = det.Cantidad;

                            db.Database.ExecuteSqlCommand("INSERT INTO CarritoRemisionKit([IDDetExterna],[IDPedido],[IDArticulo],[Cantidad],[Costo],[CantidadPedida],[Caracteristica_ID],[IDAlmacen],[Suministro],[Presentacion],[jsonPresentacion], [userid])   values" +
                            " ('" + c.IDDetRemision + "','" + c.IDExterna + "','" + det.IDArticulo + "','" + CantidadComponete + "','" + det.Precio + "'," + CantidadComponete + ",'" + det.IDCaracteristica + "','" + det.IDAlmacen + "'," + 0 + "," + caracteristicas.IDPresentacion + ", '" + caracteristicas.jsonPresentacion + "'," + usuario + ")");

                        }
                    }
                    else
                    {
                        var kits = new SIAAPI.Models.Comercial.KitContext().Database.SqlQuery<SIAAPI.Models.Comercial.Kit>("select * from kit where IDARTICULO=" + c.IDArticulo).ToList();
                        foreach (var k in kits)
                        {


                            SIAAPI.Models.Comercial.Caracteristica caracteristicas = new SIAAPI.Models.Comercial.ArticuloContext().Database.SqlQuery<SIAAPI.Models.Comercial.Caracteristica>("select * from Caracteristica where id=" + k.IDCaracteristica).ToList().FirstOrDefault();
                            decimal CantidadComponete = 0;
                            CantidadComponete = c.Cantidad * k.Cantidad;

                            db.Database.ExecuteSqlCommand("INSERT INTO CarritoRemisionKit([IDDetExterna],[IDPedido],[IDArticulo],[Cantidad],[Costo],[CantidadPedida],[Caracteristica_ID],[IDAlmacen],[Suministro],[Presentacion],[jsonPresentacion], [userid])   values" +
                            " ('" + c.IDDetRemision + "','" + c.IDExterna + "','" + k.IDArticuloComp + "','" + CantidadComponete + "','" + k.Precio + "'," + CantidadComponete + ",'" + k.IDCaracteristica + "','" + c.IDAlmacen + "'," + 0 + "," + caracteristicas.IDPresentacion + ", '" + caracteristicas.jsonPresentacion + "'," + usuario + ")");

                        }
                    }




                }
                catch (Exception err)
                {

                }


            }
            string cadenakit = "select [IDCarritoRemisionKit],[IDPedido],[IDArticulo],[Cantidad],[Costo],[CantidadPedida],[Caracteristica_ID],[IDAlmacen],[Suministro],[jsonPresentacion],[UserID],[IDDetExterna]from CarritoRemisionKit where userid=" + usuario;
            List<VCarritoRemisionKit> remision = db.Database.SqlQuery<VCarritoRemisionKit>(cadenakit).ToList();
            // var remision = new CarritoContext().CarritoRemisioneKit.Where(s => s.IDDetExterna == id).ToList();




            return View(remision);
        }

        [HttpPost]
        public ActionResult updateRemisionKit(int? id, FormCollection coleccion, List<VCarritoRemisionKit> cr)
        {



            int IDRemisionK = 0;
            int IDDetRemisionK = 0;
            int IDPedido = 0;

            foreach (VCarritoRemisionKit elev in cr)
            {
                IDDetRemisionK = elev.IDDetExterna;
                ClsDatoEntero IDRemision = db.Database.SqlQuery<ClsDatoEntero>("select IDRemision as Dato from DetRemision where IDDetRemision=" + IDDetRemisionK).ToList().FirstOrDefault();



                ClsDatoEntero IDArticuloKit = db.Database.SqlQuery<ClsDatoEntero>("select IDArticulo as Dato from DetRemision where IDDetRemision=" + IDDetRemisionK).ToList().FirstOrDefault();
                Kit kit = new KitContext().Database.SqlQuery<Kit>("select * from Kit where idArticulo=" + IDArticuloKit.Dato + " and IDCaracteristica=" + elev.Caracteristica_ID).ToList().FirstOrDefault();




                Articulo ar = new ArticuloContext().Articulo.Find(kit.IDArticuloComp);
                if (elev.Suministro > 0)
                {
                    if (ar.CtrlStock)
                    {
                        InventarioAlmacen ia = new InventarioAlmacenContext().InventarioAlmacenes.ToList().Where(s => s.IDAlmacen == kit.IDAlmacen && s.IDCaracteristica == kit.IDCaracteristica).FirstOrDefault();
                        Almacen almacen = new AlmacenContext().Almacenes.Find(kit.IDAlmacen);
                        if (ia == null)
                        {
                            throw new Exception("No hay Existencia en almacen para sutir tu remisión " + ar.Cref + " en la cantidad de " + elev.Suministro + " Por favor ingresa un invetario inicial del este articulo en el almacen " + almacen.Descripcion);
                        }
                        try
                        {
                            if (ia.Existencia < elev.Suministro)
                            {
                                throw new Exception("No hay Existencia en almacen para sutir tu remision " + ar.Cref + " en la cantidad de " + elev.Suministro + " solo hay " + ia.Existencia);
                            }
                        }
                        catch (Exception err)
                        {

                            return Content(err.Message);
                        }



                    }
                }
            }


            foreach (VCarritoRemisionKit elev in cr)
            {

                db.Database.ExecuteSqlCommand("update [dbo].[CarritoRemisionKit] set [Suministro]=" + elev.Suministro + " where [IDCarritoRemisionKit]='" + elev.IDCarritoRemisionKit + "'");

                IDDetRemisionK = elev.IDDetExterna;
                ClsDatoEntero IDRemision = db.Database.SqlQuery<ClsDatoEntero>("select IDRemision as Dato from DetRemision where IDDetRemision=" + IDDetRemisionK).ToList().FirstOrDefault();

                ClsDatoEntero IDArticuloKit = db.Database.SqlQuery<ClsDatoEntero>("select IDArticulo as Dato from DetRemision where IDDetRemision=" + IDDetRemisionK).ToList().FirstOrDefault();
                //ClsDatoEntero IDCaracteristicaKit = db.Database.SqlQuery<ClsDatoEntero>("select caracteristica_ID as Dato from DetRemision where IDDetRemision=" + IDDetRemisionK).ToList().FirstOrDefault();

                Kit kit = new KitContext().Database.SqlQuery<Kit>("select * from Kit where idArticulo=" + IDArticuloKit.Dato + " and idcaracteristica=" + elev.Caracteristica_ID).ToList().FirstOrDefault();
                Caracteristica carateristicaa = new WorkinProcessContext().Database.SqlQuery<Caracteristica>("select * from caracteristica where id=" + elev.Caracteristica_ID).ToList().FirstOrDefault();

                Articulo ar = new ArticuloContext().Articulo.Find(carateristicaa.Articulo_IDArticulo);


                IDRemisionK = IDRemision.Dato;
                if (elev.Suministro > 0)
                {
                    if (ar.CtrlStock)
                    {
                        InventarioAlmacen ia = new InventarioAlmacenContext().InventarioAlmacenes.ToList().Where(s => s.IDAlmacen == kit.IDAlmacen && s.IDCaracteristica == kit.IDCaracteristica).FirstOrDefault();
                        Almacen almacen = new AlmacenContext().Almacenes.Find(kit.IDAlmacen);


                        db.Database.ExecuteSqlCommand("update InventarioAlmacen set  Existencia=(Existencia-" + elev.Suministro + ") where IDArticulo= " + ar.IDArticulo + " and IDCaracteristica=" + elev.Caracteristica_ID + " and IDAlmacen=" + kit.IDAlmacen + "");
                        db.Database.ExecuteSqlCommand("update InventarioAlmacen set  Existencia=0 where Existencia<0 and IDArticulo= " + ar.IDArticulo + " and IDCaracteristica=" + elev.Caracteristica_ID + " and IDAlmacen=" + kit.IDAlmacen + "");
                        decimal apartado = 0;
                        try
                        {
                            if (elev.Suministro > elev.Cantidad)
                            {
                                apartado = elev.Cantidad;
                            }
                            else
                            {
                                apartado = elev.Suministro;
                            }
                        }
                        catch (Exception err)
                        {

                        }
                        db.Database.ExecuteSqlCommand("update InventarioAlmacen set Apartado=(Apartado-" + apartado + ") where IDArticulo= " + ar.IDArticulo + " and IDCaracteristica=" + elev.Caracteristica_ID + " and IDAlmacen=" + kit.IDAlmacen + "");
                        db.Database.ExecuteSqlCommand("update InventarioAlmacen set Apartado=0 where IDArticulo= " + ar.IDArticulo + " and IDCaracteristica=" + elev.Caracteristica_ID + " and IDAlmacen=" + kit.IDAlmacen + " and Apartado<0");
                        db.Database.ExecuteSqlCommand("update InventarioAlmacen set  Disponibilidad=Existencia where apartado=0 and IDArticulo= " + ar.IDArticulo + " and IDCaracteristica=" + elev.Caracteristica_ID + " and IDAlmacen=" + kit.IDAlmacen + "");
                        db.Database.ExecuteSqlCommand("update InventarioAlmacen set  Disponibilidad=(Existencia-Apartado) where   IDArticulo= " + ar.IDArticulo + " and IDCaracteristica=" + elev.Caracteristica_ID + " and IDAlmacen=" + kit.IDAlmacen + "");

                        try
                        {
                            Caracteristica carateristica = new WorkinProcessContext().Database.SqlQuery<Caracteristica>("select * from caracteristica where id=" + elev.Caracteristica_ID).ToList().FirstOrDefault();
                            InventarioAlmacen inv = new InventarioAlmacenContext().InventarioAlmacenes.ToList().Where(s => s.IDAlmacen == kit.IDAlmacen && s.IDCaracteristica == elev.Caracteristica_ID).FirstOrDefault();

                            List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
                            int usuario = userid.Select(s => s.UserID).FirstOrDefault();

                            string cadenam = "INSERT INTO [dbo].[MovimientoArticulo] ([Fecha],[Id],[IDPresentacion],[Articulo_IDArticulo],[Accion],[cantidad],[Documento],[NoDocumento],[Lote],[IDAlmacen],[TipoOperacion],[acumulado],[observacion],[Hora], Usuario) VALUES ";
                            cadenam += " (getdate(), " + carateristica.ID + "," + carateristica.IDPresentacion + "," + carateristica.Articulo_IDArticulo + ",'Remisión Artículo Componente Kit'," + elev.Suministro + ",'Remisión '," + IDRemision.Dato + ",''," + kit.IDAlmacen + ",'S'," + inv.Existencia + ",'',CONVERT (time, SYSDATETIMEOFFSET()), " + usuario + ")";
                            db.Database.ExecuteSqlCommand(cadenam);
                        }
                        catch (Exception err)
                        {
                            string mensajee = err.Message;
                        }







                    }

                    try
                    {
                        string cadenaK = "INSERT INTO [dbo].[DetRemisionKit] ([IDRemisionDetKit],[IDRemision],[IDArticulo],[CantidadAnt],[CantidadNue],[Caracteristica_ID],[IDAlmacen]) VALUES ";
                        cadenaK += " (" + IDDetRemisionK + "," + IDRemisionK + "," + ar.IDArticulo + "," + elev.Cantidad + "," + elev.Suministro + "," + elev.Caracteristica_ID + "," + kit.IDAlmacen + ")";
                        db.Database.ExecuteSqlCommand(cadenaK);
                    }
                    catch (Exception err)
                    {

                    }
                }


            }
            decimal subtotalr = 0;
            decimal ivar = 0;
            decimal totalr = 0;
            List<DetRemision> detremision = db.Database.SqlQuery<DetRemision>("select d.* from articulo as a inner join detremision as d on a.idarticulo=d.idarticulo where a.esKit='True' and idremision=" + IDRemisionK).ToList();
            foreach (DetRemision det in detremision)
            {
                decimal acumuladornuevo = 0;
                decimal acumuladorant = 0;
                decimal ImporteNuevo = 0;
                decimal importeAnt = 0;
                decimal factor = 0;
                decimal antPrecio = 0;
                decimal nuevoPrecio = 0;
                int iddetremision = 0;

                string cadenakit = "select [IDCarritoRemisionKit],[IDPedido],[IDArticulo],[Cantidad],[Costo],[CantidadPedida],[Caracteristica_ID],[IDAlmacen],[Suministro],[jsonPresentacion],[UserID],[IDDetExterna] from CarritoRemisionKit where IDDetExterna=" + det.IDDetRemision;
                List<SIAAPI.ViewModels.Comercial.VCarritoRemisionKit> remisionCarrito = db.Database.SqlQuery<SIAAPI.ViewModels.Comercial.VCarritoRemisionKit>(cadenakit).ToList();


                foreach (VCarritoRemisionKit elev in remisionCarrito)
                {
                    antPrecio = elev.Cantidad * elev.Costo;
                    nuevoPrecio = elev.Suministro * elev.Costo;
                    iddetremision = elev.IDDetExterna;

                    acumuladornuevo += nuevoPrecio;
                    acumuladorant += antPrecio;




                }
                DetRemision detRemision = new RemisionContext().DetRemisiones.Find(iddetremision);
                //ClsDatoDecimal CantidadKit = db.Database.SqlQuery<ClsDatoDecimal>("select cantidad as Dato from DetRemision where IDDetRemision=" + iddetremision).ToList().FirstOrDefault();
                //ClsDatoDecimal PrecioKit = db.Database.SqlQuery<ClsDatoDecimal>("select Costo as Dato from DetRemision where IDDetRemision=" + iddetremision).ToList().FirstOrDefault();

                importeAnt = acumuladorant;
                ImporteNuevo = acumuladornuevo;
                factor = ImporteNuevo / importeAnt;
                decimal CantidadFactor = detRemision.Cantidad * factor;

                decimal subtotal = CantidadFactor * detRemision.Costo;
                decimal iva = subtotal * decimal.Parse(SIAAPI.Properties.Settings.Default.ValorIVA);
                decimal total = subtotal + iva;


                IDPedido = det.IDExterna;



                db.Database.ExecuteSqlCommand("update [dbo].[DetRemision] set Cantidad=" + Math.Round(CantidadFactor, 2) + " where [IDDetRemision]='" + det.IDDetRemision + "'");
                //db.Database.ExecuteSqlCommand("update [dbo].[DetRemision] set [Importe]=" + subtotal + ",ImporteIva=" + iva + ",ImporteTotal=" + total + " where [IDDetRemision]='" + det.IDDetRemision + "'");
                db.Database.ExecuteSqlCommand("update [dbo].[DetRemision] set [Importe]=(cantidad*costo) where [IDDetRemision]='" + det.IDDetRemision + "'");
                db.Database.ExecuteSqlCommand("update [dbo].[DetRemision] set ImporteIva=(importe*" + decimal.Parse(SIAAPI.Properties.Settings.Default.ValorIVA) + ") where [IDDetRemision]='" + det.IDDetRemision + "'");
                db.Database.ExecuteSqlCommand("update [dbo].[DetRemision] set ImporteTotal=(importe+importeiva) where [IDDetRemision]='" + det.IDDetRemision + "'");
                DetPedido detallePedido = db.Database.SqlQuery<DetPedido>("select * from dbo.DetPedido where IDDetPedido='" + det.IDDetExterna + "'").ToList().FirstOrDefault();
                if (detallePedido.Suministro == detallePedido.Cantidad)
                {
                    db.Database.ExecuteSqlCommand("update [dbo].[DetPedido] set [suministro]=" + Math.Round(CantidadFactor, 2) + " where  [IDDetPedido]='" + det.IDDetExterna + "'");
                }
                else
                {
                    db.Database.ExecuteSqlCommand("update [dbo].[DetPedido] set [suministro]=(suministro+" + Math.Round(CantidadFactor, 2) + ") where  [IDDetPedido]='" + det.IDDetExterna + "'");
                }



                if (detallePedido.Suministro < detallePedido.Cantidad)
                {
                    db.Database.ExecuteSqlCommand("update [dbo].[DetPedido] set Status='Activo' where [IDDetPedido]='" + det.IDDetExterna + "'");

                }
            }
            List<DetRemision> datostotales = db.Database.SqlQuery<DetRemision>("select * from dbo.DetRemision where IDRemision='" + IDRemisionK + "'").ToList();
            subtotalr = datostotales.Sum(s => s.Importe);
            ivar = subtotalr * (decimal)0.16;
            totalr = subtotalr + ivar;
            db.Database.ExecuteSqlCommand("update [dbo].[EncRemision] set [Subtotal]=" + subtotalr + ",[IVA]=" + ivar + ",[Total]=" + totalr + " where [IDRemision]=" + IDRemisionK + "");

            try
            {
                List<DetPedido> dtpedido = db.Database.SqlQuery<DetPedido>("select * from dbo.DetPedido where IDPedido='" + IDPedido + "'").ToList();
                foreach (DetPedido d in dtpedido)
                {

                    if (d.Suministro >= d.Cantidad)
                    {
                        db.Database.ExecuteSqlCommand("update [dbo].[DetPedido] set  [Status]='Finalizado' where [IDDetPedido]='" + d.IDDetPedido + "'");

                    }




                }
            }
            catch (Exception err)
            {

            }


            try
            {
                ClsDatoEntero c = db.Database.SqlQuery<ClsDatoEntero>("select count(IDPedido) as Dato from DetPedido where IDPedido ='" + IDPedido + "'").ToList().FirstOrDefault();
                int x = c.Dato;

                ClsDatoEntero b = db.Database.SqlQuery<ClsDatoEntero>("select count(IDPedido) as Dato from DetPedido where IDPedido='" + IDPedido + "' and Status = 'Finalizado'").ToList().FirstOrDefault();
                int y = b.Dato;

                if (x == y)
                {
                    db.Database.ExecuteSqlCommand("update [dbo].[EncPedido] set [Status]='Finalizado' where [IDPedido]='" + IDPedido + "'");
                }
                else
                {
                    db.Database.ExecuteSqlCommand("update [dbo].[EncPedido] set [Status]='Activo' where [IDPedido]='" + IDPedido + "'");
                }
            }
            catch (Exception err)
            {

            }



            return RedirectToAction("Index");
        }

        public ActionResult CintasRemision(int idremision)
        {
            ViewBag.remision = idremision;
            List<DetRemision> remision = db.Database.SqlQuery<DetRemision>("select d.* from articulo as a inner join detremision as d on a.idarticulo=d.idarticulo where a.idtipoarticulo=6 and idremision=" + idremision).ToList();

            var listado = new RemisionContext().DetRemisiones.Where(s => s.IDRemision == idremision).ToList();

            ViewBag.listado = listado;



            return View(remision);
        }




        [HttpPost]
        public ActionResult descontarlote(string loteinterno, int remision, FormCollection coleccion)
        {

            int largo = 0;
            decimal m2 = 0;

            string mensaje = "";
            string ancho1 = string.Empty;
            string largo1 = string.Empty;
            Clslotemp lotemp = new Clslotemp();
            DetRemision dremision = new DetRemision();
            try
            {
                try
                {
                    AlmacenContext db = new AlmacenContext();
                    lotemp = db.Database.SqlQuery<Clslotemp>("Select * from clslotemp where loteinterno='" + loteinterno + "'").ToList().FirstOrDefault();
                    string[] cadenas = loteinterno.Split('/');
                    largo = int.Parse(cadenas[2].ToString());
                    Caracteristica caracteristicas = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from Caracteristica where id=" + lotemp.IDCaracteristica).ToList().FirstOrDefault();
                    FormulaSiaapi.Formulas formu = new FormulaSiaapi.Formulas();

                    ancho1 = formu.getValorCadena("ANCHO", caracteristicas.Presentacion);

                    largo1 = formu.getValorCadena("LARGO", caracteristicas.Presentacion);


                    decimal metroscuadrados = decimal.Parse(largo.ToString()) * (Convert.ToDecimal(ancho1) / 1000);

                    m2 = Math.Round(metroscuadrados, 3);

                }
                catch (Exception noestaba)
                {
                    try
                    {
                        string[] cadenas = loteinterno.Split('/');
                        lotemp.IDAlmacen = 6;//almacen de materia primas


                    }
                    catch (Exception err)
                    {
                        mensaje = "No parece una cinta";
                    }
                }


                dremision = db.Database.SqlQuery<DetRemision>("Select * from DetRemision where idremision=" + remision + " and  idarticulo=" + lotemp.IDArticulo + " and Caracteristica_ID=" + lotemp.IDCaracteristica).ToList().FirstOrDefault();


                StringBuilder cadena = new StringBuilder();
                db.Database.ExecuteSqlCommand("update clslotemp set metrosutilizados=metroscuadrados,metrosdisponibles=0 where id=" + lotemp.ID);
                string cadenaremi = "INSERT INTO [dbo].[RemisionMP]([IDRemision],[IDDetRemision],[LoteInterno])VALUES (" + remision + "," + dremision.IDDetRemision + ",'" + loteinterno + "')";
                db.Database.ExecuteSqlCommand(cadenaremi);
                Caracteristica carateristica = new WorkinProcessContext().Database.SqlQuery<Caracteristica>("select * from caracteristica where id=" + lotemp.IDCaracteristica).ToList().FirstOrDefault();
                InventarioAlmacen inv = new InventarioAlmacenContext().InventarioAlmacenes.ToList().Where(s => s.IDAlmacen == lotemp.IDAlmacen && s.IDCaracteristica == lotemp.IDCaracteristica).FirstOrDefault();



                try
                {
                    string cadenam = "INSERT INTO [dbo].[MovimientoArticulo] ([Fecha],[Id],[IDPresentacion],[Articulo_IDArticulo],[Accion],[cantidad],[Documento],[NoDocumento],[Lote],[IDAlmacen],[TipoOperacion],[acumulado],[observacion],[Hora]) VALUES ";
                    cadenam += " (getdate(), " + carateristica.ID + "," + carateristica.IDPresentacion + "," + carateristica.Articulo_IDArticulo + ",'Remisión MP'," + dremision.Cantidad + ",'Remisión MP '," + remision + ",'" + loteinterno + "'," + lotemp.IDAlmacen + ",'S'," + inv.Existencia + ",'',CONVERT (time, SYSDATETIMEOFFSET()))";
                    //db.Database.ExecuteSqlCommand(cadenam);
                }
                catch (Exception err)
                {
                    string mensajee = err.Message;
                }





                return new HttpStatusCodeResult(200, "Bien"); //200 es ok para html
            }
            catch (Exception err)
            {
                return new HttpStatusCodeResult(500, "Bien"); //algo salio mal
            }



        }

        [HttpPost]
        public JsonResult Deleteremi(int id)
        {
            try
            {
                RemisionMP dremision = new RemisionMP();
                dremision = db.Database.SqlQuery<RemisionMP>("Select * from RemisionMP where IDRemisionMP='" + id + "'").ToList().FirstOrDefault();
                Clslotemp lote = new Clslotemp();
                lote = db.Database.SqlQuery<Clslotemp>("Select * from clslotemp where loteinterno='" + dremision.LoteInterno + "'").ToList().FirstOrDefault();

                db.Database.ExecuteSqlCommand("update clslotemp set metrosdisponibles=metroscuadrados, metrosutilizados=0  where ID =" + lote.ID);

                db.Database.ExecuteSqlCommand("delete from [dbo].[RemisionMP] where IDRemisionMP=" + id);




                return Json(true);
            }
            catch (Exception err)
            {
                return Json(500, err.Message);
            }
        }


        public ActionResult delete(int? id)
        {
            List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            int UserID = userid.Select(s => s.UserID).FirstOrDefault();

            CarritoContext cr = new CarritoContext();
            CarritoRemision carritoRemision = cr.CarritoRemisiones.Find(id);
            db.Database.ExecuteSqlCommand("delete from [dbo].[CarritoRemision] where [IDCarritoRemision]='" + id + "'");
            db.Database.ExecuteSqlCommand("update [dbo].[DetPedido] set [Status]='Activo' where [IDDetPedido]='" + carritoRemision.IDDetExterna + "' and [Status]='Recepcionado'");
            db.Database.ExecuteSqlCommand("update [dbo].[EncPedido] set [Status]='Activo' where [IDPedido]='" + carritoRemision.IDPedido + "' and [Status]='Recepcionado'");


            ClsDatoEntero dato = db.Database.SqlQuery<ClsDatoEntero>("select count(IDCarritoRemision) as Dato from CarritoRemision where UserID ='" + UserID + "' and IDPedido='" + carritoRemision.IDPedido + "'").ToList().FirstOrDefault();
            if (dato.Dato == 0)
            {
                return RedirectToAction("Index", "EncPedido");
            }
            ViewBag.id = carritoRemision.IDPedido;


            ClientesContext pr = new ClientesContext();
            EncPedido encPedido = db.EncPedidos.Find(carritoRemision.IDPedido);


            ViewBag.IDUsoCFDI = new SelectList(db.c_UsoCFDIS.Where(s => s.IDUsoCFDI.Equals(encPedido.IDUsoCFDI)), "IDUsoCFDI", "Descripcion");
            ViewBag.IDAlmacen = new SelectList(db.Almacenes.Where(s => s.IDAlmacen.Equals(encPedido.IDAlmacen)), "IDAlmacen", "Descripcion");

            var cliente = prov.Clientes.ToList();

            List<SelectListItem> moneda = new List<SelectListItem>();
            c_Moneda monedap = pr.c_Monedas.Find(encPedido.IDMoneda);
            moneda.Add(new SelectListItem { Text = monedap.Descripcion, Value = monedap.IDMoneda.ToString() });
            moneda.Add(new SelectListItem { Text = "-------------------------", Value = "0" });
            var todosmoneda = prov.c_Monedas.ToList();
            if (todosmoneda != null)
            {
                foreach (var y in todosmoneda)
                {
                    moneda.Add(new SelectListItem { Text = y.Descripcion, Value = y.IDMoneda.ToString() });
                }
            }

            ViewBag.moneda = moneda;

            List<SelectListItem> metodo = new List<SelectListItem>();
            c_MetodoPago metodop = pr.c_MetodoPagos.Find(encPedido.IDMetodoPago);
            metodo.Add(new SelectListItem { Text = metodop.Descripcion, Value = metodop.IDMetodoPago.ToString() });
            ViewBag.metodo = metodo;

            List<SelectListItem> forma = new List<SelectListItem>();
            c_FormaPago formap = pr.c_FormaPagos.Find(encPedido.IDFormapago);
            forma.Add(new SelectListItem { Text = formap.Descripcion, Value = formap.IDFormaPago.ToString() });
            ViewBag.forma = forma;

            List<SelectListItem> condiciones = new List<SelectListItem>();
            CondicionesPago condicionesp = pr.CondicionesPagos.Find(encPedido.IDCondicionesPago);
            condiciones.Add(new SelectListItem { Text = condicionesp.Descripcion, Value = condicionesp.IDCondicionesPago.ToString() });
            ViewBag.condiciones = condiciones;

            List<SelectListItem> vendedor = new List<SelectListItem>();
            Vendedor vendedorp = pr.Vendedores.Find(encPedido.IDVendedor);
            vendedor.Add(new SelectListItem { Text = vendedorp.Nombre, Value = vendedorp.IDVendedor.ToString() });
            ViewBag.vendedor = vendedor;

            List<SelectListItem> li = new List<SelectListItem>();
            Clientes mm = pr.Clientes.Find(encPedido.IDCliente);
            li.Add(new SelectListItem { Text = mm.Nombre, Value = mm.IDCliente.ToString() });
            ViewBag.cliente = li;

            List<SelectListItem> entrega = new List<SelectListItem>();
            entrega.Add(new SelectListItem { Text = encPedido.Entrega, Value = encPedido.Entrega });
            ViewBag.entrega = entrega;
            //List<VEncOrdenC> orden = db.Database.SqlQuery<VEncOrdenC>("select EncOrdenCompra.IDOrdenCompra, CONVERT(VARCHAR(10),EncOrdenCompra.Fecha,103) AS Fecha,CONVERT(VARCHAR(10),EncOrdenCompra.FechaRequiere,103) AS FechaRequiere,Proveedores.Empresa as Proveedor from EncOrdenCompra INNER JOIN Proveedores ON EncOrdenCompra.IDProveedor= Proveedores.IDProveedor where  EncOrdenCompra.IDOrdenCompra='" + id + "' and EncOrdenCompra.Status='Activo'").ToList();

            List<VEncPedido> orden = db.Database.SqlQuery<VEncPedido>("select EncPedido.IDPedido, CONVERT(VARCHAR(10),EncPedido.Fecha,103) AS Fecha,CONVERT(VARCHAR(10),EncPedido.FechaRequiere,103) AS FechaRequiere,Clientes.Nombre as Cliente from EncPedido INNER JOIN Clientes ON EncPedido.IDCliente=Clientes.IDCliente where  EncPedido.IDPedido='" + carritoRemision.IDPedido + "'").ToList();
            ViewBag.data = orden;

            ClsDatoEntero denc = db.Database.SqlQuery<ClsDatoEntero>("select count(IDPedido) as Dato from EncPedido where IDPedido='" + carritoRemision.IDPedido + "' and (Status='Activo' or Status ='PreFacturado')").ToList()[0];
            ViewBag.otro = denc.Dato;
            //ViewBag.data = null;

            //ClsDatoEntero denc = db.Database.SqlQuery<ClsDatoEntero>("select count(IDOrdenCompra) as Dato from EncOrdenCompra where IDOrdenCompra='" + id + "' and Status='Activo'").ToList()[0];

            //ViewBag.otro = 0;

            //List<VDetOrdenCompra> elementos = db.Database.SqlQuery<VDetOrdenCompra>("select DetOrdenCompra.IDDetOrdenCompra,DetOrdenCompra.IDOrdenCompra,DetOrdenCompra.Suministro,Articulo.Descripcion as Articulo,DetOrdenCompra.Cantidad,DetOrdenCompra.Costo,DetOrdenCompra.CantidadPedida,DetOrdenCompra.Descuento,DetOrdenCompra.Importe,DetOrdenCompra.IVA,DetOrdenCompra.ImporteIva,DetOrdenCompra.ImporteTotal, DetOrdenCompra.Nota,DetOrdenCompra.Ordenado, Caracteristica.Presentacion as Presentacion,Caracteristica.jsonPresentacion as jsonPresentacion  from  DetOrdenCompra INNER JOIN Caracteristica ON DetOrdenCompra.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo where IDOrdenCompra='" + id + "' and Status='Activo'").ToList();

            ViewBag.datos = null;
            ViewBag.IDAlmacenP = db.Almacenes.ToList();

            List<VCarritoRemision> lista = db.Database.SqlQuery<VCarritoRemision>("select CarritoRemision.IDArticulo,CarritoRemision.IDAlmacen,CarritoRemision.Lote,CarritoRemision.IDDetExterna,CarritoRemision.IDCarritoRemision,CarritoRemision.IDPedido,CarritoRemision.Suministro,Articulo.Descripcion as Articulo,CarritoRemision.Cantidad,CarritoRemision.Costo,CarritoRemision.CantidadPedida,CarritoRemision.Descuento,CarritoRemision.Importe,CarritoRemision.IVA,CarritoRemision.ImporteIva,CarritoRemision.ImporteTotal,CarritoRemision.Nota,CarritoRemision.Ordenado, Caracteristica.Presentacion as Presentacion,Caracteristica.ID as IDCaracteristica,Caracteristica.jsonPresentacion as jsonPresentacion  from  CarritoRemision INNER JOIN Caracteristica ON CarritoRemision.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo where UserID='" + UserID + "' and IDPedido='" + carritoRemision.IDPedido + "'").ToList();
            ClsDatoEntero c = db.Database.SqlQuery<ClsDatoEntero>("select count(IDCarritoRemision) as Dato from CarritoRemision where  UserID='" + UserID + "' and IDPedido='" + carritoRemision.IDPedido + "'").ToList()[0];
            ViewBag.dato = c.Dato;
            var resumen = db.Database.SqlQuery<ResumenFac>("select (select ClaveMoneda FROM C_MONEDA WHERE IDMoneda=EncPedido.IDMoneda) as MonedaOrigen, (SUM(Importe)) as Subtotal, (SUM(ImporteIva)) as IVA, (SUM(ImporteTotal)) as Total, (SUM(ImporteTotal * EncPedido.TipoCambio)) as TotalenPesos from CarritoRemision inner join EncPedido on EncPedido.IDPedido=CarritoRemision.IDPedido where CarritoRemision.UserID='" + UserID + "' and CarritoRemision.IDPedido='" + carritoRemision.IDPedido + "' group by EncPedido.IDMoneda").ToList();
            ViewBag.sumatoria = resumen;

            return View("Pedido", lista);

        }

        //public ActionResult delete(int? id)
        //{
        //    CarritoContext cr = new CarritoContext();
        //    CarritoRemision carritoRemision = cr.CarritoRemisiones.Find(id);
        //    db.Database.ExecuteSqlCommand("delete from [dbo].[CarritoRemision] where [IDCarritoRemision]='" + id + "'");
        //    db.Database.ExecuteSqlCommand("update [dbo].[DetPedido] set [Status]='Activo' where [IDDetPedido]='" + carritoRemision.IDDetExterna + "'");
        //    db.Database.ExecuteSqlCommand("update [dbo].[EncPedido] set [Status]='Activo' where [IDPedido]='" + carritoRemision.IDPedido + "'");

        //    return RedirectToAction("Pedido");

        //}
        //////////////////////////////////////////////////////////////Devolución///////////////////////////////////////////////////////////////////////////////////
        public ActionResult DevolucionR(int? id)
        {


            List<VEncPedido> orden = db.Database.SqlQuery<VEncPedido>("select EncRemision.IDRemision as IDPedido, CONVERT(VARCHAR(10),EncRemision.Fecha,103) AS Fecha,EncRemision.Observacion AS FechaRequiere,Clientes.Nombre as Cliente from EncRemision INNER JOIN Clientes ON EncRemision.IDCliente= Clientes.IDCliente where EncRemision.IDRemision='" + id + "' and EncRemision.Status <>'Cancelado'").ToList();
            ViewBag.data = orden;

            ClsDatoEntero denc = db.Database.SqlQuery<ClsDatoEntero>("select count(IDRemision) as Dato from EncRemision where IDRemision='" + id + "' and (Status='Activo' or status='Prefacturado')").ToList().FirstOrDefault();
            int w = denc.Dato;
            ViewBag.otro = w;

            List<VDetRemision> elementos = db.Database.SqlQuery<VDetRemision>("select DetRemision.IDDetRemision,DetRemision.IDRemision,DetRemision.Suministro,Articulo.Descripcion as Articulo,DetRemision.Cantidad,DetRemision.Costo,DetRemision.CantidadPedida,DetRemision.Descuento,DetRemision.Importe,DetRemision.IVA,DetRemision.ImporteIva,DetRemision.ImporteTotal,DetRemision.Nota,DetRemision.Ordenado, Caracteristica.Presentacion as Presentacion,Caracteristica.jsonPresentacion as jsonPresentacion  from  DetRemision INNER JOIN Caracteristica ON DetRemision.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo where IDRemision='" + id + "' and (Status='Activo' or status='Prefacturado')").ToList();
            ViewBag.datos = elementos;

            ClsDatoEntero dcompra = db.Database.SqlQuery<ClsDatoEntero>("select count(IDRemision) as Dato from DetRemision where IDRemision='" + id + "' and Status='Activo'").ToList().FirstOrDefault();

            List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            int UserID = userid.Select(s => s.UserID).FirstOrDefault();


            if (id != null && denc.Dato > 0 && dcompra.Dato > 0)
            {
                for (int i = 0; i < elementos.Count(); i++)
                {
                    RemisionContext dboc = new RemisionContext();
                    DetRemision detRemision = dboc.DetRemisiones.Find(ViewBag.datos[i].IDDetRemision);


                    db.Database.ExecuteSqlCommand("INSERT INTO CarritoDevolucionR([IDDetExterna],[IDRemision],[IDArticulo],[Cantidad],[Costo],[CantidadPedida],[Descuento],[Importe],[IVA],[ImporteIva],[ImporteTotal],[Nota],[Ordenado],[Caracteristica_ID],[IDAlmacen],[Suministro],[Status],[Presentacion],[jsonPresentacion],[Lote]) values ('" + ViewBag.datos[i].IDDetRemision + "','" + ViewBag.datos[i].IDRemision + "','" + detRemision.IDArticulo + "','" + detRemision.Cantidad + "','" + detRemision.Costo + "','" + detRemision.Devolucion + "','" + detRemision.Descuento + "','" + detRemision.Importe + "','" + detRemision.IVA + "','" + detRemision.ImporteIva + "','" + detRemision.ImporteTotal + "','" + detRemision.Nota + "','" + detRemision.Ordenado + "','" + detRemision.Caracteristica_ID + "','" + detRemision.IDAlmacen + "','" + detRemision.Suministro + "','Activo','" + ViewBag.datos[i].Presentacion + "', '" + ViewBag.datos[i].jsonPresentacion + "','" + detRemision.Lote + "')");


                }
                //db.Database.ExecuteSqlCommand("update [dbo].[DetRemision] set [Status]='Devuelto' where [IDRemision]='" + id + "' and [Status]='Activo'");
                //db.Database.ExecuteSqlCommand("update [dbo].[EncRemision] set [Status]='Devuelto' where [IDRemision]='" + id + "'");
                db.Database.ExecuteSqlCommand("update [dbo].[CarritoDevolucionR] set [UserID]='" + UserID + "' where [IDRemision]='" + id + "'");
            }


            List<VCarritoDevolucionR> lista = db.Database.SqlQuery<VCarritoDevolucionR>("select * from  CarritoDevolucionR  where UserID='" + UserID + "' and IDRemision='" + id + "'").ToList();
            ClsDatoEntero c = db.Database.SqlQuery<ClsDatoEntero>("select count(IDCarritoDevolucionR) as Dato from CarritoDevolucionR where UserID='" + UserID + "' and IDRemision='" + id + "'").ToList().FirstOrDefault();
            int x = c.Dato;
            ViewBag.dato = x;
            var resumen = db.Database.SqlQuery<ResumenFac>("select (select ClaveMoneda FROM C_MONEDA WHERE IDMoneda=EncRemision.IDMoneda) as MonedaOrigen, (SUM(Importe)) as Subtotal, (SUM(ImporteIva)) as IVA, (SUM(ImporteTotal)) as Total, (SUM(ImporteTotal * EncRemision.TipoCambio)) as TotalenPesos from CarritoDevolucionR inner join EncRemision on EncRemision.IDRemision=CarritoDevolucionR.IDRemision where CarritoDevolucionR.UserID='" + UserID + "' and CarritoDevolucionR.IDRemision='" + id + "' group by EncRemision.IDMoneda").ToList();
            ViewBag.sumatoria = resumen;
            return View(lista);
        }

        public ActionResult deletedev(int? id)
        {

            CarritoContext cr = new CarritoContext();
            CarritoDevolucionR carritoDevolucionr = cr.CarritoDevolucionRes.Find(id);
            List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            int UserID = userid.Select(s => s.UserID).FirstOrDefault();
            db.Database.ExecuteSqlCommand("delete from [dbo].[CarritoDevolucionR] where [IDCarritoDevolucionR]='" + id + "'");
            db.Database.ExecuteSqlCommand("update [dbo].[DetRemision] set [Status]='Activo' where [IDDetRemision]='" + carritoDevolucionr.IDDetExterna + "'");
            db.Database.ExecuteSqlCommand("update [dbo].[EncRemision] set [Status]='Activo' where [IDRemision]='" + carritoDevolucionr.IDRemision + "'");

            ClsDatoEntero dato = db.Database.SqlQuery<ClsDatoEntero>("select count(IDCarritoDevolucionR) as Dato from CarritoDevolucionR where UserID ='" + UserID + "' and IDRemision='" + carritoDevolucionr.IDRemision + "'").ToList()[0];
            if (dato.Dato == 0)
            {
                return RedirectToAction("Index");
            }
            ViewBag.id = carritoDevolucionr.IDRemision;
            ViewBag.datos = null;
            ViewBag.data = null;
            ViewBag.otro = 0;

            List<VCarritoDevolucionR> lista = db.Database.SqlQuery<VCarritoDevolucionR>("select CarritoDevolucionR.Lote,CarritoDevolucionR.IDDetExterna,CarritoDevolucionR.IDCarritoDevolucionR,CarritoDevolucionR.IDRemision,CarritoDevolucionR.Suministro,Articulo.Descripcion as Articulo,CarritoDevolucionR.Cantidad,CarritoDevolucionR.Costo,CarritoDevolucionR.CantidadPedida,CarritoDevolucionR.Descuento,CarritoDevolucionR.Importe,CarritoDevolucionR.IVA,CarritoDevolucionR.ImporteIva,CarritoDevolucionR.ImporteTotal,CarritoDevolucionR.Nota,CarritoDevolucionR.Ordenado, Caracteristica.Presentacion as Presentacion,Caracteristica.ID as ID_Caracteristica,Caracteristica.jsonPresentacion as jsonPresentacion  from  CarritoDevolucionR INNER JOIN Caracteristica ON CarritoDevolucionR.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo where UserID='" + UserID + "' and IDRemision='" + carritoDevolucionr.IDRemision + "'").ToList();

            var resumen = db.Database.SqlQuery<ResumenFac>("select (select ClaveMoneda FROM C_MONEDA WHERE IDMoneda=EncRemision.IDMoneda) as MonedaOrigen, (SUM(Importe)) as Subtotal, (SUM(ImporteIva)) as IVA, (SUM(ImporteTotal)) as Total, (SUM(ImporteTotal * EncRemision.TipoCambio)) as TotalenPesos from CarritoDevolucionR inner join EncRemision on EncRemision.IDRemision=CarritoDevolucionR.IDRemision where CarritoDevolucionR.UserID='" + UserID + "' and CarritoDevolucionR.IDRemision='" + carritoDevolucionr.IDRemision + "' group by EncRemision.IDMoneda").ToList();
            ViewBag.sumatoria = resumen;

            return View("DevolucionR", lista);
        }

        [HttpPost]
        public ActionResult updateD(List<VCarritoDevolucionR> cr)
        {
            int id = 0;
            decimal subtotal = 0, iva = 0, total = 0, subtotalr = 0, ivar = 0, totalr = 0, cantidad = 0, Precio = 0, devuelve = 0, CantidadTotal = 0;
            string fecha = DateTime.Now.ToString("yyyyMMdd");

            List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            int UserID = userid.Select(s => s.UserID).FirstOrDefault();
            foreach (var i in cr)
            {
                id = i.IDRemision;
            }

            db.Database.ExecuteSqlCommand("insert into [dbo].[EncDevolucionR] ([Fecha],[Observacion],[DocumentoFactura],[IDCliente],[IDFormaPago],[IDMoneda],[IDMetodoPago],[IDCondicionesPago],[IDAlmacen],[TipoCambio],[UserID],[IDUsoCFDI],[Subtotal],[IVA],[Total],[Status],[IDVendedor]) select [Fecha],[Observacion],[DocumentoFactura],[IDCliente],[IDFormaPago],[IDMoneda],[IDMetodoPago],[IDCondicionesPago],[IDAlmacen],[TipoCambio],[UserID],[IDUsoCFDI],[Subtotal],[IVA],[Total],[Status],[IDVendedor] from [dbo].[EncRemision] where IDRemision='" + id + "'");

            List<EncDevolucionR> numero = db.Database.SqlQuery<EncDevolucionR>("select * from [dbo].[EncDevolucionR] WHERE IDDevolucionR = (SELECT MAX(IDDevolucionR) from EncDevolucionR)").ToList();
            int num = numero.Select(s => s.IDDevolucionR).FirstOrDefault();


            db.Database.ExecuteSqlCommand("update [dbo].[EncDevolucionR] set [Status]='Activo',[Fecha]='" + fecha + "',[UserID]='" + UserID + "' where [IDDevolucionR]='" + num + "'");

            foreach (var i in cr)
            {
                RemisionContext oc = new RemisionContext();
                DetRemision detRemision = oc.DetRemisiones.Find(i.IDDetExterna);

                cantidad = detRemision.Cantidad - detRemision.Devolucion;
                devuelve = detRemision.Devolucion + i.Cantidad;
                Precio = detRemision.Costo;
                if (cantidad == i.Cantidad)
                {
                    db.Database.ExecuteSqlCommand("update [dbo].[DetRemision] set [Status]='Finalizado',[CantidadPedida]='" + i.Cantidad + "',[Devolucion]='" + devuelve + "' where [IDDetRemision]='" + i.IDDetExterna + "'");
                    CantidadTotal = i.Cantidad;
                }

                else if (cantidad > i.Cantidad)
                {
                    decimal cantidadpedida = cantidad - i.Cantidad;
                    db.Database.ExecuteSqlCommand("update [dbo].[DetRemision] set [CantidadPedida]='" + cantidadpedida + "',[Status]='Activo',[Devolucion]='" + devuelve + "' where [IDDetRemision]='" + i.IDDetExterna + "'");
                    CantidadTotal = i.Cantidad;
                }
                else if (cantidad < i.Cantidad)
                {
                    devuelve = detRemision.Devolucion + cantidad;
                    db.Database.ExecuteSqlCommand("update [dbo].[DetRemision] set [CantidadPedida]='" + cantidad + "',[Status]=Finalizado',[Devolucion]='" + devuelve + "' where [IDDetRemision]='" + i.IDDetExterna + "'");
                    CantidadTotal = cantidad;

                }

                subtotal = i.Cantidad * Precio;
                iva = subtotal * (decimal)0.16;
                total = subtotal + iva;

                db.Database.ExecuteSqlCommand("insert into DetDevolucionR([IDDevolucionR],[IDRemision],[IDDetRemision],[IDArticulo],[Caracteristica_ID],[Cantidad],[Costo],[Importe],[ImporteIva],[ImporteTotal],[Nota],[Lote],[IDAlmacen],[Status]) values ('" + num + "','" + detRemision.IDRemision + "', '" + i.IDDetExterna + "', '" + detRemision.IDArticulo + "', '" + detRemision.Caracteristica_ID + "', '" + CantidadTotal + "','" + Precio + "','" + subtotal + "','" + iva + "','" + total + "','" + detRemision.Nota + "','" + detRemision.Lote + "','" + detRemision.IDAlmacen + "','Activo')");

                List<DetDevolucion> numero2 = db.Database.SqlQuery<DetDevolucion>("select * from [dbo].[DetDevolucionR] WHERE IDDetDevolucionR = (SELECT MAX(IDDetDevolucionR) from DetDevolucionR)").ToList();
                int num2 = numero2.Select(s => s.IDDetDevolucion).FirstOrDefault();

                db.Database.ExecuteSqlCommand("insert into [dbo].[MovComercial] ([DocumentoDestino],[IDDestino],[Cantidad],[Fecha],[IDOrigen],[DocumentoOrigen],[UserID],[IDDetOrigen],[IDDetDestino]) values('Devolución','" + num + "','" + CantidadTotal + "','" + fecha + "','" + i.IDRemision + "','Recepción','" + UserID + "','" + i.IDDetExterna + "','" + num2 + "')");
                //db.Database.ExecuteSqlCommand("exec dbo.MovArt'" + fecha + "'," + detRemision.Caracteristica_ID + ",'DevVta'," + CantidadTotal + ",'Devolución de venta'," + num2 + ",'" + detRemision.Lote + "','" + detRemision.IDAlmacen + "','" + detRemision.Nota + "',0");

                try
                {

                    Caracteristica carateristica = new WorkinProcessContext().Database.SqlQuery<Caracteristica>("select * from caracteristica where id=" + i.Caracteristica_ID).ToList().FirstOrDefault();
                    Articulo ar = new ArticuloContext().Articulo.Find(carateristica.Articulo_IDArticulo);
                    InventarioAlmacen inv = new InventarioAlmacenContext().InventarioAlmacenes.ToList().Where(s => s.IDAlmacen == i.IDAlmacen && s.IDCaracteristica == i.Caracteristica_ID).FirstOrDefault();
                    if (inv != null)
                    {
                        if (ar.CtrlStock)
                        {

                            db.Database.ExecuteSqlCommand("update InventarioAlmacen set  Existencia=(Existencia+" + i.Cantidad + ") where IDArticulo= " + carateristica.Articulo_IDArticulo + " and IDCaracteristica=" + i.Caracteristica_ID + " and IDAlmacen=" + i.IDAlmacen + "");

                            db.Database.ExecuteSqlCommand("update InventarioAlmacen set  Disponibilidad=(Existencia-Apartado) where IDArticulo= " + carateristica.Articulo_IDArticulo + " and IDCaracteristica=" + i.Caracteristica_ID + " and IDAlmacen=" + i.IDAlmacen + "");

                        }
                    }
                    InventarioAlmacen inv1 = new InventarioAlmacenContext().InventarioAlmacenes.ToList().Where(s => s.IDAlmacen == i.IDAlmacen && s.IDCaracteristica == i.Caracteristica_ID).FirstOrDefault();


                    string cadenam = "INSERT INTO [dbo].[MovimientoArticulo] ([Fecha],[Id],[IDPresentacion],[Articulo_IDArticulo],[Accion],[cantidad],[Documento],[NoDocumento],[Lote],[IDAlmacen],[TipoOperacion],[acumulado],[observacion],[Hora]) VALUES ";
                    cadenam += " (getdate(), " + carateristica.ID + "," + carateristica.IDPresentacion + "," + carateristica.Articulo_IDArticulo + ",'Devolución'," + i.Cantidad + ",' Devolución de una Remisión '," + id + ",''," + i.IDAlmacen + ",'E'," + inv1.Existencia + ",'',CONVERT (time, SYSDATETIMEOFFSET()))";
                    db.Database.ExecuteSqlCommand(cadenam);
                }
                catch (Exception err)
                {
                    string mensajee = err.Message;
                }
                db.Database.ExecuteSqlCommand("delete from [dbo].[CarritoDevolucionR] where [IDCarritoDevolucionR]='" + i.IDCarritoDevolucionR + "'");

                //Devoluciones previas en Artículo
                db.Database.ExecuteSqlCommand("update [dbo].[Articulo] set [ExistenDev]='1' where IDArticulo='" + detRemision.IDArticulo + "'");

                ClsDatoEntero c = db.Database.SqlQuery<ClsDatoEntero>("select count(IDRemision) as Dato from DetRemision where IDRemision ='" + i.IDRemision + "'").ToList()[0];
                int x = c.Dato;

                ClsDatoEntero b = db.Database.SqlQuery<ClsDatoEntero>("select count(IDRemision) as Dato from DetRemision where IDRemision ='" + i.IDRemision + "' and (Status='Devuelto' or Status='Finalizado')").ToList().FirstOrDefault();
                int y = b.Dato;

                if (x == y)
                {
                    db.Database.ExecuteSqlCommand("update [dbo].[EncRemision] set [Status]='Finalizado' where [IDRemision]='" + i.IDRemision + "'");
                    db.Database.ExecuteSqlCommand("update [dbo].[DetRemision] set [Status]='Finalizado' where [IDRemision]='" + i.IDRemision + "'");
                }
                else
                {
                    db.Database.ExecuteSqlCommand("update [dbo].[EncRemision] set [Status]='Activo' where [IDRemision]='" + i.IDRemision + "'");
                }



            }


            List<DetDevolucionR> datostotales = db.Database.SqlQuery<DetDevolucionR>("select * from dbo.DetDevolucionR where IDDevolucionR='" + num + "'").ToList();
            subtotalr = datostotales.Sum(s => s.Importe);
            ivar = subtotalr * (decimal)0.16;
            totalr = subtotalr + ivar;
            db.Database.ExecuteSqlCommand("update [dbo].[EncDevolucionR] set [Subtotal]='" + subtotalr + "',[IVA]='" + ivar + "',[Total]='" + totalr + "' where [IDDevolucionR]='" + num + "'");


            return RedirectToAction("IndexDevolucionR");
        }

        public ActionResult IndexDevolucionR(string Divisa, string Status, string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
        {


            var SerLst = new List<string>();
            var SerQry = from d in BD.c_Monedas
                         orderby d.IDMoneda
                         select d.ClaveMoneda;
            SerLst.Add(" ");
            SerLst.AddRange(SerQry.Distinct());
            ViewBag.Divisa = new SelectList(SerLst);

            var StaLst = new List<string>();
            var StaQry = from d in BD.EncDevolucioneRs
                         orderby d.IDDevolucionR
                         select d.Status;
            StaLst.Add(" ");
            StaLst.AddRange(StaQry.Distinct());
            ViewBag.Status = new SelectList(StaLst);


            var elementos = (from s in BD.EncDevolucioneRs
                             select s).OrderByDescending(s => s.IDDevolucionR);


            var resumen = db.Database.SqlQuery<ResumenFac>("select (select ClaveMoneda FROM C_MONEDA WHERE IDMoneda=EncDevolucionR.IDMoneda) as MonedaOrigen, (SUM(Subtotal)) as Subtotal, (SUM(IVA)) as IVA, (SUM(Total)) as Total, (SUM(Total * TipoCambio)) as TotalenPesos from EncDevolucionR where [Status]<>'Cancelado' group by EncDevolucionR.IDMoneda").ToList();
            ViewBag.sumatoria = resumen;


            if (!String.IsNullOrEmpty(searchString))
            {
                elementos = (from s in BD.EncDevolucioneRs
                             select s).OrderByDescending(s => s.IDDevolucionR);

                elementos = elementos.Where(s => s.IDDevolucionR.ToString().Contains(searchString) || s.Clientes.Nombre.Contains(searchString)).OrderByDescending(s => s.IDDevolucionR);

                var filtro = db.Database.SqlQuery<ResumenFac>("select (select ClaveMoneda FROM C_MONEDA WHERE IDMoneda=EncDevolucionR.IDMoneda) as MonedaOrigen, (SUM(Subtotal)) as Subtotal, (SUM(IVA)) as IVA, (SUM(Total)) as Total, (SUM(Total * TipoCambio)) as TotalenPesos from EncDevolucionR inner join clientes on Clientes.IDCliente=EncDevolucionR.IDCliente where (CAST(EncDevolucionR.IDDevolucionR AS nvarchar(max))='" + searchString + "' or Clientes.Nombre='" + searchString + "') and [Status]<>'Cancelado' group by EncDevolucionR.IDMoneda ").ToList();
                ViewBag.sumatoria = filtro;


            }
            //Filtro Divisa
            if (!String.IsNullOrEmpty(Divisa))
            {
                elementos = (from s in BD.EncDevolucioneRs
                             select s).OrderByDescending(s => s.IDDevolucionR);
                elementos = elementos.Where(s => s.c_Moneda.ClaveMoneda == Divisa).OrderByDescending(s => s.IDDevolucionR);

                var divisa = db.Database.SqlQuery<ResumenFac>("select (select ClaveMoneda FROM C_MONEDA WHERE IDMoneda=EncDevolucionR.IDMoneda) as MonedaOrigen, (SUM(Subtotal)) as Subtotal, (SUM(IVA)) as IVA, (SUM(Total)) as Total, (SUM(Total * TipoCambio)) as TotalenPesos from EncDevolucionR inner join c_Moneda on c_Moneda.IDMoneda=EncDevolucionR.IDMoneda  where c_Moneda.ClaveMoneda='" + Divisa + "' and [Status]<>'Cancelado' group by EncDevolucionR.IDMoneda").ToList();
                ViewBag.sumatoria = divisa;


            }

            //Filtro Status
            if (!String.IsNullOrEmpty(Status))
            {
                elementos = (from s in BD.EncDevolucioneRs
                             select s).OrderByDescending(s => s.IDDevolucionR);
                elementos = elementos.Where(s => s.Status.Equals(Status)).OrderByDescending(s => s.IDDevolucionR);

                var divisa = db.Database.SqlQuery<ResumenFac>("select (select ClaveMoneda FROM C_MONEDA WHERE IDMoneda=EncDevolucionR.IDMoneda) as MonedaOrigen, (SUM(Subtotal)) as Subtotal, (SUM(IVA)) as IVA, (SUM(Total)) as Total, (SUM(Total * TipoCambio)) as TotalenPesos from EncDevolucionR inner join c_Moneda on c_Moneda.IDMoneda=EncDevolucionR.IDMoneda  where Status='" + Status + "' group by EncDevolucionR.IDMoneda").ToList();
                ViewBag.sumatoria = divisa;


            }


            ViewBag.CurrentSort = sortOrder;
            ViewBag.DevolucionSortParm = String.IsNullOrEmpty(sortOrder) ? "Devolucion" : "Devolucion";
            ViewBag.ClienteSortParm = String.IsNullOrEmpty(sortOrder) ? "Cliente" : "Cliente";

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

            ViewBag.CurrentFilter = searchString;

            //Ordenacion

            switch (sortOrder)
            {
                case "Devolucion":
                    elementos = elementos.OrderByDescending(s => s.IDDevolucionR);
                    break;
                case "Cliente":
                    elementos = elementos.OrderByDescending(s => s.Clientes.Nombre);
                    break;
                default:
                    elementos = elementos.OrderByDescending(s => s.IDDevolucionR);
                    break;
            }

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = BD.EncDevolucioneRs.Count(); // Total number of elements

            // Populate DropDownList
            ViewBag.PageSize = new List<SelectListItem>()
            {
                new SelectListItem { Text = "10 ", Value = "10", Selected = true },
                new SelectListItem { Text = "25", Value = "25" },
                new SelectListItem { Text = "50", Value = "50" },
                new SelectListItem { Text = "100", Value = "100" },
                new SelectListItem { Text = "Todos", Value = count.ToString() }
             };

            int pageNumber = (page ?? 1);
            int pageSize = (PageSize ?? 10);
            ViewBag.psize = pageSize;

            return View(elementos.ToPagedList(pageNumber, pageSize));



        }

        public ActionResult DetailsDevolucionR(int? id)
        {
            List<VDetDevolucionR> orden = BD.Database.SqlQuery<VDetDevolucionR>("select DetDevolucionR.IDDetDevolucionR,DetDevolucionR.IDDevolucionR,DetDevolucionR.IDRemision,DetDevolucionR.IDDetRemision,Articulo.Descripcion as Articulo,DetDevolucionR.Cantidad,DetDevolucionR.Costo,DetDevolucionR.Importe,DetDevolucionR.ImporteIva,DetDevolucionR.ImporteTotal,DetDevolucionR.Nota,Caracteristica.Presentacion as Presentacion,Caracteristica.jsonPresentacion as jsonPresentacion,DetDevolucionR.Status,DetDevolucionR.Lote from  DetDevolucionR INNER JOIN Caracteristica ON DetDevolucionR.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo where IDDevolucionR='" + id + "'").ToList();

            ViewBag.req = orden;

            var filtro = BD.Database.SqlQuery<ResumenFac>("select (select ClaveMoneda FROM C_MONEDA WHERE IDMoneda=EncDevolucionR.IDMoneda) as MonedaOrigen, (SUM(Subtotal)) as Subtotal, (SUM(IVA)) as IVA, (SUM(Total)) as Total, (SUM(Total * TipoCambio)) as TotalenPesos from EncDevolucionR inner join Clientes on Clientes.IDCliente=EncDevolucionR.IDCliente  where EncDevolucionR.IDDevolucionR='" + id + "' group by EncDevolucionR.IDMoneda ").ToList();
            ViewBag.sumatoria = filtro;


            EncDevolucionR encDevolucionR = BD.EncDevolucioneRs.Find(id);

            return View(encDevolucionR);
        }
        ///////////////////////////////////////////////////////////////////////////////////////////Prefactura////////////////////////////////////////////////////////////////////////////////////////
        public ActionResult PrefacturaR()
        {
            List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            int UserID = userid.Select(s => s.UserID).FirstOrDefault();
            db.Database.ExecuteSqlCommand("delete CarritoPrefacturaR where UserID='" + UserID + "'");
            ViewBag.data = null;
            ViewBag.otro = 0;
            ViewBag.idnull = 0;
            return View();
        }
        [HttpPost]
        public ActionResult PrefacturaR(int? id)
        {
            List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            int UserID = userid.Select(s => s.UserID).FirstOrDefault();
            ClsDatoEntero w = db.Database.SqlQuery<ClsDatoEntero>("select count(IDCarritoPrefacturaR) as Dato from CarritoPrefacturaR where UserID='" + UserID + "'").ToList().FirstOrDefault();
            ViewBag.idnull = 1;
            if (id == null && w.Dato == 0)
            {
                ViewBag.idnull = 0;
                ViewBag.otro = 0;
                ViewBag.dato = 0;
                return View();
            }
            else if (w.Dato != 0 && id == null)
            {
                ClsDatoEntero maxidrem = db.Database.SqlQuery<ClsDatoEntero>("select max(IDCarritoPrefacturaR) as Dato from CarritoPrefacturaR  where UserID='" + UserID + "'").ToList().FirstOrDefault();
                ClsDatoEntero idrem = db.Database.SqlQuery<ClsDatoEntero>("select IDRemision as Dato from CarritoPrefacturaR where UserID='" + UserID + "' and IDCarritoPrefacturaR='" + maxidrem.Dato + "'").ToList().FirstOrDefault();

                c_TipoRelacionContext trelacion = new c_TipoRelacionContext();
                FolioVentasContext folio = new FolioVentasContext();
                EncRemision remisionc = new RemisionContext().EncRemisiones.Find(idrem.Dato);
                ViewBag.IDCondicionesPago = new SelectList(db.CondicionesPagos, "IDCondicionesPago", "Descripcion", remisionc.IDCondicionesPago);
                ViewBag.IDFormapago = new SelectList(db.c_FormaPagos, "IDFormaPago", "Descripcion", remisionc.IDFormapago);
                ViewBag.IDMetodoPago = new SelectList(db.c_MetodoPagos, "IDMetodoPago", "Descripcion", remisionc.IDMetodoPago);
                ViewBag.IDUsoCFDI = new SelectList(db.c_UsoCFDIS.OrderBy(s => s.Descripcion), "IDUsoCFDI", "Descripcion", remisionc.IDUsoCFDI);

                ViewBag.IDFolioVentas = new SelectList(folio.FoliosV, "IDFolioVentas", "Serie");

                List<SelectListItem> items = new SelectList(trelacion.c_TipoRelaciones, "IDTipoRelacion", "Descripcion").ToList();
                items.Insert(0, new SelectListItem { Selected = true, Text = "Sin relacion", Value = "" });
                SelectList relacion = new SelectList(items, "Value", "Text", null);


                ViewBag.IDTipoRelacion = relacion;


                ViewBag.otro = 0;
                List<VCarritoPrefacturaR> lista = db.Database.SqlQuery<VCarritoPrefacturaR>("select CarritoPrefacturaR.Lote,CarritoPrefacturaR.IDDetExterna,CarritoPrefacturaR.IDCarritoPrefacturaR,CarritoPrefacturaR.IDRemision,CarritoPrefacturaR.Suministro,Articulo.Descripcion as Articulo,CarritoPrefacturaR.Cantidad,CarritoPrefacturaR.Costo,CarritoPrefacturaR.CantidadPedida,CarritoPrefacturaR.Descuento,CarritoPrefacturaR.Importe,CarritoPrefacturaR.IVA,CarritoPrefacturaR.ImporteIva,CarritoPrefacturaR.ImporteTotal,CarritoPrefacturaR.Nota,CarritoPrefacturaR.Ordenado, Caracteristica.Presentacion as Presentacion,Caracteristica.ID as ID_Caracteristica,Caracteristica.jsonPresentacion as jsonPresentacion  from  CarritoPrefacturaR INNER JOIN Caracteristica ON CarritoPrefacturaR.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo where UserID='" + UserID + "'").ToList();
                ClsDatoEntero c = db.Database.SqlQuery<ClsDatoEntero>("select count(IDCarritoPrefacturaR) as Dato from CarritoPrefacturaR where UserID='" + UserID + "'").ToList().FirstOrDefault();
                int var = c.Dato;
                ViewBag.dato = var;
                var resumen = db.Database.SqlQuery<ResumenFac>("select (select ClaveMoneda FROM C_MONEDA WHERE IDMoneda=EncRemision.IDMoneda) as MonedaOrigen, (SUM(Importe)) as Subtotal, (SUM(ImporteIva)) as IVA, (SUM(ImporteTotal)) as Total, (SUM(ImporteTotal * EncRemision.TipoCambio)) as TotalenPesos from CarritoPrefacturaR inner join EncRemision on EncRemision.IDRemision=CarritoPrefacturaR.IDRemision where CarritoPrefacturaR.UserID='" + UserID + "' group by EncRemision.IDMoneda").ToList();
                ViewBag.sumatoria = resumen;
                return View(lista);

            }

            else if (id != null)
            {
                ClsDatoEntero countcc = db.Database.SqlQuery<ClsDatoEntero>("select count(IDCarritoPrefacturaR) as Dato from CarritoPrefacturaR where UserID='" + UserID + "'").ToList().FirstOrDefault();
                if (countcc.Dato != 0)
                {
                    ClsDatoEntero clienteremision = db.Database.SqlQuery<ClsDatoEntero>("select EncRemision.IDCliente as Dato from EncRemision inner join CarritoPrefacturaR  on EncRemision.IDRemision=CarritoPrefacturaR.IDRemision where EncRemision.IDRemision=CarritoPrefacturaR.IDRemision and CarritoPrefacturaR.UserID= "+ UserID).ToList()[0];
                    RemisionContext rem = new RemisionContext();
                    EncRemision encrem = rem.EncRemisiones.Find(id);

                    if (encrem == null)
                    {
                        ClsDatoEntero maxidrem = db.Database.SqlQuery<ClsDatoEntero>("select max(IDCarritoPrefacturaR) as Dato from CarritoPrefacturaR  where UserID='" + UserID + "'").ToList().FirstOrDefault();
                        ClsDatoEntero idrem = db.Database.SqlQuery<ClsDatoEntero>("select IDRemision as Dato from CarritoPrefacturaR where UserID='" + UserID + "' and IDCarritoPrefacturaR='" + maxidrem.Dato + "'").ToList().FirstOrDefault();

                        c_TipoRelacionContext trelacion = new c_TipoRelacionContext();
                        FolioVentasContext folio = new FolioVentasContext();
                        EncRemision remisionc = new RemisionContext().EncRemisiones.Find(idrem.Dato);
                        ViewBag.IDCondicionesPago = new SelectList(db.CondicionesPagos, "IDCondicionesPago", "Descripcion", remisionc.IDCondicionesPago);
                        ViewBag.IDFormapago = new SelectList(db.c_FormaPagos, "IDFormaPago", "Descripcion", remisionc.IDFormapago);
                        ViewBag.IDMetodoPago = new SelectList(db.c_MetodoPagos, "IDMetodoPago", "Descripcion", remisionc.IDMetodoPago);
                        ViewBag.IDUsoCFDI = new SelectList(db.c_UsoCFDIS.OrderBy(s => s.Descripcion), "IDUsoCFDI", "Descripcion", remisionc.IDUsoCFDI);

                        ViewBag.IDFolioVentas = new SelectList(folio.FoliosV, "IDFolioVentas", "Serie");

                        List<SelectListItem> items = new SelectList(trelacion.c_TipoRelaciones, "IDTipoRelacion", "Descripcion").ToList();
                        items.Insert(0, new SelectListItem { Selected = true, Text = "Sin relacion", Value = "" });
                        SelectList relacion = new SelectList(items, "Value", "Text", null);


                        ViewBag.IDTipoRelacion = relacion;


                        ViewBag.otro = 0;
                        List<VCarritoPrefacturaR> lista = db.Database.SqlQuery<VCarritoPrefacturaR>("select CarritoPrefacturaR.Lote,CarritoPrefacturaR.IDDetExterna,CarritoPrefacturaR.IDCarritoPrefacturaR,CarritoPrefacturaR.IDRemision,CarritoPrefacturaR.Suministro,Articulo.Descripcion as Articulo,CarritoPrefacturaR.Cantidad,CarritoPrefacturaR.Costo,CarritoPrefacturaR.CantidadPedida,CarritoPrefacturaR.Descuento,CarritoPrefacturaR.Importe,CarritoPrefacturaR.IVA,CarritoPrefacturaR.ImporteIva,CarritoPrefacturaR.ImporteTotal,CarritoPrefacturaR.Nota,CarritoPrefacturaR.Ordenado, Caracteristica.Presentacion as Presentacion,Caracteristica.ID as ID_Caracteristica,Caracteristica.jsonPresentacion as jsonPresentacion  from  CarritoPrefacturaR INNER JOIN Caracteristica ON CarritoPrefacturaR.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo where UserID='" + UserID + "'").ToList();
                        ClsDatoEntero c = db.Database.SqlQuery<ClsDatoEntero>("select count(IDCarritoPrefacturaR) as Dato from CarritoPrefacturaR where UserID='" + UserID + "'").ToList().FirstOrDefault();
                        int var = c.Dato;
                        ViewBag.dato = var;
                        var resumen = db.Database.SqlQuery<ResumenFac>("select (select ClaveMoneda FROM C_MONEDA WHERE IDMoneda=EncRemision.IDMoneda) as MonedaOrigen, (SUM(Importe)) as Subtotal, (SUM(ImporteIva)) as IVA, (SUM(ImporteTotal)) as Total, (SUM(ImporteTotal * EncRemision.TipoCambio)) as TotalenPesos from CarritoPrefacturaR inner join EncRemision on EncRemision.IDRemision=CarritoPrefacturaR.IDRemision where CarritoPrefacturaR.UserID='" + UserID + "' group by EncRemision.IDMoneda").ToList();
                        ViewBag.sumatoria = resumen;
                        return View(lista);
                    }

                    if (clienteremision.Dato != encrem.IDCliente)
                    {
                        ViewBag.mensaje = "La remisión que se desea agregar proviene de un cliente distinto, por lo tanto, no se puede continuar con la operación";


                        c_TipoRelacionContext trelacion = new c_TipoRelacionContext();
                        FolioVentasContext folio = new FolioVentasContext();
                        EncRemision remisionc = new RemisionContext().EncRemisiones.Find(id);
                        ViewBag.IDCondicionesPago = new SelectList(db.CondicionesPagos, "IDCondicionesPago", "Descripcion", remisionc.IDCondicionesPago);
                        ViewBag.IDFormapago = new SelectList(db.c_FormaPagos, "IDFormaPago", "Descripcion", remisionc.IDFormapago);
                        ViewBag.IDMetodoPago = new SelectList(db.c_MetodoPagos, "IDMetodoPago", "Descripcion", remisionc.IDMetodoPago);
                        ViewBag.IDUsoCFDI = new SelectList(db.c_UsoCFDIS.OrderBy(s => s.Descripcion), "IDUsoCFDI", "Descripcion", remisionc.IDUsoCFDI);


                        ViewBag.IDFolioVentas = new SelectList(folio.FoliosV, "IDFolioVentas", "Serie");

                        List<SelectListItem> items = new SelectList(trelacion.c_TipoRelaciones, "IDTipoRelacion", "Descripcion").ToList();
                        items.Insert(0, new SelectListItem { Selected = true, Text = "Sin relacion", Value = "" });
                        SelectList relacion = new SelectList(items, "Value", "Text", null);


                        ViewBag.IDTipoRelacion = relacion;
                        ViewBag.otro = 0;
                        List<VCarritoPrefacturaR> lista1 = db.Database.SqlQuery<VCarritoPrefacturaR>("select CarritoPrefacturaR.Lote,CarritoPrefacturaR.IDDetExterna,CarritoPrefacturaR.IDCarritoPrefacturaR,CarritoPrefacturaR.IDRemision,CarritoPrefacturaR.Suministro,Articulo.Descripcion as Articulo,CarritoPrefacturaR.Cantidad,CarritoPrefacturaR.Costo,CarritoPrefacturaR.CantidadPedida,CarritoPrefacturaR.Descuento,CarritoPrefacturaR.Importe,CarritoPrefacturaR.IVA,CarritoPrefacturaR.ImporteIva,CarritoPrefacturaR.ImporteTotal,CarritoPrefacturaR.Nota,CarritoPrefacturaR.Ordenado, Caracteristica.Presentacion as Presentacion,Caracteristica.ID as ID_Caracteristica,Caracteristica.jsonPresentacion as jsonPresentacion  from  CarritoPrefacturaR INNER JOIN Caracteristica ON CarritoPrefacturaR.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo where UserID='" + UserID + "'").ToList();
                        ClsDatoEntero c = db.Database.SqlQuery<ClsDatoEntero>("select count(IDCarritoPrefacturaR) as Dato from CarritoPrefacturaR where UserID='" + UserID + "'").ToList().FirstOrDefault();
                        int var1 = c.Dato;
                        ViewBag.dato = var1;
                        var resumen1 = db.Database.SqlQuery<ResumenFac>("select (select ClaveMoneda FROM C_MONEDA WHERE IDMoneda=EncRemision.IDMoneda) as MonedaOrigen, (SUM(Importe)) as Subtotal, (SUM(ImporteIva)) as IVA, (SUM(ImporteTotal)) as Total, (SUM(ImporteTotal * EncRemision.TipoCambio)) as TotalenPesos from CarritoPrefacturaR inner join EncRemision on EncRemision.IDRemision=CarritoPrefacturaR.IDRemision where CarritoPrefacturaR.UserID='" + UserID + "' group by EncRemision.IDMoneda").ToList();
                        ViewBag.sumatoria = resumen1;
                        return View(lista1);
                    }
                    else
                    {
                        c_TipoRelacionContext trelacion = new c_TipoRelacionContext();
                        FolioVentasContext folio = new FolioVentasContext();
                        EncRemision remisionc = new RemisionContext().EncRemisiones.Find(id);
                        ViewBag.IDCondicionesPago = new SelectList(db.CondicionesPagos, "IDCondicionesPago", "Descripcion", remisionc.IDCondicionesPago);
                        ViewBag.IDFormapago = new SelectList(db.c_FormaPagos, "IDFormaPago", "Descripcion", remisionc.IDFormapago);
                        ViewBag.IDMetodoPago = new SelectList(db.c_MetodoPagos, "IDMetodoPago", "Descripcion", remisionc.IDMetodoPago);
                        ViewBag.IDUsoCFDI = new SelectList(db.c_UsoCFDIS.OrderBy(s => s.Descripcion), "IDUsoCFDI", "Descripcion", remisionc.IDUsoCFDI);


                        ViewBag.IDFolioVentas = new SelectList(folio.FoliosV, "IDFolioVentas", "Serie");

                        List<SelectListItem> items = new SelectList(trelacion.c_TipoRelaciones, "IDTipoRelacion", "Descripcion").ToList();
                        items.Insert(0, new SelectListItem { Selected = true, Text = "Sin relacion", Value = "" });
                        SelectList relacion = new SelectList(items, "Value", "Text", null);


                        ViewBag.IDTipoRelacion = relacion;



                        List<VEncPedido> orden = db.Database.SqlQuery<VEncPedido>("select EncRemision.IDRemision as IDPedido, CONVERT(VARCHAR(10),EncRemision.Fecha,103) AS Fecha,EncRemision.Observacion AS FechaRequiere,Clientes.Nombre as Cliente from EncRemision INNER JOIN Clientes ON EncRemision.IDCliente= Clientes.IDCliente where EncRemision.IDRemision='" + id + "' and EncRemision.Status='Activo'").ToList();
                        ViewBag.data = orden;

                        ClsDatoEntero denc = db.Database.SqlQuery<ClsDatoEntero>("select count(IDRemision) as Dato from EncRemision where IDRemision='" + id + "' and Status='Activo'").ToList().FirstOrDefault();
                        int x = denc.Dato;
                        ViewBag.otro = x;

                        List<VDetRemision> elementos = db.Database.SqlQuery<VDetRemision>("select DetRemision.IDDetRemision,DetRemision.IDRemision,DetRemision.Suministro,Articulo.Descripcion as Articulo,DetRemision.Cantidad,DetRemision.Costo,DetRemision.CantidadPedida,DetRemision.Descuento,DetRemision.Importe,DetRemision.IVA,DetRemision.ImporteIva,DetRemision.ImporteTotal,DetRemision.Nota,DetRemision.Ordenado, Caracteristica.Presentacion as Presentacion,Caracteristica.jsonPresentacion as jsonPresentacion,DetRemision.IDAlmacen  from  DetRemision INNER JOIN Caracteristica ON DetRemision.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo where IDRemision='" + id + "' and Status='Activo'").ToList();
                        ViewBag.datos = elementos;

                        ClsDatoEntero dcompra = db.Database.SqlQuery<ClsDatoEntero>("select count(IDRemision) as Dato from DetRemision where IDRemision='" + id + "' and Status='Activo'").ToList().FirstOrDefault();



                        if (id != null && denc.Dato > 0 && dcompra.Dato > 0)
                        {
                            for (int i = 0; i < elementos.Count(); i++)
                            {
                                RemisionContext dboc = new RemisionContext();
                                DetRemision detRemision = dboc.DetRemisiones.Find(ViewBag.datos[i].IDDetRemision);


                                db.Database.ExecuteSqlCommand("INSERT INTO CarritoPrefacturaR([IDDetExterna],[IDRemision],[IDArticulo],[Cantidad],[Costo],[CantidadPedida],[Descuento],[Importe],[IVA],[ImporteIva],[ImporteTotal],[Nota],[Ordenado],[Caracteristica_ID],[IDAlmacen],[Suministro],[Status],[Presentacion],[jsonPresentacion],[Lote]) values ('" + ViewBag.datos[i].IDDetRemision + "','" + ViewBag.datos[i].IDRemision + "','" + detRemision.IDArticulo + "','" + detRemision.Cantidad + "','" + detRemision.Costo + "','" + detRemision.Devolucion + "','" + detRemision.Descuento + "','" + detRemision.Importe + "','" + detRemision.IVA + "','" + detRemision.ImporteIva + "','" + detRemision.ImporteTotal + "','" + detRemision.Nota + "','" + detRemision.Ordenado + "','" + detRemision.Caracteristica_ID + "','" + detRemision.IDAlmacen + "','" + detRemision.Suministro + "','Activo','" + ViewBag.datos[i].Presentacion + "', '" + ViewBag.datos[i].jsonPresentacion + "','" + detRemision.Lote + "')");


                            }
                            //db.Database.ExecuteSqlCommand("update [dbo].[DetRemision] set [Status]='PreFacturado' where [IDRemision]='" + id + "' and [Status]='Activo'");
                            //db.Database.ExecuteSqlCommand("update [dbo].[EncRemision] set [Status]='PreFacturado' where [IDRemision]='" + id + "'");
                            db.Database.ExecuteSqlCommand("update [dbo].[CarritoPrefacturaR] set [UserID]='" + UserID + "' where [IDRemision]='" + id + "'");
                        }


                        List<VCarritoPrefacturaR> lista = db.Database.SqlQuery<VCarritoPrefacturaR>("select CarritoPrefacturaR.Lote,CarritoPrefacturaR.IDDetExterna,CarritoPrefacturaR.IDCarritoPrefacturaR,CarritoPrefacturaR.IDRemision,CarritoPrefacturaR.Suministro,Articulo.Descripcion as Articulo,CarritoPrefacturaR.Cantidad,CarritoPrefacturaR.Costo,CarritoPrefacturaR.CantidadPedida,CarritoPrefacturaR.Descuento,CarritoPrefacturaR.Importe,CarritoPrefacturaR.IVA,CarritoPrefacturaR.ImporteIva,CarritoPrefacturaR.ImporteTotal,CarritoPrefacturaR.Nota,CarritoPrefacturaR.Ordenado, Caracteristica.Presentacion as Presentacion,Caracteristica.ID as ID_Caracteristica,Caracteristica.jsonPresentacion as jsonPresentacion  from  CarritoPrefacturaR INNER JOIN Caracteristica ON CarritoPrefacturaR.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo where UserID='" + UserID + "'").ToList();
                        ClsDatoEntero c = db.Database.SqlQuery<ClsDatoEntero>("select count(IDCarritoPrefacturaR) as Dato from CarritoPrefacturaR where UserID='" + UserID + "'").ToList().FirstOrDefault();
                        int var = c.Dato;
                        ViewBag.dato = var;
                        var resumen = db.Database.SqlQuery<ResumenFac>("select (select ClaveMoneda FROM C_MONEDA WHERE IDMoneda=EncRemision.IDMoneda) as MonedaOrigen, (SUM(Importe)) as Subtotal, (SUM(ImporteIva)) as IVA, (SUM(ImporteTotal)) as Total, (SUM(ImporteTotal * EncRemision.TipoCambio)) as TotalenPesos from CarritoPrefacturaR inner join EncRemision on EncRemision.IDRemision=CarritoPrefacturaR.IDRemision where CarritoPrefacturaR.UserID='" + UserID + "' group by EncRemision.IDMoneda").ToList();
                        ViewBag.sumatoria = resumen;
                        return View(lista);
                    }
                }
                else
                {
                    c_TipoRelacionContext trelacion = new c_TipoRelacionContext();
                    FolioVentasContext folio = new FolioVentasContext();
                    EncRemision remisionc = new RemisionContext().EncRemisiones.Find(id);
                    ViewBag.IDCondicionesPago = new SelectList(db.CondicionesPagos, "IDCondicionesPago", "Descripcion", remisionc.IDCondicionesPago);
                    ViewBag.IDFormapago = new SelectList(db.c_FormaPagos, "IDFormaPago", "Descripcion", remisionc.IDFormapago);
                    ViewBag.IDMetodoPago = new SelectList(db.c_MetodoPagos, "IDMetodoPago", "Descripcion", remisionc.IDMetodoPago);
                    ViewBag.IDUsoCFDI = new SelectList(db.c_UsoCFDIS.OrderBy(s => s.Descripcion), "IDUsoCFDI", "Descripcion", remisionc.IDUsoCFDI);


                    ViewBag.IDFolioVentas = new SelectList(folio.FoliosV, "IDFolioVentas", "Serie");

                    List<SelectListItem> items = new SelectList(trelacion.c_TipoRelaciones, "IDTipoRelacion", "Descripcion").ToList();
                    items.Insert(0, new SelectListItem { Selected = true, Text = "Sin relacion", Value = "" });
                    SelectList relacion = new SelectList(items, "Value", "Text", null);


                    ViewBag.IDTipoRelacion = relacion;



                    List<VEncPedido> orden = db.Database.SqlQuery<VEncPedido>("select EncRemision.IDRemision as IDPedido, CONVERT(VARCHAR(10),EncRemision.Fecha,103) AS Fecha,EncRemision.Observacion AS FechaRequiere,Clientes.Nombre as Cliente from EncRemision INNER JOIN Clientes ON EncRemision.IDCliente= Clientes.IDCliente where EncRemision.IDRemision='" + id + "' and EncRemision.Status='Activo'").ToList();
                    ViewBag.data = orden;

                    ClsDatoEntero denc = db.Database.SqlQuery<ClsDatoEntero>("select count(IDRemision) as Dato from EncRemision where IDRemision='" + id + "' and Status='Activo'").ToList().FirstOrDefault();
                    int x = denc.Dato;
                    ViewBag.otro = x;

                    List<VDetRemision> elementos = db.Database.SqlQuery<VDetRemision>("select DetRemision.IDDetRemision,DetRemision.IDRemision,DetRemision.Suministro,Articulo.Descripcion as Articulo,DetRemision.Cantidad,DetRemision.Costo,DetRemision.CantidadPedida,DetRemision.Descuento,DetRemision.Importe,DetRemision.IVA,DetRemision.ImporteIva,DetRemision.ImporteTotal,DetRemision.Nota,DetRemision.Ordenado, Caracteristica.Presentacion as Presentacion,Caracteristica.jsonPresentacion as jsonPresentacion  from  DetRemision INNER JOIN Caracteristica ON DetRemision.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo where IDRemision='" + id + "' and Status='Activo'").ToList();
                    ViewBag.datos = elementos;

                    ClsDatoEntero dcompra = db.Database.SqlQuery<ClsDatoEntero>("select count(IDRemision) as Dato from DetRemision where IDRemision='" + id + "' and Status='Activo'").ToList().FirstOrDefault();



                    if (id != null && denc.Dato > 0 && dcompra.Dato > 0)
                    {
                        for (int i = 0; i < elementos.Count(); i++)
                        {
                            RemisionContext dboc = new RemisionContext();
                            DetRemision detRemision = dboc.DetRemisiones.Find(ViewBag.datos[i].IDDetRemision);

                            string cadena = "INSERT INTO CarritoPrefacturaR([IDDetExterna],[IDRemision],[IDArticulo],[Cantidad],[Costo],[CantidadPedida],[Descuento],[Importe],[IVA],[ImporteIva],[ImporteTotal],[Nota],[Ordenado],[Caracteristica_ID],[IDAlmacen],[Suministro],[Status],[Presentacion],[jsonPresentacion],[Lote]) values ('" + ViewBag.datos[i].IDDetRemision + "','" + ViewBag.datos[i].IDRemision + "','" + detRemision.IDArticulo + "','" + detRemision.Cantidad + "','" + detRemision.Costo + "','" + detRemision.Devolucion + "','" + detRemision.Descuento + "','" + detRemision.Importe + "','" + detRemision.IVA + "','" + detRemision.ImporteIva + "','" + detRemision.ImporteTotal + "','" + detRemision.Nota + "','" + detRemision.Ordenado + "','" + detRemision.Caracteristica_ID + "','" + detRemision.IDAlmacen + "','" + detRemision.Suministro + "','Activo','" + ViewBag.datos[i].Presentacion + "', '" + ViewBag.datos[i].jsonPresentacion + "','" + detRemision.Lote + "')";

                            db.Database.ExecuteSqlCommand(cadena);
                            

                        }
                        //db.Database.ExecuteSqlCommand("update [dbo].[DetRemision] set [Status]='PreFacturado' where [IDRemision]='" + id + "' and [Status]='Activo'");
                        //db.Database.ExecuteSqlCommand("update [dbo].[EncRemision] set [Status]='PreFacturado' where [IDRemision]='" + id + "'");
                        db.Database.ExecuteSqlCommand("update [dbo].[CarritoPrefacturaR] set [UserID]='" + UserID + "' where [IDRemision]='" + id + "'");
                    }


                    List<VCarritoPrefacturaR> lista = db.Database.SqlQuery<VCarritoPrefacturaR>("select CarritoPrefacturaR.Lote,CarritoPrefacturaR.IDDetExterna,CarritoPrefacturaR.IDCarritoPrefacturaR,CarritoPrefacturaR.IDRemision,CarritoPrefacturaR.Suministro,Articulo.Descripcion as Articulo,CarritoPrefacturaR.Cantidad,CarritoPrefacturaR.Costo,CarritoPrefacturaR.CantidadPedida,CarritoPrefacturaR.Descuento,CarritoPrefacturaR.Importe,CarritoPrefacturaR.IVA,CarritoPrefacturaR.ImporteIva,CarritoPrefacturaR.ImporteTotal,CarritoPrefacturaR.Nota,CarritoPrefacturaR.Ordenado, Caracteristica.Presentacion as Presentacion,Caracteristica.ID as ID_Caracteristica,Caracteristica.jsonPresentacion as jsonPresentacion  from  CarritoPrefacturaR INNER JOIN Caracteristica ON CarritoPrefacturaR.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo where UserID='" + UserID + "'").ToList();
                    ClsDatoEntero c = db.Database.SqlQuery<ClsDatoEntero>("select count(IDCarritoPrefacturaR) as Dato from CarritoPrefacturaR where UserID='" + UserID + "'").ToList().FirstOrDefault();
                    int var = c.Dato;
                    ViewBag.dato = var;
                    var resumen = db.Database.SqlQuery<ResumenFac>("select (select ClaveMoneda FROM C_MONEDA WHERE IDMoneda=EncRemision.IDMoneda) as MonedaOrigen, (SUM(Importe)) as Subtotal, (SUM(ImporteIva)) as IVA, (SUM(ImporteTotal)) as Total, (SUM(ImporteTotal * EncRemision.TipoCambio)) as TotalenPesos from CarritoPrefacturaR inner join EncRemision on EncRemision.IDRemision=CarritoPrefacturaR.IDRemision where CarritoPrefacturaR.UserID='" + UserID + "' group by EncRemision.IDMoneda").ToList();
                    ViewBag.sumatoria = resumen;
                    return View(lista);
                }
            }
            else
            {
                ViewBag.mensaje = "Remisión Inexistente";
            }
            return View();

        }

        public ActionResult deletepr(int? id)
        {
            c_TipoRelacionContext trelacion = new c_TipoRelacionContext();
            FolioVentasContext folio = new FolioVentasContext();
            EncRemision remisionc = new RemisionContext().EncRemisiones.Find(id);
            ViewBag.IDCondicionesPago = new SelectList(db.CondicionesPagos, "IDCondicionesPago", "Descripcion", remisionc.IDCondicionesPago);
            ViewBag.IDFormapago = new SelectList(db.c_FormaPagos, "IDFormaPago", "Descripcion", remisionc.IDFormapago);
            ViewBag.IDMetodoPago = new SelectList(db.c_MetodoPagos, "IDMetodoPago", "Descripcion", remisionc.IDMetodoPago);
            ViewBag.IDUsoCFDI = new SelectList(db.c_UsoCFDIS.OrderBy(s => s.Descripcion), "IDUsoCFDI", "Descripcion", remisionc.IDUsoCFDI);


            ViewBag.IDFolioVentas = new SelectList(folio.FoliosV, "IDFolioVentas", "Serie");

            List<SelectListItem> items = new SelectList(trelacion.c_TipoRelaciones, "IDTipoRelacion", "Descripcion").ToList();
            items.Insert(0, new SelectListItem { Selected = true, Text = "Sin relacion", Value = "" });
            SelectList relacion = new SelectList(items, "Value", "Text", null);


            ViewBag.IDTipoRelacion = relacion;
            CarritoContext cr = new CarritoContext();
            CarritoPrefacturaR carritoPrefacturar = cr.CarritoPrefacturaRs.Find(id);
            List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            int UserID = userid.Select(s => s.UserID).FirstOrDefault();
            db.Database.ExecuteSqlCommand("delete from [dbo].[CarritoPrefacturaR] where [IDCarritoPrefacturaR]='" + id + "'");
            db.Database.ExecuteSqlCommand("update [dbo].[DetRemision] set [Status]='Activo' where [IDDetRemision]='" + carritoPrefacturar.IDDetExterna + "'");
            db.Database.ExecuteSqlCommand("update [dbo].[EncRemision] set [Status]='Activo' where [IDRemision]='" + carritoPrefacturar.IDRemision + "'");

            ClsDatoEntero dato = db.Database.SqlQuery<ClsDatoEntero>("select count(IDCarritoPrefacturaR) as Dato from CarritoPrefacturaR where UserID ='" + UserID + "'").ToList().FirstOrDefault();
            if (dato.Dato == 0)
            {
                return RedirectToAction("Index");
            }
            ViewBag.otro = 0;
            ViewBag.dato = 0;


            List<VCarritoPrefacturaR> lista = db.Database.SqlQuery<VCarritoPrefacturaR>("select CarritoPrefacturaR.Lote,CarritoPrefacturaR.IDDetExterna,CarritoPrefacturaR.IDCarritoPrefacturaR,CarritoPrefacturaR.IDRemision,CarritoPrefacturaR.Suministro,Articulo.Descripcion as Articulo,CarritoPrefacturaR.Cantidad,CarritoPrefacturaR.Costo,CarritoPrefacturaR.CantidadPedida,CarritoPrefacturaR.Descuento,CarritoPrefacturaR.Importe,CarritoPrefacturaR.IVA,CarritoPrefacturaR.ImporteIva,CarritoPrefacturaR.ImporteTotal,CarritoPrefacturaR.Nota,CarritoPrefacturaR.Ordenado, Caracteristica.Presentacion as Presentacion,Caracteristica.ID as ID_Caracteristica,Caracteristica.jsonPresentacion as jsonPresentacion  from  CarritoPrefacturaR INNER JOIN Caracteristica ON CarritoPrefacturaR.Caracteristica_ID= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo where UserID='" + UserID + "'").ToList();
            ClsDatoEntero c = db.Database.SqlQuery<ClsDatoEntero>("select count(IDCarritoPrefacturaR) as Dato from CarritoPrefacturaR where UserID='" + UserID + "'").ToList().FirstOrDefault();
            int var = c.Dato;
            ViewBag.dato = var;
            var resumen = db.Database.SqlQuery<ResumenFac>("select (select ClaveMoneda FROM C_MONEDA WHERE IDMoneda=EncRemision.IDMoneda) as MonedaOrigen, (SUM(Importe)) as Subtotal, (SUM(ImporteIva)) as IVA, (SUM(ImporteTotal)) as Total, (SUM(ImporteTotal * EncRemision.TipoCambio)) as TotalenPesos from CarritoPrefacturaR inner join EncRemision on EncRemision.IDRemision=CarritoPrefacturaR.IDRemision where CarritoPrefacturaR.UserID='" + UserID + "' group by EncRemision.IDMoneda").ToList();
            ViewBag.sumatoria = resumen;

            return View("PrefacturaR", lista);
        }
        [HttpPost]

        public ActionResult PrefacturaRemision(List<VCarritoPrefacturaR> cr, FolioVentas folioventas, string Observacion, FormCollection collection, int? IDTipoRelacion, int? IDCondicionesPago, int? IDFormapago, int? IDMetodoPago, int? IDUsoCFDI)
        {


            List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            int UserID = userid.Select(s => s.UserID).FirstOrDefault();
            int contador = 0, numerofolio = 0;
            FolioVentasContext folio = new FolioVentasContext();
            FolioVentas foliov = folio.FoliosV.Find(folioventas.IDFolioVentas);
            string Serie = foliov.Serie;
            numerofolio = foliov.Numero + 1;

            decimal subtotal = 0, iva = 0, total = 0, importe = 0, importetotal = 0, importeiva = 0, cantidad = 0;
            ClsDatoEntero c = db.Database.SqlQuery<ClsDatoEntero>("select IDRemision as Dato from CarritoPrefacturaR where IDCarritoPrefacturaR=(select MAX(IDCarritoPrefacturaR) from CarritoPrefacturaR) and UserID='" + UserID + "'").ToList()[0];

            EncRemision encRemision = BD.EncRemisiones.Find(c.Dato);

            string fecha = DateTime.Now.ToString("yyyyMMdd");


            BD.Database.ExecuteSqlCommand("insert into [dbo].[EncPrefactura] ([Fecha],[Observacion],[UserID],[DocumentoFactura],[Subtotal],[IVA],[Total],[IDCliente],[IDMetodoPago],[IDFormaPago],[IDMoneda],[IDCondicionesPago],[IDAlmacen],[IDUsoCFDI],[TipoCambio],[Status],[IDVendedor],[Serie],[Numero],[IDFacturaDigital],[SerieDigital],[NumeroDigital],[UUID],[Entrega],[IDTipoRelacion]) values('" + fecha + "','" + Observacion + "', '" + UserID + "','0', '0', '0', '0', '" + encRemision.IDCliente + "','" + encRemision.IDMetodoPago + "','" + encRemision.IDFormapago + "', '" + encRemision.IDMoneda + "', '" + encRemision.IDCondicionesPago + "','" + encRemision.IDAlmacen + "', '" + encRemision.IDUsoCFDI + "', '" + encRemision.TipoCambio + "','Activo','" + encRemision.IDVendedor + "','" + Serie + "','" + numerofolio + "','0','0','0','0','" + encRemision.Entrega + "','" + IDTipoRelacion + "')");

            BD.Database.ExecuteSqlCommand("update [dbo].[FolioVentas] set [Numero]='" + numerofolio + "' where [IDFolioVentas]='" + folioventas.IDFolioVentas + "'");

            List<EncPrefactura> numero = BD.Database.SqlQuery<EncPrefactura>("select * from [dbo].[EncPrefactura] WHERE IDPrefactura = (SELECT MAX(IDPrefactura) from EncPrefactura)").ToList();
            int num = numero.Select(s => s.IDPrefactura).FirstOrDefault();

            foreach (var key in collection.AllKeys)
            {
                var value = collection[key];
                if (key.Contains("campo"))
                //contador++;
                //if (contador >= 8)
                {
                    BD.Database.ExecuteSqlCommand("insert into [dbo].[PrefacturaAnticipo] ([IDPrefactura],[IDFacturaAnticipo],[UUIDAnticipo]) values('" + num + "','0','" + value + "')");

                }
            }


            if (encRemision.IDCliente == 50)
            {
                foreach (VCarritoPrefacturaR i in cr)
                {
                    DetRemision detalle = BD.Database.SqlQuery<DetRemision>("select * from dbo.DetRemision where IDDetRemision=" + i.IDDetExterna + "").ToList().FirstOrDefault();
                    // ViewBag.pedidov = detPedido;
                    BD.Database.ExecuteSqlCommand("insert into DetPrefactura([IDPrefactura],[IDExterna],[IDArticulo],[Cantidad],[Costo],[CantidadPedida],[Descuento],[Importe],[IVA],[ImporteIva],[ImporteTotal],[Nota],[Ordenado],[Caracteristica_ID],[IDAlmacen],[Suministro],[Status],[IDDetExterna],[Presentacion],[jsonPresentacion],[Lote],[Devolucion],[Proviene]) values ('" + num + "','" + detalle.IDRemision + "','" + detalle.IDArticulo + "','" + detalle.Cantidad + "','" + detalle.Costo + "','0','0','" + detalle.Importe + "','true','" + detalle.ImporteIva + "','" + importetotal + "','" + detalle.Nota + "','0','" + detalle.Caracteristica_ID + "','" + detalle.IDAlmacen + "','0','Activo','" + detalle.IDDetRemision + "','" + detalle.Presentacion + "','" + detalle.jsonPresentacion + "','" + detalle.Lote + "','0','Remision')");

                    List<DetPrefactura> numero2 = db.Database.SqlQuery<DetPrefactura>("select * from [dbo].[DetPrefactura] WHERE IDDetPrefactura = (SELECT MAX(IDDetPrefactura) from DetPrefactura)").ToList();
                    int num2 = numero2.Select(s => s.IDDetPrefactura).FirstOrDefault();

                    BD.Database.ExecuteSqlCommand("insert into [dbo].[MovComercial] ([DocumentoDestino],[IDDestino],[Cantidad],[Fecha],[IDOrigen],[DocumentoOrigen],[UserID],[IDDetOrigen],[IDDetDestino]) values('Prefactura','" + num + "','" + detalle.Cantidad + "','" + fecha + "','" + detalle.IDRemision + "','Remisión','" + UserID + "','" + detalle.IDDetRemision + "','" + num2 + "')");
                    BD.Database.ExecuteSqlCommand("insert into [dbo].[elementosprefactura] ([documento],[iddocumento],[iddetdocumento],cantidad,iddetprefactura,idprefactura ) values('Remision'," + detalle.IDRemision + "," + detalle.IDDetRemision + "," + detalle.Cantidad + "," + num2 + "," + num + ")");

                    db.Database.ExecuteSqlCommand("update [dbo].[DetRemision] set [Status]='PreFacturado' where [IDRemision]='" + detalle.IDDetRemision + "' and [Status]='Activo'");
                    db.Database.ExecuteSqlCommand("update [dbo].[EncRemision] set [Status]='PreFacturado' where [IDRemision]='" + detalle.IDRemision + "'");
                }
            }
            else
            {
                //db.Database.ExecuteSqlCommand("update [dbo].[DetRemision] set [Status]='Activo' where [Status]='Devuelto' and IDRemision='" + id + "'");
                //db.Database.ExecuteSqlCommand("delete from [dbo].[CarritoDevolucionR] where IDRemision='" + id + "'");
                Hashtable tabla = new Hashtable();
                foreach (var i in cr)
                {
                    DetRemision detalle = BD.Database.SqlQuery<DetRemision>("select * from dbo.DetRemision where IDDetRemision=" + i.IDDetExterna + "").ToList().FirstOrDefault();
                    // ViewBag.pedidov = detPedido;


                    db.Database.ExecuteSqlCommand("update [dbo].[DetRemision] set [Status]='PreFacturado' where [IDRemision]='" + detalle.IDDetRemision + "' and [Status]='Activo'");
                    db.Database.ExecuteSqlCommand("update [dbo].[EncRemision] set [Status]='PreFacturado' where [IDRemision]='" + detalle.IDRemision + "'");


                    string llave = detalle.IDArticulo + "," + detalle.Caracteristica_ID;

                    if (!tabla.ContainsKey(llave))
                    {
                        tabla.Add(llave, detalle);
                        BD.Database.ExecuteSqlCommand("insert into DetPrefactura([IDPrefactura],[IDExterna],[IDArticulo],[Cantidad],[Costo],[CantidadPedida],[Descuento],[Importe],[IVA],[ImporteIva],[ImporteTotal],[Nota],[Ordenado],[Caracteristica_ID],[IDAlmacen],[Suministro],[Status],[IDDetExterna],[Presentacion],[jsonPresentacion],[Lote],[Devolucion],[Proviene]) values ('" + num + "','" + detalle.IDRemision + "','" + detalle.IDArticulo + "','" + detalle.Cantidad + "','" + detalle.Costo + "','0','0','" + detalle.Importe + "','true','" + detalle.ImporteIva + "','" + importetotal + "','" + detalle.Nota + "','0','" + detalle.Caracteristica_ID + "','" + detalle.IDAlmacen + "','0','Activo','" + detalle.IDDetRemision + "','" + detalle.Presentacion + "','" + detalle.jsonPresentacion + "','" + detalle.Lote + "','0','Remision')");

                        List<DetPrefactura> numero2 = db.Database.SqlQuery<DetPrefactura>("select * from [dbo].[DetPrefactura] WHERE IDDetPrefactura = (SELECT MAX(IDDetPrefactura) from DetPrefactura)").ToList();
                        int num2 = numero2.Select(s => s.IDDetPrefactura).FirstOrDefault();

                        BD.Database.ExecuteSqlCommand("insert into [dbo].[MovComercial] ([DocumentoDestino],[IDDestino],[Cantidad],[Fecha],[IDOrigen],[DocumentoOrigen],[UserID],[IDDetOrigen],[IDDetDestino]) values('Prefactura','" + num + "','" + detalle.Cantidad + "','" + fecha + "','" + detalle.IDRemision + "','Remisión','" + UserID + "','" + detalle.IDDetRemision + "','" + num2 + "')");
                        BD.Database.ExecuteSqlCommand("insert into [dbo].[elementosprefactura] ([documento],[iddocumento],[iddetdocumento],cantidad,iddetprefactura,idprefactura ) values('Remision'," + detalle.IDRemision + "," + detalle.IDDetRemision + "," + detalle.Cantidad + "," + num2 + "," + num + ")");
                    }
                    else
                    {
                        DetRemision anterior = (DetRemision)tabla[llave];
                        DetRemision nuevo = anterior;
                        nuevo.Cantidad = nuevo.Cantidad + detalle.Cantidad;
                        nuevo.CantidadPedida = nuevo.CantidadPedida + detalle.CantidadPedida;
                        nuevo.Importe = Math.Round(nuevo.Cantidad * nuevo.Costo, 2);
                        nuevo.ImporteIva = Math.Round(nuevo.Importe * (decimal.Parse(SIAAPI.Properties.Settings.Default.ValorIVA)), 2);
                        nuevo.ImporteTotal = nuevo.Importe + nuevo.ImporteIva;
                        if (anterior.Lote != nuevo.Lote)
                        { nuevo.Lote = anterior.Lote + " " + nuevo.Lote; }

                        tabla.Remove(llave);
                        tabla.Add(llave, nuevo);

                        List<DetPrefactura> numero2 = db.Database.SqlQuery<DetPrefactura>("select * from [dbo].[DetPrefactura] WHERE IDArticulo = " + detalle.IDArticulo + " and Caracteristica_ID=" + detalle.Caracteristica_ID + " and idprefactura=" + num + "").ToList();
                        int num2 = numero2.Select(s => s.IDDetPrefactura).FirstOrDefault();
                        db.Database.ExecuteSqlCommand("update detprefactura set cantidad=" + nuevo.Cantidad + ",cantidadpedida=" + nuevo.CantidadPedida + ",importe=" + nuevo.Importe + ",importeiva=" + nuevo.ImporteIva + ", importetotal=" + nuevo.ImporteTotal + " where iddetprefactura=" + num2);
                        db.Database.ExecuteSqlCommand("insert into [dbo].[MovComercial] ([DocumentoDestino],[IDDestino],[Cantidad],[Fecha],[IDOrigen],[DocumentoOrigen],[UserID],[IDDetOrigen],[IDDetDestino]) values('Prefactura','" + num + "','" + detalle.CantidadPedida + "','" + fecha + "','" + detalle.IDRemision + "','Remision','" + UserID + "','" + detalle.IDDetRemision + "'," + num2 + ")");
                        db.Database.ExecuteSqlCommand("insert into [dbo].[elementosprefactura] ([documento],[iddocumento],[iddetdocumento],cantidad,iddetprefactura,idprefactura ) values('Remision'," + detalle.IDRemision + "," + detalle.IDDetRemision + "," + detalle.CantidadPedida + "," + num2 + "," + num + ")");

                    }

                      db.Database.ExecuteSqlCommand("update [dbo].[EncRemision] set [Status]='PreFacturado' where [IDRemision]='" + detalle.IDRemision + "'");
                    BD.Database.ExecuteSqlCommand("delete CarritoPrefacturaR where [IDCarritoPrefacturaR]='" + i.IDCarritoPrefacturaR + "'");

                }
            }
            //db.Database.ExecuteSqlCommand("update [dbo].[EncRemision] set [Status]='PreFacturado' where [IDRemision]='" + id + "'");

            List<DetPrefactura> datostotales2 = db.Database.SqlQuery<DetPrefactura>("select * from dbo.DetPrefactura where IDPrefactura='" + num + "'").ToList();
            subtotal = Math.Round(datostotales2.Sum(s => s.Importe), 2);
            iva = Math.Round(subtotal * (decimal.Parse(SIAAPI.Properties.Settings.Default.ValorIVA)), 2);
            total = subtotal + iva;
            BD.Database.ExecuteSqlCommand("update [dbo].[EncPrefactura] set [IDCondicionesPago]=" + IDCondicionesPago + ",[IDFormapago]=" + IDFormapago + ",[IDMetodoPago]=" + IDMetodoPago + ",[IDUsoCFDI]=" + IDUsoCFDI + ",[Subtotal]='" + subtotal + "',[IVA]='" + iva + "',[Total]='" + total + "' where [IDPrefactura]='" + num + "'");
            return RedirectToAction("Index");



        }




        //Reportes//

        public void PdfRemision(int id)
        {

            EncRemision Remision = new RemisionContext().EncRemisiones.Find(id);
            DocumentoRe x = new DocumentoRe();

            x.claveMoneda = Remision.c_Moneda.Descripcion;
            x.descuento = 0;
            x.fecha = Remision.Fecha.ToShortDateString();
            x.fechaRequerida = Remision.Fecha.ToShortDateString();
            x.Cliente = Remision.Clientes.Nombre;
            x.formaPago = Remision.c_FormaPago.ClaveFormaPago;
            x.metodoPago = Remision.c_MetodoPago.ClaveMetodoPago;
            x.RFCCliente = Remision.Clientes.RFC;
            x.TelefonoCliente = Remision.Clientes.Telefono;
            x.total = float.Parse(Remision.Total.ToString());
            x.subtotal = float.Parse(Remision.Subtotal.ToString());
            x.tipo_cambio = Remision.TipoCambio.ToString();
            x.serie = "";
            x.folio = Remision.IDRemision.ToString();
            x.UsodelCFDI = Remision.c_UsoCFDI.Descripcion;
            x.Almacen = Remision.Almacen.Descripcion;
            x.IDRemision = Remision.IDRemision;
            x.Empresa = Remision.Almacen.Telefono;
            x.condicionesdepago = Remision.CondicionesPago.Descripcion;


            ImpuestoRe iva = new ImpuestoRe();
            iva.impuesto = "IVA";
            iva.tasa = float.Parse(SIAAPI.Properties.Settings.Default.ValorIVA.ToString());
            iva.importe = float.Parse(Remision.IVA.ToString());


            x.impuestos.Add(iva);

            EmpresaContext dbe = new EmpresaContext();

            var empresa = dbe.empresas.Single(m => m.IDEmpresa == 2);
            x.Empresa = empresa.RazonSocial;
            x.Telefono = empresa.Telefono;
            x.RFC = empresa.RFC;
            x.Direccion = empresa.Calle + " " + empresa.NoExt + " " + empresa.NoInt + "\n" + empresa.Colonia + " \n" + empresa.Municipio + "," + empresa.Estados.Estado;
            x.firmadefinanzas = empresa.Director_finanzas;
            x.firmadecompras = empresa.Persona_de_Compras + "";

            List<DetRemision> detalles = db.Database.SqlQuery<DetRemision>("select * from [dbo].[DetRemision] where [IDRemision]= " + id).ToList();

            int contador = 1;
            foreach (var item in detalles)
            {
                ProductoRe producto = new ProductoRe();
                Articulo arti = new ArticuloContext().Articulo.Find(item.IDArticulo);
                Almacen alma = new AlmacenContext().Almacenes.Find(item.IDAlmacen);
                Caracteristica carateristica = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from caracteristica where id=" + item.Caracteristica_ID).ToList().FirstOrDefault();
                producto.IDPresentacion = carateristica.IDPresentacion;
                producto.ClaveProducto = arti.Cref;
                producto.idarticulo = arti.IDArticulo;
                producto.c_unidad = arti.c_ClaveUnidad.Nombre;
                producto.cantidad = item.Cantidad.ToString();
                producto.almacen = alma.CodAlm;
                producto.descripcion = arti.Descripcion;
                producto.valorUnitario = float.Parse(item.Costo.ToString());
                producto.v_unitario = float.Parse(item.Costo.ToString());
                producto.importe = float.Parse(item.Importe.ToString());
                ///
                if (item.Lote != "")
                {
                    producto.Observacion = "Lote: " + item.Lote;
                }
                producto.Presentacion = item.Presentacion; //item.presentacion;
                ///
                producto.numIdentificacion = contador.ToString();
                contador++;

                x.productos.Add(producto);

            }



            System.Drawing.Image logoempresa = byteArrayToImage(empresa.Logo);

            try
            {


                CreaRePDF documento = new CreaRePDF(logoempresa, x);

            }
            catch (Exception err)
            {
                String mensaje = err.Message;
            }
            RedirectToAction("Index");

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

        public void Activardetallepedido(int? _Remisioncancelada)
        {
            RemisionContext db = new RemisionContext();
            List<DetRemision> detallesremision = db.Database.SqlQuery<DetRemision>("Select * from DetRemision where IDRemision=" + _Remisioncancelada).ToList();

            foreach (DetRemision detalle in detallesremision)
            {
                if (detalle.Cantidad > 0)
                {
                    new RemisionContext().Database.ExecuteSqlCommand("update[Detpedido] set[Status] = 'Activo', suministro = suministro - " + detalle.Cantidad + " where Detpedido.IDDetpedido=" + detalle.IDDetExterna);
                    //db.Database.ExecuteSqlCommand("update[InventarioAlmacen] set Apartado = Apartado - " + detalle.Cantidad + " where IDCaracteristica =" + detalle.Caracteristica_ID + " and IDAlmacen = " + detalle.IDAlmacen);
                }
            }

            //db.Database.ExecuteSqlCommand("update [Detpedido] set [Status]='Activo', suministro= suministro-(select DetRemision.Cantidad from [DetRemision] inner join DetPedido on DetRemision.IDdetExterna=Detpedido.IDDetpedido   where DetRemision.IDRemision=" + id + " and DetRemision.cantidad>0 )  where DetPedido.IDDetPedido in (select IDdetExterna from [DetRemision]   where IDRemision=" + id + ")");
        }

        public List<SelectListItem> getEstados(string Seleccionado = "")
        {
            List<SelectListItem> StaLst = new List<SelectListItem>();
            SelectListItem nuevos1 = new SelectListItem();
            nuevos1.Value = "";
            nuevos1.Text = "";
            if (Seleccionado == "")
            {
                nuevos1.Selected = true;
            }
            StaLst.Add(nuevos1);
            SelectListItem nuevosa = new SelectListItem();
            nuevosa.Value = "Activo";
            nuevosa.Text = "Activo";
            if (Seleccionado == "Activo")
            {
                nuevosa.Selected = true;
            }
            StaLst.Add(nuevosa);
            SelectListItem nuevos2 = new SelectListItem();
            nuevos2.Value = "Prefacturado";
            nuevos2.Text = "Prefacturado";
            if (Seleccionado == "Prefacturado")
            {
                nuevos2.Selected = true;
            }
            StaLst.Add(nuevos2);
            SelectListItem nuevos3 = new SelectListItem();
            nuevos3.Value = "Cancelado";
            nuevos3.Text = "Cancelado";
            if (Seleccionado == "Cancelado")
            {
                nuevos3.Selected = true;
            }
            StaLst.Add(nuevos3);
            return StaLst;


        }

        public List<SelectListItem> getMonedas(string Seleccionada = "")
        {
            List<SelectListItem> divisasel = new List<SelectListItem>();
            List<c_Moneda> monedas = new c_MonedaContext().c_Monedas.ToList();
            SelectListItem nuevos1 = new SelectListItem();
            nuevos1.Value = "";
            nuevos1.Text = "";
            if (Seleccionada == "")
            {
                nuevos1.Selected = true;
            }
            divisasel.Add(nuevos1);
            foreach (c_Moneda elementodiv in monedas)
            {
                SelectListItem nuevo = new SelectListItem();
                nuevo.Value = elementodiv.ClaveMoneda;
                nuevo.Text = elementodiv.ClaveMoneda;
                if (elementodiv.ClaveMoneda == Seleccionada)
                { nuevo.Selected = true; }

                divisasel.Add(nuevo);
            }

            return divisasel;
        }


        public void generasalidakit(int IDArticulo, int IDCaracteristica, decimal cantidad, string fecha, VCarritoRemision i, int detalleremision, int idalmacenPedidoDet, int idremision)
        {
            Articulo articulo = new ArticuloContext().Articulo.Find(IDArticulo);
            if (articulo.esKit)
            {
                var kits = new KitContext().Database.SqlQuery<Kit>("select * from kit where IDARTICULO=" + IDArticulo).ToList();


                if (articulo.CtrlStock)
                {

                    InventarioAlmacen invKit = new InventarioAlmacenContext().InventarioAlmacenes.ToList().Where(a => a.IDAlmacen == idalmacenPedidoDet && a.IDCaracteristica == i.IDCaracteristica).FirstOrDefault();
                    decimal existenciakit = 0;

                    if (invKit != null)
                    {
                        /// kit
                        db.Database.ExecuteSqlCommand("update InventarioAlmacen set Apartado=0, Disponibilidad=0 where IDArticulo= " + articulo.IDArticulo + " and IDCaracteristica=" + i.IDCaracteristica + " and IDAlmacen=" + idalmacenPedidoDet + "");

                        // existenciakit = invKit.Existencia;
                    }

                    try
                    {
                        // movimiento kit
                        Caracteristica carateristicakit = new WorkinProcessContext().Database.SqlQuery<Caracteristica>("select * from caracteristica where id=" + i.IDCaracteristica).ToList().FirstOrDefault();

                        string cadenam = "INSERT INTO [dbo].[MovimientoArticulo] ([Fecha],[Id],[IDPresentacion],[Articulo_IDArticulo],[Accion],[cantidad],[Documento],[NoDocumento],[Lote],[IDAlmacen],[TipoOperacion],[acumulado],[observacion],[Hora]) VALUES ";
                        cadenam += " (getdate(), " + carateristicakit.ID + "," + carateristicakit.IDPresentacion + "," + articulo.IDArticulo + ",'Remisión',";
                        cadenam += cantidad + ",'Remisión Kit'," + idremision + ",'',";
                        cadenam += idalmacenPedidoDet + ",'N/A'," + (existenciakit) + ",'',CONVERT (time, SYSDATETIMEOFFSET()))";

                        db.Database.ExecuteSqlCommand(cadenam);
                    }
                    catch (Exception err)
                    {

                    }



                    foreach (Kit kitc in kits)
                    {
                        decimal cantidadreal = 0;
                        if (kitc.porcantidad == true)
                        {
                            cantidadreal = cantidad * kitc.Cantidad;
                        }
                        else if (kitc.porporcentaje == true)
                        {
                            cantidadreal = cantidad * (kitc.Cantidad / 100);
                        }
                        Articulo articulokit = new ArticuloContext().Articulo.Find(kitc.IDArticuloComp);
                        if (articulokit.CtrlStock)
                        {
                            try
                            {
                                if (idalmacenPedidoDet == i.IDAlmacen)
                                {
                                    Caracteristica carateristicai = new WorkinProcessContext().Database.SqlQuery<Caracteristica>("select * from caracteristica where id=" + kitc.IDCaracteristica).ToList().FirstOrDefault();



                                    db.Database.ExecuteSqlCommand("update InventarioAlmacen set  Existencia=(Existencia-" + cantidadreal + ") where IDArticulo= " + carateristicai.Articulo_IDArticulo + " and IDCaracteristica=" + carateristicai.ID + " and IDAlmacen=" + i.IDAlmacen + "");
                                    db.Database.ExecuteSqlCommand("update InventarioAlmacen set Apartado=(Apartado-" + cantidadreal + ") where IDArticulo= " + carateristicai.Articulo_IDArticulo + " and IDCaracteristica=" + carateristicai.ID + " and IDAlmacen=" + i.IDAlmacen + "");
                                    db.Database.ExecuteSqlCommand("update InventarioAlmacen set Apartado=0 where IDArticulo= " + carateristicai.Articulo_IDArticulo + " and IDCaracteristica=" + carateristicai.ID + " and IDAlmacen=" + i.IDAlmacen + " and Apartado<0");
                                    db.Database.ExecuteSqlCommand("update InventarioAlmacen set  Disponibilidad=Existencia where apartado=0 and IDArticulo= " + carateristicai.Articulo_IDArticulo + " and IDCaracteristica=" + carateristicai.ID + " and IDAlmacen=" + i.IDAlmacen + "");
                                    db.Database.ExecuteSqlCommand("update InventarioAlmacen set  Disponibilidad=(Disponibilidad-Apartado) where Apartado>0 and   IDArticulo= " + carateristicai.Articulo_IDArticulo + " and IDCaracteristica=" + carateristicai.ID + " and IDAlmacen=" + idalmacenPedidoDet + "");

                                    try
                                    {
                                        InventarioAlmacen inv = new InventarioAlmacenContext().InventarioAlmacenes.ToList().Where(s => s.IDAlmacen == i.IDAlmacen && s.IDCaracteristica == kitc.IDCaracteristica).FirstOrDefault();

                                        string cadenam = "INSERT INTO [dbo].[MovimientoArticulo] ([Fecha],[Id],[IDPresentacion],[Articulo_IDArticulo],[Accion],[cantidad],[Documento],[NoDocumento],[Lote],[IDAlmacen],[TipoOperacion],[acumulado],[observacion],[Hora]) VALUES ";
                                        cadenam += " (getdate(), " + carateristicai.ID + "," + carateristicai.IDPresentacion + "," + carateristicai.Articulo_IDArticulo + ",'Remisión'," + cantidadreal + ",'Remisión '," + idremision + ",''," + i.IDAlmacen + ",'S'," + inv.Existencia + ",'',CONVERT (time, SYSDATETIMEOFFSET()))";
                                        db.Database.ExecuteSqlCommand(cadenam);
                                    }
                                    catch (Exception err)
                                    {
                                        string mensajee = err.Message;
                                    }

                                }
                                else
                                {
                                    Caracteristica caracteristicaC = new WorkinProcessContext().Database.SqlQuery<Caracteristica>("select * from caracteristica where id=" + kitc.IDCaracteristica).ToList().FirstOrDefault();

                                    db.Database.ExecuteSqlCommand("update InventarioAlmacen set  Existencia=(Existencia-" + cantidadreal + ") where IDArticulo= " + caracteristicaC.Articulo_IDArticulo + " and IDCaracteristica=" + caracteristicaC.ID + " and IDAlmacen=" + i.IDAlmacen + "");
                                    db.Database.ExecuteSqlCommand("update InventarioAlmacen set Apartado=(Apartado-" + cantidadreal + ") where IDArticulo= " + caracteristicaC.Articulo_IDArticulo + " and IDCaracteristica=" + caracteristicaC.ID + " and IDAlmacen=" + idalmacenPedidoDet + "");
                                    db.Database.ExecuteSqlCommand("update InventarioAlmacen set Apartado=0 where IDArticulo= " + caracteristicaC.Articulo_IDArticulo + " and IDCaracteristica=" + caracteristicaC.ID + " and IDAlmacen=" + idalmacenPedidoDet + " and Apartado<0");
                                    db.Database.ExecuteSqlCommand("update InventarioAlmacen set  Disponibilidad=Existencia where apartado=0 and IDArticulo= " + caracteristicaC.Articulo_IDArticulo + " and IDCaracteristica=" + caracteristicaC.ID + " and IDAlmacen=" + idalmacenPedidoDet + "");
                                    db.Database.ExecuteSqlCommand("update InventarioAlmacen set  Disponibilidad=(Disponibilidad-Apartado) where Apartado>0 and   IDArticulo= " + caracteristicaC.Articulo_IDArticulo + " and IDCaracteristica=" + caracteristicaC.ID + " and IDAlmacen=" + idalmacenPedidoDet + "");
                                    db.Database.ExecuteSqlCommand("update InventarioAlmacen set  Disponibilidad=Existencia where  IDArticulo= " + caracteristicaC.Articulo_IDArticulo + " and IDCaracteristica=" + caracteristicaC.ID + " and IDAlmacen=" + i.IDAlmacen + "");
                                    try
                                    {
                                        InventarioAlmacen inv = new InventarioAlmacenContext().InventarioAlmacenes.ToList().Where(s => s.IDAlmacen == idalmacenPedidoDet && s.IDCaracteristica == kitc.IDCaracteristica).FirstOrDefault();

                                        string cadenam = "INSERT INTO [dbo].[MovimientoArticulo] ([Fecha],[Id],[IDPresentacion],[Articulo_IDArticulo],[Accion],[cantidad],[Documento],[NoDocumento],[Lote],[IDAlmacen],[TipoOperacion],[acumulado],[observacion],[Hora]) VALUES ";
                                        cadenam += " (getdate(), " + caracteristicaC.ID + "," + caracteristicaC.IDPresentacion + "," + caracteristicaC.Articulo_IDArticulo + ",'Remisión'," + cantidadreal + ",'Remisión '," + idremision + ",''," + idalmacenPedidoDet + ",'S'," + inv.Existencia + ",'',CONVERT (time, SYSDATETIMEOFFSET()))";
                                        db.Database.ExecuteSqlCommand(cadenam);
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
                    }
                }




            }
        }
        public void PdfNota(int id)
        {
            //RemisionContext dbr = new RemisionContext();
            //List<EncRemision> Remision = dbr.Database.SqlQuery<EncRemision>("select IDRemision, Fecha, Observacion, UserID, DocumentoFactura, Subtotal+IVA as Subtotal, IVA, Total, IDCliente, IDMetodoPago, IDFormaPago, IDCondicionesPago, IDAlmacen, IDUsoCFDI, TipoCambio, Status, IDVendedor, Entrega from[dbo].[EncRemision] where [IDRemision]= " + id).ToList();

            EncRemision Remision = new RemisionContext().EncRemisiones.Find(id);
            DocumentoNota x = new DocumentoNota();

            x.claveMoneda = Remision.c_Moneda.Descripcion;
            x.descuento = 0;
            x.fecha = Remision.Fecha.ToShortDateString();
            x.fechaRequerida = Remision.Fecha.ToShortDateString();
            x.Cliente = Remision.Clientes.Nombre;
            x.formaPago = Remision.c_FormaPago.ClaveFormaPago;
            x.metodoPago = Remision.c_MetodoPago.ClaveMetodoPago;
            x.RFCCliente = Remision.Clientes.RFC;
            x.TelefonoCliente = Remision.Clientes.Telefono;
            x.total = float.Parse(Remision.Total.ToString());
            x.subtotal = float.Parse(Remision.Subtotal.ToString());
            x.tipo_cambio = Remision.TipoCambio.ToString();
            x.serie = "";
            x.folio = Remision.IDRemision.ToString();
            x.UsodelCFDI = Remision.c_UsoCFDI.Descripcion;
            x.Almacen = Remision.Almacen.Descripcion;
            x.IDRemision = Remision.IDRemision;
            x.Empresa = Remision.Almacen.Telefono;
            x.condicionesdepago = Remision.CondicionesPago.Descripcion;


            ImpuestoNota iva = new ImpuestoNota();
            iva.impuesto = "IVA";
            iva.tasa = float.Parse(SIAAPI.Properties.Settings.Default.ValorIVA.ToString());
            iva.importe = float.Parse(Remision.IVA.ToString());
            x.subtotal = x.subtotal + iva.importe;
            iva.importe = 0;

            x.impuestos.Add(iva);

            EmpresaContext dbe = new EmpresaContext();

            var empresa = dbe.empresas.Single(m => m.IDEmpresa == 2);
            x.Empresa = empresa.RazonSocial;
            x.Telefono = empresa.Telefono;
            x.RFC = empresa.RFC;
            x.Direccion = empresa.Calle + " " + empresa.NoExt + " " + empresa.NoInt + "\n" + empresa.Colonia + " \n" + empresa.Municipio + "," + empresa.Estados.Estado;
            x.firmadefinanzas = empresa.Director_finanzas;
            x.firmadecompras = empresa.Persona_de_Compras + "";

            List<DetRemision> detalles = db.Database.SqlQuery<DetRemision>("select IDDetRemision, IDRemision, IDExterna, IDArticulo, Cantidad, CASE WHEN Cantidad = 0 THEN 0  Else (ImporteTotal/Cantidad) END   Costo, CantidadPedida, Descuento, (Importe+ ImporteIva) as Importe, IVA, ImporteTotal, Nota, Ordenado, Caracteristica_ID, IDAlmacen, Suministro, Status, IDDetExterna, Presentacion, jsonPresentacion, Lote, Devolucion from [dbo].[DetRemision] where [IDRemision]= " + id).ToList();

            int contador = 1;
            foreach (var item in detalles)
            {
                ProductoNota producto = new ProductoNota();
                Articulo arti = new ArticuloContext().Articulo.Find(item.IDArticulo);
                Almacen alma = new AlmacenContext().Almacenes.Find(item.IDAlmacen);
                Caracteristica carateristica = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from caracteristica where id=" + item.Caracteristica_ID).ToList().FirstOrDefault();
                producto.IDPresentacion = carateristica.IDPresentacion;
                producto.ClaveProducto = arti.Cref;
                producto.idarticulo = arti.IDArticulo;
                producto.c_unidad = arti.c_ClaveUnidad.Nombre;
                producto.cantidad = item.Cantidad.ToString();
                producto.almacen = alma.CodAlm;
                producto.descripcion = arti.Descripcion;
                producto.valorUnitario = float.Parse(item.Costo.ToString());
                producto.v_unitario = float.Parse(item.Costo.ToString());
                producto.importe = float.Parse(item.Importe.ToString());
                ///
                if (item.Lote != "")
                {
                    producto.Observacion = "Lote: " + item.Lote;
                }
                producto.Presentacion = item.Presentacion; //item.presentacion;
                ///
                producto.numIdentificacion = contador.ToString();
                contador++;

                x.productos.Add(producto);

            }



            System.Drawing.Image logoempresa = byteArrayToImage(empresa.Logo);

            try
            {


                CreaNotaPDF documento = new CreaNotaPDF(logoempresa, x);

            }
            catch (Exception err)
            {
                String mensaje = err.Message;
            }
            RedirectToAction("Index");
        }

        public ActionResult Notas()
        {
            EFecha elemento = new EFecha();

            return View(elemento);
        }

        [HttpPost]
        public ActionResult Notas(EFecha modelo, FormCollection coleccion)
        {
            VRemisionClieContext dbr = new VRemisionClieContext();
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

                cadena = "select * from VNotaClie where FechaRemision >= '" + FI + "' and FechaRemision <='" + FF + "' ";
                var datos = dbr.Database.SqlQuery<VRemisionClie>(cadena).ToList();
                ViewBag.datos = datos;
                ExcelPackage Ep = new ExcelPackage();
                //Crear la hoja y poner el nombre de la pestaña del libro
                var Sheet = Ep.Workbook.Worksheets.Add("Notas");

                // en la fila1 formateo las celdas y coloco el título de la hoja
                // RichText permite agregar las propiedades de tipo de letra: color, negrita, etc.
                int row = 1;
                //Fijar la fuente para A1:Q1
                Sheet.Cells["A1:P1"].Style.Font.Size = 20;
                Sheet.Cells["A1:P1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:P3"].Style.Font.Bold = true;
                Sheet.Cells["A1:P1"].Style.Font.Color.SetColor(Color.DarkBlue);
                Sheet.Cells["A1:P1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                Sheet.Cells["A1"].RichText.Add("Notas sin Facturación");

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
                Sheet.Cells["A3"].RichText.Add("ID Remisión");
                Sheet.Cells["B3"].RichText.Add("Fecha");
                //Sheet.Cells["C3"].RichText.Add("IDCliente");
                Sheet.Cells["C3"].RichText.Add("No. Expediente");
                Sheet.Cells["D3"].RichText.Add("Cliente");
                Sheet.Cells["E3"].RichText.Add("Subtotal");
                Sheet.Cells["F3"].RichText.Add("Iva");
                Sheet.Cells["G3"].RichText.Add("Total");
                Sheet.Cells["H3"].RichText.Add("Moneda");
                Sheet.Cells["I3"].RichText.Add("Tipo de Cambio");
                Sheet.Cells["J3"].RichText.Add("Total en Pesos");
                Sheet.Cells["K3"].RichText.Add("Estado");
                Sheet.Cells["L3"].RichText.Add("Documento Factura");
                Sheet.Cells["M3"].RichText.Add("Vendedor");
                Sheet.Cells["N3"].RichText.Add("Oficina");
                Sheet.Cells["O3"].RichText.Add("Observación");
                Sheet.Cells["P3"].RichText.Add("Entrega");

                //Aplicar borde doble al rango de celdas A3:Q3
                Sheet.Cells["A3:P3"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;

                // en la fila4, el foreach llenara cada fila de la hoja de excel, con cada fila que trae de la bd.
                // Se establecen los formatos para las celdas: Fecha, Moneda
                row = 4;
                Sheet.Cells.Style.Font.Bold = false;
                foreach (var item in ViewBag.datos)
                {

                    Sheet.Cells[string.Format("A{0}", row)].Value = item.IDRemision;
                    Sheet.Cells[string.Format("B{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    Sheet.Cells[string.Format("B{0}", row)].Value = item.FechaRemision;
                    //Sheet.Cells[string.Format("C{0}", row)].Value = item.IDCliente;
                    Sheet.Cells[string.Format("C{0}", row)].Value = item.noExpediente;
                    Sheet.Cells[string.Format("D{0}", row)].Value = item.Nombre;
                    Sheet.Cells[string.Format("E{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("E{0}", row)].Value = item.Subtotal;
                    Sheet.Cells[string.Format("F{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("F{0}", row)].Value = item.IVA;
                    Sheet.Cells[string.Format("G{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("G{0}", row)].Value = item.Total;
                    Sheet.Cells[string.Format("H{0}", row)].Value = item.ClaveMoneda;
                    Sheet.Cells[string.Format("I{0}", row)].Value = item.TipoCambio;
                    Sheet.Cells[string.Format("J{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("J{0}", row)].Value = item.TotalPesos;
                    Sheet.Cells[string.Format("K{0}", row)].Value = item.EstadoRemision;
                    Sheet.Cells[string.Format("L{0}", row)].Value = item.DocumentoFactura;
                    Sheet.Cells[string.Format("M{0}", row)].Value = item.Vendedor;
                    Sheet.Cells[string.Format("N{0}", row)].Value = item.Oficina;
                    Sheet.Cells[string.Format("O{0}", row)].Value = item.Observacion;
                    Sheet.Cells[string.Format("P{0}", row)].Value = item.Entrega;

                    row++;
                }
                //Se genera el archivo y se descarga

                Sheet.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment: filename=" + "Notas.xlsx");
                Response.BinaryWrite(Ep.GetAsByteArray());
                Response.End();
                return Redirect("/blah");
            }
            return Redirect("index");
        }

        public ActionResult EntreFechasR()
        {
            EFecha elemento = new EFecha();

            return View(elemento);
        }

        [HttpPost]
        public ActionResult EntreFechasR(EFecha modelo, FormCollection coleccion)
        {
            VRemisionClieContext dbr = new VRemisionClieContext();
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

                cadena = "select * from VRemisionClie where FechaRemision >= '" + FI + "' and FechaRemision <='" + FF + "' ";
                var datos = dbr.Database.SqlQuery<VRemisionClie>(cadena).ToList();
                ViewBag.datos = datos;
                ExcelPackage Ep = new ExcelPackage();
                //Crear la hoja y poner el nombre de la pestaña del libro
                var Sheet = Ep.Workbook.Worksheets.Add("Remisiones");

                // en la fila1 formateo las celdas y coloco el título de la hoja
                // RichText permite agregar las propiedades de tipo de letra: color, negrita, etc.
                int row = 1;
                //Fijar la fuente para A1:Q1
                Sheet.Cells["A1:P1"].Style.Font.Size = 20;
                Sheet.Cells["A1:P1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:P3"].Style.Font.Bold = true;
                Sheet.Cells["A1:P1"].Style.Font.Color.SetColor(Color.DarkBlue);
                Sheet.Cells["A1:P1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                Sheet.Cells["A1"].RichText.Add("Listado de Remisiones");

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
                Sheet.Cells["A3"].RichText.Add("ID Remisión");
                Sheet.Cells["B3"].RichText.Add("Fecha");
                //Sheet.Cells["C3"].RichText.Add("IDCliente");
                Sheet.Cells["C3"].RichText.Add("No. Expediente");
                Sheet.Cells["D3"].RichText.Add("Cliente");
                Sheet.Cells["E3"].RichText.Add("Subtotal");
                Sheet.Cells["F3"].RichText.Add("Iva");
                Sheet.Cells["G3"].RichText.Add("Total");
                Sheet.Cells["H3"].RichText.Add("Moneda");
                Sheet.Cells["I3"].RichText.Add("Tipo de Cambio");
                Sheet.Cells["J3"].RichText.Add("Total en Pesos");
                Sheet.Cells["K3"].RichText.Add("Estado");
                Sheet.Cells["L3"].RichText.Add("Documento Factura");
                Sheet.Cells["M3"].RichText.Add("Vendedor");
                Sheet.Cells["N3"].RichText.Add("Oficina");
                Sheet.Cells["O3"].RichText.Add("Observación");
                Sheet.Cells["P3"].RichText.Add("Entrega");

                //Aplicar borde doble al rango de celdas A3:Q3
                Sheet.Cells["A3:P3"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;

                // en la fila4, el foreach llenara cada fila de la hoja de excel, con cada fila que trae de la bd.
                // Se establecen los formatos para las celdas: Fecha, Moneda
                row = 4;
                Sheet.Cells.Style.Font.Bold = false;
                foreach (var item in ViewBag.datos)
                {

                    Sheet.Cells[string.Format("A{0}", row)].Value = item.IDRemision;
                    Sheet.Cells[string.Format("B{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    Sheet.Cells[string.Format("B{0}", row)].Value = item.FechaRemision;
                    //Sheet.Cells[string.Format("C{0}", row)].Value = item.IDCliente;
                    Sheet.Cells[string.Format("C{0}", row)].Value = item.noExpediente;
                    Sheet.Cells[string.Format("D{0}", row)].Value = item.Nombre;
                    Sheet.Cells[string.Format("E{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("E{0}", row)].Value = item.Subtotal;
                    Sheet.Cells[string.Format("F{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("F{0}", row)].Value = item.IVA;
                    Sheet.Cells[string.Format("G{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("G{0}", row)].Value = item.Total;
                    Sheet.Cells[string.Format("H{0}", row)].Value = item.ClaveMoneda;
                    Sheet.Cells[string.Format("I{0}", row)].Value = item.TipoCambio;
                    Sheet.Cells[string.Format("J{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("J{0}", row)].Value = item.TotalPesos;
                    Sheet.Cells[string.Format("K{0}", row)].Value = item.EstadoRemision;
                    Sheet.Cells[string.Format("L{0}", row)].Value = item.DocumentoFactura;
                    Sheet.Cells[string.Format("M{0}", row)].Value = item.Vendedor;
                    Sheet.Cells[string.Format("N{0}", row)].Value = item.Oficina;
                    Sheet.Cells[string.Format("O{0}", row)].Value = item.Observacion;
                    Sheet.Cells[string.Format("P{0}", row)].Value = item.Entrega;

                    row++;
                }
                //Se genera el archivo y se descarga

                Sheet.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment: filename=" + "Remisiones.xlsx");
                Response.BinaryWrite(Ep.GetAsByteArray());
                Response.End();
                return Redirect("/blah");
            }
            return Redirect("index");
        }
        [HttpPost]
        public JsonResult RastrearDocS(int id)
        {

            //var Articulos = new ArticuloContext().Articulo.Where(s => s.Descripcion.Contains(buscar) &&( s.IDTipoArticulo == 7 && s.obsoleto==false));
            //Articulo Articulos = new ArticuloContext().Database.SqlQuery<Articulo>.FirstOrDefault();

            string opciones = "";
            SIAAPI.clasescfdi.ClsRastreaDA rastreas = new SIAAPI.clasescfdi.ClsRastreaDA();
            List<SIAAPI.clasescfdi.NodoTrazo> nodoss = rastreas.getDocumentoSiguiente("Remision", id, "Encabezado");

            foreach (SIAAPI.clasescfdi.NodoTrazo nodosi in nodoss)
            {
                SIAAPI.Models.Comercial.EncPrefactura encPrefactura = new SIAAPI.Models.Comercial.PrefacturaContext().EncPrefactura.Find(nodosi.ID);

                opciones = "Prefactura " + encPrefactura.Serie + " " + encPrefactura.Numero;

            }

            List<SIAAPI.clasescfdi.NodoTrazo> nodosp = rastreas.getDocumentoAnterior("Remision", id, "Encabezado");


            foreach (SIAAPI.clasescfdi.NodoTrazo nodo in nodosp)
            {

                List<SIAAPI.clasescfdi.NodoTrazo> nodosP = rastreas.getDocumentoSiguiente("PedidoP", nodo.ID, "Encabezado");

                foreach (SIAAPI.clasescfdi.NodoTrazo nodofac in nodosP)
                {

                    opciones += "\n" + nodofac.Descripcion;

                }

            }

            return Json(opciones, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeletecarritoR(int id)
        {
            try
            {
                CarritoContext db = new CarritoContext();
                db.Database.ExecuteSqlCommand("delete from CarritoRemision where IDCarritoRemision= " + id);
                return new HttpStatusCodeResult(200, "Ok");
            }
            catch (Exception err)
            {
                return new HttpStatusCodeResult(500, "Bad");
            }
        }
      
    }
}
