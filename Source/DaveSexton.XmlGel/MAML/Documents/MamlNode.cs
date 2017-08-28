using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Xml.Linq;
using System.Xml.Schema;
using DaveSexton.XmlGel.Extensions;
using DaveSexton.XmlGel.Maml.Documents.Visitors;
using DaveSexton.XmlGel.Properties;
using DaveSexton.XmlGel.Xml;

namespace DaveSexton.XmlGel.Maml.Documents
{
	internal abstract class MamlNode : XElementNode<MamlToFlowDocumentVisitor>
	{
		public override string DisplayName
		{
			get
			{
				var displayName = Text.ResourceManager.GetString(Element.Name.LocalName, CultureInfo.CurrentUICulture);

				return displayName ?? base.DisplayName;
			}
		}

		public override string ToolTip
		{
			get
			{
				// TODO: Get tooltip from resource
				return base.ToolTip;
			}
		}

		private MamlNodeAttributeOptionFactory attributeOptionFactory;

		protected MamlNode(XElement element)
			: base(element)
		{
		}

		protected internal virtual IList<Tuple<FrameworkElement, string>> GetOptions(XmlSchemaElement elementSchema, XmlSchemaSet schemaSet)
		{
			return CreateAttributeOptions(elementSchema, schemaSet).ToList();
		}

		protected internal virtual void CommitOptions(IEnumerable<Tuple<FrameworkElement, string>> options)
		{
			if (attributeOptionFactory != null)
			{
				foreach (var option in options)
				{
					var control = option.Item1;

					if (control.Tag is XmlSchemaAttribute)
					{
						attributeOptionFactory.Commit(control);
					}
				}
			}
		}

		protected IEnumerable<Tuple<FrameworkElement, string>> CreateAttributeOptions(XmlSchemaElement elementSchema, XmlSchemaSet schemaSet)
		{
			if (attributeOptionFactory == null)
			{
				attributeOptionFactory = new MamlNodeAttributeOptionFactory(Element);
			}

			return elementSchema
				.GetFlattenedAttributes(schemaSet)
				.Select(attributeOptionFactory.CreateOption)
				.Where(option => option != null);
		}
	}
}