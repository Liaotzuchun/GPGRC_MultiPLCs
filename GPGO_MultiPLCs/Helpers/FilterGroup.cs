using System;
using System.Collections.Generic;
using System.Linq;

namespace GPGO_MultiPLCs.Helpers
{
    public class EqualFilter : ObservableObject
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

    public class FilterGroup : ObservableObject
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

                AllCommand.Result = false;
                Set(value);
            }
        }

        public bool Check(object val)
        {
            return Filter != null && (Filter.Any(x => x.Check(val)) || Filter.TrueForAll(x => !x.IsEnabled));
        }

        /// <summary></summary>
        /// <param name="StatusChangedAct">當過濾條件變更時指定執行之委派</param>
        public FilterGroup(Action StatusChangedAct = null)
        {
            AllCommand = new CommandWithResult<bool>(e =>
                                                     {
                                                         Filter.ForEach(x => x.SetEnabled(false));

                                                         StatusChangedAct?.Invoke();

                                                         return false;
                                                     });

            InvokeChangeEvent = () =>
                                {
                                    if (Filter.All(x => x.IsEnabled))
                                    {
                                        Filter.ForEach(x => x.SetEnabled(false));
                                    }

                                    AllCommand.Result = Filter.Exists(x => x.IsEnabled);

                                    StatusChangedAct?.Invoke();
                                };
        }
    }
}