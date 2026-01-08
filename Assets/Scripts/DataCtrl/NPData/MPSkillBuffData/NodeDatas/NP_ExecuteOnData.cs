namespace Ebonor.DataCtrl
{
    /// <summary>
    /// Optional execution scope for task nodes.
    /// </summary>
    public interface INPExecuteOnData
    {
        eMPNetPosition ExecuteOn { get; set; }
    }
}
