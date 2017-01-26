using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEditor;

public class UGUItoNGUI : MonoBehaviour
{
    [MenuItem("UGUItoNGUI/Change")]
    public static void ChangeUGUItoNGUI()
    {
        ChangeImageToUISprite("test01");
        ChangeButtonToUIButton();
        ChangeLabelToUILabel();
        DeleteUGUIComponent();
        //ChangeRectTransformToTransform();
    }

    private static void ChangeImageToUISprite(string atlasName)
    {
        Image[] imageObjs = GameObject.FindObjectsOfType<Image>();
        foreach (Image img in imageObjs)
        {
            UISprite sprite = img.gameObject.GetComponent<UISprite>();
            if (sprite == null)
            {
                sprite = img.gameObject.AddComponent<UISprite>();
            }

            if (img.sprite != null)
            {
                //Set Atlas
                string atlasPath = "UI/atlas/" + atlasName;
                GameObject atlasGO = Resources.Load(atlasPath) as GameObject;
                sprite.atlas = atlasGO.GetComponent<UIAtlas>();
                sprite.spriteName = img.sprite.name;
            }

            //Set Sprite Size
            RectTransform rectTransform = img.gameObject.GetComponent<RectTransform>();
            sprite.width = Mathf.RoundToInt(rectTransform.sizeDelta.x);
            sprite.height = Mathf.RoundToInt(rectTransform.sizeDelta.y);
            DestroyImmediate(img);
        }
    }

    private static void ChangeButtonToUIButton()
    {
        Button[] buttonObjs = GameObject.FindObjectsOfType<Button>();
        foreach (Button btn in buttonObjs)
        {
            UISprite sprite = btn.gameObject.GetComponent<UISprite>();
            if (sprite == null)
            {
                Debug.Log("Chang Button : ["+ sprite.gameObject.name+"] Has No UISprite!!");
                continue;
            }
            UIButton button = btn.gameObject.GetComponent<UIButton>();
            if (button == null)
            {
                button = btn.gameObject.AddComponent<UIButton>();
            }
           
            button.tweenTarget = button.gameObject;
            button.normalSprite = sprite.spriteName;
            DestroyImmediate(btn);
        }
    }

    private static void DeleteUGUIComponent()
    {
        Component[] allObjs = GameObject.FindObjectsOfType<Component>();
        foreach (Component obj in allObjs)
        {
            if (obj.gameObject.layer != 5)  //"UI"
                continue;

            if (obj.GetType().Name.Contains("Canvas"))
                DestroyImmediate(obj);
            /*
            Component[] components = obj.gameObject.GetComponents<Component>();
            foreach (Component cps in components)
            {
               
            }*/
        }
    }
    private static void ChangeLabelToUILabel()
    {
        Text[] textObjs = GameObject.FindObjectsOfType<Text>();
        foreach (Text text in textObjs)
        {
            UILabel label = text.gameObject.GetComponent<UILabel>();
            if (label == null)
            {
                label = text.gameObject.AddComponent<UILabel>();
            }

            if (text.font != null)
            {
                //Set Font
                label.text = text.text;
                label.trueTypeFont = text.font;
                label.fontSize = text.fontSize;
                label.fontStyle = text.fontStyle;
            }

            ChangeFontBehavior cfb = text.GetComponent<ChangeFontBehavior>();
            if (cfb != null)
            {
                DestroyImmediate(cfb);
            }

            DestroyImmediate(text);
        }
    }
    [MenuItem("UGUItoNGUI/Test")]
    private static void Test()
    {
        GameObject go = GameObject.Find("RankB");
        Image image = go.GetComponent<Image>();
        //Debug.Log("image.flexibleWidth = "+ image.flexibleWidth+ "image.minWidth = " + image.minWidth + "image.preferredWidth = " + image.preferredWidth);
        RectTransform rect = go.GetComponent<RectTransform>();
        Debug.Log("rect.sizeDelta = " + rect.sizeDelta + " rect.rectangle = " + rect.rect.ToString());
    }
}
