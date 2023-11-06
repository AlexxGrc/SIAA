
using FormulaEspecializada;
using FormulaSiaapi;
using iTextSharp.text.pdf.qrcode;
using PagedList;
using SIAAPI.ClasesProduccion;
using SIAAPI.Models.Administracion;
using SIAAPI.Models.Comercial;
using SIAAPI.Models.PlaneacionProduccion;
using SIAAPI.Models.Produccion;
using SIAAPI.ViewModels.Articulo;
using SIAAPI.ViewModels.Comercial;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml.Serialization;

namespace SIAAPI.Controllers.PlaneacionProduccion
{
    public class PlanPlaneacionProduccionEController : Controller
    {
        HEspecificacionEContext db = new HEspecificacionEContext();
        public VHEspCteEContext dbVHE = new VHEspCteEContext();
        // GET: PlaneacionProduccion
        [Authorize(Roles = "Administrador,AdminProduccion,Gerencia,Sistemas")]

        public ActionResult IndexHE(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
        {

            ViewBag.CurrentSort = sortOrder;

            ViewBag.NombreSortParm = String.IsNullOrEmpty(sortOrder) ? "Nombre" : "Nombre";
            ViewBag.TipoClienteSortParm = String.IsNullOrEmpty(sortOrder) ? "TipoCliente" : "TipoCliente";
            ViewBag.FamiliaSortParm = String.IsNullOrEmpty(sortOrder) ? "Familia" : "Familia";
            ViewBag.DescripcionSortParm = String.IsNullOrEmpty(sortOrder) ? "Descripción" : "Descripción";
            ViewBag.PresentacionSortParm = String.IsNullOrEmpty(sortOrder) ? "Presentación" : "Presentación";
            ViewBag.ArticuloSortParm = String.IsNullOrEmpty(sortOrder) ? "Artículo" : "Artículo";
            ViewBag.ModeloProduccionSortParm = String.IsNullOrEmpty(sortOrder) ? "ModeloProducción" : "ModeloProducción";

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

            ViewBag.CurrentFilter = searchString;

            //Paginación
            var elementos = from s in dbVHE.VHEspCtesE  
                            select s;
            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                try
                {
                    int idplaneacion = int.Parse(searchString);
                  elementos=  elementos.Where(s => s.Planeacion == idplaneacion);
                }
                catch(Exception ERR)
                {
                    String mensajedeerror = ERR.Message;
                    elementos = elementos.Where(s => s.Nombre.Contains(searchString) || s.TipoCliente.Contains(searchString) || s.Familia.Contains(searchString) || s.Descripcion.Contains(searchString) || s.Presentacion.Contains(searchString) || s.Articulo.Contains(searchString) || s.ModeloProduccion.Contains(searchString));
                }

              

            }

            //Ordenacion

            switch (sortOrder)
            {
                case "Nombre":
                    elementos = elementos.OrderBy(s => s.Nombre);
                    break;
                case "TipoCliente":
                    elementos = elementos.OrderBy(s => s.TipoCliente);
                    break;
                case "Familia":
                    elementos = elementos.OrderBy(s => s.Familia);
                    break;
                case "Descripcion":
                    elementos = elementos.OrderBy(s => s.Descripcion);
                    break;
                case "Presentacion":
                    elementos = elementos.OrderBy(s => s.Presentacion);
                    break;
                case "Artículo":
                    elementos = elementos.OrderBy(s => s.Articulo);
                    break;
                case "ModeloProducción":
                    elementos = elementos.OrderBy(s => s.ModeloProduccion);
                    break;

                default:
                    elementos = elementos.OrderByDescending(s => s.IDHE);
                    break;
            }

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = dbVHE.VHEspCtesE.OrderBy(e => e.IDHE).Count(); // Total number of elements

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

            //return View(elementos.ToPagedList(pageNumber, pageSize));
            return View(elementos.ToPagedList(pageNumber, pageSize));
            //Paginación
        }

        public ActionResult Details(int id)
        {

            string hoja = dbVHE.Database.SqlQuery<ClsDatoString>("select ESPECIFICACION as dato from [dbo].[HEspecificacionE] WHERE IDHE=" + id).ToList<ClsDatoString>()[0].Dato;
            ViewBag.Hojat=hoja;
            var elemento = dbVHE.VHEspCtesE.Single(m => m.IDHE == id);
            //if (elemento == null)
            //{
            //    return NotFound();
            //}
            return View(elemento);
        }
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult getcaracteristica(int id)
        {
            VCaracteristicaContext caracteristica = new VCaracteristicaContext();
            var carac = caracteristica.VCaracteristica.Where(x => x.Articulo_IDArticulo == id).ToList();
            List<SelectListItem> listmetodo = new List<SelectListItem>();


            foreach (var x in carac)
            {

                listmetodo.Add(new SelectListItem { Text = x.IDPresentacion +" "+ x.Presentacion, Value = x.ID.ToString() });

            }



            return Json(new SelectList(listmetodo, "Value", "Text", JsonRequestBehavior.AllowGet));
        }
        public ActionResult HojaE()
        {
            //FamiliaContext fam = new FamiliaContext();
            var listas = new FamiliaRepository().GetFamilias();
            ViewBag.IDFamilia = listas;
            //ViewBag.IDFamilia = new SelectList(fam.Familias, "IDFamilia", "Descripcion");
            ClientesContext clientes = new ClientesContext();
            ViewBag.IDCliente = new ClienteRepository().GetClientesNombre();
            ClientesPContext clientesp = new ClientesPContext();
            ViewBag.IDClienteP = new SelectList(clientesp.ClientesPs, "IDClienteP", "Nombre");
            ArticuloContext articulo = new ArticuloContext();
            //ViewBag.IDArticulo = new SelectList(articulo.Articulo, "IDArticulo", "Descripcion");
            //VCaracteristicaContext caracteristica = new VCaracteristicaContext();
            //ViewBag.IDCaracteristica = new SelectList(caracteristica.VCaracteristica, "IDCaracteristica", "Presentacion");

            var proveedor = articulo.Articulo.Where(s => s.GeneraOrden.Equals(true)).OrderBy(s => s.Cref).ToList();
            List<SelectListItem> li = new List<SelectListItem>();
            li.Add(new SelectListItem { Text = "--Selecciona un Artículo--", Value = "0" });

            //if (proveedor.Count==0)
            //{
            //    var reshtml = Server.HtmlEncode("No hay articulos que tengan marcados que generan órdenes de producción ");

            //    return Content(reshtml);
            //}

            foreach (var m in proveedor)
            {
                li.Add(new SelectListItem { Text = m.Cref+"|"+ m.Descripcion, Value = m.IDArticulo.ToString() });
               

            }
            ViewBag.proveedor = li;
            HEspecificacionE elemento = new HEspecificacionE();
            elemento.FechaEspecificacion = DateTime.Now;
            return View(elemento);
        }



        public ActionResult AgregarModelo(int? id)
        {
            ModelosDeProduccionContext db = new ModelosDeProduccionContext();
            ViewBag.id = id;
            HEspecificacionE hespecificacion = new HEspecificacionEContext().HEspecificacionesE.Find(id);
            if (hespecificacion.IDModeloProduccion != 0)
            {
                
                ViewBag.IDModeloProduccion = new SelectList(db.ModelosDeProducciones, "IDModeloProduccion", "Descripcion",hespecificacion.IDModeloProduccion);
                ViewBag.titulo = "Editar Modelo de Producción";
            }
         else
            {
                ViewBag.IDModeloProduccion = new SelectList(db.ModelosDeProducciones, "IDModeloProduccion", "Descripcion");
                ViewBag.titulo = "Agregar Modelo de Producción";
            }

            return PartialView();
        }
        [HttpPost]
        public JsonResult addmodelop(int? id, int? idmodelo)
        {
            try
            {
                db.Database.ExecuteSqlCommand("update HEspecificacionE set [IDModeloProduccion]='" + idmodelo + "' where [IDHE]='" + id + "'");

                HEspecificacionE hoja = db.HEspecificacionesE.Find(id);
                string cadenaplantilla = "plantilla-M-" + idmodelo+".xml";

                string path = Path.Combine(Server.MapPath("~/"), cadenaplantilla); 
                Plantilla plantilla;

                using (TextReader reader = new StreamReader(path))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(Plantilla));
                    plantilla = (Plantilla)serializer.Deserialize(reader);
                }

                foreach (ArticuloXML arti in plantilla.Articulos)
                {
                    db.Database.ExecuteSqlCommand("insert into [dbo].[ArticulosPlaneacionE] ([IDHE],[Version],[IDArticulo],[IDTipoArticulo],[IDCaracteristica],[IDProceso],[Formuladerelacion],[factorcierre],[Indicaciones],[Planeacion] ) values ('" + id + "', '" + hoja.Version + "', '" + arti.IDArticulo + "', '" + arti.IDTipoArticulo + "', '" + arti.IDCaracteristica + "', '" + arti.IDProceso + "', '" + arti.Formula + "', '" + arti.FactorCierre + "', '" + arti.Indicaciones + "', '"+hoja.Planeacion+"')");  
                }

                return Json(true);
            }
            catch (Exception err)
            {
                return Json(500, err.Message);
            }
        }


        [HttpPost]
        public ActionResult InsertaHojaE(HEspecificacionE hespecificacion, bool? cliente, bool? clientep, bool? General, bool? artr, bool? artnr)
        {
            SIAAPI.Models.Comercial.Articulo art= new SIAAPI.Models.Comercial.Articulo();
            int noplaneacion = 1;
            DateTime fecha = hespecificacion.FechaEspecificacion;
            string fechaf = fecha.ToString("yyyyMMdd");

            ClsDatoEntero numplaneacion = db.Database.SqlQuery<ClsDatoEntero>("select count(IDHE) as Dato from HEspecificacionE").ToList()[0];
            if (numplaneacion.Dato != 0)
            {
                ClsDatoEntero planeacion = db.Database.SqlQuery<ClsDatoEntero>("select max(Planeacion) as Dato from HEspecificacionE").ToList()[0];
                noplaneacion = planeacion.Dato + 1;
            }

            if (cliente==null && clientep==null && General==null)
            {
                General = true;
            }

            if (artr != null)
            {
                ArticuloContext articulo = new ArticuloContext();
                 art = articulo.Articulo.Find(hespecificacion.IDArticulo);
                hespecificacion.IDFamilia = art.IDFamilia;
                VCaracteristicaContext caracteristica = new VCaracteristicaContext();
                VCaracteristica carac = caracteristica.VCaracteristica.Find(hespecificacion.IDCaracteristica);
                if (cliente != null )
                {
                    //  db.Database.ExecuteSqlCommand("insert into HEspecificacionE([FechaEspecificacion],[IDCliente],[IDClienteP],[IDFamilia],[IDArticulo],[IDCaracteristica],[Descripcion],[Presentacion],[IDModeloProduccion],[Version],[Planeacion],[Adhesivo],[MangaTermoencogible],[NoTintas],[HotStamping],[CantidadPresentacion],[gapeje],[gapavance]) values ('" + fechaf + "','" + hespecificacion.IDCliente + "','0','" + art.IDFamilia + "','" + art.IDArticulo + "','" + carac.ID + "','" + art.Descripcion + "','" + carac.Presentacion + "','0','1','"+ noplaneacion + "','"+hespecificacion.Adhesivo+"','"+hespecificacion.MangaTermoencogible+"','"+hespecificacion.NoTintas+"','"+hespecificacion.HotStamping+"','"+hespecificacion.CantidadPresentacion+"',"+hespecificacion.gapeje+","+hespecificacion.gapavance+")");
                    db.Database.ExecuteSqlCommand("insert into HEspecificacionE([FechaEspecificacion],[IDCliente],[IDClienteP],[IDFamilia],[IDArticulo],[IDCaracteristica],[Descripcion],[Presentacion],[IDModeloProduccion],[Version],[Planeacion]) values ('" + fechaf + "'," + hespecificacion.IDCliente + ",0,'" + art.IDFamilia + "','" + art.IDArticulo + "','" + carac.ID + "','" + art.Descripcion + "','" + carac.Presentacion + "','0','1','" + noplaneacion + "')");
                }
                else if (clientep != null)
                {
                    db.Database.ExecuteSqlCommand("insert into HEspecificacionE([FechaEspecificacion],[IDCliente],[IDClienteP],[IDFamilia],[IDArticulo],[IDCaracteristica],[Descripcion],[Presentacion],[IDModeloProduccion],[Version],[Planeacion]) values ('" + fechaf + "',0," + hespecificacion.IDClienteP + ",'" + art.IDFamilia + "','" + art.IDArticulo + "','" + carac.ID + "','" + art.Descripcion + "','" + carac.Presentacion + "','0','1','" + noplaneacion + "')");
                }
                else if (General != null )
                {
                    db.Database.ExecuteSqlCommand("insert into HEspecificacionE([FechaEspecificacion],[IDCliente],[IDClienteP],[IDFamilia],[IDArticulo],[IDCaracteristica],[Descripcion],[Presentacion],[IDModeloProduccion],[Version],[Planeacion]) values ('" + fechaf + "',0,0,'" + art.IDFamilia + "','" + art.IDArticulo + "','" + carac.ID + "','" + art.Descripcion + "','" + carac.Presentacion + "','0','1','" + noplaneacion + "')");
                }
            }
            else if (artnr != null)
            {
                hespecificacion.Presentacion = hespecificacion.Presentacion.TrimEnd(',');
                if (cliente != null)
                {
                    // db.Database.ExecuteSqlCommand("insert into HEspecificacionE([FechaEspecificacion],[IDCliente],[IDClienteP],[IDFamilia],[IDArticulo],[IDCaracteristica],[Descripcion],[Presentacion],[IDModeloProduccion],[Version],[Planeacion],[Adhesivo],[MangaTermoencogible],[NoTintas],[HotStamping],[CantidadPresentacion],[gapeje],[gapavance]) values ('" + fechaf + "','" + hespecificacion.IDCliente + "','0','" + hespecificacion.IDFamilia + "','0','0','" + hespecificacion.Descripcion + "','" + hespecificacion.Presentacion + "','0','1','" + noplaneacion + "','" + hespecificacion.Adhesivo + "','" + hespecificacion.MangaTermoencogible + "','" + hespecificacion.NoTintas + "','" + hespecificacion.HotStamping + "','" + hespecificacion.CantidadPresentacion + "'," + hespecificacion.gapeje + "," + hespecificacion.gapavance + ")");
                    db.Database.ExecuteSqlCommand("insert into HEspecificacionE([FechaEspecificacion],[IDCliente],[IDClienteP],[IDFamilia],[IDArticulo],[IDCaracteristica],[Descripcion],[Presentacion],[IDModeloProduccion],[Version],[Planeacion]) values ('" + fechaf + "','" + hespecificacion.IDCliente + "','0','" + hespecificacion.IDFamilia + "','0','0','" + hespecificacion.Descripcion + "','" + hespecificacion.Presentacion + "','0','1','" + noplaneacion + "')");
                }
                else if (clientep != null)
                {
                    db.Database.ExecuteSqlCommand("insert into HEspecificacionE([FechaEspecificacion],[IDCliente],[IDClienteP],[IDFamilia],[IDArticulo],[IDCaracteristica],[Descripcion],[Presentacion],[IDModeloProduccion],[Version],[Planeacion]) values ('" + fechaf + "','0','" + hespecificacion.IDClienteP + "','" + hespecificacion.IDFamilia + "','0','0','" + hespecificacion.Descripcion + "','" + hespecificacion.Presentacion + "','0','1','" + noplaneacion + "')");
                }
                else if (General != null )
                {
                    db.Database.ExecuteSqlCommand("insert into HEspecificacionE([FechaEspecificacion],[IDCliente],[IDClienteP],[IDFamilia],[IDArticulo],[IDCaracteristica],[Descripcion],[Presentacion],[IDModeloProduccion],[Version],[Planeacion]) values ('" + fechaf + "',0,0,'" + hespecificacion.IDFamilia + "','0','0','" + hespecificacion.Descripcion + "','" + hespecificacion.Presentacion + "','0','1','" + noplaneacion + "')");
                }
            }
            else if (artnr == null && artr == null)
            {
                ViewBag.Mensaje = "Seleccione un Artículo o Registre un Artículo";
            }

            int idEspecD = db.Database.SqlQuery<ClsDatoEntero>("select IDHE as Dato from [dbo].[HEspecificacionE] where planeacion=" + noplaneacion + " and version=1").ToList<ClsDatoEntero>()[0].Dato;

            return RedirectToAction("CrearHE","HojaEspec",new { @id= hespecificacion.IDFamilia, @IdEspec = idEspecD });

        }
        public ActionResult AgregaArticulo(int? id)
        {
            HEspecificacionE hespecificacion = db.HEspecificacionesE.Find(id);
            ViewBag.Descripcion = hespecificacion.Descripcion;
           FamiliaContext dbfam = new FamiliaContext();
            ViewBag.IDFamilia = new SelectList(dbfam.Familias, "IDFamilia", "Descripcion",hespecificacion.IDFamilia);
           ArticuloContext dbta = new ArticuloContext();
            ViewBag.IDTipoArticulo = new SelectList(dbta.TipoArticulo, "IDTipoArticulo", "Descripcion");
            c_MonedaContext dbmo = new c_MonedaContext();
            ViewBag.IDMoneda = new SelectList(dbmo.c_Monedas, "IDMoneda", "Descripcion");
            c_ClaveUnidadContext dbcu = new c_ClaveUnidadContext();
            ViewBag.IDClaveUnidad = new SelectList(dbcu.c_ClaveUnidades, "IDClaveUnidad", "Nombre");
            ViewBag.IDAQL = new SelectList(dbta.AQLCalidad, "IDAQL", "Descripcion");
            ViewBag.IDInspeccion = new SelectList(dbta.Inspeccion, "IDInspeccion", "Descripcion");
            ViewBag.IDMuestreo = new SelectList(dbta.Muestreo, "IDMuestreo", "Descripcion");
            ViewBag.idhe = id;
            return PartialView();
        }
        public ActionResult addArticulo(int? idhe, mArticulo Art)
        {
            try
            {
                SIAAPI.Models.Comercial.Articulo newArt = new SIAAPI.Models.Comercial.Articulo();
                newArt.IDArticulo = Art.IDArticulo;
                newArt.Cref = Art.Cref;
                newArt.Descripcion = Art.Descripcion;
                newArt.IDFamilia = Art.IDFamilia;
                newArt.IDTipoArticulo = Art.IDTipoArticulo;
                newArt.Preciounico = Art.Preciounico;
                newArt.IDMoneda = Art.IDMoneda;
                newArt.CtrlStock = Art.CtrlStock;
                newArt.ManejoCar = Art.ManejoCar;
                newArt.IDClaveUnidad = Art.IDClaveUnidad;
                newArt.bCodigodebarra = Art.bCodigodebarra;
                newArt.Codigodebarras = Art.Codigodebarras;
                newArt.Obscalidad = Art.Obscalidad;
                newArt.ExistenDev = Art.ExistenDev;
                newArt.IDAQL = Art.IDAQL;
                newArt.IDInspeccion = Art.IDInspeccion;
                newArt.IDMuestreo = Art.IDMuestreo;
                newArt.esKit = Art.esKit;
                newArt.nameFoto = saveIMG(Art.fileIMG, newArt.nameFoto);
                newArt.GeneraOrden = Art.GeneraOrden;
                newArt.obsoleto = Art.obsoleto;
                newArt.MinimoVenta = Art.MinimoVenta;
                newArt.MinimoCompra = Art.MinimoCompra;

                ArticuloContext dba = new ArticuloContext();
                dba.Articulo.Add(newArt);
                dba.SaveChanges();
                //db.Database.ExecuteSqlCommand("INSERT INTO [dbo].[Articulo]([Cref],[Descripcion],[Preciounico],[CtrlStock],[ManejoCar],[Obscalidad],[ExistenDev],[IDAQL],[bCodigodebarra],[Codigodebarras],[esKit],[nameFoto],[GeneraOrden],[IDClaveUnidad],[IDMoneda],[IDFamilia],[IDMuestreo],[IDTipoArticulo],[IDInspeccion],[obsoleto],[MinimoVenta],[MinimoCompra])VALUES('" + Cref + "','" + Descripcion + "','" + Preciounico + "','" + CtrlStock + "','" + ManejoCar + "','Sin Dato','" + ExistenDev + "','" + IDAQL + "' ,'" + Codigodebarra + "','" + Codigodeb + "','" + esKit + "' ,'" + imgProducto + "','" + GeneraOrden + "','" + IDClaveUnidad + "','" + IDMoneda + "','" + IDFamilia + "','" + IDMuestreo + "','" + IDTipoArticulo + "','" + IDInspeccion + "','" + obsoleto + "','" + MinimoVenta + "','" + MinimoCompra + "')");
                HEspecificacionE hespecificacion = db.HEspecificacionesE.Find(idhe);

                string[] arraydatos;

                arraydatos = hespecificacion.Presentacion.Split(',');
                string acc=null;
                for (int i=0;i< arraydatos.Length; i++)
                {
                    string cuenta = arraydatos[i];

                    string[] arraydatoscortados;
                    arraydatoscortados = cuenta.Split(':');
                    for (int j = 0; j <arraydatoscortados.Length; j++)
                    {
                       
                        string dato = "?"+arraydatoscortados[j]+"?";

                        if (j+1 == arraydatoscortados.Length)
                        {
                            acc = acc + dato+",";
                        }
                        else
                        {
                            acc = acc + dato + ":";
                        }


                      
                    }
                  

                }
               string quitarcoma = acc.TrimEnd(',');
                string final = "{"+quitarcoma.Replace('?','"')+"}";

                int NewIDP = db.Database.SqlQuery<int>("SELECT MAX(IDArticulo) from Articulo").FirstOrDefault();
                db.Database.ExecuteSqlCommand("insert into Caracteristica (IDPresentacion, Cotizacion, version, Presentacion, jsonPresentacion, Articulo_IDArticulo )  values ('1','0','0','" + hespecificacion.Presentacion + "','" + final + "'," + NewIDP + ")");
                int idcaracteristica = db.Database.SqlQuery<int>("SELECT MAX(ID) from Caracteristica").FirstOrDefault();
                db.Database.ExecuteSqlCommand("update HEspecificacionE set IDCaracteristica='" + idcaracteristica + "', IDArticulo='" + NewIDP + "' where IDHE='" + idhe + "'");


                return RedirectToAction("IndexHE");

            }
            catch (Exception ex)
            {
                return Json(new HttpStatusCodeResult(500, ex.Message));
            }
        }

        public string saveIMG(HttpPostedFileBase file, string name)
        {
            try
            {
                if ((file != null))
                {

                    string ext = Path.GetExtension(file.FileName);
                    string newName = Guid.NewGuid().ToString();

                    newName = newName + ext;
                    string path = Server.MapPath("~/imagenes/Upload/");
                    newName = newName.ToUpper();
                    file.SaveAs(path + newName);

                    if (System.IO.File.Exists(path + name))
                    {
                        System.IO.File.Delete(path + name);
                    }

                    return newName;
                }
                else
                {
                    return name;
                }


            }
            catch (Exception ex)
            {

                return ex.Message;
            }
        }
        public ActionResult Seleccionar(int? id)
        {
            HEspecificacionE hespecificacion = db.HEspecificacionesE.Find(id);
            System.Web.HttpContext.Current.Session["IDHE"] = hespecificacion.IDHE;
            System.Web.HttpContext.Current.Session["Version"] = hespecificacion.Version;
            System.Web.HttpContext.Current.Session["IDModeloProduccion"] = hespecificacion.IDModeloProduccion;
            return RedirectToAction("Index");

        }

        public ActionResult ClonarHE(int? id)
        {
            HEspecificacionE hespecificacion = db.HEspecificacionesE.Find(id);
            DateTime fecha = hespecificacion.FechaEspecificacion;
            int numplaneacion = hespecificacion.Planeacion;
            string fechaf = fecha.ToString("yyyyMMdd");
            int noversion =  db.Database.SqlQuery<ClsDatoEntero>("select max(version)+1 as Dato from HEspecificacionE where planeacion=" + numplaneacion).ToList()[0].Dato;
           // db.Database.ExecuteSqlCommand("insert into HEspecificacionE([FechaEspecificacion],[IDCliente],[IDClienteP],[IDFamilia],[IDArticulo],[IDCaracteristica],[Descripcion],[Presentacion],[IDModeloProduccion],[Version],[Planeacion]) values ('" + fechaf + "','" + hespecificacion.IDCliente + "','" + hespecificacion.IDClienteP + "','" + hespecificacion.IDFamilia + "','" + hespecificacion.IDArticulo + "','" + hespecificacion.IDCaracteristica + "','" + hespecificacion.Descripcion + "','" + hespecificacion.Presentacion + "','" + hespecificacion.IDModeloProduccion + "','" + noversion + "','" + hespecificacion.Planeacion + "','" + hespecificacion.Adhesivo + "','" + hespecificacion.MangaTermoencogible + "','" + hespecificacion.NoTintas + "','" + hespecificacion.HotStamping + "','" + hespecificacion.CantidadPresentacion + "',"+hespecificacion.gapeje+","+hespecificacion.gapavance+")");
              db.Database.ExecuteSqlCommand("INSERT INTO HEspecificacionE([FechaEspecificacion],[IDCliente],[IDClienteP],[IDFamilia],[IDArticulo],[IDCaracteristica],[Descripcion],[Presentacion],[IDModeloProduccion],[Version],[Planeacion],especificacion) SELECT [FechaEspecificacion],[IDCliente],[IDClienteP],[IDFamilia],[IDArticulo],[IDCaracteristica],[Descripcion],[Presentacion],[IDModeloProduccion]," + noversion +"," + numplaneacion +",especificacion FROM HEspecificacionE where IDHE='" + id + "'");
            ClsDatoEntero num = db.Database.SqlQuery<ClsDatoEntero>("select IDHE as Dato from HEspecificacionE where planeacion="+ numplaneacion +" and version="+noversion).ToList()[0];
            
            List<ArticulosPlaneacionE> articulosp = db.Database.SqlQuery<ArticulosPlaneacionE>("select IDArtPlan from ArticulosPlaneacionE where IDHE='"+id+"'").ToList();
            ViewBag.articulos = articulosp;
            for (int i = 0; i < articulosp.Count(); i++)
            {
                ArticulosPlaneacionEContext car = new ArticulosPlaneacionEContext();
                ArticulosPlaneacionE articuloplaneacion = car.ArticulosPlaneacionesE.Find(ViewBag.articulos[i].IDArtPlan);
                db.Database.ExecuteSqlCommand("insert into [dbo].[ArticulosPlaneacionE] ([IDHE],[Version],[IDArticulo],[IDTipoArticulo],[IDCaracteristica],[IDProceso],[Formuladerelacion],[factorcierre],[Indicaciones],[Planeacion]) values ('" + num.Dato + "','" + noversion + "','" + articuloplaneacion.IDArticulo + "', '" + articuloplaneacion.IDTipoArticulo + "', '" + articuloplaneacion.IDCaracteristica + "', '" + articuloplaneacion.IDProceso + "', '" + articuloplaneacion.Formuladerelacion + "', '" + articuloplaneacion.factorcierre + "', '" + articuloplaneacion.Indicaciones + "','" + articuloplaneacion.Planeacion + "')");

            }

            return RedirectToAction("IndexHE");

        }

        public ActionResult GetPresentaciones(int? id,int? idproceso)
        {

            VPArticuloContext db = new VPArticuloContext();
            var x = db.VPArticulos.Find(id);
            ViewBag.Descripcion = x.Descripcion;
            var lista = db.Database.SqlQuery<SIAAPI.Models.Comercial.Caracteristica>("select * from Caracteristica where Articulo_IDArticulo=" + id + "").ToList();
            SIAAPI.Models.Comercial.Articulo arti = new ArticuloContext().Articulo.Find(id);
            ViewBag.idtipoarticuloformula = arti.IDTipoArticulo;
            ViewBag.idprocesoformula = idproceso;
            return PartialView(lista);

        }
        [HttpPost]
        public JsonResult addarticuloplaneacion(int? idhe, int? idarticulo,int? idcaracteristica,int? idproceso,string formular,decimal? factorc,string indicaciones)
        {
            try
            {
              
                     ArticuloContext articulodb = new ArticuloContext();
                SIAAPI.Models.Comercial.Articulo  articulo = articulodb.Articulo.Find(idarticulo);
                     HEspecificacionEContext hojadb = new HEspecificacionEContext();
                     HEspecificacionE hoja = hojadb.HEspecificacionesE.Find(idhe);
                     db.Database.ExecuteSqlCommand("insert into [dbo].[ArticulosPlaneacionE] ([IDHE],[Version],[IDArticulo],[IDTipoArticulo],[IDCaracteristica],[IDProceso],[Formuladerelacion],[factorcierre],[Indicaciones],[Planeacion]) values ('" + idhe + "','" + hoja.Version + "','" + idarticulo + "', '" + articulo.IDTipoArticulo + "', '" + idcaracteristica + "', '" + idproceso + "', '" + formular + "', '" + factorc + "', '" + indicaciones + "','"+hoja.Planeacion+"')");

              
                return Json(new HttpStatusCodeResult(200));
            }
            catch (Exception err)
            {
                string mensaje = err.Message;
                return Json(new { errorMessage = "error" });
            }
            
        }
        public ActionResult Presentacion(int? id)
        {
            List<AtributodeFamilia> atributos = db.Database.SqlQuery<AtributodeFamilia>("select * from AtributodeFamilia where IDFamilia='" + id + "'").ToList();

            return PartialView(atributos);
        }
      
     public ActionResult ArticuloPlaneacion(string sortOrder, string currentFilter, string searchString, int? page, int? id,string TipoArticulo,int?idhe)
        {
            ViewBag.VerPag = null;
            int idhee = 0;
            ArticuloContext db = new ArticuloContext();
            if (TipoArticulo!= null && TipoArticulo.Equals("Documento"))
            {
                try
                {
                    idhee = int.Parse(System.Web.HttpContext.Current.Session["IDHE"].ToString());

                    ViewBag.idhe = idhee;
                    idhe = idhee;
                    
                }
                catch
                {
                    ViewBag.idhe = idhe;
                }
                ViewBag.idproceso = id;
                return RedirectToAction("Documento", new { idhe = idhe, idproceso = id });
            }
          

            try
            {
                idhee= int.Parse(System.Web.HttpContext.Current.Session["IDHE"].ToString());
                
                ViewBag.idhe = idhee;
                idhe = idhee;
            }
            catch
            {
                ViewBag.idhe = idhe;
            }

            ViewBag.idproceso = id;
            HEspecificacionEContext dbhe = new HEspecificacionEContext();
            HEspecificacionE hespecificacion = dbhe.HEspecificacionesE.Find(idhe);
            ViewBag.version = hespecificacion.Version;
            ViewBag.preshe = hespecificacion.Presentacion;
            ViewBag.arthe = hespecificacion.Descripcion;

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
                procesoLst.Add("Cintas");
                procesoLst.Add("Tintas");
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
                elementos = elementos.Where(s => ( s.Descripcion.Contains(searchString) || s.Cref.Contains(searchString) ) && s.IDTipoArticulo.Equals(idtipoa.Dato));
                ViewBag.VerPag = 1;
              

            }


           
           
            int pageSize = 12;
            int pageNumber = (page ?? 1);
            int count = va.Articulo.OrderBy(e => e.IDArticulo).Count();

            List<VDocumentoE> documentos = db.Database.SqlQuery<VDocumentoE>("select DE.IDDocumento,DE.IDHE,DE.Version,DE.IDProceso,DE.Descripcion,DE.Planeacion,DE.Nombre,P.NombreProceso AS Proceso from DocumentoE as DE inner join Proceso as P on P.IDProceso=DE.IDProceso where DE.IDHE='" + idhe + "' and DE.IDProceso='" +id + "'").ToList();
            ViewBag.Documentos = documentos;

            List<VArticulosPlaneacionE> datos = db.Database.SqlQuery<VArticulosPlaneacionE>("select  AP.Planeacion,AP.IDArtPlan,AP.IDHE,AP.Version,A.Cref as Cref,A.Descripcion as Articulo,TP.Descripcion as TipoArticulo,C.Presentacion,P.NombreProceso as Proceso,AP.Formuladerelacion,AP.factorcierre,AP.Indicaciones from ArticulosPlaneacionE as AP inner join Articulo as A on A.IDArticulo=AP.IDArticulo inner join Caracteristica as C on C.ID=AP.IDCaracteristica inner join Proceso as P on P.IDProceso=AP.IDProceso inner join TipoArticulo as TP on TP.IDTipoArticulo=A.IDTipoArticulo where AP.IDHE='" + idhe+ "' and P.NombreProceso='"+proceso.NombreProceso+"'").ToList();

            ViewBag.Datos = datos;

            return View(elementos.OrderBy(i => i.Descripcion).ToPagedList(page ?? 1, pageSize));

        }

        [HttpPost]
        public JsonResult Deleteitem(int id)
        {
            try
            {
                ArticulosPlaneacionEContext car = new ArticulosPlaneacionEContext();
                car.Database.ExecuteSqlCommand("delete from ArticulosPlaneacionE where IDArtPlan=" + id);

                return Json(true);
            }
            catch (Exception err)
            {
                return Json(500, err.Message);
            }
        }
        [HttpPost]
        public ActionResult ValidarFormula(int? idhe,string formular,decimal factorc,int?caracteristica)
        {

           

            try
            {
            HEspecificacionE hespecificacion = db.HEspecificacionesE.Find(idhe);
            string Presentacion=hespecificacion.Presentacion;

            Formulas formula = new Formulas();

            formula.cadenadepresentacion = hespecificacion.Presentacion;

            string formulanueva = formula.sustituircontenidocadena(formular, 1);
            
            double valor = formula.Calcular(formulanueva, double.Parse( factorc.ToString()));
                
                if (valor < 0)
                {
                    return Json(new { errorMessage = "error" });
                }
                return Json(new HttpStatusCodeResult(200));
            }
            catch (Exception err)
            {
                string mensaje = err.Message;
                return Json(new { errorMessage = "error" });
            }

            
        }
        public ActionResult VerPrecio(int? idhe)
        {
            ViewBag.idhe = idhe;
            HEspecificacionE hespecificacion = new HEspecificacionEContext().HEspecificacionesE.Find(idhe);
            ViewBag.Articulo = hespecificacion.Descripcion;
            ViewBag.Presentacion = hespecificacion.Presentacion.Replace(","," ");
            List<VRangoPlaneacionNiveles> rangonivel = db.Database.SqlQuery<VRangoPlaneacionNiveles>("select HE.IDCliente, RPN.IDMoneda, RPN.TC, RPN.IDRPN, RPN.RangoInf, RPN.RangoSup, RPN.Precio as costo, RPN.Porcentaje, HE.Descripcion, HE.Presentacion, Nivel from RangoPlaneacionNiveles as RPN inner join HEspecificacionE as HE on RPN.IDHE = HE.IDHE  where HE.IDHE =" + idhe+ " group by HE.IDCliente, RPN.IDMoneda, RPN.TC, RPN.IDRPN, RPN.RangoInf, RPN.RangoSup, RPN.Precio, RPN.Porcentaje, HE.Descripcion, HE.Presentacion, Nivel order by RPN.RangoInf").ToList();
            

            if (rangonivel.Count==0)
            {
                return RedirectToAction("Error", new { mensaje = "No se ha realizado el proceso de calcular" });
            }
          //  List<VRangoPlaneacionNiveles> rangonivel = db.Database.SqlQuery<VRangoPlaneacionNiveles>("select distinct(Rangoinf),RangoSup from VRangoNiveles where IDHE=" + idhe + "").ToList();
            ViewBag.rangoniveles = rangonivel;

            ClsDatoString monedapsugerido = db.Database.SqlQuery<ClsDatoString>("select distinct(M.Descripcion) as Dato from RangoPlaneacionNiveles as RPN inner join HEspecificacionE as HE on RPN.IDHE=HE.IDHE inner join c_Moneda as M on M.IDMoneda=RPN.IDMoneda where HE.IDHE="+idhe+"").ToList()[0];
            ViewBag.monedapsugerido = monedapsugerido.Dato;

            return View();
        }
        public ActionResult VerCosto(int? idhe)
        {
            HEspecificacionE hespecificacion = new HEspecificacionEContext().HEspecificacionesE.Find(idhe);
            ViewBag.Articulo = hespecificacion.Descripcion;
            ViewBag.Presentacion = hespecificacion.Presentacion;

            List<VRangoPlaneacionCosto> costo = db.Database.SqlQuery<VRangoPlaneacionCosto>("select RangoPlaneacionArticulo.RangoInf,RangoPlaneacionArticulo.RangoSup,TA.Descripcion,sum(Costo) as Costo from RangoPlaneacionArticulo inner join Articulo as A on A.IDArticulo=RangoPlaneacionArticulo.IDArticulo inner join TipoArticulo as TA on TA.IDTipoArticulo=RangoPlaneacionArticulo.IDTipoArticulo where RangoPlaneacionArticulo.IDHE=" + idhe + " group by A.IDTipoArticulo,TA.Descripcion,RangoPlaneacionArticulo.RangoInf,RangoPlaneacionArticulo.RangoSup order by RangoPlaneacionArticulo.RangoInf,RangoPlaneacionArticulo.RangoSup").ToList();

            List<VRangosPlaneacion> Rangocosto = db.Database.SqlQuery<VRangosPlaneacion>("select RangoPlaneacionArticulo.RangoInf,RangoPlaneacionArticulo.RangoSup from RangoPlaneacionArticulo inner join Articulo as A on A.IDArticulo=RangoPlaneacionArticulo.IDArticulo inner join TipoArticulo as TA on TA.IDTipoArticulo=RangoPlaneacionArticulo.IDTipoArticulo where RangoPlaneacionArticulo.IDHE=" + idhe + " group by RangoPlaneacionArticulo.RangoInf,RangoPlaneacionArticulo.RangoSup order by RangoPlaneacionArticulo.RangoInf,RangoPlaneacionArticulo.RangoSup").ToList();

            if(Rangocosto.Count==0)
            {
                return RedirectToAction("Error", new { mensaje = "No se ha realizado el proceso de calcular" });
            }

            ViewBag.Rangocosto = Rangocosto;

            ClsDatoString moneda = db.Database.SqlQuery<ClsDatoString>("select distinct(M.Descripcion) as Dato from RangoPlaneacionArticulo inner join c_Moneda as M on M.IDMoneda=RangoPlaneacionArticulo.IDMoneda where RangoPlaneacionArticulo.IDHE=" + idhe + "").ToList()[0];
            ViewBag.Moneda = moneda.Dato;

            var divisa = db.Database.SqlQuery<VRangoPlaneacionCosto>("select RangoPlaneacionArticulo.RangoInf,RangoPlaneacionArticulo.RangoSup,sum(Costo)/RangoPlaneacionArticulo.RangoSup as Costo from RangoPlaneacionArticulo inner join Articulo as A on A.IDArticulo=RangoPlaneacionArticulo.IDArticulo inner join TipoArticulo as TA on TA.IDTipoArticulo=RangoPlaneacionArticulo.IDTipoArticulo where RangoPlaneacionArticulo.IDHE=" + idhe + " group by RangoPlaneacionArticulo.RangoInf,RangoPlaneacionArticulo.RangoSup").ToList();
            ViewBag.sumatoria = divisa;


            return View(costo);
        }
        public ActionResult Calcular(int idhe)
        {
            HEspecificacionE hoja = new HEspecificacionEContext().HEspecificacionesE.Find(idhe);
            int idmodelo=0;


            var rangos = new RangoPlaneacionContext().Rangos.Where(m => m.IDHE == idhe).ToList();

            if (rangos.Count==0)
            {
                return RedirectToAction("Error", new { mensaje = "No has definido los rangos a trabajar" });
            }
            



           
            try
            {
                idmodelo = hoja.IDModeloProduccion;
            }
            catch (Exception err)
            {
                string mensaje = err.Message;
            }
            ProcesoContext procesobd = new ProcesoContext();
            List<Proceso> procesos = procesobd.Database.SqlQuery<Proceso>("select P.* from Proceso as P inner join  ModeloProceso as M on p.IDProceso = M.Proceso_IDProceso where M.ModelosDeProduccion_IDModeloProduccion = " +  idmodelo +" order by M.orden; ").ToList();
            if (procesos.Count == 0)
            {
                return RedirectToAction("Error", new { mensaje = "Ups !!!No hay procesos relacionados a su modelo de produccion por favor defina procesos a su modelo" });
            }

            StringBuilder cadena2 = new StringBuilder();

           
            foreach (var proceso in procesos)
            {
                if (proceso.UsaManoDeObra)
                {
                    var articulospl = new ArticulosPlaneacionEContext().ArticulosPlaneacionesE.Where(m => m.IDHE == idhe && m.IDTipoArticulo==5 && m.IDProceso==proceso.IDProceso).ToList();
                    if (articulospl.Count == 0)
                    {
                        cadena2.Append("El proceso " + proceso.NombreProceso + " Necesita de especificar una mano de obra,");
                    }
                }
                if (proceso.UsaMaquina)
                {
                    var articulospl = new ArticulosPlaneacionEContext().ArticulosPlaneacionesE.Where(m => m.IDHE == idhe && m.IDTipoArticulo == 3 && m.IDProceso == proceso.IDProceso).ToList();
                    if (articulospl.Count == 0)
                    {
                        cadena2.Append("El proceso " + proceso.NombreProceso + " Necesita de especificar una maquina,");
                    }
                   
                }
                if (proceso.UsaInsumos)
                {
                    var articulospl = new ArticulosPlaneacionEContext().ArticulosPlaneacionesE.Where(m => m.IDHE == idhe &&( m.IDTipoArticulo == 4 || m.IDTipoArticulo == 6 || m.IDTipoArticulo == 7) && m.IDProceso == proceso.IDProceso).ToList();
                    if (articulospl.Count == 0)
                    {
                        cadena2.Append("El proceso " + proceso.NombreProceso + " Necesita de especificar al menos un insumo,");
                    }
                }
                if (proceso.UsaHerramientas)
                {
                    var articulospl = new ArticulosPlaneacionEContext().ArticulosPlaneacionesE.Where(m => m.IDHE == idhe && m.IDTipoArticulo == 3 && m.IDProceso == proceso.IDProceso).ToList();
                    if (articulospl.Count == 0)
                    {
                        cadena2.Append("El proceso " + proceso.NombreProceso + " Necesita de especificar al menos una Herramienta,");
                    }

                }

            }

            if (cadena2.ToString().Length>247)
            {
                return RedirectToAction("Error", new { mensaje = cadena2.ToString().Substring(0,247) +"..."});
            }
            if (cadena2.ToString().Length > 2)
            {
                return RedirectToAction("Error", new { mensaje = cadena2.ToString() });
            }


            ViewBag.idhe = idhe;
            ViewBag.IDMoneda = new SelectList(new c_MonedaContext().c_Monedas, "IDMoneda", "Descripcion");
            
        

            return View();
        }


        [HttpPost]
        public ActionResult Calcular(int? idhe, int? IDMoneda, decimal TC)
        {
            HEspecificacionE hoja = new HEspecificacionEContext().HEspecificacionesE.Find(idhe);
            var articulospl = new ArticulosPlaneacionEContext().ArticulosPlaneacionesE.Where(m => m.IDHE == idhe).ToList();
            var rangos = new RangoPlaneacionContext().Rangos.Where(m => m.IDHE == idhe).ToList();


            int idmodelo = 0;

            string clavemonedacotizar = new c_MonedaContext().c_Monedas.Find(IDMoneda).ClaveMoneda;
            try
            {
                idmodelo = hoja.IDModeloProduccion;
            }
            catch (Exception err)
            {
                string mensaje = err.Message;
            }

           


            ProcesoContext procesobd = new ProcesoContext();

            List<Proceso> procesos = procesobd.Database.SqlQuery<Proceso>("select P.* from Proceso as P, ModeloProceso as M where M.ModelosDeProduccion_IDModeloProduccion=" + idmodelo + " order by M.orden").ToList();


            


            new CobroContext().Database.ExecuteSqlCommand("delete from [dbo].[RangoPlaneacionArticulo] where IDHE=" + hoja.IDHE);
            foreach (RangoPlaneacionCosto rango in rangos)
            {
                decimal Rangosup = 0;
                try
                {
                    Rangosup = rango.RangoSup;

                }
                catch (Exception err)
                {
                    string mensajeerror = err.Message;
                    return RedirectToAction("Error", new { mensaje = mensajeerror });
                }
                decimal acumulador = 0;


                foreach (ArticulosPlaneacionE articulop in articulospl)
                {
                    Formulas formula = new Formulas();
                    formula.cadenadepresentacion = hoja.Presentacion;
                    string forMU = articulop.Formuladerelacion;


                    ArticuloContext basa = new ArticuloContext();
                    SIAAPI.Models.Comercial.Articulo  articulo = basa.Articulo.Find(articulop.IDArticulo);

                    //Formulaespecializada FormulaE = new Formulaespecializada();



                    //Clsmanejocadena manejo = new Clsmanejocadena();
                    //FormulaE.productosalpaso = 1;
                    //FormulaE.anchoproductomm = 0;
                    //FormulaE.largoproductomm = 0;
                    //FormulaE.Cantidadxrollo = 1;
                    //FormulaE.hotstamping = false;
                    //FormulaE.conadhesivo = false;
                    //FormulaE.mangatermo = false;
                    //FormulaE.gapeje = 0;
                    //FormulaE.gapavance = 0;
                    ////   FormulaE.Cantidad = rango.RangoSup * 1000;
                    //if (idmodelo == 7 || idmodelo == 8)
                    //{
                    //    try
                    //    {
                    //        FormulaE.anchoproductomm = manejo.getint("ANCHO MANGA", hoja.ESPECIFICACION);
                    //    }
                    //    catch (Exception ERR)
                    //    {
                    //        string mensajeerror = ERR.Message;
                    //    }

                    //    try
                    //    {
                    //        FormulaE.largoproductomm = manejo.getint("LARGO MANGA", hoja.ESPECIFICACION);
                    //    }


                    //    catch (Exception ERR)
                    //    {
                    //        string mensajeerror = ERR.Message;
                    //    }
                    //    try
                    //    {
                    //        FormulaE.Cantidadxrollo = manejo.getint("ETIQUETAS POR PAQUETE", hoja.ESPECIFICACION);
                    //    }
                    //    catch (Exception ERR)
                    //    {
                    //        string mensajeerror = ERR.Message;
                    //    }

                    //    try
                    //    {
                    //        FormulaE.mangatermo = true;
                    //    }
                    //    catch (Exception ERR)
                    //    {
                    //        string mensajeerror = ERR.Message;
                    //    }

                    //     try
                    //    {
                    //        FormulaE.Numerodetintas = manejo.getint("NUMERO DE TINTAS", hoja.ESPECIFICACION);
                    //    }
                    //    catch (Exception ERR)
                    //    {
                    //        string mensajeerror = ERR.Message;
                    //    }

                    //}


                    //try
                    //{
                    //    FormulaE.productosalpaso = manejo.getint("AL PASO", hoja.ESPECIFICACION);
                    //    if (FormulaE.productosalpaso==0)
                    //    {
                    //        FormulaE.productosalpaso = 1;
                    //    }
                    //}
                    //catch (Exception ERR)
                    //{
                       
                    //    string mensajeerror = ERR.Message;
                    //    return RedirectToAction("Error", new { mensaje = mensajeerror });
                    //}

                    //if (idmodelo >= 4 && idmodelo <= 6)
                    //{
                    //    try
                    //    {
                    //        if (FormulaE.anchoproductomm == 0)
                    //        {
                    //            FormulaE.anchoproductomm = manejo.getint("EJE", hoja.ESPECIFICACION);
                    //        }
                    //    }
                    //    catch (Exception ERR)
                    //    {
                    //        string mensajeerror = ERR.Message;
                    //    }
                    //    try
                    //    {
                    //        if (FormulaE.anchoproductomm == 0)
                    //        {
                    //            FormulaE.largoproductomm = manejo.getint("AVANCE", hoja.ESPECIFICACION);
                    //        }
                    //    }

                    //    catch (Exception ERR)
                    //    {
                    //        string mensajeerror = ERR.Message;
                    //    }

                    //    try
                    //    {
                    //        FormulaE.Cantidadxrollo = manejo.getint("ETIQUETAS X P", hoja.ESPECIFICACION);
                    //    }
                    //    catch (Exception ERR)
                    //    {
                    //        string mensajeerror = ERR.Message;
                    //    }
                    //    try
                    //    {
                    //        FormulaE.mangatermo = false;
                    //    }
                    //    catch (Exception ERR)
                    //    {
                    //        string mensajeerror = ERR.Message;
                    //    }

                    //    try
                    //    {
                    //        FormulaE.Numerodetintas = manejo.getint("NO DE TINTAS", hoja.ESPECIFICACION);
                    //    }
                    //    catch (Exception ERR)
                    //    {
                    //        string mensajeerror = ERR.Message;
                    //    }

                    //}



                    //try
                    //{
                    //    FormulaE.hotstamping = manejo.getbool("HOTSTAMPING", hoja.ESPECIFICACION);
                    //}
                    //catch (Exception ERR)
                    //{
                    //    string mensajeerror = ERR.Message;
                    //}
                    


                    //try
                    //{
                    //    FormulaE.gapeje = manejo.getint("GAP EJE", hoja.ESPECIFICACION);
                    //}
                    //catch (Exception ERR)
                    //{
                    //    string mensajeerror = ERR.Message;
                    //}

                    //try
                    //{
                    //    FormulaE.gapavance = manejo.getint("GAP AVANCE", hoja.ESPECIFICACION);
                    //}
                    //catch (Exception ERR)
                    //{
                    //    string mensajeerror = ERR.Message;
                    //}


                    //try
                    //{
                    //    FormulaE.conadhesivo = manejo.getbool("ADHESIVO", hoja.ESPECIFICACION);
                    //}
                    //catch (Exception ERR)
                    //{
                    //    string mensajeerror = ERR.Message;
                    //}
                    // estancias formula especiualizada
                    //  ***416

                    /// aguas aqui va la formulas especializadas

                    //FormulaE.conadhesivo = hoja.Adhesivo;
                    //FormulaE.hotstamping = hoja.HotStamping;
                    //FormulaE.mangatermo = hoja.MangaTermoencogible;
                    //FormulaE.Numerodetintas = hoja.NoTintas;

                    //FormulaE.Cantidad = Rangosup;

                    ////private decimal MaterialNecesitado = 0;
                    ////public decimal CantidadMP = 0;

                    //FormulaE.Calcular();

                    //pasar parametros de la hoja de especificacion a las propiedades de la clase

                    // verificar si hay una formula predeterminada dentro detro de forMU 


               //     decimal formulaporproducto = 0;
               //     // yield sustituyes la formula por la cantudad que te de el metodo getHorade(formula)
               ////     Formulaespecializada fe = new Formulaespecializada();
               //     bool contiene = FormulaE.Contiene(forMU);


               //     if (contiene == true)
               //     {
               //         string quecontiene = FormulaE.queContiene(forMU);

               //         if (quecontiene=="KgtintaArea(")
               //         {
               //             int posicion = 0;
               //             int posicionf = 0;
               //             posicion = forMU.IndexOf(quecontiene);
               //             posicionf = forMU.IndexOf(")", posicion);
               //             string cadena = forMU.Substring(posicion,  posicionf+1);
               //             quecontiene = cadena;
               //         }


               //         formulaporproducto = FormulaE.getHorade(quecontiene);

               //         forMU = forMU.Replace(quecontiene, formulaporproducto.ToString());


               //     }
               //     else
               //     {

               //     }

                    string formulanueva = formula.sustituircontenidocadena(forMU, double.Parse(Rangosup.ToString()));
                    double valorfin = formula.Calcular(formulanueva, double.Parse(articulop.factorcierre.ToString()));


                    double costo = 0;
                    try
                    {
                        //ClsDatoDecimal cuanto = new CobroContext().Database.SqlQuery<ClsDatoDecimal>("SELECT [dbo].[GetCosto] (0," + articulo.IDArticulo + "," + valorfin + ") as Dato ").ToList()[0];
                        ClsDatoDecimal cuanto = new CobroContext().Database.SqlQuery<ClsDatoDecimal>("SELECT [dbo].[GetCosto] (" + articulo.IDArticulo + "," + valorfin + ") as Dato ").ToList()[0];
                        costo = double.Parse(cuanto.Dato.ToString());
                    }
                    catch (Exception err)
                    {
                        string mensaje = err.Message;
                    }


                  

                    decimal tcc = TC;
                    if (articulo.IDMoneda == IDMoneda)
                    {
                        tcc = 1;
                    }

                    if (valorfin >= 0)
                    {
                        costo = costo * valorfin;
                    }


                    decimal costofinal = 0;



                    if (articulo.c_Moneda.ClaveMoneda=="MXN" && clavemonedacotizar=="USD")
                    {
                        costofinal = (decimal)costo / tcc;
                    }    
                    if (articulo.c_Moneda.ClaveMoneda == "USD" && clavemonedacotizar == "MXN")
                    {
                        costofinal = (decimal)costo * tcc;
                    }
                    if (articulo.c_Moneda.ClaveMoneda ==  clavemonedacotizar)
                    {
                        costofinal = (decimal)costo ;
                    }

                    new CobroContext().Database.ExecuteSqlCommand("INSERT INTO [dbo].[RangoPlaneacionArticulo] ([IDHE],[RangoInf],[RangoSup],[Costo],[Version],[IDArticulo],[IDTipoArticulo],[IDMoneda],[IDProceso],[TC],cantidad) VALUES (" + hoja.IDHE + "," + rango.RangoInf + "," + rango.RangoSup + "," + costofinal + "," + hoja.Version + "," + articulo.IDArticulo + "," + articulo.IDTipoArticulo + "," + IDMoneda + "," + articulop.IDProceso + ","+ tcc + ","+valorfin+")");

                    /////   aqui vamos a traer el costo del articulo /////
                    acumulador = acumulador + costofinal;

                }
                // fin del for
                double costouni = Math.Round(double.Parse(acumulador.ToString()) / double.Parse(Rangosup.ToString()), 2);
                new CobroContext().Database.ExecuteSqlCommand("UPDATE [dbo].[RangoPlaneacionCosto] SET costo=" + costouni + ", IDMoneda="+IDMoneda+" where IDRP=" + rango.IDRP);
               
                //new CobroContext().Database.ExecuteSqlCommand("UPDATE [dbo].[RangoPlaneacionCosto] SET precio=" + acum + " where IDRP=" + rango.IDRP);
            }

            for (int i = 1; i < 5; i++)
            {
                HEspecificacionE hespecificacionE = db.HEspecificacionesE.Find(idhe);
                // List<RangoPlaneacionArticuloN> rangosnivel = new FamiliaContext().Database.SqlQuery<RangoPlaneacionArticuloN>("select RangoPlaneacionArticulo.IDMoneda,RangoPlaneacionArticulo.TC,(select Porcentaje from NivelesGanancia where IDFamilia=" + hoja.IDFamilia + " and Nivel=" + i + ") as Porcentaje,RangoPlaneacionArticulo.RangoInf,RangoPlaneacionArticulo.RangoSup,(sum(Costo)/RangoPlaneacionArticulo.RangoSup*((select Porcentaje from NivelesGanancia where IDFamilia=" + hespecificacionE.IDFamilia + " and Nivel=" + i + ")/100))+sum(Costo)/RangoPlaneacionArticulo.RangoSup as Costo from RangoPlaneacionArticulo inner join Articulo as A on A.IDArticulo=RangoPlaneacionArticulo.IDArticulo inner join TipoArticulo as TA on TA.IDTipoArticulo=RangoPlaneacionArticulo.IDTipoArticulo where RangoPlaneacionArticulo.IDHE=" + idhe + " group by RangoPlaneacionArticulo.RangoInf,RangoPlaneacionArticulo.RangoSup,RangoPlaneacionArticulo.IDMoneda,RangoPlaneacionArticulo.TC").ToList();
                //string cadena1 = "select (select RangoPlaneacionArticulo.IDMoneda,RangoPlaneacionArticulo.TC,Porcentaje from NivelesGanancia where IDFamilia = " + hoja.IDFamilia + " and Nivel = " + i + ") as Porcentaje,RangoPlaneacionArticulo.RangoInf,RangoPlaneacionArticulo.RangoSup,((sum(Costo) / RangoPlaneacionArticulo.RangoSup)) / (((100 - (select Porcentaje from NivelesGanancia where IDFamilia = " + hespecificacionE.IDFamilia + " and Nivel = " + i + ")))/100) from RangoPlaneacionArticulo inner join Articulo as A on A.IDArticulo = RangoPlaneacionArticulo.IDArticulo inner join TipoArticulo as TA on TA.IDTipoArticulo = RangoPlaneacionArticulo.IDTipoArticulo where RangoPlaneacionArticulo.IDHE = " + idhe + " group by RangoPlaneacionArticulo.RangoInf,RangoPlaneacionArticulo.RangoSup,RangoPlaneacionArticulo.IDMoneda,RangoPlaneacionArticulo.TC ";
                string cadena1 = "SELECT NivelesGanancia.NIVEL as nivel,RangoPlaneacionArticulo.RangoInf,RangoPlaneacionArticulo.RangoSuP, NivelesGanancia.Porcentaje,(sum(Costo) / RangoPlaneacionArticulo.RangoSup) AS Costo, ((sum(Costo) / RangoPlaneacionArticulo.RangoSup)) / ((100 - (NivelesGanancia.Porcentaje)) / 100) as Precio  from RangoPlaneacionArticulo, NivelesGanancia where RangoPlaneacionArticulo.IDHE = " + idhe + "AND IDFamilia =" + hespecificacionE.IDFamilia + " group by NivelesGanancia.NIVEL, RangoPlaneacionArticulo.RangoInf, RangoPlaneacionArticulo.RangoSuP, NivelesGanancia.Porcentaje; ";
                List<RangoPlaneacionArticuloN> rangosnivel = new FamiliaContext().Database.SqlQuery<RangoPlaneacionArticuloN>(cadena1).ToList();

                try
                {
                    new CobroContext().Database.ExecuteSqlCommand("delete from RangoPlaneacionNiveles where idhe=" + idhe);
                }

                catch(Exception err)
                {
                    string error = err.Message;

                }
                foreach (var item in rangosnivel)
                {

                    new CobroContext().Database.ExecuteSqlCommand("insert into RangoPlaneacionNiveles ([IDHE],[RangoInf],[RangoSup],[Costo],[Porcentaje],[IDMoneda],[TC],[Nivel],Precio) values (" + idhe + "," + item.RangoInf + "," + item.RangoSup + "," + item.Costo + "," + item.Porcentaje + "," + IDMoneda + "," + TC + "," + item.Nivel + "," + item.Precio+")");
                }


            }
            return RedirectToAction("VerPrecio",new { idhe=idhe});
        }
       
        public ActionResult AsignarP(int? id)
        {
            HEspecificacionE hespecificacion = db.HEspecificacionesE.Find(id);
            db.Database.ExecuteSqlCommand("delete from [dbo].[CaracteristicaPlaneacionE] where IDCliente=" + hespecificacion.IDCliente + " and IDArticulo=" + hespecificacion.IDArticulo + "and IDCaracteristica=" + hespecificacion.IDCaracteristica + " and IDHE=" + id );
            db.Database.ExecuteSqlCommand("INSERT INTO [dbo].[CaracteristicaPlaneacionE]([IDCaracteristica],[IDArticulo],[Planeacion],[Version],[IDHE],[IDCliente])VALUES(" + hespecificacion.IDCaracteristica + "," + hespecificacion.IDArticulo + "," + hespecificacion.Planeacion + "," + hespecificacion.Version + "," + id + "," + hespecificacion.IDCliente + ")");
            db.Database.ExecuteSqlCommand("update Caracteristica set cotizacion=" + hespecificacion.Planeacion + ", version=" + hespecificacion.Version + " WHERE CARACTERISTICA.ID=" + hespecificacion.IDCaracteristica);
            return RedirectToAction("IndexHE");
            //HEspecificacionE hespecificacion = db.HEspecificacionesE.Find(id);
            //new VCaracteristicaContext().Database.ExecuteSqlCommand("update [dbo].[Caracteristica] SET Cotizacion=" + hespecificacion.Planeacion + ",version=" + hespecificacion.Version + " where ID=" + hespecificacion.IDCaracteristica);
            //return RedirectToAction("IndexHE");
        }

        public ActionResult Documento(int? idhe,int? idproceso)
        {
            ViewBag.idhe = idhe;
            ViewBag.idproceso = idproceso;
            return View();
        }
        [HttpPost]
        public ActionResult Documento(HttpPostedFileBase file, int? idhe,int?idproceso,string Descripcion)
        {
            HEspecificacionE hespecificacione = new HEspecificacionEContext().HEspecificacionesE.Find(idhe);
            string extension = Path.GetExtension(Request.Files[0].FileName).ToLower();

            HttpPostedFileBase archivo = Request.Files["file"];
            try
            {

           
            if (file != null && file.ContentLength > 0)
            {
                //if (extension == ".pdf")
                //{
                    var nombreArchivo = Path.GetFileName(file.FileName);
                    var path = Path.Combine(Server.MapPath("~/Documentos"), nombreArchivo);
                    

                    string nameArchivo = file.FileName;

                    db.Database.ExecuteSqlCommand("INSERT INTO [dbo].[DocumentoE] ([IDHE],[Version],[IDProceso],[Descripcion],[Planeacion],[Nombre]) VALUES('" + idhe + "','" + hespecificacione.Version + "','" + idproceso + "','" + Descripcion + "','" + hespecificacione.Planeacion + "','" + nameArchivo + "')");
                    file.SaveAs(path);

                    //}
                    //else
                    //{
                    //    ViewBag.Mensajeerror = "No se pudo subir el archivo";
                    //}

                }
            }
            catch (Exception err)
            {
                string mensaje = err.Message;
                ViewBag.Mensajeerror = "No se pudo subir el archivo";
                return View();
            }

            return RedirectToAction("ArticuloPlaneacion", new { id = idproceso });


        }

        public FileResult DescargarDoc(int id)
        {
           DocumentoEContext pf = new DocumentoEContext();
            var elemento = pf.DocumentosE.Single(m => m.IDDocumento == id);

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
           
                DocumentoEContext pf = new DocumentoEContext();

                var elemento = pf.DocumentosE.Single(m => m.IDDocumento == id);
                var file = Path.Combine(Server.MapPath("~/Documentos/" + elemento.Nombre));
                System.IO.File.Delete(file);

                ArticulosPlaneacionEContext car = new ArticulosPlaneacionEContext();
                car.Database.ExecuteSqlCommand("delete from DocumentoE where IDDocumento=" + id);
                return Json(true);
            }
            catch (Exception err)
            {
                return Json(500, err.Message);
            }
        }
        //////////////////////////Reporte/////////

     

        ////////////////////FECHA A FECHA//////
        public ActionResult CreaReporteporfecha()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreaReporteporfecha(Reportefechas modelo)
        {
            ReportDocument reporte = new ReportDocument();

            reporte.Load(Path.Combine(Server.MapPath("~/reportes/Crystal/Produccion"), "PlanProduccionE.rpt"));

            string servidor = Conexion.Darvalordelaconexion("data source", "DefaultConnection");
            string basededatos = Conexion.Darvalordelaconexion("initial catalog", "DefaultConnection");
            string usuario = Conexion.Darvalordelaconexion("user id", "DefaultConnection");
            string pass = Conexion.Darvalordelaconexion("password", "DefaultConnection");


            reporte.DataSourceConnections[0].SetConnection(@servidor, basededatos, false);
            reporte.DataSourceConnections[0].SetLogon(usuario, pass);


            reporte.SetParameterValue("fechaini", modelo.Fechainicio.ToShortDateString());
            reporte.SetParameterValue("fechafin", modelo.Fechafinal.ToShortDateString());


            string ENCABEZADO = "Fecha inicial " + modelo.Fechainicio.ToShortDateString() + " Fecha final " + modelo.Fechafinal.ToShortDateString();

            reporte.DataDefinition.FormulaFields["Encabezado"].Text = "'" + ENCABEZADO + "'";
            reporte.DataDefinition.FormulaFields["Nota"].Text = "'" + modelo.Nota + "'";



            Response.Buffer = false;

            Response.ClearContent();
            Response.ClearHeaders();

            try
            {
                Stream stream = reporte.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                stream.Seek(0, SeekOrigin.Begin);
                return File(stream, "application/pdf", "Reporte de Plan de Produccion.pdf");


            }
            catch (Exception err)
            {

            }
            return Redirect("index");
        }







        /////////////////////////////////REPORTE POR Modelo Produccion/////////////////////////////////////////////////////////////////////////////////////


        public ActionResult CreaReporteporModelo()
        {
            //Buscar Cliente
            ModelosDeProduccionContext dbc = new ModelosDeProduccionContext();
            var modelo = dbc.ModelosDeProducciones.OrderBy(m => m.Descripcion).ToList();


            List<SelectListItem> listaModelo = new List<SelectListItem>();
            listaModelo.Add(new SelectListItem { Text = "--Selecciona Modelo--", Value = "0" });

            foreach (var m in modelo)
            {
                listaModelo.Add(new SelectListItem { Text = m.Descripcion, Value = m.IDModeloProduccion.ToString() });


            }
            ViewBag.modelo = listaModelo;
            return View();
        }

        [HttpPost]
        public ActionResult CreaReporteporModelo(Reportefeno modelo)
        {


            int idmod = modelo.IDModeloProduccion;
            ModelosDeProduccionContext dbc = new ModelosDeProduccionContext();
            ModelosDeProduccion mproceso = dbc.ModelosDeProducciones.Find(modelo.IDModeloProduccion);

            ReportDocument reporte = new ReportDocument();

            reporte.Load(Path.Combine(Server.MapPath("~/reportes/Crystal/Produccion"), "PlanProduccion.rpt"));

            string servidor = Conexion.Darvalordelaconexion("data source", "DefaultConnection");
            string basededatos = Conexion.Darvalordelaconexion("initial catalog", "DefaultConnection");
            string usuario = Conexion.Darvalordelaconexion("user id", "DefaultConnection");
            string pass = Conexion.Darvalordelaconexion("password", "DefaultConnection");


            reporte.DataSourceConnections[0].SetConnection(@servidor, basededatos, false);
            reporte.DataSourceConnections[0].SetLogon(usuario, pass);

            reporte.DataDefinition.RecordSelectionFormula = "{ VHEspCte.FechaEspecificacion}>={?fechaini} and { VHEspCte.FechaEspecificacion}<={?fechafin} and {ModelosDeProduccion.IDModeloProduccion} =" + modelo.IDModeloProduccion;




            reporte.SetParameterValue("fechaini", modelo.Fechainicio.ToShortDateString());
            reporte.SetParameterValue("fechafin", modelo.Fechafinal.ToShortDateString());



            string ENCABEZADO = "Fecha inicial " + modelo.Fechainicio.ToShortDateString() + " Fecha final " + modelo.Fechafinal.ToShortDateString() + " MODELO " + mproceso.Descripcion;

            reporte.DataDefinition.FormulaFields["Encabezado"].Text = "'" + ENCABEZADO + "'";
            reporte.DataDefinition.FormulaFields["Nota"].Text = "'" + modelo.Nota + "'";



            Response.Buffer = false;

            Response.ClearContent();
            Response.ClearHeaders();

            try
            {
                Stream stream = reporte.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                stream.Seek(0, SeekOrigin.Begin);
                return File(stream, "application/pdf", "Reporte de Plan de Produccion.pdf");

            }
            catch (Exception err)
            {

            }
            return Redirect("index");
        }




        /////////////////////////////////REPORTE POR FAMILIA/////////////////////////////////////////////////////////////////////////////////////


        public ActionResult CreaReporteporFamilia()
        {
            //Buscar Cliente
            FamiliaContext dbc = new FamiliaContext();

            var modelo = dbc.Familias.OrderBy(m => m.Descripcion).ToList();


            List<SelectListItem> listaModelo = new List<SelectListItem>();
            listaModelo.Add(new SelectListItem { Text = "--Selecciona Familia--", Value = "0" });

            foreach (var m in modelo)
            {
                listaModelo.Add(new SelectListItem { Text = m.Descripcion, Value = m.IDFamilia.ToString() });


            }
            ViewBag.modelo = listaModelo;
            return View();
        }

        [HttpPost]
        public ActionResult CreaReporteporFamilia(Reportefeno modelo)
        {


            int idfam = modelo.IDFamilia;
            FamiliaContext dbc = new FamiliaContext();
            Familia mfam = dbc.Familias.Find(modelo.IDFamilia);

            ReportDocument reporte = new ReportDocument();

            reporte.Load(Path.Combine(Server.MapPath("~/reportes/Crystal/Produccion"), "PlanProduccion.rpt"));

            string servidor = Conexion.Darvalordelaconexion("data source", "DefaultConnection");
            string basededatos = Conexion.Darvalordelaconexion("initial catalog", "DefaultConnection");
            string usuario = Conexion.Darvalordelaconexion("user id", "DefaultConnection");
            string pass = Conexion.Darvalordelaconexion("password", "DefaultConnection");


            reporte.DataSourceConnections[0].SetConnection(@servidor, basededatos, false);
            reporte.DataSourceConnections[0].SetLogon(usuario, pass);

            reporte.DataDefinition.RecordSelectionFormula = "{ VHEspCte.FechaEspecificacion}>={?fechaini} and { VHEspCte.FechaEspecificacion}<={?fechafin} and {Familia.IDFamilia} =" + modelo.IDFamilia;




            reporte.SetParameterValue("fechaini", modelo.Fechainicio.ToShortDateString());
            reporte.SetParameterValue("fechafin", modelo.Fechafinal.ToShortDateString());



            string ENCABEZADO = "Fecha inicial " + modelo.Fechainicio.ToShortDateString() + " Fecha final " + modelo.Fechafinal.ToShortDateString() + " FAMILIA " + mfam.Descripcion;

            reporte.DataDefinition.FormulaFields["Encabezado"].Text = "'" + ENCABEZADO + "'";
            reporte.DataDefinition.FormulaFields["Nota"].Text = "'" + modelo.Nota + "'";



            Response.Buffer = false;

            Response.ClearContent();
            Response.ClearHeaders();

            try
            {
                Stream stream = reporte.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                stream.Seek(0, SeekOrigin.Begin);
                return File(stream, "application/pdf", "Reporte de Plan de Produccion.pdf");

            }
            catch (Exception err)
            {

            }
            return Redirect("index");
        }


 
        public ActionResult Error(string mensaje)
        {
            ViewBag.Error = mensaje;
            return View();

        }


        public ActionResult InsertarSugerido(int Rango, int id)
        {

            try
            { 
            int idhe = id;
            ViewBag.idhe = idhe;
            HEspecificacionE hespecificacion = new HEspecificacionEContext().HEspecificacionesE.Find(idhe);
            ViewBag.Articulo = hespecificacion.Descripcion;
            ViewBag.Presentacion = hespecificacion.Presentacion.Replace(",", " ");
            List<VRangoPlaneacionNiveles> rangonivel = db.Database.SqlQuery<VRangoPlaneacionNiveles>("select HE.IDCliente, RPN.IDMoneda, RPN.TC, RPN.IDRPN, RPN.RangoInf, RPN.RangoSup, RPN.Precio as costo, RPN.Porcentaje, HE.Descripcion, HE.Presentacion, Nivel from RangoPlaneacionNiveles as RPN inner join HEspecificacionE as HE on RPN.IDHE = HE.IDHE  where HE.IDHE =" + idhe + " and Nivel=" + Rango + " group by HE.IDCliente, RPN.IDMoneda, RPN.TC, RPN.IDRPN, RPN.RangoInf, RPN.RangoSup, RPN.Precio, RPN.Porcentaje, HE.Descripcion, HE.Presentacion, Nivel order by RPN.RangoInf").ToList();
            List<VRangoPlaneacionCosto> costonivel = db.Database.SqlQuery<VRangoPlaneacionCosto>("select RangoPlaneacionArticulo.RangoInf,RangoPlaneacionArticulo.RangoSup,sum(Costo)/RangoPlaneacionArticulo.RangoSup as Costo from RangoPlaneacionArticulo inner join Articulo as A on A.IDArticulo=RangoPlaneacionArticulo.IDArticulo inner join TipoArticulo as TA on TA.IDTipoArticulo=RangoPlaneacionArticulo.IDTipoArticulo where RangoPlaneacionArticulo.IDHE=" + idhe + " group by RangoPlaneacionArticulo.RangoInf,RangoPlaneacionArticulo.RangoSup").ToList();




            if (rangonivel.Count == 0)
            {
                return RedirectToAction("Error", new { mensaje = "No se tiene el nivel " });
            }
            //  List<VRangoPlaneacionNiveles> rangonivel = db.Database.SqlQuery<VRangoPlaneacionNiveles>("select distinct(Rangoinf),RangoSup from VRangoNiveles where IDHE=" + idhe + "").ToList();
            ViewBag.rangoniveles = rangonivel;

            ClsDatoEntero monedapsugerido = db.Database.SqlQuery<ClsDatoEntero>("select distinct(M.IDMoneda) as Dato from RangoPlaneacionNiveles as RPN inner join HEspecificacionE as HE on RPN.IDHE=HE.IDHE inner join c_Moneda as M on M.IDMoneda=RPN.IDMoneda where HE.IDHE=" + idhe + "").ToList()[0];
            int monedaarticulo = monedapsugerido.Dato;

            db.Database.ExecuteSqlCommand("update Articulo set IDMoneda=" + monedaarticulo + " where IDArticulo=" + hespecificacion.IDArticulo);

                List<MatrizCosto> mc = new ArticuloContext().Database.SqlQuery<MatrizCosto>("select * from MAtrizCosto where IDArticulo=" + hespecificacion.IDArticulo).ToList();


                if (mc.Count == 0)
                {


                    foreach (VRangoPlaneacionCosto rangoc in costonivel)
                    {

                        db.Database.ExecuteSqlCommand("insert into MatrizCosto (RangInf, RangSup, Precio, IDArticulo) values (" + rangoc.RangoInf + "," + rangoc.RangoSup + "," + rangoc.Costo + "," + hespecificacion.IDArticulo + ");");
                        // db.Database.ExecuteSqlCommand("insert into MatrizPrecio  (RangoInf, RangoSup, Precio, IDArticulo) values (" + rangoc.RangoInf + "," + rangoc.RangoSup + "," + rangoc.Costo + "," + hespecificacion.IDArticulo + ");");
                    }

                }





                if (hespecificacion.IDCliente == 0)
            {
                   
                    //
                    db.Database.ExecuteSqlCommand(" delete from MatrizPrecio where IDArticulo= " + hespecificacion.IDArticulo);



                foreach (VRangoPlaneacionNiveles rango in rangonivel)
                {

                    //  db.Database.ExecuteSqlCommand("insert into MatrizCosto (RangoInf, RangoSup, Precio, IDArticulo) values ("+ rango.RangoInf +"," + rango.RangoSup + "," + rango.Costo +"," + hespecificacion.IDArticulo +");");
                    db.Database.ExecuteSqlCommand("insert into MatrizPrecio  (RangoInf, RangoSup, Precio, IDArticulo) values (" + rango.RangoInf + "," + rango.RangoSup + "," + rango.Costo + "," + hespecificacion.IDArticulo + ");");
                }
            }
            if (hespecificacion.IDCliente > 0)
            {
                //  db.Database.ExecuteSqlCommand(" delete from MatrizCosto where IDArticulo= " + hespecificacion.IDArticulo);
                db.Database.ExecuteSqlCommand(" delete from MatrizPrecioCliente where IDArticulo= " + hespecificacion.IDArticulo +" and IDCliente =" + hespecificacion.IDCliente);



                foreach (VRangoPlaneacionNiveles rango in rangonivel)
                {

                    //  db.Database.ExecuteSqlCommand("insert into MatrizCosto (RangoInf, RangoSup, Precio, IDArticulo) values ("+ rango.RangoInf +"," + rango.RangoSup + "," + rango.Costo +"," + hespecificacion.IDArticulo +");");
                    db.Database.ExecuteSqlCommand("insert into MatrizPrecioCliente  (RangInf, RangSup, Precio, IDArticulo,IDCliente) values (" + rango.RangoInf + "," + rango.RangoSup + "," + rango.Costo + "," + hespecificacion.IDArticulo + "," + hespecificacion.IDCliente +");");
                }

            }


                if (hespecificacion.IDCliente == 0)
                {
                    SIAAPI.Models.Comercial.Articulo Articulo  = new ArticuloContext().Articulo.Find(hespecificacion.IDArticulo);
                    return Content("<html><body>tu Matriz prrecio se ha asignado correctamente a " + hespecificacion.Descripcion +" </body></html>", "text/html");
                }
                else
                {
                    Clientes x = new ClientesContext().Clientes.Find(hespecificacion.IDCliente);
                    return Content("<html><body>tu Matriz precio se ha asignado correctamente a " + x.Nombre + " </body></html>", "text/html");
                }
            }
            catch (Exception err)
            {
                return Content("<html><body>Error al asignar correctamente a " + err.Message + " </body></html>", "text/html");
            }
           


        }


        public ActionResult InsertarSugeridoA(int Rango, int id)
        {

            try
            {
                int idhe = id;
                ViewBag.idhe = idhe;
                HEspecificacionE hespecificacion = new HEspecificacionEContext().HEspecificacionesE.Find(idhe);
                ViewBag.Articulo = hespecificacion.Descripcion;
                ViewBag.Presentacion = hespecificacion.Presentacion.Replace(",", " ");
                List<VRangoPlaneacionNiveles> rangonivel = db.Database.SqlQuery<VRangoPlaneacionNiveles>("select HE.IDCliente, RPN.IDMoneda, RPN.TC, RPN.IDRPN, RPN.RangoInf, RPN.RangoSup, RPN.Precio as costo, RPN.Porcentaje, HE.Descripcion, HE.Presentacion, Nivel from RangoPlaneacionNiveles as RPN inner join HEspecificacionE as HE on RPN.IDHE = HE.IDHE  where HE.IDHE =" + idhe + " and Nivel=" + Rango + " group by HE.IDCliente, RPN.IDMoneda, RPN.TC, RPN.IDRPN, RPN.RangoInf, RPN.RangoSup, RPN.Precio, RPN.Porcentaje, HE.Descripcion, HE.Presentacion, Nivel order by RPN.RangoInf").ToList();
                List<VRangoPlaneacionCosto> costonivel = db.Database.SqlQuery<VRangoPlaneacionCosto>("select RangoPlaneacionArticulo.RangoInf,RangoPlaneacionArticulo.RangoSup,sum(Costo)/RangoPlaneacionArticulo.RangoSup as Costo from RangoPlaneacionArticulo inner join Articulo as A on A.IDArticulo=RangoPlaneacionArticulo.IDArticulo inner join TipoArticulo as TA on TA.IDTipoArticulo=RangoPlaneacionArticulo.IDTipoArticulo where RangoPlaneacionArticulo.IDHE=" + idhe + " group by RangoPlaneacionArticulo.RangoInf,RangoPlaneacionArticulo.RangoSup").ToList();




                if (rangonivel.Count == 0)
                {
                    return RedirectToAction("Error", new { mensaje = "No se tiene el nivel " });
                }
                //  List<VRangoPlaneacionNiveles> rangonivel = db.Database.SqlQuery<VRangoPlaneacionNiveles>("select distinct(Rangoinf),RangoSup from VRangoNiveles where IDHE=" + idhe + "").ToList();
                ViewBag.rangoniveles = rangonivel;

                ClsDatoEntero monedapsugerido = db.Database.SqlQuery<ClsDatoEntero>("select distinct(M.IDMoneda) as Dato from RangoPlaneacionNiveles as RPN inner join HEspecificacionE as HE on RPN.IDHE=HE.IDHE inner join c_Moneda as M on M.IDMoneda=RPN.IDMoneda where HE.IDHE=" + idhe + "").ToList()[0];
                int monedaarticulo = monedapsugerido.Dato;

                db.Database.ExecuteSqlCommand("update Articulo set IDMoneda=" + monedaarticulo + " where IDArticulo=" + hespecificacion.IDArticulo);

                List<MatrizCosto> mc = new ArticuloContext().Database.SqlQuery<MatrizCosto>("select * from MAtrizCosto where IDArticulo=" + hespecificacion.IDArticulo).ToList();


                if (mc.Count == 0)
                {


                    foreach (VRangoPlaneacionCosto rangoc in costonivel)
                    {

                        db.Database.ExecuteSqlCommand("insert into MatrizCosto (RangInf, RangSup, Precio, IDArticulo) values (" + rangoc.RangoInf + "," + rangoc.RangoSup + "," + rangoc.Costo + "," + hespecificacion.IDArticulo + ");");
                        // db.Database.ExecuteSqlCommand("insert into MatrizPrecio  (RangoInf, RangoSup, Precio, IDArticulo) values (" + rangoc.RangoInf + "," + rangoc.RangoSup + "," + rangoc.Costo + "," + hespecificacion.IDArticulo + ");");
                    }

                }


             
                    db.Database.ExecuteSqlCommand(" delete from MatrizPrecio where IDArticulo= " + hespecificacion.IDArticulo);



                    foreach (VRangoPlaneacionNiveles rango in rangonivel)
                    {

                        //  db.Database.ExecuteSqlCommand("insert into MatrizCosto (RangoInf, RangoSup, Precio, IDArticulo) values ("+ rango.RangoInf +"," + rango.RangoSup + "," + rango.Costo +"," + hespecificacion.IDArticulo +");");
                        db.Database.ExecuteSqlCommand("insert into MatrizPrecio  (RangInf, RangSup, Precio, IDArticulo) values (" + rango.RangoInf + "," + rango.RangoSup + "," + rango.Costo + "," + hespecificacion.IDArticulo + ");");
                    }




                SIAAPI.Models.Comercial.Articulo articulo = new ArticuloContext().Articulo.Find(hespecificacion.IDArticulo);
                try
                {
                    db.Database.ExecuteSqlCommand("update Caracteristica set cotizacion= " + hespecificacion.Planeacion + ", version =" + hespecificacion.Version + " where id =" + hespecificacion.IDCaracteristica);
                }
                catch(Exception err)
                {
                    string error = err.Message;
                }  
                    return Content("<html><body>tu Matriz prrecio se ha asignado correctamente a " + articulo.Descripcion + " </body></html>", "text/html");
                
                  
           
            }
            catch (Exception err)
            {
                return Content("<html><body>Error al asignar correctamente a " + err.Message + " </body></html>", "text/html");
            }



        }



        public ActionResult CotizadorRapido()
        {
            return View();
        }



        public ActionResult CambiarMaquinaPlaneacion(int id, int idmaquinanueva, int proceso)
        {

            try
            {
                Models.Comercial.Caracteristica caracte = db.Database.SqlQuery<Models.Comercial.Caracteristica>("select * from Caracteristica where Articulo_IDArticulo=" + idmaquinanueva).ToList().FirstOrDefault();
                string cadena = "update ArticulosPlaneacionE set IdArticulo=" + idmaquinanueva + ", IDCaracteristica=" + caracte.ID + " where IDArtPlan=" + id + " and IDProceso=" + proceso;
                db.Database.ExecuteSqlCommand(cadena);
                return Json(new HttpStatusCodeResult(200, "La maquina fue cambiada exitosamente!"));
            }
            catch (Exception err)
            {
                string mensajederror = err.Message;
                return Json(new HttpStatusCodeResult(500, "No Pude"));
            }




            //db.Database.ExecuteSqlCommand("update OrdenProduccion set Prioridad=" + prioridadabajo + " where IDOrden=" + idordencambio + "");

            //db.Database.ExecuteSqlCommand("update OrdenProduccion set Prioridad=" + prioridadarriba + " where IDOrden=" + id + "");



        }

        public ActionResult CambiarCintaPlaneacion(int id, int idcintanueva, int proceso)
        {

            try
            {
                Models.Comercial.Caracteristica caracte = db.Database.SqlQuery<Models.Comercial.Caracteristica>("select * from Caracteristica where ID=" + idcintanueva).ToList().FirstOrDefault();
                string cadena = "update ArticulosPlaneacionE set IdArticulo=" + caracte.Articulo_IDArticulo + ", IDCaracteristica=" + caracte.ID + " where IDArtPlan=" + id + " and IDProceso=" + proceso;
                db.Database.ExecuteSqlCommand(cadena);
                return Json(new HttpStatusCodeResult(200, "La maquina fue cambiada exitosamente!"));
            }
            catch (Exception err)
            {
                string mensajederror = err.Message;
                return Json(new HttpStatusCodeResult(500, "No Pude"));
            }




            //db.Database.ExecuteSqlCommand("update OrdenProduccion set Prioridad=" + prioridadabajo + " where IDOrden=" + idordencambio + "");

            //db.Database.ExecuteSqlCommand("update OrdenProduccion set Prioridad=" + prioridadarriba + " where IDOrden=" + id + "");



        }

        [HttpPost]
        public JsonResult EditFormula(int id, string Formuladerelacion, decimal factorcierre)
        {
                       
            try
            {
                //System.Diagnostics.Debug.WriteLine("Cantidad a cambiar " + cantidad);
                ArticulosPlaneacionEContext car = new ArticulosPlaneacionEContext();
                db.Database.ExecuteSqlCommand("update [dbo].[ArticulosPlaneacionE] set [Formuladerelacion]='" + Formuladerelacion + "', factorcierre= "+ factorcierre + " where [IDArtPlan]=" + id);
                return Json(true);
            }
            catch (Exception err)
            {
                return Json(500, err.Message);
            }
        }


        public JsonResult getarticulosblando(string buscar)
        {
            buscar = buscar.Remove(buscar.Length - 1);
            var Articulos = new ArticuloContext().Articulo.Where(s => s.Cref.Contains(buscar)).OrderBy(S => S.Cref);

            // Populate DropDownList
            List<SelectListItem> opciones = new List<SelectListItem>();

            foreach (Models.Comercial.Articulo art in Articulos)
            {
                SelectListItem elemento = new SelectListItem();
                elemento.Text = art.Cref + " " + art.Descripcion;
                elemento.Value = art.IDArticulo.ToString();
                opciones.Add(elemento);
            }

            return Json(opciones, JsonRequestBehavior.AllowGet);
        }

   

    }
}