
using PagedList;
using SIAAPI.Models.Comercial;
using SIAAPI.Models.Inventarios;
using SIAAPI.Models.Login;
using SIAAPI.Models.Produccion;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using OfficeOpenXml;
using System.Drawing;
using OfficeOpenXml.Style;
using SIAAPI.Reportes;
using static SIAAPI.Models.Comercial.AlmacenRepository;

namespace SIAAPI.Controllers.Comercial
{

    public class AlmacenController : Controller
    {
        private AlmacenContext db = new AlmacenContext();
        // GET: Almacen
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.ASortParm = String.IsNullOrEmpty(sortOrder) ? "Almacen" : "Almacen";
            ViewBag.BSortParm = String.IsNullOrEmpty(sortOrder) ? "Descripcion" : "Descripcion";
            ViewBag.CSortParm = String.IsNullOrEmpty(sortOrder) ? "Responsable" : "Responsable";
            // Not sure here
            if (searchString == null)
            {
                searchString = currentFilter;
            }

            // Pass filtering string to view in order to maintain filtering when paging
            ViewBag.SearchString = searchString;
            //Paginación
            var elementos = from s in db.Almacenes
                            select s;
            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                elementos = elementos.Where(s => s.Descripcion.Contains(searchString));
            }

            //Ordenacion

            switch (sortOrder)
            {
                case "Almacen":
                    elementos = elementos.OrderBy(s => s.CodAlm);
                    break;
                case "Descripcion":
                    elementos = elementos.OrderBy(s => s.Descripcion);
                    break;
                case "Responsable":
                    elementos = elementos.OrderBy(s => s.Responsable);
                    break;
                default:
                    elementos = elementos.OrderBy(s => s.CodAlm);
                    break;
            }

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = db.Almacenes.OrderBy(e => e.IDAlmacen).Count(); // Total number of elements

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
        //public ActionResult Index()
        //{

        //     var lista =  from e in db.Almacenes
        //                orderby e.IDAlmacen
        //                select e;
        //  return View(lista);
        //}

        // GET: Almacen/Details/5
        public ActionResult Details(int id)
        {
            var elemento = db.Almacenes.Single(m => m.IDAlmacen == id);
            if (elemento == null)
            {
                return NotFound();
            }
            string Estado = new EstadosRepository().getNombreEstado(elemento.IDEstado);

            ViewBag.Estado = Estado;

            return View(elemento);
        }
        private ActionResult NotFound()
        {
            throw new NotImplementedException();
        }

        // POST: Almacen/Details/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Details(int id, Almacen collection)
        {
            var elemento = db.Almacenes.Single(m => m.IDAlmacen == id);
            return View(elemento);
        }

        // GET: Almacen/Create
        public ActionResult Create()
        {
            var lista = new EstadosRepository().GetEstados();
            ViewBag.ComboEstados = lista;
            ViewBag.Validacion = "";

            //Validar el numero de almacenes
            int nodealmacenes = new AlmacenContext().Almacenes.Count();
            int limite = new EmpresaContext().empresas.FirstOrDefault().NoAlmacenes;
            if (nodealmacenes >= limite)
                { ViewBag.Validacion = "Has excedido el numero de almacenes permitido, contacta a tu administrador"; }

            return View();
        }

        // POST: Almacen/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Almacen elemento)
        {
            try
            {
                // TODO: Add insert logic here

                var db = new AlmacenContext();
                db.Almacenes.Add(elemento);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                var lista = new EstadosRepository().GetEstados();
                ViewBag.ComboEstados = lista;
                return View();
            }
        }

        // GET: Almacen/Edit/5
        public ActionResult Edit(int id)
        {
            var elemento = db.Almacenes.Single(m => m.IDAlmacen == id);
            var lista = new EstadosRepository().GetEstados();
            ViewBag.ComboEstados = lista;
            return View(elemento);
        }

        // POST: Almacen/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
                var elemento = db.Almacenes.Single(m => m.IDAlmacen == id);
                if (TryUpdateModel(elemento))
                {
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                var lista = new EstadosRepository().GetEstados();
                ViewBag.ComboEstados = lista;
                return View();
            }
            catch (Exception er)
            {
                var lista = new EstadosRepository().GetEstados();
                ViewBag.ComboEstados = lista;
                return View();
            }
        }

        // GET: Almacen/Delete/5
        public ActionResult Delete(int id)
        {
            var elemento = db.Almacenes.Single(m => m.IDAlmacen == id);
            if (elemento == null)
            {
                return HttpNotFound();
            }

            return View(elemento);
        }

        // POST: Almacen/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                var elemento = db.Almacenes.Single(m => m.IDAlmacen == id);
                db.Almacenes.Remove(elemento);
                db.SaveChanges();
                return RedirectToAction("Index");

            }
            catch
            {
                return View();
            }
        }
      
        public ActionResult PedAlmacen(string IDAlmacen = "", string searchString = "")
        {


            string ConsSql = "select * from Vdetsolicitud";
            string FiltroSql = " where status='Solicitado'";
            string OrdenSql = "order by Cref ";
            string CadSql = string.Empty;

            AlmacenContext dba = new Models.Comercial.AlmacenContext();

            ViewBag.IDAlmacen = new SelectList(dba.Almacenes, "IDAlmacen", "Descripcion");


            if (searchString == "")
            {
                searchString = string.Empty;
            }
            if (!String.IsNullOrEmpty(searchString))
            {
                FiltroSql += " and (Cref like '" + searchString + "%' or descripcion like '" + searchString + "%')";
            }

            if (IDAlmacen == "")
            {
                IDAlmacen = string.Empty;
            }
            if (!String.IsNullOrEmpty(IDAlmacen))
            {
                FiltroSql += " and IDAlmacen = " + int.Parse(IDAlmacen.ToString()) + "";
            }

            CadSql = ConsSql + " " + FiltroSql + " " + OrdenSql;


            ViewBag.Mensaje = Session["Mensaje"];
            List<Vdetsolicitud> elementos = db.Database.SqlQuery<Vdetsolicitud>(CadSql).ToList();
            return View(elementos);
        }



        public ActionResult Solicitar(int id, decimal Cantidad, int IDAlmacen, string observacion)
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

                DetSolicitud solicitud = db.Database.SqlQuery<DetSolicitud>("Select * from Detsolicitud where IDDetSolicitud=" + id).ToList().FirstOrDefault();

                Proveedor proveedordesconocido = new ProveedorContext().Proveedores.Find(SIAAPI.Properties.Settings.Default.IDProveedorDesconocido);

                //      cadmen =  cadmen +" provee desc " + proveedordesconocido +;
                EncRequisiciones requisicion = new EncRequisiciones();
                requisicion.IDRequisicion = num;
                requisicion.Fecha = DateTime.Now;
                requisicion.FechaRequiere = DateTime.Now;
                requisicion.IDProveedor = proveedordesconocido.IDProveedor;
                requisicion.IDFormapago = proveedordesconocido.IDFormaPago;
                //proveedordesconocido.IDMoneda;
                requisicion.Observacion = "Para el Documento " + observacion;
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
                    decimal costo = db.Database.SqlQuery<ClsDatoDecimal>("select dbo.[GetCosto](0," + solicitud.IDArticulo + "," + Cantidad + ") as Dato").ToList().FirstOrDefault().Dato;

                    decimal subtotal = Math.Round(costo * Cantidad);
                    decimal iva = Math.Round(subtotal * Decimal.Parse(SIAAPI.Properties.Settings.Default.ValorIVA), 2);
                    decimal total = subtotal + iva;
                    Articulo articulo = new ArticuloContext().Articulo.Find(solicitud.IDArticulo);
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
                    cadena.Append(solicitud.IDArticulo + ",");
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
                    cadena.Append(solicitud.Caracteristica_ID + ",");
                    cadena.Append(IDAlmacen + ",");
                    cadena.Append("0,"); //suministro
                    cadena.Append("'Activo',"); //status
                    cadena.Append("'" + solicitud.Presentacion + "',");
                    cadena.Append("'{" + solicitud.Presentacion + "}')");

                    try
                    {
                        db.Database.ExecuteSqlCommand(cadena.ToString());

                        decimal existencia = 0;
                        InventarioAlmacen inv = new InventarioAlmacenContext().InventarioAlmacenes.ToList().Where(s => s.IDAlmacen == IDAlmacen && s.IDCaracteristica == solicitud.Caracteristica_ID).FirstOrDefault();
                        Caracteristica cara = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from Caracteristica where id=" + solicitud.Caracteristica_ID).FirstOrDefault();
                        if (inv != null)
                        {
                            //- No hace nada por que las requisicion no afectan en nada

                            existencia = inv.Existencia;
                        }
                        else
                        {
                            db.Database.ExecuteSqlCommand("INSERT INTO inventarioAlmacen (IDAlmacen,IDArticulo,IDCaracteristica,Existencia, PorLlegar,Apartado,Disponibilidad) values ("
                                + solicitud.IDAlmacen + "," + cara.Articulo_IDArticulo + "," + solicitud.Caracteristica_ID + ",0,0,0,0))");
                            existencia = 0;
                        }
                         int usuario = userid.Select(s => s.UserID).FirstOrDefault();


                        string moviartcadena = "Insert into MovimientoArticulo (fecha,id, idPresentacion, Articulo_IDArticulo, Accion, Cantidad, Documento, Nodocumento, Lote, IDAlmacen,TipoOperacion, Acumulado, Observacion, Hora, usuario) ";
                        moviartcadena += "values (getdate()," + solicitud.Caracteristica_ID + "," + cara.IDPresentacion + "," + cara.Articulo_IDArticulo + ",'Requisicion para pedido', " + Cantidad + ",'Requisción'," + num + ",''," + IDAlmacen + ",'N/A'," + existencia + ",'" + observacion + "',CONVERT (time, SYSDATETIMEOFFSET()), "+usuario+") ";

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

                db.Database.ExecuteSqlCommand("update detsolicitud set status='Requerido', CantidadPedida=" + Cantidad + ", DocumentoR='Requisicion', NumeroDR=" + num + " where IDDetsolicitud =" + id);

                return RedirectToRoute(new { contoller = "Almacen", action = "PedAlmacen" });
            }
            catch (Exception err)
            {
                string mensaje = err.Message;
                ViewBag.Mensaje = mensaje;
                Session["Mensaje"] = ViewBag.Mensaje;
                return RedirectToRoute(new { contoller = "Almacen", action = "PedAlmacen" });


            }
        }

        public ActionResult NoSolicitar(int id)
        {
            try
            {
                db.Database.ExecuteSqlCommand("update detsolicitud set status='No solicitado' where IDDetsolicitud =" + id);
                return RedirectToRoute(new { contoller = "Almacen", action = "PedAlmacen" });
            }
            catch (Exception err)
            {
                string mensaje = err.Message;
                return RedirectToRoute(new { contoller = "Almacen", action = "PedAlmacen" });
            }
        }


        public ActionResult AsignarMateriales()
        {

            return View();
        }

        public ActionResult AsignaraOrden(int IDOrden, string Mensaje="")
        {
            ViewBag.Mensaje = Mensaje;
            var listado = new WorkinProcessContext().WIP.Where(s => s.Orden == IDOrden).ToList();

            ViewBag.listado = listado;

            ViewBag.Mensajederror = "";

            OrdenProduccion ordenp = new OrdenProduccionContext().OrdenesProduccion.Find(IDOrden);
            if (ordenp == null)
            {
                return RedirectToAction("AsignarMateriales");
            }
            List<ArticuloProduccion> materiasprimas = new ArticulosProduccionContext().Database.SqlQuery<ArticuloProduccion>("select distinct(ap.idorden), ap.* from ArticuloProduccion as ap where ap.idorden=" + IDOrden + " and idtipoarticulo=6 ").ToList();

            //List<ArticuloProduccion> materiasprimas = new ArticulosProduccionContext().ArticulosProducciones.Where(s => s.IDOrden == IDOrden && (s.IDTipoArticulo == 6) ).ToList();
            ViewBag.ordenproduccion = ordenp;
            return View(materiasprimas);
        }




        //[HttpPost]
        //public ActionResult descontarlote(int orden, string loteinterno, int IDProceso, decimal valorinputentregar, int valorcheck, FormCollection coleccion)
        //{

        //    decimal largo = 0;
        //    decimal m2 = 0;

        //    string mensaje = "";
        //    string ancho1 = string.Empty;
        //    string largo1 = string.Empty;
        //    Clslotemp lotemp = new Clslotemp();
        //    try
        //    {
        //        AlmacenContext db = new AlmacenContext();
        //        lotemp = db.Database.SqlQuery<Clslotemp>("Select * from clslotemp where loteinterno='" + loteinterno + "'").ToList().FirstOrDefault();
        //        if (lotemp == null)
        //        {
        //            try
        //            {
        //                if (lotemp == null)
        //                {
        //                    string[] cadenasmp = loteinterno.Split('/');
        //                    int anchomp = int.Parse(cadenasmp[1].ToString());
        //                    int largomp = int.Parse(cadenasmp[2].ToString());
        //                    int ordenCompra = int.Parse(cadenasmp[3].ToString());
        //                    int numCinta = int.Parse(cadenasmp[5].ToString());
        //                    OrdenProduccion ordenp = new OrdenProduccionContext().OrdenesProduccion.Find(orden);
        //                    MaterialAsignado material = new MaterialAsignado();
        //                    material = db.Database.SqlQuery<MaterialAsignado>("select*from MaterialAsignado where orden=" + ordenp.IDOrden).ToList().FirstOrDefault();
        //                    Caracteristica caracteristicas = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from Caracteristica where id=" + material.idcaracteristica).ToList().FirstOrDefault();

        //                    decimal metroscuadradosMP = largomp * (Convert.ToDecimal(anchomp) / 1000);
        //                    string cadenaMP = "INSERT INTO [dbo].[Clslotemp]([IDArticulo],[IDCaracteristica],[NoCinta],[IDDetOrdenCompra],[Ancho],[Largo],[Lote],[LoteInterno],[Fecha],[OrdenCompra],[MetrosCuadrados],[Metrosutilizados],[MetrosDisponibles],[IDProveedor],[IDAlmacen],[FacturaProv])VALUES(" +
        //                                                                   material.idmapri + "," + material.idcaracteristica + "," + numCinta + ",0," + anchomp + "," + largomp + ",'','" + loteinterno + "'," + "CONVERT (time, SYSDATETIMEOFFSET())," + ordenCompra + "," + metroscuadradosMP + ",0," + metroscuadradosMP + ",2,1,0)";
        //                    db.Database.ExecuteSqlCommand(cadenaMP);


        //                }
        //                lotemp = db.Database.SqlQuery<Clslotemp>("Select * from clslotemp where loteinterno='" + loteinterno + "'").ToList().FirstOrDefault();
        //                try
        //                {


        //                    if (lotemp.MetrosDisponibles <= 0)
        //                    {
        //                        mensaje = "No existen metros disponibles";
        //                        return new HttpStatusCodeResult(200, "No existen metros disponibles"); //200 es ok para html
        //                    }
        //                    string[] cadenas = loteinterno.Split('/');
        //                    largo = int.Parse(cadenas[2].ToString());
        //                    Caracteristica caracteristicas = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from Caracteristica where id=" + lotemp.IDCaracteristica).ToList().FirstOrDefault();
        //                    FormulaSiaapi.Formulas formu = new FormulaSiaapi.Formulas();

        //                    ancho1 = formu.getValorCadena("ANCHO", caracteristicas.Presentacion);

        //                    largo1 = formu.getValorCadena("LARGO", caracteristicas.Presentacion);

        //                    if (valorcheck == 1)
        //                    {
        //                        decimal metroscuadrados = valorinputentregar * (Convert.ToDecimal(ancho1) / 1000);
        //                        largo = valorinputentregar;
        //                        m2 = Math.Round(metroscuadrados, 3);
        //                    }
        //                    else
        //                    {

        //                        m2 = Math.Round(lotemp.MetrosDisponibles, 3);
        //                        largo = lotemp.MetrosDisponibles / (decimal.Parse(ancho1) * 0.001M);
        //                    }
        //                }
        //                catch (Exception noestaba)
        //                {
        //                    try
        //                    {
        //                        string[] cadenas = loteinterno.Split('/');
        //                        lotemp.IDAlmacen = 6;//almacen de materia primas


        //                    }
        //                    catch (Exception err)
        //                    {
        //                        mensaje = "No parece una cinta";
        //                    }
        //                }

        //                StringBuilder cadena = new StringBuilder();
        //                cadena.Append("insert into workinprocess([IDAlmacen],[IDArticulo],[IDCaracteristica],[Cantidad],[Orden],[IDproceso],[Ocupado],[Devuelto],[Merma],[Observacion],[IDlotemp],loteinterno,IDClsMateriaPrima, ocupadolineal) ");
        //                cadena.Append("values (" + lotemp.IDAlmacen + "," + lotemp.IDArticulo + "," + lotemp.IDCaracteristica + "," + m2);
        //                cadena.Append("," + orden + "," + IDProceso + "," + m2 + ",0,0,''," + lotemp.ID + ",'" + loteinterno + "'," + lotemp.ID + ", " + largo + ")");
        //                db.Database.ExecuteSqlCommand(cadena.ToString());
        //                db.Database.ExecuteSqlCommand("update MaterialAsignado set lote='" + loteinterno + "', entregado=(entregado+" + largo + ") where orden=" + orden + " and ancho=" + ancho1 + " and largo=" + largo1 + " and idmapri=" + lotemp.IDArticulo);
        //                db.Database.ExecuteSqlCommand("update clslotemp set metrosutilizados=(metrosutilizados+" + m2 + "),metrosdisponibles=(metrosdisponibles-" + m2 + ") where id=" + lotemp.ID);
        //                //db.Database.ExecuteSqlCommand("update inventarioalmacen set apartado=(apartado-" + m2 + "),existencia=(existencia-" + m2 + ") where idarticulo= " + lotemp.IDArticulo + " and idalmacen=" + lotemp.IDAlmacen + " and idcaracteristica=" + lotemp.IDCaracteristica);
        //                Caracteristica carateristica = new WorkinProcessContext().Database.SqlQuery<Caracteristica>("select * from caracteristica where id=" + lotemp.IDCaracteristica).ToList().FirstOrDefault();
        //                InventarioAlmacen inv = new InventarioAlmacenContext().InventarioAlmacenes.ToList().Where(s => s.IDAlmacen == lotemp.IDAlmacen && s.IDCaracteristica == lotemp.IDCaracteristica).FirstOrDefault();
        //                decimal existencia = 0;

        //                if (inv != null)
        //                {
        //                    existencia = inv.Existencia;
        //                }
        //                try
        //                {
        //                    string cadenam = "INSERT INTO [dbo].[MovimientoArticulo] ([Fecha],[Id],[IDPresentacion],[Articulo_IDArticulo],[Accion],[cantidad],[Documento],[NoDocumento],[Lote],[IDAlmacen],[TipoOperacion],[acumulado],[observacion],[Hora]) VALUES ";
        //                    cadenam += " (getdate(), " + carateristica.ID + "," + carateristica.IDPresentacion + "," + carateristica.Articulo_IDArticulo + ",'Entrega a Produccion'," + m2 + ",'Orden Produccion '," + orden + ",'" + loteinterno + "'," + lotemp.IDAlmacen + ",'S'," + existencia + ",'Cinta no registrada en el inventario',CONVERT (time, SYSDATETIMEOFFSET()))";
        //                    db.Database.ExecuteSqlCommand(cadenam);
        //                }
        //                catch (Exception err)
        //                {
        //                    string mensajee = err.Message;
        //                }


        //                SIAAPI.Models.Inventarios.AjustesAlmacenContext dba = new SIAAPI.Models.Inventarios.AjustesAlmacenContext();
        //                int numerodeorden = orden;
        //                SIAAPI.Models.Comercial.ClsDatoDecimal que = dba.Database.SqlQuery<SIAAPI.Models.Comercial.ClsDatoDecimal>("SELECT sum(ocupado) as Dato FROM [dbo].[WorkinProcess] where orden = " + orden + " group by idarticulo, IDCaracteristica, orden, idproceso").ToList().FirstOrDefault();

        //                decimal ocupado;
        //                try
        //                { ocupado = que.Dato; }
        //                catch (Exception err)
        //                {
        //                    string mensajederror = err.Message;
        //                    ocupado = 0;
        //                }
        //                if (ocupado > 0)
        //                {
        //                    try
        //                    {
        //                        db.Database.ExecuteSqlCommand("update Articuloproduccion set existe='1' where IDorden=" + orden + " and IDTipoArticulo =6 ");
        //                    }
        //                    catch (Exception err)
        //                    {
        //                        string mensajederror = err.Message;
        //                        ocupado = 0;
        //                    }

        //                }




        //            }
        //            catch (Exception noestaba)
        //            {
        //                mensaje = noestaba.Message;
        //            }

        //        }
        //        else
        //        {
        //            try
        //            {


        //                if (lotemp.MetrosDisponibles <= 0)
        //                {
        //                    mensaje = "No existen metros disponibles";
        //                    return new HttpStatusCodeResult(200, "No existen metros disponibles"); //200 es ok para html
        //                }
        //                string[] cadenas = loteinterno.Split('/');
        //                largo = int.Parse(cadenas[2].ToString());
        //                Caracteristica caracteristicas = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from Caracteristica where id=" + lotemp.IDCaracteristica).ToList().FirstOrDefault();
        //                FormulaSiaapi.Formulas formu = new FormulaSiaapi.Formulas();

        //                ancho1 = formu.getValorCadena("ANCHO", caracteristicas.Presentacion);

        //                largo1 = formu.getValorCadena("LARGO", caracteristicas.Presentacion);

        //                if (valorcheck == 1)
        //                {
        //                    decimal metroscuadrados = valorinputentregar * (Convert.ToDecimal(ancho1) / 1000);
        //                    largo = valorinputentregar;
        //                    m2 = Math.Round(metroscuadrados, 3);
        //                }
        //                else
        //                {

        //                    m2 = Math.Round(lotemp.MetrosDisponibles, 3);
        //                    largo = lotemp.MetrosDisponibles / (decimal.Parse(ancho1) * 0.001M);
        //                }
        //            }
        //            catch (Exception noestaba)
        //            {
        //                try
        //                {
        //                    string[] cadenas = loteinterno.Split('/');
        //                    lotemp.IDAlmacen = 6;//almacen de materia primas


        //                }
        //                catch (Exception err)
        //                {
        //                    mensaje = "No parece una cinta";
        //                }
        //            }
        //            StringBuilder cadena = new StringBuilder();
        //            cadena.Append("insert into workinprocess([IDAlmacen],[IDArticulo],[IDCaracteristica],[Cantidad],[Orden],[IDproceso],[Ocupado],[Devuelto],[Merma],[Observacion],[IDlotemp],loteinterno,IDClsMateriaPrima, ocupadolineal) ");
        //            cadena.Append("values (" + lotemp.IDAlmacen + "," + lotemp.IDArticulo + "," + lotemp.IDCaracteristica + "," + m2);
        //            cadena.Append("," + orden + "," + IDProceso + "," + m2 + ",0,0,''," + lotemp.ID + ",'" + loteinterno + "'," + lotemp.ID + ", " + largo + ")");
        //            db.Database.ExecuteSqlCommand(cadena.ToString());


        //            db.Database.ExecuteSqlCommand("update MaterialAsignado set idcaracteristica=" + lotemp.IDCaracteristica + ", lote='" + loteinterno + "', entregado=(entregado+" + largo + ") where orden=" + orden + " and ancho=" + ancho1 + " and largo=" + largo1 + " and idmapri=" + lotemp.IDArticulo);
        //            db.Database.ExecuteSqlCommand("update clslotemp set metrosutilizados=(metrosutilizados+" + m2 + "),metrosdisponibles=(metrosdisponibles-" + m2 + ") where id=" + lotemp.ID);
        //            db.Database.ExecuteSqlCommand("update inventarioalmacen set apartado=(apartado-" + m2 + "),existencia=(existencia-" + m2 + ") where idarticulo= " + lotemp.IDArticulo + " and idalmacen=" + lotemp.IDAlmacen + " and idcaracteristica=" + lotemp.IDCaracteristica);
        //            Caracteristica carateristica = new WorkinProcessContext().Database.SqlQuery<Caracteristica>("select * from caracteristica where id=" + lotemp.IDCaracteristica).ToList().FirstOrDefault();
        //            InventarioAlmacen inv = new InventarioAlmacenContext().InventarioAlmacenes.ToList().Where(s => s.IDAlmacen == lotemp.IDAlmacen && s.IDCaracteristica == lotemp.IDCaracteristica).FirstOrDefault();

        //            try
        //            {
        //                string cadenam = "INSERT INTO [dbo].[MovimientoArticulo] ([Fecha],[Id],[IDPresentacion],[Articulo_IDArticulo],[Accion],[cantidad],[Documento],[NoDocumento],[Lote],[IDAlmacen],[TipoOperacion],[acumulado],[observacion],[Hora]) VALUES ";
        //                cadenam += " (getdate(), " + carateristica.ID + "," + carateristica.IDPresentacion + "," + carateristica.Articulo_IDArticulo + ",'Entrega a Produccion'," + m2 + ",'Orden Produccion '," + orden + ",'" + loteinterno + "'," + lotemp.IDAlmacen + ",'S'," + (inv.Existencia - m2) + ",'Cinta entregada',CONVERT (time, SYSDATETIMEOFFSET()))";
        //                db.Database.ExecuteSqlCommand(cadenam);
        //            }
        //            catch (Exception err)
        //            {
        //                string mensajee = err.Message;
        //            }


        //            SIAAPI.Models.Inventarios.AjustesAlmacenContext dba = new SIAAPI.Models.Inventarios.AjustesAlmacenContext();
        //            int numerodeorden = orden;
        //            SIAAPI.Models.Comercial.ClsDatoDecimal que = dba.Database.SqlQuery<SIAAPI.Models.Comercial.ClsDatoDecimal>("SELECT sum(ocupado) as Dato FROM [dbo].[WorkinProcess] where orden = " + orden + " group by idarticulo, IDCaracteristica, orden, idproceso").ToList().FirstOrDefault();

        //            decimal ocupado;
        //            try
        //            { ocupado = que.Dato; }
        //            catch (Exception err)
        //            {
        //                string mensajederror = err.Message;
        //                ocupado = 0;
        //            }
        //            if (ocupado > 0)
        //            {
        //                try
        //                {
        //                    db.Database.ExecuteSqlCommand("update Articuloproduccion set existe='1' where IDorden=" + orden + " and IDTipoArticulo =6 ");
        //                }
        //                catch (Exception err)
        //                {
        //                    string mensajederror = err.Message;
        //                    ocupado = 0;
        //                }

        //            }
        //        }





        //        return new HttpStatusCodeResult(200, "Bien"); //200 es ok para html
        //    }
        //    catch (Exception err)
        //    {
        //        return new HttpStatusCodeResult(500, "Bien"); //algo salio mal
        //    }



        //}
        [HttpPost]

        public ActionResult descontarlote(int orden, string loteinterno, int IDProceso, decimal valorinputentregar, int valorcheck, FormCollection coleccion)
        {

            List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            int UserID = userid.Select(s => s.UserID).FirstOrDefault();
            bool MismoMaterial = false;
            decimal largo = 0;
            decimal m2 = 0;
            string CLAVEARTICULO;
            string[] cadenalote = loteinterno.Split('/');
            CLAVEARTICULO = cadenalote[0];
            Articulo articuloClave = new Articulo();
            articuloClave = db.Database.SqlQuery<Articulo>("Select * from Articulo where cref='" + CLAVEARTICULO + "'").ToList().FirstOrDefault();
            if (articuloClave == null)
            {
                return Content(">Clave de artículo no registrada");
            }
            string mensaje = "";
            string ancho1 = string.Empty;
            string largo1 = string.Empty;
            Clslotemp lotemp = new Clslotemp();
            List<MaterialAsignado> materialAsignado = new ArticulosProduccionContext().Database.SqlQuery<MaterialAsignado>("select * from Materialasignado where orden= " + orden).ToList();
            foreach (var item in materialAsignado)
            {
                if (articuloClave.IDArticulo == item.idmapri)
                {
                    MismoMaterial = true;
                }
            }
            if (MismoMaterial == true)
            {
                try
                {
                    AlmacenContext db = new AlmacenContext();
                    lotemp = db.Database.SqlQuery<Clslotemp>("Select * from clslotemp where loteinterno='" + loteinterno + "'").ToList().FirstOrDefault();
                    if (lotemp == null)
                    {
                        return Content("Lote no registrado");


                    }
                    else
                    {
                        try
                        {
                            int IDAlmacenLote = 0;
                            int IDAlmacenOrden = 0;
                            try
                            {
                                
                                //SIAAPI.Models.Produccion.OrdenProduccion produccion = new SIAAPI.Models.Produccion.OrdenProduccionContext().OrdenesProduccion.Find(orden);

                                //SIAAPI.Models.Comercial.EncPedido detPedido = new SIAAPI.Models.Comercial.PedidoContext().EncPedidos.Find(produccion.IDPedido);

                                //SIAAPI.Models.Comercial.Clientes cliente = new SIAAPI.Models.Comercial.ClientesContext().Clientes.Find(detPedido.IDCliente);

                                //SIAAPI.Models.Comercial.Oficina oficina = new SIAAPI.Models.Comercial.OficinaContext().Oficinas.Find(cliente.IDOficina);

                                //try
                                //{
                                //    IDAlmacenLote = lotemp.IDAlmacen;
                                //    if (oficina.IDOficina == 1)
                                //    {
                                //        IDAlmacenOrden = 6;
                                //    }
                                //    else
                                //    {
                                //        IDAlmacenOrden = 11;
                                //    }
                                //}
                                //catch (Exception err)
                                //{

                                //}
                                //if (IDAlmacenLote!= IDAlmacenOrden)
                                //{
                                //    //return Content("No se puede agregar un lote de otro almacén distinto al de la OP");
                                //    return RedirectToAction("AsignaraOrden", new { IDOrden=orden, Mensaje = "No se puede agregar un lote de otro almacén distinto al de la OP" });
                                //}

                            }
                            catch (Exception err)
                            {
                                
                            }
                            string[] cadenasmp = loteinterno.Split('/');
                            int anchomp = int.Parse(cadenasmp[1].ToString());
                            int largomp = int.Parse(cadenasmp[2].ToString());


                            if (valorcheck == 1)
                            {

                                decimal metroslineales = lotemp.MetrosDisponibles / (Convert.ToDecimal(anchomp) / 1000);


                                if (valorinputentregar > metroslineales)
                                {
                                    mensaje = "No existen metros disponibles";
                                    ViewBag.mensaje = mensaje;

                                    return Json(new { errorMessage = "No existen metros disponibles" });
                                    //return new HttpStatusCodeResult(200, "No existen metros disponibles"); //200 es ok para html
                                }

                            }
                            if (lotemp.MetrosDisponibles <= 0)
                            {

                                mensaje = "No existen metros disponibles";
                                ViewBag.mensaje = mensaje;

                                return Json(new { errorMessage = "No existen metros disponibles" }); //200 es ok para html
                            }
                            string[] cadenas = loteinterno.Split('/');
                            largo = int.Parse(cadenas[2].ToString());
                            Caracteristica caracteristicas = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from Caracteristica where id=" + lotemp.IDCaracteristica).ToList().FirstOrDefault();
                            FormulaSiaapi.Formulas formu = new FormulaSiaapi.Formulas();

                            ancho1 = formu.getValorCadena("ANCHO", caracteristicas.Presentacion);

                            largo1 = formu.getValorCadena("LARGO", caracteristicas.Presentacion);

                            if (valorcheck == 1)
                            {
                                decimal metroscuadrados = valorinputentregar * (Convert.ToDecimal(ancho1) / 1000);
                                largo = valorinputentregar;
                                m2 = Math.Round(metroscuadrados, 3);
                            }
                            else
                            {

                                m2 = Math.Round(lotemp.MetrosDisponibles, 3);
                                largo = lotemp.MetrosDisponibles / (decimal.Parse(ancho1) * 0.001M);
                            }
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
                        StringBuilder cadena = new StringBuilder();
                        cadena.Append("insert into workinprocess([IDAlmacen],[IDArticulo],[IDCaracteristica],[Cantidad],[Orden],[IDproceso],[Ocupado],[Devuelto],[Merma],[Observacion],[IDlotemp],loteinterno,IDClsMateriaPrima, ocupadolineal) ");
                        cadena.Append("values (" + lotemp.IDAlmacen + "," + lotemp.IDArticulo + "," + lotemp.IDCaracteristica + "," + m2);
                        cadena.Append("," + orden + "," + IDProceso + "," + m2 + ",0,0,''," + lotemp.ID + ",'" + loteinterno + "'," + lotemp.ID + ", " + largo + ")");
                        db.Database.ExecuteSqlCommand(cadena.ToString());

                        string actualiza = "update MaterialAsignado set idcaracteristica=" + lotemp.IDCaracteristica + ", lote='" + loteinterno + "', entregado=(entregado+" + largo + ") where orden=" + orden + " and idcaracteristica=" + lotemp.IDCaracteristica + " and idmapri=" + lotemp.IDArticulo;
                        db.Database.ExecuteSqlCommand(actualiza);

                        db.Database.ExecuteSqlCommand("update clslotemp set metrosutilizados=(metrosutilizados+" + m2 + "),metrosdisponibles=(metrosdisponibles-" + m2 + ") where id=" + lotemp.ID);
                        db.Database.ExecuteSqlCommand("update inventarioalmacen set apartado=(apartado-" + m2 + "),existencia=(existencia-" + m2 + ") where idarticulo= " + lotemp.IDArticulo + " and idalmacen=" + lotemp.IDAlmacen + " and idcaracteristica=" + lotemp.IDCaracteristica);
                        db.Database.ExecuteSqlCommand("update inventarioalmacen set apartado=0 where apartado<0 and  idarticulo= " + lotemp.IDArticulo + " and idalmacen=" + lotemp.IDAlmacen + " and idcaracteristica=" + lotemp.IDCaracteristica);
                        db.Database.ExecuteSqlCommand("update inventarioalmacen set existencia=0 where existencia<0 and  idarticulo= " + lotemp.IDArticulo + " and idalmacen=" + lotemp.IDAlmacen + " and idcaracteristica=" + lotemp.IDCaracteristica);


                        db.Database.ExecuteSqlCommand("update inventarioalmacen set Disponibilidad=(existencia-apartado) where idarticulo= " + lotemp.IDArticulo + " and idalmacen=" + lotemp.IDAlmacen + " and idcaracteristica=" + lotemp.IDCaracteristica);

                        Caracteristica carateristica = new WorkinProcessContext().Database.SqlQuery<Caracteristica>("select * from caracteristica where id=" + lotemp.IDCaracteristica).ToList().FirstOrDefault();
                        InventarioAlmacen inv = new InventarioAlmacenContext().InventarioAlmacenes.ToList().Where(s => s.IDAlmacen == lotemp.IDAlmacen && s.IDCaracteristica == lotemp.IDCaracteristica).FirstOrDefault();

                        try
                        {
                            string cadenam = "INSERT INTO [dbo].[MovimientoArticulo] ([Fecha],[Id],[IDPresentacion],[Articulo_IDArticulo],[Accion],[cantidad],[Documento],[NoDocumento],[Lote],[IDAlmacen],[TipoOperacion],[acumulado],[observacion],[Hora],Usuario) VALUES ";
                            cadenam += " (getdate(), " + carateristica.ID + "," + carateristica.IDPresentacion + "," + carateristica.Articulo_IDArticulo + ",'Entrega a Produccion'," + m2 + ",'Orden Produccion '," + orden + ",'" + loteinterno + "'," + lotemp.IDAlmacen + ",'S'," + inv.Existencia + ",'Cinta entregada',CONVERT (time, SYSDATETIMEOFFSET()),"+ UserID + ")";
                            db.Database.ExecuteSqlCommand(cadenam);
                        }
                        catch (Exception err)
                        {
                            string mensajee = err.Message;
                        }


                        SIAAPI.Models.Inventarios.AjustesAlmacenContext dba = new SIAAPI.Models.Inventarios.AjustesAlmacenContext();
                        int numerodeorden = orden;
                        SIAAPI.Models.Comercial.ClsDatoDecimal que = dba.Database.SqlQuery<SIAAPI.Models.Comercial.ClsDatoDecimal>("SELECT sum(ocupado) as Dato FROM [dbo].[WorkinProcess] where orden = " + orden + " group by idarticulo, IDCaracteristica, orden, idproceso").ToList().FirstOrDefault();

                        decimal ocupado;
                        try
                        { ocupado = que.Dato; }
                        catch (Exception err)
                        {
                            string mensajederror = err.Message;
                            ocupado = 0;
                        }
                        if (ocupado > 0)
                        {
                            try
                            {
                                db.Database.ExecuteSqlCommand("update Articuloproduccion set existe='1' where IDorden=" + orden + " and IDTipoArticulo =6 ");
                            }
                            catch (Exception err)
                            {
                                string mensajederror = err.Message;
                                ocupado = 0;
                            }

                        }
                    }





                    return new HttpStatusCodeResult(200, "Bien"); //200 es ok para html
                }
                catch (Exception err)
                {
                    return new HttpStatusCodeResult(500, "Bien"); //algo salio mal
                }
            }
            else
            {
                return Content("La clave del lote no coincide con el Material Asignado");
            }




        }




        [HttpPost]
        public JsonResult Deleteidwork(int id)
        {
            List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            int usuario = userid.Select(s => s.UserID).FirstOrDefault();

            try
            {
                WorkinProcessContext db = new WorkinProcessContext();
                WorkinProcess registro = db.Database.SqlQuery<WorkinProcess>("SELECT * FROM [dbo].[WorkinProcess] where idworkingprocess = " + id).ToList().FirstOrDefault();
                decimal largo = registro.ocupadolineal;
                string[] cadenas = registro.loteinterno.Split('/');
                string ancho = cadenas[1];
              

                    decimal m2 = largo * (decimal.Parse(ancho) / 1000);
            
 
                string cons = "update MaterialAsignado set lote='', entregado=(entregado-" + largo + ") where orden =" + registro.Orden + " and ancho=" + ancho + " and idmapri=" + registro.IDArticulo;
                db.Database.ExecuteSqlCommand(cons);
                db.Database.ExecuteSqlCommand("update clslotemp set metrosdisponibles=(metrosdisponibles+" + m2 + "), metrosutilizados=(metrosutilizados-" + m2 + ")  where ID =" + registro.IDlotemp);
                db.Database.ExecuteSqlCommand("update MaterialAsignado set entregado=0 where orden=" + registro.Orden + " and entregado<0");


                db.Database.ExecuteSqlCommand("delete from [dbo].[WorkinProcess] where idworkingprocess=" + registro.IDWorkingProcess);
                db.Database.ExecuteSqlCommand("update inventarioalmacen set Existencia=(Existencia+" + m2 + "), apartado=(apartado+" + m2 + ") where idarticulo= " + registro.IDArticulo + " and idalmacen=" + registro.IDAlmacen + " and idcaracteristica=" + registro.IDCaracteristica);
                db.Database.ExecuteSqlCommand("update inventarioalmacen set Disponibilidad=(Existencia-apartado) where idarticulo= " + registro.IDArticulo + " and idalmacen=" + registro.IDAlmacen + " and idcaracteristica=" + registro.IDCaracteristica);

                Caracteristica carateristica = new WorkinProcessContext().Database.SqlQuery<Caracteristica>("select * from caracteristica where id=" + registro.IDCaracteristica).ToList().FirstOrDefault();
                InventarioAlmacen inv = new InventarioAlmacenContext().InventarioAlmacenes.ToList().Where(s => s.IDAlmacen == registro.IDAlmacen && s.IDCaracteristica == registro.IDCaracteristica).FirstOrDefault();



                try
                {
                    string cadena = "INSERT INTO [dbo].[MovimientoArticulo] ([Fecha],[Id],[IDPresentacion],[Articulo_IDArticulo],[Accion],[cantidad],[Documento],[NoDocumento],[Lote],[IDAlmacen],[TipoOperacion],[acumulado],[observacion],[Hora], usuario) VALUES ";
                    cadena += " (getdate(), " + carateristica.ID + "," + carateristica.IDPresentacion + "," + carateristica.Articulo_IDArticulo + ",'Regreso de Produccion'," + m2 + ",'Orden Produccion '," + registro.Orden + ",'" + registro.loteinterno + "'," + registro.IDAlmacen + ",'E'," + inv.Existencia + ",'Cinta devuelta',CONVERT (time, SYSDATETIMEOFFSET()), "+usuario+")";
                    db.Database.ExecuteSqlCommand(cadena);
                }
                catch (Exception err)
                {
                    string mensajee = err.Message;
                }




                return Json(true);
            }
            catch (Exception err)
            {
                return Json(500, err.Message);
            }
        }




        public ActionResult ImprimeEtiqProd(int idorden)
        {
            Empresa empresa = new EmpresaContext().empresas.Find(2);
            List<WorkinProcess> elementos = new RecepcionContext().Database.SqlQuery<WorkinProcess>("select * from Workinprocess where orden=" + idorden).ToList();
            System.Drawing.Image logoempresa = byteArrayToImage(empresa.Logo);


            if (elementos.Count > 0)
            {
                string contentType = System.Net.Mime.MediaTypeNames.Application.Pdf;
                Reportes.GeneradorEtiqprod documento = new Reportes.GeneradorEtiqprod(logoempresa, elementos, idorden);
                return new FilePathResult(documento.nombreDocumento, contentType);
            }
            else
            {
                return Content("No hay lotes");
            }




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
        public ActionResult ListaAlmacen()
        {
            AlmacenContext db = new AlmacenContext();
            var almacenes = db.Almacenes.ToList();
            ReporteListaAlmacen report = new ReporteListaAlmacen();
            report.Almacenes = almacenes;
            byte[] abytes = report.PrepareReport();
            return File(abytes, "application/pdf", "ReporteAlmacen.pdf");
        }

        public void GenerarExcelAlmacen()
        {
            //Listado de datos
            AlmacenEdoContext dba = new AlmacenEdoContext();
            var almacen = dba.Database.SqlQuery<AlmacenEdo>("select A.*, e.Estado, p.Pais from dbo.almacen as a inner join (dbo.Estados as e  inner join dbo.Pais as p on e.IDPais = p.IDPais) on a.IDEstado = e.IDEstado").ToList();
            ViewBag.almacen = almacen;
            ExcelPackage Ep = new ExcelPackage();
            var Sheet = Ep.Workbook.Worksheets.Add("Almacenes");

            int row = 1;
            //Fijar la fuente para A1:Q1
            Sheet.Cells["A1:J1"].Style.Font.Size = 20;
            Sheet.Cells["A1:J1"].Style.Font.Name = "Calibri";
            Sheet.Cells["A1:J1"].Style.Font.Bold = true;
            Sheet.Cells["A1:J1"].Style.Font.Color.SetColor(Color.DarkBlue);
            Sheet.Cells["A1:J1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
            Sheet.Cells["A1"].RichText.Add("Listado de Almacenes");
            Sheet.Cells["A1:J1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;

            row = 2;
            Sheet.Cells["A2:J2"].Style.Font.Name = "Calibri";
            Sheet.Cells["A2:J2"].Style.Font.Size = 12;
            Sheet.Cells["A2:J2"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;
            Sheet.Cells["A2:J2"].Style.Font.Bold = true;
            Sheet.Cells["A2:J2"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            Sheet.Cells["A2:J2"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

            Sheet.Cells["A2"].RichText.Add("ID");
            Sheet.Cells["B2"].RichText.Add("Código del Almacen");
            Sheet.Cells["C2"].RichText.Add("Descripción");
            Sheet.Cells["D2"].RichText.Add("Dirección");
            Sheet.Cells["E2"].RichText.Add("Colonia");
            Sheet.Cells["F2"].RichText.Add("Municipio");
            Sheet.Cells["G2"].RichText.Add("Estado");
            Sheet.Cells["H2"].RichText.Add("Pais");
            Sheet.Cells["I2"].RichText.Add("CP");
            Sheet.Cells["J2"].RichText.Add("Teléfono");
            Sheet.Cells["K2"].RichText.Add("Responsable");

            row = 3;
            foreach (var item in ViewBag.almacen)
            {
                Sheet.Cells[string.Format("A{0}", row)].Value = item.IDAlmacen;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.CodAlm;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.Descripcion;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.Direccion;
                Sheet.Cells[string.Format("E{0}", row)].Value = item.Colonia;
                Sheet.Cells[string.Format("F{0}", row)].Value = item.Municipio;
                Sheet.Cells[string.Format("G{0}", row)].Value = item.Estado;
                Sheet.Cells[string.Format("H{0}", row)].Value = item.Pais;
                Sheet.Cells[string.Format("I{0}", row)].Value = item.CP;
                Sheet.Cells[string.Format("J{0}", row)].Value = item.Telefono;
                Sheet.Cells[string.Format("K{0}", row)].Value = item.Responsable;
                row++;
            }
            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "ReporteExcelAlmacenes.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////regresa mp////////////////////////////////////////////////////////////////////////////////////////
        public ActionResult RegresaMP(int? id)
        {
            ViewBag.idOrden = "";
            ViewBag.mensaje = "";
            try
            {
                string cadena = "select count([IDWorkingProcess]) as Dato from [WorkinProcess] where Orden = " + id + "";
                ClsDatoEntero cuenta = db.Database.SqlQuery<ClsDatoEntero>(cadena).ToList()[0];
                int valor = cuenta.Dato;
                if (valor != 0)
                {
                    ViewBag.idOrden = id;
                    //ClsDatoEntero idMP = db.Database.SqlQuery<ClsDatoEntero>("select [IDClsMateriaPrima] as Dato from [WorkinProcess] where Orden=" + id + "").ToList().FirstOrDefault();

                    List<WorkinProcess> materiasprimas = new WorkinProcessContext().WIP.Where(s => s.Orden == id).ToList();

                    return View(materiasprimas);

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


        [HttpPost]
        public ActionResult EditMP(int id, FormCollection coleccion)
        {
            List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            int usuario = userid.Select(s => s.UserID).FirstOrDefault();

            decimal Metro = Convert.ToDecimal(coleccion.Get("Metro"));
            int idorden = Convert.ToInt16(coleccion.Get("idorden"));
            try
            {
                Clslotemp lote = db.Database.SqlQuery<Clslotemp>("select * from  [Clslotemp] where ID=" + id + "").FirstOrDefault();
                decimal m2 = lote.MetrosCuadrados;


                decimal variable = 1000;
                decimal anchom = Convert.ToDecimal(lote.Ancho);

                decimal metrosdevueltosCuadrado = Metro * (anchom / variable);
                decimal metrosdisponibles = Math.Round(metrosdevueltosCuadrado, 2);
                decimal metrosutilizados = m2 - metrosdisponibles;
                metrosutilizados = Math.Round(metrosutilizados, 2);

                db.Database.ExecuteSqlCommand("update [dbo].[Clslotemp] set [Metrosutilizados]=" + metrosutilizados + ", MetrosDisponibles=" + metrosdisponibles + " where id=" + id);
                db.Database.ExecuteSqlCommand("update [dbo].[Workinprocess] set [devuelto]=" + metrosdevueltosCuadrado + ", ocupadolineal=ocupadolineal-" + Metro + " where [IDClsMateriaPrima]=" + id);
                db.Database.ExecuteSqlCommand("update [dbo].[inventarioalmacen] set [Existencia]=(Existencia+" + metrosdevueltosCuadrado + ") where [idalmacen]=" + lote.IDAlmacen + " and idcaracteristica=" + lote.IDCaracteristica);
                //db.Database.ExecuteSqlCommand("update [dbo].[inventarioalmacen] set Apartado=(apartado+"+metrosdevueltosCuadrado+") where [idalmacen]=" + lote.IDAlmacen + " and idcaracteristica=" + lote.IDCaracteristica);

                db.Database.ExecuteSqlCommand("update [dbo].[inventarioalmacen] set Disponibilidad=(Existencia-apartado) where [idalmacen]=" + lote.IDAlmacen + " and idcaracteristica=" + lote.IDCaracteristica);

                try
                {

                    InventarioAlmacen inv = new InventarioAlmacenContext().InventarioAlmacenes.ToList().Where(s => s.IDAlmacen == lote.IDAlmacen && s.IDCaracteristica == lote.IDCaracteristica).FirstOrDefault();
                    Caracteristica carateristica = new WorkinProcessContext().Database.SqlQuery<Caracteristica>("select * from caracteristica where id=" + lote.IDCaracteristica).ToList().FirstOrDefault();

                    string cadenam = "INSERT INTO [dbo].[MovimientoArticulo] ([Fecha],[Id],[IDPresentacion],[Articulo_IDArticulo],[Accion],[cantidad],[Documento],[NoDocumento],[Lote],[IDAlmacen],[TipoOperacion],[acumulado],[observacion],[Hora], usuario) VALUES ";
                    cadenam += " (getdate(), " + carateristica.ID + "," + carateristica.IDPresentacion + "," + lote.IDArticulo + ",'Regreso de Producción'," + metrosdevueltosCuadrado + ",'Orden Producción '," + idorden + ",'" + lote.LoteInterno + "'," + lote.IDAlmacen + ",'E'," + inv.Existencia + ",'Regreso de cinta',CONVERT (time, SYSDATETIMEOFFSET()),"+usuario+")";
                    db.Database.ExecuteSqlCommand(cadenam);
                }
                catch (Exception err)
                {
                    string mensajee = err.Message;
                }







                return RedirectToAction("RegresaMP");

            }
            catch (Exception err)
            {
                return Json(500, err.Message);
            }
        }


        //[HttpPost]
        //public ActionResult EditMP(int id, FormCollection coleccion)
        //{

        //    decimal Metro = Convert.ToDecimal(coleccion.Get("Metro"));
        //    int idorden = Convert.ToInt16(coleccion.Get("idorden"));
        //    try
        //    {
        //        Clslotemp lote = db.Database.SqlQuery<Clslotemp>("select * from  [Clslotemp] where ID=" + id + "").FirstOrDefault();
        //        decimal m2 = lote.MetrosCuadrados;


        //        decimal variable = 1000;
        //        decimal anchom = Convert.ToDecimal(lote.Ancho);

        //        decimal metrosdevueltosCuadrado = Metro * (anchom / variable);
        //        decimal metrosdisponibles = Math.Round(metrosdevueltosCuadrado, 2);
        //        decimal metrosutilizados = m2 - metrosdisponibles;
        //        metrosutilizados = Math.Round(metrosutilizados, 2);

        //        db.Database.ExecuteSqlCommand("update [dbo].[Clslotemp] set [Metrosutilizados]=" + metrosutilizados + ", MetrosDisponibles=" + metrosdisponibles + " where id=" + id);
        //        db.Database.ExecuteSqlCommand("update [dbo].[Workinprocess] set [devuelto]=" + metrosdevueltosCuadrado + ", ocupadolineal=ocupadolineal-" + Metro + " where [IDClsMateriaPrima]=" + id);
        //        db.Database.ExecuteSqlCommand("update [dbo].[inventarioalmacen] set [Existencia]=(Existencia+" + metrosdevueltosCuadrado + ") where [idalmacen]=" + lote.IDAlmacen + " and idcaracteristica=" + lote.IDCaracteristica);
        //        //db.Database.ExecuteSqlCommand("update [dbo].[inventarioalmacen] set Apartado=(apartado+"+metrosdevueltosCuadrado+") where [idalmacen]=" + lote.IDAlmacen + " and idcaracteristica=" + lote.IDCaracteristica);

        //        db.Database.ExecuteSqlCommand("update [dbo].[inventarioalmacen] set Disponibilidad=(Existencia-apartado) where [idalmacen]=" + lote.IDAlmacen + " and idcaracteristica=" + lote.IDCaracteristica);

        //        try
        //        {

        //            InventarioAlmacen inv = new InventarioAlmacenContext().InventarioAlmacenes.ToList().Where(s => s.IDAlmacen == lote.IDAlmacen && s.IDCaracteristica == lote.IDCaracteristica).FirstOrDefault();
        //            Caracteristica carateristica = new WorkinProcessContext().Database.SqlQuery<Caracteristica>("select * from caracteristica where id=" + lote.IDCaracteristica).ToList().FirstOrDefault();

        //            string cadenam = "INSERT INTO [dbo].[MovimientoArticulo] ([Fecha],[Id],[IDPresentacion],[Articulo_IDArticulo],[Accion],[cantidad],[Documento],[NoDocumento],[Lote],[IDAlmacen],[TipoOperacion],[acumulado],[observacion],[Hora]) VALUES ";
        //            cadenam += " (getdate(), " + carateristica.ID + "," + carateristica.IDPresentacion + "," + lote.IDArticulo + ",'Regreso de Producción'," + m2 + ",'Orden Producción '," + idorden + ",'" + lote.LoteInterno + "'," + lote.IDAlmacen + ",'E'," + inv.Existencia + ",'Regreso de cinta',CONVERT (time, SYSDATETIMEOFFSET()))";
        //            db.Database.ExecuteSqlCommand(cadenam);
        //        }
        //        catch (Exception err)
        //        {
        //            string mensajee = err.Message;
        //        }

        //        return RedirectToAction("RegresaMP");

        //    }
        //    catch (Exception err)
        //    {
        //        return Json(500, err.Message);
        //    }
        //}


        public ActionResult ImprimirEtiquetasDevolucion(int IDWorkinprocess)
        {
            Empresa empresa = new EmpresaContext().empresas.Find(2);

            WorkinProcess WP = new WorkinProcessContext().WIP.Find(IDWorkinprocess);

            Clslotemp elemento = new RecepcionContext().Database.SqlQuery<Clslotemp>("select * from clslotemp where ID=" + WP.IDClsMateriaPrima).ToList().FirstOrDefault();
            System.Drawing.Image logoempresa = byteArrayToImage(empresa.Logo);


            string contentType = System.Net.Mime.MediaTypeNames.Application.Pdf;
            Reportes.GeneradorEtiqdevolucion documento = new Reportes.GeneradorEtiqdevolucion(logoempresa, elemento);
            return new FilePathResult(documento.nombreDocumento, contentType);





        }


        public ActionResult ListaOrdenesConflicto(string searchString = "")
        {
            if (searchString == "")
            {
                searchString = string.Empty;
            }
            List<OrdenProduccion> ordenes = null;
            if (searchString == "")
            {
                ordenes = new OrdenProduccionContext().Database.SqlQuery<OrdenProduccion>("select distinct(op.idorden), op.* from OrdenProduccion as op inner join MAterialasignado as ma on op.idorden=ma.orden where ma.entregado=0 and (op.Estadoorden='Conflicto' or op.Estadoorden='Lista') ").ToList();

            }
            else
            {
                ordenes = new OrdenProduccionContext().Database.SqlQuery<OrdenProduccion>("select distinct(op.idorden), op.* from OrdenProduccion as op inner join MAterialasignado as ma on op.idorden=ma.orden where ma.entregado=0 and and (op.Estadoorden='Conflicto' or op.Estadoorden='Lista') and op.idorden=" + searchString).ToList();

            }
            return View(ordenes);
        }

        public ActionResult JsonPeso(string cref, decimal Peso)
        {
            decimal pesobase = 0;
            int MtsLineales = 0;

            string[] calves = cref.Split('-');
            string clavesinacho = calves[0];
            Peso dato = new PesoContext().Database.SqlQuery<Peso>("select * from Peso where IDmatpri='"+ clavesinacho + "'").ToList().FirstOrDefault();
            if ( dato!=null)
            {
                pesobase = dato.PesoxMt;
            }
           
            try {
                string datoancho = calves[1];
                decimal ancho = decimal.Parse(datoancho);
                decimal mts = 1M / (decimal.Parse(ancho.ToString()) * .001M);
                MtsLineales= int.Parse(Math.Round(((Peso * mts) / pesobase), 0).ToString());
            }
            catch(Exception err)
            {
                MtsLineales = 0;
            }
            return Json(MtsLineales, JsonRequestBehavior.AllowGet);
        }


        public ActionResult EntregarTintaaProduccion()
        {
            ViewBag.Error = string.Empty;
            return View();
        }

        [HttpPost]
        public ActionResult EntregarTintaaProduccion(FormCollection collecion)
        {
            ViewBag.Error = string.Empty;
            string lote = collecion.Get("Lote");

            try
            {
                ClslotetintaContext db = new ClslotetintaContext();
                Clslotetinta loteencontrado = db.Tintas.Where(s => s.lote == lote && s.Estado == "Existe").FirstOrDefault();
                if (loteencontrado == null)
                {
                    ViewBag.Error = "Esta tinta pertenece al Wip o su Etiqueta no corresponde al siaapi";
                }
                else
                {
                    //dar de baja cantidad de inventarioalamacen y registar movarticulo 
                    db.Database.ExecuteSqlCommand("update dbo.clslotetinta set Estado='Entregado' where id=" + loteencontrado.id);
                    db.Database.ExecuteSqlCommand("update [dbo].[inventarioalmacen] set Existencia=(Existencia-"+ loteencontrado.cantidad+ ") where [idalmacen]=" + loteencontrado.IDAlmacen + " and idcaracteristica=" + loteencontrado.idcaracteristica);
                    db.Database.ExecuteSqlCommand("update [dbo].[inventarioalmacen] set Disponibilidad=(Existencia-apartado) where [idalmacen]=" + loteencontrado.IDAlmacen + " and idcaracteristica=" + loteencontrado.idcaracteristica);

                    try
                    {
                        List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
                        int usuario = userid.Select(s => s.UserID).FirstOrDefault();

                        InventarioAlmacen inv = new InventarioAlmacenContext().InventarioAlmacenes.ToList().Where(s => s.IDAlmacen == loteencontrado.IDAlmacen && s.IDCaracteristica == loteencontrado.idcaracteristica).FirstOrDefault();
                        Caracteristica carateristica = new WorkinProcessContext().Database.SqlQuery<Caracteristica>("select * from caracteristica where id=" + loteencontrado.idcaracteristica).ToList().FirstOrDefault();

                        string cadenam = "INSERT INTO [dbo].[MovimientoArticulo] ([Fecha],[Id],[IDPresentacion],[Articulo_IDArticulo],[Accion],[cantidad],[Documento],[NoDocumento],[Lote],[IDAlmacen],[TipoOperacion],[acumulado],[observacion],[Hora], usuario) VALUES ";
                        cadenam += " (getdate(), " + carateristica.ID + "," + carateristica.IDPresentacion + "," + loteencontrado.idarticulo + ",'Entrega tinta a producción'," + loteencontrado.cantidad + ",'Orden Producción','0','" + loteencontrado.lote + "'," + loteencontrado.IDAlmacen + ",'S'," + inv.Existencia + ",'Entrega tinta a producción',CONVERT (time, SYSDATETIMEOFFSET()), "+usuario+")";
                        db.Database.ExecuteSqlCommand(cadenam);
                    }
                    catch (Exception err)
                    {
                        string mensajee = err.Message;
                    }
                }
            }
            catch(Exception err)
            {

                ViewBag.Error = "Error inesperado";
            }

            return View();
        }

    }


}
