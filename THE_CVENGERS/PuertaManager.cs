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


            listaPuertas.Add(new Puerta(new Vector3(600, 0, 560), 0, new Vector3(1.3f, 0.7f, 1), new Vector3(0.8f, 0, -0.1f), -0.9f));
            listaPuertas.Add(new Puerta(new Vector3(415, 0, 714), 90, new Vector3(1.3f, 0.7f, 1.2f), new Vector3(-0.1f, 0, 0.76f), 0.9f));
            listaPuertas.Add(new Puerta(new Vector3(599, 0, 839), 0, new Vector3(1.3f, 0.7f, 1), new Vector3(0.8f, 0, -0.1f), -0.9f));
            listaPuertas.Add(new Puerta(new Vector3(750, 0, 716), 90, new Vector3(1.3f, 0.7f, 1.2f), new Vector3(-0.1f, 0, 0.76f), 0.9f));
            listaPuertas.Add(new Puerta(new Vector3(760, 0, 920), 90, new Vector3(1.3f, 0.7f, 1f), new Vector3(-0.1f, 0, 0.6f), 0.9f));
            listaPuertas.Add(new Puerta(new Vector3(229, 0, 930), 90, new Vector3(1.3f, 0.7f, 0.68f), new Vector3(-0.1f, 0, 0.45f), 0.9f));
            listaPuertas.Add(new Puerta(new Vector3(240, 0, 375), 90, new Vector3(1.3f, 0.7f, 1.15f), new Vector3(-0.1f, 0, 0.73f), 0.9f));
            listaPuertas.Add(new Puerta(new Vector3(123, 0, 155), 0, new Vector3(0.9f, 0.7f, 1.15f), new Vector3(-0.60f, 0, -0.1f), 0.9f));
            listaPuertas.Add(new Puerta(new Vector3(484, 0, 200), 0, new Vector3(0.9f, 0.7f, 1.15f), new Vector3(-0.60f, 0, -0.1f), 0.9f));
            listaPuertas.Add(new Puerta(new Vector3(693, 0, 273), 90, new Vector3(0.9f, 0.7f, 1.15f), new Vector3(0.60f, 0, 0.1f), -0.9f));

            return listaPuertas;
        }




         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
         
        



    }



}
