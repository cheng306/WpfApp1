﻿using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using RestaurantPOS.Models;
using System.Collections.ObjectModel;

namespace RestaurantPOS.Dialogs
{
  /// <summary>
  /// 
  /// </summary>
  public partial class EditItemDialog : Window
  {
    bool validName;
    bool validPrice;
    bool validCategory;
    string currentItemName;
    string currentItemCategory;

    MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
    ObservableCollection<string> categoriesList = ((App)Application.Current).categoriesList;
    Dictionary<string, List<Item>> categoryItemDict = ((App)(Application.Current)).categoryItemsListDict;


    public EditItemDialog()
    {
      InitializeComponent();
      OtherInitializeSetup();
    }

    public EditItemDialog(Item item)
    {
      InitializeComponent();
      OtherInitializeSetup2(item);
    }

    private void OtherInitializeSetup()
    {
      currentItemName = "";
      currentItemCategory = "";
      addButton.IsEnabled = false;
      categoriesComboBox.ItemsSource = categoriesList;

      nameWarningTextBlock.Visibility = Visibility.Visible;
      priceWarningTextBlock.Visibility = Visibility.Visible;
      categoryWarningTextBlock.Visibility = Visibility.Visible;

      validName = false;
      validPrice = false;
      validCategory = false;
    }

    private void OtherInitializeSetup2(Item item)
    {
      currentItemName = item.Name;
      currentItemCategory = item.Category;
      addButton.IsEnabled = true;
      addButton.Content = "Edit";
      nameTextBox.Text = item.Name;
      priceTextBox.Text = item.Price.ToString();
      categoriesComboBox.ItemsSource = categoriesList;
      categoriesComboBox.SelectedItem = item.Category;

      nameWarningTextBlock.Visibility = Visibility.Hidden;
      priceWarningTextBlock.Visibility = Visibility.Hidden;
      categoryWarningTextBlock.Visibility = Visibility.Hidden;

      validName = true;
      validPrice = true;
      validCategory = true;
    }

    internal String ItemName
    {
      get { return nameTextBox.Text; }
    }

    internal String ItemCategory
    {
      get { return (string)categoriesComboBox.SelectedItem; }
    }

    internal double ItemPrice
    {
      get { return Double.Parse(priceTextBox.Text); }
    }

    private void NameTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
      if (!nameTextBox.Text.Equals(""))
      {
        nameWarningTextBlock.Visibility = Visibility.Hidden;
        if (categoriesComboBox.SelectedItem != null)
        {
          Console.WriteLine("=======hit it 2");
          CheckIfItemNameRepeat();
        }
      }
      else //blank name
      {
        validName = false;
        nameWarningTextBlock.Text = "Name cannot be Blank";
        nameWarningTextBlock.Visibility = Visibility.Visible;
        UpdateAddButton();
      }
    }

    private void PriceTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
      if (Double.TryParse(priceTextBox.Text, out double dump))
      {
        validPrice = true;
        priceWarningTextBlock.Visibility = Visibility.Hidden;
        UpdateAddButton();
      }
      else
      {
        validPrice = false;
        priceWarningTextBlock.Visibility = Visibility.Visible;
        UpdateAddButton();
      }
    }

    private void CategoriesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      validCategory = true;
      categoryWarningTextBlock.Visibility = Visibility.Hidden;

      if (!nameTextBox.Text.Equals(""))
      {
        Console.WriteLine("======hit it 3");
        CheckIfItemNameRepeat();
      }
    }

    //do this check after making sure nameTextBox.Text is not empty and categoresComboBox is not null
    private void CheckIfItemNameRepeat()
    {
      validName = true;
      nameWarningTextBlock.Visibility = Visibility.Hidden;

      string category = (string)categoriesComboBox.SelectedItem;
      Console.WriteLine("=========hit it1");
      Console.WriteLine("id: " + this.GetHashCode());
      List<Item> itemsList = categoryItemDict[category];
      foreach(Item item in itemsList)
      {
        if (item.Name.Equals(nameTextBox.Text) && !(item.Name.Equals(currentItemName) && item.Category.Equals(currentItemCategory)))
        {
          validName = false;
          nameWarningTextBlock.Text = nameTextBox.Text + " already existed in this Category: "+ category;
          nameWarningTextBlock.Visibility = Visibility.Visible;
          break;
        }
      }
      UpdateAddButton();
    }

    private void UpdateAddButton()
    {
      if (validName && validPrice && validCategory)
      {
        addButton.IsEnabled = true;
      }
      else
      {
        addButton.IsEnabled = false;
      }
    }

    private void AddButton_Click(object sender, RoutedEventArgs e)
    {
      this.DialogResult = true;
    }

    internal void DisableDialog()
    {
      nameTextBox.TextChanged -= NameTextBox_TextChanged;
      categoriesComboBox.SelectionChanged -= CategoriesComboBox_SelectionChanged;
      priceTextBox.TextChanged -= PriceTextBox_TextChanged;
    }




  }
}
