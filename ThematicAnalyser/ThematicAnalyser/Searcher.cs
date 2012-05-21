using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace ThematicAnalyser
{
    class Searcher
    {
        // Искать контекст (контекстный анализ)
        // выходная информация ArrayList (value - IE_Count)
        // элементы отсортированны по степени их значимости для данной тематики
        // range – диапазон анализируемой окрестности
        // screen - порог (в выходном списке будут присутствовать 
        // только те элементы, которые встретились screen и более раз 
        public ArrayList SearchContext(SortedList subject, int range, int screen)
        {
            if (subject == null) return null;
            SortedList candidate = new SortedList();
            foreach (/*InfoElement*/IE_Value ie in subject.Values)
            {
                // Перебираем все инф. потоки проходящие через данный инф.
                // элемент
                for (int i = 0; i < ie.IE.InOut.Count; i++)
                {
                    int startIndex = (int)ie.IE.InOut.GetKey(i);
                    // Проверяем вперед по инф. потоку
                    Element ie_next = ie.IE;
                    for (int index = startIndex; index < startIndex + range; index++)
                    {
                        Element ie_v = ((InOut)ie_next.InOut[index]).OutIE;
                        if (ie_v == null) break;
                        int index_v = index + 1;
                        CandidateTest(ie_v, index_v, subject, candidate);
                        ie_next = ie_v;
                    }
                    // Проверяем назад по инф. потоку
                    Element ie_prev = ie.IE;
                    for (int index = startIndex; index > startIndex - range; index--)
                    {
                        Element ie_v = ((InOut)ie_prev.InOut[index]).InIE;
                        if (ie_v == null) break;
                        int index_v = index - 1;
                        CandidateTest(ie_v, index_v, subject, candidate);
                        ie_prev = ie_v;
                    }
                }
            }
            // Копируем в выходной массив только те инф. элементы,
            // которые встречались screen и более раз
            ArrayList result = new ArrayList();
            for (int i = 0; i < candidate.Count; i++)
            {
                IE_Index ie_index = (IE_Index)candidate.GetByIndex(i);
                if (ie_index.Index.Count >= screen)
                    result.Add(new IE_Count(ie_index.IE, ie_index.Index.Count));
            }
            // Копируем в выходной массив множество первичных инф. элементов
            for (int i = 0; i < subject.Count; i++)
            {
                IE_Value ie = (IE_Value)subject.GetByIndex(i);
                result.Add(new IE_Count(ie.IE, ie.IE.InOut.Count));
            }
            // Соритировка по количеству повторений (по IE_Count.Count)
            //SortArrayList sort = new SortArrayList(result);
            return result;
        }

        private void CandidateTest(Element ie, int index, SortedList subject, SortedList candidate)
        {
            if (subject.Contains(ie.Name)) return;
            if (!candidate.Contains(ie.Name))
            {
                // Если ie еще нет в списке кандидатов
                IE_Index ie_index = new IE_Index(ie);
                ie_index.Index.Add(index, 0);
                candidate.Add(ie.Name, ie_index);
            }
            else
            {
                // Если ie уже существует в списке кандидатов
                IE_Index ie_index = ((IE_Index)candidate[ie.Name]);
                // Проверяем не было ли ie с таким же инексом 
                // (т.е. не встречали ли мы его в данном индексе потока)
                if (!ie_index.Index.Contains(index))
                    ie_index.Index.Add(index, 0);
            }
        }
    }
    // Вспомогательный класс для организации контекстного анализа
    public class IE_Index
    {
        public Element IE;
        public SortedList Index = new SortedList();
        public IE_Index(Element ie)
        {
            IE = ie;
        }
    }
    // Вспомогательный класс для организации хранения найденных
    // информационных элементов вместе с некоторыми величинами 
    // (типа int)
    public class IE_Count
    {
        public Element IE;
        public int Count;
        public IE_Count(Element ie, int count)
        {
            IE = ie;
            Count = count;
        }
    }
}
