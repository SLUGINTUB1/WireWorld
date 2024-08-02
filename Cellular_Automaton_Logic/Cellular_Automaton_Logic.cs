using System;
using System.Drawing;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Cellular_Automaton_Logic
{
    public interface IGame_Rules
    {
        string info();
        int make_rule(int x, int y, Map_Logic map_log);
        int default_cell_vaule();
        bool is_this_right_cell(int cell);
        bool is_this_right_Arrey(int[,] arrey);
        Color color_of_cell(int cell);
        int cell_of_color(Color color);
        Color[] all_colors_of_cell();
    }
    [Serializable]
    public class My_Default_Game_Rules : IGame_Rules
    {
        public string info() 
        {
            return "сіра->сірa\nсиня->червона\nчервона->жовта\nжовта->жовта але синя якщо поряд є 1 або 2 сині";
        }
        public int make_rule(int x, int y, Map_Logic map_log)
        {
            int cell;
            switch (map_log.this_move[x, y])
            {
                case 0:
                    cell = 0;
                    break;
                case 1:
                    cell = 1;
                    int k = 0;
                    for (int i = -1; i <= 1; i++)
                    {
                        for (int j = -1; j <= 1; j++)
                        {
                            if ((i == 0 && j == 0) || x + i >= map_log.x_size || y + j >= map_log.y_size || x + i < 0 || y + j < 0)
                                continue;
                            if (map_log.this_move[x + i, y + j] == 2)
                                k++;
                        }
                    }
                    if (k == 1 || k == 2)
                        cell = 2;
                    break;
                case 2:
                    cell = 3;
                    break;
                case 3:
                    cell = 1;
                    break;
                default:
                    throw new Exception("клітина не передбачена правилами");
            }
            return cell;
        }
        public int default_cell_vaule()
        {
            return 0;
        }
        public bool is_this_right_cell(int cell)
        {
            return cell >= 0 && cell <= 3;
        }
        public bool is_this_right_Arrey(int[,] arrey)
        {
            for (int x = 0; x < arrey.GetUpperBound(0) + 1; x++)
            {
                for (int y = 0; y < arrey.GetUpperBound(1) + 1; y++)
                {
                    if (!is_this_right_cell(arrey[x, y]))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        public Color color_of_cell(int cell)
        {
            switch (cell)
            {
                case 0:
                    return Color.Gray;
                case 1:
                    return Color.Yellow;
                case 2:
                    return Color.Blue;
                case 3:
                    return Color.Red;
                default:
                    throw new Exception("непередбачена правилами клітина");
            }
        }
        public Color[] all_colors_of_cell()
        {
            return new Color[] { Color.Gray, Color.Yellow, Color.Blue, Color.Red };
        }
        public int cell_of_color(Color color)
        {
            if (color == Color.Gray)
                return 0;
            if (color == Color.Yellow)
                return 1;
            if (color == Color.Blue)
                return 2;
            if (color == Color.Red)
                return 3;
            throw new Exception("непередбачена правилами клітина");
        }
    }
    [Serializable]
    public class Game_of_Life_Game_Rules : IGame_Rules
    {
        public string info()
        {
            return "біла->біла якщо є 2 або 3 білих сусіди\nбіла->сіра якщо наміє 2 або 3 білих сусідів\nсіра->біла якщо 3 білих сусіда\nсіра->сіра якщо не 3 білих сусіда";
        }
        public int make_rule(int x, int y, Map_Logic map_log)
        {
            int cell;
            switch (map_log.this_move[x, y])
            {
                case 0:
                    {
                        cell = 0;
                        int k = 0;
                        for (int i = -1; i <= 1; i++)
                        {
                            for (int j = -1; j <= 1; j++)
                            {
                                if ((i == 0 && j == 0) || x + i >= map_log.x_size || y + j >= map_log.y_size || x + i < 0 || y + j < 0)
                                    continue;
                                if (map_log.this_move[x + i, y + j] == 1)
                                    k++;
                            }
                        }
                        if (k == 3)
                        cell = 1; 
                    }
                    break;
                case 1:
                    {
                        cell = 0;
                        int k = 0;
                        for (int i = -1; i <= 1; i++)
                        {
                            for (int j = -1; j <= 1; j++)
                            {
                                if ((i == 0 && j == 0) || x + i >= map_log.x_size || y + j >= map_log.y_size || x + i < 0 || y + j < 0)
                                    continue;
                                if (map_log.this_move[x + i, y + j] == 1)
                                    k++;
                            }
                        }
                        if (k == 2 || k == 3)
                            cell = 1;
                    }
                    break;
                default:
                    throw new Exception("клітина не передбачена правилами");
            }
            return cell;
        }
        public int default_cell_vaule()
        {
            return 0;
        }
        public bool is_this_right_cell(int cell)
        {
            return cell == 0 || cell == 1;
        }
        public bool is_this_right_Arrey(int[,] arrey)
        {
            for (int x = 0; x < arrey.GetUpperBound(0) + 1; x++)
            {
                for (int y = 0; y < arrey.GetUpperBound(1) + 1; y++)
                {
                    if (!is_this_right_cell(arrey[x, y]))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        public Color color_of_cell(int cell)
        {
            switch (cell)
            {
                case 0:
                    return Color.FromArgb(30, 30, 30);
                case 1:
                    return Color.FromArgb(200, 200, 200);
                default:
                    throw new Exception("непередбачена правилами клітина");
            }
        }
        public Color[] all_colors_of_cell()
        {
            return new Color[] { Color.FromArgb(30, 30, 30), Color.FromArgb(200, 200, 200) };
        }
        public int cell_of_color(Color color)
        {
            if (color == Color.FromArgb(30, 30, 30))
                return 0;
            if (color == Color.FromArgb(200, 200, 200))
                return 1;
            throw new Exception("непередбачена правилами клітина");
        }
    }
    public interface IWin_Rules
    {
        string info();
        bool is_it_win_situation(Map_Logic map_log);
    }
    public interface IPositional_Win_Rules
    {
        void go_to_position(Map_Logic map_logic, int position);
        int how_many_position();
    }
    [Serializable]
    public class My_Win_Rules : IWin_Rules, IPositional_Win_Rules
    {
        public My_Win_Rule[] rules { private set; get; }
        private string text_info;
        public My_Win_Rules(string t_i, int n, string[][] info)
        {
            text_info = t_i;
            rules = new My_Win_Rule[n];
            for (int i = 0; i < n; i++)
            {
                rules[i] = new My_Win_Rule(info[i]);
            }
        }
        public My_Win_Rules(string t_i, My_Win_Rule[] r)
        {
            text_info = t_i;
            if (r == null)
            {
                throw new ArgumentNullException("Масив не може бути нульовим");
            }
            if (r.Length == 0)
            {
                throw new ArgumentException("Довжина масиву повинна бути більше нуля");
            }
            foreach(My_Win_Rule i in r) 
            {
                if (i == null)
                    throw new Exception("My_Win_Rule не може бути нульовим");
            }
            rules = r;

        }
        public string info()
        {
            return text_info;
        }
        public bool is_it_win_situation(Map_Logic map_log)
        {
            foreach (My_Win_Rule i in rules)
            {
                if (!i.is_it_win_situation(map_log))
                    return false;
            }
            return true;
        }
        public void go_to_position(Map_Logic map_logic, int position)
        {
            if (position < 0) 
            {
                throw new Exception("неможлива позиція");
            }
            int zsuv = 0;
            foreach(My_Win_Rule i in rules) 
            {
                if(position - (zsuv + i.how_many_position()) < 0) 
                {
                    i.go_to_position(map_logic, position - zsuv);
                    break;
                }
                else 
                {
                    zsuv += i.how_many_position();
                }
            }
        }
        public int how_many_position()
        {
            int n = 0;
            foreach (My_Win_Rule i in rules)
            {
                n += i.how_many_position();
            }
            return n;
        }
    }
    [Serializable]
    public class My_Win_Rule : IWin_Rules, IPositional_Win_Rules
    {
        public int[,] input_cells { private set; get; }
        public int[,] output_cells { private set; get; }
        public int[,] rule { private set; get; }
        private string text_info;
        public My_Win_Rule(string[] s)
        {
            string[] input = s[0].Split(';')[0].Split(','), output = s[0].Split(';')[1].Split(',');
            input_cells = new int[input.Length, 2]; output_cells = new int[output.Length, 2];
            for (int i = 0; i < input.Length; i++)
            {
                input_cells[i, 0] = Convert.ToInt32(input[i].Split(' ')[0]);
                input_cells[i, 1] = Convert.ToInt32(input[i].Split(' ')[1]);
            }
            for (int i = 0; i < output.Length; i++)
            {
                output_cells[i, 0] = Convert.ToInt32(output[i].Split(' ')[0]);
                output_cells[i, 1] = Convert.ToInt32(output[i].Split(' ')[1]);
            }
            rule = new int[s.Length - 1, input_cells.Length / 2 + output_cells.Length / 2 + 1];
            for (int i = 0; i < s.Length - 1; i++)
            {
                rule[i, input_cells.Length / 2 + output_cells.Length / 2] = Convert.ToInt32(s[i + 1].Split(';')[2]);
                input = s[i + 1].Split(';')[0].Split(','); output = s[i + 1].Split(';')[1].Split(',');
                for (int j = 0; j < input.Length; j++)
                {
                    rule[i, j] = Convert.ToInt32(input[j]);
                }
                for (int j = 0; j < output.Length; j++)
                {
                    rule[i, j + input.Length] = Convert.ToInt32(output[j]);
                }
            }
        }
        public My_Win_Rule(int[,] i_c, int[,] o_c, int[,] r,string s)
        {
            if (i_c == null || o_c == null || r == null)
                throw new Exception("Масив не може бути нульовим");
            if (i_c.GetUpperBound(1) + 1 != 2 || o_c.GetUpperBound(1) + 1 != 2 || (r.GetUpperBound(1) + 1 - (i_c.GetUpperBound(0) + 1)) % (o_c.GetUpperBound(0) + 1 + 1) != 0)
                throw new Exception("неправильні розміри масивів");
            input_cells = i_c;
            output_cells = o_c;
            rule = r;
            text_info = s;
        }
        public string info()
        {
            return text_info;
        }
        public void go_to_position(Map_Logic map_logic, int position)
        {
            int[,] map = map_logic.this_move;
            for (int i = 0; i < input_cells.Length / 2; i++)
            {
                if (map_logic.game_rules.is_this_right_cell(rule[position, i]))
                    map[input_cells[i, 0], input_cells[i, 1]] = rule[position, i];
                else throw new Exception("непарвильні данні в правилах перемоги");
            }
        }
        public int how_many_position()
        {
            return rule.GetUpperBound(0) + 1;
        }
        public bool is_it_win_situation(Map_Logic map_logic)
        {
            try
            {
                Map_Logic map = new Map_Logic(map_logic.this_move, map_logic.game_rules);
                for (int j = 0; j < rule.GetUpperBound(0) + 1; j++)
                {
                    for (int n = 1; n <= (rule.GetUpperBound(1) + 1 - (input_cells.Length / 2)) / (output_cells.Length / 2 + 1); n++)
                    {
                        map.to_default();
                        for (int i = 0; i < input_cells.Length / 2; i++)
                        {
                            map.make_change_cell(input_cells[i, 0], input_cells[i, 1], rule[j, i]);
                        }
                        for (int i = 0; i < rule[j, input_cells.Length / 2 + (n * output_cells.Length / 2) + (n - 1)]; i++)
                        {
                            map.make_move();
                        }
                        for(int i = 0;i< output_cells.Length / 2; i++) 
                        {
                            if(map.this_move[output_cells[i,0], output_cells[i, 1]]!=rule[j, input_cells.Length / 2+(n-1)*(output_cells.Length / 2+1) + i])
                                return false;
                        }
                    }
                }
                return true;
            }
            catch
            {
                throw new Exception("Правило неможливо використати");
            }
        }
    }
    //
    [Serializable]
    public class Map_Logic
    {
        public int x_size { protected set; get; }
        public int y_size { protected set; get; }
        protected internal int[,] default_move { private protected set; get; }
        protected internal int[,] this_move { private protected set; get; }
        protected int[,] next_move;
        public int move_number { protected set; get; }
        public IGame_Rules game_rules { private set; get; }
        public Map_Logic(int[,] d_m, IGame_Rules g_r)
        {
            if (g_r.is_this_right_Arrey(d_m))
            {
                move_number = 0;
                x_size = d_m.GetUpperBound(0) + 1;
                y_size = d_m.GetUpperBound(1) + 1;
                default_move = d_m;
                to_default();
                next_move = new int[x_size, y_size];
                game_rules = g_r;
            }
            else
            {
                throw new Exception("дефолтний масив містить не передбачені правилами елементи");
            }
        }
        public Map_Logic(int x_s, int y_s, IGame_Rules g_r)
        {
            move_number = 0;
            x_size = x_s;
            y_size = y_s;
            default_move = new int[x_s, y_s];
            for (int x = 0; x < x_s; x++)
                for (int y = 0; y < y_s; y++)
                    default_move[x, y] = g_r.default_cell_vaule();
            to_default();
            next_move = new int[x_size, y_size];
            game_rules = g_r;
        }
        public virtual void to_default()
        {
            move_number = 0;
            if(this_move == null) 
            {
                this_move = new int[x_size, y_size];
            }
            Array.Copy(default_move,0,this_move,0,this_move.Length);
        }
        public virtual void make_move()
        {
            move_number++;
            for (int x = 0; x < x_size; x++)
                for (int y = 0; y < y_size; y++)
                    next_move[x, y] = game_rules.make_rule(x, y, this);
            this_move = next_move;
            next_move = new int[x_size, y_size];
        }
        public virtual void make_change_cell(int x, int y, int cell)
        {
            if (game_rules.is_this_right_cell(cell))
            {
                this_move[x, y] = cell;
            }
            else
            {
                throw new Exception("ваша клітина не передбачена правилами");
            }
        }
        public virtual Color color_of_cell(int x, int y)
        {
            return game_rules.color_of_cell(value_of_cell(x, y));
        }
        public virtual int value_of_cell(int x, int y)
        {
            return this_move[x, y];
        }
        public virtual string info() 
        {
            return "Правила гри:\n" + game_rules.info();
        }
    }
    [Serializable]
    public class Level_Logic : Map_Logic
    {
        public bool is_it_solved { private set; get; }
        public IWin_Rules win_rules { private set; get; }
        protected internal bool[,] enable_table { private set; get; }
        public Level_Logic(int[,] d_m, bool[,] e_t, IGame_Rules g_r, IWin_Rules w_r) : base(d_m, g_r)
        {
            if (x_size == e_t.GetUpperBound(0) + 1 && y_size == e_t.GetUpperBound(1) + 1)
            {
                enable_table = e_t;
                win_rules = w_r;
                is_it_solved = false;
            }
            else
            {
                throw new Exception("початкове положення клітин не відповідає по розміру до масиву доступу");
            }
        }
        public override void make_change_cell(int x, int y, int cell)
        {
            if (enable_table[x, y])
                base.make_change_cell(x, y, cell);
        }
        public override Color color_of_cell(int x, int y)
        {
            if (enable_table[x, y])
            {
                return base.color_of_cell(x, y);
            }
            else
            {
                return Color.FromArgb(base.color_of_cell(x, y).R / 2, base.color_of_cell(x, y).G / 2, base.color_of_cell(x, y).B / 2);
            }
        }
        public virtual void enable_table_cell(int x, int y, bool b) 
        {
            enable_table[x, y] = b;
        }
        public virtual void check()
        {
            is_it_solved = win_rules.is_it_win_situation(this);
        }
        public override string info()
        {
            return base.info()+"\nПравила перемоги:\n"+ win_rules.info();
        }
    }
    //
    public static class Map_Logic_Creator
    {
        public static Map_Logic Crate_map_logic(string[] level_info)
        {
            switch (level_info[0])
            {
                case "Map_Logic":
                    {
                        int xsize = Convert.ToInt32(level_info[1].Split(' ')[0]), ysize = Convert.ToInt32(level_info[1].Split(' ')[1]);
                        int[,] d_m = new int[xsize, ysize];
                        int zsuv = 1; bool is_it_empty = true;
                        if (level_info[2] != "empty")
                        {
                            for (int y_ = 0; y_ < ysize; y_++)
                            {
                                for (int x_ = 0; x_ < xsize; x_++)
                                {
                                    d_m[x_, y_] = Convert.ToInt32(level_info[2 + y_].Split(' ')[x_]);
                                }
                            }
                            zsuv += ysize; is_it_empty = false;
                        }
                        else
                        {
                            zsuv++;
                        }
                        switch (level_info[2 + zsuv])
                        {
                            case "My_Default_Game_Rules":
                                {
                                    if (is_it_empty)
                                    {
                                        return new Map_Logic(xsize, ysize, new My_Default_Game_Rules());
                                    }
                                    else
                                    {
                                        return new Map_Logic(d_m, new My_Default_Game_Rules());
                                    }
                                }
                            default:
                                throw new Exception("Вказаний непередбачений нащадок IGame_Rules");
                        }
                    }
                case "Level_Logic":
                    {
                        int xsize = Convert.ToInt32(level_info[1].Split(' ')[0]), ysize = Convert.ToInt32(level_info[1].Split(' ')[1]);
                        int[,] d_m = new int[xsize, ysize]; bool[,] e_t = new bool[xsize, ysize];
                        for (int y_ = 0; y_ < ysize; y_++)
                        {
                            for (int x_ = 0; x_ < xsize; x_++)
                            {
                                d_m[x_, y_] = Convert.ToInt32(level_info[2 + y_].Split(' ')[x_]);
                            }
                        }
                        for (int y_ = 0; y_ < ysize; y_++)
                        {
                            for (int x_ = 0; x_ < xsize; x_++)
                            {
                                e_t[x_, y_] = Convert.ToBoolean(level_info[3 + ysize + y_].Split(' ')[x_]);
                            }
                        }
                        IGame_Rules g_r; int zsuv;
                        switch (level_info[4 + 2 * ysize])
                        {
                            case "My_Default_Game_Rules":
                                {
                                    g_r = new My_Default_Game_Rules();
                                    zsuv = 0;
                                }
                                break;
                            default:
                                throw new Exception("Вказаний непередбачений нащадок IGame_Rules");
                        }
                        IWin_Rules w_r;
                        switch (level_info[6 + 2 * ysize + zsuv])
                        {
                            case "My_Win_Rules":
                                {
                                    string rules_jumps = level_info[6 + 2 * ysize + zsuv + 2];
                                    string[][] win_info = new string[rules_jumps.Split(',').Length][];
                                    for (int i = 0, j = 0; i < rules_jumps.Split(',').Length; i++)
                                    {
                                        win_info[i] = new string[Convert.ToInt32(rules_jumps.Split(',')[i])];
                                        Array.Copy(level_info, 6 + 2 * ysize + zsuv + 3 + j, win_info[i], 0, Convert.ToInt32(rules_jumps.Split(',')[i]));
                                        j += Convert.ToInt32(rules_jumps.Split(',')[i]);
                                    }
                                    w_r = new My_Win_Rules(level_info[6 + 2 * ysize + zsuv + 1], rules_jumps.Split(',').Length, win_info);
                                }
                                break;
                            default:
                                throw new Exception("Вказаний непередбачений нащадок IWin_Rules");
                        }
                        return new Level_Logic(d_m, e_t, g_r, w_r);
                    }
                default:
                    throw new Exception("Вказаний непередбачений нащадок IMap_Logic");
            }
        }
        public static Map_Logic Crate_map_logic(string path) 
        {
            BinaryFormatter f = new BinaryFormatter();
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                return (Map_Logic)f.Deserialize(fs);
            }
        }
        public static void Save_map_logic(string path, Map_Logic m_l)
        {
            BinaryFormatter f = new BinaryFormatter();
            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
            {
                switch (m_l)
                {
                    case Level_Logic l:
                        l = m_l as Level_Logic;
                        f.Serialize(fs, new Level_Logic(l.this_move, l.enable_table, l.game_rules, l.win_rules));
                        break;
                    default:
                        f.Serialize(fs, new Map_Logic(m_l.this_move, m_l.game_rules));
                        break;
                }
            }
        }
    }
}