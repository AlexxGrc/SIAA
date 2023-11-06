using SIAAPI.Models.Administracion;
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

namespace SIAAPI.Models.Administracion
{

    [Table("Estados")]
    public class Estados
    {
        [Key]
        public int IDEstado { get; set; }

        [DisplayName("Estado")]
        [Required]
        [StringLength(50)]
        public string Estado { get; set; }
        public string c_Estado { get; set; }
        [DisplayName("País")]
        public int IDPais { get; set; }
        public virtual Paises paises { get; set; }


        public virtual ICollection<Clientes> Clientes { get; set; }
        public virtual ICollection<Entrega> Entrega { get; set; }
        public virtual ICollection<Cobro> Cobro { get; set; }
        public virtual ICollection<Proveedor> Proveedores { get; set; }
        public virtual ICollection<ClientesP> ClientesP { get; set; }
    }
    public class EstadosContext : System.Data.Entity.DbContext
    {
        public EstadosContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<Estados> Estados { get; set; }

        public System.Data.Entity.DbSet<SIAAPI.Models.Administracion.Paises> Paises { get; set; }
    }
    [Table("VEstado")]
    public class VEstado
    {
        [Key]
        public int IDEstado { get; set; }

        [DisplayName("Estado")]
        [Required]
        [StringLength(50)]
        public string Estado { get; set; }
        [DisplayName("Clave Estado")]
        public string c_Estado { get; set; }
        [DisplayName("IDPaís")]
        public int IDPais { get; set; }
        [DisplayName("País")]
        public string Pais { get; set; }
    }
    public class VEstadoContext : System.Data.Entity.DbContext
    {
        public VEstadoContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<VEstado> VEstados { get; set; }
    }
}

public class EstadosRepository
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
            string cadenasql = "Select* from [dbo].[Estados] where [IDPais] =" + idpais + " ";
            lista = context.Database.SqlQuery<Estados>(cadenasql).ToList()
                .OrderBy(n => n.Estado)
                    .Select(n =>
                     new SelectListItem
                     {
                         Value = n.IDEstado.ToString(),
                         Text = n.Estado
                     }).ToList();
            var countrytip = new SelectListItem()
            {
                Value = null,
                Text = "--- Selecciona un Estado ---"
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
            //lista.Add(new SelectListItem() { Value = "0", Text = "Elige un país primero" });
            return (lista);
        }
        using (var context = new EstadosContext())
        {
            string cadenasql = "Select*from dbo.Estados where IDEstado= " + idestado + " union Select * from dbo.Estados where IDEstado<> " + idestado + " and IDPais =" + idpais + " ";
            lista = context.Database.SqlQuery<Estados>(cadenasql).ToList()

                    .Select(n =>
                     new SelectListItem
                     {
                         Value = n.IDEstado.ToString(),
                         Text = n.Estado
                     }).ToList();
            //var countrytip = new SelectListItem()
            //{
            //    Value = null,
            //    Text = "--- Selecciona un país ---"
            //};
            //lista.Insert(0, countrytip);
            return new SelectList(lista, "Value", "Text");
        }
    }

    public IEnumerable<SelectListItem> GetPaisSelec(int idpais)
    {
        List<SelectListItem> lista;
        if (idpais == 0)
        {
            lista = new List<SelectListItem>();
            //lista.Add(new SelectListItem() { Value = "0", Text = "Elige un país primero" });
            return (lista);
        }
        using (var context = new EstadosContext())
        {
            string cadenasql = "Select * from dbo.Paises where IDPais= " + idpais + " union Select*from dbo.Paises where IDPais <>" + idpais + " ";
            lista = context.Database.SqlQuery<Paises>(cadenasql).ToList()
                    .Select(n =>
                     new SelectListItem
                     {
                         Value = n.IDPais.ToString(),
                         Text = n.Pais
                     }).ToList();
            //var countrytip = new SelectListItem()
            //{
            //    Value = null,
            //    Text = "--- Selecciona un país ---"
            //};
            //lista.Insert(0, countrytip);
            return new SelectList(lista, "Value", "Text");
        }


    }
    public string getNombreEstado(int _id)

    {
        Estados elemento = new EstadosContext().Estados.Single(m => m.IDEstado == _id);
        return elemento.Estado;

    }
    public IEnumerable<SelectListItem> GetEstados()
    {
        using (var context = new EstadosContext())
        {
            List<SelectListItem> lista = context.Estados.AsNoTracking()
                .OrderBy(n => n.IDPais).ThenBy(n => n.Estado)
                    .Select(n =>
                    new SelectListItem
                    {
                        Value = n.IDEstado.ToString(),
                        Text = n.Estado
                    }).ToList();
            var countrytip = new SelectListItem()
            {
                Value = null,
                Text = "--- Selecciona un Estado del pais ---"
            };
            lista.Insert(0, countrytip);
            return new SelectList(lista, "Value", "Text");
        }
    }
}