using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace StarshipTheory.PatchInjector
{
    class InjectionHelper
    {
        internal static class HelperPlaceHolder
        {

        }

        internal class HelperGenericClass
        {

        }

        public static InjectionHelper Instance { get; private set; }
        private static TypeReference NullType;

        private PatchInjector _Injector;

        public InjectionHelper(PatchInjector Injector)
        {
            Instance = this;
            _Injector = Injector;

            NullType = Patches.BasePatch.GameModule.Import(typeof(void));
        }

        public static T _Mod_GetGenericStaticFieldValue<T>(String name)
        {
            System.Reflection.FieldInfo field = typeof(HelperPlaceHolder).GetType().GetField(name);
            return (T)field.GetValue(null);
        }

        public T _Mod_GetGenericFieldValue<T>(String name)
        {
            System.Reflection.FieldInfo field = this.GetType().GetField(name);
            return (T)field.GetValue(field.IsStatic ? null : this);
        }

        public static void _Mod_SetStaticFieldValue(String name, Object value)
        {
            System.Reflection.FieldInfo field = typeof(HelperPlaceHolder).GetField(name);
            field.SetValue(null, value);
        }

        public void _Mod_SetFieldValue(String name, Object value)
        {
            System.Reflection.FieldInfo field = this.GetType().GetField(name);
            field.SetValue(field.IsStatic ? null : this, value);
        }


        public static T _Mod_GetGenericStaticPropertyValue<T>(String name)
        {
            System.Reflection.PropertyInfo prop = typeof(HelperPlaceHolder).GetProperty(name);
            return (T)prop.GetValue(null, null);
        }

        public static T _Mod_GetGenericStaticPropertyValue<T>(String name, Object[] index)
        {
            System.Reflection.PropertyInfo prop = typeof(HelperPlaceHolder).GetProperty(name);
            return (T)prop.GetValue(null, index);
        }

        public T _Mod_GetGenericPropertyValue<T>(string name)
        {
            System.Reflection.PropertyInfo prop = this.GetType().GetProperty(name);
            return (T)prop.GetValue(prop.GetGetMethod().IsStatic ? null : this, null);
        }

        public T _Mod_GetGenericPropertyValue<T>(string name, Object[] index)
        {
            System.Reflection.PropertyInfo prop = this.GetType().GetProperty(name);
            return (T)prop.GetValue(prop.GetGetMethod().IsStatic ? null : this, index);
        }

        public static void _Mod_SetStaticPropertyValue(String name, Object value)
        {
            System.Reflection.PropertyInfo prop = typeof(HelperPlaceHolder).GetProperty(name);
            prop.SetValue(null, value, null);
        }

        public static void _Mod_SetStaticPropertyValue(String name, Object value, Object[] index)
        {
            System.Reflection.PropertyInfo prop = typeof(HelperPlaceHolder).GetProperty(name);
            prop.SetValue(null, value, index);
        }

        public void _Mod_SetPropertyValue(String name, Object value)
        {
            System.Reflection.PropertyInfo prop = this.GetType().GetProperty(name);
            prop.SetValue(prop.GetSetMethod().IsStatic ? null : this, value, null);
        }

        public void _Mod_SetPropertyValue(String name, Object value, Object[] index)
        {
            System.Reflection.PropertyInfo prop = this.GetType().GetProperty(name);
            prop.SetValue(prop.GetSetMethod().IsStatic ? null : this, value, index);
        }

        public static T _Mod_GenericCallStaticMethod<T>(String name, params Object[] parameters)
        {
            System.Reflection.MethodInfo method = typeof(HelperPlaceHolder).GetMethod(name);
            return (T)method.Invoke(null, parameters);
        }

        public static Object _Mod_CallStaticMethod(String name, params Object[] parameters)
        {
            System.Reflection.MethodInfo method = typeof(HelperPlaceHolder).GetMethod(name);
            return method.Invoke(null, parameters);
        }

        public Object _Mod_CallMethod(String name, params Object[] parameters)
        {
            System.Reflection.MethodInfo method = this.GetType().GetMethod(name);
            return method.Invoke(method.IsStatic ? null : this, parameters);
        }

        public T _Mod_GenericCallMethod<T>(String name, params Object[] parameters)
        {
            System.Reflection.MethodInfo method = this.GetType().GetMethod(name);
            return (T)method.Invoke(method.IsStatic ? null : this, parameters);
        }

        private void CopyMethodTo(MethodDefinition method, TypeDefinition ownerType, String methodName = null, Type genericType = null)
        {
            if (String.IsNullOrEmpty(methodName))
                methodName = method.Name;

            MethodDefinition newMethod = null;

            if (method.HasGenericParameters)
            {
                newMethod = new MethodDefinition(methodName, method.Attributes, ownerType.Module.Import(typeof(Object)));

                foreach (GenericParameter gp in method.GenericParameters)
                    newMethod.GenericParameters.Add(new GenericParameter(gp.Name, newMethod));

                newMethod.ReturnType = newMethod.GenericParameters.Where(gp => gp.Name == method.ReturnType.Name).FirstOrDefault();
            }
            else
                newMethod = new MethodDefinition(methodName, method.Attributes, ownerType.Module.Import(method.ReturnType));

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

        public void CreateModMethods(TypeDefinition ownerType)
        {
            TypeDefinition helperType = AssemblyDefinition.ReadAssembly(System.Reflection.Assembly.GetEntryAssembly().Location).MainModule.GetTypeByName("InjectionHelper");

            if (ownerType.IsPublic && ownerType.IsAbstract && ownerType.IsSealed) //Static class
            {
                if (ownerType.HasFields)
                {
                    ownerType.CopyMethodTo(helperType.GetMethodByName("_Mod_GetGenericStaticFieldValue"), "_Mod_GetFieldValue");
                    ownerType.CopyMethodTo(helperType.GetMethodByName("_Mod_SetStaticFieldValue"), "_Mod_SetFieldValue");
                }

                if (ownerType.HasProperties)
                {
                    ownerType.CopyMethodTo(helperType.GetMethodByName("_Mod_GetGenericStaticPropertyValue", 1), "_Mod_GetPropertyValue");
                    ownerType.CopyMethodTo(helperType.GetMethodByName("_Mod_GetGenericStaticPropertyValue", 2), "_Mod_GetPropertyValue");
                    ownerType.CopyMethodTo(helperType.GetMethodByName("_Mod_SetStaticPropertyValue", 2), "_Mod_SetPropertyValue");
                    ownerType.CopyMethodTo(helperType.GetMethodByName("_Mod_SetStaticPropertyValue", 3), "_Mod_SetPropertyValue");
                }

                if (ownerType.HasMethods)
                    ownerType.CopyMethodTo(helperType.GetMethodByName("_Mod_GenericCallStaticMethod"), "_Mod_CallMethod");
            }
            else if (ownerType.IsPublic && !ownerType.IsAbstract && !ownerType.IsInterface) //Instance class
            {
                if (ownerType.HasFields)
                {
                    ownerType.CopyMethodTo(helperType.GetMethodByName("_Mod_GetGenericFieldValue"), "_Mod_GetFieldValue");
                    ownerType.CopyMethodTo(helperType.GetMethodByName("_Mod_SetFieldValue"));
                }

                if (ownerType.HasProperties)
                {
                    ownerType.CopyMethodTo(helperType.GetMethodByName("_Mod_GetGenericPropertyValue", 1), "_Mod_GetPropertyValue");
                    ownerType.CopyMethodTo(helperType.GetMethodByName("_Mod_GetGenericPropertyValue", 2), "_Mod_GetPropertyValue");
                    ownerType.CopyMethodTo(helperType.GetMethodByName("_Mod_SetPropertyValue", 2));
                    ownerType.CopyMethodTo(helperType.GetMethodByName("_Mod_SetPropertyValue", 3));

                }

                if (ownerType.HasMethods)
                {
                    ownerType.CopyMethodTo(helperType.GetMethodByName("_Mod_GenericCallMethod"), "_Mod_CallMethod");
                }
            }
        }

        public void OverrideFieldWithProperty(FieldDefinition field, MethodDefinition onFieldChangedEvent = null)
        {
            PropertyDefinition propDef = new PropertyDefinition(field.Name, PropertyAttributes.None, field.FieldType);
            field.Name = "__" + field.Name;

            MethodAttributes attributes = MethodAttributes.Public | MethodAttributes.HideBySig;

            if (field.IsStatic)
                attributes = attributes | MethodAttributes.Static;

            MethodDefinition getMethod = new MethodDefinition("get_" + propDef.Name, attributes, propDef.PropertyType);

            ILProcessor processor = getMethod.Body.GetILProcessor();

            List<Instruction> instructions = new List<Instruction> {
                Instruction.Create(OpCodes.Ldarg_0),
                Instruction.Create(OpCodes.Ldfld, Patches.BasePatch.GameModule.Import(field)),
                Instruction.Create(OpCodes.Ret)
            };

            if (field.IsStatic)
                instructions[1].OpCode = OpCodes.Ldsflda;

            processor.InjectInstructionsToEnd(instructions);
            propDef.GetMethod = getMethod;

            MethodDefinition setMethod = new MethodDefinition("set_" + propDef.Name, attributes, NullType);
            setMethod.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.None, Patches.BasePatch.GameModule.Import(propDef.PropertyType)));

            instructions.Clear();

            if (onFieldChangedEvent != null)
            {
                instructions.AddRange(new Instruction[] {
                    Instruction.Create(OpCodes.Ldarg_0),
                    Instruction.Create(OpCodes.Ldstr, propDef.Name),
                    Instruction.Create(OpCodes.Ldarg_1),
                    Instruction.Create(OpCodes.Callvirt, Patches.BasePatch.GameModule.Import(onFieldChangedEvent))
                });
            }

            instructions.AddRange(new Instruction[] {
                Instruction.Create(OpCodes.Ldarg_0),
                Instruction.Create(OpCodes.Ldarg_1),
                Instruction.Create(OpCodes.Stfld, Patches.BasePatch.GameModule.Import(field)),
                Instruction.Create(OpCodes.Ret)
            });

            processor = setMethod.Body.GetILProcessor();

            processor.InjectInstructionsToEnd(instructions);

            propDef.SetMethod = setMethod;

            field.DeclaringType.Methods.Add(getMethod);
            field.DeclaringType.Methods.Add(setMethod);

            field.DeclaringType.Properties.Add(propDef);
        }
    }
}
