using System;
using System.Windows.Forms;
using System.Drawing;
 

namespace OpenGL
{
    class cOGL
    {
        Control p;
        int Width;
        int Height;
        GLUquadric obj;

        Random rand = new Random();
        public float[] ScrollValue = new float[17];


        public const int MAX_NUMBER_OF_TREES = 10;
        public static uint numOfTrees = 0;//current number of drawed trees
        uint currentTree = 0;

        uint TREE_LIST,LEAF_LIST, STEM_LIST,STEM_AND_LEAVS_LIST,APPLE_LIST, TREE_MAT, LEAF_MAT, WATER_MAT,APPLE_MAT;//lists definations
        public uint[] Textures = new uint[6];//6 textures for skybox
        uint m_uint_HWND = 0;
        uint m_uint_DC = 0;
        uint m_uint_RC = 0;

        int size = 5; //tree size 
        double amplitude = 1; //tree height
        float widhtScale=1;  //stem width
        float leafScale = 0.25f;//leafs density
        int applesRate = 5;//apples density

        //shadows variables 
        float[] cubeXform = new float[16];
        float[] planeCoeff = { 1, 1, 1, 1 };
        float[,] ground = new float[3, 3];
        public float[] pos = new float[4];//light position

        //flags:
        public bool textureOn = false;
        public bool reflectionOn = false;
        public bool shadowOn = false;
        public bool locationOn = false;
        public bool scullFaceOn = false;

        //rand position
        int[] randX;
        int[] randZ;

        //current location of tree
        public double[] locationZ;
        public double[] locationX;
        public double[] locationRotateY;


        //light properties
        float[] light_ambient = { 0.5f, 0.5f, 0.5f, 1.0f };
        float[] light_diffuse = { 1.0f, 1.0f, 1.0f, 1.0f };
        float[] light_specular = { 1.0f, 1.0f, 1.0f, 1.0f };
        float[] light_position = { 1.0f, 0.5f, 0.0f, 0.0f };
        float[] lmodel_ambient = { 0.4f, 0.4f, 0.4f, 1.0f };

        // Start position of axes.For good look angle
        public float zShift = -10.0f;
        public float yShift = -8.0f;
        public float xShift = 0.0f;

        public float zAngle = 0.0f;
        public float yAngle = -3.0f;
        public float xAngle = 8.0f;
        public int intOptionC = 0;

        /*
         * 
         * Constructor
         * 
         */
        public cOGL(Control pb)
        {
            p=pb;
            Width = p.Width;
            Height = p.Height; 
            InitializeGL();
            obj = GLU.gluNewQuadric();

            //Display lists declaration
            TREE_LIST = GL.glGenLists(MAX_NUMBER_OF_TREES);
            STEM_LIST= GL.glGenLists(MAX_NUMBER_OF_TREES);
            LEAF_LIST= GL.glGenLists(MAX_NUMBER_OF_TREES);
            //APPLE_LIST= GL.glGenLists(MAX_NUMBER_OF_TREES);
            APPLE_LIST = GL.glGenLists(1);
            STEM_AND_LEAVS_LIST = GL.glGenLists(MAX_NUMBER_OF_TREES); 
            TREE_MAT = GL.glGenLists(4);
            LEAF_MAT = TREE_MAT + 1;
            WATER_MAT = TREE_MAT + 3;
            APPLE_MAT = TREE_MAT + 2;


            //3 points of ground plane
            ground[0, 0] = 1;
            ground[0, 1] = 0f;
            ground[0, 2] = 0;

            ground[1, 0] = -1;
            ground[1, 1] = 0f;
            ground[1, 2] = 0;

            ground[2, 0] = 0;
            ground[2, 1] = 0f;
            ground[2, 2] = -1;

            //light position

            pos[0] = light_position[0] = ScrollValue[9];
            pos[1] = light_position[1] = ScrollValue[10];
            pos[2] = light_position[2] = ScrollValue[11];
            pos[3] = light_position[3] = 0;

            //light0 default properties
            GL.glLightfv(GL.GL_LIGHT0, GL.GL_AMBIENT, light_ambient);
            GL.glLightfv(GL.GL_LIGHT0, GL.GL_DIFFUSE, light_diffuse);
            GL.glLightfv(GL.GL_LIGHT0, GL.GL_SPECULAR, light_specular);
            GL.glLightfv(GL.GL_LIGHT0, GL.GL_POSITION, light_position);
            GL.glLightModelfv(GL.GL_LIGHT_MODEL_AMBIENT, lmodel_ambient);

            randX = new int[MAX_NUMBER_OF_TREES];
            randZ = new int[MAX_NUMBER_OF_TREES];

            locationX = new double[MAX_NUMBER_OF_TREES];
            locationZ = new double[MAX_NUMBER_OF_TREES];
            locationRotateY = new double[MAX_NUMBER_OF_TREES];

            randX[0] = 0;
            randZ[0] = 0;

            //rand position for trees,or default value 0 for tree locations.
            for (int i = 0; i < MAX_NUMBER_OF_TREES; i++)
            {
                randX[i] = rand.Next(-90,90);
                randZ[i] = rand.Next(-90,90);
                locationX[i] = 0;
                locationZ[i] = 0;
                locationRotateY[i] = 0;
            }

        }

        /*
         * 
         * Destructor
         * 
         */
        ~cOGL()
        {
            GLU.gluDeleteQuadric(obj);
            WGL.wglDeleteContext(m_uint_RC);
        }

         

        /**********************************************************************************************************
         * 
         * 
         * 
         * MAIN DRAW FUNCTION
         * 
         * 
         * 
         **********************************************************************************************************/
        public void Draw()
        {

            if (m_uint_DC == 0 || m_uint_RC == 0)
                return;

            GL.glClear(GL.GL_COLOR_BUFFER_BIT | GL.GL_DEPTH_BUFFER_BIT | GL.GL_STENCIL_BUFFER_BIT);

            GL.glViewport(0, 0, Width, Height);
            GL.glLoadIdentity();

            GLU.gluLookAt(ScrollValue[0], ScrollValue[1], ScrollValue[2],
                           ScrollValue[3], ScrollValue[4], ScrollValue[5],
                           ScrollValue[6], ScrollValue[7], ScrollValue[8]);

            GL.glRotatef(xAngle, 1.0f, 0.0f, 0.0f);
            GL.glRotatef(yAngle, 0.0f, 1.0f, 0.0f);
            GL.glRotatef(zAngle, 0.0f, 0.0f, 1.0f);
            GL.glTranslatef(xShift, yShift, zShift);

            pos[0] = light_position[0] = ScrollValue[9];
            pos[1] = light_position[1] = ScrollValue[10];
            pos[2] = light_position[2] = ScrollValue[11];
            pos[3] = light_position[3] = 0;

            GL.glLightfv(GL.GL_LIGHT0, GL.GL_AMBIENT, light_ambient);
            GL.glLightfv(GL.GL_LIGHT0, GL.GL_DIFFUSE, light_diffuse);
            GL.glLightfv(GL.GL_LIGHT0, GL.GL_SPECULAR, light_specular);
            GL.glLightfv(GL.GL_LIGHT0, GL.GL_POSITION, light_position);

            GL.glLightModelfv(GL.GL_LIGHT_MODEL_AMBIENT, lmodel_ambient);
            GL.glEnable(GL.GL_LIGHT0);

            /*
             * 
             * Reflection
             * 
             */
            GL.glEnable(GL.GL_BLEND);
            GL.glBlendFunc(GL.GL_SRC_ALPHA, GL.GL_ONE_MINUS_SRC_ALPHA);
            if (reflectionOn)
            {
                //draw only to STENCIL buffer
                GL.glEnable(GL.GL_STENCIL_TEST);
                GL.glStencilOp(GL.GL_REPLACE, GL.GL_REPLACE, GL.GL_REPLACE);
                GL.glStencilFunc(GL.GL_ALWAYS, 1, 0xFFFFFFFF);
                GL.glColorMask((byte)GL.GL_FALSE, (byte)GL.GL_FALSE, (byte)GL.GL_FALSE, (byte)GL.GL_FALSE);
                GL.glDisable(GL.GL_DEPTH_TEST);

                drawLake();

                // restore regular settings
                GL.glColorMask((byte)GL.GL_TRUE, (byte)GL.GL_TRUE, (byte)GL.GL_TRUE, (byte)GL.GL_TRUE);
                GL.glEnable(GL.GL_DEPTH_TEST);

                // reflection is drawn only where STENCIL buffer value equal to 1
                GL.glStencilFunc(GL.GL_EQUAL, 1, 0xFFFFFFFF);
                GL.glStencilOp(GL.GL_KEEP, GL.GL_KEEP, GL.GL_KEEP);
                /*
                 * 
                 * draw reflected scene 
                 * 
                 */

                GL.glEnable(GL.GL_LIGHTING);
                for (currentTree = 0; currentTree < numOfTrees; currentTree++)
                {
                    GL.glPushMatrix();
                    //GL.glTranslated(randX[currentTree], 0,randZ[currentTree]);
                    GL.glTranslated(locationX[currentTree], 0, locationZ[currentTree]);
                    GL.glRotated(locationRotateY[currentTree], 0, 1, 0);
                    GL.glScalef(1, -1, 1); //swap on Z axis

                    if (scullFaceOn)
                    {
                        GL.glEnable(GL.GL_CULL_FACE);
                        GL.glCullFace(GL.GL_BACK);
                        GL.glCallList(TREE_LIST + currentTree);
                        GL.glCullFace(GL.GL_FRONT);
                        GL.glCallList(TREE_LIST + currentTree);
                        GL.glDisable(GL.GL_CULL_FACE);
                    }
                    else
                    GL.glCallList(TREE_LIST + currentTree);

                    GL.glPopMatrix();
                }

                drawLake();

                GL.glStencilFunc(GL.GL_NOTEQUAL, 1, 0xFFFFFFFF);
                GL.glStencilOp(GL.GL_KEEP, GL.GL_KEEP, GL.GL_KEEP);

                // really draw floor 
                //( half-transparent ( see its color's alpha byte)))
                // in order to see reflected objects 
                //GL.glDepthMask((byte)GL.GL_FALSE);
                GL.glDepthMask((byte)GL.GL_TRUE);
                if (!textureOn)
                    drawFloor();
                else
                {
                    drawFloorTextured();
                    DrawTexturedCube();
                }

                GL.glDisable(GL.GL_LIGHTING);
                GL.glDisable(GL.GL_STENCIL_TEST);

            }
            else
            {
                GL.glEnable(GL.GL_LIGHTING);
                drawLake();
                if (!textureOn)
                    drawFloor();
                else
                {
                    drawFloorTextured();
                    DrawTexturedCube();
                }
                GL.glDisable(GL.GL_LIGHTING);
            }

            DrawLight();

            /*
             * Draw trees
             */

            GL.glEnable(GL.GL_LIGHTING);
            GL.glPushMatrix();
            for (currentTree = 0; currentTree < numOfTrees; currentTree++)
            {
                GL.glPushMatrix();
                //GL.glTranslated(randX[currentTree], 0, randZ[currentTree]);
                GL.glTranslated(locationX[currentTree], 0, locationZ[currentTree]);
                GL.glRotated(locationRotateY[currentTree], 0, 1, 0);
                GL.glCallList(TREE_LIST + currentTree);
                GL.glPopMatrix();

            }
            GL.glPopMatrix();

            /*
             * Draw trees shadows
             */
            GL.glDisable(GL.GL_LIGHTING);
            GL.glColor3d(0, 0, 0);
            if (shadowOn)
            {
                GL.glPushMatrix();
                MakeShadowMatrix(ground);
                GL.glMultMatrixf(cubeXform);
                for (currentTree = 0; currentTree < numOfTrees; currentTree++)
                {
                    GL.glPushMatrix();
                    //GL.glTranslated(randX[currentTree], 0, randZ[currentTree]);
                    GL.glTranslated(locationX[currentTree], 0, locationZ[currentTree]);
                    GL.glRotated(locationRotateY[currentTree], 0, 1, 0);
                    GL.glCallList(TREE_LIST + currentTree);
                    GL.glPopMatrix();
                }
                GL.glPopMatrix();
            }

            GL.glFlush();
            WGL.wglSwapBuffers(m_uint_DC);
        }






        /**********************************************************************************************************
         * 
         * 
         * 
         * Draw scene components.Axes,lake,light,floor and textures.
         * 
         * 
         * 
         **********************************************************************************************************/

        void DrawAxes()
        {
            GL.glBegin(GL.GL_LINES);
            //x  RED
            GL.glColor3f(1.0f, 0.0f, 0.0f);
            GL.glVertex3f(-3.0f, 0.0f, 0.0f);
            GL.glVertex3f(3.0f, 0.0f, 0.0f);
            //y  GREEN 
            GL.glColor3f(0.0f, 1.0f, 0.0f);
            GL.glVertex3f(0.0f, -3.0f, 0.0f);
            GL.glVertex3f(0.0f, 3.0f, 0.0f);
            //z  BLUE
            GL.glColor3f(0.0f, 0.0f, 1.0f);
            GL.glVertex3f(0.0f, 0.0f, -3.0f);
            GL.glVertex3f(0.0f, 0.0f, 3.0f);
            GL.glEnd();
        }

        void drawLake()
        {
            GL.glEnable(GL.GL_LIGHTING);
            GL.glCallList(WATER_MAT);
            GL.glPushMatrix();
            if (ScrollValue[16] >= 0)
                GL.glScaled(1, 1, 1 + ScrollValue[16]);
            else
                GL.glScaled(1 + ScrollValue[16], 1, 1);
            GL.glTranslated(ScrollValue[12], 0, ScrollValue[13]);
            GL.glRotated(90, 1, 0, 0);
            GLU.gluDisk(obj, 0, ScrollValue[14], 30, 30);
            GL.glPopMatrix();
            GL.glDisable(GL.GL_LIGHTING);

        }

        void DrawLight()
        {
            GL.glTranslatef(pos[0], pos[1], pos[2]);
            GL.glColor3f(1, 1, 0);
            GLUT.glutSolidSphere(0.8, 10, 10);
            GL.glTranslatef(-pos[0], -pos[1], -pos[2]);
            GL.glEnd();
            GL.glDisable(GL.GL_LIGHTING);
        }

        void drawFloorTextured()
        {
            GL.glEnable(GL.GL_TEXTURE_2D);
            GL.glDisable(GL.GL_BLEND);
            GL.glColor3d(1, 1, 1);
            GL.glDisable(GL.GL_LIGHTING);
            GL.glBindTexture(GL.GL_TEXTURE_2D, Textures[5]);
            GL.glBegin(GL.GL_QUADS);
            GL.glNormal3f(0, 1, 0);
            GL.glTexCoord2f(0.0f, 0.0f); GL.glVertex3f(-200, -0.01f, 200);
            GL.glTexCoord2f(1.0f, 0.0f); GL.glVertex3f(200, -0.01f, 200);
            GL.glTexCoord2f(1.0f, 1.0f); GL.glVertex3f(200, -0.01f, -200);
            GL.glTexCoord2f(0.0f, 1.0f); GL.glVertex3f(-200, -0.01f, -200);
            GL.glEnd();
            GL.glDisable(GL.GL_TEXTURE_2D);
            GL.glEnable(GL.GL_BLEND);
            GL.glEnable(GL.GL_LIGHTING);

        }

        void drawFloor()
        {
            GL.glEnable(GL.GL_LIGHTING);

            GL.glPushMatrix();
            GL.glColor3d(0, 1, 0);
            GL.glTranslated(0, -0.01, 0);

            float[] grass_ambuse = { 0.03f, 0.56f, 0.19f, 1.0f };
            float[] grass_specular = { 0.0f, 0.0f, 0.0f, 1.0f };
            float[] grass_shininess = { 10 };

            GL.glMaterialfv(GL.GL_FRONT_AND_BACK, GL.GL_AMBIENT_AND_DIFFUSE, grass_ambuse);
            GL.glMaterialfv(GL.GL_FRONT_AND_BACK, GL.GL_SPECULAR, grass_specular);
            GL.glMaterialfv(GL.GL_FRONT_AND_BACK, GL.GL_SHININESS, grass_shininess);

            GL.glBegin(GL.GL_QUADS);

            GL.glNormal3f(0, 1, 0);
            GL.glVertex3f(-200, 0, -200);
            GL.glVertex3f(-200, 0, 200);
            GL.glVertex3f(200, 0, 200);
            GL.glVertex3f(200, 0, -200);
            GL.glEnd();

            GL.glDisable(GL.GL_LIGHTING);
            GL.glPopMatrix();
        }

        void DrawTexturedCube()
        {
            GL.glEnable(GL.GL_TEXTURE_2D);
            GL.glDisable(GL.GL_BLEND);
            GL.glColor3d(1, 1, 1);
            GL.glDisable(GL.GL_LIGHTING);
            // front
            GL.glBindTexture(GL.GL_TEXTURE_2D, Textures[0]);
            GL.glBegin(GL.GL_QUADS);
            GL.glTexCoord2f(0.0f, 0.0f); GL.glVertex3f(200.0f, -0.01f, 200.0f);
            GL.glTexCoord2f(1.0f, 0.0f); GL.glVertex3f(-200.0f, -0.01f, 200.0f);
            GL.glTexCoord2f(1.0f, 1.0f); GL.glVertex3f(-200.0f, 200.0f, 200.0f);
            GL.glTexCoord2f(0.0f, 1.0f); GL.glVertex3f(200.0f, 200.0f, 200.0f);
            GL.glEnd();
            // back
            GL.glBindTexture(GL.GL_TEXTURE_2D, Textures[1]);
            GL.glBegin(GL.GL_QUADS);
            GL.glTexCoord2f(0.0f, 0.0f); GL.glVertex3f(-200.0f, -0.01f, -200.0f);
            GL.glTexCoord2f(1.0f, 0.0f); GL.glVertex3f(200.0f, -0.01f, -200.0f);
            GL.glTexCoord2f(1.0f, 1.0f); GL.glVertex3f(200.0f, 200.0f, -200.0f);
            GL.glTexCoord2f(0.0f, 1.0f); GL.glVertex3f(-200.0f, 200.0f, -200.0f);
            GL.glEnd();
            // left
            GL.glBindTexture(GL.GL_TEXTURE_2D, Textures[2]);
            GL.glBegin(GL.GL_QUADS);
            GL.glTexCoord2f(0.0f, 0.0f); GL.glVertex3f(-200.0f, -0.01f, 200.0f);
            GL.glTexCoord2f(1.0f, 0.0f); GL.glVertex3f(-200.0f, -0.01f, -200.0f);
            GL.glTexCoord2f(1.0f, 1.0f); GL.glVertex3f(-200.0f, 200.0f, -200.0f);
            GL.glTexCoord2f(0.0f, 1.0f); GL.glVertex3f(-200.0f, 200.0f, 200.0f);
            GL.glEnd();
            // right
            GL.glBindTexture(GL.GL_TEXTURE_2D, Textures[3]);
            GL.glBegin(GL.GL_QUADS);
            GL.glTexCoord2f(0.0f, 0.0f); GL.glVertex3f(200.0f, -0.01f, -200.0f);
            GL.glTexCoord2f(1.0f, 0.0f); GL.glVertex3f(200.0f, -0.01f, 200.0f);
            GL.glTexCoord2f(1.0f, 1.0f); GL.glVertex3f(200.0f, 200.0f, 200.0f);
            GL.glTexCoord2f(0.0f, 1.0f); GL.glVertex3f(200.0f, 200.0f, -200.0f);
            GL.glEnd();
            // top
            GL.glBindTexture(GL.GL_TEXTURE_2D, Textures[4]);
            GL.glBegin(GL.GL_QUADS);
            GL.glTexCoord2f(0.0f, 0.0f); GL.glVertex3f(-200.0f, 200.0f, -200.0f);
            GL.glTexCoord2f(1.0f, 0.0f); GL.glVertex3f(200.0f, 200.0f, -200.0f);
            GL.glTexCoord2f(1.0f, 1.0f); GL.glVertex3f(200.0f, 200.0f, 200.0f);
            GL.glTexCoord2f(0.0f, 1.0f); GL.glVertex3f(-200.0f, 200.0f, 200.0f);
            GL.glEnd();

            GL.glDisable(GL.GL_TEXTURE_2D);
        }





        /**********************************************************************************************************
         * 
         * 
         * 
         * Functions for create lists for apple,stem,leaf and tree.
         * 
         * 
         * 
         **********************************************************************************************************/

        public void CreateLists(int level,int ampl,float widthRate,float leafRate,int appleRate,bool newTree)
        {
            size = level;
            widhtScale = widthRate;
            leafScale = leafRate;
            applesRate = appleRate;
            amplitude = ampl;
            currentTree = numOfTrees;

            if (currentTree == 0)
            {
                SetupMaterials();
                CreateApple();
            }

            createStemList();
            createLeafList();
            createStemAndLeafs();
            CreateTree();
            ++numOfTrees;
        }
    
        public void createStemList()
        {
            GL.glNewList(STEM_LIST+currentTree, GL.GL_COMPILE);
            GL.glPushMatrix();
            GL.glRotatef(-90, 1, 0, 0);
            GLU.gluCylinder(obj, 0.1*widhtScale, 0.08* widhtScale,1, 10, 10);
            GL.glPopMatrix();
            GL.glEndList();
        }

        public void createLeafList()
        {
            GL.glNewList(LEAF_LIST+currentTree, GL.GL_COMPILE);  
            GL.glBegin(GL.GL_TRIANGLES);

            GL.glNormal3f(-0.1f, 0,leafScale);
            GL.glVertex3f(0, 0, 0);
            GL.glVertex3f(leafScale, leafScale, 0.1f);
            GL.glVertex3f(0, leafScale*2, 0);

            GL.glNormal3f(0.1f, 0, leafScale);
            GL.glVertex3f(0, 0, 0);
            GL.glVertex3f(0, leafScale*2, 0);
            GL.glVertex3f(leafScale, leafScale, 0.1f);

            GL.glEnd();
            GL.glEndList();
        }

        public void createStemAndLeafs()
        {
            GL.glNewList(STEM_AND_LEAVS_LIST+currentTree, GL.GL_COMPILE);
            GL.glPushMatrix();
            GL.glPushAttrib(GL.GL_LIGHTING_BIT);
            GL.glCallList(STEM_LIST+currentTree);
            GL.glCallList(LEAF_MAT);
            for (int i = 0; i < 3; i++)
            {
                GL.glTranslatef(0,(float)1/3, 0);
                GL.glRotatef(90, 0, 1, 0);
                GL.glPushMatrix();
                GL.glRotatef(50, 1, 0, 0);
                GL.glCallList(LEAF_LIST+currentTree);
                GL.glPopMatrix();
                GL.glPushMatrix();
                GL.glRotatef(180, 0, 1, 0);
                GL.glRotatef(60, 1, 0, 0);
                GL.glCallList(LEAF_LIST+currentTree);
                GL.glPopMatrix();
            }
            GL.glPopAttrib();
            GL.glPopMatrix();
            GL.glEndList();
        }

        void CreateApple()
        {
            GL.glColor3d(1, 0, 0);
            GL.glNewList(APPLE_LIST, GL.GL_COMPILE);
            GL.glPushMatrix();
            GL.glRotatef(-90, 1, 0, 0);
            GLUT.glutSolidTorus(0.2, 0.1, 10, 10);
            GL.glPopMatrix();
            GL.glEndList();
        }

        void FractalTreeRec(int level)
        {

            if (level == size)
            {
                GL.glPushMatrix();
                GL.glRotated(rand.NextDouble() * 180, 0, 1, 0);
                GL.glCallList(STEM_AND_LEAVS_LIST+currentTree);
                for (int i = applesRate; i > 0; i--)
                {
                    if (rand.Next(1, 3) == 1)
                    {
                        GL.glPushMatrix();
                        GL.glTranslatef(0, (float)1/i, 0);
                        GL.glCallList(APPLE_MAT);
                        GL.glCallList(APPLE_LIST);
                        GL.glCallList(TREE_MAT);
                        GL.glPopMatrix();
                    }
                }
                GL.glPopMatrix();
            }
            else
            {
                GL.glCallList(STEM_LIST+currentTree);
                GL.glPushMatrix();
                GL.glRotated(rand.NextDouble() * 180, 0, 1, 0);
                GL.glTranslatef(0, 1 + size / 10, 0);
                GL.glScalef(0.7f, 0.7f, 0.7f);

                GL.glPushMatrix();
                GL.glRotated(110 + rand.NextDouble() * 40, 0, 1, 0);
                GL.glRotated(30 + rand.NextDouble() * 20, 0, 0, 1);
                FractalTreeRec(level + 1);
                GL.glPopMatrix();

                GL.glPushMatrix();
                GL.glRotated(-130 + rand.NextDouble() * 40, 0, 1, 0);
                GL.glRotated(30 + rand.NextDouble() * 20, 0, 0, 1);
                FractalTreeRec(level + 1);
                GL.glPopMatrix();

                GL.glPushMatrix();
                GL.glRotated(-20 + rand.NextDouble() * 40, 0, 1, 0);
                GL.glRotated(30 + rand.NextDouble() * 20, 0, 0, 1);
                FractalTreeRec(level + 1);
                GL.glPopMatrix();
                GL.glPopMatrix();
            }
        }
        void CreateTree()
        {
            GL.glNewList(TREE_LIST+currentTree, GL.GL_COMPILE);
            GL.glPushMatrix();
            GL.glPushAttrib(GL.GL_LIGHTING_BIT);
            GL.glCallList(TREE_MAT);
            GL.glScaled(amplitude, amplitude, amplitude);
            FractalTreeRec(0);
            GL.glPopAttrib();
            GL.glPopMatrix();
            GL.glEndList();
        }




        /**********************************************************************************************************
         *  
         * Setup materials parametrs for water,ground and tree components.
         * 
         **********************************************************************************************************/

        public void SetupMaterials()
        {

            float[] tree_ambuse = { 0.4f, 0.25f, 0.1f, 1.0f };
            float[] tree_specular = { 0.0f, 0.0f, 0.0f, 1.0f };
            float[] tree_shininess = { 0 };

            float[] leaf_ambuse = { 0.0f, 0.8f, 0.0f, 1.0f };
            float[] leaf_specular = { 0.0f, 0.8f, 0.0f, 1.0f };
            float[] leaf_shininess = { 10 };

            float[] water_ambuse = { 0.70f, 0.85f, 0.95f,ScrollValue[15] };
            float[] water_specular = { 0.0f, 0.0f, 1.0f, 1.0f };
            float[] water_shininess = { 1 };

            float[] apple_ambuse = { 0.57f, 0.04f, 0.04f, 1.0f };
            float[] apple_specular = { 0.5f, 0.05f, 0.05f, 1.0f };
            float[] apple_shininess = { 0.1f };

            GL.glNewList(APPLE_MAT, GL.GL_COMPILE);
            GL.glMaterialfv(GL.GL_FRONT_AND_BACK, GL.GL_AMBIENT_AND_DIFFUSE, apple_ambuse);
            GL.glMaterialfv(GL.GL_FRONT_AND_BACK, GL.GL_SPECULAR, apple_specular);
            GL.glMaterialfv(GL.GL_FRONT_AND_BACK, GL.GL_SHININESS, apple_shininess);
            GL.glEndList();

            GL.glNewList(WATER_MAT, GL.GL_COMPILE);
            GL.glMaterialfv(GL.GL_FRONT, GL.GL_AMBIENT_AND_DIFFUSE, water_ambuse);
            GL.glMaterialfv(GL.GL_FRONT, GL.GL_SPECULAR, water_specular);
            GL.glMaterialfv(GL.GL_FRONT, GL.GL_SHININESS, water_shininess);
            GL.glEndList();

            GL.glNewList(TREE_MAT, GL.GL_COMPILE);
            GL. glMaterialfv(GL.GL_FRONT, GL.GL_AMBIENT_AND_DIFFUSE, tree_ambuse);
            GL.glMaterialfv(GL.GL_FRONT, GL.GL_SPECULAR, tree_specular);
            GL.glMaterialfv(GL.GL_FRONT, GL.GL_SHININESS, tree_shininess);
            GL.glEndList();

            GL.glNewList(LEAF_MAT, GL.GL_COMPILE);
            GL.glMaterialfv(GL.GL_FRONT_AND_BACK, GL.GL_AMBIENT_AND_DIFFUSE, leaf_ambuse);
            GL.glMaterialfv(GL.GL_FRONT_AND_BACK, GL.GL_SPECULAR, leaf_specular);
            GL.glMaterialfv(GL.GL_FRONT_AND_BACK, GL.GL_SHININESS, leaf_shininess);
            GL.glEndList();
        }


        /**********************************************************************************************************
         * 
         * 
         * 
         * Functions for create shadows parametrs and matrix.
         * 
         * 
         * 
         **********************************************************************************************************/
        void ReduceToUnit(float[] vector)
        {
            float length;

            // Calculate the length of the vector		
            length = (float)Math.Sqrt((vector[0] * vector[0]) +
                                (vector[1] * vector[1]) +
                                (vector[2] * vector[2]));

            // Keep the program from blowing up by providing an exceptable
            // value for vectors that may calculated too close to zero.
            if (length == 0.0f)
                length = 1.0f;

            // Dividing each element by the length will result in a
            // unit normal vector.
            vector[0] /= length;
            vector[1] /= length;
            vector[2] /= length;
        }

        const int x = 0;
        const int y = 1;
        const int z = 2;

        // Points p1, p2, & p3 specified in counter clock-wise order
        void calcNormal(float[,] v, float[] outp)
        {
            float[] v1 = new float[3];
            float[] v2 = new float[3];

            // Calculate two vectors from the three points
            v1[x] = v[0, x] - v[1, x];
            v1[y] = v[0, y] - v[1, y];
            v1[z] = v[0, z] - v[1, z];

            v2[x] = v[1, x] - v[2, x];
            v2[y] = v[1, y] - v[2, y];
            v2[z] = v[1, z] - v[2, z];

            // Take the cross product of the two vectors to get
            // the normal vector which will be stored in out
            outp[x] = Math.Abs(v1[y] * v2[z] - v1[z] * v2[y]);
            outp[y] = Math.Abs(v1[z] * v2[x] - v1[x] * v2[z]);//Abs added..
            outp[z] = Math.Abs(v1[x] * v2[y] - v1[y] * v2[x]);

            // Normalize the vector (shorten length to one)
            ReduceToUnit(outp);
        }

        // Creates a shadow projection matrix out of the plane equation
        // coefficients and the position of the light. The return value is stored
        // in cubeXform[,]
        void MakeShadowMatrix(float[,] points)
        {
            float dot;

            // Find the plane equation coefficients
            // Find the first three coefficients the same way we
            // find a normal.
            calcNormal(points, planeCoeff);

            // Find the last coefficient by back substitutions
            planeCoeff[3] = -(
                (planeCoeff[0] * points[2, 0]) + (planeCoeff[1] * points[2, 1]) +
                (planeCoeff[2] * points[2, 2]));


            // Dot product of plane and light position
            dot = planeCoeff[0] * pos[0] +
                    planeCoeff[1] * pos[1] +
                    planeCoeff[2] * pos[2] +
                    planeCoeff[3];

            // Now do the projection
            // First column
            cubeXform[0] = dot - pos[0] * planeCoeff[0];
            cubeXform[4] = 0.0f - pos[0] * planeCoeff[1];
            cubeXform[8] = 0.0f - pos[0] * planeCoeff[2];
            cubeXform[12] = 0.0f - pos[0] * planeCoeff[3];

            // Second column
            cubeXform[1] = 0.0f - pos[1] * planeCoeff[0];
            cubeXform[5] = dot - pos[1] * planeCoeff[1];
            cubeXform[9] = 0.0f - pos[1] * planeCoeff[2];
            cubeXform[13] = 0.0f - pos[1] * planeCoeff[3];

            // Third Column
            cubeXform[2] = 0.0f - pos[2] * planeCoeff[0];
            cubeXform[6] = 0.0f - pos[2] * planeCoeff[1];
            cubeXform[10] = dot - pos[2] * planeCoeff[2];
            cubeXform[14] = 0.0f - pos[2] * planeCoeff[3];

            // Fourth Column
            cubeXform[3] = 0.0f - pos[3] * planeCoeff[0];
            cubeXform[7] = 0.0f - pos[3] * planeCoeff[1];
            cubeXform[11] = 0.0f - pos[3] * planeCoeff[2];
            cubeXform[15] = dot - pos[3] * planeCoeff[3];
        }



        /**********************************************************************************************************
         * 
         * 
         * 
         * Window and GL configuratins
         * 
         * 
         * 
         **********************************************************************************************************/
        protected virtual void InitializeGL()
		{
			m_uint_HWND = (uint)p.Handle.ToInt32();
			m_uint_DC   = WGL.GetDC(m_uint_HWND);

			WGL.wglSwapBuffers(m_uint_DC);

			WGL.PIXELFORMATDESCRIPTOR pfd = new WGL.PIXELFORMATDESCRIPTOR();
			WGL.ZeroPixelDescriptor(ref pfd);
			pfd.nVersion        = 1; 
			pfd.dwFlags         = (WGL.PFD_DRAW_TO_WINDOW |  WGL.PFD_SUPPORT_OPENGL |  WGL.PFD_DOUBLEBUFFER); 
			pfd.iPixelType      = (byte)(WGL.PFD_TYPE_RGBA);
			pfd.cColorBits      = 32;
			pfd.cDepthBits      = 32;
			pfd.iLayerType      = (byte)(WGL.PFD_MAIN_PLANE);

            pfd.cStencilBits = 32;

            int pixelFormatIndex = 0;
			pixelFormatIndex = WGL.ChoosePixelFormat(m_uint_DC, ref pfd);
			if(pixelFormatIndex == 0)
			{
				MessageBox.Show("Unable to retrieve pixel format");
				return;
			}

			if(WGL.SetPixelFormat(m_uint_DC,pixelFormatIndex,ref pfd) == 0)
			{
				MessageBox.Show("Unable to set pixel format");
				return;
			}
			//Create rendering context
			m_uint_RC = WGL.wglCreateContext(m_uint_DC);
			if(m_uint_RC == 0)
			{
				MessageBox.Show("Unable to get rendering context");
				return;
			}
			if(WGL.wglMakeCurrent(m_uint_DC,m_uint_RC) == 0)
			{
				MessageBox.Show("Unable to make rendering context current");
				return;
			}


            initRenderingGL();
        }
        protected virtual void initRenderingGL()
		{
			if(m_uint_DC == 0 || m_uint_RC == 0)
				return;
			if(this.Width == 0 || this.Height == 0)
				return;
            GL.glClearColor(0.5f, 0.9f, 1.0f, 1.0f);
            GL.glEnable(GL.GL_DEPTH_TEST);
            GL.glDepthFunc(GL.GL_LEQUAL);


            //GL.glHint(GL.GL_PERSPECTIVE_CORRECTION_Hint, GL.GL_NICEST);

            GL.glViewport(0, 0, this.Width, this.Height);
			GL.glMatrixMode ( GL.GL_PROJECTION );
			GL.glLoadIdentity();


            GL.glShadeModel(GL.GL_SMOOTH);

            GLU.gluPerspective(60, (float)Width / (float)Height, 0.45f, 1000.0f);
     
            GenerateTextures(1);//////////////////////////////////////

            GL.glMatrixMode ( GL.GL_MODELVIEW );
			GL.glLoadIdentity();
            
		}
        public void OnResize()
        {
            Width = p.Width;
            Height = p.Height;
            GL.glViewport(0, 0, Width, Height);
            Draw();
        }

        public uint HWND
        {
            get { return m_uint_HWND; }
        }

        public uint DC
        {
            get { return m_uint_DC; }
        }

        public uint RC
        {
            get { return m_uint_RC; }
        }





        /**********************************************************************************************************
         * 
         * 
         * 
         * Textures configuration function.
         * 
         * 
         * 
         **********************************************************************************************************/
        public void GenerateTextures(int texture)
        {
            GL.glBlendFunc(GL.GL_SRC_ALPHA, GL.GL_ONE_MINUS_SRC_ALPHA);
            GL.glGenTextures(6, Textures);

            string[] imagesName ={ "front"+texture+".bmp","back"+texture+".bmp",
                                    "left"+texture+".bmp","right"+texture+".bmp","top"+texture+".bmp","bottom"+texture+".bmp",};
            for (int i = 0; i < 6; i++)
            {
                Bitmap image = new Bitmap(imagesName[i]);
                image.RotateFlip(RotateFlipType.RotateNoneFlipY); //Y axis in Windows is directed downwards, while in OpenGL-upwards
                System.Drawing.Imaging.BitmapData bitmapdata;
                Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);

                bitmapdata = image.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                GL.glBindTexture(GL.GL_TEXTURE_2D, Textures[i]);
                //2D for XYZ
                GL.glTexImage2D(GL.GL_TEXTURE_2D, 0, (int)GL.GL_RGB8, image.Width, image.Height,0, GL.GL_BGR_EXT, GL.GL_UNSIGNED_byte, bitmapdata.Scan0);
                GL.glTexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_MIN_FILTER, (int)GL.GL_LINEAR);
                GL.glTexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_MAG_FILTER, (int)GL.GL_LINEAR);

                image.UnlockBits(bitmapdata);
                image.Dispose();
            }
        }

    }

}


