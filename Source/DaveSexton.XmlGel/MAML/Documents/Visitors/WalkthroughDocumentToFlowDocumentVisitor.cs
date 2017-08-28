using System;

namespace DaveSexton.XmlGel.Maml.Documents.Visitors
{
	/* developerWalkthroughDocumentType (developer.xsd)
	 * 
	 * Sequence:
	 *	- summary (MamlSummary) (0..1)
	 *	- introduction (MamlIntroduction)
	 *	- prerequisites (MamlNamedSection) (0..1)
	 *	- Choice: (1..N)
	 *		- procedure (MamlProcedure)
	 *		- section (MamlSection)
	 *	- nextSteps (MamlNamedSection) (0..1)
	 *	- relatedTopics (MamlRelatedTopics)
	 */
	internal sealed class WalkthroughDocumentToFlowDocumentVisitor : MamlToFlowDocumentVisitor
	{
		public WalkthroughDocumentToFlowDocumentVisitor(MamlDocument document, Action uiContainerChanged)
			: base(document, uiContainerChanged)
		{
		}
	}
}