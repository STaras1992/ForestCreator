using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenGL;
using System.Runtime.InteropServices; 

namespace myOpenGL
{
    public partial class Form1 : Form
    {
         cOGL cGL;

        public Form1()
        {

            InitializeComponent();
            cGL = new cOGL(panel1);
            //apply the bars values as cGL.ScrollValue[..] properties 
            //!!!
            hScrollBarScroll(hScrollBar1, null);
            hScrollBarScroll(hScrollBar2, null);
            hScrollBarScroll(hScrollBar3, null);
            hScrollBarScroll(hScrollBar4, null);
            hScrollBarScroll(hScrollBar5, null);
            hScrollBarScroll(hScrollBar6, null);
            hScrollBarScroll(hScrollBar7, null);
            hScrollBarScroll(hScrollBar8, null);
            hScrollBarScroll(hScrollBar9, null);
            hScrollBarScroll(hScrollBar10, null);
            hScrollBarScroll(hScrollBar11, null);
            hScrollBarScroll(hScrollBar12, null);
        }


        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            cGL.Draw();
        }

        private void panel1_Resize(object sender, EventArgs e)
        {          
            //cGL.OnResize();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void hScrollBarScroll(object sender, ScrollEventArgs e)
        {
            cGL.intOptionC = 0;
            HScrollBar hb = (HScrollBar)sender;
            int n;
            if (hb.Name.Length == 11)//look-at scrolls
            {
                n = int.Parse(hb.Name.Substring(hb.Name.Length - 1));
                cGL.ScrollValue[n - 1] = (hb.Value - 100) / 10.0f;
            }
            else
            if (hb.Name.Length == 12)//light position scrools
            {
                n = int.Parse(hb.Name.Substring(hb.Name.Length - 2));             
                cGL.ScrollValue[n-1] = (float)(hb.Value);
            }
            else
            if (hb.Name.Length == 13)//lake position scroll
            {
                n = int.Parse(hb.Name.Substring(hb.Name.Length - 2));
                cGL.ScrollValue[n - 1] =(float) (hb.Value) / 10;
                cGL.SetupMaterials();
            }

            if (e != null)
            {
                cGL.Draw();
            }
        }

        public float[] oldPos = new float[7];

        private void numericUpDownValueChanged(object sender, EventArgs e)
        {
            NumericUpDown nUD = (NumericUpDown)sender;
            int i = int.Parse(nUD.Name.Substring(nUD.Name.Length - 1));
            int pos = (int)nUD.Value; 
            switch(i)
            {
                case 1:
                    if (pos < oldPos[i - 1])
                    {
                        cGL.xShift += 1f;
                        cGL.intOptionC = 4;
                    }
                    else
                    {
                        cGL.xShift -= 1f;
                        cGL.intOptionC = -4;
                    }
                    break;
                case 2:
                    if (pos < oldPos[i - 1])
                    {
                        cGL.yShift += 1f;
                        cGL.intOptionC = 5;
                    }
                    else
                    {
                        cGL.yShift -= 1f;
                        cGL.intOptionC = -5;
                    }
                    break;
                case 3:
                    if (pos > oldPos[i - 1])
                    {
                        cGL.zShift += 1f;
                        cGL.intOptionC = 6;
                    }
                    else
                    {
                        cGL.zShift -= 1f;
                        cGL.intOptionC = -6;
                    }
                    break;
                case 4:
                    if (pos > oldPos[i - 1])
                    {
                        cGL.xAngle += 10;
                        cGL.intOptionC = 1;
                    }
                    else
                    {
                        cGL.xAngle -= 10;
                        cGL.intOptionC = -1;
                    }
                    break;
                case 5:
                    if (pos > oldPos[i - 1])
                    {
                        cGL.yAngle += 10;
                        cGL.intOptionC = 2;
                    }
                    else
                    {
                        cGL.yAngle -= 10;
                        cGL.intOptionC = -2;
                    }
                    break;
                case 6: 
	                if (pos>oldPos[i-1]) 
	                {
		                cGL.zAngle+=10;
		                cGL.intOptionC=3;
	                }
	                else
	                {
                        cGL.zAngle -= 10;
                        cGL.intOptionC = -3;
                    }
                    break;
            }
            cGL.Draw();
            oldPos[i - 1] = pos;
        }

        /*
         * Draw tree..
         */
        private void button1_Click(object sender, EventArgs e)
        {
            locationB.Enabled = true;
            //if (cOGL.numOfTrees < cOGL.MAX_NUMBER_OF_TREES)
            //{
            //    cGL.CreateLists(scrollSize.Value,scrollAmplitude.Value, (float)scrollWidth.Value / 100, (float)scrollDensity.Value / 100, scrollApples.Value, true);
            //}
            //else
            //    MessageBox.Show("Max number of trees");

            setB.Enabled = false;
            upB.Enabled = false;
            leftB.Enabled = false;
            rightB.Enabled =false;
            downB.Enabled = false;
            leftRotateB.Enabled = false;
            rightRotateB.Enabled = false;

            cGL.locationOn = false;
            //cGL.locationZ[cOGL.numOfTrees] = cGL.locationX[cOGL.numOfTrees] = 0;

            cGL.Draw();
        }

        private void checkBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            int i= int.Parse(cb.Name.Substring(cb.Name.Length - 1));
            switch (i)
            {
                case 1:
                    if (cb.Checked == true)
                    {
                        cGL.GenerateTextures(1);
                        checkBox2.Checked = false;
                        checkBox3.Checked = false;
                        cGL.textureOn = true;
                    }
                    else
                        cGL.textureOn = false;
                    break;
                case 2:
                    if (cb.Checked == true)
                    {
                        cGL.GenerateTextures(2);
                        checkBox1.Checked = false;
                        checkBox3.Checked = false;
                        cGL.textureOn = true;
                    }
                    else
                        cGL.textureOn = false;
                    break;
                case 3:
                    if (cb.Checked == true)
                    {
                        cGL.GenerateTextures(3);
                        checkBox1.Checked = false;
                        checkBox2.Checked = false;
                        cGL.textureOn = true;
                    }
                    else
                        cGL.textureOn = false;
                    break;
                case 4:
                    if (cb.Checked == true)
                        cGL.shadowOn = true;
                    else
                        cGL.shadowOn = false;
                    break;
                case 5:
                    if (cb.Checked == true)
                        cGL.reflectionOn = true;
                    else
                        cGL.reflectionOn = false;
                    break;
            }
            cGL.Draw();            
        }

        private void button5_Click(object sender, EventArgs e)
        {
            cGL.locationOn = true;

            locationB.Enabled = false;
            setB.Enabled = true;

            upB.Enabled = true;
            leftB.Enabled = true;
            rightB.Enabled = true;
            downB.Enabled = true;
            leftRotateB.Enabled = true;
            rightRotateB.Enabled = true;


            if (cOGL.numOfTrees < cOGL.MAX_NUMBER_OF_TREES)
            {
                cGL.CreateLists(scrollSize.Value, scrollAmplitude.Value, (float)scrollWidth.Value / 100, (float)scrollDensity.Value / 100, scrollApples.Value, true);
            }
            else
                MessageBox.Show("Max number of trees");

            //cGL.locationZ[cOGL.numOfTrees-1] = cGL.locationX[cOGL.numOfTrees-1] = 0;
            cGL.Draw();
        }



        private void upB_Click(object sender, EventArgs e)
        {
            cGL.locationZ[cOGL.numOfTrees - 1] -= 1;
            cGL.Draw();
        }

        private void rightB_Click(object sender, EventArgs e)
        {
            cGL.locationX[cOGL.numOfTrees - 1] += 1;
            cGL.Draw();
        }

        private void leftB_Click(object sender, EventArgs e)
        {
            cGL.locationX[cOGL.numOfTrees - 1] -= 1;
            cGL.Draw();
        }

        private void downB_Click(object sender, EventArgs e)
        {
            cGL.locationZ[cOGL.numOfTrees - 1] += 1;
            cGL.Draw();
        }

        private void rightRotateB_Click(object sender, EventArgs e)
        {
            cGL.locationRotateY[cOGL.numOfTrees - 1] += 15;
            cGL.Draw();
        }

        private void leftRotateB_Click(object sender, EventArgs e)
        {
            cGL.locationRotateY[cOGL.numOfTrees - 1]-= 15;
            cGL.Draw();
        }

        private void topViewB_Click(object sender, EventArgs e)
        {
            cGL.xShift = -30;
            oldPos[0] =-30;
            cGL.yShift = -60;
            oldPos[1] = -60;
            cGL.zShift = -70;
            oldPos[2] = -70;

            cGL.xAngle = 30;
            oldPos[3] = 30;
            cGL.yAngle = -15;
            oldPos[4] = -15;
            
            cGL.Draw();
            

        }
    }
}