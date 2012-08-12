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
        public string Image { set; get; }
        public Dictionary<string, string> Links { get; set; }
        public Dictionary<string, double> Tags { get; set; }
        public List<KeyValuePair<string, string>> Additional { get; set; }
        public static string Settings { get; set; }
    }
    public class FakeHuman
    {
        [Required(ErrorMessage = "Пожалуйста, укажите имя...")]
        public string Name { get; set; }
        //[Required(ErrorMessage = "Пожалуйста, укажите примерный возраст...")]
        //[Range(13, 100, ErrorMessage = "Введите возраст в пределах от 13 до 100 лет...")]
        public short? Age { get; set; }
        //[Required(ErrorMessage = "Введите примерное местоположение человека...")]
        public string Location { get; set; }
    }
}