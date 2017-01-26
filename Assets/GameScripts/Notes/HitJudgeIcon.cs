using UnityEngine;
using System.Collections;

public class HitJudgeIcon : MonoBehaviour
{
    public Animator IconAnimator;
    public UITexture textureJudge;

    // Use this for initialization
    void Start()
    {
    }
	
    // Update is called once per frame
    void Update()
    {
    }

    public void PlayJudgeIcon()
    {
        IconAnimator.Play("Effect");
        StartCoroutine(SelfDestroy());
    }

    public IEnumerator SelfDestroy()
    {
        yield return new WaitForSeconds(1.0f);
        Destroy(gameObject);
        yield return null;
    }
}
