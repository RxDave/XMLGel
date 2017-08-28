using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using DaveSexton.XmlGel.Extensions;
using DaveSexton.XmlGel.Xml;

namespace DaveSexton.XmlGel.Labs.XML
{
	public sealed class SchemaValidationLab : BaseConsoleLab
	{
		private static readonly XmlSchema schema = new XmlSchema()
		{
			Items =
			{
				new XmlSchemaComplexType()
				{
					Name = "seq",
					Attributes =
					{
						new XmlSchemaAttribute() { Name = "attrib", DefaultValue = "Default Value" }
					}, 
					Particle = new XmlSchemaSequence()
					{
						Items =
						{
							new XmlSchemaElement() { Name = "first" }, 
							new XmlSchemaElement() { Name = "second", MaxOccurs = 2 }, 
							new XmlSchemaElement() { Name = "third" }
						}
					}
				},
				new XmlSchemaElement()
				{
					Name = "root",
					SchemaTypeName = new XmlQualifiedName("seq")
				}
			}
		};

		private static readonly XmlSchemaSet schemas = CreateXmlSchemaSet();

		private static readonly XmlFragmentSchemaValidator validator = new XmlFragmentSchemaValidator(
			null,
			elementSchema => new XElement(elementSchema.GetXName()),
			_ => false,
			_ => null,
			schemas);

		static SchemaValidationLab()
		{
			validator.InvalidNode += TraceInvalidNode;
			validator.InvalidAttribute += TraceInvalidAttribute;
		}

		private static XmlSchemaSet CreateXmlSchemaSet()
		{
			var value = new XmlSchemaSet(new NameTable());

			value.Add(schema);

			return value;
		}

		protected override void Main()
		{
			RunExperiments();
		}

		public void Valid()
		{
			ValidateShallow(
				new XElement("root",
					new XElement("first"),
					new XElement("second"),
					new XElement("third")));
		}

		public void Valid2()
		{
			ValidateShallow(
				new XElement("root",
					new XElement("first"),
					new XElement("second"),
					new XElement("second"),
					new XElement("third")));
		}

		public void MissingAllElements()
		{
			ValidateShallow(new XElement("root"));
		}

		public void MissingLastElement()
		{
			ValidateShallow(
				new XElement("root",
					new XElement("first"),
					new XElement("second")));
		}

		public void WrongElement()
		{
			ValidateShallow(
				new XElement("root",
					new XElement("first"),
					new XElement("third"),
					new XElement("second")));
		}

		public void TooManyElements()
		{
			ValidateShallow(
				new XElement("root",
					new XElement("first"),
					new XElement("second"),
					new XElement("second"),
					new XElement("second"),
					new XElement("third")));
		}

		public void TooFewElements()
		{
			ValidateShallow(
				new XElement("root",
					new XElement("first"),
					new XElement("third")));
		}

		public void ValidAttribute()
		{
			ValidateShallow(
				new XElement("root",
					new XAttribute("attrib", "Some Value"),
					new XElement("first"),
					new XElement("second"),
					new XElement("third")));
		}

		public void InvalidAttributed()
		{
			ValidateShallow(
				new XElement("root",
					new XAttribute("invalid-attrib", "Some Value"),
					new XElement("first"),
					new XElement("second"),
					new XElement("third")));
		}

		private void ValidateShallow(XElement element)
		{
			TraceStatus("Before Validation:");
			TraceLine();
			TraceLine(element);
			TraceLine();

			validator.EnsureValid(element, _ => (XmlSchemaElement) schema.Elements[new XmlQualifiedName("root")]);

			TraceLine();
			TraceStatus("After Validation:");
			TraceLine();
			TraceSuccess(element.ToString());
		}

		private static void TraceInvalidNode(XNode node)
		{
			var lab = DaveSexton.Labs.LabTraceSource.Default;

			var element = node as XElement;

			if (element != null)
			{
				lab.TraceError("Invalid element: " + element.Name);
			}
			else
			{
				lab.TraceError("Invalid text node: " + node);
			}

			lab.TraceLine();
		}

		private static void TraceInvalidAttribute(XAttribute attribute)
		{
			var lab = DaveSexton.Labs.LabTraceSource.Default;

			lab.TraceError("Invalid attribute: " + attribute.Name);
			lab.TraceLine();
		}
	}
}