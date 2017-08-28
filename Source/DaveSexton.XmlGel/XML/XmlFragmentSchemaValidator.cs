using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using DaveSexton.XmlGel.Extensions;

namespace DaveSexton.XmlGel.Xml
{
	internal sealed class XmlFragmentSchemaValidator
	{
		public XmlSchemaSet Schemas
		{
			get
			{
				return schemas;
			}
		}

		internal Func<XmlSchemaElement, XElement> DefaultElementFactory
		{
			get
			{
				return defaultElementFactory;
			}
		}

		private readonly XmlSchemaSet schemas;
		private readonly XmlSchemaValidator validator;
		private readonly Func<XmlSchemaElement, XElement> defaultElementFactory;
		private readonly Func<XElement, bool> isInlineType;
		private readonly Func<IEnumerable<XmlSchemaElement>, XmlSchemaElement> getParagraph;

		private Action validationEventHandler;

		public XmlFragmentSchemaValidator(
			XmlResolver resolver,
			Func<XmlSchemaElement, XElement> defaultElementFactory,
			Func<XElement, bool> isInlineType,
			Func<IEnumerable<XmlSchemaElement>, XmlSchemaElement> getParagraph,
			XmlSchemaSet schemas)
		{
			// All arguments can be null

			this.defaultElementFactory = defaultElementFactory;
			this.isInlineType = isInlineType;
			this.getParagraph = getParagraph;
			this.schemas = schemas;

			var namespaceManager = new XmlNamespaceManager(schemas.NameTable);

			validator = new XmlSchemaValidator(schemas.NameTable, schemas, namespaceManager, XmlSchemaValidationFlags.ReportValidationWarnings)
			{
				XmlResolver = resolver
			};

			validator.ValidationEventHandler += (sender, e) =>
				{
					if (validationEventHandler == null)
					{
						throw e.Exception;
					}

					validationEventHandler();
				};
		}

		public void EnsureValid(XContainer container, Func<XElement, XmlSchemaElement> rootSchemaSelector)
		{
			Contract.Requires(container != null);
			Contract.Requires(rootSchemaSelector != null);

			var element = container as XElement ?? ((XDocument) container).Root;

			EnsureValid(element, rootSchemaSelector);
		}

		private void EnsureValid(XElement element, Func<XElement, XmlSchemaElement> rootSchemaSelector)
		{
			EnsureValidShallow(element, rootSchemaSelector);

			foreach (var child in element.Elements())
			{
				EnsureValid(child, null);
			}
		}

		private void EnsureValidShallow(XElement element, Func<XElement, XmlSchemaElement> rootSchemaSelector = null)
		{
			if (element.IsSchemaError())
			{
				return;
			}

			var schema = GetElementSchema(element, rootSchemaSelector);

			if (schema != null)
			{
				var context = new XmlFragmentSchemaValidatorContext(element, schema, this);

				context.EnsureValidShallow();
			}
		}

		private XmlSchemaElement GetElementSchema(XElement element, Func<XElement, XmlSchemaElement> rootSchemaSelector = null)
		{
			var schema = element.GetSchema();

			if (schema != null)
			{
				return schema;
			}
			else if (rootSchemaSelector == null)
			{
				return null;
			}

			Stack<KeyValuePair<XElement, XmlSchemaElement>> ancestors;
			XmlSchemaElement rootSchema;

			var root = GetDocumentRoot(element, rootSchemaSelector, out ancestors, out rootSchema);

			var originalAncestorCount = ancestors.Count;

			if (originalAncestorCount == 0)
			{
				Contract.Assume(element == root);

				element.SetSchema(rootSchema);

				return rootSchema;
			}

			/* Validation must start at the root to avoid ambiguity between global element names and local 
			 * element names;  i.e., XML schema is context-sensitive.  Of course, once a schema object is 
			 * attached to an element it can be used directly for subsequent validation, which is handled 
			 * above near the beginning of this method.
			 */
			validator.Initialize(rootSchema);

			try
			{
				ValidateElement(root, null, invalidHandler: null);

				do
				{
					var ancestor = ancestors.Pop();
					var stopAtChild = ancestors.Count > 0 ? ancestors.Peek().Key : element;

					schema = MoveToChild(ancestor.Key, stopAtChild);
				}
				while (schema != null && ancestors.Count > 0);
			}
			finally
			{
				try
				{
					if (schema != null)
					{
						originalAncestorCount++;
					}

					for (int i = 0; i < originalAncestorCount - ancestors.Count; i++)
					{
						ValidateEndElement();
					}
				}
				finally
				{
					validationEventHandler = null;

					validator.EndValidation();
				}
			}

			if (schema != null)
			{
				element.SetSchema(schema);
			}

			return schema;
		}

		private static XElement GetDocumentRoot(XElement element, Func<XElement, XmlSchemaElement> rootSchemaSelector, out Stack<KeyValuePair<XElement, XmlSchemaElement>> ancestors, out XmlSchemaElement schema)
		{
			ancestors = new Stack<KeyValuePair<XElement, XmlSchemaElement>>();

			schema = rootSchemaSelector(element);

			if (schema != null)
			{
				return element;
			}

			var parent = element.Parent;

			while (parent != null)
			{
				schema = rootSchemaSelector(parent);

				ancestors.Push(new KeyValuePair<XElement, XmlSchemaElement>(parent, schema));

				if (schema != null)
				{
					break;
				}

				parent = parent.Parent;
			}

			if (schema == null)
			{
				throw element.CreateXmlSchemaException("The element \"" + element.Name + "\" exists outside of the known schema root.");
			}

			Contract.Assert(parent != null);
			Contract.Assert(ancestors.Count > 0);

			return parent;
		}

		private XmlSchemaElement MoveToChild(XElement element, XElement stopAtChild)
		{
			Contract.Requires(element != null);
			Contract.Requires(stopAtChild != null);

			SkipAttributes();

			var node = element.FirstNode;

			while (node != null)
			{
				XElement child;
				XmlSchemaElement childSchema;
				bool? isValid;

				if (node.IsSchemaError())
				{
					child = null;
					childSchema = null;
					isValid = null;
				}
				else
				{
					isValid = ValidateNode(node, out child, out childSchema);
				}

				if (node == stopAtChild)
				{
					Contract.Assume((isValid.HasValue && isValid.Value) || childSchema == null);

					if (!(isValid ?? true))
					{
						SetInvalid(stopAtChild);

						validator.SkipToEndElement(null);
					}

					return childSchema;
				}

				if (child != null)
				{
					validator.SkipToEndElement(null);
				}

				node = node.NextNode;
			}

			if (!stopAtChild.IsSchemaError())
			{
				SetInvalid(stopAtChild);
			}

			return null;
		}

		internal void SetValidationEventHandler(Action action)
		{
			validationEventHandler = action;
		}

		internal void Initialize(XmlSchemaElement schema)
		{
			validator.Initialize(schema);
		}

		internal XmlSchemaParticlesExpected GetExpectedParticles()
		{
			return new XmlSchemaParticlesExpected(validator.GetExpectedParticles().ToList());
		}

		internal void ValidateAttribute(XAttribute attribute, out string defaultValue)
		{
			validationEventHandler = () => OnInvalidAttribute(attribute);

			var info = new XmlSchemaInfo();

			validator.ValidateAttribute(attribute.Name.LocalName, attribute.Name.NamespaceName, attribute.Value, info);

			if (info.SchemaAttribute != null && info.SchemaAttribute.DefaultValue != null)
			{
				defaultValue = info.SchemaAttribute.DefaultValue;
			}
			else
			{
				defaultValue = null;
			}

			validationEventHandler = null;
		}

		internal IEnumerable<XmlSchemaAttribute> GetUnspecifiedDefaultAttributes()
		{
			var defaultAttributes = new ArrayList();

			validator.GetUnspecifiedDefaultAttributes(defaultAttributes);

			return defaultAttributes.Cast<XmlSchemaAttribute>();
		}

		internal IEnumerable<XmlSchemaAttribute> GetExpectedRequiredAttributes()
		{
			return validator.GetExpectedAttributes().Where(attribute => attribute.Use == XmlSchemaUse.Required);
		}

		internal void SkipAttributes()
		{
			validationEventHandler = () => { };

			try
			{
				validator.ValidateEndOfAttributes(null);
			}
			finally
			{
				validationEventHandler = null;
			}
		}

		internal void ValidateEndOfAttributes()
		{
			validator.ValidateEndOfAttributes(null);
		}

		internal bool ValidateNode(XNode node, out XElement element, out XmlSchemaElement elementSchema)
		{
			element = node as XElement;

			if (element == null)
			{
				elementSchema = null;

				return ValidateText(node as XText);
			}
			else
			{
				return ValidateElement(element, out elementSchema);
			}
		}

		private bool ValidateText(XText text)
		{
			var isValid = true;

			if (text != null)
			{
				validationEventHandler = () => isValid = false;

				try
				{
					validator.ValidateText(text.Value);
				}
				finally
				{
					validationEventHandler = null;
				}
			}

			return isValid;
		}

		internal bool ValidateElement(XElement element, out XmlSchemaElement elementSchema)
		{
			var info = new XmlSchemaInfo();
			var isValid = true;

			ValidateElement(element, info, () => isValid = false);

			elementSchema = info.SchemaElement;

			return isValid;
		}

		internal void ValidateElement(XElement element, XmlSchemaInfo infoOut, Action invalidHandler)
		{
			validationEventHandler = invalidHandler;

			try
			{
				validator.ValidateElement(element.Name.LocalName, element.Name.NamespaceName, infoOut);
			}
			finally
			{
				validationEventHandler = null;
			}
		}

		internal void SkipToEndElement()
		{
			validator.SkipToEndElement(null);
		}

		internal bool ValidateEndElement()
		{
			try
			{
				var success = true;

				validationEventHandler = () => success = false;

				validator.ValidateEndElement(null);

				return success;
			}
			finally
			{
				validationEventHandler = null;
			}
		}

		internal void EndValidation()
		{
			validationEventHandler = null;

			validator.EndValidation();
		}

		internal bool IsInlineType(XElement element)
		{
			return isInlineType(element);
		}

		internal XmlSchemaElement GetParagraph(IEnumerable<XmlSchemaElement> elements)
		{
			return getParagraph(elements);
		}

		internal void SetInvalid(XNode node)
		{
			node.SetSchemaError(isError: true);

			OnInvalidNode(node);
		}

		public event Action<XNode> InvalidNode;
		public event Action<XAttribute> InvalidAttribute;

		private void OnInvalidNode(XNode node)
		{
			var handler = InvalidNode;

			if (handler != null)
			{
				handler(node);
			}
		}

		private void OnInvalidAttribute(XAttribute attribute)
		{
			var handler = InvalidAttribute;

			if (handler != null)
			{
				handler(attribute);
			}
		}
	}
}