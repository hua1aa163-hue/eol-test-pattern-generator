using EolTestPatternGenerator.Models;

namespace EolTestPatternGenerator.Services;

public static class DefaultBatchExporter
{
    public static IReadOnlyList<string> ExportAll(
        string outputDirectory,
        int canvasWidth,
        int canvasHeight,
        int dotRadius,
        int lineWidth,
        ImageExportOptions? exportOptions = null,
        IReadOnlyDictionary<PatternType, PatternSettings>? patternTemplates = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(outputDirectory);
        Directory.CreateDirectory(outputDirectory);

        exportOptions ??= new ImageExportOptions();
        var exported = new List<string>(14);

        ExportOne(PatternType.Border);

        for (int phase = 1; phase <= 8; phase++)
        {
            ExportOne(PatternType.PhaseStripes, phase);
        }

        ExportOne(PatternType.NinePointGrid);
        ExportOne(PatternType.DistortionGrid);
        ExportOne(PatternType.CorrectionCross);
        ExportOne(PatternType.WhiteRectangle);
        ExportOne(PatternType.Black);

        return exported;

        void ExportOne(PatternType type, int phase = 1)
        {
            PatternSettings? template = null;
            bool hasTemplate = patternTemplates?.TryGetValue(type, out template) == true;
            PatternSettings settings = hasTemplate ? template!.Clone() : PatternPresets.Create(type);
            settings.PatternType = type;
            settings.CanvasWidth = canvasWidth;
            settings.CanvasHeight = canvasHeight;
            settings.Phase = phase;

            if (!hasTemplate)
            {
                settings.DotRadius = dotRadius;
                settings.LineWidth = lineWidth;
                if (type != PatternType.Black)
                {
                    PatternLayout.Center(settings);
                }
            }

            using var image = PatternGenerator.Generate(settings);
            string path = Path.Combine(outputDirectory, PatternFileNames.Get(settings, exportOptions.Format));
            string actualPath = ImageFileWriter.Write(path, image, exportOptions);
            exported.Add(actualPath);
        }
    }
}
