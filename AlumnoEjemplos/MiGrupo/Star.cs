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

namespace AlumnoEjemplos.MiGrupo
{
    class Star
    {
        Vector2 spriteSize;
        //El indice el sprite actual.
        int currentStar;
        float size;
        float angle;
        public Vector2 Position;
        List<TgcSprite> stars;
        Size screenSize;

        float speed;


        public void Load()
        {
            screenSize = GuiController.Instance.Panel3d.Size;

            //Creo la lista de Estrellas en HyperSpeed.
            stars = new List<TgcSprite>();

            Random rnd = new Random();
            spriteSize = new Vector2(3, 3);
            size = 3 * (float)rnd.NextDouble();
            angle = 0.0f;
            speed = 300 + 100 * (float)rnd.NextDouble();

            TgcSprite newStar;
            //Creo 64 sprites asignando distintos clipping rects a cada uno.
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    newStar = new TgcSprite();
                    newStar.SrcRect = new Rectangle(j * (int)spriteSize.X, i * (int)spriteSize.Y, (int)spriteSize.X, (int)spriteSize.Y);
                    newStar.Color = Color.WhiteSmoke;
                    newStar.Texture = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosMediaDir + "\\Texturas\\Star.png");
                    newStar.Scaling = new Vector2(size, size);
                    stars.Add(newStar);
                }
            }
        //primer estrella
        currentStar = 0;
        GenerateRandomPosition();
        }

        public void Update(float elapsedTime)
        {
            //Chequeo si se escapa de la pantalla.
            if (Position.X < -spriteSize.X ||
                Position.Y < -spriteSize.Y * 2 ||
                Position.X > screenSize.Width + spriteSize.X ||
                Position.Y > screenSize.Height + spriteSize.Y)
            {
                GenerateRandomPosition();
            }

            Position.X += speed * elapsedTime * (float)Math.Cos(angle);
            Position.Y += speed * elapsedTime * (float)Math.Sin(angle);

            currentStar++;
            if (currentStar > 63) currentStar = 0;

            stars[currentStar].Position = Position;
        }


        public void GenerateRandomPosition()
        {
            //Los creo en el centro de la pantalla
            float starPosEndX;
            float starPosEndY;

            Random rnd = new Random();

            Position.X = (screenSize.Width / 2);
            Position.Y = (screenSize.Height / 2);

            int sideStar = (int)(rnd.NextDouble() * 2);
            if (sideStar == 0)
                starPosEndY = 0;
            else
                starPosEndY = screenSize.Height;
            starPosEndX = screenSize.Width * (float)rnd.NextDouble();

            //Creo el angulo hacia el borde de la pantalla.
            Vector2 posEnd = new Vector2(starPosEndX, starPosEndY);
            Vector2 screenEndVector = new Vector2();
            screenEndVector = Vector2.Subtract(posEnd, Position);
            angle = (float)Math.Atan2(screenEndVector.X, screenEndVector.Y);

        }


        public void Render()
        {
            //stars[currentStar].render();
            //Render Stars
            currentStar = 0;
            foreach (TgcSprite star in stars)
            {
                currentStar++;
                if (currentStar > 63) currentStar = 0;

                stars[currentStar].render();
            }
        }


        public void Close()
        {
            //Dispose Stars
            currentStar = 0;
            foreach (TgcSprite star in stars)
            {
                currentStar++;
                if (currentStar > 63) currentStar = 0;

                stars[currentStar].dispose();
            }
        }
    }
}
