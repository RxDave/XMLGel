using System.Xml;
using System.Xml.Linq;

namespace DaveSexton.XmlGel.Xml
{
	internal sealed class XEntitizedComment : XComment
	{
		public XEntitizedComment(string value)
			: base(value)
		{
		}

		public override void WriteTo(XmlWriter writer)
		{
			XEntitizedText.WriteEntitized(Value, writer, writer.WriteComment);
		}
	}
}