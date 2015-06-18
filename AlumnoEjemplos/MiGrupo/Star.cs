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
        float size;
        float angle;
        public Vector2 Position;
        TgcSprite newStar;
        Size screenSize;
        float offsetFromCenter = 20f;

        float speed = 300;


        public void Load()
        {
            screenSize = GuiController.Instance.Panel3d.Size;

            Random rnd = new Random();
            spriteSize = new Vector2(3, 3);
            size = 3 * (float)rnd.NextDouble();
            angle = 0.0f;
            speed += 100 * (float)rnd.NextDouble();

                    newStar = new TgcSprite();
                    newStar.SrcRect = new Rectangle(1 * (int)spriteSize.X, 1 * (int)spriteSize.Y, (int)spriteSize.X, (int)spriteSize.Y);
                    newStar.Color = Color.WhiteSmoke;
                    newStar.Texture = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosMediaDir + "\\Texturas\\Star.png");
                    newStar.Scaling = new Vector2(size, size);

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


            newStar.Position = Position;
        }


        public void GenerateRandomPosition()
        {
            //Los creo en el centro de la pantalla (alrededor de un offset dado)
            float starPosEndX;
            float starPosEndY;

            Random rnd = new Random();
            Random rndMovementX = new Random();
            Random rndDirectionX = new Random();
            Random rndMovementY = new Random();
            Random rndDirectionY = new Random();


            if ((float)rndDirectionX.NextDouble() < 0.5f)   { Position.X = (screenSize.Width / 2) - offsetFromCenter * ((float)rndMovementX.NextDouble() + 0.1f); }
            else                                            { Position.X = (screenSize.Width / 2) + offsetFromCenter * ((float)rndMovementX.NextDouble() + 0.1f); }
            if ((float)rndDirectionY.NextDouble() < 0.5f)   { Position.Y = (screenSize.Height / 2) - offsetFromCenter * ((float)rndMovementY.NextDouble() + 0.1f); }
            else                                            { Position.Y = (screenSize.Height / 2) + offsetFromCenter * ((float)rndMovementY.NextDouble() + 0.1f); }

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
                newStar.render();
        }


        public void Close()
        {
                newStar.dispose();
        }
    }
}
