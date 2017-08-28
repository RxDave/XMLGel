using System.Diagnostics.Contracts;
using System.Windows.Documents;
using System.Xml.Linq;
using System.Xml.Schema;

namespace DaveSexton.XmlGel.Xml
{
	public abstract class XElementNode<TVisitor> : XVisitableElement, IXElementNode<TVisitor>
		where TVisitor : XContainerToFlowDocumentVisitor<TVisitor>
	{
		public virtual bool? TrimParagraphs
		{
			get
			{
				return null;
			}
		}

		public virtual bool? NormalizeText
		{
			get
			{
				return null;
			}
		}

		public virtual string DisplayName
		{
			get
			{
				return Element.Name.LocalName;
			}
		}

		public virtual string ToolTip
		{
			get
			{
				return Element.Name.LocalName;
			}
		}

		public XElementNode(XElement element)
			: base(element)
		{
			Contract.Requires(element != null);
		}

		public abstract TextElement Accept(TVisitor visitor, out TextElement contentContainer);

		public virtual XElement CreateDefault(XmlSchemaElement schema)
		{
			// Null indicates that the element should be generated from the schema using the validator
			return null;
		}
	}
}