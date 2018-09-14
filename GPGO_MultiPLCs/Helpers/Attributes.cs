using System;
using GPGO_MultiPLCs.Models;

namespace GPGO_MultiPLCs.Helpers
{
    [AttributeUsage(AttributeTargets.Class)]
    public class OrderedObject : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class OrderIndex : Attribute
    {
        public int Index { get; }

        public OrderIndex(int value)
        {
            Index = value;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class LanguageTranslator : Attribute
    {
        public string CHS_Name { get; }
        public string CHT_Name { get; }
        public string EN_Name { get; }

        public string GetName(Language lng)
        {
            return lng == Language.EN ? EN_Name : lng == Language.CHS ? CHS_Name : CHT_Name;
        }

        public LanguageTranslator(string en, string cht, string chs)
        {
            EN_Name = en;
            CHT_Name = cht;
            CHS_Name = chs;
        }
    }

    //[AttributeUsage(AttributeTargets.Property)]
    //public class EN_Name : Attribute
    //{
    //    public string Name { get; }

    //    public override string ToString()
    //    {
    //        return Name;
    //    }

    //    public EN_Name(string name)
    //    {
    //        Name = name;
    //    }
    //}

    //[AttributeUsage(AttributeTargets.Property)]
    //public class CHT_Name : Attribute
    //{
    //    public string Name { get; }

    //    public override string ToString()
    //    {
    //        return Name;
    //    }

    //    public CHT_Name(string name)
    //    {
    //        Name = name;
    //    }
    //}

    //[AttributeUsage(AttributeTargets.Property)]
    //public class CHS_Name : Attribute
    //{
    //    public string Name { get; }

    //    public override string ToString()
    //    {
    //        return Name;
    //    }

    //    public CHS_Name(string name)
    //    {
    //        Name = name;
    //    }
    //}
}