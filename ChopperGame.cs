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
        GraphicsDeviceManager graphicsDeviceManager;
        GraphicsDevice graphicsDevice;
        SpriteBatch spriteBatch;
        VertexBuffer vertexBuffer;
        IndexBuffer indexBuffer;
        Heightmap heightmap;
        BasicEffect effect;
        HeightmapSystem.VertexPositionColorNormal[] vertices;
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

            Content.RootDirectory = "Content";
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

            Texture2D heightmapTexture = Content.Load<Texture2D>("canyon");
            heightmap = new Heightmap(graphicsDevice);
            heightmap.LoadHeightmap(heightmapTexture, viewMatrix, projectionMatrix);

            chopperModel = Content.Load<Model>("Chopper");
            chopperPos = new Vector3(0, 0, 0);
            chopperAngle = 0;
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

            Vector3 lightDirection = new Vector3(1.0f, -1.0f, -1.0f);
            lightDirection.Normalize();

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

        private void SetUpCamera()
        {
            viewMatrix = Matrix.CreateLookAt(new Vector3(60, 80, -180), new Vector3(0, 0, 0), new Vector3(0, 1, 0));
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, graphicsDevice.Viewport.AspectRatio, 1.0f, 500.0f);
        }
    }
}
