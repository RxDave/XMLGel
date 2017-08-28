using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DaveSexton.XmlGel.UnitTests.Documents
{
	partial class MamlPartLayoutTests
	{
		#region Single Childless Section
		[TestMethod]
		public async Task Layout_NestedParagraphAndSection()
		{
			var section = new Section();

			section.Blocks.Add(new Paragraph());
			section.Blocks.Add(new Section());

			await AssertMeasurements(new FlowDocument(section),
				documentBox,
				documentBox,
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, lineHeight, 200, documentHeight - lineHeight));
		}

		[TestMethod]
		public async Task Layout_NestedSectionAndParagraph()
		{
			var section = new Section();

			section.Blocks.Add(new Section());
			section.Blocks.Add(new Paragraph());

			var displacement = lineHeight * 2;

			await AssertMeasurements(new FlowDocument(section),
				documentBox,
				documentBox,
				new Rect(0, 0, 200, displacement),
				new Rect(0, displacement, 200, documentHeight - displacement));
		}

		[TestMethod]
		public async Task Layout_NestedParagraphAndSectionAndParagraph()
		{
			var section = new Section();

			section.Blocks.Add(new Paragraph());
			section.Blocks.Add(new Section());
			section.Blocks.Add(new Paragraph());

			var displacement = sectionDisplacementBetweenParagraphs;

			await AssertMeasurements(new FlowDocument(section),
				documentBox,
				documentBox,
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, lineHeight, 200, displacement),
				new Rect(0, lineHeight + displacement, 200, documentHeight - displacement - lineHeight));
		}
		#endregion

		#region Two Childless Sections
		[TestMethod]
		public async Task Layout_NestedSectionAndSection()
		{
			var section = new Section();

			section.Blocks.Add(new Section());
			section.Blocks.Add(new Section());

			await AssertMeasurements(new FlowDocument(section),
				documentBox,
				documentBox,
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, lineHeight, 200, documentHeight - lineHeight));
		}

		[TestMethod]
		public async Task Layout_NestedParagraphAndSectionAndSection()
		{
			var section = new Section();

			section.Blocks.Add(new Paragraph());
			section.Blocks.Add(new Section());
			section.Blocks.Add(new Section());

			var displacement = documentHeight - lineHeight;

			await AssertMeasurements(new FlowDocument(section),
				documentBox,
				documentBox,
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, lineHeight, 200, displacement / 2),
				new Rect(0, lineHeight + displacement / 2, 200, displacement / 2));
		}

		[TestMethod]
		public async Task Layout_NestedSectionAndSectionAndParagraph()
		{
			var section = new Section();

			section.Blocks.Add(new Section());
			section.Blocks.Add(new Section());
			section.Blocks.Add(new Paragraph());

			var displacement = lineHeight * 3;

			await AssertMeasurements(new FlowDocument(section),
				documentBox,
				documentBox,
				new Rect(0, 0, 200, displacement / 2),
				new Rect(0, displacement / 2, 200, displacement / 2),
				new Rect(0, displacement, 200, documentHeight - displacement));
		}

		[TestMethod]
		public async Task Layout_NestedParagraphAndSectionAndSectionAndParagraph()
		{
			var section = new Section();

			section.Blocks.Add(new Paragraph());
			section.Blocks.Add(new Section());
			section.Blocks.Add(new Section());
			section.Blocks.Add(new Paragraph());

			var displacement = sectionDisplacementBetweenParagraphs + lineHeight;

			await AssertMeasurements(new FlowDocument(section),
				documentBox,
				documentBox,
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, lineHeight, 200, displacement / 2),
				new Rect(0, lineHeight + displacement / 2, 200, displacement / 2),
				new Rect(0, lineHeight + displacement, 200, documentHeight - displacement - lineHeight));
		}
		#endregion

		#region Single Nested Section
		[TestMethod]
		public async Task Layout_NestedNestedSection()
		{
			var section = new Section();

			section.Blocks.Add(new Section(new Section()));

			await AssertMeasurements(new FlowDocument(section),
				documentBox,
				documentBox,
				documentBox,
				documentBox);
		}

		[TestMethod]
		public async Task Layout_NestedParagraphAndNestedSection()
		{
			var section = new Section();

			section.Blocks.Add(new Paragraph());
			section.Blocks.Add(new Section(new Section()));

			var displacement = documentHeight - lineHeight;

			await AssertMeasurements(new FlowDocument(section),
				documentBox,
				documentBox,
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, lineHeight, 200, displacement),
				new Rect(0, lineHeight, 200, displacement));
		}

		[TestMethod]
		public async Task Layout_NestedNestedSectionAndParagraph()
		{
			var section = new Section();

			section.Blocks.Add(new Section(new Section()));
			section.Blocks.Add(new Paragraph());

			var displacement = lineHeight * 2;

			await AssertMeasurements(new FlowDocument(section),
				documentBox,
				documentBox,
				new Rect(0, 0, 200, displacement),
				new Rect(0, 0, 200, displacement),
				new Rect(0, displacement, 200, documentHeight - displacement));
		}

		[TestMethod]
		public async Task Layout_NestedParagraphAndNestedSectionAndParagraph()
		{
			var section = new Section();

			section.Blocks.Add(new Paragraph());
			section.Blocks.Add(new Section(new Section()));
			section.Blocks.Add(new Paragraph());

			var displacement = sectionDisplacementBetweenParagraphs;

			await AssertMeasurements(new FlowDocument(section),
				documentBox,
				documentBox,
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, lineHeight, 200, displacement),
				new Rect(0, lineHeight, 200, displacement),
				new Rect(0, lineHeight + displacement, 200, documentHeight - displacement - lineHeight));
		}
		#endregion

		#region Two Sections; One Childless and One Nested
		[TestMethod]
		public async Task Layout_NestedNestedSectionAndSection()
		{
			var section = new Section();

			section.Blocks.Add(new Section(new Section()));
			section.Blocks.Add(new Section());

			await AssertMeasurements(new FlowDocument(section),
				documentBox,
				documentBox,
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, lineHeight, 200, documentHeight - lineHeight));
		}

		[TestMethod]
		public async Task Layout_NestedSectionAndNestedSection()
		{
			var section = new Section();

			section.Blocks.Add(new Section());
			section.Blocks.Add(new Section(new Section()));

			await AssertMeasurements(new FlowDocument(section),
				documentBox,
				documentBox,
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, lineHeight, 200, documentHeight - lineHeight),
				new Rect(0, lineHeight, 200, documentHeight - lineHeight));
		}

		[TestMethod]
		public async Task Layout_NestedParagraphAndNestedSectionAndSection()
		{
			var section = new Section();

			section.Blocks.Add(new Paragraph());
			section.Blocks.Add(new Section(new Section()));
			section.Blocks.Add(new Section());

			var displacement = documentHeight - lineHeight;

			await AssertMeasurements(new FlowDocument(section),
				documentBox,
				documentBox,
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, lineHeight, 200, displacement / 2),
				new Rect(0, lineHeight, 200, displacement / 2),
				new Rect(0, lineHeight + displacement / 2, 200, displacement / 2));
		}

		[TestMethod]
		public async Task Layout_NestedParagraphAndSectionAndNestedSection()
		{
			var section = new Section();

			section.Blocks.Add(new Paragraph());
			section.Blocks.Add(new Section());
			section.Blocks.Add(new Section(new Section()));

			var displacement = documentHeight - lineHeight;

			await AssertMeasurements(new FlowDocument(section),
				documentBox,
				documentBox,
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, lineHeight, 200, displacement / 2),
				new Rect(0, lineHeight + displacement / 2, 200, displacement / 2),
				new Rect(0, lineHeight + displacement / 2, 200, displacement / 2));
		}

		[TestMethod]
		public async Task Layout_NestedNestedSectionAndSectionAndParagraph()
		{
			var section = new Section();

			section.Blocks.Add(new Section(new Section()));
			section.Blocks.Add(new Section());
			section.Blocks.Add(new Paragraph());

			var displacement = lineHeight * 3;

			await AssertMeasurements(new FlowDocument(section),
				documentBox,
				documentBox,
				new Rect(0, 0, 200, displacement / 2),
				new Rect(0, 0, 200, displacement / 2),
				new Rect(0, displacement / 2, 200, displacement / 2),
				new Rect(0, displacement, 200, documentHeight - displacement));
		}

		[TestMethod]
		public async Task Layout_NestedSectionAndNestedSectionAndParagraph()
		{
			var section = new Section();

			section.Blocks.Add(new Section());
			section.Blocks.Add(new Section(new Section()));
			section.Blocks.Add(new Paragraph());

			var displacement = lineHeight * 3;

			await AssertMeasurements(new FlowDocument(section),
				documentBox,
				documentBox,
				new Rect(0, 0, 200, displacement / 2),
				new Rect(0, displacement / 2, 200, displacement / 2),
				new Rect(0, displacement / 2, 200, displacement / 2),
				new Rect(0, displacement, 200, documentHeight - displacement));
		}

		[TestMethod]
		public async Task Layout_NestedParagraphAndNestedSectionAndSectionAndParagraph()
		{
			var section = new Section();

			section.Blocks.Add(new Paragraph());
			section.Blocks.Add(new Section(new Section()));
			section.Blocks.Add(new Section());
			section.Blocks.Add(new Paragraph());

			var displacement = sectionDisplacementBetweenParagraphs + lineHeight;

			await AssertMeasurements(new FlowDocument(section),
				documentBox,
				documentBox,
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, lineHeight, 200, displacement / 2),
				new Rect(0, lineHeight, 200, displacement / 2),
				new Rect(0, lineHeight + displacement / 2, 200, displacement / 2),
				new Rect(0, lineHeight + displacement, 200, documentHeight - displacement - lineHeight));
		}

		[TestMethod]
		public async Task Layout_NestedParagraphAndSectionAndNestedSectionAndParagraph()
		{
			var section = new Section();

			section.Blocks.Add(new Paragraph());
			section.Blocks.Add(new Section());
			section.Blocks.Add(new Section(new Section()));
			section.Blocks.Add(new Paragraph());

			var displacement = sectionDisplacementBetweenParagraphs + lineHeight;

			await AssertMeasurements(new FlowDocument(section),
				documentBox,
				documentBox,
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, lineHeight, 200, displacement / 2),
				new Rect(0, lineHeight + displacement / 2, 200, displacement / 2),
				new Rect(0, lineHeight + displacement / 2, 200, displacement / 2),
				new Rect(0, lineHeight + displacement, 200, documentHeight - displacement - lineHeight));
		}
		#endregion

		#region Two Nested Sections
		[TestMethod]
		public async Task Layout_NestedNestedSectionAndNestedSection()
		{
			var section = new Section();

			section.Blocks.Add(new Section(new Section()));
			section.Blocks.Add(new Section(new Section()));

			await AssertMeasurements(new FlowDocument(section),
				documentBox,
				documentBox,
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, lineHeight, 200, documentHeight - lineHeight),
				new Rect(0, lineHeight, 200, documentHeight - lineHeight));
		}

		[TestMethod]
		public async Task Layout_NestedParagraphAndNestedSectionAndNestedSection()
		{
			var section = new Section();

			section.Blocks.Add(new Paragraph());
			section.Blocks.Add(new Section(new Section()));
			section.Blocks.Add(new Section(new Section()));

			var displacement = documentHeight - lineHeight;

			await AssertMeasurements(new FlowDocument(section),
				documentBox,
				documentBox,
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, lineHeight, 200, displacement / 2),
				new Rect(0, lineHeight, 200, displacement / 2),
				new Rect(0, lineHeight + displacement / 2, 200, displacement / 2),
				new Rect(0, lineHeight + displacement / 2, 200, displacement / 2));
		}

		[TestMethod]
		public async Task Layout_NestedNestedSectionAndNestedSectionAndParagraph()
		{
			var section = new Section();

			section.Blocks.Add(new Section(new Section()));
			section.Blocks.Add(new Section(new Section()));
			section.Blocks.Add(new Paragraph());

			var displacement = lineHeight * 3;

			await AssertMeasurements(new FlowDocument(section),
				documentBox,
				documentBox,
				new Rect(0, 0, 200, displacement / 2),
				new Rect(0, 0, 200, displacement / 2),
				new Rect(0, displacement / 2, 200, displacement / 2),
				new Rect(0, displacement / 2, 200, displacement / 2),
				new Rect(0, displacement, 200, documentHeight - displacement));
		}

		[TestMethod]
		public async Task Layout_NestedParagraphAndNestedSectionAndNestedSectionAndParagraph()
		{
			var section = new Section();

			section.Blocks.Add(new Paragraph());
			section.Blocks.Add(new Section(new Section()));
			section.Blocks.Add(new Section(new Section()));
			section.Blocks.Add(new Paragraph());

			var displacement = sectionDisplacementBetweenParagraphs + lineHeight;

			await AssertMeasurements(new FlowDocument(section),
				documentBox,
				documentBox,
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, lineHeight, 200, displacement / 2),
				new Rect(0, lineHeight, 200, displacement / 2),
				new Rect(0, lineHeight + displacement / 2, 200, displacement / 2),
				new Rect(0, lineHeight + displacement / 2, 200, displacement / 2),
				new Rect(0, lineHeight + displacement, 200, documentHeight - displacement - lineHeight));
		}
		#endregion
	}
}