using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace OSPSuite.Utility.Xml
{
   public interface IXmlValidator
   {
      SchemaValidationResult Validate(string xmlString);
   }

   public class XmlValidator : IXmlValidator
   {
      private readonly XmlSchemaSet _xmlSchemaSet;

      public XmlValidator(string xmlSchemaPath, string xmlNamespace)
      {
         var xmlReader = XmlReader.Create(new StreamReader(xmlSchemaPath));
         _xmlSchemaSet = new XmlSchemaSet();
         _xmlSchemaSet.Add(xmlNamespace, xmlReader);
      }

      public SchemaValidationResult Validate(string xmlString)
      {
         var document = XDocument.Load(new StringReader(xmlString));
         var schemaValidationResult = new SchemaValidationResult();
         document.Validate(_xmlSchemaSet, (o, e) => schemaValidationResult.Messages.Add(e.Message));
         return schemaValidationResult;
      }
   }
}