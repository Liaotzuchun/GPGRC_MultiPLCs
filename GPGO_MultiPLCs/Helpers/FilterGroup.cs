using System;
using System.Collections.Generic;
using System.Linq;

namespace GPGO_MultiPLCs.Helpers
{
    public class EqualFilter : BindableBase
    {
        public bool IsEnabled
        {
            get => Get<bool>();
            set
            {
                Set(value);
                IsEnableChanged?.Invoke();
            }
        }

        public object Value
        {
            get => Get<object>();
            set => Set(value);
        }

        public event Action IsEnableChanged;

        public bool Check(object val)
        {
            return val == null || IsEnabled && Value.Equals(val);
        }

        public void SetEnabled(bool val)
        {
            Set(val, nameof(IsEnabled));
        }

        public EqualFilter(object tagValue)
        {
            Value = tagValue;
        }
    }

    public class FilterGroup : BindableBase
    {
        private readonly Action InvokeChangeEvent;
        public CommandWithResult<bool> AllCommand { get; }

        public List<EqualFilter> Filter
        {
            get => Get<List<EqualFilter>>();
            set
            {
                if (Get<List<EqualFilter>>() is List<EqualFilter> filters && filters.Count > 0)
                {
                    foreach (var filter in filters)
                    {
                        filter.IsEnableChanged -= InvokeChangeEvent;
                    }
                }

                foreach (var filter in value)
                {
                    filter.IsEnableChanged += InvokeChangeEvent;
                }

                Set(value);
            }
        }

        public bool Check(object val)
        {
            return Filter.Any(x => x.Check(val)) || Filter.TrueForAll(x => !x.IsEnabled);
        }

        public FilterGroup(Action StatusChangedAct)
        {
            AllCommand = new CommandWithResult<bool>(e =>
                                                     {
                                                         foreach (var f in Filter)
                                                         {
                                                             f.SetEnabled(false);
                                                         }

                                                         StatusChangedAct?.Invoke();

                                                         return false;
                                                     });

            InvokeChangeEvent = () =>
                                {
                                    if (Filter.All(x => x.IsEnabled))
                                    {
                                        Filter.ForEach(x => x.IsEnabled = false);
                                    }

                                    AllCommand.Result = Filter.Exists(x => x.IsEnabled);

                                    StatusChangedAct?.Invoke();
                                };
        }
    }
}