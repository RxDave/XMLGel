using System;
using System.Windows.Documents;
using System.Xml.Linq;
using DaveSexton.XmlGel.Maml.Documents.Visitors;

namespace DaveSexton.XmlGel.Maml.Documents
{
	public sealed class TroubleshootingDocument : MamlDocument
	{
		public TroubleshootingDocument()
			: base(MamlDocumentKind.Troubleshooting)
		{
		}

		public TroubleshootingDocument(Guid id, int revisionNumber, XDocument document, XElement documentRoot, IMamlMetadata metadata)
			: base(MamlDocumentKind.Troubleshooting, id, revisionNumber, document, documentRoot, metadata)
		{
		}

		internal override MamlToFlowDocumentVisitor CreateVisitor(Action uiContainerChanged)
		{
			return new TroubleshootingDocumentToFlowDocumentVisitor(this, uiContainerChanged);
		}

		internal override FlowDocumentToMamlVisitor CreateVisitor(FlowDocument document)
		{
			return new FlowDocumentToTroubleshootingDocumentVisitor(document, this);
		}
	}
}