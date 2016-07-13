using Microsoft.DirectX;
using TGC.Core.SceneLoader;
using TGC.Core.Utils;

namespace TGC.Group.Model
{
    internal class Lampara
    {
        private TgcMesh mesh;
        public Vector3 lightDir;
        public Vector3 lightPos;

        public Lampara(string mediaDir, Vector3 posicion, float rotacion, Vector3 escalas, Vector3 dirSpotLight, Vector3 spotLight)
        {
            TgcSceneLoader loadedrL = new TgcSceneLoader();
            mesh = loadedrL.loadSceneFromFile(mediaDir + "Lampara+de+pared-TgcScene.xml").Meshes[0];
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