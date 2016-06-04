using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlumnoEjemplos.THE_CVENGERS
{
    class ObjetosManager
    {
        List<Cama> listaCamas = new List<Cama>();
        

        public ObjetosManager() { }

        public List<Cama> initCamas()
        {
            listaCamas.Add(new Cama(new Vector3(290, 6, 645), 90, new Vector3(0.4f, 0.4f, 0.4f), "My+Room-TgcScene.xml"));
            listaCamas.Add(new Cama(new Vector3(176, 6, 779), 90, new Vector3(0.4f, 0.4f, 0.4f), "My+Room-TgcScene.xml"));
            listaCamas.Add(new Cama(new Vector3(176, 6, 844), 90, new Vector3(0.4f, 0.4f, 0.4f), "My+Room-TgcScene.xml"));
            listaCamas.Add(new Cama(new Vector3(297, 6, 714), 0, new Vector3(0.4f, 0.4f, 0.4f), "My+Room-TgcScene.xml"));
            listaCamas.Add(new Cama(new Vector3(15, 0, 295), 0, new Vector3(0.25f, 0.25f, 0.25f), "chair+2-TgcScene.xml"));
            return listaCamas;

        }
    }
}
