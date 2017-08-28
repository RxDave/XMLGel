using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DaveSexton.XmlGel.UnitTests.Maml
{
	[TestClass]
	public class Topics : MamlTests
	{
		[TestMethod]
		public void Maml_Topics_alert()
		{
			TestRoundTrip(MamlGuide.alert);
		}

		[TestMethod]
		public void Maml_Topics_application()
		{
			TestRoundTrip(MamlGuide.application);
		}

		[TestMethod]
		public void Maml_Topics_autoOutline()
		{
			TestRoundTrip(MamlGuide.autoOutline);
		}

		[TestMethod]
		public void Maml_Topics_autoOutlineExample()
		{
			TestRoundTrip(MamlGuide.autoOutlineExample);
		}

		[TestMethod]
		public void Maml_Topics_BlockElements()
		{
			TestRoundTrip(MamlGuide.BlockElements);
		}

		[TestMethod]
		public void Maml_Topics_code()
		{
			TestRoundTrip(MamlGuide.code);
		}

		[TestMethod]
		public void Maml_Topics_codeEntityReference()
		{
			TestRoundTrip(MamlGuide.codeEntityReference);
		}

		[TestMethod]
		public void Maml_Topics_codeInline()
		{
			TestRoundTrip(MamlGuide.codeInline);
		}

		[TestMethod]
		public void Maml_Topics_codeReference()
		{
			TestRoundTrip(MamlGuide.codeReference);
		}

		[TestMethod]
		public void Maml_Topics_CodeSnippetsFile()
		{
			TestRoundTrip(MamlGuide.CodeSnippetsFile);
		}

		[TestMethod]
		public void Maml_Topics_command()
		{
			TestRoundTrip(MamlGuide.command);
		}

		[TestMethod]
		public void Maml_Topics_CommonInline()
		{
			TestRoundTrip(MamlGuide.CommonInline);
		}

		[TestMethod]
		public void Maml_Topics_CompanionFile()
		{
			TestRoundTrip(MamlGuide.CompanionFile);
		}

		[TestMethod]
		public void Maml_Topics_computeOutputInline()
		{
			TestRoundTrip(MamlGuide.computeOutputInline);
		}

		[TestMethod]
		public void Maml_Topics_Conceptual()
		{
			TestRoundTrip(MamlGuide.Conceptual);
		}

		[TestMethod]
		public void Maml_Topics_database()
		{
			TestRoundTrip(MamlGuide.database);
		}

		[TestMethod]
		public void Maml_Topics_definitionTable()
		{
			TestRoundTrip(MamlGuide.definitionTable);
		}

		[TestMethod]
		public void Maml_Topics_environmentVariable()
		{
			TestRoundTrip(MamlGuide.environmentVariable);
		}

		[TestMethod]
		public void Maml_Topics_Error_Message()
		{
			TestRoundTrip(MamlGuide.Error_Message);
		}

		[TestMethod]
		public void Maml_Topics_errorInline()
		{
			TestRoundTrip(MamlGuide.errorInline);
		}

		[TestMethod]
		public void Maml_Topics_externalLink()
		{
			TestRoundTrip(MamlGuide.externalLink);
		}

		[TestMethod]
		public void Maml_Topics_fictiousUri()
		{
			TestRoundTrip(MamlGuide.fictiousUri);
		}

		[TestMethod]
		public void Maml_Topics_FileTypes()
		{
			TestRoundTrip(MamlGuide.FileTypes);
		}

		[TestMethod]
		public void Maml_Topics_foreignPhrase()
		{
			TestRoundTrip(MamlGuide.foreignPhrase);
		}

		[TestMethod]
		public void Maml_Topics_Glossary()
		{
			TestRoundTrip(MamlGuide.Glossary);
		}

		[TestMethod]
		public void Maml_Topics_Glossary_TopicFile()
		{
			TestRoundTrip(MamlGuide.Glossary_TopicFile);
		}

		[TestMethod]
		public void Maml_Topics_hardware()
		{
			TestRoundTrip(MamlGuide.hardware);
		}

		[TestMethod]
		public void Maml_Topics_How_To()
		{
			TestRoundTrip(MamlGuide.How_To);
		}

		[TestMethod]
		public void Maml_Topics_IntelliSense()
		{
			TestRoundTrip(MamlGuide.IntelliSense);
		}

		[TestMethod]
		public void Maml_Topics_introduction()
		{
			TestRoundTrip(MamlGuide.introduction);
		}

		[TestMethod]
		public void Maml_Topics_KnownIssues()
		{
			TestRoundTrip(MamlGuide.KnownIssues);
		}

		[TestMethod]
		public void Maml_Topics_legacyBold()
		{
			TestRoundTrip(MamlGuide.legacyBold);
		}

		[TestMethod]
		public void Maml_Topics_legacyItalic()
		{
			TestRoundTrip(MamlGuide.legacyItalic);
		}

		[TestMethod]
		public void Maml_Topics_legacyUnderline()
		{
			TestRoundTrip(MamlGuide.legacyUnderline);
		}

		[TestMethod]
		public void Maml_Topics_Link()
		{
			TestRoundTrip(MamlGuide.Link);
		}

		[TestMethod]
		public void Maml_Topics_LinkElements()
		{
			TestRoundTrip(MamlGuide.LinkElements);
		}

		[TestMethod]
		public void Maml_Topics_Links()
		{
			TestRoundTrip(MamlGuide.Links);
		}

		[TestMethod]
		public void Maml_Topics_list()
		{
			TestRoundTrip(MamlGuide.list);
		}

		[TestMethod]
		public void Maml_Topics_literal()
		{
			TestRoundTrip(MamlGuide.literal);
		}

		[TestMethod]
		public void Maml_Topics_localUri()
		{
			TestRoundTrip(MamlGuide.localUri);
		}

		[TestMethod]
		public void Maml_Topics_markup()
		{
			TestRoundTrip(MamlGuide.markup);
		}

		[TestMethod]
		public void Maml_Topics_math()
		{
			TestRoundTrip(MamlGuide.math);
		}

		[TestMethod]
		public void Maml_Topics_MediaContentFile()
		{
			TestRoundTrip(MamlGuide.MediaContentFile);
		}

		[TestMethod]
		public void Maml_Topics_MediaElements()
		{
			TestRoundTrip(MamlGuide.MediaElements);
		}

		[TestMethod]
		public void Maml_Topics_mediaLink()
		{
			TestRoundTrip(MamlGuide.mediaLink);
		}

		[TestMethod]
		public void Maml_Topics_mediaLinkInline()
		{
			TestRoundTrip(MamlGuide.mediaLinkInline);
		}

		[TestMethod]
		public void Maml_Topics_MiscElements()
		{
			TestRoundTrip(MamlGuide.MiscElements);
		}

		[TestMethod]
		public void Maml_Topics_MSHelpViewerRoot()
		{
			TestRoundTrip(MamlGuide.MSHelpViewerRoot);
		}

		[TestMethod]
		public void Maml_Topics_newTerm()
		{
			TestRoundTrip(MamlGuide.newTerm);
		}

		[TestMethod]
		public void Maml_Topics_Orientation()
		{
			TestRoundTrip(MamlGuide.Orientation);
		}

		[TestMethod]
		public void Maml_Topics_para()
		{
			TestRoundTrip(MamlGuide.para);
		}

		[TestMethod]
		public void Maml_Topics_phrase()
		{
			TestRoundTrip(MamlGuide.phrase);
		}

		[TestMethod]
		public void Maml_Topics_placeholder()
		{
			TestRoundTrip(MamlGuide.placeholder);
		}

		[TestMethod]
		public void Maml_Topics_quote()
		{
			TestRoundTrip(MamlGuide.quote);
		}

		[TestMethod]
		public void Maml_Topics_quoteInline()
		{
			TestRoundTrip(MamlGuide.quoteInline);
		}

		[TestMethod]
		public void Maml_Topics_Reference()
		{
			TestRoundTrip(MamlGuide.Reference);
		}

		[TestMethod]
		public void Maml_Topics_Reference_With_Syntax()
		{
			TestRoundTrip(MamlGuide.Reference_With_Syntax);
		}

		[TestMethod]
		public void Maml_Topics_Reference_Without_Syntax()
		{
			TestRoundTrip(MamlGuide.Reference_Without_Syntax);
		}

		[TestMethod]
		public void Maml_Topics_relatedTopics()
		{
			TestRoundTrip(MamlGuide.relatedTopics);
		}

		[TestMethod]
		public void Maml_Topics_replaceable()
		{
			TestRoundTrip(MamlGuide.replaceable);
		}

		[TestMethod]
		public void Maml_Topics_Sample()
		{
			TestRoundTrip(MamlGuide.Sample);
		}

		[TestMethod]
		public void Maml_Topics_SchemaExtensibility()
		{
			TestRoundTrip(MamlGuide.SchemaExtensibility);
		}

		[TestMethod]
		public void Maml_Topics_SDK_Technology_Architecture()
		{
			TestRoundTrip(MamlGuide.SDK_Technology_Architecture);
		}

		[TestMethod]
		public void Maml_Topics_SDK_Technology_Code_Directory()
		{
			TestRoundTrip(MamlGuide.SDK_Technology_Code_Directory);
		}

		[TestMethod]
		public void Maml_Topics_SDK_Technology_Orientation()
		{
			TestRoundTrip(MamlGuide.SDK_Technology_Orientation);
		}

		[TestMethod]
		public void Maml_Topics_SDK_Technology_Scenarios()
		{
			TestRoundTrip(MamlGuide.SDK_Technology_Scenarios);
		}

		[TestMethod]
		public void Maml_Topics_SDK_Technology_Summary()
		{
			TestRoundTrip(MamlGuide.SDK_Technology_Summary);
		}

		[TestMethod]
		public void Maml_Topics_subscript()
		{
			TestRoundTrip(MamlGuide.subscript);
		}

		[TestMethod]
		public void Maml_Topics_superscript()
		{
			TestRoundTrip(MamlGuide.superscript);
		}

		[TestMethod]
		public void Maml_Topics_system()
		{
			TestRoundTrip(MamlGuide.system);
		}

		[TestMethod]
		public void Maml_Topics_table()
		{
			TestRoundTrip(MamlGuide.table);
		}

		[TestMethod]
		public void Maml_Topics_title()
		{
			TestRoundTrip(MamlGuide.title);
		}

		[TestMethod]
		public void Maml_Topics_Token()
		{
			TestRoundTrip(MamlGuide.Token);
		}

		[TestMethod]
		public void Maml_Topics_TokenFile()
		{
			TestRoundTrip(MamlGuide.TokenFile);
		}

		[TestMethod]
		public void Maml_Topics_TopicFile()
		{
			TestRoundTrip(MamlGuide.TopicFile);
		}

		[TestMethod]
		public void Maml_Topics_Troubleshooting()
		{
			TestRoundTrip(MamlGuide.Troubleshooting);
		}

		[TestMethod]
		public void Maml_Topics_ui()
		{
			TestRoundTrip(MamlGuide.ui);
		}

		[TestMethod]
		public void Maml_Topics_unmanagedCodeEntityReference()
		{
			TestRoundTrip(MamlGuide.unmanagedCodeEntityReference);
		}

		[TestMethod]
		public void Maml_Topics_User_Interface_Reference()
		{
			TestRoundTrip(MamlGuide.User_Interface_Reference);
		}

		[TestMethod]
		public void Maml_Topics_userInput()
		{
			TestRoundTrip(MamlGuide.userInput);
		}

		[TestMethod]
		public void Maml_Topics_v1_0_0_()
		{
			TestRoundTrip(MamlGuide.v1_0_0_0);
		}

		[TestMethod]
		public void Maml_Topics_v1_1_0_0()
		{
			TestRoundTrip(MamlGuide.v1_1_0_0);
		}

		[TestMethod]
		public void Maml_Topics_v1_1_0_1()
		{
			TestRoundTrip(MamlGuide.v1_1_0_1);
		}

		[TestMethod]
		public void Maml_Topics_v1_1_0_2()
		{
			TestRoundTrip(MamlGuide.v1_1_0_2);
		}

		[TestMethod]
		public void Maml_Topics_VersionHistory()
		{
			TestRoundTrip(MamlGuide.VersionHistory);
		}

		[TestMethod]
		public void Maml_Topics_Walkthrough()
		{
			TestRoundTrip(MamlGuide.Walkthrough);
		}

		[TestMethod]
		public void Maml_Topics_Welcome()
		{
			TestRoundTrip(MamlGuide.Welcome);
		}

		[TestMethod]
		public void Maml_Topics_Whitepaper()
		{
			TestRoundTrip(MamlGuide.Whitepaper);
		}

		[TestMethod]
		public void Maml_Topics_XML_Reference()
		{
			TestRoundTrip(MamlGuide.XML_Reference);
		}
	}
}