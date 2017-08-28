using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace DaveSexton.XmlGel.Maml.Editors
{
	internal sealed class CustomPropertyDescriptor : PropertyDescriptor
	{
		public override Type ComponentType
		{
			get
			{
				return typeof(DocumentProperties);
			}
		}

		public override bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public override Type PropertyType
		{
			get
			{
				return typeof(string);
			}
		}

		public string Value
		{
			get
			{
				return (properties.ContainsKey(name))
					? properties[name]
					: null;
			}
			set
			{
				properties[name] = value ?? string.Empty;
			}
		}

		public bool Changed
		{
			get
			{
				return !string.Equals(originalValue, Value, StringComparison.Ordinal);
			}
		}

		private readonly IDictionary<string, string> properties;
		private readonly string name, originalValue;

		public CustomPropertyDescriptor(string name, string value, IDictionary<string, string> properties, params Attribute[] attributes)
			: base(name, attributes)
		{
			this.name = name;
			this.properties = properties;

			originalValue = value;
		}

		public override bool ShouldSerializeValue(object component)
		{
			return Changed;
		}

		public override bool CanResetValue(object component)
		{
			return Changed;
		}

		public override object GetValue(object component)
		{
			return Value;
		}

		public override void ResetValue(object component)
		{
			Value = originalValue;
		}

		public override void SetValue(object component, object value)
		{
			Value = value as string;
		}
	}
}