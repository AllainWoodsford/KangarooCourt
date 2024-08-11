using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class StsKCCanvasHolder : MonoBehaviour
{
   public static StsKCCanvasHolder holder = null;
   public RectTransform MyRect = null;
   public void Awake(){
    holder = this;
    MyRect = this.GetComponent<RectTransform>();
   }
}
