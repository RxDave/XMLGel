using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Xml.Linq;

namespace DaveSexton.XmlGel.Maml
{
	[ContractClass(typeof(IMamlDocumentContract))]
	public interface IMamlDocument
	{
		MamlDocumentKind Kind { get; }

		Guid Id { get; }

		int RevisionNumber { get; }

		IMamlMetadata Metadata { get; }

		XDocument Document { get; }

		XElement DocumentRoot { get; }

		IDictionary<XNamespace, string> Namespaces { get; }

		bool HasFlowDocument { get; }

		void Update();

		void Save(string file, bool incrementRevisionNumber, bool setLastModifiedDateTime);

		void Save(Stream stream, bool incrementRevisionNumber, bool setLastModifiedDateTime);
	}

	[ContractClassFor(typeof(IMamlDocument))]
	internal abstract class IMamlDocumentContract : IMamlDocument
	{
		public MamlDocumentKind Kind
		{
			get
			{
				return default(MamlDocumentKind);
			}
		}

		public Guid Id
		{
			get
			{
				return default(Guid);
			}
		}

		public int RevisionNumber
		{
			get
			{
				Contract.Ensures(Contract.Result<int>() >= 0);
				return 0;
			}
		}

		public IMamlMetadata Metadata
		{
			get
			{
				Contract.Ensures(Contract.Result<IMamlMetadata>() != null);
				return null;
			}
		}

		public XDocument Document
		{
			get
			{
				Contract.Ensures(Contract.Result<XDocument>() != null);
				return null;
			}
		}

		public XElement DocumentRoot
		{
			get
			{
				Contract.Ensures(Contract.Result<XElement>() != null);
				return null;
			}
		}

		public IDictionary<XNamespace, string> Namespaces
		{
			get
			{
				Contract.Ensures(Contract.Result<IDictionary<XNamespace, string>>() != null);
				return null;
			}
		}

		public bool HasFlowDocument
		{
			get
			{
				return true;
			}
		}

		public void Update()
		{
			Contract.Requires(HasFlowDocument);
		}

		public void Save(string file, bool incrementRevisionNumber, bool setLastModifiedDateTime)
		{
			Contract.Requires(!string.IsNullOrEmpty(file));
		}

		public void Save(Stream stream, bool incrementRevisionNumber, bool setLastModifiedDateTime)
		{
			Contract.Requires(stream != null);
		}
	}
}