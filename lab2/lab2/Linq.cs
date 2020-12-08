using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Linq;
using System.Xml.Linq;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace Lab2
{
    class LINQtoXML : IStrategy
    {
        string path;
        public LINQtoXML(string _path)
        {
            path = _path;
        }
        public List<Car> Search(CarSearch c)
        {
            List<Car> result = new List<Car>();

            XDocument doc = XDocument.Load(path);
            var cars = from obj in doc.Descendants("car")
                       select new
                       {
                           brand = (string)obj.Parent.Attribute("Brand"),
                           model = (string)obj.Attribute("model"),
                           year = (string)obj.Attribute("year"),
                           volume = (string)obj.Attribute("volume"),
                           price = (string)obj.Attribute("price"),
                           color = (string)obj.Attribute("color")
                       };

            foreach (var current in cars)
            {
                Car car = new Car();
                car.brand = current.brand;
                car.model = current.model;
                car.year = current.year;
                car.volume = current.volume;
                car.price = current.price;
                car.color = current.color;
                if (IfEqual(car, c)) result.Add(car);
            }
            return result;
        }
        private bool IfEqual(Car c1, CarSearch c2)
        {
            if (c1.brand != c2.brand && c2.brand != "") { return false; }
            if (c1.model != c2.model && c2.model != "") { return false; }
            if (c1.color != c2.color && c2.color != "") { return false; }
            if (Convert.ToDouble(c1.volume) < c2.lowVolume || Convert.ToDouble(c1.volume) > c2.highVolume) { return false; }
            if (Convert.ToInt32(c1.year) < c2.lowYear || Convert.ToInt32(c1.year) > c2.highYear) { return false; }
            if (Convert.ToInt32(c1.price) < c2.lowPrice || Convert.ToInt32(c1.price) > c2.highPrice) { return false; }
            return true;
        }
    }
}