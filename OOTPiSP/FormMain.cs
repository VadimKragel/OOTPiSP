using Creators;
using Hierarchy;
using OOTPiSP;
using Plugin;
using Serializers;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;

namespace OOP
{
    public partial class FormMain : Form
    {
        private static Dictionary<Type, (Creator, bool)> typeCreators = new Dictionary<Type, (Creator, bool)>() {
            { typeof(Truck), (new CreatorTruck(), true) },
            { typeof(Car), (new CreatorCar(), true) },
            { typeof(Bus), (new CreatorBus(), true) },
            { typeof(Metro), (new CreatorMetro(), true) },
            { typeof(Train), (new CreatorTrain(), true) },
            { typeof(Driver), (new CreatorDriver(), false) },
        };
        private static ISerializer[] serializers = new ISerializer[] { new BinarySerializer(), new JsonMySerializer(), new TxtSerializer() };
        private static IPlugin[] plugins;

        private const int TV_PROPS_LEFT_CHARS_COUNT = 30;
        private const int TV_PROPS_RIGHT_CHARS_COUNT = 20;
        private List<object> objects = new List<object>();
        private BindingSource bindingObjects = new BindingSource();
        private PluginLoader pluginLoader = new PluginLoader();

        public FormMain()
        {
            InitializeComponent();
            SetDefaultTreeViewNodes();
            plugins = pluginLoader.LoadPlugins().ToArray();
            //extSerializers = new Dictionary<string, ISerializer>(serializers.SelectMany((ISerializer s) => s.Extensions.Select(ext => new KeyValuePair<string, ISerializer>(ext, s))));
            IEnumerable<string> serializersFilters = GetSerializerFilters(serializers);
            fdOpen.Filter = string.Join('|', serializersFilters.Concat(GetPluginOpenFilters(plugins)));
            fdSave.Filter = string.Join('|', serializersFilters.Concat(GetPluginSaveFilters(serializers, plugins)));
            bindingObjects.ListChanged += (sender, e) =>
            {
                if (sender is BindingSource source)
                {
                    saveMenuItem.Enabled = source.Count > 0;
                }
            };
            bindingObjects.DataSource = objects;
            LbObjects.DataSource = bindingObjects;
        }

        private static IEnumerable<string> GetSerializerFilters(ISerializer[] serializers)
        {
            return serializers.Select((ISerializer s) => $"{s.Name}|{string.Join(';', s.Extensions.Select(ModifyExt))}");
        }

        private static IEnumerable<string> GetPluginSaveFilters(ISerializer[] serializers, IPlugin[] plugins)
        {
            return plugins.Select((IPlugin plugin) => string.Join('|', serializers.Select((ISerializer s) => $"{s.Name} - {plugin.Name}|{string.Join(';', plugin.Extensions.Select(ModifyExt))}")));
        }

        private static IEnumerable<string> GetPluginOpenFilters(IPlugin[] plugins)
        {
            return plugins.Select((IPlugin p) => $"{p.Name}|{string.Join(';', p.Extensions.Select(ModifyExt))}");
        }

        private static string ModifyExt(string extText)
        {
            return "*" + extText;
        }

        private static string GetPropertyNameByDisplayAttr(PropertyInfo prop)
        {
            return prop.GetCustomAttribute<DisplayAttribute>()?.Name ?? prop.Name;
        }

        private static string GetEnumValueNameByDisplayAttr(object value)
        {
            return value.GetType().GetMember(value.ToString()).First()?.GetCustomAttribute<DisplayAttribute>()?.Name ?? value.ToString();
        }

        private void SetDefaultTreeViewNodes()
        {
            TvProps.Nodes.Add("Выберите объект для просмотра");
        }

        private List<TreeNode> GetTreeViewOfObject(object obj)
        {
            List<TreeNode> list = new List<TreeNode>();
            if (obj == null)
                return list;
            Type type = obj.GetType();
            if (type.IsClass)
            {
                foreach (PropertyInfo prop in type.GetProperties())
                {
                    TreeNode current = new TreeNode($"{GetPropertyNameByDisplayAttr(prop),-TV_PROPS_LEFT_CHARS_COUNT} ");
                    Type propType = prop.PropertyType;
                    if (propType.IsEnum)
                        current.Text += $"{GetEnumValueNameByDisplayAttr(prop.GetValue(obj)),TV_PROPS_RIGHT_CHARS_COUNT}";
                    else if (propType.IsValueType || propType == typeof(string))
                        current.Text += $"{prop.GetValue(obj),TV_PROPS_RIGHT_CHARS_COUNT}";
                    else if (propType.IsClass)
                        current.Nodes.AddRange(GetTreeViewOfObject(prop.GetValue(obj)).ToArray());
                    else
                        current.Text += $"{prop.GetValue(obj),TV_PROPS_RIGHT_CHARS_COUNT}";
                    list.Add(current);
                }
            }
            else
            {
                list.Add(new TreeNode($"{type.Name,-TV_PROPS_LEFT_CHARS_COUNT} {obj?.ToString() ?? "",TV_PROPS_RIGHT_CHARS_COUNT}"));
            }
            return list;
        }

        private void CmSpaceToolStripMenuItemCreate_Click(object sender, EventArgs e)
        {
            using (var formObj = new FormObj())
            {
                if (formObj.ShowDialogCreateObj(typeCreators) == DialogResult.OK)
                {
                    LbObjects.SelectedIndex = bindingObjects.Add(formObj.Result);
                }
            }
        }

        private void CmSpaceToolStripMenuItemEdit_Click(object sender, EventArgs e)
        {
            int index = LbObjects.SelectedIndex;
            if (index >= 0)
            {
                using (var formObj = new FormObj())
                {
                    if (formObj.ShowDialogEditObj(typeCreators, bindingObjects[index]) == DialogResult.OK)
                    {
                        bindingObjects[index] = formObj.Result;
                    }
                }
            }
        }

        private void CmSpaceToolStripMenuItemDelete_Click(object sender, EventArgs e)
        {
            int index = LbObjects.SelectedIndex;
            if (index >= 0)
            {
                bindingObjects.RemoveAt(index);
            }
        }

        private void LbObjects_SelectedIndexChanged(object sender, EventArgs e)
        {
            TvProps.Nodes.Clear();
            if (LbObjects.SelectedItem != null)
            {
                foreach (TreeNode node in GetTreeViewOfObject(LbObjects.SelectedItem))
                {
                    TvProps.Nodes.Add(node);
                }
                TvProps.ExpandAll();
            }
            else
            {
                SetDefaultTreeViewNodes();
            }

        }

        private void LbObjects_MouseDown(object sender, MouseEventArgs e)
        {
            LbObjects.SelectedIndex = LbObjects.IndexFromPoint(e.X, e.Y);
            LbObjects.ContextMenuStrip = LbObjects.SelectedIndex >= 0 ? CmItem : CmSpace;
        }

        private void SaveMenuItem_Click(object sender, EventArgs e)
        {
            if (fdSave.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Stream stream = new MemoryStream();
                    ISerializer serializer = serializers[(fdSave.FilterIndex - 1) % serializers.Length];
                    serializer.Serialize(objects, stream);
                    stream.Seek(0, SeekOrigin.Begin);
                    int pluginIndex = fdSave.FilterIndex - serializers.Length;
                    if (pluginIndex > 0)
                    {
                        pluginIndex = (pluginIndex - 1) % serializers.Length;
                        stream = plugins[pluginIndex / serializers.Length].Postprocess(new Dictionary<string, Stream>() { { $"file.{serializer.Extensions[0]}", stream } });
                    }
                    using FileStream fs = new FileStream(fdSave.FileName, FileMode.Create);
                    stream.CopyTo(fs);
                    fs.Close();
                }
                catch
                {
                    MessageBox.Show("Не удалось сохранить в указанный файл", "Ошибка сохранения");
                }
            }
        }

        private void OpenMenuItem_Click(object sender, EventArgs e)
        {
            if (fdOpen.ShowDialog() == DialogResult.OK)
            {
                int pluginIndex = fdOpen.FilterIndex - serializers.Length;
                List<object> bufObjects;
                try
                {
                    using FileStream fs = new FileStream(fdOpen.FileName, FileMode.Open);
                    if (pluginIndex > 0)
                    {
                        Dictionary<string, Stream> files = plugins[(pluginIndex - 1) % plugins.Length].Preprocess(fs);
                        bufObjects = new List<object>(GetObjectsFromFiles(files));
                    }
                    else
                    {
                        bufObjects = serializers[fdOpen.FilterIndex - 1].Deserialize<List<object>>(fs);
                    }
                    fs.Close();
                    bindingObjects.Clear();
                    foreach (object obj in bufObjects)
                        bindingObjects.Add(obj);
                }
                catch
                {
                    MessageBox.Show("Не удалось открыть указанный файл", "Ошибка открытия");
                }
            }
        }

        private IEnumerable<object> GetObjectsFromFiles(Dictionary<string, Stream> files)
        {
            List<object> list = new List<object>();
            foreach (var (name, stream) in files)
            {
                string ext = Path.GetExtension(name);
                if (TryFindSerializer(ext, out var serializer))
                {
                    try
                    {
                        list.AddRange(serializer.Deserialize<List<object>>(stream));
                    }
                    catch { }

                }
            }
            return list;
        }

        private bool TryFindSerializer(string ext, out ISerializer serializer)
        {
            serializer = null;
            foreach (ISerializer s in serializers)
            {
                foreach (string e in s.Extensions)
                {
                    if (e == ext)
                    {
                        serializer = s;
                        return true;
                    }
                }
            }
            return false;
        }
    }
}