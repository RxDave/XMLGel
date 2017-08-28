using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Windows.Documents;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using DaveSexton.XmlGel.Extensions;
using DaveSexton.XmlGel.Maml.Documents.Visitors;
using DaveSexton.XmlGel.Xml;

namespace DaveSexton.XmlGel.Maml
{
	public abstract class MamlDocument : IMamlDocument, INotifyPropertyChanged
	{
		public const string DefaultFileExtension = ".aml";
		public const string RelativeSchemaPath = @"Maml\Schema\Authoring\";
		public const string RootSchemaFileName = "developer.xsd";

		private static readonly IDictionary<XNamespace, string> defaultNamespaces = new Dictionary<XNamespace, string>()
			{
				{ XNamespace.None, null },
				{ XNamespace.Xml, "xml" },
				{ Maml.Namespace, null }, 
				{ Maml.XLinkNamespace, Maml.XLinkPrefixValue }
			};

		public MamlDocumentKind Kind
		{
			get
			{
				return kind;
			}
		}

		public Guid Id
		{
			get
			{
				return id;
			}
		}

		public int RevisionNumber
		{
			get
			{
				return revisionNumber;
			}
			set
			{
				if (revisionNumber != value)
				{
					revisionNumber = value;

					RaisePropertyChanged("RevisionNumber");
				}
			}
		}

		public IMamlMetadata Metadata
		{
			get
			{
				return metadata;
			}
		}

		public XDocument Document
		{
			get
			{
				return document;
			}
			private set
			{
				if (document != value)
				{
					document = value;

					RaisePropertyChanged("Document");
				}
			}
		}

		public XElement DocumentRoot
		{
			get
			{
				return documentRoot;
			}
			private set
			{
				if (documentRoot != value)
				{
					documentRoot = value;

					RaisePropertyChanged("DocumentRoot");
				}
			}
		}

		public XDocument Structure
		{
			get
			{
				Contract.Requires(HasFlowDocument);

				var structure = flowDocument.GetStructure(Maml.Namespace, flowDocumentVisitor.Factory);

				structure.Root.Add(new XAttribute("Kind", kind));
				structure.Root.Add(new XAttribute("Id", id));
				structure.Root.Add(new XAttribute("RevisionNumber", revisionNumber));

				if (!string.IsNullOrEmpty(metadata.Title))
				{
					structure.Root.Add(new XAttribute("Title", metadata.Title));
				}

				foreach (var ns in namespaces.Where(ns => ns.Key != XNamespace.None
																							 && ns.Key != XNamespace.Xml
																							 && ns.Value != null))
				{
					structure.Root.Add(new XAttribute(XNamespace.Xmlns + ns.Value, ns.Key));
				}

				return structure;
			}
		}

		public IDictionary<XNamespace, string> Namespaces
		{
			get
			{
				return namespaces;
			}
		}

		public bool HasFlowDocument
		{
			get
			{
				Contract.Ensures(Contract.Result<bool>() == (flowDocument != null && flowDocumentVisitor != null));

				return flowDocument != null && flowDocumentVisitor != null;
			}
		}

		internal XmlResolver Resolver
		{
			get
			{
				return resolver;
			}
		}

		internal ICollection<XmlSchema> Schemas
		{
			get
			{
				return schemas;
			}
		}

		internal MamlToFlowDocumentVisitor XVisitor
		{
			get
			{
				return mamlDocumentVisitor;
			}
		}

		private readonly IDictionary<XNamespace, string> namespaces = new Dictionary<XNamespace, string>(defaultNamespaces);
		private readonly Guid id;
		private readonly IMamlMetadata metadata;
		private readonly MamlDocumentKind kind;
		private XmlResolver resolver;
		private ICollection<XmlSchema> schemas;
		private XDocument document;
		private XElement documentRoot;
		private FlowDocument flowDocument;
		private MamlToFlowDocumentVisitor mamlDocumentVisitor;
		private FlowDocumentToMamlVisitor flowDocumentVisitor;
		private int revisionNumber;

		private MamlDocument(MamlDocumentKind kind, Guid id, int revisionNumber)
		{
			this.kind = kind;
			this.id = id;
			this.revisionNumber = revisionNumber;
		}

		internal MamlDocument(MamlDocumentKind kind)
			: this(kind, Guid.NewGuid(), 0)
		{
			this.documentRoot = Maml.GetDefaultDocument(kind);
			this.document = Maml.CreateTopic(id, revisionNumber, documentRoot);
			this.metadata = new MamlMetadata(id);
		}

		internal MamlDocument(MamlDocumentKind kind, Guid id, int revisionNumber, XDocument document, XElement documentRoot, IMamlMetadata metadata)
			: this(kind, id, revisionNumber)
		{
			this.document = document;
			this.documentRoot = documentRoot;
			this.metadata = metadata;
		}

		public static MamlDocument Create(MamlDocumentKind kind)
		{
			return Maml.CreateDocument(kind);
		}

		private static MamlDocument Create(Guid id, int revisionNumber, XDocument document, XElement documentRoot, IMamlMetadata metadata)
		{
			return Maml.CreateDocument(documentRoot.Name, id, revisionNumber, document, documentRoot, metadata);
		}

		public static MamlDocument FromFile(string file)
		{
			// Do not preserve whitespace when loading the XML document.  Insignificant whitespace must be ignored when visiting.
			var document = XDocument.Load(file, LoadOptions.SetLineInfo);

			return From(document, file, id => MamlMetadata.ForDocument(file, id));
		}

		public static MamlDocument FromStream(Stream stream)
		{
			// Do not preserve whitespace when loading the XML document.  Insignificant whitespace must be ignored when visiting.
			var document = XDocument.Load(stream, LoadOptions.SetLineInfo);

			var fileStream = stream as FileStream;
			var file = fileStream == null ? null : fileStream.Name;

			return From(document, file, id => MamlMetadata.ForDocument(file, id));
		}

		public static MamlDocument FromString(string topic)
		{
			// Do not preserve whitespace when loading the XML document.  Insignificant whitespace must be ignored when visiting.
			var document = XDocument.Parse(topic, LoadOptions.SetLineInfo);

			return From(document, null, id => MamlMetadata.ForDocument(null, id));
		}

		public static MamlDocument From(XDocument document, string file, Func<Guid, MamlMetadata> metadataFactory)
		{
			var topic = document.Root;

			if (topic.Name != Maml.TopicRoot)
			{
				throw topic.CreateXmlException("Invalid MAML topic.  Expected '" + Maml.TopicRoot.LocalName + "' as the root element.");
			}

			var documentRoot = topic.Elements().FirstOrDefault();

			if (documentRoot != null)
			{
				var id = EnsureId(topic, file);
				var revisionNumber = EnsureRevisionNumber(topic);

				var metadata = metadataFactory(id);

				var mamlDocument = Create(id, revisionNumber, document, documentRoot, metadata);

				if (mamlDocument == null)
				{
					throw documentRoot.CreateXmlException("Invalid MAML topic.  The document type element is unknown.");
				}

				return mamlDocument;
			}

			throw documentRoot.CreateXmlException("Invalid MAML topic.  Expected child element under '" + Maml.TopicRoot.LocalName + "' element indicating the document type.");
		}

		private static Guid EnsureId(XElement topic, string file)
		{
			Guid id;
			if (!Guid.TryParse((string) topic.Attribute(Maml.TopicId), out id))
			{
				id = TryParseFromFileNameOrCreateGuid(file);

				topic.SetAttributeValue(Maml.TopicId, id.ToString("D"));
			}

			return id;
		}

		[DebuggerHidden]
		private static Guid TryParseFromFileNameOrCreateGuid(string file)
		{
			try
			{
				file = file == null ? string.Empty : Path.GetFileNameWithoutExtension(file);
			}
			catch (ArgumentException)
			{
				file = string.Empty;
			}

			Guid id;
			TryParseOrCreateGuid(file, out id);

			return id;
		}

		private static bool TryParseOrCreateGuid(string value, out Guid id)
		{
			var parsed = Guid.TryParse(value, out id);

			if (!parsed)
			{
				id = Guid.NewGuid();
			}

			return parsed;
		}

		private static int EnsureRevisionNumber(XElement topic)
		{
			int revisionNumber;
			if (!int.TryParse((string) topic.Attribute(Maml.TopicRevisionNumber), out revisionNumber))
			{
				revisionNumber = 0;

				topic.SetAttributeValue(Maml.TopicRevisionNumber, revisionNumber);
			}

			return revisionNumber;
		}

		public FlowDocument GenerateFlowDocument(Action<XNode> invalidNode, Action<XAttribute> invalidAttribute, Action uiContainerChanged)
		{
			return GenerateFlowDocument(invalidNode, invalidAttribute, uiContainerChanged, null);
		}

		public FlowDocument GenerateFlowDocument(Action<XNode> invalidNode, Action<XAttribute> invalidAttribute, Action uiContainerChanged, XmlResolver resolver, params XmlSchema[] schemas)
		{
			return GenerateFlowDocument(invalidNode, invalidAttribute, uiContainerChanged, resolver, (ICollection<XmlSchema>) schemas);
		}

		public FlowDocument GenerateFlowDocument(Action<XNode> invalidNode, Action<XAttribute> invalidAttribute, Action uiContainerChanged, XmlResolver resolver, ICollection<XmlSchema> schemas)
		{
			Contract.Requires(!HasFlowDocument);
			Contract.Ensures(HasFlowDocument);

			if (resolver == null && (schemas == null || schemas.Count == 0))
			{
				ValidationEventHandler resourceValidationEvent = (_, e) =>
					{
						if (e.Severity == XmlSeverityType.Error)
						{
							throw e.Exception;
						}
					};

				resolver = new XmlLocalUrlResolver(Path.Combine(Environment.CurrentDirectory, RelativeSchemaPath), resourceValidationEvent);

				schemas = new List<XmlSchema>()
					{
						(XmlSchema) resolver.GetEntity(resolver.ResolveUri(null, RootSchemaFileName), null, null)
					};
			}

			this.resolver = resolver;
			this.schemas = schemas;

			mamlDocumentVisitor = CreateVisitor(uiContainerChanged);

			VisitOriginalDocument(invalidNode, invalidAttribute);

			flowDocument = mamlDocumentVisitor.Document;

			flowDocumentVisitor = CreateVisitor(flowDocument);

			RaisePropertyChanged("HasFlowDocument");

			return flowDocument;
		}

		private void VisitOriginalDocument(Action<XNode> invalidNode, Action<XAttribute> invalidAttribute)
		{
			Contract.Requires(mamlDocumentVisitor != null);

			if (invalidAttribute != null)
			{
				mamlDocumentVisitor.InvalidAttribute += invalidAttribute;
			}

			if (invalidNode != null)
			{
				mamlDocumentVisitor.InvalidNode += invalidNode;
			}

			try
			{
				mamlDocumentVisitor.Visit();
			}
			finally
			{
				if (invalidAttribute != null)
				{
					mamlDocumentVisitor.InvalidAttribute -= invalidAttribute;
				}

				if (invalidNode != null)
				{
					mamlDocumentVisitor.InvalidNode -= invalidNode;
				}
			}
		}

		public void UpdateDynamicContent()
		{
			mamlDocumentVisitor.UpdateDynamicContent();
		}

		public void Update()
		{
			flowDocumentVisitor.Visit();

			Document = flowDocumentVisitor.XDocument;
			DocumentRoot = document.Root.Elements().FirstOrDefault();

			EnsureNamespacesOnDocument();
		}

		private void EnsureNamespacesOnDocument()
		{
			var namespaceDeclarations = documentRoot.Attributes()
				.Where(attribute => attribute.Name.Namespace == XNamespace.Xmlns)
				.Select(attribute => (XNamespace) attribute.Value)
				.ToList();

			foreach (var ns in namespaces
				.Where(ns => !defaultNamespaces.ContainsKey(ns.Key))
				.Where(ns => !namespaceDeclarations.Contains(ns.Key)))
			{
				documentRoot.SetAttributeValue(XNamespace.Xmlns + ns.Value, ns.Key.NamespaceName);
			}
		}

		internal XDocument CreateEmptyMamlDocument(out XElement root)
		{
			root = new XElement(
				documentRoot.Name,	// The MAML namespace is automatically added as the default namespace.
				new XAttribute(Maml.XLinkPrefix, Maml.XLinkNamespace.NamespaceName));

			return Maml.CreateTopic(id, revisionNumber, root);
		}

		internal abstract MamlToFlowDocumentVisitor CreateVisitor(Action uiContainerChanged);

		internal abstract FlowDocumentToMamlVisitor CreateVisitor(FlowDocument document);

		public void Save(string file, bool incrementRevisionNumber, bool setLastModifiedDateTime)
		{
			Save(file, file, document.Save, incrementRevisionNumber, setLastModifiedDateTime);
		}

		public void Save(Stream stream, bool incrementRevisionNumber, bool setLastModifiedDateTime)
		{
			var fileStream = stream as FileStream;
			var file = fileStream == null ? null : fileStream.Name;

			Save(stream, file, document.Save, incrementRevisionNumber, setLastModifiedDateTime);
		}

		private void Save<TSource>(TSource source, string file, Action<TSource> save, bool incrementRevisionNumber, bool setLastModifiedDateTime)
		{
			if (incrementRevisionNumber)
			{
				RevisionNumber++;
			}

			document.Root.SetAttributeValue(Maml.TopicRevisionNumber, revisionNumber);

			try
			{
				save(source);
			}
			catch
			{
				document.Root.SetAttributeValue(Maml.TopicRevisionNumber, --RevisionNumber);
				throw;
			}

			if (file != null)
			{
				metadata.Save(file, setLastModifiedDateTime);
			}
		}

		protected void RaisePropertyChanged(string propertyName)
		{
			OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			var handler = PropertyChanged;

			if (handler != null)
			{
				handler(this, e);
			}
		}
	}
}