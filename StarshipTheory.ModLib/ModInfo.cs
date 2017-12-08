using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarshipTheory.ModLib
{
    public class ModInfo
    {
        public String Name { get; internal set; }
        public String DisplayName { get; internal set; }
        public Version Version { get; internal set; }
        public String Description { get; internal set; }
        public String Author { get; internal set; }

        public String EntryDLL { get; internal set; }

        /// <summary>
        /// <para>A list of required mods for this mod to be enabled.</para>
        /// </summary>
        public ModDependency[] Dependencies { get; internal set; }

        public ModInfo()
        {

        }
    }

    public class ModDependency
    {
        public String Name { get; internal set; }
        public Version MinimumVersion { get; internal set; }
        public Version MaximumVersion { get; internal set; }

        public ModDependency()
        {

        }
    }
}
