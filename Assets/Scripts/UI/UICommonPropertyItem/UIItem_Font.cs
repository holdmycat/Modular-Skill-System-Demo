using TMPro;
using UnityEngine;

namespace Ebonor.UI
{
    public class UIItem_Font : MonoBehaviour
    {
        [SerializeField] private TMP_Text propertyText;

        public void UpdatePropertyText(string text)
        {
            propertyText.text = text;
        }
        
    }
}
