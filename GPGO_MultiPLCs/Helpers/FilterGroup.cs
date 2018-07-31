using System;
using System.Collections.Generic;
using System.Linq;

namespace GPGO_MultiPLCs.Helpers
{
    public class EqualFilter : ViewModelBase
    {
        private bool _IsEnabled;
        private object _Value;

        public bool IsEnabled
        {
            get => _IsEnabled;
            set
            {
                _IsEnabled = value;
                NotifyPropertyChanged();
                IsEnableChanged?.Invoke();
            }
        }

        public object Value
        {
            get => _Value;
            set
            {
                _Value = value;
                NotifyPropertyChanged();
            }
        }

        public event Action IsEnableChanged;

        public bool Check(object val)
        {
            return val == null || IsEnabled && Value.Equals(val);
        }

        public EqualFilter(object tagValue)
        {
            Value = tagValue;
        }
    }

    public class FilterGroup : ViewModelBase
    {
        private List<EqualFilter> _Filter;

        public RelayCommand AllCommand { get; }

        public List<EqualFilter> Filter
        {
            get => _Filter;
            set
            {
                _Filter = value;

                foreach (var filter in _Filter)
                {
                    filter.IsEnableChanged += () => StatusChanged?.Invoke();
                }

                NotifyPropertyChanged();
            }
        }

        public event Action StatusChanged;

        public bool Check(object val)
        {
            return _Filter.Any(x => x.Check(val)) || _Filter.TrueForAll(x => !x.IsEnabled);
        }

        public FilterGroup()
        {
            AllCommand = new RelayCommand(e =>
                                          {
                                              foreach (var f in _Filter)
                                              {
                                                  f.IsEnabled = false;
                                              }

                                              StatusChanged?.Invoke();
                                          });
        }
    }
}