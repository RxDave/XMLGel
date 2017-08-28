using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DaveSexton.XmlGel.UnitTests.Maml
{
	[TestClass]
	public class MamlGuideTests : BaseTests
	{
		[TestMethod]
		public void Maml_MamlGuide_Glossary()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\MamlGuide\Glossary.aml"));
		}

		[TestMethod]
		public void Maml_MamlGuide_IntelliSense()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\MamlGuide\IntelliSense.aml"));
		}

		[TestMethod]
		public void Maml_MamlGuide_KnownIssues()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\MamlGuide\KnownIssues.aml"));
		}

		[TestMethod]
		public void Maml_MamlGuide_Links()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\MamlGuide\Links.aml"));
		}

		[TestMethod]
		public void Maml_MamlGuide_MSHelpViewerRoot()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\MamlGuide\MSHelpViewerRoot.aml"));
		}

		[TestMethod]
		public void Maml_MamlGuide_SchemaExtensibility()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\MamlGuide\SchemaExtensibility.aml"),
				expectedInvalidNodeCount: 17);
		}

		[TestMethod]
		public void Maml_MamlGuide_Welcome()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\MamlGuide\Welcome.aml"));
		}

		[TestMethod]
		public void Maml_MamlGuide_BlockElements_alert()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\MamlGuide\BlockElements\alert.aml"));
		}

		[TestMethod]
		public void Maml_MamlGuide_BlockElements_BlockElements()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\MamlGuide\BlockElements\BlockElements.aml"));
		}

		[TestMethod]
		public void Maml_MamlGuide_BlockElements_code()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\MamlGuide\BlockElements\code.aml"));
		}

		[TestMethod]
		public void Maml_MamlGuide_BlockElements_codeReference()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\MamlGuide\BlockElements\codeReference.aml"));
		}

		[TestMethod]
		public void Maml_MamlGuide_BlockElements_definitionTable()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\MamlGuide\BlockElements\definitionTable.aml"));
		}

		[TestMethod]
		public void Maml_MamlGuide_BlockElements_introduction()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\MamlGuide\BlockElements\introduction.aml"));
		}

		[TestMethod]
		public void Maml_MamlGuide_BlockElements_list()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\MamlGuide\BlockElements\list.aml"));
		}

		[TestMethod]
		public void Maml_MamlGuide_BlockElements_para()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\MamlGuide\BlockElements\para.aml"));
		}

		[TestMethod]
		public void Maml_MamlGuide_BlockElements_quote()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\MamlGuide\BlockElements\quote.aml"));
		}

		[TestMethod]
		public void Maml_MamlGuide_BlockElements_relatedTopics()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\MamlGuide\BlockElements\relatedTopics.aml"));
		}

		[TestMethod]
		public void Maml_MamlGuide_BlockElements_table()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\MamlGuide\BlockElements\table.aml"));
		}

		[TestMethod]
		public void Maml_MamlGuide_BlockElements_title()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\MamlGuide\BlockElements\title.aml"));
		}

		[TestMethod]
		public void Maml_MamlGuide_CommonInline_application()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\MamlGuide\CommonInline\application.aml"));
		}

		[TestMethod]
		public void Maml_MamlGuide_CommonInline_codeInline()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\MamlGuide\CommonInline\codeInline.aml"));
		}

		[TestMethod]
		public void Maml_MamlGuide_CommonInline_command()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\MamlGuide\CommonInline\command.aml"));
		}

		[TestMethod]
		public void Maml_MamlGuide_CommonInline_CommonInline()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\MamlGuide\CommonInline\CommonInline.aml"));
		}

		[TestMethod]
		public void Maml_MamlGuide_CommonInline_computeOutputInline()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\MamlGuide\CommonInline\computeOutputInline.aml"));
		}

		[TestMethod]
		public void Maml_MamlGuide_CommonInline_database()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\MamlGuide\CommonInline\database.aml"));
		}

		[TestMethod]
		public void Maml_MamlGuide_CommonInline_environmentVariable()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\MamlGuide\CommonInline\environmentVariable.aml"));
		}

		[TestMethod]
		public void Maml_MamlGuide_CommonInline_errorInline()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\MamlGuide\CommonInline\errorInline.aml"));
		}

		[TestMethod]
		public void Maml_MamlGuide_CommonInline_fictiousUri()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\MamlGuide\CommonInline\fictiousUri.aml"));
		}

		[TestMethod]
		public void Maml_MamlGuide_CommonInline_foreignPhrase()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\MamlGuide\CommonInline\foreignPhrase.aml"));
		}

		[TestMethod]
		public void Maml_MamlGuide_CommonInline_hardware()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\MamlGuide\CommonInline\hardware.aml"));
		}

		[TestMethod]
		public void Maml_MamlGuide_CommonInline_legacyBold()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\MamlGuide\CommonInline\legacyBold.aml"));
		}

		[TestMethod]
		public void Maml_MamlGuide_CommonInline_legacyItalic()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\MamlGuide\CommonInline\legacyItalic.aml"));
		}

		[TestMethod]
		public void Maml_MamlGuide_CommonInline_legacyUnderline()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\MamlGuide\CommonInline\legacyUnderline.aml"));
		}

		[TestMethod]
		public void Maml_MamlGuide_CommonInline_literal()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\MamlGuide\CommonInline\literal.aml"));
		}

		[TestMethod]
		public void Maml_MamlGuide_CommonInline_localUri()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\MamlGuide\CommonInline\localUri.aml"));
		}

		[TestMethod]
		public void Maml_MamlGuide_CommonInline_math()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\MamlGuide\CommonInline\math.aml"));
		}

		[TestMethod]
		public void Maml_MamlGuide_CommonInline_newTerm()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\MamlGuide\CommonInline\newTerm.aml"));
		}

		[TestMethod]
		public void Maml_MamlGuide_CommonInline_phrase()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\MamlGuide\CommonInline\phrase.aml"));
		}

		[TestMethod]
		public void Maml_MamlGuide_CommonInline_placeholder()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\MamlGuide\CommonInline\placeholder.aml"));
		}

		[TestMethod]
		public void Maml_MamlGuide_CommonInline_quoteInline()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\MamlGuide\CommonInline\quoteInline.aml"));
		}

		[TestMethod]
		public void Maml_MamlGuide_CommonInline_replaceable()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\MamlGuide\CommonInline\replaceable.aml"));
		}

		[TestMethod]
		public void Maml_MamlGuide_CommonInline_subscript()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\MamlGuide\CommonInline\subscript.aml"));
		}

		[TestMethod]
		public void Maml_MamlGuide_CommonInline_superscript()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\MamlGuide\CommonInline\superscript.aml"));
		}

		[TestMethod]
		public void Maml_MamlGuide_CommonInline_system()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\MamlGuide\CommonInline\system.aml"));
		}

		[TestMethod]
		public void Maml_MamlGuide_CommonInline_ui()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\MamlGuide\CommonInline\ui.aml"));
		}

		[TestMethod]
		public void Maml_MamlGuide_CommonInline_unmanagedCodeEntityReference()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\MamlGuide\CommonInline\unmanagedCodeEntityReference.aml"));
		}

		[TestMethod]
		public void Maml_MamlGuide_CommonInline_userInput()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\MamlGuide\CommonInline\userInput.aml"));
		}

		[TestMethod]
		public void Maml_MamlGuide_FileTypes_CodeSnippetsFile()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\MamlGuide\FileTypes\CodeSnippetsFile.aml"));
		}

		[TestMethod]
		public void Maml_MamlGuide_FileTypes_CompanionFile()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\MamlGuide\FileTypes\CompanionFile.aml"));
		}

		[TestMethod]
		public void Maml_MamlGuide_FileTypes_FileTypes()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\MamlGuide\FileTypes\FileTypes.aml"));
		}

		[TestMethod]
		public void Maml_MamlGuide_FileTypes_MediaContentFile()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\MamlGuide\FileTypes\MediaContentFile.aml"));
		}

		[TestMethod]
		public void Maml_MamlGuide_FileTypes_TokenFile()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\MamlGuide\FileTypes\TokenFile.aml"));
		}

		[TestMethod]
		public void Maml_MamlGuide_FileTypes_TopicFiles_Conceptual()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\MamlGuide\FileTypes\TopicFiles\Conceptual.aml"));
		}

		[TestMethod]
		public void Maml_MamlGuide_FileTypes_TopicFiles_Error_Message()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\MamlGuide\FileTypes\TopicFiles\Error Message.aml"));
		}

		[TestMethod]
		public void Maml_MamlGuide_FileTypes_TopicFiles_Glossary()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\MamlGuide\FileTypes\TopicFiles\Glossary.aml"));
		}

		[TestMethod]
		public void Maml_MamlGuide_FileTypes_TopicFiles_How_To()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\MamlGuide\FileTypes\TopicFiles\How To.aml"));
		}

		[TestMethod]
		public void Maml_MamlGuide_FileTypes_TopicFiles_Orientation()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\MamlGuide\FileTypes\TopicFiles\Orientation.aml"));
		}

		[TestMethod]
		public void Maml_MamlGuide_FileTypes_TopicFiles_Reference_With_Syntax()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\MamlGuide\FileTypes\TopicFiles\Reference With Syntax.aml"));
		}

		[TestMethod]
		public void Maml_MamlGuide_FileTypes_TopicFiles_Reference_Without_Syntax()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\MamlGuide\FileTypes\TopicFiles\Reference Without Syntax.aml"));
		}

		[TestMethod]
		public void Maml_MamlGuide_FileTypes_TopicFiles_Reference()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\MamlGuide\FileTypes\TopicFiles\Reference.aml"));
		}

		[TestMethod]
		public void Maml_MamlGuide_FileTypes_TopicFiles_Sample()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\MamlGuide\FileTypes\TopicFiles\Sample.aml"));
		}

		[TestMethod]
		public void Maml_MamlGuide_FileTypes_TopicFiles_SDK_Technology_Architecture()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\MamlGuide\FileTypes\TopicFiles\SDK Technology Architecture.aml"));
		}

		[TestMethod]
		public void Maml_MamlGuide_FileTypes_TopicFiles_SDK_Technology_Code_Directory()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\MamlGuide\FileTypes\TopicFiles\SDK Technology Code Directory.aml"));
		}

		[TestMethod]
		public void Maml_MamlGuide_FileTypes_TopicFiles_SDK_Technology_Orientation()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\MamlGuide\FileTypes\TopicFiles\SDK Technology Orientation.aml"));
		}

		[TestMethod]
		public void Maml_MamlGuide_FileTypes_TopicFiles_SDK_Technology_Scenarios()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\MamlGuide\FileTypes\TopicFiles\SDK Technology Scenarios.aml"));
		}

		[TestMethod]
		public void Maml_MamlGuide_FileTypes_TopicFiles_SDK_Technology_Summary()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\MamlGuide\FileTypes\TopicFiles\SDK Technology Summary.aml"));
		}

		[TestMethod]
		public void Maml_MamlGuide_FileTypes_TopicFiles_TopicFile()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\MamlGuide\FileTypes\TopicFiles\TopicFile.aml"));
		}

		[TestMethod]
		public void Maml_MamlGuide_FileTypes_TopicFiles_Troubleshooting()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\MamlGuide\FileTypes\TopicFiles\Troubleshooting.aml"));
		}

		[TestMethod]
		public void Maml_MamlGuide_FileTypes_TopicFiles_User_Interface_Reference()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\MamlGuide\FileTypes\TopicFiles\User Interface Reference.aml"));
		}

		[TestMethod]
		public void Maml_MamlGuide_FileTypes_TopicFiles_Walkthrough()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\MamlGuide\FileTypes\TopicFiles\Walkthrough.aml"));
		}

		[TestMethod]
		public void Maml_MamlGuide_FileTypes_TopicFiles_Whitepaper()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\MamlGuide\FileTypes\TopicFiles\Whitepaper.aml"));
		}

		[TestMethod]
		public void Maml_MamlGuide_FileTypes_TopicFiles_XML_Reference()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\MamlGuide\FileTypes\TopicFiles\XML Reference.aml"));
		}

		[TestMethod]
		public void Maml_MamlGuide_LinkElements_codeEntityReference()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\MamlGuide\LinkElements\codeEntityReference.aml"));
		}

		[TestMethod]
		public void Maml_MamlGuide_LinkElements_externalLink()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\MamlGuide\LinkElements\externalLink.aml"));
		}

		[TestMethod]
		public void Maml_MamlGuide_LinkElements_Link()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\MamlGuide\LinkElements\Link.aml"));
		}

		[TestMethod]
		public void Maml_MamlGuide_LinkElements_LinkElements()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\MamlGuide\LinkElements\LinkElements.aml"));
		}

		[TestMethod]
		public void Maml_MamlGuide_MediaElements_MediaElements()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\MamlGuide\MediaElements\MediaElements.aml"));
		}

		[TestMethod]
		public void Maml_MamlGuide_MediaElements_mediaLink()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\MamlGuide\MediaElements\mediaLink.aml"));
		}

		[TestMethod]
		public void Maml_MamlGuide_MediaElements_mediaLinkInline()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\MamlGuide\MediaElements\mediaLinkInline.aml"));
		}

		[TestMethod]
		public void Maml_MamlGuide_MiscElements_autoOutline()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\MamlGuide\MiscElements\autoOutline.aml"));
		}

		[TestMethod]
		public void Maml_MamlGuide_MiscElements_autoOutlineExample()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\MamlGuide\MiscElements\autoOutlineExample.aml"));
		}

		[TestMethod]
		public void Maml_MamlGuide_MiscElements_markup()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\MamlGuide\MiscElements\markup.aml"),
				expectedInvalidNodeCount: 5);
		}

		[TestMethod]
		public void Maml_MamlGuide_MiscElements_MiscElements()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\MamlGuide\MiscElements\MiscElements.aml"));
		}

		[TestMethod]
		public void Maml_MamlGuide_MiscElements_Token()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\MamlGuide\MiscElements\Token.aml"));
		}

		[TestMethod]
		public void Maml_MamlGuide_VersionHistory_v1_0_0_0()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\MamlGuide\VersionHistory\v1.0.0.0.aml"));
		}

		[TestMethod]
		public void Maml_MamlGuide_VersionHistory_v1_1_0_0()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\MamlGuide\VersionHistory\v1.1.0.0.aml"));
		}

		[TestMethod]
		public void Maml_MamlGuide_VersionHistory_v1_1_0_1()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\MamlGuide\VersionHistory\v1.1.0.1.aml"));
		}

		[TestMethod]
		public void Maml_MamlGuide_VersionHistory_v1_1_0_2()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\MamlGuide\VersionHistory\v1.1.0.2.aml"));
		}

		[TestMethod]
		public void Maml_MamlGuide_VersionHistory_VersionHistory()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\MamlGuide\VersionHistory\VersionHistory.aml"));
		}

	}
}
