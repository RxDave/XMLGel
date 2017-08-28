using System;

namespace DaveSexton.XmlGel.Maml.Documents.Visitors
{
	/* developerSDKTechnologyOverviewCodeDirectoryDocumentType (developer.xsd)
	 * 
	 * Sequence:
	 *	- summary (MamlSummary) (0..1)
	 *	- introduction (MamlIntroduction)
	 *	- codeList (MamlNamedSection)
	 *	- relatedTopics (MamlRelatedTopics)
	 */
	internal sealed class SdkCodeDirectoryDocumentToFlowDocumentVisitor : MamlToFlowDocumentVisitor
	{
		public SdkCodeDirectoryDocumentToFlowDocumentVisitor(MamlDocument document, Action uiContainerChanged)
			: base(document, uiContainerChanged)
		{
		}
	}
}