using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DaveSexton.XmlGel.UnitTests.Maml
{
	[TestClass]
	public class SampleTopicsTests : BaseTests
	{
		[TestMethod]
		public void Maml_SampleTopics_AIP_About_Spam()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\SampleTopics\AIP About Spam.aml"));
		}

		[TestMethod]
		public void Maml_SampleTopics_AIP_Getting_Assistance()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\SampleTopics\AIP Getting Assistance.aml"));
		}

		[TestMethod]
		public void Maml_SampleTopics_AIP_Getting_Started()
		{
			TestRoundTrip(
				topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\SampleTopics\AIP Getting Started.aml"), 
				expected: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\SampleTopics\AIP Getting Started-Expected.aml"));
		}

		[TestMethod]
		public void Maml_SampleTopics_AIP_Glossary()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\SampleTopics\AIP Glossary.aml"));
		}

		[TestMethod]
		public void Maml_SampleTopics_AIP_Introduction()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\SampleTopics\AIP Introduction.aml"));
		}

		[TestMethod]
		public void Maml_SampleTopics_Fruits_and_Veggies_Glossary()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\SampleTopics\Fruits and Veggies Glossary.aml"));
		}

		[TestMethod]
		public void Maml_SampleTopics_How_To_Bibliography()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\SampleTopics\How To Bibliography.aml"));
		}

		[TestMethod]
		public void Maml_SampleTopics_How_To_Linking()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\SampleTopics\How To Linking.aml"));
		}

		[TestMethod]
		public void Maml_SampleTopics_How_To_Media()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\SampleTopics\How To Media.aml"));
		}

		[TestMethod]
		public void Maml_SampleTopics_How_To_Snippets()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\SampleTopics\How To Snippets.aml"));
		}

		[TestMethod]
		public void Maml_SampleTopics_How_To_Tokens()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\SampleTopics\How To Tokens.aml"));
		}

		[TestMethod]
		public void Maml_SampleTopics_saved()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\SampleTopics\saved.aml"));
		}

	}
}
