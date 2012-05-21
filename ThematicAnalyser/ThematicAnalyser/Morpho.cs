using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using LEMMATIZERLib;
using AGRAMTABLib;

namespace ThematicAnalyser
{
    class Morpho
    {
        ArrayList al;
        ILemmatizer lemmatizer;
        public ArrayList GetLemmas(string[] words, string[] ignor)
        {
            lemmatizer.LoadDictionariesRegistry();
            foreach (var word in words)
            {
                //finding norm of the word
                ParadigmCollection parCol = lemmatizer.CreateParadigmCollectionFromForm(word, 0, 1);
                if (parCol.Count > 0)
                {
                    al.Add(parCol[0].Norm);
                }
            }
            int[] num = new int[al.Count];
            int i = 0, j = 0;
            foreach (string word in al)
            {
                if (ignor.Contains(word.ToLower()))
                {
                    num[j++] = i;
                }
                i++;
            }
            if ((num[0] == 0 && num[1] != 0) || (num[0] != 0))
            {
                for (i = 0; i < num.Count(); i++)
                {
                    al.RemoveAt(num[i] - i);
                    if (num[i + 1] == 0)
                        break;
                }
            }
            return al;
        }
        public Morpho()
        {
            lemmatizer = new LemmatizerRussian();
            al = new ArrayList();
        }
    }
}