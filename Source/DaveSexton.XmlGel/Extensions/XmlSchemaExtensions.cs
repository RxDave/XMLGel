using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Schema;
using DaveSexton.XmlGel.Xml;

namespace DaveSexton.XmlGel.Extensions
{
	internal static class XmlSchemaExtensions
	{
		[DebuggerStepThrough]
		public static XName GetXName(this XmlSchemaElement element)
		{
			return (XNamespace) element.QualifiedName.Namespace + element.QualifiedName.Name;
		}

		[DebuggerStepThrough]
		public static XName GetXName(this XmlSchemaAttribute attribute)
		{
			return (XNamespace) attribute.QualifiedName.Namespace + attribute.QualifiedName.Name;
		}

		public static object GetDefaultValue(this XmlSchemaSimpleType schemaType)
		{
			Contract.Ensures(Contract.Result<object>() != null);

			var restriction = schemaType.Content as XmlSchemaSimpleTypeRestriction;

			if (restriction != null)
			{
				if (restriction.Facets.Count > 0)
				{
					var facet = restriction.Facets[0] as XmlSchemaEnumerationFacet;

					if (facet != null)
					{
						return facet.Value;
					}
				}
			}
			else
			{
				var list = schemaType.Content as XmlSchemaSimpleTypeList;

				if (list != null)
				{
					return string.Empty;
				}
			}

			// TODO: Support simple type unions (http://msdn.microsoft.com/en-us/library/system.xml.schema.xmlschemasimpletypeunion.aspx)

			switch (schemaType.TypeCode)
			{
				case XmlTypeCode.Boolean:
					return false;
				case XmlTypeCode.Byte:
				case XmlTypeCode.Decimal:
				case XmlTypeCode.Double:
				case XmlTypeCode.Float:
				case XmlTypeCode.Int:
				case XmlTypeCode.Integer:
				case XmlTypeCode.Long:
				case XmlTypeCode.NonNegativeInteger:
				case XmlTypeCode.NonPositiveInteger:
				case XmlTypeCode.PositiveInteger:
				case XmlTypeCode.Short:
				case XmlTypeCode.UnsignedByte:
				case XmlTypeCode.UnsignedInt:
				case XmlTypeCode.UnsignedLong:
				case XmlTypeCode.UnsignedShort:
					return 0;
				case XmlTypeCode.Date:
				case XmlTypeCode.DateTime:
					return DateTime.MinValue;
				case XmlTypeCode.Duration:
				case XmlTypeCode.Time:
					return TimeSpan.Zero;
				default:
					return string.Empty;
			}
		}

		[DebuggerStepThrough]
		private static T Switch<T>(
			this XmlSchemaParticle particle,
			Func<XmlSchemaElement, T> onElement,
			Func<XmlSchemaGroupRef, T> onGroup,
			Func<XmlSchemaSequence, T> onSequence,
			Func<XmlSchemaChoice, T> onChoice,
			Func<XmlSchemaAny, T> onAny)
		{
			var element = particle as XmlSchemaElement;

			if (element != null)
			{
				return onElement(element);
			}

			var group = particle as XmlSchemaGroupBase;

			if (group != null)
			{
				return group.Switch(onSequence, onChoice);
			}

			var groupRef = particle as XmlSchemaGroupRef;

			if (groupRef != null)
			{
				return onGroup(groupRef);
			}

			var any = particle as XmlSchemaAny;

			if (any != null)
			{
				return onAny(any);
			}

			throw new ArgumentOutOfRangeException("particle");
		}

		[DebuggerStepThrough]
		private static T Switch<T>(
			this XmlSchemaGroupBase group,
			Func<XmlSchemaSequence, T> onSequence,
			Func<XmlSchemaChoice, T> onChoice)
		{
			var sequence = group as XmlSchemaSequence;

			if (sequence != null)
			{
				return onSequence(sequence);
			}

			var choice = group as XmlSchemaChoice;

			if (choice != null)
			{
				return onChoice(choice);
			}

			throw new ArgumentOutOfRangeException("group");
		}

		public static XElement CreateRequiredElementTree(this XmlSchemaElement elementSchema, XmlSchemaSet schemas)
		{
			var element = new XElement(
				elementSchema.GetXName(),
				elementSchema.ElementSchemaType.CreateRequiredNodes(e => CreateRequiredElementTree(e, schemas)),
				elementSchema.CreateDefaultAttributes(schemas));

			element.SetSchema(elementSchema);

			return element;
		}

		public static XElement CreateElement(this XmlSchemaElement elementSchema, XmlSchemaSet schemas)
		{
			return new XElement(elementSchema.GetXName(), elementSchema.CreateDefaultAttributes(schemas));
		}

		public static IEnumerable<XmlSchemaAttribute> GetFlattenedAttributes(this XmlSchemaElement element, XmlSchemaSet schemas)
		{
			var simple = element.ElementSchemaType as XmlSchemaSimpleType;

			if (simple != null)
			{
				return Enumerable.Empty<XmlSchemaAttribute>();
			}

			var complex = (XmlSchemaComplexType) element.ElementSchemaType;

			return complex.Attributes.GetFlattenedAttributes(schemas);
		}

		private static IEnumerable<XmlSchemaAttribute> GetFlattenedAttributes(this XmlSchemaObjectCollection attributes, XmlSchemaSet schemas)
		{
			return attributes.Cast<XmlSchemaObject>().SelectMany(
				attribute =>
				{
					var group = attribute as XmlSchemaAttributeGroup;

					if (group != null)
					{
						return group.Attributes.GetFlattenedAttributes(schemas);
					}

					var groupRef = attribute as XmlSchemaAttributeGroupRef;

					if (groupRef != null)
					{
						return ((XmlSchemaAttributeGroup) schemas
							.Schemas()
							.Cast<XmlSchema>()
							.Select(schema => schema.AttributeGroups[groupRef.RefName])
							.Where(g => g != null)
							.First())
							.Attributes
							.GetFlattenedAttributes(schemas);
					}

					return new[] { (XmlSchemaAttribute) attribute };
				});
		}

		private static IEnumerable<XAttribute> CreateDefaultAttributes(this XmlSchemaElement element, XmlSchemaSet schemas)
		{
			foreach (var attribute in element.GetFlattenedAttributes(schemas))
			{
				if (attribute.Use == XmlSchemaUse.Required
					|| (attribute.Use != XmlSchemaUse.Prohibited && (attribute.DefaultValue ?? attribute.FixedValue) != null))
				{
					var data = attribute.GetDefault();

					if (data.Value != null)
					{
						yield return new XAttribute(data.Key, data.Value);
					}
				}
			}
		}

		public static void AddTo(this XmlSchemaAttribute attribute, XElement element)
		{
			var data = attribute.GetDefault();

			if (data.Value != null)
			{
				if (attribute.DefaultValue != null)
				{
					element.AddAnnotation(new XAttribute(data.Key, data.Value));
				}

				element.SetAttributeValue(data.Key, data.Value);
			}
		}

		private static KeyValuePair<XName, object> GetDefault(this XmlSchemaAttribute attribute)
		{
			var name = attribute.GetXName();
			var value = attribute.EvaluateDefault<object>();

			return new KeyValuePair<XName, object>(name, value);
		}

		public static TValue EvaluateDefault<TValue>(this XmlSchemaAttribute attribute)
		{
			object value = attribute.DefaultValue ?? attribute.FixedValue;

			if (value == null)
			{
				value = attribute.AttributeSchemaType.GetDefaultValue();

				var valueAsString = value as string;

				if (valueAsString != null && valueAsString.Length == 0)
				{
					value = null;
				}
			}

			return (TValue) value;
		}

		private static IEnumerable<XNode> CreateRequiredNodes(this XmlSchemaType schemaType, Func<XmlSchemaElement, XElement> factory)
		{
			var simple = schemaType as XmlSchemaSimpleType;

			if (simple != null)
			{
				yield return new XText(simple.GetDefaultValue().ToString());
				yield break;
			}

			var complex = (XmlSchemaComplexType) schemaType;

			if (complex.ContentType == XmlSchemaContentType.TextOnly)
			{
				yield return new XText(string.Empty);
			}
			else if (complex.ContentType != XmlSchemaContentType.Empty)
			{
				foreach (var node in complex.Particle.CreateRequiredNodes(factory))
				{
					yield return node;
				}
			}
		}

		private static IEnumerable<XNode> CreateRequiredNodes(this XmlSchemaParticle particle, Func<XmlSchemaElement, XElement> factory)
		{
			if (particle.MinOccurs > 0)
			{
				var nodes = particle.Switch(
					element => new[] { factory(element) },
					groupRef => groupRef.Particle.CreateRequiredNodes(factory).ToArray(),
					sequence => sequence.CreateRequiredNodes(factory).ToArray(),
					choice => choice.CreateRequiredNodes(factory).ToArray(),
					// Perhaps it's strange to require any particle, but I guess it's possible.
					// TODO: Handle the namespace attribute; e.g., ##other
					onAny: _ => new[] { XVisitor.CreateAny() });

				if (nodes.Length > 0)
				{
					for (int i = 0; i < particle.MinOccurs; i++)
					{
						foreach (var node in nodes)
						{
							yield return node;
						}
					}
				}
			}
		}

		private static IEnumerable<XNode> CreateRequiredNodes(this XmlSchemaSequence sequence, Func<XmlSchemaElement, XElement> factory)
		{
			return sequence.Items.Cast<XmlSchemaParticle>().SelectMany(particle => particle.CreateRequiredNodes(factory));
		}

		private static IEnumerable<XNode> CreateRequiredNodes(this XmlSchemaChoice choice, Func<XmlSchemaElement, XElement> factory)
		{
			/* Previously, deep evaluation was used by calling particle.CreateRequiredNodes to select the first particle that didn't 
			 * return any nodes; however, this caused an infinite recursion for the MAML listType because it contains a choice between 
			 * listItem and conditionalSet elements, where conditionalSet is also listType.  MinOccurs is 1 for conditonalSet, which is 
			 * allowed because it's within a choice group.  Deep evaluation causes a StackOverflowException as it tries to find the first 
			 * descendent choice without any nodes by recursively evaluating listType/conditionalSet.
			 * 
			 * The solution is to check for MinOccurs == 0.  For listType, no elements will match, which means that a choice has to be 
			 * made.  Previously, the point of deep evaluation was to select the first empty particle, but the next best and simplest 
			 * behavior for avoiding StackOverflowException is to choose the first required particle instead, even if it's not empty.
			 */
			var isOptional = choice.Items.Count == 0
										|| choice.Items.Cast<XmlSchemaParticle>().Any(particle => particle.MinOccurs == 0);

			return isOptional
					 ? Enumerable.Empty<XNode>()
					 : ((XmlSchemaParticle) choice.Items[0]).CreateRequiredNodes(factory);
		}

		[DebuggerStepThrough]
		public static bool IsMaxOccursUnbounded(this XmlSchemaParticle particle)
		{
			return particle.MaxOccursString == "unbounded";
		}

		public static bool ValidateChildren(
			this XmlSchemaElement elementSchema,
			XmlSchemaSet schemas,
			XElement element,
			XElement stopAtChild = null,
			ICollection<XElement> missingRequiredElementsAdded = null,
			IList<XElement> elementPool = null,
			Func<XmlSchemaElement, XElement> defaultElementFactory = null)
		{
			XmlSchemaParticlesExpected expectedBeforeChild, expectedAfterChild;
			return ValidateChildren(elementSchema, schemas, element, false, out expectedBeforeChild, out expectedAfterChild, stopAtChild, missingRequiredElementsAdded, elementPool, defaultElementFactory);
		}

		public static bool ValidateChildren(
			this XmlSchemaElement elementSchema,
			XmlSchemaSet schemas,
			XElement element,
			out XmlSchemaParticlesExpected expectedBeforeChild,
			out XmlSchemaParticlesExpected expectedAfterChild,
			XElement stopAtChild = null,
			ICollection<XElement> missingRequiredElementsAdded = null,
			IList<XElement> elementPool = null,
			Func<XmlSchemaElement, XElement> defaultElementFactory = null)
		{
			return ValidateChildren(elementSchema, schemas, element, true, out expectedBeforeChild, out expectedAfterChild, stopAtChild, missingRequiredElementsAdded, elementPool, defaultElementFactory);
		}

		// See UPA (http://www.w3.org/wiki/UniqueParticleAttribution) (http://msdn.microsoft.com/en-us/library/ms187822.aspx)
		private static bool ValidateChildren(
			this XmlSchemaElement elementSchema,
			XmlSchemaSet schemas,
			XElement element,
			bool captureExpectedElements,
			out XmlSchemaParticlesExpected expectedBeforeChild,
			out XmlSchemaParticlesExpected expectedAfterChild,
			XElement stopAtChild = null,
			ICollection<XElement> missingRequiredElementsAdded = null,
			IList<XElement> elementPool = null,
			Func<XmlSchemaElement, XElement> defaultElementFactory = null)
		{
			Contract.Requires(elementSchema != null);
			Contract.Requires(schemas != null);
			Contract.Requires(element != null);

			var simple = elementSchema.ElementSchemaType as XmlSchemaSimpleType;

			if (simple != null)
			{
				if (captureExpectedElements)
				{
					expectedBeforeChild = new XmlSchemaParticlesExpected();
					expectedAfterChild = new XmlSchemaParticlesExpected();
				}
				else
				{
					expectedBeforeChild = null;
					expectedAfterChild = null;
				}

				return true;
			}

			var complex = (XmlSchemaComplexType) elementSchema.ElementSchemaType;

			if (complex.ContentType == XmlSchemaContentType.Empty
				|| complex.ContentType == XmlSchemaContentType.TextOnly)
			{
				var childCount = element.Elements().Count();

				if (captureExpectedElements)
				{
					expectedBeforeChild = new XmlSchemaParticlesExpected();
					expectedAfterChild = new XmlSchemaParticlesExpected();
				}
				else
				{
					expectedBeforeChild = null;
					expectedAfterChild = null;
				}

				return childCount == 0;
			}

			var context = new XmlSchemaValidationContext(schemas, element, stopAtChild, missingRequiredElementsAdded, captureExpectedElements, elementPool, defaultElementFactory);

			var existingElementsAreValid = complex.Particle.Switch(
				_ => true,
				groupRef => groupRef.ValidateGroup(context),
				sequence => sequence.ValidateGroup(context),
				choice => choice.ValidateGroup(context),
				_ => true);

			// CC rewriter outputs invalid IL (or PDB info - I'm not sure which).  It appears to be due to the coalescing operator.
			// As a result, the local variable "context" is null in the debugger even though its actually not null at runtime.
			var tempValid = existingElementsAreValid ?? true;

			Contract.Assume(tempValid == context.IsValid);

			context.CompleteValidation();

			expectedBeforeChild = context.ExpectedElementsBeforeNode;
			expectedAfterChild = context.ExpectedElementsAfterNode;

			return context.IsValid;
		}

		[DebuggerStepThrough]
		private static bool? ValidateGroup(this XmlSchemaGroupRef group, XmlSchemaValidationContext context)
		{
			return group.ValidateOccurrences(context, () => group.Particle.Switch(
				sequence => sequence.ValidateGroup(context),
				choice => choice.ValidateGroup(context)));
		}

		private static bool? ValidateGroup(this XmlSchemaSequence sequence, XmlSchemaValidationContext context)
		{
			var inheritOptional = context.NextParticleIsOptionalInheritedDeeply;

			context.PushGroup(
				sequence,
				nextParticleIsOptionalInherited: sequence.MinOccurs == 0 || inheritOptional);

			var isSequenceValid = sequence.ValidateOccurrences(
				context,
				validateRequiredOccurrence: occurrencesRemaining =>
				{
					bool? allValid = null;

					foreach (var particle in sequence.Items.Cast<XmlSchemaParticle>())
					{
						var isValid = particle.ValidateNext(context) ?? true;

						context.NextParticleIsOptionalInherited = false;

						if (!isValid && context.AddMissingRequiredElements)
						{
							foreach (var node in particle.CreateRequiredNodes(context.CreateElement))
							{
								var element = node as XElement;

								if (element != null)
								{
									context.MissingRequiredElements.Add(element);
								}

								context.InsertNode(node);
							}

							isValid = true;
						}

						if (allValid.HasValue)
						{
							allValid &= isValid;
						}
						else
						{
							allValid = isValid;
						}
					}

					return allValid;
				},
				validateOptionalOccurrence: (requiredButInheritedOptional, occurrencesRemaining) =>
				{
					context.NextParticleIsOptionalInherited = true;

					return sequence.Items.Cast<XmlSchemaParticle>().All(particle => particle.ValidateNext(context) ?? true);
				},
				overrideInheritOptional: inheritOptional);

			context.PopGroup();

			return isSequenceValid;
		}

		private static bool? ValidateGroup(this XmlSchemaChoice choice, XmlSchemaValidationContext context)
		{
			var inheritOptional = context.NextParticleIsOptionalInheritedDeeply;

			context.PushGroup(choice, nextParticleIsOptionalInherited: true);

			var isChoiceValid = choice.ValidateOccurrences(
				context,
				validateRequiredOccurrence: occurrencesRemaining =>
				{
					var anyOptionalMatches = false;

					if (choice.Items.Cast<XmlSchemaParticle>()
						.Select((particle, index) => new { particle, index })
						.Any(p =>
						{
							if (p.index + 1 == choice.Items.Count)
							{
								// If we make it as far as the final choice without matching, then the final choice is not optional.
								context.NextParticleIsOptionalInherited = false;
							}

							var isValid = p.particle.ValidateNext(context);

							anyOptionalMatches |= !isValid.HasValue;

							return isValid ?? false;
						})
						|| anyOptionalMatches)
					{
						return true;
					}

					if (context.AddMissingRequiredElements)
					{
						foreach (var node in choice.CreateRequiredNodes(context.CreateElement))
						{
							context.InsertNode(node);
						}

						return true;
					}

					return false;
				},
				validateOptionalOccurrence: (requiredButInheritedOptional, occurrencesRemaining) =>
				{
					context.NextParticleIsOptionalInherited = true;

					var items = choice.Items.Cast<XmlSchemaParticle>();

					var anyOptionalMatches = false;

					if (items.Any(particle =>
						{
							var isValid = particle.ValidateNext(context);

							anyOptionalMatches |= !isValid.HasValue;

							return isValid ?? false;
						}))
					{
						return true;
					}

					if (anyOptionalMatches)
					{
						return null;
					}

					return false;
				},
				overrideInheritOptional: inheritOptional);

			context.PopGroup();

			return isChoiceValid;
		}

		[DebuggerStepThrough]
		private static bool? ValidateNext(this XmlSchemaParticle particle, XmlSchemaValidationContext context)
		{
			return particle.Switch(
				element => element.ValidateName(context),
				group => group.ValidateGroup(context),
				sequence => sequence.ValidateGroup(context),
				choice => choice.ValidateGroup(context),
				any => any.ValidateName(context));
		}

		private static bool? ValidateName(this XmlSchemaElement element, XmlSchemaValidationContext context)
		{
			return element.ValidateName(element.GetXName(), context);
		}

		private static bool? ValidateName(this XmlSchemaAny any, XmlSchemaValidationContext context)
		{
			return any.ValidateName(null, context);
		}

		private static bool? ValidateName(this XmlSchemaParticle particle, XName name, XmlSchemaValidationContext context)
		{
			/* Does MaxOccurs = 0 indicate no-op or does it mean a match = failure?
			 * 
			 * w3.org states: "unless minOccurs=maxOccurs=0, in which case the item corresponds to no component at all"
			 * http://www.w3.org/TR/2004/REC-xmlschema-1-20041028/structures.html#element-element; §3.3.2
			 * 
			 * But that's not exactly clear.  I'm assuming no-op, which means that currentIndex isn't modified when MaxOccurs = 0
			 */
			return particle.ValidateOccurrences(
				context,
				validateRequiredOccurrence: occurrencesRemaining =>
				{
					// TODO: Invalid documents should not necessarily call TakeElement here, though in some cases it's correct.
					// Figure out why and how to control insertion suggestions within an invalid document.
					int currentIndexMoved;
					var element = context.TakeElement(out currentIndexMoved);

					var isMatch = element != null && (name == null || element.Name == name);

					if (isMatch)
					{
						context.Matched(element, particle, true, occurrencesRemaining, currentIndexMoved);
					}

					return isMatch;
				},
				validateOptionalOccurrence: (requiredButInheritedOptional, occurrencesRemaining) =>
				{
					int nextIndex;
					var element = context.PeekElement(out nextIndex);

					var isMatch = element != null && (name == null || element.Name == name);

					if (isMatch)
					{
						var currentIndexMoved = nextIndex - context.CurrentIndex;

						context.CurrentIndex = nextIndex;

						context.Matched(element, particle, requiredButInheritedOptional, occurrencesRemaining, currentIndexMoved);
					}

					return isMatch;
				});
		}

		[DebuggerStepThrough]
		private static bool? ValidateOccurrences(this XmlSchemaParticle particle, XmlSchemaValidationContext context, Func<bool?> validateOccurrence)
		{
			return particle.ValidateOccurrences(context, _ => validateOccurrence(), (_, __) => validateOccurrence());
		}

		/// <returns>
		/// If there are required occurrences, returns true if all required occurrences match; otherwise, false.
		/// If there are no required occurrences, returns true if one or more optional occurrences match; 
		/// otherwise, returns null to indicate that there were no required occurrences and no matches.
		/// </returns>
		private static bool? ValidateOccurrences(this XmlSchemaParticle particle, XmlSchemaValidationContext context, Func<int, bool?> validateRequiredOccurrence, Func<bool, int, bool?> validateOptionalOccurrence, bool? overrideInheritOptional = null)
		{
			var inheritedOptional = overrideInheritOptional ?? context.NextParticleIsOptionalInherited;

			var hasOptionalMatch = false;

			var requiredCount = particle.MinOccurs;
			var occurrencesRemaining = particle.MaxOccursValueMinusOne();

			if (requiredCount > 0 && inheritedOptional)
			{
				// Default to true since having no nested optional matches is valid; e.g., sequence(0..1) -contains-> groupRef(1) -contains-> sequence(0..1)
				// (Not sure about this.)
				if (validateOptionalOccurrence(inheritedOptional, occurrencesRemaining) ?? true)
				{
					occurrencesRemaining = MaxOccursPlus(occurrencesRemaining, -1);
					hasOptionalMatch = true;
					requiredCount--;
				}
				else
				{
					return false;
				}
			}

			for (int i = 0; i < requiredCount; i++)
			{
				// Default to true since a required match having no nested optional matches is valid; e.g., groupRef(1..1) contains sequence(0..1)
				if (!validateRequiredOccurrence(occurrencesRemaining) ?? true)
				{
					if (!context.AddMissingRequiredElements)
					{
						context.IsValid = false;
					}

					return false;
				}

				occurrencesRemaining = MaxOccursPlus(occurrencesRemaining, -1);
			}

			var remainder = MaxOccursPlus(occurrencesRemaining, 1);

			Contract.Assume(remainder == (particle.IsMaxOccursUnbounded() ? int.MaxValue : ((int) particle.MaxOccurs - particle.MinOccurs)));

			context.IsValidatingOptionalOccurrences = true;

			for (int i = 0; i < remainder && (validateOptionalOccurrence(false, occurrencesRemaining) ?? false); i++)
			{
				occurrencesRemaining = MaxOccursPlus(occurrencesRemaining, -1);
				hasOptionalMatch = true;
			}

			context.IsValidatingOptionalOccurrences = false;

			return requiredCount > 0 || hasOptionalMatch ? (bool?) true : null;
		}

		[DebuggerStepThrough]
		private static int MaxOccursValueMinusOne(this XmlSchemaParticle particle)
		{
			return particle.IsMaxOccursUnbounded() ? int.MaxValue : ((int) particle.MaxOccurs - 1);
		}

		[DebuggerStepThrough]
		private static int MaxOccursPlus(int maxOccursValue, int value)
		{
			return maxOccursValue == int.MaxValue ? int.MaxValue : maxOccursValue + value;
		}

		private static IEnumerable<XmlSchemaParticle> GetExpectedParticles(this XmlSchemaParticle particle)
		{
			return particle.Switch(
				onElement: element => new[] { element },
				onGroup: GetExpectedParticles,
				onSequence: sequence => GetExpectedParticles(sequence, null),
				onChoice: GetExpectedParticles,
				onAny: any => new XmlSchemaParticle[0]);
		}

		private static IEnumerable<XmlSchemaParticle> GetExpectedParticles(this XmlSchemaGroupRef group)
		{
			return group.Particle.GetExpectedParticles();
		}

		internal static IEnumerable<XmlSchemaParticle> GetExpectedParticles(this XmlSchemaGroupBase group, XmlSchemaParticle childBeforeExpectedParticles = null)
		{
			return group.Switch(
				sequence => GetExpectedParticles(sequence, childBeforeExpectedParticles),
				onChoice: GetExpectedParticles);
		}

		private static IEnumerable<XmlSchemaParticle> GetExpectedParticles(this XmlSchemaSequence sequence, XmlSchemaParticle childBeforeExpectedParticles = null)
		{
			var items = sequence.Items.Cast<XmlSchemaParticle>();

			if (childBeforeExpectedParticles != null)
			{
				XmlSchemaParticle previousParticle = null;

				items = items.SkipWhile(particle =>
					{
						var skip = previousParticle != childBeforeExpectedParticles
										&& (particle != childBeforeExpectedParticles || particle.MaxOccursValueMinusOne() <= 0);

						if (skip)
						{
							previousParticle = particle;
						}

						return skip;
					});
			}

			var foundRequiredElement = false;

			return items
				.TakeWhile(particle =>
					{
						if (foundRequiredElement)
						{
							return false;
						}

						foundRequiredElement = particle.MinOccurs > 0;

						return true;
					})
				.SelectMany(GetExpectedParticles);
		}

		private static IEnumerable<XmlSchemaParticle> GetExpectedParticles(this XmlSchemaChoice choice)
		{
			return choice.Items.Cast<XmlSchemaParticle>().SelectMany(GetExpectedParticles);
		}
	}
}