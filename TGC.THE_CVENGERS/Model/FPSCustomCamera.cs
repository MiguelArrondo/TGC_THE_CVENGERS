using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX.DirectInput;
using System;
using System.Collections.Generic;
using TGC.Core.Camara;
using TGC.Core.Geometry;
using TGC.Core.Input;
using TGC.Core.SceneLoader;
using TGC.Core.Utils;

namespace TGC.Group.Model
{
    internal class FPSCustomCamera : TgcCamera
    {
        //Constantes de movimiento
        public const float DEFAULT_ROTATION_SPEED = 2f;

        public const float DEFAULT_MOVEMENT_SPEED = 100f;
        public const float DEFAULT_JUMP_SPEED = 100f;

        // readonly Vector3 CAMERA_VELOCITY = new Vector3(DEFAULT_MOVEMENT_SPEED, DEFAULT_JUMP_SPEED, DEFAULT_MOVEMENT_SPEED);
        private readonly Vector3 CAMERA_POS = new Vector3(0.0f, 1.0f, 0.0f);

        //  readonly Vector3 CAMERA_ACCELERATION = new Vector3(400f, 400f, 400f);

        //Ejes para ViewMatrix
        private readonly Vector3 WORLD_XAXIS = new Vector3(1.0f, 0.0f, 0.0f);

        private readonly Vector3 WORLD_YAXIS = new Vector3(0.0f, 1.0f, 0.0f);
        private readonly Vector3 WORLD_ZAXIS = new Vector3(0.0f, 0.0f, 1.0f);
        private readonly Vector3 DEFAULT_UP_VECTOR = new Vector3(0.0f, 1.0f, 0.0f);

        private float accumPitchDegrees;
        private Vector3 eye;
        private Vector3 xAxis;
        private Vector3 yAxis;
        private Vector3 zAxis;
        private Vector3 viewDir;
        private Vector3 lookAt;

        private Vector3 directionnn = new Vector3(0.0f, 0.0f, 0.0f);

        private TgcScene escena;
        private List<Puerta> doors;
        private List<Objeto> objects;
        private List<Escondite> hides;

        public bool camaraEscondida = false;

        private TgcD3dInput Input;

        public Vector3 XAxis
        {
            get { return xAxis; }
            set { xAxis = value; }
        }

        public Vector3 YAxis
        {
            get { return yAxis; }
            set { yAxis = value; }
        }

        public Matrix ViewMatrix
        {
            get { return viewMatrix; }
        }

        public Vector3 ZAxis
        {
            get { return zAxis; }
            set { zAxis = value; }
        }

        //Banderas de Input
        private bool moveForwardsPressed = false;

        private bool moveBackwardsPressed = false;
        private bool moveRightPressed = false;
        private bool moveLeftPressed = false;
        private bool moveUpPressed = false;
        private bool moveDownPressed = false;

        #region Getters y Setters

        private bool enable;

        /// <summary>
        /// Habilita o no el uso de la camara
        /// </summary>
        public bool Enable
        {
            get { return enable; }
            set { enable = value; }
        }

        /// <summary>
        /// Velocidad de desplazamiento de los ejes XZ de la cámara
        /// </summary>
        public float MovementSpeed
        {
            get { return DEFAULT_MOVEMENT_SPEED; }
            /*       // set
					 {
					  //  velocity.X = value;
					  //  velocity.Z = value;
					 }*/
        }

        /// <summary>
        /// Velocidad de desplazamiento del eje Y de la cámara
        /// </summary>
        public float JumpSpeed
        {
            get { return DEFAULT_JUMP_SPEED; }
            //  set { velocity.Y = value; }
        }

        private float rotationSpeed;

        /// <summary>
        /// Velocidad de rotacion de la cámara
        /// </summary>
        public float RotationSpeed
        {
            get { return rotationSpeed; }
            set { rotationSpeed = value; }
        }

        private Matrix viewMatrix;
        /// <summary>
        /// View Matrix resultante
        /// </summary>

        /// <summary>
        /// Posicion actual de la camara
        /// </summary>
        //public Vector3 Position
        //{
        //	get { return eye; }
        //}

        /// <summary>
        /// Punto hacia donde mira la cámara
        /// </summary>
        //public Vector3 LookAt
        //{
        //	get { return lookAt; }
        //}

        private TgcD3dInput.MouseButtons rotateMouseButton;

        /// <summary>
        /// Boton del mouse que debe ser presionado para rotar la camara.
        /// Por default es boton izquierdo.
        /// </summary>
        public TgcD3dInput.MouseButtons RotateMouseButton
        {
            get { return rotateMouseButton; }
            set { rotateMouseButton = value; }
        }

        public bool MoveForwardsPressed
        {
            get
            {
                return moveForwardsPressed;
            }

            set
            {
                moveForwardsPressed = value;
            }
        }

        public bool MoveBackwardsPressed
        {
            get
            {
                return moveBackwardsPressed;
            }

            set
            {
                moveBackwardsPressed = value;
            }
        }

        public bool MoveRightPressed
        {
            get
            {
                return moveRightPressed;
            }

            set
            {
                moveRightPressed = value;
            }
        }

        public bool MoveLeftPressed
        {
            get
            {
                return moveLeftPressed;
            }

            set
            {
                moveLeftPressed = value;
            }
        }

        public bool MoveUpPressed
        {
            get
            {
                return moveUpPressed;
            }

            set
            {
                moveUpPressed = value;
            }
        }

        public bool MoveDownPressed
        {
            get
            {
                return moveDownPressed;
            }

            set
            {
                moveDownPressed = value;
            }
        }

        public Vector3 Direction
        {
            get
            {
                return directionnn;
            }

            set
            {
                directionnn = value;
            }
        }

        #endregion Getters y Setters

        /// <summary>
        /// Crea la cámara con valores iniciales.
        /// Aceleración desactivada por Default
        /// </summary>
        public FPSCustomCamera(TgcD3dInput input)
        {
            Input = input;
            resetValues();
        }

        /// <summary>
        /// Carga los valores default de la camara
        /// </summary>
        public void resetValues()
        {
            accumPitchDegrees = 0.0f;
            rotationSpeed = DEFAULT_ROTATION_SPEED;
            eye = new Vector3(0.0f, 0.0f, 0.0f);
            xAxis = new Vector3(1.0f, 0.0f, 0.0f);
            yAxis = new Vector3(0.0f, 1.0f, 0.0f);
            zAxis = new Vector3(0.0f, 0.0f, 1.0f);
            viewDir = new Vector3(0.0f, 0.0f, 1.0f);
            lookAt = eye + viewDir;

            //    accelerationEnable = false;
            //    acceleration = CAMERA_ACCELERATION;
            //   currentVelocity = new Vector3(0.0f, 0.0f, 0.0f);
            //   velocity = CAMERA_VELOCITY;
            viewMatrix = Matrix.Identity;
            setPosition(CAMERA_POS);

            rotateMouseButton = TgcD3dInput.MouseButtons.BUTTON_LEFT;
        }

        public void setearCamara(float elapsedTime, Vector3 eyeC, Vector3 look, Matrix viewM, Vector3 X, Vector3 Y, Vector3 Z, Vector3 direc)
        {
            accumPitchDegrees = 0.0f;
            rotationSpeed = DEFAULT_ROTATION_SPEED;
            eye = new Vector3(0f, 0f, 0f);
            xAxis = X;
            yAxis = Y;
            zAxis = Z;
            viewDir = new Vector3(0.0f, 0.0f, 1.0f);
            lookAt = look;

            //    accelerationEnable = false;
            //    acceleration = CAMERA_ACCELERATION;
            //    currentVelocity = new Vector3(0.0f, 0.0f, 0.0f);
            //    velocity = CAMERA_VELOCITY;
            viewMatrix = viewM;
            setPosition(eyeC);

            rotateMouseButton = TgcD3dInput.MouseButtons.BUTTON_LEFT;

            Vector3 dir = new Vector3(0, 0, 0);

            if (direc.X != 0f)
            {
                if (direc.X < 0f)
                {
                    dir.X += 2f;
                }
                else dir.X -= 2f;
            }

            if (direc.Y != 0f)
            {
                if (direc.Y < 0f)
                {
                    dir.Y += 2f;
                }
                else dir.Y -= 2f;
            }

            if (direc.Z != 0f)
            {
                if (direc.Z < 0f)
                {
                    dir.Z += 2f;
                }
                else dir.Z -= 2f;
            }

            updatePosition(direc + dir, elapsedTime);
            //updateVelocity(direc + dir, GuiController.Instance.ElapsedTime);
        }

        public void setearCamara(Vector3 eyeC, Vector3 look)
        {
            accumPitchDegrees = 0.0f;
            rotationSpeed = DEFAULT_ROTATION_SPEED;
            eye = eyeC;

            viewDir = new Vector3(0.0f, 0.0f, 1.0f);
            lookAt = look;

            //    accelerationEnable = false;
            //    acceleration = CAMERA_ACCELERATION;
            //    currentVelocity = new Vector3(0.0f, 0.0f, 0.0f);
            //    velocity = CAMERA_VELOCITY;
            setPosition(eyeC);

            rotateMouseButton = TgcD3dInput.MouseButtons.BUTTON_LEFT;

            //  move(eyeC.X, eyeC.Y, eyeC.Z);
        }

        /// <summary>
        /// Configura la posicion de la cámara
        /// </summary>
        private void setCamera(Vector3 eye, Vector3 target, Vector3 up)
        {
            this.eye = eye;

            zAxis = target - eye;
            zAxis.Normalize();

            viewDir = zAxis;
            lookAt = eye + viewDir;

            xAxis = Vector3.Cross(up, zAxis);
            xAxis.Normalize();

            yAxis = Vector3.Cross(zAxis, xAxis);
            yAxis.Normalize();
            //xAxis.Normalize();

            viewMatrix = Matrix.Identity;

            viewMatrix.M11 = xAxis.X;
            viewMatrix.M21 = xAxis.Y;
            viewMatrix.M31 = xAxis.Z;
            viewMatrix.M41 = -Vector3.Dot(xAxis, eye);

            viewMatrix.M12 = yAxis.X;
            viewMatrix.M22 = yAxis.Y;
            viewMatrix.M32 = yAxis.Z;
            viewMatrix.M42 = -Vector3.Dot(yAxis, eye);

            viewMatrix.M13 = zAxis.X;
            viewMatrix.M23 = zAxis.Y;
            viewMatrix.M33 = zAxis.Z;
            viewMatrix.M43 = -Vector3.Dot(zAxis, eye);

            // Extract the pitch angle from the view matrix.
            accumPitchDegrees = Geometry.RadianToDegree((float)-Math.Asin((double)viewMatrix.M23));
        }

        /// <summary>
        /// Configura la posicion de la cámara
        /// </summary>
        public void setCamera(Vector3 pos, Vector3 lookAt, TgcScene scene, List<Puerta> puertas, List<Objeto> objetos, List<Escondite> escondites)
        {
            escena = scene;
            doors = puertas;
            objects = objetos;
            hides = escondites;
            setCamera(pos, lookAt, DEFAULT_UP_VECTOR);
        }

        /// <summary>
        /// Moves the camera by dx world units to the left or right; dy
        /// world units upwards or downwards; and dz world units forwards
        /// or backwards.
        /// </summary>
        private void move(float dx, float dy, float dz)
        {
            Vector3 auxEye = this.eye;
            Vector3 forwards;

            // Calculate the forwards direction. Can't just use the camera's local
            // z axis as doing so will cause the camera to move more slowly as the
            // camera's view approaches 90 degrees straight up and down.
            forwards = Vector3.Cross(xAxis, WORLD_YAXIS);
            forwards.Normalize();

            auxEye += xAxis * dx;
            auxEye += WORLD_YAXIS * dy;
            auxEye += forwards * dz;

            TgcBoundingSphere spherePrueba = new TgcBoundingSphere(auxEye, 20f);
            bool collisionFound = false;

            foreach (TgcMesh meshi in escena.Meshes)
            {
                if (TgcCollisionUtils.testSphereAABB(spherePrueba, meshi.BoundingBox))
                {
                    collisionFound = true;
                }
            }

            foreach (Objeto meshi in objects)
            {
                if (TgcCollisionUtils.testSphereAABB(spherePrueba, meshi.getMesh().BoundingBox))
                {
                    collisionFound = true;
                }
            }

            foreach (Puerta meshi in doors)
            {
                if (TgcCollisionUtils.testSphereAABB(spherePrueba, meshi.getMesh().BoundingBox))
                {
                    collisionFound = true;
                }
            }

            foreach (Escondite meshi in hides)
            {
                if (TgcCollisionUtils.testSphereAABB(spherePrueba, meshi.getMesh().BoundingBox))
                {
                    collisionFound = true;
                }
            }

            if (!collisionFound)
            {
                setPosition(auxEye);
            }
        }

        /// <summary>
        /// Moves the camera by the specified amount of world units in the specified
        /// direction in world space.
        /// </summary>
        private void move(Vector3 direction, Vector3 amount)
        {
            eye.X += direction.X * amount.X;
            eye.Y += direction.Y * amount.Y;
            eye.Z += direction.Z * amount.Z;

            reconstructViewMatrix(false);
        }

        /// <summary>
        /// Rotates the camera based on its current behavior.
        /// Note that not all behaviors support rolling.
        ///
        /// This Camera class follows the left-hand rotation rule.
        /// Angles are measured clockwise when looking along the rotation
        /// axis toward the origin. Since the Z axis is pointing into the
        /// screen we need to negate rolls.
        /// </summary>
        private void rotate(float headingDegrees, float pitchDegrees, float rollDegrees)
        {
            rollDegrees = -rollDegrees;
            rotateFirstPerson(headingDegrees, pitchDegrees);
            reconstructViewMatrix(true);
        }

        /// <summary>
        /// This method applies a scaling factor to the rotation angles prior to
        /// using these rotation angles to rotate the camera. This method is usually
        /// called when the camera is being rotated using an input device (such as a
        /// mouse or a joystick).
        /// </summary>
        private void rotateSmoothly(float headingDegrees, float pitchDegrees, float rollDegrees)
        {
            headingDegrees *= rotationSpeed;
            pitchDegrees *= rotationSpeed;
            rollDegrees *= rotationSpeed;

            rotate(headingDegrees, pitchDegrees, rollDegrees);
        }

        /// <summary>
        /// Moves the camera using Newton's second law of motion. Unit mass is
        /// assumed here to somewhat simplify the calculations. The direction vector
        /// is in the range [-1,1].
        /// </summary>
        private void updatePosition(Vector3 direction, float elapsedTimeSec)
        {
            // if (Vector3.LengthSq(currentVelocity) != 0.0f)
            {
                // Only move the camera if the velocity vector is not of zero length.
                // Doing this guards against the camera slowly creeping around due to
                // floating point rounding errors.

                Vector3 displacement;
                //    if (accelerationEnable)
                {
                    //      displacement = (currentVelocity * elapsedTimeSec) +
                    //     (0.5f * acceleration * elapsedTimeSec * elapsedTimeSec);
                }
                //  else
                {
                    displacement = (//currentVelocity
                        new Vector3(200f * elapsedTimeSec, 200f * elapsedTimeSec, 200f * elapsedTimeSec));
                }

                // Floating point rounding errors will slowly accumulate and cause the
                // camera to move along each axis. To prevent any unintended movement
                // the displacement vector is clamped to zero for each direction that
                // the camera isn't moving in. Note that the updateVelocity() method
                // will slowly decelerate the camera's velocity back to a stationary
                // state when the camera is no longer moving along that direction. To
                // account for this the camera's current velocity is also checked.

                if (direction.X == 0.0f //&& Math.Abs(currentVelocity.X) < 1e-6f
                    )
                    displacement.X = 0.0f;

                if (direction.Y == 0.0f //&& Math.Abs(currentVelocity.Y) < 1e-6f
                    )
                    displacement.Y = 0.0f;

                if (direction.Z == 0.0f //&& Math.Abs(currentVelocity.Z) < 1e-6f
                    )
                    displacement.Z = 0.0f;

                if (direction.Z < 0.0f //&& Math.Abs(currentVelocity.Z) < 1e-6f
                   )
                    displacement.Z = -displacement.Z;
                if (direction.Y < 0.0f //&& Math.Abs(currentVelocity.Y) < 1e-6f
                    )
                    displacement.Y = -displacement.Y;
                if (direction.X < 0.0f //&& Math.Abs(currentVelocity.X) < 1e-6f
                    )
                    displacement.X = -displacement.X;

                move(displacement.X, displacement.Y, displacement.Z);
            }

            // Continuously update the camera's velocity vector even if the camera
            // hasn't moved during this call. When the camera is no longer being moved
            // the camera is decelerating back to its stationary state.

            //  if (accelerationEnable)
            {
                //    updateVelocity(direction, elapsedTimeSec);
            }
            // else
            {
                //  updateVelocityNoAcceleration(direction);
            }
        }

        private void setPosition(Vector3 pos)
        {
            eye = pos;
            reconstructViewMatrix(true);
        }

        private void rotateFirstPerson(float headingDegrees, float pitchDegrees)
        {
            accumPitchDegrees += pitchDegrees;

            if (accumPitchDegrees > 90.0f)
            {
                pitchDegrees = 90.0f - (accumPitchDegrees - pitchDegrees);
                accumPitchDegrees = 90.0f;
            }

            if (accumPitchDegrees < -90.0f)
            {
                pitchDegrees = -90.0f - (accumPitchDegrees - pitchDegrees);
                accumPitchDegrees = -90.0f;
            }

            float heading = Geometry.DegreeToRadian(headingDegrees);
            float pitch = Geometry.DegreeToRadian(pitchDegrees);

            Matrix rotMtx;
            Vector4 result;

            // Rotate camera's existing x and z axes about the world y axis.
            if (heading != 0.0f)
            {
                rotMtx = Matrix.RotationY(heading);

                result = Vector3.Transform(xAxis, rotMtx);
                xAxis = new Vector3(result.X, result.Y, result.Z);

                result = Vector3.Transform(zAxis, rotMtx);
                zAxis = new Vector3(result.X, result.Y, result.Z);
            }

            // Rotate camera's existing y and z axes about its existing x axis.
            if (pitch != 0.0f)
            {
                rotMtx = Matrix.RotationAxis(xAxis, pitch);

                result = Vector3.Transform(yAxis, rotMtx);
                yAxis = new Vector3(result.X, result.Y, result.Z);

                result = Vector3.Transform(zAxis, rotMtx);
                zAxis = new Vector3(result.X, result.Y, result.Z);
            }
        }

        /// <summary>
        /// Reconstruct the view matrix.
        /// </summary>*/
        private void reconstructViewMatrix(bool orthogonalizeAxes)
        {
            if (orthogonalizeAxes)
            {
                // Regenerate the camera's local axes to orthogonalize them.

                zAxis.Normalize();

                yAxis = Vector3.Cross(zAxis, xAxis);
                yAxis.Normalize();

                xAxis = Vector3.Cross(yAxis, zAxis);
                xAxis.Normalize();

                viewDir = zAxis;
                lookAt = eye + viewDir;
            }

            // Reconstruct the view matrix.

            viewMatrix.M11 = xAxis.X;
            viewMatrix.M21 = xAxis.Y;
            viewMatrix.M31 = xAxis.Z;
            viewMatrix.M41 = -Vector3.Dot(xAxis, eye);

            viewMatrix.M12 = yAxis.X;
            viewMatrix.M22 = yAxis.Y;
            viewMatrix.M32 = yAxis.Z;
            viewMatrix.M42 = -Vector3.Dot(yAxis, eye);

            viewMatrix.M13 = zAxis.X;
            viewMatrix.M23 = zAxis.Y;
            viewMatrix.M33 = zAxis.Z;
            viewMatrix.M43 = -Vector3.Dot(zAxis, eye);

            viewMatrix.M14 = 0.0f;
            viewMatrix.M24 = 0.0f;
            viewMatrix.M34 = 0.0f;
            viewMatrix.M44 = 1.0f;
        }

        /// <summary>
        /// Actualiza los valores de la camara
        /// </summary>
        public override void updateCamera(float elapsedTime)
        {
            //Si la camara no está habilitada, no procesar el resto del input
            if (!enable)
            {
                return;
            }

            float elapsedTimeSec = elapsedTime;
            TgcD3dInput d3dInput = Input;

            //Imprimir por consola la posicion actual de la camara
            if ((d3dInput.keyDown(Key.LeftShift) || d3dInput.keyDown(Key.RightShift)) && d3dInput.keyPressed(Key.P))
            {
                //GuiController.Instance.printCurrentPosition();
                return;
            }

            float heading = 0.0f;
            float pitch = 0.0f;

            //Obtener direccion segun entrada de teclado
            Vector3 direction = getMovementDirection(d3dInput);

            directionnn = direction;
            pitch = d3dInput.YposRelative * rotationSpeed;
            heading = d3dInput.XposRelative * rotationSpeed;

            if (!camaraEscondida)
            {
                //Solo rotar si se esta aprentando el boton del mouse configurado

                rotate(heading, pitch, 0.0f);
            }

            updatePosition(direction, elapsedTimeSec);
        }

        /// <summary>
        /// Actualiza la ViewMatrix, si es que la camara esta activada
        /// </summary>
        public void updateViewMatrix(Microsoft.DirectX.Direct3D.Device d3dDevice)
        {
            if (!enable)
            {
                return;
            }

            d3dDevice.Transform.View = viewMatrix;
        }

        /// <summary>
        /// Obtiene la direccion a moverse por la camara en base a la entrada de teclado
        /// </summary>
        private Vector3 getMovementDirection(TgcD3dInput d3dInput)
        {
            Vector3 direction = new Vector3(0.0f, 0.0f, 0.0f);

            //Forward
            if (d3dInput.keyDown(Key.W))
            {
                if (!moveForwardsPressed)
                {
                    moveForwardsPressed = true;
                    //   currentVelocity = new Vector3(currentVelocity.X, currentVelocity.Y, 0.0f);
                }

                direction.Z += 1.0f;
            }
            else
            {
                moveForwardsPressed = false;
            }

            //Backward
            if (d3dInput.keyDown(Key.S))
            {
                if (!moveBackwardsPressed)
                {
                    moveBackwardsPressed = true;
                    //   currentVelocity = new Vector3(currentVelocity.X, currentVelocity.Y, 0.0f);
                }

                direction.Z -= 1.0f;
            }
            else
            {
                moveBackwardsPressed = false;
            }

            //Strafe right
            if (d3dInput.keyDown(Key.D))
            {
                if (!moveRightPressed)
                {
                    moveRightPressed = true;
                    //  currentVelocity = new Vector3(0.0f, currentVelocity.Y, currentVelocity.Z);
                }

                direction.X += 1.0f;
            }
            else
            {
                moveRightPressed = false;
            }

            //Strafe left
            if (d3dInput.keyDown(Key.A))
            {
                if (!moveLeftPressed)
                {
                    moveLeftPressed = true;
                    //  currentVelocity = new Vector3(0.0f, currentVelocity.Y, currentVelocity.Z);
                }

                direction.X -= 1.0f;
            }
            else
            {
                moveLeftPressed = false;
            }

            return direction;
        }

        public Vector3 getPosition()
        {
            return eye;
        }

        public Vector3 getLookAt()
        {
            return this.lookAt;
        }

        /// <summary>
        /// String de codigo para setear la camara desde GuiController, con la posicion actual y direccion de la camara
        /// </summary>
        internal string getPositionCode()
        {
            //TODO ver de donde carajo sacar el LookAt de esta camara
            Vector3 lookAt = this.LookAt;

            return "GuiController.Instance.setCamera(new Vector3(" +
                TgcParserUtils.printFloat(eye.X) + "f, " + TgcParserUtils.printFloat(eye.Y) + "f, " + TgcParserUtils.printFloat(eye.Z) + "f), new Vector3(" +
                TgcParserUtils.printFloat(lookAt.X) + "f, " + TgcParserUtils.printFloat(lookAt.Y) + "f, " + TgcParserUtils.printFloat(lookAt.Z) + "f));";
        }

        public override Matrix getViewMatrix()
        {
            return viewMatrix;
        }
    }
}