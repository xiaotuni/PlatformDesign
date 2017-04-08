using System.Windows.Controls;
using System.Windows.Input;
using PlatformClient.Common.Interfaces;
using PlatformClient.Model.Method;

namespace PlatformClient.PageDesignTime
{
    /// <summary>
    /// 锚点
    /// </summary>
    public partial class Auchor : UserControl
    {
        AuchorLocation location;
        /// <summary>
        /// 
        /// </summary>
        public Auchor()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 获取或设置锚点方位
        /// </summary>
        public AuchorLocation Location
        {
            get { return location; }
            set
            {
                location = value;
                if (value == AuchorLocation.上右 || value == AuchorLocation.下左)
                {
                    rtg.Cursor = Cursors.SizeNESW;
                }
                if (value == AuchorLocation.上左 || value == AuchorLocation.下右)
                {
                    rtg.Cursor = Cursors.SizeNWSE;
                }
                if (value == AuchorLocation.上中 || value == AuchorLocation.下中)
                {
                    rtg.Cursor = Cursors.SizeNS;
                }
                if (value == AuchorLocation.右中 || value == AuchorLocation.左中)
                {
                    rtg.Cursor = Cursors.SizeWE;
                }
            }
        }
    }
}
