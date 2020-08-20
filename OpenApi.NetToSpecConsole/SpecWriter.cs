using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Models;
using Microsoft.OpenApi.Extensions;
using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using XmlToOpenApi.Models;

namespace XmlToOpenApi
{
    public static class SpecWriter
    {
        public static void Generate(Arguments args)
        {
            var xmlDocs = args.XmlFilenames.Select(x => XDocument.Load(x)).ToList();

            var input = new OpenApiGeneratorConfig(xmlDocs, args.AssemblyFilenames, args.DocumentVersion, args.FilterSetVersion);

            var generationDiagnostic = new GenerationDiagnostic();
            var generator = new OpenApiGenerator();
            var openApiDocuments = generator.GenerateDocuments(input, out generationDiagnostic);

            if (args.Verbose)
                PrintDiagnostics(generationDiagnostic);

            string outputSpec;
            if (args.OutputYaml)
                outputSpec = openApiDocuments.First().Value.SerializeAsYaml(args.OpenApiSpecVersion);
            outputSpec = openApiDocuments.First().Value.SerializeAsJson(args.OpenApiSpecVersion);

            File.WriteAllText(BuildFilename(args.OutputFilename, args.OutputYaml), outputSpec);
        }

        private static string BuildFilename(string filename, bool isYaml)
        {
            var filepath = filename is null ? "./openapi_spec" : Path.GetFileNameWithoutExtension(filename);
            if (isYaml)
                return filepath + ".yaml";
            return filepath + ".json";
        }

        private static void PrintDiagnostics(GenerationDiagnostic generationDiagnostic)
        {
            Console.WriteLine($"### Operation Diagnostics ###");
            foreach (var ogd in generationDiagnostic.OperationGenerationDiagnostics)
            {
                Console.WriteLine($"Operation:{ogd.OperationMethod}");
                Console.WriteLine($"Path:{ogd.Path}");
                foreach (var error in ogd.Errors.Select(x => $"{x.ExceptionType}-{x.Message}"))
                    Console.WriteLine($"Error:{error}");
            }

            if (generationDiagnostic.DocumentGenerationDiagnostic.Errors.Count > 0)
            {
                Console.WriteLine($"### Document Generation Errors ###");
                foreach (var error in generationDiagnostic.DocumentGenerationDiagnostic.Errors.Select(x => $"Error: {x.ExceptionType}-{x.Message}"))
                {
                    Console.WriteLine(error);
                }
            }
        }
    }
}

