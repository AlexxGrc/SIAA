using SIAAPI.Models.Administracion;
using System;
using System.Configuration;
using System.Web.Mvc;

namespace SIAAPI.Controllers.Administracion
{
    public class ConfigWEBController : Controller
    {
        // GET: ConfigWEB
        public ActionResult EditarConfiguracion()
        {
            Configuracion conf =llenaconfig();
         
            return View(conf);
        }
        [HttpPost]
        public ActionResult EditarConfiguracion(Configuracion elemento)
        {
            llenaconfigxml(elemento);
            SIAAPI.Properties.Settings.Default.Save();
            ConfigurationManager.RefreshSection("appSettings");
                      
            return View(elemento);
        }

        // GET: ConfigWEB
        public ActionResult EditarConfiguracionCotiza()
        {
            Configuracion conf = llenaconfig();
            
            return View(conf);
        }
        [HttpPost]
        public ActionResult EditarConfiguracionCotiza(Configuracion elemento)
        {



            llenaconfigxml(elemento);

            SIAAPI.Properties.Settings.Default.Save();
            ConfigurationManager.RefreshSection("appSettings");

            return RedirectToAction("CCotizador","Cotizador");
        }

        private Configuracion llenaconfig()
        {
            Configuracion conf = new Configuracion();
            //conf.AtributoAnchoCinta= Equals("AtributoAnchoCintaImpresa").ToString()
            conf.AtributoAnchoCinta = SIAAPI.Properties.Settings.Default.AtributoAnchoCinta;
            conf.AtributoAnchoCintaImpresa = SIAAPI.Properties.Settings.Default.AtributoAnchoCintaImpresa;
            conf.AtributoAvance = SIAAPI.Properties.Settings.Default.AtributoAvance;
            conf.AtributoCavidadesEje = SIAAPI.Properties.Settings.Default.AtributoCavidadesEje;
            conf.AtributoEje = SIAAPI.Properties.Settings.Default.AtributoEje;
            conf.AtributoGapAvance = SIAAPI.Properties.Settings.Default.AtributoGapAvance;
            conf.AtributoGapje = SIAAPI.Properties.Settings.Default.AtributoGapje;
            conf.ColorFuenteEncabezado = SIAAPI.Properties.Settings.Default.ColorFuenteEncabezado;
            conf.ColorReporte = SIAAPI.Properties.Settings.Default.ColorReporte;
            conf.CorreofirmaFactura = SIAAPI.Properties.Settings.Default.CorreofirmaFactura;
            conf.costodeempaque = SIAAPI.Properties.Settings.Default.costodeempaque;
            conf.CostoM2default = SIAAPI.Properties.Settings.Default.CostoM2default;
            conf.Eslogan = SIAAPI.Properties.Settings.Default.Eslogan;
            conf.factorcostoindirecto = SIAAPI.Properties.Settings.Default.factorcostoindirecto;
            conf.IDProveedorDesconocido = SIAAPI.Properties.Settings.Default.IDProveedorDesconocido;
            conf.Nombredelaaplicacion = SIAAPI.Properties.Settings.Default.Nombredelaaplicacion;
            conf.paginaoficial = SIAAPI.Properties.Settings.Default.paginaoficial;
            conf.Rango1gain = SIAAPI.Properties.Settings.Default.Rango1gain;
            conf.Rango2gain = SIAAPI.Properties.Settings.Default.Rango2gain;
            conf.Rango3gain = SIAAPI.Properties.Settings.Default.Rango3gain;
            conf.Rango4gain = SIAAPI.Properties.Settings.Default.Rango4gain;
            conf.SIAAPI_com_multifacturas_pac1_Timbrado_Remoto_de_INI = SIAAPI.Properties.Settings.Default.SIAAPI_com_multifacturas_pac1_Timbrado_Remoto_de_INI;
            conf.TCcotizador = SIAAPI.Properties.Settings.Default.TCcotizador;
            conf.TimbrarPrueba = SIAAPI.Properties.Settings.Default.TimbrarPrueba;
            conf.ValorIVA = SIAAPI.Properties.Settings.Default.ValorIVA;
            conf.ValorDienteFlex = SIAAPI.Properties.Settings.Default.ValorDienteFlex;
            conf.ValorDienteMacizo = SIAAPI.Properties.Settings.Default.ValorDienteMacizo;
            conf.ValorPulFlexible = SIAAPI.Properties.Settings.Default.ValorPulFlexible;
            conf.ValorPulMacizo = SIAAPI.Properties.Settings.Default.ValorPulMacizo;
            conf.ValorMtsXkg = SIAAPI.Properties.Settings.Default.ValorMtsXkg;
            return conf;
        }

        private void llenaconfigxml(Configuracion elemento)
        {
            Properties.Settings.Default["AtributoAnchoCinta"] = elemento.AtributoAnchoCinta;

            Properties.Settings.Default["AtributoAnchoCintaImpresa"] = elemento.AtributoAnchoCintaImpresa;
            Properties.Settings.Default["AtributoAvance"] = elemento.AtributoAvance;
            Properties.Settings.Default["AtributoCavidadesEje"] = elemento.AtributoCavidadesEje;
            Properties.Settings.Default["AtributoEje"] = elemento.AtributoEje;
            Properties.Settings.Default["AtributoGapAvance"] = elemento.AtributoGapAvance;
            Properties.Settings.Default["AtributoGapje"] = elemento.AtributoGapje;
            Properties.Settings.Default["ColorFuenteEncabezado"] = elemento.ColorFuenteEncabezado;
            Properties.Settings.Default["ColorReporte"] = elemento.ColorReporte;
            Properties.Settings.Default["CorreofirmaFactura"] = elemento.CorreofirmaFactura;
            Properties.Settings.Default["costodeempaque"] = elemento.costodeempaque;
            Properties.Settings.Default["CostoM2default"] = elemento.CostoM2default;
            Properties.Settings.Default["Eslogan"] = elemento.Eslogan;
            Properties.Settings.Default["factorcostoindirecto"] = elemento.factorcostoindirecto;
            Properties.Settings.Default["IDProveedorDesconocido"] = elemento.IDProveedorDesconocido;
            Properties.Settings.Default["Nombredelaaplicacion"] = elemento.Nombredelaaplicacion;
            Properties.Settings.Default["paginaoficial"] = elemento.paginaoficial;
            Properties.Settings.Default["Rango1gain"] = elemento.Rango1gain;
            Properties.Settings.Default["Rango2gain"] = elemento.Rango2gain;
            Properties.Settings.Default["Rango3gain"] = elemento.Rango3gain;
            Properties.Settings.Default["Rango4gain"] = elemento.Rango4gain;
            Properties.Settings.Default["SIAAPI_com_multifacturas_pac1_Timbrado_Remoto_de_INI"] = elemento.SIAAPI_com_multifacturas_pac1_Timbrado_Remoto_de_INI;
            Properties.Settings.Default["TCcotizador"] = elemento.TCcotizador;
            Properties.Settings.Default["TimbrarPrueba"] = elemento.TimbrarPrueba;
            Properties.Settings.Default["ValorIVA"] = elemento.ValorIVA;
            Properties.Settings.Default["ValorDienteFlex"] = decimal.Parse(elemento.ValorDienteFlex.ToString());
            Properties.Settings.Default["ValorDienteMacizo"] = decimal.Parse(elemento.ValorDienteMacizo.ToString());
            Properties.Settings.Default["ValorPulMacizo"] = decimal.Parse(elemento.ValorPulMacizo.ToString());
            Properties.Settings.Default["ValorPulFlexible"] = decimal.Parse(elemento.ValorPulFlexible.ToString());
            Properties.Settings.Default["ValorMtsXkg"] = Int32.Parse( elemento.ValorMtsXkg.ToString());

            return ;
        }


    }


}