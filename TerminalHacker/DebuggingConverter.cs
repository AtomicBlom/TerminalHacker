using System;
using Windows.UI.Xaml.Data;

namespace TerminalHacker
{
	public class DebuggingConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			return value; // Add the breakpoint here!!
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			return value; // Add the breakpoint here!!
		}
	}
}