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
using TgcViewer.Utils._2D;

namespace AlumnoEjemplos.NaveEspacial
{
    public class NaveEspacial : TgcExample
    {
        TgcBox box; //caja
        TgcMesh spaceShip; //nave
        TgcSkyBox skyBox;  //cieloEnvolvente
        TgcText2d text1;    //textoExplicacion
        float currentAccel;
        float maxSpeed;
        float AngleZRotation;

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

            //TEXTO de explicacion
            text1 = new TgcText2d();
            text1.Text = "Aceleracion: W, Freno: S,                     RotarHorizontal: A y D";
            text1.Align = TgcText2d.TextAlign.RIGHT;
            text1.Position = new Point(50, 50);
            text1.Size = new Size(300, 100);
            text1.Color = Color.Gold;
                
                //SKYBOX//
                //Textura del Cielo
                string texturesPath = GuiController.Instance.ExamplesMediaDir + "Texturas\\Quake\\SkyBox2\\";

                //Crear SkyBox 
                skyBox = new TgcSkyBox();
                skyBox.Center = new Vector3(0, 0, 0);
                skyBox.Size = new Vector3(3000, 3000, 3000);
                
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
            //velocidad de aceleracion de la nave
            GuiController.Instance.Modifiers.addFloat("currAccel", -15f, 15f, 0f);
            
            //lo que va incrementando la aceleracion
            GuiController.Instance.Modifiers.addFloat("speedModifier", 0f, 2f, 0.3f);

            //Para la Rotacion de la Caja a los costados (Float)
            GuiController.Instance.Modifiers.addFloat("rotationY", 0f, 2f, 0.2f);

            //Para la Rotacion de la Caja arriba y abajo (Float)
            GuiController.Instance.Modifiers.addFloat("rotationX", 0f, 10f, 1f);

            //Para la Rotacion de la Caja como barrelRoll (Float)
            GuiController.Instance.Modifiers.addFloat("rotationZ", 0f, 5f, 0.5f);

            //Para la rapidez en la cual vuelve a la rotacion original (Float)
            GuiController.Instance.Modifiers.addFloat("angRetorno", 0f, 2f, 0.8f);


            //AGREGO LA CAMARA QUE LA SIGUE
            //Habilito la camara en 1era Persona
            GuiController.Instance.ThirdPersonCamera.Enable = true;
            //Configurar a quien sigue y a que distancia Altura y Lejania
            GuiController.Instance.ThirdPersonCamera.setCamera(spaceShip.Position, 100, 200);

            //seteo valor inicial de las variables del movimiento
            currentAccel = 0f;
            maxSpeed = -2f; //valor temporal
            AngleZRotation = 0f;
        }


        
        // Método que se llama cada vez que hay que refrescar la pantalla.
        // Escribir aquí todo el código referido al renderizado.
        public override void render(float elapsedTime)
        {
            //Device de DirectX para renderizar
            Device d3dDevice = GuiController.Instance.D3dDevice;

            //Obtener valores de Modifiers
            float currAccel = (float)GuiController.Instance.Modifiers["currAccel"];
            float speed = (float)GuiController.Instance.Modifiers["speedModifier"];
            float rotationY = (float)GuiController.Instance.Modifiers["rotationY"];
            float rotationX = (float)GuiController.Instance.Modifiers["rotationX"];
            float rotationZ = (float)GuiController.Instance.Modifiers["rotationZ"];
            float angRetorno = (float)GuiController.Instance.Modifiers["angRetorno"];


            ///////////////INPUT TECLADO//////////////////
            //Tecla W apretada
            if (GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.W))
            {
                currentAccel -= speed * elapsedTime;
               
                    //Para hacer que vuelva a la posicion original a medida que acelera
                    if (!GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.D) && AngleZRotation > 0)
                    {
                        spaceShip.rotateZ(-rotationZ * angRetorno * elapsedTime); //rota barrelRoll en Z hacia la izq
                        AngleZRotation -= (rotationZ * angRetorno * elapsedTime);
                    }
                    if (!GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.A) && AngleZRotation < 0)
                    {
                        spaceShip.rotateZ(rotationZ * angRetorno * elapsedTime); //rota barrelRoll en Z hacia la der
                        AngleZRotation += (rotationZ * angRetorno * elapsedTime);
                    }

                if (currentAccel < maxSpeed) { currentAccel = maxSpeed; } //limito la velocidad maxima
            }

                //la desacelero lentamente si no se le da aceleración
                if (!GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.W))
                {
                    currentAccel += 0.5f * speed * elapsedTime;

                    if (currentAccel > 0) { currentAccel = 0; } //es desaceleracion, no la reversa
                }
                if (!GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.D) && AngleZRotation > 0)
                {
                    spaceShip.rotateZ(-rotationZ * 0.2f * elapsedTime); //rota barrelRoll en Z hacia la izq
                    AngleZRotation -= (rotationZ * 0.2f * elapsedTime);
                }
                if (!GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.A) && AngleZRotation < 0)
                {
                    spaceShip.rotateZ(rotationZ * 0.2f * elapsedTime); //rota barrelRoll en Z hacia la der
                    AngleZRotation += (rotationZ * 0.2f * elapsedTime);
                }

            //Tecla S apretada (Freno)
            if (GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.S))
            {
                currentAccel += 2 * speed * elapsedTime;
                
                if (currentAccel > 0) { currentAccel = 0; } //es desaceleracion, no la reversa
            }

            //actualizo el display de la aceleracion
            currAccel = currentAccel;

            //muevo la nave en base a la aceleracion y rotacion que se le da
            spaceShip.moveOrientedY(currentAccel);
            spaceShip.rotateY(AngleZRotation * elapsedTime);
            GuiController.Instance.ThirdPersonCamera.rotateY(AngleZRotation * elapsedTime);


            //Tecla D apretada
            if (GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.D))
            {
                if (AngleZRotation < 1.2)
                {
                    spaceShip.rotateZ(rotationZ * elapsedTime); //rota barrelRoll en Z hacia la der
                    AngleZRotation += (rotationZ * elapsedTime);
                }
            }

            //Tecla A apretada
            if (GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.A))
            {
                if (AngleZRotation > -1.2)
                {
                    spaceShip.rotateZ(-rotationZ * elapsedTime); //rota barrelRoll en Z hacia la izq
                    AngleZRotation -= (rotationZ * elapsedTime);
                }
            }

            //Tecla Up apretada
            if (GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.Up))
            {
                spaceShip.rotateX(rotationX * elapsedTime); //rota hacia la arriba en X
            }

            //Tecla Down apretada
            if (GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.Down))
            {
                spaceShip.rotateX(-rotationX * elapsedTime); //rota hacia la abajo en X
            }

            //Tecla Right apretada
            if (GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.Right))
            {
                spaceShip.rotateY(rotationY * elapsedTime); //rota hacia la izq en Y
                GuiController.Instance.ThirdPersonCamera.rotateY(rotationY * elapsedTime);
            }

            //Tecla Left apretada
            if (GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.Left))
            {
                spaceShip.rotateY(-rotationY * elapsedTime); //rota hacia la der en Y
                GuiController.Instance.ThirdPersonCamera.rotateY(-rotationY * elapsedTime);
            }


            //Hacer que la cámara en 3ra persona se ajuste a la nueva posición del objeto
            GuiController.Instance.ThirdPersonCamera.Target = spaceShip.Position;

            //RENDER
            //Siempre primero hacer todos los cálculos de lógica e input y luego al final dibujar todo (ciclo update-render)
            skyBox.render();
            box.render();
            spaceShip.render();
            text1.render();
        }



        // Método que se llama cuando termina la ejecución del ejemplo.
        // Hacer dispose() de todos los objetos creados.
        public override void close()
        {
            skyBox.dispose();
            box.dispose();
            spaceShip.dispose();
            text1.dispose();
        }
    }
}
