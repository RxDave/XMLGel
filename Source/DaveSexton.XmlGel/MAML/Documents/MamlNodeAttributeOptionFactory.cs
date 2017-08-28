using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using System.Xml.Schema;
using DaveSexton.XmlGel.Extensions;

namespace DaveSexton.XmlGel.Maml
{
	internal sealed class MamlNodeAttributeOptionFactory
	{
		private readonly XElement element;

		public MamlNodeAttributeOptionFactory(XElement element)
		{
			this.element = element;
		}

		public Tuple<FrameworkElement, string> CreateOption(XmlSchemaAttribute attribute)
		{
			var control = CreateOptionControl(attribute.AttributeSchemaType, GetValue(attribute));

			if (control == null)
			{
				return null;
			}

			// TODO: Lookup attribute display name in embedded resource
			return Tuple.Create(control, attribute.Name);
		}

		private FrameworkElement CreateOptionControl(XmlSchemaSimpleType schemaType, object value)
		{
			var content = schemaType.Content;

			var restriction = content as XmlSchemaSimpleTypeRestriction;

			if (restriction != null)
			{
				if (restriction.Facets.OfType<XmlSchemaEnumerationFacet>().Any())
				{
					/* For the MAML editor, simply override MamlNode.CreateOptions and return an strong-typed Enum ComboBox.  No need to be dynamic yet. */
					// TODO: Enums
					return null;
				}
			}
			else
			{
				var list = content as XmlSchemaSimpleTypeList;

				if (list != null)
				{
					// TODO: Support simple type lists
					return null;
				}
			}

			// TODO: Support simple type unions (http://msdn.microsoft.com/en-us/library/system.xml.schema.xmlschemasimpletypeunion.aspx)

			return CreateRestrictedOptionControl(schemaType.TypeCode, value);
		}

		private FrameworkElement CreateRestrictedOptionControl(XmlTypeCode typeCode, object value)
		{
			var control = typeCode == XmlTypeCode.Boolean ? (Control) new CheckBox() : new TextBox();

			control.DataContext = new MamlNodeAttributeOptionScalarSource()
			{
				Value = value
			};

			var binding = control.SetBinding(TextBox.TextProperty, "Value").ParentBinding;

			binding.ValidationRules.Add(new TypeCodeValidationRule(typeCode));

			return control;
		}

		public void Commit(FrameworkElement option)
		{
			var attribute = (XmlSchemaAttribute) option.Tag;

			var scalar = option.DataContext as MamlNodeAttributeOptionScalarSource;

			if (scalar != null)
			{
				SetValue(attribute, scalar.Value);
			}
		}

		private object GetValue(XmlSchemaAttribute attribute)
		{
			var name = attribute.GetXName();

			var actualAttribute = element.Attribute(name)
												 ?? element.GetDefaultAttributes().FirstOrDefault(a => a.Name == name);

			if (actualAttribute == null)
			{
				return attribute.EvaluateDefault<object>();
			}
			else
			{
				return actualAttribute.Value;
			}
		}

		private void SetValue(XmlSchemaAttribute attribute, object value)
		{
			element.SetAttributeValue(attribute.GetXName(), value);
		}
	}
}