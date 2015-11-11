using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace TerminalHacker
{
	public sealed partial class WordDisplay : UserControl
	{
		public static readonly DependencyProperty WordProperty = DependencyProperty.Register(
			"Word", typeof (WrappedWord), typeof (WordDisplay), new PropertyMetadata(default(WrappedWord), (o, args) => ((WordDisplay)o).CreateView()));

		public WrappedWord Word
		{
			get { return (WrappedWord) GetValue(WordProperty); }
			set { SetValue(WordProperty, value); }
		}

		public WordDisplay()
		{
			this.InitializeComponent();
		}

		public void CreateView()
		{
			StackPanel.Children.Clear();

			foreach (var c in Word.Word)
			{
				var tb = new TextBlock
				{
					Text = c.ToString().ToUpper(),
					Width = 24
				};
				StackPanel.Children.Add(tb);
			}
		}
	}
}
