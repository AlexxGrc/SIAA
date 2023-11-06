using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SIAAPI.Models.Comercial
{
    [Table("InventarioAlmacen")]
    public class InventarioAlmacen
    {
        [Key]
        public int IDInventarioAlmacen { get; set; }
        public int IDAlmacen { get; set; }

       
        public virtual Almacen Almacen { get; set; }

        public int IDArticulo { get; set; }
        public int IDCaracteristica { get; set; }
        public decimal Existencia { get; set; }
        public decimal PorLlegar { get; set; }
        public decimal Apartado { get; set; }
        public decimal Disponibilidad { get; set; }
    }

    public class InventarioAlmacenContext : DbContext
    {

        public InventarioAlmacenContext() : base("name=DefaultConnection")
        {
            Database.SetInitializer<InventarioAlmacenContext>(null);
        }
        public DbSet<InventarioAlmacen> InventarioAlmacenes { get; set; }
        public DbSet<VInventarioAlmacen> VInventarioAlmacenes { get; set; }
        public DbSet<Almacen> Almacenes { get; set; }
        public DbSet<Caracteristica> Caracteristica { get; set; }
        public DbSet<MaterialAsignado> MaterialAsignado { get; set; }

        public DbSet<inventariompxcb> inventariompxcbs { get; set; }
        public DbSet<inventariotintaxcb> inventariotintaxcbs { get; set; }
    }

    [Table("VInventarioAlmacen")]
    public class VInventarioAlmacen
    {
        [Key]
        public int IDInventarioAlmacen { get; set; }
        public int IDAlmacen { get; set; }
        public string Almacen { get; set; }
        public int IDArticulo { get; set; }
        public int IDCaracteristica { get; set; }
        public string Articulo { get; set; }
        public string Presentacion { get; set; }
        public string Cref { get; set; }
        public decimal Existencia { get; set; }
        public decimal PorLlegar { get; set; }
        public decimal Apartado { get; set; }
        public decimal Disponibilidad { get; set; }
    }


    [Table("VStockvsAlmacen")]
    public class VStockvsAlmacen
    {
        
       
        public string CodAlm { get; set; }
        public int IDArticulo { get; set; }
        public int IDFamilia { get; set; }
        public int IDCaracteristica { get; set; }
        public decimal StockMin { get; set; }
        public decimal StockMax { get; set; }
        public string Presentacion { get; set; }
        public string Cref { get; set; }
       
       
        public decimal Existencia { get; set; }
        public decimal PorLlegar { get; set; }
        public decimal Apartado { get; set; }
        public decimal Disponibilidad { get; set; }
        public int IDAlmacen { get; set; }
    }


    [Table("SeguimientoStock")]
    public class SeguimientoStock
    {

        [Key]
        public int IDSeguimiento { get; set; }
        public int IDOrden { get; set; }
        public DateTime Fecha { get; set; }
        public DateTime FechaCompromiso { get; set; }
        public int IDAlmacen { get; set; }
        public int IDArticulo { get; set; }
        public string Presentacion { get; set; }
        public int IDCaracteristica { get; set; }
        public decimal Cantidad { get; set; }
        public string Status { get; set; }

    
    }
    [Table("VSeguimientoStock")]
    public class VSeguimientoStock
    {


        public int IDSeguimiento { get; set; }
        public int IDOrden { get; set; }
        public DateTime Fecha { get; set; }
        public DateTime FechaCompromiso { get; set; }
        public string Almacen { get; set; }
        public string Clave { get; set; }
        public string Presentacion { get; set; }
        public int IDPresentacion { get; set; }
        public decimal Cantidad { get; set; }
        public string Status { get; set; }


    }
    public class SeguimientoStockContext : DbContext
    {

        public SeguimientoStockContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<SeguimientoStock> seguimientoStocks { get; set; }
        public DbSet<VSeguimientoStock> vseguimientoStocks { get; set; }

    }


    [Table("MaterialAsignado")]
    public class MaterialAsignado
    {
        [Key]
        public int IDMaterialAsignado { get; set; }
        public int orden { get; set; }

        public int idmapri { get; set; }
        public decimal ancho { get; set; }
        public decimal largo { get; set; }
        public decimal cantidad { get; set; }
        public string lote { get; set; }
        public string activo { get; set; }
        public decimal entregado { get; set; }
        public int idcaracteristica { get; set; }

        public int idalmacen { get;  set; }
    }


    public class VInventarioAlmacenContext : DbContext
    {

        public VInventarioAlmacenContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<VInventarioAlmacen> VInventarioAlmacenes { get; set; }

    }

    [Table("inventariompxcb")]
    public class inventariompxcb
    {
        [Key]

        public int id { get; set; }
        public string codigo { get; set; }
        public string clave { get; set; }


        public int ancho { get; set; }

        public int largo { get; set; }

        public decimal m2 { get; set; }

        public int cinta { get; set; }
        public bool Existencia { get; set; }

        public int IDAlmacen { get; set; }
    }

    [Table("inventariotintaxcb")]
    public class inventariotintaxcb
    {
        [Key]

        public int id { get; set; }
        public string Lote { get; set; }
        public string clave { get; set; }


        public int consecutivo { get; set; }

       
        public decimal cantidad { get; set; }

        public int IDDetRecepcion { get; set; }
        public bool Existencia { get; set; }

        public int IDAlmacen { get; set; }
        public string Estado { get; set; }
    }



    public class VInventarioAlmacenRepository
    {

        public IEnumerable<SelectListItem> GetArticuloxalmacen(int? idalamacen)
        {
            List<SelectListItem> lista;
            if (idalamacen == 0)
            {
                lista = new List<SelectListItem>();
                lista.Add(new SelectListItem() { Value = "0", Text = "Elige un almacen primero" });
                return (lista);
            }
            using (var context = new ArticuloContext())
            {
                string cadenasql = "Select distinct a.* from articulo as a inner join inventarioalmacen as i on a.idarticulo=i.idarticulo where i.existencia>0 and i.idalmacen= "+ idalamacen+"order by cref";
                lista = context.Database.SqlQuery<Articulo>(cadenasql).ToList()

                    .OrderBy(n => n.Cref)
                        .Select(n =>
                        new SelectListItem
                        {
                            Value = n.IDArticulo.ToString(),
                            Text = n.Cref + " | " + n.Descripcion
                        }).ToList();
                var countrytip = new SelectListItem()
                {
                    Value = "0",
                    Text = "--- Selecciona un Articulo---"
                };
                lista.Insert(0, countrytip);
                return new SelectList(lista, "Value", "Text");
            }

        }


        public IEnumerable<SelectListItem> GetArticuloxalmacentodos(int? idalamacen)
        {
            List<SelectListItem> lista;
            if (idalamacen == 0)
            {
                lista = new List<SelectListItem>();
                lista.Add(new SelectListItem() { Value = "0", Text = "Elige un almacen primero" });
                return (lista);
            }
            using (var context = new VInventarioAlmacenContext())
            {
                string cadenasql = "Select * from dbo.GetProductoPorAlmacen(" + idalamacen + ") ";
                lista = context.Database.SqlQuery<VInventarioAlmacen>(cadenasql).ToList()

                    .OrderBy(n => n.Articulo)
                        .Select(n =>
                        new SelectListItem
                        {
                            Value = n.IDArticulo.ToString(),
                            Text = n.Cref + " | " + n.Articulo
                        }).ToList();
                var countrytip = new SelectListItem()
                {
                    Value = "0",
                    Text = "--- Selecciona un Articulo---"
                };
                lista.Insert(0, countrytip);
                return new SelectList(lista, "Value", "Text");
            }

        }






        public List<SelectListItem> GetListArticuloxalmacen(int? idalamacen)
        {
            using (var context = new VInventarioAlmacenContext())
            {
                string cadenasql = "Select * from dbo.GetProductoPorAlmacen(" + idalamacen + ") ";
                List<SelectListItem> lista = context.Database.SqlQuery<VInventarioAlmacen>(cadenasql).ToList()

                    .OrderBy(n => n.Articulo)
                        .Select(n =>
                        new SelectListItem
                        {
                            Value = n.IDCaracteristica.ToString(),
                            Text = n.Articulo + " | " + n.Presentacion + " |Existencia: " + n.Existencia + " |Disponibilidad: " + n.Disponibilidad
                        }).ToList();
                var countrytip = new SelectListItem()
                {
                    Value = null,
                    Text = "--- Selecciona un Articulo---"
                };
                lista.Insert(0, countrytip);
                return lista;
            }

        }

        public List<SelectListItem> GetListaAlmacenes()
        {
            using (var context = new AlmacenContext())
            {
                List<SelectListItem> lista = context.Almacenes.AsNoTracking()
                    .OrderBy(n => n.Descripcion)
                        .Select(n =>
                        new SelectListItem
                        {
                            Value = n.IDAlmacen.ToString(),
                            Text = n.Descripcion
                        }).ToList();
                var descripciontip = new SelectListItem()
                {
                    Value = "0",
                    Text = "--- Selecciona un almacen ---"
                };
                lista.Insert(0, descripciontip);
                return lista;
            }
        }

        public IEnumerable<SelectListItem> GetListapresentaciones(int id)
        {
            using (var context = new ArticuloContext())
            {
                List<SelectListItem> lista = new List<SelectListItem>();
                string cadena = "select * from VInventarioAlmacen where IDArticulo=" + id + " and Existencia >0";
                List<VInventarioAlmacen> caracteristicas = context.Database.SqlQuery<VInventarioAlmacen>(cadena).ToList();

                foreach (VInventarioAlmacen cara in caracteristicas)
                {
                    var descripciontip2 = new SelectListItem()
                    {
                        Value = cara.IDCaracteristica.ToString(),
                        Text = cara.Presentacion + " | Existencia " + cara.Existencia
                    };
                    lista.Insert(0, descripciontip2);

                }
                var descripciontip = new SelectListItem()
                {
                    Value = "0",
                    Text = "--- Selecciona una presentacion ---"
                };
                lista.Insert(0, descripciontip);
                return new SelectList(lista, "Value", "Text");
            }
        }

        public IEnumerable<SelectListItem> GetListapresentaciones(int id, int idalmacen)
        {
            using (var context = new ArticuloContext())
            {
                List<SelectListItem> lista = new List<SelectListItem>();
                string cadena = "select * from VInventarioAlmacen where IDArticulo=" + id + " and Existencia >0 and IdAlmacen=" + idalmacen;
                List<VInventarioAlmacen> caracteristicas = context.Database.SqlQuery<VInventarioAlmacen>(cadena).ToList();

                foreach (VInventarioAlmacen cara in caracteristicas)
                {
                    var descripciontip2 = new SelectListItem()
                    {
                        Value = cara.IDCaracteristica.ToString(),
                        Text = cara.Presentacion + " | Existencia " + cara.Existencia
                    };
                    lista.Insert(0, descripciontip2);

                }
                var descripciontip = new SelectListItem()
                {
                    Value = "0",
                    Text = "--- Selecciona una presentacion ---"
                };
                lista.Insert(0, descripciontip);
                return new SelectList(lista, "Value", "Text");
            }
        }
        public IEnumerable<SelectListItem> Getpresentacioninicial()
        {
            using (var context = new ArticuloContext())
            {
                List<SelectListItem> lista = new List<SelectListItem>();

                var descripciontip = new SelectListItem()
                {
                    Value = "0",
                    Text = "--- Selecciona un Articulo ---"
                };
                lista.Insert(0, descripciontip);
                return new SelectList(lista, "Value", "Text");
            }
        }
    }
    
}

