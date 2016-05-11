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
using AlumnoEjemplos.MiGrupo;
using Examples.Quake3Loader;
using Examples.Shaders;
using TgcViewer.Utils.Shaders;
using System.IO;
using TgcViewer.Utils.TgcSkeletalAnimation;


namespace AlumnoEjemplos.MiGrupo
{

    

    public class Juego : TgcExample
    {

        string selectedMesh;
        string selectedAnim;
        TgcSkeletalMesh meshVillano;
        Color currentColor;
        TgcSkeletalBoneAttach attachment;
        bool showAttachment;
        string mediaPath;
        string[] animationsPath;

        const float MOVEMENT_SPEED = 400f;
        FPSCustomCamera camera = new FPSCustomCamera();

        List<TgcMesh> meshes;
        
        //Variable para esfera
        TgcBoundingSphere sphere;

        Vector3 lightDirAnterior = (new Vector3(500, 0, 1) - new Vector3(500, 60, 900));
        Vector3 lookAtAnterior = new Vector3(500, 0, 1);

        //PARA EL VILLANO

        Vector3 newPosition;
        Vector3 originalMeshRot;
        Matrix meshRotationMatrix;




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

            //Path para carpeta de texturas de la malla
            mediaPath = GuiController.Instance.ExamplesMediaDir + "SkeletalAnimations\\BasicHuman\\";

            //Cargar dinamicamente todos los Mesh animados que haya en el directorio
            DirectoryInfo dir = new DirectoryInfo(mediaPath);
            FileInfo[] meshFiles = dir.GetFiles("*-TgcSkeletalMesh.xml", SearchOption.TopDirectoryOnly);
            string[] meshList = new string[meshFiles.Length];
            for (int i = 0; i < meshFiles.Length; i++)
            {
                string name = meshFiles[i].Name.Replace("-TgcSkeletalMesh.xml", "");
                meshList[i] = name;
            }

           

            //Cargar dinamicamente todas las animaciones que haya en el directorio "Animations"
            DirectoryInfo dirAnim = new DirectoryInfo(mediaPath + "Animations\\");
            FileInfo[] animFiles = dirAnim.GetFiles("*-TgcSkeletalAnim.xml", SearchOption.TopDirectoryOnly);
            string[] animationList = new string[animFiles.Length];
            animationsPath = new string[animFiles.Length];
            for (int i = 0; i < animFiles.Length; i++)
            {
                string name = animFiles[i].Name.Replace("-TgcSkeletalAnim.xml", "");
                animationList[i] = name;
                animationsPath[i] = animFiles[i].FullName;
            }

            //Cargar mesh inicial
            selectedAnim = animationList[0];
            changeMesh(meshList[0]);

            //Modifier para elegir modelo
            GuiController.Instance.Modifiers.addInterval("mesh", meshList, 0);

            //Agregar combo para elegir animacion
            GuiController.Instance.Modifiers.addInterval("animation", animationList, 0);

            //Modifier para especificar si la animación se anima con loop
            bool animateWithLoop = true;
            GuiController.Instance.Modifiers.addBoolean("loop", "Loop anim:", animateWithLoop);

            //Modifier para renderizar el esqueleto
            bool renderSkeleton = false;
            GuiController.Instance.Modifiers.addBoolean("renderSkeleton", "Show skeleton:", renderSkeleton);

            //Modifier para FrameRate
            GuiController.Instance.Modifiers.addFloat("frameRate", 0, 100, 30);

            //Modifier para color
            currentColor = Color.White;
            GuiController.Instance.Modifiers.addColor("Color", currentColor);

            //Modifier para BoundingBox
            GuiController.Instance.Modifiers.addBoolean("BoundingBox", "BoundingBox:", false);

          

            //Cargamos un escenario
            TgcSceneLoader loader = new TgcSceneLoader();
            TgcScene scene = loader.loadSceneFromFile(GuiController.Instance.AlumnoEjemplosMediaDir + "Orfanato-TgcScene.xml");
            meshes = scene.Meshes;

            //Crear una UserVar
            GuiController.Instance.UserVars.addVar("variableX");
            GuiController.Instance.UserVars.addVar("variableY");
            GuiController.Instance.UserVars.addVar("variableZ");


            GuiController.Instance.UserVars.addVar("LookAt");


            GuiController.Instance.UserVars.addVar("Posicion");

            GuiController.Instance.UserVars.addVar("LightPosicion");
            GuiController.Instance.UserVars.addVar("LightDir");

            GuiController.Instance.UserVars.addVar("lukat");


            camera.Enable = true;

            camera.setCamera(new Vector3(500, 45, 900), new Vector3(500, 0, 1));


        }


        /// <summary>
        /// Cargar una nueva malla
        /// </summary>
        private void changeMesh(string meshName)
        {
            if (selectedMesh == null || selectedMesh != meshName)
            {
                if (meshVillano != null)
                {
                    meshVillano.dispose();
                    meshVillano = null;
                }

                selectedMesh = meshName;

                //Cargar mesh y animaciones
                TgcSkeletalLoader loader = new TgcSkeletalLoader();
                meshVillano = loader.loadMeshAndAnimationsFromFile(mediaPath + selectedMesh + "-TgcSkeletalMesh.xml", mediaPath, animationsPath);

                //Crear esqueleto a modo Debug
                meshVillano.buildSkletonMesh();

                meshVillano.move(500, 6, 800);//(628, 10, 51);


                //Elegir animacion inicial
                meshVillano.playAnimation(selectedAnim, true);

          


            }
        }

        private void changeAnimation(string animation)
        {
            if (selectedAnim != animation)
            {
                selectedAnim = animation;
                meshVillano.playAnimation(selectedAnim, true);
            }
        }





        public override void render(float elapsedTime)
        {
            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;

            sphere.setCenter(camera.getPosition());

            d3dDevice.BeginScene();


           

                ///////////////////////////////////////////// LUCES  /////////////////////////////////////////////////////////////

                Microsoft.DirectX.Direct3D.Effect currentShader;
                //Con luz: Cambiar el shader actual por el shader default que trae el framework para iluminacion dinamica con PointLight
                currentShader = GuiController.Instance.Shaders.TgcMeshSpotLightShader;

                //Aplicar a cada mesh el shader actual
                foreach (TgcMesh mesh in meshes)
                {
                    mesh.Effect = currentShader;
                    //El Technique depende del tipo RenderType del mesh
                    mesh.Technique = GuiController.Instance.Shaders.getTgcMeshTechnique(mesh.RenderType);
                }

            Microsoft.DirectX.Direct3D.Effect skeleticalShader;
            skeleticalShader = GuiController.Instance.Shaders.TgcSkeletalMeshPointLightShader;

            meshVillano.Effect = skeleticalShader;
            meshVillano.Technique= GuiController.Instance.Shaders.getTgcSkeletalMeshTechnique(meshVillano.RenderType);

            //Actualzar posición de la luz


            Vector3 lightPos = camera.getPosition();
                GuiController.Instance.UserVars.setValue("LightPosicion", lightPos);

                Vector3 lightDir;

                if (lookAtAnterior == camera.getLookAt())
                {

                    lightDir = lightDirAnterior;
                }

                else
                {
                    lightDir = (camera.getLookAt() - camera.getPosition());
                    lookAtAnterior = camera.getLookAt();
                    lightDirAnterior = lightDir;

                }

                GuiController.Instance.UserVars.setValue("lukat", lookAtAnterior);

                lightDir.Normalize();

                GuiController.Instance.UserVars.setValue("LightDir", lightDir);

                foreach (TgcMesh mesh in meshes)
                {

                    //Cargar variables shader de la luz
                    mesh.Effect.SetValue("lightColor", ColorValue.FromColor(Color.White));
                    mesh.Effect.SetValue("lightPosition", TgcParserUtils.vector3ToFloat4Array(camera.getPosition()));
                    mesh.Effect.SetValue("eyePosition", TgcParserUtils.vector3ToFloat4Array(lightPos));
                    mesh.Effect.SetValue("spotLightDir", TgcParserUtils.vector3ToFloat3Array(lightDir));

                mesh.Effect.SetValue("lightIntensity", (float)90f);
                mesh.Effect.SetValue("lightAttenuation", (float)0.3f);
                mesh.Effect.SetValue("spotLightAngleCos", FastMath.ToRad((float)45f));
                mesh.Effect.SetValue("spotLightExponent", (float)7f);

                Color myArgbColor = new Color();
                myArgbColor = Color.FromArgb(24, 24, 24);
                // myArgbColor = Color.FromRgb(37, 37, 37);
                //Cargar variables de shader de Material. El Material en realidad deberia ser propio de cada mesh. Pero en este ejemplo se simplifica con uno comun para todos
                mesh.Effect.SetValue("materialEmissiveColor", ColorValue.FromColor(myArgbColor));// .FromColor(Color.Black));
                    mesh.Effect.SetValue("materialAmbientColor", ColorValue.FromColor(myArgbColor));
                mesh.Effect.SetValue("materialDiffuseColor", ColorValue.FromColor(myArgbColor));
                mesh.Effect.SetValue("materialSpecularColor", ColorValue.FromColor(myArgbColor));
                mesh.Effect.SetValue("materialSpecularExp", (float)20f);

                

                    //Renderizar modelo
                    mesh.render();
                }

                
               //////////////// BETA LUCES VILLANO //////////////////
           
                //Cargar variables shader de la luz
                meshVillano.Effect.SetValue("lightColor", ColorValue.FromColor(Color.White));
            meshVillano.Effect.SetValue("lightPosition", TgcParserUtils.vector3ToFloat4Array(camera.getPosition()));
                     meshVillano.Effect.SetValue("lightIntensity", (float)90f);
            meshVillano.Effect.SetValue("lightAttenuation", (float)1.05f);
          
            //Cargar variables de shader de Material. El Material en realidad deberia ser propio de cada mesh. Pero en este ejemplo se simplifica con uno comun para todos
            meshVillano.Effect.SetValue("materialEmissiveColor", ColorValue.FromColor(Color.Black));
            meshVillano.Effect.SetValue("materialAmbientColor", ColorValue.FromColor(Color.White));
            meshVillano.Effect.SetValue("materialDiffuseColor", ColorValue.FromColor(Color.White));
            meshVillano.Effect.SetValue("materialSpecularColor", ColorValue.FromColor(Color.White));
            meshVillano.Effect.SetValue("materialSpecularExp", (float)200f);


            //Renderizar modelo
            meshVillano.animateAndRender();

            ////////////////  FIN BETA LUCES VILLANO //////////////////




            ///////////////////////////////////////////// FIN LUCES  /////////////////////////////////////////////////////////////



            /////////////////////////////////////////////  PARA EL VILLANO  ///////////////////////////////////////////////////////////

            //Ver si cambio la malla
            string meshPath = (string)GuiController.Instance.Modifiers.getValue("mesh");
            changeMesh(meshPath);

            //Ver si cambio la animacion
            string anim = (string)GuiController.Instance.Modifiers.getValue("animation");
            changeAnimation(anim);

            //Ver si rendeizamos el esqueleto
            bool renderSkeleton = (bool)GuiController.Instance.Modifiers.getValue("renderSkeleton");

            //Ver si cambio el color
            Color selectedColor = (Color)GuiController.Instance.Modifiers.getValue("Color");
            if (currentColor == null || currentColor != selectedColor)
            {
                currentColor = selectedColor;
                meshVillano.setColor(currentColor);
            }

    

            //Actualizar animacion
            meshVillano.updateAnimation();

            //Solo malla o esqueleto, depende lo seleccionado
            meshVillano.RenderSkeleton = renderSkeleton;
            meshVillano.render();

            //Se puede renderizar todo mucho mas simple (sin esqueleto) de la siguiente forma:
            //mesh.animateAndRender();


            //BoundingBox
            bool showBB = (bool)GuiController.Instance.Modifiers["BoundingBox"];
            if (showBB)
            {
                meshVillano.BoundingBox.render();
            }

            ///////////////////////////// MOVIMIENTO VILLANO/////////////////////////////////



    
  /*  newPosition.X = camera.getPosition().X;
        newPosition.Y = 0f;
        newPosition.Z = camera.getPosition().Z;

            //Ver si queda algo de distancia para mover
            Vector3 posDiff = newPosition - meshVillano.Position;
            float posDiffLength = posDiff.LengthSq();
            if (posDiffLength > float.Epsilon)
            {
                //Movemos el mesh interpolando por la velocidad
                float currentVelocity = 200f * elapsedTime;
                posDiff.Normalize();
                posDiff.Multiply(currentVelocity);

                //Ajustar cuando llegamos al final del recorrido
                Vector3 newPos = meshVillano.Position + posDiff;
                if (posDiff.LengthSq() > posDiffLength)
                {
                    newPos = newPosition;
                }

                //Actualizar posicion del mesh
                meshVillano.Position = newPos;

                //Como desactivamos la transformacion automatica, tenemos que armar nosotros la matriz de transformacion
                // mesh.Transform = meshRotationMatrix * Matrix.Translation(mesh.Position);

            }*/



            bool collisionVillanoCamara = false;
            Vector3 posicionOriginalVillano = meshVillano.Position;
            foreach (TgcMesh mesh in meshes)
            {
                //Los dos BoundingBox que vamos a testear
                TgcBoundingBox sceneMeshBoundingBox = mesh.BoundingBox;


                //Hubo colisión con un objeto. Guardar resultado y abortar loop

                //Hubo colisión con un objeto. Guardar resultado y abortar loop.
                if (TgcCollisionUtils.testSphereAABB(sphere, meshVillano.BoundingBox))  //(meshVillano.BoundingBox, sceneMeshBoundingBox))
                {
                    collisionVillanoCamara = true;
                    break;
                }
            }

            if (collisionVillanoCamara)
            {
               
              meshVillano.Position = new Vector3(628, 7, 51);
            }

            bool collisionVillanoPared = false;
            Vector3 diferenciaPosicion = new Vector3();
            foreach (TgcMesh mesh in meshes)
            {
                //Los dos BoundingBox que vamos a testear
                TgcBoundingBox sceneMeshBoundingBox = mesh.BoundingBox;


                //Hubo colisión con un objeto. Guardar resultado y abortar loop



                //Hubo colisión con un objeto. Guardar resultado y abortar loop.
                if (TgcCollisionUtils.testAABBAABB(meshVillano.BoundingBox, sceneMeshBoundingBox))
                {
                    collisionVillanoPared = true;
                    break;
                    
                }
            }
            
            if (collisionVillanoPared)
            {
                diferenciaPosicion = meshVillano.Position - posicionOriginalVillano;
                meshVillano.Position =   meshVillano.Position - diferenciaPosicion;
            }

           
            //Ver si queda algo de distancia para mover
            /*   Vector3 posDiff = newPosition - meshVillano.Position;
               float posDiffLength = posDiff.LengthSq();
               if (posDiffLength > float.Epsilon)
               {
                   //Movemos el mesh interpolando por la velocidad
                   float currentVelocity = 80 * elapsedTime;
                   posDiff.Normalize();
                   posDiff.Multiply(currentVelocity);

                   //Ajustar cuando llegamos al final del recorrido
                   Vector3 newPos = meshVillano.Position + posDiff;
                   if (posDiff.LengthSq() > posDiffLength)
                   {
                       newPos = newPosition;
                   }

                   //Actualizar flecha de movimiento


                   //Actualizar posicion del mesh
                   meshVillano.Position = newPos;

                   //Como desactivamos la transformacion automatica, tenemos que armar nosotros la matriz de transformacion
                   meshVillano.Transform = meshRotationMatrix * Matrix.Translation(meshVillano.Position);

               }

       */



            ///////////////////////////// FIN MOVIMIENTO VILLANO/////////////////////////////////



            ///////////////////////////////////////////// FIN PARA EL VILLANO  ///////////////////////////////////////////////////////////


            //Render de cada mesh
            foreach (TgcMesh mesh in meshes)
                {

                    mesh.render();
                }
                //sphere.render();

                d3dDevice.EndScene();

                //Guardar posicion original antes de cambiarla
                Vector3 originalPos = camera.getPosition();
                Vector3 originalLook = camera.getLookAt();
                Matrix view = camera.ViewMatrix;
                Vector3 z = camera.ZAxis;
                Vector3 x = camera.XAxis;
                Vector3 y = camera.YAxis;
                Vector3 direction = camera.Direction;



                //    Vector3 velocity = camera.CurrentVelocity;

                //Cargar valor en UserVar
                GuiController.Instance.UserVars.setValue("variableX", camera.getPosition().X);
                GuiController.Instance.UserVars.setValue("variableY", camera.getPosition().Y);
                GuiController.Instance.UserVars.setValue("variableZ", camera.getPosition().Z);

                GuiController.Instance.UserVars.setValue("LookAt", camera.getLookAt());
                GuiController.Instance.UserVars.setValue("Posicion", camera.getPosition());

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


                    // camera.ViewMatrix = view;
                    camera.setearCamara(originalPos, originalLook, view, x, y, z, direction);


                }








            
        }

        public override void close()
        {
            
            foreach (TgcMesh mesh in meshes)
            {
                mesh.dispose();
            }

            sphere.dispose();
            meshVillano.dispose();
        }

    }
}
