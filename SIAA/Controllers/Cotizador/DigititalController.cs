using SIAAPI.ClasesProduccion;
using SIAAPI.Models.Administracion;
using SIAAPI.Models.Comercial;
using SIAAPI.Models.Diseno;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

using System.Web.Mvc;
using System.Xml;
using System.Xml.Serialization;

namespace SIAAPI.Controllers.Cotizador
{
    public class DigitalController : Controller
    {
        // GET: Digitital
        public ActionResult SolicitudSuajeDigital()
        {
            ClsDigital digital = new ClsDigital();
            digital.SuajeNuevo = true;

          



            var Maquina = new List<SelectListItem>();
          

            Maquina.Add(new SelectListItem { Text = "Digital", Value = "Digital" });
         

            ViewBag.Maquina = new SelectList(Maquina, "Value", "Text");



          

            var Presentacion = new List<SelectListItem>();
           
            Presentacion.Add(new SelectListItem { Text = "Rollo", Value = "Rollo" });
            Presentacion.Add(new SelectListItem { Text = "Planilla", Value = "Planilla" });

            ViewBag.Presentacion = new SelectList(Presentacion, "Value", "Text");



            ViewData["Presentacion"] = Presentacion;



          

            var Acabado = new List<SelectListItem>();
            Acabado.Add(new SelectListItem { Text = "Ninguno", Value = "Ninguno" });

            Acabado.Add(new SelectListItem { Text = "Laminado", Value = "Laminado" });
            Acabado.Add(new SelectListItem { Text = "Barniz", Value = "Barniz" });
            Acabado.Add(new SelectListItem { Text = "Laminado Brillante", Value = "Laminado Brillante" });
            Acabado.Add(new SelectListItem { Text = "Laminado Mate", Value = "Laminado Mate" });
            Acabado.Add(new SelectListItem { Text = "Barniz Brillante", Value = "Barniz Brillante" });
            Acabado.Add(new SelectListItem { Text = "Barniz Mate", Value = "Barniz Mate" });
            Acabado.Add(new SelectListItem { Text = "Foil", Value = "Foil" });
            Acabado.Add(new SelectListItem { Text = "Cast&Cute", Value = "Cast&Cute" });
            ViewBag.Acabado = new SelectList(Acabado, "Value", "Text");



            ViewData["Acabado"] = Acabado;
            var Embobinado = new List<SelectListItem>();
            Embobinado.Add(new SelectListItem { Text = "A", Value = "A" });
            Embobinado.Add(new SelectListItem { Text = "B", Value = "B" });
            Embobinado.Add(new SelectListItem { Text = "C", Value = "C" });
            Embobinado.Add(new SelectListItem { Text = "D", Value = "D" });
            Embobinado.Add(new SelectListItem { Text = "E", Value = "E" });
            Embobinado.Add(new SelectListItem { Text = "F", Value = "F" });
            Embobinado.Add(new SelectListItem { Text = "G", Value = "G" });
            Embobinado.Add(new SelectListItem { Text = "H", Value = "H" });
            Embobinado.Add(new SelectListItem { Text = "I", Value = "I" });
            Embobinado.Add(new SelectListItem { Text = "FAN FOLDER", Value = "FAN FOLDER" });
            Embobinado.Add(new SelectListItem { Text = "HOJA", Value = "HOJA" });
            ViewBag.Embobinado = new SelectList(Embobinado, "Value", "Text");



            ViewData["Embobinado"] = Embobinado;


            var Metodo = new List<SelectListItem>();
            Metodo.Add(new SelectListItem { Text = "Sin Especificar", Value = "Sin Especificar" });

            Metodo.Add(new SelectListItem { Text = "SemiAutomatico", Value = "SemiAutomatico" });
            Metodo.Add(new SelectListItem { Text = "Automatico", Value = "Automatico" });
            Metodo.Add(new SelectListItem { Text = "Manual", Value = "Manual" });

            ViewBag.Metodo = new SelectList(Metodo, "Value", "Text");



            ViewData["Metodo"] = Metodo;







            var TipoFigura = new List<SelectListItem>();
            //FacPagLst.Add(new SelectListItem { Text = "Todos", Value = "na", Selected = true });
            TipoFigura.Add(new SelectListItem { Text = "Rectangulo", Value = "Rectangulo" });
            TipoFigura.Add(new SelectListItem { Text = "Circulo", Value = "Circulo" });
            TipoFigura.Add(new SelectListItem { Text = "Ovalo", Value = "Ovalo" });
            TipoFigura.Add(new SelectListItem { Text = "Recto", Value = "Recto" });
            TipoFigura.Add(new SelectListItem { Text = "Muesca General", Value = "Muesca General" });
            TipoFigura.Add(new SelectListItem { Text = "Muesca Especial", Value = "Muesca Especial" });
            TipoFigura.Add(new SelectListItem { Text = "Corte Recto", Value = "Corte Recto" });
            TipoFigura.Add(new SelectListItem { Text = "Figura Especial", Value = "Figura Especial" });


            ViewBag.TipoSuajeFigura = new SelectList(TipoFigura, "Value", "Text");

            ViewData["TipoSuajeFigura"] = TipoFigura;

            var TipoCorte = new List<SelectListItem>();
            //FacPagLst.Add(new SelectListItem { Text = "Todos", Value = "na", Selected = true });
            TipoCorte.Add(new SelectListItem { Text = "Sin Seleccionar", Value = "Sin Seleccionar" });
            TipoCorte.Add(new SelectListItem { Text = "Medio Corte", Value = "Medio Corte" });
            TipoCorte.Add(new SelectListItem { Text = "Corte Completo", Value = "Corte Completo" });
            TipoCorte.Add(new SelectListItem { Text = "Ponche", Value = "Ponche" });
            TipoCorte.Add(new SelectListItem { Text = "Multinivel", Value = "Multinivel" });
            TipoCorte.Add(new SelectListItem { Text = "Muesca", Value = "Muesca" });
            TipoCorte.Add(new SelectListItem { Text = "Solo Archivo", Value = "Solo Archivo" });

            ViewBag.TipoCorte = new SelectList(TipoCorte, "Value", "Text");

            ViewData["TipoCorte"] = TipoCorte;

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

            ViewBag.EsquinasSuaje = new SelectList(EsquinasSuaje, "Value", "Text");

            ViewData["Esquinas"] = EsquinasSuaje;


            var cintas = new Repository().GetCintas();
            ViewBag.IDMaterial = cintas;

            digital.EjeMaquina = 12.5M;



            List<SelectListItem> vendedor = new List<SelectListItem>();
       
            vendedor.Add(new SelectListItem { Text = "-------------------------", Value = "0" });
            var todosvendedor = new VendedorContext().Vendedores.ToList().Where(s => s.Activo == true).OrderBy(s => s.Nombre);
            if (todosvendedor != null)
            {
                foreach (var y in todosvendedor)
                {
                    vendedor.Add(new SelectListItem { Text = y.Nombre, Value = y.IDVendedor.ToString() });
                }
            }

            ViewBag.IDVendedor = vendedor;


            ClientesContext clientes = new ClientesContext();
            ViewBag.IDCliente = new ClienteRepository().GetClientesNombre();
            ClientesPContext clientesp = new ClientesPContext();
            List<SelectListItem> ClientesPr = new List<SelectListItem>();
            List<ClientesP> cl = new ArticuloContext().Database.SqlQuery<ClientesP>("select * from ClientesP order by Nombre").ToList();
            ClientesPr.Add(new SelectListItem { Text = "-------------------------", Value = "0" });
            foreach (ClientesP y in cl)
            {

                ClientesPr.Add(new SelectListItem { Text = y.Nombre, Value = y.IDClienteP.ToString() });
            }

            ViewBag.IDClienteP = ClientesPr;
            //ViewBag.IDClienteP = new SelectList(clientesp.ClientesPs, "IDClienteP", "Nombre");

            return View(digital);
        }


        public ActionResult CargarsuajeDigital(int Id, int IDDisenoS=0)
        {
            Cotizaciones archivocotizacion = new ArchivoCotizadorContext().cotizaciones.Find(Id);
            XmlDocument documento = new XmlDocument();
            string nombredearchivo = archivocotizacion.Ruta.TrimEnd() + "\\" + archivocotizacion.NombreArchivo + ".xml";
            documento.Load(nombredearchivo);
            ClsDigital elemento = null;
            XmlSerializer serializer = new XmlSerializer(typeof(ClsDigital));
            try
            {
                XmlDocument documentoDI = new XmlDocument();
                string nombredearchivoDI = System.Web.HttpContext.Current.Server.MapPath("~/DisenoCotizacion/SolicitudDiseno/Solicitud-" + IDDisenoS + ".xml");

                documentoDI.Load(nombredearchivoDI);


                using (Stream reader = new FileStream(nombredearchivoDI, FileMode.Open))
                {
                    // Call the Deserialize method to restore the object's state.
                    elemento = (ClsDigital)serializer.Deserialize(reader);
                }
            }
            catch (Exception er)
            {
                try
                {

                    //XmlDocument documento = new XmlDocument();
                    //string nombredearchivo = archivocotizacion.Ruta.TrimEnd() + "\\" + archivocotizacion.NombreArchivo + ".xml";
                    documento.Load(nombredearchivo);



                    using (Stream reader = new FileStream(nombredearchivo, FileMode.Open))
                    {
                        // Call the Deserialize method to restore the object's state.
                        elemento = (ClsDigital)serializer.Deserialize(reader);
                    }
                    if (elemento.IDSuaje == 0)
              {
                   elemento.SuajeNuevo = true;
                }
                }
                catch (Exception ere)
                {
                    string mensajedeerror = er.Message;
                    elemento = (ClsDigital)serializer.Deserialize(StringExtensiones.ToXmlReader(archivocotizacion.contenido));
                }



            }

            //try
            //{
            //    using (Stream reader = new FileStream(nombredearchivo, FileMode.Open))
            //    {
            //        // Call the Deserialize method to restore the object's state.
            //        elemento = (ClsDigital)serializer.Deserialize(reader);
            //    }

            //    if (elemento.IDSuaje == 0)
            //    {
            //        elemento.SuajeNuevo = true;
            //    }
            //}
            //catch(Exception err)
            //{

            //}

            ViewBag.Cliente = "";
            if (elemento.IDCliente!=0)
            {
                ViewBag.Cliente = new ClientesContext().Clientes.Find(elemento.IDCliente).Nombre;
            }
            if (elemento.IDClienteP!=0)
            {
                ClientesP clientep = new ClientesPContext().Database.SqlQuery<ClientesP>("select*from ClientesP where idclientep=" + elemento.IDClienteP).ToList().FirstOrDefault();
                ViewBag.Cliente = clientep.Nombre;
            }

            ViewBag.Vendedor = new VendedorContext().Vendedores.Find(elemento.IDVendedor).Nombre;

           

            ViewBag.Material = elemento.Material;

            return View(elemento);

        }



        public ActionResult CargarDisenoDigital(int Id, int IDDisenoS=0)
        {
            Cotizaciones archivocotizacion = new ArchivoCotizadorContext().cotizaciones.Find(Id);
            
            ClsDisenoDigital elemento = null;

            XmlSerializer serializerX = new XmlSerializer(typeof(ClsDisenoDigital));
            try
            {
                XmlDocument documentoDI = new XmlDocument();
                string nombredearchivoDI = System.Web.HttpContext.Current.Server.MapPath("~/DisenoCotizacion/SolicitudDiseno/Solicitud-" + IDDisenoS + ".xml");

                documentoDI.Load(nombredearchivoDI);


                using (Stream reader = new FileStream(nombredearchivoDI, FileMode.Open))
                {
                    // Call the Deserialize method to restore the object's state.
                    elemento = (ClsDisenoDigital)serializerX.Deserialize(reader);
                }
            }
            catch (Exception er)
            {
                try
                {

                    XmlDocument documento = new XmlDocument();
                    string nombredearchivo = archivocotizacion.Ruta.TrimEnd() + "\\" + archivocotizacion.NombreArchivo + ".xml";
                    documento.Load(nombredearchivo);



                    using (Stream reader = new FileStream(nombredearchivo, FileMode.Open))
                    {
                        // Call the Deserialize method to restore the object's state.
                        elemento = (ClsDisenoDigital)serializerX.Deserialize(reader);
                    }
                }
                catch (Exception ere)
                {
                    string mensajedeerror = er.Message;
                    elemento = (ClsDisenoDigital)serializerX.Deserialize(StringExtensiones.ToXmlReader(archivocotizacion.contenido));
                }



            }



            ViewBag.Cliente = "";
            if (elemento.IDCliente != 0)
            {
                Clientes cliente = new ClientesContext().Database.SqlQuery<Clientes>("select*from Clientes where idcliente=" + elemento.IDCliente).ToList().FirstOrDefault();

                ViewBag.Cliente = cliente.Nombre;/*new ClientesContext().Clientes.Find(elemento.IDCliente).Nombre;*/
            }
            if (elemento.IDClienteP != 0)
            {
                ClientesP clientep = new ClientesPContext().Database.SqlQuery<ClientesP>("select*from ClientesP where idclientep=" + elemento.IDClienteP).ToList().FirstOrDefault();
                ViewBag.Cliente = clientep.Nombre;
            }
            ViewBag.Vendedor = new VendedorContext().Vendedores.Find(elemento.IDVendedor).Nombre;

            ViewBag.Material = "";

            ViewBag.Material = new MaterialesContext().Materiales.Find(elemento.IDMaterial);

            Caracteristica suaje = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from Caracteristica where id="+elemento.IDSuaje).FirstOrDefault();
            ViewBag.Suaje = "";
            if (suaje == null)
            {
                ViewBag.Suaje = "Suaje Nuevo o sin especificar";
            }
            else
            {
                Articulo articulo = new ArticuloContext().Articulo.Find(suaje.Articulo_IDArticulo);
                ViewBag.Suaje = articulo.Cref + " " + articulo.Descripcion;
            }








            return View(elemento);

        }




        [System.Web.Mvc.HttpPost]
        public ActionResult SolicitudSuajeDigital(ClsDigital elemento)
        {
            try
            {
                if (elemento.anchoproductomm==0)
                {
                    throw new Exception("Tu solicitud debe tener una ancho en mm");
                }
                if (elemento.largoproductomm == 0)
                {
                    throw new Exception("Tu solicitud debe tener una largo en mm");
                }
                if (elemento.cavidadesdesuajeEje == 0)
                {
                    throw new Exception("Tu solicitud debe tener al menos una cavidad al eje");
                }
                if (elemento.cavidadesdesuajeAvance == 0)
                {
                    throw new Exception("Tu solicitud debe tener al menos una cavidad al eje");
                }
                if (elemento.cavidadesdesuajeAvance == 0)
                {
                    throw new Exception("Tu solicitud debe tener al menos una cavidad al Avance");
                }
                if (elemento.Metodo == "Sin Seleccionar")
                {
                    throw new Exception("No has indicado un Metodo");
                }
                if (elemento.TipoCorte == "Sin Seleccionar")
                {
                    throw new Exception("No has indicado un Tipo de corte");
                }
                if (elemento.Material == null || elemento.Material == string.Empty)
                {
                    throw new Exception("No  Escribiste un material");
                }
            }
            catch(Exception err)
            {
                ViewBag.Error = err.Message;




                var Maquina = new List<SelectListItem>();
             

                Maquina.Add(new SelectListItem { Text = "Digital", Value = "Digital" });
               

                ViewBag.Maquina = new SelectList(Maquina, "Value", "Text",elemento.Maquina);



                ViewData["Maquina"] = Maquina;

                var Presentacion = new List<SelectListItem>();
             

                Presentacion.Add(new SelectListItem { Text = "Rollo", Value = "Rollo" });
                Presentacion.Add(new SelectListItem { Text = "Planilla", Value = "Planilla" });

                ViewBag.Presentacion = new SelectList(Presentacion, "Value", "Text",elemento.Presentacion);



                ViewData["Presentacion"] = Presentacion;








                var Acabado = new List<SelectListItem>();
                Acabado.Add(new SelectListItem { Text = "Ninguno", Value = "Ninguno" });

                Acabado.Add(new SelectListItem { Text = "Laminado", Value = "Laminado" });
                Acabado.Add(new SelectListItem { Text = "Barniz", Value = "Barniz" });
                Acabado.Add(new SelectListItem { Text = "Laminado Brillante", Value = "Laminado Brillante" });
                Acabado.Add(new SelectListItem { Text = "Laminado Mate", Value = "Laminado Mate" });
                Acabado.Add(new SelectListItem { Text = "Barniz Brillante", Value = "Barniz Brillante" });
                Acabado.Add(new SelectListItem { Text = "Barniz Mate", Value = "Barniz Mate" });
                Acabado.Add(new SelectListItem { Text = "Foil", Value = "Foil" });
                Acabado.Add(new SelectListItem { Text = "Cast&Cute", Value = "Cast&Cute" });
                ViewBag.Acabado = new SelectList(Acabado, "Value", "Text");



                ViewData["Acabado"] = Acabado;
                var Embobinado = new List<SelectListItem>();
                Embobinado.Add(new SelectListItem { Text = "A", Value = "A" });
                Embobinado.Add(new SelectListItem { Text = "B", Value = "B" });
                Embobinado.Add(new SelectListItem { Text = "C", Value = "C" });
                Embobinado.Add(new SelectListItem { Text = "D", Value = "D" });
                Embobinado.Add(new SelectListItem { Text = "E", Value = "E" });
                Embobinado.Add(new SelectListItem { Text = "F", Value = "F" });
                Embobinado.Add(new SelectListItem { Text = "G", Value = "G" });
                Embobinado.Add(new SelectListItem { Text = "H", Value = "H" });
                Embobinado.Add(new SelectListItem { Text = "I", Value = "I" });
                Embobinado.Add(new SelectListItem { Text = "FAN FOLDER", Value = "FAN FOLDER" });
                Embobinado.Add(new SelectListItem { Text = "HOJA", Value = "HOJA" });
                ViewBag.Embobinado = new SelectList(Embobinado, "Value", "Text");



                ViewData["Embobinado"] = Embobinado;



                var Metodo = new List<SelectListItem>();

             
                Metodo.Add(new SelectListItem { Text = "Sin Especificar", Value = "Sin Especificar" });

                Metodo.Add(new SelectListItem { Text = "SemiAutomatico", Value = "SemiAutomatico" });
                Metodo.Add(new SelectListItem { Text = "Automatico", Value = "Automatico" });
                Metodo.Add(new SelectListItem { Text = "Manual", Value = "Manual" });

                ViewBag.Metodo = new SelectList(Metodo, "Value", "Text",elemento.Metodo);



                ViewData["Metodo"] = Metodo;

                



                var TipoFigura = new List<SelectListItem>();
                //FacPagLst.Add(new SelectListItem { Text = "Todos", Value = "na", Selected = true });
               
                TipoFigura.Add(new SelectListItem { Text = "Circulo", Value = "Circulo" });
                TipoFigura.Add(new SelectListItem { Text = "Rectangulo", Value = "Rectangulo" });
                TipoFigura.Add(new SelectListItem { Text = "Ovalo", Value = "Ovalo" });
                TipoFigura.Add(new SelectListItem { Text = "Recto", Value = "Recto" });
                TipoFigura.Add(new SelectListItem { Text = "Muesca General", Value = "Muesca General" });
                TipoFigura.Add(new SelectListItem { Text = "Muesca Especial", Value = "Muesca Especial" });
                TipoFigura.Add(new SelectListItem { Text = "Corte Recto", Value = "Corte Recto" });
                TipoFigura.Add(new SelectListItem { Text = "Figura Especial", Value = "Figura Especial" });


                ViewBag.TipoSuajeFigura = new SelectList(TipoFigura, "Value", "Text",elemento.TipoSuajeFigura);


                var TipoCorte = new List<SelectListItem>();
                //FacPagLst.Add(new SelectListItem { Text = "Todos", Value = "na", Selected = true });
                TipoCorte.Add(new SelectListItem { Text = "Sin Seleccionar", Value = "Sin Seleccionar" });
                TipoCorte.Add(new SelectListItem { Text = "Medio Corte", Value = "Medio Corte" });
                TipoCorte.Add(new SelectListItem { Text = "Corte Completo", Value = "Corte Completo" });
                TipoCorte.Add(new SelectListItem { Text = "Ponche", Value = "Ponche" });
                TipoCorte.Add(new SelectListItem { Text = "Multinivel", Value = "Multinivel" });
                TipoCorte.Add(new SelectListItem { Text = "Muesca", Value = "Muesca" });
                TipoCorte.Add(new SelectListItem { Text = "Solo Archivo", Value = "Solo Archivo" });

                ViewBag.TipoCorte = new SelectList(TipoCorte, "Value", "Text", elemento.TipoCorte);

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

                ViewBag.EsquinasSuaje = new SelectList(EsquinasSuaje, "Value", "Text",elemento.Esquinas);
                ViewData["Esquinas"] = EsquinasSuaje;


                MaterialesContext mat = new MaterialesContext();

                if (elemento.IDMaterial == 0)
                {
                    var cintas = new Repository().GetCintas();
                    ViewBag.IDMaterial = cintas;
                }
                else
                {
                    ViewBag.IDMaterial = new SelectList(mat.Materiales.OrderBy(s => s.Descripcion), "ID", "Descripcion", elemento.IDMaterial);
                }



               
                VendedorContext vendedores = new VendedorContext();

                ViewBag.IDVendedor = new SelectList(vendedores.Vendedores.Where(s => s.Activo == true).OrderBy(s => s.Nombre).ToList(),"IDVendedor","Nombre",elemento.IDVendedor);

               

                ClientesContext clientes = new ClientesContext();
                ViewBag.IDCliente = new SelectList(clientes.Clientes, "IDCliente", "Nombre", elemento.IDCliente);
                List<SelectListItem> ClientesPr = new List<SelectListItem>();
                List<ClientesP> cl = new ArticuloContext().Database.SqlQuery<ClientesP>("select * from ClientesP order by Nombre").ToList();

                foreach (ClientesP y in cl)
                {

                    ClientesPr.Add(new SelectListItem { Text = y.Nombre, Value = y.IDClienteP.ToString() });
                }

                ViewBag.IDClienteP = ClientesPr;
                //ClientesPContext clientesp = new ClientesPContext();
                //ViewBag.IDClienteP = new SelectList(clientesp.ClientesPs, "IDClienteP", "Nombre",elemento.IDClienteP);

                return View(elemento);
            }



            string NombredeArchivo ="SJD" + DateTime.Now.ToString().Replace(".", "").Replace(",", "").Replace(" ", "").Replace("/", "").Replace(":", "");

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


            return RedirectToAction("Index", "Diseno");

        }

        public JsonResult getarticulosblando(string buscar)
        {

            var Articulos = new MaterialesContext().Materiales.Where(s => s.Descripcion.Contains(buscar) || s.Clave.Contains(buscar)).OrderBy(S => S.Clave);

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


        public JsonResult getClientesBlando(string buscar)
        {

            var Clientes = new ClientesContext().Clientes.Where(s => s.Nombre.Contains(buscar)  );

            // Populate DropDownList
            List<SelectListItem> opciones = new List<SelectListItem>();

            foreach (Clientes art in Clientes)
            {
                SelectListItem elemento = new SelectListItem();
                elemento.Text =  art.Nombre;
                elemento.Value = art.IDCliente.ToString();
                opciones.Add(elemento);
            }

            return Json(opciones, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getProspeBlando(string buscar)
        {
            string consulta = "select* from clientesP where Nombre like '%" + buscar + "%'";
            List<ClientesP> Clientes = new ClientesPContext().Database.SqlQuery<ClientesP>(consulta).ToList();

            //var Clientes = new ClientesPContext().ClientesPs.Where(s => s.Nombre.Contains(buscar));

            // Populate DropDownList
            List<SelectListItem> opciones = new List<SelectListItem>();

            foreach (ClientesP art in Clientes)
            {
                SelectListItem elemento = new SelectListItem();
                elemento.Text = art.Nombre;
                elemento.Value = art.IDClienteP.ToString();
                opciones.Add(elemento);
            }

            return Json(opciones, JsonRequestBehavior.AllowGet);
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


        public void GrabarArchivoCotizador(ClsDigital elemento, string _nombredearchivo, string _ruta)
        {

           

         

            StringWriter stringwriter = new StringWriter();
            XmlSerializer x = new XmlSerializer(elemento.GetType());
            x.Serialize(stringwriter, elemento);
            string mensaje = stringwriter.ToString();

          

            Cotizaciones archivo = new Cotizaciones();


            string nombredearchivoagrabar = _ruta + "/" + _nombredearchivo + ".xml";

            string xmlstring = stringwriter.ToString().Replace("encoding=\"utf-16\"", "");


            EscribeArchivoXML(xmlstring, nombredearchivoagrabar, true);


            Cotizaciones cotizaciones = new Cotizaciones();

            cotizaciones.Descripcion = "Solicitud Suaje Digital " + elemento.Descripcion;
            cotizaciones.Fecha = DateTime.Now;
            cotizaciones.IDCliente = elemento.IDCliente;
            cotizaciones.IDClienteP = elemento.IDClienteP;
            cotizaciones.NombreArchivo = _nombredearchivo;
            cotizaciones.Solicitud = 1;
            cotizaciones.tipo = 1;
            cotizaciones.Usuario = User.Identity.Name;
            cotizaciones.suajenuevo = 0;
            cotizaciones.thermo = 0;
            cotizaciones.Ruta = _ruta;

            ArchivoCotizadorContext archivoc = new ArchivoCotizadorContext();
            archivoc.cotizaciones.Add(cotizaciones);
            archivoc.SaveChanges();

            SolicitudDiseno nueva = new SolicitudDiseno();
            nueva.TipoEtiqueta = "Digital";
            nueva.TipodeSolicitud = "Suaje";
            
            nueva.TipodeDiseno = "Suaje Digital";
            nueva.EstadodeSolicitud = "PENDIENTE";
            nueva.Observaciones = elemento.Observacion;
            nueva.NumeroRevision = 1;
            nueva.IDCotizacion = cotizaciones.ID;
            nueva.Fecha = DateTime.Now;
            nueva.FechaCompromiso = Convert.ToString(DateTime.Now.AddDays(2)).Replace("/", "-");
            nueva.IDVendedor = elemento.IDVendedor;
            nueva.Embobinado = "";


            if (elemento.IDCliente!=0)
            {
                try
                {
                    nueva.Cliente = new ClientesContext().Clientes.Find(elemento.IDCliente).Nombre;
                }
                catch
                {

                }
            }

            if (elemento.IDClienteP != 0)
            {
                try
                {
                    nueva.Cliente = new ClientesPContext().ClientesPs.Find(elemento.IDCliente).Nombre;
                }
                catch
                {

                }
            }

            string insertsolicitud = "insert into SolicitudDiseno(TipoEtiqueta,TipodeSolicitud,TipodeDiseno,EstadodeSolicitud,Observaciones,NumeroRevision," +
                "IDCotizacion,Fecha,FechaCompromiso,IDVendedor,Embobinado,Cliente) values ('"+nueva.TipoEtiqueta+"','"+nueva.TipodeSolicitud+"'," +
                " '"+nueva.TipodeDiseno+"','"+nueva.EstadodeSolicitud+"','"+nueva.Observaciones+"','"+nueva.NumeroRevision+"','"+nueva.IDCotizacion+"',sysdatetime(),'"+nueva.FechaCompromiso+"'," +
                "'"+nueva.IDVendedor+"','"+nueva.Embobinado+"','"+nueva.Cliente+"')";

            SolicitudDisenoContext dbd = new SolicitudDisenoContext();
            dbd.Database.ExecuteSqlCommand(insertsolicitud);
            //dbd.SolicitudDiseno.Add(nueva);
            //dbd.SaveChanges();

            List<SolicitudDiseno> numero;
            numero = new SolicitudDisenoContext().Database.SqlQuery<SolicitudDiseno>("SELECT * FROM [dbo].[SolicitudDiseno] WHERE ID = (SELECT MAX(ID) from SolicitudDiseno)").ToList();
            int IDSolicitud = numero.Select(s => s.ID).FirstOrDefault();

            try
            {
                //guardar cotización en carpeta
                ClsDigital elementoC;

                Cotizaciones archivocotizacion = new ArchivoCotizadorContext().Database.SqlQuery<Cotizaciones>("select*from Cotizaciones where id=" + nueva.IDCotizacion).FirstOrDefault();
                XmlSerializer serializerX = new XmlSerializer(typeof(ClsCotizador));
                try
                {
                    XmlDocument documento = new XmlDocument();
                    string nombredearchivo = archivocotizacion.Ruta.TrimEnd() + "\\" + archivocotizacion.NombreArchivo + ".xml";
                    documento.Load(nombredearchivo);


                    using (Stream reader = new FileStream(nombredearchivo, FileMode.Open))
                    {
                        // Call the Deserialize method to restore the object's state.
                        elementoC = (ClsDigital)serializerX.Deserialize(reader);
                    }
                }
                catch (Exception er)
                {
                    string mensajedeerror = er.Message;
                    elementoC = (ClsDigital)serializerX.Deserialize(StringExtensiones.ToXmlReader(archivocotizacion.contenido));
                }

                string NombredeArchivo = "Solicitud-" + IDSolicitud;

                string nombredecarpeta = DateTime.Now.Month + "" + DateTime.Now.Year + " " + User.Identity.Name;

                if (!Directory.Exists(System.Web.HttpContext.Current.Server.MapPath("~/DisenoCotizacion")))
                {
                    Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath("~/DisenoCotizacion"));
                }
                if (!Directory.Exists(System.Web.HttpContext.Current.Server.MapPath("~/DisenoCotizacion/SolicitudDiseno")))
                {
                    Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath("~/DisenoCotizacion/SolicitudDiseno"));
                }
                //if (!Directory.Exists(System.Web.HttpContext.Current.Server.MapPath("~/DisenoCotizacion/" + nombredecarpeta)))
                //{
                //    Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath("~/DisenoCotizacion/" + nombredecarpeta));
                //}


                nombredecarpeta = System.Web.HttpContext.Current.Server.MapPath("~/DisenoCotizacion/SolicitudDiseno");

                this.GrabarArchivoCotizador(elementoC, NombredeArchivo, nombredecarpeta);



            }
            catch (Exception err)
            {

            }

        }


        public void GrabarArchivoCotizadordiseno(ClsDisenoDigital elemento, string _nombredearchivo, string _ruta)
        {
            
            StringWriter stringwriter = new StringWriter();
            XmlSerializer x = new XmlSerializer(elemento.GetType());
            x.Serialize(stringwriter, elemento);
            string mensaje = stringwriter.ToString();



            Cotizaciones archivo = new Cotizaciones();


            string nombredearchivoagrabar = _ruta + "/" + _nombredearchivo + ".xml";

            string xmlstring = stringwriter.ToString().Replace("encoding=\"utf-16\"", "");


            EscribeArchivoXML(xmlstring, nombredearchivoagrabar, true);


            Cotizaciones cotizaciones = new Cotizaciones();

            cotizaciones.Descripcion = "Solicitud Diseno Digital " + elemento.Descripcion;
            cotizaciones.Fecha = DateTime.Now;
            cotizaciones.IDCliente = elemento.IDCliente;
            cotizaciones.IDClienteP = elemento.IDClienteP;
            cotizaciones.NombreArchivo = _nombredearchivo;
            cotizaciones.Solicitud = 1;
            cotizaciones.tipo = 2;
            cotizaciones.Usuario = User.Identity.Name;
            cotizaciones.suajenuevo = 0;
            cotizaciones.thermo = 0;
            cotizaciones.Ruta = _ruta;

            ArchivoCotizadorContext archivoc = new ArchivoCotizadorContext();
            archivoc.cotizaciones.Add(cotizaciones);
            archivoc.SaveChanges();

            SolicitudDiseno nueva = new SolicitudDiseno();
            nueva.TipoEtiqueta = "Digital";
            nueva.TipodeSolicitud = "Diseno";
          
            nueva.TipodeDiseno = "Diseno Digital";
            nueva.EstadodeSolicitud = "PENDIENTE";
            nueva.Observaciones = elemento.Observacion;
            nueva.NumeroRevision = 1;
            nueva.IDCotizacion = cotizaciones.ID;
            nueva.Fecha = DateTime.Now;
            nueva.FechaCompromiso = Convert.ToString(DateTime.Now.AddDays(2)).Replace("/", "-");
            nueva.IDVendedor = elemento.IDVendedor;
            nueva.Embobinado = "";
            nueva.FechaContrato = Convert.ToString(DateTime.Now.AddDays(2)).Replace("/", "-");

            if (elemento.IDCliente != 0)
            {
                try
                {
                    nueva.Cliente = new ClientesContext().Clientes.Find(elemento.IDCliente).Nombre;
                }
                catch
                {

                }
            }

            if (elemento.IDClienteP != 0)
            {
                try
                {
                    nueva.Cliente = new ClientesPContext().ClientesPs.Find(elemento.IDClienteP).Nombre;
                }
                catch
                {

                }
            }
            string insertsolicitud = "insert into SolicitudDiseno(TipoEtiqueta,TipodeSolicitud,TipodeDiseno,EstadodeSolicitud,Observaciones,NumeroRevision," +
                "IDCotizacion,Fecha,FechaCompromiso,IDVendedor,Embobinado,Cliente) values ('" + nueva.TipoEtiqueta + "','" + nueva.TipodeSolicitud + "'," +
                " '" + nueva.TipodeDiseno + "','" + nueva.EstadodeSolicitud + "','" + nueva.Observaciones + "','" + nueva.NumeroRevision + "','" + nueva.IDCotizacion + "',sysdatetime(),'" + nueva.FechaCompromiso + "'," +
                "'" + nueva.IDVendedor + "','" + nueva.Embobinado + "','" + nueva.Cliente + "')";

            SolicitudDisenoContext dbd = new SolicitudDisenoContext();
            dbd.Database.ExecuteSqlCommand(insertsolicitud);
            //SolicitudDisenoContext dbd = new SolicitudDisenoContext();
            //dbd.SolicitudDiseno.Add(nueva);
            //dbd.SaveChanges();

            List<SolicitudDiseno> numero;
            numero = new SolicitudDisenoContext().Database.SqlQuery<SolicitudDiseno>("SELECT * FROM [dbo].[SolicitudDiseno] WHERE ID = (SELECT MAX(ID) from SolicitudDiseno)").ToList();
            int IDSolicitud = numero.Select(s => s.ID).FirstOrDefault();

            try
            {
                //guardar cotización en carpeta
                ClsDisenoDigital elementoC;

                Cotizaciones archivocotizacion = new ArchivoCotizadorContext().Database.SqlQuery<Cotizaciones>("select * from Cotizaciones where id=" + nueva.IDCotizacion).FirstOrDefault();
                XmlSerializer serializerX = new XmlSerializer(typeof(ClsDisenoDigital));
                try
                {
                    XmlDocument documento = new XmlDocument();
                    string nombredearchivo = archivocotizacion.Ruta.TrimEnd() + "\\" + archivocotizacion.NombreArchivo + ".xml";
                    documento.Load(nombredearchivo);


                    using (Stream reader = new FileStream(nombredearchivo, FileMode.Open))
                    {
                        // Call the Deserialize method to restore the object's state.
                        elementoC = (ClsDisenoDigital)serializerX.Deserialize(reader);
                    }
                }
                catch (Exception er)
                {
                    string mensajedeerror = er.Message;
                    elementoC = (ClsDisenoDigital)serializerX.Deserialize(StringExtensiones.ToXmlReader(archivocotizacion.contenido));
                }

                string NombredeArchivo = "Solicitud-" + IDSolicitud;

                string nombredecarpeta = DateTime.Now.Month + "" + DateTime.Now.Year + " " + User.Identity.Name;

                if (!Directory.Exists(System.Web.HttpContext.Current.Server.MapPath("~/DisenoCotizacion")))
                {
                    Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath("~/DisenoCotizacion"));
                }
                if (!Directory.Exists(System.Web.HttpContext.Current.Server.MapPath("~/DisenoCotizacion/SolicitudDiseno")))
                {
                    Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath("~/DisenoCotizacion/SolicitudDiseno"));
                }
                //if (!Directory.Exists(System.Web.HttpContext.Current.Server.MapPath("~/DisenoCotizacion/" + nombredecarpeta)))
                //{
                //    Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath("~/DisenoCotizacion/" + nombredecarpeta));
                //}

                nombredecarpeta = System.Web.HttpContext.Current.Server.MapPath("~/DisenoCotizacion/SolicitudDiseno");

              //  this.GrabarArchivoCotizadordiseno(elementoC, NombredeArchivo, nombredecarpeta);


            }
            catch (Exception err)
            {

            }

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


        public ActionResult SolicitudDisenoDigital()
        {
            ClsDisenoDigital elemento = new ClsDisenoDigital();



            var Acabado = new List<SelectListItem>();
            Acabado.Add(new SelectListItem { Text = "Ninguno", Value = "Ninguno" });

            Acabado.Add(new SelectListItem { Text = "Laminado", Value = "Laminado" });
            Acabado.Add(new SelectListItem { Text = "Barniz", Value = "Barniz" });
            Acabado.Add(new SelectListItem { Text = "Laminado Brillante", Value = "Laminado Brillante" });
            Acabado.Add(new SelectListItem { Text = "Laminado Mate", Value = "Laminado Mate" });
            Acabado.Add(new SelectListItem { Text = "Barniz Brillante", Value = "Barniz Brillante" });
            Acabado.Add(new SelectListItem { Text = "Barniz Mate", Value = "Barniz Mate" });
            Acabado.Add(new SelectListItem { Text = "Foil", Value = "Foil" });
            Acabado.Add(new SelectListItem { Text = "Heliografico", Value = "Heliografico" });
            ViewBag.Acabado = new SelectList(Acabado, "Value", "Text");



            ViewData["Acabado"] = Acabado;


            var Embobinado = new List<SelectListItem>();
            Embobinado.Add(new SelectListItem { Text = "A", Value = "A" });
            Embobinado.Add(new SelectListItem { Text = "B", Value = "B" });
            Embobinado.Add(new SelectListItem { Text = "C", Value = "C" });
            Embobinado.Add(new SelectListItem { Text = "D", Value = "D" });
            Embobinado.Add(new SelectListItem { Text = "E", Value = "E" });
            Embobinado.Add(new SelectListItem { Text = "F", Value = "F" });
            Embobinado.Add(new SelectListItem { Text = "G", Value = "G" });
            Embobinado.Add(new SelectListItem { Text = "H", Value = "H" });
            Embobinado.Add(new SelectListItem { Text = "I", Value = "I" });
            Embobinado.Add(new SelectListItem { Text = "FAN FOLDER", Value = "FAN FOLDER" });
            Embobinado.Add(new SelectListItem { Text = "HOJA", Value = "HOJA" });
            ViewBag.Embobinado = new SelectList(Embobinado, "Value", "Text");



            ViewData["Embobinado"] = Embobinado;



            var Diseno = new List<SelectListItem>();
            Diseno.Add(new SelectListItem { Text = "Ninguno", Value = "Ninguno" });

            Diseno.Add(new SelectListItem { Text = "Muestra Envase", Value = "Muestra Envase" });
            Diseno.Add(new SelectListItem { Text = "Solo Archivo", Value = "Solo Archivo" });
            Diseno.Add(new SelectListItem { Text = "Diseño Nuevo", Value = "Diseño Nuevo" });
            ViewBag.Diseno = new SelectList(Diseno, "Value", "Text");



            ViewData["Diseno"] = Diseno;

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

            ViewBag.EsquinasSuaje = new SelectList(EsquinasSuaje, "Value", "Text");

            ViewData["Esquinas"] = EsquinasSuaje;

            var cintas = new Repository().GetCintas();
            ViewBag.IDMaterial = cintas;


            List<SelectListItem> vendedor = new List<SelectListItem>();

            vendedor.Add(new SelectListItem { Text = "-------------------------", Value = "0" });
            var todosvendedor = new VendedorContext().Vendedores.ToList().Where(s => s.Activo == true).OrderBy(s => s.Nombre);
            if (todosvendedor != null)
            {
                foreach (var y in todosvendedor)
                {
                    vendedor.Add(new SelectListItem { Text = y.Nombre, Value = y.IDVendedor.ToString() });
                }
            }

            ViewBag.IDVendedor = vendedor;


            ClientesContext clientes = new ClientesContext();
            ViewBag.IDCliente = new ClienteRepository().GetClientesNombre();
            ClientesPContext clientesp = new ClientesPContext();

            List<SelectListItem> ClientesPr = new List<SelectListItem>();
            List<ClientesP> cl = new ArticuloContext().Database.SqlQuery<ClientesP>("select * from ClientesP order by Nombre").ToList();
            ClientesPr.Add(new SelectListItem { Text = "-------------------------", Value = "0" });
            foreach (ClientesP y in cl)
                {
                   
                ClientesPr.Add(new SelectListItem { Text = y.Nombre, Value = y.IDClienteP.ToString() });
                }

            ViewBag.IDClienteP = ClientesPr;
            /*new SelectList(clientesp.ClientesPs, "IDClienteP", "Nombre");*/

            List<Articulo> arti = new ArticuloContext().Database.SqlQuery<Articulo>("select a.*  from articulo as a where (a.idfamilia= 11 or a.idfamilia= 69 or a.idfamilia= 71 or a.idfamilia= 72 or a.idfamilia= 75 or a.IDFamilia=76 or a.IDFamilia=77 or a.IDFamilia=79 or a.IDFamilia=80 or a.IDFamilia=81) and obsoleto='0' order by cref").ToList();

            List<SelectListItem> articulo = new List<SelectListItem>();
            articulo.Add(new SelectListItem { Text = "Suaje Nuevo", Value = "0" });

            if (arti != null)
            {
                foreach (Articulo y in arti)
                {
                    string des = y.Cref + " " + y.Descripcion;
                    articulo.Add(new SelectListItem { Text = des, Value = y.IDArticulo.ToString() });
                }
            }

            ViewBag.IDSuaje = articulo;


            return View(elemento);
        }

        [System.Web.Mvc.HttpPost]

        public ActionResult SolicitudDisenoDigital(ClsDisenoDigital elemento)
        {
            try
            {
                if (elemento.IDCliente==0 && elemento.IDClienteP==0)
                {
                    throw new Exception("No seleccionaste ni cliente ni porspecto, te suguiero que comienzes de nuevo con F5");
                }
                if (elemento.anchoproductomm == 0)
                {
                    throw new Exception("Tu solicitud debe tener una ancho en mm");
                }
                if (elemento.largoproductomm == 0)
                {
                    throw new Exception("Tu solicitud debe tener una largo en mm");
                }
                if (elemento.Cantidadxrollo == 0)
                {
                    throw new Exception("Tu solicitud debe tener el valor de Cantidad por rollo o paquete");
                }
                if (elemento.AlPaso == 0)
                {
                    throw new Exception("Tu solicitud cantidad de etiquetas al paso");
                }

              
                if (elemento.consumomensual == 0)
                {
                    throw new Exception("Indica la cantidad aproximada de etiquetas mensual");
                }
            }

            catch (Exception err)
            {
                ViewBag.Error = err.Message;



                var Acabado = new List<SelectListItem>();
                Acabado.Add(new SelectListItem { Text = "Ninguno", Value = "Ninguno" });

                Acabado.Add(new SelectListItem { Text = "Laminado", Value = "Laminado" });
                Acabado.Add(new SelectListItem { Text = "Barniz", Value = "Barniz" });
                Acabado.Add(new SelectListItem { Text = "Laminado Brillante", Value = "Laminado Brillante" });
                Acabado.Add(new SelectListItem { Text = "Laminado Mate", Value = "Laminado Mate" });
                Acabado.Add(new SelectListItem { Text = "Barniz Brillante", Value = "Barniz Brillante" });
                Acabado.Add(new SelectListItem { Text = "Barniz Mate", Value = "Barniz Mate" });
                Acabado.Add(new SelectListItem { Text = "Foil", Value = "Foil" });
                Acabado.Add(new SelectListItem { Text = "Heliografico", Value = "Heliografico" });
                ViewBag.Acabado = new SelectList(Acabado, "Value", "Text");



                ViewData["Acabado"] = Acabado;


                var Embobinado = new List<SelectListItem>();
                Embobinado.Add(new SelectListItem { Text = "A", Value = "A" });
                Embobinado.Add(new SelectListItem { Text = "B", Value = "B" });
                Embobinado.Add(new SelectListItem { Text = "C", Value = "C" });
                Embobinado.Add(new SelectListItem { Text = "D", Value = "D" });
                Embobinado.Add(new SelectListItem { Text = "E", Value = "E" });
                Embobinado.Add(new SelectListItem { Text = "F", Value = "F" });
                Embobinado.Add(new SelectListItem { Text = "G", Value = "G" });
                Embobinado.Add(new SelectListItem { Text = "H", Value = "H" });
                Embobinado.Add(new SelectListItem { Text = "I", Value = "I" });
                Embobinado.Add(new SelectListItem { Text = "FAN FOLDER", Value = "FAN FOLDER" });
                Embobinado.Add(new SelectListItem { Text = "HOJA", Value = "HOJA" });
                ViewBag.Embobinado = new SelectList(Embobinado, "Value", "Text");



                ViewData["Embobinado"] = Embobinado;



                var Diseno = new List<SelectListItem>();
                Diseno.Add(new SelectListItem { Text = "Ninguno", Value = "Ninguno" });

                Diseno.Add(new SelectListItem { Text = "Muestra Envase", Value = "Muestra Envase" });
                Diseno.Add(new SelectListItem { Text = "Solo Archivo", Value = "Solo Archivo" });
                Diseno.Add(new SelectListItem { Text = "Diseño Nuevo", Value = "Diseño Nuevo" });
                ViewBag.Diseno = new SelectList(Diseno, "Value", "Text");



                ViewData["Diseno"] = Diseno;

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

                ViewData["Esquinas"] = EsquinasSuaje;

                var cintas = new Repository().GetCintas();
                ViewBag.IDMaterial = cintas;


                List<SelectListItem> vendedor = new List<SelectListItem>();

                vendedor.Add(new SelectListItem { Text = "-------------------------", Value = "0" });
                var todosvendedor = new VendedorContext().Vendedores.ToList().Where(s => s.Activo == true).OrderBy(s => s.Nombre);
                if (todosvendedor != null)
                {
                    foreach (var y in todosvendedor)
                    {
                        vendedor.Add(new SelectListItem { Text = y.Nombre, Value = y.IDVendedor.ToString() });
                    }
                }

                ViewBag.IDVendedor = vendedor;


                ClientesContext clientes = new ClientesContext();
                ViewBag.IDCliente = new ClienteRepository().GetClientesNombre();
              
                ClientesPContext clientesp = new ClientesPContext();

                List<SelectListItem> ClientesPr = new List<SelectListItem>();
                List<ClientesP> cl = new ArticuloContext().Database.SqlQuery<ClientesP>("select * from ClientesP order by Nombre").ToList();
                ClientesPr.Add(new SelectListItem { Text = "-------------------------", Value = "0" });
                foreach (ClientesP y in cl)
                {

                    ClientesPr.Add(new SelectListItem { Text = y.Nombre, Value = y.IDClienteP.ToString() });
                }

                ViewBag.IDClienteP = ClientesPr;
                //ViewBag.IDClienteP = new SelectList(clientesp.ClientesPs, "IDClienteP", "Nombre");

                List<Articulo> arti = new ArticuloContext().Database.SqlQuery<Articulo>("select a.*  from articulo as a where (a.idfamilia= 11 or a.idfamilia= 69 or a.idfamilia= 71 or a.idfamilia= 72 or a.idfamilia= 75 or a.IDFamilia=76 or a.IDFamilia=77 or a.IDFamilia=79 or a.IDFamilia=80 or a.IDFamilia=81) and obsoleto='0' order by cref").ToList();

                List<SelectListItem> articulo = new List<SelectListItem>();
                articulo.Add(new SelectListItem { Text = "Suaje Nuevo", Value = "0" });

                if (arti != null)
                {
                    foreach (Articulo y in arti)
                    {
                        string des = y.Cref + " " + y.Descripcion;
                        articulo.Add(new SelectListItem { Text = des, Value = y.IDArticulo.ToString() });
                    }
                }

                ViewBag.IDSuaje = articulo;


           

                return View(elemento);
            }

            string NombredeArchivo = "SDD" + DateTime.Now.ToString().Replace(".", "").Replace(",", "").Replace(" ", "").Replace("/", "").Replace(":", "");

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


            this.GrabarArchivoCotizadordiseno(elemento, NombredeArchivo, nombredecarpeta);


            return RedirectToAction("Index", "Cotizador");


        }

     

        public JsonResult getSuaje(int IDSuaje)
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
            suajec.CavidadEje = 2;

            try
            {
                suajec.CavidadEje = int.Parse(formula.getvalor("REP EJE", cara.Presentacion).ToString());


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
    }
}