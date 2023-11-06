using FormulaEspecializada;
using PagedList;
using SIAAPI.ClasesProduccion;
using SIAAPI.Models.Administracion;
using SIAAPI.Models.Comercial;
using SIAAPI.Models.PlaneacionProduccion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace SIAAPI.Controllers.PlaneacionProduccion
{
    public class PlaneacionClassController : Controller
    {
        // GET: Planeacion class lables


        [Authorize(Roles = "Administrador,AdminProduccion,Gerencia,Sistemas")]

        public ActionResult Asistente(int idsuaje = 0)
        {
            int IDArticulo = Int16.Parse(System.Web.HttpContext.Current.Session["IDArticulo"].ToString());
            int IDCaracteristica = Int16.Parse(System.Web.HttpContext.Current.Session["IDCaracteristica"].ToString());

            Models.Comercial.Articulo articulo = new SIAAPI.Models.Comercial.ArticuloContext().Articulo.Find(IDArticulo);
            Models.Comercial.Caracteristica presentacion = new SIAAPI.Models.Comercial.ArticuloContext().Database.SqlQuery<Models.Comercial.Caracteristica>("select * from Caracteristica where id=" + IDCaracteristica).ToList().FirstOrDefault();

            ViewBag.Etiqueta = articulo.Descripcion;
            ViewBag.Presentacion = "presentacion " + presentacion.IDPresentacion;
            ViewBag.Presentacion2 = presentacion.Presentacion;

            FormulaSiaapi.Formulas formulas = new FormulaSiaapi.Formulas();
            int alpaso = int.Parse(formulas.getvalor("AL PASO", presentacion.Presentacion).ToString());

            int etiqxr = int.Parse(formulas.getvalor("ETIQUETAXR", presentacion.Presentacion).ToString());

            decimal ancho =  decimal.Parse(formulas.getvalor("ANCHO", presentacion.Presentacion).ToString());

            decimal  largo=  decimal.Parse(formulas.getvalor("LARGO", presentacion.Presentacion).ToString());

            
            ClsCotizador elemento = new ClsCotizador();

            elemento.productosalpaso = alpaso;
            elemento.Cantidadxrollo = etiqxr;


            if (elemento.TCcotizado == 0)

            {
                elemento.TCcotizado = SIAAPI.Properties.Settings.Default.TCcotizador;
            }


            ViewBag.IDCotizacion = 0;
            elemento.IDMonedapreciosconvenidos = new c_MonedaContext().c_Monedas.Where(s => s.ClaveMoneda == "USD").ToList().FirstOrDefault().IDMoneda;
            elemento.CostoM2Cinta = SIAAPI.Properties.Settings.Default.CostoM2default;
            elemento.TCcotizado = SIAAPI.Properties.Settings.Default.TCcotizador;
            elemento.DiluirSuajeEnPedidos = 50;
            elemento.Yatienematriz = false;
            elemento.CobrarMaster = false;
            elemento.Valido = false;
            ViewBag.Mensajedeerror = "";
            ViewBag.DescripcionSuaje = "";
            
            
            if (idsuaje > 0)
            {
                Models.Comercial.Caracteristica presentacionsuaje = new SIAAPI.Models.Comercial.ArticuloContext().Database.SqlQuery<Models.Comercial.Caracteristica>("select * from Caracteristica where id=" + idsuaje).ToList().FirstOrDefault();
                ViewBag.DescripcionSuaje = presentacionsuaje.Presentacion;

                elemento = this.llenaelemento(elemento, idsuaje);
                           
            }


            FamiliaContext dbar = new FamiliaContext();
            var datosArticuloF = dbar.Familias.Where(s => s.IDTipoArticulo == 6).OrderBy(i => i.CCodFam).ToList();
            List<SelectListItem> liAC = new List<SelectListItem>();
            liAC.Add(new SelectListItem { Text = "--Selecciona una Familia--", Value = "0" });
            foreach (var a in datosArticuloF)
            {
                liAC.Add(new SelectListItem { Text = a.CCodFam + " | " + a.Descripcion, Value = a.IDFamilia.ToString() });

            }
            ViewBag.IDFamilia = liAC;
            ViewBag.IDMaterial = getFamiliaPorArticulo(0);
            ViewBag.IDCaracteristica = getPresentacionPorArticuloMaterial(0);
            ViewBag.IDMaquinaPrensa     = this.getmaquinaPrensa();
            ViewBag.IDMaquinaEmbobinado = this.getmaquinaEmbobinado();
            ViewBag.IDFamilia2 = liAC;
            ViewBag.IDMAterial2 = getFamiliaPorArticulo(0);
            ViewBag.IDCaracteristica2 = getPresentacionPorArticuloMaterial(0);









            decimal cantidadd = ((1500000 / (elemento.largoproductomm + elemento.gapavance)) * elemento.cavidadesdesuaje)/1000;

            elemento.Cantidad = decimal.Parse(Math.Round(cantidadd,2).ToString());
            elemento.Minimoproducir = elemento.Cantidad;

            var suajes = new Repository().GetSuajes(idsuaje);
            ViewBag.IDSuaje = suajes;
            var plecas = new Repository().GetPlecas(elemento.IDSuaje2);
            ViewBag.IDSuaje2 = plecas;


            ViewBag.nombresuaje = idsuaje;
            var cintas = new Repository().GetCintas();
            //ViewBag.IDMaterial = cintas;

            ViewBag.IDCentro = new Repository().GetCentros(elemento.IDCentro);
            ViewBag.IDCaja = new Repository().GetCajas(elemento.IDCaja);
            ViewBag.Mensajedeerror = "";
            //ViewBag.IDMAterial2 = cintas;
        

            return View(elemento);

        }



        [HttpPost]
        public ActionResult Asistente(ClsCotizador elemento, FormCollection colecciondeelementos)
        {

          


            int IDArticulo = Int16.Parse(System.Web.HttpContext.Current.Session["IDArticulo"].ToString());
            int IDCaracteristica = Int16.Parse(System.Web.HttpContext.Current.Session["IDCaracteristica"].ToString());

            Models.Comercial.Articulo articulo = new SIAAPI.Models.Comercial.ArticuloContext().Articulo.Find(IDArticulo);
            Models.Comercial.Caracteristica presentacion = new SIAAPI.Models.Comercial.ArticuloContext().Database.SqlQuery<Models.Comercial.Caracteristica>("select * from Caracteristica where id=" + IDCaracteristica).ToList().FirstOrDefault();

            ViewBag.Etiqueta = articulo.Descripcion;
            ViewBag.Presentacion = "presentacion " + presentacion.IDPresentacion;
            ViewBag.Presentacion2 = presentacion.Presentacion;

            


            if (elemento.TCcotizado == 0)

            {
                elemento.TCcotizado = SIAAPI.Properties.Settings.Default.TCcotizador;
            }


            ViewBag.IDCotizacion = 0;
            elemento.IDMonedapreciosconvenidos = new c_MonedaContext().c_Monedas.Where(s => s.ClaveMoneda == "USD").ToList().FirstOrDefault().IDMoneda;
            elemento.CostoM2Cinta = SIAAPI.Properties.Settings.Default.CostoM2default;
            elemento.TCcotizado = SIAAPI.Properties.Settings.Default.TCcotizador;
            elemento.DiluirSuajeEnPedidos = 50;
            elemento.Yatienematriz = false;
            elemento.CobrarMaster = false;
            ViewBag.Mensajedeerror = "";
            ViewBag.DescripcionSuaje = "";

            
               Models.Comercial.Caracteristica presentacionsuaje = new SIAAPI.Models.Comercial.ArticuloContext().Database.SqlQuery<Models.Comercial.Caracteristica>("select * from Caracteristica where id=" + elemento.IDSuaje).ToList().FirstOrDefault();
                ViewBag.DescripcionSuaje = presentacionsuaje.Presentacion;

               elemento = this.llenaelemento(elemento, elemento.IDSuaje);
           

            var suajes = new Repository().GetSuajes(elemento.IDSuaje);
            ViewBag.IDSuaje = suajes;
            var plecas = new Repository().GetPlecas(elemento.IDSuaje2);
            ViewBag.IDSuaje2 = plecas;


            ViewBag.nombresuaje = elemento.IDSuaje;
            var cintas = new Repository().GetCintas();
            //ViewBag.IDMaterial = cintas;
            ViewBag.IDMaquinaPrensa = this.getmaquinaPrensa();
            ViewBag.IDMaquinaEmbobinado = this.getmaquinaEmbobinado();
            ViewBag.IDCentro = new Repository().GetCentros(elemento.IDCentro);
            ViewBag.IDCaja = new Repository().GetCajas(elemento.IDCaja);
            ViewBag.Mensajedeerror = "";
            //ViewBag.IDMAterial2 = cintas;



            FamiliaContext dbar = new FamiliaContext();
            var datosArticuloF = dbar.Familias.Where(s => s.IDTipoArticulo == 6).OrderBy(i => i.CCodFam).ToList();
            List<SelectListItem> liAC = new List<SelectListItem>();
            liAC.Add(new SelectListItem { Text = "--Selecciona una Familia--", Value = "0" });
            foreach (var a in datosArticuloF)
            {
                liAC.Add(new SelectListItem { Text = a.CCodFam + " | " + a.Descripcion, Value = a.IDFamilia.ToString() });

            }
            ViewBag.IDFamilia = liAC;
            
           
            ViewBag.IDFamilia2 = liAC;


            ViewBag.IDMaterial = getFamiliaPorArticulo(elemento.IDFamilia);
            ViewBag.IDCaracteristica = getPresentacionPorArticuloMaterial(elemento.IDMaterial);

            FormulaSiaapi.Formulas forsi = new FormulaSiaapi.Formulas();
            if (elemento.IDMaterial > 0)
            {
               
                Models.Comercial.Caracteristica cara = new ArticuloContext().Database.SqlQuery<Models.Comercial.Caracteristica>("select * from Caracteristica where Articulo_IDarticulo=" + elemento.IDMaterial).ToList().FirstOrDefault();
                double ancho = forsi.getvalor(cara.Presentacion, "ANCHO");
                int anchomaterial = int.Parse(Math.Round(ancho, 0).ToString());
                elemento.anchomaterialenmm=anchomaterial;
            }

            ViewBag.IDFamilia2 = liAC;
            ViewBag.IDMAterial2 = getFamiliaPorArticulo(elemento.IDFamilia2);
            ViewBag.IDCaracteristica2 = getPresentacionPorArticuloMaterial(elemento.IDMaterial2);
            ViewBag.IDMaquinaPrensa = this.getmaquinaPrensa(elemento.IDMaquinaPrensa);
            ViewBag.IDEmbobinado = this.getmaquinaEmbobinado(elemento.IDMaquinaEmbobinado);




            FormulaEspecializada.Formulaespecializada formula = new FormulaEspecializada.Formulaespecializada();
            formula = igualar(elemento, formula);



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


            formula.CobrarMaster = false;
            //////////////////////////  Calcular //////////////////////

            formula.Cantidad = 0;

            formula.Calcular();

            //   formula = CosteaTintas(formula);
            elemento.Minimoproducir = formula.Minimoproducir;
            formula.Cantidad = formula.Minimoproducir;

            formula.Calcular();
            decimal mo = formula.getHoraPrensa();
            elemento.HrPrensa = mo;
            elemento.Cantidad = formula.Cantidad;
         
            if (elemento.Cantidad>0 && elemento.IDMaterial>0 && elemento.HrPrensa>0)
            {
                elemento.Valido = true;
            }

            decimal moe = 0;

            if (formula.Cantidadxrollo > 0)
            {

                moe = formula.getHoraEmbobinado();
            }


            if (colecciondeelementos.Get("btnSiguiente") == "Crea Planeacion")

            {

                GeneradorPlaneacion(elemento);
                int IDPedido = int.Parse(System.Web.HttpContext.Current.Session["IDPedido"].ToString());
                return RedirectToAction("Details", "Encpedido", new { id = IDPedido });
            }

            return View(elemento);

        }
        public IEnumerable<SelectListItem> getPresentacionPorArticuloPla(int ida)
        {
            var presentacion = new ArticuloRepository().GetCaracteristicaPorArticulo(ida);
            return presentacion;

        }

        private ClsCotizador llenaelemento(ClsCotizador elemento, int IDSuaje)
        {
            Models.Comercial.Caracteristica cara = new ArticuloContext().Database.SqlQuery<Models.Comercial.Caracteristica>("Select * from Caracteristica where ID=" + IDSuaje).ToList().FirstOrDefault();
            ClasesProduccion.SuajeCaracteristicas suajec = new ClasesProduccion.SuajeCaracteristicas();
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



            suajec.Cavidades = 2;

            try
            {
                suajec.Cavidades = int.Parse(formula.getvalor("CAVIDADES EJE", cara.Presentacion).ToString());

                if (suajec.Cavidades == 0)
                {
                    suajec.Cavidades = int.Parse(formula.getvalor("CAV EJE", cara.Presentacion).ToString());

                    if (suajec.Cavidades == 0)

                    {

                        suajec.Cavidades = int.Parse(formula.getvalor("CAV AL EJE", cara.Presentacion).ToString());

                        if (suajec.Cavidades == 0)
                        {
                            suajec.Cavidades = int.Parse(formula.getvalor("CAVIDADES AL EJE", cara.Presentacion).ToString());
                            if (suajec.Cavidades == 0)
                            {
                                suajec.Cavidades = int.Parse(formula.getvalor("REP EJE", cara.Presentacion).ToString());
                                if (suajec.Cavidades == 0)
                                {
                                    suajec.Cavidades = int.Parse(formula.getvalor("REPETICIONES AL EJE", cara.Presentacion).ToString());
                                    if (suajec.Cavidades == 0)
                                    {
                                        suajec.Cavidades = int.Parse(formula.getvalor("REPETICIONES EJE", cara.Presentacion).ToString());
                                    }
                                }
                            }
                        }

                    }

                }

            }
            catch(Exception err)
            { 
                    string mensajederror = err.Message;
                suajec.Cavidades = 2;
            }

            elemento.cavidadesdesuaje = suajec.Cavidades;

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
                suajec.RepAvance = int.Parse(formula.getvalor("CAVIDADES DESARROLLO", cara.Presentacion).ToString());
                if (suajec.RepAvance == 0)
                {
                    suajec.RepAvance = int.Parse(formula.getvalor("CAV AVANCE", cara.Presentacion).ToString());
                    if (suajec.RepAvance == 0)
                    {
                        suajec.RepAvance = int.Parse(formula.getvalor("CAVIDADES AVANCE", cara.Presentacion).ToString());
                        if (suajec.RepAvance == 0)
                        {
                          
                                suajec.RepAvance = int.Parse(formula.getvalor("CAVIDADES AL AVANCE", cara.Presentacion).ToString());
                            if (suajec.RepAvance == 0)
                            {

                                suajec.RepAvance = int.Parse(formula.getvalor("CAVIDADES AL DESARROLLO", cara.Presentacion).ToString());
                                if (suajec.RepAvance == 0)
                                {

                                    suajec.RepAvance = int.Parse(formula.getvalor("REP AVANCE", cara.Presentacion).ToString());
                                    if (suajec.RepAvance == 0)
                                    {

                                        suajec.RepAvance = int.Parse(formula.getvalor("REPETICIONES AL AVANCE", cara.Presentacion).ToString());
                                        if (suajec.RepAvance == 0)
                                        {

                                            suajec.RepAvance = int.Parse(formula.getvalor("REPETICIONES AVANCE", cara.Presentacion).ToString());

                                        }
                                    }
                                }
                            }
                        }
                    }
                }

            }
                           
            catch(Exception err)
            { 
                    string mensajederror = err.Message;
                suajec.RepAvance = 1;
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
                if (suajec.TH == 0)
                {
                    suajec.TH = int.Parse(formula.getvalor("TH", cara.Presentacion).ToString());
                    if (suajec.TH==0)
                    {
                        suajec.TH = int.Parse(formula.getvalor("ALMA", cara.Presentacion).ToString());
                        if (suajec.TH == 0)
                        {
                            suajec.TH = int.Parse(formula.getvalor("DIENTES", cara.Presentacion).ToString());
                        }
                    }
                }

            }
            catch (Exception err)
            {
                string mensajederror = err.Message;
                suajec.TH = 0;
            }

            suajec.AnchoCinta = 0;
            try
            {
                suajec.AnchoCinta = int.Parse(formula.getvalor("ANCHO DE CINTA", cara.Presentacion).ToString());

            }
            catch (Exception err)
            {
                string mensajederror = err.Message;
                suajec.AnchoCinta = int.Parse(((suajec.Eje * decimal.Parse(suajec.Cavidades.ToString())) + (decimal.Parse(((suajec.Cavidades - 1) * suajec.Gapeje).ToString()))).ToString());
            }

            elemento.anchomaterialenmm = suajec.AnchoCinta;


            try
            {
                suajec.EjeSuaje = int.Parse(formula.getvalor("EJE SUAJE", cara.Presentacion).ToString());

            }
            catch (Exception err)
            {
                string mensajederror = err.Message;
                suajec.EjeSuaje = 7;
            }

            elemento.EjeMaquina = suajec.EjeSuaje;

            return elemento;
        }


        public ActionResult SuajeExistentePla(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize, string eje, string avance)
        {
            int idarticulo = 0;
            int idcaracteristica = 0;
            try
            {
                idarticulo = int.Parse(System.Web.HttpContext.Current.Session["IDArticulo"].ToString());
                idcaracteristica = int.Parse(System.Web.HttpContext.Current.Session["IDCaracteristica"].ToString());
                ViewBag.Articulo = idarticulo;
            }
            catch (Exception err)
            {

            }
            if (idarticulo > 0)
            {
                SIAAPI.Models.Comercial.Articulo articulo = new ArticuloContext().Articulo.Find(idarticulo);
                ViewBag.Articulo = articulo.Cref + " | " + articulo.Descripcion;

                SIAAPI.Models.Comercial.Caracteristica caracteristica = new ArticuloContext().Database.SqlQuery<SIAAPI.Models.Comercial.Caracteristica>("select * from Caracteristica where id=" + idcaracteristica).ToList().FirstOrDefault();
                ViewBag.Caracteristica = caracteristica.Presentacion;
                ViewBag.IDPresentacion = caracteristica.IDPresentacion;
            }
            else
            {
                ViewBag.Articulo = "";
                ViewBag.Caracteristica = "";
            }



            ViewBag.CurrentSort = sortOrder;

            ViewBag.RazoneSortParm = String.IsNullOrEmpty(sortOrder) ? "ID" : "ID";

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

            string cadenaeje = "EJE:" + eje;
            string cadenaavance = "AVANCE:" + avance;

            ViewBag.CurrentFilter = searchString;

            ViewBag.PresentacionList = getPresentacionPorArticulo(0);

            string ConsultaSql = "select*from Caracteristica as c inner join articulo as a on  a.idarticulo=c.articulo_idarticulo";
            string Filtro = "";

            if (String.IsNullOrEmpty(eje) && String.IsNullOrEmpty(avance)) //00 ninguna
            {

                if (Filtro == string.Empty)
                {

                    //Response.Write("<script>alert('Campos vacios')</script>");
                    //Filtro = " where presentacion like '%" + cadenaeje + "%'";

                    Filtro = " where c.presentacion like '%" + cadenaeje + "%' and c.presentacion like '%" + cadenaavance + "%' and  (a.idfamilia= 11 or a.idfamilia= 69 or a.idfamilia= 71 or a.idfamilia= 72 or a.idfamilia= 75 or a.IDFamilia=76 or a.IDFamilia=77 or a.IDFamilia=79 or a.IDFamilia=80)";
                }
                else
                {
                    Filtro = " and c.presentacion like '%" + cadenaeje + "%' and c.presentacion like '%" + cadenaavance + "%' and  (a.idfamilia= 11 or a.idfamilia= 69 or a.idfamilia= 71 or a.idfamilia= 72 or a.idfamilia= 75 or a.IDFamilia=76 or a.IDFamilia=77 or a.IDFamilia=79 or a.IDFamilia=80)";

                }



            }

            if (String.IsNullOrEmpty(eje) && !String.IsNullOrEmpty(avance)) //01 eje no, avance si
            {

                if (Filtro == string.Empty)
                {


                    Filtro = " where c.presentacion like '%" + cadenaavance + "%' and  (a.idfamilia= 11 or a.idfamilia= 69 or a.idfamilia= 71 or a.idfamilia= 72 or a.idfamilia= 75 or a.IDFamilia=76 or a.IDFamilia=77 or a.IDFamilia=79 or a.IDFamilia=80)";
                }
                else
                {
                    Filtro = " and c.presentacion like '%" + cadenaavance + "%'";
                }


            }

            if (!String.IsNullOrEmpty(eje) && String.IsNullOrEmpty(avance)) //01 eje si, avance no
            {

                if (Filtro == string.Empty)
                {


                    Filtro = " where c.presentacion like '%" + cadenaeje + "%' and (a.idfamilia= 11 or a.idfamilia= 69 or a.idfamilia= 71 or a.idfamilia= 72 or a.idfamilia= 75 or a.IDFamilia=76 or a.IDFamilia=77 or a.IDFamilia=79 or a.IDFamilia=80)";
                }
                else
                {
                    Filtro = " and c.presentacion like '%" + cadenaeje + "%'";
                }


            }

            if (!String.IsNullOrEmpty(eje) && !String.IsNullOrEmpty(avance)) //11 eje si, avance si
            {

                if (Filtro == string.Empty)
                {


                    Filtro = " where c.presentacion like '%" + cadenaeje + "%' and c.presentacion like '%" + cadenaavance + "%' and  (a.idfamilia= 11 or a.idfamilia= 69 or a.idfamilia= 71 or a.idfamilia= 72 or a.idfamilia= 75 or a.IDFamilia=76 or a.IDFamilia=77 or a.IDFamilia=79 or a.IDFamilia=80)";
                }
                else
                {
                    Filtro = " and c.presentacion like '%" + cadenaeje + "%' and c.presentacion like '%" + cadenaavance + "%'";
                }


            }

            // Pass filtering string to view in order to maintain filtering when paging
            ViewBag.eje = eje;
            ViewBag.avance = avance;

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

            string cadenaSQl = ConsultaSql + " " + Filtro + " " + orden;
            var elementos1 = cc.Database.SqlQuery<Models.Comercial.Caracteristica>(cadenaSQl).ToList();
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
        public ActionResult getJsonCaracteristicaArticuloMaterial(int id)
        {
            var presentacion = new ArticuloRepository().GetCaracteristicaPorArticulo(id);
            return Json(presentacion, JsonRequestBehavior.AllowGet);

        }

        public IEnumerable<SelectListItem> getPresentacionPorArticuloMaterial(int ida)
        {
            var presentacion = new ArticuloRepository().GetCaracteristicaPorArticulo(ida);
            return presentacion;

        }
        public ActionResult getJsonFamiliArticulo(int id)
        {
            var presentacion = new ArticuloRepository().GetArticuloFamilia(id);
            return Json(presentacion, JsonRequestBehavior.AllowGet);

        }
        public IEnumerable<SelectListItem> getFamiliaPorArticulo(int ida)
        {
            var presentacion = new ArticuloRepository().GetArticuloFamilia(ida);
            return presentacion;

        }

        public IEnumerable<SelectListItem> getmaquinaPrensa()
        {
            var presentacion = new ArticuloRepository().GetArticuloFamilia(18);
            return presentacion;

        }


        public IEnumerable<SelectListItem> getmaquinaPrensa(int seleccionado)
        {
            var presentacion = new ArticuloRepository().GetArticuloFamilia(18,seleccionado);
            return presentacion;

        }


        public IEnumerable<SelectListItem> getmaquinaEmbobinado()
        {
            var presentacion = new ArticuloRepository().GetArticuloFamilia(22);
            return presentacion;

        }

        public IEnumerable<SelectListItem> getmaquinaEmbobinado(int seleccionado)
        {
            var presentacion = new ArticuloRepository().GetArticuloFamilia(22, seleccionado);
            return presentacion;

        }


        public ActionResult SuajeExistentePlapedido(int IDArticulo, int IDCaracteristica, string sortOrder, string currentFilter, string searchString, int? page, int? PageSize, string eje, string avance,int IDPedido=0)
        {
            if (IDArticulo > 0)
            {
                System.Web.HttpContext.Current.Session["IDArticulo"] = IDArticulo;
                System.Web.HttpContext.Current.Session["IDPedido"] = IDPedido;
                System.Web.HttpContext.Current.Session["IDCaracteristica"] = IDCaracteristica;
                ViewBag.IDArticulo = IDArticulo;
                ViewBag.IDArticuloC = IDCaracteristica;
            }
            if (IDArticulo==0 && System.Web.HttpContext.Current.Session["IDArticulo"].ToString().Length>0)
            {

                 IDArticulo = Int16.Parse(System.Web.HttpContext.Current.Session["IDArticulo"].ToString());
                 IDCaracteristica = Int16.Parse(System.Web.HttpContext.Current.Session["IDCaracteristica"].ToString());

            }



            SIAAPI.Models.Comercial.Articulo articulo = new ArticuloContext().Articulo.Find(IDArticulo);
                ViewBag.Articulo = articulo.Cref + " | " + articulo.Descripcion;

                SIAAPI.Models.Comercial.Caracteristica caracteristica = new ArticuloContext().Database.SqlQuery<SIAAPI.Models.Comercial.Caracteristica>("select*from Caracteristica where id=" + IDCaracteristica).ToList().FirstOrDefault();
                ViewBag.Caracteristica = caracteristica.Presentacion;
                ViewBag.IDPresentacion = caracteristica.IDPresentacion;
            


            ViewBag.CurrentSort = sortOrder;

            ViewBag.RazoneSortParm = String.IsNullOrEmpty(sortOrder) ? "ID" : "ID";

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
            
            string cadenaeje = "EJE:" + eje +"%";
            string cadenaavance;
            if (avance == string.Empty)
            {
                cadenaavance = "%";
            }
            else
            {
                 cadenaavance = "%,AVANCE%:" + avance + "%";
            }

            ViewBag.CurrentFilter = searchString;

            ViewBag.PresentacionList = getPresentacionPorArticulo(0);

            string ConsultaSql = "select * from Caracteristica as c inner join articulo as a on  a.idarticulo=c.articulo_idarticulo";
            string Filtro = "";

            if (String.IsNullOrEmpty(eje) && String.IsNullOrEmpty(avance)) //00 ninguna
            {

                if (Filtro == string.Empty)
                {

                    //Response.Write("<script>alert('Campos vacios')</script>");
                    //Filtro = " where presentacion like '%" + cadenaeje + "%'";

                    Filtro = " where  (a.idfamilia= 11 or a.idfamilia= 69 or a.idfamilia= 71 or a.idfamilia= 72 or a.idfamilia= 75 or a.IDFamilia=76 or a.IDFamilia=77 or a.IDFamilia=79 or a.IDFamilia=80)";
                }
                else
                {
                    Filtro = " and   (a.idfamilia= 11 or a.idfamilia= 69 or a.idfamilia= 71 or a.idfamilia= 72 or a.idfamilia= 75 or a.IDFamilia=76 or a.IDFamilia=77 or a.IDFamilia=79 or a.IDFamilia=80)";

                }



            }

            if (String.IsNullOrEmpty(eje) && !String.IsNullOrEmpty(avance)) //01 eje no, avance si
            {

                if (Filtro == string.Empty)
                {


                    Filtro = " where c.presentacion like '" + cadenaavance + "' and  (a.idfamilia= 11 or a.idfamilia= 69 or a.idfamilia= 71 or a.idfamilia= 72 or a.idfamilia= 75 or a.IDFamilia=76 or a.IDFamilia=77 or a.IDFamilia=79 or a.IDFamilia=80)";
                }
                else
                {
                    Filtro = " and c.presentacion like '" + cadenaavance + "'";
                }


            }

            if (!String.IsNullOrEmpty(eje) && String.IsNullOrEmpty(avance)) //01 eje si, avance no
            {

                if (Filtro == string.Empty)
                {


                    Filtro = " where c.presentacion like '" + cadenaeje + "' and (a.idfamilia= 11 or a.idfamilia= 69 or a.idfamilia= 71 or a.idfamilia= 72 or a.idfamilia= 75 or a.IDFamilia=76 or a.IDFamilia=77 or a.IDFamilia=79 or a.IDFamilia=80)";
                }
                else
                {
                    Filtro = " and c.presentacion like '" + cadenaeje + "'";
                }


            }

            if (!String.IsNullOrEmpty(eje) && !String.IsNullOrEmpty(avance)) //11 eje si, avance si
            {

                if (Filtro == string.Empty)
                {


                    Filtro = " where c.presentacion like '%" + cadenaeje + "%' and c.presentacion like '%" + cadenaavance + "%' and  (a.idfamilia= 11 or a.idfamilia= 69 or a.idfamilia= 71 or a.idfamilia= 72 or a.idfamilia= 75 or a.IDFamilia=76 or a.IDFamilia=77 or a.IDFamilia=79 or a.IDFamilia=80)";
                }
                else
                {
                    Filtro = " and c.presentacion like '%" + cadenaeje + "%' and c.presentacion like '%" + cadenaavance + "%'";
                }


            }

            // Pass filtering string to view in order to maintain filtering when paging
            ViewBag.eje = eje;
            ViewBag.avance = avance;

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

            string cadenaSQl = ConsultaSql + " " + Filtro + " " + orden;
            var elementos1 = cc.Database.SqlQuery<Models.Comercial.Caracteristica>(cadenaSQl).ToList();
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



        public ActionResult BuscaEtiqueta()
        {


            //Articulos
            ArticuloContext dbar = new ArticuloContext();
            var datosArticulo = dbar.Articulo.OrderBy(i => i.Cref).ToList();
            List<SelectListItem> liAC = new List<SelectListItem>();
            liAC.Add(new SelectListItem { Text = "--Selecciona un Articulo--", Value = "0" });
            foreach (var a in datosArticulo)
            {
                liAC.Add(new SelectListItem { Text = a.Cref + " | " + a.Descripcion, Value = a.IDArticulo.ToString() });

            }
            ViewBag.IDArticulo = liAC;
            ViewBag.PresentacionList = getPresentacionPorArticulo(0);





            return View();
        }

        // POST: Inventario/Create
        [HttpPost]

        public ActionResult BuscaEtiqueta(SIAAPI.Models.Comercial.Caracteristica coleccion)
        {

            System.Web.HttpContext.Current.Session["IDArticulo"] = coleccion.Articulo_IDArticulo;
            System.Web.HttpContext.Current.Session["IDCaracteristica"] = coleccion.ID;



            return RedirectToAction("SuajeExistentePla");


        }





        public ActionResult getJsonCaracteristicaArticulo(int id)
        {
            var presentacion = new ArticuloRepository().GetCaracteristicaPorArticulo(id);
            return Json(presentacion, JsonRequestBehavior.AllowGet); ;

        }

        public IEnumerable<SelectListItem> getPresentacionPorArticulo(int ida)
        {
            var presentacion = new ArticuloRepository().GetCaracteristicaPorArticulo(ida);
            return presentacion;

        }


        private List<ClasesProduccion.Tinta> Gettintasiniciales(int numerotintas, FormCollection _collection, decimal MetrosCuadrados = 0)
        {
            List<ClasesProduccion.Tinta> tintas = new List<ClasesProduccion.Tinta>();
            for (int i = 1; i <= numerotintas; i++)
            {
                ClasesProduccion.Tinta nueva = new ClasesProduccion.Tinta();
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


        private ClsCotizador pasaTintas(List<ClasesProduccion.Tinta> tintas, ClsCotizador formula)
        {
            decimal costoacu = 0;
            formula.Tintas.Clear();
            foreach (ClasesProduccion.Tinta tin in tintas)
            {
                ClasesProduccion.Tinta nueva = new ClasesProduccion.Tinta();
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
        public FormulaEspecializada.Formulaespecializada pasaTintas(List<ClasesProduccion.Tinta> tintas, FormulaEspecializada.Formulaespecializada formula)
        {
            decimal costoacu = 0;
            formula.Tintas.Clear();
            foreach (ClasesProduccion.Tinta tin in tintas)
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

        public void GeneradorPlaneacion(ClsCotizador elemento)
        {
            int IDHE = CrearNHE(4);

            HEspecificacionEContext HEC = new HEspecificacionEContext();
            HEspecificacionE HE = HEC.HEspecificacionesE.Find(IDHE);


            ArchivoCotizadorContext AC = new ArchivoCotizadorContext();
            ConfigPlaneacion config = AC.Configuracionpleaneacion.ToList().FirstOrDefault();

            // crando la mano de obra
            /// primero relvemos la cantidad de millares por hora
            /// 

            decimal millaresporhora = Math.Round((elemento.Cantidad / elemento.HrPrensa), 2);

            string formulahora = "C/" + millaresporhora;

            insertaarticulo(config.IDMO7, config.IDpresMO7, IDHE, HE.Version, 5, 5, formulahora, 0, "", HE.Planeacion); // mano de obra
            /// la maquina de acuerdo al eje del suaje
            /// 

            //if (elemento.EjeMaquina==7)
            //{
            //    insertaarticulo(config.IDMaquina7, config.IDPresentacion7, IDHE, HE.Version,3, 5, formulahora, 0, "", HE.Planeacion);
            //}

            //if (elemento.EjeMaquina == 10)
            //{
            //    insertaarticulo(config.IDMaquina10, config.IDPresentacion10, IDHE, HE.Version, 3, 5, formulahora, 0, "", HE.Planeacion);
            //}
            //if (elemento.EjeMaquina == 13)

            //{
            Models.Comercial.Caracteristica cara = new ArticuloContext().Database.SqlQuery<Models.Comercial.Caracteristica>("select * from Caracteristica where Articulo_IDArticulo=" + elemento.IDMaquinaPrensa).ToList().FirstOrDefault();
                insertaarticulo(elemento.IDMaquinaPrensa, cara.ID, IDHE, HE.Version, 3, 5, formulahora, 0, "", HE.Planeacion);
            //}

            /////// suajes

            SIAAPI.Models.Comercial.Caracteristica caracteristica = HEC.Database.SqlQuery<SIAAPI.Models.Comercial.Caracteristica>("select * from Caracteristica where ID=" + elemento.IDSuaje).ToList().FirstOrDefault();

            insertaarticulo( caracteristica.Articulo_IDArticulo, caracteristica.ID, IDHE, HE.Version, 2, 5, "1/50", 0, "", HE.Planeacion);

            if (elemento.IDSuaje2>0)
            {

                SIAAPI.Models.Comercial.Caracteristica caracteristica2 = HEC.Database.SqlQuery<SIAAPI.Models.Comercial.Caracteristica>("select * from Caracteristica where ID=" + elemento.IDSuaje2).ToList().FirstOrDefault();
                insertaarticulo(caracteristica2.Articulo_IDArticulo, caracteristica2.ID, IDHE, HE.Version, 2, 5, "1/50", 0, "", HE.Planeacion);

            }


            /// MATERIALES
        //    /// 
         //   string crefabuscar = Cadenamaterial(elemento.IDMaterial, elemento.anchomaterialenmm);
            //int IDArticulomaterial = new ArticuloContext().Articulo.Where(s => s.Cref == crefabuscar).ToList().FirstOrDefault().IDArticulo;
            
           //     int IDArticulomaterial = HEC.Database.SqlQuery<ClsDatoEntero>("select IDArticulo as Dato from Articulo where cref='" + crefabuscar + "'").ToList().FirstOrDefault().Dato;
           
            //HEC.Database.SqlQuery<Models.Comercial.Articulo>("Select * from articulo where cref='" + crefabuscar + "'").ToList().FirstOrDefault().IDArticulo;

           // SIAAPI.Models.Comercial.Caracteristica caracteristica3 = HEC.Database.SqlQuery<SIAAPI.Models.Comercial.Caracteristica>("select * from Caracteristica where Articulo_IDArticulo=" + IDArticulomaterial).ToList().FirstOrDefault();

            string cadenaformulam2 = "(((C*(" + elemento.largoproductomm + "+" + elemento.gapavance + "))/" + elemento.cavidadesdesuaje + ")*(" + elemento.anchomaterialenmm + "/1000))";
            string cadenaformulamaterial = cadenaformulam2+"*1.03";

            insertaarticulo(elemento.IDMaterial, elemento.IDCaracteristica, IDHE, HE.Version, 6, 5, cadenaformulamaterial, 0, "", HE.Planeacion);

            if (elemento.IDMaterial2 > 0)
            {
              //  string crefabuscar2 = Cadenamaterial(elemento.IDMaterial2, elemento.anchomaterialenmm);
               // int IDArticulomaterial2 = HEC.Database.SqlQuery<Models.Comercial.Articulo>("Select * from articulo where cref='" + crefabuscar2 + "'").ToList().FirstOrDefault().IDArticulo;

               // SIAAPI.Models.Comercial.Caracteristica caracteristica4 = HEC.Database.SqlQuery<SIAAPI.Models.Comercial.Caracteristica>("select * from Caracteristica where ID=" + IDArticulomaterial2).ToList().FirstOrDefault();


                insertaarticulo(elemento.IDMaterial2, elemento.IDCaracteristica2, IDHE, HE.Version, 6, 5, cadenaformulamaterial, 0, "", HE.Planeacion);

            }



            //// TINTAS

            foreach (ClasesProduccion.Tinta tinta in elemento.Tintas)
            {
                string formulatinta = "(" + cadenaformulam2 + "/300)*" + Math.Round((tinta.Area / 100), 0);
             
               

                SIAAPI.Models.Comercial.Caracteristica caracteristicatint = HEC.Database.SqlQuery<SIAAPI.Models.Comercial.Caracteristica>("select * from Caracteristica where Articulo_IDArticulo=" + tinta.IDTinta).ToList().FirstOrDefault();


                insertaarticulo(tinta.IDTinta, caracteristicatint.ID, IDHE, HE.Version, 7, 5, formulatinta, 0.25M, "", HE.Planeacion); ///insertar las tintas


            }

            //////////////////embobinado

            Models.Comercial.Caracteristica caraembo = new ArticuloContext().Database.SqlQuery<Models.Comercial.Caracteristica>("select * from Caracteristica where Articulo_IDArticulo=" + elemento.IDMaquinaEmbobinado).ToList().FirstOrDefault();
            //// metemos la maquina de embobinado
            string formulahoraemb = "C/" +Math.Round( millaresporhora*0.8M) ;
                insertaarticulo(caraembo.Articulo_IDArticulo, caraembo.ID, IDHE, HE.Version,3,4, formulahoraemb, 0.25M, "", HE.Planeacion); ///insertar las tintas

            insertaarticulo(config.IDMOEmb, config.IDpresMoEmb, IDHE, HE.Version, 5, 4, formulahoraemb, 0.25M, "", HE.Planeacion); ///insertar las tintas

           
            

            //////caja

            SIAAPI.Models.Comercial.Caracteristica caracteristicacaja = HEC.Database.SqlQuery<SIAAPI.Models.Comercial.Caracteristica>("select * from Caracteristica where Articulo_IDArticulo=" + elemento.IDCaja).ToList().FirstOrDefault();

            int cantidadxcaja = int.Parse(Math.Round((elemento.Cantidad * 0.8M),0).ToString());
            insertaarticulo(elemento.IDCaja, caracteristicacaja.ID, IDHE, HE.Version, 4, 4, "C/"+ cantidadxcaja, 1, "", HE.Planeacion); ///insertar las tintas



            ///// tubo


            SIAAPI.Models.Comercial.Caracteristica caracteristicatubo = HEC.Database.SqlQuery<SIAAPI.Models.Comercial.Caracteristica>("select * from Caracteristica where Articulo_IDArticulo=" + elemento.IDCentro).ToList().FirstOrDefault();

            string formulacentro = "(C/" + elemento.productosalpaso + ")/(1000/" +( elemento.anchoproductomm + elemento.gapeje )+ ")";

            insertaarticulo(elemento.IDCentro, caracteristicatubo.ID, IDHE, HE.Version, 4, 4,   formulacentro, 1, "", HE.Planeacion); ///insertar las tintas

            string cadenaesecificacione = "INSERT INTO [dbo].[EspecificacionEtiqueta] ([IDHE],[Ancho],[Largo],[Cavidades] ,[GapEje],[GapAvance],[Alpaso],[Etiquetaxrollo],[AnchodeMaterialR],[LargodeMaterial]) values ";

            cadenaesecificacione += "(" + IDHE + "," + elemento.anchoproductomm + "," + elemento.largoproductomm + "," + elemento.cavidadesdesuaje + "," + elemento.gapeje + "," + elemento.gapavance + "," + elemento.productosalpaso + "," + elemento.Cantidadxrollo + "," + elemento.anchomaterialenmm + ",1500)";

            HEC.Database.ExecuteSqlCommand(cadenaesecificacione);

            /////caracteristica planeacion
            int IDARTICULOPLANEADO = int.Parse(System.Web.HttpContext.Current.Session["IDArticulo"].ToString());
            int idcaracteristicapla = int.Parse(System.Web.HttpContext.Current.Session["IDCaracteristica"].ToString());


            string cadenaesecificaciocarc = "INSERT INTO [dbo].[CaracteristicaPlaneacionE] ([IDCaracteristica],[IDArticulo],[Planeacion],[Version],[IDHE] ,[IDCliente]) VALUES ";

            cadenaesecificaciocarc += "(" +idcaracteristicapla+","+IDARTICULOPLANEADO+"," +  HE.Planeacion + "," + HE.Version + "," + IDHE + ",0)";
            HEC.Database.ExecuteSqlCommand(cadenaesecificaciocarc);

            int IDArticulo = Int16.Parse(System.Web.HttpContext.Current.Session["IDArticulo"].ToString());
            int IDCaracteristicaa = Int16.Parse(System.Web.HttpContext.Current.Session["IDCaracteristica"].ToString());

            string Asignaarticulo = "update Caracteristica set cotizacion=" + HE.Planeacion + ", version=" + HE.Version + " where id=" + IDCaracteristicaa;

            try
            {
                HEC.Database.ExecuteSqlCommand(Asignaarticulo);
            }
            catch(Exception err)
            {

            }
           
        }




        public int CrearNHE(int Modelo) /// va retornar el id de la hoja de especificacion 
        {
            HEspecificacionEContext HE = new HEspecificacionEContext();

            int numeroPlaneacion = 1;
            try
            {
                numeroPlaneacion = HE.Database.SqlQuery<ClsDatoEntero>("select max(planeacion) as Dato from HespecificacionE").ToList().FirstOrDefault().Dato;
                numeroPlaneacion ++;
            
            }
            catch (Exception err)
            {

            }
            int IDArticulo = Int16.Parse(System.Web.HttpContext.Current.Session["IDArticulo"].ToString());
            int IDCaracteristica = Int16.Parse(System.Web.HttpContext.Current.Session["IDCaracteristica"].ToString());

            Models.Comercial.Articulo articulo = new SIAAPI.Models.Comercial.ArticuloContext().Articulo.Find(IDArticulo);
            Models.Comercial.Caracteristica presentacion = new SIAAPI.Models.Comercial.ArticuloContext().Database.SqlQuery<Models.Comercial.Caracteristica>("select * from Caracteristica where id=" + IDCaracteristica).ToList().FirstOrDefault();

            try
            {
                String cadena = "insert into HEspecificacionE ([FechaEspecificacion] ,[IDCliente],[IDClienteP],[IDFamilia],[IDArticulo],[IDCaracteristica],[Descripcion],[Presentacion],[IDModeloProduccion],[Version],[Planeacion],[ESPECIFICACION]) values ";
                cadena += "(GETDATE(),0,0," + articulo.IDFamilia+"," + articulo.IDArticulo + "," + presentacion.ID + ",'" + articulo.Descripcion + "','" + presentacion.Presentacion + "'," + Modelo + ",1," + numeroPlaneacion + ",'')";
                HE.Database.ExecuteSqlCommand(cadena);
            }
            catch (Exception err)
            {

            }
            int numerohe = HE.Database.SqlQuery<ClsDatoEntero>("select max(IDHE) as Dato from HEspecificacionE WHERE IDArticulo =" + IDArticulo + " and IDCaracteristica=" + presentacion.ID).ToList().FirstOrDefault().Dato;

            return numerohe;
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
            formula.cavidadesdesuaje = elemento.cavidadesdesuaje;
            formula.conadhesivo = elemento.conadhesivo;
            formula.hotstamping = elemento.hotstamping;
            formula.CostoM2Cinta = elemento.CostoM2Cinta;
            formula.LargoCinta = elemento.LargoCinta;

            return formula;
        }


        public JsonResult Valida(int idmaterial, int anchomaterial)
        {
            string cadena;
            matval matval1 = new matval();
            matval1.valido = "false";
            cadena = Cadenamaterial(idmaterial, anchomaterial);

            matval1.clave = cadena;
            var articulos = new ArticuloContext().Articulo.Where(s => s.Cref == cadena).ToList();

            if (articulos.Count > 0)
            {
                matval1.valido = "true";
                return Json(matval1, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(matval1, JsonRequestBehavior.AllowGet);
            }

        }

        public string Cadenamaterial(int idmaterial, int achomaterial)
        {
            MaterialesContext mat = new MaterialesContext();
            Models.Administracion.Materiales material = mat.Materiales.Find(idmaterial);
            string cadena = "BOB";

            string ancho = achomaterial.ToString();
            if (ancho.Length == 2)
            {
                ancho = "0" + ancho;
            }
            else if (ancho.Length == 1)
            {
                ancho = "00" + ancho;
            }

         //   cadena = cadena + ancho + material.Fam;
            return cadena;

        }



        public void insertaarticulo(int idarticulo, int idpresentacion, int he, int version, int idtipo, int proceso, string formula, decimal factorcierre, string indicaciones, int planeacion)
        {
            try
            {
                SIAAPI.Models.PlaneacionProduccion.ArticulosPlaneacionE dba = new SIAAPI.Models.PlaneacionProduccion.ArticulosPlaneacionE();
                dba.IDArticulo = idarticulo;
                dba.IDCaracteristica = idpresentacion;
                dba.IDHE = he;
                dba.IDTipoArticulo = idtipo;
                dba.IDProceso = proceso;
                dba.Formuladerelacion = formula;
                dba.Indicaciones = indicaciones;
                dba.factorcierre = factorcierre;
                dba.Planeacion = planeacion;
                dba.Version = version;
                SIAAPI.Models.PlaneacionProduccion.ArticulosPlaneacionEContext db = new SIAAPI.Models.PlaneacionProduccion.ArticulosPlaneacionEContext();
                db.ArticulosPlaneacionesE.Add(dba);
                db.SaveChanges();

            }
            catch(Exception err)
            {
                string mensajederror = err.Message;
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

        public JsonResult gettintasblandas(string buscar)
        {
            buscar = buscar.Remove(buscar.Length - 1);
            var Articulos = new ArticuloContext().Articulo.Where(s => s.Descripcion.Contains(buscar) && s.IDTipoArticulo == 7);

            // Populate DropDownList
            List<SelectListItem> opciones = new List<SelectListItem>();

            foreach (SIAAPI.Models.Comercial.Articulo art in Articulos)
            {
                SelectListItem elemento = new SelectListItem();
                elemento.Text = art.Descripcion;
                elemento.Value = art.IDArticulo.ToString();
                opciones.Add(elemento);
            }

            return Json(opciones, JsonRequestBehavior.AllowGet);
        }



        public class matval
    {
        public string clave { get; set; }
        public string valido { get; set; }

        }
    }
}