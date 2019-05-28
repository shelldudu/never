using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Never;
using Never.Configuration;
using Never.Configuration.ConfigCenter;

namespace Never.TestConsole.Configurations
{
    public class ConfigCenterTest
    {
        public void Test()
        {
            var config = new System.IO.FileInfo(@"D:\documents\vscode-git\never\Never.Configuration\App_Config\share.config");
            var json = new System.IO.FileInfo(@"D:\documents\vscode-git\never\Never.Configuration\App_Config\share.json");
            var sharBuilder = new ShareConfigurationBuilder(new ConfigFileInfo() { Encoding = Encoding.UTF8, File = json });
            sharBuilder.OnBuilding += SharBuilder_OnShareQueryReplace;
            sharBuilder.Build();
           // var wwwBuilder = new JsonConfigurtionBuilder(new[] { sharBuilder }, new System.IO.FileInfo(@"D:\documents\vscode-git\never\Core\Never.Configuration\App_Config\www.json"), new MyCustomKeyValueFinder()).Build();
           // var wapBuilder = new JsonConfigurtionBuilder(new[] { sharBuilder }, new System.IO.FileInfo(@"D:\documents\vscode-git\never\Core\Never.Configuration\App_Config\wap.json"), new MyCustomKeyValueFinder()).Build();
        }

        private void SharBuilder_OnShareQueryReplace(object sender, ShareFileEventArgs e)
        {
            if (e.JsonShareFile != null)
            {
                if (e.JsonShareFile.Node.IsNotNullOrEmpty())
                {
                    foreach (var node in e.JsonShareFile.Node.Where(ta => ta.NeedToChange))
                    {
                        if (node.ToString() == "user")
                            node.Replace("1236");
                        else
                            node.Replace("666");
                    }
                }
            }
        }
    }

    public class MyCustomKeyValueFinder : ICustomKeyValueFinder
    {
        public string Find(string mode, string key)
        {
            return "aaa";
        }
    }
}
