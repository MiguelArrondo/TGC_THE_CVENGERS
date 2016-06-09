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
        List<Objeto> listaItems = new List<Objeto>();

        public ObjetosManager() { }

        public List<Objeto> initObjetos()
        {
            listaObjetos.Add(new Objeto(new Vector3(290, 6, 645), 90, new Vector3(0.4f, 0.4f, 0.4f), "My+Room-TgcScene.xml"));
            listaObjetos.Add(new Objeto(new Vector3(176, 6, 779), 90, new Vector3(0.4f, 0.4f, 0.4f), "My+Room-TgcScene.xml"));
            listaObjetos.Add(new Objeto(new Vector3(176, 6, 844), 90, new Vector3(0.4f, 0.4f, 0.4f), "My+Room-TgcScene.xml"));
            listaObjetos.Add(new Objeto(new Vector3(297, 6, 714), 0, new Vector3(0.4f, 0.4f, 0.4f), "My+Room-TgcScene.xml"));
            listaObjetos.Add(new Objeto(new Vector3(385, 0, 240), 180, new Vector3(0.8f, 0.8f, 0.8f), "My+Room-TgcScene.xml"));
            listaObjetos.Add(new Objeto(new Vector3(45, 0, 320), 0, new Vector3(0.25f, 0.25f, 0.25f), "chair+2-TgcScene.xml"));
            listaObjetos.Add(new Objeto(new Vector3(410, 0, 876), 0, new Vector3(0.20f, 0.20f, 0.20f), "mesita-TgcScene.xml"));
            listaObjetos.Add(new Objeto(new Vector3(929, 0, 252), 0, new Vector3(0.20f, 0.20f, 0.20f), "mesita-TgcScene.xml"));
            listaObjetos.Add(new Objeto(new Vector3(210, 0, 302), 0, new Vector3(0.20f, 0.20f, 0.20f), "mesita-TgcScene.xml"));
            listaObjetos.Add(new Objeto(new Vector3(576, 0, 179), 90, new Vector3(0.20f, 0.20f, 0.20f), "inodoro-TgcScene.xml"));
            listaObjetos.Add(new Objeto(new Vector3(59, 0, 365), 90, new Vector3(0.35f, 0.35f, 0.35f), "mesa-TgcScene.xml"));
            listaObjetos.Add(new Objeto(new Vector3(534, 0, 367), 0, new Vector3(0.4f, 0.4f, 0.4f), "escritorio-TgcScene.xml"));
            listaObjetos.Add(new Objeto(new Vector3(511, 0, 809), 270, new Vector3(0.4f, 0.4f, 0.4f), "escritorio-TgcScene.xml"));
            return listaObjetos;

        }

        public List<Escondite> initEscondites()
        {
            listaEscondites.Add(new Escondite(new Vector3(530, 0, 600), -90, new Vector3(0.5f, 0.5f, 0.5f), "Wardrobe2-TgcScene.xml", new Vector3(503, 45, 630), new Vector3(3000, 44, 4500)));
            listaEscondites.Add(new Escondite(new Vector3(815, 0, 68), -90, new Vector3(0.5f, 0.5f, 0.5f), "Wardrobe2-TgcScene.xml", new Vector3(790, 45, 100), new Vector3(90, 44, 6000)));
            listaEscondites.Add(new Escondite(new Vector3(185, 0, 473), 90, new Vector3(0.5f, 0.5f, 0.5f), "Wardrobe2-TgcScene.xml", new Vector3(209, 45, 440), new Vector3(180, 44, -90)));
            listaEscondites.Add(new Escondite(new Vector3(800, 0, 425), 90, new Vector3(0.5f, 0.5f, 0.5f), "Wardrobe2-TgcScene.xml", new Vector3(830, 45, 390), new Vector3(-90, 44, -6000)));
            listaEscondites.Add(new Escondite(new Vector3(339, 0, 163), -90, new Vector3(0.5f, 0.5f, 0.5f), "Wardrobe2-TgcScene.xml", new Vector3(328, 45, 195), new Vector3(90, 44, 6000)));
            return listaEscondites;

        }

        public List<Objeto> initFotos()
        {
            listaFotos.Add(new Objeto(new Vector3(645, 45, 384), 90, new Vector3(0.15f, 0.15f, 0.15f), "foto1-TgcScene.xml"));
            listaFotos.Add(new Objeto(new Vector3(581, 45, 23), 0, new Vector3(0.15f, 0.15f, 0.15f), "foto2-TgcScene.xml"));
            listaFotos.Add(new Objeto(new Vector3(51, 45, 918), 90, new Vector3(0.15f, 0.15f, 0.15f), "foto3-TgcScene.xml"));
            return listaFotos;
        }

        public List<Objeto> initItems()
        {
            listaItems.Add(new Objeto(new Vector3(422, 22, 870), 0, new Vector3(0.15f, 0.15f, 0.15f), "candle-TgcScene.xml"));
            listaItems.Add(new Objeto(new Vector3(211, 31, 311), 45, new Vector3(0.15f, 0.15f, 0.15f), "flashlight-TgcScene.xml"));
            listaItems.Add(new Objeto(new Vector3(935, 31, 265), 90, new Vector3(0.06f, 0.06f, 0.06f), "lantern-TgcScene.xml"));
            return listaItems;
        }

    }
}
