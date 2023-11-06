using OfficeOpenXml;
using OfficeOpenXml.Style;
using PagedList;
using SIAAPI.Models.Administracion;
using SIAAPI.Models.Cfdi;
using SIAAPI.Models.Comercial;
using SIAAPI.Models.Login;
using SIAAPI.Reportes;
using SIAAPI.ViewModels.Cfdi;
using SIAAPI.ViewModels.Comercial;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace SIAAPI.Controllers.Cfdi
{
[Authorize(Roles = "Almacenista,Administrador,Compras,Sistemas,Facturacion, Proveedor,Contabilidad")]
    public class FacturaProvController : Controller
    {
        // GET: FacturaProv
        // GET: FacturaComplemento

        EncfacturaProvContext db = new EncfacturaProvContext();
        VEncFacturaProvSaldoContext dbv = new VEncFacturaProvSaldoContext();
        public ActionResult Index(string Numero, string ClieFac, string sortOrder, string currentFilter, int? page, int? PageSize, string searchString, string FacPag, string ticket, int idProveedor = 0, string Estado = "A")
        {


            if (Session["Proveedor"] != null)
            {
                RedirectToAction("Indexp");
            }

            //Buscar Facturas: Pagadas o no pagadas
            var FacPagLst = new List<SelectListItem>();


            var EstadoLst = new List<SelectListItem>();
            //FacPagLst.Add(new SelectListItem { Text = "Todos", Value = "na", Selected = true });

            EstadoLst.Add(new SelectListItem { Text = "Cancelada", Value = "C" });
            EstadoLst.Add(new SelectListItem { Text = "Activas", Value = "A" });

            ViewData["Estado"] = EstadoLst;
            ViewBag.Estado = new SelectList(EstadoLst, "Value", "Text");

            //Facturas Pagadas
            FacPagLst.Add(new SelectListItem { Text = "Pagada", Value = "SI" });
            FacPagLst.Add(new SelectListItem { Text = "NoPagada", Value = "NO" });
            FacPagLst.Add(new SelectListItem { Text = "SinRevisión", Value = "SR" });

            ViewData["FacPag"] = FacPagLst;
            ViewBag.FacPag = new SelectList(FacPagLst, "Value", "Text");


            ViewBag.sumatoria = "";

            //Buscar proveedor
            ProveedorContext prov = new ProveedorContext();
            var proveedor = prov.Proveedores.OrderBy(s => s.Empresa).ToList();
            List<SelectListItem> li = new List<SelectListItem>();
            li.Add(new SelectListItem { Text = "--Selecciona Proveedor--", Value = "0" });

            foreach (var m in proveedor)
            {
                li.Add(new SelectListItem { Text = m.Empresa, Value = m.IDProveedor.ToString() });


            }
            ViewBag.proveedor = li;

            var ClieLst = new List<string>();
            var ClieQry = from d in db.EncfacturaProveedores
                          orderby d.Nombre_Proveedor
                          select d.Nombre_Proveedor;
            ClieLst.AddRange(ClieQry.Distinct());
            ViewBag.ClieFac = new SelectList(ClieLst);
            int proveedore = idProveedor;
            ViewBag.idProveedorSeleccionado = idProveedor;
            string ConsultaSql = "select top 200 * from VEncFacturaProvSaldo ";
            string ConsultaSqlResumen = "select Moneda as MonedaOrigen, (SUM(Subtotal)) as Subtotal, (SUM(IVA)) as IVA, (SUM(Total)) as Total, (SUM(Total * TC)) as TotalenPesos from VEncFacturaProvSaldo ";
            string ConsultaAgrupado = "group by Moneda order by Moneda ";
            string Filtro = string.Empty;
            string Orden = " order by fecha desc , Nombre_Proveedor  ";

            if (!String.IsNullOrEmpty(ticket))
            {
                if (Filtro == string.Empty)
                {
                    Filtro = " where ID=" + ticket + "";
                }
                else
                {
                    Filtro += " and  ID=" + ticket + "";
                }

            }

            //Buscar por numero
            if (!String.IsNullOrEmpty(Numero))
            {
                if (Filtro == string.Empty)
                {
                    Filtro = " where Numero='" + Numero + "'";
                }
                else
                {
                    Filtro += " and  Numero='" + Numero + "'";
                }

            }

            ///tabla filtro: Nombre Cliente
            ///

            if (idProveedor != 0)
            {
                Proveedor pro = new ProveedorContext().Proveedores.Find(idProveedor);

                //EncFacProveedor encp = new EncfacturaProvContext().EncfacturaProveedores.Find(pro.IDProveedor);

                if (Filtro == string.Empty)
                {
                    Filtro = "where idProveedor='" + idProveedor + "'";
                }
                else
                {
                    Filtro += "and idProveedor ='" + idProveedor + "'";
                }

            }
            //if (!String.IsNullOrEmpty(ClieFac))
            //{

            //    if (Filtro == string.Empty)
            //    {
            //        Filtro = "where Nombre_Proveedor='" + ClieFac + "'";
            //    }
            //    else
            //    {
            //        Filtro += "and  Nombre_Proveedor='" + ClieFac + "'";
            //    }

            //}

            ///tabla filtro: Factura pagada/no pagada && Serie, Nombre Cliente
            if (FacPag != "Todas")
            {
                if (FacPag == "SI")
                {
                    if (Filtro == string.Empty)
                    {
                        Filtro = "where pagada='1' ";
                    }
                    else
                    {
                        Filtro += "and  pagada='1' ";
                    }
                }
                if (FacPag == "NO")
                {
                    if (Filtro == string.Empty)
                    {
                        Filtro = "where pagada='0' ";
                    }
                    else
                    {
                        Filtro += "and  pagada='0' ";
                    }
                }
                if (FacPag == "SR")
                {
                    if (Filtro == string.Empty)
                    {
                        Filtro = "where ImportePagado is null ";
                    }
                    else
                    {
                        Filtro += "and ImportePagado is null ";
                    }
                }
            }

            if (Estado != "Todos")
            {
                if (Estado == "C")
                {
                    if (Filtro == string.Empty)
                    {
                        Filtro = "where Estado='C'";
                    }
                    else
                    {
                        Filtro += "and  Estado='C'";
                    }
                }
                if (Estado == "A")
                {
                    if (Filtro == string.Empty)
                    {
                        Filtro = "where  Estado='A'";
                    }
                    else
                    {
                        Filtro += "and Estado='A'";
                    }
                }
            }



            if (!String.IsNullOrEmpty(searchString)) //pusieron una fecha
            {
                if (Filtro == string.Empty)
                {
                    Filtro = "where  Fecha BETWEEN '" + searchString + "' and '" + searchString + " 23:59:59.999' ";
                }
                else
                {
                    Filtro += " and Fecha BETWEEN '" + searchString + "' and '" + searchString + " 23:59:59.999'";
                }
            }


            ViewBag.CurrentSort = sortOrder;
            ViewBag.IDSortParm = "ID";
            ViewBag.NumeroSortParm = String.IsNullOrEmpty(sortOrder) ? "Numero" : "";
            ViewBag.FechaSortParm = sortOrder == "Fecha" ? "Fecha" : "";
            ViewBag.ClienteSortParm = String.IsNullOrEmpty(sortOrder) ? "Nombre_Proveedor" : "";


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


            //Ordenacion

            switch (sortOrder)
            {

                case "ID":
                    Orden = " order by ID desc ";
                    break;
                case "Numero":
                    Orden = " order by numero desc ";
                    break;
                case "Fecha":
                    Orden = " order by fecha desc ";
                    break;
                case "Nombre_Cliente":
                    Orden = " order by  Nombre_Proveedor ";
                    break;
                default:
                    Orden = " order by fecha desc ,  Nombre_Proveedor  ";
                    break;
            }

            //var elementos = from s in db.encfacturas
            //select s;
            string cadenaSQl = ConsultaSql + " " + Filtro + " " + Orden;

            var elementos = dbv.Database.SqlQuery<VEncFacturaProvSaldo>(cadenaSQl).ToList();



            ViewBag.sumatoria = "";
            try
            {

                var SumaLst = new List<string>();
                var SumaQry = ConsultaSqlResumen + " " + Filtro + " " + ConsultaAgrupado;
                List<ResumenFac> data = db.Database.SqlQuery<ResumenFac>(SumaQry).ToList();
                ViewBag.sumatoria = data;

            }
            catch (Exception err)
            {

            }

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = dbv.VEncFacturaProvSaldo.OrderBy(e => e.ID).Count();// Total number of elements

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
        public ActionResult RutaPDF(int id)
        {

            EncfacturaProv Ordencompra = db.EncfacturaProveedores.Find(id);
            DocumentoRutaOrdenCompraF x = new DocumentoRutaOrdenCompraF();
            Proveedor Proveedor = new ProveedorContext().Proveedores.Find(Ordencompra.IDProveedor);
            CondicionesPago condiciones = new CondicionesPagoContext().CondicionesPagos.Find(Proveedor.IDCondicionesPago);
            c_FormaPago pago = new c_FormaPagoContext().c_FormaPagos.Where(s => s.ClaveFormaPago == Ordencompra.Formadepago).FirstOrDefault();

            x.claveMoneda = Ordencompra.c_Moneda.Descripcion;
            x.descuento = 0;
            x.fecha = Ordencompra.Fecha.ToShortDateString();
            x.fechaRevision = Ordencompra.FechaRevision.ToShortDateString();
            x.fechaVencimiento = Ordencompra.FechaVencimiento.ToShortDateString();
            x.dias = condiciones.DiasCredito;
            //x.fechaRequerida = Ordencompra.FechaRequiere.ToShortDateString();
            x.Proveedor = Ordencompra.Nombre_Proveedor;
            x.formaPago = pago.ClaveFormaPago;
            x.metodoPago = Ordencompra.Metododepago;
            x.RFCproveedor = Proveedor.Telefonouno;
            x.total = float.Parse(Ordencompra.Total.ToString());
            x.subtotal = float.Parse(Ordencompra.Subtotal.ToString());
            //x.Entregaren = Ordencompra.Entregaren;
            //  x.tipo_cambio = Ordencompra.TipoCambio.ToString();
            x.tipo_cambio =
              x.serie = "";
            x.folio = Ordencompra.ID.ToString();
            //x.UsodelCFDI = Ordencompra.c_UsoCFDI.Descripcion;
            //x.IDALmacen = Ordencompra.Almacen.IDAlmacen;
            x.Telefonoproveedor = Proveedor.Telefonouno;
            //x.condicionesdepago = Ordencompra.CondicionesPago.Descripcion;
            //x.Observacion = Ordencompra.Observacion;
            //x.Autorizado = Ordencompra.Status;
            x.DireccionProveedor = x.DireccionProveedor = Proveedor.Calle + " " + Proveedor.NoExt + " " + Proveedor.NoInt + "," + Proveedor.Colonia + "," + Proveedor.Municipio + "," + Proveedor.Estados.Estado + "," + Proveedor.paises.Pais;

            ImpuestoRutaOCF iva = new ImpuestoRutaOCF();
            iva.impuesto = "IVA";
            iva.tasa = 16;
            iva.importe = float.Parse(Ordencompra.IVA.ToString());


            x.impuestos.Add(iva);

            EmpresaContext dbe = new EmpresaContext();

            var empresa = dbe.empresas.Single(m => m.IDEmpresa == 2);
            x.Empresa = empresa.RazonSocial;
            x.Telefono = empresa.Telefono;
            x.RFC = empresa.RFC;
            x.Direccion = empresa.Calle + " " + empresa.NoExt + " " + empresa.NoInt + "\n" + empresa.Colonia + " \n" + empresa.Municipio + "," + empresa.Estados.Estado;
            x.firmadefinanzas = empresa.Director_finanzas;
            x.firmadecompras = empresa.Persona_de_Compras + "";
            //List<int> Factura = new EncfacturaProvContext().Database.SqlQuery<int>("select distinct(id) from  EncFacturaProv where ID= " + id).ToList();

            EncRecepcion recepciones = new RecepcionContext().Database.SqlQuery<EncRecepcion>("select * from  EncRecepcion where documentoFactura= '" + id + "'").ToList().FirstOrDefault();

            List<DetRecepcion> detalles = new PrefacturaContext().Database.SqlQuery<DetRecepcion>("Select * from detRecepcion where idrecepcion=" + recepciones.IDRecepcion + " and status!='Cancelado'").ToList();
            foreach (DetRecepcion detalle in detalles)
            {
                List<DetOrdenCompra> detallesO = db.Database.SqlQuery<DetOrdenCompra>("select * from [dbo].[DetOrdenCompra] where [IDDetOrdenCompra]= " + detalle.IDDetExterna).ToList();

                int contador = 1;
                foreach (DetOrdenCompra item in detallesO)
                {
                    ProductoRutaOCF producto = new ProductoRutaOCF();
                    Articulo arti = new ArticuloContext().Articulo.Find(item.IDArticulo);

                    c_ClaveProductoServicio claveprodsat = db.Database.SqlQuery<c_ClaveProductoServicio>("select c_ClaveProductoServicio.* from (Articulo inner join Familia on articulo.IDFamilia= Familia.IDFamilia) inner join c_ClaveProductoServicio on c_ClaveProductoServicio.IDProdServ= Familia.IDProdServ where Articulo.IDArticulo= " + item.IDArticulo).ToList()[0];
                    //  producto.ClaveProducto = claveprodsat.ClaveProdServ;
                    producto.ClaveProducto = arti.Cref;
                    producto.c_unidad = arti.c_ClaveUnidad.Nombre;
                    producto.cantidad = item.Cantidad.ToString();
                    producto.iddetordencompra = item.IDDetOrdenCompra;
                    producto.descripcion = arti.Descripcion;
                    producto.valorUnitario = float.Parse(item.Costo.ToString());
                    producto.v_unitario = float.Parse(item.Costo.ToString());
                    producto.importe = float.Parse(item.Importe.ToString());
                    producto.Tipo = new ArticuloContext().TipoArticulo.Find(arti.IDTipoArticulo).Descripcion.ToString();
                    ///
                    producto.suministro = item.Suministro;
                    Caracteristica caracteristica = new ArticuloContext().Database.SqlQuery<Caracteristica>("SELECT * FROM Caracteristica where ID=" + item.Caracteristica_ID).ToList().FirstOrDefault();

                    producto.Presentacion = item.Presentacion; //item.presentacion;
                    producto.Observacion = item.Nota;
                    ///
                    producto.numIdentificacion = contador.ToString();
                    contador++;

                    x.productos.Add(producto);

                }
            }




            System.Drawing.Image logoempresa = byteArrayToImage(empresa.Logo);

            try
            {


                CreaRutaOrdenCompraPDFF documento = new CreaRutaOrdenCompraPDFF(logoempresa, x);

            }
            catch (Exception err)
            {
                String mensaje = err.Message;
            }
            return RedirectToAction("Index");

        }

        //     public ActionResult Index(string Numero, string ClieFac, string sortOrder, string currentFilter, int? page, int? PageSize, string searchString, string FacPag, string ticket,int idProveedor=0,string Estado = "A")
        //     {


        //         if (Session["Proveedor"] != null)
        //         {
        //             RedirectToAction("Indexp");
        //         }

        //         //Buscar Facturas: Pagadas o no pagadas
        //         var FacPagLst = new List<SelectListItem>();


        //         var EstadoLst = new List<SelectListItem>();
        //         //FacPagLst.Add(new SelectListItem { Text = "Todos", Value = "na", Selected = true });

        //         EstadoLst.Add(new SelectListItem { Text = "Cancelada", Value = "C" });
        //         EstadoLst.Add(new SelectListItem { Text = "Activas", Value = "A" });

        //         ViewData["Estado"] = EstadoLst;
        //         ViewBag.Estado = new SelectList(EstadoLst, "Value", "Text");

        //         //Facturas Pagadas
        //         FacPagLst.Add(new SelectListItem { Text = "Pagada", Value = "SI" });
        //         FacPagLst.Add(new SelectListItem { Text = "NoPagada", Value = "NO" });

        //         ViewData["FacPag"] = FacPagLst;
        //         ViewBag.FacPag = new SelectList(FacPagLst, "Value", "Text");


        //         ViewBag.sumatoria = "";

        //         //Buscar proveedor
        //         ProveedorContext prov = new ProveedorContext();
        //         var proveedor = prov.Proveedores.OrderBy(s => s.Empresa).ToList();
        //         List<SelectListItem> li = new List<SelectListItem>();
        //         li.Add(new SelectListItem { Text = "--Selecciona Proveedor--", Value = "0" });

        //         foreach (var m in proveedor)
        //         {
        //             li.Add(new SelectListItem { Text = m.Empresa, Value = m.IDProveedor.ToString() });


        //         }
        //         ViewBag.proveedor = li;
        //         var ClieLst = new List<string>();
        //         var ClieQry = from d in db.EncfacturaProveedores
        //                       orderby d.Nombre_Proveedor
        //                       select d.Nombre_Proveedor;
        //         ClieLst.AddRange(ClieQry.Distinct());
        //         ViewBag.ClieFac = new SelectList(ClieLst);
        //int proveedore = idProveedor;

        //         string ConsultaSql = "select * from VEncFacturaProvSaldo ";
        //         string ConsultaSqlResumen = "select Moneda as MonedaOrigen, (SUM(Subtotal)) as Subtotal, (SUM(IVA)) as IVA, (SUM(Total)) as Total, (SUM(Total * TC)) as TotalenPesos from VEncFacturaProvSaldo ";
        //         string ConsultaAgrupado = "group by Moneda order by Moneda ";
        //         string Filtro = string.Empty;
        //         string Orden = " order by fecha desc , Nombre_Proveedor  ";

        //         if (!String.IsNullOrEmpty(ticket))
        //         {
        //             if (Filtro == string.Empty)
        //             {
        //                 Filtro = " where ID=" + ticket + "";
        //             }
        //             else
        //             {
        //                 Filtro += " and  ID=" + ticket + "";
        //             }

        //         }

        //         //Buscar por numero
        //         if (!String.IsNullOrEmpty(Numero))
        //         {
        //             if (Filtro == string.Empty)
        //             {
        //                 Filtro = " where Numero='" + Numero + "'";
        //             }
        //             else
        //             {
        //                 Filtro += " and  Numero='" + Numero + "'";
        //             }

        //         }

        //         ///tabla filtro: Nombre Cliente
        //         ///

        //         if (idProveedor !=0)
        //         {
        //             Proveedor pro = new ProveedorContext().Proveedores.Find(idProveedor);

        //             //EncFacProveedor encp = new EncfacturaProvContext().EncfacturaProveedores.Find(pro.IDProveedor);

        //             if (Filtro == string.Empty)
        //             {
        //                 Filtro = "where idProveedor='" + idProveedor + "'";
        //             }
        //             else
        //             {
        //                 Filtro += "and idProveedor ='" + idProveedor + "'";
        //             }

        //         }


        //         ///tabla filtro: Factura pagada/no pagada && Serie, Nombre Cliente
        //         if (FacPag != "Todas")
        //         {
        //             if (FacPag == "SI")
        //             {
        //                 if (Filtro == string.Empty)
        //                 {
        //                     Filtro = "where pagada='1' ";
        //                 }
        //                 else
        //                 {
        //                     Filtro += "and  pagada='1' ";
        //                 }
        //             }
        //             if (FacPag == "NO")
        //             {
        //                 if (Filtro == string.Empty)
        //                 {
        //                     Filtro = "where pagada='0' ";
        //                 }
        //                 else
        //                 {
        //                     Filtro += "and  pagada='0' ";
        //                 }
        //             }
        //         }


        //         if (Estado != "Todos")
        //         {
        //             if (Estado == "C")
        //             {
        //                 if (Filtro == string.Empty)
        //                 {
        //                     Filtro = "where Estado='C'";
        //                 }
        //                 else
        //                 {
        //                     Filtro += "and  Estado='C'";
        //                 }
        //             }
        //             if (Estado == "A")
        //             {
        //                 if (Filtro == string.Empty)
        //                 {
        //                     Filtro = "where  Estado='A'";
        //                 }
        //                 else
        //                 {
        //                     Filtro += "and Estado='A'";
        //                 }
        //             }
        //         }



        //         if (!String.IsNullOrEmpty(searchString)) //pusieron una fecha
        //         {
        //             if (Filtro == string.Empty)
        //             {
        //                 Filtro = "where  Fecha BETWEEN '" + searchString + "' and '" + searchString + " 23:59:59.999' ";
        //             }
        //             else
        //             {
        //                 Filtro += " and Fecha BETWEEN '" + searchString + "' and '" + searchString + " 23:59:59.999'";
        //             }
        //         }


        //         ViewBag.CurrentSort = sortOrder;
        //         ViewBag.IDSortParm = "ID";
        //         ViewBag.NumeroSortParm = String.IsNullOrEmpty(sortOrder) ? "Numero" : "";
        //         ViewBag.FechaSortParm = sortOrder == "Fecha" ? "Fecha" : "";
        //         ViewBag.ClienteSortParm = String.IsNullOrEmpty(sortOrder) ? "Nombre_Proveedor" : "";


        //         // Not sure here
        //         if (searchString == null)
        //         {
        //             searchString = currentFilter;
        //         }

        //         // Pass filtering string to view in order to maintain filtering when paging
        //         ViewBag.SearchString = searchString;

        //         //Paginación
        //         if (searchString != null)
        //         {
        //             page = 1;
        //         }
        //         else
        //         {
        //             searchString = currentFilter;
        //         }

        //         ViewBag.CurrentFilter = searchString;


        //         //Ordenacion

        //         switch (sortOrder)
        //         {

        //             case "ID":
        //                 Orden = " order by ID desc ";
        //                 break;
        //             case "Numero":
        //                 Orden = " order by numero desc ";
        //                 break;
        //             case "Fecha":
        //                 Orden = " order by fecha desc ";
        //                 break;
        //             case "Nombre_Cliente":
        //                 Orden = " order by  Nombre_Proveedor ";
        //                 break;
        //             default:
        //                 Orden = " order by fecha desc ,  Nombre_Proveedor  ";
        //                 break;
        //         }



        //         //var elementos = from s in db.encfacturas
        //         //select s;
        //         string cadenaSQl = ConsultaSql + " " + Filtro + " " + Orden;

        //         var elementos = dbv.Database.SqlQuery<VEncFacturaProvSaldo>(cadenaSQl).ToList();



        //         ViewBag.sumatoria = "";
        //         try
        //         {

        //             var SumaLst = new List<string>();
        //             var SumaQry = ConsultaSqlResumen + " " + Filtro + " " + ConsultaAgrupado;
        //             List<ResumenFac> data = db.Database.SqlQuery<ResumenFac>(SumaQry).ToList();
        //             ViewBag.sumatoria = data;

        //         }
        //         catch (Exception err)
        //         {

        //         }

        //         //Paginación
        //         // DROPDOWNLIST FOR UPDATING PAGE SIZE
        //         int count = dbv.VEncFacturaProvSaldo.OrderBy(e => e.ID).Count();// Total number of elements

        //         // Populate DropDownList
        //         ViewBag.PageSize = new List<SelectListItem>()
        //         {
        //             new SelectListItem { Text = "10", Value = "10" },
        //             new SelectListItem { Text = "25", Value = "25" , Selected = true},
        //             new SelectListItem { Text = "50", Value = "50" },
        //             new SelectListItem { Text = "100", Value = "100" },
        //             new SelectListItem { Text = "Todos", Value = count.ToString() }
        //          };

        //         int pageNumber = (page ?? 1);
        //         int pageSize = (PageSize ?? 25);
        //         ViewBag.psize = pageSize;

        //         return View(elementos.ToPagedList(pageNumber, pageSize));
        //         //Paginación
        //     }


        public ActionResult Create()
        {
            EncfacturaProvContext db = new EncfacturaProvContext();
            ProveedorContext dbp = new ProveedorContext();
            c_FormaPagoContext dbf = new c_FormaPagoContext();
            c_MonedaContext dbm = new c_MonedaContext();
            c_MetodoPagoContext dbmp = new c_MetodoPagoContext();

            string fecha = DateTime.Now.ToString("yyyyMMdd");
            ViewBag.fecha = fecha;
            ClsDatoEntero idpais = dbp.Database.SqlQuery<ClsDatoEntero>(" select IDPais as Dato from dbo.Paises where Codigo = 'MEX'").ToList().FirstOrDefault();


            var prov = dbp.Proveedores.OrderBy(m => m.Empresa).ToList();
            List<SelectListItem> liProv = new List<SelectListItem>();
            List<Proveedor> liprov2 = dbp.Database.SqlQuery<Proveedor>("select * from Proveedores where IDPais != " + idpais.Dato + " order by Empresa").ToList();
            liProv.Add(new SelectListItem { Text = "--Selecciona un Proveedor--", Value = "0" });

            foreach (var m in liprov2)
            {
                liProv.Add(new SelectListItem { Text = m.Empresa, Value = m.IDProveedor.ToString() });
            }
            ViewBag.idproveedor = liProv;

            ViewBag.idmoneda = new SelectList(dbm.c_Monedas, "IDMoneda", "ClaveMoneda");
            ViewBag.metododepago = new SelectList(dbmp.c_MetodoPagos, "IDMetodoPago", "Descripcion");
            ViewBag.formadepago = new SelectList(dbf.c_FormaPagos, "IDFormaPago", "Descripcion");

            //ViewBag.idtipoiva = new SelectList(dbf.c_FormaPagos, "ClaveTipo", "Descripcion");

            //var EstadoLst = new List<SelectListItem>();
            //EstadoLst.Add(new SelectListItem { Text = "Activa", Value = "A" });
            //EstadoLst.Add(new SelectListItem { Text = "Cancelada", Value = "C" });
            //ViewBag.Estado = new SelectList(EstadoLst, "Value", "Text");

            return View();
        }


        // POST: Factura/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(FormCollection coleccion)
        {
            EncfacturaProv elemento = new EncfacturaProv();
            elemento.IDProveedor = int.Parse(coleccion.Get("IDproveedor"));
            elemento.Serie = coleccion.Get("Serie");
            elemento.Numero = coleccion.Get("Numero");

            ClsDatoString nombreproveedor = db.Database.SqlQuery<ClsDatoString>("select empresa as Dato from Proveedores where IDProveedor = " + elemento.IDProveedor + "").ToList().FirstOrDefault();
            elemento.Nombre_Proveedor = nombreproveedor.Dato;
            elemento.Subtotal = decimal.Parse(coleccion.Get("Subtotal"));
            elemento.IVA = decimal.Parse(coleccion.Get("IVA"));
            elemento.Total = decimal.Parse(coleccion.Get("Total"));
            elemento.UUID = "No Aplica";
            elemento.RutaXML = "";
            //elemento.RutaXML = coleccion.Get("fileIMG");
            elemento.Fecha = DateTime.Parse(coleccion.Get("Fecha"));

            elemento.TC = decimal.Parse(coleccion.Get("TC"));
            elemento.IDMoneda = int.Parse(coleccion.Get("IDMoneda"));
            ClsDatoString clavemoneda = db.Database.SqlQuery<ClsDatoString>("select ClaveMoneda as Dato from c_Moneda where IDMoneda= " + elemento.IDMoneda + " ").ToList().FirstOrDefault();
            elemento.Moneda = clavemoneda.Dato;
            int mp = int.Parse(coleccion.Get("Metododepago"));
            ClsDatoString metodopago = db.Database.SqlQuery<ClsDatoString>("select  ClaveMetodoPago as Dato from c_MetodoPago where IDMetodopago= " + mp + " ").ToList().FirstOrDefault();
            elemento.Metododepago = metodopago.Dato;
            elemento.Estado = "A";
            elemento.pagada = false;
            int fp = int.Parse(coleccion.Get("Formadepago"));
            ClsDatoString formapago = db.Database.SqlQuery<ClsDatoString>("select ClaveFormaPago as Dato from c_FormaPago where IDFormaPago= " + fp + " ").ToList().FirstOrDefault();
            elemento.Formadepago = formapago.Dato;
            elemento.ConPagos = false;
            elemento.FechaVencimiento = elemento.Fecha;
            elemento.FechaRevision = elemento.Fecha;


            try
            {


                var db = new EncfacturaProvContext();
                db.EncfacturaProveedores.Add(elemento);
                db.SaveChanges();

                //ClsDatoEntero idfactura = db.Database.SqlQuery<ClsDatoEntero>("select max(id) as Dato from dbo.EncFacturaProv where IDProveedor =" + elemento.IDProveedor + " ").ToList().FirstOrDefault();

                //SaldoFacturaProvContext sfp = new SaldoFacturaProvContext();
                //string comandoSql = "insert into [dbo].[SaldoFacturaProv](IDFacturaProv, IDProveedor, Serie, Numero, Total, ImporteSaldoAnterior, importepagado,  ImporteSaldoInsoluto, empresa)";
                //comandoSql += " values(" + idfactura.Dato + ", " + elemento.IDProveedor + ",  '" + elemento.Serie + "', '" + elemento.Numero + "',  " + elemento.Total + ", '" + elemento.Total + "',  0,  " + elemento.Total + ", '" + elemento.Nombre_Proveedor + "')";
                //sfp.Database.ExecuteSqlCommand(comandoSql);

                return RedirectToAction("Index");
            }
            catch (Exception err)
            {
                string error = err.Message;

                EncfacturaProvContext db = new EncfacturaProvContext();
                ProveedorContext dbp = new ProveedorContext();
                c_FormaPagoContext dbf = new c_FormaPagoContext();
                c_MonedaContext dbm = new c_MonedaContext();
                c_MetodoPagoContext dbmp = new c_MetodoPagoContext();

                string fecha = DateTime.Now.ToString("yyyyMMdd");
                ViewBag.fecha = fecha;
                ClsDatoEntero idpais = dbp.Database.SqlQuery<ClsDatoEntero>(" select IDPais as Dato from dbo.Paises where Codigo = 'MEX'").ToList().FirstOrDefault();


                var prov = dbp.Proveedores.OrderBy(m => m.Empresa).ToList();
                List<SelectListItem> liProv = new List<SelectListItem>();
                List<Proveedor> liprov2 = dbp.Database.SqlQuery<Proveedor>("select * from Proveedores where IDPais != " + idpais.Dato + " order by Empresa").ToList();
                liProv.Add(new SelectListItem { Text = "--Selecciona un Proveedor--", Value = "0" });

                foreach (var m in liprov2)
                {
                    liProv.Add(new SelectListItem { Text = m.Empresa, Value = m.IDProveedor.ToString() });
                }
                ViewBag.idproveedor = liProv;

                ViewBag.idmoneda = new SelectList(dbm.c_Monedas, "IDMoneda", "ClaveMoneda");
                ViewBag.metododepago = new SelectList(dbmp.c_MetodoPagos, "IDMetodoPago", "Descripcion");
                ViewBag.formadepago = new SelectList(dbf.c_FormaPagos, "IDFormaPago", "Descripcion");
                return View();

            }
        }


   


    public ActionResult CreatedesdeArchivo()
        {
            return View();
        }


        [HttpPost]
        public ActionResult CreatedesdeArchivo(FormCollection collection)
        {
            EncfacturaProv elemento = new EncfacturaProv();

            try
            {
                /*Aqui voy intentar guardar la imagen que me pase la vista*/
                try
                {
                    HttpPostedFileBase archivo = Request.Files["filexml"];
                    //archivo.SaveAs(Path.Combine(directory, Path.GetFileName(archivo.FileName)));
                    // Generador.CreaPDF(Path.Combine(@".\facturas", Path.GetFileName(archivo.FileName)));
                    StreamReader reader = new StreamReader(archivo.InputStream);
                    String contenidoxml = reader.ReadToEnd();


                    Generador.CreaPDF temp = new Generador.CreaPDF(contenidoxml);

                    elemento.Fecha = System.DateTime.Parse(temp._templatePDF.fechaEmisionCFDI);


                    elemento.Serie = temp._templatePDF.serie;

                    try
                    {
                        elemento.Numero = temp._templatePDF.folio;
                    }
                    catch (Exception err)
                    {
                        string mensaje = err.Message;
                        elemento.Numero = "0";
                    }
                    elemento.Nombre_Proveedor = temp._templatePDF.emisor.Nombre;

                    // verifica si el proveedor existe


                    try
                    {
                        ProveedorContext dbp = new ProveedorContext();
                        Proveedor proveedorenlabase = dbp.Database.SqlQuery<Proveedor>("select * from proveedores where Empresa='" + temp._templatePDF.emisor.Nombre + "'").ToList()[0];
                        int IDpbase = proveedorenlabase.IDProveedor; /// si la consulta no devolvio fila lanazara una excepcion 
                        elemento.IDProveedor = IDpbase;
                    }
                    catch (Exception err)
                    {
                        string error = err.Message;
                        string mensajealusuario = "EL PROVEEDOR NO SE ENCUENTRA REGISTRADO O SU NOMBRE NO CONSIDE CON EXACTITUD A TU REGISTRO VERIFICA PUNTOS, COMAS Y ESPACIOS ";
                        ViewBag.Mensajeerror = mensajealusuario;
                        //return View();
                        elemento.IDProveedor = 0;
                    }

                    try
                    {
                        EncfacturaProvContext dbp = new EncfacturaProvContext();
                        EncfacturaProv proveedorenlabase = dbp.Database.SqlQuery<EncfacturaProv>("select * from EncfacturaProv where UUID='" + temp._templatePDF.folioFiscalUUID + "'").ToList()[0];
                        int IDpbase = proveedorenlabase.ID; /// si la consulta no devolvio fila lanazara una excepcion 
                        string mensajealusuario = "LA FACTURA YA SE ENCUENTRA EN EL SISTEMA ";
                        ViewBag.Mensajeerror = mensajealusuario;
                        return View();
                    }
                    catch (Exception err)
                    {
                        //string error = err.Message;


                    }


                    elemento.Subtotal = decimal.Parse(temp._templatePDF.subtotal.ToString());
                    elemento.Total = decimal.Parse(temp._templatePDF.total.ToString());
                    elemento.IVA = elemento.Total - elemento.Subtotal;



                    List<c_Moneda> clavemoneda;
                    clavemoneda = db.Database.SqlQuery<c_Moneda>("select * from c_Moneda WHERE claveMoneda='" + temp._templatePDF.claveMoneda + "'").ToList();
                    int clave = clavemoneda.Select(s => s.IDMoneda).FirstOrDefault();
                    elemento.IDMoneda = clave;
                    elemento.Moneda = temp._templatePDF.claveMoneda;
                    elemento.Estado = "A";
                    if (temp._templatePDF.tipo_cambio != "")
                    {
                        elemento.TC = decimal.Parse(temp._templatePDF.tipo_cambio);
                    }
                    else
                    {
                        elemento.TC = 1;
                    }



                    elemento.RutaXML = contenidoxml;

                    elemento.UUID = temp._templatePDF.folioFiscalUUID;
                    elemento.FechaVencimiento = elemento.Fecha;
                    elemento.FechaRevision = elemento.Fecha;
                    elemento.Metododepago = temp._templatePDF.metodoPago;
                    elemento.Formadepago = temp._templatePDF.formaPago;
                    elemento.ConPagos = false;

                    //string cadenainsert = " insert into dbo.encfacturaprov      ([IDProveedor] ,[NoRecepcion] ,[Serie] ,[Numero] ,[Nombre_Proveedor] ,[Subtotal] ,[IVA] ,[Total] ,[UUID]  ,[RutaPDF] ,[Fecha] ,[TC] ,[IDMoneda] ,[Moneda] ,[Metododepago] ,[Estado] ,[pagada] ,[FormadePago] ,[conPagos] ,[Descuento] ,[FechaVencimiento] ,[FechaRevision])     VALUES";
                    //cadenainsert += "(" + elemento.IDProveedor + ",0,'" ','"+elemento.Numero+"','" + elemento.Nombre_Proveedor + "'," + elemento.Subtotal + "," + elemento.IVA + "," + elemento.Total;
                    //cadenainsert += ",'" + elemento.UUID + "',' ',convert(datetime,'" + elemento.Fecha.Day + "/" + elemento.Fecha.Month + "/" + elemento.Fecha.Year + "',103)," + elemento.TC + "," + elemento.IDMoneda + ",'" + elemento.Moneda + "','" + elemento.Metododepago + "','A','0','" + elemento.Formadepago + "','0',0,convert(datetime,'" + elemento.Fecha.Day + "/" + elemento.Fecha.Month + "/" + elemento.Fecha.Year + "',103),convert(datetime,'" + elemento.Fecha.Day + "/" + elemento.Fecha.Month + "/" + elemento.Fecha.Year + "',103))";


                //    new EncfacturaProvContext().Database.ExecuteSqlCommand(cadenainsert);


                       db.EncfacturaProveedores.Add(elemento);
                      db.SaveChanges();
                    ////Aqui actualiza saldos
                    //ClsDatoEntero idfactura = db.Database.SqlQuery<ClsDatoEntero>("select max(id) as Dato from dbo.EncFacturaProv where IDProveedor =" + elemento.IDProveedor + " ").ToList().FirstOrDefault();

                    //SaldoFacturaProvContext sfp = new SaldoFacturaProvContext();
                    //string comandoSql = "insert into [dbo].[SaldoFacturaProv](IDFacturaProv, IDProveedor, Serie, Numero, Total, ImporteSaldoAnterior, importepagado,  ImporteSaldoInsoluto, empresa)";
                    //comandoSql += " values(" + idfactura.Dato + ", " + elemento.IDProveedor + ",  '" + elemento.Serie + "', " + elemento.Numero + ",  " + elemento.Total + ", '" + elemento.Total + "',  0,  " + elemento.Total + ", '" + elemento.Nombre_Proveedor + "')";
                    //sfp.Database.ExecuteSqlCommand(comandoSql);

                    //con eso lo voy a subir asi
                    List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
                    int usuario = userid.Select(s => s.UserID).FirstOrDefault();

                    List<EncfacturaProv> facturaP = db.Database.SqlQuery<EncfacturaProv>("select * from [dbo].[EncFacturaProv] where UUID='" + elemento.UUID + "'").ToList();
                    int IDFacturaP = facturaP.Select(s => s.ID).FirstOrDefault();
                    try
                    {
                        string insert = "insert into FechaSubFacturaP (IDFacturaP, Fecha,Usuario,IDOrdenC) values (" + IDFacturaP + ",sysdatetime()," + usuario + "," + 0 + ")";
                        db.Database.ExecuteSqlCommand(insert);
                    }

                    catch (Exception err)
                    {

                    }

                    //////////////////////77
                    ///subiendo pdf
                    ////////////////////

                    string extension = Path.GetExtension(Request.Files[1].FileName).ToLower();

                    HttpPostedFileBase archivo1 = Request.Files["filepdf"];

                    if (archivo1 != null && archivo1.ContentLength > 0)
                    {
                        if (extension == ".pdf")
                        {
                            var nombreArchivo = Path.GetFileName(archivo1.FileName);
                            var path = Path.Combine(Server.MapPath("~/PDFProveedor"), nombreArchivo);
                            archivo1.SaveAs(path);

                            string nameArchivo = archivo1.FileName;

                            db.Database.ExecuteSqlCommand("update [dbo].[EncFacturaProv] set [RutaPDF]='" + nameArchivo + "' where [UUID]='" + elemento.UUID + "'");
                            //List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
                            //int usuario = userid.Select(s => s.UserID).FirstOrDefault();

                            //List<EncfacturaProv> facturaP = db.Database.SqlQuery<EncfacturaProv>("select * from [dbo].[EncFacturaProv] where UUID='" + elemento.UUID + "'").ToList();
                            //int IDFacturaP = facturaP.Select(s => s.ID).FirstOrDefault();
                            //try
                            //{
                            //    string insert = "insert into FechaSubFacturaP (IDFacturaP, Fecha,Usuario) values (" + IDFacturaP + ",sysdatetime()," + usuario + ")";
                            //    db.Database.ExecuteSqlCommand(insert);
                            //}

                            //catch (Exception err)
                            //{

                            //}

                        }
                        else
                        {
                            ViewBag.Mensajeerror1 = "No se pudo subir el archivo";
                        }

                    }


                    return RedirectToAction("Index");
                }
                catch (Exception ERR)
                {
                    ViewBag.Mensajeerror = ERR.InnerException.Message;
                    return View();
                }


            }
            catch (Exception ERR2)
            {
                ViewBag.Mensajeerror = "Este archivo Xml no contiene una factura valida";
                return View();
            }
        }


        [HttpPost]
        public JsonResult cancelarFactura(int id)
        {
            try
                {

                   
                    string cadenasql = "update [EncFacturaProv] set Estado='C' where ID=" + id;
                    db.Database.ExecuteSqlCommand(cadenasql);
                    return Json(new HttpStatusCodeResult(200));
                }
               


     
            catch (Exception err)
            {
                return Json(500, err.Message);
            }

        }

        public FileResult Descargarxml(int id)
        {
            // Obtener contenido del archivo

            EncfacturaProvContext pf = new EncfacturaProvContext();
            var elemento = pf.EncfacturaProveedores.Single(m => m.ID == id);
            var stream = new MemoryStream(Encoding.ASCII.GetBytes(elemento.RutaXML));

            return File(stream, "text/plain", "fact prov "+ elemento.Nombre_Proveedor + elemento.Serie + elemento.Numero + ".xml");
        }


        public ActionResult Borrafactura(int id)
        {
            // Obtener contenido del archivo

            EncfacturaProvContext pf = new EncfacturaProvContext();
            EncfacturaProv factura = pf.EncfacturaProveedores.Find(id);
            pf.EncfacturaProveedores.Remove(factura);
            pf.SaveChanges();


           return  RedirectToAction   ("index", new { searchString=factura.Nombre_Proveedor });
        }

        /////////////////////// Ver Pagos y sus relaciones ///////////////////////
        //////////////////////////////////////////////////////////////////////////////
        public ActionResult VPagosProv(int id, List<VEncPagos> enc)
        {
            try
            {

                VEncPagosProv encFac = db.Database.SqlQuery<VEncPagosProv>("select ID, Nombre_Proveedor, UUID, Numero as NoFactura, Total from dbo.EncFacturaProv where ID =  " + id + "").ToList()[0];

                ViewBag.Nombre = encFac.Nombre_Proveedor;
                ViewBag.UUID = encFac.UUID;
                ViewBag.NoFactura = encFac.NoFactura;
                ViewBag.Total = encFac.Total;

            }
            catch (Exception err)
            {
                ViewBag.Nombre = "";
                ViewBag.NoFactura = "";
                ViewBag.Total = "";
            }


            List<VPagosProv> elemento = db.Database.SqlQuery<VPagosProv>("select P.[FechaPago],  D.ImporteSaldoInsoluto, D.importepagado, D.NoParcialidad, D.Estado from dbo.PagoFacturaProv as P left join ([dbo].[DocumentoRelacionadoProv]as D left join [dbo].[PagoFacturaSPEIProv] as S on D.IDPagoFacturaProv = S.IDPagoFacturaProv) on P.IDPagoFacturaProv = D.IDPagoFacturaProv where  D.IDFacturaProv =  " + id + "  order by NoParcialidad").ToList();
            if (elemento == null)
            {
                return HttpNotFound();
            }
            return View(elemento);

        }

        public ActionResult ValidarProv()
        {

            //Valida proveedor en Pago Factura  
            PagoFacturaProvContext pprov = new PagoFacturaProvContext();
            string querySQL1 = "update dbo.EncFacturaProv  set IDProveedor = t2.IDProveedor from dbo.EncFacturaProv t1, dbo.Proveedores as t2  where t1.Nombre_Proveedor = t2.Empresa";
            pprov.Database.ExecuteSqlCommand(querySQL1);

           
            return RedirectToAction("Index");
        }

        public ActionResult SubirArchivo(int? id)
        {
            ViewBag.id = id;
            return View();
        }

        /////////////////////// Boton Subir PDF ///////////////////////
        /////////////////////////////////////////////////////////////////

        [HttpPost]
        public ActionResult SubirArchivo(HttpPostedFileBase file, int? id)
        {
            string extension = Path.GetExtension(Request.Files[0].FileName).ToLower();

            HttpPostedFileBase archivo = Request.Files["file"];

            if (file != null && file.ContentLength > 0)
            {
                if (extension == ".pdf")
                {
                    var nombreArchivo = Path.GetFileName(file.FileName);
                    var path = Path.Combine(Server.MapPath("~/PDFProveedor"), nombreArchivo);
                    file.SaveAs(path);

                    string nameArchivo = file.FileName;

                    db.Database.ExecuteSqlCommand("update [dbo].[EncFacturaProv] set [RutaPDF]='" + nameArchivo + "' where [ID]='" + id + "'");
                }
                else
                {
                    ViewBag.Mensajeerror = "No se pudo subir el archivo";
                }
                
            }
           

            return RedirectToAction("Index");


        }

        public FileResult DescargarPdf(int id)
        {
            EncfacturaProvContext pf = new EncfacturaProvContext();
            var elemento = pf.EncfacturaProveedores.Single(m => m.ID == id);

            //var ms = new MemoryStream(Encoding.ASCII.GetBytes("~/PDFProveedor/"+elemento.RutaPDF));
            //string ruta= HttpContext.Current.Server.MapPath("~/Documentostemporales/Factura" + _templatePDF.serie + _templatePDF.folio + ".pdf");
            try
            {
                string path = Server.MapPath("~/PDFProveedor/" + elemento.RutaPDF);
                byte[] fileBytes = System.IO.File.ReadAllBytes(path);
                MemoryStream ms = new MemoryStream(fileBytes, 0, 0, true, true);
                Response.AddHeader("content-disposition", "attachment;filename=" + elemento.RutaPDF + "");
                Response.Buffer = true;
                Response.Clear();
                Response.OutputStream.Write(ms.GetBuffer(), 0, ms.GetBuffer().Length);
                Response.OutputStream.Flush();
                Response.End();
                return new FileStreamResult(Response.OutputStream, "application/pdf");
            }
            catch (Exception err)
            {
                string mensaje = err.Message;
                string text = "No se encontro la factura del proveedor del ticket " + id + " en los archivos del servidor ";

                return File(Encoding.ASCII.GetBytes(text), "text/plain", "Error de descarga.txt");
              

            }
           
        }

        public ActionResult descargarTicket (int id)
        {



            EmpresaContext dbe = new EmpresaContext();

            Empresa empresa = dbe.empresas.Single(m => m.IDEmpresa == 2);
            System.Drawing.Image logoempresa = byteArrayToImage(empresa.Logo);
            EncfacturaProv facturaprov = new EncfacturaProvContext().EncfacturaProveedores.Find(id);

            GeneradorTicket documentop = new GeneradorTicket(logoempresa, facturaprov);
            return RedirectToAction("Index");

        }

        ////
        /// //////////Factura por Proveedor//////////
        /// 


        public ActionResult IndexP(string Numero, string sortOrder, string currentFilter, int? page, int? PageSize, string searchString, string FacPag, string Fechainicio, string Fechafinal)
        {
            int p = -1;
            try
            {
                SIAAPI.Models.Comercial.ClsDatoEntero c = db.Database.SqlQuery<SIAAPI.Models.Comercial.ClsDatoEntero>("select  [IDProveedor] as Dato from [dbo].proveedores where RFC ='" + User.Identity.Name + "'").ToList().FirstOrDefault();
                p = c.Dato;
            }
            catch (Exception err)
            {

            }
            Session["Proveedor"] = p;

            List<Proveedor> proveedor = db.Database.SqlQuery<Proveedor>("select * from [dbo].[Proveedores] where [IDProveedor]=" + p + "").ToList();
            ViewBag.Proveedor = proveedor;
            decimal total = 0;
            decimal totalMNX = 0;
            decimal iva = 0;
            decimal subtotal = 0;


            //Buscar Facturas: Pagadas o no pagadas
            var FacPagLst = new List<SelectListItem>();
            //Facturas Pagadas
            FacPagLst.Add(new SelectListItem { Text = "Pagada", Value = "SI" });
            FacPagLst.Add(new SelectListItem { Text = "NoPagada", Value = "NO" });

            ViewData["FacPag"] = FacPagLst;
            ViewBag.FacPag = new SelectList(FacPagLst, "Value", "Text");
            ViewBag.FacPagseleccionado = FacPag;


            ViewBag.sumatoria = "";


            string ConsultaSql = "select * from VEncFacturaProvSaldo ";
            string ConsultaSqlResumen = "select Moneda as MonedaOrigen, (SUM(Subtotal)) as Subtotal, (SUM(IVA)) as IVA, (SUM(Total)) as Total, (SUM(Total * TC)) as TotalenPesos from EncfacturaProv ";
            string ConsultaAgrupado = "group by Moneda order by Moneda ";
            string Filtro = string.Empty;
            string Orden = " order by fecha desc , Nombre_Proveedor  ";

            ///tabla filtro: serie

            //Buscar por numero
            if (!String.IsNullOrEmpty(Numero))
            {
                if (Filtro == string.Empty)
                {
                    Filtro = "where Numero ='" + Numero + "'";
                }
                else
                {
                    Filtro += " and  Numero ='" + Numero + "'";
                }

            }


            ///tabla filtro: Factura pagada/no pagada && Serie, Nombre Cliente
            if (FacPag != "Todas")
            {
                if (FacPag == "SI")
                {
                    if (Filtro == string.Empty)
                    {
                        Filtro = "where pagada='1'";
                    }
                    else
                    {
                        Filtro += " and  pagada='1'";
                    }
                }
                if (FacPag == "NO")
                {
                    if (Filtro == string.Empty)
                    {
                        Filtro = "where pagada='0'";
                    }
                    else
                    {
                        Filtro += " and  pagada='0'";
                    }
                }
            }



            ViewBag.CurrentSort = sortOrder;

            ViewBag.NumeroSortParm = String.IsNullOrEmpty(sortOrder) ? "Numero" : "";
            ViewBag.FechaSortParm = sortOrder == "Fecha" ? "Fecha" : "";
            ViewBag.Fechainicio = Fechainicio;
            ViewBag.Fechafinal = Fechafinal;

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
            ///Busqueda:Periodo de fechas

            if (Fechainicio == "")
            {
                Fechainicio = string.Empty;
            }
            if (Fechafinal == "")
            {
                Fechafinal = string.Empty;
            }

            if (!String.IsNullOrEmpty(Fechainicio))  //pusieron una fecha
            {
                if (Fechafinal == string.Empty)
                {
                    Fechafinal = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Day.ToString();
                }
                if (Filtro == string.Empty)
                {
                    Filtro = " where  Fecha BETWEEN  '" + Fechainicio + "'   and '" + Fechafinal + "'";
                }
                else
                {
                    Filtro += " and  Fecha BETWEEN  '" + Fechainicio + "'   and '" + Fechafinal + "'";
                }
            }

            //Ordenacion

            switch (sortOrder)
            {

                case "Numero":
                    Orden = " order by numero asc ";
                    break;
                case "UUID":
                    Orden = " order by UUID asc ";
                    break;
                case "Fecha":
                    Orden = " order by fecha ";
                    break;

                default:
                    Orden = " order by fecha desc ,  Nombre_Proveedor  ";
                    break;
            }

            //var elementos = from s in db.encfacturas
            //select s;
            if (Filtro == string.Empty)
            {
                Filtro = "where estado = 'A' and IDProveedor = " + p + "";
            }
            else
            {
                Filtro += " and estado = 'A' and IDProveedor = " + p + " ";
            }

            string cadenaSQl = ConsultaSql + " " + Filtro + " " + Orden;

            var elementos = dbv.Database.SqlQuery<VEncFacturaProvSaldo>(cadenaSQl).ToList();



            ViewBag.sumatoria = "";
            try
            {

                var SumaLst = new List<string>();
                var SumaQry = ConsultaSqlResumen + " " + Filtro + " " + ConsultaAgrupado;
                List<ResumenFac> data = db.Database.SqlQuery<ResumenFac>(SumaQry).ToList();
                ViewBag.sumatoria = data;

            }
            catch (Exception err)
            {
                total = 0;
                subtotal = 0;
                iva = 0;
                totalMNX = 0;
            }

            var cadenaSaldoI = "select  e.Moneda as MonedaOrigen, (SUM(s.ImporteSaldoInsoluto)) as SaldoActual,  (SUM(s.ImporteSaldoInsoluto)* e.TC) as TotalenPesos from dbo.EncFacturaProv as e inner join SaldoFacturaprov as s on s.IDFacturaProv=e.id  where s.ImporteSaldoInsoluto>0 and e.idproveedor= " + p + " and  e.Estado='A' group by e.Moneda, e.tc order by e.Moneda ";

            List<ResumenSaldoInsoluto> dataSI = db.Database.SqlQuery<ResumenSaldoInsoluto>(cadenaSaldoI).ToList();
            ViewBag.sumSaldoI = dataSI;

            //Paginación

            int count = elementos.Count();

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

            return View(elementos.ToPagedList(pageNumber, pageSize));
            //Paginación
        }

        //public ActionResult CreatedesdeArchivo(string returnUrl)
        //{

        //    return View();

        //}


        /////
        ///  Cargar Factura por proveedor ////
        ///  

        public ActionResult CreatedesdeArchivoP(int? id, string returnUrl)
        {

            int IDOrdenCompra = 0;
            try
            {
                IDOrdenCompra = (int)id;
            }
            catch (Exception err)
            {

            }
            ViewBag.IDOrdenC = IDOrdenCompra;


             try
            {
                SIAAPI.Models.Comercial.ClsDatoEntero c = db.Database.SqlQuery<SIAAPI.Models.Comercial.ClsDatoEntero>("select  [IDProveedor] as Dato from [dbo].[ContactosProv] where [Email] ='" + User.Identity.Name + "'").ToList()[0];
                int p = c.Dato;
                List<Proveedor> proveedor = db.Database.SqlQuery<Proveedor>("select * from [dbo].[Proveedores] where [IDProveedor]=" + p + "").ToList();
                ViewBag.Proveedor = proveedor;
            }
            catch (Exception err)
            {
               
            }
            return View();

        }

        [HttpPost]

        public ActionResult CreatedesdeArchivoP(FormCollection collection, string returnUrl)
        {
            int IDOrdenC = 0;
            try
            {
                IDOrdenC = int.Parse(collection.Get("IDOrdenC"));
            }
            catch (Exception err)
            {

            }
            EncfacturaProv elemento = new EncfacturaProv();

            try
            {
                /*Aqui voy intentar guardar la imagen que me pase la vista*/
                try
                {
                    HttpPostedFileBase archivo = Request.Files["Imag1"];
                    //archivo.SaveAs(Path.Combine(directory, Path.GetFileName(archivo.FileName)));
                    // Generador.CreaPDF(Path.Combine(@".\facturas", Path.GetFileName(archivo.FileName)));
                    StreamReader reader = new StreamReader(archivo.InputStream);
                    String contenidoxml = reader.ReadToEnd();


                    Generador.CreaPDF temp = new Generador.CreaPDF(contenidoxml);

                    elemento.Fecha = System.DateTime.Parse(temp._templatePDF.fechaEmisionCFDI);


                    elemento.Serie = temp._templatePDF.serie;

                    try
                    {
                        elemento.Numero = temp._templatePDF.folio;
                    }
                    catch (Exception err)
                    {
                        string mensaje = err.Message;
                        elemento.Numero = "0";
                    }
                    elemento.Nombre_Proveedor = temp._templatePDF.emisor.Nombre;

                    // verifica si el proveedor existe


                    try
                    {
                        ProveedorContext dbp = new ProveedorContext();
                        Proveedor proveedorenlabase = dbp.Database.SqlQuery<Proveedor>("select * from proveedores where Empresa='" + temp._templatePDF.emisor.Nombre + "'").ToList()[0];
                        int IDpbase = proveedorenlabase.IDProveedor; /// si la consulta no devolvio fila lanazara una excepcion 
                        elemento.IDProveedor = IDpbase;
                    }
                    catch (Exception err)
                    {
                        //string error = err.Message;
                        //string mensajealusuario = "EL PROVEEDOR NO SE ENCUENTRA REGISTRADO O SU NOMBRE NO CONSIDE CON EXACTITUD A TU REGISTRO VERIFICA PUNTOS, COMAS Y ESPACIOS ";
                        //ViewBag.Mensajeerror = mensajealusuario;
                        //return View();
                        elemento.IDProveedor = 0;
                    }

                    try
                    {
                        EncfacturaProvContext dbp = new EncfacturaProvContext();
                        EncfacturaProv proveedorenlabase = dbp.Database.SqlQuery<EncfacturaProv>("select * from EncfacturaProv where UUID='" + temp._templatePDF.folioFiscalUUID + "'").ToList()[0];
                        int IDpbase = proveedorenlabase.ID; /// si la consulta no devolvio fila lanazara una excepcion 
                        string mensajealusuario = "LA FACTURA YA SE ENCUENTRA EN EL SISTEMA ";
                        ViewBag.Mensajeerror = mensajealusuario;
                        return View();
                    }
                    catch (Exception err)
                    {
                        //string error = err.Message;


                    }


                    elemento.Subtotal = decimal.Parse(temp._templatePDF.subtotal.ToString());
                    elemento.Total = decimal.Parse(temp._templatePDF.total.ToString());
                    elemento.IVA = elemento.Total - elemento.Subtotal;



                    List<c_Moneda> clavemoneda;
                    clavemoneda = db.Database.SqlQuery<c_Moneda>("select * from c_Moneda WHERE claveMoneda='" + temp._templatePDF.claveMoneda + "'").ToList();
                    int clave = clavemoneda.Select(s => s.IDMoneda).FirstOrDefault();
                    elemento.IDMoneda = clave;
                    elemento.Moneda = temp._templatePDF.claveMoneda;
                    elemento.Estado = "A";
                    if (temp._templatePDF.tipo_cambio != "")
                    {
                        elemento.TC = decimal.Parse(temp._templatePDF.tipo_cambio);
                    }
                    else
                    {
                        elemento.TC = 1;
                    }

                    elemento.FechaVencimiento = elemento.Fecha;
                    elemento.FechaRevision = elemento.Fecha;

                    elemento.RutaXML = contenidoxml;

                    elemento.UUID = temp._templatePDF.folioFiscalUUID;

                    elemento.Metododepago = temp._templatePDF.metodoPago;
                    elemento.Formadepago = temp._templatePDF.formaPago;
                    elemento.ConPagos = false;
                    db.EncfacturaProveedores.Add(elemento);
                    db.SaveChanges();
                    List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
                    int usuario = userid.Select(s => s.UserID).FirstOrDefault();

                    List<EncfacturaProv> facturaP = db.Database.SqlQuery<EncfacturaProv>("select * from [dbo].[EncFacturaProv] where UUID='" + elemento.UUID + "'").ToList();
                    int IDFacturaP = facturaP.Select(s => s.ID).FirstOrDefault();
                    try
                    {
                        string insert = "insert into FechaSubFacturaP (IDFacturaP, Fecha,Usuario,IDOrdenC) values (" + IDFacturaP + ",sysdatetime()," + usuario + "," + IDOrdenC + ")";
                        db.Database.ExecuteSqlCommand(insert);
                    }

                    catch (Exception err)
                    {

                    }
                    ///////////////////////
                    /// SUBIR PDF
                    /////////////////

                    string extension = Path.GetExtension(Request.Files[1].FileName).ToLower();

                    HttpPostedFileBase archivo1 = Request.Files["file"];

                    if (archivo1 != null && archivo1.ContentLength > 0)
                    {
                        if (extension == ".pdf")
                        {
                            var nombreArchivo = Path.GetFileName(archivo1.FileName);
                            var path = Path.Combine(Server.MapPath("~/PDFProveedor"), nombreArchivo);
                            archivo1.SaveAs(path);

                            string nameArchivo = archivo1.FileName;

                            db.Database.ExecuteSqlCommand("update [dbo].[EncFacturaProv] set [RutaPDF]='" + nameArchivo + "' where [UUID]='" + elemento.UUID + "'");

                            ////List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
                            ////int usuario = userid.Select(s => s.UserID).FirstOrDefault();

                            ////List<EncfacturaProv> facturaP = db.Database.SqlQuery<EncfacturaProv>("select * from [dbo].[EncFacturaProv] where UUID='" + elemento.UUID + "'").ToList();
                            ////int IDFacturaP = facturaP.Select(s => s.ID).FirstOrDefault();
                            ////try
                            ////{
                            ////    string insert = "insert into FechaSubFacturaP (IDFacturaP, Fecha,Usuario,IDOrdenC) values (" + IDFacturaP + ",sysdatetime()," + usuario + ","+ IDOrdenC + ")";
                            ////    db.Database.ExecuteSqlCommand(insert);
                            ////}

                            ////catch (Exception err)
                            ////{

                            ////}
                        
                        }
                        else
                        {
                            ViewBag.Mensajeerror1 = "No se pudo subir el archivo";
                        }

                    }



                    //return RedirectToAction("Index");
                    if (!String.IsNullOrEmpty(returnUrl))
                        return Redirect(returnUrl);
                    else
                        return RedirectToAction("IndexP");

                }
                catch (Exception ERR)
                {
                    ViewBag.Mensajeerror = ERR.InnerException.Message;
                    return View();
                }


            }
            catch (Exception ERR2)
            {
                ViewBag.Mensajeerror = "Este archivo Xml no contiene una factura valida";
                return View();
            }
        }

        ////////////////

        /////////////////////// Ver Pagos Por Proveedores y sus relaciones ///////////////////////
        //////////////////////////////////////////////////////////////////////////////

        public ActionResult VPagosProvP(int id, List<VEncPagos> enc)
        {
            //int idFact = int.Parse(id.ToString());
            try
            {
                string cadena = "select ID, Nombre_Proveedor, UUID, Numero as NoFactura, Total from dbo.EncFacturaProv where ID =  " + id + "";

                VEncPagosProv encFac = db.Database.SqlQuery<VEncPagosProv>(cadena).ToList().FirstOrDefault();
                ViewBag.Nombre = encFac.Nombre_Proveedor;
                ViewBag.UUID = encFac.UUID;
                ViewBag.NoFactura = encFac.NoFactura;
                ViewBag.Total = encFac.Total;


            }
            catch (Exception err)
            {
                ViewBag.Nombre = "";
                ViewBag.NoFactura = "";
                ViewBag.Total = "";
            }
            string cadena1 = "select P.[FechaPago], FolioP,   D.ImporteSaldoInsoluto, D.importepagado, D.NoParcialidad, D.Estado from dbo.PagoFacturaProv as P left join ([dbo].[DocumentoRelacionadoProv]as D left join [dbo].[PagoFacturaSPEIProv] as S on D.IDPagoFacturaProv = S.IDPagoFacturaProv) on P.IDPagoFacturaProv = D.IDPagoFacturaProv where  D.IDFacturaProv =  " + id + " order by NoParcialidad";
            List<VPagosProv> elemento = db.Database.SqlQuery<VPagosProv>(cadena1).ToList();
            if (elemento == null)
            {
                return HttpNotFound();
            }
            return View(elemento);
        }

      

        ///

        /////////////////////// Boton Subir PDF por proveedor///////////////////////
        /////////////////////////////////////////////////////////////////
        public ActionResult SubirArchivoP(int? id)
        {
            try
            {

                VEncPagosProv encFac = db.Database.SqlQuery<VEncPagosProv>("select ID, Nombre_Proveedor, UUID, Numero as NoFactura, Total from dbo.EncFacturaProv where ID =  " + id + "").ToList()[0];

                ViewBag.Nombre = encFac.Nombre_Proveedor;
                ViewBag.UUID = encFac.UUID;
                ViewBag.NoFactura = encFac.NoFactura;
                ViewBag.Total = encFac.Total;

            }
            catch (Exception err)
            {
                ViewBag.Nombre = "";
                ViewBag.NoFactura = "";
                ViewBag.Total = "";
            }
            ViewBag.id = id;
            return View();
        }

        [HttpPost]
        public ActionResult SubirArchivoP(HttpPostedFileBase file, int? id)
        {
            string extension = Path.GetExtension(Request.Files[0].FileName).ToLower();

            HttpPostedFileBase archivo = Request.Files["file"];

            if (file != null && file.ContentLength > 0)
            {
                if (extension == ".pdf")
                {
                    var nombreArchivo = Path.GetFileName(file.FileName);
                    var path = Path.Combine(Server.MapPath("~/PDFProveedor"), nombreArchivo);
                    file.SaveAs(path);

                    string nameArchivo = file.FileName;

                    db.Database.ExecuteSqlCommand("update [dbo].[EncFacturaProv] set [RutaPDF]='" + nameArchivo + "' where [ID]='" + id + "'");
                }
                else
                {
                    ViewBag.Mensajeerror = "No se pudo subir el archivo";
                }

            }


            return RedirectToAction("IndexP");


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

        //fecha revi

        public ActionResult FechaRevision(int id, string Mensaje="")
        {
            var elemento = db.EncfacturaProveedores.Single(m => m.ID == id);
            ViewBag.Mensaje = Mensaje;
            return View(elemento);
        }



        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult FechaRevision(EncfacturaProv encF, FormCollection collection, int id)
        {
           // encF.FechaRevision = DateTime.Parse(collection.Get("FechaRevision"));
            DateTime FechaRe = encF.FechaRevision;
            EncfacturaProv elemento = new EncfacturaProvContext().EncfacturaProveedores.Find(id);

            try
            {

            //    elemento = encF;



                ClsDatoEntero contarfac = db.Database.SqlQuery<ClsDatoEntero>("select Proveedores.idCondicionesPago as Dato from EncFacturaProv inner join Proveedores on EncFacturaProv.idproveedor=Proveedores.idproveedor  where EncFacturaProv.id=" + elemento.ID).ToList().FirstOrDefault();
                int conPago = contarfac.Dato;
                string d = "select DiasCredito as Dato from [CondicionesPago]  where idCondicionesPago=" + conPago;
                ClsDatoDecimal DIASCRE = db.Database.SqlQuery<ClsDatoDecimal>(d).ToList().FirstOrDefault();
                decimal? diasCredito = DIASCRE.Dato;
                if (diasCredito==null)
                {
                    diasCredito = 1;
                }

                DateTime FechaVen = FechaRe.AddDays(Convert.ToInt32(diasCredito.ToString()));

                string cadena = "update EncFacturaProv set FechaRevision=convert(datetime,'" + FechaRe.Day + "/" + FechaRe.Month + "/" + FechaRe.Year + "',103)  ,FechaVencimiento=convert(datetime,'" + FechaVen.Day + "/" + FechaVen.Month + "/" + FechaVen.Year + "',103) where ID=" + id;
                db.Database.ExecuteSqlCommand(cadena);

                VEncFacturaProvSaldo saldo = db.Database.SqlQuery<VEncFacturaProvSaldo>("select * from VEncFacturaProvSaldo where ID=" + id).ToList().FirstOrDefault();

                if (saldo.ImportePagado==-1)
                {
                    string NombreProveedor = elemento.Nombre_Proveedor.Replace("'", "''");
                    SaldoFacturaProvContext sfp = new SaldoFacturaProvContext();
                    string comandoSql = "insert into [dbo].[SaldoFacturaProv](IDFacturaProv, IDProveedor, Serie, Numero, Total, ImporteSaldoAnterior, importepagado,  ImporteSaldoInsoluto, empresa)";
                    comandoSql += " values(" + id + ", " + elemento.IDProveedor + ",  '" + elemento.Serie + "', '" + elemento.Numero + "',  " + elemento.Total + ", " + elemento.Total + ",  0,  " + elemento.Total + ", '" + NombreProveedor + "')";
                    sfp.Database.ExecuteSqlCommand(comandoSql);

                }



            }
            catch (Exception err)
            {
               // elemento = encF;



                //ClsDatoEntero contarfac = db.Database.SqlQuery<ClsDatoEntero>("select Proveedores.idCondicionesPago as dato from EncFacturaProv inner join Proveedores on EncFacturaProv.idproveedor=Proveedores.idproveedor and Proveedores.IDCondicionesPago <> 10 and Proveedores.IDCondicionesPago <>1 and Proveedores.IDCondicionesPago <>2  where EncFacturaProv.id=" + elemento.ID).ToList().FirstOrDefault();
                //int conPago = contarfac.Dato;
                //string d = "select DiasCredito as Dato from [CondicionesPago]  where idCondicionesPago=" + conPago;
                //ClsDatoDecimal DIASCRE = db.Database.SqlQuery<ClsDatoDecimal>(d).ToList().FirstOrDefault();
                //decimal diasCredito = DIASCRE.Dato;


                //DateTime FechaVen = FechaRe.AddDays(Convert.ToInt32(diasCredito));
                //string cadena = "update EncFacturaProv set FechaRevision=convert(datetime,'" + FechaRe.Day + "/" + FechaRe.Month + "/" + FechaRe.Year + "',103) where ID=" + id;

                //return RedirectToAction("FechaRevision",new { id = elemento.ID, Mensaje = err.Message + " "+ cadena  });
         
            }



           return RedirectToAction("FechaRevision", new { id = elemento.ID, Mensaje = "" });

        }


        public ActionResult AntiguedadSaldos()
        {
            List<Proveedor> proveedores = new List<Proveedor>();
            string cadena = "select* from dbo.Proveedores where Empresa in (select distinct Nombre_Proveedor from[dbo].[VEncFacturaSaldoProv] where ImporteSaldoInsoluto> 0) order by Empresa";
            proveedores = db.Database.SqlQuery<Proveedor>(cadena).ToList();
            List<SelectListItem> listaproveedor = new List<SelectListItem>();
            listaproveedor.Add(new SelectListItem { Text = "--Selecciona Proveedor--", Value = "0" });

            foreach (var m in proveedores)
            {
                listaproveedor.Add(new SelectListItem { Text = m.Empresa, Value = m.IDProveedor.ToString() });
            }
            ViewBag.proveedor = listaproveedor;




            return View();
        }

        [HttpPost]
        public ActionResult AntiguedadSaldos(AntiguedadSaldosProveedores r, ProveedoresFacturasA C)
        {
            int IDProveedor = C.IDProveedor;
            try
            {

                ProveedorContext dbc = new ProveedorContext();
                Proveedor cls = dbc.Proveedores.Find(C.IDProveedor);
            }
            catch (Exception ERR)
            {

            }




            AntiguedadSaldosProveedores report = new AntiguedadSaldosProveedores();
            byte[] abytes = report.PrepareReport(IDProveedor);
            return File(abytes, "application/pdf");
        }
  public ActionResult EntreFechaFacProv()
        {
            EFecha elemento = new EFecha();

            return View(elemento);
        }

        [HttpPost]
        public ActionResult EntreFechaFacProv(EFecha modelo, FormCollection coleccion)
        {
            VEncFacturaProvSaldoContext dbr = new VEncFacturaProvSaldoContext();
            string FI = modelo.fechaini.Year.ToString() + "-" + modelo.fechaini.Month.ToString() + "-" + modelo.fechaini.Day.ToString();
            string FF = modelo.fechafin.Year.ToString() + "-" + modelo.fechafin.Month.ToString() + "-" + modelo.fechafin.Day.ToString();

            string cual = coleccion.Get("Enviar");

            string cadena = "";
            string cadena2 = "";
            string cadenaDet = "";

            if (cual == "Generar reporte")
            {
                return View();
            }
            if (cual == "Generar excel")
            {

                cadena = "select * from  dbo. VEncFacturaProvSaldo where Fecha >= '" + FI + "' and Fecha <='" + FF + "' order by Nombre_Proveedor, ID Desc";
                var datos = dbr.Database.SqlQuery<VEncFacturaProvSaldo>(cadena).ToList();
                ViewBag.datos = datos;
                cadena2 = "select * from  dbo. VEncFacturaProvSaldo where Fecha >= '" + FI + "' and Fecha <='" + FF + "' order by ID Desc";
                var datos2 = dbr.Database.SqlQuery<VEncFacturaProvSaldo>(cadena2).ToList();
                ViewBag.datos2 = datos2;



                ExcelPackage Ep = new ExcelPackage();

                //Hoja 1
                //Crear la hoja y poner el nombre de la pestaña del libro
                var Sheet = Ep.Workbook.Worksheets.Add("Facturas Proveedores");

                // en la fila1 formateo las celdas y coloco el título de la hoja
                // RichText permite agregar las propiedades de tipo de letra: color, negrita, etc.
                int row = 1;
                //Fijar la fuente para A1:Q1
                Sheet.Cells["A1:O1"].Style.Font.Size = 20;
                Sheet.Cells["A1:O1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:O3"].Style.Font.Bold = true;
                Sheet.Cells["A1:O1"].Style.Font.Color.SetColor(Color.DarkBlue);
                Sheet.Cells["A1:O1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                Sheet.Cells["A1"].RichText.Add("Listado de facturas de proveedores");

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
                //En la fila3 se da el formato a el encabezado
                row = 3;
                Sheet.Cells.Style.Font.Name = "Calibri";
                Sheet.Cells.Style.Font.Size = 10;
                Sheet.Cells["A3:Q3"].Style.Font.Bold = true;
                Sheet.Cells["A3:Q3"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                Sheet.Cells["A3:Q3"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                //Se establece el nombre que identifica a cada una de las columnas de datos.
                Sheet.Cells["A3"].RichText.Add("ID");
                Sheet.Cells["B3"].RichText.Add("Fecha"); ;
                Sheet.Cells["C3"].RichText.Add("FechaRevision");
                Sheet.Cells["D3"].RichText.Add("FechaVencimiento");
                Sheet.Cells["E3"].RichText.Add("Proveedor");
                Sheet.Cells["F3"].RichText.Add("Serie");
                Sheet.Cells["G3"].RichText.Add("Numero");
                Sheet.Cells["H3"].RichText.Add("Subtotal");
                Sheet.Cells["I3"].RichText.Add("IVA");
                Sheet.Cells["J3"].RichText.Add("Total");
                Sheet.Cells["K3"].RichText.Add("TC");
                Sheet.Cells["L3"].RichText.Add("Divisa");
                Sheet.Cells["M3"].RichText.Add("Metodo de Pago");
                Sheet.Cells["N3"].RichText.Add("FormadePago");
                Sheet.Cells["O3"].RichText.Add("ImportePagado");
                Sheet.Cells["P3"].RichText.Add("Importe Saldo Insoluto");
                Sheet.Cells["Q3"].RichText.Add("UUID");



                //Aplicar borde doble al rango de celdas A3:Q3
                Sheet.Cells["A3:Q3"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;

                // en la fila4, el foreach llenara cada fila de la hoja de excel, con cada fila que trae de la bd.
                // Se establecen los formatos para las celdas: Fecha, Moneda
                row = 4;
                Sheet.Cells.Style.Font.Bold = false;
                foreach (VEncFacturaProvSaldo item2 in ViewBag.datos)
                {

                    Sheet.Cells[string.Format("A{0}", row)].Value = item2.ID;
                    Sheet.Cells[string.Format("B{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    Sheet.Cells[string.Format("B{0}", row)].Value = item2.Fecha;
                    Sheet.Cells[string.Format("C{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    Sheet.Cells[string.Format("C{0}", row)].Value = item2.FechaRevision;
                    Sheet.Cells[string.Format("D{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    Sheet.Cells[string.Format("D{0}", row)].Value = item2.FechaVencimiento;
                    Sheet.Cells[string.Format("E{0}", row)].Value = item2.Nombre_Proveedor;
                    Sheet.Cells[string.Format("F{0}", row)].Value = item2.Serie;
                    Sheet.Cells[string.Format("G{0}", row)].Value = item2.Numero;
                    Sheet.Cells[string.Format("H{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("H{0}", row)].Value = item2.Subtotal;
                    Sheet.Cells[string.Format("I{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("I{0}", row)].Value = item2.IVA;
                    Sheet.Cells[string.Format("J{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("J{0}", row)].Value = item2.Total;
                    Sheet.Cells[string.Format("K{0}", row)].Value = item2.TC;
                    Sheet.Cells[string.Format("L{0}", row)].Value = item2.Moneda;

                    Sheet.Cells[string.Format("M{0}", row)].Value = item2.Metododepago;
                    Sheet.Cells[string.Format("N{0}", row)].Value = item2.Formadepago;
                    Sheet.Cells[string.Format("O{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("O{0}", row)].Value = item2.ImportePagado;
                    Sheet.Cells[string.Format("P{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("P{0}", row)].Value = item2.ImporteSaldoInsoluto;
                    Sheet.Cells[string.Format("Q{0}", row)].Value = item2.UUID;


                    row++;

                }
                ///Fin hoja 1
                ///
                /// hoja 2

                //Crear la hoja y poner el nombre de la pestaña del libro
                Sheet = Ep.Workbook.Worksheets.Add("Facturas proveedores con pagos");

                // en la fila1 formateo las celdas y coloco el título de la hoja
                // RichText permite agregar las propiedades de tipo de letra: color, negrita, etc.
                row = 1;
                //Fijar la fuente para A1:Q1
                Sheet.Cells["A1:O1"].Style.Font.Size = 20;
                Sheet.Cells["A1:O1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:O3"].Style.Font.Bold = true;
                Sheet.Cells["A1:O1"].Style.Font.Color.SetColor(Color.DarkBlue);
                Sheet.Cells["A1:O1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                Sheet.Cells["A1"].RichText.Add("Listado de pagos de facturas de proveedores");

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
                //En la fila3 se da el formato a el encabezado
                row = 3;
                Sheet.Cells.Style.Font.Name = "Calibri";
                Sheet.Cells.Style.Font.Size = 10;
                Sheet.Cells["A3:Q3"].Style.Font.Bold = true;
                Sheet.Cells["A3:Q3"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                Sheet.Cells["A3:Q3"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                //Se establece el nombre que identifica a cada una de las columnas de datos.
                Sheet.Cells["A3"].RichText.Add("ID");
                Sheet.Cells["B3"].RichText.Add("Fecha"); ;
                Sheet.Cells["C3"].RichText.Add("FechaRevision");
                Sheet.Cells["D3"].RichText.Add("FechaVencimiento");
                Sheet.Cells["E3"].RichText.Add("Proveedor");
                Sheet.Cells["F3"].RichText.Add("Serie");
                Sheet.Cells["G3"].RichText.Add("Numero");
                Sheet.Cells["H3"].RichText.Add("Subtotal");
                Sheet.Cells["I3"].RichText.Add("IVA");
                Sheet.Cells["J3"].RichText.Add("Total");
                Sheet.Cells["K3"].RichText.Add("TC");
                Sheet.Cells["L3"].RichText.Add("Divisa");
                Sheet.Cells["M3"].RichText.Add("Metodo de Pago");
                Sheet.Cells["N3"].RichText.Add("FormadePago");
                Sheet.Cells["O3"].RichText.Add("ImportePagado");
                Sheet.Cells["P3"].RichText.Add("Importe Saldo Insoluto");
                Sheet.Cells["Q3"].RichText.Add("UUID");



                //Aplicar borde doble al rango de celdas A3:Q3
                Sheet.Cells["A3:Q3"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;

                // en la fila4, el foreach llenara cada fila de la hoja de excel, con cada fila que trae de la bd.
                // Se establecen los formatos para las celdas: Fecha, Moneda
                row = 4;
                Sheet.Cells.Style.Font.Bold = false;
                foreach (VEncFacturaProvSaldo item in ViewBag.datos2)
                {

                    Sheet.Cells[string.Format("A{0}", row)].Value = item.ID;
                    Sheet.Cells[string.Format("B{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    Sheet.Cells[string.Format("B{0}", row)].Value = item.Fecha;
                    Sheet.Cells[string.Format("C{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    Sheet.Cells[string.Format("C{0}", row)].Value = item.FechaRevision;
                    Sheet.Cells[string.Format("D{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    Sheet.Cells[string.Format("D{0}", row)].Value = item.FechaVencimiento;
                    Sheet.Cells[string.Format("E{0}", row)].Value = item.Nombre_Proveedor;
                    Sheet.Cells[string.Format("F{0}", row)].Value = item.Serie;
                    Sheet.Cells[string.Format("G{0}", row)].Value = item.Numero;
                    Sheet.Cells[string.Format("H{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("H{0}", row)].Value = item.Subtotal;
                    Sheet.Cells[string.Format("I{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("I{0}", row)].Value = item.IVA;
                    Sheet.Cells[string.Format("J{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("J{0}", row)].Value = item.Total;
                    Sheet.Cells[string.Format("K{0}", row)].Value = item.TC;
                    Sheet.Cells[string.Format("L{0}", row)].Value = item.Moneda;

                    Sheet.Cells[string.Format("M{0}", row)].Value = item.Metododepago;
                    Sheet.Cells[string.Format("N{0}", row)].Value = item.Formadepago;
                    Sheet.Cells[string.Format("O{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("O{0}", row)].Value = item.ImportePagado;
                    Sheet.Cells[string.Format("P{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("P{0}", row)].Value = item.ImporteSaldoInsoluto;
                    Sheet.Cells[string.Format("Q{0}", row)].Value = item.UUID;


                    row++;
                    if (item.ConPagos != false)
                    {
                        PagoFacturaDetContext dbp = new PagoFacturaDetContext();

                        cadenaDet = "select * from dbo.PagoFacturaDet where idFacturaProv = '" + item.ID + "'  order by NoParcialidad";
                        var datosDet = dbr.Database.SqlQuery<PagoFacturaDet>(cadenaDet).ToList();
                        ViewBag.datosDet = datosDet;


                        // Se establece el nombre que identifica a cada una de las columnas de datos.

                        Sheet.Cells[string.Format("D{0}", row)].Value = "Fecha Pago";
                        Sheet.Cells[string.Format("E{0}", row)].Value = "No Parcialidad";
                        Sheet.Cells[string.Format("F{0}", row)].Value = "Serie";
                        Sheet.Cells[string.Format("G{0}", row)].Value = "Numero";
                        Sheet.Cells[string.Format("H{0}", row)].Value = "ImportePagado";
                        Sheet.Cells[string.Format("I{0}", row)].Value = "Importe Saldo Insoluto";
                        Sheet.Cells[string.Format("J{0}", row)].Value = "No Operacion";
                        Sheet.Cells[string.Format("K{0}", row)].Value = "RFCBanco";
                        Sheet.Cells[string.Format("L{0}", row)].Value = "Cuenta";
                        Sheet.Cells[string.Format("M{0}", row)].Value = "Metodo de Pago";
                        Sheet.Cells[string.Format("N{0}", row)].Value = "FormaPago";
                        Sheet.Cells[string.Format("Q{0}", row)].Value = "Observacion";
                        row++;


                        foreach (PagoFacturaDet itemp in ViewBag.datosDet)
                        {

                            Sheet.Cells[string.Format("D{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                            Sheet.Cells[string.Format("D{0}", row)].Value = itemp.FechaPago;
                            Sheet.Cells[string.Format("E{0}", row)].Value = itemp.NoParcialidad;
                            Sheet.Cells[string.Format("F{0}", row)].Value = itemp.Serie;
                            Sheet.Cells[string.Format("G{0}", row)].Value = itemp.Numero;
                            Sheet.Cells[string.Format("H{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                            Sheet.Cells[string.Format("H{0}", row)].Value = itemp.ImportePagado;
                            Sheet.Cells[string.Format("I{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                            Sheet.Cells[string.Format("I{0}", row)].Value = itemp.ImporteSaldoInsoluto;
                            Sheet.Cells[string.Format("J{0}", row)].Value = itemp.NoOperacion;
                            Sheet.Cells[string.Format("K{0}", row)].Value = itemp.RFCBancoProv;
                            Sheet.Cells[string.Format("L{0}", row)].Value = itemp.CuentaProv;
                            Sheet.Cells[string.Format("M{0}", row)].Value = itemp.ClaveMetododepago;
                            Sheet.Cells[string.Format("N{0}", row)].Value = itemp.Formapago;
                            Sheet.Cells[string.Format("Q{0}", row)].Value = itemp.Observacion;
                            row++;
                        }
                        row++;
                    }
                }

                ///Fin hoja 2
                /// 
              

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

        public ActionResult SubirArchivoRec(int id)
        {
            ViewBag.ID = id;
            return View();
        }

        [HttpPost]
        public ActionResult SubirArchivoRec(HttpPostedFileBase file, int id)
        {
            int idF = int.Parse(id.ToString());
            SubirArchivosModelo modelo = new SubirArchivosModelo();
            if (file != null)
            {
                string ruta = Server.MapPath("~/RecepcionAdd/");
                ruta += "Rec_" + id + "_" + file.FileName;
                string cad = "insert into  [dbo].[RecepcionAdd]([IDFactura], [RutaArchivo], nombreArchivo) Values (" + idF + ", '" + ruta + "','" + "Rec_" + id + "_" + file.FileName + "' )";
                new RecepcionAddContext().Database.ExecuteSqlCommand(cad);
                modelo.SubirArchivo(ruta, file);
                ViewBag.Error = modelo.error;
                ViewBag.Correcto = modelo.Confirmacion;
            }
            return RedirectToAction("indexP", new { searchString = id });
        }

        public FileResult DescargarPDFRec(int id)
        {
            // Obtener contenido del archivo
            RecepcionAddContext dbp = new RecepcionAddContext();
            RecepcionAdd elemento = dbp.RecepcionAdd.Find(id);
            //    string contentType = System.Net.Mime.MediaTypeNames.Application.Pdf;
            //    return new FilePathResult(elemento.RutaArchivo.ToString(), contentType);
            string extension = elemento.nombreArchivo.Substring(elemento.nombreArchivo.Length - 3, 3);
            extension = extension.ToLower();
            string contentType = "";
            if (extension == "pdf")
            {
                contentType = System.Net.Mime.MediaTypeNames.Application.Pdf;
                return new FilePathResult(elemento.RutaArchivo.ToString(), contentType);
            }
            if (extension == "xml")
            {
                var stream = new MemoryStream(Encoding.ASCII.GetBytes(elemento.RutaArchivo.ToString()));
                return File(stream, "text/plain", elemento.nombreArchivo);
            }
            if (extension == "prn" || extension == "txt")
            {
                contentType = System.Net.Mime.MediaTypeNames.Text.RichText;
                return File(elemento.RutaArchivo.ToString(), contentType);
            }

            if (extension == "jpg" || extension == "jpeg")
            {
                contentType = System.Net.Mime.MediaTypeNames.Image.Jpeg;
                return new FilePathResult(elemento.RutaArchivo.ToString(), contentType);
            }
            if (extension == "gif")
            {
                contentType = System.Net.Mime.MediaTypeNames.Image.Gif;
                return new FilePathResult(elemento.RutaArchivo.ToString(), contentType);
            }
            return new FilePathResult(elemento.RutaArchivo.ToString(), contentType);
        }


        public ActionResult EliminarArchivoRec(int id)
        {

            RecepcionAddContext db = new RecepcionAddContext();

            string cad = "delete from dbo.RecepcionAdd where ID= " + id + "";
            new PedidoAddContext().Database.ExecuteSqlCommand(cad);

            return RedirectToAction("indexP");
        }
        public ActionResult EliminarArchivoRecAd(int id)
        {

            RecepcionAddContext db = new RecepcionAddContext();

            string cad = "delete from dbo.RecepcionAdd where ID= " + id + "";
            new PedidoAddContext().Database.ExecuteSqlCommand(cad);

            return RedirectToAction("index");
        }

        public ActionResult VHistorialPagoProv()
        {
            //Buscar proveedor
            ProveedorContext prov = new ProveedorContext();
            var proveedor = prov.Proveedores.OrderBy(s => s.Empresa).ToList();
            List<SelectListItem> li = new List<SelectListItem>();
            li.Add(new SelectListItem { Text = "--Selecciona Proveedor--", Value = "0" });

            foreach (var m in proveedor)
            {
                li.Add(new SelectListItem { Text = m.Empresa, Value = m.IDProveedor.ToString() });


            }
            ViewBag.proveedor = li;

            EFechaval elemento = new EFechaval();
            return View(elemento);
        }

        [HttpPost]
        public ActionResult VHistorialPagoProv(EFechaval modelo, FormCollection coleccion)
        {
            VEncFacturaProvSaldoContext dbr = new VEncFacturaProvSaldoContext();
            string FI = modelo.fechaini.Year.ToString() + "-" + modelo.fechaini.Month.ToString() + "-" + modelo.fechaini.Day.ToString();
            string FF = modelo.fechafin.Year.ToString() + "-" + modelo.fechafin.Month.ToString() + "-" + modelo.fechafin.Day.ToString();

            int prov = int.Parse(modelo.IDProveedor.ToString());
            string cual = coleccion.Get("Enviar");
            string cadena = string.Empty;
            string cadena1 = "select * from VHistorialPagoProv";
            string cadena2 = string.Empty;
            string cadena3 = string.Empty;
            string cadena4 = "order by Nombre_Proveedor, Numero, ImporteSaldoInsoluto";


            if (cual == "Generar reporte")
            {
                return View();
            }
            if (cual == "Generar excel")
            {
                if (prov == 0)
                {
                    cadena2 = string.Empty;
                }
                else
                {
                    cadena2 = " where IDProveedor = " + modelo.IDProveedor + "";
                }
                if (cadena2 == string.Empty)
                {
                    cadena3 = "where Fecha >= DATEADD(day, -1, '" + FF + "') and Fecha <= DATEADD(day, 1, '" + FF + "'))";
                }
                else
                {
                    cadena3 = "and (Fecha >= DATEADD(day, -1, '" + FI + "') and Fecha <=  DATEADD(day, 1, '" + FF + "'))";
                }

                cadena = cadena1 + " " + cadena2 + " " + cadena3 + " " + cadena4;
                var datos = dbr.Database.SqlQuery<VHistorialPagoProv>(cadena).ToList();
                ViewBag.datos = datos;




                ExcelPackage Ep = new ExcelPackage();

                //Hoja 1
                //Crear la hoja y poner el nombre de la pestaña del libro
                var Sheet = Ep.Workbook.Worksheets.Add("Facturas vs Pagos");

                // en la fila1 formateo las celdas y coloco el título de la hoja
                // RichText permite agregar las propiedades de tipo de letra: color, negrita, etc.
                int row = 1;
                //Fijar la fuente para A1:Q1
                Sheet.Cells["A1:Q1"].Style.Font.Size = 20;
                Sheet.Cells["A1:Q1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:Q3"].Style.Font.Bold = true;
                Sheet.Cells["A1:Q1"].Style.Font.Color.SetColor(Color.DarkBlue);
                Sheet.Cells["A1:Q1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                Sheet.Cells["A1"].RichText.Add("Historial de Facturas Proveedores vs Pagos");

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
                //En la fila3 se da el formato a el encabezado
                row = 3;
                Sheet.Cells.Style.Font.Name = "Calibri";
                Sheet.Cells.Style.Font.Size = 10;
                Sheet.Cells["A3:Q3"].Style.Font.Bold = true;
                Sheet.Cells["A3:Q3"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                Sheet.Cells["A3:Q3"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                //Se establece el nombre que identifica a cada una de las columnas de datos.
                Sheet.Cells["A3"].RichText.Add("Ticket");
                Sheet.Cells["B3"].RichText.Add("Serie Factura"); ;
                Sheet.Cells["C3"].RichText.Add("Folio Factura");
                Sheet.Cells["D3"].RichText.Add("Fecha Factura");
                Sheet.Cells["E3"].RichText.Add("Proveedor");
                Sheet.Cells["F3"].RichText.Add("Subtotal");
                Sheet.Cells["G3"].RichText.Add("Iva");
                Sheet.Cells["H3"].RichText.Add("Total");
                Sheet.Cells["I3"].RichText.Add("Moneda");
                Sheet.Cells["J3"].RichText.Add("TC Factura");
                Sheet.Cells["K3"].RichText.Add("Serie Pago");
                Sheet.Cells["L3"].RichText.Add("Folio Pago");
                Sheet.Cells["M3"].RichText.Add("FechaPago");
                Sheet.Cells["N3"].RichText.Add("No. Parcialidad");
                Sheet.Cells["O3"].RichText.Add("ImportePagado");
                Sheet.Cells["P3"].RichText.Add("TC Pago");
                Sheet.Cells["Q3"].RichText.Add("Importe Saldo Insoluto");


                //Aplicar borde doble al rango de celdas A3:Q3
                Sheet.Cells["A3:Q3"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;

                // en la fila4, el foreach llenara cada fila de la hoja de excel, con cada fila que trae de la bd.
                // Se establecen los formatos para las celdas: Fecha, Moneda
                row = 4;
                Sheet.Cells.Style.Font.Bold = false;
                foreach (VHistorialPagoProv item2 in ViewBag.datos)
                {

                    Sheet.Cells[string.Format("A{0}", row)].Value = item2.ID;
                    Sheet.Cells[string.Format("B{0}", row)].Value = item2.Serie;
                    Sheet.Cells[string.Format("C{0}", row)].Value = item2.Numero;
                    Sheet.Cells[string.Format("D{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    Sheet.Cells[string.Format("D{0}", row)].Value = item2.Fecha;
                    Sheet.Cells[string.Format("E{0}", row)].Value = item2.Nombre_Proveedor;
                    Sheet.Cells[string.Format("F{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("F{0}", row)].Value = item2.subtotal;
                    Sheet.Cells[string.Format("G{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("G{0}", row)].Value = item2.iva;
                    Sheet.Cells[string.Format("H{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("H{0}", row)].Value = item2.total;
                    Sheet.Cells[string.Format("I{0}", row)].Value = item2.Moneda;
                    Sheet.Cells[string.Format("J{0}", row)].Value = item2.TCFactura;
                    Sheet.Cells[string.Format("K{0}", row)].Value = item2.SerieP;
                    Sheet.Cells[string.Format("L{0}", row)].Value = item2.FolioP;
                    if (item2.FechaPago == null)
                    {

                    }
                    else
                    {
                        Sheet.Cells[string.Format("M{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                        Sheet.Cells[string.Format("M{0}", row)].Value = item2.FechaPago;
                    }
                    Sheet.Cells[string.Format("N{0}", row)].Value = item2.NoParcialidad;
                    Sheet.Cells[string.Format("O{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("O{0}", row)].Value = item2.importepagado;
                    Sheet.Cells[string.Format("p{0}", row)].Value = item2.TCPago;
                    Sheet.Cells[string.Format("Q{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                    Sheet.Cells[string.Format("Q{0}", row)].Value = item2.importeSaldoInsoluto;
                    row++;

                }
                ///Fin hoja 1
                ///Sheet.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment: filename=" + "VHistorialPagoProv.xlsx");
                Response.BinaryWrite(Ep.GetAsByteArray());
                Response.End();
                return Redirect("/blah");
            }
            return Redirect("index");
        }


        public ActionResult HistorialCortePagoProv()
        {
            //Buscar proveedor
            ProveedorContext prov = new ProveedorContext();
            var proveedor = prov.Proveedores.OrderBy(s => s.Empresa).ToList();
            List<SelectListItem> li = new List<SelectListItem>();
            li.Add(new SelectListItem { Text = "--Selecciona Proveedor--", Value = "0" });

            foreach (var m in proveedor)
            {
                li.Add(new SelectListItem { Text = m.Empresa, Value = m.IDProveedor.ToString() });


            }
            ViewBag.proveedor = li;

            EFechaCorte elemento = new EFechaCorte();
            return View(elemento);
        }

        [HttpPost]
        public ActionResult HistorialCortePagoProv(EFechaCorte modelo, FormCollection coleccion)
        {
            VEncFacturaProvSaldoContext dbr = new VEncFacturaProvSaldoContext();
            string FI = modelo.fechaini.Year.ToString() + "-" + modelo.fechaini.Month.ToString() + "-" + modelo.fechaini.Day.ToString();
            int prov = int.Parse(modelo.IDProveedor.ToString());
            string cual = coleccion.Get("Enviar");
            string cadena = string.Empty;
            string cadena1 = "select * from dbo.VHistorialPagoProv";
            string cadena2 = string.Empty;
            string cadena3 = string.Empty;
            string cadena4 = "order by Nombre_Proveedor, Numero, ImporteSaldoInsoluto";
            List<VHistorialPagoProv> listafinal = null;

            if (cual == "Generar reporte")
            {
                return View();
            }
            if (cual == "Generar excel")
            {
                if (prov == 0)
                {
                    cadena2 = string.Empty;
                }
                else
                {
                    cadena2 = " where IDProveedor = " + modelo.IDProveedor + "";
                }
                if (cadena2 == string.Empty)
                {
                    cadena3 = "where FechaPago <=  DATEADD(day, 1, '" + FI + "')";
                }
                else
                {
                    cadena3 = "and FechaPago <= DATEADD(day, 1, '" + FI + "')";
                }

                cadena = cadena1 + " " + cadena2 + " " + cadena3 + " " + cadena4;
                List<VHistorialPagoProv> datos = db.Database.SqlQuery<VHistorialPagoProv>(cadena).ToList();
                Hashtable tabla = new Hashtable();

                foreach (VHistorialPagoProv elemento in datos)
                {
                    string llave = elemento.Serie + "," + elemento.Numero;
                    if (!tabla.ContainsKey(llave))
                    {
                        tabla.Add(llave, elemento);
                    }
                    else
                    {
                    }
                }
                //try{
                //    foreach (object llave in tabla.Keys)
                //    {
                //        string llave2 = llave.ToString();
                //        VHistorialPagoProv elemento = (VHistorialPagoProv)tabla[llave2];
                //        if (elemento.importeSaldoInsoluto == 0)
                //        {
                //            tabla.Remove(llave);
                //        }
                //    }
                //}
                //catch (Exception err)
                //{
                //    string error = err.Message;
                //}


                listafinal = new List<VHistorialPagoProv>();
                foreach (string llave in tabla.Keys)
                {
                    VHistorialPagoProv elemento = (VHistorialPagoProv)tabla[llave];

                    listafinal.Add(elemento);
                }

                ExcelPackage Ep = new ExcelPackage();

                //Hoja 1
                //Crear la hoja y poner el nombre de la pestaña del libro
                var Sheet = Ep.Workbook.Worksheets.Add("Saldos de Facturas");

                // en la fila1 formateo las celdas y coloco el título de la hoja
                // RichText permite agregar las propiedades de tipo de letra: color, negrita, etc.
                int row = 1;
                //Fijar la fuente para A1:Q1
                Sheet.Cells["A1:K1"].Style.Font.Size = 20;
                Sheet.Cells["A1:K1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:K3"].Style.Font.Bold = true;
                Sheet.Cells["A1:K1"].Style.Font.Color.SetColor(Color.DarkBlue);
                Sheet.Cells["A1:K1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                Sheet.Cells["A1"].RichText.Add("Saldos de Proveedores");

                row = 2;
                Sheet.Cells["A1:K1"].Style.Font.Size = 12;
                Sheet.Cells["A1:K1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:K1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
                Sheet.Cells["A2:K2"].Style.Font.Bold = true;
                //Subtitulo según el filtrado del periodo de datos
                row = 2;
                Sheet.Cells[string.Format("A2", row)].Value = "Fecha de corte";
                Sheet.Cells[string.Format("C2", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("C2", row)].Value = FI;

                //En la fila3 se da el formato a el encabezado
                row = 3;
                Sheet.Cells.Style.Font.Name = "Calibri";
                Sheet.Cells.Style.Font.Size = 10;
                Sheet.Cells["A3:L3"].Style.Font.Bold = true;
                Sheet.Cells["A3:L3"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                Sheet.Cells["A3:L3"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                //Se establece el nombre que identifica a cada una de las columnas de datos.
                Sheet.Cells["A3"].RichText.Add("Ticket");
                Sheet.Cells["B3"].RichText.Add("Serie Factura"); ;
                Sheet.Cells["C3"].RichText.Add("Folio Factura");
                Sheet.Cells["D3"].RichText.Add("Fecha Factura");
                Sheet.Cells["E3"].RichText.Add("Proveedor");
                Sheet.Cells["F3"].RichText.Add("Subtotal");
                Sheet.Cells["G3"].RichText.Add("Iva");
                Sheet.Cells["H3"].RichText.Add("Total");
                Sheet.Cells["I3"].RichText.Add("Moneda");
                Sheet.Cells["J3"].RichText.Add("TC");
                Sheet.Cells["K3"].RichText.Add("Importe Saldo Insoluto");
                Sheet.Cells["L3"].RichText.Add("Importe Saldo Insoluto en Pesos");

                //Aplicar borde doble al rango de celdas A3:Q3
                Sheet.Cells["A3:L3"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;

                // en la fila4, el foreach llenara cada fila de la hoja de excel, con cada fila que trae de la bd.
                // Se establecen los formatos para las celdas: Fecha, Moneda
                row = 4;
                Sheet.Cells.Style.Font.Bold = false;
                foreach (VHistorialPagoProv item2 in listafinal)
                {
                    if (item2.importeSaldoInsoluto != 0)
                    {
                        Sheet.Cells[string.Format("A{0}", row)].Value = item2.ID;
                        Sheet.Cells[string.Format("B{0}", row)].Value = item2.Serie;
                        Sheet.Cells[string.Format("C{0}", row)].Value = item2.Numero;
                        Sheet.Cells[string.Format("D{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                        Sheet.Cells[string.Format("D{0}", row)].Value = item2.Fecha;
                        Sheet.Cells[string.Format("E{0}", row)].Value = item2.Nombre_Proveedor;
                        Sheet.Cells[string.Format("F{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                        Sheet.Cells[string.Format("F{0}", row)].Value = item2.subtotal;
                        Sheet.Cells[string.Format("G{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                        Sheet.Cells[string.Format("G{0}", row)].Value = item2.iva;
                        Sheet.Cells[string.Format("H{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                        Sheet.Cells[string.Format("H{0}", row)].Value = item2.total;
                        Sheet.Cells[string.Format("I{0}", row)].Value = item2.Moneda;
                        Sheet.Cells[string.Format("J{0}", row)].Value = item2.TCFactura;
                        Sheet.Cells[string.Format("K{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                        Sheet.Cells[string.Format("K{0}", row)].Value = item2.importeSaldoInsoluto;
                        Sheet.Cells[string.Format("L{0}", row)].Style.Numberformat.Format = "$#,##0.00";
                        Sheet.Cells[string.Format("L{0}", row)].Value = item2.importeSaldoInsoluto * item2.TCFactura;
                        row++;
                    }
                    else { }


                }
                ///Fin hoja 1
                ///Sheet.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment: filename=" + "VHistorialPagoProv.xlsx");
                Response.BinaryWrite(Ep.GetAsByteArray());
                Response.End();
                return Redirect("/blah");
            }
            return Redirect("index");
        }




    }
}



class datosproveedor
{
    public int IDMoneda { get; set; }
    public int IDMetodoPago { get; set; }
    public int IDFormaPago { get; set; }
}


