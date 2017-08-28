using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Xml.Linq;
using DaveSexton.XmlGel.Maml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace DaveSexton.XmlGel.UnitTests.Maml
{
	public abstract class BaseTests
	{
		private static int winDiffCount;

		protected static void TestRoundTrip(string topic, string expected = null, int expectedInvalidNodeCount = 0, int expectedInvalidAttributeCount = 0, [System.Runtime.CompilerServices.CallerMemberName] string caller = null)
		{
			topic = topic.Trim();

			if (expected == null)
			{
				expected = topic;
			}
			else
			{
				expected = expected.Trim();
			}

			string actual;

			var document = MamlDocument.FromString(topic);

			var invalidNodes = new List<XNode>();
			var invalidAttributes = new List<XAttribute>();

			var flowDocument = document.GenerateFlowDocument(invalidNodes.Add, invalidAttributes.Add, () => { });

			Assert.AreEqual(expectedInvalidNodeCount, invalidNodes.Count, "Unexpected invalid nodes: " + invalidNodes
				.Aggregate(Environment.NewLine, (acc, node) => acc + (node is XText ? "Text" : ((XElement) node).Name) + Environment.NewLine));

			Assert.AreEqual(expectedInvalidAttributeCount, invalidAttributes.Count, "Unexpected invalid attributes: " + invalidAttributes
				.Aggregate(Environment.NewLine, (acc, attribute) => acc + attribute.Name + Environment.NewLine));

			var xmlDocumentOriginal = document.Document;

			document.Update();

			Assert.AreNotEqual(xmlDocumentOriginal, document.Document);

			using (var output = new MemoryStream())
			{
				document.Save(output, incrementRevisionNumber: false, setLastModifiedDateTime: false);

				output.Position = 0;

				using (var reader = new StreamReader(output))
				{
					actual = reader.ReadToEnd().Trim();
				}
			}

			if (expected != actual)
			{
				var expectedPath = Path.Combine(Environment.CurrentDirectory, caller + " (Expected).xml");
				var actualPath = Path.Combine(Environment.CurrentDirectory, caller + " (Actual).xml");
				var comparedPath = Path.Combine(Environment.CurrentDirectory, caller + ".dif");

				File.WriteAllText(expectedPath, expected);
				File.WriteAllText(actualPath, actual);

				if (Interlocked.Increment(ref winDiffCount) <= 5)
				{
					Process.Start(
						@"C:\Program Files (x86)\Microsoft SDKs\Windows\v7.0A\Bin\x64\WinDiff.Exe",
						"-FILRFGSA \"" + comparedPath + "\" \"" + expectedPath + "\" \"" + actualPath + "\"")
						.WaitForInputIdle();
				}
			}

			Assert.AreEqual(expected, actual, ignoreCase: false);
		}
	}
}