namespace Ebonor.DataCtrl
{
    public interface IInputService : IPlayerInputSource
    {
        void SetInputEnabled(bool enabled);
        void SetSkillsEnabled(bool enabled);
        void SetMovementEnabled(bool enabled);
        void SetUiEnabled(bool enabled);
    }
}
