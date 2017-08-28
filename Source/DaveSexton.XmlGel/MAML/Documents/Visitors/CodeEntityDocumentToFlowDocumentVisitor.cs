using System;
using System.Windows.Documents;
using DaveSexton.XmlGel.Extensions;

namespace DaveSexton.XmlGel.Maml.Documents.Visitors
{
	/* codeEntityDocumentType (developer.xsd)
	 * 
	 * Sequence:
	 *	- assembly (MamlAssembly)
	 *	- codeEntities (MamlCodeEntities)
	 */
	internal sealed class CodeEntityDocumentToFlowDocumentVisitor : MamlToFlowDocumentVisitor
	{
		public CodeEntityDocumentToFlowDocumentVisitor(MamlDocument document, Action uiContainerChanged)
			: base(document, uiContainerChanged)
		{
		}

		public override TextElement Visit(MamlAssembly assembly, out TextElement contentContainer)
		{
			return contentContainer = new Paragraph()
			{
				Tag = assembly.Element.AsDataOnly(assembly)
			};
		}

		public override TextElement Visit(MamlCodeEntities entities, out TextElement contentContainer)
		{
			return contentContainer = new Section()
			{
				Tag = entities.Element.AsDataOnly(entities)
			};
		}

		public override TextElement Visit(MamlCodeEntity entity, out TextElement contentContainer)
		{
			return contentContainer = new Section()
			{
				Tag = entity.Element.AsDataOnly(entity)
			};
		}

		public override TextElement Visit(MamlCodeReferenceDescription description, out TextElement contentContainer)
		{
			return contentContainer = new Section()
			{
				Tag = description.Element.AsDataOnly(description)
			};
		}

		public override TextElement Visit(MamlParameterReferenceDescription description, out TextElement contentContainer)
		{
			return contentContainer = new Section()
			{
				Tag = description.Element.AsDataOnly(description)
			};
		}

		public override TextElement Visit(MamlGenericParameters parameters, out TextElement contentContainer)
		{
			return contentContainer = new Section()
			{
				Tag = parameters.Element.AsDataOnly(parameters)
			};
		}

		public override TextElement Visit(MamlParameters parameters, out TextElement contentContainer)
		{
			return contentContainer = new Section()
			{
				Tag = parameters.Element.AsDataOnly(parameters)
			};
		}

		public override TextElement Visit(MamlExceptions exceptions, out TextElement contentContainer)
		{
			return contentContainer = new Section()
			{
				Tag = exceptions.Element.AsDataOnly(exceptions)
			};
		}

		public override TextElement Visit(MamlPlatformNotes notes, out TextElement contentContainer)
		{
			return contentContainer = new Section()
			{
				Tag = notes.Element.AsDataOnly(notes)
			};
		}

		public override TextElement Visit(MamlPlatformNote note, out TextElement contentContainer)
		{
			return contentContainer = new Section()
			{
				Tag = note.Element.AsDataOnly(note)
			};
		}

		public override TextElement Visit(MamlPlatforms platforms, out TextElement contentContainer)
		{
			return contentContainer = new Paragraph()
			{
				Tag = platforms.Element.AsDataOnly(platforms)
			};
		}

		public override TextElement Visit(MamlPermissions permissions, out TextElement contentContainer)
		{
			return contentContainer = new Section()
			{
				Tag = permissions.Element.AsDataOnly(permissions)
			};
		}
	}
}