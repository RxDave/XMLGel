using System;

namespace DaveSexton.XmlGel.Maml.Documents.Visitors
{
	/* developerSDKTechnologyOverviewOrientationDocumentType (developer.xsd)
	 * 
	 * Sequence:
	 *	- summary (MamlSummary) (0..1)
	 *	- introduction (MamlIntroduction)
	 *	- inThisSection (MamlNamedSection)
	 *	- reference (MamlNamedSection) (0..1)
	 *	- relatedSections (MamlNamedSection) (0..1)
	 *	- externalResources (MamlExternalResources) (0..1)
	 */
	internal sealed class SdkOrientationDocumentToFlowDocumentVisitor : MamlToFlowDocumentVisitor
	{
		public SdkOrientationDocumentToFlowDocumentVisitor(MamlDocument document, Action uiContainerChanged)
			: base(document, uiContainerChanged)
		{
		}
	}
}