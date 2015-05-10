using System;
using System.Collections.Generic;
using System.Text;
using TgcViewer.Example;
using TgcViewer;
using Microsoft.DirectX.Direct3D;
using System.Drawing;
using Microsoft.DirectX;
using TgcViewer.Utils.Input;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.Terrain;
using TgcViewer.Utils.Shaders;
using TgcViewer.Utils;
using System.Windows.Forms;

namespace AlumnoEjemplos.MiGrupo
{
     class BlurEffect
    {
        Effect effect;
        Surface g_pDepthStencil;     // Depth-stencil buffer 
        Texture g_pRenderTarget, g_pRenderTarget4, g_pRenderTarget4Aux;
        VertexBuffer g_pVBV3D;

        int cant_pasadas = 2;
        bool separable = true;
        TgcBox box;

        public void Load()
        {
            GuiController.Instance.CustomRenderEnabled = true;

            Device d3dDevice = GuiController.Instance.D3dDevice;

            //Crear caja vacia
            box = new TgcBox();
            box.Color = Color.AliceBlue;
            Vector3 sizeAndPos = new Vector3(30,30,30);
            box.Position = sizeAndPos;
            box.Size = sizeAndPos;

            //Cargar Shader personalizado
            string compilationErrors;
            effect = Effect.FromFile(GuiController.Instance.D3dDevice,
                GuiController.Instance.AlumnoEjemplosMediaDir + "\\Shaders\\GaussianBlur.fx",
                null, null, ShaderFlags.PreferFlowControl, null, out compilationErrors);
            if (effect == null)
            {
                throw new Exception("Error al cargar shader. Errores: " + compilationErrors);
            }
            //Configurar Technique dentro del shader
            effect.Technique = "DefaultTechnique";


            g_pDepthStencil = d3dDevice.CreateDepthStencilSurface(d3dDevice.PresentationParameters.BackBufferWidth,
                                                             d3dDevice.PresentationParameters.BackBufferHeight,
                                                             DepthFormat.D24S8,
                                                             MultiSampleType.None,
                                                             0,
                                                             true);

            // inicializo el render target
            g_pRenderTarget = new Texture(d3dDevice, d3dDevice.PresentationParameters.BackBufferWidth
                    , d3dDevice.PresentationParameters.BackBufferHeight, 1, Usage.RenderTarget,
                        Format.X8R8G8B8, Pool.Default);

            g_pRenderTarget4 = new Texture(d3dDevice, d3dDevice.PresentationParameters.BackBufferWidth/4
                    , d3dDevice.PresentationParameters.BackBufferHeight/4, 1, Usage.RenderTarget,
                        Format.X8R8G8B8, Pool.Default);

            g_pRenderTarget4Aux = new Texture(d3dDevice, d3dDevice.PresentationParameters.BackBufferWidth / 4
                    , d3dDevice.PresentationParameters.BackBufferHeight / 4, 1, Usage.RenderTarget,
                        Format.X8R8G8B8, Pool.Default);

            effect.SetValue("g_RenderTarget", g_pRenderTarget);

            // Resolucion de pantalla
            effect.SetValue("screen_dx", d3dDevice.PresentationParameters.BackBufferWidth);
            effect.SetValue("screen_dy", d3dDevice.PresentationParameters.BackBufferHeight);

            CustomVertex.PositionTextured[] vertices = new CustomVertex.PositionTextured[]
		    {
    			new CustomVertex.PositionTextured( -1, 1, 1, 0,0), 
			    new CustomVertex.PositionTextured(1,  1, 1, 1,0),
			    new CustomVertex.PositionTextured(-1, -1, 1, 0,1),
			    new CustomVertex.PositionTextured(1,-1, 1, 1,1)
    		};

            //vertex buffer de los triangulos
            g_pVBV3D = new VertexBuffer(typeof(CustomVertex.PositionTextured),
                    4, d3dDevice, Usage.Dynamic | Usage.WriteOnly,
                        CustomVertex.PositionTextured.Format, Pool.Default);
            g_pVBV3D.SetData(vertices, 0, LockFlags.None);
        }


        public void Render(float elapsedTime, List<TgcMesh> allMeshes)
        {
            Device device = GuiController.Instance.D3dDevice;
            Control panel3d = GuiController.Instance.Panel3d;
            float aspectRatio = (float)panel3d.Width / (float)panel3d.Height;


            //Cargar variables de shader

            // dibujo la escena una textura 
            effect.Technique = "DefaultTechnique";
            // guardo el Render target anterior y seteo la textura como render target
            Surface pOldRT = device.GetRenderTarget(0);
            Surface pSurf = g_pRenderTarget.GetSurfaceLevel(0);
            
            device.SetRenderTarget(0, pSurf);
            // hago lo mismo con el depthbuffer, necesito el que no tiene multisampling
            Surface pOldDS = device.DepthStencilSurface;
            // Probar de comentar esta linea, para ver como se produce el fallo en el ztest
            // por no soportar usualmente el multisampling en el render to texture.
            
            device.DepthStencilSurface = g_pDepthStencil;

            device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
            device.BeginScene();

            //Dibujamos todos los meshes del escenario
            foreach (TgcMesh m in allMeshes)
            {
                m.render();
            }
            box.render();
            device.EndScene();
            pSurf.Dispose();


                if (separable)
                {
                    // 1er pasada: downfilter x 4
                    // -----------------------------------------------------
                    pSurf = g_pRenderTarget4.GetSurfaceLevel(0);
                    device.SetRenderTarget(0, pSurf);
                    device.BeginScene();
                    effect.Technique = "DownFilter4";
                    device.VertexFormat = CustomVertex.PositionTextured.Format;
                    device.SetStreamSource(0, g_pVBV3D, 0);
                    effect.SetValue("g_RenderTarget", g_pRenderTarget);

                    device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
                    effect.Begin(FX.None);
                    effect.BeginPass(0);
                    device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
                    effect.EndPass();
                    effect.End();
                    pSurf.Dispose();
                    device.EndScene();

                    // TextureLoader.Save("scene.bmp", ImageFileFormat.Bmp, g_pRenderTarget4);
                    device.DepthStencilSurface = pOldDS;

                    // Pasadas de blur
                    for (int P = 0; P < cant_pasadas; ++P)
                    {
                        // Gaussian blur Horizontal
                        // -----------------------------------------------------
                        pSurf = g_pRenderTarget4Aux.GetSurfaceLevel(0);
                        device.SetRenderTarget(0, pSurf);
                        // dibujo el quad pp dicho :
                        device.BeginScene();
                        effect.Technique = "GaussianBlurSeparable";
                        device.VertexFormat = CustomVertex.PositionTextured.Format;
                        device.SetStreamSource(0, g_pVBV3D, 0);
                        effect.SetValue("g_RenderTarget", g_pRenderTarget4);

                        device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
                        effect.Begin(FX.None);
                        effect.BeginPass(0);
                        device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
                        effect.EndPass();
                        effect.End();
                        pSurf.Dispose();
                        device.EndScene();
                        //TextureLoader.Save("blurH.bmp", ImageFileFormat.Bmp, g_pRenderTarget4Aux);

                        if (P < cant_pasadas - 1)
                        {
                            pSurf = g_pRenderTarget4.GetSurfaceLevel(0);
                            device.SetRenderTarget(0, pSurf);
                            pSurf.Dispose();
                        }
                        else
                            // Ultima pasada vertical va sobre la pantalla pp dicha
                            device.SetRenderTarget(0, pOldRT);


                        //  Gaussian blur Vertical
                        // -----------------------------------------------------
                        device.BeginScene();
                        effect.Technique = "GaussianBlurSeparable";
                        device.VertexFormat = CustomVertex.PositionTextured.Format;
                        device.SetStreamSource(0, g_pVBV3D, 0);
                        effect.SetValue("g_RenderTarget", g_pRenderTarget4Aux);

                        device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
                        effect.Begin(FX.None);
                        effect.BeginPass(1);
                        device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
                        effect.EndPass();
                        effect.End();
                        if (P < cant_pasadas - 1)
                            device.EndScene();

                        //TextureLoader.Save("blurV.bmp", ImageFileFormat.Bmp, g_pRenderTarget4);

                    }
                }
                else
                {
                    // Naive Gaussian blur
                    // restuaro el render target y el stencil
                    device.DepthStencilSurface = pOldDS;
                    device.SetRenderTarget(0, pOldRT);

                    // dibujo el quad pp dicho :
                    device.BeginScene();
                    effect.Technique = "GaussianBlur";
                    device.VertexFormat = CustomVertex.PositionTextured.Format;
                    device.SetStreamSource(0, g_pVBV3D, 0);
                    effect.SetValue("g_RenderTarget", g_pRenderTarget);

                    device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
                    effect.Begin(FX.None);
                    effect.BeginPass(0);
                    device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
                    effect.EndPass();
                    effect.End();
                }
                GuiController.Instance.Text3d.drawText("FPS: " + HighResolutionTimer.Instance.FramesPerSecond, 0, 0, Color.Yellow);
                device.EndScene();
            }

        public void Close()
        {
            effect.Dispose();
            g_pRenderTarget.Dispose();
            g_pRenderTarget4Aux.Dispose();
            g_pRenderTarget4.Dispose();
            g_pVBV3D.Dispose();
            g_pDepthStencil.Dispose();
        }
    }

}
