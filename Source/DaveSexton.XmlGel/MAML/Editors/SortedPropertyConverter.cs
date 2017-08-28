using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;

namespace DaveSexton.XmlGel.Maml.Editors
{
	internal class SortedPropertyConverter : TypeConverter
	{
		public StringComparison SameIndexPropertyComparison
		{
			get;
			set;
		}

		public SortedPropertyConverter()
		{
			SameIndexPropertyComparison = StringComparison.CurrentCultureIgnoreCase;
		}

		public sealed override bool GetPropertiesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
		{
			return GetSortedProperties(context, value, TypeDescriptor.GetProperties(value, attributes));
		}

		protected virtual PropertyDescriptorCollection GetSortedProperties(ITypeDescriptorContext context, object value, PropertyDescriptorCollection unsortedProperties)
		{
			return unsortedProperties.Sort(new IndexNameComparer(this));
		}

		protected virtual int GetPropertySortIndex(MemberDescriptor member)
		{
			Contract.Requires(member != null);

			var attribute = member.Attributes[typeof(SortedPropertyAttribute)] as SortedPropertyAttribute;

			return attribute == null ? 0 : attribute.SortIndex;
		}

		public virtual PropertyDescriptorCollection Sort(PropertyDescriptorCollection properties)
		{
			Contract.Requires(properties != null);

			return properties.Sort(new IndexNameComparer(this));
		}

		private sealed class IndexNameComparer : Comparer<PropertyDescriptor>
		{
			private readonly SortedPropertyConverter owner;

			public IndexNameComparer(SortedPropertyConverter owner)
			{
				Contract.Requires(owner != null);

				this.owner = owner;
			}

			public override int Compare(PropertyDescriptor x, PropertyDescriptor y)
			{
				if (x == null)
				{
					return (y == null) ? 0 : -1;
				}
				else if (y == null)
				{
					return 1;
				}

				int result = owner.GetPropertySortIndex(x).CompareTo(owner.GetPropertySortIndex(y));

				return result == 0
					? string.Compare(x.Name, y.Name, owner.SameIndexPropertyComparison)
					: result;
			}
		}
	}
}