using CADShark.Common.Logging;
using CADShark.Common.MultiConverter.Core;
using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using CADShark.Common.MultiConverter.Extractors;

namespace CADShark.Common.MultiConverter.Converters
{
    /// <summary>
    /// Abstract base class for converters that export SolidWorks models to various formats.
    /// </summary>
    public abstract class BaseConverter(ISldWorks swApp) : IConverter
    {
        protected readonly ISldWorks SwApp = swApp ?? throw new ArgumentNullException(nameof(swApp));
        protected readonly CadLogger Logger = CadLogger.GetLogger<BaseConverter>();

        /// <summary>
        /// Exports the model to the specified path.
        /// </summary>
        public virtual bool Export(ModelDoc2 model, string path)
        {
            try
            {
                Logger.Info($"Starting export for {model?.GetTitle()} to {path}");
                return DoExport(model, path);
            }
            catch (Exception ex)
            {
                Logger.Error($"Export failed for {model?.GetTitle()}", ex);
                return false;
            }
        }

        public List<CutListItem> GetCutListData(ModelDoc2 model)
        {
            return null;
        }

        /// <summary>
        /// Performs the actual export operation.
        /// </summary>
        protected abstract bool DoExport(ModelDoc2 model, string path);

    }
}