using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;

public class PerformSearch : MonoBehaviour
{

  public List<ParsingCodes> parsingCodesList;

  public string OutputLogic(string input)
  {
    string output = "";
    List<string> includeStrings = new List<string>(); //First List -> Words with No Variants
    List<(string, string)> includePairStrings = new List<(string, string)>(); //Second List -> Words with Variants
    List<bool> listOks = new List<bool>(); //Final Check

    if (parsingCodesList.Count > 0)
    {
      OrderListByPriority();

      foreach (var item in parsingCodesList) //Loop of Scriptable Objects
      {
        int howManyAnds = item.content.Count(i => i == '&'); //How many words
        string copyOfContent = item.content;

        if (howManyAnds == 0)
          break;

        for (int i = 1; i <= howManyAnds; i++) //Isolate the words
        {
          int subsStart = 0;
          int subsEnd = 0;
          string firstWord = "";

          subsStart = copyOfContent.IndexOf('&');
          subsEnd = copyOfContent.IndexOf('&', subsStart + 1);

          if (subsEnd == -1)
            subsEnd = copyOfContent.Length - 1;

          firstWord = copyOfContent.Substring(subsStart + 1, subsEnd); //Using Subs
          copyOfContent = copyOfContent.Remove(subsStart, subsEnd);

          firstWord = RemoveSymbols(firstWord); //& and " "

          CheckIfHasVariants(includeStrings, includePairStrings, firstWord);
        }
        // LogFinalLists(includeStrings, includePairStrings);

        ComparisonConditions(input, includeStrings, includePairStrings, listOks); //All info of SO, compare

        if (listOks.Count(i => i == true) == includeStrings.Count() + includePairStrings.Count())
        {
          listOks = new List<bool>();
          output = $" Title: {item.name} \n Content: {item.content} \n Priority {item.priority}";
          break; //Get out to prevent useless calculations
        }
        else
          Debug.Log("ERROR!");

        if (item == parsingCodesList.Last()) //If not a match was found, return this!
          output = "Nothing was found!";
      }
    }

    return output; //Return the result!
  }

  private void OrderListByPriority()
  {
    parsingCodesList = parsingCodesList
          .OrderByDescending(i => i.priority)
          .ToList();
  }

  private static string RemoveSymbols(string firstWord)
  {
    var charsToRemove = new string[] { "&", " " };
    foreach (var c in charsToRemove)
      firstWord = firstWord.Replace(c, string.Empty);

    return firstWord;
  }

  private static void CheckIfHasVariants(List<string> includeStrings, List<(string, string)> includePairStrings, string firstWord)
  {
    if (firstWord.Contains('[') == false) //Doesn't have a Variant
      includeStrings.Add(firstWord); //First List (No variants)
    else
    {
      int bracketStart = firstWord.IndexOf('[');
      int bracketEnd = firstWord.IndexOf(']');
      int diff = (bracketEnd - bracketStart) - 1;
      string firstPartOfWord = firstWord.Substring(0, bracketStart);

      if (firstWord.Contains('/') == true) //Check if "/" -> Strawberr [y/ies]
      {
        int slashStart = firstWord.IndexOf('/');
        int diffFirstHalf = (slashStart - bracketStart) - 1;
        string firstHalf = firstWord.Substring(bracketStart + 1, diffFirstHalf);

        int diffSecondHalf = (bracketEnd - slashStart) - 1;
        string secondHalf = firstWord.Substring(slashStart + 1, diffSecondHalf);

        includePairStrings.Add((string.Concat(firstPartOfWord, firstHalf), string.Concat(firstPartOfWord, secondHalf))); //Second List -> Item1 and Item2
      }
      else //Just Plural -> Banana [s]
      {
        string variant = firstWord.Substring(bracketStart + 1, diff);
        includePairStrings.Add((firstPartOfWord, string.Concat(firstPartOfWord, variant))); //Second List -> Item1 and Item2
      }
    }
  }

  // private static void LogFinalLists(List<string> includeStrings, List<(string, string)> includePairStrings)
  // {
  //   Debug.Log($"includeStrings: {includeStrings[0]} | Size: {includeStrings.Count}");
  //   Debug.Log($"Count: {includePairStrings.Count} | Item1: {includePairStrings[0].Item1} | Item2: {includePairStrings[0].Item2}");
  // }

  private static void ComparisonConditions(string input, List<string> includeStrings, List<(string, string)> includePairStrings, List<bool> listOks)
  {
    foreach (var j in includeStrings) //Checks if the No Variant words are included in the input
    {
      if (input.Contains(j) == false) //Not -> add false
        listOks.Add(false);
      else
        listOks.Add(true); //Yes -> add true
    }

    foreach (var z in includePairStrings)
    {
      if (input.Contains(z.Item1) == false && input.Contains(z.Item2) == false)
        listOks.Add(false);
      else
        listOks.Add(true); //If at least one of the variants are included, add true
    }
  }

}
