using PagedList;
using SIAAPI.Models.Comercial;
using SIAAPI.Reportes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SIAAPI.Controllers.Comercial
{
    public class EmpaqueController : Controller
    {
        // GET: Empaque
        public ActionResult Index(string Numero, string ClieFac, string sortOrder, string currentFilter, int? page, int? PageSize, string Fechainicio, string Fechafinal, string Estado = "Solicitado")
        {
            //string ConsultaSql = "select * from Encfacturas ";
            string ConsultaSql = "Select * from dbo.EncPack ";
            string cadenaSQl = string.Empty;

            string Filtro = string.Empty;
            try
            {

                //Buscar Facturas: Pagadas o no pagadas
                var FacPagLst = new List<SelectListItem>();


                var EstadoLst = new List<SelectListItem>();
                //FacPagLst.Add(new SelectListItem { Text = "Todos", Value = "na", Selected = true });

                EstadoLst.Add(new SelectListItem { Text = "Cancelado", Value = "Cancelado" });
                EstadoLst.Add(new SelectListItem { Text = "Empacado", Value = "Empacado" });
                EstadoLst.Add(new SelectListItem { Text = "Solicitado", Value = "Solicitado" });

                ViewData["Estado"] = EstadoLst;

                ViewBag.Estado = new SelectList(EstadoLst, "Value", "Text");

            


              

                ViewBag.ClieFac = new ClienteRepository().GetClientes();

                ViewBag.ClieFacseleccionado = ClieFac;/// mandar el viewbag el parametro que viene de la pagina anterior

                


                string Orden = " order by idencpack desc ";

               
                //Buscar por numero
                if (!String.IsNullOrEmpty(Numero))
                {
                    if (Filtro == string.Empty)
                    {
                        Filtro = "where IDPedido=" + Numero + "";
                    }
                    else
                    {
                        Filtro += "and  IDPedido=" + Numero + "";
                    }

                }

                ///tabla filtro: Nombre Cliente
                if (!String.IsNullOrEmpty(ClieFac))
                {

                    if (Filtro == string.Empty)
                    {
                        Filtro = "where Cliente='" + ClieFac + "'";
                    }
                    else
                    {
                        Filtro += "and  Cliente='" + ClieFac + "'";
                    }

                }


                if (Estado != "Todos")
                {
                    if (Estado == "Cancelado")
                    {
                        if (Filtro == string.Empty)
                        {
                            Filtro = "where status='Cancelado'";
                        }
                        else
                        {
                            Filtro += "and  status='Cancelado'";
                        }
                    }
                    if (Estado == "Solicitado")
                    {
                        if (Filtro == string.Empty)
                        {
                            Filtro = "where  status='Solicitado'";
                        }
                        else
                        {
                            Filtro += "and status='Solicitado'";
                        }
                    }
                    if (Estado == "Empacado")
                    {
                        if (Filtro == string.Empty)
                        {
                            Filtro = "where  status='Empacado'";
                        }
                        else
                        {
                            Filtro += "and status='Empacado'";
                        }
                    }
                }



                if (!String.IsNullOrEmpty(Fechainicio) && String.IsNullOrEmpty(Fechafinal)) //pusieron una fecha
                {
                    if (Filtro == string.Empty)
                    {
                        Filtro = "where  Fecha BETWEEN '" + Fechainicio + " 00:00:00.1' and '" + Fechainicio + " 23:59:59.999' ";
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


                ViewBag.CurrentSort = sortOrder;
             
                ViewBag.NumeroSortParm = String.IsNullOrEmpty(sortOrder) ? "Numero" : "";
                ViewBag.FechaSortParm = sortOrder == "Fecha" ? "Fecha" : "";
                ViewBag.ClienteSortParm = String.IsNullOrEmpty(sortOrder) ? "Cliente" : "";




                // Pass filtering string to view in order to maintain filtering when paging
                ViewBag.Fechainicio = Fechainicio;
                ViewBag.Fechafinal = Fechafinal;

                ////Paginación
                //if (Fechainicio != null)
                //{
                //    page = 1;
                //}
                //else
                //{
                //    Fechainicio = currentFilter;
                //}

                //ViewBag.CurrentFilter = Fechainicio;


                //Ordenacion

                switch (sortOrder)
                {
                    case "Estado":
                        Orden = " order by  status ";
                        break;
                   
                    case "Numero":
                        Orden = " order by   numero asc ";
                        break;
                    case "Fecha":
                        Orden = " order by fecha ";
                        break;
                    case "Cliente":
                        Orden = " order by  Nombre_cliente ";
                        break;
                    default:
                        Orden = " order by idencpack  desc ";
                        break;
                }


                // si no ha seleccionado nada muestra las facturas del ultimo mes 

                if (Filtro == " where  status='Solicitado'")
                {
                    Filtro += " and Fecha >='" + DateTime.Now.AddDays(-30).Year + "-" + DateTime.Now.AddDays(-30).Month + "-" + DateTime.Now.AddDays(-30).Day + "'  and  Fecha<'" + DateTime.Now.AddDays(1).Year + "-" + DateTime.Now.AddDays(1).Month + "-" + DateTime.Now.AddDays(1).Day + "' ";
                    ViewBag.Fechainicio = "" + DateTime.Now.AddDays(-30).Year + "-" + DateTime.Now.AddDays(-30).Month + "-" + DateTime.Now.AddDays(-30).Day + "";
                    ViewBag.Fechafinal = "" + DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + "";
                }

                //var elementos = from s in db.encfacturas
                //select s;

                cadenaSQl = ConsultaSql + " " + Filtro + " " + Orden;

               
                var elementos = new EncPackContext().Database.SqlQuery<EncPack>(cadenaSQl).ToList();



                //Paginación
                // DROPDOWNLIST FOR UPDATING PAGE SIZE
                //int count = db.Encfacturas.OrderBy(e => e.Serie).Count();// Total number of elements
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

                var reshtml = Server.HtmlEncode(cadenaSQl);

                return Content(reshtml);
            }
        }

        public ActionResult Details (int id)
        {
            EncPack enc = new EncPackContext().EncPackaging.Find(id);
            EncPedido pedido = new PedidoContext().EncPedidos.Find(enc.idPedido);
            Clientes cliente = new ClientesContext().Clientes.Find(pedido.IDCliente);
            ViewBag.EncPack = enc;
            ViewBag.Pedido = pedido;
            ViewBag.Cliente = cliente;
            List<detpack> detalles = new EncPackContext().detalles.Where(s => s.idEncPack == id).ToList();
            return View(detalles);
        }

        public ActionResult Empacar(int id)
        {
            EncPack enc = new EncPackContext().EncPackaging.Find(id);
            EncPedido pedido = new PedidoContext().EncPedidos.Find(enc.idPedido);
            Clientes cliente = new ClientesContext().Clientes.Find(pedido.IDCliente);
            ViewBag.EncPack = enc;
            ViewBag.Pedido = pedido;
            ViewBag.Cliente = cliente;
            List<detpack> detalles = new EncPackContext().detalles.Where(s => s.idEncPack == id).ToList();
            return View(detalles);
        }

        public ActionResult EditaArticulo(int? id)
        {
            detpack elemento = new EncPackContext().detalles.Find(id);
            
            return PartialView(elemento);
        }

        [HttpPost]

        public ActionResult editArticulo(detpack elemento)
        {
            
            new EncPackContext().Database.ExecuteSqlCommand("update detpack set Kilos="+elemento.Kilos+", Lote='" + elemento.Lote + "', LoteMp='" + elemento.LoteMp + "', serie='" + elemento.Serie + "', CantEmp=" + elemento.CantEmp + ", Cajas=" + elemento.Cajas + ", Paquetes=" + elemento.Paquetes + ", Pedimento='" + elemento.Pedimento + "', Observacion='" + elemento.Observacion + "', Estado='Empacado' where iddetpack="+ elemento.iddetpack);

            EncPack enc = new EncPackContext().EncPackaging.Find(elemento.idEncPack);

            List<detpack> detalles = new EncPackContext().detalles.Where(s => s.idEncPack == enc.idencpack || s.Estado != "Cancelado").ToList();

            bool termina = true;
            foreach(detpack detalle in detalles )
            {
                if (detalle.Estado=="SOLICITADO")
                {
                    termina = false;
                }
            }
            string fechaEm = DateTime.Now.ToString();
            if (termina)
            {
                new EncPackContext().Database.ExecuteSqlCommand("update Encpack set status='Empacado', fechaEmpacada='"+ fechaEm + "' where idencpack=" + enc.idencpack);
            }

            return RedirectToAction("Empacar", new { id = elemento.idEncPack });
        }

        public ActionResult Cancelar(int id)
        {
            try
            {
                new EncPackContext().Database.ExecuteSqlCommand("update Encpack set status='Cancelado' where idencpack=" + id);
                new EncPackContext().Database.ExecuteSqlCommand("update detpack set Estado='Cancelado' where idencpack=" + id);
                return RedirectToAction("Index", new { Estado = "Solicitado" });
            }
            catch (Exception err)
            {
                return RedirectToAction("Index", new { Estado = "Solicitado" });
            }
        }

        public void PdfEmpaque(int id)
        {

            EncPack empaque = new EncPackContext().EncPackaging.Find(id);
            EncPedido pedido = new PedidoContext().EncPedidos.Find(empaque.idPedido);
            Clientes cliente = new ClientesContext().Clientes.Find(pedido.IDCliente);

            DocumentoEm x = new DocumentoEm();

            x.Cliente = empaque.Cliente;
            x.Observacion = empaque.observa;
            x.fecha = empaque.Fecha.ToShortDateString();
            x.version = empaque.Version;
            x.estado = empaque.status;
            x.lugarEntrega = pedido.Entrega;
            x.facturacionExacta = cliente.FacturacionExacta;
            x.IDEncpack = empaque.idencpack;
            x.IDPedido = empaque.idPedido;
            x.serie = "";
            x.folio = empaque.idPedido.ToString();


            EmpresaContext dbe = new EmpresaContext();

            var empresa = dbe.empresas.Single(m => m.IDEmpresa == 2);
            x.Empresa = empresa.RazonSocial;
            x.Telefono = empresa.Telefono;
            x.RFC = empresa.RFC;
            x.Direccion = empresa.Calle + " " + empresa.NoExt + " " + empresa.NoInt + "\n" + empresa.Colonia + " \n" + empresa.Municipio + "," + empresa.Estados.Estado;
            x.firmadefinanzas = empresa.Director_finanzas;
            x.firmadecompras = empresa.Persona_de_Compras + "";

            List<detpack> detalles = new EncPackContext().Database.SqlQuery<detpack>("select * from [dbo].[detpack] where [idEncPack]= " + id).ToList();

            int contador = 1;
            foreach (var item in detalles)
            {
                ProductoEm producto = new ProductoEm();

                detpack detempaque = new EncPackContext().detalles.Find(empaque.idencpack);
                producto.iddetempaque = item.iddetpack;
                producto.cref = item.Cref;
                producto.NP = item.NP;
                producto.cantidad = decimal.Parse(item.Cantidad.ToString());
                producto.lote = item.Lote;
                producto.loteMP = item.LoteMp;
                producto.serie = item.Serie;
                producto.pedimento = item.Pedimento;
                producto.cantEmp = decimal.Parse(item.CantEmp.ToString());
                producto.idorden = int.Parse(item.IDOrden.ToString());
                producto.observacion = item.Observacion;
                producto.status = item.Estado;
                producto.cajas = int.Parse(item.Cajas.ToString());
                producto.paquetes = int.Parse(item.Paquetes.ToString());
                ///

                ///
                producto.numIdentificacion = contador.ToString();
                contador++;

                x.productos.Add(producto);

            }



            System.Drawing.Image logoempresa = byteArrayToImage(empresa.Logo);

            try
            {


                CreaEmPDF documento = new CreaEmPDF(logoempresa, x);

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

    }
}