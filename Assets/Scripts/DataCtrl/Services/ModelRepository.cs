using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Ebonor.Framework;
using UnityEngine;

namespace Ebonor.DataCtrl
{
    public class ModelRepository : IModelRepository
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ModelRepository));
        
        public UniTask SaveModelAsync(IList<Object> list)
        {
            log.Debug("[ModelRepository], Starting save model");
            //throw new System.NotImplementedException();

            return UniTask.CompletedTask;
        }
    }
}

