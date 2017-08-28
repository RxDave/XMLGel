using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace DaveSexton.XmlGel.Labs.XML
{
	public sealed class XmlBuilderLab : BaseConsoleLab
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
							new XmlSchemaElement() { Name = "second" }, 
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

		private static readonly XmlFixer fixer = new XmlFixer();

		protected override void Main()
		{
			RunExperiments();
		}

		public void ValidSequenceFragment()
		{
			EnsureValid(
				new XElement("root",
					new XElement("first", "text"),
					new XElement("second", "more text"),
					new XElement("third", 100d)));
		}

		public void ValidAttributedSequenceFragment()
		{
			EnsureValid(
				new XElement("root",
					new XAttribute("attrib", "Custom Value"),
					new XElement("first", "text"),
					new XElement("second", "more text"),
					new XElement("third", 100d)));
		}

		public void InvalidAttributedSequenceFragment()
		{
			EnsureValid(
				new XElement("root",
					new XAttribute("invalidAttrib", "Some Value"),
					new XElement("first", "text"),
					new XElement("second", "more text"),
					new XElement("third", 100d)));
		}

		public void InvalidEmptySequenceFragment()
		{
			EnsureValid(new XElement("root"));
		}

		public void InvalidSequenceFragment()
		{
			EnsureValid(
				new XElement("root",
					new XElement("first", "text"),
					new XElement("third", 100d),
					new XElement("second", "more text"),
					new XElement("third", 100d)));
		}

		public void InvalidSequenceFragment2()
		{
			EnsureValid(
				new XElement("root",
					new XElement("first", "text"),
					new XElement("third", 100d),
					new XElement("third", 100d),
					new XElement("second", "more text"),
					new XElement("third", 100d)));
		}

		public void InvalidSequenceFragment3()
		{
			EnsureValid(
				new XElement("root",
					new XElement("third", 100d),
					new XElement("first", "first text"),
					new XElement("third", 200d),
					new XElement("second", "first second text"),
					new XElement("first", "second first text"),
					new XElement("third", 300d)));
		}

		private void EnsureValid(XElement element)
		{
			TraceStatus("Before Analysis:");
			TraceLine();
			TraceLine(element);
			TraceLine();

			fixer.EnsureValid(element, (XmlSchemaElement) schema.Elements[new XmlQualifiedName("seq")]);

			TraceLine();
			TraceStatus("After Analysis:");
			TraceLine();
			TraceSuccess(element.ToString());
		}

		private sealed class XmlFixer
		{
			public static readonly object ErrorAnnotation = new object();

			private readonly XmlSchemaSet schemas;

			public XmlFixer()
				: this(new XmlSchemaSet())
			{
			}

			public XmlFixer(XmlSchemaSet schemas)
			{
				this.schemas = schemas;
			}

			public void EnsureValid(XElement element, XmlSchemaElement schemaElement)
			{
				var schemaType = schemaElement.ElementSchemaType;
				var complexType = schemaType as XmlSchemaComplexType;

				if (complexType != null)
				{
					EnsureRequiredAttributes(element, complexType);

					var sequence = complexType.Particle as XmlSchemaSequence;

					if (sequence != null)
					{
						EnsureSequence(element, sequence);
					}
					else
					{
						var choice = complexType.Particle as XmlSchemaChoice;

						if (choice != null)
						{
							EnsureChoice(element, choice);
						}
						else
						{
							throw new InvalidOperationException("Unsupported particle at element \"" + element.Name + "\".");
						}
					}
				}
				else
				{
					var simpleType = (XmlSchemaSimpleType) schemaType;

					if (element.IsEmpty)
					{
						element.SetValue(schemaElement.DefaultValue ?? GetDefault(simpleType) ?? string.Empty);
					}
				}
			}

			private void EnsureSequence(XElement element, XmlSchemaSequence sequence)
			{
				var node = element.FirstNode;

				var index = 0;
				var choices = GetChoicesInSequence(index, sequence);

				while (node != null)
				{
					var child = node as XElement;

					if (child != null)
					{


						node = child.NextNode;
						choices = GetChoicesInSequence(++index, sequence);
					}
				}

				XmlSchemaParticle nextRequired;
				while (choices.Count > 0 && (nextRequired = choices.FirstOrDefault(choice => choice.MinOccurs > 0)) != null)
				{
					element.Add(CreateDefaultElement(nextRequired));

					choices = GetChoicesInSequence(++index, sequence, nextRequired);
				}

				while (node != null)
				{
					var child = node as XElement;

					if (child != null)
					{
						child.AddAnnotation(ErrorAnnotation);
					}
				}
			}

			private XElement CreateDefaultElement(XmlSchemaParticle particle)
			{
				return null;
			}

			private IList<XmlSchemaParticle> GetChoicesInSequence(int startIndex, XmlSchemaSequence sequence, XmlSchemaParticle previousMatch = null)
			{
				var items = sequence.Items;

				var choice = new List<XmlSchemaParticle>(items.Count);

				for (int i = startIndex; i < items.Count; i++)
				{
					var particle = items[i] as XmlSchemaParticle;

					if (particle != null && particle.MaxOccurs > 0)
					{
						choice.Add(particle);
					}
				}

				return choice;
			}

			private void EnsureChoice(XElement element, XmlSchemaChoice choice)
			{
			}

			private void EnsureRequiredAttributes(XElement element, XmlSchemaComplexType complexType)
			{
				var actualAttributes = element.Attributes().Select(attribute => Tuple.Create(attribute.Name, (XmlSchemaAttribute) null));

				foreach (var missingRequiredAttribute in complexType.Attributes
					.Cast<XmlSchemaAttribute>()
					.Where(attribute => attribute.Use == XmlSchemaUse.Required)
					.Select(attribute => Tuple.Create((XNamespace) attribute.QualifiedName.Namespace + attribute.Name, attribute))
					.Except(actualAttributes, AttributeEqualityComparer.Default))
				{
					var attribute = missingRequiredAttribute.Item2;

					var name = missingRequiredAttribute.Item1;
					var value = attribute.DefaultValue ?? attribute.FixedValue;

					if (value != null)
					{
						element.SetAttributeValue(name, value);
					}
					else
					{
						element.SetAttributeValue(name, GetDefault(attribute.AttributeSchemaType));
					}
				}
			}

			private static object GetDefault(XmlSchemaSimpleType schemaType)
			{
				var restriction = schemaType.Content as XmlSchemaSimpleTypeRestriction;

				if (restriction != null)
				{
					if (restriction.Facets.Count > 0)
					{
						var facet = restriction.Facets[0] as XmlSchemaEnumerationFacet;

						if (facet != null)
						{
							return facet.Value;
						}
					}
				}

				switch (schemaType.TypeCode)
				{
					case XmlTypeCode.Boolean:
						return false;
					case XmlTypeCode.Byte:
					case XmlTypeCode.Decimal:
					case XmlTypeCode.Double:
					case XmlTypeCode.Float:
					case XmlTypeCode.Int:
					case XmlTypeCode.Integer:
					case XmlTypeCode.Long:
					case XmlTypeCode.NonNegativeInteger:
					case XmlTypeCode.NonPositiveInteger:
					case XmlTypeCode.PositiveInteger:
					case XmlTypeCode.Short:
					case XmlTypeCode.UnsignedByte:
					case XmlTypeCode.UnsignedInt:
					case XmlTypeCode.UnsignedLong:
					case XmlTypeCode.UnsignedShort:
						return 0;
					case XmlTypeCode.Date:
					case XmlTypeCode.DateTime:
						return DateTime.MinValue;
					case XmlTypeCode.Duration:
					case XmlTypeCode.Time:
						return TimeSpan.Zero;
					default:
						return string.Empty;
				}
			}

			private sealed class AttributeEqualityComparer : IEqualityComparer<Tuple<XName, XmlSchemaAttribute>>
			{
				public static readonly AttributeEqualityComparer Default = new AttributeEqualityComparer();

				private AttributeEqualityComparer()
				{
				}

				public bool Equals(Tuple<XName, XmlSchemaAttribute> x, Tuple<XName, XmlSchemaAttribute> y)
				{
					return x.Item1 == y.Item1;
				}

				public int GetHashCode(Tuple<XName, XmlSchemaAttribute> obj)
				{
					return obj.Item1.GetHashCode();
				}
			}
		}
	}
}