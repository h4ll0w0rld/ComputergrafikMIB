using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Scene;
using Fusee.Engine.Core.Effects;
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
    [FuseeApplication(Name = "Tut11_AssetsPicking", Description = "Yet another FUSEE App.")]
    public class Tut11_AssetsPicking : RenderCanvas
    {
        private SceneContainer _scene;
        private SceneRendererForward _sceneRenderer;
        private Transform _bodyTransform;
        private Transform _transformRadHintenLinks, _transformRadHintenRechts, _transformRadVorneRechts, _transformRadVorneLinks, _schaufelTransform;
        private SurfaceEffect _rightRearEffect;
        private ScenePicker _scenePicker;
        private PickResult _currentPick;
        private float4 _oldColor;



        SceneContainer CreateScene()
        {
            // Initialize transform components that need to be changed inside "RenderAFrame"
            _bodyTransform = new Transform
            {
                Rotation = new float3(0, 0, 0),
                Scale = new float3(1, 1, 1),
                Translation = new float3(0, 0, 0)
            };

            // Setup the scene graph
            return new SceneContainer
            {
                Children = new List<SceneNode>
                {
                    new SceneNode
                    {
                        Components = new List<SceneComponent>
                        {
                            // TRANSFROM COMPONENT
                            _bodyTransform,

                            // SHADER EFFECT COMPONENT
                            SimpleMeshes.MakeMaterial((float4) ColorUint.LightGrey),

                            // MESH COMPONENT
                            // SimpleAssetsPickinges.CreateCuboid(new float3(10, 10, 10))
                            SimpleMeshes.CreateCuboid(new float3(10, 10, 10))
                        }
                    },
                }
            };
        }


        // Init is called on startup. 
        public override void Init()
        {
            RC.ClearColor = new float4(0.8f, 0.9f, 0.7f, 1);

            _scene = CreateScene();
            _scene = AssetStorage.Get<SceneContainer>("Traktor.fus");

            //Pick Parts of asset 
            _rightRearEffect = _scene.Children.FindNodes((node) => node.Name == "Rad_hinten_links")?.FirstOrDefault()?.GetComponent<SurfaceEffect>();

            _transformRadHintenLinks = _scene.Children.FindNodes(node => node.Name == "Rad_hinten_links")?.FirstOrDefault()?.GetTransform();
            _transformRadHintenRechts = _scene.Children.FindNodes(node => node.Name == "Rad_hinten_rechts")?.FirstOrDefault()?.GetTransform();
            _transformRadVorneLinks = _scene.Children.FindNodes(node => node.Name == "Rad_vorne_links")?.FirstOrDefault()?.GetTransform();
            _transformRadVorneRechts = _scene.Children.FindNodes(node => node.Name == "Rad_vorne_rechts")?.FirstOrDefault()?.GetTransform();
            //_schaufelTransform = _scene.Children.FindNodes(node => node.Name == "Schaufel")?.FirstOrDefault()?.GetTransform();



            _bodyTransform = _scene.Children.FindNodes((node) => node.Name == "Traktor")?.FirstOrDefault()?.GetTransform();


            // _rightRearEffect = _scene.Children.FindNodes(node => node.Name == "RightRearWheel")?.FirstOrDefault()?.GetComponent<SurfaceEffect>();
            // _rightRearEffect.SurfaceInput.Albedo = (float4)ColorUint.OrangeRed;

            _scenePicker = new ScenePicker(_scene);


            // Create a scene renderer holding the scene above
            _sceneRenderer = new SceneRendererForward(_scene);
        }


        public override void RenderAFrame()
        {
            SetProjectionAndViewport();
            //_rightRearEffect.SurfaceInput.Albedo = (float4)ColorUint.OrangeRed;

            // _rightRearTransform.Rotation = new float3(0, M.MinAngle(TimeSinceStart), 0);
            // _bodyTransform.Rotation = new float3(0, M.MinAngle(TimeSinceStart), 0);

            // Clear the backbuffer
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            // Setup the camera 
            //RC.View = float4x4.CreateTranslation(0, 0, 40) * float4x4.CreateRotationX(-(float)Math.Atan(15.0 / 40.0));

            RC.View = float4x4.CreateTranslation(0, 0, 40);
            // Render the scene on the current render context
            _sceneRenderer.Render(RC);

            // Setup the camera 

            if (Mouse.LeftButton)
            {
                float2 pickPosClip = Mouse.Position * new float2(2.0f / Width, -2.0f / Height) + new float2(-1, 1);

                PickResult newPick = _scenePicker.Pick(RC, pickPosClip).OrderBy(pr => pr.ClipPos.z).FirstOrDefault();



                if (newPick?.Node != _currentPick?.Node)
                {
                    if (_currentPick != null)
                    {
                        var ef = _currentPick.Node.GetComponent<DefaultSurfaceEffect>();
                        ef.SurfaceInput.Albedo = _oldColor;
                    }
                    if (newPick != null)
                    {
                        var ef = newPick.Node.GetComponent<SurfaceEffect>();
                        _oldColor = ef.SurfaceInput.Albedo;
                        ef.SurfaceInput.Albedo = (float4)ColorUint.Blue;




                    }
                }
                _currentPick = newPick;

            }
            // if (_currentPick != null)
            //     Diagnostics.Debug("The picked object is " + _currentPick.Node.Name);
            // if (_currentPick != null && _currentPick.Node.Name == "_transformRadHintenLinks")

            _transformRadHintenLinks.Rotation.y += -Keyboard.WSAxis * DeltaTime * 3;
            // else if (_currentPick != null && _currentPick.Node.Name == " _transformRadHintenRechts")
            _transformRadHintenRechts.Rotation.y += -Keyboard.WSAxis * DeltaTime * 3;
            // else if (_currentPick != null && _currentPick.Node.Name == " _transformRadVorneRechts")
            _transformRadVorneRechts.Rotation.y += -Keyboard.WSAxis * DeltaTime * 3;
            // else if (_currentPick != null && _currentPick.Node.Name == "  _transformRadVorneLinks")
            _transformRadVorneLinks.Rotation.y += -Keyboard.WSAxis * DeltaTime * 3;

           // _schaufelTransform.Rotation.y += Keyboard.ADAxis*DeltaTime;

            _bodyTransform.Translation.z += Keyboard.LeftRightAxis / 10;
            _bodyTransform.Translation.x += Keyboard.WSAxis / 10;
            _bodyTransform.Rotation.y += Keyboard.ADAxis * DeltaTime;


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