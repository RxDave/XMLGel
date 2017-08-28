using System.Xml.Linq;
using DaveSexton.XmlGel.Maml.Documents.Visitors;

namespace DaveSexton.XmlGel.Maml.Documents
{
	/* This type does not exist in the MAML schema, though it's required by MamlToFlowDocumentVisitor to properly handle
	 * schema types that are allowed as children of both block- and inline-style elements; e.g., inlineLinkingGroup types
	 * are allowed within other inline types, directly within section content elements and within relatedTopics elements.
	 */
	internal abstract class MamlInlineContainer : MamlNode
	{
		protected MamlInlineContainer(XElement element)
			: base(element)
		{
		}
	}
}