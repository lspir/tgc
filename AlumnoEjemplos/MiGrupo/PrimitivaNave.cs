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
using TgcViewer.Utils.Terrain;

namespace AlumnoEjemplos.NaveEspacial
{
    public class NaveEspacial : TgcExample
    {
        TgcBox box; //caja
        TgcMesh spaceShip; //nave
        TgcSkyBox skyBox;  //cieloEnvolvente
        float currentAccel;
        float maxSpeed;
        int subiendoDir; //1 - subiendo, 0 - neutro, -1 - bajando
        float anguloSubida;

        public override string getCategory()
        {
            return "AlumnoEjemplos";
        }

        public override string getName()
        {
            return "Grupo 00";
        }

        public override string getDescription()
        {
            return "NaveEspacial - Primitiva de lo que sera el TP";
        }



        // Método que se llama una sola vez,  al principio cuando se ejecuta el ejemplo.
        // Escribir aquí todo el código de inicialización: cargar modelos, texturas, modifiers, uservars, etc.
        public override void init()
        {
            //GuiController.Instance: acceso principal a todas las herramientas del Framework
            //Device de DirectX para crear primitivas
            Device d3dDevice = GuiController.Instance.D3dDevice;

            //Cargo el loader de Scenes y los Meshes
            TgcSceneLoader loader = new TgcSceneLoader();
            TgcScene scene = loader.loadSceneFromFile(GuiController.Instance.ExamplesMediaDir + "MeshCreator\\Meshes\\Vehiculos\\AvionCaza\\AvionCaza-TgcScene.xml");

                
                //SKYBOX//
                //Textura del Cielo
                string texturesPath = GuiController.Instance.ExamplesMediaDir + "Texturas\\Quake\\SkyBox2\\";

                //Crear SkyBox 
                skyBox = new TgcSkyBox();
                skyBox.Center = new Vector3(0, 0, 0);
                skyBox.Size = new Vector3(4000, 4000, 4000);
                
                //Configurar las texturas para cada una de las 6 caras
                skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Up, texturesPath + "lun4_up.jpg");
                skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Down, texturesPath + "lun4_dn.jpg");
                skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Left, texturesPath + "lun4_lf.jpg");
                skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Right, texturesPath + "lun4_rt.jpg");
                skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Front, texturesPath + "lun4_bk.jpg");
                skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Back, texturesPath + "lun4_ft.jpg");

                skyBox.updateValues();

       
                
                
            //AGREGO UNA CAJA de METAL
            Vector3 center = new Vector3(0, 0, 0);
            Vector3 size = new Vector3(5, 5, 5);
            TgcTexture texture = TgcTexture.createTexture(GuiController.Instance.ExamplesMediaDir + "MeshCreator\\Textures\\Metal\\cajaMetal.jpg");
            box = TgcBox.fromSize(center, size, texture);

            //AGREGO UNA NAVE
            spaceShip = scene.Meshes[0];


             ///////////////MODIFIERS//////////////////
            //display de la aceleracion de la nave
            GuiController.Instance.Modifiers.addFloat("curAccel", -15f, 15f, 0f);

            //lo que va incrementando la aceleracion
            GuiController.Instance.Modifiers.addFloat("SpeedModifier", 0f, 2f, 0.1f);

            //Para la Rotacion de la Caja a los costados (Float)
            GuiController.Instance.Modifiers.addFloat("RotationY", 0f, 20f, 2f);

            //Para la Rotacion de la Caja arriba y abajo (Float)
            GuiController.Instance.Modifiers.addFloat("RotationX", 0f, 10f, 1f);

            //Para la Rotacion de la Caja como barrelRoll (Float)
            GuiController.Instance.Modifiers.addFloat("RotationZ", 0f, 5f, 0.5f);


            //AGREGO LA CAMARA QUE LA SIGUE
            //Habilito la camara en 1era Persona
            GuiController.Instance.ThirdPersonCamera.Enable = true;
            //Configurar a quien sigue y a que distancia Altura y Lejania
            GuiController.Instance.ThirdPersonCamera.setCamera(spaceShip.Position, 100, 200);

            //seteo valor inicial de las variables del movimiento
            currentAccel = 0f;
            maxSpeed = -2f; //valor temporal
            subiendoDir = 0;
            anguloSubida = 0f;
        }



        // Método que se llama cada vez que hay que refrescar la pantalla.
        // Escribir aquí todo el código referido al renderizado.
        public override void render(float elapsedTime)
        {
            //Device de DirectX para renderizar
            Device d3dDevice = GuiController.Instance.D3dDevice;

            //Obtener valores de Modifiers
            float curAccel = (float)GuiController.Instance.Modifiers["curAccel"];
            float Speed = (float)GuiController.Instance.Modifiers["SpeedModifier"];
            float RotationY = (float)GuiController.Instance.Modifiers["RotationY"];
            float RotationX = (float)GuiController.Instance.Modifiers["RotationX"];
            float RotationZ = (float)GuiController.Instance.Modifiers["RotationZ"];

            ///////////////INPUT TECLADO//////////////////
            //Tecla W apretada
            if (GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.W))
            {
                //spaceShip.move(Speed * elapsedTime, 0, 0); //hacia adelante en X
                //spaceShip.moveOrientedY(-Speed * elapsedTime);
                currentAccel -= Speed * elapsedTime;

                if (currentAccel < maxSpeed) { currentAccel = maxSpeed; } //limito la velocidad maxima
            }

            if (GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.S))
            {
                //spaceShip.move(Speed * elapsedTime, 0, 0); //hacia adelante en X
                //spaceShip.moveOrientedY(-Speed * elapsedTime);
                currentAccel += Speed * elapsedTime;

                if (currentAccel > 0) { currentAccel = 0; } //es desaceleracion, no la reversa
            }

            //actualizo el display de la aceleracion
            curAccel = currentAccel;

            //muevo la nave en base a la aceleracion que se le da
            spaceShip.moveOrientedY(currentAccel);

            //Tecla A apretada
            if (GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.D))
            {
                spaceShip.rotateY(RotationY * elapsedTime); //rota hacia la izq en Y
                GuiController.Instance.ThirdPersonCamera.rotateY(RotationY * elapsedTime);
            }

            //Tecla D apretada
            if (GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.A))
            {
                spaceShip.rotateY(-RotationY * elapsedTime); //rota hacia la der en Y
                GuiController.Instance.ThirdPersonCamera.rotateY(-RotationY * elapsedTime);
            }

            //Shift, quiero subir
            if (GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.LeftShift))
            {
                subiendoDir = 1;
            }
            else if (GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.LeftControl)) //control, quiero bajar
            {
                subiendoDir = -1;
            }
            else //nada
            {
                subiendoDir = 0;
            }

            //proceso la subiendoDir
            spaceShip.move(0, curAccel * subiendoDir * -1f, 0); //multiplico por la dir para que sea Arriba/Abajo
            Subiendo(subiendoDir, elapsedTime);

            //Tecla Up apretada
            if (GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.Up))
            {
                spaceShip.rotateX(RotationX * elapsedTime); //rota hacia la arriba en X
            }

            //Tecla Down apretada
            if (GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.Down))
            {
                spaceShip.rotateX(-RotationX * elapsedTime); //rota hacia la abajo en X
            }

            //Tecla Right apretada
            if (GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.Right))
            {
                spaceShip.rotateZ(RotationZ * elapsedTime); //rota barrelRoll en Z hacia la der
            }

            //Tecla Left apretada
            if (GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.Left))
            {
                spaceShip.rotateZ(-RotationZ * elapsedTime); //rota barrelRoll en Z hacia la izq
            }


            //Hacer que la cámara en 3ra persona se ajuste a la nueva posición del objeto
            GuiController.Instance.ThirdPersonCamera.Target = spaceShip.Position;

            //RENDER
            //Siempre primero hacer todos los cálculos de lógica e input y luego al final dibujar todo (ciclo update-render)
            skyBox.render();
            box.render();
            spaceShip.render();
        }



        // Método que se llama cuando termina la ejecución del ejemplo.
        // Hacer dispose() de todos los objetos creados.
        public override void close()
        {
            skyBox.dispose();
            box.dispose();
            spaceShip.dispose();
        }

        public void Subiendo(int unaDir, float deltaTime)
        {
            if (subiendoDir != 0)
            {
                if (anguloSubida < Geometry.DegreeToRadian(30) * unaDir || anguloSubida > Geometry.DegreeToRadian(30) * unaDir) { anguloSubida += 0.1f * unaDir * deltaTime; spaceShip.rotateX(0.1f * unaDir * deltaTime); }
            }
            else
            {
                spaceShip.rotateX(-anguloSubida);
                anguloSubida = 0;
            }
        }
    }
}
