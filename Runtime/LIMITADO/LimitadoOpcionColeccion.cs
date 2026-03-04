using System.Collections.Generic;
using Bounds.Cofres;
using Bounds.Infraestructura;
using Bounds.Modulos.Cartas.Tinteros;
using Bounds.Persistencia;
using Bounds.Persistencia.Datos;
using Bounds.Tienda;
using Ging1991.Core.Interfaces;
using Ging1991.Interfaces;
using Ging1991.Persistencia.Direcciones;
using Ging1991.Ventanas;
using UnityEngine;
using UnityEngine.UI;

namespace Bounds.Limitado {

	public class LimitadoOpcionColeccion : MonoBehaviour, IPresionarBoton {

		public int precio;
		public GameObject precioOBJ;
		public GameObject nombreOBJ;
		public GameObject posesionOBJ;
		public Coleccion coleccion;
		public string codigo;
		protected readonly int INDICE_ILUSTRACION = 0;
		protected readonly int INDICE_NOMBRE = 1;
		public ITintero tintero;
		private Cofre cofre;
		private Billetera billetera;
		public Image ilustracionOBJ;


		public void Inicializar(Cofre cofre, Coleccion coleccion, Billetera billetera, IProveedor<string, Sprite> ilustrador) {
			this.cofre = cofre;
			this.coleccion = coleccion;
			this.billetera = billetera;
			tintero = new TinteroBounds();
			precioOBJ.GetComponent<MarcoConTexto>().SetTexto($"${precio}");
			precioOBJ.GetComponent<MarcoConTexto>().SetColorRelleno(Color.yellow);
			nombreOBJ.GetComponent<MarcoConTexto>().SetTexto($"{coleccion.titulo}");
			posesionOBJ.GetComponent<MarcoConTexto>().SetTexto($"{EstablecerPosesion()}");
			GetComponent<ContenedorDeCartas>()?.Inicializar(ilustrador, tintero, coleccion.emblema.cartaID);
		}


		protected string EstablecerPosesion() {
			List<CartaColeccionBD> cartas = coleccion.GetListaCompleta();
			List<int> cartasID = new List<int>();
			foreach (var carta in cartas) {
				if (!cartasID.Contains(carta.cartaID))
					cartasID.Add(carta.cartaID);
			}
			int cartasObtenidas = cofre.GetCantidadCartasDiferentes(cartasID);
			int cartasTotales = cartas.Count;
			int porcentaje = (int)(((float)cartasObtenidas / cartasTotales) * 100);
			return $"{cartasObtenidas}/{cartasTotales} ({porcentaje}%)";
		}


		void OnMouseDown() {

			if (billetera.LeerOro() >= precio) {
				VentanaControl.CrearVentanaConfirmar($"¿Desea comprar el pase de limitado por ${precio}?", this);
			}
			else {
				VentanaControl.CrearVentanaAceptar("No tiene suficiente oro: $" + billetera.LeerOro());
			}
		}


		public void PresionarBoton(TipoBoton boton) {
			if (boton == TipoBoton.ACEPTAR) {
				billetera.GastarOro(precio);

				//EfectosDeSonido.Tocar("FxAdquisicion");
				LectorLimitado lector = new LectorLimitado();
				lector.GuardarColeccion(coleccion.codigo);
				lector.GuardarEstado("PAGADO");
				ControlEscena.GetInstancia().CambiarEscena("LIMITADO SELECCION CARTAS");
			}
			else {
				//EfectosDeSonido.Tocar("FxRebote");
			}
		}


	}

}