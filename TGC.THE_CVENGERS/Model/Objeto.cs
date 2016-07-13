using Microsoft.DirectX;
using TGC.Core.SceneLoader;
using TGC.Core.Utils;

namespace TGC.Group.Model
{
    internal class Objeto
    {
        private TgcMesh mesh;
        public string nombre;
        public Vector3 posicionAux;

        public Objeto(string MediaDir, Vector3 posicion, float rotacion, Vector3 escalas, string objeto)
        {
            TgcSceneLoader loadedrL = new TgcSceneLoader();
            mesh = loadedrL.loadSceneFromFile(MediaDir + "" + objeto).Meshes[0];
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