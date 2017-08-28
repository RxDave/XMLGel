using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using DaveSexton.XmlGel.Extensions;
using DaveSexton.XmlGel.Xml;

namespace DaveSexton.XmlGel.Maml.Documents.Adorners
{
	internal sealed partial class StructurePalette : EditorAdornerBase
	{
		public bool IsDebugEnabled
		{
			get
			{
				return isDebugEnabled;
			}
			set
			{
				if (isDebugEnabled != value)
				{
					isDebugEnabled = value;

					debugAdorner.CurrentTabRowBuilder = isDebugEnabled ? currentTabRowBuilder : null;
				}
			}
		}

		public bool TabRowsFollowMouse
		{
			get
			{
				return tabRowsFollowMouse;
			}
			set
			{
				if (tabRowsFollowMouse != value)
				{
					SetTabRowsFollowMouseWithoutUpdate(value);

					Update();
				}
			}
		}

		protected override bool CanRender
		{
			get
			{
				return true;
			}
		}

		internal MamlDocument Document
		{
			get
			{
				return document;
			}
		}

		private readonly Popup toolTip = new Popup();
		private readonly Run toolTipContent = new Run();
		private readonly MouseAdorner debugAdorner;
		private readonly GhostLabelsAdorner ghostLabelsAdorner;
		private readonly MamlDocument document;
		private readonly Dictionary<TextElement, Rect> blockRegions = new Dictionary<TextElement, Rect>();
		private DispatcherTimer toolTipOpenTimer;
		private StructureTabRowBuilder currentTabRowBuilder;
		private StructureTab tabUnderMouse;
		private Rect tabRowsFollowMouseActivationRegion;
		private bool isDebugEnabled, executingTabFunction, blockRegionsDirty = true;
		private bool tabRowsFollowMouse, isMouseOverTabRowsFollowMouseActivationRegion;

		static StructurePalette()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(StructurePalette), new FrameworkPropertyMetadata(typeof(StructurePalette)));
		}

		public StructurePalette(MamlDocument document, MamlTopicEditorTextBox editor, MouseAdorner debugAdorner, GhostLabelsAdorner ghostLabelsAdorner)
			: base(document, editor)
		{
#if DEBUG
			TabRowsFollowMouseActivationRegionDockLeft = true;
#endif

			this.document = document;
			this.debugAdorner = debugAdorner;
			this.ghostLabelsAdorner = ghostLabelsAdorner;

			toolTip.StaysOpen = true;
			toolTip.Placement = PlacementMode.Top;
			toolTip.PlacementTarget = this;
			toolTip.VerticalOffset = -5d;
			toolTip.AllowsTransparency = true;
			toolTip.MaxWidth = 150d;

			toolTip.Child = new Border()
			{
				CornerRadius = new CornerRadius(5d),
				BorderThickness = new Thickness(0d),
				Background = SystemColors.InfoBrush,
				Child = new TextBlock(toolTipContent)
				{
					Foreground = SystemColors.InfoTextBrush
				}
			};

			toolTipOpenTimer = new DispatcherTimer(DispatcherPriority.Background, Dispatcher)
			{
				Interval = TimeSpan.FromSeconds(.5)
			};

			toolTipOpenTimer.Tick += toolTipOpenTimer_Tick;
		}

		private void SetTabRowsFollowMouseWithoutUpdate(bool value)
		{
			tabRowsFollowMouse = value;
			ghostLabelsAdorner.IsAdornerEnabled = value;
		}

		private void InvalidateBlockRegions(ICollection<TextChange> collection)
		{
			// TODO: Calculate dirtiness granularly and remove modified elements from blockRegions, taking into account the current viewport.
			blockRegions.Clear();
			blockRegionsDirty = true;
		}

		private void InvalidateBlockRegions(FrameworkContentElement container)
		{
			// TODO: Calculate dirtiness granularly and remove modified elements from blockRegions, taking into account the current viewport.
			blockRegions.Clear();
			blockRegionsDirty = true;
		}

		private void InvalidateBlockRegions()
		{
			// TODO: Clear regions within the current viewport only.  Then modify the UpdateBlockRegions method similarly.
			blockRegions.Clear();
			blockRegionsDirty = true;
		}

		private void Update()
		{
			ClearRenderState();

			Update(Mouse.GetPosition(Editor), forceInvalidate: true);

			ghostLabelsAdorner.InvalidateVisual();

			Dispatcher.BeginInvoke(new Action(() => UpdateTabUnderMouse(Mouse.GetPosition(Editor))), DispatcherPriority.Render);
		}

		private void UpdateTabUnderMouse(Point mouse)
		{
			var noTabUnderMouse = tabUnderMouse == null;

			if (noTabUnderMouse || !tabUnderMouse.RenderedBox.Contains(mouse))
			{
				tabUnderMouse = FindTab(tab => tab.RenderedBox.Contains(mouse));

				if (tabUnderMouse == null)
				{
					if (!noTabUnderMouse)
					{
						HideToolTip();
					}
				}
				else if (toolTip.IsOpen)
				{
					ShowToolTip();
				}
				else
				{
					toolTipOpenTimer.Stop();
					toolTipOpenTimer.Start();
				}
			}
		}

		protected override bool UpdateCore(TextPointer pointer)
		{
			UpdateTabRowsFollowMouseActivationRegion();

			var blockRegionsUpdated = blockRegionsDirty;

			if (blockRegionsDirty)
			{
				UpdateBlockRegions();

				if (blockRegionsDirty)
				{
					ScheduleUpdate(forceInvalidate: true);
				}

				if (pointer == null)
				{
					return true;
				}
			}

			var builder = CreateTabRowBuilder(pointer);

			if (isDebugEnabled)
			{
				debugAdorner.CurrentTabRowBuilder = builder;
			}

			if (!tabRowsFollowMouse)
			{
				ClearRenderState();

				return blockRegionsUpdated;
			}

			if (!builder.ShouldCreateTabRow)
			{
				var requiresUpdate = blockRegionsUpdated || currentTabRowBuilder != null;

				ClearRenderState();

				return requiresUpdate;
			}

			if (currentTabRowBuilder != null && currentTabRowBuilder.Equals(builder))
			{
				return blockRegionsUpdated;
			}

			currentTabRowBuilder = builder;

			CreateTabRows();

			return true;
		}

		protected override void RenderCore(DrawingContext drawingContext)
		{
			drawingContext.PushOpacity(Opacity);

			try
			{
				DrawTabRowsFollowMouseActivationRegion(drawingContext);

				if (isDebugEnabled && blockRegions.Count > 0)
				{
					DrawBlockRegions(drawingContext);
				}

				if (currentTabRowBuilder != null)
				{
					DrawTabRows(drawingContext);
				}
			}
			finally
			{
				drawingContext.Pop();
			}
		}

		protected override void ClearRenderState()
		{
			currentTabRowBuilder = null;

			if (tabUnderMouse != null)
			{
				tabUnderMouse = null;

				HideToolTip();
			}
		}

		private void UpdateTabRowsFollowMouseActivationRegion()
		{
			var width = TabRowsFollowMouseActivationRegionWidth;
			var margin = TabRowsFollowMouseActivationRegionMargin;

			tabRowsFollowMouseActivationRegion = new Rect(new Point(0, 0), Editor.RenderSize);

			if (TabRowsFollowMouseActivationRegionDockLeft)
			{
				tabRowsFollowMouseActivationRegion.Width = width + margin;
			}
			else
			{
				tabRowsFollowMouseActivationRegion.X = tabRowsFollowMouseActivationRegion.Right - width - margin;
				tabRowsFollowMouseActivationRegion.Width = width + margin;
			}
		}

		private void DrawTabRowsFollowMouseActivationRegion(DrawingContext drawingContext)
		{
			var box = tabRowsFollowMouseActivationRegion;
			var transparent = box;
			var margin = TabRowsFollowMouseActivationRegionMargin;

			box.Width -= margin;

			if (TabRowsFollowMouseActivationRegionDockLeft)
			{
				box.X += margin;
			}

			drawingContext.DrawRectangle(Brushes.Transparent, null, transparent);
			drawingContext.DrawRectangle(TabRowBrush, TabRowPen, box);
		}

		private void UpdateBlockRegions()
		{
			if (!Editor.Document.ContentStart.HasValidLayout)
			{
				blockRegions.Clear();
				blockRegionsDirty = true;
				return;
			}

			blockRegionsDirty = false;

			var documentBox = Editor.GetVisualBox();

			documentBox.X += BlockRegionMarkerMargin;
			documentBox.Width -= BlockRegionMarkerMargin * 2 + BlockRegionMarkerRadius * 2;

			foreach (var pair in MamlPartLayout.MeasureLogicalBoxesOfDescendants(Editor.Document, documentBox))
			{
				var descendant = pair.Item1.Element;
				var box = pair.Item2;

				if (!blockRegions.ContainsKey(descendant))
				{
					box.X = documentBox.X;
					box.Width = documentBox.Width;

					blockRegions.Add(descendant, box);
				}
			}
		}

		private void DrawBlockRegions(DrawingContext drawingContext)
		{
			var spacing = BlockRegionMarkerRadius * 2 + BlockRegionMarkerMargin;

			foreach (var pair in blockRegions)
			{
				var box = pair.Value;

				if (!TabRowsFollowMouseActivationRegionDockLeft)
				{
					box.X -= TabRowsFollowMouseActivationRegionMargin + TabRowsFollowMouseActivationRegionWidth;
				}

				drawingContext.DrawEllipse(Brushes.DarkOliveGreen, null, box.BottomRight, BlockRegionMarkerRadius, BlockRegionMarkerRadius);

				box.Width -= spacing;

				var middle = new Point(box.Right, box.GetCenter().Y);

				drawingContext.DrawEllipse(Brushes.Goldenrod, null, middle, BlockRegionMarkerRadius, BlockRegionMarkerRadius);

				box.Width -= spacing;

				drawingContext.DrawEllipse(Brushes.Blue, null, box.TopRight, BlockRegionMarkerRadius, BlockRegionMarkerRadius);
			}
		}

		internal void DeleteTargetElement(StructureTabRow row)
		{
			executingTabFunction = true;

			try
			{
				row.Expected.DeleteTextElements();
			}
			finally
			{
				executingTabFunction = false;
			}

			OnBlockRemoved(EventArgs.Empty);

			InvalidateBlockRegions(row.ContainerPart.ElementOrDocument);

			Update();

			Validate(row);
		}

		internal void AddSibling(StructureTab tab)
		{
			var newElement = document.XVisitor.CreateElement(tab.Node, tab.Schema, tab.Row.ContainerPart.ElementOrDocument as TextElement, tab.Row.ContainerPart.Node);

			var expected = tab.Row.Expected;
			var target = expected.TargetPart;

			executingTabFunction = true;

			try
			{
				if (target != null)
				{
					if (expected.IsExpectedAfterTargetPart)
					{
						target.Element.AddAfterSelf(newElement, tab.Row.ContainerPart.ElementOrDocument);
					}
					else
					{
						target.Element.AddBeforeSelf(newElement, tab.Row.ContainerPart.ElementOrDocument);
					}
				}
				else
				{
					var blocks = tab.Row.ContainerPart.ElementOrDocument.TryGetBlockCollection();

					Contract.Assume(blocks != null);

					var last = blocks.LastBlock;

					if (last == null)
					{
						blocks.Add(newElement.ToBlock());
					}
					else
					{
						blocks.InsertAfter(last, (Block) newElement);
					}
				}
			}
			finally
			{
				executingTabFunction = false;
			}

			OnBlockAdded(EventArgs.Empty);

			InvalidateBlockRegions(tab.Row.ContainerPart.ElementOrDocument);

			Update();

			Validate(tab.Row);
		}

		internal void ShowTargetElementOptions(MamlPart target, IList<Tuple<FrameworkElement, string>> options)
		{
			// TODO: Create a strong-typed Window class with XAML to use as a proper dialog
			var grid = new Grid()
			{
				ColumnDefinitions =
				{
					new ColumnDefinition()
					{
						Width = GridLength.Auto
					},
					new ColumnDefinition()
				}
			};

			foreach (var option in options)
			{
				var control = option.Item1;
				var optionName = option.Item2;

				var row = grid.RowDefinitions.Count;

				var label = new Label()
				{
					Target = control,
					Content = optionName,
					VerticalAlignment = VerticalAlignment.Top
				};

				control.VerticalAlignment = VerticalAlignment.Top;

				Grid.SetRow(label, row);
				Grid.SetRow(control, row);

				Grid.SetColumn(label, 0);
				Grid.SetColumn(control, 1);

				grid.RowDefinitions.Add(new RowDefinition());

				grid.Children.Add(label);
				grid.Children.Add(control);
			}

			var dialog = new Window()
			{
				Title = target.Node.DisplayName + " Options",
				Owner = Application.Current.MainWindow,
				WindowStartupLocation = WindowStartupLocation.CenterOwner,
				Content = grid
			};

			if (dialog.ShowDialog() ?? false)
			{
				target.Node.CommitOptions(options);
			}
		}

		private void Validate(StructureTabRow row)
		{
			var part = row.ContainerPart;

			part.UpdateValidatableData();

			var isValid = document.XVisitor.ValidateChildren(part.Data, part.Schema);

			SetValidity(part.ElementOrDocument, isValid);
		}

		private void SetValidity(FrameworkContentElement contentElement, bool isValid)
		{
			var element = contentElement as TextElement;

			if (isValid)
			{
				if (element == null)
				{
					if (Editor.Document.ReadLocalValue(FlowDocument.BackgroundProperty) == InvalidBackground)
					{
						Editor.Document.ClearValue(FlowDocument.BackgroundProperty);
					}
				}
				else
				{
					if (element.ReadLocalValue(TextElement.BackgroundProperty) == InvalidBackground)
					{
						element.ClearValue(TextElement.BackgroundProperty);
					}
				}
			}
			else
			{
				if (element == null)
				{
					if (Editor.Document.ReadLocalValue(FlowDocument.BackgroundProperty) == DependencyProperty.UnsetValue)
					{
						Editor.Document.Background = InvalidBackground;
					}
				}
				else
				{
					if (element.ReadLocalValue(TextElement.BackgroundProperty) == DependencyProperty.UnsetValue)
					{
						((TextElement) element).Background = InvalidBackground;
					}
				}

				// TODO: Show error exclamation circle at top-right corner
				// Mousing over the circle is a gesture that enables an error adorner (similar gesture behavior to the palette)
				// The error adorner displays error indicators in the doc similar to the palette.  A Tooltip that contains error details is shown.
			}
		}

		private StructureTabRowBuilder CreateTabRowBuilder(TextPointer pointer = null, TextElement parent = null, StructureTabRowBuilder childBuilder = null)
		{
			Contract.Requires(pointer != null || parent != null);

			var documentBox = Editor.GetVisualBox();

			var target = pointer != null
				? pointer.TryGetPart(MamlPart.TryGet, documentBox)
				: parent.TryGetPart(MamlPart.TryGetWithSchema, documentBox);

			return StructureTabRowBuilder.Create(this, target, Mouse.GetPosition(Editor), childBuilder, currentTabRowBuilder);
		}

		private void CreateTabRows()
		{
			Contract.Requires(currentTabRowBuilder.ShouldCreateTabRow);

			if (!currentTabRowBuilder.ShouldCreateParentTabRow
				|| currentTabRowBuilder.ContainerPart.ElementOrDocument is FlowDocument)
			{
				CreateTabRow(currentTabRowBuilder);
				return;
			}

			var builder = currentTabRowBuilder;
			var parent = builder.ContainerPart.Element;

			do
			{
				var parentBuilder = CreateTabRowBuilder(parent: parent, childBuilder: builder);

				if (!parentBuilder.ShouldCreateTabRow)
				{
					break;
				}

				builder = parentBuilder;

				if (!builder.ShouldCreateParentTabRow)
				{
					break;
				}

				parent = parent.Parent as TextElement;
			}
			while (parent != null);

			do
			{
				Contract.Assume(builder.ShouldCreateTabRow);

				CreateTabRow(builder);

				builder = builder.Child;
			}
			while (builder != null);
		}

		private void CreateTabRow(StructureTabRowBuilder builder)
		{
			XmlSchemaParticlesExpected expectedBeforeTarget, expectedAfterTarget;

			var isValid = document.XVisitor.GetExpectedElements(
				builder.TargetPart == null ? null : builder.TargetPart.Data,
				builder.ContainerPart.Data,
				builder.ContainerPart.Schema,
				out expectedBeforeTarget,
				out expectedAfterTarget);

			SetValidity(builder.ContainerPart.ElementOrDocument, isValid);

			if (builder.TargetPart != null)
			{
				expectedBeforeTarget.SetTargetPart(builder.TargetPart, isExpectedAfter: false);
				expectedAfterTarget.SetTargetPart(builder.TargetPart, isExpectedAfter: true);
			}

			builder.CreateTabRows(expectedBeforeTarget, expectedAfterTarget);
		}

		private void DrawTabRows(DrawingContext drawingContext)
		{
			foreach (var row in from builder in currentTabRowBuilder.GetSelfAndAncestors()
													from row in builder.TabRows
													select row)
			{
				row.Draw(drawingContext);
			}
		}

		private StructureTab FindTab(Func<StructureTab, bool> predicate)
		{
			if (currentTabRowBuilder != null)
			{
				return (from builder in currentTabRowBuilder.GetSelfAndAncestors()
								from row in builder.TabRows
								from tab in row
								where predicate(tab)
								select tab)
							 .FirstOrDefault();
			}

			return null;
		}

		private void HideToolTip()
		{
			toolTipOpenTimer.Stop();

			toolTip.IsOpen = false;
		}

		private void ShowToolTip()
		{
			var content = tabUnderMouse.ToolTip;

			if (content == null)
			{
				HideToolTip();
			}
			else
			{
				toolTipContent.Text = content;
				toolTip.PlacementRectangle = tabUnderMouse.RenderedBox;
				toolTip.IsOpen = true;
			}
		}

		private void toolTipOpenTimer_Tick(object sender, EventArgs e)
		{
			toolTipOpenTimer.Stop();

			ShowToolTip();
		}

		protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
		{
			if (isMouseOverTabRowsFollowMouseActivationRegion)
			{
				TabRowsFollowMouse = false;
			}

			if (tabUnderMouse != null)
			{
				tabUnderMouse.OnClick();

				e.Handled = true;
			}

			base.OnMouseLeftButtonUp(e);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			var mouse = e.GetPosition(Editor);

			if (tabRowsFollowMouseActivationRegion.Contains(mouse))
			{
				if (tabRowsFollowMouse)
				{
					// prime for disabling by clicking
					isMouseOverTabRowsFollowMouseActivationRegion = true;
				}
				else if (!isMouseOverTabRowsFollowMouseActivationRegion)
				{
					// mouse gestured
					SetTabRowsFollowMouseWithoutUpdate(true);
				}
			}

			UpdateTabUnderMouse(mouse);

			base.OnMouseMove(e);
		}

		protected override void OnMouseLeave(MouseEventArgs e)
		{
			isMouseOverTabRowsFollowMouseActivationRegion = false;

			if (tabUnderMouse != null)
			{
				tabUnderMouse = null;

				HideToolTip();
			}

			base.OnMouseLeave(e);
		}

		protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
		{
			InvalidateBlockRegions();

			base.OnRenderSizeChanged(sizeInfo);
		}

		protected override void OnEditorMouseDown(MouseButtonEventArgs e)
		{
			if (tabRowsFollowMouse)
			{
				TabRowsFollowMouse = false;
			}
		}

		protected override void OnEditorTextChanged(TextChangedEventArgs e)
		{
			if (!executingTabFunction)
			{
				SetTabRowsFollowMouseWithoutUpdate(false);
			}

			InvalidateBlockRegions(e.Changes);

			Update(null, forceUpdate: blockRegionsDirty, forceInvalidate: true);
		}

		public event EventHandler BlockAdded;
		public event EventHandler BlockRemoved;

		private void OnBlockAdded(EventArgs e)
		{
			var handler = BlockAdded;

			if (handler != null)
			{
				handler(this, e);
			}
		}

		private void OnBlockRemoved(EventArgs e)
		{
			var handler = BlockRemoved;

			if (handler != null)
			{
				handler(this, e);
			}
		}
	}
}