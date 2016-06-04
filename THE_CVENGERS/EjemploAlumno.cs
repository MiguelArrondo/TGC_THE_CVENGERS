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
using System.IO;
using TgcViewer.Utils.TgcSkeletalAnimation;
using TgcViewer.Utils._2D;

namespace AlumnoEjemplos.THE_CVENGERS
{

    

    public class Juego : TgcExample
    {

        string selectedAnim;
        TgcSkeletalMesh meshVillano;


        string mediaPath;
        string[] animationsPath;

        CalculadoraDeTrayecto Aux = new CalculadoraDeTrayecto();
        SearchParameters parametrosBusq;
        Vector3 camaraAnterior = new Vector3(0, 0, 0);
        List<Point> path = new List<Point>();
        int contadorFrames = 0;
        Microsoft.DirectX.Direct3D.Effect currentShader;
        //Con luz: Cambiar el shader actual por el shader default que trae el framework para iluminacion dinamica con PointLight
        Microsoft.DirectX.Direct3D.Effect skeletalShader;

        int tipoLuz;
       
        CalculadoraDeTrayecto Astar;

   

        const float MOVEMENT_SPEED = 400f;
        FPSCustomCamera camera = new FPSCustomCamera();

        List<TgcMesh> meshes;

        bool tengoLuz = true;
        
        //Variable para esfera
        TgcBoundingSphere sphere;
        TgcBoundingSphere spherePuertas;


        List<Puerta> listaPuertas;


        TgcMesh meshIluminacion;

        LightManager lightManager = new LightManager();




        //PARA EL VILLANO

        Vector3 newPosition;
        Vector3 originalMeshRot;

        List<Point> caminoVillano;
        List<Point> listaPuntosAux;

        bool villanoPersiguiendo = false;
        TgcBoundingSphere esferaVillano;

        Cama cama1;

        Puerta puertaSelecionada;
        bool abriendoPuerta;
        int contadorAbertura;
        int contadorAberturaVillano = 0;

        TgcSprite spritePuerta;

        List<Puerta> puertasAbiertasVillano = new List<Puerta>();
        List<Puerta> puertasAbiertasVillanoAux = new List<Puerta>();
        bool abriendoPuertaVillano;
        Puerta puertaVillano;

        TgcBoundingSphere esferaVillanoPuertas = new TgcBoundingSphere();


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
            spherePuertas = new TgcBoundingSphere(new Vector3(160, 60, 240), 60f);

            //Activamos el renderizado customizado. De esta forma el framework nos delega control total sobre como dibujar en pantalla
            //La responsabilidad cae toda de nuestro lado
            GuiController.Instance.CustomRenderEnabled = true;

            //Path para carpeta de texturas de la malla
            mediaPath = GuiController.Instance.ExamplesMediaDir + "SkeletalAnimations\\BasicHuman\\";


            Color myArgbColor = Color.FromArgb(15, 15, 15);


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
            TgcScene scene = loader.loadSceneFromFile(GuiController.Instance.AlumnoEjemplosDir + "THE_CVENGERS\\AlumnoMedia\\mapaDef-TgcScene.xml");
            meshes = scene.Meshes;

            //foreach(TgcMesh meshScene in meshes)
          //  {
         //       meshScene.Scale = new Vector3(2, 2, 2);
         //   }

            Aux.map = scene;
            Aux.personaje = meshVillano;
            Aux.analizarPuntosPared();
            Aux.InitializeNodes(Aux.mapBool);

            esferaVillano = new TgcBoundingSphere(new Vector3(0, 0, 0), 135f);
            esferaVillanoPuertas = new TgcBoundingSphere(new Vector3(0, 0, 0), 50f);

            //Crear una UserVar
            GuiController.Instance.UserVars.addVar("PosicionX");
            GuiController.Instance.UserVars.addVar("PosicionY");
            GuiController.Instance.UserVars.addVar("PosicionZ");
            GuiController.Instance.UserVars.addVar("Paths");

            tipoLuz = 1;
          
           meshIluminacion = lightManager.Init(tipoLuz);

    

        
            originalMeshRot = new Vector3(0, 0, -1);


            camera.Enable = true;

            camera.setCamera(new Vector3(609, 45, 921), new Vector3(500, 0, 1));

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



            caminoVillano = PathInitializer.crearPathVerde();
            listaPuntosAux = new List<Point>();


            PuertaManager pepe = new PuertaManager();

            listaPuertas = pepe.initPuertas();

            foreach (Puerta puerta in listaPuertas)
            {
                puerta.getMesh().Effect = currentShader;
                //El Technique depende del tipo RenderType del mesh
                puerta.getMesh().Technique = GuiController.Instance.Shaders.getTgcMeshTechnique(puerta.getMesh().RenderType);
            }

            cama1 = new Cama(new Vector3(335, 5, 615), 80f);
            cama1.getMesh().Effect = currentShader;
            //El Technique depende del tipo RenderType del mesh
            cama1.getMesh().Technique = GuiController.Instance.Shaders.getTgcMeshTechnique(cama1.getMesh().RenderType);
            cama1.getMesh().BoundingBox.transform(cama1.getMesh().Transform);

            spritePuerta = new TgcSprite();
            spritePuerta.Texture = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosDir + "THE_CVENGERS\\AlumnoMedia\\puertitaIcono.png");
            Size screenSize = GuiController.Instance.Panel3d.Size;
            Size textureSize = spritePuerta.Texture.Size;
            
            spritePuerta.Position = new Vector2(FastMath.Max(screenSize.Width / 2 - textureSize.Width / 2, 0), FastMath.Max(screenSize.Height / 8 - textureSize.Height / 8, 0));
        }
       

        public override void render(float elapsedTime)
        {
            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;
            TgcD3dInput input = GuiController.Instance.D3dInput;
            sphere.setCenter(camera.getPosition());
            spherePuertas.setCenter(camera.getPosition());

            d3dDevice.BeginScene();
            meshIluminacion.Transform = lightManager.getMatriz(camera, tipoLuz);

            cama1.Render();

            if (tengoLuz)
            {

                meshIluminacion.render();
                lightManager.renderLuces(camera, meshes, tengoLuz, tipoLuz,listaPuertas);
            }
            else
            {
                lightManager.renderLuces(camera, meshes, tengoLuz, tipoLuz,listaPuertas);
            }
                
           

           if (input.keyUp(Key.V))
            {
                if (tengoLuz)
                {
                    tengoLuz = false;
                }
                else { tengoLuz = true; }


            }

 

            


            if (input.keyUp(Key.T))
            {
                switch (tipoLuz) {
                    case 1:
                        tipoLuz = 2;
                        meshIluminacion=lightManager.changeMesh(meshIluminacion, 2);
                        break;
                    case 2:
                        tipoLuz = 1;
                        meshIluminacion=lightManager.changeMesh(meshIluminacion, 1);
                        break;
                }
               
            }
          

            /////////////////////////////////////////////  PARA EL VILLANO  ///////////////////////////////////////////////////////////

            meshVillano = lightManager.shaderVillano(meshVillano, skeletalShader, camera);

            meshVillano.updateAnimation();
            meshVillano.render();


            ///////////////////////////// MOVIMIENTO VILLANO/////////////////////////////////
            esferaVillano.render();
            esferaVillano.setCenter(meshVillano.Position);
            esferaVillanoPuertas.setCenter(meshVillano.Position);

            bool collisionVillanoCamara = false;


            newPosition.X = camera.getPosition().X;
            newPosition.Y = 5;
            newPosition.Z = camera.getPosition().Z;

            if (contadorFrames == 0)
            {
                meshVillano.Position = new Vector3(331, 5, 366);
            }

            if (!villanoPersiguiendo)
            {



                if (caminoVillano.Count != 0)
                {
                    Vector3 proximoLugar = new Vector3(caminoVillano.Find(punti => punti.X == punti.X).X, 5, caminoVillano.Find(punti => punti.X == punti.X).Y);
                    listaPuntosAux.Add(caminoVillano.Find(punti => punti.X == punti.X));
                    caminoVillano.Remove(caminoVillano.Find(punti => punti.X == punti.X));


                    Vector3 direction2 = Vector3.Normalize(proximoLugar - meshVillano.Position);
                    if (direction2.Z > 0)
                    {
                        float angle = FastMath.Acos(Vector3.Dot(originalMeshRot, direction2));
                        Vector3 axisRotation = Vector3.Cross(originalMeshRot, direction2);
                        meshVillano.Rotation = axisRotation * angle;
                        meshVillano.rotateY(135);
                    }
                    else
                    {
                        float angle = FastMath.Acos(Vector3.Dot(originalMeshRot, direction2));
                        Vector3 axisRotation = Vector3.Cross(originalMeshRot, direction2);
                        meshVillano.Rotation = axisRotation * angle;
                    }

                       meshVillano.Position = proximoLugar;
                }
                else
                {
                    caminoVillano = listaPuntosAux;
                }


            }
            else
            {


                if (Math.Abs((newPosition - meshVillano.Position).X) > 350 || Math.Abs((newPosition - meshVillano.Position).Z) > 350)
                {
                    villanoPersiguiendo = false;
                    listaPuntosAux.Clear();
                    caminoVillano = PathInitializer.crearPathRojo();

                }



                if (camera.getPosition() != camaraAnterior)
                    {

                        camaraAnterior = camera.getPosition();
                        parametrosBusq = new SearchParameters(new Point(((int)meshVillano.Position.X), ((int)meshVillano.Position.Z)), new Point(((int)camera.Position.X), ((int)camera.Position.Z)), Aux.mapBool);


                        Astar = new CalculadoraDeTrayecto(parametrosBusq, Aux.nodes);



                        path = Astar.FindPath(new Point(((int)camera.Position.X), ((int)camera.Position.Z)));
                    CalculadoraDeTrayecto.resetearNodos();
                    }
                

                if (path.Count != 0)
                {



                    Vector3 proximoLugar = new Vector3(path.Find(punti => punti.X == punti.X).X, 5, path.Find(punti => punti.X == punti.X).Y);
                    path.Remove(path.Find(punti => punti.X == punti.X));
                    meshVillano.Position = proximoLugar;


                }

                Vector3 direction2 = Vector3.Normalize(newPosition - meshVillano.Position);
                if (direction2.Z > 0 && direction2.Z > Math.Abs(direction2.X))
                {
                    float angle = FastMath.Acos(Vector3.Dot(originalMeshRot, direction2));
                    Vector3 axisRotation = Vector3.Cross(originalMeshRot, direction2);
                    meshVillano.Rotation = axisRotation * angle;
                    meshVillano.rotateY(135);
                }
                else
                {
                    float angle = FastMath.Acos(Vector3.Dot(originalMeshRot, direction2));
                    Vector3 axisRotation = Vector3.Cross(originalMeshRot, direction2);
                    meshVillano.Rotation = axisRotation * angle;
                }

            }


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

                    meshVillano.Position = new Vector3(565, 5, 84);
                   
                }









            ///////////////////////////// FIN MOVIMIENTO VILLANO/////////////////////////////////



            ///////////////////////////////////////////// FIN PARA EL VILLANO  ///////////////////////////////////////////////////////////

            ///// PUERTAS

            foreach (Puerta puerta in listaPuertas)
            {
                puerta.getMesh().render();

            }


            ///// PUERTAS

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
                GuiController.Instance.UserVars.setValue("Paths", path.Count);


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

            foreach (Puerta door in listaPuertas)
            {
                if (!abriendoPuerta)
                {
                    if (TgcCollisionUtils.testSphereAABB(sphere, door.getMesh().BoundingBox))
                    {
                        collisionFound = true;
                        break;
                    }
                }

                if (!villanoPersiguiendo)
                {
                    if (!door.getStatus() && door.contadorVillano == 0 && TgcCollisionUtils.testSphereAABB(esferaVillanoPuertas, door.getMesh().BoundingBox))
                    {
                        if (!door.villanoAbriendoPrimera || door.villanoAbriendoSiguientes)
                        {
                            door.villanoAbriendoSiguientes = false;
                            puertasAbiertasVillano.Add(door);
                            door.villanoAbriendoPrimera = true;
                            door.contadorVillano = 0;
                        }
                    }
                    if (!TgcCollisionUtils.testSphereAABB(esferaVillanoPuertas, door.getMesh().BoundingBox) && door.villanoAbriendoPrimera)
                    {
                        door.villanoAbriendoSiguientes = true;
                    }
                }
            }

           
            foreach(Puerta door in puertasAbiertasVillano)
            {

                
                

                    if (door.contadorVillano < 100 && !door.getStatus())
                    {

                        door.accionarPuerta();
                        door.contadorVillano++;
                    }
                    else
                    {
                        if (door.contadorVillano == 100)
                            door.cambiarStatus();

                        if (door.contadorVillano > 0)
                        {
                            door.accionarPuerta();
                            door.contadorVillano--;
                        }
                        else
                        {
                            door.cambiarStatus();
                            puertasAbiertasVillanoAux.Add(door);
                        }
                    }

                
            }

            foreach(Puerta door in puertasAbiertasVillanoAux)
            {
                puertasAbiertasVillano.Remove(door);
            }

            puertasAbiertasVillanoAux.Clear();
         

            cama1.getMesh().BoundingBox.render();

                if (TgcCollisionUtils.testSphereAABB(sphere, cama1.getMesh().BoundingBox))
            {
                collisionFound = true;
            }

            //Si hubo alguna colisión, entonces restaurar la posición original del mesh
            if (collisionFound)
                {


                    // camera.ViewMatrix = view;
                    camera.setearCamara(originalPos, originalLook, view, x, y, z, direction);


                }



            if (TgcCollisionUtils.testSphereSphere(esferaVillano, sphere))
            {
                villanoPersiguiendo = true;
            }

            foreach (Puerta puerta in listaPuertas)
            {
                if (TgcCollisionUtils.testSphereAABB(spherePuertas, puerta.getMesh().BoundingBox))
                {
                    GuiController.Instance.Drawer2D.beginDrawSprite();

                    spritePuerta.render();

                    GuiController.Instance.Drawer2D.endDrawSprite();

                    if (input.keyUp(Key.E))
            {

                        if (!abriendoPuerta)
                        {

                            puertaSelecionada = puerta;
                            abriendoPuerta = true;
                            contadorAbertura = 0;
                            break;
                        }
                    }

                }
            }

            if (contadorFrames % 5 == 0 && abriendoPuerta)
            {

                if (abriendoPuerta && contadorAbertura < 100)
                {
                    
                    puertaSelecionada.accionarPuerta();
                    contadorAbertura++;
                }
                else
                {
                    abriendoPuerta = false;
                    puertaSelecionada.cambiarStatus();
                }

            }

            if (abriendoPuerta)
            {
                camera.Enable = false;
            }
            else
            {
                camera.Enable = true;
            }

        }


        public override void close()
        {
            
            foreach (TgcMesh mesh in meshes)
            {
                mesh.dispose();
            }

            foreach (Puerta puerta in listaPuertas)
            {
                puerta.getMesh().dispose();

            }

            meshIluminacion.dispose();
            sphere.dispose();
            meshVillano.dispose();
            esferaVillano.dispose();
            spherePuertas.dispose();
            
        }



        

    }
}
