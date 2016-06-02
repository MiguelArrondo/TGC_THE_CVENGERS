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
        TgcMesh Mesh;
        bool open;

        public Puerta(Vector3 posicionCentro, float rotacion)
        {

            open = false;

            TgcSceneLoader loadedrL = new TgcSceneLoader();
            Mesh = loadedrL.loadSceneFromFile(GuiController.Instance.AlumnoEjemplosDir + "THE_CVENGERS\\AlumnoMedia\\ObjetosMapa\\Puerta\\puerta-TgcScene.xml").Meshes[0];
            Mesh.move(posicionCentro);
            Mesh.Scale = new Vector3(0.5f,0.5f,0.5f);
            Mesh.rotateY(rotacion);
            
        }

        public TgcMesh getMesh()
        {
            return this.Mesh;
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
