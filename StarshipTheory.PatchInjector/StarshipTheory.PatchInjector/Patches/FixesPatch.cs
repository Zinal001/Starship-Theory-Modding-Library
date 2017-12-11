using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarshipTheory.PatchInjector.Patches
{
    class FixesPatch : BasePatch
    {
        public override string Name => "Bug Fixes Patch";

        public override void Inject()
        {
            AI_anyPartOnFire_Fix();
        }

        private void AI_anyPartOnFire_Fix()
        {
            TypeDefinition AIDef = GameModule.GetTypeByName("AI");
            MethodDefinition anyPartOnFireDef = AIDef.GetMethodByName("anyPartOnFire");
            
            ILProcessor il = anyPartOnFireDef.Body.GetILProcessor();

            Instruction[] ending = new Instruction[] {
                Instruction.Create(OpCodes.Ldc_I4_0),
                Instruction.Create(OpCodes.Ret)
            };

            il.Append(ending[0]);
            il.Append(ending[1]);

            Instruction[] beginning = new Instruction[] {
                Instruction.Create(OpCodes.Ldarg_1),
                Instruction.Create(OpCodes.Ldarg_2),
                Instruction.Create(OpCodes.Add),
                Instruction.Create(OpCodes.Ldc_I4, -888),
                Instruction.Create(OpCodes.Ceq),
                Instruction.Create(OpCodes.Brtrue, ending[0])
            };

            Instruction last = il.Body.Instructions[0];
            for(int i = beginning.Length - 1; i >= 0; i--)
            {
                il.InsertBefore(last, beginning[i]);
                last = beginning[i];
            }
        }


    }
}
