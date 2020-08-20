using Fclp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using XmlToOpenApi.Models;

namespace XmlToOpenApi
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var p = new FluentCommandLineParser<Arguments>();

                p.Setup(arg => arg.XmlFilenames).As('x', "xmlFilenames").WithDescription("Path to xml documents").Required().Callback(DoFilesExist);
                p.Setup(arg => arg.AssemblyFilenames).As('a', "assemblyFilenames").WithDescription("Path to assembly(*.dll) files").Required().Callback(DoFilesExist);
                p.Setup(arg => arg.OutputFilename).As('o', "outputFilename").WithDescription("Name of the output file(without extension)");
                p.Setup(arg => arg.OutputYaml).As('y', "outputYaml").WithDescription("Output YAML instead of JSON");
                p.Setup(arg => arg.DocumentVersion).As('d', "documentVersion").WithDescription("What version of API is this");
                p.Setup(arg => arg.FilterSetVersion).As('f', "filterSetVersion").WithDescription("What version of OpenAPI should the document be written in");
                p.Setup(arg => arg.Verbose).As('v', "verbose").WithDescription("Enable detailed logging");
                p.Setup(arg => arg.OpenApiSpecVersion).As('s', "openApiSpecVersion").WithDescription("What version of OpenAPI spec should the document be written in");
                p.SetupHelp("?", "help").Callback(text => Console.WriteLine(text));
                
                var result = p.Parse(args);

                if (result.HasErrors)
                {
                    var argNames = string.Join(", ", result.Errors.Select(x => $"--{x.Option.LongName}(--{x.Option.ShortName})"));
                    Console.WriteLine($"Error with the following args: {argNames}");
                    return;
                }
                SpecWriter.Generate(p.Object);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Error occured, exiting...");
                return;
            }
        }

        private static void DoFilesExist(List<string> filenames)
        {
            foreach (var filename in filenames)
            {
                if (!File.Exists(filename))
                {
                    throw new FileNotFoundException($"File does not exist: '{filename}'");
                }
            }
        }
    }
}
