using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameOfLifeWindowsForms
{
    /// <summary>
    /// This should be self-explanatory: N = "North", E = "East", S = "South", W = "West"
    /// </summary>
    public enum Direction
    {
        N, NE, E, SE, S, SW, W, NW
    }

    /// <summary>
    /// Our form for showing the cellular automaton simulations (e.g. "Conway's Game of Life").
    /// </summary>
    public partial class Form1 : Form
    {
        //GUI format configuration:

        /// <summary>
        /// The configuration of the GUI is defined in this class. 
        /// Any changes to this configuration will either result in a 'RebuildGui' or 'ReformatGui' call on the Form1 form. 
        /// 'RebuildGui' will be necessary when the fundamental GUI elements have changed in size, position or quantity. 
        /// 'ReformatGui' will be enough in situations where only colors or texts have changed.
        /// </summary>
        public class GuiConfig
        {
            //the form for which we will call 'RebuildGui' or 'ReformatGui'
            protected Form1 form;

            //c'tor (variant 1)
            public GuiConfig(Form1 form)
            {
                this.form = form;
            }

            /*
            //c'tor (variant 2)
            public GuiConfig(Form1 form, int cellSize, int topLeftX, int topLeftY,
                string alifeText, string deadText, Color alifeColor, Color deadColor,
                string runningText, string stoppedText, Color runningColor, Color stoppedColor,
                string generationText, CultureInfo generationTextCulture, Color generationTextColor) : this(form)
            {
                this.cellSize = cellSize;
                this.topLeftX = topLeftX;
                this.topLeftY = topLeftY;
                this.alifeText = alifeText;
                this.deadText = deadText;
                this.alifeColor = alifeColor;
                this.deadColor = deadColor;
                this.runningText = runningText;
                this.stoppedText = stoppedText;
                this.runningColor = runningColor;
                this.stoppedColor = stoppedColor;
                this.generationText = generationText;
                this.generationTextCulture = generationTextCulture;
                this.generationTextColor = generationTextColor;
            }
            */

            //state:

            /// <summary>
            /// The size of the cells (width and height) in pixels.
            /// </summary>
            public int CellSize 
            { 
                get => cellSize;
                set { cellSize = value; form.RebuildGui(); }
            }
            protected int cellSize = 23;

            /// <summary>
            /// The x coordinate of the top-left position of our cells field.
            /// </summary>
            public int TopLeftX
            { 
                get => topLeftX;
                set { topLeftX = value; form.RebuildGui(); }
            }
            protected int topLeftX = 3;

            /// <summary>
            /// The y coordinate of the top-left position of our cells field.
            /// </summary>
            public int TopLeftY
            {
                get => topLeftY;
                set { topLeftY = value; form.RebuildGui(); }
            }
            protected int topLeftY = 25;

            /// <summary>
            /// We display this text, when cell is alife (more than 0% life).
            /// </summary>
            public string AlifeText 
            { 
                get => alifeText;
                set { alifeText = value; form.ReformatGui(); }
            }
            protected string alifeText = "X";

            /// <summary>
            /// We display this text, when cell is dead (at 0% life).
            /// </summary>
            public string DeadText
            {
                get => deadText;
                set { deadText = value; form.ReformatGui(); }
            }
            protected string deadText = "";

            /// <summary>
            /// We display this color, when cell is (100%) alife.
            /// </summary>
            public Color AlifeColor
            {
                get => AlifeColor;
                set { alifeColor = value; form.ReformatGui(); }
            }
            protected Color alifeColor = Color.Green;

            /// <summary>
            /// We display this color, when cell is (100%) dead.
            /// </summary>
            public Color DeadColor
            {
                get => DeadColor;
                set { deadColor = value; form.ReformatGui(); }
            }
            protected Color deadColor = Color.Transparent;

            /// <summary>
            /// We display this text, when the simulation is running.
            /// </summary>
            public string RunningText
            {
                get => runningText;
                set { runningText = value; form.ReformatGui(); }
            }
            protected string runningText = "RUNNING";

            /// <summary>
            /// We display this text, when the simulation is stopped.
            /// </summary>
            public string StoppedText
            {
                get => stoppedText;
                set { stoppedText = value; form.ReformatGui(); }
            }
            protected string stoppedText = "STOPPED";

            /// <summary>
            /// We display this color, when the simulation is running.
            /// </summary>
            public Color RunningColor
            {
                get => runningColor;
                set { runningColor = value; form.ReformatGui(); }
            }
            protected Color runningColor = Color.DarkGreen;

            /// <summary>
            /// We display this color, when the simulation is stopped.
            /// </summary>
            public Color StoppedColor
            {
                get => stoppedColor;
                set { stoppedColor = value; form.ReformatGui(); }
            }
            protected Color stoppedColor = Color.DarkRed;

            /// <summary>
            /// This formatting string will be used to display the information "which generation do we currently have?". 
            /// The string will be used with 'String.Format()'. It is expected to receive one value of type integer.
            /// </summary>
            public string GenerationText
            {
                get => generationText;
                set { generationText = value; form.ReformatGui(); }
            }
            protected string generationText = "generation: {0,0.0}";

            /// <summary>
            /// More information about the formatting string. In some cultures the decimal separator is a point, not a 
            /// comma (as in US English). So, in an Englsih GUI the number 12334521 will be shown as "12,334,521", but 
            /// in a German GUI the number will be shown as "12.334.521".
            /// </summary>
            public CultureInfo GenerationTextCulture
            {
                get => generationTextCulture;
                set { generationTextCulture = value; form.ReformatGui(); }
            }
            protected CultureInfo generationTextCulture = CultureInfo.InvariantCulture;

            /// <summary>
            /// We display this color, when showing the current generation number.
            /// </summary>
            public Color GenerationTextColor
            {
                get => generationTextColor;
                set { generationTextColor = value; form.ReformatGui(); }
            }
            protected Color generationTextColor = Color.DarkBlue;
        }

        protected GuiConfig guiConf;
        protected GuiConfig Conf
        {
            get => guiConf;
            set { guiConf = value; RebuildGui(); }
        }


        //dynamic GUI elements:

        //1st dim = "X"
        //2nd dim = "Y"
        //(0,0) = "upper left corner"
        Button[,] buttons;


        //data and logic:

        //our data (type depends on the simulation type)
        object data = new GenericRingBuffer3D<bool>(32, 32, 1000);

        
        bool wrap = true;

        //is simulation RUNNING or STOPPED?
        bool running = false;


        //c'tor
        public Form1()
        {
            InitializeComponent();
        }

        protected void Form1_Load(object sender, EventArgs e)
        {
            //establish a gui config
            guiConf = new GuiConfig(this);


            //pull default values into the UI elements

            PullValuesFromModelToView();


            //data and simulation delegate

            simulation = AdvanceSimulation1;


            //create cell grafix

            Button[,] newButtons = CreateButtons(cnumX, cnumY);
            ConnectButtons(newButtons);
            ResizeButtons();//just make sure, font looks always the same
        }

        protected void RebuildGui()
        {

        }

        protected void PullValuesFromModelToView()
        {
            Num_BoardXY_Ignore = true;
            Num_BoardX.Value = cnumX;
            Num_BoardY.Value = cnumY;
            Num_BoardXY_Ignore = false;

            Num_SizeXY_Ignore = true;
            Num_SizeXY.Value = csize;
            Num_SizeXY_Ignore = false;

            Chk_Wrap_Ignore = true;
            Chk_Wrap.Checked = wrap;
            Chk_Wrap_Ignore = false;

            UpdateStatus();
        }


        protected void UpdateStatus()
        {
            if (running)
            {
                Lbl_Status.ForeColor = Color.DarkGreen;
                Lbl_Status.Text = "RUNNING";
            }
            else
            {
                Lbl_Status.ForeColor = Color.DarkRed;
                Lbl_Status.Text = "STOPPED";
            }
        }

        protected void AdvanceSimulation1()
        {

        }

        protected void AdvanceSimulation2()
        {

        }

        protected bool IsAlife(Button button)
        {
            for (int i = 0; i < cnumX; i++)
                for (int j = 0; j < cnumY; j++)
                    if (buttons[i, j] == button)
                        return IsAlife(i, j);
            throw new ArgumentException($"button with Text \"{button.Text}\" is not a cell!");
        }

        protected void SetAlife(Button button, bool alife = true)
        {
            button.Text = alife ? alifeText : "";
        }

        protected bool IsAlife(int x, int y)
        {
            while (x < 0)
                x += cnumX;
            while (y < 0)
                y += cnumY;
            if (x >= cnumX)
                x %= cnumX;
            if (y >= cnumY)
                y %= cnumY;
            return IsAlife(buttons[x, y]);
        }

        protected bool IsAlife(int x, int y, Direction dir)
        {
            int x2 = x, y2 = y;
            switch (dir)
            {
                case Direction.N: y2--; break;
                case Direction.NE: x2++; y2--; break;
                case Direction.E: x2++; break;
                case Direction.SE: x2++; y2++; break;
                case Direction.S: y2++; break;
                case Direction.SW: x2--; y2++; break;
                case Direction.W: x2--; break;
                case Direction.NW: x2--; y2--; break;
            }
            return IsAlife(x2, y2);
        }

        protected int GetNumAlife(int x, int y)
        {
            int num = 0;
            if (IsAlife(x, y, Direction.N)) num++;
            if (IsAlife(x, y, Direction.NE)) num++;
            if (IsAlife(x, y, Direction.E)) num++;
            if (IsAlife(x, y, Direction.SE)) num++;
            if (IsAlife(x, y, Direction.S)) num++;
            if (IsAlife(x, y, Direction.SW)) num++;
            if (IsAlife(x, y, Direction.W)) num++;
            if (IsAlife(x, y, Direction.NW)) num++;
            return num;
        }

        protected void EnsureOutputBuffer()
        {
            if (outputBuffer == null)
                outputBuffer = new bool[cnumX, cnumY];
        }

        protected void RunSimulation()
        {
            EnsureOutputBuffer();

            int neighbors;
            for(int x = 0; x < cnumX; x++)
                for(int y = 0; y < cnumY; y++)
                {
                    neighbors = GetNumAlife(x, y);
                    if (IsAlife(x, y))
                    {//cell is alive
                        if (neighbors == 2 || neighbors == 3)
                            outputBuffer[x, y] = true;
                        else
                            outputBuffer[x, y] = false;
                    }
                    else
                    {//cell is dead
                        if (neighbors == 3)
                            outputBuffer[x, y] = true;
                        else
                            outputBuffer[x, y] = false;
                    }
                }

            for(int x = 0; x < cnumX; x++)
                for(int y = 0; y < cnumY; y++)
                {
                    SetAlife(buttons[x, y], outputBuffer[x, y]);
                }
        }

        protected Button[,] CreateButtons(int numX, int numY)
        {
            Button[,] buttons = new Button[numX, numY];
            Button button;
            for (int x = 0; x < numX; x++)
                for (int y = 0; y < numY; y++)
                {
                    button = new Button();
                    buttons[x, y] = button;
                    button.Name = "button(" + x + "," + y + ")";

                    button.Size = new Size(csize, csize);
                    button.Location = new Point(
                        topLeftX + x * csize,
                        topLeftY + y * csize);
                    button.Margin = new Padding(0);
                    button.Padding = new Padding(0);

                    button.UseVisualStyleBackColor = true;
                }
            return buttons;
        }

        protected void ConnectButtons(Button[,] buttons)
        {
            this.buttons = buttons;
            foreach(Button button in buttons)
            {
                button.Click += new System.EventHandler(this.button1_Click);
                this.Controls.Add(button);
            }
        }

        protected void RemoveButtons()
        {
            foreach(Button button in buttons)
            {
                button.Click -= new System.EventHandler(this.button1_Click);
                this.Controls.Remove(button);
            }
        }

        protected void ResizeField(int newX, int newY)
        {
            if (buttons == null)
                return;

            //- create new button-Array
            Button[,] newButtons = CreateButtons(newX, newY);

            //- copy values from old button-Array
            int maxX = cnumX > newX ? newX : cnumX;
            int maxY = cnumY > newY ? newY : cnumY;
            for(int x = 0; x < maxX; x++)
                for(int y = 0; y < maxY; y++)
                    newButtons[x, y].Text = buttons[x, y].Text;

            //- remove old buttons
            RemoveButtons();

            //- update the size information HERE
            cnumX = newX;
            cnumY = newY;

            //- connect new buttons
            ConnectButtons(newButtons);

            //- replace the 'outputBuffer' by apropriate array
            outputBuffer = null;
            EnsureOutputBuffer();
        }

        protected void ResizeButtons()
        {
            csize = (int) Num_SizeXY.Value;

            Font oldFont = buttons[0, 0].Font;
            //float oldSize = oldFont.Size;
            float newSize = csize / 3f;
            Font newFont = new Font(
                    oldFont.FontFamily.Name,
                    newSize,
                    oldFont.Style
                );

            Button button;
            for (int x = 0; x < cnumX; x++)
                for (int y = 0; y < cnumY; y++)
                {
                    button = buttons[x, y];

                    button.Size = new Size(csize, csize);
                    button.Location = new Point(
                        topLeftX + x * csize,
                        topLeftY + y * csize);
                    button.Margin = new Padding(0);
                    button.Padding = new Padding(0);

                    button.Font = newFont;
                }
        }

        protected void button1_Click(object sender, EventArgs e)
        {
            if(sender is Button)
            {
                Button button = (sender as Button);
                if (IsAlife(button))
                    SetAlife(button, false);
                else
                    SetAlife(button, true);
            }
        }

        protected void timer1_Tick(object sender, EventArgs e)
        {
            if(running)
                RunSimulation();
        }

        protected void Cmd_StartStop_Click(object sender, EventArgs e)
        {
            running = !running;
            UpdateStatus();
        }

        protected bool Num_SizeXY_Ignore = false;
        protected void Num_SizeXY_ValueChanged(object sender, EventArgs e)
        {
            if (Num_SizeXY_Ignore)
                return;
            ResizeButtons();
        }

        protected bool Num_BoardXY_Ignore = false;
        protected void Num_BoardXY_ValueChanged(object sender, EventArgs e)
        {
            if (Num_BoardXY_Ignore)
                return;
            int newX = (int)Num_BoardX.Value;
            int newY = (int)Num_BoardY.Value;
            ResizeField(newX, newY);
        }

        protected bool Chk_Wrap_Ignore = false;
        protected void Chk_Wrap_CheckedChanged(object sender, EventArgs e)
        {
            if (Chk_Wrap_Ignore)
                return;
            wrap = wrap ? false : true;
        }

        protected bool Txt_TextAlife_Ignore = false;
        protected void Txt_TextAlife_TextChanged(object sender, EventArgs e)
        {
            if (Txt_TextAlife_Ignore)
                return;
            ReformatField();
        }

        protected void Cmd_Random_Click(object sender, EventArgs e)
        {
            Random rand = new Random();
            for (int x = 0; x < cnumX; x++)
                for (int y = 0; y < cnumY; y++)
                    SetAlife(buttons[x,y], rand.Next(0, 2) == 1 ? true : false);
        }

        protected void Cmd_Clear_Click(object sender, EventArgs e)
        {
            foreach (Button button in buttons)
                SetAlife(button, false);
        }

        protected void Cmd_Fill_Click(object sender, EventArgs e)
        {
            foreach (Button button in buttons)
                SetAlife(button, true);
        }

        protected void Cmd_Load_Click(object sender, EventArgs e)
        {

        }

        protected void Cmd_Save_Click(object sender, EventArgs e)
        {

        }

        protected void setAllToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        protected void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }


    }
}
