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

        public InjectionHelper(PatchInjector Injector)
        {
            Instance = this;
            _Injector = Injector;

            NullType = Patches.BasePatch.GameModule.Import(typeof(void));
        }


        public void CreateSetterGetterFor(FieldDefinition field, TypeDefinition ownerType, bool isStaticField = false)
        {
            MethodDefinition set_method = new MethodDefinition("Set_" + field.Name, MethodAttributes.Public, NullType);
            set_method.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.None, field.FieldType));

            set_method.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
            set_method.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_1));
            if(isStaticField)
                set_method.Body.Instructions.Add(Instruction.Create(OpCodes.Stsfld, Patches.BasePatch.GameModule.Import(field)));
            else
                set_method.Body.Instructions.Add(Instruction.Create(OpCodes.Stfld, Patches.BasePatch.GameModule.Import(field)));
            set_method.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));

            ownerType.Methods.Add(set_method);

            MethodDefinition get_method = new MethodDefinition("Get_" + field.Name, MethodAttributes.Public, field.FieldType);

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
