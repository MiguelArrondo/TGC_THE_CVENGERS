using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX.DirectInput;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Geometry;
using TGC.Core.Input;
using TGC.Core.Interpolation;
using TGC.Core.SceneLoader;
using TGC.Core.Shaders;
using TGC.Core.SkeletalAnimation;
using TGC.Core.Sound;
using TGC.Core.Textures;
using TGC.Core.Utils;

namespace TGC.Group.Model
{
    public class Juego : TgcExample
    {
        private Control focusWindows = D3DDevice.Instance.Device.CreationParameters.FocusWindow;

        private string selectedAnim;
        private string selectedAnim2;
        private TgcSkeletalMesh meshVillano;

        private TgcScene scene;

        private string mediaPath;
        private string[] animationsPath;

        private CalculadoraDeTrayecto Aux = new CalculadoraDeTrayecto();
        private SearchParameters parametrosBusq;
        private Vector3 camaraAnterior = new Vector3(0, 0, 0);
        private List<Point> path = new List<Point>();
        private int contadorFrames = 0;
        private Microsoft.DirectX.Direct3D.Effect currentShader2;

        //Con luz: Cambiar el shader actual por el shader default que trae el framework para iluminacion dinamica con PointLight
        private Microsoft.DirectX.Direct3D.Effect skeletalShader;

        private int tipoLuz;
        private float tiempoVela;
        private float tiempoLinterna;
        private float tiempoLampara;
        private bool velaRota;
        private bool linternaRota;
        private bool lamparaRota;

        private float tiempoVillano = 0;
        private float tiempoVillanoPath = 0;
        private float tiempoPuertaVillano = 0;
        private float tiempoPuerta = 0;
        private float tiempo = 0;
        private float tiempoBateria = 0;

        private CalculadoraDeTrayecto Astar;

        private TgcStaticSound sonidoPuerta = new TgcStaticSound();
        private TgcStaticSound sonidoPasos = new TgcStaticSound();
        private TgcStaticSound sonidoEscondite = new TgcStaticSound();
        private TgcStaticSound sonidoMonstruo = new TgcStaticSound();
        private TgcStaticSound sonidoFoto = new TgcStaticSound();
        private TgcStaticSound sonidoFoto2 = new TgcStaticSound();
        private TgcStaticSound sonidoFoto3 = new TgcStaticSound();
        private TgcStaticSound sonidoRespiracion = new TgcStaticSound();
        private TgcStaticSound sonidoMuerte = new TgcStaticSound();
        private TgcMp3Player musica = new TgcMp3Player();
        private TgcMp3Player musicaInicial = new TgcMp3Player();

        private const float MOVEMENT_SPEED = 400f;
        private FPSCustomCamera camera;

        private List<TgcMesh> meshes;

        private bool tengoLuz = false;

        //Variable para esfera
        private TgcBoundingSphere sphere;

        private TgcBoundingSphere spherePuertas;
        private TgcBoundingSphere sphereEscondites;

        private List<Puerta> listaPuertas;
        private List<Objeto> listaObjetos;
        private List<Escondite> listaEscondites;
        private List<Objeto> listaFotos;

        private Objeto flashlight;
        private Objeto candle;
        private Objeto lantern;

        private TgcMesh meshIluminacion;

        private LightManager lightManager = new LightManager();
        private List<Lampara> listaLamparas;

        //PARA EL VILLANO

        private Vector3 newPosition;
        private Vector3 originalMeshRot;

        private List<Point> caminoVillano;
        private List<Point> listaPuntosAux;

        private bool villanoPersiguiendo = false;
        private TgcBoundingSphere esferaVillano;

        private Puerta puertaSelecionada;
        private bool abriendoPuerta;
        private int contadorAbertura;

        private bool sinEsconderse = true;
        private Vector3 posicionPrevia;
        private Vector3 lookAtPrevio;
        private bool luzAnterior;

        private TgcSprite spritePuerta;
        private TgcSprite pantallaInicio;
        private TgcSprite pantallaHistoria;
        private TgcSprite pantallaInstrucciones;
        private TgcSprite pantallaMuerte;
        private TgcSprite pantallaEscondido;
        private TgcSprite pantallaObjetivo;
        private TgcSprite pantallaGanaste;
        private TgcSprite keyHole;
        private TgcSprite iconoFoto;
        private TgcSprite iconoMano;
        private TgcSprite iconoInventario;
        private TgcSprite iconoMapa;
        private TgcSprite mapa;
        private TgcSprite inventario1;
        private TgcSprite inventario2;
        private TgcSprite inventario3;
        private TgcSprite inventario4;
        private TgcSprite inventario5;
        private TgcSprite inventario6;
        private TgcSprite inventario7;
        private TgcSprite sinEnergia;

        private List<Puerta> puertasAbiertasVillano = new List<Puerta>();
        private List<Puerta> puertasAbiertasVillanoAux = new List<Puerta>();

        private TgcBoundingSphere esferaVillanoPuertas = new TgcBoundingSphere();

        private int contadorFotos = 0;
        private int fotoActual = 0;

        private bool respiracion;

        private bool muerte = false;

        private bool tengoLinterna = false;
        private bool tengoVela = false;
        private bool tengoLampara = false;

        private bool reproducirMusica = true;
        private bool inicioJuego = true;
        private int contadorPantalla = 0;

        private bool pantallaInicioBool = false;
        private bool pantallaInstruccionesBool = false;
        private bool pantallaHistoriaBool = false;
        private bool meshIluminacionBool = false;
        private bool iconoFotoBool = false;
        private bool keyHoleBool = false;
        private bool pantallaEscondidoBool = false;
        private bool spritePuertaBool = false;
        private bool iconoManoBool = false;
        private bool pantallaMuerteBool = false;
        private bool pantallaObjetivoBool = false;
        private bool ganaste = false;
        private bool pantallaGanasteBool = false;

        private bool iconoMapaBool = false;
        private bool iconoInventarioBool = false;
        private bool mapaAbierto = false;
        private bool invAbierto = false;

        private VertexBuffer screenQuadVB;
        private Texture renderTarget2D;
        private Surface pOldRT;
        private Microsoft.DirectX.Direct3D.Effect effect;
        private TgcTexture alarmTexture;
        private InterpoladorVaiven intVaivenAlarm;

        private Size screenSize;

        private Surface depthStencil;

        /// <summary>
        /// Constructor del juego
        /// </summary>
        /// <param name="mediaDir">Ruta donde esta la carpeta con los assets</param>
        /// <param name="shadersDir">Ruta donde esta la carpeta con los shaders</param>
        public Juego(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
            Category = "THE_CVENGERS";
            Name = "The Orphanage";
            Description = "The Orphanage... a survival horror videogame made by Miguel Arrondo & Leandro Wagner.";
        }

        public override void Init()
        {
            //Creamos caja de colision
            sphere = new TgcBoundingSphere(new Vector3(0, 0, 0), 20f);
            spherePuertas = new TgcBoundingSphere(new Vector3(160, 60, 240), 60f);
            sphereEscondites = new TgcBoundingSphere(new Vector3(160, 60, 240), 30f);

            Microsoft.DirectX.Direct3D.Device d3dDevice = D3DDevice.Instance.Device;

            //Path para carpeta de texturas de la malla
            mediaPath = MediaDir + "SkeletalAnimations\\BasicHuman\\";

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
            meshVillano = loaderVillano.loadMeshAndAnimationsFromFile(MediaDir + "CS_Gign-TgcSkeletalMesh.xml", MediaDir + "", animationsPath);

            //Crear esqueleto a modo Debug
            meshVillano.buildSkletonMesh();

            meshVillano.move(new Vector3(289, 5, 577));//(628, 10, 51);

            meshVillano.playAnimation(selectedAnim, true);

            //Cargamos un escenario
            TgcSceneLoader loader = new TgcSceneLoader();
            scene = loader.loadSceneFromFile(MediaDir + "mapaDef-TgcScene.xml");
            meshes = scene.Meshes;

            //foreach(TgcMesh meshScene in meshes)
            //  {
            //       meshScene.Scale = new Vector3(2, 2, 2);
            //   }

            ObjetosManager carlos = new ObjetosManager();

            listaObjetos = carlos.initObjetos(MediaDir);

            Aux.map = scene;
            Aux.objetosMapa = listaObjetos;
            Aux.personaje = meshVillano;
            Aux.analizarPuntosPared();
            Aux.InitializeNodes(Aux.mapBool);

            esferaVillano = new TgcBoundingSphere(new Vector3(0, 0, 0), 135f);
            esferaVillanoPuertas = new TgcBoundingSphere(new Vector3(0, 0, 0), 50f);

            //Crear una UserVar
            //TODO en la nueva version no hay mas UserVars si las usan deben dibujarlas como texto por pantalla
            //GuiController.Instance.UserVars.addVar("PosicionX");
            //GuiController.Instance.UserVars.addVar("PosicionY");
            //GuiController.Instance.UserVars.addVar("PosicionZ");

            tipoLuz = 1;

            meshIluminacion = lightManager.Init(MediaDir, tipoLuz);

            tipoLuz = 0;

            originalMeshRot = new Vector3(0, 0, -1);

            // currentShader = TgcShaders.loadEffect(MediaDir + "Shaders\\MeshSpotLightShader.fx");
            currentShader2 = TgcShaders.loadEffect(MediaDir + "Shaders\\MultiDiffuseLightsCustom.fx");

            //Aplicar a cada mesh el shader actual
            foreach (TgcMesh mesh in meshes)
            {
                mesh.Effect = currentShader2;
                //El Technique depende del tipo RenderType del mesh
                mesh.Technique = "MultiDiffuseLightsTechnique";
            }

            skeletalShader = TgcShaders.Instance.TgcSkeletalMeshPointLightShader;

            meshVillano.Technique = TgcShaders.Instance.getTgcSkeletalMeshTechnique(meshVillano.RenderType);

            caminoVillano = PathInitializer.crearPathRojo();
            listaPuntosAux = new List<Point>();

            PuertaManager pepe = new PuertaManager();

            listaPuertas = pepe.initPuertas(MediaDir);

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

            listaFotos = carlos.initFotos(MediaDir);

            foreach (Objeto fot in listaFotos)
            {
                fot.getMesh().Effect = currentShader2;
                //El Technique depende del tipo RenderType del mesh
                fot.getMesh().Technique = "MultiDiffuseLightsTechnique";
            }

            listaEscondites = carlos.initEscondites(MediaDir);

            foreach (Escondite hide in listaEscondites)
            {
                hide.getMesh().Effect = currentShader2;
                //El Technique depende del tipo RenderType del mesh
                hide.getMesh().Technique = "MultiDiffuseLightsTechnique";
            }

            listaLamparas = lightManager.initLamparas(MediaDir);

            foreach (Lampara lamp in listaLamparas)
            {
                lamp.getMesh().Effect = currentShader2;
                lamp.getMesh().Technique = "MultiDiffuseLightsTechnique";
            }

            candle = carlos.initItems(MediaDir).Find(item => item.nombre == "candle-TgcScene.xml");
            candle.getMesh().Effect = currentShader2;
            //El Technique depende del tipo RenderType del mesh
            candle.getMesh().Technique = "MultiDiffuseLightsTechnique";

            flashlight = carlos.initItems(MediaDir).Find(item => item.nombre == "flashlight-TgcScene.xml");
            flashlight.getMesh().Effect = currentShader2;
            //El Technique depende del tipo RenderType del mesh
            flashlight.getMesh().Technique = "MultiDiffuseLightsTechnique";

            lantern = carlos.initItems(MediaDir).Find(item => item.nombre == "lantern-TgcScene.xml");
            lantern.getMesh().Effect = currentShader2;
            //El Technique depende del tipo RenderType del mesh
            lantern.getMesh().Technique = "MultiDiffuseLightsTechnique";

            spritePuerta = new TgcSprite();
            spritePuerta.Texture = TgcTexture.createTexture(MediaDir + "puertitaIcono.png");
            spritePuerta.Scaling = new Vector2(((float)screenSize.Width / spritePuerta.Texture.Width) * 0.2f, ((float)screenSize.Height / spritePuerta.Texture.Height) * 0.2f);
            Size textureSizePuerta = spritePuerta.Texture.Size;
            spritePuerta.Position = new Vector2(FastMath.Max(screenSize.Width / 2 - spritePuerta.Texture.Height * spritePuerta.Scaling.X, 0), FastMath.Max(screenSize.Height / 2 + spritePuerta.Texture.Height * spritePuerta.Scaling.Y, 0));

            pantallaInicio = new TgcSprite();
            pantallaInicio.Texture = TgcTexture.createTexture(MediaDir + "PSD\\intPORTADA.png");
            pantallaInicio.Scaling = new Vector2((float)screenSize.Width / pantallaInicio.Texture.Width, (float)screenSize.Height / pantallaInicio.Texture.Height);
            pantallaInicio.Position = new Vector2(0, 0);

            pantallaMuerte = new TgcSprite();
            pantallaMuerte.Texture = TgcTexture.createTexture(MediaDir + "PSD\\pantallaDIED.png");
            pantallaMuerte.Scaling = new Vector2((float)screenSize.Width / pantallaMuerte.Texture.Width, (float)screenSize.Height / pantallaMuerte.Texture.Height);
            pantallaMuerte.Position = new Vector2(0, 0);

            pantallaHistoria = new TgcSprite();
            pantallaHistoria.Texture = TgcTexture.createTexture(MediaDir + "PSD\\intTEXT2.png");
            pantallaHistoria.Scaling = new Vector2((float)screenSize.Width / pantallaHistoria.Texture.Width, (float)screenSize.Height / pantallaHistoria.Texture.Height);
            pantallaHistoria.Position = new Vector2(0, 0);

            pantallaEscondido = new TgcSprite();
            pantallaEscondido.Texture = TgcTexture.createTexture(MediaDir + "PSD\\intKEYHOLE.png");
            pantallaEscondido.Scaling = new Vector2((float)screenSize.Width / pantallaEscondido.Texture.Width, (float)screenSize.Height / pantallaEscondido.Texture.Height);
            pantallaEscondido.Position = new Vector2(0, 0);

            pantallaInstrucciones = new TgcSprite();
            pantallaInstrucciones.Texture = TgcTexture.createTexture(MediaDir + "PSD\\intCONTROLS.png");
            pantallaInstrucciones.Scaling = new Vector2((float)screenSize.Width / pantallaInstrucciones.Texture.Width, (float)screenSize.Height / pantallaInstrucciones.Texture.Height);
            pantallaInstrucciones.Position = new Vector2(0, 0);

            pantallaObjetivo = new TgcSprite();
            pantallaObjetivo.Texture = TgcTexture.createTexture(MediaDir + "PSD\\intBEWARE.png");
            pantallaObjetivo.Scaling = new Vector2((float)screenSize.Width / pantallaObjetivo.Texture.Width, (float)screenSize.Height / pantallaObjetivo.Texture.Height);
            pantallaObjetivo.Position = new Vector2(0, 0);

            pantallaGanaste = new TgcSprite();
            pantallaGanaste.Texture = TgcTexture.createTexture(MediaDir + "PSD\\pantallaWIN2.png");
            pantallaGanaste.Scaling = new Vector2((float)screenSize.Width / pantallaGanaste.Texture.Width, (float)screenSize.Height / pantallaGanaste.Texture.Height);
            pantallaGanaste.Position = new Vector2(0, 0);

            keyHole = new TgcSprite();
            keyHole.Texture = TgcTexture.createTexture(MediaDir + "intHIDE.png");
            keyHole.Scaling = new Vector2((float)screenSize.Width / keyHole.Texture.Width * 0.20f, (float)screenSize.Height / keyHole.Texture.Height * 0.10f);
            Size textureSizeKey = keyHole.Texture.Size;
            keyHole.Position = new Vector2(FastMath.Max(screenSize.Width / 2 + keyHole.Texture.Width * keyHole.Scaling.X * 0.5f, 0), FastMath.Max(screenSize.Height - spritePuerta.Texture.Height * spritePuerta.Scaling.Y * 2, 0));

            iconoMapa = new TgcSprite();
            iconoMapa.Texture = TgcTexture.createTexture(MediaDir + "PSD\\intMAPinGAME.png");
            iconoMapa.Scaling = new Vector2((float)screenSize.Width / iconoMapa.Texture.Width * 0.15f, (float)screenSize.Height / iconoMapa.Texture.Height * 0.1f);
            Size textureSizeMap = iconoMapa.Texture.Size;
            iconoMapa.Position = new Vector2(FastMath.Max(screenSize.Width - iconoMapa.Texture.Width * iconoMapa.Scaling.X, 0), FastMath.Max(0, 0));

            iconoInventario = new TgcSprite();
            iconoInventario.Texture = TgcTexture.createTexture(MediaDir + "PSD\\intINVENTORYinGAME.png");
            iconoInventario.Scaling = new Vector2((float)screenSize.Width / iconoInventario.Texture.Width * 0.15f, (float)screenSize.Height / iconoInventario.Texture.Height * 0.1f);
            Size textureSizeInv = iconoInventario.Texture.Size;
            iconoInventario.Position = new Vector2(FastMath.Max(screenSize.Width - iconoInventario.Texture.Width * iconoInventario.Scaling.X, 0), FastMath.Max(iconoMapa.Texture.Height * iconoMapa.Scaling.Y, 0));

            iconoFoto = new TgcSprite();
            iconoFoto.Texture = TgcTexture.createTexture(MediaDir + "camera icon.png");
            iconoFoto.Scaling = new Vector2((float)screenSize.Width / iconoFoto.Texture.Width * 0.20f, (float)screenSize.Height / iconoFoto.Texture.Height * 0.10f);
            Size textureSizeCam = iconoFoto.Texture.Size;
            iconoFoto.Position = new Vector2(FastMath.Max(screenSize.Width / 2 - iconoFoto.Texture.Width * iconoFoto.Scaling.X * 0.5f, 0), FastMath.Max(screenSize.Height - spritePuerta.Texture.Height * spritePuerta.Scaling.Y * 2, 0));

            iconoMano = new TgcSprite();
            iconoMano.Texture = TgcTexture.createTexture(MediaDir + "hand.png");
            iconoMano.Scaling = new Vector2((float)screenSize.Width / iconoMano.Texture.Width * 0.20f, (float)screenSize.Height / iconoMano.Texture.Height * 0.10f);
            Size textureSizeHand = iconoMano.Texture.Size;
            iconoMano.Position = new Vector2(FastMath.Max(screenSize.Width / 2 + iconoMano.Texture.Height * iconoMano.Scaling.X, 0), FastMath.Max(screenSize.Height / 2 + spritePuerta.Texture.Height * spritePuerta.Scaling.Y * 0.5f, 0));

            mapa = new TgcSprite();
            mapa.Texture = TgcTexture.createTexture(MediaDir + "mapa tgcTransparente.png");
            mapa.Scaling = new Vector2((float)screenSize.Width / mapa.Texture.Width * 0.5f, (float)screenSize.Height / mapa.Texture.Height * 0.6f);
            Size textureSizeMapa = mapa.Texture.Size;
            mapa.Position = new Vector2(FastMath.Max(screenSize.Width / 2 - mapa.Texture.Width / 2 * mapa.Scaling.X, 0), FastMath.Max(screenSize.Height / 2 - mapa.Texture.Height / 2 * mapa.Scaling.Y, 0));

            inventario1 = new TgcSprite();
            inventario1.Texture = TgcTexture.createTexture(MediaDir + "PSD\\INVENTORY\\inv1.png");
            inventario1.Scaling = new Vector2((float)screenSize.Width / inventario1.Texture.Width, (float)screenSize.Height / inventario1.Texture.Height);
            inventario1.Position = new Vector2(0, 0);

            inventario2 = new TgcSprite();
            inventario2.Texture = TgcTexture.createTexture(MediaDir + "PSD\\INVENTORY\\inv2.png");
            inventario2.Scaling = new Vector2((float)screenSize.Width / inventario2.Texture.Width, (float)screenSize.Height / inventario2.Texture.Height);
            inventario2.Position = new Vector2(0, 0);

            inventario3 = new TgcSprite();
            inventario3.Texture = TgcTexture.createTexture(MediaDir + "PSD\\INVENTORY\\inv3.png");
            inventario3.Scaling = new Vector2((float)screenSize.Width / inventario3.Texture.Width, (float)screenSize.Height / inventario3.Texture.Height);
            inventario3.Position = new Vector2(0, 0);

            inventario4 = new TgcSprite();
            inventario4.Texture = TgcTexture.createTexture(MediaDir + "PSD\\INVENTORY\\inv4.png");
            inventario4.Scaling = new Vector2((float)screenSize.Width / inventario4.Texture.Width, (float)screenSize.Height / inventario4.Texture.Height);
            inventario4.Position = new Vector2(0, 0);

            inventario5 = new TgcSprite();
            inventario5.Texture = TgcTexture.createTexture(MediaDir + "PSD\\INVENTORY\\inv5.png");
            inventario5.Scaling = new Vector2((float)screenSize.Width / inventario5.Texture.Width, (float)screenSize.Height / inventario5.Texture.Height);
            inventario5.Position = new Vector2(0, 0);

            inventario6 = new TgcSprite();
            inventario6.Texture = TgcTexture.createTexture(MediaDir + "PSD\\INVENTORY\\inv6.png");
            inventario6.Scaling = new Vector2((float)screenSize.Width / inventario6.Texture.Width, (float)screenSize.Height / inventario6.Texture.Height);
            inventario6.Position = new Vector2(0, 0);

            inventario7 = new TgcSprite();
            inventario7.Texture = TgcTexture.createTexture(MediaDir + "PSD\\INVENTORY\\inv7.png");
            inventario7.Scaling = new Vector2((float)screenSize.Width / inventario7.Texture.Width, (float)screenSize.Height / inventario7.Texture.Height);
            inventario7.Position = new Vector2(0, 0);

            sinEnergia = new TgcSprite();
            sinEnergia.Texture = TgcTexture.createTexture(MediaDir + "PSD\\intOUTenergy.png");
            sinEnergia.Scaling = new Vector2((float)screenSize.Width / iconoMano.Texture.Width * 0.4f, (float)screenSize.Height / iconoMano.Texture.Height * 0.20f);
            sinEnergia.Position = new Vector2(0, FastMath.Max(screenSize.Height - sinEnergia.Texture.Height * sinEnergia.Scaling.Y, 0));

            sonidoPuerta.loadSound(MediaDir + "Sonidos\\door creaks open   sound effect.wav", DirectSound.DsDevice);
            sonidoPasos.loadSound(MediaDir + "Sonidos\\Foot Steps Sound Effect.wav", DirectSound.DsDevice);
            sonidoEscondite.loadSound(MediaDir + "Sonidos\\Wardrobe closing sound effect.wav", DirectSound.DsDevice);
            sonidoMonstruo.loadSound(MediaDir + "Sonidos\\Monster Roar   Sound Effect.wav", DirectSound.DsDevice);
            sonidoFoto.loadSound(MediaDir + "Sonidos\\Girl Laugh Short Sound Effect.wav", DirectSound.DsDevice);
            sonidoFoto2.loadSound(MediaDir + "Sonidos\\Cut_Girl Singing   music sound FX.wav", DirectSound.DsDevice);
            sonidoFoto3.loadSound(MediaDir + "Sonidos\\Cut(1)_Girl Singing   music sound FX.wav", DirectSound.DsDevice);
            sonidoRespiracion.loadSound(MediaDir + "Sonidos\\Heavy Breathing Man.wav", DirectSound.DsDevice);
            sonidoMuerte.loadSound(MediaDir + "Sonidos\\Evil Laugh.wav", DirectSound.DsDevice);
            musica.FileName = MediaDir + "Sonidos\\music.mp3";
            musicaInicial.FileName = MediaDir + "Sonidos\\music1.mp3";

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

            //Cargar shader con efectos de Post-Procesado
            effect = TgcShaders.loadEffect(MediaDir + "Shaders\\PostProcess.fx");

            //Cargar textura que se va a dibujar arriba de la escena del Render Target
            alarmTexture = TgcTexture.createTexture(d3dDevice, MediaDir + "efecto_alarma.png");

            //Interpolador para efecto de variar la intensidad de la textura de alarma
            intVaivenAlarm = new InterpoladorVaiven();
            intVaivenAlarm.Min = 0;
            intVaivenAlarm.Max = 1;
            intVaivenAlarm.Speed = 5;
            intVaivenAlarm.reset();

            //Creamos un Render Targer sobre el cual se va a dibujar la pantalla
            renderTarget2D = new Texture(d3dDevice, d3dDevice.PresentationParameters.BackBufferWidth, d3dDevice.PresentationParameters.BackBufferHeight, 1, Usage.RenderTarget, Format.X8R8G8B8, Pool.Default);
            //Creamos un DepthStencil que debe ser compatible con nuestra definicion de renderTarget2D.
            depthStencil = d3dDevice.CreateDepthStencilSurface(d3dDevice.PresentationParameters.BackBufferWidth, d3dDevice.PresentationParameters.BackBufferHeight, DepthFormat.D24S8, MultiSampleType.None, 0, true);
            // Luego en se debe asignar este stencil:
            // Probar de comentar esta linea, para ver como se produce el fallo en el ztest
            // por no soportar usualmente el multisampling en el render to texture (en nuevas placas de video)

            this.camera = new FPSCustomCamera(Input);
            //TODO La nueva version maneja en forma interna algunas cuestiones de la camara de esta forma vinculo la suya con la de TGC
            this.Camara = this.camera;
            camera.Enable = true;
            camera.setCamera(new Vector3(609, 45, 921), new Vector3(500, 0, 1), scene, listaPuertas, listaObjetos, listaEscondites);
        }

        /// <summary>
        /// Se llama en cada frame.
        /// Se debe escribir toda la logica de computo del modelo.
        /// </summary>
        public override void Update()
        {
            PreUpdate();
            //TODO en la nueva estructura esta separado el Update del Render.
        }

        public override void Render()
        {
            //Inicio el render de la escena
            //PreRender();

            TgcD3dInput input = Input;
            sphere.setCenter(camera.getPosition());
            spherePuertas.setCenter(camera.getPosition());
            sphereEscondites.setCenter(camera.getPosition());

            Cursor.Position = focusWindows.PointToScreen(
                new Point(
                    focusWindows.Width / 2,
                    focusWindows.Height / 2)
                    );

            Cursor.Hide();

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

                    tiempo = tiempo + ElapsedTime;

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

                    tiempo = tiempo + ElapsedTime;

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

                    tiempo = tiempo + ElapsedTime;

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
                if (!ganaste && !muerte && sinEsconderse && !invAbierto)
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
                                meshIluminacion = lightManager.changeMesh(MediaDir, meshIluminacion, 2);
                                break;

                            case 2:
                                tipoLuz = 1;
                                meshIluminacion = lightManager.changeMesh(MediaDir, meshIluminacion, 1);
                                break;
                        }
                    }
                    if (!tengoVela && tengoLinterna && tengoLampara)
                    {
                        switch (tipoLuz)
                        {
                            case 2:
                                tipoLuz = 3;
                                meshIluminacion = lightManager.changeMesh(MediaDir, meshIluminacion, 3);
                                break;

                            case 3:
                                tipoLuz = 2;
                                meshIluminacion = lightManager.changeMesh(MediaDir, meshIluminacion, 2);
                                break;
                        }
                    }
                    if (tengoVela && !tengoLinterna && tengoLampara)
                    {
                        switch (tipoLuz)
                        {
                            case 1:
                                tipoLuz = 3;
                                meshIluminacion = lightManager.changeMesh(MediaDir, meshIluminacion, 3);
                                break;

                            case 3:
                                tipoLuz = 1;
                                meshIluminacion = lightManager.changeMesh(MediaDir, meshIluminacion, 1);
                                break;
                        }
                    }
                    if (tengoVela && tengoLinterna && tengoLampara)
                    {
                        switch (tipoLuz)
                        {
                            case 1:
                                tipoLuz = 2;
                                meshIluminacion = lightManager.changeMesh(MediaDir, meshIluminacion, 2);
                                break;

                            case 2:
                                tipoLuz = 3;
                                meshIluminacion = lightManager.changeMesh(MediaDir, meshIluminacion, 3);
                                break;

                            case 3:
                                tipoLuz = 1;
                                meshIluminacion = lightManager.changeMesh(MediaDir, meshIluminacion, 1);
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

                meshVillano = lightManager.shaderVillano(meshVillano, skeletalShader, camera, tengoLuz);

                meshVillano.updateAnimation(ElapsedTime);

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
                    tiempoVillano = tiempoVillano + ElapsedTime;

                    if (tiempoVillano > 0.01f)
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
                        tiempoVillanoPath = tiempoVillanoPath + ElapsedTime;

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
                //TODO en la nueva version no hay mas UserVars si las usan deben dibujarlas como texto por pantalla
                //GuiController.Instance.UserVars.setValue("PosicionX", camera.getPosition().X);
                //GuiController.Instance.UserVars.setValue("PosicionY", camera.getPosition().Y);
                //GuiController.Instance.UserVars.setValue("PosicionZ", camera.getPosition().Z);

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
                        if (!abriendoPuerta)
                        {
                            door.siendoAbiertaPorVillano = true;

                            tiempoPuertaVillano = tiempoPuertaVillano + ElapsedTime;

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
                                    meshIluminacionBool = false;
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
                    if (TgcCollisionUtils.testSphereAABB(spherePuertas, puerta.getMesh().BoundingBox) && puerta.puedeAbrirseSinTrabarse(MediaDir, sphere) && sinEsconderse)
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
                    tiempoPuerta = tiempoPuerta + ElapsedTime;

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
                else if (!muerte && !ganaste && !invAbierto && !mapaAbierto)
                {
                    camera.Enable = true;
                }

                if (!muerte && !ganaste && sinEsconderse)
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
                                meshIluminacion = lightManager.changeMesh(MediaDir, meshIluminacion, 1);
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
                                meshIluminacion = lightManager.changeMesh(MediaDir, meshIluminacion, 2);
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
                                meshIluminacion = lightManager.changeMesh(MediaDir, meshIluminacion, 3);
                            }
                        }
                    }
                }

                if (tengoLuz && tipoLuz == 1)
                {
                    tiempoVela += ElapsedTime;
                }
                if (tengoLuz && tipoLuz == 2)
                {
                    tiempoLinterna += ElapsedTime;
                }
                if (tengoLuz && tipoLuz == 3)
                {
                    tiempoLampara += ElapsedTime;
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

            Microsoft.DirectX.Direct3D.Device d3dDevice = D3DDevice.Instance.Device;

            //Cargamos el Render Targer al cual se va a dibujar la escena 3D. Antes nos guardamos el surface original
            //En vez de dibujar a la pantalla, dibujamos a un buffer auxiliar, nuestro Render Target.
            pOldRT = d3dDevice.GetRenderTarget(0);
            Surface pSurf = renderTarget2D.GetSurfaceLevel(0);
            d3dDevice.SetRenderTarget(0, pSurf);

            Surface pOldDS = d3dDevice.DepthStencilSurface;
            d3dDevice.DepthStencilSurface = depthStencil;

            d3dDevice.Clear((ClearFlags.Target | ClearFlags.ZBuffer), Color.Black, 1.0f, 0);

            //Dibujamos la escena comun, pero en vez de a la pantalla al Render Target
            d3dDevice.BeginScene();
            this.renderAll(ElapsedTime);
            d3dDevice.EndScene();

            //Liberar memoria de surface de Render Target
            pSurf.Dispose();

            //Ahora volvemos a restaurar el Render Target original (osea dibujar a la pantalla)
            d3dDevice.DepthStencilSurface = pOldDS;
            d3dDevice.SetRenderTarget(0, pOldRT);

            //Luego tomamos lo dibujado antes y lo combinamos con una textura con efecto de alarma

            this.drawPostProcess(d3dDevice);

            //Finaliza el render
            D3DDevice.Instance.Device.Present();
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
            //bool activar_efecto = true;
            if (villanoPersiguiendo && !muerte)
            {
                effect.Technique = "AlarmaTechnique";
            }
            else
            {
                effect.Technique = "DefaultTechnique";
            }

            //Cargamos parametros en el shader de Post-Procesado
            effect.SetValue("time", ElapsedTime);
            effect.SetValue("render_target2D", renderTarget2D);
            effect.SetValue("textura_alarma", alarmTexture.D3dTexture);
            effect.SetValue("alarmaScaleFactor", intVaivenAlarm.update(ElapsedTime));

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

        public override void Dispose()
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

            foreach (Objeto fot in listaFotos)
            {
                fot.getMesh().Enabled = true;
            }

            listaPuertas.Clear();

            PuertaManager jose = new PuertaManager();

            listaPuertas = jose.initPuertas(MediaDir);

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

        public void renderAll(float ElapsedTime)
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

            this.RenderFPS();

            if (pantallaInicioBool)
            {
                TgcDrawer2D.Instance.beginDrawSprite();

                pantallaInicio.render();

                TgcDrawer2D.Instance.endDrawSprite();
                pantallaInicioBool = false;
            }

            if (pantallaInstruccionesBool)
            {
                TgcDrawer2D.Instance.beginDrawSprite();

                pantallaInstrucciones.render();

                TgcDrawer2D.Instance.endDrawSprite();
                pantallaInstruccionesBool = false;
            }

            if (pantallaHistoriaBool)
            {
                TgcDrawer2D.Instance.beginDrawSprite();

                pantallaHistoria.render();

                TgcDrawer2D.Instance.endDrawSprite();
                pantallaHistoriaBool = false;
            }

            if (iconoFotoBool && !muerte)
            {
                TgcDrawer2D.Instance.beginDrawSprite();

                iconoFoto.render();

                TgcDrawer2D.Instance.endDrawSprite();

                iconoFotoBool = false;
            }

            if (keyHoleBool && !muerte)
            {
                TgcDrawer2D.Instance.beginDrawSprite();

                keyHole.render();

                TgcDrawer2D.Instance.endDrawSprite();

                keyHoleBool = false;
            }

            if (pantallaEscondidoBool)
            {
                TgcDrawer2D.Instance.beginDrawSprite();

                pantallaEscondido.render();

                TgcDrawer2D.Instance.endDrawSprite();

                pantallaEscondidoBool = false;
            }

            if (spritePuertaBool && !muerte)
            {
                TgcDrawer2D.Instance.beginDrawSprite();

                spritePuerta.render();

                TgcDrawer2D.Instance.endDrawSprite();

                spritePuertaBool = false;
            }

            if (iconoManoBool && !muerte)
            {
                TgcDrawer2D.Instance.beginDrawSprite();

                iconoMano.render();

                TgcDrawer2D.Instance.endDrawSprite();

                iconoManoBool = false;
            }

            if (pantallaMuerteBool)
            {
                TgcDrawer2D.Instance.beginDrawSprite();

                pantallaMuerte.render();

                TgcDrawer2D.Instance.endDrawSprite();

                pantallaMuerteBool = false;
            }

            if (pantallaObjetivoBool)
            {
                TgcDrawer2D.Instance.beginDrawSprite();

                pantallaObjetivo.render();

                TgcDrawer2D.Instance.endDrawSprite();

                pantallaObjetivoBool = false;
            }

            if (pantallaGanasteBool)
            {
                TgcDrawer2D.Instance.beginDrawSprite();

                pantallaGanaste.render();

                TgcDrawer2D.Instance.endDrawSprite();

                pantallaGanasteBool = false;
            }

            if (iconoMapaBool)
            {
                TgcDrawer2D.Instance.beginDrawSprite();

                iconoMapa.render();

                TgcDrawer2D.Instance.endDrawSprite();

                iconoMapaBool = false;
            }

            if (iconoInventarioBool)
            {
                TgcDrawer2D.Instance.beginDrawSprite();

                iconoInventario.render();

                TgcDrawer2D.Instance.endDrawSprite();

                iconoInventarioBool = false;
            }

            if (mapaAbierto)
            {
                TgcDrawer2D.Instance.beginDrawSprite();

                mapa.render();

                TgcDrawer2D.Instance.endDrawSprite();
            }

            if (invAbierto)
            {
                if (fotoActual == 0)
                {
                    TgcDrawer2D.Instance.beginDrawSprite();

                    inventario1.render();

                    TgcDrawer2D.Instance.endDrawSprite();
                }
                else if (fotoActual == 1)
                {
                    foreach (Objeto fot in listaFotos)
                    {
                        if (!fot.getMesh().Enabled)
                        {
                            if (fot.posicionAux == new Vector3(51, 45, 918))
                            {
                                TgcDrawer2D.Instance.beginDrawSprite();

                                inventario4.render();

                                TgcDrawer2D.Instance.endDrawSprite();
                            }
                            else if (fot.posicionAux == new Vector3(581, 45, 23))
                            {
                                TgcDrawer2D.Instance.beginDrawSprite();

                                inventario3.render();

                                TgcDrawer2D.Instance.endDrawSprite();
                            }
                            else if (fot.posicionAux == new Vector3(645, 45, 384))
                            {
                                TgcDrawer2D.Instance.beginDrawSprite();

                                inventario2.render();

                                TgcDrawer2D.Instance.endDrawSprite();
                            }
                        }
                    }
                }
                else if (fotoActual == 2)
                {
                    foreach (Objeto fot in listaFotos)
                    {
                        if (fot.getMesh().Enabled)
                        {
                            if (fot.posicionAux == new Vector3(51, 45, 918))
                            {
                                TgcDrawer2D.Instance.beginDrawSprite();

                                inventario5.render();

                                TgcDrawer2D.Instance.endDrawSprite();
                            }
                            else if (fot.posicionAux == new Vector3(581, 45, 23))
                            {
                                TgcDrawer2D.Instance.beginDrawSprite();

                                inventario6.render();

                                TgcDrawer2D.Instance.endDrawSprite();
                            }
                            else if (fot.posicionAux == new Vector3(645, 45, 384))
                            {
                                TgcDrawer2D.Instance.beginDrawSprite();

                                inventario7.render();

                                TgcDrawer2D.Instance.endDrawSprite();
                            }
                        }
                    }
                }
            }

            if ((tipoLuz == 1 && tiempoVela > 30) || (tipoLuz == 2 && tiempoLinterna > 30) || (tipoLuz == 3 && tiempoLampara > 30))
            {
                TgcDrawer2D.Instance.beginDrawSprite();

                sinEnergia.render();

                TgcDrawer2D.Instance.endDrawSprite();
            }
        }
    }
}