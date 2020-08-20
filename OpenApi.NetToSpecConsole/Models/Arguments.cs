using Microsoft.OpenApi;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration;
using System.Collections.Generic;

namespace XmlToOpenApi.Models
{
    public class Arguments
    {
        public List<string> AssemblyFilenames { get; set; }
        public string DocumentVersion { get; set; } = "V1";
        public FilterSetVersion FilterSetVersion { get; set; } = FilterSetVersion.V1;
        public OpenApiSpecVersion OpenApiSpecVersion { get; set; } = OpenApiSpecVersion.OpenApi3_0;
        public bool OutputYaml { get; set; } = false;
        public string OutputFilename { get; set; }
        public bool Verbose { get; set; } = false;
        public List<string> XmlFilenames { get; set; }
    }
}
