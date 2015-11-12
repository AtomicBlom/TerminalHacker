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
		private IEnumerable<int> _correctLetterCountList;
		private readonly List<WrappedWord> _allWords = new List<WrappedWord>();

		public DelegateCommand<string> AddWordCommand { get; }
		private Collection<string> WordCollection { get; } = new ObservableCollection<string>();
		private IDictionary<string, int> FailedWords { get; } = new Dictionary<string, int>();

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
			DidNotWorkCommand = new DelegateCommand<int>(ExecuteWordDidNotWork);
		}

		public DelegateCommand<int> DidNotWorkCommand { get; set; }

		private void ExecuteWordDidNotWork(int correctLetters)
		{
			FailedWords.Add(BestWord.Word, correctLetters);
			BestWord.Failed = true;

			foreach (var word in _allWords.Where(_ => !_.Failed))
			{
				int matchedCount = 0;
				for (int i = 0; i < _wordLength; ++i)
				{
					if (word.Word[i] == BestWord.Word[i])
					{
						matchedCount++;
					}
				}

				if (matchedCount != correctLetters)
				{
					word.Failed = true;
				}
			}
			

			RecalculatePopularity();
		}

		private void ExecuteWordWorked(WrappedWord obj)
		{
			BestWord = null;
			_allWords.Clear();
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
				CorrectLetterCountList = Enumerable.Range(0, _wordLength ?? -1);
			}
			

			RecalculatePopularity();
			WordToAdd = string.Empty;
		}

		public IEnumerable<int> CorrectLetterCountList
		{
			get { return _correctLetterCountList; }
			set
			{
				if (Equals(value, _correctLetterCountList)) return;
				_correctLetterCountList = value;
				OnPropertyChanged();
			}
		}

		private void RecalculatePopularity()
		{
			if (_wordLength == null) return;

			_allWords.AddRange(WordCollection.Where(_ => !_allWords.Any(existingWord => _ == existingWord.Word)).Select(w => new WrappedWord(w, false)));

			decimal totalEffectiveness = 0;
			for (int i = 0; i < _wordLength.Value; i++)
			{
				var index = i;

				var sum = WordCollection.Count;
				var columnPopularities = WordCollection.Select(w => w[index]).GroupBy(c => c).ToDictionary(k => k.Key, v => v.Count() / (decimal)sum);

				foreach (var word in _allWords.Where(_ => !_.Failed))
				{
					if (i == 0)
					{
						word.Effectiveness = 1;
					} 

					var character = word.Word[i];
					word.Effectiveness *= columnPopularities[character];
					if (i == _wordLength.Value - 1)
					{
						totalEffectiveness += word.Effectiveness;
					}
				}
			}

			BestWord = _allWords.OrderByDescending(_ => _.Effectiveness).First();
			OtherWords.Clear();
			foreach (var wrappedWord in _allWords.OrderByDescending(_ => _.Effectiveness).Except(new []{ BestWord }))
			{
				OtherWords.Add(wrappedWord);
			}

			foreach (var wrappedWord in _allWords)
			{
				wrappedWord.Helpfulness = wrappedWord.Effectiveness/totalEffectiveness;
			}
			
		}

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
