﻿using CADShark.Common.Logging;
using SolidWorks.Interop.sldworks;
using System;

namespace CADShark.Common.MultiConverter
{
    internal class ConvertDxf : ConvertBuilder
    {
        private static readonly CadLogger Logger = CadLogger.GetLogger(className: nameof(ConvertDxf));

        internal static bool ExportFile(bool isSheetmetal)
        {
            var swModel = (ModelDoc2)SwApp.ActiveDoc;
            var configManager = swModel.ConfigurationManager;
            var configName = configManager.ActiveConfiguration.Name;

            // ReSharper disable once SuspiciousTypeConversion.Global
            var partDoc = swModel as IPartDoc;
            try
            {
                var alignment = (object)new[]
                {
                    0.0,
                    0.0,
                    0.0,
                    1.0,
                    0.0,
                    0.0,
                    0.0,
                    1.0,
                    0.0,
                    0.0,
                    0.0,
                    1.0
                };
                var options = SheetMetalOptions(true, false, false, false, false, true, false);
                var modelName = swModel.GetPathName();
                var action = isSheetmetal ? 1 : 2;
                object views = isSheetmetal ? 0 : 3;

                Logger.Trace($"Path export: {FilePath}");

                if (partDoc == null)
                {
                    Logger.Warning("Body is empty");
                    return false;
                }

                var status = partDoc.ExportToDWG2(FilePath, modelName, action, true, alignment, false, false, options,
                    views);

                Logger.Trace($"Status export: {status} Configuration: {configName}");
                return status;
            }
            catch (Exception ex)
            {
                var error = $"Failed build dxf {swModel.GetTitle()} with configuration {configName} {ex.Message}";
                Logger.Error(error);
                return false;
            }
        }

        private static int SheetMetalOptions(
            bool exportGeometry,
            bool icnludeHiddenEdges,
            bool exportBendLines,
            bool includeScetches,
            bool mergeCoplanarFaces,
            bool exportLibraryFeatures,
            bool exportFirmingTools)
        {
            return SheetMetalOptions(exportGeometry ? 1 : 0, icnludeHiddenEdges ? 1 : 0, exportBendLines ? 1 : 0,
                includeScetches ? 1 : 0, mergeCoplanarFaces ? 1 : 0, exportLibraryFeatures ? 1 : 0,
                exportFirmingTools ? 1 : 0);
        }

        private static int SheetMetalOptions(int p0, int p1, int p2, int p3, int p4, int p5, int p6)
        {
            return p0 + p1 * 2 + p2 * 4 + p3 * 8 + p4 * 16 + p5 * 32 + p6 * 64;
        }
    }
}