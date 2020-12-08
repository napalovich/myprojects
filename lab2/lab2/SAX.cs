using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Windows.Forms;

namespace Lab2
{
    class SAX : IStrategy
    {
        string path;
        public SAX(string _path)
        {
            path = _path;
        }
        public List<Car> Search(CarSearch c)
        {
            List<Car> result = new List<Car>();

            var reader = new XmlTextReader(path);

            string currBrand = "";
            while (reader.Read())
            {
                if (reader.Name == "brand")
                {
                    if (reader.HasAttributes)
                    {
                        reader.MoveToNextAttribute();
                        if (reader.Name.Equals("Brand"))
                        {
                            currBrand = reader.Value;
                        }
                    }
                }
                if (reader.Name == "car")
                {
                    if (reader.HasAttributes)
                    {
                        Car current = new Car();
                        current.brand = currBrand;
                        while (reader.MoveToNextAttribute())
                        {
                            if (reader.Name == "model") current.model = reader.Value;
                            if (reader.Name == "year") current.year = reader.Value;
                            if (reader.Name == "volume") current.volume = reader.Value;
                            if (reader.Name == "price") current.price = reader.Value;
                            if (reader.Name == "color") current.color = reader.Value;
                        }
                        if (IfEqual(current, c)) { result.Add(current); }
                    }
                }
            }
            reader.Close();
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