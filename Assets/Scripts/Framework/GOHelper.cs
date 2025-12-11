using UnityEngine;
using UObject = UnityEngine.Object;

namespace Ebonor.Framework
{
    public static class GOHelper
    {
      
        public static T InstantiatePrefab<T>(string path) where T: UnityEngine.Object
        {
            var objPrefab = UnityEngine.Resources.Load(path);
            if (null == objPrefab)
            {
                Debug.LogWarningFormat("Fail to find player, path:{0}", path);
                return null;
            }
            var SkillPrefab = UnityEngine.Object.Instantiate(objPrefab) as T;
            SkillPrefab.name = objPrefab.name;
            return SkillPrefab;
        }
        
        public static T InstantiatePrefab<T>(T obj) where T : UnityEngine.Object
        {
            var skillPrefab = UnityEngine.Object.Instantiate(obj) as T;
            skillPrefab.name = obj.name;
            return skillPrefab;
        }

        public static GameObject InstantiateGOPrefab(UnityEngine.Object obj)
        {
            if (null == obj)
            {
#if UNITY_EDITOR
                Debug.LogErrorFormat("obj is null");
                return null;
#endif
            }
            var skillPrefab = UnityEngine.Object.Instantiate(obj) as GameObject;
            ResetGameObject(skillPrefab);
            skillPrefab.name = obj.name;
            return skillPrefab;
        }
        
        public static GameObject InstantiateGOPrefab(GameObject obj)
        {
            var skillPrefab = UnityEngine.Object.Instantiate(obj) as GameObject;
            ResetGameObject(skillPrefab);
            skillPrefab.name = obj.name;
            return skillPrefab;
        }
        
        public static void ResetGameObject(GameObject go, float scale = 1f)
        {
            go.transform.position = Vector3.zero;
            go.transform.rotation = Quaternion.identity;
            go.transform.localScale = Vector3.one * scale;
        }
        
        public static void ResetLocalGameObject(GameObject parent, GameObject go, bool active = false, float scale = 1f)
        {
            go.transform.SetParent(parent.transform);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = Vector3.one * scale;
            go.SetActive(active);
        }
        
    }

}