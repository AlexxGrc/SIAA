using SIAAPI.Models;
using SIAAPI.Models.Administracion;
using SIAAPI.Models.Cfdi;
using SIAAPI.Models.Comercial;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SIAAPI.Models.Comisiones
{
    [Table("ComisionVendedor")]
    public class CierreComisiones
    {
        [Key]
        public int IDComisionVendedor { get; set; }

        [Required(ErrorMessage = "Campo obligatorio")]
        [DisplayName("Fecha factura")]
        public DateTime FechaFac { get; set; }

        [Required(ErrorMessage = "Campo obligatorio")]
        [DisplayName("Fecha pedido")]
        public DateTime FechaPed { get; set; }

        [Required(ErrorMessage = "Campo obligatorio")]
        [DisplayName("Vendedor")]
        public int IDVendedor { get; set; }

        public virtual Vendedor Vendedor { get; set; }

        [Required(ErrorMessage = "Campo obligatorio")]
        [DisplayName("Cliente")]
        public int IDCliente { get; set; }

        [Required(ErrorMessage = "Campo obligatorio")]
        [DisplayName("Serie")]
        public string Serie { get; set; }

        [Required(ErrorMessage = "Campo obligatorio")]
        [DisplayName("Factura")]
        public int NoFactura { get; set; }

        [Required(ErrorMessage = "Campo obligatorio")]
        [DisplayName("Subtotal")]
        public decimal Subtotal { get; set; }

        [Required(ErrorMessage = "Campo obligatorio")]
        [DisplayName("Rentabilidad")]
        public decimal Rentabilidad { get; set; }

        [Required(ErrorMessage = "Campo obligatorio")]
        [DisplayName("Penalizaciones")]
        public decimal Penalizaciones { get; set; }

        [Required(ErrorMessage = "Campo obligatorio")]
        [DisplayName("Comisión")]
        public decimal Comision { get; set; }

        [Required(ErrorMessage = "Campo obligatorio")]
        [DisplayName("Tipo")]
        public string Tipo { get; set; }

        [Required(ErrorMessage = "Campo obligatorio")]
        [DisplayName("Pagada")]
        public bool Pagada { get; set; }

        [Required(ErrorMessage = "Campo obligatorio")]
        [DisplayName("Costo")]
        public decimal Costo { get; set; }

        [Required(ErrorMessage = "Campo obligatorio")]
        [DisplayName("Moneda factura")]
        public string MonedaFactura { get; set; }

        [Required(ErrorMessage = "Campo obligatorio")]
        [DisplayName("Moneda costo")]
        public string MonedaCosto { get; set; }

        [Required(ErrorMessage = "Campo obligatorio")]
        [DisplayName("Monto rentabilidad")]
        public decimal MontoRentabilidad { get; set; }

        [Required(ErrorMessage = "Campo obligatorio")]
        [DisplayName("Monto comisión")]
        public decimal MontoComision { get; set; }

        [Required(ErrorMessage = "Campo obligatorio")]
        [DisplayName("Numero pedido")]
        public string NoPedido { get; set; }

     
      

        [DisplayName("Mes de la comision")]
        public int MesCom { get; set; }
        public virtual ICollection<c_Meses> c_Meses { get; set; }
        [DisplayName("aÑO")]
        public int AnioCom { get; set; }

        [DisplayName("Base comisionable")]
        public decimal BaseComisionable { get; set; }
        [DisplayName("TC")]
        public decimal TC { get; set; }
        [DisplayName("ComisionPesos")]
        public decimal comisionpesos { get; set; }
        public bool Cierre { get; set; }

    }

    [Table("VComisionVendedor")]
    public class VComisionVendedor
    {
        [Key]
        public int IDComisionVendedor { get; set; }

        [DisplayName("Fecha factura")]
        public DateTime FechaFac { get; set; }

        [DisplayName("Fecha pedido")]
        public DateTime FechaPed { get; set; }

        [DisplayName("IDVendedor")]
        public int IDVendedor { get; set; }

        [DisplayName("Vendedor")]
        public string Vendedor { get; set; }

        [DisplayName("IDCliente")]
        public int IDCliente { get; set; }

        [DisplayName("Cliente")]
        public string Cliente { get; set; }

        [DisplayName("Serie")]
        public string Serie { get; set; }

        [DisplayName("Factura")]
        public int NoFactura { get; set; }

        [DisplayName("Subtotal")]
        public decimal Subtotal { get; set; }

        [DisplayName("Rentabilidad")]
        public decimal Rentabilidad { get; set; }

        [DisplayName("Penalizaciones")]
        public decimal Penalizaciones { get; set; }

        [DisplayName("Comisión")]
        public decimal Comision { get; set; }

        [DisplayName("Tipo")]
        public string Tipo { get; set; }

        [DisplayName("Pagada")]
        public bool Pagada { get; set; }

        [DisplayName("Costo")]
        public decimal Costo { get; set; }

        [DisplayName("Moneda factura")]
        public string MonedaFactura { get; set; }

        [DisplayName("Moneda costo")]
        public string MonedaCosto { get; set; }

        [DisplayName("Monto rentabilidad")]
        public decimal MontoRentabilidad { get; set; }

        [DisplayName("Monto comisión")]
        public decimal MontoComision { get; set; }

        [DisplayName("Numero pedido")]
        public string NoPedido { get; set; }
        [DisplayName("AniCom")]
        public int aniocom { get; set; }
        [DisplayName("MesCom")]
        public int mescom { get; set; }
        [DisplayName("MesComisión")]
        public string Mes { get; set; }
        [DisplayName("Base comisionable")]
        public decimal BaseComisionable { get; set; }

        [DisplayName("TC")]
        public decimal TC { get; set; }

        [DisplayName("TC DIA")]
        public decimal TCD { get; set; }

        [DisplayName("ComisionPesos")]
        public decimal comisionpesos { get; set; }
        [DisplayName("Condiciones Pago")]
        public string CondicionesPago { get; set; }
        [DisplayName("Dias Crédito")]
        public decimal diascredito { get; set; }

    }

    public class VComisionVendedorContext : DbContext
    {
        public VComisionVendedorContext() : base("name=DefaultConnection")
        {
            //Database.SetInitializer<CierreComisionesContext>(null);
        }
        public DbSet<VComisionVendedor> VComisionVendedores { get; set; }
    }



    public class CierreComisionesContext : DbContext
    {
        public CierreComisionesContext() : base("name=DefaultConnection")
        {
            //Database.SetInitializer<CierreComisionesContext>(null);
        }
        public DbSet<CierreVentas> CierreVentas { get; set; }
        public DbSet<CierreComisiones> CierreComisiones { get; set; }
        public DbSet<Vendedor> VendedorBD { get; set; }
        public DbSet<c_Meses> c_MesesBD { get; set; }
    }

    [Table("ComisionesPagos")]
    public class ComisionesPagos
    {
        [Key]
        public int IDComisionesP { get; set; }

       
        public string Mes { get; set; }

        public int Anno { get; set; }

        public int IDPago { get; set; }


       

    }
    [Table("DetComisionesPagos")]
    public class DetComisionesPagos
    {
        [Key]
        public int IDDetComisionesP { get; set; }


        public int IDComisionesP { get; set; }

        public int IDDFactura { get; set; }

        public int IDVendedor { get; set; }




    }

    public class PagoFacturaCom
    {
       

        public int IDPagoFactura { get; set; }




    }
}