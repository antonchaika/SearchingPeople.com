using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace ThematicAnalyser
{
    public class IE_Value
    {
        public Element IE;
        public double Value;
        public IE_Value(Element ie, double val)
        {
            IE = ie;
            Value = val;
        }
    }
    class Analyzer
    {
        private SortedList allIE = new SortedList();
        /// <summary>
        /// Извлекает ключевые слова из текста с учетом контекста и количества повторений.
        /// </summary>
        /// <param name="threshold">Порог в процентах, необходимый для определения первичного множества ключевых элементов.</param>
        /// <param name="range">Окрестность, в пределах которой осуществляется контекстный анализ.</param>
        /// <param name="screen">Параметр отсечки, используется для исключения из множества ключевых элементов тех элементов, количество повторений которых меньше или равно screen.</param>
        /// <returns>SortedList</returns>
        public SortedList GetKey(int threshold, int range, int screen)
        {
            ArrayList key = GetPrimaryKey(threshold);
            key = GetExpandPrimary(key, range, screen);
            SortedList key_sl = ConvertToSortedList(key);
            Reduction(key_sl);
            return key_sl;
        }

        private ArrayList GetPrimaryKey(int threshold)
        {
            int max = GetMaxInOut();
            int val = (int)((max / 100.0) * threshold);
            ArrayList result = new ArrayList();
            foreach (Element ie in allIE.Values)
            {
                if (ie.InOut.Count >= val)
                {
                    result.Add(new IE_Count(ie, ie.InOut.Count));
                }
            }
            return result;
        }
        private int GetMaxInOut()
        {
            int max = 0;
            foreach (Element ie in allIE.Values)
            {
                if (max < ie.InOut.Count) max = ie.InOut.Count;
            }
            return max;
        }

        private ArrayList GetExpandPrimary(ArrayList primary, int range, int screen)
        {
            Searcher search = new Searcher();
            ArrayList result = search.SearchContext(ConvertToSortedList(primary), range, screen);
            return result;
        }

        static private SortedList ConvertToSortedList(ArrayList al)
        {
            if (al == null) return null;
            SortedList result = new SortedList();
            foreach (IE_Count ie_count in al)
            {
                result.Add(ie_count.IE.Name, new IE_Value(ie_count.IE, ie_count.Count));
            }
            return result;
        }

        static private void Reduction(SortedList sl)
        {
            // общая сумма
            double sum = 0;
            foreach (IE_Value ie_value in sl.Values) sum += ie_value.Value;
            foreach (IE_Value ie_value in sl.Values)
            {
                double k = ie_value.Value / sum;
                ie_value.Value = k;
            }
        }
    }
}