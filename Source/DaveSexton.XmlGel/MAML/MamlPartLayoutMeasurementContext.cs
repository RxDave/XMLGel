using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using DaveSexton.XmlGel.Extensions;

namespace DaveSexton.XmlGel.Maml
{
	// Based on the MeasurementContext struct defined in FlowDocumentLayoutLab
	internal struct MamlPartLayoutMeasurementContext
	{
		private readonly TextElement element, parent, firstContentDescendant, lastContentDescendant;
		private readonly Rect documentBox, union;
		private readonly IList<TextElement> siblings;
		private readonly int currentSiblingIndex, leadingConsecutiveEmptyDescendantsCount, trailingConsecutiveEmptyDescendantsCount;
		private readonly bool isFirstChild, isLastChild, isEmptySection;
		private Rect box, parentBox;
		private bool isBoxCalculated;

		private MamlPartLayoutMeasurementContext(TextElement element, Rect documentBox)
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
			var context = new MamlPartLayoutMeasurementContext(element, documentBox);

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

			Contract.Assume(isBoxCalculated == !parentBox.IsEmpty);

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

			Contract.Assume(!box.IsEmpty);

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

			Contract.Assume(!isEmpty || localTrailingCount == 0);
			Contract.Assume(!isEmpty || localFirstContent == null);
			Contract.Assume(!isEmpty || localLastContent == null);
			Contract.Assume(isEmpty || localFirstContent != null || localLastContent != null);
			Contract.Assume(!(localFirstContent is Section));
			Contract.Assume(!(localLastContent is Section));

			firstContentDescendant = localFirstContent;
			lastContentDescendant = localLastContent;
			leadingConsecutiveEmptyDescendantsCount = localLeadingCount;
			trailingConsecutiveEmptyDescendantsCount = isEmpty ? localLeadingCount : localTrailingCount;

			return isEmpty;
		}
	}
}