using System;
using System.Xml.Linq;

namespace DaveSexton.XmlGel.Maml.Documents.Visitors
{
	/* developerWhitePaperDocument (developer.xsd)
	 * 
	 * Sequence:
	 *	- summary (MamlSummary) (0..1)
	 *	- byline (MamlSingleParagraphSimpleContent)
	 *	- date (MamlSingleParagraphSimpleContent)
	 *	- summarySection (MamlNamedSection)
	 *	- downloadCenterLink (MamlNamedSection) (0..1)
	 *	- appliesTo (MamlNamedSection) (0..1)
	 *	- contents (MamlNamedSection) (0..1)
	 *	- introduction (MamlIntroduction)
	 *  - section (MamlSection) (0..N)
	 *  - conclusion (MamlNamedSection)
	 *	- relatedTopics (MamlRelatedTopics) (0..1)
	 *	- additionalInformation (MamlNamedSection) (0..1)
	 *	- authorBio (MamlNamedSection) (0..1)
	 */
	internal sealed class WhitePaperDocumentToFlowDocumentVisitor : MamlToFlowDocumentVisitor
	{
		public WhitePaperDocumentToFlowDocumentVisitor(MamlDocument document, Action uiContainerChanged)
			: base(document, uiContainerChanged)
		{
		}

		protected override MamlNode CreateDateNode(XElement element)
		{
			if (CurrentNode == null)
			{
				return new MamlSingleParagraphSimpleContent(element);
			}
			else
			{
				return base.CreateDateNode(element);
			}
		}
	}
}