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
using System.Windows.Navigation;
using System.Windows.Shapes;
using RestaurantPOS.Models;
using System.Diagnostics;
using RestaurantPOS.Dialogs;

namespace RestaurantPOS.Pages
{
  /// <summary>
  /// Interaction logic for Table.xaml
  /// </summary>
  public partial class TablesPage : UserControl
  {
    Circle newTable;

    Point previousPt;

    Point addButtonAbsolutePoint;

    Point deleteButtonAbsolutePoint;

    //This is the horizontal distance between the circle UpperLeft and the mouse or touch pointer
    double xDistance;
    double yDistance;

    //radius if Circle
    double radius;

    //diameter of Circle
    double diameter;

    //a list of all centers of circles. Used for Overlap detection
    List<Point> pointList;

    //a list of circles 
    List<Models.Table> tablesList;
    //a list of boolean
    List<bool> tableNumberBooleanList;

    Object auxObject;

    bool goToSelectionPage;

    

    double canvasDimension;

    Circle selectedCircle;

    SolidColorBrush redBrush = new SolidColorBrush(Colors.Red);
    SolidColorBrush yellowBrush = new SolidColorBrush(Colors.Yellow);
    SolidColorBrush greenBrush = new SolidColorBrush(Colors.Green);
    SolidColorBrush blueBrush = new SolidColorBrush(Colors.Blue);
    SolidColorBrush whiteBrush = new SolidColorBrush(Colors.White);

    MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

    public TablesPage()
    {
      InitializeComponent();
      OtherInitialSetup();
      Console.WriteLine("=================tablesPage Created");
    }

    private void OtherInitialSetup()
    {
      pointList = new List<Point>();

      tablesList = ((App)Application.Current).tablesList;
      tableNumberBooleanList = ((App)Application.Current).tableNumberBooleanList;
       
      previousPt = new Point();
      selectedCircle = null;

      double minDimension = Math.Min(SystemParameters.FullPrimaryScreenWidth, SystemParameters.FullPrimaryScreenHeight);
      canvasDimension = minDimension * 0.85;
      canvas.Width = canvasDimension;
      canvas.Height = canvasDimension;

      diameter = canvasDimension / 10;
      radius = diameter / 2;

      addButtonAbsolutePoint = new Point(canvasDimension - diameter, canvasDimension - diameter);
      deleteButtonAbsolutePoint = new Point(0, canvasDimension - diameter);

      addButton.numberTextBlock.Text = "+";
      addButton.numberTextBlock.Foreground = whiteBrush;

      pointList.Add(addButtonAbsolutePoint);
      addButton.circleUI.Width = diameter;
      addButton.circleUI.Height = diameter;

      //deleteButton.circleUI.Fill = new SolidColorBrush(Colors.Red);
      deleteButton.circleUI.Fill = redBrush;
      deleteButton.numberTextBlock.Text = "-";
      deleteButton.circleUI.Width = diameter;
      deleteButton.circleUI.Height = diameter;
      deleteButton.numberTextBlock.Foreground = whiteBrush;
      //deleteButton.numberTextBlock.Foreground = new SolidColorBrush(Colors.White);

      //TablesUI must be loaded after 
      LoadTablesUI();
    }

    private void AddButton_MouseDown(object sender, MouseButtonEventArgs e)
    {

      Console.WriteLine("AddButton_MouseDown");
      if (e.LeftButton == MouseButtonState.Pressed && selectedCircle == null)
      {
        Console.WriteLine("In_AddButton_MouseDown");
        newTable = new Circle();
        selectedCircle = newTable;

        //how the new circle look
        xDistance = diameter * 0.75;
        yDistance = diameter * 0.75;
        newTable.SetValue(Canvas.LeftProperty, e.GetPosition(canvas).X - xDistance);
        newTable.SetValue(Canvas.TopProperty, e.GetPosition(canvas).Y - yDistance);
        //SolidColorBrush myBrush = new SolidColorBrush()
        //{
        //  Color = Colors.Red
        //};
        newTable.circleUI.Fill = redBrush;
        newTable.Opacity = 0.5;
        newTable.circleUI.Width = diameter;
        newTable.circleUI.Height = diameter;
        ChangeZIndex(newTable, 3);


        //logic of the new circle
        newTable.Added = false;
        canvas.Children.Add(newTable);

        newTable.CaptureMouse();

        //add listener
        AddMouseListener(newTable);
      }

    }

    private void AddButton_TouchDown(object sender, TouchEventArgs e)
    {
      Console.WriteLine("AddButton_TouchDown");
      if (selectedCircle == null)
      {
        Console.WriteLine("In_AddButton_TouchDown");
        newTable = new Circle();
        selectedCircle = newTable;

        //how the new circle look
        xDistance = diameter * 0.75;
        yDistance = diameter * 0.75;
        newTable.SetValue(Canvas.LeftProperty, e.GetTouchPoint(canvas).Position.X - xDistance);
        newTable.SetValue(Canvas.TopProperty, e.GetTouchPoint(canvas).Position.Y - yDistance);
        //SolidColorBrush myBrush = new SolidColorBrush()
        //{
        //  Color = Colors.Red
        //};
        newTable.circleUI.Fill = redBrush;
        newTable.Opacity = 0.5;
        newTable.circleUI.Width = diameter;
        newTable.circleUI.Height = diameter;


        //logic of the new circle
        newTable.Added = false;
        canvas.Children.Add(newTable);
        ChangeZIndex(newTable, 3);
        newTable.CaptureTouch(e.TouchDevice);

        //add listener
        AddTouchListener(newTable);
      }

    }

    private async void Table_MouseDownAsync(object sender, MouseButtonEventArgs e)
    {
      Console.WriteLine("Table_MouseDownAsync");
      if (e.LeftButton == MouseButtonState.Pressed && selectedCircle == null)
      {
        Console.WriteLine("In_Table_MouseDownAsync");
        Circle circle = (Circle)sender;
        RemoveTouchListeners(circle);
        selectedCircle = circle;
        auxObject = new object();
        Object auxObject2 = auxObject;
        Console.WriteLine("goToSelectionPage--1");
        goToSelectionPage = true;
        //hold for 1 second, if mouse leave within a second, it fail to gain mousecapture
        await HoldDelay();

        if (auxObject == auxObject2)
        {
          Console.WriteLine("goToSelectionPage--2");
          goToSelectionPage = false;

          ChangeZIndex(circle, 3);

          previousPt.X = (Double)(circle.GetValue(Canvas.LeftProperty));
          previousPt.Y = (Double)(circle.GetValue(Canvas.TopProperty));

          pointList.Remove(new Point(previousPt.X, previousPt.Y));

          xDistance = e.GetPosition(canvas).X - previousPt.X;
          yDistance = e.GetPosition(canvas).Y - previousPt.Y;

          circle.Opacity = 0.5;
          //((SolidColorBrush)circle.circleUI.Fill).Color = Colors.Green;
          circle.circleUI.Fill = greenBrush;

          circle.CaptureMouse();
        }
      }
    }

    private void Table_MouseLeave(object sender, MouseEventArgs e)
    {
      Console.WriteLine("Table_MouseLeave");
      Circle circle = (Circle)sender;
      if (circle == selectedCircle)
      {
        Console.WriteLine("In_Table_MouseLeave");
        auxObject = null;
        Console.WriteLine("goToSelectionPage--3");
        goToSelectionPage = false;
        selectedCircle = null;

        AddTouchListener(circle);
      }

    }

    private void Table_MouseMove(object sender, MouseEventArgs e)
    {
      Circle circle = (Circle)sender;
      if (circle.IsMouseCaptured)//&& e.LeftButton == MouseButtonState.Pressed
      {
        Point point = e.GetPosition(canvas);

        circle.SetValue(Canvas.LeftProperty, point.X - xDistance);//Canvas.LeftProperty
        circle.SetValue(Canvas.TopProperty, point.Y - yDistance);

        point.X = point.X - xDistance;
        point.Y = point.Y - yDistance;

        if (!CoordinateConflict(circle, point))
        {
          circle.circleUI.Fill = greenBrush;
        }
        else
        {
          circle.circleUI.Fill = redBrush;
        }
      }
    }


    //fist condition check if mouseup after moving a table or go to selectionPage
    //if after moving a table, checked if table is added
    private void Table_MouseUp(object sender, MouseButtonEventArgs e)
    {
      Console.WriteLine("Table_MouseUp");
      Circle circle = (Circle)sender;
      if (selectedCircle == circle)
      {
        Console.WriteLine("In_Table_MouseUp");
        if (goToSelectionPage)
        {
          DetermineGoingToSelectionPage(circle);
        }
        else
        {
          ChangeZIndex(circle, 2);
          if (!((Circle)sender).Added)
          {
            if (circle.circleUI.Fill == redBrush)//((SolidColorBrush)((Circle)sender).circleUI.Fill).Color == Colors.Red
            {
              canvas.Children.Remove(circle);
            }
            else //green color, then add the circle
            {
              //add the table affliated to this circle
              AddTableToCircle(circle);
              //CreateSelectionPageToCircle(circle);
            }
          }
          else //((Circle)sender).Added) which mean move a existed table
          {
            if (circle.circleUI.Fill == redBrush)//((SolidColorBrush)((Circle)sender).circleUI.Fill).Color == Colors.Red
            {
              RestoreOriginalCoordinate(circle);
              SolidYellowCircle(circle);
            }
            else if (circle.circleUI.Fill == greenBrush)//((SolidColorBrush)((Circle)sender).circleUI.Fill).Color == Colors.Green
            {
              DeleteOrRelocateTable(circle);
            }
            else //if yellow color
            {
              //do nothing
            }
          }
        }
        circle.ReleaseMouseCapture();
        goToSelectionPage = false;
        auxObject = null;
        selectedCircle = null;
        AddTouchListener(circle);
      }
    }

    private async void TableUI_TouchDownAsync(object sender, TouchEventArgs e)
    {
      Console.WriteLine("TableUI_TouchDownAsync");
      if (selectedCircle == null)
      {
        Console.WriteLine("In_TableUI_TouchDownAsync");
        Circle circle = (Circle)sender;
        RemoveMouseListener(circle);
        selectedCircle = circle;
        auxObject = new object();
        Object auxObject2 = auxObject;
        Console.WriteLine("goToSelectionPage--5");
        goToSelectionPage = true;

        //hold for 1 second, if mouse le
        await HoldDelay();

        if (auxObject == auxObject2)//circle.AreAnyTouchesDirectlyOver &&
        {
          Console.WriteLine("goToSelectionPage--6");
          goToSelectionPage = false;

          ChangeZIndex(circle, 3);
          previousPt.X = (Double)(circle.GetValue(Canvas.LeftProperty));
          previousPt.Y = (Double)(circle.GetValue(Canvas.TopProperty));

          pointList.Remove(new Point(previousPt.X, previousPt.Y));

          xDistance = e.GetTouchPoint(canvas).Position.X - previousPt.X;
          yDistance = e.GetTouchPoint(canvas).Position.Y - previousPt.Y;

          circle.Opacity = 0.5;
          circle.circleUI.Fill = greenBrush;
          //((SolidColorBrush)circle.circleUI.Fill).Color = Colors.Green;
          circle.CaptureTouch(e.TouchDevice);
        }
      }


    }

    private void TableUI_TouchLeave(object sender, TouchEventArgs e)
    {
      Console.WriteLine("TableUI_TouchLeave");
      Circle circle = (Circle)sender;
      if (selectedCircle == circle)
      {
        Console.WriteLine("In_TableUI_TouchLeave");
        auxObject = null;
        Console.WriteLine("goToSelectionPage--7");
        goToSelectionPage = false;
        selectedCircle = null;
        AddMouseListener(circle);
      }
    }

    private void TableUI_TouchMove(object sender, TouchEventArgs e)
    {

      Circle circle = (Circle)sender;
      if (circle.AreAnyTouchesCaptured)
      {
        Point point = e.GetTouchPoint(canvas).Position;

        circle.SetValue(Canvas.LeftProperty, point.X - xDistance);//Canvas.LeftProperty
        circle.SetValue(Canvas.TopProperty, point.Y - yDistance);

        point.X = point.X - xDistance;
        point.Y = point.Y - yDistance;

        if (!CoordinateConflict(circle, point))
        {
          circle.circleUI.Fill = greenBrush;
          //((SolidColorBrush)((Circle)sender).circleUI.Fill).Color = Colors.Green;
        }
        else
        {
          circle.circleUI.Fill = redBrush;
          //((SolidColorBrush)((Circle)sender).circleUI.Fill).Color = Colors.Red;
        }
      }

    }

    private void TableUI_TouchUp(object sender, TouchEventArgs e)
    {
      Console.WriteLine("TableUI_TouchUp");
      Circle circle = (Circle)sender;
      if (selectedCircle == circle)
      {
        Console.WriteLine("In_TableUI_TouchUp");
        if (goToSelectionPage)
        {
          DetermineGoingToSelectionPage(circle);
        }
        else
        {
          ChangeZIndex(circle, 2);
          if (!((Circle)sender).Added)
          {
            if (circle.circleUI.Fill == redBrush)//((SolidColorBrush)((Circle)sender).circleUI.Fill).Color == Colors.Red
            {
              canvas.Children.Remove(circle);
            }
            else //green color, then add the circle
            {
              //add the table affliated to this circle
              AddTableToCircle(circle);
              //CreateSelectionPageToCircle(circle);
            }

          }
          else //((Circle)sender).Added) which mean move a existed table
          {
            if (circle.circleUI.Fill == redBrush)//((SolidColorBrush)((Circle)sender).circleUI.Fill).Color == Colors.Red
            {
              RestoreOriginalCoordinate(circle);
              SolidYellowCircle(circle);
            }
            else if (circle.circleUI.Fill == greenBrush)//((SolidColorBrush)((Circle)sender).circleUI.Fill).Color == Colors.Green
            {
              DeleteOrRelocateTable(circle);
            }
            else //if yellow color
            {
              //do nothing
            }
          }
        }

        circle.ReleaseMouseCapture();
        goToSelectionPage = false;
        auxObject = null;
        selectedCircle = null;
        AddMouseListener(circle);
      }

    }

    /*
     * circle is the moving circle object
     * point is the upper left point of moving cirlce
     * 
     * AllowRelease composed of 2 parts
     * Part1) check if circle move out of boundry
     * Part2) check if overlap with other circles
     */
    public bool CoordinateConflict(Circle circle, Point upperLeftPoint)
    {

      if (upperLeftPoint.X < 0 || upperLeftPoint.X + diameter > canvas.Width || upperLeftPoint.Y < 0 || upperLeftPoint.Y + diameter > canvas.Height)
      {
        return true;
      }

      if (!circle.Added || circle.Table.IsActive)
      {
        pointList.Add(deleteButtonAbsolutePoint);
      }

      foreach (Point otherPoint in pointList)
      {
        if (Math.Sqrt(Math.Pow(upperLeftPoint.X - otherPoint.X, 2) + Math.Pow(upperLeftPoint.Y - otherPoint.Y, 2)) < diameter)
        {
          pointList.Remove(deleteButtonAbsolutePoint);
          return true;
        }
      }

      // will remove deleteButton center if exist. Will do nothing if deleteButton not exist
      pointList.Remove(deleteButtonAbsolutePoint);
      return false;
    }

    private Point AbsoluteToRelative(Point point)
    {
      return new Point(point.X / canvasDimension, point.Y / canvasDimension);
    }

    private Point RelativeToAbsolute(Point point)
    {
      return new Point(point.X * canvasDimension, point.Y * canvasDimension);
    }

    private void AddNewCoordinate(Circle circle)
    {
      Double x = (Double)circle.GetValue(Canvas.LeftProperty);
      Double y = (Double)circle.GetValue(Canvas.TopProperty);

      Point newAbsolutePoint = new Point(x, y);
      Point newRelativePoint = AbsoluteToRelative(newAbsolutePoint);

      pointList.Add(newAbsolutePoint);
      circle.Table.UpperLeftPoint = newRelativePoint;
      Console.WriteLine("added-x:{0} added-y:{1}", newRelativePoint.X, newRelativePoint.Y);
    }

    private void SolidYellowCircle(Circle circle)
    {
      circle.circleUI.Fill = yellowBrush;
      circle.Opacity = 1;
    }

    private void ChangeZIndex(Circle circle, int i)
    {
      circle.SetValue(Panel.ZIndexProperty, i);
    }

    private async Task HoldDelay()
    {
      await Task.Delay(1000);
    }

    /*
     * 1) restore circle to its original coordinate
     * 2) add the coordinate of the circle to pointList
     */
    private void RestoreOriginalCoordinate(Circle circle)
    {
      circle.SetValue(Canvas.LeftProperty, previousPt.X);
      circle.SetValue(Canvas.TopProperty, previousPt.Y);
      Point newAbsolutePoint = new Point((double)circle.GetValue(Canvas.LeftProperty), (double)circle.GetValue(Canvas.TopProperty));
      pointList.Add(newAbsolutePoint);
    }

    private void AddListener(Circle tableUI)
    {
      tableUI.MouseMove += Table_MouseMove;
      tableUI.MouseUp += Table_MouseUp;
      tableUI.MouseDown += Table_MouseDownAsync;
      tableUI.MouseLeave += Table_MouseLeave;

      tableUI.TouchDown += TableUI_TouchDownAsync;
      tableUI.TouchMove += TableUI_TouchMove;
      tableUI.TouchLeave += TableUI_TouchLeave;
      tableUI.TouchUp += TableUI_TouchUp;
    }

    private void RemoveListeners(Circle tableUI)
    {
      tableUI.MouseMove += Table_MouseMove;
      tableUI.MouseUp += Table_MouseUp;
      tableUI.MouseDown += Table_MouseDownAsync;
      tableUI.MouseLeave += Table_MouseLeave;

      tableUI.TouchDown -= TableUI_TouchDownAsync;
      tableUI.TouchMove -= TableUI_TouchMove;
      tableUI.TouchLeave -= TableUI_TouchLeave;
      tableUI.TouchUp -= TableUI_TouchUp;
    }

    private void AddMouseListener(Circle tableUI)
    {
      tableUI.MouseMove += Table_MouseMove;
      tableUI.MouseUp += Table_MouseUp;
      tableUI.MouseDown += Table_MouseDownAsync;
      tableUI.MouseLeave += Table_MouseLeave;
    }

    private void AddTouchListener(Circle tableUI)
    {
      tableUI.TouchDown += TableUI_TouchDownAsync;
      tableUI.TouchMove += TableUI_TouchMove;
      tableUI.TouchLeave += TableUI_TouchLeave;
      tableUI.TouchUp += TableUI_TouchUp;
    }

    private void RemoveMouseListener(Circle tableUI)
    {
      tableUI.MouseMove -= Table_MouseMove;
      tableUI.MouseUp -= Table_MouseUp;
      tableUI.MouseDown -= Table_MouseDownAsync;
      tableUI.MouseLeave -= Table_MouseLeave;
    }

    private void RemoveTouchListeners(Circle tableUI)
    {
      tableUI.TouchDown -= TableUI_TouchDownAsync;
      tableUI.TouchMove -= TableUI_TouchMove;
      tableUI.TouchLeave -= TableUI_TouchLeave;
      tableUI.TouchUp -= TableUI_TouchUp;
    }



    private void DetermineGoingToSelectionPage(Circle tableUI)
    {
      if (tableUI.Table.IsActive == true)
      {
        mainWindow.selectionPageTab.IsEnabled = true;
        GoToSelectionPage(tableUI);
      }
      else
      {
        YesNoCancelDialog yesNoCancelDialog = new YesNoCancelDialog("A new Session for Table "+tableUI.Table.TableNumber);
        if (yesNoCancelDialog.ShowDialog() == true)
        {
          tableUI.Table.IsActive = true;
          tableUI.circleUI.Stroke = greenBrush;
          mainWindow.selectionPageTab.IsEnabled = true;
          GoToSelectionPage(tableUI);
        }
      } 
    }

    private void GoToSelectionPage(Circle tableUI)
    {
      mainWindow.itemsSelectionPage.tableUI = tableUI;
      mainWindow.tabControl.SelectedItem = mainWindow.selectionPageTab;
    }


    private void AddTableToCircle(Circle circle)
    {
      Models.Table table = new Models.Table();
      circle.Table = table;

      FillCircleTableNumber(circle);

      tablesList.Add(table);

      AddNewCoordinate(circle);
      SolidYellowCircle(circle);
      circle.Added = true;
    }

    private void DeleteOrRelocateTable(Circle circle)
    {
      Double x = (Double)circle.GetValue(Canvas.LeftProperty);
      Double y = (Double)circle.GetValue(Canvas.TopProperty);

      if (Math.Sqrt(Math.Pow(x - deleteButtonAbsolutePoint.X, 2) + Math.Pow(y - deleteButtonAbsolutePoint.Y, 2)) < diameter)
      {
        YesNoCancelDialog yesNoCancelDialog = new YesNoCancelDialog("Do You want to DELETE this Table");
        if (yesNoCancelDialog.ShowDialog() == true)
        {
          RemoveTable(circle);
        }
        else
        {
          RestoreOriginalCoordinate(circle);
          SolidYellowCircle(circle);
        }
      }
      else //green successfuly move the circle
      {
        AddNewCoordinate(circle);
        SolidYellowCircle(circle);
      }
    }

    private void RemoveTable(Circle circle)
    {
      canvas.Children.Remove(circle);
      tableNumberBooleanList[circle.Table.TableNumber - 1] = false;
      tablesList.Remove(circle.Table);
    }

    private void FillCircleTableNumber(Circle circle)
    {
      bool fillNumberGap = false;
      for (int i = 0; i < tableNumberBooleanList.Count; i++)
      {
        if (!tableNumberBooleanList[i])
        {
          tableNumberBooleanList[i] = true;
          fillNumberGap = true;
          circle.Table.TableNumber = i + 1;
          circle.NumberTextBlockText = (i + 1).ToString();
          break;
        }
      }
      if (!fillNumberGap)
      {
        tableNumberBooleanList.Add(true);
        circle.Table.TableNumber = tableNumberBooleanList.Count;
        circle.NumberTextBlockText = tableNumberBooleanList.Count.ToString();
      }
    }

    internal void LoadTablesUI()
    {
      foreach (Models.Table table in tablesList)
      {
        //logic of tableUI
        Circle tableUI = new Circle();
        tableUI.Table = table;
        tableUI.Added = true;
        //CreateSelectionPageToCircle(tableUI);

        //apperance of the tableUI
        tableUI.circleUI.Width = diameter;
        tableUI.circleUI.Height = diameter;
        tableUI.circleUI.Fill = yellowBrush;
        if (tableUI.Table.IsActive)
        {
          tableUI.circleUI.Stroke = greenBrush;
        }
        tableUI.Opacity = 1;
        tableUI.NumberTextBlockText = tableUI.Table.TableNumber.ToString();
     

        //add listeners to tableUI
        AddListener(tableUI);

        //show on canvas
        Point absolutePoint = RelativeToAbsolute(table.UpperLeftPoint);
        tableUI.SetValue(Canvas.LeftProperty, absolutePoint.X);
        tableUI.SetValue(Canvas.TopProperty, absolutePoint.Y);
        Console.WriteLine("x:{0} y:{1}",absolutePoint.X,absolutePoint.Y);
        canvas.Children.Add(tableUI);

        pointList.Add(absolutePoint);

      }
    }
  }
}
