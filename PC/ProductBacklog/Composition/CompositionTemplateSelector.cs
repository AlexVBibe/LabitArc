using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Labit.Composition
{
    public class CompositionTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item != null)
            {
                var type = item.GetType();
                var attributes = type.GetCustomAttributes(true);
                var viewAttribute = attributes.OfType<ViewAttribute>().FirstOrDefault();
                if (viewAttribute != null)
                {
                    var typeName = viewAttribute.TypeName;

                    Type viewType = AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(x => x.GetTypes())
                        .FirstOrDefault(x => x.Name == typeName);

                    var factory = new FrameworkElementFactory(viewType);
                    factory.SetValue(ContentControl.DataContextProperty, item);

                    var dataTemplate = new DataTemplate(viewType);
                    dataTemplate.VisualTree = factory;
                    return dataTemplate;
                }
            }

            return null;
        }
    }
}
