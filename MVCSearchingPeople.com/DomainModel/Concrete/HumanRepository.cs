using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DomainModel.Abstract;
using DomainModel.People;

namespace DomainModel.Concrete
{
    public class HumanRepository : IHumanRepository
    {
        private HumanContext context = new HumanContext();
        public List<Human> Humans
        {
            get { return context.Humans; }
        }
        public HumanRepository()
        {
            context.Humans = new List<Human>();
        }
        public void SaveProduct(Human human)
        {
            if (human.HumanID != 0)
            {
                context.Humans.Add(human);
            }
        }
    }
}