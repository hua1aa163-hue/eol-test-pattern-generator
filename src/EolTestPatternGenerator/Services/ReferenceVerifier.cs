using EolTestPatternGenerator.Models;
using OpenCvSharp;

namespace EolTestPatternGenerator.Services;

public static class ReferenceVerifier
{
    public static int Run(string referenceDirectory)
    {
        string fullDirectory = Path.GetFullPath(referenceDirectory);
        Console.WriteLine($"参考目录：{fullDirectory}");

        var cases = new List<(PatternSettings Settings, bool BinaryThreshold)>();
        cases.Add((PatternPresets.Create(PatternType.Border), false));

        for (int phase = 1; phase <= 8; phase++)
        {
            PatternSettings phaseSettings = PatternPresets.Create(PatternType.PhaseStripes);
            phaseSettings.Phase = phase;
            cases.Add((phaseSettings, false));
        }

        cases.Add((PatternPresets.Create(PatternType.NinePointGrid), true));
        cases.Add((PatternPresets.Create(PatternType.DistortionGrid), true));
        cases.Add((PatternPresets.Create(PatternType.CorrectionCross), true));
        cases.Add((PatternPresets.Create(PatternType.WhiteRectangle), false));
        cases.Add((PatternPresets.Create(PatternType.Black), false));

        bool allPassed = true;

        foreach ((PatternSettings settings, bool binaryThreshold) in cases)
        {
            string fileName = PatternFileNames.Get(settings);
            string referencePath = Path.Combine(fullDirectory, fileName);

            if (!File.Exists(referencePath))
            {
                Console.WriteLine($"[缺失] {fileName}");
                allPassed = false;
                continue;
            }

            using var generated = PatternGenerator.Generate(settings);
            using var reference = Cv2.ImDecode(File.ReadAllBytes(referencePath), ImreadModes.Color);

            if (reference.Empty())
            {
                Console.WriteLine($"[读取失败] {fileName}");
                allPassed = false;
                continue;
            }

            long differences = CountPixelDifferences(generated, reference, binaryThreshold);
            string comparison = binaryThreshold ? "阈值几何" : "逐像素";
            Console.WriteLine(differences == 0
                ? $"[通过] {fileName,-42} {comparison}差异 0"
                : $"[失败] {fileName,-42} {comparison}差异 {differences:N0} 像素");
            allPassed &= differences == 0;
        }

        Console.WriteLine(allPassed ? "全部 14 张图卡验证通过。" : "存在验证失败项。");
        return allPassed ? 0 : 1;
    }

    private static unsafe long CountPixelDifferences(Mat generated, Mat reference, bool binaryThreshold)
    {
        if (generated.Rows != reference.Rows || generated.Cols != reference.Cols ||
            generated.Type() != MatType.CV_8UC3 || reference.Type() != MatType.CV_8UC3)
        {
            return long.MaxValue;
        }

        long differences = 0;
        int rows = generated.Rows;
        int columns = generated.Cols;

        for (int y = 0; y < rows; y++)
        {
            byte* generatedRow = (byte*)generated.Ptr(y);
            byte* referenceRow = (byte*)reference.Ptr(y);

            for (int x = 0; x < columns; x++)
            {
                int offset = x * 3;

                if (binaryThreshold)
                {
                    bool generatedWhite = generatedRow[offset] >= 128;
                    bool referenceWhite = referenceRow[offset] >= 128;
                    differences += generatedWhite == referenceWhite ? 0 : 1;
                }
                else if (generatedRow[offset] != referenceRow[offset] ||
                         generatedRow[offset + 1] != referenceRow[offset + 1] ||
                         generatedRow[offset + 2] != referenceRow[offset + 2])
                {
                    differences++;
                }
            }
        }

        return differences;
    }
}
