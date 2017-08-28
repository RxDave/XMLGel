using System;

namespace DaveSexton.XmlGel.Maml.Documents.Visitors
{
	/* developerTroubleshootingDocument (developer.xsd)
	 * 
	 * Sequence:
	 *	- summary (MamlSummary) (0..1)
	 *	- introduction (MamlIntroduction)
	 *	- section (MamlSection) (0..N)
	 *	- relatedTopics (MamlRelatedTopics)
	 */
	internal sealed class TroubleshootingDocumentToFlowDocumentVisitor : MamlToFlowDocumentVisitor
	{
		public TroubleshootingDocumentToFlowDocumentVisitor(MamlDocument document, Action uiContainerChanged)
			: base(document, uiContainerChanged)
		{
		}
	}
}