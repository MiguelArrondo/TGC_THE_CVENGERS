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
        public Vector3 posicion { get; set; }
        float rotacion;
        Vector3 escala;
        Vector3 traslado;
        float angApertura;
        float contadorApertura;
        Matrix rotacionActual;
        Matrix rotacionFinal;
        Matrix rotacionOriginal;
        public int contadorVillano { get; set; }
        public bool villanoAbriendoPrimera { get; set; }
        public bool villanoAbriendoSiguientes { get; set; }
        Vector3 posicionOriginal;

        public bool siendoAbiertaPorVillano = false;

        Puerta puertaSimulada;




        public Puerta(Vector3 posicionCentro, float rotacion,Vector3 escalas, Vector3 traslado2, float angApertura2)
        {

            this.open = false;

            TgcSceneLoader loadedrL = new TgcSceneLoader();
            this.Mesh = loadedrL.loadSceneFromFile(GuiController.Instance.AlumnoEjemplosDir + "THE_CVENGERS\\AlumnoMedia\\ObjetosMapa\\Puerta\\puerta-TgcScene.xml").Meshes[0];
            this.Mesh.AutoTransformEnable = false;
            this.Mesh.AutoUpdateBoundingBox = false;
            this.posicionOriginal = posicionCentro; 
            this.posicion = posicionCentro;
            this.traslado = traslado2;
            this.angApertura = angApertura2;
            this.rotacion = rotacion;
            this.contadorVillano = 0;
            this.contadorApertura = 0;
            this.villanoAbriendoPrimera = false;
            this.villanoAbriendoSiguientes = false;

            this.escala = escalas;
            //aca escalas

            Matrix matrizEscala = Matrix.Scaling(escalas.X,escalas.Y,escalas.Z);
            Matrix matrizPosicion = Matrix.Translation(posicionCentro);


         //   this.Mesh.move(posicionCentro);
        
                float angleY = FastMath.ToRad(rotacion);
                Matrix matrizRotacion = Matrix.RotationY(angleY);
            this.rotacionActual = matrizRotacion;
            this.rotacionOriginal = matrizRotacion;


            this.Mesh.Transform = matrizRotacion * matrizEscala *matrizPosicion;
            this.Mesh.BoundingBox.transform(this.Mesh.Transform);




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
            Vector3 nuevaPosicion = this.posicion + this.traslado; // los valores estan calculados como ((Ancho/2) - (Espesor/2))
            Matrix translate = Matrix.Translation(nuevaPosicion);
            //seteo la nueva posicion

            this.posicion = nuevaPosicion;

            this.contadorApertura = this.contadorApertura + angApertura;
            

            float angleY = FastMath.ToRad(contadorApertura);
            Matrix rotation = Matrix.RotationY(angleY);

            
            
                rotation = rotation * rotacionActual;
            

          //  

            //this.Mesh.move(nuevaPosicion);

            Matrix matrizEscala = Matrix.Scaling(this.escala.X, this.escala.Y, this.escala.Z);
    
            this.Mesh.Transform = rotation * matrizEscala * translate;
            this.Mesh.render();
            this.rotacionFinal = rotation;
            this.Mesh.BoundingBox.transform(this.Mesh.Transform);
            

        }

        public void cerrarPuerta()
        {

            Vector3 nuevaPosicion = this.posicion - this.traslado;
            Matrix translate = Matrix.Translation(nuevaPosicion);

            this.posicion = nuevaPosicion;

            this.contadorApertura = this.contadorApertura - angApertura;

            float angleY = FastMath.ToRad(contadorApertura);
            Matrix rotation = Matrix.RotationY(angleY);
            rotation = rotation * rotacionActual;
            Matrix matrizEscala = Matrix.Scaling(this.escala.X, this.escala.Y, this.escala.Z);

            this.Mesh.Transform = rotation * matrizEscala * translate;
            this.Mesh.render();
            this.Mesh.BoundingBox.transform(this.Mesh.Transform);

        }

        public void accionarPuerta()
        {
            if (this.open)
            {
                this.cerrarPuerta();
            }
            else this.abrirPuerta();
        }

        public void cambiarStatus()
        {
            if (this.open)
            {
                this.setStatus(false);
                rotacionActual = rotacionOriginal;
            }
            else
            {
                this.setStatus(true);
                rotacionActual = rotacionFinal;
            }

                this.contadorApertura = 0;
            
        }

        public bool puedeAbrirseSinTrabarse(TgcBoundingSphere spherePuertas)
        {
            return !TgcCollisionUtils.testSphereAABB(spherePuertas, this.BoundingBoxSimulada());
        }

        private TgcBoundingBox BoundingBoxSimulada()
        {
            puertaSimulada = new Puerta(this.posicionOriginal, this.rotacion, this.escala, this.traslado, this.angApertura);

            if (!this.getStatus())
            {
                puertaSimulada.simularApertura();
                return puertaSimulada.getMesh().BoundingBox;
            }
            else
            {

                puertaSimulada.simularClausura();
                return puertaSimulada.getMesh().BoundingBox;
            }

        }

        private void simularApertura()
        {
            Vector3 nuevaPosicion = this.posicionOriginal + this.traslado*10; // los valores estan calculados como ((Ancho/2) - (Espesor/2))
            Matrix translate = Matrix.Translation(nuevaPosicion);
            //seteo la nueva posicion

            this.posicion = nuevaPosicion;

            this.contadorApertura = this.contadorApertura + angApertura*10;


            float angleY = FastMath.ToRad(contadorApertura);
            Matrix rotation = Matrix.RotationY(angleY);



            rotation = rotation * rotacionActual;


            //  

            //this.Mesh.move(nuevaPosicion);

            Matrix matrizEscala = Matrix.Scaling(this.escala.X, this.escala.Y, this.escala.Z);

            this.Mesh.Transform = rotation * matrizEscala * translate;
            this.rotacionFinal = rotation;
            this.Mesh.BoundingBox.transform(this.Mesh.Transform);
        }

        private void simularClausura()
        {
            Vector3 nuevaPosicion = this.posicionOriginal + this.traslado * 10; // los valores estan calculados como ((Ancho/2) - (Espesor/2))
            Matrix translate = Matrix.Translation(nuevaPosicion);
            //seteo la nueva posicion

            this.posicion = nuevaPosicion;

            this.contadorApertura = this.contadorApertura + angApertura * 10;


            float angleY = FastMath.ToRad(contadorApertura);
            Matrix rotation = Matrix.RotationY(angleY);



            rotation = rotation * rotacionActual;


            //  

            //this.Mesh.move(nuevaPosicion);

            Matrix matrizEscala = Matrix.Scaling(this.escala.X, this.escala.Y, this.escala.Z);

            this.Mesh.Transform = rotation * matrizEscala * translate;
            this.rotacionFinal = rotation;
            this.Mesh.BoundingBox.transform(this.Mesh.Transform);

            nuevaPosicion = this.posicion - this.traslado*10;
            translate = Matrix.Translation(nuevaPosicion);

            this.posicion = nuevaPosicion;

            this.contadorApertura = this.contadorApertura - angApertura*10;

            angleY = FastMath.ToRad(contadorApertura);
            rotation = Matrix.RotationY(angleY);
            rotation = rotation * rotacionActual;
            matrizEscala = Matrix.Scaling(this.escala.X, this.escala.Y, this.escala.Z);

            this.Mesh.Transform = rotation * matrizEscala * translate;
            this.Mesh.BoundingBox.transform(this.Mesh.Transform);
        }
    }
}
