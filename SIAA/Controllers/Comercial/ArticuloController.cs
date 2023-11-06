using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SIAAPI.Models.Comercial;
using SIAAPI.Models.Administracion;
using SIAAPI.ViewModels.Articulo;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using SIAAPI.Models.Calidad;
using SIAAPI.Controllers.Cfdi;

namespace SIAAPI.Controllers
{
    [Authorize(Roles = "Administrador,Almacenista,Facturacion,Gerencia,Sistemas,Comercial,Ventas,GerenteVentas,AdminProduccion")]
    public class ArticuloController : Controller
    {
        private ArticuloContext db = new ArticuloContext();
        private FamiliaContext dbf = new FamiliaContext();
        private c_ClaveUnidadContext dbunidad = new c_ClaveUnidadContext();

        private c_MonedaContext dbmoneda = new c_MonedaContext();

        // GET: Articulo
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult getListArticulos()
        {
            try
            {
                var lista = db.Database.SqlQuery<VPArticulo>("Select TOP 100 *  from VPArticulo order by CREF WHERE TIPO='Articulo' ").ToList();
                return Json(lista);
            }
            catch (Exception err)
            {
                string mensajederror = err.Message;
                return null;
            }
        }

        public ActionResult getListArticulosFiltro(FilterArticulos filtro)
        {
            try
            {
                filtro.texto = "WHERE ";

                if (!String.IsNullOrEmpty(filtro.cmbTArticulo))
                {
                    filtro.texto = filtro.texto + "IDTipoArticulo = " + filtro.cmbTArticulo + " and ";
                }
                if (!String.IsNullOrEmpty(filtro.cmbTFamilia))
                {
                    filtro.texto = filtro.texto + "IDFamilia = " + filtro.cmbTFamilia + " and ";
                }

                if (filtro.texto.Length > 6)
                {
                    filtro.texto = filtro.texto.Substring(0, filtro.texto.Length - 4);
                }
                else
                {
                    filtro.texto = "";
                }

                string orden = " ORDER BY CREF";
                var lista = db.Database.SqlQuery<VPArticulo>("Select * from VPArticulo " + filtro.texto + orden).ToList();
                return Json(lista);
            }
            catch (Exception err)
            {
                string mensajederror = err.Message;
                return null;
            }
        }

        public ActionResult getArticulosxIDP(int IDA)
        {
            try
            {
                var lista = db.Database.SqlQuery<VPArticulo>("Select * from VPArticulo WHERE IDArticulo = " + IDA + " ").ToList();
                return Json(lista);
            }
            catch (Exception err)
            {
                string mensajederror = err.Message;
                return null;
            }
        }

        public ActionResult GetModelArticulos()
        {
            try
            {
                DatosArticulos da = new DatosArticulos();
                da.Articulos = db.Database.SqlQuery<VPArticulo>("Select * from VPArticulo ORDER BY CREF").ToList();
                //   var jda = Json(da);
                //return Json(new DatosArticulos());
                return Json(da);
            }
            catch (Exception err)
            {
                string mensajederror = err.Message;
                return null;
            }
        }

        public ActionResult getParametrosArticulos()
        {
            modelAddEditArticulo da = new modelAddEditArticulo();
            try
            {
                da.TipoArticulos = db.Database.SqlQuery<TipoArticulo>("Select * from TipoArticulo ").ToList();
                da.Familias = new FamiliaContext().Familias.ToList<Familia>();
                da.AQLCalidad = db.Database.SqlQuery<AQLCalidad>("Select * from AQLCalidad ").ToList();
                da.c_ClaveUnidad = db.Database.SqlQuery<c_ClaveUnidad>("Select * from c_ClaveUnidad ").ToList();
                da.Muestreo = db.Database.SqlQuery<Muestreo>("Select * from Muestreo ").ToList();
                da.Inspenccion = db.Database.SqlQuery<Inspeccion>("Select * from inspeccion ").ToList();
                da.Monedas = db.Database.SqlQuery<c_Moneda>("Select * from c_Moneda ").ToList();

                return Json(da);
            }
            catch (Exception err)
            {
                return Json(500, err.Message);
            }
        }


        public ActionResult GetModelArticuloXID(int IDA)
        {
            try
            {
                Articulo data = db.Articulo.Find(IDA);

                mArticulo Articulo = new mArticulo();
                Articulo.IDArticulo = data.IDArticulo;
                Articulo.Cref = data.Cref;
                Articulo.Descripcion = data.Descripcion;
                Articulo.IDFamilia = data.IDFamilia;
                Articulo.IDTipoArticulo = data.IDTipoArticulo;
                Articulo.Preciounico = data.Preciounico;
                Articulo.IDMoneda = data.IDMoneda;
                Articulo.CtrlStock = data.CtrlStock;
                Articulo.ManejoCar = data.ManejoCar;
                Articulo.IDClaveUnidad = data.IDClaveUnidad;
                Articulo.bCodigodebarra = data.bCodigodebarra;
                Articulo.Codigodebarras = data.Codigodebarras;
                Articulo.Obscalidad = data.Obscalidad;
                Articulo.ExistenDev = data.ExistenDev;
                Articulo.IDAQL = data.IDAQL;
                Articulo.IDInspeccion = data.IDInspeccion;
                Articulo.IDMuestreo = data.IDMuestreo;
                Articulo.esKit = data.esKit;
                Articulo.nameFoto = data.nameFoto;
                Articulo.GeneraOrden = data.GeneraOrden;
                Articulo.obsoleto = data.obsoleto;
                Articulo.MinimoVenta = data.MinimoVenta;
                Articulo.MinimoCompra = data.MinimoCompra;

                return Json(Articulo);
            }
            catch (Exception err)
            {
                string mensajederror = err.Message;
                return null;
            }
        }

        public ActionResult addArticulo(mArticulo Art)
        {
            try
            {
                Articulo newArt = new Articulo();
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

                if (ModelState.IsValid)
                {
                    db.Articulo.Add(newArt);
                    db.SaveChanges();
                    return Json(new HttpStatusCodeResult(200, "Operacion Exitosa"));
                }
                else
                {
                return Json(new HttpStatusCodeResult(500, "No cumple con las especificaciones de articulo |  " + Json(Art)));
                }
            }
            catch (Exception ex)
            {
                return Json(new HttpStatusCodeResult(500, ex.Message));
            }
        }

        public ActionResult updateArticulo(mArticulo Art)
        {
            try
            {
                



                //db.Database.ExecuteSqlCommand(cadena.ToString());
                Articulo newArt = new Articulo();
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
                newArt.nameFoto = saveIMG(Art.fileIMG, Art.nameFoto); // no vas a grabar la extencion de la imagen? // si alguna validacion se me fue, aun asi es un string no infiere en el problema 
                newArt.GeneraOrden = Art.GeneraOrden;
                newArt.obsoleto = Art.obsoleto;
                newArt.MinimoVenta = Art.MinimoVenta;
                newArt.MinimoCompra = Art.MinimoCompra;

                db.Entry(newArt).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json("");
            }
            catch (Exception ex)
            {
                return (new HttpStatusCodeResult(500, ex.Message));
            }
        }


        

        public ActionResult GetMatrizPrecios(int id)
        {
            try
            {

                List<MatrizPrecio> lista = db.Database.SqlQuery<MatrizPrecio>("Select * from MatrizPrecio where IDArticulo = " + id).ToList();
                return Json(lista);
            }
            catch (Exception err)
            {
                string mensajederror = err.Message;
                return null;
            }
        }

        public ActionResult GetMatrizCostos(int id)
        {
            try
            {
                List<MatrizCosto> lista = db.Database.SqlQuery<MatrizCosto>("Select * from MatrizCosto where IDArticulo = " + id).ToList();
                return Json(lista);
            }
            catch (Exception err)
            {
                string mensajederror = err.Message;
                return null;
            }
        }

      
        // GET: Articulo/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Articulo articulo = db.Articulo.Find(id);
            if (articulo == null)
            {
                return HttpNotFound();
            }
            return View(articulo);
        }

        // GET: Articulo/Create
        public ActionResult Create()
        {
            ElaborarComboFamilia();
            ElaborarComboInspeccion();
            ElaborarComboMoneda();
            ElaborarComboMuestreo();
            ElaborarComboTipoArticulo();
            ElaborarComboUnidad();
            ElaborarComboCalidad();

            return View();
        }


     


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateCaracteristica(int id, FormCollection collection)
        {
            // aqui voy a grabar el json que salga
            return View();
        }

      

        // GET: Articulo/Edit/5
        public ActionResult Edit(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Articulo articulo = db.Articulo.Find(id);

            if (articulo == null)
            {
                return HttpNotFound();
            }
            ElaborarComboFamilia(articulo.IDFamilia);
            ElaborarComboInspeccion(articulo.IDInspeccion);
            ElaborarComboMoneda(articulo.IDMoneda);
            ElaborarComboMuestreo(articulo.IDMuestreo);
            ElaborarComboTipoArticulo(articulo.IDTipoArticulo);
            ElaborarComboUnidad(articulo.IDClaveUnidad);
            ElaborarComboCalidad(articulo.IDAQL);
            return View(articulo);
        }

        //// POST: Articulo/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit(Articulo articulo)
        //{
        //    ElaborarComboFamilia(articulo.IDFamilia);
        //    ElaborarComboInspeccion(articulo.IDInspeccion);
        //    ElaborarComboMoneda(articulo.IDMoneda);
        //    ElaborarComboMuestreo(articulo.IDMuestreo);
        //    ElaborarComboTipoArticulo(articulo.IDTipoArticulo);
        //    ElaborarComboUnidad(articulo.IDClaveUnidad);
        //    ElaborarComboCalidad(articulo.IDAQL);
        //    try
        //    {

        //        HttpPostedFileBase archivo = Request.Files["Image1"];
        //        if (archivo.FileName != "")
        //        {
        //            articulo.Foto = new byte[archivo.ContentLength];
        //            archivo.InputStream.Read(articulo.Foto, 0, archivo.ContentLength);
        //        }
        //        if (ModelState.IsValid)
        //        {
        //            db.Entry(articulo).State = System.Data.Entity.EntityState.Modified;
        //            db.SaveChanges();
        //            return RedirectToAction("Index");
        //        }
        //        return View(articulo);
        //    }
        //    catch
        //    {
        //        return View(articulo);
        //    }
        //}

        // GET: Articulo/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Articulo articulo = db.Articulo.Find(id);
            if (articulo == null)
            {
                return HttpNotFound();
            }
            ElaborarComboFamilia(articulo.IDFamilia);
            ElaborarComboInspeccion(articulo.IDInspeccion);
            ElaborarComboMoneda(articulo.IDMoneda);
            ElaborarComboMuestreo(articulo.IDMuestreo);
            ElaborarComboTipoArticulo(articulo.IDTipoArticulo);
            ElaborarComboUnidad(articulo.IDClaveUnidad);
            ElaborarComboCalidad(articulo.IDAQL);
            return View(articulo);
        }

        // POST: Articulo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Articulo articulo = db.Articulo.Find(id);
            db.Articulo.Remove(articulo);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult AddUpdateRowMatrizPrecio(MatrizPrecio rowMatrizPrecio)
        {
            try
            {
                if ((String.IsNullOrEmpty(rowMatrizPrecio.idMatrizPrecio.ToString())) || (rowMatrizPrecio.idMatrizPrecio == 0))
                {
                    db.Database.ExecuteSqlCommand("insert into MatrizPrecio ( IDArticulo, Ranginf, RangSup, Precio ) values (" + rowMatrizPrecio.IDArticulo + "," + rowMatrizPrecio.RangInf + "," + rowMatrizPrecio.RangSup + "," + rowMatrizPrecio.Precio + ")");
                }
                else
                {
                    Articulo articulo = db.Articulo.Find(rowMatrizPrecio.IDArticulo);
                    if (articulo.Preciounico == true)
                    {
                        db.Database.ExecuteSqlCommand("Exec GuardaPrecioUnico " + rowMatrizPrecio.IDArticulo + "," + rowMatrizPrecio.Precio);
                    }
                    else
                    {
                        db.Database.ExecuteSqlCommand("updarte  MatrizPrecio set RangInf = " + rowMatrizPrecio.RangInf + " , RangSup = " + rowMatrizPrecio.RangSup + " , Precio = " + rowMatrizPrecio.Precio + " , IDArticulo = " + rowMatrizPrecio.IDArticulo + " WHERE idMatrizPrecio = " + rowMatrizPrecio.idMatrizPrecio);
                    }
                }

                return Json(new HttpStatusCodeResult(200, "Operacion Exitosa"));

            }
            catch (Exception err)
            {
                string mensajederror = err.Message;
                return Json(new HttpStatusCodeResult(500, "Datos invalidos"));
            }
        }

        public ActionResult AddUpdateRowMatrizCosto(MatrizCosto rowMatrizCosto)
        {
            try
            {
                if ((String.IsNullOrEmpty(rowMatrizCosto.idMatrizCosto.ToString())) || (rowMatrizCosto.idMatrizCosto == 0))
                {
                    db.Database.ExecuteSqlCommand("insert into MatrizCosto ( IDArticulo, Ranginf, RangSup, Precio ) values (" + rowMatrizCosto.IDArticulo + "," + rowMatrizCosto.RangInf + "," + rowMatrizCosto.RangSup + "," + rowMatrizCosto.Precio + ")");
                }
                else
                {
                    db.Database.ExecuteSqlCommand("update  MatrizCosto set RangInf = " + rowMatrizCosto.RangInf + " , RangSup = " + rowMatrizCosto.RangSup + " , Precio = " + rowMatrizCosto.Precio + " , IDArticulo = " + rowMatrizCosto.IDArticulo + " WHERE idMatrizCosto = " + rowMatrizCosto.idMatrizCosto);
                }
                return Json(new HttpStatusCodeResult(200, "Operacion Exitosa"));
            }
            catch (Exception err)
            {
                string mensajederror = err.Message;
                return Json(new HttpStatusCodeResult(500, "Datos invalidos"));
            }
        }





        public ActionResult deleteRowRowMatrizPrecio(int id)
        {
            try
            {
                db.Database.ExecuteSqlCommand("delete from MatrizPrecio where IDMatrizPrecio=" + id);

                return Json(new HttpStatusCodeResult(200, "Operacion Exitosa"));
            }
            catch (Exception err)
            {
                string mensajederror = err.Message;
                return Json(new HttpStatusCodeResult(500, "Tus datos estan siendo utilizados por lo que no puede eliminarse"));
            }
        }

        public ActionResult deleteRowRowMatrizCosto(int id)

        {
            try
            {
                db.Database.ExecuteSqlCommand("delete from MatrizCosto where IDMatrizCosto=" + id);

                return Json(new HttpStatusCodeResult(200, "Operacion Exitosa"));
            }
            catch (Exception err)
            {
                string mensajederror = err.Message;
                return Json(new HttpStatusCodeResult(500, "Tus datos estan siendo utilizados por lo que no puede eliminarse"));
            }
        }

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}



        private void ElaborarComboFamilia(object selectedFamilia = null)
        {
            var familiasQuery = from d in dbf.Familias
                                orderby d.Descripcion
                                select d;
            ViewBag.IDFamilia = new SelectList(familiasQuery, "IDFamilia", "Descripcion", selectedFamilia);
        }

        private void ElaborarComboMuestreo(object selectedMuestreo = null)
        {
            var MuestreoQuery = from d in db.Muestreo
                                orderby d.Descripcion
                                select d;
            ViewBag.IDMuestreo = new SelectList(MuestreoQuery, "IDMuestreo", "Descripcion", selectedMuestreo);
        }

        private void ElaborarComboUnidad(object selectedUnidad = null)
        {
            var UnidadQuery = from d in dbunidad.c_ClaveUnidades
                              orderby d.Nombre
                              select d;
            ViewBag.IDClaveUnidad = new SelectList(UnidadQuery, "IDClaveUnidad", "Nombre", selectedUnidad);
        }

        private void ElaborarComboTipoArticulo(object selectedTA = null)
        {
            var TAQuery = from d in db.TipoArticulo
                          orderby d.Descripcion
                          select d;
            ViewBag.IDTipoArticulo = new SelectList(TAQuery, "IDTipoArticulo", "Descripcion", selectedTA);
        }


        public string saveIMG(HttpPostedFileBase file, string name)
        {
            try
            {
                if ((file != null) )
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


        private void ElaborarComboMoneda(object selectedTA = null)
        {
            var MonedaQuery = from d in dbmoneda.c_Monedas
                              orderby d.Descripcion
                              select d;
            ViewBag.IDMoneda = new SelectList(MonedaQuery, "IDMoneda", "Descripcion", selectedTA);
        }

        private void ElaborarComboInspeccion(object selectedInspeccion = null)
        {
            var InspeccionQuery = from d in db.Inspeccion
                                  orderby d.Descripcion
                                  select d;
            ViewBag.IDInspeccion = new SelectList(InspeccionQuery, "IDInspeccion", "Descripcion", selectedInspeccion);
        }

        private void ElaborarComboCalidad(object selectedCalidad = null)
        {
            var CalidadQuery = from d in db.AQLCalidad
                               orderby d.Descripcion
                               select d;
            ViewBag.IDAQL = new SelectList(CalidadQuery, "IDAQL", "Descripcion", selectedCalidad);
        }

        //public async Task<ActionResult> RenderImage(int id)
        //{
        //    ArticuloContext db = new ArticuloContext();
        //    Articulo item = await db.Articulo.FindAsync(id);

        //    byte[] photoBack = item.Foto;

        //    return File(photoBack, "image/png");
        //}


        //public ActionResult GetListPresentaciones_Schema(Articulo art)
        //{
        //    try
        //    {
        //        LPresentacionSchema dat = new LPresentacionSchema();
        //        dat.lPrecentacion = db.Database.SqlQuery<LPresentacion>("Select [ID] ,[IDPresentacion],[Cotizacion] ,[version] ,[Presentacion] ,[jsonPresentacion] ,[Articulo_IDArticulo] ,[obsoleto] from Caracteristica where Articulo_IDArticulo = " + art.IDArticulo + " order by IDPresentacion").ToList();
        //        dat.Schema = new FamiliaRepository().getAtributosJsonSchemaFJ(art.IDFamilia);
        //        return Json(dat);
        //    }
        //    catch (Exception err)
        //    {
        //        return Json(new HttpStatusCodeResult(500, "Datos invalidos"));
        //    }
        //}





        /// <summary>
        /// ActionResult para Agregar/Modificar las Precentaciones de cada uno de los Articulos 
        /// </summary>
        /// <param name="pre"> pre es el Modelo de Caracteristicas de los Productos </param>
        /// <returns> retorna un Json que sera recivido por AngularJs </returns>
        public ActionResult addUpdatePresentacion(Caracteristica pre) // solo pasas el IDArticulo y el Json del formulario dinamico
        {
            try
            {
                bool insertar = pre.ID > 0 ? false : true;

                pre.Presentacion = String.IsNullOrEmpty(pre.jsonPresentacion) == true ? "{}" : pre.jsonPresentacion.Replace("{", "").Replace("}", "").Replace("\"", "");

                if (insertar)
                {
                    int NewIDP = db.Database.SqlQuery<int>("SELECT ISNULL(MAX(IDPRESENTACION)+1,0) from Caracteristica where Articulo_IDArticulo = " + pre.Articulo_IDArticulo + " ").FirstOrDefault();
                    //int NewIDP = Convert.ToInt32(db.Database.SqlQuery("Select MAX(IDPresentacion)+1 from Caracteristica where Articulo_IDArticulo = ", pre.Articulo_IDArticulo));
                    NewIDP = NewIDP > 0 ? NewIDP : 1;
                    db.Database.ExecuteSqlCommand("insert into Caracteristica ( IDPresentacion, Cotizacion, version, Presentacion, jsonPresentacion, Articulo_IDArticulo )  values (" + NewIDP + "," + pre.Cotizacion + "," + pre.version + ",'" + pre.Presentacion + "','" + pre.jsonPresentacion + "'," + pre.Articulo_IDArticulo + ")");

                    return Json(new HttpStatusCodeResult(200, "Presentación agregada Correctamente!"));
                }
                else // es modificacion
                {
                    db.Database.ExecuteSqlCommand("update Caracteristica set Cotizacion=" + pre.Cotizacion + ", version=" + pre.version + ", Presentacion='" + pre.Presentacion + "', JsonPresentacion='" + pre.jsonPresentacion + "' where ID =" + pre.ID);
                    return Json(new HttpStatusCodeResult(200, "La Presentación se modificó Correctamente!"));
                }
            }
            catch (Exception err)
            {
                string mensajederror = err.Message;
                return Json(new HttpStatusCodeResult(500, "Datos invalidos"));
            }
        }


        public ActionResult deletePresentacion(int id)
        {
            try
            {
                db.Database.ExecuteSqlCommand("delete from Caracteristica where ID=" + id);
                return Json(new HttpStatusCodeResult(200, "La presentacion fue eliminada Exitosamente!"));
            }
            catch (SqlException err)
            {
                string mensajederror = err.Message;
                return Json(new HttpStatusCodeResult(500, "Este dato esta siendo utilizado"));
            }
        }
      
        

    }
}