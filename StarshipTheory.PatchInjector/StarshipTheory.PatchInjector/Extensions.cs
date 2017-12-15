using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace StarshipTheory.PatchInjector
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

        public static MethodDefinition GetMethodByName(this TypeDefinition typeDef, String name, int numParams, TypeReference param1Type)
        {
            return typeDef.Methods.Single(m => m.Name == name && m.Parameters.Count == numParams && m.Parameters.First().ParameterType.Name == param1Type.Name);
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

        public static void CopyMethodTo(this TypeDefinition ownerType, MethodDefinition method, String overrideMethodName = null)
        {
            if (String.IsNullOrEmpty(overrideMethodName))
                overrideMethodName = method.Name;

            MethodDefinition newMethod = null;

            if (method.HasGenericParameters)
            {
                newMethod = new MethodDefinition(overrideMethodName, method.Attributes, ownerType.Module.Import(typeof(Object)));

                foreach (GenericParameter gp in method.GenericParameters)
                    newMethod.GenericParameters.Add(new GenericParameter(gp.Name, newMethod));

                newMethod.ReturnType = newMethod.GenericParameters.Where(gp => gp.Name == method.ReturnType.Name).FirstOrDefault();
            }
            else
                newMethod = new MethodDefinition(overrideMethodName, method.Attributes, ownerType.Module.Import(method.ReturnType));

            newMethod.CallingConvention = method.CallingConvention;
            newMethod.ExplicitThis = method.ExplicitThis;

            foreach (ParameterDefinition pd in method.Parameters)
            {
                ParameterDefinition newPd = new ParameterDefinition(pd.Name, pd.Attributes, ownerType.Module.Import(pd.ParameterType));

                foreach (CustomAttribute attr in pd.CustomAttributes)
                {
                    CustomAttribute nAttr = new CustomAttribute(ownerType.Module.Import(attr.Constructor.Resolve()), attr.GetBlob());
                    if (attr.HasConstructorArguments)
                    {
                        foreach (CustomAttributeArgument arg in attr.ConstructorArguments)
                        {
                            if (arg.Type == null)
                                nAttr.ConstructorArguments.Add(new CustomAttributeArgument());
                            else
                                nAttr.ConstructorArguments.Add(new CustomAttributeArgument(ownerType.Module.Import(arg.Type.Resolve()), arg.Value));
                        }
                    }

                    if (attr.HasFields)
                    {
                        foreach (CustomAttributeNamedArgument arg in attr.Fields)
                        {
                            if (String.IsNullOrEmpty(arg.Name))
                                nAttr.Fields.Add(new CustomAttributeNamedArgument());
                            else
                                nAttr.Fields.Add(new CustomAttributeNamedArgument(arg.Name, arg.Argument));
                        }
                    }

                    if (attr.HasProperties)
                    {
                        foreach (CustomAttributeNamedArgument arg in attr.Properties)
                        {
                            if (String.IsNullOrEmpty(arg.Name))
                                nAttr.Properties.Add(new CustomAttributeNamedArgument());
                            else
                                nAttr.Properties.Add(new CustomAttributeNamedArgument(arg.Name, arg.Argument));
                        }
                    }

                    newPd.CustomAttributes.Add(nAttr);
                }

                if (pd.HasConstant)
                    newPd.Constant = pd.Constant;

                newMethod.Parameters.Add(newPd);
            }

            foreach (CustomAttribute attr in method.CustomAttributes)
            {
                newMethod.CustomAttributes.Add(new CustomAttribute(ownerType.Module.Import(attr.Constructor.Resolve()), attr.GetBlob()));
            }

            Instruction[] instructions = method.Body.Instructions.ToArray();
            ILProcessor processor = newMethod.Body.GetILProcessor();

            foreach (Instruction inst in instructions)
            {
                if (inst.OpCode == OpCodes.Ldtoken && ((TypeReference)inst.Operand).Name == "HelperPlaceHolder")
                    processor.Append(Instruction.Create(inst.OpCode, ownerType));
                else if (inst.Operand is MethodReference)
                    processor.Append(Instruction.Create(inst.OpCode, ownerType.Module.Import(((MethodReference)inst.Operand).Resolve())));
                else if (inst.Operand is FieldReference)
                    processor.Append(Instruction.Create(inst.OpCode, ownerType.Module.Import(((FieldReference)inst.Operand).Resolve())));
                else if (inst.Operand is TypeReference)
                {
                    if (((TypeReference)inst.Operand).Name == "HelperGenericClass")
                        processor.Append(Instruction.Create(inst.OpCode, ownerType.Module.Import(((TypeReference)inst.Operand).Resolve())));
                    else if (((TypeReference)inst.Operand).IsGenericParameter)
                        processor.Append(Instruction.Create(inst.OpCode, newMethod.GenericParameters.Where(gp => gp.Name == ((TypeReference)inst.Operand).Name).FirstOrDefault()));
                    else
                        processor.Append(Instruction.Create(inst.OpCode, ownerType.Module.Import(((TypeReference)inst.Operand).Resolve())));
                }
                else if (inst.Operand is Instruction)
                    processor.Append(Instruction.Create(inst.OpCode, inst.Operand as Instruction));
                else
                    processor.Append(inst);
            }

            //COPY BODY
            newMethod.Body.MaxStackSize = method.Body.MaxStackSize;
            if (method.Body.HasExceptionHandlers)
            {
                foreach (ExceptionHandler eh in method.Body.ExceptionHandlers)
                {
                    ExceptionHandler nEh = new ExceptionHandler(eh.HandlerType);

                    nEh.CatchType = eh.CatchType == null ? null : ownerType.Module.Import(eh.CatchType);
                    nEh.FilterStart = eh.FilterStart == null ? null : newMethod.Body.Instructions.Where(i => i.Offset == eh.FilterStart.Offset).FirstOrDefault();
                    nEh.HandlerStart = eh.HandlerStart == null ? null : newMethod.Body.Instructions.Where(i => i.Offset == eh.HandlerStart.Offset).FirstOrDefault();
                    nEh.HandlerEnd = eh.HandlerEnd == null ? null : newMethod.Body.Instructions.Where(i => i.Offset == eh.HandlerEnd.Offset).FirstOrDefault();
                    nEh.TryStart = eh.TryStart == null ? null : newMethod.Body.Instructions.Where(i => i.Offset == eh.TryStart.Offset).FirstOrDefault();
                    nEh.TryEnd = eh.TryEnd == null ? null : newMethod.Body.Instructions.Where(i => i.Offset == eh.TryEnd.Offset).FirstOrDefault();

                    newMethod.Body.ExceptionHandlers.Add(nEh);
                }
            }

            if (method.Body.HasVariables)
            {
                foreach (VariableDefinition vd in method.Body.Variables)
                {
                    if (vd.VariableType.IsGenericParameter)
                        newMethod.Body.Variables.Add(new VariableDefinition(vd.Name, newMethod.GenericParameters.Where(gp => gp.Name == vd.VariableType.Name).FirstOrDefault()));
                    else
                        newMethod.Body.Variables.Add(new VariableDefinition(vd.Name, ownerType.Module.Import(vd.VariableType.Resolve())));
                }
            }

            ownerType.Methods.Add(newMethod);
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
