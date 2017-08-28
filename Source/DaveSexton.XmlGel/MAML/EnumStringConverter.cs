using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;

namespace DaveSexton.XmlGel.Maml
{
	internal static class EnumStringConverter
	{
		private const string defaultValueResourceName = "|DEFAULT VALUE|";
		private static readonly string enumResourceNamePrefix = typeof(EnumStringConverter).Namespace + ".Enums.";

		private static readonly MethodInfo fromDocumentValueMethod = typeof(EnumStringConverter)
			.GetMethods(BindingFlags.Public | BindingFlags.Static)
			.First(method => method.IsGenericMethod && method.Name == "FromDocumentValue");

		private static readonly Dictionary<Type, ResourceManager> fromDocumentValueResourceManagers = new Dictionary<Type, ResourceManager>();
		private static readonly Dictionary<Type, ResourceManager> toDocumentValueResourceManagers = new Dictionary<Type, ResourceManager>();
		private static readonly Dictionary<Type, ResourceManager> displayNameResourceManagers = new Dictionary<Type, ResourceManager>();

		public static IEnumerable<KeyValuePair<object, object>> GetOptions<TEnum>()
			where TEnum : struct
		{
			return from value in Enum.GetValues(typeof(TEnum)).Cast<TEnum>()
						 select new KeyValuePair<object, object>(ToDisplayName(value), value);
		}

		public static object FromDocumentValue(Type enumType, string value)
		{
			return fromDocumentValueMethod.MakeGenericMethod(enumType).Invoke(null, new[] { value });
		}

		public static TEnum? FromDocumentValue<TEnum>(string value)
			where TEnum : struct
		{
			if (string.IsNullOrEmpty(value))
			{
				return ((TEnum[]) Enum.GetValues(typeof(TEnum)))[0];
			}

			var manager = GetFromDocumentValueResourceManager(typeof(TEnum));

			TEnum enumValue;

			if (manager == null
				? Enum.TryParse(value, true, out enumValue)
				: Enum.TryParse(manager.GetString(value, CultureInfo.CurrentUICulture), true, out enumValue))
			{
				return enumValue;
			}
			else
			{
				return null;
			}
		}

		public static string ToDocumentValue<TEnum>(TEnum? value)
			where TEnum : struct
		{
			if (value == null)
			{
				return null;
			}

			var manager = GetToDocumentValueResourceManager(typeof(TEnum));

			return manager == null
					? value.ToString()
					: manager.GetString(value.ToString(), CultureInfo.CurrentUICulture);
		}

		public static string ToDisplayName<TEnum>(TEnum value)
			where TEnum : struct
		{
			var manager = GetDisplayNameResourceManager(typeof(TEnum));

			return manager == null
					? value.ToString()
					: manager.GetString(value.ToString(), CultureInfo.CurrentUICulture);
		}

		private static ResourceManager GetFromDocumentValueResourceManager(Type type)
		{
			return GetResourceManager(type, fromDocumentValueResourceManagers, ".FromDocumentValue");
		}

		private static ResourceManager GetToDocumentValueResourceManager(Type type)
		{
			return GetResourceManager(type, toDocumentValueResourceManagers, ".ToDocumentValue");
		}

		private static ResourceManager GetDisplayNameResourceManager(Type type)
		{
			return GetResourceManager(type, displayNameResourceManagers, ".DisplayNames");
		}

		[DebuggerHidden]
		private static ResourceManager GetResourceManager(Type type, Dictionary<Type, ResourceManager> cache, string enumResourceFileNameSuffix)
		{
			ResourceManager manager;

			if (!cache.TryGetValue(type, out manager))
			{
				manager = new ResourceManager(enumResourceNamePrefix + type.Name + enumResourceFileNameSuffix, typeof(EnumStringConverter).Assembly);

				try
				{
					manager.GetResourceSet(CultureInfo.CurrentUICulture, createIfNotExists: false, tryParents: true);
				}
				catch (MissingManifestResourceException)
				{
					manager = null;
				}

				cache.Add(type, manager);
			}

			return manager;
		}
	}
}