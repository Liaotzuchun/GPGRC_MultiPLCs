using System;
using GPGO_MultiPLCs.Models;

namespace GPGO_MultiPLCs.Helpers
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class OrderedObject : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class OrderIndex : Attribute
    {
        public int Index { get; }

        public OrderIndex(int value)
        {
            Index = value;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class LanguageTranslator : Attribute
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
}