using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
public class KCMenuBackgroundGrid : MonoBehaviour
{
    public List<KCMenuBackgroundTile> Tiles = new List<KCMenuBackgroundTile>();
    public List<Sprite> backgrounds = new List<Sprite>();
    public bool RoutineIsRunning = false;
    public List<int> imageID = new List<int>();
    public List<int> tileID = new List<int>();
    // Start is called before the first frame update
    public void Start()
    {
        initializeRandomBG();
        intializeTileID();
        initializeImageID();
        StartCoroutine(PickAnImageToFade());
    }

    private void initializeRandomBG(){
        List<Sprite> tempBG = new List<Sprite>();
        for(int i =0; i < backgrounds.Count ; i ++){
            tempBG.Add(backgrounds[i]);
        }

        for(int i = 0; i < Tiles.Count ; i++){
            try{
                int rand = Random.Range(0,tempBG.Count);
                Tiles[i].MyRenderer.sprite = tempBG[rand];
                tempBG.RemoveAt(rand);
            }catch{
                continue;
            }
            
        }
    }

    private void intializeTileID(){
        for(int i =0; i < Tiles.Count ; i++){
            tileID.Add(i);
        }
    }

    private void initializeImageID(){
        for(int i = 0; i< backgrounds.Count ;i++){
            imageID.Add(i);
        }
    }

    IEnumerator PickAnImageToFade(){
        RoutineIsRunning = true;
       yield return new WaitForSeconds(.75f);
         RoutineIsRunning = false;
         try{
            int r =Random.Range(0,imageID.Count);
            int rand = imageID[r];
            int p = Random.Range(0,tileID.Count);
         int randomTile =  tileID[p];
         Tiles[randomTile].PlayAnimationThenSetSprite(backgrounds[rand]);
       
         imageID.RemoveAt(r);
           tileID.RemoveAt(p);
         if(!tileID.Any()){
              intializeTileID();
       
         }
         if(!imageID.Any()){
             initializeImageID();
         }
          StartCoroutine(PickAnImageToFade());
         }catch{
                if(!tileID.Any()){
              intializeTileID();
       
         }
         if(!imageID.Any()){
             initializeImageID();
         }
                StartCoroutine(PickAnImageToFade());
         }
        
     
         
    }

  
}
