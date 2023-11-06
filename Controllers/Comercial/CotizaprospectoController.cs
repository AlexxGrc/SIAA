using OfficeOpenXml;
using OfficeOpenXml.Style;
using PagedList;
using SIAAPI.Models.Administracion;
using SIAAPI.Models.Comercial;
using SIAAPI.Reportes;
using SIAAPI.ViewModels.Cfdi;
using SIAAPI.ViewModels.Comercial;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SIAAPI.Controllers.Comercial
{
    [Authorize(Roles = "Administrador,Gerencia, Comercial,Sistemas,GerenteVentas, Ventas,Compras")]
    public class CotizaprospectoController : Controller
    {
        private CotizacionProspectoContext db = new CotizacionProspectoContext();
        // GET: Cotizaprospecto
        public ActionResult Index(string Cliente, string Vendedor, string Oficina, string Divisa, string Status, string sortOrder, string currentFilter, string searchString, string Fechainicio, string Fechafinal, int? page, int? PageSize)
        {
            VCotizacionesProsContext dbc = new VCotizacionesProsContext();
            VendedorContext dbv = new VendedorContext();


            string ConsultaSql = "select * from dbo.VCotizacionesPros";
            string FiltroSql = string.Empty;
            string OrdenSql = "order by ID desc";
            string SumaSql = "select ClaveMoneda as MonedaOrigen, Sum(Subtotal) as Subtotal, Sum(IVA) as IVA, Sum(Total) as Total, sum(Total * TipoCambio) as TotalenPesos from dbo.VCotizacionesPros";
            string FiltroSuma = "and Estado != 'Cancelado'";
            string GroupSql = "group by ClaveMoneda";
            string CadenaSql = string.Empty;
            string CadenaResumenSql = string.Empty;

            try
            {

                ///filtro: Divisa
                var SerLst = new List<string>();
                var SerQry = from d in db.c_Monedas
                             orderby d.IDMoneda
                             select d.ClaveMoneda;
                SerLst.Add(" ");
                SerLst.AddRange(SerQry.Distinct());
                ViewBag.Divisa = new SelectList(SerLst);
                ViewBag.DivisaSeleccionada = Divisa;

                ///filtro: Vendedor
                var VenLst = new List<string>();
                var VenQry = from d in dbv.Vendedores
                             orderby d.IDVendedor
                             select d.Nombre;
                VenLst.Add(" ");
                VenLst.AddRange(VenQry.Distinct());
                ViewBag.Vendedor = new SelectList(VenLst);
                ViewBag.VendedorSeleccionado = Vendedor;

                OficinaContext dbof = new OficinaContext();

                ///filtro: Oficina
                var OfLst = new List<string>();
                var OfQry = from d in dbof.Oficinas
                            orderby d.NombreOficina
                            select d.NombreOficina;
                OfLst.Add(" ");
                OfLst.AddRange(OfQry.Distinct());
                ViewBag.Oficina = new SelectList(OfLst);
                ViewBag.OficinaSeleccionada = Oficina;

                ///filtro: Status
                var StaLst = new List<string>();
                var StaQry = from d in dbc.VCotizacionesPros
                             orderby d.ID
                             select d.Estado;
                StaLst.Add(" ");
                StaLst.AddRange(StaQry.Distinct());
                ViewBag.Status = new SelectList(StaLst);
                ViewBag.StatusSeleccionado = Status;

                ///tabla filtro: Divisa
                if (Divisa == " ")
                {
                    Divisa = string.Empty;
                }

                if (!String.IsNullOrEmpty(Divisa))
                {
                    if (FiltroSql == string.Empty)
                    {
                        FiltroSql = "where ClaveMoneda = '" + Divisa + "'";
                    }
                    else
                    {
                        FiltroSql += " and  ClaveMoneda = '" + Divisa + "'";
                    }
                }

                ///tabla filtro: Status
                if (Status == " ")
                {
                    Status = string.Empty;
                }

                if (!String.IsNullOrEmpty(Status))
                {
                    if (FiltroSql == string.Empty)
                    {
                        FiltroSql = "where Estado = '" + Status + "'";
                    }
                    else
                    {
                        FiltroSql += " and  Estado = '" + Status + "'";
                    }
                }

                ///tabla filtro: searchString
                if (searchString == "")
                {
                    searchString = string.Empty;
                }

                if (!String.IsNullOrEmpty(searchString))
                {
                    if (FiltroSql == string.Empty)
                    {
                        FiltroSql = "where ID = " + int.Parse(searchString.ToString()) + "";
                    }
                    else
                    {
                        FiltroSql += " and  ID = " + int.Parse(searchString.ToString()) + "";
                    }

                }


                ///tabla filtro: Cliente
                if (Cliente == "")
                {
                    Cliente = string.Empty;
                }

                if (!String.IsNullOrEmpty(Cliente))
                {
                    if (FiltroSql == string.Empty)
                    {
                        FiltroSql = "where Cliente like '" + Cliente + "%'";
                    }
                    else
                    {
                        FiltroSql += " and  Cliente like '" + Cliente + "%'";
                    }

                }
                ///tabla filtro: Oficina
                if (Oficina == "")
                {
                    Oficina = string.Empty;
                }

                if (!String.IsNullOrEmpty(Oficina))
                {
                    if (FiltroSql == string.Empty)
                    {
                        FiltroSql = "where NombreOficina = '" + Oficina + "'";
                    }
                    else
                    {
                        FiltroSql += " and NombreOficina = '" + Oficina + "'";
                    }

                }

                ///tabla filtro: Vendedor
                if (Vendedor == "")
                {
                    Vendedor = string.Empty;
                }

                if (!String.IsNullOrEmpty(Vendedor))
                {
                    if (FiltroSql == string.Empty)
                    {
                        FiltroSql = "where Nombre = '" + Vendedor + "'";
                    }
                    else
                    {
                        FiltroSql += " and Nombre = '" + Vendedor + "'";
                    }

                }

                if (!String.IsNullOrEmpty(Fechainicio))  //pusieron una fecha
                {
                    if (Fechafinal == string.Empty)
                    {
                        Fechafinal = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Day.ToString();
                    }
                    if (FiltroSql == string.Empty)
                    {
                        FiltroSql = " where  Fecha BETWEEN  '" + Fechainicio + "'   and '" + Fechafinal + "'";
                    }
                    else
                    {
                        FiltroSql += " and  Fecha BETWEEN  '" + Fechainicio + "'   and '" + Fechafinal + "'";
                    }
                }


                ViewBag.CurrentSort = sortOrder;
                ViewBag.DivisaSortParm = String.IsNullOrEmpty(sortOrder) ? "Divisa" : "";
                ViewBag.StatusSortParm = String.IsNullOrEmpty(sortOrder) ? "Estado" : "";
                ViewBag.FechaSortParm = sortOrder == "Fecha" ? "Fecha" : "";
                ViewBag.EmpresaSortParm = String.IsNullOrEmpty(sortOrder) ? "Cliente" : "";
                // Pass filtering string to view in order to maintain filtering when paging
                ViewBag.Fechainicio = Fechainicio;
                ViewBag.Fechafinal = Fechafinal;

                switch (sortOrder)
                {
                    case "Cotizacion":
                        OrdenSql = " order by  ID asc ";
                        break;

                    case "Cliente":
                        OrdenSql = " order by  Cliente, ID desc ";
                        break;

                    default:
                        OrdenSql = " order by ID desc ";
                        break;
                }



                ///tabla filtro: FechaInicial

                if (FiltroSql == string.Empty)
                {
                    FiltroSql = " where Fecha >  DATEADD(mm, -1, getdate()) and fecha <=  getdate() ";
                }

                CadenaSql = ConsultaSql + " " + FiltroSql + " " + OrdenSql;

                var elementos = dbv.Database.SqlQuery<VCotizacionesPros>(CadenaSql).ToList();


                ViewBag.sumatoria = "";
                try
                {

                    var SumaLst = new List<string>();


                    FiltroSuma = FiltroSql + " " + FiltroSuma;
                    var SumaQry = SumaSql + " " + FiltroSuma + " " + GroupSql;
                    List<ResumenFac> data = db.Database.SqlQuery<ResumenFac>(SumaQry).ToList();
                    ViewBag.sumatoria = data;

                }
                catch (Exception err)
                {
                    string mensaje = err.Message;
                }

                //Paginación
                int count = elementos.Count();// Total number of elements
                // Populate DropDownList
                ViewBag.PageSize = new List<SelectListItem>()
            {
                new SelectListItem { Text = "10", Value = "10" },
                new SelectListItem { Text = "25", Value = "25" , Selected = true},
                new SelectListItem { Text = "50", Value = "50" },
                new SelectListItem { Text = "100", Value = "100" },
                new SelectListItem { Text = "Todos", Value = count.ToString() }
             };

                int pageNumber = (page ?? 1);
                int pageSize = (PageSize ?? 25);
                ViewBag.psize = pageSize;


                return View(elementos.ToPagedList(pageNumber, pageSize));
                //Paginación

            }

            catch (Exception err)
            {
                string mensaje = err.Message;

                var reshtml = Server.HtmlEncode(CadenaSql);

                return Content(reshtml);
            }
        }

        public ActionResult Create()
        {
            Enccotizapros cotizacion = new Enccotizapros();
            cotizacion.Fecha = DateTime.Now;
            cotizacion.username = User.Identity.Name;


            List<SelectListItem> ClientesPr = new List<SelectListItem>();
            List<ClientesP> cl = new ArticuloContext().Database.SqlQuery<ClientesP>("select * from ClientesP order by Nombre").ToList();

            foreach (ClientesP y in cl)
            {

                ClientesPr.Add(new SelectListItem { Text = y.Nombre, Value = y.IDClienteP.ToString() });
            }

            //ViewBag.IDClienteP = ClientesPr;

            ViewBag.idprospecto = ClientesPr;

            List<SelectListItem> monedap = new c_MonedaContext().c_Monedas.OrderByDescending(s => s.ClaveMoneda).Select(c => new SelectListItem
            {
                Value = c.IDMoneda.ToString(),
                Text = c.ClaveMoneda
            }).ToList();

            ViewBag.moneda = monedap;

            List<SelectListItem> condiciones = new List<SelectListItem>();

            condiciones.Add(new SelectListItem { Text = "-------------------------", Value = "0" });
            var todoscondiciones = new ClientesContext().CondicionesPagos.ToList();
            if (todoscondiciones != null)
            {
                foreach (var y in todoscondiciones)
                {
                    condiciones.Add(new SelectListItem { Text = y.Descripcion, Value = y.IDCondicionesPago.ToString() });
                }
            }

            ViewBag.condiciones = condiciones;

            List<SelectListItem> vendedor = new List<SelectListItem>();


            vendedor.Add(new SelectListItem { Text = "-------------------------", Value = "0" });
            var todosvendedor = new VendedorContext().Vendedores.ToList();
            if (todosvendedor != null)
            {
                foreach (var y in todosvendedor)
                {
                    vendedor.Add(new SelectListItem { Text = y.Nombre, Value = y.IDVendedor.ToString() });
                }
            }

            ViewBag.vendedor = vendedor;




            return View(cotizacion);
        }

        [HttpPost]
        public ActionResult Create(Enccotizapros elemento, FormCollection coleccion)
        {
            string IDCLIENTE = coleccion.Get("idprospecto");
            try
            {
                CotizacionProspectoContext db = new CotizacionProspectoContext();
                //db.Enccotizaprospecto.Add(elemento);
                //db.SaveChanges();
                string cadenaencabezado = "insert into enccotizapros(idprospecto, idcondiciones, atencion, Vigencia, idmoneda, idvendedor, Fecha,  Subtotal, iva, total, Tipocambio, Estado, Observacion,username) values (";
                cadenaencabezado += elemento.idprospecto + "," + elemento.idcondiciones + ",'" + elemento.atencion + "',";
                cadenaencabezado += elemento.Vigencia + "," + elemento.idMoneda + "," + elemento.idvendedor + ",getdate(),0,0,0,dbo.getTipoCambioCadena(Getdate(),'USD','MXN'), 'Activo','" + elemento.Observacion + "','" + User.Identity.Name + "')";


                db.Database.ExecuteSqlCommand(cadenaencabezado);

            }
            catch (Exception err)
            {
                List<SelectListItem> prospectos = new ClientesPContext().ClientesPs.ToList().OrderBy(s => s.Nombre).Select(c => new SelectListItem
                {
                    Value = c.IDClienteP.ToString(),
                    Text = c.Nombre
                }).ToList();

                ViewBag.prospectos = prospectos;

                List<SelectListItem> monedap = new c_MonedaContext().c_Monedas.Select(c => new SelectListItem
                {
                    Value = c.IDMoneda.ToString(),
                    Text = c.ClaveMoneda
                }).ToList();

                ViewBag.moneda = monedap;

                List<SelectListItem> condiciones = new List<SelectListItem>();

                condiciones.Add(new SelectListItem { Text = "-------------------------", Value = "0" });
                var todoscondiciones = new ClientesContext().CondicionesPagos.ToList();
                if (todoscondiciones != null)
                {
                    foreach (var y in todoscondiciones)
                    {
                        condiciones.Add(new SelectListItem { Text = y.Descripcion, Value = y.IDCondicionesPago.ToString() });
                    }
                }

                ViewBag.condiciones = condiciones;

                List<SelectListItem> vendedor = new List<SelectListItem>();


                vendedor.Add(new SelectListItem { Text = "-------------------------", Value = "0" });
                var todosvendedor = new VendedorContext().Vendedores.ToList();
                if (todosvendedor != null)
                {
                    foreach (var y in todosvendedor)
                    {
                        vendedor.Add(new SelectListItem { Text = y.Nombre, Value = y.IDVendedor.ToString() });
                    }
                }

                ViewBag.vendedor = vendedor;
                return View(elemento);
            }
            try
            {
                int indice = 1;
                try
                {
                    int indicef = db.Database.SqlQuery<ClsDatoEntero>("select MAX(ID)  as Dato FROM enccotizapros ").FirstOrDefault().Dato;
                    indice = indicef;
                }
                catch (Exception err)
                {

                }
                decimal montoiva = 0;
                decimal Subtotal = 0;
                decimal Total = 0;
                decimal miva = decimal.Parse(SIAAPI.Properties.Settings.Default.ValorIVA.ToString());
                foreach (detcotizapro detalle in elemento.conceptos)
                {
                    detalle.Importe = detalle.Precio * detalle.cantidad;
                    detalle.idcotizapros = indice;

                    //if(detalle.iva)
                    //{
                    montoiva += detalle.Importe * miva;
                    //}
                    Subtotal += detalle.Importe;

                    db.Database.ExecuteSqlCommand("insert into detcotizapro (idcotizapros,cantidad,Unidad,concepto,precio,importe,iva) values (" + indice + "," + detalle.cantidad + ",'"+detalle.Unidad+"','" + detalle.Concepto + "'," + detalle.Precio + "," + detalle.Importe + ",'1')");
                }



                montoiva = Math.Round(montoiva, 2);
                Subtotal = Math.Round(Subtotal, 2);
                Total = Subtotal + montoiva;
                db.Database.ExecuteSqlCommand("update enccotizapros set subtotal=" + Subtotal + ",iva=" + montoiva + ",total=" + Total + " where id=" + indice);

                return RedirectToAction("Index");
            }
            catch (Exception ERR)
            {
                return RedirectToAction("Index");
            }
        }

        public ActionResult Details(int id)
        {
            List<detcotizapro> cotizacion = db.Database.SqlQuery<detcotizapro>("select * from  detcotizapro   where idcotizapros=" + id + "").ToList();

            ViewBag.cotizacion = cotizacion;

            Enccotizapros enccotizacion = db.Database.SqlQuery<Enccotizapros>("select * from enccotizapros where id=" + id).FirstOrDefault();

            ClientesP prospecto = new ClientesPContext().ClientesPs.Find(enccotizacion.idprospecto);

            CondicionesPago condiciones = new CondicionesPagoContext().CondicionesPagos.Find(enccotizacion.idcondiciones);

            ViewBag.Prospecto = prospecto;

            ViewBag.Condiciones = condiciones;

            c_Moneda moneda = new c_MonedaContext().c_Monedas.Find(enccotizacion.idMoneda);

            ViewBag.Moneda = moneda;

            Vendedor vendedor = new VendedorContext().Vendedores.Find(enccotizacion.idvendedor);

            ViewBag.Vendedor = vendedor;


            return View(enccotizacion);
        }

        public JsonResult getprospectoblando(string buscar)
        {
            buscar = buscar.Remove(buscar.Length - 1);
            var prospectos = new ClientesPContext().ClientesPs.Where(s => s.Nombre.Contains(buscar)).OrderBy(S => S.Nombre);

            // Populate DropDownList
            List<SelectListItem> opciones = new List<SelectListItem>();

            foreach (ClientesP art in prospectos)
            {
                SelectListItem elemento = new SelectListItem();
                elemento.Text = art.Nombre;
                elemento.Value = art.IDClienteP.ToString();
                opciones.Add(elemento);
            }

            return Json(opciones, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Cancelar(int ID)
        {
            CotizacionContext db = new CotizacionContext();
            db.Database.ExecuteSqlCommand("update enccotizapros set estado ='Cancelada' where id=" + ID);
            return RedirectToAction("Index");
        }


        public ActionResult DescargarPDF(int id)
        {

            Enccotizapros cotiza = new CotizacionProspectoContext().Database.SqlQuery<Enccotizapros>("select* from enccotizapros where id="+id).FirstOrDefault();
            Documentocotizapros x = new Documentocotizapros();

            x.Atencion = cotiza.atencion;
            x.fecha = cotiza.Fecha.ToShortDateString();
            ClientesP clientesP = new ClientesPContext().Database.SqlQuery<ClientesP>("select*from ClientesP where idclientep= "+cotiza.idprospecto).ToList().FirstOrDefault();
            x.prospecto = clientesP.Nombre;
            x.iva = decimal.Parse(cotiza.iva.ToString());
            x.total = decimal.Parse(cotiza.Total.ToString());
            x.subtotal = decimal.Parse(cotiza.Subtotal.ToString());
          //  x.tipo_cambio = cotiza..ToString();
          
            x.folio = cotiza.id.ToString();
            x.Moneda = new c_MonedaContext().c_Monedas.Find(cotiza.idMoneda).Descripcion;

            x.Vigencia = cotiza.Vigencia;
            x.condiciones = new CondicionesPagoContext().CondicionesPagos.Find(cotiza.idcondiciones).Descripcion;
          
            x.Observacion = cotiza.Observacion;
            x.Vendedor = new VendedorContext().Vendedores.Find(cotiza.idvendedor).Nombre;
            x.IDPedido = cotiza.id;


            EmpresaContext dbe = new EmpresaContext();
            Empresa empresa = dbe.empresas.Find(2);

            List<detcotizapro> detalles = db.Database.SqlQuery<detcotizapro>("select * from [dbo].[detcotizapro] where IDCotizapros=" + id).ToList();

            int contador = 1;
            foreach (var item in detalles)
            {
                ProductoCotizapros producto = new ProductoCotizapros();
               
                // c_ClaveProductoServicio claveprodsat = db.Database.SqlQuery<c_ClaveProductoServicio>("select c_ClaveProductoServicio.* from (Articulo inner join Familia on articulo.IDFamilia= Familia.IDFamilia) inner join c_ClaveProductoServicio on c_ClaveProductoServicio.IDProdServ= Familia.IDProdServ where Articulo.IDArticulo= " + item.IDArticulo).ToList()[0];
             
                producto.cantidad = item.cantidad.ToString();
                producto.descripcion = item.Concepto;
                producto.Unidad = item.Unidad;
                producto.valorUnitario = float.Parse(item.Precio.ToString());
               
                producto.importe = float.Parse(item.Importe.ToString());

             
                contador++;

                x.productos.Add(producto);

            }
            //CreaCotizaprosPDF documentop = new CreaCotizaprosPDF(logoempresa, x);
            System.Drawing.Image logoempresa = byteArrayToImage(empresa.Logo);
            string contentType = System.Net.Mime.MediaTypeNames.Application.Pdf;
            try
            {
                CreaCotizaprosPDF documento = new CreaCotizaprosPDF(logoempresa, x);
                return new FilePathResult(documento.nombreDocumento, contentType);
            }
            catch (Exception err)
            {
                string mensaje = err.Message;
            }
            return RedirectToAction("Index");
        }
        public ActionResult Edit(int id)

        {
            List<detcotizapro> cotizacion = db.Database.SqlQuery<detcotizapro>("select * from  detcotizapro   where idcotizapros=" + id + "").ToList();

            ViewBag.cotizacion = cotizacion;

            Enccotizapros enccotizacion = db.Database.SqlQuery<Enccotizapros>("select * from enccotizapros where id=" + id).FirstOrDefault();

            ClientesP prospecto = new ClientesPContext().ClientesPs.Find(enccotizacion.idprospecto);

            ViewBag.Prospecto = prospecto;

            CondicionesPago condicionesre = new CondicionesPagoContext().CondicionesPagos.Find(enccotizacion.idcondiciones);

            List<SelectListItem> condiciones = new List<SelectListItem>();


            condiciones.Add(new SelectListItem { Text = condicionesre.Descripcion, Value = condicionesre.IDCondicionesPago.ToString()});
            var todoscondiciones = new ClientesContext().CondicionesPagos.ToList();
            if (todoscondiciones != null)
            {
                foreach (var y in todoscondiciones)
                {
                    condiciones.Add(new SelectListItem { Text = y.Descripcion, Value = y.IDCondicionesPago.ToString() });
                }
            }

            ViewBag.condiciones = condiciones;
            ViewBag.Condiciones = condiciones;

            c_Moneda moneda = new c_MonedaContext().c_Monedas.Find(enccotizacion.idMoneda);

            List<SelectListItem> Monedas = new List<SelectListItem>();


            Monedas.Add(new SelectListItem { Text = moneda.ClaveMoneda, Value = moneda.IDMoneda.ToString() });
            var todasLasMonedas = new ClientesContext().c_Monedas.ToList();
            if (todasLasMonedas != null)
            {
                foreach (var y in todasLasMonedas)
                {
                    Monedas.Add(new SelectListItem { Text = y.ClaveMoneda, Value = y.IDMoneda.ToString() });
                }
            }

           

          
            ViewBag.Moneda = Monedas;

            Vendedor vendedor = new VendedorContext().Vendedores.Find(enccotizacion.idvendedor);

            ViewBag.Vendedor = vendedor;



            return View(enccotizacion);
        }

        [HttpPost]
        public ActionResult Edit(FormCollection colleccion)

        {
            int idcotiza = int.Parse(colleccion.Get("id"));
            int idcondiciones = int.Parse(colleccion.Get("idcondiciones"));
            string observaciones = colleccion.Get("observacion").ToString();
            string Atencion = colleccion.Get("Atencion").ToString();

            CotizacionContext db = new CotizacionContext();
            db.Database.ExecuteSqlCommand("update enccotizapros set idcondiciones=" + idcondiciones + ",observacion='" + observaciones + "', atencion='" + Atencion + "' where id=" + idcotiza);
            int cuantos =int.Parse(( (colleccion.Keys.Count - 5) / 4).ToString());

            decimal acuiva = 0;
            decimal acuimporte = 0;
            for (int i=1;i<=cuantos;i++)
            {
                string llaveid = "id" + i.ToString();
                int iddetalle = int.Parse(colleccion.Get(llaveid).ToString());
                decimal cantidad = decimal.Parse(colleccion.Get("cantidad" + i.ToString()));
                decimal precio = decimal.Parse(colleccion.Get("Precio" + i.ToString()));
                string concepto = colleccion.Get("concepto" + i.ToString());
                decimal importe = cantidad * precio;
                acuimporte = acuimporte + (Math.Round(importe, 2));
                decimal iva=Math.Round(importe* decimal.Parse(SIAAPI.Properties.Settings.Default.ValorIVA),2);
                acuiva = acuiva + iva;
                db.Database.ExecuteSqlCommand("update detcotizapro set cantidad=" + cantidad + ",precio=" + precio + ",importe=" + importe + ", concepto='"+concepto +"' where id=" + iddetalle);
            }

            db.Database.ExecuteSqlCommand("update enccotizapros set subtotal= " + acuimporte + ", iva=" + acuiva + ", total=" + (acuiva + acuimporte) + " where id=" + idcotiza);
                
            return RedirectToAction("index",new { SearchString= idcotiza});
        }

        public ActionResult EntreFechasCotPros()
        {
            EFecha elemento = new EFecha();

            return View(elemento);
        }

        [HttpPost]
        public ActionResult EntreFechasCotPros(EFecha modelo, FormCollection coleccion)
        {
            VCotizacionesContext dbe = new VCotizacionesContext();
            VDetCotizacionesContext dbr = new VDetCotizacionesContext();
            string FI = modelo.fechaini.Year.ToString() + "-" + modelo.fechaini.Month.ToString() + "-" + modelo.fechaini.Day.ToString();
            string FF = modelo.fechafin.Year.ToString() + "-" + modelo.fechafin.Month.ToString() + "-" + modelo.fechafin.Day.ToString();

            string cual = coleccion.Get("Enviar");

            string cadena = "";
            string cadenaDet = "";
            if (cual == "Generar reporte")
            {
                return View();
            }
            if (cual == "Generar excel")
            {

                cadena = "select * from dbo.VCotizacionesPros where fecha >= '" + FI + "' and fecha  <='" + FF + "' ";
                var datos = dbe.Database.SqlQuery<VCotizacionesPros>(cadena).ToList();
                ViewBag.datos = datos;

                cadenaDet = "select * from [dbo].[VDetCotizacionePros] where fecha >= '" + FI + "' and fecha  <='" + FF + "' ";
                var datosDet = dbr.Database.SqlQuery<VDetCotizacionePros>(cadenaDet).ToList();
                ViewBag.datosDet = datosDet;

                ExcelPackage Ep = new ExcelPackage();
                //Crear la hoja y poner el nombre de la pestaña del libro
                var Sheet = Ep.Workbook.Worksheets.Add("Cotizaciones Prospecto");

                // en la fila1 formateo las celdas y coloco el título de la hoja
                // RichText permite agregar las propiedades de tipo de letra: color, negrita, etc.
                int row = 1;
                //Fijar la fuente para A1:Q1
                Sheet.Cells["A1:O1"].Style.Font.Size = 20;
                Sheet.Cells["A1:O1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:O3"].Style.Font.Bold = true;
                Sheet.Cells["A1:O1"].Style.Font.Color.SetColor(Color.DarkBlue);
                Sheet.Cells["A1:O1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                Sheet.Cells["A1"].RichText.Add("Listado de Cotizaciones de Prospectos");

                row = 2;
                Sheet.Cells["A1:O1"].Style.Font.Size = 12;
                Sheet.Cells["A1:O1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:O1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
                Sheet.Cells["A2:O2"].Style.Font.Bold = true;
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
                Sheet.Cells["A3"].RichText.Add("ID");
                Sheet.Cells["A3"].RichText.Add("ID Cotización");
                Sheet.Cells["B3"].RichText.Add("Fecha");
                Sheet.Cells["C3"].RichText.Add("Se cotizo a");
                Sheet.Cells["D3"].RichText.Add("Cliente");
                Sheet.Cells["E3"].RichText.Add("Subtotal");
                Sheet.Cells["F3"].RichText.Add("IVA");
                Sheet.Cells["G3"].RichText.Add("Total");
                Sheet.Cells["H3"].RichText.Add("TipoCambio");
                Sheet.Cells["I3"].RichText.Add("Moneda");
                Sheet.Cells["J3"].RichText.Add("TotalPesos");
                Sheet.Cells["K3"].RichText.Add("Condiciones de Pago");
                Sheet.Cells["L3"].RichText.Add("Vendedor");
                Sheet.Cells["M3"].RichText.Add("Oficina");
                Sheet.Cells["N3"].RichText.Add("Estado");
                Sheet.Cells["O3"].RichText.Add("Observación");
                //Aplicar borde doble al rango de celdas A3:Q3
                Sheet.Cells["A3:O3"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;

                // en la fila4, el foreach llenara cada fila de la hoja de excel, con cada fila que trae de la bd.
                // Se establecen los formatos para las celdas: Fecha, Moneda
                row = 4;
                Sheet.Cells.Style.Font.Bold = false;
                foreach (VCotizacionesPros item in ViewBag.datos)
                {
                    Sheet.Cells[string.Format("A{0}", row)].Value = item.ID;
                    Sheet.Cells[string.Format("B{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    Sheet.Cells[string.Format("B{0}", row)].Value = item.Fecha;
                    if (item.idprospecto == 0)
                    {
                        Sheet.Cells[string.Format("C{0}", row)].Value = "Cliente";
                    }
                    else
                    {
                        Sheet.Cells[string.Format("C{0}", row)].Value = "Prospecto";
                    }
                    Sheet.Cells[string.Format("D{0}", row)].Value = item.Cliente;
                    Sheet.Cells[string.Format("E{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("E{0}", row)].Value = item.Subtotal;
                    Sheet.Cells[string.Format("F{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("F{0}", row)].Value = item.IVA;
                    Sheet.Cells[string.Format("G{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("G{0}", row)].Value = item.Total;
                    Sheet.Cells[string.Format("H{0}", row)].Style.Numberformat.Format = "##0.00";
                    Sheet.Cells[string.Format("H{0}", row)].Value = item.TipoCambio;
                    Sheet.Cells[string.Format("I{0}", row)].Value = item.ClaveMoneda;
                    Sheet.Cells[string.Format("J{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("J{0}", row)].Value = item.TotalPesos;
                    Sheet.Cells[string.Format("K{0}", row)].Value = item.CondicionesPago;
                    Sheet.Cells[string.Format("L{0}", row)].Value = item.Nombre;
                    Sheet.Cells[string.Format("M{0}", row)].Value = item.NombreOficina;
                    Sheet.Cells[string.Format("N{0}", row)].Value = item.Estado;
                    Sheet.Cells[string.Format("O{0}", row)].Value = item.Observacion;
                    row++;
                }

                //Hoja No. 2
                Sheet = Ep.Workbook.Worksheets.Add("Detalles");
                row = 1;
                //Fijar la fuente para A1:Q1
                Sheet.Cells["A1:M1"].Style.Font.Size = 20;
                Sheet.Cells["A1:M1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:M3"].Style.Font.Bold = true;
                Sheet.Cells["A1:M1"].Style.Font.Color.SetColor(Color.DarkBlue);
                Sheet.Cells["A1:M1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                Sheet.Cells["A1"].RichText.Add("Detalle de Cotizaciones de Prospectos");

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
                Sheet.Cells["A3"].RichText.Add("ID");
                Sheet.Cells["B3"].RichText.Add("ID Cotización");
                Sheet.Cells["C3"].RichText.Add("Fecha");
                Sheet.Cells["D3"].RichText.Add("Se cotizo a");
                Sheet.Cells["E3"].RichText.Add("Cliente");
                Sheet.Cells["F3"].RichText.Add("Cantidad");
                Sheet.Cells["G3"].RichText.Add("Unidad");
                Sheet.Cells["H3"].RichText.Add("Concepto");
                Sheet.Cells["I3"].RichText.Add("Precio");
                Sheet.Cells["J3"].RichText.Add("Importe");
                Sheet.Cells["K3"].RichText.Add("IVA");
                Sheet.Cells["L3"].RichText.Add("Total");
                Sheet.Cells["M3"].RichText.Add("Estado");

                //Aplicar borde doble al rango de celdas A3:Q3
                Sheet.Cells["A3:M3"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;

                // en la fila4, el foreach llenara cada fila de la hoja de excel, con cada fila que trae de la bd.
                // Se establecen los formatos para las celdas: Fecha, Moneda
                row = 4;
                Sheet.Cells.Style.Font.Bold = false;
                foreach (VDetCotizacionePros itemD in ViewBag.datosDet)
                {
                    Sheet.Cells[string.Format("A{0}", row)].Value = itemD.ID;
                    Sheet.Cells[string.Format("B{0}", row)].Value = itemD.IDCotiza;
                    Sheet.Cells[string.Format("C{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    Sheet.Cells[string.Format("C{0}", row)].Value = itemD.Fecha;
                    if (itemD.IDProspecto == 0)
                    {
                        Sheet.Cells[string.Format("D{0}", row)].Value = "Cliente";
                    }
                    else
                    {
                        Sheet.Cells[string.Format("D{0}", row)].Value = "Prospecto";
                    }
                    Sheet.Cells[string.Format("E{0}", row)].Value = itemD.Cliente;




                    Sheet.Cells[string.Format("F{0}", row)].Value = itemD.cantidad;
                    Sheet.Cells[string.Format("G{0}", row)].Value = itemD.Unidad;
                    Sheet.Cells[string.Format("H{0}", row)].Value = itemD.Concepto;
                    Sheet.Cells[string.Format("I{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("I{0}", row)].Value = itemD.Precio;
                    Sheet.Cells[string.Format("J{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("J{0}", row)].Value = itemD.Importe;
                    Sheet.Cells[string.Format("K{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("K{0}", row)].Value = itemD.IVA;
                    Sheet.Cells[string.Format("L{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("L{0}", row)].Value = itemD.Total;
                    Sheet.Cells[string.Format("M{0}", row)].Value = itemD.Estado;
                    row++;
                }
                //Se genera el archivo y se descarga

                Sheet.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment: filename=" + "Recepcion.xlsx");
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
    }
}