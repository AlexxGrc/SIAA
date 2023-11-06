using OfficeOpenXml;
using OfficeOpenXml.Style;
using PagedList;
using SIAAPI.ClasesProduccion;
using SIAAPI.Controllers.Cfdi;
using SIAAPI.Models.Administracion;
using SIAAPI.Models.Comercial;
using SIAAPI.Models.Diseno;
using SIAAPI.ViewModels.Comercial;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Serialization;

namespace SIAAPI.Controllers.Diseno
{
    [Authorize(Roles = "Administrador,Gerencia, Comercial,Sistemas, Almacenista, Ventas,Diseno, GerenteVentas")]

    public class DisenoController : Controller
    {

        private SolicitudDisenoContext db = new SolicitudDisenoContext();
        // GET: Almacen
        public ActionResult Index(string sortOrder, string currentFilter, string NoSolicitud, int? page, int? PageSize, string etiqueta, string cyrel, string Tipo, string Fechainicio, string Fechafinal, string Estado)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.ASortParm = String.IsNullOrEmpty(sortOrder) ? "NR" : "NR";
            ViewBag.BSortParm = String.IsNullOrEmpty(sortOrder) ? "Cliente" : "Cliente";
            ViewBag.CSortParm = String.IsNullOrEmpty(sortOrder) ? "Vendedor" : "Vendedor";

            int pageNumber = (page ?? 1);
            int pageSize = (PageSize ?? 10);

            //var listaestados = new EstadosolicitudRepository().GetEstados();
            //ViewBag.lista = listaestados;

            // Not sure here
            if (NoSolicitud == null)
            {
                NoSolicitud = currentFilter;
            }

            // Pass filtering string to view in order to maintain filtering when paging
            ViewBag.NoSolicitud = NoSolicitud;
            //Paginación
            string Filtro = " where id>0";
            if (!String.IsNullOrEmpty(NoSolicitud))
            {
                Filtro += " and id=" + NoSolicitud + " ";
            }


            if (!String.IsNullOrEmpty(Fechainicio) && String.IsNullOrEmpty(Fechafinal)) //pusieron una fecha
            {
                if (Filtro == string.Empty)
                {
                    Filtro = " where  Fecha BETWEEN '" + Fechainicio + " 00:00:00.1' and '" + Fechainicio + " 23:59:59.999' ";
                }
                else
                {
                    Filtro += " and Fecha BETWEEN '" + Fechainicio + " 00:00:00.1' and '" + Fechainicio + " 23:59:59.999'";
                }
            }


            if (!String.IsNullOrEmpty(Fechainicio) && !String.IsNullOrEmpty(Fechafinal)) //pusieron una fecha
            {
                if (Filtro == string.Empty)
                {
                    Filtro = "where  Fecha BETWEEN '" + Fechainicio + " 00:00:00.1' and '" + Fechafinal + " 23:59:59.999' ";
                }
                else
                {
                    Filtro += " and Fecha BETWEEN '" + Fechainicio + " 00:00:00.1' and '" + Fechafinal + " 23:59:59.999'";
                }

            }

            if (!String.IsNullOrEmpty(Estado) ) //pusieron una fecha
            {
                if (Filtro == string.Empty)
                {
                   // Filtro = "where  Fecha BETWEEN '" + Fechainicio + " 00:00:00.1' and '" + Fechafinal + " 23:59:59.999' ";
                }
                else
                {
                    Filtro += " and EstadodeSolicitud= '" + Estado + "' ";
                }

            }

            ViewBag.cyrel = string.Empty;
            if (!String.IsNullOrEmpty(cyrel)) //pusieron una fecha
            {
                if (Filtro == string.Empty)
                {
                    // Filtro = "where  Fecha BETWEEN '" + Fechainicio + " 00:00:00.1' and '" + Fechafinal + " 23:59:59.999' ";
                }
                else
                {
                    Filtro += " and Numerodecirel = '" + cyrel + "' ";
                    ViewBag.cyrel = cyrel;
                }

            }



            List<SolicitudDiseno> elementos = new SolicitudDisenoContext().Database.SqlQuery<SolicitudDiseno>("select  * from [dbo].[SolicitudDiseno] "+Filtro+" order by id desc").ToList() ;
            //Busqueda
          
                switch (sortOrder)
                {
                    case "ID":
                        sortOrder = " order by s.ID ";
                        break;
                    case "Descripcion":
                        sortOrder = " order by c.Descripcion ";
                        break;
                    
                    default:
                        sortOrder = " order by s.ID desc ";
                        break;
                }

              
                // Populate DropDownList
                        ViewBag.PageSize = new List<SelectListItem>()
                    {
                        new SelectListItem { Text = "10", Value = "10"  },
                        new SelectListItem { Text = "25", Value = "25" ,Selected = true},
                        new SelectListItem { Text = "50", Value = "50" },
                        new SelectListItem { Text = "100", Value = "100" },
                       
                     };

                 ViewBag.TipodeSolicitud = new List<SelectListItem>()
                    {
                        new SelectListItem { Text = "", Value = ""  },
                        new SelectListItem { Text = "Diseño", Value = "Diseño" },
                        new SelectListItem { Text = "Suaje", Value = "Suaje" },
                       

                     };

            ViewBag.Estado = new List<SelectListItem>()
                    {
                        new SelectListItem { Text = "", Value = ""  },
                        new SelectListItem { Text = "PENDIENTE", Value = "PENDIENTE" },
                        new SelectListItem { Text = "TERMINADO", Value = "TEMINADO" },
                        new SelectListItem { Text = "AUTORIZADA", Value = "AUTORIZADA" },
                        new SelectListItem { Text = "CANCELADO", Value = "CANCELADO" },

                     };

            pageNumber = (page ?? 1);
                 pageSize = (PageSize ?? 10);
                ViewBag.psize = pageSize;
                ViewBag.etiqueta = etiqueta;
            ViewBag.PageNumber = pageNumber;
            ViewBag.IDCotizacion = NoSolicitud;
            ViewBag.FechaF = Fechafinal;
            ViewBag.FechaI = Fechainicio;
                return View(elementos.ToPagedList(pageNumber, pageSize));


            }

            //Ordenacion
        

        // GET: SolicitudDiseno/Create
        public ActionResult Create(int IDCotizacion, decimal Monto)
        {
            SolicitudDiseno solicitud = new SolicitudDiseno();
            solicitud.IDCotizacion = IDCotizacion;
            solicitud.EstadodeSolicitud = "PENDIENTE";
            solicitud.NumeroRevision = 1;
            string fecha = DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day;
            solicitud.Fecha = DateTime.Now;
            solicitud.FechaCompromiso = Convert.ToString(DateTime.Now).Replace("/", "-");
            solicitud.TipoEtiqueta = "FLEXOGRAFICA";
            solicitud.TipodeSolicitud = "Diseño";
            ViewBag.IDVendedor = new SelectList(db.Vendedores, "IDVendedor", "Nombre");
            var listaetiquetas = new TipoEtiquetaRepository().GetTipos(2);
            ViewBag.lista = listaetiquetas;
            ViewBag.cotizacion = IDCotizacion;
            solicitud.consumomensual = Monto;
            //ViewBag.TipoEtiqueta = new TipoEtiquetaRepository();
            ViewBag.IDCliente = new SelectList(db.Clientes.OrderBy(s=> s.Nombre), "IDCliente", "Nombre");

            var Embobinado = new List<SelectListItem>();
            Embobinado.Add(new SelectListItem { Text = "A", Value = "A" });
            Embobinado.Add(new SelectListItem { Text = "B", Value = "B" });
            Embobinado.Add(new SelectListItem { Text = "C", Value = "C" });
            Embobinado.Add(new SelectListItem { Text = "D", Value = "D" });
            Embobinado.Add(new SelectListItem { Text = "E", Value = "E" });
            Embobinado.Add(new SelectListItem { Text = "F", Value = "F" });
            Embobinado.Add(new SelectListItem { Text = "G", Value = "G" });
            Embobinado.Add(new SelectListItem { Text = "H", Value = "H" });
            Embobinado.Add(new SelectListItem { Text = "I", Value = "I" });
            Embobinado.Add(new SelectListItem { Text = "J", Value = "J" });
            Embobinado.Add(new SelectListItem { Text = "K", Value = "K" });
            Embobinado.Add(new SelectListItem { Text = "N/A", Value = "N/A", Selected= true });
            ViewBag.Embobinado = new SelectList(Embobinado, "Value", "Text");



            ViewData["Embobinado"] = Embobinado;



            return View(solicitud);
        }

        // POST: SolicitudDiseno/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SolicitudDiseno elemento, FormCollection coleccion)
        {
            string numRev = coleccion.Get("NumeroRevision");
            string IDCotizacion = coleccion.Get("IDCotizacion");
            string Fecha = coleccion.Get("Fecha");
            string FechaCompromiso = coleccion.Get("FechaCompromiso");
            string Cliente = coleccion.Get("IDCliente");
            string IDVendedor = coleccion.Get("IDVendedor");
            string TipodeSolicitud = coleccion.Get("TipodeSolicitud");
            string TipoEtiqueta = coleccion.Get("TipoEtiqueta");
            string EstadodeSolicitud = coleccion.Get("EstadodeSolicitud");
            string NumerodeCirel = coleccion.Get("NumerodeCirel");
            string consumomensual = coleccion.Get("consumomensual");
            string MontoAnticipo = coleccion.Get("MontoAnticipo");
            string Observacion = coleccion.Get("Observaciones");
            string Embobinado = coleccion.Get("Embobinado");



           
            DateTime fe = Convert.ToDateTime(Fecha);
            DateTime fe1 = DateTime.Now;
            string fechabien = Convert.ToString(fe).Replace("/","-");
            string fechabienC = Convert.ToString(fe1).Replace("/", "-");
            //int NumeroRevision = Convert.ToInt16(numRev);
            //editar estado

            try
            {
                string NombreCliente = "";
                int idcliente = int.Parse(Cliente);
                try
                {
                    Clientes cliente = new ClientesContext().Clientes.Find(idcliente);
                    NombreCliente = cliente.Nombre;
                }
                catch (Exception err)
                {

                }

                string cadena = "INSERT INTO [dbo].[SolicitudDiseno]([NumeroRevision],[IDCotizacion],[Fecha],[FechaCompromiso],[Cliente],[IDVendedor],[TipodeSolicitud],[TipoEtiqueta],TipodeDiseno,[EstadodeSolicitud],[NumerodeCirel],[consumomensual],[MontoAnticipo],[Observaciones], FechaContrato,FechaEntGrabado,FechaAutorizacion,FechaTerminacion, Embobinado)VALUES(" + Convert.ToInt16(numRev) +
                    "," + Convert.ToInt16(IDCotizacion) + ",sysdatetime(),'" + fechabienC + "','"+ NombreCliente + "'," + Convert.ToInt16(IDVendedor) +",'"+ TipodeSolicitud + "','"+TipoEtiqueta+ "','Suaje Existente','"+EstadodeSolicitud+"',"+Convert.ToInt16(NumerodeCirel)+","+Convert.ToDecimal(consumomensual)+","+Convert.ToDecimal(MontoAnticipo)+",'"+Observacion+"','','','','','"+Embobinado+"');";
                var db = new SolicitudDisenoContext();
                db.Database.ExecuteSqlCommand(cadena);
                //db.SolicitudDiseno.Add(elemento);
                //db.SaveChanges();


                List<SolicitudDiseno> numero;
                numero = db.Database.SqlQuery<SolicitudDiseno>("SELECT * FROM [dbo].[SolicitudDiseno] WHERE ID = (SELECT MAX(ID) from SolicitudDiseno)").ToList();
                int IDSolicitud = numero.Select(s => s.ID).FirstOrDefault();

                try
                {
                    //guardar cotización en carpeta
                    ClsCotizador elementoC;

                    Cotizaciones archivocotizacion = new ArchivoCotizadorContext().Database.SqlQuery<Cotizaciones>("select*from Cotizaciones where id="+IDCotizacion).FirstOrDefault();
                    XmlSerializer serializerX = new XmlSerializer(typeof(ClsCotizador));
                    try
                    {
                        XmlDocument documento = new XmlDocument();
                        string nombredearchivo = archivocotizacion.Ruta.TrimEnd() + "\\" + archivocotizacion.NombreArchivo + ".xml";
                        documento.Load(nombredearchivo);


                        using (Stream reader = new FileStream(nombredearchivo, FileMode.Open))
                        {
                            // Call the Deserialize method to restore the object's state.
                            elementoC = (ClsCotizador)serializerX.Deserialize(reader);
                        }
                    }
                    catch (Exception er)
                    {
                        string mensajedeerror = er.Message;
                        elementoC = (ClsCotizador)serializerX.Deserialize(StringExtensiones.ToXmlReader(archivocotizacion.contenido));
                    }

                    string NombredeArchivo = "Solicitud-"+ IDSolicitud;

                    string nombredecarpeta = DateTime.Now.Month + "" + DateTime.Now.Year + " " + User.Identity.Name;

                    if (!Directory.Exists(System.Web.HttpContext.Current.Server.MapPath("~/DisenoCotizacion")))
                    {
                        Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath("~/DisenoCotizacion"));
                    }
                    if (!Directory.Exists(System.Web.HttpContext.Current.Server.MapPath("~/DisenoCotizacion/SolicitudDiseno")))
                    {
                        Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath("~/DisenoCotizacion/SolicitudDiseno"));
                    }
                    //if (!Directory.Exists(System.Web.HttpContext.Current.Server.MapPath("~/DisenoCotizacion/" + nombredecarpeta)))
                    //{
                    //    Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath("~/DisenoCotizacion/" + nombredecarpeta));
                    //}


                    nombredecarpeta = System.Web.HttpContext.Current.Server.MapPath("~/DisenoCotizacion/SolicitudDiseno");

                    this.GrabarArchivoCotizador(elementoC, NombredeArchivo, nombredecarpeta);



                }
                catch (Exception err)
                {

                }
                return RedirectToAction("Index");
            }
            catch(Exception err)
            {
                ViewBag.IDCliente = new SelectList(db.Clientes.OrderBy(s => s.Nombre), "IDCliente", "Nombre");

                var listaetiquetas = new TipoEtiquetaRepository().GetTipos(2);
                ViewBag.lista = listaetiquetas;
                ViewBag.cotizacion = IDCotizacion;
                ViewBag.IDVendedor = new SelectList(db.Vendedores, "IDVendedor", "Nombre");
                var Embobinadoc = new List<SelectListItem>();
                Embobinadoc.Add(new SelectListItem { Text = "A", Value = "A" });
                Embobinadoc.Add(new SelectListItem { Text = "B", Value = "B" });
                Embobinadoc.Add(new SelectListItem { Text = "C", Value = "C" });
                Embobinadoc.Add(new SelectListItem { Text = "D", Value = "D" });
                Embobinadoc.Add(new SelectListItem { Text = "E", Value = "E" });
                Embobinadoc.Add(new SelectListItem { Text = "F", Value = "F" });
                Embobinadoc.Add(new SelectListItem { Text = "G", Value = "G" });
                Embobinadoc.Add(new SelectListItem { Text = "H", Value = "H" });
                Embobinadoc.Add(new SelectListItem { Text = "I", Value = "I" });
                Embobinadoc.Add(new SelectListItem { Text = "J", Value = "J" });
                Embobinadoc.Add(new SelectListItem { Text = "K", Value = "K" });
                Embobinadoc.Add(new SelectListItem { Text = "N/A", Value = "N/A", Selected = true });
                ViewBag.Embobinado = new SelectList(Embobinadoc, "Value", "Text");



                ViewData["Embobinado"] = Embobinadoc;
                return View();
            }
        }

        public void GrabarArchivoCotizador(ClsCotizador elemento, string _nombredearchivo, string _ruta)
        {

            

            StringWriter stringwriter = new StringWriter();
            XmlSerializer x = new XmlSerializer(elemento.GetType());
            x.Serialize(stringwriter, elemento);
            string mensaje = stringwriter.ToString();

           

            Cotizaciones archivo = new Cotizaciones();


            string nombredearchivoagrabar = _ruta + "/" + _nombredearchivo + ".xml";

            string xmlstring = stringwriter.ToString().Replace("encoding=\"utf-16\"", "");


            EscribeArchivoXML(xmlstring, nombredearchivoagrabar, true);



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


        public ActionResult CreatePleca(int IDCotizacion, decimal Monto)
        {
            SolicitudDiseno solicitud = new SolicitudDiseno();
            solicitud.IDCotizacion = IDCotizacion;
            solicitud.EstadodeSolicitud = "PENDIENTE";
            solicitud.NumeroRevision = 1;
            string fecha = DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day;
            solicitud.Fecha = DateTime.Now;
            solicitud.FechaCompromiso = Convert.ToString(DateTime.Now).Replace("/", "-");
            solicitud.TipoEtiqueta = "FLEXOGRAFICA";
            solicitud.TipodeSolicitud = "Pleca";
            ViewBag.IDVendedor = new SelectList(db.Vendedores, "IDVendedor", "Nombre");
            var listaetiquetas = new TipoEtiquetaRepository().GetTipos(2);
            ViewBag.lista = listaetiquetas;
            ViewBag.cotizacion = IDCotizacion;
            solicitud.consumomensual = Monto;
            //ViewBag.TipoEtiqueta = new TipoEtiquetaRepository();
            ViewBag.IDCliente = new SelectList(db.Clientes.OrderBy(s => s.Nombre), "IDCliente", "Nombre");

            return View(solicitud);
        }

        // POST: SolicitudDiseno/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePleca(SolicitudDiseno elemento, FormCollection coleccion)
        {
            string numRev = coleccion.Get("NumeroRevision");
            string IDCotizacion = coleccion.Get("IDCotizacion");
            string Fecha = coleccion.Get("Fecha");
            string FechaCompromiso = coleccion.Get("FechaCompromiso");
            string Cliente = coleccion.Get("IDCliente");
            string IDVendedor = coleccion.Get("IDVendedor");
            string TipodeSolicitud = coleccion.Get("TipodeSolicitud");
            string TipoEtiqueta = coleccion.Get("TipoEtiqueta");
            string EstadodeSolicitud = coleccion.Get("EstadodeSolicitud");
            string NumerodeCirel = coleccion.Get("NumerodeCirel");
            string consumomensual = coleccion.Get("consumomensual");
            string MontoAnticipo = coleccion.Get("MontoAnticipo");
            string Observacion = coleccion.Get("Observaciones");

            DateTime fe = Convert.ToDateTime(Fecha);
            DateTime fe1 = DateTime.Now;
            string fechabien = Convert.ToString(fe).Replace("/", "-");
            string fechabienC = Convert.ToString(fe1).Replace("/", "-");
            //int NumeroRevision = Convert.ToInt16(numRev);
            //editar estado

            try
            {
                string NombreCliente = "";
                int idcliente = int.Parse(Cliente);
                try
                {
                    Clientes cliente = new ClientesContext().Clientes.Find(idcliente);
                    NombreCliente = cliente.Nombre;
                }
                catch (Exception err)
                {

                }

                string cadena = "INSERT INTO [dbo].[SolicitudDiseno]([NumeroRevision],[IDCotizacion],[Fecha],[FechaCompromiso],[Cliente],[IDVendedor],[TipodeSolicitud],[TipoEtiqueta],TipodeDiseno,[EstadodeSolicitud],[NumerodeCirel],[consumomensual],[MontoAnticipo],[Observaciones], FechaContrato,FechaEntGrabado,FechaAutorizacion,FechaTerminacion)VALUES(" + Convert.ToInt16(numRev) +
                    "," + Convert.ToInt16(IDCotizacion) + ",sysdatetime(),'" + fechabienC + "','" + NombreCliente + "'," + Convert.ToInt16(IDVendedor) + ",'" + TipodeSolicitud + "','" + TipoEtiqueta + "','Suaje Existente','" + EstadodeSolicitud + "'," + Convert.ToInt16(NumerodeCirel) + "," + Convert.ToDecimal(consumomensual) + "," + Convert.ToDecimal(MontoAnticipo) + ",'" + Observacion + "','','','','');";
                var db = new SolicitudDisenoContext();
                db.Database.ExecuteSqlCommand(cadena);
                //db.SolicitudDiseno.Add(elemento);
                //db.SaveChanges();

                List<SolicitudDiseno> numero;
                numero = db.Database.SqlQuery<SolicitudDiseno>("SELECT * FROM [dbo].[SolicitudDiseno] WHERE ID = (SELECT MAX(ID) from SolicitudDiseno)").ToList();
                int IDSolicitud = numero.Select(s => s.ID).FirstOrDefault();

                try
                {
                    //guardar cotización en carpeta
                    ClsCotizador elementoC;

                    Cotizaciones archivocotizacion = new ArchivoCotizadorContext().Database.SqlQuery<Cotizaciones>("select*from Cotizaciones where id=" + IDCotizacion).FirstOrDefault();
                    XmlSerializer serializerX = new XmlSerializer(typeof(ClsCotizador));
                    try
                    {
                        XmlDocument documento = new XmlDocument();
                        string nombredearchivo = archivocotizacion.Ruta.TrimEnd() + "\\" + archivocotizacion.NombreArchivo + ".xml";
                        documento.Load(nombredearchivo);


                        using (Stream reader = new FileStream(nombredearchivo, FileMode.Open))
                        {
                            // Call the Deserialize method to restore the object's state.
                            elementoC = (ClsCotizador)serializerX.Deserialize(reader);
                        }
                    }
                    catch (Exception er)
                    {
                        string mensajedeerror = er.Message;
                        elementoC = (ClsCotizador)serializerX.Deserialize(StringExtensiones.ToXmlReader(archivocotizacion.contenido));
                    }

                    string NombredeArchivo = "Solicitud-" + IDSolicitud;

                    string nombredecarpeta = DateTime.Now.Month + "" + DateTime.Now.Year + " " + User.Identity.Name;

                    if (!Directory.Exists(System.Web.HttpContext.Current.Server.MapPath("~/DisenoCotizacion")))
                    {
                        Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath("~/DisenoCotizacion"));
                    }
                    if (!Directory.Exists(System.Web.HttpContext.Current.Server.MapPath("~/DisenoCotizacion/SolicitudDiseno")))
                    {
                        Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath("~/DisenoCotizacion/SolicitudDiseno"));
                    }
                    //if (!Directory.Exists(System.Web.HttpContext.Current.Server.MapPath("~/DisenoCotizacion/" + nombredecarpeta)))
                    //{
                    //    Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath("~/DisenoCotizacion/" + nombredecarpeta));
                    //}


                    nombredecarpeta = System.Web.HttpContext.Current.Server.MapPath("~/DisenoCotizacion/SolicitudDiseno");

                    this.GrabarArchivoCotizador(elementoC, NombredeArchivo, nombredecarpeta);



                }
                catch (Exception err)
                {

                }
                return RedirectToAction("Index");
            }
            catch (Exception err)
            {
                ViewBag.IDCliente = new SelectList(db.Clientes.OrderBy(s => s.Nombre), "IDCliente", "Nombre");

                var listaetiquetas = new TipoEtiquetaRepository().GetTipos(2);
                ViewBag.lista = listaetiquetas;
                ViewBag.cotizacion = IDCotizacion;
                ViewBag.IDVendedor = new SelectList(db.Vendedores, "IDVendedor", "Nombre");
                return View();
            }
        }

        // GET: SolicitudDiseno/Edit/5
        public ActionResult Edit(int id)
        {
            var elemento = new SolicitudDisenoContext().SolicitudDiseno.Find(id);
            var listaestados = new EstadosolicitudRepository().GetEstados();
            ViewBag.lista = listaestados;
            //ViewBag.IDCliente = new SelectList(db.Clientes, "IDCliente", "Nombre",elemento.IDCliente);
            ViewBag.TipoEtiqueta = new TipoEtiquetaRepository().GetTipoSeleccionado(elemento.TipoEtiqueta);
            ViewBag.IDVendedor = new SelectList(db.Vendedores, "IDVendedor", "Nombre", elemento.IDVendedor);
           

            return View(elemento);
        }

        // POST: SolicitudDiseno/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
                var elemento = db.SolicitudDiseno.Single(m => m.ID == id);
                if (TryUpdateModel(elemento))
                {
                    if (elemento.EstadodeSolicitud== "TERMINADO")
                    {
                        elemento.FechaTerminacion = Convert.ToString(DateTime.Now).Replace("/","-");

                    }
                    
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                ViewBag.IDCliente = new SelectList(db.Clientes, "IDCliente", "Nombre");
                ViewBag.IDVendedor = new SelectList(db.Vendedores, "IDVendedor", "Nombre");
                return View();
            }
            catch (Exception er)
            {
                ViewBag.IDCliente = new SelectList(db.Clientes, "IDCliente", "Nombre");
                ViewBag.IDVendedor = new SelectList(db.Vendedores, "IDVendedor", "Nombre");
                return View();
            }
        }

        public ActionResult Details(int id)
        {
            var elemento = db.SolicitudDiseno.Single(m => m.ID == id);
            if (elemento == null)
            {
                return NotFound();
            }
            return View(elemento);
        }
        private ActionResult NotFound()
        {
            throw new NotImplementedException();
        }

        public ActionResult CotizadorRapido1(int? Id, int idsuaje = 0, int IDDisenoS=0)
        {
              ArchivoCotizadorContext db = new ArchivoCotizadorContext();
        ClsCotizador elemento = new ClsCotizador();
            var coti = db.cotizaciones.Single(m => m.ID == Id);
            ViewBag.fecha = coti.Fecha;

            if (elemento.TCcotizado == 0)

            {
                elemento.TCcotizado = SIAAPI.Properties.Settings.Default.TCcotizador;
            }
            try
            {
                if (Id == null || Id == 0)
                {
                    ViewBag.IDCotizacion = 0;
                    elemento.IDMonedapreciosconvenidos = new c_MonedaContext().c_Monedas.Where(s => s.ClaveMoneda == "USD").ToList().FirstOrDefault().IDMoneda;
                    elemento.CostoM2Cinta = SIAAPI.Properties.Settings.Default.CostoM2default;
                    elemento.TCcotizado = SIAAPI.Properties.Settings.Default.TCcotizador;
                    elemento.DiluirSuajeEnPedidos = 50;
                    elemento.Yatienematriz = false;
                    elemento.CobrarMaster = false;
                    ViewBag.Mensajedeerror = "";
                    if (idsuaje > 0)
                    {

                        elemento = this.llenaelemento(elemento, idsuaje);
                    }
                    var suajes = new Repository().GetSuajes(idsuaje);
                    ViewBag.IDSuaje = suajes;
                    ViewBag.nombresuaje = idsuaje;

                    var suajes2 = new Repository().GetPlecas();
                    ViewBag.IDSuaje2 = suajes2;


                    var cintas = new Repository().GetCintas();
                    ViewBag.IDMaterial = cintas;
                    ViewBag.Mensajedeerror = "Estamos iniciando la cotización";
                    ViewBag.IDMAterial2 = cintas;
                }
                else   // viene de cargar el archivo de la cotizacion

                {



                    //ClsCotizador elementoDI;
                    XmlSerializer serializerX = new XmlSerializer(typeof(ClsCotizador));
                    try
                    {
                        XmlDocument documentoDI = new XmlDocument();
                        string nombredearchivoDI = System.Web.HttpContext.Current.Server.MapPath("~/DisenoCotizacion/SolicitudDiseno/Solicitud-" + IDDisenoS + ".xml");
                      
                        documentoDI.Load(nombredearchivoDI);


                        using (Stream reader = new FileStream(nombredearchivoDI, FileMode.Open))
                        {
                            // Call the Deserialize method to restore the object's state.
                            elemento = (ClsCotizador)serializerX.Deserialize(reader);
                        }
                    }
                    catch (Exception er)
                    {
                        Cotizaciones archivocotizacion = new ArchivoCotizadorContext().cotizaciones.Find(Id);

                        try
                        {
                             XmlDocument documento = new XmlDocument();
                            string nombredearchivo = archivocotizacion.Ruta.TrimEnd() + "\\" + archivocotizacion.NombreArchivo + ".xml";
                            documento.Load(nombredearchivo);
                            elemento = null;
                            XmlSerializer serializer = new XmlSerializer(typeof(ClsCotizador));
                            using (Stream reader = new FileStream(nombredearchivo, FileMode.Open))
                            {
                                // Call the Deserialize method to restore the object's state.
                                elemento = (ClsCotizador)serializer.Deserialize(reader);
                            }
                        }
                        catch (Exception ERR)
                        {
                            string mensajedeerror = er.Message;
                            elemento = (ClsCotizador)serializerX.Deserialize(StringExtensiones.ToXmlReader(archivocotizacion.contenido));
                        }
                        

                    }




                    ViewBag.Mensajedeerror = "";


                   

                    //string tsuaje = colecciondeelementos.Get("Tsuaje");
                    //elemento.TipoSuaje = tsuaje.Replace(",", "");

                    //string tsuajeFig = colecciondeelementos.Get("TSuajeFi");
                    //elemento.TipoSuajeFigura = tsuajeFig.Replace(",", "");

                    //string tsuajeCorte = colecciondeelementos.Get("TSuajeCorte");
                    //elemento.TipoCorte = tsuajeCorte.Replace(",", "");



                    var TipoSuaje = new List<SelectListItem>();
                    TipoSuaje.Add(new SelectListItem { Text = "---Seleccionar---", Value = "S/N" });
                    TipoSuaje.Add(new SelectListItem { Text = "Macizo", Value = "M" });
                    TipoSuaje.Add(new SelectListItem { Text = "Flexible", Value = "F" });
                    ViewBag.TSuaje = new SelectList(TipoSuaje, "Value", "Text", elemento.TipoSuaje);

                    var TipoFigura = new List<SelectListItem>();
                    TipoFigura.Add(new SelectListItem { Text = "---Seleccionar---", Value = "S/N" });
                    TipoFigura.Add(new SelectListItem { Text = "Circulo", Value = "C" });
                    TipoFigura.Add(new SelectListItem { Text = "Rectangulo", Value = "R" });
                    TipoFigura.Add(new SelectListItem { Text = "Triangulo", Value = "T" });
                    TipoFigura.Add(new SelectListItem { Text = "Cuadrado", Value = "Cu" });

                    ViewBag.TSuajeFi = new SelectList(TipoFigura, "Value", "Text", elemento.TipoSuajeFigura);



                    var TipoCorte = new List<SelectListItem>();
                    TipoCorte.Add(new SelectListItem { Text = "---Seleccionar---", Value = "S/N" });
                    TipoCorte.Add(new SelectListItem { Text = "Medio Corte", Value = "M" });
                    TipoCorte.Add(new SelectListItem { Text = "Corte Completo", Value = "CC" });
                    TipoCorte.Add(new SelectListItem { Text = "Corte a Liner", Value = "CL" });
                    TipoCorte.Add(new SelectListItem { Text = "Pleca", Value = "P" });

                    ViewBag.TSuajeCorte = new SelectList(TipoCorte, "Value", "Text", elemento.TipoCorte);

                    var EsquinasSuaje = new List<SelectListItem>();
                    EsquinasSuaje.Add(new SelectListItem { Text = "N/A", Value = "N/A" });
                    EsquinasSuaje.Add(new SelectListItem { Text = "1/2 in", Value = "1/2 in" });
                    EsquinasSuaje.Add(new SelectListItem { Text = "1/4 in", Value = "1/4 in" });
                    EsquinasSuaje.Add(new SelectListItem { Text = "1/8 in", Value = "1/8 in" });
                    EsquinasSuaje.Add(new SelectListItem { Text = "1/16 in", Value = "1/16 in" });
                    EsquinasSuaje.Add(new SelectListItem { Text = "1/32 in", Value = "1/32 in" });
                    EsquinasSuaje.Add(new SelectListItem { Text = "1/64 in", Value = "1/64 in" });
                    EsquinasSuaje.Add(new SelectListItem { Text = "3/4 in", Value = "3/4 in" });
                    EsquinasSuaje.Add(new SelectListItem { Text = "3/8 in", Value = "3/8 in" });
                    EsquinasSuaje.Add(new SelectListItem { Text = "3/16 in", Value = "3/16 in" });
                    EsquinasSuaje.Add(new SelectListItem { Text = "3/64 in", Value = "3/64 in" });
                    EsquinasSuaje.Add(new SelectListItem { Text = "3/32 in", Value = "3/32 in" });
                    EsquinasSuaje.Add(new SelectListItem { Text = "5/8 in", Value = "5/8 in" });
                    EsquinasSuaje.Add(new SelectListItem { Text = "5/64 in", Value = "5/64 in" });
                    EsquinasSuaje.Add(new SelectListItem { Text = "7/16 in", Value = "7/16 in" });
                    EsquinasSuaje.Add(new SelectListItem { Text = "7/32 in", Value = "7/32 in" });
                    EsquinasSuaje.Add(new SelectListItem { Text = "7/64 in", Value = "7/64 in" });
                    EsquinasSuaje.Add(new SelectListItem { Text = "9/16 in", Value = "9/16 in" });
                    EsquinasSuaje.Add(new SelectListItem { Text = "9/32 in", Value = "9/32 in" });
                    EsquinasSuaje.Add(new SelectListItem { Text = "9/64 in", Value = "9/64 in" });
                    EsquinasSuaje.Add(new SelectListItem { Text = "11/16 in", Value = "11/16 in" });
                    EsquinasSuaje.Add(new SelectListItem { Text = "11/32 in", Value = "11/32 in" });
                    EsquinasSuaje.Add(new SelectListItem { Text = "11/64 in", Value = "11/64 in" });

                    ViewBag.EsquinasSuaje = new SelectList(EsquinasSuaje, "Value", "Text", elemento.Esquinas);

                    if (elemento.IDMonedapreciosconvenidos == 0)

                    {
                        elemento.IDMonedapreciosconvenidos = new c_MonedaContext().c_Monedas.Where(s => s.ClaveMoneda == "USD").ToList().FirstOrDefault().IDMoneda;
                    }
                    if (elemento.TCcotizado == 0)

                    {
                        elemento.TCcotizado = SIAAPI.Properties.Settings.Default.TCcotizador;
                    }



                    ViewData["Tintas"] = null;
                    try
                    {

                        ViewData["Tintas"] = elemento.Tintas;
                        ViewBag.tintas = elemento.Tintas.Count;
                        ViewBag.tintasselec = elemento.Tintas;
                    }
                    catch (Exception err)
                    {
                        string mensaje = err.Message;
                    }

                    ViewBag.Rango1 = elemento.Rango1;
                    ViewBag.Rango2 = elemento.Rango2;
                    ViewBag.Rango3 = elemento.Rango3;
                    ViewBag.Rango4 = elemento.Rango4;

                    elemento.IDCotizacion = int.Parse((Id == null ? 0 : Id).ToString());

                    ViewBag.IDCotizacion = elemento.IDCotizacion;
                    var suajes = new Repository().GetSuajes(elemento.IDSuaje);

                    ViewBag.IDSuaje = suajes;
                    ViewBag.nombresuaje = elemento.IDSuaje;
                    if (elemento.IDSuaje2 == 0)
                    {
                        var suajes2 = new Repository().GetPlecas();
                        ViewBag.IDSuaje2 = suajes2;
                    }
                    else
                    {
                        var suajes2 = new Repository().GetPlecas(elemento.IDSuaje2);
                        ViewBag.IDSuaje2 = suajes2;
                    }



                    ViewBag.suajeseleccionado = elemento.IDSuaje;

                    var cintas = new Repository().GetCintasbyClave(elemento.IDMaterial);
                    ViewBag.IDMaterial = cintas;

                    var cintas2 = new Repository().GetCintasbyClave(elemento.IDMaterial2);
                    ViewBag.IDMaterial2 = cintas2;

                }
            }
            catch (Exception err)
            {

            }
            


            return View(elemento);
        }

        public ClsCotizador llenaelemento(ClsCotizador elemento, int IDSuaje)
        {
            SIAAPI.Models.Comercial.Caracteristica cara = new SIAAPI.Models.Comercial.ArticuloContext().Database.SqlQuery<SIAAPI.Models.Comercial.Caracteristica>("Select * from Caracteristica where ID=" + IDSuaje).ToList().FirstOrDefault();
            SuajeCaracteristicas suajec = new SuajeCaracteristicas();
            FormulaSiaapi.Formulas formula = new FormulaSiaapi.Formulas();
            suajec.Eje = 0;

            try
            {
                suajec.Eje = decimal.Parse(formula.getvalor("EJE", cara.Presentacion).ToString());
            }
            catch (Exception err)
            {
                string mensajederror = err.Message;
                suajec.Eje = 0;
            }

            elemento.anchoproductomm = (int)suajec.Eje;
            suajec.Avance = 0;
            try
            {
                suajec.Avance = decimal.Parse(formula.getvalor("AVANCE", cara.Presentacion).ToString());
            }
            catch (Exception err)
            {
                string mensajederror = err.Message;
                suajec.Avance = 0;
            }
            elemento.largoproductomm = (int)suajec.Avance;



            suajec.CavidadAvance = 2;

            try
            {
                suajec.CavidadAvance = int.Parse(formula.getvalor("REP AVANCE", cara.Presentacion).ToString());


            }
            catch (Exception err)
            {
                string mensajederror = err.Message;
                suajec.CavidadAvance = 2;
            }

            elemento.cavidadesdesuajeAvance = suajec.CavidadAvance;


            suajec.CavidadEje = 2;

            try
            {
                suajec.CavidadEje = int.Parse(formula.getvalor("REP EJE", cara.Presentacion).ToString());


            }
            catch (Exception err)
            {
                string mensajederror = err.Message;
                suajec.CavidadEje = 2;
            }

            elemento.cavidadesdesuajeEje = suajec.CavidadEje;


            suajec.Gapeje = 0;
            try
            {
                suajec.Gapeje = decimal.Parse(formula.getvalor("GAP EJE", cara.Presentacion).ToString());
                if (suajec.Gapeje == 0M)
                {
                    suajec.Gapeje = decimal.Parse(formula.getvalor("GAP EJE ", cara.Presentacion).ToString());

                }
            }
            catch (Exception err)
            {
                string mensajederror = err.Message;
                suajec.Gapeje = 0;
            }


            elemento.gapeje = suajec.Gapeje;

            suajec.Gapavance = 3;
            try
            {
                suajec.Gapavance = decimal.Parse(formula.getvalor("GAP AVANCE", cara.Presentacion).ToString());

            }
            catch (Exception err)
            {
                string mensajederror = err.Message;
                suajec.Gapavance = 2;
            }


            elemento.gapavance = suajec.Gapavance;

            suajec.RepAvance = 0;
            try
            {
                suajec.RepAvance = int.Parse(formula.getvalor("REP AVANCE", cara.Presentacion).ToString());


            }
            catch (Exception err)
            {
                string mensajederror = err.Message;
                suajec.RepAvance = 0;
            }

            suajec.Corte = "";
            try
            {
                suajec.Corte = formula.getValorCadena("CORTE", cara.Presentacion).ToString();

            }
            catch (Exception err)
            {
                string mensajederror = err.Message;
                suajec.Corte = "";
            }
            suajec.Material = "";
            try
            {
                suajec.Material = formula.getValorCadena("MATERIAL", cara.Presentacion).ToString();

            }
            catch (Exception err)
            {
                string mensajederror = err.Message;
                suajec.Material = "";
            }


            suajec.Material = "";
            try
            {
                suajec.Material = formula.getValorCadena("MATERIAL", cara.Presentacion).ToString();

            }
            catch (Exception err)
            {
                string mensajederror = err.Message;
                suajec.Material = "";
            }





            suajec.TH = 0;


            //elemento.TH = suajec.TH;

            if (suajec.TH == 0)
            {

                try
                {
                    suajec.TH = int.Parse(formula.getValorCadena("TH", cara.Presentacion).ToString());

                }
                catch (Exception err)
                {
                    string mensajederror = err.Message;
                    suajec.TH = 0;
                }
            }
            if (suajec.TH == 0)
            {
                try
                {
                    suajec.Alma = formula.getValorCadena("ALMA", cara.Presentacion).ToString();

                    string alma = suajec.Alma.Replace(" TH", "");

                    suajec.TH = int.Parse(alma);

                }
                catch (Exception err)
                {
                    string mensajederror = err.Message;
                    suajec.TH = 0;
                }


            }
            if (suajec.TH == 0)
            {
                try
                {
                    suajec.TH = int.Parse(formula.getvalor("DIENTES_TH", cara.Presentacion).ToString());

                }
                catch (Exception err)
                {
                    string mensajederror = err.Message;
                    suajec.TH = 0;
                }
            }
            elemento.TH = suajec.TH;












            return elemento;
        }




        public ActionResult CreateDisenoSuaje(int IDCotizacion, decimal Monto)
        {

            SolicitudDiseno solicitud = new SolicitudDiseno();
            solicitud.IDCotizacion = IDCotizacion;
            solicitud.EstadodeSolicitud = "PENDIENTE";
            solicitud.NumeroRevision = 1;
            string fecha = DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day;
            solicitud.Fecha = DateTime.Now;
            solicitud.FechaCompromiso = Convert.ToString(DateTime.Now).Replace("/", "-");
            solicitud.TipoEtiqueta = "FLEXOGRAFICA";
            solicitud.TipodeSolicitud = "Suaje";
            ViewBag.IDVendedor = new SelectList(db.Vendedores, "IDVendedor", "Nombre");
            var listaetiquetas = new TipoEtiquetaRepository().GetTipos(1);
            ViewBag.lista = listaetiquetas;
            solicitud.consumomensual = Monto;

            //ViewBag.TipoEtiqueta = new TipoEtiquetaRepository();
            ViewBag.IDCliente = new SelectList(db.Clientes, "IDCliente", "Nombre");

            return View(solicitud);
        }

        // POST: SolicitudDiseno/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateDisenoSuaje(SolicitudDiseno elemento, FormCollection coleccion)
        {
            string numRev = coleccion.Get("NumeroRevision");
            string IDCotizacion = coleccion.Get("IDCotizacion");
            string Fecha = coleccion.Get("Fecha");
            string FechaCompromiso = coleccion.Get("FechaCompromiso");
            string Cliente = coleccion.Get("Cliente");
            string IDVendedor = coleccion.Get("IDVendedor");
            //string TipodeSolicitud = coleccion.Get("TipodeSolicitud");
            string TipoEtiqueta = coleccion.Get("TipoEtiqueta");
            string EstadodeSolicitud = coleccion.Get("EstadodeSolicitud");
            string NumerodeCirel = coleccion.Get("NumerodeCirel");
            string consumomensual = coleccion.Get("consumomensual");
            string MontoAnticipo = coleccion.Get("MontoAnticipo");
            string Observacion = coleccion.Get("Observaciones");

            DateTime fe = Convert.ToDateTime(Fecha);
            DateTime fe1 = DateTime.Now;
            string fechabien = Convert.ToString(fe).Replace("/", "-");
            string fechabienC = Convert.ToString(fe1).Replace("/", "-");
            //int NumeroRevision = Convert.ToInt16(numRev);
            //editar estado

            try
            {
                string cadena = "INSERT INTO [dbo].[SolicitudDiseno]([NumeroRevision],[IDCotizacion],[Fecha],[FechaCompromiso],[Cliente],[IDVendedor],[TipodeSolicitud],[TipoEtiqueta],TipodeDiseno,[EstadodeSolicitud],[NumerodeCirel],[consumomensual],[MontoAnticipo],[Observaciones])VALUES(" + Convert.ToInt16(numRev) +
                    "," + Convert.ToInt16(IDCotizacion) + ",sysdatetime(),'" + fechabienC + "','" +Cliente + "'," + Convert.ToInt16(IDVendedor) + ",'Suaje','" + TipoEtiqueta + "','Nuevo Suaje','" + EstadodeSolicitud + "'," + Convert.ToInt16(NumerodeCirel) + "," + Convert.ToDecimal(consumomensual) + "," + Convert.ToDecimal(MontoAnticipo) + ",'" + Observacion + "');";
                var db = new SolicitudDisenoContext();
                db.Database.ExecuteSqlCommand(cadena);
                //db.SolicitudDiseno.Add(elemento);
                //db.SaveChanges();

                List<SolicitudDiseno> numero;
                numero = db.Database.SqlQuery<SolicitudDiseno>("SELECT * FROM [dbo].[SolicitudDiseno] WHERE ID = (SELECT MAX(ID) from SolicitudDiseno)").ToList();
                int IDSolicitud = numero.Select(s => s.ID).FirstOrDefault();

                try
                {
                    //guardar cotización en carpeta
                    ClsCotizador elementoC;

                    Cotizaciones archivocotizacion = new ArchivoCotizadorContext().Database.SqlQuery<Cotizaciones>("select*from Cotizaciones where id=" + IDCotizacion).FirstOrDefault();
                    XmlSerializer serializerX = new XmlSerializer(typeof(ClsCotizador));
                    try
                    {
                        XmlDocument documento = new XmlDocument();
                        string nombredearchivo = archivocotizacion.Ruta.TrimEnd() + "\\" + archivocotizacion.NombreArchivo + ".xml";
                        documento.Load(nombredearchivo);


                        using (Stream reader = new FileStream(nombredearchivo, FileMode.Open))
                        {
                            // Call the Deserialize method to restore the object's state.
                            elementoC = (ClsCotizador)serializerX.Deserialize(reader);
                        }
                    }
                    catch (Exception er)
                    {
                        string mensajedeerror = er.Message;
                        elementoC = (ClsCotizador)serializerX.Deserialize(StringExtensiones.ToXmlReader(archivocotizacion.contenido));
                    }

                    string NombredeArchivo = "Solicitud-" + IDSolicitud;

                    string nombredecarpeta = DateTime.Now.Month + "" + DateTime.Now.Year + " " + User.Identity.Name;

                    if (!Directory.Exists(System.Web.HttpContext.Current.Server.MapPath("~/DisenoCotizacion")))
                    {
                        Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath("~/DisenoCotizacion"));
                    }
                    if (!Directory.Exists(System.Web.HttpContext.Current.Server.MapPath("~/DisenoCotizacion/SolicitudDiseno")))
                    {
                        Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath("~/DisenoCotizacion/SolicitudDiseno"));
                    }
                    //if (!Directory.Exists(System.Web.HttpContext.Current.Server.MapPath("~/DisenoCotizacion/" + nombredecarpeta)))
                    //{
                    //    Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath("~/DisenoCotizacion/" + nombredecarpeta));
                    //}


                    nombredecarpeta = System.Web.HttpContext.Current.Server.MapPath("~/DisenoCotizacion/SolicitudDiseno");

                    this.GrabarArchivoCotizador(elementoC, NombredeArchivo, nombredecarpeta);



                }
                catch (Exception err)
                {

                }
                return RedirectToAction("Index");
            }
            catch (Exception err)
            {
                ViewBag.IDCliente = new SelectList(db.Clientes, "IDCliente", "Nombre");
                var listaetiquetas = new TipoEtiquetaRepository().GetTipos(1);
                ViewBag.lista = listaetiquetas;
                //ViewBag.TipoEtiqueta = new TipoEtiquetaRepository().GetTipoSeleccionado("FLEXOGRAFICA");
                ViewBag.IDVendedor = new SelectList(db.Vendedores, "IDVendedor", "Nombre");
                return View();
            }
        }


        public ActionResult Autorizar(int? id, string sortOrder, string NoSolicitud, string Fechafinal, string Fechainicio, int page, string currentFilter, string searchString, int pageSize, string etiqueta)
        {
            string fechaA = Convert.ToString(DateTime.Now).Replace("/", "-"); ;

           db.Database.ExecuteSqlCommand("update [dbo].[SolicitudDiseno] set FechaAutorizacion='"+fechaA+"' , [EstadodeSolicitud]='AUTORIZADA' where [id]='" + id + "'");
           return RedirectToAction("Index", new {  sortOrder= sortOrder,  currentFilter=currentFilter, page=page, searchString= currentFilter, NoSolicitud=NoSolicitud, Fechainicio=Fechainicio, Fechafinal=Fechafinal, etiqueta = etiqueta });
        }
        public ActionResult NoAutorizar(int? id, string sortOrder, string NoSolicitud, string Fechafinal, string Fechainicio, int page, string currentFilter, string searchString, int pageSize, string etiqueta)
        {
            db.Database.ExecuteSqlCommand("update [dbo].[SolicitudDiseno] set [EstadodeSolicitud]='NO AUTORIZADA' where [id]='" + id + "'");
            return RedirectToAction("Index", new { sortOrder = sortOrder, currentFilter = currentFilter, page = page, searchString = currentFilter, NoSolicitud = NoSolicitud, Fechainicio = Fechainicio, Fechafinal = Fechafinal, etiqueta = etiqueta });

        }



        public ActionResult SubirArchivoDis(int id)
        {
            ViewBag.ID = id;
            return View();
        }

        [HttpPost]
        public ActionResult SubirArchivoDis(HttpPostedFileBase file, int id)
        {
            int idP = int.Parse(id.ToString());
            SubirArchivosModelo modelo = new SubirArchivosModelo();
            if (file != null)
            {
                string ruta = Server.MapPath("~/PDFDisenoAdd/");
                ruta += "D_" + id + "_" + file.FileName;
                string cad = "insert into  dbo.DisenoAdd ([IDDiseno], [RutaArchivo], nombreArchivo) Values (" + idP + ", '" + ruta + "','" + "D_" + id + "_" + file.FileName + "' )";
                new DisenoAddContext().Database.ExecuteSqlCommand(cad);
                modelo.SubirArchivo(ruta, file);
                ViewBag.Error = modelo.error;
                ViewBag.Correcto = modelo.Confirmacion;
            }
            return RedirectToAction("index", new { searchString = id });
        }

        public ActionResult DescargarPDFDis(int id)
        {
            // Obtener contenido del archivo
            DisenoAddContext dbp = new DisenoAddContext();
            DisenoAdd elemento = dbp.DisenoAdd.Find(id);
            string contentType = System.Net.Mime.MediaTypeNames.Application.Pdf;
            if (elemento.RutaArchivo.ToString().Substring(elemento.RutaArchivo.ToString().Length-3,3).ToUpper()=="PDF")
            {
               contentType = System.Net.Mime.MediaTypeNames.Application.Pdf;
            }
            if (elemento.RutaArchivo.ToString().Substring(elemento.RutaArchivo.ToString().Length - 3, 3).ToUpper() == "JPG")
            {
                contentType= "image/jpeg";
            }

            return new FilePathResult(elemento.RutaArchivo.ToString(), contentType);
        }

        public ActionResult EliminarArchivoDis(int id)
        {
            DisenoAddContext dbp = new DisenoAddContext();
            string cadena = "select * from dbo.DisenoAdd where IDDiseno= " + id + "";
            var datos = dbp.Database.SqlQuery<DisenoAdd>(cadena).ToList();
            ViewBag.datos = datos;
            return View(datos);
        }


        public ActionResult EliminarArchivo(int id, DisenoAdd mod)
        {

            DisenoAddContext db = new DisenoAddContext();
            List<SelectListItem> docto = new List<SelectListItem>();
            ClsDatoEntero contard = db.Database.SqlQuery<ClsDatoEntero>("select count(ID) as dato from dbo.DisenoAdd where ID= " + id + "").ToList().FirstOrDefault();
            int registro = 0;
            if (contard.Dato != 0)
            {
                var elemento = db.DisenoAdd.Single(m => m.ID == id);
                registro = elemento.IDDiseno;

                string cad = "delete from dbo.DisenoAdd where ID= " + elemento.ID + "";
                new DisenoAddContext().Database.ExecuteSqlCommand(cad);

            }

            return RedirectToAction("index", new { searchString = registro });
        }


        public ActionResult VSolicitudDiseno()
        {
            EFecha elemento = new EFecha();

            return View(elemento);
        }

        [HttpPost]
        public ActionResult VSolicitudDiseno(EFecha modelo, FormCollection coleccion)
        {
            VSolicitudDisenoContext dbr = new VSolicitudDisenoContext();
            string FI = modelo.fechaini.Year.ToString() + "-" + modelo.fechaini.Month.ToString() + "-" + modelo.fechaini.Day.ToString();
            string FF = modelo.fechafin.Year.ToString() + "-" + modelo.fechafin.Month.ToString() + "-" + modelo.fechafin.Day.ToString();

            string cual = coleccion.Get("Enviar");
            //string FI = modelo.fechaini.ToString("M-d-yyyy");
            //string FF = modelo.fechafin.ToString("M-d-yyyy");

            string cadena = "";
            if (cual == "Generar reporte")
            {
                return View();
            }
            if (cual == "Generar excel")
            {

                cadena = "select  * from VSolicitudDiseno where   Fecha >= '" + FI + " 12:00:00' and Fecha <='" + FF + " 23:59:59' order by ID";
                //cadena = "select * from VSolicitudDiseno where fecha >= '" + FI + " 12:00:00' and fecha<='" + FF + " 23:59:59'";
                var datos = dbr.Database.SqlQuery<VSolicitudDiseno>(cadena).ToList();
                ViewBag.datos = datos;
                ExcelPackage Ep = new ExcelPackage();
                //Crear la hoja y poner el nombre de la pestaña del libro
                var Sheet = Ep.Workbook.Worksheets.Add("Solicitud de Diseño");

                // en la fila1 formateo las celdas y coloco el título de la hoja
                // RichText permite agregar las propiedades de tipo de letra: color, negrita, etc.
                int row = 1;
                //Fijar la fuente para A1:Q1
                Sheet.Cells["A1:V1"].Style.Font.Size = 20;
                Sheet.Cells["A1:V1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:V3"].Style.Font.Bold = true;
                Sheet.Cells["A1:V1"].Style.Font.Color.SetColor(Color.DarkBlue);
                Sheet.Cells["A1:V1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                Sheet.Cells["A1"].RichText.Add("Listado de las solicitudes de diseño");

                row = 2;
                Sheet.Cells["A1:V1"].Style.Font.Size = 12;
                Sheet.Cells["A1:V1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:V1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
                Sheet.Cells["A2:V2"].Style.Font.Bold = true;
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
                Sheet.Cells["A3:V3"].Style.Font.Bold = true;
                Sheet.Cells["A3:V3"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                Sheet.Cells["A3:V3"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                // Se establece el nombre que identifica a cada una de las columnas de datos.
                Sheet.Cells["A3"].RichText.Add("No.");
                Sheet.Cells["B3"].RichText.Add("No. Cotización");
                Sheet.Cells["C3"].RichText.Add("No. Revisión");
                Sheet.Cells["D3"].RichText.Add("Fecha (Mes-Día-Año)");
                Sheet.Cells["E3"].RichText.Add("Fecha Compromiso");
                Sheet.Cells["F3"].RichText.Add("Fecha Contrato");
                Sheet.Cells["G3"].RichText.Add("Cliente");
                Sheet.Cells["H3"].RichText.Add("Vendedor");
                Sheet.Cells["I3"].RichText.Add("Oficina");
                Sheet.Cells["J3"].RichText.Add("Tipo de Solicitud");
                Sheet.Cells["K3"].RichText.Add("Tipo de Etiqueta");
                Sheet.Cells["L3"].RichText.Add("Tipo de Diseño");
                Sheet.Cells["M3"].RichText.Add("Fecha Entrega Grabado");
                Sheet.Cells["N3"].RichText.Add("Folio de Grabado");
                Sheet.Cells["O3"].RichText.Add("No. Grabados");
                Sheet.Cells["P3"].RichText.Add("Cm2");
                Sheet.Cells["Q3"].RichText.Add("Numero de Repeticiones");
                Sheet.Cells["R3"].RichText.Add("Numero de Cirel");
                Sheet.Cells["S3"].RichText.Add("Estado de Solicitud");
                Sheet.Cells["T3"].RichText.Add("Consumo Mensual");
                Sheet.Cells["U3"].RichText.Add("Monto Anticipo");
                Sheet.Cells["V3"].RichText.Add("Observaciones");
                //Aplicar borde doble al rango de celdas A3:Q3
                Sheet.Cells["A3:V3"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;
                row = 4;
                Sheet.Cells.Style.Font.Bold = false;
                foreach (VSolicitudDiseno item in ViewBag.datos)
                {
                    
                    Sheet.Cells[string.Format("A{0}", row)].Value = item.ID;
                    Sheet.Cells[string.Format("B{0}", row)].Value = item.IDCotizacion;
                    Sheet.Cells[string.Format("C{0}", row)].Value = item.NumeroRevision;
                    Sheet.Cells[string.Format("D{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    Sheet.Cells[string.Format("D{0}", row)].Value = item.Fecha;
                    Sheet.Cells[string.Format("E{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    Sheet.Cells[string.Format("E{0}", row)].Value = item.FechaCompromiso;
                    Sheet.Cells[string.Format("F{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    Sheet.Cells[string.Format("F{0}", row)].Value = item.FechaContrato;
                    Sheet.Cells[string.Format("G{0}", row)].Value = item.Cliente;
                    Sheet.Cells[string.Format("H{0}", row)].Value = item.Vendedor;
                    Sheet.Cells[string.Format("I{0}", row)].Value = item.NombreOficina;
                    Sheet.Cells[string.Format("J{0}", row)].Value = item.TipodeSolicitud;
                    Sheet.Cells[string.Format("K{0}", row)].Value = item.TipoEtiqueta;
                    Sheet.Cells[string.Format("L{0}", row)].Value = item.TipodeDiseno;
                    Sheet.Cells[string.Format("M{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    Sheet.Cells[string.Format("M{0}", row)].Value = item.FechaEntGrabado;
                    Sheet.Cells[string.Format("N{0}", row)].Value = item.FoliodeGrabado;
                    Sheet.Cells[string.Format("O{0}", row)].Value = item.NumerodeGrabados;
                    Sheet.Cells[string.Format("p{0}", row)].Value = item.Cm2;
                    Sheet.Cells[string.Format("Q{0}", row)].Value = item.NumeroRepeticiones;
                    Sheet.Cells[string.Format("R{0}", row)].Value = item.NumerodeCirel;
                    Sheet.Cells[string.Format("S{0}", row)].Value = item.EstadodeSolicitud;
                    Sheet.Cells[string.Format("T{0}", row)].Value = item.consumomensual;
                    Sheet.Cells[string.Format("U{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("U{0}", row)].Value = item.MontoAnticipo;
                    Sheet.Cells[string.Format("V{0}", row)].Value = item.Observaciones;

                    row++;
                }
                //Se genera el archivo y se descarga

                Sheet.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment: filename=" + "SolicitudDiseno.xlsx");
                Response.BinaryWrite(Ep.GetAsByteArray());
                Response.End();
                return Redirect("/blah");
            }
            return Redirect("index");
        }


    }
}