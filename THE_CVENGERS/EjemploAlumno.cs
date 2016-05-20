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
        Vector3 posicionOriginalVillano;
        CalculadoraDeTrayecto Aux = new CalculadoraDeTrayecto();
        SearchParameters parametrosBusq;
        Vector3 camaraAnterior = new Vector3(0, 0, 0);
        List<Point> path;
        int contadorFrames = 0;
        Microsoft.DirectX.Direct3D.Effect currentShader;
        //Con luz: Cambiar el shader actual por el shader default que trae el framework para iluminacion dinamica con PointLight
        Microsoft.DirectX.Direct3D.Effect skeletalShader;
        Vector3 prueba;
        Color myArgbColor = new Color();
        CalculadoraDeTrayecto Astar;


        const float MOVEMENT_SPEED = 400f;
        FPSCustomCamera camera = new FPSCustomCamera();

        List<TgcMesh> meshes;

        bool luzPrendida = true;
        
        //Variable para esfera
        TgcBoundingSphere sphere;

        TgcBox puerta;
        bool open = false;



        //PARA EL VILLANO

        Vector3 newPosition;
        Vector3 originalMeshRot;
        Matrix meshRotationMatrix;




        public override string getCategory()
        {
            return "THE_CVENGERS";
        }

        public override string getName()
        {
            return "Orfanato";
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

      

           

            //Cargar dinamicamente todas las animaciones que haya en el directorio "Animations"
            DirectoryInfo dirAnim = new DirectoryInfo(mediaPath + "Animations\\");
            FileInfo[] animFiles = dirAnim.GetFiles("CrouchWalk-TgcSkeletalAnim.xml", SearchOption.TopDirectoryOnly);
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
            //  changeMesh(meshList[0]);


            TgcSkeletalLoader loaderVillano = new TgcSkeletalLoader();
            meshVillano = loaderVillano.loadMeshAndAnimationsFromFile(mediaPath + "CS_Gign-TgcSkeletalMesh.xml", mediaPath, animationsPath);

            //Crear esqueleto a modo Debug
            meshVillano.buildSkletonMesh();

            meshVillano.move(new Vector3(289, 5, 577));//(628, 10, 51);

            meshVillano.playAnimation(selectedAnim, true);


          

            //Cargamos un escenario
            TgcSceneLoader loader = new TgcSceneLoader();
            TgcScene scene = loader.loadSceneFromFile(GuiController.Instance.AlumnoEjemplosDir + "THE_CVENGERS\\AlumnoMedia\\Orfanato-TgcScene.xml");
            meshes = scene.Meshes;

            Aux.map = scene;
            Aux.personaje = meshVillano;
            Aux.analizarPuntosPared();
            Aux.InitializeNodes(Aux.mapBool);

            //Crear una UserVar
            GuiController.Instance.UserVars.addVar("PosicionX");
            GuiController.Instance.UserVars.addVar("PosicionY");
            GuiController.Instance.UserVars.addVar("PosicionZ");


            ////// PUERTA 1.0

            Vector3 center = new Vector3(374, 50, 810);
            Vector3 size = new Vector3(70, 80, 5);

            TgcTexture texture = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosDir+ "THE_CVENGERS\\AlumnoMedia\\wood-door.jpg");

            puerta = TgcBox.fromSize(center, size, texture);

            // aca esta el bardiñho
            puerta.AutoTransformEnable = false;

            //// PUERTA 1.0


            originalMeshRot = new Vector3(0, 0, -1);


            camera.Enable = true;

            camera.setCamera(new Vector3(500, 45, 900), new Vector3(500, 0, 1));

            currentShader = TgcShaders.loadEffect(GuiController.Instance.AlumnoEjemplosDir + "THE_CVENGERS\\AlumnoMedia\\Shaders\\MeshSpotLightShader.fx");

            //Aplicar a cada mesh el shader actual
            foreach (TgcMesh mesh in meshes)
            {
                mesh.Effect = currentShader;
                //El Technique depende del tipo RenderType del mesh
                mesh.Technique = GuiController.Instance.Shaders.getTgcMeshTechnique(mesh.RenderType);
            }

            skeletalShader = GuiController.Instance.Shaders.TgcSkeletalMeshPointLightShader;

            
            meshVillano.Technique = GuiController.Instance.Shaders.getTgcSkeletalMeshTechnique(meshVillano.RenderType);

            myArgbColor = Color.FromArgb(11, 11, 11);

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
            TgcD3dInput input = GuiController.Instance.D3dInput;
            sphere.setCenter(camera.getPosition());

            d3dDevice.BeginScene();




            ///////////////////////////////////////////// LUCES  /////////////////////////////////////////////////////////////



          
            


           
            //Actualzar posición de la luz

            



            Vector3 lightDir = (camera.getLookAt() - camera.getPosition());
            lightDir.Normalize();

            

            /* saraseada de prueba
              prueba.X = (camera.getLookAt().X - camera.getPosition().X) - 15;
            prueba.Z = (camera.getLookAt().Y - camera.getPosition().Y) - 40;
            prueba.Y = camera.getLookAt().Y - 20;

             
             
             */

            if (luzPrendida)
            {
                prueba.X = camera.getPosition().X - 15;
                prueba.Z = camera.getPosition().Z - 40;
                prueba.Y = camera.getPosition().Y - 20;
            }

            else {

                prueba.X = 0;
                prueba.Z = 0;
                prueba.Y = 0;


            }

            
        

            foreach (TgcMesh mesh in meshes)
            {




                //Cargar variables shader de la luz

                if (luzPrendida)

                {
                    mesh.Effect.SetValue("lightColor", ColorValue.FromColor(Color.White));
                }
                else
                {
                    mesh.Effect.SetValue("lightColor", ColorValue.FromColor(myArgbColor));

                }
                mesh.Effect.SetValue("lightPosition", TgcParserUtils.vector3ToFloat4Array(prueba));
                mesh.Effect.SetValue("eyePosition", TgcParserUtils.vector3ToFloat4Array(camera.getPosition()));
                mesh.Effect.SetValue("spotLightDir", TgcParserUtils.vector3ToFloat3Array(lightDir));

                mesh.Effect.SetValue("spotLightAngleCos", FastMath.ToRad((float)54f));

                //   Ya seteados en el shader propio
                mesh.Effect.SetValue("spotLightExponent", (float)8f);
                mesh.Effect.SetValue("lightIntensity", (float)300f);
                mesh.Effect.SetValue("lightAttenuation", (float)0.5f);



                //Cargar variables de shader de Material. El Material en realidad deberia ser propio de cada mesh. Pero en este ejemplo se simplifica con uno comun para todos
                mesh.Effect.SetValue("materialEmissiveColor", ColorValue.FromColor(myArgbColor));// .FromColor(Color.Black));
                mesh.Effect.SetValue("materialAmbientColor", ColorValue.FromColor(myArgbColor));
                mesh.Effect.SetValue("materialDiffuseColor", ColorValue.FromColor(myArgbColor));
                mesh.Effect.SetValue("materialSpecularColor", ColorValue.FromColor(myArgbColor));
                mesh.Effect.SetValue("materialSpecularExp", (float)20f);




                //Renderizar modelo
                mesh.render();
            }




            if (input.keyUp(Key.F))
            {
                if (luzPrendida)
                {
                    luzPrendida = false;
                }
                else { luzPrendida = true; }


            }
            //////////////// BETA LUCES VILLANO //////////////////
            meshVillano.Effect = skeletalShader;
            //Cargar variables shader de la luz
            meshVillano.Effect.SetValue("lightColor", ColorValue.FromColor(Color.White));

            meshVillano.Effect.SetValue("lightPosition", TgcParserUtils.vector3ToFloat4Array(prueba));
            meshVillano.Effect.SetValue("lightIntensity", (float)30f);
            meshVillano.Effect.SetValue("lightAttenuation", (float)1.05f);

            //Cargar variables de shader de Material. El Material en realidad deberia ser propio de cada mesh. Pero en este ejemplo se simplifica con uno comun para todos
            meshVillano.Effect.SetValue("materialEmissiveColor", ColorValue.FromColor(Color.Black));
            meshVillano.Effect.SetValue("materialAmbientColor", ColorValue.FromColor(Color.White));
            meshVillano.Effect.SetValue("materialDiffuseColor", ColorValue.FromColor(myArgbColor));
            meshVillano.Effect.SetValue("materialSpecularColor", ColorValue.FromColor(Color.White));
            meshVillano.Effect.SetValue("materialSpecularExp", (float)20f);



            ////////////////  FIN BETA LUCES VILLANO //////////////////




            ///////////////////////////////////////////// FIN LUCES  /////////////////////////////////////////////////////////////



            /////////////////////////////////////////////  PARA EL VILLANO  ///////////////////////////////////////////////////////////

        
            meshVillano.updateAnimation();
            meshVillano.render();


            ///////////////////////////// MOVIMIENTO VILLANO/////////////////////////////////

            bool collisionVillanoCamara = false;


            newPosition.X = camera.getPosition().X;
            newPosition.Y = 5;
            newPosition.Z = camera.getPosition().Z;

            if (contadorFrames == 0)
            {
                meshVillano.Position = new Vector3(289, 5, 577);
            }
                    if (contadorFrames == 0 || contadorFrames % 100 == 0)
                      {
                         

                          if (camera.getPosition() != camaraAnterior) {

                          camaraAnterior = camera.getPosition();
                          parametrosBusq = new SearchParameters(new Point(((int)meshVillano.Position.X), ((int)meshVillano.Position.Z)), new Point(((int)camera.Position.X), ((int)camera.Position.Z)), Aux.mapBool);


                        Astar = new CalculadoraDeTrayecto(parametrosBusq, Aux.nodes);



                    path = Astar.FindPath(new Point(((int)camera.Position.X), ((int)camera.Position.Z)));
                }
                  }

                          if (path.Count != 0)
                          {
                              Vector3 proximoLugar = new Vector3(path.Find(punti => punti.X == punti.X).X, 5, path.Find(punti => punti.X == punti.X).Y);
                              path.Remove(path.Find(punti => punti.X == punti.X));
                              meshVillano.Position = proximoLugar;
                          }
                          Vector3 direction2 = Vector3.Normalize(newPosition - meshVillano.Position);
                          float angle = FastMath.Acos(Vector3.Dot(originalMeshRot, direction2));
                          Vector3 axisRotation = Vector3.Cross(originalMeshRot, direction2);
                          meshVillano.Rotation = axisRotation * angle;

                      

                      contadorFrames = contadorFrames + 1;
         /*
            //Rotar modelo en base a la nueva dirección a la que apunta
            Vector3 direction2 = Vector3.Normalize(newPosition - meshVillano.Position);
                        float angle = FastMath.Acos(Vector3.Dot(originalMeshRot, direction2));
                        Vector3 axisRotation = Vector3.Cross(originalMeshRot, direction2);
                        meshVillano.Rotation = axisRotation * angle;
                      //  meshRotationMatrix = Matrix.RotationAxis(axisRotation, angle);




                        //Ver si queda algo de distancia para mover
                        Vector3 posDiff = newPosition - meshVillano.Position;
                        float posDiffLength = posDiff.LengthSq();
                        if (posDiffLength > float.Epsilon)
                        {
                            //Movemos el mesh interpolando por la velocidad
                            float currentVelocity = 50f * elapsedTime;
                            posDiff.Normalize();
                            posDiff.Multiply(currentVelocity);

                            //Ajustar cuando llegamos al final del recorrido
                            Vector3 newPos = meshVillano.Position + posDiff;
                            if (posDiff.LengthSq() > posDiffLength)
                            {
                                newPos = newPosition;
                            }

                            bool collisionVillanoPared = false;
                            Vector3 diferenciaPosicion = new Vector3();
                            foreach (TgcMesh mesh in meshes)
                            {
                                //Los dos BoundingBox que vamos a testear
                                TgcBoundingBox sceneMeshBoundingBox = mesh.BoundingBox;

                                //Hubo colisión con un objeto. Guardar resultado y abortar loop.
                                if (TgcCollisionUtils.testAABBAABB(meshVillano.BoundingBox, sceneMeshBoundingBox))
                                {
                                    collisionVillanoPared = true;
                                    meshVillano.Position = posicionOriginalVillano;
                                    break;

                                }

                            }
                            if (!collisionVillanoPared)
                            {
                                Vector3 posicionMarca = meshVillano.Position;
                                meshVillano.Position = newPos;
                                meshVillano.AutoTransformEnable = true;
                                bool collisionVillanoParedAux = false;
                                foreach (TgcMesh mesh in meshes)
                                {
                                    TgcBoundingBox sceneMeshBoundingBox2 = mesh.BoundingBox;
                                    if (TgcCollisionUtils.testAABBAABB(meshVillano.BoundingBox, sceneMeshBoundingBox2))
                                    {
                                        collisionVillanoParedAux = true;

                                    }

                                }

                                if (collisionVillanoParedAux)
                                {
                                    posicionOriginalVillano = posicionMarca;
                                }
                                else posicionOriginalVillano = camera.Position;

                            }
                        }
                       
    */

                    //Hubo colisión con un objeto. Guardar resultado y abortar loop

                    //Hubo colisión con un objeto. Guardar resultado y abortar loop.
                    if (TgcCollisionUtils.testSphereAABB(sphere, meshVillano.BoundingBox))  //(meshVillano.BoundingBox, sceneMeshBoundingBox))
                    {
                        collisionVillanoCamara = true;
                        
                    }
              

                if (collisionVillanoCamara)
                {

                meshVillano.Position = new Vector3(289, 5, 577);
                   
                }









            ///////////////////////////// FIN MOVIMIENTO VILLANO/////////////////////////////////



            ///////////////////////////////////////////// FIN PARA EL VILLANO  ///////////////////////////////////////////////////////////



            ///// PUERTA 1.0

            puerta.render();

            if (input.keyUp(Key.R))
            {
                if (!open)
                {
                    puerta.Transform = transAbrePuerta(elapsedTime * 0.01f); //prueba para bajar el tiempo y que la animacion quede mejor
                    open = true;
                    puerta.render();

                }

                else
                {
                    puerta.Transform = transCierraPuerta(elapsedTime * 0.01f);
                    open = false;
                    puerta.render();

                }

            }


          


            ///// PUERTA 1.0

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
                GuiController.Instance.UserVars.setValue("PosicionX", camera.getPosition().X);
                GuiController.Instance.UserVars.setValue("PosicionY", camera.getPosition().Y);
                GuiController.Instance.UserVars.setValue("PosicionZ", camera.getPosition().Z);


              
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


        private Matrix transAbrePuerta(float elapsedTime)
        {
            Matrix translate = Matrix.Translation(new Vector3(20, 0, -20));
            float angleY = FastMath.ToRad(90);
            Matrix rotation = Matrix.RotationYawPitchRoll(angleY, 0, 0);
            return rotation * translate;
        }

        private Matrix transCierraPuerta(float elapsedTime)
        {
            Matrix translate = Matrix.Translation(new Vector3(0, 0, 0));
            float angleY = FastMath.ToRad(90);
            return translate;
        }

        public override void close()
        {
            
            foreach (TgcMesh mesh in meshes)
            {
                mesh.dispose();
            }

            sphere.dispose();
            meshVillano.dispose();
            puerta.dispose();
        }

    }
}
