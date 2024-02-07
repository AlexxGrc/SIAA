namespace SIAAPI.Models.Comercial
{
    using Administracion;
    using Comercial;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity;
    using System.Data.Entity.Spatial;
    using System.Linq;
    using System.Web.Mvc;

    [Table("Empresa")]
    public  class Empresa
    {
        [Key]
       
        public int IDEmpresa { get; set; }
        [DisplayName("Razón Social")]
        [StringLength(250)]
        public string RazonSocial { get; set; }

        [StringLength(150)]
        [DisplayName("Director")]
        public string Director { get; set; }

       
        [StringLength(150)]
        [DisplayName("Persona de Compras")]
        public string Persona_de_Compras { get; set; }

        [StringLength(150)]
        [DisplayName("Director Financiero")]
        public string Director_finanzas { get; set; }

        [DisplayName("Cajero")]
        [StringLength(150)]
        public string Recepcion_caja { get; set; }

        [StringLength(150)]
        public string Calle { get; set; }
        [DisplayName("No. Exterior")]
        [StringLength(50)]
        public string NoExt { get; set; }
        [DisplayName("No. Interior")]
        [StringLength(50)]
        public string NoInt { get; set; }

        [StringLength(150)]
        public string Colonia { get; set; }

        [StringLength(150)]
        public string Municipio { get; set; }

        //    public virtual Estados Estados { get; set; }
        [Display(Name = "Estado")]
        public int IDEstado { get; set; }
        public virtual Estados Estados { get; set; }
        [DisplayName("C.P.")]
        [StringLength(5)]
        public string CP { get; set; }
        [DisplayName("Teléfono")]
        [StringLength(20)]
        public string Telefono { get; set; }

        [Required]
        [StringLength(13)]
        public string RFC { get; set; }

        public bool Activa { get; set; }

        [DisplayName("Regimen Fiscal")]
        //  public virtual c_RegimenFiscal c_RegimenFiscal { get; set; }
        public int IDRegimenFiscal { get; set; }
        public virtual c_RegimenFiscal c_RegimenFiscal { get; set; }
        [Column(TypeName = "image")]
        public byte[] Logo { get; set; }

        public string mail { get; set; }

        public string Siglas { get; set; }


        [Required]
        public string Nombre40 { get; set; }

        [DisplayName("Regimen Societario")]
        [Required]
        public string RegimenSocietario { get; set; }

        [DisplayName("Numero de Almacenes")]
        [Required]
        public int NoAlmacenes { get; set; }


        [DisplayName("Numero de Articulos")]
        [Required]
        public int NoArticulos { get; set; }


        [DisplayName("Numero de Usuarios")]
        [Required]
        public int NoUsuarios { get; set; }
    }

    public class EmpresaContext : DbContext
    {
        public EmpresaContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<Empresa> empresas { get; set; }

       
    }

    public class EmpresaEditViewModel
    {

        [Key]

        public int IDEmpresa { get; set; }

        [StringLength(250)]
        public string RazonSocial { get; set; }

        [StringLength(150)]
        public string Director { get; set; }

        [StringLength(150)]
        public string Director_finanzas { get; set; }
        [StringLength(150)]
        public string Persona_de_Compras { get; set; }

        [StringLength(150)]
        public string Recepcion_caja { get; set; }

        [StringLength(150)]
        public string Calle { get; set; }

        [StringLength(50)]
        public string NoExt { get; set; }

        [StringLength(50)]
        public string NoInt { get; set; }

        [StringLength(150)]
        public string Colonia { get; set; }

        [StringLength(150)]
        public string Municipio { get; set; }

        [Required]
        [Display(Name = "Estado de la Republica")]
        public int IDEstado { get; set; }
        public IEnumerable<SelectListItem> estados { get; set; }

        [StringLength(5)]
        public string CP { get; set; }

        [StringLength(20)]
        public string Telefono { get; set; }

        [Required]
        [StringLength(13)]
        public string RFC { get; set; }

        public bool Activa { get; set; }

        [Required]
        [Display(Name = "Regimen Fiscal")]
        public int IDRegimenFiscal { get; set; }
        public IEnumerable<SelectListItem> regimenes { get; set; }

        [Column(TypeName = "image")]
        public byte[] Logo { get; set; }

        public string mail { get; set; }

        public string Siglas { get; set; }




    }

    public class EmpresaDisplayViewModel
    {

        [Key]

        public int IDEmpresa { get; set; }
        [Display(Name = "Logotipo")]
        [Column(TypeName = "image")]
        public byte[] Logo { get; set; }

        [StringLength(250)]
        [Display(Name = "Razon Social")]
        public string RazonSocial { get; set; }

      
        [StringLength(20)]
        public string Telefono { get; set; }

        [Required]
        [StringLength(13)]
        public string RFC { get; set; }

        public bool Activa { get; set; }


        public string mail { get; set; }

        public string Siglas { get; set; }




    }


    public class EmpresaDetalle
    {
        [Key]

        public int IDEmpresa { get; set; }
        [Column(TypeName = "image")]
        public byte[] Logo { get; set; }

        [StringLength(250)]
        [DisplayName("Razón Social")]
        public string RazonSocial { get; set; }
        [DisplayName("Director")]
        [StringLength(150)]
         
        public string Director { get; set; }

        [StringLength(150)]

        [DisplayName("Director Financiero")]
        public string Director_finanzas { get; set; }

        [StringLength(150)]


        [Display(Name = "Cajero")]
        public string Recepcion_caja { get; set; }

        [StringLength(150)]
        public string Calle { get; set; }
        [DisplayName("No. Exterior")]
        [StringLength(50)]
        public string NoExt { get; set; }
        [DisplayName("No. Interior")]
        [StringLength(50)]
        public string NoInt { get; set; }

        [StringLength(150)]
        public string Colonia { get; set; }

        [StringLength(150)]
        public string Municipio { get; set; }

        [Display(Name = "Estado")]
        public string Estado { get; set; }


        [DisplayName("C.P.")]
        [StringLength(5)]
        public string CP { get; set; }
        [DisplayName("Teléfono")]
        [StringLength(20)]
        public string Telefono { get; set; }

        [Required]
        [StringLength(13)]
        public string RFC { get; set; }

        public bool Activa { get; set; }

        [Display(Name = "Regimen fiscal")]
        public string _RegimenFiscal { get; set; }

        [DisplayName("Correo")]

        public string mail { get; set; }

        public string Siglas { get; set; }

        [StringLength(150)]
        [DisplayName("Persona de Compras")]
        public string Persona_de_Compras { get; set; }


    }



    public class EmpresaRepository
    {

        public List<EmpresaDisplayViewModel> GetEmpresaslistacorta()
        {
            using (var context = new EmpresaContext())
            {
                List<Empresa> listaempresas = new List<Empresa>();
                listaempresas = context.empresas.AsNoTracking()

                  .ToList();

                if (listaempresas != null)
                {
                    List<EmpresaDisplayViewModel> empresasDisplay = new List<EmpresaDisplayViewModel>();
                    foreach (var x in listaempresas)
                    {
                        var elementoDisplay = new EmpresaDisplayViewModel()
                        {
                            IDEmpresa = x.IDEmpresa,
                            Logo = x.Logo,
                            RazonSocial = x.RazonSocial,
                            Telefono = x.Telefono,
                            RFC = x.RFC,
                            Activa = x.Activa,
                            mail = x.mail,
                            Siglas = x.Siglas
                        };
                        empresasDisplay.Add(elementoDisplay);
                    }
                    return empresasDisplay;
                }
                return null;
            }
        }

        public EmpresaDetalle GetEmpresaDetalle(int _id)
        {
            using (var context = new EmpresaContext())
            {
                var empresa = context.empresas.Single(m => m.IDEmpresa == _id);

                var elementoamostrar = new EmpresaDetalle()
                {
                    IDEmpresa = empresa.IDEmpresa,
                    Activa = empresa.Activa,
                    Calle = empresa.Calle,
                    Colonia = empresa.Colonia,
                    CP = empresa.CP,
                    _RegimenFiscal = empresa.c_RegimenFiscal.Descripcion,
                    Director = empresa.Director,
                    Director_finanzas = empresa.Director_finanzas,
                    Persona_de_Compras = empresa.Persona_de_Compras,
                    Estado = empresa.Estados.Estado,
                    mail = empresa.mail,
                    Municipio = empresa.Municipio,
                    NoExt = empresa.NoExt,
                    NoInt = empresa.NoInt,
                    RazonSocial = empresa.RazonSocial,
                    Recepcion_caja = empresa.Recepcion_caja,
                    RFC = empresa.RFC,
                    Siglas = empresa.Siglas,
                    Telefono = empresa.Telefono

                };
                return elementoamostrar;
            }


        }
        public class EmpresaNombre
        {
            public IEnumerable<SelectListItem> GetEmpresa()
            {
                using (var context = new EmpresaContext())
                {
                    List<SelectListItem> lista = context.empresas.AsNoTracking()
                        .OrderBy(n => n.RazonSocial)
                            .Select(n =>
                            new SelectListItem
                            {
                                Value = n.IDEmpresa.ToString(),
                                Text = n.RFC + " | " + n.RazonSocial
                            }).ToList();
                    var countrytip = new SelectListItem()
                    {
                        Value = null,
                        Text = "--- Selecciona una empresa---"
                    };
                    lista.Insert(0, countrytip);
                    return new SelectList(lista, "Value", "Text");
                }

            }

        }
    }
}


