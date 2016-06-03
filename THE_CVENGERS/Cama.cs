using Examples.SceneLoader;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.THE_CVENGERS
{
    class Cama
    {
        TgcMesh mesh;

        public Cama(Vector3 posicion, float rotacion)
        {
            TgcSceneLoader loadedrL = new TgcSceneLoader();
            mesh = loadedrL.loadSceneFromFile(GuiController.Instance.AlumnoEjemplosDir + "THE_CVENGERS\\AlumnoMedia\\My+Room-TgcScene.xml").Meshes[0];
            mesh.rotateY(rotacion);
            mesh.move(posicion);
            mesh.Scale = new Vector3(0.4f, 0.4f, 0.4f);
           

        }

        public TgcMesh getMesh()
        {
            return this.mesh;
        }

        public void Render()
        {
            mesh.render();
        }
    }
}
