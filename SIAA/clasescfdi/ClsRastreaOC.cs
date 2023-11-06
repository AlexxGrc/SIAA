using SIAAPI.Models.Comercial;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIAAPI.clasescfdi
{
    public class ClsRastreaOC
    {
        public int IDPrefactura;

        public ClsRastreaOC ( int _Prefactura)
        {
            IDPrefactura = _Prefactura;
        }

        public ClsRastreaOC()
        {
            
        }
        public string Rastreadefactura(int idfactura)

        {

            EncPrefactura prefactura = new PrefacturaContext().EncPrefactura.Where(s => s.IDFacturaDigital == idfactura).ToList().FirstOrDefault();
            string oc = string.Empty;
            IDPrefactura = prefactura.IDPrefactura;
            oc = Rastreadeprefactura();
            return oc;
        }

        public string Rastreadeprefactura ()
        {
            Hashtable OC = new Hashtable() ;
            List<DetPrefactura> detalles = new PrefacturaContext().Database.SqlQuery<DetPrefactura>("Select * from detprefactura where IDPrefactura="+ IDPrefactura).ToList();
            foreach (DetPrefactura detalle in detalles)
            {
               if ( detalle.Proviene =="Pedido")
                {
                    DetPedido detallepedido = new PedidoContext().DetPedido.Find(detalle.IDDetExterna);
                    EncPedido pedido = new PedidoContext().EncPedidos.Find(detallepedido.IDPedido);
                   try
                    {
                        if (OC.ContainsKey(pedido.IDPedido))
                        {
                            throw new Exception("ya tengo la orden de compara de ese pedido");
                        }
                        OC.Add(pedido.IDPedido, pedido.OCompra);
                    }
                    catch (Exception err)
                    {
                        string mensajedeerror = err.Message;
                    }
                    

                }

               if (detalle.Proviene=="Remision")
                {
                    DetRemision detalleremision = new RemisionContext().DetRemisiones.Find(detalle.IDDetExterna);
                    DetPedido detallepedido = new PedidoContext().DetPedido.Find(detalleremision.IDDetExterna);
                    EncPedido pedido = new PedidoContext().EncPedidos.Find(detallepedido.IDPedido);

                    try
                    {
                        if (OC.ContainsKey(pedido.IDPedido))
                        {
                            throw new Exception("ya tengo la orden de compara de ese pedido");
                        }
                        OC.Add(pedido.IDPedido, pedido.OCompra);
                    }
                    catch (Exception err)
                    {
                        string mensajedeerror = err.Message;
                    }


                }
            }

            string acuoc = string.Empty;
            foreach( int item in OC.Keys)
            {
                string oc = (string) OC[item];
                acuoc +=  oc +" ";
            }
            acuoc = acuoc.TrimEnd();
            acuoc = acuoc.TrimStart();
            return acuoc;
        }


        public string RastreadeRemision()
        {
            Hashtable OC = new Hashtable();
            List<DetRemision> detalles = new RemisionContext().Database.SqlQuery<DetRemision>("Select * from detremision where IDRemision=" + IDPrefactura).ToList();
            foreach (DetRemision detalle in detalles)
            {
               
                    DetPedido detallepedido = new PedidoContext().DetPedido.Find(detalle.IDDetExterna);
                    EncPedido pedido = new PedidoContext().EncPedidos.Find(detallepedido.IDPedido);
                    try
                    {
                        if (OC.ContainsKey(pedido.IDPedido))
                        {
                            throw new Exception("ya tengo la orden de compara de ese pedido");
                        }
                        OC.Add(pedido.IDPedido, pedido.OCompra);
                    }
                    catch (Exception err)
                    {
                        string mensajedeerror = err.Message;
                    }


          

                

                
            }

            string acuoc = string.Empty;
            foreach (int item in OC.Keys)
            {
                string oc = (string)OC[item];
                acuoc += oc + " ";
            }
            acuoc = acuoc.TrimEnd();
            acuoc = acuoc.TrimStart();
            return acuoc;
        }
		  
		  public string RastreaOC(int ID, int idcaracteristica, decimal cantidad)        {
            Hashtable DA = new Hashtable();

            string c = "Select  distinct(ep.idprefactura), dp.* from elementosprefactura as e inner join encprefactura as ep on e.idprefactura=ep.idprefactura inner join detprefactura as dp on dp.idprefactura=e.idprefactura where dp.caracteristica_id=" + idcaracteristica + " and ep.idprefactura=" + ID + " and  ep.status!='Cancelado'";
            List<DetPrefactura> detallesP = new PrefacturaContext().Database.SqlQuery<DetPrefactura>(c).ToList();

            if (detallesP != null)
            {
                try
                {
                    

                    foreach (DetPrefactura detalleP in detallesP)
                    {

                        string oc = string.Empty;
                        int iddetpedido = 0;

                        if(detalleP.Proviene == "Pedido")
                        {
                            if (detalleP.Cantidad== cantidad)
                            {
                                DetPedido detallepedido = new PedidoContext().DetPedido.Find(detalleP.IDDetExterna);
                                iddetpedido = detallepedido.IDDetPedido;
                            }
                            


                        }

                        if (detalleP.Proviene == "Remision")
                        {
                            if (detalleP.Cantidad== cantidad)
                            {
                                DetRemision detalleremision = new RemisionContext().DetRemisiones.Find(detalleP.IDDetExterna);

                                iddetpedido = detalleremision.IDExterna;
                            }
                            
                        }
                        int idpedido = 0;
                        try
                        {
                            EncPedido pedidos = new PedidoContext().EncPedidos.Find(iddetpedido);
                            oc = pedidos.OCompra;
                            idpedido = pedidos.IDPedido;
                        }
                        catch (Exception err)
                        {
                           
                        }
                        
                        if (DA.ContainsKey("O.C. " + oc))
                        {
                            throw new Exception("ya tengo la OC");
                        }
                        //NodoTrazo nuevo = new NodoTrazo();
                        //nuevo.Documento = "O.C.";
                        //nuevo.ID = idpedido;
                        //nuevo.Descripcion = "O.C." + oc;
                        //DA.Add("O.C." + oc, nuevo);
                        //if (DA.ContainsKey("O.C. " + oc))
                        //    {
                        //        throw new Exception("ya tengo el pedido");
                        //    }
                        try
                        {
                            DA.Add(idpedido, oc);
                        }
                        catch (Exception err)
                        {

                        }
                           
                       
                        



                    }

                }
                catch (Exception err)
                {
                    string mensajedeerror = err.Message;
                }
            }



            string acuoc = string.Empty;
            foreach (int item in DA.Keys)
            {
                string oc = (string)DA[item];
                acuoc += oc + " ";
            }
            acuoc = acuoc.TrimEnd();
            acuoc = acuoc.TrimStart();
            return acuoc;
        }
    }
}