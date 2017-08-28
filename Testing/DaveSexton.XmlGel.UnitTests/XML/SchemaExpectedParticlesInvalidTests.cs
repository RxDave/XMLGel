using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DaveSexton.XmlGel.UnitTests.Xml
{
	[TestClass]
	public sealed class SchemaExpectedParticlesInvalidTests
	{
		[TestMethod]
		public void SchemaExpected_Single_Empty_Before()
		{
			var context = SchemaExpectedParticlesTestContext.ExpectInvalid(
					@"<sequence>"
				+ @"<element name=""summary"" />"
				+ @"</sequence>");

			context.ValidateBefore(null,
				context.GetDescendantElement("summary", 0));
		}

		[TestMethod]
		public void SchemaExpected_Single_Empty_After()
		{
			var context = SchemaExpectedParticlesTestContext.ExpectInvalid(
					@"<sequence>"
				+ @"<element name=""summary"" />"
				+ @"</sequence>");

			context.ValidateAfter(null,
				context.GetDescendantElement("summary", 0));
		}
	}
}