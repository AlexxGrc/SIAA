﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SIAAPI.ViewModels.CartaPorte
{
    public class VChofer
    {
        [Key]
        [DisplayName("ID Chofer")]
        public int IDChofer { get; set; }
        [DisplayName("RFC")]
        public string RFC { get; set; }
        [DisplayName("Nombre")]
        public string Nombre { get; set; }
        [DisplayName("No. de licencia")]
        public string NoLicencia{ get; set; }
        [DisplayName("Calle")]
        public string Calle { get; set; }
        [DisplayName("No. Ext")]
        public string NumExt { get; set; }
        [DisplayName("No. Int")]
        public string NumInt { get; set; }
        [DisplayName("ID Pais")]
        public int IDPais { get; set; }
        [DisplayName("Clave Pais")]
        public string c_Pais { get; set; }
        [DisplayName("Pais")]
        public string Pais { get; set; }
        [DisplayName("ID Estado")]
        public int IDEstado { get; set; }
        [DisplayName("Clave Estado")]
        public string c_Estado { get; set; }
        [DisplayName("Estado")]
        public string Estado { get; set; }
        [DisplayName("ID Municipio")]
        public int IDMunicipio { get; set; }
        [DisplayName("Clave Municipio")]
        public string c_Municipio { get; set; }
        [DisplayName("Municipio")]
        public string Municipio { get; set; }
        [DisplayName("ID Localidad")]
        public int IDLocalidad { get; set; }
        [DisplayName("Clave Localidad")]
        public string c_Localidad { get; set; }
        [DisplayName("Localidad")]
        public string Localidad { get; set; }
        [DisplayName("ID ")]
        public int IDColonia { get; set; }
        [DisplayName("Clave Colonia")]
        public string c_Colonia { get; set; }
        [DisplayName("Colonia")]
        public string Colonia { get; set; }
        [DisplayName("CP")]
        public string CP { get; set; }
        [DisplayName("Referencia")]
        public string Referencia { get; set; }
        [DisplayName("Activo")]
        public Boolean Activo { get; set; }
    }
    public class VChoferContext : DbContext
    {
        public VChoferContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<VChofer> VChofer { get; set; }
    }
}