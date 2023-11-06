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

namespace SIAAPI.ClasesProduccion
{
    public class ClsCotizador
    {

        public int IDCotizacion { get; set; }

        [DisplayName("Adhesivo")]
        public bool conadhesivo { get; set; }

        [DisplayName("Descripcion del archivo")]

        public string Descripcion { get; set; }

        public int IDArticulo { get; set; }


        [DisplayName("Hotstamping")]
        public bool hotstamping { get; set; }

        [DisplayName("TermoEncogible")]
        public bool mangatermo { get; set; }

        public string ACABADO { get; set; }

        public bool SuajeNuevo { get; set; }
        public bool MangaTransparente { get; set; }
        public string TipoSuaje { get; set; }
        public string TipoSuajeFigura { get; set; }
        public string TipoCorte { get; set; }
        public string Esquinas { get; set; }


        //dientes
        [DisplayName("Numero de dientes")]
        public decimal TH { get; set; }


        [DisplayName("Numero de tintas")]
        [Required]
        [Range(0, 7, ErrorMessage = "Numero de tintas debe ser entre 0 y 7")]
        public int Numerodetintas { get; set; }

        [BrowsableAttribute(true)]
        [Description("Cantidad a producir")]
        [DisplayName("Cantidad a producir")]
        [Required(ErrorMessage = "Por favor ingresa la cantidad")]
        public decimal Cantidad { get; set; }

        [DisplayName("Cantidad por paquete o rollo")]
        public int Cantidadxrollo { get; set; }

        [DisplayName("Largo Material mts sin Merma")]
        public int largomaterialenMts { get; set; }

        [DisplayName("Ancho minimo de material")]
        public int anchomaterialenmm { get; set; }

        [DisplayName("Avance del producto en mm")]
        public decimal largoproductomm { get; set; }

        [DisplayName("Largo Material Mts necesitados")]
        public decimal MaterialNecesitado { get; set; }


        [DisplayName("Cantidad de cinta necesitada en Mts2")]
        public decimal CantidadMPMts2 { get; set; }



        private int alpaso = 1;

        public decimal Cantidaderollos { get; set; }



        [DisplayName("Cantidad al paso")]
        public int productosalpaso
        {
            get
            {
                if (alpaso == 0)
                { return 1; }

                else
                {
                    return alpaso;
                }
            }
            set
            {
                if ((value == 0))
                {
                    alpaso = 1;
                }
                else
                {
                    alpaso = value;
                }
            }
        }

        [DisplayName("Eje del producto en mm")]
        [Required]
        public decimal anchoproductomm { get; set; }

        [DisplayName("Gap al Eje en mm")]
        [Required]
        public decimal gapeje { get; set; }

        [DisplayName("Gap al Avance en mm")]
        [Required]
        public decimal gapavance { get; set; }

        private int cavidad { get; set; }

        [DisplayName("Cavidades del suaje")]
        public int cavidadesdesuajeEje
        {
            get
            {
                if (cavidad == 0)
                { return 1; }

                else
                {
                    return cavidad;
                }
            }
            set
            {
                if ((value == 0))
                {
                    cavidad = 1;
                }
                else
                {
                    cavidad = value;
                }
            }
        }

        private int cavidadAvacne { get; set; }

        [DisplayName("Cavidades Avance del suaje")]
        public int cavidadesdesuajeAvance
        {
            get
            {
                if (cavidadAvacne == 0)
                { return 1; }

                else
                {
                    return cavidadAvacne;
                }
            }
            set
            {
                if ((value == 0))
                {
                    cavidadAvacne = 1;
                }
                else
                {
                    cavidadAvacne = value;
                }
            }
        }


        [DisplayName("Numero de Cintas")]
        public decimal Numerodecintas { get; set; }

        [DisplayName("Mts Lineales de Merma")]
        public int MtsdeMerma { get; set; }


        [DisplayName("Horas de Inspeccion")]
        public decimal HrInspeccion { get; set; }

        [DisplayName("Horas Prensa")]
        public decimal HrPrensa { get; set; }

        [DisplayName("Horas Embobinado")]
        public decimal HrEmbobinado { get; set; }

        [DisplayName("Hora Corte")]
        public decimal HrCorte { get; set; }

        [DisplayName("Hora de sellado")]
        public decimal HrSellado { get; set; }

        private int largoc;

        [DisplayName("largo de la cinta en Mts")]

        public int LargoCinta
        {
            get
            {
                if (largoc == 0)
                { return 1524; }

                else
                {
                    return largoc;
                }
            }
            set
            {
                if ((value == 0))
                {
                    largoc = 1524;
                }
                else
                {
                    largoc = value;
                }
            }
        }
        [DisplayName("Velocidad de prensa en Pies por Hora")]
        public int VelocidaddePrensaPies { get; set; }

        [DisplayName("Velocidad de prensa en Mts por Hora")]
        public int VelocidaddePrensaMts { get; set; }


        [DisplayName("Minimo a producir")]
        public decimal Minimoproducir { get; set; }


        private int _anchommmaster;

        [DisplayName("Ancho Master en mm")]

        public int anchommmaster
        {
            get
            {
                if (_anchommmaster == 0)
                { return 1524; }

                else
                {
                    return _anchommmaster;
                }
            }
            set
            {
                if ((value == 0))
                {
                    _anchommmaster = 1524;
                }
                else
                {
                    _anchommmaster = value;
                }
            }
        }
        [DisplayName("Cintas en el Master")]
        public int CintasMaster { get; set; }

        public bool CobrarMaster { get; set; }

        private decimal _coste;

        [Range(0, 100,
        ErrorMessage = "El valor para {0} debe ser entre {1} y {2}.")]
        public decimal CostoM2Cinta
        {
            get { return _coste; }

            set
            {
                if ((value < 0))
                {
                    _coste = 0;
                }
                else
                {
                    _coste = value;
                }
            }
        }



        private decimal _coste2;
        private decimal _coste3;

        [Range(0, 1000,
        ErrorMessage = "El valor para {0} debe ser entre {1} y {2}.")]
        public decimal CostoM2Cinta2
        {
            get { return _coste2; }

            set
            {
                if ((value < 0))
                {
                    _coste2 = 0;
                }
                else
                {
                    _coste2 = value;
                }
            }
        }

        public decimal CostoM2Cinta3
        {
            get { return _coste3; }

            set
            {
                if ((value < 0))
                {
                    _coste3 = 0;
                }
                else
                {
                    _coste3 = value;
                }
            }
        }


        public int IDMaterial { get; set; }

        public int IDMaterial2 { get; set; }

        public int IDMaterial3 { get; set; }

        public int IDMaquinaPrensa { get; set; }

        public int IDMaquinaEmbobinado { get; set; }

        public int IDSuaje { get; set; }
        public int IDSuaje2 { get; set; }
        public decimal CostototalMP { get; set; }

        public string Enviar { get; set; }

        [DisplayName("Suaje en Existencia")]
        public int Suaje { get; set; }

        public int Pleca { get; set; }

        public decimal Rango1 { get; set; }

        public decimal Rango2 { get; set; }

        public decimal Rango3 { get; set; }

        public decimal Rango4 { get; set; }

        public decimal Rango1gain { get; set; }

        public decimal Rango2gain { get; set; }

        public decimal Rango3gain { get; set; }

        public decimal Rango4gain { get; set; }

        public bool Yatienematriz { get; set; }

        public MatrizPrecioCotiza MatrizPrecio = new MatrizPrecioCotiza();

        public int DiluirSuajeEnPedidos { get; set; }

        public List<Tinta> Tintas = new List<Tinta>();

        public decimal Costo1 { get; set; }
        public decimal Costo2 { get; set; }
        public decimal Costo3 { get; set; }
        public decimal Costo4 { get; set; }

        public decimal Costo1mxn { get; set; }
        public decimal Costo2mxn { get; set; }
        public decimal Costo3mxn { get; set; }
        public decimal Costo4mxn { get; set; }


        public PrecioConvenido precioconvenidos = new PrecioConvenido();
        public PrecioConvenido precioconvenidosmxn = new PrecioConvenido();

        public int IDMonedapreciosconvenidos { get; set; }

        public decimal TCcotizado { get; set; }
        public ClsCotizador()
        {
            Rango1gain = SIAAPI.Properties.Settings.Default.Rango1gain;
            Rango2gain = SIAAPI.Properties.Settings.Default.Rango2gain;
            Rango3gain = SIAAPI.Properties.Settings.Default.Rango3gain;
            Rango4gain = SIAAPI.Properties.Settings.Default.Rango4gain;
        }


        public bool Materialnecesitarefile { get; set; }


        public int cavidades { get; set; }
        public int IDFamilia { get; set; }

        public int IDFamilia2 { get; set; }

        public bool Valido { get; set; }

        public int cavidadesdesuaje { get; set; }



        public int IDCaracteristica { get; set; }

        public int IDCaracteristica2 { get; set; }

        public int IDCentro { get; set; }

        public int IDCaja { get; set; }

        public string cyrel { get; set; }

        public int CuantasPlecas { get; set; }

        public decimal AvanceReal { get; set; }

        public decimal LargoPlaca { get; set; }

        public int EjeMaquina { get; set; }

        public decimal PulgadasLineales { get; set; }

        public decimal LargoSuaje { get; set; }

        public decimal CostoSuaje { get; set; }

        public decimal PrecioSuaje { get; set; }


    }



    public class suajeC
    {

        public int IDCaracteristica { get; set; }

        public string Descripcion { get; set; }

        public string Cref { get; set; }

    }

    public class SuajeCaracteristicas
    {
        public decimal Eje { get; set; }

        public decimal Avance { get; set; }

        public decimal Gapeje { get; set; }

        public decimal Gapavance { get; set; }

        public int CavidadEje { get; set; }
        public int CavidadAvance { get; set; }
        public int RepAvance { get; set; }
        public int TH { get; set; }
        public string THAlma { get; set; }

        public string Corte { get; set; }
        public string Material { get; set; }
        public string Alma { get; set; }
        public int AnchoCinta { get; set; }
        public int AnchoCintaimpresa { get; set; }

        public int Cavidades { get; set; }
        public int EjeSuaje { get; set; }


    }


    public class Tinta
    {
        public int IDTinta { get; set; }
        public decimal Area { get; set; }
        public decimal kg { get; set; }
        public decimal Costo { get; set; }

        public string Cref { get; set; }

        public string Descripcion { get; set; }
        public Tinta()
        {

        }

        public Tinta(int pan, int Ar)
        {
            IDTinta = pan;
            Area = Ar;
        }
    }

    public class Celda
    {
        public decimal Valor { get; set; }

        public string Color { get; set; }

        public Celda()
        {
            Valor = 0M;
        }

    }

    public class Fila
    {
        public Celda Celda1 { get; set; }
        public Celda Celda2 { get; set; }
        public Celda Celda3 { get; set; }
        public Celda Celda4 { get; set; }

        public Fila()
        {
            Celda1 = new Celda();
            Celda2 = new Celda();
            Celda3 = new Celda();
            Celda4 = new Celda();
        }
    }

    public class MatrizPrecioCotiza
    {
        public Fila Fila1 { get; set; }
        public Fila Fila2 { get; set; }

        public Fila Fila3 { get; set; }

        public Fila Fila4 { get; set; }

        public MatrizPrecioCotiza()
        {
            Fila1 = new Fila();
            Fila2 = new Fila();
            Fila3 = new Fila();
            Fila4 = new Fila();

        }
    }


    public class PrecioConvenido
    {


        public decimal precio1 { get; set; }

        public decimal precio2 { get; set; }

        public decimal precio3 { get; set; }

        public decimal precio4 { get; set; }

    }





    public class Repository
    {
        public IEnumerable<SelectListItem> GetTintas()
        {
            List<SelectListItem> lista;
            using (var context = new ArticuloContext())
            {
                string cadenasql = "Select * from [dbo].[Articulo] where idtipoarticulo=7 and obsoleto='0' order by descripcion";

                lista = context.Database.SqlQuery<Articulo>(cadenasql).ToList().Select(n => new SelectListItem
                {
                    Value = n.IDArticulo.ToString(),
                    Text = n.Descripcion
                }).ToList();
                var countrytip = new SelectListItem()
                {
                    Value = null,
                    Text = "Selecciona una tinta"
                };
                lista.Insert(0, countrytip);
                return new SelectList(lista, "Value", "Text");
            }
            //using (var context = new ArticuloContext())
            //{
            //    List<SelectListItem> listaPeriocidadPago = context.Articulo.AsNoTracking().Where(s => (s.IDTipoArticulo == 7 && s.obsoleto == false))
            //        .OrderBy(n => n.Descripcion)
            //            .Select(n =>
            //            new SelectListItem
            //            {
            //                Value = n.IDArticulo.ToString(),
            //                Text = n.Descripcion
            //            }).ToList();
            //    var descripciontip = new SelectListItem()
            //    {
            //        Value = "0",
            //        Text = "--- Selecciona una Tinta ---"
            //    };
            //    listaPeriocidadPago.Insert(0, descripciontip);
            //    return new SelectList(listaPeriocidadPago, "Value", "Text");
            //}
        }

        public IEnumerable<SelectListItem> GetSuajes()
        {
            using (var context = new ArticuloContext())
            {
                string cadena = "select C.ID as IDCaracteristica, A.Cref,A.Descripcion from Articulo as A inner join Caracteristica as C on A.IDArticulo=C.Articulo_IDArticulo where A.IDTipoArticulo=2 and A.obsoleto='0' and C.obsoleto='0' and (A.IDFamilia=76 or A.IDFamilia=75 or A.IDFamilia=77 or A.IDFamilia=71 or A.IDFamilia =72 or A.IDFamilia=11 or A.IDFamilia=69 or A.IDFamilia=80) order by A.descripcion";
                List<suajeC> listadesuaje = context.Database.SqlQuery<suajeC>(cadena).ToList();

                List<SelectListItem> listadesuajes = new List<SelectListItem>();
                foreach (suajeC item in listadesuaje)
                {
                    SelectListItem art = new SelectListItem { Text = item.Cref + "  |   " + item.Descripcion, Value = item.IDCaracteristica.ToString() };

                    listadesuajes.Add(art);
                }

                var descripciontip = new SelectListItem()
                {
                    Value = "0",
                    Text = "--- Selecciona una Suaje ---"
                };
                listadesuajes.Insert(0, descripciontip);
                return new SelectList(listadesuajes, "Value", "Text");
            }
        }

        public IEnumerable<SelectListItem> GetPlecas()
        {
            using (var context = new ArticuloContext())
            {
                string cadena = "select C.ID as IDCaracteristica, A.Cref,A.Descripcion from Articulo as A inner join Caracteristica as C on A.IDArticulo=C.Articulo_IDArticulo where A.IDTipoArticulo=2 and A.obsoleto='0' and C.obsoleto='0' and (A.IDFamilia=70) order by A.cref";
                List<suajeC> listadesuaje = context.Database.SqlQuery<suajeC>(cadena).ToList();

                List<SelectListItem> listadesuajes = new List<SelectListItem>();
                foreach (suajeC item in listadesuaje)
                {
                    SelectListItem art = new SelectListItem { Text = item.Cref + "  |   " + item.Descripcion, Value = item.IDCaracteristica.ToString() };

                    listadesuajes.Add(art);
                }

                var descripciontip = new SelectListItem()
                {
                    Value = "0",
                    Text = "--- Selecciona una Suaje ---"
                };
                listadesuajes.Insert(0, descripciontip);
                return new SelectList(listadesuajes, "Value", "Text");
            }
        }

        public IEnumerable<SelectListItem> GetCintas()
        {
            using (var context = new ArticuloContext())
            {
                string cadena = "select * From Materiales where Obsoleto='0' order by clave";
                List<FormulaEspecializada.Materiales> listadecintas = context.Database.SqlQuery<FormulaEspecializada.Materiales>(cadena).ToList();

                List<SelectListItem> listadecintacombo = new List<SelectListItem>();
                foreach (FormulaEspecializada.Materiales item in listadecintas)
                {
                    SelectListItem art = new SelectListItem { Text = item.CLAVE + "   |   " + item.Descripcion, Value = item.Id.ToString() };

                    listadecintacombo.Add(art);
                }

                var descripciontip = new SelectListItem()
                {
                    Value = "0",
                    Text = "--- Selecciona una Cinta  ---"
                };
                listadecintacombo.Insert(0, descripciontip);
                return new SelectList(listadecintacombo, "Value", "Text");
            }
        }

        public IEnumerable<SelectListItem> GetCintasTermo()
        {
            using (var context = new ArticuloContext())
            {
                string cadena = "select * From Materiales where Obsoleto='0' and (Descripcion like '%PVC%' or Descripcion like '%PETG%')  order by clave";
                List<FormulaEspecializada.Materiales> listadecintas = context.Database.SqlQuery<FormulaEspecializada.Materiales>(cadena).ToList();

                List<SelectListItem> listadecintacombo = new List<SelectListItem>();
                foreach (FormulaEspecializada.Materiales item in listadecintas)
                {
                    SelectListItem art = new SelectListItem { Text = item.CLAVE + "   |   " + item.Descripcion, Value = item.Id.ToString() };

                    listadecintacombo.Add(art);
                }

                var descripciontip = new SelectListItem()
                {
                    Value = "0",
                    Text = "--- Selecciona una Cinta  ---"
                };
                listadecintacombo.Insert(0, descripciontip);
                return new SelectList(listadecintacombo, "Value", "Text");
            }
        }


        public IEnumerable<SelectListItem> GetCintasbyClave(int Seleccionado)
        {
            using (var context = new ArticuloContext())
            {
                string cadena = "select * From Materiales where Obsoleto='0' order by clave";
                List<FormulaEspecializada.Materiales> listadecintas = context.Database.SqlQuery<FormulaEspecializada.Materiales>(cadena).ToList();

                List<SelectListItem> listadecintacombo = new List<SelectListItem>();
                foreach (FormulaEspecializada.Materiales item in listadecintas)
                {
                    SelectListItem art = new SelectListItem { Text = item.CLAVE + " | " + item.Descripcion, Value = item.Id.ToString() };
                    if (item.Id == Seleccionado)
                    {
                        art.Selected = true;
                    }
                    listadecintacombo.Add(art);
                }

                var descripciontip = new SelectListItem()
                {
                    Value = "0",
                    Text = "--- Selecciona una Cinta  ---"
                };
                listadecintacombo.Insert(0, descripciontip);
                return new SelectList(listadecintacombo, "Value", "Text");
            }

        }
        public IEnumerable<SelectListItem> GetSuajes(int Seleccionado)
        {
            using (var context = new ArticuloContext())
            {
                string cadena = "select C.ID as IDCaracteristica, A.Cref,A.Descripcion from Articulo as A inner join Caracteristica as C on A.IDArticulo=C.Articulo_IDArticulo where A.IDTipoArticulo=2 and A.obsoleto='0' and C.obsoleto='0' and (A.IDFamilia=76 or A.IDFamilia=75 or A.IDFamilia=77 or A.IDFamilia=71 or A.IDFamilia =72 or A.IDFamilia=11 or A.IDFamilia=69) order by A.descripcion";
                List<suajeC> listadesuaje = context.Database.SqlQuery<suajeC>(cadena).ToList();

                List<SelectListItem> listadesuajes = new List<SelectListItem>();
                foreach (suajeC item in listadesuaje)
                {
                    SelectListItem art = new SelectListItem { Text = item.Cref + " | " + item.Descripcion, Value = item.IDCaracteristica.ToString() };
                    if (item.IDCaracteristica == Seleccionado)
                    {
                        art.Selected = true;
                    }
                    listadesuajes.Add(art);
                }

                var descripciontip = new SelectListItem()
                {
                    Value = "0",
                    Text = "--- Selecciona una Suaje ---"
                };
                listadesuajes.Insert(0, descripciontip);
                return new SelectList(listadesuajes, "Value", "Text");
            }
        }
        public IEnumerable<SelectListItem> GetSuajesProduccion(int Seleccionado)
        {
            using (var context = new ArticuloContext())
            {
                string cadena = "select C.ID as IDCaracteristica, A.Cref,A.Descripcion from Articulo as A inner join Caracteristica as C on A.IDArticulo=C.Articulo_IDArticulo where A.IDTipoArticulo=2 and A.obsoleto='0' and C.obsoleto='0' and (A.IDFamilia=76 or A.IDFamilia=75 or A.IDFamilia=77 or A.IDFamilia=71 or A.IDFamilia =72 or A.IDFamilia=11 or A.IDFamilia=69) order by A.descripcion";
                List<suajeC> listadesuaje = context.Database.SqlQuery<suajeC>(cadena).ToList();

                List<SelectListItem> listadesuajes = new List<SelectListItem>();
                foreach (suajeC item in listadesuaje)
                {
                    SelectListItem art = new SelectListItem { Text = item.Descripcion, Value = item.IDCaracteristica.ToString() };
                    if (item.IDCaracteristica == Seleccionado)
                    {
                        art.Selected = true;
                    }
                    listadesuajes.Add(art);
                }

                var descripciontip = new SelectListItem()
                {
                    Value = "0",
                    Text = "--- Selecciona una Suaje ---"
                };
                listadesuajes.Insert(0, descripciontip);
                return new SelectList(listadesuajes, "Value", "Text");
            }
        }

        public IEnumerable<SelectListItem> GetPlecas(int Seleccionado)
        {
            using (var context = new ArticuloContext())
            {
                string cadena = "select C.ID as IDCaracteristica, A.Cref,A.Descripcion from Articulo as A inner join Caracteristica as C on A.IDArticulo=C.Articulo_IDArticulo where A.IDTipoArticulo=2 and A.obsoleto='0' and C.obsoleto='0' and (A.IDFamilia=70 ) order by A.cref";
                List<suajeC> listadesuaje = context.Database.SqlQuery<suajeC>(cadena).ToList();

                List<SelectListItem> listadesuajes = new List<SelectListItem>();
                foreach (suajeC item in listadesuaje)
                {
                    SelectListItem art = new SelectListItem { Text = item.Cref + " | " + item.Descripcion, Value = item.IDCaracteristica.ToString() };
                    if (item.IDCaracteristica == Seleccionado)
                    {
                        art.Selected = true;
                    }
                    listadesuajes.Add(art);
                }

                var descripciontip = new SelectListItem()
                {
                    Value = "0",
                    Text = "--- Selecciona una Suaje ---"
                };
                listadesuajes.Insert(0, descripciontip);
                return new SelectList(listadesuajes, "Value", "Text");
            }
        }

        public IEnumerable<SelectListItem> GetCentros()
        {
            using (var context = new ArticuloContext())
            {
                string cadena = "select * from configplaneacion";
                ConfigPlaneacion configuracion = context.Database.SqlQuery<ConfigPlaneacion>(cadena).ToList().FirstOrDefault();

                List<SelectListItem> listadecentros = new List<SelectListItem>();
                var descripciontiptresimp = new SelectListItem()
                {
                    Value = configuracion.IDTuboImpresoTres.ToString(),
                    Text = "Centro de 3 Pulgadas Impreso"

                };


                listadecentros.Insert(0, descripciontiptresimp);

                var descripciontipunamediaimp = new SelectListItem()
                {
                    Value = configuracion.IDTuboImpresoUnamedia.ToString(),
                    Text = "Centro de 1 1/2 Pulgadas Impreso"

                };


                listadecentros.Insert(0, descripciontipunamediaimp);



                var descripciontipunaimp = new SelectListItem()
                {
                    Value = configuracion.IDTuboImpresoUna.ToString(),
                    Text = "Centro de 1 Pulgadas Impreso"

                };

                listadecentros.Insert(0, descripciontipunaimp);


                var descripciontiptres = new SelectListItem()
                {
                    Value = configuracion.IDTubotres.ToString(),
                    Text = "Centro de 3 Pulgadas"

                };


                listadecentros.Insert(0, descripciontiptres);

                var descripciontipunamedia = new SelectListItem()
                {
                    Value = configuracion.IDTubounamedia.ToString(),
                    Text = "Centro de 1 1/2 Pulgadas"

                };



                listadecentros.Insert(0, descripciontipunamedia);





                var descripciontip = new SelectListItem()
                {
                    Value = configuracion.IDTubouna.ToString(),
                    Text = "Centro de 1 Pulgada"

                };


                listadecentros.Insert(0, descripciontip);


                return new SelectList(listadecentros, "Value", "Text");
            }
        }

        public IEnumerable<SelectListItem> GetCentros(int seleccionado)
        {
            using (var context = new ArticuloContext())
            {
                string cadena = "select * from configplaneacion";
                ConfigPlaneacion configuracion = context.Database.SqlQuery<ConfigPlaneacion>(cadena).ToList().FirstOrDefault();




                List<SelectListItem> listadecentros = new List<SelectListItem>();

                var descripciontipseisimp = new SelectListItem()
                {
                    Value = configuracion.IDTuboSeis.ToString(),
                    Text = "Centro de 6 Pulgadas"

                };

                if (configuracion.IDTuboSeis == seleccionado)
                {
                    descripciontipseisimp.Selected = true;
                }

                listadecentros.Insert(0, descripciontipseisimp);




                var descripciontiptresimp = new SelectListItem()
                {

                    Value = configuracion.IDTuboImpresoTres.ToString(),
                    Text = "Centro de 3 Pulgadas Impreso"

                };
                if (configuracion.IDTuboImpresoTres == seleccionado)
                {
                    descripciontiptresimp.Selected = true;
                }

                listadecentros.Insert(0, descripciontiptresimp);

                var descripciontipunamediaimp = new SelectListItem()
                {
                    Value = configuracion.IDTuboImpresoUnamedia.ToString(),
                    Text = "Centro de 1 1/2 Pulgadas Impreso"

                };

                if (configuracion.IDTuboImpresoUnamedia == seleccionado)
                {
                    descripciontipunamediaimp.Selected = true;
                }

                listadecentros.Insert(0, descripciontipunamediaimp);



                var descripciontipunaimp = new SelectListItem()
                {
                    Value = configuracion.IDTuboImpresoUna.ToString(),
                    Text = "Centro de 1 Pulgadas Impreso"

                };

                if (configuracion.IDTuboImpresoUna == seleccionado)
                {
                    descripciontipunaimp.Selected = true;
                }

                listadecentros.Insert(0, descripciontipunaimp);


                var descripciontiptres = new SelectListItem()
                {
                    Value = configuracion.IDTubotres.ToString(),
                    Text = "Centro de 3 Pulgadas"

                };

                if (configuracion.IDTubotres == seleccionado)
                {
                    descripciontiptres.Selected = true;
                }



                listadecentros.Insert(0, descripciontiptres);

                var descripciontipunamedia = new SelectListItem()
                {
                    Value = configuracion.IDTubounamedia.ToString(),
                    Text = "Centro de 1 1/2 Pulgadas"

                };

                if (configuracion.IDTubounamedia == seleccionado)
                {
                    descripciontipunamedia.Selected = true;
                }

                listadecentros.Insert(0, descripciontipunamedia);





                var descripciontip = new SelectListItem()
                {
                    Value = configuracion.IDTubouna.ToString(),
                    Text = "Centro de 1 Pulgada"

                };

                if (configuracion.IDTubouna == seleccionado)
                {
                    descripciontip.Selected = true;
                }

                listadecentros.Insert(0, descripciontip);

                var descripcionna = new SelectListItem()
                {

                    Value = "0",
                    Text = "No Aplica"

                };
                listadecentros.Insert(0, descripcionna);


                return new SelectList(listadecentros, "Value", "Text");
            }

        }


        public IEnumerable<SelectListItem> GetCajas()
        {
            using (var context = new ArticuloContext())
            {
                string cadena = "select * from configplaneacion";
                ConfigPlaneacion configuracion = context.Database.SqlQuery<ConfigPlaneacion>(cadena).ToList().FirstOrDefault();

                List<SelectListItem> listadecaja = new List<SelectListItem>();
                var descripciontiptresimp = new SelectListItem()
                {
                    Value = configuracion.IDCajaCiega.ToString(),
                    Text = "Caja Ciega"

                };


                listadecaja.Insert(0, descripciontiptresimp);

                var descripciontipunamediaimp = new SelectListItem()
                {
                    Value = configuracion.IDCajaimpresa.ToString(),
                    Text = "Caja Impresa"

                };


                listadecaja.Insert(0, descripciontipunamediaimp);


                return new SelectList(listadecaja, "Value", "Text");
            }

        }
        public IEnumerable<SelectListItem> GetCajas(int seleccionado)
        {
            using (var context = new ArticuloContext())
            {
                string cadena = "select * from configplaneacion";
                ConfigPlaneacion configuracion = context.Database.SqlQuery<ConfigPlaneacion>(cadena).ToList().FirstOrDefault();


                List<SelectListItem> listadecaja = new List<SelectListItem>();
                var descripciontiptresimp = new SelectListItem()
                {
                    Value = configuracion.IDCajaCiega.ToString(),
                    Text = "Caja Ciega"

                };

                if (configuracion.IDCajaCiega == seleccionado)
                {
                    descripciontiptresimp.Selected = true;
                }

                listadecaja.Insert(0, descripciontiptresimp);

                var descripciontipunamediaimp = new SelectListItem()
                {
                    Value = configuracion.IDCajaimpresa.ToString(),
                    Text = "Caja Impresa"

                };

                if (configuracion.IDCajaimpresa == seleccionado)
                {
                    descripciontiptresimp.Selected = true;
                }


                listadecaja.Insert(0, descripciontipunamediaimp);



                return new SelectList(listadecaja, "Value", "Text");
            }





        }
    }


    [Table("Cotizaciones")]
    public class Cotizaciones
    {
        [Key]
        public int ID { get; set; }
        [DisplayName("Fecha")]
        public DateTime Fecha { get; set; }

        [DisplayName("Descripcion")]

        [Required]
        public string Descripcion { get; set; }

        [DisplayName("Ruta")]

        public string Ruta { get; set; }
        [DisplayName("Archivo")]
        [StringLength(100)]
        public string NombreArchivo { get; set; }
        [DisplayName("Usuario")]
        [StringLength(100)]
        public string Usuario { get; set; }

        public int suajenuevo { get; set; }

        public int thermo { get; set; }

        public int Solicitud { get; set; }

        public int tipo { get; set; }

        public int IDCliente { get; set; }

        public int IDClienteP { get; set; }

        public string contenido { get; set; }

    }



    public class ArchivoCotizadorContext : DbContext
    {
        public ArchivoCotizadorContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<Cotizaciones> cotizaciones { get; set; }
        public DbSet<ConfigPlaneacion> Configuracionpleaneacion { get; set; }

    }

    [Table("ConfigPlaneacion")]
    public class ConfigPlaneacion
    {
        [Key]
        public int ID { get; set; }
        public int IDMaquina7 { get; set; }
        public int IDMaquina10 { get; set; }
        public int IDMaquina13 { get; set; }
        public int IDEmbobinado { get; set; }
        public int IDPresentacion7 { get; set; }
        public int IDPresentacion10 { get; set; }
        public int IDPresentacion13 { get; set; }
        public int IDPresentacionE { get; set; }
        public int IDTubouna { get; set; }
        public int IDTubounamedia { get; set; }

        public int IDTuboSeis { get; set; }
        public int IDPresTuboSeis { get; set; }

        public int IDTubotres { get; set; }
        public int IDPresuna { get; set; }
        public int IDPresunamedia { get; set; }
        public int IDPrestres { get; set; }
        public int IDCajaCiega { get; set; }
        public int IDPresCajaCiega { get; set; }
        public int IDCajaimpresa { get; set; }
        public int IDPresCajaimpresa { get; set; }
        public int IDMO7 { get; set; }
        public int IDMO10 { get; set; }
        public int IDMO13 { get; set; }
        public int IDpresMO7 { get; set; }
        public int IDpresMO10 { get; set; }
        public int IDpresMO13 { get; set; }
        public int IDMOEmb { get; set; }
        public int IDpresMoEmb { get; set; }
        public int IDTuboImpresoUna { get; set; }
        public int IDPreTuboImpresouna { get; set; }
        public int IDTuboImpresoUnamedia { get; set; }
        public int IDPresTuboImpresoUnamedia { get; set; }
        public int IDTuboImpresoTres { get; set; }
        public int IDPresTuboImpresoTre { get; set; }
    }

}

