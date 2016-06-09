using Examples.SceneLoader;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.THE_CVENGERS
{
    class Objeto
    {
        TgcMesh mesh;
        public string nombre;
        public Vector3 posicionAux;

        public Objeto(Vector3 posicion, float rotacion, Vector3 escalas, string objeto)
        {
            TgcSceneLoader loadedrL = new TgcSceneLoader();
            mesh = loadedrL.loadSceneFromFile(GuiController.Instance.AlumnoEjemplosDir + "THE_CVENGERS\\AlumnoMedia\\" + objeto).Meshes[0];
            this.mesh.AutoTransformEnable = false;
            this.mesh.AutoUpdateBoundingBox = false;
            nombre = objeto;
            posicionAux = posicion;
            Matrix matrizEscala = Matrix.Scaling(escalas);
            Matrix matrizPosicion = Matrix.Translation(posicion);


            float angleY = FastMath.ToRad(rotacion);
            Matrix matrizRotacion = Matrix.RotationY(angleY);


            this.mesh.Transform = matrizRotacion * matrizEscala * matrizPosicion;
            this.mesh.BoundingBox.transform(this.mesh.Transform);


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
