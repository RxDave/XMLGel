using System;
using System.Linq;
using DaveSexton.XmlGel.Documents;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DaveSexton.XmlGel.UnitTests.Documents
{
	[TestClass]
	public class RunNormalizationRangeTests
	{
		private static void RangeNormalize_Test(string input, params Tuple<bool, int, int>[] expected)
		{
			var results = RunNormalization.NormalizeIntoAlternatingRanges(input);

			CollectionAssert.AreEqual(expected, results.ToList());
		}

		[TestMethod]
		public void RangeNormalize_NormalizeIntoAlternatingGroups_All_Whitespace()
		{
			RangeNormalize_Test(
				new string(' ', 4),
				Tuple.Create(true, 0, 4));
		}

		[TestMethod]
		public void RangeNormalize_NormalizeIntoAlternatingGroups_Tab()
		{
			RangeNormalize_Test(
				"\t",
				Tuple.Create(true, 0, 1));
		}

		[TestMethod]
		public void RangeNormalize_NormalizeIntoAlternatingGroups_No_Whitespace()
		{
			RangeNormalize_Test(
				"AAA",
				Tuple.Create(false, 0, 3));
		}

		[TestMethod]
		public void RangeNormalize_NormalizeIntoAlternatingGroups_Single_Leading_and_Single_Trailing_Whitespace()
		{
			RangeNormalize_Test(
				" AAA ",
				Tuple.Create(false, 0, 5));
		}

		[TestMethod]
		public void RangeNormalize_NormalizeIntoAlternatingGroups_Trailing_Tab()
		{
			RangeNormalize_Test(
				"AAA\t",
				Tuple.Create(false, 0, 3),
				Tuple.Create(true, 3, 1));
		}

		[TestMethod]
		public void RangeNormalize_NormalizeIntoAlternatingGroups_Single_Leading_Whitespace()
		{
			RangeNormalize_Test(
				" AAA",
				Tuple.Create(false, 0, 4));
		}

		[TestMethod]
		public void RangeNormalize_NormalizeIntoAlternatingGroups_Leading_Tab_and_Single_Trailing_Whitespace()
		{
			RangeNormalize_Test(
				"\tAAA ",
				Tuple.Create(true, 0, 1),
				Tuple.Create(false, 1, 4));
		}

		[TestMethod]
		public void RangeNormalize_NormalizeIntoAlternatingGroups_Leading_Whitespace()
		{
			RangeNormalize_Test(
				"  AAA",
				Tuple.Create(true, 0, 2),
				Tuple.Create(false, 2, 3));
		}

		[TestMethod]
		public void RangeNormalize_NormalizeIntoAlternatingGroups_Leading_and_Single_Trailing_Whitespace()
		{
			RangeNormalize_Test(
				"  AAA ",
				Tuple.Create(true, 0, 2),
				Tuple.Create(false, 2, 4));
		}

		[TestMethod]
		public void RangeNormalize_NormalizeIntoAlternatingGroups_Leading_and_Trailing_Whitespace()
		{
			RangeNormalize_Test(
				"  AAA  ",
				Tuple.Create(true, 0, 2),
				Tuple.Create(false, 2, 3),
				Tuple.Create(true, 5, 2));
		}

		[TestMethod]
		public void RangeNormalize_NormalizeIntoAlternatingGroups_Trailing_Whitespace()
		{
			RangeNormalize_Test(
				"AAA   ",
				Tuple.Create(false, 0, 3),
				Tuple.Create(true, 3, 3));
		}

		[TestMethod]
		public void RangeNormalize_NormalizeIntoAlternatingGroups_Single_Interleaving_Whitespace()
		{
			RangeNormalize_Test(
				"AAA BBB CCC",
				Tuple.Create(false, 0, 11));
		}

		[TestMethod]
		public void RangeNormalize_NormalizeIntoAlternatingGroups_Interleaving_and_Single_Interleaving_Whitespace()
		{
			RangeNormalize_Test(
				"AAA   BBB CCC",
				Tuple.Create(false, 0, 3),
				Tuple.Create(true, 3, 3),
				Tuple.Create(false, 6, 7));
		}

		[TestMethod]
		public void RangeNormalize_NormalizeIntoAlternatingGroups_Single_Interleaving_and_Interleaving_Whitespace()
		{
			RangeNormalize_Test(
				"AAA BBB   CCC",
				Tuple.Create(false, 0, 7),
				Tuple.Create(true, 7, 3),
				Tuple.Create(false, 10, 3));
		}

		[TestMethod]
		public void RangeNormalize_NormalizeIntoAlternatingGroups_Leading_Single_Interleaving_and_Interleaving_Whitespace()
		{
			RangeNormalize_Test(
				"  AAA BBB   CCC",
				Tuple.Create(true, 0, 2),
				Tuple.Create(false, 2, 7),
				Tuple.Create(true, 9, 3),
				Tuple.Create(false, 12, 3));
		}

		[TestMethod]
		public void RangeNormalize_NormalizeIntoAlternatingGroups_Leading_and_Interleaving_Whitespace()
		{
			RangeNormalize_Test(
				"  AAA  BBB  CCC",
				Tuple.Create(true, 0, 2),
				Tuple.Create(false, 2, 3),
				Tuple.Create(true, 5, 2),
				Tuple.Create(false, 7, 3),
				Tuple.Create(true, 10, 2),
				Tuple.Create(false, 12, 3));
		}
	}
}