using System;

namespace DaveSexton.XmlGel.Maml.Documents.Visitors
{
	/* developerReferenceWithoutSyntaxDocumentType (developer.xsd)
	 * 
	 * Sequence:
	 *	- summary (MamlSummary) (0..1)
	 *	- introduction (MamlIntroduction)
	 *	- Choice: (1..N)
	 *		- section (MamlSection)
	 *		- languageReferenceRemarks (MamlNamedSection)
	 *		- codeExample (MamlCodeExample)
	 *	- relatedTopics (MamlRelatedTopics)
	 */
	internal sealed class ReferenceWithoutSyntaxDocumentToFlowDocumentVisitor : MamlToFlowDocumentVisitor
	{
		public ReferenceWithoutSyntaxDocumentToFlowDocumentVisitor(MamlDocument document, Action uiContainerChanged)
			: base(document, uiContainerChanged)
		{
		}
	}
}