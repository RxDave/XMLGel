using System.Collections.Generic;
using System.Windows.Documents;
using System.Xml.Linq;
using DaveSexton.XmlGel.Maml.Documents.Visitors;

namespace DaveSexton.XmlGel.Maml.Documents
{
	internal sealed class MamlEnum<TEnum> : MamlEnum
		where TEnum : struct
	{
		public TEnum? Value
		{
			get
			{
				return EnumStringConverter.FromDocumentValue<TEnum>(Element.Value);
			}
		}

		public override IEnumerable<KeyValuePair<object, object>> Options
		{
			get
			{
				return EnumStringConverter.GetOptions<TEnum>();
			}
		}

		public MamlEnum(XElement element)
			: base(element)
		{
		}

		public override TextElement Accept(MamlToFlowDocumentVisitor visitor, out TextElement contentContainer)
		{
			return visitor.Visit(this, out contentContainer);
		}

		public override object GetValueFromText(string value)
		{
			return EnumStringConverter.FromDocumentValue(typeof(TEnum), value);
		}

		public override string GetTextFromValue(object value)
		{
			return EnumStringConverter.ToDocumentValue((TEnum?) value);
		}
	}
}