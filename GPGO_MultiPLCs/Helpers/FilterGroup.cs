using System;
using System.Collections.Generic;
using System.Linq;

namespace GPGO_MultiPLCs.Helpers
{
    public class EqualFilter : ViewModelBase
    {
        private object _Value;
        private bool _IsEnabled;

        public object Value
        {
            get => _Value;
            set
            {
                _Value = value;
                NotifyPropertyChanged();
            }
        }

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

        public event Action IsEnableChanged;

        public bool Check(object val)
        {
            return IsEnabled && Value.Equals(val);
        }

        public EqualFilter(object tagValue)
        {
            Value = tagValue;
        }
    }

    public class FilterGroup : ViewModelBase
    {
        public event Action StatusChanged;

        private List<EqualFilter> _Filter;

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

        public RelayCommand AllCommand { get; }

        public bool Check(object val) => _Filter.Any(x => x.Check(val)) || _Filter.TrueForAll(x => !x.IsEnabled);

        public FilterGroup()
        {
            AllCommand = new RelayCommand(e=>
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
