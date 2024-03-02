using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace GnomeCrawler
{
    public class TextUpdater : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;

        public void SetText(float progress)
        {
            _text.SetText($"{(progress * 100).ToString("N2")}%");
        }
    }
}
