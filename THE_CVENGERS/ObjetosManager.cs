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
            listaCamas.Add(new Cama(new Vector3(290, 6, 645), 90, new Vector3(0.4f, 0.4f, 0.4f)));

            return listaCamas;

        }
    }
}
