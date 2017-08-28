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
		public async Task Layout_NestedNestedParagraphAndSection()
		{
			var outerSection = new Section();

			outerSection.Blocks.Add(new Section(new Paragraph()));
			outerSection.Blocks.Add(new Section());

			await AssertMeasurements(new FlowDocument(outerSection),
				documentBox,
				documentBox,
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, lineHeight, 200, documentHeight - lineHeight));
		}

		[TestMethod]
		public async Task Layout_NestedSectionAndNestedParagraph()
		{
			var outerSection = new Section();

			outerSection.Blocks.Add(new Section());
			outerSection.Blocks.Add(new Section(new Paragraph()));

			var displacement = lineHeight * 2;

			await AssertMeasurements(new FlowDocument(outerSection),
				documentBox,
				documentBox,
				new Rect(0, 0, 200, displacement),
				new Rect(0, displacement, 200, documentHeight - displacement),
				new Rect(0, displacement, 200, documentHeight - displacement));
		}

		[TestMethod]
		public async Task Layout_NestedNestedParagraphBeforeSectionAndParagraph()
		{
			var outerSection = new Section();
			var section = new Section();

			section.Blocks.Add(new Paragraph());
			section.Blocks.Add(new Section());

			outerSection.Blocks.Add(section);
			outerSection.Blocks.Add(new Paragraph());

			var displacement = sectionDisplacementBetweenParagraphs;

			await AssertMeasurements(new FlowDocument(outerSection),
				documentBox,
				documentBox,
				new Rect(0, 0, 200, lineHeight + displacement),
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, lineHeight, 200, displacement),
				new Rect(0, lineHeight + displacement, 200, documentHeight - displacement - lineHeight));
		}

		[TestMethod]
		public async Task Layout_NestedParagraphAndNestedSectionBeforeParagraph()
		{
			var outerSection = new Section();

			outerSection.Blocks.Add(new Paragraph());

			var section = new Section();

			section.Blocks.Add(new Section());
			section.Blocks.Add(new Paragraph());

			outerSection.Blocks.Add(section);

			var displacement = sectionDisplacementBetweenParagraphs;

			await AssertMeasurements(new FlowDocument(outerSection),
				documentBox,
				documentBox,
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, lineHeight, 200, documentHeight - lineHeight),
				new Rect(0, lineHeight, 200, displacement),
				new Rect(0, lineHeight + displacement, 200, documentHeight - displacement - lineHeight));
		}
		#endregion

		#region Two Childless Sections
		[TestMethod]
		public async Task Layout_NestedNestedParagraphAndSectionAndSection()
		{
			var outerSection = new Section();

			outerSection.Blocks.Add(new Section(new Paragraph()));
			outerSection.Blocks.Add(new Section());
			outerSection.Blocks.Add(new Section());

			var displacement = documentHeight - lineHeight;

			await AssertMeasurements(new FlowDocument(outerSection),
				documentBox,
				documentBox,
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, lineHeight, 200, displacement / 2),
				new Rect(0, lineHeight + displacement / 2, 200, displacement / 2));
		}

		[TestMethod]
		public async Task Layout_NestedSectionAndSectionAndNestedParagraph()
		{
			var outerSection = new Section();

			outerSection.Blocks.Add(new Section());
			outerSection.Blocks.Add(new Section());
			outerSection.Blocks.Add(new Section(new Paragraph()));

			var displacement = lineHeight * 3;

			await AssertMeasurements(new FlowDocument(outerSection),
				documentBox,
				documentBox,
				new Rect(0, 0, 200, displacement / 2),
				new Rect(0, displacement / 2, 200, displacement / 2),
				new Rect(0, displacement, 200, documentHeight - displacement),
				new Rect(0, displacement, 200, documentHeight - displacement));
		}

		[TestMethod]
		public async Task Layout_NestedNestedParagraphBeforeSectionAndSection()
		{
			var outerSection = new Section();
			var section = new Section();

			section.Blocks.Add(new Paragraph());
			section.Blocks.Add(new Section());

			outerSection.Blocks.Add(section);
			outerSection.Blocks.Add(new Section());

			var displacement = documentHeight - lineHeight;

			await AssertMeasurements(new FlowDocument(outerSection),
				documentBox,
				documentBox,
				new Rect(0, 0, 200, lineHeight + displacement / 2),
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, lineHeight, 200, displacement / 2),
				new Rect(0, lineHeight + displacement / 2, 200, displacement / 2));
		}

		[TestMethod]
		public async Task Layout_NestedSectionAndNestedSectionBeforeParagraph()
		{
			var outerSection = new Section();

			outerSection.Blocks.Add(new Section());

			var section = new Section();

			section.Blocks.Add(new Section());
			section.Blocks.Add(new Paragraph());

			outerSection.Blocks.Add(section);

			var displacement = lineHeight * 3;

			await AssertMeasurements(new FlowDocument(outerSection),
				documentBox,
				documentBox,
				new Rect(0, 0, 200, displacement / 2),
				new Rect(0, displacement / 2, 200, documentHeight - displacement / 2),
				new Rect(0, displacement / 2, 200, displacement / 2),
				new Rect(0, displacement, 200, documentHeight - displacement));
		}

		[TestMethod]
		public async Task Layout_NestedNestedParagraphBeforeSectionAndSectionAndParagraph()
		{
			var outerSection = new Section();
			var section = new Section();

			section.Blocks.Add(new Paragraph());
			section.Blocks.Add(new Section());

			outerSection.Blocks.Add(section);
			outerSection.Blocks.Add(new Section());
			outerSection.Blocks.Add(new Paragraph());

			var displacement = sectionDisplacementBetweenParagraphs + lineHeight;

			await AssertMeasurements(new FlowDocument(outerSection),
				documentBox,
				documentBox,
				new Rect(0, 0, 200, lineHeight + displacement / 2),
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, lineHeight, 200, displacement / 2),
				new Rect(0, lineHeight + displacement / 2, 200, displacement / 2),
				new Rect(0, lineHeight + displacement, 200, documentHeight - displacement - lineHeight));
		}

		[TestMethod]
		public async Task Layout_NestedParagraphAndSectionAndNestedSectionBeforeParagraph()
		{
			var outerSection = new Section();

			outerSection.Blocks.Add(new Paragraph());
			outerSection.Blocks.Add(new Section());

			var section = new Section();

			section.Blocks.Add(new Section());
			section.Blocks.Add(new Paragraph());

			outerSection.Blocks.Add(section);

			var displacement = sectionDisplacementBetweenParagraphs + lineHeight;

			await AssertMeasurements(new FlowDocument(outerSection),
				documentBox,
				documentBox,
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, lineHeight, 200, displacement / 2),
				new Rect(0, lineHeight + displacement / 2, 200, documentHeight - lineHeight - displacement / 2),
				new Rect(0, lineHeight + displacement / 2, 200, displacement / 2),
				new Rect(0, lineHeight + displacement, 200, documentHeight - displacement - lineHeight));
		}

		[TestMethod]
		public async Task Layout_NestedNestedParagraphAndSectionAndSectionAndParagraph()
		{
			var outerSection = new Section();

			outerSection.Blocks.Add(new Section(new Paragraph()));
			outerSection.Blocks.Add(new Section());
			outerSection.Blocks.Add(new Section());
			outerSection.Blocks.Add(new Paragraph());

			var displacement = sectionDisplacementBetweenParagraphs + lineHeight;

			await AssertMeasurements(new FlowDocument(outerSection),
				documentBox,
				documentBox,
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, lineHeight, 200, displacement / 2),
				new Rect(0, lineHeight + displacement / 2, 200, displacement / 2),
				new Rect(0, lineHeight + displacement, 200, documentHeight - displacement - lineHeight));
		}

		[TestMethod]
		public async Task Layout_NestedParagraphAndSectionAndSectionAndNestedParagraph()
		{
			var outerSection = new Section();

			outerSection.Blocks.Add(new Paragraph());
			outerSection.Blocks.Add(new Section());
			outerSection.Blocks.Add(new Section());

			var section = new Section();

			section.Blocks.Add(new Paragraph());

			outerSection.Blocks.Add(section);

			var displacement = sectionDisplacementBetweenParagraphs + lineHeight;

			await AssertMeasurements(new FlowDocument(outerSection),
				documentBox,
				documentBox,
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, lineHeight, 200, displacement / 2),
				new Rect(0, lineHeight + displacement / 2, 200, displacement / 2),
				new Rect(0, lineHeight + displacement, 200, documentHeight - displacement - lineHeight),
				new Rect(0, lineHeight + displacement, 200, documentHeight - displacement - lineHeight));
		}

		[TestMethod]
		public async Task Layout_NestedParagraphAndNestedSectionBeforeSectionAndParagraph()
		{
			var outerSection = new Section();

			outerSection.Blocks.Add(new Paragraph());

			var section = new Section();

			section.Blocks.Add(new Section());
			section.Blocks.Add(new Section());

			outerSection.Blocks.Add(section);
			outerSection.Blocks.Add(new Paragraph());

			var displacement = sectionDisplacementBetweenParagraphs + lineHeight;

			await AssertMeasurements(new FlowDocument(outerSection),
				documentBox,
				documentBox,
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, lineHeight, 200, displacement),
				new Rect(0, lineHeight, 200, displacement / 2),
				new Rect(0, lineHeight + displacement / 2, 200, displacement / 2),
				new Rect(0, lineHeight + displacement, 200, documentHeight - displacement - lineHeight));
		}
		#endregion

		#region Single Nested Section
		[TestMethod]
		public async Task Layout_NestedNestedParagraphAndNestedSection()
		{
			var outerSection = new Section();

			outerSection.Blocks.Add(new Section(new Paragraph()));
			outerSection.Blocks.Add(new Section(new Section()));

			var displacement = documentHeight - lineHeight;

			await AssertMeasurements(new FlowDocument(outerSection),
				documentBox,
				documentBox,
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, lineHeight, 200, displacement),
				new Rect(0, lineHeight, 200, displacement));
		}

		[TestMethod]
		public async Task Layout_NestedNestedSectionAndNestedParagraph()
		{
			var outerSection = new Section();

			outerSection.Blocks.Add(new Section(new Section()));
			outerSection.Blocks.Add(new Section(new Paragraph()));

			var displacement = lineHeight * 2;

			await AssertMeasurements(new FlowDocument(outerSection),
				documentBox,
				documentBox,
				new Rect(0, 0, 200, displacement),
				new Rect(0, 0, 200, displacement),
				new Rect(0, displacement, 200, documentHeight - displacement),
				new Rect(0, displacement, 200, documentHeight - displacement));
		}

		[TestMethod]
		public async Task Layout_NestedNestedNestedParagraphAndNestedSection()
		{
			var outerSection = new Section();

			outerSection.Blocks.Add(new Section(new Section(new Paragraph())));
			outerSection.Blocks.Add(new Section(new Section()));

			var displacement = documentHeight - lineHeight;

			await AssertMeasurements(new FlowDocument(outerSection),
				documentBox,
				documentBox,
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, lineHeight, 200, displacement),
				new Rect(0, lineHeight, 200, displacement));
		}

		[TestMethod]
		public async Task Layout_NestedNestedSectionAndNestedNestedParagraph()
		{
			var outerSection = new Section();

			outerSection.Blocks.Add(new Section(new Section()));
			outerSection.Blocks.Add(new Section(new Section(new Paragraph())));

			var displacement = lineHeight * 2;

			await AssertMeasurements(new FlowDocument(outerSection),
				documentBox,
				documentBox,
				new Rect(0, 0, 200, displacement),
				new Rect(0, 0, 200, displacement),
				new Rect(0, displacement, 200, documentHeight - displacement),
				new Rect(0, displacement, 200, documentHeight - displacement),
				new Rect(0, displacement, 200, documentHeight - displacement));
		}

		[TestMethod]
		public async Task Layout_NestedNestedParagraphAndNestedSectionAndParagraph()
		{
			var outerSection = new Section();

			outerSection.Blocks.Add(new Section(new Paragraph()));
			outerSection.Blocks.Add(new Section(new Section()));
			outerSection.Blocks.Add(new Paragraph());

			var displacement = sectionDisplacementBetweenParagraphs;

			await AssertMeasurements(new FlowDocument(outerSection),
				documentBox,
				documentBox,
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, lineHeight, 200, displacement),
				new Rect(0, lineHeight, 200, displacement),
				new Rect(0, lineHeight + displacement, 200, documentHeight - displacement - lineHeight));
		}

		[TestMethod]
		public async Task Layout_NestedParagraphAndNestedSectionAndNestedParagraph()
		{
			var outerSection = new Section();

			outerSection.Blocks.Add(new Paragraph());
			outerSection.Blocks.Add(new Section(new Section()));
			outerSection.Blocks.Add(new Section(new Paragraph()));

			var displacement = sectionDisplacementBetweenParagraphs;

			await AssertMeasurements(new FlowDocument(outerSection),
				documentBox,
				documentBox,
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, lineHeight, 200, displacement),
				new Rect(0, lineHeight, 200, displacement),
				new Rect(0, lineHeight + displacement, 200, documentHeight - displacement - lineHeight),
				new Rect(0, lineHeight + displacement, 200, documentHeight - displacement - lineHeight));
		}
		#endregion

		#region Two Sections; One Childless and One Nested
		[TestMethod]
		public async Task Layout_NestedNestedParagraphAndNestedSectionAndSection()
		{
			var outerSection = new Section();

			outerSection.Blocks.Add(new Section(new Paragraph()));
			outerSection.Blocks.Add(new Section(new Section()));
			outerSection.Blocks.Add(new Section());

			var displacement = documentHeight - lineHeight;

			await AssertMeasurements(new FlowDocument(outerSection),
				documentBox,
				documentBox,
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, lineHeight, 200, displacement / 2),
				new Rect(0, lineHeight, 200, displacement / 2),
				new Rect(0, lineHeight + displacement / 2, 200, displacement / 2));
		}

		[TestMethod]
		public async Task Layout_NestedNestedParagraphAndSectionAndNestedSection()
		{
			var outerSection = new Section();

			outerSection.Blocks.Add(new Section(new Paragraph()));
			outerSection.Blocks.Add(new Section());
			outerSection.Blocks.Add(new Section(new Section()));

			var displacement = documentHeight - lineHeight;

			await AssertMeasurements(new FlowDocument(outerSection),
				documentBox,
				documentBox,
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, lineHeight, 200, displacement / 2),
				new Rect(0, lineHeight + displacement / 2, 200, displacement / 2),
				new Rect(0, lineHeight + displacement / 2, 200, displacement / 2));
		}

		[TestMethod]
		public async Task Layout_NestedNestedSectionAndSectionAndNestedParagraph()
		{
			var outerSection = new Section();

			outerSection.Blocks.Add(new Section(new Section()));
			outerSection.Blocks.Add(new Section());
			outerSection.Blocks.Add(new Section(new Paragraph()));

			var displacement = lineHeight * 3;

			await AssertMeasurements(new FlowDocument(outerSection),
				documentBox,
				documentBox,
				new Rect(0, 0, 200, displacement / 2),
				new Rect(0, 0, 200, displacement / 2),
				new Rect(0, displacement / 2, 200, displacement / 2),
				new Rect(0, displacement, 200, documentHeight - displacement),
				new Rect(0, displacement, 200, documentHeight - displacement));
		}

		[TestMethod]
		public async Task Layout_NestedSectionAndNestedSectionAndNestedParagraph()
		{
			var outerSection = new Section();

			outerSection.Blocks.Add(new Section());
			outerSection.Blocks.Add(new Section(new Section()));
			outerSection.Blocks.Add(new Section(new Paragraph()));

			var displacement = lineHeight * 3;

			await AssertMeasurements(new FlowDocument(outerSection),
				documentBox,
				documentBox,
				new Rect(0, 0, 200, displacement / 2),
				new Rect(0, displacement / 2, 200, displacement / 2),
				new Rect(0, displacement / 2, 200, displacement / 2),
				new Rect(0, displacement, 200, documentHeight - displacement),
				new Rect(0, displacement, 200, documentHeight - displacement));
		}

		[TestMethod]
		public async Task Layout_NestedNestedParagraphAndNestedSectionAndSectionAndParagraph()
		{
			var outerSection = new Section();

			outerSection.Blocks.Add(new Section(new Paragraph()));
			outerSection.Blocks.Add(new Section(new Section()));
			outerSection.Blocks.Add(new Section());
			outerSection.Blocks.Add(new Paragraph());

			var displacement = sectionDisplacementBetweenParagraphs + lineHeight;

			await AssertMeasurements(new FlowDocument(outerSection),
				documentBox,
				documentBox,
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, lineHeight, 200, displacement / 2),
				new Rect(0, lineHeight, 200, displacement / 2),
				new Rect(0, lineHeight + displacement / 2, 200, displacement / 2),
				new Rect(0, lineHeight + displacement, 200, documentHeight - displacement - lineHeight));
		}

		[TestMethod]
		public async Task Layout_NestedNestedParagraphAndSectionAndNestedSectionAndParagraph()
		{
			var outerSection = new Section();

			outerSection.Blocks.Add(new Section(new Paragraph()));
			outerSection.Blocks.Add(new Section());
			outerSection.Blocks.Add(new Section(new Section()));
			outerSection.Blocks.Add(new Paragraph());

			var displacement = sectionDisplacementBetweenParagraphs + lineHeight;

			await AssertMeasurements(new FlowDocument(outerSection),
				documentBox,
				documentBox,
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, lineHeight, 200, displacement / 2),
				new Rect(0, lineHeight + displacement / 2, 200, displacement / 2),
				new Rect(0, lineHeight + displacement / 2, 200, displacement / 2),
				new Rect(0, lineHeight + displacement, 200, documentHeight - displacement - lineHeight));
		}

		[TestMethod]
		public async Task Layout_NestedParagraphAndNestedSectionAndSectionAndNestedParagraph()
		{
			var outerSection = new Section();

			outerSection.Blocks.Add(new Paragraph());
			outerSection.Blocks.Add(new Section(new Section()));
			outerSection.Blocks.Add(new Section());
			outerSection.Blocks.Add(new Section(new Paragraph()));

			var displacement = sectionDisplacementBetweenParagraphs + lineHeight;

			await AssertMeasurements(new FlowDocument(outerSection),
				documentBox,
				documentBox,
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, lineHeight, 200, displacement / 2),
				new Rect(0, lineHeight, 200, displacement / 2),
				new Rect(0, lineHeight + displacement / 2, 200, displacement / 2),
				new Rect(0, lineHeight + displacement, 200, documentHeight - displacement - lineHeight),
				new Rect(0, lineHeight + displacement, 200, documentHeight - displacement - lineHeight));
		}

		[TestMethod]
		public async Task Layout_NestedParagraphAndSectionAndNestedSectionAndNestedParagraph()
		{
			var outerSection = new Section();

			outerSection.Blocks.Add(new Paragraph());
			outerSection.Blocks.Add(new Section());
			outerSection.Blocks.Add(new Section(new Section()));
			outerSection.Blocks.Add(new Section(new Paragraph()));

			var displacement = sectionDisplacementBetweenParagraphs + lineHeight;

			await AssertMeasurements(new FlowDocument(outerSection),
				documentBox,
				documentBox,
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, lineHeight, 200, displacement / 2),
				new Rect(0, lineHeight + displacement / 2, 200, displacement / 2),
				new Rect(0, lineHeight + displacement / 2, 200, displacement / 2),
				new Rect(0, lineHeight + displacement, 200, documentHeight - displacement - lineHeight),
				new Rect(0, lineHeight + displacement, 200, documentHeight - displacement - lineHeight));
		}
		#endregion

		#region Two Nested Sections
		[TestMethod]
		public async Task Layout_NestedNestedParagraphAndNestedSectionAndNestedSection()
		{
			var outerSection = new Section();

			outerSection.Blocks.Add(new Section(new Paragraph()));
			outerSection.Blocks.Add(new Section(new Section()));
			outerSection.Blocks.Add(new Section(new Section()));

			var displacement = documentHeight - lineHeight;

			await AssertMeasurements(new FlowDocument(outerSection),
				documentBox,
				documentBox,
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, lineHeight, 200, displacement / 2),
				new Rect(0, lineHeight, 200, displacement / 2),
				new Rect(0, lineHeight + displacement / 2, 200, displacement / 2),
				new Rect(0, lineHeight + displacement / 2, 200, displacement / 2));
		}

		[TestMethod]
		public async Task Layout_NestedNestedSectionAndNestedSectionAndNestedParagraph()
		{
			var outerSection = new Section();

			outerSection.Blocks.Add(new Section(new Section()));
			outerSection.Blocks.Add(new Section(new Section()));
			outerSection.Blocks.Add(new Section(new Paragraph()));

			var displacement = lineHeight * 3;

			await AssertMeasurements(new FlowDocument(outerSection),
				documentBox,
				documentBox,
				new Rect(0, 0, 200, displacement / 2),
				new Rect(0, 0, 200, displacement / 2),
				new Rect(0, displacement / 2, 200, displacement / 2),
				new Rect(0, displacement / 2, 200, displacement / 2),
				new Rect(0, displacement, 200, documentHeight - displacement),
				new Rect(0, displacement, 200, documentHeight - displacement));
		}

		[TestMethod]
		public async Task Layout_NestedNestedParagraphAndNestedSectionAndNestedSectionAndNestedParagraph()
		{
			var outerSection = new Section();

			outerSection.Blocks.Add(new Section(new Paragraph()));
			outerSection.Blocks.Add(new Section(new Section()));
			outerSection.Blocks.Add(new Section(new Section()));
			outerSection.Blocks.Add(new Section(new Paragraph()));

			var displacement = sectionDisplacementBetweenParagraphs + lineHeight;

			await AssertMeasurements(new FlowDocument(outerSection),
				documentBox,
				documentBox,
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, lineHeight, 200, displacement / 2),
				new Rect(0, lineHeight, 200, displacement / 2),
				new Rect(0, lineHeight + displacement / 2, 200, displacement / 2),
				new Rect(0, lineHeight + displacement / 2, 200, displacement / 2),
				new Rect(0, lineHeight + displacement, 200, documentHeight - displacement - lineHeight),
				new Rect(0, lineHeight + displacement, 200, documentHeight - displacement - lineHeight));
		}
		#endregion

		#region Complex Nesting
		[TestMethod]
		public async Task Layout_NestedParagraphAndNestedSectionAndNestedSectionBeforeParagraph()
		{
			var outerSection = new Section();

			outerSection.Blocks.Add(new Paragraph());
			outerSection.Blocks.Add(new Section(new Section()));

			var section = new Section();

			section.Blocks.Add(new Section());
			section.Blocks.Add(new Paragraph());

			outerSection.Blocks.Add(section);

			var displacement = sectionDisplacementBetweenParagraphs + lineHeight;

			await AssertMeasurements(new FlowDocument(outerSection),
				documentBox,
				documentBox,
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, lineHeight, 200, displacement / 2),
				new Rect(0, lineHeight, 200, displacement / 2),
				new Rect(0, lineHeight + displacement / 2, 200, documentHeight - lineHeight - displacement / 2),
				new Rect(0, lineHeight + displacement / 2, 200, displacement / 2),
				new Rect(0, lineHeight + displacement, 200, documentHeight - displacement - lineHeight));
		}
		#endregion
	}
}