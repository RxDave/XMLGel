﻿using System.Windows.Documents;
using System.Xml.Linq;
using DaveSexton.XmlGel.Maml.Documents.Visitors;

namespace DaveSexton.XmlGel.Maml.Documents
{
	/* token (XSD type)
	 * 
	 *	token			::= '.' | '/' | '//' | '|' | '@' | NameTest
	 *	NameTest	::= QName | '*' | NCName ':' '*'
	 */
	internal class MamlRestrictedText : MamlString
	{
		public MamlRestrictedText(XElement element)
			: base(element)
		{
		}

		public override TextElement Accept(MamlToFlowDocumentVisitor visitor, out TextElement contentContainer)
		{
			return visitor.Visit(this, out contentContainer);
		}
	}
}