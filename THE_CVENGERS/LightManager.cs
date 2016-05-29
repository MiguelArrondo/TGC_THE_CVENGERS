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
    class LightManager
    {
        public LightManager()
        {
            

        }

        Color myArgbColor = Color.FromArgb(15, 15, 15);
            Vector3 lightDir;


        public TgcMesh Init(int tipoLuz)
        {
            TgcMesh mesh;
            TgcSceneLoader loaderL = new TgcSceneLoader();
            
           switch(tipoLuz)
            {
                case 1:
                    mesh = loaderL.loadSceneFromFile(GuiController.Instance.AlumnoEjemplosDir + "THE_CVENGERS\\AlumnoMedia\\Linterna\\flashlight-TgcScene.xml").Meshes[0];
                    break;
                case 2:
                    mesh = loaderL.loadSceneFromFile(GuiController.Instance.AlumnoEjemplosDir + "THE_CVENGERS\\AlumnoMedia\\Candle\\candle-TgcScene.xml").Meshes[0];
                    break;

                    //defaulteo linterna sino putea
                default:
                    mesh = loaderL.loadSceneFromFile(GuiController.Instance.AlumnoEjemplosDir + "THE_CVENGERS\\AlumnoMedia\\Candle\\candle-TgcScene.xml").Meshes[0];
                    break;
            }
            
            mesh.move(500, 45, 900);
            mesh.AutoTransformEnable = false;

            return mesh;

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


        public void renderLuces(FPSCustomCamera camera, List<TgcMesh> meshes, bool luzPrendida,int tipoLuz)
        {



           lightDir = (camera.getLookAt() - camera.getPosition());
            lightDir.Normalize();



            foreach (TgcMesh mesh in meshes)
            {

                //Cargar variables shader de la luz

                if (luzPrendida)

                {
                    if (tipoLuz == 1)
                    {
                        mesh.Effect.SetValue("lightColor", ColorValue.FromColor(Color.White));
                        mesh.Effect.SetValue("spotLightExponent", (float)60f);
                        mesh.Effect.SetValue("lightIntensity", (float)500f);
                        mesh.Effect.SetValue("lightAttenuation", (float)0.5f);
                    }
                   if (tipoLuz == 2)
                    {
                        mesh.Effect.SetValue("lightColor", ColorValue.FromColor(Color.Orange));
                        mesh.Effect.SetValue("spotLightExponent", (float)18f);
                        mesh.Effect.SetValue("lightIntensity", (float)200f);
                        mesh.Effect.SetValue("lightAttenuation", (float)0.5f);
                    }
                }
                else
                {
                    mesh.Effect.SetValue("lightColor", ColorValue.FromColor(myArgbColor));

                }


                mesh.Effect.SetValue("lightPosition", TgcParserUtils.vector3ToFloat4Array(camera.getPosition()));
                mesh.Effect.SetValue("eyePosition", TgcParserUtils.vector3ToFloat4Array(camera.getPosition()));
                mesh.Effect.SetValue("spotLightDir", TgcParserUtils.vector3ToFloat3Array(lightDir));
                mesh.Effect.SetValue("spotLightAngleCos", FastMath.ToRad((float)45f));



                //   Ya seteados en el shader propio
          



                //Cargar variables de shader de Material. El Material en realidad deberia ser propio de cada mesh. Pero en este ejemplo se simplifica con uno comun para todos
                mesh.Effect.SetValue("materialEmissiveColor", ColorValue.FromColor(myArgbColor));
                mesh.Effect.SetValue("materialAmbientColor", ColorValue.FromColor(myArgbColor));
                mesh.Effect.SetValue("materialDiffuseColor", ColorValue.FromColor(myArgbColor));
                mesh.Effect.SetValue("materialSpecularColor", ColorValue.FromColor(myArgbColor));
                mesh.Effect.SetValue("materialSpecularExp", (float)20f);




                //Renderizar modelo
                mesh.render();
            }
                     

        }

        public TgcMesh changeMesh(TgcMesh mesh, int tipoMesh)
        {
            TgcSceneLoader loaderL = new TgcSceneLoader();

            switch (tipoMesh)
            {
                case 1:
                    mesh = loaderL.loadSceneFromFile(GuiController.Instance.AlumnoEjemplosDir + "THE_CVENGERS\\AlumnoMedia\\Linterna\\flashlight-TgcScene.xml").Meshes[0];
                    break;
                case 2:
                    mesh = loaderL.loadSceneFromFile(GuiController.Instance.AlumnoEjemplosDir + "THE_CVENGERS\\AlumnoMedia\\Candle\\candle-TgcScene.xml").Meshes[0];
                    break;

                //defaulteo linterna sino putea
                default:
                    mesh = loaderL.loadSceneFromFile(GuiController.Instance.AlumnoEjemplosDir + "THE_CVENGERS\\AlumnoMedia\\Candle\\candle-TgcScene.xml").Meshes[0];
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
