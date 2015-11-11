using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using System.Windows.Input;
using Windows.System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

namespace TerminalHacker
{
	class LinkToButton
	{
		public static readonly DependencyProperty ButtonProperty = DependencyProperty.RegisterAttached(
			"Button", typeof (ButtonBase), typeof (LinkToButton), new PropertyMetadata(default(ButtonBase), OnButtonChanged));

		private static void OnButtonChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
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
				FrameworkElement element = sender as FrameworkElement;
				if (element != null)
				{
					var buttonBase = GetButton(element);
					
					var command = buttonBase.Command;
					if (command != null)
					{
						if (command.CanExecute(buttonBase.CommandParameter))
						{
							command.Execute(buttonBase.CommandParameter);
						}
					}
				}
			}
		}

		public static void SetButton(DependencyObject element, ButtonBase value)
		{
			element.SetValue(ButtonProperty, value);
		}

		public static ButtonBase GetButton(DependencyObject element)
		{
			return (ButtonBase) element.GetValue(ButtonProperty);
		}
	}
}
