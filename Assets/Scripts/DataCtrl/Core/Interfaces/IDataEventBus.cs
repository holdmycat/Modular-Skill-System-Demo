namespace Ebonor.DataCtrl
{
    public interface IDataEventBus
    {
        void OnAttach<T>(System.Action<T> del) where T : struct;

        void OnDetach<T>(System.Action<T> del) where T : struct;

        void OnValueChange<T>(T parameter) where T : struct;

        void OnClearAllDicDELEvents();
    }

}
