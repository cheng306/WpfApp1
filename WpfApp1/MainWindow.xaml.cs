﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;
using WpfApp1.Models;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //path of xml file that store itemsList in Edit Page
        string itemListXmlPath;

        //Serializer used to serialize and deserialize ObservableCollection<Item>
        XmlSerializer listSerializer;

        string categoriesListXmlPath;
        XmlSerializer CategoriesListSerializer;

        public MainWindow()
        {
            InitializeComponent();

            Console.WriteLine("================Window start==================");

            //Create the directory for database if not exist
            CreateDbDirectory();

            //locate the path and initialize the serializer
            ItemsList();


            //editPage.itemList.Add(new Item { Name = "bbb", Category = "aaa", Price = 12.6, AddTime = DateTime.Now });
            //editPage.itemList.Add(new Item { Name = "www", Category = "aaa", Price = 12.6, AddTime = DateTime.Now });
            //editPage.itemList.Add(new Item { Name = "ggg", Category = "aaa", Price = 12.6, AddTime = DateTime.Now });
            //editPage.itemList.Add(new Item { Name = "bbb", Category = "kkk", Price = 12.6, AddTime = DateTime.Now });
            //editPage.itemList.Add(new Item { Name = "bbb", Category = "bbb", Price = 12.6, AddTime = DateTime.Now });
            //editPage.itemList.Add(new Item { Name = "bbb", Category = "zzz", Price = 12.6, AddTime = DateTime.Now });
            //editPage.itemList.Add(new Item { Name = "qqq", Category = "www", Price = 12.6, AddTime = DateTime.Now });
            //editPage.itemList.Add(new Item { Name = "eee", Category = "rrr", Price = 12.6, AddTime = DateTime.Now });
            //editPage.itemList.Add(new Item { Name = "ttt", Category = "yyy", Price = 12.6, AddTime = DateTime.Now });
            //editPage.itemList.Add(new Item { Name = "ccc", Category = "aaa", Price = 12.6, AddTime = DateTime.Now });
            //editPage.itemList.Add(new Item { Name = "ccc", Category = "bbb", Price = 12.6, AddTime = DateTime.Now });
            //editPage.itemList.Add(new Item { Name = "ccc", Category = "ccc", Price = 12.6, AddTime = DateTime.Now });



        }

        private void CreateDbDirectory()
        {
            DirectoryInfo dbDirectory = new DirectoryInfo("db");
            if (!dbDirectory.Exists)
            {
                dbDirectory.Create();
            }
        }

        private void ItemsList()
        {
            //locate the path
            itemListXmlPath = @"db\itemlist.xml";

            //initilize serizlizer
            listSerializer = new XmlSerializer(typeof(ObservableCollection<Item>));

            if (File.Exists(itemListXmlPath))
            {
                StreamReader streamReader = new StreamReader(itemListXmlPath);
                editPage.itemList = (ObservableCollection<Item>)listSerializer.Deserialize(streamReader);
                streamReader.Close();
                Console.WriteLine("itemList loaded");
            }
            else
            {
                editPage.itemList = new ObservableCollection<Item>();
                Console.WriteLine("itemList created");
            }

            editPage.itemsListView.ItemsSource = editPage.itemList;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {     
            FileStream fs = File.Create(itemListXmlPath);
            ObservableCollection<Item> list = editPage.itemList;

            listSerializer.Serialize(fs, list);
            fs.Close();
        }
    }
}
