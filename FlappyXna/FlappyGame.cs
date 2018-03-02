﻿using FlappyXna.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FlappyXna
{
    public class FlappyGame : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Panorama panorama;
        Bird bird;
        Ground ground;

        PhysicsEngine physics;

        KeyboardState lastKeyboardState;
        bool gameOver;

        public FlappyGame()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = 505;
            graphics.PreferredBackBufferWidth = 288;
            graphics.ApplyChanges();
            Content.RootDirectory = "Content";
            gameOver = true;
        }

        protected override void Initialize()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            this.Services.AddService<SpriteBatch>(spriteBatch);

            physics = new PhysicsEngine(this);
            this.Components.Add(physics);

            panorama = new Panorama(this);
            this.Components.Add(panorama);

            bird = new Bird(this);
            this.Components.Add(bird);
            physics.AddBody(bird);

            ground = new Ground(this);
            this.Components.Add(ground);
            physics.AddBody(ground);

            base.Initialize();
        }

        protected override void LoadContent()
        {

        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        private void StartGame()
        {
            bird.Reset();
        }

        protected override void Update(GameTime gameTime)
        {
            physics.CheckCollision(bird, ground, OnBirdCollided);

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            var currentKeyboardState = Keyboard.GetState();
            if (currentKeyboardState.IsKeyDown(Keys.Space) && lastKeyboardState.IsKeyUp(Keys.Space))
            {
                if (gameOver)
                {
                    bird.Reset();
                    gameOver = false;
                    ground.IsAlive = true;
                    panorama.IsAlive = true;
                }
                else
                {
                    bird.Flap();
                }
            }
            
            base.Update(gameTime);

            lastKeyboardState = currentKeyboardState;
        }

        private void OnBirdCollided(IPhysicsBody bird, IPhysicsBody enemy)
        {
            var actualBird = bird as Bird;
            if (!actualBird.OnGround)
            {
                actualBird.OnGround = true;
            }

            if (!gameOver)
            {
                gameOver = true;
                actualBird.IsAlive = false;
                ground.IsAlive = false;
                panorama.IsAlive = false;
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.FromNonPremultiplied(7,140,254,255));

            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.LinearWrap, null, null);
            base.Draw(gameTime);
            spriteBatch.End();
        }
    }
}
