using Newtonsoft.Json;
using System;

namespace GPGO_MultiPLCs.Helpers
{
    public abstract class RecipeBase<T> : ObservableObject, IEquatable<T> where T : class, new()
    {
        public virtual string RecipeName { get; set; }
        public virtual DateTime Updated { get; set; }
        public virtual string Editor { get; set; }
        public abstract bool Equals(T other);
        public abstract T Copy(string user);
        public abstract void CopyValue(string user, T recipe);

        public RecipeBase(string name, string user)
        {
            RecipeName = string.IsNullOrEmpty(name) ? Updated.Ticks.ToString() : name;
            Editor = user;
            Updated = DateTime.Now;
        }

        public RecipeBase()
        {
            RecipeName = Updated.Ticks.ToString();
            Editor = "";
            Updated = DateTime.Now;
        }
    }
}
