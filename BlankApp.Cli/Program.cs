using BlankApp.Service;
using BlankApp.Service.Impl;
using NTwain;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Unity;

namespace BlankApp.Cli
{
    public class Program
    {
        private static readonly IUnityContainer _container = new UnityContainer();

        public static void Main(string[] args)
        {

            LoadContainers();
            Util util = null;
            try
            {
                util = _container.Resolve<Util>();
                    object[] objs = new object[args.Length - 1];
                    for (int i = 1; i < args.Length; i++)
                    {
                        objs[i - 1] = args[i];
                    }
                    typeof(Util).GetMethod(args[0]).Invoke(util, objs);

            }
            catch (Exception e)
            {
                if (e.Message.Equals("未将对象引用设置到对象的实例。"))
                {
                    Console.WriteLine("命令不存在。");
                }
                else if (e.Message.Equals("参数计数不匹配。"))
                {
                    Console.WriteLine("命令参数错误。");
                    util.命令帮助(typeof(Util).GetMethod(args[0]));
                }
                else
                {
                    File.WriteAllText(Path.Combine(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "log.txt"), e.ToString());
                }
            }
        }


        static void LoadContainers()
        {
            _container.RegisterSingleton<IConfigurationService, ConfigurationService>();
            _container.RegisterSingleton<IMaskService, MaskService>();
            _container.RegisterSingleton<IArchiveService, ArchiveService>();
            _container.RegisterSingleton<IArticleService, RawArticleService>();
            _container.RegisterSingleton<IWordService, WordService>();
            _container.RegisterSingleton<IReportService, ReportService>();
            _container.RegisterSingleton<Util>();
        }

    }
}
