using System.ComponentModel;
using System.Runtime.CompilerServices;
using TerminalHacker.Annotations;

namespace TerminalHacker
{
	public class WrappedWord : INotifyPropertyChanged
	{
		private decimal _effectiveness;
		private bool _failed;
		public string Word { get; }

		public bool Failed
		{
			get { return _failed; }
			set
			{
				if (value == _failed) return;
				_failed = value;
				OnPropertyChanged();
				if (Failed)
				{
					Effectiveness = 0;
				}
			}
		}

		public decimal Effectiveness
		{
			get { return _effectiveness; }
			set
			{
				if (value == _effectiveness) return;
				_effectiveness = value;
				OnPropertyChanged();
			}
		}

		public decimal Helpfulness { get; set; }

		public WrappedWord(string word, bool failed)
		{
			Word = word;
			Failed = failed;
			Effectiveness = 0;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}