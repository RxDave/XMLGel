using System;
using System.Xml;
using System.Xml.Linq;

namespace DaveSexton.XmlGel.Xml
{
	internal sealed class XEntitizedText : XText
	{
		public XEntitizedText(string value)
			: base(value)
		{
		}

		public override void WriteTo(XmlWriter writer)
		{
			if (Parent == null)
			{
				base.WriteTo(writer);
			}
			else
			{
				WriteEntitized(Value, writer, writer.WriteString);
			}
		}

		internal static void WriteEntitized(string value, XmlWriter writer, Action<string> writeText)
		{
			int nonEntityIndex = 0, currentIndex = 0;

			for (; currentIndex < value.Length; currentIndex++)
			{
				var c = value[currentIndex];

				if ((int) c == 160)
				{
					if (nonEntityIndex < currentIndex)
					{
						writeText(value.Substring(nonEntityIndex, currentIndex - nonEntityIndex));
					}

					writer.WriteCharEntity(c);

					nonEntityIndex = currentIndex + 1;
				}
			}

			if (nonEntityIndex < currentIndex)
			{
				writeText(value.Substring(nonEntityIndex));
			}
		}
	}
}