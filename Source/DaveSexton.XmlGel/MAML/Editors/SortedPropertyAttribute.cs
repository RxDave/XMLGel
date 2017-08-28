using System;

namespace DaveSexton.XmlGel.Maml.Editors
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
	internal sealed class SortedPropertyAttribute : Attribute
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
		public static readonly SortedPropertyAttribute Default = new SortedPropertyAttribute(0);

		public int SortIndex
		{
			get
			{
				return sortIndex;
			}
		}

		private int sortIndex;

		public SortedPropertyAttribute(int sortIndex)
		{
			this.sortIndex = sortIndex;
		}

		public override bool IsDefaultAttribute()
		{
			return sortIndex == 0;
		}
	}
}