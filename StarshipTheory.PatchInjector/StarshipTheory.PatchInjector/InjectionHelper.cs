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
        public static InjectionHelper Instance { get; private set; }
        private static TypeReference NullType;

        private PatchInjector _Injector;
        
        private TypeReference TypeTypeRef = null;
        private TypeReference ObjectTypeRef = null;
        private TypeReference ObjectArrayTypeRef = null;
        private TypeReference StringTypeRef = null;
        private TypeReference MethodInfoRef = null;
        private TypeReference MethodBaseRef = null;

        private MethodReference TypeGetTypeFromHandle = null;
        private MethodReference ObjectGetTypeRef = null;
        private MethodReference TypeGetMethodRef = null;
        private MethodReference MethodBaseInvokeRef = null;

        private MethodReference ObjectGetFieldRef = null;
        private MethodReference FieldInfoGetValueRef = null;
        private MethodReference FieldInfoSetValueRef = null;

        private MethodReference ObjectGetPropertyRef = null;
        private MethodReference PropertyInfoGetValueRef = null;
        private MethodReference PropertyInfoSetValueRef = null;

        private MethodReference ParamArrayAttributeRef = null;

        public InjectionHelper(PatchInjector Injector)
        {
            Instance = this;
            _Injector = Injector;

            NullType = Patches.BasePatch.GameModule.Import(typeof(void));
        }

        internal void SetupReferences(ModuleDefinition module)
        {
            if (TypeTypeRef == null)
                TypeTypeRef = module.Import(typeof(Type));

            if (ObjectTypeRef == null)
                ObjectTypeRef = module.Import(typeof(System.Object));

            if (ObjectArrayTypeRef == null)
                ObjectArrayTypeRef = module.Import(typeof(System.Object[]));

            if (StringTypeRef == null)
                StringTypeRef = module.Import(typeof(String));

            if (MethodInfoRef == null)
                MethodInfoRef = module.Import(typeof(System.Reflection.MethodInfo));

            if (MethodBaseRef == null)
                MethodBaseRef = module.Import(typeof(System.Reflection.MethodBase));

            if (TypeGetTypeFromHandle == null)
                TypeGetTypeFromHandle = module.Import(_Injector.GetModuleZ("mscorlib.dll").GetTypeByName("Type").GetMethodByName("GetTypeFromHandle"));

            if (ObjectGetTypeRef == null)
                ObjectGetTypeRef = module.Import(_Injector.GetModuleZ("mscorlib.dll").GetTypeByName("Object").GetMethodByName("GetType"));

            if (TypeGetMethodRef == null)
                TypeGetMethodRef = module.Import(_Injector.GetModuleZ("mscorlib.dll").GetTypeByName("Type").GetMethodByName("GetMethod", 1, StringTypeRef));

            if (MethodBaseInvokeRef == null)
                MethodBaseInvokeRef = module.Import(_Injector.GetModuleZ("mscorlib.dll").GetTypeByName("MethodBase").GetMethodByName("Invoke", 2));

            if (ObjectGetFieldRef == null)
                ObjectGetFieldRef = module.Import(_Injector.GetModuleZ("mscorlib.dll").GetTypeByName("Type").GetMethodByName("GetField", 1, StringTypeRef));

            if (FieldInfoGetValueRef == null)
                FieldInfoGetValueRef = module.Import(_Injector.GetModuleZ("mscorlib.dll").GetTypeByName("FieldInfo").GetMethodByName("GetValue"));

            if (FieldInfoSetValueRef == null)
                FieldInfoSetValueRef = module.Import(_Injector.GetModuleZ("mscorlib.dll").GetTypeByName("FieldInfo").GetMethodByName("SetValue", 2));

            if (ObjectGetPropertyRef == null)
                ObjectGetPropertyRef = module.Import(_Injector.GetModuleZ("mscorlib.dll").GetTypeByName("Type").GetMethodByName("GetProperty", 1));

            if (PropertyInfoGetValueRef == null)
                PropertyInfoGetValueRef = module.Import(_Injector.GetModuleZ("mscorlib.dll").GetTypeByName("PropertyInfo").GetMethodByName("GetValue", 2));

            if (PropertyInfoSetValueRef == null)
                PropertyInfoSetValueRef = module.Import(_Injector.GetModuleZ("mscorlib.dll").GetTypeByName("PropertyInfo").GetMethodByName("SetValue", 3));

            if(ParamArrayAttributeRef == null)
                ParamArrayAttributeRef = module.Import(_Injector.GetModuleZ("mscorlib.dll").GetTypeByName("ParamArrayAttribute").GetMethodByName(".ctor"));
        }

        private void CreateStaticSetProperty(TypeDefinition ownerType, bool withParams = false)
        {
            //TODO: Make sure this one work (SST doesn't have any static properties right now)

            MethodDefinition md = new MethodDefinition("_Mod_SetStaticProperty", MethodAttributes.Public | MethodAttributes.Static, NullType);
            md.Parameters.Add(new ParameterDefinition("name", ParameterAttributes.None, StringTypeRef));
            md.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.None, ObjectTypeRef));

            Instruction[] instructions = null;

            if(withParams)
            {
                ParameterDefinition pd = new ParameterDefinition("index", ParameterAttributes.None, ObjectArrayTypeRef);
                pd.CustomAttributes.Add(new CustomAttribute(ParamArrayAttributeRef));
                md.Parameters.Add(pd);
                instructions = new Instruction[] {
                    Instruction.Create(OpCodes.Ldtoken, ownerType),
                    Instruction.Create(OpCodes.Call, TypeGetTypeFromHandle),
                    Instruction.Create(OpCodes.Ldarg_0),
                    Instruction.Create(OpCodes.Callvirt, ObjectGetPropertyRef),
                    Instruction.Create(OpCodes.Ldnull),
                    Instruction.Create(OpCodes.Ldarg_1),
                    Instruction.Create(OpCodes.Ldarg_2),
                    Instruction.Create(OpCodes.Callvirt, PropertyInfoSetValueRef),
                    Instruction.Create(OpCodes.Ret)
                };
            }
            else
            {
                instructions = new Instruction[] {
                    Instruction.Create(OpCodes.Ldtoken, ownerType),
                    Instruction.Create(OpCodes.Call, TypeGetTypeFromHandle),
                    Instruction.Create(OpCodes.Ldarg_0),
                    Instruction.Create(OpCodes.Callvirt, ObjectGetPropertyRef),
                    Instruction.Create(OpCodes.Ldnull),
                    Instruction.Create(OpCodes.Ldarg_1),
                    Instruction.Create(OpCodes.Ldnull),
                    Instruction.Create(OpCodes.Callvirt, PropertyInfoSetValueRef),
                    Instruction.Create(OpCodes.Ret)
                };
            }

            foreach (Instruction inst in instructions)
                md.Body.Instructions.Add(inst);

            ownerType.Methods.Add(md);
        }

        private void CreateSetProperty(TypeDefinition ownerType, bool withParams = false)
        {
            MethodDefinition md = new MethodDefinition("_Mod_SetProperty", MethodAttributes.Public, NullType);
            md.Parameters.Add(new ParameterDefinition("name", ParameterAttributes.None, StringTypeRef));
            md.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.None, ObjectTypeRef));

            Instruction[] instructions = null;

            if (withParams)
            {
                ParameterDefinition pd = new ParameterDefinition("index", ParameterAttributes.None, ObjectArrayTypeRef);
                pd.CustomAttributes.Add(new CustomAttribute(ParamArrayAttributeRef));
                md.Parameters.Add(pd);
                instructions = new Instruction[] {
                    Instruction.Create(OpCodes.Ldarg_0),
                    Instruction.Create(OpCodes.Call, ObjectGetTypeRef),
                    Instruction.Create(OpCodes.Ldarg_1),
                    Instruction.Create(OpCodes.Call, ObjectGetPropertyRef),
                    Instruction.Create(OpCodes.Ldarg_0),
                    Instruction.Create(OpCodes.Ldarg_2),
                    Instruction.Create(OpCodes.Ldarg_3),
                    Instruction.Create(OpCodes.Callvirt, PropertyInfoSetValueRef),
                    Instruction.Create(OpCodes.Ret)
                };
            }
            else
            {
                instructions = new Instruction[] {
                    Instruction.Create(OpCodes.Ldarg_0),
                    Instruction.Create(OpCodes.Call, ObjectGetTypeRef),
                    Instruction.Create(OpCodes.Ldarg_1),
                    Instruction.Create(OpCodes.Call, ObjectGetPropertyRef),
                    Instruction.Create(OpCodes.Ldarg_0),
                    Instruction.Create(OpCodes.Ldarg_2),
                    Instruction.Create(OpCodes.Ldnull),
                    Instruction.Create(OpCodes.Callvirt, PropertyInfoSetValueRef),
                    Instruction.Create(OpCodes.Ret)
                };
            }

            foreach (Instruction inst in instructions)
                md.Body.Instructions.Add(inst);

            ownerType.Methods.Add(md);
        }

        private void CreateStaticGetProperty(TypeDefinition ownerType)
        {
            //TODO: Make sure this one work (SST doesn't have any static properties right now)

            MethodDefinition md = new MethodDefinition("_Mod_GetStaticProperty", MethodAttributes.Public | MethodAttributes.Static, ObjectTypeRef);
            md.Parameters.Add(new ParameterDefinition("name", ParameterAttributes.None, StringTypeRef));

            Instruction[] instructions = new Instruction[] {
                Instruction.Create(OpCodes.Ldtoken, ownerType),
                Instruction.Create(OpCodes.Call, TypeGetTypeFromHandle),
                Instruction.Create(OpCodes.Ldarg_0),
                Instruction.Create(OpCodes.Callvirt, ObjectGetPropertyRef),
                Instruction.Create(OpCodes.Ldnull),
                Instruction.Create(OpCodes.Callvirt, PropertyInfoGetValueRef),
                Instruction.Create(OpCodes.Ret)
            };

            foreach (Instruction inst in instructions)
                md.Body.Instructions.Add(inst);

            ownerType.Methods.Add(md);
        }

        private void CreateGetProperty(TypeDefinition ownerType)
        {
            MethodDefinition md = new MethodDefinition("_Mod_GetProperty", MethodAttributes.Public, ObjectTypeRef);
            md.Parameters.Add(new ParameterDefinition("name", ParameterAttributes.None, StringTypeRef));
            md.Parameters.Add(new ParameterDefinition("index", ParameterAttributes.HasDefault | ParameterAttributes.Optional, ObjectArrayTypeRef) { Constant = null });


            Instruction[] instructions = new Instruction[] {
                Instruction.Create(OpCodes.Ldarg_0),
                Instruction.Create(OpCodes.Call, ObjectGetTypeRef),
                Instruction.Create(OpCodes.Ldarg_1),
                Instruction.Create(OpCodes.Callvirt, ObjectGetPropertyRef),
                Instruction.Create(OpCodes.Ldarg_0),
                Instruction.Create(OpCodes.Ldarg_2),
                Instruction.Create(OpCodes.Callvirt, PropertyInfoGetValueRef),
                Instruction.Create(OpCodes.Ret)
            };

            foreach (Instruction inst in instructions)
                md.Body.Instructions.Add(inst);

            ownerType.Methods.Add(md);

        }

        private void CreateStaticSetField(TypeDefinition ownerType)
        {
            MethodDefinition md = new MethodDefinition("_Mod_SetStaticField", MethodAttributes.Public | MethodAttributes.Static, NullType);
            md.Parameters.Add(new ParameterDefinition("name", ParameterAttributes.None, StringTypeRef));
            md.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.None, ObjectTypeRef));

            Instruction[] instructions = new Instruction[] {
                Instruction.Create(OpCodes.Ldtoken, ownerType),
                Instruction.Create(OpCodes.Call, TypeGetTypeFromHandle),
                Instruction.Create(OpCodes.Ldarg_0),
                Instruction.Create(OpCodes.Callvirt, ObjectGetFieldRef),
                Instruction.Create(OpCodes.Ldnull),
                Instruction.Create(OpCodes.Ldarg_1),
                Instruction.Create(OpCodes.Callvirt, FieldInfoSetValueRef),
                Instruction.Create(OpCodes.Ret)
            };

            foreach (Instruction inst in instructions)
                md.Body.Instructions.Add(inst);

            ownerType.Methods.Add(md);
        }

        private void CreateSetField(TypeDefinition ownerType)
        {
            MethodDefinition md = new MethodDefinition("_Mod_SetField", MethodAttributes.Public, NullType);
            md.Parameters.Add(new ParameterDefinition("name", ParameterAttributes.None, StringTypeRef));
            md.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.None, ObjectTypeRef));

            Instruction[] instructions = new Instruction[] {
                Instruction.Create(OpCodes.Ldarg_0),
                Instruction.Create(OpCodes.Call, ObjectGetTypeRef),
                Instruction.Create(OpCodes.Ldarg_1),
                Instruction.Create(OpCodes.Call, ObjectGetFieldRef),
                Instruction.Create(OpCodes.Ldarg_0),
                Instruction.Create(OpCodes.Ldarg_2),
                Instruction.Create(OpCodes.Callvirt, FieldInfoSetValueRef),
                Instruction.Create(OpCodes.Ret)
            };

            foreach (Instruction inst in instructions)
                md.Body.Instructions.Add(inst);

            ownerType.Methods.Add(md);
        }

        private void CreateGetStaticField(TypeDefinition ownerType)
        {
            MethodDefinition md = new MethodDefinition("_Mod_GetStaticField", MethodAttributes.Public | MethodAttributes.Static, ObjectTypeRef);
            md.Parameters.Add(new ParameterDefinition("name", ParameterAttributes.None, StringTypeRef));

            Instruction[] instructions = new Instruction[] {
                Instruction.Create(OpCodes.Ldtoken, ownerType),
                Instruction.Create(OpCodes.Call, TypeGetTypeFromHandle),
                Instruction.Create(OpCodes.Ldarg_0),
                Instruction.Create(OpCodes.Callvirt, ObjectGetFieldRef),
                Instruction.Create(OpCodes.Ldnull),
                Instruction.Create(OpCodes.Callvirt, FieldInfoGetValueRef),
                Instruction.Create(OpCodes.Ret)
            };

            foreach (Instruction inst in instructions)
                md.Body.Instructions.Add(inst);

            ownerType.Methods.Add(md);
        }

        private void CreateGetField(TypeDefinition ownerType)
        {
            MethodDefinition md = new MethodDefinition("_Mod_GetField", MethodAttributes.Public, ObjectTypeRef);
            md.Parameters.Add(new ParameterDefinition("name", ParameterAttributes.None, StringTypeRef));

            Instruction[] instructions = new Instruction[] {
                Instruction.Create(OpCodes.Ldarg_0),
                Instruction.Create(OpCodes.Call, ObjectGetTypeRef),
                Instruction.Create(OpCodes.Ldarg_1),
                Instruction.Create(OpCodes.Call, ObjectGetFieldRef),
                Instruction.Create(OpCodes.Ldarg_0),
                Instruction.Create(OpCodes.Callvirt, FieldInfoGetValueRef),
                Instruction.Create(OpCodes.Ret)
            };

            foreach (Instruction inst in instructions)
                md.Body.Instructions.Add(inst);

            ownerType.Methods.Add(md);
        }

        private void CreateStaticCallMethod(TypeDefinition ownerType)
        {
            MethodDefinition md = new MethodDefinition("_Mod_CallStaticMethod", MethodAttributes.Public | MethodAttributes.Static, ObjectTypeRef);
            md.Parameters.Add(new ParameterDefinition("name", ParameterAttributes.None, StringTypeRef));
            md.Parameters.Add(new ParameterDefinition("parameters", ParameterAttributes.HasDefault | ParameterAttributes.Optional, ObjectArrayTypeRef) { Constant = null });

            Instruction[] instructions = new Instruction[] {
                Instruction.Create(OpCodes.Ldtoken, ownerType),
                Instruction.Create(OpCodes.Call, TypeGetTypeFromHandle),
                Instruction.Create(OpCodes.Ldarg_0),
                Instruction.Create(OpCodes.Callvirt, TypeGetMethodRef),
                Instruction.Create(OpCodes.Ldnull),
                Instruction.Create(OpCodes.Ldarg_1),
                Instruction.Create(OpCodes.Callvirt, MethodBaseInvokeRef),
                Instruction.Create(OpCodes.Ret)
            };

            foreach (Instruction inst in instructions)
                md.Body.Instructions.Add(inst);

            ownerType.Methods.Add(md);
        }

        private void CreateCallMethod(TypeDefinition ownerType)
        {
            MethodDefinition md = new MethodDefinition("_Mod_CallMethod", MethodAttributes.Public, ObjectTypeRef);
            md.Parameters.Add(new ParameterDefinition("name", ParameterAttributes.None, StringTypeRef));
            md.Parameters.Add(new ParameterDefinition("parameters", ParameterAttributes.None, ObjectArrayTypeRef));
            md.Parameters.Last().CustomAttributes.Add(new CustomAttribute(ParamArrayAttributeRef));

            Instruction[] instructions = new Instruction[] {
                Instruction.Create(OpCodes.Ldarg_0),
                Instruction.Create(OpCodes.Call, ObjectGetTypeRef),
                Instruction.Create(OpCodes.Ldarg_1),
                Instruction.Create(OpCodes.Callvirt, TypeGetMethodRef),
                Instruction.Create(OpCodes.Ldarg_0),
                Instruction.Create(OpCodes.Ldarg_2),
                Instruction.Create(OpCodes.Callvirt, MethodBaseInvokeRef),
                Instruction.Create(OpCodes.Ret)
            };

            foreach (Instruction inst in instructions)
                md.Body.Instructions.Add(inst);

            ownerType.Methods.Add(md);
        }

        public void CreateModMethods(TypeDefinition ownerType)
        {
            if (ownerType.IsPublic && ownerType.IsAbstract && ownerType.IsSealed) //Static class
            {
                if (ownerType.HasFields)
                {
                    CreateGetStaticField(ownerType);
                    CreateStaticSetField(ownerType);
                }

                if(ownerType.HasProperties)
                {
                    CreateStaticGetProperty(ownerType);
                    CreateStaticSetProperty(ownerType);
                    CreateStaticSetProperty(ownerType, true);
                }

                if (ownerType.HasMethods)
                    CreateStaticCallMethod(ownerType);
            }
            else if (ownerType.IsPublic && !ownerType.IsAbstract && !ownerType.IsInterface) //Instance class
            {
                if (ownerType.HasFields)
                {
                    CreateGetField(ownerType);
                    CreateSetField(ownerType);
                }

                if(ownerType.HasProperties)
                {
                    CreateGetProperty(ownerType);
                    CreateSetProperty(ownerType);
                    CreateSetProperty(ownerType, true);
                }

                if (ownerType.HasMethods)
                    CreateCallMethod(ownerType);
            }
        }

        public void CreateSetterGetterFor(FieldDefinition field, TypeDefinition ownerType, bool isStaticField = false)
        {
            MethodDefinition set_method = new MethodDefinition("_ModSet_" + field.Name, MethodAttributes.Public, NullType);
            set_method.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.None, field.FieldType));

            set_method.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
            set_method.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_1));
            if(isStaticField)
                set_method.Body.Instructions.Add(Instruction.Create(OpCodes.Stsfld, Patches.BasePatch.GameModule.Import(field)));
            else
                set_method.Body.Instructions.Add(Instruction.Create(OpCodes.Stfld, Patches.BasePatch.GameModule.Import(field)));
            set_method.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));

            ownerType.Methods.Add(set_method);

            MethodDefinition get_method = new MethodDefinition("_ModGet_" + field.Name, MethodAttributes.Public, field.FieldType);

            get_method.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
            if(isStaticField)
                get_method.Body.Instructions.Add(Instruction.Create(OpCodes.Ldsfld, Patches.BasePatch.GameModule.Import(field)));
            else
                get_method.Body.Instructions.Add(Instruction.Create(OpCodes.Ldfld, Patches.BasePatch.GameModule.Import(field)));

            get_method.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));

            ownerType.Methods.Add(get_method);
        }


    }
}
