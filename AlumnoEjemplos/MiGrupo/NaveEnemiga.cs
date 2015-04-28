﻿using System;
using System.Collections.Generic;
using System.Text;
using TgcViewer.Example;
using TgcViewer;
using Microsoft.DirectX.Direct3D;
using System.Drawing;
using Microsoft.DirectX;
using TgcViewer.Utils.Modifiers;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;
using System.Diagnostics;

namespace AlumnoEjemplos.NaveEnemiga
{
    public class NaveEnemiga : TgcExample
    {

        TgcBox box; //Simula la nave principal
        TgcMesh naveEnemiga; //Nave
        Vector3 dir_nave;
        Vector3 pos_nave;
        float time;
        Random random = new Random();

        //Variable direccion de movimiento
        float currentMoveDir = 1f;

        public override string getCategory()
        {
            return "AlumnoEjemplos";
        }

        public override string getName()
        {
            return "Nave Enemiga";
        }

        public override string getDescription()
        {
            return "NaveEnemiga";
        }

        public override void init()
        {
            Device d3dDevice = GuiController.Instance.D3dDevice;
            string alumnoMediaFolder = GuiController.Instance.AlumnoEjemplosMediaDir;

            //Creo la caja...
            Vector3 center = new Vector3(0, 0, 0);
            Vector3 size = new Vector3(10, 10, 10);
            Color color = Color.Red;
            box = TgcBox.fromSize(center, size, color);

            //Loader de Scenes y los Meshes
            TgcSceneLoader loader = new TgcSceneLoader();
            TgcScene scene = loader.loadSceneFromFile(GuiController.Instance.AlumnoEjemplosMediaDir + "NaveStarWars\\naveStarWars-TgcScene.xml");
            naveEnemiga = scene.Meshes[0];

            //Inicializo la nave
            obtenerPosicionYMovimiento();
            naveEnemiga.Scale = new Vector3(0.2f, 0.2f, 0.2f);
            naveEnemiga.Position = pos_nave;
            naveEnemiga.AutoTransformEnable = false;
            dir_nave = new Vector3(0, 0, 1);


            //Velocidad de la Nave
            GuiController.Instance.Modifiers.addFloat("Speed", 0f, 30f, 10f);


            //Camara sobre la caja
            GuiController.Instance.RotCamera.CameraDistance = 200;
            GuiController.Instance.RotCamera.RotationSpeed = 10.0f;
        }

        public override void render(float elapsedTime)
        {
            Device d3dDevice = GuiController.Instance.D3dDevice;

            //Obtene valor de Modifier
            float Speed = (float)GuiController.Instance.Modifiers["Speed"];
            
            //Configurar Posicion y Movimiento de la nave
            time += elapsedTime;
            float alfa = -time * Geometry.DegreeToRadian(15.0f);
            naveEnemiga.Position = new Vector3(pos_nave.X * (float)Math.Cos(alfa), pos_nave.Y * (float)Math.Sin(alfa), pos_nave.Z * (float)Math.Sin(alfa));
            dir_nave = new Vector3(-(float)Math.Sin(alfa), 0, (float)Math.Cos(alfa));
            naveEnemiga.Transform = CalcularMatriz(naveEnemiga.Position, naveEnemiga.Scale, dir_nave);

            //Renderizar nave y caja
            naveEnemiga.move(0, 0, -Speed * currentMoveDir * elapsedTime);
            naveEnemiga.render();
            box.render();
        }

        public override void close()
        {
            //Liberar memoria
            naveEnemiga.dispose();
            box.dispose();

        }

        //TODO Configurar para que sea random
        public Matrix CalcularMatriz(Vector3 Pos, Vector3 Scale, Vector3 Dir)
        {
            Vector3 VUP = new Vector3(0, 1, 0);
            Matrix matWorld = Matrix.Scaling(Scale);
            // Determino la orientacion
            Vector3 U = Vector3.Cross(VUP, Dir);
            U.Normalize();
            Vector3 V = Vector3.Cross(Dir, U);
            Matrix Orientacion;
            Orientacion.M11 = U.X;
            Orientacion.M12 = U.Y;
            Orientacion.M13 = U.Z;
            Orientacion.M14 = 0;

            Orientacion.M21 = V.X;
            Orientacion.M22 = V.Y;
            Orientacion.M23 = V.Z;
            Orientacion.M24 = 0;

            Orientacion.M31 = Dir.X;
            Orientacion.M32 = Dir.Y;
            Orientacion.M33 = Dir.Z;
            Orientacion.M34 = 0;

            Orientacion.M41 = 0;
            Orientacion.M42 = 0;
            Orientacion.M43 = 0;
            Orientacion.M44 = 1;
            matWorld = matWorld * Orientacion;

            // Traslado
            matWorld = matWorld * Matrix.Translation(Pos);
            return matWorld;
        }

        public void obtenerPosicionYMovimiento()
        {
            float x1 = getRandomFloat();
            float y1 = getRandomFloat();
            float z1 = getRandomFloat();

            //Limitar para que no tenga mucho movimiento sobre eje y
            while (y1 > 20){
                y1 = getRandomFloat();
            }

            pos_nave = new Vector3(x1, y1, z1);
        }

        public float getRandomFloat(){
            float r = (float)random.Next(20, 200);
            
            return r;
        }

    }
}
