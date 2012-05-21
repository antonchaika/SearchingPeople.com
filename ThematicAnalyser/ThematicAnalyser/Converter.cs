using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;

namespace ThematicAnalyser
{
    class Converter
    {
        // text - текст, который необходимо преобразовать в инф. структуру
        // allIE - список всех существующих инф. элементов
        // ignor - список игнорируемых слов
        public Converter(string text, SortedList allIE, ref int index)
        {
            ArrayList al = ConvertToArray(text);
            if (al.Count < 2) return;

            Element ie_prev = null;
            Element ie = CreateIE((string)al[0], allIE);
            Element ie_next = CreateIE((string)al[1], allIE);

            ie.InOut.Add(index++, new InOut(ie_prev, ie_next));

            for (int i = 1; i < al.Count - 2; i++)
            {
                ie_prev = ie;
                ie = ie_next;
                ie_next = CreateIE((string)al[i + 1], allIE);

                ie.InOut.Add(index++, new InOut(ie_prev, ie_next));
            }
            // Последний инф. эл. в тексте
            ie_next.InOut.Add(index++, new InOut(ie, null));
        }

        public ArrayList ConvertToArray(string text)
        {
            //string[] list = text.Split(new char[]{',','!','?',' ','.',';',':','%','"','-'}, StringSplitOptions.RemoveEmptyEntries);
            string[] source = text.Split(new char[] { '.', '?', '!', ' ', ';', ':', ',', '-', '#', '@', '&', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            var dist = from word in source
                       where word.IndexOfAny("1234567890!@#$%^&*()\\|+~`-/-+_ ".ToCharArray()) == -1
                       select word;
            string[] ignor = GetIgnorList("stop-words");
            if (ignor != null && source != null)
            {
                Morpho morpho = new Morpho();
                ArrayList ar = morpho.GetLemmas(source, ignor);
                return ar;
            }
            else return null;
        }

        private string[] GetIgnorList(string filename)
        {
            using (StreamReader sr = new StreamReader(filename + ".txt"))
            {
                if (sr != null)
                {
                    string s = sr.ReadToEnd();
                    string[] ignor = s.Split(new char[]{}, StringSplitOptions.RemoveEmptyEntries);
                    return ignor;
                }
                else return null;
            }
        }
        // name - имя инф. элемента
        // listIE - список сущ. инф. элементов
        private Element CreateIE(string name, SortedList allIE)
        {
            Element ie = null;
            if (!allIE.Contains(name))
            {
                ie = new Element(name);
                allIE.Add(name, ie);
            }
            else ie = (Element)allIE[name];
            return ie;
        }
    }
}
