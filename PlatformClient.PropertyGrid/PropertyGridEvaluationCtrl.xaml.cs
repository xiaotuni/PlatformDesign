using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using PlatformClient.Extend.Core;
using PlatformClient.Common;
using PlatformClient.Common.Interfaces;
using PlatformClient.PropertyGrid.Events;
using PlatformClient.PropertyGrid.Lib;
using PlatformClient.Utility;

namespace PlatformClient.PropertyGrid
{
    /// <summary>
    /// 属性控件里面的赋值控件
    /// </summary>
    public partial class PropertyGridEvaluationCtrl : UserControl, INotifyPropertyChanged, IUserControl
    {
        /// <summary>
        /// 失去焦点
        /// </summary>
        public new event EventHandler<EvaluationCtrlLostFocusEventArgs> LostFocus;

        /// <summary>
        /// 内容改变
        /// </summary>
        public event EventHandler<EvaluationCtrlTextChangeEventArgs> ContentChange;
        /// <summary>
        /// 
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty EvaluationValueProperty = DependencyProperty.Register("EvaluationValueProperty", typeof(Object), typeof(PropertyGridEvaluationCtrl), new PropertyMetadata(null));
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty MethodNameProperty = DependencyProperty.Register("MethodName", typeof(String), typeof(PropertyGridEvaluationCtrl), new PropertyMetadata(null));
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty AssemblyQualifiedNameProperty = DependencyProperty.Register("AssemblyQualifiedName", typeof(String), typeof(PropertyGridEvaluationCtrl), new PropertyMetadata(null));
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty DataTypeProperty = DependencyProperty.Register("DataType", typeof(Type), typeof(PropertyGridEvaluationCtrl), new PropertyMetadata(null));
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty DefaultValueProperty = DependencyProperty.Register("DefaultValue", typeof(object), typeof(PropertyGridEvaluationCtrl), new PropertyMetadata(null));

        /// <summary>
        /// 设计时框架接口
        /// </summary>
        public IPageDesignFramework IDesignFramework { get; set; }
        /// <summary>
        /// 当前选中控件名称
        /// </summary>
        public string CurrentSelectedControlName { get; set; }

        /// <summary>
        /// 默认值
        /// </summary>
        public object DefaultValue { get { return (object)GetValue(DefaultValueProperty); } set { SetValue(DefaultValueProperty, value); } }

        /// <summary>
        /// 
        /// </summary>
        public Type DataType { get { return (Type)GetValue(DataTypeProperty); } set { SetValue(DataTypeProperty, value); } }

        /// <summary>
        /// 
        /// </summary>
        public string AssemblyQualifiedName { get { return (string)GetValue(AssemblyQualifiedNameProperty); } set { SetValue(AssemblyQualifiedNameProperty, value); } }

        /// <summary>
        /// 方法名称
        /// </summary>
        public string MethodName { get { return (string)GetValue(MethodNameProperty); } set { SetValue(MethodNameProperty, value); } }

        /// <summary>
        /// 父级控件
        /// </summary>
        public UIElement ParentControl { get; set; }
        /// <summary>
        /// 显示的名称
        /// </summary>
        public string ShowName { get { return this.txtTitle.Text.Trim(); } set { txtTitle.Text = value; } }

        /// <summary>
        /// 赋值的值
        /// </summary>
        public Object EvaluationValue { get { return (Object)GetValue(EvaluationValueProperty); } set { SetValue(EvaluationValueProperty, value); } }

        void _TextChangeMethod(object sender, EvaluationCtrlTextChangeEventArgs e)
        {
            if (null == ContentChange)
            {
                return;
            }
            ContentChange(this, e);
        }

        void _LostFocusMethod(object sender, EvaluationCtrlLostFocusEventArgs e)
        {
            if (null == LostFocus)
            {
                return;
            }
            LostFocus(sender, e);
        }

        /// <summary>
        /// 
        /// </summary>
        public PropertyGridEvaluationCtrl()
        {
            InitializeComponent();
            this.Loaded += pgec_Loaded;
        }

        void pgec_Loaded(object sender, RoutedEventArgs e)
        {
            if (null == DataType)
            {
                return;
            }
            this.gControlType.Children.Clear();
            object obj = DataType;
            if (!(obj is Type))
            {
                return;
            }
            Type tt = obj as Type;
            if (tt.IsEnum)
            {
                this.gControlType.Children.Add(_ComboBoxCtrl(tt));
                return;
            }
            if (!String.IsNullOrEmpty(AssemblyQualifiedName))
            {
                //-->这里开始创建一个弹出来的对话框，
                //-->然后在对话框里增加把要反射创建的对话显示出来
                //-->当选择好后，他一定是复合控件
                this.gControlType.Children.Add(_ShowDialog(tt));
                return;
            }
            switch (tt.Name)
            {
                case "DateTime":
                case "Boolean":
                case "Char":
                case "String":
                case "Decimal":
                case "Single":
                case "Double":
                case "Byte":
                case "Int16":
                case "Int32":
                case "Int64":
                case "SByte":
                case "UInt16":
                case "UInt32":
                case "UInt64":
                case "Double?":
                    this.gControlType.Children.Add(_TextBoxCtrl());
                    break;
                default:

                    if (tt.GetConstructors().Count() > 0)
                    {
                        this.gControlType.Children.Add(_PropertyGrid(tt));
                        return;
                    }
                    break;
            }
            return;
        }

        FrameworkElement _ShowDialog(Type tt)
        {
            object obj = Wrapper.CreateInstance(AssemblyQualifiedName);
            if (null == obj)
            {
                return new TextBox();
            }
            var dialog = new PropertyGridDialogCtrl();
            dialog.Close += dialog_Close;
            dialog.IDesignFramework = IDesignFramework;
            dialog.UIElementProperty = obj as FrameworkElement;
            dialog.AssemblyQualifiedName = AssemblyQualifiedName;
            dialog.CurrentSelectedControlName = this.CurrentSelectedControlName;
            if (null != this.EvaluationContent && !this.EvaluationContent.Value.IsNullOrEmpty())
            {
                dialog.EvaluationContent = this.EvaluationContent;
            }
            else
            {
                this.EvaluationContent = dialog.EvaluationContent;
            }
            return dialog;
        }

        FrameworkElement _PropertyGrid(Type tt)
        {
            var obj = Activator.CreateInstance(tt);
            if (obj is DateTime)
            {
                return _DatePickerCtrl();
            }
            else if (obj is FrameworkElement)
            {
                var ui = obj as FrameworkElement;
                var dialog = new PropertyGridDialogCtrl();
                dialog.Close += dialog_Close;
                dialog.UIElementProperty = ui;
                dialog.AssemblyQualifiedName = AssemblyQualifiedName;
                return dialog;
            }
            return _TextBoxCtrl();
        }

        void dialog_Close(object sender, EventArgs e)
        {
            IUserControl iuc = sender as IUserControl;
            var newContent = iuc.EvaluationContent;
            if (null == newContent)
            {
                return;
            }
            //-->前后两个值一样，不用调用失去焦点事件了。
            if (null != this.EvaluationValue && null != newContent && (EvaluationValue is XElement))
            {
                var ev = EvaluationContent as XElement;
                if (ev.Value.Equals(newContent.Value))
                {
                    return;
                }
            }
            this.EvaluationValue = newContent.Value;
            _LostFocusMethod(this, new EvaluationCtrlLostFocusEventArgs(sender, this.MethodName, this.EvaluationValue.ToString(), this.CurrentSelectedControlName));
        }

        FrameworkElement _TextBoxCtrl()
        {
            //-->如果是bool值的话
            string name = DataType.Name;
            //创建一个ComboBox
            if (name.Equals("Boolean"))
            {
                ComboBox cb = new ComboBox();
                cb.SelectionChanged += cb_SelectionChanged;
                ComboBoxItem item = new ComboBoxItem();
                var _d = new EnumsTypeDescription();
                _d.Description = "真";
                _d.Name = "true";
                _d.Index = 0;
                item.Content = _d;
                ToolTipService.SetToolTip(item, _d.Description);
                cb.Items.Add(item);

                item = new ComboBoxItem();
                _d = new EnumsTypeDescription();
                _d.Description = "假";
                _d.Name = "false";
                _d.Index = 1;
                item.Content = _d;
                ToolTipService.SetToolTip(item, _d.Description);
                cb.Items.Add(item);

                //-->判断是否是默认值
                cb.SelectedIndex = 0;
                if (!DefaultValue.ToString().Equals("True"))
                {
                    cb.SelectedIndex = 1;
                }
                return cb;
            }
            else if (name.Equals("DateTime"))
            {
                return _DatePickerCtrl();
            }
            else
            {
                TextBox tb = new TextBox();
                tb.TextChanged += tb_TextChanged;
                tb.LostFocus += tb_LostFocus;
                tb.Text = string.Format("{0}", DefaultValue);
                this.EvaluationValue = DefaultValue;
                return tb;
            }
        }

        FrameworkElement _DatePickerCtrl()
        {
            DatePicker dp = new DatePicker();
            dp.SelectedDateChanged += dp_SelectedDateChanged;
            dp.LostFocus += dp_LostFocus;
            return dp;
        }

        FrameworkElement _ComboBoxCtrl(Type tt)
        {
            FieldInfo[] fiItem = tt.GetFields();
            int _index = 0;
            var result = from p in fiItem
                         where p.GetCustomAttributes(true).Length > 0
                         select new EnumsTypeDescription
                         {
                             Index = _index++,
                             Name = p.Name,
                             Description = (p.GetCustomAttributes(true)[0] as DescriptionAttribute).Description,
                         };
            ComboBox cb = new ComboBox();
            cb.SelectionChanged += cb_SelectionChanged;
            cb.LostFocus += cb_LostFocus;
            foreach (var v in result)
            {
                ComboBoxItem _cbItem = new ComboBoxItem();
                ToolTipService.SetToolTip(_cbItem, v.Description);
                _cbItem.Content = v;
                cb.Items.Add(_cbItem);
                //-->判断默认值，如果默认值，与当前是一样的，cb.SelectedItem = v;
                if (DefaultValue.ToString().Equals(v.Name))
                {
                    cb.SelectedItem = _cbItem;
                }
            }
            return cb;
        }

        void cb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            if (null == cb || null == cb.SelectedItem || !((cb.SelectedItem as ComboBoxItem).Content is EnumsTypeDescription))
            {
                return;
            }
            var etd = (cb.SelectedItem as ComboBoxItem).Content as EnumsTypeDescription;
            EvaluationValue = etd.Name;
            _TextChangeMethod(this, new EvaluationCtrlTextChangeEventArgs(sender, etd.Name));
        }

        void dp_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            _TextChangeMethod(this,
                new EvaluationCtrlTextChangeEventArgs(sender,
                    (sender as DatePicker).DisplayDate.ToString()));
        }

        void tb_TextChanged(object sender, TextChangedEventArgs e)
        {
            _TextChangeMethod(this, new EvaluationCtrlTextChangeEventArgs(sender, (sender as TextBox).Text));
        }

        void _ErrorDataType(Type tt, TextBox tb)
        {
            string fullName = tt.Name;
            switch (fullName)
            {
                case "ToBoolean":
                    break;
                case "ToChar":
                case "ToString":
                    tb.Text = string.Empty;
                    break;
                case "ToDateTime":
                case "ToDecimal":
                case "ToSingle":
                case "ToDouble":
                    tb.Text = "0.0";
                    break;
                case "ToByte":
                case "ToInt16":
                case "ToInt32":
                case "ToInt64":
                case "ToSByte":
                case "ToUInt16":
                case "ToUInt32":
                case "ToUInt64":
                    tb.Text = "0";
                    break;

            }
        }

        void _PropertyChangedMethod(string propertyName)
        {
            if (null == PropertyChanged)
            {
                return;
            }
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        void dp_LostFocus(object sender, RoutedEventArgs e)
        {
            DatePicker dp = sender as DatePicker;
            EvaluationValue = dp.DisplayDate;
            _LostFocusMethod(this, new EvaluationCtrlLostFocusEventArgs(sender, this.MethodName, this.EvaluationValue.ToString(), this.CurrentSelectedControlName));
        }

        void tb_LostFocus(object sender, RoutedEventArgs e)
        {
            //->判断数据类型是否正确
            //IConvertible
            TextBox tb = sender as TextBox;
            if (null == tb)
            {
                return;
            }
            string value = tb.Text;
            Type tt = DataType;
            try
            {
                Convert.ChangeType(value, tt, null);
                EvaluationValue = value;
                _LostFocusMethod(this, new EvaluationCtrlLostFocusEventArgs(sender, this.MethodName, this.EvaluationValue.ToString(), this.CurrentSelectedControlName));
            }
            catch (Exception ee)
            {
                tb.Text = string.Format("{0}", DefaultValue);
                MessageBox.Show(ee.Message);
            }

        }

        void cb_LostFocus(object sender, RoutedEventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            if (null == cb.SelectedItem)
            {
                return;
            }
            ComboBoxItem cbItem = cb.SelectedItem as ComboBoxItem;
            EvaluationValue = string.Format("{0}", cbItem.Content.ToString());
            _LostFocusMethod(this, new EvaluationCtrlLostFocusEventArgs(sender, this.MethodName, this.EvaluationValue.ToString(), this.CurrentSelectedControlName));
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            this.Loaded -= pgec_Loaded;
            if (0 == gControlType.Children.Count)
            {
                return;
            }
            UIElement ui = gControlType.Children[0];
            if (ui is DatePicker)
            {
                DatePicker dp = ui as DatePicker;
                dp.SelectedDateChanged -= dp_SelectedDateChanged;
                dp.LostFocus -= dp_LostFocus;
            }
            else if (ui is TextBox)
            {
                TextBox tb = ui as TextBox;
                tb.TextChanged -= tb_TextChanged;
                tb.LostFocus -= tb_LostFocus;
            }
            else if (ui is PropertyGridDialogCtrl)
            {
                PropertyGridDialogCtrl dc = ui as PropertyGridDialogCtrl;
                dc.Dispose();
            }
            else if (ui is ComboBox)
            {
                ComboBox cb = ui as ComboBox;
                cb.SelectionChanged -= cb_SelectionChanged;
                cb.LostFocus -= cb_LostFocus;
            }
        }

        /***************************************************************
         * 
         * Format 
         * 
         *      1、
         *      <item PropertyName="MethodName" PropertyValue=""/>
         *      2、
         *      <item PropertyName="MethodName">
         *          Content
         *      </item>
         * 
         * *************************************************************/
        /// <summary>
        /// 赋值内容
        /// </summary>
        /// <returns></returns>
        public XElement EvaluationContent
        {
            get
            {
                Type tt = DataType as Type;
                if (tt.Name.Equals("Char"))
                {
                    EvaluationValue = "";
                }
                XElement xe = EvaluationValue is XElement ? EvaluationValue as XElement :
                    new XElement(MethodName, new XCData(string.Format("{0}", EvaluationValue)));
                return xe;
            }
            set
            {
                EvaluationValue = value;
            }
        }
        /// <summary>
        /// 属性名称
        /// </summary>
        public string PropertyName { get; set; }
    }
}
