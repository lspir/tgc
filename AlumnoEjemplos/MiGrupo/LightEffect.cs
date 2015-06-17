﻿using System;
using System.Collections.Generic;
using System.Text;
using TgcViewer.Example;
using TgcViewer;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcSceneLoader;
using System.Drawing;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.Shaders;
using TgcViewer.Utils;

namespace AlumnoEjemplos.MiGrupo
{
    class LightEffect
    {
        Effect effect;
        Vector3[] lightsPos;

        Color lightColor;
        float lightIntensity;
        float lightAttenuation;
        float specularEx;
        float k_la; //ambiente
        float k_ld; //diffuse
        float k_ls; //specular

        TgcMesh mesh;

        Boolean usandoPhong;

        public void Iniciar(TgcMesh unaNave)
        {
            GuiController.Instance.CustomRenderEnabled = true;
            Device d3dDevice = GuiController.Instance.D3dDevice;

            mesh = unaNave;

            //Cargar Shader personalizado
            string compilationErrors;
            effect = Effect.FromFile(GuiController.Instance.D3dDevice,
                GuiController.Instance.AlumnoEjemplosMediaDir + "\\Shaders\\LightShading.fx",
                null, null, ShaderFlags.PreferFlowControl, null, out compilationErrors);
            if (effect == null)
            {
                throw new Exception("Error al cargar shader. Errores: " + compilationErrors);
            }

            effect.Technique = "DefaultTechnique";

            mesh.Effect = effect;
            mesh.Technique = "DefaultTechnique";

            lightsPos = new Vector3[2];

            lightColor = Color.Green; //probando nomas
            lightIntensity = 40f;
            lightAttenuation = 0.15f;
            specularEx = 9f;
            k_la = 0.5f;
            k_ld = 0.6f;
            k_ls = 0.5f;

            usandoPhong = true; ;

            if (!usandoPhong)
            {
                d3dDevice.Lights[0].Type = LightType.Point;
                d3dDevice.Lights[0].Diffuse = Color.FromArgb(255, 255, 255, 255);
                d3dDevice.Lights[0].Specular = Color.FromArgb(255, 255, 255, 255);
                d3dDevice.Lights[0].Attenuation0 = 0.2f;
                d3dDevice.Lights[0].Range = 1.0f;
                d3dDevice.Lights[0].Enabled = true;
            }
        }

        public void Render(float elapsedTime, Vector3 luz1, Vector3 luz2)
        {
            Device device = GuiController.Instance.D3dDevice;

            lightsPos[0] = luz1;
            lightsPos[1] = luz2;

            //Cargar variables shader de la luz
            //mesh.Effect.SetValue("lightColor", ColorValue.FromColor(lightColor));
            if (usandoPhong)
            {

                mesh.Effect.SetValue("fvLightPosition1", TgcParserUtils.vector3ToFloat4Array(lightsPos[0]));
                mesh.Effect.SetValue("fvLightPosition2", TgcParserUtils.vector3ToFloat4Array(lightsPos[1]));
                mesh.Effect.SetValue("fvEyePosition", TgcParserUtils.vector3ToFloat4Array(GuiController.Instance.ThirdPersonCamera.getPosition()));
                mesh.Effect.SetValue("lightIntensity", lightIntensity);
                mesh.Effect.SetValue("lightAttenuation", lightAttenuation);

                mesh.Effect.SetValue("k_la", k_la);
                mesh.Effect.SetValue("k_ld", k_ld);
                mesh.Effect.SetValue("k_ls", k_ls);
                mesh.Effect.SetValue("fSpecularPower", specularEx);
            }
            else
            {
                device.SetRenderState(RenderStates.Lighting, true);
                device.SetRenderState(RenderStates.SpecularEnable, true);
                device.Lights[0].Position = lightsPos[0];
                device.Lights[0].Update();
            }

            mesh.render();
        }

        public void Close()
        {
            effect.Dispose();
        }
    }
}
