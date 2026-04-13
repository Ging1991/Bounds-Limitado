using Bounds.Cofres;
using Bounds.Entrenamiento;
using Bounds.Modulos.Cartas.Ilustradores;
using Bounds.Modulos.Cartas.Persistencia;
using Bounds.Modulos.Cartas.Persistencia.Datos;
using Bounds.Musica;
using Bounds.Persistencia;
using Bounds.Persistencia.Parametros;
using Ging1991.Core;
using Ging1991.Core.Interfaces;
using Ging1991.Persistencia.Direcciones;
using Ging1991.Ventanas;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bounds.Limitado {

	public class ControlLimitadoColeccion : SingletonMonoBehaviour<ControlLimitadoColeccion> {

		public MusicaDeFondo musicaDeFondo;
		public ParametrosControl parametrosControl;
		public IProveedor<int, CartaBD> proveedorCartas;
		public VentanaControl ventanaControl;
		private Billetera billetera;
		private Configuracion configuracion;
		public PersonalizarUI personalizarUI;

		void Start() {
			personalizarUI.Personalizar();
			parametrosControl.Inicializar();
			ParametrosEscena parametros = parametrosControl.parametros;
			proveedorCartas = new LectorCartas(new DireccionRecursos(parametrosControl.parametros.direcciones["CARTAS_DATOS"]));

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
				opcion.Inicializar(proveedorCartas, cofre, coleccion, billetera, ilustradorDeCartas);
			}

		}


		public void Volver() {
			SceneManager.LoadScene(parametrosControl.parametros.escenaPadre);
		}


	}

}