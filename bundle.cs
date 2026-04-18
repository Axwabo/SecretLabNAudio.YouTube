#:property TargetFramework=net10.0

using System.IO.Compression;

const string license = "LICENSE";
const string thirdParty = "THIRD_PARTY_LICENSES";

using var zipFile = File.Create("dependencies.zip");
using var archive = new ZipArchive(zipFile, ZipArchiveMode.Create);

foreach (var file in Directory.EnumerateFiles("."))
{
    var name = Path.GetFileNameWithoutExtension(file.AsSpan());
    if (name is "AngleSharp" or "System.Text.Encoding.CodePages" or "System.Text.Encodings.Web" or "YoutubeExplode")
        archive.CreateEntryFromFile(file, $"bin/{name}");
}

archive.CreateEntryFromFile($"../{license}", $"LICENSES/{license}");

foreach (var file in Directory.EnumerateFiles($"../{thirdParty}"))
    archive.CreateEntryFromFile(file, $"LICENSES/{thirdParty}/{Path.GetFileName(file)}");
