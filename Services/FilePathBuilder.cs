using System;
using System.IO;
using CADShark.Common.Logging;
using CADShark.Common.MultiConverter.Core;

namespace CADShark.Common.MultiConverter.Services;

/// <summary>
/// Implementation of IFilePathBuilder for constructing output file paths.
/// </summary>
public class FilePathBuilder : IFilePathBuilder
{
    private static readonly CadLogger Logger = CadLogger.GetLogger<FilePathBuilder>();

    /// <summary>
    /// Builds the output file path.
    /// </summary>
    /// <param name="sourcePath">The source file path.</param>
    /// <param name="format">The export format (e.g., Pdf, Dxf, Step).</param>
    /// <param name="config">Optional configuration name to include in the file name.</param>
    /// <param name="newSavePath">Optional new save path; defaults to a subfolder in the source directory.</param>
    /// <returns>The built file path.</returns>
    public string Build(string sourcePath, ExportFormat format, string config = null, string newSavePath = null)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(sourcePath))
            {
                Logger.Error("Source path is null or empty");
                throw new ArgumentException("Source path cannot be null or empty", nameof(sourcePath));
            }

            // Convert ExportFormat to string extension
            var extension = format switch
            {
                ExportFormat.Pdf => "pdf",
                ExportFormat.Dxf => "dxf",
                ExportFormat.Step => "step",
                _ => throw new ArgumentException($"Unsupported export format: {format}", nameof(format))
            };

            var directoryName = Path.GetDirectoryName(sourcePath);
            if (string.IsNullOrEmpty(directoryName))
            {
                Logger.Error("Directory name could not be resolved from path: {SourcePath}", sourcePath);
                throw new InvalidOperationException($"Invalid source path: {sourcePath}");
            }

            var outputFolder = !string.IsNullOrWhiteSpace(newSavePath)
                ? newSavePath
                : directoryName;

            var fileNameWithoutExt = Path.GetFileNameWithoutExtension(sourcePath);

            var fullName = !string.IsNullOrEmpty(config)
                ? $"{fileNameWithoutExt}-{config}.{extension.ToLowerInvariant()}"
                : $"{fileNameWithoutExt}.{extension.ToLowerInvariant()}";

            if (!Directory.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
                Logger.Info($"Created output directory: {outputFolder}");
            }

            var resultPath = Path.Combine(outputFolder, fullName);

            Logger.Trace($"Built file path: {resultPath}");
            return resultPath;
        }
        catch (Exception ex)
        {
            Logger.Error("Failed to build file path", ex);
            throw;
        }
    }
}