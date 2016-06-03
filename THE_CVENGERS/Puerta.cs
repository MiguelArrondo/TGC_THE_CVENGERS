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
    class Puerta
    {
        TgcMesh Mesh;
        bool open;
        Vector3 posicion;
        Vector3 escala;

    


        public Puerta(Vector3 posicionCentro, float rotacion,Vector3 escalas)
        {

            this.open = false;

            TgcSceneLoader loadedrL = new TgcSceneLoader();
            this.Mesh = loadedrL.loadSceneFromFile(GuiController.Instance.AlumnoEjemplosDir + "THE_CVENGERS\\AlumnoMedia\\ObjetosMapa\\Puerta\\puerta-TgcScene.xml").Meshes[0];
            this.Mesh.AutoTransformEnable = false;
            this.posicion = posicionCentro;

            this.escala = escalas;
            //aca escalas
            Matrix escala = Matrix.Scaling(0.5f, 0.5f, 0.5f);


            Matrix posicion = Matrix.Translation(posicionCentro);
            this.Mesh.Transform = escala * posicion;

            this.Mesh.move(posicionCentro);
            if (rotacion != 0) {
                float angleY = FastMath.ToRad(90);
                Matrix rotation = Matrix.RotationYawPitchRoll(angleY, 0, 0);
                this.Mesh.Transform = rotation;

            }
            
          
            

        }

        public TgcMesh getMesh()
        {
            return this.Mesh;
        }

        public bool getStatus()
        {
            return this.open;
        }

        public void setStatus(bool status)
        {
            open = status;
        }


        public void abrirPuerta()
        {
            Vector3 nuevaPosicion = this.posicion + new Vector3(100f, 0, 100f); // los valores estan calculados como ((Ancho/2) - (Espesor/2))
            Matrix translate = Matrix.Translation(nuevaPosicion);
            float angleY = FastMath.ToRad(90);
            Matrix rotation = Matrix.RotationYawPitchRoll(angleY, 0, 0);

            this.getMesh().Transform = rotation * translate;
            this.getMesh().render();
        }

        public void cerrarPuerta()
        {

            Vector3 nuevaPosicion = this.posicion + new Vector3(-32.5f, 0, -32.5f);
            Matrix translate = Matrix.Translation(nuevaPosicion - new Vector3(-32.5f, 0, -32.5f));
            float angleY = FastMath.ToRad(90);

            this.getMesh().Transform = translate;
            this.getMesh().render();
          
        }



    }
}
