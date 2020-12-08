using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Xsl;
using System.Diagnostics;

namespace Lab2
{ 
    public partial class Form1 : Form
    {
        private string path = @"C:\Users\ivnik\source\repos\lab2\lab2\XMLCar.xml";
        private string xslPath = @"C:\Users\ivnik\source\repos\lab2\lab2\Car.xsl";
        string fXML = @"C:\Users\ivnik\source\repos\lab2\lab2\CurrentCars.xml";
        string fHTML = @"C:\Users\ivnik\source\repos\lab2\lab2\out.html";
        Dictionary<string, SortedSet<string>> modelList = new Dictionary<string, SortedSet<string>>();
        List<Car> list = new List<Car>();
        public Form1()
        {
            InitializeComponent();
            GetAllCars();
            Model.Enabled = false;
            ModelCheck.Enabled = false;
            Dom.Checked = true;
        }

        private void GetAllCars()
        {
            string currBrand;
            XmlDocument doc = new XmlDocument();
            doc.Load(path);

            XmlElement xRoot = doc.DocumentElement;
            foreach (XmlNode n in xRoot.ChildNodes)
            {
                foreach (XmlNode a in n.ChildNodes)
                {
                    currBrand = n.Attributes[0].Value;
                    if (!Brand.Items.Contains(currBrand)) Brand.Items.Add(currBrand);

                    if (modelList.ContainsKey(currBrand))
                        modelList[currBrand].Add(a.Attributes[0].Value);
                    else
                    {
                        SortedSet<string> x = new SortedSet<string>();
                        x.Add(a.Attributes[0].Value);
                        modelList[currBrand] = x;
                    }
                    if (!Color.Items.Contains(a.Attributes[4].Value)) Color.Items.Add(a.Attributes[4].Value);
                }
            }
        }

        private void PrintInfo(List<Car> lis)
        {
            CarInfo.Text = "";
            foreach (Car car in lis)
            {
                CarInfo.Text += "Марка: " + car.brand + '\n';
                CarInfo.Text += "Модель: " + car.model + '\n';
                CarInfo.Text += "Рік випуску: " + car.year + '\n';
                CarInfo.Text += "Об'єм двигуна: " + car.volume + '\n';
                CarInfo.Text += "Колір: " + car.color + '\n';
                CarInfo.Text += "Ціна: " + car.price + '\n';
                CarInfo.Text += "\n\n";
            }
            if (CarInfo.Text == "")
            {
                CarInfo.Text = "Жодних результатів =(";
            }
        }

        private CarSearch FindParams()
        {
            CarSearch car = new CarSearch();
            if (BrandCheck.Checked) { car.brand = Brand.Text; }
            if (ModelCheck.Checked) { car.model = Model.Text; }
            if (ColorCheck.Checked) { car.color = Color.Text; }
            if (PriceCheck.Checked)
            {
                if (LowerPrice.Text != "") { car.lowPrice = Convert.ToInt32(LowerPrice.Text); }
                if (HigherPrice.Text != "") { car.highPrice = Convert.ToInt32(HigherPrice.Text); }
            }
            if (VolumeCheck.Checked)
            {
                if (LowerVolume.Text != "") { car.lowVolume = Convert.ToDouble(LowerVolume.Text); }
                if (HigherVolume.Text != "") { car.highVolume = Convert.ToDouble(HigherVolume.Text); }
            }
            if (YearCheck.Checked)
            {
                if (LowerYear.Text != "") { car.lowYear = Convert.ToInt32(LowerYear.Text); }
                if (HigherYear.Text != "") { car.highYear = Convert.ToInt32(HigherYear.Text); }
            }

            return car;
        }

        private void CreateXML()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(fXML);
            XmlElement root = doc.DocumentElement;
            root.RemoveAll();

            foreach (Car car in list)
            {
                XmlElement x = doc.DocumentElement;
                XmlElement c = doc.CreateElement("brand");
                XmlAttribute a = doc.CreateAttribute("Brand");
                XmlText t = doc.CreateTextNode(car.brand);

                XmlElement s = doc.CreateElement("car");


                XmlAttribute b = doc.CreateAttribute("color");
                XmlText y = doc.CreateTextNode(car.color);
                b.AppendChild(y);
                s.Attributes.Append(b);

                b = doc.CreateAttribute("price");
                y = doc.CreateTextNode(car.price);
                b.AppendChild(y);
                s.Attributes.Append(b);

                b = doc.CreateAttribute("volume");
                y = doc.CreateTextNode(car.volume);
                b.AppendChild(y);
                s.Attributes.Append(b);

                b = doc.CreateAttribute("year");
                y = doc.CreateTextNode(car.year);
                b.AppendChild(y);
                s.Attributes.Append(b);

                b = doc.CreateAttribute("model");
                y = doc.CreateTextNode(car.model);
                b.AppendChild(y);
                s.Attributes.Append(b);

                c.AppendChild(s);

                a.AppendChild(t);
                c.Attributes.Append(a);
                x.AppendChild(c);
            }
            doc.Save(fXML);
        }

        private void Find_Click(object sender, EventArgs e)
        {
            IStrategy strategy = new DOM(path);
            CarSearch c = FindParams();

            if (Dom.Checked == true)
            {
                strategy = new DOM(path);
            }
            if (Sax.Checked == true)
            {
                strategy = new SAX(path);
            }
            if (Linq.Checked == true)
            {
                strategy = new LINQtoXML(path);
            }

            list = strategy.Search(c);
            PrintInfo(list);
            CreateXML();
        }

        private void Clear_Click(object sender, EventArgs e)
        {
            CarInfo.Text = "";
            Brand.Text = "";
            Model.Text = "";
            Color.Text = "";
            LowerYear.Text = "";
            HigherYear.Text = "";
            LowerPrice.Text = "";
            HigherPrice.Text = "";
            LowerVolume.Text = "";
            HigherVolume.Text = "";
            BrandCheck.Checked = false;
            ModelCheck.Checked = false;
            YearCheck.Checked = false;
            ColorCheck.Checked = false;
            PriceCheck.Checked = false;
            VolumeCheck.Checked = false;
            Model.Enabled = false;
            ModelCheck.Enabled = false;
        }

        private void Brand_TextUpdate(object sender, EventArgs e)
        {
            if (Brand.Text != "")
            {
                Model.Enabled = true;
                ModelCheck.Enabled = true;
                Model.Items.Clear();
                foreach (string str in modelList[Brand.Text])
                {
                    Model.Items.Add(str);
                }
            }
            else
            {
                Model.Enabled = false;
                ModelCheck.Enabled = false;
                ModelCheck.Checked = false;
                Model.Items.Clear();
            }
        }

        private void TransformToHTML()
        {
            XslCompiledTransform xslCompiledTransform = new XslCompiledTransform();
            xslCompiledTransform.Load(xslPath);
            xslCompiledTransform.Transform(fXML, fHTML);
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Name == "trans")
            {
                TransformToHTML();
                MessageBox.Show("Виконано!");
            }
            else
                if (e.ClickedItem.Name == "open")
            {
                var openHTML = System.Diagnostics.Process.Start(fHTML);
            }

            else MessageBox.Show("1. Оберіть критерії пошуку.\n" +
            "2. Оберіть метод пошуку ( За замовчуванням DOM).\n" +
            "3. Натисніть кнопку Знайти.\n" +
            "4. Якщо ви бажаєте перевести файл у HTML натисніть Трансформувати в HTML та Відкрити HTML.\n" +
            "5. Щоб очистити поля та виконати новий пошук, натисніть Очистити", "Довідка!", MessageBoxButtons.OK);
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult result = MessageBox.Show("Ви дійсно бажаєте закрити програму?", "Закриття", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.No)
            {
                e.Cancel = true;
            }
        }

    }
}