using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace GPGO_MultiPLCs.Models
{
    public class ObjectViewModelHierarchy
    {
        public ReadOnlyCollection<ObjectViewModel> FirstGeneration { get; }

        public ObjectViewModelHierarchy(object rootObject)
        {
            var o = new ObjectViewModel(rootObject);
            FirstGeneration = new ReadOnlyCollection<ObjectViewModel>(new[] { o });
        }
    }

    public class ObjectViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly object _object;
        private readonly Type _type;

        private bool _isExpanded;
        private bool _isSelected;

        public PropertyInfo Info { get; }

        public string Name
        {
            get
            {
                var name = string.Empty;
                if (Info != null)
                {
                    name = Info.Name;
                }

                return name;
            }
        }

        public ObjectViewModel Parent { get; }

        public string Type
        {
            get
            {
                var type = string.Empty;
                if (_object != null)
                {
                    type = $"({_type.Name})";
                }
                else
                {
                    if (Info != null)
                    {
                        type = $"({Info.PropertyType.Name})";
                    }
                }

                return type;
            }
        }

        public string Value
        {
            get
            {
                var value = string.Empty;
                if (_object != null)
                {
                    if (IsPrintableType(_type))
                    {
                        value = _object.ToString();
                    }
                }
                else
                {
                    value = "<null>";
                }

                return value;
            }
        }

        public ReadOnlyCollection<ObjectViewModel> Children { get; private set; }

        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                if (_isExpanded != value)
                {
                    _isExpanded = value;
                    if (_isExpanded)
                    {
                        LoadChildren();
                    }

                    OnPropertyChanged("IsExpanded");
                }

                // Expand all the way up to the root.
                if (_isExpanded && Parent != null)
                {
                    Parent.IsExpanded = true;
                }
            }
        }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged("IsSelected");
                }
            }
        }

        /// <summary>Gets a value indicating if the object graph can display this type without enumerating its children</summary>
        private static bool IsPrintableType(Type type)
        {
            return type != null && (type.IsPrimitive || type.IsAssignableFrom(typeof(string)) || type.IsEnum);
        }

        public void LoadChildren()
        {
            if (_object != null)
            {
                // exclude value types and strings from listing child members
                if (!IsPrintableType(_type))
                {
                    // the public properties of this object are its children
                    var children = _type.GetProperties()
                                        .Where(p => !p.GetIndexParameters().Any()) // exclude indexed parameters for now
                                        .Select(p => new ObjectViewModel(p.GetValue(_object, null), p, this))
                                        .ToList();

                    // if this is a collection type, add the contained items to the children
                    if (_object is IEnumerable collection)
                    {
                        children.AddRange(from object item in collection select new ObjectViewModel(item, null, this));
                    }

                    Children = new ReadOnlyCollection<ObjectViewModel>(children);
                    OnPropertyChanged("Children");
                }
            }
        }

        public bool NameContains(string text)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(Name))
            {
                return false;
            }

            return Name.IndexOf(text, StringComparison.InvariantCultureIgnoreCase) > -1;
        }

        public bool ValueContains(string text)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(Value))
            {
                return false;
            }

            return Value.IndexOf(text, StringComparison.InvariantCultureIgnoreCase) > -1;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ObjectViewModel(object obj) : this(obj, null, null)
        {
        }

        private ObjectViewModel(object obj, PropertyInfo info, ObjectViewModel parent)
        {
            _object = obj;
            Info = info;
            if (_object != null)
            {
                _type = obj.GetType();
                if (!IsPrintableType(_type))
                {
                    // load the _children object with an empty collection to allow the + expander to be shown
                    Children = new ReadOnlyCollection<ObjectViewModel>(new[] { new ObjectViewModel(null) });
                }
            }

            Parent = parent;
        }
    }
}