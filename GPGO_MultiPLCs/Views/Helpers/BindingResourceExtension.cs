using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace GPGO_MultiPLCs.Views
{
    public class BindingResourceExtension : StaticResourceExtension
    {
        public BindingResourceExtension()
        { }

        public BindingResourceExtension(object resourceKey) : base(resourceKey) { }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (base.ProvideValue(serviceProvider) is BindingBase binding)
                return binding.ProvideValue(serviceProvider);
            else
                return null; //or throw an exception
        }
    }
}
