<?xml version="1.0" encoding="ISO-8859-1"?>
<definitions xmlns:SOAP-ENV="http://schemas.xmlsoap.org/soap/envelope/" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:SOAP-ENC="http://schemas.xmlsoap.org/soap/encoding/" xmlns:tns="urn:wservicewsdl" xmlns:soap="http://schemas.xmlsoap.org/wsdl/soap/" xmlns:wsdl="http://schemas.xmlsoap.org/wsdl/" xmlns="http://schemas.xmlsoap.org/wsdl/" targetNamespace="urn:wservicewsdl">
<types>
<xsd:schema targetNamespace="urn:wservicewsdl"
>
 <xsd:import namespace="http://schemas.xmlsoap.org/soap/encoding/" />
 <xsd:import namespace="http://schemas.xmlsoap.org/wsdl/" />
 <xsd:complexType name="datos">
  <xsd:all>
   <xsd:element name="usuario" type="xsd:string"/>
   <xsd:element name="pass" type="xsd:string"/>
   <xsd:element name="accion" type="xsd:string"/>
   <xsd:element name="produccion" type="xsd:string"/>
   <xsd:element name="uuid" type="xsd:string"/>
   <xsd:element name="rfc" type="xsd:string"/>
   <xsd:element name="password" type="xsd:string"/>
   <xsd:element name="motivo" type="xsd:string"/>
   <xsd:element name="folioSustitucion" type="xsd:string"/>
   <xsd:element name="b64Cer" type="xsd:string"/>
   <xsd:element name="b64Key" type="xsd:string"/>
  </xsd:all>
 </xsd:complexType>
 <xsd:complexType name="respuesta">
  <xsd:all>
   <xsd:element name="codigo_mf_numero" type="xsd:string"/>
   <xsd:element name="codigo_mf_texto" type="xsd:string"/>
   <xsd:element name="mensaje_original" type="xsd:string"/>
   <xsd:element name="acuse" type="xsd:string"/>
   <xsd:element name="status" type="xsd:string"/>
   <xsd:element name="uuid" type="xsd:string"/>
   <xsd:element name="codigo_respuesta_sat" type="xsd:string"/>
   <xsd:element name="codigo_respuesta_sat_texto" type="xsd:string"/>
   <xsd:element name="codigo_respuesta_sat_texto_descripcion" type="xsd:string"/>
   <xsd:element name="produccion" type="xsd:string"/>
   <xsd:element name="accion" type="xsd:string"/>
  </xsd:all>
 </xsd:complexType>
 <xsd:complexType name="datosRelacionados">
  <xsd:all>
   <xsd:element name="usuario" type="xsd:string"/>
   <xsd:element name="pass" type="xsd:string"/>
   <xsd:element name="accion" type="xsd:string"/>
   <xsd:element name="produccion" type="xsd:string"/>
   <xsd:element name="uuid" type="xsd:string"/>
   <xsd:element name="rfc_receptor" type="xsd:string"/>
   <xsd:element name="password" type="xsd:string"/>
   <xsd:element name="b64Cer_receptor" type="xsd:string"/>
   <xsd:element name="b64Key_receptor" type="xsd:string"/>
  </xsd:all>
 </xsd:complexType>
 <xsd:complexType name="respuestaCCancelar">
  <xsd:all>
   <xsd:element name="codigo_mf_numero" type="xsd:string"/>
   <xsd:element name="codigo_mf_texto" type="xsd:string"/>
   <xsd:element name="mensaje_original" type="xsd:string"/>
   <xsd:element name="status" type="xsd:string"/>
   <xsd:element name="json_uuids" type="xsd:string"/>
   <xsd:element name="codigo_respuesta_sat" type="xsd:string"/>
   <xsd:element name="codigo_respuesta_sat_texto" type="xsd:string"/>
   <xsd:element name="codigo_respuesta_sat_texto_descripcion" type="xsd:string"/>
   <xsd:element name="produccion" type="xsd:string"/>
   <xsd:element name="accion" type="xsd:string"/>
  </xsd:all>
 </xsd:complexType>
</xsd:schema>
</types>
<message name="cancelarCfdiRequest">
  <part name="datos" type="tns:datos" /></message>
<message name="cancelarCfdiResponse">
  <part name="return" type="tns:respuesta" /></message>
<message name="aceptarCancelarCfdiRequest">
  <part name="datos" type="tns:datos" /></message>
<message name="aceptarCancelarCfdiResponse">
  <part name="return" type="tns:respuesta" /></message>
<message name="rechazarCancelarCfdiRequest">
  <part name="datos" type="tns:datos" /></message>
<message name="rechazarCancelarCfdiResponse">
  <part name="return" type="tns:respuesta" /></message>
<message name="consultarCancelarCfdiRequest">
  <part name="datos" type="tns:datos" /></message>
<message name="consultarCancelarCfdiResponse">
  <part name="return" type="tns:respuestaCCancelar" /></message>
<message name="consultarCfdiRelacionadoRequest">
  <part name="datos" type="tns:datosRelacionados" /></message>
<message name="consultarCfdiRelacionadoResponse">
  <part name="return" type="tns:respuesta" /></message>
<portType name="Cancelar cfdi 4.0 SATPortType">
  <operation name="cancelarCfdi">
    <documentation>Descripcion de uso de la funcion : cancelarCfdi</documentation>
    <input message="tns:cancelarCfdiRequest"/>
    <output message="tns:cancelarCfdiResponse"/>
  </operation>
  <operation name="aceptarCancelarCfdi">
    <documentation>Descripcion de uso de la funcion : aceptarCancelarCfdi</documentation>
    <input message="tns:aceptarCancelarCfdiRequest"/>
    <output message="tns:aceptarCancelarCfdiResponse"/>
  </operation>
  <operation name="rechazarCancelarCfdi">
    <documentation>Descripcion de uso de la funcion : rechazarCancelarCfdi</documentation>
    <input message="tns:rechazarCancelarCfdiRequest"/>
    <output message="tns:rechazarCancelarCfdiResponse"/>
  </operation>
  <operation name="consultarCancelarCfdi">
    <documentation>Descripcion de uso de la funcion : consultarCancelarCfdi</documentation>
    <input message="tns:consultarCancelarCfdiRequest"/>
    <output message="tns:consultarCancelarCfdiResponse"/>
  </operation>
  <operation name="consultarCfdiRelacionado">
    <documentation>Descripcion de uso de la funcion : consultarCfdiRelacionado</documentation>
    <input message="tns:consultarCfdiRelacionadoRequest"/>
    <output message="tns:consultarCfdiRelacionadoResponse"/>
  </operation>
</portType>
<binding name="Cancelar cfdi 4.0 SATBinding" type="tns:Cancelar cfdi 4.0 SATPortType">
  <soap:binding style="rpc" transport="http://schemas.xmlsoap.org/soap/http"/>
  <operation name="cancelarCfdi">
    <soap:operation soapAction="urn:wservicewsdl#cancelarCfdi" style="rpc"/>
    <input><soap:body use="encoded" namespace="urn:wservicewsdl" encodingStyle="http://schemas.xmlsoap.org/soap/encoding/"/></input>
    <output><soap:body use="encoded" namespace="urn:wservicewsdl" encodingStyle="http://schemas.xmlsoap.org/soap/encoding/"/></output>
  </operation>
  <operation name="aceptarCancelarCfdi">
    <soap:operation soapAction="urn:wservicewsdl#aceptarCancelarCfdi" style="rpc"/>
    <input><soap:body use="encoded" namespace="urn:wservicewsdl" encodingStyle="http://schemas.xmlsoap.org/soap/encoding/"/></input>
    <output><soap:body use="encoded" namespace="urn:wservicewsdl" encodingStyle="http://schemas.xmlsoap.org/soap/encoding/"/></output>
  </operation>
  <operation name="rechazarCancelarCfdi">
    <soap:operation soapAction="urn:wservicewsdl#rechazarCancelarCfdi" style="rpc"/>
    <input><soap:body use="encoded" namespace="urn:wservicewsdl" encodingStyle="http://schemas.xmlsoap.org/soap/encoding/"/></input>
    <output><soap:body use="encoded" namespace="urn:wservicewsdl" encodingStyle="http://schemas.xmlsoap.org/soap/encoding/"/></output>
  </operation>
  <operation name="consultarCancelarCfdi">
    <soap:operation soapAction="urn:wservicewsdl#consultarCancelarCfdi" style="rpc"/>
    <input><soap:body use="encoded" namespace="urn:wservicewsdl" encodingStyle="http://schemas.xmlsoap.org/soap/encoding/"/></input>
    <output><soap:body use="encoded" namespace="urn:wservicewsdl" encodingStyle="http://schemas.xmlsoap.org/soap/encoding/"/></output>
  </operation>
  <operation name="consultarCfdiRelacionado">
    <soap:operation soapAction="urn:wservicewsdl#consultarCfdiRelacionado" style="rpc"/>
    <input><soap:body use="encoded" namespace="urn:wservicewsdl" encodingStyle="http://schemas.xmlsoap.org/soap/encoding/"/></input>
    <output><soap:body use="encoded" namespace="urn:wservicewsdl" encodingStyle="http://schemas.xmlsoap.org/soap/encoding/"/></output>
  </operation>
</binding>
<service name="Cancelar cfdi 4.0 SAT">
  <port name="Cancelar cfdi 4.0 SATPort" binding="tns:Cancelar cfdi 4.0 SATBinding">
    <soap:address location="http://pac1.multifacturas.com/cancelacion2022/index.php"/>
  </port>
</service>
</definitions> 