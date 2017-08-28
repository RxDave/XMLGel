using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace DaveSexton.XmlGel.Maml
{
	internal static class MamlCompanion
	{
		public const string AssetTypeIdValue = "CompanionFile";

		private static readonly Regex xmlNameEncoder = new Regex(
			// match invalid XML name leading and inner characters so they may be replaced
			@"(?<Leading> ^[^a-zA-Z]+ )  |  (?<Inner> [^a-zA-Z0-9\.] )",
			RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline | RegexOptions.CultureInvariant);

		public static readonly XName Root = "metadata";
		public static readonly XName AssetTypeId = "assetTypeId";
		public static readonly XName FileAssetGuid = "fileAssetGuid";

		public static readonly XName Title = "title";
		public static readonly XName TableOfContentsTitle = "tableOfContentsTitle";
		public static readonly XName Author = "author";
		public static readonly XName Attribute = "attribute";
		public static readonly XName Keyword = "keyword";
		public static readonly XName LastModified = "lastModified";

		public static readonly XName KeywordIndex = "index";
		public static readonly XName AttributeName = "name";

		public static bool IsKnownMetadata(XElement element)
		{
			var name = element.Name;

			return name == Title
					|| name == TableOfContentsTitle
					|| name == Author
					|| name == Attribute
					|| name == Keyword
					|| name == LastModified;
		}

		public static IList<string> ParseKeywords(XElement element, string index)
		{
			return (from node in element.Nodes()
							let t = node as XText
							let text = (t == null) ? null : t.Value.Trim()
							let child = node as XElement
							where !string.IsNullOrWhiteSpace(text)
								 || (child != null
									 && child.Name == MamlCompanion.Keyword
									 && string.Equals(index, (string) child.Attribute(MamlCompanion.KeywordIndex), StringComparison.OrdinalIgnoreCase))
							let subtext = child == null ? null : ((string) child).Trim()
							let keyword = subtext ?? text
							where !string.IsNullOrWhiteSpace(keyword)
							select UnescapeKeywordTerm(keyword))
							.ToList();
		}

		public static XDocument CreateDocument(IMamlMetadata metadata)
		{
			var id = metadata.Id.ToString("D");

			return new XDocument(
				new XElement(MamlCompanion.Root,
					new XAttribute(MamlCompanion.AssetTypeId, MamlCompanion.AssetTypeIdValue),
					new XAttribute(MamlCompanion.FileAssetGuid, id),
					new XElement(Maml.TopicRoot,
						new XAttribute(Maml.TopicId, id),

						string.IsNullOrWhiteSpace(metadata.Title) ? null : new XElement(MamlCompanion.Title, metadata.Title),
						string.IsNullOrWhiteSpace(metadata.TableOfContentsTitle) ? null : new XElement(MamlCompanion.TableOfContentsTitle, metadata.TableOfContentsTitle),
						string.IsNullOrWhiteSpace(metadata.Author) ? null : new XElement(MamlCompanion.Author, metadata.Author),
						metadata.LastModified == DateTimeOffset.MinValue ? null : new XElement(MamlCompanion.LastModified, metadata.LastModified),

						metadata.CustomProperties.Select(name => new XElement(EncodeXName(name), metadata.GetCustomPropertyValue(name))),

						metadata.AttributeNames.SelectMany(
							metadata.GetAttributeValues,
							(name, value) =>
								new XElement(MamlCompanion.Attribute,
									new XAttribute(MamlCompanion.AttributeName, name),
									value)),

						metadata.KeywordIndexes.SelectMany(
							metadata.GetKeywords,
							(index, keyword) =>
							{
								var subKeywords = metadata.GetSubkeywords(index, keyword);

								return new XElement(MamlCompanion.Keyword,
									new XAttribute(MamlCompanion.KeywordIndex, index),
									string.IsNullOrWhiteSpace(keyword) ? null : EscapeKeywordTerm(keyword.Trim()),
									subKeywords.Count == 0
										? null
										: subKeywords.Select(
												subKeyword => subKeyword == null || (subKeyword = subKeyword.Trim()).Length == 0
												? null
												: new XElement(MamlCompanion.Keyword,
														new XAttribute(MamlCompanion.KeywordIndex, index),
														EscapeKeywordTerm(subKeyword))));
							})
						)
					)
				);
		}

		private static string EncodeXName(string name)
		{
			if (string.IsNullOrWhiteSpace(name))
			{
				return name;
			}

			name = xmlNameEncoder.Replace(
				name,
				match =>
				{
					if (match.Groups["Leading"].Success)
					{
						return string.Empty;
					}
					else
					{
						return "_";
					}
				});

			if (name.Length == 0)
			{
				return "property";
			}
			else
			{
				return name;
			}
		}

		private static string EscapeKeywordTerm(string term)
		{
			return term == null ? null : term.Replace(",", "%2C").Replace(";", "%3B").Replace(":", "%3A");
		}

		private static string UnescapeKeywordTerm(string term)
		{
			return term == null ? null : term.Replace("%2C", ",").Replace("%3B", ";").Replace("%3A", ":");
		}
	}
}