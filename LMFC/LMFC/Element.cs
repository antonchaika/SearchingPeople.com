using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace ThematicAnalyser
{
    // Класс для хранения и обработки пар вход-выход
    public class InOut
    {
        private Element inIE;
        private Element outIE;

        public InOut(Element inIE_rq, Element outIE_rq)
        {
            inIE = inIE_rq;
            outIE = outIE_rq;
        }

        public Element InIE
        {
            get { return inIE; }
        }

        public Element OutIE
        {
            get { return outIE; }
        }
    }

    // Класс InfoElement - информационный элемент, формирующий инф. структуру
    public class Element
    {
        // Key - index, value - InOut
        public SortedList InOut = new SortedList();
        private string name;    // имя инф. элемента

        public Element(string name_rq)
        {
            name = name_rq;
        }

        public string Name
        {
            get { return name; }
        }

        public bool ContainsInOut(InOut in_out)
        {
            foreach (InOut v in InOut.Values)
            {
                if (v.InIE == in_out.InIE)
                    if (v.OutIE == in_out.OutIE) return true;
            }
            return false;
        }
    }
}
