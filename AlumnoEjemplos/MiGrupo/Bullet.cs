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

namespace AlumnoEjemplos.MiGrupo
{
    class Bullet
    {
        public TgcBox renderModel;
        public float timeAlive = 0f;
        public float destroyTime = 0.5f;
        public bool done = false;

        public Bullet(TgcBox unModelo)
        {
            renderModel = unModelo;
        }

        public void incrementarTiempo(float deltaTime)
        {
            timeAlive += deltaTime;
            if (timeAlive > destroyTime)
            {
                done = true;
            }
        }

        public Boolean getDone()
        {
            return done;
        }
    }
}
