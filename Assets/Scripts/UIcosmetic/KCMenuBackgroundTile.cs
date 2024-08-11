using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class KCMenuBackgroundTile : MonoBehaviour
{
  public Animation MyAnimator;
  public AnimationClip MyAnimation;
  public Image MyRenderer = null;
  Sprite ToChangeSprite = null;

  public void PlayAnimationThenSetSprite(Sprite sprite){
    ToChangeSprite = sprite;
     MyAnimator.enabled = true;
     MyAnimator.clip = MyAnimation;
     MyAnimator.Play();
    StartCoroutine(AwaitAnimation());
   
  }

  IEnumerator AwaitAnimation(){
    yield return new WaitForSeconds(.5f);
      MyRenderer.sprite = ToChangeSprite;
     yield return new WaitForSeconds(.5f);
    MyAnimator.Stop();
    MyAnimator.enabled = false;
  
  }
}
