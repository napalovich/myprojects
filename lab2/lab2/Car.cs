using System;
using System.Collections.Generic;
using System.Text;

namespace Lab2
{
    interface IStrategy
    {
        List<Car> Search(CarSearch c);
    }

    class Car
    {
        public string brand;
        public string model;
        public string year;
        public string volume;
        public string color;
        public string price;

        public Car()
        {
            brand = "";
            model = "";
            year = "";
            volume = "";
            color = "";
            price = "";
        }
    }

    class CarSearch : Car
    {
        public int lowYear;
        public int highYear;
        public int lowPrice;
        public int highPrice;
        public double lowVolume;
        public double highVolume;
        public CarSearch()
        {
            brand = "";
            model = "";
            year = "";
            volume = "";
            color = "";
            price = "";
            lowPrice = 0;
            highPrice = 100000;
            lowYear = 1950;
            highYear = 2020;
            lowVolume = 1;
            highVolume = 6;
        }
    }

}