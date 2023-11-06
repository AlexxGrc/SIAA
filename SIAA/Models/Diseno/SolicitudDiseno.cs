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

namespace SIAAPI.Models.Diseno
{
    
    [Table("SolicitudDiseno")]
    public class SolicitudDiseno
    {
        [Key]
        [DisplayName("Número Diseño")]
        public int ID { get; set; }

        //[DisplayName("Descripcion")]
        //public string Descripcion { get; set; }


        [DisplayName("Número de Revisión")]
        public int NumeroRevision { get; set; }

        [DisplayName("no. Cotización")]
        public int IDCotizacion { get; set; }

        [DisplayName("Fecha")]
        public DateTime Fecha { get; set; }

        [DisplayName("Fecha Compromiso")]
        public string FechaCompromiso { get; set; }

        [DisplayName("Cliente")]
        public string Cliente { get; set; }
        //public virtual Clientes Cliente { get; set; }

        [DisplayName("Vendedor")]
        public int IDVendedor { get; set; }
        public virtual Vendedor Vendedor { get; set; }

        [DisplayName("Tipo de Solicitud")]
        public string TipodeSolicitud { get; set; }

        [DisplayName("Tipo de Etiqueta")]
        public string TipoEtiqueta { get; set; }

        public string TipodeDiseno { get; set; }
        public string Embobinado { get; set; }

        //[DisplayName("Tipo de Diseño")]
        //public string TipodeDiseno { get; set; }

        [DisplayName("Estado")]
        public string EstadodeSolicitud { get; set; }

        [DisplayName("Folio de Grabado")]
        public int FoliodeGrabado { get; set; }

        [DisplayName("Fecha de Contrato")]
        public string FechaContrato { get; set; }

        [DisplayName("Fecha de Entrega Grabado")]
        public string FechaEntGrabado { get; set; }

        [DisplayName("No. Grabados")]
        public int NumerodeGrabados { get; set; }

        [DisplayName("CM2")]
        public int Cm2 { get; set; }

        [DisplayName("No. Repeticiones")]
        public int NumeroRepeticiones { get; set; }

        [DisplayName("No. Cirel")]
        public int NumerodeCirel { get; set; }

        [DisplayName("Consumo Mensual")]
        public decimal consumomensual { get; set; }

        [DisplayName("Monto Anticipo")]
        public decimal MontoAnticipo { get; set; }

        [DisplayName("Observaciones")]
        public string Observaciones { get; set; }
        [DisplayName("Fecha de Autorización")]
        public string FechaAutorizacion { get; set; }
        [DisplayName("Fecha de Terminación")]
        public string FechaTerminacion { get; set; }


    }



    [Table("VSolicitudDiseno")]
    public class VSolicitudDiseno
    {
        [Key]
        [DisplayName("Número Diseño")]
        public int ID { get; set; }

        [DisplayName("Número de Revisión")]
        public int NumeroRevision { get; set; }

        [DisplayName("no. Cotización")]
        public int IDCotizacion { get; set; }

        [DisplayName("Fecha")]
        public DateTime Fecha { get; set; }

        [DisplayName("Fecha Compromiso")]
        public string FechaCompromiso { get; set; }

        [DisplayName("Cliente")]
        public string Cliente { get; set; }

        [DisplayName("Vendedor")]
        public string Vendedor { get; set; }
        [DisplayName("Oficina")]
        public string NombreOficina { get; set; }

        [DisplayName("Tipo de Solicitud")]
        public string TipodeSolicitud { get; set; }

        [DisplayName("Tipo de Etiqueta")]
        public string TipoEtiqueta { get; set; }

        public string TipodeDiseno { get; set; }

        [DisplayName("Estado")]
        public string EstadodeSolicitud { get; set; }

        [DisplayName("Folio de Grabado")]
        public int FoliodeGrabado { get; set; }

        [DisplayName("Fecha de Contrato")]
        public string FechaContrato { get; set; }

        [DisplayName("Fecha de Entrega Grabado")]
        public string FechaEntGrabado { get; set; }

        [DisplayName("No. Grabados")]
        public int NumerodeGrabados { get; set; }

        [DisplayName("CM2")]
        public int Cm2 { get; set; }

        [DisplayName("No. Repeticiones")]
        public int NumeroRepeticiones { get; set; }

        [DisplayName("No. Cirel")]
        public int NumerodeCirel { get; set; }

        [DisplayName("Consumo Mensual")]
        public decimal consumomensual { get; set; }

        [DisplayName("Monto Anticipo")]
        public decimal MontoAnticipo { get; set; }

        [DisplayName("Observaciones")]
        public string Observaciones { get; set; }

        [DisplayName("Fecha de Autorización")]
        public  string FechaAutorizacion { get; set; }
        [DisplayName("Fecha de Terminación")]
        public string FechaTerminacion{ get; set; }
    }
    public class VSolicitudDisenoContext : DbContext
    {
        public VSolicitudDisenoContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<VSolicitudDiseno> VSolicitudDiseno { get; set; }
    }

    public class EstadosolicitudRepository
    {
       public IEnumerable<SelectListItem> GetEstados()
        {
            List<SelectListItem> lista = new List<SelectListItem>();
            var pendiente = new SelectListItem()
            {
                Value = "PENDIENTE",
                Text = "PENDIENTE"
            };

            var iniciado = new SelectListItem()
            {
                Value = "INICIADO",
                Text = "INICIADO"
            };
            var terminado = new SelectListItem()
            {
                Value = "TERMINADO",
                Text = "TERMINADO"
            };
            var Cancelado =new SelectListItem()
            {
                Value = "CANCELADO",
                Text = "CANCELADO"
            };
            lista.Insert(0, Cancelado);
            lista.Insert(0, terminado);
            lista.Insert(0, iniciado);
            lista.Insert(0, pendiente);
            return new SelectList(lista, "Value", "Text");
        }

        public String GetEstadoBYID(string ID)
        {
            return ID;
        }


        public IEnumerable<SelectListItem> GetEstadosSeleccionado(string ID)
        {
            List<SelectListItem> lista = new List<SelectListItem>();


            var pendiente = new SelectListItem()
            {
                Value = "PENDIENTE",
                Text = "PENDIENTE"           
            };
            if (ID== "PENDIENTE")
            {
                pendiente.Selected = true;
            }


            var iniciado = new SelectListItem()
            {
                Value = "INICIADO",
                Text = "INICIADO"
            };

            if (ID == "INICIADO")
            {
                iniciado.Selected = true;
            }
            var terminado = new SelectListItem()
            {
                Value = "TERMINADO",
                Text = "TERMINADO"
            };
            if (ID == "TERMINADO")
            {
                terminado.Selected = true;
            }
            var Cancelado = new SelectListItem()
            {
                Value = "CANCELADO",
                Text = "CANCELADO"
            };
            if (ID == "CANCELADO")
            {
                Cancelado.Selected = true;
            }
            lista.Insert(0, Cancelado);
            lista.Insert(0, terminado);
            lista.Insert(0, iniciado);
            lista.Insert(0, pendiente);
            return new SelectList(lista, "Value", "Text");
        }
    }

      public class TipoEtiquetaRepository
    {
       public IEnumerable<SelectListItem> GetTipos(int suajenuevo)
        {
            List<SelectListItem> lista = new List<SelectListItem>();
            
            if (suajenuevo==1)
            {
                var pendiente = new SelectListItem()
                {
                    Value = "FLEXOGRAFIA",
                    Text = "FLEXOGRAFIA"
                };
                var terminado = new SelectListItem()
                {
                    Value = "DIGITAL",
                    Text = "DIGITAL"
                };


                lista.Insert(0, terminado);
                lista.Insert(0, pendiente);
               
            }
            else
            {
               
                var pendiente = new SelectListItem()
                {
                    Value = "FLEXOGRAFIA",
                    Text = "FLEXOGRAFIA"
                };

                var iniciado = new SelectListItem()
                {
                    Value = "TERMOENCOGIBLE",
                    Text = "TERMOENCOGIBLE"
                };
                var terminado = new SelectListItem()
                {
                    Value = "DIGITAL",
                    Text = "DIGITAL"
                };


                lista.Insert(0, terminado);
                lista.Insert(0, iniciado);
                lista.Insert(0, pendiente);
                
            }
            return new SelectList(lista, "Value", "Text");
        }

        public String GetEstadoBYID(string ID)
        {
            return ID;
        }


        public IEnumerable<SelectListItem> GetTipoSeleccionado(string ID)
        {
            List<SelectListItem> lista = new List<SelectListItem>();


         
            var pendiente = new SelectListItem()
            {
                Value = "FLEXOGRAFIA",
                Text = "FLEXOGRAFIA"
            };
            if (ID== "FLEXOGRAFIA")
            {
                pendiente.Selected = true;
            }


            var iniciado = new SelectListItem()
            {
                Value = "TERMOENCOGIBLE",
                Text = "TERMOENCOGIBLE"
            };

            if (ID == "TERMOENCOGIBLE")
            {
                iniciado.Selected = true;
            }
            var terminado = new SelectListItem()
            {
                Value = "DIGITAL",
                Text = "DIGITAL"
            };

            if (ID == "DIGITAL")
            {
                terminado.Selected = true;
            }
            
          
            lista.Insert(0, terminado);
            lista.Insert(0, iniciado);
            lista.Insert(0, pendiente);
            return new SelectList(lista, "Value", "Text");
        }
    }

    public class SolicitudDisenoContext : DbContext
    {
        public SolicitudDisenoContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<SolicitudDiseno> SolicitudDiseno { get; set; }
        public DbSet<Vendedor> Vendedores { get; set; }
        public DbSet<Clientes> Clientes { get; set; }

    }
    [Table("DisenoAdd")]
    public class DisenoAdd
    {
        [Key]
        public int ID { get; set; }
        [DisplayName("Diseño")]
        public int IDDiseno { get; set; }
        [DisplayName("RutaArchivo")]
        public string RutaArchivo { get; set; }
        [DisplayName("NombreArchivo")]
        public string nombreArchivo { get; set; }
    }
    public class DisenoAddContext : DbContext
    {
        public DisenoAddContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<DisenoAdd> DisenoAdd { get; set; }
    }

}
