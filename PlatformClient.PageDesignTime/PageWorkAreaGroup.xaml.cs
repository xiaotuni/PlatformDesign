using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Linq;
using PlatformClient.Common;
using PlatformClient.Common.Events;
using PlatformClient.Common.Interfaces;
using PlatformClient.Common.Lib;
using PlatformClient.Model.Table;
using PlatformClient.PlatformServer;
using PlatformClient.PropertyGrid.Events;
using PlatformClient.Utility;
using PlatformClient.Utility.Events;
using PlatformClient.Model.Events;
using PlatformClient.Model.Method;
using PlatformClient.Extend.Core;

namespace PlatformClient.PageDesignTime
{
    /// <summary>
    /// 
    /// </summary>
    public partial class PageWorkAreaGroup : BaseControl, IDisposable
    {
        /// <summary>
        /// 控件位置发生变发
        /// </summary>
        public event EventHandler<ControlModifyPropertyEventArgs> ControlPosition;
        /// <summary>
        /// 更新控件数量事件
        /// </summary>
        public event EventHandler<ControlTimesUpdateEventArgs> ControlTimesUpdate;
        /// <summary>
        /// 清空操作
        /// </summary>
        public event EventHandler OnClear;

        /// <summary>
        /// 设计时框架接口
        /// </summary>
        internal IPageDesignFramework IDesignFramework { get; set; }

        /// <summary>
        /// 当前的TabItem
        /// </summary>
        internal object SelectedItem { get { return tcCollection.SelectedItem; } }

        void _ControlPositionMethod(object sender, ControlModifyPropertyEventArgs e)
        {
            if (null != ControlPosition)
            {
                ControlPosition(sender, e);
            }
        }

        void pwa_ControlTimesUpdate(object sender, ControlTimesUpdateEventArgs e)
        {
            if (null != ControlTimesUpdate)
            {
                ControlTimesUpdate(sender, e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public PageWorkAreaGroup()
        {
            InitializeComponent();
        }

        void pwa_ControlPosition(object sender, ControlModifyPropertyEventArgs e)
        {
            _ControlPositionMethod(sender, e);
        }

        /// <summary>
        /// 打开一个新的设计页面
        /// </summary>
        /// <param name="e"></param>
        internal void OpenPageInfo(OpenPageInfoEventArgs e)
        {
            //-->判断当前页面是否已经打开，如果打开了，就设置为选中状态，否则就打开一个新的。
            bool isOpen = JudgePageIsOpen(e);
            if (!isOpen)
            {
                TabItem ti = new TabItem();
                TabItemHeader header = new TabItemHeader();
                PageWorkArea pwa = new PageWorkArea();
                pwa.ControlPosition += pwa_ControlPosition;
                pwa.ContentUpdate += pwa_ContentUpdate;
                pwa.ControlTimesUpdate += pwa_ControlTimesUpdate;
                pwa.ParentControl = ti;
                ti.Header = header;
                ti.Content = pwa;
                this.tcCollection.Items.Add(ti);

                header.OnCloseTabItem += header_Delete;
                header.MouseLeftButtonUp += header_MouseLeftButtonUp;
                header.Tag = ti;
                header.Header = e.PageDirectorySub.PageName;

                ti.Tag = e.PageDirectorySub;
                ti.Name = string.Format("TabItem_{0}", e.PageDirectorySub.ID);
                ti.IsSelected = true;
                pwa.OpenPageInfo(e, IDesignFramework);
            }
        }

        void header_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //-->删除当前点击的TabItem
            TabItemHeader btn = sender as TabItemHeader;
            if (null == btn || null == btn.Tag)
            {
                return;
            }
            TabItem ti = btn.Tag as TabItem;
            PageWorkArea pwa = ti.Content as PageWorkArea;
            pwa.UpdatePropertyGridAndControlTree();
        }

        void pwa_ContentUpdate(object sender, RoutedEventArgs e)
        {
            PageWorkArea pwa = sender as PageWorkArea;
            TabItem ti = pwa.ParentControl as TabItem;
            TabItemHeader header = ti.Header as TabItemHeader;
            header.SetUpdateImage(pwa.IsUpdate);
        }

        void header_Delete(object sender, RoutedEventArgs e)
        {
            //-->删除当前点击的TabItem
            TabItemHeader btn = sender as TabItemHeader;
            if (null == btn || null == btn.Tag)
            {
                return;
            }
            TabItem ti = btn.Tag as TabItem;
            PageWorkArea pwa = ti.Content as PageWorkArea;
            pwa.ControlPosition -= pwa_ControlPosition;
            pwa.ContentUpdate -= pwa_ContentUpdate;
            pwa.Dispose();
            pwa.ParentControl = null;
            pwa = null;
            btn.OnCloseTabItem -= header_Delete;
            tcCollection.Items.Remove(ti);
            ti.Content = null;
            ti = null;

            var cpwa = CurrentPageWorkArea;
            if (null == cpwa)
            {
                //-->清空属性控件，和文本大刚。
                if (null != OnClear)
                {
                    OnClear(this, null);
                }
                return;
            }
            //-->更新当前的属性控件
            //-->更新文本大刚
            CurrentPageWorkArea.UpdatePropertyGridAndControlTree();
        }

        /// <summary>
        /// 设置TabItem的页面头
        /// </summary>
        /// <param name="title"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        object GetHeader(string title, UIElement parent)
        {
            StackPanel sp = new StackPanel();
            sp.Orientation = Orientation.Horizontal;
            TextBlock tb = new TextBlock();
            tb.Text = title;
            tb.VerticalAlignment = VerticalAlignment.Center;
            sp.Children.Add(tb);
            Button btn = new Button();
            btn.Content = "x";
            btn.Height = 15;
            btn.Click += btn_Click;
            btn.Tag = parent;
            sp.Children.Add(btn);
            return sp;
        }

        void btn_Click(object sender, RoutedEventArgs e)
        {
            //-->删除当前点击的TabItem
            Button btn = sender as Button;
            if (null == btn || null == btn.Tag)
            {
                return;
            }
            TabItem ti = btn.Tag as TabItem;
            PageWorkArea pwa = ti.Content as PageWorkArea;
            pwa.ControlPosition -= pwa_ControlPosition;
            pwa.Dispose();
            btn.Click -= btn_Click;
            tcCollection.Items.Remove(ti);
            ti.Content = null;
            ti = null;
        }

        /// <summary>
        /// 判断页面是否打开
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        bool JudgePageIsOpen(OpenPageInfoEventArgs e)
        {
            bool isOpen = false;
            foreach (var v in this.tcCollection.Items)
            {
                TabItem ti = v as TabItem;
                if (null == ti)
                {
                    continue;
                }
                PageDirectorySub pds = ti.Tag as PageDirectorySub;
                if (null == pds)
                {
                    continue;
                }
                if (pds.ID.Equals(e.PageDirectorySub.ID))
                {
                    ti.IsSelected = true;
                    isOpen = true;
                    break;
                }
            }
            return isOpen;
        }

        /// <summary>
        /// 当前工作页面
        /// </summary>
        internal PageWorkArea CurrentPageWorkArea
        {
            get
            {
                TabItem ti = SelectedItem as TabItem;
                if (null == ti)
                {
                    return null;
                }
                return ti.Content as PageWorkArea;
            }
        }

        /// <summary>
        /// 增加控件
        /// </summary>
        /// <param name="fe"></param>
        internal void AddChild(FrameworkElement fe)
        {
            //-->添加到当前选择的tabItem上去。
            PageWorkArea pwa = CurrentPageWorkArea;
            if (null == pwa)
            {
                return;
            }
            pwa.AddChild(fe);
        }

        /// <summary>
        /// 属性推动焦点事件
        /// </summary>
        /// <param name="e"></param>
        internal void PropertyLostFoucs(EvaluationCtrlLostFocusEventArgs e)
        {
            if (null == CurrentPageWorkArea)
            {
                return;
            }
            CurrentPageWorkArea.PropertyLostFoucs(e);
        }

        /// <summary>
        /// 页面预览
        /// </summary>
        internal void PageView()
        {
            if (null == CurrentPageWorkArea)
            {
                return;
            }
            CurrentPageWorkArea.PageView();
        }

        /// <summary>
        /// 打开数据框架
        /// </summary>
        internal void OpenDBFramework()
        {
            if (null == CurrentPageWorkArea)
            {
                return;
            }
            CurrentPageWorkArea.OpenDBFramework();
        }

        /// <summary>
        /// 保存
        /// </summary>
        internal void Save()
        {
            if (null == CurrentPageWorkArea)
            {
                return;
            }
            this.bi.IsBusy = true;
            cmd = new CtrlPlatformCommandInfo();
            cmd.CommandName = ConstantCollection.CommandName_SavePageInfo;
            List<XElement> xmlItem = new List<XElement>();
            PageWorkArea pwa = this.CurrentPageWorkArea;
            if (!pwa.IsUpdate)
            {
                this.bi.IsBusy = false;
                return;
            }
            string saveContent = string.Empty;
            string sql = SavePageSqlSentence(pwa, ref saveContent);
            XElement item = Wrapper.SetXmlValue("PageInfo", "",
                                    new List<Object>() 
                                    {
                                        saveContent 
                                    },
                                    new KeyValuePair<String, Object>[] 
                                    {
                                        new KeyValuePair<String,Object>("PageGuid",pwa.PageInfo.PageGuid),
                                        new KeyValuePair<String,Object>("SqlSentence",sql),
                                        new KeyValuePair<String,Object>("ControlName",(this.tcCollection.SelectedItem as TabItem).Name)
                                    });
            xmlItem.Add(item);
            cmd.ParamCollection = new XElement("SaveAll", xmlItem);
            cmd.TempValue = Wrapper.SetXmlValue("Save", "SavePage");
            this.SendToService(cmd, this);
        }

        /// <summary>
        /// 全部保存
        /// </summary>
        internal void SaveAll()
        {
            this.bi.IsBusy = true;
            cmd = new CtrlPlatformCommandInfo();
            cmd.CommandName = ConstantCollection.CommandName_SavePageInfo;
            List<XElement> xmlItem = new List<XElement>();
            foreach (var v in this.tcCollection.Items)
            {
                TabItem ti = v as TabItem;
                if (null == ti || !(ti.Content is PageWorkArea))
                {
                    continue;
                }
                PageWorkArea pwa = ti.Content as PageWorkArea;
                if (!pwa.IsUpdate)
                {
                    continue;
                }
                string saveContent = string.Empty;
                string sql = SavePageSqlSentence(pwa, ref saveContent);
                if (null == saveContent)
                {
                    continue;
                }
                XElement item = Wrapper.SetXmlValue("PageInfo", "",
                                        new List<Object>() 
                                        {
                                            saveContent 
                                        },
                                        new KeyValuePair<String, Object>[] 
                                        {
                                            new KeyValuePair<String,Object>("PageGuid",pwa.PageInfo.PageGuid),
                                            new KeyValuePair<String,Object>("SqlSentence",sql),
                                            new KeyValuePair<String,Object>("ControlName",ti.Name)
                                        });
                xmlItem.Add(item);
            }
            cmd.ParamCollection = new XElement("SaveAll", xmlItem);
            cmd.TempValue = Wrapper.SetXmlValue("Save", "SavePage");
            this.SendToService(cmd, this);
        }

        /// <summary>
        /// 获取保存SQL语句
        /// </summary>
        /// <param name="pwa"></param>
        /// <param name="saveContent"></param>
        /// <returns></returns>
        String SavePageSqlSentence(PageWorkArea pwa, ref string saveContent)
        {
            string xaml = pwa.PageXaml;
            string xml = pwa.PageXml;
            PageDirectorySub pds = pwa.PageInfo;

            XElement _xe_xaml = xaml.IsNullOrEmpty() ? null : new XElement("xaml", XElement.Parse(xaml));
            XElement _xe_xml = xml.IsNullOrEmpty() ? null : new XElement("xml", XElement.Parse(xml));
            saveContent = string.Format("<PageSchema>\r\n{0}\r\n{1}\r\n</PageSchema>", _xe_xaml, _xe_xml);

            string sql = string.Format("update PageDirectorySub set ");
            sql += string.Format(" PageContent = ?PageContent");
            sql += string.Format(" where ID = {0}", pds.ID);
            return sql;
        }

        /// <summary>
        /// 返回保存页面是否成功
        /// </summary>
        /// <param name="cmd"></param>
        public void ProcessSavePage(CtrlPlatformCommandInfo cmd)
        {
            var resultItem = cmd.ParamCollection.Elements("PageInfo");
            List<String> errorItem = new List<string>();
            foreach (var v in resultItem)
            {
                string controlName = v.GetAttributeValue("ControlName");
                string result = v.GetAttributeValue("Result");
                string ErrorMessage = v.GetAttributeValue("ErrorMessage");
                TabItem ti = this.tcCollection.FindName(controlName) as TabItem;
                PageWorkArea pwa = ti.Content as PageWorkArea;
                if (result.Equals("0"))
                {
                    pwa.PageInfo.PageContent = v.Value.ToString();
                    //-->更新header
                    pwa.IsUpdate = false;
                }
                else
                {
                    errorItem.Add(string.Format("{0} {1}", pwa.PageInfo.PageName, ErrorMessage));
                }
            }
            if (0 < errorItem.Count)
            {
                Wrapper.ShowDialog(string.Join("\r\n", errorItem));
            }
            this.bi.IsBusy = false;
        }
        /// <summary>
        /// 处理保存操作
        /// </summary>
        /// <param name="cmd"></param>
        public void ProcessSave(CtrlPlatformCommandInfo cmd)
        {
            //-->判断上传操作是否成功了。
            //CurrentPageWorkArea.IsEnabled = true;
            bi.IsBusy = false;
            if (!cmd.ErrorMessage.IsNullOrEmpty())
            {
                Wrapper.ShowDialog("保存失败:" + cmd.ErrorMessage);
                return;
            }
            this.CurrentPageWorkArea.PageInfo.PageContent = cmd.ParamCollection.ToString();
        }

        /// <summary>
        /// 更新Tab标签信息
        /// </summary>
        /// <param name="e"></param>
        internal void UpdateTabItemHeader(OpenPageInfoEventArgs e)
        {
            foreach (var v in this.tcCollection.Items)
            {
                TabItem ti = v as TabItem;
                if (null == ti)
                {
                    continue;
                }
                PageWorkArea pwa = ti.Content as PageWorkArea;
                if (null == pwa)
                {
                    continue;
                }
                if (pwa.PageInfo.PageGuid.Equals(e.PageDirectorySub.PageGuid))
                {
                    pwa.PageInfo.PageName = e.PageDirectorySub.PageName;
                    TabItemHeader header = ti.Header as TabItemHeader;
                    header.Header = e.PageDirectorySub.PageName;
                    pwa.UpdateXmlPageNameValue(pwa.PageInfo.PageName);
                }
            }
        }

        /// <summary>
        /// 当前页面的xml模板
        /// </summary>
        /// <returns></returns>
        internal XmlTemplate GetCurrentXmlTemplate()
        {
            if (null == CurrentPageWorkArea)
            {
                return null;
            }
            return CurrentPageWorkArea._XmlTemplate;
        }
        /// <summary>
        /// 当着页面的xaml模板
        /// </summary>
        /// <returns></returns>
        internal XamlTemplate GetCurrentXamlTemplate()
        {
            if (null == CurrentPageWorkArea)
            {
                return null;
            }
            return CurrentPageWorkArea._XamlTemplate;
        }

        /// <summary>
        /// 更新当前模板
        /// </summary>
        internal void UpdateCurrentTemplate()
        {
            if (null == CurrentPageWorkArea)
            {
                return;
            }
            CurrentPageWorkArea.UpdateCurrentTemplate();
        }

        /// <summary>
        /// 查找控件
        /// </summary>
        /// <param name="controlName"></param>
        internal UIElement FindControl(string controlName)
        {
            if (null == CurrentPageWorkArea)
            {
                return null;
            }
            return CurrentPageWorkArea.FindControl(controlName);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public override void Dispose()
        {
            if (0 == this.tcCollection.Items.Count)
            {
                return;
            }
            foreach (var v in this.tcCollection.Items)
            {
                TabItem ti = v as TabItem;
                PageWorkArea pwa = ti.Content as PageWorkArea;
                pwa.ControlPosition -= pwa_ControlPosition;
                pwa.ContentUpdate -= pwa_ContentUpdate;
                pwa.ParentControl = null;
                pwa.Dispose();
                pwa = null;

                //-->删除当前点击的TabItem
                TabItemHeader btn = ti.Header as TabItemHeader;
                if (null == btn || null == btn.Tag)
                {
                    return;
                }
                btn.OnCloseTabItem -= header_Delete;
                tcCollection.Items.Remove(ti);
                ti.Content = null;
            }
            this.tcCollection.Items.Clear();
        }

        /// <summary>
        /// 获取MetaData信息
        /// </summary>
        /// <param name="tableName">表的名称</param>
        /// <returns>返回一个MetaData对象</returns>
        internal MetaDataInfo GetMetaDataInfoByTableName(string tableName)
        {
            if (null == CurrentPageWorkArea)
            {
                return null;
            }
            return CurrentPageWorkArea.GetMetaDataInfoByTableName(tableName);
        }

        /// <summary>
        /// 获取控件信息
        /// </summary>
        /// <param name="controlName">控件名称</param>
        /// <returns>返回控件信息</returns>
        internal ControlInfo GetControlInfoByControlName(string controlName)
        {
            if (null == CurrentPageWorkArea)
            {
                return null;
            }
            return CurrentPageWorkArea.GetControlInfoByControlName(controlName);
        }
    }
}
