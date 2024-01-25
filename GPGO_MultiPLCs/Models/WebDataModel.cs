namespace GPGRC_MultiPLCs.Models
{
    public class WebDataModel
    {
        // 注意: 產生的程式碼可能至少需要 .NET Framework 4.5 或 .NET Core/Standard 2.0。
        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
        public partial class Ingredients
        {

            private IngredientsItem[] itemField;

            private string macCodeField;

            private uint wipEntityField;

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute("item")]
            public IngredientsItem[] item
            {
                get
                {
                    return this.itemField;
                }
                set
                {
                    this.itemField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string macCode
            {
                get
                {
                    return this.macCodeField;
                }
                set
                {
                    this.macCodeField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public uint wipEntity
            {
                get
                {
                    return this.wipEntityField;
                }
                set
                {
                    this.wipEntityField = value;
                }
            }
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class IngredientsItem
        {

            private string tagCodeField;

            private string tagValueField;

            private string timeStampField;

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string tagCode
            {
                get
                {
                    return this.tagCodeField;
                }
                set
                {
                    this.tagCodeField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string tagValue
            {
                get
                {
                    return this.tagValueField;
                }
                set
                {
                    this.tagValueField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string timeStamp
            {
                get
                {
                    return this.timeStampField;
                }
                set
                {
                    this.timeStampField = value;
                }
            }
        }


    }
}
