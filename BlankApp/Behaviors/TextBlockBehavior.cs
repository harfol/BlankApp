using BlankApp.Services;
using Microsoft.Xaml.Behaviors;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace BlankApp.Behaviors
{
    public class TextBlockBehavior: Behavior<TextBlock>
    {
        protected override void OnAttached()
        {
            AssociatedObject.Loaded += AssociatedObject_Loaded;
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            AssociatedObject.DataContextChanged += AssociatedObject_DataContextChanged;
        }

        private void AssociatedObject_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is TextBlock txt && !string.IsNullOrWhiteSpace(txt.DataContext as string))
            {
                string src = txt.DataContext as string;
                IMaskService maskService = ContainerLocator.Container.Resolve<IMaskService>();
                string mask = maskService.Simplify(src);
                if( mask != null)
                {
                    int i = src.IndexOf(mask);
                    string start = src.Substring(0, i + 1);
                    string end = src.Substring(i + mask.Length + 1);
                    txt.Inlines.Add(mask);
                    Run run = new Run();
                    run.Text = mask;
                    run.Foreground = Brushes.Red;
                    txt.Inlines.Add(run);
                    txt.Inlines.Add(end);
                }
                else
                {
                    txt.Text = src;
                }
 
                
            }
        }

        private void AssociatedObject_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
