using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using PlatformClient.Extend.Core;
using PlatformClient.Model.Method;

namespace PlatformClient.Utility.ParseXml
{
    /// <summary>
    /// 
    /// </summary>
    public class ParseActivityConfig
    {

        static List<ActivityConfig> _ActivityItem = new List<ActivityConfig>();
        static List<ApiSelector> _ApiSelectorItem = new List<ApiSelector>();
        private static ActivityRoot _ActivityRoot;
        /// <summary>
        /// 
        /// </summary>
        public static List<ActivityConfig> ActivityItem
        {
            get
            {
                if (null == _ActivityItem || 0 == _ActivityItem.Count)
                {
                    if (null == _ActivityRoot)
                    {
                        InitLoadConfig();
                    }
                    _ActivityItem = null != _ActivityRoot ? _ActivityRoot.ActivityItem : null;
                }
                return _ActivityItem;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static List<ApiSelector> ApiSelectorItem
        {
            get
            {
                if (null == _ApiSelectorItem || 0 == _ApiSelectorItem.Count)
                {
                    if (null == _ActivityRoot)
                    {
                        InitLoadConfig();
                    }
                    _ApiSelectorItem = null != _ActivityRoot ? _ActivityRoot.ApiItem : null;
                }
                return _ApiSelectorItem;
            }
        }

        static ParseActivityConfig()
        {
            InitLoadConfig();
        }

        static void InitLoadConfig()
        {
            try
            {
                XElement root = XElement.Load(ConstantCollection.Path_ActivityConfig);
                if (null == root)
                {
                    return;
                }
                _ActivityRoot = root.ToModel<ActivityRoot>();
            }
            catch { }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ActivityConfig GetActivityConfig(string name)
        {
            if (null == _ActivityRoot)
            {
                return null;
            }
            return _ActivityRoot.ActivityItem.Where(p => p.Name.Equals(name)).GetFirst<ActivityConfig>();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ApiSelector GetApiSelector(string name)
        {
            if (null == _ActivityRoot)
            {
                return null;
            }
            return _ActivityRoot.ApiItem.Where(p => p.Name.Equals(name)).GetFirst<ApiSelector>();
        }
    }
}
