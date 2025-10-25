using System.Collections.ObjectModel;

namespace PdfGhostScriptWrapper;

public class GhostScriptOptions
{
    public GhostScriptOptions(string inputFile, string outputFile)
    {
        InputFile = inputFile ?? throw new ArgumentNullException(nameof(inputFile));
        OutputFile = outputFile ?? throw new ArgumentNullException(nameof(outputFile));
    }

    public string InputFile { get; }

    public string OutputFile { get; }

    public bool SubsetFonts { get; set; } = true;

    public bool CompressFonts { get; set; } = true;

    public string? PdfSettingsPreset { get; set; }
        = null;

    public IList<string> AdditionalOptions { get; } = new List<string>();

    public ReadOnlyCollection<string> BuildArguments()
    {
        var args = new List<string>
        {
            "-dBATCH",
            "-dNOPAUSE",
            "-dSAFER",
            "-sDEVICE=pdfwrite",
            $"-sOutputFile={OutputFile}",
            $"-dSubsetFonts={(SubsetFonts ? "true" : "false")}",
            $"-dCompressFonts={(CompressFonts ? "true" : "false")}",
            InputFile
        };

        if (!string.IsNullOrWhiteSpace(PdfSettingsPreset))
        {
            var preset = PdfSettingsPreset!.StartsWith("/")
                ? PdfSettingsPreset!
                : $"/{PdfSettingsPreset}";
            args.Insert(4, $"-dPDFSETTINGS={preset}");
        }

        args.InsertRange(args.Count - 1, AdditionalOptions);

        return args.AsReadOnly();
    }
}
