using System;

namespace DaveSexton.XmlGel.Maml.Documents.Visitors
{
	/* developerConceptualDocumentType (developer.xsd)
	 */
	internal sealed class SdkArchitectureDocumentToFlowDocumentVisitor : ConceptualDocumentToFlowDocumentVisitor
	{
		public SdkArchitectureDocumentToFlowDocumentVisitor(MamlDocument document, Action uiContainerChanged)
			: base(document,  uiContainerChanged)
		{
		}
	}
}