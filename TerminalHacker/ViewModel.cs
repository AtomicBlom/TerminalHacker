using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using TerminalHacker.Annotations;

namespace TerminalHacker
{
	class ViewModel : INotifyPropertyChanged
	{
		private string _wordToAdd;
		private int? _wordLength;

		public DelegateCommand<string> AddWordCommand { get; }
		public Collection<WrappedWord> WordCollection { get; } = new ObservableCollection<WrappedWord>();

		public string WordToAdd
		{
			get { return _wordToAdd; }
			set
			{
				if (value == _wordToAdd) return;
				_wordToAdd = value;
				OnPropertyChanged();
				AddWordCommand.NotifyCanExecuteChanged();
            }
		}

		public ViewModel()
		{
			AddWordCommand = new DelegateCommand<string>(ExecuteAddWord, CanExecuteAddWord);
		}

		private bool CanExecuteAddWord(string word)
		{
			if (word == null)
			{
				return false;
			}

			word = word.ToLower();
			if (word.Any(c => c < 'a' || c > 'z'))
			{
				return false;
			}

			if (_wordLength == null && word.Length > 0)
			{
				return true;
			}

			return _wordLength == word.Length && !WordCollection.Contains(new WrappedWord(word));
		}

		private void ExecuteAddWord(string word)
		{
			word = word.ToLower();
			WordCollection.Add(new WrappedWord(word));
			if (_wordLength == null)
			{
				_wordLength = word.Length;
			}
			WordToAdd = string.Empty;

			ReclaculatePopularity();
		}

		private void ReclaculatePopularity()
		{
			if (_wordLength == null) return;
			for (int i = 0; i < _wordLength.Value; i++)
			{
				var index = i;
				
				var sum = WordCollection.Count;
				var columnPopularities = WordCollection.Select(w => w.Word[index]).GroupBy(c => c).ToDictionary(k => k.Key, v => v.Count() / (decimal)sum);

				foreach (var word in WordCollection)
				{
					if (i == 0)
					{
						word.Effectiveness = 1;
					}

					var character = word.Word[i];
					word.Effectiveness *= columnPopularities[character];
				}
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}

	public class WrappedWord : IEquatable<WrappedWord>, INotifyPropertyChanged
	{
		private decimal _effectiveness;
		public string Word { get; }

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

		public WrappedWord(string word)
		{
			Word = word;
			Effectiveness = 0;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			return obj is WrappedWord && Equals((WrappedWord) obj);
		}

		public bool Equals(WrappedWord other)
		{
			return string.Equals(Word, other.Word);
		}

		public override int GetHashCode()
		{
			return Word?.GetHashCode() ?? 0;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
