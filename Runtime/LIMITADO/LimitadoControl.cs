using System.Collections.Generic;
using Bounds.Cofres;
using Bounds.Global.Mazos;
using Bounds.Infraestructura;
using Bounds.Modulos.Cartas;
using Bounds.Modulos.Cartas.Ilustradores;
using Bounds.Modulos.Cartas.Persistencia;
using Bounds.Modulos.Cartas.Tinteros;
using Bounds.Modulos.Persistencia;
using Bounds.Persistencia;
using Bounds.Persistencia.Datos;
using Bounds.Persistencia.Parametros;
using Caballero.Infraestructura;
using Ging1991.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Bounds.Limitado {

	public class LimitadoControl : SingletonMonoBehaviour<LimitadoControl> {

		public int sobreActual;
		public int sobreVisible;
		private readonly int SOBRE_MINIMO = 1;
		private readonly int SOBRE_MAXIMO = 6;
		public int seleccionActual;
		public List<CartaMazo> mazoJugador = new();
		public bool debeInicializar;
		public bool debeInicializarEnemigos;
		public bool debeInicializarMiniaturas;
		public Cofre cofre;
		public GameObject contadoOBJ;
		public IlustradorDeCartas ilustradorDeCartas;
		public MusicaDeFondo musicaDeFondo;
		public ParametrosControl parametrosControl;
		private Billetera billetera;
		private Configuracion configuracion;


		void Start() {
			parametrosControl.Inicializar();
			ParametrosEscena parametros = parametrosControl.parametros;

			musicaDeFondo.Inicializar(parametros.direcciones["MUSICA_TIENDA"]);
			billetera = new(parametros.direcciones["BILLETERA"]);
			configuracion = new(parametros.direcciones["CONFIGURACION"]);
			cofre = new(parametros.direcciones["COFRE"], parametros.direcciones["COFRE_RECURSOS"]);

			ilustradorDeCartas = new IlustradorDeCartas(
				parametrosControl.parametros.direcciones["CARTAS_RECURSO"],
				parametrosControl.parametros.direcciones["CARTAS_DINAMICA"]
			);

			DatosDeCartas.Instancia.Inicializar();
			if (debeInicializarMiniaturas)
				InicializarMiniaturaVisual();

			foreach (var ocultador in FindObjectsByType<Ocultador>(FindObjectsSortMode.None)) {
				ocultador.OcultarObjeto(true);
			}

			if (debeInicializar) {
				InicializarSobre(1, ilustradorDeCartas);
				InicializarSobre(2, ilustradorDeCartas);
				InicializarSobre(3, ilustradorDeCartas);
				InicializarSobre(4, ilustradorDeCartas);
				InicializarSobre(5, ilustradorDeCartas);
				InicializarSobre(6, ilustradorDeCartas);
				sobreActual = 1;
				seleccionActual = 1;
				sobreVisible = 1;
				EstablecerTexto();
			}
			if (debeInicializarEnemigos) {
				InicializarEnemigos("PESADILLA");
				InicializarEnemigos("PIRATA");
				InicializarEnemigos("SIRENA");
			}
		}


		public void SeleccionarCarta(GameObject opcion) {

			MoverSobre(sobreVisible, LimitadoSobre.Direccion.IZQUIERDA);
			MoverSobre(sobreVisible + 1, LimitadoSobre.Direccion.CENTRO);
			SeleccionarOtrasCartas(sobreVisible);
			sobreVisible++;
			if (sobreVisible > SOBRE_MAXIMO)
				sobreVisible = SOBRE_MINIMO;
			LimitadoOpcionCarta componente = opcion.GetComponent<LimitadoOpcionCarta>();
			CartaMazo cartaMazo = new($"{componente.carta.cartaID}_{componente.carta.imagen}_{componente.rareza}_1");
			mazoJugador.Add(cartaMazo);
			InicializarMiniatura(mazoJugador.Count, componente.carta, componente.rareza);
			componente.Habilitar(false);
			seleccionActual++;
			if (seleccionActual > 6) {
				seleccionActual = 1;
				sobreActual++;
				InicializarSobre(1, ilustradorDeCartas);
				InicializarSobre(2, ilustradorDeCartas);
				InicializarSobre(3, ilustradorDeCartas);
				InicializarSobre(4, ilustradorDeCartas);
				InicializarSobre(5, ilustradorDeCartas);
				InicializarSobre(6, ilustradorDeCartas);
			}
			if (mazoJugador.Count == 20) {
				LectorLimitado lector = new LectorLimitado();
				lector.GuardarEstado("JUGANDO");
				MazoBD mazoBD = new();
				mazoBD.cartas = new();
				foreach (var carta in mazoJugador) {
					mazoBD.cartas.Add(carta.GetCodigo());
				}
				lector.GuardarMazo(mazoBD);
				ControlEscena.GetInstancia().CambiarEscena("LIMITADO SELECCION ENEMIGOS");
			}
			EstablecerTexto();
		}


		public void InicializarEnemigos(string indice) {
			LectorLimitado lector = new LectorLimitado();
			string texto = lector.LeerResultado(indice);
			texto = (texto == "") ? "JUGAR" : texto;

			GameObject objeto = GameObject.Find($"Enemigo{indice}");
			objeto.GetComponent<LimitadoEnemigo>().habilitado = (texto == "JUGAR");
			objeto.GetComponentInChildren<Text>().text = texto;
		}


		public void InicializarMiniatura(int indice, CartaColeccionBD carta, string rareza) {
			GameObject miniatura = GameObject.Find($"Miniatura{indice}");
			miniatura.GetComponent<Ocultador>().OcultarObjeto(false);
			CartaMazo cartaMazo = new($"{carta.cartaID}_{carta.imagen}_{rareza}_1");
			miniatura.GetComponentInChildren<CartaFrente>().Mostrar(carta.cartaID, carta.imagen, rareza);
		}


		public void InicializarMiniaturaVisual() {
			ITintero tintero = new TinteroBounds();
			for (int i = 1; i < 21; i++) {
				GameObject miniatura = GameObject.Find($"Miniatura{i}");
				miniatura.GetComponentInChildren<CartaFrente>().Inicializar(DatosDeCartas.Instancia, ilustradorDeCartas, tintero);
			}
		}


		public void EstablecerTexto() {
			contadoOBJ.GetComponentInChildren<Text>().text = $"Sobre {sobreActual} | Seleccion {seleccionActual}";
		}


		public void InicializarSobre(int indice, IlustradorDeCartas ilustradorDeCartas) {
			GameObject.Find("Sobre" + indice).GetComponent<LimitadoSobre>().InicializarSobre(ilustradorDeCartas, cofre);
		}


		public void BotonFinalizar() {
			LectorLimitado lector = new LectorLimitado();
			lector.GuardarEstado("");
			lector.GuardarResultado("PESADILLA", "");
			lector.GuardarResultado("PIRATA", "");
			lector.GuardarResultado("SIRENA", "");

			foreach (var carta in lector.LeerMazo().cartas) {
				cofre.AgregarCarta(new CartaCofreBD(carta));
			}
			cofre.Guardar();
			ControlEscena.GetInstancia().CambiarEscena_menu();
		}


		public void Volver() {
			SceneManager.LoadScene(parametrosControl.parametros.escenaPadre);
		}



		public void MoverSobre(int indice, LimitadoSobre.Direccion direccion) {
			if (indice > SOBRE_MAXIMO)
				indice = SOBRE_MINIMO;
			LimitadoSobre componente = GameObject.Find("Sobre" + indice).GetComponent<LimitadoSobre>();
			componente.MoverSobre(direccion);
		}


		public void SeleccionarOtrasCartas(int indice) {
			if (indice > SOBRE_MAXIMO)
				indice = SOBRE_MINIMO;
			List<int> sobres = new List<int> { 1, 2, 3, 4, 5, 6 };
			sobres.Remove(indice);
			foreach (int elemento in sobres) {
				LimitadoSobre componente = GameObject.Find("Sobre" + elemento).GetComponent<LimitadoSobre>();
				componente.SeleccionarAlAzar();
			}
		}


	}

}