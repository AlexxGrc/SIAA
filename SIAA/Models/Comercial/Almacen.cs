using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.Mvc;


namespace SIAAPI.Models.Comercial
{
   //Nombre de la tabla
    [Table("Almacen")]
    public class Almacen
    {
        //Primary Key IDAlmacen
        [Key]
        public int IDAlmacen { get; set; }

        //CodAlm: Código de almacen
        [DisplayName("Código de Almacen")]
        [StringLength(4, MinimumLength = 4, ErrorMessage = "El campo Código de Almacen debe contener 4 dígitos")]
        [Required]
        public string CodAlm { get; set; }

        //Descripcion: Descripcion de Almacen 
        [DisplayName("Descripción")]
        [StringLength(100)]
        [Required]
        public string Descripcion { get; set; }

        //Direccion: Dirección de Almacen
        [DisplayName("Dirección")]
        [StringLength(250)]
        [Required]
        public string DIRECCION { get; set; }

        //Colonia: Colonia de Almacen
        [DisplayName("Colonia")]
        [StringLength(100)]
        [Required]
        public string Colonia { get; set;}

        //Municipio: Municipio de Almacen
        [DisplayName("Municipio")]
        [StringLength(100)]
        [Required]
        public string Municipio { get; set; }

        //CP: Código postal de Almacen
        [DisplayName("C.P.")]
        [StringLength(5, MinimumLength = 5, ErrorMessage = "El campo C.P. debe contener 5 dígitos")]
        [Required]
        public string CP { get; set; }

        //Telefono: Telefóno de Almacen
        [DisplayName("Telefóno")]
        [StringLength(15, MinimumLength = 10, ErrorMessage = "El campo Telefóno debe contener un mínimo de 10 dígitos")]
        [Required]
        public string Telefono { get; set; }

        //Responsable: Responsable de Almacen 
        [DisplayName("Responsable de Almacen")]
        [StringLength(150)]
        [Required]
        public string Responsable { get; set; }

        //IDEstado
        [DisplayName("Estado")]
        [Required]
        public int IDEstado { get; set; }
        
    }

    public class AlmacenContext : DbContext
    {
        public AlmacenContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<Almacen> Almacenes { get; set; }

    }


    public class AlmacenRepository
    {


        public IEnumerable<SelectListItem> GetAlmacenes()
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
                return new SelectList(lista, "Value", "Text");
            }
        }

        public IEnumerable<SelectListItem> GetSimilares(int IDCaracteristica)
        {
            List<SelectListItem> lista = new List<SelectListItem>();
            Caracteristica caracteris= new AlmacenContext().Database.SqlQuery<Caracteristica>("SELECT*FROM Caracteristica where ID=" + IDCaracteristica).ToList().FirstOrDefault();
            Articulo articuloOriginal = new ArticuloContext().Articulo.Find(caracteris.Articulo_IDArticulo);
            String cref = articuloOriginal.Cref;
            FormulaSiaapi.Formulas formula = new FormulaSiaapi.Formulas();
            int anchooOriginal = int.Parse(formula.getvalor("ANCHO", caracteris.Presentacion).ToString());


            string crefBuscar = cref.Substring(1, cref.Length - 3);
            List<Articulo> articulos = new ArticuloContext().Articulo.Where(s => s.Cref.Contains(crefBuscar)).ToList();
            foreach (Articulo articulo in articulos)
            {
                List<Caracteristica> caracteristicas = new AlmacenContext().Database.SqlQuery<Caracteristica>("SELECT*FROM Caracteristica where Articulo_IDArticulo=" + articulo.IDArticulo).ToList();
                foreach(Caracteristica cara in caracteristicas)
                {
                    int anchoEncontrado = int.Parse(formula.getvalor("ANCHO", cara.Presentacion).ToString());
                    if(EstaEnRango(anchooOriginal, anchoEncontrado))
                    {
                        SelectListItem item = new SelectListItem();
                        item.Value = cara.ID.ToString();
                        item.Text = cara.Presentacion;

                        lista.Add(item);

                    }
                }
                
            }
            return new SelectList(lista, "Value", "Text");
        }

        private bool EstaEnRango(int ancho, int ancho2)
        {
            if(ancho2 >= (ancho-3) && ancho2 <= (ancho+5))
            {
                return true;
            }
            return false;
        }
        public class AlmacenEdo
        {
            //Primary Key IDAlmacen
            [Key]
            public int IDAlmacen { get; set; }

            //CodAlm: Código de almacen
            [DisplayName("Código de Almacen")]
            [StringLength(4, MinimumLength = 4, ErrorMessage = "El campo Código de Almacen debe contener 4 dígitos")]
            [Required]
            public string CodAlm { get; set; }

            //Descripcion: Descripcion de Almacen 
            [DisplayName("Descripción")]
            [StringLength(100)]
            [Required]
            public string Descripcion { get; set; }

            //Direccion: Dirección de Almacen
            [DisplayName("Dirección")]
            [StringLength(250)]
            [Required]
            public string Direccion { get; set; }

            //Colonia: Colonia de Almacen
            [DisplayName("Colonia")]
            [StringLength(100)]
            [Required]
            public string Colonia { get; set; }

            //Municipio: Municipio de Almacen
            [DisplayName("Municipio")]
            [StringLength(100)]
            [Required]
            public string Municipio { get; set; }

            //CP: Código postal de Almacen
            [DisplayName("C.P.")]
            [StringLength(5, MinimumLength = 5, ErrorMessage = "El campo C.P. debe contener 5 dígitos")]
            [Required]
            public string CP { get; set; }

            //Telefono: Telefóno de Almacen
            [DisplayName("Telefóno")]
            [StringLength(15, MinimumLength = 10, ErrorMessage = "El campo Telefóno debe contener un mínimo de 10 dígitos")]
            [Required]
            public string Telefono { get; set; }

            //Responsable: Responsable de Almacen 
            [DisplayName("Responsable de Almacen")]
            [StringLength(150)]
            [Required]
            public string Responsable { get; set; }
            //IDEstado
            [DisplayName("IDEstado")]
            [Required]
            public int IDEstado { get; set; }

            [DisplayName("Estado")]
            [Required]
            public string Estado { get; set; }
            [DisplayName("País")]
            [Required]
            public string Pais { get; set; }

        }

        public class AlmacenEdoContext : DbContext
        {
            public AlmacenEdoContext() : base("name=DefaultConnection")
            {

            }
            public DbSet<AlmacenEdo> AlmaceneEdos { get; set; }

        }

      
    }
}