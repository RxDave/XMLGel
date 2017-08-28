using System;

namespace DaveSexton.XmlGel.Maml.Documents.Visitors
{
	/* developerOrientationDocumentType (developer.xsd)
	 * 
	 * Sequence:
	 *	- summary (MamlSummary) (0..1)
	 *	- introduction (MamlIntroduction)
	 *	- inThisSection (MamlNamedSection) (0..1)
	 *	- reference (MamlNamedSection) (0..1)
	 *	- relatedSections (MamlNamedSection) (0..1)
	 *	- externalResources (MamlExternalResources) (0..1)
	 *	- relatedTopics (MamlRelatedTopics) (0..1)
	 */
	internal sealed class OrientationDocumentToFlowDocumentVisitor : MamlToFlowDocumentVisitor
	{
		public OrientationDocumentToFlowDocumentVisitor(MamlDocument document, Action uiContainerChanged)
			: base(document, uiContainerChanged)
		{
		}
	}
}