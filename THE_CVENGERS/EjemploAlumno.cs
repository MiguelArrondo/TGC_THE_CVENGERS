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
using TgcViewer.Utils.Sound;
using TgcViewer.Utils.Interpolation;

namespace AlumnoEjemplos.THE_CVENGERS
{

    

    public class Juego : TgcExample
    {

        string selectedAnim;
        string selectedAnim2;
        TgcSkeletalMesh meshVillano;

        TgcScene scene;

        string mediaPath;
        string[] animationsPath;

        CalculadoraDeTrayecto Aux = new CalculadoraDeTrayecto();
        SearchParameters parametrosBusq;
        Vector3 camaraAnterior = new Vector3(0, 0, 0);
        List<Point> path = new List<Point>();
        int contadorFrames = 0;
        Microsoft.DirectX.Direct3D.Effect currentShader2;
        //Con luz: Cambiar el shader actual por el shader default que trae el framework para iluminacion dinamica con PointLight
        Microsoft.DirectX.Direct3D.Effect skeletalShader;

        int tipoLuz;
        float tiempoVela;
        float tiempoLinterna;
        float tiempoLampara;
        bool velaRota;
        bool linternaRota;
        bool lamparaRota;

        float tiempoVillano = 0;
        float tiempoVillanoPath = 0;
        float tiempoPuertaVillano = 0;
        float tiempoPuerta = 0;
        float tiempo = 0;


        CalculadoraDeTrayecto Astar;

        TgcStaticSound sonidoPuerta = new TgcStaticSound();
        TgcStaticSound sonidoPasos = new TgcStaticSound();
        TgcStaticSound sonidoEscondite = new TgcStaticSound();
        TgcStaticSound sonidoMonstruo = new TgcStaticSound();
        TgcStaticSound sonidoFoto = new TgcStaticSound();
        TgcStaticSound sonidoFoto2 = new TgcStaticSound();
        TgcStaticSound sonidoFoto3 = new TgcStaticSound();
        TgcStaticSound sonidoRespiracion = new TgcStaticSound();
        TgcStaticSound sonidoMuerte = new TgcStaticSound();
        TgcMp3Player musica = new TgcMp3Player();
        TgcMp3Player musicaInicial = new TgcMp3Player();

        const float MOVEMENT_SPEED = 400f;
        FPSCustomCamera camera = new FPSCustomCamera();

        List<TgcMesh> meshes;

        bool tengoLuz = false;
        
        //Variable para esfera
        TgcBoundingSphere sphere;
        TgcBoundingSphere spherePuertas;
        TgcBoundingSphere sphereEscondites;


        List<Puerta> listaPuertas;
        List<Objeto> listaObjetos;
        List<Escondite> listaEscondites;
        List<Objeto> listaFotos;

        Objeto flashlight;
        Objeto candle;
        Objeto lantern;


        TgcMesh meshIluminacion;

        LightManager lightManager = new LightManager();
        List<Lampara> listaLamparas;



        //PARA EL VILLANO

        Vector3 newPosition;
        Vector3 originalMeshRot;

        List<Point> caminoVillano;
        List<Point> listaPuntosAux;

        bool villanoPersiguiendo = false;
        TgcBoundingSphere esferaVillano;



        Puerta puertaSelecionada;
        bool abriendoPuerta;
        int contadorAbertura;

        bool sinEsconderse = true;
        Vector3 posicionPrevia;
        Vector3 lookAtPrevio;
        bool luzAnterior;

        TgcSprite spritePuerta;
        TgcSprite pantallaInicio;
        TgcSprite pantallaHistoria;
        TgcSprite pantallaInstrucciones;
        TgcSprite pantallaMuerte;
        TgcSprite pantallaEscondido;
        TgcSprite pantallaObjetivo;
        TgcSprite pantallaGanaste;
        TgcSprite keyHole;
        TgcSprite iconoFoto;
        TgcSprite iconoMano;

        List<Puerta> puertasAbiertasVillano = new List<Puerta>();
        List<Puerta> puertasAbiertasVillanoAux = new List<Puerta>();


        TgcBoundingSphere esferaVillanoPuertas = new TgcBoundingSphere();


        int contadorFotos = 0;
        int fotoActual = 0;

        bool respiracion;

        bool muerte = false;

        bool tengoLinterna = false;
        bool tengoVela = false;
        bool tengoLampara = false;

        bool reproducirMusica = true;
        bool inicioJuego = true;
        int contadorPantalla = 0;

        bool pantallaInicioBool = false;
        bool pantallaInstruccionesBool = false;
        bool pantallaHistoriaBool = false;
        bool meshIluminacionBool = false;
        bool iconoFotoBool = false;
        bool keyHoleBool = false;
        bool pantallaEscondidoBool = false;
        bool spritePuertaBool = false;
        bool iconoManoBool = false;
        bool pantallaMuerteBool = false;
        bool pantallaObjetivoBool = false;
        bool ganaste = false;
        bool pantallaGanasteBool = false;
        TgcSprite iconoInventario;
        TgcSprite iconoMapa;
        TgcSprite mapa;
        bool iconoMapaBool = false;
        bool iconoInventarioBool = false;
        bool mapaAbierto = false;
        bool invAbierto = false;

        VertexBuffer screenQuadVB;
        Texture renderTarget2D;
        Surface pOldRT;
        Microsoft.DirectX.Direct3D.Effect effect;
        TgcTexture alarmTexture;
        InterpoladorVaiven intVaivenAlarm;
        

        public override string getCategory()
        {
            return "THE_CVENGERS";
        }

        public override string getName()
        {
            return "The Orphanage";
        }

        public override string getDescription()
        {
            return "The Orphanage... a survival horror videogame made by Miguel Arrondo & Leandro Wagner.";
        }


        public override void init()
        {
           

            //Creamos caja de colision
            sphere = new TgcBoundingSphere(new Vector3(0, 0, 0), 20f);
            spherePuertas = new TgcBoundingSphere(new Vector3(160, 60, 240), 60f);
            sphereEscondites = new TgcBoundingSphere(new Vector3(160, 60, 240), 30f);

            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;

            //Activamos el renderizado customizado. De esta forma el framework nos delega control total sobre como dibujar en pantalla
            //La responsabilidad cae toda de nuestro lado
            GuiController.Instance.CustomRenderEnabled = true;

            //Path para carpeta de texturas de la malla
            mediaPath = GuiController.Instance.ExamplesMediaDir + "SkeletalAnimations\\BasicHuman\\";


            Color myArgbColor = Color.FromArgb(15, 15, 15);


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
            selectedAnim = animationList[9];          

            selectedAnim2 = animationList[6];

            TgcSkeletalLoader loaderVillano = new TgcSkeletalLoader();
            meshVillano = loaderVillano.loadMeshAndAnimationsFromFile(GuiController.Instance.AlumnoEjemplosDir + "THE_CVENGERS\\AlumnoMedia\\CS_Gign-TgcSkeletalMesh.xml", GuiController.Instance.AlumnoEjemplosDir + "THE_CVENGERS\\AlumnoMedia\\", animationsPath);

            //Crear esqueleto a modo Debug
            meshVillano.buildSkletonMesh();

            meshVillano.move(new Vector3(289, 5, 577));//(628, 10, 51);

            meshVillano.playAnimation(selectedAnim, true);


          

            //Cargamos un escenario
            TgcSceneLoader loader = new TgcSceneLoader();
            scene = loader.loadSceneFromFile(GuiController.Instance.AlumnoEjemplosDir + "THE_CVENGERS\\AlumnoMedia\\mapaDef-TgcScene.xml");
            meshes = scene.Meshes;

            //foreach(TgcMesh meshScene in meshes)
            //  {
            //       meshScene.Scale = new Vector3(2, 2, 2);
            //   }

            ObjetosManager carlos = new ObjetosManager();

            listaObjetos = carlos.initObjetos();

            Aux.map = scene;
            Aux.objetosMapa = listaObjetos;
            Aux.personaje = meshVillano;
            Aux.analizarPuntosPared();
            Aux.InitializeNodes(Aux.mapBool);


            esferaVillano = new TgcBoundingSphere(new Vector3(0, 0, 0), 135f);
            esferaVillanoPuertas = new TgcBoundingSphere(new Vector3(0, 0, 0), 50f);

            //Crear una UserVar
            GuiController.Instance.UserVars.addVar("PosicionX");
            GuiController.Instance.UserVars.addVar("PosicionY");
            GuiController.Instance.UserVars.addVar("PosicionZ");

            tipoLuz = 1;
          
           meshIluminacion = lightManager.Init(tipoLuz);

            tipoLuz = 0;

        
            originalMeshRot = new Vector3(0, 0, -1);


            camera.Enable = true;



           // currentShader = TgcShaders.loadEffect(GuiController.Instance.AlumnoEjemplosDir + "THE_CVENGERS\\AlumnoMedia\\Shaders\\MeshSpotLightShader.fx");
            currentShader2 = TgcShaders.loadEffect(GuiController.Instance.AlumnoEjemplosDir + "THE_CVENGERS\\AlumnoMedia\\Shaders\\MultiDiffuseLightsCustom.fx");

            //Aplicar a cada mesh el shader actual
            foreach (TgcMesh mesh in meshes)
            {
                mesh.Effect = currentShader2;
                //El Technique depende del tipo RenderType del mesh
                mesh.Technique = "MultiDiffuseLightsTechnique";
            }

           

            skeletalShader = GuiController.Instance.Shaders.TgcSkeletalMeshPointLightShader;

            
            meshVillano.Technique = GuiController.Instance.Shaders.getTgcSkeletalMeshTechnique(meshVillano.RenderType);



            caminoVillano = PathInitializer.crearPathRojo();
            listaPuntosAux = new List<Point>();


            PuertaManager pepe = new PuertaManager();

            listaPuertas = pepe.initPuertas();

            foreach (Puerta puerta in listaPuertas)
            {
                puerta.getMesh().Effect = currentShader2;
                //El Technique depende del tipo RenderType del mesh
                puerta.getMesh().Technique = "MultiDiffuseLightsTechnique";
            }


            foreach (Objeto obj in listaObjetos)
            {
                obj.getMesh().Effect = currentShader2;
                //El Technique depende del tipo RenderType del mesh
                obj.getMesh().Technique = "MultiDiffuseLightsTechnique";
            }

            listaFotos = carlos.initFotos();

            foreach (Objeto fot in listaFotos)
            {
                fot.getMesh().Effect = currentShader2;
                //El Technique depende del tipo RenderType del mesh
                fot.getMesh().Technique = "MultiDiffuseLightsTechnique";
            }

            listaEscondites = carlos.initEscondites();

            foreach (Escondite hide in listaEscondites)
            {
                hide.getMesh().Effect = currentShader2;
                //El Technique depende del tipo RenderType del mesh
                hide.getMesh().Technique = "MultiDiffuseLightsTechnique";
            }

            listaLamparas = lightManager.initLamparas();

            foreach (Lampara lamp in listaLamparas)
            {
                lamp.getMesh().Effect = TgcShaders.loadEffect(GuiController.Instance.AlumnoEjemplosDir + "THE_CVENGERS\\AlumnoMedia\\ObjetosIluminacion\\ShaderObjetos.fx");
                lamp.getMesh().Technique = "Darkening";
                lamp.getMesh().Effect.SetValue("darkFactor", (float)0.35f);
            }

            candle = carlos.initItems().Find(item => item.nombre == "candle-TgcScene.xml");
            candle.getMesh().Effect = currentShader2;
            //El Technique depende del tipo RenderType del mesh
            candle.getMesh().Technique = "MultiDiffuseLightsTechnique";

            flashlight = carlos.initItems().Find(item => item.nombre == "flashlight-TgcScene.xml");
            flashlight.getMesh().Effect = currentShader2;
            //El Technique depende del tipo RenderType del mesh
            flashlight.getMesh().Technique = "MultiDiffuseLightsTechnique";

            lantern = carlos.initItems().Find(item => item.nombre == "lantern-TgcScene.xml");
            lantern.getMesh().Effect = currentShader2;
            //El Technique depende del tipo RenderType del mesh
            lantern.getMesh().Technique = "MultiDiffuseLightsTechnique";





            camera.setCamera(new Vector3(609, 45, 921), new Vector3(500, 0, 1), scene, listaPuertas, listaObjetos, listaEscondites);

            Size screenSize = GuiController.Instance.Panel3d.Size;

            spritePuerta = new TgcSprite();
            spritePuerta.Texture = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosDir + "THE_CVENGERS\\AlumnoMedia\\puertitaIcono.png");
            spritePuerta.Scaling = new Vector2(0.3f, 0.3f);
            Size textureSizePuerta = spritePuerta.Texture.Size;           
            spritePuerta.Position = new Vector2(FastMath.Max(screenSize.Width / 2 - spritePuerta.Texture.Height * 0.3f, 0), FastMath.Max(screenSize.Height / 2 + spritePuerta.Texture.Height * 0.45f, 0));

            pantallaInicio = new TgcSprite();
            pantallaInicio.Texture = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosDir + "THE_CVENGERS\\AlumnoMedia\\PSD\\intPORTADA.png");
            pantallaInicio.Scaling = new Vector2(0.58f, 0.74f);
            pantallaInicio.Position = new Vector2(FastMath.Max(0, 0), FastMath.Max(0, 0));

            pantallaMuerte = new TgcSprite();
            pantallaMuerte.Texture = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosDir + "THE_CVENGERS\\AlumnoMedia\\PSD\\pantallaDIED.png");
            pantallaMuerte.Scaling = new Vector2(0.58f, 0.74f);
            pantallaMuerte.Position = new Vector2(FastMath.Max(0, 0), FastMath.Max(0, 0));

            pantallaHistoria = new TgcSprite();
            pantallaHistoria.Texture = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosDir + "THE_CVENGERS\\AlumnoMedia\\PSD\\intTEXT2.png");
            pantallaHistoria.Scaling = new Vector2(0.58f, 0.74f);
            pantallaHistoria.Position = new Vector2(FastMath.Max(0, 0), FastMath.Max(0, 0));

            pantallaEscondido = new TgcSprite();
            pantallaEscondido.Texture = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosDir + "THE_CVENGERS\\AlumnoMedia\\PSD\\intKEYHOLE.png");
            pantallaEscondido.Scaling = new Vector2(0.58f, 0.74f);
            pantallaEscondido.Position = new Vector2(FastMath.Max(0, 0), FastMath.Max(0, 0));


            pantallaInstrucciones = new TgcSprite();
            pantallaInstrucciones.Texture = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosDir + "THE_CVENGERS\\AlumnoMedia\\PSD\\intCONTROLS.png");
            pantallaInstrucciones.Scaling = new Vector2(0.58f, 0.74f);
            pantallaInstrucciones.Position = new Vector2(FastMath.Max(0, 0), FastMath.Max(0, 0));

            pantallaObjetivo = new TgcSprite();
            pantallaObjetivo.Texture = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosDir + "THE_CVENGERS\\AlumnoMedia\\PSD\\intBEWARE.png");
            pantallaObjetivo.Scaling = new Vector2(0.58f, 0.74f);
            pantallaObjetivo.Position = new Vector2(FastMath.Max(0, 0), FastMath.Max(0, 0));

            pantallaGanaste = new TgcSprite();
            pantallaGanaste.Texture = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosDir + "THE_CVENGERS\\AlumnoMedia\\PSD\\pantallaWIN2.png");
            pantallaGanaste.Scaling = new Vector2(0.58f, 0.74f);
            pantallaGanaste.Position = new Vector2(FastMath.Max(0, 0), FastMath.Max(0, 0));

            keyHole = new TgcSprite();
            keyHole.Texture = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosDir + "THE_CVENGERS\\AlumnoMedia\\intHIDE.png");
            keyHole.Scaling = new Vector2(0.3f, 0.3f);
            Size textureSizeKey = keyHole.Texture.Size;
            keyHole.Position = new Vector2(FastMath.Max(screenSize.Width / 2 - keyHole.Texture.Height * 0.3f, 0), FastMath.Max(screenSize.Height / 2 + keyHole.Texture.Height * 0.45f, 0));

            iconoMapa = new TgcSprite();
            iconoMapa.Texture = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosDir + "THE_CVENGERS\\AlumnoMedia\\PSD\\intMAPinGAME.png");
            iconoMapa.Scaling = new Vector2(0.3f, 0.3f);
            Size textureSizeMap = iconoMapa.Texture.Size;
            iconoMapa.Position = new Vector2(FastMath.Max(screenSize.Width - iconoMapa.Texture.Width*0.3f, 0), FastMath.Max(0, 0));

            iconoInventario = new TgcSprite();
            iconoInventario.Texture = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosDir + "THE_CVENGERS\\AlumnoMedia\\PSD\\intINVENTORYinGAME.png");
            iconoInventario.Scaling = new Vector2(0.3f, 0.3f);
            Size textureSizeInv = iconoInventario.Texture.Size;
            iconoInventario.Position = new Vector2(FastMath.Max(screenSize.Width - iconoInventario.Texture.Width * 0.3f, 0), FastMath.Max(iconoMapa.Texture.Height * 0.3f, 0));

            iconoFoto = new TgcSprite();
            iconoFoto.Texture = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosDir + "THE_CVENGERS\\AlumnoMedia\\camera icon.png");
            iconoFoto.Scaling = new Vector2(0.3f, 0.3f);
            Size textureSizeCam = iconoFoto.Texture.Size;
            iconoFoto.Position = new Vector2(FastMath.Max(screenSize.Width / 2 - iconoFoto.Texture.Height * 0.3f, 0), FastMath.Max(screenSize.Height / 2 + iconoFoto.Texture.Height * 0.45f, 0));

            iconoMano = new TgcSprite();
            iconoMano.Texture = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosDir + "THE_CVENGERS\\AlumnoMedia\\hand.png");
            iconoMano.Scaling = new Vector2(0.3f, 0.3f);
            Size textureSizeHand = iconoMano.Texture.Size;
            iconoMano.Position = new Vector2(FastMath.Max(screenSize.Width / 2 - iconoMano.Texture.Height * 0.3f, 0), FastMath.Max(screenSize.Height / 2 + iconoMano.Texture.Height * 0.45f, 0));

            mapa = new TgcSprite();
            mapa.Texture = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosDir + "THE_CVENGERS\\AlumnoMedia\\mapa tgcTransparente.png");
            mapa.Scaling = new Vector2(0.5f, 0.5f);
            Size textureSizeMapa = mapa.Texture.Size;
            mapa.Position = new Vector2(FastMath.Max(screenSize.Width / 2 - mapa.Texture.Width / 2 * 0.5f,0), FastMath.Max(screenSize.Height / 2 - mapa.Texture.Height/2*0.5f, 0));

            sonidoPuerta.loadSound(GuiController.Instance.AlumnoEjemplosDir + "THE_CVENGERS\\AlumnoMedia\\Sonidos\\door creaks open   sound effect.wav");
            sonidoPasos.loadSound(GuiController.Instance.AlumnoEjemplosDir + "THE_CVENGERS\\AlumnoMedia\\Sonidos\\Foot Steps Sound Effect.wav");
            sonidoEscondite.loadSound(GuiController.Instance.AlumnoEjemplosDir + "THE_CVENGERS\\AlumnoMedia\\Sonidos\\Wardrobe closing sound effect.wav");
            sonidoMonstruo.loadSound(GuiController.Instance.AlumnoEjemplosDir + "THE_CVENGERS\\AlumnoMedia\\Sonidos\\Monster Roar   Sound Effect.wav");
            sonidoFoto.loadSound(GuiController.Instance.AlumnoEjemplosDir + "THE_CVENGERS\\AlumnoMedia\\Sonidos\\Girl Laugh Short Sound Effect.wav");
            sonidoFoto2.loadSound(GuiController.Instance.AlumnoEjemplosDir + "THE_CVENGERS\\AlumnoMedia\\Sonidos\\Cut_Girl Singing   music sound FX.wav");
            sonidoFoto3.loadSound(GuiController.Instance.AlumnoEjemplosDir + "THE_CVENGERS\\AlumnoMedia\\Sonidos\\Cut(1)_Girl Singing   music sound FX.wav");
            sonidoRespiracion.loadSound(GuiController.Instance.AlumnoEjemplosDir + "THE_CVENGERS\\AlumnoMedia\\Sonidos\\Heavy Breathing Man.wav");
            sonidoMuerte.loadSound(GuiController.Instance.AlumnoEjemplosDir + "THE_CVENGERS\\AlumnoMedia\\Sonidos\\Evil Laugh.wav"); 
            musica.FileName = GuiController.Instance.AlumnoEjemplosDir + "THE_CVENGERS\\AlumnoMedia\\Sonidos\\music.mp3";
            musicaInicial.FileName = GuiController.Instance.AlumnoEjemplosDir + "THE_CVENGERS\\AlumnoMedia\\Sonidos\\music1.mp3";



            //Se crean 2 triangulos (o Quad) con las dimensiones de la pantalla con sus posiciones ya transformadas
            // x = -1 es el extremo izquiedo de la pantalla, x = 1 es el extremo derecho
            // Lo mismo para la Y con arriba y abajo
            // la Z en 1 simpre
            CustomVertex.PositionTextured[] screenQuadVertices = new CustomVertex.PositionTextured[]
            {
                new CustomVertex.PositionTextured( -1, 1, 1, 0,0),
                new CustomVertex.PositionTextured(1,  1, 1, 1,0),
                new CustomVertex.PositionTextured(-1, -1, 1, 0,1),
                new CustomVertex.PositionTextured(1,-1, 1, 1,1)
            };
            //vertex buffer de los triangulos
            screenQuadVB = new VertexBuffer(typeof(CustomVertex.PositionTextured),
                    4, d3dDevice, Usage.Dynamic | Usage.WriteOnly,
                        CustomVertex.PositionTextured.Format, Pool.Default);
            screenQuadVB.SetData(screenQuadVertices, 0, LockFlags.None);

            //Creamos un Render Targer sobre el cual se va a dibujar la pantalla
            renderTarget2D = new Texture(d3dDevice, d3dDevice.PresentationParameters.BackBufferWidth
                    , d3dDevice.PresentationParameters.BackBufferHeight, 1, Usage.RenderTarget,
                        Format.X8R8G8B8, Pool.Default);


            //Cargar shader con efectos de Post-Procesado
            effect = TgcShaders.loadEffect(GuiController.Instance.AlumnoEjemplosDir + "THE_CVENGERS\\AlumnoMedia\\Shaders\\PostProcess.fx");

            //Configurar Technique dentro del shader
            effect.Technique = "AlarmaTechnique";


            //Cargar textura que se va a dibujar arriba de la escena del Render Target
            alarmTexture = TgcTexture.createTexture(d3dDevice, GuiController.Instance.ExamplesMediaDir + "Shaders\\efecto_alarma.png");

            //Interpolador para efecto de variar la intensidad de la textura de alarma
            intVaivenAlarm = new InterpoladorVaiven();
            intVaivenAlarm.Min = 0;
            intVaivenAlarm.Max = 1;
            intVaivenAlarm.Speed = 5;
            intVaivenAlarm.reset();


        }


        public override void render(float elapsedTime)
        {
           

           


            TgcD3dInput input = GuiController.Instance.D3dInput;
            sphere.setCenter(camera.getPosition());
            spherePuertas.setCenter(camera.getPosition());
            sphereEscondites.setCenter(camera.getPosition());

            if (inicioJuego)
            {


                if (contadorPantalla == 0)
                {
                    if (reproducirMusica)
                    {
                        musicaInicial.play(true);
                        reproducirMusica = false;
                        camera.Enable = false;
                    }

                    pantallaInicioBool = true;

                  

                    if (input.keyPressed(Key.Return))
                    {

                        contadorPantalla++;

                    }
                }

                if (contadorPantalla == 1)
                {
                    pantallaInstruccionesBool = true;

  

                    tiempo = tiempo + elapsedTime;

                    reproducirMusica = true;

                    if (input.keyPressed(Key.Return) && tiempo > 0.3)
                    {

                        contadorPantalla++;
                        tiempo = 0;
                    }
                }

                if (contadorPantalla == 2)
                {
                    pantallaHistoriaBool = true;


                    tiempo = tiempo + elapsedTime;

                    if (reproducirMusica)
                    {
                        musicaInicial.closeFile();
                        musica.play(true);
                        reproducirMusica = false;
                    }
                    if (input.keyPressed(Key.Return) && tiempo > 0.3)
                    {
                        contadorPantalla++;
                        tiempo = 0;
                        


                    }
                }

                if (contadorPantalla == 3)
                {
                    pantallaObjetivoBool = true;


                    tiempo = tiempo + elapsedTime;

                    if (reproducirMusica)
                    {
                        musicaInicial.closeFile();
                        musica.play(true);
                        reproducirMusica = false;
                    }
                    if (input.keyPressed(Key.Return) && tiempo > 0.5)
                    {

                        camera.Enable = true;
                        inicioJuego = false;


                    }
                }

            }
            else
            {
                if (!ganaste && !muerte && sinEsconderse && !mapaAbierto && !invAbierto)
                {
                    
                    iconoInventarioBool = true;
                }
                if(!ganaste && !muerte && sinEsconderse && !invAbierto)
                {
                    iconoMapaBool = true;
                }
                    
                    meshIluminacion.Transform = lightManager.getMatriz(camera, tipoLuz);

                 


                    if (tipoLuz != 0)
                    {
                        if (sinEsconderse && !muerte)
                        {
                             meshIluminacionBool = true;
                            
                            lightManager.renderLuces(camera, currentShader2, tengoLuz, tipoLuz);
                    }
                    else
                    {
                        lightManager.renderLuces(camera, currentShader2, tengoLuz, tipoLuz);
                    }
                    }
                    else
                    {
                        lightManager.renderLuces(camera, currentShader2, tengoLuz, 1);
                    }






                    if (input.keyUp(Key.Q))
                    {
                        if (tengoVela || tengoLinterna || tengoLampara)
                        {

                            if ((tipoLuz == 1 && !velaRota) || (tipoLuz == 2 && !linternaRota) || (tipoLuz == 3 && !lamparaRota))
                            {
                                if (sinEsconderse)
                                {
                                    if (tengoLuz)
                                    {
                                        tengoLuz = false;
                                    }
                                    else { tengoLuz = true; }
                                }
                            }
                        }

                    }

                    if ((tipoLuz == 1 && velaRota) || (tipoLuz == 2 && linternaRota) || (tipoLuz == 3 && lamparaRota))
                    {
                        tengoLuz = false;
                    }



                    if (input.keyUp(Key.Tab))
                    {
                        if (tengoVela && !tengoLinterna && !tengoLampara)
                        {

                        }
                        if (!tengoVela && tengoLinterna && !tengoLampara)
                        {

                        }
                        if (!tengoVela && !tengoLinterna && tengoLampara)
                        {

                        }
                        if (!tengoVela && !tengoLinterna && !tengoLampara)
                        {

                        }
                        if (tengoVela && tengoLinterna && !tengoLampara)
                        {
                            switch (tipoLuz)
                            {
                                case 1:
                                    tipoLuz = 2;
                                    meshIluminacion = lightManager.changeMesh(meshIluminacion, 2);
                                    break;
                                case 2:
                                    tipoLuz = 1;
                                    meshIluminacion = lightManager.changeMesh(meshIluminacion, 1);
                                    break;
                            }
                        }
                        if (!tengoVela && tengoLinterna && tengoLampara)
                        {
                            switch (tipoLuz)
                            {
                                case 2:
                                    tipoLuz = 3;
                                    meshIluminacion = lightManager.changeMesh(meshIluminacion, 3);
                                    break;
                                case 3:
                                    tipoLuz = 2;
                                    meshIluminacion = lightManager.changeMesh(meshIluminacion, 2);
                                    break;
                            }
                        }
                        if (tengoVela && !tengoLinterna && tengoLampara)
                        {
                            switch (tipoLuz)
                            {
                                case 1:
                                    tipoLuz = 3;
                                    meshIluminacion = lightManager.changeMesh(meshIluminacion, 3);
                                    break;
                                case 3:
                                    tipoLuz = 1;
                                    meshIluminacion = lightManager.changeMesh(meshIluminacion, 1);
                                    break;
                            }
                        }
                        if (tengoVela && tengoLinterna && tengoLampara)
                        {
                            switch (tipoLuz)
                            {
                                case 1:
                                    tipoLuz = 2;
                                    meshIluminacion = lightManager.changeMesh(meshIluminacion, 2);
                                    break;
                                case 2:
                                    tipoLuz = 3;
                                    meshIluminacion = lightManager.changeMesh(meshIluminacion, 3);
                                    break;
                                case 3:
                                    tipoLuz = 1;
                                    meshIluminacion = lightManager.changeMesh(meshIluminacion, 1);
                                    break;
                            }
                        }


                    }

                    if (contadorFotos != fotoActual)
                    {
                        fotoActual = contadorFotos;

                        if (fotoActual == 1)
                        {
                            caminoVillano = PathInitializer.crearPathAzul();
                            listaPuntosAux.Clear();
                        }

                        if (fotoActual == 2)
                        {
                            caminoVillano = PathInitializer.crearPathVerde();
                            listaPuntosAux.Clear();
                        }

                        if (fotoActual == 3)
                        {
                            ganaste = true;
                        }
                    }


                    /////////////////////////////////////////////  PARA EL VILLANO  ///////////////////////////////////////////////////////////

                    meshVillano = lightManager.shaderVillano(meshVillano, skeletalShader, camera);


                meshVillano.updateAnimation();

                ///////////////////////////// MOVIMIENTO VILLANO/////////////////////////////////
                //esferaVillano.render();
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

                    if (!villanoPersiguiendo && !ganaste)
                    {
                        tiempoVillano = tiempoVillano + elapsedTime;

                        if (tiempoVillano > 0.01f )
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

                            tiempoVillano = 0;
                        }

                    }
                    else
                    {


                        if ((Math.Abs((newPosition - meshVillano.Position).X) > 350 || Math.Abs((newPosition - meshVillano.Position).Z) > 350) && !ganaste)
                        {
                            villanoPersiguiendo = false;
                            listaPuntosAux.Clear();
                            meshVillano.playAnimation(selectedAnim, true);
                            if (fotoActual == 0)
                            {
                                caminoVillano = PathInitializer.crearPathRojo();
                                listaPuntosAux.Clear();
                            }
                            if (fotoActual == 1)
                            {
                                caminoVillano = PathInitializer.crearPathAzul();
                                listaPuntosAux.Clear();
                            }
                            if (fotoActual == 2)
                            {
                                caminoVillano = PathInitializer.crearPathVerde();
                                listaPuntosAux.Clear();
                            }


                        }



                        if (camera.getPosition() != camaraAnterior && !ganaste)
                        {

                            camaraAnterior = camera.getPosition();
                            parametrosBusq = new SearchParameters(new Point(((int)meshVillano.Position.X), ((int)meshVillano.Position.Z)), new Point(((int)camera.Position.X), ((int)camera.Position.Z)), Aux.mapBool);


                            Astar = new CalculadoraDeTrayecto(parametrosBusq, Aux.nodes);



                            path = Astar.FindPath(new Point(((int)camera.Position.X), ((int)camera.Position.Z)));
                            CalculadoraDeTrayecto.resetearNodos();
                        }


                        if (path.Count != 0 && !ganaste)
                        {

                            tiempoVillanoPath = tiempoVillanoPath + elapsedTime;

                            if (tiempoVillanoPath > 0.005f)
                            {

                                Vector3 proximoLugar = new Vector3(path.Find(punti => punti.X == punti.X).X, 5, path.Find(punti => punti.X == punti.X).Y);
                                path.Remove(path.Find(punti => punti.X == punti.X));
                                meshVillano.Position = proximoLugar;

                                tiempoVillanoPath = 0;
                            }

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

                    if (TgcCollisionUtils.testSphereAABB(sphere, meshVillano.BoundingBox))  //(meshVillano.BoundingBox, sceneMeshBoundingBox))
                    {
                        collisionVillanoCamara = true;

                    }


                    if (collisionVillanoCamara && sinEsconderse)
                    {

                        muerte = true;
                    mapaAbierto = false;
                    invAbierto = false;
                        sonidoMuerte.play();

                    }




                    if (respiracion && !villanoPersiguiendo)
                    {
                        sonidoRespiracion.play();
                        respiracion = false;
                    }




                    ///////////////////////////// FIN MOVIMIENTO VILLANO/////////////////////////////////

                    


                    if (input.keyDown(Key.W) || input.keyDown(Key.A) || input.keyDown(Key.S) || input.keyDown(Key.D))
                    {
                        if (sinEsconderse)
                        {
                            sonidoPasos.play(true);
                        }
                    }
                    else
                    {
                        sonidoPasos.stop();

                    }





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




                    if (TgcCollisionUtils.testSphereSphere(esferaVillano, sphere) && sinEsconderse)
                    {
                        villanoPersiguiendo = true;
                        sonidoMonstruo.play(true);
                        meshVillano.playAnimation(selectedAnim2, true);
                        respiracion = true;
                    }

                    if (!villanoPersiguiendo)
                    {
                        sonidoMonstruo.stop();
                    }

                    foreach (Puerta door in puertasAbiertasVillano)
                    {

                        if (!villanoPersiguiendo)
                        {
                        if (!abriendoPuerta) { 
                            door.siendoAbiertaPorVillano = true;

                            tiempoPuertaVillano = tiempoPuertaVillano + elapsedTime;

                            if (tiempoPuertaVillano > 0.01)
                            {

                                if (door.contadorVillano < 100 && !door.getStatus())
                                {

                                    door.accionarPuerta();
                                    door.contadorVillano++;
                                    tiempoPuertaVillano = 0;
                                }
                                else
                                {
                                    if (door.contadorVillano == 100)
                                        door.cambiarStatus();

                                    if (door.contadorVillano > 0)
                                    {
                                        door.accionarPuerta();
                                        door.contadorVillano--;
                                        tiempoPuertaVillano = 0;
                                    }
                                    else
                                    {
                                        door.cambiarStatus();
                                        puertasAbiertasVillanoAux.Add(door);
                                        door.siendoAbiertaPorVillano = false;
                                        tiempoPuertaVillano = 0;
                                    }
                                }
                            }
                        }
                    }
                        else door.siendoAbiertaPorVillano = true;
                    }

                    foreach (Puerta door in listaPuertas)
                    {


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
                                    door.siendoAbiertaPorVillano = true;
                                }
                            }
                            if (!TgcCollisionUtils.testSphereAABB(esferaVillanoPuertas, door.getMesh().BoundingBox) && door.villanoAbriendoPrimera)
                            {
                                door.villanoAbriendoSiguientes = true;
                                door.siendoAbiertaPorVillano = true;
                            }
                        }
                        else
                        {
                            if ((TgcCollisionUtils.testSphereAABB(esferaVillanoPuertas, door.getMesh().BoundingBox) && !door.getStatus() && !door.siendoAbiertaPorVillano))
                            {
                                villanoPersiguiendo = false;
                                listaPuntosAux.Clear();
                                meshVillano.playAnimation(selectedAnim, true);

                                if (fotoActual == 0)
                                {
                                    caminoVillano = PathInitializer.crearPathRojo();
                                    listaPuntosAux.Clear();
                                }
                                if (fotoActual == 1)
                                {
                                    caminoVillano = PathInitializer.crearPathAzul();
                                    listaPuntosAux.Clear();
                                }
                                if (fotoActual == 2)
                                {
                                    caminoVillano = PathInitializer.crearPathVerde();
                                    listaPuntosAux.Clear();
                                }


                            }
                        }
                    }




                    foreach (Puerta door in puertasAbiertasVillanoAux)
                    {
                        puertasAbiertasVillano.Remove(door);
                    }

                    puertasAbiertasVillanoAux.Clear();

                    foreach (Objeto fot in listaFotos)
                    {
                        if (TgcCollisionUtils.testSphereAABB(spherePuertas, fot.getMesh().BoundingBox))
                        {
                            if (fot.getMesh().Enabled)
                            {


                            iconoFotoBool = true;   
                         



                                if (input.keyUp(Key.E))
                                {
                                    if (fotoActual == 0)
                                    {
                                        sonidoFoto.play();
                                    }
                                    if (fotoActual == 1)
                                    {
                                        sonidoFoto2.play();
                                    }
                                    if (fotoActual == 2)
                                    {
                                        sonidoFoto3.play(true);
                                        musica.stop();
                                    }
                                    fot.getMesh().Enabled = false;
                                    contadorFotos++;

                                }
                            }

                        }
                    }


                if (!mapaAbierto && !invAbierto)
                {
                    foreach (Escondite hide in listaEscondites)
                    {
                        if (TgcCollisionUtils.testSphereAABB(sphereEscondites, hide.getMesh().BoundingBox) && !villanoPersiguiendo)
                        {

                            if (sinEsconderse)
                            {

                                keyHoleBool = true;

                            }
                            else
                            {

                            }
                            if (input.keyUp(Key.R))
                            {
                                sonidoEscondite.play();

                                if (sinEsconderse)
                                {
                                    sinEsconderse = false;
                                    posicionPrevia = camera.Position;
                                    lookAtPrevio = camera.LookAt;
                                    luzAnterior = tengoLuz;
                                    tengoLuz = false;
                                    camera.camaraEscondida = true;
                                    camera.setCamera(hide.posHidden, hide.LookAtHidden, scene, listaPuertas, listaObjetos, listaEscondites);


                                }
                                else
                                {
                                    sinEsconderse = true;
                                    tengoLuz = luzAnterior;
                                    camera.camaraEscondida = false;
                                    camera.setCamera(posicionPrevia, lookAtPrevio, scene, listaPuertas, listaObjetos, listaEscondites);
                                }


                            }

                        }
                    }
                }
                    if (!sinEsconderse)
                    {

                        pantallaEscondidoBool = true;

                    }

                    foreach (Puerta puerta in listaPuertas)
                    {
                        if (TgcCollisionUtils.testSphereAABB(spherePuertas, puerta.getMesh().BoundingBox) && puerta.puedeAbrirseSinTrabarse(sphere) && sinEsconderse)
                        {
                        if (!muerte && !mapaAbierto && !invAbierto)
                        {

                            spritePuertaBool = true;



                            if (input.keyUp(Key.E))
                            {

                                if (!abriendoPuerta && sinEsconderse)
                                {

                                    sonidoPuerta.play();
                                    puertaSelecionada = puerta;
                                    abriendoPuerta = true;
                                    contadorAbertura = 0;
                                    break;
                                }
                            }
                        }
                        }
                    }

                    if (abriendoPuerta)
                    {
                        tiempoPuerta = tiempoPuerta + elapsedTime;

                        if (tiempoPuerta > 0.02)
                        {

                            if (abriendoPuerta && contadorAbertura < 100)
                            {

                                puertaSelecionada.accionarPuerta();
                                contadorAbertura++;
                                tiempoPuerta = 0;
                            }
                            else
                            {
                                abriendoPuerta = false;
                                puertaSelecionada.cambiarStatus();
                                tiempoPuerta = 0;
                            }
                        }
                    }

                    if (abriendoPuerta)
                    {
                        camera.Enable = false;
                    }
                    else if(!muerte && !ganaste && !invAbierto && !mapaAbierto)
                    {
                        camera.Enable = true;
                    }


                 if(!muerte && !ganaste && sinEsconderse)
                {
                    if (input.keyPressed(Key.M) && !invAbierto)
                    {
                        if (mapaAbierto)
                        {
                            mapaAbierto = false;
                            camera.Enable = true;
                        }
                        else
                        {
                            mapaAbierto = true;
                            camera.Enable = false;
                        }
                    }

                    if (input.keyPressed(Key.I) && !mapaAbierto)
                    {
                            if (invAbierto)
                            {
                                invAbierto = false;
                            camera.Enable = true;
                        }
                            else
                            {
                                invAbierto = true;
                                camera.Enable = false;
                            }
                     }
                }   

                if (!muerte && !mapaAbierto && !invAbierto)
                {
                    if (TgcCollisionUtils.testSphereAABB(spherePuertas, candle.getMesh().BoundingBox))
                    {
                        if (candle.getMesh().Enabled)
                        {
                            iconoManoBool = true;



                        if (input.keyUp(Key.R))
                        {


                            candle.getMesh().Enabled = false;
                            tengoVela = true;
                            tipoLuz = 1;
                            tengoLuz = false;
                            meshIluminacion = lightManager.changeMesh(meshIluminacion, 1);



                        }

                    }

                        

                    }

              
                    if (TgcCollisionUtils.testSphereAABB(spherePuertas, flashlight.getMesh().BoundingBox))
                    {
                        if (flashlight.getMesh().Enabled)
                        {
                            iconoManoBool = true;

                            if (input.keyUp(Key.R))
                        {


                            flashlight.getMesh().Enabled = false;
                            tengoLinterna = true;
                            tipoLuz = 2;
                            tengoLuz = false;
                            meshIluminacion = lightManager.changeMesh(meshIluminacion, 2);

                        }

                    }

                       

                    }


                    if (TgcCollisionUtils.testSphereAABB(spherePuertas, lantern.getMesh().BoundingBox))
                    {
                        if (lantern.getMesh().Enabled)
                        {

                            iconoManoBool = true;

                            if (input.keyUp(Key.R))
                            {


                                lantern.getMesh().Enabled = false;
                                tengoLampara = true;
                                tipoLuz = 3;
                                tengoLuz = false;
                                meshIluminacion = lightManager.changeMesh(meshIluminacion, 3);


                            }

                        }
                    }
                        

                    }

                    if (tengoLuz && tipoLuz == 1)
                    {
                        tiempoVela += elapsedTime;
                    }
                    if (tengoLuz && tipoLuz == 2)
                    {
                        tiempoLinterna += elapsedTime;
                    }
                    if (tengoLuz && tipoLuz == 3)
                    {
                        tiempoLampara += elapsedTime;
                    }

                    if (tiempoVela > 30)
                    {
                        if (tipoLuz == 1)
                        {
                            tengoLuz = false;
                        }
                    }
                    if (tiempoLinterna > 30)
                    {
                        if (tipoLuz == 2)
                        {
                            tengoLuz = false;
                        }
                    }
                    if (tiempoLampara > 30)
                    {
                        if (tipoLuz == 3)
                        {
                            tengoLuz = false;
                        }
                    }

                if (muerte)
                { 
                sonidoMonstruo.stop();

                camera.Enable = false;
                    tengoLuz = false;

                    pantallaMuerteBool = true;
          

                if (input.keyPressed(Key.R))
                {
                    sonidoMuerte.stop();
                    muerte = false;
                    this.restart();
                }

                }

                if (ganaste)
                {
                    sonidoMonstruo.stop();

                    camera.Enable = false;
                    tengoLuz = false;

                    pantallaGanasteBool = true;
                    meshIluminacion.Enabled = false;

                    if (input.keyPressed(Key.R))
                    {
                        sonidoFoto3.stop();
                        ganaste = false;
                        musica.closeFile();
                        musica.play(true);
                        this.restart();
                    }

                }



            }

            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;

            //Cargamos el Render Targer al cual se va a dibujar la escena 3D. Antes nos guardamos el surface original
            //En vez de dibujar a la pantalla, dibujamos a un buffer auxiliar, nuestro Render Target.
          //  pOldRT = d3dDevice.GetRenderTarget(0);
          //  Surface pSurf = renderTarget2D.GetSurfaceLevel(0);
          //  d3dDevice.SetRenderTarget(0, pSurf);
         //   d3dDevice.Clear((ClearFlags.Target | ClearFlags.ZBuffer) , Color.Black, 1.0f, 0);

            //Dibujamos la escena comun, pero en vez de a la pantalla al Render Target
            d3dDevice.BeginScene();
            this.renderAll();
            d3dDevice.EndScene();

            //Liberar memoria de surface de Render Target
         //   pSurf.Dispose();

            //Si quisieramos ver que se dibujo, podemos guardar el resultado a una textura en un archivo para debugear su resultado (ojo, es lento)
            //TextureLoader.Save(GuiController.Instance.ExamplesMediaDir + "Shaders\\render_target.bmp", ImageFileFormat.Bmp, renderTarget2D);


            //Ahora volvemos a restaurar el Render Target original (osea dibujar a la pantalla)
          //  d3dDevice.SetRenderTarget(0, pOldRT);


            //Luego tomamos lo dibujado antes y lo combinamos con una textura con efecto de alarma
        //    this.drawPostProcess(d3dDevice);



        }
        /// <summary>
        /// Se toma todo lo dibujado antes, que se guardo en una textura, y se combina con otra textura, que en este ejemplo
        /// es para generar un efecto de alarma.
        /// Se usa un shader para combinar ambas texturas y lograr el efecto de alarma.
        /// </summary>
        private void drawPostProcess(Microsoft.DirectX.Direct3D.Device d3dDevice)
        {
            //Arrancamos la escena
            d3dDevice.BeginScene();

            //Cargamos para renderizar el unico modelo que tenemos, un Quad que ocupa toda la pantalla, con la textura de todo lo dibujado antes
            d3dDevice.VertexFormat = CustomVertex.PositionTextured.Format;
            d3dDevice.SetStreamSource(0, screenQuadVB, 0);

            //Ver si el efecto de alarma esta activado, configurar Technique del shader segun corresponda
            bool activar_efecto = true;
            if (activar_efecto)
            {
                effect.Technique = "AlarmaTechnique";
            }
            else
            {
                effect.Technique = "DefaultTechnique";
            }

            //Cargamos parametros en el shader de Post-Procesado
            effect.SetValue("render_target2D", renderTarget2D);
            effect.SetValue("textura_alarma", alarmTexture.D3dTexture);
            effect.SetValue("alarmaScaleFactor", intVaivenAlarm.update());

            //Limiamos la pantalla y ejecutamos el render del shader
            d3dDevice.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
            effect.Begin(FX.None);
            effect.BeginPass(0);
            d3dDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
            effect.EndPass();
            effect.End();

            //Terminamos el renderizado de la escena
            d3dDevice.EndScene();
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

            foreach (Objeto obj in listaObjetos)
            {
                obj.getMesh().dispose();
            }

            foreach (Objeto fot in listaFotos)
            {
                fot.getMesh().dispose();
            }

            foreach (Escondite hide in listaEscondites)
            {
                hide.getMesh().dispose();

            }

            foreach (Lampara lamp in listaLamparas)
            {
                lamp.getMesh().dispose();

            }


            meshIluminacion.dispose();
            sphere.dispose();
            meshVillano.dispose();
            esferaVillano.dispose();
            sphereEscondites.dispose();
            esferaVillanoPuertas.dispose();
            spherePuertas.dispose();
            sonidoPuerta.dispose();
            sonidoPasos.dispose();
            sonidoEscondite.dispose();
            sonidoFoto.dispose();
            sonidoFoto2.dispose();
            sonidoFoto3.dispose();
            sonidoMonstruo.dispose();
            sonidoRespiracion.dispose();
            effect.Dispose();
            alarmTexture.dispose();
            screenQuadVB.Dispose();
            renderTarget2D.Dispose();

        }

        public void restart()
        {


            tipoLuz = 0;

            contadorFotos = 0;
            fotoActual = 0;

            tengoLampara = false;
            tengoLinterna = false;
            tengoLuz = false;
            tengoVela = false;

            tiempoLinterna = 0;
            tiempoVela = 0;
            tiempoLampara = 0;

            meshIluminacionBool = false;

            respiracion = false;

            lantern.getMesh().Enabled = true;
            candle.getMesh().Enabled = true;
            flashlight.getMesh().Enabled = true;

            foreach(Objeto fot in listaFotos)
            {
                fot.getMesh().Enabled = true;
            }

            listaPuertas.Clear();

            PuertaManager jose = new PuertaManager();

            listaPuertas = jose.initPuertas();

            foreach (Puerta puerta in listaPuertas)
            {
                puerta.getMesh().Effect = currentShader2;
                //El Technique depende del tipo RenderType del mesh
                puerta.getMesh().Technique = "MultiDiffuseLightsTechnique";
            }


            caminoVillano = PathInitializer.crearPathRojo();
            listaPuntosAux = new List<Point>();


            camera.setCamera(new Vector3(609, 45, 921), new Vector3(500, 0, 1), scene, listaPuertas, listaObjetos, listaEscondites);

            camera.Enable = true;

        }

        public void renderAll()
        {

            foreach (TgcMesh mesh in meshes)
            {

                mesh.render();
            }

            foreach (Objeto fot in listaFotos)
            {
                fot.Render();

            }

            foreach (Puerta puerta in listaPuertas)
            {
                puerta.getMesh().render();

            }

           

            foreach (Objeto obj in listaObjetos)
            {
                obj.Render();

            }


            candle.Render();

            foreach (Lampara lamp in listaLamparas)
            {
                lamp.Render();
            }

            meshVillano.render();

            foreach (Escondite hide in listaEscondites)
            {
                hide.Render();

            }


            if (meshIluminacionBool)
            {
                meshIluminacion.render();
            }




            flashlight.Render();
            lantern.Render();











            GuiController.Instance.Text3d.drawText("FPS: " + HighResolutionTimer.Instance.FramesPerSecond, 0, 0, Color.Yellow);


            

            




            if (pantallaInicioBool)
            {
                GuiController.Instance.Drawer2D.beginDrawSprite();

                pantallaInicio.render();

                GuiController.Instance.Drawer2D.endDrawSprite();
                pantallaInicioBool = false;
            }

            if (pantallaInstruccionesBool)
            {
                GuiController.Instance.Drawer2D.beginDrawSprite();

                pantallaInstrucciones.render();

                GuiController.Instance.Drawer2D.endDrawSprite();
                pantallaInstruccionesBool = false;
            }

            if (pantallaHistoriaBool)
            {
                GuiController.Instance.Drawer2D.beginDrawSprite();

                pantallaHistoria.render();

                GuiController.Instance.Drawer2D.endDrawSprite();
                pantallaHistoriaBool = false;
            }


            if (iconoFotoBool)
            {
                GuiController.Instance.Drawer2D.beginDrawSprite();

                iconoFoto.render();

                GuiController.Instance.Drawer2D.endDrawSprite();

                iconoFotoBool = false;
            }

            if (keyHoleBool)
            {
                GuiController.Instance.Drawer2D.beginDrawSprite();

                keyHole.render();

                GuiController.Instance.Drawer2D.endDrawSprite();

                keyHoleBool = false;
            }


            if (pantallaEscondidoBool)
            {
                GuiController.Instance.Drawer2D.beginDrawSprite();

                pantallaEscondido.render();

                GuiController.Instance.Drawer2D.endDrawSprite();

                pantallaEscondidoBool = false;
            }


            if (spritePuertaBool)
            {
                GuiController.Instance.Drawer2D.beginDrawSprite();

                spritePuerta.render();

                GuiController.Instance.Drawer2D.endDrawSprite();

                spritePuertaBool = false;
            }


            if (iconoManoBool)
            {
                GuiController.Instance.Drawer2D.beginDrawSprite();

                iconoMano.render();

                GuiController.Instance.Drawer2D.endDrawSprite();

                iconoManoBool = false;
            }


            if (pantallaMuerteBool)
            {
                GuiController.Instance.Drawer2D.beginDrawSprite();

                pantallaMuerte.render();

                GuiController.Instance.Drawer2D.endDrawSprite();

                pantallaMuerteBool = false;
            }

            if (pantallaObjetivoBool)
            {
                GuiController.Instance.Drawer2D.beginDrawSprite();

                pantallaObjetivo.render();

                GuiController.Instance.Drawer2D.endDrawSprite();

                pantallaObjetivoBool = false;
            }

            if (pantallaGanasteBool)
            {
                GuiController.Instance.Drawer2D.beginDrawSprite();

                pantallaGanaste.render();

                GuiController.Instance.Drawer2D.endDrawSprite();

                pantallaGanasteBool = false;
            }

            if (iconoMapaBool)
            {
                GuiController.Instance.Drawer2D.beginDrawSprite();

                iconoMapa.render();

                GuiController.Instance.Drawer2D.endDrawSprite();

                iconoMapaBool = false;
            }

            if (iconoInventarioBool)
            {
                GuiController.Instance.Drawer2D.beginDrawSprite();

                iconoInventario.render();

                GuiController.Instance.Drawer2D.endDrawSprite();

                iconoInventarioBool = false;
            }

            if (mapaAbierto)
            {
                GuiController.Instance.Drawer2D.beginDrawSprite();

                mapa.render();

                GuiController.Instance.Drawer2D.endDrawSprite();

           
            }



        }

    }
}
