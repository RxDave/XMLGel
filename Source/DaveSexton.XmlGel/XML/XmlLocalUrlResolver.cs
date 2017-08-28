using System;
using System.IO;
using System.Net;
using System.Xml;
using System.Xml.Schema;

namespace DaveSexton.XmlGel.Xml
{
	internal sealed class XmlLocalUrlResolver : XmlResolver
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

		public XmlLocalUrlResolver(string basePath, ValidationEventHandler validationEventHandler)
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