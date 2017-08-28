using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DaveSexton.XmlGel.UnitTests.Maml
{
	[TestClass]
	public class AutoFixTests : BaseTests
	{
		[TestMethod]
		public void Maml_AutoFix_ConceptualDocument()
		{
			TestRoundTrip(
				topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\AutoFix\ConceptualDocument.aml"), 
				expected: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\AutoFix\ConceptualDocument-Expected.aml"));
		}

		[TestMethod]
		public void Maml_AutoFix_MoveParaIntoMissingContent()
		{
			TestRoundTrip(
				topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\AutoFix\MoveParaIntoMissingContent.aml"), 
				expected: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\AutoFix\MoveParaIntoMissingContent-Expected.aml"));
		}

		[TestMethod]
		public void Maml_AutoFix_SectionTitle()
		{
			TestRoundTrip(
				topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\AutoFix\SectionTitle.aml"), 
				expected: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\AutoFix\SectionTitle-Expected.aml"));
		}

		[TestMethod]
		public void Maml_AutoFix_SectionTitleAndMoveParaIntoMissingContent()
		{
			TestRoundTrip(
				topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\AutoFix\SectionTitleAndMoveParaIntoMissingContent.aml"), 
				expected: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\AutoFix\SectionTitleAndMoveParaIntoMissingContent-Expected.aml"));
		}

		[TestMethod]
		public void Maml_AutoFix_Unfixable_UnexpectedProcedure()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\AutoFix\Unfixable-UnexpectedProcedure.aml"),
				expectedInvalidNodeCount: 1);
		}

	}
}
