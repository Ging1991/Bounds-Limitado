using Bounds.Cofres;
using Bounds.Modulos.Cartas;
using Bounds.Modulos.Cartas.Ilustradores;
using Bounds.Modulos.Cartas.Persistencia;
using Bounds.Modulos.Cartas.Persistencia.Datos;
using Bounds.Modulos.Cartas.Tinteros;
using Bounds.Persistencia.Datos;
using Ging1991.Core.Interfaces;
using Ging1991.Interfaces.Contadores;
using UnityEngine;

namespace Bounds.Limitado {

	public class LimitadoOpcionCarta : MonoBehaviour {

		public CartaColeccionBD carta;
		public GameObject contadorOBJ;
		public string rareza;

		public void Inicializar(CartaColeccionBD carta, Cofre cofre, string rareza, IProveedor<int, CartaBD> proveedorCartas, IlustradorDeCartas ilustradorDeCartas, ITintero tintero) {
			this.carta = carta;
			this.rareza = rareza;
			Habilitar(true);
			GetComponentInChildren<CartaFrente>().Inicializar(proveedorCartas, ilustradorDeCartas, tintero);
			GetComponentInChildren<CartaFrente>().Mostrar(carta.cartaID, carta.imagen, rareza);
			contadorOBJ.GetComponent<ContadorNumero>().SetColorRelleno(Color.red);
			int cantidad = cofre.GetCantidadCartasDiferentes(carta.cartaID);
			contadorOBJ.GetComponent<ContadorNumero>().SetValor(cantidad);
			if (cantidad > 1) {
				contadorOBJ.GetComponent<ContadorNumero>().SetColorRelleno(Color.yellow);
			}
			if (cantidad >= 5) {
				contadorOBJ.GetComponent<ContadorNumero>().SetColorRelleno(Color.green);
			}
		}


		public void Habilitar(bool habilitar) {
			transform.GetChild(0).gameObject.SetActive(habilitar);
			GetComponent<BoxCollider2D>().enabled = habilitar;
			contadorOBJ.SetActive(habilitar);
		}


		void OnMouseDown() {
			GameObject control = GameObject.Find("Control");
			control.GetComponent<LimitadoControl>().SeleccionarCarta(gameObject);
		}


	}

}