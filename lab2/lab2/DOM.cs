using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Windows.Forms;

namespace Lab2
{
    class DOM : IStrategy
    {
        string path;
        public DOM(string _path)
        {
            path = _path;
        }
        public List<Car> Search(CarSearch c)
        {
            string currentBrand;
            List<Car> result = new List<Car>();

            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            XmlElement root = doc.DocumentElement;


            foreach (XmlNode n in root.ChildNodes)
            {
                foreach (XmlNode a in n.ChildNodes)
                {
                    Car current = new Car();
                    currentBrand = n.Attributes[0].Value;
                    if (currentBrand != c.brand && c.brand != "") { break; }
                    else
                    {
                        current.brand = currentBrand;
                        foreach (XmlAttribute attribu in a.Attributes)
                        {
                            if (attribu.Name == "model") current.model = attribu.InnerText;
                            if (attribu.Name == "year") current.year = attribu.InnerText;
                            if (attribu.Name == "volume") current.volume = attribu.InnerText;
                            if (attribu.Name == "price") current.price = attribu.InnerText;
                            if (attribu.Name == "color") current.color = attribu.InnerText;
                        }
                        if (IfEqual(current, c)) { result.Add(current); }
                    }
                }
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