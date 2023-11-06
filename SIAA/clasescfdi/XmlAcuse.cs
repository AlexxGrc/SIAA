using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace SIAAPI.clasescfdi
{

	[XmlRoot(ElementName = "Folios", Namespace = "http://cancelacfd.sat.gob.mx")]
	public class Folios
	{
		[XmlElement(ElementName = "UUID", Namespace = "http://cancelacfd.sat.gob.mx")]
		public string UUID { get; set; }
		[XmlElement(ElementName = "EstatusUUID", Namespace = "http://cancelacfd.sat.gob.mx")]
		public int EstatusUUID { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
	}

	[XmlRoot(ElementName = "CanonicalizationMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
	public class CanonicalizationMethod
	{
		[XmlAttribute(AttributeName = "Algorithm")]
		public string Algorithm { get; set; }
	}

	[XmlRoot(ElementName = "SignatureMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
	public class SignatureMethod
	{
		[XmlAttribute(AttributeName = "Algorithm")]
		public string Algorithm { get; set; }
	}

	[XmlRoot(ElementName = "Transform", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
	public class Transform
	{
		[XmlElement(ElementName = "XPath", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
		public string XPath { get; set; }
		[XmlAttribute(AttributeName = "Algorithm")]
		public string Algorithm { get; set; }
	}

	[XmlRoot(ElementName = "Transforms", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
	public class Transforms
	{
		[XmlElement(ElementName = "Transform", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
		public Transform Transform { get; set; }
	}

	[XmlRoot(ElementName = "DigestMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
	public class DigestMethod
	{
		[XmlAttribute(AttributeName = "Algorithm")]
		public string Algorithm { get; set; }
	}

	[XmlRoot(ElementName = "Reference", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
	public class Reference
	{
		[XmlElement(ElementName = "Transforms", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
		public Transforms Transforms { get; set; }
		[XmlElement(ElementName = "DigestMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
		public DigestMethod DigestMethod { get; set; }
		[XmlElement(ElementName = "DigestValue", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
		public string DigestValue { get; set; }
		[XmlAttribute(AttributeName = "URI")]
		public string URI { get; set; }
	}

	[XmlRoot(ElementName = "SignedInfo", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
	public class SignedInfo
	{
		[XmlElement(ElementName = "CanonicalizationMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
		public CanonicalizationMethod CanonicalizationMethod { get; set; }
		[XmlElement(ElementName = "SignatureMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
		public SignatureMethod SignatureMethod { get; set; }
		[XmlElement(ElementName = "Reference", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
		public Reference Reference { get; set; }
	}

	[XmlRoot(ElementName = "RSAKeyValue", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
	public class RSAKeyValue
	{
		[XmlElement(ElementName = "Modulus", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
		public string Modulus { get; set; }
		[XmlElement(ElementName = "Exponent", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
		public string Exponent { get; set; }
	}

	[XmlRoot(ElementName = "KeyValue", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
	public class KeyValue
	{
		[XmlElement(ElementName = "RSAKeyValue", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
		public RSAKeyValue RSAKeyValue { get; set; }
	}

	[XmlRoot(ElementName = "KeyInfo", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
	public class KeyInfo
	{
		[XmlElement(ElementName = "KeyName", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
		public string KeyName { get; set; }
		[XmlElement(ElementName = "KeyValue", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
		public KeyValue KeyValue { get; set; }
	}

	[XmlRoot(ElementName = "Signature", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
	public class Signature
	{
		[XmlElement(ElementName = "SignedInfo", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
		public SignedInfo SignedInfo { get; set; }
		[XmlElement(ElementName = "SignatureValue", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
		public string SignatureValue { get; set; }
		[XmlElement(ElementName = "KeyInfo", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
		public KeyInfo KeyInfo { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
		[XmlAttribute(AttributeName = "Id")]
		public string Id { get; set; }
	}

	[XmlRoot(ElementName = "Acuse")]
	public class Acuse
	{
		[XmlElement(ElementName = "Folios", Namespace = "http://cancelacfd.sat.gob.mx")]
		public Folios Folios { get; set; }
		[XmlElement(ElementName = "Signature", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
		public Signature Signature { get; set; }
		[XmlAttribute(AttributeName = "xsd", Namespace = "http://www.w3.org/2000/xmlns/")]
		public string Xsd { get; set; }
		[XmlAttribute(AttributeName = "xsi", Namespace = "http://www.w3.org/2000/xmlns/")]
		public string Xsi { get; set; }
		[XmlAttribute(AttributeName = "Fecha")]
		public DateTime Fecha { get; set; }
		[XmlAttribute(AttributeName = "RfcEmisor")]
		public string RfcEmisor { get; set; }
	}



}