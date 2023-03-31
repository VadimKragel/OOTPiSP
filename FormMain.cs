using Creators;
using Hierarchy;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

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
        private const int TV_PROPS_LEFT_CHARS_COUNT = 30;
        private const int TV_PROPS_RIGHT_CHARS_COUNT = 20;
        private static Objects objects = new Objects();

        public FormMain()
        {
            InitializeComponent();
            SetDefaultTreeViewNodes();
            LbObjects.DataSource = objects.BindingSource;
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
            Type type = obj.GetType();
            List<TreeNode> list = new List<TreeNode>();
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
                    LbObjects.SelectedIndex = objects.BindingSource.Add(formObj.Result);
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
                    if (formObj.ShowDialogEditObj(typeCreators, objects.BindingSource[index]) == DialogResult.OK)
                    {
                        objects.BindingSource[index] = formObj.Result;
                    }
                }
            }

        }

        private void CmSpaceToolStripMenuItemDelete_Click(object sender, EventArgs e)
        {
            int index = LbObjects.SelectedIndex;
            if (index >= 0)
            {
                objects.BindingSource.RemoveAt(index);
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
    }
    internal class Objects
    {
        private List<object> objects;
        public BindingSource BindingSource { get; private set; }
        public Objects()
        {
            objects = new List<object>();
            BindingSource = new BindingSource();
            BindingSource.DataSource = objects;
        }
    }
}