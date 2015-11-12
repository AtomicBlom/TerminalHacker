using System;
using System.Collections.Generic;
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
		private WrappedWord _bestWord;

		public DelegateCommand<string> AddWordCommand { get; }
		private Collection<string> WordCollection { get; } = new ObservableCollection<string>();
		private IList<string> FailedWords { get; } = new List<string>();

		public WrappedWord BestWord
		{
			get { return _bestWord; }
			set
			{
				if (Equals(value, _bestWord)) return;
				_bestWord = value;
				OnPropertyChanged();
			}
		}

		public Collection<WrappedWord> OtherWords { get; } = new ObservableCollection<WrappedWord>();

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
			WorkedCommand = new DelegateCommand<WrappedWord>(ExecuteWordWorked);
			DidNotWorkCommand = new DelegateCommand<WrappedWord>(ExecuteWordDidNotWork);
		}

		public DelegateCommand<WrappedWord> DidNotWorkCommand { get; set; }

		private void ExecuteWordDidNotWork(WrappedWord obj)
		{
			FailedWords.Add(obj.Word);
			RecalculatePopularity();
		}

		private void ExecuteWordWorked(WrappedWord obj)
		{
			BestWord = null;
			FailedWords.Clear();
			OtherWords.Clear();
			WordCollection.Clear();
		}

		public DelegateCommand<WrappedWord> WorkedCommand { get; set; }

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

			return _wordLength == word.Length && !WordCollection.Contains(word);
		}

		private void ExecuteAddWord(string word)
		{
			word = word.ToLower();
			WordCollection.Add(word);
			if (_wordLength == null)
			{
				_wordLength = word.Length;
			}
			

			RecalculatePopularity();
			WordToAdd = string.Empty;
		}

		private void RecalculatePopularity()
		{
			if (_wordLength == null) return;

			IList<WrappedWord> wordList = WordCollection.Select(w => new WrappedWord(w, FailedWords.Contains(w))).ToList();
			//var activeWordList = wordList.Where(_ => !_.Failed).Select(_ => _.Word).ToList();
			

			for (int i = 0; i < _wordLength.Value; i++)
			{
				var index = i;

				var invalidChars = FailedWords.Select(w => w[index]).Distinct().ToArray();

				var sum = WordCollection.Count;
				var columnPopularities = WordCollection.Select(w => w[index]).GroupBy(c => c).ToDictionary(k => k.Key, v => v.Count() / (decimal)sum);

				foreach (var word in wordList)
				{
					if (i == 0)
					{
						word.Effectiveness = 1;
					}

					var character = word.Word[i];
					if (invalidChars.Contains(character))
					{
						word.Failed = true;
						word.Effectiveness = 0;
					}
					else
					{
						word.Effectiveness *= columnPopularities[character];
					}
				}
			}

			BestWord = wordList.OrderByDescending(_ => _.Effectiveness).First();
			OtherWords.Clear();
			foreach (var wrappedWord in wordList.OrderByDescending(_ => _.Effectiveness).Except(new []{ BestWord }))
			{
				OtherWords.Add(wrappedWord);
			}
			
		}

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}

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
