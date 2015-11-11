using System;
using System.Windows.Input;

namespace TerminalHacker
{
	class DelegateCommand : ICommand
	{
		private readonly Action<object> _executeMethod;
		private readonly Predicate<object> _canExecuteMethod;

		public DelegateCommand(Action<object> executeMethod)
		{
			_executeMethod = executeMethod;
		}

		public DelegateCommand(Action<object> executeMethod, Predicate<object> canExecuteMethod)
		{
			_executeMethod = executeMethod;
			_canExecuteMethod = canExecuteMethod;
		}

		public bool CanExecute(object parameter)
		{
			return _canExecuteMethod?.Invoke(parameter) ?? true;
		}

		public void Execute(object parameter)
		{
			_executeMethod?.Invoke(parameter);
		}

		public event EventHandler CanExecuteChanged;

		public void NotifyCanExecuteChanged()
		{
			CanExecuteChanged?.Invoke(this, EventArgs.Empty);
		}
	}

	class DelegateCommand<T> : ICommand
	{
		private readonly Action<T> _executeMethod;
		private readonly Predicate<T> _canExecuteMethod;

		public DelegateCommand(Action<T> executeMethod)
		{
			_executeMethod = executeMethod;
		}

		public DelegateCommand(Action<T> executeMethod, Predicate<T> canExecuteMethod)
		{
			_executeMethod = executeMethod;
			_canExecuteMethod = canExecuteMethod;
		}

		public bool CanExecute(object parameter)
		{
			return _canExecuteMethod?.Invoke((T)parameter) ?? true;
		}

		public void Execute(object parameter)
		{
			_executeMethod?.Invoke((T)parameter);
		}

		public event EventHandler CanExecuteChanged;

		public void NotifyCanExecuteChanged()
		{
			CanExecuteChanged?.Invoke(this, EventArgs.Empty);
		}
	}
}