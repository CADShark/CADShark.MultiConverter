using CADShark.Common.MultiConverter.Converters;
using CADShark.Common.MultiConverter.Core;
using SolidWorks.Interop.sldworks;
using System;

namespace CADShark.Common.MultiConverter;

public class ConverterFactory(ISldWorks swApp) : IConverterFactory
{
    public IConverter Create(ExportFormat format)
    {
        return format switch
        {
            ExportFormat.Dxf => new DxfConvert(swApp),
            ExportFormat.Pdf => new PdfConverter(swApp),
            ExportFormat.Step => new StepConverter(swApp),

            _ => throw new NotSupportedException($"Format {format} not supported")
        };
    }
}