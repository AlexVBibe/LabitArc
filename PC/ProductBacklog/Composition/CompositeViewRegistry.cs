using Labit.Interfaces;
using System.Collections.Generic;

namespace Labit.Composition
{
    class CompositeViewRegistry : ICompositeViewRegistry
    {
        private List<object> views = new List<object>();

        public void RegistorView(object view)
        {
            views.Add(view);
        }

        public void UnregisterView(object view)
        {
            views.Remove(view);
        }

        public object[] Views
        {
            get
            {
                return views.ToArray();
            }
        }
    }
}
