using Labit.Composition;

namespace Labit
{
    [View("HostWindow")]
    public class HostWindowViewModel : BaseViewModel, IHostWindowViewModel
    {
        private object content;

        public object Content
        {
            get
            {
                return content;
            }
            set
            {
                content = value;
                this.OnPropertyChanged(() => Content);
            }
        }
    }
}
