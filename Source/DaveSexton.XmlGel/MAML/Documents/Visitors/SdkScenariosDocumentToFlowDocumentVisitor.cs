using System;

namespace DaveSexton.XmlGel.Maml.Documents.Visitors
{
	/* developerConceptualDocumentType (developer.xsd)
	 */
	internal sealed class SdkScenariosDocumentToFlowDocumentVisitor : ConceptualDocumentToFlowDocumentVisitor
	{
		public SdkScenariosDocumentToFlowDocumentVisitor(MamlDocument document, Action uiContainerChanged)
			: base(document, uiContainerChanged)
		{
		}
	}
}