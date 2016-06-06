using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlumnoEjemplos.THE_CVENGERS
{
    class ObjetosManager
    {
        List<Objeto> listaObjetos = new List<Objeto>();
        List<Escondite> listaEscondites = new List<Escondite>();
        List<Objeto> listaFotos = new List<Objeto>();

        public ObjetosManager() { }

        public List<Objeto> initObjetos()
        {
            listaObjetos.Add(new Objeto(new Vector3(290, 6, 645), 90, new Vector3(0.4f, 0.4f, 0.4f), "My+Room-TgcScene.xml"));
            listaObjetos.Add(new Objeto(new Vector3(176, 6, 779), 90, new Vector3(0.4f, 0.4f, 0.4f), "My+Room-TgcScene.xml"));
            listaObjetos.Add(new Objeto(new Vector3(176, 6, 844), 90, new Vector3(0.4f, 0.4f, 0.4f), "My+Room-TgcScene.xml"));
            listaObjetos.Add(new Objeto(new Vector3(297, 6, 714), 0, new Vector3(0.4f, 0.4f, 0.4f), "My+Room-TgcScene.xml"));
            listaObjetos.Add(new Objeto(new Vector3(15, 0, 295), 0, new Vector3(0.25f, 0.25f, 0.25f), "chair+2-TgcScene.xml"));
            listaObjetos.Add(new Objeto(new Vector3(530, 0, 600), -90, new Vector3(0.5f, 0.5f, 0.5f), "Wardrobe2-TgcScene.xml"));
            return listaObjetos;

        }

        public List<Escondite> initEscondites()
        {
            listaEscondites.Add(new Escondite(new Vector3(530, 0, 600), -90, new Vector3(0.5f, 0.5f, 0.5f), "Wardrobe2-TgcScene.xml", new Vector3(503, 45, 630), new Vector3(3000, 44, 4500)));
            listaEscondites.Add(new Escondite(new Vector3(815, 0, 68), -90, new Vector3(0.5f, 0.5f, 0.5f), "Wardrobe2-TgcScene.xml", new Vector3(790, 45, 100), new Vector3(90, 44, 6000)));
            listaEscondites.Add(new Escondite(new Vector3(185, 0, 473), 90, new Vector3(0.5f, 0.5f, 0.5f), "Wardrobe2-TgcScene.xml", new Vector3(209, 45, 440), new Vector3(180, 44, -90)));

            return listaEscondites;

        }

        public List<Objeto> initFotos()
        {
            listaFotos.Add(new Objeto(new Vector3(645, 45, 384), 90, new Vector3(0.15f, 0.15f, 0.15f), "foto1-TgcScene.xml"));
            listaFotos.Add(new Objeto(new Vector3(581, 45, 23), 0, new Vector3(0.15f, 0.15f, 0.15f), "foto2-TgcScene.xml"));
            listaFotos.Add(new Objeto(new Vector3(51, 45, 918), 90, new Vector3(0.15f, 0.15f, 0.15f), "foto3-TgcScene.xml"));
            return listaFotos;
        }
    }
}
