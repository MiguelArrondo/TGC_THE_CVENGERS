using Microsoft.DirectX;
using TGC.Core.SceneLoader;
using TGC.Core.Utils;

namespace TGC.Group.Model
{
    internal class Escondite
    {
        private TgcMesh mesh;
        public Vector3 posHidden;
        public Vector3 LookAtHidden;

        public Escondite(string MediaDir, Vector3 posicion, float rotacion, Vector3 escalas, string objeto, Vector3 posicionEscondido, Vector3 lookAtEscondido)
        {
            TgcSceneLoader loadedrL = new TgcSceneLoader();
            mesh = loadedrL.loadSceneFromFile(MediaDir + "" + objeto).Meshes[0];
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