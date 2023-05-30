using Microsoft.VisualBasic;
using Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OOTPiSP
{
    public class PluginLoader
    {
        public const string FolderPlugins = @".\Plugins";
        public List<IPlugin> Plugins { get; private set; }

        public IEnumerable<IPlugin> LoadPlugins()
        {
            Plugins = new List<IPlugin>();
            if (Directory.Exists(FolderPlugins))
            {
                string[] files = Directory.GetFiles(FolderPlugins);
                foreach (string file in files)
                {
                    if (file.EndsWith(".dll"))
                    {
                        Assembly.LoadFile(Path.GetFullPath(file));
                    }
                }
            }
            Type interfaceType = typeof(IPlugin);
            Type[] types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(p => interfaceType.IsAssignableFrom(p) && p.IsClass)
                .ToArray();
            foreach (Type type in types)
            {
                Plugins.Add((IPlugin)Activator.CreateInstance(type));
            }
            return Plugins;
        }
    }
}
