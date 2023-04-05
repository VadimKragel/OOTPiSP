using Creators;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;

namespace OOP
{
    public partial class FormObj : Form
    {
        private Control ControlType;
        private Dictionary<Type, (Creator creator, bool visible)> _typeCreators;
        public object Result { get; private set; }

        public FormObj()
        {
            InitializeComponent();
            Result = null;
        }

        private static string GetPropertyNameByDisplayAttr(PropertyInfo prop)
        {
            return prop.GetCustomAttribute<DisplayAttribute>()?.Name ?? prop.Name;
        }

        private static string GetEnumValueNameByDisplayAttr(object value)
        {
            return value.GetType().GetMember(value.ToString()).First()?.GetCustomAttribute<DisplayAttribute>()?.Name ?? value.ToString();
        }

        private static string GetTypeNameByDisplayAttr(Type type)
        {
            return type?.GetCustomAttribute<DisplayAttribute>()?.Name ?? type.ToString();
        }

        private void CbClass_SelectedValueChanged(object sender, EventArgs e)
        {
            Type type = (sender as ComboBox)?.SelectedItem as Type;
            SetControlForObj(type);
        }
        private string ToStringOrEmptyIfNull(object obj)
        {
            return obj?.ToString() ?? "";
        }
        private void SetControlForObj(Type type)
        {
            if (type != null)
            {
                PnlContent.Controls.Clear();
                Control control = GenerateControl(type);
                control.Width = PnlContent.Width;
                control.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                PnlContent.Controls.Add(control);
            }
        }

        private void SetControlForObj(object obj)
        {
            Type type = obj.GetType();
            if (type != null)
            {
                PnlContent.Controls.Clear();
                Control control = GenerateControl(type, obj);
                control.Width = PnlContent.Width;
                control.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                PnlContent.Controls.Add(control);
            }
        }

        private Control GenerateControl(Type type, object startValue = null)
        {
            if (type.IsEnum)
            {
                ComboBox comboBox = new ComboBox()
                {
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    Font = new Font("Tahoma", 12F, FontStyle.Regular, GraphicsUnit.Point),
                    FormattingEnabled = true,
                };
                comboBox.Items.AddRange(Enum.GetValues(type).Cast<object>().ToArray());
                comboBox.Format += (sender, e) => e.Value = GetEnumValueNameByDisplayAttr(e.Value);
                comboBox.SelectedItem = startValue;
                if (comboBox.SelectedItem == null)
                    comboBox.SelectedIndex = 0;
                return comboBox;
            }
            if (type == typeof(string))
            {
                return new TextBox()
                {
                    Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point),
                    Text = ToStringOrEmptyIfNull(startValue),
                };
            }
            if (type.IsClass)
            {
                int row = 0;
                TableLayoutPanel tlp = new TableLayoutPanel()
                {
                    ColumnCount = 2,
                    BorderStyle = BorderStyle.FixedSingle,
                    AutoSize = true,
                };
                tlp.ColumnStyles.Add(new ColumnStyle());
                tlp.ColumnStyles.Add(new ColumnStyle());
                tlp.RowStyles.Add(new RowStyle());
                Dictionary<PropertyInfo, Control> propertyControls = new Dictionary<PropertyInfo, Control>();
                foreach (PropertyInfo prop in type.GetProperties())
                {
                    tlp.Controls.Add(new Label()
                    {
                        AutoSize = true,
                        Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point),
                        Text = GetPropertyNameByDisplayAttr(prop),
                    }, 0, row);
                    object value = startValue != null ? prop.GetValue(startValue) : startValue;
                    Control control = GenerateControl(prop.PropertyType, value);
                    control.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                    tlp.Controls.Add(control, 1, row);
                    row++;
                    propertyControls.Add(prop, control);
                }
                tlp.Tag = propertyControls;
                return tlp;
            }
            return new TextBox()
            {
                Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point),
                Text = ToStringOrEmptyIfNull(startValue),
            };
        }

        internal DialogResult ShowDialogCreateObj(Dictionary<Type, (Creator, bool)> typeCreators)
        {
            _typeCreators = typeCreators;
            Load += (sender, e) =>
            {
                Text = "Создать";
                ComboBox CbType = new ComboBox()
                {
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point),
                    FormattingEnabled = true,
                    Size = new Size(240, 33),
                    TabIndex = 0,
                    DataSource = _typeCreators.Where(pair => pair.Value.visible).Select(pair => pair.Key).ToArray(),
                };
                CbType.Format += (sender, e) => e.Value = GetTypeNameByDisplayAttr(e.Value as Type);
                CbType.SelectedValueChanged += CbClass_SelectedValueChanged;
                ControlType = CbType;
                FlpTop.Controls.Add(CbType);
                Button BtnCancel = new Button()
                {
                    Text = "Отмена",
                    DialogResult = DialogResult.Cancel,
                    AutoSize = true,
                };
                CancelButton = BtnCancel;
                Button BtnOk = new Button()
                {
                    Text = "ОК",
                    AutoSize = true,
                };
                BtnOk.Click += BtnOkCreate_Click;
                FlpBottom.Controls.Add(BtnCancel);
                FlpBottom.Controls.Add(BtnOk);
            };
            return ShowDialog();
        }

        internal DialogResult ShowDialogEditObj(Dictionary<Type, (Creator, bool)> typeCreators, object obj)
        {
            _typeCreators = typeCreators;
            Result = obj;
            Type type = obj.GetType();
            object localObj = obj;
            Load += (sender, e) =>
            {
                Text = "Изменить";
                ControlType = new Label()
                {
                    Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point),
                    AutoSize = true,
                    MaximumSize = new Size(300, 0),
                    TabIndex = 0,
                    Text = GetTypeNameByDisplayAttr(type),
                    Tag = type,
                };
                FlpTop.Controls.Add(ControlType);
                Button BtnCancel = new Button()
                {
                    Text = "Отмена",
                    DialogResult = DialogResult.Cancel,
                    AutoSize = true,
                };
                CancelButton = BtnCancel;
                Button BtnOk = new Button()
                {
                    Text = "ОК",
                    AutoSize = true,
                };
                BtnOk.Click += BtnOkEdit_Click;
                FlpBottom.Controls.Add(BtnCancel);
                FlpBottom.Controls.Add(BtnOk);
                SetControlForObj(localObj);
            };
            return ShowDialog();
        }

        private bool TryFillObjectFromControl(Type type, Control control, out object result)
        {
            bool TryFillClass(Type type, out object result)
            {
                object obj = null;
                StringBuilder builder = new StringBuilder();
                bool isValid = true;
                Dictionary<PropertyInfo, Control> propertyControls = control.Tag as Dictionary<PropertyInfo, Control>;
                if (propertyControls != null)
                {
                    if (_typeCreators.TryGetValue(type, out var typeData))
                    {
                        obj = typeData.creator.Create();
                        foreach (PropertyInfo prop in type.GetProperties())
                        {
                            if (propertyControls.TryGetValue(prop, out var propControl))
                            {
                                object value;
                                if (TryFillObjectFromControl(prop.PropertyType, propControl, out value))
                                {
                                    prop.SetValue(obj, value);
                                }
                                else
                                {
                                    isValid = false;
                                    builder.Append(GetPropertyNameByDisplayAttr(prop)).Append(": ").Append(value).Append("\n");
                                }
                            }
                        }
                    }
                }
                result = isValid ? obj : builder.ToString();
                return isValid;
            }
            result = null;
            if (type == null || control == null)
                return false;
            if (type.IsEnum)
            {
                result = (control as ComboBox)?.SelectedItem;
                return true;
            }
            if (type == typeof(string))
            {
                result = control.Text;
                return true;
            }
            if (type.IsClass)
            {
                return TryFillClass(type, out result);
            }
            try
            {
                result = TypeDescriptor.GetConverter(type).ConvertFromString(control.Text);
                return true;
            }
            catch
            {
                result = $"Некорректное значение в элементе управления";
                return false;
            }

        }

        private bool TryValidateObjectRecursively(object obj, ICollection<ValidationResult> validationResults, bool validateAllProperties)
        {
            bool Recursive(object obj)
            {
                bool isValid = true;
                foreach (PropertyInfo prop in obj.GetType().GetProperties())
                {
                    if (prop.PropertyType.IsClass)
                    {
                        if (!Recursive(prop.GetValue(obj)))
                            isValid = false;
                        if (!isValid && !validateAllProperties)
                            return false;
                    }
                }
                if (!isValid && !validateAllProperties)
                    return false;
                ICollection<ValidationResult> results = new List<ValidationResult>();
                ValidationContext context = new ValidationContext(obj);
                if (!Validator.TryValidateObject(obj, context, results, validateAllProperties))
                {
                    foreach (var result in results)
                        validationResults.Add(result);
                    return false;
                }
                return isValid;
            }
            validationResults.Clear();
            return Recursive(obj);
        }

        private void BtnOkCreate_Click(object sender, EventArgs e)
        {
            Type type = (ControlType as ComboBox)?.SelectedItem as Type;
            HandleBtnClick(type);
        }

        private void BtnOkEdit_Click(object sender, EventArgs e)
        {
            Type type = ControlType.Tag as Type;
            HandleBtnClick(type);
        }

        private void HandleBtnClick(Type type)
        {
            if (type != null && _typeCreators.TryGetValue(type, out var typeData))
            {
                object resultFilling;
                if (TryFillObjectFromControl(type, PnlContent.Controls[0], out resultFilling))
                {
                    List<ValidationResult> results = new List<ValidationResult>();
                    ValidationContext context = new ValidationContext(resultFilling);
                    if (TryValidateObjectRecursively(resultFilling, results, true))
                    {
                        Result = resultFilling;
                        DialogResult = DialogResult.OK;
                        Close();
                    }
                    else
                    {
                        StringBuilder builder = new StringBuilder();
                        for (int i = 0; i < results.Count; i++)
                        {
                            builder.Append(i + 1).Append(") ").Append(results[i].ErrorMessage).Append("\n");
                        }
                        MessageBox.Show(builder.ToString(), "Ошибка");
                    }
                }
                else
                {
                    MessageBox.Show(resultFilling.ToString(), "Ошибка");
                }
            }
        }
    }
}
