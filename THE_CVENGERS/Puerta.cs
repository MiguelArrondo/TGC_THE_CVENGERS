using System;
using System.Collections.Generic;
using System.Text;
using TgcViewer.Example;
using TgcViewer;
using Microsoft.DirectX.Direct3D;
using System.Drawing;
using Microsoft.DirectX;
using TgcViewer.Utils.Modifiers;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.Shaders;
using TgcViewer.Utils;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.Input;
using Microsoft.DirectX.DirectInput;
using Examples.Quake3Loader;
using Examples.Shaders;
using TgcViewer.Utils.Shaders;
using System.IO;
using TgcViewer.Utils.TgcSkeletalAnimation;

namespace AlumnoEjemplos.THE_CVENGERS
{
    class Puerta
    {
        TgcBox box;
        bool open;

        public Puerta(Vector3 posicionCentro)
        {

            open = false;

            Vector3 size = new Vector3(70, 80, 5);
            TgcTexture texture = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosDir + "THE_CVENGERS\\AlumnoMedia\\wood-door.jpg");
            box = TgcBox.fromSize(size, texture);
            box.Position = posicionCentro;
            box.AutoTransformEnable = false;
            box.Transform = Matrix.Translation(posicionCentro);

        }

        public TgcBox getBox()
        {
            return this.box;
        }

        public bool getStatus()
        {
            return this.open;
        }

        public void setStatus(bool status)
        {
            open = status;
        }
    }
}
