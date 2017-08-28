using System;

namespace DaveSexton.XmlGel.Maml.Documents.Visitors
{
	/* developerConceptualDocumentType (developer.xsd)
	 * 
	 * Sequence:
	 *	- summary (MamlSummary) (0..1)
	 *	- introduction (MamlIntroduction)
	 *	- section (MamlSection) (0..N)
	 *	- relatedTopics (MamlRelatedTopics)
	 */
	internal class ConceptualDocumentToFlowDocumentVisitor : MamlToFlowDocumentVisitor
	{
		public ConceptualDocumentToFlowDocumentVisitor(MamlDocument document, Action uiContainerChanged)
			: base(document, uiContainerChanged)
		{
		}
	}
}