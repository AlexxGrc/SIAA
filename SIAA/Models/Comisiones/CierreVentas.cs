using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using SIAAPI.Models.Comisiones;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Web.Mvc;
using SIAAPI.Models.Administracion;
using SIAAPI.Models.Comercial;
using SIAAPI.ViewModels.Comisiones;

namespace SIAAPI.Models.Comisiones
{
    [Table("CierreVentas")]
    public partial class CierreVentas
    {
        [Key]
        public int IDCierreVentas { get; set; }


        [Required(ErrorMessage = "Campo obligatorio")]
        [DisplayName("Mes")]
        public int IDMes { get; set; }

        [Required(ErrorMessage = "Campo obligatorio")]
        [DisplayName("Año")]
        public int Ano { get; set; }

        [Required(ErrorMessage = "Campo obligatorio")]
        [DisplayName("Vendedor")]
        public int IDVendedor { get; set; }

        [Required(ErrorMessage = "Campo obligatorio")]
        [DisplayName("Pedidos totales")]
        public int NumPedidos { get; set; }

        [Required(ErrorMessage = "Campo obligatorio")]
        [DisplayName("Ventas MXN")]
        public decimal VentasMXN { get; set; }

        [Required(ErrorMessage = "Campo obligatorio")]
        [DisplayName("Ventas USD")]
        public decimal VentasUSD { get; set; }

        [Required(ErrorMessage = "Campo obligatorio")]
        [DisplayName("Moneda de Cuota")]
        public int IDMonedaCA { get; set; }

        [Required(ErrorMessage = "Campo obligatorio")]
        [DisplayName("Cuota establecida")]
        public decimal Cuota { get; set; }

        [Required(ErrorMessage = "Campo obligatorio")]
        [DisplayName("Cuota alcanzada")]
        public decimal CuotaAlcanzada { get; set; }

        [Required(ErrorMessage = "Campo obligatorio")]
        [DisplayName("Comisión")]
        public decimal Comision { get; set; }

        public virtual ICollection<c_Meses> c_Meses { get; set; }
        public virtual ICollection<Vendedor> Vendedor { get; set; }
        public virtual ICollection<CierreVentas> CierreVentasIC { get; set; }

    }
    [Table("VCierreVentas")]
    public partial class VCierreVentas
    {
        [Key]
        public int IDCierreVentas { get; set; }

        [DisplayName("IDMes")]
        public int IDMes { get; set; }

        [DisplayName("Mes")]
        public string Mes { get; set; }

        [DisplayName("Año")]
        public int Año { get; set; }
        [DisplayName("IDVendedor")]
        public int IDVendedor { get; set; }

        [DisplayName("Vendedor")]
        public string Vendedor { get; set; }

        [DisplayName("Pedidos totales")]
        public int PedidosTotales { get; set; }

        [DisplayName("Ventas MXN")]
        public decimal VentasMXN { get; set; }

        [DisplayName("Ventas USD")]
        public decimal VentasUSD { get; set; }

        [Required(ErrorMessage = "Campo obligatorio")]

        [DisplayName("Cuota establecida")]
        public decimal CuotaEstablecida { get; set; }

        [DisplayName("Cuota alcanzada")]
        public decimal CuotaAlcanzada { get; set; }

        [DisplayName("Moneda")]
        public int IDMonedaCA { get; set; }

        [DisplayName("Moneda de Cuota")]
        public string Moneda { get; set; }

        [Required(ErrorMessage = "Campo obligatorio")]
        [DisplayName("Comisión")]
        public decimal Comision { get; set; }

    }


    public class VCierreVentasContext : DbContext
    {
        public VCierreVentasContext() : base("name=DefaultConnection")
        {
            //Database.SetInitializer<CierreVentasContext>(null);
        }
        public DbSet<CierreVentas> VCierreVentas { get; set; }
        public DbSet<c_Meses> c_Meses { get; set; }
    }
    public class CierreVentasContext : DbContext
    {
        public CierreVentasContext() : base("name=DefaultConnection")
        {
            //Database.SetInitializer<CierreVentasContext>(null);
        }
        public DbSet<CierreVentas> CierreVentas { get; set; }
        public DbSet<Vendedor> VendedorBD { get; set; }
        public DbSet<c_Meses> c_MesesBD { get; set; }
        //public DbSet<VPagoComi> VPagoComi { get; set; }
    }

    //public class CierreVentasRepository
    //{

    //    public IEnumerable<SelectListItem> GetCierreVenta()

    //    {
    //        using (var context = new CierreVentasContext())
    //        {

    //            List<SelectListItem> lista = context.VendedorBD.AsNoTracking()
    //                                   .OrderBy(n => n.IDVendedor)

    //                                       .Select(n => new SelectListItem
    //                                       {

    //                                           Value = n.IDVendedor.ToString(),
    //                                           Text = n.Nombre
    //                                       }).ToList();



    //            var countrytip = new SelectListItem()
    //            {

    //                Value = null,
    //                Text = "--- Selecciona un vendedor---"
    //            };

    //            lista.Insert(0, countrytip);
    //            return new SelectList(lista, "Value", "Text");



    //        }


    //    }
    //}


}
