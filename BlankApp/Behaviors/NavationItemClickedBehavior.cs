#region Copyright Syncfusion Inc. 2001-2021.
// Copyright Syncfusion Inc. 2001-2021. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using BlankApp.Service.Model;
using BlankApp.Services;
using BlankApp.ViewModel;
using BlankApp.Views;
using Microsoft.Xaml.Behaviors;
using Prism.Ioc;
using Syncfusion.UI.Xaml.NavigationDrawer;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace BlankApp.Behaviors
{
    public class KV
    {
        public string Number { get; set; }
        public string Path { get; set; }
    }
    public class NavationItemClickedAction : TargetedTriggerAction<Grid>
    {
        private  IProjectService _ps;
        private  IArchiveService _as;


 
        UserControl userControl=null;

        Button button = null;
   

        protected override void Invoke(object parameter)
        {
            var args = parameter as NavigationItemClickedEventArgs;
            if (args == null || args.Item.ItemType != ItemType.Tab)
                return;
            if (button == null)
            {
                button = ((this.Target as Grid).Children[0] as StackPanel).Children[0] as Button;
                button.Click += Button_Click;
            }
            if (userControl == null)
            {
                userControl = (this.Target as Grid).Children[1] as UserControl;
            }


            var pagename = args.Item.Header.ToString();

            this._ps = _ps != null ? _ps : ContainerLocator.Container.Resolve<IProjectService>();
            this._as = _as != null ? _as : ContainerLocator.Container.Resolve<IArchiveService>(); ;

            if (_ps.Contains(pagename))
            {
                /*                string source = "F:\\Destop\\pdf";
                                string[] vs = Directory.GetFiles(source, "*", SearchOption.TopDirectoryOnly).Where( f => f.Contains("〔")).ToArray();
                                Array.ForEach(vs, s => File.Move(s, s.Replace("〔", "[").Replace("〕", "]")));*/
                /*                IArchivesService archivesService = ContainerLocator.Container.Resolve<IArchivesService>();
                                Project project = projectService.Find(pagename);
                                string path = project.Path;
                                string[] vs = Directory.GetFiles(path, "*.txt", SearchOption.AllDirectories).Where(p => Path.GetFileName(p).Contains("协议书")).ToArray();
                                List<KV> kvs = new List<KV>();
                                foreach (string item in vs)
                                {
                                    string key = archivesService.ReadTxtProperty(item, "档案号");
                                    KV kv = new KV();
                                    kv.Number = key;
                                    kv.Path = Directory.GetFiles(Path.GetDirectoryName(item), "*.pdf", SearchOption.TopDirectoryOnly).FirstOrDefault();  
                                    kvs.Add(kv);
                                }
                                string dst = "F:\\Destop\\pdf";
                                foreach (var item in kvs)
                                {
                                    File.Copy(item.Path, Path.Combine( dst, item.Number + ".pdf" ), true );
                                }
                                */


                Filtering filtering = ContainerLocator.Container.Resolve<Filtering>();

                if( _ps.HasArchives(pagename))
                {
                    ObservableCollection<Archive> archives = _ps.GetArchives(pagename);
                    (filtering.DataContext as FilteringViewModel).ArchivesDetails = archives;
                }
                else
                {
                    ObservableCollection<Archive> archives = GenerateArchivesInfoViewModel(pagename);
                    _ps.SetArchives(pagename, archives);
                    (filtering.DataContext as FilteringViewModel).ArchivesDetails = archives;
                }
                

                // 设置当前项目
                button.Content = pagename;

                // 设置控件
                userControl.Content = filtering;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (userControl?.Content is Filtering filtering && button?.Content != null)
            {
                string name = button.Content as string;
                ObservableCollection<Archive> archives = GenerateArchivesInfoViewModel(name);
                _ps.SetArchives(name, archives);
                filtering.DataContext = _ps.GetArchives(name);
            }
        }

        ObservableCollection<Archive> GenerateArchivesInfoViewModel(string name)
        {
            ObservableCollection<Archive> av = new ObservableCollection<Archive>();
            if ( _ps.Contains(name) )
            {
                _ps.RefreshArchivalSourcePaths(name);
                string[] paths = _ps.GetArchivalSourcePaths(name);

                new ProgressBarWindow().Show("读取档案信息", paths.Length, (w, i) =>
                {
                    foreach (string path in paths)
                    {
                        w.ReportProgress(++i);
                        Archive archives = _as.Read(path);
                        av.Add(archives);
                    }
                });
            }
            return av;
        }

    }
}


