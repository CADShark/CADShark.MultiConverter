using SolidWorks.Interop.swconst;
using System.Collections.Generic;

namespace CADShark.Common.MultiConverter
{
    internal class SwSaveOperation
    {
         internal static string ParseSaveError(swFileSaveError_e err)
        {
            var errors = new List<string>();

            if (err.HasFlag(swFileSaveError_e.swFileLockError))
            {
                errors.Add("File lock error");
            }

            if (err.HasFlag(swFileSaveError_e.swFileNameContainsAtSign))
            {
                errors.Add("File name cannot contain the at symbol(@)");
            }

            if (err.HasFlag(swFileSaveError_e.swFileNameEmpty))
            {
                errors.Add("File name cannot be empty");
            }

            if (err.HasFlag(swFileSaveError_e.swFileSaveAsBadEDrawingsVersion))
            {
                errors.Add("Bad eDrawings data");
            }

            if (err.HasFlag(swFileSaveError_e.swFileSaveAsDoNotOverwrite))
            {
                errors.Add("Cannot overwrite an existing file");
            }

            if (err.HasFlag(swFileSaveError_e.swFileSaveAsInvalidFileExtension))
            {
                errors.Add("File name extension does not match the SOLIDWORKS document type");
            }

            if (err.HasFlag(swFileSaveError_e.swFileSaveAsNameExceedsMaxPathLength))
            {
                errors.Add("File name cannot exceed 255 characters");
            }

            if (err.HasFlag(swFileSaveError_e.swFileSaveAsNotSupported))
            {
                errors.Add("Save As operation is not supported in this environment");
            }

            if (err.HasFlag(swFileSaveError_e.swFileSaveFormatNotAvailable))
            {
                errors.Add("Save As file type is not valid");
            }

            if (err.HasFlag(swFileSaveError_e.swFileSaveRequiresSavingReferences))
            {
                errors.Add("Saving an assembly with renamed components requires saving the references");
            }

            if (err.HasFlag(swFileSaveError_e.swGenericSaveError))
            {
                errors.Add("Generic error");
            }

            if (err.HasFlag(swFileSaveError_e.swReadOnlySaveError))
            {
                errors.Add("File is readonly");
            }

            if (errors.Count == 0)
            {
                errors.Add("Unknown error");
            }

            return string.Join("; ", errors);
        }
    }
}
