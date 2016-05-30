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
    class PuertaManager
    {

        List<Puerta> listaPuertas = new List<Puerta>();

        public PuertaManager() { }

        /// <summary>
        /// Sobre una lista de puntos, creo todas las puertas y las devuelvo en un vector, para que luego sean recorridas por el render() con un foreach
        /// 
        /// </summary>
        /// <returns></returns>



        public List<Puerta> initPuertas()
        {
            List<Vector3> posicionesPuertas = new List<Vector3>();

            //aca ponemos todas las posiciones de las puertas a colocar

            posicionesPuertas.Add(new Vector3(-20, 80, 450));
            posicionesPuertas.Add(new Vector3(-20, 200, 450));
            posicionesPuertas.Add(new Vector3(-216, 80, -29));


            foreach (Vector3 posicion in posicionesPuertas)
            {
                Puerta puerta = new Puerta(posicion);
                listaPuertas.Add(puerta);

            }


            return listaPuertas;
        }



        public Matrix transAbrePuerta(Vector3 posicionAntes)
        {
            Vector3 nuevaPosicion = posicionAntes + new Vector3(-32.5f, 0, -32.5f); // los valores estan calculados como ((Ancho/2) - (Espesor/2))
            Matrix translate = Matrix.Translation(nuevaPosicion);
            float angleY = FastMath.ToRad(90);
            Matrix rotation = Matrix.RotationYawPitchRoll(angleY, 0, 0);

            return rotation * translate;
        }

        public Matrix transCierraPuerta(Vector3 posicionAntes)
        {

            Vector3 nuevaPosicion = posicionAntes + new Vector3(-32.5f, 0, -32.5f);
            Matrix translate = Matrix.Translation(nuevaPosicion - new Vector3(-32.5f, 0, -32.5f));
            float angleY = FastMath.ToRad(90);

            return translate;
        }


        /// cuando hay colision entre boundingboxes y presiono R

        /*
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         */



    }



}
