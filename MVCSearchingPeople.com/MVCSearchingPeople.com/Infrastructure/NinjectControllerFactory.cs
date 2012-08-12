using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using DomainModel.Abstract;
using DomainModel.People;
using Moq;
using MVCSearchingPeople.com.Models;
using Ninject;
using System.Web.Hosting;
using DomainModel.Concrete;

namespace MVCSearchingPeople.com.Infastructure
{
    public class NinjectControllerFactory : DefaultControllerFactory
    {
        private IKernel ninjectKernel;
        public NinjectControllerFactory()
        {
            ninjectKernel = new StandardKernel();
            AddBindings();
        }
        protected override IController GetControllerInstance(RequestContext requestContext,Type controllerType)
        {
            return controllerType == null ? null : (IController)ninjectKernel.Get(controllerType);
        }
        private void AddBindings()
        {
            // Mock implementation of the IProductRepository Interface
            /*Mock<IHumanRepository> mock = new Mock<IHumanRepository>();
            mock.Setup(m => m.Humans).Returns(new List<Human> {
            new Human { HumanID = 0, Links = new Dictionary<string, string> {{"http://vk.com/antonchaika", "Vk.com"}}, Name = "Антон Чайка", Age = "21", Location = "Минск", Image="http://cs9637.userapi.com/u18615771/a_0ca6494b.jpg"},
            new Human { HumanID = 1, Links = new Dictionary<string, string> {{"http://vk.com/antonchaika", "Vk.com"}}, Name = "Андрей Позняк", Age = "20", Location = "Минск", Image="http://cs5863.userapi.com/u7677444/-6/y_5a74ce06.jpg", 
                Tags = new Dictionary<string,double>(){{"привет", 0.5}, {"пример", 0.2},{"хорошо", 0.04}, {"контакт", 0.5},{"суши", 0.7}, {"терпеть", 0.4}, {"привыкать", 0.01} , {"зарубежный", 0.35}, {"высокохудожественный", 0.78}}, 
                Additional = new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>("День рождения: ", "16 ноября 1991 г."), new KeyValuePair<string, string>("Семейное положение: ", "не женат"), new KeyValuePair<string, string>("ВУЗ: ", "БГУИР (бывш. МРТИ) '14")} },
            new Human { HumanID = 2, Links = new Dictionary<string, string> {{"http://vk.com/antonchaika", "Vk.com"}}, Name = "Антон Чайка", Age = "21", Location = "Minsk", Image=HostingEnvironment.ApplicationPhysicalPath + "Content\\images\\ava.gif" },
            //new Human { HumanID = 3, Links = new Dictionary<string, string> {{"http://vk.com/antonchaika", "Vk.com"}}, Name = "Илья Вашкевич", Age = "20", Location = "Vitebsk", Image=HostingEnvironment.ApplicationPhysicalPath + "Content\\images\\ava.gif" },
            //new Human { HumanID = 4, Links = new Dictionary<string, string> {{"http://vk.com/antonchaika", "Vk.com"}}, Name = "Антон Чайка", Age = "21", Location = "Minsk", Image=HostingEnvironment.ApplicationPhysicalPath + "Content\\images\\ava.gif" },
            });
            ninjectKernel.Bind<IHumanRepository>().ToConstant(mock.Object);*/
            HumanRepository rep = new HumanRepository();
            SearchNET service = new SearchNET();
            ninjectKernel.Bind<IHumanRepository>().ToConstant(rep);
            ninjectKernel.Bind<ISearchNET>().ToConstant(service);
        }
    }
}