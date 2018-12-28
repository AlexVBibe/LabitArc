namespace Labit.Interfaces
{
    public interface ICompositeViewRegistry
    {
        void RegistorView(object view);

        void UnregisterView(object view);

        object[] Views { get; }
    }
}
