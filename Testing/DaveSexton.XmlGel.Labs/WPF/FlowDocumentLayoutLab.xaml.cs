using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using DaveSexton.XmlGel.Extensions;
using DaveSexton.XmlGel.Maml.Documents.Adorners;

namespace DaveSexton.XmlGel.Labs.WPF
{
	public partial class FlowDocumentLayoutLab : BaseLab
	{
		// Used by internal code (see "FlowDocumentLayoutLab - Internal.cs")
		public struct MeasurementContext
		{
			private readonly TextElement element, parent, firstContentDescendant, lastContentDescendant;
			private readonly Rect documentBox, union;
			private readonly IList<TextElement> siblings;
			private readonly int currentSiblingIndex, leadingConsecutiveEmptyDescendantsCount, trailingConsecutiveEmptyDescendantsCount;
			private readonly bool isFirstChild, isLastChild, isEmptySection;
			private Rect box, parentBox;
			private bool isBoxCalculated;

			private MeasurementContext(TextElement element, Rect documentBox)
			{
				this.element = element;
				this.documentBox = documentBox;

				box = Rect.Empty;
				parentBox = Rect.Empty;
				isBoxCalculated = false;

				var start = element.ElementStart.GetCharacterRect(LogicalDirection.Forward);
				var end = element.ElementEnd.GetCharacterRect(LogicalDirection.Backward);

				union = Rect.Union(start, end);

				parent = element.Parent as TextElement;
				siblings = ((FrameworkContentElement) element.Parent).TryGetChildCollection().Cast<TextElement>().ToList();
				currentSiblingIndex = siblings.IndexOf(element);

				isFirstChild = currentSiblingIndex == 0;
				isLastChild = currentSiblingIndex == siblings.Count - 1;

				if (isFirstChild && isLastChild)
				{
					isEmptySection = false;		// not used
					leadingConsecutiveEmptyDescendantsCount = 0;
					trailingConsecutiveEmptyDescendantsCount = 0;
					firstContentDescendant = null;
					lastContentDescendant = null;
				}
				else
				{
					isEmptySection = IsEmptySection(element, out leadingConsecutiveEmptyDescendantsCount, out trailingConsecutiveEmptyDescendantsCount, out firstContentDescendant, out lastContentDescendant);
				}
			}

			[DebuggerStepThrough]
			public static Rect MeasureLogicalBox(TextElement element, Rect documentBox)
			{
				var context = new MeasurementContext(element, documentBox);

				return context.MeasureLogicalBox();
			}

			private Rect MeasureLogicalBox()
			{
				var isOnlyChild = isFirstChild && isLastChild;

				if (!isOnlyChild
					&& (isEmptySection
						|| leadingConsecutiveEmptyDescendantsCount > 0		// Required by lab: MeasureSectionThenNestedSectionAndParagraph
						|| trailingConsecutiveEmptyDescendantsCount > 0))	// Required by lab: MeasureNestedParagraphAndSectionThenSection
				{
					MeasureConsecutiveEmptySectionsPartition();
				}

				// Contract.Assume(isBoxCalculated == !parentBox.IsEmpty);

				if (isLastChild)
				{
					if (!isBoxCalculated)
					{
						if (isFirstChild)
						{
							if (parentBox.IsEmpty)
							{
								parentBox = parent != null ? MeasureLogicalBox(parent, documentBox) : documentBox;
							}

							box = parentBox;
						}
						else
						{
							box = union;
							box.X = documentBox.X;
							box.Width = documentBox.Width;
						}

						// isBoxCalculated must remain false
					}

					if (!isFirstChild)
					{
						var nextSiblingOrAncestor = element.GetFollowingSiblingOrAncestor();
						var isLastDescendant = nextSiblingOrAncestor == null;

						if (isLastDescendant)
						{
							if (!isBoxCalculated)
							{
								box.Y = union.Y;		// Required by lab MeasureSectionThenSection
							}

							box.Height = documentBox.Height - box.Y;
						}
					}
				}
				else if (!isBoxCalculated)
				{
					box = union;
					box.X = documentBox.X;
					box.Width = documentBox.Width;
				}

				// Contract.Assume(!box.IsEmpty);

				return box;
			}

			private void MeasureConsecutiveEmptySectionsPartition()
			{
				/* See the IsEmptySection method for the definition of "empty section".
				 * 
				 * 1. An only child has the same logical box as its parent.  (Implemented before this method is called.)
				 * 2. Empty consecutive sections must be evenly partitioned among the space between the previous non-empty descendant, sibling or 
				 *		sibling's trailing non-empty descendant and the following non-empty descendant, sibling or sibling's leading non-empty 
				 *		descendant, or the documentBox - whichever comes first.
				 */

				int previousConsecutiveEmptySiblingsCount, previousConsecutiveEmptyDescendantsCount;
				var previousContent = GetPreviousSiblingContent(out previousConsecutiveEmptySiblingsCount, out previousConsecutiveEmptyDescendantsCount);

				int followingConsecutiveEmptySiblingsCount, followingConsecutiveEmptyDescendantsCount;
				var followingContent = GetFollowingSiblingContent(out followingConsecutiveEmptySiblingsCount, out followingConsecutiveEmptyDescendantsCount);

				int previousConsecutiveEmptyAncestorsCount = 0, followingConsecutiveEmptyAncestorsCount = 0;

				if (previousContent == null)
				{
					previousContent = GetPreviousAncestorContent(parent, out previousConsecutiveEmptyAncestorsCount);
				}

				if (followingContent == null)
				{
					followingContent = GetFollowingAncestorContent(parent, out followingConsecutiveEmptyAncestorsCount);
				}

				var isDocumentEmpty = isEmptySection && previousContent == null && followingContent == null;

				if (!isDocumentEmpty)	// Required by lab MeasureSectionThenSection.  Union is correct in an empty document; only content causes an empty section's union to be collapsed.
				{
					parentBox = parent != null ? MeasureLogicalBox(parent, documentBox) : documentBox;

					var previousBottom = previousContent != null
						? MeasureLogicalBox(previousContent, documentBox).Bottom
						: documentBox.Top;

					var previousOrDescendantBottom = isEmptySection
						? previousBottom
						: MeasureLogicalBox(lastContentDescendant, documentBox).Bottom;

					var followingTop = followingContent != null
						? MeasureLogicalBox(followingContent, documentBox).Top
						: documentBox.Bottom;

					var followingOrDescendantTop = isEmptySection
						? followingTop
						: MeasureLogicalBox(firstContentDescendant, documentBox).Top;

					var leadingIndex = previousConsecutiveEmptySiblingsCount + previousConsecutiveEmptyDescendantsCount + previousConsecutiveEmptyAncestorsCount;
					var trailingIndex = followingConsecutiveEmptySiblingsCount + followingConsecutiveEmptyDescendantsCount + followingConsecutiveEmptyAncestorsCount;

					var leadingConsecutiveEmptySectionsCount = leadingIndex;
					var trailingConsecutiveEmptySectionsCount = trailingIndex;

					if (isEmptySection)
					{
						leadingConsecutiveEmptySectionsCount++;
						trailingConsecutiveEmptySectionsCount++;
					}
					else
					{
						leadingConsecutiveEmptySectionsCount += leadingConsecutiveEmptyDescendantsCount;
						trailingConsecutiveEmptySectionsCount += trailingConsecutiveEmptyDescendantsCount;
					}

					var leadingPartitionedHeight = leadingConsecutiveEmptySectionsCount == 0
						? 0
						: (followingOrDescendantTop - previousBottom) / leadingConsecutiveEmptySectionsCount;

					var trailingPartitionedHeight = trailingConsecutiveEmptySectionsCount == 0
						? 0
						: (followingTop - previousOrDescendantBottom) / trailingConsecutiveEmptySectionsCount;

					box = parentBox;
					box.Y = previousBottom + (leadingPartitionedHeight * leadingIndex);
					box.Height = followingTop - (trailingPartitionedHeight * trailingIndex) - box.Y;

					isBoxCalculated = true;
				}
			}

			private TextElement GetPreviousSiblingContent(out int consecutiveEmptySiblingsCount, out int consecutiveEmptyDescendantsCount)
			{
				TextElement previousContent = null;
				int descendantsCount = 0;

				consecutiveEmptySiblingsCount = siblings
					.Reverse()
					.Skip(siblings.Count - (currentSiblingIndex + 1) + 1)
					.TakeWhile(sibling =>
					{
						TextElement ignoreContent, content;
						int ignore, count;
						var isSiblingEmpty = IsEmptySection(sibling, out ignore, out count, out ignoreContent, out content);

						if (!isSiblingEmpty)
						{
							if (previousContent == null)
							{
								previousContent = content;
							}

							descendantsCount += count;
						}

						return isSiblingEmpty;
					})
					.Count();

				consecutiveEmptyDescendantsCount = descendantsCount;

				return previousContent;
			}

			private TextElement GetFollowingSiblingContent(out int consecutiveEmptySiblingsCount, out int consecutiveEmptyDescendantsCount)
			{
				TextElement followingContent = null;
				int descendantsCount = 0;

				consecutiveEmptySiblingsCount = siblings
					.Skip(currentSiblingIndex + 1)
					.TakeWhile(sibling =>
					{
						TextElement ignoreContent, content;
						int ignore, count;
						var isSiblingEmpty = IsEmptySection(sibling, out count, out ignore, out content, out ignoreContent);

						if (!isSiblingEmpty)
						{
							followingContent = content;
							descendantsCount += count;
						}

						return isSiblingEmpty;
					})
					.Count();

				consecutiveEmptyDescendantsCount = descendantsCount;

				return followingContent;
			}

			private static TextElement GetPreviousAncestorContent(FrameworkContentElement element, out int consecutiveEmptySectionsCount)
			{
				consecutiveEmptySectionsCount = 0;

				if (element == null)
				{
					return null;
				}

				var parent = element.Parent as FrameworkContentElement;

				if (parent == null)
				{
					return null;
				}

				TextElement lastContent = null;

				var children = parent.TryGetChildCollection();

				for (int i = 0; children[i] != element; i++)
				{
					var child = (TextElement) children[i];

					if (!(child is Section))
					{
						lastContent = child;
					}
					else
					{
						int ignoreLeading, trailingConsecutiveEmptyDescendantsCount;
						TextElement ignoreFirst, lastContentDescendant;

						if (IsEmptySection(child, out ignoreLeading, out trailingConsecutiveEmptyDescendantsCount, out ignoreFirst, out lastContentDescendant))
						{
							consecutiveEmptySectionsCount++;
							consecutiveEmptySectionsCount += trailingConsecutiveEmptyDescendantsCount > 1 ? trailingConsecutiveEmptyDescendantsCount : 0;
						}
						else
						{
							lastContent = lastContentDescendant;
							consecutiveEmptySectionsCount = trailingConsecutiveEmptyDescendantsCount;
						}
					}
				}

				if (lastContent != null)
				{
					return lastContent;
				}

				int parentConsecutiveEmptySectionsCount;

				lastContent = GetPreviousAncestorContent(parent, out parentConsecutiveEmptySectionsCount);

				consecutiveEmptySectionsCount += parentConsecutiveEmptySectionsCount;

				return lastContent;
			}

			private static TextElement GetFollowingAncestorContent(FrameworkContentElement element, out int consecutiveEmptySectionsCount)
			{
				consecutiveEmptySectionsCount = 0;

				if (element == null)
				{
					return null;
				}

				var parent = element.Parent as FrameworkContentElement;

				if (parent == null)
				{
					return null;
				}

				TextElement firstContent = null;

				var children = parent.TryGetChildCollection();

				for (int i = children.Count - 1; children[i] != element; i--)
				{
					var child = (TextElement) children[i];

					if (!(child is Section))
					{
						firstContent = child;
					}
					else
					{
						int leadingConsecutiveEmptyDescendantsCount, ignoreTrailing;
						TextElement firstContentDescendant, ignoreLast;

						if (IsEmptySection(child, out leadingConsecutiveEmptyDescendantsCount, out ignoreTrailing, out firstContentDescendant, out ignoreLast))
						{
							consecutiveEmptySectionsCount++;
							consecutiveEmptySectionsCount += leadingConsecutiveEmptyDescendantsCount > 1 ? leadingConsecutiveEmptyDescendantsCount : 0;
						}
						else
						{
							firstContent = firstContentDescendant;
							consecutiveEmptySectionsCount = leadingConsecutiveEmptyDescendantsCount;
						}
					}
				}

				if (firstContent != null)
				{
					return firstContent;
				}

				int parentConsecutiveEmptySectionsCount;

				firstContent = GetFollowingAncestorContent(parent, out parentConsecutiveEmptySectionsCount);

				consecutiveEmptySectionsCount += parentConsecutiveEmptySectionsCount;

				return firstContent;
			}

			// An empty section is a childless section or a section that contains nothing but other empty sections.
			private static bool IsEmptySection(TextElement element, out int leadingConsecutiveEmptyDescendantsCount, out int trailingConsecutiveEmptyDescendantsCount, out TextElement firstContentDescendant, out TextElement lastContentDescendant)
			{
				int localLeadingCount = 0, localTrailingCount = 0;
				TextElement localFirstContent = null, localLastContent = null;
				var leading = true;
				var section = element as Section;
				var isEmpty = section != null;

				if (isEmpty)
				{
					foreach (var child in section.Blocks)
					{
						int childLeadingCount, childTrailingCount;
						TextElement childFirstContent, childLastContent;
						var isChildEmpty = IsEmptySection(child, out childLeadingCount, out childTrailingCount, out childFirstContent, out childLastContent);

						if (leading)
						{
							localLeadingCount += childLeadingCount;
						}

						if (isChildEmpty)
						{
							if (leading)
							{
								localLeadingCount++;
							}
							else
							{
								localTrailingCount += 1 + childTrailingCount;
							}
						}
						else
						{
							if (localFirstContent == null)
							{
								localFirstContent = childFirstContent;
							}

							localLastContent = childLastContent;

							localTrailingCount = childTrailingCount;
							leading = false;
						}

						isEmpty &= isChildEmpty;
					}
				}

				if (section == null)
				{
					localFirstContent = localLastContent = element;
				}

				// Contract.Assume(!result || localTrailingCount == 0);
				// Contract.Assume(!result || localFirstContent == null);
				// Contract.Assume(!result || localLastContent == null);
				// Contract.Assume(result || localFirstContent != null || localLastContent != null);
				// Contract.Assume(!(localFirstContent is Section));
				// Contract.Assume(!(localLastContent is Section));

				firstContentDescendant = localFirstContent;
				lastContentDescendant = localLastContent;
				leadingConsecutiveEmptyDescendantsCount = localLeadingCount;
				trailingConsecutiveEmptyDescendantsCount = isEmpty ? localLeadingCount : localTrailingCount;

				return isEmpty;
			}
		}

		// Called by internal code (see "FlowDocumentLayoutLab - Internal.cs")
		private IEnumerable<InsertionLine> GetInsertionLines(TextElement element, Rect logicalBox, Rect documentBox)
		{
			const double lineHeight = 1;

			var previous = element.GetPreviousSiblingOrAncestor();
			var following = element.GetFollowingSiblingOrAncestor();

			var previousBottomLeft = previous == null
				? documentBox.TopLeft
				: MeasurementContext.MeasureLogicalBox(previous, documentBox).BottomLeft;

			var followingTopLeft = following == null
				? documentBox.BottomLeft
				: MeasurementContext.MeasureLogicalBox(following, documentBox).TopLeft;

			var aboveBetween = new Rect(previousBottomLeft, logicalBox.TopLeft).GetCenter();
			var belowBetween = new Rect(logicalBox.BottomLeft, followingTopLeft).GetCenter();

			yield return new InsertionLine()
			{
				RelativeLocation = StructureTabLocation.Above,
				Box = new Rect(logicalBox.X, Math.Min(logicalBox.Y, aboveBetween.Y), logicalBox.Width, lineHeight)
			};

			yield return new InsertionLine()
			{
				RelativeLocation = StructureTabLocation.Centered,
				Box = new Rect(logicalBox.X, logicalBox.GetCenter().Y, logicalBox.Width, lineHeight)
			};

			yield return new InsertionLine()
			{
				RelativeLocation = StructureTabLocation.Below,
				Box = new Rect(logicalBox.X, Math.Max(logicalBox.Y, belowBetween.Y), logicalBox.Width, lineHeight)
			};
		}

		/* The following experiments are automatically detected at runtime.
		 * For details, see the constructor in the partial class.
		 * 
		 * Semantic naming convention for the following experiments:
		 * 
		 *  Nested: Applies to all subsequent elements until a higher level modifier is used (above the level at which the term "Nested" was used.)
		 *	Then: Concatenates siblings on the first level.
		 *	And: Concatenates siblings on the second level.
		 *	With: Concatenates siblings on the third level.
		 */

		private Task MeasureChildlessDocument()
		{
			return Measure(new FlowDocument());
		}

		#region Basic Structure
		#region Single Childless Section
		private Task MeasureSection()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Section());

			return Measure(document);
		}

		private Task MeasureParagraphThenSection()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Paragraph());
			document.Blocks.Add(new Section());

			return Measure(document);
		}

		private Task MeasureSectionThenParagraph()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Section());
			document.Blocks.Add(new Paragraph());

			return Measure(document);
		}

		private Task MeasureParagraphThenSectionThenParagraph()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Paragraph());
			document.Blocks.Add(new Section());
			document.Blocks.Add(new Paragraph());

			return Measure(document);
		}
		#endregion

		#region Two Childless Sections
		private Task MeasureSectionThenSection()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Section());
			document.Blocks.Add(new Section());

			return Measure(document);
		}

		private Task MeasureParagraphThenSectionThenSection()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Paragraph());
			document.Blocks.Add(new Section());
			document.Blocks.Add(new Section());

			return Measure(document);
		}

		private Task MeasureSectionThenSectionThenParagraph()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Section());
			document.Blocks.Add(new Section());
			document.Blocks.Add(new Paragraph());

			return Measure(document);
		}

		private Task MeasureParagraphThenSectionThenSectionThenParagraph()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Paragraph());
			document.Blocks.Add(new Section());
			document.Blocks.Add(new Section());
			document.Blocks.Add(new Paragraph());

			return Measure(document);
		}
		#endregion

		#region Single Nested Section
		private Task MeasureNestedSection()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Section(new Section()));

			return Measure(document);
		}

		private Task MeasureParagraphThenNestedSection()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Paragraph());
			document.Blocks.Add(new Section(new Section()));

			return Measure(document);
		}

		private Task MeasureNestedSectionThenParagraph()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Section(new Section()));
			document.Blocks.Add(new Paragraph());

			return Measure(document);
		}

		private Task MeasureParagraphThenNestedSectionThenParagraph()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Paragraph());
			document.Blocks.Add(new Section(new Section()));
			document.Blocks.Add(new Paragraph());

			return Measure(document);
		}
		#endregion

		#region Two Sections; One Childless and One Nested
		private Task MeasureNestedSectionThenSection()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Section(new Section()));
			document.Blocks.Add(new Section());

			return Measure(document);
		}

		private Task MeasureSectionThenNestedSection()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Section());
			document.Blocks.Add(new Section(new Section()));

			return Measure(document);
		}

		private Task MeasureParagraphThenNestedSectionThenSection()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Paragraph());
			document.Blocks.Add(new Section(new Section()));
			document.Blocks.Add(new Section());

			return Measure(document);
		}

		private Task MeasureParagraphThenSectionThenNestedSection()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Paragraph());
			document.Blocks.Add(new Section());
			document.Blocks.Add(new Section(new Section()));

			return Measure(document);
		}

		private Task MeasureNestedSectionThenSectionThenParagraph()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Section(new Section()));
			document.Blocks.Add(new Section());
			document.Blocks.Add(new Paragraph());

			return Measure(document);
		}

		private Task MeasureSectionThenNestedSectionThenParagraph()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Section());
			document.Blocks.Add(new Section(new Section()));
			document.Blocks.Add(new Paragraph());

			return Measure(document);
		}

		private Task MeasureParagraphThenNestedSectionThenSectionThenParagraph()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Paragraph());
			document.Blocks.Add(new Section(new Section()));
			document.Blocks.Add(new Section());
			document.Blocks.Add(new Paragraph());

			return Measure(document);
		}

		private Task MeasureParagraphThenSectionThenNestedSectionThenParagraph()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Paragraph());
			document.Blocks.Add(new Section());
			document.Blocks.Add(new Section(new Section()));
			document.Blocks.Add(new Paragraph());

			return Measure(document);
		}
		#endregion

		#region Two Nested Sections
		private Task MeasureNestedSectionThenNestedSection()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Section(new Section()));
			document.Blocks.Add(new Section(new Section()));

			return Measure(document);
		}

		private Task MeasureParagraphThenNestedSectionThenNestedSection()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Paragraph());
			document.Blocks.Add(new Section(new Section()));
			document.Blocks.Add(new Section(new Section()));

			return Measure(document);
		}

		private Task MeasureNestedSectionThenNestedSectionThenParagraph()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Section(new Section()));
			document.Blocks.Add(new Section(new Section()));
			document.Blocks.Add(new Paragraph());

			return Measure(document);
		}

		private Task MeasureParagraphThenNestedSectionThenNestedSectionThenParagraph()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Paragraph());
			document.Blocks.Add(new Section(new Section()));
			document.Blocks.Add(new Section(new Section()));
			document.Blocks.Add(new Paragraph());

			return Measure(document);
		}
		#endregion
		#endregion

		const string firstExperimentWithFullNesting = "MeasureNestedParagraphAndSection";

		#region Full Nesting
		#region Single Childless Section
		private Task MeasureNestedParagraphAndSection()
		{
			var section = new Section();

			section.Blocks.Add(new Paragraph());
			section.Blocks.Add(new Section());

			return Measure(new FlowDocument(section));
		}

		private Task MeasureNestedSectionAndParagraph()
		{
			var section = new Section();

			section.Blocks.Add(new Section());
			section.Blocks.Add(new Paragraph());

			return Measure(new FlowDocument(section));
		}

		private Task MeasureNestedParagraphAndSectionAndParagraph()
		{
			var section = new Section();

			section.Blocks.Add(new Paragraph());
			section.Blocks.Add(new Section());
			section.Blocks.Add(new Paragraph());

			return Measure(new FlowDocument(section));
		}
		#endregion

		#region Two Childless Sections
		private Task MeasureNestedSectionAndSection()
		{
			var section = new Section();

			section.Blocks.Add(new Section());
			section.Blocks.Add(new Section());

			return Measure(new FlowDocument(section));
		}

		private Task MeasureNestedParagraphAndSectionAndSection()
		{
			var section = new Section();

			section.Blocks.Add(new Paragraph());
			section.Blocks.Add(new Section());
			section.Blocks.Add(new Section());

			return Measure(new FlowDocument(section));
		}

		private Task MeasureNestedSectionAndSectionAndParagraph()
		{
			var section = new Section();

			section.Blocks.Add(new Section());
			section.Blocks.Add(new Section());
			section.Blocks.Add(new Paragraph());

			return Measure(new FlowDocument(section));
		}

		private Task MeasureNestedParagraphAndSectionAndSectionAndParagraph()
		{
			var section = new Section();

			section.Blocks.Add(new Paragraph());
			section.Blocks.Add(new Section());
			section.Blocks.Add(new Section());
			section.Blocks.Add(new Paragraph());

			return Measure(new FlowDocument(section));
		}
		#endregion

		#region Single Nested Section
		private Task MeasureNestedNestedSection()
		{
			var section = new Section();

			section.Blocks.Add(new Section(new Section()));

			return Measure(new FlowDocument(section));
		}

		private Task MeasureNestedParagraphAndNestedSection()
		{
			var section = new Section();

			section.Blocks.Add(new Paragraph());
			section.Blocks.Add(new Section(new Section()));

			return Measure(new FlowDocument(section));
		}

		private Task MeasureNestedNestedSectionAndParagraph()
		{
			var section = new Section();

			section.Blocks.Add(new Section(new Section()));
			section.Blocks.Add(new Paragraph());

			return Measure(new FlowDocument(section));
		}

		private Task MeasureNestedParagraphAndNestedSectionAndParagraph()
		{
			var section = new Section();

			section.Blocks.Add(new Paragraph());
			section.Blocks.Add(new Section(new Section()));
			section.Blocks.Add(new Paragraph());

			return Measure(new FlowDocument(section));
		}
		#endregion

		#region Two Sections; One Childless and One Nested
		private Task MeasureNestedNestedSectionAndSection()
		{
			var section = new Section();

			section.Blocks.Add(new Section(new Section()));
			section.Blocks.Add(new Section());

			return Measure(new FlowDocument(section));
		}

		private Task MeasureNestedSectionAndNestedSection()
		{
			var section = new Section();

			section.Blocks.Add(new Section());
			section.Blocks.Add(new Section(new Section()));

			return Measure(new FlowDocument(section));
		}

		private Task MeasureNestedParagraphAndNestedSectionAndSection()
		{
			var section = new Section();

			section.Blocks.Add(new Paragraph());
			section.Blocks.Add(new Section(new Section()));
			section.Blocks.Add(new Section());

			return Measure(new FlowDocument(section));
		}

		private Task MeasureNestedParagraphAndSectionAndNestedSection()
		{
			var section = new Section();

			section.Blocks.Add(new Paragraph());
			section.Blocks.Add(new Section());
			section.Blocks.Add(new Section(new Section()));

			return Measure(new FlowDocument(section));
		}

		private Task MeasureNestedNestedSectionAndSectionAndParagraph()
		{
			var section = new Section();

			section.Blocks.Add(new Section(new Section()));
			section.Blocks.Add(new Section());
			section.Blocks.Add(new Paragraph());

			return Measure(new FlowDocument(section));
		}

		private Task MeasureNestedSectionAndNestedSectionAndParagraph()
		{
			var section = new Section();

			section.Blocks.Add(new Section());
			section.Blocks.Add(new Section(new Section()));
			section.Blocks.Add(new Paragraph());

			return Measure(new FlowDocument(section));
		}

		private Task MeasureNestedParagraphAndNestedSectionAndSectionAndParagraph()
		{
			var section = new Section();

			section.Blocks.Add(new Paragraph());
			section.Blocks.Add(new Section(new Section()));
			section.Blocks.Add(new Section());
			section.Blocks.Add(new Paragraph());

			return Measure(new FlowDocument(section));
		}

		private Task MeasureNestedParagraphAndSectionAndNestedSectionAndParagraph()
		{
			var section = new Section();

			section.Blocks.Add(new Paragraph());
			section.Blocks.Add(new Section());
			section.Blocks.Add(new Section(new Section()));
			section.Blocks.Add(new Paragraph());

			return Measure(new FlowDocument(section));
		}
		#endregion

		#region Two Nested Sections
		private Task MeasureNestedNestedSectionAndNestedSection()
		{
			var section = new Section();

			section.Blocks.Add(new Section(new Section()));
			section.Blocks.Add(new Section(new Section()));

			return Measure(new FlowDocument(section));
		}

		private Task MeasureNestedParagraphAndNestedSectionAndNestedSection()
		{
			var section = new Section();

			section.Blocks.Add(new Paragraph());
			section.Blocks.Add(new Section(new Section()));
			section.Blocks.Add(new Section(new Section()));

			return Measure(new FlowDocument(section));
		}

		private Task MeasureNestedNestedSectionAndNestedSectionAndParagraph()
		{
			var section = new Section();

			section.Blocks.Add(new Section(new Section()));
			section.Blocks.Add(new Section(new Section()));
			section.Blocks.Add(new Paragraph());

			return Measure(new FlowDocument(section));
		}

		private Task MeasureNestedParagraphAndNestedSectionAndNestedSectionAndParagraph()
		{
			var section = new Section();

			section.Blocks.Add(new Paragraph());
			section.Blocks.Add(new Section(new Section()));
			section.Blocks.Add(new Section(new Section()));
			section.Blocks.Add(new Paragraph());

			return Measure(new FlowDocument(section));
		}
		#endregion
		#endregion

		const string firstExperimentWithJaggedNesting = "MeasureNestedParagraphThenSection";

		#region Jagged Nesting
		#region Single Childless Section
		private Task MeasureNestedParagraphThenSection()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Section(new Paragraph()));
			document.Blocks.Add(new Section());

			return Measure(document);
		}

		private Task MeasureSectionThenNestedParagraph()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Section());
			document.Blocks.Add(new Section(new Paragraph()));

			return Measure(document);
		}

		private Task MeasureNestedParagraphAndSectionThenParagraph()
		{
			var document = new FlowDocument();
			var section = new Section();

			section.Blocks.Add(new Paragraph());
			section.Blocks.Add(new Section());

			document.Blocks.Add(section);
			document.Blocks.Add(new Paragraph());

			return Measure(document);
		}

		private Task MeasureParagraphThenNestedSectionAndParagraph()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Paragraph());

			var section = new Section();

			section.Blocks.Add(new Section());
			section.Blocks.Add(new Paragraph());

			document.Blocks.Add(section);

			return Measure(document);
		}
		#endregion

		#region Two Childless Sections
		private Task MeasureNestedParagraphThenSectionThenSection()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Section(new Paragraph()));
			document.Blocks.Add(new Section());
			document.Blocks.Add(new Section());

			return Measure(document);
		}

		private Task MeasureSectionThenSectionThenNestedParagraph()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Section());
			document.Blocks.Add(new Section());
			document.Blocks.Add(new Section(new Paragraph()));

			return Measure(document);
		}

		private Task MeasureNestedParagraphAndSectionThenSection()
		{
			var document = new FlowDocument();
			var section = new Section();

			section.Blocks.Add(new Paragraph());
			section.Blocks.Add(new Section());

			document.Blocks.Add(section);
			document.Blocks.Add(new Section());

			return Measure(document);
		}

		private Task MeasureSectionThenNestedSectionAndParagraph()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Section());

			var section = new Section();

			section.Blocks.Add(new Section());
			section.Blocks.Add(new Paragraph());

			document.Blocks.Add(section);

			return Measure(document);
		}

		private Task MeasureNestedParagraphAndSectionThenSectionThenParagraph()
		{
			var document = new FlowDocument();
			var section = new Section();

			section.Blocks.Add(new Paragraph());
			section.Blocks.Add(new Section());

			document.Blocks.Add(section);
			document.Blocks.Add(new Section());
			document.Blocks.Add(new Paragraph());

			return Measure(document);
		}

		private Task MeasureParagraphThenSectionThenNestedSectionAndParagraph()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Paragraph());
			document.Blocks.Add(new Section());

			var section = new Section();

			section.Blocks.Add(new Section());
			section.Blocks.Add(new Paragraph());

			document.Blocks.Add(section);

			return Measure(document);
		}

		private Task MeasureNestedParagraphThenSectionThenSectionThenParagraph()
		{
			var document = new FlowDocument();
			var section = new Section();

			section.Blocks.Add(new Paragraph());

			document.Blocks.Add(section);
			document.Blocks.Add(new Section());
			document.Blocks.Add(new Section());
			document.Blocks.Add(new Paragraph());

			return Measure(document);
		}

		private Task MeasureParagraphThenSectionThenSectionThenNestedParagraph()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Paragraph());
			document.Blocks.Add(new Section());
			document.Blocks.Add(new Section());

			var section = new Section();

			section.Blocks.Add(new Paragraph());

			document.Blocks.Add(section);

			return Measure(document);
		}

		private Task MeasureParagraphThenNestedSectionAndSectionThenParagraph()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Paragraph());

			var section = new Section();

			section.Blocks.Add(new Section());
			section.Blocks.Add(new Section());

			document.Blocks.Add(section);
			document.Blocks.Add(new Paragraph());

			return Measure(document);
		}
		#endregion

		#region Single Nested Section
		private Task MeasureNestedParagraphThenNestedSection()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Section(new Paragraph()));
			document.Blocks.Add(new Section(new Section()));

			return Measure(document);
		}

		private Task MeasureNestedSectionThenNestedParagraph()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Section(new Section()));
			document.Blocks.Add(new Section(new Paragraph()));

			return Measure(document);
		}

		private Task MeasureNestedNestedParagraphThenNestedSection()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Section(new Section(new Paragraph())));
			document.Blocks.Add(new Section(new Section()));

			return Measure(document);
		}

		private Task MeasureNestedSectionThenNestedNestedParagraph()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Section(new Section()));
			document.Blocks.Add(new Section(new Section(new Paragraph())));

			return Measure(document);
		}

		private Task MeasureNestedParagraphThenNestedSectionThenParagraph()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Section(new Paragraph()));
			document.Blocks.Add(new Section(new Section()));
			document.Blocks.Add(new Paragraph());

			return Measure(document);
		}

		private Task MeasureParagraphThenNestedSectionThenNestedParagraph()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Paragraph());
			document.Blocks.Add(new Section(new Section()));
			document.Blocks.Add(new Section(new Paragraph()));

			return Measure(document);
		}
		#endregion

		#region Two Sections; One Childless and One Nested
		private Task MeasureNestedParagraphThenNestedSectionThenSection()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Section(new Paragraph()));
			document.Blocks.Add(new Section(new Section()));
			document.Blocks.Add(new Section());

			return Measure(document);
		}

		private Task MeasureNestedParagraphThenSectionThenNestedSection()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Section(new Paragraph()));
			document.Blocks.Add(new Section());
			document.Blocks.Add(new Section(new Section()));

			return Measure(document);
		}

		private Task MeasureNestedSectionThenSectionThenNestedParagraph()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Section(new Section()));
			document.Blocks.Add(new Section());
			document.Blocks.Add(new Section(new Paragraph()));

			return Measure(document);
		}

		private Task MeasureSectionThenNestedSectionThenNestedParagraph()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Section());
			document.Blocks.Add(new Section(new Section()));
			document.Blocks.Add(new Section(new Paragraph()));

			return Measure(document);
		}

		private Task MeasureNestedParagraphThenNestedSectionThenSectionThenParagraph()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Section(new Paragraph()));
			document.Blocks.Add(new Section(new Section()));
			document.Blocks.Add(new Section());
			document.Blocks.Add(new Paragraph());

			return Measure(document);
		}

		private Task MeasureNestedParagraphThenSectionThenNestedSectionThenParagraph()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Section(new Paragraph()));
			document.Blocks.Add(new Section());
			document.Blocks.Add(new Section(new Section()));
			document.Blocks.Add(new Paragraph());

			return Measure(document);
		}

		private Task MeasureParagraphThenNestedSectionThenSectionThenNestedParagraph()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Paragraph());
			document.Blocks.Add(new Section(new Section()));
			document.Blocks.Add(new Section());
			document.Blocks.Add(new Section(new Paragraph()));

			return Measure(document);
		}

		private Task MeasureParagraphThenSectionThenNestedSectionThenNestedParagraph()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Paragraph());
			document.Blocks.Add(new Section());
			document.Blocks.Add(new Section(new Section()));
			document.Blocks.Add(new Section(new Paragraph()));

			return Measure(document);
		}
		#endregion

		#region Two Nested Sections
		private Task MeasureNestedParagraphThenNestedSectionThenNestedSection()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Section(new Paragraph()));
			document.Blocks.Add(new Section(new Section()));
			document.Blocks.Add(new Section(new Section()));

			return Measure(document);
		}

		private Task MeasureNestedSectionThenNestedSectionThenNestedParagraph()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Section(new Section()));
			document.Blocks.Add(new Section(new Section()));
			document.Blocks.Add(new Section(new Paragraph()));

			return Measure(document);
		}

		private Task MeasureNestedParagraphThenNestedSectionThenNestedSectionThenNestedParagraph()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Section(new Paragraph()));
			document.Blocks.Add(new Section(new Section()));
			document.Blocks.Add(new Section(new Section()));
			document.Blocks.Add(new Section(new Paragraph()));

			return Measure(document);
		}
		#endregion

		#region Complex Nesting
		private Task MeasureParagraphThenNestedSectionThenNestedSectionAndParagraph()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Paragraph());
			document.Blocks.Add(new Section(new Section()));

			var section = new Section();

			section.Blocks.Add(new Section());
			section.Blocks.Add(new Paragraph());

			document.Blocks.Add(section);

			return Measure(document);
		}
		#endregion
		#endregion

		const string firstExperimentWithConsecutiveParagraphs = "MeasureParagraphThenParagraph";

		#region Consecutive Paragraphs
		private Task MeasureParagraphThenParagraph()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Paragraph());
			document.Blocks.Add(new Paragraph());

			return Measure(document);
		}

		private Task MeasureParagraphTextThenParagraphText()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Paragraph(new Run(paragraph1)));
			document.Blocks.Add(new Paragraph(new Run(paragraph2)));

			return Measure(document);
		}

		private Task MeasureNestedParagraphAndParagraph()
		{
			var section = new Section();

			section.Blocks.Add(new Paragraph());
			section.Blocks.Add(new Paragraph());

			return Measure(new FlowDocument(section));
		}

		private Task MeasureNestedParagraphTextAndParagraphText()
		{
			var section = new Section();

			section.Blocks.Add(new Paragraph(new Run(paragraph1)));
			section.Blocks.Add(new Paragraph(new Run(paragraph2)));

			return Measure(new FlowDocument(section));
		}

		private Task MeasureParagraphThenNestedParagraph()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Paragraph());
			document.Blocks.Add(new Section(new Paragraph()));

			return Measure(document);
		}

		private Task MeasureNestedParagraphThenParagraph()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Section(new Paragraph()));
			document.Blocks.Add(new Paragraph());

			return Measure(document);
		}

		private Task MeasureSectionThenParagraphThenNestedParagraph()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Section());
			document.Blocks.Add(new Paragraph());
			document.Blocks.Add(new Section(new Paragraph()));

			return Measure(document);
		}

		private Task MeasureParagraphThenNestedParagraphThenSection()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Paragraph());
			document.Blocks.Add(new Section(new Paragraph()));
			document.Blocks.Add(new Section());

			return Measure(document);
		}

		private Task MeasureSectionThenNestedParagraphAndNestedParagraph()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Section());

			var section = new Section();

			section.Blocks.Add(new Paragraph());
			section.Blocks.Add(new Section(new Paragraph()));

			document.Blocks.Add(section);

			return Measure(document);
		}

		private Task MeasureNestedParagraphAndNestedParagraphThenSection()
		{
			var document = new FlowDocument();
			var section = new Section();

			section.Blocks.Add(new Paragraph());
			section.Blocks.Add(new Section(new Paragraph()));

			document.Blocks.Add(section);
			document.Blocks.Add(new Section());

			return Measure(document);
		}
		#endregion

		#region BlockUIContainer
		private Task MeasureBlockUIContainer()
		{
			var text = new TextBox() { Text = paragraph1, IsReadOnly = true };

			return Measure(new FlowDocument(new BlockUIContainer(text)));
		}

		private Task MeasureNestedBlockUIContainer()
		{
			var text = new TextBox() { Text = paragraph1, IsReadOnly = true };

			return Measure(new FlowDocument(new Section(new BlockUIContainer(text))));
		}

		private Task MeasureNestedBlockUIContainerAndEmptySection()
		{
			var section = new Section();

			var text = new TextBox() { Text = paragraph1, IsReadOnly = true };

			section.Blocks.Add(new BlockUIContainer(text));
			section.Blocks.Add(new Section());

			return Measure(new FlowDocument(section));
		}

		private Task MeasureNestedBlockUIContainerAndEmptySectionFollowedByEmptySection()
		{
			var section = new Section();

			var text = new TextBox() { Text = paragraph1, IsReadOnly = true };

			section.Blocks.Add(new BlockUIContainer(text));
			section.Blocks.Add(new Section());

			var document = new FlowDocument();

			document.Blocks.Add(section);
			document.Blocks.Add(new Section());

			return Measure(document);
		}

		private Task MeasureNestedBlockUIContainerAndEmptySectionFollowedByParagraph()
		{
			var section = new Section();

			var text = new TextBox() { Text = paragraph1, IsReadOnly = true };

			section.Blocks.Add(new BlockUIContainer(text));
			section.Blocks.Add(new Section());

			var document = new FlowDocument();

			document.Blocks.Add(section);
			document.Blocks.Add(new Paragraph());

			return Measure(document);
		}

		private Task MeasureEmptySectionFollowedByNestedBlockUIContainerAndEmptySection()
		{
			var section = new Section();

			var text = new TextBox() { Text = paragraph1, IsReadOnly = true };

			section.Blocks.Add(new BlockUIContainer(text));
			section.Blocks.Add(new Section());

			var document = new FlowDocument();

			document.Blocks.Add(new Section());
			document.Blocks.Add(section);

			return Measure(document);
		}

		private Task MeasureEmptySectionsSurroundingNestedBlockUIContainerAndEmptySection()
		{
			var section = new Section();
			var text = new TextBox() { Text = paragraph1, IsReadOnly = true };

			section.Blocks.Add(new BlockUIContainer(text));
			section.Blocks.Add(new Section());

			var document = new FlowDocument();

			document.Blocks.Add(new Section());
			document.Blocks.Add(section);
			document.Blocks.Add(new Section());

			return Measure(document);
		}
		#endregion
	}
}