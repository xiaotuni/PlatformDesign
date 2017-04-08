using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace PlatformService.Utility
{
    /// <summary>
    /// 封装类
    /// </summary>
    public class Wrapper
    {

        #region silverlight 密码\加密\解密
        /**/
        /// <summary>
        /// 加密数据
        /// </summary>
        /// <param name="input">加密前的字符串</param>
        /// <returns>加密后的字符串</returns>
        public static string Encrypt(string input)
        {
            // 盐值
            string saltValue = "saltValue";
            // 密码值
            string pwdValue = "pwdValue";

            byte[] data = UTF8Encoding.UTF8.GetBytes(input);
            byte[] salt = UTF8Encoding.UTF8.GetBytes(saltValue);

            // AesManaged - 高级加密标准(AES) 对称算法的管理类
            AesManaged aes = new AesManaged();

            // Rfc2898DeriveBytes - 通过使用基于 HMACSHA1 的伪随机数生成器，实现基于密码的密钥派生功能 (PBKDF2 - 一种基于密码的密钥派生函数)
            // 通过 密码 和 salt 派生密钥
            Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(pwdValue, salt);

            /*
             * AesManaged.BlockSize - 加密操作的块大小（单位：bit）
             * AesManaged.LegalBlockSizes - 对称算法支持的块大小（单位：bit）
             * AesManaged.KeySize - 对称算法的密钥大小（单位：bit）
             * AesManaged.LegalKeySizes - 对称算法支持的密钥大小（单位：bit）
             * AesManaged.Key - 对称算法的密钥
             * AesManaged.IV - 对称算法的密钥大小
             * Rfc2898DeriveBytes.GetBytes(int 需要生成的伪随机密钥字节数) - 生成密钥
             */

            aes.BlockSize = aes.LegalBlockSizes[0].MaxSize;
            aes.KeySize = aes.LegalKeySizes[0].MaxSize;
            aes.Key = rfc.GetBytes(aes.KeySize / 8);
            aes.IV = rfc.GetBytes(aes.BlockSize / 8);

            // 用当前的 Key 属性和初始化向量 IV 创建对称加密器对象
            ICryptoTransform encryptTransform = aes.CreateEncryptor();

            // 加密后的输出流
            MemoryStream encryptStream = new System.IO.MemoryStream();

            // 将加密后的目标流（encryptStream）与加密转换（encryptTransform）相连接
            CryptoStream encryptor = new CryptoStream(encryptStream, encryptTransform, CryptoStreamMode.Write);

            // 将一个字节序列写入当前 CryptoStream （完成加密的过程）
            encryptor.Write(data, 0, data.Length);
            encryptor.Close();

            // 将加密后所得到的流转换成字节数组，再用Base64编码将其转换为字符串
            string encryptedString = Convert.ToBase64String(encryptStream.ToArray());

            return encryptedString;
        }

        /**/
        /// <summary>
        /// 解密数据
        /// </summary>
        /// <param name="input">加密后的字符串</param>
        /// <returns>加密前的字符串</returns>
        public static string Decrypt(string input)
        {
            // 盐值（与加密时设置的值一致）
            string saltValue = "saltValue";
            // 密码值（与加密时设置的值一致）
            string pwdValue = "pwdValue";

            byte[] encryptBytes = Convert.FromBase64String(input);
            byte[] salt = Encoding.UTF8.GetBytes(saltValue);

            AesManaged aes = new AesManaged();
            Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(pwdValue, salt);

            aes.BlockSize = aes.LegalBlockSizes[0].MaxSize;
            aes.KeySize = aes.LegalKeySizes[0].MaxSize;
            aes.Key = rfc.GetBytes(aes.KeySize / 8);
            aes.IV = rfc.GetBytes(aes.BlockSize / 8);

            // 用当前的 Key 属性和初始化向量 IV 创建对称解密器对象
            ICryptoTransform decryptTransform = aes.CreateDecryptor();

            // 解密后的输出流
            MemoryStream decryptStream = new MemoryStream();

            // 将解密后的目标流（decryptStream）与解密转换（decryptTransform）相连接
            CryptoStream decryptor = new CryptoStream(decryptStream, decryptTransform, CryptoStreamMode.Write);

            // 将一个字节序列写入当前 CryptoStream （完成解密的过程）
            decryptor.Write(encryptBytes, 0, encryptBytes.Length);
            decryptor.Close();

            // 将解密后所得到的流转换为字符串
            byte[] decryptBytes = decryptStream.ToArray();
            string decryptedString = UTF8Encoding.UTF8.GetString(decryptBytes, 0, decryptBytes.Length);

            return decryptedString;
        }

        #endregion

        /// <summary>
        /// 修改xaml属性值
        /// </summary>
        /// <param name="modify"></param>
        /// <param name="controlXaml"></param>
        /// <returns></returns>
        public static string ModifyXamlAttribute(Dictionary<string, object> modify, string controlXaml)
        {
            string split = "_1_2_3_4_";
            controlXaml = controlXaml.Replace(":", split);
            XElement xaml = XElement.Parse(controlXaml);
            foreach (var v in modify)
            {
                xaml.Attribute(v.Key).Value = string.Format("{0}", v.Value == null ? "" : v.Value);
            }
            controlXaml = xaml.ToString();
            controlXaml = controlXaml.Replace(split, ":");
            return controlXaml;
        }
        /// <summary>
        /// 获取属性值
        /// </summary>
        /// <param name="UIElement"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static object GetPropertyValue(object UIElement, string propertyName)
        {
            if (null == UIElement || propertyName.IsNullOrEmpty())
            {
                return null;
            }
            PropertyInfo pi = UIElement.GetType().GetProperty(propertyName);
            if (null == pi)
            {
                return null;
            }
            return pi.GetValue(UIElement, null);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result"></param>
        /// <returns></returns>
        public static List<T> ConvertToList<T>(XElement result) where T : new()
        {
            XElement _table = result.Element("rows");
            List<T> item = new List<T>();
            foreach (var v in _table.Elements("row"))
            {
                T _new = new T();
                Type tt = _new.GetType();
                PropertyInfo[] piItem = tt.GetProperties();
                foreach (var pi in piItem)
                {
                    //pi.Name.ToString()
                    string value = v.GetAttributeValue(pi.Name.ToLower());
                    if (value.IsNullOrEmpty())
                    {
                        continue;
                    }
                    try
                    {
                        pi.SetValue(_new, Convert.ChangeType(value, pi.PropertyType, null), null);
                    }
                    catch
                    {
                    }
                }
                item.Add(_new);
            }
            return item;
        }

        /// <summary>
        /// 格式是：
        /// sql;table
        /// {
        ///     sql = select * from table;
        ///     table = user_info;
        /// }
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static string ParseSqlSentence(string sql, string tableName)
        {
            return string.Format("{0};{1}", sql, tableName);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        /// <param name="methodName"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static object InvokeMethod(object param, string methodName, object obj)
        {
            Type tt = obj.GetType();
            MethodInfo method = tt.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (null == method)
            {
                return null;
            }
            return method.Invoke(obj, new object[] { param });
        }
        /// <summary>
        /// 获取GUID的值
        /// </summary>
        public static string GuidValue
        {
            get { return Guid.NewGuid().ToString("N").ToUpper(); }
        }


        /// <summary>
        /// 设置xml的值
        /// </summary>
        /// <param name="nodeName"></param>
        /// <param name="functionName"></param>
        /// <param name="dictAttribute"></param>
        /// <returns></returns>
        public static XElement SetXmlValue(string nodeName, string functionName, params KeyValuePair<String, Object>[] dictAttribute)
        {
            XElement xml = new XElement(nodeName);
            if (!functionName.IsNullOrEmpty())
            {
                xml.Add(new XAttribute("FunctionName", functionName));
            }
            if (null != dictAttribute && 0 < dictAttribute.Length)
            {
                foreach (var v in dictAttribute)
                {
                    xml.Add(new XAttribute(v.Key, v.Value));
                }
            }
            return xml;
        }

        /// <summary>
        /// 设置xml的值
        /// </summary>
        /// <param name="nodeName"></param>
        /// <param name="functionName"></param>
        /// <param name="dictAttribute"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static XElement SetXmlValue(string nodeName, string functionName, List<object> content, params KeyValuePair<String, Object>[] dictAttribute)
        {
            XElement xml = SetXmlValue(nodeName, functionName, dictAttribute);
            foreach (var v in content)
            {
                xml.Add(v is XElement ? v : new XCData(string.Format("{0}", v)));
            }
            return xml;
        }
    }
}
