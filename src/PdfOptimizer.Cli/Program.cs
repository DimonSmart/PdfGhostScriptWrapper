using PdfGhostScriptWrapper;

if (args.Length == 0 || args.Contains("--help") || args.Contains("-h"))
{
    PrintUsage();
    return args.Length == 0 ? 1 : 0;
}

if (args.Length < 2)
{
    Console.Error.WriteLine("Input and output file paths must be provided.");
    PrintUsage();
    return 1;
}

var inputFile = args[0];
var outputFile = args[1];

var options = new GhostScriptOptions(inputFile, outputFile);
var additionalOptions = options.AdditionalOptions;

string? ghostScriptPath = null;

try
{
    for (var i = 2; i < args.Length; i++)
    {
        var arg = args[i];
        switch (arg)
        {
            case "--subset-fonts":
                options.SubsetFonts = true;
                break;
            case "--no-subset-fonts":
                options.SubsetFonts = false;
                break;
            case "--compress-fonts":
                options.CompressFonts = true;
                break;
            case "--no-compress-fonts":
                options.CompressFonts = false;
                break;
            case "--preset":
                EnsureHasValue(args, i, arg);
                options.PdfSettingsPreset = args[++i];
                break;
            case "--ghostscript":
                EnsureHasValue(args, i, arg);
                ghostScriptPath = args[++i];
                break;
            case "--gs-arg":
                EnsureHasValue(args, i, arg);
                additionalOptions.Add(args[++i]);
                break;
            case "--":
                for (var j = i + 1; j < args.Length; j++)
                {
                    additionalOptions.Add(args[j]);
                }
                i = args.Length;
                break;
            default:
                Console.Error.WriteLine($"Unknown option '{arg}'.");
                PrintUsage();
                return 1;
        }
    }
}
catch (ArgumentException ex)
{
    Console.Error.WriteLine(ex.Message);
    PrintUsage();
    return 1;
}

var runner = new GhostScriptRunner(ghostScriptPath);

try
{
    var result = await runner.ExecuteAsync(options);

    if (!string.IsNullOrWhiteSpace(result.StandardOutput))
    {
        Console.WriteLine(result.StandardOutput.TrimEnd());
    }

    if (!string.IsNullOrWhiteSpace(result.StandardError))
    {
        Console.Error.WriteLine(result.StandardError.TrimEnd());
    }

    return 0;
}
catch (GhostScriptExecutionException ex)
{
    Console.Error.WriteLine(ex.Message);
    if (!string.IsNullOrWhiteSpace(ex.Result.StandardError))
    {
        Console.Error.WriteLine(ex.Result.StandardError.TrimEnd());
    }

    return ex.Result.ExitCode != 0 ? ex.Result.ExitCode : 1;
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Unhandled error: {ex.Message}");
    return 1;
}

static void EnsureHasValue(string[] arguments, int currentIndex, string option)
{
    if (currentIndex + 1 >= arguments.Length)
    {
        throw new ArgumentException($"Option '{option}' requires a value.");
    }
}

static void PrintUsage()
{
    const string usage = """
PDF optimizer utility built on top of GhostScript.

Usage: pdf-optimizer <input.pdf> <output.pdf> [options]

Options:
  --subset-fonts           Enable font subsetting (default).
  --no-subset-fonts        Disable font subsetting.
  --compress-fonts         Enable font compression (default).
  --no-compress-fonts      Disable font compression.
  --preset <name>          Apply a GhostScript PDF preset (screen, ebook, printer, prepress, default).
  --ghostscript <path>     Path to the GhostScript executable.
  --gs-arg <value>         Forward a raw argument to GhostScript (can be repeated).
  --                       Treat all following values as raw GhostScript arguments.
  -h, --help               Display this message.
""";

    Console.WriteLine(usage);
}
