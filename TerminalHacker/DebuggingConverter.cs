using System;
using Windows.UI;
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

	public class DisabledWordConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			var wrappedWord = (WrappedWord) value;
			if (wrappedWord.Failed)
			{
				return Colors.Red;
			}
			else
			{
				return Colors.Black;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}
}