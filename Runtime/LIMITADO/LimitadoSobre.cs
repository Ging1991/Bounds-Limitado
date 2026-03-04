using System.Collections.Generic;
using Bounds.Cofres;
using Bounds.Modulos.Cartas.Ilustradores;
using Bounds.Modulos.Cartas.Persistencia;
using Bounds.Modulos.Cartas.Tinteros;
using Bounds.Persistencia;
using Bounds.Persistencia.Datos;
using Ging1991.Core;
using Ging1991.Core.Interfaces;
using Ging1991.Core.Movimiento;
using Ging1991.Persistencia.Direcciones;
using UnityEngine;

namespace Bounds.Limitado {

	public class LimitadoSobre : MonoBehaviour, IEjecutable {

		public enum Direccion {
			CENTRO,
			DERECHA,
			IZQUIERDA
		}

		private Vector3 POSICION_CENTRAL = new Vector3(0, 75, 0);
		private Vector3 POSICION_IZQUIERDA = new Vector3(-1800, 75, 0);
		private Vector3 POSICION_DERECHA = new Vector3(1300, 75, 0);
		private Direccion direccionActual = Direccion.CENTRO;
		public IlustradorDeCartas ilustradorDeCartas;
		public ITintero tintero;


		public void InicializarSobre(IlustradorDeCartas ilustradorDeCartas, Cofre cofre) {
			this.ilustradorDeCartas = ilustradorDeCartas;
			LectorLimitado lectorLimitado = new();
			Coleccion coleccion = new(lectorLimitado.LeerColeccion(), new DireccionRecursos("COLECCIONES", lectorLimitado.LeerColeccion()).Generar());
			Sobre sobre = coleccion.CrearSobre();
			tintero = new TinteroBounds();

			InicializarOpcionCarta(1, sobre.rara, cofre, sobre.rarezaSobre);
			InicializarOpcionCarta(2, sobre.infrecuentes[0], cofre, "PLA");
			InicializarOpcionCarta(3, sobre.infrecuentes[1], cofre, "PLA");
			InicializarOpcionCarta(4, sobre.comunes[0], cofre, "N");
			InicializarOpcionCarta(5, sobre.comunes[1], cofre, "N");
			InicializarOpcionCarta(6, sobre.comunes[2], cofre, "N");
		}


		public void InicializarOpcionCarta(int indice, CartaColeccionBD carta, Cofre cofre, string rareza) {
			GameObject opcion = transform.GetChild(indice).gameObject;
			opcion.GetComponent<LimitadoOpcionCarta>().Inicializar(carta, cofre, rareza, DatosDeCartas.Instancia, ilustradorDeCartas, tintero);
		}


		public void SeleccionarAlAzar() {
			List<int> cartasDisponibles = new List<int>();
			for (int i = 1; i < 7; i++) {
				GameObject carta = transform.GetChild(i).gameObject;
				if (carta.transform.GetChild(0).gameObject.activeSelf)
					cartasDisponibles.Add(i);
			}
			GameObject cartaElegida = transform.GetChild(Azar<int>.ValorAleatorio(cartasDisponibles)).gameObject;
			cartaElegida.GetComponent<LimitadoOpcionCarta>().Habilitar(false);
		}


		public void MoverSobre(Direccion direccion) {
			direccionActual = direccion;
			int velocidad = 15;
			PosicionamientoLocal posicionamiento = GetComponent<PosicionamientoLocal>();
			if (direccion == Direccion.DERECHA) {
				transform.localPosition = POSICION_DERECHA;
			}
			if (direccion == Direccion.CENTRO) {
				Bloqueador.BloquearGrupo("GLOBAL", true);
				posicionamiento.Posicionar(POSICION_CENTRAL, velocidad, accion: this);
			}
			if (direccion == Direccion.IZQUIERDA) {
				posicionamiento.Posicionar(POSICION_IZQUIERDA, velocidad, accion: this);
			}
		}


		public void Ejecutar() {
			if (direccionActual == Direccion.CENTRO)
				Bloqueador.BloquearGrupo("GLOBAL", false);
			if (direccionActual == Direccion.IZQUIERDA)
				MoverSobre(Direccion.DERECHA);
		}


	}

}