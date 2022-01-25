using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BlankApp.Cli.App
{
    public class Program
    {
        static void Main(string[] args)
        {
            if( args.Length > 0 && args[0].Equals("生成注册表脚本"))
            {
                生成注册表脚本();
            }
            else
            {
                BlankApp.Cli.Program.Main(args);
            }
        }

        [Register("", Type = RegisterType.Tree, Icon ="档案袋.ico")]
        public void 文档() { }

        [Register("/文档", "%1", "插入.ico",FuncName = "插入条目")]
        public void 插入(string artiPath) { }

        [Register("/文档", "%1", "删除.ico", FuncName = "删除条目")]
        public void 删除(string artiPath) { }
        [Register("/文档", "%1", "上移.ico", FuncName = "上移条目")]
        public void 上移(string artiPath) { }

        [Register("/文档", "%1", "下移.ico", FuncName = "下移条目")]
        public void 下移(string artiPath) { }

        [Register("", "%1", "编辑.ico", BinName = "BlankApp.Input.exe")]
        public void 打开档案工具(string archPath) { }



        static void 生成注册表脚本()
        {
            Dictionary<string, RegisterAttribute> tree = new Dictionary<string, RegisterAttribute>();
            Dictionary<string, RegisterAttribute> func = new Dictionary<string, RegisterAttribute>();

            
            MethodInfo[] methodInfos = typeof(Program).GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
            var enumerable = methodInfos.Where(m => m.GetCustomAttribute<RegisterAttribute>() != null);
            foreach (var item in enumerable)
            {
                RegisterAttribute ra = item.GetCustomAttribute<RegisterAttribute>();
                RegisterAttribute result = ra.Clone();


                // 运行程序名字
                if( ra.BinName == null)
                {
                    result.BinName = Path.GetFileName(typeof(Program).Assembly.Location);
                }

                // 设置资源路径
                if (ra.Icon != null)
                {
                    string iconPath = Path.Combine(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "Resources", ra.Icon);
                    if (File.Exists(iconPath))
                    {
                        result.Icon = Path.Combine("Resources", ra.Icon);
                    }
                }

                // 设置多级节点
                if (!ra.UseNonFuncName)
                {
                    result.Multilevel = ra.Multilevel + "/" + item.Name;
                }

                // 可使用应用图标
                if (ra.UseProcessIcon)
                {
                    result.Icon = Path.GetFileName(typeof(Program).Assembly.Location); 
                }

                // 分别放到 tree，func 中，用完整Multilevel 做索引
                if (ra.Type == RegisterType.Tree)
                {
                    if (tree.ContainsKey(result.Multilevel))
                    {
                        tree[result.Multilevel] = result;
                    }
                    else
                    {
                        tree.Add(result.Multilevel, result);
                    }
                }
                else if (ra.Type == RegisterType.Func)
                {
                    if (func.ContainsKey(result.Multilevel))
                    {
                        func[result.Multilevel] = result;
                    }
                    else
                    {
                        func.Add(result.Multilevel, result);
                    }
                }
            }

            string dirPath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Windows Registry Editor Version 5.00");
            sb.AppendLine();
            foreach (string key in tree.Keys)
            {
                sb.AppendLine($"[HKEY_CLASSES_ROOT\\Directory{key.Replace("/", "\\shell\\")}]");
                if (!string.IsNullOrWhiteSpace(tree[key].Icon))
                {
                    sb.AppendLine($"\"Icon\"=\"{Path.Combine(dirPath, tree[key].Icon)}\"".Replace("\\", "\\\\"));
                }
                sb.AppendLine($"\"SubCommands\"=\"\"");
                sb.AppendLine();
            }

            foreach (string key in func.Keys)
            {
                if (!string.IsNullOrWhiteSpace(func[key].Icon))
                {
                    sb.AppendLine($"[HKEY_CLASSES_ROOT\\Directory{key.Replace("/", "\\shell\\")}]");
                    sb.AppendLine($"\"Icon\"=\"{Path.Combine(dirPath, func[key].Icon)}\"".Replace("\\", "\\\\"));
                    sb.AppendLine();
                }
                sb.AppendLine($"[HKEY_CLASSES_ROOT\\Directory{key.Replace("/", "\\shell\\")}\\command]");
                sb.Append($"@=\"{Path.Combine(dirPath, func[key].BinName)} {func[key].FuncName}".Replace("\\", "\\\\"));
                foreach (string pa in func[key].Paraments.Split(' '))
                {
                    if (!string.IsNullOrWhiteSpace(pa))
                    {
                        sb.Append($" \\\"{pa.Trim()}\\\"");

                    }
                }
                sb.AppendLine("\"");
                sb.AppendLine();
            }

            string path = Path.Combine(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "setup.reg");
            File.WriteAllText(path, sb.ToString(), Encoding.Unicode);
            sb.Replace("[", "[-");
            path = path.Replace("setup.reg", "setup_un.reg");
            File.WriteAllText(path, sb.ToString(), Encoding.Unicode);

        }





    }
}
