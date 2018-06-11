﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using RestaurantPOS.Pages;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RestaurantPOS.Models
{
  public class Table: INotifyPropertyChanged
  {
    private double priceTotal;
    ObservableCollection<ItemNameCategoryQuantity> itemNameCategoryQuantityList;

    public Table()
    {
      this.itemNameCategoryQuantityList = new ObservableCollection<ItemNameCategoryQuantity>();
    }

    public double PriceTotal {
      get { return this.priceTotal; }
      set
      {
        if (value != this.priceTotal)
        {
          this.priceTotal = value;
          NotifyPropertyChanged();
        }
      }
    }

    public Point UpperLeftPoint { get; set; }

    public int TableNumber { get; set; }

    public Boolean IsActive { get; set; }

    public ObservableCollection<ItemNameCategoryQuantity> DatabaseItemNameCategoryQuantityList {
      get { return this.itemNameCategoryQuantityList; }
      set { this.itemNameCategoryQuantityList = value; }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
    {
      if (PropertyChanged != null)
      {
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
      }
    }

  }

  

}
