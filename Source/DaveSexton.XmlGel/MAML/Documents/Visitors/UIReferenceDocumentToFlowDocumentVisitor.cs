using System;

namespace DaveSexton.XmlGel.Maml.Documents.Visitors
{
	/* developerUIReferenceDocument (developer.xsd)
	 * 
	 * Sequence:
	 *	- summary (MamlSummary) (0..1)
	 *	- introduction (MamlIntroduction)
	 *	- taskList (MamlNamedSection) (0..1)
	 *	- Choice: (1..N)
	 *		- section (MamlSection)
	 *		- uiElementList (MamlNamedSection)
	 *	- relatedTopics (MamlRelatedTopics)
	 */
	internal sealed class UIReferenceDocumentToFlowDocumentVisitor : MamlToFlowDocumentVisitor
	{
		public UIReferenceDocumentToFlowDocumentVisitor(MamlDocument document, Action uiContainerChanged)
			: base(document, uiContainerChanged)
		{
		}
	}
}