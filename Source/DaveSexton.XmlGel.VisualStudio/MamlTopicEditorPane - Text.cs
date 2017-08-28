using System;
using System.Collections;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Windows.Forms;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using tom;

namespace DaveSexton.XmlGel.VisualStudio
{
	/***************
	 * Not compiled
	 ***************/

	[ComSourceInterfaces(typeof(IVsTextViewEvents))]
	partial class MamlTopicEditorPane :
		IVsFindTarget,      //to implement find and replace capabilities within the editor
		IVsTextImage,       //to support find and replace in a text image
		IVsTextSpanSet,     //to support find and replace in a text image
		IVsTextBuffer,      //needed for Find and Replace to work appropriately
		IVsTextView,        //needed for Find and Replace to work appropriately
		IVsCodeWindow,      //needed for Find and Replace to work appropriately
		IVsTextLines       //needed for Find and Replace to work appropriately
	{
		private object findState;
		private bool lockImage;
		private IVsTextImage spTextImage;

		#region IVsFindTarget Members

		/// <summary>
		/// Return the object that was requested
		/// </summary>
		/// <param name="propid">Id of the requested object</param>
		/// <param name="pvar">Object returned</param>
		/// <returns>HResult</returns>
		int IVsFindTarget.GetProperty(uint propid, out object pvar)
		{
			pvar = null;

			switch (propid)
			{
				case (uint) __VSFTPROPID.VSFTPROPID_DocName:
					{
						// Return a copy of the file name
						pvar = fileName;
						break;
					}
				case (uint) __VSFTPROPID.VSFTPROPID_InitialPattern:
				case (uint) __VSFTPROPID.VSFTPROPID_InitialPatternAggressive:
					{
						// Return the selected text
						GetInitialSearchString(out pvar);
						//pvar = editorControl.TextBox.SelectedText;
						break;
					}
				case (uint) __VSFTPROPID.VSFTPROPID_WindowFrame:
					{
						// Return the Window frame
						pvar = (IVsWindowFrame) GetService(typeof(SVsWindowFrame));
						break;
					}
				case (uint) __VSFTPROPID.VSFTPROPID_IsDiskFile:
					{
						// We currently assume the file is on disk
						pvar = true;
						break;
					}
				default:
					{
						return VSConstants.E_NOTIMPL;
					}
			}

			return VSConstants.S_OK;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="grfOptions"></param>
		/// <param name="ppSpans"></param>
		/// <param name="ppTextImage"></param>
		int IVsFindTarget.GetSearchImage(uint grfOptions, IVsTextSpanSet[] ppSpans, out IVsTextImage ppTextImage)
		{
			//set the IVsTextSpanSet object
			if (null != ppSpans && ppSpans.Length > 0)
			{
				ppSpans[0] = (IVsTextSpanSet) this;
			}

			//set the IVsTextImage object
			ppTextImage = (IVsTextImage) this;

			//attach this text image to the span
			if (null != ppSpans && ppSpans.Length > 0)
			{
				ErrorHandler.ThrowOnFailure(ppSpans[0].AttachTextImage(ppTextImage));
			}

			return VSConstants.S_OK;
		}

		/// <summary>
		/// Retrieve a previously stored object
		/// </summary>
		/// <returns>The object that is being asked</returns>
		int IVsFindTarget.GetFindState(out object ppunk)
		{
			ppunk = findState;
			return VSConstants.S_OK;
		}

		/// <summary>
		/// Search for the string in the text of our editor.
		/// Options specify how we do the search. No need to implement this since we implement IVsTextImage
		/// </summary>
		/// <param name="pszSearch">Search string</param>
		/// <param name="grfOptions">Search options</param>
		/// <param name="fResetStartPoint">Is this a new search?</param>
		/// <param name="pHelper">We are not using it</param>
		/// <param name="pResult">True if we found the search string</param>
		int IVsFindTarget.Find(string pszSearch, uint grfOptions, int fResetStartPoint, IVsFindHelper pHelper, out uint pResult)
		{
			pResult = 0;

			return VSConstants.E_NOTIMPL;
		}

		/// <summary>
		/// Bring the focus to a specific position in the document
		/// </summary>
		/// <param name="pts">Location where to move the cursor to</param>
		int IVsFindTarget.NavigateTo(TextSpan[] pts)
		{
			int hr = VSConstants.S_OK;

			// Activate the window
			IVsWindowFrame frame = (IVsWindowFrame) GetService(typeof(SVsWindowFrame));
			if (frame != null)
				hr = frame.Show();
			else
				return VSConstants.E_NOTIMPL;

			// Now navigate to the specified location (if any)
			if (ErrorHandler.Succeeded(hr) && (null != pts) && (pts.Length > 0))
			{
				// first set start location
				int NewPosition = editorControl.TextBox.GetFirstCharIndexFromLine(pts[0].iStartLine);
				NewPosition += pts[0].iStartIndex;
				if (NewPosition > editorControl.TextBox.Text.Length)
					NewPosition = editorControl.TextBox.Text.Length;
				editorControl.TextBox.SelectionStart = NewPosition;

				// now set the length of the selection
				NewPosition = editorControl.TextBox.GetFirstCharIndexFromLine(pts[0].iEndLine);
				NewPosition += pts[0].iEndIndex;
				if (NewPosition > editorControl.TextBox.Text.Length)
					NewPosition = editorControl.TextBox.Text.Length;
				int length = NewPosition - editorControl.TextBox.SelectionStart;
				if (length >= 0)
					editorControl.TextBox.SelectionLength = length;
				else
					editorControl.TextBox.SelectionLength = 0;
			}
			return hr;
		}

		/// <summary>
		/// Get current cursor location
		/// </summary>
		/// <param name="pts">Current location</param>
		/// <returns>HResult</returns>
		int IVsFindTarget.GetCurrentSpan(TextSpan[] pts)
		{
			if (null == pts || 0 == pts.Length)
				return VSConstants.E_INVALIDARG;

			pts[0].iStartIndex = editorControl.GetColumnFromIndex(editorControl.TextBox.SelectionStart);
			pts[0].iEndIndex = editorControl.GetColumnFromIndex(editorControl.TextBox.SelectionStart + editorControl.TextBox.SelectionLength);
			pts[0].iStartLine = editorControl.TextBox.GetLineFromCharIndex(editorControl.TextBox.SelectionStart);
			pts[0].iEndLine = editorControl.TextBox.GetLineFromCharIndex(editorControl.TextBox.SelectionStart + editorControl.TextBox.SelectionLength);

			return VSConstants.S_OK;
		}

		/// <summary>
		/// Highlight a given text span. No need to implement
		/// </summary>
		/// <param name="pts"></param>
		/// <returns></returns>
		int IVsFindTarget.MarkSpan(TextSpan[] pts)
		{
			return VSConstants.E_NOTIMPL;
		}

		/// <summary>
		/// Replace a string in the text. No need to implement since we implement IVsTextImage
		/// </summary>
		/// <param name="pszSearch">string containing the search text</param>
		/// <param name="pszReplace">string containing the replacement text</param>
		/// <param name="grfOptions">Search options available</param>
		/// <param name="fResetStartPoint">flag to reset the search start point</param>
		/// <param name="pHelper">IVsFindHelper interface object</param>
		/// <param name="pfReplaced">returns whether replacement was successful or not</param>
		/// <returns></returns>
		int IVsFindTarget.Replace(string pszSearch, string pszReplace, uint grfOptions, int fResetStartPoint, IVsFindHelper pHelper, out int pfReplaced)
		{
			pfReplaced = 0;

			return VSConstants.E_NOTIMPL;
		}

		/// <summary>
		/// Store an object that will later be returned
		/// </summary>
		/// <returns>The object that is being stored</returns>
		int IVsFindTarget.SetFindState(object pUnk)
		{
			findState = pUnk;
			return VSConstants.S_OK;
		}


		/// <summary>
		/// This implementation does not use notification
		/// </summary>
		/// <param name="notification"></param>
		int IVsFindTarget.NotifyFindTarget(uint notification)
		{
			return VSConstants.S_OK;
		}

		/// <summary>
		/// Specify which search option we support.
		/// </summary>
		/// <param name="pfImage">Do we support IVsTextImage?</param>
		/// <param name="pgrfOptions">Supported options</param>
		int IVsFindTarget.GetCapabilities(bool[] pfImage, uint[] pgrfOptions)
		{
			// We do support IVsTextImage
			if (pfImage != null && pfImage.Length > 0)
				pfImage[0] = true;

			if (pgrfOptions != null && pgrfOptions.Length > 0)
			{
				pgrfOptions[0] = (uint) __VSFINDOPTIONS.FR_Backwards;        //Search backwards from the insertion point
				pgrfOptions[0] |= (uint) __VSFINDOPTIONS.FR_MatchCase;       //Match the case while searching
				pgrfOptions[0] |= (uint) __VSFINDOPTIONS.FR_WholeWord;       //Match whole word while searching
				pgrfOptions[0] |= (uint) __VSFINDOPTIONS.FR_Selection;       //Search in selected text only
				pgrfOptions[0] |= (uint) __VSFINDOPTIONS.FR_ActionMask;      //Find/Replace capabilities

				// Only support selection if something is selected
				if (editorControl == null || editorControl.TextBox.SelectionLength == 0)
					pgrfOptions[0] &= ~((uint) __VSFINDOPTIONS.FR_Selection);

				//if the file is read only, don't support replace
				if (editorControl == null || editorControl.TextBox.ReadOnly)
					pgrfOptions[0] &= ~((uint) __VSFINDOPTIONS.FR_Replace | (uint) __VSFINDOPTIONS.FR_ReplaceAll);
			}
			return VSConstants.S_OK;
		}

		/// <summary>
		/// Return the Screen coordinates of the matched string. No need to implement
		/// </summary>
		/// <param name="prc"></param>
		/// <returns></returns>
		int IVsFindTarget.GetMatchRect(RECT[] prc)
		{
			return VSConstants.E_NOTIMPL;
		}

		#endregion

		/// <summary>
		/// Function to return the string to be used in the "Find What" field of the find window. Will return
		/// null if no text is selected or if there are multiple lines of text selected.
		/// </summary>
		/// <param name="pvar">the string to be returned</param>
		private void GetInitialSearchString(out object pvar)
		{
			//If no text is selected, return null
			if (0 == editorControl.TextBox.SelectionLength)
			{
				pvar = null;
				return;
			}

			//Now check if multiple lines have been selected
			int endIndex = editorControl.TextBox.SelectionStart + editorControl.TextBox.SelectionLength;
			int endline = editorControl.TextBox.GetLineFromCharIndex(endIndex);
			int startline = editorControl.TextBox.GetLineFromCharIndex(editorControl.TextBox.SelectionStart);
			if (startline != endline)
			{
				pvar = null;
				return;
			}

			pvar = editorControl.TextBox.SelectedText;
		}

		#region IVsTextImage members

		/// <summary>
		/// To return the number of characters in the text image. No need to implement
		/// </summary>
		/// <param name="pcch">contain the number of characters</param>
		/// <returns></returns>
		int IVsTextImage.GetCharSize(out int pcch)
		{
			pcch = 0;
			return VSConstants.E_NOTIMPL;
		}

		/// <summary>
		/// To return the number of lines in the text image
		/// </summary>
		/// <param name="pcLines">pointer to the number of lines in the text image</param>
		/// <returns>S_OK</returns>
		int IVsTextImage.GetLineSize(out int pcLines)
		{
			//get the number of the lines in the control
			int len = editorControl.TextBox.Lines.Length;
			pcLines = len;

			return VSConstants.S_OK;
		}

		/// <summary>
		/// To return the buffer address of the given text address. No need to implement
		/// </summary>
		/// <param name="ta">contains the TextAddress</param>
		/// <param name="piOffset">will contain the ofset from the start of the buffer</param>
		/// <returns></returns>
		int IVsTextImage.GetOffsetOfTextAddress(TextAddress ta, out int piOffset)
		{
			piOffset = 0;
			return VSConstants.E_NOTIMPL;
		}

		/// <summary>
		/// To return the text address of the given buffer address. No need to implement
		/// </summary>
		/// <param name="iOffset">offset from the start of the buffer</param>
		/// <param name="pta">will contain the TextAddress</param>
		/// <returns></returns>
		int IVsTextImage.GetTextAddressOfOffset(int iOffset, TextAddress[] pta)
		{
			return VSConstants.E_NOTIMPL;
		}

		/// <summary>
		/// Notification for a text span replacement
		/// </summary>
		/// <param name="dwFlags">Flags used for the replace</param>
		/// <param name="pts">Contains the TextSpan to be replaced</param>
		/// <param name="cch">count of characters in pchText</param>
		/// <param name="pchText">the replacement text</param>
		/// <param name="ptsChanged">TextSpan of the replaced text</param>
		/// <returns></returns>
		int IVsTextImage.Replace(uint dwFlags,
														 TextSpan[] pts,
														 int cch,
														 string pchText,
														 TextSpan[] ptsChanged
				)
		{
			//pts contains the span of the item which is to be replaced
			if (null == pts || 0 == pts.Length)
				return VSConstants.E_INVALIDARG;

			if (null == pchText)
				return VSConstants.E_INVALIDARG;

			// first set start location
			int NewPosition = editorControl.TextBox.GetFirstCharIndexFromLine(pts[0].iStartLine);
			NewPosition += pts[0].iStartIndex;
			if (NewPosition > editorControl.TextBox.Text.Length)
				NewPosition = editorControl.TextBox.Text.Length;
			editorControl.TextBox.SelectionStart = NewPosition;

			// now set the length of the selection
			NewPosition = editorControl.TextBox.GetFirstCharIndexFromLine(pts[0].iEndLine);
			NewPosition += pts[0].iEndIndex;
			if (NewPosition > editorControl.TextBox.Text.Length)
				NewPosition = editorControl.TextBox.Text.Length;
			int length = NewPosition - editorControl.TextBox.SelectionStart;
			if (length >= 0)
				editorControl.TextBox.SelectionLength = length;
			else
				editorControl.TextBox.SelectionLength = 0;

			//replace the text
			editorControl.TextBox.SelectedText = pchText;

			if ((dwFlags & (uint) __VSFINDOPTIONS.FR_Backwards) == 0)
			{
				// In case of forward search we have to place the insertion point at the
				// end of the new text, so it will be skipped during the next call to Find.
				editorControl.TextBox.SelectionStart += editorControl.TextBox.SelectionLength;
			}
			else
			{
				// If the search is backward, then set the end postion at the
				// beginning of the new text.
				editorControl.TextBox.SelectionLength = 0;
			}

			//set the ptsChanged to the TextSpan of the replaced text
			if (null != ptsChanged && ptsChanged.Length > 0)
			{
				ptsChanged[0].iStartIndex = editorControl.GetColumnFromIndex(editorControl.TextBox.SelectionStart);
				ptsChanged[0].iEndIndex = editorControl.GetColumnFromIndex(editorControl.TextBox.SelectionStart + editorControl.TextBox.SelectionLength);
				ptsChanged[0].iStartLine = editorControl.TextBox.GetLineFromCharIndex(editorControl.TextBox.SelectionStart);
				ptsChanged[0].iEndLine = editorControl.TextBox.GetLineFromCharIndex(editorControl.TextBox.SelectionStart + editorControl.TextBox.SelectionLength);
			}

			return VSConstants.S_OK;
		}

		/// <summary>
		/// To return the number of characters in a TextSpan
		/// </summary>
		/// <param name="pts">The TextSpan structure</param>
		/// <param name="pcch">will contain the number of characters</param>
		/// <returns></returns>
		int IVsTextImage.GetSpanLength(TextSpan[] pts, out int pcch)
		{
			pcch = 0;
			if (null == pts || 0 == pts.Length)
				return VSConstants.E_INVALIDARG;

			int startIndex = editorControl.GetIndexFromLineAndColumn(pts[0].iStartLine, pts[0].iStartIndex);
			if (startIndex < 0)
				return VSConstants.E_INVALIDARG;

			int endIndex = editorControl.GetIndexFromLineAndColumn(pts[0].iEndLine, pts[0].iEndIndex);
			if (endIndex < 0)
				return VSConstants.E_INVALIDARG;

			pcch = Math.Abs(endIndex - startIndex);

			return VSConstants.S_OK;
		}

		/// <summary>
		/// to return the text of a TextSpan as a BSTR
		/// </summary>
		/// <param name="pts">the TextSpan structure</param>
		/// <param name="pbstrText">the BSTR text</param>
		/// <returns></returns>
		int IVsTextImage.GetTextBSTR(TextSpan[] pts, out string pbstrText)
		{
			pbstrText = null;
			return VSConstants.S_OK;
		}

		/// <summary>
		/// To return the text of a TextSpan. No need to implement
		/// </summary>
		/// <param name="pts">TextSpan structure</param>
		/// <param name="cch">number of characters to return</param>
		/// <param name="psz">will contain the text</param>
		/// <returns></returns>
		int IVsTextImage.GetText(TextSpan[] pts, int cch, ushort[] psz)
		{
			return VSConstants.E_NOTIMPL;
		}

		/// <summary>
		/// To return the length of a particular line
		/// </summary>
		/// <param name="iLine">zero based line number</param>
		/// <param name="piLength">will contain the length</param>
		/// <returns></returns>
		int IVsTextImage.GetLineLength(int iLine, out int piLength)
		{
			int numberOfLines = 0;
			piLength = 0;
			ErrorHandler.ThrowOnFailure(((IVsTextImage) this).GetLineSize(out numberOfLines));

			if (iLine < 0 || iLine > numberOfLines - 1)
			{
				return VSConstants.E_INVALIDARG;
			}
			piLength = editorControl.TextBox.Lines[iLine].Length;

			return VSConstants.S_OK;
		}

		/// <summary>
		/// To provide line oriented access to the text buffer
		/// </summary>
		/// <param name="grfGet">flags containing information on the line to get</param>
		/// <param name="iLine">zero based line number</param>
		/// <param name="iStartIndex">starting character index of the line</param>
		/// <param name="iEndIndex">ending character index of the line</param>
		/// <param name="pLineData">Will contain the filled LINEDATA structure</param>
		/// <returns></returns>
		[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		int IVsTextImage.GetLine(uint grfGet,
																		int iLine,
																		int iStartIndex,
																		int iEndIndex,
																		LINEDATAEX[] pLineData
				)
		{
			if (null == pLineData || 0 == pLineData.Length)
				return VSConstants.E_INVALIDARG;

			//first initialize the Line Data object
			pLineData[0].iLength = 0;
			pLineData[0].pszText = IntPtr.Zero;
			pLineData[0].iEolType = EOLTYPE.eolCR;
			pLineData[0].pAttributes = IntPtr.Zero;
			pLineData[0].dwFlags = (ushort) LINEDATAEXFLAGS.ldfDefault;
			pLineData[0].dwReserved = 0;
			pLineData[0].pAtomicTextChain = IntPtr.Zero;

			int lineCount = editorControl.TextBox.Lines.Length;
			if ((iLine < 0) || (iLine >= lineCount) || (iStartIndex < 0) || (iEndIndex < 0) ||
					(iStartIndex > iEndIndex))
			{
				return VSConstants.E_INVALIDARG;
			}

			string lineText = editorControl.TextBox.Lines[iLine];
			// If the line is empty then do not attempt to calculate the span in the normal way; just return.
			if (string.IsNullOrEmpty(lineText) && iStartIndex == 0 && iEndIndex == 0)
				return VSConstants.S_OK;
			int lineLength = lineText.Length;

			//Error if startIndex is greater than the line length
			if (iStartIndex >= lineLength || iEndIndex >= lineLength)
				return VSConstants.E_INVALIDARG;

			int spanLength = iEndIndex - iStartIndex + 1;

			//Error in arguments if the span length is greater than the line length
			if (spanLength > lineLength)
				return VSConstants.E_INVALIDARG;

			//If we are looking for a subset of the line i.e. a line span
			if (0 != (grfGet & (uint) GLDE_FLAGS.gldeSubset))
			{
				pLineData[0].iLength = spanLength;
				string spanText = lineText.Substring(iStartIndex, spanLength);
				pLineData[0].pszText = new IntPtr();
				pLineData[0].pszText = Marshal.StringToCoTaskMemAuto(spanText);
			}
			//else we need to return the complete line
			else
			{
				pLineData[0].iLength = lineLength;
				pLineData[0].pszText = new IntPtr();
				pLineData[0].pszText = Marshal.StringToCoTaskMemAuto(lineText);
			}

			return VSConstants.S_OK;

		}

		/// <summary>
		/// Release the LINEDATAEX structure
		/// </summary>
		/// <param name="pLineData">pointer to the LINEDATAEX structure</param>
		/// <returns></returns>
		[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		int IVsTextImage.ReleaseLine(LINEDATAEX[] pLineData)
		{
			if (null == pLineData || 0 == pLineData.Length)
				return VSConstants.E_INVALIDARG;

			//clear the Line Data object
			pLineData[0].iLength = 0;
			Marshal.FreeCoTaskMem(pLineData[0].pszText);
			pLineData[0].iEolType = EOLTYPE.eolNONE;
			pLineData[0].pAttributes = IntPtr.Zero;
			pLineData[0].dwFlags = (ushort) LINEDATAEXFLAGS.ldfDefault;
			pLineData[0].dwReserved = 0;
			pLineData[0].pAtomicTextChain = IntPtr.Zero;

			return VSConstants.S_OK;
		}

		/// <summary>
		/// Registers the environment to retrieve notifications of text image changes.
		/// </summary>
		/// <param name="pSink">Object requesting notification on text image changes</param>
		/// <param name="pCookie">Handle for the event sink</param>
		/// <returns></returns>
		int IVsTextImage.AdviseTextImageEvents(IVsTextImageEvents pSink, out uint pCookie)
		{
			//We don't use this
			pCookie = 0;
			return VSConstants.S_OK;
		}

		/// <summary>
		/// Cancels notification for text image changes
		/// </summary>
		/// <param name="Cookie">Handle to the event sink</param>
		/// <returns></returns>
		int IVsTextImage.UnadviseTextImageEvents(uint Cookie)
		{
			//We don't use this
			return VSConstants.S_OK;
		}

		/// <summary>
		/// Notification from the environment that it is locking an image
		/// </summary>
		/// <param name="grfLock">the locking flag</param>
		/// <returns></returns>
		int IVsTextImage.LockImage(uint grfLock)
		{
			//We only allow one reader/writer
			if (!lockImage)
			{
				lockImage = true;
				return VSConstants.S_OK;
			}
			else
				return VSConstants.E_FAIL;
		}

		/// <summary>
		/// Notification from the environment that the text image is not in use
		/// </summary>
		/// <param name="grfLock">the locking flag</param>
		/// <returns></returns>
		int IVsTextImage.UnlockImage(uint grfLock)
		{
			lockImage = false;
			return VSConstants.S_OK;
		}

		#endregion

		#region IVsTextSpanSet Members

		/// <summary>
		/// The environment uses this to get a text image
		/// </summary>
		/// <param name="pText">Pointer to the text image</param>
		/// <returns></returns>
		int IVsTextSpanSet.AttachTextImage(object pText)
		{
			if (null == pText)
				return VSConstants.E_INVALIDARG;

			if (null != spTextImage)
			{
				if (spTextImage.Equals(pText))
					return VSConstants.S_OK;
			}

			spTextImage = (IVsTextImage) this;

			//get the number of lines in the Text Image
			int lineCount = 0;
			ErrorHandler.ThrowOnFailure(spTextImage.GetLineSize(out lineCount));

			//create a text span for the entire text image
			TextSpan textSpan = new TextSpan();
			textSpan.iStartLine = 0;
			textSpan.iStartIndex = 0;
			textSpan.iEndLine = 0;

			//get the length of the last line
			int lastLineLength = 0;
			if (lineCount > 0)
			{
				textSpan.iEndLine = lineCount - 1;
				ErrorHandler.ThrowOnFailure(spTextImage.GetLineLength(lineCount - 1, out lastLineLength));
			}

			//set the end index corresponding to the last line length
			textSpan.iEndIndex = lastLineLength;

			//add it to the text span array
			textSpanArray.Add(textSpan);

			return VSConstants.S_OK;
		}

		/// <summary>
		/// To Release a text image
		/// </summary>
		/// <returns></returns>
		int IVsTextSpanSet.Detach()
		{
			spTextImage = null;
			textSpanArray.RemoveRange(0, textSpanArray.Count);

			return VSConstants.S_OK;
		}

		/// <summary>
		/// Not needed to be implemented
		/// </summary>
		/// <returns></returns>
		int IVsTextSpanSet.SuspendTracking()
		{
			return VSConstants.E_NOTIMPL;
		}

		/// <summary>
		/// Not needed to be implemented
		/// </summary>
		/// <returns></returns>
		int IVsTextSpanSet.ResumeTracking()
		{
			return VSConstants.E_NOTIMPL;
		}

		/// <summary>
		/// To add the TExtSpan to an array at the specified location
		/// </summary>
		/// <param name="cEl">the index to insert</param>
		/// <param name="pSpan">the TextSpan object</param>
		/// <returns></returns>
		int IVsTextSpanSet.Add(int cEl, TextSpan[] pSpan)
		{
			if (null == pSpan || 0 == pSpan.Length)
				return VSConstants.E_INVALIDARG;

			if (cEl < 0)
				return VSConstants.E_INVALIDARG;

			textSpanArray.Insert(cEl, pSpan[0]);
			return VSConstants.S_OK;
		}

		/// <summary>
		/// Returns the number of text spans in the array
		/// </summary>
		/// <param name="pcel">will contain the count</param>
		/// <returns></returns>
		int IVsTextSpanSet.GetCount(out int pcel)
		{
			pcel = textSpanArray.Count;
			return VSConstants.S_OK;
		}

		/// <summary>
		/// Return the text span at the requested index
		/// </summary>
		/// <param name="iEl">the index</param>
		/// <param name="pSpan">will contain the TextSpan returned</param>
		/// <returns></returns>
		int IVsTextSpanSet.GetAt(int iEl, TextSpan[] pSpan)
		{
			if (iEl >= textSpanArray.Count || iEl < 0)
				return VSConstants.E_INVALIDARG;

			if (null == pSpan || 0 == pSpan.Length)
				return VSConstants.E_INVALIDARG;

			pSpan[0] = (TextSpan) textSpanArray[iEl];

			return VSConstants.S_OK;
		}

		/// <summary>
		/// Clear up the text span array
		/// </summary>
		/// <returns></returns>
		int IVsTextSpanSet.RemoveAll()
		{
			textSpanArray.RemoveRange(0, textSpanArray.Count);
			return VSConstants.S_OK;
		}

		/// <summary>
		/// No need to implement this
		/// </summary>
		/// <param name="sortOptions"></param>
		/// <returns></returns>
		int IVsTextSpanSet.Sort(uint SortOptions)
		{
			return VSConstants.E_NOTIMPL;
		}

		/// <summary>
		/// No need to implement this
		/// </summary>
		/// <param name="pEnum"></param>
		/// <returns></returns>
		int IVsTextSpanSet.AddFromEnum(IVsEnumTextSpans pEnum)
		{
			return VSConstants.E_NOTIMPL;
		}
		#endregion

		#region IVsTextBuffer Members

		/*The IVsTextBuffer interface is used to provide just general information about the Text Buffer used
        by the Editor. For our sample this is just provided so that the find in files scenario will work 
        properly.  It isn't necessary to implement most of the methods for this
        scenario to work correctly.*/

		public int GetLanguageServiceID(out Guid pguidLangService)
		{
			pguidLangService = Guid.Empty;
			return VSConstants.E_NOTIMPL;
		}

		public int GetLastLineIndex(out int piLine, out int piIndex)
		{
			//Initialize the parameters first
			piLine = 0;
			piIndex = 0;

			int totalLines = editorControl.TextBox.Lines.Length;
			if (totalLines > 0)
				piLine = totalLines - 1;
			int lineLen = editorControl.TextBox.Lines[piLine].Length;
			piIndex = lineLen >= 1 ? lineLen - 1 : lineLen;

			return VSConstants.S_OK;
		}

		public int GetLengthOfLine(int iLine, out int piLength)
		{
			piLength = 0;
			int totalLines = editorControl.TextBox.Lines.Length;

			if (iLine < 0 || iLine >= totalLines)
				return VSConstants.E_INVALIDARG;

			piLength = editorControl.TextBox.Lines[iLine].Length;

			return VSConstants.S_OK;
		}

		public int GetLineCount(out int piLineCount)
		{
			piLineCount = editorControl.TextBox.Lines.Length;
			return VSConstants.E_NOTIMPL;
		}

		public int GetLineIndexOfPosition(int iPosition, out int piLine, out int piColumn)
		{
			//Initialize the parameters first
			piLine = 0;
			piColumn = 0;

			return VSConstants.E_NOTIMPL;
		}

		public int GetPositionOfLine(int iLine, out int piPosition)
		{
			piPosition = 0;

			return VSConstants.E_NOTIMPL;
		}

		public int GetPositionOfLineIndex(int iLine, int iIndex, out int piPosition)
		{
			piPosition = 0;

			return VSConstants.E_NOTIMPL;
		}

		public int GetSize(out int piLength)
		{
			piLength = 0;
			return VSConstants.E_NOTIMPL;
		}

		public int GetStateFlags(out uint pdwReadOnlyFlags)
		{
			pdwReadOnlyFlags = 0;
			return VSConstants.E_NOTIMPL;
		}

		public int GetUndoManager(out IOleUndoManager ppUndoManager)
		{
			ppUndoManager = null;
			return VSConstants.E_NOTIMPL;
		}

		public int InitializeContent(string pszText, int iLength)
		{
			return VSConstants.E_NOTIMPL;
		}

		public int LockBuffer()
		{
			return VSConstants.E_NOTIMPL;
		}

		public int LockBufferEx(uint dwFlags)
		{
			return VSConstants.E_NOTIMPL;
		}

		public int Reload(int fUndoable)
		{
			return VSConstants.E_NOTIMPL;
		}

		public int SetLanguageServiceID(ref Guid guidLangService)
		{
			return VSConstants.E_NOTIMPL;
		}

		public int SetStateFlags(uint dwReadOnlyFlags)
		{
			return VSConstants.E_NOTIMPL;
		}

		public int UnlockBuffer()
		{
			return VSConstants.E_NOTIMPL;
		}

		public int UnlockBufferEx(uint dwFlags)
		{
			return VSConstants.E_NOTIMPL;
		}

		public int Reserved1()
		{
			return VSConstants.E_NOTIMPL;
		}

		public int Reserved2()
		{
			return VSConstants.E_NOTIMPL;
		}

		public int Reserved3()
		{
			return VSConstants.E_NOTIMPL;
		}

		public int Reserved4()
		{
			return VSConstants.E_NOTIMPL;
		}

		public int Reserved5()
		{
			return VSConstants.E_NOTIMPL;
		}

		public int Reserved6()
		{
			return VSConstants.E_NOTIMPL;
		}

		public int Reserved7()
		{
			return VSConstants.E_NOTIMPL;
		}

		public int Reserved8()
		{
			return VSConstants.E_NOTIMPL;
		}

		public int Reserved9()
		{
			return VSConstants.E_NOTIMPL;
		}

		public int Reserved10()
		{
			return VSConstants.E_NOTIMPL;
		}
		#endregion

		#region IVsTextView Members

		/*This interface contains methods to manage the Text View i.e. the editor window which is shown to
        the user. For our sample this is just provided so that the find in files scenario will work 
        properly.  It isn't necessary to implement most of the methods for this
        scenario to work correctly.*/

		int IVsTextView.AddCommandFilter(IOleCommandTarget pNewCmdTarg, out IOleCommandTarget ppNextCmdTarg)
		{
			ppNextCmdTarg = null;
			return VSConstants.E_NOTIMPL;
		}

		int IVsTextView.CenterColumns(int iLine, int iLeftCol, int iColCount)
		{
			return VSConstants.E_NOTIMPL;
		}

		int IVsTextView.CenterLines(int iTopLine, int iCount)
		{
			return VSConstants.E_NOTIMPL;
		}

		int IVsTextView.ClearSelection(int fMoveToAnchor)
		{
			return VSConstants.E_NOTIMPL;
		}

		int IVsTextView.CloseView()
		{
			return VSConstants.E_NOTIMPL;
		}

		int IVsTextView.EnsureSpanVisible(TextSpan span)
		{
			return VSConstants.E_NOTIMPL;
		}

		int IVsTextView.GetBuffer(out IVsTextLines ppBuffer)
		{
			ppBuffer = (IVsTextLines) this;
			return VSConstants.S_OK;
		}

		int IVsTextView.GetCaretPos(out int piLine, out int piColumn)
		{
			piLine = 0;
			piColumn = 0;
			return VSConstants.E_NOTIMPL;
		}

		int IVsTextView.GetLineAndColumn(int iPos, out int piLine, out int piIndex)
		{
			piLine = 0;
			piIndex = 0;
			return VSConstants.E_NOTIMPL;
		}

		int IVsTextView.GetLineHeight(out int piLineHeight)
		{
			piLineHeight = 0;
			return VSConstants.E_NOTIMPL;
		}

		int IVsTextView.GetNearestPosition(int iLine, int iCol, out int piPos, out int piVirtualSpaces)
		{
			piPos = 0;
			piVirtualSpaces = 0;
			return VSConstants.E_NOTIMPL;
		}

		int IVsTextView.GetPointOfLineColumn(int iLine, int iCol, Microsoft.VisualStudio.OLE.Interop.POINT[] ppt)
		{
			return VSConstants.E_NOTIMPL;
		}

		int IVsTextView.GetScrollInfo(int iBar, out int piMinUnit, out int piMaxUnit,
										out int piVisibleUnits, out int piFirstVisibleUnit)
		{
			piMinUnit = 0;
			piMaxUnit = 0;
			piVisibleUnits = 0;
			piFirstVisibleUnit = 0;
			return VSConstants.E_NOTIMPL;
		}

		int IVsTextView.GetSelectedText(out string pbstrText)
		{
			pbstrText = null;
			return VSConstants.E_NOTIMPL;
		}

		int IVsTextView.GetSelection(out int piAnchorLine,
								out int piAnchorCol,
								out int piEndLine,
								out int piEndCol)
		{
			piAnchorLine = 0;
			piAnchorCol = 0;
			piEndLine = 0;
			piEndCol = 0;
			return VSConstants.E_NOTIMPL;
		}

		int IVsTextView.GetSelectionDataObject(out Microsoft.VisualStudio.OLE.Interop.IDataObject ppIDataObject)
		{
			ppIDataObject = null;
			return VSConstants.E_NOTIMPL;
		}

		TextSelMode IVsTextView.GetSelectionMode()
		{
			return TextSelMode.SM_STREAM;
		}

		int IVsTextView.GetSelectionSpan(TextSpan[] pSpan)
		{
			return VSConstants.E_NOTIMPL;
		}

		int IVsTextView.GetTextStream(int iTopLine,
								int iTopCol,
								int iBottomLine,
								int iBottomCol,
								out string pbstrText
				)
		{
			pbstrText = null;
			return VSConstants.E_NOTIMPL;
		}

		IntPtr IVsTextView.GetWindowHandle()
		{
			return IntPtr.Zero;
		}

		int IVsTextView.GetWordExtent(int iLine,
								int iCol,
								uint dwFlags,
								TextSpan[] pSpan
				)
		{
			return VSConstants.E_NOTIMPL;
		}

		int IVsTextView.HighlightMatchingBrace(uint dwFlags, uint cSpans, TextSpan[] rgBaseSpans)
		{
			return VSConstants.E_NOTIMPL;
		}

		int IVsTextView.Initialize(IVsTextLines pBuffer,
						IntPtr hwndParent,
						uint InitFlags,
						INITVIEW[] pInitView
				)
		{
			return VSConstants.E_NOTIMPL;
		}

		int IVsTextView.PositionCaretForEditing(int iLine, int cIndentLevels)
		{
			return VSConstants.E_NOTIMPL;
		}

		int IVsTextView.RemoveCommandFilter(IOleCommandTarget pCmdTarg)
		{
			return VSConstants.E_NOTIMPL;
		}

		int IVsTextView.ReplaceTextOnLine(int iLine,
						int iStartCol,
						int iCharsToReplace,
						string pszNewText,
						int iNewLen
				)
		{
			return VSConstants.E_NOTIMPL;
		}

		int IVsTextView.RestrictViewRange(int iMinLine, int iMaxLine, IVsViewRangeClient pClient)
		{
			return VSConstants.E_NOTIMPL;
		}

		int IVsTextView.SendExplicitFocus()
		{
			return VSConstants.E_NOTIMPL;
		}

		int IVsTextView.SetBuffer(IVsTextLines pBuffer)
		{
			return VSConstants.E_NOTIMPL;
		}

		int IVsTextView.SetCaretPos(int iLine, int iColumn)
		{
			return VSConstants.E_NOTIMPL;
		}

		int IVsTextView.SetScrollPosition(int iBar, int iFirstVisibleUnit)
		{
			return VSConstants.E_NOTIMPL;
		}

		int IVsTextView.SetSelection(int iAnchorLine, int iAnchorCol, int iEndLine, int iEndCol)
		{
			// first set start location
			int startPosition = editorControl.GetIndexFromLineAndColumn(iAnchorLine, iAnchorCol);
			if (startPosition < 0)
				return VSConstants.E_INVALIDARG;
			editorControl.TextBox.SelectionStart = startPosition;

			// now set the length of the selection
			int endPosition = editorControl.GetIndexFromLineAndColumn(iEndLine, iEndCol);
			if (endPosition < 0)
				return VSConstants.E_INVALIDARG;
			int length = endPosition - editorControl.TextBox.SelectionStart;
			if (length >= 0)
				editorControl.TextBox.SelectionLength = length;
			else
				editorControl.TextBox.SelectionLength = 0;
			return VSConstants.S_OK;
		}

		int IVsTextView.SetSelectionMode(TextSelMode iSelMode)
		{
			return VSConstants.E_NOTIMPL;
		}

		int IVsTextView.SetTopLine(int iBaseLine)
		{
			return VSConstants.E_NOTIMPL;
		}

		int IVsTextView.UpdateCompletionStatus(IVsCompletionSet pCompSet, uint dwFlags)
		{
			return VSConstants.E_NOTIMPL;
		}

		int IVsTextView.UpdateTipWindow(IVsTipWindow pTipWindow, uint dwFlags)
		{
			return VSConstants.E_NOTIMPL;
		}

		int IVsTextView.UpdateViewFrameCaption()
		{
			return VSConstants.E_NOTIMPL;
		}

		#endregion

		#region IVsTextViewEvents Members

		/*This interface is used as a notifier for the events that are occurring on the Text View.
        For our sample this is just provided so that the find in files scenario will work 
        properly.  It isn't necessary to implement any of the methods. */

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "pView")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "iOldLine")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "iNewLine")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "p")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "i")]
		public void OnChangeCaretLine(IVsTextView pView, int iNewLine, int iOldLine)
		{
			//Not Implemented
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "pView")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "iVisibleUnits")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "iMinUnit")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "iMaxUnits")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "iFirstVisibleUnit")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "iBar")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "p")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "i")]
		public void OnChangeScrollInfo(IVsTextView pView,
								int iBar,
								int iMinUnit,
								int iMaxUnits,
								int iVisibleUnits,
								int iFirstVisibleUnit
				)
		{
			//Not Implemented
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "p")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "pView")]
		public void OnKillFocus(IVsTextView pView)
		{
			//Not Implemented
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "pView")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "pBuffer")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "p")]
		public void OnSetBuffer(IVsTextView pView, IVsTextLines pBuffer)
		{
			//Not Implemented
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "p")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "pView")]
		public void OnSetFocus(IVsTextView pView)
		{
			//Not Implemented
		}
		#endregion

		#region IVsCodeWindow Members

		/* This interface is used for hosting of the views for a text buffer. Multiple views can be enclosed
        with the code window. 
        Since our editor support the LOGVIEWID_TextView logical view, we need to implement this interface 
        for find in files scenario to work properly.  
        It isn't necessary to implement most of the methods for this scenario to work correctly. */

		int IVsCodeWindow.GetPrimaryView(out IVsTextView ppView)
		{
			ppView = (IVsTextView) this;
			return VSConstants.S_OK;
		}

		int IVsCodeWindow.GetSecondaryView(out IVsTextView ppView)
		{
			ppView = null;
			return VSConstants.E_NOTIMPL;
		}

		int IVsCodeWindow.GetLastActiveView(out IVsTextView ppView)
		{
			ppView = (IVsTextView) this;
			return VSConstants.S_OK;
		}

		int IVsCodeWindow.Close()
		{
			return VSConstants.E_NOTIMPL;
		}

		int IVsCodeWindow.GetBuffer(out IVsTextLines ppBuffer)
		{
			ppBuffer = (IVsTextLines) this;
			return VSConstants.S_OK;
		}

		int IVsCodeWindow.GetEditorCaption(READONLYSTATUS dwReadOnly, out string pbstrEditorCaption)
		{
			pbstrEditorCaption = null;
			return VSConstants.E_NOTIMPL;
		}

		int IVsCodeWindow.GetViewClassID(out Guid pclsidView)
		{
			pclsidView = Guid.Empty;
			return VSConstants.E_NOTIMPL;
		}

		int IVsCodeWindow.SetBaseEditorCaption(string[] pszBaseEditorCaption)
		{
			return VSConstants.E_NOTIMPL;
		}

		int IVsCodeWindow.SetBuffer(IVsTextLines pBuffer)
		{
			return VSConstants.E_NOTIMPL;
		}

		int IVsCodeWindow.SetViewClassID(ref Guid clsidView)
		{
			return VSConstants.E_NOTIMPL;
		}

		#endregion

		#region IVsTextLines Members

		/* This interface is used for a line-oriented access to the contents of the text buffer. 
         For our sample all methods return E_NOTIMPL. This is needed for Find/Replace to work appropriately.
         The Caller just does a QueryInterface for this particular interface, but does not use any 
         of the methods available on the interface*/

		int IVsTextLines.AdviseTextLinesEvents(IVsTextLinesEvents pSink, out uint pdwCookie)
		{
			pdwCookie = 0;
			return VSConstants.E_NOTIMPL;
		}

		int IVsTextLines.UnadviseTextLinesEvents(uint dwCookie)
		{
			return VSConstants.E_NOTIMPL;
		}

		int IVsTextLines.CanReplaceLines(int iStartLine,
								int iStartIndex,
								int iEndLine,
								int iEndIndex,
								int iNewLen
				)
		{
			return VSConstants.E_NOTIMPL;
		}

		int IVsTextLines.CopyLineText(int iStartLine,
								int iStartIndex,
								int iEndLine,
								int iEndIndex,
								IntPtr pszBuf,
								ref int pcchBuf
				)
		{
			return VSConstants.E_NOTIMPL;
		}

		int IVsTextLines.CreateEditPoint(int iLine, int iIndex, out Object ppEditPoint)
		{
			ppEditPoint = null;
			return VSConstants.E_NOTIMPL;
		}

		int IVsTextLines.CreateLineMarker(int iMarkerType,
								int iStartLine,
								int iStartIndex,
								int iEndLine,
								int iEndIndex,
								IVsTextMarkerClient pClient,
								IVsTextLineMarker[] ppMarker
				)
		{
			return VSConstants.E_NOTIMPL;
		}

		int IVsTextLines.CreateTextPoint(int iLine, int iIndex, out Object ppTextPoint)
		{
			ppTextPoint = null;
			return VSConstants.E_NOTIMPL;
		}

		int IVsTextLines.EnumMarkers(int iStartLine,
								int iStartIndex,
								int iEndLine,
								int iEndIndex,
								int iMarkerType,
								uint dwFlags,
								out IVsEnumLineMarkers ppEnum
				)
		{
			ppEnum = null;
			return VSConstants.E_NOTIMPL;
		}

		int IVsTextLines.FindMarkerByLineIndex(int iMarkerType,
								int iStartingLine,
								int iStartingIndex,
								uint dwFlags,
								out IVsTextLineMarker ppMarker
				)
		{
			ppMarker = null;
			return VSConstants.E_NOTIMPL;
		}

		int IVsTextLines.GetMarkerData(int iTopLine, int iBottomLine, MARKERDATA[] pMarkerData)
		{
			return VSConstants.E_NOTIMPL;
		}

		int IVsTextLines.ReleaseMarkerData(MARKERDATA[] pMarkerData)
		{
			return VSConstants.E_NOTIMPL;
		}

		int IVsTextLines.GetLineData(int iLine, LINEDATA[] pLineData, MARKERDATA[] pMarkerData)
		{
			return VSConstants.E_NOTIMPL;
		}

		int IVsTextLines.ReleaseLineData(LINEDATA[] pLineData)
		{
			return VSConstants.E_NOTIMPL;
		}

		int IVsTextLines.GetLineDataEx(uint dwFlags,
								int iLine,
								int iStartIndex,
								int iEndIndex,
								LINEDATAEX[] pLineData,
								MARKERDATA[] pMarkerData
				)
		{
			return VSConstants.E_NOTIMPL;
		}

		int IVsTextLines.ReleaseLineDataEx(LINEDATAEX[] pLineData)
		{
			return VSConstants.E_NOTIMPL;
		}

		int IVsTextLines.IVsTextLinesReserved1(int iLine, LINEDATA[] pLineData, int fAttributes)
		{
			return VSConstants.E_NOTIMPL;
		}

		int IVsTextLines.GetLineText(int iStartLine,
								int iStartIndex,
								int iEndLine,
								int iEndIndex,
								out string pbstrBuf
				)
		{
			pbstrBuf = null;
			return VSConstants.E_NOTIMPL;
		}

		int IVsTextLines.GetPairExtents(TextSpan[] pSpanIn, TextSpan[] pSpanOut)
		{
			return VSConstants.E_NOTIMPL;
		}

		int IVsTextLines.ReplaceLines(int iStartLine,
								int iStartIndex,
								int iEndLine,
								int iEndIndex,
								IntPtr pszText,
								int iNewLen,
								TextSpan[] pChangedSpan
				)
		{
			return VSConstants.E_NOTIMPL;
		}

		int IVsTextLines.ReplaceLinesEx(uint dwFlags,
								int iStartLine,
								int iStartIndex,
								int iEndLine,
								int iEndIndex,
								IntPtr pszText,
								int iNewLen,
								TextSpan[] pChangedSpan
				)
		{
			return VSConstants.E_NOTIMPL;
		}

		int IVsTextLines.ReloadLines(int iStartLine,
								int iStartIndex,
								int iEndLine,
								int iEndIndex,
								IntPtr pszText,
								int iNewLen,
								TextSpan[] pChangedSpan
				)
		{
			return VSConstants.E_NOTIMPL;
		}

		#endregion
	}
}