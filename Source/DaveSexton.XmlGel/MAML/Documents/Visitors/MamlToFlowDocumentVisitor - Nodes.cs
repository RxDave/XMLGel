using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace DaveSexton.XmlGel.Maml.Documents.Visitors
{
	partial class MamlToFlowDocumentVisitor
	{
		private static readonly IDictionary<XName, Func<MamlToFlowDocumentVisitor, XElement, MamlNode>> nodeFactories = new Dictionary<XName, Func<MamlToFlowDocumentVisitor, XElement, MamlNode>>()
		{
			// baseConditional
			{ Maml.ProgrammingLanguage, (_, element) => new MamlEnum<DevLanguage>(element) }, 
			{ Maml.Platform, (_, element) => new MamlEnum<Platform>(element) }, 
			{ Maml.Platforms, (_, element) => new MamlPlatforms(element) }, 

			// blockCommon
			{ Maml.Para, (_, element) => new MamlPara(element) }, 
			{ Maml.Quote, (_, element) => new MamlQuote(element) }, 
			{ Maml.Title, (_, element) => new MamlTitle(element) }, 
			{ Maml.SecondaryErrorTitle, (_, element) => new MamlSecondaryErrorTitle(element) }, 
			{ Maml.NonLocErrorTitle, (_, element) => new MamlNonLocErrorTitle(element) }, 
			{ Maml.Alert, (_, element) => new MamlAlert(element) }, 
			{ Maml.SharedContentBlock, (_, element) => new MamlSharedContent(element) }, 
			{ Maml.Introduction, (_, element) => new MamlIntroduction(element) }, 
			{ Maml.Description, (_, element) => new MamlNamedSection(element) }, 
			{ Maml.Conclusion, (_, element) => new MamlNamedSection(element) },

			// blockSoftware
			{ Maml.ComputerOutput, (_, element) => new MamlInline(element) }, 
			{ Maml.Code, (_, element) => new MamlCode(element) }, 
			{ Maml.Comment, (_, element) => new MamlString(element) }, 
			{ Maml.CodeReference, (_, element) => new MamlCodeReference(element) }, 

			// conditionSet
			{ Maml.Conditions, (_, element) => new MamlConditions(element) }, 

			// developer
			{ Maml.Prerequisites, (_, element) => new MamlNamedSection(element) }, 
			{ Maml.NextSteps, (_, element) => new MamlNamedSection(element) },
			{ Maml.InThisSection, (_, element) => new MamlNamedSection(element) },
			{ Maml.Reference, (_, element) => new MamlNamedSection(element) },
			{ Maml.RelatedSections, (_, element) => new MamlNamedSection(element) },
			{ Maml.Requirements, (_, element) => new MamlNamedSection(element) },
			{ Maml.Demonstrates, (_, element) => new MamlNamedSection(element) },
			{ Maml.TaskList, (_, element) => new MamlNamedSection(element) },
			{ Maml.UIElementList, (_, element) => new MamlNamedSection(element) },
			{ Maml.Byline, (_, element) => new MamlSingleParagraphSimpleContent(element) },
			{ Maml.SummarySection, (_, element) => new MamlNamedSection(element) },
			{ Maml.DownloadCenterLink, (_, element) => new MamlNamedSection(element) },
			{ Maml.AppliesTo, (_, element) => new MamlNamedSection(element) },
			{ Maml.Contents, (_, element) => new MamlNamedSection(element) },
			{ Maml.AdditionalInformation, (_, element) => new MamlNamedSection(element) },
			{ Maml.Type, (_, element) => new MamlSingleParagraphSimpleContent(element) },
			{ Maml.AuthorBio, (_, element) => new MamlNamedSection(element) },
			{ Maml.TextValue, (_, element) => new MamlNamedSection(element) },
			{ Maml.CodeList, (_, element) => new MamlNamedSection(element) },
			{ Maml.Keywords, (_, element) => new MamlNamedSection(element) },
			{ Maml.Namespaces, (_, element) => new MamlNamedSection(element) },
			{ Maml.RelatedTechnologies, (_, element) => new MamlNamedSection(element) },
			{ Maml.Background, (_, element) => new MamlNamedSection(element) },
			{ Maml.ImplementingTechnologyNameClasses, (_, element) => new MamlSection(element) },
			{ Maml.TechnologyNameClassesAtAGlance, (_, element) => new MamlSection(element) },
			{ Maml.WhatsNew, (_, element) => new MamlNamedSection(element) },
			{ Maml.LanguageReferenceRemarks, (_, element) => new MamlNamedSection(element) },
			{ Maml.DotNetFrameworkEquivalent, (_, element) => new MamlNamedSection(element) },

			// developerStructure
			{ Maml.Summary, (_, element) => new MamlSummary(element) }, 
			{ Maml.Remarks, (_, element) => new MamlNamedSection(element) }, 
			{ Maml.Comments, (_, element) => new MamlNamedSection(element) }, 
			{ Maml.BuildInstructions, (_, element) => new MamlNamedSection(element) }, 
			{ Maml.RobustProgramming, (_, element) => new MamlNamedSection(element) }, 
			{ Maml.Security, (_, element) => new MamlNamedSection(element) }, 
			{ Maml.CodeExample, (_, element) => new MamlCodeExample(element) },
			{ Maml.CodeExamples, (_, element) => new MamlCodeExamples(element) },
			{ Maml.Input, (_, element) => new MamlCodeSection(element) },
			{ Maml.Output, (_, element) => new MamlCodeSection(element) },
			{ Maml.SampleOutput, (_, element) => new MamlCodeSection(element) },
			{ Maml.SyntaxSection, (_, element) => new MamlSyntaxSection(element) }, 
			{ Maml.LegacySyntax, (_, element) => new MamlLegacySyntax(element) }, 
			{ Maml.SchemaHierarchy, (_, element) => new MamlSchemaHierarchy(element) }, 
			{ Maml.Attribute, (_, element) => new MamlAttribute(element) }, 
			{ Maml.Attributes, (_, element) => new MamlAttributes(element) },
			{ Maml.ChildElement, (_, element) => new MamlAttributes(element) },
			{ Maml.ParentElement, (_, element) => new MamlAttributes(element) },
			{ Maml.AttributesAndElements, (_, element) => new MamlAttributesAndElements(element) },
			{ Maml.ElementInformation, (_, element) => new MamlElementInformation(element) },

			// developerReference
			{ Maml.CodeEntities, (_, element) => new MamlCodeEntities(element) },
			{ Maml.CodeEntity, (_, element) => new MamlCodeEntity(element) },
			{ Maml.Assembly, (_, element) => new MamlAssembly(element) },
			{ Maml.AssemblyName, (_, element) => new MamlRestrictedText(element) },
			{ Maml.AssemblyVersion, (_, element) => new MamlRestrictedText(element) },
			{ Maml.AssemblyCulture, (_, element) => new MamlRestrictedText(element) },
			{ Maml.AssemblyPublicKey, (_, element) => new MamlRestrictedText(element) },
			{ Maml.AssemblyHash, (_, element) => new MamlRestrictedText(element) },
			{ Maml.InternalOnly, (_, element) => new MamlEmpty(element) },
			{ Maml.UseBase, (_, element) => new MamlEmpty(element) },
			{ Maml.ClsCompliantAlternative, (_, element) => new MamlCodeReferenceDescription(element) },
			{ Maml.EquivalentCodeEntity, (_, element) => new MamlCodeReferenceDescription(element) },
			{ Maml.ObsoleteCodeEntity, (_, element) => new MamlCodeReferenceDescription(element) },
			{ Maml.UnmanagedCodeEntity, (_, element) => new MamlCodeReferenceDescription(element) },
			{ Maml.GenericParameters, (_, element) => new MamlGenericParameters(element) },
			{ Maml.GenericParameter, (_, element) => new MamlParameterReferenceDescription(element) },
			{ Maml.Parameters, (visitor, element) => visitor.CreateParametersNode(element) },
			{ Maml.Parameter, (_, element) => new MamlParameterReferenceDescription(element) },
			{ Maml.ReturnValue, (visitor, element) => visitor.CreateReturnValueNode(element) },
			{ Maml.Exceptions, (visitor, element) => visitor.CreateExceptionsNode(element) },
			{ Maml.Exception, (_, element) => new MamlCodeReferenceDescription(element) },
			{ Maml.ThreadSafety, (_, element) => new MamlSectionContent(element) },
			{ Maml.NotesForImplementers, (_, element) => new MamlSectionContent(element) },
			{ Maml.NotesForCallers, (_, element) => new MamlSectionContent(element) },
			{ Maml.NotesForInheritors, (_, element) => new MamlSectionContent(element) },
			{ Maml.PlatformNotes, (_, element) => new MamlPlatformNotes(element) },
			{ Maml.PlatformNote, (_, element) => new MamlPlatformNote(element) },
			{ Maml.Permissions, (_, element) => new MamlPermissions(element) },
			{ Maml.Permission, (_, element) => new MamlCodeReferenceDescription(element) },

			// hierarchy
			{ Maml.Section, (_, element) => new MamlSection(element) }, 
			{ Maml.SectionSimple, (_, element) => new MamlSectionSimple(element) }, 
			{ Maml.Sections, (_, element) => new MamlSections(element) }, 
			{ Maml.SampleRef, (_, element) => new MamlSampleRef(element) }, 
			{ Maml.RelatedTopics, (_, element) => new MamlRelatedTopics(element) }, 
			{ Maml.ExternalResources, (_, element) => new MamlExternalResources(element) }, 

			// inlineCommon
			{ Maml.Phrase, (_, element) => new MamlInline(element) }, 
			{ Maml.LegacyBold, (_, element) => new MamlBold(element) }, 
			{ Maml.LegacyItalic, (_, element) => new MamlItalic(element) }, 
			{ Maml.LegacyUnderline, (_, element) => new MamlUnderline(element) }, 
			{ Maml.NotLocalizable, (_, element) => new MamlInline(element) }, 
			{ Maml.LocalizedText, (_, element) => new MamlInline(element) }, 
			{ Maml.EmbeddedLabel, (_, element) => new MamlInline(element) }, 
			{ Maml.QuoteInline, (_, element) => new MamlText(element) }, 
			{ Maml.Date, (visitor, element) => visitor.CreateDateNode(element) }, 
			{ Maml.ForeignPhrase, (_, element) => new MamlText(element) }, 
			{ Maml.Corporation, (_, element) => new MamlText(element) }, 
			{ Maml.Country, (_, element) => new MamlText(element) }, 
			{ Maml.Suffix, (_, element) => new MamlText(element) }, 
			{ Maml.Prefix, (_, element) => new MamlText(element) }, 
			{ Maml.Subscript, (_, element) => new MamlRestrictedText(element) }, 
			{ Maml.Superscript, (_, element) => new MamlRestrictedText(element) }, 
			{ Maml.Trademark, (_, element) => new MamlRestrictedText(element) }, 
			{ Maml.Holder, (_, element) => new MamlRestrictedText(element) }, 
			{ Maml.Separator, (_, element) => new MamlRestrictedText(element) }, 
			{ Maml.Keyword, (_, element) => new MamlRestrictedText(element) }, 
			{ Maml.Year, (_, element) => new MamlInteger(element) }, 
			{ Maml.Copyright, (_, element) => new MamlCopyright(element) }, 
			{ Maml.DynamicLink, (_, element) => new MamlDynamicLink(element) }, 

			// inlineLinking
			{ Maml.ParameterReference, (_, element) => new MamlParameterReference(element) }, 
			{ Maml.LinkText, (_, element) => new MamlExternalLinkText(element) }, 
			{ Maml.LinkAlternateText, (_, element) => new MamlExternalLinkAlternateText(element) }, 
			{ Maml.LinkUri, (_, element) => new MamlExternalLinkUri(element) }, 
			{ Maml.LinkTarget, (_, element) => new MamlExternalLinkTarget(element) }, 
			{ Maml.Link, (_, element) => new MamlLink(element) }, 
			{ Maml.LegacyLink, (_, element) => new MamlLink(element) },
			{ Maml.MediaLinkInline, (_, element) => new MamlMediaLinkInline(element) }, 
			{ Maml.Caption, (_, element) => new MamlString(element) }, 
			{ Maml.Image, (_, element) => new MamlImage(element) }, 
			{ Maml.CodeEntityReference, (_, element) => new MamlCodeEntityReference(element) }, 
			{ Maml.SharedContentParameter, (_, element) => new MamlSharedContentParameter(element) }, 
			{ Maml.SharedContent, (_, element) => new MamlSharedContent(element) }, 
			{ Maml.ExternalLink, (_, element) => new MamlExternalLink(element) }, 

			// inlineSoftware
			{ Maml.Math, (_, element) => new MamlInline(element) }, 
			{ Maml.Application, (_, element) => new MamlText(element) }, 
			{ Maml.Database, (_, element) => new MamlText(element) }, 
			{ Maml.EnvironmentVariable, (_, element) => new MamlText(element) }, 
			{ Maml.ErrorInline, (_, element) => new MamlText(element) }, 
			{ Maml.Hardware, (_, element) => new MamlText(element) }, 
			{ Maml.Literal, (_, element) => new MamlText(element) }, 
			{ Maml.Token, (_, element) => new MamlText(element) }, 
			{ Maml.LanguageKeyword, (_, element) => new MamlText(element) }, 
			{ Maml.ComputerOutputInline, (_, element) => new MamlText(element) }, 
			{ Maml.UI, (_, element) => new MamlText(element) }, 
			{ Maml.System, (_, element) => new MamlText(element) }, 
			{ Maml.Placeholder, (_, element) => new MamlText(element) }, 
			{ Maml.NewTerm, (_, element) => new MamlText(element) }, 
			{ Maml.UnmanagedCodeEntityReference, (_, element) => new MamlText(element) }, 
			{ Maml.CodeFeaturedElement, (_, element) => new MamlText(element) }, 
			{ Maml.Replaceable, (_, element) => new MamlText(element) }, 
			{ Maml.FictitiousUri, (_, element) => new MamlFictitiousUri(element) }, 
			{ Maml.LocalUri, (_, element) => new MamlLocalUri(element) }, 
			{ Maml.Markup, (_, element) => new MamlAnyNode(element) }, 
			{ Maml.Command, (_, element) => new MamlCommand(element) }, 
			{ Maml.CodeInline, (_, element) => new MamlCodeInline(element) }, 

			// inlineUi
			{ Maml.UserInput, (_, element) => new MamlRestrictedText(element) }, 
			{ Maml.UserInputLocalizable, (_, element) => new MamlRestrictedText(element) }, 

			// structure
			{ Maml.AutoOutline, (_, element) => new MamlAutoOutline(element) },
			{ Maml.DefinedTerm, (_, element) => new MamlDefinedTerm(element) }, 
			{ Maml.Content, (_, element) => new MamlContent(element) }, 
			{ Maml.Definition, (_, element) => new MamlDefinition(element) }, 
			{ Maml.DefinitionTable, (_, element) => new MamlDefinitionTable(element) }, 

			// structureList
			{ Maml.List, (_, element) => new MamlList(element) }, 
			{ Maml.ListItem, (_, element) => new MamlListItem(element) }, 

			// structureMedia
			{ Maml.MediaLink, (_, element) => new MamlMediaLink(element) },

			// structureProcedure
			{ Maml.Steps, (_, element) => new MamlSteps(element) },
			{ Maml.Step, (_, element) => new MamlStep(element) },
			{ Maml.Procedure, (_, element) => new MamlProcedure(element) },

			// structureTable
			{ Maml.Table, (_, element) => new MamlTable(element) },
			{ Maml.TableHeader, (_, element) => new MamlTableHeader(element) },
			{ Maml.Entry, (_, element) => new MamlTableEntry(element) },
			{ Maml.Row, (_, element) => new MamlTableRow(element) }, 

			// glossary
			{ Maml.Glossary, (_, element) => new MamlGlossary(element) },
			{ Maml.GlossaryDiv, (_, element) => new MamlGlossaryDiv(element) },
			{ Maml.GlossaryEntry, (_, element) => new MamlGlossaryEntry(element) },
			{ Maml.Terms, (_, element) => new MamlTerms(element) }, 
			{ Maml.Term, (_, element) => new MamlString(element) }, 
			{ Maml.RelatedEntry, (_, element) => new MamlRelatedEntry(element) }
		};

		protected virtual MamlNode CreateDateNode(XElement element)
		{
			return new MamlText(element);
		}

		protected virtual MamlNode CreateParametersNode(XElement element)
		{
			return new MamlParameters(element);
		}

		protected virtual MamlNode CreateReturnValueNode(XElement element)
		{
			return new MamlSectionContent(element);
		}

		protected virtual MamlNode CreateExceptionsNode(XElement element)
		{
			return new MamlExceptions(element);
		}
	}
}