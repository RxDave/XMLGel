﻿using System.Windows.Documents;
using System.Xml.Linq;
using DaveSexton.XmlGel.Maml.Documents.Visitors;

namespace DaveSexton.XmlGel.Maml.Documents
{
	/* row (structureTable.xsd)
	 * 
	 *	- Allows 1..N entry (MamlTableEntry) elements
	 */
	internal sealed class MamlTableRow : MamlNode
	{
		public MamlTableRow(XElement element)
			: base(element)
		{
		}

		public override TextElement Accept(MamlToFlowDocumentVisitor visitor, out TextElement contentContainer)
		{
			return visitor.Visit(this, out contentContainer);
		}
	}
}