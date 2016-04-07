using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public int[,] input = new int[3, 5];
        Web NW1;

        private void button1_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            button2.Enabled = true;
            openFileDialog1.Title = "Укажите тестируемый файл";
            openFileDialog1.ShowDialog();
            pictureBox1.Image = Image.FromFile(openFileDialog1.FileName);

            Bitmap im = pictureBox1.Image as Bitmap; //сформировать массив входных данных
            for (var i = 0; i <= 5; i++) listBox1.Items.Add(" ");

            for (var x = 0; x <= 2; x++)
            {
                for (var y = 0; y <= 4; y++)
                {
                    // listBox1.Items.Add(Convert.ToString(im.GetPixel(x, y).R));
                    int n = (im.GetPixel(x, y).R);
                    if (n >= 250) n = 0; // Определяем, закрашен ли пиксель
                    else n = 1;
                    listBox1.Items[y] = listBox1.Items[y] + "  " + Convert.ToString(n);
                    input[x, y] = n; // Присваиваем соответствующее значение каждой ячейке входных данных
                    //if (n == 0) input[x, y] = 1;
                }

            }

            recognize();
        }

        public void recognize() //Распознаем символ, вызывая описанные выше методы класса
        {
            NW1.mul_w();
            NW1.Sum();
            if (NW1.Rez()) listBox1.Items.Add(" - True, Sum = "+Convert.ToString(NW1.sum));
            else listBox1.Items.Add( " - False, Sum = "+Convert.ToString(NW1.sum));
        }

        class Web //Класс нейрона
        {
            public int[,] mul; // Тут будем хранить отмасштабированные сигналы
            public int[,] weight; // Массив для хранения весов
            public int[,] input; // Входная информация
            public int limit = 9; // Порог - выбран экспериментально, для быстрого обучения
            public int sum; // Тут сохраним сумму масштабированных сигналов

            public Web(int sizex, int sizey, int[,] inP) // Задаем свойства при создании объекта
            {
                weight = new int[sizex, sizey]; // Определяемся с размером массива (число входов)
                mul = new int[sizex, sizey];

                input = new int[sizex, sizey];
                input = inP; // Получаем входные данные
            }


            public void mul_w() //Масштабирование
            {
                for (int x = 0; x <= 2; x++) 
                {
                    for (int y = 0; y <= 4; y++) // Пробегаем по каждому аксону
                    {
                        mul[x, y] = input[x, y] * weight[x, y]; // Умножаем его сигнал (0 или 1) на его собственный вес и сохраняем в массив.
                    }
                }
            }

            public void Sum() //Сложение
            {
                sum = 0;
                for (int x = 0; x <= 2; x++)
                {
                    for (int y = 0; y <= 4; y++)
                    {
                        sum += mul[x, y];
                    }
                }
            }

            public bool Rez() //Сравнение
            {
                if (sum >= limit)
                    return true;
                else return false;
            }
            public void incW(int[,] inP) //Если её неправильный ответ False, то прибавляем значения входов к весам каждой ножки (к ножке 1 — значение в точке [0,0] картинки и т.д.):
            {
                for (int x = 0; x <= 2; x++)
                {
                    for (int y = 0; y <= 4; y++)
                    {
                        weight[x, y] += inP[x, y];
                    }
                }
            }
            public void decW(int[,] inP) //Если её неправильный ответ True, то вычитаем значения входов из веса каждой ножки:
            {
                for (int x = 0; x <= 2; x++)
                {
                    for (int y = 0; y <= 4; y++)
                    {
                        weight[x, y] -= inP[x, y];
                    }
                }
            }

        }



        private void Form1_Load(object sender, EventArgs e) //Cохранtybt значения весов в текстовый файл
        {


            NW1 = new Web(3, 5, input); // Создаем экземпляр нашего нейрона

            openFileDialog1.Title = "Укажите файл весов";
            openFileDialog1.ShowDialog();
            string s = openFileDialog1.FileName;
            StreamReader sr = File.OpenText(s); // Загружаем файл весов
            string line;
            string[] s1;
            int k = 0;
            while ((line = sr.ReadLine()) != null)
            {
               
                s1 = line.Split(' ');
                for (int i = 0; i < s1.Length; i++)
                {
                    listBox1.Items.Add("");
                    if (k < 5)
                    {
                        NW1.weight[i, k] = Convert.ToInt32(s1[i]); // Назначаем каждой связи её записанный ранее вес
                        listBox1.Items[k] += Convert.ToString(NW1.weight[i, k]); // Выводим веса, для наглядности
                    }

                }
                k++;

            }
            sr.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button2.Enabled = false;

                if (NW1.Rez() == false)
                    NW1.incW(input);
                else NW1.decW(input);
               
                //Запись
                  string s="";
                  string[] s1 = new string[5];
                  System.IO.File.Delete("w.txt");
                  FileStream FS = new FileStream("w.txt", FileMode.OpenOrCreate);
                  StreamWriter SW = new StreamWriter(FS);

                for (int y = 0; y <= 4; y++)
                {

                    s = Convert.ToString(NW1.weight[0, y]) + " " + Convert.ToString(NW1.weight[1, y]) + " " + Convert.ToString(NW1.weight[2, y]) ;
                        

                    s1[y] = s;
                   
                    SW.WriteLine(s);

                }
                SW.Close();


            
        }
    }

}
