using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameFramework;

public class UI_GMTool : NGUIChildGUI
{
	private const string GUI_SMARTOBJECT_NAME = "m_UIGMTool";

	public UIInput input;
	public UITextList textList;
	public bool fillWithDummyData = false;
	bool IgnoreNextEnter = false;
	List<string>	history = new List<string>();
	int index = -1;

	private UI_GMTool()
		: base(GUI_SMARTOBJECT_NAME)
	{
	}

	// Use this for initialization
	void Start ()
	{
		if (fillWithDummyData && textList != null)
		{
			for (int i = 0; i < 30; ++i)
			{
				textList.Add(((i % 2 == 0) ? "[FFFFFF]" : "[AAAAAA]") +
				             "This is an example paragraph for the text list, testing line " + i + "[-]");
			}
		}
		EventDelegate.Add(input.onSubmit, OnSubmit);

	}
	
	// Update is called once per frame
	void Update ()
	{
		// control + g 啟動GM UI
		if (textList.gameObject.activeSelf)
		{
			if (Input.GetKeyUp(KeyCode.Return))
			{
				if (!IgnoreNextEnter && !input.isSelected)
				{
					input.isSelected = true;
				}
				IgnoreNextEnter = false;
			}
		}

		if (Input.GetKey(KeyCode.LeftControl))
		{
			if (Input.GetKeyDown(KeyCode.UpArrow))
			{
				if (0 <= index)
				{
					if (index < history.Count)
					{
						string cmd = history[history.Count - index -1];
						index++;
						input.value = cmd;
					}
				}
			}
			else if (Input.GetKeyDown(KeyCode.DownArrow))
			{
				if (0 <= index)
				{
					if (index < history.Count)
					{
						string cmd = history[history.Count - index -1];
						index--;
						input.value = cmd;
					}
				}
			}

			if (history.Count > 0)
				index = Mathf.Clamp(index, 0 , history.Count - 1);
			else
				index = -1;

		}

		if (Input.GetKey(KeyCode.Escape))
		{
			Hide();
		}
	}

	void OnSubmit ()
	{
		if (textList != null)
		{
			// It's a good idea to strip out all symbols as we don't want user input to alter colors, add new lines, etc
			string text = input.value;
			if (!string.IsNullOrEmpty(text))
			{
				string sp = " ";
				string[] strFunc = text.Split(sp.ToCharArray());
				ClientCommand.RunCommand( text);
				//textList.Add(text);
				input.value = "";
				input.isSelected = false;
				history.Add(text);
				index = 0;
			}
		}
		IgnoreNextEnter = true;
	}

	public void AddSystemMsgtoList(string SystemMsg)
	{
		textList.Add(SystemMsg);
	}

	public void OnEnable()
	{
		ClientCommand.doDisplayMessage += AddSystemMsgtoList;
		input.isSelected = true;
	}

	public void OnDisable()
	{
		ClientCommand.doDisplayMessage -= AddSystemMsgtoList;
		input.isSelected = false;
	}

}
