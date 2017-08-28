using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Xml.Linq;
using DaveSexton.XmlGel.Extensions;

namespace DaveSexton.XmlGel.Documents
{
	public class TextElementNode : ITextElementNode
	{
		public TextElement Element
		{
			get
			{
				return element;
			}
		}

		public IEnumerable<ITextElementNode> Children
		{
			get
			{
				return GetChildren().Select(factory.Create);
			}
		}

		protected virtual string StructureName
		{
			get
			{
				return element.GetType().Name;
			}
		}

		private readonly TextElement element;
		private readonly ITextElementNodeFactory factory;

		public TextElementNode(TextElement element, ITextElementNodeFactory factory)
		{
			this.element = element;
			this.factory = factory;
		}

		public virtual void Accept(ITextElementVisitor visitor)
		{
			visitor.Visit(this);
		}

		protected virtual IEnumerable<TextElement> GetChildren()
		{
			return LogicalTreeHelper.GetChildren(element).OfType<TextElement>();
		}

		public XElement GetStructure(XNamespace defaultNamespace)
		{
			var name = element.Name;
			var tag = element.Tag;

			var data = tag as XElement;

			var isSchemaError = data == null ? false : data.IsSchemaError();

			Hack_RemoveMamlNamespace(ref tag);

			return new XElement(defaultNamespace + StructureName,
				string.IsNullOrEmpty(name) ? null : new XAttribute("Name", name),
				isSchemaError ? new XAttribute("IsAfterSchemaError", true) : null,
				tag == null ? null : new XAttribute("Tag", tag.ToString()),
				GetStructureContent(defaultNamespace));
		}

		private static void Hack_RemoveMamlNamespace(ref object tag)
		{
			var element = tag as XElement;

			if (element != null && element.Name.Namespace == Maml.Maml.Namespace)
			{
				tag = new XElement(element.Name.LocalName, element.Attributes(), element.Elements());
			}
		}

		protected virtual IEnumerable<object> GetStructureContent(XNamespace defaultNamespace)
		{
			return Children.Select(child => child.GetStructure(defaultNamespace));
		}
	}
}