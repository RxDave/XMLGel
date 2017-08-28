using System;
using System.Windows.Documents;
using System.Xml.Linq;
using DaveSexton.XmlGel.Maml.Documents.Visitors;

namespace DaveSexton.XmlGel.Maml.Documents
{
	public sealed class ReferenceWithoutSyntaxDocument : MamlDocument
	{
		public ReferenceWithoutSyntaxDocument()
			: base(MamlDocumentKind.ReferenceWithoutSyntax)
		{
		}

		public ReferenceWithoutSyntaxDocument(Guid id, int revisionNumber, XDocument document, XElement documentRoot, IMamlMetadata metadata)
			: base(MamlDocumentKind.ReferenceWithoutSyntax, id, revisionNumber, document, documentRoot, metadata)
		{
		}

		internal override MamlToFlowDocumentVisitor CreateVisitor(Action uiContainerChanged)
		{
			return new ReferenceWithoutSyntaxDocumentToFlowDocumentVisitor(this, uiContainerChanged);
		}

		internal override FlowDocumentToMamlVisitor CreateVisitor(FlowDocument document)
		{
			return new FlowDocumentToReferenceWithoutSyntaxDocumentVisitor(document, this);
		}
	}
}