using SIAAPI.Models.Comercial;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SIAAPI.ClasesProduccion
{
    public class ClsDigital
    {
        public int IDCliente { get; set;}

        public int IDClienteP { get; set; }

        public int IDVendedor { get; set; }
        [DisplayName("Descripcion del archivo")]
        public string Descripcion { get; set; }

       
        public bool SuajeNuevo { get; set; }
      
        public string TipoSuajeFigura { get; set; }
        public string TipoCorte { get; set; }
        public string Esquinas { get; set; }


        [DisplayName("Dientes (TH)")]
        public decimal TH { get; set; }




        [DisplayName("Cantidad por paquete o rollo")]
        public int Cantidadxrollo { get; set; }


        [Required]
        public string Maquina { get; set; }
        [Required]
        public string Presentacion { get; set; }
        [Required]
        public string Metodo { get; set; }
        [Required]
        public string Acabado { get; set; }

        public string Embobinado { get; set; }


        [DisplayName("Eje del producto en mm")]
        [Required]
        public decimal anchoproductomm { get; set; }

        [DisplayName("Largo del producto en mm")]
        [Required]
        public decimal largoproductomm { get; set; }

        public decimal largoreal { get; set; }

        [DisplayName("Gap al Eje en mm")]
        [Required]
        public decimal gapeje { get; set; }

        [DisplayName("Gap al Avance en mm")]
        [Required]
        public decimal gapavance { get; set; }

        private int cavidad { get; set; }

        [DisplayName("Cavidades del suaje")]
        public int cavidadesdesuajeEje
        {
            get
            {
                if (cavidad == 0)
                { return 1; }

                else
                {
                    return cavidad;
                }
            }
            set
            {
                if ((value == 0))
                {
                    cavidad = 1;
                }
                else
                {
                    cavidad = value;
                }
            }
        }

        private int cavidadAvance { get; set; }

        [DisplayName("Cavidades Avance del suaje")]
        public int cavidadesdesuajeAvance
        {
            get
            {
                if (cavidadAvance == 0)
                { return 1; }

                else
                {
                    return cavidadAvance;
                }
            }
            set
            {
                if ((value == 0))
                {
                    cavidadAvance = 1;
                }
                else
                {
                    cavidadAvance = value;
                }
            }
        }


     
       


      


        public int IDMaterial { get; set; }
        public string Material { get; set; }

        public int IDMaquinaPrensa { get; set; }

   

        public int IDSuaje { get; set; }
        public int IDSuaje2 { get; set; }
      

        [DisplayName("Suaje en Existencia")]
        public int Suaje { get; set; }



        public int cavidades { get; set; }
   
        public bool Valido { get; set; }

        public int cavidadesdesuaje { get; set; }

        public decimal EjeMaquina { get; set; }

        public int IDCaracteristica { get; set; }

        public int IDCaracteristica2 { get; set; }

       public decimal CalibreMaterial { get; set; }

        public decimal CalibreRespaldo { get; set; }

        public string Observacion { get; set; }

        public decimal EjeReal { get; set; }

        public decimal AvanceReal { get; set; }

        public void calculadientes()
        {
            decimal mvarSobrante = ((EjeMaquina * 25.4M)) - (this.cavidadesdesuaje * (this.anchoproductomm + this.gapeje));
            if (this.cavidadesdesuajeAvance == 0)
            {
                cavidadesdesuajeAvance = 1;
            }

            decimal mvarCalculodientes = ((cavidadesdesuajeAvance * (largoproductomm + gapavance)) / 25.4M) / 0.125M;
            TH = int.Parse(mvarCalculodientes.ToString());
        }

    }



    public class ClsDisenoDigital
    {
        public int IDCliente { get; set; }

        public int IDClienteP { get; set; }

        public int IDVendedor { get; set; }
        [DisplayName("Descripcion del archivo")]
        public string Descripcion { get; set; }

        public int AlPaso { get; set; }


        public string Esquinas { get; set; }

        public string Embobinado { get; set; }



        [DisplayName("Cantidad por paquete o rollo")]
        public int Cantidadxrollo { get; set; }


        [Required]
        public string Acabado { get; set; }

        [Required]
        public string Diseno { get; set; }

        [DisplayName("Eje del producto en mm")]
        [Required]
        public decimal anchoproductomm { get; set; }

        [DisplayName("Largo del producto en mm")]
        [Required]
        public decimal largoproductomm { get; set; }

       
        
        public int IDMaterial { get; set; }

        public string Material { get; set; }
        public int IDSuaje { get; set; }
   


        public string Observacion { get; set; }

      
        public decimal consumomensual { get; set; }

        public decimal MontoAnticipo { get; set; }


    }
}