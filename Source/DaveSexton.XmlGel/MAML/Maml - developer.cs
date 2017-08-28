using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Linq;
using DaveSexton.XmlGel.Maml.Documents;

namespace DaveSexton.XmlGel.Maml
{
	partial class Maml
	{
		// Nested elements
		public static readonly XName Prerequisites = Namespace + "prerequisites";
		public static readonly XName NextSteps = Namespace + "nextSteps";
		public static readonly XName InThisSection = Namespace + "inThisSection";
		public static readonly XName Reference = Namespace + "reference";
		public static readonly XName RelatedSections = Namespace + "relatedSections";
		public static readonly XName Requirements = Namespace + "requirements";
		public static readonly XName Demonstrates = Namespace + "demonstrates";
		public static readonly XName TaskList = Namespace + "taskList";
		public static readonly XName UIElementList = Namespace + "uiElementList";
		public static readonly XName Byline = Namespace + "byline";
		public static readonly XName SummarySection = Namespace + "summarySection";
		public static readonly XName DownloadCenterLink = Namespace + "downloadCenterLink";
		public static readonly XName AppliesTo = Namespace + "appliesTo";
		public static readonly XName Contents = Namespace + "contents";
		public static readonly XName AdditionalInformation = Namespace + "additionalInformation";
		public static readonly XName Type = Namespace + "type";
		public static readonly XName AuthorBio = Namespace + "authorBio";
		public static readonly XName TextValue = Namespace + "textValue";
		public static readonly XName CodeList = Namespace + "codeList";
		public static readonly XName Keywords = Namespace + "keywords";
		public static readonly XName Namespaces = Namespace + "namespaces";
		public static readonly XName RelatedTechnologies = Namespace + "relatedTechnologies";
		public static readonly XName Background = Namespace + "background";
		public static readonly XName ImplementingTechnologyNameClasses = Namespace + "implementingTechnologyNameClasses";
		public static readonly XName TechnologyNameClassesAtAGlance = Namespace + "technologyNameClassesAtAGlance";
		public static readonly XName WhatsNew = Namespace + "whatsNew";
		public static readonly XName LanguageReferenceRemarks = Namespace + "languageReferenceRemarks";
		public static readonly XName Parameters = Namespace + "parameters";
		public static readonly XName ReturnValue = Namespace + "returnValue";
		public static readonly XName Exceptions = Namespace + "exceptions";
		public static readonly XName DotNetFrameworkEquivalent = Namespace + "dotNetFrameworkEquivalent";

		// Document elements
		public static readonly XName CodeEntityDocument = Namespace + "codeEntityDocument";
		public static readonly XName DeveloperHowToDocument = Namespace + "developerHowToDocument";
		public static readonly XName DeveloperConceptualDocument = Namespace + "developerConceptualDocument";
		public static readonly XName DeveloperWalkthroughDocument = Namespace + "developerWalkthroughDocument";
		public static readonly XName DeveloperOrientationDocument = Namespace + "developerOrientationDocument";
		public static readonly XName DeveloperSampleDocument = Namespace + "developerSampleDocument";
		public static readonly XName DeveloperTroubleshootingDocument = Namespace + "developerTroubleshootingDocument";
		public static readonly XName DeveloperReferenceWithoutSyntaxDocument = Namespace + "developerReferenceWithoutSyntaxDocument";
		public static readonly XName DeveloperReferenceWithSyntaxDocument = Namespace + "developerReferenceWithSyntaxDocument";
		public static readonly XName DeveloperErrorMessageDocument = Namespace + "developerErrorMessageDocument";
		public static readonly XName DeveloperUIReferenceDocument = Namespace + "developerUIReferenceDocument";
		public static readonly XName DeveloperSdkTechnologyOverviewOrientationDocument = Namespace + "developerSDKTechnologyOverviewOrientationDocument";
		public static readonly XName DeveloperSdkTechnologyOverviewTechnologySummaryDocument = Namespace + "developerSDKTechnologyOverviewTechnologySummaryDocument";
		public static readonly XName DeveloperSdkTechnologyOverviewArchitectureDocument = Namespace + "developerSDKTechnologyOverviewArchitectureDocument";
		public static readonly XName DeveloperSdkTechnologyOverviewScenariosDocument = Namespace + "developerSDKTechnologyOverviewScenariosDocument";
		public static readonly XName DeveloperSdkTechnologyOverviewCodeDirectoryDocument = Namespace + "developerSDKTechnologyOverviewCodeDirectoryDocument";
		public static readonly XName DeveloperWhitePaperDocument = Namespace + "developerWhitePaperDocument";
		public static readonly XName DeveloperGlossaryDocument = Namespace + "developerGlossaryDocument";
		public static readonly XName DeveloperXmlReference = Namespace + "developerXmlReference";

		private static readonly Dictionary<MamlDocumentKind, XElement> defaultDocuments = new Dictionary<MamlDocumentKind, XElement>()
		{
			{ MamlDocumentKind.Reference, XElement.Parse(Templates.Reference, LoadOptions.SetLineInfo) }, 
			{ MamlDocumentKind.HowTo, XElement.Parse(Templates.HowTo, LoadOptions.SetLineInfo) }, 
			{ MamlDocumentKind.Conceptual, XElement.Parse(Templates.Conceptual, LoadOptions.SetLineInfo) }, 
			{ MamlDocumentKind.Walkthrough, XElement.Parse(Templates.Walkthrough, LoadOptions.SetLineInfo) },
			{ MamlDocumentKind.Orientation, XElement.Parse(Templates.Orientation, LoadOptions.SetLineInfo) }, 
			{ MamlDocumentKind.Sample, XElement.Parse(Templates.Sample, LoadOptions.SetLineInfo) }, 
			{ MamlDocumentKind.Troubleshooting, XElement.Parse(Templates.Troubleshooting, LoadOptions.SetLineInfo) }, 
			{ MamlDocumentKind.ReferenceWithoutSyntax, XElement.Parse(Templates.ReferenceWithoutSyntax, LoadOptions.SetLineInfo) }, 
			{ MamlDocumentKind.ReferenceWithSyntax, XElement.Parse(Templates.ReferenceWithSyntax, LoadOptions.SetLineInfo) }, 
			{ MamlDocumentKind.ErrorMessage, XElement.Parse(Templates.ErrorMessage, LoadOptions.SetLineInfo) }, 
			{ MamlDocumentKind.UserInterface, XElement.Parse(Templates.UserInterface, LoadOptions.SetLineInfo) }, 
			{ MamlDocumentKind.SdkOrientation, XElement.Parse(Templates.SdkOrientation, LoadOptions.SetLineInfo) }, 
			{ MamlDocumentKind.SdkSummary, XElement.Parse(Templates.SdkSummary, LoadOptions.SetLineInfo) }, 
			{ MamlDocumentKind.SdkArchitecture, XElement.Parse(Templates.SdkArchitecture, LoadOptions.SetLineInfo) }, 
			{ MamlDocumentKind.SdkScenarios, XElement.Parse(Templates.SdkScenarios, LoadOptions.SetLineInfo) }, 
			{ MamlDocumentKind.SdkCodeDirectory, XElement.Parse(Templates.SdkCodeDirectory, LoadOptions.SetLineInfo) }, 
			{ MamlDocumentKind.Whitepaper, XElement.Parse(Templates.Whitepaper, LoadOptions.SetLineInfo) }, 
			{ MamlDocumentKind.Glossary, XElement.Parse(Templates.Glossary, LoadOptions.SetLineInfo) }, 
			{ MamlDocumentKind.XmlReference, XElement.Parse(Templates.XmlReference, LoadOptions.SetLineInfo) }
		};

		private static readonly Dictionary<XName, Func<Guid, int, XDocument, XElement, IMamlMetadata, MamlDocument>> mamlDocumentFactory = new Dictionary<XName, Func<Guid, int, XDocument, XElement, IMamlMetadata, MamlDocument>>()
		{
			{ CodeEntityDocument, (id, revisionNumber, document, documentRoot, metadata) => new CodeEntityDocument(id, revisionNumber, document, documentRoot, metadata) },
			{ DeveloperHowToDocument, (id, revisionNumber, document, documentRoot, metadata) => new HowToDocument(id, revisionNumber, document, documentRoot, metadata) },
			{ DeveloperConceptualDocument, (id, revisionNumber, document, documentRoot, metadata) => new ConceptualDocument(id, revisionNumber, document, documentRoot, metadata) },
			{ DeveloperWalkthroughDocument, (id, revisionNumber, document, documentRoot, metadata) => new WalkthroughDocument(id, revisionNumber, document, documentRoot, metadata) },
			{ DeveloperOrientationDocument, (id, revisionNumber, document, documentRoot, metadata) => new OrientationDocument(id, revisionNumber, document, documentRoot, metadata) },
			{ DeveloperSampleDocument, (id, revisionNumber, document, documentRoot, metadata) => new SampleDocument(id, revisionNumber, document, documentRoot, metadata) },
			{ DeveloperTroubleshootingDocument, (id, revisionNumber, document, documentRoot, metadata) => new TroubleshootingDocument(id, revisionNumber, document, documentRoot, metadata) },
			{ DeveloperReferenceWithoutSyntaxDocument, (id, revisionNumber, document, documentRoot, metadata) => new ReferenceWithoutSyntaxDocument(id, revisionNumber, document, documentRoot, metadata) },
			{ DeveloperReferenceWithSyntaxDocument, (id, revisionNumber, document, documentRoot, metadata) => new ReferenceWithSyntaxDocument(id, revisionNumber, document, documentRoot, metadata) },
			{ DeveloperErrorMessageDocument, (id, revisionNumber, document, documentRoot, metadata) => new ErrorMessageDocument(id, revisionNumber, document, documentRoot, metadata) },
			{ DeveloperUIReferenceDocument, (id, revisionNumber, document, documentRoot, metadata) => new UIReferenceDocument(id, revisionNumber, document, documentRoot, metadata) },
			{ DeveloperSdkTechnologyOverviewOrientationDocument, (id, revisionNumber, document, documentRoot, metadata) => new SdkOrientationDocument(id, revisionNumber, document, documentRoot, metadata) },
			{ DeveloperSdkTechnologyOverviewTechnologySummaryDocument, (id, revisionNumber, document, documentRoot, metadata) => new SdkSummaryDocument(id, revisionNumber, document, documentRoot, metadata) },
			{ DeveloperSdkTechnologyOverviewArchitectureDocument, (id, revisionNumber, document, documentRoot, metadata) => new SdkArchitectureDocument(id, revisionNumber, document, documentRoot, metadata) },
			{ DeveloperSdkTechnologyOverviewScenariosDocument, (id, revisionNumber, document, documentRoot, metadata) => new SdkScenariosDocument(id, revisionNumber, document, documentRoot, metadata) },
			{ DeveloperSdkTechnologyOverviewCodeDirectoryDocument, (id, revisionNumber, document, documentRoot, metadata) => new SdkCodeDirectoryDocument(id, revisionNumber, document, documentRoot, metadata) },
			{ DeveloperWhitePaperDocument, (id, revisionNumber, document, documentRoot, metadata) => new WhitePaperDocument(id, revisionNumber, document, documentRoot, metadata) },
			{ DeveloperGlossaryDocument, (id, revisionNumber, document, documentRoot, metadata) => new GlossaryDocument(id, revisionNumber, document, documentRoot, metadata) },
			{ DeveloperXmlReference, (id, revisionNumber, document, documentRoot, metadata) => new XmlReferenceDocument(id, revisionNumber, document, documentRoot, metadata) }
		};

		public static bool IsDocumentName(XName name)
		{
			return mamlDocumentFactory.ContainsKey(name);
		}

		internal static MamlDocument CreateDocument(XName name, Guid id, int revisionNumber, XDocument document, XElement documentRoot, IMamlMetadata metadata)
		{
			Func<Guid, int, XDocument, XElement, IMamlMetadata, MamlDocument> factory;

			if (mamlDocumentFactory.TryGetValue(name, out factory))
			{
				return factory(id, revisionNumber, document, documentRoot, metadata);
			}

			return null;
		}

		internal static MamlDocument CreateDocument(MamlDocumentKind kind)
		{
			switch (kind)
			{
				case MamlDocumentKind.Reference:
					return new CodeEntityDocument();
				case MamlDocumentKind.HowTo:
					return new HowToDocument();
				case MamlDocumentKind.Conceptual:
					return new ConceptualDocument();
				case MamlDocumentKind.Walkthrough:
					return new WalkthroughDocument();
				case MamlDocumentKind.Orientation:
					return new OrientationDocument();
				case MamlDocumentKind.Sample:
					return new SampleDocument();
				case MamlDocumentKind.Troubleshooting:
					return new TroubleshootingDocument();
				case MamlDocumentKind.ReferenceWithoutSyntax:
					return new ReferenceWithoutSyntaxDocument();
				case MamlDocumentKind.ReferenceWithSyntax:
					return new ReferenceWithSyntaxDocument();
				case MamlDocumentKind.ErrorMessage:
					return new ErrorMessageDocument();
				case MamlDocumentKind.UserInterface:
					return new UIReferenceDocument();
				case MamlDocumentKind.SdkOrientation:
					return new SdkOrientationDocument();
				case MamlDocumentKind.SdkSummary:
					return new SdkSummaryDocument();
				case MamlDocumentKind.SdkArchitecture:
					return new SdkArchitectureDocument();
				case MamlDocumentKind.SdkScenarios:
					return new SdkScenariosDocument();
				case MamlDocumentKind.SdkCodeDirectory:
					return new SdkCodeDirectoryDocument();
				case MamlDocumentKind.Whitepaper:
					return new WhitePaperDocument();
				case MamlDocumentKind.Glossary:
					return new GlossaryDocument();
				case MamlDocumentKind.XmlReference:
					return new XmlReferenceDocument();
				default:
					throw new InvalidEnumArgumentException("kind", (int) kind, typeof(MamlDocumentKind));
			}
		}

		internal static XElement GetDefaultDocument(MamlDocumentKind kind)
		{
			return defaultDocuments[kind];
		}

		internal static XDocument CreateTopic(Guid id, int revisionNumber, XElement documentRoot)
		{
			return new XDocument(
				new XElement(
					Maml.TopicRoot,
					new XAttribute(Maml.TopicId, id),
					new XAttribute(Maml.TopicRevisionNumber, revisionNumber),
					documentRoot));
		}
	}
}