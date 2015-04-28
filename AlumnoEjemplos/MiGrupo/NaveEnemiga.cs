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

namespace AlumnoEjemplos.NaveEnemiga
{
    public class NaveEnemiga : TgcExample
    {

        TgcBox box; //Simula la nave principal
        TgcMesh naveEnemiga; //Nave

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


            //Velocidad de la Nave
            GuiController.Instance.Modifiers.addFloat("Speed", 0f, 30f, 10f);


            //Camara sobre la caja
            GuiController.Instance.ThirdPersonCamera.Enable = true;
            GuiController.Instance.ThirdPersonCamera.setCamera(box.Position, 100, 200);
        }

        public override void render(float elapsedTime)
        {
            Device d3dDevice = GuiController.Instance.D3dDevice;

            //Obtene valor de Modifier
            float Speed = (float)GuiController.Instance.Modifiers["Speed"];

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
    }
}
