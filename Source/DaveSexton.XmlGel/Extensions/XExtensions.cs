using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using DaveSexton.XmlGel.Xml;

namespace DaveSexton.XmlGel.Extensions
{
	public static class XExtensions
	{
		/// <summary>
		/// Creates a copy of the specified <paramref name="element"/> excluding child nodes but including attributes and
		/// default attribute annotations.  The specified <paramref name="annotations"/> are also added to the new element.
		/// </summary>
		/// <param name="element">The element from which data is copied.</param>
		/// <param name="annotations">Additional data for the new element.</param>
		/// <returns>A copy of the specified <paramref name="element"/>.</returns>
		public static XElement AsDataOnly(this XElement element, params object[] annotations)
		{
			if (element != null)
			{
				element = element.CloneWithAnnotations(deep: false);

				if (annotations != null && annotations.Length > 0)
				{
					foreach (var annotation in annotations)
					{
						element.AddAnnotation(annotation);
					}
				}
			}

			return element;
		}

		internal static IXNode GetNode(this XElement element)
		{
			return element.Annotation<IXNode>();
		}

		internal static void SetNode(this XElement element, IXNode node)
		{
			element.RemoveAnnotations<IXNode>();

			if (node != null)
			{
				element.AddAnnotation(node);
			}
		}

		public static bool IsSchemaError(this XNode node)
		{
			return node.Annotation<XmlSchemaErrorAnnotation>() != null
					|| node.Ancestors().Any(n => n.Annotation<XmlSchemaErrorAnnotation>() != null);
		}

		public static void SetSchemaError(this XNode node, bool isError)
		{
			node.RemoveAnnotations<XmlSchemaErrorAnnotation>();

			if (isError)
			{
				node.AddAnnotation(XmlSchemaErrorAnnotation.Instance);
			}
		}

		internal static XmlSchemaElement GetSchema(this XElement element)
		{
			return element.Annotation<XmlSchemaElement>();
		}

		internal static void SetSchema(this XElement element, XmlSchemaElement schema)
		{
			element.RemoveAnnotations<XmlSchemaElement>();

			if (schema != null)
			{
				element.AddAnnotation(schema);
			}
		}

		internal static IEnumerable<XAttribute> GetDefaultAttributes(this XElement element)
		{
			return element.Annotations<XAttribute>();
		}

		internal static XElement CloneWithAnnotations(this XElement element, bool deep = true)
		{
			var node = element.GetNode();
			var schema = element.GetSchema();
			var isSchemaError = element.IsSchemaError();
			var defaultAttributes = element.GetDefaultAttributes().ToList();

			element = deep ? new XElement(element) : new XElement(element.Name, element.Attributes());

			foreach (var defaultAttribute in defaultAttributes)
			{
				element.AddAnnotation(defaultAttribute);
			}

			if (isSchemaError)
			{
				element.SetSchemaError(isError: true);
			}

			if (schema != null)
			{
				element.SetSchema(schema);
			}

			if (node != null)
			{
				element.SetNode(node);
			}

			return element;
		}

		internal static void InsertAt(this XElement element, int index, XNode child)
		{
			Contract.Requires(element != null);
			Contract.Requires(index >= 0);

			var nodes = element.Nodes().ToList();

			if (index == nodes.Count)
			{
				element.Add(child);
			}
			else if (index < nodes.Count)
			{
				nodes[index].AddBeforeSelf(child);
			}
			else
			{
				throw new ArgumentOutOfRangeException("index");
			}
		}

		internal static int IndexOf(this XNode node)
		{
			Contract.Requires(node != null);
			Contract.Ensures(Contract.Result<int>() >= -1);

			if (node.Parent != null)
			{
				var index = 0;

				foreach (var child in node.Parent.Nodes())
				{
					if (child == node)
					{
						return index;
					}

					index++;
				}
			}

			return -1;
		}

		public static XmlException CreateXmlException(this XNode node, string message)
		{
			return node.CreateXmlException(message, null);
		}

		public static XmlException CreateXmlException(this XNode node, string message, Exception innerException)
		{
			var lineInfo = (IXmlLineInfo) node;

			if (lineInfo.HasLineInfo())
			{
				return new XmlException(message, innerException, lineInfo.LineNumber, lineInfo.LinePosition);
			}
			else
			{
				return new XmlException(message, innerException);
			}
		}

		public static XmlSchemaException CreateXmlSchemaException(this XNode node, string message)
		{
			return node.CreateXmlSchemaException(message, null);
		}

		public static XmlSchemaException CreateXmlSchemaException(this XNode node, string message, Exception innerException)
		{
			var lineInfo = (IXmlLineInfo) node;

			if (lineInfo.HasLineInfo())
			{
				return new XmlSchemaException(message, innerException, lineInfo.LineNumber, lineInfo.LinePosition);
			}
			else
			{
				return new XmlSchemaException(message, innerException);
			}
		}
	}
}