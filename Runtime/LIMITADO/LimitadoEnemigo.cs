using System.Collections.Generic;
using Bounds.Global;
using Bounds.Global.Mazos;
using Bounds.Infraestructura;
using Bounds.Persistencia;
using Bounds.Persistencia.Datos;
using Ging1991.Persistencia.Direcciones;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bounds.Limitado {

	public class LimitadoEnemigo : MonoBehaviour {

		public string personaje;
		public string nombre;
		public bool habilitado;

		void OnMouseDown() {
			if (habilitado) {
				GlobalDuelo parametros = GlobalDuelo.GetInstancia();

				parametros.modo = "LIMITADO";
				parametros.finalizarDuelo = new Fin();

				parametros.jugadorLP1 = 2000;
				parametros.jugadorLP2 = 2000;

				parametros.jugadorMiniatura1 = "LAUNIX";
				parametros.jugadorMiniatura2 = personaje;

				parametros.jugadorNombre1 = "Launix";
				parametros.jugadorNombre2 = nombre;

				parametros.mazoVacio1 = null;
				parametros.mazoVacio2 = null;
				LectorLimitado lector = new LectorLimitado();
				List<string> cartas = new List<string>();
				foreach (var carta in lector.LeerMazo().cartas)
					cartas.Add(carta);

				parametros.mazo1 = Mazo.ConvertirCartas(cartas);
				parametros.mazo2 = Mazo.ConvertirCartas(cartas);
				SceneManager.LoadScene("DUELO");
			}
		}


		public class Fin : IFinalizarDuelo {
			public void FinalizarDuelo(int jugadorGanador) {
				Configuracion configuracion = new Configuracion(new DireccionDinamica("CONFIGURACION", "CONFIGURACION.json").Generar());
				GlobalDuelo parametros = GlobalDuelo.GetInstancia();
				LectorLimitado lector = new LectorLimitado();
				lector.GuardarResultado(parametros.jugadorMiniatura2, jugadorGanador == 1 ? "VICTORIA" : "DERROTA");
				Billetera billetera = new(new DireccionDinamica("CONFIGURACION", "BILLETERA.json").Generar());
				billetera.GanarOro(300);
				ControlEscena escena = ControlEscena.GetInstancia();
				escena.CambiarEscena("LIMITADO SELECCION ENEMIGOS");
			}

		}



	}

}