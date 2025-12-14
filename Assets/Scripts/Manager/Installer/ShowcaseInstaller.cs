using System.Collections;
using System.Collections.Generic;
using Ebonor.Framework;
using UnityEngine;
using Zenject;

namespace Ebonor.Manager
{
    public class ShowcaseInstaller : MonoInstaller
    {
        
        private static readonly ILog log = LogManager.GetLogger(typeof(ShowcaseInstaller));
        
        public override void InstallBindings()
        {
            log.Debug("[ShowcaseInstaller] InstallBindings called.");
            Container.BindInterfacesAndSelfTo<ShowcaseSceneManager>().AsSingle().NonLazy();
        }
    }
}
