using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace polygon
{
    
    public partial class Form1 : Form
    {
        //显示在坐标以及储存的坐标都是左下坐标系，
        //在储存点的时候会对点的坐标进行计算，绘图的时候再变换回来
        public Form1()
        {
            InitializeComponent();
        }

        List<List<Edge>> ET = new List<List<Edge>>();

        //储存边的表
        List<Edge> Edge_list = new List<Edge>();

        //边的活化链表
        List<Edge> AEL = new List<Edge>();

        //全局变量用来储存点击的点
        List<Point> point_list = new List<Point>();

        //标记鼠标的位置，记录鼠标移动的时候鼠标的位置
        Point mouse_position;

        //标记是否需要响应鼠标移动的事件
        bool mouse_move = false;

        //用来判断是否进行过双击
        bool double_click = false;

        //获取有边的最大和最小y坐标
        int y_min = 0;
        int y_max = 0;

        //用来储存在填充的时候扫描线和边的交点在排序前的横坐标
        List<double> x_list = new List<double>();
        List<double> x_list_sort = new List<double>();

        //传入纵坐标和横坐标的序列，进行填充，在原图层上不覆盖
        //x_list_sort的数目必须是偶数
        public void draw(int y,List<double> x_list_sort,Bitmap image)
        {
            //从picturebox中创建画笔
            pictureBox1.Image = image;
            Graphics g = Graphics.FromImage(image);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            Pen mypen = new Pen(Color.FromArgb(255, 0, 0, 255));
             
            //两两配对绘制连线
            for(int i=0;i<x_list_sort.Count;i=i+2)
            {
                Point point1 = new Point((int)x_list_sort[i], pictureBox1.Height - y);
                Point point2 = new Point((int)x_list_sort[i + 1], pictureBox1.Height - y);
                g.DrawLine(mypen, point1, point2);
            }
        }
    
        //鼠标点击就记录点击的坐标
        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            
            if(!double_click)
            {
                //鼠标响应事件开启
                mouse_move = true;

                //变换坐标
                mouse_position = e.Location;
                mouse_position.Y = pictureBox1.Height - mouse_position.Y;
                point_list.Add(mouse_position);
            }
            
        }

        //实现橡皮筋的效果，当鼠标移动时不断更新bitmap
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            //如果需要更新鼠标位置
            if(mouse_move)
            {
                //更新鼠标位置
                mouse_position = e.Location;
                mouse_position.Y = pictureBox1.Height - mouse_position.Y;

                //绘制连线
                rubber();
            }
            label1.Text = "提示:双击结束输入,Backspace键回退.想要再画一次或者重新开始请点击按钮" + "   " + "X:" + e.X.ToString() + " " + "Y:" + (pictureBox1.Height-e.Y).ToString();
        }

        //双击结束，开始填充
        private void pictureBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //鼠标响应事件关闭
            mouse_move = false;

            //双击事件为真
            double_click = true;

            //开始填充
            //从picturebox中创建画笔
            Bitmap image = new Bitmap( pictureBox1.Width,pictureBox1.Height);
            pictureBox1.Image = image;

            //绘制图形
            rubber(image);
            fill_polygon(image);
        }

        //按了backspace键则消除一个点
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Back)
            {
                if(point_list.Count>0)
                {
                    point_list.RemoveAt(point_list.Count - 1);
                }

                //更新图层
                rubber();
            }
        }

        //创建新图层，绘制连线
        //重载函数，一个传入bitmap绘图，另一个不需要
        //为了在双击后绘图
        public void rubber(Bitmap image)
        {
            //创建bitmap
            pictureBox1.Image = image;
            Graphics g = Graphics.FromImage(image);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            Pen mypen = new Pen(Color.FromArgb(255, 0, 0, 0));

            //如果列表里只有一个点
            if (point_list.Count == 1)
            {
                //则绘制第一个点到鼠标的连线
                Point point1 = point_list[0];
                Point point2 = mouse_position;

                //将坐标变换到原来的坐标系
                point1.Y = pictureBox1.Height - point1.Y;
                point2.Y = pictureBox1.Height - point2.Y;

                g.DrawLine(mypen, point1, point2);
            }

            //如果有多个点
            else if (point_list.Count >= 2)
            {
                Point point1 = new Point();
                Point point2 = new Point();
                //先将列表中的点按照顺序连接起来
                for (int i = 0; i < point_list.Count - 1; i++)
                {
                    point1 = point_list[i];
                    point2 = point_list[i + 1];

                    //将坐标变换到原来的坐标系
                    point1.Y = pictureBox1.Height - point1.Y;
                    point2.Y = pictureBox1.Height - point2.Y;

                    g.DrawLine(mypen, point1, point2);
                }

                //再将首位两个点和鼠标位置的点相连
                point1 = point_list[0];
                point2 = point_list[point_list.Count - 1];
                Point point3 = mouse_position;

                //将坐标变换到原来的坐标系
                point1.Y = pictureBox1.Height - point1.Y;
                point2.Y = pictureBox1.Height - point2.Y;
                point3.Y = pictureBox1.Height - point3.Y;
                g.DrawLine(mypen, point1, point3);
                g.DrawLine(mypen, point2, point3);
            }

        } 

        //创建新图层，绘制连线
        //为了在移动的时候绘图
        public void rubber()
        {
            //创建bitmap
            Bitmap image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox1.Image = image;
            Graphics g = Graphics.FromImage(image);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            Pen mypen = new Pen(Color.FromArgb(255, 0, 0, 0));

            //如果列表里只有一个点
            if (point_list.Count == 1)
            {
                //则绘制第一个点到鼠标的连线
                Point point1 = point_list[0];
                Point point2 = mouse_position;

                //将坐标变换到原来的坐标系
                point1.Y = pictureBox1.Height - point1.Y;
                point2.Y = pictureBox1.Height - point2.Y;

                g.DrawLine(mypen, point1, point2);
            }

            //如果有多个点
            else if (point_list.Count >= 2)
            {
                Point point1 = new Point();
                Point point2 = new Point();
                //先将列表中的点按照顺序连接起来
                for (int i = 0; i < point_list.Count - 1; i++)
                {
                    point1 = point_list[i];
                    point2 = point_list[i+1];

                    //将坐标变换到原来的坐标系
                    point1.Y = pictureBox1.Height - point1.Y;
                    point2.Y = pictureBox1.Height - point2.Y;

                    g.DrawLine(mypen, point1, point2);
                }

                //再将首位两个点和鼠标位置的点相连
                point1 = point_list[0];
                point2 = point_list[point_list.Count - 1];
                Point point3 = mouse_position;

                //将坐标变换到原来的坐标系
                point1.Y = pictureBox1.Height - point1.Y;
                point2.Y = pictureBox1.Height - point2.Y;
                point3.Y = pictureBox1.Height - point3.Y;
                g.DrawLine(mypen, point1, point3);
                g.DrawLine(mypen, point2, point3);
            }
        } 

        //点击初始化按钮，则清空画布，清空列表，还原属性
        private void button1_MouseClick(object sender, MouseEventArgs e)
        {
            //创建bitmap,清空画布
            Bitmap image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox1.Image = image;

            //还原属性
            mouse_move = false;

            //清空列表
            point_list.Clear();
            x_list.Clear();
            x_list_sort.Clear();
            ET.Clear();
            AEL.Clear();
            Edge_list.Clear();
            double_click = false;

            //还原标签
            label1.Text = "提示:双击结束输入,Backspace键回退.想要再画一次或者重新开始请点击按钮" + "   " + "X: " + " " + "Y: ";
        }

        //自定义的对Edge排序的算法,对y_min进行升序排列
        public int compare_ymin(Edge a, Edge b)
        {
            //如果一样大就返回0
            if(a.y_min==b.y_min)
            {
                return 0;
            }
            //前面比后面大就返回1
            else if(a.y_min>b.y_min)
            {
                return 1;
            }
            //前面比后面小就返回-1
            else
            {
                return -1;
            }
        }

        //多边形填充算法，在双击后调用此函数
        public void fill_polygon(Bitmap image)
        {
            y_min = point_list[0].Y;
            y_max = point_list[0].Y;
            //先将所有的边存入Edge_list中
            for (int i=0;i<point_list.Count-1;i++)
            {
                //更新最大最小纵坐标
                y_min = Math.Min(y_min, point_list[i].Y);
                y_max = Math.Max(y_max, point_list[i].Y);

                Edge one_edge = new Edge();
                //x为下端点的横坐标，需要先判断那个点是下端点
                if (point_list[i].Y > point_list[i + 1].Y)
                {
                    one_edge.x = point_list[i + 1].X;
                }
                else if (point_list[i].Y < point_list[i + 1].Y)
                {
                    one_edge.x = point_list[i].X;
                }
                //如果这条线是水平的则不存入EdgeTable中
                else if (point_list[i].Y == point_list[i + 1].Y)
                {
                    continue;
                }
                //y_max为上端点的y坐标
                one_edge.y_max = Math.Max(point_list[i].Y, point_list[i + 1].Y);
                //y_min为下端点的y坐标
                one_edge.y_min = Math.Min(point_list[i].Y, point_list[i + 1].Y);

                //delta_x
                if (point_list[i].Y > point_list[i + 1].Y)
                {
                    one_edge.delta_x = (double)(point_list[i].X - point_list[i + 1].X) / (double)(point_list[i].Y - point_list[i + 1].Y);
                }
                else
                {
                    one_edge.delta_x = (double)(point_list[i + 1].X - point_list[i].X) / (double)(point_list[i + 1].Y - point_list[i].Y);
                }
                //加入列表中
                Edge_list.Add(one_edge);
            }

            //最后首位相连的边
            //更新最大最小纵坐标
            y_min = Math.Min(y_min, point_list[point_list.Count-1].Y);
            y_max = Math.Max(y_max, point_list[point_list.Count - 1].Y);
            Edge last = new Edge();
            if(point_list[0].Y==point_list[point_list.Count-1].Y)
            {
                //水平的线不要
            }

            //按照上端点的不同分两种情况
            else if(point_list[0].Y> point_list[point_list.Count - 1].Y)
            {
                last.x = point_list[point_list.Count - 1].X;

                //y_max为上端点的y坐标
                last.y_max = Math.Max(point_list[0].Y, point_list[point_list.Count - 1].Y);
                //y_min为下端点的y坐标
                last.y_min = Math.Min(point_list[0].Y, point_list[point_list.Count - 1].Y);

                //delta_x
                last.delta_x = (double)(point_list[0].X - point_list[point_list.Count - 1].X) / (double)(point_list[0].Y - point_list[point_list.Count - 1].Y);
               
                //加入列表中
                Edge_list.Add(last);

            }
            else if(point_list[0].Y < point_list[point_list.Count - 1].Y)
            {
                last.x = point_list[0].X;

                //y_max为上端点的y坐标
                last.y_max = Math.Max(point_list[0].Y, point_list[point_list.Count - 1].Y);
                //y_min为下端点的y坐标
                last.y_min = Math.Min(point_list[0].Y, point_list[point_list.Count - 1].Y);


                //delta_x
                last.delta_x = (double)(point_list[point_list.Count - 1].X - point_list[0].X) / (double)(point_list[point_list.Count - 1].Y - point_list[0].Y);
              
                //加入列表中
                Edge_list.Add(last);
            }
           
            //所有的边已经生成，需要对其按照下端点坐标进行分类，将下端点坐标相同的分到同一类
            //先对下端点的y坐标进行排序，这样同一类的点会被排在相邻的位置
            Edge_list.Sort(compare_ymin);

            //遍历列表，将下端点y坐标相同的点加入同一个数组里，并将这个数组压入ET中
            for(int i=0;i<Edge_list.Count;i++)
            {
                List<Edge> one_list = new List<Edge>();
                one_list.Add(Edge_list[i]);

                //如果下一个和这一个y_min相同的话则不断加入新的边，直到不相同
                if(i<Edge_list.Count-1) //后面还有一条边就判断是否属于一个类，没有的话直接加入ET就行
                {
                    if (Edge_list[i + 1].y_min == Edge_list[i].y_min)
                    {
                        while (Edge_list[i + 1].y_min == Edge_list[i].y_min)
                        {
                            one_list.Add(Edge_list[i + 1]);
                            i++;
                            //如果超出了数组范围就跳出
                            if (i == Edge_list.Count - 1)
                            {
                                break;
                            }
                        }
                    }
                }
                //将这一类的边加入ET中
                ET.Add(one_list);
            }
           

            //开始填充算法
            for(int i=y_min;i<=y_max;i++) 
            {
                //将ET表中该类边都提取出来，添加到AEL中
                for(int j=0;j<ET.Count;j++)
                {
                    if(ET[j][0].y_min==i)
                    {
                        //将边添加到活化链表里
                        AEL = AEL.Union(ET[j]).ToList<Edge>();

                        //将下端点的坐标加入列表中作为初始值
                        for (int k=0;k<ET[j].Count;k++)
                        {
                            x_list.Add(ET[j][k].x);
                        }
                        break; //跳出
                    }
                }

                //对交点进行排序后开始画图,进行着色
                if(x_list.Count>0)
                {
                    x_list_sort.Clear();
                    //因为引用赋值的话会有问题，所以采用这种循环的方式赋值
                    for(int m=0;m<x_list.Count;m++)
                    {
                        double x = x_list[m];
                        x_list_sort.Add(x);
                    }
                    x_list_sort.Sort();
                    draw(i, x_list_sort,image);
                }

                //删除边,遍历AEL，如果i+1等于某条边的y_max则删去这条边，同时删去对应的x
                int num = AEL.Count;
                int remove_num = 0;
                for (int j = 0; j < num; j++)
                {
                    if (i + 1 == AEL[j-remove_num].y_max)
                    {
                        AEL.RemoveAt(j - remove_num);
                        x_list.RemoveAt(j - remove_num);
                        remove_num++;
                    }
                }

                //更新交点坐标
                if(x_list.Count>0)
                {
                    for (int j = 0; j < x_list.Count; j++)
                    {
                        x_list[j] = x_list[j] + AEL[j].delta_x;
                    }
                }
            }
        }
    }
}
