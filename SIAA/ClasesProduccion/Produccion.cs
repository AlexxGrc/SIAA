using iTextSharp.text;
using iTextSharp.text.pdf;
using SIAAPI.Models.Produccion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace SIAAPI.ClasesProduccion
{
    public class PlaneaciondeProducccionPdf
    {
        public OrdenProduccion orden;

        public string nombredeldocumento = string.Empty;
        public void Generar(int numerodeorden)
        {
            nombredeldocumento = "";


            Document pdfDoc = new Document(PageSize.LETTER, 10f, 10f, 10f, 0f);
            PdfWriter.GetInstance(pdfDoc, HttpContext.Current.Response.OutputStream);
            pdfDoc.Open();
            pdfDoc.Add(new Paragraph("Orden de produccion"));


            pdfDoc.Add(new Paragraph("Hola mundo!"));


            pdfDoc.Close();
           

        }

        public void llenaencabezdo()
        {

        }

    }

    

    //public class PlaneacionProducccion
    //{
       
    //    public int IDPlaneacion { get; set; }
    //    public int IDModelo { get; set; }
    //    public virtual ModelosDeProduccion Modelo { get; set; }

    //    List<ProcesoProduccion> procesos = new List<ProcesoProduccion>();
    //    public string Descripcion { get; set; }
    //    public string Indicaciones { get; set; }
    //    public string detallesdeArticulo { get; set; }
       

    //    public int numero;

    //    public int Version;
    //}

    //public class ProcesoProduccion
    //{
    //    Proceso proceso; // lo jala del Modelo
    //    List<MaquinaPlaneacion> maquinas = null;
    //    List<ManodeobraPlaneacion> manodeobra = null;
    //    List<InsumoPlaneacion> insumos = null;
    //    List<HerramientasPlaneacion> herramientas = null;
    //    List<string> documentos = null;
    //    public bool subcontratar=false;
    //    public ProcesoProduccion(Proceso _proceso)
    //    {
    //        proceso = _proceso;
    //        if (proceso.UsaMaquina) { maquinas = new List<MaquinaPlaneacion>(); }
    //        if (proceso.UsaManoDeObra) { manodeobra = new List<ManodeobraPlaneacion>(); }
    //        if (proceso.UsaInsumos) { insumos = new List<InsumoPlaneacion>(); }
    //        if (proceso.UsaHerramientas) { herramientas = new List<HerramientasPlaneacion>(); }
    //        if (proceso.UsaDocumentos) { documentos = new List<string>(); }

    //    }


    //    public void GenererSubcontratarionPDF()
    //    {
    //        /// aqui generaria un pdf con una orden de producccion a maquilero
    //        /// 

    //    }

    //    public void GenererSubcontratarion(int _idproveedor)
    //    {
    //        /// 
    //        /// meteria una orden de compra al proveedor

    //    }



    //}
    //public class MaquinaPlaneacion
    //{

    //    public int IDArticulo;
    //    public int IDCaracteristica;
    //    public int IDProceso;
    //    public string Descripcion;
    //    public string Caracteristicas;
    //    public decimal HorasPlaneadas;
    //    public string Formuladerelacion = string.Empty;
    //    public decimal Cantidad = 0;
    //    public decimal factorcierre = 0;

    //    public decimal Costo = 0;
    //    public decimal PreciodeVenta = 1;

    //}

    //public class ManodeobraPlaneacion
    //{
    //    public int IDArticulo;
    //    public int IDCaracteristica;
    //    public int IDProceso;
    //    public string Descripcion;
    //    public string Caracteristicas;
    //    public decimal HorasPlaneadas;

    //    public string Formuladerelacion = string.Empty;
    //    public decimal Cantidad = 0;
    //    public decimal factorcierre = 0;

    //    public decimal Costo = 0;
       
        
     

        //public ManodeobraPlaneacion(bool _conbitacora = true)
        //{
        //    if (_conbitacora)
        //    {
        //        bitacora = new List<Bitacora>();
        //    }
        //}

        //public decimal GetHorastotalesReal()
        //{
        //    decimal ht = HorasReales;
        //    if (bitacora != null)
        //    {
        //      //  ht = bitacora.Sum(x => x.Diferenciaenhoras); ***************************
        //    }
        //    return ht;
        //}

        //public void VerificaCostoPlaneado()
        //{
        //    Costo = verificacosto(HorasPlaneadas);
        //}

        //public void VerificaCostoReal()
        //{
        //    Costo = verificacosto(GetHorastotalesReal());
        //}
        //public decimal verificacosto(decimal _horas)
        //{
        //    decimal rectifica = _horas;
        //    if ((Cerraralaproporcionde < 1) && (Cerraralaproporcionde > 0))
        //    {
        //        decimal diferenciadecimal = (rectifica - Math.Floor(rectifica));
        //        if (diferenciadecimal > 0)
        //        {
        //            bool salir = true;
        //            decimal rangoinferior = 0;
        //            decimal rangosuperior = Cerraralaproporcionde;
        //            while (salir)
        //            {
        //                if ((diferenciadecimal >= rangoinferior) && (diferenciadecimal <= rangosuperior))
        //                {
        //                    decimal difransup = rangosuperior - diferenciadecimal;
        //                    diferenciadecimal += difransup;
        //                    salir = false;
        //                }
        //                else
        //                {
        //                    rangoinferior = rangosuperior;
        //                    rangosuperior += Cerraralaproporcionde;
        //                }
        //            }
        //            rectifica = Math.Floor(rectifica) + diferenciadecimal;
        //        }
        //    }
        //    decimal costocalculado = rectifica * PreciodeVenta;
        //    return costocalculado;
        //}
          
        
    
    


    //public class InsumoPlaneacion
    //{
    //    public int IDArticulo;
    //    public int IDProceso;
    //    public string Descripcion;
    //    public string Caracteristicas;
    //    public decimal CantidadPlaneada;
    //    public decimal CantidadReal;
    //    public decimal ProporcionConLaCantidad = 1.0M;
    //    public decimal Cerraralaproporcionde = 0.25M;
    //    public decimal Costo = 0;
    //    public decimal PreciodeVenta = 1;
    //    public bool PorcionVacia = true;
 
       

    //    //public void VerificaCostoPlaneado()
    //    //{
    //    //    Costo = verificacosto(CantidadPlaneada);
    //    //}

    //    //public void VerificaCostoReal()
    //    //{
    //    //    Costo = verificacosto(CantidadReal);
    //    //}
    //    //public decimal verificacosto(decimal _horas)
    //    //{
    //    //    decimal rectifica = _horas;
    //    //    if ((Cerraralaproporcionde < 1) && (Cerraralaproporcionde > 0))
    //    //    {
    //    //        decimal diferenciadecimal = (rectifica - Math.Floor(rectifica));
    //    //        if (diferenciadecimal > 0)
    //    //        {
    //    //            bool salir = true;
    //    //            decimal rangoinferior = 0;
    //    //            decimal rangosuperior = Cerraralaproporcionde;
    //    //            while (salir)
    //    //            {
    //    //                if ((diferenciadecimal >= rangoinferior) && (diferenciadecimal <= rangosuperior))
    //    //                {
    //    //                    decimal difransup = rangosuperior - diferenciadecimal;
    //    //                    diferenciadecimal += difransup;
    //    //                    salir = false;
    //    //                }
    //    //                else
    //    //                {
    //    //                    rangoinferior = rangosuperior;
    //    //                    rangosuperior += Cerraralaproporcionde;
    //    //                }
    //    //            }
    //    //            rectifica = Math.Floor(rectifica) + diferenciadecimal;
    //    //        }
    //    //    }
    //    //    decimal costocalculado = rectifica * PreciodeVenta;
    //    //    return costocalculado;
    //    //}

    //}

    //public class HerramientasPlaneacion
    //{
    //      public int IDArticulo;
    //    public int IDProceso;
    //    public string Descripcion;
    //    public string Caracteristicas;
    //    public decimal CantidadPlaneada;
    //    public decimal CantidadReal;
    //    public decimal ProporcionConLaCantidad = 1.0M;
    //    public decimal Cerraralaproporcionde = 0.20M;
    //    public decimal Costo = 0;
    //    public decimal PreciodeVenta = 1;

    //    //public void VerificaCostoPlaneado()
    //    //{
    //    //    Costo = verificacosto(CantidadPlaneada);
    //    //}

    //    //public void VerificaCostoReal()
    //    //{
    //    //    Costo = verificacosto(CantidadReal);
    //    //}
    //    //public decimal verificacosto(decimal _horas)
    //    //{
    //    //    decimal rectifica = _horas;
    //    //    if ((Cerraralaproporcionde < 1) && (Cerraralaproporcionde > 0))
    //    //    {
    //    //        decimal diferenciadecimal = (rectifica - Math.Floor(rectifica));
    //    //        if (diferenciadecimal > 0)
    //    //        {
    //    //            bool salir = true;
    //    //            decimal rangoinferior = 0;
    //    //            decimal rangosuperior = Cerraralaproporcionde;
    //    //            while (salir)
    //    //            {
    //    //                if ((diferenciadecimal >= rangoinferior) && (diferenciadecimal <= rangosuperior))
    //    //                {
    //    //                    decimal difransup = rangosuperior - diferenciadecimal;
    //    //                    diferenciadecimal += difransup;
    //    //                    salir = false;
    //    //                }
    //    //                else
    //    //                {
    //    //                    rangoinferior = rangosuperior;
    //    //                    rangosuperior += Cerraralaproporcionde;
    //    //                }
    //    //            }
    //    //            rectifica = Math.Floor(rectifica) + diferenciadecimal;
    //    //        }
    //    //    }
    //    //    decimal costocalculado = rectifica * PreciodeVenta;
    //    //    return costocalculado;
    //    //}
    //}
    

}