using BlankApp.Service;
using BlankApp.Service.Impl;
using NTwain;
using System;
using Unity;

namespace BlankApp.Cli
{
    public class Program
    {
        private static readonly IUnityContainer _container = new UnityContainer();
        private static TwainSession _session;

        static void Main(string[] args)
        {

            LoadContainers();
            Util util = _container.Resolve<Util>();

            object[] objs = new object[args.Length - 1];
            for (int i = 1; i < args.Length; i++)
            {
                objs[i - 1] = args[i];
            }
            typeof(Util).GetMethod(args[0]).Invoke(util, objs);

            /*    var appId = TWIdentity.CreateFromAssembly(DataGroups.Image | DataGroups.Audio, Assembly.GetEntryAssembly());
                _session = new TwainSession(appId);
                _session.TransferError += _session_TransferError;
                _session.TransferReady += _session_TransferReady;
                _session.DataTransferred += _session_DataTransferred;
                _session.SourceDisabled += _session_SourceDisabled;
                _session.StateChanged += _session_StateChanged;
                _session.Open();

                DataSource currentSource = _session.CurrentSource;*/
        }

        private static void _session_StateChanged(object sender, EventArgs e)
        {
            ;
        }

        private static void _session_SourceDisabled(object sender, EventArgs e)
        {
            ;
        }

        private static void _session_DataTransferred(object sender, DataTransferredEventArgs e)
        {
            ;
        }

        private static void _session_TransferReady(object sender, TransferReadyEventArgs e)
        {
            ;
        }

        private static void _session_TransferError(object sender, TransferErrorEventArgs e)
        {
            ;
        }

        static void LoadContainers()
        {
            _container.RegisterSingleton<IConfigurationService, ConfigurationService>();
            _container.RegisterSingleton<IMaskService, MaskService>();
            _container.RegisterSingleton<IArchiveService, ArchiveService>();
            _container.RegisterSingleton<IArticleService, RawArticleService>();
            _container.RegisterSingleton<IWordService, WordService>();
            _container.RegisterSingleton<Util>();
        }

    }
}
