using System.Windows;
using System.Windows.Controls;
using PlatformClient.Common.Interfaces;
using PlatformClient.Model.Method;

namespace PlatformClient.PageRuntime.Lib
{
    /// <summary>
    /// 
    /// </summary>
    public class EventBinding
    {

        /// <summary>
        /// 单击事件
        /// </summary>
        /// <param name="sender">事件源</param>
        /// <param name="e"></param>
        public static void ClickEvent(object sender, RoutedEventArgs e)
        {
            //ProcessEventAndMethod("Click", sender, e);
            Button btn = sender as Button;
            var ipr = btn == null ? null : btn.Tag is IPageRuntime ? btn.Tag as IPageRuntime : null;
            if (null == ipr)
            {
                return;
            }
            EventLinkInfo eli = ipr.GetEventLinkInfoByControlName(btn.Name);
            ipr.CompositeControlNotifyRuntimePage("", "");
            ipr.EventBindNotifiyRuntimePage(btn.Name);
        }

        /// <summary>
        /// 加载事件
        /// </summary>
        /// <param name="sender">事件源</param>
        /// <param name="e"></param>
        public static void LoadedEvent(object sender, RoutedEventArgs e)
        {
            //ProcessEventAndMethod("Loaded", sender, e);
        }

        /// <summary>
        /// 密码变更事件
        /// </summary>
        /// <param name="sender">事件源</param>
        /// <param name="e"></param>
        public static void PasswordChangedEvent(object sender, RoutedEventArgs e)
        {
            //ProcessEventAndMethod("PasswordChanged", sender, e);
        }

        /// <summary>
        /// 失去焦点事件
        /// </summary>
        /// <param name="sender">事件源</param>
        /// <param name="e"></param>
        public static void LostFocusEvent(object sender, RoutedEventArgs e)
        {
            //ProcessEventAndMethod("LostFocus", sender, e);
        }

        /// <summary>
        /// 选择改变事件
        /// </summary>
        /// <param name="sender">事件源</param>
        /// <param name="e"></param>
        public static void SelectionChangedEvent(object sender, SelectionChangedEventArgs e)
        {
            //if (sender is ComboBox)
            //{
            //    ComboBox cbb = sender as ComboBox;
            //}
            //else if (sender is DataGrid)
            //{
            //    DataGrid dg = sender as DataGrid;
            //    //-->弹出显示的内容。
            //}
        }

        /// <summary>
        /// 内容改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void TextChangedEvent(object sender, TextChangedEventArgs e)
        {
        }
    }
}
