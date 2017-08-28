using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using DaveSexton.XmlGel.Documents;
using DaveSexton.XmlGel.Extensions;
using DaveSexton.XmlGel.Xml;

namespace DaveSexton.XmlGel.Maml.Documents.Adorners
{
	internal sealed class StructureTabRowBuilder
	{
		public IEnumerable<StructureTabRow> TabRows
		{
			get
			{
				if (insertPreviousSiblingRow != null)
				{
					yield return insertPreviousSiblingRow;
				}

				if (insertChildRow != null)
				{
					yield return insertChildRow;
				}

				if (insertFollowingSiblingRow != null)
				{
					yield return insertFollowingSiblingRow;
				}
			}
		}

		public StructureTabRowBuilder Child
		{
			get
			{
				return childBuilder;
			}
		}

		public bool ShouldCreateTabRow
		{
			get
			{
				return shouldCreateTabRow;
			}
		}

		public bool ShouldCreateParentTabRow
		{
			get
			{
				return target == null
						|| container.Children.Count == 0
						|| target.Element == container.Children[0].Element
						|| target.Element == container.Children.Last().Element;
			}
		}

		public MamlPart TargetPart
		{
			get
			{
				return target;
			}
		}

		public MamlPart ContainerPart
		{
			get
			{
				return container;
			}
		}

		public Rect LogicalBox
		{
			get
			{
				return logicalBox;
			}
		}

		private readonly StructurePalette palette;
		private readonly Rect logicalBox;
		private readonly MamlPart target;
		private readonly MamlPart container;
		private readonly bool isOriginalTargetEmpty;
		private readonly bool shouldCreateTabRow;
		private readonly StructureTabRowBuilder childBuilder;
		private StructureTabRowBuilder parentBuilder;
		private StructureTabRow insertPreviousSiblingRow, insertChildRow, insertFollowingSiblingRow;

		private StructureTabRowBuilder(StructurePalette palette, MamlPart target, MamlPart container, Point mouse, StructureTabRowBuilder childBuilder, StructureTabRowBuilder currentActiveBuilder)
		{
			Contract.Requires(palette != null);
			Contract.Requires(container != null);
			Contract.Requires(childBuilder == null || target != null);
			Contract.Requires(childBuilder == null || childBuilder.container == target);

			this.palette = palette;
			this.target = target;
			this.container = container;
			this.childBuilder = childBuilder;

			if (childBuilder != null)
			{
				childBuilder.parentBuilder = this;
			}

			logicalBox = (target ?? container).LogicalBox;

			if (childBuilder == null)
			{
				isOriginalTargetEmpty = target == null;

				var extendedLogicalBox = isOriginalTargetEmpty
					? container.ChildInsertionLine
					: Rect.Union(target.PreviousSiblingInsertionLine, target.FollowingSiblingInsertionLine);

				if (currentActiveBuilder != null)
				{
					// TODO: Union all ancestor builders into extendedLogicalBox.  That will require calculating the linked list of ancestors
					// before calling this constructor.  Currently, parentBuilder is null until after this constructor exists and its parent
					// builder is being created (see assignment to parentBuilder above).

					if (currentActiveBuilder.insertPreviousSiblingRow != null)
					{
						extendedLogicalBox.Union(currentActiveBuilder.insertPreviousSiblingRow.SchemaTabsTray);
						extendedLogicalBox.Union(currentActiveBuilder.insertPreviousSiblingRow.BehaviorTabsTray);
					}

					if (currentActiveBuilder.insertFollowingSiblingRow != null)
					{
						extendedLogicalBox.Union(currentActiveBuilder.insertFollowingSiblingRow.SchemaTabsTray);
					}
				}

				if (!Rect.Union(logicalBox, extendedLogicalBox).Contains(mouse))
				{
					shouldCreateTabRow = false;
					return;
				}
			}
			else
			{
				isOriginalTargetEmpty = childBuilder.isOriginalTargetEmpty;
			}

			container.EnsureValidatableData(target);

			shouldCreateTabRow = true;
		}

		public static StructureTabRowBuilder Create(StructurePalette palette, MamlPart target, Point mouse, StructureTabRowBuilder childBuilder, StructureTabRowBuilder currentActiveBuilder)
		{
			Contract.Requires(palette != null);
			Contract.Requires(target != null);

			MamlPart container;

			if (target.HasContainer)
			{
				container = target.Container;
			}
			else
			{
				container = target;
				target = null;
			}

			if (childBuilder == null)
			{
				while (container.ElementOrDocument is Inline || container.Node is MamlInlineContainer)
				{
					target = container;
					container = target.Container;
				}
			}

			var section = container.ElementOrDocument as HeaderedSection;

			if (section != null && section.Header.EqualsOrContainsDescendant(target.Element))
			{
				target = container;
				container = target.Container;
			}

			if (childBuilder == null && target != null && target.IsStructure && !target.HasChildren)
			{
				container = target;
				target = null;
			}

			return new StructureTabRowBuilder(palette, target, container, mouse, childBuilder, currentActiveBuilder);
		}

		public IEnumerable<StructureTabRowBuilder> GetSelfAndAncestors()
		{
			var builder = this;

			do
			{
				yield return builder;

				builder = builder.parentBuilder;
			}
			while (builder != null);
		}

		public void CreateTabRows(XmlSchemaParticlesExpected expectedBeforeTarget, XmlSchemaParticlesExpected expectedAfterTarget)
		{
			Contract.Requires(expectedBeforeTarget != null);
			Contract.Requires(expectedAfterTarget != null);

			if (isOriginalTargetEmpty && childBuilder == null)
			{
				insertChildRow = new StructureTabRow(
					palette,
					expectedBeforeTarget,
					container,
					StructureTabLocation.Centered,
					GetInsertionLineRelativeToParent(container.ChildInsertionLine),
					addBehaviorTabs: false);
			}
			else
			{
				Contract.Assume(target != null);

				var isFirst = container.Children[0] == target;

				insertPreviousSiblingRow = new StructureTabRow(
					palette,
					expectedBeforeTarget,
					container,
					isFirst ? StructureTabLocation.Above : StructureTabLocation.Centered,
					GetInsertionLineRelativeToParent(target.PreviousSiblingInsertionLine, above: isFirst ? true : (bool?) null),
					addBehaviorTabs: true);

				if (expectedAfterTarget.Count > 0)
				{
					var isLast = container.Children[container.Children.Count - 1] == target;

					insertFollowingSiblingRow = new StructureTabRow(
						palette,
						expectedAfterTarget,
						container,
						isLast ? StructureTabLocation.Below : StructureTabLocation.Centered,
						GetInsertionLineRelativeToParent(target.FollowingSiblingInsertionLine, above: isLast ? false : (bool?) null),
						addBehaviorTabs: false);
				}
			}
		}

		private Rect GetInsertionLineRelativeToParent(Rect baseLine, bool? above = null)
		{
			if (parentBuilder != null)
			{
				var estimatedTabHeight = palette.TabCaptionFontSize + palette.TabCaptionMargin.Top + palette.TabCaptionMargin.Bottom;

				double leftOffset = 0d;

				if (above.HasValue)
				{
					estimatedTabHeight += above.Value
						? palette.TabMargin.Top + palette.TabPadding.Top
						: palette.TabMargin.Bottom + palette.TabPadding.Bottom;

					var parentRow = above.Value
						? parentBuilder.insertPreviousSiblingRow
						: parentBuilder.insertFollowingSiblingRow ?? parentBuilder.insertPreviousSiblingRow;

					var estimatedBox = new Rect(
						baseLine.X,
						baseLine.Y + (above.Value ? -estimatedTabHeight : 0),
						baseLine.Width,
						estimatedTabHeight);

					if (estimatedBox.IntersectsWith(parentRow.SchemaTabsTray))
					{
						leftOffset = parentRow.SchemaTabsTray.Right;
					}
				}
				else
				{
					estimatedTabHeight += palette.TabMargin.Top + palette.TabPadding.Top + palette.TabMargin.Bottom + palette.TabPadding.Bottom;

					var estimatedBox = new Rect(
						baseLine.X,
						baseLine.Y - estimatedTabHeight / 2,
						baseLine.Width,
						estimatedTabHeight);

					if (parentBuilder.insertPreviousSiblingRow != null
						&& estimatedBox.IntersectsWith(parentBuilder.insertPreviousSiblingRow.SchemaTabsTray))
					{
						leftOffset = parentBuilder.insertPreviousSiblingRow.SchemaTabsTray.Right;
					}

					if (parentBuilder.insertFollowingSiblingRow != null
						&& estimatedBox.IntersectsWith(parentBuilder.insertFollowingSiblingRow.SchemaTabsTray))
					{
						leftOffset = Math.Max(leftOffset, parentBuilder.insertFollowingSiblingRow.SchemaTabsTray.Right);
					}
				}

				if (leftOffset > 0)
				{
					baseLine.X = leftOffset;
					baseLine.Width -= leftOffset;
				}
			}

			var leftMargin = palette.TabRowsFollowMouseActivationRegionDockLeft
				? palette.TabRowsFollowMouseActivationRegionMargin + palette.TabRowsFollowMouseActivationRegionWidth
				: 0;

			leftMargin += palette.TabMargin.Left + palette.TabMargin.Right;

			var offset = leftMargin - baseLine.X;

			if (offset > 0)
			{
				baseLine.Offset(offset, 0);
			}

			return baseLine;
		}

		public override int GetHashCode()
		{
			return shouldCreateTabRow.GetHashCode()
				^ (target == null ? 0 : target.GetHashCode())
				^ (container == null ? 0 : container.GetHashCode())
				^ logicalBox.GetHashCode()
				^ isOriginalTargetEmpty.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as StructureTabRowBuilder);
		}

		public bool Equals(StructureTabRowBuilder other)
		{
			return other != null
				&& shouldCreateTabRow == other.shouldCreateTabRow
				&& target == other.target
				&& container == other.container
				&& logicalBox == other.logicalBox
				&& isOriginalTargetEmpty == other.isOriginalTargetEmpty;
		}
	}
}