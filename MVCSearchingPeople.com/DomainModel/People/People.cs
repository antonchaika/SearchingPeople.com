using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace DomainModel.People
{
    public class Human
    {
        public int HumanID { get; set; }
        public string Name { get; set; }
        public string Age { get; set; }
        public string Location { get; set; }
        public string Additional { get; set; }
    }
    public class FakeHuman
    {
        [Required(ErrorMessage = "Пожалуйста, укажите имя...")]
        public string Name { get; set; }
        [Required]
        [Range(5, 100, ErrorMessage = "Введите возраст в пределах от 5 до 100 лет...")]
        public short Age { get; set; }
        [Required(ErrorMessage = "Введите примерное местоположение человека...")]
        public string Location { get; set; }
    }
}
