using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GnomeCrawler
{
    public class RemoverFromParent : MonoBehaviour
    {

        public void DisconnectFromParent()
        {
            transform.parent = null;
        }
    }
}
