using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace StarshipTheory.ModInjector
{
    class PatchInjector
    {
        private Dictionary<String, Mono.Cecil.AssemblyDefinition> _AssemblyCache = new Dictionary<string, Mono.Cecil.AssemblyDefinition>();

        public event EventHandler<StringEventArgs> Error;

        private String _ApplicationPath;

        public String ApplicationPath
        {
            get { return _ApplicationPath; }
            set
            {
                if (!Directory.Exists(Path.Combine(value, "StarshipTheory_Data")))
                    throw new InvalidDataException("No data directory in application path");
                _ApplicationPath = value;
            }
        }

        private List<Patches.BasePatch> _Patches;

        public PatchInjector()
        {

        }

        public Mono.Cecil.ModuleDefinition GetModuleZ(String DllFilename)
        {
            if (_AssemblyCache.ContainsKey(DllFilename))
                return _AssemblyCache[DllFilename].MainModule;

            FileInfo DllFile = null;
            if (!Path.IsPathRooted(DllFilename))
            {
                if (File.Exists(Path.Combine(_ApplicationPath, DllFilename)))
                    DllFile = new FileInfo(Path.Combine(_ApplicationPath, DllFilename));
                else if (File.Exists(PathZ.Combine(_ApplicationPath, "StarshipTheory_Data", DllFilename)))
                    DllFile = new FileInfo(PathZ.Combine(_ApplicationPath, "StarshipTheory_Data", DllFilename));
                else if (File.Exists(PathZ.Combine(_ApplicationPath, "StarshipTheory_Data", "Managed", DllFilename)))
                    DllFile = new FileInfo(PathZ.Combine(_ApplicationPath, "StarshipTheory_Data", "Managed", DllFilename));
                else if (File.Exists(PathZ.Combine(AppDomain.CurrentDomain.BaseDirectory, DllFilename)))
                    DllFile = new FileInfo(PathZ.Combine(AppDomain.CurrentDomain.BaseDirectory, DllFilename));
            }
            else if (File.Exists(DllFilename))
                DllFile = new FileInfo(DllFilename);

            if (DllFile == null)
                return null;

            Mono.Cecil.AssemblyDefinition Def = Mono.Cecil.AssemblyDefinition.ReadAssembly(DllFile.FullName);
            if(Def != null)
            {
                _AssemblyCache[DllFilename] = Def;
                return Def.MainModule;
            }

            return null;
        }

        public void RestoreFiles()
        {
            DirectoryInfo BackupDir = new DirectoryInfo(PathZ.Combine(_ApplicationPath, "StarshipTheory_Data", "Managed", "@BACKUP"));
            DirectoryInfo ManagedDir = new DirectoryInfo(PathZ.Combine(_ApplicationPath, "StarshipTheory_Data", "Managed"));
            if (BackupDir.Exists)
            {
                foreach (FileInfo File in BackupDir.GetFiles("*.*", SearchOption.TopDirectoryOnly))
                    File.CopyTo(Path.Combine(ManagedDir.FullName, File.Name), true);

                Directory.Delete(BackupDir.FullName, true);
            }
        }

        public void BackupFiles()
        {
            DirectoryInfo BackupDir = new DirectoryInfo(PathZ.Combine(_ApplicationPath, "StarshipTheory_Data", "Managed", "@BACKUP"));
            DirectoryInfo ManagedDir = new DirectoryInfo(PathZ.Combine(_ApplicationPath, "StarshipTheory_Data", "Managed"));
            if (!BackupDir.Exists)
            {
                BackupDir.Create();

                foreach (FileInfo File in ManagedDir.GetFiles("*.*", SearchOption.TopDirectoryOnly))
                    File.CopyTo(Path.Combine(BackupDir.FullName, File.Name), true);
            }
        }


        public int LoadPatches(System.Reflection.Assembly Assembly)
        {
            Patches.BasePatch.Injector = this;
            Patches.BasePatch.GameModule = Mono.Cecil.AssemblyDefinition.ReadAssembly(PathZ.Combine(_ApplicationPath, "StarshipTheory_Data", "Managed", "Assembly-CSharp.dll")).MainModule;
            Patches.BasePatch.ModLibModule = Mono.Cecil.AssemblyDefinition.ReadAssembly(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "StarshipTheory.ModLib.dll")).MainModule;

            _Patches = new List<Patches.BasePatch>();
            new InjectionHelper(this);

            Type BasePatchType = typeof(Patches.BasePatch);
            int LoadedPatches = 0;
            foreach (Type PatchType in Assembly.GetTypes().Where(t => t.IsSubclassOf(BasePatchType) && !t.IsInterface && !t.IsAbstract))
            {
                try
                {
                    Patches.BasePatch P = (Patches.BasePatch)Activator.CreateInstance(PatchType);
                    _Patches.Add(P);
                    LoadedPatches++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error Initializing Patch " + PatchType.Name + ": " + ex.Message);
                    Error?.Invoke(this, new StringEventArgs("Error Initializing Patch '" + PatchType.Name + "': " + ex.Message));
                }
            }

            return LoadedPatches;
        }

        public void LoadPatch(Patches.BasePatch patch)
        {
            _Patches.Add(patch);
        }

        public void InjectPatches()
        {
            Patches.ModLoaderPatch loaderPatch = (Patches.ModLoaderPatch)_Patches.Where(p => p.Name == "Mod Loader Patch").FirstOrDefault();
            if(loaderPatch != null)
            {
                _Patches.Remove(loaderPatch);
                _Patches.Insert(0, loaderPatch);
            }

            foreach(Patches.BasePatch P in _Patches)
            {
                try
                {
                    if (P.ShouldInject())
                        P.Inject();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error Injecting Patch " + P.Name + ": " + ex.Message);
                    Error?.Invoke(this, new StringEventArgs("Error Injecting Patch '" + P.Name + "': " + ex.Message));
                }
            }

            Patches.BasePatch.GameModule.Assembly.Write(PathZ.Combine(_ApplicationPath, "StarshipTheory_Data", "Managed", "Assembly-CSharp.dll"));

            new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "StarshipTheory.ModLib.dll")).CopyTo(PathZ.Combine(_ApplicationPath, "StarshipTheory_Data", "Managed", "StarshipTheory.ModLib.dll"), true);
        }

        public class StringEventArgs : EventArgs
        {
            public String Text { get; private set; }

            public StringEventArgs(String Text)
            {
                this.Text = Text;
            }
        }
    }
}
