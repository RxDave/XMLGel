using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using DaveSexton.XmlGel.Extensions;
using DaveSexton.XmlGel.Maml.Documents.Adorners;
using SD = System.Drawing;

namespace DaveSexton.XmlGel.Labs.WPF
{
	partial class FlowDocumentLayoutLab
	{
		const int experimentsPerPage = 5;
		const int cellMargin = 5;
		const double textSize = 10;
		const double markerRadius = 3;
		private static readonly string paragraph1 = string.Concat(Enumerable.Repeat("xxx ", 12).ToArray());
		private static readonly string paragraph2 = string.Concat(Enumerable.Repeat("ooo ", 10).ToArray());
		private static readonly Brush lineBrush = Brushes.ForestGreen;
		private static readonly Brush textBrush = Brushes.Black;
		private static readonly Typeface typeface = new Typeface(new FontFamily("Segoe UI"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);

		private static readonly IList<SD.Color> elementColors = new List<SD.Color>()
			{
				SD.Color.Red, SD.Color.Blue, SD.Color.Goldenrod, SD.Color.Purple, SD.Color.Green, SD.Color.DarkOrange, SD.Color.Violet, SD.Color.Aqua
			}
			.AsReadOnly();

		private static readonly IList<SD.Color> contentColors = new List<SD.Color>()
			{
				SD.Color.DeepPink, SD.Color.Turquoise, SD.Color.PaleGoldenrod, SD.Color.MediumPurple, SD.Color.LightGreen, SD.Color.Orange, SD.Color.PaleVioletRed, SD.Color.Azure
			}
			.AsReadOnly();

		private readonly List<BoxData> documentBoxes = new List<BoxData>();
		private readonly List<MethodInfo> experiments;
		private Grid grid;
		private int currentPage, experimentsWithFullNestingFirstPage, experimentsWithJaggedNestingFirstPage, experimentsWithConsecutiveParagraphsFirstPage;

		public FlowDocumentLayoutLab()
		{
			InitializeComponent();

			experiments = (from method in GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
										 where method.Name.StartsWith("Measure")
										 where method.ReturnType == typeof(Task)
										 where method.GetParameters().Length == 0
										 select method)
										 .ToList();

			experimentsWithFullNestingFirstPage = experiments
				.Select((e, index) => new { e, index })
				.First(x => x.e.Name == firstExperimentWithFullNesting)
				.index / experimentsPerPage;

			experimentsWithJaggedNestingFirstPage = experiments
				.Select((e, index) => new { e, index })
				.First(x => x.e.Name == firstExperimentWithJaggedNesting)
				.index / experimentsPerPage;

			experimentsWithConsecutiveParagraphsFirstPage = experiments
				.Select((e, index) => new { e, index })
				.First(x => x.e.Name == firstExperimentWithConsecutiveParagraphs)
				.index / experimentsPerPage;

			Loaded += (sender, e) => ExecuteExperiments();
		}

		private async Task ResetGrid()
		{
			grid = new Grid()
			{
				SnapsToDevicePixels = true,
				ColumnDefinitions =
					{
						new ColumnDefinition() { Width = GridLength.Auto }, 
						new ColumnDefinition(), 
						new ColumnDefinition(), 
						new ColumnDefinition(), 
						new ColumnDefinition()
					},
				RowDefinitions =
					{
						new RowDefinition()
						{
							Height = GridLength.Auto
						}
					}
			};

			var text = new TextBlock(new Run("Element Forward")) { Margin = new Thickness(cellMargin) };
			text.SetValue(Grid.RowProperty, 0);
			text.SetValue(Grid.ColumnProperty, 1);
			grid.Children.Add(text);

			text = new TextBlock(new Run("Element Backward")) { Margin = new Thickness(cellMargin) };
			text.SetValue(Grid.RowProperty, 0);
			text.SetValue(Grid.ColumnProperty, 2);
			grid.Children.Add(text);

			text = new TextBlock(new Run("Content Forward")) { Margin = new Thickness(cellMargin) };
			text.SetValue(Grid.RowProperty, 0);
			text.SetValue(Grid.ColumnProperty, 3);
			grid.Children.Add(text);

			text = new TextBlock(new Run("Content Backward")) { Margin = new Thickness(cellMargin) };
			text.SetValue(Grid.RowProperty, 0);
			text.SetValue(Grid.ColumnProperty, 4);
			grid.Children.Add(text);

			this.Content = grid;

			AddPager();

			await Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(
				h => grid.Loaded += h,
				h => grid.Loaded -= h)
				.Take(1);
		}

		private void AddPager()
		{
			grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

			if (currentPage > 0)
			{
				var previous = new Button() { Content = "Previous Page", HorizontalAlignment = HorizontalAlignment.Left, Margin = new Thickness(cellMargin) };
				previous.SetValue(Grid.RowProperty, experimentsPerPage + 1);
				previous.SetValue(Grid.ColumnProperty, 0);
				previous.SetValue(Grid.ColumnSpanProperty, 2);
				previous.Click += (sender, e) =>
				{
					currentPage--;
					ExecuteExperiments();
				};

				grid.Children.Add(previous);
			}

			var centerStack = new StackPanel()
			{
				Orientation = Orientation.Horizontal,
				HorizontalAlignment = HorizontalAlignment.Center,
				Margin = new Thickness(cellMargin, cellMargin, 0, cellMargin)
			};
			centerStack.SetValue(Grid.RowProperty, experimentsPerPage + 1);
			centerStack.SetValue(Grid.ColumnProperty, 0);
			centerStack.SetValue(Grid.ColumnSpanProperty, grid.ColumnDefinitions.Count);

			if (currentPage < experimentsWithFullNestingFirstPage || currentPage > experimentsWithFullNestingFirstPage)
			{
				var button = new Button() { Content = "Full Nesting", Margin = new Thickness(0, 0, cellMargin, 0) };
				button.Click += (sender, e) =>
				{
					currentPage = experimentsWithFullNestingFirstPage;
					ExecuteExperiments();
				};

				centerStack.Children.Add(button);
			}

			if (currentPage < experimentsWithJaggedNestingFirstPage || currentPage > experimentsWithJaggedNestingFirstPage)
			{
				var button = new Button() { Content = "Jagged Nesting", Margin = new Thickness(0, 0, cellMargin, 0) };
				button.Click += (sender, e) =>
				{
					currentPage = experimentsWithJaggedNestingFirstPage;
					ExecuteExperiments();
				};

				centerStack.Children.Add(button);
			}

			if (currentPage < experimentsWithConsecutiveParagraphsFirstPage || currentPage > experimentsWithConsecutiveParagraphsFirstPage)
			{
				var button = new Button() { Content = "Consecutive Paragraphs", Margin = new Thickness(0, 0, cellMargin, 0) };
				button.Click += (sender, e) =>
				{
					currentPage = experimentsWithConsecutiveParagraphsFirstPage;
					ExecuteExperiments();
				};

				centerStack.Children.Add(button);
			}

			grid.Children.Add(centerStack);

			if ((currentPage + 1) * experimentsPerPage < experiments.Count)
			{
				var next = new Button() { Content = "Next Page", HorizontalAlignment = HorizontalAlignment.Right, Margin = new Thickness(cellMargin) };
				next.SetValue(Grid.RowProperty, experimentsPerPage + 1);
				next.SetValue(Grid.ColumnProperty, 3);
				next.SetValue(Grid.ColumnSpanProperty, 2);
				next.Click += (sender, e) =>
				{
					currentPage++;
					ExecuteExperiments();
				};

				grid.Children.Add(next);
			}
		}

		private async void ExecuteExperiments()
		{
			await ResetGrid();

			documentBoxes.Clear();

			var page = experiments
				.Skip(currentPage * experimentsPerPage)
				.Take(experimentsPerPage)
				.Select(method =>
					{
						return (Task) method.Invoke(this, null);
					})
				.ToArray();

			await Task.WhenAll(page);

			AddBoxShapes();
		}

		private RichTextBox AddEditor(int row, string experimentName, int experimentNumber, out Canvas elementForward, out Canvas elementBackward, out Canvas contentForward, out Canvas contentBackward)
		{
			var editorLabel = new TextBlock(new Run(experimentNumber + "."));
			editorLabel.SetValue(Grid.RowProperty, row);
			editorLabel.SetValue(Grid.ColumnProperty, 0);
			editorLabel.ToolTip = experimentName;
			grid.Children.Add(editorLabel);

			elementForward = AddCanvas(row, 1);
			elementBackward = AddCanvas(row, 2);
			contentForward = AddCanvas(row, 3);
			contentBackward = AddCanvas(row, 4);

			var editor = new RichTextBox()
			{
				IsDocumentEnabled = true,
				BorderThickness = new Thickness(0),
				Padding = new Thickness(0)
			};

			var border = (Border) elementForward.Parent;

			border.Child = null;
			border.Child = new Grid()
			{
				Children = 
				{
					new ScrollViewer()	// Expected by the TextElementExtensions.GetVisualBox method.  (The MamlTopicEditor XAML is similar.)
					{
						VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
						Content = new AdornerDecorator()
						{
							Child = editor
						}
					}, 
					elementForward
				}
			};

			AddMouseAdorner(editor);

			return editor;
		}

		private Canvas AddCanvas(int row, int column)
		{
			var canvas = new Canvas();

			var border = new Border()
			{
				Margin = new Thickness(cellMargin),
				BorderThickness = new Thickness(1),
				BorderBrush = Brushes.Black
			};

			border.SetValue(Grid.RowProperty, row);
			border.SetValue(Grid.ColumnProperty, column);

			border.Child = canvas;

			grid.Children.Add(border);

			return canvas;
		}

		private void AddMouseAdorner(RichTextBox editor)
		{
			var layer = AdornerLayer.GetAdornerLayer(editor);

			var adorner = new MouseAdorner(this, editor)
			{
				IsEnabled = false
			};

			editor.Tag = adorner;

			layer.Add(adorner);
		}

		private async Task Measure(FlowDocument document, [CallerMemberName] string experimentName = null)
		{
			grid.RowDefinitions.Insert(grid.RowDefinitions.Count - 1, new RowDefinition());

			var row = grid.RowDefinitions.Count - 2;
			var experimentNumber = row + currentPage * experimentsPerPage;

			Canvas elementForward, elementBackward, contentForward, contentBackward;
			var editor = AddEditor(row, experimentName, experimentNumber, out elementForward, out elementBackward, out contentForward, out contentBackward);

			editor.Document = document;

			await EnsureValidLayout(document);

			TraceStatus("Experiment {0}: {1}", experimentNumber, experimentName);
			TraceLine();
			TraceSuccess("Editor: {0}", editor.GetVisualBox());
			TraceLine();

			Measure(document, editor, elementForward, elementBackward, contentForward, contentBackward);
			ResetTagRecursive(document);

			var adorner = (MouseAdorner) editor.Tag;

			adorner.IsEnabled = true;
		}

		private void Measure(FlowDocument document, RichTextBox editor, Canvas elementForward, Canvas elementBackward, Canvas contentForward, Canvas contentBackward)
		{
			TraceSuccess(document.GetType().Name);
			TraceLine("\t ContentStart (Forward): {0}", document.ContentStart.GetCharacterRect(LogicalDirection.Forward));
			TraceLine("\t   ContentEnd (Forward): {0}", document.ContentEnd.GetCharacterRect(LogicalDirection.Forward));
			TraceLine("\tContentStart (Backward): {0}", document.ContentStart.GetCharacterRect(LogicalDirection.Backward));
			TraceLine("\t  ContentEnd (Backward): {0}", document.ContentEnd.GetCharacterRect(LogicalDirection.Backward));
			TraceLine();

			FrameworkContentElement previous = document;
			int depth = 0, current = 0;

			document.Tag = depth;

			foreach (var element in document.GetDescendantStructure())
			{
				if (element.Parent == previous)
				{
					depth++;
				}
				else if (element.Parent != previous.Parent)
				{
					depth = (int) ((FrameworkContentElement) element.Parent).Tag + 1;
				}

				element.Tag = depth;

				var elementColor = elementColors[current];
				var contentColor = contentColors[current++];

				var data = new BoxData()
				{
					ElementForwardCanvas = elementForward,
					ElementBackwardCanvas = elementBackward,
					ContentForwardCanvas = contentForward,
					ContentBackwardCanvas = contentBackward,
					ElementColor = Color.FromRgb(elementColor.R, elementColor.G, elementColor.B),
					ContentColor = Color.FromRgb(contentColor.R, contentColor.G, contentColor.B),
					Name = element.GetType().Name,
					ElementStartForward = element.ElementStart.GetCharacterRect(LogicalDirection.Forward),
					ElementEndForward = element.ElementEnd.GetCharacterRect(LogicalDirection.Forward),
					ElementStartBackward = element.ElementStart.GetCharacterRect(LogicalDirection.Backward),
					ElementEndBackward = element.ElementEnd.GetCharacterRect(LogicalDirection.Backward),
					ContentStartForward = element.ContentStart.GetCharacterRect(LogicalDirection.Forward),
					ContentEndForward = element.ContentEnd.GetCharacterRect(LogicalDirection.Forward),
					ContentStartBackward = element.ContentStart.GetCharacterRect(LogicalDirection.Backward),
					ContentEndBackward = element.ContentEnd.GetCharacterRect(LogicalDirection.Backward)
				};

				documentBoxes.Add(data);

				var tabs = new string('\t', depth);

				TraceSuccess(tabs + data.Name + " (Child of " + element.Parent.GetType().Name + ")");

				TraceLine("{0} ElementStart Forward: {2} ({1})", tabs, elementColor.Name, data.ElementStartForward);
				TraceLine("{0}   ElementEnd Forward: {2} ({1})", tabs, elementColor.Name, data.ElementEndForward);
				TraceLine("{0}ElementStart Backward: {2} ({1})", tabs, elementColor.Name, data.ElementStartBackward);
				TraceLine("{0}  ElementEnd Backward: {2} ({1})", tabs, elementColor.Name, data.ElementEndBackward);

				TraceLine("{0} ContentStart Forward: {2} ({1})", tabs, contentColor.Name, data.ContentStartForward);
				TraceLine("{0}   ContentEnd Forward: {2} ({1})", tabs, contentColor.Name, data.ContentEndForward);
				TraceLine("{0}ContentStart Backward: {2} ({1})", tabs, contentColor.Name, data.ContentStartBackward);
				TraceLine("{0}  ContentEnd Backward: {2} ({1})", tabs, contentColor.Name, data.ContentEndBackward);
				TraceLine();

				previous = element;
			}
		}

		private async Task EnsureValidLayout(FlowDocument document)
		{
			Debug.Assert(!document.ContentStart.HasValidLayout, "document.ContentStart.HasValidLayout is unexpectedly true.");

			while (!document.ContentStart.HasValidLayout)
			{
				// After paging was implemented, clicking Next would sometimes hang the UI while it got stuck in this never-ending loop.
				// Calling InvalidateArrange does not seem to prevent this, though the hang doesn't happen often anyway.  When it does
				// happen, the solution is to place a break point on the following await and then move execution outside of the while loop.
				// InvalidateArrange();
				// Update: Changed DispatcherPriority from Loaded to Input and that seems to have fixed the problem.

				await Dispatcher.BeginInvoke(new Action(() => { }), DispatcherPriority.Input);
				// Calling BeginInvoke once with Normal priority used to be enough to fix an invalid layout; however, after paging was
				// implemented, calling BeginInvoke once with Normal priority was not enough - I don't know why.  Calling it in a loop 
				// with Input priority seems to work, though perhaps only twice is necessary.
			}
		}

		[DebuggerStepThrough]
		public static void ResetTagRecursive(FrameworkContentElement element, int? childIndex = null)
		{
			var tag = element.Tag as string ?? string.Empty;

			if (tag.Length > 0)
			{
				tag += '-';
			}

			if (childIndex.HasValue)
			{
				tag += childIndex + " (" + element.GetType().Name + ')';
			}
			else
			{
				tag += "FlowDoc";
			}

			element.Tag = tag;

			var children = element.TryGetChildCollection();

			if (children != null)
			{
				var current = 0;

				foreach (TextElement child in children)
				{
					child.Tag = element.Tag;

					ResetTagRecursive(child, current++);
				}
			}
		}

		private void AddBoxShapes()
		{
			foreach (var data in documentBoxes)
			{
				var elementBrush = new SolidColorBrush(data.ElementColor);
				elementBrush.Freeze();

				var contentBrush = new SolidColorBrush(data.ContentColor);
				contentBrush.Freeze();

				AddBoxShape(data.ElementStartForward, data.ElementEndForward, data.ElementForwardCanvas, elementBrush);
				AddBoxShape(data.ElementStartBackward, data.ElementEndBackward, data.ElementBackwardCanvas, elementBrush);

				AddBoxShape(data.ContentStartForward, data.ContentEndForward, data.ContentForwardCanvas, contentBrush);
				AddBoxShape(data.ContentStartBackward, data.ContentEndBackward, data.ContentBackwardCanvas, contentBrush);
			}
		}

		private void AddBoxShape(Rect start, Rect end, Canvas canvas, Brush brush)
		{
			if (start.Width == 0)
			{
				start.Width = 1;
			}

			if (start.Height == 0)
			{
				start.Height = 1;
			}

			canvas.Children.Add(ToFilledShape(start, brush));
			canvas.Children.Add(ToFilledShape(end, brush));

			if (!start.IsEmpty)
			{
				canvas.Children.Add(ToTextBlock("S", Offset(start.TopLeft, x: 1, y: 1), brush));
			}

			if (!end.IsEmpty)
			{
				canvas.Children.Add(ToTextBlock("E", Offset(end.BottomRight, x: 5, y: -9), brush));
			}

			var box = start;
			box.Union(end);

			canvas.Children.Add(ToStrokedShape(box, brush));
		}

		private void DrawMouseAdorner(RichTextBox editor, DrawingContext drawingContext)
		{
			if (!editor.Document.ContentStart.HasValidLayout)
			{
				return;
			}

			var mouse = Mouse.GetPosition(editor);

			var current = 0;
			var sdBrush = elementColors[current++];
			var brush = new SolidColorBrush(Color.FromArgb(50, sdBrush.R, sdBrush.G, sdBrush.B));

			var documentBox = editor.GetVisualBox();

			if (!documentBox.Contains(mouse))
			{
				return;
			}

			drawingContext.DrawRectangle(brush, null, documentBox);

			var lineMarkerXOffset = -markerRadius * 10;

			foreach (var descendant in editor.Document.GetDescendantStructure())
			{
				var box = MeasurementContext.MeasureLogicalBox(descendant, documentBox);

				if (box.Contains(mouse))
				{
					sdBrush = elementColors[current];
					brush = new SolidColorBrush(Color.FromArgb(50, sdBrush.R, sdBrush.G, sdBrush.B));

					drawingContext.DrawRectangle(brush, null, box);
					drawingContext.DrawText(descendant.Tag.ToString(), typeface, textSize, textBrush, new Point(box.Left, box.GetCenter().Y - textSize / 2), 2);

					foreach (var line in GetInsertionLines(descendant, box, documentBox))
					{
						var lineMarker = line.Box.GetCenter();

						switch (line.RelativeLocation)
						{
							case StructureTabLocation.Above:
								lineMarker.Offset(lineMarkerXOffset, 0);
								break;
							case StructureTabLocation.Centered:
								lineMarker.Offset(lineMarkerXOffset + markerRadius * 2, 0);
								break;
							case StructureTabLocation.Below:
								lineMarker.Offset(lineMarkerXOffset + markerRadius * 4, 0);
								break;
						}

						drawingContext.DrawRectangle(lineBrush, null, line.Box);
						drawingContext.DrawEllipse(lineBrush, null, lineMarker, markerRadius, markerRadius);
						drawingContext.DrawText(line.RelativeLocation.ToString(), typeface, textSize, textBrush, lineMarker, markerRadius + 1, -textSize / 2);
					}

					lineMarkerXOffset += markerRadius * 16;
				}
			}

			var position = editor.GetPositionFromPoint(mouse);
			var target = position == null ? null : (FrameworkContentElement) position.Parent;

			if (target != null && target.Tag != null)
			{
				drawingContext.DrawText(target.Tag.ToString(), typeface, textSize, textBrush, mouse, 0, -textSize);
			}
		}

		private static Rectangle ToFilledShape(Rect rect, Brush brush)
		{
			if (rect.IsEmpty)
			{
				return new Rectangle();
			}

			var rectangle = new Rectangle()
			{
				IsHitTestVisible = false,
				Width = rect.Width,
				Height = rect.Height,
				Fill = brush,
				StrokeThickness = 0
			};

			rectangle.SetValue(Canvas.LeftProperty, rect.X);
			rectangle.SetValue(Canvas.TopProperty, rect.Y);

			return rectangle;
		}

		private static Rectangle ToStrokedShape(Rect rect, Brush brush)
		{
			if (rect.IsEmpty)
			{
				return new Rectangle();
			}

			var rectangle = new Rectangle()
			{
				IsHitTestVisible = false,
				Width = rect.Width,
				Height = rect.Height,
				Stroke = brush,
				StrokeThickness = 1
			};

			rectangle.SetValue(Canvas.LeftProperty, rect.X);
			rectangle.SetValue(Canvas.TopProperty, rect.Y);

			return rectangle;
		}

		private static TextBlock ToTextBlock(string text, Point location, Brush brush)
		{
			var block = new TextBlock(new Run(text))
			{
				FontFamily = new FontFamily("Segoe UI"),
				FontSize = 8,
				Foreground = brush,
				Background = Brushes.Transparent
			};

			block.SetValue(Canvas.LeftProperty, location.X);
			block.SetValue(Canvas.TopProperty, location.Y);

			return block;
		}

		private static Point Offset(Point point, double x = 0, double y = 0)
		{
			point.Offset(x, y);

			return point;
		}

		private sealed class InsertionLine
		{
			public Rect Box { get; set; }
			public StructureTabLocation RelativeLocation { get; set; }
		}

		private sealed class BoxData
		{
			public string Name { get; set; }
			public Color ElementColor { get; set; }
			public Color ContentColor { get; set; }
			public Canvas ElementForwardCanvas { get; set; }
			public Canvas ElementBackwardCanvas { get; set; }
			public Canvas ContentForwardCanvas { get; set; }
			public Canvas ContentBackwardCanvas { get; set; }
			public Rect ElementStartForward { get; set; }
			public Rect ElementStartBackward { get; set; }
			public Rect ContentStartForward { get; set; }
			public Rect ContentStartBackward { get; set; }
			public Rect ContentEndForward { get; set; }
			public Rect ContentEndBackward { get; set; }
			public Rect ElementEndForward { get; set; }
			public Rect ElementEndBackward { get; set; }
		}

		private sealed class MouseAdorner : Adorner
		{
			private readonly FlowDocumentLayoutLab lab;
			private readonly RichTextBox editor;
			private bool canRender;

			public MouseAdorner(FlowDocumentLayoutLab lab, RichTextBox editor)
				: base(editor)
			{
				this.lab = lab;
				this.editor = editor;

				IsHitTestVisible = false;

				editor.MouseMove += editor_MouseMove;
				editor.MouseLeave += editor_MouseLeave;
			}

			private void editor_MouseMove(object sender, MouseEventArgs e)
			{
				canRender = true;

				InvalidateVisual();
			}

			private void editor_MouseLeave(object sender, MouseEventArgs e)
			{
				canRender = false;

				InvalidateVisual();
			}

			protected override void OnRender(DrawingContext drawingContext)
			{
				if (canRender && IsEnabled)
				{
					lab.DrawMouseAdorner(editor, drawingContext);
				}
			}
		}
	}
}