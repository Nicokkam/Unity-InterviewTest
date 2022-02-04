using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class SearchSentenceWindow : EditorWindow
{
  public List<ParsingCodes> parsingCodesList;
  string stringInput = "";
  string stringOutput = "";

  [MenuItem("Window/Search sentence")]
  public static void ShowWindow()
  {
    GetWindow<SearchSentenceWindow>("Search sentence");
  }

  void OnGUI()
  {
    //Window Title
    EditorGUI.DropShadowLabel(new Rect(0, 0, position.width, 20),
            "Check if sentence is part of any parsing code!");

    //Window Input Field
    stringInput = EditorGUI.TextField(new Rect(10, 25, position.width - 20, 20),
            "New Names:",
            stringInput);

    //Window Button
    if (GUI.Button(new Rect(0, 50, position.width, 30), "Search Sentence!"))
      Check();

    //Window Output Label
    EditorGUI.SelectableLabel(new Rect(10, 85, position.width, 60),
                stringOutput);
  }

  void Check()
  {
    GameObject SearchGameObj = GameObject.FindWithTag("Search Sentence");
    PerformSearch PerformSearchCS = SearchGameObj.GetComponent<PerformSearch>();

    stringOutput = PerformSearchCS.OutputLogic(stringInput);
  }

}