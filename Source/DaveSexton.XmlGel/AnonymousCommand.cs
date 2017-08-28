using System;
using System.Diagnostics.Contracts;
using System.Windows.Input;

namespace DaveSexton.XmlGel.Maml
{
	internal sealed class AnonymousCommand : ICommand
	{
		public bool CanRaiseCanExecuteChanged
		{
			get
			{
				Contract.Ensures(Contract.Result<bool>() == (canExecuteChanged != null));

				return canExecuteChanged != null;
			}
		}

		private readonly Func<object, bool> canExecute;
		private readonly Action<object> execute;
		private readonly Action<EventHandler> addCanExecuteChanged, removeCanExecuteChanged;
		private readonly ThreadSafeEvent canExecuteChanged;

		public AnonymousCommand(Action<object> execute)
			: this(_ => true, execute, _ => { }, _ => { })
		{
			Contract.Requires(execute != null);
			Contract.Ensures(!CanRaiseCanExecuteChanged);
		}

		public AnonymousCommand(
			Func<object, bool> canExecute,
			Action<object> execute,
			Action<EventHandler> addCanExecuteChanged,
			Action<EventHandler> removeCanExecuteChanged)
		{
			Contract.Requires(canExecute != null);
			Contract.Requires(execute != null);
			Contract.Requires(addCanExecuteChanged != null);
			Contract.Requires(removeCanExecuteChanged != null);
			Contract.Ensures(!CanRaiseCanExecuteChanged);

			this.canExecute = canExecute;
			this.execute = execute;
			this.addCanExecuteChanged = addCanExecuteChanged;
			this.removeCanExecuteChanged = removeCanExecuteChanged;
		}

		public AnonymousCommand(
			Func<object, bool> canExecute,
			Action<object> execute)
		{
			Contract.Requires(canExecute != null);
			Contract.Requires(execute != null);
			Contract.Ensures(CanRaiseCanExecuteChanged);

			this.canExecute = canExecute;
			this.execute = execute;

			canExecuteChanged = new ThreadSafeEvent(this);
		}

		[ContractInvariantMethod]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
		private void ObjectInvariant()
		{
			Contract.Invariant(canExecute != null);
			Contract.Invariant(execute != null);
			Contract.Invariant(canExecuteChanged != null || addCanExecuteChanged != null);
			Contract.Invariant(canExecuteChanged != null || removeCanExecuteChanged != null);
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate",
			Justification = "It anonymously raises an event.")]
		public void RaiseCanExecuteChanged()
		{
			Contract.Requires(CanRaiseCanExecuteChanged);

			canExecuteChanged.RaiseEvent();
		}

		public bool CanExecute(object parameter)
		{
			return canExecute(parameter);
		}

		public void Execute(object parameter)
		{
			execute(parameter);
		}

		public event EventHandler CanExecuteChanged
		{
			add
			{
				if (canExecuteChanged == null)
				{
					addCanExecuteChanged(value);
				}
				else
				{
					canExecuteChanged.Event += value;
				}
			}
			remove
			{
				if (canExecuteChanged == null)
				{
					removeCanExecuteChanged(value);
				}
				else
				{
					canExecuteChanged.Event -= value;
				}
			}
		}

		private sealed class ThreadSafeEvent
		{
			private readonly object sender;

			public ThreadSafeEvent(object sender)
			{
				this.sender = sender;
			}

			public void RaiseEvent()
			{
				var handler = Event;

				if (handler != null)
				{
					handler(sender, EventArgs.Empty);
				}
			}

			public event EventHandler Event;
		}
	}
}