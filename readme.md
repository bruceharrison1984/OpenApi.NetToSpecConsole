# OpenApi.NetToSpecConsole
This is a very simple console app that allows you to take the XML documentation output provided by [microsoft/OpenAPI.NET.CSharpAnnotations](https://https://github.com/Microsoft/OpenAPI.NET.CSharpAnnotations), and generate an OpenAPI spec. This is really just a very thin wrapper around the `OpenApiGenerator` found in the CSharpAnnotations library.

## Use Case
I created this because I wanted to generate an OpenAPI spec as part of an after build step. The current CSharpAnnotations library requires post processing by the `OpenApiGenerator` class, but this is only accessible while within application code. In order to create the spec as a post-build step, an external program is required.

## Console Usage
You must already have your application setup to output XML documentation per the [microsoft/OpenAPI.NET.CSharpAnnotations](https://https://github.com/Microsoft/OpenAPI.NET.CSharpAnnotations) documentation. You then just need to pass the application DLLs and the generated XML files in to the executable

```sh
$ ./OpenApi.NetToSpecConsole.exe -?

        a:assemblyFilenames             Path to assembly(*.dll) files
        d:documentVersion               What version of API is this
        f:filterSetVersion              What version of OpenAPI should the document be written in
        o:outputFilename                Name of the output file(without extension)
        s:openApiSpecVersion            What version of OpenAPI spec should the document be written in
        v:verbose                       Enable detailed logging
        x:xmlFilenames                  Path to xml documents
        y:outputYaml                    Output YAML instead of JSON
        c:advancedConfig                Path to an advanced configuration file
```

## Console Example
```sh
> ./OpenApi.NetToSpecConsole.exe --assemblyFilenames "<relative-or-absolute-path-to.dll>" "<relative-or-absolute-path-to.dll>" --xmlFilenames "<relative-or-absolute-path-to.xml>"
```

## Post-build Step (*.csproj)
```xml
  <Target Name="CreateOpenApiSpec" AfterTargets="AfterBuild" Condition="'$(Configuration)' == 'OpenApi'">
    <Exec Command=".\OpenApi\OpenApi.NetToSpecConsole.exe -a bin\$(Configuration)\$(TargetFramework)\Api.dll bin\$(Configuration)\$(TargetFramework)\Data.dll -x bin\$(Configuration)\$(TargetFramework)\Api.xml -o .\OpenApi\openapi_spec.json -v " />
  </Target>
```
