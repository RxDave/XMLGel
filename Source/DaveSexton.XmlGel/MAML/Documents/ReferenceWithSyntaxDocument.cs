using System;
using System.Windows.Documents;
using System.Xml.Linq;
using DaveSexton.XmlGel.Maml.Documents.Visitors;

namespace DaveSexton.XmlGel.Maml.Documents
{
	public sealed class ReferenceWithSyntaxDocument : MamlDocument
	{
		public ReferenceWithSyntaxDocument()
			: base(MamlDocumentKind.ReferenceWithSyntax)
		{
		}

		public ReferenceWithSyntaxDocument(Guid id, int revisionNumber, XDocument document, XElement documentRoot, IMamlMetadata metadata)
			: base(MamlDocumentKind.ReferenceWithSyntax, id, revisionNumber, document, documentRoot, metadata)
		{
		}

		internal override MamlToFlowDocumentVisitor CreateVisitor(Action uiContainerChanged)
		{
			return new ReferenceWithSyntaxDocumentToFlowDocumentVisitor(this, uiContainerChanged);
		}

		internal override FlowDocumentToMamlVisitor CreateVisitor(FlowDocument document)
		{
			return new FlowDocumentToReferenceWithSyntaxDocumentVisitor(document, this);
		}
	}
}