using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace StarshipTheory.PatchInjector.Patches
{
    class DataPatch : BasePatch
    {
        public override string Name => "Save/Load Data Patch";

        public override void Inject()
        {
            TypeDefinition saveLoadDataType = ModLibModule.GetTypeByName("SaveLoadData");
            MethodDefinition saveLoadInitMethod = saveLoadDataType.GetMethodByName("Init");

            TypeDefinition ES2InitType = GameModule.GetTypeByName("ES2Init");
            MethodDefinition ES2InitMethod = ES2InitType.GetMethodByName("Init");

            Instruction insertAfter = ES2InitMethod.Body.Instructions[ES2InitMethod.Body.Instructions.Count - 4];

            ES2InitMethod.Body.GetILProcessor().InsertAfter(insertAfter, new Instruction[] {
                Instruction.Create(OpCodes.Callvirt, GameModule.Import(saveLoadInitMethod))
            });


        }
    }
}
