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
        ILemmatizer lemmatizerRus;
        /// <summary>
        /// Get norm form of the word.
        /// </summary>
        /// <param name="words">Words after lexical analyse</param>
        /// <param name="ignor">List of stop-words</param>
        /// <returns></returns>
        public ArrayList GetLemmas(IEnumerable<string> words, string[] ignor)
        {
            lemmatizerRus.LoadDictionariesRegistry();
            foreach (var word in words)
            {
                ParadigmCollection parCol = null;
                //finding norm of the word
                //if (word.IndexOfAny("abcdefghijklmnopqrstuvwxyz".ToCharArray()) == -1)
                //{
                    parCol = lemmatizerRus.CreateParadigmCollectionFromForm(word, 0, 1);
                //}
                //else
                //{
                //    parCol = lemmatizerEng.CreateParadigmCollectionFromForm(word, 0, 1);
                //}
                if (parCol.Count > 0)
                {
                    al.Add(parCol[0].Norm.ToLower());
                }
            }
            int[] num = new int[al.Count];
            int i = 0, j = 0;
            foreach (string word in al)
            {
                if (ignor.Contains(word))
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
            lemmatizerRus = new LemmatizerRussian();
            al = new ArrayList();
        }
    }
}