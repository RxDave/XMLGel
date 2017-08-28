using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using DaveSexton.XmlGel.Extensions;
using DaveSexton.XmlGel.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DaveSexton.XmlGel.UnitTests.Xml
{
	public sealed class SchemaExpectedParticlesTestContext
	{
		private readonly bool isValidExpected;
		private readonly XmlSchemaSet schemaSet;
		private readonly XmlSchemaElement rootSchema;
		private readonly XElement root;

		private SchemaExpectedParticlesTestContext(bool isValidExpected, string groupSchema, params object[] actualContent)
		{
			this.isValidExpected = isValidExpected;

			schemaSet = new XmlSchemaSet();

			var schema = XmlSchema.Read(new StringReader(
					@"<schema xmlns=""http://www.w3.org/2001/XMLSchema"" blockDefault=""#all"" attributeFormDefault=""unqualified"" xml:lang=""en"">"
				+ @"<element name=""root"">"
				+ @"<complexType mixed=""false"">"
				+ groupSchema
				+ @"</complexType></element></schema>"),
				(sender, e) => { });

			schemaSet.Add(schema);

			schemaSet.Compile();

			rootSchema = (XmlSchemaElement) schema.Elements[new XmlQualifiedName("root")];

			root = new XElement("root", actualContent);
		}

		public static SchemaExpectedParticlesTestContext ExpectValid(string groupSchema, params object[] actualContent)
		{
			return new SchemaExpectedParticlesTestContext(true, groupSchema, actualContent);
		}

		public static SchemaExpectedParticlesTestContext ExpectInvalid(string groupSchema, params object[] actualContent)
		{
			return new SchemaExpectedParticlesTestContext(false, groupSchema, actualContent);
		}

		/* Just specifying the expectedName alone is not enough because it's possible to have sibling schema elements with the same 
		 * names in an unambiguous manner; e.g., <root><a /><b /><a /></root>.  Though specifying the expectedName is safer than 
		 * specifying the index alone.
		 */
		public XmlSchemaElement GetDescendantElement(string expectedName, params int[] indexPath)
		{
			if (indexPath == null || indexPath.Length == 0)
			{
				throw new ArgumentOutOfRangeException();
			}

			XmlSchemaElement match = null;
			XmlSchemaParticle currentParticle;

			var group = (XmlSchemaGroupBase) ((XmlSchemaComplexType) rootSchema.ElementSchemaType).Particle;

			for (int i = 0; i < indexPath.Length; i++)
			{
				var index = indexPath[i];

				currentParticle = (XmlSchemaParticle) group.Items[index];

				group = currentParticle as XmlSchemaGroupBase;

				if (group == null)
				{
					Assert.IsTrue(i + 1 == indexPath.Length);

					match = (XmlSchemaElement) currentParticle;
					break;
				}
			}

			if (match == null || match.Name != expectedName)
			{
				throw new InvalidOperationException("The expected element \"" + expectedName + "\" was not found at the specified index path.");
			}

			return match;
		}

		public void ValidateBefore(XElement target, params XmlSchemaParticle[] expectedElementsBetweenNodes)
		{
			Validate(target, true, expectedElementsBetweenNodes);
		}

		public void ValidateAfter(XElement target, params XmlSchemaParticle[] expectedElementsBetweenNodes)
		{
			Validate(target, false, expectedElementsBetweenNodes);
		}

		private void Validate(XElement target, bool validateBeforeTarget, params XmlSchemaParticle[] expectedParticles)
		{
			XmlSchemaParticlesExpected actualExpectedBeforeTarget, actualExpectedAfterTarget;

			var isValid = rootSchema.ValidateChildren(schemaSet, root, out actualExpectedBeforeTarget, out actualExpectedAfterTarget, target);

			Assert.AreEqual(
				expected: isValidExpected,
				actual: isValid,
				message: "Validation unexpectedly " + (isValid ? "succeeded" : "failed") + " for element: " + target);

			Assert.IsTrue(actualExpectedBeforeTarget != null, "The expectedBeforeTarget object was not assigned.");
			Assert.IsTrue(actualExpectedAfterTarget != null, "The expectedAfterTarget object was not assigned");

			var actual = validateBeforeTarget ? actualExpectedBeforeTarget : actualExpectedAfterTarget;
			var expected = new XmlSchemaParticlesExpected(expectedParticles);

			CollectionAssert.AreEqual(expected: expected, actual: actual);
		}
	}
}