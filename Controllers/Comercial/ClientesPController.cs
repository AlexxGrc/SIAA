using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SIAAPI.Models.Comercial;
using PagedList;

using System.IO;
using OfficeOpenXml;
using System.Drawing;
using OfficeOpenXml.Style;
using SIAAPI.Reportes;
using SIAAPI.Models.Administracion;
using System.Globalization;
using SIAAPI.Models.Login;

namespace SIAAPI.Controllers.Comercial
{
    public class ClientesPController : Controller
    {
        private ClientesPContext db = new ClientesPContext();

        // GET: ClientesP

        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NombreSortParm = String.IsNullOrEmpty(sortOrder) ? "Nombre" : "Nombre";
            ViewBag.VendedorSortParm = String.IsNullOrEmpty(sortOrder) ? "Vendedor" : "Vendedor";
           
            // Not sure here
            if (searchString == null)
            {
                searchString = currentFilter;
            }

            // Pass filtering string to view in order to maintain filtering when paging
            ViewBag.SearchString = searchString;
            //Paginación

            // var elementos = from s in db.ClientesPs
            //  select s;
            //Busqueda

            string Filtro = "Select TOP 200 * from ClientesP   ";

            if (!string.IsNullOrEmpty(searchString))
            {
                Filtro = Filtro + " where nombre like '%"+ searchString +"%' ";
            }

            //Ordenacion



            Filtro = Filtro + " ORDER BY nOMBRE";


          List<ClientesP> elementos = new ClientesPContext().Database.SqlQuery<ClientesP>(Filtro).ToList();
            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = elementos.Count; // Total number of elements

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

        // GET: ClientesP/Details/5
        public ActionResult Details(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClientesP clientesP = db.Database.SqlQuery<ClientesP>("select * from clientesP where idClientep=" +id).FirstOrDefault();
            ViewBag.Vendedor = new VendedorContext().Vendedores.Find(clientesP.IDClienteP);
            if (clientesP == null)
            {
                return HttpNotFound();
            }
            return View(clientesP);
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


        // GET: ClientesP/Create
        public ActionResult Create()
        {
            ViewBag.Error = "";
            ViewBag.IDVendedor = new SelectList(db.Vendedores.AsNoTracking().Where(n => n.Activo.Equals(true)), "IDVendedor", "Nombre");

            //Paises
          
            return View();
        }


        // POST: ClientesP/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ClientesP clientesP, FormCollection collection)
        {
           


            try
            {
                if (ModelState.IsValid)
                {
                    db.ClientesPs.Add(clientesP);
                  
                    List<User> userid;
                    userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
                    int UserID = userid.Select(s => s.UserID).FirstOrDefault();

                    string cadena = "insert into[dbo].[ClientesP]( Nombre ,Correo,Telefono,IDVendedor, userid ) ";
                    cadena = cadena + "VALUES('" + clientesP.Nombre + "', '" + clientesP.Correo + "', '" + clientesP.Telefono + "',"+clientesP.IDVendedor+","+ UserID+")";
                    db.Database.ExecuteSqlCommand(cadena);
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.IDVendedor = new SelectList(db.Vendedores.AsNoTracking().Where(n => n.Activo.Equals(true)), "IDVendedor", "Nombre", clientesP.IDVendedor);

                    //Paises
                   
                    return View(clientesP);
                }
            }
            catch(Exception)
            {
              
                ViewBag.IDVendedor = new SelectList(db.Vendedores.AsNoTracking().Where(n => n.Activo.Equals(true)), "IDVendedor", "Nombre", clientesP.IDVendedor);

              
                return View(clientesP);
            }

           
        }

        public ActionResult CreateClie(Clientes elemento, int id)
        {
            var datosClieP = db.Database.SqlQuery<ClientesP>("select * from ClientesP where IDClienteP = " + id + "").ToList();
            ViewBag.datosClieP = datosClieP;

            foreach (ClientesP cliep in datosClieP)
            {
               elemento.Nombre = cliep.Nombre.ToString();
               elemento.Correo = cliep.Correo;
               elemento.Telefono = cliep.Telefono;
             
               ViewBag.IDVendedor = cliep.IDVendedor;
             

            }
            //Paises
          

         
            return View(elemento);
        }


        // GET: ClientesP/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClientesP clientesP = db.Database.SqlQuery<ClientesP>("select * from clientesP where idClientep=" + id).FirstOrDefault();
          
            if (clientesP == null)
            {
                return HttpNotFound();
            }
            //Paises
            //Paises
         

            ViewBag.IDVendedor = new SelectList(db.Vendedores.AsNoTracking().Where(n => n.Activo.Equals(true)), "IDVendedor", "Nombre", clientesP.IDVendedor);
            return View(clientesP);
        }

        // POST: ClientesP/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ClientesP clientesP)
        {
           

            //if (ModelState.IsValid)
            //{
                //db.Entry(clientesP).State = System.Data.Entity.EntityState.Modified;
                //db.SaveChanges();
                string cadena = "update [dbo].[ClientesP] set Nombre = '" + clientesP.Nombre + "', correo= '" + clientesP.Correo + "',Telefono ='" + clientesP.Telefono + "'";
                cadena = cadena + ",IDVendedor= '" + clientesP.IDVendedor + "' where IDClienteP= "+ clientesP.IDClienteP +" "; 
                db.Database.ExecuteSqlCommand(cadena);

                return RedirectToAction("Index");
            //}
            //Paises
          
            ViewBag.IDVendedor = new SelectList(db.Vendedores.AsNoTracking().Where(n => n.Activo.Equals(true)), "IDVendedor", "Nombre", clientesP.IDVendedor);
            //ViewBag.IDVendedor = new SelectList(db.Vendedores, "IDVendedor", "Nombre", clientesP.IDVendedor);
            return View(clientesP);
        }

        // GET: ClientesP/Delete/5
        public ActionResult Delete(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClientesP clientesP = db.Database.SqlQuery<ClientesP>("select * from clientesP where idClientep=" + id).FirstOrDefault();

            if (clientesP == null)
            {
                return HttpNotFound();
            }
            return View(clientesP);
        }

        // POST: ClientesP/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ClientesP clientesP = db.ClientesPs.Find(id);
            db.ClientesPs.Remove(clientesP);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        ///////////////////////////////////Contactos Clientes//////////////////////////////////////////////////////////////////
        public ActionResult VerContactos(int id)
        {
            System.Web.HttpContext.Current.Session["IDClienteP"] = id;

            ContactosProsContext db = new ContactosProsContext();
            var lista = from e in db.ContactosPross
                        where e.IDClienteP == id
                        orderby e.IDProspecto
                        select e;

            return View(lista);
        }

        public ActionResult CreateContacto()
        {

            return View();
        }

        [HttpPost]
        public ActionResult CreateContacto(ContactosPros elemento)
        {
            try
           {
                ContactosProsContext db = new ContactosProsContext();
                Int32 idproveedor = Int32.Parse(System.Web.HttpContext.Current.Session["IDClienteP"].ToString());
                elemento.IDClienteP = idproveedor;
                db.ContactosPross.Add(elemento);
                db.SaveChanges();
                return RedirectToAction("VerContactos", new { id = elemento.IDClienteP });
            }
            catch (Exception err)
            {
                string mensajederror = err.Message;
                return View();
            }
        }

        public ActionResult EditContacto(int id)
        {
            ContactosProsContext db = new ContactosProsContext();

            var elemento = db.ContactosPross.Single(m => m.IDProspecto == id);

            return View(elemento);


        }

        // POST: ModeloProceso/Edit/5
        [HttpPost]
        public ActionResult EditContacto(ContactosPros Elemento)
        {
            try
            {
                Int32 idproveedor = Int32.Parse(System.Web.HttpContext.Current.Session["IDClienteP"].ToString());
                ContactosProsContext db = new ContactosProsContext();


                var elemento = db.ContactosPross.Single(m => m.IDProspecto == Elemento.IDProspecto);
                if (TryUpdateModel(elemento))
                {
                    db.SaveChanges();
                    return RedirectToAction("VerContactos", new { id = idproveedor });
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
            ContactosProsContext db = new ContactosProsContext();

            var elemento = db.ContactosPross.Single(m => m.IDProspecto == id);

            return View(elemento);


        }

        [HttpPost]
        public ActionResult DeleteContacto(int id, ContactosPros collection)
        {
            try
            {
                ContactosProsContext db = new ContactosProsContext();
                var elemento = db.ContactosPross.Single(m => m.IDProspecto == id);
                db.ContactosPross.Remove(elemento);
                db.SaveChanges();
                return RedirectToAction("VerContactos", new { id = elemento.IDClienteP });

            }
            catch
            {
                return View();
            }
        }

        // GET: Proveedor/Details/5
        public ActionResult DetailsContacto(int id)
        {
            ContactosProsContext db = new ContactosProsContext();
            var elemento = db.ContactosPross.Single(m => m.IDProspecto == id);
            if (elemento == null)
            {
                return HttpNotFound();
            }

            return View(elemento);
        }


        // POST: Proveedor/Details/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DetailsContacto(int id, ContactosPros collection)
        {

            ContactosProsContext db = new ContactosProsContext();
            var elemento = db.ContactosPross.Single(m => m.IDProspecto == id);
            return View(elemento);
        }
     
        public ActionResult ReportePorClientesP()
        {

            List<ClientesP> cl = new List<ClientesP>();
            string cadenaf = "SELECT * FROM ClientesP";
            cl = db.Database.SqlQuery<ClientesP>(cadenaf).ToList();
            List<SelectListItem> listacll = new List<SelectListItem>();
            listacll.Add(new SelectListItem { Text = "--Todos los Clientes Prospecto--", Value = "0" });

            foreach (var m in cl)
            {
                listacll.Add(new SelectListItem { Text = m.Nombre, Value = m.IDClienteP.ToString() });
            }
            ViewBag.clientesP = listacll;

            return View();
        }

        [HttpPost]
        public ActionResult ReportePorClientesP(ReporteClientesP modelo, ClientesP A)
        {
            int idclienteP = A.IDClienteP;
            try
            {
                ClientesPContext dbc = new ClientesPContext();
                ClientesP cls = dbc.ClientesPs.Find(A.IDClienteP);
            }
            catch (Exception ERR)
            {

            }
            ReporteClientesP report = new ReporteClientesP();
            byte[] abytes = report.PrepareReport(idclienteP);
            return File(abytes, "application/pdf");
        }

        //
        public void ExcelClientesP()
        {
            //Listado de datos

            List<ClientesP> clientes = new List<ClientesP>();
            string cadena = "select * from ClientesP";
            clientes = db.Database.SqlQuery<ClientesP>(cadena).ToList();

            ExcelPackage Ep = new ExcelPackage();
            var Sheet = Ep.Workbook.Worksheets.Add("ClientesP");

            int row = 1;
            //Fijar la fuente para A1:Q1
            Sheet.Cells["A1:K1"].Style.Font.Size = 20;
            Sheet.Cells["A1:K1"].Style.Font.Name = "Calibri";
            Sheet.Cells["A1:K1"].Style.Font.Bold = true;
            Sheet.Cells["A1:K1"].Style.Font.Color.SetColor(Color.DarkBlue);
            Sheet.Cells["A1:K1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
            Sheet.Cells["A1"].RichText.Add("Listado general de Clientes Prospecto");
            Sheet.Cells["A1:K1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;

            row = 2;
            Sheet.Cells["A2:K2"].Style.Font.Name = "Calibri";
            Sheet.Cells["A2:K2"].Style.Font.Size = 12;
            Sheet.Cells["A2:K2"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;
            Sheet.Cells["A2:K2"].Style.Font.Bold = true;

            Sheet.Cells["A2"].RichText.Add("NOMBRE CLIENTE");
            Sheet.Cells["B2"].RichText.Add("CORREO");
            Sheet.Cells["C2"].RichText.Add("TELÉFONO");

            Sheet.Cells["D2"].RichText.Add("Fecha de alta");
          
            Sheet.Cells["E2"].RichText.Add("Vendedor");

            row = 3;
            foreach (var item in clientes)
            {
                

                Sheet.Cells[string.Format("A{0}", row)].Value = item.Nombre;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.Correo;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.Telefono;
               
                Sheet.Cells[string.Format("D{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.fechaAlta;
              
               
                ClsDatoString vend = db.Database.SqlQuery<ClsDatoString>("select Nombre as Dato from Vendedor where IDVendedor = " + item.IDVendedor + "").FirstOrDefault();
                ViewBag.vendedor =  vend.Dato;
                Sheet.Cells[string.Format("E{0}", row)].Value = ViewBag.vendedor;
                row++;
            }
            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "ExcelClientesProspecto.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();

        }



    }
}
