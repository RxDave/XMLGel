using System;
using System.Collections.Generic;

namespace DaveSexton.XmlGel.Maml.Editors
{
	internal sealed class NameValuePairEqualityComparer : EqualityComparer<NameValuePair>
	{
		private readonly StringComparison comparison;
		private bool compareNames;

		public NameValuePairEqualityComparer(StringComparison comparison, bool compareNames)
		{
			this.comparison = comparison;
			this.compareNames = compareNames;
		}

		public override bool Equals(NameValuePair x, NameValuePair y)
		{
			var first = x == null ? null : compareNames ? x.Name : x.Value;
			var second = y == null ? null : compareNames ? y.Name : y.Value;

			return (first == null && second == null)
					|| (first != null && second != null && first.Equals(second, comparison));
		}

		public override int GetHashCode(NameValuePair obj)
		{
			var value = obj == null ? null : compareNames ? obj.Name : obj.Value;

			return value == null ? 0 : value.GetHashCode();
		}
	}
}