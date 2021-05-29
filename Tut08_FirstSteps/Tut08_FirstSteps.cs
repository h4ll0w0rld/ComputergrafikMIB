using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Scene;
using Fusee.Math.Core;
using Fusee.Serialization;
using Fusee.Xene;
using static Fusee.Engine.Core.Input;
using static Fusee.Engine.Core.Time;
using Fusee.Engine.GUI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FuseeApp
{
    [FuseeApplication(Name = "Tut08_FirstSteps", Description = "Yet another FUSEE App.")]
    public class Tut08_FirstSteps : RenderCanvas
    {
        private float _camAngle = 0;

        public SceneRendererForward _sceneRenderer;
        private Transform _cubeTransform;
        private SceneContainer _scene;



        // Init is called on startup. 
        public override void Init()
        {
            getCube(0, 7);
             _scene = new SceneContainer();

            RC.ClearColor = new float4(1, 1, 1, 1);
            


        }
        public SceneRendererForward getCube(int x, int ammount)
        {
            int i = 0;
            _scene = new SceneContainer();
            while (i < ammount)
            {

                _cubeTransform = new Transform { Scale = new float3(1, 1, 1), Translation = new float3(x + i * 10, 0, 0) };

                var cubeShader = MakeEffect.FromDiffuseSpecular((float4)ColorUint.Blue, float4.Zero);
                var cubeMesh = SimpleMeshes.CreateCuboid(new float3(5, 5, 5));

                // Assemble the cube node containing the three components
                var cubeNode = new SceneNode();
                cubeNode.Components.Add(_cubeTransform);
                cubeNode.Components.Add(cubeShader);
                cubeNode.Components.Add(cubeMesh);

                // Create the scene containing the cube as the only object

                _scene.Children.Add(cubeNode);
                i++;
            }

            // Create a scene renderer holding the scene above
            _sceneRenderer = new SceneRendererForward(_scene);
            

            return _sceneRenderer;

        }


        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {
           
            _camAngle += 90.0f * M.Pi / 180.0f * DeltaTime; ;

            SetProjectionAndViewport();


            // Clear the backbuffer
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);
            //  _cubeTransform.Translation = new float3(0, 5 * M.Sin(3 * TimeSinceStart), 0);
          
            _cubeTransform.Rotation = new float3(0, 3 * M.Sin(3 * TimeSinceStart), 0);
            _cubeTransform.Scale = new float3(3 * M.Sin(3 * TimeSinceStart), 3 * M.Sin(3 * TimeSinceStart), 3 * M.Sin(3 * TimeSinceStart));
           
            //Camera setup
              RC.View = float4x4.CreateTranslation(0, 0, 50) * float4x4.CreateRotationY(_camAngle);

            _sceneRenderer.Render(RC);



            // Swap buffers: Show the contents of the backbuffer (containing the currently rendered frame) on the front buffer.
            Present();
        }



        public void SetProjectionAndViewport()
        {
            // Set the rendering area to the entire window size
            RC.Viewport(0, 0, Width, Height);

            // Create a new projection matrix generating undistorted images on the new aspect ratio.
            var aspectRatio = Width / (float)Height;

            // 0.25*PI Rad -> 45° Opening angle along the vertical direction. Horizontal opening angle is calculated based on the aspect ratio
            // Front clipping happens at 1 (Objects nearer than 1 world unit get clipped)
            // Back clipping happens at 2000 (Anything further away from the camera than 2000 world units gets clipped, polygons will be cut)
            var projection = float4x4.CreatePerspectiveFieldOfView(M.PiOver4, aspectRatio, 1, 20000);
            RC.Projection = projection;
        }

    }



}