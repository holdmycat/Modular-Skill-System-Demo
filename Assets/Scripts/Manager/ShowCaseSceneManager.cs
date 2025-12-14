//------------------------------------------------------------
// File: ShowCaseSceneManager.cs
// Created: 2025-12-06
// Purpose: Data-driven scene manager for the ShowcaseScene sandbox.
// Author: Xuefei Zhao (clashancients@gmail.com)
//------------------------------------------------------------
using System;
using Ebonor.DataCtrl;
using Ebonor.Framework;
using Ebonor.UI;
using Zenject;

namespace Ebonor.Manager
{
    
    public class ShowcaseSceneManager : IInitializable, IDisposable
    {
        
        private static readonly ILog log = LogManager.GetLogger(typeof(ShowcaseSceneManager));
        
        readonly IUIService _uiService;
        readonly ICharacterDataRepository _dataRepo;
    
        // 构造函数注入
        public ShowcaseSceneManager(IUIService uiService, ICharacterDataRepository dataRepo)
        {
            log.Debug("[ShowcaseSceneManager] Starting Construction");
            _uiService = uiService;
            _dataRepo = dataRepo;

        }

        public void Initialize() 
        {
            
            log.Debug("[ShowcaseSceneManager] Starting Initialize");
            // 这里写原来的 OnEnter 逻辑
            // LoadRoom();
            // LoadPlayer();
            // OpenUI();
        }

        public void Dispose()
        {
            // 这里写原来的 OnExit 逻辑
            log.Debug("[ShowcaseSceneManager] Starting Dispose");
        }
    }
    

}
