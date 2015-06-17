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
        public TgcSphere renderModel;
        public float movementTimer = 0f;
        public float timeForNewMovement = 5f;
        public float minimumOffset = 20f;
        public float offsetX = 1;
        public float offsetY = 1;
        public float offsetZ = 1;

        public Asteroids(TgcSphere unModelo)
        {
            renderModel = unModelo;
        }

        public void Update(float elapsedTime) //Asigna con el paso del tiempo un nuevo valor de posicion futuro, obtenido con la funcion getNewMovement()
        {
            if (movementTimer <= 0f)
            {
                Random rndNewMovement = new Random();

                getNewMovement();
                movementTimer = timeForNewMovement * ((float)rndNewMovement.NextDouble()+0.1f);
            }

            movementTimer -= elapsedTime;
        }

        public void getNewMovement()    //Da futuras posiciones random, asignadas por un valor de offset minimo y una dirección que depende de otro valor random.
        {   
            Random rndMovementX = new Random();
            Random rndDirectionX = new Random();
            Random rndMovementY = new Random();
            Random rndDirectionY = new Random();
            Random rndMovementZ = new Random();
            Random rndDirectionZ = new Random();

            if ((float)rndDirectionX.NextDouble() < 0.5f)   { offsetX = -minimumOffset * ((float)rndMovementX.NextDouble() + 0.1f); }
            else                                            { offsetX = minimumOffset * ((float)rndMovementX.NextDouble() + 0.1f); }
            if ((float)rndDirectionY.NextDouble() < 0.5f)   { offsetY = -minimumOffset * ((float)rndMovementY.NextDouble() + 0.1f); }
            else                                            { offsetY = minimumOffset * ((float)rndMovementY.NextDouble() + 0.1f); }
            if ((float)rndDirectionX.NextDouble() < 0.5f)   { offsetZ = -minimumOffset * ((float)rndMovementZ.NextDouble() + 0.1f); }
            else                                            { offsetZ = minimumOffset * ((float)rndMovementZ.NextDouble() + 0.1f); }
        }
    }
}
