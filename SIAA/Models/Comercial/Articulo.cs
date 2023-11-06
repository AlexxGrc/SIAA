using SIAAPI.Models.Administracion;
using SIAAPI.Models.Calidad;
using SIAAPI.Models.Produccion;
using SIAAPI.ViewModels.Articulo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SIAAPI.Models.Comercial
{
    [Table("Articulo")]
    public class Articulo
    {
        [Key]
        public int IDArticulo { get; set; }

        [DisplayName("Clave alfanumerica del producto")]
        [Required]
        public string Cref { get; set; }

        [DisplayName("Descripcion del producto")]
        [Required]
        public string Descripcion { get; set; }

        [DisplayName("Pertenece a la familia de productos")]

        public int IDFamilia { get; set; }
        public virtual Familia Familia { get; set; }

        [DisplayName("Tipo de Articulo")]
        [Required]

        public int IDTipoArticulo { get; set; }
        public virtual TipoArticulo TipoArticulo { get; set; }

        [DisplayName("Precio Unico de venta")]
        public bool Preciounico { get; set; }


        public decimal StockMin { get; set; }
        public decimal StockMax { get; set; }

        [DisplayName("Moneda de compra")]


        public int IDMoneda { get; set; }
        public virtual c_Moneda c_Moneda { get; set; }

        [DisplayName("Se controla inventario")]
        public bool CtrlStock { get; set; }

        [DisplayName("Manejo de mas de una caracteristica")]
        public bool ManejoCar { get; set; }

        [DisplayName("Unidad")]

        public int IDClaveUnidad { get; set; }
        public virtual c_ClaveUnidad c_ClaveUnidad { get; set; }

        //[DisplayName("Se maneja en")]
        //public int  { get; set; }



        [DisplayName("Maneja codigo de barras")]
        public bool bCodigodebarra { get; set; }

        [DisplayName("Codigo de  barras")]

        public string Codigodebarras { get; set; }


        [DisplayName("Observaciones de calidad")]
        public string Obscalidad { get; set; }

        [DisplayName("Existen devoluciones previas")]
        public bool ExistenDev { get; set; }

        [DisplayName("AQL")]
        public int IDAQL { get; set; }
        public virtual AQLCalidad AQLCalidad { get; set; }

        [DisplayName("No de inpeccion")]
        public int IDInspeccion { get; set; }
        public virtual Inspeccion Inspeccion { get; set; }

        [DisplayName("Muestreo")]

        public int IDMuestreo { get; set; }
        public virtual Muestreo Muestreo { get; set; }


        [DisplayName("Es kit")]
        public bool esKit { get; set; }


        [DisplayName("Genera Orden de produccion")]
        public bool GeneraOrden { get; set; }

        [DisplayName("Obsoleto")]
        public bool obsoleto { get; set; }

        public string nameFoto { get; set; }
        //public HttpPostedFileBase fileIMG { get; set; }
        public decimal MinimoVenta { get; set; }
        public decimal MinimoCompra { get; set; }

        public int IDCotizacion { get; set; }
        public virtual ICollection<MatrizPrecio> MatrizPrecio { get; set; }

        public virtual ICollection<Caracteristica> Caracteristica { get; set; }
        public virtual ICollection<Composicion> Composicion { get; set; }
        public virtual ICollection<Carrito> Carrito { get; set; }
        public virtual ICollection<c_TipoCuota> c_TipoCuota { get; set; }

        public virtual ICollection<OrdenProduccion> OrdenesdeProduccion { get; set; }
    }

    [Table("VArticuloRep")]
    public class VArticuloRep
    {
        [Key]
        public int IDArticulo { get; set; }
        public string Cref { get; set; }
        public string Articulo { get; set; }
        public string nameFoto { get; set; }
        public int IDCotizacion { get; set; }
        public string Codigodebarras { get; set; }
        public string Estado { get; set; }
        public decimal MinimoVenta { get; set; }
        public decimal MinimoCompra { get; set; }
        public decimal StockMin { get; set; }
        public decimal StockMax { get; set; }
        public string ClaveUnidad { get; set; }
        public string Unidad { get; set; }
        public string Moneda { get; set; }
        public int IDFamilia { get; set; }
        public string Familia { get; set; }
        public string TipoArticulo { get; set; }
        public string Preciounico { get; set; }
        public string CtrlStock { get; set; }
        public string Masdeunapresentacion { get; set; }
        public string esKit { get; set; }
        public string GeneraOP { get; set; }
        public string Obscalidad { get; set; }
        public string ExistenDevoluciones { get; set; }
        public int IDAQL { get; set; }
        public string Muestreo { get; set; }
        public string NivelInspeccion { get; set; }
    }
    public class VArticuloRepContext : DbContext
    {
        public VArticuloRepContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<VArticuloRep> VArticuloReps { get; set; }
    }

    [Table("VCaracteristicaRep")]
    public class VCaracteristicaRep
    {
        [Key]
        public int ID { get; set; }
        public int Articulo_IDArticulo { get; set; }
        public string cref { get; set; }
        public string Articulo { get; set; }
        public string Presentacion { get; set; }
        public string jsonPresentacion { get; set; }
        public int IDPresentacion { get; set; }
        public int IDCotizacion { get; set; }
        public int Cotizacion { get; set; }
        public int version { get; set; }
        public string Estado { get; set; }
        public decimal Existencia { get; set; }
        public decimal PorLlegar { get; set; }
        public decimal Apartado { get; set; }
        public decimal Disponibilidad { get; set; }
        public int IDHE { get; set; }
        public decimal StockMin { get; set; }
        public decimal StockMax { get; set; }
        public int IDFamilia { get; set; }
        public string Familia { get; set; }
    }
    public class VCaracteristicaRepContext : DbContext
    {
        public VCaracteristicaRepContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<VCaracteristicaRep> VCaracteristicaReps { get; set; }
    }

    [Table("VKitRep")]
    public class VKitRep
    {
        [Key]
        public int IDKit { get; set; }
        public int IDArticulo { get; set; }
        public virtual Articulo Articulo { get; set; }
        public string Cref { get; set; }
        public string Descripcion { get; set; }
        public int IDFamilia { get; set; }
        public string Familia { get; set; }
        public int IDArticuloComp { get; set; }
        public int IDCaracteristica { get; set; }
        public virtual Caracteristica Caracteristica { get; set; }
        public string Clave { get; set; }
        public decimal Cantidad { get; set; }
        public string composicion { get; set; }
        public int IDAlmacen { get; set; }
        public string Almacen { get; set; }
        public decimal Precio { get; set; }
        public string Preciounico { get; set; }
    }

    public class VKitRepContext : DbContext
    {
        public VKitRepContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<VKitRep> VKitReps { get; set; }

    }

    public class fam
    {
        [Key]
        public int idfamilia { get; set; }
    }
    public class mArticulo
    {
        public int IDArticulo { get; set; }
        public string Cref { get; set; }
        public string Descripcion { get; set; }
        public int IDFamilia { get; set; }
        public int IDTipoArticulo { get; set; }
        public bool Preciounico { get; set; }
        public int IDMoneda { get; set; }
        public bool CtrlStock { get; set; }
        public bool ManejoCar { get; set; }
        public int IDClaveUnidad { get; set; }

        public decimal StockMin { get; set; }
        public decimal StockMax { get; set; }
        public bool bCodigodebarra { get; set; }
        public string Codigodebarras { get; set; }
        public string Obscalidad { get; set; }
        public bool ExistenDev { get; set; }
        public int IDAQL { get; set; }
        public int IDInspeccion { get; set; }
        public int IDMuestreo { get; set; }
        public bool esKit { get; set; }
        public string nameFoto { get; set; }
        public HttpPostedFileBase fileIMG { get; set; }
        public bool GeneraOrden { get; set; }
        public bool obsoleto { get; set; }
        public decimal MinimoVenta { get; set; }
        public decimal MinimoCompra { get; set; }
    }

[Table("ArticulosGOPPlotter")]
    public class ArticulosGOPPlotter
    {
        [Key]
        public int IDPlotter { get; set; }
        public int IDArticulo { get; set; }
        public int IDPresentacion { get; set; }
    }

    [Table("Caracteristica")]
    public class Caracteristica
    {
        [Key]
        public int ID { get; set; }

        public int IDPresentacion { get; set; }

        public int Articulo_IDArticulo { get; set; }
        public virtual Articulo Articulo { get; set; }

        [DisplayName("No de Planeacion")]
        public int Cotizacion { get; set; }

        [DisplayName("Version de Planeacion")]
        public int version { get; set; }

        [DisplayName("Presentacion")]
        public string Presentacion { get; set; }

        public bool obsoleto { get; set; }

        public string jsonPresentacion { get; set; }

        public decimal Existencia { get; set; }
        public decimal PorLlegar { get; set; }
        public decimal Apartado { get; set; }
        public decimal Disponibilidad { get; set; }
        public decimal StockMin { get; set; }
        public decimal StockMax { get; set; }

        public int IDCotizacion { get; set; }

        public int IDHE { get; set; }

        public virtual ICollection<Carrito> Carrito { get; set; }
        

    }


    [Table("MatrizPrecio")]

    public class MatrizPrecio
    {
        [Key]
        public int idMatrizPrecio { get; set; }

        [DisplayName("De")]
        public decimal RangInf { get; set; }

        [DisplayName("A")]
        public decimal RangSup { get; set; }

        [DisplayName("Precio")]
        public decimal Precio { get; set; }

        [ForeignKey("Articulo")]
        public int IDArticulo { get; set; }
        public virtual Articulo Articulo { get; set; }

    }

    [Table("MatrizCosto")]
    public class MatrizCosto
    {
        [Key]
        public int idMatrizCosto { get; set; }


        [DisplayName("De")]
        public decimal RangInf { get; set; }

        [DisplayName("A")]
        public decimal RangSup { get; set; }

        [DisplayName("Precio")]
        public decimal Precio { get; set; }


        [ForeignKey("Articulo")]
        public int IDArticulo { get; set; }
        public virtual Articulo Articulo { get; set; }

    }

    [Table("Composicion")]

    public class Composicion
    {
        [Key]
        public int idComposicion { get; set; }

        //[ForeignKey("Caracteristica")]
        public int IDAtipicaProducto { get; set; }
        public Caracteristica Caracteristica { get; set; }

        [DisplayName("Factor de composicion ")]
        public decimal Factor { get; set; }

    }




    [Table("ArtProveedor")]
    public class ArtProveedor
    {
        [Key]
        public int IDArtProveedor { get; set; }

        //[ForeignKey("Caracteristica")]
        public int IDAtipicaProducto { get; set; }
        public Caracteristica Caracteristica { get; set; }

        [DisplayName("Codigo del Proveedor")]
        public string CodigoProveedor { get; set; }

        [DisplayName("Precio")]
        public decimal Precio { get; set; }

        [DisplayName("Moneda")]
        public int IDMoneda { get; set; }
        public virtual c_Moneda c_Moneda { get; set; }

    }



 
    [Table("TipoArticulo")]
    public class TipoArticulo
    {
        [Key]
        public int IDTipoArticulo { get; set; }
        [StringLength(50, ErrorMessage = "Has excedido el limite de 50 caracteres")]
        [DisplayName("Descripción")]
        public string Descripcion { get; set; }
        public virtual ICollection<Articulo> Articulo { get; set; }
    }
    public class TipoArticuloContext : DbContext
    {
        public TipoArticuloContext() : base("name=DefaultConnection")
        {
        }
        public DbSet<TipoArticulo> TipoArticulo { get; set; }
    }
    public class DatosArticulos
    {
        public List<TipoArticulo> TipoArticulos { get; set; }
        public List<Familia> Familias { get; set; }
        public List<VPArticulo> Articulos { get; set; }
    }

    public class FilterArticulos
    {
        public string texto { get; set; }
        public string cmbTArticulo { get; set; }
        public string cmbTFamilia { get; set; }
    }


    [Table("VMatrizPrecioProv")]

    public class VMatrizPrecioProv
    {
        [Key]
        public int IDMatrizPrecio { get; set; }
        [DisplayName("IDProveedor")]
        public int IDProveedor { get; set; }
        [DisplayName("Proveedor")]
        public string Empresa { get; set; }
        [DisplayName("Empresa")]
        public int IDArticulo { get; set; }
        [DisplayName("Artículo")]
        public string Descripcion { get; set; }
        [DisplayName("De")]
        public decimal RangInf { get; set; }

        [DisplayName("A")]
        public decimal RangSup { get; set; }

        [DisplayName("Precio")]
        public decimal Precio { get; set; }
        [DisplayName("IDMoneda")]
        public int IDMoneda { get; set; }
        [DisplayName("Divisa")]
        public string Moneda { get; set; }

        public string cref { get; set; }
        [DisplayName("Observacion")]
        public string Observacion { get; set; }

    }
    [Table("StockVSAlmacen")]
    public class StockVSAlmacen
    {
        [Key]
        public int ID { get; set; }
        public int IDCaracteristica { get; set; }
        public int IDAlmacen { get; set; }
        public decimal StockMin { get; set; }
        public decimal StockMax { get; set; }
    }
    public class StockVSAlmacenContext : DbContext
    {
        public StockVSAlmacenContext() : base("name=DefaultConnection")
        {
            Database.SetInitializer<StockVSAlmacenContext>(null);
        }
        public DbSet<StockVSAlmacen> StockVSAlmacens { get; set; }

    }
    public class VMatrizPrecioProvContext : DbContext
    {
        public VMatrizPrecioProvContext() : base("name=DefaultConnection")
        {
            Database.SetInitializer<VMatrizPrecioProvContext>(null);
        }
        public DbSet<VMatrizPrecioProv> VMPP { get; set; }

    }

    public class modelAddEditArticulo
    {
        //public Articulo Articulo { get; set; }
        public List<TipoArticulo> TipoArticulos { get; set; }
        public List<Familia> Familias { get; set; }
        public List<c_Moneda> Monedas { get; set; }
        public List<Inspeccion> Inspenccion { get; set; }
        public List<Muestreo> Muestreo { get; set; }
        public List<AQLCalidad> AQLCalidad { get; set; }
        public List<c_ClaveUnidad> c_ClaveUnidad { get; set; }
    }


    public class ArticuloContext : DbContext
    {
        public ArticuloContext() : base("name=DefaultConnection")
        {
            Database.SetInitializer<ArticuloContext>(null);
        }
        public DbSet<Articulo> Articulo { get; set; }
        public DbSet<Composicion> Composicion { get; set; }

        public DbSet<Caracteristica> Caracteristica { get; set; }
        public DbSet<MatrizPrecio> MatrizPrecio { get; set; }
        public DbSet<ArtProveedor> ArtProveedor { get; set; }

        public DbSet<Muestreo> Muestreo { get; set; }

        public DbSet<Inspeccion> Inspeccion { get; set; }
        public DbSet<MatrizCosto> MatrizCostos { get; set; }

        public DbSet<AQLCalidad> AQLCalidad { get; set; }
        public DbSet<TipoArticulo> TipoArticulo { get; set; }

    
        public DbSet<SIAAPI.Models.Administracion.c_ClaveUnidad> c_ClaveUnidad { get; set; }

        public DbSet<SIAAPI.Models.Administracion.c_Moneda> c_Moneda { get; set; }

        public DbSet<SIAAPI.Models.Comercial.Familia> Familias { get; set; }
    }

    public class ArticuloRepository
    {
        public int getFamilia(int IDArticulo)
        {
            ArticuloContext Ac = new ArticuloContext();
            Articulo elemento = Ac.Articulo.Single(m => m.IDArticulo == IDArticulo);
            return elemento.IDFamilia;
        }
        public IEnumerable<SelectListItem> GetCaracteristicaPorArticulosindefecto(int idarticulo)
        {
            List<SelectListItem> lista;
            if (idarticulo == 0)
            {
                lista = new List<SelectListItem>();
                lista.Add(new SelectListItem() { Value = "0", Text = "Elige un articulo primero" });
                return (lista);
            }
            using (var context = new ArticuloContext())
            {
                string cadenasql = "select * from Caracteristica where  Articulo_IDArticulo =" + idarticulo;
                lista = context.Database.SqlQuery<Caracteristica>(cadenasql).ToList()
                    .OrderBy(n => n.Presentacion)
                        .Select(n =>
                         new SelectListItem
                         {
                             Value = n.ID.ToString(),
                             Text = n.Presentacion
                         }).ToList();
                var countrytip = new SelectListItem()
                {
                    Value = null,
                    Text = "--- Selecciona una presentación ---"
                };
                lista.Insert(0, countrytip);
                return new SelectList(lista, "Value", "Text");
            }
        }
        public IEnumerable<SelectListItem> GetTipoArticulos()
        {
            using (var context = new ArticuloContext())
            {
                List<SelectListItem> lista = context.TipoArticulo.AsNoTracking()
                    .OrderBy(n => n.Descripcion)
                        .Select(n =>
                        new SelectListItem
                        {
                            Value = n.IDTipoArticulo.ToString(),
                            Text = n.Descripcion
                        }).ToList();
                var countrytip = new SelectListItem()
                {
                    Value = null,
                    Text = "--- Selecciona el tipo de articulos ---"
                };
                lista.Insert(0, countrytip);
                return new SelectList(lista, "Value", "Text");
            }
        }

        public IEnumerable<SelectListItem> GetCaracteristicaPorArticulo(int idarticulo, int idalmacen)
        {
            List<SelectListItem> lista;
            if (idarticulo == 0)
            {
                lista = new List<SelectListItem>();
                lista.Add(new SelectListItem() { Value = "0", Text = "Elige un articulo primero" });
                return (lista);
            }
            using (var context = new ArticuloContext())
            {
                string cadenasql = "select * from VInventarioAlmacen where  IDArticulo =" + idarticulo + " and IDAlmacen= " + idalmacen;

                List<VInventarioAlmacen> elementos = context.Database.SqlQuery<VInventarioAlmacen>(cadenasql).ToList();

                lista = new List<SelectListItem>();

                foreach (VInventarioAlmacen elemento in elementos)
                {
                    Caracteristica cara = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from caracteristica where id=" + elemento.IDCaracteristica).FirstOrDefault();

                    SelectListItem opcion =      new SelectListItem
                        {
                            Value = elemento.IDCaracteristica.ToString(),
                            Text = "Np" + cara.IDPresentacion +" "+ elemento.Presentacion + "| Existencia :" + elemento.Existencia + " Por llegar :" + elemento.PorLlegar + " Apartado :" + elemento.Apartado + " Disponibilidad:" + elemento.Disponibilidad
                        };
                    lista.Insert(0, opcion);
                }
                var countrytip = new SelectListItem()
                {
                    Value = null,
                    Text = "--- Selecciona una presentación ---"
                };
                lista.Insert(0, countrytip);
                return new SelectList(lista, "Value", "Text");
            }
        }

        public IEnumerable<SelectListItem> GetCaracteristicaPorArticulo(int idarticulo)
        {
            List<SelectListItem> lista;
            if (idarticulo == 0)
            {
                lista = new List<SelectListItem>();
               lista.Add(new SelectListItem() { Value = "0", Text = "Elige un articulo primero" });
                return (lista);
            }
            using (var context = new ArticuloContext())
            {
                string cadenasql = "select * from Caracteristica where  Articulo_IDArticulo =" + idarticulo +" order by IDPresentacion";
                lista = context.Database.SqlQuery<Caracteristica>(cadenasql).ToList()
                    .OrderBy(n => n.Presentacion)
                        .Select(n =>
                         new SelectListItem
                         {
                             Value = n.ID.ToString(),
                             Text = n.IDPresentacion+"->"+ n.Presentacion
                         }).ToList();
                var countrytip = new SelectListItem()
                {
                    Value = null,
                    Text = "--- Selecciona una presentación ---"
                };
                lista.Insert(0, countrytip);
                return new SelectList(lista, "Value", "Text");
            }
        }


        public IEnumerable<SelectListItem> GetArticuloFamilia(int idfamilia)
        {
            List<SelectListItem> lista;
            if (idfamilia == 0)
            {
                lista = new List<SelectListItem>();
                lista.Add(new SelectListItem() { Value = "0", Text = "Elige una familia primero" });
                return (lista);
            }
            using (var context = new ArticuloContext())
            {
                string cadenasql = "select * from Articulo where idfamilia=" + idfamilia +" order by descripcion";
                lista = context.Database.SqlQuery<Articulo>(cadenasql).ToList().OrderBy(s=>s.Descripcion)
                   
                        .Select(n =>
                         new SelectListItem
                         {
                             Value = n.IDArticulo.ToString(),
                             Text = n.Descripcion
                         }).ToList();
                var countrytip = new SelectListItem()
                {
                    Value = null,
                    Text = "--- Selecciona una familia ---"
                };
                lista.Insert(0, countrytip);
                return new SelectList(lista, "Value", "Text");
            }
        }

        public IEnumerable<SelectListItem> GetArticuloFamilia(int idfamilia, int seleccionado)
        {
            List<SelectListItem> lista =new List<SelectListItem>();
            
            using (var context = new ArticuloContext())
            {
                string cadenasql = "select * from Articulo where idfamilia=" + idfamilia + " order by descripcion";
                var lista2 = context.Database.SqlQuery<Articulo>(cadenasql).ToList().OrderBy(s => s.Descripcion);
               foreach (Articulo articulo in lista2)
                {
                    var countrytip2 = new SelectListItem()
                    {
                        Value = articulo.IDArticulo.ToString(),
                        Text = articulo.Descripcion
                    };
                    if (countrytip2.Value == seleccionado.ToString())
                    {
                        countrytip2.Selected = true;
                    }
                    lista.Insert(0,countrytip2);
                }
            
                return new SelectList(lista, "Value", "Text");
            }
        }

        public IEnumerable<SelectListItem> GetCaracteristicaPorArticuloconidpres(int idarticulo)
        {
            List<SelectListItem> lista;
            if (idarticulo == 0)
            {
                lista = new List<SelectListItem>();
                lista.Add(new SelectListItem() { Value = "0", Text = "Elige un articulo primero" });
                return (lista);
            }
            using (var context = new ArticuloContext())
            {
                string cadenasql = "select * from Caracteristica where  Articulo_IDArticulo =" + idarticulo;
                lista = context.Database.SqlQuery<Caracteristica>(cadenasql).ToList()
                    .OrderBy(n => n.Presentacion)
                        .Select(n =>
                         new SelectListItem
                         {
                             Value = n.ID.ToString(),
                             Text = n.IDPresentacion + "|" + n.Presentacion
                         }).ToList();
                //var countrytip = new SelectListItem()
                //{
                //    Value = null,
                //    Text = "--- Selecciona una presentación ---"
                //};
                //lista.Insert(0, countrytip);
                return new SelectList(lista, "Value", "Text");
            }
        }
    }

    [Table("MatrizPrecioProv")]

    public class MatrizPrecioProv
    {
        [Key]
        public int idMatrizPrecio { get; set; }
        [DisplayName("Proveedor")]
        public int IDProveedor { get; set; }
        public virtual Proveedor Proveedor { get; set; }

        [DisplayName("De")]
        public decimal RangInf { get; set; }

        [DisplayName("A")]
        public decimal RangSup { get; set; }

        [DisplayName("Precio")]
        public decimal Precio { get; set; }

        //[ForeignKey("Articulo")]
        public int IDArticulo { get; set; }
        // public virtual Articulo Articulos { get; set; }
        //[ForeignKey("Divisa")]
        public int IDMoneda { get; set; }
        // public virtual c_Moneda Monedas { get; set; }

        [DisplayName("Referencia Proveedor")]
        public string cref { get; set; }

        [DisplayName("Observacion")]
        public string Observacion { get; set; }

    }
    public class MatrizPrecioProvContext : DbContext
    {
        public MatrizPrecioProvContext() : base("name=DefaultConnection")
        {
            Database.SetInitializer<MatrizPrecioProvContext>(null);
        }
        //public DbSet<Articulo> Articulos { get; set; }
        public DbSet<MatrizPrecioProv> MatrizPP { get; set; }
        //public DbSet<Proveedor> Proveedores { get; set; }
    }

    [Table("ArtPresentacionAdd")]
    public class ArtPresentacionAdd
    {
        [Key]
        public int ID { get; set; }
        [System.ComponentModel.DisplayName("Pagos")]
        public int IDCaracteristica { get; set; }
        public int IDArticulo { get; set; }
        [DisplayName("RutaArchivo")]
        public string RutaArchivo { get; set; }
        [DisplayName("NombreArchivo")]
        public string nombreArchivo { get; set; }
    }
    public class ArtPresentacionAddContext : DbContext
    {
        public ArtPresentacionAddContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<ArtPresentacionAdd> ArtPresentacionAdd { get; set; }
    }

}