using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Cellular_Automaton_Logic;

namespace My_Controls
{
    public abstract class My_Control : Control
    {
        protected My_Control()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint, true);
            DoubleBuffered = true;
            Font = new Font("GOST type A", 20, FontStyle.Bold);
        }
        public virtual void replace(int x_l, int y_l, int x_s, int y_s)
        {
            Location = new Point(x_l, y_l);
            Size = new Size(x_s, y_s);
            Invalidate();
        }
        protected Font binary_font_search(int i1, int i2, int x, int y, Font f, string t)
        {
            if (Math.Abs(i1 - i2) < 2)
                return f;
            f = new Font(f.FontFamily, (i1 + i2) / 2, f.Style);
            Size s = TextRenderer.MeasureText(t, f);
            if (s.Width >= x || s.Height >= y)
                return binary_font_search(Math.Min(i1, i2), (i1 + i2) / 2, x, y, f, t);
            if (Math.Min(10000 * (x - s.Width) / x, 10000 * (y - s.Height) / y) > 500)
                return binary_font_search((i1 + i2) / 2, Math.Max(i1, i2), x, y, f, t);
            return f;
        }
    }
    //
    public abstract class Click_Control : My_Control
    {
        protected bool pressed = false;
        private string text;
        public override string Text
        {
            get { return text; }
            set
            {
                text = value;
                Set_correct_Font();
            }
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            pressed = true;
            Invalidate();
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            pressed = false;
            Invalidate();
        }
        protected virtual void Set_correct_Font()
        {
            if (Text != null && Text.Length != 0)
            {
                Font = binary_font_search(1, 500, 85 * Width / 100, 85 * Height / 100, Font, Text);
            }
        }
    }
    //
    public abstract class My_Button : Click_Control
    {
        protected StringFormat s_f = new StringFormat();
        protected Color Border_color = Color.Black;
        public My_Button()
        {
            s_f.Alignment = StringAlignment.Center;
            s_f.LineAlignment = StringAlignment.Center;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics graf = e.Graphics;
            graf.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            graf.Clear(Parent.BackColor);
            Rectangle rect;
            if (pressed)
            {
                Draw_top_side_of_button(graf, 3, Height - 3);
                rect = new Rectangle(0, 3, Width, Height - 3);
            }
            else
            {
                Draw_top_side_of_button(graf, 0, Height - 3);
                graf.DrawRectangle(new Pen(Border_color, 3), 0, Height - 3, Width - 1, 3);
                rect = new Rectangle(0, 0, Width, Height);
            }
            if (!Enabled)
            {
                graf.FillRectangle(new SolidBrush(Color.FromArgb(125, 0, 0, 0)), 0, 0, Width, Height);
            }
            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddRectangle(rect);
            Region = new Region(path);
        }
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            Set_correct_Font();
        }
        protected abstract void Draw_top_side_of_button(Graphics graf, int start_Height_point, int height);
    }
    public class Menu_Button : My_Button
    {
        public Menu_Button(int x_l, int y_l, int x_s, int y_s, Color back_color, string text)
        {
            replace(x_l, y_l, x_s, y_s);
            BackColor = back_color;
            Text = text;
        }
        public Menu_Button(int x_l, int y_l, int x_s, int y_s, Color back_color, Color border_color, string text) : this(x_l, y_l, x_s, y_s, back_color, text)
        {
            Border_color = border_color;
        }
        protected override void Set_correct_Font()
        {
            if (Text != null && Text.Length != 0)
            {
                Font = binary_font_search(1, 500, 9 * Width / 10, 7 * Height / 10, Font, Text);
            }
        }
        protected override void Draw_top_side_of_button(Graphics graf, int start_Height_point, int height)
        {
            graf.FillRectangle(new SolidBrush(BackColor), 0, start_Height_point, Width, height);
            if (pressed)
            {
                graf.FillRectangle(new SolidBrush(Color.FromArgb(127, 0, 0, 0)), 0, start_Height_point, Width, height / 2);
            }
            else
            {
                graf.FillRectangle(new SolidBrush(Color.FromArgb(127, 0, 0, 0)), 0, start_Height_point + height / 2, Width, height / 2);
            }
            graf.DrawRectangle(new Pen(Border_color, 3), 0, start_Height_point, Width - 1, height - 1);
            if (BackColor.R + BackColor.G + BackColor.B < Math.Sqrt(255) * 9)
                graf.DrawString(Text, Font, new SolidBrush(Color.White), Width / 2, start_Height_point + height / 2, s_f);
            else
                graf.DrawString(Text, Font, new SolidBrush(Color.Black), Width / 2, start_Height_point + height / 2, s_f);
        }
    }
    public class Level_Button : My_Button
    {
        public Level_Button(int x_l, int y_l, int x_s, int y_s, Color back_color, string text) : base()
        {
            replace(x_l, y_l, x_s, y_s);
            BackColor = back_color;
            Text = text;
        }
        public Level_Button(int x_l, int y_l, int x_s, int y_s, Color back_color, Color border_color, string text) : this(x_l, y_l, x_s, y_s, back_color, text)
        {
            Border_color = border_color;
        }
        protected override void Draw_top_side_of_button(Graphics graf, int start_Height_point, int height)
        {
            graf.FillRectangle(new SolidBrush(BackColor), 0, start_Height_point, Width, height);
            graf.FillPolygon(new SolidBrush(Color.FromArgb(127, 0, 0, 0)), new PointF[]
            {
                new PointF(0, start_Height_point),
                new PointF(Math.Min(Width, height) / 6, start_Height_point + Math.Min(Width, height) / 6),
                new PointF(Math.Min(Width, height) / 6, start_Height_point + height - Math.Min(Width, height) / 6),
                new PointF(0, start_Height_point + height)
            });
            graf.FillPolygon(new SolidBrush(Color.FromArgb(127, 0, 0, 0)), new PointF[]
            {
                new PointF(Width, start_Height_point),
                new PointF(Width - Math.Min(Width, height) / 6, start_Height_point + Math.Min(Width,height) / 6),
                new PointF(Width - Math.Min(Width, height) / 6, start_Height_point + height - Math.Min(Width,height) / 6),
                new PointF(Width, start_Height_point + height)
            });
            if (pressed)
            {
                graf.FillPolygon(new SolidBrush(Color.FromArgb(192, 0, 0, 0)), new PointF[]
                {
                    new PointF(0, start_Height_point),
                    new PointF(Math.Min(Width,height) / 6, start_Height_point + Math.Min(Width,height) / 6),
                    new PointF(Width - Math.Min(Width,height) / 6, start_Height_point + Math.Min(Width,height) / 6),
                    new PointF(Width, start_Height_point)
                });
            }
            else
            {
                graf.FillPolygon(new SolidBrush(Color.FromArgb(192, 0, 0, 0)), new PointF[]
                {
                    new PointF(0, start_Height_point + height),
                    new PointF(Math.Min(Width,height) / 6, start_Height_point + height - Math.Min(Width,height) / 6),
                    new PointF(Width - Math.Min(Width,height) / 6, start_Height_point + height - Math.Min(Width,height) / 6),
                    new PointF(Width, start_Height_point + height)
                });
            }
            graf.FillPolygon(new SolidBrush(Color.FromArgb(64, 0, 0, 0)), new PointF[]
            {
                new PointF(Width - Math.Min(Width,height) / 6, start_Height_point + Math.Min(Width,height) / 6),
                new PointF(Math.Min(Width,height) / 6, start_Height_point + Math.Min(Width,height) / 6),
                new PointF(Math.Min(Width,height) / 6, start_Height_point + height - Math.Min(Width,height) / 6),
                new PointF(Width - Math.Min(Width,height) / 6, start_Height_point + height - Math.Min(Width,height) / 6)
            });
            graf.DrawRectangle(new Pen(Border_color, 3), 0, start_Height_point, Width - 1, height - 1);
            if (BackColor.R + BackColor.G + BackColor.B < Math.Sqrt(255) * 9)
                graf.DrawString(Text, Font, new SolidBrush(Color.White), Width / 2, start_Height_point + height / 2, s_f);
            else
                graf.DrawString(Text, Font, new SolidBrush(Color.Black), Width / 2, start_Height_point + height / 2, s_f);
        }
        protected override void Set_correct_Font()
        {
            if (Text != null && Text.Length != 0)
            {
                int i = Math.Min(45 * Width / 100, 45 * Height / 100);
                Font = binary_font_search(1, 500, Width - i, Height - i, Font, Text);
            }
        }
    }
    public class Color_Button : My_Button
    {
        public Color_Button(int x_l, int y_l, int x_s, int y_s, Color c)
        {
            BackColor = c;
            replace(x_l, y_l, x_s, y_s);
        }
        public Color_Button(int x_l, int y_l, int x_s, int y_s, Color b_c, Color c) : this(x_l, y_l, x_s, y_s, c)
        {
            Border_color = b_c;
        }
        protected override void Draw_top_side_of_button(Graphics graf, int start_Height_point, int height)
        {
            graf.Clear(BackColor);
            graf.FillRectangle(new SolidBrush(Color.FromArgb(127, 0, 0, 0)), 0, start_Height_point, Width, height);
            graf.FillRectangle(new SolidBrush(BackColor), height / 10, start_Height_point + height / 10, Width - height / 5, 4 * height / 5);
            if (pressed)
            {
                graf.FillRectangle(new SolidBrush(Color.FromArgb(127, 0, 0, 0)), 0, start_Height_point, Width, height);
            }
            graf.DrawRectangle(new Pen(Border_color, 3), 0, start_Height_point, Width - 1, height - 1);
        }
    }
    public class Text_Color_Button : Color_Button
    {
        public Text_Color_Button(int x_l, int y_l, int x_s, int y_s, Color c, string text) : base(x_l, y_l, x_s, y_s, c)
        {
            Text = text;
        }
        public Text_Color_Button(int x_l, int y_l, int x_s, int y_s, Color b_c, Color c, string text) : base(x_l, y_l, x_s, y_s, b_c, c)
        {
            Text = text;
        }
        protected override void Draw_top_side_of_button(Graphics graf, int start_Height_point, int height)
        {
            base.Draw_top_side_of_button(graf, start_Height_point, height);
            if (BackColor.R + BackColor.G + BackColor.B < Math.Sqrt(255) * 9)
                graf.DrawString(Text, Font, new SolidBrush(Color.White), Width / 2, start_Height_point + height / 2, s_f);
            else
                graf.DrawString(Text, Font, new SolidBrush(Color.Black), Width / 2, start_Height_point + height / 2, s_f);
        }
    }
    public class Start_Simulation_Button : My_Button
    {
        private bool start = false;
        public bool Start
        {
            get { return start; }
            protected set
            {
                start = value;
                Set_correct_Font();
            }
        }
        public override string Text
        {
            get {if (Start) return "СТОП"; else return "СТАРТ";}
            set { }
        }
        public Start_Simulation_Button(int x_l, int y_l, int x_s, int y_s)
        {
            replace(x_l, y_l, x_s, y_s);
        }
        public Start_Simulation_Button(int x_l, int y_l, int x_s, int y_s, Color border_color) : this(x_l, y_l, x_s, y_s)
        {
            Border_color = border_color;
        }
        protected override void Draw_top_side_of_button(Graphics graf, int start_Height_point, int height)
        {
            Color c;
            if (Start) c = Color.FromArgb(0,255,0); else c = Color.FromArgb(255,0,0);
            graf.Clear(c);
            graf.FillRectangle(new SolidBrush(Color.FromArgb(127, 0, 0, 0)), 0, start_Height_point, Width, height);
            graf.FillRectangle(new SolidBrush(c), height / 10, start_Height_point + height / 10, Width - height / 5, 4 * height / 5);
            if (pressed)
            {
                graf.FillRectangle(new SolidBrush(Color.FromArgb(127, 0, 0, 0)), 0, start_Height_point, Width, height);
            }
            graf.DrawRectangle(new Pen(Border_color, 3), 0, start_Height_point, Width - 1, height - 1);
            if (c.R + c.G + c.B < Math.Sqrt(255) * 9)
                graf.DrawString(Text, Font, new SolidBrush(Color.White), Width / 2, start_Height_point + height / 2, s_f);
            else
                graf.DrawString(Text, Font, new SolidBrush(Color.Black), Width / 2, start_Height_point + height / 2, s_f);
        }
        public virtual void to_default()
        {
            Start = false;
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            Start = !Start;
            base.OnMouseUp(e);
        }
    }
    public class Simulation_Speed_Button : My_Button
    {
        private int speed = 1;
        public int Speed
        {
            get { return speed; }
            protected set
            {
                speed = value;
                Set_correct_Font();
            }
        }
        public override string Text
        {
            get { return "x" + Speed; }
            set { }
        }
        public Simulation_Speed_Button(int x_l, int y_l, int x_s, int y_s)
        {
            replace(x_l, y_l, x_s, y_s);
        }
        public Simulation_Speed_Button(int x_l, int y_l, int x_s, int y_s, Color border_color) : this(x_l, y_l, x_s, y_s)
        {
            Border_color = border_color;
        }
        protected override void Draw_top_side_of_button(Graphics graf, int start_Height_point, int height)
        {
            graf.Clear(Color.Gray);
            graf.FillRectangle(new SolidBrush(Color.FromArgb(127, 0, 0, 0)), 0, start_Height_point, Width, height);
            graf.FillRectangle(new SolidBrush(Color.Gray), height / 10, start_Height_point + height / 10, Width - height / 5, 4 * height / 5);
            if (pressed)
            {
                graf.FillRectangle(new SolidBrush(Color.FromArgb(127, 0, 0, 0)), 0, start_Height_point, Width, height);
            }
            graf.DrawRectangle(new Pen(Border_color, 3), 0, start_Height_point, Width - 1, height - 1);
            graf.DrawString(Text, Font, new SolidBrush(Color.Black), Width / 2, start_Height_point + height / 2, s_f);
        }
        public virtual void to_default()
        {
            Speed = 1;
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            switch (Speed)
            {
                case 1:
                    Speed = 2;
                    break;
                case 2:
                    Speed = 5;
                    break;
                case 5:
                    Speed = 10;
                    break;
                default:
                    Speed = 1;
                    break;
            }
            base.OnMouseUp(e);
        }
    }
    //
    public class Message : Click_Control
    {
        protected StringFormat s_f = new StringFormat();
        public Message(int x_l, int y_l, int x_s, int y_s, Color back_color, string text)
        {
            BackColor = back_color;
            Text = text;
            s_f.Alignment = StringAlignment.Center;
            s_f.LineAlignment = StringAlignment.Center;
            replace(x_l, y_l, x_s, y_s);
            Text = text;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics graf = e.Graphics;
            graf.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            graf.FillRectangle(new SolidBrush(BackColor), 0, 0, Width, Height);
            graf.FillRectangle(new SolidBrush(Color.FromArgb(125, 0, 0, 0)), 0, 0, Width, Height);
            graf.FillRectangle(new SolidBrush(BackColor), 5, 5, Width - 10, Height - 10);
            if (BackColor.R + BackColor.G + BackColor.B < Math.Sqrt(255) * 9)
                graf.DrawString(Text, Font, new SolidBrush(Color.White), Width / 2, Height / 2, s_f);
            else
                graf.DrawString(Text, Font, new SolidBrush(Color.Black), Width / 2, Height / 2, s_f);
            if (pressed)
            {
                graf.FillRectangle(new SolidBrush(Color.FromArgb(125, 0, 0, 0)), 0, 0, Width, Height);
            }
        }
        protected override void Set_correct_Font()
        {
            if (Text != null && Text.Length != 0)
            {
                int i = Math.Min(Width / 10, Height / 10);
                Font = binary_font_search(1, 500, Width - i, Height - i, Font, Text);
            }
        }
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            Set_correct_Font();
        }
    }
    //
    public abstract class Visual_Map : Click_Control
    {
        public Map_Logic Map { protected set; get; }
        protected int startpointx;
        protected int startpointy;
        protected int minresolution;
        protected int resolution;
        protected int curent_zoom_cell_x_coord = 0;
        protected int curent_zoom_cell_y_coord = 0;
        protected int previous_cursor_x;
        protected int previous_cursor_y;
        public Visual_Map(Map_Logic map)
        {
            Map = map;
            replace(0, 0, 300, 300);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics graf = e.Graphics;
            graf.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            graf.Clear(Color.Black);
            drow_all(graf);
            if (minresolution != resolution)
                drow_zoom_all(graf);
        }
        protected virtual void drow_one(int x, int y, Graphics graf)
        {
            SolidBrush p = new SolidBrush(Map.color_of_cell(x, y));
            graf.FillRectangle(p, startpointx + resolution * x, startpointy + resolution * y, resolution - 1, resolution - 1);
            graf.FillRectangle(new SolidBrush(Color.FromArgb(125, 255, 255, 255)), startpointx + resolution * (x - 1) + 21 * resolution / 20 + resolution / 10, startpointy + resolution * (y - 1) + 21 * resolution / 20, 8 * resolution / 10 - 1, resolution / 10 - 1);
            graf.FillRectangle(new SolidBrush(Color.FromArgb(125, 0, 0, 0)), startpointx + resolution * (x - 1) + 21 * resolution / 20, startpointy + resolution * (y - 1) + 21 * resolution / 20 + resolution / 10, resolution / 10 - 1, 8 * resolution / 10 - 1);
        }
        protected virtual void drow_all(Graphics graf)
        {
            for (int i = 0; i < Map.x_size; i++)
                for (int j = 0; j < Map.y_size; j++)
                    drow_one(i, j, graf);
        }
        protected virtual void drow_zoom_one(int x, int y, Graphics graf)
        {
            int zoom_resolution = minresolution / 3;
            int startpointx = (Width - minresolution * Map.x_size) / 6;
            int startpointy = (Height - minresolution * Map.y_size) / 6;
            SolidBrush p = new SolidBrush(Color.FromArgb(150, Map.color_of_cell(x, y).R / 2, Map.color_of_cell(x, y).G / 2, Map.color_of_cell(x, y).B / 2));
            graf.FillRectangle(p, startpointx + zoom_resolution * x, startpointy + zoom_resolution * y, zoom_resolution, zoom_resolution);
        }
        protected virtual void drow_zoom_all(Graphics graf)
        {
            for (int i = 0; i < Map.x_size; i++)
                for (int j = 0; j < Map.y_size; j++)
                    drow_zoom_one(i, j, graf);
        }
        protected virtual void find_start_poins()
        {
            if (resolution == minresolution)
            {
                startpointx = (Width - minresolution * Map.x_size) / 2;
                startpointy = (Height - minresolution * Map.y_size) / 2;
            }
            else
            {
                if (PointToClient(Cursor.Position).X > 0 && PointToClient(Cursor.Position).X < Width && PointToClient(Cursor.Position).Y > 0 && PointToClient(Cursor.Position).Y < Height)
                {
                    startpointx = -resolution * curent_zoom_cell_x_coord + PointToClient(Cursor.Position).X - resolution / 2;
                    startpointy = -resolution * curent_zoom_cell_y_coord + PointToClient(Cursor.Position).Y - resolution / 2;
                }
            }
        }
        public override void replace(int xl, int yl, int xs, int ys)
        {
            try
            {
                double propotion = Convert.ToDouble(resolution) / Convert.ToDouble(minresolution);
                minresolution = Math.Min(xs / Map.x_size, ys / Map.y_size);
                resolution = Convert.ToInt32(minresolution * propotion);
            }
            catch 
            {
                minresolution = Math.Min(xs / Map.x_size, ys / Map.y_size);
                resolution = minresolution;
            }
            base.replace(xl, yl, xs, ys);
            find_start_poins();
            Invalidate();
        }
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (e.Delta > 0)
            {
                int x = (e.X - startpointx) / resolution, y = (e.Y - startpointy) / resolution;
                if (x >= 0 && x < Map.x_size && y >= 0 && y < Map.y_size)
                {
                    curent_zoom_cell_x_coord = x;
                    curent_zoom_cell_y_coord = y;
                }
                else
                    return;
                if (resolution * SystemInformation.MouseWheelScrollLines / 3 <= Math.Min(Width, Height))
                    resolution *= SystemInformation.MouseWheelScrollLines / 3;

            }
            else
            {
                if (resolution / SystemInformation.MouseWheelScrollLines * 3 >= minresolution)
                    resolution /= SystemInformation.MouseWheelScrollLines / 3;
                else
                    resolution = minresolution;
            }
            find_start_poins();
            Invalidate();
        }
        protected virtual int find_x_cursor_coord_on_map(MouseEventArgs e) 
        {
            return (e.X - startpointx) / resolution;
        }
        protected virtual int find_y_cursor_coord_on_map(MouseEventArgs e)
        {
            return (e.Y - startpointy) / resolution;
        }
    }
    public class Type_Visual_Map : Visual_Map
    {
        private Timer timer = new Timer();
        public bool simulation_enabled { get { return timer.Enabled; } set { timer.Enabled = value; } }
        public int simulation_speed { get { return 200 / timer.Interval; } set { if (value > 0) timer.Interval = 200 / value; } }
        private int current_type_of_cell;
        public int Current_type_of_cell { get { return current_type_of_cell; } set { if (Map.game_rules.is_this_right_cell(value)) current_type_of_cell = value; } }
        public bool show_move_number = true;
        public Type_Visual_Map(Map_Logic map) : base(map)
        {
            timer.Interval = 200;
            timer.Enabled = false;
            timer.Tick += new EventHandler(this.timer_Tick);
            Current_type_of_cell = Map.game_rules.default_cell_vaule();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics graf = e.Graphics;
            if (Map is Level_Logic)
            {
                Level_Logic level_map = Map as Level_Logic;
                if (level_map.is_it_solved)
                {
                    simulation_enabled = false;
                    graf.FillRectangle(new SolidBrush(Color.DarkGray), Width / 4 - 5, Height / 4 - 5, Width / 2 + 10, Height / 2 + 10);
                    graf.FillRectangle(new SolidBrush(Color.Gray), Width / 4, Height / 4, Width / 2, Height / 2);
                    StringFormat s_f = new StringFormat();
                    s_f.Alignment = StringAlignment.Center;
                    s_f.LineAlignment = StringAlignment.Center;
                    Set_correct_Font();
                    graf.DrawString("Рівень пройдено", Font, new SolidBrush(Color.Black), Width / 2, Height / 2, s_f);
                }
            }
            if (show_move_number)
            {
                Font Font_ = new Font("GOST type A", 30, FontStyle.Bold);
                Size len = TextRenderer.MeasureText(Convert.ToString(Map.move_number), Font_);
                graf.FillRectangle(new SolidBrush(Color.FromArgb(Color.Gray.R - 40, Color.Gray.G - 40, Color.Gray.B - 40)), Width - Convert.ToString(Map.move_number).Length * Font_.Size / 2 - 20, 10 - len.Height, Convert.ToString(Map.move_number).Length * Font_.Size / 2 + 20, len.Height * 2);
                graf.DrawRectangle(new Pen(Color.FromArgb(125, 0, 0, 0), 5), Width - Convert.ToString(Map.move_number).Length * Font_.Size / 2 - 20, 10 - len.Height, Convert.ToString(Map.move_number).Length * Font_.Size / 2 + 20, len.Height * 2);
                StringFormat s_f = new StringFormat();
                s_f.Alignment = StringAlignment.Near;
                s_f.LineAlignment = StringAlignment.Near;
                graf.DrawString(Convert.ToString(Map.move_number), Font_, new SolidBrush(Color.White), Width - Convert.ToString(Map.move_number).Length * 15 - 20, 10, s_f);
            }
        }
        protected virtual void timer_Tick(object sender, EventArgs e)
        {
            Map.make_move();
            Invalidate();
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (Map is Level_Logic)
            {
                Level_Logic level_map = Map as Level_Logic;
                if (level_map.is_it_solved)
                    return;
            }
            base.OnMouseDown(e);
            if (!timer.Enabled)
            {
                int x = find_x_cursor_coord_on_map(e), y = find_y_cursor_coord_on_map(e);
                if (!(x >= 0 && x < Map.x_size && y >= 0 && y < Map.y_size))
                    return;
                Map.make_change_cell(x, y, Current_type_of_cell);
                Invalidate();
            }
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (Map is Level_Logic)
            {
                Level_Logic level_map = Map as Level_Logic;
                if (level_map.is_it_solved)
                    return;
            }
            if (!timer.Enabled && pressed)
            {
                int x = find_x_cursor_coord_on_map(e), y = find_y_cursor_coord_on_map(e);
                if (!(x >= 0 && x < Map.x_size && y >= 0 && y < Map.y_size))
                    return;
                Map.make_change_cell(x, y, Current_type_of_cell);
                Invalidate();
            }
        }
        protected override void Set_correct_Font()
        {
            Font = binary_font_search(1, 500, 4 * Width / 10, 4 * Height / 10, Font, "Рівень пройдено");
        }
        public void check()
        {
            if (Map is Level_Logic)
            {
                Level_Logic level_map = Map as Level_Logic;
                level_map.check();
                if (level_map.is_it_solved)
                {
                    show_move_number = false;
                }
                Invalidate();
            }
        }
    }
    public class Accessibility_Visual_Map : Visual_Map
    {
        public bool current_type_of_cell = false;
        public Accessibility_Visual_Map(Level_Logic map) : base(map)
        {
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            int x = find_x_cursor_coord_on_map(e), y = find_y_cursor_coord_on_map(e);
            if (!(x >= 0 && x < Map.x_size && y >= 0 && y < Map.y_size))
                return;
            Level_Logic l = Map as Level_Logic;
            l.enable_table_cell(x, y, current_type_of_cell);
            Invalidate();
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            int x = find_x_cursor_coord_on_map(e), y = find_y_cursor_coord_on_map(e);
            if (!(x >= 0 && x < Map.x_size && y >= 0 && y < Map.y_size))
                return;
            Level_Logic l = Map as Level_Logic;
            l.enable_table_cell(x, y, current_type_of_cell);
            Invalidate();
        }
    }
    //
    public class Vertical_Scroll_Bar : Click_Control
    {
        private List<My_Control> elements;
        public List<My_Control> Elements
        {
            set
            {
                if (value != null)
                {
                    elements = new List<My_Control>();
                    highest_my_control = null;
                    lowest_my_control = null;
                    foreach (My_Control i in value)
                    {
                        if (highest_my_control == null || highest_my_control.Location.Y < i.Location.Y)
                        {
                            highest_my_control = i;
                        }
                        if (lowest_my_control == null || lowest_my_control.Location.Y > i.Location.Y)
                        {
                            lowest_my_control = i;
                        }
                        elements.Add(i);
                    }
                    find_position();
                }
            }
            get { return elements; }
        }
        private My_Control highest_my_control;
        private My_Control lowest_my_control;
        private int max_height;
        private int current_height;
        private int current_size;
        private int previous_height;
        public Vertical_Scroll_Bar(int xl, int yl, int xs, int ys, List<My_Control> el)
        {
            BackColor = SystemColors.Control;
            Elements = el;
            base.replace(xl, yl, xs, ys);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics graf = e.Graphics;
            graf.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            graf.FillRectangle(new SolidBrush(BackColor), 2 * Width / 5, 0, Width / 5, Height);
            graf.FillRectangle(new SolidBrush(Color.FromArgb(160, 0, 0, 0)), 2 * Width / 5, 0, Width / 5, Height);
            graf.FillRectangle(new SolidBrush(BackColor), 0, 0, Width - 1, Height / 50);
            graf.FillRectangle(new SolidBrush(Color.FromArgb(160, 0, 0, 0)), 0, 0, Width - 1, Height / 50);
            graf.FillRectangle(new SolidBrush(BackColor), 0, Height - Height / 50, Width - 1, Height);
            graf.FillRectangle(new SolidBrush(Color.FromArgb(160, 0, 0, 0)), 0, Height - Height / 50, Width - 1, Height);

            int border = Math.Min(Width / 5, Height * current_size / (max_height + current_size) / 5), start = Height * current_height / (max_height + current_size);
            graf.FillRectangle(new SolidBrush(BackColor), 0, start, Width - 1, Height * current_size / (max_height + current_size));
            graf.FillRectangle(new SolidBrush(Color.FromArgb(127, 0, 0, 0)), 0, start, Width - 1, Height * current_size / (max_height + current_size));
            graf.FillRectangle(new SolidBrush(BackColor), border, start + border, Width - 2 * border, Height * current_size / (max_height + current_size) - 2 * border);

            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddRectangle(new Rectangle(0, start, Width - 1, Height * current_size / (max_height + current_size)));
            if (start - Height / 50 > 0)
            {
                path.AddRectangle(new Rectangle(0, 0, Width - 1, Height / 50));
                path.AddRectangle(new Rectangle(2 * Width / 5, Height / 50, Width / 5, start - Height / 50));
            }
            else
            {
                path.AddRectangle(new Rectangle(0, 0, Width - 1, start));
            }
            if (start + Height * current_size / (max_height + current_size) - (Height - Height / 50) < 0)
            {
                path.AddRectangle(new Rectangle(0, Height - Height / 50, Width - 1, Height));
                path.AddRectangle(new Rectangle(2 * Width / 5, start + Height * current_size / (max_height + current_size), Width / 5, Height - Height / 50 - (start + Height * current_size / (max_height + current_size))));
            }
            else
            {
                path.AddRectangle(new Rectangle(0, start + Height * current_size / (max_height + current_size), Width - 1, Height));
            }
            Region = new Region(path);
        }
        public override void replace(int x_l, int y_l, int x_s, int y_s)
        {
            base.replace(x_l, y_l, x_s, y_s);
            previous_height = 0;
            double proportion;
            if (max_height != 0)
                proportion = Convert.ToDouble(current_height) / Convert.ToDouble(max_height);
            else
                proportion = 0;
            current_height = 0;
            find_position();
            Move_elements_to_position(Convert.ToInt32(proportion * Convert.ToDouble(max_height)));
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            Scroll(e);
            Invalidate();
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (pressed)
            {
                Scroll(e);
                Invalidate();
            }
        }
        protected virtual void Scroll(MouseEventArgs e)
        {
            try
            {
                int otstup = Height * current_size / (max_height + current_size) / 2;
                int candidat = max_height * (e.Y - otstup) / (Height - 2 * otstup);
                if (candidat >= 0 && candidat <= max_height)
                {
                    current_height = candidat;
                    if (Math.Abs(previous_height - current_height) >= 5)
                        Move_elements();
                }
                else if (candidat < 0)
                {
                    current_height = 0;
                    if (Math.Abs(previous_height - current_height) > 0)
                        Move_elements();
                }
                else
                {
                    current_height = max_height;
                    if (Math.Abs(previous_height - current_height) > 0)
                        Move_elements();
                }
            }
            catch { }
        }
        protected virtual void Move_elements()
        {
            foreach (My_Control i in elements)
            {
                i.Location = new Point(i.Location.X, i.Location.Y - (current_height - previous_height));
            }
            previous_height = current_height;
        }
        protected virtual void Move_elements_to_position(int c_h)
        {
            current_height = c_h;
            Move_elements();
            Invalidate();
        }
        protected virtual void find_position()
        {
            current_size = Height;
            max_height = highest_my_control.Location.Y + highest_my_control.Height - current_size + lowest_my_control.Location.Y;
            if (max_height < 0)
            {
                max_height = 0;
            }
        }
    }
    //
    public class Picture : My_Control
    {
        public Image image;
        public Picture(Image i) 
        {
            replace(0, 0, 100, 100);
            BackColor = Color.FromArgb(0, 0, 0, 0);
            image = i;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
            Region = new Region(path);
        }
    }
    public class Menu_Backgraund : My_Control
    {
        private List<Image> images = new List<Image>();
        public List<Image> Images { set { if (value == null || value.Contains(null)) throw new Exception("зображення рівне null"); images = value; } get { return images; } }
        private Timer timer = new Timer();
        private byte Index = 0;
        private Menu_Backgraund()
        {
            timer.Interval = 800;
            timer.Enabled = true;
            timer.Tick += new EventHandler(this.timer_Tick);
            replace(0, 0, 100, 100);
        }
        public Menu_Backgraund(List<Image> images) : this()
        {
            Images = images;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics graf = e.Graphics;
            int proportion = Math.Max(105 * Width / Images[Index].Width, 105 * Height / Images[Index].Height);
            graf.DrawImage(Images[Index], Width / 2 - proportion * Images[Index].Width / 200, Height / 2 - proportion * Images[Index].Height / 200, proportion * Images[Index].Width / 100, proportion * Images[Index].Height / 100);
        }
        protected virtual void timer_Tick(object sender, EventArgs e)
        {
            Index++;
            if (Index >= Images.Count)
                Index = 0;
            Invalidate();
        }
    }
    public class Menu_Backgraund_with_pictures : Menu_Backgraund
    {
        private List<Picture> pictures = new List<Picture>();
        public List<Picture> Pictures { set { if (value == null || value.Contains(null)) throw new Exception("зображення рівне null"); pictures = value; } get { return pictures; } }
        public Menu_Backgraund_with_pictures(List<Image> images, List<Picture> pictures) : base(images)
        {
            Pictures = pictures;
        }
        protected override void OnPaint(PaintEventArgs e) 
        {
            base.OnPaint(e);
            Graphics graf = e.Graphics;
            graf.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            foreach (Picture i in Pictures) 
            {
                int proportion = Math.Min(95 * i.Width / i.image.Width, 95 * i.Height / i.image.Height);
                graf.DrawImage(i.image, i.Location.X + i.Width / 2 - proportion * i.image.Width / 200, i.Location.Y + i.Height / 2 - proportion * i.image.Height / 200, proportion * i.image.Width / 100, proportion * i.image.Height / 100);
            }
        }
    }
    //
    public class Border : My_Control
    {
        public Border(int xl, int yl, int xs, int ys, Color color)
        {
            replace(xl, yl, xs, ys);
            BackColor = color;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics graf = e.Graphics;
            graf.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            graf.FillRectangle(new SolidBrush(BackColor), 0, 0, Width, Height);
            graf.FillRectangle(new SolidBrush(Color.FromArgb(125, 0, 0, 0)), 0, 0, Width, Height);
            graf.FillRectangle(new SolidBrush(BackColor), Math.Min(Width, Height) / 5, Math.Min(Width, Height) / 5, Width - 2 * Math.Min(Width, Height) / 5, Height - 2 * Math.Min(Width, Height) / 5);
        }
    }
    //
    public class My_Text_Box : My_Control
    {
        public string Top_Text;
        public override string Text { get { return input.Text; } set { input.Text = value; } }
        public override Font Font { get => base.Font; set { base.Font = new Font("GOST type A", value.Size, FontStyle.Bold); } }
        protected StringFormat s_f = new StringFormat();
        protected TextBox input = new TextBox();
        protected My_Text_Box()
        {
            BackColor = SystemColors.Control;
            s_f.Alignment = StringAlignment.Near;
            s_f.LineAlignment = StringAlignment.Near;
            Font = new Font("GOST type A", 20, FontStyle.Bold);
            input_preparation();
            Controls.Add(input);
        }
        public My_Text_Box(int xl, int yl, int xs, int ys) : this()
        {
            replace(xl, yl, xs, ys);
        }
        public My_Text_Box(int xl, int yl, int xs, int ys, Color b_c) : this(xl, yl, xs, ys)
        {
            BackColor = b_c;
        }
        public My_Text_Box(int xl, int yl, int xs, int ys, string t_t) : this(xl, yl, xs, ys)
        {
            Top_Text = t_t;
        }
        public My_Text_Box(int xl, int yl, int xs, int ys, Color b_c, string t_t) : this(xl, yl, xs, ys, b_c)
        {
            Top_Text = t_t;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            input_preparation();
            Graphics graf = e.Graphics;
            graf.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            graf.Clear(BackColor);
            graf.DrawRectangle(new Pen(Color.FromArgb(125, 0, 0, 0), 1), 0, 0, Width - 1, Height - 1);
            graf.DrawRectangle(new Pen(Color.FromArgb(125, 0, 0, 0), 3), 0, Height - 3, Width - 1, 3);
            if (BackColor.R + BackColor.G + BackColor.B < Math.Sqrt(255) * 9)
                graf.DrawString(Top_Text, Font, new SolidBrush(Color.White), 0, 0, s_f);
            else
                graf.DrawString(Top_Text, Font, new SolidBrush(Color.Black), 0, 0, s_f);
        }
        protected virtual void input_preparation()
        {
            int height = Font.Height;
            input.Location = new Point(4, height + 4);
            input.Size = new Size(Width - 8, Height - input.Location.Y - 8);
            input.Font = binary_font_search(1, 500, 2 * Height, 98 * (Height - input.Location.Y - 4) / 100, Font, "A");
            input.BorderStyle = BorderStyle.None;
            input.BackColor = Color.FromArgb((2 * BackColor.R + 255) / 3, (2 * BackColor.G + 255) / 3, (2 * BackColor.B + 255) / 3);
            if (BackColor.R + BackColor.G + BackColor.B < Math.Sqrt(255) * 9)
                input.ForeColor = Color.White;
            else
                input.ForeColor = Color.Black;
        }
        public override void replace(int x_l, int y_l, int x_s, int y_s)
        {
            int height = Font.Height;
            base.replace(x_l, y_l, x_s, Math.Max(23 * height / 10, y_s));
        }
    }
    public class My_Natural_Number_Text_Box : My_Text_Box
    {
        public My_Natural_Number_Text_Box(int xl, int yl, int xs, int ys) : base(xl, yl, xs, ys)
        {
            Text = "1";
            input.KeyPress += new KeyPressEventHandler(this.is_it_digit);
        }
        public My_Natural_Number_Text_Box(int xl, int yl, int xs, int ys, Color b_c) : base(xl, yl, xs, ys, b_c)
        {
            Text = "1";
            input.KeyPress += new KeyPressEventHandler(this.is_it_digit);
        }
        public My_Natural_Number_Text_Box(int xl, int yl, int xs, int ys, string t_t) : base(xl, yl, xs, ys, t_t)
        {
            Text = "1";
            input.KeyPress += new KeyPressEventHandler(this.is_it_digit);
        }
        public My_Natural_Number_Text_Box(int xl, int yl, int xs, int ys, Color b_c, string t_t) : base(xl, yl, xs, ys, b_c, t_t)
        {
            Text = "1";
            input.KeyPress += new KeyPressEventHandler(this.is_it_digit);
        }
        protected virtual void is_it_digit(object sender, KeyPressEventArgs e)
        {
            try
            {
                Char c = e.KeyChar;
                if (c == 8)
                    return;
                int i = Convert.ToInt32(Text + c);
                if (i == 0)
                    throw new Exception();
            }
            catch
            {
                e.Handled = true;
            }
        }
    }
}
