using System;

namespace DaveSexton.XmlGel.Maml.Documents.Visitors
{
	/* developerSDKTechnologyOverviewTechnologySummaryDocumentType (developer.xsd)
	 * 
	 * Sequence:
	 *	- summary (MamlSummary) (0..1)
	 *	- introduction (MamlIntroduction)
	 *	- keywords (MamlNamedSection)
	 *	- namespaces (MamlNamedSection)
	 *	- relatedTechnologies (MamlNamedSection) (0..1)
	 *	- background (MamlNamedSection)
	 *	- implementingTechnologyNameClasses (MamlSection)
	 *	- technologyNameClassesAtAGlance (MamlSection)
	 *	- whatsNew (MamlNamedSection) (0..1)
	 *	- relatedTopics (MamlRelatedTopics)
	 */
	internal sealed class SdkSummaryDocumentToFlowDocumentVisitor : MamlToFlowDocumentVisitor
	{
		public SdkSummaryDocumentToFlowDocumentVisitor(MamlDocument document, Action uiContainerChanged)
			: base(document, uiContainerChanged)
		{
		}
	}
}