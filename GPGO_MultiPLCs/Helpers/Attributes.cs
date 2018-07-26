using System;

namespace GPGO_MultiPLCs.Helpers
{
    [AttributeUsage(AttributeTargets.Property)]
    public class EN_Name : Attribute
    {
        public string Name { get; }

        public EN_Name(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class CHT_Name : Attribute
    {
        public string Name { get; }

        public CHT_Name(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class CHS_Name : Attribute
    {
        public string Name { get; }

        public CHS_Name(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
