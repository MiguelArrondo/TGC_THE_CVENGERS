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
using Examples.Quake3Loader;
using Examples.Shaders;
using TgcViewer.Utils.Shaders;
using System.IO;
using TgcViewer.Utils.TgcSkeletalAnimation;


namespace AlumnoEjemplos.THE_CVENGERS
{
    class LightManager
    {
        public LightManager()
        {
            

        }

        Color myArgbColor = Color.FromArgb(10, 10, 10);
            Vector3 lightDir;

        List<Lampara> listaLamparas = new List<Lampara>();


        public TgcMesh Init(int tipoLuz)
        {
            TgcMesh mesh;
            TgcSceneLoader loaderL = new TgcSceneLoader();
            
           switch(tipoLuz)
            {
                case 1:
                    mesh = loaderL.loadSceneFromFile(GuiController.Instance.AlumnoEjemplosDir + "THE_CVENGERS\\AlumnoMedia\\ObjetosIluminacion\\Linterna\\flashlight-TgcScene.xml").Meshes[0];
                    mesh.Effect = TgcShaders.loadEffect(GuiController.Instance.AlumnoEjemplosDir + "THE_CVENGERS\\AlumnoMedia\\ObjetosIluminacion\\ShaderObjetos.fx");
                    mesh.Technique = "Darkening";
                    mesh.Effect.SetValue("darkFactor", (float)0.35f);
                    break;
                case 2:
                    mesh = loaderL.loadSceneFromFile(GuiController.Instance.AlumnoEjemplosDir + "THE_CVENGERS\\AlumnoMedia\\ObjetosIluminacion\\Candle\\candle-TgcScene.xml").Meshes[0];
                    mesh.Effect = TgcShaders.loadEffect(GuiController.Instance.AlumnoEjemplosDir + "THE_CVENGERS\\AlumnoMedia\\ObjetosIluminacion\\ShaderObjetos.fx");
                    mesh.Technique = "Darkening";
                    mesh.Effect.SetValue("darkFactor", (float)0.45f);
                    break;

                    //defaulteo linterna sino putea
                default:
                    mesh = loaderL.loadSceneFromFile(GuiController.Instance.AlumnoEjemplosDir + "THE_CVENGERS\\AlumnoMedia\\ObjetosIluminacion\\Candle\\candle-TgcScene.xml").Meshes[0];
                    break;
            }
            
            mesh.move(500, 45, 900);
            mesh.AutoTransformEnable = false;

            return mesh;

        }

        public List<Lampara> initLamparas()
        {
            listaLamparas.Add(new Lampara(new Vector3(32, 80, 701), 90, new Vector3(0.2f, 0.2f, 0.2f), new Vector3(0.4f, -1, 0), new Vector3(75, 100, 682)));
            listaLamparas.Add(new Lampara(new Vector3(450, 88, 960), 180, new Vector3(0.2f, 0.2f, 0.2f), new Vector3(0, -1, 0.4f), new Vector3(450, 100, 930)));
            listaLamparas.Add(new Lampara(new Vector3(928, 88, 310), 0, new Vector3(0.2f, 0.2f, 0.2f), new Vector3(0, -1, 0.4f), new Vector3(928, 100, 330)));
            listaLamparas.Add(new Lampara(new Vector3(750, 88, 628), 270, new Vector3(0.2f, 0.2f, 0.2f), new Vector3(-0.4f, -1, 0), new Vector3(729, 100, 628)));

            return listaLamparas;
        }

        public Matrix getMatriz(FPSCustomCamera camera, int tipoLuz)
        {
            Matrix magiaOscura = Matrix.Invert(camera.ViewMatrix);
            Matrix distanciaCamara=Matrix.Identity;
            Matrix tamanio=Matrix.Identity;

            if (tipoLuz == 1)
            {
                distanciaCamara = Matrix.Translation(new Vector3(10f, -20f, 30f));
                tamanio = Matrix.Scaling(0.2f, 0.2f, 0.2f);
            }


         if (tipoLuz == 2)
            {
                distanciaCamara = Matrix.Translation(new Vector3(20f, -75f, 30f));
                tamanio = Matrix.Scaling(0.5f, 0.5f, 0.5f);
            }

            Matrix rotaciony = Matrix.RotationY(-0.2f);
            Matrix rotacionx = Matrix.RotationX(-0.2f);

            return tamanio * rotaciony * rotacionx * distanciaCamara * magiaOscura;
        }


        public void renderLuces(FPSCustomCamera camera, Microsoft.DirectX.Direct3D.Effect shader, bool luzPrendida,int tipoLuz)
        {

           lightDir = (camera.getLookAt() - camera.getPosition());
            lightDir.Normalize();

            ColorValue[] lightColors = new ColorValue[5];
            Vector4[] pointLightPositions = new Vector4[5];
            float[] pointLightIntensity = new float[5];
            float[] pointLightAttenuation = new float[5];
            Vector3 spotLightDir0 = new Vector3(0,0,0);
            Vector3 spotLightDir1 = new Vector3(0, 0, 0);
            Vector3 spotLightDir2 = new Vector3(0, 0, 0);
            Vector3 spotLightDir3 = new Vector3(0, 0, 0);
            Vector3 spotLightDir4 = new Vector3(0, 0, 0);
            float[] spotLightAngleCos = new float[5]; //Angulo de apertura del cono de luz (en radianes)
            float[] spotLightExponent = new float[5];

            if (luzPrendida)

                {
                    if (tipoLuz == 1)
                    {
                    lightColors[0] =  ColorValue.FromColor(Color.White);
                    spotLightExponent[0] = 60f;
                    pointLightIntensity[0] =  2000f;
                    pointLightAttenuation[0] =  0.5f;
                    }
                   if (tipoLuz == 2)
                    {
                    lightColors[0] = ColorValue.FromColor(Color.Orange);
                    spotLightExponent[0] = 18f;
                    pointLightIntensity[0] = 800f;
                    pointLightAttenuation[0] = 0.5f;
                    }
                }
                else
                {
                lightColors[0] = ColorValue.FromColor(myArgbColor);
                pointLightIntensity[0] = 0f;
                spotLightExponent[0] = 18f;
                pointLightAttenuation[0] = 2.5f;

            }
            spotLightDir0 = lightDir;
            pointLightPositions[0] = TgcParserUtils.vector3ToVector4(camera.getPosition());
            spotLightAngleCos[0] = FastMath.ToRad(45f);

            foreach(Lampara lamp in listaLamparas)
            {
                int i = listaLamparas.FindIndex(lampi => lamp == lampi);

                lightColors[i+1] = ColorValue.FromColor(Color.Yellow);
                spotLightExponent[i+1] = 2f;
                pointLightIntensity[i+1] = 1000f;
                pointLightAttenuation[i+1] = 0.5f;
                if(i == 0)
                {
                    spotLightDir1 = lamp.lightDir;
                }
                if (i == 1)
                {
                    spotLightDir2 = lamp.lightDir;
                }
                if (i == 2)
                {
                    spotLightDir3 = lamp.lightDir;
                }
                if (i == 3)
                {
                    spotLightDir4 = lamp.lightDir;
                }

                pointLightPositions[i + 1] = TgcParserUtils.vector3ToVector4(lamp.lightPos);
                spotLightAngleCos[i + 1] = FastMath.ToRad(0f);
            }

            shader.SetValue("materialEmissiveColor", ColorValue.FromColor(myArgbColor));
            shader.SetValue("materialDiffuseColor", ColorValue.FromColor(myArgbColor));
            shader.SetValue("lightColor", lightColors);
            shader.SetValue("lightPosition", pointLightPositions);
            shader.SetValue("lightIntensity", pointLightIntensity);
            shader.SetValue("lightAttenuation", pointLightAttenuation);
            shader.SetValue("spotLightDir0", TgcParserUtils.vector3ToFloat3Array(spotLightDir0));
            shader.SetValue("spotLightDir1", TgcParserUtils.vector3ToFloat3Array(spotLightDir1));
            shader.SetValue("spotLightDir2", TgcParserUtils.vector3ToFloat3Array(spotLightDir2));
            shader.SetValue("spotLightDir3", TgcParserUtils.vector3ToFloat3Array(spotLightDir3));
            shader.SetValue("spotLightDir4", TgcParserUtils.vector3ToFloat3Array(spotLightDir4));
            shader.SetValue("spotLightAngleCos", spotLightAngleCos);
            shader.SetValue("spotLightExponent", spotLightExponent);



        }

        public TgcMesh changeMesh(TgcMesh mesh, int tipoMesh)
        {
            TgcSceneLoader loaderL = new TgcSceneLoader();

            switch (tipoMesh)
            {
                case 1:
                    mesh = loaderL.loadSceneFromFile(GuiController.Instance.AlumnoEjemplosDir + "THE_CVENGERS\\AlumnoMedia\\ObjetosIluminacion\\Linterna\\flashlight-TgcScene.xml").Meshes[0];
                    mesh.Effect = TgcShaders.loadEffect(GuiController.Instance.AlumnoEjemplosDir + "THE_CVENGERS\\AlumnoMedia\\ObjetosIluminacion\\ShaderObjetos.fx");
                    mesh.Technique = "Darkening";
                    mesh.Effect.SetValue("darkFactor", (float)0.30f);
                    break;
                case 2:
                    mesh = loaderL.loadSceneFromFile(GuiController.Instance.AlumnoEjemplosDir + "THE_CVENGERS\\AlumnoMedia\\ObjetosIluminacion\\Candle\\candle-TgcScene.xml").Meshes[0];
                    mesh.Effect = TgcShaders.loadEffect(GuiController.Instance.AlumnoEjemplosDir + "THE_CVENGERS\\AlumnoMedia\\ObjetosIluminacion\\ShaderObjetos.fx");
                    mesh.Technique = "Darkening";
                    mesh.Effect.SetValue("darkFactor", (float)0.55f);
                    break;

                //defaulteo linterna sino putea
                default:
                    mesh = loaderL.loadSceneFromFile(GuiController.Instance.AlumnoEjemplosDir + "THE_CVENGERS\\AlumnoMedia\\ObjetosIluminacion\\Candle\\candle-TgcScene.xml").Meshes[0];
                    break;
            }

            mesh.move(500, 45, 900);
            mesh.AutoTransformEnable = false;


            return mesh;

        }

        public TgcSkeletalMesh shaderVillano(TgcSkeletalMesh meshVillano,Microsoft.DirectX.Direct3D.Effect skeletalShader,FPSCustomCamera camera)
        {
            meshVillano.Effect = skeletalShader;
            //Cargar variables shader de la luz
            meshVillano.Effect.SetValue("lightColor", ColorValue.FromColor(Color.White));

            meshVillano.Effect.SetValue("lightPosition", TgcParserUtils.vector3ToFloat4Array(camera.getPosition()));
            meshVillano.Effect.SetValue("lightIntensity", (float)30f);
            meshVillano.Effect.SetValue("lightAttenuation", (float)1.05f);

            //Cargar variables de shader de Material. El Material en realidad deberia ser propio de cada mesh. Pero en este ejemplo se simplifica con uno comun para todos
            meshVillano.Effect.SetValue("materialEmissiveColor", ColorValue.FromColor(Color.Black));
            meshVillano.Effect.SetValue("materialAmbientColor", ColorValue.FromColor(Color.White));
            meshVillano.Effect.SetValue("materialDiffuseColor", ColorValue.FromColor(myArgbColor));
            meshVillano.Effect.SetValue("materialSpecularColor", ColorValue.FromColor(Color.White));
            meshVillano.Effect.SetValue("materialSpecularExp", (float)20f);


            return meshVillano;



        }

        

    }
}
