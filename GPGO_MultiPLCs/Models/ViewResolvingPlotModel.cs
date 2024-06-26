using System;
using System.Linq;
using System.Reflection;
using OxyPlot;

namespace GPGRC_MultiPLCs.Models;

/// <summary>解決PlotView使用在DataTemplate時可能發生的佔用例外</summary>
public class ViewResolvingPlotModel : PlotModel, IPlotModel
{
    private static readonly Type BaseType = typeof(ViewResolvingPlotModel).BaseType!;
    private static readonly MethodInfo? BaseAttachMethod = BaseType
                                                          .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                                                          .Where(methodInfo => methodInfo is { IsFinal: true, IsPrivate: true })
                                                          .FirstOrDefault(methodInfo => methodInfo.Name.EndsWith(nameof(IPlotModel.AttachPlotView)));

    #region Interface Implementations
    void IPlotModel.AttachPlotView(IPlotView? plotView)
    {
        if (plotView != null && PlotView != null && !Equals(plotView, PlotView))
        {
            BaseAttachMethod?.Invoke(this, [null]);
            BaseAttachMethod?.Invoke(this, [plotView]);
        }
        else
        {
            BaseAttachMethod?.Invoke(this, [plotView]);
        }
    }
    #endregion
}