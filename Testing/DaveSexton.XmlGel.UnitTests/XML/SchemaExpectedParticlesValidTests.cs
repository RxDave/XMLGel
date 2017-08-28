using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DaveSexton.XmlGel.UnitTests.Xml
{
	/* These tests match the behavior of the ValidateChildren extension method.  Its behavior is to generate expected 
	 * particle lists based on the actual document, not just the current schema context.
	 * 
	 * For example, the SchemaExpected_Single_Single_Before test expects the expected particle list to be empty because 
	 * the summary element is already present in the document, even though technically the summary element is expected 
	 * at that position.
	 * 
	 * The reason for this behavior is that the expected particle list is used to populate the block palette tab rows in 
	 * the editor (the tabs that appear near the mouse pointer when hovering over an element).  We don't want to give a 
	 * user the option to insert another summary element before the summary element that is already present in the 
	 * document.  Unless of course the number of occurrences in the schema allows it, but that isn't the case in the 
	 * SchemaExpected_Single_Single_Before test in particular.
	 */

	[TestClass]
	public sealed class SchemaExpectedParticlesValidTests
	{
		[TestMethod]
		public void SchemaExpected_Empty_Empty_Before()
		{
			var context = SchemaExpectedParticlesTestContext.ExpectValid("<sequence />");
			context.ValidateBefore(null);
		}

		[TestMethod]
		public void SchemaExpected_Empty_Empty_After()
		{
			var context = SchemaExpectedParticlesTestContext.ExpectValid("<sequence />");
			context.ValidateAfter(null);
		}

		[TestMethod]
		public void SchemaExpected_Single_Single_Before()
		{
			XElement target;
			var context = SchemaExpectedParticlesTestContext.ExpectValid(
					@"<sequence>"
				+ @"<element name=""summary"" />"
				+ @"</sequence>",

				target = new XElement("summary"));

			context.ValidateBefore(target);
		}

		[TestMethod]
		public void SchemaExpected_Single_Single_After()
		{
			XElement target;
			var context = SchemaExpectedParticlesTestContext.ExpectValid(
					@"<sequence>"
				+ @"<element name=""summary"" />"
				+ @"</sequence>",

				target = new XElement("summary"));

			context.ValidateAfter(target);
		}

		[TestMethod]
		public void SchemaExpected_OptionalSingle_Empty_Before()
		{
			var context = SchemaExpectedParticlesTestContext.ExpectValid(
					@"<sequence>"
				+ @"<element name=""summary"" minOccurs=""0"" />"
				+ @"</sequence>");

			context.ValidateBefore(null,
				context.GetDescendantElement("summary", 0));
		}

		[TestMethod]
		public void SchemaExpected_OptionalSingle_Empty_After()
		{
			var context = SchemaExpectedParticlesTestContext.ExpectValid(
					@"<sequence>"
				+ @"<element name=""summary"" minOccurs=""0"" />"
				+ @"</sequence>");

			context.ValidateAfter(null,
				context.GetDescendantElement("summary", 0));
		}

		[TestMethod]
		public void SchemaExpected_OptionalSingle_Single_Before()
		{
			XElement target;
			var context = SchemaExpectedParticlesTestContext.ExpectValid(
					@"<sequence>"
				+ @"<element name=""summary"" minOccurs=""0"" />"
				+ @"</sequence>",

				target = new XElement("summary"));

			context.ValidateBefore(target);
		}

		[TestMethod]
		public void SchemaExpected_OptionalSingle_Single_After()
		{
			XElement target;
			var context = SchemaExpectedParticlesTestContext.ExpectValid(
					@"<sequence>"
				+ @"<element name=""summary"" minOccurs=""0"" />"
				+ @"</sequence>",

				target = new XElement("summary"));

			context.ValidateAfter(target);
		}

		[TestMethod]
		public void SchemaExpected_Complex()
		{
			XElement target;
			var context = SchemaExpectedParticlesTestContext.ExpectValid(
					@"<sequence>"
				+ @"<element name=""summary"" minOccurs=""0"" />"
				+ @"<element name=""introduction"" />"
				+ @"<element name=""section"" minOccurs=""0"" maxOccurs=""unbounded"" />"
				+ @"<element name=""relatedTopics"" />"
				+ @"</sequence>",

				new XElement("introduction"),
				target = new XElement("section"),
				new XElement("relatedTopics"));

			context.ValidateBefore(target,
				context.GetDescendantElement("section", 2));
		}
	}
}