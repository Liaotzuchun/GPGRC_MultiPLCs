using System;

namespace GPGO_MultiPLCs.Helpers
{
    [AttributeUsage(AttributeTargets.Property)]
    public class EN_Name : Attribute
    {
        public string Name { get; }

        public override string ToString()
        {
            return Name;
        }

        public EN_Name(string name)
        {
            Name = name;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class CHT_Name : Attribute
    {
        public string Name { get; }

        public override string ToString()
        {
            return Name;
        }

        public CHT_Name(string name)
        {
            Name = name;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class CHS_Name : Attribute
    {
        public string Name { get; }

        public override string ToString()
        {
            return Name;
        }

        public CHS_Name(string name)
        {
            Name = name;
        }
    }
}