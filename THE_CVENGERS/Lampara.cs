using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.THE_CVENGERS
{
    class Lampara
    {
        TgcMesh mesh;
        public Vector3 lightDir;
        public Vector3 lightPos;

        public Lampara(Vector3 posicion, float rotacion, Vector3 escalas, Vector3 dirSpotLight, Vector3 spotLight)
        {
            TgcSceneLoader loadedrL = new TgcSceneLoader();
            mesh = loadedrL.loadSceneFromFile(GuiController.Instance.AlumnoEjemplosDir + "THE_CVENGERS\\AlumnoMedia\\Lampara+de+pared-TgcScene.xml").Meshes[0];
            this.mesh.AutoTransformEnable = false;
            this.mesh.AutoUpdateBoundingBox = false;

            Matrix matrizEscala = Matrix.Scaling(escalas);
            Matrix matrizPosicion = Matrix.Translation(posicion);


            float angleY = FastMath.ToRad(rotacion);
            Matrix matrizRotacion = Matrix.RotationY(angleY);


            this.mesh.Transform = matrizRotacion * matrizEscala * matrizPosicion;
            this.mesh.BoundingBox.transform(this.mesh.Transform);

            lightDir = dirSpotLight;
            lightPos = spotLight;


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
