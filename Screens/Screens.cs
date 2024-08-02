using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;
using Cellular_Automaton_Logic;
using My_Controls;

namespace Screens
{
    public abstract class A_AScreen_compatible_Form : Form
    {
        protected internal AScreen current_screen;
        public A_AScreen_compatible_Form() : base()
        {
            MinimumSize = new Size(400,280);
            try
            {
                Icon = new Icon(Application.StartupPath + @"\Image\Wire World.ico");
            }
            catch { }
            List<Image> images = new List<Image>();
            string[] r = Directory.GetFiles(Application.StartupPath + @"\Menu_Backgraund_Images", "*.bmp");
            foreach (string i in r)
            {
                images.Add(Image.FromFile(i));
            }
            current_screen = new Menu_Screen(this, images);
            SizeChanged += new EventHandler(this.size_change);
        }
        protected virtual void size_change(object sender, EventArgs e)
        {
            if(current_screen!=null)
            current_screen.Sizeble();
        }
    }
    //
    public abstract class AScreen
    {
        internal protected A_AScreen_compatible_Form Current_form;
        internal protected List<My_Control> graf_controls = new List<My_Control>();
        public AScreen(A_AScreen_compatible_Form f)
        {
            Current_form = f;
        }
        public virtual void make_visible()
        {
            if (graf_controls != null)
            {
                foreach (Control i in graf_controls)
                    Current_form.Controls.Add(i);
            }
        }
        public virtual void make_invisible()
        {
            if (graf_controls != null)
            {
                foreach (Control i in graf_controls)
                    Current_form.Controls.Remove(i);
            }
        }
        public virtual void change_forms_screen(AScreen screen)
        {
            make_invisible();
            Current_form.current_screen = screen;
            screen.make_visible();
            screen.Sizeble();
        }
        public abstract void replace(int xl, int yl, int xs, int ys);
        public virtual void Sizeble()
        {
            replace(0, 0, Current_form.Width - 15, Current_form.Height - 39);
        }
    }
    //
    public class Menu_Screen : AScreen
    {
        public Menu_Screen(A_AScreen_compatible_Form f, List<Image> images) : base(f)
        {
            graf_controls.Add(new Menu_Button(0, 0, 100, 40, Color.Gray, "Навчання"));
            graf_controls.Add(new Menu_Button(0, 0, 100, 40, Color.Gray, "Вибiр рiвня"));
            graf_controls.Add(new Menu_Button(0, 0, 100, 40, Color.Gray, "Вільний режим"));
            graf_controls.Add(new Menu_Button(0, 0, 100, 40, Color.Gray, "Редактор рівнів"));
            graf_controls.Add(new Menu_Button(0, 0, 100, 40, Color.Gray, "Вихід"));
            Picture p = new Picture(Image.FromFile(Application.StartupPath + @"\Image\Image.png"));
            graf_controls.Add(p);
            List<Picture> pictures = new List<Picture>();
            pictures.Add(p);
            graf_controls.Add(new Menu_Backgraund_with_pictures(images, pictures));
            graf_controls[0].MouseUp += new MouseEventHandler(this.Tutorial);
            graf_controls[1].MouseUp += new MouseEventHandler(this.Level_Menu);
            graf_controls[2].MouseUp += new MouseEventHandler(this.Sandbox_Map_Menu);
            graf_controls[3].MouseUp += new MouseEventHandler(this.Create_Level);
            graf_controls[4].MouseUp += new MouseEventHandler(this.Exit_MouseUp);
        }
        public override void replace(int xl, int yl, int xs, int ys)
        {
            for (int i = 0; i < graf_controls.Count; i++)
            {
                if (graf_controls[i] is Menu_Button)
                    graf_controls[i].replace(xl + xs / 6, yl + 2 * ys / 5 + i * ys / 10, 2 * xs / 3, 9 * ys / 100);
                if (graf_controls[i] is Menu_Backgraund)
                    graf_controls[i].replace(xl, yl, xs, ys);
                if (graf_controls[i] is Picture)
                    graf_controls[i].replace(xl + xs / 6, yl + ys / 20, 2 * xs / 3, 2 * ys / 5 - ys / 20 - 10);
            }
        }
        protected virtual void Tutorial(object sender, MouseEventArgs e) 
        {
            change_forms_screen(new Tutorial_Levels_Screen(Current_form,this));
        }
        protected virtual void Level_Menu(object sender, MouseEventArgs e)
        {
            Menu_Backgraund m = graf_controls[graf_controls.Count - 1] as Menu_Backgraund;
            change_forms_screen(new Level_Menu_Screen(Current_form, m.Images));
        }
        protected virtual void Sandbox_Map_Menu(object sender, MouseEventArgs e)
        {
            Menu_Backgraund m = graf_controls[graf_controls.Count - 1] as Menu_Backgraund;
            change_forms_screen(new Sandbox_Level_Menu_Screen(Current_form, m.Images));
        }
        protected virtual void Create_Level(object sender, MouseEventArgs e)
        {
            Menu_Backgraund m = graf_controls[graf_controls.Count - 1] as Menu_Backgraund;
            change_forms_screen(new Map_Logic_Menu_Screen(Current_form, m.Images, this));
        }
        protected virtual void Exit_MouseUp(object sender, MouseEventArgs e)
        {
            Current_form.Close();
        }
    }
    public class Level_Menu_Screen : AScreen
    {
        protected My_Controls.Message error_message = new My_Controls.Message(0, 0, 50, 50, Color.Red, "Неможливо відкрити рівень:\n");
        public Level_Menu_Screen(A_AScreen_compatible_Form f, List<Image> images) : base(f)
        {
            List<string>[] levels;
            using (FileStream fs = new FileStream(Application.StartupPath + @"\Levels\Levels.dat", FileMode.Open))
            {
                BinaryFormatter bf = new BinaryFormatter();
                levels = bf.Deserialize(fs) as List<string>[];
            }
            for(int i = 0; i < levels[0].Count; i++) 
            {
                string[] info = levels[0][i].Split(' ');
                string name = "";
                for (int j = 0; j < info.Length - 1; j++)
                {
                    name += info[j];
                }
                switch (info[info.Length - 1])
                {
                    case "1":
                        graf_controls.Add(new Level_Button(f.Location.X + f.Width / 4 + i % 5 * (f.Width / 10), f.Location.Y + f.Height / 4 + i / 5 * (f.Width / 10), f.Width / 10 - 10, f.Width / 10 - 10, Color.FromArgb(0, 200, 0), name));
                        break;
                    default:
                        graf_controls.Add(new Level_Button(f.Location.X + f.Width / 4 + i % 5 * (f.Width / 10), f.Location.Y + f.Height / 4 + i / 5 * (f.Width / 10), f.Width / 10 - 10, f.Width / 10 - 10, Color.Gray, name));
                        break;
                }
                graf_controls[graf_controls.Count - 1].MouseUp += new MouseEventHandler(this.Open_level);
            }
            graf_controls.Add(new Vertical_Scroll_Bar(f.Width - 50, 0, 50, f.Height - 39, graf_controls));
            graf_controls.Add(new Menu_Button(5, 5, 100, 40, Color.Red, "Вихід в меню"));
            graf_controls[graf_controls.Count - 1].MouseUp += new MouseEventHandler(this.Back_to_Menu);
            graf_controls.Add(new Menu_Backgraund(images));
        }
        public override void replace(int xl, int yl, int xs, int ys)
        {
            error_message.replace(xl + (xs- Math.Max(150, xs / 3)) / 2, yl + (ys - Math.Max(50, ys / 3)) / 2, Math.Max(150, xs / 3), Math.Max(50, ys / 3));
            for (int i = 0; i < graf_controls.Count - 3; i++)
            {
                graf_controls[i].replace(xl + xs / 4 + i % 5 * (xs / 10), yl + ys / 4 + i / 5 * (xs / 10), 9 * xs / 100, 9 * xs / 100);
            }
            graf_controls[graf_controls.Count - 3].replace(xl + xs - 20, yl, 20, ys);
            graf_controls[graf_controls.Count - 2].replace(xl + 5, yl + 5, 9 * xs / 40, Math.Max(30, ys / 20));
            graf_controls[graf_controls.Count - 1].replace(xl, yl, xs, ys);
        }
        protected virtual void Back_to_Menu(object sender, MouseEventArgs e)
        {
            Menu_Backgraund m = graf_controls[graf_controls.Count - 1] as Menu_Backgraund;
            change_forms_screen(new Menu_Screen(Current_form, m.Images));
        }
        protected virtual void Open_level(object sender, MouseEventArgs e)
        {
            Level_Button level_button = sender as Level_Button;
            try
            {
                Level_Logic l_l = (Level_Logic)Map_Logic_Creator.Crate_map_logic(Application.StartupPath + @"\Levels\Default_Levels\" + level_button.Text + ".dat");
                change_forms_screen(new Default_Levels_Screen(Current_form, this, l_l, level_button));
            }
            catch 
            {
                error_message.Text = "Неможливо відкрити рівень:\n" + level_button.Text;
                Current_form.Controls.Add(error_message);
                error_message.BringToFront();
                foreach (My_Control i in graf_controls) 
                {
                    i.Enabled = false;
                }
                error_message.MouseUp += new MouseEventHandler(this.Remove_error_message);
            }
        }
        protected virtual void Remove_error_message(object sender, MouseEventArgs e) 
        {
            foreach (My_Control i in graf_controls)
            {
                i.Enabled = true;
            }
            Current_form.Controls.Remove(error_message);
            error_message.MouseUp -= new MouseEventHandler(this.Remove_error_message);
        }
    }
    public class Sandbox_Level_Menu_Screen : AScreen
    {
        protected My_Controls.Message error_message = new My_Controls.Message(0, 0, 50, 50, Color.Red, "Неможливо відкрити рівень:\n");
        public Sandbox_Level_Menu_Screen(A_AScreen_compatible_Form f, List<Image> images) : base(f)
        {
            List<string>[] levels;
            using (FileStream fs = new FileStream(Application.StartupPath + @"\Levels\Levels.dat", FileMode.Open))
            {
                BinaryFormatter bf = new BinaryFormatter();
                levels = bf.Deserialize(fs) as List<string>[];
            }
            for (int i = 0; i < levels[1].Count; i++)
            {
                graf_controls.Add(new Level_Button(f.Location.X + f.Width / 4, f.Location.Y + f.Height / 4 + i * (f.Height / 5), f.Width / 2, f.Width / 5 - 10, Color.Gray, levels[1][i]));
                graf_controls[graf_controls.Count - 1].MouseUp += new MouseEventHandler(this.Open_level);
            }
            graf_controls.Add(new Vertical_Scroll_Bar(f.Width - 50, 0, 50, f.Height - 39, graf_controls));
            graf_controls.Add(new Menu_Button(5, 5, 100, 40, Color.Red, "Вихід в меню"));
            graf_controls[graf_controls.Count - 1].MouseUp += new MouseEventHandler(this.Back_to_Menu);
            graf_controls.Add(new Menu_Backgraund(images));
        }
        public override void replace(int xl, int yl, int xs, int ys)
        {
            Size s = TextRenderer.MeasureText(error_message.Text, new Font("GOST type A", 30, FontStyle.Bold));
            error_message.replace(xl + (xs - s.Width) / 2, yl + (ys - s.Height) / 2, s.Width, s.Height);
            for (int i = 0; i < graf_controls.Count - 3; i++)
            {
                graf_controls[i].replace(xl + xs / 4, yl + ys / 4 + i * (xs / 10), xs / 2, 9 * xs / 100);
            }
            graf_controls[graf_controls.Count - 3].replace(xl + xs - 20, yl, 20, ys);
            graf_controls[graf_controls.Count - 2].replace(xl + 5, yl + 5, 9 * xs / 40, Math.Max(30, ys / 20));
            graf_controls[graf_controls.Count - 1].replace(xl, yl, xs, ys);
        }
        protected virtual void Back_to_Menu(object sender, MouseEventArgs e)
        {
            Menu_Backgraund m = graf_controls[graf_controls.Count - 1] as Menu_Backgraund;
            change_forms_screen(new Menu_Screen(Current_form, m.Images));
        }
        protected virtual void Open_level(object sender, MouseEventArgs e)
        {
            Level_Button level_button = sender as Level_Button;
            try 
            { 
                Map_Logic m_l = Map_Logic_Creator.Crate_map_logic(Application.StartupPath + @"\Levels\Sandbox_Maps\" + level_button.Text + ".dat");
                change_forms_screen(new Sandbox_Maps_Screen(Current_form, this, m_l, level_button.Text));
            }
            catch
            {
                error_message.Text = "Неможливо відкрити рівень:\n" + level_button.Text;
                Current_form.Controls.Add(error_message);
                error_message.BringToFront();
                foreach (My_Control i in graf_controls)
                {
                    i.Enabled = false;
                }
                error_message.MouseUp += new MouseEventHandler(this.Remove_error_message);
            }
        }
        protected virtual void Remove_error_message(object sender, MouseEventArgs e)
        {
            foreach (My_Control i in graf_controls)
            {
                i.Enabled = true;
            }
            Current_form.Controls.Remove(error_message);
            error_message.MouseUp -= new MouseEventHandler(this.Remove_error_message);
        }
    }
    //
    public abstract class Level_Screen : AScreen
    {
        protected Map_Logic Map;
        protected AScreen previous_screen;
        protected List<My_Control> controls_buttons = new List<My_Control>();
        protected List<My_Control> color_buttons = new List<My_Control>();
        protected List<My_Control> info_buttons = new List<My_Control>();
        protected My_Controls.Message map_info;
        public Level_Screen(A_AScreen_compatible_Form f, AScreen s, Map_Logic m_l) : base(f)
        {
            previous_screen = s;
            Map = m_l;
            f.BackColor = Color.Black;
            controls_buttons.Add(new Start_Simulation_Button(f.Location.X + f.Width - 110, f.Location.Y + f.Height + 20 + 0 * 90, 70, 70, Color.DarkGray));
            controls_buttons.Add(new Simulation_Speed_Button(f.Location.X + f.Width - 110, f.Location.Y + f.Height + 20 + 1 * 90, 70, 70, Color.DarkGray));
            controls_buttons.Add(new Color_Button(f.Location.X + f.Width - 110, f.Location.Y + f.Height + 20 + 2 * 90, 70, 70, Color.DarkGray, m_l.game_rules.color_of_cell(m_l.game_rules.default_cell_vaule())));
            controls_buttons.Add(new Text_Color_Button(f.Location.X + f.Width - 110, f.Location.Y + f.Height + 20 + 3 * 90, 70, 70, Color.DarkGray, Color.DarkGoldenrod, "ДО\nПОЧАТКУ"));
            controls_buttons.Add(new Text_Color_Button(f.Location.X + f.Width - 110, f.Location.Y + f.Height + 20 + 4 * 90, 70, 70, Color.DarkGray, Color.DarkBlue, "ІНФО."));
            controls_buttons.Add(new Text_Color_Button(f.Location.X + f.Width - 110, f.Location.Y + f.Height + 20 + 5 * 90, 70, 70, Color.DarkGray, Color.Red, "ВИХІД"));
            controls_buttons[0].MouseUp += new MouseEventHandler(this.Start_or_Stop);
            controls_buttons[1].MouseUp += new MouseEventHandler(this.Set_Speed);
            controls_buttons[2].MouseUp += new MouseEventHandler(this.Color_Сhoice);
            controls_buttons[3].MouseUp += new MouseEventHandler(this.To_Default);
            controls_buttons[4].MouseUp += new MouseEventHandler(this.Info);
            controls_buttons[5].MouseUp += new MouseEventHandler(this.Exit);

            foreach (Color i in m_l.game_rules.all_colors_of_cell())
            {
                Color_Button b = new Color_Button(0, 0, 50, 50, Color.DarkGray, i);
                b.MouseUp += new MouseEventHandler(this.Set_Color);
                color_buttons.Add(b);
            }
            {
                info_buttons.Add(new Text_Color_Button(0, 0, 50, 50, Color.DarkGray, Color.FromArgb(20, 20, 255), "НОМЕР\nХОДУ"));
                info_buttons[info_buttons.Count - 1].MouseUp += new MouseEventHandler(this.Show_hide_move_number);
                info_buttons.Add(new Text_Color_Button(0, 0, 50, 50, Color.DarkGray, Color.FromArgb(20, 20, 255), "ОПИС\nРІВНЯ"));
                info_buttons[info_buttons.Count - 1].MouseUp += new MouseEventHandler(this.Show_info);
                info_buttons.Add(new Text_Color_Button(0, 0, 50, 50, Color.DarkGray, Color.YellowGreen, "НАЗАД"));
                info_buttons[info_buttons.Count - 1].MouseUp += new MouseEventHandler(this.Back);
            }

            graf_controls.Add(new Border(0, 0, 50, 50, SystemColors.Control));
            graf_controls.Add(new Type_Visual_Map(Map));
            graf_controls.Add(new Vertical_Scroll_Bar(Current_form.Width - 10, 0, 10, Current_form.Height - 39, controls_buttons));
            graf_controls.AddRange(controls_buttons);

            map_info = new My_Controls.Message(0,0,0,0,Color.Gray,"ІНФОРМАЦІЯ ПРО РІВЕНЬ:\n" + Map.info()); 
        }
        public override void replace(int xl, int yl, int xs, int ys)
        {
            for (int i = 3; i < graf_controls.Count; i++)
            {
                graf_controls[i].replace(xs - 110, yl + 20 + (i - 3) * 90, 100 - 30, 100 - 30);
            }
            graf_controls[0].replace(xs - 140, 0, 10, ys);
            graf_controls[2].replace(xs - 20, 0, 20, ys);
            graf_controls[1].replace(xl, yl, xs - 140, ys);
            map_info.replace(xl + 10, yl + 10, xs - 160, ys - 20);
        }

        protected virtual void Start_or_Stop(object sender, MouseEventArgs e)
        {
            Start_Simulation_Button B = sender as Start_Simulation_Button;
            Type_Visual_Map visual_map = graf_controls[1] as Type_Visual_Map;
            visual_map.simulation_enabled = B.Start;
        }
        protected virtual void Set_Speed(object sender, MouseEventArgs e)
        {
            Simulation_Speed_Button B = sender as Simulation_Speed_Button;
            Type_Visual_Map visual_map = graf_controls[1] as Type_Visual_Map;
            visual_map.simulation_speed = B.Speed;
        }
        protected virtual void Color_Сhoice(object sender, MouseEventArgs e)
        {
            make_invisible();
            while (graf_controls.Count != 3)
            {
                graf_controls.RemoveAt(graf_controls.Count - 1);
            }
            graf_controls.AddRange(color_buttons);
            Vertical_Scroll_Bar v = graf_controls[2] as Vertical_Scroll_Bar;
            Sizeble();
            v.Elements = color_buttons;
            make_visible();
        }
        protected virtual void To_Default(object sender, MouseEventArgs e)
        {
            Map.to_default();
            graf_controls[1].Invalidate();
        }
        protected virtual void Info(object sender, MouseEventArgs e)
        {
            make_invisible();
            while (graf_controls.Count != 3)
            {
                graf_controls.RemoveAt(graf_controls.Count - 1);
            }
            graf_controls.AddRange(info_buttons);
            Vertical_Scroll_Bar v = graf_controls[2] as Vertical_Scroll_Bar;
            Sizeble();
            v.Elements = info_buttons;
            make_visible();
        }
        protected virtual void Exit(object sender, MouseEventArgs e)
        {
            Current_form.Controls.Remove(map_info);
            change_forms_screen(previous_screen);
        }

        protected virtual void Set_Color(object sender, MouseEventArgs e)
        {
            Color_Button s = sender as Color_Button;
            controls_buttons[2].BackColor = s.BackColor;
            make_invisible();
            while (graf_controls.Count != 3)
            {
                graf_controls.RemoveAt(graf_controls.Count - 1);
            }
            graf_controls.AddRange(controls_buttons);
            Vertical_Scroll_Bar v = graf_controls[2] as Vertical_Scroll_Bar;
            Sizeble();
            v.Elements = controls_buttons;
            make_visible();
            Type_Visual_Map v_m = graf_controls[1] as Type_Visual_Map;
            v_m.Current_type_of_cell = v_m.Map.game_rules.cell_of_color(controls_buttons[2].BackColor);
        }

        protected virtual void Show_hide_move_number(object sender, MouseEventArgs e)
        {
            Type_Visual_Map v_m = graf_controls[1] as Type_Visual_Map;
            v_m.show_move_number = !v_m.show_move_number;
            v_m.Invalidate();
        }
        protected virtual void Show_info(object sender, MouseEventArgs e) 
        {
            if (Current_form.Controls.Contains(map_info))
            {
                Current_form.Controls.Remove(map_info);
            }
            else
            {
                map_info.Enabled = false;
                Current_form.Controls.Add(map_info);
                map_info.BringToFront();
            }
        }
        protected virtual void Back(object sender, MouseEventArgs e)
        {
            make_invisible();
            while (graf_controls.Count != 3)
            {
                graf_controls.RemoveAt(graf_controls.Count - 1);
            }
            graf_controls.AddRange(controls_buttons);
            Vertical_Scroll_Bar v = graf_controls[2] as Vertical_Scroll_Bar;
            v.Elements = controls_buttons;
            Sizeble();
            make_visible();
        }
    }
    public class Tutorial_Levels_Screen : Level_Screen
    {
        protected My_Controls.Message tutorial_message;
        protected byte index = 0;
        protected Map_Logic V_R;
        protected Level_Logic Z_R;
        public Tutorial_Levels_Screen(A_AScreen_compatible_Form f, AScreen s) : base(f, s, Map_Logic_Creator.Crate_map_logic(Application.StartupPath + @"\Levels\Tutorial_Levels\Map_logic.dat"))
        {
            V_R = Map;
            Z_R= (Level_Logic)Map_Logic_Creator.Crate_map_logic(Application.StartupPath + @"\Levels\Tutorial_Levels\Level_logic.dat");

            My_Control end = controls_buttons[controls_buttons.Count - 1];
            controls_buttons.RemoveAt(controls_buttons.Count - 1);
            controls_buttons.Add(new Text_Color_Button(0, 0, 50, 50, Color.DarkGray, Color.Cyan, "ЗБЕ-\nРЕГТИ"));
            controls_buttons.Add(end);
            while (graf_controls.Count != 3)
            {
                graf_controls.RemoveAt(graf_controls.Count - 1);
            }
            graf_controls.AddRange(controls_buttons);
            Sizeble();
            Vertical_Scroll_Bar v = graf_controls[2] as Vertical_Scroll_Bar;
            v.Elements = controls_buttons;

            Preparation_for_tutorial_message_0();
        }
        public override void replace(int xl, int yl, int xs, int ys) 
        {
            base.replace(xl, yl, xs, ys);
            if (tutorial_message != null)
            {
                try
                {
                    Font f = new Font("GOST type A", 30, FontStyle.Bold); int i = f.Height;
                    int x, y;
                    switch (index)
                    {
                        case 0:
                            x = 8; y = 3;
                            goto center;
                        case 1:
                            x = 11; y = 3;
                            goto center;
                        case 4:
                            x = 16; y = 5;
                            goto center;
                        case 13:
                            x = 8; y = 3;
                            goto center;
                        case 14:
                            x = 15; y = 3;
                            goto center;
                        case 16:
                            x = 25; y = 9;
                            goto center;
                        case 17:
                            x = 12; y = 3;
                            goto center;
                        center:
                            {
                                int unit = Math.Min(45, Math.Min((graf_controls[1].Width - 10) / x, (graf_controls[1].Height - 10) / y));
                                tutorial_message.replace(graf_controls[1].Location.X + (graf_controls[1].Width - (x * unit + 10)) / 2, graf_controls[1].Location.Y + (graf_controls[1].Height - (y * unit + 10)) / 2, x * unit + 10, y * unit + 10);
                            }
                            break;
                    }
                    switch (index)
                    {
                        case 2:
                            tutorial_message.replace(graf_controls[1].Location.X + graf_controls[1].Width - Math.Min(graf_controls[1].Width, (350 + 10)), graf_controls[3].Location.Y, Math.Min(graf_controls[1].Width, 350 + 10), 70);
                            break;
                        case 3:
                            tutorial_message.replace(graf_controls[1].Location.X + graf_controls[1].Width - Math.Min(graf_controls[1].Width, (350 + 10)), graf_controls[7].Location.Y, Math.Min(graf_controls[1].Width, 350 + 10), 70);
                            break;
                        case 5:
                            tutorial_message.replace(graf_controls[1].Location.X + graf_controls[1].Width - Math.Min(graf_controls[1].Width, (420 + 10)), graf_controls[5].Location.Y, Math.Min(graf_controls[1].Width, 420 + 10), 70);
                            break;
                        case 6:
                            tutorial_message.replace(graf_controls[1].Location.X + graf_controls[1].Width - Math.Min(graf_controls[1].Width, (270 + 10)), 90, Math.Min(graf_controls[1].Width, 270 + 10), 105);
                            break;
                        case 7:
                            tutorial_message.replace(graf_controls[1].Location.X + graf_controls[1].Width - Math.Min(graf_controls[1].Width, (350 + 10)), graf_controls[4].Location.Y, Math.Min(graf_controls[1].Width, 350 + 10), 70);
                            break;
                        case 8:
                            tutorial_message.replace(graf_controls[1].Location.X + graf_controls[1].Width - Math.Min(graf_controls[1].Width, (350 + 10)), graf_controls[3].Location.Y, Math.Min(graf_controls[1].Width, 350 + 10), 70);
                            break;
                        case 9:
                            tutorial_message.replace(graf_controls[1].Location.X + graf_controls[1].Width - Math.Min(graf_controls[1].Width, (385 + 10)), graf_controls[5].Location.Y, Math.Min(graf_controls[1].Width, 385 + 10), 105);
                            break;
                        case 11:
                            tutorial_message.replace(graf_controls[1].Location.X + graf_controls[1].Width - Math.Min(graf_controls[1].Width, (400 + 10)), graf_controls[6].Location.Y, Math.Min(graf_controls[1].Width, 400 + 10), 70);
                            break;
                        case 12:
                            tutorial_message.replace(graf_controls[1].Location.X + graf_controls[1].Width - Math.Min(graf_controls[1].Width, (430 + 10)), graf_controls[8].Location.Y, Math.Min(graf_controls[1].Width, 430 + 10), 70);
                            break;
                        case 15:
                            tutorial_message.replace(graf_controls[1].Location.X + graf_controls[1].Width - Math.Min(graf_controls[1].Width, (350 + 10)), graf_controls[7].Location.Y, Math.Min(graf_controls[1].Width, 350 + 10), 70);
                            break;
                    }
                    switch (index)
                    {
                        case 10:
                            int unit = Math.Min(graf_controls[1].Width / 24, i);
                            tutorial_message.replace(graf_controls[1].Location.X + (graf_controls[1].Width - 24 * unit) / 2, graf_controls[1].Location.Y + graf_controls[1].Height - (3 * unit + 10), 24 * unit, 3 * unit + 10);
                            break;
                    }
                    switch (index)
                    {
                        case 18:
                            tutorial_message.replace(graf_controls[1].Location.X, graf_controls[1].Location.Y + (graf_controls[1].Height - 2 * i + 20) / 2, i + 10, 2 * i + 20);
                            break;
                    }
                }
                catch { }
            }
        }
        protected override void Exit(object sender, MouseEventArgs e)
        {
            Current_form.Controls.Remove(tutorial_message);
            base.Exit(sender, e);
        }
        protected virtual void Change_level_type() 
        {
            Type_Visual_Map v = graf_controls[1] as Type_Visual_Map;
            Start_Simulation_Button t = controls_buttons[0] as Start_Simulation_Button;
            t.to_default();
            v.simulation_enabled = t.Start;
            Simulation_Speed_Button s = controls_buttons[1] as Simulation_Speed_Button;
            s.to_default();
            v.simulation_speed = s.Speed;
            v.Current_type_of_cell = Map.game_rules.default_cell_vaule();
            controls_buttons[2].BackColor = Map.game_rules.color_of_cell(Map.game_rules.default_cell_vaule());
            make_invisible();
            if (Map==V_R)
            {
                Map = Z_R;
                graf_controls[1] = new Type_Visual_Map(Map);

                graf_controls.Remove(controls_buttons[controls_buttons.Count - 2]); controls_buttons.RemoveAt(controls_buttons.Count - 2);
                if (graf_controls.Contains(controls_buttons[0]))
                {
                    while (graf_controls.Count != 3)
                    {
                        graf_controls.RemoveAt(graf_controls.Count - 1);
                    }
                    graf_controls.AddRange(controls_buttons);
                    Sizeble();
                    Vertical_Scroll_Bar v_s = graf_controls[2] as Vertical_Scroll_Bar;
                    v_s.Elements = controls_buttons;
                }

                My_Control end = info_buttons[info_buttons.Count - 1];
                info_buttons.RemoveAt(info_buttons.Count - 1);
                Level_Logic l_l = Map as Level_Logic;
                if (l_l.win_rules is IPositional_Win_Rules)
                {
                    IPositional_Win_Rules w_r = l_l.win_rules as IPositional_Win_Rules;
                    for (int i = 0; i < w_r.how_many_position(); i++)
                    {
                        info_buttons.Add(new Text_Color_Button(0, 0, 50, 50, Color.DarkGray, Color.FromArgb(50, 50, 255), "ПЕРЕЙТИ\nДО " + i));
                        info_buttons[info_buttons.Count - 1].MouseUp += new MouseEventHandler(this.Go_to_position);
                    }
                }
                info_buttons.Add(new Text_Color_Button(0, 0, 50, 50, Color.DarkGray, Color.Gold, "ПЕРЕ-\nВІРКА"));
                info_buttons[info_buttons.Count - 1].MouseUp += new MouseEventHandler(this.Check);
                info_buttons.Add(end);
                if (graf_controls.Contains(info_buttons[0]))
                {
                    while (graf_controls.Count != 3)
                    {
                        graf_controls.RemoveAt(graf_controls.Count - 1);
                    }
                    graf_controls.AddRange(info_buttons);
                    Sizeble();
                    Vertical_Scroll_Bar v_s = graf_controls[2] as Vertical_Scroll_Bar;
                    v_s.Elements = info_buttons;
                }
            }
            else 
            {
                Map = V_R;
                graf_controls[1] = new Type_Visual_Map(Map);

                My_Control end = info_buttons[info_buttons.Count - 1];
                while (info_buttons.Count != 2)
                {
                    graf_controls.Remove(info_buttons[info_buttons.Count - 1]);
                    info_buttons.RemoveAt(info_buttons.Count - 1);
                }  
                info_buttons.Add(end);
                if (graf_controls.Contains(info_buttons[0]))
                {
                    while (graf_controls.Count != 3)
                    {
                        graf_controls.RemoveAt(graf_controls.Count - 1);
                    }
                    graf_controls.AddRange(info_buttons);
                    Sizeble();
                    Vertical_Scroll_Bar v_s = graf_controls[2] as Vertical_Scroll_Bar;
                    v_s.Elements = info_buttons;
                }

                end = controls_buttons[controls_buttons.Count - 1];
                controls_buttons.RemoveAt(controls_buttons.Count - 1);
                controls_buttons.Add(new Text_Color_Button(0, 0, 50, 50, Color.DarkGray, Color.Cyan, "ЗБЕ-\nРЕГТИ"));
                controls_buttons.Add(end);
                if (graf_controls.Contains(controls_buttons[0])) 
                {
                    while (graf_controls.Count != 3)
                    {
                        graf_controls.RemoveAt(graf_controls.Count - 1);
                    }
                    graf_controls.AddRange(controls_buttons);
                    Sizeble();
                    Vertical_Scroll_Bar v_s = graf_controls[2] as Vertical_Scroll_Bar;
                    v_s.Elements = controls_buttons;
                }
            }
            map_info.Text = Map.info();
            make_visible();
        }
        protected virtual void Go_to_position(object sender, MouseEventArgs e)
        {
            Text_Color_Button t = sender as Text_Color_Button;
            Type_Visual_Map visual_map = graf_controls[1] as Type_Visual_Map;
            Level_Logic l_l = visual_map.Map as Level_Logic;
            IPositional_Win_Rules w_r = l_l.win_rules as IPositional_Win_Rules;
            w_r.go_to_position(l_l, Convert.ToInt32(t.Text.Split(' ')[1]));
            visual_map.Invalidate();
        }
        protected virtual void Check(object sender, MouseEventArgs e)
        {
            Type_Visual_Map v_m = graf_controls[1] as Type_Visual_Map;
            v_m.check();
            Level_Logic l = Map as Level_Logic;
            if (l.is_it_solved)
            {
                {
                    make_invisible();
                    while (graf_controls.Count != 3)
                    {
                        graf_controls.RemoveAt(graf_controls.Count - 1);
                    }
                    graf_controls.AddRange(controls_buttons);
                    Vertical_Scroll_Bar v = graf_controls[2] as Vertical_Scroll_Bar;
                    v.Elements = controls_buttons;
                    Sizeble();
                    make_visible();
                }
                foreach (My_Control i in controls_buttons)
                {
                    if (i.Text != "ВИХІД")
                        i.Enabled = false;
                }
            }
            else
            {
                Map.to_default();
            }
        }

        protected virtual void Preparation_for_tutorial_message_0() 
        {
            tutorial_message = new My_Controls.Message(0, 0, 50, 50, Color.Gray, "Розглянемо структуру\nвільного режиму.\nПРОДОВЖИТИ");
            Current_form.Controls.Add(tutorial_message);
            tutorial_message.BringToFront();
            foreach (My_Control i in graf_controls) 
            {
                if (i.Text== "ВИХІД")
                    i.Enabled = true;
                else
                    i.Enabled = false;
                if (i is Vertical_Scroll_Bar)
                {
                    i.Enabled = true;
                }
            }
            tutorial_message.Enabled = true;
            tutorial_message.MouseUp += new MouseEventHandler(this.Preparation_for_tutorial_message_1);
            Sizeble();
        }
        protected virtual void Preparation_for_tutorial_message_1(object sender, MouseEventArgs e)
        {
            index++;
            foreach (My_Control i in graf_controls)
            {
                if (i.Text == "ВИХІД")
                    i.Enabled = true;
                else
                    i.Enabled = false;
                if (i is Vertical_Scroll_Bar)
                {
                    i.Enabled = true;
                }
            }
            tutorial_message.Text = "В ньому ввідсутня можливість перемоги\nта можна вносити будь-які зміни.\nПРОДОВЖИТИ";
            tutorial_message.Enabled = true;
            tutorial_message.MouseUp -= new MouseEventHandler(this.Preparation_for_tutorial_message_1);
            tutorial_message.MouseUp += new MouseEventHandler(this.Preparation_for_tutorial_message_2);
            Sizeble();
        }
        protected virtual void Preparation_for_tutorial_message_2(object sender, MouseEventArgs e)
        {
            index++;
            foreach (My_Control i in graf_controls)
            {
                if ((i.Text == "ВИХІД"))
                    i.Enabled = true;
                else
                    i.Enabled = false;
                if (i is Start_Simulation_Button)
                {
                    i.Enabled = true;
                    i.MouseUp += new MouseEventHandler(this.Preparation_for_tutorial_message_3);
                }
                if (i is Vertical_Scroll_Bar)
                {
                    i.Enabled = true;
                    Vertical_Scroll_Bar i_ = i as Vertical_Scroll_Bar;
                    i_.Elements.Add(tutorial_message);
                }
            }
            tutorial_message.Text = "Для початку симуляції\nнатисніть на кнопку СТАРТ";
            tutorial_message.Enabled = false;
            tutorial_message.MouseUp -= new MouseEventHandler(this.Preparation_for_tutorial_message_2);
            Sizeble();
        }
        protected virtual void Preparation_for_tutorial_message_3(object sender, MouseEventArgs e) 
        {
            index++;
            foreach (My_Control i in graf_controls)
            {
                if ((i.Text == "ВИХІД"))
                    i.Enabled = true;
                else
                    i.Enabled = false;
                if (i is Start_Simulation_Button)
                {
                    i.MouseUp -= new MouseEventHandler(this.Preparation_for_tutorial_message_3);
                }
                if (i.Text== "ІНФО.")
                {
                    i.Enabled = true;
                    i.MouseUp += new MouseEventHandler(this.Preparation_for_tutorial_message_4);
                }
                if (i is Vertical_Scroll_Bar)
                {
                    i.Enabled = true;
                }
            }
            tutorial_message.Enabled = false;
            tutorial_message.Text = "Інформацію про рівень можна\nотримати в вкладці ІНФО";
            Sizeble();
        }
        protected virtual void Preparation_for_tutorial_message_4(object sender, MouseEventArgs e)
        {
            index++;
            foreach(My_Button i in controls_buttons) 
            {
                if (i.Text == "ІНФО.")
                {
                    i.MouseUp -= new MouseEventHandler(this.Preparation_for_tutorial_message_4);
                }
            }
            foreach (My_Control i in graf_controls)
            {
                if (i.Text == "НАЗАД")
                {
                    i.Enabled = false;
                }
                if (i is Vertical_Scroll_Bar)
                {
                    i.Enabled = true;
                    Vertical_Scroll_Bar i_ = i as Vertical_Scroll_Bar;
                    i_.Elements.Remove(tutorial_message);
                }
            }
            tutorial_message.Enabled = true;
            tutorial_message.Text = "Натиснувши на кнопку-НОМЕР ХОДУ\nможна отримати/прибрати кількість ходів.\nНатиснувши на кнопку-ОПИС РІВНЯ\nможна отримати/прибрати інформацію про рівень.\nПРОДОВЖИТИ";
            tutorial_message.MouseUp += new MouseEventHandler(this.Preparation_for_tutorial_message_5);
            Sizeble();
        }
        protected virtual void Preparation_for_tutorial_message_5(object sender, MouseEventArgs e)
        {
            index++;
            foreach (My_Control i in graf_controls)
            {
                if ((i.Text == "НАЗАД"))
                {
                    i.Enabled = true;
                    i.MouseUp += new MouseEventHandler(this.Preparation_for_tutorial_message_6);
                }
                else
                    i.Enabled = false;
                if (i is Vertical_Scroll_Bar)
                {
                    i.Enabled = true;
                }
            }
            Current_form.Controls.Remove(map_info);
            tutorial_message.Enabled = false;
            tutorial_message.Text = "Пара з синьої і червоної клітини\nзазвичай називається сигналом";
            tutorial_message.MouseUp -= new MouseEventHandler(this.Preparation_for_tutorial_message_5);
            Sizeble();
        }
        protected virtual void Preparation_for_tutorial_message_6(object sender, MouseEventArgs e)
        {
            index++;
            foreach (My_Control i in graf_controls)
            {
                if ((i.Text == "ВИХІД"))
                    i.Enabled = true;
                else
                    i.Enabled = false;
                if (i is Vertical_Scroll_Bar)
                {
                    i.Enabled = true;
                }
            }
            foreach (My_Control i in info_buttons) 
            {
                if ((i.Text == "НАЗАД"))
                {
                    i.MouseUp -= new MouseEventHandler(this.Preparation_for_tutorial_message_6);
                }
            }
            tutorial_message.Enabled = true;
            tutorial_message.Text = "Зліва вгорі показаний\nномер поточног ходу.\nПРОДОВЖИТИ";
            tutorial_message.MouseUp += new MouseEventHandler(this.Preparation_for_tutorial_message_7);
            Sizeble();
        }
        protected virtual void Preparation_for_tutorial_message_7(object sender, MouseEventArgs e)
        {
            index++;
            foreach (My_Control i in graf_controls)
            {
                if ((i.Text == "ВИХІД"))
                    i.Enabled = true;
                else
                    i.Enabled = false;
                if (i is Simulation_Speed_Button)
                {
                    i.Enabled = true;
                    i.MouseUp += new MouseEventHandler(this.Preparation_for_tutorial_message_8);
                }
                if (i is Vertical_Scroll_Bar)
                {
                    i.Enabled = true;
                    Vertical_Scroll_Bar i_ = i as Vertical_Scroll_Bar;
                    i_.Elements.Add(tutorial_message);
                }
            }
            tutorial_message.Enabled = false;
            tutorial_message.Text = "Швидкість симуляції можна\nзмінити наступною кнопкою";
            tutorial_message.MouseUp -= new MouseEventHandler(this.Preparation_for_tutorial_message_7);
            Sizeble();
        }
        protected virtual void Preparation_for_tutorial_message_8(object sender, MouseEventArgs e)
        {
            index++;
            foreach (My_Control i in graf_controls)
            {
                if ((i.Text == "ВИХІД"))
                    i.Enabled = true;
                else
                    i.Enabled = false;
                if (i is Simulation_Speed_Button)
                {
                    i.MouseUp -= new MouseEventHandler(this.Preparation_for_tutorial_message_8);
                }
                if (i is Start_Simulation_Button)
                {
                    i.Enabled = true;
                    i.MouseUp += new MouseEventHandler(this.Preparation_for_tutorial_message_9);
                }
                if (i is Vertical_Scroll_Bar)
                {
                    i.Enabled = true;
                }
            }
            tutorial_message.Enabled = false;
            tutorial_message.Text = "Для внесення змін потрібно\nзупинити симуляцію";
            Sizeble();
        }
        protected virtual void Preparation_for_tutorial_message_9(object sender, MouseEventArgs e)
        {
            index++;
            foreach (My_Control i in graf_controls)
            {
                if ((i.Text == "ВИХІД"))
                    i.Enabled = true;
                else
                    i.Enabled = false;
                if (i is Start_Simulation_Button)
                {
                    i.MouseUp -= new MouseEventHandler(this.Preparation_for_tutorial_message_9);
                }
                if((i is Color_Button) && !(i is Text_Color_Button)) 
                {
                    i.Enabled = true;
                    i.MouseUp += new MouseEventHandler(this.Preparation_for_tutorial_message_10);
                }
                if (i is Vertical_Scroll_Bar)
                {
                    i.Enabled = true;
                }
            }
            tutorial_message.Text = "Кнопка сірого кольору\nвідповідає за вибір кольру\nяким ви вноситиме зміни";
            tutorial_message.Enabled = false;
            Sizeble();
        }
        protected virtual void Preparation_for_tutorial_message_10(object sender, MouseEventArgs e)
        {
            index++;
            foreach (My_Control i in graf_controls)
            {
               i.Enabled = true;
            }
            foreach (My_Control i in controls_buttons)
            {
                i.Enabled = true;
                if ((i is Color_Button) && !(i is Text_Color_Button))
                {
                    i.MouseUp -= new MouseEventHandler(this.Preparation_for_tutorial_message_10);
                }
                if ((i.Text == "ЗБЕ-\nРЕГТИ") || (i.Text == "ДО\nПОЧАТКУ") || (i.Text == "ІНФО."))
                    i.Enabled = false;
            }
            tutorial_message.Enabled = true;
            tutorial_message.Text = "Можете внести зміни обравши колір і натиснувши на бажану клітину\n(якщо потрібно можна приблизити колесиком миші).\nПРОДОВЖИТИ";
            tutorial_message.MouseUp += new MouseEventHandler(this.Preparation_for_tutorial_message_11);
            Sizeble();
        }
        protected virtual void Preparation_for_tutorial_message_11(object sender, MouseEventArgs e)
        {
            index++;
            if (!graf_controls.Contains(controls_buttons[0])) 
            {
                make_invisible();
                while (graf_controls.Count != 3)
                {
                    graf_controls.RemoveAt(graf_controls.Count - 1);
                }
                graf_controls.AddRange(controls_buttons);
                Vertical_Scroll_Bar v = graf_controls[2] as Vertical_Scroll_Bar;
                Sizeble();
                v.Elements = controls_buttons;
                make_visible();
            }
            foreach (My_Control i in graf_controls)
            {
                if ((i.Text == "ВИХІД"))
                    i.Enabled = true;
                else
                    i.Enabled = false;
                if (i.Text == "ДО\nПОЧАТКУ")
                {
                    i.Enabled = true;
                    i.MouseUp += new MouseEventHandler(this.Preparation_for_tutorial_message_12);
                }
                if (i is Vertical_Scroll_Bar)
                {
                    i.Enabled = true;
                    Vertical_Scroll_Bar i_ = i as Vertical_Scroll_Bar;
                    i_.Elements.Add(tutorial_message);
                }
            }
            tutorial_message.Enabled = false;
            tutorial_message.Text = "Для повернення до початкової ситуації\nнатисніть на кнопку ДО ПОЧАТКУ";
            tutorial_message.MouseUp -= new MouseEventHandler(this.Preparation_for_tutorial_message_11);
            Sizeble();
        }
        protected virtual void Preparation_for_tutorial_message_12(object sender, MouseEventArgs e)
        {
            index++;
            foreach (My_Control i in graf_controls)
            {
                if ((i.Text == "ВИХІД"))
                    i.Enabled = true;
                else
                    i.Enabled = false;
                if (i.Text == "ДО\nПОЧАТКУ")
                {
                    i.MouseUp -= new MouseEventHandler(this.Preparation_for_tutorial_message_12);
                }
                if (i.Text == "ЗБЕ-\nРЕГТИ")
                {
                    i.Enabled = true;
                    i.MouseUp += new MouseEventHandler(this.Preparation_for_tutorial_message_13);
                }
                if (i is Vertical_Scroll_Bar)
                {
                    i.Enabled = true;
                }
            }
            tutorial_message.Enabled = false;
            tutorial_message.Text = "Для збереження поточної ситуації\nнатисніть на кнопку ЗБЕ-\nРЕГТИ";
            Sizeble();
        }
        protected virtual void Preparation_for_tutorial_message_13(object sender, MouseEventArgs e)
        {
            index++;
            Change_level_type();
            foreach (My_Control i in graf_controls)
            {
                if ((i.Text == "ВИХІД"))
                    i.Enabled = true;
                else
                    i.Enabled = false;
                if (i.Text == "ЗБЕ-\nРЕГТИ")
                {
                    i.MouseUp -= new MouseEventHandler(this.Preparation_for_tutorial_message_13);
                }
                if (i is Vertical_Scroll_Bar)
                {
                    i.Enabled = true;
                    Vertical_Scroll_Bar i_ = i as Vertical_Scroll_Bar;
                    i_.Elements.Remove(tutorial_message);
                }
            }
            tutorial_message.Enabled = true;
            tutorial_message.Text = "Розглянемо структуру\nзвичайного рівня.\nПРОДОВЖИТИ";
            tutorial_message.MouseUp += new MouseEventHandler(this.Preparation_for_tutorial_message_14);
            Sizeble();
        }
        protected virtual void Preparation_for_tutorial_message_14(object sender, MouseEventArgs e)
        {
            index++;
            foreach (My_Control i in graf_controls)
            {
                if ((i.Text == "ВИХІД") || (i is Type_Visual_Map))
                    i.Enabled = true;
                else
                    i.Enabled = false;
                if (i is Vertical_Scroll_Bar)
                {
                    i.Enabled = true;
                }
            }
            tutorial_message.Enabled = true;
            tutorial_message.Text = "В ньому є можливість перемоги та деякі\nклітини(темніші) заборонено змінювати.\nПРОДОВЖИТИ";
            tutorial_message.MouseUp -= new MouseEventHandler(this.Preparation_for_tutorial_message_14);
            tutorial_message.MouseUp += new MouseEventHandler(this.Preparation_for_tutorial_message_15);
            Sizeble();
        }
        protected virtual void Preparation_for_tutorial_message_15(object sender, MouseEventArgs e)
        {
            index++;
            foreach (My_Control i in graf_controls)
            {
                if ((i.Text == "ВИХІД"))
                    i.Enabled = true;
                else
                    i.Enabled = false;
                if (i.Text == "ІНФО.")
                {
                    i.Enabled = true;
                    i.MouseUp += new MouseEventHandler(this.Preparation_for_tutorial_message_16);
                }
                if (i is Vertical_Scroll_Bar)
                {
                    i.Enabled = true;
                    Vertical_Scroll_Bar i_ = i as Vertical_Scroll_Bar;
                    i_.Elements.Add(tutorial_message);
                }
            }
            tutorial_message.Enabled = false;
            tutorial_message.Text = "В вкладці ІНФО зявляється\nнова інформація";
            tutorial_message.MouseUp -= new MouseEventHandler(this.Preparation_for_tutorial_message_15);
            Sizeble();
        }
        protected virtual void Preparation_for_tutorial_message_16(object sender, MouseEventArgs e)
        {
            index++;
            foreach (My_Control i in graf_controls)
            {
                i.Enabled = true;
            }
            foreach (My_Control i in controls_buttons)
            {
                i.Enabled = true;
                if (i.Text == "ІНФО.")
                {
                    i.MouseUp -= new MouseEventHandler(this.Preparation_for_tutorial_message_16);
                }
            }
            foreach (My_Control i in color_buttons)
            {
                i.Enabled = true;
            }
            foreach (My_Control i in graf_controls)
            {
                i.Enabled = true;
                if (i is Vertical_Scroll_Bar)
                {
                    i.Enabled = true;
                    Vertical_Scroll_Bar i_ = i as Vertical_Scroll_Bar;
                    i_.Elements.Remove(tutorial_message);
                }
            }
            tutorial_message.Enabled = true;
            tutorial_message.Text = "Для звичайного рівня тут додається можливість\nперевірки рівня(якщо рівень пройдено неправильно ,\nвідбудеться повернення до початкової ситуації)\nОПИС РІВНЯ також показує як перемогти\nі (якщо умова перемоги передбачає зміну початкового\nположення)всі початкові положення.\nПРОДОВЖИТИ";
            tutorial_message.MouseUp += new MouseEventHandler(this.Preparation_for_tutorial_message_17);
            Sizeble();
        }
        protected virtual void Preparation_for_tutorial_message_17(object sender, MouseEventArgs e)
        {
            index++;
            tutorial_message.Text = "Тепер ви можете перевірити\nвсі функції самостійно.\nПРОДОВЖИТИ";
            tutorial_message.MouseUp -= new MouseEventHandler(this.Preparation_for_tutorial_message_17);
            tutorial_message.MouseUp += new MouseEventHandler(this.Preparation_for_tutorial_message_18);
            Sizeble();
        }
        protected virtual void Preparation_for_tutorial_message_18(object sender, MouseEventArgs e)
        {
            index=18;
            tutorial_message.Text = "З.\nР.";
            tutorial_message.MouseUp -= new MouseEventHandler(this.Preparation_for_tutorial_message_18);
            tutorial_message.MouseUp += new MouseEventHandler(this.Preparation_for_tutorial_message_change);
            Sizeble();
        }
        protected virtual void Preparation_for_tutorial_message_change(object sender, MouseEventArgs e)
        {
            if (tutorial_message.Text == "З.\nР.")
                tutorial_message.Text = "B.\nР.";
            else
                tutorial_message.Text = "З.\nР.";
            Change_level_type();
        }
    }
    public class Default_Levels_Screen : Level_Screen
    {
        protected My_Button source_button;
        protected string level_name;
        public Default_Levels_Screen(A_AScreen_compatible_Form f, AScreen s, Level_Logic m_l, My_Button l) : base(f, s, m_l)
        {
            level_name = l.Text;
            source_button = l;
            My_Control end = info_buttons[info_buttons.Count - 1];
            info_buttons.RemoveAt(info_buttons.Count - 1);
            if (m_l.win_rules is IPositional_Win_Rules)
            {
                IPositional_Win_Rules w_r = m_l.win_rules as IPositional_Win_Rules;
                for (int i = 0; i < w_r.how_many_position(); i++) 
                {
                    info_buttons.Add(new Text_Color_Button(0, 0, 50, 50, Color.DarkGray, Color.FromArgb(50, 50, 255), "ПЕРЕЙТИ\nДО " + i));
                    info_buttons[info_buttons.Count - 1].MouseUp += new MouseEventHandler(this.Go_to_position);
                }
            }
            info_buttons.Add(new Text_Color_Button(0, 0, 50, 50, Color.DarkGray, Color.Gold, "ПЕРЕ-\nВІРКА"));
            info_buttons[info_buttons.Count - 1].MouseUp += new MouseEventHandler(this.Check);
            info_buttons.Add(end);
        }
        protected virtual void Go_to_position(object sender, MouseEventArgs e) 
        {
            Text_Color_Button t = sender as Text_Color_Button;
            Type_Visual_Map visual_map = graf_controls[1] as Type_Visual_Map;
            Level_Logic l_l = visual_map.Map as Level_Logic;
            IPositional_Win_Rules w_r = l_l.win_rules as IPositional_Win_Rules;
            w_r.go_to_position(l_l,Convert.ToInt32(t.Text.Split(' ')[1]));
            visual_map.Invalidate();
        }
        protected virtual void Check(object sender, MouseEventArgs e) 
        {
            Type_Visual_Map v_m = graf_controls[1] as Type_Visual_Map;
            v_m.check();
            Level_Logic l = Map as Level_Logic;
            if (l.is_it_solved) 
            {
                {
                    source_button.BackColor = Color.FromArgb(0, 200, 0);
                }
                {
                    List<string>[] levels;
                    using (FileStream fs = new FileStream(Application.StartupPath + @"\Levels\Levels.dat", FileMode.Open))
                    {
                        BinaryFormatter bf = new BinaryFormatter();
                        levels = bf.Deserialize(fs) as List<string>[];
                    }
                    if(levels[0].IndexOf(level_name+" 0") != -1) 
                    {
                        levels[0][levels[0].IndexOf(level_name + " 0")] = level_name + " 1";
                    }
                    using (FileStream fs = new FileStream(Application.StartupPath + "Levels.dat", FileMode.OpenOrCreate))
                    {
                        BinaryFormatter f = new BinaryFormatter();
                        f.Serialize(fs, levels);
                    }
                }
                {
                    make_invisible();
                    while (graf_controls.Count != 3)
                    {
                        graf_controls.RemoveAt(graf_controls.Count - 1);
                    }
                    graf_controls.AddRange(controls_buttons);
                    Vertical_Scroll_Bar v = graf_controls[2] as Vertical_Scroll_Bar;
                    v.Elements = controls_buttons;
                    Sizeble();
                    make_visible();
                }
                foreach (My_Control i in controls_buttons)
                {
                    if (i.Text != "ВИХІД")
                        i.Enabled = false;
                }
            }
            else
            {
                Map.to_default();
            }
        }
    }
    public class Sandbox_Maps_Screen : Level_Screen
    {
        protected string level_name;
        public Sandbox_Maps_Screen(A_AScreen_compatible_Form f, AScreen s, Map_Logic m_l, string l_n) : base(f, s, m_l)
        {
            level_name = l_n;
            My_Control end = controls_buttons[controls_buttons.Count - 1];
            controls_buttons.RemoveAt(controls_buttons.Count - 1);
            controls_buttons.Add(new Text_Color_Button(0, 0, 50, 50, Color.DarkGray, Color.Cyan, "ЗБЕ-\nРЕГТИ"));
            controls_buttons[controls_buttons.Count - 1].MouseUp += new MouseEventHandler(this.Save);
            controls_buttons.Add(end);
            while (graf_controls.Count != 3)
            {
                graf_controls.RemoveAt(graf_controls.Count - 1);
            }
            graf_controls.AddRange(controls_buttons);
            Sizeble();
            Vertical_Scroll_Bar v = graf_controls[2] as Vertical_Scroll_Bar;
            v.Elements = controls_buttons;
        }
        protected virtual void Save(object sender, MouseEventArgs e) 
        {
            Map_Logic_Creator.Save_map_logic(Application.StartupPath + @"\Levels\Sandbox_Maps\" + level_name + ".dat", Map);
        }
    }
    //
    public abstract class List_Screen : AScreen
    {
        private AScreen previous_screen;
        public List_Screen(A_AScreen_compatible_Form f, List<Image> images, AScreen p_s) : base(f)
        {
            previous_screen = p_s;
            graf_controls.Add(new Vertical_Scroll_Bar(f.Width - 50, 0, 50, f.Height - 39, new List<My_Control>() { new Menu_Button(0, 0, 50, 50, Color.Gray, "Несправжня кнопка") }));
            graf_controls.Add(new Menu_Button(5, 5, 100, 40, Color.Red, "Назад"));
            graf_controls[1].MouseUp += new MouseEventHandler(this.Back_to_Menu);
            graf_controls.Add(new Menu_Backgraund(images));
        }
        public override void replace(int xl, int yl, int xs, int ys)
        {
            for (int i = 0; i < graf_controls.Count; i++)
            {
                if(graf_controls[i] is Menu_Button)
                    graf_controls[i].replace(xl + xs / 4, yl + ys / 4 + (i - 2) * (xs / 10), xs / 2, 9 * xs / 100);
                if (graf_controls[i].Text == "Назад")
                    graf_controls[i].replace(xl + 5, yl + 5, 9 * xs / 40, Math.Max(30, ys / 20));
                if (graf_controls[i] is Vertical_Scroll_Bar)
                    graf_controls[i].replace(xl + xs - 20, yl, 20, ys);
                if (graf_controls[i] is Menu_Backgraund)
                    graf_controls[i].replace(xl, yl, xs, ys);
            }
        }
        protected virtual void Back_to_Menu(object sender, MouseEventArgs e)
        {
            change_forms_screen(previous_screen);
        }
    }
    public enum E_Map_Logic {Map_Logic,Level_Logic }
    public class Map_Logic_Menu_Screen : List_Screen
    {
        public Map_Logic_Menu_Screen(A_AScreen_compatible_Form f, List<Image> images, AScreen p_s) : base(f, images, p_s)
        {
            My_Control end = graf_controls[graf_controls.Count - 1];
            graf_controls.Remove(end);
            graf_controls.Add(new Menu_Button(0, 0, 50, 50, Color.Gray, "Звичайний рівень"));
            graf_controls[graf_controls.Count - 1].MouseUp += new MouseEventHandler(this.Create_Level_Logic);
            graf_controls.Add(new Menu_Button(0, 0, 50, 50, Color.Gray, "Вільний режим"));
            graf_controls[graf_controls.Count - 1].MouseUp += new MouseEventHandler(this.Create_Map_Logic);
            Sizeble();
            Vertical_Scroll_Bar v = graf_controls[0] as Vertical_Scroll_Bar;
            v.Elements = new List<My_Control>() { graf_controls[graf_controls.Count - 1], graf_controls[graf_controls.Count - 2] };
            graf_controls.Add(end);
        }
        protected virtual void Create_Level_Logic(object sender, MouseEventArgs e)
        {
            Menu_Backgraund m = graf_controls[graf_controls.Count - 1] as Menu_Backgraund;
            change_forms_screen(new IGame_Rules_Menu_Screen(Current_form, m.Images, this, E_Map_Logic.Level_Logic));
        }
        protected virtual void Create_Map_Logic(object sender, MouseEventArgs e)
        {
            Menu_Backgraund m = graf_controls[graf_controls.Count - 1] as Menu_Backgraund;
            change_forms_screen(new IGame_Rules_Menu_Screen(Current_form, m.Images, this, E_Map_Logic.Map_Logic));
        }
    }
    public class IGame_Rules_Menu_Screen : List_Screen
    {
        E_Map_Logic imap_logic;
        public IGame_Rules_Menu_Screen(A_AScreen_compatible_Form f, List<Image> images, AScreen p_s, E_Map_Logic e) : base(f, images, p_s)
        {
            imap_logic = e;
            My_Control end = graf_controls[graf_controls.Count - 1];
            graf_controls.Remove(end);
            graf_controls.Add(new Menu_Button(0, 0, 50, 50, Color.Gray, "Правила <<Wire World>>"));
            graf_controls[graf_controls.Count - 1].MouseUp += new MouseEventHandler(this.Create_My_Default_Game_Rules);
            graf_controls.Add(new Menu_Button(0, 0, 50, 50, Color.Gray, "Правила <<Game of Life>>"));
            graf_controls[graf_controls.Count - 1].MouseUp += new MouseEventHandler(this.Create_Game_of_Life_Game_Rules);
            Sizeble();
            Vertical_Scroll_Bar v = graf_controls[0] as Vertical_Scroll_Bar;
            v.Elements = new List<My_Control>() { graf_controls[graf_controls.Count - 1], graf_controls[graf_controls.Count - 2] };
            graf_controls.Add(end);
        }
        protected virtual void Create_My_Default_Game_Rules(object sender, MouseEventArgs e)
        {
            Menu_Backgraund m = graf_controls[graf_controls.Count - 1] as Menu_Backgraund;
            change_forms_screen(new Map_Logic_Size_Menu_Screen(Current_form, m.Images, this, imap_logic, new My_Default_Game_Rules()));
        }
        protected virtual void Create_Game_of_Life_Game_Rules(object sender, MouseEventArgs e)
        {
            Menu_Backgraund m = graf_controls[graf_controls.Count - 1] as Menu_Backgraund;
            change_forms_screen(new Map_Logic_Size_Menu_Screen(Current_form, m.Images, this, imap_logic, new Game_of_Life_Game_Rules()));
        }
    }
    public class Map_Logic_Size_Menu_Screen : List_Screen
    {
        E_Map_Logic imap_logic;
        IGame_Rules game_rules;
        public Map_Logic_Size_Menu_Screen(A_AScreen_compatible_Form f, List<Image> images, AScreen p_s, E_Map_Logic m_l, IGame_Rules g_r) : base(f, images, p_s)
        {
            imap_logic = m_l;
            game_rules = g_r;
            My_Control end = graf_controls[graf_controls.Count - 1];
            graf_controls.Remove(end);
            graf_controls.Add(new My_Natural_Number_Text_Box(0, 0, 50, 50, Color.Gray, "Розмір по горизонталі"));
            graf_controls.Add(new My_Natural_Number_Text_Box(0, 0, 50, 50, Color.Gray, "Розмір по вертикалі"));
            graf_controls.Add(new Menu_Button(0, 0, 50, 50, Color.Gray, "Далі"));
            graf_controls[graf_controls.Count - 1].MouseUp += new MouseEventHandler(this.Go_to_next);
            Sizeble();
            Vertical_Scroll_Bar v = graf_controls[0] as Vertical_Scroll_Bar;
            v.Elements = new List<My_Control>() { graf_controls[graf_controls.Count - 1], graf_controls[graf_controls.Count - 2], graf_controls[graf_controls.Count - 3] };
            graf_controls.Add(end);
        }
        public override void replace(int xl, int yl, int xs, int ys)
        {
            base.replace(xl, yl, xs, ys);
            for (int i = 0; i < graf_controls.Count; i++)
            {
                if (graf_controls[i] is My_Natural_Number_Text_Box)
                    graf_controls[i].replace(xl + xs / 4, yl + ys / 4 + (i - 2) * (ys / 2 / 3), xs / 2, 9 * ys / 2 / 30);
                if (graf_controls[i] is Menu_Button && graf_controls[i].Text == "Далі")
                    graf_controls[i].replace(graf_controls[i - 1].Location.X, graf_controls[i - 1].Location.Y + 10 * graf_controls[i - 1].Height / 9, graf_controls[i - 1].Width, graf_controls[i - 1].Height);
            }
            if(graf_controls[graf_controls.Count - 3].Height > 9 * ys / 2 / 30) 
            {
                graf_controls[graf_controls.Count - 3].replace(xl + xs / 4, yl + ys / 2 - graf_controls[graf_controls.Count - 3].Height / 2, xs / 2, 9 * ys / 2 / 30);
                graf_controls[graf_controls.Count - 4].replace(xl + xs / 4, graf_controls[graf_controls.Count - 3].Location.Y - 10 * graf_controls[graf_controls.Count - 4].Height / 9, xs / 2, 9 * ys / 2 / 30);
                graf_controls[graf_controls.Count - 2].replace(xl + xs / 4, graf_controls[graf_controls.Count - 3].Location.Y + 10 * graf_controls[graf_controls.Count - 3].Height / 9, graf_controls[graf_controls.Count - 3].Width, graf_controls[graf_controls.Count - 3].Height);
            }
        }
        protected virtual void Go_to_next(object sender, MouseEventArgs e)
        {
            try
            {
                switch (imap_logic)
                {
                    case E_Map_Logic.Map_Logic:
                        int x = Convert.ToInt32(graf_controls[graf_controls.Count - 3].Text);
                        int y = Convert.ToInt32(graf_controls[graf_controls.Count - 4].Text);
                        change_forms_screen(new Map_Logic_Creator_Menu_Screen(Current_form, this, new Map_Logic(x, y, game_rules)));
                        break;
                    case E_Map_Logic.Level_Logic:
                        break;
                }
            }
            catch { }
        }
    }
    public class Map_Logic_Creator_Menu_Screen : Level_Screen
    {
        protected string path;
        protected List<My_Control> Save_controls = new List<My_Control>()
        {
            new My_Text_Box(0, 0, 50, 50, Color.Gray, "Назва рівня"),
            new Menu_Button(0, 0, 50, 50, Color.Gray, "Зберегти"),
            new Menu_Button(0, 0, 50, 50, Color.Gray, "Відмінити"),
        };
        protected My_Controls.Message error=new My_Controls.Message(0,0,50,50,Color.FromArgb(200,0,0),"Не вдалось зберегти");
        public Map_Logic_Creator_Menu_Screen(A_AScreen_compatible_Form f, AScreen s, Map_Logic m_l) : base(f, s, m_l) 
        {
            path = Application.StartupPath + @"\Levels\Sandbox_Maps\";
            Save_controls[1].MouseUp += new MouseEventHandler(this.Save);
            Save_controls[2].MouseUp += new MouseEventHandler(this.UnShow_Save);

            My_Control end = controls_buttons[controls_buttons.Count - 1];
            controls_buttons.RemoveAt(controls_buttons.Count - 1);
            controls_buttons.Add(new Text_Color_Button(0, 0, 50, 50, Color.DarkGray, Color.Cyan, "ЗБЕ-\nРЕГТИ"));
            controls_buttons[controls_buttons.Count - 1].MouseUp += new MouseEventHandler(this.Show_Save);
            controls_buttons.Add(end);
            while (graf_controls.Count != 3)
            {
                graf_controls.RemoveAt(graf_controls.Count - 1);
            }
            graf_controls.AddRange(controls_buttons);
            Sizeble();
            Vertical_Scroll_Bar v = graf_controls[2] as Vertical_Scroll_Bar;
            v.Elements = controls_buttons;
        }
        public Map_Logic_Creator_Menu_Screen(A_AScreen_compatible_Form f, AScreen s, Level_Logic m_l) : this(f, s, (Map_Logic)m_l)
        {
            path = Application.StartupPath + @"\Levels\Created_Levels\";

            My_Control end = info_buttons[info_buttons.Count - 1];
            info_buttons.RemoveAt(info_buttons.Count - 1);
            if (m_l.win_rules is IPositional_Win_Rules)
            {
                IPositional_Win_Rules w_r = m_l.win_rules as IPositional_Win_Rules;
                for (int i = 0; i < w_r.how_many_position(); i++)
                {
                    info_buttons.Add(new Text_Color_Button(0, 0, 50, 50, Color.DarkGray, Color.FromArgb(50, 50, 255), "ПЕРЕЙТИ\nДО " + i));
                    info_buttons[info_buttons.Count - 1].MouseUp += new MouseEventHandler(this.Go_to_position);
                }
            }
            info_buttons.Add(end);

            end = controls_buttons[controls_buttons.Count - 1];
            controls_buttons.RemoveAt(controls_buttons.Count - 1);
            controls_buttons.Add(new Text_Color_Button(0, 0, 50, 50, Color.DarkGray, Color.Gold, "ДОСТУП\nЗМІН"));
            controls_buttons[controls_buttons.Count - 1].MouseUp += new MouseEventHandler(this.Access);
            controls_buttons.Add(end);
            while (graf_controls.Count != 3)
            {
                graf_controls.RemoveAt(graf_controls.Count - 1);
            }
            graf_controls.AddRange(controls_buttons);
            Sizeble();
            Vertical_Scroll_Bar v = graf_controls[2] as Vertical_Scroll_Bar;
            v.Elements = controls_buttons;
        }
        public override void replace(int xl, int yl, int xs, int ys)
        {
            base.replace(xl, yl, xs, ys);
            Save_controls[0].replace(xl + graf_controls[1].Width / 4, yl + graf_controls[1].Height / 4, graf_controls[1].Width / 2, graf_controls[1].Height / 4);
            Save_controls[0].replace(xl + graf_controls[1].Width / 4, yl + graf_controls[1].Height / 2 - Save_controls[0].Height, graf_controls[1].Width / 2, graf_controls[1].Height / 4);
            Save_controls[1].replace(Save_controls[0].Location.X, Save_controls[0].Location.Y+ Save_controls[0].Height, Save_controls[0].Width / 2, Save_controls[0].Height);
            Save_controls[2].replace(Save_controls[0].Location.X + Save_controls[0].Width / 2, Save_controls[0].Location.Y + Save_controls[0].Height, Save_controls[0].Width / 2, Save_controls[0].Height);
            error.replace(xl + graf_controls[1].Width / 4, yl + graf_controls[1].Height / 4, graf_controls[1].Width / 2, graf_controls[1].Height / 2);
        }
        protected override void Exit(object sender, MouseEventArgs e)
        {
            foreach(My_Control i in Save_controls) 
            {
                Current_form.Controls.Remove(i);
            }
            Current_form.Controls.Remove(error);
            base.Exit(sender,e);
        }
        protected virtual void Save(object sender, MouseEventArgs e)
        {
            try
            {
                Map_Logic_Creator.Save_map_logic(path + Save_controls[0].Text + ".dat", Map);
                List<string>[] levels;
                using (FileStream fs = new FileStream(Application.StartupPath + @"\Levels\Levels.dat", FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    levels = bf.Deserialize(fs) as List<string>[];
                }
                if (Map is Level_Logic)
                {
                    if (levels[2].IndexOf(Save_controls[0].Text + " 0") == -1 && levels[2].IndexOf(Save_controls[0].Text + " 1") == -1)
                    {
                        levels[2].Add(Save_controls[0].Text + " 0");
                    }
                    else 
                    {
                        if(levels[2].IndexOf(Save_controls[0].Text + " 0") != -1) 
                        {
                            levels[2][levels[2].IndexOf(Save_controls[0].Text + " 0")] = Save_controls[0].Text + " 0";
                        }
                        else 
                        {
                            levels[2][levels[2].IndexOf(Save_controls[0].Text + " 1")] = Save_controls[0].Text + " 0";
                        }
                    }
                }
                else 
                {
                    if (levels[1].IndexOf(Save_controls[0].Text) == -1)
                    {
                        levels[1].Add(Save_controls[0].Text);
                    }
                }
                using (FileStream fs = new FileStream(Application.StartupPath + @"\Levels\Levels.dat", FileMode.OpenOrCreate))
                {
                    BinaryFormatter f = new BinaryFormatter();
                    f.Serialize(fs, levels);
                }

                foreach (My_Control i in Save_controls)
                {
                    Current_form.Controls.Remove(i);
                }
                Current_form.Controls.Remove(error);
                Current_form.Controls.Remove(map_info);

                Menu_Backgraund m = previous_screen.graf_controls[previous_screen.graf_controls.Count - 1] as Menu_Backgraund;
                change_forms_screen(new Menu_Screen(Current_form, m.Images));
            }
            catch 
            {
                foreach (My_Control i in graf_controls)
                {
                    if (i is My_Button)
                        i.Enabled = false;
                }
                Current_form.Controls.Add(error);
                error.BringToFront();
                error.MouseUp += new MouseEventHandler(this.Remove_error);
            }
        }
        protected virtual void Remove_error(object sender, MouseEventArgs e) 
        {
            Current_form.Controls.Remove(error);
            foreach (My_Control i in graf_controls)
            {
                if (i is My_Button)
                    i.Enabled = true;
            }
        }
        protected virtual void Show_Save(object sender, MouseEventArgs e)
        {
            foreach(My_Control i in Save_controls) 
            {
                Current_form.Controls.Add(i);
                i.BringToFront();
            }
            foreach (My_Control i in graf_controls)
            {
                if (i is My_Button && i.Text != "ВИХІД")
                    i.Enabled = false;
            }
        }
        protected virtual void UnShow_Save(object sender, MouseEventArgs e)
        {
            foreach (My_Control i in Save_controls)
            {
                Current_form.Controls.Remove(i);
            }
            foreach (My_Control i in graf_controls)
            {
                if(i is My_Button)
                    i.Enabled = true;
            }
        }
        protected virtual void Access(object sender, MouseEventArgs e)
        {
            color_buttons = new List<My_Control>() { new Color_Button(0, 0, 50, 50, Color.Black), new Color_Button(0, 0, 50, 50, Color.White) };
            color_buttons[0].MouseUp += new MouseEventHandler(this.Set_Accessibility);
            color_buttons[1].MouseUp += new MouseEventHandler(this.Set_Accessibility);

            My_Control end = controls_buttons[controls_buttons.Count - 1];
            controls_buttons.RemoveAt(controls_buttons.Count - 1);
            controls_buttons.Remove(controls_buttons[controls_buttons.Count - 1]);
            controls_buttons.Add(end);
            while (graf_controls.Count != 3)
            {
                graf_controls.RemoveAt(graf_controls.Count - 1);
            }
            graf_controls.AddRange(controls_buttons);
            Sizeble();
            Vertical_Scroll_Bar v = graf_controls[2] as Vertical_Scroll_Bar;
            v.Elements = controls_buttons;
            Type_Visual_Map t_v = graf_controls[1] as Type_Visual_Map;
            t_v.simulation_enabled = false;
            graf_controls[1] = new Accessibility_Visual_Map((Level_Logic)Map);
        }
        protected virtual void Go_to_position(object sender, MouseEventArgs e)
        {
            Text_Color_Button t = sender as Text_Color_Button;
            Type_Visual_Map visual_map = graf_controls[1] as Type_Visual_Map;
            Level_Logic l_l = visual_map.Map as Level_Logic;
            IPositional_Win_Rules w_r = l_l.win_rules as IPositional_Win_Rules;
            w_r.go_to_position(l_l, Convert.ToInt32(t.Text.Split(' ')[1]));
            visual_map.Invalidate();
        }
        protected virtual void Set_Accessibility(object sender, MouseEventArgs e)
        {
            Color_Button s = sender as Color_Button;
            controls_buttons[2].BackColor = s.BackColor;
            make_invisible();
            while (graf_controls.Count != 3)
            {
                graf_controls.RemoveAt(graf_controls.Count - 1);
            }
            graf_controls.AddRange(controls_buttons);
            Vertical_Scroll_Bar v = graf_controls[2] as Vertical_Scroll_Bar;
            Sizeble();
            v.Elements = controls_buttons;
            make_visible();
            Accessibility_Visual_Map v_m = graf_controls[1] as Accessibility_Visual_Map;
            v_m.current_type_of_cell = controls_buttons[2].BackColor == Color.White;
        }
    }
}
