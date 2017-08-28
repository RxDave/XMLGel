using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Threading;
using DaveSexton.XmlGel.Extensions;
using DaveSexton.XmlGel.Labs.WPF;
using DaveSexton.XmlGel.Maml;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DaveSexton.XmlGel.UnitTests.Documents
{
	/* For the purpose of the following description, an "empty section" may contain other empty sections with any 
	 * amount of jagged nesting or depth, but it doesn't have any descendant that is content, such as a paragraph, 
	 * even if the paragraph is empty.
	 * ------------
	 * 
	 * Some tests may seem wrong, but in fact a paragraph following an empty section causes that section's content 
	 * box (character start * character end) to be displaced to the start of the paragraph, and the paragraph's box
	 * to be displaced by 1 additional line height below where the section should have been.
	 * 
	 * To compensate, the paragraph's logical box is calculated from where the text actually starts (so that there 
	 * doesn't appear to be an extra large top margin) and the preceeding section fills up the remainder of the top 
	 * space, assuming that it's the only empty section.
	 * 
	 * The remainder of the top space happens to be exactly line height * N, where N is the number of consecutive 
	 * empty sections + 1 (for the paragraph itself) + the number of jaggedly nested empty sections that are 
	 * descendants of the section and any siblings found between the section and the following paragraph, including 
	 * those empty sections that are descendants of the paragraph's ancestor if the paragraph is not a sibling of 
	 * the section, yet excluding any singly-nested sections (i.e., a childless section that is the only child of a
	 * section counts as 1, not 2).  The total displacement of the paragraph is partitioned evenly among its 
	 * preceeding consecutive empty sections (whether siblings or descendants).
	 * 
	 * (Yes, it's recidiculous and confusing.  It would be great if RichTextBox simply exposed its own logical box
	 * calculation, but alas I couldn't find any APIs, documentation or forum posts even alluding to that point.)
	 * 
	 * The result is similar when an empty section follows a paragraph, except in reverse.  In this case, the 
	 * displacement of the section is counted after the paragraph so that the logical box of the first consecutive 
	 * empty section starts at the end of the paragraph.  If, for example, it's the only empty section that follows 
	 * the paragraph and there are no empty descendants, then the height of the section's logical box continues 
	 * until the top of the following content or to the end of the document - whichever comes first.
	 */

	/* Semantic Naming Convention (used by tests defined in partial classes):
	 * 
	 * Nested
	 *		Applies to all subsequent elements until a higher level modifier is used (above the level at which the term "Nested" was used.)
	 *
	 * Then
	 *		Concatenates siblings at depth = 0.
	 *	
	 * And
	 *		Concatenates siblings at depth = 1.
	 *	
	 * Before
	 *		Concatenates siblings at depth = 2.
	 */

	[TestClass]
	public partial class MamlPartLayoutTests
	{
		const double lineHeight = 15.96;
		const double consecutiveParagraphOffset = lineHeight * 2;
		const double sectionDisplacementBetweenParagraphs = consecutiveParagraphOffset + lineHeight;
		const double documentWidth = 200;
		const double documentHeight = 100;
		private static Rect documentBox = new Rect(0, 0, documentWidth, documentHeight);
		private static readonly string paragraph1 = string.Concat(Enumerable.Repeat("xxx ", 8).ToArray());
		private static readonly string paragraph2 = string.Concat(Enumerable.Repeat("ooo ", 6).ToArray());

		[TestInitialize]
		public void Initialize()
		{
			var context = new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher);

			DispatcherSynchronizationContext.SetSynchronizationContext(context);
		}

		private static Rect MeasureLogicalBox(TextElement element, Rect documentBox)
		{
			return MamlPartLayoutMeasurementContext.MeasureLogicalBox(element, documentBox);
			//return FlowDocumentLayoutLab.MeasurementContext.MeasureLogicalBox(element, documentBox);
		}

		private static async Task AssertMeasurements(FlowDocument document, params Rect[] expectedMeasurements)
		{
			if (Debugger.IsAttached)
			{
				FlowDocumentLayoutLab.ResetTagRecursive(document);
			}

			var ui = Measure(document);

			var actualMeasurements = await ui;

			var actualMeasurementsAsString = actualMeasurements.Aggregate("", (acc, cur) => acc + Environment.NewLine + cur);
			var expectedMeasurementsAsString = expectedMeasurements.Aggregate("", (acc, cur) => acc + Environment.NewLine + cur);

			var measurements = "Actual: " + actualMeasurementsAsString + Environment.NewLine + "Expected: " + expectedMeasurementsAsString;

			// Rounding isn't typically necessary, though certain tests require it; specifically, those related to displacement of two or
			// more consecutive empty sections preceeded by a paragraph.
			Round(expectedMeasurements);
			Round(actualMeasurements);

			var message = "Unexpected layout measurements." + Environment.NewLine + measurements + Environment.NewLine;

			CollectionAssert.AreEqual(expectedMeasurements, actualMeasurements, message);
		}

		private static async Task<List<Rect>> Measure(FlowDocument document)
		{
			var measurements = new List<Rect>();

			var editor = new RichTextBox()
			{
				IsDocumentEnabled = true,
				BorderThickness = new Thickness(0),
				Padding = new Thickness(0),
				Document = document
			};

			var window = new Window()
			{
				Content = editor,
				ShowInTaskbar = false,
				IsHitTestVisible = false,
				ResizeMode = ResizeMode.NoResize,
				WindowStyle = WindowStyle.None,
				WindowStartupLocation = WindowStartupLocation.Manual,
				Top = 0,
				Left = 0,
				Width = documentBox.Width,
				Height = documentBox.Height
			};

			window.Show();

			try
			{
				await EnsureValidLayout(document);

				var editorBox = editor.GetVisualBox();

				measurements.Add(editorBox);

				foreach (var descendant in document.GetDescendantStructure())
				{
					measurements.Add(MeasureLogicalBox(descendant, editorBox));
				}
			}
			finally
			{
				window.Close();
			}

			return measurements;
		}

		private static async Task EnsureValidLayout(FlowDocument document)
		{
			while (!document.ContentStart.HasValidLayout)
			{
				await document.Dispatcher.BeginInvoke(new Action(() => { }), DispatcherPriority.Input);
			}
		}

		private static void Round(IList<Rect> measurements)
		{
			for (int i = 0; i < measurements.Count; i++)
			{
				var measurement = measurements[i];

				measurement.X = Math.Round(measurement.X, 2);
				measurement.Y = Math.Round(measurement.Y, 2);
				measurement.Width = Math.Round(measurement.Width, 2);
				measurement.Height = Math.Round(measurement.Height, 2);

				measurements[i] = measurement;
			}
		}
	}
}