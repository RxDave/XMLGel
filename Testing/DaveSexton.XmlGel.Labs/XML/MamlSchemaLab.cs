using System;
using System.IO;
using System.Net;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using DaveSexton.XmlGel.Extensions;
using DaveSexton.XmlGel.Maml;
using DaveSexton.XmlGel.Xml;

namespace DaveSexton.XmlGel.Labs.XML
{
	public sealed class MamlSchemaLab : BaseConsoleLab
	{
		protected override void Main()
		{
			var schemasFolder = @"F:\Projects\DaveSexton Products\DaveSexton.XmlGel\Main\Artifacts\Schemas\MAML\Authoring\";

			XmlSchema schema;
			using (var stream = File.OpenRead(Path.Combine(schemasFolder, "developer.xsd")))
			{
				schema = XmlSchema.Read(stream, InvalidSchema);
			}

			var schemas = new XmlSchemaSet(new NameTable());

			schemas.Add(schema);

			var validator = new XmlFragmentSchemaValidator(
				new Resolver(schemasFolder, InvalidSchema),
				elementSchema => new XElement(elementSchema.GetXName()),
				_ => false,
				_ => null,
				schemas);

			validator.InvalidNode += validator_InvalidNode;
			validator.InvalidAttribute += validator_InvalidAttribute;

			var element = Maml.Maml.GetDefaultDocument(MamlDocumentKind.Conceptual);

			var elementSchemaType = validator.Schemas.GlobalTypes[new XmlQualifiedName("developerConceptualDocumentType", Maml.Maml.Namespace.NamespaceName)];

			validator.EnsureValid(element, _ => (XmlSchemaElement) elementSchemaType);
		}

		private void InvalidSchema(object sender, ValidationEventArgs e)
		{
			if (e.Severity == XmlSeverityType.Error)
			{
				TraceError(e.Exception);
			}
			else
			{
				TraceWarning(e.Message);
			}
		}

		private void validator_InvalidAttribute(XAttribute attribute)
		{
			TraceError("Invalid attribute: " + attribute.Name);
		}

		private void validator_InvalidNode(XNode node)
		{
			var element = node as XElement;

			if (element == null)
			{
				TraceError("Invalid text: " + node);
			}
			else
			{
				TraceError("Invalid element: " + element.Name);
			}
		}

		private sealed class Resolver : XmlResolver
		{
			public override ICredentials Credentials
			{
				set
				{
					throw new NotSupportedException();
				}
			}

			private readonly Uri actualBaseUri;
			private readonly ValidationEventHandler validationEventHandler;

			public Resolver(string basePath, ValidationEventHandler validationEventHandler)
			{
				this.actualBaseUri = new Uri(basePath, UriKind.Absolute);
				this.validationEventHandler = validationEventHandler;
			}

			public override Uri ResolveUri(Uri baseUri, string relativeUri)
			{
				return new Uri(actualBaseUri, relativeUri);
			}

			public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
			{
				using (var stream = File.OpenRead(absoluteUri.LocalPath))
				{
					return XmlSchema.Read(stream, validationEventHandler);
				}
			}
		}
	}
}