using System.Windows.Controls;
using System.Windows.Forms;

namespace DaveSexton.XmlGel.VisualStudio
{
	/***************
	 * Not compiled
	 ***************/

	partial class MamlTopicEditor
	{
		private string textToRecord;

		private void RecordDelete(bool backspace, bool word)
		{
			// If not backspace then it's a delete
			// If not word then it's a single character
			LastMacro macroType = backspace ?
							(word ? LastMacro.BackspaceWord : LastMacro.BackspaceChar) :
							(word ? LastMacro.DeleteWord : LastMacro.DeleteChar);

			// Get the number of times the macro type calculated above has been recorded already
			// (if any) and then add one to get the current count
			uint count = recorder.GetTimesPreviouslyRecorded(macroType) + 1;

			string macroString = "";
			// if this parameter is negative, it indicates a backspace, rather then a delete
			macroString += "ActiveDocument.Object.Delete(" + (int) (word ? tom.tomConstants.tomWord : tom.tomConstants.tomCharacter) + ", " + (backspace ? -1 * count : count) + ")";

			recorder.RecordBatchedLine(macroType, macroString);
		}

		private void RecordMove(LastMacro state, string direction, MoveScope scope, bool extend)
		{
			string macroString = "";
			macroString += "ActiveDocument.Object.Move";
			macroString += direction;
			// Get the number of times this macro type has been recorded already
			// (if any) and then add one to get the current count
			macroString += "(" + (int) scope + ", " + (recorder.GetTimesPreviouslyRecorded(state) + 1) + ", " + (int) (extend ? tom.tomConstants.tomExtend : tom.tomConstants.tomMove) + ")";

			recorder.RecordBatchedLine(state, macroString);
		}

		private void RecordCommand(string command)
		{
			if (recorder.IsRecording())
			{
				string line = "ActiveDocument.Object.";

				line += command;

				recorder.RecordLine(line);
			}
		}

		private void StopRecorder()
		{
			recorder.Stop();
		}

		private void RecordPrintableChar(char currentValue)
		{
			string macroString = "";

			if (!recorder.IsLastRecordedMacro(LastMacro.Text))
			{
				textToRecord = "";
			}

			// Only deal with text characters.  Everything, space and above is a text character
			// except DEL (0x7f).  Include carriage return (enter key) and tab, which are
			// below space, since those are also text characters.
			if (char.IsLetterOrDigit(currentValue) ||
							char.IsPunctuation(currentValue) ||
							char.IsSeparator(currentValue) ||
							char.IsSymbol(currentValue) ||
							char.IsWhiteSpace(currentValue) ||
							'\r' == currentValue || '\t' == currentValue)
			{
				if ('\r' == currentValue)
				{
					// Emit "\r\n" as the standard line terminator
					textToRecord += "\" & vbCr & \"";
				}
				else if ('\t' == currentValue)
				{
					// Emit "\t" as the standard tab
					textToRecord += "\" & vbTab & \"";
				}
				else
				{
					textToRecord += currentValue;
				}

				macroString += "ActiveDocument.Object.TypeText(\"";
				macroString += textToRecord;
				macroString += "\")";

				if (recorder.RecordBatchedLine(LastMacro.Text, macroString, 100)) // arbitrary max length
				{
					// Clear out the buffer if the line hit max length, since
					// it will not continue to be appended to
					textToRecord = "";
				}
			}
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
		private void RecordNonprintableChar(Keys currentKey)
		{
			string macroString = "";

			// Obtain the CTRL and SHIFT as they modify a number of the virtual keys. 
			bool shiftDown = System.Windows.Forms.Keys.Shift == (System.Windows.Forms.Control.ModifierKeys & System.Windows.Forms.Keys.Shift); //Keyboard::IsKeyDown(VK_SHIFT);
			bool controlDown = System.Windows.Forms.Keys.Control == (System.Windows.Forms.Control.ModifierKeys & System.Windows.Forms.Keys.Control); //Keyboard::IsKeyDown(VK_CONTROL);

			// msg.WParam indicates the virtual key.
			switch (currentKey)
			{
				case Keys.Back: // BackSpace key
					// Note that SHIFT does not affect this command
					RecordDelete(true, controlDown);
					break;

				case Keys.Delete:
					// Note that SHIFT completely disables this command
					if (!shiftDown)
					{
						RecordDelete(false, controlDown);
					}
					break;

				case Keys.Left: // Left Arrow
					// SHIFT indicates selection, CTRL indicates words instead of characters
					{
						LastMacro macroType = controlDown ?
						(shiftDown ? LastMacro.LeftArrowWordSelection : LastMacro.LeftArrowWord) :
						(shiftDown ? LastMacro.LeftArrowCharSelection : LastMacro.LeftArrowChar);

						RecordMove(macroType, "Left", controlDown ? MoveScope.Word : MoveScope.Character, shiftDown);
					}
					break;

				case Keys.Right: // Right Arrow
					// SHIFT indicates selection, CTRL indicates words instead of characters
					{
						LastMacro macroType = controlDown ?
						(shiftDown ? LastMacro.RightArrowWordSelection : LastMacro.RightArrowWord) :
						(shiftDown ? LastMacro.RightArrowCharSelection : LastMacro.RightArrowChar);

						RecordMove(macroType, "Right", controlDown ? MoveScope.Word : MoveScope.Character, shiftDown);
					}
					break;

				case Keys.Up: // Up Arrow
					// SHIFT indicates selection, CTRL indicates paragraphs instead of lines
					{
						LastMacro macroType = controlDown ?
						(shiftDown ? LastMacro.UpArrowParaSelection : LastMacro.UpArrowPara) :
						(shiftDown ? LastMacro.UpArrowLineSelection : LastMacro.UpArrowLine);

						RecordMove(macroType, "Up", controlDown ? MoveScope.Paragraph : MoveScope.Line, shiftDown);
					}
					break;

				case Keys.Down: // Down Arrow
					// SHIFT indicates selection, CTRL indicates paragraphs instead of lines
					{
						LastMacro macroType = controlDown ?
						(shiftDown ? LastMacro.DownArrowParaSelection : LastMacro.DownArrowPara) :
						(shiftDown ? LastMacro.DownArrowLineSelection : LastMacro.DownArrowLine);

						RecordMove(macroType, "Down", controlDown ? MoveScope.Paragraph : MoveScope.Line, shiftDown);
					}
					break;

				case Keys.Prior: // Page Up
				case Keys.Next: // Page Down
					macroString += "ActiveDocument.Object.Move";

					if (System.Windows.Forms.Keys.Prior == currentKey)
					{
						macroString += "Up";
					}
					else
					{
						macroString += "Down";
					}

					macroString += "(" + (int) (controlDown ? tom.tomConstants.tomWindow : tom.tomConstants.tomScreen) + ", 1, " + (int) (shiftDown ? tom.tomConstants.tomExtend : tom.tomConstants.tomMove) + ")";

					recorder.RecordLine(macroString);
					break;

				case Keys.End:
				case Keys.Home:
					macroString += "ActiveDocument.Object.";

					if (System.Windows.Forms.Keys.End == currentKey)
					{
						macroString += "EndKey";
					}
					else
					{
						macroString += "HomeKey";
					}

					macroString += "(" + (int) (controlDown ? tom.tomConstants.tomStory : tom.tomConstants.tomLine) + ", " + (int) (shiftDown ? tom.tomConstants.tomExtend : tom.tomConstants.tomMove) + ")";

					recorder.RecordLine(macroString);
					break;

				case Keys.Insert:
					// Note that the CTRL completely disables this command.  Also the SHIFT+INSERT
					// actually generates a WM_PASTE message rather than a WM_KEYDOWN
					if (!controlDown)
					{
						macroString = "ActiveDocument.Object.Flags = ActiveDocument.Object.Flags Xor ";
						macroString += (int) tom.tomConstants.tomSelOvertype;
						recorder.RecordLine(macroString);
					}
					break;
			}
		}
	}
}