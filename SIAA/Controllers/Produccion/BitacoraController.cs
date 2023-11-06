﻿using PagedList;
using SIAAPI.Models.Login;
using SIAAPI.Models.Produccion;
using SIAAPI.ViewModels.Comercial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Data;

using System.IO;
using SIAAPI.Models.Comercial;
using SIAAPI.Models.Administracion;
using SIAAPI.ViewModels.Articulo;
using Automatadll;

using System.Xml.Serialization;
using System.Data.SqlClient;
using System.Globalization;
using OfficeOpenXml.Style;
using System.Drawing;
using OfficeOpenXml;
using SIAAPI.ClasesProduccion;
using System.ComponentModel.DataAnnotations;

namespace SIAAPI.Controllers.Produccion
{
    public class BitacoraController : Controller
    {
        private VBitacoraContext db = new VBitacoraContext();
        private BitacoraContext DB = new BitacoraContext();
        // GET: Bitacora

        [Authorize(Roles = "Administrador,Facturacion,Gerencia,GestionCalidad,Sistemas,Comercial,Almacenista,BitacoraProduccion,AdminProduccion,Calidad")]


        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize, int? idorden, int? idproceso, int? orden)
        {
            OrdenProduccionContext ordena = new OrdenProduccionContext();
            ViewBag.idorden = new SelectList(ordena.OrdenesProduccion.Where(a => a.EstadoOrden == "Programada"), "IDOrden", "IDOrden");

            ViewBag.idorden = null;
            ViewBag.idproceso = null;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.vercreate = null;
            if (orden != null)
            {
                ViewBag.vercreate = orden;
            }

            if (searchString == null)
            {
                searchString = currentFilter;
            }

            ViewBag.SearchString = searchString;
            DateTime fechapreestablecida = DateTime.Now.AddDays(-60);
            ////Paginación
            var elementos = from s in db.VBitacoras/*.Where(s => s.EstadoBitacora == "Programada" || s.EstadoBitacora == "Iniciada")*/.OrderByDescending(s => s.IDBitacora)
                            select s;
            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                elementos = elementos.Where(s => s.IDOrden.ToString().Contains(searchString));
            }
            if (idorden != null)
            {
                elementos = elementos.Where(s => s.IDOrden == idorden);
                ViewBag.idorden = idorden;
            }
            if (idproceso != null)
            {
                elementos = elementos.Where(s => s.IDProceso == idproceso);
                ViewBag.idproceso = idproceso;
            }
            if (idorden != null && idproceso != null)
            {
                elementos = elementos.Where(s => s.IDOrden == idorden && s.IDProceso == idproceso);
                ViewBag.idorden = idorden;
                ViewBag.idproceso = idproceso;
            }
            //Ordenacion

            switch (sortOrder)
            {
                case "Bitacora":
                    elementos = elementos.OrderByDescending(s => s.IDBitacora);
                    break;

                default:
                    elementos = elementos.OrderByDescending(s => s.IDBitacora);
                    break;
            }

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = db.VBitacoras.OrderBy(e => e.IDBitacora).Count(); // Total number of elements

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
        public ActionResult getJsonProcesoPorTrabajador(int id)
        {
            var estado = new PRepository().GetProcesoPorTrabajador(id);
            return Json(estado, JsonRequestBehavior.AllowGet); ;

        }
        public JsonResult getJsonMaquinaoPorProceso(int id)
        {
            var estado = new PRepositoryMaquina().GetMaquinaPorProcesor(id);
            //return Json(estado, JsonRequestBehavior.AllowGet); ;
            return Json(new SelectList(estado, "Value", "Text", JsonRequestBehavior.AllowGet));

        }
        public IEnumerable<SelectListItem> getProcesoPorTrabajador(int IDTrabajador)
        {
            var estado = new PRepository().GetProcesoPorTrabajador(IDTrabajador);
            return estado;

        }

        public IEnumerable<SelectListItem> getMaquinaPorProceso(int IDTrabajador)
        {
            var estado = new PRepositoryMaquina().GetMaquinaPorProcesor(IDTrabajador);
            return estado;

        }



        public ActionResult getJsonOrdenPorMaquina(int id)
        {
            var estado = new PRepositoryMaquina().GetOrdenPorMaquina(id);
            return Json(estado, JsonRequestBehavior.AllowGet); ;

        }
        public IEnumerable<SelectListItem> getOrdenPorMaquina(int IDMaquina)
        {
            var estado = new PRepositoryMaquina().GetOrdenPorMaquina(IDMaquina);
            return estado;

        }

        public ActionResult Create()
        {
            OrdenProduccionContext orden = new OrdenProduccionContext();

            List<SelectListItem> opciones = new List<SelectListItem>();
            string cadenasql = "select distinct(o.idorden), o.* from ordenproduccion as o inner join ordenproducciondetalle as od on o.idorden=od.idorden and od.estadoproceso<>'Terminado' and (o.estadoorden='Iniciada' or o.estadoOrden='Programada') order by o.idorden desc ";
            var OrdD = orden.Database.SqlQuery<OrdenProduccion>(cadenasql).ToList();

            foreach (OrdenProduccion art in OrdD)
            {
                SelectListItem elemento = new SelectListItem();
                elemento.Text = art.IDOrden.ToString();
                elemento.Value = art.IDOrden.ToString();
                opciones.Add(elemento);
            }
            ViewBag.idorden = opciones;
            //ViewBag.idorden = new SelectList(orden.OrdenesProduccion.Where(s => s.EstadoOrden == "Programada" || s.EstadoOrden == "Iniciada"), "IDOrden", "IDOrden");

            //ViewBag.idorden = listaorden;
            EstadoOrdenContext estadorden = new EstadoOrdenContext();
            ViewBag.IDEstadoOrden = new SelectList(estadorden.EstadoOrdenes.Where(s => s.Tipo == ("P")), "Descripcion", "Descripcion");

            var datost = db.Trabajadores.OrderBy(i => i.Nombre).ToList();
            List<SelectListItem> liP = new List<SelectListItem>();
            liP.Add(new SelectListItem { Text = "--Selecciona un trabajador--", Value = "0" });
            foreach (var a in datost)
            {
                liP.Add(new SelectListItem { Text = a.IDTrabajador.ToString(), Value = a.IDTrabajador.ToString() });

            }
            ViewBag.ListTra = liP;
            ViewBag.ListProce = getProcesoPorTrabajador(0);
            ViewBag.ListMaquina = getMaquinaPorProceso(0);
            ViewBag.ListOrden = getOrdenPorMaquina(0);



            c_ClaveUnidadContext unidad = new c_ClaveUnidadContext();
            ViewBag.IDClaveUnidad = new SelectList(unidad.c_ClaveUnidades, "IDClaveUnidad", "Nombre");
            string horafecha = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            ViewBag.horafecha = horafecha;
            return View();
        }

        // POST: Almacen/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Bitacora bitacora)
        {
            VMaquinaProceso vMaquinaProceso = new VMaquinaProcesoContext().VMaquinaProceso.Where(s => s.IDArticulo == bitacora.IDMaquina).FirstOrDefault();
            bitacora.IDProceso = vMaquinaProceso.IDProceso;



            int ido = bitacora.IDOrden;
            try
            {

                OrdenProduccionContext dbc = new OrdenProduccionContext();
                OrdenProduccion ord = dbc.OrdenesProduccion.Find(bitacora.IDOrden);
            }
            catch (Exception err)
            {
            }

            int idprocesoanterior = 0;
            try
            {
                OrdenProduccion orden = new OrdenProduccionContext().OrdenesProduccion.Find(ido);
                int noorden = db.Database.SqlQuery<int>("select orden from [VModeloProceso] where IDModeloProduccion=" + orden.IDModeloProduccion + " and IDProceso=" + bitacora.IDProceso + "").FirstOrDefault();
                if (noorden != 1)
                {
                    idprocesoanterior = db.Database.SqlQuery<int>("select IDProceso from [VModeloProceso] where IDModeloProduccion=" + orden.IDModeloProduccion + " and orden=" + noorden + "-1").FirstOrDefault();



                    ClsDatoString estadoProcesoAnt = db.Database.SqlQuery<ClsDatoString>("select EstadoProceso as Dato from OrdenProduccionDetalle where IDOrden=" + ido + " and IDProceso=" + idprocesoanterior + "").ToList().FirstOrDefault();

                    try
                    {

                        if (estadoProcesoAnt.Dato.Equals("Conflicto") || estadoProcesoAnt.Dato.Equals("Programado"))
                        {
                            throw new Exception("No puedes iniciar este proceso porque el anterior no se ha realizado");
                        }
                    }
                    catch
                    {

                    }

                }
                string estadoProceso = "";
                try
                {
                    ClsDatoString estadoProcesoOrden = db.Database.SqlQuery<ClsDatoString>("select Estado as Dato from prioridades where IDOrden=" + ido + " and IDProceso=" + vMaquinaProceso.IDProceso + " and idmaquina='" + vMaquinaProceso.IDArticulo + "'").ToList().FirstOrDefault();
                    estadoProceso = estadoProcesoOrden.Dato;
                }
                catch (Exception err)
                {

                }



                var OrdenDetalle = new OrdenProduccionDetalleContext().OrdenProduccionDetalles.Where(s => s.IDOrden == orden.IDOrden && s.IDProceso == bitacora.IDProceso);
                foreach (var O in OrdenDetalle)
                {
                    if (estadoProceso == "Terminado")
                    {
                        throw new Exception("No puedes iniciar este proceso porque ya fue terminado");
                    }
                }

                BitacoraContext dbbit = new BitacoraContext();

                XmlSerializer serializer = new XmlSerializer(typeof(automata));

                // Declare an object variable of the type to be deserialized.
                automata Automata;

                string path = Path.Combine(Server.MapPath("~/Automatas/Bitacora.Xml"));


                using (TextReader reader = new StreamReader(path))
                {
                    serializer = new XmlSerializer(typeof(automata));
                    Automata = (automata)serializer.Deserialize(reader);
                }





                string estadobitacora = Automata.Estadoactual.Nombre;

                bitacora.EstadoBitacora = estadobitacora;


                bitacora.FechaFin = null;
                bitacora.DiferenciaHoras = string.Empty;
                dbbit.Bitacoras.Add(bitacora);
                dbbit.SaveChanges();


                /// verificar automata proceso
                /// 



                db.Database.ExecuteSqlCommand("update OrdenProduccionDetalle set EstadoProceso='Iniciado' where IDOrden= " + ido + " and IDProceso=" + bitacora.IDProceso + "");




                db.Database.ExecuteSqlCommand("update OrdenProduccion set EstadoOrden='Iniciada' where IDOrden= " + ido + "");





                ClsDatoEntero CuentaProcesoXOrden = db.Database.SqlQuery<ClsDatoEntero>("select count(IDProceso) as Dato from prioridades where IDOrden=" + bitacora.IDOrden + "and IDProceso=" + bitacora.IDProceso).ToList()[0];
                if (CuentaProcesoXOrden.Dato == 0)
                {
                    string cadena = "insert into prioridades (IDOrden,IDProceso,IDMaquina,Prioridad,Estado) values(" + bitacora.IDOrden + "," + bitacora.IDProceso + "," + bitacora.IDMaquina + ",0,'Iniciada')";
                    db.Database.ExecuteSqlCommand(cadena);
                }
                else
                {

                    db.Database.ExecuteSqlCommand("update prioridades set prioridad=0, estado='Iniciada' where  idorden=" + bitacora.IDOrden + " and estado<>'Cancelado' and  idproceso=" + vMaquinaProceso.IDProceso + " and idmaquina=" + vMaquinaProceso.IDArticulo);
                }


                return RedirectToAction("Index", new { idorden = ido, idproceso = bitacora.IDProceso });

            }


            catch (Exception err)
            {
                ViewBag.mensaje = err.Message;
                ViewBag.idorden = ido;
                EstadoOrdenContext estadorden = new EstadoOrdenContext();
                ViewBag.IDEstadoOrden = new SelectList(estadorden.EstadoOrdenes.Where(s => s.Tipo == ("P")), "Descripcion", "Descripcion");
                //TrabajadorContext trabajador = new TrabajadorContext();
                //ViewBag.IDTrabajador = new SelectList(trabajador.Trabajadores.Where(s => s.Nombre.Equals(User.Identity.Name)), "IDTrabajador", "Nombre");
                //ProcesoContext proceso = new ProcesoContext();
                //ViewBag.IDProceso = new SelectList(proceso.Procesos.Where(s => s.IDProceso.Equals(bitacora.IDProceso)), "IDProceso", "NombreProceso");
                var datost = db.Trabajadores.OrderBy(i => i.Nombre).ToList();
                List<SelectListItem> liP = new List<SelectListItem>();
                liP.Add(new SelectListItem { Text = "--Selecciona un trabajador--", Value = "0" });
                foreach (var a in datost)
                {
                    liP.Add(new SelectListItem { Text = a.Nombre, Value = a.IDTrabajador.ToString() });

                }
                ViewBag.ListTra = liP;
                ViewBag.ListProce = getProcesoPorTrabajador(0);
                ViewBag.ListMaquina = getMaquinaPorProceso(0);
                ViewBag.ListOrden = getOrdenPorMaquina(0);



                c_ClaveUnidadContext unidad = new c_ClaveUnidadContext();
                ViewBag.IDClaveUnidad = new SelectList(unidad.c_ClaveUnidades, "IDClaveUnidad", "Nombre");
                string horafecha = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                ViewBag.horafecha = horafecha;

                OrdenProduccionContext orden = new OrdenProduccionContext();
                ViewBag.idorden = new SelectList(orden.OrdenesProduccion.Where(s => s.EstadoOrden == "Programada" || s.EstadoOrden == "Iniciada"), "IDOrden", "IDOrden");
                return View();
            }

        }


        public ActionResult FinalizaB(int? id, int? idproceso, bool terminarproceso = false)
        {
            ViewBag.id = id;
            var elemento = db.VBitacoras.Single(m => m.IDBitacora == id);
            string horafecha = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            ViewBag.horafecha = horafecha;
            c_ClaveUnidadContext claveu = new c_ClaveUnidadContext();

            ViewBag.IDClaveUnidad = null;
            if (idproceso == 16)
            {
                ViewBag.IDClaveUnidad = new SelectList(claveu.c_ClaveUnidades.Where(s => s.IDClaveUnidad == 63), "IDClaveUnidad", "Nombre");
            }
            if (idproceso == 5 || idproceso == 11 || idproceso == 12)
            {
                ViewBag.IDClaveUnidad = new SelectList(claveu.c_ClaveUnidades.Where(s => s.IDClaveUnidad == 55), "IDClaveUnidad", "Nombre");

            }
            if (idproceso == 4)
            {
                ViewBag.IDClaveUnidad = new SelectList(claveu.c_ClaveUnidades.Where(s => s.IDClaveUnidad == 63 || s.IDClaveUnidad == 66), "IDClaveUnidad", "Nombre");

            }
            if (ViewBag.IDClaveUnidad == null)
            {

                ViewBag.IDClaveUnidad = new SelectList(claveu.c_ClaveUnidades, "IDClaveUnidad", "Nombre");


            }

            List<VArticulosProduccion> articulosproduccion = db.Database.SqlQuery<VArticulosProduccion>("select AP.IDClaveUnidad,AP.Existe,AP.IDArtProd,A.Descripcion as Articulo,TA.Descripcion as TipoArticulo,C.Presentacion as Caracteristica,P.NombreProceso as Proceso,AP.IDOrden,AP.Cantidad,CU.Nombre as Unidad,AP.Indicaciones,AP.CostoPlaneado,AP.CostoReal from ArticuloProduccion as AP inner join Articulo as A on A.IDArticulo=AP.IDArticulo inner join TipoArticulo as TA on A.IDTipoArticulo=TA.IDTipoArticulo inner join Caracteristica as C on C.ID=AP.IDCaracteristica inner join Proceso as P on P.IDProceso=AP.IDProceso inner join c_ClaveUnidad as CU on CU.IDClaveUnidad=AP.IDClaveUnidad where AP.IDOrden='" + elemento.IDOrden + "' and AP.IDProceso='" + elemento.IDProceso + "'").ToList();

            ViewBag.Terminarproceso = terminarproceso.ToString();

            return View(articulosproduccion);
        }


        [HttpPost]
        public ActionResult FinalizaB(int? id, FormCollection colecciondecontroles, Bitacora bitacora, List<VArticulosProduccion> cr)
        {
            bitacora.FechaFin = DateTime.Parse(colecciondecontroles.Get("FechaFin"));
            bitacora.Observacion = colecciondecontroles.Get("Observacion");
            bitacora.IDClaveUnidad = int.Parse(colecciondecontroles.Get("IDClaveUnidad"));
            bitacora.Cantidad = decimal.Parse(colecciondecontroles.Get("Cantidad"));
            VBitacora bita = db.VBitacoras.Find(id);
            string fecha = bitacora.FechaFin.Value.ToString("s");

            bool terminarproceso = bool.Parse(colecciondecontroles.Get("Terminarproceso"));

            HttpPostedFileBase archivo = Request.Files["files1"];
            saveIMG(archivo);



            XmlSerializer serializer = new XmlSerializer(typeof(automata));

            // Declare an object variable of the type to be deserialized.
            automata Automata;

            string path = Path.Combine(Server.MapPath("~/Automatas/Bitacora.Xml"));


            using (TextReader reader = new StreamReader(path))
            {
                serializer = new XmlSerializer(typeof(automata));
                Automata = (automata)serializer.Deserialize(reader);
            }
            Bitacora op = new BitacoraContext().Bitacoras.Find(id);

            Automata.ejecutar(op.EstadoBitacora, "T");

            string estadoBitacora = Automata.Estadoactual.Nombre;

            ///TERMINA BITACORA
            db.Database.ExecuteSqlCommand("update [dbo].[Bitacora] set EstadoBitacora='" + estadoBitacora + "', [FechaFin]='" + fecha + "',[Observacion]='" + bitacora.Observacion + "', [Cantidad]='" + bitacora.Cantidad + "', [IDClaveUnidad]='" + bitacora.IDClaveUnidad + "',imagen='" + archivo.FileName + "' where IDBitacora=" + id);




            DatoString dato = db.Database.SqlQuery<DatoString>("SELECT convert(nvarchar(8), DATEADD(SECOND,(SELECT DATEDIFF(SECOND,convert(char(8), (select FechaInicio from Bitacora where IDBitacora='" + id + "'), 108) , convert(char(8), (select FechaFin from Bitacora where IDBitacora='" + id + "'), 108))), CAST('00:00:00' AS TIME))) as Dato").ToList()[0];
            db.Database.ExecuteSqlCommand("update [dbo].[Bitacora] set [DiferenciaHoras]='" + dato.Dato + "' where IDBitacora=" + id);

            ///// SI ES EL ULTIMO PROCESO Y SE TERMINA LA BITACORA SE AGREGA PARA PODER HACER UNA LIBERACION PARCIAL
            OrdenProduccion orden = new OrdenProduccionContext().OrdenesProduccion.Find(op.IDOrden);
            try
            {


                OrdenProduccionDetalle procesoactual = db.Database.SqlQuery<OrdenProduccionDetalle>("select * from OrdenProduccionDetalle where IDOrden=" + op.IDOrden + " and estadoProceso='Terminado' and IDProceso=" + op.IDProceso).FirstOrDefault();

                bool eselutimo = seterminaelultimoproceo(orden.IDOrden, op.IDProceso);
                if (bitacora.Cantidad > 0)
                {
                    if (eselutimo)
                    {
                        db.Database.ExecuteSqlCommand("INSERT INTO [dbo].[OrdenesALiberar]([IDOrden],[IDProceso],Cantidad)VALUES('" + bita.IDOrden + "','" + op.IDProceso + "'," + bitacora.Cantidad + ")");


                    }

                }

            }
            catch (Exception)
            {

            }

            /// // vamos a programar el siguiente proceso
            Bitacora BitacoraOrden = new BitacoraContext().Bitacoras.Find(id);
            VMaquinaProceso maquinaproceso = new VMaquinaProcesoContext().VMaquinaProceso.Where(s => s.IDArticulo == BitacoraOrden.IDMaquina).FirstOrDefault();
            int idprocesodelamaquina = maquinaproceso.IDProceso;


            ModeloProcesoContext mpc = new ModeloProcesoContext();

            int ordenproceso = mpc.Database.SqlQuery<ClsDatoEntero>("select orden as Dato from Modeloproceso where ModelosdeProduccion_IDModeloProduccion= " + orden.IDModeloProduccion + " and Proceso_IDProceso=" + idprocesodelamaquina).ToList().FirstOrDefault().Dato;

          
            int siguienteproceso = ordenproceso + 1;
            int siguientepr = 0;
            try
            {
                siguientepr = mpc.ModeloProceso.Where(s => s.ModelosDeProduccion_IDModeloProduccion == orden.IDModeloProduccion && s.orden == siguienteproceso).FirstOrDefault().Proceso_IDProceso;

            }
            catch (Exception err)
            {

            }
            ArticuloProduccion aertP = new ArticuloProduccion();
            try
            {
                aertP = new ArticulosProduccionContext().Database.SqlQuery<ArticuloProduccion>("select * from ArticuloProduccion where idtipoarticulo=3 and  idorden=" + orden.IDOrden + " and idproceso=" + siguientepr).ToList().FirstOrDefault();

            }
            catch (Exception err)
            {

            }

            OrdenProduccionDetalle procesodetalle = new OrdenProduccionDetalleContext().Database.SqlQuery<OrdenProduccionDetalle>("select * from OrdenProducciondetalle where idorden=" + orden.IDOrden + " and idproceso=" + siguientepr).ToList().FirstOrDefault();
            try
            {
                if (siguientepr == 16)
                {
                    if (aertP == null)
                    {
                        db.Database.ExecuteSqlCommand("INSERT INTO [dbo].[ArticuloProduccion]  ([IDArticulo],[IDTipoArticulo],[IDCaracteristica],[IDProceso],[IDOrden],[Cantidad],[IDClaveUnidad],[Indicaciones],[CostoPlaneado],[CostoReal],[Existe],[IDHE],[TC],[TCR])VALUES(2685,3,4502,16," + orden.IDOrden + ",1,59,'',1,1,'1',0,23.56,23.56)");

                        aertP = new ArticulosProduccionContext().Database.SqlQuery<ArticuloProduccion>("select * from ArticuloProduccion where idtipoarticulo=3 and  idorden=" + orden.IDOrden + " and idproceso=" + siguientepr).ToList().FirstOrDefault();

                    }

                }
                //sellado
                if (siguientepr == 11)
                {
                    if (aertP == null)
                    {
                        db.Database.ExecuteSqlCommand("INSERT INTO [dbo].[ArticuloProduccion]  ([IDArticulo],[IDTipoArticulo],[IDCaracteristica],[IDProceso],[IDOrden],[Cantidad],[IDClaveUnidad],[Indicaciones],[CostoPlaneado],[CostoReal],[Existe],[IDHE],[TC],[TCR])VALUES(89,3,35,11," + orden.IDOrden + ",1,59,'',1,1,'1',0,23.56,23.56)");

                        aertP = new ArticulosProduccionContext().Database.SqlQuery<ArticuloProduccion>("select * from ArticuloProduccion where idtipoarticulo=3 and  idorden=" + orden.IDOrden + " and idproceso=" + siguientepr).ToList().FirstOrDefault();

                    }

                }
                //inspeccion
                if (siguientepr == 12)
                {
                    if (aertP == null)
                    {
                        db.Database.ExecuteSqlCommand("INSERT INTO [dbo].[ArticuloProduccion]  ([IDArticulo],[IDTipoArticulo],[IDCaracteristica],[IDProceso],[IDOrden],[Cantidad],[IDClaveUnidad],[Indicaciones],[CostoPlaneado],[CostoReal],[Existe],[IDHE],[TC],[TCR])VALUES(2684,3,3733,12," + orden.IDOrden + ",1,59,'',1,1,'1',0,23.56,23.56)");

                        aertP = new ArticulosProduccionContext().Database.SqlQuery<ArticuloProduccion>("select * from ArticuloProduccion where idtipoarticulo=3 and  idorden=" + orden.IDOrden + " and idproceso=" + siguientepr).ToList().FirstOrDefault();

                    }

                }
            }
            catch (Exception err)
            {

            }
            int prioridad = 0;
            try
            {
                string consulta = "select max(prioridad) as Dato from prioridades where estado='Programado' and idmaquina=" + aertP.IDArticulo + " and idorden=" + orden.IDOrden + " and idproceso=" + siguientepr;
                ClsDatoEntero prio = db.Database.SqlQuery<ClsDatoEntero>(consulta).ToList().FirstOrDefault();


                prioridad = prio.Dato;
            }
            catch (Exception err)
            {

            }
            prioridad = prioridad + 1;
            try
            {
                db.Database.ExecuteSqlCommand("update Prioridades set prioridad=9999,Estado='Programado' where IDOrden=" + BitacoraOrden.IDOrden + " and idmaquina= "+ maquinaproceso.IDArticulo + " and idproceso="+ idprocesodelamaquina);

            }
            catch (Exception err)
            {

            }
            //// SI TERMINA BITACORA Y PROCESO

            bool terminarBitacora = false;


            if (siguientepr == 0)
            {
                
                    terminarBitacora = false;
               
               
                
            }

            if (terminarBitacora)
            {
                
                ClsDatoEntero CuentaProcesoXOrden = db.Database.SqlQuery<ClsDatoEntero>("select count(IDProceso) as Dato from prioridades where IDOrden=" + BitacoraOrden.IDOrden + "and IDProceso=" + siguientepr).ToList().FirstOrDefault();
                if (CuentaProcesoXOrden.Dato == 0)
                {
                    string cadena = "insert into prioridades (IDOrden,IDProceso,IDMaquina,Prioridad,Estado) values(" + BitacoraOrden.IDOrden + "," + siguientepr + "," + aertP.IDArticulo + ","+ prioridad + ",'Programado')";
                    db.Database.ExecuteSqlCommand(cadena);
                }
             


            }

            return RedirectToAction("Index", new { idorden = bita.IDOrden, idproceso = bita.IDProceso });
        }
        public ActionResult FinalizaProceso(int? id, int? idproceso)

        {


            /// automata de bitacota


            return RedirectToAction("FinalizaProcesoB", new { id = id, terminarproceso = true, idproceso = idproceso });
        }

        public ActionResult FinalizaProcesoB(int? id, int? idproceso, bool terminarproceso = false)
        {
            ViewBag.id = id;
            var elemento = db.VBitacoras.Single(m => m.IDBitacora == id);
            string horafecha = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            ViewBag.horafecha = horafecha;
            c_ClaveUnidadContext claveu = new c_ClaveUnidadContext();

            ViewBag.IDClaveUnidad = null;
            if (idproceso == 16)
            {
                ViewBag.IDClaveUnidad = new SelectList(claveu.c_ClaveUnidades.Where(s => s.IDClaveUnidad == 63), "IDClaveUnidad", "Nombre");
            }
            if (idproceso == 5 || idproceso == 11 || idproceso == 12)
            {
                ViewBag.IDClaveUnidad = new SelectList(claveu.c_ClaveUnidades.Where(s => s.IDClaveUnidad == 55), "IDClaveUnidad", "Nombre");

            }
            if (idproceso == 4)
            {
                ViewBag.IDClaveUnidad = new SelectList(claveu.c_ClaveUnidades.Where(s => s.IDClaveUnidad == 63 || s.IDClaveUnidad == 66), "IDClaveUnidad", "Nombre");

            }
            if (ViewBag.IDClaveUnidad == null)
            {

                ViewBag.IDClaveUnidad = new SelectList(claveu.c_ClaveUnidades, "IDClaveUnidad", "Nombre");


            }

            List<VArticulosProduccion> articulosproduccion = db.Database.SqlQuery<VArticulosProduccion>("select AP.IDClaveUnidad,AP.Existe,AP.IDArtProd,A.Descripcion as Articulo,TA.Descripcion as TipoArticulo,C.Presentacion as Caracteristica,P.NombreProceso as Proceso,AP.IDOrden,AP.Cantidad,CU.Nombre as Unidad,AP.Indicaciones,AP.CostoPlaneado,AP.CostoReal from ArticuloProduccion as AP inner join Articulo as A on A.IDArticulo=AP.IDArticulo inner join TipoArticulo as TA on A.IDTipoArticulo=TA.IDTipoArticulo inner join Caracteristica as C on C.ID=AP.IDCaracteristica inner join Proceso as P on P.IDProceso=AP.IDProceso inner join c_ClaveUnidad as CU on CU.IDClaveUnidad=AP.IDClaveUnidad where AP.IDOrden='" + elemento.IDOrden + "' and AP.IDProceso='" + elemento.IDProceso + "'").ToList();

            ViewBag.Terminarproceso = terminarproceso.ToString();

            return View(articulosproduccion);
        }


        [HttpPost]
        public ActionResult FinalizaProcesoB(int? id, FormCollection colecciondecontroles, Bitacora bitacora, List<VArticulosProduccion> cr)
        {
            
            bitacora.FechaFin = DateTime.Parse(colecciondecontroles.Get("FechaFin"));
            bitacora.Observacion = colecciondecontroles.Get("Observacion");
            bitacora.IDClaveUnidad = int.Parse(colecciondecontroles.Get("IDClaveUnidad"));
            bitacora.Cantidad = decimal.Parse(colecciondecontroles.Get("Cantidad"));
            VBitacora bita = db.VBitacoras.Find(id);
            string fecha = bitacora.FechaFin.Value.ToString("s");

            bool terminarproceso = bool.Parse(colecciondecontroles.Get("Terminarproceso"));

            HttpPostedFileBase archivo = Request.Files["files1"];
            saveIMG(archivo);



            XmlSerializer serializer = new XmlSerializer(typeof(automata));

            // Declare an object variable of the type to be deserialized.
            automata Automata;

            string path = Path.Combine(Server.MapPath("~/Automatas/Bitacora.Xml"));


            using (TextReader reader = new StreamReader(path))
            {
                serializer = new XmlSerializer(typeof(automata));
                Automata = (automata)serializer.Deserialize(reader);
            }
            Bitacora op = new BitacoraContext().Bitacoras.Find(id);


            try
            {
                foreach (var i in cr)
                {
                    if (i.Existe.Equals(true))
                    {
                        db.Database.ExecuteSqlCommand("INSERT INTO [dbo].[ArticuloProduccionReal]([IDOrden],[IDArtProd],[IDBitacora],[IDTrabajador],[IDProceso],[Cantidad],[IDClaveUnidad])VALUES('" + op.IDOrden + "','" + i.IDArtProd + "','" + id + "','" + op.IDTrabajador + "','" + op.IDProceso + "','" + i.Cantidad + "','" + i.IDClaveUnidad + "')");
                    }
                }
            }
            catch (Exception err )
            {

            }


            Automata.ejecutar(op.EstadoBitacora, "T");

            string estadoBitacora = Automata.Estadoactual.Nombre;

            ///TERMINA BITACORA
            db.Database.ExecuteSqlCommand("update [dbo].[Bitacora] set EstadoBitacora='" + estadoBitacora + "', [FechaFin]='" + fecha + "',[Observacion]='" + bitacora.Observacion + "', [Cantidad]='" + bitacora.Cantidad + "', [IDClaveUnidad]='" + bitacora.IDClaveUnidad + "',imagen='" + archivo.FileName + "' where IDBitacora=" + id);




            DatoString dato = db.Database.SqlQuery<DatoString>("SELECT convert(nvarchar(8), DATEADD(SECOND,(SELECT DATEDIFF(SECOND,convert(char(8), (select FechaInicio from Bitacora where IDBitacora='" + id + "'), 108) , convert(char(8), (select FechaFin from Bitacora where IDBitacora='" + id + "'), 108))), CAST('00:00:00' AS TIME))) as Dato").ToList()[0];
            db.Database.ExecuteSqlCommand("update [dbo].[Bitacora] set [DiferenciaHoras]='" + dato.Dato + "' where IDBitacora=" + id);

            ///// SI ES EL ULTIMO PROCESO Y SE TERMINA LA BITACORA SE AGREGA PARA PODER HACER UNA LIBERACION PARCIAL
            OrdenProduccion orden = new OrdenProduccionContext().OrdenesProduccion.Find(op.IDOrden);
            try
            {


                OrdenProduccionDetalle procesoactual = db.Database.SqlQuery<OrdenProduccionDetalle>("select * from OrdenProduccionDetalle where IDOrden=" + op.IDOrden + " and estadoProceso='Terminado' and IDProceso=" + op.IDProceso).FirstOrDefault();

                bool eselutimo = seterminaelultimoproceo(orden.IDOrden, op.IDProceso);
                if (bitacora.Cantidad > 0)
                {
                    if (eselutimo)
                    {
                        db.Database.ExecuteSqlCommand("INSERT INTO [dbo].[OrdenesALiberar]([IDOrden],[IDProceso],Cantidad)VALUES('" + bita.IDOrden + "','" + op.IDProceso + "'," + bitacora.Cantidad + ")");


                    }

                }

            }
            
            catch (Exception err)
            {
                string mensaje = err.Message;
                return RedirectToAction("Index", new { idorden = bita.IDOrden, idproceso = bita.IDProceso });
            }


            ////TERMINAR EL PROCESO DE LA ORDEN
            ///

            Bitacora BitacoraOrden = new BitacoraContext().Bitacoras.Find(id);
            VMaquinaProceso maquinaproceso = new VMaquinaProcesoContext().VMaquinaProceso.Where(s => s.IDArticulo == BitacoraOrden.IDMaquina).FirstOrDefault();
            int idprocesodelamaquina = maquinaproceso.IDProceso;


            ModeloProcesoContext mpc = new ModeloProcesoContext();

            int ordenproceso = mpc.Database.SqlQuery<ClsDatoEntero>("select orden as Dato from Modeloproceso where ModelosdeProduccion_IDModeloProduccion= " + orden.IDModeloProduccion + " and Proceso_IDProceso=" + idprocesodelamaquina).ToList().FirstOrDefault().Dato;

            int siguienteproceso = ordenproceso + 1;
            int siguientepr = 0;
            try
            {
                siguientepr= mpc.ModeloProceso.Where(s => s.ModelosDeProduccion_IDModeloProduccion == orden.IDModeloProduccion && s.orden == siguienteproceso).FirstOrDefault().Proceso_IDProceso;

            }
            catch (Exception err)
            {

            }
            ArticuloProduccion aertP = new ArticuloProduccion();
            try
            {
               aertP = new ArticulosProduccionContext().Database.SqlQuery<ArticuloProduccion>("select * from ArticuloProduccion where idtipoarticulo=3 and  idorden=" + orden.IDOrden + " and idproceso=" + siguientepr).ToList().FirstOrDefault();

            }
            catch (Exception err )
            {

            }
            int prioridad = 0;

            bool TIEENEPROCESOSIG = true;

            if (siguientepr == 0)
            {
               

                
                try
                {
                    ClsDatoEntero prio = db.Database.SqlQuery<ClsDatoEntero>("select max(prioridad) as Dato from prioridades where estado='Programado' and idmaquina=" + aertP.IDArticulo + " and idorden=" + orden.IDOrden + " and idproceso=" + siguientepr).ToList().FirstOrDefault();


                    prioridad = prio.Dato;
                }
                catch (Exception err)
                {

                }
                prioridad = prioridad + 1;

                db.Database.ExecuteSqlCommand("update [dbo].[Prioridades] set estado='Terminado',prioridad=0 where IDOrden=" + BitacoraOrden.IDOrden);

                /// termina el proceso en la table ordenesdeproducciondetalle

                db.Database.ExecuteSqlCommand("update OrdenProduccionDetalle set EstadoProceso='Terminado' where IDOrden=" + BitacoraOrden.IDOrden );

                db.Database.ExecuteSqlCommand("update OrdenProduccion set EstadoOrden='Finalizada' where IDOrden=" + BitacoraOrden.IDOrden + "");

                TIEENEPROCESOSIG = false;
                //throw new Exception("No hay proceso siguiente");
            }
            if (TIEENEPROCESOSIG)
            {
               

                try
                {

                                        
                        //// SE TERMINA EN PRIORIDADES EL PROCESO QUE SE FINALIZO EN BITACORA
                        db.Database.ExecuteSqlCommand("update [dbo].[Prioridades] set estado='Terminado',prioridad=0 where idmaquina=" + BitacoraOrden.IDMaquina + "and IDProceso=" + idprocesodelamaquina + " and IDOrden=" + BitacoraOrden.IDOrden);

                        /// termina el proceso en la table ordenesdeproducciondetalle

                        db.Database.ExecuteSqlCommand("update OrdenProduccionDetalle set EstadoProceso='Terminado' where IDOrden=" + op.IDOrden + " and IDProceso=" + idprocesodelamaquina);





                        List<Bitacora> eleBitacora = db.Database.SqlQuery<Bitacora>("select * from Bitacora where idproceso=" + idprocesodelamaquina + " and  IDOrden=" + op.IDOrden).ToList();

                        foreach (Bitacora b in eleBitacora)
                        {
                            if (b.EstadoBitacora == "Iniciada")
                            {
                                db.Database.ExecuteSqlCommand("update [dbo].[Bitacora] set EstadoBitacora='Terminado', [FechaFin]='" + fecha + "',[Observacion]='" + bitacora.Observacion + "', [Cantidad]='" + bitacora.Cantidad + "', [IDClaveUnidad]='" + bitacora.IDClaveUnidad + "',imagen='" + archivo.FileName + "' where  idproceso="+idprocesodelamaquina+" and IDOrden=" + op.IDOrden);

                            }
                        }





                        bool pasa = true;
                        try
                        {




                            try
                            {
                                 
                                    //corte
                                    if (siguientepr == 16)
                                    {
                                        if (aertP == null)
                                        {
                                            db.Database.ExecuteSqlCommand("INSERT INTO [dbo].[ArticuloProduccion]  ([IDArticulo],[IDTipoArticulo],[IDCaracteristica],[IDProceso],[IDOrden],[Cantidad],[IDClaveUnidad],[Indicaciones],[CostoPlaneado],[CostoReal],[Existe],[IDHE],[TC],[TCR])VALUES(2685,3,4502,16," + orden.IDOrden + ",1,59,'',1,1,'1',0,23.56,23.56)");

                                            aertP = new ArticulosProduccionContext().Database.SqlQuery<ArticuloProduccion>("select * from ArticuloProduccion where idtipoarticulo=3 and  idorden=" + orden.IDOrden + " and idproceso=" + siguientepr).ToList().FirstOrDefault();

                                        }

                                    }
                                    //sellado
                                    if (siguientepr == 11)
                                    {
                                        if (aertP == null)
                                        {
                                            db.Database.ExecuteSqlCommand("INSERT INTO [dbo].[ArticuloProduccion]  ([IDArticulo],[IDTipoArticulo],[IDCaracteristica],[IDProceso],[IDOrden],[Cantidad],[IDClaveUnidad],[Indicaciones],[CostoPlaneado],[CostoReal],[Existe],[IDHE],[TC],[TCR])VALUES(89,3,35,11," + orden.IDOrden + ",1,59,'',1,1,'1',0,23.56,23.56)");

                                            aertP = new ArticulosProduccionContext().Database.SqlQuery<ArticuloProduccion>("select * from ArticuloProduccion where idtipoarticulo=3 and  idorden=" + orden.IDOrden + " and idproceso=" + siguientepr).ToList().FirstOrDefault();

                                        }

                                    }
                                    //inspeccion
                                    if (siguientepr == 12)
                                    {
                                        if (aertP == null)
                                        {
                                            db.Database.ExecuteSqlCommand("INSERT INTO [dbo].[ArticuloProduccion]  ([IDArticulo],[IDTipoArticulo],[IDCaracteristica],[IDProceso],[IDOrden],[Cantidad],[IDClaveUnidad],[Indicaciones],[CostoPlaneado],[CostoReal],[Existe],[IDHE],[TC],[TCR])VALUES(2684,3,3733,12," + orden.IDOrden + ",1,59,'',1,1,'1',0,23.56,23.56)");

                                            aertP = new ArticulosProduccionContext().Database.SqlQuery<ArticuloProduccion>("select * from ArticuloProduccion where idtipoarticulo=3 and  idorden=" + orden.IDOrden + " and idproceso=" + siguientepr).ToList().FirstOrDefault();

                                        }

                                    }

                            ClsDatoEntero cuentaExiste = db.Database.SqlQuery<ClsDatoEntero>("select count(idorden) as Dato from prioridades where (estado='Programado' or estado='Iniciada') and idmaquina=" + aertP.IDArticulo + " and idorden=" + orden.IDOrden + " and idproceso=" + siguientepr).ToList().FirstOrDefault();
                            try
                            {
                                if (cuentaExiste.Dato > 0)
                                {

                                }
                                else
                                {
                                    string cadena = "insert into prioridades (IDOrden,IDProceso,IDMaquina,Prioridad,Estado) values(" + orden.IDOrden + "," + siguientepr + "," + aertP.IDArticulo + "," + prioridad + ",'Programado')";
                                    db.Database.ExecuteSqlCommand(cadena);
                                }
                            }
                            catch (Exception err)
                            {

                            }



                        }
                            catch (Exception err)
                            {

                            }
                            db.Database.ExecuteSqlCommand("update OrdenProducciondetalle set EstadoProceso='Programado' where IDProceso="+ siguientepr);



                        }
                        catch (Exception err)
                        {
                            pasa = false;
                        }



                    


                   

                }
                catch (Exception err)
                {
                    string mensaje = err.Message;
                    return RedirectToAction("Index", new { idorden = bita.IDOrden, idproceso = bita.IDProceso });
                }


            }


            return RedirectToAction("Index", new { idorden = bita.IDOrden, idproceso = bita.IDProceso });
        }
        /////////////////////////////////////////////////////////////////////////////////////Reporte Desperfecto///////////////////////////////////////////////////////////////////////////////////////////

        public ActionResult Reportes(string sortOrder, int? idorden, string currentFilter, string searchString, int? page, int? PageSize, string Familia, int? idproceso, int? orden)
        {
            ReporteDesperfectoContext re = new ReporteDesperfectoContext();
            OrdenProduccionContext ordena = new OrdenProduccionContext();
            ViewBag.idorden = new SelectList(ordena.OrdenesProduccion.Where(a => a.EstadoOrden == "Programada"), "IDOrden", "IDOrden");

            ViewBag.idorden = null;
            ViewBag.idproceso = null;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.vercreate = null;

            var elementos = from s in re.ReporteDesperfectos
                            select s;
            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                elementos = elementos.Where(s => s.IDBitacora.ToString().Contains(searchString));
            }


            switch (sortOrder)
            {
                case "Bitacora":
                    elementos = elementos.OrderBy(s => s.IDBitacora);
                    break;

                default:
                    elementos = elementos.OrderBy(s => s.IDBitacora);
                    break;
            }

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = re.ReporteDesperfectos.OrderBy(e => e.IDBitacora).Count(); // Total number of elements

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

        public ActionResult ReporteD(int? id, ReporteDesperfecto reportedesperfecto, int? inserta)
        {
            Bitacora bitacora = new BitacoraContext().Bitacoras.Find(id);
            ClsDatoEntero tipomaquina = db.Database.SqlQuery<ClsDatoEntero>("select IDTipoArticulo as Dato from TipoArticulo where Descripcion='Maquina'").ToList()[0];
            ArticuloContext articulo = new ArticuloContext();
            ViewBag.IDArticulo = new SelectList(articulo.Articulo.Where(s => s.IDTipoArticulo.Equals(tipomaquina.Dato)), "IDArticulo", "Descripcion");

            string horafecha = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            ViewBag.horafecha = horafecha;
            if (inserta == 1)
            {
                string fecha = reportedesperfecto.FechaInicioParo.ToString("s");
                db.Database.ExecuteSqlCommand("insert into ReporteDesperfecto([IDBitacora],[IDArticulo],[FechaInicioParo],[FechaTerminacionParo],[TiempoFalla],[Falla]) values ('" + id + "','" + reportedesperfecto.IDArticulo + "','" + fecha + "',null,null,'" + reportedesperfecto.Falla + "')");

                ////Bitacora
                XmlSerializer serializer = new XmlSerializer(typeof(automata));

                // Declare an object variable of the type to be deserialized.
                automata Automata;

                string path = Path.Combine(Server.MapPath("~/Automatas/Bitacora.Xml"));


                using (TextReader reader = new StreamReader(path))
                {
                    serializer = new XmlSerializer(typeof(automata));
                    Automata = (automata)serializer.Deserialize(reader);
                }
                Bitacora op = new BitacoraContext().Bitacoras.Find(id);

                Automata.ejecutar(op.EstadoBitacora, "R");

                string estadoBitacora = Automata.Estadoactual.Nombre;

                /////// Proceso
                ///

                path = Path.Combine(Server.MapPath("~/Automatas/Proceso.Xml"));


                using (TextReader reader = new StreamReader(path))
                {
                    serializer = new XmlSerializer(typeof(automata));
                    Automata = (automata)serializer.Deserialize(reader);
                }
                OrdenProduccionDetalle pro = new OrdenProduccionDetalleContext().OrdenProduccionDetalles.Where(s => s.IDOrden == op.IDOrden && s.IDProceso == op.IDProceso).ToList().FirstOrDefault();

                Automata.ejecutar(pro.EstadoProceso, "R");

                string estadoProceso = Automata.Estadoactual.Nombre;


                db.Database.ExecuteSqlCommand("update [dbo].[OrdenProduccionDetalle] set [EstadoProceso]='" + estadoProceso + "' where IDOrden=" + op.IDOrden + " and IDProceso=" + op.IDProceso);
                db.Database.ExecuteSqlCommand("update [dbo].[Bitacora] set [EstadoBitacora]='" + estadoBitacora + "' where IDBitacora=" + id);

                return RedirectToAction("Reportes", new { id = id });
            }
            ViewBag.id = id;
            return View();
        }




        public ActionResult FinalizaReporte(int? id)
        {
            ReporteDesperfectoContext dbrd = new ReporteDesperfectoContext();
            ReporteDesperfecto reporte = dbrd.ReporteDesperfectos.Find(id);
            string fecha = DateTime.Now.ToString("s");

            ////Bitacora
            XmlSerializer serializer = new XmlSerializer(typeof(automata));

            // Declare an object variable of the type to be deserialized.
            automata Automata;

            string path = Path.Combine(Server.MapPath("~/Automatas/Bitacora.Xml"));


            using (TextReader reader = new StreamReader(path))
            {
                serializer = new XmlSerializer(typeof(automata));
                Automata = (automata)serializer.Deserialize(reader);
            }

            int datob = db.Database.SqlQuery<ClsDatoEntero>("select IDBitacora as Dato from [dbo].[ReporteDesperfecto] where IDReporte=" + id).ToList().FirstOrDefault().Dato;



            Bitacora op = new BitacoraContext().Bitacoras.Find(datob);


            Automata.ejecutar(op.EstadoBitacora, "R");

            string estadoBitacora = Automata.Estadoactual.Nombre;

            /////// Proceso
            ///

            path = Path.Combine(Server.MapPath("~/Automatas/Proceso.Xml"));


            using (TextReader reader = new StreamReader(path))
            {
                serializer = new XmlSerializer(typeof(automata));
                Automata = (automata)serializer.Deserialize(reader);
            }
            OrdenProduccionDetalle pro = new OrdenProduccionDetalleContext().OrdenProduccionDetalles.Where(s => s.IDOrden == op.IDOrden && s.IDProceso == op.IDProceso).ToList().FirstOrDefault();

            Automata.ejecutar(pro.EstadoProceso, "R");

            string estadoProceso = Automata.Estadoactual.Nombre;


            db.Database.ExecuteSqlCommand("update [dbo].[OrdenProduccionDetalle] set [EstadoProceso]='" + estadoProceso + "' where IDOrden=" + op.IDOrden + " and IDProceso=" + op.IDProceso);
            db.Database.ExecuteSqlCommand("update [dbo].[Bitacora] set [EstadoBitacora]='" + estadoBitacora + "' where IDBitacora=" + id);
            db.Database.ExecuteSqlCommand("update [dbo].[ReporteDesperfecto] set [FechaTerminacionParo]='" + fecha + "' where IDReporte=" + id);

            DatoString dato = db.Database.SqlQuery<DatoString>("SELECT convert(nvarchar(8), DATEADD(SECOND,(SELECT DATEDIFF(SECOND,convert(char(8), (select FechaInicioParo from ReporteDesperfecto where IDReporte='" + id + "'), 108) , convert(char(8), (select FechaTerminacionParo from ReporteDesperfecto where IDReporte='" + id + "'), 108))), CAST('00:00:00' AS TIME))) as Dato").ToList()[0];
            db.Database.ExecuteSqlCommand("update [dbo].[ReporteDesperfecto] set [TiempoFalla]='" + dato.Dato + "' where IDReporte=" + id);
            return RedirectToAction("Reportes", new { id = reporte.IDBitacora });
            // return Json(200, JsonRequestBehavior.AllowGet);
        }


        public async Task<ActionResult> RenderImage(int? id)
        {
            VBitacoraContext db = new VBitacoraContext();
            VBitacora item = await db.VBitacoras.FindAsync(id);
            string path = Server.MapPath("~/imagenes/Bitacora/");

            byte[] photoBack = System.IO.File.ReadAllBytes(path + item.Imagen);


            return File(photoBack, "image/png");
        }

        public ActionResult ListaTintas(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize, string Familia)
        {
            //Buscar Serie Factura

            var SerLst = new List<string>();

            ViewBag.Familias = new FamiliaRepository().GetFamilias();
            ViewBag.Familia = Familia;

            if (sortOrder == string.Empty)
            {
                sortOrder = "Prioridad";
            }

            VPArticuloContext db = new VPArticuloContext();
            ViewBag.CurrentSort = sortOrder;
            ViewBag.ASortParm = String.IsNullOrEmpty(sortOrder) ? "Descripcion" : "Descripcion";
            ViewBag.BSortParm = String.IsNullOrEmpty(sortOrder) ? "Prioridad" : "Prioridad";
            ViewBag.CSortParm = String.IsNullOrEmpty(sortOrder) ? "Familia" : "Familia";
            // Not sure here
            if (searchString == null)
            {
                searchString = currentFilter;
            }

            // Pass filtering string to view in order to maintain filtering when paging
            ViewBag.SearchString = searchString;
            ViewBag.CurrentFilter = searchString;
            //Paginación


            string cadena = "select o.IDOrden,a.IDArticulo, o.Prioridad, a.Cref, a.Descripcion, c.Nombre as Cliente from Articulo as a inner join ArticuloProduccion as ap on a.IDArticulo=ap.IDArticulo inner join OrdenProduccion as o on ap.IDOrden=o.IDOrden inner join TipoArticulo as t on a.IDTipoArticulo=t.IDTipoArticulo inner join Clientes as c on o.IDCliente= c.IDCliente where t.IDTipoArticulo=7 and o.EstadoOrden ='Programada' ";
            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                if (string.IsNullOrEmpty(Familia))
                {
                    cadena = "select o.IDOrden,a.IDArticulo, o.Prioridad, a.Cref, a.Descripcion, c.Nombre as Cliente from Articulo as a inner join ArticuloProduccion as ap on a.IDArticulo=ap.IDArticulo inner join OrdenProduccion as o on ap.IDOrden=o.IDOrden inner join TipoArticulo as t on a.IDTipoArticulo=t.IDTipoArticulo inner join Clientes as c on o.IDCliente= c.IDCliente where t.IDTipoArticulo=7  and o.EstadoOrden='Programada' and (a.cref like '%" + searchString + "%' or a.Descripcion like '%" + searchString + "%') ";
                }
                else
                {
                    cadena = "select o.IDOrden,a.IDArticulo, o.Prioridad, a.Cref, a.Descripcion, c.Nombre as Cliente from Articulo as a inner join ArticuloProduccion as ap on a.IDArticulo=ap.IDArticulo inner join OrdenProduccion as o on ap.IDOrden=o.IDOrden inner join TipoArticulo as t on a.IDTipoArticulo=t.IDTipoArticulo inner join Clientes as c on o.IDCliente= c.IDCliente where t.IDTipoArticulo=7 and o.EstadoOrden='Programada'  and o.EstadoOrden <>'Cancelada' and (a.cref like '%" + searchString + "%' or a.Descripcion like '%" + searchString + "%') and a.IDFamilia =" + Familia + " ";
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(Familia) && (Familia != "-"))
                {
                    cadena = "select o.IDOrden,a.IDArticulo, a.Cref, o.Prioridad, a.Descripcion, c.Nombre as Cliente from Articulo as a inner join ArticuloProduccion as ap on a.IDArticulo=ap.IDArticulo inner join OrdenProduccion as o on ap.IDOrden=o.IDOrden inner join TipoArticulo as t on a.IDTipoArticulo=t.IDTipoArticulo inner join Clientes as c on o.IDCliente= c.IDCliente where t.IDTipoArticulo=7 and o.EstadoOrden='Programada'   and a.IDFamilia=" + Familia + " ";
                }
                else
                {
                    cadena = "select o.IDOrden,a.IDArticulo, a.Cref, o.Prioridad, a.Descripcion, c.Nombre as Cliente from Articulo as a inner join ArticuloProduccion as ap on a.IDArticulo=ap.IDArticulo inner join OrdenProduccion as o on ap.IDOrden=o.IDOrden inner join TipoArticulo as t on a.IDTipoArticulo=t.IDTipoArticulo inner join Clientes as c on o.IDCliente= c.IDCliente where t.IDTipoArticulo=7 and o.EstadoOrden='Programada'";
                }
            }



            //Ordenacion

            switch (sortOrder)
            {
                case "Prioridad":
                    cadena = cadena + " order by o.Prioridad";
                    break;

                default:
                    cadena = cadena + " order by o.Prioridad"; ;
                    break;
            }



            List<VOrdenProduccion> elementos = db.Database.SqlQuery<VOrdenProduccion>(cadena).ToList<VOrdenProduccion>();
            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
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

            return View(elementos.ToPagedList(pageNumber, pageSize));
            //Paginación
        }
        public ActionResult Cintas(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize, string Familia)
        {
            //Buscar Serie Factura

            var SerLst = new List<string>();

            ViewBag.Familias = new FamiliaRepository().GetFamilias();
            ViewBag.Familia = Familia;

            if (sortOrder == string.Empty)
            {
                sortOrder = "Prioridad";
            }

            VPArticuloContext db = new VPArticuloContext();
            ViewBag.CurrentSort = sortOrder;
            ViewBag.ASortParm = String.IsNullOrEmpty(sortOrder) ? "Descripcion" : "Descripcion";
            ViewBag.BSortParm = String.IsNullOrEmpty(sortOrder) ? "Prioridad" : "Prioridad";
            ViewBag.CSortParm = String.IsNullOrEmpty(sortOrder) ? "Familia" : "Familia";
            // Not sure here
            if (searchString == null)
            {
                searchString = currentFilter;
            }

            // Pass filtering string to view in order to maintain filtering when paging
            ViewBag.SearchString = searchString;
            ViewBag.CurrentFilter = searchString;
            //Paginación


            string cadena = "select o.IDOrden,a.IDArticulo, a.Cref, a.Descripcion, o.Prioridad, c.Nombre as Cliente from Articulo as a inner join ArticuloProduccion as ap on a.IDArticulo=ap.IDArticulo inner join OrdenProduccion as o on ap.IDOrden=o.IDOrden inner join TipoArticulo as t on a.IDTipoArticulo=t.IDTipoArticulo inner join Clientes as c on o.IDCliente= c.IDCliente where t.IDTipoArticulo=6 and o.EstadoOrden ='Programada'";
            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                if (string.IsNullOrEmpty(Familia))
                {
                    cadena = "select o.IDOrden,a.IDArticulo, a.Cref, o.Prioridad, a.Descripcion, c.Nombre as Cliente from Articulo as a inner join ArticuloProduccion as ap on a.IDArticulo=ap.IDArticulo inner join OrdenProduccion as o on ap.IDOrden=o.IDOrden inner join TipoArticulo as t on a.IDTipoArticulo=t.IDTipoArticulo inner join Clientes as c on o.IDCliente= c.IDCliente where t.IDTipoArticulo=6  and o.EstadoOrden ='Programada'  and (a.cref like '%" + searchString + "%' or a.Descripcion like '%" + searchString + "%') ";
                }
                else
                {
                    cadena = "select o.IDOrden,a.IDArticulo, a.Cref, o.Prioridad, a.Descripcion, c.Nombre as Cliente from Articulo as a inner join ArticuloProduccion as ap on a.IDArticulo=ap.IDArticulo inner join OrdenProduccion as o on ap.IDOrden=o.IDOrden inner join TipoArticulo as t on a.IDTipoArticulo=t.IDTipoArticulo inner join Clientes as c on o.IDCliente= c.IDCliente where t.IDTipoArticulo=6 and o.EstadoOrden ='Programada'  and (a.cref like '%" + searchString + "%' or a.Descripcion like '%" + searchString + "%') and a.IDFamilia =" + Familia + " ";
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(Familia) && (Familia != "-"))
                {
                    cadena = "select o.IDOrden,a.IDArticulo, a.Cref, o.Prioridad, a.Descripcion, c.Nombre as Cliente from Articulo as a inner join ArticuloProduccion as ap on a.IDArticulo=ap.IDArticulo inner join OrdenProduccion as o on ap.IDOrden=o.IDOrden inner join TipoArticulo as t on a.IDTipoArticulo=t.IDTipoArticulo inner join Clientes as c on o.IDCliente= c.IDCliente where t.IDTipoArticulo=6 and o.EstadoOrden ='Programada' and a.IDFamilia=" + Familia + " ";
                }
                else
                {
                    cadena = "select o.IDOrden,a.IDArticulo, a.Cref, o.Prioridad, a.Descripcion, c.Nombre as Cliente from Articulo as a inner join ArticuloProduccion as ap on a.IDArticulo=ap.IDArticulo inner join OrdenProduccion as o on ap.IDOrden=o.IDOrden inner join TipoArticulo as t on a.IDTipoArticulo=t.IDTipoArticulo inner join Clientes as c on o.IDCliente= c.IDCliente where t.IDTipoArticulo=6 and o.EstadoOrden ='Programada' ";
                }
            }



            //Ordenacion

            switch (sortOrder)
            {
                case "Prioridad":
                    cadena = cadena + " order by o.Prioridad";
                    break;

                default:
                    cadena = cadena + " order by o.Prioridad"; ;
                    break;
            }


            List<VOrdenProduccion> elementos = db.Database.SqlQuery<VOrdenProduccion>(cadena).ToList<VOrdenProduccion>();

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
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

            return View(elementos.ToPagedList(pageNumber, pageSize));
            //Paginación
        }

        public ActionResult Herramientas(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize, string Familia)
        {
            //Buscar Serie Factura

            var SerLst = new List<string>();

            ViewBag.Familias = new FamiliaRepository().GetFamilias();
            ViewBag.Familia = Familia;

            if (sortOrder == string.Empty)
            {
                sortOrder = "Prioridad";
            }

            VPArticuloContext db = new VPArticuloContext();
            ViewBag.CurrentSort = sortOrder;
            ViewBag.ASortParm = String.IsNullOrEmpty(sortOrder) ? "Descripcion" : "Descripcion";
            ViewBag.BSortParm = String.IsNullOrEmpty(sortOrder) ? "Prioridad" : "Prioridad";
            ViewBag.CSortParm = String.IsNullOrEmpty(sortOrder) ? "Familia" : "Familia";
            // Not sure here
            if (searchString == null)
            {
                searchString = currentFilter;
            }

            // Pass filtering string to view in order to maintain filtering when paging
            ViewBag.SearchString = searchString;
            ViewBag.CurrentFilter = searchString;
            //Paginación


            string cadena = "select o.IDOrden,a.IDArticulo, a.Cref, o.Prioridad, a.Descripcion, c.Nombre as Cliente from Articulo as a inner join ArticuloProduccion as ap on a.IDArticulo=ap.IDArticulo inner join OrdenProduccion as o on ap.IDOrden=o.IDOrden inner join TipoArticulo as t on a.IDTipoArticulo=t.IDTipoArticulo inner join Clientes as c on o.IDCliente= c.IDCliente where t.IDTipoArticulo=2 and o.EstadoOrden ='Programada' ";
            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                if (string.IsNullOrEmpty(Familia))
                {
                    cadena = "select o.IDOrden,a.IDArticulo, a.Cref, o.Prioridad, a.Descripcion, c.Nombre as Cliente from Articulo as a inner join ArticuloProduccion as ap on a.IDArticulo=ap.IDArticulo inner join OrdenProduccion as o on ap.IDOrden=o.IDOrden inner join TipoArticulo as t on a.IDTipoArticulo=t.IDTipoArticulo inner join Clientes as c on o.IDCliente= c.IDCliente where t.IDTipoArticulo=2 and o.EstadoOrden ='Programada' and (a.cref like '%" + searchString + "%' or a.Descripcion like '%" + searchString + "%') ";
                }
                else
                {
                    cadena = "select o.IDOrden,a.IDArticulo, a.Cref, o.Prioridad, a.Descripcion, c.Nombre as Cliente from Articulo as a inner join ArticuloProduccion as ap on a.IDArticulo=ap.IDArticulo inner join OrdenProduccion as o on ap.IDOrden=o.IDOrden inner join TipoArticulo as t on a.IDTipoArticulo=t.IDTipoArticulo inner join Clientes as c on o.IDCliente= c.IDCliente where t.IDTipoArticulo=2 and o.EstadoOrden ='Programada' and (a.cref like '%" + searchString + "%' or a.Descripcion like '%" + searchString + "%') and a.IDFamilia =" + Familia + " ";
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(Familia) && (Familia != "-"))
                {
                    cadena = "select o.IDOrden,a.IDArticulo, a.Cref, o.Prioridad, a.Descripcion, c.Nombre as Cliente from Articulo as a inner join ArticuloProduccion as ap on a.IDArticulo=ap.IDArticulo inner join OrdenProduccion as o on ap.IDOrden=o.IDOrden inner join TipoArticulo as t on a.IDTipoArticulo=t.IDTipoArticulo inner join Clientes as c on o.IDCliente= c.IDCliente where t.IDTipoArticulo=2  and o.EstadoOrden ='Programada' and a.IDFamilia=" + Familia + " ";
                }
                else
                {
                    cadena = "select o.IDOrden,a.IDArticulo, a.Cref, o.Prioridad, a.Descripcion, c.Nombre as Cliente from Articulo as a inner join ArticuloProduccion as ap on a.IDArticulo=ap.IDArticulo inner join OrdenProduccion as o on ap.IDOrden=o.IDOrden inner join TipoArticulo as t on a.IDTipoArticulo=t.IDTipoArticulo inner join Clientes as c on o.IDCliente= c.IDCliente where t.IDTipoArticulo=2 and o.EstadoOrden ='Programada'";
                }
            }



            //Ordenacion

            switch (sortOrder)
            {

                case "Prioridad":
                    cadena = cadena + " order by o.Prioridad";
                    break;

                default:
                    cadena = cadena + " order by o.Prioridad"; ;
                    break;
            }


            List<VOrdenProduccion> elementos = db.Database.SqlQuery<VOrdenProduccion>(cadena).ToList<VOrdenProduccion>();

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
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

            return View(elementos.ToPagedList(pageNumber, pageSize));
            //Paginación
        }


        public ActionResult ImpHerramientas()
        {

            EmpresaContext db = new EmpresaContext();

            //Console.WriteLine("Esto es mi mensaje : entre aqui" );
            // toma la empresa que tenga el id 1 via de mientras 
            var empresa = db.empresas.Find(2);

            System.Drawing.Image logoempresa = byteArrayToImage(empresa.Logo);




            SuajesyCyreles documento = new SuajesyCyreles();


            documento = new SuajesyCyreles();


            byte[] abytes = documento.PrepareReport();
            return File(abytes, "applicacion/pdf", "Programasuaje.pdf");



        }



        public ActionResult Imptintas()
        {

            EmpresaContext db = new EmpresaContext();

            //Console.WriteLine("Esto es mi mensaje : entre aqui" );
            // toma la empresa que tenga el id 1 via de mientras 
            var empresa = db.empresas.Find(2);

            System.Drawing.Image logoempresa = byteArrayToImage(empresa.Logo);




            ProgTintas documento = new ProgTintas();





            byte[] abytes = documento.PrepareReport();
            return File(abytes, "applicacion/pdf", "Programatinta.pdf");



        }

        public ActionResult OrdenesProgramadas(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize, int? idorden, int? idproceso, int? orden, int? Maquina = 0)
        {

            try
            {
                if (searchString == null)
                {
                    searchString = string.Empty;
                }
            }
            catch (Exception err)
            {
                searchString = string.Empty;
            }
            int pageNumber = 0;
            int pageSize = 0;
            int count = 0;
            VMaquinaProcesoContext dba = new VMaquinaProcesoContext();



            List<SelectListItem> nueva = new SelectList(dba.VMaquinaProceso, "IDArticulo", "Descripcion", Maquina).ToList();
            nueva.Insert(0, (new SelectListItem { Text = "Todas", Value = "0" }));

            ViewBag.Maquinas = nueva;
            ViewBag.Maquina = Maquina;

            string cadena = "";
            string cadenaMaquina = "";

            if (idproceso == null)
            {
                idproceso = 5;
            }

            OrdenProduccionContext ordena = new OrdenProduccionContext();
            ViewBag.idorden = new SelectList(ordena.Database.SqlQuery<OrdenProduccion>("Select OP.* from OrdenProduccion as OP inner join OrdenProduccionDetalle DP on OP.Idorden=DP.IDorden where IDPRoceso=" + idproceso + " and (EstadoProceso='Programado')  order by OP.prioridad").ToList());

            ViewBag.idorden = null;
            ViewBag.idproceso = null;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.vercreate = null;



            if (Maquina != 0)
            {
                ViewBag.MaquinaOrden = Maquina;
                VMaquinaProceso maquinaproceso = new VMaquinaProcesoContext().VMaquinaProceso.Where(s => s.IDArticulo == Maquina).FirstOrDefault();
                int idprocesodelamaquina = maquinaproceso.IDProceso;


                if (!string.IsNullOrEmpty(searchString))
                {
                    cadenaMaquina = "select o.idorden, p.idproceso, p.estado as EstadoProceso,c.nombre as Cliente, o.descripcion as Articulo, o.presentacion as Caracteristica, p.prioridad  from ordenproduccion as o inner join prioridades as p on o.idorden=p.idorden inner join clientes as c on c.idcliente=o.idcliente where o.idorden='" + idorden + "' and p.idmaquina=" + Maquina + " and p.idproceso=" + idprocesodelamaquina + " and (p.estado = 'Iniciada' or p.estado='Programado') ORDER BY CASE WHEN p.estado = 'Iniciada' then 1 when p.estado = 'Programado' then 2  end, p.prioridad";

                }
                else
                {
                    Reenumerar(Maquina);


                    cadenaMaquina = "select  o.idorden, p.idproceso, p.estado as EstadoProceso,c.nombre as Cliente, o.descripcion as Articulo, o.presentacion as Caracteristica, p.prioridad  from ordenproduccion as o inner join prioridades as p on o.idorden=p.idorden inner join clientes as c on c.idcliente=o.idcliente where p.idmaquina=" + Maquina + " and p.idproceso=" + idprocesodelamaquina + " and (p.estado = 'Iniciada' or p.estado='Programado') ORDER BY CASE WHEN p.estado = 'Iniciada' then 1 when p.estado = 'Programado' then 2  end, p.prioridad";
                }
                //List<OrdenProduccionDetalle> elementosOrden = db.Database.SqlQuery<OrdenProduccionDetalle>(cadena).ToList<OrdenProduccionDetalle>();
                //List<OrdenProduccion> ele = new List<OrdenProduccion>();
                List<OrdenProduccionPrioridades> ele = db.Database.SqlQuery<OrdenProduccionPrioridades>(cadenaMaquina).ToList<OrdenProduccionPrioridades>();





                //Paginación
                // DROPDOWNLIST FOR UPDATING PAGE SIZE
                //count = ele.Count;
                count = ele.OrderBy(e => e.Prioridad).Count();

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
            else
            {
                if (orden != null)
                {
                    ViewBag.vercreate = orden;
                }
                if (searchString == null)
                {
                    searchString = string.Empty;
                }
                cadena = "select o.idorden, p.idproceso, p.estado as EstadoProceso,c.nombre as Cliente, o.descripcion as Articulo, o.presentacion as Caracteristica, p.prioridad from ordenproduccion as o inner join prioridades as p on o.idorden=p.idorden inner join clientes as c on c.idcliente=o.idcliente  where  (p.estado='Programado' or p.estado='Iniciada')ORDER BY CASE WHEN p.estado = 'Iniciada' then 1 when p.estado = 'Programado' then 2 end, p.prioridad";

                ViewBag.SearchString = searchString;

                List<OrdenProduccionPrioridades> ele = db.Database.SqlQuery<OrdenProduccionPrioridades>(cadena).ToList<OrdenProduccionPrioridades>();



                //Paginación
                // DROPDOWNLIST FOR UPDATING PAGE SIZE
                count = ele.OrderBy(e => e.Prioridad).Count();

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


        }



        public void Reenumerar(int? maquina)
        {
            try
            {
                VMaquinaProceso maquinaproceso = new VMaquinaProcesoContext().VMaquinaProceso.Where(s => s.IDArticulo == maquina).FirstOrDefault();
                int idprocesodelamaquina = maquinaproceso.IDProceso;


                //string cadena = "select OP.* from (ordenproduccion as op inner join OrdenProduccionDetalle as apd on op.IDorden= apd.IDorden) inner join Articuloproduccion aP on op.idOrden=ap.idorden  where ap.idarticulo=" + maquina + " and ap.Idproceso=" + idprocesodelamaquina + " and (op.estadoorden='Iniciada' or op.estadoorden='Programada') and ( apd.EstadoProceso='Iniciado' or apd.EstadoProceso='Programado' )  order by op.prioridad, apd.EstadoProceso";
                //string cadena = "select*from OrdenProduccionMaquina" + maquina + " as op ORDER BY CASE WHEN op.ESTADOORDEN = 'Iniciada' then 1 when op.ESTADOORDEN = 'Programada' then 2  end, op.prioridad";
                string cadena = "select o.*from ordenproduccion as o inner join prioridades as p on o.idorden=p.idorden where p.idproceso=" + idprocesodelamaquina + " and  p.idmaquina=" + maquina + " and p.estado='Programado' ORDER BY CASE WHEN p.estado = 'Iniciada' then 1 when p.estado = 'Programado' then 2 end , p.prioridad";

                //string cadena = "select distinct(OP.IDORDEN),OP.* from ordenproduccion as op inner join articuloproduccion as ap on op.idorden=ap.idorden inner join EstadoOrden as e on op.estadoorden=e.descripcion where (op.estadoorden='Iniciada' or op.estadoorden='Programada') and ap.idarticulo=" + maquina + " and ap.Idtipoarticulo=3 order by prioridad";
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
        public void saveIMG(HttpPostedFileBase file)
        {
            try
            {
                if ((file != null))
                {

                    string ext = Path.GetExtension(file.FileName);



                    string path = Server.MapPath("~/imagenes/Bitacora/");

                    if (System.IO.File.Exists(path + file.FileName))
                    {
                        System.IO.File.Delete(path + file.FileName);
                    }

                    file.SaveAs(path + file.FileName);

                }


            }
            catch (Exception ex)
            {

                string mensaje = ex.Message;
            }
        }

        public ActionResult CambiarMaquinaB(int id, int idmaquinanueva, int orden, int proceso)
        {

            try
            {
                Caracteristica caracte = db.Database.SqlQuery<Caracteristica>("select * from Caracteristica where Articulo_IDArticulo=" + idmaquinanueva).ToList().FirstOrDefault();


                //db.Database.ExecuteSqlCommand("update articuloproduccion set IdArticulo=" + idmaquinanueva + ", IDCaracteristica=" + caracte.ID + " where idorden=" + orden + " and IDProceso=" + proceso + " and idarticulo=" + id);
                return Json(new HttpStatusCodeResult(200, "La maquina fue cambiada exitosamente!"));
            }
            catch (SqlException err)
            {
                string mensajederror = err.Message;
                return Json(new HttpStatusCodeResult(500, "No Pude"));
            }




            //db.Database.ExecuteSqlCommand("update OrdenProduccion set Prioridad=" + prioridadabajo + " where IDOrden=" + idordencambio + "");

            //db.Database.ExecuteSqlCommand("update OrdenProduccion set Prioridad=" + prioridadarriba + " where IDOrden=" + id + "");



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

        public ActionResult EntreFechasBP()
        {
            EFecha elemento = new EFecha();

            return View(elemento);
        }

        [HttpPost]
        public ActionResult EntreFechasBP(EFecha modelo, FormCollection coleccion)
        {
            VOrdenProduccionPedidoContext dbe = new VOrdenProduccionPedidoContext();
            VBitacoraReporteContext dbr = new VBitacoraReporteContext();
            VMatAsignadoContext dbm = new VMatAsignadoContext();
            string FI = modelo.fechaini.Year.ToString() + "-" + modelo.fechaini.Month.ToString() + "-" + modelo.fechaini.Day.ToString();
            string FF = modelo.fechafin.Year.ToString() + "-" + modelo.fechafin.Month.ToString() + "-" + modelo.fechafin.Day.ToString();

            string cual = coleccion.Get("Enviar");

            string cadena = "";
            string cadenaDet = "";
            string cadenaMat = "";
            if (cual == "Generar reporte")
            {
                return View();
            }
            if (cual == "Generar excel")
            {
                List<VOrdenProduccionPedido> datos;
                List<VBitacoraReporte> datosDet;
                List<VMatAsignado> datosMat;
                try
                {
                    cadena = "select * from VOrdenProduccionPedido where FechaPedido >= '" + FI + "' and FechaPedido  <='" + FF + "' ";
                    datos = dbe.Database.SqlQuery<VOrdenProduccionPedido>(cadena).ToList();
                }
                catch (Exception err)
                {
                    throw new Exception("Hay un error " + err.Message);
                }

                //ViewBag.req = pedido;
                //var datos = dbe.Database.SqlQuery<VPagoClie>(cadena).ToList();
                //ViewBag.datos = datos;
                try
                {
                    cadenaDet = "select * from  VBitacoraReporte where FechaPedido >= '" + FI + "' and FechaPedido <='" + FF + "' ";
                    datosDet = dbr.Database.SqlQuery<VBitacoraReporte>(cadenaDet).ToList();
                }
                catch (Exception err)
                {
                    throw new Exception("Hay un error " + err.Message);
                }
                try
                {
                    cadenaMat = "select * from  dbo.VMatAsignado where Fecha >= '" + FI + "' and Fecha <='" + FF + "' ";
                    datosMat = dbm.Database.SqlQuery<VMatAsignado>(cadenaMat).ToList();
                }
                catch (Exception err)
                {
                    throw new Exception("Hay un error " + err.Message);
                }
                //var datosDet = dbr.Database.SqlQuery<VPagoClieDoctos>(cadenaDet).ToList();
                //ViewBag.datosDet = datosDet;

                ExcelPackage Ep = new ExcelPackage();
                //Crear la hoja y poner el nombre de la pestaña del libro
                var Sheet = Ep.Workbook.Worksheets.Add("PedidosProducción");

                // en la fila1 formateo las celdas y coloco el título de la hoja
                // RichText permite agregar las propiedades de tipo de letra: color, negrita, etc.
                int row = 1;
                //Fijar la fuente para A1:Q1
                Sheet.Cells["A1:Q1"].Style.Font.Size = 20;
                Sheet.Cells["A1:Q1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:Q3"].Style.Font.Bold = true;
                Sheet.Cells["A1:Q1"].Style.Font.Color.SetColor(Color.DarkBlue);
                Sheet.Cells["A1:Q1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                Sheet.Cells["A1"].RichText.Add("Reportes de Bitacoras de Producción");

                row = 2;
                Sheet.Cells["A1:Q1"].Style.Font.Size = 12;
                Sheet.Cells["A1:Q1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:Q1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
                Sheet.Cells["A2:Q2"].Style.Font.Bold = true;
                //Subtitulo según el filtrado del periodo de datos
                row = 2;
                Sheet.Cells[string.Format("A2", row)].Value = "Fecha inicial";
                Sheet.Cells[string.Format("B2", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("B2", row)].Value = FI;
                Sheet.Cells[string.Format("D2", row)].Value = "Fecha Final";
                Sheet.Cells[string.Format("E2", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("E2", row)].Value = FF;
                Sheet.Cells[string.Format("G2", row)].Value = "Fecha Emision";
                Sheet.Cells[string.Format("H2", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("H2", row)].Value = DateTime.Now;
                //En la fila3 se da el formato a el encabezado
                row = 3;
                Sheet.Cells.Style.Font.Name = "Calibri";
                Sheet.Cells.Style.Font.Size = 10;
                Sheet.Cells["A3:Q3"].Style.Font.Bold = true;
                Sheet.Cells["A3:Q3"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                Sheet.Cells["A3:Q3"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                // Se establece el nombre que identifica a cada una de las columnas de datos.
                Sheet.Cells["A3"].RichText.Add("No. Pedido");
                Sheet.Cells["B3"].RichText.Add("Fecha Pedido");
                Sheet.Cells["C3"].RichText.Add("Cliente");
                Sheet.Cells["D3"].RichText.Add("Clave");
                Sheet.Cells["E3"].RichText.Add("Descripción");
                Sheet.Cells["F3"].RichText.Add("Presentación");
                Sheet.Cells["G3"].RichText.Add("No. Orden");
                Sheet.Cells["H3"].RichText.Add("Fecha Compromiso");
                Sheet.Cells["I3"].RichText.Add("Fecha Inicio");
                Sheet.Cells["J3"].RichText.Add("Fecha Creación Orden");
                Sheet.Cells["K3"].RichText.Add("Fecha Programada");
                Sheet.Cells["L3"].RichText.Add("Fecha Ultimo Embobinado");
                Sheet.Cells["M3"].RichText.Add("Estado de la Orden");
                Sheet.Cells["N3"].RichText.Add("Fecha Compromiso - Finalizacion");
                Sheet.Cells["O3"].RichText.Add("Fecha Pedido - Finalizacion");
                Sheet.Cells["P3"].RichText.Add("Fecha Pedido - Emision Reporte");
                Sheet.Cells["Q3"].RichText.Add("Fecha Pedido - Creación OP");


                //Aplicar borde doble al rango de celdas A3:Q3
                Sheet.Cells["A3:Q3"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;

                // en la fila4, el foreach llenara cada fila de la hoja de excel, con cada fila que trae de la bd.
                // Se establecen los formatos para las celdas: Fecha, Moneda
                row = 4;
                Sheet.Cells.Style.Font.Bold = false;
                foreach (VOrdenProduccionPedido item in datos)
                {
                    Sheet.Cells[string.Format("A{0}", row)].Value = item.IDPedido;
                    Sheet.Cells[string.Format("B{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    Sheet.Cells[string.Format("B{0}", row)].Value = item.FechaPedido;
                    Sheet.Cells[string.Format("C{0}", row)].Value = item.Cliente;
                    Sheet.Cells[string.Format("D{0}", row)].Value = item.Clave;
                    Sheet.Cells[string.Format("E{0}", row)].Value = item.Descripcion;
                    Sheet.Cells[string.Format("F{0}", row)].Value = item.Presentacion;
                    Sheet.Cells[string.Format("G{0}", row)].Value = item.IDOrden;
                    Sheet.Cells[string.Format("H{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    Sheet.Cells[string.Format("H{0}", row)].Value = item.FechaCompromiso;
                    Sheet.Cells[string.Format("I{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    Sheet.Cells[string.Format("I{0}", row)].Value = item.FechaInicio;

                    Sheet.Cells[string.Format("J{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    Sheet.Cells[string.Format("J{0}", row)].Value = item.FechaCreacion;
                    Sheet.Cells[string.Format("K{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    Sheet.Cells[string.Format("K{0}", row)].Value = item.FechaProgramada;


                    //Sheet.Cells[string.Format("M{0}", row)].Style.Numberformat.Format = "0";
                    //      Sheet.Cells[string.Format("M{0}", row)].Value = item.FechaCreacion - item.FechaCompromiso;
                    //Sheet.Cells[string.Format("M{0}", row)].Formula = "=DIAS(K{0},H{0})";

                    Bitacora ultimabitacora = new BitacoraContext().Bitacoras.Where(s => s.IDOrden == item.IDOrden).OrderBy(s => s.IDBitacora).ToList().LastOrDefault();
                    try
                    {
                        Sheet.Cells[string.Format("L{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                        Sheet.Cells[string.Format("L{0}", row)].Value = ultimabitacora.FechaFin;
                    }
                    catch (Exception err)
                    {

                        Sheet.Cells[string.Format("L{0}", row)].Value = "";
                    }


                    Sheet.Cells[string.Format("M{0}", row)].Value = item.EstadoOrden;

                    //Fecha Compromiso - Finalizacion
                    int diferencia = 0;
                    DateTime fechafin;

                    try
                    {
                        if (ultimabitacora.FechaFin.HasValue)
                        {
                            fechafin = ultimabitacora.FechaFin.GetValueOrDefault();
                        }
                        else
                        {
                            fechafin = DateTime.Now;
                        }

                        TimeSpan span = (fechafin - item.FechaCompromiso).GetValueOrDefault();
                        diferencia = span.Days;

                    }
                    catch (Exception err)
                    {

                    }

                    Sheet.Cells[string.Format("N{0}", row)].Value = diferencia;

                    //FECHA PEDIDO  VS FECHA FINALIZACION
                    int diferencia1 = 0;
                    DateTime fechafin1;
                    try
                    {
                        if (ultimabitacora.FechaFin.HasValue)
                        {
                            fechafin1 = ultimabitacora.FechaFin.GetValueOrDefault();
                        }
                        else
                        {
                            fechafin1 = DateTime.Now;
                        }

                        TimeSpan span1 = (fechafin1 - item.FechaPedido);
                        diferencia1 = span1.Days;
                    }
                    catch (Exception err)
                    {
                    }

                    Sheet.Cells[string.Format("O{0}", row)].Value = diferencia;

                    //FECHA DE  PEDIDO VS FECHA EMISION DE REPORTE
                    int diferencia2 = 0;
                    try
                    {
                        var Span2 = DateTime.Now - item.FechaPedido;
                        diferencia2 = Span2.Days;
                    }
                    catch (Exception err)
                    {

                    }
                    Sheet.Cells[string.Format("P{0}", row)].Value = diferencia1;
                    //FECHA PEDIDO VS FECHA CREACION DE OP
                    int diferencia3 = 0;
                    DateTime fechacrea;
                    try
                    {
                        if (item.FechaCreacion.HasValue)
                        {
                            fechacrea = item.FechaCreacion.GetValueOrDefault();
                        }
                        else
                        {
                            fechacrea = DateTime.Now;
                        }
                        DateTime fecPed = item.FechaPedido;
                        var Span3 = (fecPed - fechacrea);
                        diferencia3 = Span3.Days;
                    }
                    catch (Exception err)
                    {

                    }
                    Sheet.Cells[string.Format("Q{0}", row)].Value = diferencia1;
                    row++;
                }

                //Hoja No. 2
                Sheet = Ep.Workbook.Worksheets.Add("BitacoraProducción");
                row = 1;
                //Fijar la fuente para A1:Q1
                Sheet.Cells["A1:R1"].Style.Font.Size = 20;
                Sheet.Cells["A1:R1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:R3"].Style.Font.Bold = true;
                Sheet.Cells["A1:R1"].Style.Font.Color.SetColor(Color.DarkBlue);
                Sheet.Cells["A1:R1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                Sheet.Cells["A1"].RichText.Add("Reportes de Bitacoras de Producción");

                row = 2;
                Sheet.Cells["A1:R1"].Style.Font.Size = 12;
                Sheet.Cells["A1:R1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:R1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
                Sheet.Cells["A2:R2"].Style.Font.Bold = true;
                //Subtitulo según el filtrado del periodo de datos
                row = 2;
                Sheet.Cells[string.Format("A2", row)].Value = "Fecha inicial";
                Sheet.Cells[string.Format("B2", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("B2", row)].Value = FI;
                Sheet.Cells[string.Format("D2", row)].Value = "Fecha Final";
                Sheet.Cells[string.Format("E2", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("E2", row)].Value = FF;
                Sheet.Cells[string.Format("G2", row)].Value = "Fecha Emision";
                Sheet.Cells[string.Format("H2", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("H2", row)].Value = DateTime.Now;
                //En la fila3 se da el formato a el encabezado
                row = 3;
                Sheet.Cells.Style.Font.Name = "Calibri";
                Sheet.Cells.Style.Font.Size = 10;
                Sheet.Cells["A3:R3"].Style.Font.Bold = true;
                Sheet.Cells["A3:R3"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                Sheet.Cells["A3:R3"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                // Se establece el nombre que identifica a cada una de las columnas de datos.
                Sheet.Cells["A3"].RichText.Add("No. Pedido");
                Sheet.Cells["B3"].RichText.Add("FechaPedido");
                Sheet.Cells["C3"].RichText.Add("Fecha Compromiso");
                Sheet.Cells["D3"].RichText.Add("No. Orden");
                Sheet.Cells["E3"].RichText.Add("Estado de la Orden"); ;
                Sheet.Cells["F3"].RichText.Add("Trabajador");
                Sheet.Cells["G3"].RichText.Add("Proceso ");
                Sheet.Cells["H3"].RichText.Add("Fecha Jornada");
                Sheet.Cells["I3"].RichText.Add("Fecha Inicio");
                Sheet.Cells["J3"].RichText.Add("FechaFin");
                Sheet.Cells["K3"].RichText.Add("Diferencia Horas");
                Sheet.Cells["L3"].RichText.Add("Observacion");
                Sheet.Cells["M3"].RichText.Add("Cantidad");
                Sheet.Cells["N3"].RichText.Add("Unidad");
                Sheet.Cells["O3"].RichText.Add("Fecha Inicio Paro");
                Sheet.Cells["P3"].RichText.Add("Fecha Fin Paro");
                Sheet.Cells["Q3"].RichText.Add("Tiempo Falla");
                Sheet.Cells["R3"].RichText.Add("Falla");

                //Aplicar borde doble al rango de celdas A3:Q3
                Sheet.Cells["A3:R3"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;

                // en la fila4, el foreach llenara cada fila de la hoja de excel, con cada fila que trae de la bd.
                // Se establecen los formatos para las celdas: Fecha, Moneda
                row = 4;
                Sheet.Cells.Style.Font.Bold = false;
                foreach (VBitacoraReporte itemD in datosDet)
                {

                    Sheet.Cells[string.Format("A{0}", row)].Value = itemD.IDPedido;
                    Sheet.Cells[string.Format("B{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    Sheet.Cells[string.Format("B{0}", row)].Value = itemD.FechaPedido;
                    Sheet.Cells[string.Format("C{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    Sheet.Cells[string.Format("C{0}", row)].Value = itemD.FechaRequiere;
                    Sheet.Cells[string.Format("D{0}", row)].Value = itemD.IDOrden;
                    Sheet.Cells[string.Format("E{0}", row)].Value = itemD.EstadoOrden;
                    Sheet.Cells[string.Format("F{0}", row)].Value = itemD.Trabajador;
                    Sheet.Cells[string.Format("G{0}", row)].Value = itemD.NombreProceso;
                    Sheet.Cells[string.Format("H{0}", row)].Style.Numberformat.Format = "mm/dd/yyyy";
                    Sheet.Cells[string.Format("H{0}", row)].Value = itemD.FechaInicio;
                    Sheet.Cells[string.Format("I{0}", row)].Style.Numberformat.Format = "mm/dd/yyyy hh:mm:ss";
                    Sheet.Cells[string.Format("I{0}", row)].Value = itemD.FechaInicio;
                    Sheet.Cells[string.Format("J{0}", row)].Style.Numberformat.Format = "mm/dd/yyyy hh:mm:ss";
                    Sheet.Cells[string.Format("J{0}", row)].Value = itemD.FechaFin;
                    Sheet.Cells[string.Format("K{0}", row)].Style.Numberformat.Format = "hh:mm:ss";
                    Sheet.Cells[string.Format("K{0}", row)].Value = itemD.DiferenciaHorasInicioFin;
                    Sheet.Cells[string.Format("L{0}", row)].Value = itemD.Observacion;
                    Sheet.Cells[string.Format("M{0}", row)].Value = itemD.Cantidad;
                    Sheet.Cells[string.Format("N{0}", row)].Value = itemD.Unidad;
                    Sheet.Cells[string.Format("O{0}", row)].Style.Numberformat.Format = "mm/dd/yyyy hh:mm:ss";
                    Sheet.Cells[string.Format("O{0}", row)].Value = itemD.FechaInicioParo;
                    Sheet.Cells[string.Format("P{0}", row)].Style.Numberformat.Format = "mm/dd/yyyy hh:mm:ss";
                    Sheet.Cells[string.Format("P{0}", row)].Value = itemD.FechaTerminacionParo;
                    Sheet.Cells[string.Format("Q{0}", row)].Style.Numberformat.Format = "hh:mm:ss";
                    Sheet.Cells[string.Format("Q{0}", row)].Value = itemD.TiempoFalla;
                    Sheet.Cells[string.Format("R{0}", row)].Value = itemD.Falla;
                    row++;
                }


                //Hoja No. 3
                Sheet = Ep.Workbook.Worksheets.Add("Material Asignado");
                row = 1;
                //Fijar la fuente para A1:Q1
                Sheet.Cells["A1:R1"].Style.Font.Size = 20;
                Sheet.Cells["A1:R1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:R3"].Style.Font.Bold = true;
                Sheet.Cells["A1:R1"].Style.Font.Color.SetColor(Color.DarkBlue);
                Sheet.Cells["A1:R1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                Sheet.Cells["A1"].RichText.Add("Reportes de Material Asignado");

                row = 2;
                Sheet.Cells["A1:R1"].Style.Font.Size = 12;
                Sheet.Cells["A1:R1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:R1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
                Sheet.Cells["A2:R2"].Style.Font.Bold = true;
                //Subtitulo según el filtrado del periodo de datos
                row = 2;
                Sheet.Cells[string.Format("A2", row)].Value = "Fecha inicial";
                Sheet.Cells[string.Format("B2", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("B2", row)].Value = FI;
                Sheet.Cells[string.Format("D2", row)].Value = "Fecha Final";
                Sheet.Cells[string.Format("E2", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("E2", row)].Value = FF;
                Sheet.Cells[string.Format("G2", row)].Value = "Fecha Emision";
                Sheet.Cells[string.Format("H2", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("H2", row)].Value = DateTime.Now;
                //En la fila3 se da el formato a el encabezado
                row = 3;
                Sheet.Cells.Style.Font.Name = "Calibri";
                Sheet.Cells.Style.Font.Size = 10;
                Sheet.Cells["A3:R3"].Style.Font.Bold = true;
                Sheet.Cells["A3:R3"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                Sheet.Cells["A3:R3"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                // Se establece el nombre que identifica a cada una de las columnas de datos.
                Sheet.Cells["A3"].RichText.Add("No. Pedido");
                Sheet.Cells["B3"].RichText.Add("FechaPedido");
                Sheet.Cells["C3"].RichText.Add("No. Orden");
                Sheet.Cells["D3"].RichText.Add("Clave");
                Sheet.Cells["E3"].RichText.Add("Material"); ;
                Sheet.Cells["F3"].RichText.Add("Ancho");
                Sheet.Cells["G3"].RichText.Add("Largo");
                Sheet.Cells["H3"].RichText.Add("M2 Asignados");
                Sheet.Cells["I3"].RichText.Add("M2 Entregados");
                Sheet.Cells["J3"].RichText.Add("ML Asignados");
                Sheet.Cells["K3"].RichText.Add("ML Entregados");
                Sheet.Cells["L3"].RichText.Add("Almacen");

                //Aplicar borde doble al rango de celdas A3:Q3
                Sheet.Cells["A3:L3"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;

                // en la fila4, el foreach llenara cada fila de la hoja de excel, con cada fila que trae de la bd.
                // Se establecen los formatos para las celdas: Fecha, Moneda
                row = 4;
                Sheet.Cells.Style.Font.Bold = false;
                foreach (VMatAsignado itemD in datosMat)
                {
                    Sheet.Cells[string.Format("A{0}", row)].Value = itemD.NoPedido;
                    Sheet.Cells[string.Format("B{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    Sheet.Cells[string.Format("B{0}", row)].Value = itemD.Fecha;
                    Sheet.Cells[string.Format("C{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    Sheet.Cells[string.Format("C{0}", row)].Value = itemD.Orden;
                    Sheet.Cells[string.Format("D{0}", row)].Value = itemD.Cref;
                    Sheet.Cells[string.Format("E{0}", row)].Value = itemD.Descripcion;
                    Sheet.Cells[string.Format("F{0}", row)].Value = itemD.ancho;
                    Sheet.Cells[string.Format("G{0}", row)].Value = itemD.largo;
                    Sheet.Cells[string.Format("H{0}", row)].Value = itemD.M2Asignados;
                    Sheet.Cells[string.Format("I{0}", row)].Value = itemD.M2Entregados;
                    Sheet.Cells[string.Format("J{0}", row)].Value = itemD.MLAsignados;
                    Sheet.Cells[string.Format("K{0}", row)].Value = itemD.MLEntregado;
                    Sheet.Cells[string.Format("L{0}", row)].Value = itemD.Almacen;
                    row++;
                }

                //Se genera el archivo y se descarga

                Sheet.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment: filename=" + "Pedido.xlsx");
                Response.BinaryWrite(Ep.GetAsByteArray());
                Response.End();
                return Redirect("/blah");
            }
            return Redirect("index");
        }

        public JsonResult GetNombreTrabajador(int id) //checar la unidad del articulo
        {

            using (TrabajadorContext db = new TrabajadorContext())
            {
                string nombre = string.Empty;
                try
                {
                    nombre = db.Trabajadores.Find(id).Nombre;
                    return Json(new { result = nombre }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception err)
                {
                    string mensaje = err.Message;
                    return Json(new { result = "" }, JsonRequestBehavior.AllowGet);
                }


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




    }


    public class PRepositoryMaquina
    {
        public IEnumerable<SelectListItem> GetMaquinaPorProcesor(int IDTrabajador)
        {
            List<SelectListItem> lista;
            if (IDTrabajador == 0)
            {
                lista = new List<SelectListItem>();
                lista.Add(new SelectListItem() { Value = "0", Text = "Elige una máquina" });
                return (lista);
            }
            using (var context = new TrabajadorContext())
            {
                string cadenasql = "select m.IDMaquinaProceso, m.IDArticulo, m.IDProceso, A.Descripcion as Maquina, P.Nombreproceso from Articulo as A inner join maquinaproceso as m  on A.IDArticulo=m.IDArticulo  inner join Proceso as p on p.idproceso=m.idproceso inner join TrabajadorProceso as tp on p.idproceso=tp.idproceso  where tp.idtrabajador=" + IDTrabajador + " order by a.descripcion";
                lista = context.Database.SqlQuery<ProcesoM>(cadenasql).ToList()
                    .OrderBy(n => n.IDArticulo)
                        .Select(n =>
                        new SelectListItem
                        {
                            Value = n.IDArticulo.ToString(),
                            Text = n.Maquina
                        }).ToList();
                var countrytip = new SelectListItem()
                {
                    Value = null,
                    Text = "--- Selecciona una Máquina ---"
                };
                lista.Insert(0, countrytip);
                return new SelectList(lista, "Value", "Text");
            }

        }

        public IEnumerable<SelectListItem> GetOrdenPorMaquina(int IDMaquina)
        {
            List<SelectListItem> lista;
            if (IDMaquina == 0)
            {
                lista = new List<SelectListItem>();
                lista.Add(new SelectListItem() { Value = "0", Text = "Elige una Máquina" });
                return (lista);
            }
            using (var context = new TrabajadorContext())
            {
                string cadenasql = "select distinct(o.IDOrden) as IDOrden, a.prioridad,c.Nombre as NombreCliente from Ordenproduccion as o inner join prioridades  as a on o.idOrden=a.idorden  inner join Clientes as c on c.idcliente=o.idcliente where  (a.estado='Programado' or a.estado='Iniciada') and  a.idmaquina=" + IDMaquina + "order by a.prioridad;";
                lista = context.Database.SqlQuery<OrdenM>(cadenasql).ToList()

                         .Select(n =>
                        new SelectListItem
                        {
                            Value = n.IDOrden.ToString(),
                            Text = n.IDOrden.ToString() + " | " + n.NombreCliente
                        }).ToList();
                //lista.Add(new SelectListItem() { Value = "0", Text = "Elige una Máquina" });
                var countrytip = new SelectListItem()
                {
                    Value = null,
                    Text = "--- Selecciona una Orden ---"
                };
                lista.Insert(0, countrytip);
                return new SelectList(lista, "Value", "Text");
            }

        }

    }
    public class ProcesoM
    {
        [Key]
        public int IDMaquinaProceso { get; set; }

        public int IDArticulo { get; set; }
        //public virtual Articulo Art { get; set; }

        public int IDProceso { get; set; }
        //public virtual Proceso Proceso { get; set; }

        public string Maquina { get; set; }
        public string Nombreproceso { get; set; }

    }
    public class OrdenM
    {

        public int IDOrden { get; set; }
        public string NombreCliente { get; set; }
        //public virtual Articulo Art { get; set; }


    }

}