using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace StarshipTheory.ModInjector
{
    internal static class Extensions
    {
        public static TypeDefinition GetTypeByName(this ModuleDefinition ModuleDef, String Name)
        {
            return ModuleDef.GetTypes().Single(t => t.Name == Name);
        }

        public static FieldDefinition GetFieldByName(this TypeDefinition TypeDef, String Name)
        {
            return TypeDef.Fields.Single(f => f.Name == Name);
        }

        public static PropertyDefinition GetPropertyByName(this TypeDefinition TypeDef, String Name)
        {
            return TypeDef.Properties.Single(f => f.Name == Name);
        }

        public static MethodDefinition GetMethodByName(this TypeDefinition TypeDef, String Name, int? NumParams = null)
        {
            if(NumParams.HasValue)
                return TypeDef.Methods.Single(m => m.Name == Name && m.Parameters.Count == NumParams.Value);
            
            return TypeDef.Methods.Single(m => m.Name == Name);
        }

        public static MethodDefinition GetMethodByName(this TypeDefinition typeDef, String name, TypeReference param1Type)
        {
            return typeDef.Methods.Single(m => m.Name == name && m.Parameters.Count > 0 && m.Parameters.First().ParameterType.Name == param1Type.Name);
        }

        public static void InjectInstructionsToEnd(this ILProcessor Processor, IEnumerable<Instruction> Instructions)
        {
            if(Processor.Body.Instructions.Count > 0)
            {
                var lastInstruction = Processor.Body.Instructions.Last();
                Processor.Replace(lastInstruction, Instructions.First());

                if (Instructions.Count() > 0)
                    Instructions.ToList().GetRange(1, Instructions.Count() - 1).ForEach(instruction => Processor.Append(instruction));
            }
            else
            {
                for (int i = 0; i < Instructions.Count(); i++)
                    Processor.Body.Instructions.Add(Instructions.ElementAt(i));
            }

            if(Instructions.Last().OpCode != OpCodes.Ret)
                Processor.Append(Instruction.Create(OpCodes.Ret));
        }

        public static void InjectInstructionsToStart(this ILProcessor processor, IEnumerable<Instruction> instructions)
        {
            instructions = instructions.Reverse();
            foreach(Instruction I in instructions)
                processor.Body.Instructions.Insert(0, I);
        }

        public static void InsertAfter(this ILProcessor processor, Instruction instruction, IEnumerable<Instruction> instructions)
        {
            Instruction I = instruction;
            for(int i = 0; i < instructions.Count(); i++)
            {
                processor.InsertAfter(I, instructions.ElementAt(i));
                I = instructions.ElementAt(i);
            }
        }

        public static void InsertBefore(this ILProcessor processor, Instruction instruction, IEnumerable<Instruction> instructions)
        {
            Instruction I = instruction;
            for (int i = instructions.Count() - 1; i >= 0; i--)
            {
                processor.InsertBefore(I, instructions.ElementAt(i));
                I = instructions.ElementAt(i);
            }
        }

    }

    internal class PathZ
    {
        public static String Combine(params String[] paths)
        {
            String full_path = "";
            foreach (String path in paths)
                full_path += path.Trim('.', '/', '\\') + System.IO.Path.DirectorySeparatorChar;

            return full_path.Substring(0, full_path.Length - 1);
        }
    }
}
