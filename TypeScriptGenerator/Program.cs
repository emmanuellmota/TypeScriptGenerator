using CommandLine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace TypeScriptGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
              .WithParsed(RunOptions)
              .WithNotParsed(HandleParseError);
        }
        static void RunOptions(Options opts)
        {
            Assembly assembly = Assembly.LoadFrom(opts.Input);
            TypeScriptBuilder.TypeScriptGenerator ts = new TypeScriptBuilder.TypeScriptGenerator(new TypeScriptBuilder.TypeScriptGeneratorOptions
            {
                UseCamelCase = opts.UseCamelCase,
                EmitIinInterface = opts.EmitIinInterface,
                EmitReadonly = opts.EmitReadonly,
                EmitComments = opts.EmitComments,
                IgnoreNamespaces = opts.IgnoreNamespaces
            });

            foreach (Type t in assembly.GetTypes())
            {
                if (!opts.ExcludeType.Any(j => t.Name == j))
                {
                    ts.AddCSType(t);
                }
            }

            Directory.CreateDirectory(opts.Output);
            ts.Store(Path.Join(opts.Output, assembly.GetName().Name + ".ts"));
        }
        static void HandleParseError(IEnumerable<Error> errs)
        {
            //handle errors
        }
    }

    class Options
    {
        [Value(0, Required = true, MetaName = "input-dll", HelpText = "Input dll.")]
        public string Input { get; set; }

        [Option('o', "output-directory", Required = false, HelpText = "Output file path (default STDOUT).")]
        public string Output { get; set; } = "./";

        [Option('e', "exclude-types", Required = false, HelpText = "Classes, structs or fields/properties omitted during TS code generation.")]
        public IEnumerable<string> ExcludeType { get; set; } = new string[] { };

        [Option('c', "use-camel-case", Required = false, HelpText = "Changes field names form MyTestField to myTestField (default true).")]
        public bool UseCamelCase { get; set; } = true;

        [Option('i', "emit-i-in-interface", Required = false, HelpText = "Adds I in interface names, MySimpleData becomes IMySimpleData (default true).")]
        public bool EmitIinInterface { get; set; } = true;

        [Option('r', "emit-readonly", Required = false, HelpText = "Adds readonly to readonly fields, requires TypeScript 2.0 (default true).")]
        public bool EmitReadonly { get; set; } = true;

        [Option('m', "emit-comments", Required = false, HelpText = "Adds comments with oryginal C# type description (default false).")]
        public bool EmitComments { get; set; } = false;

        [Option('n', "ignore-namespaces", Required = false, HelpText = "Ignores namespace in emissions (default false).")]
        public bool IgnoreNamespaces { get; set; } = false;
    }
}
