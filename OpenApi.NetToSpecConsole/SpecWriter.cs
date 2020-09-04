using Fclp.Internals.Extensions;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Models;
using Microsoft.OpenApi.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using XmlToOpenApi.Models;

namespace XmlToOpenApi
{
    public static class SpecWriter
    {
        public static string Generate(Arguments args)
        {
            var xmlPaths = GetAbsolutePaths(args.XmlFilenames);
            var assemblyPaths = GetAbsolutePaths(args.AssemblyFilenames);

            var xmlDocs = xmlPaths.Select(x => XDocument.Load(x)).ToList();

            var input = new OpenApiGeneratorConfig(xmlDocs, assemblyPaths, args.DocumentVersion, args.FilterSetVersion);

            if (!string.IsNullOrEmpty(args.AdvancedConfigurationPath))
            {
                Console.WriteLine("Advanced configuration setting detected");
                if (!File.Exists(args.AdvancedConfigurationPath)) throw new FileNotFoundException($"File does not exist: '{args.AdvancedConfigurationPath}'");
                input.AdvancedConfigurationXmlDocument = XDocument.Load(Path.GetFullPath(args.AdvancedConfigurationPath));
            }

            var generationDiagnostic = new GenerationDiagnostic();
            var generator = new OpenApiGenerator();
            var openApiDocuments = generator.GenerateDocuments(input, out generationDiagnostic);

            var documentErrors = generationDiagnostic.DocumentGenerationDiagnostic.Errors;
            var operationErrors = generationDiagnostic.OperationGenerationDiagnostics.SelectMany(x => x.Errors);

            if (!documentErrors.IsNullOrEmpty() || !operationErrors.IsNullOrEmpty())
            {
                var compiledDocumentErrors = string.Join("\n\r", documentErrors.Select(x => $"- {x.ExceptionType}|{x.Message}"));
                var compiledOperationErrors = string.Join("\n\r", operationErrors.Select(x => $"- {x.ExceptionType}|{x.Message}"));
                throw new Exception(string.Join("\n\r", compiledDocumentErrors, compiledOperationErrors));
            }

            if (args.Verbose)
                PrintDiagnostics(generationDiagnostic);

            string outputSpec;
            if (args.OutputYaml)
            {
                outputSpec = openApiDocuments.First().Value.SerializeAsYaml(args.OpenApiSpecVersion);
            }
            else
            {
                outputSpec = openApiDocuments.First().Value.SerializeAsJson(args.OpenApiSpecVersion);
            }

            var outputFilename = BuildFilename(args.OutputFilename, args.OutputYaml);
            File.WriteAllText(BuildFilename(args.OutputFilename, args.OutputYaml), outputSpec);
            return outputFilename;
        }

        private static string BuildFilename(string filename, bool isYaml)
        {
            var filepath = filename is null ? "./openapi_spec" : Path.GetFileNameWithoutExtension(filename);
            if (isYaml)
                return filepath + ".yaml";
            return filepath + ".json";
        }

        private static List<string> GetAbsolutePaths(List<string> filenames)
        {
            var absolutePaths = new List<string>();
            foreach (var filename in filenames)
            {
                if (!File.Exists(filename)) throw new FileNotFoundException($"File does not exist: '{filename}'");

                absolutePaths.Add(Path.GetFullPath(filename));
            }
            return absolutePaths;
        }

        private static void PrintDiagnostics(GenerationDiagnostic generationDiagnostic)
        {
            Console.WriteLine($"### Operation Diagnostics ###");
            foreach (var ogd in generationDiagnostic.OperationGenerationDiagnostics)
            {
                Console.WriteLine($"Operation:{ogd.OperationMethod}");
                Console.WriteLine($"Path:{ogd.Path}");
            }
        }
    }
}

