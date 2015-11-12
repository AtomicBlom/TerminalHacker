using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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
