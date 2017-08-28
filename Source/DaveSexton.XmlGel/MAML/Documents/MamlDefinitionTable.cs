using System.Windows.Documents;
using System.Xml.Linq;
using DaveSexton.XmlGel.Maml.Documents.Visitors;

namespace DaveSexton.XmlGel.Maml.Documents
{
	/* definitionTableType (structure.xsd)
	 * 
	 * Sequence: (1..N)
	 *	- definedTerm (MamlInline)
	 *	- definition (MamlDefinition)
	 */
	internal sealed class MamlDefinitionTable : MamlNode
	{
		public MamlDefinitionTable(XElement element)
			: base(element)
		{
		}

		public override TextElement Accept(MamlToFlowDocumentVisitor visitor, out TextElement contentContainer)
		{
			return visitor.Visit(this, out contentContainer);
		}
	}
}