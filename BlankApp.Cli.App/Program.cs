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
        public  static string DesktopLocation = @"HKEY_CLASSES_ROOT\DesktopBackground";
        public  static string FileLocation = @"HKEY_CLASSES_ROOT\*";
        public  static string FolderLocation = @"HKEY_CLASSES_ROOT\Directory";
        public  static string FileAndFolderLocation = @"HKEY_CLASSES_ROOT\AllFilesystemObjects";
        public  static string FolderAndEmptyLocation = @"HKEY_CLASSES_ROOT\Directory/Background";
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

        [Register(Type = RegisterType.Tree, Icon = "档案袋.ico", Location = LocationType.Folder)]
        [Register(Multilevel = "/文档/插入", Paraments = "%1", Icon = "插入.ico", FuncName = "插入条目", UseNonFuncName = true)]
        [Register(Multilevel = "/文档/删除", Paraments = "%1", Icon = "删除.ico", FuncName = "删除条目", UseNonFuncName = true)]
        [Register(Multilevel = "/文档/上移", Paraments = "%1", Icon = "上移.ico", FuncName = "上移条目", UseNonFuncName = true)]
        [Register(Multilevel = "/文档/下移", Paraments = "%1", Icon = "下移.ico", FuncName = "下移条目", UseNonFuncName = true)]
        public void 文档() { }

        [Register(Type = RegisterType.Tree, Icon = "编辑.ico", Location = LocationType.FolderAndEmpty, Paraments = "%1", BinName = "BlankApp.Input.exe")]
        public void 打开档案工具() { }



        static void 生成注册表脚本()
        {
            Dictionary<string, RegisterAttribute> tree = new Dictionary<string, RegisterAttribute>();
            Dictionary<string, RegisterAttribute> func = new Dictionary<string, RegisterAttribute>();

            
            MethodInfo[] methodInfos = typeof(Program).GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
            var ms = methodInfos.Where(m => m.GetCustomAttributes<RegisterAttribute>().Count() > 0);
            foreach (var m in ms)
            {
                foreach (var ra in m.GetCustomAttributes<RegisterAttribute>())
                {
                    RegisterAttribute result = ra.Clone();

                    // 设置资源路径
                    if (ra.Icon != null)
                    {
                        string iconPath = Path.Combine(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "Resources", ra.Icon);
                        if (File.Exists(iconPath))
                        {
                            result.Icon = iconPath;
                        }
                    }

                    // 设置多级节点
                    if (!ra.UseNonFuncName)
                    {
                        result.Multilevel = ra.Multilevel + "/" + m.Name;
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
                
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Windows Registry Editor Version 5.00");
            sb.AppendLine();

            foreach (var t in tree)
            {
                var kvps = func.Where(f => f.Key.StartsWith(t.Key)).ToArray();
                RegisterAttribute[] ras = Array.ConvertAll(kvps, d => d.Value);
                if ((t.Value.Location & LocationType.Desktop) == LocationType.Desktop)
                {
                    RegisterAttribute rac = t.Value.Clone();
                    rac.Multilevel = rac.Multilevel.Remove(0, 1);
                    RegisterAttribute[] rasc = new RegisterAttribute[ras.Length];
                    for (int i = 0; i < ras.Length; i++)
                    {
                        rasc[i] = ras[i].Clone();
                        rasc[i].Multilevel = rasc[i].Multilevel.Remove(0, 1);
                    }
                    sb.AppendLine(Build(DesktopLocation + @"\Shell\", rac, rasc).ToString());
                }
                if ((t.Value.Location & LocationType.File) == LocationType.File)
                {
                    sb.AppendLine(Build(FileLocation, t.Value, ras).ToString());
                }
                if ((t.Value.Location & LocationType.Folder) == LocationType.Folder)
                {
                    sb.AppendLine(Build(FolderLocation, t.Value, ras).ToString());
                }
                if ((t.Value.Location & LocationType.FileAndFolder) == LocationType.FileAndFolder)
                {
                    sb.AppendLine(Build(FileAndFolderLocation, t.Value, ras).ToString());
                }
                if ((t.Value.Location & LocationType.FolderAndEmpty) == LocationType.FolderAndEmpty)
                {
                    sb.AppendLine(Build(FolderAndEmptyLocation, t.Value, ras).ToString());
                }
            }

            string path = Path.Combine(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "setup.reg");
            File.WriteAllText(path, sb.ToString(), Encoding.Unicode);
            sb.Replace("[", "[-");
            path = path.Replace("setup.reg", "setup_un.reg");
            File.WriteAllText(path, sb.ToString(), Encoding.Unicode);

        }


        static StringBuilder Build(string location, RegisterAttribute tree, RegisterAttribute[] funcs)
        {
            string dirPath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            StringBuilder sb = new StringBuilder();

            if( funcs !=null && funcs.Length!=0)
            {
                sb.AppendLine($"[{location}{tree.Multilevel}]".Replace("/", "\\shell\\"));
                sb.AppendLine($"\"Icon\"=\"{tree.Icon}\"".Replace("\\", "\\\\"));
                sb.AppendLine($"\"SubCommands\"=\"\"");
                sb.AppendLine();

            }
            else
            {
                funcs = new RegisterAttribute[] { tree };
            }
            foreach (RegisterAttribute func in funcs)
            {

                if (!string.IsNullOrWhiteSpace(func.Icon))
                {
                    sb.AppendLine($"[{location}{func.Multilevel}]".Replace("/", "\\shell\\"));
                    sb.AppendLine($"\"Icon\"=\"{func.Icon}\"".Replace("\\", "\\\\"));
                    sb.AppendLine();
                }
                sb.AppendLine($"[{location}{func.Multilevel}\\command]".Replace("/", "\\shell\\"));
                sb.Append($"@=\"{Path.Combine(dirPath, func.BinName)} {func.FuncName}".Replace("\\", "\\\\"));
                foreach (string pa in func.Paraments.Split(' '))
                {
                    if (!string.IsNullOrWhiteSpace(pa))
                    {
                        sb.Append($" \\\"{pa.Trim()}\\\"");
                    }
                }
                sb.AppendLine("\"");
                sb.AppendLine();
            }

            return sb;
        }




    }
}
