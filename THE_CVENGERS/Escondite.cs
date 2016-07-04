﻿using Microsoft.DirectX;
using TgcViewer;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.THE_CVENGERS
{
    class Escondite
    {
        TgcMesh mesh;
        public Vector3 posHidden;
        public Vector3 LookAtHidden;

        public Escondite(Vector3 posicion, float rotacion, Vector3 escalas, string objeto, Vector3 posicionEscondido, Vector3 lookAtEscondido)
        {
            TgcSceneLoader loadedrL = new TgcSceneLoader();
            mesh = loadedrL.loadSceneFromFile(GuiController.Instance.AlumnoEjemplosDir + "THE_CVENGERS\\AlumnoMedia\\" + objeto).Meshes[0];
            this.mesh.AutoTransformEnable = false;
            this.mesh.AutoUpdateBoundingBox = false;

            Matrix matrizEscala = Matrix.Scaling(escalas);
            Matrix matrizPosicion = Matrix.Translation(posicion);

            this.mesh.Position = posicion;

            float angleY = FastMath.ToRad(rotacion);
            Matrix matrizRotacion = Matrix.RotationY(angleY);


            this.mesh.Transform = matrizRotacion * matrizEscala * matrizPosicion;
            this.mesh.BoundingBox.transform(this.mesh.Transform);

            posHidden = posicionEscondido;
            LookAtHidden = lookAtEscondido;

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
