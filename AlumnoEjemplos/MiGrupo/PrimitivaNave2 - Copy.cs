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

         readonly Vector3 SUN_SCALE = new Vector3(12, 12, 12);
        readonly Vector3 EARTH_SCALE = new Vector3(3, 3, 3);
        readonly Vector3 MOON_SCALE = new Vector3(0.5f, 0.5f, 0.5f);

        const float AXIS_ROTATION_SPEED = 0.1f;
        const float EARTH_AXIS_ROTATION_SPEED = 1f;
        const float EARTH_ORBIT_SPEED = 0.2f;
        const float MOON_ORBIT_SPEED = 1f;

        const float EARTH_ORBIT_OFFSET = 700;
        const float MOON_ORBIT_OFFSET = 80;

        TgcMesh sun;
        TgcMesh earth;
        TgcMesh moon;

        float axisRotation = 0f;
        float earthAxisRotation = 0f;
        float earthOrbitRotation = 0f;
        float moonOrbitRotation = 0f;



        TgcBox box; //caja
        TgcMesh spaceShip; //nave
        TgcSkyBox skyBox;  //cieloEnvolvente
        TgcText2d text1;    //textoExplicacion
        float currentAccel;
        float maxSpeed;
        float AngleZRotation;
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
            TgcScene scene = loader.loadSceneFromFile(GuiController.Instance.AlumnoEjemplosMediaDir + "NaveStarWars\\NaveStarWars-TgcScene.xml");

            string sphere = GuiController.Instance.ExamplesMediaDir + "ModelosTgc\\Sphere\\Sphere-TgcScene.xml";
            sun = loader.loadSceneFromFile(sphere).Meshes[0];
            sun.changeDiffuseMaps(new TgcTexture[] { TgcTexture.createTexture(d3dDevice, GuiController.Instance.ExamplesDir + "Transformations\\SistemaSolar\\SunTexture.jpg") });

            earth = loader.loadSceneFromFile(sphere).Meshes[0];
            earth.changeDiffuseMaps(new TgcTexture[] { TgcTexture.createTexture(d3dDevice, GuiController.Instance.ExamplesDir + "Transformations\\SistemaSolar\\EarthTexture.jpg") });

            moon = loader.loadSceneFromFile(sphere).Meshes[0];
            moon.changeDiffuseMaps(new TgcTexture[] { TgcTexture.createTexture(d3dDevice, GuiController.Instance.ExamplesDir + "Transformations\\SistemaSolar\\MoonTexture.jpg") });


            //Deshabilitamos el manejo automático de Transformaciones de TgcMesh, para poder manipularlas en forma customizada
            sun.AutoTransformEnable = false;
            earth.AutoTransformEnable = false;
            moon.AutoTransformEnable = false;


            //Color de fondo
            GuiController.Instance.BackgroundColor = Color.Black;






            //TEXTO de explicacion
            text1 = new TgcText2d();
            text1.Text = "Aceleracion: W, Freno: S                     RotarHorizontal: A y D                              Subir: Shift, Bajar: Ctrl";
            text1.Align = TgcText2d.TextAlign.RIGHT;
            text1.Position = new Point(50, 50);
            text1.Size = new Size(300, 100);
            text1.Color = Color.Gold;
                
                //SKYBOX//
                //Textura del Cielo
              //  string texturesPath = GuiController.Instance.ExamplesMediaDir + "Texturas\\Quake\\SkyBox2\\";

                //Crear SkyBox 
             //   skyBox = new TgcSkyBox();
            //    skyBox.Center = new Vector3(0, 0, 0);
            //    skyBox.Size = new Vector3(3000, 3000, 3000);
                
                //Configurar las texturas para cada una de las 6 caras
            //    skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Up, texturesPath + "lun4_up.jpg");
            //    skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Down, texturesPath + "lun4_dn.jpg");
            //    skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Left, texturesPath + "lun4_lf.jpg");
            //    skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Right, texturesPath + "lun4_rt.jpg");
            //    skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Front, texturesPath + "lun4_bk.jpg");
            //    skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Back, texturesPath + "lun4_ft.jpg");

            //    skyBox.updateValues();

                
                
            //AGREGO UNA CAJA de METAL
            Vector3 center = new Vector3(0, 0, 0);
            Vector3 size = new Vector3(5, 5, 5);
            TgcTexture texture = TgcTexture.createTexture(GuiController.Instance.ExamplesMediaDir + "MeshCreator\\Textures\\Metal\\cajaMetal.jpg");
            box = TgcBox.fromSize(center, size, texture);

            //AGREGO UNA NAVE
            spaceShip = scene.Meshes[0];
            Vector3 escala=new Vector3(0.5f, 0.5f, 0.5f);
            spaceShip.Scale=(escala);
            

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
            GuiController.Instance.Modifiers.addFloat("rotationZ", 0f, 5f, 1.5f);

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
            anguloSubida = 0f;
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

                //DESACELERACION y vuelta a la posicion original (horizonte)
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
                if (!GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.LeftShift) && anguloSubida > 0)
                {
                    spaceShip.rotateX(-rotationX * elapsedTime); //rota la trompa hacia abajo
                    anguloSubida -= (rotationX * elapsedTime);
                }
                if (!GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.LeftControl) && anguloSubida < 0)
                {
                    spaceShip.rotateX(rotationX * elapsedTime); //rota la trompa hacia arriba
                    anguloSubida += rotationX * elapsedTime;
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
                    spaceShip.rotateZ(rotationZ * (-currAccel/2) * elapsedTime); //rota barrelRoll en Z hacia la der
                    AngleZRotation += (rotationZ * (-currAccel / 2) * elapsedTime);
                }
            }

            //Tecla A apretada
            if (GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.A))
            {
                if (AngleZRotation > -1.2)
                {
                    spaceShip.rotateZ(-rotationZ * (-currAccel / 2) * elapsedTime); //rota barrelRoll en Z hacia la izq
                    AngleZRotation -= (rotationZ * (-currAccel / 2) * elapsedTime);
                }
            }

            //Shift, quiero subir
            if (GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.LeftShift))
            {
                spaceShip.move(0, currAccel * -1f, 0); //multiplico por la dir para que sea Arriba/Abajo
                Subiendo(1, currAccel*rotationX, elapsedTime);
            }
            
            //Control, quiero bajar
            else if (GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.LeftControl)) 
            {
                spaceShip.move(0, currAccel, 0); //multiplico por la dir para que sea Arriba/Abajo
                Subiendo(-1, currAccel*rotationX, elapsedTime);
            }

            /*
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
            */

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

             //Actualizar transformacion y renderizar el sol
            sun.Transform = getSunTransform(elapsedTime);
            sun.render();

            //Actualizar transformacion y renderizar la tierra
            earth.Transform = getEarthTransform(elapsedTime);
            earth.render();

            //Actualizar transformacion y renderizar la luna
            moon.Transform = getMoonTransform(elapsedTime, earth.Transform);
            moon.render();

            axisRotation += AXIS_ROTATION_SPEED * elapsedTime;
            earthAxisRotation += EARTH_AXIS_ROTATION_SPEED * elapsedTime;
            earthOrbitRotation += EARTH_ORBIT_SPEED * elapsedTime;
            moonOrbitRotation += MOON_ORBIT_SPEED * elapsedTime;

            //Limpiamos todas las transformaciones con la Matrix identidad
            d3dDevice.Transform.World = Matrix.Identity;
       
            //RENDER
            //Siempre primero hacer todos los cálculos de lógica e input y luego al final dibujar todo (ciclo update-render)
            //skyBox.render();
            box.render();
            spaceShip.render();
            text1.render();
        }



        private Matrix getSunTransform(float elapsedTime)
        {
            Matrix scale = Matrix.Scaling(SUN_SCALE);
            Matrix yRot = Matrix.RotationY(axisRotation);

            return scale * yRot;
        }

        private Matrix getEarthTransform(float elapsedTime)
        {
            Matrix scale = Matrix.Scaling(EARTH_SCALE);
            Matrix yRot = Matrix.RotationY(earthAxisRotation);
            Matrix sunOffset = Matrix.Translation(EARTH_ORBIT_OFFSET, 0, 0);
            Matrix earthOrbit = Matrix.RotationY(earthOrbitRotation);

            return scale * yRot * sunOffset * earthOrbit;
        }

        private Matrix getMoonTransform(float elapsedTime, Matrix earthTransform)
        {
            Matrix scale = Matrix.Scaling(MOON_SCALE);
            Matrix yRot = Matrix.RotationY(axisRotation);
            Matrix earthOffset = Matrix.Translation(MOON_ORBIT_OFFSET, 0, 0);
            Matrix moonOrbit = Matrix.RotationY(moonOrbitRotation);

            return scale * yRot * earthOffset * moonOrbit * earthTransform;
        }




        // Método que se llama cuando termina la ejecución del ejemplo.
        // Hacer dispose() de todos los objetos creados.
        public override void close()
        {
            skyBox.dispose();
            box.dispose();
            spaceShip.dispose();
            text1.dispose();
            sun.dispose();
            moon.dispose();
            earth.dispose();
        }

        public void Subiendo(int unaDir, float aceleracion, float deltaTime)
        {
            if (anguloSubida < Geometry.DegreeToRadian(30) && anguloSubida > Geometry.DegreeToRadian(-30)) 
            {
                anguloSubida += (-aceleracion/2) * unaDir * deltaTime; 
                spaceShip.rotateX((-aceleracion/2) * unaDir * deltaTime); 
            }
        }
    }
}