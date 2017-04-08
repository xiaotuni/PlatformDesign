using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PlatformClient.Common;
using PlatformClient.Common.Interfaces;
using PlatformClient.EventDesign.Core.Lib;
using PlatformClient.Model.Method;
using PlatformClient.Utility;
using PlatformClient.Extend.Core;

namespace PlatformClient.EventDesign.Core.Activitys
{
    /// <summary>
    /// 
    /// </summary>
    public partial class XtnBaseActivity : Grid, IBaseActivity
    {
        string _ActivityGUID;
        Dictionary<String, IArrowLine> _DictArrowCapPoint = new Dictionary<String, IArrowLine>();
        Dictionary<String, IArrowLine> _DictArrowFootPoint = new Dictionary<String, IArrowLine>();
        /// <summary>
        /// 
        /// </summary>
        public Point CurrentEnterPoint = new Point();
        /// <summary>
        /// 
        /// </summary>
        public event MouseEventHandler DotMouseEnter;
        /// <summary>
        /// 
        /// </summary>
        public event MouseEventHandler DotMouseMove;
        /// <summary>
        /// 
        /// </summary>
        public event MouseEventHandler DotMouseLeave;
        /// <summary>
        /// 
        /// </summary>
        public event MouseButtonEventHandler DotMouseLeftButtonUp;
        /// <summary>
        /// 
        /// </summary>
        public event MouseButtonEventHandler DotMouseLeftButtonDown;
        /// <summary>
        /// 箭头集合
        /// </summary>
        public Dictionary<String, IArrowLine> DictArrowCapPoint { get { return _DictArrowCapPoint; } set { _DictArrowCapPoint = value; } }
        /// <summary>
        /// 箭尾集合
        /// </summary>
        public Dictionary<String, IArrowLine> DictArrowFootPoint { get { return _DictArrowFootPoint; } set { _DictArrowFootPoint = value; } }
        /// <summary>
        /// 
        /// </summary>
        public Point CurrentLeftButtonDownPoint { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public String LabelContent
        {
            get { return this.txtCommon.Text; }
            set
            {
                this.txtCommon.Text = value;
                //   ToolTipService.SetToolTip(this, value);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public string ActivityGUID
        {
            get
            {
                if (String.IsNullOrEmpty(_ActivityGUID))
                {
                    _ActivityGUID = Wrapper.GuidValue;
                }
                return _ActivityGUID;
            }
            set { _ActivityGUID = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected List<ContextMenuInfo> _ContextMenus
        {
            get
            {
                return new List<ContextMenuInfo>{
                    new ContextMenuInfo{ Header="删除",  Type = ContextMenuType.Delete, Source=this },
                    new ContextMenuInfo{ Header="修改内容", Type = ContextMenuType.ModifyContent,Source=this}
                };
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public XtnBaseActivity()
        {
            InitializeComponent();
            this.eDot.MouseEnter += eDot_MouseEnter;
            this.eDot.MouseLeave += eDot_MouseLeave;
            this.eDot.MouseMove += eDot_MouseMove;
            this.eDot.MouseLeftButtonDown += eDot_MouseLeftButtonDown;
            this.eDot.MouseLeftButtonUp += eDot_MouseLeftButtonUp;
        }

        void eDot_MouseLeave(object sender, MouseEventArgs e)
        {
            if (null != DotMouseLeave)
            {
                DotMouseLeave(this, e);
            }
        }

        void eDot_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (null != DotMouseLeftButtonUp)
            {
                DotMouseLeftButtonUp(this, e);
            }
        }

        void eDot_MouseMove(object sender, MouseEventArgs e)
        {
            if (null != DotMouseMove)
            {
                DotMouseMove(this, e);
            }
        }

        void eDot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (null != DotMouseLeftButtonDown)
            {
                DotMouseLeftButtonDown(this, e);
            }
        }

        void eDot_MouseEnter(object sender, MouseEventArgs e)
        {
            if (null != DotMouseEnter)
            {
                DotMouseEnter(this, e);
            }
        }

        /// <summary>
        /// 更新所有箭头的坐标
        /// </summary>
        /// <param name="point"></param>
        public void UpdateArrowCapPoint(Point point)
        {
            if (CurrentEnterPoint.X.Equals(point.X) && CurrentEnterPoint.Y.Equals(point.Y))
            {
                return;
            }
            double _x = 0;
            double _y = 0;
            if (0 != CurrentEnterPoint.X && 0 != CurrentEnterPoint.Y)
            {
                _x = point.X - CurrentEnterPoint.X;
                _y = point.Y - CurrentEnterPoint.Y;
            }

            foreach (var v in DictArrowFootPoint)
            {
                var ep = v.Value.StartPoint;
                v.Value.StartPoint = new Point(ep.X + _x, ep.Y + _y);
            }
            CurrentEnterPoint = point;
        }

        /// <summary>
        /// 更新所有箭尾的坐标
        /// </summary>
        /// <param name="point"></param>
        public void UpdateArrowFootPoint(Point point)
        {
            if (CurrentLeftButtonDownPoint.X.Equals(point.X) && CurrentLeftButtonDownPoint.Y.Equals(point.Y))
            {
                return;
            }
            double _x = 0;
            double _y = 0;
            if (0 != CurrentLeftButtonDownPoint.X && 0 != CurrentLeftButtonDownPoint.Y)
            {
                _x = point.X - CurrentLeftButtonDownPoint.X;
                _y = point.Y - CurrentLeftButtonDownPoint.Y;
            }

            foreach (var v in DictArrowCapPoint)
            {
                var sp = v.Value.EndPoint;
                v.Value.EndPoint = new Point(sp.X + _x, sp.Y + _y);
            }
            CurrentLeftButtonDownPoint = point;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="line"></param>
        public void RemoveLine(IArrowLine line)
        {
            //if (DictArrowCapPoint.ContainsKey(line))
            //{
            //    DictArrowCapPoint.Remove(line);
            //}
            //if (DictArrowFootPoint.ContainsKey(line))
            //{
            //    DictArrowFootPoint.Remove(line);
            //}

            if (DictArrowCapPoint.ContainsKey(line.CtrName))
            {
                DictArrowCapPoint.Remove(line.CtrName);
            }
            if (DictArrowFootPoint.ContainsKey(line.CtrName))
            {
                DictArrowFootPoint.Remove(line.CtrName);
            }
        }

        /// <summary>
        /// 判断线是否已经存在了
        /// </summary>
        /// <param name="iact"></param>
        /// <returns></returns>
        public bool CheckedArrowIsExists(IActivity iact)
        {
            //-->所有字典里的箭头
            foreach (var v in DictArrowCapPoint)
            {
                if (null == v.Value.ArrowFootControl || !(v.Value.ArrowFootControl is IActivity))
                {
                    continue;
                }
                if ((v.Value.ArrowFootControl as IActivity).Equals(iact))
                {
                    return true;
                }
            }
            //-->所有字典里的箭尾
            foreach (var v in DictArrowFootPoint)
            {
                if (null == v.Value.ArrowCapControl || !(v.Value.ArrowCapControl is IActivity))
                {
                    continue;
                }
                if ((v.Value.ArrowCapControl as IActivity).Equals(iact))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        public string ExportLocation()
        {
            ActivityInfo ai = new ActivityInfo();
            ai.Type = this.GetType().Name;
            ai.Name = this.Name;
            ai.Guid = this.ActivityGUID;
            ai.Width = this.Width;
            ai.Height = this.Height;
            ai.Left = Canvas.GetLeft(this);
            ai.Top = Canvas.GetTop(this);
            ai.Content = this.txtCommon.Text.Trim();
            ai.EnterX = CurrentEnterPoint.X;
            ai.EnterY = CurrentEnterPoint.Y;

            var aa = ai.ToXElement("Activity");
            return aa.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        public string ExportControlRelationship()
        {  //-->
            List<string> item = new List<string>();
            string _arrowCap = string.Empty;
            List<String> capItem = new List<string>();
            foreach (var v in DictArrowCapPoint)
            {
                capItem.Add(v.Key);
            }
            List<String> footItem = new List<string>();
            foreach (var v in DictArrowFootPoint)
            {
                footItem.Add(v.Key);
            }
            string result = string.Empty;
            result += string.Format("<Control Name=\"{0}\" Cap=\"{1}\" Foot=\"{2}\" />",
                this.Name, string.Join("|", capItem), string.Join("|", footItem));
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            try
            {
                this.eDot.MouseEnter -= eDot_MouseEnter;
                this.eDot.MouseLeave -= eDot_MouseLeave;
                this.eDot.MouseMove -= eDot_MouseMove;
                this.eDot.MouseLeftButtonDown -= eDot_MouseLeftButtonDown;
                this.eDot.MouseLeftButtonUp -= eDot_MouseLeftButtonUp;
                DictArrowFootPoint.Clear();
                DictArrowCapPoint.Clear();
            }
            catch { }
        }

        /// <summary>
        /// 是否可以开始
        /// </summary>
        /// <returns></returns>
        protected bool IsStart()
        {
            //-->如果已经存在一根开始线了，就不能再有了。
            if (0 < DictArrowFootPoint.Count)
            {
                return false;
            }
            return true;
        }
    }
}
