using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Schema;
using DaveSexton.XmlGel.Xml;

namespace DaveSexton.XmlGel.Extensions
{
	internal sealed class XmlSchemaValidationContext
	{
		public int CurrentIndex
		{
			get
			{
				return currentIndex;
			}
			set
			{
				currentIndex = value;
			}
		}

		public bool NextParticleIsOptionalInherited
		{
			get
			{
				// In testing, groups was empty when the root particle was XmlSchemaGroupRef instead of XmlSchemaGroupBase.
				return groups.Count > 0
						&& groups.Peek().IsNextParticleOptional;
			}
			set
			{
				groups.Peek().IsNextParticleOptional = value;
			}
		}

		public bool NextParticleIsOptionalInheritedDeeply
		{
			get
			{
				return IsValidatingOptionalOccurrences || groups.Any(frame => frame.IsNextParticleOptional);
			}
		}

		public bool IsValidatingOptionalOccurrences
		{
			get
			{
				return validatingOptionalOccurrences > 0;
			}
			set
			{
				if (value)
				{
					validatingOptionalOccurrences++;
				}
				else
				{
					validatingOptionalOccurrences--;
				}

				Contract.Assume(validatingOptionalOccurrences >= 0);
			}
		}

		public bool AddMissingRequiredElements
		{
			get
			{
				return missingRequiredElements != null;
			}
		}

		public ICollection<XElement> MissingRequiredElements
		{
			get
			{
				return missingRequiredElements;
			}
		}

		public XmlSchemaParticlesExpected ExpectedElementsBeforeNode
		{
			get
			{
				return expectedElementsBeforeNode;
			}
		}

		public XmlSchemaParticlesExpected ExpectedElementsAfterNode
		{
			get
			{
				return expectedElementsAfterNode;
			}
		}

		public bool IsValid
		{
			get
			{
				return isValid;
			}
			set
			{
				Contract.Requires(!value || IsValid);

				isValid = value;
			}
		}

		private readonly XmlSchemaSet schemas;
		private readonly XElement validatingElement, matchChild;
		private readonly IList<XNode> nodes;
		private readonly ICollection<XElement> missingRequiredElements;
		private readonly bool captureExpectedElements;
		private readonly IList<XElement> elementPool;
		private readonly Func<XmlSchemaElement, XElement> defaultElementFactory;
		private readonly Stack<GroupFrame> groups = new Stack<GroupFrame>();
		private bool hasPushedFirstGroup, foundMatch, foundMatchBeforeRequiredMatch, isValid = true;
		private bool? expectedElementsBeforeNodeIsSequence;
		private int currentIndex, currentMatchOccurrencesRemaining, validatingOptionalOccurrences;
		private XmlSchemaParticlesExpected expectedElementsBeforeNode, expectedElementsAfterNode;
		private XmlSchemaParticle currentMatch;
		private GroupFrame lastGroup;

		public XmlSchemaValidationContext(XmlSchemaSet schemas, XElement element, XElement matchChild, ICollection<XElement> missingRequiredElements, bool captureExpectedElements, IList<XElement> elementPool, Func<XmlSchemaElement, XElement> defaultElementFactory)
		{
			this.schemas = schemas;
			this.validatingElement = element;
			this.matchChild = matchChild;
			this.nodes = element.Nodes().ToList();
			this.missingRequiredElements = missingRequiredElements;
			this.captureExpectedElements = captureExpectedElements;
			this.elementPool = elementPool ?? new XElement[0];
			this.defaultElementFactory = defaultElementFactory ?? (e => XmlSchemaExtensions.CreateRequiredElementTree(e, schemas));
		}

		public void PushGroup(XmlSchemaGroupBase group, bool nextParticleIsOptionalInherited)
		{
			var frame = new GroupFrame(group)
				{
					IsNextParticleOptional = nextParticleIsOptionalInherited
				};

			groups.Push(frame);

			if (captureExpectedElements && !hasPushedFirstGroup)
			{
				int nextIndex;
				var nextSibling = PeekElement(out nextIndex);

				if (nextSibling == matchChild)
				{
					bool isSequence;
					expectedElementsBeforeNode = CaptureExpectedParticles(isRequired: false, isSequence: out isSequence);
					expectedElementsBeforeNodeIsSequence = isSequence;
				}

				/* This field MUST be used instead of checking groups.Count == 1, because when the root particle is XmlSchemaGroupRef, the root 
				 * group may be popped and pushed multiple times while validating occurrences of the XmlSchemaGroupRef particle.
				 */
				hasPushedFirstGroup = true;
			}
		}

		public void PopGroup()
		{
			lastGroup = groups.Pop();
		}

		public void CompleteValidation()
		{
			Contract.Ensures(!captureExpectedElements || expectedElementsBeforeNode != null);
			Contract.Ensures(!captureExpectedElements || expectedElementsAfterNode != null);

			if (currentIndex < nodes.Count)
			{
				isValid = false;
			}

			if (captureExpectedElements)
			{
				Contract.Assume(expectedElementsBeforeNode != null || !isValid);

				if (expectedElementsBeforeNode == null)
				{
					expectedElementsBeforeNode = new XmlSchemaParticlesExpected();
				}

				if (expectedElementsAfterNode == null)
				{
					if (matchChild == null)
					{
						bool isSequence;
						expectedElementsAfterNode = CaptureExpectedParticles(isRequired: false, isSequence: out isSequence);

						if (currentMatchOccurrencesRemaining == 0 && currentMatch != null)
						{
							expectedElementsAfterNode.Remove(currentMatch);
						}
					}
					else
					{
						Contract.Assume(!isValid);

						expectedElementsAfterNode = new XmlSchemaParticlesExpected();
					}
				}

				// TODO: Remove unmatched following siblings from expected collection if they are required and the stop element is not the last sibling.
				// e.g., a conceptual doc looks like: 
				//	<docRoot><intro /><section /></docRoot>
				// though it's missing <relatedTopics />
				// The goal is not to show relatedTopics as an option after <intro />.  It should only be an option after <section />.
				// The general pattern is: remove all required unmatched siblings from all expected collections up to the last sibling that
				// has it in its expected collection.
				// This required expected collections to be gathered for all elements, not just the target element.
			}
		}

		public void InsertNode(XNode node)
		{
			if (currentIndex == nodes.Count)
			{
				validatingElement.Add(node);
			}
			else
			{
				nodes[currentIndex].AddBeforeSelf(node);
			}

			nodes.Insert(currentIndex++, node);
		}

		public void Matched(XElement element, XmlSchemaParticle schema, bool isRequired, int occurrencesRemaining, int currentIndexMoved)
		{
			currentMatch = schema;
			currentMatchOccurrencesRemaining = occurrencesRemaining;

			if (foundMatch)
			{
				if (captureExpectedElements)
				{
					Contract.Assume(expectedElementsBeforeNode != null);
					Contract.Assume(expectedElementsAfterNode != null);

					SynchronizePreviousExpectedParticles();
				}

				if (isRequired)
				{
					foundMatchBeforeRequiredMatch = true;
				}
			}
			else if (element == matchChild)
			{
				foundMatch = true;

				if (captureExpectedElements)
				{
					SynchronizePreviousExpectedParticles();

					bool isSequence;
					expectedElementsAfterNode = CaptureExpectedParticles(isRequired, out isSequence);

					if (occurrencesRemaining == 0)
					{
						expectedElementsAfterNode.Remove(currentMatch);
					}
				}
			}
			else if (captureExpectedElements)
			{
				int nextIndex;
				var nextSibling = PeekElement(out nextIndex);

				if (nextSibling == matchChild)
				{
					bool isSequence;
					expectedElementsBeforeNode = CaptureExpectedParticles(isRequired, out isSequence);
					expectedElementsBeforeNodeIsSequence = isSequence;
				}
			}
		}

		private void SynchronizePreviousExpectedParticles()
		{
			if (!foundMatchBeforeRequiredMatch && currentMatchOccurrencesRemaining == 0)
			{
				Contract.Assume(expectedElementsBeforeNode != null || !isValid);

				if (expectedElementsBeforeNode != null)
				{
					if (expectedElementsBeforeNodeIsSequence ?? false)
					{
						expectedElementsBeforeNodeIsSequence = null;

						for (int i = expectedElementsBeforeNode.Count - 1; expectedElementsBeforeNode[i] != currentMatch; i--)
						{
							expectedElementsBeforeNode.RemoveAt(i);
						}
					}

					expectedElementsBeforeNode.Remove(currentMatch);
				}

				if (expectedElementsAfterNode != null)
				{
					expectedElementsAfterNode.Remove(currentMatch);
				}
			}
		}

		private XmlSchemaParticlesExpected CaptureExpectedParticles(bool isRequired, out bool isSequence)
		{
			var expected = new XmlSchemaParticlesExpected(GetExpectedParticles(out isSequence));

			//expected.IsRequired = isRequired;

			return expected;
		}

		private IEnumerable<XmlSchemaParticle> GetExpectedParticles(out bool isSequence)
		{
			var frame = groups.Count == 0 ? lastGroup : groups.Peek();

			Contract.Assume(frame != null);

			isSequence = frame.Group is XmlSchemaSequence;

			// TODO: Fix group context logic; e.g., current sequence or product of ancestor choices?
			return frame.Group.GetExpectedParticles(currentMatch);
		}

		public XElement CreateElement(XmlSchemaElement schema)
		{
			var name = schema.GetXName();

			var element = TakeElementFromPool(name);

			if (element == null)
			{
				element = defaultElementFactory(schema);

				if (element == null)
				{
					element = schema.CreateRequiredElementTree(schemas);
				}
			}
			else
			{
				element.SetSchema(schema);
			}

			return element;
		}

		private XElement TakeElementFromPool(XName name)
		{
			for (int i = 0; i < elementPool.Count; i++)
			{
				var element = elementPool[i];

				if (element.Name == name)
				{
					elementPool.RemoveAt(i);

					return element;
				}
			}

			return null;
		}

		public XElement PeekElement(out int nextIndex)
		{
			nextIndex = currentIndex;

			return AdvanceToNextNonErrorElement(ref nextIndex);
		}

		public XElement TakeElement(out int currentIndexMoved)
		{
			var index = currentIndex;

			var element = AdvanceToNextNonErrorElement(ref index);

			currentIndexMoved = index - currentIndex;
			currentIndex = index;

			return element;
		}

		private XElement AdvanceToNextNonErrorElement(ref int index)
		{
			XElement element = null;

			for (; element == null && index < nodes.Count; index++)
			{
				element = nodes[index] as XElement;

				if (element != null && element.IsSchemaError())
				{
					element = null;
				}
			}

			return element;
		}

		private sealed class GroupFrame
		{
			public XmlSchemaGroupBase Group
			{
				get
				{
					return group;
				}
			}

			public bool IsNextParticleOptional
			{
				get;
				set;
			}

			private readonly XmlSchemaGroupBase group;

			public GroupFrame(XmlSchemaGroupBase group)
			{
				this.group = group;
			}
		}
	}
}