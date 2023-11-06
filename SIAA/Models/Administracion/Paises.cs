using SIAAPI.Models.Administracion;
using SIAAPI.Models.CartaPorte;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SIAAPI.Models.Administracion
{
    [Table("Paises")]
    public class Paises
    {
        [Key]
        public int IDPais { get; set; }


        [Required(ErrorMessage = "La clave es obligatoria")]
        [DisplayName("Código de país")]
        [StringLength(4)]
        public string Codigo { get; set; }

        [DisplayName("Nombre del país")]
        [StringLength(100)]
        public string Pais { get; set; }
        [DisplayName("Clave de país")]
        [StringLength(3)]
        public string c_Pais { get; set; }
    }
    public class PaisesContext : DbContext
    {
        public PaisesContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<Paises> Paises { get; set; }
    }


    public class PaisesRepository
    {
        public IEnumerable<SelectListItem> GetEstadoPorPais(int idpais)
        {
            List<SelectListItem> lista;
            if (idpais == 0)
            {
                lista = new List<SelectListItem>();
                lista.Add(new SelectListItem() { Value = "0", Text = "Elige un país primero" });
                return (lista);
            }
            using (var context = new EstadosContext())
            {
                string cadenasql = "Select * from [dbo].[Estados] where [IDPais] =" + idpais + " ";

                lista = context.Database.SqlQuery<Estados>(cadenasql).ToList().OrderBy(n => n.Estado).Select(n => new SelectListItem
                {
                    Value = n.IDEstado.ToString(),
                    Text = n.Estado
                }).ToList();
                var countrytip = new SelectListItem()
                {
                    Value = null,
                    Text = "Selecciona un Estado"
                };
                lista.Insert(0, countrytip);
                return new SelectList(lista, "Value", "Text");
            }

        }

        public IEnumerable<SelectListItem> GetEstadoPorPaisSelec(int idpais, int idestado)
        {
            List<SelectListItem> lista;
            if (idpais == 0)
            {
                lista = new List<SelectListItem>();
                lista.Add(new SelectListItem() { Value = "0", Text = "Elige un país primero" });
                return (lista);
            }
            using (var context = new EstadosContext())
            {
                string cadenasql = "Select IDEstado, Estado, IDPais,c_estado from dbo.Estados where IDEstado= " + idestado + " union Select IDEstado, Estado, IDPais, c_estado from dbo.Estados where IDEstado<> " + idestado + " and IDPais =" + idpais + " ";
                lista = context.Database.SqlQuery<Estados>(cadenasql).ToList()

                        .Select(n =>
                         new SelectListItem
                         {
                             Value = n.IDEstado.ToString(),
                             Text = n.Estado
                         }).ToList();
                var countrytip = new SelectListItem()
                {
                    Value = null,
                    Text = "Selecciona un estado"
                };
                lista.Insert(0, countrytip);
                return new SelectList(lista, "Value", "Text");
            }
        }

        public IEnumerable<SelectListItem> GetMunicipioPorEstado(int idestado)
        {
            List<SelectListItem> lista;
            if (idestado == 0)
            {
                lista = new List<SelectListItem>();
                lista.Add(new SelectListItem() { Value = "0", Text = "Elige un Estado primero" });
                return (lista);
            }
            using (var context = new EstadosContext())
            {
                string cadenasql = "  select m.* from c_municipio as m inner join estados as e on e.c_estado=m.c_estado where e.idestado= " + idestado + " ";
                lista = context.Database.SqlQuery<c_Municipio>(cadenasql).ToList()
                    .OrderBy(n => n.Descripcion)
                        .Select(n =>
                         new SelectListItem
                         {
                             Value = n.IDMunicipio.ToString(),
                             Text = n.Descripcion
                         }).ToList();
                var countrytip = new SelectListItem()
                {
                    Value = null,
                    Text = "Selecciona un Municipio"
                };
                lista.Insert(0, countrytip);
                return new SelectList(lista, "Value", "Text");
            }

        }

        public IEnumerable<SelectListItem> GetMunicipioPorEstadoSelect(int idestado, int idmunicipio)
        {
            List<SelectListItem> lista;
            if (idestado == 0)
            {
                lista = new List<SelectListItem>();
                lista.Add(new SelectListItem() { Value = "0", Text = "Elige un Estado primero" });
                return (lista);
            }
            using (var context = new EstadosContext())
            {
                string cadenasql = "  select m.* from c_municipio as m inner join estados as e on e.c_estado=m.c_estado where e.idestado= " + idestado + "" +
                    "union Select mu.* from dbo.c_municipio  as mu inner join estados as es on es.c_estado=mu.c_estado where mu.idmunicipio<> " + idmunicipio + " and es.idestado =" + idestado + "";
                lista = context.Database.SqlQuery<c_Municipio>(cadenasql).ToList()
                    .OrderBy(n => n.Descripcion)
                        .Select(n =>
                         new SelectListItem
                         {
                             Value = n.IDMunicipio.ToString(),
                             Text = n.Descripcion
                         }).ToList();
                var countrytip = new SelectListItem()
                {
                    Value = null,
                    Text = "Selecciona un Municipio"
                };
                lista.Insert(0, countrytip);
                return new SelectList(lista, "Value", "Text");
            }

        }

        public IEnumerable<SelectListItem> GetLocalidadPorEstado(int idestado)
        {
            List<SelectListItem> lista;
            if (idestado == 0)
            {
                lista = new List<SelectListItem>();
                lista.Add(new SelectListItem() { Value = "0", Text = "Elige un Estado primero" });
                return (lista);
            }
            using (var context = new EstadosContext())
            {
                string cadenasql = " select l.* from c_localidad as l inner join estados as e on e.c_estado=l.c_estado where e.idestado=" + idestado + " ";
                lista = context.Database.SqlQuery<c_Localidad>(cadenasql).ToList()
                    .OrderBy(n => n.Descripcion)
                        .Select(n =>
                         new SelectListItem
                         {
                             Value = n.IDLocalidad.ToString(),
                             Text = n.Descripcion
                         }).ToList();
                var countrytip = new SelectListItem()
                {
                    Value = null,
                    Text = "Selecciona una Localidad"
                };
                lista.Insert(0, countrytip);
                return new SelectList(lista, "Value", "Text");
            }

        }
        public IEnumerable<SelectListItem> GetLocalidadPorEstadoSelec(int idestado, int idlocalidad)
        {
            List<SelectListItem> lista;
            if (idestado == 0)
            {
                lista = new List<SelectListItem>();
                lista.Add(new SelectListItem() { Value = "0", Text = "Elige un Estado primero" });
                return (lista);
            }
            using (var context = new EstadosContext())
            {
                string cadenasql = " select l.* from c_localidad as l inner join estados as e on e.c_estado=l.c_estado where e.idestado=" + idestado + " " +
                     "union Select lo.* from dbo.c_localidad  as lo inner join estados as es on es.c_estado=lo.c_estado where lo.idlocalidad<> " + idlocalidad + " and es.idestado =" + idestado + "";

                lista = context.Database.SqlQuery<c_Localidad>(cadenasql).ToList()
                    .OrderBy(n => n.Descripcion)
                        .Select(n =>
                         new SelectListItem
                         {
                             Value = n.IDLocalidad.ToString(),
                             Text = n.Descripcion
                         }).ToList();
                var countrytip = new SelectListItem()
                {
                    Value = null,
                    Text = "Selecciona una Localidad"
                };
                lista.Insert(0, countrytip);
                return new SelectList(lista, "Value", "Text");
            }

        }
        public IEnumerable<SelectListItem> GetColoniaPorCPSelec(string CP, int idcolonia)
        {
            List<SelectListItem> lista;
            if (CP == "")
            {
                lista = new List<SelectListItem>();
                lista.Add(new SelectListItem() { Value = "0", Text = "Ingresa CP primero" });
                return (lista);
            }
            using (var context = new EstadosContext())
            {
                string cadenasql = " select c.* from c_Colonia as c where c.c_codigopostal='" + CP.Trim() + "'" +
                     "union Select * from dbo.c_colonia where idcolonia<> " + idcolonia + " and c_codigopostal ='" + CP.Trim() + "'";

                lista = context.Database.SqlQuery<c_Colonia>(cadenasql).ToList()
                    .OrderBy(n => n.NomAsentamiento)
                        .Select(n =>
                         new SelectListItem
                         {
                             Value = n.IDColonia.ToString(),
                             Text = n.NomAsentamiento
                         }).ToList();
                var countrytip = new SelectListItem()
                {
                    Value = null,
                    Text = "Selecciona una Colonia"
                };
                lista.Insert(0, countrytip);
                return new SelectList(lista, "Value", "Text");
            }

        }
    }



}

