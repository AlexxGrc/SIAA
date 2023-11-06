﻿using SIAAPI.ClasesProduccion;
using SIAAPI.Models.Comercial;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Serialization;
using PagedList;
using SIAAPI.ViewModels.Articulo;
using SIAAPI.Models.Administracion;
using SIAAPI.Models.PlaneacionProduccion;
using System.Data.SqlClient;
using SIAAPI.Models.Cotizador;

using SIAAPI.Models.Login;

namespace SIAAPI.Controllers.Cotizador
{
    [Authorize(Roles = "Administrador,Ventas,Sistemas,Almacenista,AdminProduccion,Produccion,Comercial, GerenteVentas, Gerencia")]
    public class CotizadorController : Controller
    {
      
        private ArchivoCotizadorContext db = new ArchivoCotizadorContext();
        // GET: Cotizador
        ClsCotizador elementog = new ClsCotizador();
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize, string Fechainicio, string Fechafinal, string NoCotizacion)
        {
            Session["IDCaracteristica"] = null;
            //var elementos = new ArchivoCotizadorContext().cotizaciones.ToList();

            ViewBag.CurrentSort = sortOrder;

            ViewBag.RazoneSortParm = String.IsNullOrEmpty(sortOrder) ? "Descripcion" : "Descripcion";

            if (searchString == null)
            {
                searchString = currentFilter;
            }

            // Pass filtering string to view in order to maintain filtering when paging
            ViewBag.SearchString = searchString;

            ////Paginación
            //if (searchString != null)
            //{
            //    page = 1;
            //}
            //else
            //{
            //    searchString = currentFilter;
            //}

            ViewBag.CurrentFilter = searchString;
            string ConsultaSql = "select top 200 * from Cotizaciones ";
            string Filtro = "";

            if (!String.IsNullOrEmpty(searchString))
            {

                if (Filtro == string.Empty)
                {
                    Filtro = " where Descripcion like '%" + searchString + "%'";
                }
                else
                {
                    Filtro += " and  like '%" + searchString + "%'";
                }

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

            if (!String.IsNullOrEmpty(NoCotizacion))
            {
                if (Filtro == string.Empty)
                {
                    Filtro = " where ID= "+NoCotizacion;
                }
                else
                {
                    Filtro += " and ID= " + NoCotizacion;
                }
            }

            ViewBag.NoCotizacion = NoCotizacion;

            // Pass filtering string to view in order to maintain filtering when paging
            ViewBag.Fechainicio = Fechainicio;
            ViewBag.Fechafinal = Fechafinal;

            string orden = sortOrder;

            //Ordenacion

            switch (sortOrder)
            {
                case "ID":
                    orden = " order by Fecha desc ,ID ";
                    break;
                case "Descripcion":
                    orden = " order by Descripcion ";
                    break;
                case "Usuario":
                    orden = " order by Fecha desc ,Usuario ";
                    break;
                default:
                    orden = " order by Fecha desc,ID ";
                    break;
            }
            string cadenaSQl = ConsultaSql + " " + Filtro + " " + orden;
            db.Database.CommandTimeout = 300;

            var elementos1 = db.Database.SqlQuery<Cotizaciones>(cadenaSQl).ToList();
            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = db.cotizaciones.OrderBy(e => e.ID).Count(); // Total number of elements

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

            return View(elementos1.ToPagedList(pageNumber, pageSize));
        }


        // GET: Cotizaciones/Details/5
        public ActionResult Details(int id, int page = 1, int pagesize = 10, string searchString = "", string Fechainicio = "", string Fechafinal = "")
        {
            var elemento = db.cotizaciones.Single(m => m.ID == id);
            if (elemento == null)
            {
                return NotFound();
            }
            ViewBag.page = page;
            ViewBag.pagesize = pagesize;
            ViewBag.searchString = searchString;
            ViewBag.Fechainicio = Fechainicio;
            ViewBag.Fechafinal = Fechafinal;
            return View(elemento);
        }

        private ActionResult NotFound()
        {
            throw new NotImplementedException();
        }

        // POST: Cotizaciones/Details/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Details(int id, ClsCotizador collection, int page = 1, int pagesize = 10, string searchString = "", string Fechainicio = "", string Fechafinal = "")
        {
            var elemento = db.cotizaciones.Single(m => m.ID == id);
            return View(elemento);
        }



        // GET: Cotizaciones/Edit/5

        public ActionResult Edit(int? id, int page = 1, int pagesize = 10, string searchString = "", string Fechainicio = "", string Fechafinal = "")
        {

            var elemento = db.cotizaciones.Single(m => m.ID == id);

            ViewBag.page = page;
            ViewBag.pagesize = pagesize;
            ViewBag.searchString = searchString;
            ViewBag.Fechainicio = Fechainicio;
            ViewBag.Fechafinal = Fechafinal;
            return View(elemento);
        }

        // POST: Cotizaciones/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection, int page = 1, int pagesize = 10, string searchString = "", string Fechainicio = "", string Fechafinal = "")
        {
            try
            {
                // TODO: Add update logic here
                var elemento = db.cotizaciones.Single(m => m.ID == id);
                if (TryUpdateModel(elemento))
                {
                    db.SaveChanges();
                    return RedirectToAction("Index", new { page = page, PageSize = pagesize, searchString = searchString, fecini = Fechainicio, fecfin = Fechafinal });
                }
                return View(elemento);
            }
            catch
            {
                return View();
            }
        }

        // GET: Cotizaciones/Delete/5

        public ActionResult Delete(int id, int page = 1, int pagesize = 10, string searchString = "", string Fechainicio = "", string Fechafinal = "")
        {
            var elemento = db.cotizaciones.Single(m => m.ID == id);
            if (elemento == null)
            {
                return HttpNotFound();
            }
            ViewBag.page = page;
            ViewBag.pagesize = pagesize;
            ViewBag.searchString = searchString;
            ViewBag.Fechainicio = Fechainicio;
            ViewBag.Fechafinal = Fechafinal;
            return View(elemento);
        }


        // POST: Cotizaciones/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection, int page = 1, int pagesize = 10, string searchString = "", string Fechainicio = "", string Fechafinal = "")
        {
            try
            {
                var elemento = db.cotizaciones.Single(m => m.ID == id);
                db.cotizaciones.Remove(elemento);
                db.SaveChanges();
                return RedirectToAction("Index", new { page = page, PageSize = pagesize, searchString = searchString, fecini = Fechainicio, fecfin = Fechafinal });

            }
            catch
            {
                return View();
            }
        }

       
        public ActionResult CotizadorRapido1(int? Id, int idsuaje=0)
        {
            ViewBag.Caracteristica = null;
            ViewBag.Articulo = null;
            Caracteristica carac = null;
            ClsCotizador elemento = new ClsCotizador();

            var AcabadoL = new List<SelectListItem>();



            List<Articulo> articulos = new List<Articulo>();
            string cadena = "select * from Articulo where descripcion like '%BARNIZ%'";
            articulos = db.Database.SqlQuery<Articulo>(cadena).ToList();
            List<SelectListItem> listaArticulo = new List<SelectListItem>();
            listaArticulo.Add(new SelectListItem { Text = "--Todos los artículos--", Value = "0" });

            foreach (var m in articulos)
            {
                listaArticulo.Add(new SelectListItem { Text = m.Descripcion, Value = m.IDArticulo.ToString() });
            }
            //ViewBag.ACABADOL = listaArticulo;
            AcabadoL.Add(new SelectListItem { Text = "---Seleccionar---", Value = "N/A" });
            
            AcabadoL.Add(new SelectListItem { Text = "LAMINADO BRILLANTE", Value = "LAMINADO BRILLANTE" });
            AcabadoL.Add(new SelectListItem { Text = "LAMINADO MATE", Value = "LAMINADO MATE" });
            AcabadoL.Add(new SelectListItem { Text = "BARNIZ BRILLANTE", Value = "BARNIZ BRILLANTE" });
            AcabadoL.Add(new SelectListItem { Text = "BARNIZ MATE", Value = "BARNIZ MATE" });
            AcabadoL.Add(new SelectListItem { Text = "BARNIZ RELEASE", Value = "BARNIZ RELEASE" });
            AcabadoL.Add(new SelectListItem { Text = "FOIL", Value = "FOIL" });

            //AcabadoL.Add(new SelectListItem { Text = "CAST&CURE", Value = "CAST&CURE" });
          


            ViewBag.ACABADOL = new SelectList(AcabadoL, "Value", "Text");

            ViewData["ACABADO"] = ViewBag.ACABADOL;




            if (Session["IDCaracteristica"] != null)
            {
                string idc = Session["IDCaracteristica"].ToString();
                carac = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from  caracteristica where ID=" + idc).FirstOrDefault();
                Articulo arti = new ArticuloContext().Articulo.Find(carac.Articulo_IDArticulo);
                ViewBag.Caracteristica = carac;
                ViewBag.Articulo = arti;
            }
       

            if (carac!=null)
            {
                FormulaSiaapi.Formulas formux = new FormulaSiaapi.Formulas();
                string etiqxrollo = string.Empty;
                string alpaso = string.Empty;
                try
                {
                    etiqxrollo = formux.getValorCadena("ETIQUETAXR", carac.Presentacion);
                    if (etiqxrollo!="0")
                    {
                        elemento.Cantidadxrollo =int.Parse(etiqxrollo);
                    }
                }
                catch(Exception err)
                {

                }
                try
                {
                    alpaso = formux.getValorCadena("AL PASO", carac.Presentacion);
                    if (alpaso != "0")
                    {
                        elemento.productosalpaso = int.Parse(alpaso);
                    }
                }
                catch (Exception err)
                {

                }
            }

            ViewBag.IDCentro = new Repository().GetCentros(elemento.IDCentro);
            ViewBag.IDCaja = new Repository().GetCajas(elemento.IDCaja);
            if (elemento.TCcotizado == 0)

            {
                elemento.TCcotizado = SIAAPI.Properties.Settings.Default.TCcotizador;
            }

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
                elemento.IDSuaje = idsuaje;
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
                ViewBag.IDMAterial3 = cintas;

            }

            else   // viene de cargar el archivo de la cotizacion

            {




                ViewBag.Mensajedeerror = "";
                Cotizaciones archivocotizacion = new ArchivoCotizadorContext().cotizaciones.Find(Id);
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


                if (elemento.IDSuaje == 0)
                {
                    elemento.SuajeNuevo = true;
                }

                var TipoSuaje = new List<SelectListItem>();
                TipoSuaje.Add(new SelectListItem { Text = "---Seleccionar---", Value = "S/N" });
                TipoSuaje.Add(new SelectListItem { Text = "Macizo", Value = "M" });
                TipoSuaje.Add(new SelectListItem { Text = "Flexible", Value = "F" });
                ViewBag.TSuaje = new SelectList(TipoSuaje, "Value", "Text", elemento.TipoSuaje);

                ViewData["TSuaje"] = TipoSuaje;

                var TipoFigura = new List<SelectListItem>();
                TipoFigura.Add(new SelectListItem { Text = "---Seleccionar---", Value = "S/N" });
                TipoFigura.Add(new SelectListItem { Text = "Circulo", Value = "C" });
                TipoFigura.Add(new SelectListItem { Text = "Rectangulo", Value = "R" });
                TipoFigura.Add(new SelectListItem { Text = "Triangulo", Value = "T" });
                TipoFigura.Add(new SelectListItem { Text = "Cuadrado", Value = "Cu" });
                TipoFigura.Add(new SelectListItem { Text = "Irregular", Value = "I" });
                TipoFigura.Add(new SelectListItem { Text = "Muesca General", Value = "Muesca General" });
                TipoFigura.Add(new SelectListItem { Text = "Muesca Especial", Value = "Muesca Especial" });
                TipoFigura.Add(new SelectListItem { Text = "Corte Recto", Value = "Corte Recto" });
                TipoFigura.Add(new SelectListItem { Text = "Figura Especial", Value = "Figura Especial" });


                ViewBag.TSuajeFi = new SelectList(TipoFigura, "Value", "Text", elemento.TipoSuajeFigura);

                ViewData["TSuajeFi"] = TipoFigura;

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

              

                ViewBag.ACABADOL = new SelectList(AcabadoL, "Value", "Text", elemento.ACABADO);

              
                if (elemento.IDMonedapreciosconvenidos == 0)

                { elemento.IDMonedapreciosconvenidos = new c_MonedaContext().c_Monedas.Where(s => s.ClaveMoneda == "USD").ToList().FirstOrDefault().IDMoneda;
                }
                if (elemento.TCcotizado == 0 )

                {
                    elemento.TCcotizado = SIAAPI.Properties.Settings.Default.TCcotizador;
                }



                ViewData["Tintas"] = null;
                try
                {
                    
                   ViewData["Tintas"] = elemento.Tintas;
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
                //var suajes = new Repository().GetSuajes(elemento.IDSuaje);
                //ViewBag.IDSuaje = suajes;
              
                ViewBag.nombresuaje = elemento.IDSuaje;
                if (elemento.IDSuaje2==0)
                {
                    var suajes2 = new Repository().GetPlecas();
                    ViewBag.IDSuaje2 = suajes2;
                }
                else
                {
                    var suajes2 = new Repository().GetPlecas(elemento.IDSuaje2);
                    ViewBag.IDSuaje2 = suajes2;
                }

                ViewBag.IDCentro = new Repository().GetCentros(elemento.IDCentro);
                ViewBag.IDCaja = new Repository().GetCajas(elemento.IDCaja);


                var cintas = new Repository().GetCintasbyClave(elemento.IDMaterial);
                ViewBag.IDMaterial = cintas;

                var cintas2 = new Repository().GetCintasbyClave(elemento.IDMaterial2);
               
                ViewBag.IDMaterial2 = cintas2;
                var cintas3 = new Repository().GetCintasbyClave(elemento.IDMaterial3);
                ViewBag.IDMAterial3 = cintas3;


                ViewBag.ACABADO = new SelectList(AcabadoL, "Value", "Text", elemento.ACABADO);

            }


            return View(elemento);
        }


        [HttpPost]
        public ActionResult ActualizarPrecioConvenido(ClsCotizador elemento, FormCollection colecciondeelementos)
        {
            try
            {
                elemento.precioconvenidos.precio1 = decimal.Parse(colecciondeelementos.Get("precioconvenido1"));

                elemento.precioconvenidos.precio2 = decimal.Parse(colecciondeelementos.Get("precioconvenido2"));
                elemento.precioconvenidos.precio3 = decimal.Parse(colecciondeelementos.Get("precioconvenido3"));
                elemento.precioconvenidos.precio4 = decimal.Parse(colecciondeelementos.Get("precioconvenido4"));
            }
            catch (Exception err)
            {

            }
            return View(elemento);
        }






        [HttpPost]
        public ActionResult CotizadorRapido1(ClsCotizador elemento, FormCollection colecciondeelementos)
        {
            ViewBag.Caracteristica = null;
            elemento = verificaprecios(elemento);


            List<Articulo> articulos = new List<Articulo>();
            string cadena = "select * from Articulo where descripcion like '%BARNIZ%'";
            articulos = db.Database.SqlQuery<Articulo>(cadena).ToList();
            List<SelectListItem> listaArticulo = new List<SelectListItem>();
            listaArticulo.Add(new SelectListItem { Text = "--Todos los artículos--", Value = "0" });

            foreach (var m in articulos)
            {
                listaArticulo.Add(new SelectListItem { Text = m.Descripcion, Value = m.IDArticulo.ToString() });
            }
            //ViewBag.ACABADOL = listaArticulo;


            var AcabadoL = new List<SelectListItem>();
            AcabadoL.Add(new SelectListItem { Text = "---Seleccionar---", Value = "N/A" });
           
            AcabadoL.Add(new SelectListItem { Text = "LAMINADO BRILLANTE", Value = "LAMINADO BRILLANTE" });
            AcabadoL.Add(new SelectListItem { Text = "LAMINADO MATE", Value = "LAMINADO MATE" });
            AcabadoL.Add(new SelectListItem { Text = "BARNIZ BRILLANTE", Value = "BARNIZ BRILLANTE" });
            AcabadoL.Add(new SelectListItem { Text = "BARNIZ MATE", Value = "BARNIZ MATE" });
            AcabadoL.Add(new SelectListItem { Text = "BARNIZ RELEASE", Value = "BARNIZ RELEASE" });
            AcabadoL.Add(new SelectListItem { Text = "FOIL", Value = "FOIL" });
            //AcabadoL.Add(new SelectListItem { Text = "CAST&CURE", Value = "CAST&CURE" });



            ViewBag.ACABADOL = new SelectList(AcabadoL, "Value", "Text", elemento.ACABADO);

            ViewData["ACABADO"] = ViewBag.ACABADOL;



            ViewBag.Articulo = null;
            if (Session["IDCaracteristica"] != null)
            {
                string idc = Session["IDCaracteristica"].ToString();
                Caracteristica carac = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from caracteristica where id=" + idc).FirstOrDefault();
                Articulo arti = new ArticuloContext().Articulo.Find(carac.Articulo_IDArticulo);
                ViewBag.Caracteristica = carac;
                ViewBag.Articulo = arti;
            }

            ViewBag.IDCentro = new Repository().GetCentros(elemento.IDCentro);
            ViewBag.IDCaja = new Repository().GetCajas(elemento.IDCaja);

          //  elemento.SuajeNuevo = false;


            try
            {
                int idsuaje2 = elemento.IDSuaje2;
                string suaje2 = colecciondeelementos.Get("IDSuaje2");
                int IDS2 = int.Parse(suaje2);
                if (elemento.IDSuaje2==0)
                {
                   
                    elemento.IDSuaje2 = IDS2;
                    idsuaje2 = elemento.IDSuaje2;
                }

            }
            catch (Exception err)
            {
                string menjsaje1 = err.Message;
            }

            if (elemento.IDSuaje==0 && elemento.IDSuaje2==0)
            {
                elemento.SuajeNuevo = true;
            }

            try
            {
                string cavidades = colecciondeelementos.Get("CavidadesdeSuajeAvance");
                elemento.cavidadesdesuajeAvance = Convert.ToInt32(cavidades);
            }
            catch (Exception err)
            {
                string menjsaje1 = err.Message;
            }


            ViewBag.nombresuaje = elemento.IDSuaje;
            if (elemento.TCcotizado == 0)

            {
                elemento.TCcotizado = SIAAPI.Properties.Settings.Default.TCcotizador;
            }
            ViewBag.IDCotizacion = elemento.IDCotizacion;

            //if (elemento.ACABADO == "BARNIZ BRILLANTE" || elemento.ACABADO== "BARNIZ MATE" || elemento.ACABADO== "BARNIZ RELEASE")
            //{
            //    elemento.Numerodetintas += 1;
                

            //}


            FormulaEspecializada.Formulaespecializada formula = new FormulaEspecializada.Formulaespecializada();
            formula = igualar(elemento, formula);


            try
            {
                string cm = colecciondeelementos.Get("Monedafinal");
                elemento.IDMonedapreciosconvenidos = new c_MonedaContext().c_Monedas.Where(s => s.ClaveMoneda == cm).ToList()[0].IDMoneda;
            }
            catch (Exception err)
            {
                string menjsaje1 = err.Message;
            }

            ///////////////////////////// suajes ///////////////////
            ViewBag.EnqueEstoy = 1;

            var suajes = new Repository().GetSuajes(elemento.IDSuaje);

            ViewBag.IDSuaje = suajes;



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
            var TipoSuaje = new List<SelectListItem>();
            TipoSuaje.Add(new SelectListItem { Text = "---Seleccionar---", Value = "S/N" });
            TipoSuaje.Add(new SelectListItem { Text = "Macizo", Value = "M" });
            TipoSuaje.Add(new SelectListItem { Text = "Flexible", Value = "F" });
            ViewBag.TSuaje = new SelectList(TipoSuaje, "Value", "Text", elemento.TipoSuaje);

            ViewData["TSuaje"] = TipoSuaje;

            var TipoFigura = new List<SelectListItem>();
            TipoFigura.Add(new SelectListItem { Text = "---Seleccionar---", Value = "S/N" });
            TipoFigura.Add(new SelectListItem { Text = "Circulo", Value = "C" });
            TipoFigura.Add(new SelectListItem { Text = "Rectangulo", Value = "R" });
            TipoFigura.Add(new SelectListItem { Text = "Triangulo", Value = "T" });
            TipoFigura.Add(new SelectListItem { Text = "Cuadrado", Value = "Cu" });
            TipoFigura.Add(new SelectListItem { Text = "Irregular", Value = "I" });
            TipoFigura.Add(new SelectListItem { Text = "Muesca General", Value = "Muesca General" });
            TipoFigura.Add(new SelectListItem { Text = "Muesca Especial", Value = "Muesca Especial" });
            TipoFigura.Add(new SelectListItem { Text = "Corte Recto", Value = "Corte Recto" });
            TipoFigura.Add(new SelectListItem { Text = "Figura Especial", Value = "Figura Especial" });


            ViewBag.TSuajeFi = new SelectList(TipoFigura, "Value", "Text", elemento.TipoSuajeFigura);

            ViewData["TSuajeFi"] = TipoFigura;

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



        






            /////////////////////////////Cintas ///////////////////
            var cintas = new Repository().GetCintasbyClave(elemento.IDMaterial);

            if (elemento.IDMaterial != 0)
            {
                try
                {
                    Materiales mat = new MaterialesContext().Materiales.Find(elemento.IDMaterial);
                    if (mat.Fam == "NYL")
                    {
                        elemento.Materialnecesitarefile = false;
                        formula.Materialnecesitarefile = false;
                    }
                    else
                    {
                        elemento.Materialnecesitarefile = true;
                        formula.Materialnecesitarefile = false;
                    }
                }
                catch(Exception err)
                {
                    string mensajeerror = err.Message;
                }
            }

            ViewBag.IDMaterial = cintas;

            ViewBag.IDMAterial2 = new Repository().GetCintasbyClave(elemento.IDMaterial2);
            ViewBag.IDMAterial3 = new Repository().GetCintasbyClave(elemento.IDMaterial3);
            ///////////////////// Tintas //////////////////////////////////////

         
            var tintas = Gettintasiniciales(formula.Numerodetintas, colecciondeelementos, elemento.CantidadMPMts2);

            if (verificatintas(elemento) == true)
            {
                
                List<Tinta> tintasA = new List<Tinta>();
                if (elemento.ACABADO == "BARNIZ BRILLANTE")
                {
                    try
                    {
                        int num = formula.Numerodetintas + 1;
                        //int idartciuloacabado = int.Parse(colecciondeelementos.Get("Tinta1"));

                        //Articulo art = new ArticuloContext().Articulo.Find(idartciuloacabado);

                        //if (art.Descripcion.Contains("BARNIZ"))
                        //{
                            Tinta nueva = new Tinta();

                            string tintaacabado = colecciondeelementos.Get("Tinta" + num);
                            if (tintaacabado != "5536")
                            {
                                nueva.IDTinta = int.Parse(tintaacabado);
                            }
                            else
                            {
                                nueva.IDTinta = (5536);
                            }
                          


                            //if (formula.Numerodetintas == 1)
                            //{
                            //    num = num - 1;
                            //}
                            try
                            {
                                decimal areaacabado = decimal.Parse(colecciondeelementos.Get("Area" + num).ToString());
                                if (areaacabado != 100)
                                {
                                    nueva.Area = areaacabado;
                                }
                                else
                                {
                                    nueva.Area = (100);
                                }

                            }
                            catch (Exception err)
                            {
                                nueva.Area = (100);
                            }

                            tintas.Add(nueva);

                        //}

                      
                    }
                    catch (Exception err)
                    {
                        Tinta nueva = new Tinta();
                        nueva.IDTinta = (5536);
                        nueva.Area = (100);
                        tintas.Add(nueva);
                    }

                }
                if (elemento.ACABADO == "BARNIZ MATE")
                {
                    Tinta nueva = new Tinta();
                    nueva.IDTinta = (5182);
                    int num = formula.Numerodetintas + 1;
                    //if (formula.Numerodetintas == 1)
                    //{
                    //    num = num - 1;
                    //}
                    try
                    {
                        decimal areaacabado = int.Parse(colecciondeelementos.Get("Area" + num));
                        if (areaacabado != 100)
                        {
                            nueva.Area = areaacabado;
                        }
                        else
                        {
                            nueva.Area = (100);
                        }

                    }
                    catch (Exception err)
                    {
                        nueva.Area = (100);
                    }
                    tintas.Add(nueva);
                }
            }
            

            ViewData["Tintas"] = null;
            try
            {
                ViewData["Tintas"] = tintas;
            }
            catch (Exception err)
            {
                string mensaje = err.Message;
            }

            formula = pasaTintas(tintas, formula); // lo paso para calcular
            elemento = pasaTintas(tintas, elemento); // lo paso al formulario web para generar archivo

            formula.CobrarMaster = elemento.CobrarMaster;
            formula.anchommmaster = elemento.anchommmaster;

            //////////////////////////  Calcular //////////////////////

            if (elemento.IDMaterial2 != 0)
            {
                try
                {
                    Materiales mat2 = new MaterialesContext().Materiales.Find(elemento.IDMaterial2);
                    if (mat2.Completo)
                    {
                        formula.CobrarMaster2m = true;
                        formula.anchomaster2m = mat2.Ancho;
                        formula.largomaster2m = mat2.Largo;


                    }
                }
                catch (Exception err)
                {
                    string mensajeerror = err.Message;
                }
            }

            formula.Calcular();

            formula = CosteaTintas(formula);

            decimal mo = formula.getHoraPrensa();

            decimal moe = 0;

            if ( formula.Cantidadxrollo>0)
            { 

              moe  = formula.getHoraEmbobinado();
            }

  			elemento.Costo1mxn = elemento.Costo1 * SIAAPI.Properties.Settings.Default.TCcotizador;
            elemento.Costo2mxn = elemento.Costo2 * SIAAPI.Properties.Settings.Default.TCcotizador;
            elemento.Costo3mxn = elemento.Costo3 * SIAAPI.Properties.Settings.Default.TCcotizador;
            elemento.Costo4mxn = elemento.Costo4 * SIAAPI.Properties.Settings.Default.TCcotizador;

            elemento.MaterialNecesitado = formula.MaterialNecesitado;

            elemento.Numerodecintas = formula.Numerodecintas;
            elemento.CintasMaster = formula.CintasMaster;
            elemento.Minimoproducir = formula.Minimoproducir;

            //if (elemento.Cantidad < elemento.Minimoproducir && elemento.Cantidad > 0)
            //{
            //    elemento.Cantidad = elemento.Minimoproducir;
            //}
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


            ViewBag.EnqueEstoy = 1;

            ViewBag.Mensajedeerror = "";
            ViewBag.Minino = Math.Round( elemento.Minimoproducir,2);

            bool pasa = true;
           
            bool pasafinal = true;

            //////cavidades
            ///


            if ((elemento.cavidadesdesuajeEje % elemento.productosalpaso) != 0)
            {

                ViewBag.Mensajedeerror += "La cantidad de etiquetas al paso no es un multiplo de las cavidades del suaje\n";
                pasa = false;
            }

            if ((elemento.cavidadesdesuajeEje < elemento.productosalpaso) )
            {

                ViewBag.Mensajedeerror += "La cantidad de etiquetas al paso no puede ser mayor al suaje\n";
                pasa = false;
            }

           




            if (elemento.Cantidad==0)
            {

                ViewBag.Mensajedeerror += "No hay datos suficientes para calcular un precio, Comienza por dar  una cantidad a producir\n";
                pasa = false;
            }


            if (elemento.Minimoproducir==0)
            {
                ViewBag.Mensajedeerror += "No hay datos suficientes para calcular un precio, Comienza por dar las medidas de la etiqueta y una cantidad a producir\n";
                pasa = false;
            }

            if (elemento.Cantidadxrollo == 0)
            {
                ViewBag.Mensajedeerror += "No has elegido la cantidad por rollo o paquete \n";
                pasa = false;
            }

            //if (elemento.ACABADO.Contains("LAMINADO"))
            //{
            //    ViewBag.Mensajedeerror += "Elegiste UN ACABADO LAMINADO, ingresar el material ADICIONAL \n";
            //       pasa = true;
            //    //if ((elemento.IDMaterial2 == 0) && (elemento.IDMaterial3 == 0))
            //    //{
            //    //    ViewBag.Mensajedeerror += "Elegiste UN ACABADO LAMINADO, ingresar el material ADICIONAL \n";
            //    //    pasa = false;
            //    //}
            //    //else if (elemento.IDMaterial2 != 0 && elemento.IDMaterial3 == 0)
            //    //{
            //    //    ViewBag.Mensajedeerror += "Elegiste UN ACABADO LAMINADO, ingresar el material ADICIONAL \n";
            //    //    pasa = false;
            //    //}
            //    //else if (elemento.IDMaterial2 != 0 && elemento.IDMaterial3 == 0)
            //    //{
            //    //    ViewBag.Mensajedeerror += "Elegiste UN ACABADO LAMINADO, ingresar el material ADICIONAL \n";
            //    //    pasa = false;
            //    //}

            //}
            //if (elemento.ACABADO.Contains("FOIL"))
            //{
            //    ViewBag.Mensajedeerror += "Elegiste UN ACABADO FOIL, ingresar el material ADICIONAL \n";
            //    pasa = true;
            //}
            if (elemento.IDMaterial==0)
            {
                ViewBag.Mensajedeerror+= "\nNo has seleccionado una cinta con la cual trabajar";
                pasa = false;
            }
            if ((elemento.IDMaterial > 0) && (elemento.CostoM2Cinta==0))
            {
                ViewBag.Mensajedeerror += "\nLa Cinta elegida no contiene un costo asignado, registra un costo o escribe manualmente el costo pretendido en la cinta\n";
                pasa = false;
            }
            if ((elemento.IDMaterial2 > 0) && (elemento.CostoM2Cinta2 == 0))
            {
                ViewBag.Mensajedeerror += "\nLa Cinta Adicional elegida no contiene un costo asignado, registra un costo o escribe manualmente el costo pretendido en la cinta Adicional\n";
                pasa = false;
            }


            if ((elemento.IDMaterial3 > 0) && (elemento.CostoM2Cinta3 == 0))
            {
                ViewBag.Mensajedeerror += "\nLa Cinta Adicional elegida no contiene un costo asignado, registra un costo o escribe manualmente el costo pretendido en la cinta Adicional\n";
                pasa = false;
            }

            if(verificatintas(elemento)==false)
            {
                ViewBag.Mensajedeerror += "\nIndicaste tintas pero al menos una de ellas no especificaste tinta\n";
                pasa = false;
            }

            

            try
            {
                if (colecciondeelementos.Get("Rango1").ToString() == "" || colecciondeelementos.Get("Rango1").ToString() == "0")
                {
                    throw new Exception("El Rango 1 es null o 0");
                }
                elemento.Rango1 = decimal.Parse( colecciondeelementos.Get("Rango1").ToString());
                if (elemento.Rango1<0)
                {
                    throw new Exception("El Rango 1 es menor a 0");
                }
                ViewBag.Rango1 = elemento.Rango1;
            }

            catch(Exception err)
            {
                string mensajedeerror = err.Message;
                pasafinal = false;
            }

            try
            {
                if (colecciondeelementos.Get("Rango2").ToString() == "" || colecciondeelementos.Get("Rango2").ToString() == "0")
                {
                    throw new Exception("El Rango 2 es null o 0");
                }
                elemento.Rango2 = decimal.Parse(colecciondeelementos.Get("Rango2").ToString());
                if (elemento.Rango1 < 0)
                {
                    throw new Exception("El Rango 2 es menor a 0");
                }
                ViewBag.Rango2 = elemento.Rango2;
            }
            catch (Exception err)
            {
                string mensajedeerror = err.Message;
                pasafinal = false;
            }


            try
            {
                if (colecciondeelementos.Get("Rango3").ToString() == "" || colecciondeelementos.Get("Rango3").ToString() == "0")
                {
                    throw new Exception("El Rango 3 es null o 0");
                }
                elemento.Rango3 = decimal.Parse(colecciondeelementos.Get("Rango3").ToString());
                if (elemento.Rango1 < 0)
                {
                    throw new Exception("El Rango 3 es menor a 0");
                }
                ViewBag.Rango3 = elemento.Rango3;
            }
            catch (Exception err)
            {
                string mensajedeerror = err.Message;
                pasafinal = false;
            }

            try
            {
                if (colecciondeelementos.Get("Rango4").ToString() == "" || colecciondeelementos.Get("Rango4").ToString() == "0")
                {
                    throw new Exception("El Rango 4 es null o 0");
                }
                elemento.Rango4 = decimal.Parse(colecciondeelementos.Get("Rango4").ToString());
                if (elemento.Rango1 < 0)
                {
                    throw new Exception("El Rango 4 es menor a 0");
                }
                ViewBag.Rango4 = elemento.Rango4;
            }
            catch (Exception err)
            {
                string mensajedeerror = err.Message;
                pasafinal = false;
            }


            if (elemento.Rango1==0 && elemento.Rango2==0 )
            {
               if (elemento.Cantidad< formula.Minimoproducir)
                {
                    elemento.Rango1 = elemento.Cantidad;
                    elemento.Rango2 = Math.Round( elemento.Minimoproducir,1);
                }
               if (elemento.Cantidad>= formula.Minimoproducir)
                {
                    elemento.Rango1 = Math.Round(elemento.Minimoproducir, 1);
                    elemento.Rango2 = elemento.Cantidad;
                }
            }



            if (mo==0 || moe==0)
            {
                pasafinal = false;
            }

           if (pasa && pasafinal)
            {
                decimal costosuaje = new ArticuloContext().Database.SqlQuery<decimal>("select dbo.GetCosto(0," + elemento.IDSuaje + ",1)").ToList().FirstOrDefault();
                if (elemento.DiluirSuajeEnPedidos==0)
                { elemento.DiluirSuajeEnPedidos = 50; }
                costosuaje = costosuaje / elemento.DiluirSuajeEnPedidos;

                if (costosuaje<2)
                {
                    costosuaje = 2;
                }


                // suma del costo del suaje diluido + costo materias primas + costo de tintas + costo de prensa + costo de embobinado 
                


                FormulaEspecializada.Formulaespecializada Formulapara1 = new FormulaEspecializada.Formulaespecializada();
                Formulapara1 = igualar(elemento, Formulapara1);
                Formulapara1.Cantidad = decimal.Parse(colecciondeelementos.Get("Rango1"));
                Formulapara1 = pasaTintas(tintas, Formulapara1);
                Formulapara1.CobrarMaster = elemento.CobrarMaster;
                Formulapara1.anchommmaster = elemento.anchommmaster;
                if (elemento.IDMaterial2 != 0)
                {
                    try
                    {
                        Materiales mat2 = new MaterialesContext().Materiales.Find(elemento.IDMaterial2);
                        if (mat2.Completo)
                        {
                            Formulapara1.CobrarMaster2m = true;
                            Formulapara1.anchomaster2m = mat2.Ancho;
                            Formulapara1.largomaster2m = mat2.Largo;


                        }
                    }
                    catch (Exception err)
                    {
                        string mensajeerror = err.Message;
                    }
                }


                Formulapara1.Calcular();
                Formulapara1 = CosteaTintas(Formulapara1);
                Formulapara1.calcularCostoMO();


                decimal costototal = Formulapara1.CostototalMP + Formulapara1.Costodetintas + Formulapara1.CostototalMO;//+ (costosuaje) + SIAAPI.Properties.Settings.Default.costodeempaque;

                decimal costoxmillar = costototal / Formulapara1.Cantidad;
                elemento.Costo1 = costoxmillar;





                elemento.MatrizPrecio.Fila1.Celda1.Valor =Math.Round( (costoxmillar * (1 / ((100 - Formulapara1.Rango1gain) / 100))),2);
                elemento.MatrizPrecio.Fila1.Celda2.Valor = Math.Round((costoxmillar * (1 / ((100 - Formulapara1.Rango2gain) / 100))), 2);
                elemento.MatrizPrecio.Fila1.Celda3.Valor = Math.Round((costoxmillar * (1 / ((100 - Formulapara1.Rango3gain) / 100))), 2);
                elemento.MatrizPrecio.Fila1.Celda4.Valor = Math.Round((costoxmillar * (1 / ((100 - Formulapara1.Rango4gain) / 100))), 2);


                FormulaEspecializada.Formulaespecializada Formulapara2 = new FormulaEspecializada.Formulaespecializada();
                Formulapara2 = igualar(elemento, Formulapara2);
                Formulapara2.Cantidad = decimal.Parse(colecciondeelementos.Get("Rango2"));
                Formulapara2 = pasaTintas(tintas, Formulapara2);
                Formulapara2.CobrarMaster = elemento.CobrarMaster;
                Formulapara2.anchommmaster = elemento.anchommmaster;
                if (elemento.IDMaterial2 != 0)
                {
                    try
                    {
                        Materiales mat2 = new MaterialesContext().Materiales.Find(elemento.IDMaterial2);
                        if (mat2.Completo)
                        {
                            Formulapara2.CobrarMaster2m = true;
                            Formulapara2.anchomaster2m = mat2.Ancho;
                            Formulapara2.largomaster2m = mat2.Largo;


                        }
                    }
                    catch (Exception err)
                    {
                        string mensajeerror = err.Message;
                    }
                }

                Formulapara2.Calcular();
                Formulapara2 = CosteaTintas(Formulapara2);
                Formulapara2.calcularCostoMO();

                costototal = Formulapara2.CostototalMP + Formulapara2.Costodetintas + Formulapara2.CostototalMO;//+ (costosuaje) + SIAAPI.Properties.Settings.Default.costodeempaque;

                costoxmillar = costototal / Formulapara2.Cantidad;

                elemento.Costo2 = costoxmillar;

                elemento.MatrizPrecio.Fila2.Celda1.Valor = Math.Round((costoxmillar * (1 / ((100 - Formulapara2.Rango1gain) / 100))), 2);
                elemento.MatrizPrecio.Fila2.Celda2.Valor = Math.Round((costoxmillar * (1 / ((100 - Formulapara2.Rango2gain) / 100))), 2);
                elemento.MatrizPrecio.Fila2.Celda3.Valor = Math.Round((costoxmillar * (1 / ((100 - Formulapara2.Rango3gain) / 100))), 2);
                elemento.MatrizPrecio.Fila2.Celda4.Valor = Math.Round((costoxmillar * (1 / ((100 - Formulapara2.Rango4gain) / 100))), 2);


                FormulaEspecializada.Formulaespecializada Formulapara3 = new FormulaEspecializada.Formulaespecializada();
                Formulapara3 = igualar(elemento, Formulapara3);
                Formulapara3.Cantidad = decimal.Parse(colecciondeelementos.Get("Rango3"));
                Formulapara3 = pasaTintas(tintas, Formulapara3);
                Formulapara3.CobrarMaster = elemento.CobrarMaster;
                Formulapara3.anchommmaster = elemento.anchommmaster;
                if (elemento.IDMaterial2 != 0)
                {
                    try
                    {
                        Materiales mat2 = new MaterialesContext().Materiales.Find(elemento.IDMaterial2);
                        if (mat2.Completo)
                        {
                            Formulapara3.CobrarMaster2m = true;
                            Formulapara3.anchomaster2m = mat2.Ancho;
                            Formulapara3.largomaster2m = mat2.Largo;


                        }
                    }
                    catch (Exception err)
                    {
                        string mensajeerror = err.Message;
                    }
                }

                Formulapara3.Calcular();
                Formulapara3 = CosteaTintas(Formulapara3);
                Formulapara3.calcularCostoMO();

                costototal = Formulapara3.CostototalMP + Formulapara3.Costodetintas + Formulapara3.CostototalMO;//+ (costosuaje) + SIAAPI.Properties.Settings.Default.costodeempaque;

                costoxmillar = costototal / Formulapara3.Cantidad;

                elemento.Costo3 = costoxmillar;

                elemento.MatrizPrecio.Fila3.Celda1.Valor = Math.Round((costoxmillar * (1 / ((100 - Formulapara3.Rango1gain) / 100))), 2);
                elemento.MatrizPrecio.Fila3.Celda2.Valor = Math.Round((costoxmillar * (1 / ((100 - Formulapara3.Rango2gain) / 100))), 2);
                elemento.MatrizPrecio.Fila3.Celda3.Valor = Math.Round((costoxmillar * (1 / ((100 - Formulapara3.Rango3gain) / 100))), 2);
                elemento.MatrizPrecio.Fila3.Celda4.Valor = Math.Round((costoxmillar * (1 / ((100 - Formulapara3.Rango4gain) / 100))), 2);

                FormulaEspecializada.Formulaespecializada Formulapara4 = new FormulaEspecializada.Formulaespecializada();
                Formulapara4 = igualar(elemento, Formulapara4);
                Formulapara4.Cantidad = decimal.Parse(colecciondeelementos.Get("Rango4"));
                Formulapara4 = pasaTintas(tintas, Formulapara4);
                Formulapara4.CobrarMaster = elemento.CobrarMaster;
                Formulapara4.anchommmaster = elemento.anchommmaster;

                if (elemento.IDMaterial2 != 0)
                {
                    try
                    {
                        Materiales mat2 = new MaterialesContext().Materiales.Find(elemento.IDMaterial2);
                        if (mat2.Completo)
                        {
                            Formulapara4.CobrarMaster2m = true;
                            Formulapara4.anchomaster2m = mat2.Ancho;
                            Formulapara4.largomaster2m = mat2.Largo;


                        }
                    }
                    catch (Exception err)
                    {
                        string mensajeerror = err.Message;
                    }
                }

                Formulapara4.Calcular();
                Formulapara4 = CosteaTintas(Formulapara4);
                Formulapara4.calcularCostoMO();

                costototal = Formulapara4.CostototalMP + Formulapara4.Costodetintas + Formulapara4.CostototalMO;// + (costosuaje) + SIAAPI.Properties.Settings.Default.costodeempaque;

                costoxmillar = costototal / Formulapara4.Cantidad;

                elemento.Costo4 = costoxmillar;

                elemento.MatrizPrecio.Fila4.Celda1.Valor = Math.Round((costoxmillar * (1 / ((100 - Formulapara4.Rango1gain) / 100))), 2);
                elemento.MatrizPrecio.Fila4.Celda2.Valor = Math.Round((costoxmillar * (1 / ((100 - Formulapara4.Rango2gain) / 100))), 2);
                elemento.MatrizPrecio.Fila4.Celda3.Valor = Math.Round((costoxmillar * (1 / ((100 - Formulapara4.Rango3gain) / 100))), 2);
                elemento.MatrizPrecio.Fila4.Celda4.Valor = Math.Round((costoxmillar * (1 / ((100 - Formulapara4.Rango4gain) / 100))), 2);

                elemento.Yatienematriz = true;

                try  /// aqui pone los precios convenidos con el cliente
                {
                    elemento.precioconvenidos.precio1 =0M/*decimal.Parse(colecciondeelementos.Get("precioconvenido1"))*/;
                                                      
                    elemento.precioconvenidos.precio2 =0M/*decimal.Parse(colecciondeelementos.Get("precioconvenido2"))*/;
                    elemento.precioconvenidos.precio3 =0M/*decimal.Parse(colecciondeelementos.Get("precioconvenido3"))*/;
                    elemento.precioconvenidos.precio4 =0M/*decimal.Parse(colecciondeelementos.Get("precioconvenido4"))*/;
                }
                catch (Exception err)
                {

                }


                if (elemento.precioconvenidos.precio1 == 0)
                {
                    elemento.precioconvenidos.precio1 = elemento.MatrizPrecio.Fila1.Celda1.Valor;
                }
                if (elemento.precioconvenidos.precio2 == 0)
                {
                    elemento.precioconvenidos.precio2 = elemento.MatrizPrecio.Fila2.Celda1.Valor;
                }
                if (elemento.precioconvenidos.precio3 == 0)
                {
                    elemento.precioconvenidos.precio3 = elemento.MatrizPrecio.Fila3.Celda1.Valor;
                }
                if (elemento.precioconvenidos.precio4 == 0)
                {
                    elemento.precioconvenidos.precio4 = elemento.MatrizPrecio.Fila4.Celda1.Valor;
                }


                if (colecciondeelementos.Get("Enviar") == "Sobreescribir")
                {
                    Cotizaciones archivocotizacion = new ArchivoCotizadorContext().cotizaciones.Find(elemento.IDCotizacion);
                    XmlDocument documento = new XmlDocument();
                    string nombredearchivo = archivocotizacion.Ruta.TrimEnd() + "\\" + archivocotizacion.NombreArchivo + ".xml";


                    StringWriter stringwriter = new StringWriter();
                    XmlSerializer x = new XmlSerializer(elemento.GetType());
                    x.Serialize(stringwriter, elemento);

                    string xmlstring = stringwriter.ToString().Replace("encoding=\"utf-16\"", "");


                    EscribeArchivoXMLC(xmlstring, nombredearchivo, true);


                    new ArchivoCotizadorContext().Database.ExecuteSqlCommand("update cotizaciones set contenido='" + xmlstring + "' where id=" + elemento.IDCotizacion);


                    try
                    {
                        List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
                        int UserID = userid.Select(s => s.UserID).FirstOrDefault();

                        //sobreescribir cotización
                        string insert = "insert into RegistroEdicionCotizacion(IDUsuario, Fecha, IDCotizacion) values (" + UserID + ", sysdatetime()," + elemento.IDCotizacion + ")";
                        db.Database.ExecuteSqlCommand(insert);
                    }
                    catch (Exception err)
                    {

                    }

                }



                if (colecciondeelementos.Get("Enviar") == "Grabar Archivo")
                {
                    string NombredeArchivo = DateTime.Now.ToString().Replace(".", "").Replace(",", "").Replace(" ", "").Replace("/", "").Replace(":", "");

                    string nombredecarpeta = DateTime.Now.Month+""+ DateTime.Now.Year + " " + User.Identity.Name;

                    if (!Directory.Exists(System.Web.HttpContext.Current.Server.MapPath("~/Cotizaciones")))
                    {
                        Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath("~/Cotizaciones"));
                    }
                    if (!Directory.Exists(System.Web.HttpContext.Current.Server.MapPath("~/Cotizaciones/" + User.Identity.Name)))
                    {
                        Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath("~/Cotizaciones" + User.Identity.Name));
                    }
                    if (!Directory.Exists(System.Web.HttpContext.Current.Server.MapPath("~/Cotizaciones/" + User.Identity.Name + "/" + nombredecarpeta)))
                    {
                        Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath("~/Cotizaciones/" + User.Identity.Name + "/" + nombredecarpeta));
                    }


                    nombredecarpeta = System.Web.HttpContext.Current.Server.MapPath("~/Cotizaciones/" + User.Identity.Name + "/" + nombredecarpeta);

                     this.GrabarArchivoCotizador(elemento,NombredeArchivo,nombredecarpeta);

                    return RedirectToAction("ArchivoCotizador", new { _nombredearchivo = NombredeArchivo, _ruta = nombredecarpeta, termo = 0, suajeN = 0, suajeE = 0 });

                }
                elemento.IDCotizacion = ViewBag.IDCotizacion;
                if (colecciondeelementos.Get("Enviar") == "Crear Articulo" && elemento.IDCotizacion>0) //llego hasta crear articulo
                {
                   

                    return RedirectToAction("CreaArticulo", new {id = elemento.IDCotizacion });


                }
                if (colecciondeelementos.Get("Enviar") == "Asignar Articulo" && elemento.IDCotizacion > 0) //llego hasta crear articulo
                {


                    return RedirectToAction("Asignarunarticulo", new { id = elemento.IDCotizacion });


                }
                if (colecciondeelementos.Get("Enviar") == "Crear PDF" && elemento.IDCotizacion > 0) //llego hasta crear articulo
                {


                    return RedirectToAction("CrearCotizacionPDF", new { IDCotizacion = elemento.IDCotizacion });


                }
            }



            return View(elemento);

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
            formula.CobrarMaster = elemento.CobrarMaster;
            formula.anchommmaster = elemento.anchommmaster;

            formula.conadhesivo = elemento.conadhesivo;
            formula.hotstamping = elemento.hotstamping;
            decimal precio1 = 0M;
            decimal precio2 = 0M;
            decimal precio3 = 0M;

            try
            {
                Materiales mat1 = new MaterialesContext().Materiales.Find(elemento.IDMaterial);
                precio1 = mat1.Precio;
            }
            catch(Exception err)
            {
               
            }
            try
            {
                Materiales mat2 = new MaterialesContext().Materiales.Find(elemento.IDMaterial2);
                precio2 = mat2.Precio;
            }
            catch (Exception err)
            {

            }

            try
            {
                Materiales mat3 = new MaterialesContext().Materiales.Find(elemento.IDMaterial3);
                precio3 = mat3.Precio;
            }
            catch (Exception err)
            {

            }


            formula.CostoM2Cinta = precio1;
            formula.CostoM2Cinta2 = precio2;
            formula.CostoM2Cinta3 = precio3;
            elemento.CostoM2Cinta = precio1;
            elemento.CostoM2Cinta2 = precio2;
            elemento.CostoM2Cinta3 = precio3;

            formula.LargoCinta = elemento.LargoCinta;

            return formula;
        }

        public FormulaEspecializada.Formulaespecializada pasaTintas(List<Tinta> tintas, FormulaEspecializada.Formulaespecializada formula)
        {
            decimal costoacu = 0;
            formula.Tintas.Clear();
            foreach(Tinta tin in tintas )
            {
                FormulaEspecializada.Tinta nueva = new FormulaEspecializada.Tinta();
                nueva.IDTinta = tin.IDTinta;
                nueva.Area = tin.Area;
             //   tin.kg =formula.getKGtinta(tin.Area);

                nueva.kg = tin.kg;


              //  decimal costo = new ArticuloContext().Database.SqlQuery<decimal>("select dbo.GetCosto(" + nueva.IDTinta + ",1)").ToList().FirstOrDefault();

             //   nueva.Costo = Math.Round( tin.kg * costo,2);
                costoacu += nueva.Costo;
                formula.Tintas.Add(nueva);
            }
            formula.Costodetintas = costoacu;
            return formula;
        }

        public ClsCotizador  pasaTintas(List<Tinta> tintas, ClsCotizador formula)
        {
            decimal costoacu = 0;
            formula.Tintas.Clear();
            foreach (Tinta tin in tintas)
            {
                Tinta nueva = new Tinta();
                nueva.IDTinta = tin.IDTinta;
                nueva.Area = tin.Area;
                //   tin.kg =formula.getKGtinta(tin.Area);

                nueva.kg = tin.kg;


                //  decimal costo = new ArticuloContext().Database.SqlQuery<decimal>("select dbo.GetCosto(" + nueva.IDTinta + ",1)").ToList().FirstOrDefault();

                //   nueva.Costo = Math.Round( tin.kg * costo,2);
                costoacu += nueva.Costo;
                formula.Tintas.Add(nueva);
            }
           // formula.Costodetintas = costoacu;
            return formula;
        }

        public FormulaEspecializada.Formulaespecializada CosteaTintas( FormulaEspecializada.Formulaespecializada formula)
        {
            decimal costoacu = 0;
           // formula.Tintas.Clear();
           List< FormulaEspecializada.Tinta > listanueva = new List<FormulaEspecializada.Tinta>();

            foreach (FormulaEspecializada.Tinta tin in formula.Tintas)
            {
                FormulaEspecializada.Tinta nueva = new FormulaEspecializada.Tinta();
                nueva.IDTinta = tin.IDTinta;
                nueva.Area = tin.Area;
                int metroscuadrados = SIAAPI.Properties.Settings.Default.ValorMtsXkg;
                if (tin.Area==0)
                {
                    tin.Area = 0.01M;
                }
                decimal senecesita = (formula.CantidadMPMts2 * (tin.Area / 100)) / decimal.Parse(metroscuadrados.ToString());
                tin.kg = senecesita;

                nueva.kg = tin.kg;


                  decimal costo = new ArticuloContext().Database.SqlQuery<decimal>("select dbo.GetCosto(0," + nueva.IDTinta + ","+ nueva.kg+")").ToList().FirstOrDefault();

                 nueva.Costo = Math.Round( tin.kg * costo,2);
                costoacu += nueva.Costo;
                listanueva.Add(nueva);
            }
            formula.Costodetintas = costoacu;
            formula.Tintas = listanueva;
            return formula;
        }




        public List<Tinta> Gettintasiniciales( int numerotintas, FormCollection _collection, decimal MetrosCuadrados=0)
        {
            List<Tinta> tintas = new List<Tinta>();
            for(int i=1; i<=numerotintas;i++)
            {
                Tinta nueva = new Tinta();
               
                    try
                    {
                        if (_collection.Get("Tinta" + i) != "")
                        {

                            nueva.IDTinta = int.Parse(_collection.Get("Tinta" + i));

                        }
                    }
                    catch (Exception err1)
                    {
                        string mensjae = err1.Message;
                    }
                    try
                    {
                        if (_collection.Get("Area" + i) != "")
                        {
                            nueva.Area = decimal.Parse(_collection.Get("Area" + i));

                        }
                    }


                    catch (Exception err1)
                    {
                        string mensjae = err1.Message;
                    }
                    if (MetrosCuadrados > 0)
                    {
                        // aqui me voy a traer el costo de la tinta
                    }
                    tintas.Add(nueva);
                
              
            }
            return tintas;
        }

        public List<Tinta> GettintasinicialesyAcabado(int numerotintas, FormCollection _collection, decimal MetrosCuadrados = 0)
        {
            List<Tinta> tintas = new List<Tinta>();
            for (int i = 1; i <= numerotintas; i++)
            {
                Tinta nueva = new Tinta();
                try
                {
                    if (_collection.Get("Tinta" + i) != "")
                    {
                        nueva.IDTinta = int.Parse(_collection.Get("Tinta" + i));

                    }
                }
                catch (Exception err1)
                {
                    string mensjae = err1.Message;
                }
                try
                {
                    if (_collection.Get("Area" + i) != "")
                    {
                        nueva.Area = decimal.Parse(_collection.Get("Area" + i));

                    }
                }


                catch (Exception err1)
                {
                    string mensjae = err1.Message;
                }
                if (MetrosCuadrados > 0)
                {
                    // aqui me voy a traer el costo de la tinta
                }
                tintas.Add(nueva);
            }
            return tintas;
        }

        //public JsonResult GetCosto(int id)
        //{

        //  //  decimal costo = new ArticuloContext().Database.SqlQuery<decimal>("select dbo.costo (")
        //}

        public JsonResult getSuaje(int IDSuaje)
        {
          Caracteristica cara = new ArticuloContext().Database.SqlQuery<Caracteristica>("Select * from Caracteristica where ID=" + IDSuaje).ToList().FirstOrDefault();
            SuajeCaracteristicas suajec = new SuajeCaracteristicas();
           FormulaSiaapi.Formulas formula = new FormulaSiaapi.Formulas();
            suajec.Eje =0;
        
            try
            {
                suajec.Eje =decimal.Parse( formula.getvalor("EJE", cara.Presentacion).ToString());
            }
            catch(Exception err)
            {
                string mensajederror = err.Message;
                suajec.Eje = 0;
            }
            suajec.Avance = 0;
            try
            {
                suajec.Avance = decimal.Parse( formula.getvalor("AVANCE", cara.Presentacion).ToString());
            }
            catch (Exception err)
            {
                string mensajederror = err.Message;
                suajec.Avance = 0;
            }
            suajec.CavidadEje = 2;

            try
            {
                suajec.CavidadEje = int.Parse( formula.getvalor("REP EJE", cara.Presentacion).ToString());
               

            }
            catch (Exception err)
            {
                string mensajederror = err.Message;
                suajec.CavidadEje = 2;
            }



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



            suajec.Gapeje= 0;
            try
            {
                suajec.Gapeje = decimal.Parse( formula.getvalor("GAP EJE", cara.Presentacion).ToString());
              if (suajec.Gapeje==0M)
                {
                    suajec.Gapeje = decimal.Parse(formula.getvalor("GAP EJE ", cara.Presentacion).ToString());
                    
                }
            }
            catch (Exception err)
            {
                string mensajederror = err.Message;
                suajec.Gapeje = 0;
            }



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
            try
            {
                suajec.TH = int.Parse(formula.getvalor("DIENTES_TH", cara.Presentacion).ToString());

            }
            catch (Exception err)
            {
                string mensajederror = err.Message;
                suajec.TH = 0;
            }





            return Json(suajec, JsonRequestBehavior.AllowGet);
        }


        public JsonResult getMP(int IDMp)
        {
          
           
            FormulaEspecializada.Materiales material = new ArticuloContext().Database.SqlQuery<FormulaEspecializada.Materiales>("Select * from Materiales where id=" + IDMp).ToList().FirstOrDefault();
            int? ancho = 1524;
            int? largo = 1524;
            decimal? Costo = 0;
            bool? CobrarMaster = false;

            try
            {
                ancho = material.Ancho;
                largo = material.Largo;
                Costo = material.Precio;
                CobrarMaster = material.Completo;
            }
            catch
            {
                
            }
           

   

            return Json(new { Ancho = ancho, Largo = largo, Costo = Costo, CobrarMaster = CobrarMaster }, JsonRequestBehavior.AllowGet);
        }


        public void GrabarArchivoCotizador(ClsCotizador elemento, string _nombredearchivo, string _ruta)
        {
           
            Caracteristica carac = null;

            if (Session["IDCArticulo"] != null)
            {
                string idc = Session["IDCArticulo"].ToString();
               
                elemento.IDArticulo = int.Parse(idc);

            }
            if (Session["IDCaracteristica"] != null)
            {
                string idc = Session["IDCaracteristica"].ToString();
                carac = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from  caracteristica where ID=" + idc).FirstOrDefault();
                Articulo arti = new ArticuloContext().Articulo.Find(carac.Articulo_IDArticulo);
                elemento.IDCaracteristica = int.Parse(idc);
                elemento.IDArticulo = carac.Articulo_IDArticulo;
              
            }

            StringWriter stringwriter = new StringWriter();
            XmlSerializer x = new XmlSerializer(elemento.GetType());
            x.Serialize(stringwriter, elemento);
            string mensaje = stringwriter.ToString();

            elemento.Descripcion = _nombredearchivo;
            

            Cotizaciones archivo = new Cotizaciones();
           

            string nombredearchivoagrabar = _ruta + "/" + _nombredearchivo +".xml";

            string xmlstring = stringwriter.ToString().Replace("encoding=\"utf-16\"", "");


            EscribeArchivoXML(xmlstring, nombredearchivoagrabar, true);

  

        }

        public ActionResult ArchivoCotizador(string _nombredearchivo, string _ruta, int termo, int suajeN, int suajeE)
        {

            Cotizaciones archivoagrabar = new Cotizaciones();
            archivoagrabar.Fecha = DateTime.Now;
            archivoagrabar.NombreArchivo = _nombredearchivo;
            archivoagrabar.Ruta = _ruta;
            archivoagrabar.tipo = 0;
            archivoagrabar.thermo = termo;
            archivoagrabar.Usuario = User.Identity.Name;


            return View(archivoagrabar);



        }

        [HttpPost]
        public ActionResult ArchivoCotizador(Cotizaciones elemento)
        {

            Articulo arti = null;
            ViewBag.Caracteristica = null;
            ViewBag.Articulo = null;
            Caracteristica carac = null;
            if (Session["IDCaracteristica"] != null)
            {
                string idc = Session["IDCaracteristica"].ToString();
                carac = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from  caracteristica where ID=" + idc).FirstOrDefault();
                arti = new ArticuloContext().Articulo.Find(carac.Articulo_IDArticulo);
                ViewBag.Caracteristica = carac;
                ViewBag.Articulo = arti;
            }
            var db = new ArchivoCotizadorContext();


            db.cotizaciones.Add(elemento);

            if (ModelState.IsValid)
            {
                db.SaveChanges();
            }

            int idcotizacion = db.Database.SqlQuery<ClsDatoEntero>("select Max(ID) as Dato from cotizaciones").FirstOrDefault().Dato;

            try
            {

                db.Database.ExecuteSqlCommand("update Articulo set IDCotizacion=" + idcotizacion + " where articulo.IDArticulo =" + arti.IDArticulo);



            }
            catch (Exception err)
            {
                //return RedirectToAction("CotizadorRapido1", new { id = idcotizacion });

            }
            try
            {
                if (elemento.thermo == 1)
                {
                    return RedirectToAction("Termoencogible", new { id = idcotizacion });
                }
                else
                {
                    return RedirectToAction("CotizadorRapido1", new { id = idcotizacion });
                }
            }
            catch (Exception err)
            {
                if (elemento.thermo == 1)
                {
                    return RedirectToAction("Termoencogible", new { id = idcotizacion });
                }
                else
                {
                    return RedirectToAction("CotizadorRapido1", new { id = idcotizacion });
                }
            }

        }

        public ActionResult Cargar(int Id)
        {
            Session["IDCaracteristica"] = null;
            ClsCotizador elemento;
            Cotizaciones archivocotizacion = new ArchivoCotizadorContext().cotizaciones.Find(Id);
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


            if (elemento.IDSuaje==0 && elemento.IDSuaje2==0 && elemento.mangatermo==false)
            {
                return RedirectToAction("Suajenuevo", new { Id = Id,idsuaje=0 });
            }
            if (elemento.IDSuaje == 0 && elemento.IDSuaje2 == 0 && elemento.mangatermo == true)
            {
                return RedirectToAction("Termoencogible", new { Id = Id, idsuaje = 0 });
            }
            return RedirectToAction("CotizadorRapido1", new { Id = Id });
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

        public ActionResult Asignarunarticulo(int id, string filtro ="")
        {

          

            ViewBag.Caracteristica = null;
            ViewBag.Articulo = null;
            Articulo arti = null;
            if (Session["IDCaracteristica"] != null)
            {
                string idc = Session["IDCaracteristica"].ToString();
                Caracteristica carac = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from caracteristica where id=" + idc).FirstOrDefault();
                arti = new ArticuloContext().Articulo.Find(carac.Articulo_IDArticulo);
                ViewBag.Caracteristica = carac;
                ViewBag.Articulo = arti;
            }

                

            ClsCotizador elemento = new ClsCotizador();
            Cotizaciones archivocotizacion = new ArchivoCotizadorContext().cotizaciones.Find(id);
            XmlDocument documento = new XmlDocument();
            string nombredearchivo = archivocotizacion.Ruta + "\\" + archivocotizacion.NombreArchivo + ".xml";
            documento.Load(nombredearchivo);
            elemento = null;
            XmlSerializer serializer = new XmlSerializer(typeof(ClsCotizador));
            using (Stream reader = new FileStream(nombredearchivo, FileMode.Open))
            {
                // Call the Deserialize method to restore the object's state.
                elemento = (ClsCotizador)serializer.Deserialize(reader);
                reader.Close();
            }

            ViewData["Tintas"] = null;
            try
            {

                ViewData["Tintas"] = elemento.Tintas;
            }
            catch (Exception err)
            {
                string mensaje = err.Message;
            }
            string claveet = "" + new ArticuloContext().Database.SqlQuery<FormulaEspecializada.Materiales>("Select * from Materiales where id=" + elemento.IDMaterial).ToList().FirstOrDefault().ClaveEt;
            claveet = claveet + elemento.anchoproductomm + Math.Round(elemento.largoproductomm,0).ToString();


            if (filtro == "")
            { filtro = claveet;
            }

            ViewBag.IDCotizacion = id;
            ViewBag.filtro = filtro;

            var articulos = new ArticuloContext().Articulo.ToList().Where(s => s.Cref.Contains(filtro) && s.obsoleto==false).OrderBy(s=>s.Cref);

            

            return View(articulos);




        }

        public ActionResult Seleccionar (int id, int idCotizacion)
        {
            Articulo arti= new Articulo();
            ViewBag.Caracteristica = null;
            ViewBag.Articulo = null;
            if (Session["IDCaracteristica"] == null)
            {
                Session["IDCaracteristica"] = id;
                ArticuloContext ac = new ArticuloContext();
                ac.Database.ExecuteSqlCommand("update Caracteristica set idCotizacion=" + idCotizacion + "  where id=" + id);
                
                Caracteristica carac = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from caracteristica where id=" + id).FirstOrDefault();
                arti = ac.Articulo.Find(carac.Articulo_IDArticulo);


                ViewBag.Caracteristica = carac;
                ViewBag.Articulo = arti;

            }
            if (Session["IDCaracteristica"] != null)
            {
                string idc = Session["IDCaracteristica"].ToString();
                Caracteristica carac = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from caracteristica where id=" + idc).FirstOrDefault();

                ArticuloContext ac = new ArticuloContext();
                ac.Database.ExecuteSqlCommand("update Caracteristica set idCotizacion=" + idCotizacion+ "  where id=" + idc);
                arti = ac.Articulo.Find(carac.Articulo_IDArticulo);
                

                ViewBag.Caracteristica = carac;
                ViewBag.Articulo = arti;
            }

            Articulo articulo = arti;

            ClsCotizador elemento = new ClsCotizador();
            Cotizaciones archivocotizacion = new ArchivoCotizadorContext().cotizaciones.Find(idCotizacion);
            XmlDocument documento = new XmlDocument();
            string nombredearchivo = archivocotizacion.Ruta + "\\" + archivocotizacion.NombreArchivo + ".xml";
            documento.Load(nombredearchivo);
            elemento = null;
            XmlSerializer serializer = new XmlSerializer(typeof(ClsCotizador));
            using (Stream reader = new FileStream(nombredearchivo, FileMode.Open))
            {
                // Call the Deserialize method to restore the object's state.
                elemento = (ClsCotizador)serializer.Deserialize(reader);
                reader.Close();
            }

            ViewData["Tintas"] = null;
            try
            {

                ViewData["Tintas"] = elemento.Tintas;
            }
            catch (Exception err)
            {
                string mensaje = err.Message;
            }

            ViewBag.IDFamilia = articulo.IDFamilia;

            ViewBag.IDCotizacion = idCotizacion;

            ViewBag.Cantidad1 = elemento.Rango1;
            ViewBag.Cantidad2 = elemento.Rango2;
            ViewBag.Cantidad3 = elemento.Rango3;
            ViewBag.Cantidad4 = elemento.Rango4;
            ViewBag.precioconvenidos = elemento.precioconvenidos;

            var clientes = new ClienteRepository().GetClientes();

            ViewBag.clientes = clientes;

            return View(articulo);



        }

        [HttpPost]
        public ActionResult Seleccionar(Articulo articulo, FormCollection coleccion)
        {
            ViewBag.Caracteristica = null;
            ViewBag.Articulo = null;
            Caracteristica carac = null;
            if (Session["IDCaracteristica"] != null)
            {
                string idc = Session["IDCaracteristica"].ToString();
               carac = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from caracteristica where id=" + idc).FirstOrDefault();
                Articulo arti = new ArticuloContext().Articulo.Find(carac.Articulo_IDArticulo);
                ViewBag.Caracteristica = carac;
                ViewBag.Articulo = arti;
            }

            var clientes = new ClienteRepository().GetClientes();

            ViewBag.clientes = clientes;


            int IDCotizacion = int.Parse(coleccion.Get("IDCotizacion").ToString());
            int IDCliente = int.Parse(coleccion.Get("IDCliente").ToString());
         //   int precio = int.Parse(coleccion.Get("precio").ToString());

            ViewBag.IDCotizacion = IDCotizacion;


            ClsCotizador elemento = new ClsCotizador();
            Cotizaciones archivocotizacion = new ArchivoCotizadorContext().cotizaciones.Find(IDCotizacion);
            XmlDocument documento = new XmlDocument();
            string nombredearchivo = archivocotizacion.Ruta + "\\" + archivocotizacion.NombreArchivo + ".xml";
            documento.Load(nombredearchivo);
            elemento = null;
            XmlSerializer serializer = new XmlSerializer(typeof(ClsCotizador));
            using (Stream reader = new FileStream(nombredearchivo, FileMode.Open))
            {
                // Call the Deserialize method to restore the object's state.
                elemento = (ClsCotizador)serializer.Deserialize(reader);
                reader.Close();
            }

            ViewData["Tintas"] = null;
            try
            {

                ViewData["Tintas"] = elemento.Tintas;
            }
            catch (Exception err)
            {
                string mensaje = err.Message;
            }


            try
            {
                int IDArticulo = articulo.IDArticulo;

               

                if (IDCliente > 0)
                {
                   
                        db.Database.ExecuteSqlCommand("delete from [MatrizPrecioCliente]  where IDArticulo=" + IDArticulo + " and IDCliente="+ IDCliente);
                        db.Database.ExecuteSqlCommand("insert into MatrizPrecioCliente ( IDArticulo, Ranginf, RangSup, Precio,IDCliente ) values (" + IDArticulo + "," + elemento.Rango1 + "," + elemento.Rango2 + "," + elemento.precioconvenidos.precio1 + ","+IDCliente+")");
                        db.Database.ExecuteSqlCommand("insert into MatrizPrecioCliente ( IDArticulo, Ranginf, RangSup, Precio ,IDCliente) values (" + IDArticulo + "," + elemento.Rango2 + "," + elemento.Rango3 + "," + elemento.precioconvenidos.precio2 + "," + IDCliente+ ")");
                        db.Database.ExecuteSqlCommand("insert into MatrizPrecioCliente ( IDArticulo, Ranginf, RangSup, Precio,IDCliente ) values (" + IDArticulo + "," + elemento.Rango3 + "," + elemento.Rango4 + "," + elemento.precioconvenidos.precio3 + "," + IDCliente + ")");
                        db.Database.ExecuteSqlCommand("insert into MatrizPrecioCliente ( IDArticulo, Ranginf, RangSup, Precio ,IDCliente) values (" + IDArticulo + "," + elemento.Rango4 + ",5000," + elemento.precioconvenidos.precio4 + "," + IDCliente + ")");



                }
            }
            catch (Exception err)
            {

                
                ViewBag.IDFamilia = articulo.IDFamilia;
                ViewBag.Cantidad1 = elemento.Rango1;
                ViewBag.Cantidad2 = elemento.Rango2;
                ViewBag.Cantidad3 = elemento.Rango3;
                ViewBag.Cantidad4 = elemento.Rango4;
                ViewBag.MatrizPrecio = elemento.MatrizPrecio;

                ViewBag.errorpersonalizado = err.Message;
                if (err.Message.Contains("EntityValidationErrors"))
                {
                    ViewBag.errorpersonalizado = "";
                }
                return View(articulo);
            }

            if (ViewBag.Caracteristica == null)
            {
                return RedirectToAction("Addpresentacion", "Cotizador", new { centro=elemento.IDCentro, IDFamilia = articulo.IDFamilia, IDArticulo = articulo.IDArticulo, ETIQUETAXR = elemento.Cantidadxrollo, ancho = elemento.anchoproductomm, largo = elemento.largoproductomm, alpaso = elemento.productosalpaso, notintas = elemento.Tintas.Count, Cref = articulo.Cref, Descripcion = articulo.Descripcion, IDCliente = IDCliente, IDCotizacion = IDCotizacion });
            }
            else
            {
                try
                {
                    new CotizacionContext().Database.ExecuteSqlCommand("udpdate carcateristica set IDcotizacion=" + coleccion.Get("IDCotizacion").ToString() + " where ID=" + carac.ID);
                }
                catch(Exception err)
                {

                }
                return RedirectToAction("GetPresentacionesTienda", "Tienda", new { searchString = articulo.Cref, page = 1 });
            }

        }


        public ActionResult CreaArticulo (int id)
        {
            ClsCotizador elemento = new ClsCotizador();
            Cotizaciones archivocotizacion = new ArchivoCotizadorContext().cotizaciones.Find(id);
            XmlSerializer serializerX = new XmlSerializer(typeof(ClsCotizador));
            //bool isDebug = false;
            //Debug.Assert(isDebug = true);
            //if (!isDebug)
            //{
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
            //}
            //else
            //{
            //    elemento = (ClsCotizador)serializerX.Deserialize(StringExtensiones.ToXmlReader(archivocotizacion.contenido));
            //}


            ViewData["Tintas"] = null;
            try
            {

                ViewData["Tintas"] = elemento.Tintas;
            }
            catch (Exception err)
            {
                string mensaje = err.Message;
            }

            int numeroTintas = elemento.Tintas.Count;

            if (elemento.ACABADO == "BARNIZ BRILLANTE" || elemento.ACABADO == "BARNIZ MATE" || elemento.ACABADO == "BARNIZ RELEASE")
            {
                numeroTintas = elemento.Tintas.Count - 1;
            }
            var familiasQuery = from d in new ArticuloContext().Familias
                                where d.Descripcion.Substring(0, 8).ToLower()=="etiqueta" /// ("Etiqueta")
                                orderby d.Descripcion
                                select d;

            ViewBag.IDFamilia = new SelectList(familiasQuery, "IDFamilia", "Descripcion", 0);


            Materiales mate = new MaterialesContext().Materiales.Find(elemento.IDMaterial);
            string claveet =  "" + mate.ClaveEt;
           
            
            
            claveet = claveet + Math.Round(elemento.anchoproductomm,0) + Math.Round( elemento.largoproductomm,0).ToString();

            Articulo articulo = new Articulo();
            articulo.Cref = claveet;
            articulo.Descripcion = "ETIQUETA ";
            if (elemento.Tintas.Count==0)
            {
                articulo.Descripcion += "BLANCA ";
            }
            else
            {
                
                articulo.Descripcion += "IMPRESA A " + numeroTintas + " TINTAS";
            }

            articulo.Descripcion += " EN " + mate.Descripcion + " DE " + elemento.anchoproductomm + " X " + elemento.largoproductomm;

            if (elemento.ACABADO=="N/A" || elemento.ACABADO==string.Empty)
            {
                articulo.Descripcion += " SIN ACABADO";
            }
            else
            {
                articulo.Descripcion += elemento.ACABADO;
            }


           articulo.Obscalidad = "";
            articulo.obsoleto = false;
            articulo.ExistenDev = false;
            articulo.esKit = false;
            articulo.GeneraOrden = true;
            articulo.MinimoVenta = elemento.Minimoproducir;
            articulo.MinimoCompra = 0;
            articulo.IDClaveUnidad = 63;
            articulo.ManejoCar = true;
            articulo.CtrlStock = true;
            articulo.IDMoneda = 181;
            articulo.Preciounico = false;
            articulo.IDTipoArticulo = 1;
            articulo.bCodigodebarra = true;
            articulo.IDAQL = 1;
            articulo.IDInspeccion = 2;
            articulo.IDMuestreo = 3;

            ViewBag.IDCotizacion = id;

            ViewBag.Cantidad1 = elemento.Rango1;
            ViewBag.Cantidad2 = elemento.Rango2;
            ViewBag.Cantidad3 = elemento.Rango3;
            ViewBag.Cantidad4 = elemento.Rango4;
            ViewBag.precioconvenidos = elemento.precioconvenidos;


            var clientes = new ClienteRepository().GetClientes();

            ViewBag.clientes = clientes;

            return View(articulo);




        }

        [HttpPost]
        public ActionResult CreaArticulo(Articulo articulo, FormCollection coleccion)
        {
            int IDCotizacion = int.Parse( coleccion.Get("IDCotizacion").ToString());
            int IDCliente = int.Parse(coleccion.Get("IDCliente").ToString());
          //  int precio = int.Parse(coleccion.Get("precio").ToString());

            ViewBag.IDCotizacion = IDCotizacion;

            var clientes = new ClienteRepository().GetClientes();

            ViewBag.clientes = clientes;

            ClsCotizador elemento = new ClsCotizador();
            Cotizaciones archivocotizacion = new ArchivoCotizadorContext().cotizaciones.Find(IDCotizacion);
            XmlSerializer serializerX = new XmlSerializer(typeof(ClsCotizador));
            //bool isDebug = false;
            //Debug.Assert(isDebug = true);
            //if (!isDebug)
            //{
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

            ViewData["Tintas"] = null;
            try
            {

                ViewData["Tintas"] = elemento.Tintas;
            }
            catch (Exception err)
            {
                string mensaje = err.Message;
            }


            try
            {



                ArticuloContext dba = new ArticuloContext();

                Articulo verificaarticulo = new ArticuloContext().Database.SqlQuery<Articulo>("select * from Articulo where Cref='" + articulo.Cref + "'").ToList().FirstOrDefault();

                if (verificaarticulo == null)
                {
                    string Cref = "";
                    string Descripcion = "";
                     if (articulo.Cref.Contains("'"))
                    {
                        Cref = articulo.Cref.Replace("'", "''");
                    }
                    else
                    {
                        Cref = articulo.Cref;
                    }
                   if (articulo.Descripcion.Contains("'"))
                    {
                        Descripcion = articulo.Descripcion.Replace("'", "''");
                    }
                    else
                    {
                        Descripcion = articulo.Descripcion;
                    }
                    articulo.Cref = Cref;
                    articulo.Descripcion = Descripcion;
                    articulo.IDAQL = 1;
                    articulo.IDInspeccion =2;
                    articulo.IDMuestreo =3;
                    dba.Articulo.Add(articulo);
                    dba.SaveChanges();

                    int IDArticulo = new ArticuloContext().Database.SqlQuery<Articulo>("select * from Articulo where Cref='" + articulo.Cref + "'").ToList().FirstOrDefault().IDArticulo;

                  

                        db.Database.ExecuteSqlCommand("insert into MatrizPrecio ( IDArticulo, Ranginf, RangSup, Precio ) values (" + IDArticulo + "," + elemento.Rango1 + "," + elemento.Rango2 + "," + elemento.precioconvenidos.precio1 + ")");
                        db.Database.ExecuteSqlCommand("insert into MatrizPrecio ( IDArticulo, Ranginf, RangSup, Precio ) values (" + IDArticulo + "," + elemento.Rango2 + "," + elemento.Rango3 + "," + elemento.precioconvenidos.precio2 + ")");
                        db.Database.ExecuteSqlCommand("insert into MatrizPrecio ( IDArticulo, Ranginf, RangSup, Precio ) values (" + IDArticulo + "," + elemento.Rango3 + "," + elemento.Rango4 + "," + elemento.precioconvenidos.precio3 + ")");
                        db.Database.ExecuteSqlCommand("insert into MatrizPrecio ( IDArticulo, Ranginf, RangSup, Precio ) values (" + IDArticulo + "," + elemento.Rango4 + ",5000," + elemento.precioconvenidos.precio4 + ")");


                 
                    db.Database.ExecuteSqlCommand("insert into MatrizCosto ( IDArticulo, Ranginf, RangSup, Precio ) values (" + IDArticulo + "," + elemento.Rango1 + "," + elemento.Rango2 + "," + elemento.Costo1 + ")");
                    db.Database.ExecuteSqlCommand("insert into MatrizCosto ( IDArticulo, Ranginf, RangSup, Precio ) values (" + IDArticulo + "," + elemento.Rango2 + "," + elemento.Rango3 + "," + elemento.Costo2 + ")");
                    db.Database.ExecuteSqlCommand("insert into MatrizCosto ( IDArticulo, Ranginf, RangSup, Precio ) values (" + IDArticulo + "," + elemento.Rango3 + "," + elemento.Rango4 + "," + elemento.Costo3 + ")");
                    db.Database.ExecuteSqlCommand("insert into MatrizCosto ( IDArticulo, Ranginf, RangSup, Precio ) values (" + IDArticulo + "," + elemento.Rango4 + ",5000," + elemento.Costo4 + ")");


                    if (IDCliente > 0)
                    {
                       
                            db.Database.ExecuteSqlCommand("delete from [MatrizPrecioCliente]  where IDArticulo=" + IDArticulo + " and IDCliente=" + IDCliente);
                            db.Database.ExecuteSqlCommand("insert into MatrizPrecioCliente ( IDArticulo, Ranginf, RangSup, Precio,IDCliente ) values (" + IDArticulo + "," + elemento.Rango1 + "," + elemento.Rango2 + "," + elemento.precioconvenidos.precio1 + "," + IDCliente + ")");
                            db.Database.ExecuteSqlCommand("insert into MatrizPrecioCliente ( IDArticulo, Ranginf, RangSup, Precio ,IDCliente) values (" + IDArticulo + "," + elemento.Rango2 + "," + elemento.Rango3 + "," + elemento.precioconvenidos.precio2 + "," + IDCliente + ")");
                            db.Database.ExecuteSqlCommand("insert into MatrizPrecioCliente ( IDArticulo, Ranginf, RangSup, Precio,IDCliente ) values (" + IDArticulo + "," + elemento.Rango3 + "," + elemento.Rango4 + "," + elemento.precioconvenidos.precio3 + "," + IDCliente + ")");
                            db.Database.ExecuteSqlCommand("insert into MatrizPrecioCliente ( IDArticulo, Ranginf, RangSup, Precio ,IDCliente) values (" + IDArticulo + "," + elemento.Rango4 + ",5000," + elemento.precioconvenidos.precio4 + "," + IDCliente + ")");
  

                    }

                }
                else
                {
                    throw new Exception("Hay una articulo con esa referencia");
                }



            }
            catch (Exception err)
            {
              
                    
                var familiasQuery = from d in new ArticuloContext().Familias
                                    where d.Descripcion.Substring(0, 8) == "Etiqueta" /// ("Etiqueta")
                                    orderby d.Descripcion
                                    select d;
                ViewBag.IDFamilia = new SelectList(familiasQuery, "IDFamilia", "Descripcion", 0);
                ViewBag.Cantidad1 = elemento.Rango1;
                ViewBag.Cantidad2 = elemento.Rango2;
                ViewBag.Cantidad3 = elemento.Rango3;
                ViewBag.Cantidad4 = elemento.Rango4;
                ViewBag.MatrizPrecio = elemento.MatrizPrecio;

                ViewBag.errorpersonalizado = err.Message;
                if (err.Message.Contains("EntityValidationErrors"))
                {
                    ViewBag.errorpersonalizado = "";
                }
                return View(articulo);
            }

            int numeroTintas = elemento.Tintas.Count;

            if (elemento.ACABADO == "BARNIZ BRILLANTE" || elemento.ACABADO == "BARNIZ MATE" || elemento.ACABADO == "BARNIZ RELEASE")
            {
                numeroTintas = elemento.Tintas.Count - 1;
            }

            return RedirectToAction("Addpresentacion", "Cotizador", new { centro=elemento.IDCentro , IDFamilia = articulo.IDFamilia, IDArticulo = articulo.IDArticulo, ETIQUETAXR= elemento.Cantidadxrollo, ancho= elemento.anchoproductomm, largo= elemento.largoproductomm, ACABADO = elemento.ACABADO,alpaso = elemento.productosalpaso, notintas = numeroTintas,  Cref = articulo.Cref, Descripcion = articulo.Descripcion, IDCliente= IDCliente, IDCotizacion=IDCotizacion });

        }




        public ActionResult Addpresentacion(int? IDFamilia, int? IDArticulo, int ETIQUETAXR, decimal ancho, decimal largo, int alpaso, int notintas, string Cref = "", string Descripcion = "", int IDCotizacion = 0, int IDCliente = 0, string ACABADO = "N/A", string Embobinado = "A", int centro=0)
        {
            List<AtributodeFamilia> atributos = db.Database.SqlQuery<AtributodeFamilia>("select * from AtributodeFamilia where IDFamilia='" + IDFamilia + "'").ToList();
            Articulo centro2 = new ArticuloContext().Articulo.Find(centro);

                ViewBag.Cref = Cref;
            ViewBag.Descripcion = Descripcion;
            ViewBag.IDArticulo = IDArticulo;
            ViewBag.ETIQUETAXR = ETIQUETAXR;
            ViewBag.ancho = ancho;
            ViewBag.largo = largo;
            ViewBag.alpaso = alpaso;
            ViewBag.notintas = notintas;
            ViewBag.IDCotizacion = IDCotizacion;
            ViewBag.IDCliente = IDCliente;
            ViewBag.Acabado = ACABADO;
            ViewBag.Embobinado = Embobinado;
            


            ViewBag.Cliente = new ClientesContext().Clientes.Find(IDCliente);
            return View(atributos);
        }

        public ActionResult ArchivoCotizadorN(string _nombredearchivo, string _ruta, int suajeN)
        {

            Cotizaciones archivoagrabar = new Cotizaciones();
            archivoagrabar.Fecha = DateTime.Now;
            archivoagrabar.NombreArchivo = _nombredearchivo;
            archivoagrabar.Ruta = _ruta;
            archivoagrabar.tipo = 0;
            archivoagrabar.suajenuevo = suajeN;
            archivoagrabar.Usuario = User.Identity.Name;


            return View(archivoagrabar);



        }

        [HttpPost]
        public ActionResult ArchivoCotizadorN(Cotizaciones elemento)
        {

            Articulo arti = null;
            ViewBag.Caracteristica = null;
            ViewBag.Articulo = null;
            Caracteristica carac = null;
            if (Session["IDCaracteristica"] != null)
            {
                string idc = Session["IDCaracteristica"].ToString();
                carac = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from  caracteristica where ID=" + idc).FirstOrDefault();
                arti = new ArticuloContext().Articulo.Find(carac.Articulo_IDArticulo);
                ViewBag.Caracteristica = carac;
                ViewBag.Articulo = arti;
            }
            var db = new ArchivoCotizadorContext();


            db.cotizaciones.Add(elemento);




            db.SaveChanges();

            int idcotizacion = db.Database.SqlQuery<ClsDatoEntero>("select Max(ID) as Dato from cotizaciones").FirstOrDefault().Dato;
            try
            {
                XmlDocument documento = new XmlDocument();
                string nombredearchivo = elemento.Ruta.TrimEnd() + "\\" + elemento.NombreArchivo + ".xml";
                documento.Load(nombredearchivo);

                db.Database.ExecuteSqlCommand("update cotizaciones set contenido='" + documento.OuterXml + "' where id=" + idcotizacion);
            }
            catch (Exception err)
            {

            }


            try
            {

                db.Database.ExecuteSqlCommand("update Articulo set IDCotizacion=" + idcotizacion + " where articulo.IDArticulo =" + arti.IDArticulo);


                return RedirectToAction("SuajeNuevo", new { Id = idcotizacion });
            }
            catch (Exception err)
            {
                return RedirectToAction("SuajeNuevo", new { Id = idcotizacion });

            }


        }

        public ActionResult Addpresentacionp(int idarticulo, string  Presentacion, int IDCliente=0, int IDCotizacion=0 )
        {


            try
            {
                Articulo articulo = new ArticuloContext().Articulo.Find(idarticulo);
                ViewBag.nombrearticulo = articulo.Descripcion;
                ViewBag.idarticulo = idarticulo;

                Presentacion = Presentacion.TrimEnd(',');
                string[] arraydatos;

                arraydatos = Presentacion.Split(',');
                string acc = null;
                for (int i = 0; i < arraydatos.Length; i++)
                {
                    string cuenta = arraydatos[i];

                    string[] arraydatoscortados;
                    arraydatoscortados = cuenta.Split(':');
                    for (int j = 0; j < arraydatoscortados.Length; j++)
                    {

                        string dato = "?" + arraydatoscortados[j] + "?";

                        if (j + 1 == arraydatoscortados.Length)
                        {
                            acc = acc + dato + ",";
                        }
                        else
                        {
                            acc = acc + dato + ":";
                        }



                    }


                }
                string quitarcoma = acc.TrimEnd(',');
                string jsonPresentacion = "{" + quitarcoma.Replace('?', '"') + "}";
                int NewIDP = db.Database.SqlQuery<int>("SELECT ISNULL(MAX(IDPRESENTACION)+1,0) from Caracteristica where Articulo_IDArticulo = " + idarticulo + " ").FirstOrDefault();
                //int NewIDP = Convert.ToInt32(db.Database.SqlQuery("Select MAX(IDPresentacion)+1 from Caracteristica where Articulo_IDArticulo = ", pre.Articulo_IDArticulo));
                NewIDP = NewIDP > 0 ? NewIDP : 1;
                db.Database.ExecuteSqlCommand("insert into Caracteristica ( IDPresentacion, Cotizacion, version, Presentacion, jsonPresentacion, Articulo_IDArticulo, IDCotizacion )  values (" + NewIDP + ",0,0,'" + Presentacion + "','" + jsonPresentacion + "'," + idarticulo + "," + IDCotizacion +")");

                //int IDPresentacionnueva = db.Database.SqlQuery<ClsDatoEntero>(" Select ID as Dato from Caracteristica where Articulo_IDArticulo =" + idarticulo + " and IDpresentacion =" + NewIDP).ToList().FirstOrDefault().Dato;

                //int nhe = creaplaneacion(IDCotizacion, IDCliente, IDPresentacionnueva);

                ////HEspecificacionE hoja = new HEspecificacionEContext().HEspecificacionesE.Find(nhe);
                //try
                //{
                //    db.Database.ExecuteSqlCommand("update  Caracteristica set  version=" + hoja.Version + ", Cotizacion=" + hoja.Planeacion + " where ID =" + IDPresentacionnueva);
                //}
                //catch(Exception err)
                //{
                //    string mensajeerror = err.Message;
                //}
                return RedirectToAction("index");

            }
            catch (Exception err)
            {
                string mensajedeerror = err.Message;
                return RedirectToAction("index");
            }
        }
        /// <summary>
        /// crea la presentacion 
        /// </summary>
        /// <param name="IDCotizacion"></param>
        /// <param name="IDCliente"></param>
        /// <param name="IDPresentacion"></param>
        /// <returns> el id de la hoja de especificacion</returns>


        public int creaplaneacion (int IDCotizacion, int IDCliente, int IDPresentacion)
        {


            ClsCotizador elemento = new ClsCotizador();
            Cotizaciones archivocotizacion = new ArchivoCotizadorContext().cotizaciones.Find(IDCotizacion);
            XmlDocument documento = new XmlDocument();
            string nombredearchivo = archivocotizacion.Ruta + "\\" + archivocotizacion.NombreArchivo + ".xml";
            documento.Load(nombredearchivo);
            elemento = null;
            XmlSerializer serializer = new XmlSerializer(typeof(ClsCotizador));
            using (Stream reader = new FileStream(nombredearchivo, FileMode.Open))
            {
                // Call the Deserialize method to restore the object's state.
                elemento = (ClsCotizador)serializer.Deserialize(reader);
                reader.Close();
            }


            int modelo = 4;
            Plantilla plantilla= new Plantilla();

            if (elemento.Tintas.Count == 0 && elemento.mangatermo == false)
            {
                modelo = 4;
                try
                {
                    plantilla = Modelo4(IDCotizacion, IDPresentacion);
                }
                catch(Exception err)
                {
                    string mensajeerror = err.Message;
                }
            }

            if (elemento.Tintas.Count > 0 && elemento.mangatermo == false)
            {
                modelo = 5;
                try
                {
                    plantilla = Modelo5(IDCotizacion, IDPresentacion);
                }
                catch (Exception err)
                {
                    string mensajeerror = err.Message;
                }
            }

            if ( elemento.mangatermo)
            {
                modelo = 8;
                try
                {
                    plantilla = Modelo8(IDCotizacion);
                }
                catch (Exception err)
                {
                    string mensajeerror = err.Message;
                }
            }
            int he = 0;
            try
            {
               he  = creaHE(IDCotizacion, IDCliente, IDPresentacion, plantilla, modelo);
                string cadena1 = "INSERT INTO [dbo].[RangoPlaneacionCosto] ([IDHE]  ,[RangoInf] ,[RangoSup] ,[Precio] ,[Costo] ,[Version]  ,[IDMoneda]) VALUES (" + he + ",1," + elemento.Rango1 + ",0,0,0,181)";
                db.Database.ExecuteSqlCommand(cadena1);
                db.Database.ExecuteSqlCommand("INSERT INTO [dbo].[RangoPlaneacionCosto] ([IDHE]  ,[RangoInf] ,[RangoSup] ,[Precio] ,[Costo] ,[Version]  ,[IDMoneda]) VALUES (" + he + "," +elemento.Rango1+"," + elemento.Rango2 + ",0,0,0," + elemento.IDMonedapreciosconvenidos +")");
                db.Database.ExecuteSqlCommand("INSERT INTO [dbo].[RangoPlaneacionCosto] ([IDHE]  ,[RangoInf] ,[RangoSup] ,[Precio] ,[Costo] ,[Version]  ,[IDMoneda]) VALUES (" + he + "," + elemento.Rango2 + "," + elemento.Rango3 + ",0,0,0," + elemento.IDMonedapreciosconvenidos + ")");
                db.Database.ExecuteSqlCommand("INSERT INTO [dbo].[RangoPlaneacionCosto] ([IDHE]  ,[RangoInf] ,[RangoSup] ,[Precio] ,[Costo] ,[Version]  ,[IDMoneda]) VALUES (" + he + "," + elemento.Rango3 + "," + elemento.Rango4 + ",0,0,0," + elemento.IDMonedapreciosconvenidos + ")");
                db.Database.ExecuteSqlCommand("INSERT INTO [dbo].[RangoPlaneacionCosto] ([IDHE]  ,[RangoInf] ,[RangoSup] ,[Precio] ,[Costo] ,[Version]  ,[IDMoneda]) VALUES (" + he + "," + elemento.Rango4 + ",9999,0,0,0," + elemento.IDMonedapreciosconvenidos + ")");
            }
            catch (Exception eer)
            {
                string mensajedeerror = eer.Message;
            }
         

            return he;

        }

        /// <summary>
        /// retorna la atipica del material  en su defecto tambien crea el articulo
        /// si no existe lo crea
        /// </summary>
        /// <param name="clave"></param>
        /// <param name="ancho"></param>
        /// <param name="largo"></param>
        /// <returns></returns>
        public int verificamaterial(string clave, int ancho, int largo, string fam, string Material, decimal Costo)
        {
            ArticuloContext db = new ArticuloContext();
            string clavemat = clave + "-" + ancho;

            int IDArticulo=0;

            List<Articulo> arti = db.Database.SqlQuery<Articulo>("select * from Articulo where cref='" + clavemat + "'").ToList();

            if (arti.Count>0)
            {
                IDArticulo = arti.FirstOrDefault().IDArticulo;
          
            } 
             else /// no existe el articulo
            {
                int IDFam = db.Database.SqlQuery<Familia>("select * from Familia where ccodfam='" + fam + "'").ToList().FirstOrDefault().IDFamilia;
                if (IDFam==0)
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



               
                IDArticulo =  db.Database.SqlQuery<Articulo>("select * from Articulo where cref='" + clavemat + "'").ToList().FirstOrDefault().IDArticulo;

                db.Database.ExecuteSqlCommand("delete from MatrizCosto  where IDArticulo=" + IDArticulo);
                db.Database.ExecuteSqlCommand("insert into MatrizCosto ( IDArticulo, Ranginf, RangSup, Precio ) values (" + IDArticulo + ",0,999999," + Costo + ")");

                db.Database.ExecuteSqlCommand("delete from MatrizPrecio  where IDArticulo=" + IDArticulo);
                db.Database.ExecuteSqlCommand("insert into MatrizPrecio ( IDArticulo, Ranginf, RangSup, Precio ) values (" + IDArticulo + ",0,999999," + Costo * 2 + ")");




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
                return retornar;

            }


         

        }

        /// <summary>
        /// Modelo 4 etiqueta blanca
        /// </summary>
        /// <param name="IDCotizacion"></param>
        /// <param name="IDCaracteristicaGrabandose"> IDCaracteristica del Articulo que se esta trabajando</param>
        /// <returns></returns>
        public Plantilla Modelo4(int IDCotizacion, int IDCaracteristicaGrabandose)
        {

            ClsCotizador elemento = new ClsCotizador();
            Cotizaciones archivocotizacion = new ArchivoCotizadorContext().cotizaciones.Find(IDCotizacion);
            XmlDocument documento = new XmlDocument();
            string nombredearchivo = archivocotizacion.Ruta + "\\" + archivocotizacion.NombreArchivo + ".xml";
            documento.Load(nombredearchivo);
            elemento = null;
            XmlSerializer serializer = new XmlSerializer(typeof(ClsCotizador));
            using (Stream reader = new FileStream(nombredearchivo, FileMode.Open))
            {
                // Call the Deserialize method to restore the object's state.
                elemento = (ClsCotizador)serializer.Deserialize(reader);
                reader.Close();
            }
           
            Plantilla plantilla = new Plantilla();


            /////// proceso prensa suaje materiales y tintas ********************************
            ///////// manode obra y maquina de prensa
            /********************  mano de obra *******************/



        

            ArticuloXML nuevomanodeobra = new ArticuloXML();

            nuevomanodeobra.IDArticulo = "2916";
            nuevomanodeobra.IDCaracteristica = "3972";
            nuevomanodeobra.Formula = "(C*" + Math.Round((elemento.Cantidad / elemento.MaterialNecesitado),4) +")/" +Math.Round( (elemento.HrPrensa == 0 ? elemento.MaterialNecesitado : elemento.HrPrensa / elemento.MaterialNecesitado),6);
            if (elemento.Numerodetintas>0)
                {
                nuevomanodeobra.Formula = "((C*" + (elemento.Cantidad / elemento.MaterialNecesitado) + ")+"+ elemento.Numerodetintas +" * 60)/" + (elemento.HrPrensa == 0 ? elemento.MaterialNecesitado : elemento.HrPrensa / elemento.MaterialNecesitado);
            }

            nuevomanodeobra.IDTipoArticulo = "5";
            nuevomanodeobra.IDProceso = "5";
            nuevomanodeobra.FactorCierre = "0.25";



            plantilla.Articulos.Add(nuevomanodeobra);


            /*********************************   maquina *********************************/


                ArticuloXML nuevomaquina = new ArticuloXML();

                nuevomaquina.IDArticulo = "87";
                nuevomaquina.IDCaracteristica = "32";

                 
                nuevomaquina.Formula = "(C*" +Math.Round( (elemento.Cantidad / elemento.MaterialNecesitado),4) + ")/" +Math.Round( (elemento.HrPrensa == 0 ? elemento.MaterialNecesitado : elemento.HrPrensa / elemento.MaterialNecesitado),6);
                if (elemento.Numerodetintas > 0)
                {
                    nuevomaquina.Formula = "((C*" + Math.Round((elemento.Cantidad / elemento.MaterialNecesitado), 4) + ")+" + elemento.Numerodetintas + " * 60)/" + Math.Round((elemento.HrPrensa == 0 ? elemento.MaterialNecesitado : elemento.HrPrensa / elemento.MaterialNecesitado), 6);
                }

                nuevomaquina.IDTipoArticulo = "3";
                nuevomaquina.IDProceso = "5";
                nuevomaquina.FactorCierre = "0.25";

                plantilla.Articulos.Add(nuevomaquina);
          

         
            /***************************************  suaje ****************************/

            ArticuloXML nuevoplaneacion = new ArticuloXML();

            Caracteristica CIDSuaje = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from Caracteristica where ID="+elemento.IDSuaje).ToList().FirstOrDefault();

            int IDSuaje = CIDSuaje.Articulo_IDArticulo;

            nuevoplaneacion.IDArticulo = IDSuaje.ToString();
            nuevoplaneacion.IDCaracteristica = elemento.IDSuaje.ToString();
            nuevoplaneacion.IDTipoArticulo = "2";
            nuevoplaneacion.IDProceso = "5";
            nuevoplaneacion.Formula = "1/50";
            nuevoplaneacion.Indicaciones = "";

            if (IDSuaje > 0)
            {
                plantilla.Articulos.Add(nuevoplaneacion);
            }



            Materiales material = new MaterialesContext().Materiales.Find(elemento.IDMaterial);


            int IDMaterial = verificamaterial(material.Clave, elemento.anchomaterialenmm, material.Largo, material.Fam, material.Descripcion, material.Precio);

            ArticuloXML nuevoplaneacion2 = new ArticuloXML();

            int IDMaterialArticulo = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from Caracteristica where ID=" + IDMaterial).ToList().FirstOrDefault().Articulo_IDArticulo; 

            nuevoplaneacion2.IDArticulo = IDMaterialArticulo.ToString();
            nuevoplaneacion2.IDCaracteristica = IDMaterial.ToString();
            nuevoplaneacion2.IDTipoArticulo = "6";
            nuevoplaneacion2.IDProceso = "5";

            decimal cantidadmillar = (elemento.CantidadMPMts2 / elemento.Cantidad );



            nuevoplaneacion2.Formula = "(C*" + Math.Round( cantidadmillar,4) + ")";






            nuevoplaneacion2.Indicaciones = "";


            plantilla.Articulos.Add(nuevoplaneacion2);

            if (elemento.IDMaterial2 > 0)
            {

                ArticuloXML nuevoplaneacion3 = new ArticuloXML();


                Materiales material2 = new MaterialesContext().Materiales.Find(elemento.IDMaterial2);
                int IDMaterial2 = verificamaterial(material2.Clave, elemento.anchomaterialenmm, material.Largo, material.Fam, material.Descripcion, material.Precio);

                int IDMaterialArticulo2 = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from Caracteristica where ID=" + IDMaterial2).ToList().FirstOrDefault().Articulo_IDArticulo;

                nuevoplaneacion3.IDArticulo = IDMaterialArticulo2.ToString();
                nuevoplaneacion3.IDCaracteristica = IDMaterial2.ToString();
                nuevoplaneacion3.IDTipoArticulo = "6";
                nuevoplaneacion3.IDProceso = "5";

                decimal cantidadmillar2 = Math.Round((elemento.CantidadMPMts2 / elemento.Cantidad  ), 4);



                nuevoplaneacion3.Formula = "(C*" + cantidadmillar2 + ")";


                nuevoplaneacion3.Indicaciones = "";


                plantilla.Articulos.Add(nuevoplaneacion3);
            }


            if (elemento.IDMaterial3 > 0)
            {

                ArticuloXML nuevoplaneacion3 = new ArticuloXML();


                Materiales material3 = new MaterialesContext().Materiales.Find(elemento.IDMaterial3);
                int IDMaterial3 = verificamaterial(material3.Clave, elemento.anchomaterialenmm, material3.Largo, material.Fam, material3.Descripcion, material.Precio);

                int IDMaterialArticulo3 = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from Caracteristica where ID=" + IDMaterial3).ToList().FirstOrDefault().Articulo_IDArticulo;

                nuevoplaneacion3.IDArticulo = IDMaterialArticulo3.ToString();
                nuevoplaneacion3.IDCaracteristica = IDMaterial3.ToString();
                nuevoplaneacion3.IDTipoArticulo = "6";
                nuevoplaneacion3.IDProceso = "5";

                decimal cantidadmillar2 = Math.Round((elemento.CantidadMPMts2 / elemento.Cantidad), 4);



                nuevoplaneacion3.Formula = "(C*" + cantidadmillar2 + ")";


                nuevoplaneacion3.Indicaciones = "";


                plantilla.Articulos.Add(nuevoplaneacion3);
            }


            /**** proceso embobinado *******************/

            ArticuloXML nuevotiempoembobinado = new ArticuloXML();



            nuevotiempoembobinado.IDArticulo = 226.ToString();
            nuevotiempoembobinado.IDCaracteristica = 215.ToString();
            nuevotiempoembobinado.IDTipoArticulo = "5";
            nuevotiempoembobinado.IDProceso = "4";
            nuevotiempoembobinado.Formula = "C*" + Math.Round ((elemento.HrEmbobinado/elemento.Cantidad  ),4)+"";
            nuevotiempoembobinado.FactorCierre = "0.1666";
            nuevotiempoembobinado.Indicaciones = "";


            plantilla.Articulos.Add(nuevotiempoembobinado);



            ArticuloXML nuevomaquinaenbobinado = new ArticuloXML();

            nuevomaquinaenbobinado.IDArticulo = "91";
            nuevomaquinaenbobinado.IDCaracteristica = "3970";


            nuevomaquinaenbobinado.Formula = "C*" + Math.Round((elemento.HrEmbobinado / elemento.Cantidad), 4) + "";


            nuevomaquinaenbobinado.IDTipoArticulo = "3";
            nuevomaquinaenbobinado.IDProceso = "4";
            nuevomaquinaenbobinado.FactorCierre = "0.25";

            plantilla.Articulos.Add(nuevomaquinaenbobinado);



            ArticuloXML centros = new ArticuloXML();

            FormulaSiaapi.Formulas formu = new FormulaSiaapi.Formulas();

            Caracteristica caragrabandose = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from Caracteristica where ID=" + IDCaracteristicaGrabandose).ToList().FirstOrDefault();

            string presentacion = caragrabandose.Presentacion;

            formu.cadenadepresentacion = presentacion;

            string dameelcentro = formu.getValorCadena("CENTRO", presentacion);

            decimal largocentroenmm = 1000M;

            switch(dameelcentro.ToUpper().TrimEnd().TrimStart()) // quito espacion al frente y atras
            {
                case "3 PULGADAS":
                    {
                        centros.IDArticulo = 160.ToString();
                        centros.IDCaracteristica = 211.ToString();
                        break;
                    }
                case "40 MM":
                    {
                        centros.IDArticulo = 2918.ToString();
                        centros.IDCaracteristica = 3976.ToString();
                        break;
                    }
                case "1 PULGADA":
                    {
                        centros.IDArticulo = 160.ToString();
                        centros.IDCaracteristica = 211.ToString();
                        largocentroenmm = 1020;
                        break;
                    }
                default:
                    {
                        centros.IDArticulo = 160.ToString();
                        centros.IDCaracteristica = 211.ToString();
                        break;
                    }
            }

            



            centros.IDTipoArticulo = "4";
            centros.IDProceso = "4";
            centros.Formula = "(((C*1000)/" +  elemento.Cantidadxrollo+")/"+ Math.Truncate(   decimal.Parse( (largocentroenmm/ (elemento.anchoproductomm * elemento.productosalpaso)).ToString()))+")" ; /// la cantidad de centros entre la cantidad de centros que caben en una pieza de rollo completa
            centros.FactorCierre = "1";
            centros.Indicaciones = "";


            plantilla.Articulos.Add(centros);



            /******* proceso de empaque ******************/

            //ArticuloXML nuevotiempoempaque = new ArticuloXML();



            //nuevotiempoempaque.IDArticulo = 228.ToString();
            //nuevotiempoempaque.IDCaracteristica = 217.ToString();
            //nuevotiempoempaque.IDTipoArticulo = "5";
            //nuevotiempoempaque.IDProceso = "7";
            //nuevotiempoempaque.Formula = "0.25";
            //nuevotiempoempaque.Indicaciones = "";


            //plantilla.Articulos.Add(nuevotiempoempaque);

            //ArticuloXML cajas = new ArticuloXML();



            //cajas.IDArticulo = 3543.ToString();
            //cajas.IDCaracteristica = 4478.ToString();
            //cajas.IDTipoArticulo = "4";
            //cajas.IDProceso = "7";
            //cajas.Formula = "C/50";
            //cajas.FactorCierre = "1";
            //cajas.Indicaciones = "";

            //plantilla.Articulos.Add(cajas);




            return plantilla;
        }

        public Plantilla Modelo5(int IDCotizacion, int IDCaracteristicaGrabandose)
        {

            ClsCotizador elemento = new ClsCotizador();
            Cotizaciones archivocotizacion = new ArchivoCotizadorContext().cotizaciones.Find(IDCotizacion);
            XmlDocument documento = new XmlDocument();
            string nombredearchivo = archivocotizacion.Ruta + "\\" + archivocotizacion.NombreArchivo + ".xml";
            documento.Load(nombredearchivo);
            elemento = null;
            XmlSerializer serializer = new XmlSerializer(typeof(ClsCotizador));
            using (Stream reader = new FileStream(nombredearchivo, FileMode.Open))
            {
                // Call the Deserialize method to restore the object's state.
                elemento = (ClsCotizador)serializer.Deserialize(reader);
                reader.Close();
            }

            Plantilla plantilla = new Plantilla();


            /********************** igualacion y montaje ******************************/



            ArticuloXML nuevomanodeobraigualacion = new ArticuloXML();

            nuevomanodeobraigualacion.IDArticulo = "227";
            nuevomanodeobraigualacion.IDCaracteristica = "216";
            nuevomanodeobraigualacion.Formula = "(0.25+(" + (elemento.Numerodetintas) + "*0.5)";


            nuevomanodeobraigualacion.IDTipoArticulo = "5";
            nuevomanodeobraigualacion.IDProceso = "3";




            plantilla.Articulos.Add(nuevomanodeobraigualacion);


            /////// proceso prensa suaje materiales y tintas ********************************
            ///////// manode obra y maquina de prensa
            /********************  mano de obra *******************/





            ArticuloXML nuevomanodeobra = new ArticuloXML();

            nuevomanodeobra.IDArticulo = "2916";
            nuevomanodeobra.IDCaracteristica = "3972";
            nuevomanodeobra.Formula = "(C*" + Math.Round( (elemento.Cantidad / elemento.MaterialNecesitado),4) + ")/" + Math.Round( (elemento.HrPrensa == 0 ? elemento.MaterialNecesitado : elemento.HrPrensa / elemento.MaterialNecesitado),6);
            if (elemento.Numerodetintas > 0)
            {
                nuevomanodeobra.Formula = "((C*" + Math.Round((elemento.Cantidad / elemento.MaterialNecesitado), 4) + ")+" + elemento.Numerodetintas + " * 60)/" + Math.Round((elemento.HrPrensa == 0 ? elemento.MaterialNecesitado : elemento.HrPrensa / elemento.MaterialNecesitado), 6);
            }

            nuevomanodeobra.IDTipoArticulo = "5";
            nuevomanodeobra.IDProceso = "5";
            nuevomanodeobra.FactorCierre = "0.25";



            plantilla.Articulos.Add(nuevomanodeobra);


            /*********************************   maquina *********************************/


            if (elemento.Numerodetintas < 4)
            {

                ArticuloXML nuevomaquina = new ArticuloXML();

                nuevomaquina.IDArticulo = "87";
                nuevomaquina.IDCaracteristica = "32";


                nuevomaquina.Formula = "(C*" + Math.Round((elemento.Cantidad / elemento.MaterialNecesitado), 4) + ")/" + Math.Round((elemento.HrPrensa == 0 ? elemento.MaterialNecesitado : elemento.HrPrensa / elemento.MaterialNecesitado), 6);
                if (elemento.Numerodetintas > 0)
                {
                    nuevomaquina.Formula = "(C*" + Math.Round((elemento.Cantidad / elemento.MaterialNecesitado), 4) + ")+" + elemento.Numerodetintas + " * 60)/" + Math.Round((elemento.HrPrensa == 0 ? elemento.MaterialNecesitado : elemento.HrPrensa / elemento.MaterialNecesitado), 6);
                }

                nuevomaquina.IDTipoArticulo = "3";
                nuevomaquina.IDProceso = "5";
                nuevomaquina.FactorCierre = "0.25";

                plantilla.Articulos.Add(nuevomaquina);
            }

            if (elemento.Numerodetintas >=4 )
            {

                ArticuloXML nuevomaquina = new ArticuloXML();

                nuevomaquina.IDArticulo = "88";
                nuevomaquina.IDCaracteristica = "213";


                nuevomaquina.Formula = "(C*" + Math.Round((elemento.Cantidad / elemento.MaterialNecesitado),4) + ")/" + (elemento.HrPrensa == 0 ? elemento.MaterialNecesitado : elemento.HrPrensa / elemento.MaterialNecesitado);
                if (elemento.Numerodetintas > 0)
                {
                    nuevomaquina.Formula = "(C*" + Math.Round((elemento.Cantidad / elemento.MaterialNecesitado), 4) + ")+" + elemento.Numerodetintas + " * 60)/" + Math.Round( (elemento.HrPrensa == 0 ? elemento.MaterialNecesitado : elemento.HrPrensa / elemento.MaterialNecesitado),6);
                }

                nuevomaquina.IDTipoArticulo = "3";
                nuevomaquina.IDProceso = "5";
                nuevomaquina.FactorCierre = "0.25";

                plantilla.Articulos.Add(nuevomaquina);
            }

            /***************************************  suaje ****************************/

            try
            {
                ArticuloXML nuevosuaje = new ArticuloXML();

                Caracteristica CIDSuaje = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from Caracteristica where ID=" + elemento.IDSuaje).ToList().FirstOrDefault();

                int IDSuaje = CIDSuaje.Articulo_IDArticulo;


                nuevosuaje.IDArticulo = IDSuaje.ToString();
                nuevosuaje.IDCaracteristica = elemento.IDSuaje.ToString();
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

            int IDMaterialArticulo  = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from Caracteristica where ID=" + IDMaterial).ToList().FirstOrDefault().Articulo_IDArticulo; 

            nuevoplaneacion2.IDArticulo = IDMaterialArticulo.ToString();
            nuevoplaneacion2.IDCaracteristica = IDMaterial.ToString();
            nuevoplaneacion2.IDTipoArticulo = "6";
            nuevoplaneacion2.IDProceso = "5";

            decimal cantidadmillar = Math.Round((elemento.CantidadMPMts2  /elemento.Cantidad ), 4);



            nuevoplaneacion2.Formula = "(C*" + cantidadmillar + ")";






            nuevoplaneacion2.Indicaciones = "";


            plantilla.Articulos.Add(nuevoplaneacion2);

            if (elemento.IDMaterial2 > 0)
            {

                ArticuloXML nuevoplaneacion3 = new ArticuloXML();


                Materiales material2 = new MaterialesContext().Materiales.Find(elemento.IDMaterial2);
                int IDMaterial2 = verificamaterial(material2.Clave, elemento.anchomaterialenmm, material.Largo, material.Fam, material.Descripcion, material.Precio);

                int IDMaterialArticulo2 = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from Caracteristica where ID=" + IDMaterial2).ToList().FirstOrDefault().Articulo_IDArticulo; 

                nuevoplaneacion3.IDArticulo = IDMaterialArticulo2.ToString();
                nuevoplaneacion3.IDCaracteristica = IDMaterial2.ToString();
                nuevoplaneacion3.IDTipoArticulo = "6";
                nuevoplaneacion3.IDProceso = "5";

                decimal cantidadmillar2 = Math.Round( (elemento.CantidadMPMts2 /elemento.Cantidad ),4);



                nuevoplaneacion3.Formula = "(C*" + cantidadmillar2 + ")";


                nuevoplaneacion3.Indicaciones = "";


                plantilla.Articulos.Add(nuevoplaneacion3);
            }

            if (elemento.IDMaterial3 > 0)
            {

                ArticuloXML nuevoplaneacion3 = new ArticuloXML();


                Materiales material3 = new MaterialesContext().Materiales.Find(elemento.IDMaterial3);
                int IDMaterial3 = verificamaterial(material3.Clave, elemento.anchomaterialenmm, material.Largo, material.Fam, material.Descripcion, material.Precio);

                int IDMaterialArticulo3 = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from Caracteristica where ID=" + IDMaterial3).ToList().FirstOrDefault().Articulo_IDArticulo;

                nuevoplaneacion3.IDArticulo = IDMaterialArticulo3.ToString();
                nuevoplaneacion3.IDCaracteristica = IDMaterial3.ToString();
                nuevoplaneacion3.IDTipoArticulo = "6";
                nuevoplaneacion3.IDProceso = "5";

                decimal cantidadmillar2 = Math.Round((elemento.CantidadMPMts2 / elemento.Cantidad), 4);



                nuevoplaneacion3.Formula = "(C*" + cantidadmillar2 + ")";


                nuevoplaneacion3.Indicaciones = "";


                plantilla.Articulos.Add(nuevoplaneacion3);
            }





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



                    nuevoplaneaciontinta.Formula = "(C*" + Math.Round(cantidadmillar,6) + "*" + tinta.Area/100 + ")/300";

                    nuevoplaneaciontinta.FactorCierre = "0.125";

                    nuevoplaneaciontinta.Indicaciones = "";


                    plantilla.Articulos.Add(nuevoplaneaciontinta);
                }
                catch(Exception err)
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
            nuevotiempoembobinado.Formula = "C*" + Math.Round((elemento.HrEmbobinado / elemento.Cantidad), 4) + "";
            nuevotiempoembobinado.FactorCierre = "0.15";
            nuevotiempoembobinado.Indicaciones = "";


            plantilla.Articulos.Add(nuevotiempoembobinado);


            ArticuloXML centros = new ArticuloXML();

            FormulaSiaapi.Formulas formu = new FormulaSiaapi.Formulas();

            Caracteristica caragrabandose = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from Caracteristica where ID=" + IDCaracteristicaGrabandose).ToList().FirstOrDefault();

            string presentacion = caragrabandose.Presentacion;

            formu.cadenadepresentacion = presentacion;

            string dameelcentro = formu.getValorCadena("CENTRO", presentacion);

            decimal largocentroenmm = 1000M;

            switch (dameelcentro.ToUpper().TrimEnd().TrimStart()) // quito espacion al frente y atras
            {
                case "3 PULGADAS":
                    {
                        centros.IDArticulo = 160.ToString();
                        centros.IDCaracteristica = 211.ToString();
                        break;
                    }
                case "40 MM":
                    {
                        centros.IDArticulo = 2918.ToString();
                        centros.IDCaracteristica = 3976.ToString();
                        break;
                    }
                case "1 PULGADA":
                    {
                        centros.IDArticulo = 160.ToString();
                        centros.IDCaracteristica = 211.ToString();
                        largocentroenmm = 1020;
                        break;
                    }
                default:
                    {
                        centros.IDArticulo = 160.ToString();
                        centros.IDCaracteristica = 211.ToString();
                        break;
                    }
            }





            centros.IDTipoArticulo = "4";
            centros.IDProceso = "4";
            centros.Formula = "(((C*1000)/" + elemento.Cantidadxrollo + ")/" + Math.Truncate(decimal.Parse((largocentroenmm / (elemento.anchoproductomm * elemento.productosalpaso)).ToString())) + ")"; /// la cantidad de centros entre la cantidad de centros que caben en una pieza de rollo completa
            centros.FactorCierre = "1";
            centros.Indicaciones = "";


            plantilla.Articulos.Add(centros);

            ArticuloXML nuevomaquinaenbobinado = new ArticuloXML();

            nuevomaquinaenbobinado.IDArticulo = "91";
            nuevomaquinaenbobinado.IDCaracteristica = "3970";


            nuevomaquinaenbobinado.Formula = "C*" + Math.Round((elemento.HrEmbobinado / elemento.Cantidad), 4) + "";


            nuevomaquinaenbobinado.IDTipoArticulo = "3";
            nuevomaquinaenbobinado.IDProceso = "4";
            nuevomaquinaenbobinado.FactorCierre = "0.25";

            plantilla.Articulos.Add(nuevomaquinaenbobinado);



            /******* proceso de empaque ******************/

            //ArticuloXML nuevotiempoempaque = new ArticuloXML();



            //nuevotiempoempaque.IDArticulo = 228.ToString();
            //nuevotiempoempaque.IDCaracteristica = 217.ToString();
            //nuevotiempoempaque.IDTipoArticulo = "5";
            //nuevotiempoempaque.IDProceso = "7";
            //nuevotiempoempaque.Formula = "0.25";
            //nuevotiempoempaque.Indicaciones = "";


            //plantilla.Articulos.Add(nuevotiempoempaque);

            //ArticuloXML cajas = new ArticuloXML();



            //cajas.IDArticulo = 3543.ToString();
            //cajas.IDCaracteristica = 4478.ToString();
            //cajas.IDTipoArticulo = "4";
            //cajas.IDProceso = "7";
            //cajas.Formula = "C/50";
            //cajas.FactorCierre = "1";
            //cajas.Indicaciones = "";


            //plantilla.Articulos.Add(cajas);




            return plantilla;
        }


        /// <summary>
        /// MODELO 8 TERMOENCOGIBLE MAQUINAS TERMONENCOGIBLES
        /// </summary>
        /// <param name="IDCotizacion"></param>
        /// <returns></returns>
        public Plantilla Modelo8(int IDCotizacion)
        {

            ClsCotizador elemento = new ClsCotizador();
            Cotizaciones archivocotizacion = new ArchivoCotizadorContext().cotizaciones.Find(IDCotizacion);
            XmlDocument documento = new XmlDocument();
            string nombredearchivo = archivocotizacion.Ruta + "\\" + archivocotizacion.NombreArchivo + ".xml";
            documento.Load(nombredearchivo);
            elemento = null;
            XmlSerializer serializer = new XmlSerializer(typeof(ClsCotizador));
            using (Stream reader = new FileStream(nombredearchivo, FileMode.Open))
            {
                // Call the Deserialize method to restore the object's state.
                elemento = (ClsCotizador)serializer.Deserialize(reader);
                reader.Close();
            }

            Plantilla plantilla = new Plantilla();




            /********************** igualacion y montaje ******************************/




            ArticuloXML nuevomanodeobraigualacion = new ArticuloXML();

            nuevomanodeobraigualacion.IDArticulo = "227";
            nuevomanodeobraigualacion.IDCaracteristica = "216";
            nuevomanodeobraigualacion.Formula = "(0.25+(" + (elemento.Numerodetintas) + "*0.5)";


            nuevomanodeobraigualacion.IDTipoArticulo = "5";
            nuevomanodeobraigualacion.IDProceso = "3";

            plantilla.Articulos.Add(nuevomanodeobraigualacion);





            /////// proceso prensa suaje materiales y tintas ********************************
            ///////// manode obra y maquina de prensa
            /********************  mano de obra *******************/





            ArticuloXML nuevomanodeobra = new ArticuloXML();

            nuevomanodeobra.IDArticulo = "2916";
            nuevomanodeobra.IDCaracteristica = "3972";
            nuevomanodeobra.Formula = "(C*" + (elemento.Cantidad / elemento.MaterialNecesitado) + ")/" + (elemento.HrPrensa == 0 ? elemento.MaterialNecesitado : elemento.HrPrensa / elemento.MaterialNecesitado);
            if (elemento.Numerodetintas > 0)
            {
                nuevomanodeobra.Formula = "((C*" + (elemento.Cantidad / elemento.MaterialNecesitado) + ")+" + elemento.Numerodetintas + " * 60)/" + (elemento.HrPrensa == 0 ? elemento.MaterialNecesitado : elemento.HrPrensa / elemento.MaterialNecesitado);
            }

            nuevomanodeobra.IDTipoArticulo = "5";
            nuevomanodeobra.IDProceso = "5";
            nuevomanodeobra.FactorCierre = "0.25";



            plantilla.Articulos.Add(nuevomanodeobra);


            /*********************************   maquina *********************************/


          

                ArticuloXML nuevomaquina = new ArticuloXML();

                nuevomaquina.IDArticulo = "3558";  // nilpeter uv
                nuevomaquina.IDCaracteristica = "4504";


                nuevomaquina.Formula = "(C*" + (elemento.Cantidad / elemento.MaterialNecesitado) + ")/" + (elemento.HrPrensa == 0 ? elemento.MaterialNecesitado : elemento.HrPrensa / elemento.MaterialNecesitado);
                if (elemento.Numerodetintas > 0)
                {
                    nuevomaquina.Formula = "((C*" + (elemento.Cantidad / elemento.MaterialNecesitado) + ")+" + elemento.Numerodetintas + " * 60)/" + (elemento.HrPrensa == 0 ? elemento.MaterialNecesitado : elemento.HrPrensa / elemento.MaterialNecesitado);
                }

                nuevomaquina.IDTipoArticulo = "3";
                nuevomaquina.IDProceso = "5";
                nuevomaquina.FactorCierre = "0.25";

                plantilla.Articulos.Add(nuevomaquina);


            /***************************************  suaje ****************************/
            try
            {
                ArticuloXML nuevosuaje = new ArticuloXML();

                int IDSuaje = new ArticuloContext().Caracteristica.Find(elemento.IDSuaje).Articulo_IDArticulo;

                nuevosuaje.IDArticulo = IDSuaje.ToString();
                nuevosuaje.IDCaracteristica = elemento.IDSuaje.ToString();
                nuevosuaje.IDTipoArticulo = "2";
                nuevosuaje.IDProceso = "5";
                nuevosuaje.Formula = "1/"+elemento.DiluirSuajeEnPedidos;
                nuevosuaje.Indicaciones = "";


                plantilla.Articulos.Add(nuevosuaje);
            }
            catch(Exception err)
            {
                string mensajedeerro = err.Message;

            }

            Materiales material = new MaterialesContext().Materiales.Find(elemento.IDMaterial);


            int IDMaterial = verificamaterial(material.Clave, elemento.anchomaterialenmm, material.Largo, material.Fam, material.Descripcion, material.Precio);

            ArticuloXML nuevoplaneacion2 = new ArticuloXML();

            int IDMaterialArticulo = new ArticuloContext().Caracteristica.Find(elemento.IDMaterial).Articulo_IDArticulo;

            nuevoplaneacion2.IDArticulo = IDMaterialArticulo.ToString();
            nuevoplaneacion2.IDCaracteristica = IDMaterial.ToString();
            nuevoplaneacion2.IDTipoArticulo = "6";
            nuevoplaneacion2.IDProceso = "5";

            decimal cantidadmillar = (elemento.CantidadMPMts2 /elemento.Cantidad );



            nuevoplaneacion2.Formula = "(C*" + cantidadmillar + ")";






            nuevoplaneacion2.Indicaciones = "";


            plantilla.Articulos.Add(nuevoplaneacion2);

            if (elemento.IDMaterial2 > 0)
            {

                ArticuloXML nuevoplaneacion3 = new ArticuloXML();

                int IDMaterialArticulo2 = new ArticuloContext().Caracteristica.Find(elemento.IDMaterial2).Articulo_IDArticulo;

                Materiales material2 = new MaterialesContext().Materiales.Find(elemento.IDMaterial2);
                int IDMaterial2 = verificamaterial(material2.Clave, elemento.anchomaterialenmm, material2.Largo, material2.Fam, material2.Descripcion, material2.Precio);

                nuevoplaneacion3.IDArticulo = IDMaterialArticulo2.ToString();
                nuevoplaneacion3.IDCaracteristica = IDMaterial2.ToString();
                nuevoplaneacion3.IDTipoArticulo = "6";
                nuevoplaneacion3.IDProceso = "5";

                decimal cantidadmillar2 = (elemento.CantidadMPMts2 /elemento.Cantidad  );



                nuevoplaneacion3.Formula = "(C*" + cantidadmillar + ")";


                nuevoplaneacion3.Indicaciones = "";


                plantilla.Articulos.Add(nuevoplaneacion3);
            }


            if (elemento.IDMaterial3 > 0)
            {

                ArticuloXML nuevoplaneacion3 = new ArticuloXML();

                int IDMaterialArticulo3 = new ArticuloContext().Caracteristica.Find(elemento.IDMaterial3).Articulo_IDArticulo;



                Materiales material3 = new MaterialesContext().Materiales.Find(elemento.IDMaterial3);
                int IDMaterial3 = verificamaterial(material3.Clave, elemento.anchomaterialenmm, material3.Largo, material3.Fam, material3.Descripcion, material3.Precio);


                nuevoplaneacion3.IDArticulo = IDMaterialArticulo3.ToString();
                nuevoplaneacion3.IDCaracteristica = IDMaterial3.ToString();
                nuevoplaneacion3.IDTipoArticulo = "6";
                nuevoplaneacion3.IDProceso = "5";

                decimal cantidadmillar2 = (elemento.CantidadMPMts2 / elemento.Cantidad);



                nuevoplaneacion3.Formula = "(C*" + cantidadmillar + ")";


                nuevoplaneacion3.Indicaciones = "";


                plantilla.Articulos.Add(nuevoplaneacion3);
            }



            foreach (Tinta tinta in elemento.Tintas)
            {
                ArticuloXML nuevoplaneaciontinta = new ArticuloXML();

                int IDTinta = tinta.IDTinta;

                int IDMpresentaciontin = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from Caracteristica where ID=" + IDTinta).ToList().FirstOrDefault().Articulo_IDArticulo;

                nuevoplaneaciontinta.IDArticulo = IDTinta.ToString();
                nuevoplaneaciontinta.IDCaracteristica = IDMpresentaciontin.ToString();
                nuevoplaneaciontinta.IDTipoArticulo = "6";
                nuevoplaneaciontinta.IDProceso = "5";

                decimal cantidadmillartinta = (elemento.Cantidad / elemento.CantidadMPMts2);



                nuevoplaneaciontinta.Formula = "(C*" + cantidadmillar + "*" + tinta.Area + ")/300";

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
            nuevotiemposellado.Formula = "C*" + Math.Round( (elemento.MaterialNecesitado / 7200)/elemento.Cantidad,6); // 7200 MTS X HRS
            nuevotiemposellado.FactorCierre = "0.15";
            nuevotiemposellado.Indicaciones = "";


            plantilla.Articulos.Add(nuevotiemposellado);



            ArticuloXML nuevamaquinaosellado = new ArticuloXML();



            nuevamaquinaosellado.IDArticulo = 2317.ToString();
            nuevamaquinaosellado.IDCaracteristica = 12325.ToString();
            nuevamaquinaosellado.IDTipoArticulo = "3";
            nuevamaquinaosellado.IDProceso = "11";
            nuevamaquinaosellado.Formula = "C*" + Math.Round((elemento.MaterialNecesitado / 7200) / elemento.Cantidad, 6); // 7200 MTS X HRS
            nuevamaquinaosellado.FactorCierre = "0.15";
            nuevamaquinaosellado.Indicaciones = "";


            plantilla.Articulos.Add(nuevamaquinaosellado);


            /**** proceso inspeccion *******************/

            ArticuloXML nuevotiempoinspeccion = new ArticuloXML();



            nuevotiempoinspeccion.IDArticulo = 232.ToString();
            nuevotiempoinspeccion.IDCaracteristica = 219.ToString();
            nuevotiempoinspeccion.IDTipoArticulo = "5";
            nuevotiempoinspeccion.IDProceso = "12";
            nuevotiempoinspeccion.Formula = "(((C  * " +elemento.largoproductomm+") / 50) / 60 )+0.33";
            nuevotiempoinspeccion.FactorCierre = "0.25";
            nuevotiempoinspeccion.Indicaciones = "";


            plantilla.Articulos.Add(nuevotiempoinspeccion);

            ArticuloXML nuevamaquinainspeccion = new ArticuloXML();



            nuevamaquinainspeccion.IDArticulo = 2684.ToString();
            nuevamaquinainspeccion.IDCaracteristica = 3733.ToString();
            nuevamaquinainspeccion.IDTipoArticulo = "3";
            nuevamaquinainspeccion.IDProceso = "12";
            nuevamaquinainspeccion.Formula = "(((C  * " + elemento.largoproductomm + ") / 50) / 60 )+0.33"; // 7200 MTS X HRS
            nuevamaquinainspeccion.FactorCierre = "0.15";
            nuevamaquinainspeccion.Indicaciones = "";


            plantilla.Articulos.Add(nuevamaquinainspeccion);

            /**** proceso inspeccion *******************/

            ArticuloXML tiempocorte = new ArticuloXML();



            tiempocorte.IDArticulo = 232.ToString();
            tiempocorte.IDCaracteristica = 219.ToString();
            tiempocorte.IDTipoArticulo = "5";
            tiempocorte.IDProceso = "16";
            tiempocorte.Formula = "  ((((C * 1000) * LARGO) / 13000) / 60) + (((C * 1000) / ETIQUETAXR) * 3) / 60"; // 13 MTS * MIN
            tiempocorte.FactorCierre = "0.25";
            tiempocorte.Indicaciones = "";


            plantilla.Articulos.Add(tiempocorte);

            ArticuloXML nuevamaquinacorte = new ArticuloXML();



            nuevamaquinacorte.IDArticulo = 2686.ToString();
            nuevamaquinacorte.IDCaracteristica = 4503.ToString();
            nuevamaquinacorte.IDTipoArticulo = "3";
            nuevamaquinacorte.IDProceso = "16";
            nuevamaquinacorte.Formula = "  ((((C * 1000) * LARGO) / 13000) / 60) + (((C * 1000) / ETIQUETAXR) * 3) / 60"; // 13 MTS * MIN
            nuevamaquinacorte.FactorCierre = "0.15";
            nuevamaquinacorte.Indicaciones = "";


            plantilla.Articulos.Add(nuevamaquinacorte);
            /******* proceso de empaque ******************/

            //ArticuloXML nuevotiempoempaque = new ArticuloXML();



            //nuevotiempoempaque.IDArticulo = 228.ToString();
            //nuevotiempoempaque.IDCaracteristica = 217.ToString();
            //nuevotiempoempaque.IDTipoArticulo = "5";
            //nuevotiempoempaque.IDProceso = "7";
            //nuevotiempoempaque.Formula = "0.5";
            //nuevotiempoempaque.Indicaciones = "";


            //plantilla.Articulos.Add(nuevotiempoempaque);

            //ArticuloXML cajas = new ArticuloXML();
            //cajas.IDArticulo = 3543.ToString();
            //cajas.IDCaracteristica = 4478.ToString();
            //cajas.IDTipoArticulo = "4";
            //cajas.IDProceso = "7";
            //cajas.Formula = "C/20";
            //cajas.FactorCierre = "1";
            //cajas.Indicaciones = "";

            //plantilla.Articulos.Add(cajas);

            return plantilla;


        }



        public int creaHE(int IDCotizacion, int IDCliente,  int IDPresentacion, Plantilla plantilla, int _Modelodeproduccion) // crea la planeacion
        {


            
            ClsCotizador elemento = new ClsCotizador();
            Cotizaciones archivocotizacion = new ArchivoCotizadorContext().cotizaciones.Find(IDCotizacion);
            XmlDocument documento = new XmlDocument();
            string nombredearchivo = archivocotizacion.Ruta + "\\" + archivocotizacion.NombreArchivo + ".xml";
            documento.Load(nombredearchivo);
            elemento = null;
            XmlSerializer serializer = new XmlSerializer(typeof(ClsCotizador));
            using (Stream reader = new FileStream(nombredearchivo, FileMode.Open))
            {
                // Call the Deserialize method to restore the object's state.
                elemento = (ClsCotizador)serializer.Deserialize(reader);
                reader.Close();
            }




            SIAAPI.Models.Comercial.Articulo art = new SIAAPI.Models.Comercial.Articulo();
         
            DateTime fecha = DateTime.Now;
            string fechaf = fecha.ToString("yyyyMMdd");

            int numplaneacion=1;

            try
            {
                int planeacion = db.Database.SqlQuery<ClsDatoEntero>("select max(Planeacion) as Dato from HEspecificacionE").ToList().FirstOrDefault().Dato;
                numplaneacion = planeacion + 1;
            }
            catch(Exception err)
            {
                string mensajederror = err.Message;
               // numplaneacion = 1;
            }

            VCaracteristicaContext caracteristica = new VCaracteristicaContext();
            VCaracteristica carac = caracteristica.VCaracteristica.Find(IDPresentacion);

            ArticuloContext articulo = new ArticuloContext();
            art = articulo.Articulo.Find(carac.Articulo_IDArticulo);


           try
            {
                string cadensql = "insert into HEspecificacionE([FechaEspecificacion],[IDCliente],[IDClienteP],[IDFamilia],[IDArticulo],[IDCaracteristica],[Descripcion],[Presentacion],[IDModeloProduccion],[Version],[Planeacion]) values ('" + fechaf + "'," + IDCliente + ",0," + art.IDFamilia + "," + art.IDArticulo + "," + carac.ID + ",'" + art.Descripcion + "','" + carac.Presentacion + "'," + _Modelodeproduccion + ",1," + numplaneacion + ")";
                db.Database.ExecuteSqlCommand(cadensql);
            }
            catch (SqlException err)
            {
                string mesajederror = err.Message;
            }
                
            int idEspecD = db.Database.SqlQuery<ClsDatoEntero>("select IDHE as Dato from [dbo].[HEspecificacionE] where planeacion=" + numplaneacion + " and version=1").ToList<ClsDatoEntero>().FirstOrDefault().Dato;


            foreach (ArticuloXML arti in plantilla.Articulos)
            {
                try
                {
                    if (  arti.FactorCierre== null )
                        {
                        arti.FactorCierre = "0";
                    }
                    string cadenasql = "insert into [dbo].[ArticulosPlaneacionE] ([IDHE],[Version],[IDArticulo],[IDTipoArticulo],[IDCaracteristica],[IDProceso],[Formuladerelacion],[factorcierre],[Indicaciones],[Planeacion] ) values ('" + idEspecD + "', 1, '" + arti.IDArticulo + "', '" + arti.IDTipoArticulo + "', '" + arti.IDCaracteristica + "', " + arti.IDProceso + ", '" + arti.Formula + "', " + arti.FactorCierre + ", '" + arti.Indicaciones + "', '" + numplaneacion + "')";
                    db.Database.ExecuteSqlCommand(cadenasql);
                }
                catch(Exception err)
                {
                    string mensajeerror = err.Message;
                }

                
            }

            return idEspecD;
        }


        public ActionResult CrearCotizacionPDF(int IDCotizacion)
        {

            ClsCotizador elemento = new ClsCotizador();
            Cotizaciones archivocotizacion = new ArchivoCotizadorContext().cotizaciones.Find(IDCotizacion);
            XmlDocument documento = new XmlDocument();
            string nombredearchivo = archivocotizacion.Ruta + "\\" + archivocotizacion.NombreArchivo + ".xml";
            documento.Load(nombredearchivo);
            elemento = null;
            XmlSerializer serializer = new XmlSerializer(typeof(ClsCotizador));
            using (Stream reader = new FileStream(nombredearchivo, FileMode.Open))
            {
                // Call the Deserialize method to restore the object's state.
                elemento = (ClsCotizador)serializer.Deserialize(reader);
                reader.Close();
            }


            ClientePDF cliente = new ClientePDF();
            cliente.IDCotizacion = IDCotizacion;

            ViewBag.MatrizPrecio = elemento.MatrizPrecio;

            ViewBag.cantidad1 = elemento.Rango1;
            ViewBag.cantidad2 = elemento.Rango2;
            ViewBag.cantidad3 = elemento.Rango3;
            ViewBag.cantidad4 = elemento.Rango4;

            ViewBag.precio1 = elemento.precioconvenidos.precio1.ToString("C");
            ViewBag.precio2 = elemento.precioconvenidos.precio2.ToString("C");
            ViewBag.precio3 = elemento.precioconvenidos.precio3.ToString("C");
            ViewBag.precio4 = elemento.precioconvenidos.precio4.ToString("C");

            //  ViewBag.IDVendedor = new SelectList(new VendedorContext().Vendedores.OrderBy(m =>m.Nombre),"IDVendedor","Nombre");
            ViewBag.IDVendedor = new VendedorRepository().GetVendedor();
            ViewBag.Mensajedeerror = "";
            ViewBag.Precios = elemento.MatrizPrecio;


            return View(cliente);


        }

        [HttpPost]
        public ActionResult CrearCotizacionPDF(ClientePDF cliente, FormCollection coleccion )
        {
            bool rango1 = true;

            bool rango2 = true;
            bool rango3 = true;
            bool rango4 = true;

            if(coleccion.Get("precio1") == null)
            {
                rango1 = false;
            }

            if (coleccion.Get("precio2") == null)
            {
                rango2 = false;
            }

            if (coleccion.Get("precio3")== null)
            {
                rango3 = false;
            }
            if (coleccion.Get("precio4") == null)
            {
                rango4 = false;
            }


            ClsCotizador elemento = new ClsCotizador();

            try
            {
                if (cliente.IDVendedor==0)
                {
                    throw new Exception("Selecciona el Vendedor");
                }

            
                Cotizaciones archivocotizacion = new ArchivoCotizadorContext().cotizaciones.Find(cliente.IDCotizacion);
                XmlDocument documento = new XmlDocument();
                string nombredearchivo = archivocotizacion.Ruta + "\\" + archivocotizacion.NombreArchivo + ".xml";
                documento.Load(nombredearchivo);
                elemento = null;
                XmlSerializer serializer = new XmlSerializer(typeof(ClsCotizador));
                using (Stream reader = new FileStream(nombredearchivo, FileMode.Open))
                {
                    // Call the Deserialize method to restore the object's state.
                    elemento = (ClsCotizador)serializer.Deserialize(reader);
                    reader.Close();
                }

                XmlDocument configcotiza = new XmlDocument();
                string nombredearchivo2 = System.Web.HttpContext.Current.Server.MapPath("~/RedaccionCotizacionRapida.xml");
                documento.Load(nombredearchivo2);
                XMLRedaccionCR configuracion = null;
                XmlSerializer serializer2 = new XmlSerializer(typeof(XMLRedaccionCR));
                using (Stream reader2 = new FileStream(nombredearchivo2, FileMode.Open))
                {
                    // Call the Deserialize method to restore the object's state.
                    configuracion = (XMLRedaccionCR)serializer2.Deserialize(reader2);
                    reader2.Close();
                }

                DocumentoCotizacionRapida cotizapdr = new DocumentoCotizacionRapida(elemento, cliente, configuracion);

                string nombredecarpeta = System.Web.HttpContext.Current.Server.MapPath("~/Cotizaciones/"+ archivocotizacion.Usuario +"/"+ archivocotizacion.Fecha.Month + "" + archivocotizacion.Fecha.Year  );
                if (!Directory.Exists(nombredecarpeta))
                {
                    Directory.CreateDirectory(nombredecarpeta);
                }

                nombredecarpeta += "/" + elemento.IDCotizacion + ".pdf";

                cotizapdr.crear(nombredecarpeta, rango1, rango2,rango3,rango4);


                string contentType = System.Net.Mime.MediaTypeNames.Application.Pdf;
                return new FilePathResult(nombredecarpeta, contentType);


            }
            catch(Exception err)
            {
                ViewBag.mensajedeerror = err.Message;
                var listavendedores = new VendedorRepository().GetVendedor();

                ViewBag.IDVendedor = listavendedores;
                ViewBag.MatrizPrecio = elemento.MatrizPrecio;
                return View(cliente);

            }

           





        }

        public ActionResult TipoArticulo(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize, string Familia)
        {
            //Buscar Serie Factura

            var SerLst = new List<string>();

            ViewBag.Familias = new FamiliaRepository().GetFamilias();
            ViewBag.Familia = Familia;

            if (sortOrder == string.Empty)
            {
                sortOrder = "Cref";
            }

            VPArticuloContext db = new VPArticuloContext();
            ViewBag.CurrentSort = sortOrder;
            ViewBag.ASortParm = String.IsNullOrEmpty(sortOrder) ? "Descripcion" : "Descripcion";
            ViewBag.BSortParm = String.IsNullOrEmpty(sortOrder) ? "Cref" : "Cref";
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


            string cadena = "select TOP 50 * from VPArticulo where Tipo='Tintas' ";
            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                if (string.IsNullOrEmpty(Familia))
                {
                    cadena = "select * from VPArticulo where Tipo='Tintas'  and (cref like '%" + searchString + "%' or Descripcion like '%" + searchString + "%') ";
                }
                else
                {
                    cadena = "select * from VPArticulo where  Tipo='Tintas'  and (cref like '%" + searchString + "%' or Descripcion like '%" + searchString + "%') and IDFamilia =" + Familia + " ";
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(Familia) && (Familia != "-"))
                {
                    cadena = "select * from VPArticulo where Tipo='Tintas' and IDFamilia=" + Familia + " ";
                }
                else
                {
                    cadena = "select Top 50 * from VPArticulo where Tipo='Tintas' ";
                }
            }



            //Ordenacion

            switch (sortOrder)
            {
                case "Descripcion":
                    cadena = cadena + " order by Descripcion";
                    break;
                case "Cref":
                    cadena = cadena + " order by cref";
                    break;
                case "Familia":
                    cadena = cadena + " order by Familia";
                    break;
                default:
                    cadena = cadena + " order by cref"; ;
                    break;
            }


            List<VPArticulo> elementos = db.Database.SqlQuery<VPArticulo>(cadena).ToList<VPArticulo>();
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

        public JsonResult gettintasblandas(string buscar)
        {

            string cadena = "select * from Articulo where  obsoleto='false' and Descripcion like '%" + buscar + "%' and (idtipoarticulo=7 and obsoleto='0')  and (idfamilia=14 or idfamilia=12 or idfamilia=20 or idfamilia=15 or idfamilia=16 or idfamilia=13 or idfamilia=85 or idfamilia=86)";
            List<Articulo> Articulos = new ArticuloContext().Database.SqlQuery<Articulo>(cadena).ToList();

            // Populate DropDownList
            List<SelectListItem> opciones = new List<SelectListItem>();

            foreach (Articulo art in Articulos)
            {
                SelectListItem elemento = new SelectListItem();
                elemento.Text = art.Descripcion;
                elemento.Value = art.IDArticulo.ToString();
                opciones.Add(elemento);
            }

            return Json(opciones, JsonRequestBehavior.AllowGet);
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


        public ActionResult CCotizador()
        {
            Session["IDCaracteristica"] = null;
            return View();
        }

        //// POST: Cotizaciones//5
        //[HttpPost]
        public ActionResult SuajeExistente(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize, string eje, string avance, string clave)
        {
            ViewBag.Caracteristica = null;
            ViewBag.Articulo = null;
            if (Session["IDCaracteristica"] != null)
            {
                string idc = Session["IDCaracteristica"].ToString();
                Caracteristica carac = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from caracteristica where  id=" + idc).FirstOrDefault();
                Articulo arti = new ArticuloContext().Articulo.Find(carac.Articulo_IDArticulo);
                ViewBag.Caracteristica = carac;
                ViewBag.Articulo = arti;
            }

            ViewBag.CurrentSort = sortOrder;

            ViewBag.RazoneSortParm = String.IsNullOrEmpty(sortOrder) ? "ID" : "ID";


            if (searchString == null)
            {
                searchString = currentFilter;
            }

            // Pass filtering string to view in order to maintain filtering when paging
            

            string cadenaeje = "EJE:" + eje;
            string cadenaavance = "AVANCE:" + avance;

            


            string ConsultaSql = "select * from Caracteristica as c inner join articulo as a on  a.idarticulo=c.articulo_idarticulo";
            string Filtro = "";

            if (String.IsNullOrEmpty(eje) && String.IsNullOrEmpty(avance) && !String.IsNullOrEmpty(clave)) //clave
            {

                if (Filtro == string.Empty)
                {

                    //Response.Write("<script>alert('Campos vacios')</script>");
                    //Filtro = " where presentacion like '%" + cadenaeje + "%'";

                    Filtro = " where a.cref like '%" + clave + "%' and  (a.idfamilia= 11 or a.idfamilia= 69 or a.idfamilia= 70 or a.idfamilia= 71 or a.idfamilia= 72 or a.idfamilia= 75 or a.IDFamilia=76 or a.IDFamilia=77 or a.IDFamilia=79 or a.IDFamilia=80 or a.IDFamilia=81 or a.IDFamilia=93)";
                }
                else
                {
                    Filtro = " where  (a.idfamilia= 11 or a.idfamilia= 69 or a.idfamilia= 70 or a.idfamilia= 71 or a.idfamilia= 72 or a.idfamilia= 75 or a.IDFamilia=76 or a.IDFamilia=77 or a.IDFamilia=79 or a.IDFamilia=80 or a.IDFamilia=81 or a.IDFamilia=93)";

                }



            }

            if (String.IsNullOrEmpty(eje) && !String.IsNullOrEmpty(avance) && !String.IsNullOrEmpty(clave)) //01 eje no, avance si y clave si
            {

                if (Filtro == string.Empty)
                {


                    Filtro = " where c.presentacion like '%" + cadenaavance + "%' and  a.cref like '%" +  clave + "%' and  (a.idfamilia= 11 or a.idfamilia= 69 or a.idfamilia= 71 or a.idfamilia= 72 or a.idfamilia= 75 or a.IDFamilia=76 or a.IDFamilia=77 or a.IDFamilia=79 or a.IDFamilia=80 or a.IDFamilia=81 or a.IDFamilia=93)";
                }
                else
                {
                    Filtro = " where  (a.idfamilia= 11 or a.idfamilia= 69 or a.idfamilia= 70 or a.idfamilia= 71 or a.idfamilia= 72 or a.idfamilia= 75 or a.IDFamilia=76 or a.IDFamilia=77 or a.IDFamilia=79 or a.IDFamilia=80 or a.IDFamilia=81 or a.IDFamilia=93)";

                }


            }

            if (!String.IsNullOrEmpty(eje) && String.IsNullOrEmpty(avance) && !String.IsNullOrEmpty(clave)) //01 eje si, avance no y clave si
            {

                if (Filtro == string.Empty)
                {


                    Filtro = " where c.presentacion like '%" + cadenaeje + "%' and  a.cref like '%" + clave + "%' and  (a.idfamilia= 11 or a.idfamilia= 69 or a.idfamilia= 70 or a.idfamilia= 71 or a.idfamilia= 72 or a.idfamilia= 75 or a.IDFamilia=76 or a.IDFamilia=77 or a.IDFamilia=79 or a.IDFamilia=80 or a.IDFamilia=81 or a.IDFamilia=93)";
                }
                else
                {
                    Filtro = " where  (a.idfamilia= 11 or a.idfamilia= 69 or a.idfamilia= 70 or a.idfamilia= 71 or a.idfamilia= 72 or a.idfamilia= 75 or a.IDFamilia=76 or a.IDFamilia=77 or a.IDFamilia=79 or a.IDFamilia=80 or a.IDFamilia=81 or a.IDFamilia=93)";

                }


            }



            if (!String.IsNullOrEmpty(eje) && String.IsNullOrEmpty(avance) && String.IsNullOrEmpty(clave)) //00 ninguna
            {

                if (Filtro == string.Empty)
                {

                    //Response.Write("<script>alert('Campos vacios')</script>");
                    //Filtro = " where presentacion like '%" + cadenaeje + "%'";

                    Filtro = " where c.presentacion like '%" + cadenaeje + "%' and  (a.idfamilia= 11 or a.idfamilia= 69 or a.idfamilia= 70 or a.idfamilia= 71 or a.idfamilia= 72 or a.idfamilia= 75 or a.IDFamilia=76 or a.IDFamilia=77 or a.IDFamilia=79 or a.IDFamilia=80 or a.IDFamilia=81 or a.IDFamilia=93)";
                }
                else
                {
                    Filtro = " where  (a.idfamilia= 11 or a.idfamilia= 69 or a.idfamilia= 70 or a.idfamilia= 71 or a.idfamilia= 72 or a.idfamilia= 75 or a.IDFamilia=76 or a.IDFamilia=77 or a.IDFamilia=79 or a.IDFamilia=80 or a.IDFamilia=81 or a.IDFamilia=93)";

                }



             }

            if (String.IsNullOrEmpty(eje) && !String.IsNullOrEmpty(avance) && String.IsNullOrEmpty(clave)) //01 eje no, avance si
            {

                if (Filtro == string.Empty)
                {


                    Filtro = " where c.presentacion like '%" + cadenaavance + "%' and  (a.idfamilia= 11 or a.idfamilia= 69 or a.idfamilia= 70 or a.idfamilia= 71 or a.idfamilia= 72 or a.idfamilia= 75 or a.IDFamilia=76 or a.IDFamilia=77 or a.IDFamilia=79 or a.IDFamilia=80 or a.IDFamilia=81 or a.IDFamilia=93)";
                }
                else
                {
                    Filtro = " where  (a.idfamilia= 11 or a.idfamilia= 69 or a.idfamilia= 70  or a.idfamilia= 71 or a.idfamilia= 72 or a.idfamilia= 75 or a.IDFamilia=76 or a.IDFamilia=77 or a.IDFamilia=79 or a.IDFamilia=80 or a.IDFamilia=81 or a.IDFamilia=93)";

                }


            }

            //if (!String.IsNullOrEmpty(eje) && String.IsNullOrEmpty(avance)) //01 eje si, avance no
            //{

            //    if (Filtro == string.Empty)
            //    {


            //        Filtro = " where c.presentacion like '%" + cadenaeje + "%' and (a.idfamilia= 11 or a.idfamilia= 69 or a.idfamilia= 71 or a.idfamilia= 72 or a.idfamilia= 75 or a.IDFamilia=76 or a.IDFamilia=77 or a.IDFamilia=79 or a.IDFamilia=80)";
            //    }
            //    else
            //    {
            //        Filtro = " where  (a.idfamilia= 11 or a.idfamilia= 69 or a.idfamilia= 71 or a.idfamilia= 72 or a.idfamilia= 75 or a.IDFamilia=76 or a.IDFamilia=77 or a.IDFamilia=79 or a.IDFamilia=80)";

            //    }


            //}

            if (!String.IsNullOrEmpty(eje) && !String.IsNullOrEmpty(avance) && String.IsNullOrEmpty(clave)) //11 eje si, avance si y clave no
            {

                if (Filtro == string.Empty)
                {


                    Filtro = " where c.presentacion like '%" + cadenaeje + "%' and c.presentacion like '%" + cadenaavance + "%' and  (a.idfamilia= 11 or a.idfamilia= 69 or a.idfamilia= 70 or a.idfamilia= 71 or a.idfamilia= 72 or a.idfamilia= 75 or a.IDFamilia=76 or a.IDFamilia=77 or a.IDFamilia=79 or a.IDFamilia=80 or a.IDFamilia=81 or a.IDFamilia=93)";
                }
                else
                {
                    Filtro = " where  (a.idfamilia= 11 or a.idfamilia= 69 or a.idfamilia= 70 or a.idfamilia= 71 or a.idfamilia= 72 or a.idfamilia= 75 or a.IDFamilia=76 or a.IDFamilia=77 or a.IDFamilia=79 or a.IDFamilia=80 or a.IDFamilia=81 or a.IDFamilia=93)";

                }


            }

            if (String.IsNullOrEmpty(eje) && String.IsNullOrEmpty(avance) && String.IsNullOrEmpty(clave)) //11 eje si, avance si y clave no
            {

                if (Filtro == string.Empty)
                {


                    Filtro = " where (a.idfamilia= 11  or a.idfamilia= 70 or a.idfamilia= 69 or a.idfamilia= 71 or a.idfamilia= 72 or a.idfamilia= 75 or a.IDFamilia=76 or a.IDFamilia=77 or a.IDFamilia=79 or a.IDFamilia=80 or a.IDFamilia=81 or a.IDFamilia=93)";
                }
                else
                {
                    Filtro = " where  (a.idfamilia= 11  or a.idfamilia= 70 or a.idfamilia= 69 or a.idfamilia= 71 or a.idfamilia= 72 or a.idfamilia= 75 or a.IDFamilia=76 or a.IDFamilia=77 or a.IDFamilia=79 or a.IDFamilia=80 or a.IDFamilia=81 or a.IDFamilia=93)";

                }


            }

            // Pass filtering string to view in order to maintain filtering when paging
            ViewBag.eje = eje;
            ViewBag.avance = avance;
            ViewBag.clave = clave;

            string orden = sortOrder;

            //Ordenacion

            switch (sortOrder)
            {
                case "ID":
                    orden = " order by ID ";
                    break;
                
                default:
                    orden = " order by ID ";
                    break;
            }

            ArticuloContext cc = new ArticuloContext();
            string condition = " and c.obsoleto='0'";
            string cadenaSQl = ConsultaSql + " " + Filtro +" "+ condition+ " " + orden;
            var elementos1 = cc.Database.SqlQuery<Caracteristica>(cadenaSQl).ToList();
            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = cc.Caracteristica.OrderBy(e => e.ID).Count(); // Total number of elements

            // Populate DropDownList
            ViewBag.PageSize = new List<SelectListItem>()
            {
                new SelectListItem { Text = "10", Value = "10", Selected = true },
                new SelectListItem { Text = "25", Value = "25" },
                new SelectListItem { Text = "50", Value = "50" },
                new SelectListItem { Text = "100", Value = "100" },
                new SelectListItem { Text = "Todos", Value = elementos1.Count.ToString() }
             };

            int pageNumber = (page ?? 1);
            int pageSize = (PageSize ?? 10);
            ViewBag.psize = pageSize;

            return View(elementos1.ToPagedList(pageNumber, pageSize));
        }

       
  [HttpPost]

       public static void EscribeArchivoXMLC(string contenido, string rutaArchivo, bool sobrescribir = true)
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



        public ActionResult Termoencogible(int? Id, int idsuaje = 0)
        {
            ViewBag.IDCentro = new Repository().GetCentros(0);
            ViewBag.IDCaja = new Repository().GetCajas(0);

            ClsCotizador elemento = new ClsCotizador();
            

            elemento.SuajeNuevo = true;
            
            
                elemento.TCcotizado = SIAAPI.Properties.Settings.Default.TCcotizador;
            
                ViewBag.IDCotizacion = 0;
                elemento.IDMonedapreciosconvenidos = new c_MonedaContext().c_Monedas.Where(s => s.ClaveMoneda == "USD").ToList().FirstOrDefault().IDMoneda;
                elemento.CostoM2Cinta = SIAAPI.Properties.Settings.Default.CostoM2default;
                elemento.TCcotizado = SIAAPI.Properties.Settings.Default.TCcotizador;
                elemento.DiluirSuajeEnPedidos = 50;
                elemento.Yatienematriz = false;
                elemento.CobrarMaster = false;
                ViewBag.Mensajedeerror = "";
                elemento.cavidadesdesuaje = 1;
                elemento.cavidadesdesuajeAvance = 1;
                elemento.Cantidadxrollo = 500;
                elemento.productosalpaso = 1;
                
                ViewBag.IDSuaje = 0;
                var suajes2 = new Repository().GetPlecas();
                ViewBag.IDSuaje2 = suajes2;
                var cintas = new Repository().GetCintasTermo();
                ViewBag.IDMaterial = cintas;
                ViewBag.Mensajedeerror = "Estamos iniciando la cotización";
                ViewBag.IDMAterial2 = cintas;

                ViewBag.IDMAterial3 = cintas;

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
              
              
                ViewBag.IDMAterial2 = cintas;
                ViewBag.IDMAterial3 = cintas;
            }

            else   // viene de cargar el archivo de la cotizacion

            {


                ViewBag.Mensajedeerror = "";



                Cotizaciones archivocotizacion = new ArchivoCotizadorContext().cotizaciones.Find(Id);
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



                //Cotizaciones archivocotizacion = new ArchivoCotizadorContext().cotizaciones.Find(Id);
                //XmlDocument documento = new XmlDocument();
                //string nombredearchivo = archivocotizacion.Ruta.TrimEnd() + "\\" + archivocotizacion.NombreArchivo + ".xml";
                //documento.Load(nombredearchivo);
                //elemento = null;
                //XmlSerializer serializer = new XmlSerializer(typeof(ClsCotizador));
                //using (Stream reader = new FileStream(nombredearchivo, FileMode.Open))
                //{
                //    // Call the Deserialize method to restore the object's state.
                //    elemento = (ClsCotizador)serializer.Deserialize(reader);
                //}

                if (elemento.IDMonedapreciosconvenidos == 0)

                {
                    elemento.IDMonedapreciosconvenidos = new c_MonedaContext().c_Monedas.Where(s => s.ClaveMoneda == "USD").ToList().FirstOrDefault().IDMoneda;
                }
                if (elemento.TCcotizado == 0)

                {
                    elemento.TCcotizado = SIAAPI.Properties.Settings.Default.TCcotizador;
                }

                //ViewData["TSuaje"] = elemento.TipoSuaje;

                ViewData["Tintas"] = null;
                try
                {

                    ViewData["Tintas"] = elemento.Tintas;
                }
                catch (Exception err)
                {
                    string mensaje = err.Message;
                }
                if (elemento.Tintas.Count() ==  0)
                {
                    elemento.MangaTransparente = true;
                }
                ViewBag.Rango1 = elemento.Rango1;
                ViewBag.Rango2 = elemento.Rango2;
                ViewBag.Rango3 = elemento.Rango3;
                ViewBag.Rango4 = elemento.Rango4;

                elemento.IDCotizacion = int.Parse((Id == null ? 0 : Id).ToString());

                ViewBag.IDCotizacion = elemento.IDCotizacion;
                var suajes = new Repository().GetSuajes(elemento.IDSuaje);
                ViewBag.IDSuaje = suajes;


                var cintas2 = new Repository().GetCintasbyClave(elemento.IDMaterial2);
                ViewBag.IDMaterial2 = cintas2;

                var cintas3 = new Repository().GetCintasbyClave(elemento.IDMaterial3);
                ViewBag.IDMaterial2 = cintas3;


            }


            return View(elemento);
        }


        [HttpPost]
        public ActionResult Termoencogible(ClsCotizador elemento, FormCollection colecciondeelementos)
        {

            elemento.SuajeNuevo = true;
            elemento = verificaprecios(elemento);

            ViewBag.IDCentro = new Repository().GetCentros(elemento.IDCentro);
            ViewBag.IDCaja = new Repository().GetCajas(elemento.IDCaja);

            elemento.mangatermo = true;
            if (elemento.TCcotizado == 0)

            {
                elemento.TCcotizado = SIAAPI.Properties.Settings.Default.TCcotizador;
            }
            ViewBag.IDCotizacion = elemento.IDCotizacion;

            elemento.mangatermo = true;



            FormulaEspecializada.Formulaespecializada formula = new FormulaEspecializada.Formulaespecializada();
            formula = igualar(elemento, formula);


            try
            {
                string cm = colecciondeelementos.Get("Monedafinal");
                elemento.IDMonedapreciosconvenidos = new c_MonedaContext().c_Monedas.Where(s => s.ClaveMoneda == cm).ToList().FirstOrDefault().IDMoneda;
            }
            catch (Exception err)
            {
                string menjsaje1 = err.Message;
            }

            ///////////////////////////// suajes ///////////////////
            ViewBag.EnqueEstoy = 1;

            var suajes = new Repository().GetSuajes(0);

            ViewBag.IDSuaje = suajes;

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


            /////////////////////////////Cintas ///////////////////
            var cintas = new Repository().GetCintasbyClave(elemento.IDMaterial);

            if (elemento.IDMaterial != 0)
            {
                try
                {
                   
                        elemento.Materialnecesitarefile = true;
                        formula.Materialnecesitarefile = false;
                    
                }
                catch (Exception err)
                {
                    string mensajeerror = err.Message;
                }
            }

            ViewBag.IDMaterial = cintas;

            ViewBag.IDMAterial2 = new Repository().GetCintasbyClave(elemento.IDMaterial2);

            ViewBag.IDMAterial3 = new Repository().GetCintasbyClave(elemento.IDMaterial3);
            ///////////////////// Tintas //////////////////////////////////////


            var tintas = Gettintasiniciales(formula.Numerodetintas, colecciondeelementos, elemento.CantidadMPMts2);
            ViewData["Tintas"] = null;
            try
            {
               
                ViewData["Tintas"] = tintas;
            }
            catch (Exception err)
            {
                string mensaje = err.Message;
            }

            formula = pasaTintas(tintas, formula); // lo paso para calcular
            elemento = pasaTintas(tintas, elemento); // lo paso al formulario web para generar archivo

            formula.CobrarMaster = elemento.CobrarMaster;
            formula.anchommmaster = elemento.anchommmaster;


            if (elemento.IDMaterial2 != 0)
            {
                try
                {
                    Materiales mat2 = new MaterialesContext().Materiales.Find(elemento.IDMaterial2);
                    if (mat2.Completo)
                    {
                        formula.CobrarMaster2m = true;


                    }
                }
                catch (Exception err)
                {
                    string mensajeerror = err.Message;
                }
            }



            //////////////////////////  Calcular //////////////////////



            formula.Calcular();

            formula = CosteaTintas(formula);

            decimal mo = formula.getHoraPrensa();

            decimal moe = 0;

            if (formula.Cantidadxrollo > 0)
            {

                moe = formula.getHoraEmbobinado();
            }

            elemento.Costo1mxn = elemento.Costo1 * SIAAPI.Properties.Settings.Default.TCcotizador;
            elemento.Costo2mxn = elemento.Costo2 * SIAAPI.Properties.Settings.Default.TCcotizador;
            elemento.Costo3mxn = elemento.Costo3 * SIAAPI.Properties.Settings.Default.TCcotizador;
            elemento.Costo4mxn = elemento.Costo4 * SIAAPI.Properties.Settings.Default.TCcotizador;

            elemento.MaterialNecesitado = formula.MaterialNecesitado;
            elemento.Minimoproducir = formula.Minimoproducir;

            //if (elemento.Cantidad < elemento.Minimoproducir && elemento.Cantidad > 0)
            //{
            //    elemento.Cantidad = elemento.Minimoproducir;
            //}
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
          
            ViewBag.EnqueEstoy = 1;

            ViewBag.Mensajedeerror = "";
            ViewBag.Minino = Math.Round(elemento.Minimoproducir, 2);

            bool pasa = true;

            bool pasafinal = true;

            
            if (elemento.Cantidad == 0)
            {

                ViewBag.Mensajedeerror += "No hay datos suficientes para calcular un precio, Comienza por dar  una cantidad a producir\n";
                pasa = false;
            }


            if (elemento.Minimoproducir == 0)
            {
                ViewBag.Mensajedeerror += "No hay datos suficientes para calcular un precio, Comienza por dar las medidas de la etiqueta y una cantidad a producir\n";
                pasa = false;
            }

            if (elemento.Cantidadxrollo == 0)
            {
                ViewBag.Mensajedeerror += "No has elegido la cantidad por rollo o paquete \n";
                pasa = false;
            }


            if (elemento.IDMaterial == 0)
            {
                ViewBag.Mensajedeerror += "\nNo has seleccionado una cinta con la cual trabajar";
                pasa = false;
            }
            if ((elemento.IDMaterial > 0) && (elemento.CostoM2Cinta == 0))
            {
                ViewBag.Mensajedeerror += "\nLa Cinta elegida no contiene un costo asignado, registra un costo o escribe manualmente el costo pretendido en la cinta\n";
                pasa = false;
            }
            if ((elemento.IDMaterial2 > 0) && (elemento.CostoM2Cinta2 == 0))
            {
                ViewBag.Mensajedeerror += "\nLa Cinta Adicional elegida no contiene un costo asignado, registra un costo o escribe manualmente el costo pretendido en la cinta Adicional\n";
                pasa = false;
            }

            if ((elemento.IDMaterial3 > 0) && (elemento.CostoM2Cinta3 == 0))
            {
                ViewBag.Mensajedeerror += "\nLa Cinta Adicional elegida no contiene un costo asignado, registra un costo o escribe manualmente el costo pretendido en la cinta Adicional\n";
                pasa = false;
            }

            if(verificatintas(elemento)==false)
            {
                ViewBag.Mensajedeerror += "\nindicaste que tenias tintas pero al menos una de ellas no seleccionaste que tinta\n";
                pasa = false;
            }

            try
            {
                if (colecciondeelementos.Get("Rango1").ToString() == "" || colecciondeelementos.Get("Rango1").ToString() == "0")
                {
                    throw new Exception("El Rango 1 es null o 0");
                }
                elemento.Rango1 = decimal.Parse(colecciondeelementos.Get("Rango1").ToString());
                if (elemento.Rango1 < 0)
                {
                    throw new Exception("El Rango 1 es menor a 0");
                }
                ViewBag.Rango1 = elemento.Rango1;
            }

            catch (Exception err)
            {
                string mensajedeerror = err.Message;
                pasafinal = false;
            }

            try
            {
                if (colecciondeelementos.Get("Rango2").ToString() == "" || colecciondeelementos.Get("Rango2").ToString() == "0")
                {
                    throw new Exception("El Rango 2 es null o 0");
                }
                elemento.Rango2 = decimal.Parse(colecciondeelementos.Get("Rango2").ToString());
                if (elemento.Rango1 < 0)
                {
                    throw new Exception("El Rango 2 es menor a 0");
                }
                ViewBag.Rango2 = elemento.Rango2;
            }
            catch (Exception err)
            {
                string mensajedeerror = err.Message;
                pasafinal = false;
            }


            try
            {
                if (colecciondeelementos.Get("Rango3").ToString() == "" || colecciondeelementos.Get("Rango3").ToString() == "0")
                {
                    throw new Exception("El Rango 3 es null o 0");
                }
                elemento.Rango3 = decimal.Parse(colecciondeelementos.Get("Rango3").ToString());
                if (elemento.Rango1 < 0)
                {
                    throw new Exception("El Rango 3 es menor a 0");
                }
                ViewBag.Rango3 = elemento.Rango3;
            }
            catch (Exception err)
            {
                string mensajedeerror = err.Message;
                pasafinal = false;
            }

            try
            {
                if (colecciondeelementos.Get("Rango4").ToString() == "" || colecciondeelementos.Get("Rango4").ToString() == "0")
                {
                    throw new Exception("El Rango 4 es null o 0");
                }
                elemento.Rango4 = decimal.Parse(colecciondeelementos.Get("Rango4").ToString());
                if (elemento.Rango1 < 0)
                {
                    throw new Exception("El Rango 4 es menor a 0");
                }
                ViewBag.Rango4 = elemento.Rango4;
            }
            catch (Exception err)
            {
                string mensajedeerror = err.Message;
                pasafinal = false;
            }

            if (elemento.Tintas.Count() == 0)
            {
                elemento.MangaTransparente = true;
            }
            if (elemento.Rango1 == 0 && elemento.Rango2 == 0)
            {
                if (elemento.Cantidad < formula.Minimoproducir)
                {
                    elemento.Rango1 = elemento.Cantidad;
                    elemento.Rango2 = Math.Round(elemento.Minimoproducir, 1);
                }
                if (elemento.Cantidad >= formula.Minimoproducir)
                {
                    elemento.Rango1 = Math.Round(elemento.Minimoproducir, 1);
                    elemento.Rango2 = elemento.Cantidad;
                }
            }



            if (mo == 0 || moe == 0)
            {
                pasafinal = false;
            }

            if (pasa && pasafinal)
            {
                decimal costosuaje = new ArticuloContext().Database.SqlQuery<decimal>("select dbo.GetCosto(0," + elemento.IDSuaje + ",1)").ToList().FirstOrDefault();
                if (elemento.DiluirSuajeEnPedidos == 0)
                { elemento.DiluirSuajeEnPedidos = 50; }
                costosuaje = costosuaje / elemento.DiluirSuajeEnPedidos;

                if (costosuaje < 12)
                {
                    costosuaje = 12;
                }


                // suma del costo del suaje diluido + costo materias primas + costo de tintas + costo de prensa + costo de embobinado 



                FormulaEspecializada.Formulaespecializada Formulapara1 = new FormulaEspecializada.Formulaespecializada();
                Formulapara1 = igualar(elemento, Formulapara1);
                Formulapara1.Cantidad = decimal.Parse(colecciondeelementos.Get("Rango1"));
                Formulapara1 = pasaTintas(tintas, Formulapara1);
                Formulapara1.CobrarMaster = elemento.CobrarMaster;
                Formulapara1.anchommmaster = elemento.anchommmaster;



                Formulapara1.CobrarMaster = elemento.CobrarMaster;

                if (elemento.IDMaterial2 != 0)
                {
                    try
                    {
                        Materiales mat2 = new MaterialesContext().Materiales.Find(elemento.IDMaterial2);
                        if (mat2.Completo)
                        {
                            Formulapara1.CobrarMaster2m = true;


                        }
                    }
                    catch (Exception err)
                    {
                        string mensajeerror = err.Message;
                    }
                }


                Formulapara1.Calcular();
                Formulapara1 = CosteaTintas(Formulapara1);
                Formulapara1.calcularCostoMO();


                decimal costototal = (costosuaje) + Formulapara1.CostototalMP + Formulapara1.Costodetintas + Formulapara1.CostototalMO + SIAAPI.Properties.Settings.Default.costodeempaque;

                decimal costoxmillar = costototal / Formulapara1.Cantidad;
                elemento.Costo1 = costoxmillar;

                elemento.MatrizPrecio.Fila1.Celda1.Valor = Math.Round((costoxmillar * (1 / ((100 - Formulapara1.Rango1gain) / 100))), 2);
                elemento.MatrizPrecio.Fila1.Celda2.Valor = Math.Round((costoxmillar * (1 / ((100 - Formulapara1.Rango2gain) / 100))), 2);
                elemento.MatrizPrecio.Fila1.Celda3.Valor = Math.Round((costoxmillar * (1 / ((100 - Formulapara1.Rango3gain) / 100))), 2);
                elemento.MatrizPrecio.Fila1.Celda4.Valor = Math.Round((costoxmillar * (1 / ((100 - Formulapara1.Rango4gain) / 100))), 2);


                FormulaEspecializada.Formulaespecializada Formulapara2 = new FormulaEspecializada.Formulaespecializada();
                Formulapara2 = igualar(elemento, Formulapara2);
                Formulapara2.Cantidad = decimal.Parse(colecciondeelementos.Get("Rango2"));
                Formulapara2 = pasaTintas(tintas, Formulapara2);
                Formulapara2.CobrarMaster = elemento.CobrarMaster;
                Formulapara2.anchommmaster = elemento.anchommmaster;

                if (elemento.IDMaterial2 != 0)
                {
                    try
                    {
                        Materiales mat2 = new MaterialesContext().Materiales.Find(elemento.IDMaterial2);
                        if (mat2.Completo)
                        {
                            Formulapara2.CobrarMaster2m = true;


                        }
                    }
                    catch (Exception err)
                    {
                        string mensajeerror = err.Message;
                    }
                }


                Formulapara2.Calcular();
                Formulapara2 = CosteaTintas(Formulapara2);
                Formulapara2.calcularCostoMO();

                costototal = (costosuaje) + Formulapara2.CostototalMP + Formulapara2.Costodetintas + Formulapara2.CostototalMO + SIAAPI.Properties.Settings.Default.costodeempaque;

                costoxmillar = costototal / Formulapara2.Cantidad;

                elemento.Costo2 = costoxmillar;

                elemento.MatrizPrecio.Fila2.Celda1.Valor = Math.Round((costoxmillar * (1 / ((100 - Formulapara2.Rango1gain) / 100))), 2);
                elemento.MatrizPrecio.Fila2.Celda2.Valor = Math.Round((costoxmillar * (1 / ((100 - Formulapara2.Rango2gain) / 100))), 2);
                elemento.MatrizPrecio.Fila2.Celda3.Valor = Math.Round((costoxmillar * (1 / ((100 - Formulapara2.Rango3gain) / 100))), 2);
                elemento.MatrizPrecio.Fila2.Celda4.Valor = Math.Round((costoxmillar * (1 / ((100 - Formulapara2.Rango4gain) / 100))), 2);


                FormulaEspecializada.Formulaespecializada Formulapara3 = new FormulaEspecializada.Formulaespecializada();
                Formulapara3 = igualar(elemento, Formulapara3);
                Formulapara3.Cantidad = decimal.Parse(colecciondeelementos.Get("Rango3"));
                Formulapara3 = pasaTintas(tintas, Formulapara3);
                Formulapara3.CobrarMaster = elemento.CobrarMaster;
                Formulapara3.anchommmaster = elemento.anchommmaster;

                if (elemento.IDMaterial2 != 0)
                {
                    try
                    {
                        Materiales mat2 = new MaterialesContext().Materiales.Find(elemento.IDMaterial2);
                        if (mat2.Completo)
                        {
                            Formulapara3.CobrarMaster2m = true;


                        }
                    }
                    catch (Exception err)
                    {
                        string mensajeerror = err.Message;
                    }
                }
                Formulapara3.Calcular();
                Formulapara3 = CosteaTintas(Formulapara3);
                Formulapara3.calcularCostoMO();

                costototal = (costosuaje) + Formulapara3.CostototalMP + Formulapara3.Costodetintas + Formulapara3.CostototalMO + SIAAPI.Properties.Settings.Default.costodeempaque;

                costoxmillar = costototal / Formulapara3.Cantidad;

                elemento.Costo3 = costoxmillar;

                elemento.MatrizPrecio.Fila3.Celda1.Valor = Math.Round((costoxmillar * (1 / ((100 - Formulapara3.Rango1gain) / 100))), 2);
                elemento.MatrizPrecio.Fila3.Celda2.Valor = Math.Round((costoxmillar * (1 / ((100 - Formulapara3.Rango2gain) / 100))), 2);
                elemento.MatrizPrecio.Fila3.Celda3.Valor = Math.Round((costoxmillar * (1 / ((100 - Formulapara3.Rango3gain) / 100))), 2);
                elemento.MatrizPrecio.Fila3.Celda4.Valor = Math.Round((costoxmillar * (1 / ((100 - Formulapara3.Rango4gain) / 100))), 2);

                FormulaEspecializada.Formulaespecializada Formulapara4 = new FormulaEspecializada.Formulaespecializada();
                Formulapara4 = igualar(elemento, Formulapara4);
                Formulapara4.Cantidad = decimal.Parse(colecciondeelementos.Get("Rango4"));
                Formulapara4 = pasaTintas(tintas, Formulapara4);
                Formulapara4.CobrarMaster = elemento.CobrarMaster;
                Formulapara4.anchommmaster = elemento.anchommmaster;

                if (elemento.IDMaterial2 != 0)
                {
                    try
                    {
                        Materiales mat2 = new MaterialesContext().Materiales.Find(elemento.IDMaterial2);
                        if (mat2.Completo)
                        {
                            Formulapara4.CobrarMaster2m = true;


                        }
                    }
                    catch (Exception err)
                    {
                        string mensajeerror = err.Message;
                    }
                }
                Formulapara4.Calcular();
                Formulapara4 = CosteaTintas(Formulapara4);
                Formulapara4.calcularCostoMO();

                costototal = (costosuaje) + Formulapara4.CostototalMP + Formulapara4.Costodetintas + Formulapara4.CostototalMO + SIAAPI.Properties.Settings.Default.costodeempaque;

                costoxmillar = costototal / Formulapara4.Cantidad;

                elemento.Costo4 = costoxmillar;

                elemento.MatrizPrecio.Fila4.Celda1.Valor = Math.Round((costoxmillar * (1 / ((100 - Formulapara4.Rango1gain) / 100))), 2);
                elemento.MatrizPrecio.Fila4.Celda2.Valor = Math.Round((costoxmillar * (1 / ((100 - Formulapara4.Rango2gain) / 100))), 2);
                elemento.MatrizPrecio.Fila4.Celda3.Valor = Math.Round((costoxmillar * (1 / ((100 - Formulapara4.Rango3gain) / 100))), 2);
                elemento.MatrizPrecio.Fila4.Celda4.Valor = Math.Round((costoxmillar * (1 / ((100 - Formulapara4.Rango4gain) / 100))), 2);

                elemento.Yatienematriz = true;

                try
                {
                    elemento.precioconvenidos.precio1 = decimal.Parse(colecciondeelementos.Get("precioconvenido1"));

                    elemento.precioconvenidos.precio2 = decimal.Parse(colecciondeelementos.Get("precioconvenido2"));
                    elemento.precioconvenidos.precio3 = decimal.Parse(colecciondeelementos.Get("precioconvenido3"));
                    elemento.precioconvenidos.precio4 = decimal.Parse(colecciondeelementos.Get("precioconvenido4"));
                }
                catch (Exception err)
                {

                }


                if (elemento.precioconvenidos.precio1 == 0)
                {
                    elemento.precioconvenidos.precio1 = elemento.MatrizPrecio.Fila1.Celda1.Valor;
                }
                if (elemento.precioconvenidos.precio2 == 0)
                {
                    elemento.precioconvenidos.precio2 = elemento.MatrizPrecio.Fila2.Celda1.Valor;
                }
                if (elemento.precioconvenidos.precio3 == 0)
                {
                    elemento.precioconvenidos.precio3 = elemento.MatrizPrecio.Fila3.Celda1.Valor;
                }
                if (elemento.precioconvenidos.precio4 == 0)
                {
                    elemento.precioconvenidos.precio4 = elemento.MatrizPrecio.Fila4.Celda1.Valor;
                }

                if (colecciondeelementos.Get("Enviar") == "Sobreescribir")
                {
                    Cotizaciones archivocotizacion = new ArchivoCotizadorContext().cotizaciones.Find(elemento.IDCotizacion);
                    XmlDocument documento = new XmlDocument();
                    string nombredearchivo = archivocotizacion.Ruta.TrimEnd() + "\\" + archivocotizacion.NombreArchivo + ".xml";


                    StringWriter stringwriter = new StringWriter();
                    XmlSerializer x = new XmlSerializer(elemento.GetType());
                    x.Serialize(stringwriter, elemento);

                    string xmlstring = stringwriter.ToString().Replace("encoding=\"utf-16\"", "");


                    EscribeArchivoXMLC(xmlstring, nombredearchivo, true);
                }

                if (colecciondeelementos.Get("Enviar") == "Grabar Archivo")
                {
                    string NombredeArchivo = DateTime.Now.ToString().Replace(".", "").Replace(",", "").Replace(" ", "").Replace("/", "").Replace(":", "");

                    string nombredecarpeta = DateTime.Now.Month + "" + DateTime.Now.Year + " " + User.Identity.Name;

                    if (!Directory.Exists(System.Web.HttpContext.Current.Server.MapPath("~/Cotizaciones")))
                    {
                        Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath("~/Cotizaciones"));
                    }
                    if (!Directory.Exists(System.Web.HttpContext.Current.Server.MapPath("~/Cotizaciones/" + User.Identity.Name)))
                    {
                        Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath("~/Cotizaciones" + User.Identity.Name));
                    }
                    if (!Directory.Exists(System.Web.HttpContext.Current.Server.MapPath("~/Cotizaciones/" + User.Identity.Name + "/" + nombredecarpeta)))
                    {
                        Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath("~/Cotizaciones/" + User.Identity.Name + "/" + nombredecarpeta));
                    }


                    nombredecarpeta = System.Web.HttpContext.Current.Server.MapPath("~/Cotizaciones/" + User.Identity.Name + "/" + nombredecarpeta);


                    this.GrabarArchivoCotizador(elemento, NombredeArchivo, nombredecarpeta);

                    return RedirectToAction("ArchivoCotizador", new { _nombredearchivo = NombredeArchivo, _ruta = nombredecarpeta, termo = 1, suajeN = 0, suajeE = 0 });

                }
                elemento.IDCotizacion = ViewBag.IDCotizacion;

                if (colecciondeelementos.Get("Enviar") == "Crear Articulo" && elemento.IDCotizacion > 0) //llego hasta crear articulo
                {


                    return RedirectToAction("CreaArticulo", new { id = elemento.IDCotizacion });


                }
                if (colecciondeelementos.Get("Enviar") == "Asignar Articulo" && elemento.IDCotizacion > 0) //llego hasta crear articulo
                {


                    return RedirectToAction("Asignarunarticulo", new { id = elemento.IDCotizacion });


                }
                if (colecciondeelementos.Get("Enviar") == "Crear PDF" && elemento.IDCotizacion > 0) //llego hasta crear articulo
                {


                    return RedirectToAction("CrearCotizacionPDF", new { IDCotizacion = elemento.IDCotizacion });


                }
            }



            return View(elemento);

        }


        public ActionResult SuajeNuevo(int? Id, int idsuaje = 0)
        {
            ViewBag.IDCentro = new Repository().GetCentros(0);
            ViewBag.IDCaja = new Repository().GetCajas(0);

            ClsCotizador elemento = new ClsCotizador();

            Paramsuajes parametrossuajes = new ParamsuajesContext().Paramsuajes.FirstOrDefault();
            ViewBag.Paramsuajes = parametrossuajes;

            List<Articulo> articulos = new List<Articulo>();
            string cadena = "select * from Articulo where descripcion like '%BARNIZ%'";
            articulos = db.Database.SqlQuery<Articulo>(cadena).ToList();
            List<SelectListItem> listaArticulo = new List<SelectListItem>();
            listaArticulo.Add(new SelectListItem { Text = "--Todos los artículos--", Value = "0" });

            foreach (var m in articulos)
            {
                listaArticulo.Add(new SelectListItem { Text = m.Descripcion, Value = m.IDArticulo.ToString() });
            }
            //ViewBag.ACABADOL = listaArticulo;

            var AcabadoL = new List<SelectListItem>();
            AcabadoL.Add(new SelectListItem { Text = "---Seleccionar---", Value = "N/A" });

            AcabadoL.Add(new SelectListItem { Text = "LAMINADO BRILLANTE", Value = "LAMINADO BRILLANTE" });
            AcabadoL.Add(new SelectListItem { Text = "LAMINADO MATE", Value = "LAMINADO MATE" });
            AcabadoL.Add(new SelectListItem { Text = "BARNIZ BRILLANTE", Value = "BARNIZ BRILLANTE" });
            AcabadoL.Add(new SelectListItem { Text = "BARNIZ MATE", Value = "BARNIZ MATE" });
            AcabadoL.Add(new SelectListItem { Text = "BARNIZ RELEASE", Value = "BARNIZ RELEASE" });
            AcabadoL.Add(new SelectListItem { Text = "FOIL", Value = "FOIL" });



            ViewBag.ACABADO = new SelectList(AcabadoL, "Value", "Text");

            ViewData["ACABADO"] = AcabadoL;



            var TipoSuaje = new List<SelectListItem>();
            TipoSuaje.Add(new SelectListItem { Text = "---Seleccionar---", Value = "S/N" });

            TipoSuaje.Add(new SelectListItem { Text = "Macizo", Value = "M" });
            TipoSuaje.Add(new SelectListItem { Text = "Flexible", Value = "F" });

            ViewBag.TSuaje = new SelectList(TipoSuaje, "Value", "Text");



            ViewData["TSuaje"] = TipoSuaje;



            var Almas = new List<SelectListItem>();
            Almas.Add(new SelectListItem { Text = "---Seleccionar---", Value = "0" });

            Almas.Add(new SelectListItem { Text = "88", Value = "88" });
            Almas.Add(new SelectListItem { Text = "98", Value = "98" });
            Almas.Add(new SelectListItem { Text = "104", Value = "104" });
            Almas.Add(new SelectListItem { Text = "108", Value = "108" });
            Almas.Add(new SelectListItem { Text = "116", Value = "116" });
            Almas.Add(new SelectListItem { Text = "122", Value = "122" });
            Almas.Add(new SelectListItem { Text = "126", Value = "126" });

            ViewBag.Almas = new SelectList(Almas, "Value", "Text");

            ViewData["Almas"] = Almas;




            var TipoFigura = new List<SelectListItem>();
            //FacPagLst.Add(new SelectListItem { Text = "Todos", Value = "na", Selected = true });
            TipoFigura.Add(new SelectListItem { Text = "---Seleccionar---", Value = "S/N" });
        
            TipoFigura.Add(new SelectListItem { Text = "Circulo", Value = "C" });
            TipoFigura.Add(new SelectListItem { Text = "Rectangulo", Value = "R" });
            TipoFigura.Add(new SelectListItem { Text = "Triangulo", Value = "T" });
            TipoFigura.Add(new SelectListItem { Text = "Cuadrado", Value = "Cu" });
            TipoFigura.Add(new SelectListItem { Text = "Irregular", Value = "I" });
            TipoFigura.Add(new SelectListItem { Text = "Muesca General", Value = "Muesca General" });
            TipoFigura.Add(new SelectListItem { Text = "Muesca Especial", Value = "Muesca Especial" });
            TipoFigura.Add(new SelectListItem { Text = "Corte Recto", Value = "Corte Recto" });
            TipoFigura.Add(new SelectListItem { Text = "Figura Especial", Value = "Figura Especial" });

            ViewBag.TSuajeFi = new SelectList(TipoFigura, "Value", "Text");

            ViewData["TSuajeFi"] = TipoFigura;

            var TipoCorte = new List<SelectListItem>();
            //FacPagLst.Add(new SelectListItem { Text = "Todos", Value = "na", Selected = true });
            TipoCorte.Add(new SelectListItem { Text = "---Seleccionar---", Value = "S/N" });
            TipoCorte.Add(new SelectListItem { Text = "Medio Corte", Value = "M" });
            TipoCorte.Add(new SelectListItem { Text = "Corte Completo", Value = "CC" });
            TipoCorte.Add(new SelectListItem { Text = "Corte a Liner", Value = "CL" });
            TipoCorte.Add(new SelectListItem { Text = "Pleca", Value = "P" });

            ViewBag.TSuajeCorte = new SelectList(TipoCorte, "Value", "Text");

            ViewData["TSuajeCorte"] = TipoCorte;

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

            ViewBag.EsquinaSuaje = new SelectList(EsquinasSuaje, "Value", "Text");

            ViewData["EsquinaSuaje"] = EsquinasSuaje;






            elemento.SuajeNuevo = true;

            if (elemento.TCcotizado == 0)

            {
                elemento.TCcotizado = SIAAPI.Properties.Settings.Default.TCcotizador;
            }

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
                var suajes2 = new Repository().GetPlecas();
                ViewBag.IDSuaje2 = suajes2;
                var cintas = new Repository().GetCintas();
                ViewBag.IDMaterial = cintas;
                ViewBag.Mensajedeerror = "Estamos iniciando la cotización";
                ViewBag.IDMAterial2 = cintas;
                ViewBag.IDMAterial3 = cintas;
            }

            else   // viene de cargar el archivo de la cotizacion

            {


                ViewBag.Mensajedeerror = "";
                Cotizaciones archivocotizacion = new ArchivoCotizadorContext().cotizaciones.Find(Id);
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


             

                if (elemento.IDMonedapreciosconvenidos == 0)

                {
                    elemento.IDMonedapreciosconvenidos = new c_MonedaContext().c_Monedas.Where(s => s.ClaveMoneda == "USD").ToList().FirstOrDefault().IDMoneda;
                }
                if (elemento.TCcotizado == 0)

                {
                    elemento.TCcotizado = SIAAPI.Properties.Settings.Default.TCcotizador;
                }

                //ViewData["TSuaje"] = elemento.TipoSuaje;

                ViewData["Tintas"] = null;
                try
                {

                    ViewData["Tintas"] = elemento.Tintas;
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

                var suajes2 = new Repository().GetPlecas(elemento.IDSuaje2);
                ViewBag.IDSuaje2 = suajes2;

                var cintas = new Repository().GetCintasbyClave(elemento.IDMaterial);
                ViewBag.IDMaterial = cintas;

                var cintas2 = new Repository().GetCintasbyClave(elemento.IDMaterial2);
                ViewBag.IDMaterial2 = cintas2;

                var cintas3 = new Repository().GetCintasbyClave(elemento.IDMaterial3);
                ViewBag.IDMaterial3 = cintas3;


                //TipoSuaje = new List<SelectListItem>();
                //TipoSuaje.Add(new SelectListItem { Text = "---Seleccionar---", Value = "S/N" });
                //TipoSuaje.Add(new SelectListItem { Text = "Macizo", Value = "M" });
                //TipoSuaje.Add(new SelectListItem { Text = "Flexible", Value = "F" });
                ViewBag.TSuaje = new SelectList(TipoSuaje, "Value", "Text", elemento.TipoSuaje);

               // TipoFigura = new List<SelectListItem>();
               // TipoFigura.Add(new SelectListItem { Text = "---Seleccionar---", Value = "S/N" });
               // TipoFigura.Add(new SelectListItem { Text = "Circulo", Value = "C" });
               // TipoFigura.Add(new SelectListItem { Text = "Rectangulo", Value = "R" });
               // TipoFigura.Add(new SelectListItem { Text = "Triangulo", Value = "T" });
               // TipoFigura.Add(new SelectListItem { Text = "Cuadrado", Value = "Cu" });
               // TipoFigura.Add(new SelectListItem { Text = "Irregular", Value = "I" });
               //TipoFigura.Add(new SelectListItem { Text = "Muesca General", Value = "Muesca General" });
               // TipoFigura.Add(new SelectListItem { Text = "Muesca Especial", Value = "Muesca Especial" });
               // TipoFigura.Add(new SelectListItem { Text = "Corte Recto", Value = "Corte Recto" });
               // TipoFigura.Add(new SelectListItem { Text = "Figura Especial", Value = "Figura Especial" });

                ViewBag.TSuajeFi = new SelectList(TipoFigura, "Value", "Text", elemento.TipoSuajeFigura);

                //Almas = new List<SelectListItem>();
                //Almas.Add(new SelectListItem { Text = "---Seleccionar---", Value = "0" });

                //Almas.Add(new SelectListItem { Text = "88", Value = "88" });
                //Almas.Add(new SelectListItem { Text = "98", Value = "98" });
                //Almas.Add(new SelectListItem { Text = "104", Value = "104" });
                //Almas.Add(new SelectListItem { Text = "108", Value = "108" });
                //Almas.Add(new SelectListItem { Text = "116", Value = "116" });
                //Almas.Add(new SelectListItem { Text = "122", Value = "122" });
                //Almas.Add(new SelectListItem { Text = "126", Value = "126" });

                ViewBag.Almas = new SelectList(Almas, "Value", "Text", elemento.TH);



                //TipoCorte = new List<SelectListItem>();
                //TipoCorte.Add(new SelectListItem { Text = "---Seleccionar---", Value = "S/N" });
                //TipoCorte.Add(new SelectListItem { Text = "Medio Corte", Value = "M" });
                //TipoCorte.Add(new SelectListItem { Text = "Corte Completo", Value = "CC" });
                //TipoCorte.Add(new SelectListItem { Text = "Corte a Liner", Value = "CL" });
                //TipoCorte.Add(new SelectListItem { Text = "Pleca", Value = "P" });

                ViewBag.TSuajeCorte = new SelectList(TipoCorte, "Value", "Text", elemento.TipoCorte);

                //  EsquinasSuaje = new List<SelectListItem>();
                //EsquinasSuaje = new List<SelectListItem>();
                //EsquinasSuaje.Add(new SelectListItem { Text = "N/A", Value = "N/A" });
                //EsquinasSuaje.Add(new SelectListItem { Text = "1/2 in", Value = "1/2 in" });
                //EsquinasSuaje.Add(new SelectListItem { Text = "1/4 in", Value = "1/4 in" });
                //EsquinasSuaje.Add(new SelectListItem { Text = "1/8 in", Value = "1/8 in" });
                //EsquinasSuaje.Add(new SelectListItem { Text = "1/16 in", Value = "1/16 in" });
                //EsquinasSuaje.Add(new SelectListItem { Text = "1/32 in", Value = "1/32 in" });
                //EsquinasSuaje.Add(new SelectListItem { Text = "1/64 in", Value = "1/64 in" });
                //EsquinasSuaje.Add(new SelectListItem { Text = "3/4 in", Value = "3/4 in" });
                //EsquinasSuaje.Add(new SelectListItem { Text = "3/8 in", Value = "3/8 in" });
                //EsquinasSuaje.Add(new SelectListItem { Text = "3/16 in", Value = "3/16 in" });
                //EsquinasSuaje.Add(new SelectListItem { Text = "3/64 in", Value = "3/64 in" });
                //EsquinasSuaje.Add(new SelectListItem { Text = "3/32 in", Value = "3/32 in" });
                //EsquinasSuaje.Add(new SelectListItem { Text = "5/8 in", Value = "5/8 in" });
                //EsquinasSuaje.Add(new SelectListItem { Text = "5/64 in", Value = "5/64 in" });
                //EsquinasSuaje.Add(new SelectListItem { Text = "7/16 in", Value = "7/16 in" });
                //EsquinasSuaje.Add(new SelectListItem { Text = "7/32 in", Value = "7/32 in" });
                //EsquinasSuaje.Add(new SelectListItem { Text = "7/64 in", Value = "7/64 in" });
                //EsquinasSuaje.Add(new SelectListItem { Text = "9/16 in", Value = "9/16 in" });
                //EsquinasSuaje.Add(new SelectListItem { Text = "9/32 in", Value = "9/32 in" });
                //EsquinasSuaje.Add(new SelectListItem { Text = "9/64 in", Value = "9/64 in" });
                //EsquinasSuaje.Add(new SelectListItem { Text = "11/16 in", Value = "11/16 in" });
                //EsquinasSuaje.Add(new SelectListItem { Text = "11/32 in", Value = "11/32 in" });
                //EsquinasSuaje.Add(new SelectListItem { Text = "11/64 in", Value = "11/64 in" });

                ViewBag.EsquinaSuaje = new SelectList(EsquinasSuaje, "Value", "Text", elemento.Esquinas);

                ViewBag.ACABADO = new SelectList(AcabadoL, "Value", "Text", elemento.ACABADO);

            }


            return View(elemento);
        }

        [HttpPost]
        public ActionResult SuajeNuevo(ClsCotizador elemento, FormCollection colecciondeelementos)
        {
            elemento = verificaprecios(elemento);
            Paramsuajes parametrossuajes = new ParamsuajesContext().Paramsuajes.FirstOrDefault();
            ViewBag.Paramsuajes = parametrossuajes;

            ViewBag.IDCentro = new Repository().GetCentros(elemento.IDCentro);
            ViewBag.IDCaja = new Repository().GetCajas(elemento.IDCaja);
            elemento = verificaprecios(elemento);
            elemento.SuajeNuevo = true;
            string tsuaje = colecciondeelementos.Get("Tsuaje");
            elemento.TipoSuaje = tsuaje.Replace(",", "");

            string tsuajeFig = colecciondeelementos.Get("TSuajeFi");
            elemento.TipoSuajeFigura = tsuajeFig.Replace(",", "");

            string tsuajeCorte = colecciondeelementos.Get("TSuajeCorte");
            elemento.TipoCorte = tsuajeCorte.Replace(",", "");
            string esquinasC = colecciondeelementos.Get("EsquinaSuaje");
            elemento.Esquinas = esquinasC.Replace(",", "");
            //esquinasC = colecciondeelementos.Get("Esquinas");


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
            TipoFigura.Add(new SelectListItem { Text = "Irregular", Value = "I" });
            TipoFigura.Add(new SelectListItem { Text = "Muesca General", Value = "Muesca General" });
            TipoFigura.Add(new SelectListItem { Text = "Muesca Especial", Value = "Muesca Especial" });
            TipoFigura.Add(new SelectListItem { Text = "Corte Recto", Value = "Corte Recto" });
            TipoFigura.Add(new SelectListItem { Text = "Figura Especial", Value = "Figura Especial" });

            ViewBag.TSuajeFi = new SelectList(TipoFigura, "Value", "Text", elemento.TipoSuajeFigura);


            var Almas = new List<SelectListItem>();
            Almas.Add(new SelectListItem { Text = "---Seleccionar---", Value = "0" });

            Almas.Add(new SelectListItem { Text = "88", Value = "88" });
            Almas.Add(new SelectListItem { Text = "98", Value = "98" });
            Almas.Add(new SelectListItem { Text = "104", Value = "104" });
            Almas.Add(new SelectListItem { Text = "108", Value = "108" });
            Almas.Add(new SelectListItem { Text = "116", Value = "116" });
            Almas.Add(new SelectListItem { Text = "122", Value = "122" });
            Almas.Add(new SelectListItem { Text = "126", Value = "126" });

            ViewBag.Almas = new SelectList(Almas, "Value", "Text", elemento.TH);

            List<Articulo> articulos = new List<Articulo>();
            string cadena = "select * from Articulo where descripcion like '%BARNIZ%'";
            articulos = db.Database.SqlQuery<Articulo>(cadena).ToList();
            List<SelectListItem> listaArticulo = new List<SelectListItem>();
            listaArticulo.Add(new SelectListItem { Text = "--Todos los artículos--", Value = "0" });

            foreach (var m in articulos)
            {
                listaArticulo.Add(new SelectListItem { Text = m.Descripcion, Value = m.IDArticulo.ToString() });
            }
            //ViewBag.ACABADOL = listaArticulo;

            var AcabadoL = new List<SelectListItem>();
            AcabadoL.Add(new SelectListItem { Text = "---Seleccionar---", Value = "N/A" });

            AcabadoL.Add(new SelectListItem { Text = "LAMINADO BRILLANTE", Value = "LAMINADO BRILLANTE" });
            AcabadoL.Add(new SelectListItem { Text = "LAMINADO MATE", Value = "LAMINADO MATE" });
            AcabadoL.Add(new SelectListItem { Text = "BARNIZ BRILLANTE", Value = "BARNIZ BRILLANTE" });
            AcabadoL.Add(new SelectListItem { Text = "BARNIZ MATE", Value = "BARNIZ MATE" });
            AcabadoL.Add(new SelectListItem { Text = "BARNIZ RELEASE", Value = "BARNIZ RELEASE" });
            AcabadoL.Add(new SelectListItem { Text = "FOIL", Value = "FOIL" });
            //AcabadoL.Add(new SelectListItem { Text = "CAST&CURE", Value = "CAST&CURE" });



            ViewBag.ACABADOL = new SelectList(AcabadoL, "Value", "Text", elemento.ACABADO);

            ViewData["ACABADO"] = ViewBag.ACABADOL;





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

            ViewBag.EsquinaSuaje = new SelectList(EsquinasSuaje, "Value", "Text", elemento.Esquinas);
            ViewData["EsquinaSuaje"] = EsquinasSuaje;

            if (elemento.TCcotizado == 0)

            {
                elemento.TCcotizado = SIAAPI.Properties.Settings.Default.TCcotizador;
            }
            ViewBag.IDCotizacion = elemento.IDCotizacion;





            FormulaEspecializada.Formulaespecializada formula = new FormulaEspecializada.Formulaespecializada();
            formula = igualar(elemento, formula);


            try
            {
                string cm = colecciondeelementos.Get("Monedafinal");
                elemento.IDMonedapreciosconvenidos = new c_MonedaContext().c_Monedas.Where(s => s.ClaveMoneda == cm).ToList().FirstOrDefault().IDMoneda;
            }
            catch (Exception err)
            {
                string menjsaje1 = err.Message;
            }
            
            ///////////////////////////// suajes ///////////////////
            ViewBag.EnqueEstoy = 1;

            var suajes = new Repository().GetSuajes(elemento.IDSuaje);

            ViewBag.IDSuaje = suajes;

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


            /////////////////////////////Cintas ///////////////////
            var cintas = new Repository().GetCintasbyClave(elemento.IDMaterial);

            if (elemento.IDMaterial != 0)
            {
                try
                {
                    Materiales mat = new MaterialesContext().Materiales.Find(elemento.IDMaterial);
                    if (mat.Fam == "NYL")
                    {
                        elemento.Materialnecesitarefile = false;
                        formula.Materialnecesitarefile = false;
                    }
                    else
                    {
                        elemento.Materialnecesitarefile = true;
                        formula.Materialnecesitarefile = false;
                    }
                }
                catch (Exception err)
                {
                    string mensajeerror = err.Message;
                }
            }

            ViewBag.IDMaterial = cintas;

            ViewBag.IDMAterial2 = new Repository().GetCintasbyClave(elemento.IDMaterial2);
            ViewBag.IDMAterial3 = new Repository().GetCintasbyClave(elemento.IDMaterial3);
            ///////////////////// Tintas //////////////////////////////////////


            var tintas = Gettintasiniciales(formula.Numerodetintas, colecciondeelementos, elemento.CantidadMPMts2);

            //List<Tinta> tintasA = new List<Tinta>();
            //if (elemento.ACABADO == "BARNIZ BRILLANTE")
            //{
            //    Tinta nueva = new Tinta();
            //    nueva.IDTinta = (5536);
            //    nueva.Area = (100);
            //    tintas.Add(nueva);
            //}
            //if (elemento.ACABADO == "BARNIZ MATE")
            //{
            //    Tinta nueva = new Tinta();
            //    nueva.IDTinta = (5182);
            //    nueva.Area = (100);
            //    tintas.Add(nueva);
            //}
            if (elemento.ACABADO == "BARNIZ BRILLANTE")
            {
                try
                {
                    int num = formula.Numerodetintas + 1;
                    //int idartciuloacabado = int.Parse(colecciondeelementos.Get("Tinta1"));

                    //Articulo art = new ArticuloContext().Articulo.Find(idartciuloacabado);

                    //if (art.Descripcion.Contains("BARNIZ"))
                    //{
                    Tinta nueva = new Tinta();

                    string tintaacabado = colecciondeelementos.Get("Tinta" + num);
                    if (tintaacabado != "5536")
                    {
                        nueva.IDTinta = int.Parse(tintaacabado);
                    }
                    else
                    {
                        nueva.IDTinta = (5536);
                    }



                    //if (formula.Numerodetintas == 1)
                    //{
                    //    num = num - 1;
                    //}
                    try
                    {
                        decimal areaacabado = decimal.Parse(colecciondeelementos.Get("Area" + num).ToString());
                        if (areaacabado != 100)
                        {
                            nueva.Area = areaacabado;
                        }
                        else
                        {
                            nueva.Area = (100);
                        }

                    }
                    catch (Exception err)
                    {
                        nueva.Area = (100);
                    }

                    tintas.Add(nueva);

                    //}


                }
                catch (Exception err)
                {
                    Tinta nueva = new Tinta();
                    nueva.IDTinta = (5536);
                    nueva.Area = (100);
                    tintas.Add(nueva);
                }

            }
            if (elemento.ACABADO == "BARNIZ MATE")
            {
                Tinta nueva = new Tinta();
                nueva.IDTinta = (5182);
                int num = formula.Numerodetintas + 1;
                //if (formula.Numerodetintas == 1)
                //{
                //    num = num - 1;
                //}
                try
                {
                    decimal areaacabado = int.Parse(colecciondeelementos.Get("Area" + num));
                    if (areaacabado != 100)
                    {
                        nueva.Area = areaacabado;
                    }
                    else
                    {
                        nueva.Area = (100);
                    }

                }
                catch (Exception err)
                {
                    nueva.Area = (100);
                }
                tintas.Add(nueva);
            }
            ViewData["Tintas"] = null;
            try
            {
                ViewData["Tintas"] = tintas;
            }
            catch (Exception err)
            {
                string mensaje = err.Message;
            }

            formula = pasaTintas(tintas, formula); // lo paso para calcular
            elemento = pasaTintas(tintas, elemento); // lo paso al formulario web para generar archivo

            formula.CobrarMaster = elemento.CobrarMaster;
            //////////////////////////  Calcular //////////////////////
            if (elemento.IDMaterial2 != 0)
            {
                try
                {
                    Materiales mat2 = new MaterialesContext().Materiales.Find(elemento.IDMaterial2);
                    if (mat2.Completo)
                    {
                        formula.CobrarMaster2m = true;
                        formula.anchomaster2m = mat2.Ancho;
                        formula.largomaster2m = mat2.Largo;


                    }
                }
                catch (Exception err)
                {
                    string mensajeerror = err.Message;
                }
            }


            formula.Calcular();

            formula = CosteaTintas(formula);

            decimal mo = formula.getHoraPrensa();

            decimal moe = 0;

            if (formula.Cantidadxrollo > 0)
            {

                moe = formula.getHoraEmbobinado();
            }

            elemento.Costo1mxn = elemento.Costo1 * SIAAPI.Properties.Settings.Default.TCcotizador;
            elemento.Costo2mxn = elemento.Costo2 * SIAAPI.Properties.Settings.Default.TCcotizador;
            elemento.Costo3mxn = elemento.Costo3 * SIAAPI.Properties.Settings.Default.TCcotizador;
            elemento.Costo4mxn = elemento.Costo4 * SIAAPI.Properties.Settings.Default.TCcotizador;

            elemento.MaterialNecesitado = formula.MaterialNecesitado;
            elemento.Minimoproducir = formula.Minimoproducir;

            elemento.Numerodecintas = formula.Numerodecintas;




            //if (elemento.Cantidad < elemento.Minimoproducir && elemento.Cantidad > 0)
            //{
            //    elemento.Cantidad = elemento.Minimoproducir;
            //}
            elemento.CantidadMPMts2 = formula.CantidadMPMts2;
            elemento.anchomaterialenmm = formula.anchomaterialenmm;
            elemento.largomaterialenMts = formula.largomaterialenMts;
            elemento.CintasMaster = formula.CintasMaster;
            elemento.Numerodecintas = formula.Numerodecintas;
            elemento.MtsdeMerma = formula.MtsdeMerma;
            elemento.CostototalMP = formula.CostototalMP;
            elemento.CintasMaster = elemento.anchommmaster / elemento.anchomaterialenmm;


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


            ViewBag.EnqueEstoy = 1;

            ViewBag.Mensajedeerror = "";
            ViewBag.Minino = Math.Round(elemento.Minimoproducir, 2);

            bool pasa = true;

            bool pasafinal = true;


            ////////th puede ser flexible o mazico
            ///

            if (elemento.TipoSuaje == "F" && !(elemento.TH == 88 || elemento.TH == 98 || elemento.TH == 104 || elemento.TH == 108 || elemento.TH == 116 || elemento.TH == 122 || elemento.TH == 126))
            {
                ViewBag.Mensajedeerror += "La cantidad de dientes no corresponde al tipo de suaje Flexible \n";
                pasa = false;
            }

            //if ((elemento.cavidadesdesuajeEje % elemento.productosalpaso) != 0)
            //{

            //    ViewBag.Mensajedeerror += "La cantidad de etiquetas al paso no es un multiplo de las cavidades del suaje\n";
            //    pasa = false;
            //}

            if (elemento.Cantidad == 0)
            {

                ViewBag.Mensajedeerror += "No hay datos suficientes para calcular un precio, Comienza por dar  una cantidad a producir\n";
                pasa = false;
            }


            if (elemento.Minimoproducir == 0)
            {
                ViewBag.Mensajedeerror += "No hay datos suficientes para calcular un precio, Comienza por dar las medidas de la etiqueta y una cantidad a producir\n";
                pasa = false;
            }

            if (elemento.Cantidadxrollo == 0)
            {
                ViewBag.Mensajedeerror += "No has elegido la cantidad por rollo o paquete \n";
                pasa = false;
            }


            if (elemento.IDMaterial == 0)
            {
                ViewBag.Mensajedeerror += "\nNo has seleccionado una cinta con la cual trabajar";
                pasa = false;
            }
            if ((elemento.IDMaterial > 0) && (elemento.CostoM2Cinta == 0))
            {
                ViewBag.Mensajedeerror += "\nLa Cinta elegida no contiene un costo asignado, registra un costo o escribe manualmente el costo pretendido en la cinta\n";
                pasa = false;
            }
            if ((elemento.IDMaterial2 > 0) && (elemento.CostoM2Cinta2 == 0))
            {
                ViewBag.Mensajedeerror += "\nLa Cinta Adicional elegida no contiene un costo asignado, registra un costo o escribe manualmente el costo pretendido en la cinta Adicional\n";
                pasa = false;
            }

            if ((elemento.IDMaterial3 > 0) && (elemento.CostoM2Cinta3 == 0))
            {
                ViewBag.Mensajedeerror += "\nLa Cinta Adicional elegida no contiene un costo asignado, registra un costo o escribe manualmente el costo pretendido en la cinta Adicional\n";
                pasa = false;
            }

            if (verificatintas(elemento) == false)
            {
                ViewBag.Mensajedeerror += "\nindicaste que tenias tintas pero al menos una de ellas no seleccionaste que tinta\n";
                pasa = false;
            }

            try
            {
                if (colecciondeelementos.Get("Rango1").ToString() == "" || colecciondeelementos.Get("Rango1").ToString() == "0")
                {
                    throw new Exception("El Rango 1 es null o 0");
                }
                elemento.Rango1 = decimal.Parse(colecciondeelementos.Get("Rango1").ToString());
                if (elemento.Rango1 < 0)
                {
                    throw new Exception("El Rango 1 es menor a 0");
                }
                ViewBag.Rango1 = elemento.Rango1;
            }

            catch (Exception err)
            {
                string mensajedeerror = err.Message;
                pasafinal = false;
            }

            try
            {
                if (colecciondeelementos.Get("Rango2").ToString() == "" || colecciondeelementos.Get("Rango2").ToString() == "0")
                {
                    throw new Exception("El Rango 2 es null o 0");
                }
                elemento.Rango2 = decimal.Parse(colecciondeelementos.Get("Rango2").ToString());
                if (elemento.Rango1 < 0)
                {
                    throw new Exception("El Rango 2 es menor a 0");
                }
                ViewBag.Rango2 = elemento.Rango2;
            }
            catch (Exception err)
            {
                string mensajedeerror = err.Message;
                pasafinal = false;
            }


            try
            {
                if (colecciondeelementos.Get("Rango3").ToString() == "" || colecciondeelementos.Get("Rango3").ToString() == "0")
                {
                    throw new Exception("El Rango 3 es null o 0");
                }
                elemento.Rango3 = decimal.Parse(colecciondeelementos.Get("Rango3").ToString());
                if (elemento.Rango1 < 0)
                {
                    throw new Exception("El Rango 3 es menor a 0");
                }
                ViewBag.Rango3 = elemento.Rango3;
            }
            catch (Exception err)
            {
                string mensajedeerror = err.Message;
                pasafinal = false;
            }

            try
            {
                if (colecciondeelementos.Get("Rango4").ToString() == "" || colecciondeelementos.Get("Rango4").ToString() == "0")
                {
                    throw new Exception("El Rango 4 es null o 0");
                }
                elemento.Rango4 = decimal.Parse(colecciondeelementos.Get("Rango4").ToString());
                if (elemento.Rango1 < 0)
                {
                    throw new Exception("El Rango 4 es menor a 0");
                }
                ViewBag.Rango4 = elemento.Rango4;
            }
            catch (Exception err)
            {
                string mensajedeerror = err.Message;
                pasafinal = false;
            }


            if (elemento.Rango1 == 0 && elemento.Rango2 == 0)
            {
                if (elemento.Cantidad < formula.Minimoproducir)
                {
                    elemento.Rango1 = elemento.Cantidad;
                    elemento.Rango2 = Math.Round(elemento.Minimoproducir, 1);
                }
                if (elemento.Cantidad >= formula.Minimoproducir)
                {
                    elemento.Rango1 = Math.Round(elemento.Minimoproducir, 1);
                    elemento.Rango2 = elemento.Cantidad;
                }
            }



            if (mo == 0 || moe == 0)
            {
                pasafinal = false;
            }

            if (pasa && pasafinal)
            {
                decimal costosuaje = new ArticuloContext().Database.SqlQuery<decimal>("select dbo.GetCosto(0," + elemento.IDSuaje + ",1)").ToList().FirstOrDefault();
                if (elemento.DiluirSuajeEnPedidos == 0)
                { elemento.DiluirSuajeEnPedidos = 50; }
                costosuaje = costosuaje / elemento.DiluirSuajeEnPedidos;

                if (costosuaje < 12)
                {
                    costosuaje = 12;
                }


                // suma del costo del suaje diluido + costo materias primas + costo de tintas + costo de prensa + costo de embobinado 



                FormulaEspecializada.Formulaespecializada Formulapara1 = new FormulaEspecializada.Formulaespecializada();
                Formulapara1 = igualar(elemento, Formulapara1);
                Formulapara1.Cantidad = decimal.Parse(colecciondeelementos.Get("Rango1"));
                Formulapara1 = pasaTintas(tintas, Formulapara1);
                Formulapara1.CobrarMaster = elemento.CobrarMaster;

                if (elemento.IDMaterial2 != 0)
                {
                    try
                    {
                        Materiales mat2 = new MaterialesContext().Materiales.Find(elemento.IDMaterial2);
                        if (mat2.Completo)
                        {
                            Formulapara1.CobrarMaster2m = true;
                            Formulapara1.anchomaster2m = mat2.Ancho;
                            Formulapara1.largomaster2m = mat2.Largo;


                        }
                    }
                    catch (Exception err)
                    {
                        string mensajeerror = err.Message;
                    }
                }

                Formulapara1.Calcular();
                Formulapara1 = CosteaTintas(Formulapara1);
                Formulapara1.calcularCostoMO();


                decimal costototal = (costosuaje) + Formulapara1.CostototalMP + Formulapara1.Costodetintas + Formulapara1.CostototalMO + SIAAPI.Properties.Settings.Default.costodeempaque;

                decimal costoxmillar = costototal / Formulapara1.Cantidad;
                elemento.Costo1 = costoxmillar;

                elemento.MatrizPrecio.Fila1.Celda1.Valor = Math.Round((costoxmillar * (1 / ((100 - Formulapara1.Rango1gain) / 100))), 2);
                elemento.MatrizPrecio.Fila1.Celda2.Valor = Math.Round((costoxmillar * (1 / ((100 - Formulapara1.Rango2gain) / 100))), 2);
                elemento.MatrizPrecio.Fila1.Celda3.Valor = Math.Round((costoxmillar * (1 / ((100 - Formulapara1.Rango3gain) / 100))), 2);
                elemento.MatrizPrecio.Fila1.Celda4.Valor = Math.Round((costoxmillar * (1 / ((100 - Formulapara1.Rango4gain) / 100))), 2);


                FormulaEspecializada.Formulaespecializada Formulapara2 = new FormulaEspecializada.Formulaespecializada();
                Formulapara2 = igualar(elemento, Formulapara2);
                Formulapara2.Cantidad = decimal.Parse(colecciondeelementos.Get("Rango2"));
                Formulapara2 = pasaTintas(tintas, Formulapara2);
                Formulapara2.CobrarMaster = elemento.CobrarMaster;
                if (elemento.IDMaterial2 != 0)
                {
                    try
                    {
                        Materiales mat2 = new MaterialesContext().Materiales.Find(elemento.IDMaterial2);
                        if (mat2.Completo)
                        {
                            Formulapara2.CobrarMaster2m = true;
                            Formulapara2.anchomaster2m = mat2.Ancho;
                            Formulapara2.largomaster2m = mat2.Largo;


                        }
                    }
                    catch (Exception err)
                    {
                        string mensajeerror = err.Message;
                    }
                }

                Formulapara2.Calcular();
                Formulapara2 = CosteaTintas(Formulapara2);
                Formulapara2.calcularCostoMO();

                costototal = (costosuaje) + Formulapara2.CostototalMP + Formulapara2.Costodetintas + Formulapara2.CostototalMO + SIAAPI.Properties.Settings.Default.costodeempaque;

                costoxmillar = costototal / Formulapara2.Cantidad;

                elemento.Costo2 = costoxmillar;

                elemento.MatrizPrecio.Fila2.Celda1.Valor = Math.Round((costoxmillar * (1 / ((100 - Formulapara2.Rango1gain) / 100))), 2);
                elemento.MatrizPrecio.Fila2.Celda2.Valor = Math.Round((costoxmillar * (1 / ((100 - Formulapara2.Rango2gain) / 100))), 2);
                elemento.MatrizPrecio.Fila2.Celda3.Valor = Math.Round((costoxmillar * (1 / ((100 - Formulapara2.Rango3gain) / 100))), 2);
                elemento.MatrizPrecio.Fila2.Celda4.Valor = Math.Round((costoxmillar * (1 / ((100 - Formulapara2.Rango4gain) / 100))), 2);


                FormulaEspecializada.Formulaespecializada Formulapara3 = new FormulaEspecializada.Formulaespecializada();
                Formulapara3 = igualar(elemento, Formulapara3);
                Formulapara3.Cantidad = decimal.Parse(colecciondeelementos.Get("Rango3"));
                Formulapara3 = pasaTintas(tintas, Formulapara3);
                Formulapara3.CobrarMaster = elemento.CobrarMaster;
                if (elemento.IDMaterial2 != 0)
                {
                    try
                    {
                        Materiales mat2 = new MaterialesContext().Materiales.Find(elemento.IDMaterial2);
                        if (mat2.Completo)
                        {
                            Formulapara3.CobrarMaster2m = true;
                            Formulapara3.anchomaster2m = mat2.Ancho;
                            Formulapara3.largomaster2m = mat2.Largo;


                        }
                    }
                    catch (Exception err)
                    {
                        string mensajeerror = err.Message;
                    }
                }

                Formulapara3.Calcular();
                Formulapara3 = CosteaTintas(Formulapara3);
                Formulapara3.calcularCostoMO();

                costototal = (costosuaje) + Formulapara3.CostototalMP + Formulapara3.Costodetintas + Formulapara3.CostototalMO + SIAAPI.Properties.Settings.Default.costodeempaque;

                costoxmillar = costototal / Formulapara3.Cantidad;

                elemento.Costo3 = costoxmillar;

                elemento.MatrizPrecio.Fila3.Celda1.Valor = Math.Round((costoxmillar * (1 / ((100 - Formulapara3.Rango1gain) / 100))), 2);
                elemento.MatrizPrecio.Fila3.Celda2.Valor = Math.Round((costoxmillar * (1 / ((100 - Formulapara3.Rango2gain) / 100))), 2);
                elemento.MatrizPrecio.Fila3.Celda3.Valor = Math.Round((costoxmillar * (1 / ((100 - Formulapara3.Rango3gain) / 100))), 2);
                elemento.MatrizPrecio.Fila3.Celda4.Valor = Math.Round((costoxmillar * (1 / ((100 - Formulapara3.Rango4gain) / 100))), 2);

                FormulaEspecializada.Formulaespecializada Formulapara4 = new FormulaEspecializada.Formulaespecializada();
                Formulapara4 = igualar(elemento, Formulapara4);
                Formulapara4.Cantidad = decimal.Parse(colecciondeelementos.Get("Rango4"));
                Formulapara4 = pasaTintas(tintas, Formulapara4);
                Formulapara4.CobrarMaster = elemento.CobrarMaster;
                if (elemento.IDMaterial2 != 0)
                {
                    try
                    {
                        Materiales mat2 = new MaterialesContext().Materiales.Find(elemento.IDMaterial2);
                        if (mat2.Completo)
                        {
                            Formulapara4.CobrarMaster2m = true;
                            Formulapara4.anchomaster2m = mat2.Ancho;
                            Formulapara4.largomaster2m = mat2.Largo;


                        }
                    }
                    catch (Exception err)
                    {
                        string mensajeerror = err.Message;
                    }
                }

                Formulapara4.Calcular();
                Formulapara4 = CosteaTintas(Formulapara4);
                Formulapara4.calcularCostoMO();

                costototal = (costosuaje) + Formulapara4.CostototalMP + Formulapara4.Costodetintas + Formulapara4.CostototalMO + SIAAPI.Properties.Settings.Default.costodeempaque;

                costoxmillar = costototal / Formulapara4.Cantidad;

                elemento.Costo4 = costoxmillar;

                elemento.MatrizPrecio.Fila4.Celda1.Valor = Math.Round((costoxmillar * (1 / ((100 - Formulapara4.Rango1gain) / 100))), 2);
                elemento.MatrizPrecio.Fila4.Celda2.Valor = Math.Round((costoxmillar * (1 / ((100 - Formulapara4.Rango2gain) / 100))), 2);
                elemento.MatrizPrecio.Fila4.Celda3.Valor = Math.Round((costoxmillar * (1 / ((100 - Formulapara4.Rango3gain) / 100))), 2);
                elemento.MatrizPrecio.Fila4.Celda4.Valor = Math.Round((costoxmillar * (1 / ((100 - Formulapara4.Rango4gain) / 100))), 2);

                elemento.Yatienematriz = true;

                try
                {
                    elemento.precioconvenidos.precio1 = 0M/*decimal.Parse(colecciondeelementos.Get("precioconvenido1"))*/;
                                                      
                    elemento.precioconvenidos.precio2 = 0M/*decimal.Parse(colecciondeelementos.Get("precioconvenido2"))*/;
                    elemento.precioconvenidos.precio3 = 0M/*decimal.Parse(colecciondeelementos.Get("precioconvenido3"))*/;
                    elemento.precioconvenidos.precio4 = 0M/*decimal.Parse(colecciondeelementos.Get("precioconvenido4"))*/;
                }
                catch (Exception err)
                {

                }


                if (elemento.precioconvenidos.precio1 == 0)
                {
                    elemento.precioconvenidos.precio1 = elemento.MatrizPrecio.Fila1.Celda1.Valor;
                }
                if (elemento.precioconvenidos.precio2 == 0)
                {
                    elemento.precioconvenidos.precio2 = elemento.MatrizPrecio.Fila2.Celda1.Valor;
                }
                if (elemento.precioconvenidos.precio3 == 0)
                {
                    elemento.precioconvenidos.precio3 = elemento.MatrizPrecio.Fila3.Celda1.Valor;
                }
                if (elemento.precioconvenidos.precio4 == 0)
                {
                    elemento.precioconvenidos.precio4 = elemento.MatrizPrecio.Fila4.Celda1.Valor;
                }


                if (colecciondeelementos.Get("Enviar") == "Sobreescribir")
                {
                    Cotizaciones archivocotizacion = new ArchivoCotizadorContext().cotizaciones.Find(elemento.IDCotizacion);
                    XmlDocument documento = new XmlDocument();
                    string nombredearchivo = archivocotizacion.Ruta.TrimEnd() + "\\" + archivocotizacion.NombreArchivo + ".xml";


                    StringWriter stringwriter = new StringWriter();
                    XmlSerializer x = new XmlSerializer(elemento.GetType());
                    x.Serialize(stringwriter, elemento);

                    string xmlstring = stringwriter.ToString().Replace("encoding=\"utf-16\"", "");


                    EscribeArchivoXMLC(xmlstring, nombredearchivo, true);
                }


                if (colecciondeelementos.Get("Enviar") == "Grabar Archivo")
                {
                    string NombredeArchivo = DateTime.Now.ToString().Replace(".", "").Replace(",", "").Replace(" ", "").Replace("/", "").Replace(":", "");

                    string nombredecarpeta = DateTime.Now.Month + "" + DateTime.Now.Year + " " + User.Identity.Name;

                    if (!Directory.Exists(System.Web.HttpContext.Current.Server.MapPath("~/Cotizaciones")))
                    {
                        Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath("~/Cotizaciones"));
                    }
                    if (!Directory.Exists(System.Web.HttpContext.Current.Server.MapPath("~/Cotizaciones/" + User.Identity.Name)))
                    {
                        Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath("~/Cotizaciones" + User.Identity.Name));
                    }
                    if (!Directory.Exists(System.Web.HttpContext.Current.Server.MapPath("~/Cotizaciones/" + User.Identity.Name + "/" + nombredecarpeta)))
                    {
                        Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath("~/Cotizaciones/" + User.Identity.Name + "/" + nombredecarpeta));
                    }


                    nombredecarpeta = System.Web.HttpContext.Current.Server.MapPath("~/Cotizaciones/" + User.Identity.Name + "/" + nombredecarpeta);


                    this.GrabarArchivoCotizador(elemento, NombredeArchivo, nombredecarpeta);

                    return RedirectToAction("ArchivoCotizadorN", new { _nombredearchivo = NombredeArchivo, _ruta = nombredecarpeta, suajeN = 1 });

                }
                elemento.IDCotizacion = ViewBag.IDCotizacion;

                if (colecciondeelementos.Get("Enviar") == "Crear Articulo" && elemento.IDCotizacion > 0) //llego hasta crear articulo
                {


                    return RedirectToAction("CreaArticulo", new { id = elemento.IDCotizacion });


                }
                if (colecciondeelementos.Get("Enviar") == "Asignar Articulo" && elemento.IDCotizacion > 0) //llego hasta crear articulo
                {


                    return RedirectToAction("Asignarunarticulo", new { id = elemento.IDCotizacion });


                }
                if (colecciondeelementos.Get("Enviar") == "Crear PDF" && elemento.IDCotizacion > 0) //llego hasta crear articulo
                {


                    return RedirectToAction("CrearCotizacionPDF", new { IDCotizacion = elemento.IDCotizacion });


                }
            }



            return View(elemento);

        }
        public ClsCotizador llenaelemento(ClsCotizador elemento, int IDSuaje)
        {
            Caracteristica cara = new ArticuloContext().Database.SqlQuery<Caracteristica>("Select * from Caracteristica where ID=" + IDSuaje).ToList().FirstOrDefault();
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
            elemento.largoproductomm = suajec.Avance;



            suajec.CavidadAvance = 2;

            try
            {
                suajec.CavidadAvance = int.Parse(formula.getvalor("REP AVANCE", cara.Presentacion).ToString());
                if (suajec.CavidadAvance == 0)
                {
                    suajec.CavidadAvance = int.Parse(formula.getvalor("REPETICIONES AVANCE", cara.Presentacion).ToString());
                    if (suajec.CavidadAvance == 0)
                    {
                        suajec.CavidadAvance = int.Parse(formula.getvalor("REPETICIONES AL AVANCE", cara.Presentacion).ToString());
                        if (suajec.CavidadAvance == 0)
                        {
                            suajec.CavidadAvance = int.Parse(formula.getvalor("CAV AVA", cara.Presentacion).ToString());
                            if (suajec.CavidadAvance == 0)
                            {
                                suajec.CavidadAvance = int.Parse(formula.getvalor("CAV AVANCE", cara.Presentacion).ToString());
                                if (suajec.CavidadAvance == 0)
                                {
                                    suajec.CavidadAvance = int.Parse(formula.getvalor("CAVIDADES AVANCE", cara.Presentacion).ToString());
                                    if (suajec.CavidadAvance == 0)
                                    {
                                        suajec.CavidadAvance = int.Parse(formula.getvalor("CAVIDADES AL AVANCE", cara.Presentacion).ToString());

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
                suajec.CavidadAvance = 2;
            }

            elemento.cavidadesdesuajeAvance = suajec.CavidadAvance;


            suajec.CavidadEje = 2;

            try
            {
                suajec.CavidadEje = int.Parse(formula.getvalor("REP EJE", cara.Presentacion).ToString());
                if (suajec.CavidadEje == 0)
                {
                    suajec.CavidadEje = int.Parse(formula.getvalor("REPETICIONES EJE", cara.Presentacion).ToString());
                    if (suajec.CavidadEje == 0)
                    {
                        suajec.CavidadEje = int.Parse(formula.getvalor("REPETICIONES AL EJE", cara.Presentacion).ToString());
                        if (suajec.CavidadEje == 0)
                        {
                            suajec.CavidadEje = int.Parse(formula.getvalor("CAV EJE", cara.Presentacion).ToString());
                            if (suajec.CavidadEje == 0)
                            {
                                suajec.CavidadEje = int.Parse(formula.getvalor("CAVIDADES EJE", cara.Presentacion).ToString());
                                if (suajec.CavidadEje == 0)
                                {
                                    suajec.CavidadEje = int.Parse(formula.getvalor("CAVIDADES AL EJE", cara.Presentacion).ToString());

                                }
                            }
                        }
                    }
                }

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
                    suajec.Gapeje = decimal.Parse(formula.getvalor("GAP AL EJE", cara.Presentacion).ToString());
                   
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
                    if (suajec.TH == 0)
                    {
                        suajec.TH = int.Parse(formula.getValorCadena("DIENTES", cara.Presentacion).ToString());
                        if (suajec.TH == 0)
                        {
                            suajec.TH = int.Parse(formula.getValorCadena("DIENTES_TH", cara.Presentacion).ToString());
                            if (suajec.TH == 0)
                            {
                                suajec.TH = int.Parse(formula.getValorCadena("NO DE DIENTES", cara.Presentacion).ToString());
                                if (suajec.TH == 0)
                                {
                                    suajec.TH = int.Parse(formula.getValorCadena("NUMERO DE DIENTES", cara.Presentacion).ToString());
                                    if (suajec.TH == 0)
                                    {
                                        suajec.TH = int.Parse(formula.getValorCadena("No DIENTES", cara.Presentacion).ToString());
                                    }
                                }
                            }
                        }
                    }
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

                    string alma= suajec.Alma.Replace(" TH", "");

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

        public JsonResult getarticulosblando(string buscar)
        {
          
            var Articulos = new MaterialesContext().Materiales.Where(s => s.Obsoleto==false && (s.Descripcion.Contains(buscar) || s.Clave.Contains(buscar))).OrderBy(S => S.Clave);

            // Populate DropDownList
            List<SelectListItem> opciones = new List<SelectListItem>();

            foreach (Materiales art in Articulos)
            {
                SelectListItem elemento = new SelectListItem();
                elemento.Text = art.Clave + " " + art.Descripcion;
                elemento.Value = art.ID.ToString();
                opciones.Add(elemento);
            }

            return Json(opciones, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Sustituirprecios()
        {
            ViewBag.Caracteristica = null;
            ViewBag.Articulo = null;
            Articulo arti = null;
            Caracteristica carac = null;
            ArchivoCotizadorContext ac = new ArchivoCotizadorContext();
            if (Session["IDCaracteristica"] != null)
            {
                string idc = Session["IDCaracteristica"].ToString();
                carac = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from  caracteristica where ID=" + idc).FirstOrDefault();
               arti = new ArticuloContext().Articulo.Find(carac.Articulo_IDArticulo);
                ViewBag.Caracteristica = carac;
                ViewBag.Articulo = arti;
            }
            if (Session["IDCotizacion"] != null)
            {
                int idCotizacion = int.Parse(Session["IDCotizacion"].ToString());

                ac.Database.ExecuteSqlCommand("update Articulo set idCotizacion=" + idCotizacion + " where IDArticulo=" + arti.IDArticulo);

                ClsCotizador elemento = new ClsCotizador();
                Cotizaciones archivocotizacion = ac.cotizaciones.Find(idCotizacion);
                XmlDocument documento = new XmlDocument();
                string nombredearchivo = archivocotizacion.Ruta + "\\" + archivocotizacion.NombreArchivo + ".xml";
                documento.Load(nombredearchivo);
                elemento = null;
                XmlSerializer serializer = new XmlSerializer(typeof(ClsCotizador));
                using (Stream reader = new FileStream(nombredearchivo, FileMode.Open))
                {
                    // Call the Deserialize method to restore the object's state.
                    elemento = (ClsCotizador)serializer.Deserialize(reader);
                    reader.Close();
                }



                try
                {

                    db.Database.ExecuteSqlCommand("delete from MatrizPrecio  where IDArticulo=" + arti.IDArticulo);
                    db.Database.ExecuteSqlCommand("insert into MatrizPrecio ( IDArticulo, Ranginf, RangSup, Precio ) values (" + arti.IDArticulo + "," + elemento.Rango1 + "," + elemento.Rango2 + "," + elemento.precioconvenidos.precio1 + ")");
                    db.Database.ExecuteSqlCommand("insert into MatrizPrecio ( IDArticulo, Ranginf, RangSup, Precio ) values (" + arti.IDArticulo + "," + elemento.Rango2 + "," + elemento.Rango3 + "," + elemento.precioconvenidos.precio2 + ")");
                    db.Database.ExecuteSqlCommand("insert into MatrizPrecio ( IDArticulo, Ranginf, RangSup, Precio ) values (" + arti.IDArticulo + "," + elemento.Rango3 + "," + elemento.Rango4 + "," + elemento.precioconvenidos.precio3 + ")");
                    db.Database.ExecuteSqlCommand("insert into MatrizPrecio ( IDArticulo, Ranginf, RangSup, Precio ) values (" + arti.IDArticulo + "," + elemento.Rango4 + ",5000," + elemento.precioconvenidos.precio4 + ")");

                    return Json(new { success = true, responseText = "Cambiado" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception err)
                {
                    return Json(new { success = false, responseText = "Sin sustitucicion" }, JsonRequestBehavior.AllowGet);
                }
            }


            return Json(new { success = false, responseText = "Sin sustitucicion" }, JsonRequestBehavior.AllowGet);
        }

       

        public JsonResult Sustituircostos()
        {
            ViewBag.Caracteristica = null;
            ViewBag.Articulo = null;
            Articulo arti = null;
            Caracteristica carac = null;
            ArchivoCotizadorContext ac = new ArchivoCotizadorContext();

            if (Session["IDCaracteristica"] != null)
            {
                string idc = Session["IDCaracteristica"].ToString();
                carac = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from  caracteristica where ID=" + idc).FirstOrDefault();
                
                arti = new ArticuloContext().Articulo.Find(carac.Articulo_IDArticulo);
                ViewBag.Caracteristica = carac;
                ViewBag.Articulo = arti;
            }
            if (Session["IDCotizacion"] != null)
            {
                int idCotizacion = int.Parse(Session["IDCotizacion"].ToString());

                ac.Database.ExecuteSqlCommand("update Articulo set idCotizacion=" + idCotizacion + " where IDArticulo=" + arti.IDArticulo);


                ClsCotizador elemento = new ClsCotizador();
                Cotizaciones archivocotizacion = ac.cotizaciones.Find(idCotizacion);
                XmlDocument documento = new XmlDocument();
                string nombredearchivo = archivocotizacion.Ruta + "\\" + archivocotizacion.NombreArchivo + ".xml";
                documento.Load(nombredearchivo);
                elemento = null;
                XmlSerializer serializer = new XmlSerializer(typeof(ClsCotizador));
                using (Stream reader = new FileStream(nombredearchivo, FileMode.Open))
                {
                    // Call the Deserialize method to restore the object's state.
                    elemento = (ClsCotizador)serializer.Deserialize(reader);
                    reader.Close();
                }



                try
                {


                    db.Database.ExecuteSqlCommand("delete from MatrizCosto  where IDArticulo=" + carac.Articulo_IDArticulo);
                    db.Database.ExecuteSqlCommand("insert into MatrizCosto ( IDArticulo, Ranginf, RangSup, Precio ) values (" +  carac.Articulo_IDArticulo + "," + elemento.Rango1 + "," + elemento.Rango2 + "," + elemento.Costo1 + ")");
                    db.Database.ExecuteSqlCommand("insert into MatrizCosto ( IDArticulo, Ranginf, RangSup, Precio ) values (" +  carac.Articulo_IDArticulo + "," + elemento.Rango2 + "," + elemento.Rango3 + "," + elemento.Costo2 + ")");
                    db.Database.ExecuteSqlCommand("insert into MatrizCosto ( IDArticulo, Ranginf, RangSup, Precio ) values (" +  carac.Articulo_IDArticulo + "," + elemento.Rango3 + "," + elemento.Rango4 + "," + elemento.Costo3 + ")");
                    db.Database.ExecuteSqlCommand("insert into MatrizCosto ( IDArticulo, Ranginf, RangSup, Precio ) values (" + carac.Articulo_IDArticulo + "," + elemento.Rango4 + ",5000," + elemento.Costo4 + ")");


                    return Json(new { success = true, responseText = "Cambiado" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception err)
                {
                    return Json(new { success = false, responseText = "Sin sustitucicion" }, JsonRequestBehavior.AllowGet);
                }
            }


            return Json(new { success = false, responseText = "Sin sustitucicion" }, JsonRequestBehavior.AllowGet);
        }


        public JsonResult SustituirMatrizPrecioCliente(int IDCliente)
        {
            ViewBag.Caracteristica = null;
            ViewBag.Articulo = null;
            Articulo arti = null;
            Caracteristica carac = null;
            if (Session["IDCaracteristica"] != null)
            {
                string idc = Session["IDCaracteristica"].ToString();
                carac = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from  caracteristica where ID=" + idc).FirstOrDefault();
                arti = new ArticuloContext().Articulo.Find(carac.Articulo_IDArticulo);
                ViewBag.Caracteristica = carac;
                ViewBag.Articulo = arti;
            }


            if (Session["IDCotizacion"] != null)
            {
                int idCotizacion = int.Parse(Session["IDCotizacion"].ToString());
                ClsCotizador elemento = new ClsCotizador();
                Cotizaciones archivocotizacion = new ArchivoCotizadorContext().cotizaciones.Find(idCotizacion);
                XmlDocument documento = new XmlDocument();


                string nombredearchivo = archivocotizacion.Ruta + "\\" + archivocotizacion.NombreArchivo + ".xml";
                documento.Load(nombredearchivo);
                elemento = null;
                XmlSerializer serializer = new XmlSerializer(typeof(ClsCotizador));
                using (Stream reader = new FileStream(nombredearchivo, FileMode.Open))
                {
                    // Call the Deserialize method to restore the object's state.
                    elemento = (ClsCotizador)serializer.Deserialize(reader);
                    reader.Close();
                }



                try
                {


                    db.Database.ExecuteSqlCommand("delete from [MatrizPrecioCliente]  where IDArticulo=" + arti.IDArticulo + " and IDCliente=" + IDCliente);
                    db.Database.ExecuteSqlCommand("insert into MatrizPrecioCliente ( IDArticulo, Ranginf, RangSup, Precio,IDCliente ) values (" + arti.IDArticulo + "," + elemento.Rango1 + "," + elemento.Rango2 + "," + elemento.precioconvenidos.precio1 + "," + IDCliente + ")");
                    db.Database.ExecuteSqlCommand("insert into MatrizPrecioCliente ( IDArticulo, Ranginf, RangSup, Precio ,IDCliente) values (" + arti.IDArticulo + "," + elemento.Rango2 + "," + elemento.Rango3 + "," + elemento.precioconvenidos.precio2 + "," + IDCliente + ")");
                    db.Database.ExecuteSqlCommand("insert into MatrizPrecioCliente ( IDArticulo, Ranginf, RangSup, Precio,IDCliente ) values (" + arti.IDArticulo + "," + elemento.Rango3 + "," + elemento.Rango4 + "," + elemento.precioconvenidos.precio3 + "," + IDCliente + ")");
                    db.Database.ExecuteSqlCommand("insert into MatrizPrecioCliente ( IDArticulo, Ranginf, RangSup, Precio ,IDCliente) values (" + arti.IDArticulo + "," + elemento.Rango4 + ",5000," + elemento.precioconvenidos.precio4 + "," + IDCliente + ")");


                    return Json(new { success = true, responseText = "Cambiado" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception err)
                {
                    return Json(new { success = false, responseText = "Sin sustitucicion" }, JsonRequestBehavior.AllowGet);
                }
            }


            return Json(new { success = false, responseText = "Sin sustitucicion" }, JsonRequestBehavior.AllowGet);
        }


        public ClsCotizador verificaprecios(ClsCotizador elemento)
        {
            decimal precio1 = 0M;
            decimal precio2 = 0M;
            decimal precio3 = 0M;

            try
            {
                Materiales mat1 = new MaterialesContext().Materiales.Find(elemento.IDMaterial);
                if  (mat1.Precio==0)
                {
                    mat1.Precio = SIAAPI.Properties.Settings.Default.CostoM2default;
                }
                precio1 = mat1.Precio;
                elemento.CobrarMaster = mat1.Completo;
                elemento.anchommmaster = mat1.Ancho;
                elemento.LargoCinta = mat1.Largo;
            }
            catch (Exception err)
            {

            }
            try
            {
                Materiales mat2 = new MaterialesContext().Materiales.Find(elemento.IDMaterial2);
                if (mat2.Precio == 0)
                {
                    mat2.Precio = SIAAPI.Properties.Settings.Default.CostoM2default;
                }
                precio2 = mat2.Precio;
            }
            catch (Exception err)
            {

            }

            try
            {
                Materiales mat3 = new MaterialesContext().Materiales.Find(elemento.IDMaterial3);
                if (mat3.Precio == 0)
                {
                    mat3.Precio = SIAAPI.Properties.Settings.Default.CostoM2default;
                }
                precio3 = mat3.Precio;
            }
            catch (Exception err)
            {

            }
            elemento.CostoM2Cinta = precio1;
            elemento.CostoM2Cinta2 = precio2;
            elemento.CostoM2Cinta3 = precio3;
           
            return elemento;
        }

        public bool verificatintas(ClsCotizador elemento)

        {
            if (elemento.Tintas.Count>0)
            {
                foreach (Tinta tin in elemento.Tintas)
                {
                    if (tin.IDTinta==0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public string verificatintasC(ClsCotizador elemento)

        {
            string Mensaje = "";
            if (elemento.Tintas.Count > 0)
            {
                foreach (Tinta tin in elemento.Tintas)
                {
                    if (tin.IDTinta == 0)
                    {
                        return Mensaje;
                    }
                    else
                    {
                        
                    }
                }
            }
            return Mensaje;
        }

        public JsonResult Calculadientes(int cavavance, decimal largo, int almas)
        {
             if (almas>0)
            {
                decimal factor = almas * 3.175M;
                decimal factor2 = factor / cavavance;
                decimal resultado = factor2 - largo;
                return Json(resultado, JsonRequestBehavior.AllowGet);
            }
            return Json(0);
        }

        public JsonResult CalculagapReal(int cavavance, decimal largo, decimal dientes)
        {
            decimal largoenelsuaje = dientes *3.175M ;
            decimal largoporcavidad = largoenelsuaje / cavavance;
            decimal gapreal = largoporcavidad - largo;


            decimal resultado = gapreal;
                return Json(resultado, JsonRequestBehavior.AllowGet);
           
        }


        public ActionResult VisualizarXml(int id)
        {
            // Obtener contenido del archivo
            ArchivoCotizadorContext dbp = new ArchivoCotizadorContext();
            //Cotizaciones elemento = dbp.cotizaciones.Find(id);
            //string extension = elemento.NombreArchivo.Substring(elemento.NombreArchivo.Length - 3, 3);
            //extension = extension.ToLower();           

            //    string contentType = System.Net.Mime.MediaTypeNames.Text.Xml;
            //    string cadena = elemento.Ruta.ToString() + @"\" + elemento.NombreArchivo.ToString() +".xml";
            //    return new FilePathResult(cadena, contentType);
            ClsCotizador elemento;
            Cotizaciones archivocotizacion = new ArchivoCotizadorContext().cotizaciones.Find(id);
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

                string contentType = System.Net.Mime.MediaTypeNames.Text.Xml;
                string cadena = archivocotizacion.Ruta.ToString() + @"\" + archivocotizacion.NombreArchivo.ToString() +".xml";
            return new FilePathResult(cadena, contentType);
        }

        public ActionResult DescargarXml(int id)
        {
            // Obtener contenido del archivo
            ArchivoCotizadorContext dbp = new ArchivoCotizadorContext();
            Cotizaciones elemento = dbp.cotizaciones.Find(id);
            string extension = elemento.NombreArchivo.Substring(elemento.NombreArchivo.Length - 3, 3);
            extension = extension.ToLower();

            string contentType = System.Net.Mime.MediaTypeNames.Text.Xml;
            string cadena = elemento.Ruta.ToString() + @"\" + elemento.NombreArchivo.ToString() + ".xml";
            return new FilePathResult(cadena, contentType);

            System.Web.HttpContext.Current.Response.Buffer = true;
            System.Web.HttpContext.Current.Response.Clear();
            System.Web.HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=" + elemento.NombreArchivo.ToString() + ".xml");
            System.Web.HttpContext.Current.Response.WriteFile(cadena);

        }

        public ActionResult Createcontenido()
        {
            ArchivoCotizadorContext dbp = new ArchivoCotizadorContext();
            List<Cotizaciones> elementos = dbp.cotizaciones.Where(s=> s.contenido==null).ToList();
            foreach(Cotizaciones elementoarchivo in elementos)
            {
                try
                {
                    XmlDocument documento = new XmlDocument();
                    string nombredearchivo = elementoarchivo.Ruta.TrimEnd() + "\\" + elementoarchivo.NombreArchivo + ".xml";
                    documento.Load(nombredearchivo);

                    dbp.Database.ExecuteSqlCommand("update cotizaciones set contenido='" + documento.OuterXml + "' where id=" + elementoarchivo.ID);
                }
                catch(Exception err)
                { 
                
                }
            }

            return Content("YA termine", "text/plain", Encoding.UTF8);
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



