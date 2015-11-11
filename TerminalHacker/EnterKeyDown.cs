using System.Windows.Input;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

namespace TerminalHacker
{
	public sealed class EnterKeyDown
	{

		#region Properties

		#region Command

		public static ICommand GetCommand(DependencyObject obj)
		{
			return (ICommand)obj.GetValue(CommandProperty);
		}

		public static void SetCommand(DependencyObject obj, ICommand value)
		{
			obj.SetValue(CommandProperty, value);
		}

		public static readonly DependencyProperty CommandProperty =
			DependencyProperty.RegisterAttached("Command", typeof(ICommand), typeof(EnterKeyDown),
				new PropertyMetadata(null, OnCommandChanged));

		#endregion Command

		#region CommandArgument

		public static object GetCommandArgument(DependencyObject obj)
		{
			return (object)obj.GetValue(CommandArgumentProperty);
		}

		public static void SetCommandArgument(DependencyObject obj, object value)
		{
			obj.SetValue(CommandArgumentProperty, value);
		}

		public static readonly DependencyProperty CommandArgumentProperty =
			DependencyProperty.RegisterAttached("CommandArgument", typeof(object), typeof(EnterKeyDown),
				new PropertyMetadata(null, OnCommandArgumentChanged));

		#endregion CommandArgument

		#region HasCommandArgument


		private static bool GetHasCommandArgument(DependencyObject obj)
		{
			return (bool)obj.GetValue(HasCommandArgumentProperty);
		}

		private static void SetHasCommandArgument(DependencyObject obj, bool value)
		{
			obj.SetValue(HasCommandArgumentProperty, value);
		}

		private static readonly DependencyProperty HasCommandArgumentProperty =
			DependencyProperty.RegisterAttached("HasCommandArgument", typeof(bool), typeof(EnterKeyDown),
				new PropertyMetadata(false));


		#endregion HasCommandArgument

		#endregion Propreties

		#region Event Handling

		private static void OnCommandArgumentChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			SetHasCommandArgument(o, true);
		}

		private static void OnCommandChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			FrameworkElement element = o as FrameworkElement;
			if (element != null)
			{
				if (e.NewValue == null)
				{
					element.KeyDown -= FrameworkElement_KeyDown;
				}
				else if (e.OldValue == null)
				{
					element.KeyDown += FrameworkElement_KeyDown;
				}
			}
		}

		private static void FrameworkElement_KeyDown(object sender, KeyRoutedEventArgs e)
		{
			if (e.Key == VirtualKey.Enter)
			{
				DependencyObject o = sender as DependencyObject;
				ICommand command = GetCommand(sender as DependencyObject);

				FrameworkElement element = e.OriginalSource as FrameworkElement;
				if (element != null)
				{
					// If the command argument has been explicitly set (even to NULL)
					if (GetHasCommandArgument(o))
					{
						object commandArgument = GetCommandArgument(o);

						// Execute the command
						if (command.CanExecute(commandArgument))
						{
							command.Execute(commandArgument);
						}
					}
					else if (command.CanExecute(element.DataContext))
					{
						command.Execute(element.DataContext);
					}
				}
			}
		}

		#endregion
	}
}