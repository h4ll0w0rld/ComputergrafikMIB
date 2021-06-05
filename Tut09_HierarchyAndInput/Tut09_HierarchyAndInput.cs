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
    [FuseeApplication(Name = "Tut09_HierarchyAndInput", Description = "Yet another FUSEE App.")]
    public class Tut09_HierarchyAndInput : RenderCanvas
    {
        private SceneContainer _scene;
        private SceneRendererForward _sceneRenderer;
        private float _camAngle = 0;
        private Transform _baseTransform;
        private Transform _bodyTransform;
        private Transform _upperArmTransform;
        private Transform _foreArmTransform;
        private Transform _hand1Transform;
         private Transform _hand2Transform;



        SceneContainer CreateScene()
        {
            // Initialize transform components that need to be changed inside "RenderAFrame"
            _baseTransform = new Transform
            {
                Rotation = new float3(0, 0, 0),
                Scale = new float3(1, 1, 1),
                Translation = new float3(0, 0, 0)
            };
            _bodyTransform = new Transform
            {
                Rotation = new float3(0, 0, 0),
                Scale = new float3(1, 1, 1),
                Translation = new float3(0, 6, 0)
            };
            _upperArmTransform = new Transform
            {
                Rotation = new float3(0, 0, 0),
                Scale = new float3(1, 1, 1),
                Translation = new float3(2, 4, 0),
            };
            _foreArmTransform = new Transform
            {
                Rotation = new float3(0, 0, 0),
                Scale = new float3(1, 1, 1),
                Translation = new float3(-2, 8, 0),
            };
            _hand1Transform = new Transform
            {
                Rotation = new float3(0, 0, M.Pi/4),
                Scale = new float3(1, 1, 1),
                Translation = new float3(0, 8, 0),
            };
            _hand2Transform= new Transform{
                Rotation = new float3(0,0,-M.Pi/4),
                Scale = new float3(1,1,1),
                Translation = new float3(0,8,0)
            };
            

            // Setup the scene graph
            return new SceneContainer
            {
                Children = new List<SceneNode>
            {
                // GREY BASE
                new SceneNode
                {
                    Components = new List<SceneComponent>
                    {
                        // TRANSFORM COMPONENT
                        _baseTransform,

                        // SHADER EFFECT COMPONENT
                        MakeEffect.FromDiffuseSpecular((float4) ColorUint.LightGrey, float4.Zero),

                        // MESH COMPONENT
                        SimpleMeshes.CreateCuboid(new float3(10, 2, 10))
                    }
                },
                // RED BODY
                new SceneNode
                {
                    Components = new List<SceneComponent>
                    {
                        _bodyTransform,
                        MakeEffect.FromDiffuseSpecular((float4) ColorUint.Red, float4.Zero),
                        SimpleMeshes.CreateCuboid(new float3(2, 10, 2))
                    },
                    Children = new ChildList{
                        new SceneNode{
                            Components = new List<SceneComponent>{
                                _upperArmTransform
                            },
                            Children = new ChildList
                                {
                                    new SceneNode
                                    {
                                        Components = new List<SceneComponent>
                                        {
                                            new Transform
                                            {
                                                Rotation = new float3(0, 0, 0),
                                                Scale = new float3(1, 1, 1),
                                                Translation = new float3(0, 4, 0)
                                            },
                                            MakeEffect.FromDiffuseSpecular((float4) ColorUint.Green, float4.Zero),
                                            SimpleMeshes.CreateCuboid(new float3(2, 10, 2))
                                        }
                                    },
                                    new SceneNode{
                                        Components = new List<SceneComponent>{
                                            _foreArmTransform
                                        },
                                        Children = new ChildList{
                                            new SceneNode{
                                                Components = new List<SceneComponent>{
                                                    new Transform{
                                                        Rotation = new float3(0,0,0),
                                                        Scale = new float3(1,1,1),
                                                        Translation = new float3(0,4,0),
                                                    },
                                                      MakeEffect.FromDiffuseSpecular((float4) ColorUint.Blue, float4.Zero),
                                                     SimpleMeshes.CreateCuboid(new float3(2, 10, 2))
                                                }
                                            },
                                            new SceneNode{
                                                Components = new List<SceneComponent>{
                                                    _hand1Transform
                                                },
                                                Children = new ChildList{
                                                    new SceneNode{
                                                        Components = new List<SceneComponent>{
                                                            new Transform{
                                                                  Rotation = new float3(0,0,0),
                                                        Scale = new float3(1,1,1),
                                                        Translation = new float3(0,2,0),
                                                            },
                                                              MakeEffect.FromDiffuseSpecular((float4) ColorUint.Pink, float4.Zero),
                                                                SimpleMeshes.CreateCuboid(new float3(1, 4, 1))

                                                        }
                                                    }
                                                }

                                            },
                                            new SceneNode{
                                                Components = new List<SceneComponent>{
                                                    _hand2Transform
                                                },
                                                Children = new ChildList{
                                                    new SceneNode{
                                                        Components = new List<SceneComponent>{
                                                            new Transform{
                                                                Rotation = new float3(0,0,0),
                                                                Scale = new float3(1,1,1),
                                                                Translation = new float3(0,2,0),
                                                            },
                                                            MakeEffect.FromDiffuseSpecular((float4) ColorUint.Pink, float4.Zero),
                                                                SimpleMeshes.CreateCuboid(new float3(1, 4, 1))

                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                        }
                    }
                }
            }
        }
            };
        }


        // Init is called on startup. 
        public override void Init()
        {
            // Set the clear color for the backbuffer to white (100% intensity in all color channels R, G, B, A).
            RC.ClearColor = new float4(0.8f, 0.9f, 0.7f, 1);

            _scene = CreateScene();

            // Create a scene renderer holding the scene above
            _sceneRenderer = new SceneRendererForward(_scene);
        }

        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {
            SetProjectionAndViewport();

            // Clear the backbuffer
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            // Setup the camera 
            RC.View = float4x4.CreateTranslation(0, -10, 50) * float4x4.CreateRotationY(_camAngle);

            _bodyTransform.Rotation.y += Keyboard.LeftRightAxis * Time.DeltaTime;

            _upperArmTransform.Rotation.x += Keyboard.UpDownAxis * Time.DeltaTime;

            _foreArmTransform.Rotation.x += Keyboard.ADAxis * Time.DeltaTime;


            if (Mouse.LeftButton)
            {

                _camAngle += Mouse.Velocity.x * Time.DeltaTime / 6;

            }
            else if (!Mouse.LeftButton && Mouse.Velocity.x > 0)
            {
                _camAngle -= 1 * Time.DeltaTime;
            }
            if(Mouse.RightButton && _hand1Transform.Rotation.z >-0.2f)
            {
                _hand1Transform.Rotation += new float3(0,0,-0.2f);
                _hand2Transform.Rotation += new float3(0,0,0.2f);

            }
            else if (!Mouse.RightButton && _hand1Transform.Rotation.z <= M.Pi/4 ){
                 _hand1Transform.Rotation += new float3(0,0,0.2f);
                   _hand2Transform.Rotation += new float3(0,0,-0.2f);
            }

            // Render the scene on the current render context
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