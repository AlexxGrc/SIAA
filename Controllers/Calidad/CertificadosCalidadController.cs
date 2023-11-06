using PagedList;
using SIAAPI.Models.Calidad;
using SIAAPI.Models.Comercial;
using SIAAPI.Models.Login;
using SIAAPI.Models.Produccion;
using SIAAPI.Reportes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using static SIAAPI.Controllers.Comercial.EncOrdenCompraController;

namespace SIAAPI.Controllers.Calidad
{
    public class CertificadosCalidadController : Controller
    {
        // GET: Certificados
        CertificadoCalidadContext db = new CertificadoCalidadContext();
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.DescripcionSortParm = String.IsNullOrEmpty(sortOrder) ? "Descripcion" : "Descripcion";
            // Not sure here
            if (searchString == null)
            {
                searchString = currentFilter;
            }

            // Pass filtering string to view in order to maintain filtering when paging
            ViewBag.SearchString = searchString;
            //Paginación
            var elementos = from s in db.CertificadosCalidad.OrderByDescending(s=> s.IDCertificado)
                            select s;
            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                elementos = elementos.Where(s => s.Descripcion.Contains(searchString) || s.Lote.Contains(searchString) || s.Clave.Contains(searchString));
            }

            //Ordenacion

            switch (sortOrder)
            {
                case "Descripcion":
                    elementos = elementos.OrderByDescending(s => s.IDCertificado);
                    break;
                default:
                    elementos = elementos.OrderByDescending(s => s.IDCertificado);
                    break;
            }

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = db.CertificadosCalidad.OrderByDescending(e => e.IDCertificado).Count(); // Total number of elements
            
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

        public ActionResult getJsonCaracteristicaArticulo(int id)
        {
            var presentacion = new ListaPresentacion().GetCaracteristicaPorArticuloconidpresentacion(id);
            return Json(presentacion, JsonRequestBehavior.AllowGet); ;

        }

        public IEnumerable<SelectListItem> getPresentacionPorArticulo(int ida)
        {
            var presentacion = new ArticuloRepository().GetCaracteristicaPorArticulo(ida);
            return presentacion;

        }

        public ActionResult Calidad(int IDArticulo, int IDCaracteristica, int IDCliente, int IDMuestreo, int IDInspeccion, decimal Cantidad)
        {
            ArticuloContext dbar = new ArticuloContext();


            //Articulos
            var datosArticulo = dbar.Articulo.OrderBy(i => i.Cref).ToList();
            List<SelectListItem> liAC = new List<SelectListItem>();
            liAC.Add(new SelectListItem { Text = "--Selecciona un Articulo--", Value = "0" });
            foreach (var a in datosArticulo)
            {
                if (a.IDArticulo == IDArticulo)
                {
                    liAC.Add(new SelectListItem { Text = a.Cref + " | " + a.Descripcion, Value = a.IDArticulo.ToString() });

                }
                liAC.Add(new SelectListItem { Text = a.Cref + " | " + a.Descripcion, Value = a.IDArticulo.ToString() });

            }
            ViewBag.IDArticulo = liAC;
            ViewBag.PresentacionList = getPresentacionPorArticulo(0);
            CalidadContext db = new CalidadContext();
            ViewBag.IDMuestreo = new SelectList(db.Muestreos, "IDMuestreo", "Descripcion");
            ViewBag.IDInspeccion = new SelectList(db.Inspecciones, "IDInspeccion", "Descripcion");






            return View();
        }
        public JsonResult getarticulosblando(string buscar)
        {
            buscar = buscar.Remove(buscar.Length - 1);
            var Articulos = new ArticuloContext().Articulo.Where(i=> i.Cref.Contains(buscar) && (i.IDFamilia == 10 || i.IDFamilia == 27 || i.IDFamilia == 62 || i.IDFamilia == 63)).OrderBy(S => S.Cref);

            // Populate DropDownList
            List<SelectListItem> opciones = new List<SelectListItem>();

            foreach (Articulo art in Articulos)
            {
                SelectListItem elemento = new SelectListItem();
                elemento.Text = art.Cref + " " + art.Descripcion;
                elemento.Value = art.IDArticulo.ToString();
                opciones.Add(elemento);
            }

            return Json(opciones, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Create(string Mensaje = "")
        {
            ArticuloContext dbar = new ArticuloContext();
            if (Mensaje != "")
            {
                ViewBag.Mensaje = Mensaje;
            }
            else
            {
                ViewBag.Mensaje = "";
            }

            //Articulos
            var datosArticulo = dbar.Articulo.Where(i => i.IDFamilia == 10 || i.IDFamilia == 27 || i.IDFamilia == 62 || i.IDFamilia == 63).OrderBy(i => i.Cref).ToList();
            List<SelectListItem> liAC = new List<SelectListItem>();
            liAC.Add(new SelectListItem { Text = "--Selecciona un Articulo--", Value = "0" });
            foreach (var a in datosArticulo)
            {
                liAC.Add(new SelectListItem { Text = a.Cref + " | " + a.Descripcion, Value = a.IDArticulo.ToString() });

            }
            ViewBag.IDArticulo = liAC;
            ViewBag.PresentacionList = getPresentacionPorArticulo(0);
            CalidadContext db = new CalidadContext();
            ViewBag.IDMuestreo = new SelectList(db.Muestreos, "IDMuestreo", "Descripcion");
            ViewBag.IDInspeccion = new SelectList(db.Inspecciones, "IDInspeccion", "Descripcion");
            ViewBag.IDCliente = new SelectList(new ClientesContext().Clientes.OrderBy(s => s.Nombre), "IDCliente", "Nombre");





            return View();
        }

        public ActionResult PresentacionArticulo(int IDCertificado)
        {
            var elementos = new CertificadoCalidadContext().CertificadosCalidad.ToList().Where(S => S.IDCertificado == IDCertificado);// en realidad es detalle de la recepcion

            CertificadoCalidad cer = new CertificadoCalidadContext().CertificadosCalidad.Find(IDCertificado);
            Caracteristica carateristica = new WorkinProcessContext().Database.SqlQuery<Caracteristica>("select * from caracteristica where id=" + cer.IDCaracteristica).ToList().FirstOrDefault();
            ViewBag.Presentacion = carateristica.Presentacion;
            ViewBag.IDCertificado = IDCertificado;

            return View();
        }
        [HttpPost]
        public ActionResult PresentacionArticulo(FormCollection coleccion, CertificadoCalidad certificado)
        {
            string idc = coleccion.Get("IDCertificado");
            int IDCertificado = int.Parse(idc);
            CertificadoCalidad cer = new CertificadoCalidadContext().CertificadosCalidad.Find(IDCertificado);

            int contador = 0;
            string[] arraydatos;
            arraydatos = cer.Presentacion.Split(',');
            int cuantos = arraydatos.Length;
            string metodo = "";
            string resultado = "";
            string parametro = "";
            string PresentacionResultado = "";
            string PresentacionMetodoResultado = "";
            string PresentacionFinalResultado = "";
            string PresentacionFinalMetodoResultado = "";

            int vueltas = cuantos - 1;
            metodo = coleccion.Get("IDMetodo" + contador);
            resultado = coleccion.Get("Resultado" + contador);
            parametro = coleccion.Get("Parametro" + contador);

            for (int i = 0; i < arraydatos.Count(); i++)
            {
                string concatenacion = "";
                string concatenacionMetodo = "";

                string[] arraydatosMetodo;
                arraydatosMetodo = metodo.Split(',');
                string[] arraydatosParametro;
                arraydatosParametro = parametro.Split(',');
                string[] arraydatosResultado;
                arraydatosResultado = resultado.Split(',');



                if (vueltas == contador)
                {
                    concatenacion = arraydatosParametro[contador] + ":" + arraydatosResultado[contador];
                    concatenacionMetodo = arraydatosParametro[contador] + ":" + arraydatosMetodo[contador];
                }
                else
                {
                    concatenacion = arraydatosParametro[contador] + ":" + arraydatosResultado[contador] + ",";
                    concatenacionMetodo = arraydatosParametro[contador] + ":" + arraydatosMetodo[contador] + ",";
                }

                PresentacionMetodoResultado += concatenacionMetodo;
                PresentacionResultado += concatenacion;

                contador++;
            }
            PresentacionFinalResultado = PresentacionResultado;
            PresentacionFinalMetodoResultado = PresentacionMetodoResultado;

            try
            {
                db.Database.ExecuteSqlCommand("update CertificadoCalidad set PresentacionResultado='" +PresentacionFinalResultado + "', MetodoResultadoPresentacion='"+ PresentacionFinalMetodoResultado + "' where IDCertificado= " + IDCertificado + "");

                return RedirectToAction("Index");
            }


            catch (Exception ERR)
            {
                return PartialView();
            }

        }

        public JsonResult JsonArticulo(int id)
        {

            Articulo articulo = new ArticuloContext().Articulo.Find(id);
            if (articulo != null)
            {


                datosprov nuevo = new datosprov();
                nuevo.IDMuestreo = articulo.IDMuestreo;
                nuevo.IDInspeccion = articulo.IDInspeccion;


            



                //////////////aqui el codigo actualizando el precio segun el prov////////////////

                //  new CarritoPContext().Database.ExecuteSqlCommand("update dbo.CarritoRequisicion set costo=1 where  userID=" + USERID);

                return Json(nuevo, JsonRequestBehavior.AllowGet);
            }

            else
            {
                var errorModel = new { error = "No se encontro" };
                return new JsonHttpStatusResult(errorModel, HttpStatusCode.InternalServerError);
            }
        }

        public JsonResult JsonCantidad(int id, decimal cantidad)
        {
            Articulo articulo = new ArticuloContext().Articulo.Find(id);
            try
            {
                decimal Rev = 0;
                string Letra = "";
                var CalidadLetras = new CalidadLetraContext().CalidadLetras;
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

                datosC nuevo = new datosC();
                nuevo.LetraR = Letra;
                nuevo.Revisar = Rev;

                return Json(nuevo, JsonRequestBehavior.AllowGet);

            }
            catch (Exception err)
            {
                var errorModel = new { error = "No se encontro" };
                return new JsonHttpStatusResult(errorModel, HttpStatusCode.InternalServerError);
            }



        }

        public JsonResult JsonCaracteristica(int idP)
        {

            Caracteristica carateristica = new WorkinProcessContext().Database.SqlQuery<Caracteristica>("select * from caracteristica where id=" + idP).ToList().FirstOrDefault();
            if (carateristica != null)
            {
                datosP ndnd = new datosP();
                ndnd.Presentacion = carateristica.Presentacion;
                ViewBag.Presentacion = ndnd.Presentacion;
                return Json( JsonRequestBehavior.AllowGet);
            }

            else
            {
                var errorModel = new { error = "No se encontro" };
                return new JsonHttpStatusResult(errorModel, HttpStatusCode.InternalServerError);
            }
        }
        public ActionResult PdfCertificado(int id)
        {

            CertificadoCalidad certificado = new CertificadoCalidadContext().CertificadosCalidad.Where(s => s.IDCertificado == id).FirstOrDefault();
            Clientes clientes = new ClientesContext().Clientes.Find(certificado.IDCliente);
            Muestreo muestreo = new CalidadContext().Muestreos.Find(certificado.IDMuestreo);

            DocumentoCe x = new DocumentoCe();

            x.lote = certificado.Lote;
            String fnew = String.Format(certificado.FechaCertificado.ToString(), "dd/mm/aaaa");
            x.fecha = fnew +" "+  certificado.FechaCertificado.Hour;
            x.Cliente = clientes.Nombre;
            x.Etiqueta = certificado.Descripcion;
            x.Presentacion = certificado.Presentacion;
            x.CantidadLiberada = certificado.Cantidad;
            x.Muestreo = muestreo.Descripcion;
            x.Responsable = certificado.Responsable;
            x.Orden = certificado.IDOrden;
            x.Pedido = certificado.IDPedido;
            x.Liberacion = certificado.IDCertificado;
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



            System.Drawing.Image logoempresa = byteArrayToImage(empresa.Logo);
            string contentType = System.Net.Mime.MediaTypeNames.Application.Pdf;
            try
            {
                CreaCertificadoPDF documento = new CreaCertificadoPDF(logoempresa, x);
                return new FilePathResult(documento.nombreDocumento, contentType);
            }
            catch (Exception err)
            {
                string mensaje = err.Message;
            }
            return RedirectToAction("Index");
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


        // POST: Calidad
        [HttpPost]
        public ActionResult Create(CertificadoCalidad certificado, FormCollection coleccion)
        {
            //return RedirectToAction("CalidadCertificado", "CertificadosCalidad", new { idmaquina = Maquina });

            ViewBag.Mensaje = "";
            string catidadMal = coleccion.Get("CantidadMal");
            string Letra = "";
            //OrdenProduccion orden = new WorkinProcessContext().Database.SqlQuery<OrdenProduccion>("Select*from OrdenProduccion where idorden=" + certificado.IDOrden).ToList().FirstOrDefault();
            Articulo articulo = new WorkinProcessContext().Database.SqlQuery<Articulo>("Select*from Articulo where IDArticulo=" + certificado.IDArticulo).ToList().FirstOrDefault();
            var CalidadLetras = new CalidadLetraContext().CalidadLetras;
            if (articulo.IDInspeccion == 1)
            {
                foreach (CalidadLetra CalidadL in CalidadLetras)
                {
                    if (certificado.Cantidad >= CalidadL.LI_Lote_mill && certificado.Cantidad <= CalidadL.LS_Lote_mill)
                    {
                        Letra = CalidadL.NGI1;
                    }

                }


            }
            else if (articulo.IDInspeccion == 2)
            {
                foreach (CalidadLetra CalidadL in CalidadLetras)
                {
                    if (certificado.Cantidad >= CalidadL.LI_Lote_mill && certificado.Cantidad <= CalidadL.LS_Lote_mill)
                    {
                        Letra = CalidadL.NGI2;
                    }

                }
            }
            else if (articulo.IDInspeccion == 3)
            {
                foreach (CalidadLetra CalidadL in CalidadLetras)
                {
                    if (certificado.Cantidad >= CalidadL.LI_Lote_mill && certificado.Cantidad <= CalidadL.LS_Lote_mill)
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
                string cadenaA = " select " + Descripcion + " as Dato from dbo.CalidadLimiteAceptacion where [CodigoLetra]='" + Letra + "'";
                ClsDatoEntero rechazo = db.Database.SqlQuery<ClsDatoEntero>(cadenaA).ToList().FirstOrDefault();


                if ((CantidadEtiquetasMal >= rechazo.Dato))
                {
                    ViewBag.Mensaje = "No se genera certificado de calidad por rechazo";
                    return RedirectToAction("Create", new { Mensaje = ViewBag.Mensaje });



                }
                else
                {

                    //OrdenProduccion orden = new WorkinProcessContext().Database.SqlQuery<OrdenProduccion>("Select*from OrdenProduccion where idorden=" + liberacion.IDOrden).ToList().FirstOrDefault();
                    SIAAPI.Models.Comercial.Clientes cliente = new SIAAPI.Models.Comercial.ClientesContext().Clientes.Find(certificado.IDCliente);
                    string fecha = DateTime.Now.Day + "/" + DateTime.Now.Month + "/" + DateTime.Now.Year;
                    try
                    {
                        List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
                        string usuario = userid.Select(s => s.Username).FirstOrDefault();
                        Caracteristica carateristica = new WorkinProcessContext().Database.SqlQuery<Caracteristica>("select * from caracteristica where id=" + certificado.IDCaracteristica).ToList().FirstOrDefault();

                        certificado.IDMuestreo = articulo.IDMuestreo;
                        certificado.IDInspeccion = articulo.IDInspeccion;

                        string ingresar = "INSERT INTO [dbo].[CertificadoCalidad] ([FechaCertificado],[IDOrden],[IDPedido],[IDLibera],[IDCliente],[IDArticulo], IDCaracteristica,Descripcion,Presentacion,IDMuestreo,IDInspeccion,Lote,Cantidad,CodigoLetra,MetodoResultadoPresentacion,PresentacionResultado,PresentacionTintas,Clave,Responsable) VALUES (sysdatetimeoffset(),'" + 0 + "','" + 0 + "','" + 0 + "','" + certificado.IDCliente + "','" + certificado.IDArticulo + "'," + certificado.IDCaracteristica + ",'" + articulo.Descripcion + "','" + carateristica.Presentacion + "'," + articulo.IDMuestreo + "," + articulo.IDInspeccion + ",'" + certificado.Lote + "'," + certificado.Cantidad + ",'" + Letra + "','" + certificado.MetodoResultadoPresentacion + "','','','" + articulo.Cref + "','" + usuario + "')";

                        db.Database.ExecuteSqlCommand(ingresar);

                    }
                    catch (Exception err)
                    {
                        ArticuloContext dbar = new ArticuloContext();


                        //Articulos
                        var datosArticulo = dbar.Articulo.OrderBy(i => i.Cref).ToList();
                        List<SelectListItem> liAC = new List<SelectListItem>();
                        liAC.Add(new SelectListItem { Text = "--Selecciona un Articulo--", Value = "0" });
                        foreach (var a in datosArticulo)
                        {
                            liAC.Add(new SelectListItem { Text = a.Cref + " | " + a.Descripcion, Value = a.IDArticulo.ToString() });

                        }
                        ViewBag.IDArticulo = liAC;
                        ViewBag.PresentacionList = getPresentacionPorArticulo(0);
                        CalidadContext db = new CalidadContext();
                        ViewBag.IDMuestreo = new SelectList(db.Muestreos, "IDMuestreo", "Descripcion");
                        ViewBag.IDInspeccion = new SelectList(db.Inspecciones, "IDInspeccion", "Descripcion");
                        ViewBag.IDCliente = new SelectList(new ClientesContext().Clientes.OrderBy(s => s.Nombre), "IDCliente", "Nombre");





                        return View();

                    }
                }
            }
            catch (Exception err)
            {
                return View();
            }

            int IDC = 0;
            try
            {
                ClsDatoEntero liberacion = db.Database.SqlQuery<ClsDatoEntero>("select max(IDCertificado) as Dato from CertificadoCalidad").ToList().FirstOrDefault();


                IDC = liberacion.Dato;
            }
            catch (Exception err)
            {

            }
            return RedirectToAction("PresentacionArticulo", new { IDCertificado = IDC });





        }

    }
    public class datosprov
    {
        public int IDMuestreo { get; set; }
        public int IDInspeccion { get; set; }
        
    }

    public class datosC
    {
        
        public string LetraR { get; set; }
        public decimal Revisar { get; set; }

    }
    public class datosP
    {

        public string Presentacion { get; set; }
       

    }
}
public class ListaPresentacion
{
    public IEnumerable<SelectListItem> GetCaracteristicaPorArticuloconidpresentacion(int idarticulo)
    {
        List<SelectListItem> lista;
        if (idarticulo == 0)
        {
            lista = new List<SelectListItem>();
            lista.Add(new SelectListItem() { Value = "0", Text = "Elige un articulo primero" });
            return (lista);
        }
        using (var context = new ArticuloContext())
        {
            string cadenasql = "select * from Caracteristica where  Articulo_IDArticulo =" + idarticulo;
            lista = context.Database.SqlQuery<Caracteristica>(cadenasql).ToList()
                .OrderBy(n => n.Presentacion)
                    .Select(n =>
                     new SelectListItem
                     {
                         Value = n.ID.ToString(),
                         Text = n.IDPresentacion + "|" + n.Presentacion
                     }).ToList();
            var countrytip = new SelectListItem()
            {
                Value = null,
                Text = "--- Selecciona una presentación ---"
            };
            lista.Insert(0, countrytip);
            return new SelectList(lista, "Value", "Text");
        }
    }
}
    