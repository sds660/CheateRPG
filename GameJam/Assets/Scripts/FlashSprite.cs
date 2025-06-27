using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashSprite : MonoBehaviour
{
    private bool _inProgress = false;
    public void DoFlash(SpriteRenderer sprite, int numTimes, float delay, bool disable = false)
    {
        if (_inProgress) return;

        _inProgress = true;
        StartCoroutine(FlashSpriteCoroutine(sprite, numTimes, delay, disable));
    }

    public bool GetInProgress()
    {
        return _inProgress;
    }
    
    private IEnumerator FlashSpriteCoroutine(SpriteRenderer sprite, int numTimes, float delay, bool disable = false) {
        // number of times to loop
        for (var loop = 0; loop < numTimes; loop++) {            
            // cycle through all sprites
            if (disable) 
            {
                // for disabling
                sprite.enabled = false;
            } else {
                // for changing the alpha
                sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 0.5f);
            }
            
            // delay specified amount
            yield return new WaitForSeconds(delay);
 
            // cycle through all sprites
            if (disable)
            {
                // for disabling
                sprite.enabled = true;
            } else {
                // for changing the alpha
                sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 1);
            }
            
            // delay specified amount
            yield return new WaitForSeconds(delay);
        }
        _inProgress = false;
    }
}