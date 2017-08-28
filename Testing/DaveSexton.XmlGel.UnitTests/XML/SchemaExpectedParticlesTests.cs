using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DaveSexton.XmlGel.UnitTests.Xml
{
	[TestClass]
	public sealed class SchemaExpectedParticlesTests
	{
		[TestMethod]
		public void Schema_Expected()
		{
			XElement target;

			var context = new SchemaExpectedParticlesTestContext(
					@"<sequence>"
				+ @"<element name=""summary"" minOccurs=""0"" />"
				+ @"<element name=""introduction"" />"
				+ @"<element name=""section"" minOccurs=""0"" maxOccurs=""unbounded"" />"
				+ @"<element name=""relatedTopics"" />"
				+ @"</sequence>",

				target = new XElement("introduction"),
				new XElement("relatedTopics"));

			context.ValidateBefore(target,
				context.GetDescendantElement("summary", 0),
				context.GetDescendantElement("introduction", 1));
		}
	}
}