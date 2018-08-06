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

        public void SetEnabled(bool val)
        {
            _IsEnabled = val;
            NotifyPropertyChanged(nameof(IsEnabled));
        }

        public EqualFilter(object tagValue)
        {
            Value = tagValue;
        }
    }

    public class FilterGroup : ViewModelBase
    {
        private List<EqualFilter> _Filter;

        private readonly Action InvokeChangeEvent;

        public CommandWithResult<bool> AllCommand { get; }

        public List<EqualFilter> Filter
        {
            get => _Filter;
            set
            {
                if(_Filter!=null && _Filter.Count > 0)
                {
                    foreach (var filter in _Filter)
                    {
                        filter.IsEnableChanged -= InvokeChangeEvent;
                    }
                }

                _Filter = value;

                foreach (var filter in _Filter)
                {
                    filter.IsEnableChanged += InvokeChangeEvent;
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
            AllCommand = new CommandWithResult<bool>(e =>
                                                     {
                                                         foreach (var f in _Filter)
                                                         {
                                                             f.SetEnabled(false);
                                                         }

                                                         StatusChanged?.Invoke();

                                                         return false;
                                                     });

            InvokeChangeEvent = () =>
                                {
                                    AllCommand.Result = _Filter.Exists(x => x.IsEnabled);
                                    StatusChanged?.Invoke();
                                };
        }
    }
}