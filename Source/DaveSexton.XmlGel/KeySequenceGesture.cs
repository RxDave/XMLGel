using System.Windows.Input;

namespace DaveSexton.XmlGel
{
	internal class KeySequenceGesture : KeyGesture
	{
		private readonly Key first;
		private readonly Key second;
		private readonly ModifierKeys modifiers;
		private bool matchedFirst;

		public KeySequenceGesture(Key first, Key second, ModifierKeys modifiers)
			: base(Key.None, modifiers)
		{
			this.first = first;
			this.second = second;
			this.modifiers = modifiers;
		}

		public override bool Matches(object targetElement, InputEventArgs inputEventArgs)
		{
			var keyEventArgs = inputEventArgs as KeyEventArgs;

			if (keyEventArgs != null
				&& !keyEventArgs.IsRepeat
				&& keyEventArgs.Key != Key.LeftCtrl
				&& keyEventArgs.Key != Key.RightCtrl
				&& keyEventArgs.Key != Key.LeftAlt
				&& keyEventArgs.Key != Key.RightAlt
				&& keyEventArgs.Key != Key.LeftShift
				&& keyEventArgs.Key != Key.RightShift
				&& keyEventArgs.Key != Key.LWin
				&& keyEventArgs.Key != Key.RWin)
			{
				if (!matchedFirst)
				{
					if (keyEventArgs.Key == first && Keyboard.Modifiers == modifiers)
					{
						matchedFirst = true;
					}
				}
				else
				{
					matchedFirst = false;

					if (keyEventArgs.Key == second && Keyboard.Modifiers == modifiers)
					{
						return true;
					}
				}
			}

			return false;
		}
	}
}