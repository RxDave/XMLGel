using System.Collections.Generic;
using System.Windows.Documents;
using System.Xml.Linq;
using DaveSexton.XmlGel.Maml.Documents.Visitors;

namespace DaveSexton.XmlGel.Maml.Documents
{
	internal abstract class MamlEnum : MamlNode
	{
		public abstract IEnumerable<KeyValuePair<object, object>> Options
		{
			get;
		}

		protected MamlEnum(XElement element)
			: base(element)
		{
		}

		public abstract object GetValueFromText(string value);

		public abstract string GetTextFromValue(object value);
	}
}