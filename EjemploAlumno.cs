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

namespace Examples.DirectX
{
 
    public class EjemploGetZBuffer : TgcExample
    {
        const float MOVEMENT_SPEED = 400f;

        List<TgcMesh> meshes;
        
        //Variable para esfera
        TgcBoundingSphere sphere;


        public override string getCategory()
        {
            return "MiGrupo";
        }

        public override string getName()
        {
            return "EjemploAlumno";
        }

        public override string getDescription()
        {
            return "Orfanato.";
        }


        public override void init()
        {
            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;

            //Creamos caja de colision
            sphere = new TgcBoundingSphere(new Vector3(160, 60, 240), 20f);

            //Activamos el renderizado customizado. De esta forma el framework nos delega control total sobre como dibujar en pantalla
            //La responsabilidad cae toda de nuestro lado
            GuiController.Instance.CustomRenderEnabled = true;


            //Cargamos un escenario
            TgcSceneLoader loader = new TgcSceneLoader();
            TgcScene scene = loader.loadSceneFromFile(GuiController.Instance.ExamplesMediaDir + "MeshCreator\\Scenes\\Deposito\\Deposito-TgcScene.xml");
            meshes = scene.Meshes;


           
            GuiController.Instance.FpsCamera.Enable = true;
            GuiController.Instance.FpsCamera.MovementSpeed = 400f;
            GuiController.Instance.FpsCamera.JumpSpeed = 300f;
            GuiController.Instance.FpsCamera.setCamera(new Vector3(50, 60, 240), new Vector3(2300, 0, 1));
            //GuiController.Instance.FpsCamera.setCamera(new Vector3(160, 60, 240), new Vector3(2300, 0, 1));

        }


        public override void render(float elapsedTime)
        {
            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;



            sphere.setCenter(GuiController.Instance.FpsCamera.getPosition());

            d3dDevice.BeginScene();
            //sphere.render();

           

            //Render de cada mesh
            foreach (TgcMesh mesh in meshes)
            {
              
                mesh.render();
            }

  
            d3dDevice.EndScene();

            //Guardar posicion original antes de cambiarla
            Vector3 originalPos = sphere.Position;
            Vector3 originalCam = GuiController.Instance.FpsCamera.getLookAt();



            //Chequear si el objeto principal en su nueva posición choca con alguno de los objetos de la escena.
            //Si es así, entonces volvemos a la posición original.
            //Cada TgcMesh tiene un objeto llamado BoundingBox. El BoundingBox es una caja 3D que representa al objeto
            //de forma simplificada (sin tener en cuenta toda la complejidad interna del modelo).
            //Este BoundingBox se utiliza para chequear si dos objetos colisionan entre sí.
            //El framework posee la clase TgcCollisionUtils con muchos algoritmos de colisión de distintos tipos de objetos.
            //Por ejemplo chequear si dos cajas colisionan entre sí, o dos esferas, o esfera con caja, etc.
            bool collisionFound = false;
            foreach (TgcMesh mesh in meshes)
            {
                //Los dos BoundingBox que vamos a testear
                TgcBoundingSphere mainMeshBoundingBox = sphere;
                TgcBoundingBox sceneMeshBoundingBox = mesh.BoundingBox;


                //Hubo colisión con un objeto. Guardar resultado y abortar loop.
                if (TgcCollisionUtils.testSphereAABB(mainMeshBoundingBox, sceneMeshBoundingBox))
                {
                    collisionFound = true;
                    break;
                }
            }

            //Si hubo alguna colisión, entonces restaurar la posición original del mesh
            if (collisionFound)
            {
              GuiController.Instance.FpsCamera.setCamera(originalPos, originalCam);
    
            }




           

         


        }

        public override void close()
        {
            
            foreach (TgcMesh mesh in meshes)
            {
                mesh.dispose();
            }

            sphere.dispose();
        }

    }
}
