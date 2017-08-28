using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Windows.Documents;
using System.Xml.Linq;
using System.Xml.Schema;

namespace DaveSexton.XmlGel.Xml
{
	[ContractClass(typeof(IXElementNodeContract<>))]
	public interface IXElementNode<in TVisitor> : IXNode
		where TVisitor : XContainerToFlowDocumentVisitor<TVisitor>
	{
		string DisplayName { get; }

		string ToolTip { get; }

		TextElement Accept(TVisitor visitor, out TextElement contentContainer);

		XElement CreateDefault(XmlSchemaElement schema);
	}

	[ContractClassFor(typeof(IXElementNode<>))]
	internal abstract class IXElementNodeContract<TVisitor> : IXElementNode<TVisitor>
		where TVisitor : XContainerToFlowDocumentVisitor<TVisitor>
	{
		public string DisplayName
		{
			get
			{
				return null;
			}
		}

		public string ToolTip
		{
			get
			{
				return null;
			}
		}

		public TextElement Accept(TVisitor visitor, out TextElement contentContainer)
		{
			Contract.Requires(visitor != null);
			contentContainer = null;
			return null;
		}

		public XElement CreateDefault(XmlSchemaElement schema)
		{
			Contract.Requires(schema != null);
			return null;
		}

		public bool? TrimParagraphs
		{
			get
			{
				return null;
			}
		}

		public bool? NormalizeText
		{
			get
			{
				return null;
			}
		}

		public IEnumerable<IXNode> Children
		{
			get
			{
				return null;
			}
		}

		public void Accept(IXVisitor visitor)
		{
		}
	}
}