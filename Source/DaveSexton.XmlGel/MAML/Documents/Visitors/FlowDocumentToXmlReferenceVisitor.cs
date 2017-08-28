using System.Windows.Documents;

namespace DaveSexton.XmlGel.Maml.Documents.Visitors
{
	internal sealed class FlowDocumentToXmlReferenceVisitor : FlowDocumentToMamlVisitor
	{
		public FlowDocumentToXmlReferenceVisitor(FlowDocument flowDocument, MamlDocument document)
			: base(flowDocument, document)
		{
		}
	}
}