using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace ThematicAnalyser
{
    // Вспомогательный класс для организации хранения найденных
    // информационных элементов вместе с некоторыми величинами 
    // (типа double)
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
        public int[] param_threshold = new int[] { 90, 80, 70, 60, 50, 40, 30, 20, 10 };
        public int[] param_range = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
        // Получить общий набор ключевых элементов, с нормировкой по 1
        public SortedList GetKey(int threshold, int range, int screen)
        {
            ArrayList key = GetPrimaryKey(threshold);
            key = GetExpandPrimary(key, range, screen);
            SortedList key_sl = ConvertToSortedList(key);
            Reduction(key_sl);
            return key_sl;
        }

        // Получить набор первичных ключевых элементов
        // threshold - порог 
        public ArrayList GetPrimaryKey(int threshold)
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
            //SortArrayList sort = new SortArrayList(result);
            return result;
        }

        // Максимальное количество пар in-out, для отдельного инф. элемента
        private int GetMaxInOut()
        {
            int max = 0;
            foreach (Element ie in allIE.Values)
            {
                if (max < ie.InOut.Count) max = ie.InOut.Count;
            }
            return max;
        }


        // Расширить набор первичных ключевых элементов
        public ArrayList GetExpandPrimary(ArrayList primary, int range, int screen)
        {
            Searcher search = new Searcher();
            ArrayList result = search.SearchContext(ConvertToSortedList(primary), range, screen);
            return result;
        }

        static public SortedList ConvertToSortedList(ArrayList al)
        {
            if (al == null) return null;
            SortedList result = new SortedList();
            foreach (IE_Count ie_count in al)
            {
                result.Add(ie_count.IE.Name, new IE_Value(ie_count.IE, ie_count.Count));
            }
            return result;
        }

        // Приведение к единице суммы всех коэффициентов
        static public void Reduction(SortedList sl)
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

        // Вычислить тематическую близость
        // standard - множество инф. эл. определяющих тематику текста
        // образца
        // verifiable - множество инф. эл. определяющих тематику сравноваемого текста 
        static public SortedList CalculateNearness(SortedList standard, SortedList verifiable,
                                                   int range, ref double total)
        {
            total = 0;
            SortedList result = new SortedList();
            for (int i = 0; i < standard.Count; i++)
            {
                string ie = (string)standard.GetKey(i);
                if (verifiable.Contains(ie))
                {
                    // Вычисляем близость коэф.
                    double k1 = ((IE_Value)standard.GetByIndex(i)).Value;
                    double k2 = ((IE_Value)verifiable[ie]).Value;
                    double w = Nearness(k1, k2);

                    // Учет контекста ---------------------------------
                    SortedList subject = new SortedList();
                    Searcher search = new Searcher();

                    // Выделяем окружение (контекст) для текущего проверяемого
                    // информационного элемента 
                    // из списка ключевых элементов текста образца
                    subject.Add(ie, ((IE_Value)standard.GetByIndex(i)).IE);
                    ArrayList environment1 = search.SearchContext(subject, range, 0);
                    SortedList environment1_sl = ConvertToSortedList(environment1);

                    // Выделяем окружение для текущего проверяемого
                    // информационного элемента
                    // из списка ключевых элементов проверяемого текста
                    subject.Clear();
                    subject.Add(ie, ((IE_Value)verifiable[ie]).IE);
                    ArrayList environment2 = search.SearchContext(subject, range, 0);
                    SortedList environment2_sl = ConvertToSortedList(environment2);

                    double context = CompareEnvironment(environment1_sl, environment2_sl);
                    w *= context;
                    // ----------------------------------------------
                    result.Add(ie, w);
                    total += w;
                }
                else result.Add(ie, (double)0);
            }
            return result;
        }


        // Вычислить близость двух инф. элементов
        // standard - вес элемента из документа образца
        // verifiable - вес элемента из анализируемого документа
        static private double Nearness(double standard, double verifiable)
        {
            double k_max = standard;
            double k_min = verifiable;
            if (standard < verifiable)
            {
                k_max = verifiable;
                k_min = standard;
            }
            double w = (k_min / k_max) * standard;
            return w;
        }

        // Сравнение контекстов
        private static double CompareEnvironment(SortedList environment1, SortedList environment2)
        {
            // Выполнить пересечение двух множеств
            int intersection = 0;
            foreach (string ie in environment1.Keys)
            {
                if (environment2.Contains(ie)) intersection++;
            }
            double result = (double)(intersection * 2) /
                            (double)(environment1.Count + environment2.Count);
            return result;
        }
        // Поиск параметров тематики
        public double FindParamSubject(string text_v1, string text_v2,
                                       double expert_v1, double expert_v2,
                                       ref int threshold, ref int range, ref double scale)
        {
            double[,] array_v1 = null;
            CalculateNearnessParam(text_v1, ref array_v1);

            double[,] array_v2 = null;
            CalculateNearnessParam(text_v2, ref array_v2);

            double diff = MinimizationDiff(expert_v1, expert_v2, array_v1, array_v2,
                                           ref threshold, ref range, ref scale);

            return diff;
        }

        // textVerifiable - ализируемый на тематическую близость
        // текст
        public void CalculateNearnessParam(string textVerifiable, ref double[,] array)
        {
            // Подготавливаем информационную структуру анализируемого текста
            Analyzer ia_verifiable = new Analyzer(textVerifiable);
            array = new double[param_range.Length, param_threshold.Length];
            for (int k = 0; k < param_threshold.Length; k++)
            {
                int threshold = param_threshold[k];
                for (int i = 0; i < param_range.Length; i++)
                {
                    int range = param_range[i];

                    SortedList standard = null;
                    SortedList verifiable = null;

                    // Выделяем наборы ключевых элементов
                    standard = GetKey(threshold, range, 1);
                    verifiable = ia_verifiable.GetKey(threshold, range, 1);

                    // Вычисляем тематическую близость
                    double nearness = 0;
                    CalculateNearness(standard, verifiable, range, ref nearness);
                    array[i, k] = nearness;
                }
            }
        }

        // Создать экземпляр InfoAnalyzer для некоторого текста 
        public Analyzer(string text)
        {
            Convert(text);
        }
        // Конвертировать текст в информационный поток
        public void Convert(string text)
        {
            int index = 0;
            Converter converter = new Converter(text, allIE, ref index);
            index++;
        }

        // Минимизация diff и поиск оптимальных парметров выделения 
        // тематики
        public double MinimizationDiff(double expert_v1, double expert_v2,
                                       double[,] array_v1, double[,] array_v2,
                                       ref int threshold, ref int range, ref double scale)
        {
            int countT = param_threshold.Length;
            int countR = param_range.Length;
            double diff_min = double.MaxValue;
            threshold = 0;
            range = 0;
            double calc_v1 = 0;
            double calc_v2 = 0;
            double diff_expert = expert_v2 / expert_v1;

            for (int k = 0; k < countT; k++)
            {
                for (int i = 0; i < countR; i++)
                {
                    double diff_calc = array_v2[i, k] / array_v1[i, k];
                    double diff = Math.Abs(diff_expert - diff_calc);
                    if (diff < diff_min)
                    {
                        diff_min = diff;
                        threshold = param_threshold[k];
                        range = param_range[i];
                        calc_v2 = array_v2[i, k];
                        calc_v1 = array_v1[i, k];
                    }
                }
            }
            scale = ((expert_v1 / calc_v1) + (expert_v2 / calc_v2)) / 2.0;
            return diff_min;
        }
    }
}
