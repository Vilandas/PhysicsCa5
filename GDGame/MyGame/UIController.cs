﻿using System;
using System.Collections.Generic;
using GDLibrary.Actors;
using GDLibrary.Enums;
using GDLibrary.Managers;
using GDLibrary.Parameters;
using GDLibrary.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.UI.Forms;

namespace GDGame
{
    public class UIController : ControlManager
    {
        private ObjectManager objectManager;
        private KeyboardManager keyboardManager;
        private TextArea infoPanel;
        private Button inputButton;
        private ModelObject box;
        private Physics.Properties p;
        private Physics.Rk4 rk4;
        private bool run;
        private bool step;
        private bool mainHidden;
        private bool useGameSpeed;
        private GameTime gameTime;
        private Camera3D camera3D;
        private Vector3 offset;
        PrimitiveObject archetypalQuad;
        private List<PrimitiveObject> planes;

        private Button[] buttons;
        private Button[] mainButtons;
        private Button[] modifySetters;
        private Button[] modifyButtons;

        public UIController(Game game, ObjectManager objectManager, KeyboardManager keyboardManager,
            ModelObject box, Camera3D camera3D, PrimitiveObject archetypalQuad)
            : base(game)
        {
            this.objectManager = objectManager;
            this.keyboardManager = keyboardManager;
            this.box = box;
            this.p = Physics.ExampleData.example1;
            this.run = false;
            this.step = false;
            this.mainHidden = false;
            this.rk4 = new Physics.Rk4(this.p);
            this.useGameSpeed = false;
            this.camera3D = camera3D;
            this.offset = Vector3.Zero;
            this.archetypalQuad = archetypalQuad;
            this.planes = new List<PrimitiveObject>();
        }

        public override void InitializeComponent()
        {
            Button btn1 = new Button()
            {
                Text = "1: Example 1",
                Size = new Vector2(220, 50),
                BackgroundColor = Color.Black,
                Location = new Vector2(1200, 20),
                ZIndex = 1
            };

            Button btn2 = new Button()
            {
                Text = "2: Example 2",
                Size = new Vector2(220, 50),
                BackgroundColor = Color.Black,
                Location = new Vector2(1200, 90),
                ZIndex = 1
            };

            Button btn3 = new Button()
            {
                Text = "3: Example 3",
                Size = new Vector2(220, 50),
                BackgroundColor = Color.Black,
                Location = new Vector2(1200, 160),
                ZIndex = 1
            };

            Button btn4 = new Button()
            {
                Text = "4: Custom",
                Size = new Vector2(220, 50),
                BackgroundColor = Color.Black,
                Location = new Vector2(1200, 230),
                ZIndex = 1
            };

            Button btnModify = new Button()
            {
                Text = "5: Modify Custom",
                Size = new Vector2(220, 50),
                BackgroundColor = Color.Black,
                Location = new Vector2(1200, 300),
                ZIndex = 1
            };

            Button btnStart = new Button()
            {
                Text = "C: Start",
                Size = new Vector2(220, 50),
                BackgroundColor = Color.Black,
                Location = new Vector2(1200, 370),
                ZIndex = 1
            };

            Button btnStep = new Button()
            {
                Text = "Space: Step",
                Size = new Vector2(220, 50),
                BackgroundColor = Color.Black,
                Location = new Vector2(1200, 440),
                ZIndex = 1
            };

            Button btnGameSpeed = new Button()
            {
                Text = "Use Game Time\nAs Steps: OFF",
                Size = new Vector2(220, 80),
                BackgroundColor = Color.Black,
                Location = new Vector2(1200, 510),
                ZIndex = 1
            };


            infoPanel = new TextArea()
            {
                Text = "\n Gravity: \n Time: \n Step Size: " +
                "\n Mass: \n Dimensions: \n Position: " +
                "\n Velocity: ",
                FontName = "Assets/Fonts/MyFont",
                TextColor = Color.Black,
                Location = new Vector2(20, 560)
            };

            buttons = new Button[] { btn1, btn2, btn3, btn4 };
            mainButtons = new Button[] { btn1, btn2, btn3, btn4, btnModify, btnStart, btnStep, btnGameSpeed };

            foreach (Button button in buttons)
            {
                button.Clicked += ExampleBtn_Clicked;
                button.MouseEnter += Example_MouseEnter;
                button.MouseLeave += Example_MouseLeave;
                Controls.Add(button);
            }

            Controls.Add(btnStart);
            Controls.Add(btnStep);
            Controls.Add(btnModify);
            Controls.Add(btnGameSpeed);
            btnStart.Clicked += BtnStart_Clicked;
            btnStep.Clicked += BtnStep_Clicked;
            btnModify.Clicked += BtnModify_Clicked;
            btnGameSpeed.Clicked += BtnGameSpeed_Clicked;
            Controls.Add(infoPanel);

            InitModifyMenu();

            ExampleBtn_Clicked(btn1, null);
        }

        private void BtnGameSpeed_Clicked(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if(useGameSpeed)
            {
                useGameSpeed = false;
                btn.Text = "Use Game Time\nAs Steps: OFF";
            }
            else
            {
                useGameSpeed = true;
                btn.Text = "Use Game Time\nAs Steps: ON";
            }
        }

        private void InitModifyMenu()
        {
            inputButton = new Button()
            {
                Text = "",
                Size = new Vector2(220, 50),
                BackgroundColor = Color.Black,
                Location = new Vector2(1200, 20),
                IsVisible = false,
                Enabled = false,
                ZIndex = 0
            };

            Button b1 = new Button()
            {
                Text = "Set Gravity",
                Size = new Vector2(220, 50),
                BackgroundColor = Color.Black,
                Location = new Vector2(1200, 90),
                IsVisible = false,
                Enabled = false,
                ZIndex = 0
            };

            Button b2 = new Button()
            {
                Text = "Set Steps",
                Size = new Vector2(220, 50),
                BackgroundColor = Color.Black,
                Location = new Vector2(1200, 230),
                IsVisible = false,
                Enabled = false,
                ZIndex = 0
            };

            Button b3 = new Button()
            {
                Text = "Set Mass",
                Size = new Vector2(220, 50),
                BackgroundColor = Color.Black,
                Location = new Vector2(1200, 300),
                IsVisible = false,
                Enabled = false,
                ZIndex = 0
            };

            Button b4 = new Button()
            {
                Text = "Set MuStatic",
                Size = new Vector2(220, 50),
                BackgroundColor = Color.Black,
                Location = new Vector2(1200, 160),
                IsVisible = false,
                Enabled = false,
                ZIndex = 0
            };

            Button b5 = new Button()
            {
                Text = "Set MuKinetic",
                Size = new Vector2(220, 50),
                BackgroundColor = Color.Black,
                Location = new Vector2(1200, 370),
                IsVisible = false,
                Enabled = false,
                ZIndex = 0
            };

            Button back = new Button()
            {
                Text = "Back",
                Size = new Vector2(220, 50),
                BackgroundColor = Color.Black,
                Location = new Vector2(1200, 440),
                IsVisible = false,
                Enabled = false,
                ZIndex = 0
            };

            #region Position
            Button b6 = new Button()
            {
                Text = "Position: ",
                Size = new Vector2(220, 20),
                BackgroundColor = Color.Black,
                Location = new Vector2(960, 20),
                IsVisible = false,
                Enabled = false,
                ZIndex = 0
            };

            Button b7 = new Button()
            {
                Text = "p.X",
                Size = new Vector2(73, 30),
                BackgroundColor = Color.Black,
                Location = new Vector2(960, 40),
                IsVisible = false,
                Enabled = false,
                ZIndex = 0
            };
            Button b8 = new Button()
            {
                Text = "p.Y",
                Size = new Vector2(73, 30),
                BackgroundColor = Color.Black,
                Location = new Vector2(1033, 40),
                IsVisible = false,
                Enabled = false,
                ZIndex = 0
            };
            Button b9 = new Button()
            {
                Text = "p.Z",
                Size = new Vector2(74, 30),
                BackgroundColor = Color.Black,
                Location = new Vector2(1106, 40),
                IsVisible = false,
                Enabled = false,
                ZIndex = 0
            };
            #endregion

            #region Velocity
            Button b10 = new Button()
            {
                Text = "Velocity: ",
                Size = new Vector2(220, 20),
                BackgroundColor = Color.Black,
                Location = new Vector2(960, 90),
                IsVisible = false,
                Enabled = false,
                ZIndex = 0
            };

            Button b11 = new Button()
            {
                Text = "v.X",
                Size = new Vector2(73, 30),
                BackgroundColor = Color.Black,
                Location = new Vector2(960, 110),
                IsVisible = false,
                Enabled = false,
                ZIndex = 0
            };
            Button b12 = new Button()
            {
                Text = "v.Y",
                Size = new Vector2(73, 30),
                BackgroundColor = Color.Black,
                Location = new Vector2(1033, 110),
                IsVisible = false,
                Enabled = false,
                ZIndex = 0
            };
            Button b13 = new Button()
            {
                Text = "v.Z",
                Size = new Vector2(74, 30),
                BackgroundColor = Color.Black,
                Location = new Vector2(1106, 110),
                IsVisible = false,
                Enabled = false,
                ZIndex = 0
            };
            #endregion

            #region Normal
            Button b14 = new Button()
            {
                Text = "Normal: ",
                Size = new Vector2(220, 20),
                BackgroundColor = Color.Black,
                Location = new Vector2(960, 160),
                IsVisible = false,
                Enabled = false,
                ZIndex = 0
            };

            Button b15 = new Button()
            {
                Text = "n.X",
                Size = new Vector2(73, 30),
                BackgroundColor = Color.Black,
                Location = new Vector2(960, 180),
                IsVisible = false,
                Enabled = false,
                ZIndex = 0
            };
            Button b16 = new Button()
            {
                Text = "n.Y",
                Size = new Vector2(73, 30),
                BackgroundColor = Color.Black,
                Location = new Vector2(1033, 180),
                IsVisible = false,
                Enabled = false,
                ZIndex = 0
            };
            Button b17 = new Button()
            {
                Text = "n.Z",
                Size = new Vector2(74, 30),
                BackgroundColor = Color.Black,
                Location = new Vector2(1106, 180),
                IsVisible = false,
                Enabled = false,
                ZIndex = 0
            };
            #endregion

            modifySetters = new Button[]{ b1, b2, b3, b4, b5, b6, b7, b8, b9, b10,
                b11, b12, b13, b14, b15, b16, b17 };

            modifyButtons = new Button[]{ b1, b2, b3, b4, b5, b6, b7, b8, b9, b10,
                b11, b12, b13, b14, b15, b16, b17,
                inputButton, back };

            foreach (Button button in modifySetters)
            {
                button.Clicked += ModifyButton_Clicked;
                Controls.Add(button);
            }

            back.Clicked += Back_Clicked;
            Controls.Add(back);
            Controls.Add(inputButton);
        }

        private void ModifyButton_Clicked(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (inputButton.Text.Length > 0)
            {
                switch (btn.Text)
                {
                    case "Set Gravity":
                        Physics.ExampleData.custom.Gravity = Convert.ToDouble(inputButton.Text);
                        break;
                    case "Set MuStatic":
                        Physics.ExampleData.custom.Planes[0].MuStatic = Convert.ToDouble(inputButton.Text);
                        break;
                    case "Set MuKinetic":
                        Physics.ExampleData.custom.Planes[0].MuKinetic = Convert.ToDouble(inputButton.Text);
                        break;
                    case "Set Steps":
                        Physics.ExampleData.custom.Steps = Physics.ExampleData.custom.OriginalSteps
                            = Convert.ToDouble(inputButton.Text);
                        break;
                    case "Set Mass":
                        Physics.ExampleData.custom.Mass = Convert.ToDouble(inputButton.Text);
                        break;
                    case "d.X":
                        Physics.ExampleData.custom.Dimensions.X = Convert.ToDouble(inputButton.Text);
                        break;
                    case "d.Y":
                        Physics.ExampleData.custom.Dimensions.Y = Convert.ToDouble(inputButton.Text);
                        break;
                    case "d.Z":
                        Physics.ExampleData.custom.Dimensions.Z = Convert.ToDouble(inputButton.Text);
                        break;
                    case "p.X":
                        Physics.ExampleData.custom.Position.X = Convert.ToDouble(inputButton.Text);
                        break;
                    case "p.Y":
                        Physics.ExampleData.custom.Position.Y = Convert.ToDouble(inputButton.Text);
                        break;
                    case "p.Z":
                        Physics.ExampleData.custom.Position.Z = Convert.ToDouble(inputButton.Text);
                        break;
                    case "v.X":
                        Physics.ExampleData.custom.Velocity.X = Convert.ToDouble(inputButton.Text);
                        break;
                    case "v.Y":
                        Physics.ExampleData.custom.Velocity.Y = Convert.ToDouble(inputButton.Text);
                        break;
                    case "v.Z":
                        Physics.ExampleData.custom.Velocity.Z = Convert.ToDouble(inputButton.Text);
                        break;
                    case "n.X":
                        Physics.ExampleData.custom.Planes[0].Normal.X = Convert.ToDouble(inputButton.Text);
                        break;
                    case "n.Y":
                        Physics.ExampleData.custom.Planes[0].Normal.Y = Convert.ToDouble(inputButton.Text);
                        break;
                    case "n.Z":
                        Physics.ExampleData.custom.Planes[0].Normal.Z = Convert.ToDouble(inputButton.Text);
                        break;
                }
                ExampleBtn_Clicked(buttons[3], null);
            }
        }

        private void Back_Clicked(object sender, EventArgs e)
        {
            ShowMainButtons();
        }

        private void BtnModify_Clicked(object sender, EventArgs e)
        {
            HideMainButtons();
        }

        private void BtnStep_Clicked(object sender, EventArgs e)
        {
            step = true;
        }

        private void BtnStart_Clicked(object sender, EventArgs e)
        {
            run = true;
            if (useGameSpeed)
            {
                p.Steps = gameTime.ElapsedGameTime.TotalSeconds;
            }
            else
                p.Steps = p.OriginalSteps;
        }

        private void Example_MouseEnter(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            Physics.Properties p = null;
            switch (btn.Text)
            {
                case "1: Example 1":
                    p = Physics.ExampleData.example1;
                    break;
                case "2: Example 2":
                    p = Physics.ExampleData.example2;
                    break;
                case "3: Example 3":
                    p = Physics.ExampleData.example3;
                    break;
                case "4: Custom":
                    p = Physics.ExampleData.custom;
                    break;
            }
            infoPanel.Text = "\n Gravity: " + p.Gravity + "\n Time: " + p.Time + " \n Step Size: " + p.Steps +
                "\n Mass: " + p.Mass + " \n Dimensions: " + p.Dimensions + " \n Position: " + p.OriginalPosition +
                "\n Velocity: " + p.OriginalVelocity + "\n MuStatic: " + p.Planes[0].MuStatic + "\n MuKinetic: " + p.Planes[0].MuKinetic +
                "\n Normal: " + p.Planes[0].Normal;
        }

        private void Example_MouseLeave(object sender, EventArgs e)
        {
            infoPanel.Text = "\n Gravity: " + p.Gravity + "\n Time: " + p.Time + " \n Step Size: " + p.Steps +
                "\n Mass: " + p.Mass + " \n Dimensions: " + p.Dimensions + " \n Position: " + p.Position +
                "\n Velocity: " + p.Velocity + "\n MuStatic: " + p.Planes[0].MuStatic + "\n MuKinetic: " + p.Planes[0].MuKinetic +
                "\n Normal: " + p.Planes[0].Normal;
        }

        private void ExampleBtn_Clicked(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            switch (btn.Text)
            {
                case "1: Example 1":
                    p = Physics.ExampleData.example1;
                    break;
                case "2: Example 2":
                    p = Physics.ExampleData.example2;
                    break;
                case "3: Example 3":
                    p = Physics.ExampleData.example3;
                    break;
                case "4: Custom":
                    p = Physics.ExampleData.custom;
                    break;
            }

            Example_MouseEnter(btn, null);

            p.Position = p.OriginalPosition;
            p.Velocity = p.OriginalVelocity;
            p.Steps = p.OriginalSteps;
            p.Time = p.OriginalTime;

            rk4 = new Physics.Rk4(p);
            box.Transform3D.Translation = ToVector3(p.Position);
            box.Transform3D.Scale = ToVector3(p.Dimensions);

            foreach(PrimitiveObject plane in planes)
            {
                objectManager.RemoveByID(plane.ID);
            }
            planes.Clear();

            InitPlanes();
        }

        private void HideMainButtons()
        {
            mainHidden = true;
            foreach(Button button in mainButtons)
            {
                button.IsVisible = false;
                button.Enabled = false;
                button.ZIndex = 0;
            }
            foreach (Button button in modifyButtons)
            {
                button.IsVisible = true;
                button.Enabled = true;
                button.ZIndex = 1;
            }
            ExampleBtn_Clicked(buttons[3], null);
        }

        private void ShowMainButtons()
        {
            mainHidden = false;
            foreach (Button button in modifyButtons)
            {
                button.IsVisible = false;
                button.Enabled = false;
                button.ZIndex = 0;
            }
            foreach (Button button in mainButtons)
            {
                button.IsVisible = true;
                button.Enabled = true;
                button.ZIndex = 1;
            }
        }

        public override void Update(GameTime gameTime)
        {
            this.gameTime = gameTime;
            if (run || step)
            {
                Run();
            }
            HandleKeyboardInput(gameTime);
            HandleCameraFollow();

            CheckKeysSet();

            base.Update(gameTime);
        }

        private void InitPlanes()
        {
            for (int i = 0; i < p.Planes.Count; i++)
            {
                Physics.Plane plane = p.Planes[i];
                PrimitiveObject planeObject = archetypalQuad.Clone() as PrimitiveObject;
                planeObject.ID = "plane";
                planeObject.EffectParameters.Texture = null;
                planeObject.StatusType = StatusType.Drawn;
                planeObject.ActorType = ActorType.Decorator;

                Transform3D transform3D;
                Vector3 yawPitchRoll = AnglesFromVector(ToVector3(plane.Normal));

                if (i == 0)
                {
                    transform3D = new Transform3D(
                    box.Transform3D.Translation - ToVector3(p.Dimensions / 2.6),
                    -Vector3.UnitZ,
                    Vector3.UnitY);

                    planeObject.EffectParameters.DiffuseColor = Color.Red;

                    transform3D.RotateBy(yawPitchRoll);
                    box.Transform3D.RotateBy(yawPitchRoll);
                }
                else
                {
                    PrimitiveObject lastP = planes[i - 1];

                    transform3D = new Transform3D(
                        new Vector3(
                            lastP.Transform3D.Translation.X + (float)plane.Offset.X,
                            lastP.Transform3D.Translation.Y + (float)plane.Offset.Y,
                            lastP.Transform3D.Translation.Z + (float)plane.Offset.Z),
                        -Vector3.UnitZ,
                        Vector3.UnitY);

                    transform3D.RotateBy(yawPitchRoll);
                }
                transform3D.Scale = ToVector3(plane.Dimensions);

                planeObject.Transform3D = transform3D;

                planes.Add(planeObject);
                objectManager.Add(planeObject);
            }
        }

        private void Run()
        {
            Physics.Vector3 acceleration = rk4.CalculateAcceleration(p.Velocity);
            Physics.Vector2 pv = rk4.CalculateRk4();
            rk4.UpdatePV(pv);
            p.Position = pv.X;
            p.Velocity = pv.Y;
            p.Time += p.Steps;

            if (p.Position.Z - p.Dimensions.Z/2 <= 0)
            {
                p.Position.Z = p.Dimensions.Z/2;
                p.Velocity.Z = 0;
                rk4.Data.ExportData();
                run = false;
            }
            box.Transform3D.Translation = ToVector3(p.Position);

            infoPanel.Text = "\n Gravity: " + p.Gravity + "\n Time: " + p.Time + " \n Step Size: " + p.Steps +
            "\n Mass: " + p.Mass + " \n Dimensions: " + p.Dimensions + " \n Position: " + p.Position +
            "\n Velocity: " + p.Velocity + "\n MuStatic: " + p.Planes[0].MuStatic + "\n MuKinetic: " + p.Planes[0].MuKinetic +
            "\n Normal: " + p.Planes[0].Normal + "\n Acceleration: " + acceleration;

            if (p.Velocity.IsZero())
            {
                infoPanel.Text += "\n Static";
                rk4.Data.ExportData();
                run = false;
            }
            else infoPanel.Text += "\n Kinetic";

            step = false;
        }

        private void HandleCameraFollow()
        {
            //Offest the objects position to where the camera should be
            Vector3 parentPos = box.Transform3D.Translation;
            parentPos.X += box.Transform3D.Scale.X;
            parentPos.Y += box.Transform3D.Scale.Y;
            parentPos.Z += box.Transform3D.Scale.Z;

            //subtract objects position from camera position to get the distance
            parentPos -= camera3D.Transform3D.Translation;

            camera3D.Transform3D.Translation += parentPos + offset;

            Vector3 cameraToTarget = MathUtility.GetNormalizedObjectToTargetVector(camera3D.Transform3D, box.Transform3D);

            //round to prevent floating-point precision errors across updates
            cameraToTarget = MathUtility.Round(cameraToTarget, 3);

            camera3D.Transform3D.Look = cameraToTarget;
        }

        private void HandleKeyboardInput(GameTime gameTime)
        {
            Vector3 moveVector = Vector3.Zero;

            if (keyboardManager.IsKeyDown(Keys.W))
            {
                moveVector -= camera3D.Transform3D.Look;
            }
            else if (keyboardManager.IsKeyDown(Keys.S))
            {
                moveVector += camera3D.Transform3D.Look;
            }

            if (keyboardManager.IsKeyDown(Keys.A))
            {
                moveVector += camera3D.Transform3D.Right;
            }
            else if (keyboardManager.IsKeyDown(Keys.D))
            {
                moveVector -= camera3D.Transform3D.Right;
            }

            moveVector.Y = 0;

            //apply the movement
            offset += 0.2f * moveVector * (float)Math.Cos(gameTime.ElapsedGameTime.Milliseconds);
        }

        private void CheckKeysSet()
        {
            if(!mainHidden)
            {
                if (keyboardManager.IsFirstKeyPress(Keys.C))
                    BtnStart_Clicked(null, null);
                else if (keyboardManager.IsFirstKeyPress(Keys.Space))
                    BtnStep_Clicked(null, null);
                else if (keyboardManager.IsFirstKeyPress(Keys.D1))
                    ExampleBtn_Clicked(buttons[0], null);
                else if (keyboardManager.IsFirstKeyPress(Keys.D2))
                    ExampleBtn_Clicked(buttons[1], null);
                else if (keyboardManager.IsFirstKeyPress(Keys.D3))
                    ExampleBtn_Clicked(buttons[2], null);
                else if (keyboardManager.IsFirstKeyPress(Keys.D4))
                    ExampleBtn_Clicked(buttons[3], null);
                else if (keyboardManager.IsFirstKeyPress(Keys.D5))
                    BtnModify_Clicked(null, null);
            }
            else
            {
                if (keyboardManager.IsFirstKeyPress(Keys.D1))
                    inputButton.Text += "1";
                else if (keyboardManager.IsFirstKeyPress(Keys.D2))
                    inputButton.Text += "2";
                else if (keyboardManager.IsFirstKeyPress(Keys.D3))
                    inputButton.Text += "3";
                else if (keyboardManager.IsFirstKeyPress(Keys.D4))
                    inputButton.Text += "4";
                else if (keyboardManager.IsFirstKeyPress(Keys.D5))
                    inputButton.Text += "5";
                else if (keyboardManager.IsFirstKeyPress(Keys.D6))
                    inputButton.Text += "6";
                else if (keyboardManager.IsFirstKeyPress(Keys.D7))
                    inputButton.Text += "7";
                else if (keyboardManager.IsFirstKeyPress(Keys.D8))
                    inputButton.Text += "8";
                else if (keyboardManager.IsFirstKeyPress(Keys.D9))
                    inputButton.Text += "9";
                else if (keyboardManager.IsFirstKeyPress(Keys.D0))
                    inputButton.Text += "0";
                else if (keyboardManager.IsFirstKeyPress(Keys.OemPeriod))
                    inputButton.Text += ".";
                else if (keyboardManager.IsFirstKeyPress(Keys.Back))
                {
                    if(inputButton.Text.Length > 0)
                        inputButton.Text = inputButton.Text.Remove(inputButton.Text.Length - 1);
                }
            }
        }
        public Vector3 ToVector3(Physics.Vector3 pVec)
        {
            return new Vector3(
                (float)pVec.X,
                (float)pVec.Z,
                (float)pVec.Y);
        }

        //https://www.codeproject.com/Questions/324240/Determining-yaw-pitch-and-roll
        public Vector3 AnglesFromVector(Vector3 v)
        {
            Matrix matrix = Matrix.CreateLookAt(Vector3.Zero, v, Vector3.Up);
            float yaw = (float)Math.Atan2(matrix.M13, matrix.M33);
            float pitch = (float)Math.Asin(-matrix.M23);
            float roll = (float)Math.Atan2(matrix.M21, matrix.M22);

            return new Vector3(
                MathHelper.ToDegrees(pitch),
                MathHelper.ToDegrees(yaw),
                MathHelper.ToDegrees(roll));
        }
    }
}
