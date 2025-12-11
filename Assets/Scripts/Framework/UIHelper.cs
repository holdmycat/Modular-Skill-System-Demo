using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UObject = UnityEngine.Object;

namespace Ebonor.Framework
{
    public static class UIHelper
    {
        
        static List<GameObject> g_ListGO = new List<GameObject>();
        
        public static List<GameObject> OnDynamicLoadItem(GameObject mGOPoolHeroWeaponItem,  GameObject mGOSpawnHeroWeaponItem,
            LayoutGroup mGridLayoutWeaponItem, int ListCount)
        {
            var curWeaponGONum = mGridLayoutWeaponItem.transform.childCount;

            List<GameObject> children = g_ListGO;
            
            children.Clear();
            
            var curListNum = ListCount;
            
            if (curListNum > curWeaponGONum)
            {
                var numInst = curListNum - curWeaponGONum;

                for (var i = 0; i < curWeaponGONum; i++)
                {
                    children.Add(mGridLayoutWeaponItem.transform.GetChild(i).gameObject);
                }
                
                for (var i = 0; i < numInst; i++)
                {
                    GameObject go = null;
                    if (mGOPoolHeroWeaponItem.transform.childCount > 0)
                    {
                        go = mGOPoolHeroWeaponItem.transform.GetChild(0).gameObject;
                        go.SetActive(true);
                    }
                    else
                    {
                        go = GOHelper.InstantiatePrefab(mGOSpawnHeroWeaponItem);
                    }
                    children.Add(go);
                    GOHelper.ResetLocalGameObject(mGridLayoutWeaponItem.gameObject, go, true);
                }
                
                
            }
            else if (curListNum < curWeaponGONum)
            {
                var numRestore = curWeaponGONum - curListNum;

                while (numRestore > 0)
                {
                    var trans = mGridLayoutWeaponItem.transform.GetChild(0);
                    children.Remove(trans.gameObject);
                    GOHelper.ResetLocalGameObject(mGOPoolHeroWeaponItem, trans.gameObject);
                    numRestore--;
                }
                
                for (var i = 0; i < mGridLayoutWeaponItem.transform.childCount; i++)
                {
                    children.Add(mGridLayoutWeaponItem.transform.GetChild(i).gameObject);
                }
            }
            else
            {
                for (var i = 0; i < mGridLayoutWeaponItem.gameObject.transform.childCount; i++)
                {
                    children.Add(mGridLayoutWeaponItem.gameObject.transform.GetChild(i).gameObject);
                }
            }
            return children;
        }
        
        public static void OnSetCanvasState(CanvasGroup canvasGroup, bool on)
        {
            canvasGroup.alpha = on?1f:0f;
            canvasGroup.interactable = on;
            canvasGroup.blocksRaycasts = on;
        }
        
        public static void OnSetCanvasStateV2(CanvasGroup canvasGroup, bool on, bool interactable)
        {
            canvasGroup.alpha = on?1f:0f;
            canvasGroup.interactable = interactable;
            canvasGroup.blocksRaycasts = interactable;
        }
        
        
    }

}