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
using TgcViewer.Utils;
using AlumnoEjemplos.MiGrupo;


namespace AlumnoEjemplos.NaveEspacial
{
    public class NaveEspacial : TgcExample
    {

        readonly Vector3 SUN_SCALE = new Vector3(30, 30, 30);
        readonly Vector3 VENUS_SCALE = new Vector3(6, 6, 6);
        readonly Vector3 EARTH_SCALE = new Vector3(8, 8, 8);
        readonly Vector3 MOON_SCALE = new Vector3(0.4f, 0.4f, 0.4f);
        readonly Vector3 JUPITER_SCALE = new Vector3(12, 12, 12);
        readonly Vector3 NEPTUNE_SCALE = new Vector3(9, 9, 9);
        readonly Vector3 SPACE_SCALE = new Vector3(200, 200, 200);

        const float AXIS_ROTATION_SPEED = 0.1f;
        const float EARTH_AXIS_ROTATION_SPEED = 1f;
        const float VENUS_ORBIT_SPEED = 0.15f;
        const float EARTH_ORBIT_SPEED = 0.2f;
        const float MOON_ORBIT_SPEED = 1f;
        const float JUPITER_ORBIT_SPEED = 0.08f;
        const float NEPTUNE_ORBIT_SPEED = 0.05f;

        const float VENUS_ORBIT_OFFSET = 1200;
        const float EARTH_ORBIT_OFFSET = 1800;
        const float MOON_ORBIT_OFFSET = 70;
        const float JUPITER_ORBIT_OFFSET = 3000;
        const float NEPTUNE_ORBIT_OFFSET = 4000;
        
        float axisRotation = 0f;
        float earthAxisRotation = 0f;
        float venusOrbitRotation = 0f;
        float earthOrbitRotation = 0f;
        float moonOrbitRotation = 0f;
        float jupiterOrbitRotation = 0f;
        float neptuneOrbitRotation = 0f;

        TgcMesh sun;
        TgcMesh venus;
        TgcMesh earth;
        TgcMesh moon;
        TgcMesh jupiter;
        TgcMesh neptune;

        TgcMesh spaceSphere;
        public Size screenSize;

        //shooting related stuff
        List<Bullet> Disparos;
        List<Bullet> DisparosEnemy;
        float timeSinceLastShot;
        float timeSinceLastEnemyShot;
        float largoBala = 4f;
        


        TgcMesh spaceShip; //nave
        TgcObb obbSpaceShip; //para la colision de la nave
        List<TgcBoundingBox> Colisionables = new List<TgcBoundingBox>();

        TgcMesh naveEnemiga; //nave enemiga


        TgcText2d text1;    //textoExplicacion
        TgcSprite spriteLife;   //barra de vida
        float vidaTotal = 100f;
        float vidaNave;
        float vidaEnemigoTotal = 50f;
        float vidaEnemigo;
        TgcSprite spriteNitro; //barra de nitro
        float nitroNaveTotal = 600f;
        float nitroNave;
        float currentSpeed;
        float maxSpeed = 0.8f;
        float AngleZRotation;
        float anguloSubida;


        List<Star> gameStars;
        const int starCount = 50; //La cantidad maxima de estrellas simultaneas.
        
        List<Asteroids> gameAsteroids;
        const int asteroidCount = 5; //La cantidad maxima de asteroides simultaneos.

        BlurEffect effectBlur;
        List<TgcMesh> blurredMeshes;



        public override string getCategory()
        {
            return "AlumnoEjemplos";
        }

        public override string getName()
        {
            return "Grupo 04";
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

            //inicio la lista
            Disparos = new List<Bullet>();
            DisparosEnemy = new List<Bullet>();
            blurredMeshes = new List<TgcMesh>();

            //Cargo el loader de Scenes y los Meshes
            TgcSceneLoader loader = new TgcSceneLoader();
            TgcScene scene = loader.loadSceneFromFile(GuiController.Instance.AlumnoEjemplosMediaDir + "NaveStarWars\\NaveStarWars-TgcScene.xml");
            //TgcScene scene = loader.loadSceneFromFile(GuiController.Instance.AlumnoEjemplosMediaDir + "dark_fighter\\dark_fighter-TgcScene.xml");
            TgcScene sceneEnemigo = loader.loadSceneFromFile(GuiController.Instance.AlumnoEjemplosMediaDir + "xWing\\xWing-TgcScene.xml");

            string sphere = GuiController.Instance.ExamplesMediaDir + "ModelosTgc\\Sphere\\Sphere-TgcScene.xml";
            sun = loader.loadSceneFromFile(sphere).Meshes[0];
            sun.changeDiffuseMaps(new TgcTexture[] { TgcTexture.createTexture(d3dDevice, GuiController.Instance.AlumnoEjemplosMediaDir + "Texturas\\SunTexture2.jpg") });
            blurredMeshes.Add(sun);

            venus = loader.loadSceneFromFile(sphere).Meshes[0];
            venus.changeDiffuseMaps(new TgcTexture[] { TgcTexture.createTexture(d3dDevice, GuiController.Instance.AlumnoEjemplosMediaDir + "Texturas\\VenusTexture.png") });
            blurredMeshes.Add(venus);

            earth = loader.loadSceneFromFile(sphere).Meshes[0];
            earth.changeDiffuseMaps(new TgcTexture[] { TgcTexture.createTexture(d3dDevice, GuiController.Instance.AlumnoEjemplosMediaDir + "Texturas\\EarthTexture.png") });
            blurredMeshes.Add(earth);

            moon = loader.loadSceneFromFile(sphere).Meshes[0];
            moon.changeDiffuseMaps(new TgcTexture[] { TgcTexture.createTexture(d3dDevice, GuiController.Instance.AlumnoEjemplosMediaDir + "Texturas\\MoonTexture.png") });
            blurredMeshes.Add(moon);

            jupiter = loader.loadSceneFromFile(sphere).Meshes[0];
            jupiter.changeDiffuseMaps(new TgcTexture[] { TgcTexture.createTexture(d3dDevice, GuiController.Instance.AlumnoEjemplosMediaDir + "Texturas\\JupiterTexture.png") });
            blurredMeshes.Add(jupiter);

            neptune = loader.loadSceneFromFile(sphere).Meshes[0];
            neptune.changeDiffuseMaps(new TgcTexture[] { TgcTexture.createTexture(d3dDevice, GuiController.Instance.AlumnoEjemplosMediaDir + "Texturas\\NeptuneTexture.png") });
            blurredMeshes.Add(neptune);

            spaceSphere = loader.loadSceneFromFile(sphere).Meshes[0];
            spaceSphere.changeDiffuseMaps(new TgcTexture[] { TgcTexture.createTexture(d3dDevice, GuiController.Instance.AlumnoEjemplosMediaDir + "Texturas\\spaceBackTexture.jpg") });
            spaceSphere.Scale = SPACE_SCALE;
            //blurredMeshes.Add(spaceSphere);


            //Deshabilitamos el manejo automático de Transformaciones de TgcMesh, para poder manipularlas en forma customizada
            sun.AutoTransformEnable = false;
            venus.AutoTransformEnable = false;
            earth.AutoTransformEnable = false;
            moon.AutoTransformEnable = false;
            jupiter.AutoTransformEnable = false;
            neptune.AutoTransformEnable = false;


            //Color de fondo
            GuiController.Instance.BackgroundColor = Color.Black;


            //seteo valor inicial de las variables
            currentSpeed = 0f;      //inicia frenado
            AngleZRotation = 0f;    //para la rotacion de la nave (izq y der) inicia derecho
            anguloSubida = 0f;      //para la rotacion de la nave (arriba y abajo) inicia derecho

            timeSinceLastShot = 10f;
            timeSinceLastEnemyShot = 10f;

            vidaNave = 100f;
            vidaEnemigo = 50f;
            nitroNave = 600f;


            //Crear Sprites
            spriteLife = new TgcSprite();
            spriteLife.Texture = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosMediaDir + "\\Texturas\\LifeBar.png");
            spriteLife.Scaling = new Vector2(0.3f, -0.5f);
            spriteNitro = new TgcSprite();
            spriteNitro.Texture = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosMediaDir + "\\Texturas\\NitroBar.png");
            spriteNitro.Scaling = new Vector2(0.1f, -0.3f);
            //Ubicarlos en pantalla
            screenSize = GuiController.Instance.Panel3d.Size;
            spriteLife.Position = new Vector2(FastMath.Max(screenSize.Width / 11 - spriteLife.Texture.Size.Width, 0), FastMath.Max(screenSize.Height - screenSize.Height / 18, 0));
            spriteNitro.Position = new Vector2(FastMath.Max(screenSize.Width / 12 - spriteNitro.Texture.Size.Width, 0), FastMath.Max(screenSize.Height - screenSize.Height / 18, 0));


           
            //Creo la lista de Estrellas en HyperSpeed.
            gameStars = new List<Star>();
            for (int numberStar = 0; numberStar < starCount; numberStar++)
            {
                Star actualStar = new Star();
                actualStar.Load();
                gameStars.Add(actualStar);
            } 
            
            //Creo la lista de Asteroides.
            gameAsteroids = new List<Asteroids>();
            for (int numberAsteroid = 0; numberAsteroid < asteroidCount; numberAsteroid++)
            {
                Asteroids actualAsteroid;

                actualAsteroid = new Asteroids();
                actualAsteroid.Load();
                gameAsteroids.Add(actualAsteroid);
            }

            //TEXTO de explicacion
            text1 = new TgcText2d();
            text1.Text = "Aceleracion: W, Freno: S                       RotarHorizontal: A y D                         Subir: Shift, Bajar: Ctrl                         HiperVelocidad: Space                             Disparos: ClickIzq";
            text1.Align = TgcText2d.TextAlign.RIGHT;
            text1.Position = new Point(50, 50);
            text1.Size = new Size(300, 100);
            text1.Color = Color.Gold;



            //AGREGO UNA NAVE
            spaceShip = scene.Meshes[0];
            Vector3 escala = new Vector3(0.05f, 0.05f, 0.05f); //en lugar de bajar la escala a todo, bajo a la nave y evito problemas de numeros gigantes
            spaceShip.Scale = (escala);
            obbSpaceShip = TgcObb.computeFromAABB(spaceShip.BoundingBox);
            spaceShip.Position = new Vector3(700, 0, 300); //pos inicial
            //blurredMeshes.Add(spaceShip);

            //AGREGO NAVE ENEMIGA
            naveEnemiga = sceneEnemigo.Meshes[0];
            naveEnemiga.Scale = (escala * 5);
            naveEnemiga.Position = new Vector3(800, 0, 200); //una pos inicial
            //naveEnemiga.AutoTransformEnable = false;
            //blurredMeshes.Add(naveEnemiga);


            ///////////////MODIFIERS//////////////////
            //lo que va incrementando la aceleracion
            GuiController.Instance.Modifiers.addFloat("accel", 0f, 2f, 0.2f);

            //lo que va incrementando la aceleracion
            GuiController.Instance.Modifiers.addFloat("breaks", 0f, 5f, 0.5f);

            //Para la Rotacion de la Caja a los costados (Float)
            GuiController.Instance.Modifiers.addFloat("rotationY", 0f, 2f, 0.2f);

            //Para la Rotacion de la Caja arriba y abajo (Float)
            GuiController.Instance.Modifiers.addFloat("rotationX", 0f, 10f, 2f);

            //Para la Rotacion de la Caja como barrelRoll (Float)
            GuiController.Instance.Modifiers.addFloat("rotationZ", 0f, 5f, 1.5f);

            //Para la rapidez en la cual vuelve a la rotacion original (Float)
            GuiController.Instance.Modifiers.addFloat("angRetorno", 0f, 2f, 0.8f);

            //hipervelocidad de la nave
            GuiController.Instance.Modifiers.addFloat("hyperSpeed", 1f, 5f, 4f);

            //distancia del follow enemigo
            GuiController.Instance.Modifiers.addFloat("distEnemigo", 10f, 99f, 20f);

            //Modifier para BoundingBox
            GuiController.Instance.Modifiers.addBoolean("BoundingBox", "BoundingBox:", false);

            //Modifier para MotionBlur
            GuiController.Instance.Modifiers.addBoolean("MotionBlur", "MotionBlur:", false);


            //AGREGO LA CAMARA QUE LA SIGUE
            //Habilito la camara en 1era Persona
            GuiController.Instance.ThirdPersonCamera.Enable = true;
            //Configurar a quien sigue y a que distancia Altura y Lejania
            GuiController.Instance.ThirdPersonCamera.setCamera(spaceShip.Position, 5, 30);


            //Para colisiones
            Colisionables.Clear();
            Colisionables.Add(sun.BoundingBox);
            Colisionables.Add(venus.BoundingBox);
            Colisionables.Add(earth.BoundingBox);
            Colisionables.Add(moon.BoundingBox);
            Colisionables.Add(jupiter.BoundingBox);
            Colisionables.Add(neptune.BoundingBox);
            

            //activo el Blur
            effectBlur = new BlurEffect();
            effectBlur.Load(blurredMeshes);
        }



        // Método que se llama cada vez que hay que refrescar la pantalla.
        // Escribir aquí todo el código referido al renderizado.
        public override void render(float elapsedTime)
        {
            //Device de DirectX para renderizar
            Device d3dDevice = GuiController.Instance.D3dDevice;
            GuiController.Instance.Text3d.drawText("FPS: " + HighResolutionTimer.Instance.FramesPerSecond, 0, 0, Color.Yellow);

            //Obtener valores de Modifiers
            float accel = (float)GuiController.Instance.Modifiers["accel"];
            float breaks = (float)GuiController.Instance.Modifiers["breaks"];
            float rotationY = (float)GuiController.Instance.Modifiers["rotationY"];
            float rotationX = (float)GuiController.Instance.Modifiers["rotationX"];
            float rotationZ = (float)GuiController.Instance.Modifiers["rotationZ"];
            float angRetorno = (float)GuiController.Instance.Modifiers["angRetorno"];
            float hyperSpeed = (float)GuiController.Instance.Modifiers["hyperSpeed"];
            float distEnemigo = (float)GuiController.Instance.Modifiers["distEnemigo"];
            bool showBB = (bool)GuiController.Instance.Modifiers["BoundingBox"];
            bool shaderMB = (bool)GuiController.Instance.Modifiers["MotionBlur"];

            spaceSphere.render(); //la rendereo primero porque es el fondo

            //cada frame voy actualizando el tiempo entre disparos
            timeSinceLastShot += elapsedTime;
            timeSinceLastEnemyShot += elapsedTime;

            ///////////////INPUT TECLADO//////////////////

            //Boton Izq, disparo
            if (GuiController.Instance.D3dInput.buttonDown(TgcViewer.Utils.Input.TgcD3dInput.MouseButtons.BUTTON_LEFT) && timeSinceLastShot > 0.5f) //reviso tecla y el intervalo de disparo
            {
                Bullet disparoTemp = crearBalaPara(spaceShip);
                Disparos.Add(disparoTemp);
                timeSinceLastShot = 0f;
            }

            for (int i = 0; i < Disparos.Count; i += 1)
            {
                Disparos[i].renderModel.moveOrientedY(-1f);
                Disparos[i].renderModel.render();
                Disparos[i].incrementarTiempo(elapsedTime);

                if (Disparos[i].getDone()) //si la bala hace X segundos que esta en el juego, ya viajo lejos y no me interesa, la destruyo
                {
                    Disparos[i].renderModel.dispose();
                    Disparos.Remove(Disparos[i]);
                }
            }

            //Tecla W apretada (si además tiene Space, va a hiperVelocidad)
            if (GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.W))
            {
                //esto hace que si esta llendo en reversa, baje la velocidad a 0 mucho mas rapido, y luego acelere normal, y si se presiona la BarraEspaciadora acelere más rapido
                if (currentSpeed < 0)
                { currentSpeed += 5 * breaks * accel * elapsedTime; }
                else if (currentSpeed >= 0 && !GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.Space))
                { currentSpeed += accel * elapsedTime; }
                else if (currentSpeed >= 0 && GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.Space))
                { 
                    currentSpeed += hyperSpeed * accel * elapsedTime;
                    reducirNitro(spaceShip, elapsedTime);
                }

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

                //limito la velocidad maxima
                if (currentSpeed > maxSpeed && !GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.Space))
                { currentSpeed = maxSpeed; }
                else if (currentSpeed > (maxSpeed * hyperSpeed) && GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.Space))
                { currentSpeed = maxSpeed * hyperSpeed; }

            }

            //DESACELERACION y vuelta a la posicion original (horizonte)//
            if (!GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.W) && !GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.S))
            {
                if (currentSpeed > 0) { currentSpeed -= breaks * accel * elapsedTime; }
                else { currentSpeed += 2 * breaks * accel * elapsedTime; } //esto hace que si no esta acelerando baje la velocidad a 0 (para reversa es mas rapido)
            }
            if (!GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.D) && AngleZRotation > 0)
            {
                spaceShip.rotateZ(-rotationZ * (angRetorno / 2) * elapsedTime); //rota barrelRoll en Z hacia la izq
                AngleZRotation -= (rotationZ * (angRetorno / 2) * elapsedTime);
            }
            if (!GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.A) && AngleZRotation < 0)
            {
                spaceShip.rotateZ(rotationZ * (angRetorno/2) * elapsedTime); //rota barrelRoll en Z hacia la der
                AngleZRotation += (rotationZ * (angRetorno / 2) * elapsedTime);
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
                if (currentSpeed > 0) { currentSpeed -= breaks * elapsedTime; }
                else { currentSpeed -= accel/2 * elapsedTime; }   //esto hace que frene rapido hasta 0 y luego acelere a medias en reversa

                if (currentSpeed < -maxSpeed / 2) { currentSpeed = -maxSpeed / 2; } //limito la velocidad maxima negativa
            }

            //Tecla D apretada
            if (GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.D))
            {
                if (currentSpeed >= 0 && AngleZRotation < 1.2)
                {
                    spaceShip.rotateZ(rotationZ * (currentSpeed / 2) * elapsedTime); //rota barrelRoll en Z hacia la der
                    AngleZRotation += (rotationZ * (currentSpeed / 2) * elapsedTime);
                }
            }

            //Tecla A apretada
            if (GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.A))
            {
                if (currentSpeed >= 0 && AngleZRotation > -1.2)
                {
                    spaceShip.rotateZ(-rotationZ * (currentSpeed / 2) * elapsedTime); //rota barrelRoll en Z hacia la izq
                    AngleZRotation -= (rotationZ * (currentSpeed / 2) * elapsedTime);
                }
            }

            //Shift, quiero subir
            if (GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.LeftShift))
            {
                if (currentSpeed >= 0)
                {
                    spaceShip.move(0, currentSpeed / 2, 0);
                    Subiendo(1, currentSpeed * rotationX, elapsedTime); //la direccion de la subida es negativa, la velocidad depende de la rotacion en X
                }
            }

            //Control, quiero bajar
            else if (GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.LeftControl))
            {
                if (currentSpeed >= 0)
                {
                    spaceShip.move(0, -currentSpeed / 2, 0);
                    Subiendo(-1, currentSpeed * rotationX, elapsedTime); //la direccion de la subida es negativa, la velocidad depende de la rotacion en X
                }
            }

            //Tecla E apretada
            if (GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.E))
            {
                spaceShip.rotateY(rotationY * elapsedTime); //rota hacia la izq en Y (sin inclinarse, rota aunque este detenido o marcha atras)
                GuiController.Instance.ThirdPersonCamera.rotateY(rotationY * elapsedTime);
            }

            //Tecla Q apretada
            if (GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.Q))
            {
                spaceShip.rotateY(-rotationY * elapsedTime); //rota hacia la der en Y (sin inclinarse, rota aunque este detenido o marcha atras)
                GuiController.Instance.ThirdPersonCamera.rotateY(-rotationY * elapsedTime);
            }


            //muevo la nave en base a la aceleracion y rotacion que se le da, previamente guardando la pos anterior
            Vector3 lastPos = spaceShip.Position;

            spaceShip.moveOrientedY(-currentSpeed);
            spaceShip.rotateY(AngleZRotation * elapsedTime);
            GuiController.Instance.ThirdPersonCamera.rotateY(AngleZRotation * elapsedTime);

            //logica AI
            if (vidaEnemigo > 0)
            {
                Vector3 targetDeEnemigo = Vector3.Subtract(spaceShip.Position, naveEnemiga.Position);
                float enemySpeed = maxSpeed * 0.9f; //que sea un poco mas lenta
                if (DistanceAtoB(spaceShip.Position, naveEnemiga.Position) < 200) //si me encuentro a X unidades de distancia, me detecta y se activa
                {
                    if (DistanceAtoB(spaceShip.Position, naveEnemiga.Position) < 80 && timeSinceLastEnemyShot > 1f) //rango de Disparo
                    {
                        Bullet disparoTempE = crearBalaPara(naveEnemiga);
                        DisparosEnemy.Add(disparoTempE);
                        timeSinceLastEnemyShot = 0f;
                    }

                    if (DistanceAtoB(spaceShip.Position, naveEnemiga.Position) > 50) //muy cerca la freno pero sino que me siga
                    {
                        naveEnemiga.move(Vector3.Normalize(targetDeEnemigo) * maxSpeed);
                    }

                    naveEnemiga.Rotation = LookAt(naveEnemiga.Position, spaceShip.Position); //rotacion donde la nave "me mira"
                }

                for (int i = 0; i < DisparosEnemy.Count; i += 1) //for de balas enemigas
                {
                    DisparosEnemy[i].renderModel.moveOrientedY(-1f);
                    DisparosEnemy[i].renderModel.render();
                    DisparosEnemy[i].incrementarTiempo(elapsedTime);

                    if (DisparosEnemy[i].getDone()) //si la bala hace X segundos que esta en el juego, ya viajo lejos y no me interesa, la destruyo
                    {
                        DisparosEnemy[i].renderModel.dispose();
                        DisparosEnemy.Remove(DisparosEnemy[i]);
                    }
                }
            }


            //Hacer que la cámara en 3ra persona se ajuste a la nueva posición del objeto
            GuiController.Instance.ThirdPersonCamera.Target = spaceShip.Position;
            
            //actualizo el obb
            obbSpaceShip.move(spaceShip.Position - obbSpaceShip.Position);
            obbSpaceShip.setRotation(spaceShip.Rotation);
            if (showBB) { obbSpaceShip.render(); }

            TgcBox misil;
            //verifico disparo
            for (int i = 0; i < DisparosEnemy.Count; i += 1) //for de balas enemigas
            {
                misil = DisparosEnemy[i].renderModel;
                if (Math.Sqrt(Math.Pow(misil.Position.X - spaceShip.Position.X, 2.0) + Math.Pow(misil.Position.Y - spaceShip.Position.Y, 2.0)+ Math.Pow(misil.Position.Z - spaceShip.Position.Z, 2.0)) <= largoBala)
                    {
                        reducirVida(spaceShip);
                        DisparosEnemy[i].incrementarTiempo(5f);
                    }
            }

            for (int i = 0; i < Disparos.Count; i += 1) //for de balas enemigas
            {
                misil = Disparos[i].renderModel;
                if (Math.Sqrt(Math.Pow(misil.Position.X - naveEnemiga.Position.X, 2.0) + Math.Pow(misil.Position.Y - naveEnemiga.Position.Y, 2.0) + Math.Pow(misil.Position.Z - naveEnemiga.Position.Z, 2.0)) <= largoBala)
                {
                    vidaEnemigo -= 25f;
                    Disparos[i].incrementarTiempo(5f);
                }
            }




            text1.render();

            //Actualizar transformaciones y renderizar planetas

            //Sol
            sun.Transform = getPlanetTransform(SUN_SCALE, axisRotation, 0, 0, Matrix.Identity);
            sun.BoundingBox.transform(sun.Transform);
            sun.render();
            //Venus
            venus.Transform = getPlanetTransform(VENUS_SCALE, axisRotation, VENUS_ORBIT_OFFSET, venusOrbitRotation, Matrix.Identity);
            venus.BoundingBox.transform(venus.Transform);
            venus.render();
            //Tierra
            earth.Transform = getPlanetTransform(EARTH_SCALE, earthAxisRotation, EARTH_ORBIT_OFFSET, earthOrbitRotation, Matrix.Identity);
            earth.BoundingBox.transform(earth.Transform);
            earth.render();
            //Luna
            moon.Transform = getPlanetTransform(MOON_SCALE, axisRotation, MOON_ORBIT_OFFSET, moonOrbitRotation, earth.Transform);
            moon.BoundingBox.transform(moon.Transform);
            moon.render();
            //Jupiter
            jupiter.Transform = getPlanetTransform(JUPITER_SCALE, axisRotation, JUPITER_ORBIT_OFFSET, jupiterOrbitRotation, Matrix.Identity);
            jupiter.BoundingBox.transform(jupiter.Transform);
            jupiter.render();
            //Neptune
            neptune.Transform = getPlanetTransform(NEPTUNE_SCALE, axisRotation, NEPTUNE_ORBIT_OFFSET, neptuneOrbitRotation, Matrix.Identity);
            neptune.BoundingBox.transform(neptune.Transform);
            neptune.render();

            axisRotation += AXIS_ROTATION_SPEED * elapsedTime;
            venusOrbitRotation += VENUS_ORBIT_SPEED * elapsedTime;
            earthAxisRotation += EARTH_AXIS_ROTATION_SPEED * elapsedTime;
            earthOrbitRotation += EARTH_ORBIT_SPEED * elapsedTime;
            moonOrbitRotation += MOON_ORBIT_SPEED * elapsedTime;
            jupiterOrbitRotation += JUPITER_ORBIT_SPEED * elapsedTime;
            neptuneOrbitRotation += NEPTUNE_ORBIT_SPEED * elapsedTime;

            //checkeo colisiones
            foreach (TgcBoundingBox col in Colisionables)
            {
                if (TgcCollisionUtils.testObbAABB(obbSpaceShip, col))
                {
                    spaceShip.Position = lastPos; obbSpaceShip.move(spaceShip.Position - obbSpaceShip.Position);
                }
            }

            spaceSphere.Position = spaceShip.Position;


            //Limpiamos todas las transformaciones con la Matrix identidad
            d3dDevice.Transform.World = Matrix.Identity;

            //RENDER
            //Siempre primero hacer todos los cálculos de lógica e input y luego al final dibujar todo (ciclo update-render)
            spaceShip.render();

            if (vidaEnemigo > 0) { naveEnemiga.render(); 
                                 //naveEnemiga.BoundingBox.render();
                                 }
            if (vidaEnemigo < 1) {
                                 //naveEnemiga.dispose();
                                 }



            //BoundingBox
            if (showBB)
            {
                sun.BoundingBox.render();
                venus.BoundingBox.render();
                moon.BoundingBox.render();
                earth.BoundingBox.render();
                jupiter.BoundingBox.render();
                neptune.BoundingBox.render();
                naveEnemiga.BoundingBox.render();
            }

            //EfectoShader
            if (shaderMB)
            {
                if (GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.W)
                    && GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.Space)
                    && !GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.S))
                {
                    effectBlur.Render(elapsedTime);
                    spaceShip.render();
                }
            }

            //Iniciar dibujado de todos los Sprites de la escena
            GuiController.Instance.Drawer2D.beginDrawSprite();
            //Dibujar sprites
            spriteLife.render();
            spriteNitro.render();
            if(     GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.W) 
                &&  GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.Space)
                && !GuiController.Instance.D3dInput.keyDown(Microsoft.DirectX.DirectInput.Key.S))
            {
                foreach(Star actualStar in gameStars)
                {
                    actualStar.Update(elapsedTime);
                    actualStar.Render();
                }
            }
            GuiController.Instance.Drawer2D.endDrawSprite();
            //Finalizado el dibujado de Sprites


            foreach (Asteroids actualAsteroid in gameAsteroids)
                {
                    actualAsteroid.Update(elapsedTime);
                    actualAsteroid.Render();
                }


            foreach (TgcMesh m in blurredMeshes)
            {
                m.Technique = "DefaultTechnique";
            }
        }

        // Método que se llama cuando termina la ejecución del ejemplo.
        // Hacer dispose() de todos los objetos creados.
        public override void close()
        {
            spaceShip.dispose();
            naveEnemiga.dispose();
            text1.dispose();
            sun.dispose();
            venus.dispose();
            moon.dispose();
            earth.dispose();
            jupiter.dispose();
            neptune.dispose();
            spriteLife.dispose();
            spriteNitro.dispose();
            spaceSphere.dispose();

            foreach (Star actualStar in gameStars)
            {
                actualStar.Close();
            }
            effectBlur.Close();
            
            foreach (Asteroids actualAsteroid in gameAsteroids)
            {
                actualAsteroid.Close();
            }
        }


        private void reducirVida(TgcMesh spaceShip)
        {
            vidaNave -= 10;
            spriteLife.Scaling = new Vector2(spriteLife.Scaling.X, spriteLife.Scaling.Y * (vidaNave / vidaTotal));

        }

        private void reducirNitro(TgcMesh spaceShip, float deltaTime)
        {
            nitroNave -= 1 * deltaTime;
            spriteNitro.Scaling = new Vector2(spriteNitro.Scaling.X, spriteNitro.Scaling.Y * (nitroNave / nitroNaveTotal));

        }

        //Transformacion para todos los planetas: Escala, RotacionTierra, OffsetOrbita, RotacionOrbita, MatrizDeQuienEsSatelite
        private Matrix getPlanetTransform(Vector3 selfScale, float axisRot, float orbitOff, float orbitRot, Matrix satelite)
        {
            Matrix scale = Matrix.Scaling(selfScale);
            Matrix yRot = Matrix.RotationY(axisRot);
            Matrix planetOffset = Matrix.Translation(orbitOff, 0, 0);
            Matrix planetOrbit = Matrix.RotationY(orbitRot);

            return scale * yRot * planetOffset * planetOrbit * satelite;
        }


        public void Subiendo(int unaDir, float velocidad, float deltaTime)
        {
            if (anguloSubida < Geometry.DegreeToRadian(30) && anguloSubida > Geometry.DegreeToRadian(-30))
            {
                anguloSubida += (velocidad / 2) * unaDir * deltaTime;
                spaceShip.rotateX((velocidad / 2) * unaDir * deltaTime);
            }
        }

        public float DistanceAtoB(Vector3 pos1, Vector3 pos2)
        {
            return (pos1 - pos2).Length();
        }

        Vector3 LookAt(Vector3 center, Vector3 target)
        {
            Vector3 up = new Vector3(0, 1, 0);

            Vector3 direccion = Vector3.Normalize(target - center);
            float compX = -FastMath.Asin(-direccion.Y);
            float compY = FastMath.Atan2(-direccion.X, -direccion.Z);

            return new Vector3(compX, compY, 0);
        }

        Bullet crearBalaPara(TgcMesh owner)
        {
            Bullet disparo;
            TgcBox disparoModel;
            disparoModel = TgcBox.fromSize(new Vector3(1, 1, largoBala), Color.Pink);
            disparoModel.Position = owner.Position;
            disparoModel.Rotation = owner.Rotation;
            disparoModel.Rotation.Multiply(0f);
            disparo = new Bullet(disparoModel);
            return disparo;
        }
    }

}
