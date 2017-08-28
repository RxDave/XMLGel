using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Xml.Linq;
using System.Xml.Schema;
using DaveSexton.XmlGel.Extensions;

namespace DaveSexton.XmlGel
{
	internal abstract class Part : IEquatable<Part>
	{
		public Rect DocumentBox
		{
			get
			{
				return documentBox;
			}
		}

		public TextElement Element
		{
			get
			{
				return (TextElement) element;
			}
		}

		public FrameworkContentElement ElementOrDocument
		{
			get
			{
				return element;
			}
		}

		public XElement Data
		{
			get
			{
				return validatableData ?? data;
			}
		}

		public XmlSchemaElement Schema
		{
			get
			{
				return schema;
			}
		}

		public IList<Part> Children
		{
			get
			{
				if (children == null)
				{
					children = GetChildParts(this, element).ToList().AsReadOnly();
				}

				return children;
			}
		}

		private readonly Rect documentBox;
		private readonly FrameworkContentElement element;
		private readonly XmlSchemaElement schema;
		private XElement data;
		private XElement validatableData;
		private IList<Part> children;

		protected Part(FrameworkContentElement element, XElement data, XmlSchemaElement schema, Rect documentBox)
		{
			Contract.Requires(element != null);

			Contract.Assume(schema == null || data != null);

			this.element = element;
			this.data = data;
			this.schema = schema;
			this.documentBox = documentBox;
		}

		public void UpdateValidatableData()
		{
			Contract.Requires(Data != null);

			children = null;

			EnsureValidatableData();
		}

		public void EnsureValidatableData(Part updateChildPart = null)
		{
			Contract.Requires(Data != null);

			validatableData = data.CloneWithAnnotations(deep: false);

			foreach (var child in Children)
			{
				// At the time of writing, only shallow data is needed; i.e., just immediate children, not descendants.
				// Therefore, use child.data not child.validatableData.

				if (child.data != null)
				{
					var newData = child.data.CloneWithAnnotations(deep: false);

					validatableData.Add(newData);

					if (updateChildPart != null && child.element.EqualsOrContainsDescendant(updateChildPart.element))
					{
						updateChildPart.data = newData;
						updateChildPart = null;
					}
				}
			}
		}

		protected abstract Part TryCreatePart(FrameworkContentElement element);

		private static IEnumerable<Part> GetChildParts(Part source, FrameworkContentElement element)
		{
			var children = element.TryGetChildCollection();

			if (children != null)
			{
				foreach (TextElement child in children)
				{
					var part = source.TryCreatePart(child);

					if (part != null)
					{
						Contract.Assume(part.data != null);
						Contract.Assume(part.schema != null);

						yield return part;
						continue;
					}

					foreach (var descendant in GetChildParts(source, child))
					{
						yield return descendant;
					}
				}
			}
		}

		public static bool operator ==(Part first, Part second)
		{
			return ReferenceEquals(first, second)
					|| (!ReferenceEquals(first, null) && first.Equals(second));
		}

		public static bool operator !=(Part first, Part second)
		{
			return !(first == second);
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as Part);
		}

		public bool Equals(Part other)
		{
			return other != null && element == other.element;
		}

		public override int GetHashCode()
		{
			return element.GetHashCode();
		}

		public override string ToString()
		{
			return '{'
				+ element.GetType().Name + ", "
				+ (data == null ? null : data.Name.ToString()) + ", "
				+ (schema == null ? null : schema.QualifiedName.Name)
				+ '}';
		}
	}
}