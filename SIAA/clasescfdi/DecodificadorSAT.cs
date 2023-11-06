using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIAAPI.clasescfdi
{
    public static class DecodificadorSAT
    {
        public static string getRegimen(string _regimen)
        {
            if (_regimen == "601")
            {
                return "Regimen Fiscal: 601 General de Ley Personas Morales";
            }

            if (_regimen == "603")
            {
                return "Regimen Fiscal: 603 Personas Morales con Fines no Lucrativos";
            }

            if (_regimen == "605")
            {
                return "Regimen Fiscal: 605 Sueldos y Salarios e Ingresos Asimilados a Salarios";
            }

            if (_regimen == "606")
            {
                return "Regimen Fiscal: 606 Arrendamiento";
            }

            if (_regimen == "608")
            {
                return "Regimen Fiscal: 608 Demás ingresos";
            }

            if (_regimen == "609")
            {
                return "Regimen Fiscal: 609 Consolidación";
            }

            if (_regimen == "610")
            {
                return "Regimen Fiscal: 610 Residentes en el Extranjero sin Establecimiento Permanente en México";
            }

            if (_regimen == "611")
            {
                return "Regimen Fiscal: 611 Ingresos por Dividendos (socios y accionistas)";
            }

            if (_regimen == "612")
            {
                return "Regimen Fiscal: 612 Personas Físicas con Actividades Empresariales y Profesionales";
            }

            if (_regimen == "614")
            {
                return "Regimen Fiscal: 614 Ingresos por intereses";
            }

            if (_regimen == "616")
            {
                return "Regimen Fiscal: 616 Sin obligaciones fiscales";
            }
            if (_regimen == "620")
            {
                return "Regimen Fiscal: 620 Sociedades Cooperativas de Producción que optan por diferir sus ingresos";
            }

            if (_regimen == "621")
            {
                return "Regimen Fiscal: 621 Incorporación Fiscal";
            }

            if (_regimen == "622")
            {
                return "Regimen Fiscal: 622 Actividades Agrícolas, Ganaderas, Silvícolas y Pesqueras";
            }

            if (_regimen == "624")
            {
                return "Regimen Fiscal: 624 Coordinados";
            }

            if (_regimen == "628")
            {
                return "Regimen Fiscal: 628 Hidrocarburos";
            }

            if (_regimen == "607")
            {
                return "Regimen Fiscal: 607 Régimen de Enajenación o Adquisición de Bienes";
            }

            if (_regimen == "629")
            {
                return "Regimen Fiscal: 629 De los Regímenes Fiscales Preferentes y de las Empresas Multinacionales";
            }

            if (_regimen == "630")
            {
                return "Regimen Fiscal: 630 Enajenación de acciones en bolsa de valores";
            }

            if (_regimen == "615")
            {
                return "Regimen Fiscal: 615 Régimen de los ingresos por obtención de premios";
            }
            return _regimen;
        }

        public static string getUso(string _uso)
        {
            if (_uso == "G01")
            {
                return "G01: Adquisición de mercancias";
            }

            if (_uso == "G02")
            {
                return "G02: Devoluciones, descuentos o bonificaciones";
            }

            if (_uso == "G03")
            {
                return "G03: Gastos en general";
            }

            if (_uso == "I01")
            {
                return "I01: Construcciones";
            }

            if (_uso == "I02")
            {
                return "I02: Mobilario y equipo de oficina por inversiones";
            }

            if (_uso == "I03")
            {
                return "I03: Equipo de transporte";
            }

            if (_uso == "I04")
            {
                return "I04: Equipo de computo y accesorios";
            }

            if (_uso == "I05")
            {
                return "I05: Dados, troqueles, moldes, matrices y herramental";
            }

            if (_uso == "I06")
            {
                return "I06: Comunicaciones telefónicas";
            }

            if (_uso == "I08")
            {
                return "I08: Otra maquinaria y equipo";
            }

            if (_uso == "D01")
            {
                return "D01: Honorarios médicos, dentales y gastos hospitalarios.";
            }

            if (_uso == "D02")
            {
                return "D02: Gastos médicos por incapacidad o discapacidad";
            }

            if (_uso == "D03")
            {
                return "D03: Gastos funerales.";
            }

            if (_uso == "G01")
            {
                return "G01: Adquisición de mercancias";
            }

            if (_uso == "D05")
            {
                return "D05: Intereses reales efectivamente pagados por créditos hipotecarios (casa habitación).";
            }

            if (_uso == "D06")
            {
                return "D06: Aportaciones voluntarias al SAR.";
            }

            if (_uso == "D07")
            {
                return "D07: Primas por seguros de gastos médicos.";
            }

            if (_uso == "G01")
            {
                return "G01: Adquisición de mercancias";
            }

            if (_uso == "D08")
            {
                return "D08: Gastos de transportación escolar obligatoria.";
            }

            if (_uso == "D09")
            {
                return "D09: Depósitos en cuentas para el ahorro, primas que tengan como base planes de pensiones.";
            }


            if (_uso == "D10")
            {
                return "D10: Pagos por servicios educativos (colegiaturas)";
            }

            if (_uso == "P01")
            {
                return "P01: Por definir";
            }
            return _uso;


        }

        public static string getFormapago(string _forma)
        {
            if (_forma == "01")
            {
                return "01: Efectivo";
            }

            if (_forma == "02")
            {
                return "02: Cheque nominativo";
            }

            if (_forma == "03")
            {
                return "03: Transferencia electrónica de fondos";
            }

            if (_forma == "04")
            {
                return "04: Tarjeta de crédito";
            }

            if (_forma == "05")
            {
                return "05: Monedero electrónico";
            }

            if (_forma == "06")
            {
                return "06: Dinero electrónico";
            }

            if (_forma == "08")
            {
                return "08: Vales de despensa";
            }

            if (_forma == "12")
            {
                return "12: Dación en pago";
            }

            if (_forma == "13")
            {
                return "13: Pago por subrogación";
            }

            if (_forma == "14")
            {
                return "14: Pago por consignación";
            }

            if (_forma == "15")
            {
                return "15: Condonación";
            }

            if (_forma == "17")
            {
                return "17: Compensación";
            }

            if (_forma == "23")
            {
                return "23: Novación";
            }

            if (_forma == "24")
            {
                return "24: Confusión";
            }

            if (_forma == "25")
            {
                return "25: Remisión de deuda";
            }

            if (_forma == "26")
            {
                return "26: Prescripción o caducidad";
            }

            if (_forma == "01")
            {
                return "01: Efectivo";
            }

            if (_forma == "27")
            {
                return "27: A satisfacción del acreedor";
            }

            if (_forma == "28")
            {
                return "28: Tarjeta de débito";
            }

            if (_forma == "29")
            {
                return "29: Tarjeta de servicios";
            }

            if (_forma == "99")
            {
                return "99: Por definir";
            }
            return _forma;
        }

        public static string getMetodo(string _metodoPago)
        {
            if (_metodoPago == "PUE")
            {
                return "PUE: Pago en una sola exhibición";
            }

            if (_metodoPago == "PIP")
            {
                return "PIP: Pago en parcialidades o diferido";
            }

            if (_metodoPago == "PPD")
            {
                return "PPD: Pago en parcialidades o diferido";
            }

            return _metodoPago;
        }


        public static string getTipoderelacion(string Clave)
        {


            String Valor = "";
            if (Clave == "01")
            {
                Valor = "01 Nota de crédito de los documentos relacionados";
            }

            if (Clave == "02")
            {
                Valor = "02 Nota de débito de los documentos relacionados";
            }

            if (Clave == "03")
            {
                Valor = "03 Devolución de mercancía sobre facturas o traslados previos";
            }
            if (Clave == "04")
            {
                Valor = "04 Sustitución de los CFDI previos";
            }
            if (Clave == "05")
            {
                Valor = "05 Traslados de mercancias facturados previamente";
            }
            if (Clave == "06")
            {
                Valor = "06 Factura generada por los traslados previos";
            }
            if (Clave == "07")
            {
                Valor = "07 CFDI por aplicación de anticipo";
            }
            return Valor;

        }

        public static string getBanco(string RFC)
        {
            string STRBANCO = "";
            if (RFC == "BNM840515VB1") { STRBANCO = " BANAMEX"; }
            if (RFC == "BBA830831LJ2") { STRBANCO = " BBVA BANCOMER"; }
            if (RFC == "BSM970519DU8") { STRBANCO = " SANTANDER"; }
            if (RFC == "BNE820901682") { STRBANCO = " BANJERCITO"; }
            if (RFC == "HMI950125KG8") { STRBANCO = " HSBC"; }
            if (RFC == "BBA940707IE1") { STRBANCO = " BAJIO"; }
            if (RFC == "IBA950503GTA") { STRBANCO = " IXE"; }
            if (RFC == "BII931004P61") { STRBANCO = " INBURSA"; }
            if (RFC == "BIN931011519") { STRBANCO = " INTERACCIONES"; }
            if (RFC == "BAF950102JP5") { STRBANCO = " MIFEL"; }
            if (RFC == "SIN9412025I4") { STRBANCO = " SCOTIABANK"; }
            if (RFC == "BRM940216EQ6") { STRBANCO = " BANREGIO"; }
            if (RFC == "BIN940223KE0") { STRBANCO = " INVEX"; }
            if (RFC == "BAF950102JP5") { STRBANCO = " AFIRME"; }
            if (RFC == "BMN930209927") { STRBANCO = " BANORTE"; }
            if (RFC == "AEB960223JP7") { STRBANCO = " AMERICAN EXPRESS"; }
            if (RFC == "BJP950104LJ5") { STRBANCO = " JP MORGAN"; }
            if (RFC == "BMI9704113PA") { STRBANCO = " BMONEX"; }
            if (RFC == "BAI0205236Y8") { STRBANCO = " AZTECA"; }
            if (RFC == "BAM0511076B3") { STRBANCO = " AUTOFIN"; }
            if (RFC == "BCI001030ECA") { STRBANCO = " COMPARTAMOS"; }
            if (RFC == "BAF060524EV6") { STRBANCO = " BANCO FAMSA"; }
            if (RFC == "PBI061115SC6") { STRBANCO = " ACTINVER"; }
            if (RFC == "BWM0611132A9") { STRBANCO = " WAL - MART"; }
            if (RFC == "IBI061030GD4") { STRBANCO = " INTERBANCO"; }
            if (RFC == "BSI061110963") { STRBANCO = " BANCOPPEL"; }
            if (RFC == "CIB850918BN8") { STRBANCO = " CIBANCO"; }
            if (RFC == "BAN500901167") { STRBANCO = " BANSEFI"; }
            if (RFC == "BBA130722BR7") { STRBANCO = " Bancrea"; }
            return STRBANCO;
        }

        public static string getTipoComprobante(string _tC)
        {
            if (_tC == "I")
            {
                return "Ingreso";
            }

            if (_tC == "E")
            {
                return "Egreso";
            }
            return "Ingreso";
        }


        public static string getStatusUUID(string _Status)
        {
            if (_Status == "201")
            {
                return "UUID Enviado correctamente.";
            }
            if (_Status == "202")
            {
                return "UUID Previamente enviado (este código se recibe cuando la petición es enviada por segunda vez).";
            }
            if (_Status == "203")
            {
                return "UUID no corresponde al emisor.";
            }
            if (_Status == "204")
            {
                return "UUID no aplicable para cancelación";
            }
            if (_Status == "205")
            {
                return "UUID no existe.";
            }
            if (_Status == "301")
            {
                return "XML mal formado, esquema invalido (hace referencia al xml que contiene la petición de cancelación).";

            }
            if (_Status == "302")
            {
                return "El sello del emisor es inválido.";
            }
            if (_Status == "303")
            {
                return "Sello no corresponde a emisor.";
            }
            if (_Status == "304")
            {
                return "Certificado Revocado o caduco.";
            }
            if (_Status == "305")
            {
                return "La fecha de emisión no está dentro de la vigencia del CSD del Emisor.";
            }
            if (_Status == "306")
            {
                return "EL certificado no es de tipo CSD.";
            }
            if (_Status == "308")
            {
                return "Certificado no expedido por el SAT.";
            }
            if (_Status == "401")
            {
                return "Fecha y hora de generación fuera de rango, debe ser menor a 72 horas.";
            }
            if (_Status == "402")
            {
                return "RFC del emisor no se encuentra en el régimen de contribuyentes.";
            }
            if (_Status == "403")
            {
                return "La fecha de emisión no es posterior al 01 de enero 2011.";
            }

            return _Status;
        }

    }
}
