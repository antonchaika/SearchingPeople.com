using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DomainModel.People;

namespace DomainModel.Abstract
{
    public interface IHumanRepository
    {
        List<Human> Humans { get; }
        void SaveProduct(Human human);
    }

    public interface ISearchNET
    {
        void Search(IHumanRepository rep, int operationid);
        void GetFake(FakeHuman fake);
    }
}