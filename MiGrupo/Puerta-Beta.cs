using System;
using System.Collections.Generic;
using System.Text;
using TgcViewer.Example;
using TgcViewer;
using Microsoft.DirectX.Direct3D;
using System.Drawing;
using Microsoft.DirectX;
using TgcViewer.Utils.Modifiers;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.Input;
using Microsoft.DirectX.DirectInput;

using TgcViewer.Utils.TgcSceneLoader;


namespace AlumnoEjemplos.MiGrupo
{
    /// <summary>
    /// Tutorial 1:
    /// Unidades Involucradas:
    ///     # Unidad 3 - Conceptos Básicos de 3D - Mesh
    /// 
    /// Muestra como crear una caja 3D de color y como mostrarla por pantalla.
    /// 
    /// Autor: Matías Leone
    /// 
    /// </summary>
    public class Tutorial1 : TgcExample
    {

        //Variable para caja 3D
        TgcBox box;


        public override string getCategory()
        {
            return "MiGrupo";
        }

        public override string getName()
        {
            return "Puerta";
        }

        public override string getDescription()
        {
            return "Beta de la puerta, algo me esta boludeando";
        }
        /// <summary>
        /// Método en el que se deben crear todas las cosas que luego se van a querer usar.
        /// Es invocado solo una vez al inicio del ejemplo.
        /// </summary>
        public override void init()
        {
            //Acceso a Device de DirectX. Siempre conviene tenerlo a mano. Suele ser pedido como parámetro de varios métodos
            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;

            //Creamos una caja 3D de color rojo, ubicada en el origen y lado 10
            Vector3 center = new Vector3(0, 0, 0);
            Vector3 size = new Vector3(50, 100, 10);
            Color color = Color.Chartreuse;
            TgcTexture texture = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosMediaDir + "wood-door.jpg");

            box = TgcBox.fromSize(center, size, texture);

            //Todos los recursos que se van a necesitar (objetos 3D, meshes, texturas, etc) se deben cargar en el metodo init().
            //Crearlos cada vez en el metodo render() es un error grave. Destruye la performance y suele provocar memory leaks.

            box.AutoTransformEnable = false;

            //Ubicar la camara del framework mirando al centro de este objeto.
            //La camara por default del framework es RotCamera, cuyo comportamiento es
            //centrarse sobre un objeto y permitir rotar y hacer zoom con el mouse.
            //Con clic izquierdo del mouse se rota la cámara, con clic derecho se traslada y con la rueda
            //del mouse se hace zoom.
            //Otras cámaras disponibles son: FpsCamera (1ra persona) y ThirdPersonCamera (3ra persona).
            GuiController.Instance.RotCamera.targetObject(box.BoundingBox);
        }

        /// <summary>
        /// Creo la caja en una posicion determinada y le agrego textura.
        /// El booleano open me dice que la puerta está cerrada al arrancar.
        /// </summary>

        bool open = false;

        public override void render(float elapsedTime)
        {
            //Acceso a Device de DirectX. Siempre conviene tenerlo a mano. Suele ser pedido como parámetro de varios métodos
            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;
            TgcD3dInput input = GuiController.Instance.D3dInput;

            //Dibujar la caja en pantalla
            box.render();



            if (input.keyUp(Key.R))
            {
                if (!open)
                {
                    box.Transform = transAbrePuerta(elapsedTime * 0.01f); //prueba para bajar el tiempo y que la animacion quede mejor
                    open = true;
                    box.render();

                }

                else
                {
                    box.Transform = transCierraPuerta(elapsedTime * 0.01f);
                    open = false;
                    box.render();

                }

            }

        }

        /// <summary>
        /// Creo las matrices afuera para que sea un poco mas claro
        /// </summary>
        /// 

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
            //Liberar memoria de la puerta

            box.dispose();
        }

    }
}