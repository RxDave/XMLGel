using System;
using System.Collections.Generic;
using System.Linq;
using DaveSexton.XmlGel.Documents;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DaveSexton.XmlGel.UnitTests.Documents
{
	/*		• = single space character (' ')
	 *		t = single tab character ('\t') or any other whitespace
	 *		[] = Run
	 *		{} = Run.Tag
	 * 
	 * Input						-> Output
	 * ---------------------------------------
	 * ••••							-> [•]{••••}
	 * t								-> [•]{t}
	 * AAA							-> [AAA]
	 * •AAA•						-> [•AAA•]
	 * AAAt							-> [AAA] [•]{t}
	 * •AAA							-> [•AAA]
	 * tAAA•						-> [•AAA•]{t}
	 * ••AAA						-> [•AAA]{••}
	 * ••AAA•						-> [•AAA•]{••}
	 * ••AAA••					-> [•AAA]{••} [•]{••}
	 * AAA•••						-> [AAA] [•]{•••}
	 * AAA•BBB•CCC 			-> [AAA•BBB•CCC]
	 * AAA•••BBB•CCC 		-> [AAA] [•BBB•CCC]{•••}
	 * AAA•BBB•••CCC 		-> [AAA•BBB] [•CCC]{•••}
	 * ••AAA•BBB•••CCC	-> [•AAA•BBB]{••} [•CCC]{•••}
	 * ••AAA••BBB••CCC	-> [•AAA]{••} [•BBB]{••} [•CCC]{••}
	 */
	[TestClass]
	public class RunNormalizationTests
	{
		private static void RunNormalize_Test(string input, params Tuple<string, string>[] expected)
		{
			var runs = RunNormalization.Normalize(input).ToList();

			var results = runs.Select(run =>
				{
					var whitespace = RunNormalization.GetModification(run);

					if (whitespace.HasValue)
					{
						Assert.AreEqual(whitespace.Value.Key, RunNormalization.normalizedWhitespaceKey);

						return Tuple.Create(run.Text, whitespace.Value.Value);
					}
					else
					{
						return Tuple.Create(run.Text, string.Empty);
					}
				});

			CollectionAssert.AreEqual(expected, results.ToList());

			var denormalized = runs
				.Select(RunNormalization.Denormalize)
				.Aggregate("", (acc, run) => acc += run.Text);

			Assert.AreEqual(input, denormalized);
		}

		[TestMethod]
		public void RunNormalize_NormalizeIntoAlternatingGroups_All_Whitespace()
		{
			RunNormalize_Test(
				new string(' ', 4),
				Tuple.Create(" ", new string(' ', 4)));
		}

		[TestMethod]
		public void RunNormalize_NormalizeIntoAlternatingGroups_Tab()
		{
			RunNormalize_Test(
				"\t",
				Tuple.Create(" ", "\t"));
		}

		[TestMethod]
		public void RunNormalize_NormalizeIntoAlternatingGroups_No_Whitespace()
		{
			RunNormalize_Test(
				"AAA",
				Tuple.Create("AAA", ""));
		}

		[TestMethod]
		public void RunNormalize_NormalizeIntoAlternatingGroups_Single_Leading_and_Single_Trailing_Whitespace()
		{
			RunNormalize_Test(
				" AAA ",
				Tuple.Create(" AAA ", ""));
		}

		[TestMethod]
		public void RunNormalize_NormalizeIntoAlternatingGroups_Trailing_Tab()
		{
			RunNormalize_Test(
				"AAA\t",
				Tuple.Create("AAA", ""),
				Tuple.Create(" ", "\t"));
		}

		[TestMethod]
		public void RunNormalize_NormalizeIntoAlternatingGroups_Single_Leading_Whitespace()
		{
			RunNormalize_Test(
				" AAA",
				Tuple.Create(" AAA", ""));
		}

		[TestMethod]
		public void RunNormalize_NormalizeIntoAlternatingGroups_Leading_Tab_and_Single_Trailing_Whitespace()
		{
			RunNormalize_Test(
				"\tAAA ",
				Tuple.Create(" AAA ", "\t"));
		}

		[TestMethod]
		public void RunNormalize_NormalizeIntoAlternatingGroups_Leading_Whitespace()
		{
			RunNormalize_Test(
				"  AAA",
				Tuple.Create(" AAA", "  "));
		}

		[TestMethod]
		public void RunNormalize_NormalizeIntoAlternatingGroups_Leading_and_Single_Trailing_Whitespace()
		{
			RunNormalize_Test(
				"  AAA ",
				Tuple.Create(" AAA ", "  "));
		}

		[TestMethod]
		public void RunNormalize_NormalizeIntoAlternatingGroups_Leading_and_Trailing_Whitespace()
		{
			RunNormalize_Test(
				"  AAA  ",
				Tuple.Create(" AAA", "  "),
				Tuple.Create(" ", "  "));
		}

		[TestMethod]
		public void RunNormalize_NormalizeIntoAlternatingGroups_Trailing_Whitespace()
		{
			RunNormalize_Test(
				"AAA   ",
				Tuple.Create("AAA", ""),
				Tuple.Create(" ", "   "));
		}

		[TestMethod]
		public void RunNormalize_NormalizeIntoAlternatingGroups_Single_Interleaving_Whitespace()
		{
			RunNormalize_Test(
				"AAA BBB CCC",
				Tuple.Create("AAA BBB CCC", ""));
		}

		[TestMethod]
		public void RunNormalize_NormalizeIntoAlternatingGroups_Interleaving_and_Single_Interleaving_Whitespace()
		{
			RunNormalize_Test(
				"AAA   BBB CCC",
				Tuple.Create("AAA", ""),
				Tuple.Create(" BBB CCC", "   "));
		}

		[TestMethod]
		public void RunNormalize_NormalizeIntoAlternatingGroups_Single_Interleaving_and_Interleaving_Whitespace()
		{
			RunNormalize_Test(
				"AAA BBB   CCC",
				Tuple.Create("AAA BBB", ""),
				Tuple.Create(" CCC", "   "));
		}

		[TestMethod]
		public void RunNormalize_NormalizeIntoAlternatingGroups_Leading_Single_Interleaving_and_Interleaving_Whitespace()
		{
			RunNormalize_Test(
				"  AAA BBB   CCC",
				Tuple.Create(" AAA BBB", "  "),
				Tuple.Create(" CCC", "   "));
		}

		[TestMethod]
		public void RunNormalize_NormalizeIntoAlternatingGroups_Leading_and_Interleaving_Whitespace()
		{
			RunNormalize_Test(
				"  AAA  BBB  CCC",
				Tuple.Create(" AAA", "  "),
				Tuple.Create(" BBB", "  "),
				Tuple.Create(" CCC", "  "));
		}
	}
}