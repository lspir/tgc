using System;
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

namespace AlumnoEjemplos.MiGrupo
{
    class Asteroids
    {
        TgcMesh asteroid;
        float movementTimer = 0f;
        float timeForNewMovement = 50f;
        Vector3 movement;
        Device d3dDevice = GuiController.Instance.D3dDevice;

        public void Load()
        {
            //Cargo el loader de Scenes y los Meshes
            TgcSceneLoader loader = new TgcSceneLoader();
            string sphere = GuiController.Instance.ExamplesMediaDir + "ModelosTgc\\Sphere\\Sphere-TgcScene.xml";
            asteroid = loader.loadSceneFromFile(sphere).Meshes[0];
            asteroid.changeDiffuseMaps(new TgcTexture[] { TgcTexture.createTexture(d3dDevice, GuiController.Instance.AlumnoEjemplosMediaDir + "Texturas\\AsteroidTexture.jpg") });

            Random rndScale = new Random();
            asteroid.Scale = (new Vector3(6 + (float)rndScale.NextDouble(), 6 + (float)rndScale.NextDouble(), 6 + (float)rndScale.NextDouble()));
            Random rndPosition = new Random();
            asteroid.Position = (new Vector3(100 + (float)rndPosition.NextDouble(), 100 + (float)rndPosition.NextDouble(), (float)rndPosition.NextDouble()));

            movement = getNewMovement();

            //return asteroid.BoundingBox;
        }
        public void Update(float elapsedTime)
        {
            if (movementTimer == 0f)
            {
                Random rndNewMovement = new Random();

                movement = getNewMovement();
                movementTimer = timeForNewMovement * (float)rndNewMovement.NextDouble();
            }

            movementTimer -= elapsedTime;
            asteroid.move(movement);
        }

        public void Render()
        {
            asteroid.render();
        }

        public void Close()
        {
            asteroid.dispose();
        }

        public Vector3 getNewMovement(){
            Random rndMovementX = new Random();
            Random rndMovementY = new Random();
            Random rndMovementZ = new Random();

            return new Vector3((float)rndMovementX.NextDouble(), (float)rndMovementY.NextDouble(), (float)rndMovementZ.NextDouble());
        }
    }
}
