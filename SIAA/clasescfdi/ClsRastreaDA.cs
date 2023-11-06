using SIAAPI.Models.Cfdi;
using SIAAPI.Models.Comercial;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIAAPI.clasescfdi
{
    // clase para rastrear documento anterior
    public class ClsRastreaDA
    {
        public string Documento { get; set; }
        public int ID { get; set; }

        public string Nivel { get; set; }

        public List<NodoTrazo> getDocumentoAnterior(string _DocumentoActual, int _ID, string _Nivel)
        {

            List<NodoTrazo> nodos = new List<NodoTrazo>();

            switch (_DocumentoActual)
            {
                case "Devolucion":
                    {
                        switch (_Nivel)
                        {
                            case "Detalle":
                                {
                                    return nodos;
                                    break;
                                }
                            case "Encabezado":
                                {
                                    return nodos;
                                    break;
                                }
                        }
                        return nodos;
                        break;
                    }
                case "Factura":
                    {
                        switch (_Nivel)
                        {
                            case "Detalle":
                                {
                                    return nodos;
                                    break;
                                }
                            case "Encabezado":
                                {
                                    return nodos;
                                    break;
                                }
                        }
                        return nodos;
                        break;
                    }
                case "Prefactura":
                    {
                        switch (_Nivel)
                        {
                            case "Detalle":
                                {
                                    nodos = getPrefacturaDetalleAnterior(_ID);
                                    return nodos;

                                }
                            case "Encabezado":
                                {
                                    nodos = getPrefacturaEncabezadoAnterior(_ID);
                                    return nodos;

                                }
                        }
                        return nodos;

                    }
                case "Remision":
                    {
                        switch (_Nivel)
                        {
                            case "Detalle":
                                {
                                    return nodos;

                                }
                            case "Encabezado":
                                {
                                    nodos = getRemisionEncabezadoAnterior(_ID);
                                    return nodos;

                                }
                        }
                        return nodos;
                        break;
                    }
                case "Pedido":
                    {
                        switch (_Nivel)
                        {
                            case "Detalle":
                                {
                                    return nodos;
                                    break;
                                }
                            case "Encabezado":
                                {
                                    return nodos;
                                    break;
                                }
                        }
                        return nodos;
                        break;
                    }
                case "Requisicion":
                    {
                        switch (_Nivel)
                        {
                            case "Detalle":
                                {
                                    return nodos;
                                    break;
                                }
                            case "Encabezado":
                                {

                                    nodos = getRequisicionEncabezadoSiguiente(_ID);
                                    return nodos;

                                }
                        }
                        return nodos;
                        break;
                    }

                case "Recepcion":
                    {
                        switch (_Nivel)
                        {
                            case "Detalle":
                                {
                                    return nodos;
                                    break;
                                }
                            case "Encabezado":
                                {

                                    nodos = getRecepcionEncabezadoSiguiente(_ID);
                                    return nodos;

                                }
                        }
                        return nodos;
                        break;
                    }
                case "OCompra":
                    {
                        switch (_Nivel)
                        {
                            case "Detalle":
                                {
                                    return nodos;
                                    break;
                                }
                            case "Encabezado":
                                {

                                    nodos = getOCompraEncabezadoSiguiente(_ID);
                                    return nodos;

                                }
                        }
                        return nodos;
                        break;
                    }
                case "FacturaProv":
                    {
                        switch (_Nivel)
                        {
                            case "Detalle":
                                {
                                    return nodos;
                                    break;
                                }
                            case "Encabezado":
                                {

                                    nodos = getFacProvEncabezadoSiguiente(_ID);
                                    return nodos;

                                }
                        }
                        return nodos;
                        break;
                    }
                case "OCompraRequi":
                    {
                        switch (_Nivel)
                        {
                            case "Detalle":
                                {
                                    return nodos;
                                    break;
                                }
                            case "Encabezado":
                                {

                                    nodos = getOrdenCRequiDocSiguiente(_ID);
                                    return nodos;

                                }
                        }
                        return nodos;
                        break;
                    }

            }

            return nodos;

        }

        public List<NodoTrazo> getDocumentoSiguiente(string _DocumentoActual, int _ID, string _Nivel)
        {

            List<NodoTrazo> nodos = new List<NodoTrazo>();

            switch (_DocumentoActual)
            {
                case "Devolucion":
                    {
                        switch (_Nivel)
                        {
                            case "Detalle":
                                {
                                    return nodos;
                                    break;
                                }
                            case "Encabezado":
                                {
                                    return nodos;
                                    break;
                                }
                        }
                        return nodos;
                        break;
                    }
                case "Factura":
                    {
                        switch (_Nivel)
                        {
                            case "Detalle":
                                {
                                    return nodos;
                                    break;
                                }
                            case "Encabezado":
                                {
                                    return nodos;
                                    break;
                                }
                        }
                        return nodos;
                        break;
                    }
                case "Prefactura":
                    {
                        switch (_Nivel)
                        {

                            case "Encabezado":
                                {
                                    nodos = getPrefacturaEncabezadoSiguiente(_ID);
                                    return nodos;

                                }
                        }
                        return nodos;

                    }
                case "Remision":
                    {
                        switch (_Nivel)
                        {
                            case "Detalle":
                                {
                                    return nodos;
                                    break;
                                }
                            case "Encabezado":
                                {
                                    nodos = getRemisionEncabezadoSiguiente(_ID);
                                    return nodos;

                                }
                        }
                        return nodos;
                        break;
                    }
                case "Pedido":
                    {
                        switch (_Nivel)
                        {
                            case "Detalle":
                                {
                                    return nodos;
                                    break;
                                }
                            case "Encabezado":
                                {

                                    nodos = getPedidoEncabezadoSiguiente(_ID);
                                    return nodos;

                                }
                        }
                        return nodos;
                        break;
                    }
                case "PedidoP":
                    {
                        switch (_Nivel)
                        {
                            case "Detalle":
                                {
                                    return nodos;
                                    break;
                                }
                            case "Encabezado":
                                {

                                    nodos = getPedidoFacturaEncabezadoSiguiente(_ID);
                                    return nodos;

                                }
                        }
                        return nodos;
                        break;
                    }
                   
                case "Requisicion":
                    {
                        switch (_Nivel)
                        {
                            case "Detalle":
                                {
                                    return nodos;
                                    break;
                                }
                            case "Encabezado":
                                {

                                    nodos = getRequisicionEncabezadoSiguiente(_ID);
                                    return nodos;

                                }
                        }
                        return nodos;
                        break;
                    }

                case "Recepcion":
                    {
                        switch (_Nivel)
                        {
                            case "Detalle":
                                {
                                    return nodos;
                                    break;
                                }
                            case "Encabezado":
                                {

                                    nodos = getRecepcionEncabezadoSiguiente(_ID);
                                    return nodos;

                                }
                        }
                        return nodos;
                        break;
                    }
                case "OCompra":
                    {
                        switch (_Nivel)
                        {
                            case "Detalle":
                                {
                                    return nodos;
                                    break;
                                }
                            case "Encabezado":
                                {

                                    nodos = getOCompraEncabezadoSiguiente(_ID);
                                    return nodos;

                                }
                        }
                        return nodos;
                        break;
                    }

                case "FacturaProv":
                    {
                        switch (_Nivel)
                        {
                            case "Detalle":
                                {
                                    return nodos;
                                    break;
                                }
                            case "Encabezado":
                                {

                                    nodos = getFacProvEncabezadoSiguiente(_ID);
                                    return nodos;

                                }
                        }
                        return nodos;
                        break;
                    }
                case "OCompraRequi":
                    {
                        switch (_Nivel)
                        {
                            case "Detalle":
                                {
                                    return nodos;
                                    break;
                                }
                            case "Encabezado":
                                {

                                    nodos = getOrdenCRequiDocSiguiente(_ID);
                                    return nodos;

                                }
                        }
                        return nodos;
                        break;
                    }


            }

            return nodos;

        }

        private List<NodoTrazo> getOrdenCRequiDocSiguiente(int _ID)
        {
            Hashtable DA = new Hashtable();

            List<DetOrdenCompra> detalles = new PrefacturaContext().Database.SqlQuery<DetOrdenCompra>("Select * from detOrdenCompra where IDDetExterna=" + _ID + " and status!='Cancelado'").ToList();
            foreach (DetOrdenCompra detalle in detalles)
            {

                if (detalle.IDDetExterna > 0)
                {
                    try
                    {
                        if (DA.ContainsKey("OCompra " + detalle.IDOrdenCompra))
                        {
                            throw new Exception("ya tengo la OC");
                        }
                        NodoTrazo nuevo = new NodoTrazo();
                        nuevo.Documento = "OCompra ";
                        nuevo.ID = detalle.IDOrdenCompra;
                        nuevo.Descripcion = "OCompra " + detalle.IDOrdenCompra;
                        DA.Add("OCompra " + detalle.IDOrdenCompra, nuevo);
                    }
                    catch (Exception err)
                    {
                        string mensajedeerror = err.Message;
                    }
                }




            }

            List<NodoTrazo> nodos = new List<NodoTrazo>();
            foreach (string item in DA.Keys)
            {
                NodoTrazo oc = (NodoTrazo)DA[item];
                nodos.Add(oc);
            }

            return nodos;


        }



        private List<NodoTrazo> getFacProvEncabezadoSiguiente(int ID)
        {
            Hashtable DA = new Hashtable();

            List<EncRecepcion> detalles = new PrefacturaContext().Database.SqlQuery<EncRecepcion>("Select * from  [EncRecepcion] where idrecepcion=" + ID + " and status!='Cancelado'").ToList();
            foreach (EncRecepcion detalle in detalles)
            {

                if (detalle.DocumentoFactura != "")
                {
                    try
                    {
                        if (DA.ContainsKey("Factura " + detalle.DocumentoFactura))
                        {
                            throw new Exception("ya tengo la fac");
                        }
                        NodoTrazo nuevo = new NodoTrazo();
                        nuevo.Documento = "Factura ";
                        nuevo.ID = int.Parse(detalle.DocumentoFactura);
                        nuevo.Descripcion = "Factura " + detalle.DocumentoFactura;
                        DA.Add("Factura " + detalle.DocumentoFactura, nuevo);
                    }
                    catch (Exception err)
                    {
                        string mensajedeerror = err.Message;
                    }
                }
                //else 
                //{
                //    try
                //    {
                //        if (DA.ContainsKey("Factura " + detalle.DocumentoFactura))
                //        {
                //            throw new Exception("ya tengo la fac");
                //        }
                //        NodoTrazo nuevo = new NodoTrazo();
                //        nuevo.Documento = "Factura ";
                //        nuevo.ID = int.Parse(detalle.DocumentoFactura);
                //        nuevo.Descripcion = "Factura " + detalle.DocumentoFactura;
                //        DA.Add("Factura " + detalle.DocumentoFactura, nuevo);
                //    }
                //    catch (Exception err)
                //    {
                //        string mensajedeerror = err.Message;
                //    }
                //}




            }

            List<NodoTrazo> nodos = new List<NodoTrazo>();
            foreach (string item in DA.Keys)
            {
                NodoTrazo oc = (NodoTrazo)DA[item];
                nodos.Add(oc);
            }

            return nodos;
        }

        private List<NodoTrazo> getRemisionEncabezadoSiguiente(int _ID)
        {
            Hashtable DA = new Hashtable();

            List<DetRemision> detalles = new RemisionContext().Database.SqlQuery<DetRemision>("Select * from DetRemision as d inner join elementosprefactura as e on d.iddetremision=e.iddetdocumento inner join encprefactura as enc on enc.idprefactura=e.idprefactura  where e.documento='Remision' and e.iddocumento=" + _ID + " and enc.status!='Cancelado'").ToList();
            foreach (DetRemision detalle in detalles)
            {
                DetPrefactura detalleprefactura = new PrefacturaContext().DetPrefactura.ToList().Where(s => s.IDDetExterna == detalle.IDDetRemision && s.Proviene =="Remision").FirstOrDefault();


                if (detalleprefactura != null)
                {
                    try
                    {
                        if (DA.ContainsKey("Prefactura " + detalleprefactura.IDPrefactura))
                        {
                            throw new Exception("ya tengo la prefactura");
                        }
                        NodoTrazo nuevo = new NodoTrazo();
                        nuevo.Documento = "Prefactura ";
                        nuevo.ID = detalleprefactura.IDPrefactura;
                        nuevo.Descripcion = "Prefactura " + detalleprefactura.IDPrefactura;
                        DA.Add("Prefactura " + detalleprefactura.IDPrefactura, nuevo);
                    }
                    catch (Exception err)
                    {
                        string mensajedeerror = err.Message;
                    }
                }




            }

            List<NodoTrazo> nodos = new List<NodoTrazo>();
            foreach (string item in DA.Keys)
            {
                NodoTrazo oc = (NodoTrazo)DA[item];
                nodos.Add(oc);
            }

            return nodos;



        }

        private List<NodoTrazo> getPedidoEncabezadoSiguiente(int ID)
        {
            Hashtable DA = new Hashtable();

            List<DetPedido> detalles = new PrefacturaContext().Database.SqlQuery<DetPedido>("Select * from detpedido where IDpedido=" + ID + " and status!='Cancelado'").ToList();


            foreach (DetPedido detalle in detalles)
            {

                DetRemision detalleremsion = new PrefacturaContext().Database.SqlQuery<DetRemision>("Select * from detRemision where IDDetExterna=" + detalle.IDDetPedido + " and status!='Cancelado'").ToList().FirstOrDefault();

                if (detalleremsion != null)
                {
                    try
                    {
                        if (DA.ContainsKey("Remision " + detalleremsion.IDRemision))
                        {
                            throw new Exception("ya tengo la remision");
                        }
                        NodoTrazo nuevo = new NodoTrazo();
                        nuevo.Documento = "Remision ";
                        nuevo.ID = detalle.IDRemision;
                        nuevo.Descripcion = "Remision " + detalle.IDRemision;
                        DA.Add("Remision " + detalle.IDRemision, nuevo);
                    }
                    catch (Exception err)
                    {
                        string mensajedeerror = err.Message;
                    }


                    



                }

                List<EncPrefactura> detallesP = new PrefacturaContext().Database.SqlQuery<EncPrefactura>("Select ep.* from elementosprefactura as e inner join encprefactura as ep on e.idprefactura=ep.idprefactura where documento='Pedido' and e.iddocumento=" + ID + " and ep.status!='Cancelado'").ToList();

                if (detallesP != null)
                {
                    try
                    {
                        foreach (EncPrefactura detalleP in detallesP)
                        {
                            if (DA.ContainsKey("Prefactura " + detalleP.IDPrefactura))
                            {
                                throw new Exception("ya tengo la prefactura");
                            }
                            NodoTrazo nuevo = new NodoTrazo();
                            nuevo.Documento = "Prefactura ";
                            nuevo.ID = detalleP.IDPrefactura;
                            nuevo.Descripcion = "Prefactura " + detalleP.IDPrefactura; ;
                            DA.Add("Prefactura" + detalleP.IDPrefactura, nuevo);
                        }
                            
                    }
                    catch (Exception err)
                    {
                        string mensajedeerror = err.Message;
                    }
                }



            }

            List<NodoTrazo> nodos = new List<NodoTrazo>();
            foreach (string item in DA.Keys)
            {
                NodoTrazo oc = (NodoTrazo)DA[item];
                nodos.Add(oc);
            }

            return nodos;
        }

        private List<NodoTrazo> getPedidoFacturaEncabezadoSiguiente(int ID)
        {
            Hashtable DA = new Hashtable();

            List<DetPedido> detalles = new PrefacturaContext().Database.SqlQuery<DetPedido>("Select * from detpedido where IDpedido=" + ID + " and status!='Cancelado'").ToList();


            foreach (DetPedido detalle in detalles)
            {

                DetRemision detalleremsion = new PrefacturaContext().Database.SqlQuery<DetRemision>("Select * from detRemision where IDDetExterna=" + detalle.IDDetPedido + " and status!='Cancelado'").ToList().FirstOrDefault();

                if (detalleremsion != null)
                {
                    try
                    {
                        if (DA.ContainsKey("Remision " + detalleremsion.IDRemision))
                        {
                            throw new Exception("ya tengo la remision");
                        }

                        string CADENA = "Select * from DetRemision as d inner join elementosprefactura as e on d.iddetremision=e.iddetdocumento where e.documento='Remision' and e.iddocumento=" + detalleremsion.IDRemision + " and status!='Cancelado'";

                        List<DetRemision> detallesRemi = new RemisionContext().Database.SqlQuery<DetRemision>(CADENA).ToList();



                        foreach (DetRemision detalleR in detallesRemi)
                        {
                            DetPrefactura detalleprefactura = new PrefacturaContext().Database.SqlQuery<DetPrefactura>("Select * from DetPrefactura where IDDetExterna=" + detalleR.IDDetRemision + " and proviene='Remision'").ToList().FirstOrDefault();

                            //DetPrefactura detalleprefactura = new PrefacturaContext().DetPrefactura.ToList().Where(s => s.IDDetExterna == detalleR.IDDetRemision).FirstOrDefault();
                            if (detalleprefactura != null)
                            {

                                EncPrefactura detallesPrefa = new PrefacturaContext().Database.SqlQuery<EncPrefactura>("Select * from EncPrefactura where IDPrefactura=" + detalleprefactura.IDPrefactura + " and status!='Cancelado'").ToList().FirstOrDefault();
                                NodoTrazo nuevo = new NodoTrazo();
                                nuevo.Documento = "FACTURA";
                                nuevo.ID = detallesPrefa.IDFacturaDigital;
                                nuevo.Descripcion = "Factura " + " " + detallesPrefa.SerieDigital + " " + detallesPrefa.NumeroDigital;
                                List<NodoTrazo> lista = new List<NodoTrazo>();
                                lista.Add(nuevo);
                                return lista;
                            }

                        }




                    }
                    catch (Exception err)
                    {
                        string mensajedeerror = err.Message;
                    }






                }


                List<EncPrefactura> detallesP = new PrefacturaContext().Database.SqlQuery<EncPrefactura>("Select ep.* from elementosprefactura as e inner join encprefactura as ep on e.idprefactura=ep.idprefactura where documento='Pedido' and e.iddocumento=" + ID + " and ep.status!='Cancelado'").ToList();

                if (detallesP != null)
                {
                    try
                    {
                        foreach (EncPrefactura detalleP in detallesP)
                        {
                            if (DA.ContainsKey("Prefactura " + detalleP.IDPrefactura))
                            {
                                throw new Exception("ya tengo la prefactura");
                            }
                            //NodoTrazo nuevo = new NodoTrazo();
                            //nuevo.Documento = "Prefactura ";
                            //nuevo.ID = detalleP.IDPrefactura;
                            //nuevo.Descripcion = "Prefactura " + detalleP.IDPrefactura; ;
                            //DA.Add("Prefactura" + detalleP.IDPrefactura, nuevo);

                            EncPrefactura detallespP = new PrefacturaContext().Database.SqlQuery<EncPrefactura>("Select * from EncPrefactura where IDPrefactura=" + detalleP.IDPrefactura + " and status!='Cancelado'").ToList().FirstOrDefault();

                            NodoTrazo nuevo = new NodoTrazo();
                            nuevo.Documento = "FACTURA";
                            nuevo.ID = detallespP.IDFacturaDigital;
                            nuevo.Descripcion = "Factura " + " " + detallespP.SerieDigital + " " + detallespP.NumeroDigital;
                            List<NodoTrazo> lista = new List<NodoTrazo>();
                            lista.Add(nuevo);
                            return lista;


                        }

                    }
                    catch (Exception err)
                    {
                        string mensajedeerror = err.Message;
                    }
                }

            }

            List<NodoTrazo> nodos = new List<NodoTrazo>();
            foreach (string item in DA.Keys)
            {
                NodoTrazo oc = (NodoTrazo)DA[item];
                nodos.Add(oc);
            }

            return nodos;
        }

        private List<NodoTrazo> getRequisicionEncabezadoSiguiente(int ID)
        {
            Hashtable DA = new Hashtable();

            List<DetOrdenCompra> detalles = new PrefacturaContext().Database.SqlQuery<DetOrdenCompra>("Select * from detOrdenCompra where IDOrdenCompra=" + ID + " and status!='Cancelado'").ToList();
            foreach (DetOrdenCompra detalle in detalles)
            {

                if (detalle.IDDetExterna > 0)
                {
                    try
                    {
                        if (DA.ContainsKey("Requisicion " + detalle.IDDetExterna))
                        {
                            throw new Exception("ya tengo la requi");
                        }
                        NodoTrazo nuevo = new NodoTrazo();
                        nuevo.Documento = "Requisicion ";
                        nuevo.ID = detalle.IDDetExterna;
                        nuevo.Descripcion = "Requisicion " + detalle.IDDetExterna;
                        DA.Add("Requisición " + detalle.IDDetExterna, nuevo);
                    }
                    catch (Exception err)
                    {
                        string mensajedeerror = err.Message;
                    }
                }




            }

            List<NodoTrazo> nodos = new List<NodoTrazo>();
            foreach (string item in DA.Keys)
            {
                NodoTrazo oc = (NodoTrazo)DA[item];
                nodos.Add(oc);
            }

            return nodos;
        }

        private List<NodoTrazo> getRecepcionEncabezadoSiguiente(int ID)
        {
            Hashtable DA = new Hashtable();

            List<DetRecepcion> detalles = new PrefacturaContext().Database.SqlQuery<DetRecepcion>("Select * from detRecepcion where IDExterna=" + ID + " and status!='Cancelado'").ToList();
            foreach (DetRecepcion detalle in detalles)
            {

                if (detalle.IDExterna > 0)
                {
                    try
                    {
                        if (DA.ContainsKey("Recepcion" + detalle.IDRecepcion))
                        {
                            throw new Exception("ya tengo la Recepcion");
                        }
                        NodoTrazo nuevo = new NodoTrazo();
                        nuevo.Documento = "Recepcion ";
                        nuevo.ID = detalle.IDRecepcion;
                        nuevo.Descripcion = "Recepcion " + detalle.IDRecepcion;
                        DA.Add("Recepción " + detalle.IDRecepcion, nuevo);
                    }
                    catch (Exception err)
                    {
                        string mensajedeerror = err.Message;
                    }
                }




            }

            List<NodoTrazo> nodos = new List<NodoTrazo>();
            foreach (string item in DA.Keys)
            {
                NodoTrazo oc = (NodoTrazo)DA[item];
                nodos.Add(oc);
            }

            return nodos;
        }
        private List<NodoTrazo> getOCompraEncabezadoSiguiente(int ID)
        {
            Hashtable DA = new Hashtable();

            List<DetRecepcion> detalles = new PrefacturaContext().Database.SqlQuery<DetRecepcion>("Select * from detRecepcion where idrecepcion=" + ID + " and status!='Cancelado'").ToList();
            foreach (DetRecepcion detalle in detalles)
            {

                if (detalle.IDExterna > 0)
                {
                    try
                    {
                        if (DA.ContainsKey("OCompra" + detalle.IDExterna))
                        {
                            throw new Exception("ya tengo la oc");
                        }
                        NodoTrazo nuevo = new NodoTrazo();
                        nuevo.Documento = "OCompra ";
                        nuevo.ID = detalle.IDExterna;
                        nuevo.Descripcion = "OCompra " + detalle.IDExterna;
                        DA.Add("OCompra " + detalle.IDExterna, nuevo);
                    }
                    catch (Exception err)
                    {
                        string mensajedeerror = err.Message;
                    }
                }




            }

            List<NodoTrazo> nodos = new List<NodoTrazo>();
            foreach (string item in DA.Keys)
            {
                NodoTrazo oc = (NodoTrazo)DA[item];
                nodos.Add(oc);
            }

            return nodos;
        }
        private List<NodoTrazo> getPrefacturaEncabezadoSiguiente(int IDPrefactura)
        {


            EncPrefactura detalles = new PrefacturaContext().Database.SqlQuery<EncPrefactura>("Select * from EncPrefactura where IDPrefactura=" + IDPrefactura + " and status!='Cancelado'").ToList().FirstOrDefault();

            NodoTrazo nuevo = new NodoTrazo();
            nuevo.Documento = "FACTURA";
            nuevo.ID = detalles.IDFacturaDigital;
            nuevo.Descripcion = "Factura " + " " + detalles.SerieDigital + " " + detalles.NumeroDigital;
            List<NodoTrazo> lista = new List<NodoTrazo>();
            lista.Add(nuevo);
            return lista;

        }

        private List<NodoTrazo> getPrefacturaEncabezadoAnterior(int IDPrefactura)
        {
            Hashtable DA = new Hashtable();

            List<DetPrefactura> detalles = new PrefacturaContext().Database.SqlQuery<DetPrefactura>("Select * from detprefactura where IDPrefactura=" + IDPrefactura + " and status!='Cancelado'").ToList();
            foreach (DetPrefactura detalle in detalles)
            {
                if (detalle.Proviene == "Pedido")
                {
                    DetPedido detallepedido = new PedidoContext().DetPedido.Find(detalle.IDDetExterna);
                    EncPedido pedido = new PedidoContext().EncPedidos.Find(detallepedido.IDPedido);
                    try
                    {
                        if (DA.ContainsKey("Pedido " + pedido.IDPedido))
                        {
                            throw new Exception("ya tengo la orden de compara de ese pedido");
                        }
                        NodoTrazo nuevo = new NodoTrazo();
                        nuevo.Documento = "Pedido";
                        nuevo.ID = pedido.IDPedido;
                        nuevo.Descripcion = "Pedido " + pedido.IDPedido;
                        DA.Add("Pedido " + pedido.IDPedido, nuevo);
                    }
                    catch (Exception err)
                    {
                        string mensajedeerror = err.Message;
                    }


                }

                if (detalle.Proviene == "Remision")
                {
                    DetRemision detalleremision = new RemisionContext().DetRemisiones.Find(detalle.IDDetExterna);



                    try
                    {
                        if (DA.ContainsKey("Remision " + detalleremision.IDRemision))
                        {
                            throw new Exception("ya tengo la remision contemplada");
                        }
                        NodoTrazo nuevo = new NodoTrazo();
                        nuevo.Documento = "Remision";
                        nuevo.ID = detalleremision.IDRemision;
                        nuevo.Descripcion = "Remision " + detalleremision.IDRemision;
                        DA.Add("Remision " + detalleremision.IDRemision, nuevo);
                    }
                    catch (Exception err)
                    {
                        string mensajedeerror = err.Message;
                    }


                }
            }

            List<NodoTrazo> nodos = new List<NodoTrazo>();
            foreach (string item in DA.Keys)
            {
                NodoTrazo oc = (NodoTrazo)DA[item];
                nodos.Add(oc);
            }

            return nodos;


        }

        private List<NodoTrazo> getPrefacturaDetalleAnterior(int IDdetprefactura)
        {
            List<NodoTrazo> nodos = new List<NodoTrazo>();

            DetPrefactura detalle = new PrefacturaContext().Database.SqlQuery<DetPrefactura>("Select * from detprefactura where IDdetprefactura=" + IDdetprefactura + " and status!='Cancelado'").ToList().FirstOrDefault();

            if (detalle.Proviene == "Pedido")
            {
                DetPedido detallepedido = new PedidoContext().DetPedido.Find(detalle.IDDetExterna);
                EncPedido pedido = new PedidoContext().EncPedidos.Find(detallepedido.IDPedido);
                NodoTrazo nuevo = new NodoTrazo();
                nuevo.Documento = "Pedido";
                nuevo.ID = pedido.IDPedido;
                nuevo.Descripcion = "Pedido " + pedido.IDPedido;
                nodos.Add(nuevo);
                return nodos;


            }

            if (detalle.Proviene == "Remision")
            {
                DetRemision detalleremision = new RemisionContext().DetRemisiones.Find(detalle.IDDetExterna);

                NodoTrazo nuevo = new NodoTrazo();
                nuevo.Documento = "Remision";
                nuevo.ID = detalleremision.IDRemision;
                nuevo.Descripcion = "Remision " + detalleremision.IDRemision;



                nodos.Add(nuevo);
                return nodos;


            }


            return nodos;

        }


        private List<NodoTrazo> getPrefacturaDetalleSiguiente(int IDdetprefactura)
        {
            List<NodoTrazo> nodos = new List<NodoTrazo>();
            DetPrefactura detall = new PrefacturaContext().DetPrefactura.Find(IDdetprefactura);

            EncPrefactura detalles = new PrefacturaContext().Database.SqlQuery<EncPrefactura>("Select * from prefactura where IDPrefactura=" + detall.IDPrefactura + " and status!='Cancelado'").ToList().FirstOrDefault();

            NodoTrazo nuevo = new NodoTrazo();
            nuevo.Documento = "FACTURA";
            nuevo.ID = detalles.IDFacturaDigital;
            nuevo.Descripcion = "Factura " + detalles.SerieDigital + " " + detalles.NumeroDigital;

            nodos.Add(nuevo);
            return nodos;

        }

        /// <summary>
        /// Devuelve de que pedidos vienen sus detalles
        /// </summary>
        /// <param name="IDRemision"></param>
        /// <returns></returns>

        public List<NodoTrazo> getRemisionEncabezadoAnterior(int IDRemision)
        {
            Hashtable DA = new Hashtable();

            List<DetRemision> detalles = new PrefacturaContext().Database.SqlQuery<DetRemision>("Select * from DetRemision where IDRemision=" + IDRemision + " and status!='Cancelado'").ToList();
            foreach (DetRemision detalle in detalles)
            {
                if (detalle != null)
                {
                    if (detalle.IDDetExterna > 0)
                    {
                        DetPedido detallepedido = new PedidoContext().DetPedido.Find(detalle.IDDetExterna);
                        EncPedido pedido = new PedidoContext().EncPedidos.Find(detallepedido.IDPedido);
                        try
                        {
                            if (DA.ContainsKey("Pedido " + pedido.IDPedido))
                            {
                                throw new Exception("ya tengo la el pedido");
                            }
                            NodoTrazo nuevo = new NodoTrazo();
                            nuevo.Documento = "Pedido";
                            nuevo.ID = pedido.IDPedido;
                            nuevo.Descripcion = "Pedido " + pedido.IDPedido;
                            DA.Add("Pedido " + pedido.IDPedido, nuevo);
                        }
                        catch (Exception err)
                        {
                            string mensajedeerror = err.Message;
                        }
                    }
                }
            }



            List<NodoTrazo> nodos = new List<NodoTrazo>();
            foreach (string item in DA.Keys)
            {
                NodoTrazo oc = (NodoTrazo)DA[item];
                nodos.Add(oc);
            }

            return nodos;

        }
    }



    public class NodoTrazo
    {
        public string Documento { get; set; }
        public int ID { get; set; }

        public string Descripcion { get; set; }
    }
}