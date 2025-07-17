using System;
using System.IO;
using System.Text;
using cfg;
using Luban;
using SimpleJSON;
using UnityEngine;
using Xicheng.Utility;

namespace Xicheng.Datable
{

    public class DataTableManager : MonoSingleton<DataTableManager>, ILogic
    {
        private Tables _tables;
        public bool InitStartUp => true;

        public void OnInit()
        {
            Initialized();
        }

        public void OnRelease()
        {
            _tables = null;
        }

        private void Initialized()
        {
            //只需一行代码即可加载所有配置表。整个游戏运行期间只加载一次（除非要运行中重新加载配置）。
            //实践中在创建tables后将它保存起来，以便后续使用。
            if (_tables == null)
            {
                var tablesCtor = typeof(Tables).GetConstructors()[0];
                var loaderReturnType = tablesCtor.GetParameters()[0].ParameterType.GetGenericArguments()[1];
                // 根据cfg.Tables的构造函数的Loader的返回值类型决定使用json还是ByteBuf Loader
                Delegate loader = loaderReturnType == typeof(ByteBuf)
                    ? new Func<string, ByteBuf>(LoadByteBuf)
                    : new Func<string, JSONNode>(LoadByJson);
                _tables = (Tables)tablesCtor.Invoke(new object[] { loader });
            }
            // Debug.LogFormat("item[1].name:{0}", _tables.TbReward.Get(1001).Name);
            //var t = _tables.TbUILayer.Get(111).Id;

            // 访问一个单例表
            //Debug.Log(tables.TbGlobal.Name);

            // 访问普通的 key-value 表
            //Debug.Log(tables.TbItem.Get(12).Name);
            // 支持 operator []用法
            //Debug.Log(tables.TbMail[1001].Desc);

            // Debug.Log("xc.[ConfigManager.Initialized]Initialized  successful ==");
        }

        private static JSONNode LoadByJson(string file)
        {
            string path = Application.dataPath + $"/AssetsPackage/DataTable/{file}.json";
            return JSON.Parse(File.ReadAllText(path, Encoding.UTF8));
        }

        private static ByteBuf LoadByteBuf(string file)
        {
            string path = Application.dataPath + $"/AssetsPackage/DataTable/{file}.bytes";
            ByteBuf byteBuf = new ByteBuf(File.ReadAllBytes(path));
            return byteBuf;
        }



        public Tables Table
        {
            get
            {
                if (_tables == null)
                {
                    Initialized();
                }

                return _tables;
            }
        }
    }
}
