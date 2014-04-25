// Implementation of first person fly through camera
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace CS426Assignment3
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Model terrain;
        Matrix[] terrain_boneTransforms;
        Matrix terrain_transform;

        Vector3 viewerPosition = new Vector3(0, 2, -24);
        Quaternion viewerRotation = Quaternion.CreateFromAxisAngle(new Vector3(0, 1, 0), -MathHelper.PiOver2);
        float dx, dy, dz;
        Matrix view, projection;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            // graphics.IsFullScreen = true;
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            terrain = Content.Load<Model>(@"houses");
            terrain_boneTransforms = new Matrix[terrain.Bones.Count];
            terrain.CopyAbsoluteBoneTransformsTo(terrain_boneTransforms);
            terrain_transform = terrain.Root.Transform * Matrix.CreateScale(0.01f); // 0.05 Campus

        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState keys = Keyboard.GetState();
            if (keys.IsKeyDown(Keys.Q)) this.Exit();

            dx = 0f; dy = 0f; dz = 0f;
            if (keys.IsKeyDown(Keys.Right)) dy = -0.01f;
            if (keys.IsKeyDown(Keys.Left)) dy = 0.01f;
            if (keys.IsKeyDown(Keys.Down)) dx = -0.01f;
            if (keys.IsKeyDown(Keys.Up)) dx = 0.01f;
            if (keys.IsKeyDown(Keys.PageDown)) dz = 0.01f;
            if (keys.IsKeyDown(Keys.PageUp)) dz = -0.01f;

            viewerRotation *= Quaternion.CreateFromAxisAngle(Vector3.Up, dy)
                * Quaternion.CreateFromAxisAngle(Vector3.Right, dx)
                * Quaternion.CreateFromAxisAngle(Vector3.Backward, dz);

            float advance = 0.01f;

            Vector3 changePosition = Vector3.Transform(new Vector3(0, 0, advance), Matrix.CreateFromQuaternion(viewerRotation));
            viewerPosition += changePosition;

            Vector3 camup = new Vector3(0, 1, 0);
            camup = Vector3.Transform(camup, Matrix.CreateFromQuaternion(viewerRotation));

            view = Matrix.CreateLookAt(viewerPosition - changePosition, viewerPosition, camup); // (camera position, camera target, up)

            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, graphics.GraphicsDevice.Viewport.AspectRatio, 0.01f, 5000.0f);

            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            terrain.CopyAbsoluteBoneTransformsTo(terrain_boneTransforms);
            terrain.Root.Transform = terrain_transform;
            foreach (ModelMesh mesh in terrain.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.View = view;
                    effect.Projection = projection;
                    effect.World = terrain_boneTransforms[mesh.ParentBone.Index];
                }
                mesh.Draw();
            }

            base.Draw(gameTime);
        }
    }
}
