using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using PagedList;
using SIAAPI.Models.CartaPorte;
using SIAAPI.Reportes;
using SIAAPI.Models.Comercial;
using SIAAPI.ViewModels.CartaPorte;

namespace SIAAPI.Controllers.CartaPorte
{
    public class ParqueVehicularController : Controller
    {
        private ParqueVehicularDBContext db = new ParqueVehicularDBContext();
        private ParqueVehicularDBContext dbv = new ParqueVehicularDBContext();

        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.PermisoSortParm = String.IsNullOrEmpty(sortOrder) ? "IDPermisoSCT" : "IDPermisoSCT";
            ViewBag.NombreSortParm = String.IsNullOrEmpty(sortOrder) ? "ClavePermisoSCT" : "ClavePermisoSCT";
            ViewBag.RazoneSortParm = String.IsNullOrEmpty(sortOrder) ? "Aseguradora" : "Aseguradora";
            ViewBag.RazoneSortParm = String.IsNullOrEmpty(sortOrder) ? "PolizaSeguro" : "PolizaSeguro";
            ViewBag.RazoneSortParm = String.IsNullOrEmpty(sortOrder) ? "IDVehiculo" : "IDVehiculo";
            ViewBag.RazoneSortParm = String.IsNullOrEmpty(sortOrder) ? "ClaveVehiculo" : "ClaveVehiculo";
            ViewBag.RazoneSortParm = String.IsNullOrEmpty(sortOrder) ? "PlacaVehiculo" : "PlacaVehiculo";
            ViewBag.RazoneSortParm = String.IsNullOrEmpty(sortOrder) ? "AnnoVehiculo" : "AnnoVechiculo";
            ViewBag.RazoneSortParm = String.IsNullOrEmpty(sortOrder) ? "Modelo" : "Modelo";
            ViewBag.RazoneSortParm = String.IsNullOrEmpty(sortOrder) ? "IDRemolque" : "IDRemolque";
            ViewBag.RazoneSortParm = String.IsNullOrEmpty(sortOrder) ? "ClaveRemolque" : "ClaveRemolque";
            ViewBag.RazoneSortParm = String.IsNullOrEmpty(sortOrder) ? "Placa" : "Placa";
            ViewBag.RazoneSortParm = String.IsNullOrEmpty(sortOrder) ? "EsArrendado" : "EsArrendado";
            ViewBag.RazoneSortParm = String.IsNullOrEmpty(sortOrder) ? "EsdeunPropietario" : "EsdeunPropietario";
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
            var elementos = from s in db.ParqueVe
                            select s;
            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                //elementos = elementos.Where(s => s.ClaveBanco.ToUpper().Contains(searchString.ToUpper()));

                elementos = elementos.Where(s => s.ClavePermisoSCT.Contains(searchString) || s.ClaveRemolque.Contains(searchString) || s.ClaveVehiculo.Contains(searchString));

            }

            //Ordenacion

            switch (sortOrder)
            {
                case "ClaveVehiculo":
                    elementos = elementos.OrderBy(s => s.ClaveVehiculo);
                    break;
                case "PlacaVehiculo":
                    elementos = elementos.OrderBy(s => s.PlacaVehiculo);
                    break;
                case "AnnoVehiculo":
                    elementos = elementos.OrderBy(s => s.AnnoVehiculo);
                    break;
                case "Modelo":
                    elementos = elementos.OrderBy(s => s.Modelo);
                    break;
                case "Color":
                    elementos = elementos.OrderBy(s => s.Color);
                    break;
                case "EsArrendado":
                    elementos = elementos.OrderBy(s => s.EsArrendado);
                    break;
                case "EsdeunPropietario":
                    elementos = elementos.OrderBy(s => s.TienePropietario);
                    break;
                default:
                    elementos = elementos.OrderByDescending(s => s.IDParqueV);
                    break;
            }

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = db.ParqueVe.OrderBy(e => e.IDParqueV).Count(); // Total number of elements

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

        
        // GET: ParqueVehicular/Details/5
        public ActionResult Details(int? id)
        {

            var elemento = dbv.Database.SqlQuery<VParqueVehicular>("select * from VParqueVehicular where IDParqueV = " + id).ToList().FirstOrDefault();
            ViewBag.data = elemento;

            VPropietarioContext dbp = new VPropietarioContext();
            if (elemento.IDPropietario != 0 )
            {
                List <VPropietario> lstpropietario = dbp.Database.SqlQuery<VPropietario>("select * from VPropietario where IDPropietario = " + elemento.IDPropietario).ToList();
                ViewBag.propietario = lstpropietario;

            }

            VArrendatarioContext dba = new VArrendatarioContext();
            if (elemento.IDArrendatario != 0)
            {
                List<VArrendatario> lstarrendatario = dba.Database.SqlQuery<VArrendatario>("select * from VArrendatario where IDArrendatario = " + elemento.IDArrendatario).ToList();
                ViewBag.arrendatario = lstarrendatario;
            }

            return View(elemento);
        }

        // GET: ParqueVehicular/Create
        public ActionResult Create()
        {
            ParqueVehicularDBContext p = new ParqueVehicularDBContext();

            var permiso = new TipoPermisoRepository().GetTipoPermiso();
            ViewBag.IDPermisoSCT = permiso;

            var vehiculo = new ConfigAutotransporteRepository().GetConfigAutotransporte();
            ViewBag.IDVehiculo = vehiculo;

            var remolque = new TipoRemRepository().GetTipoRem();
            ViewBag.IDRemolque = remolque;

            var EsArrendadoLst = new List<SelectListItem>();
            EsArrendadoLst.Add(new SelectListItem { Text = "No", Value = "false" });
            EsArrendadoLst.Add(new SelectListItem { Text = "Si", Value = "true" });
            ViewBag.EsArrendado = new SelectList(EsArrendadoLst, "Value", "Text");

            var TienePropietarioLst = new List<SelectListItem>();
            TienePropietarioLst.Add(new SelectListItem { Text = "No", Value = "false" });
            TienePropietarioLst.Add(new SelectListItem { Text = "Si", Value = "true" });
            ViewBag.TienePropietario = new SelectList(TienePropietarioLst, "Value", "Text");

            var arrendatario = new ArrendatarioRepository().GetArrendatario();
            ViewBag.IDArrendatario = arrendatario;

            var propietario = new PropietarioRepository().GetPropietario();
            ViewBag.IDPropietario = propietario;

            return View();
        }

        // POST: ParqueVehicular/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ParqueVehicular parqueVehicular)
        {
            ClsDatoString clavepermisostcc = db.Database.SqlQuery<ClsDatoString>("select Clave as Dato from [dbo].[c_TipoPermiso] where IDTipoPermiso =" + parqueVehicular.IDPermisoSCT).ToList().FirstOrDefault();
            parqueVehicular.ClavePermisoSCT = clavepermisostcc.Dato;
            ClsDatoString idvehiculo = db.Database.SqlQuery<ClsDatoString>("select ClaveNom as Dato from [dbo].[c_ConfigAutotransporte] where IDConfAutoT =" + parqueVehicular.IDVehiculo).ToList().FirstOrDefault();
            parqueVehicular.ClaveVehiculo = idvehiculo.Dato;
            ClsDatoString idremolque = db.Database.SqlQuery<ClsDatoString>("select ClaveTipoRemolque as Dato from [dbo].[c_SubTipoRem] where IDTRemolque=" + parqueVehicular.IDRemolque).ToList().FirstOrDefault();
            parqueVehicular.ClaveRemolque = idremolque.Dato;

            

               if (ModelState.IsValid)
            {

                //db.ParqueVe.Add(parqueVehicular);
                //db.SaveChanges();

                string cadenasql = "Insert into ParqueVehicular ([IDPermisoSCT],[ClavePermisoSCT],[NoPermisoSCT],[Aseguradora],[PolizaSeguro],[IDVehiculo],[ClaveVehiculo],[PlacaVehiculo],[AnnoVehiculo],[Modelo],[Color],[IDRemolque],[ClaveRemolque],[Placa],EsArrendado,IDArrendatario,TienePropietario,IDPropietario )  " +
                    "values ( " + parqueVehicular.IDPermisoSCT + ", '" + parqueVehicular.ClavePermisoSCT + "', '" + parqueVehicular.NoPermisoSCT + "', '" + parqueVehicular.Aseguradora + "', '" + parqueVehicular.PolizaSeguro + "', " + parqueVehicular.IDVehiculo + ",'"+ parqueVehicular.ClaveVehiculo+"','"+parqueVehicular.PlacaVehiculo+"',"+parqueVehicular.AnnoVehiculo+",'"+parqueVehicular.Modelo+"','"+parqueVehicular.Color+"',"+parqueVehicular.IDRemolque+",'"+parqueVehicular.ClaveRemolque+"','"+parqueVehicular.Placa+ "','" + parqueVehicular.EsArrendado + "'," + parqueVehicular.IDArrendatario + ",'" + parqueVehicular.TienePropietario + "', " + parqueVehicular.IDPropietario + ")";
                new ParqueVehicularDBContext().Database.ExecuteSqlCommand(cadenasql);

                return RedirectToAction("Index");
            }
               else
            {
                ParqueVehicularDBContext p = new ParqueVehicularDBContext();

                var permiso = new TipoPermisoRepository().GetTipoPermiso();
                ViewBag.IDPermisoSCT = permiso;

                var vehiculo = new ConfigAutotransporteRepository().GetConfigAutotransporte();
                ViewBag.IDVehiculo = vehiculo;

                var remolque = new TipoRemRepository().GetTipoRem();
                ViewBag.IDRemolque = remolque;

                var EsArrendadoLst = new List<SelectListItem>();
                EsArrendadoLst.Add(new SelectListItem { Text = "No", Value = "false" });
                EsArrendadoLst.Add(new SelectListItem { Text = "Si", Value = "true" });
                ViewBag.EsArrendado = new SelectList(EsArrendadoLst, "Value", "Text");

                var TienePropietarioLst = new List<SelectListItem>();
                TienePropietarioLst.Add(new SelectListItem { Text = "No", Value = "false" });
                TienePropietarioLst.Add(new SelectListItem { Text = "Si", Value = "true" });
                ViewBag.TienePropietario = new SelectList(TienePropietarioLst, "Value", "Text");

                var arrendatario = new ArrendatarioRepository().GetArrendatario();
                ViewBag.IDArrendatario = arrendatario;

                var propietario = new PropietarioRepository().GetPropietario();
                ViewBag.IDPropietario = propietario;

            }

            return View(parqueVehicular);
        }


        public ActionResult QuitarA(int id)
        {
            var elemento = db.ParqueVe.Single(m => m.IDParqueV == id);
            string cadenasql = "Update ParqueVehicular set IDArrendatario = 0 where IDParqueV =" + id;
            new ParqueVehicularDBContext().Database.ExecuteSqlCommand(cadenasql);
            return RedirectToAction("Index");
        }
        public ActionResult QuitarP(int id)
        {
            var elemento = db.ParqueVe.Single(m => m.IDParqueV == id);
            string cadenasql = "Update ParqueVehicular set IDPropietario = 0 where IDParqueV =" + id;
            new ParqueVehicularDBContext().Database.ExecuteSqlCommand(cadenasql);
            return RedirectToAction("Index");
        }

        // GET: ParqueVehicular/Edit/5
        public ActionResult Edit(int id)
        {
            var elemento = db.ParqueVe.Single(m => m.IDParqueV == id);

            ViewBag.IDPermisoSCT = new TipoPermisoRepository().GetTipoPermiso(elemento.IDPermisoSCT);
            ViewBag.IDVehiculo = new ConfigAutotransporteRepository().GetConfigAutotransporte(elemento.IDVehiculo);
            ViewBag.IDRemolque = new TipoRemRepository().GetTipoRem(elemento.IDRemolque);
            ViewBag.IDArrendatario = new ArrendatarioRepository().GetArrendatario(elemento.IDArrendatario);
            ViewBag.IDPropietario = new PropietarioRepository().GetPropietario(elemento.IDPropietario);

            var EsArrendadoLst = new List<SelectListItem>();
            EsArrendadoLst.Add(new SelectListItem { Text = "No", Value = "false" });
            EsArrendadoLst.Add(new SelectListItem { Text = "Si", Value = "true" });
            ViewBag.EsArrendado = new SelectList(EsArrendadoLst, "Value", "Text",elemento.EsArrendado);

            var TienePropietarioLst = new List<SelectListItem>();
            TienePropietarioLst.Add(new SelectListItem { Text = "No", Value = "false" });
            TienePropietarioLst.Add(new SelectListItem { Text = "Si", Value = "true" });
            ViewBag.TienePropietario = new SelectList(TienePropietarioLst, "Value", "Text", elemento.TienePropietario);
            List<ParqueVehicular> lista = db.Database.SqlQuery<ParqueVehicular>("select *from ParqueVehicular where IDParqueV ='" + id + "'").ToList();
            ViewBag.parque = lista;
            return View(elemento);       
        }

        // POST: ParqueVehicular/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult Edit(int id, FormCollection collection)
        public ActionResult Edit(ParqueVehicular elemento)
        {
            var elementon = db.ParqueVe.Single(m => m.IDParqueV == elemento.IDParqueV);
            ClsDatoString clavepermisostcc = db.Database.SqlQuery<ClsDatoString>("select Clave as Dato from [dbo].[c_TipoPermiso] where IDTipoPermiso =" + elemento.IDPermisoSCT).ToList().FirstOrDefault();
            elemento.ClavePermisoSCT = clavepermisostcc.Dato;
            ClsDatoString idvehiculo = db.Database.SqlQuery<ClsDatoString>("select ClaveNom as Dato from [dbo].[c_ConfigAutotransporte] where IDConfAutoT =" + elemento.IDVehiculo).ToList().FirstOrDefault();
            elemento.ClaveVehiculo = idvehiculo.Dato;
            ClsDatoString idremolque = db.Database.SqlQuery<ClsDatoString>("select ClaveTipoRemolque as Dato from [dbo].[c_SubTipoRem] where IDTRemolque=" + elemento.IDRemolque).ToList().FirstOrDefault();
            elemento.ClaveRemolque = idremolque.Dato;

            try
            {
                string query = "update [dbo].[ParqueVehicular] set [IDPermisoSCT]= " + elemento.IDPermisoSCT + ", [ClavePermisoSCT]= '" + elemento.ClavePermisoSCT + "' , [NoPermisoSCT]= '" + elemento.NoPermisoSCT + "' , [Aseguradora]='" + elemento.Aseguradora + "' , [PolizaSeguro]= '" + elemento.PolizaSeguro + "' , [IDVehiculo]=" + elemento.IDVehiculo + "  ,[ClaveVehiculo]= '" + elemento.ClaveVehiculo + "' , [PlacaVehiculo] = '" + elemento.PlacaVehiculo + "' , [AnnoVehiculo] = " + elemento.AnnoVehiculo + " , [Modelo] = '" + elemento.Modelo + "' , [Color] = '" + elemento.Color + "' , [IDRemolque] = " + elemento.IDRemolque + " ,[ClaveRemolque] = '" + elemento.ClaveRemolque + "', [Placa] = '" + elemento.Placa + "',[EsArrendado] = '" + elemento.EsArrendado + "' ,[IDArrendatario] = " + elemento.IDArrendatario + ",[TienePropietario] = '" + elemento.TienePropietario + "' ,[IDPropietario] = " + elemento.IDPropietario + " where[IDParqueV] = " + elemento.IDParqueV + ";";
                db.Database.ExecuteSqlCommand(query);
                return RedirectToAction("Index");

            }
            catch
            {
                ViewBag.IDPermisoSCT = new TipoPermisoRepository().GetTipoPermiso(elementon.IDPermisoSCT);
                ViewBag.IDVehiculo = new ConfigAutotransporteRepository().GetConfigAutotransporte(elementon.IDVehiculo);
                ViewBag.IDRemolque = new TipoRemRepository().GetTipoRem(elementon.IDRemolque);
                ViewBag.IDArrendatario = new ArrendatarioRepository().GetArrendatario(elementon.IDArrendatario);
                ViewBag.IDPropietario = new PropietarioRepository().GetPropietario(elementon.IDPropietario);

                var EsArrendadoLst = new List<SelectListItem>();
                EsArrendadoLst.Add(new SelectListItem { Text = "No", Value = "false" });
                EsArrendadoLst.Add(new SelectListItem { Text = "Si", Value = "true" });
                ViewBag.EsArrendado = new SelectList(EsArrendadoLst, "Value", "Text", elementon.EsArrendado);

                var TienePropietarioLst = new List<SelectListItem>();
                TienePropietarioLst.Add(new SelectListItem { Text = "No", Value = "false" });
                TienePropietarioLst.Add(new SelectListItem { Text = "Si", Value = "true" });
                ViewBag.TienePropietario = new SelectList(TienePropietarioLst, "Value", "Text", elementon.TienePropietario);
                List<ParqueVehicular> lista = db.Database.SqlQuery<ParqueVehicular>("select *from ParqueVehicular where IDParqueV ='" + elementon.IDParqueV + "'").ToList();
                ViewBag.parque = lista;             

                return View(elementon);
            }
            
        }

        // GET: ParqueVehicular/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ParqueVehicular parqueVehicular = db.ParqueVe.Find(id);
            if (parqueVehicular == null)
            {
                return HttpNotFound();
            }
            return View(parqueVehicular);
        }

        // POST: ParqueVehicular/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ParqueVehicular parqueVehicular = db.ParqueVe.Find(id);
            db.ParqueVe.Remove(parqueVehicular);
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


        public void GenerarExParqueVehicular ()
        {
            //Listado de datos
            var config = db.ParqueVe.ToList();
            ExcelPackage Ep = new ExcelPackage();
            var Sheet = Ep.Workbook.Worksheets.Add("ParqueVehicular");

            int row = 1;
            //Fijar la fuente para A1:Q1
            Sheet.Cells["A1:P1"].Style.Font.Size = 20;
            Sheet.Cells["A1:P1"].Style.Font.Name = "Calibri";
            Sheet.Cells["A1:P1"].Style.Font.Bold = true;
            Sheet.Cells["A1:P1"].Style.Font.Color.SetColor(Color.DarkBlue);
            Sheet.Cells["A1:P1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
            Sheet.Cells["A1"].RichText.Add("ConfigAutotransporte");
            Sheet.Cells["A1:P1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;

            row = 2;
            Sheet.Cells["A2:P2"].Style.Font.Name = "Calibri";
            Sheet.Cells["A2:P2"].Style.Font.Size = 12;
            Sheet.Cells["A2:P2"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;
            Sheet.Cells["A2:P2"].Style.Font.Bold = true;

            Sheet.Cells["B2"].RichText.Add("Tipo Permiso SCT");
            Sheet.Cells["A2"].RichText.Add("Clave Permiso SCT");
            Sheet.Cells["C2"].RichText.Add("No. Permiso SCT");
            Sheet.Cells["D2"].RichText.Add("Aseguradora");
            Sheet.Cells["E2"].RichText.Add("Poliza Seguro");
            Sheet.Cells["F2"].RichText.Add("Tipo Vehiculo");
            Sheet.Cells["G2"].RichText.Add("Clave Vehiculo");
            Sheet.Cells["H2"].RichText.Add("Placa Vehiculo");
            Sheet.Cells["I2"].RichText.Add("Año Vehiculo");
            Sheet.Cells["J2"].RichText.Add("Modelo");
            Sheet.Cells["K2"].RichText.Add("Color");
            Sheet.Cells["L2"].RichText.Add("Tipo Remolque");
            Sheet.Cells["M2"].RichText.Add("Clave Remolque");
            Sheet.Cells["N2"].RichText.Add("Placa");
            Sheet.Cells["O2"].RichText.Add("Es Arrendado");
            Sheet.Cells["P2"].RichText.Add("Tiene Propietario");
            Sheet.Cells["O2"].RichText.Add("Arrendatarrio");
            Sheet.Cells["P2"].RichText.Add("Propietario");
            row = 3;
            foreach (var item in config)
            {
                SIAAPI.Models.CartaPorte.c_TipoPermiso permiso = new SIAAPI.Models.CartaPorte.c_TipoPermisoContext().permiso.Find(item.IDPermisoSCT);
                SIAAPI.Models.CartaPorte.c_ConfigAutotransporte vehiculo = new SIAAPI.Models.CartaPorte.c_ConfigAutotransporteDBContext().ConfigAutotransporte.Find(item.IDVehiculo);
                SIAAPI.Models.CartaPorte.c_SubTipoRem remolque = new SIAAPI.Models.CartaPorte.c_SubTipoRemContext().TipoRem.Find(item.IDRemolque);


                Sheet.Cells[string.Format("A{0}", row)].Value = permiso.Descripcion;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.ClavePermisoSCT;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.NoPermisoSCT;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.Aseguradora;
                Sheet.Cells[string.Format("E{0}", row)].Value = item.PolizaSeguro;
                Sheet.Cells[string.Format("F{0}", row)].Value = vehiculo.Descripcion;
                Sheet.Cells[string.Format("G{0}", row)].Value = item.ClaveVehiculo;
                Sheet.Cells[string.Format("H{0}", row)].Value = item.PlacaVehiculo;
                Sheet.Cells[string.Format("I{0}", row)].Value = item.AnnoVehiculo;
                Sheet.Cells[string.Format("J{0}", row)].Value = item.Modelo;
                Sheet.Cells[string.Format("K{0}", row)].Value = item.Color;
                Sheet.Cells[string.Format("L{0}", row)].Value = remolque.RemoSemi;
                Sheet.Cells[string.Format("M{0}", row)].Value = item.ClaveRemolque;
                Sheet.Cells[string.Format("N{0}", row)].Value = item.Placa;
                if(item.EsArrendado== false)
                {
                    Sheet.Cells[string.Format("O{0}", row)].Value = "No";
                }
                else
                {
                    Sheet.Cells[string.Format("O{0}", row)].Value = "Si";
                }
                if (item.TienePropietario == false)
                {
                    Sheet.Cells[string.Format("P{0}", row)].Value = "No";
                }
                else
                {
                    Sheet.Cells[string.Format("P{0}", row)].Value = "Si";
                }

                if (item.IDPropietario == 0)
                {
                    Sheet.Cells[string.Format("Q{0}", row)].Value = "";
                }
                else
                {
                     
                    SIAAPI.Models.CartaPorte.Arrendatario arrendatario = new SIAAPI.Models.CartaPorte.ArrendatarioContext().Arrendatario.Find(item.IDArrendatario);
                    Sheet.Cells[string.Format("Q{0}", row)].Value = arrendatario.NombreArrendatario;
                }
                if (item.IDPropietario == 0)
                {
                    Sheet.Cells[string.Format("R{0}", row)].Value = "";
                }
                else
                {
                    SIAAPI.Models.CartaPorte.Propietario propietario = new SIAAPI.Models.CartaPorte.PropietarioContext().Propietario.Find(item.IDPropietario);
                    Sheet.Cells[string.Format("R{0}", row)].Value = propietario.NombrePropietario;
                }


                row++;
            }
            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "ReporteParqueVehicular.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
        }

        public ActionResult ReportePDFParqueVehicular()
        {
            var config = db.ParqueVe.ToList();
            Reporte__ParqueVehicular report = new Reporte__ParqueVehicular();
            report.ParqueVeh = config;
            byte[] abytes = report.PrepareReport();
            return File(abytes, "applicacion/pdf", "ReporteParqueVehicular.pdf");
        }


    }
}
