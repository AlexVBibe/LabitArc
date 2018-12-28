using System.Windows;
using System.Windows.Controls;

namespace Labit.Composition
{
    public class MvvmTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item != null)
            {
                var rootFactory = new FrameworkElementFactory(typeof(CompositeView));
                rootFactory.SetValue(ContentControl.DataContextProperty, item);
                rootFactory.SetValue(CompositeView.ViewModelProperty, item);
                var rootTemplate = new DataTemplate(typeof(CompositeView));
                rootTemplate.VisualTree = rootFactory;
                return rootTemplate;
            }
            return null;
        }
    }
}
