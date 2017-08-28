using System;
using System.Windows.Documents;
using DaveSexton.XmlGel.Extensions;

namespace DaveSexton.XmlGel.Maml.Documents.Visitors
{
	/* developerErrorMessageDocument (developer.xsd)
	 * 
	 * Sequence:
	 *	- nonLocErrorTitle (MamlText) (0..1)
	 *	- secondaryErrorTitle (MamlText) (0..1)
	 *	- summary (MamlSummary) (0..1)
	 *	- introduction (MamlIntroduction)
	 *	- procedure (MamlProcedure) (0..N)
	 *  - codeExample (MamlCodeExample) (0..N)
	 *	- relatedTopics (MamlRelatedTopics) (0..1)
	 */
	internal sealed class ErrorMessageDocumentToFlowDocumentVisitor : MamlToFlowDocumentVisitor
	{
		public ErrorMessageDocumentToFlowDocumentVisitor(MamlDocument document, Action uiContainerChanged)
			: base(document, uiContainerChanged)
		{
		}

		public override TextElement Visit(MamlNonLocErrorTitle title, out TextElement contentContainer)
		{
			return contentContainer = new Paragraph()
			{
				Tag = title.Element.AsDataOnly(title)
			};
		}

		public override TextElement Visit(MamlSecondaryErrorTitle title, out TextElement contentContainer)
		{
			return contentContainer = new Paragraph()
			{
				Tag = title.Element.AsDataOnly(title)
			};
		}
	}
}