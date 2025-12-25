namespace Ebonor.DataCtrl
{
    
    public interface ISceneResourceManager
    {
        GameSceneResource GetSceneResource();

        CommanderBootstrapInfo GetCommanderBootstrapInfo();
        CommanderBootstrapInfo GetPlayerCommanderBootstrapInfo();
        
    }
}

