using System;
using System.Windows.Documents;
using DaveSexton.XmlGel.Extensions;

namespace DaveSexton.XmlGel.Maml.Documents.Visitors
{
	/* developerXmlReference (developer.xsd)
	 * 
	 * Sequence:
	 *	- summary (MamlSummary) (0..1)
	 *	- introduction (MamlIntroduction)
	 *	- schemaHierarchy (MamlSchemaHierarchy) (0..1)
	 *	- syntaxSection (MamlSyntaxSection)
	 *	- type (MamlSingleParagraphSimpleContent) (0..1)
	 *  - attributesandElements (MamlAttributesAndElements)
	 *  - textValue (MamlNamedSection) (0..1)
	 *  - remarks (MamlNamedSection) (0..1)
	 *  - codeExample (MamlCodeExample) (0..N)
	 *  - elementInformation (MamlElementInformation) (0..1)
	 *	- relatedTopics (MamlRelatedTopics)
	 */
	internal sealed class XmlReferenceToFlowDocumentVisitor : MamlToFlowDocumentVisitor
	{
		public XmlReferenceToFlowDocumentVisitor(MamlDocument document, Action uiContainerChanged)
			: base(document, uiContainerChanged)
		{
		}

		public override TextElement Visit(MamlSchemaHierarchy schema, out TextElement contentContainer)
		{
			return contentContainer = new Paragraph()
			{
				Tag = schema.Element.AsDataOnly(schema)
			};
		}

		public override TextElement Visit(MamlAttributesAndElements attributesAndElements, out TextElement contentContainer)
		{
			return contentContainer = new Section()
			{
				Tag = attributesAndElements.Element.AsDataOnly(attributesAndElements)
			};
		}

		public override TextElement Visit(MamlAttribute attribute, out TextElement contentContainer)
		{
			return contentContainer = new Section()
			{
				Tag = attribute.Element.AsDataOnly(attribute)
			};
		}

		public override TextElement Visit(MamlAttributes attributes, out TextElement contentContainer)
		{
			return contentContainer = new Section()
			{
				Tag = attributes.Element.AsDataOnly(attributes)
			};
		}

		public override TextElement Visit(MamlElementInformation information, out TextElement contentContainer)
		{
			return contentContainer = new Section()
			{
				Tag = information.Element.AsDataOnly(information)
			};
		}
	}
}