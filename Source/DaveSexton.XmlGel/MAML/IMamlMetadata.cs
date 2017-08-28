using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace DaveSexton.XmlGel.Maml
{
	public interface IMamlMetadata : INotifyPropertyChanged
	{
		Guid Id { get; }

		string TableOfContentsTitle { get; set; }

		string Title { get; set; }

		string Author { get; set; }

		DateTimeOffset LastModified { get; set; }

		/// <summary>
		/// Gets a collection of keyword index names, which can be passed to implementations of <see cref="GetKeywords"/>, 
		/// <see cref="GetSubkeywords"/> and <see cref="SetKeyword"/>.
		/// </summary>
		ICollection<string> KeywordIndexes { get; }

		ICollection<string> AttributeNames { get; }

		ICollection<string> CustomProperties { get; }

		void Save(string documentFile, bool setLastModifiedDateTime);

		ICollection<string> GetKeywords(string index);

		ICollection<string> GetSubkeywords(string index, string keyword);

		void SetKeyword(string index, string keyword, params string[] subkeywords);

		ICollection<string> GetAttributeValues(string name);

		void AddAttributeValue(string name, string value);

		void SetAttributeValues(string name, params string[] values);

		void SetAttributes(IEnumerable<KeyValuePair<string, string>> allAttributes);

		string GetCustomPropertyValue(string name);

		void SetCustomPropertyValue(string name, string value);

		void ClearAttributes();

		void ClearKeywords();

		void ClearCustomProperties();
	}
}