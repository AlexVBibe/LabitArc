using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;

namespace ProductBacklog.RxExtensions
{
    public static class RxExtensions
    {
        public static IObservable<TSource> OnPropertyChanged<TSource>(this TSource source, string propertyName) where TSource : INotifyPropertyChanged
        {
            return Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                                handler => handler.Invoke,
                                h => source.PropertyChanged += h,
                                h => source.PropertyChanged -= h)
                                .Where(args => args.EventArgs.PropertyName == propertyName)
                            .Select(args => (TSource)args.Sender);
        }

        public static IObservable<TSource> OnPropertyChanged<TSource>(this TSource source, params string[] properties) where TSource : INotifyPropertyChanged
        {
            return properties.Select(property => source.OnPropertyChanged(property))
                             .Merge();
        }

        /// <summary>
        /// Returns an observable sequence of the source any time the <c>PropertyChanged</c> event is raised.
        /// </summary>
        /// <typeparam name="T">The type of the source object. Type must implement <seealso cref="INotifyPropertyChanged"/>.</typeparam>
        /// <param name="source">The object to observe property changes on.</param>
        /// <returns>Returns an observable sequence of the value of the source when ever the <c>PropertyChanged</c> event is raised.</returns>
        public static IObservable<string> OnPropertyChanges<T>(this T source)
            where T : INotifyPropertyChanged
        {
            return Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                                handler => handler.Invoke,
                                h => source.PropertyChanged += h,
                                h => source.PropertyChanged -= h)
                            .Select(args => args.EventArgs.PropertyName);
        }
    }
}
