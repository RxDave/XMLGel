using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Schema;
using DaveSexton.XmlGel.Extensions;

namespace DaveSexton.XmlGel.Xml
{
	internal sealed class XmlFragmentSchemaValidatorContext
	{
		public IList<XmlSchemaParticlesExpected> ExpectedParticlesBetweenNodes
		{
			get
			{
				return expectedParticlesBetweenNodes.AsReadOnly();
			}
		}

		private readonly XElement validatingElement;
		private readonly XmlSchemaElement validatingSchema;
		private readonly XmlFragmentSchemaValidator validator;
		private readonly List<XmlSchemaParticlesExpected> expectedParticlesBetweenNodes = new List<XmlSchemaParticlesExpected>();
		private readonly List<XElement> addedMissingRequiredElements = new List<XElement>();
		private List<Tuple<XNode, int>> invalidChildren;
		private bool used;

		public XmlFragmentSchemaValidatorContext(XElement element, XmlSchemaElement schema, XmlFragmentSchemaValidator validator)
		{
			this.validatingElement = element;
			this.validatingSchema = schema;
			this.validator = validator;
		}

		private void EnsureContextNotUsed()
		{
			if (used)
			{
				throw new InvalidOperationException("A validation context was used more than once.");
			}
			else
			{
				used = true;
			}
		}

		public void EnsureValidShallow()
		{
			EnsureContextNotUsed();

			invalidChildren = new List<Tuple<XNode, int>>();

			var ensureRequiredAttributes = true;

			bool missingRequiredElements;
			bool isValid;

			do
			{
				expectedParticlesBetweenNodes.Clear();

				isValid = ValidateShallow(ensureRequiredAttributes, out missingRequiredElements);

				ensureRequiredAttributes = false;
			}
			while (!isValid && !missingRequiredElements);

			if (missingRequiredElements)
			{
				AddMissingRequiredElements();

				isValid = Revalidate();
			}

			Contract.Assume(isValid);

			InsertInvalidChildren();
		}

		public bool ValidateShallow()
		{
			EnsureContextNotUsed();

			bool missingRequiredElements;

			return ValidateShallow(false, out missingRequiredElements);
		}

		private bool ValidateShallow(bool ensureRequiredAttributes, out bool missingRequiredElements)
		{
			validator.Initialize(validatingSchema);

			/* The invalidHandler must be null so that a validation exception is rethrown.
			 * An invalid container element indicates a bug in the program.  The program is responsible for 
			 * supplying a schema fragment corresponding to the specified element, which should always be correct.
			 */
			validator.ValidateElement(validatingElement, null, invalidHandler: null);

			if (ensureRequiredAttributes)
			{
				EnsureRequiredAttributes();
			}
			else
			{
				validator.SkipAttributes();
			}

			IEnumerable<XmlSchemaParticle> remainingExpectedParticles;

			missingRequiredElements = false;

			var isValid = false;

			if (ValidateExistingChildren(out remainingExpectedParticles))
			{
				if (validator.ValidateEndElement())
				{
					isValid = true;
				}
				else
				{
					missingRequiredElements = true;
				}
			}
			else
			{
				validator.SkipToEndElement();
			}

			validator.EndValidation();

			return isValid;
		}

		private void EnsureRequiredAttributes()
		{
			try
			{
				ValidateSpecifiedAttributes();

				SetUnspecifiedAttributeDefaults();

				var failed = false;

				validator.SetValidationEventHandler(() => failed = true);

				validator.ValidateEndOfAttributes();

				if (failed)
				{
					SetRequiredAttributeDefaults();
				}
			}
			finally
			{
				validator.SetValidationEventHandler(null);
			}
		}

		private void ValidateSpecifiedAttributes()
		{
			foreach (var attribute in validatingElement
				.Attributes()
				.Where(attribute =>
						attribute.Name.Namespace != XNamespace.Xmlns		// xmlns:prefix="..."
					&& attribute.Name != "xmlns"))										// default xmlns="..."
			{
				string defaultValue;
				validator.ValidateAttribute(attribute, out defaultValue);

				if (defaultValue != null)
				{
					validatingElement.AddAnnotation(new XAttribute(attribute.Name, defaultValue));
				}
			}
		}

		private void SetUnspecifiedAttributeDefaults()
		{
			foreach (XmlSchemaAttribute attribute in validator.GetUnspecifiedDefaultAttributes())
			{
				/* A null value occurred in testing for the xml:space attribute.  FixedValue was null too.  Internally, the 
				 * XmlSchemaAttribute object did appear to contain a default value in a non-public attrDef field, but it doesn't 
				 * expose it publicly for some unknown reason.  Anyway, it's perhaps better that the default value for the 
				 * xml:space attribute isn't added to the element.
				 */

				attribute.AddTo(validatingElement);
			}
		}

		private void SetRequiredAttributeDefaults()
		{
			foreach (var attribute in validator.GetExpectedRequiredAttributes())
			{
				attribute.AddTo(validatingElement);
			}
		}

		private bool ValidateExistingChildren(out IEnumerable<XmlSchemaParticle> remainingExpectedParticles)
		{
			var expectedParticles = CaptureExpectedParticles();

			var node = validatingElement.FirstNode;

			while (node != null)
			{
				XElement child;
				XmlSchemaElement childSchema = null;

				if (node.IsSchemaError())
				{
					child = null;
				}
				else if (!validator.ValidateNode(node, out child, out childSchema))
				{
					if (invalidChildren != null)
					{
						invalidChildren.Add(Tuple.Create(node, node.IndexOf()));

						node.Remove();
					}

					if (child != null)
					{
						validator.SkipToEndElement();
					}

					remainingExpectedParticles = null;

					/* Testing has shown that after a failed validation of a child element, the validator 
					 * sets its internal state to a "Skip" flag for the containing element so that all 
					 * subsequent validations will automatically succeed without validation.  As a result, 
					 * validating subsequent elements causes the ValidateElement method to succeed though
					 * the specified XmlSchemaInfo object is not populated with a reference to a matching 
					 * schema object.  It seems that the only way to reset the current state is to restart
					 * the entire validation process from the beginning.
					 */
					return false;
				}
				else if (child != null)
				{
					child.SetSchema(childSchema);

					validator.SkipToEndElement();
				}

				expectedParticles = CaptureExpectedParticles();

				node = node.NextNode;
			}

			remainingExpectedParticles = expectedParticles;

			Contract.Assume(expectedParticlesBetweenNodes.Count == validatingElement.Nodes().Count() + 1);

			return true;
		}

		private bool Revalidate()
		{
			var newContext = new XmlFragmentSchemaValidatorContext(validatingElement, validatingSchema, validator);

			var isValid = newContext.ValidateShallow();

			expectedParticlesBetweenNodes.Clear();
			expectedParticlesBetweenNodes.AddRange(newContext.expectedParticlesBetweenNodes);

			return isValid;
		}

		private void AddMissingRequiredElements()
		{
			var elementPool = invalidChildren
				.Where(pair => pair.Item1 is XElement)
				.Select(pair => (XElement) pair.Item1)
				.ToList();

			var addedElements = new List<XElement>();

			var isValid = validatingSchema.ValidateChildren(
				validator.Schemas,
				validatingElement,
				null,
				addedElements,
				elementPool,
				validator.DefaultElementFactory);

			Contract.Assume(isValid);

			foreach (var element in addedElements)
			{
				addedMissingRequiredElements.Add(element);
			}
		}

		private XmlSchemaParticlesExpected CaptureExpectedParticles()
		{
			var expectedParticles = validator.GetExpectedParticles();

			expectedParticlesBetweenNodes.Add(expectedParticles);

			return expectedParticles;
		}

		private void InsertInvalidChildren()
		{
			var requiresRevalidation = false;
			var offset = 0;

			for (int i = 0; i < invalidChildren.Count; i++)
			{
				var child = invalidChildren[i].Item1;
				var element = child as XElement;
				var originalIndex = invalidChildren[i].Item2 + offset;

				bool addedToAnotherElement;

				if (!TryInsertParagraph(child, originalIndex, out addedToAnotherElement)
					&& (element == null || !TryInsertElement(element, out addedToAnotherElement)))
				{
					var setInvalid = true;

					if (element == null || !TryInsertAny(child, out addedToAnotherElement))
					{
						if (element != null && TryInsertParent(element))
						{
							addedToAnotherElement = true;
							setInvalid = false;
						}
						else
						{
							validatingElement.InsertAt(originalIndex, child);

							addedToAnotherElement = false;
							requiresRevalidation = true;
						}
					}

					if (setInvalid)
					{
						validator.SetInvalid(child);
					}
				}

				if (!addedToAnotherElement)
				{
					offset++;
				}
			}

			if (requiresRevalidation)
			{
				var isValid = Revalidate();

				// The new element has a schema error flag set, which must cause validation to skip it and therefore to succeed.
				Contract.Assume(isValid);
			}

			Contract.Assume(expectedParticlesBetweenNodes.Count == validatingElement.Nodes().Count() + 1);
		}

		private bool TryInsertParagraph(XNode child, int originalIndex, out bool addedAsChildOfChild)
		{
			addedAsChildOfChild = false;

			var element = child as XElement;

			return (element == null || validator.IsInlineType(element))
				&& TryInsertChild(
						validatingElement,
						validatingSchema,
						child,
						originalIndex,
						true,
						(expected, index, parent, node) =>
						{
							var paragraphSchema = validator.GetParagraph(expected.OfType<XmlSchemaElement>());

							if (paragraphSchema != null)
							{
								Contract.Assume(node is XElement || paragraphSchema.ElementSchemaType.IsMixed);

								XElement paragraph = null;

								if (index > 0)
								{
									var elements = parent.Elements().ToList();

									var paragraphName = paragraphSchema.GetXName();

									for (int i = Math.Min(elements.Count - 1, index - 1); i >= 0; i--)
									{
										var candidate = elements[i];

										if (candidate.Name == paragraphName)
										{
											paragraph = candidate;
											break;
										};
									}
								}

								if (paragraph == null)
								{
									paragraph = paragraphSchema.CreateRequiredElementTree(validator.Schemas);

									parent.InsertAt(index, paragraph);
								}

								paragraph.Add(node);

								return true;
							}

							return false;
						},
						out addedAsChildOfChild);
		}

		private bool TryInsertElement(XElement child, out bool addedAsChildOfChild)
		{
			return TryInsertChild(
				validatingElement,
				validatingSchema,
				child,
				-1,
				true,
				(expected, index, parent, node) =>
				{
					if (expected.OfType<XmlSchemaElement>().Any(elementSchema => child.Name == elementSchema.GetXName()))
					{
						parent.InsertAt(index, node);
						return true;
					}

					return false;
				},
				out addedAsChildOfChild);
		}

		private bool TryInsertAny(XNode child, out bool addedAsChildOfChild)
		{
			return TryInsertChild(
				validatingElement,
				validatingSchema,
				child,
				-1,
				true,
				(expected, index, parent, node) =>
				{
					if (expected.OfType<XmlSchemaAny>().Any())
					{
						parent.InsertAt(index, node);
						return true;
					}

					return false;
				},
				out addedAsChildOfChild);
		}

		private bool TryInsertParent(XElement child)
		{
			var parent = validatingElement.Parent;
			var parentSchema = parent == null ? null : parent.GetSchema();

			return parentSchema != null
					&& TryInsertChildInAnotherElement(
							parent,
							parentSchema,
							child,
							validatingElement.IndexOf() + 1,
							(expected, index, container, node) =>
							{
								if (expected.OfType<XmlSchemaElement>().Any(elementSchema => child.Name == elementSchema.GetXName()))
								{
									container.InsertAt(index, node);
									return true;
								}

								return false;
							});
		}

		private bool TryInsertChild(
			XElement element,
			XmlSchemaElement elementSchema,
			XNode child,
			int indexPreference,
			bool checkAddedMissingRequiredElements,
			Func<XmlSchemaParticlesExpected, int, XElement, XNode, bool> insertExpectedChild,
			out bool addedAsChildOfChild)
		{
			if (indexPreference >= 0
				&& indexPreference < expectedParticlesBetweenNodes.Count
				&& insertExpectedChild(expectedParticlesBetweenNodes[indexPreference], indexPreference, element, child)
				&& ValidateOrRemoveChild(element, elementSchema, child))
			{
				addedAsChildOfChild = false;
				return true;
			}

			for (int i = expectedParticlesBetweenNodes.Count - 1; i >= 0; i--)
			{
				if (i != indexPreference
					&& insertExpectedChild(expectedParticlesBetweenNodes[i], i, element, child)
					&& ValidateOrRemoveChild(element, elementSchema, child))
				{
					addedAsChildOfChild = false;
					return true;
				}
			}

			if (checkAddedMissingRequiredElements && addedMissingRequiredElements.Count > 0)
			{
				foreach (var missingChild in addedMissingRequiredElements)
				{
					var missingChildSchema = missingChild.GetSchema();

					if (TryInsertChildInAnotherElement(missingChild, missingChildSchema, child, -1, insertExpectedChild))
					{
						addedAsChildOfChild = true;
						return true;
					}
				}
			}

			addedAsChildOfChild = false;
			return false;
		}

		private bool TryInsertChildInAnotherElement(
			XElement element,
			XmlSchemaElement elementSchema,
			XNode child,
			int indexPreference,
			Func<XmlSchemaParticlesExpected, int, XElement, XNode, bool> insertExpectedChild)
		{
			var newContext = new XmlFragmentSchemaValidatorContext(element, elementSchema, validator);

			// prepare to call TryInsertChild
			bool isValid = newContext.ValidateShallow();

			Contract.Assume(isValid);

			bool ignore;
			return newContext.TryInsertChild(element, elementSchema, child, indexPreference, false, insertExpectedChild, out ignore);
		}

		private bool ValidateOrRemoveChild(XElement element, XmlSchemaElement elementSchema, XNode child)
		{
			var newContext = new XmlFragmentSchemaValidatorContext(element, elementSchema, validator);

			/* Note that this isn't a deep re-validation, which means that although the element names match at the current position,
			 * the semantics of the invalid child might be different than the expected element schema.  If an author creates
			 * a document for which its schema contains ambiguous element names and yet the author doesn't match the exact schema, 
			 * then there's no (easy?) way to fix it automatically.
			 * 
			 * However, I don't think this is generally a concern in XSD.  See UPA for more info: 
			 * http://www.w3.org/wiki/UniqueParticleAttribution
			 * http://msdn.microsoft.com/en-us/library/ms187822.aspx
			 * 
			 * The following algorithm simply takes a best guess under the assumption that abmiguity doesn't exist here.  If the 
			 * elements match by name and the schema validates with the new child element in place, then that's good enough.
			 */
			if (newContext.ValidateShallow())
			{
				expectedParticlesBetweenNodes.Clear();
				expectedParticlesBetweenNodes.AddRange(newContext.expectedParticlesBetweenNodes);

				Contract.Assume(expectedParticlesBetweenNodes.Count == element.Nodes().Count() + 1);

				return true;
			}
			else
			{
				child.Remove();

				return false;
			}
		}
	}
}