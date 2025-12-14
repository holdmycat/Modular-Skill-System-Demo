//------------------------------------------------------------
// File: ShowCaseSceneManager.cs
// Created: 2025-12-06
// Purpose: Data-driven scene manager for the ShowcaseScene sandbox.
// Author: Xuefei Zhao (clashancients@gmail.com)
//------------------------------------------------------------
using System;
using Cysharp.Threading.Tasks;
using Ebonor.DataCtrl;
using Ebonor.Framework;
using Ebonor.UI;
using Zenject;

namespace Ebonor.Manager
{
    
    public class ShowcaseSceneManager : ISceneManager, IDisposable
    {
        
        private static readonly ILog log = LogManager.GetLogger(typeof(ShowcaseSceneManager));
        
        private readonly IUIService _uiService;
        private readonly ICharacterDataRepository _dataRepo;
        private readonly IRoomManagerService _roomManagerService;
        public ShowcaseSceneManager(IRoomManagerService roomManagerService, IUIService uiService, ICharacterDataRepository dataRepo)
        {
            log.Debug("[ShowcaseSceneManager] Starting Construction");
            _uiService = uiService;
            _dataRepo = dataRepo;
            _roomManagerService = roomManagerService;
        }
        
        public void Dispose()
        {
            log.Debug("[ShowcaseSceneManager] Starting Dispose");
        }

        public async UniTask StartupSequence()
        {
            await LoadRoom();
        }
        
        private async UniTask LoadRoom()
        {
            log.Debug("[ShowcaseSceneManager] Starting LoadRoom");

            await _roomManagerService.CreateRoomAndAddPlayer();
            
        }
        
    }
    

}
