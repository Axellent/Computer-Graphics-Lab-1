using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GameEngine;

namespace NAJ_Lab1
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class ChopperGame : Game
    {
        public struct VertexPositionColorNormal
        {
            public Vector3 Position;
            public Color Color;
            public Vector3 Normal;

            public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
            (
                new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                new VertexElement(sizeof(float) * 3, VertexElementFormat.Color, VertexElementUsage.Color, 0),
                new VertexElement(sizeof(float) * 3 + 4, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0)
            );
        }

        GraphicsDeviceManager graphicsDeviceManager;
        GraphicsDevice graphicsDevice;
        SpriteBatch spriteBatch;
        VertexBuffer myVertexBuffer;
        IndexBuffer myIndexBuffer;

        BasicEffect effect;
        VertexPositionColorNormal[] vertices;
        Matrix viewMatrix;
        Matrix projectionMatrix;

        private Matrix chopperWorld = Matrix.CreateTranslation(new Vector3(0, 0, 10));
        private Matrix chopperView = Matrix.CreateLookAt(new Vector3(0, 0, 10), new Vector3(0, 0, 0), Vector3.UnitY);
        private Matrix chopperProjection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 1280f / 720f, 0.1f, 100f);
        private Matrix chopoperScale = Matrix.CreateScale(0.3f);

        private float angle = 0f;
        private int terrainWidth = 4;
        private int terrainHeight = 3;
        int[] indices;
        private float[,] heightData;
        private float indicesLenDiv3 = 1;

        private Vector3 chopperPos;
        private float chopperAngle;
        Model chopperModel;

        public ChopperGame() : base()
        {
            graphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        public void StartGame()
        {
            Run();
        }

        public void ExitGame()
        {
            Exit();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            graphicsDeviceManager.PreferredBackBufferWidth = 1280;
            graphicsDeviceManager.PreferredBackBufferHeight = 720;
            graphicsDeviceManager.IsFullScreen = false;
            graphicsDeviceManager.ApplyChanges();

            Window.Title = "Lab 1 - Datorgrafik!";

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            graphicsDevice = GraphicsDevice;
            spriteBatch = new SpriteBatch(graphicsDevice);
            SetUpCamera();

            effect = new BasicEffect(graphicsDevice);
            effect.EnableDefaultLighting();

            Texture2D heightMap = Content.Load<Texture2D>("canyon");
            LoadHeightData(heightMap);

            chopperModel = Content.Load<Model>("Chopper");
            chopperPos = new Vector3(0, 0, 0);
            chopperAngle = 0;

            SetUpVertices();
            SetUpIndices();
            CalculateNormals();
            CopyToBuffers();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            chopperPos += new Vector3(0, 0.000f, 0);

            ChangeBoneTransform(chopperModel, 1, Matrix.CreateRotationY(0.5f));
            ChangeBoneTransform(chopperModel, 3, Matrix.CreateRotationY(0.5f));

            KeyboardState keyState = Keyboard.GetState();

            if (keyState.IsKeyDown(Keys.Left))
            {
                angle += 0.05f;
            }
            if (keyState.IsKeyDown(Keys.Right))
            {
                angle -= 0.05f;
            }

            SystemManager.Instance.RunAllUpdateSystems(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphicsDevice.Clear(Color.CornflowerBlue);

            RasterizerState rs = new RasterizerState();
            rs.CullMode = CullMode.None;
            graphicsDevice.RasterizerState = rs;

            Matrix worldMatrix = Matrix.CreateTranslation(-terrainWidth / 2.0f, 0, terrainHeight / 2.0f) * Matrix.CreateRotationY(angle);
            effect.View = viewMatrix;
            effect.Projection = projectionMatrix;
            effect.World = worldMatrix;
            effect.VertexColorEnabled = true;
            Vector3 lightDirection = new Vector3(1.0f, -1.0f, -1.0f);
            lightDirection.Normalize();


            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                graphicsDevice.Indices = myIndexBuffer;
                graphicsDevice.SetVertexBuffer(myVertexBuffer);
                graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, (int)indicesLenDiv3);
            }

            DrawModel(chopperModel, chopperWorld, chopperView, chopperProjection);

            SystemManager.Instance.RunAllRenderSystems(spriteBatch, gameTime);

            base.Draw(gameTime);
        }

        private void DrawModel(Model model, Matrix world, Matrix view, Matrix projection)
        {

            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();

                    effect.World = transforms[mesh.ParentBone.Index] *
                                   Matrix.CreateRotationY(chopperAngle)
                                   * Matrix.CreateTranslation(chopperPos)
                                   * chopoperScale;
                    effect.View = view;
                    effect.Projection = projection;
                }

                mesh.Draw();
            }
        }

        private void ChangeBoneTransform(Model model, int boneIndex, Matrix t)
        {
            model.Bones[boneIndex].Transform = t * model.Bones[boneIndex].Transform;
        }

        private void LoadHeightData(Texture2D heightMap)
        {
            terrainWidth = heightMap.Width;
            terrainHeight = heightMap.Height;

            Color[] heightMapColors = new Color[terrainWidth * terrainHeight];
            heightMap.GetData(heightMapColors);

            heightData = new float[terrainWidth, terrainHeight];
            for (int x = 0; x < terrainWidth; x++)
                for (int y = 0; y < terrainHeight; y++)
                    heightData[x, y] = heightMapColors[x + y * terrainWidth].R / 4f;
        }

        private void SetUpCamera()
        {
            viewMatrix = Matrix.CreateLookAt(new Vector3(60, 80, -180), new Vector3(0, 0, 0), new Vector3(0, 1, 0));
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, graphicsDevice.Viewport.AspectRatio, 1.0f, 500.0f);
        }

        private void SetUpVertices()
        {
            float minHeight = float.MaxValue;
            float maxHeight = float.MinValue;
            for (int x = 0; x < terrainWidth; x++)
            {
                for (int y = 0; y < terrainHeight; y++)
                {
                    if (heightData[x, y] < minHeight)
                        minHeight = heightData[x, y];
                    if (heightData[x, y] > maxHeight)
                        maxHeight = heightData[x, y];
                }
            }

            vertices = new VertexPositionColorNormal[terrainWidth * terrainHeight];
            for (int x = 0; x < terrainWidth; x++)
            {
                for (int y = 0; y < terrainHeight; y++)
                {
                    vertices[x + y * terrainWidth].Position = new Vector3(x, heightData[x, y], -y);

                    if (heightData[x, y] < minHeight + (maxHeight - minHeight) / 4)
                        vertices[x + y * terrainWidth].Color = Color.SandyBrown;
                    else if (heightData[x, y] < minHeight + (maxHeight - minHeight) * 2 / 4)
                        vertices[x + y * terrainWidth].Color = Color.BurlyWood;
                    else if (heightData[x, y] < minHeight + (maxHeight - minHeight) * 3 / 4)
                        vertices[x + y * terrainWidth].Color = Color.SandyBrown;
                    else
                        vertices[x + y * terrainWidth].Color = Color.Chocolate;
                }
            }
        }

        private void SetUpIndices()
        {
            indices = new int[(terrainWidth - 1) * (terrainHeight - 1) * 6];
            int counter = 0;
            for (int y = 0; y < terrainHeight - 1; y++)
            {
                for (int x = 0; x < terrainWidth - 1; x++)
                {
                    int lowerLeft = x + y * terrainWidth;
                    int lowerRight = (x + 1) + y * terrainWidth;
                    int topLeft = x + (y + 1) * terrainWidth;
                    int topRight = (x + 1) + (y + 1) * terrainWidth;

                    indices[counter++] = topLeft;
                    indices[counter++] = lowerRight;
                    indices[counter++] = lowerLeft;

                    indices[counter++] = topLeft;
                    indices[counter++] = topRight;
                    indices[counter++] = lowerRight;
                }
            }

            indicesLenDiv3 = indices.Length / 3;
        }

        private void CalculateNormals()
        {
            for (int i = 0; i < vertices.Length; i++)
                vertices[i].Normal = new Vector3(0, 0, 0);

            for (int i = 0; i < indices.Length / 3; i++)
            {
                int index1 = indices[i * 3];
                int index2 = indices[i * 3 + 1];
                int index3 = indices[i * 3 + 2];

                Vector3 side1 = vertices[index1].Position - vertices[index3].Position;
                Vector3 side2 = vertices[index1].Position - vertices[index2].Position;
                Vector3 normal = Vector3.Cross(side1, side2);

                vertices[index1].Normal += normal;
                vertices[index2].Normal += normal;
                vertices[index3].Normal += normal;
            }

            for (int i = 0; i < vertices.Length; i++)
                vertices[i].Normal.Normalize();
        }

        private void CopyToBuffers()
        {
            myVertexBuffer = new VertexBuffer(graphicsDevice, VertexPositionColorNormal.VertexDeclaration, vertices.Length, BufferUsage.WriteOnly);
            myVertexBuffer.SetData(vertices);

            myIndexBuffer = new IndexBuffer(graphicsDevice, typeof(int), indices.Length, BufferUsage.WriteOnly);
            myIndexBuffer.SetData(indices);
        }
    }
}
