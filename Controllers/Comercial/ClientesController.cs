using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using SIAAPI.Models.Comercial;
using PagedList;
using SIAAPI.Models.Login;
using System.Security.Cryptography;
using System.Text;
using SIAAPI.ViewModels.Comercial;

using System.IO;
using OfficeOpenXml;
using System.Drawing;
using OfficeOpenXml.Style;
using SIAAPI.Reportes;
using System.Globalization;
using SIAAPI.Models.CartaPorte;
using SIAAPI.Models.Administracion;

namespace SIAAPI.Controllers.Comercial
{
    [Authorize(Roles = "Administrador,Gerencia, Comercial,Sistemas,AdminProduccion, Compras, Almacenista, Ventas, Produccion, GerenteVentas")]
    public class ClientesController : Controller
    {
        private ClientesContext db = new ClientesContext();
        private VClienteContext dbv = new VClienteContext();
        // GET: Clientes
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NombreSortParm = String.IsNullOrEmpty(sortOrder) ? "Nombre" : "Nombre";
            ViewBag.GrupoSortParm = String.IsNullOrEmpty(sortOrder) ? "Grupo" : "Grupo";
            ViewBag.MunicipioSortParm = String.IsNullOrEmpty(sortOrder) ? "Municipio" : "Municipio";
            ViewBag.EstadoSortParm = String.IsNullOrEmpty(sortOrder) ? "Estado" : "Estado";


            // Pass filtering string to view in order to maintain filtering when paging
            ViewBag.SearchString = searchString;
            //Paginación

            var elementos = from s in dbv.VClientes
                            select s;
            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                elementos = elementos.Where(s => s.Nombre.Contains(searchString) || s.Estado.Contains(searchString) || s.Municipio.Contains(searchString) || s.Grupo.Contains(searchString) || s.Vendedor.Contains(searchString));
            }

            //Ordenacion

            switch (sortOrder)
            {
                case "Municipio":
                    //elementos = elementos.OrderByDescending(s => s.IDProveedor);
                    if (elementos == elementos.OrderBy(s => s.Municipio))
                    {
                        elementos = elementos.OrderByDescending(s => s.Municipio);
                    }

                    else
                    {
                        elementos = elementos.OrderBy(s => s.Municipio);
                    }
                    break;
                case "Nombre":
                    if (elementos == elementos.OrderBy(s => s.Nombre))
                    {
                        elementos = elementos.OrderByDescending(s => s.Nombre);
                    }
                    else
                    {
                        elementos = elementos.OrderBy(s => s.Nombre);
                    }
                    break;
                case "Estado":
                    if (elementos == elementos.OrderBy(s => s.Estado))
                    {
                        elementos = elementos.OrderByDescending(s => s.Estado);
                    }
                    else
                    {
                        elementos = elementos.OrderBy(s => s.Estado);
                    }
                    break;
                case "Grupo":
                    if (elementos == elementos.OrderBy(s => s.Grupo))
                    {
                        elementos = elementos.OrderByDescending(s => s.Grupo);
                    }

                    else
                    {
                        elementos = elementos.OrderBy(s => s.Grupo);
                    }
                    break;
                default:
                    elementos = elementos.OrderBy(s => s.Nombre);
                    break;
            }

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = dbv.VClientes.Count(); // Total number of elements

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
            //return View("Index", "_LayoutCopy", elementos.ToPagedList(pageNumber, pageSize));
            return View(elementos.ToPagedList(pageNumber, pageSize));
            //Paginación
        }
        // GET: Clientes/Details/5
        public ActionResult Details(int id, string searchString="")
        {
            ViewBag.searchString = searchString;
            System.Web.HttpContext.Current.Session["IDCliente"] = id;
            Session["IDCliente"] = id;
           Clientes clientes = db.Clientes.Find(id);
            if (clientes== null)
            {
                return HttpNotFound();
            }
            return View(clientes);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////7

        public ActionResult getJsonEstadoPorPais(int id)
        {
            var estado = new EstadosRepository().GetEstadoPorPais(id);
            return Json(estado, JsonRequestBehavior.AllowGet); ;

        }

        public IEnumerable<SelectListItem> getEstadoPorPais(int idp)
        {
            var estado = new EstadosRepository().GetEstadoPorPais(idp);
            return estado;

        }
        public string convertirfechaamericana(string data)
        {
            DateTime fecha = DateTime.Parse(data);
            string nuevafecha = fecha.Year + "-" + fecha.Month + "-" + fecha.Day;
            return nuevafecha;
        }

        // GET: Clientes/Create
        public ActionResult Create(int id = 0)
        {
            ViewData["ClientePros"] = id;
            //ViewBag.Prospecto = "No";
            ViewData["Prospecto"] = "NO";
            ViewBag.pais = "";
            ViewBag.estado = "";
            Clientes cliente = new Clientes();
            if (id != 0)
            {
                ViewData["Prospecto"] = "SI";

                try
                {
                    ClientesPContext dbc = new ClientesPContext();
                    ClientesP clientesp = dbc.ClientesPs.Find(id);
                    cliente.Nombre = clientesp.Nombre;
                    cliente.Correo = clientesp.Correo;
                    cliente.Vendedor = clientesp.Vendedor;
                    
                    cliente.Telefono = clientesp.Telefono;
                    


                    ViewBag.IDCondicionesPago = new SelectList(db.CondicionesPagos, "IDCondicionesPago", "Descripcion");
                    ViewBag.IDFormapago = new SelectList(db.c_FormaPagos, "IDFormaPago", "Descripcion");
                    ViewBag.IDGrupo = new SelectList(db.c_Grupos, "IDGrupo", "Descripcion");
                    ViewBag.IDMetodoPago = new SelectList(db.c_MetodoPagos, "IDMetodoPago", "Descripcion");
                    ViewBag.IDMoneda = new SelectList(db.c_Monedas, "IDMoneda", "Descripcion");
                    ViewBag.IDRegimenFiscal = new SelectList(db.c_RegimenFiscales, "IDRegimenFiscal", "Descripcion");
                    //ViewBag.IDEstado = new SelectList(db.estados, "IDEstado", "Estado",);
                    ViewBag.IDPais = new EstadosRepository().GetPaisSelec(cliente.IDPais);
                    ViewBag.IDEstado = new EstadosRepository().GetEstadoPorPaisSelec(cliente.IDPais, cliente.IDEstado);
                    ViewBag.IDOficina = new SelectList(db.Oficinas, "IDOficina", "NombreOficina");
                    ViewBag.IDVendedor = new SelectList(db.Vendedores.AsNoTracking().Where(n => n.Activo.Equals(true)), "IDVendedor", "Nombre");
                    var listas = new ElementosRepository().GetStatus();
                    ViewBag.ComboStatus = listas;
                    ViewBag.IDUsoCFDI = new SelectList(db.c_UsoCFDIS.OrderBy(s => s.Descripcion), "IDUsoCFDI", "Descripcion");



                   
                    /* new EstadoRepository().GetEstadoPorPaisSelec(clientesp.IDPais, clientesp.IDEstado);*/



                }
                catch (Exception err)
                {

                }

            }
            else
            {
                try
                {
                    ViewBag.IDCondicionesPago = new SelectList(db.CondicionesPagos, "IDCondicionesPago", "Descripcion");
                    ViewBag.IDFormapago = new SelectList(db.c_FormaPagos, "IDFormaPago", "Descripcion");
                    ViewBag.IDGrupo = new SelectList(db.c_Grupos, "IDGrupo", "Descripcion");
                    ViewBag.IDMetodoPago = new SelectList(db.c_MetodoPagos, "IDMetodoPago", "Descripcion");
                    ViewBag.IDMoneda = new SelectList(db.c_Monedas, "IDMoneda", "Descripcion");
                    ViewBag.IDRegimenFiscal = new SelectList(db.c_RegimenFiscales, "IDRegimenFiscal", "Descripcion");
                    ViewBag.IDEstado = new SelectList(db.estados, "IDEstado", "Estado");
                    ViewBag.IDOficina = new SelectList(db.Oficinas, "IDOficina", "NombreOficina");
                    ViewBag.IDVendedor = new SelectList(db.Vendedores.AsNoTracking().Where(n => n.Activo.Equals(true)), "IDVendedor", "Nombre");
                    var listas = new ElementosRepository().GetStatus();
                    ViewBag.ComboStatus = listas;
                    ViewBag.IDUsoCFDI = new SelectList(db.c_UsoCFDIS.OrderBy(s => s.Descripcion), "IDUsoCFDI", "Descripcion");

                    //Paises
                    var datosPaises = db.paises.OrderBy(i => i.Pais).ToList();
                    List<SelectListItem> liP = new List<SelectListItem>();
                    liP.Add(new SelectListItem { Text = "--Selecciona un Pais--", Value = "0" });
                    foreach (var a in datosPaises)
                    {
                        liP.Add(new SelectListItem { Text = a.Pais, Value = a.IDPais.ToString() });

                    }
                    ViewBag.ListPais = liP;
                    ViewBag.ListEstado = getEstadoPorPais(0);
                }
                catch (Exception err)
                {

                }


            }


            return View(cliente);
        }

        // POST: Clientes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Clientes clientes, FormCollection coleccion)
        {



            //clientes.IDPais = int.Parse(clientes.IDPais.ToString());
            //clientes.IDEstado = int.Parse(clientes.IDEstado.ToString());
            int idpais = clientes.IDPais;
            int idedo = clientes.IDEstado;
            ViewBag.error = "";

            string pros = coleccion.Get("prospecto");
            string idprospecto = coleccion.Get("idprospecto");
            string FEC = clientes.Ultimaventa.Year.ToString() + "-" + clientes.Ultimaventa.Month.ToString() + "-" + clientes.Ultimaventa.Day.ToString();
            DateTime FECHA = DateTime.Now;
            string fechaActual = FECHA.Year.ToString() + "-" + FECHA.Month.ToString() + "-" + FECHA.Day.ToString();
            try
            {
                if (ModelState.IsValid)
                {
                    try
                    {


                        string cadena = "INSERT INTO dbo.Clientes(Nombre, Mayorista, IDGrupo, IDRegimenFiscal, Status, IDOficina, Correo, Password, Telefono, Calle, NumExt, NumInt, Colonia, Municipio, CP, IDEstado, IDVendedor, Observacion, Curp, VentasAcu, Ultimaventa, RFC, CorreoCfdi, IDFormapago, IDMetodoPago, IDMoneda, IDCondicionesPago, FacturacionExacta, IDUsoCFDI, CertificadoCalidad, IDPais,cuentaContable, CorreoPagoC, noExpediente, SinFactura,Nombre40, RegimenSocietario)";
                        cadena = cadena + "VALUES('" + clientes.Nombre + "', '" + clientes.Mayorista + "', " + clientes.IDGrupo + ", " + clientes.IDRegimenFiscal + ",'" + clientes.Status + "'," + clientes.IDOficina + ", '" + clientes.Correo + "', '" + clientes.Password + "', '" + clientes.Telefono + "','" + clientes.Calle + "','" + clientes.NumExt + "', '" + clientes.NumInt + "','" + clientes.Colonia + "', '" + clientes.Municipio + "', '" + clientes.CP + "'," + idedo + ",'" + clientes.IDVendedor + "','" + clientes.Observacion + "','" + clientes.Curp + "','" + clientes.VentasAcu + "','" + FEC + "','" + clientes.RFC + "','" + clientes.CorreoCfdi + "','" + clientes.IDFormapago + "', '" + clientes.IDMetodoPago + "','" + clientes.IDMoneda + "','" + clientes.IDCondicionesPago + "','" + clientes.FacturacionExacta + "','" + clientes.IDUsoCFDI + "','" + clientes.CertificadoCalidad + "'," + idpais + "," + clientes.cuentaContable + ", '" + clientes.CorreoPagoC + "', '" + clientes.noExpediente + "', " +
                            "'" + clientes.SinFactura + "','"+clientes.Nombre40+"','"+clientes.RegimenSocietario+"')";
                        db.Database.ExecuteSqlCommand(cadena);
                        //db.Clientes.Add(clientes);
                        //db.SaveChanges();
                        ClsDatoEntero IDClienteR = db.Database.SqlQuery<ClsDatoEntero>("select MAX(IDCliente) as Dato from Clientes").ToList()[0];

                        if (pros == "SI")
                        {
                            // tabla clientep a cliente
                            List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
                            int usuario = userid.Select(s => s.UserID).FirstOrDefault();
                           
                            //string cadenat = "INSERT INTO dbo.ClientePaCliente(IDCliente, IDClienteP, Fecha, IDVendedor, IDUsuario)";
                            //cadenat = cadenat + "VALUES(" + IDClienteR.Dato + ", " + idprospecto + ", '" + FECHA.Year + "-" + FECHA.Month + "-" + FECHA.Day + "', " + clientes.IDVendedor + "," + usuario + ")";
                            //db.Database.ExecuteSqlCommand(cadenat);

                            string cadenac = "update [dbo].[ClientesP] set EsCliente = 'true', fechaContrato = '"+ fechaActual +"', IDCliente=" + IDClienteR.Dato + ", IDVendedor= " + clientes.IDVendedor + " where IDClienteP = " + idprospecto + "";
                            db.Database.ExecuteSqlCommand(cadenac);
                        }

                        string pass = MD5P(clientes.Password);

                        db.Database.ExecuteSqlCommand("insert into [dbo].[User]([Username],[Password],[EmailID],[Estado]) values ('" + clientes.Correo + "','" + pass + "','" + clientes.Nombre + "','Asignado')");
                        List<User> numero = db.Database.SqlQuery<User>("SELECT * FROM [dbo].[User] WHERE UserID = (SELECT MAX(UserID) from [dbo].[User])").ToList();
                        int num = numero.Select(s => s.UserID).FirstOrDefault();
                        ClsDatoEntero numrol = db.Database.SqlQuery<ClsDatoEntero>("select RoleID as Dato from Roles where ROleName='Cliente'").ToList()[0];
                        db.Database.ExecuteSqlCommand("insert into [dbo].[UserRole]([RoleID],[UserID]) values ('" + numrol.Dato + "','" + num + "')");

                        return RedirectToAction("Index");
                    }
                    catch (Exception err)
                    {
                        if (err.Message.Contains("Dupl"))
                        {
                            ViewBag.error = "Tu registro esta duplicado, verifica que el RFC no este duplicado";
                        }
                        else
                        {
                            ViewBag.error = "Verificar los datos ingresados";
                        }
                    }
                }
                ViewData["ClientePros"] = idprospecto;
                //ViewBag.Prospecto = "No";
                ViewData["Prospecto"] = pros;


                //ViewBag.IDCondicionesPago = new SelectList(db.CondicionesPagos, "IDCondicionesPago", "Descripcion", clientes.IDCondicionesPago);
                //ViewBag.IDFormapago = new SelectList(db.c_FormaPagos, "IDFormaPago", "Descripcion", clientes.IDFormapago);
                //ViewBag.IDGrupo = new SelectList(db.c_Grupos, "IDGrupo", "Descripcion", clientes.IDGrupo);
                //ViewBag.IDMetodoPago = new SelectList(db.c_MetodoPagos, "IDMetodoPago", "Descripcion", clientes.IDMetodoPago);
                //ViewBag.IDMoneda = new SelectList(db.c_Monedas, "IDMoneda", "Descripcion");
                //ViewBag.IDRegimenFiscal = new SelectList(db.c_RegimenFiscales, "IDRegimenFiscal", "Descripcion");
                //ViewBag.IDPais = new EstadoRepository().GetPaisSelec(clientes.IDPais);
                //ViewBag.IDEstado = new EstadoRepository().GetEstadoPorPaisSelec(clientes.IDPais, clientes.IDEstado);
                ////ViewBag.IDEstado = new SelectList(db.estados, "IDEstado", "Estado",);
                //ViewBag.IDOficina = new SelectList(db.Oficinas, "IDOficina", "NombreOficina");
                //ViewBag.IDVendedor = new SelectList(db.Vendedores.AsNoTracking().Where(n => n.Activo.Equals(true)), "IDVendedor", "Nombre");
                //var listass = new ElementosRepository().GetStatus();
                //ViewBag.ComboStatus = listass;
                //ViewBag.IDUsoCFDI = new SelectList(db.c_UsoCFDIS.OrderBy(s => s.Descripcion), "IDUsoCFDI", "Descripcion");
                ViewBag.IDCondicionesPago = new SelectList(db.CondicionesPagos, "IDCondicionesPago", "Descripcion", clientes.IDCondicionesPago);
                ViewBag.IDFormapago = new SelectList(db.c_FormaPagos, "IDFormaPago", "Descripcion",clientes.IDFormapago);
                ViewBag.IDGrupo = new SelectList(db.c_Grupos, "IDGrupo", "Descripcion", clientes.IDGrupo);
                ViewBag.IDMetodoPago = new SelectList(db.c_MetodoPagos, "IDMetodoPago", "Descripcion",clientes.IDMetodoPago);
                ViewBag.IDMoneda = new SelectList(db.c_Monedas, "IDMoneda", "Descripcion",clientes.IDMoneda);
                ViewBag.IDRegimenFiscal = new SelectList(db.c_RegimenFiscales, "IDRegimenFiscal", "Descripcion",clientes.IDRegimenFiscal);
                ViewBag.IDEstado = new SelectList(db.estados, "IDEstado", "Estado",clientes.IDEstado);
                ViewBag.IDOficina = new SelectList(db.Oficinas, "IDOficina", "NombreOficina");
                ViewBag.IDVendedor = new SelectList(db.Vendedores.AsNoTracking().Where(n => n.Activo.Equals(true)), "IDVendedor", "Nombre",clientes.IDOficina);
                var listas = new ElementosRepository().GetStatus();
                ViewBag.ComboStatus = listas;
                ViewBag.IDUsoCFDI = new SelectList(db.c_UsoCFDIS.OrderBy(s => s.Descripcion), "IDUsoCFDI", "Descripcion",clientes.IDUsoCFDI);

                //Paises
                var datosPaises = db.paises.OrderBy(i => i.Pais).ToList();
                List<SelectListItem> liP = new List<SelectListItem>();
                liP.Add(new SelectListItem { Text = "--Selecciona un Pais--", Value = "0" });
                foreach (var a in datosPaises)
                {
                    if (a.IDPais== clientes.IDPais)
                    {
                        liP.Add(new SelectListItem { Text = a.Pais, Value = a.IDPais.ToString(), Selected= true });
                    }
                    else
                    {
                        liP.Add(new SelectListItem { Text = a.Pais, Value = a.IDPais.ToString() });
                    }

                }
                ViewBag.ListPais = liP;
                ViewBag.ListEstado = getEstadoPorPais(clientes.IDPais);

                if (pros == "NO")
                {//    //Paises
                    var datosPaises1 = db.paises.OrderBy(i => i.Pais).ToList();
                    List<SelectListItem> liP1 = new List<SelectListItem>();
                    liP1.Add(new SelectListItem { Text = "--Selecciona un Pais--", Value = "0" });
                    foreach (var a in datosPaises1)
                    {
                        SelectListItem elementopais = new SelectListItem();
                        if (clientes.IDPais == a.IDPais)
                        {
                            elementopais.Text = a.Pais;
                            elementopais.Value = a.IDPais.ToString();
                            elementopais.Selected = true;
                            liP1.Add(elementopais);
                        }
                        else
                        {

                            liP1.Add(new SelectListItem { Text = a.Pais, Value = a.IDPais.ToString() });
                        }

                    }
                    ViewBag.ListPais = liP1;
                    ViewBag.ListEstado = getEstadoPorPais(0);
                }
             
                return View(clientes);
            }
            catch (Exception err)
            {

                //ViewData["Prospecto"] = "SI";
                return View(clientes);
            }
        }



        private string MD5P(string password)
        {
            MD5 md5 = MD5CryptoServiceProvider.Create();
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] stream = null;
            StringBuilder sb = new StringBuilder();
            stream = md5.ComputeHash(encoding.GetBytes(password));
            for (int i = 0; i < stream.Length; i++) sb.AppendFormat("{0:x2}", stream[i]);
            return sb.ToString();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////7

 

        // GET: Clientes/Edit/5
        public ActionResult Edit(int? id, string searchString ="")
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Clientes clientes = db.Clientes.Find(id);
            if (clientes == null)
            {
                return HttpNotFound();
            }
            ViewBag.IDFormapago = new SelectList(db.c_FormaPagos, "IDFormaPago", "Descripcion", clientes.IDFormapago);
            ViewBag.IDGrupo = new SelectList(db.c_Grupos, "IDGrupo", "Descripcion", clientes.IDGrupo);
            ViewBag.IDMetodoPago = new SelectList(db.c_MetodoPagos, "IDMetodoPago", "Descripcion", clientes.IDMetodoPago);
            ViewBag.IDMoneda = new SelectList(db.c_Monedas, "IDMoneda", "Descripcion", clientes.IDMoneda);
            ViewBag.IDRegimenFiscal = new SelectList(db.c_RegimenFiscales, "IDRegimenFiscal", "Descripcion", clientes.IDRegimenFiscal);
            ViewBag.IDCondicionesPago = new SelectList(db.CondicionesPagos, "IDCondicionesPago", "Descripcion", clientes.IDCondicionesPago);
            //ViewBag.IDEstado = new SelectList(db.estados, "IDEstado", "Estado", clientes.IDEstado);
            //ViewBag.IDPais = new SelectList(db.paises, "IDPais", "Pais", clientes.IDPais);
            ViewBag.IDOficina = new SelectList(db.Oficinas, "IDOficina", "NombreOficina", clientes.IDOficina);
            ViewBag.IDVendedor = new SelectList(db.Vendedores.AsNoTracking().Where(n => n.Activo.Equals(true)), "IDVendedor", "Nombre", clientes.IDVendedor);
            ViewBag.IDUsoCFDI = new SelectList(db.c_UsoCFDIS.OrderBy(s => s.Descripcion), "IDUsoCFDI", "Descripcion",clientes.IDUsoCFDI);
            var listas = new ElementosRepository().GetStatus();
            ViewBag.ComboStatus = listas;
          //  int pais = clientes.IDPais;
            //Paises
            ViewBag.IDPais = new EstadosRepository().GetPaisSelec(clientes.IDPais);
            ViewBag.IDEstado = new EstadosRepository().GetEstadoPorPaisSelec(clientes.IDPais, clientes.IDEstado);
            ViewBag.searchString = searchString;

            return View(clientes);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Clientes clientes, string searchString="")
        {
            if (ModelState.IsValid)
            {
                //db.Entry(clientes).State = System.Data.Entity.EntityState.Modified;
                // db.SaveChanges();
                int idpais = clientes.IDPais;
                int idedo = clientes.IDEstado;
                string cadena = "Update dbo.Clientes set Nombre ='" + clientes.Nombre + "' , Mayorista ='" + clientes.Mayorista + "' , IDGrupo = " + clientes.IDGrupo + ", IDRegimenFiscal = " + clientes.IDRegimenFiscal + ", Status='" + clientes.Status + "', IDOficina =" + clientes.IDOficina + ", Correo = '" + clientes.Correo + "', Password = '" + clientes.Password + "', Telefono = '" + clientes.Telefono + "', Calle = '" + clientes.Calle + "', NumExt = '" + clientes.NumExt + "', NumInt = '" + clientes.NumInt + "' , Colonia = '" + clientes.Colonia + "', Municipio= '" + clientes.Municipio + "', CP = '" + clientes.CP + "', IDEstado = " + idedo + ", IDVendedor = '" + clientes.IDVendedor + "', Observacion = '" + clientes.Observacion + "', Curp = '" + clientes.Curp + "', VentasAcu ='" + clientes.VentasAcu + "', Ultimaventa = '" + convertirfechaamericana(clientes.Ultimaventa.ToString()) + "', RFC= '" + clientes.RFC + "', CorreoCfdi = '" + clientes.CorreoCfdi + "', IDFormapago = '" + clientes.IDFormapago + "', IDMetodoPago = '" + clientes.IDMetodoPago + "', IDMoneda = '" + clientes.IDMoneda + "', IDCondicionesPago = '" + clientes.IDCondicionesPago + "', FacturacionExacta = '" + clientes.FacturacionExacta + "', IDUsoCFDI = '" + clientes.IDUsoCFDI + "' , CertificadoCalidad = '" + clientes.CertificadoCalidad + "', IDPais = " + idpais + ", cuentaContable='" + clientes.cuentaContable + "' , CorreoPagoC = '" + clientes.CorreoPagoC + "' , noExpediente= '" + clientes.noExpediente + "', SinFactura = '" + clientes.SinFactura + "', Nombre40='"+clientes.Nombre40+"', RegimenSocietario='"+clientes.RegimenSocietario+"'  where IDCliente = " + clientes.IDCliente + ""; 
                db.Database.ExecuteSqlCommand(cadena);
                return RedirectToAction("Index", new { searchString = searchString });
            }
            ViewBag.IDCondicionesPago = new SelectList(db.CondicionesPagos, "IDCondicionesPago", "ClaveCondicionesPago", clientes.IDCondicionesPago);
            ViewBag.IDFormapago = new SelectList(db.c_FormaPagos, "IDFormaPago", "Descripcion", clientes.IDFormapago);
            ViewBag.IDGrupo = new SelectList(db.c_Grupos, "IDGrupo", "Descripcion", clientes.IDGrupo);
            ViewBag.IDMetodoPago = new SelectList(db.c_MetodoPagos, "IDMetodoPago", "Descripcion", clientes.IDMetodoPago);
            ViewBag.IDMoneda = new SelectList(db.c_Monedas, "IDMoneda", "Descripcion", clientes.IDMoneda);
            ViewBag.IDRegimenFiscal = new SelectList(db.c_RegimenFiscales, "IDRegimenFiscal", "Descripcion", clientes.IDRegimenFiscal);
            //ViewBag.IDEstado = new SelectList(db.estados, "IDEstado", "Estado", clientes.IDEstado);
            //ViewBag.IDPais = new SelectList(db.paises, "IDPais", "Pais", clientes.IDPais);
            ViewBag.IDOficina = new SelectList(db.Oficinas, "IDOficina", "NombreOficina", clientes.IDOficina);
            ViewBag.IDVendedor = new SelectList(db.Vendedores.AsNoTracking().Where(n => n.Activo.Equals(true)), "IDVendedor", "Nombre", clientes.IDVendedor);
            ViewBag.IDUsoCFDI = new SelectList(db.c_UsoCFDIS.OrderBy(s => s.Descripcion), "IDUsoCFDI", "Descripcion",clientes.IDUsoCFDI);
            var listas = new ElementosRepository().GetStatus();
            ViewBag.ComboStatus = listas;

            //Paises
            ViewBag.IDPais = new EstadosRepository().GetPaisSelec(clientes.IDPais);
            ViewBag.IDEstado = new EstadosRepository().GetEstadoPorPaisSelec(clientes.IDPais, clientes.IDEstado);
            return View(clientes);
        }

        // GET: Clientes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Clientes clientes = db.Clientes.Find(id);
            if (clientes == null)
            {
                return HttpNotFound();
            }
            return View(clientes);
        }

        // POST: Clientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Clientes clientes = db.Clientes.Find(id);
            db.Clientes.Remove(clientes);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

  
    ////////////////////////////////////////////Entrega//////////////////////////////////////////////////////////////////////////
    public ActionResult VerEntrega(int id)
    {
        System.Web.HttpContext.Current.Session["IDCliente"] = id;
        EntregaContext db = new EntregaContext();

        var lista = from e in db.Entregas.Include(e => e.Clientes).Include(e => e.Estados)
                    where e.IDCliente == id
                    orderby e.IDEntrega
                    select e;
        return View(lista);

    }

        public ActionResult DetailsE(int id)
        {
            EntregaContext db = new EntregaContext();
            var elemento = db.Entregas.Include(m => m.Estados).Single(m => m.IDEntrega == id);
            if (elemento == null)
            {
                return HttpNotFound();
            }

            return View(elemento);
        }


        // POST: Proveedor/Details/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DetailsE(int id, Entrega collection)
        {
            EntregaContext db = new EntregaContext();
            var elemento = db.Entregas.Include(m => m.Estados).Single(m => m.IDEntrega == id);
            return View(elemento);
        }

        // GET: Entregas/Create
        // GET: Entregas/Create
        public ActionResult CreateE()
        {

            //Paises
            var datosPaises = new PaisesContext().Paises.OrderBy(i => i.Pais).ToList();
            List<SelectListItem> liP = new List<SelectListItem>();
            liP.Add(new SelectListItem { Text = "Selecciona un Pais", Value = "0" });
            foreach (var a in datosPaises)
            {
                liP.Add(new SelectListItem { Text = a.Pais, Value = a.IDPais.ToString() });

            }
            ViewBag.ListPais2 = liP;
            ViewBag.ListEstado2 = new PaisesRepository().GetEstadoPorPais(0);
            ViewBag.ListMunicipio2 = new PaisesRepository().GetMunicipioPorEstado(0);
            ViewBag.ListLocalidad2 = new PaisesRepository().GetLocalidadPorEstado(0);

            var Colonias = new c_ColoniaContext().Database.SqlQuery<c_Colonia>("select*from c_Colonia").ToList();
            List<SelectListItem> liAC = new List<SelectListItem>();
            liAC.Add(new SelectListItem { Text = "Selecciona una Colonia", Value = "0" });
            foreach (var a in Colonias)
            {

                liAC.Add(new SelectListItem { Text = a.NomAsentamiento, Value = a.IDColonia.ToString() });

            }
            ViewBag.IDColonia = liAC;
            var colonias = new SIAAPI.Models.CartaPorte.c_ColoniaContext().Database.SqlQuery<SIAAPI.Models.CartaPorte.c_Colonia>("select * from c_Colonia").ToList();
            ViewBag.Colonias = colonias;

            ViewBag.IDCliente = Session["IDCliente"];
            EntregaContext db = new EntregaContext();
            Entrega entrega = new Entrega();

            //Models.Administracion.EstadosContext dbe = new Models.Administracion.EstadosContext();
            //ViewBag.IDEstado = new SelectList(dbe.Estados, "IDEstado", "Estado");

            entrega.IDCliente = ViewBag.IDCliente;

            return View(entrega);
        }

        // POST: Entregas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateE(Entrega entrega)
        {
            EntregaContext db = new EntregaContext();

            try
            {
                Paises clavePais = new PaisesContext().Paises.Find(entrega.IDPais);
                Models.Administracion.Estados estados = new Models.Administracion.EstadosContext().Estados.Find(entrega.IDEstado);
                c_Municipio municipio = new c_MunicipioContext().municipio.Find(entrega.IDMunicipio);
                c_Localidad localidad = new c_LocalidadContext().Localidad.Find(entrega.IDLocalidad);
                c_Colonia colonia = new c_ColoniaContext().colonias.Find(entrega.IDColonia);

                Int32 iid = Int32.Parse(Session["IDCliente"].ToString());
                entrega.IDCliente = iid;
                entrega.c_Pais = clavePais.c_Pais;
                entrega.c_Estado = estados.c_Estado;
                entrega.c_Municipio = municipio.C_Municipio;
                entrega.MunicipioEntrega = municipio.Descripcion;
                entrega.c_Localidad = localidad.C_Localidad;
                entrega.c_Colonia = colonia.C_Colonia;
                entrega.ColoniaEntrega = colonia.NomAsentamiento;

                string cadena = "insert into[dbo].[Entrega]([IDCliente],[CalleEntrega],[ColoniaEntrega],[MunicipioEntrega],[IDEstado],[CPEntrega],[ObservacionEntrega],[NumExtEntrega],[NumIntEntrega],[DiaEntLu],[DiaEntMa],[DiaEntMi],[DiaEntJu],[DiaEntVi],[HorarioEnt],[IDPais],[c_Pais],[c_Estado],[IDMunicipio],[c_Municipio],[IDLocalidad],[c_Localidad],[IDColonia],[c_Colonia],[Referencia])";
                cadena = cadena + "values (" + entrega.IDCliente + ", '" + entrega.CalleEntrega + "', '" + entrega.ColoniaEntrega + "','" + entrega.MunicipioEntrega + "', " + entrega.IDEstado + ",'" + entrega.CPEntrega + "','" + entrega.ObservacionEntrega + "','" + entrega.NumExtEntrega + "', '" + entrega.NumIntentrega + "', '" + entrega.DiaEntLu + "','" + entrega.DiaEntMa + "','" + entrega.DiaEntMi + "','" + entrega.DiaEntJu + "','" + entrega.DiaEntVi + "','" + entrega.HorarioEnt + "'," + entrega.IDPais + ",'" + entrega.c_Pais + "','" + entrega.c_Estado + "'," + entrega.IDMunicipio + ",'" + entrega.c_Municipio + "'," + entrega.IDLocalidad + ",'" + entrega.c_Localidad + "'," + entrega.IDColonia + ",'" + entrega.c_Colonia + "','" + entrega.Referencia + "')";
                db.Database.ExecuteSqlCommand(cadena);

                try
                {
                    List<Entrega> numero;
                    numero = db.Database.SqlQuery<Entrega>("SELECT * FROM [dbo].[entrega] WHERE identrega = (SELECT MAX(idenetrega) from entrega)").ToList();
                   int num = numero.Select(s => s.IDEntrega).FirstOrDefault();

                    List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
                    int usuario = userid.Select(s => s.UserID).FirstOrDefault();
                   
                    string creacion = "insert into [dbo].[FechaCreacionDomEC]([Fecha],[IDUsuario],[IDEntrega], tipo)values (SYSDATETIME(),'" + usuario + "', '" + num + "','Creacion')";
                    db.Database.ExecuteSqlCommand(creacion);
                }
                catch (Exception ex)
                {

                }
                //db.Entregas.Add(entrega);
                //db.SaveChanges();
                return RedirectToAction("Details", new { id = iid });
            }
            catch (Exception err)
            {
                var datosPaises = new PaisesContext().Paises.OrderBy(i => i.Pais).ToList();
                List<SelectListItem> liP = new List<SelectListItem>();
                liP.Add(new SelectListItem { Text = "Selecciona un Pais", Value = "0" });
                foreach (var a in datosPaises)
                {
                    if (entrega.IDPais == a.IDPais)
                    {
                        liP.Add(new SelectListItem { Text = a.Pais, Value = a.IDPais.ToString(), Selected=true});
                    }
                    else
                    {
                        liP.Add(new SelectListItem { Text = a.Pais, Value = a.IDPais.ToString() });
                    }
                    

                }
                ViewBag.ListPais2 = liP;
                ViewBag.ListEstado2 = new PaisesRepository().GetEstadoPorPais(entrega.IDPais);
                ViewBag.ListMunicipio2 = new PaisesRepository().GetMunicipioPorEstado(entrega.IDEstado);
                ViewBag.ListLocalidad2 = new PaisesRepository().GetLocalidadPorEstado(entrega.IDEstado);

                var Colonias = new c_ColoniaContext().Database.SqlQuery<c_Colonia>("select*from c_Colonia").ToList();
                List<SelectListItem> liAC = new List<SelectListItem>();
                liAC.Add(new SelectListItem { Text = "Selecciona una Colonia", Value = "0" });
                foreach (var a in Colonias)
                {
                    if (entrega.IDColonia==a.IDColonia)
                    {
                        liAC.Add(new SelectListItem { Text = a.NomAsentamiento, Value = a.IDColonia.ToString(),Selected=true });
                    }
                    else
                    {
                        liAC.Add(new SelectListItem { Text = a.NomAsentamiento, Value = a.IDColonia.ToString() });
                    }

                }
                ViewBag.IDColonia = liAC;
                var colonias = new SIAAPI.Models.CartaPorte.c_ColoniaContext().Database.SqlQuery<SIAAPI.Models.CartaPorte.c_Colonia>("select * from c_Colonia").ToList();
                ViewBag.Colonias = colonias;

                ViewBag.IDCliente = Session["IDCliente"];
              

                //Models.Administracion.EstadosContext dbe = new Models.Administracion.EstadosContext();
                //ViewBag.IDEstado = new SelectList(dbe.Estados, "IDEstado", "Estado");

                entrega.IDCliente = ViewBag.IDCliente;
                return View();
            }

        }


        // GET: Entregas/Edit/5
        public ActionResult EditE(int id)
        {
            //EntregaContext db = new EntregaContext();

            //Entrega entrega = db.Entregas.Find(id);
            //if (entrega == null)
            //{
            //    return HttpNotFound();
            //}
            //ViewBag.IDEstado = new SelectList(db.Estados, "IDEstado", "Estado", entrega.IDEstado);
            //var listas = new ElementosRepository().GetStatus();
            //ViewBag.ComboStatus = listas;
            Entrega elementonuevo = db.Entregas.Find(id);

            var datosPaises = new PaisesContext().Paises.OrderBy(i => i.Pais).ToList();
            List<SelectListItem> liP = new List<SelectListItem>();
            liP.Add(new SelectListItem { Text = "Selecciona un Pais", Value = "0" });
            foreach (var a in datosPaises)
            {
                if (elementonuevo.IDPais == a.IDPais)
                {
                    liP.Add(new SelectListItem { Text = a.Pais, Value = a.IDPais.ToString(), Selected = true });
                }
                else
                {
                    liP.Add(new SelectListItem { Text = a.Pais, Value = a.IDPais.ToString() });
                }


            }
            ViewBag.ListPais = liP;
            ViewBag.IDEstado = getEstadoPorPaisSelec(elementonuevo.IDPais, elementonuevo.IDEstado);
            ViewBag.IDMunicipio = getMunicipioPorEstadoSelec(elementonuevo.IDEstado, elementonuevo.IDMunicipio);
            ViewBag.IDLocalidad = getLocalidadPorEstadoSelec(elementonuevo.IDEstado, elementonuevo.IDLocalidad);


            ViewBag.IDColonia = getColoniaPorCPSelec(elementonuevo.CPEntrega, elementonuevo.IDColonia);
            return View(elementonuevo);
        }

        // POST: Clientes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditE(Entrega entrega)
        {
            //EntregaContext db = new EntregaContext();
            //if (ModelState.IsValid)
            //{

            //    Int32 iid = Int32.Parse(Session["IDCliente"].ToString());
            //    entrega.IDCliente = iid;
            //    db.Entry(entrega).State = System.Data.Entity.EntityState.Modified;
            //    db.SaveChanges();
            //    return RedirectToAction("Details", new { id = iid });

            //}

            //ViewBag.IDEstado = new SelectList(db.Estados, "IDEstado", "Estado", entrega.IDEstado);
            //var listas = new ElementosRepository().GetStatus();
            //ViewBag.ComboStatus = listas;
            //return View(entrega);

            Entrega elemento = db.Entregas.Find(entrega.IDEntrega);
            try
            {
                Paises paises = new PaisesContext().Paises.Find(entrega.IDPais);
                Estados estados = new EstadosContext().Estados.Find(entrega.IDEstado);
                c_Municipio c_Municipio = new c_MunicipioContext().municipio.Find(entrega.IDMunicipio);
                c_Localidad c_Localidad = new c_LocalidadContext().Localidad.Find(entrega.IDLocalidad);
                c_Colonia c_Colonia = new c_ColoniaContext().colonias.Find(entrega.IDColonia);
                entrega.c_Pais = paises.c_Pais;
                entrega.c_Estado = estados.c_Estado;
                entrega.c_Municipio = c_Municipio.C_Municipio;
                entrega.c_Localidad = c_Localidad.C_Localidad;
                entrega.c_Colonia = c_Colonia.C_Colonia;


                //if (TryUpdateModel(elemento))
                //{

                //    db.SaveChanges();
                string insert = "UPDATE [dbo].[Entrega] SET [IDCliente] ='" + entrega.IDCliente + "' ,[CalleEntrega] ='" + entrega.CalleEntrega + "' ,[ColoniaEntrega] ='" + entrega.ColoniaEntrega + "' ," +
                    "[MunicipioEntrega] = '" + c_Municipio.Descripcion + "',[IDEstado] ='" + entrega.IDEstado + "' ,[CPEntrega] = '" + entrega.CPEntrega + "',[ObservacionEntrega] ='" + entrega.ObservacionEntrega + "',[NumExtEntrega] ='" + entrega.NumExtEntrega + "',[NumIntEntrega] ='" + entrega.NumIntentrega + "' ," +
                    "[DiaEntLu] = '" + entrega.DiaEntLu + "',[DiaEntMa] = '" + entrega.DiaEntMa + "',[DiaEntMi] = '" + entrega.DiaEntMi + "',[DiaEntJu] ='" + entrega.DiaEntJu + "' ,[DiaEntVi] = '" + entrega.DiaEntVi + "',[HorarioEnt] ='" + entrega.HorarioEnt + "' ,[IDPais] = '" + entrega.IDPais + "' ," +
                    "[c_Pais] = '" + entrega.c_Pais + "',[c_Estado] = '" + entrega.c_Estado + "',[IDMunicipio] ='" + entrega.IDMunicipio + "' ,[c_Municipio] ='" + entrega.c_Municipio + "' ,[IDLocalidad] = '" + entrega.IDLocalidad + "',[c_Localidad] ='" + entrega.c_Localidad + "' ,[IDColonia] = '" + entrega.IDColonia + "',[c_Colonia] ='" + entrega.c_Colonia + "' ,[Referencia] = '" + entrega.Referencia + "'  WHERE" +
                    " IDEntrega=" + elemento.IDEntrega;
                db.Database.ExecuteSqlCommand(insert);
                try
                {
                    List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
                    int usuario = userid.Select(s => s.UserID).FirstOrDefault();
                   
                    string creacion = "insert into [dbo].[FechaCreacionDomEC]([Fecha],[IDUsuario],[IDEntrega], tipo)values (SYSDATETIME(),'" + usuario + "', '" + entrega.IDEntrega + "','Modificacion')";
                    db.Database.ExecuteSqlCommand(creacion);
                }
                catch (Exception err)
                {

                }
                return RedirectToAction("Details", new { id = entrega.IDCliente });
                //}
                //return View(elemento);
            }
            catch
            {

                Entrega elementonuevo = db.Entregas.Find(entrega.IDEntrega);

                var datosPaises = new PaisesContext().Paises.OrderBy(i => i.Pais).ToList();
                List<SelectListItem> liP = new List<SelectListItem>();
                liP.Add(new SelectListItem { Text = "Selecciona un Pais", Value = "0" });
                foreach (var a in datosPaises)
                {
                    if (elementonuevo.IDPais == a.IDPais)
                    {
                        liP.Add(new SelectListItem { Text = a.Pais, Value = a.IDPais.ToString(), Selected = true });
                    }
                    else
                    {
                        liP.Add(new SelectListItem { Text = a.Pais, Value = a.IDPais.ToString() });
                    }


                }
                ViewBag.ListPais = liP;
                ViewBag.IDEstado = getEstadoPorPaisSelec(elementonuevo.IDPais, elementonuevo.IDEstado);
                ViewBag.IDMunicipio = getMunicipioPorEstadoSelec(elementonuevo.IDEstado, elementonuevo.IDMunicipio);
                ViewBag.IDLocalidad = getLocalidadPorEstadoSelec(elementonuevo.IDEstado, elementonuevo.IDLocalidad);


                ViewBag.IDColonia = getColoniaPorCPSelec(elementonuevo.CPEntrega, elementonuevo.IDColonia);
                return View(elemento);
            }

        }





        // GET: Entregas/Delete/5
        public ActionResult DeleteE(int id)
        {
            EntregaContext db = new EntregaContext();
            //if (id == null)
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //}
            Entrega entrega = db.Entregas.Find(id);
            if (entrega == null)
            {
                return HttpNotFound();
            }
            return View(entrega);
        }

        // POST: Entregas/Delete/5
        [HttpPost, ActionName("DeleteE")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmedE(int id)
        {
            EntregaContext db = new EntregaContext();
            Entrega entrega = db.Entregas.Find(id);
            Int32 iid = Int32.Parse(Session["IDCliente"].ToString());
            entrega.IDCliente = iid;

            db.Entregas.Remove(entrega);
            db.SaveChanges();
            return RedirectToAction("Details", new { id = iid });
        }




        ////////////////////////////////////////Cobro//////////////////////////////////////////////////////////////////////


        // GET: Cobro
        public ActionResult VerCobro(int id)
    {
        System.Web.HttpContext.Current.Session["IDCliente"] = id;
        CobroContext db = new CobroContext();
        var cobros = db.Cobros.Include(c => c.Estados);
        return View(cobros.ToList());
    }

    // GET: Entregas/Details/5
    public ActionResult DetailsC(int id)
    {
        CobroContext db = new CobroContext();
        var elemento = db.Cobros.Include(m => m.Estados).Single(m => m.IDCobro == id);
        if (elemento == null)
        {
            return HttpNotFound();
        }

        return View(elemento);
    }


    // POST: Proveedor/Details/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult DetailsC(int id, Cobro collection)
    {
        CobroContext db = new CobroContext();
        var elemento = db.Cobros.Include(m => m.Estados).Single(m => m.IDCobro == id);
        return View(elemento);
    }


    // GET: Entregas/Create
    public ActionResult CreateC()
    {
        CobroContext db = new CobroContext();
        ViewBag.IDEstado = new SelectList(db.Estados, "IDEstado", "Estado");

        return View();
    }

    // POST: Entregas/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult CreateC(Cobro cobro)
    {
        CobroContext db = new CobroContext();

        try
        {

            Int32 iid = Int32.Parse(Session["IDCliente"].ToString());
            cobro.IDCliente = iid;
            db.Cobros.Add(cobro);
            db.SaveChanges();
            return RedirectToAction("Details", new { id = iid });
        }
        catch (Exception err)
        {
                string mensaje = err.Message;
                ViewBag.IDEstado = new SelectList(db.Estados, "IDEstado", "Estado", cobro.IDEstado);

            return View();
        }

    }

    // GET: Entregas/Edit/5
    public ActionResult EditC(int id)
    {
        CobroContext db = new CobroContext();
        //if (id == null)
        //{
        //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //}
        Cobro cobro = db.Cobros.Find(id);
        if (cobro == null)
        {
            return HttpNotFound();
        }
        ViewBag.IDEstado = new SelectList(db.Estados, "IDEstado", "Estado", cobro.IDEstado);

        return View(cobro);
    }

    // POST: Clientes/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult EditC(Cobro cobro)
    {
        CobroContext db = new CobroContext();

        if (ModelState.IsValid)
        {

            Int32 iid = Int32.Parse(Session["IDCliente"].ToString());
            cobro.IDCliente = iid;

            db.Entry(cobro).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Details", new { id = iid });

        }

        ViewBag.IDEstado = new SelectList(db.Estados, "IDEstado", "Estado", cobro.IDEstado);

        return View(cobro);
    }




    // GET: Entregas/Delete/5
    public ActionResult DeleteC(int id)
    {
        CobroContext db = new CobroContext();
        //if (id == null)
        //{
        //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //}
        Cobro cobro = db.Cobros.Find(id);
        if (cobro == null)
        {
            return HttpNotFound();
        }
        return View(cobro);
    }

    // POST: Entregas/Delete/5
    [HttpPost, ActionName("DeleteC")]
    [ValidateAntiForgeryToken]
    public ActionResult DeleteConfirmedC(int id)
    {
        CobroContext db = new CobroContext();
        Cobro cobro = db.Cobros.Find(id);
        Int32 iid = Int32.Parse(Session["IDCliente"].ToString());
        cobro.IDCliente = iid;

        db.Cobros.Remove(cobro);
        db.SaveChanges();
        return RedirectToAction("Details", new { id = iid });
    }


    ///////////////////////////////////Contactos Clientes//////////////////////////////////////////////////////////////////
    public ActionResult VerContactos(int id)
    {
        System.Web.HttpContext.Current.Session["IDCliente"] = id;

        ContactosClieContext db = new ContactosClieContext();
        var lista = from e in db.ContactosClies
                    where e.IDCliente == id
                    orderby e.IDContactoClie
                    select e;
        //return View(elemento);

        return View(lista);
    }

    public ActionResult CreateContacto()
    {

        return View();
    }

    [HttpPost]
    public ActionResult CreateContacto(ContactosClie elemento)
    {
        try
        {
            ContactosClieContext db = new ContactosClieContext();
            Int32 idproveedor = Int32.Parse(Session["IDCliente"].ToString());
            elemento.IDCliente = idproveedor;
            db.ContactosClies.Add(elemento);
            db.SaveChanges();
            return RedirectToAction("Details", new { id = elemento.IDCliente });
        }
        catch (Exception err)
        {
                string mensaje = err.Message;
                return View();
        }
    }

    public ActionResult EditContacto(int id)
    {
        ContactosClieContext db = new ContactosClieContext();

        var elemento = db.ContactosClies.Single(m => m.IDContactoClie == id);

        return View(elemento);


    }

    // POST: ModeloProceso/Edit/5
    [HttpPost]
    public ActionResult EditContacto(ContactosClie Elemento)
    {
        try
        {
            Int32 id = Int32.Parse(Session["IDCliente"].ToString());
            ContactosClieContext db = new ContactosClieContext();


            var elemento = db.ContactosClies.Single(m => m.IDContactoClie == Elemento.IDContactoClie);
            if (TryUpdateModel(elemento))
            {
                db.SaveChanges();
                return RedirectToAction("Details", new { id = id });
            }
            return View(Elemento);
        }
        catch
        {
            return View();
        }
    }

    public ActionResult DeleteContacto(int id)
    {
        ContactosClieContext db = new ContactosClieContext();

        var elemento = db.ContactosClies.Single(m => m.IDContactoClie == id);

        return View(elemento);


    }

    [HttpPost]
    public ActionResult DeleteContacto(int id, ContactosClie collection)
    {
        try
        {
            ContactosClieContext db = new ContactosClieContext();
            var elemento = db.ContactosClies.Single(m => m.IDContactoClie == id);
            db.ContactosClies.Remove(elemento);
            db.SaveChanges();
            return RedirectToAction("Details", new { id = elemento.IDCliente });

        }
        catch
        {
            return View();
        }
    }

    // GET: Proveedor/Details/5
    public ActionResult DetailsContacto(int id)
    {
        ContactosClieContext db = new ContactosClieContext();
        var elemento = db.ContactosClies.Single(m => m.IDContactoClie == id);
        if (elemento == null)
        {
            return HttpNotFound();
        }

        return View(elemento);
    }


    // POST: Proveedor/Details/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult DetailsContacto(int id, ContactosClie collection)
    {

        ContactosClieContext db = new ContactosClieContext();
        var elemento = db.ContactosClies.Single(m => m.IDContactoClie == id);
        return View(elemento);
    }

        ///////////////////////////////////////////////////////////////////////Referencia Pago Cliente//////////////////////////////////////////////////////////////////////////////////////////

        public ActionResult VerReferencia(int id)
        {
            ReferenciaPagoClienteContext db = new ReferenciaPagoClienteContext();
            System.Web.HttpContext.Current.Session["IDCliente"] = id;


            var lista = from e in db.ReferenciaPagoClientes
                        where e.IDCliente == id
                        orderby e.IDCliente
                        select e;
            List<VBancoCliente> orden = db.Database.SqlQuery<VBancoCliente>("select BancoCliente.IDBancoCliente,BancoCliente.CuentaBanco, Clientes.Nombre as Cliente, c_Banco.Nombre as Banco, c_Moneda.Descripcion as Moneda from BancoCliente inner join Clientes on Clientes.IDCliente=BancoCliente.IDCliente inner join c_Banco on c_Banco.IDBanco=BancoCliente.IDBanco inner join c_Moneda on c_Moneda.IDMoneda=BancoCliente.IDMoneda where  BancoCliente.IDCliente='" + id + "'").ToList();
            ViewBag.data = orden;

            ClsDatoEntero c = db.Database.SqlQuery<ClsDatoEntero>("select count(IDCliente) as Dato from ReferenciaPagoCliente where IDCliente='" + id + "'").ToList()[0];
            int x = c.Dato;
            ViewBag.dato = x;
            return View(lista);
        }
        // POST: Proveedor/Details/5


        public ActionResult DetailsR(int id)
        {
            ReferenciaPagoClienteContext db = new ReferenciaPagoClienteContext();

            ReferenciaPagoCliente referenciaPagoCliente = db.ReferenciaPagoClientes.Find(id);

            return View(referenciaPagoCliente);
        }
        

        // GET: Entregas/Create
        public ActionResult CreateR()
        {
            return View();
        }

        // POST: Entregas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateR(ReferenciaPagoCliente referenciaPagoCliente)
        {
            ReferenciaPagoClienteContext db = new ReferenciaPagoClienteContext();
             try
            {
                Int32 iid = Int32.Parse(Session["IDCliente"].ToString());
                referenciaPagoCliente.IDCliente = iid;
                db.ReferenciaPagoClientes.Add(referenciaPagoCliente);
                db.SaveChanges();
                return RedirectToAction("Details", new { id = iid });
            }
            catch (Exception err)
            {
                string mensaje = err.Message;
                return View();
            }

        }

        // GET: Entregas/Edit/5
        public ActionResult EditR(int id)
        {
            ReferenciaPagoClienteContext db = new ReferenciaPagoClienteContext();
            //if (id == null)
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //}
            ReferenciaPagoCliente referenciaPagoCliente = db.ReferenciaPagoClientes.Find(id);
            

            return View(referenciaPagoCliente);
        }

        // POST: Clientes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditR(ReferenciaPagoCliente referenciaPagoCliente)
        {
            ReferenciaPagoClienteContext db = new ReferenciaPagoClienteContext();

            if (ModelState.IsValid)
            {

                Int32 iid = Int32.Parse(Session["IDCliente"].ToString());
                referenciaPagoCliente.IDCliente = iid;

                db.Entry(referenciaPagoCliente).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Details", new { id = iid });

            }
            

            return View(referenciaPagoCliente);
        }




        // GET: Entregas/Delete/5
        public ActionResult DeleteR(int id)
        {
            ReferenciaPagoClienteContext db = new ReferenciaPagoClienteContext();

            ReferenciaPagoCliente referenciaPagoCliente = db.ReferenciaPagoClientes.Find(id);
           
            return View(referenciaPagoCliente);
        }

        // POST: Entregas/Delete/5
        [HttpPost, ActionName("DeleteR")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmedR(int id)
        {
            ReferenciaPagoClienteContext db = new ReferenciaPagoClienteContext();
            ReferenciaPagoCliente referenciaPagoCliente = db.ReferenciaPagoClientes.Find(id);
            Int32 iid = Int32.Parse(System.Web.HttpContext.Current.Session["IDCliente"].ToString());
            referenciaPagoCliente.IDCliente = iid;

            db.ReferenciaPagoClientes.Remove(referenciaPagoCliente);
            db.SaveChanges();
            return RedirectToAction("Details", new { id = iid });
        }


        ///////////////////////////////////////////////////////////////////////////////Bancos/////////////////////////////////////////////////////////////////////////////////////////////////

        public ActionResult DetailsB(int id)
        {
            BancoClienteContext db = new BancoClienteContext();

            BancoCliente bancoCliente = db.BancoClientes.Find(id);

            return View(bancoCliente);
        }
        public ActionResult CreateB()
        {
            BancoClienteContext db = new BancoClienteContext();
            ViewBag.IDBanco = new SelectList(db.c_Bancos, "IDBanco", "Nombre");
            ViewBag.IDMoneda = new SelectList(db.c_Monedas, "IDMoneda", "Descripcion");

            return View();
        }

        // POST: Entregas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateB(BancoCliente bancoCliente)
        {
            BancoClienteContext db = new BancoClienteContext();
            try
            {
                Int32 iid = Int32.Parse(Session["IDCliente"].ToString());
                bancoCliente.IDCliente = iid;
                db.BancoClientes.Add(bancoCliente);
                db.SaveChanges();
                return RedirectToAction("Details", new { id = iid });
            }
            catch (Exception err)
            {

                ViewBag.IDBanco = new SelectList(db.c_Bancos, "IDBanco", "Nombre", bancoCliente.IDBanco);
                ViewBag.IDMoneda = new SelectList(db.c_Monedas, "IDMoneda", "Descripcion", bancoCliente.IDMoneda);
                string mensaje = err.Message;
                return View();
            }

        }
        public ActionResult EditB(int id)
        {
            BancoClienteContext db = new BancoClienteContext();
            BancoCliente bancoCliente = db.BancoClientes.Find(id);
            ViewBag.IDBanco = new SelectList(db.c_Bancos, "IDBanco", "Nombre", bancoCliente.IDBanco);
            ViewBag.IDMoneda = new SelectList(db.c_Monedas, "IDMoneda", "Descripcion", bancoCliente.IDMoneda);

            return View(bancoCliente);
        }

        // POST: Clientes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditB(BancoCliente bancoCliente)
        {
            BancoClienteContext db = new BancoClienteContext();

            if (ModelState.IsValid)
            {

                Int32 iid = Int32.Parse(Session["IDCliente"].ToString());
                bancoCliente.IDCliente = iid;

                db.Entry(bancoCliente).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Details", new { id = iid });

            }

            ViewBag.IDBanco = new SelectList(db.c_Bancos, "IDBanco", "Nombre", bancoCliente.IDBanco);
            ViewBag.IDMoneda = new SelectList(db.c_Monedas, "IDMoneda", "Descripcion", bancoCliente.IDMoneda);
            return View(bancoCliente);
        }




        // GET: Entregas/Delete/5
        public ActionResult DeleteB(int id)
        {
            BancoClienteContext db = new BancoClienteContext();

            BancoCliente bancoCliente = db.BancoClientes.Find(id);
           
            return View(bancoCliente);
        }

        // POST: Entregas/Delete/5
        [HttpPost, ActionName("DeleteB")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmedB(int id)
        {
            BancoClienteContext db = new BancoClienteContext();
            BancoCliente bancoCliente = db.BancoClientes.Find(id);
            Int32 iid = Int32.Parse(Session["IDCliente"].ToString());
            bancoCliente.IDCliente = iid;

            db.BancoClientes.Remove(bancoCliente);
            db.SaveChanges();
            return RedirectToAction("Details", new { id = iid });
        }

      
        public ActionResult ReportePorClientes()
        {

            List<Clientes> cl = new List<Clientes>();
            string cadenaf = "SELECT * FROM Clientes";
            cl = db.Database.SqlQuery<Clientes>(cadenaf).ToList();
            List<SelectListItem> listacll = new List<SelectListItem>();
            listacll.Add(new SelectListItem { Text = "--Todos los Clientes--", Value = "0" });

            foreach (var m in cl)
            {
                listacll.Add(new SelectListItem { Text = m.Nombre, Value = m.IDCliente.ToString() });
            }
            ViewBag.clientes = listacll;

            return View();
        }

        [HttpPost]
        public ActionResult ReportePorClientes(ReporteClientes modelo, Clientes A)
        {
            int idcliente = A.IDCliente;
            try
            {
                ClientesContext dbc = new ClientesContext();
                Clientes cls = dbc.Clientes.Find(A.IDCliente);
            }
            catch (Exception ERR)
            {

            }
            ReporteClientes report = new ReporteClientes();
            byte[] abytes = report.PrepareReport(idcliente);
            return File(abytes, "application/pdf");
        }

        //
        public void ExcelClientes()
        {
            //Listado de datos

            List<Clientes> clientes = new List<Clientes>();
            string cadena = "select * from Clientes order by IDCliente";
            clientes = db.Database.SqlQuery<Clientes>(cadena).ToList();

            //var bancos = dba.c_Bancos.ToList();
            ExcelPackage Ep = new ExcelPackage();
            var Sheet = Ep.Workbook.Worksheets.Add("Clientes");

            int row = 1;
            //Fijar la fuente para A1:Q1
            Sheet.Cells["A1:M1"].Style.Font.Size = 20;
            Sheet.Cells["A1:M1"].Style.Font.Name = "Calibri";
            Sheet.Cells["A1:M1"].Style.Font.Bold = true;
            Sheet.Cells["A1:M1"].Style.Font.Color.SetColor(Color.DarkBlue);
            Sheet.Cells["A1:M1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
            Sheet.Cells["A1"].RichText.Add("Listado general de Clientes");
            Sheet.Cells["A1:M1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;

            row = 2;
            Sheet.Cells["A2:M2"].Style.Font.Name = "Calibri";
            Sheet.Cells["A2:M2"].Style.Font.Size = 12;
            Sheet.Cells["A2:M2"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;
            Sheet.Cells["A2:M2"].Style.Font.Bold = true;

            Sheet.Cells["A2"].RichText.Add("NOMBRE CLIENTE");
            Sheet.Cells["B2"].RichText.Add("ESTATUS");
            Sheet.Cells["C2"].RichText.Add("CORREO");
            Sheet.Cells["D2"].RichText.Add("TELÉFONO");
            Sheet.Cells["E2"].RichText.Add("CALLE");
            Sheet.Cells["F2"].RichText.Add("COLONIA");
            Sheet.Cells["G2"].RichText.Add("MUNICIPIO");
            Sheet.Cells["H2"].RichText.Add("ESTADO");
            Sheet.Cells["I2"].RichText.Add("OBSERVACIÓN");
            Sheet.Cells["J2"].RichText.Add("CURP");
            Sheet.Cells["K2"].RichText.Add("RFC");
            Sheet.Cells["L2"].RichText.Add("CORREO CFDI");
            Sheet.Cells["M2"].RichText.Add("CUENTA CONTABLE");
            Sheet.Cells["N2"].RichText.Add("GRUPO");
            Sheet.Cells["O2"].RichText.Add("VENDEDOR");


            row = 3;
            foreach (var item in clientes)
            {
                string nomestado = new Models.Administracion.EstadosContext().Estados.Find(item.IDEstado).Estado;
                string grupo = new Models.Administracion.c_GrupoContext().c_Grupos.Find(item.IDGrupo).Descripcion;
                string Vendedor = new Models.Comercial.VendedorContext().Vendedores.Find(item.IDVendedor).Nombre;

                Sheet.Cells[string.Format("A{0}", row)].Value = item.Nombre;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.Status;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.Correo;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.Telefono;
                Sheet.Cells[string.Format("E{0}", row)].Value = item.Calle;
                Sheet.Cells[string.Format("F{0}", row)].Value = item.Colonia;
                Sheet.Cells[string.Format("G{0}", row)].Value = item.Municipio;
                Sheet.Cells[string.Format("H{0}", row)].Value = nomestado;
                Sheet.Cells[string.Format("I{0}", row)].Value = item.Observacion;
                Sheet.Cells[string.Format("J{0}", row)].Value = item.Curp;
                Sheet.Cells[string.Format("K{0}", row)].Value = item.RFC;
                Sheet.Cells[string.Format("L{0}", row)].Value = item.CorreoCfdi;
                Sheet.Cells[string.Format("M{0}", row)].Value = item.cuentaContable;
                Sheet.Cells[string.Format("N{0}", row)].Value = grupo;
                Sheet.Cells[string.Format("O{0}", row)].Value = Vendedor;
                row++;
            }
            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "ExcelClientes.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();

        }
        public ActionResult ArticulosCliente()
        {
            EFechaval elemento = new EFechaval();
            List<Clientes> cl = new List<Clientes>();
            string cadenaf = "SELECT * FROM Clientes";
            cl = db.Database.SqlQuery<Clientes>(cadenaf).ToList();
            List<SelectListItem> listacll = new List<SelectListItem>();
            listacll.Add(new SelectListItem { Text = "--Todos los Clientes--", Value = "0" });

            foreach (var m in cl)
            {
                listacll.Add(new SelectListItem { Text = m.Nombre, Value = m.IDCliente.ToString() });
            }
            ViewBag.IDProveedor = listacll;

            return View(elemento);
        }

        [HttpPost]
        public ActionResult ArticulosCliente(EFechaval modelo, FormCollection coleccion)
        {
            VClienteArticuloCompraContext dbCom = new VClienteArticuloCompraContext();
            string FI = modelo.fechaini.Year.ToString() + "-" + modelo.fechaini.Month.ToString() + "-" + modelo.fechaini.Day.ToString();
            string FF = modelo.fechafin.Year.ToString() + "-" + modelo.fechafin.Month.ToString() + "-" + modelo.fechafin.Day.ToString();
            int IDCliente = int.Parse(modelo.IDProveedor.ToString());

            string cual = coleccion.Get("Enviar");
            if (cual == "Generar reporte")
            {

            }
            if (cual == "Generar excel")
            {
                string cadenaCli = "";
                if (IDCliente == 0)
                {
                    cadenaCli = "select * from VClienteArticuloCompra where (Fecha >= '" + FI + " 00:00:00.1' and Fecha <='" + FF + " 23:59:59.999') order by cref, Fecha ";

                }
                else
                {
                    cadenaCli = "select * from VClienteArticuloCompra where IDCliente = " + IDCliente + " and (Fecha >= '" + FI + " 00:00:00.1' and Fecha <='" + FF + " 23:59:59.999') order by cref, Fecha ";
                }
                var articuloCli = dbCom.Database.SqlQuery<VClienteArticuloCompra>(cadenaCli).ToList();
                ViewBag.articuloCli = articuloCli;

                ExcelPackage Ep = new ExcelPackage();
                //Crear la hoja y poner el nombre de la pestaña del libro
                var Sheet = Ep.Workbook.Worksheets.Add("articulo Cliente");

                // en la fila1 formateo las celdas y coloco el título de la hoja
                // RichText permite agregar las propiedades de tipo de letra: color, negrita, etc.
                int row = 1;
                //Fijar la fuente para A1:Q1
                Sheet.Cells["A1:H1"].Style.Font.Size = 20;
                Sheet.Cells["A1:H1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:H3"].Style.Font.Bold = true;
                Sheet.Cells["A1:H1"].Style.Font.Color.SetColor(Color.DarkBlue);
                Sheet.Cells["A1:H1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                Sheet.Cells["A1"].RichText.Add("Listado de compras de Artículos del cliente");

                row = 2;
                Sheet.Cells["A1:H1"].Style.Font.Size = 12;
                Sheet.Cells["A1:H1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:H1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
                Sheet.Cells["A2:H2"].Style.Font.Bold = true;
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
                Sheet.Cells["A3:H3"].Style.Font.Bold = true;
                Sheet.Cells["A3:H3"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                Sheet.Cells["A3:H3"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                // Se establece el nombre que identifica a cada una de las columnas de datos.
                Sheet.Cells["A3"].RichText.Add("Serie");
                Sheet.Cells["B3"].RichText.Add("No. Factura");
                Sheet.Cells["C3"].RichText.Add("Fecha");
                Sheet.Cells["D3"].RichText.Add("Cliente");
                Sheet.Cells["E3"].RichText.Add("Cref");
                Sheet.Cells["F3"].RichText.Add("Artículo");
                Sheet.Cells["G3"].RichText.Add("Presentacion");
                Sheet.Cells["H3"].RichText.Add("Cantidad");


                //.Value solo trae datos, sin formato, a diferencia de .RichText que permite aplicar: tipos, tamaños, colores, negrita
                //Sheet.Cells["A3"].Value = "Serie";

                //Aplicar borde doble al rango de celdas A3:Q3
                Sheet.Cells["A3:R3"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;

                // en la fila4, el foreach llenara cada fila de la hoja de excel, con cada fila que trae de la bd.
                // Se establecen los formatos para las celdas: Fecha, Moneda
                row = 4;
                Sheet.Cells.Style.Font.Bold = false;
                foreach (VClienteArticuloCompra item in ViewBag.articuloCli)
                {
                    Sheet.Cells[string.Format("A{0}", row)].Value = item.SerieDigital;
                    Sheet.Cells[string.Format("B{0}", row)].Value = item.NumeroDigital;
                    Sheet.Cells[string.Format("C{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    Sheet.Cells[string.Format("C{0}", row)].Value = item.Fecha;
                    Sheet.Cells[string.Format("D{0}", row)].Value = item.Nombre_Cliente;
                    Sheet.Cells[string.Format("E{0}", row)].Value = item.Cref;
                    Sheet.Cells[string.Format("F{0}", row)].Value = item.Descripcion;
                    Sheet.Cells[string.Format("G{0}", row)].Value = item.Presentacion;
                    Sheet.Cells[string.Format("H{0}", row)].Style.Numberformat.Format = "#,##0.00";
                    Sheet.Cells[string.Format("H{0}", row)].Value = item.Cantidad;

                    row++;
                }


                //Se genera el archivo y se descarga

                Sheet.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment: filename=" + "RSuajes.xlsx");
                Response.BinaryWrite(Ep.GetAsByteArray());
                Response.End();
                return Redirect("/blah");
            }
            return Redirect("index");
        }


        public ActionResult getJsonMunicipioPorEstado(int id)
        {
            var estado = new PaisesRepository().GetMunicipioPorEstado(id);
            return Json(estado, JsonRequestBehavior.AllowGet); ;

        }

        public IEnumerable<SelectListItem> getMunicipioPorEstado(int idp)
        {
            var estado = new PaisesRepository().GetMunicipioPorEstado(idp);
            return estado;

        }

        public ActionResult getJsonLocalidadPorEstado(int id)
        {
            var estado = new PaisesRepository().GetLocalidadPorEstado(id);
            return Json(estado, JsonRequestBehavior.AllowGet); ;

        }

        public IEnumerable<SelectListItem> getLocalidadPorEstado(int idp)
        {
            var estado = new PaisesRepository().GetLocalidadPorEstado(idp);
            return estado;

        }
        public IEnumerable<SelectListItem> getEstadoPorPaisSelec(int idp, int ide)
        {
            var estado = new PaisesRepository().GetEstadoPorPaisSelec(idp, ide);
            return estado;

        }
        public IEnumerable<SelectListItem> getMunicipioPorEstadoSelec(int ide, int idm)
        {
            var estado = new PaisesRepository().GetMunicipioPorEstadoSelect(ide, idm);
            return estado;

        }
        public IEnumerable<SelectListItem> getLocalidadPorEstadoSelec(int ide, int idl)
        {
            var estado = new PaisesRepository().GetLocalidadPorEstadoSelec(ide, idl);
            return estado;

        }

        public IEnumerable<SelectListItem> getColoniaPorCPSelec(string CP, int idc)
        {
            var estado = new PaisesRepository().GetColoniaPorCPSelec(CP, idc);
            return estado;

        }
        public JsonResult getLocalidades(string buscar)
        {
            ////buscar = buscar.Remove(buscar.Length - 1);
            var Localidades = new c_LocalidadContext().Database.SqlQuery<c_Localidad>("  select l.* from c_localidad as l inner join estados as e on e.c_estado=l.c_estado where e.idestado='" + buscar + "'").ToList();

            // Populate DropDownList
            List<SelectListItem> opciones = new List<SelectListItem>();

            foreach (c_Localidad art in Localidades)
            {
                SelectListItem elemento = new SelectListItem();
                elemento.Text = art.Descripcion;
                elemento.Value = art.IDLocalidad.ToString();
                opciones.Add(elemento);
            }
            return Json(opciones, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getColonias(string buscar)
        {
            ////buscar = buscar.Remove(buscar.Length - 1);
            var Colonias = new c_ColoniaContext().Database.SqlQuery<c_Colonia>("select*from c_Colonia where C_CodigoPostal='" + buscar + "'").ToList();

            // Populate DropDownList
            List<SelectListItem> opciones = new List<SelectListItem>();

            foreach (c_Colonia art in Colonias)
            {
                SelectListItem elemento = new SelectListItem();
                elemento.Text = art.NomAsentamiento;
                elemento.Value = art.IDColonia.ToString();
                opciones.Add(elemento);
            }
            //var colonias = new SIAAPI.Models.CartaPorte.c_ColoniaContext().Database.SqlQuery<SIAAPI.Models.CartaPorte.c_Colonia>("select * from c_Colonia where C_CodigoPostal='" + buscar + "'").ToList();
            //ViewBag.Colonias = colonias;
            return Json(opciones, JsonRequestBehavior.AllowGet);
        }
    }
}
