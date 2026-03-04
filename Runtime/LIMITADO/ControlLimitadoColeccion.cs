using Bounds.Cofres;
using Bounds.Modulos.Cartas.Ilustradores;
using Bounds.Modulos.Persistencia;
using Bounds.Persistencia;
using Bounds.Persistencia.Parametros;
using Ging1991.Persistencia.Direcciones;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bounds.Limitado {

	public class ControlLimitadoColeccion : MonoBehaviour {

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
			Cofre cofre = new(parametros.direcciones["COFRE"], parametros.direcciones["COFRE_RECURSOS"]);

			IlustradorDeCartas ilustradorDeCartas = new(
				parametrosControl.parametros.direcciones["CARTAS_RECURSO"],
				parametrosControl.parametros.direcciones["CARTAS_DINAMICA"]
			);

			foreach (var opcion in FindObjectsOfType<LimitadoOpcionColeccion>()) {
				Coleccion coleccion = new Coleccion(opcion.codigo, new DireccionRecursos("COLECCIONES", opcion.codigo).Generar());
				opcion.Inicializar(cofre, coleccion, billetera, ilustradorDeCartas);
			}

		}


		public void Volver() {
			SceneManager.LoadScene(parametrosControl.parametros.escenaPadre);
		}


	}

}