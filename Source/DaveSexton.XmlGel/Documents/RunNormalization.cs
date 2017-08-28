using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;
using System.Windows.Documents;

namespace DaveSexton.XmlGel.Documents
{
	/* To solve the problems described below, I've modified the editor control so that it automatically 
	 * nulls the containing Run's Tag property whenever any text is modified (by calling the UserModified
	 * method); however, this class still stores a reference to the Run in the Tag object, just as a 
	 * precaution in case the RichTextBox's default duplication behavior still causes a Tag object to be 
	 * copied to new consecutive Runs, as described below.
	 * ------------------------------------------------------------------------------------------------
	 * 
	 * While designing this class I had assumed that editing any Run causes its Tag to be deleted by the 
	 * RichTextBox control; however, testing has revealed that this isn't the actual behavior.  In fact, 
	 * not only can a Run be edited in place while keeping its Tag object, but the RichTextBox control
	 * creates consecutive Run objects and copies the Tag object from the previous Run.  This causes new
	 * consecutive Run objects to have duplicate whitespace modification Tags that cause denormalization
	 * to not function properly.  In other words, the RichTextBox duplicates the Tag reference from the 
	 * previous Run.  As a result, pressing enter within a modified Run may cause the subsequent new Run 
	 * to be tagged with the previous whitespace modification when it should actually have no Tag at all.
	 * 
	 * By storing a reference to the Run within the Tag it can be determined later whether the Tag belongs 
	 * to the current Run or was instead duplicated by the RichTextBox control.
	 * 
	 * A consequence of storing a reference to the Run is that deleting it and then undoing the deletion 
	 * causes the Run to be replaced with a new Run object but with the same Tag object.  The tag object 
	 * is the same, but the Run object doesn't match so the whitespace modification is ignored.  It's an 
	 * acceptable trade-off.
	 * 
	 * There are other problems with denormalization related to the RichTextBox behavior (i.e., behavior 
	 * that causes edited Runs to keep their Tag objects).  For example, selecting the whitespace character 
	 * between two Runs that have been normalized and replacing it with a single space character (i.e., 
	 * delete/insert in one action) causes a space to be appended to the end of the first run and removed 
	 * from the beginning of the second run; however, the second run contains a normalized whitespace Tag
	 * to indicate that the leading space should be removed by the Denormalize method.  The Run object 
	 * itself is the same reference, so Denormalize deletes the first character, which used to be the 
	 * single normalized space but is now the first non-whitespace character (previously the character in
	 * the second position).
	 * 
	 * Similar issues may exist, though only the previous issue was seen during testing.
	 */
	internal sealed class RunNormalization
	{
		private static readonly object leadingWhitespaceKey = new object();
		private static readonly object trailingWhitespaceKey = new object();
		internal static readonly object normalizedWhitespaceKey = new object();

		public static Run TrimStart(string value)
		{
			var leadingWhitespace = new StringBuilder();

			int i;
			for (i = 0; i < value.Length; i++)
			{
				var c = value[i];

				if (char.IsWhiteSpace(c))
				{
					leadingWhitespace.Append(c);
				}
				else
				{
					break;
				}
			}

			return CreateModifiedRun(value.Substring(i), leadingWhitespaceKey, leadingWhitespace.ToString());
		}

		public static void EnsureLastRunIsTrimmed(Paragraph paragraph)
		{
			if (paragraph != null && paragraph.Inlines.Count > 0)
			{
				var last = paragraph.Inlines.LastInline;

				var lastSpan = last as Span;

				while (lastSpan != null)
				{
					last = lastSpan.Inlines.LastInline;
					lastSpan = last as Span;
				}

				var lastRun = last as Run;

				if (lastRun != null && lastRun.Text != null)
				{
					var text = lastRun.Text;

					var trimmed = GetModification(lastRun);

					if (!trimmed.HasValue || trimmed.Value.Key == normalizedWhitespaceKey)
					{
						var trailingWhitespace = new StringBuilder();

						int i;
						for (i = text.Length - 1; i >= 0; i--)
						{
							var c = text[i];

							if (char.IsWhiteSpace(c))
							{
								trailingWhitespace.Insert(0, c);
							}
							else
							{
								break;
							}
						}

						if (i < text.Length - 1)
						{
							Contract.Assert(!trimmed.HasValue || trimmed.Value.Key == normalizedWhitespaceKey);

							if (!trimmed.HasValue || trailingWhitespace.Length < text.Length)
							{
								SetModification(lastRun, trailingWhitespaceKey, trailingWhitespace.ToString());
							}

							lastRun.Text = text.Substring(0, i + 1);
						}
					}
				}
			}
		}

		/// <summary>
		/// Reverses the changes made by <see cref="Normalize"/>.
		/// </summary>
		/// <remarks>
		/// This method must return a new <see cref="Run"/> instead of mutating the specified <paramref name="run"/> 
		/// because changing the Text property causes the parent element's child iterators to be flagged as changed, 
		/// causing an exception to be thrown while iterating in the parent scope.
		/// </remarks>
		public static Run Denormalize(Run run)
		{
			var whitespace = GetModification(run);

			if (whitespace.HasValue)
			{
				var key = whitespace.Value.Key;
				var value = whitespace.Value.Value;

				if (key == leadingWhitespaceKey)
				{
					return new Run(value + run.Text)
					{
						// The tag is no longer used at this point, but it's nice to have when dumping the document structure.
						Tag = run.Tag
					};
				}
				else if (key == trailingWhitespaceKey)
				{
					return new Run(run.Text + value)
					{
						// The tag is no longer used at this point, but it's nice to have when dumping the document structure.
						Tag = run.Tag
					};
				}
				else if (key == normalizedWhitespaceKey)
				{
					if (run.Text.Length > 1)
					{
						return new Run(value + run.Text.Substring(1))
						{
							// The tag is no longer used at this point, but it's nice to have when dumping the document structure.
							Tag = run.Tag
						};
					}
					else
					{
						return new Run(value)
						{
							// The tag is no longer used at this point, but it's nice to have when dumping the document structure.
							Tag = run.Tag
						};
					}
				}
			}

			return run;
		}

		/// <summary>
		/// Collapses whitespace into a single space character that leads a new Run (or representing an entire Run when at the end of the
		/// specified <paramref name="value"/>) and then tags that Run with the original leading whitespace so that it can be expanded 
		/// later by <see cref="FlowDocumentToXDocumentVisitor"/>.
		/// </summary>
		public static IEnumerable<Run> Normalize(string value)
		{
			Contract.Requires(!string.IsNullOrEmpty(value));

			var runBuilder = new StringBuilder();
			string whitespace = null;

			foreach (var range in NormalizeIntoAlternatingRanges(value))
			{
				var isNormalizableWhitespace = range.Item1;

				if (isNormalizableWhitespace)
				{
					if (runBuilder.Length > 0)
					{
						if (whitespace != null)
						{
							yield return CreateModifiedRun(runBuilder.ToString(), normalizedWhitespaceKey, whitespace);

							whitespace = null;
						}
						else
						{
							yield return new Run(runBuilder.ToString());
						}

						runBuilder.Clear();
					}

					whitespace = value.Substring(range.Item2, range.Item3);

					runBuilder.Append(' ');
				}
				else
				{
					runBuilder.Append(value, range.Item2, range.Item3);
				}
			}

			if (runBuilder.Length > 0)
			{
				if (whitespace != null)
				{
					yield return CreateModifiedRun(runBuilder.ToString(), normalizedWhitespaceKey, whitespace);
				}
				else
				{
					yield return new Run(runBuilder.ToString());
				}
			}
		}

		internal static IEnumerable<Tuple<bool, int, int>> NormalizeIntoAlternatingRanges(string value)
		{
			Contract.Requires(!string.IsNullOrEmpty(value));

			int currentNonWhitespaceIndex = 0, currentNonWhitespaceLength = 0;
			int currentWhitespaceIndex = 0, currentWhitespaceLength = 0;

			var waitingForConsecutiveWhitespace = false;

			for (int i = 0; i < value.Length; i++)
			{
				var c = value[i];

				if (char.IsWhiteSpace(c))
				{
					if (!waitingForConsecutiveWhitespace
						&& currentWhitespaceLength == 0
						&& c == ' ')
					{
						waitingForConsecutiveWhitespace = true;
					}
					else
					{
						if (currentWhitespaceLength == 0)
						{
							currentWhitespaceIndex = i;
						}

						currentWhitespaceLength++;

						if (waitingForConsecutiveWhitespace)
						{
							currentWhitespaceIndex--;
							currentWhitespaceLength++;

							waitingForConsecutiveWhitespace = false;
						}

						if (currentNonWhitespaceLength > 0)
						{
							yield return Tuple.Create(false, currentNonWhitespaceIndex, currentNonWhitespaceLength);

							currentNonWhitespaceIndex = 0;
							currentNonWhitespaceLength = 0;
						}
					}
				}
				else
				{
					if (currentNonWhitespaceLength == 0)
					{
						currentNonWhitespaceIndex = i;
					}

					currentNonWhitespaceLength++;

					if (waitingForConsecutiveWhitespace)
					{
						if (currentNonWhitespaceLength == 1)
						{
							currentNonWhitespaceIndex--;
						}

						currentNonWhitespaceLength++;

						waitingForConsecutiveWhitespace = false;
					}

					if (currentWhitespaceLength > 0)
					{
						yield return Tuple.Create(true, currentWhitespaceIndex, currentWhitespaceLength);

						currentWhitespaceIndex = 0;
						currentWhitespaceLength = 0;
					}
				}
			}

			if (waitingForConsecutiveWhitespace)
			{
				Contract.Assert(currentWhitespaceLength == 0);

				currentNonWhitespaceLength++;
			}

			if (currentWhitespaceLength > 0)
			{
				yield return Tuple.Create(true, currentWhitespaceIndex, currentWhitespaceLength);
			}
			else if (currentNonWhitespaceLength > 0)
			{
				yield return Tuple.Create(false, currentNonWhitespaceIndex, currentNonWhitespaceLength);
			}
		}

		private static Run CreateModifiedRun(string text, object key, string modification)
		{
			var run = new Run(text);

			SetModification(run, key, modification);

			return run;
		}

		private static void SetModification(Run run, object key, string modification)
		{
			run.Tag = Tuple.Create(run, new KeyValuePair<object, string>(key, modification));
		}

		internal static KeyValuePair<object, string>? GetModification(Run run)
		{
			var modifications = run.Tag as Tuple<Run, KeyValuePair<object, string>>;

			return modifications != null && object.ReferenceEquals(modifications.Item1, run)
				? modifications.Item2
				: (KeyValuePair<object, string>?) null;
		}

		internal static void UserModified(Run run)
		{
			if (run != null)
			{
				run.Tag = null;

				/* Testing has shown that NextInline must be nulled too.  The RichTextBox may strip the normalized 
				 * leading space from the next Run yet it leaves the caret at the end of the previous Run.
				 */
				run = run.NextInline as Run;

				if (run != null)
				{
					run.Tag = null;
				}
			}
		}
	}
}